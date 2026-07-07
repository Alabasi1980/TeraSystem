import Button from './Button';

const tiers = [
  {
    name: 'Essential',
    features: ['Online multiplayer', '2 monthly games', 'Exclusive discounts'],
  },
  {
    name: 'Extra',
    features: ['Everything in Essential', '400+ PS4 & PS5 games', 'Ubisoft+ Classics'],
  },
  {
    name: 'Premium',
    features: ['Everything in Extra', 'Classics catalog', 'Game trials', 'Cloud streaming'],
  },
];

export default function PSPlusBanner() {
  return (
    <section className="bg-[var(--color-surface-dark-elevated)] py-12 md:py-16 relative overflow-hidden">
      {/* Gold gradient accent bar */}
      <div className="gold-gradient h-1.5 w-full absolute top-0 left-0" />

      <div className="content-container relative z-10">
        <div className="text-center mb-12">
          <h2 className="type-heading-xl text-[var(--color-on-dark)] mb-3">
            Discover PlayStation Plus
          </h2>
          <p className="type-body-md text-[var(--color-body-dark)] max-w-[600px] mx-auto">
            Join millions of gamers and unlock monthly games, exclusive discounts, online multiplayer, and more.
          </p>
        </div>

        {/* Tier Cards */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-10">
          {tiers.map((tier) => (
            <div
              key={tier.name}
              className="bg-[var(--color-surface-dark-card)] rounded-[var(--radius-md)] p-6 text-center"
            >
              <h3 className="type-heading-lg text-[var(--color-on-dark)] mb-4">
                {tier.name}
              </h3>
              <ul className="space-y-3 mb-8">
                {tier.features.map((feat) => (
                  <li
                    key={feat}
                    className="type-body-sm text-[var(--color-body-dark)] flex items-center gap-2 justify-center"
                  >
                    <span className="text-[var(--color-primary)]">✓</span>
                    {feat}
                  </li>
                ))}
              </ul>
              <Button variant="primary">Learn more</Button>
            </div>
          ))}
        </div>

        <div className="text-center">
          <Button variant="secondary-dark">Compare all plans</Button>
        </div>
      </div>
    </section>
  );
}
