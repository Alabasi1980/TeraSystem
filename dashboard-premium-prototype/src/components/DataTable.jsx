// ── جدول المعاملات (Data Table) ──
import { motion } from 'framer-motion';
import { recentTransactions } from '../data/mockData';

const statusStyles = {
  success: 'bg-olive/15 text-olive',
  pending: 'bg-copper/15 text-copper-dark',
  warning: 'bg-orange-soft/15 text-orange-soft',
};

export default function DataTable() {
  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ delay: 0.4, duration: 0.5 }}
      className="bg-white rounded-xl card-shadow overflow-hidden"
    >
      {/* Header */}
      <div className="px-6 py-4 border-b border-border">
        <h3 className="text-base font-semibold text-espresso">آخر المعاملات</h3>
      </div>

      {/* Table */}
      <div className="overflow-x-auto">
        <table className="w-full text-right">
          <thead>
            <tr className="bg-cream-dark">
              <th className="px-6 py-3 text-xs font-semibold text-espresso uppercase tracking-wider">
                المعرف
              </th>
              <th className="px-6 py-3 text-xs font-semibold text-espresso uppercase tracking-wider">
                التاريخ
              </th>
              <th className="px-6 py-3 text-xs font-semibold text-espresso uppercase tracking-wider">
                العميل
              </th>
              <th className="px-6 py-3 text-xs font-semibold text-espresso uppercase tracking-wider">
                الفرع
              </th>
              <th className="px-6 py-3 text-xs font-semibold text-espresso uppercase tracking-wider text-right">
                القيمة
              </th>
              <th className="px-6 py-3 text-xs font-semibold text-espresso uppercase tracking-wider text-center">
                الحالة
              </th>
            </tr>
          </thead>
          <tbody className="divide-y divide-border">
            {recentTransactions.map((trx, idx) => (
              <motion.tr
                key={trx.id}
                initial={{ opacity: 0, x: 10 }}
                animate={{ opacity: 1, x: 0 }}
                transition={{ delay: 0.4 + idx * 0.05 }}
                className="hover:bg-copper/[0.02] transition-colors"
              >
                <td className="px-6 py-3.5 text-sm font-mono text-espresso-muted">{trx.id}</td>
                <td className="px-6 py-3.5 text-sm text-espresso">{trx.date}</td>
                <td className="px-6 py-3.5 text-sm font-medium text-espresso">{trx.client}</td>
                <td className="px-6 py-3.5 text-sm text-espresso-muted">{trx.branch}</td>
                <td className="px-6 py-3.5 text-sm font-semibold text-espresso text-right font-mono" dir="ltr">
                  {trx.amount.toLocaleString('ar-SA')} ر.س
                </td>
                <td className="px-6 py-3.5 text-center">
                  <span
                    className={`inline-block px-3 py-1 rounded-full text-xs font-semibold ${
                      statusStyles[trx.statusType] || statusStyles.warning
                    }`}
                  >
                    {trx.status}
                  </span>
                </td>
              </motion.tr>
            ))}
          </tbody>
        </table>
      </div>
    </motion.div>
  );
}
