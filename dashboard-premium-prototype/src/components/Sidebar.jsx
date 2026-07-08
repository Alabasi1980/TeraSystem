// ── الشريط الجانبي (Sidebar) ──
import { motion } from 'framer-motion';
import {
  LayoutDashboard,
  BarChart3,
  FileText,
  Users,
  Settings,
  LogOut,
  ChevronLeft,
} from 'lucide-react';
import { sidebarItems } from '../data/mockData';

const iconMap = {
  LayoutDashboard,
  BarChart3,
  FileText,
  Users,
  Settings,
};

export default function Sidebar({ activeScreen, onNavigate, onLogout, isCollapsed, onToggle }) {
  const handleItemClick = (item) => {
    if (item.id === 'clients') onNavigate('clients');
    else if (item.id === 'settings') onNavigate('admin');
    else onNavigate('dashboard');
  };

  return (
    <aside
      className={`fixed top-0 start-0 h-full bg-espresso z-50 flex flex-col transition-all duration-300 sidebar-shadow ${
        isCollapsed ? 'w-20' : 'w-[280px]'
      }`}
    >
      {/* Logo */}
      <div className="h-16 flex items-center justify-center border-b border-white/10 px-4">
        <div className="flex items-center gap-3">
          <div className="w-9 h-9 rounded-lg bg-copper flex items-center justify-center shrink-0">
            <LayoutDashboard className="w-5 h-5 text-white" />
          </div>
          {!isCollapsed && (
            <motion.span
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              className="font-display text-lg text-cream font-bold whitespace-nowrap"
            >
              منصة البيانات
            </motion.span>
          )}
        </div>
      </div>

      {/* Navigation Items */}
      <nav className="flex-1 px-3 py-4 space-y-1 overflow-y-auto">
        {sidebarItems.map((item, idx) => {
          const Icon = iconMap[item.icon] || LayoutDashboard;
          const isActive = item.screen === activeScreen;
          return (
            <motion.button
              key={item.id}
              initial={{ opacity: 0, x: -20 }}
              animate={{ opacity: 1, x: 0 }}
              transition={{ delay: idx * 0.05 }}
              onClick={() => handleItemClick(item)}
              className={`w-full flex items-center gap-3 px-4 py-3 rounded-lg transition-all duration-200 group relative ${
                isActive
                  ? 'bg-copper/15 text-copper'
                  : 'text-cream/60 hover:text-cream hover:bg-white/5'
              }`}
            >
              {/* Active accent bar */}
              {isActive && (
                <motion.div
                  layoutId="sidebar-active"
                  className="absolute start-0 top-1/2 -translate-y-1/2 w-1 h-6 bg-copper rounded-full"
                />
              )}
              <Icon className={`w-5 h-5 shrink-0 ${isActive ? 'text-copper' : ''}`} />
              {!isCollapsed && (
                <span className="text-sm font-medium whitespace-nowrap">{item.label}</span>
              )}
            </motion.button>
          );
        })}
      </nav>

      {/* Collapse Toggle */}
      <button
        onClick={onToggle}
        className="mx-3 mb-2 p-2 rounded-lg text-cream/40 hover:text-cream hover:bg-white/5 transition-all"
      >
        <ChevronLeft
          className="w-5 h-5 mx-auto transition-transform duration-300"
          style={{ transform: 'scaleX(-1)' }}
        />
      </button>

      {/* Logout */}
      <div className="border-t border-white/10 p-3">
        <button
          onClick={onLogout}
          className="w-full flex items-center gap-3 px-4 py-3 rounded-lg text-cream/50 hover:text-burgundy-light hover:bg-burgundy/10 transition-all duration-200"
        >
          <LogOut className="w-5 h-5 shrink-0" />
          {!isCollapsed && <span className="text-sm font-medium">تسجيل الخروج</span>}
        </button>
      </div>
    </aside>
  );
}
