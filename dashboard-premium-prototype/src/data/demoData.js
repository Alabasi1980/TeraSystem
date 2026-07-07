// Demo data for the Dynamic Dashboard Builder prototype

export const operationsData = {
  kpis: [
    {
      id: 'revenue',
      title: 'إجمالي الإيرادات',
      value: '2,847,500',
      unit: 'ريال',
      change: '12.5%',
      changeType: 'up',
      period: 'مقارنة بالشهر الماضي',
      icon: 'revenue',
    },
    {
      id: 'projects',
      title: 'المشاريع النشطة',
      value: '24',
      change: '8.3%',
      changeType: 'up',
      period: 'مقارنة بالربع الماضي',
      icon: 'projects',
    },
    {
      id: 'team',
      title: 'فريق العمل',
      value: '186',
      change: '3 أعضاء جدد',
      changeType: 'up',
      period: 'هذا الشهر',
      icon: 'team',
    },
    {
      id: 'tasks',
      title: 'المهام المنجزة',
      value: '1,423',
      change: '94.2%',
      changeType: 'up',
      period: 'من المستهدف الشهري',
      icon: 'tasks',
    },
  ],

  revenueTrend: [
    { month: 'يناير', revenue: 2100000, target: 2200000 },
    { month: 'فبراير', revenue: 2350000, target: 2250000 },
    { month: 'مارس', revenue: 2200000, target: 2300000 },
    { month: 'أبريل', revenue: 2480000, target: 2350000 },
    { month: 'مايو', revenue: 2600000, target: 2400000 },
    { month: 'يونيو', revenue: 2750000, target: 2450000 },
    { month: 'يوليو', revenue: 2690000, target: 2500000 },
    { month: 'أغسطس', revenue: 2820000, target: 2550000 },
    { month: 'سبتمبر', revenue: 2900000, target: 2600000 },
    { month: 'أكتوبر', revenue: 2780000, target: 2650000 },
    { month: 'نوفمبر', revenue: 2950000, target: 2700000 },
    { month: 'ديسمبر', revenue: 2847500, target: 2750000 },
  ],

  revenueByCategory: [
    { category: 'مقاولات', value: 1240000 },
    { category: 'صيانة', value: 860000 },
    { category: 'استشارات', value: 490000 },
    { category: 'أخرى', value: 257500 },
  ],

  recentTransactions: [
    { id: 'TXN-001', client: 'مجموعة الراجحي', project: 'برج الأعمال', amount: 485000, status: 'مكتمل', date: '2026-07-05' },
    { id: 'TXN-002', client: 'أمانة الرياض', project: 'تطوير طرق', amount: 320000, status: 'قيد المراجعة', date: '2026-07-04' },
    { id: 'TXN-003', client: 'مستشفى الملك', project: 'تجهيزات', amount: 175000, status: 'مكتمل', date: '2026-07-03' },
    { id: 'TXN-004', client: 'شركة الأفق', project: 'مجمع سكني', amount: 640000, status: 'قيد التنفيذ', date: '2026-07-02' },
    { id: 'TXN-005', client: 'مؤسسة البناء', project: 'جسور', amount: 210000, status: 'مكتمل', date: '2026-07-01' },
    { id: 'TXN-006', client: 'مدينة المعرفة', project: 'بنية تحتية', amount: 380000, status: 'قيد المراجعة', date: '2026-06-30' },
  ],

  drillDownDetails: [
    { id: 'BR-001', branch: 'الرياض', revenue: 820000, target: 780000, change: '5.1%', status: 'متجاوز' },
    { id: 'BR-002', branch: 'جدة', revenue: 650000, target: 640000, change: '1.6%', status: 'متجاوز' },
    { id: 'BR-003', branch: 'الدمام', revenue: 410000, target: 430000, change: '-4.7%', status: 'تحت المستهدف' },
    { id: 'BR-004', branch: 'مكة', revenue: 380000, target: 375000, change: '1.3%', status: 'متجاوز' },
    { id: 'BR-005', branch: 'المدينة', revenue: 287500, target: 300000, change: '-4.2%', status: 'تحت المستهدف' },
  ],
};

export const projectsData = {
  kpis: [
    {
      id: 'active',
      title: 'المشاريع النشطة',
      value: '24',
      change: '2 مشاريع جديدة',
      changeType: 'up',
      period: 'هذا الربع',
      icon: 'projects',
    },
    {
      id: 'ontime',
      title: 'نسبة الإنجاز',
      value: '73.5%',
      change: '4.2%',
      changeType: 'up',
      period: 'مقارنة بالشهر الماضي',
      icon: 'progress',
    },
    {
      id: 'budget',
      title: 'الميزانية المستخدمة',
      value: '68.2%',
      change: '2.1%',
      changeType: 'down',
      period: 'ضمن الخطة',
      icon: 'budget',
    },
    {
      id: 'ontime2',
      title: 'المشاريع في الموعد',
      value: '18',
      change: '75%',
      changeType: 'up',
      period: 'من إجمالي المشاريع',
      icon: 'clock',
    },
  ],

  projectList: [
    { name: 'برج الأعمال - الرياض', progress: 82, budget: 1200000, used: 960000, status: 'على الموعد', deadline: '2026-09-15', team: 24 },
    { name: 'مجمع سكني - جدة', progress: 67, budget: 2400000, used: 1580000, status: 'على الموعد', deadline: '2026-11-20', team: 38 },
    { name: 'مستشفى الملك - الدمام', progress: 45, budget: 3600000, used: 1620000, status: 'متأخر', deadline: '2027-01-10', team: 52 },
    { name: 'تطوير طرق - أمانة الرياض', progress: 90, budget: 1800000, used: 1530000, status: 'على الموعد', deadline: '2026-08-01', team: 31 },
    { name: 'جسور - مؤسسة البناء', progress: 38, budget: 950000, used: 410000, status: 'على الموعد', deadline: '2026-12-05', team: 19 },
    { name: 'بنية تحتية - مدينة المعرفة', progress: 100, budget: 2100000, used: 2080000, status: 'مكتمل', deadline: '2026-06-28', team: 44 },
  ],

  statusDistribution: [
    { name: 'على الموعد', value: 18 },
    { name: 'متأخر', value: 3 },
    { name: 'مكتمل', value: 3 },
  ],

  teamPerformance: [
    { team: 'الرياض', tasks: 420, completed: 398, efficiency: 94.8 },
    { team: 'جدة', tasks: 380, completed: 351, efficiency: 92.4 },
    { team: 'الدمام', tasks: 310, completed: 268, efficiency: 86.5 },
    { team: 'مكة', tasks: 240, completed: 226, efficiency: 94.2 },
  ],

  recentProjects: [
    { id: 'PRJ-101', name: 'برج الأعمال', client: 'مجموعة الراجحي', progress: 82, status: 'على الموعد', manager: 'أحمد العتيبي' },
    { id: 'PRJ-102', name: 'مجمع سكني', client: 'شركة الأفق', progress: 67, status: 'على الموعد', manager: 'سارة المالكي' },
    { id: 'PRJ-103', name: 'مستشفى الملك', client: 'وزارة الصحة', progress: 45, status: 'متأخر', manager: 'خالد الزهراني' },
    { id: 'PRJ-104', name: 'تطوير طرق', client: 'أمانة الرياض', progress: 90, status: 'على الموعد', manager: 'منى القحطاني' },
    { id: 'PRJ-105', name: 'جسور', client: 'مؤسسة البناء', progress: 38, status: 'على الموعد', manager: 'عبدالله الشمري' },
  ],

  drillDownDetails: [
    { id: 'TASK-001', project: 'برج الأعمال', task: 'التشطيبات', progress: 85, status: 'قيد التنفيذ', assignee: 'فريق أ' },
    { id: 'TASK-002', project: 'برج الأعمال', task: 'الواجهات', progress: 72, status: 'قيد التنفيذ', assignee: 'فريق ب' },
    { id: 'TASK-003', project: 'مجمع سكني', task: 'الأساسات', progress: 100, status: 'مكتمل', assignee: 'فريق ج' },
    { id: 'TASK-004', project: 'مجمع سكني', task: 'الهيكل', progress: 64, status: 'قيد التنفيذ', assignee: 'فريق د' },
    { id: 'TASK-005', project: 'مستشفى الملك', task: 'التجهيزات', progress: 40, status: 'متأخر', assignee: 'فريق هـ' },
    { id: 'TASK-006', project: 'تطوير طرق', task: 'الرصف', progress: 92, status: 'قيد التنفيذ', assignee: 'فريق و' },
  ],
};

// Chart color palettes
export const chartColors = {
  bronze: '#CD7F32',
  orange: '#D4876A',
  olive: '#6B8E4E',
  gold: '#B8860B',
  mocha: '#8B7355',
  burgundy: '#800020',
  sage: '#7A8B6E',
  terracotta: '#C4716E',
};

export const chartPalette = ['#CD7F32', '#8B7355', '#6B8E4E', '#B8860B', '#D4876A', '#7A8B6E'];
