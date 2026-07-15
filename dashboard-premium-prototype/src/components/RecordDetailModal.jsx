// ── نافذة تفاصيل السجل (Record Detail Modal) — Level 2 DrillDown ──
import { motion, AnimatePresence } from 'framer-motion';
import { X, ArrowRight, Package, Users, Wallet, TrendingUp } from 'lucide-react';
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
} from 'recharts';
import { branchDetailData } from '../data/mockData';

const summaryCards = [
  {
    key: 'topProduct',
    label: 'أفضل منتج',
    icon: Package,
    color: 'copper',
    format: (v) => v,
  },
  {
    key: 'activeClients',
    label: 'العملاء النشطون',
    icon: Users,
    color: 'olive',
    format: (v) => v.toLocaleString('ar-SA'),
  },
  {
    key: 'avgOrderValue',
    label: 'متوسط قيمة الطلب',
    icon: Wallet,
    color: 'burgundy',
    format: (v) => `${v.toLocaleString('ar-SA')} ر.س`,
  },
  {
    key: 'monthlyGrowth',
    label: 'النمو الشهري',
    icon: TrendingUp,
    color: 'golden-olive',
    format: (v) => `+${v}%`,
  },
];

const colorMap = {
  copper: {
    bg: 'bg-copper/10',
    text: 'text-copper',
    border: 'border-copper/25',
    stroke: '#CD7F32',
  },
  olive: {
    bg: 'bg-olive/10',
    text: 'text-olive',
    border: 'border-olive/25',
    stroke: '#6B8E4E',
  },
  burgundy: {
    bg: 'bg-burgundy/10',
    text: 'text-burgundy',
    border: 'border-burgundy/25',
    stroke: '#800020',
  },
  'golden-olive': {
    bg: 'bg-golden-olive/10',
    text: 'text-golden-olive',
    border: 'border-golden-olive/25',
    stroke: '#B8860B',
  },
};

export default function RecordDetailModal({ isOpen, onClose, onBack, branchName, kpiData: kpi }) {
  const data = branchDetailData[branchName];

  if (!kpi || !data) return null;

  const { summary, topClients, monthlyTrend } = data;

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
            className="fixed inset-0 bg-espresso/60 backdrop-blur-sm z-[110]"
            onClick={onClose}
          />

          {/* Modal */}
          <motion.div
            initial={{ opacity: 0, scale: 0.94, y: 16 }}
            animate={{ opacity: 1, scale: 1, y: 0 }}
            exit={{ opacity: 0, scale: 0.96, y: 16 }}
            transition={{ duration: 0.3, ease: 'easeOut' }}
            className="fixed inset-0 z-[111] flex items-center justify-center p-4 pointer-events-none"
          >
            <div
              className="w-full max-w-[900px] bg-white rounded-2xl shadow-2xl overflow-hidden pointer-events-auto"
              style={{ maxHeight: '88vh' }}
            >
              {/* Header */}
              <div className="flex items-center justify-between px-8 pt-8 pb-4 border-b border-border">
                <div>
                  <div className="flex items-center gap-3 mb-1">
                    <button
                      onClick={onBack}
                      className="flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-xs font-semibold text-espresso-muted hover:text-espresso hover:bg-cream-dark transition-all"
                    >
                      <ArrowRight className="w-3.5 h-3.5" />
                      <span>رجوع</span>
                    </button>
                    <h2 className="text-xl font-bold text-espresso">
                      تفاصيل فرع {branchName}
                    </h2>
                  </div>
                  <p className="text-sm text-espresso-muted">
                    {kpi.title} — {kpi.details}
                  </p>
                </div>
                <button
                  onClick={onClose}
                  className="p-2 rounded-lg text-espresso-muted hover:text-espresso hover:bg-cream-dark transition-all"
                >
                  <X className="w-5 h-5" />
                </button>
              </div>

              {/* Content */}
              <div className="px-8 py-6 overflow-y-auto" style={{ maxHeight: '58vh' }}>
                {/* Summary Cards */}
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
                  {summaryCards.map((card) => {
                    const Icon = card.icon;
                    const colors = colorMap[card.color];
                    return (
                      <div
                        key={card.key}
                        className={`relative overflow-hidden rounded-xl border ${colors.border} ${colors.bg} p-5 transition-all hover:shadow-md card-shadow`}
                        style={{ borderTopWidth: '3px', borderTopColor: colors.stroke }}
                      >
                        <div className="flex items-center justify-between mb-3">
                          <span className="text-xs font-semibold text-espresso-muted">
                            {card.label}
                          </span>
                          <Icon className={`w-5 h-5 ${colors.text}`} />
                        </div>
                        <p className={`text-lg font-bold ${colors.text} leading-tight`}>
                          {card.format(summary[card.key])}
                        </p>
                      </div>
                    );
                  })}
                </div>

                {/* Two Column Layout: Table + Chart */}
                <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                  {/* Top Clients Table */}
                  <div className="bg-cream rounded-xl p-5 border border-border">
                    <h3 className="text-base font-bold text-espresso mb-4">
                      أعلى 5 عملاء — {branchName}
                    </h3>
                    <div className="overflow-x-auto">
                      <table className="w-full text-right">
                        <thead>
                          <tr className="border-b border-border">
                            <th className="pb-3 text-xs font-semibold text-espresso-muted">العميل</th>
                            <th className="pb-3 text-xs font-semibold text-espresso-muted text-left" dir="ltr">القيمة</th>
                          </tr>
                        </thead>
                        <tbody className="divide-y divide-border">
                          {topClients.map((client, idx) => (
                            <tr key={idx} className="hover:bg-white/60 transition-colors">
                              <td className="py-3 text-sm font-medium text-espresso">{client.name}</td>
                              <td className="py-3 text-sm font-semibold text-copper text-left font-mono" dir="ltr">
                                {client.value.toLocaleString('ar-SA')} ر.س
                              </td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>
                  </div>

                  {/* Monthly Trend Chart */}
                  <div className="bg-cream rounded-xl p-5 border border-border">
                    <h3 className="text-base font-bold text-espresso mb-4">
                      الأداء الشهري — {branchName}
                    </h3>
                    <div className="h-[220px]">
                      <ResponsiveContainer width="100%" height="100%">
                        <LineChart
                          data={monthlyTrend}
                          margin={{ top: 10, right: 10, left: 0, bottom: 10 }}
                        >
                          <CartesianGrid strokeDasharray="3 3" stroke="#E8DCD0" opacity={0.5} />
                          <XAxis
                            dataKey="month"
                            tick={{ fontSize: 11, fill: '#6B5A50' }}
                            axisLine={{ stroke: '#E8DCD0' }}
                            tickLine={false}
                          />
                          <YAxis
                            tick={{ fontSize: 11, fill: '#6B5A50' }}
                            axisLine={false}
                            tickLine={false}
                            tickFormatter={(value) =>
                              value >= 1000 ? `${(value / 1000).toLocaleString('ar-SA')}K` : value
                            }
                          />
                          <Tooltip
                            content={({ active, payload, label }) =>
                              active && payload?.[0] ? (
                                <div className="bg-white rounded-lg shadow-lg border border-border p-3 text-right">
                                  <p className="text-sm font-semibold text-espresso mb-1">{label}</p>
                                  <p className="text-sm text-copper">
                                    القيمة: {payload[0].value.toLocaleString('ar-SA')} ر.س
                                  </p>
                                </div>
                              ) : null
                            }
                          />
                          <Line
                            type="monotone"
                            dataKey="value"
                            stroke={colorMap.copper.stroke}
                            strokeWidth={3}
                            dot={{ r: 4, fill: '#FFFFFF', stroke: colorMap.copper.stroke, strokeWidth: 2 }}
                            activeDot={{ r: 6, fill: colorMap.copper.stroke }}
                          />
                        </LineChart>
                      </ResponsiveContainer>
                    </div>
                  </div>
                </div>
              </div>

              {/* Footer */}
              <div className="px-8 py-4 border-t border-border flex items-center justify-between">
                <button
                  onClick={onBack}
                  className="flex items-center gap-2 text-sm font-medium text-copper hover:text-copper-light transition-colors"
                >
                  <ArrowRight className="w-4 h-4" />
                  <span>العودة إلى التحليل التفصيلي</span>
                </button>
                <p className="text-xs text-espresso-muted">
                  آخر تحديث: ٦ يوليو ٢٠٢٦
                </p>
              </div>
            </div>
          </motion.div>
        </>
      )}
    </AnimatePresence>
  );
}
