export default function GameTile({
  title = 'Game Title',
  imageUrl = 'https://placehold.co/400x225/181818/ffffff?text=Game+Art',
  platform = 'PS5',
  badge,
}) {
  return (
    <div className="bg-[var(--color-surface-dark-card)] rounded-[var(--radius-md)] overflow-hidden cursor-pointer group transition-transform duration-200 hover:scale-[1.02]">
      {/* 16:9 Cover Art */}
      <div className="relative aspect-video overflow-hidden">
        <img
          src={imageUrl}
          alt={title}
          className="w-full h-full object-cover transition-transform duration-300 group-hover:scale-105"
        />
        {/* Badge */}
        {badge && (
          <span className="absolute top-3 left-3 bg-[var(--color-primary)] text-white type-caption-sm px-[10px] py-[4px] rounded-[var(--radius-full)]">
            {badge}
          </span>
        )}
        {/* Gradient overlay at bottom for text readability */}
        <div className="absolute inset-x-0 bottom-0 h-1/2 bg-gradient-to-t from-black/70 to-transparent" />
      </div>

      {/* Title + Platform */}
      <div className="p-4">
        <h3 className="type-body-sm text-[var(--color-on-dark)] font-medium truncate">
          {title}
        </h3>
        <span className="type-caption-md text-[var(--color-on-dark-mute)]">
          {platform}
        </span>
      </div>
    </div>
  );
}
