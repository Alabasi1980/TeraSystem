import Modal from './Modal';
import DataTable from './DataTable';

export default function DrillDownModalL2({ isOpen, onClose, onBack, title, columns, data }) {
  return (
    <Modal isOpen={isOpen} onClose={onClose} title={title || 'تفاصيل كاملة'} size="xl">
      <div className="p-6">
        <div className="mb-6">
          <DataTable columns={columns} data={data || []} />
        </div>

        <div className="flex justify-between items-center">
          <button
            onClick={onClose}
            className="px-6 py-2.5 bg-bg-card-warm text-text-secondary rounded-lg type-body-sm font-medium border border-border hover:border-accent-bronze transition-colors flex items-center gap-2"
          >
            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
            إغلاق
          </button>
          <div className="flex items-center gap-3">
            <button
              onClick={onBack}
              className="px-6 py-2.5 bg-bg-card-warm text-text-secondary rounded-lg type-body-sm font-medium border border-border hover:border-accent-bronze transition-colors flex items-center gap-2"
            >
              <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10 19l-7-7m0 0l7-7m-7 7h18" />
              </svg>
              رجوع
            </button>
            <button className="px-6 py-2.5 bg-accent-bronze text-white rounded-lg type-body-sm font-medium hover:bg-accent-bronze-hover transition-colors flex items-center gap-2">
              <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 10v6m0 0l3-3m-3 3l-3-3m2-8a9 9 0 100 18 9 9 0 000-18z" />
              </svg>
              تصدير Excel
            </button>
          </div>
        </div>
      </div>
    </Modal>
  );
}
