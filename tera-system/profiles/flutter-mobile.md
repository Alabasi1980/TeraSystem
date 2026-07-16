# Flutter Mobile Technology Profile

## 1. Purpose

This profile provides Flutter/Dart-specific execution rules for mobile application projects in the Tera system.

It is a **stack-specific Technology Profile** — loaded only when a project is classified as Mobile with Flutter as the chosen framework.

## 2. Identity

| Field | Value |
|---|---|
| Profile ID | `flutter-mobile` |
| Stack | Flutter + Dart |
| Minimum Dart SDK | 3.0+ |
| Minimum Flutter SDK | 3.16+ (current stable channel) |
| Status | Available |
| Owner | Tera Agent (decision engine) / EngineeringAgent (execution) |

## 3. When to Load This Profile

Load `flutter-mobile.md` when:

- Project classification = **Mobile**
- Technology stack confirmed as **Flutter + Dart**
- `project-control/PROJECT_STATE.md` or `project-preparation/08_TECHNICAL_ARCHITECTURE.md` records `Active Technology Profile: flutter-mobile`

### Automatic activation triggers

| Trigger | Action |
|---|---|
| Project type = Mobile AND Stack = Flutter | Load `flutter-mobile.md` as active profile |
| `PROJECT_STATE.md` sets `flutter-mobile` | Load immediately |
| Pre-Execution Gate detects mobile + Flutter | Verify profile is loaded; BLOCKED if not |

## 4. Project Structure

Standard Flutter project layout (created by `flutter create`):

```
lib/
  main.dart
  app.dart
  core/
    theme/
    constants/
    network/
    router/
    utils/
  features/
    {feature_name}/
      data/
      domain/
      presentation/
  l10n/                          # Localization (ARB files)
test/
integration_test/
android/
ios/
web/                             # Optional — web target
```

### Rules

- Do not create additional top-level directories unrelated to the project scope.
- Prefer feature-first structure over layer-first for medium+ projects.
- Keep `main.dart` minimal — app configuration belongs in `app.dart` or `core/`.

## 5. Architecture Guidance

| Concern | Recommendation (2026) |
|---|---|
| State Management | **Riverpod** (default for new projects) / Bloc (enterprise/regulated) / Provider (legacy/simple only) |
| Navigation | GoRouter / AutoRoute |
| API Client | Dio / http |
| Local Storage | **Drift (SQLite)** — default. See database warning below. |
| Dependency Injection | GetIt / Riverpod auto-dispose |
| Routing / Deep Links | GoRouter with deep link support |
| Localization | Flutter l10n (ARB files) + intl (standard). `slang` as alternative for complex multilingual apps. |

### Architecture rules

- Do not commit to a state management approach before confirming with the project's `TECHNICAL_ARCHITECTURE.md` or `PROJECT_RULES.md`.
- For new projects (2026+): **Riverpod 3.x** is the default — compile-time safety, built-in async handling (`AsyncValue`), automatic provider disposal, no `BuildContext` dependency.
- Bloc remains the choice for **enterprise/regulated apps** (fintech, healthcare) where explicit event audit trails matter.
- Provider is **legacy** — acceptable only for very small projects (1-3 screens, minimal state).
- Do not add both Provider and Bloc in the same project — choose exactly one.
- Navigation must support deep links if the project uses push notifications or external links.
- **Architecture approach:** Feature-first at the top level, Clean Architecture layers (data/domain/presentation) inside each feature only if justified by complexity. Do not apply Clean Architecture to simple CRUD screens.
- **Repository pattern:** Use it, but do NOT add interfaces for every repository if you only have one implementation. Add abstractions when you have a concrete need (testing, swapping implementations).

### Database Warning (2026)

| Type | Package | Status | Recommendation |
|------|---------|:------:|:--------------|
| Relational (SQL) | **Drift** (SQLite) | ✅ Actively maintained | **Default choice** |
| NoSQL (local) | Hive | ⚠ Abandoned by author | **Do not use for new projects** |
| NoSQL (local) | Isar | ⚠ Abandoned (community fork: `isar-community`) | **Maintenance risk — avoid** |
| NoSQL (sync) | ObjectBox | ✅ Commercial with sync | Use only if offline-first sync is required |
| KV storage | SharedPreferences | ✅ Maintained | Simple key-value only (not for structured data) |

**Critical warning:** Hive and Isar were both abandoned by their original authors. **Drift is the safe default for all new projects.**

### Offline / Sync

| Scenario | Approach |
|---|---|
| Read-only offline | Cache API responses with Drift (SQLite) / ObjectBox |
| Write offline (queue) | Local DB (Drift/ObjectBox) + sync engine |
| Real-time | WebSocket / Firebase Realtime / MQTT |

- Offline support must be explicitly requested in scope. Do not add offline capabilities by default.
- If offline is required, document the sync conflict resolution strategy (last-write-wins / timestamp / manual).

## 6. Permissions

Common Flutter permissions:

| Permission | Required For | Notes |
|---|---|---|
| Camera | Photo/video capture | `image_picker` or `camera` package |
| Location (background) | GPS tracking | `geolocator` + `permission_handler` |
| Location (foreground) | Map, near-by features | `geolocator` |
| Notifications | Push notifications | `firebase_messaging` / `flutter_local_notifications` |
| Storage (Android < 10) | File download/save | `permission_handler` |
| Photos/Media (Android 13+) | Image picker | Declare in `AndroidManifest.xml` |
| Biometric | Fingerprint/Face auth | `local_auth` |

### Rules

- Request permissions at runtime — do not assume they are granted.
- Handle denied / permanently-denied states gracefully (show rationale, open settings).
- Document every permission in the project's `15_SECURITY_AND_ACCESS_CONTROL.md` or `PROJECT_RULES.md`.
- **Do not add permissions not required by the approved scope.**

### Platform Channel Communication

When communicating with native code (Android/iOS):

| Method | When | Recommendation |
|--------|------|:--------------:|
| **Pigeon** | Custom native integration | Default (type-safe, less boilerplate) |
| `MethodChannel` | Simple one-off calls | Acceptable |
| `EventChannel` | Streaming data from native | Acceptable |

**Rule:** For new projects, prefer **Pigeon** for platform channel communication. It generates type-safe Dart and native code, eliminating string-based API errors.

## 7. Push Notifications

| Service | Package | Notes |
|---|---|---|
| Firebase Cloud Messaging (FCM) | `firebase_messaging` | Default for Android + iOS |
| Local notifications | `flutter_local_notifications` | Scheduled / in-app notifications |

- FCM requires Firebase project setup + `google-services.json` (Android) + `GoogleService-Info.plist` (iOS).
- Local notifications do not require Firebase.
- Do not add push notifications unless explicitly in scope.

### Android 13+ Notification Permission

Starting with **Android 13 (API 33)**, notification permission is a **runtime permission** — must be requested like camera or location.

- Use `permission_handler` to request `Permission.notification` before registering for push notifications.
- Handle denied state consistently (show in-app prompt explaining value before the OS dialog).
- Use "demonstrate value first" pattern — show what notifications look like before requesting.
- For local notifications without FCM, this permission is still required on Android 13+.

## 8. Secure Storage

| Use Case | Package |
|---|---|
| API tokens, keys | `flutter_secure_storage` |
| Biometric auth | `local_auth` |

- Do not store secrets in `SharedPreferences` or `Hive`.
- Use platform keystore (Keychain on iOS, EncryptedSharedPreferences on Android).

## 9. Testing

| Type | Tool | Notes |
|---|---|---|
| Unit tests | `flutter_test`, `mocktail` | For models, services, controllers |
| Widget tests | `flutter_test`, `mocktail` | For widgets with mocked dependencies |
| Integration tests | `integration_test` (+ **Patrol** for native interactions) | For full user flows |
| Golden tests | `alchemist` (replaces `golden_toolkit`) | For visual regression (design system components only) |

### Testing Pyramid (2026 Standard)

| Layer | Ratio | Speed | What to test |
|-------|:-----:|:-----:|-------------|
| Unit tests | **60-70%** | Fast | Business logic, services, controllers, repositories |
| Widget tests | **20-25%** | Medium | UI rendering, interactions, state changes |
| Integration tests | **~10%** | Slow | Critical user flows (login, checkout, primary workflow) |
| Golden tests | **~5%** | Medium | Design system components (visual regression) |

- **Target coverage:** 80-95% for business logic, 70-80% for widget UI. Exclude generated code (freezed, json_serializable, drift).
- **High coverage does not equal quality tests.** Focus on behavior, not hitting every line.

### Mocking

| Library | When | Recommendation |
|---------|------|:--------------:|
| **mocktail** | New projects | Default — no code generation, clean API |
| mockito | Existing projects with mockito already set up | Acceptable (requires `build_runner`) |

**Rule:** Choose exactly one mocking library per project. Do not mix `mocktail` and `mockito`.

### Integration Testing

- Use built-in `integration_test` package for standard flows.
- Use **Patrol** (by LeanCode) when the test requires native device interactions:
  - Permission dialogs (accept/deny)
  - Notification handling
  - Platform views
  - Deep links from external sources
- Patrol runs on Firebase Test Lab for CI — recommended for critical user flows.

### Rules

- Unit tests are required for services, repositories, and state management.
- Widget tests are required for complex or reusable widgets.
- Integration tests are required for critical user flows (login, checkout, primary workflow).
- Golden tests are optional — use only when visual consistency is critical.
- Do not test generated code (freezed, json_serializable).
- Run `flutter test` before marking any task as complete.

## 10. Android Configuration

| Item | File | Notes |
|---|---|---|
| App ID | `android/app/build.gradle` | `applicationId` |
| Min SDK | `android/app/build.gradle` | Default 21; adjust per project |
| Target SDK | `android/app/build.gradle` | Latest stable |
| Signing | `android/key.properties` + `android/app/build.gradle` | Debug + Release keystore |
| ProGuard | `android/app/proguard-rules.pro` | Keep Flutter engine classes |
| Permissions | `android/app/src/main/AndroidManifest.xml` | Declare required permissions |

### Build commands

```bash
# Debug APK
flutter build apk --debug

# Release APK
flutter build apk --release

# App Bundle (recommended for Play Store)
flutter build appbundle --release

# Android AAB with split per ABI
flutter build appbundle --release --target-platform android-arm,android-arm64,android-x64
```

## 11. iOS Configuration

| Item | File | Notes |
|---|---|---|
| Bundle ID | `ios/Runner.xcodeproj` / Xcode project | Reverse-domain |
| Minimum iOS version | `ios/Podfile` or Xcode project | Default 12.0 |
| Signing (dev) | Xcode Automatic signing | Requires Apple Developer account |
| Signing (release) | Xcode / Fastlane match | Distribution certificate |
| Permissions | `ios/Runner/Info.plist` | Privacy usage descriptions |

### Important Constraints

| Constraint | Notes |
|---|---|
| **iOS build requires macOS** | Flutter iOS builds only work on macOS with Xcode |
| **iOS simulator does not support camera/motion** | Test camera/location on real device |
| **App Store requires Apple Developer Program** | $99/year for distribution |
| **iOS code signing** | Manual or Fastlane; must match bundle ID |

### Build commands

```bash
# Debug (requires macOS + Xcode)
flutter build ios --debug

# Release (requires macOS + Xcode)
flutter build ios --release

# Archive for App Store (requires macOS + Xcode)
flutter build ios --release --no-codesign
# Then open ios/Runner.xcworkspace in Xcode -> Archive
```

### iOS Validation Environment Constraint

```text
إذا كانت بيئة iOS غير متوفرة (لا يوجد macOS/Xcode):
- نفّذ Flutter + Android validation حيثما أمكن
- سجّل iOS validation كـ "Blocked by Environment" أو "Deferred"
- لا يُعتبر نجاح Android alone دليلاً على جاهزية iOS Release
```

## 12. Environment Verification

```bash
# Verify Flutter SDK is installed
flutter doctor

# Verify Dart SDK version
dart --version

# Verify project dependencies
flutter pub get

# Run all tests
flutter test
```

### Pre-project package compatibility check

Before starting any Flutter project:
1. Run `flutter pub outdated` on a fresh test project.
2. Verify all intended packages are compatible with current Flutter/Dart version.
3. Log the Flutter/Dart version used in `PROJECT_STATE.md` or `PROJECT_RULES.md`.

## 13. Dependency Governance

| Rule | Notes |
|---|---|
| Use stable packages only (no beta/dev unless explicitly approved) | Check pub.dev for status |
| Prefer well-maintained packages (updated within 6 months) | Check last update date |
| Avoid duplicate packages for the same purpose | Choose one HTTP client, one state management, etc. |
| Document any package that requires platform-specific setup | Firebase, camera, maps, etc. |
| Run `flutter pub outdated` at project start and before major upgrades | Log versions used |
| Do not add packages without explicit scope approval | Every package must serve an approved feature |

## 14. Performance

| Concern | Guideline |
|---|---|
| Frame rate | Maintain 60fps (16.67ms per frame) / 120fps (8.33ms per frame) |
| Image loading | Use `cached_network_image` with `cacheWidth`/`cacheHeight` — decode images at target size (never load 4K into a 100px avatar) |
| List performance | Use `ListView.builder` / `GridView.builder` for any list over 20-30 items; never use `ListView(children: [...])` for dynamic lists |
| State rebuilds | Minimize with `const` constructors everywhere possible, `select()` in Riverpod |
| Bundle size | Use `--split-debug-info`, `--obfuscate`, `--tree-shake-icons` |
| Memory | Dispose controllers, streams, and subscriptions — missing `dispose()` is the #1 memory leak cause |
| RepaintBoundary | Use strategically around animated widgets, complex list items — but NOT everywhere (each RepaintBoundary costs ~50KB memory) |

### Impeller Rendering Engine

As of **Flutter 3.44+**, Impeller is the default rendering engine on iOS and available on Android. It eliminates shader compilation jank — the most common cause of first-use stuttering.

| Platform | Impeller Status | Action |
|----------|:---------------:|--------|
| iOS | Default (enabled) | No action needed |
| Android | Available (opt-in) | Enable in `AndroidManifest.xml`: `<meta-data android:name="io.flutter.embedding.android.EnableImpeller" android:value="true"/>` |

**Key benefit:** Impeller pre-compiles all shaders at build time. No shader compilation jank on first use.

### Performance Budget

| Target | Frame Budget | How to Measure |
|--------|:-----------:|----------------|
| 60 fps (standard) | **16.67 ms** per frame | Flutter DevTools Performance tab |
| 120 fps (high refresh) | **8.33 ms** per frame | Flutter DevTools Performance tab |

**Profiling rules:**
- Profile on **real devices in profile mode** — emulator/simulator numbers are meaningless.
- Use Flutter DevTools to identify whether bottleneck is UI thread (Dart code, rebuilds) or raster thread (painting, images).
- If frame time consistently exceeds budget, investigate.

### Shader Compilation

With **Impeller** enabled (Flutter 3.44+), shader compilation jank is eliminated.

If Impeller is NOT available (older Flutter versions, Android without Impeller):
- Use `ShaderWarmUp` class to pre-compile shaders during splash screen.
- Target the most common rendering operations (blur, gradient, path).

**Recommendation:** Always use the latest Flutter stable channel to get Impeller by default.

### Performance Checklist

- [ ] Profile on physical device in profile mode (not debug, not emulator)
- [ ] const constructors used where possible
- [ ] `ListView.builder` / `GridView.builder` for scrollable content
- [ ] Images decoded at display size (cacheWidth/cacheHeight)
- [ ] Controllers/streams disposed appropriately
- [ ] Impeller enabled (verify on target platform)

### Build optimization

```bash
# Production build with optimizations
flutter build apk --release --split-debug-info=build/debug-info --obfuscate
flutter build appbundle --release --split-debug-info=build/debug-info --obfuscate
```

## 15. Known Flutter Limits (Document These)

| Limit | Notes |
|---|---|
| **Platform-specific features** | Camera, Bluetooth, NFC, USB, fingerprint — require platform channels or native packages |
| **Background execution** | Limited on iOS; Android WorkManager for background tasks |
| **Desktop/web** | Flutter web and desktop are separate targets; not interchangeable with mobile |
| **Native performance** | Heavy 3D, real-time video processing, or complex animations may need native code |
| **Platform channels** | Custom native code requires Kotlin/Swift knowledge — specialist work |

### Escalation Path

When a requirement exceeds Flutter's capabilities (native-heavy, unsupported hardware, etc.):

1. **Assess** — Is this doable in Flutter with a package? If yes, proceed.
2. **Escalate to specialist** — If platform channels, native modules, or complex native features are needed, create a focused task for native development.
3. **Defer** — If the feature is optional and native-heavy, defer to a future phase.
4. **Request Tera decision** — If unclear, generate an AIS suggestion or ask Majed.

## 16. Security

Refer to existing Tera security policies:

- `project-preparation/15_SECURITY_AND_ACCESS_CONTROL.md`
- `tera-system/TeraPreExecutionGate.md` (secret handling rules)
- `project-preparation/PROJECT_RULES.md`

### Flutter-Specific Security Rules

| Rule | Detail |
|------|--------|
| Secure storage | Use `flutter_secure_storage` for tokens/keys — never `SharedPreferences` or `Hive` |
| Code obfuscation | `--obfuscate --split-debug-info` on all release builds (necessary but not sufficient — strings still extractable) |
| No hardcoded secrets | API keys in Dart source are extractable even with obfuscation |
| SSL/TLS validation | Always validate in production — never disable certificate verification |
| Biometric auth | `local_auth` package paired with `flutter_secure_storage` for protected data |

### SSL Certificate Pinning

When the app communicates with an API handling sensitive data (financial, personal, credentials):

- **Pin to the public key hash** (not the full certificate) using `http_certificate_pinning` or a custom `SecurityContext` with Dart's `HttpClient`.
- Always include a fallback strategy (e.g., disable pinning after X failed attempts to avoid permanent app lockout).
- HTTPS alone is vulnerable to MITM attacks using compromised CA certificates — pinning is the extra layer.

| App Type | SSL Pinning Required? |
|----------|:---------------------:|
| Handles user data + authentication | Yes |
| Read-only public content | Not required |
| Financial/healthcare data | Yes (mandatory) |

### API Key Management

| Method | When | Security Level |
|--------|------|:--------------:|
| `--dart-define=KEY=value` | Compile-time injection from env | Medium |
| `flutter_dotenv` + `.env` (gitignored) | Local development | Medium |
| **Backend proxy** (key stays on server) | **Production** | **High** |

**Rules:**
- For production apps, route API calls through a backend proxy so keys never reach the client device.
- For development, use `--dart-define` for compile-time injection.
- Never commit `.env` files to version control.
- Never hardcode keys in Dart source files — they are extractable with basic tools even after obfuscation.

### OWASP Mobile Top 10 — Reference Framework

This profile does not require full compliance for every project. However, Tera must assess which risks apply and document mitigations per project:

| OWASP Risk | Flutter Mitigation | When Relevant |
|---|---|---|
| **M1:** Improper Credential Usage | `flutter_secure_storage` + backend proxy | All apps with auth |
| **M2:** Inadequate Supply Chain Security | Audit third-party packages, use `dart pub deps` | All projects |
| **M3:** Insecure Authentication | Short-lived JWT + refresh tokens + token rotation | Apps with login |
| **M4:** Insufficient Input/Output Validation | Server-side validation (never trust client input) | Apps with API calls |
| **M5:** Insecure Communication | **SSL pinning** + TLS 1.2+ | Apps with sensitive data |
| **M6:** Inadequate Privacy Controls | Biometric lock + private notification previews | Apps with personal data |
| **M7:** Insufficient Binary Protections | `--obfuscate --split-debug-info` | All release builds |
| **M8:** Security Misconfiguration | No debug flags in release, proper Keychain config | All projects |
| **M9:** Insecure Data Storage | `flutter_secure_storage` for secrets, encrypted DB for sensitive data | All projects |
| **M10:** Insufficient Cryptography | Use platform crypto (not custom implementations) | Apps with encryption |

**Rule:** During Pre-Execution Gate for mobile projects, assess which OWASP Mobile risks apply and document mitigations.

## 17. QA / Acceptance (Reference Only)

Refer to:

- `project-preparation/10_TESTING_AND_ACCEPTANCE.md`
- `tera-system/runtime/TERA_RUNTIME_CHECKLISTS.md` (§ Mobile Checklists)
- `tera-system/design-system/UI_ACCEPTANCE_GATE.md`

Flutter QA rules:

- Test on both Android and iOS (or document if iOS is environment-blocked).
- Test on at least one real device per target platform.
- Test offline/online states.
- Test permission grant/deny/deny-permanently flows.
- Test with weak network (throttled) if the app uses API calls.
- Test RTL/LTR layout for multilingual apps.

## 18. DevOps / Release

### 18.1 CI/CD Pipeline (Industry Standard)

| Stage | Tool | Environment | Trigger |
|-------|------|:-----------:|---------|
| PR Validation | GitHub Actions | Linux | Every PR |
| Staging Build | Codemagic | macOS | Push to develop |
| Production Release | Codemagic + Fastlane | macOS + Store | Git tag |

**PR Validation (Linux — fast and cheap):**
- `flutter analyze` (zero warnings — fail on any)
- `flutter test` (all pass)
- `dart run build_runner build --delete-conflicting-outputs` (codegen check)
- Code formatting check (`dart format --set-exit-if-changed`)

**Staging Build (macOS — Codemagic recommended):**
- `flutter build apk --release`
- `flutter build ios --release --no-codesign` (staging, unsigned)
- Upload to TestFlight (iOS) + Play Internal Track (Android)

**Production Release:**
- `flutter build appbundle --release --obfuscate --split-debug-info=./build/debug-info`
- `flutter build ipa --release --obfuscate --split-debug-info=./build/debug-info`
- Upload to Google Play (production track) + App Store Connect
- Upload crash symbols to Sentry or crash reporting tool

### 18.2 Recommended Tools

| Tool | Purpose | Why |
|------|---------|-----|
| **Codemagic** | Flutter-native CI/CD | Built-in iOS code signing, Apple Silicon Mac instances, free 500 min/month |
| **GitHub Actions** | PR validation | Linux runners: fast and cheap for tests/analysis |
| **Fastlane** | Store submission automation | Upload to Play Store / App Store Connect, manage metadata and screenshots |
| **Shorebird** | OTA Dart code updates | Skip App Store review for Dart-only changes. Cannot change native code, assets, or platform channels. Complementary to CI/CD. |

### 18.3 Version Automation

- Use sequential build numbers: `1.0.0+1`, `1.0.0+2`, etc.
- Pass via `flutter build ... --build-name=1.0.0 --build-number=$BUILD_NUMBER`
- Never use timestamp-based build numbers in production.
- Increment build number for every submission (including TestFlight).

### 18.4 Store Deployment

| Store | File | Signing | Submission |
|-------|------|---------|------------|
| Google Play | `.aab` (App Bundle) | Upload key + App signing by Google | Play Console to Production track |
| App Store | `.ipa` (Xcode Archive) | Apple Distribution Certificate + Provisioning Profile | App Store Connect to TestFlight to Review |

### Flutter release notes

- Android: AAB for Play Store, APK for direct distribution.
- iOS: IPA via Xcode Archive + App Store Connect.
- Version strings in `pubspec.yaml`: `version: 1.0.0+1` (semantic + build number).
- For iOS, increment build number for each TestFlight submission.

## 19. References

| Topic | Source |
|---|---|
| Flutter official docs | https://docs.flutter.dev |
| Dart official docs | https://dart.dev |
| State management | Riverpod / Bloc / Provider |
| Flutter project structure guidelines | https://docs.flutter.dev/get-started/codelab |
| Tera Design Governance | `tera-system/design-system/` |
| Tera Mobile UI/UX Standards | `tera-system/design-system/MOBILE_UI_UX_STANDARDS.md` |
| Tera Pre-Execution Gate | `tera-system/TeraPreExecutionGate.md` |
| Tera QA/Acceptance | `tera-system/runtime/TERA_RUNTIME_CHECKLISTS.md` |
| Tera Security | `tera-system/TeraPreExecutionGate.md` (Secret Handling) |

## 20. Profile Version

- **Current Profile Version:** 1.1.0
- **Last Updated:** 2026-07-16
- **Updated Sections (v1.0 to v1.1):** §5 (Architecture, database warning), §6 (Pigeon), §7 (Android 13+), §9 (Testing pyramid, mocktail, Patrol), §14 (Impeller, performance budget, shader), §16 (SSL pinning, API keys, OWASP), §18 (CI/CD pipeline, tools, version automation)
- **Last Updated By:** Gap Analysis Closure (SCP-096 Phase 2)
- **Maintainer:** TeraSystemEvolutionAgent (حارس)
- **Flutter Baseline:** 3.16+ (stable channel)
- **Dart Baseline:** 3.0+
