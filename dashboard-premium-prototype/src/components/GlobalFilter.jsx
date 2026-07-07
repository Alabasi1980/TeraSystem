import { useState } from 'react';

export default function GlobalFilter() {
  const [dateFrom, setDateFrom] = useState('2026-06-01');
  const [dateTo, setDateTo] = useState('2026-07-07');

  return (
    <div className="bg-bg-card rounded-xl p-4 card-shadow mb-6 flex flex-wrap items-center gap-4">
      <div className="flex items-center gap-2">
        <svg className="w-5 h-5 text-accent-bronze" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 4a1 1 0 011-1h16a1 1 0 011 1v2.586a1 1 0 01-.293.707l-6.414 6.414a1 1 0 00-.293.707V17l-4 4v-6.586a1 1 0 00-.293-.707L3.293 7.293A1 1 0 013 6.586V4z" />
        </svg>
        <span className="type-heading-2 text-text-primary">الفلاتر العامة</span>
      </div>

      <div className="flex items-center gap-2 flex-1 min-w-[300px]">
        <div className="flex items-center gap-2 px-3 py-2 bg-bg-card-warm rounded-lg border border-border">
          <span className="type-body-sm text-text-secondary">من</span>
          <input
            type="date"
            value={dateFrom}
            onChange={(e) => setDateFrom(e.target.value)}
            className="bg-transparent type-body-sm text-text-primary outline-none"
          />
        </div>
        <div className="flex items-center gap-2 px-3 py-2 bg-bg-card-warm rounded-lg border border-border">
          <span className="type-body-sm text-text-secondary">إلى</span>
          <input
            type="date"
            value={dateTo}
            onChange={(e) => setDateTo(e.target.value)}
            className="bg-transparent type-body-sm text-text-primary outline-none"
          />
        </div>
      </div>

      <div className="flex items-center gap-2 mr-auto">
        <button className="px-4 py-2 bg-accent-bronze text-white rounded-lg type-body-sm font-medium hover:bg-accent-bronze-hover transition-colors">
          تطبيق
        </button>
        <button className="px-4 py-2 bg-bg-card-warm text-text-secondary rounded-lg type-body-sm font-medium border border-border hover:border-accent-bronze transition-colors flex items-center gap-2">
          <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
          </svg>
          تحديث
        </button>
      </div>
    </div>
  );
}
