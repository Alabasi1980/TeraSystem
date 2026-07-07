import { PieChart, Pie, Cell, Tooltip, ResponsiveContainer, Legend } from 'recharts';
import { chartPalette } from '../data/demoData';

export default function PieChartWidget({ data, title, height = 300 }) {
  return (
    <div className="bg-bg-card rounded-xl p-6 card-shadow">
      <h3 className="type-heading-1 text-text-primary mb-4">{title}</h3>
      <ResponsiveContainer width="100%" height={height}>
        <PieChart>
          <Pie
            data={data}
            cx="50%"
            cy="50%"
            labelLine={false}
            label={({ name, percent }) => `${name} ${(percent * 100).toFixed(0)}%`}
            outerRadius={80}
            fill="#8884d8"
            dataKey="value"
          >
            {data.map((entry, index) => (
              <Cell key={`cell-${index}`} fill={chartPalette[index % chartPalette.length]} />
            ))}
          </Pie>
          <Tooltip
            contentStyle={{
              backgroundColor: '#FFFFFF',
              border: '1px solid #E8DCD0',
              borderRadius: '8px',
              fontSize: '13px',
            }}
            formatter={(value, name) => [`${value} مشروع`, name]}
          />
          <Legend wrapperStyle={{ fontSize: '12px', paddingTop: '8px' }} />
        </PieChart>
      </ResponsiveContainer>
    </div>
  );
}
