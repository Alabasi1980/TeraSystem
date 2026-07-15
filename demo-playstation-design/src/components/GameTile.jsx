export default function GameTile({
  title = 'Game Title',
  imageUrl = 'https://placehold.co/400x225/121314/ffffff?text=Game+Art',
  platform = 'PS5',
  badge,
}) {
  return (
    <div className="bg-[var(--color-surface-dark-elevated)] rounded-[var(--radius-md)] overflow-hidden cursor-pointer group transition-transform duration-200 hover:scale-[1.02]">
      {/* 16:9 Cover Art — full bleed inside radius */}
      <div className="relative aspect-video overflow-hidden">
        <img
          src={imageUrl}
          alt={title}
          className="w-full h-full object-cover transition-transform duration-300 group-hover:scale-105"
        />

        {/* Gradient overlay at bottom for text readability */}
        <div className="absolute inset-x-0 bottom-0 h-2/3 bg-gradient-to-t from-black/80 via-black/40 to-transparent pointer-events-none" />

        {/* Badge — top-left corner */}
        {badge && (
          <span className="absolute top-3 left-3 bg-[var(--color-primary)] text-white type-caption-sm px-[10px] py-[4px] rounded-[var(--radius-full)] z-10">
            {badge}
          </span>
        )}

        {/* Title + Platform — overlaid at bottom-left INSIDE the image */}
        <div className="absolute inset-x-0 bottom-0 p-4 z-10">
          <h3 className="type-body-sm text-white font-medium truncate drop-shadow-sm">
            {title}
          </h3>
          <span className="type-caption-md text-white/70">
            {platform}
          </span>
        </div>
      </div>
    </div>
  );
}
