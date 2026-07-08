// ── Modal تحليلي (Analytic Modal) مع Framer Motion ──
import { useState } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { X, Maximize2, Minimize2, Table2, BarChart3, TrendingUp, TrendingDown } from 'lucide-react';
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  Cell,
} from 'recharts';
import { detailedKpiData } from '../data/mockData';
import RecordDetailModal from './RecordDetailModal';

const chartColors = ['#CD7F32', '#D4876A', '#6B8E4E', '#800020', '#A0886A'];

export default function AnalyticModal({ isOpen, onClose, kpiData: kpi }) {
  const [activeTab, setActiveTab] = useState('table');
  const [isWide, setIsWide] = useState(false);
  const [selectedBranch, setSelectedBranch] = useState(null);
  const [isRecordDetailOpen, setIsRecordDetailOpen] = useState(false);

  if (!kpi) return null;

  const handleRowClick = (branch) => {
    setSelectedBranch(branch);
    setIsRecordDetailOpen(true);
  };

  const handleRecordDetailClose = () => {
    setIsRecordDetailOpen(false);
    setTimeout(() => setSelectedBranch(null), 300);
  };

  const details = detailedKpiData[kpi.id] || [];
  const isUp = kpi.trend === 'up';

  return (
    <AnimatePresence>
      {isOpen && (
        <>
          {/* Overlay */}
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            transition={{ duration: 0.25 }}
            className="fixed inset-0 bg-espresso/50 backdrop-blur-sm z-[100]"
            onClick={onClose}
          />

          {/* Modal */}
          <motion.div
            initial={{ opacity: 0, scale: 0.93 }}
            animate={{ opacity: 1, scale: 1 }}
            exit={{ opacity: 0, scale: 0.93 }}
            transition={{ duration: 0.3, ease: 'easeOut' }}
            className="fixed inset-0 z-[101] flex items-center justify-center p-4 pointer-events-none"
          >
            <div
              className={`w-full bg-white rounded-2xl shadow-2xl overflow-hidden pointer-events-auto ${
                isWide ? 'max-w-[1100px]' : 'max-w-[800px]'
              }`}
              style={{ maxHeight: '85vh' }}
            >
            {/* Header */}
            <div className="flex items-center justify-between px-8 pt-8 pb-4 border-b border-border">
              <div>
                <div className="flex items-center gap-3 mb-1">
                  <h2 className="text-xl font-bold text-espresso">{kpi.title}</h2>
                  <div
                    className={`flex items-center gap-1 px-2.5 py-1 rounded-full text-xs font-semibold ${
                      isUp ? 'bg-olive/15 text-olive' : 'bg-burgundy/15 text-burgundy'
                    }`}
                  >
                    {isUp ? (
                      <TrendingUp className="w-3.5 h-3.5" />
                    ) : (
                      <TrendingDown className="w-3.5 h-3.5" />
                    )}
                    <span>{isUp ? '+' : ''}{kpi.change}%</span>
                  </div>
                </div>
                <p className="text-sm text-espresso-muted">{kpi.details}</p>
              </div>
              <div className="flex items-center gap-2">
                <button
                  onClick={() => setIsWide(!isWide)}
                  className="p-2 rounded-lg text-espresso-muted hover:text-espresso hover:bg-cream-dark transition-all"
                >
                  {isWide ? <Minimize2 className="w-5 h-5" /> : <Maximize2 className="w-5 h-5" />}
                </button>
                <button
                  onClick={onClose}
                  className="p-2 rounded-lg text-espresso-muted hover:text-espresso hover:bg-cream-dark transition-all"
                >
                  <X className="w-5 h-5" />
                </button>
              </div>
            </div>

            {/* Tabs */}
            <div className="flex items-center gap-1 px-8 pt-4">
              <button
                onClick={() => setActiveTab('table')}
                className={`flex items-center gap-2 px-4 py-2.5 rounded-lg text-sm font-medium transition-all ${
                  activeTab === 'table'
                    ? 'bg-copper/10 text-copper'
                    : 'text-espresso-muted hover:text-espresso hover:bg-cream-dark'
                }`}
              >
                <Table2 className="w-4 h-4" />
                <span>جدول تفصيلي</span>
              </button>
              <button
                onClick={() => setActiveTab('chart')}
                className={`flex items-center gap-2 px-4 py-2.5 rounded-lg text-sm font-medium transition-all ${
                  activeTab === 'chart'
                    ? 'bg-copper/10 text-copper'
                    : 'text-espresso-muted hover:text-espresso hover:bg-cream-dark'
                }`}
              >
                <BarChart3 className="w-4 h-4" />
                <span>تحليل رسومي</span>
              </button>
            </div>

            {/* Content */}
            <div className="px-8 py-6 overflow-y-auto" style={{ maxHeight: '50vh' }}>
              {activeTab === 'table' ? (
                /* جدول تفصيلي */
                <div className="overflow-x-auto">
                  <table className="w-full text-right">
                    <thead>
                      <tr className="bg-cream-dark rounded-lg">
                        <th className="px-5 py-3 text-xs font-semibold text-espresso">الفرع</th>
                        <th className="px-5 py-3 text-xs font-semibold text-espresso text-right">القيمة</th>
                        <th className="px-5 py-3 text-xs font-semibold text-espresso text-right">النسبة</th>
                        <th className="px-5 py-3 text-xs font-semibold text-espresso text-center">الحالة</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-border">
                      {details.map((row, idx) => (
                        <tr
                          key={idx}
                          onClick={() => handleRowClick(row.branch)}
                          className="cursor-pointer hover:bg-copper/[0.06] transition-colors"
                        >
                          <td className="px-5 py-4 text-sm font-medium text-espresso">{row.branch}</td>
                          <td className="px-5 py-4 text-sm font-semibold text-espresso text-right font-mono" dir="ltr">
                            {typeof row.value === 'number' && row.value > 100
                              ? row.value.toLocaleString('ar-SA')
                              : row.value}
                            {kpi.prefix && row.value > 100 ? ` ${kpi.prefix}` : ''}
                          </td>
                          <td className="px-5 py-4 text-sm text-espresso text-right">
                            {row.percentage}%
                          </td>
                          <td className="px-5 py-4 text-center">
                            <span
                              className={`inline-block px-3 py-1 rounded-full text-xs font-semibold ${
                                row.status === 'ممتاز'
                                  ? 'bg-olive/15 text-olive'
                                  : row.status === 'جيد'
                                  ? 'bg-copper/15 text-copper-dark'
                                  : 'bg-orange-soft/15 text-orange-soft'
                              }`}
                            >
                              {row.status}
                            </span>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              ) : (
                /* رسم بياني */
                <div className="h-[350px]">
                  <ResponsiveContainer width="100%" height="100%">
                    <BarChart data={details} margin={{ top: 10, right: 10, left: -10, bottom: 10 }}>
                      <CartesianGrid strokeDasharray="3 3" stroke="#E8DCD0" opacity={0.5} />
                      <XAxis
                        dataKey="branch"
                        tick={{ fontSize: 12, fill: '#6B5A50' }}
                        axisLine={{ stroke: '#E8DCD0' }}
                        tickLine={false}
                      />
                      <YAxis
                        tick={{ fontSize: 12, fill: '#6B5A50' }}
                        axisLine={false}
                        tickLine={false}
                      />
                      <Tooltip
                        content={({ active, payload, label }) =>
                          active && payload?.[0] ? (
                            <div className="bg-white rounded-lg shadow-lg border border-border p-3 text-right">
                              <p className="text-sm font-semibold text-espresso mb-1">{label}</p>
                              <p className="text-sm text-copper">
                                القيمة: {payload[0].value}
                                {kpi.prefix ? ` ${kpi.prefix}` : ''}
                              </p>
                            </div>
                          ) : null
                        }
                      />
                      <Bar
                        dataKey="value"
                        radius={[4, 4, 0, 0]}
                        barSize={40}
                      >
                        {details.map((_, idx) => (
                          <Cell key={idx} fill={chartColors[idx % chartColors.length]} />
                        ))}
                      </Bar>
                    </BarChart>
                  </ResponsiveContainer>
                </div>
              )}
            </div>

            {/* Footer */}
            <div className="px-8 py-4 border-t border-border flex items-center justify-between">
              <p className="text-xs text-espresso-muted">
                آخر تحديث: ٦ يوليو ٢٠٢٦
              </p>
              <button
                onClick={() => setIsWide(!isWide)}
                className="text-sm font-medium text-copper hover:text-copper-light transition-colors"
              >
                {isWide ? 'عرض عادي' : 'تفاصيل أوسع ←'}
              </button>
            </div>
            </div>
          </motion.div>

          {/* Level 2: Record Detail Modal */}
          <RecordDetailModal
            isOpen={isRecordDetailOpen}
            onClose={handleRecordDetailClose}
            onBack={() => setIsRecordDetailOpen(false)}
            branchName={selectedBranch}
            kpiData={kpi}
          />
        </>
      )}
    </AnimatePresence>
  );
}
