// ── جميع البيانات الوهمية للبروتوتايب ──
// Dynamic Dashboard Builder — Premium Prototype

// ── عملاء متعددين (Multi-Tenant) ──
export const clients = [
  {
    id: 'client-1',
    name: 'شركة الأفق',
    nameEn: 'Al-Ufuq Company',
    icon: 'Building2',
    color: 'copper',
    bgClass: 'bg-copper/10',
    textClass: 'text-copper',
    borderClass: 'border-copper/30',
  },
  {
    id: 'client-2',
    name: 'مجموعة النور',
    nameEn: 'Al-Noor Group',
    icon: 'Globe',
    color: 'burgundy',
    bgClass: 'bg-burgundy/10',
    textClass: 'text-burgundy',
    borderClass: 'border-burgundy/30',
  },
  {
    id: 'client-3',
    name: 'مؤسسة السلام',
    nameEn: 'Al-Salam Foundation',
    icon: 'Shield',
    color: 'olive',
    bgClass: 'bg-olive/10',
    textClass: 'text-olive',
    borderClass: 'border-olive/30',
  },
  {
    id: 'client-4',
    name: 'شركة التميز',
    nameEn: 'Al-Tamayuz Company',
    icon: 'Award',
    color: 'golden-olive',
    bgClass: 'bg-golden-olive/10',
    textClass: 'text-golden-olive',
    borderClass: 'border-golden-olive/30',
  },
];

// ── بيانات المستخدم ──
export const currentUser = {
  name: 'أحمد آل سعود',
  role: 'مدير البيانات',
  avatar: null, // placeholder — سيستخدم الأحرف الأولى
  initials: 'أ.س',
  notifications: 3,
};

// ── بيانات الصفحات والمسارات (للشريط الجانبي) ──
export const sidebarItems = [
  { id: 'dashboard', label: 'لوحة القيادة', icon: 'LayoutDashboard', screen: 'dashboard' },
  { id: 'analytics', label: 'التحليلات', icon: 'BarChart3', screen: 'dashboard' },
  { id: 'reports', label: 'التقارير', icon: 'FileText', screen: 'dashboard' },
  { id: 'clients', label: 'العملاء', icon: 'Users', screen: 'clients' },
  { id: 'settings', label: 'الإعدادات', icon: 'Settings', screen: 'admin' },
];

// ── بيانات KPI (الست بطاقات) ──
export const kpiData = [
  {
    id: 'kpi-1',
    title: 'إجمالي المبيعات',
    value: '12,847,500',
    prefix: 'ر.س',
    change: 12.5,
    trend: 'up',
    icon: 'TrendingUp',
    accentColor: 'copper',
    details: 'إجمالي المبيعات لكافة الفروع خلال الفترة المحددة',
  },
  {
    id: 'kpi-2',
    title: 'صافي الإيراد',
    value: '5,623,800',
    prefix: 'ر.س',
    change: 8.3,
    trend: 'up',
    icon: 'PiggyBank',
    accentColor: 'olive',
    details: 'صافي الإيراد بعد خصم المصروفات والتكاليف',
  },
  {
    id: 'kpi-3',
    title: 'العملاء الجدد',
    value: '2,847',
    prefix: '',
    change: 24.7,
    trend: 'up',
    icon: 'UserPlus',
    accentColor: 'orange-soft',
    details: 'عدد العملاء الجدد المسجلين خلال الفترة',
  },
  {
    id: 'kpi-4',
    title: 'الطلبات النشطة',
    value: '1,293',
    prefix: '',
    change: 3.2,
    trend: 'down',
    icon: 'ShoppingCart',
    accentColor: 'burgundy',
    details: 'الطلبات قيد التنفيذ حالياً',
  },
  {
    id: 'kpi-5',
    title: 'نسبة الإنجاز',
    value: '92.4',
    prefix: '%',
    change: 5.1,
    trend: 'up',
    icon: 'Target',
    accentColor: 'golden-olive',
    details: 'نسبة إنجاز الأهداف المخططة للربع الحالي',
  },
  {
    id: 'kpi-6',
    title: 'معدل الرضا',
    value: '4.8',
    prefix: '/5',
    change: 1.2,
    trend: 'up',
    icon: 'Star',
    accentColor: 'copper-glow',
    details: 'متوسط تقييم العملاء للخدمات المقدمة',
  },
];

// ── المبيعات الشهرية (12 شهر) للرسم الخطي ──
export const monthlySales = [
  { month: 'يناير', value: 820000, previousYear: 720000 },
  { month: 'فبراير', value: 910000, previousYear: 780000 },
  { month: 'مارس', value: 1050000, previousYear: 850000 },
  { month: 'إبريل', value: 985000, previousYear: 820000 },
  { month: 'مايو', value: 1120000, previousYear: 900000 },
  { month: 'يونيو', value: 1180000, previousYear: 950000 },
  { month: 'يوليو', value: 1250000, previousYear: 980000 },
  { month: 'أغسطس', value: 1300000, previousYear: 1020000 },
  { month: 'سبتمبر', value: 1150000, previousYear: 960000 },
  { month: 'أكتوبر', value: 1220000, previousYear: 1000000 },
  { month: 'نوفمبر', value: 1350000, previousYear: 1080000 },
  { month: 'ديسمبر', value: 1420000, previousYear: 1120000 },
];

// ── توزيع المبيعات حسب الفئة (للدونات) ──
export const salesByCategory = [
  { name: 'المنتجات الرقمية', value: 35, color: '#CD7F32' },
  { name: 'الاستشارات', value: 25, color: '#D4876A' },
  { name: 'خدمات الصيانة', value: 20, color: '#6B8E4E' },
  { name: 'الاشتراكات', value: 12, color: '#800020' },
  { name: 'أخرى', value: 8, color: '#A0886A' },
];

// ── المقارنة السنوية (للأعمدة) ──
export const yearlyComparison = [
  { branch: 'الرياض', currentYear: 4200000, previousYear: 3500000 },
  { branch: 'جدة', currentYear: 3800000, previousYear: 3200000 },
  { branch: 'الدمام', currentYear: 2900000, previousYear: 2500000 },
  { branch: 'مكة', currentYear: 2100000, previousYear: 1800000 },
  { branch: 'المدينة', currentYear: 1800000, previousYear: 1500000 },
];

// ── آخر المعاملات (للجدول) ──
export const recentTransactions = [
  {
    id: 'TRX-001',
    date: '٢٠٢٦-٠٧-٠٦',
    client: 'شركة الأفق',
    branch: 'الرياض',
    amount: 284500,
    status: 'مكتمل',
    statusType: 'success',
  },
  {
    id: 'TRX-002',
    date: '٢٠٢٦-٠٧-٠٥',
    client: 'مجموعة النور',
    branch: 'جدة',
    amount: 156000,
    status: 'مكتمل',
    statusType: 'success',
  },
  {
    id: 'TRX-003',
    date: '٢٠٢٦-٠٧-٠٤',
    client: 'مؤسسة السلام',
    branch: 'الدمام',
    amount: 423000,
    status: 'قيد التنفيذ',
    statusType: 'pending',
  },
  {
    id: 'TRX-004',
    date: '٢٠٢٦-٠٧-٠٣',
    client: 'شركة التميز',
    branch: 'الرياض',
    amount: 97500,
    status: 'معلق',
    statusType: 'warning',
  },
  {
    id: 'TRX-005',
    date: '٢٠٢٦-٠٧-٠٢',
    client: 'شركة الأفق',
    branch: 'مكة',
    amount: 612000,
    status: 'مكتمل',
    statusType: 'success',
  },
];

// ── بيانات تفصيلية للـ Modal (جدول تفصيلي) ──
export const detailedKpiData = {
  'kpi-1': [
    { branch: 'الرياض', value: 4250000, percentage: 33.1, status: 'ممتاز' },
    { branch: 'جدة', value: 3840000, percentage: 29.9, status: 'ممتاز' },
    { branch: 'الدمام', value: 2120000, percentage: 16.5, status: 'جيد' },
    { branch: 'مكة', value: 1485000, percentage: 11.6, status: 'جيد' },
    { branch: 'المدينة', value: 1150000, percentage: 8.9, status: 'متوسط' },
  ],
  'kpi-2': [
    { branch: 'الرياض', value: 1890000, percentage: 33.6, status: 'ممتاز' },
    { branch: 'جدة', value: 1620000, percentage: 28.8, status: 'ممتاز' },
    { branch: 'الدمام', value: 985000, percentage: 17.5, status: 'جيد' },
    { branch: 'مكة', value: 623800, percentage: 11.1, status: 'جيد' },
    { branch: 'المدينة', value: 505000, percentage: 9.0, status: 'متوسط' },
  ],
  'kpi-3': [
    { branch: 'الرياض', value: 892, percentage: 31.3, status: 'ممتاز' },
    { branch: 'جدة', value: 745, percentage: 26.2, status: 'ممتاز' },
    { branch: 'الدمام', value: 512, percentage: 18.0, status: 'جيد' },
    { branch: 'مكة', value: 398, percentage: 14.0, status: 'جيد' },
    { branch: 'المدينة', value: 300, percentage: 10.5, status: 'متوسط' },
  ],
  'kpi-4': [
    { branch: 'الرياض', value: 423, percentage: 32.7, status: 'جيد' },
    { branch: 'جدة', value: 356, percentage: 27.5, status: 'جيد' },
    { branch: 'الدمام', value: 245, percentage: 18.9, status: 'ممتاز' },
    { branch: 'مكة', value: 153, percentage: 11.8, status: 'متوسط' },
    { branch: 'المدينة', value: 116, percentage: 9.0, status: 'جيد' },
  ],
  'kpi-5': [
    { branch: 'الرياض', value: 95.2, percentage: 25.8, status: 'ممتاز' },
    { branch: 'جدة', value: 93.8, percentage: 24.5, status: 'ممتاز' },
    { branch: 'الدمام', value: 91.5, percentage: 20.0, status: 'جيد' },
    { branch: 'مكة', value: 90.1, percentage: 16.2, status: 'جيد' },
    { branch: 'المدينة', value: 88.7, percentage: 13.5, status: 'جيد' },
  ],
  'kpi-6': [
    { branch: 'الرياض', value: 4.9, percentage: 20.8, status: 'ممتاز' },
    { branch: 'جدة', value: 4.8, percentage: 20.4, status: 'ممتاز' },
    { branch: 'الدمام', value: 4.8, percentage: 20.4, status: 'ممتاز' },
    { branch: 'مكة', value: 4.7, percentage: 19.4, status: 'جيد' },
    { branch: 'المدينة', value: 4.6, percentage: 19.0, status: 'جيد' },
  ],
};

// ── بيانات الإدارة (Admin) ──
export const adminData = {
  totalClients: 4,
  totalUsers: 28,
  totalDashboards: 12,
  totalDataSources: 8,
  clientsList: [
    { id: 1, name: 'شركة الأفق', status: 'نشط', users: 8, dashboards: 3 },
    { id: 2, name: 'مجموعة النور', status: 'نشط', users: 7, dashboards: 4 },
    { id: 3, name: 'مؤسسة السلام', status: 'نشط', users: 6, dashboards: 3 },
    { id: 4, name: 'شركة التميز', status: 'تجريبي', users: 7, dashboards: 2 },
  ],
};

// ── بيانات تفصيلية لكل فرع (Level 2 DrillDown) ──
export const branchDetailData = {
  'الرياض': {
    summary: {
      topProduct: 'نظام ERP المتقدم',
      activeClients: 142,
      avgOrderValue: 30500,
      monthlyGrowth: 12.8,
    },
    topClients: [
      { name: 'شركة النور', value: 485000 },
      { name: 'مؤسسة الأمل', value: 372000 },
      { name: 'مجموعة السلام', value: 298000 },
      { name: 'شركة التميز', value: 245000 },
      { name: 'مؤسسة الريادة', value: 189000 },
    ],
    monthlyTrend: [
      { month: 'يناير', value: 320000 },
      { month: 'فبراير', value: 345000 },
      { month: 'مارس', value: 380000 },
      { month: 'إبريل', value: 365000 },
      { month: 'مايو', value: 410000 },
      { month: 'يونيو', value: 425000 },
    ],
  },
  'جدة': {
    summary: {
      topProduct: 'حلول التخزين السحابي',
      activeClients: 118,
      avgOrderValue: 28200,
      monthlyGrowth: 9.4,
    },
    topClients: [
      { name: 'مجموعة الفهد', value: 412000 },
      { name: 'شركة البناء الحديث', value: 356000 },
      { name: 'مؤسسة السحابة', value: 284000 },
      { name: 'شركة الإبداع', value: 231000 },
      { name: 'مؤسسة المستقبل', value: 178000 },
    ],
    monthlyTrend: [
      { month: 'يناير', value: 290000 },
      { month: 'فبراير', value: 310000 },
      { month: 'مارس', value: 335000 },
      { month: 'إبريل', value: 340000 },
      { month: 'مايو', value: 360000 },
      { month: 'يونيو', value: 380000 },
    ],
  },
  'الدمام': {
    summary: {
      topProduct: 'خدمات الأمن السيبراني',
      activeClients: 87,
      avgOrderValue: 26400,
      monthlyGrowth: 7.2,
    },
    topClients: [
      { name: 'شركة البترول الوطنية', value: 298000 },
      { name: 'مؤسسة الصناعات', value: 245000 },
      { name: 'شركة الخليج', value: 198000 },
      { name: 'مجموعة الأمان', value: 165000 },
      { name: 'شركة التقنية المتقدمة', value: 132000 },
    ],
    monthlyTrend: [
      { month: 'يناير', value: 160000 },
      { month: 'فبراير', value: 175000 },
      { month: 'مارس', value: 185000 },
      { month: 'إبريل', value: 180000 },
      { month: 'مايو', value: 200000 },
      { month: 'يونيو', value: 210000 },
    ],
  },
  'مكة': {
    summary: {
      topProduct: 'منصة إدارة الموارد',
      activeClients: 64,
      avgOrderValue: 23800,
      monthlyGrowth: 5.8,
    },
    topClients: [
      { name: 'شركة الزهراء', value: 215000 },
      { name: 'مؤسسة الحرمين', value: 178000 },
      { name: 'مجموعة الأبراج', value: 146000 },
      { name: 'شركة الساعي', value: 124000 },
      { name: 'مؤسسة الرؤية', value: 98000 },
    ],
    monthlyTrend: [
      { month: 'يناير', value: 110000 },
      { month: 'فبراير', value: 125000 },
      { month: 'مارس', value: 130000 },
      { month: 'إبريل', value: 135000 },
      { month: 'مايو', value: 140000 },
      { month: 'يونيو', value: 148000 },
    ],
  },
  'المدينة': {
    summary: {
      topProduct: 'نظام إدارة العلاقات',
      activeClients: 53,
      avgOrderValue: 22100,
      monthlyGrowth: 4.3,
    },
    topClients: [
      { name: 'شركة المدينة المنورة', value: 185000 },
      { name: 'مؤسسة الأصالة', value: 152000 },
      { name: 'مجموعة النخيل', value: 128000 },
      { name: 'شركة الهدى', value: 105000 },
      { name: 'مؤسسة الإيمان', value: 87000 },
    ],
    monthlyTrend: [
      { month: 'يناير', value: 90000 },
      { month: 'فبراير', value: 98000 },
      { month: 'مارس', value: 105000 },
      { month: 'إبريل', value: 108000 },
      { month: 'مايو', value: 112000 },
      { month: 'يونيو', value: 115000 },
    ],
  },
};

// ── خيارات الفلاتر ──
export const filterOptions = {
  branches: ['جميع الفروع', 'الرياض', 'جدة', 'الدمام', 'مكة', 'المدينة'],
  statuses: ['جميع الحالات', 'مكتمل', 'قيد التنفيذ', 'معلق', 'ملغي'],
  dateRanges: [
    { label: 'آخر ٧ أيام', value: '7d' },
    { label: 'آخر ٣٠ يوماً', value: '30d' },
    { label: 'آخر ٩٠ يوماً', value: '90d' },
    { label: 'هذا العام', value: 'year' },
    { label: 'مخصص', value: 'custom' },
  ],
};
