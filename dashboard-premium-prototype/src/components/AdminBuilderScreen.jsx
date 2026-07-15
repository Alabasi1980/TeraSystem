// ── لوحة الإدارة (Admin Builder) ──
import { useState } from 'react';
import { motion } from 'framer-motion';
import {
  LayoutDashboard,
  Users,
  Monitor,
  Database,
  Settings,
  Shield,
  BarChart3,
  FileText,
  Clock,
  ArrowLeft,
  ChevronLeft,
} from 'lucide-react';
import { adminData, currentUser } from '../data/mockData';
import Sidebar from './Sidebar';
import Header from './Header';

const adminNavItems = [
  { id: 'overview', label: 'نظرة عامة', icon: LayoutDashboard },
  { id: 'clients', label: 'العملاء', icon: Users },
  { id: 'users', label: 'المستخدمين', icon: Shield },
  { id: 'dashboards', label: 'الداشبوردات', icon: Monitor },
  { id: 'datasources', label: 'مصادر البيانات', icon: Database },
  { id: 'settings', label: 'الإعدادات', icon: Settings },
];

const colorMap = {
  copper: {
    bg: 'bg-copper/10',
    text: 'text-copper',
  },
  olive: {
    bg: 'bg-olive/10',
    text: 'text-olive',
  },
  burgundy: {
    bg: 'bg-burgundy/10',
    text: 'text-burgundy',
  },
  'golden-olive': {
    bg: 'bg-golden-olive/10',
    text: 'text-golden-olive',
  },
};

// بطاقة "قريباً"
function ComingSoonCard({ icon: Icon, title, description, color }) {
  const colors = colorMap[color] || colorMap.copper;

  return (
    <div className="bg-white/50 border-2 border-dashed border-border-dark rounded-xl p-8 text-center">
      <div className={`w-14 h-14 rounded-xl ${colors.bg} ${colors.text} flex items-center justify-center mx-auto mb-4`}>
        <Icon className="w-7 h-7" />
      </div>
      <h3 className="text-lg font-bold text-espresso mb-1">{title}</h3>
      <p className="text-sm text-espresso-muted mb-4">{description}</p>
      <span className="inline-block px-3 py-1 bg-copper/10 text-copper text-xs font-semibold rounded-full">
        قريباً
      </span>
    </div>
  );
}

export default function AdminBuilderScreen({ onNavigate, onLogout }) {
  const [activeSection, setActiveSection] = useState('overview');
  const [sidebarCollapsed, setSidebarCollapsed] = useState(false);

  return (
    <div className="min-h-screen bg-cream flex">
      <Sidebar
        activeScreen="admin"
        onNavigate={onNavigate}
        onLogout={onLogout}
        isCollapsed={sidebarCollapsed}
        onToggle={() => setSidebarCollapsed(!sidebarCollapsed)}
      />

      <main
        className="flex-1 transition-all duration-300"
        style={{ marginInlineStart: sidebarCollapsed ? '5rem' : '280px' }}
      >
        <Header
          title="لوحة الإدارة"
          breadcrumbs={['الرئيسية', 'لوحة الإدارة']}
        />

        <div className="p-6">
          {/* Admin Navigation */}
          <div className="flex items-center gap-2 mb-8 overflow-x-auto pb-2">
            {adminNavItems.map((item) => {
              const Icon = item.icon;
              const isActive = activeSection === item.id;
              return (
                <button
                  key={item.id}
                  onClick={() => setActiveSection(item.id)}
                  className={`flex items-center gap-2 px-4 py-2.5 rounded-lg text-sm font-medium whitespace-nowrap transition-all ${
                    isActive
                      ? 'bg-copper/10 text-copper shadow-sm'
                      : 'text-espresso-muted hover:text-espresso hover:bg-cream-dark'
                  }`}
                >
                  <Icon className="w-4 h-4" />
                  <span>{item.label}</span>
                </button>
              );
            })}
          </div>

          {/* Overview Cards */}
          <motion.div
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-5 mb-8"
          >
            <div className="bg-white rounded-xl p-6 card-shadow">
              <div className="w-10 h-10 rounded-lg bg-copper/10 text-copper flex items-center justify-center mb-3">
                <Users className="w-5 h-5" />
              </div>
              <p className="text-2xl font-bold text-espresso">{adminData.totalClients}</p>
              <p className="text-sm text-espresso-muted">إجمالي العملاء</p>
            </div>
            <div className="bg-white rounded-xl p-6 card-shadow">
              <div className="w-10 h-10 rounded-lg bg-olive/10 text-olive flex items-center justify-center mb-3">
                <Shield className="w-5 h-5" />
              </div>
              <p className="text-2xl font-bold text-espresso">{adminData.totalUsers}</p>
              <p className="text-sm text-espresso-muted">إجمالي المستخدمين</p>
            </div>
            <div className="bg-white rounded-xl p-6 card-shadow">
              <div className="w-10 h-10 rounded-lg bg-burgundy/10 text-burgundy flex items-center justify-center mb-3">
                <Monitor className="w-5 h-5" />
              </div>
              <p className="text-2xl font-bold text-espresso">{adminData.totalDashboards}</p>
              <p className="text-sm text-espresso-muted">الداشبوردات المنشأة</p>
            </div>
            <div className="bg-white rounded-xl p-6 card-shadow">
              <div className="w-10 h-10 rounded-lg bg-golden-olive/10 text-golden-olive flex items-center justify-center mb-3">
                <Database className="w-5 h-5" />
              </div>
              <p className="text-2xl font-bold text-espresso">{adminData.totalDataSources}</p>
              <p className="text-sm text-espresso-muted">مصادر البيانات</p>
            </div>
          </motion.div>

          {/* Clients Table */}
          <motion.div
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.1 }}
            className="bg-white rounded-xl card-shadow overflow-hidden mb-8"
          >
            <div className="px-6 py-4 border-b border-border">
              <h3 className="text-base font-semibold text-espresso">قائمة العملاء</h3>
            </div>
            <div className="overflow-x-auto">
              <table className="w-full text-right">
                <thead>
                  <tr className="bg-cream-dark">
                    <th className="px-6 py-3 text-xs font-semibold text-espresso">اسم العميل</th>
                    <th className="px-6 py-3 text-xs font-semibold text-espresso">الحالة</th>
                    <th className="px-6 py-3 text-xs font-semibold text-espresso">المستخدمين</th>
                    <th className="px-6 py-3 text-xs font-semibold text-espresso">الداشبوردات</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-border">
                  {adminData.clientsList.map((client, idx) => (
                    <tr key={client.id} className="hover:bg-copper/[0.02] transition-colors">
                      <td className="px-6 py-3.5 text-sm font-medium text-espresso">{client.name}</td>
                      <td className="px-6 py-3.5">
                        <span
                          className={`inline-block px-3 py-1 rounded-full text-xs font-semibold ${
                            client.status === 'نشط'
                              ? 'bg-olive/15 text-olive'
                              : 'bg-copper/15 text-copper-dark'
                          }`}
                        >
                          {client.status}
                        </span>
                      </td>
                      <td className="px-6 py-3.5 text-sm text-espresso">{client.users}</td>
                      <td className="px-6 py-3.5 text-sm text-espresso">{client.dashboards}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </motion.div>

          {/* Coming Soon Grid */}
          <div className="grid grid-cols-1 sm:grid-cols-3 gap-5">
            <ComingSoonCard
              icon={BarChart3}
              title="تحليلات متقدمة"
              description="تحليلات متعمقة وذكاء اصطناعي للبيانات"
              color="copper"
            />
            <ComingSoonCard
              icon={FileText}
              title="تقارير مخصصة"
              description="إنشاء وتصدير تقارير مخصصة"
              color="burgundy"
            />
            <ComingSoonCard
              icon={Clock}
              title="سجل النشاطات"
              description="تتبع جميع النشاطات والتغييرات"
              color="olive"
            />
          </div>
        </div>
      </main>
    </div>
  );
}
