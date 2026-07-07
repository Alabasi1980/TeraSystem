import { useState } from 'react';
import Layout from './components/Layout';
import DashboardOperations from './components/DashboardOperations';
import DashboardProjects from './components/DashboardProjects';

export default function App() {
  const [activeDashboard, setActiveDashboard] = useState('operations');

  const handleNavigation = (dashboard) => {
    setActiveDashboard(dashboard);
  };

  return (
    <Layout
      activeDashboard={activeDashboard}
      onNavigate={handleNavigation}
    >
      {activeDashboard === 'operations' ? (
        <DashboardOperations />
      ) : (
        <DashboardProjects />
      )}
    </Layout>
  );
}
