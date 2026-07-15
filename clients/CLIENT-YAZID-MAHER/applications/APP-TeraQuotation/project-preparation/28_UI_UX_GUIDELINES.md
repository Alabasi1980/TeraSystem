# 28_UI_UX_GUIDELINES.md — TeraQuotation

> **Design Rules File**
> **Project:** TeraQuotation — نظام إدارة عروض أسعار قطع السيارات
> **Platform:** WPF (.NET 8) Desktop — Arabic RTL
> **Effective Date:** 2026-07-14
> **Status:** ✅ Approved by Design Reviewer (PASS_WITH_REQUIRED_FIXES — all applied)

---

## 1. Design Source Decision

| Field | Value |
|-------|-------|
| **Design Source Mode** | `HYBRID` |
| **Selected Sources** | `USER_PROVIDED_REFERENCE` (Majed screenshots + feedback) + `INTERNAL_TERA_KIT` (calm Arabic RTL business desktop) |
| **Why Selected** | The app is a desktop business tool. No Figma file. Client feedback and screenshots fully capture the UX pain points. |
| **What Will Be Used** | Current app workflow/domain; Arabic RTL; calm professional desktop layout; clear labels, cards, validation, empty states |
| **What Will NOT Be Used** | Current confusing table-first QuotationForm; random hardcoded colors; emoji-heavy action buttons; unlabeled input fields |
| **Final Executable File** | This file — `28_UI_UX_GUIDELINES.md` |

---

## 2. Product UX Principles

1. **User is non-technical.** Every screen must explain itself. If a user needs to guess, the design fails.
2. **Speed matters, but not at clarity's expense.** Fast entry is important, but empty fields without labels or purpose are unacceptable.
3. **QuotationForm is the center of the app.** It must be the most polished, clearest, and fastest screen.
4. **Every field has a label.** No exceptions. Placeholders are not replacements for labels.
5. **No confusing buttons.** Every button's purpose must be obvious from its text alone.
6. **No empty tables or lists without Empty State.** Every data view must show helpful content even when empty.
7. **Every action has feedback.** Save, delete, print, export — all must show success or error toasts.
8. **The app must feel "alive."** Loading states, status badges, toast messages, unsaved changes indicator.

---

## 3. Design Tokens

### 3.1 Colors

| Token | Hex | Usage |
|-------|-----|-------|
| `AppBackground` | `#F6F8FB` | Main window background |
| `Surface` | `#FFFFFF` | Card / panel background |
| `Primary` | `#2563EB` | Primary buttons, active states |
| `PrimaryHover` | `#1D4ED8` | Primary button hover |
| `TextPrimary` | `#111827` | Main text |
| `TextSecondary` | `#6B7280` | Helper text, secondary info |
| `TextMuted` | `#9CA3AF` | Disabled text, placeholders |
| `Border` | `#E5E7EB` | Card borders, field borders |
| `BorderLight` | `#F3F4F6` | Subtle dividers |
| `Success` | `#16A34A` | Success states, "completed" badges |
| `SuccessBg` | `#DCFCE7` | Success card background |
| `Warning` | `#D97706` | Warning states, "needs attention" badges |
| `WarningBg` | `#FEF3C7` | Warning card background |
| `Danger` | `#DC2626` | Delete buttons, error messages |
| `DangerBg` | `#FEE2E2` | Error card background |
| `Info` | `#2563EB` | Information badges |
| `InfoBg` | `#DBEAFE` | Information card background |
| `BestPrice` | `#16A34A` | Best price highlight |
| `BestPriceBg` | `#F0FDF4` | Best price card background |

### 3.2 Typography

| Token | Value | Usage |
|-------|-------|-------|
| `FontFamily` | `Segoe UI` | Default system font for WPF |
| `PageTitle` | 22px Bold | Screen/page title |
| `SectionTitle` | 16px SemiBold | Card/section headers |
| `Body` | 14px Regular | Main content, field values |
| `Label` | 13px Regular | Field labels |
| `Helper` | 12px Regular | Helper text, subtitles |
| `Badge` | 12px SemiBold | Status badges |

### 3.3 Spacing

| Token | px | Usage |
|-------|----|-------|
| `Space2` | 2 | Tiny gaps |
| `Space4` | 4 | Tight spacing |
| `Space8` | 8 | Between related items |
| `Space12` | 12 | Between sections/cards |
| `Space16` | 16 | Default padding |
| `Space20` | 20 | Card padding |
| `Space24` | 24 | Page margins |
| `Space32` | 32 | Large section gaps |

### 3.4 Borders & Radius

| Token | Value | Usage |
|-------|-------|-------|
| `RadiusCard` | 10 | Card corners |
| `RadiusButton` | 8 | Button corners |
| `RadiusField` | 6 | Input field corners |
| `RadiusBadge` | 4 | Badge corners |
| `BorderDefault` | 1px `#E5E7EB` | Default border |
| `BorderField` | 1px `#D1D5DB` | Input field border |

### 3.5 Shadows

| Token | Value | Usage |
|-------|-------|-------|
| `ShadowCard` | `0 2 8 rgba(0,0,0,0.06)` | Card shadow (WPF: DropShadowEffect) |
| `ShadowPopup` | `0 4 12 rgba(0,0,0,0.1)` | Popup/dialog shadow |

---

## 4. Layout Patterns

### 4.1 MainWindow Shell

- RTL layout with optional sidebar (right side) or top navigation
- Clean frame-based content switching
- No cluttered toolbars

### 4.2 Page Structure

Every page follows:
```
┌─ Page Header ──────────────────────────────┐
│  Title        [Action Button(s)]            │
├─────────────────────────────────────────────┤
│  Content Area                               │
│  (Cards / Sections / Grids)                 │
└─────────────────────────────────────────────┘
```

### 4.3 QuotationForm — Master-Detail Layout

At 1100px+:
```
┌─ Header (quote info + status + save time) ───────────────┐
├─ Guidance Strip (single line hint) ──────────────────────┤
├──────────┬──────────────────────────┬────────────────────┤
│ Right    │ Center (Main)            │ Left (Collapsible) │
│ 260px    │ ~540px                   │ 300px              │
│          │                          │                    │
│ Details  │ Materials List           │ Pricing Panel      │
│ +Suppliers│ +Add Material Form       │ (selected material)│
└──────────┴──────────────────────────┴────────────────────┘
├─ Sticky Action Bar (save / print / PDF / Outlook) ───────┤
```

If < 980px: Left pricing panel becomes a collapsible drawer or moves below.

### 4.4 Settings — Tabbed Card Layout

Each tab uses card-based layout, not raw tables.

---

## 5. Component Rules

### 5.1 Buttons

| Variant | Style | Usage |
|---------|-------|-------|
| **Primary** | `Background=#2563EB, Foreground=White, Height=40, Radius=8` | Main action (save, create) |
| **Secondary** | `Background=White, Border=#D1D5DB, Text=#374151, Height=38` | Secondary actions |
| **Danger** | `Background=#DC2626, Foreground=White, Height=38, Radius=8` | Delete only |
| **Ghost** | `Background=Transparent, Foreground=#2563EB` | Simple inline actions |
| **Disabled** | `Opacity=0.5, Tooltip=reason` | Must show cause of disable |

- One primary button per section maximum.
- Action bar: no more than 2-3 distinct button colors.

### 5.2 Text Fields

- Every field must have a visible Label above it.
- Placeholder text is for examples only, not label replacement.
- Helper text (12px, `#6B7280`) appears below field only when needed.
- Validation error (12px, `#DC2626`) appears below field.
- Height: 36-40px.
- Numeric/currency fields: proper alignment, LTR within RTL.
- Disabled fields: `Background=#F9FAFB`, `Foreground=#9CA3AF`.

### 5.3 Cards

- Background: White
- Border: `1px #E5E7EB`
- Radius: 10
- Shadow: minimal (DropShadowEffect with BlurRadius=8, Opacity=0.06)
- Padding: 16-20px inside
- Title at top of card: 16px SemiBold

### 5.4 Status Badges

| Status | Background | Text |
|--------|-----------|------|
| مسودة | `#F3F4F6` | `#6B7280` |
| محفوظ | `#DBEAFE` | `#2563EB` |
| ناقص أسعار | `#FEF3C7` | `#D97706` |
| مكتمل | `#DCFCE7` | `#16A34A` |
| أفضل سعر | `#F0FDF4` | `#16A34A` |
| خطأ | `#FEE2E2` | `#DC2626` |

Badge style: Radius=4, Padding=4-8px horizontal, 2-4px vertical.

### 5.5 Empty State

- Icon/illustration (optional, simple)
- Title: "لا توجد مواد بعد" style message
- Description: helpful next step
- Action button: primary call-to-action
- Centered in the container

### 5.6 Toast / Message Bar

- Duration: 3-4 seconds for success, 5-6 seconds for error
- Success: Green background, white text
- Error: Red background, white text
- Warning: Amber background, dark text
- Position: Top-right (RTL) or bottom-center
- Must not block user interaction.

### 5.7 Confirmation Dialog

- Title: short and clear
- Message: specific about what will happen
- Buttons: "تأكيد" + "إلغاء" for destructive, "حفظ" + "إلغاء" for save
- Never just "Yes" / "No" — be explicit.

### 5.8 Tables / DataGrids

- Header row: fixed, bold text
- Alternating row background: `#F9FAFB`
- Row height: 40-44px
- Hover effect on rows
- Sort on primary columns
- No gridlines unless necessary
- Empty state instead of empty table

---

## 6. Screen-Specific Rules

### 6.1 Login (S1)

- Centered card, max 380px wide
- App title + subtitle
- Single password field with label
- "دخول" button full-width
- Error message below field
- Loading state on button

### 6.2 Settings (S2)

- 4 tabs: الموردين, القطع, التوقيعات, الترويسة
- Each tab: card-based layout with clear labels
- Add form at top of each tab with labels
- Table/list below for existing data
- Empty state when no data
- Save button for letterhead tab only (other tabs save on edit)

### 6.3 QuotationForm (S3) — Master-Detail Workspace

**Header:**
- "عرض سعر جديد / تعديل عرض رقم XXX"
- Quote number, date, status badge, last save indicator
- Back button: "← رجوع للقائمة"

**Guidance Strip:**
- Single line: "أضف الموردين والمواد، ثم أدخل أسعار كل مورد"
- Updates based on state: "تمت إضافة الموردين، أضف المواد المطلوبة"

**Right Panel — Details + Suppliers:**
- Quote details card (number, date, status, summary)
- Suppliers section with cards
- Each supplier card shows: name, pricing status (مكتمل / ناقص / لم يبدأ)
- Add supplier button: "+ إضافة مورد"
- Empty state: "لا يوجد موردون بعد. أضف الموردين الذين تريد مقارنة أسعارهم."

**Center Panel — Materials:**
- Add material form: اسم المادة (with search), الكمية, الوحدة
- Add button: "+ إضافة مادة"
- Materials list: each item shows name, quantity, unit, pricing status
- Click/tap material to open pricing panel
- Empty state: "لا توجد مواد بعد. أضف أول مادة ليبدأ عرض السعر."
- Search/filter in materials

**Left Panel — Pricing:**
- Visible only when a material is selected
- Title: "تسعير: [اسم المادة]"
- Quantity reminder
- For each supplier: brand/type dropdown (أصلي/تجاري/بديل), price per unit field, optional note
- Best price highlight (green badge)
- Empty state: "اختر مادة من القائمة لتسعيرها"

**Sticky Action Bar:**
- Save (primary)
- Print without prices (secondary)
- Print final (secondary, disabled until prices complete)
- Export PDF (secondary, disabled until saved)
- Send Outlook (secondary, disabled until saved)
- Disabled buttons show tooltip with reason

**Feedback:**
- Toast: "تم حفظ العرض بنجاح"
- Toast: "تمت إضافة المادة"
- Unsaved changes warning on exit
- "لا يمكن الطباعة النهائية — توجد مواد بدون أسعار"

### 6.4 QuotationList (S4)

- Header: "قائمة عروض الأسعار" + "عرض جديد" button
- Filter bar: search text, status filter, date range
- Results as cards or clean table:
  - Quote number, date, description, status badge, action (open)
- Empty state: "لا توجد عروض أسعار بعد. اضغط 'عرض جديد' لإنشاء أول عرض."
- Pagination if > 20 results
- Search real-time

### 6.5 Reports (S5)

- Header: "التقارير"
- Report selection as cards with icons, not colored buttons
- Each report card shows name + short description
- Active report highlighted
- Results area: clean table with sort
- Empty state: "اختر تقريراً لعرض النتائج"
- Print and PDF buttons on each report

---

## 7. Arabic RTL Rules

1. All XAML pages/shells use `FlowDirection="RightToLeft"`
2. Numeric fields (price, quantity, phone): handle LTR within RTL context
3. No English enum names visible to user — translate all status labels
4. Arabic labels must be human-friendly, not transliterated technical terms
5. Avoid justified Arabic text in small containers
6. Directional icons (arrows, chevrons) must be mirrored for RTL
7. Date format: dd/MM/yyyy or "14 يوليو 2026"
8. Currency: "دينار" or "ريال" suffix, not symbol alone

---

## 8. Content Guidelines — Arabic UI Copy

### Approved Labels & Buttons

| Arabic | Usage |
|--------|-------|
| عرض سعر جديد | Page title |
| قائمة عروض الأسعار | Page title |
| الإعدادات | Page title |
| التقارير | Page title |
| حفظ العرض | Action button |
| طباعة بدون أسعار | Action button |
| طباعة نهائية | Action button |
| تصدير PDF | Action button |
| إرسال عبر Outlook | Action button |
| + إضافة مورد | Action |
| + إضافة مادة | Action |
| إضافة أول مورد | Empty state CTA |
| إضافة أول مادة | Empty state CTA |
| اسم المادة | Field label |
| الكمية | Field label |
| الوحدة | Field label |
| اسم المورد | Field label |
| نوع / ماركة القطعة | Field label (brand) |
| السعر للوحدة | Field label |
| ملاحظة اختيارية | Field label |
| ← رجوع للقائمة | Navigation |
| تغييرات غير محفوظة | Warning |
| تم الحفظ بنجاح | Success toast |
| حدث خطأ غير متوقع | Error toast |
| لديك تغييرات غير محفوظة. هل تريد المغادرة؟ | Exit warning |
| لا يمكن الطباعة النهائية — توجد مواد بدون أسعار | Validation |
| أضف مادة واحدة على الأقل للطباعة | Disabled reason |
| احفظ العرض أولاً | Disabled reason |

### Forbidden Labels

| Forbidden | Reason |
|-----------|--------|
| "إضافة ص فارغ" | Meaningless |
| `UpdatedWithPrices` | Raw enum, not translated |
| "إجراء" | Vague column header |
| User ID display | Technical |
| "الصفحة الرئيسية" for Settings | Misleading |
| Any English status in UI | Must be translated |

---

## 9. UI Acceptance Gate Reference

Every UI implementation task must pass `tera-system/design-system/UI_ACCEPTANCE_GATE.md` before acceptance.

### Minimum Requirements

- [ ] Design source mode recorded in task file
- [ ] This file (`28_UI_UX_GUIDELINES.md`) used as primary design source
- [ ] No invented colors, spacing, typography
- [ ] Component rules followed
- [ ] Layout pattern followed
- [ ] RTL rules applied
- [ ] No forbidden labels
- [ ] Design gaps recorded, not guessed
- [ ] Empty states present
- [ ] Loading states present
- [ ] Buttons have visible text and clear purpose
- [ ] Disabled states with explanations
- [ ] Fields have labels

---

*This file is the official visual design authority for APP-TeraQuotation. All UI implementation must reference this file first. Any design gap must be recorded as a deferred item, not guessed during implementation.*
