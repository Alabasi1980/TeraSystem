export default function CardGrid({ children, cols = 4, className = '' }) {
  const gridCols = {
    1: 'grid-cols-1',
    2: 'grid-cols-1 sm:grid-cols-2',
    3: 'grid-cols-1 sm:grid-cols-2 lg:grid-cols-3',
    4: 'grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4',
  };

  return (
    <div
      className={`grid ${gridCols[cols] || gridCols[4]} gap-6 ${className}`}
    >
      {children}
    </div>
  );
}
