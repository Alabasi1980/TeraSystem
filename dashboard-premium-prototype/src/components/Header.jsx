// ── الشريط العلوي (Header) ──
import { Bell, ChevronLeft, Search } from 'lucide-react';
import { currentUser } from '../data/mockData';

export default function Header({ title, breadcrumbs }) {
  return (
    <header className="h-16 bg-cream/80 backdrop-blur-md border-b border-border flex items-center justify-between px-6 sticky top-0 z-40">
      {/* Right: Breadcrumbs */}
      <div className="flex items-center gap-2 text-sm">
        {breadcrumbs?.map((crumb, idx) => (
          <span key={idx} className="flex items-center gap-2">
            {idx > 0 && (
              <ChevronLeft
                className="w-4 h-4 text-espresso-muted"
                style={{ transform: 'scaleX(-1)' }}
              />
            )}
            <span
              className={
                idx === breadcrumbs.length - 1
                  ? 'text-espresso font-semibold'
                  : 'text-espresso-muted'
              }
            >
              {crumb}
            </span>
          </span>
        ))}
      </div>

      {/* Left: User area */}
      <div className="flex items-center gap-4">
        {/* Search */}
        <button className="p-2 rounded-lg text-espresso-muted hover:text-espresso hover:bg-cream-dark transition-all">
          <Search className="w-5 h-5" />
        </button>

        {/* Notifications */}
        <button className="relative p-2 rounded-lg text-espresso-muted hover:text-espresso hover:bg-cream-dark transition-all">
          <Bell className="w-5 h-5" />
          {currentUser.notifications > 0 && (
            <span className="absolute -top-0.5 -start-0.5 w-4 h-4 bg-burgundy text-white text-[10px] font-bold rounded-full flex items-center justify-center">
              {currentUser.notifications}
            </span>
          )}
        </button>

        {/* User Avatar */}
        <div className="flex items-center gap-3 ps-3 border-s border-border">
          <div className="text-start">
            <p className="text-sm font-semibold text-espresso">{currentUser.name}</p>
            <p className="text-xs text-espresso-muted">{currentUser.role}</p>
          </div>
          <div className="w-9 h-9 rounded-full bg-copper/20 text-copper flex items-center justify-center text-sm font-bold">
            {currentUser.initials}
          </div>
        </div>
      </div>
    </header>
  );
}
