import Sidebar from './Sidebar';

export default function Layout({ activeDashboard, onNavigate, children }) {
  return (
    <div className="flex min-h-screen bg-bg-page">
      <Sidebar activeDashboard={activeDashboard} onNavigate={onNavigate} />
      <main className="flex-1 overflow-x-hidden">
        <div className="page-container py-8">
          {children}
        </div>
      </main>
    </div>
  );
}
