'use client'

import { useState, useEffect } from 'react'
import { getCurrentUser } from './actions'

const styles: Record<string, React.CSSProperties> = {
  page: {
    minHeight: '100vh',
    backgroundColor: '#F8FAFC',
    padding: '32px 24px',
    fontFamily: 'system-ui, Arial, sans-serif',
    direction: 'rtl',
  },
  container: {
    maxWidth: '800px',
    marginLeft: 'auto',
    marginRight: 'auto',
  },
  header: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: '32px',
    flexWrap: 'wrap' as const,
    gap: '12px',
  },
  titleGroup: {
    display: 'flex',
    flexDirection: 'column' as const,
    gap: '4px',
  },
  appTitle: {
    fontSize: '24px',
    fontWeight: 700,
    color: '#111827',
    margin: 0,
  },
  userInfo: {
    display: 'flex',
    alignItems: 'center',
    gap: '10px',
    fontSize: '14px',
    color: '#6B7280',
    flexWrap: 'wrap' as const,
  },
  roleBadge: {
    display: 'inline-block',
    backgroundColor: '#EFF6FF',
    color: '#2563EB',
    fontSize: '12px',
    fontWeight: 600,
    padding: '4px 10px',
    borderRadius: '12px',
  },
  logoutButton: {
    backgroundColor: '#FFFFFF',
    color: '#374151',
    fontWeight: 500,
    padding: '8px 18px',
    border: '1px solid #E5E7EB',
    borderRadius: '8px',
    fontSize: '14px',
    cursor: 'pointer',
    textDecoration: 'none',
    display: 'inline-block',
  },
  grid: {
    display: 'grid',
    gridTemplateColumns: '1fr 1fr',
    gap: '16px',
  },
  card: {
    backgroundColor: '#FFFFFF',
    borderRadius: '8px',
    border: '1px solid #E5E7EB',
    padding: '24px',
    cursor: 'pointer',
    textDecoration: 'none',
    display: 'block',
    position: 'relative' as const,
    overflow: 'hidden',
  },
  cardInactive: {
    backgroundColor: '#FFFFFF',
    borderRadius: '8px',
    border: '1px solid #E5E7EB',
    padding: '24px',
    cursor: 'default',
    opacity: 0.55,
    position: 'relative' as const,
    overflow: 'hidden',
  },
  cardAccent: {
    position: 'absolute' as const,
    right: 0,
    top: 0,
    width: '4px',
    height: '100%',
    backgroundColor: '#2563EB',
  },
  cardTitle: {
    fontSize: '16px',
    fontWeight: 700,
    color: '#111827',
    margin: 0,
    marginBottom: '8px',
  },
  cardTitleInactive: {
    fontSize: '16px',
    fontWeight: 700,
    color: '#9CA3AF',
    margin: 0,
    marginBottom: '8px',
  },
  cardDescription: {
    fontSize: '13px',
    color: '#6B7280',
    margin: 0,
    lineHeight: 1.5,
  },
  badge: {
    display: 'inline-block',
    backgroundColor: '#FEF3C7',
    color: '#92400E',
    fontSize: '11px',
    fontWeight: 600,
    padding: '3px 8px',
    borderRadius: '8px',
    marginTop: '12px',
  },
  loadingContainer: {
    minHeight: '100vh',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: '#F8FAFC',
    fontFamily: 'system-ui, Arial, sans-serif',
    direction: 'rtl',
  },
  loadingText: {
    fontSize: '16px',
    color: '#6B7280',
  },
  errorContainer: {
    minHeight: '100vh',
    display: 'flex',
    flexDirection: 'column' as const,
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: '#F8FAFC',
    fontFamily: 'system-ui, Arial, sans-serif',
    direction: 'rtl',
    gap: '16px',
    padding: '24px',
  },
  errorText: {
    fontSize: '16px',
    color: '#6B7280',
  },
  loginLink: {
    backgroundColor: '#2563EB',
    color: '#FFFFFF',
    fontWeight: 500,
    padding: '10px 24px',
    border: 'none',
    borderRadius: '8px',
    fontSize: '14px',
    cursor: 'pointer',
    textDecoration: 'none',
    display: 'inline-block',
  },
}

interface NavCardProps {
  title: string
  description: string
  href?: string
  active: boolean
}

function NavCard({ title, description, href, active }: NavCardProps) {
  if (active && href) {
    return (
      <a href={href} style={styles.card}>
        <div style={styles.cardAccent} />
        <h3 style={styles.cardTitle}>{title}</h3>
        <p style={styles.cardDescription}>{description}</p>
      </a>
    )
  }

  return (
    <div style={styles.cardInactive}>
      <h3 style={styles.cardTitleInactive}>{title}</h3>
      <p style={styles.cardDescription}>{description}</p>
      <div style={styles.badge}>قيد الإنشاء</div>
    </div>
  )
}

export default function HomePage() {
  const [user, setUser] = useState<{ username: string; role: string } | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(false)

  useEffect(() => {
    getCurrentUser()
      .then((data) => {
        setUser(data)
        setLoading(false)
      })
      .catch(() => {
        setError(true)
        setLoading(false)
      })
  }, [])

  if (loading) {
    return (
      <div style={styles.loadingContainer}>
        <p style={styles.loadingText}>جاري تحميل البيانات...</p>
      </div>
    )
  }

  if (error || !user) {
    return (
      <div style={styles.errorContainer}>
        <p style={styles.errorText}>
          {error ? 'حدث خطأ في تحميل بيانات المستخدم' : 'يرجى تسجيل الدخول'}
        </p>
        <a href="/login" style={styles.loginLink}>
          تسجيل الدخول
        </a>
      </div>
    )
  }

  const roleLabel = user.role === 'ADMIN' ? 'Admin' : 'User'

  return (
    <div style={styles.page}>
      <div style={styles.container}>
        {/* Header */}
        <div style={styles.header}>
          <div style={styles.titleGroup}>
            <h1 style={styles.appTitle}>نظام إدارة الشيكات</h1>
            <div style={styles.userInfo}>
              <span>مرحبًا، {user.username}</span>
              <span style={styles.roleBadge}>{roleLabel}</span>
            </div>
          </div>
          <a href="/logout" style={styles.logoutButton}>
            تسجيل خروج
          </a>
        </div>

        {/* Navigation Cards */}
        <div style={styles.grid}>
          <NavCard
            title="البنوك"
            description="إدارة البنوك المسجلة في النظام"
            href="/banks"
            active={true}
          />
          <NavCard
            title="الجهات"
            description="إدارة الجهات المرتبطة بالشيكات"
            href="/parties"
            active={true}
          />
          <NavCard
            title="الشيكات"
            description="إدارة الشيكات بأنواعها وحالاتها"
            href="/checks"
            active={true}
          />
          <NavCard
            title="المستخدمين"
            description="إدارة المستخدمين والصلاحيات"
            href="/users"
            active={true}
          />
        </div>
      </div>
    </div>
  )
}
