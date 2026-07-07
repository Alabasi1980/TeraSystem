import { useState } from 'react';
import { operationsData } from '../data/demoData';
import GlobalFilter from './GlobalFilter';
import KPICard from './KPICard';
import LineChartWidget from './LineChartWidget';
import BarChartWidget from './BarChartWidget';
import DataTable from './DataTable';
import DrillDownModalL1 from './DrillDownModalL1';
import DrillDownModalL2 from './DrillDownModalL2';
import { chartColors } from '../data/demoData';

const statusRender = (status) => {
  const colors = {
    'مكتمل': 'text-success',
    'قيد التنفيذ': 'text-accent-bronze',
    'قيد المراجعة': 'text-warning',
  };
  return <span className={`font-medium ${colors[status] || 'text-text-primary'}`}>{status}</span>;
};

const amountRender = (amount) => `${amount.toLocaleString()} ريال`;

export default function DashboardOperations() {
  const [modalL1, setModalL1] = useState(false);
  const [modalL2, setModalL2] = useState(false);
  const [selectedCard, setSelectedCard] = useState(null);

  const handleCardClick = (card) => {
    setSelectedCard(card);
    setModalL1(true);
  };

  const columns = [
    { key: 'id', label: 'الرقم المرجعي' },
    { key: 'client', label: 'العميل' },
    { key: 'project', label: 'المشروع' },
    { key: 'amount', label: 'المبلغ', render: amountRender },
    { key: 'status', label: 'الحالة', render: statusRender },
    { key: 'date', label: 'التاريخ' },
  ];

  const detailColumns = [
    { key: 'id', label: 'الرقم' },
    { key: 'branch', label: 'الفرع' },
    { key: 'revenue', label: 'الإيراد', render: amountRender },
    { key: 'target', label: 'المستهدف', render: amountRender },
    { key: 'change', label: 'التغير' },
    { key: 'status', label: 'الحالة' },
  ];

  return (
    <div className="animate-fadeIn">
      {/* Header */}
      <div className="mb-6">
        <h1 className="type-display-1 text-text-primary">لوحة العمليات</h1>
        <p className="type-body text-text-secondary mt-1">
          نظرة شاملة على الأداء التشغيلي والمؤشرات الرئيسية
        </p>
      </div>

      <GlobalFilter />

      {/* KPI Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-6">
        {operationsData.kpis.map((kpi) => (
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

      {/* Charts Row */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
        <LineChartWidget
          data={operationsData.revenueTrend}
          lines={[
            { key: 'revenue', name: 'الإيرادات', color: chartColors.bronze },
            { key: 'target', name: 'المستهدف', color: chartColors.mocha },
          ]}
          title="الاتجاه الشهري للإيرادات"
        />
        <BarChartWidget
          data={operationsData.revenueByCategory}
          xKey="category"
          yKey="value"
          title="الإيرادات حسب الفئة"
        />
      </div>

      {/* Data Table */}
      <div>
        <h2 className="type-heading-1 text-text-primary mb-4">أحدث الحركات المالية</h2>
        <DataTable
          columns={columns}
          data={operationsData.recentTransactions}
          maxRows={6}
          onRowClick={() => {
            setSelectedCard({ title: 'تفاصيل الحركة', value: '', change: '', period: '' });
            setModalL2(true);
          }}
        />
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
        data={operationsData.drillDownDetails}
      />
      <DrillDownModalL2
        isOpen={modalL2}
        onClose={() => setModalL2(false)}
        onBack={() => {
          setModalL2(false);
          setModalL1(true);
        }}
        title="تفاصيل الإيرادات حسب الفرع"
        columns={detailColumns}
        data={operationsData.drillDownDetails}
      />
    </div>
  );
}
