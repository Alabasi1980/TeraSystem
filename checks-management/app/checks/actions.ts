'use server'

import { prisma } from '@/lib/prisma'
import { revalidatePath } from 'next/cache'
import { getSession, requireAdmin } from '@/lib/auth'
import type { CheckType, CheckStatus } from '@prisma/client'

// ---------------------------------------------------------------------------
// Exported types
// ---------------------------------------------------------------------------

export type DropdownItem = {
  id: number
  name: string
}

export type CheckItem = {
  id: number
  checkNumber: string
  type: string
  bankId: number
  bankName: string
  partyId: number
  partyName: string
  amount: number
  issueDate: Date
  dueDate: Date
  status: string
  statusNote: string | null
  statusChangedAt: Date | null
  notes: string | null
  createdById: number
  createdByUsername: string
  createdAt: Date
}

export type CheckFormData = {
  type: 'ISSUED' | 'INCOMING'
  bankId: number
  partyId: number
  checkNumber: string
  amount: number
  issueDate: string // YYYY-MM-DD
  dueDate: string // YYYY-MM-DD
  notes?: string
}

export type CheckSummary = {
  totalCount: number
  totalAmount: number
  dueSoonCount: number
  returnedCount: number
}

export type CheckFilters = {
  type?: CheckType
  status?: CheckStatus
  bankId?: number
  partyId?: number
  dueDateFrom?: string // YYYY-MM-DD
  dueDateTo?: string // YYYY-MM-DD
  search?: string
}

// ---------------------------------------------------------------------------
// Internal helpers
// ---------------------------------------------------------------------------

type NormalizedCheckData = {
  type: 'ISSUED' | 'INCOMING'
  bankId: number
  partyId: number
  checkNumber: string
  amount: number
  issueDate: string
  dueDate: string
  notes: string | null
}

function trimValue(value: string | undefined): string {
  return typeof value === 'string' ? value.trim() : ''
}

/**
 * Require any authenticated session (not necessarily admin).
 * Returns the session on success, or an error object compatible with action return types.
 */
async function requireSession(): Promise<
  { userId: number; username: string; role: string } | { error: string }
> {
  const session = await getSession()
  if (!session) return { error: 'يجب تسجيل الدخول أولاً' }
  return session
}

/**
 * Validate check form data.
 */
function validateCheckData(
  data: CheckFormData
): NormalizedCheckData | { error: string } {
  const checkNumber = trimValue(data.checkNumber)
  const notes = trimValue(data.notes)
  const amount = data.amount

  if (!checkNumber) return { error: 'رقم الشيك مطلوب' }
  if (checkNumber.length > 50)
    return { error: 'رقم الشيك يجب ألا يتجاوز 50 حرف' }
  if (typeof amount !== 'number' || isNaN(amount) || amount <= 0) {
    return { error: 'المبلغ يجب أن يكون أكبر من 0' }
  }
  if (notes.length > 500)
    return { error: 'الملاحظات يجب ألا تتجاوز 500 حرف' }

  // Validate dates
  if (!data.issueDate) return { error: 'تاريخ الإصدار مطلوب' }
  if (!data.dueDate) return { error: 'تاريخ الاستحقاق مطلوب' }

  const parsedIssueDate = new Date(data.issueDate)
  const parsedDueDate = new Date(data.dueDate)

  if (isNaN(parsedIssueDate.getTime())) {
    return { error: 'تاريخ الإصدار غير صحيح' }
  }
  if (isNaN(parsedDueDate.getTime())) {
    return { error: 'تاريخ الاستحقاق غير صحيح' }
  }

  // Compare date-only values
  const issueStr = parsedIssueDate.toISOString().split('T')[0]
  const dueStr = parsedDueDate.toISOString().split('T')[0]
  if (dueStr < issueStr) {
    return { error: 'تاريخ الاستحقاق يجب أن يكون أكبر من أو يساوي تاريخ الإصدار' }
  }

  return {
    type: data.type,
    bankId: data.bankId,
    partyId: data.partyId,
    checkNumber,
    amount,
    issueDate: issueStr,
    dueDate: dueStr,
    notes: notes || null,
  }
}

// Valid transitions: Map<current status, allowed next statuses>
const VALID_TRANSITIONS: Record<string, string[]> = {
  REGISTERED: ['IN_HAND', 'CANCELLED'],
  IN_HAND: ['CASHED', 'RETURNED', 'CANCELLED'],
  CASHED: [],
  RETURNED: ['IN_HAND', 'CASHED', 'CANCELLED'],
  CANCELLED: [],
}

function isValidTransition(
  current: string,
  next: string
): boolean {
  const allowed = VALID_TRANSITIONS[current]
  return allowed ? allowed.includes(next) : false
}

// ---------------------------------------------------------------------------
// Server Actions
// ---------------------------------------------------------------------------

/**
 * List checks with optional filters.
 */
export async function listChecks(
  filters?: CheckFilters
): Promise<CheckItem[] | { error: string }> {
  const session = await requireSession()
  if ('error' in session) return session

  const where: Record<string, unknown> = {}

  if (filters) {
    if (filters.type) where.type = filters.type
    if (filters.status) where.status = filters.status
    if (filters.bankId) where.bankId = filters.bankId
    if (filters.partyId) where.partyId = filters.partyId

    if (filters.dueDateFrom || filters.dueDateTo) {
      const dueDateFilter: Record<string, Date> = {}
      if (filters.dueDateFrom) dueDateFilter.gte = new Date(filters.dueDateFrom)
      if (filters.dueDateTo) {
        // Include the full day by setting to end of day
        const end = new Date(filters.dueDateTo)
        end.setHours(23, 59, 59, 999)
        dueDateFilter.lte = end
      }
      where.dueDate = dueDateFilter
    }

    if (filters.search) {
      const search = filters.search
      where.OR = [
        { checkNumber: { contains: search, mode: 'insensitive' } },
        { party: { name: { contains: search, mode: 'insensitive' } } },
      ]
    }
  }

  const checks = await prisma.check.findMany({
    where,
    include: {
      bank: { select: { id: true, name: true } },
      party: { select: { id: true, name: true } },
      createdBy: { select: { id: true, username: true } },
    },
    orderBy: { createdAt: 'desc' },
  })

  return checks.map((c) => ({
    id: c.id,
    checkNumber: c.checkNumber,
    type: c.type,
    bankId: c.bank.id,
    bankName: c.bank.name,
    partyId: c.party.id,
    partyName: c.party.name,
    amount: Number(c.amount),
    issueDate: c.issueDate,
    dueDate: c.dueDate,
    status: c.status,
    statusNote: c.statusNote,
    statusChangedAt: c.statusChangedAt,
    notes: c.notes,
    createdById: c.createdBy.id,
    createdByUsername: c.createdBy.username,
    createdAt: c.createdAt,
  }))
}

/**
 * Get an aggregated summary of all checks.
 */
export async function getCheckSummary(): Promise<
  CheckSummary | { error: string }
> {
  const session = await requireSession()
  if ('error' in session) return session

  const totalCount = await prisma.check.count()

  const aggregation = await prisma.check.aggregate({
    _sum: { amount: true },
  })
  const totalAmount = Number(aggregation._sum.amount ?? 0)

  const today = new Date()
  const sevenDaysLater = new Date()
  sevenDaysLater.setDate(today.getDate() + 7)

  const dueSoonCount = await prisma.check.count({
    where: {
      dueDate: {
        gte: today,
        lte: sevenDaysLater,
      },
      status: { notIn: ['CASHED', 'CANCELLED'] },
    },
  })

  const returnedCount = await prisma.check.count({
    where: { status: 'RETURNED' },
  })

  return { totalCount, totalAmount, dueSoonCount, returnedCount }
}

/**
 * Get banks for a dropdown selector.
 */
export async function getBanksForDropdown(): Promise<
  DropdownItem[] | { error: string }
> {
  const session = await requireSession()
  if ('error' in session) return session

  const banks = await prisma.bank.findMany({
    orderBy: { name: 'asc' },
    select: { id: true, name: true },
  })

  return banks
}

/**
 * Get parties for a dropdown selector.
 */
export async function getPartiesForDropdown(): Promise<
  DropdownItem[] | { error: string }
> {
  const session = await requireSession()
  if ('error' in session) return session

  const parties = await prisma.party.findMany({
    orderBy: { name: 'asc' },
    select: { id: true, name: true },
  })

  return parties
}

/**
 * Create a new check.
 */
export async function createCheck(
  data: CheckFormData
): Promise<{ success: true } | { error: string }> {
  const session = await requireSession()
  if ('error' in session) return session

  const validated = validateCheckData(data)
  if ('error' in validated) return validated

  // Verify bank exists
  const bank = await prisma.bank.findUnique({
    where: { id: validated.bankId },
  })
  if (!bank) return { error: 'البنك المحدد غير موجود' }

  // Verify party exists
  const party = await prisma.party.findUnique({
    where: { id: validated.partyId },
  })
  if (!party) return { error: 'الجهة المحددة غير موجودة' }

  await prisma.check.create({
    data: {
      checkNumber: validated.checkNumber,
      type: validated.type as CheckType,
      bankId: validated.bankId,
      partyId: validated.partyId,
      amount: validated.amount,
      issueDate: new Date(validated.issueDate),
      dueDate: new Date(validated.dueDate),
      status: 'REGISTERED',
      notes: validated.notes,
      createdById: session.userId,
    },
  })

  revalidatePath('/checks')
  return { success: true }
}

/**
 * Update an existing check (only if not in a final state).
 */
export async function updateCheck(
  id: number,
  data: CheckFormData
): Promise<{ success: true } | { error: string }> {
  const session = await requireSession()
  if ('error' in session) return session

  const existing = await prisma.check.findUnique({ where: { id } })
  if (!existing) return { error: 'الشيك غير موجود' }

  if (existing.status === 'CASHED' || existing.status === 'CANCELLED') {
    return { error: 'لا يمكن تعديل شيك في حالة نهائية' }
  }

  const validated = validateCheckData(data)
  if ('error' in validated) return validated

  // Verify bank exists
  const bank = await prisma.bank.findUnique({
    where: { id: validated.bankId },
  })
  if (!bank) return { error: 'البنك المحدد غير موجود' }

  // Verify party exists
  const party = await prisma.party.findUnique({
    where: { id: validated.partyId },
  })
  if (!party) return { error: 'الجهة المحددة غير موجودة' }

  await prisma.check.update({
    where: { id },
    data: {
      checkNumber: validated.checkNumber,
      type: validated.type as CheckType,
      bankId: validated.bankId,
      partyId: validated.partyId,
      amount: validated.amount,
      issueDate: new Date(validated.issueDate),
      dueDate: new Date(validated.dueDate),
      notes: validated.notes,
    },
  })

  revalidatePath('/checks')
  return { success: true }
}

/**
 * Update the status of a check with optional status note.
 * Validates the transition according to business rules.
 */
export async function updateCheckStatus(
  id: number,
  newStatus: string,
  statusNote?: string
): Promise<{ success: true } | { error: string }> {
  const session = await requireSession()
  if ('error' in session) return session

  const existing = await prisma.check.findUnique({ where: { id } })
  if (!existing) return { error: 'الشيك غير موجود' }

  if (!isValidTransition(existing.status, newStatus)) {
    return { error: 'لا يمكن تغيير الحالة إلى الحالة المطلوبة' }
  }

  const note = trimValue(statusNote ?? '')

  await prisma.check.update({
    where: { id },
    data: {
      status: newStatus as CheckStatus,
      statusNote: note || null,
      statusChangedAt: new Date(),
    },
  })

  revalidatePath('/checks')
  return { success: true }
}

/**
 * Delete a check. Requires Admin role.
 */
export async function deleteCheck(
  id: number
): Promise<{ success: true } | { error: string }> {
  const session = await requireAdmin()
  if ('error' in session) return session

  const existing = await prisma.check.findUnique({ where: { id } })
  if (!existing) return { error: 'الشيك غير موجود' }

  if (existing.status === 'CASHED' || existing.status === 'CANCELLED') {
    return { error: 'لا يمكن حذف شيك في حالة نهائية' }
  }

  await prisma.check.delete({ where: { id } })

  revalidatePath('/checks')
  return { success: true }
}

/**
 * Get a single check by ID.
 */
export async function getCheckById(
  id: number
): Promise<CheckItem | { error: string }> {
  const session = await requireSession()
  if ('error' in session) return session

  const check = await prisma.check.findUnique({
    where: { id },
    include: {
      bank: { select: { id: true, name: true } },
      party: { select: { id: true, name: true } },
      createdBy: { select: { id: true, username: true } },
    },
  })

  if (!check) return { error: 'الشيك غير موجود' }

  return {
    id: check.id,
    checkNumber: check.checkNumber,
    type: check.type,
    bankId: check.bank.id,
    bankName: check.bank.name,
    partyId: check.party.id,
    partyName: check.party.name,
    amount: Number(check.amount),
    issueDate: check.issueDate,
    dueDate: check.dueDate,
    status: check.status,
    statusNote: check.statusNote,
    statusChangedAt: check.statusChangedAt,
    notes: check.notes,
    createdById: check.createdBy.id,
    createdByUsername: check.createdBy.username,
    createdAt: check.createdAt,
  }
}
