// ── شاشة اختيار العميل (Client Select) ──
import { motion } from 'framer-motion';
import {
  Building2,
  Globe,
  Shield,
  Award,
  ArrowLeft,
  LayoutDashboard,
} from 'lucide-react';
import { clients, currentUser } from '../data/mockData';

const clientIconMap = {
  Building2,
  Globe,
  Shield,
  Award,
};

const colorMap = {
  copper: {
    bg: 'bg-copper',
    light: 'bg-copper/10',
    text: 'text-copper',
    border: 'border-copper/20',
    shadow: 'shadow-copper/10',
  },
  burgundy: {
    bg: 'bg-burgundy',
    light: 'bg-burgundy/10',
    text: 'text-burgundy',
    border: 'border-burgundy/20',
    shadow: 'shadow-burgundy/10',
  },
  olive: {
    bg: 'bg-olive',
    light: 'bg-olive/10',
    text: 'text-olive',
    border: 'border-olive/20',
    shadow: 'shadow-olive/10',
  },
  'golden-olive': {
    bg: 'bg-golden-olive',
    light: 'bg-golden-olive/10',
    text: 'text-golden-olive',
    border: 'border-golden-olive/20',
    shadow: 'shadow-golden-olive/10',
  },
};

export default function ClientSelectScreen({ onSelectClient }) {
  return (
    <div className="min-h-screen bg-cream">
      {/* Header */}
      <header className="h-16 bg-cream/80 backdrop-blur-md border-b border-border flex items-center justify-between px-6">
        <div className="flex items-center gap-3">
          <div className="w-8 h-8 rounded-lg bg-copper flex items-center justify-center">
            <LayoutDashboard className="w-4 h-4 text-white" />
          </div>
          <span className="font-display text-lg font-bold text-espresso">منصة البيانات</span>
        </div>
        <div className="flex items-center gap-3">
          <span className="text-sm text-espresso-muted">{currentUser.name}</span>
          <div className="w-8 h-8 rounded-full bg-copper/20 text-copper flex items-center justify-center text-xs font-bold">
            {currentUser.initials}
          </div>
        </div>
      </header>

      {/* Content */}
      <div className="max-w-5xl mx-auto px-6 py-16">
        <motion.div
          initial={{ opacity: 0, y: -10 }}
          animate={{ opacity: 1, y: 0 }}
          className="text-center mb-14"
        >
          <h1 className="font-display text-3xl font-bold text-espresso mb-3">
            اختر العميل
          </h1>
          <p className="text-espresso-muted">
            اختر العميل الذي تريد عرض لوحة بياناته
          </p>
        </motion.div>

        {/* Client Cards Grid */}
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-6">
          {clients.map((client, idx) => {
            const Icon = clientIconMap[client.icon] || Building2;
            const colors = colorMap[client.color] || colorMap.copper;

            return (
              <motion.button
                key={client.id}
                initial={{ opacity: 0, y: 30 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: idx * 0.1, duration: 0.4 }}
                onClick={() => onSelectClient(client)}
                className="card-shadow card-shadow-hover bg-white rounded-xl p-8 text-right group relative overflow-hidden border border-transparent hover:border-copper/20 transition-all duration-300"
              >
                {/* Decorative background */}
                <div
                  className={`absolute -top-12 -start-12 w-32 h-32 rounded-full opacity-[0.06] ${colors.light} transition-all duration-300 group-hover:scale-125`}
                />

                <div className="relative z-10">
                  {/* Icon */}
                  <div
                    className={`w-14 h-14 rounded-xl ${colors.light} ${colors.text} flex items-center justify-center mb-5 transition-all duration-300 group-hover:scale-110`}
                  >
                    <Icon className="w-7 h-7" />
                  </div>

                  {/* Name */}
                  <h3 className={`text-xl font-bold mb-2 ${colors.text}`}>
                    {client.name}
                  </h3>
                  <p className="text-sm text-espresso-muted mb-6">{client.nameEn}</p>

                  {/* Action Button */}
                  <div
                    className={`inline-flex items-center gap-2 px-5 py-2.5 ${colors.bg} text-white rounded-lg text-sm font-medium transition-all duration-200 shadow-md ${colors.shadow} hover:shadow-lg`}
                  >
                    <span>عرض الداشبورد</span>
                    <ArrowLeft className="w-4 h-4" />
                  </div>
                </div>
              </motion.button>
            );
          })}
        </div>
      </div>
    </div>
  );
}
