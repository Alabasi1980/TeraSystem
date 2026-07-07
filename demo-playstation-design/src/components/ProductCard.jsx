import Button from './Button';

export default function ProductCard({
  title = 'PlayStation Store',
  description = 'Discover great deals and discounts on the PlayStation Store.',
  imageUrl,
  ctaText,
  variant = 'light',
  className = '',
}) {
  const isDark = variant === 'dark';

  const containerClass = [
    'rounded-[var(--radius-md)] p-6 lg:p-8',
    isDark
      ? 'bg-[var(--color-surface-dark-card)] text-[var(--color-on-dark)]'
      : variant === 'feature'
        ? 'bg-[var(--color-canvas-light)] text-[var(--color-ink)]'
        : 'bg-[var(--color-surface-card)] text-[var(--color-ink)]',
    className,
  ]
    .filter(Boolean)
    .join(' ');

  const titleClass = isDark
    ? 'type-heading-lg text-[var(--color-on-dark)]'
    : 'type-heading-lg text-[var(--color-ink)]';

  const descClass = isDark
    ? 'type-body-md text-[var(--color-body-dark)]'
    : 'type-body-md text-[var(--color-body-light)]';

  return (
    <div className={containerClass}>
      {imageUrl && (
        <img
          src={imageUrl}
          alt={title}
          className="w-full h-40 object-contain mb-4"
        />
      )}
      <h3 className={`${titleClass} mb-2`}>{title}</h3>
      <p className={`${descClass} mb-6`}>{description}</p>
      {ctaText && (
        <Button variant={isDark ? 'secondary-dark' : 'primary'}>
          {ctaText}
        </Button>
      )}
    </div>
  );
}
