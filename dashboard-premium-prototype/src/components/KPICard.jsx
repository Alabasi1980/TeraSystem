// ── بطاقة KPI فاخرة مع hover و Framer Motion ──
import { motion } from 'framer-motion';
import {
  TrendingUp,
  TrendingDown,
  PiggyBank,
  UserPlus,
  ShoppingCart,
  Target,
  Star,
} from 'lucide-react';

const iconMap = {
  TrendingUp,
  PiggyBank,
  UserPlus,
  ShoppingCart,
  Target,
  Star,
};

// خريطة الألوان الديناميكية
const accentMap = {
  copper: {
    bg: 'bg-copper/10',
    text: 'text-copper',
    border: 'border-copper',
    badgeUp: 'bg-olive/15 text-olive',
    badgeDown: 'bg-burgundy/15 text-burgundy',
  },
  olive: {
    bg: 'bg-olive/10',
    text: 'text-olive',
    border: 'border-olive',
    badgeUp: 'bg-olive/15 text-olive',
    badgeDown: 'bg-burgundy/15 text-burgundy',
  },
  'orange-soft': {
    bg: 'bg-orange-soft/10',
    text: 'text-orange-soft',
    border: 'border-orange-soft',
    badgeUp: 'bg-olive/15 text-olive',
    badgeDown: 'bg-burgundy/15 text-burgundy',
  },
  burgundy: {
    bg: 'bg-burgundy/10',
    text: 'text-burgundy',
    border: 'border-burgundy',
    badgeUp: 'bg-olive/15 text-olive',
    badgeDown: 'bg-burgundy/15 text-burgundy',
  },
  'golden-olive': {
    bg: 'bg-golden-olive/10',
    text: 'text-golden-olive',
    border: 'border-golden-olive',
    badgeUp: 'bg-olive/15 text-olive',
    badgeDown: 'bg-burgundy/15 text-burgundy',
  },
  'copper-glow': {
    bg: 'bg-copper/10',
    text: 'text-copper',
    border: 'border-copper',
    badgeUp: 'bg-olive/15 text-olive',
    badgeDown: 'bg-burgundy/15 text-burgundy',
  },
};

export default function KPICard({ data, onClick, index = 0 }) {
  const colors = accentMap[data.accentColor] || accentMap.copper;
  const Icon = iconMap[data.icon] || TrendingUp;
  const isUp = data.trend === 'up';

  return (
    <motion.button
      initial={{ opacity: 0, y: 30 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ delay: index * 0.08, duration: 0.4, ease: 'easeOut' }}
      onClick={() => onClick?.(data)}
      className="card-shadow card-shadow-hover bg-white rounded-xl p-6 text-right w-full transition-all duration-300 border-t-[3px] group relative overflow-hidden"
      style={{ borderTopColor: `var(--color-${data.accentColor === 'copper-glow' ? 'copper' : data.accentColor})` }}
      whileHover={{ y: -2 }}
    >
      {/* Background decorative circle */}
      <div
        className={`absolute -top-6 -start-6 w-24 h-24 rounded-full opacity-5 ${colors.bg} transition-all duration-300 group-hover:opacity-10 group-hover:scale-110`}
      />

      {/* Top row: Icon + Badge */}
      <div className="flex items-start justify-between mb-3">
        <div className={`w-10 h-10 rounded-lg ${colors.bg} ${colors.text} flex items-center justify-center`}>
          <Icon className="w-5 h-5" />
        </div>
        <div
          className={`flex items-center gap-1 px-2.5 py-1 rounded-full text-xs font-semibold ${
            isUp ? colors.badgeUp : colors.badgeDown
          }`}
        >
          {isUp ? (
            <TrendingUp className="w-3.5 h-3.5" />
          ) : (
            <TrendingDown className="w-3.5 h-3.5" />
          )}
          <span>{isUp ? '+' : ''}{data.change}%</span>
        </div>
      </div>

      {/* Value */}
      <div className="mb-1">
        <span className="font-display text-[28px] font-bold text-espresso leading-tight">
          {data.value}
        </span>
        {data.prefix && (
          <span className="text-sm text-espresso-muted ms-1 font-medium">{data.prefix}</span>
        )}
      </div>

      {/* Label */}
      <p className="text-sm text-espresso-muted font-medium">{data.title}</p>
    </motion.button>
  );
}
