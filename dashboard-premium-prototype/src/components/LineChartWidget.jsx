import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, Legend } from 'recharts';
import { chartColors } from '../data/demoData';

export default function LineChartWidget({ data, lines, title, height = 300 }) {
  return (
    <div className="bg-bg-card rounded-xl p-6 card-shadow">
      <h3 className="type-heading-1 text-text-primary mb-4">{title}</h3>
      <ResponsiveContainer width="100%" height={height}>
        <LineChart data={data} margin={{ top: 10, right: 10, left: 0, bottom: 0 }}>
          <CartesianGrid strokeDasharray="3 3" stroke="#E8DCD0" vertical={false} />
          <XAxis
            dataKey="month"
            tick={{ fontSize: 11, fill: '#6B5344' }}
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
            formatter={(value, name) => [`${value.toLocaleString()} ريال`, name]}
          />
          <Legend wrapperStyle={{ fontSize: '12px', paddingTop: '8px' }} />
          {lines.map((line, idx) => (
            <Line
              key={idx}
              type="monotone"
              dataKey={line.key}
              name={line.name}
              stroke={line.color}
              strokeWidth={2}
              dot={{ r: 3 }}
              activeDot={{ r: 5 }}
            />
          ))}
        </LineChart>
      </ResponsiveContainer>
    </div>
  );
}
