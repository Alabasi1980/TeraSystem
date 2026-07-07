const linkGroups = [
  {
    title: 'Store',
    links: ['PS5 Games', 'PS4 Games', 'PS VR2 Games', 'Deals', 'Pre-orders'],
  },
  {
    title: 'Account',
    links: ['Sign In', 'Account Management', 'Wallet', 'Subscription'],
  },
  {
    title: 'Support',
    links: ['Support Center', 'Manualuals', 'Warranty', 'Returns'],
  },
  {
    title: 'Social',
    links: ['PlayStation Blog', 'YouTube', 'Twitter', 'Instagram', 'Facebook'],
  },
  {
    title: 'About',
    links: ['About SIE', 'Careers', 'Press', 'Corporate'],
  },
];

export default function Footer() {
  return (
    <footer className="bg-[var(--color-primary)] text-white">
      <div className="content-container py-12">
        {/* Wordmark */}
        <div className="mb-10">
          <span className="text-2xl font-bold tracking-wider">PlayStation</span>
        </div>

        {/* Link Grid */}
        <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-5 gap-8 mb-10">
          {linkGroups.map((group) => (
            <div key={group.title}>
              <h4 className="type-caption-md font-semibold text-white mb-3">
                {group.title}
              </h4>
              <ul className="space-y-2">
                {group.links.map((link) => (
                  <li
                    key={link}
                    className="type-caption-md text-white/70 hover:text-white cursor-pointer transition-colors"
                  >
                    {link}
                  </li>
                ))}
              </ul>
            </div>
          ))}
        </div>

        {/* Bottom Bar */}
        <div className="border-t border-white/20 pt-6 flex flex-col md:flex-row justify-between items-center gap-4">
          <div className="flex flex-wrap gap-4 text-center md:text-left">
            <span className="type-caption-sm text-white/60 cursor-pointer hover:text-white transition-colors">
              Terms of Service
            </span>
            <span className="type-caption-sm text-white/60 cursor-pointer hover:text-white transition-colors">
              Privacy Policy
            </span>
            <span className="type-caption-sm text-white/60 cursor-pointer hover:text-white transition-colors">
              Cookie Policy
            </span>
            <span className="type-caption-sm text-white/60 cursor-pointer hover:text-white transition-colors">
              Legal
            </span>
          </div>
          <span className="type-caption-sm text-white/60">
            &copy; 2026 Sony Interactive Entertainment Inc. All Rights Reserved.
          </span>
        </div>
      </div>
    </footer>
  );
}
