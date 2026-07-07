import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, Cell } from 'recharts';
import { chartPalette } from '../data/demoData';

export default function BarChartWidget({ data, xKey, yKey, title, height = 300 }) {
  return (
    <div className="bg-bg-card rounded-xl p-6 card-shadow">
      <h3 className="type-heading-1 text-text-primary mb-4">{title}</h3>
      <ResponsiveContainer width="100%" height={height}>
        <BarChart data={data} margin={{ top: 10, right: 10, left: 0, bottom: 0 }}>
          <CartesianGrid strokeDasharray="3 3" stroke="#E8DCD0" vertical={false} />
          <XAxis
            dataKey={xKey}
            tick={{ fontSize: 12, fill: '#6B5344' }}
            tickLine={false}
            axisLine={{ stroke: '#E8DCD0' }}
          />
          <YAxis
            tick={{ fontSize: 12, fill: '#6B5344' }}
            tickLine={false}
            axisLine={false}
            tickFormatter={(value) => `${(value / 1000).toFixed(0)}K`}
          />
          <Tooltip
            contentStyle={{
              backgroundColor: '#FFFFFF',
              border: '1px solid #E8DCD0',
              borderRadius: '8px',
              fontSize: '13px',
            }}
            formatter={(value) => [`${value.toLocaleString()} ريال`, 'القيمة']}
          />
          <Bar dataKey={yKey} radius={[6, 6, 0, 0]}>
            {data.map((entry, index) => (
              <Cell key={`cell-${index}`} fill={chartPalette[index % chartPalette.length]} />
            ))}
          </Bar>
        </BarChart>
      </ResponsiveContainer>
    </div>
  );
}
