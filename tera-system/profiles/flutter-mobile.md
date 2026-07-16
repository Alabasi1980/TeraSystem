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

| Concern | Recommendation |
|---|---|
| State Management | Provider / Riverpod / Bloc (choose per project complexity) |
| Navigation | GoRouter / AutoRoute |
| API Client | Dio / http |
| Local Storage | SharedPreferences / Hive / Isar / Drift (SQLite) |
| Dependency Injection | GetIt / Riverpod auto-dispose |
| Routing / Deep Links | GoRouter with deep link support |
| Localization | Flutter l10n (ARB files) + intl |

### Architecture rules

- Do not commit to a state management approach before confirming with the project's `TECHNICAL_ARCHITECTURE.md` or `PROJECT_RULES.md`.
- For small projects (1–3 screens, minimal state), Provider is sufficient.
- For medium+ projects, prefer Riverpod or Bloc.
- Do not add both Provider and Bloc — choose exactly one.
- Navigation must support deep links if the project uses push notifications or external links.

### Offline / Sync

| Scenario | Approach |
|---|---|
| Read-only offline | Cache API responses with Hive/Isar |
| Write offline (queue) | Local DB (Drift/Isar) + sync engine |
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

## 7. Push Notifications

| Service | Package | Notes |
|---|---|---|
| Firebase Cloud Messaging (FCM) | `firebase_messaging` | Default for Android + iOS |
| Local notifications | `flutter_local_notifications` | Scheduled / in-app notifications |

- FCM requires Firebase project setup + `google-services.json` (Android) + `GoogleService-Info.plist` (iOS).
- Local notifications do not require Firebase.
- Do not add push notifications unless explicitly in scope.

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
| Unit tests | `flutter_test` | For models, services, controllers |
| Widget tests | `flutter_test` | For widgets with mocked dependencies |
| Integration tests | `integration_test` package | For full user flows |
| Golden tests | `alchemist` / `golden_toolkit` | For visual regression |

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
| Frame rate | Maintain 60fps (120fps on high-refresh devices) |
| Image loading | Use `cached_network_image`; avoid full-resolution images |
| List performance | Use `ListView.builder` / `GridView.builder` for large lists |
| State rebuilds | Minimize with `const` constructors, `select()` in Riverpod |
| Bundle size | Use `--split-debug-info`, `--obfuscate`, `--tree-shake-icons` |
| Memory | Dispose controllers, streams, and subscriptions |

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

## 16. Security (Reference Only)

Refer to existing Tera security policies:

- `project-preparation/15_SECURITY_AND_ACCESS_CONTROL.md`
- `tera-system/TeraPreExecutionGate.md` (secret handling rules)
- `project-preparation/PROJECT_RULES.md`

Flutter-specific security notes:

- Use `flutter_secure_storage` for tokens — not `SharedPreferences`.
- Obfuscate release builds (`--obfuscate`).
- Do not hard-code API keys or secrets.
- Validate SSL/TLS in production (do not disable certificate verification).
- For biometric auth, use `local_auth` package.

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

## 18. DevOps / Release (Reference Only)

Refer to:

- `project-control/PROJECT_DETAILED_EXECUTION_PLAN.md`
- `project-preparation/22_DEPLOYMENT_AND_ENVIRONMENTS.md`
- `tera-system/runtime/VERSION_LIFECYCLE_PROTOCOL.md`

Flutter release notes:

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

- **Current Profile Version:** 1.0.0
- **Last Updated:** 2026-07-15
- **Maintainer:** TeraSystemEvolutionAgent (حارس)
- **Flutter Baseline:** 3.16+ (stable channel)
- **Dart Baseline:** 3.0+
