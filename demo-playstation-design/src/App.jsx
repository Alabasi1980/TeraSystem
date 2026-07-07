import Navbar from './components/Navbar';
import HeroBandDark from './components/HeroBandDark';
import HeroBandLight from './components/HeroBandLight';
import HeroBandBlue from './components/HeroBandBlue';
import GameTile from './components/GameTile';
import ProductCard from './components/ProductCard';
import PSPlusBanner from './components/PSPlusBanner';
import CardGrid from './components/CardGrid';
import Footer from './components/Footer';

const games = [
  { title: 'Marathon', platform: 'PS5', badge: 'Pre-order', image: 'https://placehold.co/400x225/1a1a2e/e8e8e8?text=Marathon' },
  { title: 'God of War Ragnarök', platform: 'PS5', badge: 'New', image: 'https://placehold.co/400x225/2d1b00/e8e8e8?text=God+of+War' },
  { title: 'Horizon Forbidden West', platform: 'PS5 | PS4', image: 'https://placehold.co/400x225/003c2a/e8e8e8?text=Horizon' },
  { title: 'The Last of Us Part I', platform: 'PS5', badge: 'New', image: 'https://placehold.co/400x225/2e1a1a/e8e8e8?text=The+Last+of+Us' },
  { title: 'Spider-Man 2', platform: 'PS5', badge: 'Coming Soon', image: 'https://placehold.co/400x225/1a002e/e8e8e8?text=Spider-Man+2' },
  { title: 'Gran Turismo 7', platform: 'PS5 | PS4', image: 'https://placehold.co/400x225/001f3f/e8e8e8?text=Gran+Turismo+7' },
  { title: 'Returnal', platform: 'PS5', image: 'https://placehold.co/400x225/1a1a1a/e8e8e8?text=Returnal' },
  { title: 'Ratchet & Clank: Rift Apart', platform: 'PS5', image: 'https://placehold.co/400x225/001a2e/e8e8e8?text=Ratchet+%26+Clank' },
];

const consoles = [
  { name: 'PS5 Console', image: 'https://placehold.co/200x180/f5f7fa/000000?text=PS5' },
  { name: 'PS5 Digital Edition', image: 'https://placehold.co/200x180/f5f7fa/000000?text=PS5+Digital' },
  { name: 'PS5 Pro', image: 'https://placehold.co/200x180/f5f7fa/000000?text=PS5+Pro' },
  { name: 'DualSense Controller', image: 'https://placehold.co/200x180/f5f7fa/000000?text=DualSense' },
  { name: 'Pulse 3D Headset', image: 'https://placehold.co/200x180/f5f7fa/000000?text=Pulse+3D' },
];

const newsItems = [
  { title: 'Marathon reveals new gameplay trailer', category: 'Games', image: 'https://placehold.co/400x225/121314/e8e8e8?text=Marathon+Trailer' },
  { title: 'PS5 system update 24.00 rolling out now', category: 'System', image: 'https://placehold.co/400x225/121314/e8e8e8?text=PS5+Update' },
  { title: 'PlayStation Plus monthly games for July', category: 'PS Plus', image: 'https://placehold.co/400x225/121314/e8e8e8?text=PS+Plus+July' },
];

export default function App() {
  return (
    <div className="min-h-screen bg-[var(--color-canvas-dark)]">
      {/* 1. Primary Nav */}
      <Navbar />

      {/* 2. Dark Hero Band — "Discover all PS5 consoles and accessories" */}
      <HeroBandDark
        title="Discover all PS5 consoles and accessories"
        subtitle="Experience lightning-fast loading with an ultra-high-speed SSD, deeper immersion with haptic feedback, adaptive triggers, and 3D Audio."
        primaryCta="Learn more"
        secondaryCta="Watch trailer"
        imageUrl="https://placehold.co/800x600/121314/ffffff?text=DualSense+%26+PS5+Console"
      />

      {/* 3. Light Console Showcase — "Discover all PS5 consoles and accessories" */}
      <section className="bg-[var(--color-canvas-light)] section-padding">
        <div className="content-container">
          <h2 className="type-display-lg text-[var(--color-ink)] mb-3">
            Discover all PS5 consoles and accessories
          </h2>
          <p className="type-body-md text-[var(--color-body-light)] mb-8 max-w-[600px]">
            Choose your console bundle, add accessories, and customize your setup.
          </p>
          <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-5 gap-6">
            {consoles.map((item) => (
              <div
                key={item.name}
                className="bg-[var(--color-surface-card)] rounded-[var(--radius-md)] p-4 text-center cursor-pointer"
              >
                <img
                  src={item.image}
                  alt={item.name}
                  className="w-full h-32 object-contain mb-3"
                />
                <span className="type-body-sm text-[var(--color-ink)] font-medium">
                  {item.name}
                </span>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* 4. Dark Game Rail — "Great PS4 & PS5 games out now or coming soon" */}
      <section className="bg-[var(--color-canvas-dark)] section-padding">
        <div className="content-container">
          <h2 className="type-display-lg text-[var(--color-on-dark)] mb-3">
            Great PS4 & PS5 games out now or coming soon
          </h2>
          <p className="type-body-md text-[var(--color-body-dark)] mb-8 max-w-[600px]">
            Explore the latest blockbusters, indie gems, and upcoming releases.
          </p>
          <CardGrid cols={4}>
            {games.map((game) => (
              <GameTile
                key={game.title}
                title={game.title}
                platform={game.platform}
                badge={game.badge}
                imageUrl={game.image}
              />
            ))}
          </CardGrid>
        </div>
      </section>

      {/* 5. PS Plus Banner — dark elevated with gold gradient */}
      <PSPlusBanner />

      {/* 6. Light Section — Feature Cards */}
      <section className="bg-[var(--color-canvas-light)] section-padding">
        <div className="content-container">
          <h2 className="type-display-lg text-[var(--color-ink)] mb-8 text-center">
            Explore PlayStation Store
          </h2>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <ProductCard
              title="Latest Deals"
              description="Save big on hundreds of games, add-ons, and more during our summer sale."
              ctaText="Shop deals"
              variant="feature"
              imageUrl="https://placehold.co/200x120/f5f7fa/000000?text=%F0%9F%8F%B7%EF%B8%8F+Deals"
            />
            <ProductCard
              title="Pre-orders"
              description="Secure your copy of the most anticipated upcoming games before launch."
              ctaText="Pre-order now"
              variant="feature"
              imageUrl="https://placehold.co/200x120/f5f7fa/000000?text=%F0%9F%93%A6+Pre-order"
            />
            <ProductCard
              title="PS Plus Catalog"
              description="Explore 400+ PS5 and PS4 games included with your PlayStation Plus membership."
              ctaText="Browse catalog"
              variant="feature"
              imageUrl="https://placehold.co/200x120/f5f7fa/000000?text=%F0%9F%8E%AE+Catalog"
            />
          </div>
        </div>
      </section>

      {/* 7. PlayStation Blue Band — Marathon Launch CTA */}
      <HeroBandBlue
        title="Marathon"
        subtitle="The next evolution of competitive gaming. Pre-order now and get exclusive in-game rewards at launch."
        ctaText="Pre-order now"
        ctaVariant="commerce"
      />

      {/* 8. Dark "ON PLAYSTATION" Anniversary Band */}
      <section className="bg-[var(--color-canvas-dark)] section-padding relative overflow-hidden">
        {/* Gradient overlay like the "ON PLAYSTATION" band */}
        <div className="absolute inset-0 bg-gradient-to-b from-[var(--color-surface-dark-elevated)] to-[var(--color-canvas-dark)] pointer-events-none" />
        <div className="content-container relative z-10 text-center">
          <h2 className="type-heading-xl text-[var(--color-on-dark)] mb-4">
            30 Years of PlayStation
          </h2>
          <p className="type-body-md text-[var(--color-body-dark)] mb-8 max-w-[600px] mx-auto">
            Celebrating three decades of innovation, unforgettable stories, and
            groundbreaking gaming experiences.
          </p>
          <button className="pill-btn bg-transparent text-[var(--color-on-dark)] border border-[var(--color-hairline-dark)] hover:bg-white/10">
            Explore the legacy
          </button>
        </div>
      </section>

      {/* 9. Light News Strip */}
      <section className="bg-[var(--color-canvas-light)] section-padding">
        <div className="content-container">
          <h2 className="type-display-lg text-[var(--color-ink)] mb-3">
            Latest news
          </h2>
          <p className="type-body-md text-[var(--color-body-light)] mb-8 max-w-[600px]">
            Stay up to date with the latest announcements from PlayStation.
          </p>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {newsItems.map((item) => (
              <div
                key={item.title}
                className="bg-[var(--color-surface-card)] rounded-[var(--radius-md)] overflow-hidden cursor-pointer"
              >
                <img
                  src={item.image}
                  alt={item.title}
                  className="w-full aspect-video object-cover"
                />
                <div className="p-4">
                  <span className="type-caption-sm text-[var(--color-primary)] uppercase font-semibold">
                    {item.category}
                  </span>
                  <h3 className="type-heading-lg text-[var(--color-ink)] mt-1">
                    {item.title}
                  </h3>
                </div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* 10. Blue Footer */}
      <Footer />

      {/* Demo watermark for clarity */}
      <div className="fixed bottom-4 right-4 bg-black/80 text-white type-caption-sm px-3 py-1.5 rounded-full z-[999]">
        PlayStation Design System — Demo
      </div>
    </div>
  );
}
