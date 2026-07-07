import { useState } from 'react';
import { projectsData } from '../data/demoData';
import GlobalFilter from './GlobalFilter';
import KPICard from './KPICard';
import PieChartWidget from './PieChartWidget';
import DataTable from './DataTable';
import DrillDownModalL1 from './DrillDownModalL1';
import DrillDownModalL2 from './DrillDownModalL2';

const statusRender = (status) => {
  const colors = {
    'على الموعد': 'text-success',
    'متأخر': 'text-danger',
    'مكتمل': 'text-accent-bronze',
  };
  return <span className={`font-medium ${colors[status] || 'text-text-primary'}`}>{status}</span>;
};

const progressRender = (progress) => (
  <div className="flex items-center gap-2">
    <div className="w-16 h-2 bg-bg-card-warm rounded-full overflow-hidden">
      <div
        className="h-full bg-accent-bronze rounded-full"
        style={{ width: `${progress}%` }}
      />
    </div>
    <span className="type-body-sm text-text-secondary">{progress}%</span>
  </div>
);

export default function DashboardProjects() {
  const [modalL1, setModalL1] = useState(false);
  const [modalL2, setModalL2] = useState(false);
  const [selectedCard, setSelectedCard] = useState(null);

  const handleCardClick = (card) => {
    setSelectedCard(card);
    setModalL1(true);
  };

  const columns = [
    { key: 'name', label: 'المشروع' },
    { key: 'client', label: 'العميل' },
    { key: 'progress', label: 'نسبة الإنجاز', render: progressRender },
    { key: 'status', label: 'الحالة', render: statusRender },
    { key: 'manager', label: 'مدير المشروع' },
  ];

  const detailColumns = [
    { key: 'id', label: 'الرقم' },
    { key: 'project', label: 'المشروع' },
    { key: 'task', label: 'المهمة' },
    { key: 'progress', label: 'نسبة الإنجاز', render: progressRender },
    { key: 'status', label: 'الحالة', render: statusRender },
    { key: 'assignee', label: 'الفريق المسؤول' },
  ];

  return (
    <div className="animate-fadeIn">
      {/* Header */}
      <div className="mb-6">
        <h1 className="type-display-1 text-text-primary">إدارة المشاريع</h1>
        <p className="type-body text-text-secondary mt-1">
          متابعة المشاريع وتوزيع المهام والأداء حسب الفريق
        </p>
      </div>

      <GlobalFilter />

      {/* KPI Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-6">
        {projectsData.kpis.map((kpi) => (
          <KPICard
            key={kpi.id}
            title={kpi.title}
            value={kpi.value}
            unit={kpi.unit}
            change={kpi.change}
            changeType={kpi.changeType}
            period={kpi.period}
            icon={kpi.icon}
            onClick={() => handleCardClick(kpi)}
          />
        ))}
      </div>

      {/* Projects List */}
      <div className="mb-6">
        <h2 className="type-heading-1 text-text-primary mb-4">المشاريع النشطة</h2>
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          {projectsData.projectList.map((project, idx) => (
            <div
              key={idx}
              className="bg-bg-card rounded-xl p-6 card-shadow card-shadow-hover border-r-2 border-accent-bronze"
            >
              <div className="flex items-start justify-between mb-3">
                <h3 className="type-heading-2 text-text-primary">{project.name}</h3>
                <span className={`type-caption px-2 py-1 rounded-full ${
                  project.status === 'مكتمل'
                    ? 'bg-success/10 text-success'
                    : project.status === 'متأخر'
                    ? 'bg-danger/10 text-danger'
                    : 'bg-accent-bronze/10 text-accent-bronze'
                }`}>
                  {project.status}
                </span>
              </div>
              <div className="mb-4">
                <div className="flex justify-between mb-1">
                  <span className="type-body-sm text-text-secondary">نسبة الإنجاز</span>
                  <span className="type-body-sm font-medium text-text-primary">{project.progress}%</span>
                </div>
                <div className="w-full h-2 bg-bg-card-warm rounded-full overflow-hidden">
                  <div
                    className="h-full bg-accent-bronze rounded-full"
                    style={{ width: `${project.progress}%` }}
                  />
                </div>
              </div>
              <div className="flex justify-between type-body-sm text-text-muted">
                <span>الميزانية: {project.used.toLocaleString()} / {project.budget.toLocaleString()} ريال</span>
                <span>{project.team} فرد</span>
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* Charts + Table Row */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="lg:col-span-1">
          <PieChartWidget
            data={projectsData.statusDistribution}
            title="توزيع حالات المشاريع"
          />
        </div>
        <div className="lg:col-span-2">
          <h2 className="type-heading-1 text-text-primary mb-4">أحدث المشاريع</h2>
          <DataTable
            columns={columns}
            data={projectsData.recentProjects}
            onRowClick={() => {
              setSelectedCard({ title: 'تفاصيل المشروع', value: '', change: '', period: '' });
              setModalL2(true);
            }}
          />
        </div>
      </div>

      {/* DrillDown Modals */}
      <DrillDownModalL1
        isOpen={modalL1}
        onClose={() => setModalL1(false)}
        onDetails={() => {
          setModalL1(false);
          setModalL2(true);
        }}
        card={selectedCard}
        data={projectsData.drillDownDetails}
      />
      <DrillDownModalL2
        isOpen={modalL2}
        onClose={() => setModalL2(false)}
        onBack={() => {
          setModalL2(false);
          setModalL1(true);
        }}
        title="تفاصيل المهام والمسؤوليات"
        columns={detailColumns}
        data={projectsData.drillDownDetails}
      />
    </div>
  );
}
