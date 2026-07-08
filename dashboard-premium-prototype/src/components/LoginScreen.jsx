// ── شاشة تسجيل الدخول (Login Screen) ──
import { useState } from 'react';
import { motion } from 'framer-motion';
import { LayoutDashboard, Eye, EyeOff, User, Lock } from 'lucide-react';

export default function LoginScreen({ onLogin }) {
  const [showPassword, setShowPassword] = useState(false);
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = (e) => {
    e.preventDefault();
    setIsLoading(true);
    // Simulate login delay
    setTimeout(() => {
      setIsLoading(false);
      onLogin();
    }, 800);
  };

  return (
    <div className="min-h-screen flex">
      {/* Left side — Decorative */}
      <motion.div
        initial={{ opacity: 0 }}
        animate={{ opacity: 1 }}
        transition={{ duration: 0.6 }}
        className="hidden lg:flex w-1/2 bg-cream relative overflow-hidden items-center justify-center"
      >
        {/* Decorative pattern */}
        <div className="absolute inset-0 opacity-[0.03]">
          <div
            className="w-full h-full"
            style={{
              backgroundImage: `
                radial-gradient(circle at 20% 50%, #CD7F32 1px, transparent 1px),
                radial-gradient(circle at 80% 30%, #800020 1px, transparent 1px),
                radial-gradient(circle at 50% 70%, #8B7355 1px, transparent 1px)
              `,
              backgroundSize: '60px 60px',
            }}
          />
        </div>

        {/* Gradient orbs */}
        <div className="absolute -top-40 -start-40 w-96 h-96 bg-copper/5 rounded-full blur-3xl" />
        <div className="absolute -bottom-40 -end-40 w-80 h-80 bg-burgundy/5 rounded-full blur-3xl" />

        {/* Content */}
        <div className="relative z-10 text-center px-12">
          <motion.div
            initial={{ scale: 0.8, opacity: 0 }}
            animate={{ scale: 1, opacity: 1 }}
            transition={{ delay: 0.2, duration: 0.6 }}
          >
            <div className="w-20 h-20 rounded-2xl bg-copper flex items-center justify-center mx-auto mb-6 shadow-lg shadow-copper/20">
              <LayoutDashboard className="w-10 h-10 text-white" />
            </div>
            <h1 className="font-display text-4xl font-bold text-espresso mb-3">
              منصة البيانات
            </h1>
            <p className="text-espresso-muted text-lg max-w-md mx-auto leading-relaxed">
              لوحة تحكم ذكية لتحليل بيانات مؤسستك واتخاذ قرارات استراتيجية مبنية على المعلومات
            </p>

            {/* Feature badges */}
            <div className="flex flex-wrap justify-center gap-3 mt-10">
              {['تحليل فوري', 'تقارير ذكية', 'لوحات مخصصة', 'دعم متعدد'].map((feature, idx) => (
                <span
                  key={idx}
                  className="px-4 py-2 bg-white/60 border border-copper/10 rounded-full text-xs font-medium text-espresso-muted"
                >
                  {feature}
                </span>
              ))}
            </div>
          </motion.div>
        </div>
      </motion.div>

      {/* Right side — Login form */}
      <motion.div
        initial={{ opacity: 0, x: 40 }}
        animate={{ opacity: 1, x: 0 }}
        transition={{ duration: 0.6, delay: 0.1 }}
        className="w-full lg:w-1/2 flex items-center justify-center p-8 bg-gradient-to-br from-cream to-cream-dark"
      >
        <div className="w-full max-w-md">
          {/* Logo (mobile) */}
          <div className="lg:hidden text-center mb-10">
            <div className="w-16 h-16 rounded-2xl bg-copper flex items-center justify-center mx-auto mb-4 shadow-lg shadow-copper/20">
              <LayoutDashboard className="w-8 h-8 text-white" />
            </div>
            <h1 className="font-display text-2xl font-bold text-espresso">منصة البيانات</h1>
          </div>

          {/* Card */}
          <motion.div
            initial={{ y: 20, opacity: 0 }}
            animate={{ y: 0, opacity: 1 }}
            transition={{ delay: 0.3, duration: 0.5 }}
            className="bg-mocha/5 backdrop-blur-sm border border-mocha/10 rounded-2xl p-8 shadow-xl"
          >
            <div className="text-center mb-8">
              <h2 className="text-2xl font-bold text-espresso mb-1">تسجيل الدخول</h2>
              <p className="text-sm text-espresso-muted">أهلاً بك في منصة البيانات</p>
            </div>

            <form onSubmit={handleSubmit} className="space-y-5">
              {/* Username */}
              <div>
                <label className="block text-sm font-medium text-espresso mb-1.5">
                  اسم المستخدم
                </label>
                <div className="relative">
                  <User className="absolute start-3 top-1/2 -translate-y-1/2 w-4 h-4 text-espresso-muted" />
                  <input
                    type="text"
                    value={username}
                    onChange={(e) => setUsername(e.target.value)}
                    placeholder="أدخل اسم المستخدم"
                    className="w-full pe-10 ps-4 py-3 bg-white border border-border-dark rounded-xl text-sm text-espresso placeholder:text-espresso-muted/50 focus:outline-none focus:ring-2 focus:ring-copper/20 focus:border-copper/50 transition-all"
                    required
                  />
                </div>
              </div>

              {/* Password */}
              <div>
                <label className="block text-sm font-medium text-espresso mb-1.5">
                  كلمة المرور
                </label>
                <div className="relative">
                  <Lock className="absolute start-3 top-1/2 -translate-y-1/2 w-4 h-4 text-espresso-muted" />
                  <input
                    type={showPassword ? 'text' : 'password'}
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    placeholder="أدخل كلمة المرور"
                    className="w-full pe-10 ps-10 py-3 bg-white border border-border-dark rounded-xl text-sm text-espresso placeholder:text-espresso-muted/50 focus:outline-none focus:ring-2 focus:ring-copper/20 focus:border-copper/50 transition-all"
                    required
                  />
                  <button
                    type="button"
                    onClick={() => setShowPassword(!showPassword)}
                    className="absolute end-3 top-1/2 -translate-y-1/2 text-espresso-muted hover:text-espresso transition-colors"
                  >
                    {showPassword ? <EyeOff className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
                  </button>
                </div>
              </div>

              {/* Remember & Forgot */}
              <div className="flex items-center justify-between">
                <label className="flex items-center gap-2 cursor-pointer">
                  <input
                    type="checkbox"
                    className="w-4 h-4 rounded border-border-dark text-copper focus:ring-copper/20"
                  />
                  <span className="text-xs text-espresso-muted">تذكرني</span>
                </label>
                <button type="button" className="text-xs text-copper hover:text-copper-light font-medium">
                  نسيت كلمة المرور؟
                </button>
              </div>

              {/* Submit Button */}
              <motion.button
                type="submit"
                whileHover={{ scale: 1.01 }}
                whileTap={{ scale: 0.98 }}
                disabled={isLoading}
                className="w-full py-3.5 bg-copper text-white rounded-xl text-sm font-bold hover:bg-copper-light transition-all duration-200 shadow-md shadow-copper/20 disabled:opacity-70"
              >
                {isLoading ? (
                  <span className="flex items-center justify-center gap-2">
                    <svg className="animate-spin w-4 h-4" viewBox="0 0 24 24">
                      <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" fill="none" />
                      <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
                    </svg>
                    جاري تسجيل الدخول...
                  </span>
                ) : (
                  'تسجيل الدخول'
                )}
              </motion.button>
            </form>

            {/* Demo hint */}
            <p className="text-center text-xs text-espresso-muted/60 mt-6">
              استخدم أي بيانات للدخول — هذا بروتوتايب تجريبي
            </p>
          </motion.div>

          {/* Footer */}
          <p className="text-center text-xs text-espresso-muted/50 mt-8">
            © Dynamic Dashboard Builder — جميع الحقوق محفوظة
          </p>
        </div>
      </motion.div>
    </div>
  );
}
