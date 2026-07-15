// ── قسم الرسوم البيانية (Charts) ──
import { motion } from 'framer-motion';
import {
  LineChart,
  Line,
  BarChart,
  Bar,
  PieChart,
  Pie,
  Cell,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  Area,
} from 'recharts';
import { monthlySales, salesByCategory, yearlyComparison } from '../data/mockData';

// مكون التولتيب المخصص
const CustomTooltip = ({ active, payload, label }) => {
  if (active && payload && payload.length) {
    return (
      <div className="bg-white rounded-lg shadow-lg border border-border p-3 text-right">
        <p className="text-sm font-semibold text-espresso mb-1">{label}</p>
        {payload.map((entry, idx) => (
          <p key={idx} className="text-sm" style={{ color: entry.color }}>
            {entry.name}: {entry.value.toLocaleString('ar-SA')} ر.س
          </p>
        ))}
      </div>
    );
  }
  return null;
};

// ── الرسم الخطي: المبيعات الشهرية ──
function MonthlySalesChart() {
  return (
    <div className="bg-white rounded-xl p-6 card-shadow">
      <h3 className="text-base font-semibold text-espresso mb-5">المبيعات الشهرية</h3>
      <ResponsiveContainer width="100%" height={280}>
        <LineChart data={monthlySales} margin={{ top: 5, right: 5, left: -10, bottom: 5 }}>
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
            tickFormatter={(val) => `${(val / 1000000).toFixed(1)}م`}
          />
          <Tooltip content={<CustomTooltip />} />
          <defs>
            <linearGradient id="copperGradient" x1="0" y1="0" x2="0" y2="1">
              <stop offset="5%" stopColor="#CD7F32" stopOpacity={0.3} />
              <stop offset="95%" stopColor="#CD7F32" stopOpacity={0.02} />
            </linearGradient>
          </defs>
          <Area
            type="monotone"
            dataKey="value"
            stroke="none"
            fill="url(#copperGradient)"
          />
          <Line
            type="monotone"
            dataKey="value"
            stroke="#CD7F32"
            strokeWidth={2.5}
            dot={{ r: 4, fill: '#CD7F32', stroke: '#fff', strokeWidth: 2 }}
            activeDot={{ r: 6, fill: '#CD7F32', stroke: '#fff', strokeWidth: 2 }}
          />
        </LineChart>
      </ResponsiveContainer>
    </div>
  );
}

// ── الرسم الدائري: توزيع المبيعات ──
function CategoryDonutChart() {
  return (
    <div className="bg-white rounded-xl p-6 card-shadow">
      <h3 className="text-base font-semibold text-espresso mb-5">توزيع حسب الفئة</h3>
      <ResponsiveContainer width="100%" height={280}>
        <PieChart>
          <Pie
            data={salesByCategory}
            cx="50%"
            cy="50%"
            innerRadius={70}
            outerRadius={100}
            paddingAngle={3}
            dataKey="value"
            stroke="none"
          >
            {salesByCategory.map((entry, idx) => (
              <Cell key={idx} fill={entry.color} />
            ))}
          </Pie>
          <Tooltip
            content={({ active, payload }) =>
              active && payload?.[0] ? (
                <div className="bg-white rounded-lg shadow-lg border border-border p-3 text-right">
                  <p className="text-sm font-semibold text-espresso">{payload[0].name}</p>
                  <p className="text-sm" style={{ color: payload[0].color }}>
                    {payload[0].value}%
                  </p>
                </div>
              ) : null
            }
          />
        </PieChart>
      </ResponsiveContainer>
      {/* Legend */}
      <div className="flex flex-wrap justify-center gap-3 mt-3">
        {salesByCategory.map((item, idx) => (
          <div key={idx} className="flex items-center gap-1.5">
            <div className="w-3 h-3 rounded-full" style={{ backgroundColor: item.color }} />
            <span className="text-xs text-espresso-muted">{item.name}</span>
          </div>
        ))}
      </div>
    </div>
  );
}

// ── الرسم الأعمدة: مقارنة سنوية ──
function BranchComparisonChart() {
  return (
    <div className="bg-white rounded-xl p-6 card-shadow">
      <h3 className="text-base font-semibold text-espresso mb-5">مقارنة سنوية حسب الفرع</h3>
      <ResponsiveContainer width="100%" height={280}>
        <BarChart data={yearlyComparison} margin={{ top: 5, right: 5, left: -10, bottom: 5 }}>
          <CartesianGrid strokeDasharray="3 3" stroke="#E8DCD0" opacity={0.5} />
          <XAxis
            dataKey="branch"
            tick={{ fontSize: 11, fill: '#6B5A50' }}
            axisLine={{ stroke: '#E8DCD0' }}
            tickLine={false}
          />
          <YAxis
            tick={{ fontSize: 11, fill: '#6B5A50' }}
            axisLine={false}
            tickLine={false}
            tickFormatter={(val) => `${(val / 1000000).toFixed(1)}م`}
          />
          <Tooltip content={<CustomTooltip />} />
          <Bar
            dataKey="currentYear"
            name="العام الحالي"
            fill="#CD7F32"
            radius={[4, 4, 0, 0]}
            barSize={28}
          />
          <Bar
            dataKey="previousYear"
            name="العام السابق"
            fill="#A0886A"
            radius={[4, 4, 0, 0]}
            barSize={28}
          />
        </BarChart>
      </ResponsiveContainer>
    </div>
  );
}

// ── المكون الرئيسي ──
export default function ChartsSection() {
  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ delay: 0.3, duration: 0.5 }}
      className="grid grid-cols-1 lg:grid-cols-3 gap-6"
    >
      <MonthlySalesChart />
      <CategoryDonutChart />
      <BranchComparisonChart />
    </motion.div>
  );
}
