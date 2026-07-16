# Mobile UI/UX Standards

## 1. Purpose

This file defines the mobile-specific design standards and UX rules for Flutter mobile application projects.

It is an **extension** to `tera-system/design-system/` — loaded only when a project is classified as **Mobile**.

It does not replace:
- `tera-system/design-system/DESIGN_REVIEW_STANDARDS.md`
- `tera-system/design-system/UI_ACCEPTANCE_GATE.md`
- `project-preparation/28_UI_UX_GUIDELINES.md`

It supplements them with mobile-specific concerns.

---

## 2. Core Principle

```text
Mobile is not "shrunk web."
A mobile-first design is built from mobile interaction patterns, not from scaling down a desktop interface.
```

| Wrong Approach | Correct Approach |
|---|---|
| Shrink web layout to fit mobile | Design for touch-first, thumb zones, and screen size |
| Use desktop hover patterns | Use tap, swipe, long-press gestures |
| Stack desktop navigation inside mobile | Use bottom navigation, tab bars, or drawer |
| Keep desktop forms as-is | Break forms into steps, use mobile keyboards |
| Show full tables | Use cards, lists, or horizontal scroll |

---

## 3. Platform Patterns

### 3.1 Android (Material Design 3)

- Use Material 3 components (M3) as the baseline.
- Navigation: Bottom Navigation Bar (3–5 destinations) or Navigation Rail (tablet/landscape).
- App Bar: Standard `AppBar` with title + actions.
- FAB: Floating Action Button for primary action when appropriate.
- Back navigation: System back button + AppBar back arrow.
- Dialogs: `AlertDialog`, `BottomSheet` for selections.

### 3.2 iOS (Cupertino / Human Interface Guidelines)

- Use Cupertino widgets for native iOS feel when the app is iOS-only or platform-adaptive.
- Navigation: Tab Bar (bottom) + Navigation Stack.
- App Bar: Large title + inline title patterns (`.large`, `.inline`).
- Back navigation: Swipe-back gesture + back chevron.
- Sheets: Modal bottom sheets for actions and filters.
- Pickers: iOS-style wheel pickers for date/time/option selection.

### 3.3 Cross-Platform (Platform Adaptive)

When targeting both Android and iOS:

- Use `ThemeData` + `CupertinoThemeData` for platform-adaptive styling.
- Use `Platform.isAndroid` / `Platform.isIOS` sparingly — prefer adaptive widgets.
- Navigation: `GoRouter` with platform-adaptive transitions (slide iOS, fade Android).
- Dialogs: `adaptive` dialogs that match platform conventions.

**Rule:** Do not force iOS users into Android navigation patterns or vice versa. Use platform detection for navigation style, date pickers, and dialog styles.

### 3.4 Dynamic Color / Material You (Android 12+)

- Android 12+ supports **Material You** dynamic color — the app color palette can adapt to the user wallpaper.
- Use `DynamicColorBuilder` (or `ColorScheme.fromSeed`) to extract tonal colors.
- Flutter M3 supports this natively with `useMaterial3: true` + `ColorScheme.fromSeed(seedColor: ...)`.
- Allow users to toggle: "Follow system colors" vs "Use app theme" in settings.
- Optional — do not force if the project has a fixed brand palette.

---

## 4. Touch Targets

| Element | Minimum Size | Notes |
|---|---|---|
| Buttons (tappable) | 48 x 48 dp | 44 pt on iOS; 48 dp recommended by Material |
| Icon buttons | 48 x 48 dp | Include adequate padding |
| List items | 48 dp height | Minimum tap target per item |
| Form fields | 48 dp height | Comfortable for text entry |
| Links / inline taps | 44 x 44 dp | Ensure adequate hit area |

### Spacing rules
- Minimum 8 dp between tappable elements.
- Content padding: 16 dp (mobile) / 24 dp (tablet).
- Touch targets should include visual feedback (ink splash / highlight).

---

## 5. Gestures

| Gesture | Use Case | Fallback |
|---|---|---|
| Tap | Primary action (button click, select) | Always have a visible button alternative |
| Long press | Context menu, reorder, delete | Provide icon button as fallback |
| Swipe (horizontal) | Dismiss, navigate between tabs | Always have a tap/button alternative |
| Swipe (vertical) | Pull-to-refresh, scroll | Always have a refresh button alternative |
| Pinch | Zoom in/out | Provide +/- zoom buttons |
| Drag | Reorder list items | Provide move up/down buttons |

### Critical Rule
```text
Never rely on gestures as the only way to perform a primary action.
Always provide a visible tappable alternative.
```

---

## 6. Safe Areas and Notch

- Use `SafeArea` widget to avoid notch, status bar, home indicator on iOS.
- Use `MediaQuery.padding` and `ViewInsets` for keyboard handling.
- On Android: handle cutouts (notch/punch-hole) via `systemUIOverlayStyle`.

### Keyboard Behavior
- When keyboard opens: scroll content, do not overlap input fields.
- Use `resizeToAvoidBottomInset: true` in Scaffold.
- Provide "Done" / dismiss keyboard button for numeric fields.
- Test all forms with keyboard open.

---

## 7. Navigation

| Pattern | Best For | Notes |
|---|---|---|
| Bottom Navigation Bar | 3–5 top-level destinations | Standard mobile pattern |
| Tab Bar (iOS) | 2–5 top-level destinations | iOS-native feel |
| Navigation Drawer | 5+ destinations, settings | Use only when bottom nav is insufficient |
| Stack Navigation | Drill-down (list → detail) | Default for sub-screens |
| Modal / Bottom Sheet | Filters, actions, picks | Keep sheets short; avoid nested scroll |

### Navigation hierarchy
- Max 3 levels deep for primary workflows (tab → list → detail).
- For deeper navigation, use breadcrumbs or step indicators.
- Do not mix Bottom Navigation + Drawer unless absolutely necessary.

---

## 8. Loading, Empty, Error, Offline States

Every screen must handle these states:

| State | UI Element | Notes |
|---|---|---|
| **Loading** | Skeleton shimmer / CircularProgressIndicator | Show immediately on navigation |
| **Empty** | Illustration + message + CTA | "No items yet — Create first item" |
| **Error** | Error illustration + message + retry button | "Something went wrong — Try again" |
| **Offline** | Offline banner / snackbar + cached content | Show at top; allow browsing cached data |
| **Permission denied** | Rationale explanation + settings button | "Camera access needed — Open Settings" |

### Offline Behavior
- Show offline indicator immediately when connectivity is lost.
- If offline data is available, show it with a "cached" label.
- If offline data is not available, show the offline state with a retry button.
- Reconnect automatically when connectivity returns.

---

## 9. Permissions

Handle every permission state gracefully:

| State | Action |
|---|---|
| Not yet requested | Request at the moment of need (not on app start) |
| Granted | Proceed with the feature |
| Denied | Show explanation + request again |
| Permanently denied | Show explanation + "Open Settings" button |
| Restricted (parental control) | Show "Not available" message |
| Limited (iOS Photos only) | Explain user can add more photos; show system picker for individual additions |

### Rules
- Request permissions **contextually** — when the user triggers the feature, not at app start.
- Explain **why** the permission is needed before requesting (use a dialog or bottom sheet).
- Never crash on denied permissions.
- Test all permission flows (grant, deny, deny permanently).

---

## 10. Responsive Layout

| Device | Layout Behavior |
|---|---|
| **Phone (portrait)** | Single column, bottom navigation |
| **Phone (landscape)** | Single column, consider hiding bottom nav behind hamburger |
| **Small tablet (7")** | Adaptive — single or two-column depending on content |
| **Large tablet (10"+) / Foldable** | Two-column layout (master-detail) or navigation rail |
| **Foldable (unfolded)** | Treat as tablet when unfolded, phone when folded |

### Rules
- Use `LayoutBuilder` or `MediaQuery.size` to adapt layout.
- Do not hard-code a single layout for all screen sizes.
- Test on both phone (360–428 dp) and tablet (600–900 dp) widths.
- Support portrait and landscape (or explicitly lock to portrait if the UI breaks).

---

## 11. Portrait / Landscape

| Application Type | Orientation Rule |
|---|---|
| Form-heavy apps | Portrait preferred; landscape acceptable |
| Media/video apps | Both required |
| Games (simple) | Landscape preferred |
| Reader / document apps | Both required |
| Camera / barcode apps | Portrait preferred |

- Document orientation support in `PROJECT_RULES.md`.
- If landscape is supported, do not crop or break the UI.
- Use `OrientationBuilder` to adapt layout when orientation changes.

---

## 12. RTL / LTR

For Arabic/RTL projects (Tera's primary market):

- Set `Directionality.rtl` at the app root.
- Test all screens with long Arabic text (no truncation or overflow).
- Verify leading/trailing alignment: RTL uses leading = right, trailing = left.
- Check icon alignment: back arrow = right-pointing in RTL.
- Test both RTL and LTR if the app supports multiple languages.

### Standard RTL adjustments
- Padding/margin: reverse left/right values when switching locale.
- Icons: flip horizontally if they imply direction (arrows, chevrons).
- Text alignment: auto-align with `TextAlign.start` / `TextAlign.end`.
- Sliders and progress: reverse direction for RTL.

### Arabic Typography

| Guideline | Value |
|-----------|-------|
| Recommended fonts | Noto Sans Arabic (safe default), Cairo, Tajawal, IBM Plex Sans Arabic |
| Font size vs Latin | Arabic text needs **10-15% larger** size than Latin equivalents for same readability |
| Line height | Minimum **1.6x** for Arabic body text (vs 1.4x for English) |
| fontFamilyFallback | Always include: `['Noto Sans Arabic', 'Roboto']` — ensures Arabic characters render correctly on all devices |
| Font appearance | Arabic at weight 400 may appear thinner than Latin — consider weight 500 for body text |

**Test rule:** Always compare Arabic and English side-by-side at the same font size. If Arabic appears thinner or smaller, adjust size/weight.

### Arabic Plural Forms

Arabic has **6 plural forms** — most apps only handle singular/plural (2 forms), which is incorrect for Arabic.

| CLDR Category | Arabic Form | Example (مهمة) |
|:---:|---|---|
| =0 | لا مهمات | 0 tasks |
| =1 | مهمة واحدة | 1 task |
| =2 | مهمتان | 2 tasks |
| few (3-10) | مهمات | 3-10 tasks |
| many (11-99) | مهمة | 11-99 tasks |
| other (100+) | مهمة | 100+ tasks |

**Rule:** When using ARB files for Arabic, always define all 6 plural forms using ICU message format. Do not assume that singular/plural only handles Arabic — test with count=0, 1, 2, 3, 11, 100.

---

## 13. Accessibility (Mobile)

| Requirement | Standard |
|---|---|
| Touch target size | 48 x 48 dp minimum |
| Text contrast | WCAG AA minimum (4.5:1 normal text, 3:1 large) |
| Screen reader | `Semantics` widget for custom widgets |
| Focus order | Logical (LTR: top→bottom→left→right) |
| Reduce motion | Support `AnimationController` with `disableAnimations` |
| Font scaling | Support system font size changes; test with large fonts |
| Button labels | Use `Semantics` labels for icon-only buttons |

### Checklist
- [ ] All icons have `Semantics` labels.
- [ ] All form fields have `Semantics` hints/labels.
- [ ] Custom gestures have accessible alternatives.
- [ ] System font size changes do not break layout.
- [ ] Screen reader reads content in correct order.

---

## 14. Platform Adaptation

When building for both platforms:

| Element | Android | iOS | Adaptive |
|---|---|---|---|
| App bar | Material AppBar | CupertinoNavBar | Platform-adaptive |
| Bottom nav | BottomNavigationBar | CupertinoTabBar | Platform-adaptive |
| Switch | Material Switch | CupertinoSwitch | Platform-adaptive |
| Slider | Material Slider | CupertinoSlider | Platform-adaptive |
| Date picker | Material DatePicker | CupertinoDatePicker | Platform-adaptive |
| Time picker | Material TimePicker | CupertinoTimerPicker | Platform-adaptive |
| Dialog | AlertDialog | CupertinoAlertDialog | Platform-adaptive |
| Activity indicator | CircularProgressIndicator | CupertinoActivityIndicator | Platform-adaptive |

Use `ThemeMode.system` as default to respect user's system theme (light/dark).

---

## 15. Icons, Splash Screen, and Store Assets

### App Icon
- Android: Adaptive icon (foreground + background layers).
- iOS: App icon set in Assets.xcassets (multiple sizes).
- Generate from a single source using `flutter_launcher_icons`.

### Splash Screen
- Use `flutter_native_splash` package for native splash.
- Keep splash simple: logo + brand background.
- Configure for both light and dark mode.

### Store Assets
| Asset | Android | iOS |
|---|---|---|
| Screenshots | 1–8 per device type (phone, tablet) | 1–8 per device type |
| Feature graphic | 1024 x 500 px | N/A |
| App icon | 512 x 512 px | 1024 x 1024 px |
| Description | 4000 chars max | 4000 chars max |

---

## 16. Design Review Additions (Mobile)

In addition to `DESIGN_REVIEW_STANDARDS.md`, mobile designs must be checked for:

- [ ] Touch targets meet minimum 48x48 dp.
- [ ] No gesture-only primary actions.
- [ ] All states handled: loading, empty, error, offline, permission-denied.
- [ ] Safe areas respected (notch, status bar, home indicator).
- [ ] Keyboard does not overlap input fields.
- [ ] Platform patterns respected (Android vs iOS conventions).
- [ ] RTL layouts tested with long text.
- [ ] Font scaling does not break layout.
- [ ] Dark mode tested if supported.
- [ ] Both portrait and landscape tested (if landscape is supported).
- [ ] Tablet layout adapted (not just stretched phone UI).

---

## 17. UI Acceptance Gate — Mobile Additions

When the project is **Mobile**, the UI Acceptance Gate (from `UI_ACCEPTANCE_GATE.md`) must include these additional checks:

| Check | Required Result |
|---|---|
| Touch targets ≥ 48x48 dp | PASS |
| All states handled (loading, empty, error, offline, permission) | PASS |
| Safe areas respected | PASS |
| Keyboard behavior tested | PASS |
| Platform patterns applied (Material vs Cupertino) | PASS / N/A |
| RTL/LTR tested | PASS / N/A |
| Portrait and landscape tested | PASS / N/A |
| Font scaling tested | PASS |
| Screen reader accessible | PASS |
| At least one real device test per target platform | PASS / N/A |

---

## 18. References

| Topic | Source |
|---|---|
| Material Design 3 | https://m3.material.io |
| Apple Human Interface Guidelines | https://developer.apple.com/design/human-interface-guidelines |
| Flutter platform adaptation | https://docs.flutter.dev/platform-integration |
| Flutter accessibility | https://docs.flutter.dev/accessibility |
| Tera Design Review Standards | `tera-system/design-system/DESIGN_REVIEW_STANDARDS.md` |
| Tera UI Acceptance Gate | `tera-system/design-system/UI_ACCEPTANCE_GATE.md` |
| Tera Flutter Profile | `tera-system/profiles/flutter-mobile.md` |

## 19. Version

- **Version:** 1.1.0
- **Last Updated:** 2026-07-16
- **Updated Sections (v1.0 to v1.1):** §3 (Material You/Dynamic Color), §9 (iOS Photos limited state), §12 (Arabic typography, Arabic plural forms)
- **Last Updated By:** Gap Analysis Closure (SCP-096 Phase 2)
- **Maintainer:** TeraSystemEvolutionAgent (حارس)
