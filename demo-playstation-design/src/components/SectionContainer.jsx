export default function SectionContainer({
  background = 'bg-[var(--color-canvas-dark)]',
  children,
  className = '',
}) {
  return (
    <section className={`${background} section-padding ${className}`}>
      <div className="content-container">{children}</div>
    </section>
  );
}
