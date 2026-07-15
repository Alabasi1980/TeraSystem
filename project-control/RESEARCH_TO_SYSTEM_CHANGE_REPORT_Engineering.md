# RESEARCH_TO_SYSTEM_CHANGE_REPORT

## إنشاء EngineeringAgent — بحث عميق في مبادئ هندسة البرمجيات للمهندس المنفّذ المحترف

---

### Research Topic
كيف نبني عميلاً مهندساً منفّذاً (EngineeringAgent) يمتلك:
- **مهارات هندسية عميقة** في بناء تطبيقات متكاملة
- **قدرة على ربط التصميم بالكود الخلفي** (API Integration)
- **معرفة بأنماط العمارة النظيفة** و SOLID
- **فهم عميق** لإدارة الحالة، قواعد البيانات، الأمان
- **التزام بجودة الكود** والاختبارات والتوثيق

### Sources Reviewed
1. **The Twelve-Factor App** — Adam Wiggins / Heroku
2. **Clean Code: JavaScript** — Ryan McDermott (Robert C. Martin principles)
3. **SOLID Principles** — Robert C. Martin
4. **Design Patterns** — Gang of Four
5. **Clean Architecture** — Robert C. Martin
6. **12 Factor App Methodology** — cloud-native best practices

---

## Relevant Findings

### 1. The Twelve-Factor App Methodology

| العامل | المبدأ | تطبيقه في المهندس المنفّذ |
|--------|--------|--------------------------|
| **I. Codebase** | One codebase tracked in revision control, many deploys | نظام تحكم بالإصدارات (git)، فرع واحد للمصدر، نشر متعدد |
| **II. Dependencies** | Explicitly declare and isolate dependencies | package.json, requirements.txt — لا اعتماد على تبعيات ضمنية |
| **III. Config** | Store config in the environment | متغيرات البيئة (env)، لا Config في الكود |
| **IV. Backing services** | Treat backing services as attached resources | قاعدة البيانات، API، Cache — كلها موارد قابلة للتبديل |
| **V. Build, release, run** | Strictly separate build and run stages | CI/CD pipeline: Build → Release → Run |
| **VI. Processes** | Execute the app as stateless processes | بدون حالة (stateless) — التوسع أفقي |
| **VII. Port binding** | Export services via port binding | الخدمة تexport نفسها على port |
| **VIII. Concurrency** | Scale out via the process model | تعدد العمليات بدلاً من الخيوط |
| **IX. Disposability** | Maximize robustness | بداية سريعة + إغلاق graceful |
| **X. Dev/prod parity** | Keep dev, staging, prod similar | نفس البيئة في كل مرحلة |
| **XI. Logs** | Treat logs as event streams | logs تذهب إلى stdout/stderr |
| **XII. Admin processes** | Run admin tasks as one-off | مهام منفصلة للإدارة |

### 2. مبادئ Clean Code — متغيرات

| المبدأ | التطبيق |
|--------|---------|
| أسماء ذات معنى | `getUser()` بدلاً من `getClientData()` أو `getCustomerRecord()` |
| أسماء قابلة للبحث | `MILLISECONDS_PER_DAY` بدلاً من `86400000` |
| Explicit over implicit | `locations.forEach(location =>` بدلاً من `l =>` |
| لا تكرار للسياق | `{ make, model }` بدلاً من `{ carMake, carModel }` |

### 3. مبادئ Clean Code — دوال (Functions)

| المبدأ | التطبيق |
|--------|---------|
| **Do one thing** | كل دالة تفعل شيئاً واحداً فقط |
| **2 arguments or fewer** | استخدم destructuring object بدلاً من arguments كثيرة |
| **No side effects** | لا تغير متغيرات خارجية |
| **Remove duplication** | DRY — لا تكرر نفسك |
| **One level of abstraction** | دالة واحدة = مستوى تجريد واحد |
| **Favor functional over imperative** | `.map()`, `.filter()`, `.reduce()` بدلاً من loops |
| **Encapsulate conditionals** | `if (isVisible())` بدلاً من `if (x === true && y === 'ok')` |
| **Avoid negative conditionals** | `if (isPresent)` بدلاً من `if (!isNotPresent)` |

### 4. مبادئ SOLID

| المبدأ | المعنى | التطبيق |
|--------|--------|---------|
| **S** — Single Responsibility | كل كلاس له مسؤولية واحدة | Service Layer ≠ Controller Layer |
| **O** — Open/Closed | مفتوح للتمديد، مغلق للتعديل | استخدم extends, implements |
| **L** — Liskov Substitution | الفئات الفرعية تحل محل الأم | بدون كسر للسلوك |
| **I** — Interface Segregation | واجهات صغيرة متخصصة | لا تجبر على تنفيذ ما لا يحتاج |
| **D** — Dependency Inversion | اعتمد على التجريد لا التنفيذ | Inject dependencies, لا تنشئها |

### 5. مسؤوليات المهندس المنفّذ

#### 5.1 API Integration
- يقرأ Tech Spec ← يفهم كل API endpoint
- يكتب API calls (fetch, axios, HttpClient)
- يعالج success/error/loading لكل طلب
- يعيّن headers (Authorization, Content-Type)
- يتعامل مع Pagination, Retry, Timeout

#### 5.2 State Management
- يختار الأداة المناسبة: Context API / Redux / Zustand / NgRx
- يصمم Store structure
- يربط API calls بالـ State
- يتعامل مع Optimistic Updates
- يدير Cache و Stale Data

#### 5.3 Business Logic
- يطبق Business Rules من Tech Spec
- يكتب Validation منطق
- يتعامل مع Formatting (تواريخ، أرقام، عملات)
- يكتب User Permissions logic
- يطبق Authorization (من يمكنه فعل ماذا)

#### 5.4 Database Operations
- يختار ORM المناسب: EF Core, Prisma, Sequelize, Django ORM
- يكتب Queries (SQL, LINQ, Query Builder)
- يتعامل مع Migrations
- يحسّن Performance (Indexes, Lazy/Eager Loading)

#### 5.5 Error Handling
- يعالج كل حالة: Loading, Empty, Error, Success
- يكتب Error Boundaries
- يعرض رسائل خطأ مفهومة
- يسجل الأخطاء في Logs

#### 5.6 Security
- Authentication (JWT, OAuth, Session)
- Authorization (Roles, Permissions)
- Input Validation (Sanitization)
- SQL Injection prevention
- XSS/CSRF protection

### 6. سير العمل الكامل مع الـ EngineeringAgent

```
1. Software Designer ← TECHNICAL_SPECIFICATION.md
   (API endpoints, Data models, Business rules, Component tree)

2. UI Designer ← UI Code (React + Tailwind — طبقة بصرية فقط)
   (شكل الصفحة، الألوان، الخطوط، التخطيط)

3. EngineeringAgent:
   ← يقرأ Tech Spec (يفهم API، البيانات، القواعد)
   ← يقرأ UI Code (يفهم المكونات التي تحتاج ربط)
   ← يربط API بالـ UI (fetch, axios, State)
   ← يكتب Business Logic (validation, formatting, rules)
   ← يكتب Controllers/Services (إذا Backend)
   ← يكتب Database Queries (إذا Backend)
   ← يكتب Error/Loading/Empty حالات (يربطها بـ UI)
   ← يكتب Authentication/Authorization
   ← يربط كل شيء = تطبيق حي شغّال

4. TeraAgent ← يوافق أو يطلب تعديل

5. DesignReviewer ← يراجع المخرجات النهائية
```

### 7. متى يعمل مع UI Designer ومتى يعمل بمفرده

| السيناريو | من يعمل |
|-----------|---------|
| **UI Designer + EngineeringAgent** | Full Stack (React + API) |
| **EngineeringAgent فقط** | Backend API (.NET, Django, Node) |
| **UI Designer فقط** | Prototype بصري فقط (بدون Backend) |

---

## What Should NOT Be Adopted

1. **التعقيد المفرط** — microservices في مشروع صغير
2. **عدم استخدام ORM** — كتابة SQL يدوي طويل
3. **عدم التعامل مع الأخطاء** — try/catch فارغ
4. **Hardcoding** — كتابة قيم ثابتة في الكود
5. **تجاهل الأداء** — N+1 queries، عدم استخدام Pagination

---

## Anti-Bloat Check

| السؤال | الإجابة |
|--------|---------|
| ما المشكلة التي تحلها؟ | لا يوجد من يربط التصميم بالكود الخلفي |
| لماذا لا يكفي عميل موجود؟ | Software Designer يخطط فقط، UI Designer يجمّل فقط — لا أحد يشغّل |
| هل تزيد التعقيد أم تقلله؟ | تزيده تنظيمياً لكن تقلله تشغيلياً |
| هل توجد طريقة أصغر؟ | لا — هذه فجوة حقيقية، سدها يتطلب عميلاً جديداً |

---

**أعدّه:** حارس (TeraSystemEvolutionAgent)  
**التاريخ:** 2026-07-07
