const navItems = [
  {
    id: 'operations',
    label: 'لوحة العمليات',
    icon: (
      <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
      </svg>
    ),
  },
  {
    id: 'projects',
    label: 'إدارة المشاريع',
    icon: (
      <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10" />
      </svg>
    ),
  },
];

export default function Sidebar({ activeDashboard, onNavigate }) {
  return (
    <aside className="w-[260px] bg-bg-sidebar text-text-on-dark flex flex-col fixed inset-y-0 right-0 z-40">
      {/* Logo */}
      <div className="px-6 py-6 border-b border-white/10">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-lg bg-accent-bronze flex items-center justify-center">
            <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
            </svg>
          </div>
          <div>
            <h1 className="type-heading-1 text-text-on-dark font-semibold">منصة البيانات</h1>
            <p className="type-caption text-text-on-dark-muted mt-1">Dynamic Dashboard</p>
          </div>
        </div>
      </div>

      {/* Navigation */}
      <nav className="flex-1 px-4 py-6">
        <p className="type-caption text-text-on-dark-muted px-3 mb-3">اللوحات</p>
        <ul className="space-y-2">
          {navItems.map((item) => (
            <li key={item.id}>
              <button
                onClick={() => onNavigate(item.id)}
                className={`w-full flex items-center gap-3 px-3 py-3 rounded-lg transition-all text-right ${
                  activeDashboard === item.id
                    ? 'bg-bg-sidebar-hover text-text-on-dark border-r-2 border-accent-bronze'
                    : 'text-text-on-dark-muted hover:bg-bg-sidebar-hover hover:text-text-on-dark'
                }`}
              >
                {item.icon}
                <span className="type-body font-medium">{item.label}</span>
              </button>
            </li>
          ))}
        </ul>
      </nav>

      {/* User Profile */}
      <div className="px-4 py-4 border-t border-white/10">
        <div className="flex items-center gap-3 px-3">
          <div className="w-10 h-10 rounded-full bg-accent-bronze flex items-center justify-center text-white font-semibold">
            م
          </div>
          <div className="flex-1">
            <p className="type-body-sm font-medium text-text-on-dark">ماجد السالم</p>
            <p className="type-caption text-text-on-dark-muted">مدير النظام</p>
          </div>
        </div>
      </div>
    </aside>
  );
}
