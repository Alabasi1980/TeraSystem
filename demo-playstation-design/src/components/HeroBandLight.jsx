import Button from './Button';

export default function HeroBandLight({
  title = 'Discover all PS5 consoles and accessories',
  subtitle = 'Experience lightning-fast loading, haptic feedback, and immersive 3D audio with the PlayStation 5 console.',
  primaryCta = 'Learn more',
  secondaryCta = 'Watch trailer',
  imageUrl = 'https://placehold.co/800x600/f5f7fa/000000?text=PS5+Console',
  imagePosition = 'right',
}) {
  return (
    <section className="bg-[var(--color-canvas-light)] text-[var(--color-ink)] section-padding relative overflow-hidden">
      <div className="content-container">
        <div
          className={`flex flex-col ${
            imagePosition === 'right' ? 'lg:flex-row' : 'lg:flex-row-reverse'
          } items-center gap-8 lg:gap-16`}
        >
          {/* Text Column */}
          <div className="flex-1 lg:max-w-[520px]">
            <h1 className="type-display-xl text-[var(--color-ink)] mb-4">
              {title}
            </h1>
            <p className="type-body-md text-[var(--color-body-light)] mb-8 max-w-[480px]">
              {subtitle}
            </p>
            <div className="flex flex-wrap gap-4">
              <Button variant="primary">{primaryCta}</Button>
              <Button variant="secondary-light">{secondaryCta}</Button>
            </div>
          </div>

          {/* Image Column */}
          <div className="flex-1 w-full">
            <img
              src={imageUrl}
              alt={title}
              className="w-full h-auto rounded-[var(--radius-md)] object-cover"
            />
          </div>
        </div>
      </div>
    </section>
  );
}
