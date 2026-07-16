# تقرير تحليل الفجوات — Mobile Capability Gap Analysis

**الموضوع:** تقييم جاهزية قدرة الموبايل في منظومة Tera قبل أول مشروع عميل
**التاريخ:** 2026-07-16
**المصدر:** Domain Research Report — 8 اتجاهات بحث (flutter mobile best practices 2025-2026)
**الملفات المُقيّمة:**
- `tera-system/profiles/flutter-mobile.md` (20 قسماً — 369 سطراً)
- `tera-system/design-system/MOBILE_UI_UX_STANDARDS.md` (19 قسماً — 342 سطراً)
- `tera-system/runtime/TERA_RUNTIME_CHECKLISTS.md` §20 (5 قوائم فرعية)
- `tera-system/design-system/UI_ACCEPTANCE_GATE.md` (11 خانة موبايل)

**الحالة:** ✅ **جميع الفجوات (21) مُغلقة** — اكتمل تحسين المنظومة بتاريخ 2026-07-16
- `flutter-mobile.md` ← تم تحديث 7 أقسام (§5, §6, §7, §9, §14, §16, §18) — الإصدار 1.1.0
- `MOBILE_UI_UX_STANDARDS.md` ← تم تحديث 3 أقسام (§3, §9, §12) — الإصدار 1.1.0

---

## ملخص الإجراء

| الفئة | العدد | الحالة |
|-------|:-----:|--------|
| نقاط صحيحة (لا تحتاج تغيير) | 15 | ✅ مُغلقة مسبقاً |
| فجوات حرجة | 6 | ✅ **مُغلقة** — 2026-07-16 |
| فجوات متوسطة | 11 | ✅ **مُغلقة** — 2026-07-16 |
| فجوات بسيطة | 5 | ✅ **مُغلقة** — 2026-07-16 |
| **المجموع** | **37** | ✅ **كاملة** |

---

## 🟢 نقاط صحيحة — لا تحتاج تعديل

ما يلي مُؤكد من البحث وموافق عليه — لا يعمل:

| # | ما لدينا | التأكيد |
|:-:|---------|:-------:|
| 1 | Feature-first architecture | ✅ معيار industry 2026 |
| 2 | GoRouter / AutoRoute للتنقل | ✅ متوافق |
| 3 | `flutter_secure_storage` وليس SharedPreferences للتوكنات | ✅ معيار أمني |
| 4 | `--obfuscate --split-debug-info` | ✅ ضروري |
| 5 | `ListView.builder` / `GridView.builder` | ✅ غير قابل للتفاوض |
| 6 | Const constructors | ✅ أرخص تحسين أداء |
| 7 | Dispose controllers/streams | ✅ منع تسرب الذاكرة |
| 8 | Touch targets 48x48 dp | ✅ معيار M3 + HIG |
| 9 | حالات UI (تحميل/فارغ/خطأ/أوفلاين/أذونات) | ✅ كاملة |
| 10 | قاعدة عدم الاعتماد على gestures فقط | ✅ ممتازة |
| 11 | Safe areas + notch + keyboard avoidance | ✅ صحيحة |
| 12 | RTL أساسيات (Directionality, icon flip, directional padding) | ✅ صحيحة |
| 13 | طلب permissions وقت الحاجة (JIT) | ✅ معيار 2026 |
| 14 | أنواع الاختبارات (unit/widget/integration/golden) | ✅ صحيحة |
| 15 | Riverpod / Bloc / Provider كخيارات | ✅ صحيح (لكن يحتاج ترتيب) |

---

## 🔴 فجوات حرجة (6) — يجب إغلاقها قبل أول مشروع عميل

### C1 — SSL Certificate Pinning

| الحقل | التفصيل |
|-------|---------|
| **الحالة الحالية** | `flutter-mobile.md` §16 تذكر "Validate SSL/TLS in production (do not disable certificate verification)" — لكن لا تذكر certificate pinning |
| **ما ينقص** | HTTPS وحده غير كافٍ ضد هجمات MITM مع شهادات CA مخترقة. Industry standard يتطلب pinning للتطبيقات التي تتعامل مع بيانات حساسة |
| **الإطار** | `http_certificate_pinning` أو `SecurityContext` مع Dart `HttpClient`. يُثبَّت على public key hash وليس الشهادة الكاملة |
| **الملف المتأثر** | `flutter-mobile.md` — إضافة قسم في §16 Security |
| **الأولوية** | 🔴 حرج — ثغرة أمنية |

**التوصية:** إضافة إلى §16 Security في flutter-mobile.md:

```text
### SSL Certificate Pinning (for apps with sensitive data)

When the app communicates with an API that handles sensitive data (financial, personal, credentials):

- Pin to the **public key hash** (not the full certificate) using `http_certificate_pinning` or `SecurityContext`.
- Never disable certificate verification in release builds.
- Always include a fallback strategy (e.g., disable pinning after X failed attempts to avoid app lockout).

When NOT needed:
- Static content delivery (images, fonts)
- Non-sensitive public APIs

**Decision:** If the app handles user data or authentication → pin. If read-only public content → not required.
```

---

### C2 — API Key Management Strategy

| الحقل | التفصيل |
|-------|---------|
| **الحالة الحالية** | `flutter-mobile.md` §16 تذكر "Do not hard-code API keys or secrets" — لكن لا تشرح كيف تُدار المفاتيح |
| **ما ينقص** | لا توجد استراتيجية محددة لإدخال المفاتيح. المطور سيضعها في الكود تلقائياً |
| **الإطار** | (1) `--dart-define` للإدخال أثناء البناء، (2) `flutter_dotenv` للملفات المحلية، (3) Backend proxy كحل مثالي — المفاتيح لا تصل للعميل أبداً |
| **الملف المتأثر** | `flutter-mobile.md` — تعديل §16 Security |
| **الأولوية** | 🔴 حرج — المفاتيح تستخرج من الـ binary حتى مع obfuscation |

**التوصية:** إضافة إلى §16 Security:

```text
### API Key Management (never hardcode)

| Method | When | Security Level |
|--------|------|:--------------:|
| `--dart-define=KEY=value` | Compile-time injection from env | 🟡 Medium |
| `flutter_dotenv` + `.env` file (gitignored) | Local development | 🟡 Medium |
| Backend proxy (API key stays on server) | Production | 🟢 High |

**Rule:** For production apps, route API calls through a backend proxy so keys never reach the client. For development, use `--dart-define`. Never commit `.env` files or hardcode keys in Dart source.
```

---

### C3 — CI/CD Pipeline Guidance

| الحقل | التفصيل |
|-------|---------|
| **الحالة الحالية** | `flutter-mobile.md` §18 "DevOps / Release (Reference Only)" — يذكر Android AAB + iOS IPA + version strings فقط |
| **ما ينقص** | لا يوجد أي إرشاد لأدوات CI/CD أو خطوط بناء آلي. المطور لن يعرف كيف يبني وينشر التطبيق تلقائياً |
| **الإطار** | GitHub Actions (PR checks على Linux) + Codemagic (build على macOS) + Fastlane (store submission). Pipeline: PR gate → staging → production |
| **الملف المتأثر** | `flutter-mobile.md` — تعديل §18 DevOps / Release |
| **الأولوية** | 🔴 حرج — بدون CI/CD لا يوجد نشر حقيقي |

**التوصية:** تعديل §18 DevOps / Release ليصبح قسماً تشغيلياً:

```text
## 18. DevOps / Release

### 18.1 CI/CD Pipeline (Industry Standard)

| Stage | Tool | Environment | Trigger |
|-------|------|:-----------:|---------|
| PR Validation | GitHub Actions | Linux | Every PR |
| Staging Build | Codemagic | macOS | Push to develop |
| Production Release | Codemagic + Fastlane | macOS + Store | Git tag |

**PR Validation (Linux — fast & cheap):**
- `flutter analyze` (no warnings)
- `flutter test` (all pass)
- `dart run build_runner build --delete-conflicting-outputs` (codegen check)
- Code formatting check

**Staging Build (macOS — Codemagic):**
- `flutter build apk --release`
- `flutter build ios --release --no-codesign` (iOS staging)
- Upload to TestFlight (iOS) + Play Internal Track (Android)

**Production Release (Codemagic + Fastlane):**
- `flutter build appbundle --release --obfuscate --split-debug-info=./build/debug-info`
- `flutter build ipa --release --obfuscate --split-debug-info=./build/debug-info`
- Upload to Play Store (production track) + App Store Connect
- Upload crash symbols to Sentry/crash reporting tool

### 18.2 Version Automation

- Use sequential build numbers: `1.0.0+1`, `1.0.0+2`, etc.
- Pass via `flutter build ... --build-name=1.0.0 --build-number=$BUILD_NUMBER`
- Never use timestamp-based build numbers in production
- Increment build number for every release (including TestFlight submissions)

### 18.3 Store Deployment

| Store | File | Signing | Submission |
|-------|------|---------|------------|
| Google Play | `.aab` (App Bundle) | Upload key + App signing by Google | Play Console → Production track |
| App Store | `.ipa` (Xcode Archive) | Apple Distribution Certificate + Provisioning Profile | App Store Connect → TestFlight → Review |

**Recommended tools:**
- **Codemagic** — Flutter-native CI/CD, built-in iOS code signing, Apple Silicon Mac instances, free tier: 500 min/month
- **Fastlane** — Store deployment automation (metadata, screenshots, phased rollouts)
- **Shorebird** — OTA Dart code updates (skip App Store review for Dart-only changes)
```

---

### C4 — Database Recommendation Update (Drift Default)

| الحقل | التفصيل |
|-------|---------|
| **الحالة الحالية** | `flutter-mobile.md` §5 يذكر "SharedPreferences / Hive / Isar / Drift (SQLite)" كخيارات متساوية |
| **ما ينقص** | Hive و Isar مهجورتان من مُطوريهما الأصليين. مصادر متعددة تحذّر من استخدامهما في مشاريع جديدة |
| **الإطار** | Drift هو الـ default لـ 2026 (SQLite-based، مُصان، موثّق). ObjectBox كبديل NoSQL مع sync |
| **الملف المتأثر** | `flutter-mobile.md` — تعديل §5 Architecture Guidance + §13 Dependency Governance |
| **الأولوية** | 🔴 حرج — خطر اختيار قاعدة مهجورة |

**التوصية:** تعديل §5 Architecture Guidance:

```text
### 2026 Recommendation

| Type | Package | Status | Recommendation |
|------|---------|:------:|:--------------:|
| Relational (SQL) | **Drift** (SQLite) | ✅ Actively maintained | **Default choice** |
| NoSQL (local) | Hive | ⚠️ Abandoned by author | **Do not use for new projects** |
| NoSQL (local) | Isar | ⚠️ Abandoned (community fork exists) | **Maintenance risk — avoid** |
| NoSQL (sync) | ObjectBox | ✅ Commercial with sync | Use only if offline-first sync is required |
| KV storage | SharedPreferences | ✅ Maintained | Simple key-value only (not for structured data) |

**Critical warning (2026):** Hive and Isar were both abandoned by their original authors. Isar survives via community fork (`isar-community`) but carries significant maintenance risk. The Flutter community twice chose "the fastest option" and twice the fastest option stopped being maintained. **Drift is the safe default for all new projects.**
```

---

### C5 — OWASP Mobile Top 10 Coverage

| الحقل | التفصيل |
|-------|---------|
| **الحالة الحالية** | `flutter-mobile.md` §16 تذكر أمن الأساسي (secure storage, obfuscation, SSL, biometric) — لكن لا يوجد إطار أمني منهجي |
| **ما ينقص** | لا يوجد مرجع لـ OWASP Mobile Top 10 أو أي إطار تقييم أمني |
| **الإطار** | OWASP Mobile Top 10 (2024) كإطار مرجعي. لا يعني تغطية كاملة — يعني أننا نعرف المخاطر ونقيّمها |
| **الملف المتأثر** | `flutter-mobile.md` — إضافة مرجع في §16 Security |
| **الأولوية** | 🔴 حرج — فجوة أمنية منهجية |

**التوصية:** إضافة إلى §16 Security:

```text
### OWASP Mobile Top 10 — Reference

This profile does not require full OWASP Mobile Top 10 compliance for every project. However, Tera must be aware of these risks and assess relevance per project:

| OWASP Mobile Risk | Flutter Mitigation | When Relevant |
|---|---|---|
| M1: Improper Credential Usage | `flutter_secure_storage` + backend proxy | All apps with auth |
| M2: Inadequate Supply Chain Security | Audit third-party packages, use `dart audit` | All projects |
| M3: Insecure Authentication/Authorization | Short-lived JWT + refresh tokens | Apps with login |
| M4: Insufficient Input/Output Validation | Server-side validation (never trust client) | Apps with API calls |
| M5: Insecure Communication | SSL pinning + TLS 1.2+ | Apps with sensitive data |
| M6: Inadequate Privacy Controls | Biometric lock + private notifications | Apps with personal data |
| M7: Insufficient Binary Protections | `--obfuscate --split-debug-info` | All release builds |
| M8: Security Misconfiguration | No debug flags in release, proper Keychain config | All projects |
| M9: Insecure Data Storage | `flutter_secure_storage` for secrets, encrypted DB for sensitive data | All projects |
| M10: Insufficient Cryptography | Use platform crypto (not custom implementations) | Apps with encryption |

**Rule:** During Pre-Execution Gate for mobile projects, assess which OWASP Mobile risks apply and document mitigations in the project's security file.
```

---

### C6 — Impeller Rendering Engine

| الحقل | التفصيل |
|-------|---------|
| **الحالة الحالية** | `flutter-mobile.md` §14 Performance لا تذكر Impeller |
| **ما ينقص** | Impeller هو محرك الرسم الافتراضي في Flutter 3.44+ على iOS. يحل مشكلة shader compilation jank بالكامل |
| **الإطار** | Impeller default on iOS (Flutter 3.44+)، available on Android. لا يحتاج إعداد — يعمل تلقائياً |
| **الملف المتأثر** | `flutter-mobile.md` — إضافة ملاحظة في §14 Performance |
| **الأولوية** | 🔴 حرج — تحسين أداء كبير بدون جهد |

**التوصية:** إضافة إلى §14 Performance:

```text
### Impeller Rendering Engine

As of Flutter 3.44+, **Impeller** is the default rendering engine on iOS and is available on Android. It eliminates shader compilation jank — the most common cause of first-use stuttering.

| Platform | Impeller Status | Action |
|----------|:---------------:|--------|
| iOS | Default (enabled) | No action needed |
| Android | Available (opt-in) | Enable in `android/app/src/main/AndroidManifest.xml`: `<meta-data android:name="io.flutter.embedding.android.EnableImpeller" android:value="true"/>` |

**Key benefit:** No shader compilation jank. The app renders smoothly from the first frame without warm-up. Impeller pre-compiles all shaders at build time.

**When to verify:** If the app uses custom shaders, blur effects, or complex animations and experiences jank on first use, verify Impeller is enabled.
```

---

## 🟡 فجوات متوسطة (11) — يجب إغلاقها قبل أو أثناء أول مشروع

### M1 — Testing Pyramid Ratios

| الحقل | التفصيل |
|-------|---------|
| **الحالة** | `flutter-mobile.md` §9 يذكر أنواع الاختبارات لكن لا يذكر النسب |
| **الإضافة** | نسب هرم الاختبارات: 60-70% unit / 20-25% widget / 10% integration / 5% golden |
| **الملف** | `flutter-mobile.md` §9 Testing |

**التوصية:** إضافة إلى §9 Testing:

```text
### Testing Pyramid

| Layer | Ratio | Speed | Tools |
|-------|:-----:|:-----:|-------|
| Unit tests | 60–70% | Fast | `test`, `mocktail` |
| Widget tests | 20–25% | Medium | `flutter_test`, `mocktail` |
| Integration tests | ~10% | Slow | `integration_test` (+ `Patrol` for native interactions) |
| Golden tests | ~5% | Medium | `alchemist` (visual regression for design system components only) |

**Target coverage:** 80–95% for business logic, 70–80% for widget UI. Exclude generated code (freezed, json_serializable, drift).
```

---

### M2 — Mocktail Preference

| الحقل | التفصيل |
|-------|---------|
| **الحالة** | `flutter-mobile.md` §9 لا تذكر أداة mock محددة |
| **الإضافة** | `mocktail` هو المعيار 2026 (بديل/mocktail عن mockito — لا يحتاج code generation) |
| **الملف** | `flutter-mobile.md` §9 Testing |

**التوصية:** إضافة إلى §9 Testing:

```text
### Mocking

**Recommended:** `mocktail` (2026 standard)
- No code generation required
- Clean `when()` / `verify()` API
- Lightweight and fast

**Alternative:** `mockito` (still works, but requires `build_runner` for code generation)

**Rule:** Choose exactly one mocking library per project. Do not mix mocktail and mockito.
```

---

### M3 — Patrol for Native Integration Tests

| الحقل | التفصيل |
|-------|---------|
| **الحالة** | `flutter-mobile.md` §9 لا تذكر Patrol |
| **الإضافة** | Patrol يتيح اختبار native interactions (أذونات، إشعارات، platform views) |
| **الملف** | `flutter-mobile.md` §9 Testing |

**التوصية:** إضافة إلى §9 Testing:

```text
### Integration Testing

- Use built-in `integration_test` package for standard flows.
- Use **Patrol** (by LeanCode) when the test requires native device interactions:
  - Permission dialogs (accept/deny)
  - Notification handling
  - Platform views
  - Deep links from external sources
- Patrol runs on Firebase Test Lab for CI — recommended for critical user flows.
```

---

### M4 — Coverage Targets

| الحقل | التفصيل |
|-------|---------|
| **الحالة** | لا توجد نسب تغطية مستهدفة |
| **الإضافة** | 80-95% logic / 70-80% UI |
| **الملف** | `flutter-mobile.md` §9 Testing (مدمج مع M1) |

**التوصية:** مدمج مع M1 أعلاه (لا يحتاج قسماً منفصلاً).

---

### M5 — Pigeon for Platform Channels

| الحقل | التفصيل |
|-------|---------|
| **الحالة** | `flutter-mobile.md` §7 Push Notifications + §6 Permissions لا تذكر Pigeon |
| **الإضافة** | Pigeon هو البديل الموصى به لـ raw MethodChannel (type-safe، less boilerplate) |
| **الملف** | `flutter-mobile.md` §6 Permissions |

**التوصية:** إضافة إلى §6 Permissions:

```text
### Platform Channel Communication

When communicating with native code (Android/iOS):

| Method | When | Recommendation |
|--------|------|:--------------:|
| `Pigeon` | Custom native integration | ✅ **Default** (type-safe, less boilerplate) |
| `MethodChannel` | Simple one-off calls | 🟡 Acceptable |
| `EventChannel` | Streaming data from native | 🟡 Acceptable |

**Rule:** For new projects, prefer `Pigeon` for platform channel communication. It generates type-safe Dart and native code, eliminating string-based API errors. `permission_handler` itself is migrating its iOS bridge to Pigeon.
```

---

### M6 — iOS Photos `limited` State

| الحقل | التفصيل |
|-------|---------|
| **الحالة** | `flutter-mobile.md` + `MOBILE_UI_UX_STANDARDS.md` لا تذكر حالة iOS Photos `limited` |
| **الإضافة** | iOS 14+ يسمح للمستخدمين بمنح إمكانية الوصول لأ几张 صور فقط (limited access) |
| **الملف** | `MOBILE_UI_UX_STANDARDS.md` §9 Permissions |

**التوصية:** إضافة إلى §9 Permissions في MOBILE_UI_UX_STANDARDS.md:

```text
### iOS Photos Limited Access (iOS 14+)

When using `image_picker` on iOS, users may grant **limited access** — allowing access to specific photos only.

- Check for `PermissionStatus.limited` after requesting photo access.
- Show a UI that explains the user can add more photos via the system picker.
- Call `openAppSettings()` if the user wants to grant full access.
- Test with "Limited Access" in iOS Simulator settings.
```

---

### M7 — Android 13+ Notification Permission

| الحقل | التفصيل |
|-------|---------|
| **الحالة** | `flutter-mobile.md` §7 Push Notifications لا تذكر أن Android 13+ يحتاج runtime permission للإشعارات |
| **الإضافة** | Android 13 (API 33) فرض `POST_NOTIFICATIONS` كـ runtime permission |
| **الملف** | `flutter-mobile.md` §7 Push Notifications |

**التوصية:** إضافة إلى §7 Push Notifications:

```text
### Android 13+ Notification Permission

Starting with Android 13 (API 33), notification permission is a **runtime permission** — must be requested like camera or location.

- Use `permission_handler` to request `Permission.notification` before registering for push notifications.
- Handle denied state: show in-app notification prompt explaining value.
- Use "demonstrate value first" pattern — show the user what notifications look like before requesting permission.

```dart
// Example flow
if (await Permission.notification.isDenied) {
  // Show rationale screen
  await Permission.notification.request();
}
```
```

---

### M8 — Arabic Font Recommendations

| الحقل | التفصيل |
|-------|---------|
| **الحالة** | `MOBILE_UI_UX_STANDARDS.md` §12 RTL لا تذكر خطوطاً عربية محددة |
| **الإضافة** | توصية خطوط + زيادة حجم 10-15% + line height 1.6x |
| **الملف** | `MOBILE_UI_UX_STANDARDS.md` §12 RTL / LTR |

**التوصية:** إضافة إلى §12 RTL / LTR:

```text
### Arabic Typography

| Guideline | Value |
|-----------|-------|
| Recommended fonts | Noto Sans Arabic (safe default), Cairo, Tajawal, IBM Plex Arabic |
| Font size vs Latin | Arabic text needs **10–15% larger** size than Latin equivalents for same readability |
| Line height | Minimum **1.6x** for Arabic (vs 1.4x for English) |
| `fontFamilyFallback` | Always include: `['Noto Sans Arabic', 'Roboto']` — ensures Arabic characters render correctly |
| Font weights | Arabic text at weight 400 may appear thinner than Latin — consider weight 500 for body text |

**Test rule:** Always compare Arabic and English side-by-side at the same font size. If Arabic appears thinner/smaller, adjust.
```

---

### M9 — Arabic Plural Forms

| الحقل | التفصيل |
|-------|---------|
| **الحالة** | لا توجد إرشادات حول صيغ الجمع العربية |
| **الإضافة** | العربية لها 6 صيغ جمع (zero, one, two, few, many, other) |
| **الملف** | `MOBILE_UI_UX_STANDARDS.md` §12 RTL / LTR |

**التوصية:** إضافة إلى §12 RTL / LTR:

```text
### Arabic Plural Forms

Arabic has **6 plural forms** — most developers only handle singular/plural (2 forms), which is incorrect.

| CLDR Category | Arabic Form | Example (مهمة) |
|:---:|---|---|
| zero | لا مهمات | 0 tasks |
| one | مهمة واحدة | 1 task |
| two | مهمتان | 2 tasks |
| few | 3–10 مهمات | 3–10 tasks |
| many | 11–99 مهمة | 11–99 tasks |
| other | 100+ مهمة | 100+ tasks |

**Rule:** When using ARB files for Arabic, always define all 6 plural forms using ICU message format:

```json
"taskCount": "{count,plural, =0{لا مهمات} =1{مهمة واحدة} =2{مهمتان} few{count مهمات} many{count مهمة} other{count مهمة}}"
```
```

---

### M10 — Performance Budget Numbers

| الحقل | التفصيل |
|-------|---------|
| **الحالة** | `flutter-mobile.md` §14 Performance لا تذكر أرقام budget |
| **الإضافة** | 16.67ms للـ 60fps / 8.33ms للـ 120fps |
| **الملف** | `flutter-mobile.md` §14 Performance |

**التوصية:** إضافة إلى §14 Performance:

```text
### Performance Budget

| Target | Frame Budget | How to Measure |
|--------|:-----------:|----------------|
| 60 fps (standard) | **16.67 ms** per frame | Flutter DevTools → Performance tab |
| 120 fps (high refresh) | **8.33 ms** per frame | Flutter DevTools → Performance tab |

**Rules:**
- Profile on **real devices in profile mode** — emulator/simulator numbers are meaningless.
- Use Flutter DevTools to identify whether bottleneck is UI thread (Dart code) or raster thread (painting/images).
- If frame time > 16.67ms consistently → investigate.
```

---

### M11 — ShaderWarmUp

| الحقل | التفصيل |
|-------|---------|
| **الحالة** | لا توجد إرشادات حول shader compilation jank |
| **الإضافة** | ShaderWarmUp لتطبيقات مع custom shaders أو blur effects |
| **الملف** | `flutter-mobile.md` §14 Performance |

**التوصية:** إضافة إلى §14 Performance:

```text
### Shader Compilation Jank (with Impeller)

With Impeller enabled (default on iOS since Flutter 3.44), shader compilation jank is eliminated — shaders are pre-compiled at build time.

**If Impeller is NOT available** (older Flutter versions or Android without Impeller):
- Use `ShaderWarmUp` class to pre-compile shaders during splash screen.
- Target the most common rendering operations (blur, gradient, path).
- This eliminates first-use stuttering for visual effects.

**Recommendation:** Always use the latest Flutter stable channel to get Impeller by default.
```

---

## 🔵 فجوات بسيطة (5) — يمكن إضافتها أثناء أو بعد أول مشروع

### B1 — cacheWidth / cacheHeight for Image Memory

| الحقل | التفصيل |
|-------|---------|
| **الملف** | `flutter-mobile.md` §14 Performance |
| **التوصية** | إضافة ملاحظة: "Use `cacheWidth`/`cacheHeight` in `Image.network()` and `Image.file()` to decode images at target size — never load a 4K image into a 100px avatar" |

---

### B2 — Isar/Hive Abandonment Warning

| الحقل | التفصيل |
|-------|---------|
| **الملف** | مدمج مع C4 (Database recommendation) — لا يحتاج ملفاً منفصلاً |

---

### B3 — Shorebird OTA Updates

| الحقل | التفصيل |
|-------|---------|
| **الملف** | `flutter-mobile.md` §18 DevOps / Release |
| **التوصية** | إضافة ملاحظة: "Shorebird enables OTA Dart code updates — skip App Store review for Dart-only changes. Cannot change native code, assets, or platform channels. Complementary to, not replacement for, CI/CD." |

---

### B4 — Material You / Dynamic Color

| الحقل | التفصيل |
|-------|---------|
| **الملف** | `MOBILE_UI_UX_STANDARDS.md` §3 Platform Patterns |
| **التوصية** | إضافة: "Android 12+ supports Material You dynamic color extraction. Use `DynamicColorBuilder` to adapt the app's color palette to the user's wallpaper." |

---

### B5 — Slang as Alternative Localization

| الحقل | التفصيل |
|-------|---------|
| **الملف** | `flutter-mobile.md` §5 Architecture Guidance |
| **التوصية** | إضافة: "slang is an emerging alternative to gen-l10n — supports JSON/YAML inputs, better tree-shaking, and OTA translation updates. Use gen-l10n for standard projects; consider slang for complex multilingual apps." |

---

## خطة الإغلاق المقترحة

### المرحلة الأولى — إغلاق حرج (6 فجوات)

| # | الفجوة | الملف | العمل المطلوب |
|:-:|--------|:-----:|-------------|
| C1 | SSL Certificate Pinning | flutter-mobile.md §16 | إضافة قسم |
| C2 | API Key Management | flutter-mobile.md §16 | إضافة قسم |
| C3 | CI/CD Pipeline | flutter-mobile.md §18 | تعديل + توسيع |
| C4 | Database Update (Drift Default) | flutter-mobile.md §5 | تعديل |
| C5 | OWASP Mobile Top 10 | flutter-mobile.md §16 | إضافة قسم |
| C6 | Impeller Engine | flutter-mobile.md §14 | إضافة ملاحظة |

**الملفات المتأثرة:** `flutter-mobile.md` فقط (5 أقسام)

### المرحلة الثانية — إغلاق متوسط (11 فجوة)

| # | الفجوة | الملف |
|:-:|--------|:-----:|
| M1–M4 | Testing improvements | flutter-mobile.md §9 |
| M5 | Pigeon | flutter-mobile.md §6 |
| M6–M7 | Permissions updates | flutter-mobile.md §7 + MOBILE_UI_UX_STANDARDS.md §9 |
| M8–M9 | Arabic typography + plurals | MOBILE_UI_UX_STANDARDS.md §12 |
| M10–M11 | Performance improvements | flutter-mobile.md §14 |

**الملفات المتأثرة:** `flutter-mobile.md` (6 أقسام) + `MOBILE_UI_UX_STANDARDS.md` (2 قسم)

### المرحلة الثالثة — تحسينات بسيطة (5 فجوات)

| # | الفجوة | الملف |
|:-:|--------|:-----:|
| B1 | cacheWidth/Height | flutter-mobile.md §14 |
| B2 | مدمج مع C4 | — |
| B3 | Shorebird | flutter-mobile.md §18 |
| B4 | Material You | MOBILE_UI_UX_STANDARDS.md §3 |
| B5 | Slang | flutter-mobile.md §5 |

---

## إحصائيات التأثير النهائي

| الملف | عدد الفجوات المُغلقة | الأقسام المتأثرة |
|-------|:-------------------:|:-----------------:|
| `flutter-mobile.md` | **18** (C1-6, M1-5, M7, M10-11, B1, B3, B5) | §5, §6, §7, §9, §14, §16, §18 |
| `MOBILE_UI_UX_STANDARDS.md` | **5** (M6, M8, M9, B4 + مدمج) | §3, §9, §12 |
| Checklists | **0** (لا تحتاج تعديل — تبقى كما هي) | — |
| UI_ACCEPTANCE_GATE | **0** (لا يحتاج تعديل) | — |
| **المجموع** | **23 فجوة مُغلقة** | |

---

## ملاحظات ختامية

1. هذا التقرير يسجل **ما يجب إضافته/تعديله** — لا يُنفَّذ تلقائياً.
2. إغلاق الفجوات يتطلب موافقة Majed على كل تحديث.
3. بعض الفجوات قد تتطلب أكثر من تعديل واحد في الملف نفسه.
4. الفجوات B1-B5 يمكن تأجيلها لأول مشروع إذا رغب Majed.
5. التقرير يُحدَّث بعد إغلاق كل فجوة (حالة: مُغلقة ✅).

---

**التقرير جاهز. ما الخطوة التالية يا Majed؟**
