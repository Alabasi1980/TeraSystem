import Modal from './Modal';
import LineChartWidget from './LineChartWidget';
import DataTable from './DataTable';
import { chartColors } from '../data/demoData';

export default function DrillDownModalL1({ isOpen, onClose, onDetails, card, data }) {
  if (!card) return null;

  const lineData = data || [
    { month: 'يناير', value: 2100000 },
    { month: 'فبراير', value: 2350000 },
    { month: 'مارس', value: 2200000 },
    { month: 'أبريل', value: 2480000 },
    { month: 'مايو', value: 2600000 },
    { month: 'يونيو', value: 2750000 },
  ];

  const tableColumns = [
    { key: 'branch', label: 'الفرع' },
    { key: 'revenue', label: 'الإيراد', render: (v) => `${v.toLocaleString()} ريال` },
    { key: 'target', label: 'المستهدف', render: (v) => `${v.toLocaleString()} ريال` },
    { key: 'change', label: 'التغير' },
    { key: 'status', label: 'الحالة' },
  ];

  return (
    <Modal isOpen={isOpen} onClose={onClose} title={`تحليل: ${card.title}`} size="xl">
      <div className="p-6">
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
          <div>
            <h3 className="type-heading-2 text-text-primary mb-4">تحليل الاتجاه</h3>
            <LineChartWidget
              data={lineData}
              lines={[{ key: 'value', name: 'القيمة', color: chartColors.bronze }]}
              title=""
              height={250}
            />
          </div>
          <div>
            <h3 className="type-heading-2 text-text-primary mb-4">ملخص المؤشر</h3>
            <div className="bg-bg-card-warm rounded-xl p-6">
              <div className="mb-4">
                <p className="type-kpi-label text-text-secondary mb-1">{card.title}</p>
                <p className="type-kpi-value text-text-primary">{card.value}</p>
              </div>
              <div className="space-y-3">
                <div className="flex justify-between">
                  <span className="type-body-sm text-text-muted">التغير</span>
                  <span className="type-body-sm font-medium text-success">{card.change}</span>
                </div>
                <div className="flex justify-between">
                  <span className="type-body-sm text-text-muted">الفترة</span>
                  <span className="type-body-sm">{card.period}</span>
                </div>
                <div className="flex justify-between">
                  <span className="type-body-sm text-text-muted">آخر تحديث</span>
                  <span className="type-body-sm">الآن</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div className="mb-6">
          <h3 className="type-heading-2 text-text-primary mb-4">تفصيل حسب الفرع</h3>
          <DataTable columns={tableColumns} data={data || []} />
        </div>

        <div className="flex justify-end gap-3">
          <button
            onClick={onClose}
            className="px-6 py-2.5 bg-bg-card-warm text-text-secondary rounded-lg type-body-sm font-medium border border-border hover:border-accent-bronze transition-colors"
          >
            إغلاق
          </button>
          <button
            onClick={onDetails}
            className="px-6 py-2.5 bg-accent-bronze text-white rounded-lg type-body-sm font-medium hover:bg-accent-bronze-hover transition-colors"
          >
            عرض التفاصيل الكاملة
          </button>
        </div>
      </div>
    </Modal>
  );
}
