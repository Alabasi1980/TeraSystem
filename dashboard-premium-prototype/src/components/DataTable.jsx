export default function DataTable({ columns, data, onRowClick, maxRows }) {
  const displayData = maxRows ? data.slice(0, maxRows) : data;

  return (
    <div className="bg-bg-card rounded-xl card-shadow overflow-hidden">
      <div className="overflow-x-auto">
        <table className="w-full text-right">
          <thead>
            <tr className="bg-bg-card-warm border-b border-border">
              {columns.map((col) => (
                <th key={col.key} className="px-6 py-3 type-body-sm font-semibold text-text-secondary">
                  {col.label}
                </th>
              ))}
            </tr>
          </thead>
          <tbody>
            {displayData.map((row, idx) => (
              <tr
                key={idx}
                onClick={onRowClick ? () => onRowClick(row) : undefined}
                className={`border-b border-border-light hover:bg-bg-card-warm transition-colors ${
                  onRowClick ? 'cursor-pointer' : ''
                }`}
              >
                {columns.map((col) => (
                  <td key={col.key} className="px-6 py-4 type-body text-text-primary">
                    {col.render ? col.render(row[col.key], row) : row[col.key]}
                  </td>
                ))}
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
