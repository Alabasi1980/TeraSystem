export default function Button({
  variant = 'primary',
  children,
  onClick,
  disabled = false,
  className = '',
  ...props
}) {
  const baseClass =
    'pill-btn inline-flex items-center justify-center select-none';

  const variants = {
    primary:
      'bg-[var(--color-primary)] text-white hover:bg-[var(--color-primary-pressed)] active:bg-[var(--color-primary-active)]',
    commerce:
      'bg-[var(--color-commerce)] text-white hover:bg-[var(--color-commerce-pressed)]',
    'secondary-light':
      'bg-transparent text-[var(--color-ink)] border border-[var(--color-ash-light)] hover:bg-[var(--color-surface-soft)]',
    'secondary-dark':
      'bg-transparent text-[var(--color-on-dark)] border border-[var(--color-hairline-dark)] hover:bg-white/10',
    disabled:
      'bg-[var(--color-surface-soft)] text-[var(--color-ash-light)] cursor-not-allowed',
  };

  const classes = [
    baseClass,
    disabled ? variants.disabled : variants[variant] || variants.primary,
    className,
  ]
    .filter(Boolean)
    .join(' ');

  return (
    <button
      className={classes}
      onClick={disabled ? undefined : onClick}
      disabled={disabled}
      {...props}
    >
      {children}
    </button>
  );
}
