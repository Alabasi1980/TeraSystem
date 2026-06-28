'use server'

import { prisma } from '@/lib/prisma'
import { revalidatePath } from 'next/cache'
import { requireAdmin } from '@/lib/auth'
import bcrypt from 'bcryptjs'

export type UserItem = {
  id: number
  username: string
  displayName: string
  role: string
  isActive: boolean
  checkCount: number
  createdAt: Date
}

export type UserFormData = {
  username: string
  password?: string
  displayName: string
  role: 'ADMIN' | 'USER'
  isActive?: boolean
}

export type UserDetail = {
  id: number
  username: string
  displayName: string
  role: string
  isActive: boolean
  checkCount: number
  createdAt: Date
}

function trimValue(value: string | undefined): string {
  return typeof value === 'string' ? value.trim() : ''
}

export async function listUsers(): Promise<UserItem[] | { error: string }> {
  const session = await requireAdmin()
  if ('error' in session) return session

  const users = await prisma.user.findMany({
    orderBy: { username: 'asc' },
    include: { _count: { select: { checks: true } } },
  })

  return users.map((u) => ({
    id: u.id,
    username: u.username,
    displayName: u.displayName,
    role: u.role,
    isActive: u.isActive,
    checkCount: u._count.checks,
    createdAt: u.createdAt,
  }))
}

export async function createUser(
  data: UserFormData
): Promise<{ success: true } | { error: string }> {
  const session = await requireAdmin()
  if ('error' in session) return session

  const username = trimValue(data.username)
  const displayName = trimValue(data.displayName)
  const password = data.password

  // Validate username
  if (!username) return { error: 'اسم المستخدم مطلوب' }
  if (username.length > 50) return { error: 'اسم المستخدم يجب ألا يتجاوز 50 حرف' }

  // Check username uniqueness
  const existingUsername = await prisma.user.findUnique({
    where: { username },
  })
  if (existingUsername) return { error: 'اسم المستخدم موجود مسبقًا' }

  // Validate password (required on create)
  if (!password) return { error: 'كلمة المرور مطلوبة' }
  if (password.length < 6) return { error: 'كلمة المرور يجب أن تكون 6 أحرف على الأقل' }

  // Validate displayName
  if (!displayName) return { error: 'الاسم المعروض مطلوب' }
  if (displayName.length > 100) return { error: 'الاسم المعروض يجب ألا يتجاوز 100 حرف' }

  const salt = await bcrypt.genSalt(10)
  const passwordHash = await bcrypt.hash(password, salt)

  await prisma.user.create({
    data: {
      username,
      passwordHash,
      displayName,
      role: data.role,
      isActive: data.isActive ?? true,
    },
  })

  revalidatePath('/users')
  return { success: true }
}

export async function updateUser(
  id: number,
  data: UserFormData
): Promise<{ success: true } | { error: string }> {
  const session = await requireAdmin()
  if ('error' in session) return session

  const username = trimValue(data.username)
  const displayName = trimValue(data.displayName)
  const password = data.password

  // Validate username
  if (!username) return { error: 'اسم المستخدم مطلوب' }
  if (username.length > 50) return { error: 'اسم المستخدم يجب ألا يتجاوز 50 حرف' }

  // Check username uniqueness (exclude current user)
  const existingUsername = await prisma.user.findFirst({
    where: { username, id: { not: id } },
  })
  if (existingUsername) return { error: 'اسم المستخدم موجود مسبقًا' }

  // Validate displayName
  if (!displayName) return { error: 'الاسم المعروض مطلوب' }
  if (displayName.length > 100) return { error: 'الاسم المعروض يجب ألا يتجاوز 100 حرف' }

  // Build update data
  const updateData: {
    username: string
    displayName: string
    role: 'ADMIN' | 'USER'
    passwordHash?: string
    isActive?: boolean
  } = {
    username,
    displayName,
    role: data.role,
  }

  // If password provided and non-empty: validate and re-hash
  if (password && password.trim().length > 0) {
    if (password.length < 6) return { error: 'كلمة المرور يجب أن تكون 6 أحرف على الأقل' }
    const salt = await bcrypt.genSalt(10)
    updateData.passwordHash = await bcrypt.hash(password, salt)
  }

  // Handle isActive if provided
  if (data.isActive !== undefined) {
    updateData.isActive = data.isActive
  }

  // SELF-PROTECTION
  if (id === session.userId) {
    // Do NOT allow changing own role from ADMIN to USER
    if (data.role === 'USER') {
      return { error: 'لا يمكنك تغيير صلاحيتك من مدير إلى مستخدم' }
    }
    // Do NOT allow deactivating own account
    updateData.isActive = true
  }

  await prisma.user.update({
    where: { id },
    data: updateData,
  })

  revalidatePath('/users')
  return { success: true }
}

export async function toggleUserActive(
  id: number
): Promise<{ success: true } | { error: string }> {
  const session = await requireAdmin()
  if ('error' in session) return session

  // SELF-PROTECTION: cannot deactivate own account
  if (id === session.userId) {
    return { error: 'لا يمكن تعطيل حسابك الخاص' }
  }

  const user = await prisma.user.findUnique({ where: { id } })
  if (!user) return { error: 'المستخدم غير موجود' }

  await prisma.user.update({
    where: { id },
    data: { isActive: !user.isActive },
  })

  revalidatePath('/users')
  return { success: true }
}

export async function getUserById(
  id: number
): Promise<UserDetail | { error: string }> {
  const session = await requireAdmin()
  if ('error' in session) return session

  const user = await prisma.user.findUnique({
    where: { id },
    include: { _count: { select: { checks: true } } },
  })

  if (!user) return { error: 'المستخدم غير موجود' }

  return {
    id: user.id,
    username: user.username,
    displayName: user.displayName,
    role: user.role,
    isActive: user.isActive,
    checkCount: user._count.checks,
    createdAt: user.createdAt,
  }
}
