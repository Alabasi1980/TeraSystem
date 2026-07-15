// ── التطبيق الرئيسي (App.jsx) ──
// Dynamic Dashboard Builder — Premium Prototype
import { useState, useCallback } from 'react';
import { motion, AnimatePresence } from 'framer-motion';

// شاشات التطبيق
import LoginScreen from './components/LoginScreen';
import ClientSelectScreen from './components/ClientSelectScreen';
import Sidebar from './components/Sidebar';
import Header from './components/Header';
import FiltersBar from './components/FiltersBar';
import KPICard from './components/KPICard';
import ChartsSection from './components/ChartsSection';
import DataTable from './components/DataTable';
import AnalyticModal from './components/AnalyticModal';
import AdminBuilderScreen from './components/AdminBuilderScreen';

import { kpiData } from './data/mockData';

// ── شاشة الداشبورد الرئيسية ──
function MainDashboard({ onNavigate, onLogout }) {
  const [sidebarCollapsed, setSidebarCollapsed] = useState(false);
  const [selectedKpi, setSelectedKpi] = useState(null);
  const [isModalOpen, setIsModalOpen] = useState(false);

  const handleKpiClick = useCallback((kpi) => {
    setSelectedKpi(kpi);
    setIsModalOpen(true);
  }, []);

  const closeModal = useCallback(() => {
    setIsModalOpen(false);
    setTimeout(() => setSelectedKpi(null), 300);
  }, []);

  return (
    <div className="min-h-screen bg-cream flex">
      {/* Sidebar */}
      <Sidebar
        activeScreen="dashboard"
        onNavigate={onNavigate}
        onLogout={onLogout}
        isCollapsed={sidebarCollapsed}
        onToggle={() => setSidebarCollapsed(!sidebarCollapsed)}
      />

      {/* Main Content */}
      <main
        className="flex-1 transition-all duration-300"
        style={{ marginInlineStart: sidebarCollapsed ? '5rem' : '280px' }}
      >
        {/* Header */}
        <Header
          breadcrumbs={['الرئيسية', 'لوحة القيادة']}
        />

        {/* Page Content */}
        <div className="p-6 space-y-6">
          {/* Page Title */}
          <motion.div
            initial={{ opacity: 0, y: -10 }}
            animate={{ opacity: 1, y: 0 }}
          >
            <h1 className="font-display text-2xl font-bold text-espresso mb-1">
              لوحة القيادة
            </h1>
            <p className="text-sm text-espresso-muted">
              مرحباً بك في لوحة التحكم — نظرة شاملة على أداء المنشأة
            </p>
          </motion.div>

          {/* Filters */}
          <FiltersBar />

          {/* KPI Cards Grid */}
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-6 gap-5">
            {kpiData.map((kpi, idx) => (
              <KPICard
                key={kpi.id}
                data={kpi}
                index={idx}
                onClick={handleKpiClick}
              />
            ))}
          </div>

          {/* Charts Section */}
          <ChartsSection />

          {/* Data Table */}
          <DataTable />
        </div>
      </main>

      {/* Analytic Modal */}
      <AnalyticModal
        isOpen={isModalOpen}
        onClose={closeModal}
        kpiData={selectedKpi}
      />
    </div>
  );
}

// ── التطبيق الرئيسي مع التوجيه المبني على الحالة ──
export default function App() {
  const [screen, setScreen] = useState('login');
  const [selectedClient, setSelectedClient] = useState(null);

  const handleLogin = useCallback(() => {
    setScreen('clients');
  }, []);

  const handleSelectClient = useCallback((client) => {
    setSelectedClient(client);
    setScreen('dashboard');
  }, []);

  const handleNavigate = useCallback((target) => {
    setScreen(target);
  }, []);

  const handleLogout = useCallback(() => {
    setSelectedClient(null);
    setScreen('login');
  }, []);

  // Framer Motion page transition variants
  const pageVariants = {
    initial: { opacity: 0, y: 8 },
    animate: { opacity: 1, y: 0 },
    exit: { opacity: 0, y: -8 },
  };

  return (
    <div dir="rtl">
      <AnimatePresence mode="wait">
        {screen === 'login' && (
          <motion.div
            key="login"
            variants={pageVariants}
            initial="initial"
            animate="animate"
            exit="exit"
            transition={{ duration: 0.35, ease: 'easeInOut' }}
          >
            <LoginScreen onLogin={handleLogin} />
          </motion.div>
        )}

        {screen === 'clients' && (
          <motion.div
            key="clients"
            variants={pageVariants}
            initial="initial"
            animate="animate"
            exit="exit"
            transition={{ duration: 0.35, ease: 'easeInOut' }}
          >
            <ClientSelectScreen onSelectClient={handleSelectClient} />
          </motion.div>
        )}

        {screen === 'dashboard' && (
          <motion.div
            key="dashboard"
            variants={pageVariants}
            initial="initial"
            animate="animate"
            exit="exit"
            transition={{ duration: 0.35, ease: 'easeInOut' }}
          >
            <MainDashboard
              onNavigate={handleNavigate}
              onLogout={handleLogout}
            />
          </motion.div>
        )}

        {screen === 'admin' && (
          <motion.div
            key="admin"
            variants={pageVariants}
            initial="initial"
            animate="animate"
            exit="exit"
            transition={{ duration: 0.35, ease: 'easeInOut' }}
          >
            <AdminBuilderScreen
              onNavigate={handleNavigate}
              onLogout={handleLogout}
            />
          </motion.div>
        )}
      </AnimatePresence>
    </div>
  );
}
