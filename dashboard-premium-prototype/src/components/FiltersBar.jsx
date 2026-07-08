// ── شريط الفلاتر (Filters Bar) ──
import { useState } from 'react';
import { Calendar, RefreshCw, ChevronDown } from 'lucide-react';
import { filterOptions } from '../data/mockData';

export default function FiltersBar() {
  const [dateRange, setDateRange] = useState(filterOptions.dateRanges[1].value);
  const [branch, setBranch] = useState(filterOptions.branches[0]);
  const [status, setStatus] = useState(filterOptions.statuses[0]);

  const handleRefresh = () => {
    // Mock refresh — just visual feedback
    const btn = document.getElementById('refresh-btn');
    btn.classList.add('rotate-center');
    setTimeout(() => btn.classList.remove('rotate-center'), 600);
  };

  return (
    <div className="bg-cream-dark rounded-xl px-5 py-4 flex items-center justify-between flex-wrap gap-3">
      {/* Filters */}
      <div className="flex items-center gap-3 flex-wrap">
        {/* Date Range */}
        <div className="relative">
          <select
            value={dateRange}
            onChange={(e) => setDateRange(e.target.value)}
            className="appearance-none bg-white border border-border-dark rounded-lg px-4 py-2.5 pe-10 text-sm text-espresso font-medium cursor-pointer hover:border-copper/50 transition-colors focus:outline-none focus:ring-2 focus:ring-copper/20"
          >
            {filterOptions.dateRanges.map((opt) => (
              <option key={opt.value} value={opt.value}>
                {opt.label}
              </option>
            ))}
          </select>
          <Calendar className="absolute start-3 top-1/2 -translate-y-1/2 w-4 h-4 text-espresso-muted pointer-events-none" />
        </div>

        {/* Branch */}
        <div className="relative">
          <select
            value={branch}
            onChange={(e) => setBranch(e.target.value)}
            className="appearance-none bg-white border border-border-dark rounded-lg px-4 py-2.5 pe-10 text-sm text-espresso font-medium cursor-pointer hover:border-copper/50 transition-colors focus:outline-none focus:ring-2 focus:ring-copper/20 min-w-[130px]"
          >
            {filterOptions.branches.map((b) => (
              <option key={b} value={b}>
                {b}
              </option>
            ))}
          </select>
          <ChevronDown className="absolute end-3 top-1/2 -translate-y-1/2 w-4 h-4 text-espresso-muted pointer-events-none" />
        </div>

        {/* Status */}
        <div className="relative">
          <select
            value={status}
            onChange={(e) => setStatus(e.target.value)}
            className="appearance-none bg-white border border-border-dark rounded-lg px-4 py-2.5 pe-10 text-sm text-espresso font-medium cursor-pointer hover:border-copper/50 transition-colors focus:outline-none focus:ring-2 focus:ring-copper/20 min-w-[130px]"
          >
            {filterOptions.statuses.map((s) => (
              <option key={s} value={s}>
                {s}
              </option>
            ))}
          </select>
          <ChevronDown className="absolute end-3 top-1/2 -translate-y-1/2 w-4 h-4 text-espresso-muted pointer-events-none" />
        </div>
      </div>

      {/* Refresh Button */}
      <button
        id="refresh-btn"
        onClick={handleRefresh}
        className="flex items-center gap-2 px-5 py-2.5 bg-copper text-white rounded-lg text-sm font-medium hover:bg-copper-light transition-all duration-200 shadow-sm hover:shadow-copper/20"
      >
        <RefreshCw className="w-4 h-4" />
        <span>تحديث</span>
      </button>

      <style>{`
        .rotate-center {
          animation: rotateCenter 0.6s ease-in-out;
        }
        @keyframes rotateCenter {
          from { transform: rotate(0deg); }
          to { transform: rotate(360deg); }
        }
        #refresh-btn .rotate-center {
          animation: rotateCenter 0.6s ease-in-out;
        }
      `}</style>
    </div>
  );
}
