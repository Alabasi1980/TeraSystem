import Button from './Button';

export default function HeroBandBlue({
  title = 'Marathon Launch',
  subtitle = 'The next evolution of competitive gaming is here.',
  ctaText = 'Pre-order now',
  ctaVariant = 'primary',
}) {
  return (
    <section className="bg-[var(--color-primary)] text-white section-padding">
      <div className="content-container text-center lg:text-left">
        <div className="max-w-[720px] mx-auto lg:mx-0">
          <h2 className="type-display-md text-white mb-4">{title}</h2>
          <p className="type-body-md text-white/80 mb-8 max-w-[520px]">
            {subtitle}
          </p>
          <Button variant={ctaVariant}>{ctaText}</Button>
        </div>
      </div>
    </section>
  );
}
