'use client'

import { useState, useEffect, useCallback } from 'react'
import { listUsers, createUser, updateUser, toggleUserActive } from './actions'
import { getCurrentUser } from '../actions'
import type { UserItem, UserFormData } from './actions'

const styles: Record<string, React.CSSProperties> = {
  page: {
    minHeight: '100vh',
    backgroundColor: '#F8FAFC',
    padding: '24px',
    fontFamily: 'system-ui, Arial, sans-serif',
    direction: 'rtl',
  },
  headerRow: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: '24px',
    maxWidth: '1100px',
    marginLeft: 'auto',
    marginRight: 'auto',
  },
  pageTitle: {
    fontSize: '22px',
    fontWeight: 700,
    color: '#111827',
    margin: 0,
  },
  addButton: {
    backgroundColor: '#2563EB',
    color: '#FFFFFF',
    fontWeight: 500,
    padding: '10px 20px',
    border: 'none',
    borderRadius: '8px',
    fontSize: '14px',
    cursor: 'pointer',
  },
  card: {
    maxWidth: '1100px',
    marginLeft: 'auto',
    marginRight: 'auto',
    backgroundColor: '#FFFFFF',
    borderRadius: '8px',
    border: '1px solid #E5E7EB',
    overflow: 'hidden',
  },
  table: {
    width: '100%',
    borderCollapse: 'collapse' as const,
    fontSize: '14px',
  },
  th: {
    backgroundColor: '#F3F4F6',
    color: '#6B7280',
    fontWeight: 600,
    textAlign: 'start' as const,
    padding: '12px 16px',
    borderBottom: '1px solid #E5E7EB',
    fontSize: '13px',
    textTransform: 'uppercase' as const,
    letterSpacing: '0.05em',
  },
  td: {
    padding: '12px 16px',
    borderBottom: '1px solid #E5E7EB',
    color: '#111827',
  },
  actionButton: {
    padding: '6px 14px',
    border: 'none',
    borderRadius: '6px',
    fontSize: '13px',
    cursor: 'pointer',
    marginLeft: '8px',
    fontWeight: 500,
  },
  editButton: {
    backgroundColor: '#EFF6FF',
    color: '#2563EB',
  },
  toggleActiveButton: {
    backgroundColor: '#DCFCE7',
    color: '#166534',
  },
  toggleInactiveButton: {
    backgroundColor: '#FEF2F2',
    color: '#DC2626',
  },
  toggleDisabled: {
    backgroundColor: '#F3F4F6',
    color: '#9CA3AF',
    cursor: 'not-allowed',
    padding: '6px 14px',
    borderRadius: '6px',
    fontSize: '13px',
    marginLeft: '8px',
    fontWeight: 500,
    display: 'inline-block',
  },
  badge: {
    display: 'inline-block',
    padding: '2px 10px',
    borderRadius: '12px',
    fontSize: '13px',
    fontWeight: 500,
  },
  badgeAdmin: {
    backgroundColor: '#DBEAFE',
    color: '#1D4ED8',
  },
  badgeUser: {
    backgroundColor: '#F3F4F6',
    color: '#374151',
  },
  badgeActive: {
    backgroundColor: '#DCFCE7',
    color: '#166534',
  },
  badgeInactive: {
    backgroundColor: '#F3F4F6',
    color: '#6B7280',
  },
  emptyState: {
    padding: '60px 24px',
    textAlign: 'center' as const,
    color: '#6B7280',
  },
  emptyTitle: {
    fontSize: '16px',
    fontWeight: 600,
    color: '#111827',
    marginBottom: '8px',
  },
  emptyText: {
    fontSize: '14px',
    marginBottom: '24px',
  },
  emptyAddButton: {
    backgroundColor: '#2563EB',
    color: '#FFFFFF',
    fontWeight: 500,
    padding: '10px 20px',
    border: 'none',
    borderRadius: '8px',
    fontSize: '14px',
    cursor: 'pointer',
  },
  overlay: {
    position: 'fixed' as const,
    top: 0,
    left: 0,
    right: 0,
    bottom: 0,
    backgroundColor: 'rgba(0, 0, 0, 0.5)',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    zIndex: 1000,
    padding: '16px',
  },
  modal: {
    backgroundColor: '#FFFFFF',
    borderRadius: '12px',
    width: '100%',
    maxWidth: '480px',
    padding: '28px',
    boxShadow: '0 4px 20px rgba(0, 0, 0, 0.15)',
  },
  modalTitle: {
    fontSize: '18px',
    fontWeight: 700,
    color: '#111827',
    margin: 0,
    marginBottom: '24px',
  },
  fieldGroup: {
    marginBottom: '20px',
  },
  label: {
    display: 'block',
    fontSize: '14px',
    fontWeight: 500,
    color: '#111827',
    marginBottom: '6px',
  },
  requiredStar: {
    color: '#DC2626',
    marginRight: '2px',
  },
  input: {
    width: '100%',
    padding: '10px 14px',
    border: '1px solid #E5E7EB',
    borderRadius: '8px',
    fontSize: '14px',
    color: '#111827',
    outline: 'none',
    boxSizing: 'border-box' as const,
  },
  select: {
    width: '100%',
    padding: '10px 14px',
    border: '1px solid #E5E7EB',
    borderRadius: '8px',
    fontSize: '14px',
    color: '#111827',
    outline: 'none',
    boxSizing: 'border-box' as const,
    backgroundColor: '#FFFFFF',
  },
  checkboxGroup: {
    display: 'flex',
    alignItems: 'center',
    gap: '8px',
    marginBottom: '20px',
  },
  checkbox: {
    width: '18px',
    height: '18px',
    cursor: 'pointer',
  },
  checkboxLabel: {
    fontSize: '14px',
    fontWeight: 500,
    color: '#111827',
    cursor: 'pointer',
  },
  formError: {
    backgroundColor: '#FEF2F2',
    border: '1px solid #FECACA',
    color: '#B91C1C',
    fontSize: '13px',
    borderRadius: '8px',
    padding: '10px 14px',
    marginBottom: '16px',
  },
  modalActions: {
    display: 'flex',
    justifyContent: 'flex-end',
    gap: '12px',
    marginTop: '8px',
  },
  saveButton: {
    backgroundColor: '#2563EB',
    color: '#FFFFFF',
    fontWeight: 500,
    padding: '10px 24px',
    border: 'none',
    borderRadius: '8px',
    fontSize: '14px',
    cursor: 'pointer',
  },
  saveButtonDisabled: {
    opacity: 0.5,
    cursor: 'not-allowed',
  },
  cancelButton: {
    backgroundColor: '#FFFFFF',
    color: '#374151',
    fontWeight: 500,
    padding: '10px 24px',
    border: '1px solid #E5E7EB',
    borderRadius: '8px',
    fontSize: '14px',
    cursor: 'pointer',
  },
  toast: {
    position: 'fixed' as const,
    bottom: '24px',
    left: '50%',
    transform: 'translateX(-50%)',
    padding: '12px 24px',
    borderRadius: '8px',
    fontSize: '14px',
    fontWeight: 500,
    zIndex: 1200,
    boxShadow: '0 4px 12px rgba(0, 0, 0, 0.15)',
    direction: 'rtl' as const,
  },
  toastSuccess: {
    backgroundColor: '#DCFCE7',
    color: '#166534',
    border: '1px solid #BBF7D0',
  },
  toastError: {
    backgroundColor: '#FEF2F2',
    color: '#B91C1C',
    border: '1px solid #FECACA',
  },
  loadingRow: {
    textAlign: 'center' as const,
    padding: '40px 16px',
    color: '#6B7280',
    fontSize: '14px',
  },
  checkCount: {
    display: 'inline-block',
    backgroundColor: '#F3F4F6',
    color: '#374151',
    padding: '2px 10px',
    borderRadius: '12px',
    fontSize: '13px',
    fontWeight: 500,
  },
  dateText: {
    fontSize: '13px',
    color: '#6B7280',
  },
}

function formatDate(date: Date): string {
  const d = new Date(date)
  const year = d.getFullYear()
  const month = String(d.getMonth() + 1).padStart(2, '0')
  const day = String(d.getDate()).padStart(2, '0')
  return `${year}-${month}-${day}`
}

type ToastState = { message: string; type: 'success' | 'error' } | null

export default function UsersPage() {
  const [users, setUsers] = useState<UserItem[]>([])
  const [loading, setLoading] = useState(true)
  const [currentUsername, setCurrentUsername] = useState<string | null>(null)
  const [modalOpen, setModalOpen] = useState(false)
  const [editingUser, setEditingUser] = useState<UserItem | null>(null)
  const [toast, setToast] = useState<ToastState>(null)

  // Form fields
  const [formUsername, setFormUsername] = useState('')
  const [formDisplayName, setFormDisplayName] = useState('')
  const [formPassword, setFormPassword] = useState('')
  const [formRole, setFormRole] = useState<'ADMIN' | 'USER'>('USER')
  const [formIsActive, setFormIsActive] = useState(true)
  const [formError, setFormError] = useState('')
  const [saving, setSaving] = useState(false)

  const loadUsers = useCallback(async () => {
    setLoading(true)
    try {
      const [usersResult, currentUserResult] = await Promise.all([
        listUsers(),
        getCurrentUser(),
      ])
      if (Array.isArray(usersResult)) {
        setUsers(usersResult)
      } else {
        showToast(usersResult.error, 'error')
      }
      if (currentUserResult) {
        setCurrentUsername(currentUserResult.username)
      }
    } catch {
      showToast('حدث خطأ أثناء تحميل البيانات', 'error')
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => {
    loadUsers()
  }, [loadUsers])

  const showToast = (message: string, type: 'success' | 'error') => {
    setToast({ message, type })
    setTimeout(() => setToast(null), 4000)
  }

  const openAddModal = () => {
    setEditingUser(null)
    setFormUsername('')
    setFormDisplayName('')
    setFormPassword('')
    setFormRole('USER')
    setFormIsActive(true)
    setFormError('')
    setModalOpen(true)
  }

  const openEditModal = (user: UserItem) => {
    setEditingUser(user)
    setFormUsername(user.username)
    setFormDisplayName(user.displayName)
    setFormPassword('')
    setFormRole(user.role as 'ADMIN' | 'USER')
    setFormIsActive(user.isActive)
    setFormError('')
    setModalOpen(true)
  }

  const closeModal = () => {
    setModalOpen(false)
    setEditingUser(null)
    setFormError('')
  }

  const handleSave = async () => {
    // Client-side validation
    if (!formUsername.trim()) {
      setFormError('اسم المستخدم مطلوب')
      return
    }
    if (formUsername.trim().length > 50) {
      setFormError('اسم المستخدم يجب ألا يتجاوز 50 حرف')
      return
    }
    if (!formDisplayName.trim()) {
      setFormError('الاسم المعروض مطلوب')
      return
    }
    if (formDisplayName.trim().length > 100) {
      setFormError('الاسم المعروض يجب ألا يتجاوز 100 حرف')
      return
    }
    if (!editingUser && !formPassword) {
      setFormError('كلمة المرور مطلوبة')
      return
    }
    if (formPassword && formPassword.length < 6) {
      setFormError('كلمة المرور يجب أن تكون 6 أحرف على الأقل')
      return
    }

    setSaving(true)
    setFormError('')

    const data: UserFormData = {
      username: formUsername.trim(),
      displayName: formDisplayName.trim(),
      password: formPassword || undefined,
      role: formRole,
      isActive: editingUser ? formIsActive : undefined,
    }

    let result
    try {
      if (editingUser) {
        result = await updateUser(editingUser.id, data)
      } else {
        result = await createUser(data)
      }
    } catch {
      setFormError('حدث خطأ أثناء حفظ البيانات. حاول مرة أخرى.')
      setSaving(false)
      return
    }

    if ('error' in result) {
      setFormError(result.error)
      setSaving(false)
      return
    }

    closeModal()
    showToast(
      editingUser ? 'تم تعديل المستخدم بنجاح' : 'تم إضافة المستخدم بنجاح',
      'success'
    )
    setSaving(false)
    loadUsers()
  }

  const handleToggleActive = async (user: UserItem) => {
    try {
      const result = await toggleUserActive(user.id)
      if ('error' in result) {
        showToast(result.error, 'error')
        return
      }
      showToast(
        user.isActive ? 'تم تعطيل المستخدم بنجاح' : 'تم تفعيل المستخدم بنجاح',
        'success'
      )
      loadUsers()
    } catch {
      showToast('حدث خطأ أثناء تحديث الحالة', 'error')
    }
  }

  return (
    <div style={styles.page}>
      {/* Header */}
      <div style={styles.headerRow}>
        <h1 style={styles.pageTitle}>إدارة المستخدمين</h1>
        <button
          style={styles.addButton}
          onClick={openAddModal}
          onMouseEnter={(e) => {
            e.currentTarget.style.backgroundColor = '#1D4ED8'
          }}
          onMouseLeave={(e) => {
            e.currentTarget.style.backgroundColor = '#2563EB'
          }}
        >
          إضافة مستخدم جديد
        </button>
      </div>

      {/* Table Card */}
      <div style={styles.card}>
        {loading ? (
          <div style={styles.loadingRow}>جاري تحميل المستخدمين...</div>
        ) : users.length === 0 ? (
          <div style={styles.emptyState}>
            <div style={styles.emptyTitle}>لا يوجد مستخدمون مسجلون بعد</div>
            <div style={styles.emptyText}>
              أضف مستخدمًا جديدًا للبدء في إدارة المستخدمين.
            </div>
            <button
              style={styles.emptyAddButton}
              onClick={openAddModal}
              onMouseEnter={(e) => {
                e.currentTarget.style.backgroundColor = '#1D4ED8'
              }}
              onMouseLeave={(e) => {
                e.currentTarget.style.backgroundColor = '#2563EB'
              }}
            >
              أضف مستخدمًا جديدًا
            </button>
          </div>
        ) : (
          <table style={styles.table}>
            <thead>
              <tr>
                <th style={styles.th}>اسم المستخدم</th>
                <th style={styles.th}>الاسم المعروض</th>
                <th style={styles.th}>الدور</th>
                <th style={styles.th}>الحالة</th>
                <th style={styles.th}>عدد الشيكات</th>
                <th style={styles.th}>تاريخ الإضافة</th>
                <th style={styles.th}>الإجراءات</th>
              </tr>
            </thead>
            <tbody>
              {users.map((user) => {
                const isSelf = currentUsername === user.username
                return (
                  <tr
                    key={user.id}
                    onMouseEnter={(e) => {
                      if (e.currentTarget instanceof HTMLElement) {
                        e.currentTarget.style.backgroundColor = '#F9FAFB'
                      }
                    }}
                    onMouseLeave={(e) => {
                      if (e.currentTarget instanceof HTMLElement) {
                        e.currentTarget.style.backgroundColor = ''
                      }
                    }}
                  >
                    <td style={styles.td}>{user.username}</td>
                    <td style={styles.td}>{user.displayName}</td>
                    <td style={styles.td}>
                      <span
                        style={{
                          ...styles.badge,
                          ...(user.role === 'ADMIN'
                            ? styles.badgeAdmin
                            : styles.badgeUser),
                        }}
                      >
                        {user.role === 'ADMIN' ? 'مدير' : 'مستخدم'}
                      </span>
                    </td>
                    <td style={styles.td}>
                      <span
                        style={{
                          ...styles.badge,
                          ...(user.isActive
                            ? styles.badgeActive
                            : styles.badgeInactive),
                        }}
                      >
                        {user.isActive ? 'نشط' : 'غير نشط'}
                      </span>
                    </td>
                    <td style={styles.td}>
                      <span style={styles.checkCount}>{user.checkCount}</span>
                    </td>
                    <td style={styles.td}>
                      <span style={styles.dateText}>
                        {formatDate(user.createdAt)}
                      </span>
                    </td>
                    <td style={styles.td}>
                      <button
                        style={{ ...styles.actionButton, ...styles.editButton }}
                        onClick={() => openEditModal(user)}
                        onMouseEnter={(e) => {
                          e.currentTarget.style.backgroundColor = '#DBEAFE'
                        }}
                        onMouseLeave={(e) => {
                          e.currentTarget.style.backgroundColor = '#EFF6FF'
                        }}
                      >
                        تعديل
                      </button>
                      {isSelf ? (
                        <span style={styles.toggleDisabled}>حسابك</span>
                      ) : (
                        <button
                          style={{
                            ...styles.actionButton,
                            ...(user.isActive
                              ? styles.toggleInactiveButton
                              : styles.toggleActiveButton),
                          }}
                          onClick={() => handleToggleActive(user)}
                          onMouseEnter={(e) => {
                            if (user.isActive) {
                              e.currentTarget.style.backgroundColor = '#FEE2E2'
                            } else {
                              e.currentTarget.style.backgroundColor = '#BBF7D0'
                            }
                          }}
                          onMouseLeave={(e) => {
                            if (user.isActive) {
                              e.currentTarget.style.backgroundColor = '#FEF2F2'
                            } else {
                              e.currentTarget.style.backgroundColor = '#DCFCE7'
                            }
                          }}
                        >
                          {user.isActive ? 'تعطيل' : 'تفعيل'}
                        </button>
                      )}
                    </td>
                  </tr>
                )
              })}
            </tbody>
          </table>
        )}
      </div>

      {/* Add/Edit Modal */}
      {modalOpen && (
        <div style={styles.overlay} onClick={closeModal}>
          <div
            style={styles.modal}
            onClick={(e) => e.stopPropagation()}
          >
            <h2 style={styles.modalTitle}>
              {editingUser
                ? `تعديل المستخدم: ${editingUser.displayName}`
                : 'إضافة مستخدم جديد'}
            </h2>

            {formError && (
              <div style={styles.formError} role="alert">
                {formError}
              </div>
            )}

            <div style={styles.fieldGroup}>
              <label htmlFor="user-username" style={styles.label}>
                اسم المستخدم
                <span style={styles.requiredStar}>*</span>
              </label>
              <input
                id="user-username"
                type="text"
                required
                value={formUsername}
                onChange={(e) => setFormUsername(e.target.value)}
                placeholder="أدخل اسم المستخدم"
                maxLength={50}
                style={styles.input}
                onFocus={(e) => {
                  e.target.style.borderColor = '#2563EB'
                  e.target.style.boxShadow =
                    '0 0 0 2px rgba(37, 99, 235, 0.2)'
                }}
                onBlur={(e) => {
                  e.target.style.borderColor = '#E5E7EB'
                  e.target.style.boxShadow = 'none'
                }}
              />
            </div>

            <div style={styles.fieldGroup}>
              <label htmlFor="user-displayname" style={styles.label}>
                الاسم المعروض
                <span style={styles.requiredStar}>*</span>
              </label>
              <input
                id="user-displayname"
                type="text"
                required
                value={formDisplayName}
                onChange={(e) => setFormDisplayName(e.target.value)}
                placeholder="أدخل الاسم المعروض"
                maxLength={100}
                style={styles.input}
                onFocus={(e) => {
                  e.target.style.borderColor = '#2563EB'
                  e.target.style.boxShadow =
                    '0 0 0 2px rgba(37, 99, 235, 0.2)'
                }}
                onBlur={(e) => {
                  e.target.style.borderColor = '#E5E7EB'
                  e.target.style.boxShadow = 'none'
                }}
              />
            </div>

            <div style={styles.fieldGroup}>
              <label htmlFor="user-password" style={styles.label}>
                كلمة المرور
                {!editingUser && (
                  <span style={styles.requiredStar}>*</span>
                )}
              </label>
              <input
                id="user-password"
                type="password"
                required={!editingUser}
                value={formPassword}
                onChange={(e) => setFormPassword(e.target.value)}
                placeholder={
                  editingUser
                    ? 'اتركه فارغًا إذا لم ترد التغيير'
                    : 'أدخل كلمة المرور'
                }
                minLength={6}
                style={styles.input}
                onFocus={(e) => {
                  e.target.style.borderColor = '#2563EB'
                  e.target.style.boxShadow =
                    '0 0 0 2px rgba(37, 99, 235, 0.2)'
                }}
                onBlur={(e) => {
                  e.target.style.borderColor = '#E5E7EB'
                  e.target.style.boxShadow = 'none'
                }}
              />
            </div>

            <div style={styles.fieldGroup}>
              <label htmlFor="user-role" style={styles.label}>
                الدور
                <span style={styles.requiredStar}>*</span>
              </label>
              <select
                id="user-role"
                value={formRole}
                onChange={(e) =>
                  setFormRole(e.target.value as 'ADMIN' | 'USER')
                }
                style={styles.select}
                onFocus={(e) => {
                  e.target.style.borderColor = '#2563EB'
                  e.target.style.boxShadow =
                    '0 0 0 2px rgba(37, 99, 235, 0.2)'
                }}
                onBlur={(e) => {
                  e.target.style.borderColor = '#E5E7EB'
                  e.target.style.boxShadow = 'none'
                }}
              >
                <option value="ADMIN">مدير</option>
                <option value="USER">مستخدم</option>
              </select>
            </div>

            {editingUser && (
              <div style={styles.checkboxGroup}>
                <input
                  id="user-isactive"
                  type="checkbox"
                  checked={formIsActive}
                  onChange={(e) => setFormIsActive(e.target.checked)}
                  style={styles.checkbox}
                />
                <label htmlFor="user-isactive" style={styles.checkboxLabel}>
                  نَشط
                </label>
              </div>
            )}

            <div style={styles.modalActions}>
              <button
                style={styles.cancelButton}
                onClick={closeModal}
                onMouseEnter={(e) => {
                  e.currentTarget.style.backgroundColor = '#F9FAFB'
                }}
                onMouseLeave={(e) => {
                  e.currentTarget.style.backgroundColor = '#FFFFFF'
                }}
              >
                إلغاء
              </button>
              <button
                disabled={saving}
                style={{
                  ...styles.saveButton,
                  ...(saving ? styles.saveButtonDisabled : {}),
                }}
                onClick={handleSave}
                onMouseEnter={(e) => {
                  if (!saving)
                    e.currentTarget.style.backgroundColor = '#1D4ED8'
                }}
                onMouseLeave={(e) => {
                  if (!saving)
                    e.currentTarget.style.backgroundColor = '#2563EB'
                }}
              >
                {saving ? 'جاري الحفظ...' : 'حفظ'}
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Toast */}
      {toast && (
        <div
          style={{
            ...styles.toast,
            ...(toast.type === 'success'
              ? styles.toastSuccess
              : styles.toastError),
          }}
          role="alert"
        >
          {toast.message}
        </div>
      )}
    </div>
  )
}
