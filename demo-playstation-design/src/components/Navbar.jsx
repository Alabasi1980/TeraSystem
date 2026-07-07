import { useState } from 'react';

const navLinks = [
  'Games',
  'PS5',
  'PS4',
  'PS VR2',
  'Subscriptions',
  'Hardware',
  'Mobile',
  'News',
  'Shop',
  'Support',
];

export default function Navbar() {
  const [menuOpen, setMenuOpen] = useState(false);

  return (
    <nav className="bg-[var(--color-canvas-dark)] text-[var(--color-on-dark)] h-12 px-4 md:px-6 flex items-center select-none">
      <div className="flex items-center justify-between w-full max-w-[1440px] mx-auto">
        {/* Left: Logo */}
        <div className="flex items-center gap-6">
          {/* PlayStation P-Logo */}
          <svg className="w-8 h-8 cursor-pointer" viewBox="0 0 40 40" fill="none">
            <circle cx="20" cy="20" r="18" fill="white" />
            <text x="20" y="26" textAnchor="middle" fill="#0070d1" fontSize="20" fontWeight="700" fontFamily="Arial">P</text>
          </svg>

          {/* Desktop Nav Links */}
          <ul className="hidden lg:flex items-center gap-5 ml-4">
            {navLinks.map((link) => (
              <li
                key={link}
                className="type-body-strong text-[var(--color-body-dark)] hover:text-white cursor-pointer transition-colors text-[15px]"
              >
                {link}
              </li>
            ))}
          </ul>
        </div>

        {/* Right: Icons */}
        <div className="flex items-center gap-4">
          {/* Search */}
          <svg
            className="w-5 h-5 text-[var(--color-body-dark)] hover:text-white cursor-pointer"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"
            />
          </svg>

          {/* Cart */}
          <svg
            className="w-5 h-5 text-[var(--color-body-dark)] hover:text-white cursor-pointer"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 100 4 2 2 0 000-4z"
            />
          </svg>

          {/* User Avatar */}
          <div className="w-8 h-8 rounded-full bg-[var(--color-surface-dark-elevated)] border border-[var(--color-hairline-dark)] flex items-center justify-center cursor-pointer">
            <svg
              className="w-4 h-4 text-[var(--color-body-dark)]"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"
              />
            </svg>
          </div>

          {/* Hamburger - tablet & below */}
          <button
            className="lg:hidden flex flex-col gap-1 p-1"
            onClick={() => setMenuOpen(!menuOpen)}
            aria-label="Toggle menu"
          >
            <span className="block w-5 h-[2px] bg-[var(--color-on-dark)]" />
            <span className="block w-5 h-[2px] bg-[var(--color-on-dark)]" />
            <span className="block w-5 h-[2px] bg-[var(--color-on-dark)]" />
          </button>
        </div>
      </div>

      {/* Mobile Drawer */}
      {menuOpen && (
        <div className="fixed inset-0 top-12 bg-[var(--color-canvas-dark)] z-50 lg:hidden">
          <ul className="flex flex-col p-6 gap-4">
            {navLinks.map((link) => (
              <li
                key={link}
                className="type-body-strong text-[var(--color-body-dark)] hover:text-white cursor-pointer transition-colors py-2"
                onClick={() => setMenuOpen(false)}
              >
                {link}
              </li>
            ))}
          </ul>
        </div>
      )}
    </nav>
  );
}
