# 07_SCREENS_AND_UI_STRUCTURE.md — TeraQuotation

> **Screens and UI Structure Document**
> **المشروع:** TeraQuotation — نظام إدارة عروض أسعار قطع السيارات
> **التقنية:** WPF (.NET 8) + CommunityToolkit.Mvvm + SQLite
> **واجهة:** RTL بالكامل — اللغة العربية
> **تاريخ الإصدار:** 2026-07-13
> **المصادر:** APPLICATION_BLUEPRINT.md ✅ + 08_TECHNICAL_ARCHITECTURE.md ✅ + 06_DATA_MODEL_PREPARATION.md ✅ + 28_UI_UX_GUIDELINES.md (قالب) ✅

---

## Lifecycle Header

| الحقل | القيمة |
|:------|:-------|
| **Document State** | **Module Baseline Approved** |
| **Baseline Module** | TeraQuotation (Full Application) |
| **Current State** | Module Baseline Approved |
| **Owner** | Software Designer Agent (مُصمم) |
| **Last Review** | 2026-07-13 |
| **Expiry** | End of Project Delivery |

---

## قائمة المحتويات

1. [نظرة عامة — هيكل التنقل](#1-نظرة-عامة--هيكل-التنقل)
2. [الشاشة الأولى — S1: Login Screen](#2-الشاشة-الأولى--s1-login-screen)
3. [الشاشة الثانية — S2: Settings Screen](#3-الشاشة-الثانية--s2-settings-screen)
4. [الشاشة الثالثة — S3: Quotation Form](#4-الشاشة-الثالثة--s3-quotation-form)
5. [الشاشة الرابعة — S4: Quotation List](#5-الشاشة-الرابعة--s4-quotation-list)
6. [الشاشة الخامسة — S5: Reports Screen](#6-الشاشة-الخامسة--s5-reports-screen)
7. [المكونات المشتركة (Shared Components & Converters)](#7-المكونات-المشتركة-shared-components--converters)
8. [قواعد عامة (Cross-Cutting Rules)](#8-قواعد-عامة-cross-cutting-rules)
9. [Gaps and Recommendations](#9-gaps-and-recommendations)

---

## 1. نظرة عامة — هيكل التنقل

### 1.1 هيكل الشاشات

```
┌──────────────────────────────────────────────────────────────────┐
│                     MainWindow Shell                              │
│  ┌──────────┐  ┌──────────────────────────────────────────────┐  │
│  │ Sidebar   │  │              Content Frame                    │  │
│  │ (قائمة    │  │                                               │  │
│  │  تنقل)    │  │  [يُحمّل هنا محتوى الصفحة الحالية]             │  │
│  │           │  │                                               │  │
│  │ 🏠 رئيسي  │  │  الصفحات:                                      │  │
│  │ ⚙️ إعدادات│  │  - SettingsView (TabControl داخلي)             │  │
│  │ 📝 عرض    │  │  - QuotationFormView                           │  │
│  │   جديد   │  │  - QuotationListView                            │  │
│  │ 📂 عروض   │  │  - ReportsView                                 │  │
│  │ 📊 تقارير │  │                                               │  │
│  │           │  │                                               │  │
│  │           │  │                                               │  │
│  │ [تسجيل    │  │                                               │  │
│  │  خروج]    │  │                                               │  │
│  └──────────┘  └──────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────────────┘
```

### 1.2 تدفق التنقل الأساسي

```
[تشغيل التطبيق]
      │
      ▼
┌──────────────┐
│  LoginView   │ ← نافذة Modal منفصلة (Window)
│  (كلمة مرور)  │
└──────┬───────┘
       │ (نجاح)
       ▼
┌──────────────────────────────────────┐
│         MainWindow                   │
│  ┌────────────────────────────────┐  │
│  │  شريط جانبي + Content Frame    │  │
│  └────────────────────────────────┘  │
└──────────────────────────────────────┘
       │
       ├── ⚙️ ← SettingsView (4 Tabs)
       ├── 📝 ← QuotationFormView (جديد)
       ├── 📂 ← QuotationListView
       │         └── فتح عرض ← QuotationFormView (موجود)
       └── 📊 ← ReportsView
```

### 1.3 أنواع النوافذ

| الشاشة | النوع | الوصف |
|:-------|:-----|:-------|
| **LoginView** | `Window` (Modal Dialog) | نافذة منفصلة، تظهر قبل MainWindow |
| **MainWindow** | `Window` (Shell) | النافذة الرئيسية بعد تسجيل الدخول |
| **SettingsView** | `Page` → يُحمّل في Frame | TabControl داخلي بـ 4 Tabs |
| **QuotationFormView** | `Page` → يُحمّل في Frame | استمارة إنشاء/تعديل عرض |
| **QuotationListView** | `Page` → يُحمّل في Frame | جدول العروض مع بحث وتصفية |
| **ReportsView** | `Page` → يُحمّل في Frame | أزرار تقارير + منطقة عرض ديناميكية |

### 1.4 Sidebar — عناصر القائمة الجانبية في MainWindow

| العنصر | الأيقونة | الأمر (Command) | الوصف |
|:-------|:--------:|:----------------|:-------|
| رئيسي | 🏠 | `NavigateToHomeCommand` | يعيد التوجيه إلى الصفحة الافتراضية (آخر شاشة مفتوحة أو QuotationForm) |
| إعدادات | ⚙️ | `NavigateToSettingsCommand` | يفتح SettingsView |
| عرض سعر جديد | 📝 | `NavigateToNewQuotationCommand` | يفتح QuotationFormView في وضع إنشاء جديد |
| قائمة العروض | 📂 | `NavigateToQuotationListCommand` | يفتح QuotationListView |
| تقارير | 📊 | `NavigateToReportsCommand` | يفتح ReportsView |
| تسجيل خروج | 🚪 | `LogoutCommand` | يعود إلى LoginView |

---

## 2. الشاشة الأولى — S1: Login Screen

### 2.1 الغرض

شاشة دخول بسيطة بكلمة مرور فقط (لا اسم مستخدم لأن التطبيق لمستخدم واحد). عند أول تشغيل، تظهر شاشة الإعداد الأولي (First Time Setup).

### 2.2 العناصر (Controls)

| # | العنصر | النوع WPF | الخصائص الرئيسية | Binding Path |
|:-:|:-------|:----------|:-----------------|:-------------|
| 1 | **Title** | `TextBlock` | نص: "TeraQuotation"، FontSize=24, FontWeight=Bold, HorizontalAlignment=Center | — |
| 2 | **Subtitle** | `TextBlock` | نص: "نظام إدارة عروض أسعار قطع السيارات"، FontSize=14, Opacity=0.7, HorizontalAlignment=Center | — |
| 3 | **Icon/Lock** | `TextBlock` (أو Image) | رمز 🔒، FontSize=48 | — |
| 4 | **Password Label** | `TextBlock` | نص: "كلمة المرور:" | — |
| 5 | **Password Box** | `PasswordBox` | `PasswordChar="●"`, `MaxLength=50`, `FlowDirection=RightToLeft` | `{Binding Password, Mode=OneWayToSource}` عبر Behavior |
| 6 | **Login Button** | `Button` | نص: "دخول"، `IsDefault=True`, `Command={Binding LoginCommand}`, `Width=150` | `Command="{Binding LoginCommand}"` |
| 7 | **Error Message** | `TextBlock` | `Foreground=Red`, `Visibility=Collapsed`, يظهر عند فشل الدخول | `Visibility="{Binding HasError, Converter=...}"`, `Text="{Binding ErrorMessage}"` |
| 8 | **Loading Indicator** | `ProgressRing` (أو `TextBlock`) | يظهر أثناء التحقق من كلمة المرور | `Visibility="{Binding IsLoading, Converter=...}"` |

### 2.3 حالات الشاشة (Screen States)

| الحالة | الوصف | المظهر |
|:-------|:------|:-------|
| **Idle** | في انتظار إدخال كلمة المرور | PasswordBox فارغ، زر الدخول مفعّل |
| **Loading** | جارٍ التحقق من كلمة المرور | PasswordBox معطل، زر الدخول معطل، يظهر مؤشر التحميل |
| **Error** | كلمة المرور خاطئة | رسالة خطأ حمراء، PasswordBox يُمسح، الزر يعود للتفعيل |
| **FirstTimeSetup** | لا يوجد مستخدم بعد | تظهر شاشة إنشاء كلمة المرور الأولية (حقلان: كلمة مرور جديدة + تأكيد) |

### 2.4 الأحداث والأوامر (Commands/Triggers)

| الأمر | المشغل (Trigger) | الإجراء |
|:------|:-----------------|:--------|
| `LoginCommand` | ضغط زر "دخول" أو Enter | 1. يقرأ كلمة المرور من PasswordBox<br>2. يستدعي `AuthenticationService.LoginAsync("admin", password)`<br>3. عند النجاح: يفتح MainWindow ويغلق LoginView<br>4. عند الفشل: يعرض رسالة الخطأ |
| `SetupFirstUserCommand` | ضغط زر في وضع FirstTimeSetup | 1. يتحقق من تطابق كلمتي المرور<br>2. يستدعي `AuthenticationService.SetupFirstUserAsync(password)`<br>3. عند النجاح: يفتح MainWindow |
| `SkipSetupCommand` | ضغط زر "تخطي" | يغلق التطبيق (اختياري — يمكن إلغاؤه) |

### 2.5 التحقق من الإدخال (Validation)

| العنصر | القاعدة | رسالة الخطأ |
|:-------|:--------|:------------|
| **PasswordBox** | ≥ 4 أحرف | "كلمة المرور يجب أن تكون 4 أحرف على الأقل" |
| **Confirm Password** (في FirstTimeSetup) | يطابق كلمة المرور الجديدة | "كلمة المرور غير متطابقة" |

### 2.6 الخدمات المستخدمة

| الخدمة | الدالة المستخدمة |
|:-------|:-----------------|
| `IAuthenticationService` | `LoginAsync(username, password)` |
| `IAuthenticationService` | `SetupFirstUserAsync(password)` |
| `IAuthenticationService` | `IsFirstTimeSetup()` |

### 2.7 XAML توضيحي (جزئي)

```xml
<!-- LoginView.xaml — RTL كامل -->
<UserControl x:Class="TeraQuotation.Views.LoginView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:vm="clr-namespace:TeraQuotation.ViewModels"
             FlowDirection="RightToLeft">
    <Grid Background="{StaticResource SurfaceBrush}">
        <StackPanel VerticalAlignment="Center" HorizontalAlignment="Center" Width="350">
            <!-- الشعار والعنوان -->
            <TextBlock Text="TeraQuotation" FontSize="28" FontWeight="Bold"
                       HorizontalAlignment="Center" />
            <TextBlock Text="نظام إدارة عروض أسعار قطع السيارات"
                       FontSize="14" Opacity="0.6"
                       HorizontalAlignment="Center" Margin="0,8,0,24"/>

            <!-- حقل كلمة المرور -->
            <TextBlock Text="كلمة المرور:" Margin="0,0,0,4"/>
            <PasswordBox x:Name="PasswordBox" PasswordChar="●"
                         MaxLength="50" />

            <!-- زر الدخول -->
            <Button Content="دخول" Command="{Binding LoginCommand}"
                    CommandParameter="{Binding ElementName=PasswordBox, Path=Password}"
                    Margin="0,16,0,0" Height="40" Width="150"
                    HorizontalAlignment="Center"/>

            <!-- رسالة الخطأ -->
            <TextBlock Text="{Binding ErrorMessage}"
                       Foreground="{StaticResource DangerBrush}"
                       Visibility="{Binding HasError, Converter={StaticResource BoolToVis}}"
                       Margin="0,8,0,0" HorizontalAlignment="Center"/>
        </StackPanel>
    </Grid>
</UserControl>
```

---

## 3. الشاشة الثانية — S2: Settings Screen

### 3.1 الغرض

إدارة البيانات المرجعية للتطبيق: الموردين، قطع الكتالوج، أسماء التوقيعات، وإعدادات الترويسة. تحتوي على 4 أقسام داخل `TabControl`.

### 3.2 هيكل الشاشة — Layout

```
┌─────────────────────────────────────────────────────────────────┐
│  ⚙️ الإعدادات                                                    │
│  ┌─────────────────────────────────────────────────────────────┐ │
│  │  [الموردين]  [القطع]  [التوقيعات]  [الترويسة]               │ │
│  ├─────────────────────────────────────────────────────────────┤ │
│  │                                                             │ │
│  │  (محتوى التبويب المختار — يُحمّل ديناميكياً)                  │ │
│  │                                                             │ │
│  └─────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

### 3.3 Tab 1: Suppliers (الموردين)

#### 3.3.1 العناصر

| # | العنصر | النوع WPF | Binding Path | ملاحظات |
|:-:|:-------|:----------|:-------------|:--------|
| 1 | **Suppliers DataGrid** | `DataGrid` | `ItemsSource="{Binding Suppliers}"` | `AutoGenerateColumns=False`, `IsReadOnly=False`, `CanUserAddRows=False` |
| 2 | Column: الاسم | `DataGridTextColumn` | `Binding={Binding Name}` | Required, Max 200 حرف |
| 3 | Column: معلومات الاتصال | `DataGridTextColumn` | `Binding={Binding ContactInfo}` | Optional, Max 500 حرف |
| 4 | Column: ملاحظات | `DataGridTextColumn` | `Binding={Binding Notes}` | Optional, Max 1000 حرف |
| 5 | **Add Button** | `Button` | `Command="{Binding AddSupplierCommand}"` | يضيف صفاً جديداً للـ DataGrid |
| 6 | **Delete Button** | `Button` | `Command="{Binding DeleteSupplierCommand}"` | يحذف الصف المحدد مع تأكيد |
| 7 | **Save Button** | `Button` | `Command="{Binding SaveSuppliersCommand}"` | يحفظ كل التغييرات دفعة واحدة |

#### 3.3.2 الأوامر

| الأمر | الوصف |
|:------|:-------|
| `AddSupplierCommand` | يضيف صفاً جديداً فارغاً إلى DataGrid (Suppliers ObservableCollection) |
| `DeleteSupplierCommand` | يأخذ `SelectedItem` من DataGrid، يعرض تأكيد "هل تريد حذف المورد [Name]؟"، عند التأكيد يستدعي `SettingsService.DeleteSupplierAsync(id)` |
| `SaveSuppliersCommand` | يحفظ جميع التغييرات عبر `SettingsService.AddSupplierAsync` / `UpdateSupplierAsync` لكل صف معدّل |
| `CellEditEnded` (حدث) | عند تعديل خلية، يميّز الصف كـ "معدّل" للحفظ اللاحق |

#### 3.3.3 الخدمات

| الخدمة | الدوال |
|:-------|:-------|
| `ISettingsService` | `GetAllSuppliersAsync()`, `AddSupplierAsync(Supplier)`, `UpdateSupplierAsync(Supplier)`, `DeleteSupplierAsync(int)` |

### 3.4 Tab 2: Items (قطع الكتالوج)

#### 3.4.1 العناصر

| # | العنصر | النوع WPF | Binding Path | ملاحظات |
|:-:|:-------|:----------|:-------------|:--------|
| 1 | **Search Box** | `TextBox` | `Text="{Binding ItemSearchText, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"` | مع تأخير 300ms للبحث |
| 2 | **Items DataGrid** | `DataGrid` | `ItemsSource="{Binding Items}"` | `AutoGenerateColumns=False` |
| 3 | Column: الاسم | `DataGridTextColumn` | `Binding={Binding Name}` | Required, Max 200 حرف |
| 4 | Column: الوصف | `DataGridTextColumn` | `Binding={Binding Description}` | Optional, Max 1000 حرف |
| 5 | Column: ملاحظات | `DataGridTextColumn` | `Binding={Binding Notes}` | Optional, Max 1000 حرف |
| 6 | **Add Button** | `Button` | `Command="{Binding AddItemCommand}"` | — |
| 7 | **Delete Button** | `Button` | `Command="{Binding DeleteItemCommand}"` | مع تأكيد وتحقق من عدم استخدام القطعة |
| 8 | **Save Button** | `Button` | `Command="{Binding SaveItemsCommand}"` | — |

#### 3.4.2 الأوامر

| الأمر | الوصف |
|:------|:-------|
| `SearchItemsCommand` (أو `PropertyChanged` على `ItemSearchText`) | بعد 300ms من آخر حرف، يستدعي `SettingsService.GetAllItemsAsync(searchText)` ويحدّث `Items` |
| `AddItemCommand` | يضيف صفاً جديداً |
| `DeleteItemCommand` | يحاول الحذف: إذا القطعة مستخدمة في عروض سابقة ← رسالة "لا يمكن حذف هذه القطعة لأنها مستخدمة في [N] عرض سعر" |
| `SaveItemsCommand` | يحفظ التغييرات |

#### 3.4.3 الخدمات

| الخدمة | الدوال |
|:-------|:-------|
| `ISettingsService` | `GetAllItemsAsync(search)`, `AddItemAsync(Item)`, `UpdateItemAsync(Item)`, `DeleteItemAsync(int)`, `IsItemUsedInAnyQuotationAsync(int)` |

### 3.5 Tab 3: Signatures (التوقيعات)

#### 3.5.1 العناصر

| # | العنصر | النوع WPF | Binding Path | ملاحظات |
|:-:|:-------|:----------|:-------------|:--------|
| 1 | **Signatures ListBox/DataGrid** | `ListBox` أو `DataGrid` | `ItemsSource="{Binding Signatures}"` | قائمة بسيطة: اسم التوقيع + ترتيب |
| 2 | Column: الاسم | `DataGridTextColumn` | `Binding={Binding Name}` | Required |
| 3 | Column: الترتيب | `DataGridTextColumn` | `Binding={Binding OrderIndex}` | رقم صحيح |
| 4 | **Move Up Button** | `Button` | `Command="{Binding MoveUpCommand}"` | يرفع التوقيع المحدد في الترتيب |
| 5 | **Move Down Button** | `Button` | `Command="{Binding MoveDownCommand}"` | يخفض التوقيع المحدد في الترتيب |
| 6 | **Add Button** | `Button` | `Command="{Binding AddSignatureCommand}"` | — |
| 7 | **Delete Button** | `Button` | `Command="{Binding DeleteSignatureCommand}"` | مع تأكيد |

#### 3.5.2 الأوامر

| الأمر | الوصف |
|:------|:-------|
| `AddSignatureCommand` | يضيف توقيعاً جديداً مع `OrderIndex` تلقائي |
| `DeleteSignatureCommand` | يحذف التوقيع المحدد ويعيد ترتيب الباقي |
| `MoveUpCommand` | يبدّل `OrderIndex` مع التوقيع الذي يسبقه |
| `MoveDownCommand` | يبدّل `OrderIndex` مع التوقيع الذي يليه |
| `SaveSignaturesCommand` | يحفظ جميع التغييرات |

#### 3.5.3 الخدمات

| الخدمة | الدوال |
|:-------|:-------|
| `ISettingsService` | `GetAllSignaturesAsync()`, `AddSignatureAsync(Signature)`, `DeleteSignatureAsync(int)`, `UpdateSignatureAsync(Signature)` |

### 3.6 Tab 4: Letterhead (الترويسة)

#### 3.6.1 العناصر

| # | العنصر | النوع WPF | Binding Path | ملاحظات |
|:-:|:-------|:----------|:-------------|:--------|
| 1 | **Logo Preview** | `Image` | `Source="{Binding LogoImageSource}"` | معاينة الشعار المختار (150×150) |
| 2 | **Select Logo Button** | `Button` | `Command="{Binding SelectLogoCommand}"` | يفتح `OpenFileDialog` (صور: png, jpg, bmp) |
| 3 | **Remove Logo Button** | `Button` | `Command="{Binding RemoveLogoCommand}"` | يزيل الشعار |
| 4 | **Company Name** | `TextBox` | `Text="{Binding CompanyName, Mode=TwoWay}"` | Max 200 حرف |
| 5 | **Company Address** | `TextBox` | `Text="{Binding CompanyAddress, Mode=TwoWay}"` | MultiLine, Max 500 حرف |
| 6 | **Company Phone** | `TextBox` | `Text="{Binding CompanyPhone, Mode=TwoWay}"` | Max 50 حرف |
| 7 | **Company Email** | `TextBox` | `Text="{Binding CompanyEmail, Mode=TwoWay}"` | Max 100 حرف |
| 8 | **Save Button** | `Button` | `Command="{Binding SaveLetterheadCommand}"` | يحفظ عبر `SettingsService.SaveLetterheadDataAsync()` |
| 9 | **Print Preview Button** | `Button` | `Command="{Binding PreviewLetterheadCommand}"` | يعرض معاينة للترويسة |

#### 3.6.2 الأوامر

| الأمر | الوصف |
|:------|:-------|
| `SelectLogoCommand` | يفتح `Microsoft.Win32.OpenFileDialog` (Filter: Images \| *.png;*.jpg;*.jpeg;*.bmp). ينسخ الصورة إلى مجلد التطبيق ويحفظ المسار في `Setting` |
| `RemoveLogoCommand` | يمسح مسار الشعار من `Setting` |
| `SaveLetterheadCommand` | يحفظ جميع بيانات الترويسة عبر `SettingsService.SaveLetterheadDataAsync(LetterheadDto)` |
| `PreviewLetterheadCommand` | يُظهر `PrintDialog` مع `FixedDocument` يحتوي الترويسة فقط (اختياري) |

#### 3.6.3 الخدمات

| الخدمة | الدوال |
|:-------|:-------|
| `ISettingsService` | `GetLetterheadDataAsync()`, `SaveLetterheadDataAsync(LetterheadDto)` |
| `ISettingsService` | `GetSettingAsync(key)`, `SetSettingAsync(key, value)` |

### 3.7 حالات Settings Screen

| الحالة | السلوك |
|:-------|:-------|
| **Loading** | `ProgressBar` يظهر أثناء تحميل البيانات من قاعدة البيانات عند فتح الشاشة |
| **Loaded** | جميع Tabs تعرض البيانات المحملة |
| **Empty (Suppliers)** | DataGrid فارغ مع رسالة "لا يوجد موردين — اضف مورداً جديداً" |
| **Empty (Items)** | DataGrid فارغ مع رسالة "لا توجد قطع في الكتالوج — أضف قطعة جديدة" |
| **Empty (Signatures)** | قائمة فارغة مع رسالة "لا توجد توقيعات — أضف توقيعاً" |
| **Error** | رسالة خطأ إذا فشل تحميل البيانات أو حفظها |
| **Saving** | `IsSaving=true` يعطل الأزرار ويظهر "جارٍ الحفظ..." |

---

## 4. الشاشة الثالثة — S3: Quotation Form

### 4.1 الغرض

**جوهر التطبيق.** إنشاء وتعديل عروض الأسعار مع جدول القطع والموردين الثلاثة. تدعم الإضافة السريعة للقطع والطباعة على مرحلتين.

### 4.2 هيكل الشاشة — Layout

```
┌─────────────────────────────────────────────────────────────────┐
│ 📝 عرض سعر جديد                                                  │
│ ┌─────────────────────────────────────────────────────────────┐ │
│ │  رقم العرض: Q-003      التاريخ: [13/07/2026 ▾]              │ │
│ │  الوصف: [_______________________________________________]   │ │
│ │  الحالة: ⚪ Draft                                             │ │
│ ├─────────────────────────────────────────────────────────────┤ │
│ │  ┌────────┬───────────┬──────────┬──────────┬──────────┬──┐ │ │
│ │  │ القطعة │المورد1-نوع│م1-سعر   │م2-نوع   │م2-سعر   │...│ │ │
│ │  ├────────┼───────────┼──────────┼──────────┼──────────┼──┤ │ │
│ │  │ [⌵]    │ [⌵]      │ [___]   │ [⌵]     │ [___]   │🗑️│ │ │
│ │  │ ....   │ ....      │ ....    │ ....     │ ....    │  │ │ │
│ │  └────────┴───────────┴──────────┴──────────┴──────────┴──┘ │ │
│ │  [إضافة قطعة من الكتالوج]  [إضافة قطعة جديدة سريعة]          │ │
│ ├─────────────────────────────────────────────────────────────┤ │
│ │  [💾 حفظ] [🖨️ طباعة بدون أسعار] [🖨️ طباعة نهائية]            │ │
│ │  [📄 PDF] [📧 Outlook]                                      │ │
│ └─────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

### 4.3 رأس العرض — Header Controls

| # | العنصر | النوع WPF | Binding Path | ملاحظات |
|:-:|:-------|:----------|:-------------|:--------|
| 1 | **Quote Number** | `TextBlock` | `Text="{Binding CurrentQuotation.QuoteNumber}"` | للقراءة فقط — يُنشأ تلقائياً |
| 2 | **Date** | `DatePicker` | `SelectedDate="{Binding CurrentQuotation.Date, Mode=TwoWay}"` | افتراضياً: اليوم، قابل للتعديل |
| 3 | **Description** | `TextBox` | `Text="{Binding CurrentQuotation.Description, Mode=TwoWay}"` | Optional, Max 500 حرف, `AcceptsReturn=True` للأسطر المتعددة |
| 4 | **Status Badge** | `Border` + `TextBlock` | `Background="{Binding CurrentQuotation.Status, Converter={StaticResource StatusToColor}}"` | نص: ترجمة الحالة (Draft → مسودة، Printed → مطبوع...) |
| 5 | **Hidden: QuotationId** | (Binding فقط) | `CurrentQuotation.Id` | للاستخدام في Service Calls |

### 4.4 جدول القطع — Items DataGrid

هذا هو الجدول الأهم في التطبيق. يحتوي على **7 أعمدة** لكل قطعة:

| العمود | النوع | Binding Path | ملاحظات |
|:-------|:------|:-------------|:--------|
| **القطعة (اسمها)** | `DataGridTemplateColumn` مع `ComboBox` | `SelectedItem={Binding Item, Mode=TwoWay}`, `ItemsSource={Binding AvailableItems}` | ComboBox مع `IsTextSearchEnabled=True` و `StaysOnEdit=True`. يعرض `Name` من `Item` |
| **المورد 1 — النوع** | `DataGridTemplateColumn` مع `ComboBox` | `Binding={Binding Supplier1Type, Mode=TwoWay}` | `ItemsSource="{Binding Suppliers}"` (DisplayMemberPath="Name") — ينقل الاسم كنص |
| **المورد 1 — السعر** | `DataGridTextColumn` | `Binding={Binding Supplier1Price, Mode=TwoWay, TargetNullValue=''}` | `StringFormat=C`, التحقق: رقم موجب أو فارغ |
| **المورد 2 — النوع** | `DataGridTemplateColumn` مع `ComboBox` | `Binding={Binding Supplier2Type, Mode=TwoWay}` | نفس هيكل المورد 1 |
| **المورد 2 — السعر** | `DataGridTextColumn` | `Binding={Binding Supplier2Price, Mode=TwoWay, TargetNullValue=''}` | — |
| **المورد 3 — النوع** | `DataGridTemplateColumn` مع `ComboBox` | `Binding={Binding Supplier3Type, Mode=TwoWay}` | — |
| **المورد 3 — السعر** | `DataGridTextColumn` | `Binding={Binding Supplier3Price, Mode=TwoWay, TargetNullValue=''}` | — |
| **حذف** | `DataGridTemplateColumn` مع `Button` | `Command="{Binding DataContext.RemoveItemCommand, RelativeSource=...}"`, `CommandParameter={Binding}` | زر ❌ أحمر لكل صف |

#### 4.4.1 تفصيل عمود القطعة (Item ComboBox)

```xml
<!-- TemplateColumn لاختيار القطعة من الكتالوج -->
<DataGridTemplateColumn Header="القطعة" Width="*">
    <DataGridTemplateColumn.CellTemplate>
        <DataTemplate>
            <ComboBox ItemsSource="{Binding DataContext.AvailableItems,
                RelativeSource={RelativeSource AncestorType=DataGrid}}"
                      SelectedItem="{Binding Item, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
                      DisplayMemberPath="Name"
                      IsEditable="True"
                      IsTextSearchEnabled="True"
                      StaysOnEdit="True"
                      TextSearch.TextPath="Name" />
        </DataTemplate>
    </DataGridTemplateColumn.CellTemplate>
</DataGridTemplateColumn>
```

#### 4.4.2 تفصيل عمود المورد (Supplier ComboBox)

```xml
<!-- قالب عمود المورد: ComboBox من Suppliers مع تخزين الاسم كنص -->
<DataGridTemplateColumn Header="المورد 1">
    <DataGridTemplateColumn.CellTemplate>
        <DataTemplate>
            <ComboBox ItemsSource="{Binding DataContext.Suppliers,
                RelativeSource={RelativeSource AncestorType=DataGrid}}"
                      Text="{Binding Supplier1Type, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
                      DisplayMemberPath="Name"
                      IsEditable="True"
                      IsTextSearchEnabled="True" />
        </DataTemplate>
    </DataGridTemplateColumn.CellTemplate>
</DataGridTemplateColumn>
```

### 4.5 أزرار إضافة القطع

| الزر | الأمر | الوصف |
|:-----|:------|:-------|
| **إضافة قطعة من الكتالوج** | `AddItemFromCatalogCommand` | يضيف صفاً جديداً `QuotationItem` مع `Item` فارغ (يختار المستخدم من ComboBox لاحقاً) |
| **إضافة قطعة جديدة سريعة** | `QuickAddItemCommand` | يفتح `QuickAddItemDialog` (نافذة صغيرة أو `InlineExpander`): اسم القطعة فقط ← يحفظ في `Items` + يضيفه للعرض الحالي في خطوة واحدة |

#### 4.5.1 QuickAddItemDialog

| العنصر | النوع | Binding |
|:-------|:------|:--------|
| اسم القطعة | `TextBox` | `Text="{Binding NewItemName, Mode=TwoWay}"` |
| وصف القطعة | `TextBox` | `Text="{Binding NewItemDescription, Mode=TwoWay}"` (اختياري) |
| حفظ وإضافة | `Button` | `Command="{Binding ConfirmQuickAddCommand}"` |
| إلغاء | `Button` | `Command="{Binding CancelQuickAddCommand}"` |

### 4.6 أزرار الإجراءات الرئيسية

| الزر | الأمر | الوصف | الحالة المطلوبة |
|:-----|:------|:-------|:----------------|
| **💾 حفظ** | `SaveQuotationCommand` | يحفظ العرض كمسودة (Status = Draft). إذا كان موجوداً مسبقاً: تحديث | دائماً |
| **🖨️ طباعة بدون أسعار** | `PrintWithoutPricesCommand` | 1. يتحقق من وجود قطعة واحدة على الأقل<br>2. يستدعي `PrintService.PrintQuotation(quotationId, WithoutPrices)` | بعد الحفظ (عرض محفوظ) |
| **🖨️ طباعة نهائية** | `PrintWithPricesCommand` | 1. يتحقق من وجود أسعار مدخلة<br>2. يحدّث Status إلى `UpdatedWithPrices`<br>3. يستدعي `PrintService.PrintQuotation(quotationId, WithPrices)`<br>4. بعد الطباعة: Status = `Printed` | يجب أن يكون Status ≥ `UpdatedWithPrices` |
| **📄 PDF** | `ExportPdfCommand` | 1. يفتح `SaveFileDialog` (Filter: PDF files \| *.pdf)<br>2. يستدعي `PdfService.ExportQuotationToPdfAsync(quotationId, path, showPrices: true)`<br>3. عند النجاح: Status = `PDFExported` | بعد الحفظ |
| **📧 Outlook** | `SendOutlookCommand` | 1. يستدعي `OutlookService.SendQuotationViaOutlookAsync(quotationId)`<br>2. إذا Outlook غير موجود ← رسالة "Outlook غير موجود — سيتم فتح PDF بدلاً من ذلك" + `PdfService`<br>3. عند النجاح: Status = `SentViaOutlook` | بعد الحفظ |

### 4.7 حالات الشاشة

| الحالة | الوصف | المظهر |
|:-------|:------|:-------|
| **New — Empty** | عرض جديد، لا قطع بعد | DataGrid فارغ، رسالة "أضف قطعة لبدء إنشاء العرض" |
| **Editing** | عرض موجود يتم تعديله | جميع البيانات محملة، DataGrid يعرض القطع |
| **Loading** | جارٍ تحميل بيانات عرض موجود | `ProgressBar` يغطي المحتوى (عند فتح عرض للتعديل) |
| **Saving** | جارٍ حفظ التغييرات | الأزرار معطلة، يظهر "جارٍ الحفظ..." |
| **Printing** | جارٍ الطباعة | مؤقت (PrintDialog متزامن) |
| **Error** | فشل في عملية | رسالة خطأ مع إمكانية إعادة المحاولة |
| **Dirty (Unsaved Changes)** | تغييرات غير محفوظة | علامة على زر الحفظ + تحذير عند الخروج |

### 4.8 أحداث مهمة

| الحدث | الإجراء |
|:------|:--------|
| `OnNavigatedTo()` (إذا `quotationId` موجود) | تحميل عرض موجود عبر `QuotationService.GetQuotationByIdAsync(id)` |
| `OnNavigatedTo()` (إذا جديد) | إنشاء عرض جديد عبر `QuotationService.CreateNewQuotationAsync()` |
| `Closing` / `OnNavigatingFrom` | التحقق من وجود تغييرات غير محفوظة → رسالة تأكيد "لديك تغييرات غير محفوظة، هل تريد الحفظ قبل الخروج؟" |

### 4.9 الخدمات المستخدمة

| الخدمة | الدوال |
|:-------|:-------|
| `IQuotationService` | `CreateNewQuotationAsync()`, `GetQuotationByIdAsync(id)`, `UpdateQuotationAsync(quotation)`, `AddQuotationItemAsync(item)`, `RemoveQuotationItemAsync(itemId)`, `UpdateQuotationItemAsync(item)`, `ChangeStatusAsync(quotationId, status)` |
| `ISettingsService` | `GetAllItemsAsync(search)`, `GetAllSuppliersAsync()`, `AddItemAsync(item)` (للقطعة السريعة) |
| `IPrintService` | `PrintQuotation(quotationId, printMode)` |
| `IPdfService` | `ExportQuotationToPdfAsync(quotationId, filePath, showPrices)` |
| `IOutlookService` | `SendQuotationViaOutlookAsync(quotationId)` |

---

## 5. الشاشة الرابعة — S4: Quotation List

### 5.1 الغرض

عرض جميع عروض الأسعار المحفوظة مع إمكانية البحث والتصفية والوصول السريع لفتح أو طباعة أو حذف كل عرض.

### 5.2 هيكل الشاشة — Layout

```
┌─────────────────────────────────────────────────────────────────┐
│ 📂 قائمة العروض                                                  │
│ ┌─────────────────────────────────────────────────────────────┐ │
│ │  بحث: [____________________]    تصفية: [الكل ▾]             │ │
│ ├─────────────────────────────────────────────────────────────┤ │
│ │  ┌───────┬──────────┬─────────────┬────────────┬─────────┐ │ │
│ │  │رقم    │تاريخ     │وصف          │حالة       │إجراءات  │ │ │
│ │  ├───────┼──────────┼─────────────┼────────────┼─────────┤ │ │
│ │  │Q-003  │13/07/2026│توريد قطع... │🟢 مطبوع   │[فتح]    │ │ │
│ │  │Q-002  │12/07/2026│....         │⚪ مسودة   │[فتح]    │ │ │
│ │  │Q-001  │10/07/2026│....         │🟣 مرسل    │[فتح]    │ │ │
│ │  └───────┴──────────┴─────────────┴────────────┴─────────┘ │ │
│ │                                                             │ │
│ │  [← سابق]  الصفحة 1 من 5  [التالي →]                        │ │
│ └─────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

### 5.3 العناصر (Controls)

| # | العنصر | النوع WPF | Binding Path | ملاحظات |
|:-:|:-------|:----------|:-------------|:--------|
| 1 | **Search Box** | `TextBox` | `Text="{Binding SearchText, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"` | مع تأخير 400ms (`Delay=400`) |
| 2 | **Status Filter** | `ComboBox` | `SelectedItem="{Binding SelectedStatusFilter, Mode=TwoWay}"` | Items: الكل, Draft, UpdatedWithPrices, Printed, PDFExported, SentViaOutlook |
| 3 | **Date From Filter** | `DatePicker` | `SelectedDate="{Binding FilterFromDate, Mode=TwoWay}"` | اختياري |
| 4 | **Date To Filter** | `DatePicker` | `SelectedDate="{Binding FilterToDate, Mode=TwoWay}"` | اختياري |
| 5 | **Quotations DataGrid** | `DataGrid` | `ItemsSource="{Binding Quotations}"` | `IsReadOnly=True`, `AutoGenerateColumns=False`, `SelectionMode="Single"` |
| 6 | Column: رقم العرض | `DataGridTextColumn` | `Binding={Binding QuoteNumber}` | Bold |
| 7 | Column: التاريخ | `DataGridTextColumn` | `Binding={Binding Date, StringFormat={}{0:dd/MM/yyyy}}` | باستخدام `DateStringConverter` |
| 8 | Column: الوصف | `DataGridTextColumn` | `Binding={Binding Description}` | يتم اقتطاع النص الطويل (MaxWidth=200) مع `TextTrimming="CharacterEllipsis"` |
| 9 | Column: الحالة | `DataGridTemplateColumn` | `Background={Binding Status, Converter={StaticResource StatusToColor}}` | `Border` ملون + نص الحالة المترجم |
| 10 | Column: إجراءات | `DataGridTemplateColumn` | أزرار: فتح | زر واحد "فتح" الذي ينقل إلى QuotationFormView |
| 11 | **Open Button** (في Column) | `Button` | `Command="{Binding DataContext.OpenQuotationCommand, RelativeSource=...}"`, `CommandParameter={Binding Id}` | "فتح" |
| 12 | **Delete Button** | `Button` (خارج DataGrid) | `Command="{Binding DeleteSelectedCommand}"` | عند وجود تحديد، مع تأكيد |
| 13 | **Pagination** | `StackPanel` | `Text="{Binding PageInfo}"` | أزرار سابق/تالي + "الصفحة X من Y" |

### 5.4 الأوامر

| الأمر | المشغل | الإجراء |
|:------|:-------|:--------|
| `SearchCommand` | تغير `SearchText` (بعد 400ms) | يستدعي `QuotationService.SearchQuotationsAsync(keyword, statusFilter, fromDate, toDate)` |
| `FilterChangedCommand` | تغير الـ ComboBox أو DatePicker | نفس البحث مع الفلتر الجديد |
| `OpenQuotationCommand` | ضغط "فتح" | `NavigationService.NavigateTo("QuotationForm", quotationId)` |
| `DeleteQuotationCommand` | ضغط "حذف" | رسالة تأكيد "هل تريد حذف عرض [QuoteNumber]؟" → `QuotationService.DeleteQuotationAsync(id)` → تحديث القائمة |
| `RefreshCommand` | ضغط F5 أو زر تحديث | إعادة تحميل القائمة |
| `NextPageCommand` | ضغط "التالي" | `Page++` → إعادة البحث |
| `PreviousPageCommand` | ضغط "السابق" | `Page--` → إعادة البحث |

### 5.5 حالات الشاشة

| الحالة | الوصف | المظهر |
|:-------|:------|:-------|
| **Loading** | جارٍ تحميل القائمة | `ProgressBar` في أعلى الشاشة |
| **Loaded** | البيانات ظهرت | DataGrid يعرض النتائج |
| **Empty** | لا توجد عروض مطابقة | رسالة "لا توجد عروض — اضغط 'عرض سعر جديد' لإنشاء أول عرض" + زر اختصار |
| **Search No Results** | البحث لم يجد نتائج | "لا توجد نتائج للبحث المطلوب" |
| **Error** | فشل التحميل | رسالة خطأ + زر "إعادة المحاولة" |

### 5.6 ألوان الحالة في القائمة

| الحالة | اللون | النص المترجم |
|:-------|:------|:-------------|
| `Draft` | رمادي (#9E9E9E) | مسودة |
| `UpdatedWithPrices` | أزرق (#2196F3) | محدث بالأسعار |
| `Printed` | أخضر (#4CAF50) | مطبوع |
| `PDFExported` | برتقالي (#FF9800) | تم تصدير PDF |
| `SentViaOutlook` | بنفسجي (#9C27B0) | مرسل عبر البريد |

### 5.7 الخدمات المستخدمة

| الخدمة | الدوال |
|:-------|:-------|
| `IQuotationService` | `SearchQuotationsAsync(keyword, statusFilter, from, to)`, `DeleteQuotationAsync(id)`, `GetQuotationByIdAsync(id)` |
| `INavigationService` | `NavigateTo(viewName, parameter)` |

---

## 6. الشاشة الخامسة — S5: Reports Screen

### 6.1 الغرض

لوحة تقارير تحليلية تحتوي على 4 تقارير مع أزرار طباعة وتصدير PDF لكل تقرير.

### 6.2 هيكل الشاشة — Layout

```
┌─────────────────────────────────────────────────────────────────┐
│ 📊 التقارير                                                      │
│ ┌─────────────────────────────────────────────────────────────┐ │
│ │  ┌──────────────┐  ┌──────────────┐                        │ │
│ │  │ 📊 مقارنة    │  │ 🔥 أكثر القطع │                        │ │
│ │  │    أسعار     │  │   طلباً      │                        │ │
│ │  │   الموردين   │  │              │                        │ │
│ │  └──────────────┘  └──────────────┘                        │ │
│ │  ┌──────────────┐  ┌──────────────┐                        │ │
│ │  │ 📋 سجل العروض│  │ 💰 الإجمالي  │                        │ │
│ │  │              │  │    الشهري    │                        │ │
│ │  └──────────────┘  └──────────────┘                        │ │
│ ├─────────────────────────────────────────────────────────────┤ │
│ │  [🖨️ طباعة]  [📄 PDF]                                      │ │
│ │  ┌─────────────────────────────────────────────────────────┐ │ │
│ │  │                                                         │ │ │
│ │  │     (منطقة عرض التقرير المحدد — ContentControl)          │ │ │
│ │  │                                                         │ │ │
│ │  └─────────────────────────────────────────────────────────┘ │ │
│ └─────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

### 6.3 أزرار التقارير — Navigation

| الزر | الأمر | الوصف |
|:-----|:------|:-------|
| **مقارنة أسعار الموردين** | `ShowReportCommand("D1")` | يعرض `SupplierComparisonView` في منطقة المحتوى |
| **أكثر القطع طلباً** | `ShowReportCommand("D2")` | يعرض `TopItemsView` |
| **سجل العروض** | `ShowReportCommand("D3")` | يعرض `QuotationHistoryView` مع فلتر تاريخ |
| **الإجمالي الشهري** | `ShowReportCommand("D4")` | يعرض `MonthlyTotalView` |

#### 6.3.1 تفصيل منطقة عرض التقرير

```xml
<!-- المحتوى الديناميكي للتقرير المختار -->
<ContentControl Content="{Binding CurrentReportView}">
    <ContentControl.Resources>
        <DataTemplate DataType="{x:Type vm:SupplierComparisonViewModel}">
            <views:SupplierComparisonView />
        </DataTemplate>
        <DataTemplate DataType="{x:Type vm:TopItemsViewModel}">
            <views:TopItemsView />
        </DataTemplate>
        <DataTemplate DataType="{x:Type vm:QuotationHistoryViewModel}">
            <views:QuotationHistoryView />
        </DataTemplate>
        <DataTemplate DataType="{x:Type vm:MonthlyTotalViewModel}">
            <views:MonthlyTotalView />
        </DataTemplate>
    </ContentControl.Resources>
</ContentControl>
```

### 6.4 التقرير D1 — Supplier Price Comparison (مقارنة أسعار الموردين)

#### 6.4.1 العناصر

| # | العنصر | النوع WPF | Binding |
|:-:|:-------|:----------|:--------|
| 1 | **Title** | `TextBlock` | نص: "مقارنة أسعار الموردين"، FontSize=18 |
| 2 | **DataGrid** | `DataGrid` | `ItemsSource="{Binding ComparisonData}"` |
| 3 | Column: اسم المورد | `DataGridTextColumn` | `Binding={Binding SupplierName}` |
| 4 | Column: إجمالي المبلغ | `DataGridTextColumn` | `Binding={Binding TotalAmount, StringFormat=C}` |
| 5 | Column: متوسط السعر | `DataGridTextColumn` | `Binding={Binding AveragePrice, StringFormat=C}` |
| 6 | Column: عدد القطع | `DataGridTextColumn` | `Binding={Binding ItemCount}` |
| 7 | **Print Button** | `Button` | `Command="{Binding PrintCommand}"` |
| 8 | **PDF Button** | `Button` | `Command="{Binding ExportPdfCommand}"` |

### 6.5 التقرير D2 — Most Requested Items (أكثر القطع طلباً)

#### 6.5.1 العناصر

| # | العنصر | النوع WPF | Binding |
|:-:|:-------|:----------|:--------|
| 1 | **Title** | `TextBlock` | نص: "أكثر 10 قطع طلباً في العروض" |
| 2 | **Top N Spinner** | `NumericUpDown` | `Value="{Binding TopN, Mode=TwoWay}"`, Min=5, Max=50, Default=10 |
| 3 | **DataGrid** | `DataGrid` | `ItemsSource="{Binding TopItems}"` |
| 4 | Column: # | `DataGridTextColumn` | رقم الترتيب (1, 2, 3...) |
| 5 | Column: اسم القطعة | `DataGridTextColumn` | `Binding={Binding ItemName}` |
| 6 | Column: عدد مرات الطلب | `DataGridTextColumn` | `Binding={Binding RequestCount}` |
| 7 | **Refresh Button** | `Button` | `Command="{Binding RefreshCommand}"` |

### 6.6 التقرير D3 — Quotation History (سجل العروض)

#### 6.6.1 العناصر

| # | العنصر | النوع WPF | Binding |
|:-:|:-------|:----------|:--------|
| 1 | **Title** | `TextBlock` | نص: "سجل العروض" |
| 2 | **From Date** | `DatePicker` | `SelectedDate="{Binding FromDate, Mode=TwoWay}"` |
| 3 | **To Date** | `DatePicker` | `SelectedDate="{Binding ToDate, Mode=TwoWay}"` |
| 4 | **DataGrid** | `DataGrid` | `ItemsSource="{Binding HistoryData}"` |
| 5 | Column: رقم العرض | `DataGridTextColumn` | `Binding={Binding QuoteNumber}` |
| 6 | Column: التاريخ | `DataGridTextColumn` | `Binding={Binding Date, StringFormat=dd/MM/yyyy}` |
| 7 | Column: الوصف | `DataGridTextColumn` | `Binding={Binding Description}` |
| 8 | Column: الحالة | `DataGridTemplateColumn` | مع StatusToColor |
| 9 | Column: المبلغ الإجمالي | `DataGridTextColumn` | `Binding={Binding TotalAmount, StringFormat=C}` |

**ملاحظة:** `TotalAmount` يُحسب كمجموع Supplier1Price + Supplier2Price + Supplier3Price لكل `Quotation` ويُضاف كخاصية في `QuotationHistoryDto`.

### 6.7 التقرير D4 — Monthly Total (الإجمالي الشهري)

#### 6.7.1 العناصر

| # | العنصر | النوع WPF | Binding |
|:-:|:-------|:----------|:--------|
| 1 | **Title** | `TextBlock` | نص: "الإجمالي الشهري" |
| 2 | **Year** | `ComboBox` | `SelectedValue="{Binding SelectedYear, Mode=TwoWay}"` | Items: 2026, 2027... |
| 3 | **Month** | `ComboBox` | `SelectedValue="{Binding SelectedMonth, Mode=TwoWay}"` | Items: يناير... ديسمبر |
| 4 | **Summary Cards** | `StackPanel` مع `Border` لكل بطاقة | — |
| 5 | Card: إجمالي المورد 1 | `TextBlock` | `Text="{Binding MonthlyData.TotalFromSupplier1, StringFormat=C}"` |
| 6 | Card: إجمالي المورد 2 | `TextBlock` | `Text="{Binding MonthlyData.TotalFromSupplier2, StringFormat=C}"` |
| 7 | Card: الإجمالي الكلي | `TextBlock` | `Text="{Binding MonthlyData.GrandTotal, StringFormat=C}"` |
| 8 | Card: عدد العروض | `TextBlock` | `Text="{Binding MonthlyData.QuotationCount}"` |
| 9 | **Refresh Button** | `Button` | `Command="{Binding RefreshCommand}"` |

#### 6.7.2 بطاقات الملخص — XAML توضيحي

```xml
<StackPanel Orientation="Horizontal" HorizontalAlignment="Center">
    <!-- بطاقة الإجمالي الكلي -->
    <Border Background="{StaticResource PrimaryBrush}" CornerRadius="8" Padding="16" Margin="8">
        <StackPanel>
            <TextBlock Text="الإجمالي الكلي" Foreground="White" FontSize="12"/>
            <TextBlock Text="{Binding MonthlyData.GrandTotal, StringFormat=C}"
                       Foreground="White" FontSize="24" FontWeight="Bold"/>
        </StackPanel>
    </Border>
    <!-- بطاقة عدد العروض -->
    <Border Background="{StaticResource AccentBrush}" CornerRadius="8" Padding="16" Margin="8">
        <StackPanel>
            <TextBlock Text="عدد العروض" Foreground="White" FontSize="12"/>
            <TextBlock Text="{Binding MonthlyData.QuotationCount}"
                       Foreground="White" FontSize="24" FontWeight="Bold"/>
        </StackPanel>
    </Border>
</StackPanel>
```

### 6.8 حالات Reports Screen

| الحالة | الوصف |
|:-------|:------|
| **No Report Selected** | لم يختر المستخدم تقريراً بعد — رسالة "اختر تقريراً من الأزرار أعلاه" |
| **Loading** | جارٍ تحميل بيانات التقرير المختار — `ProgressBar` |
| **Loaded** | التقرير معروض |
| **Empty** | لا توجد بيانات للتقرير (مثلاً: لا عروض في الشهر المحدد) |
| **Error** | فشل تحميل البيانات |

### 6.9 الخدمات المستخدمة

| الخدمة | الدوال |
|:-------|:-------|
| `IReportService` | `GetSupplierPriceComparisonAsync()`, `GetMostRequestedItemsAsync(topN)`, `GetQuotationHistoryAsync(from, to)`, `GetMonthlyTotalAsync(year, month)` |
| `IPrintService` | `PrintReport(reportData, reportType)` |
| `IPdfService` | `ExportReportToPdfAsync(reportData, reportType, filePath)` |

---

## 7. المكونات المشتركة (Shared Components & Converters)

### 7.1 الـ Converters المطلوبة

| الـ Converter | المدخل | المخرج | الغرض |
|:--------------|:-------|:-------|:-------|
| `BoolToVisibilityConverter` | `bool` | `Visibility.Visible` / `Collapsed` | إظهار/إخفاء عناصر (مرر `true=Visible`, `false=Collapsed`) |
| `InverseBoolToVisibilityConverter` | `bool` | `Collapsed` / `Visible` | عكس BoolToVisibility |
| `StatusToColorConverter` | `string` (Status) | `Color` (SolidColorBrush) | تحويل حالة العرض إلى لون |
| `StatusToTextConverter` | `string` (Status) | `string` (عربي) | تحويل الحالة الإنجليزية إلى نص عربي |
| `DateFormatConverter` | `DateTime` | `string` (dd/MM/yyyy) | تنسيق التاريخ |
| `PriceFormatConverter` | `decimal?` | `string` (0.00 د.ك) | تنسيق العملة مع فراغ إذا null |
| `InverseBoolConverter` | `bool` | `bool` | عكس القيمة المنطقية (لـ IsEnabled) |
| `NullToVisibilityConverter` | `object?` | `Visible` / `Collapsed` | إذا null ← Collapsed |

**مثال تنفيذ StatusToColorConverter:**

```csharp
public class StatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString() switch
        {
            "Draft"             => new SolidColorBrush(Color.FromRgb(0x9E, 0x9E, 0x9E)), // رمادي
            "UpdatedWithPrices" => new SolidColorBrush(Color.FromRgb(0x21, 0x96, 0xF3)), // أزرق
            "Printed"           => new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50)), // أخضر
            "PDFExported"       => new SolidColorBrush(Color.FromRgb(0xFF, 0x98, 0x00)), // برتقالي
            "SentViaOutlook"    => new SolidColorBrush(Color.FromRgb(0x9C, 0x27, 0xB0)), // بنفسجي
            _                   => new SolidColorBrush(Colors.Gray)
        };
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
```

**مثال تنفيذ StatusToTextConverter:**

```csharp
public class StatusToTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString() switch
        {
            "Draft"             => "مسودة",
            "UpdatedWithPrices" => "محدّث بالأسعار",
            "Printed"           => "مطبوع",
            "PDFExported"       => "تم تصدير PDF",
            "SentViaOutlook"    => "مرسل عبر البريد",
            _                   => value?.ToString() ?? ""
        };
    }
}
```

### 7.2 عناصر UI مشتركة

| العنصر | الموقع | الوصف |
|:-------|:-------|:-------|
| `StatusBadge` | `Styles/Theme.xaml` أو ControlTemplate | `Border` مدوّر (`CornerRadius=12`) مع `TextBlock` بداخله — الملء من StatusToColorConverter و StatusToTextConverter |
| `CurrencyTextBox` | عنصر مباشر | `TextBox` مع `StringFormat=C` أو استخدام `PriceFormatConverter` |
| `SidebarButton` | `Styles/Theme.xaml` | Style مخصص لأزرار الشريط الجانبي في MainWindow |
| `EmptyStateMessage` | `Styles/DataTemplates.xaml` | قالب لعرض رسالة "لا توجد بيانات" مع أيقونة ونص |
| `LoadingOverlay` | `Styles/DataTemplates.xaml` | `ProgressBar` يغطي المحتوى أثناء التحميل |

### 7.3 Style للـ StatusBadge (XAML نموذجي)

```xml
<Style x:Key="StatusBadgeStyle" TargetType="Border">
    <Setter Property="CornerRadius" Value="12"/>
    <Setter Property="Padding" Value="8,4"/>
    <Setter Property="Margin" Value="4"/>
    <Setter Property="BorderThickness" Value="0"/>
</Style>

<!-- استخدام StatusBadge في DataGrid أو أي مكان -->
<Border Style="{StaticResource StatusBadgeStyle}"
        Background="{Binding Status, Converter={StaticResource StatusToColor}}">
    <TextBlock Text="{Binding Status, Converter={StaticResource StatusToText}}"
               Foreground="White" FontSize="12"/>
</Border>
```

### 7.4 تسجيل الـ Converters في App.xaml

```xml
<Application.Resources>
    <ResourceDictionary>
        <converters:BoolToVisibilityConverter x:Key="BoolToVis"/>
        <converters:StatusToColorConverter x:Key="StatusToColor"/>
        <converters:StatusToTextConverter x:Key="StatusToText"/>
        <converters:DateFormatConverter x:Key="DateFormat"/>
        <converters:PriceFormatConverter x:Key="PriceFormat"/>
        <converters:InverseBoolConverter x:Key="InverseBool"/>
        <converters:NullToVisibilityConverter x:Key="NullToVis"/>
    </ResourceDictionary>
</Application.Resources>
```

---

## 8. قواعد عامة (Cross-Cutting Rules)

### 8.1 قواعد التصميم

| القاعدة | الوصف |
|:--------|:------|
| **RTL إلزامي** | `FlowDirection="RightToLeft"` على جميع الـ Windows و Pages و UserControls. لا استثناء |
| **الخط العربي** | استخدام `Noto Sans Arabic` أو خط النظام العربي مع `FontFamily="Segoe UI, Noto Sans Arabic"` |
| **الألوان** | تعريف الألوان في `Styles/Theme.xaml` كـ `SolidColorBrush` موارد (Resources) |
| **أحجام النوافذ** | LoginView: 400×350. MainWindow: 1200×800 (قابل للتصغير إلى 900×600) |
| **إطارات DataGrid** | استخدام `RowDetailsTemplate` لتفادي الازدحام عند الحاجة |
| **أزرار الحذف** | دائماً مع `MessageBox.Show("هل أنت متأكد؟", "تأكيد", MessageBoxButton.YesNo)` قبل الحذف |
| **تحميل غير متزامن** | جميع استدعاءات الخدمات تكون `async Task` مع `await` |

### 8.2 قواعد الأخطاء

| القاعدة | الوصف |
|:--------|:-------|
| **أخطاء قاعدة البيانات** | رسالة عامة: "حدث خطأ في قاعدة البيانات. تحقق من اتصال التطبيق." |
| **أخطاء الإدخال** | `Validation.ErrorTemplate` أحمر حول الحقل + `ToolTip` مع رسالة الخطأ |
| **Outlook غير موجود** | رسالة: "Outlook غير مثبت على هذا الجهاز. سيتم فتح نسخة PDF." |
| **أخطاء غير متوقعة** | رسالة: "حدث خطأ غير متوقع. تم تسجيل الخطأ في السجلات." |

### 8.3 قواعد الحفظ التلقائي

| القاعدة | التفاصيل |
|:--------|:---------|
| **Settings** | حفظ يدوي (زر حفظ) — لا حفظ تلقائي |
| **QuotationForm** | عند الضغط على أي زر طباعة/PDF/Outlook → حفظ تلقائي قبل التنفيذ |
| **الخروج من QuotationForm** | إذا كان فيها تغييرات غير محفوظة → رسالة تأكيد |

### 8.4 قواعد التحقق من الإدخال (Validation Rules)

| الحقل | القاعدة | مكان التنفيذ |
|:------|:--------|:-------------|
| **كلمة المرور** | ≥ 4 أحرف | ViewModel (LoginViewModel) |
| **اسم القطعة** | Required, Max 200 | Data Annotations + View |
| **اسم المورد** | Required, Max 200 | Data Annotations + View |
| **السعر** | رقم موجب (≥ 0) أو null | ViewModel (OnPropertyChanged) |
| **الوصف** | Optional, Max 500 | Data Annotations |
| **التاريخ** | لا يمكن أن يكون في المستقبل البعيد (> سنة) | ViewModel |

### 8.5 قواعد التنقل (Navigation Rules)

| القاعدة | الوصف |
|:--------|:-------|
| **Login First** | لا يمكن الوصول لأي شاشة قبل تسجيل الدخول |
| **Modal Login** | LoginView نافذة Modal — لا يمكن تجاوزها |
| **حفظ الحالة** | عند إغلاق التطبيق، يُحفظ آخر شاشة مفتوحة في `Setting(Key="LastActiveScreen")` |
| **Back Navigation** | كل Page تدعم `NavigationService.GoBack()` عند الحاجة |
| **عرض جديد** | `NavigateTo("QuotationForm")` بدون parameter → وضع إنشاء جديد |
| **فتح عرض** | `NavigateTo("QuotationForm", quotationId)` → وضع تعديل |

---

## 9. Gaps and Recommendations

### 9.1 Design Gaps

| # | الفجوة | النوع | التأثير | التوصية |
|:-:|:-------|:-----:|:--------|:--------|
| 1 | **`28_UI_UX_GUIDELINES.md` غير مكتمل** | 📄 قالب | لا توجد ألوان وخطوط وأحجام محددة مسبقاً | EngineeringAgent يستخدم ألواناً افتراضية محايدة ويوثقها في `Styles/Theme.xaml` خلال التنفيذ |
| 2 | **`05_BUSINESS_WORKFLOWS.md` غير موجود** | 📄 مفقود | قد توجد حالات Workflow غير موثقة تؤثر على تدفق الشاشات (مثلاً: هل يمكن الطباعة قبل الحفظ؟ هل يمكن تعديل عرض مطبوع؟) | يُنشأ الملف فوراً. راجع توصية Section 9.3 |
| 3 | **`13_REPORTS_AND_DASHBOARDS.md` غير موجود** | 📄 مفقود | تفاصيل التقارير (الأعمدة، التجميع، الفلاتر) موثقة جزئياً فقط | يُنشأ الملف لتوثيق DTOs والاستعلامات الكاملة |
| 4 | **التوقيعات — هل تدعم الصور؟** | 🤔 غير محدد | العميل طلب أسماء توقيعات فقط، ولكن قد يحتاج صور توقيعات لاحقاً | الاكتفاء بالنصوص حالياً — التوسيع للصور يؤجل للإصدار الثاني |
| 5 | **Outlook Integration — تفاصيل** | 🤔 غير محدد | هل يفتح Outlook مع الرسالة جاهزة للإرسال أم يُرسل تلقائياً؟ | فتح نافذة Outlook مع العرض مرفقاً — المستخدم يضغط إرسال يدوياً |

### 9.2 توصيات للملفات التالية

#### 9.2.1 `05_BUSINESS_WORKFLOWS.md`

يجب توثيق سير العمل التالي بالتفصيل:

```
1.  تسجيل الدخول → فتح MainWindow
2.  إعداد أولي (موردين + قطع + ترويسة) ← اختياري لكن موصى به
3.  إنشاء عرض سعر جديد → ملء البيانات ← حفظ (Draft)
4.  طباعة بدون أسعار ← إرسال للموردين
5.  استلام الأسعار ← فتح العرض ← إدخال الأسعار ← حفظ (UpdatedWithPrices)
6.  طباعة نهائية ← (Printed)
7.  تصدير PDF ← (PDFExported) / إرسال Outlook ← (SentViaOutlook)
8.  الرجوع للعرض: يفتح للتعديل ← يحافظ على الحالة أو يسمح بالتراجع؟

أسئلة تحتاج إجابة في Workflow:
- هل يمكن تعديل عرض بعد طباعته؟ (Draft فقط؟)
- هل يمكن حذف عرض مطبوع؟
- ماذا يحدث عند حذف قطعة مستخدمة في عروض سابقة؟ (Restrict حالياً)
```

#### 9.2.2 `13_REPORTS_AND_DASHBOARDS.md`

يجب توثيق لكل تقرير:

| التقرير | الأعمدة | الفلاتر | المصدر (LINQ) | تنسيق الطباعة/PDF |
|:--------|:--------|:--------|:--------------|:-------------------|
| D1 | SupplierName, TotalAmount, AveragePrice, ItemCount | لا فلاتر | `ReportService.GetSupplierPriceComparisonAsync()` | جدول بسيط |
| D2 | Rank, ItemName, RequestCount | TopN (5-50) | `ReportService.GetMostRequestedItemsAsync(topN)` | قائمة مرتبة |
| D3 | QuoteNumber, Date, Description, Status, TotalAmount | FromDate, ToDate | `ReportService.GetQuotationHistoryAsync(from, to)` | جدول زمني |
| D4 | GrandTotal, TotalBySupplier, QuotationCount | Year, Month | `ReportService.GetMonthlyTotalAsync(year, month)` | بطاقات ملخص |

### 9.3 توافق مع Architecture و Data Model

| النقطة | Architecture / Data Model | UI Structure | التوافق |
|:-------|:--------------------------|:-------------|:--------|
| **5 حالات Status** | Draft, UpdatedWithPrices, Printed, PDFExported, SentViaOutlook | نفس الحالات مع StatusToColorConverter ✅ | ✅ متوافق |
| **Supplier ليس FK** | ADR-005 — يُخزن كنص | ComboBox في DataGrid ينقل `Name` كنص إلى `Supplier1Type` ✅ | ✅ متوافق |
| **Item FK مع Restrict** | يمنع حذف قطعة مستخدمة | رسالة خطأ عند محاولة حذف قطعة مستخدمة ✅ | ✅ متوافق |
| **QuoteNumber تلقائي** | Q-XXX | TextBlock للقراءة فقط ✅ | ✅ متوافق |
| **NavigationService** | Frame + Pages | جميع الشاشات Pages في Frame ✅ | ✅ متوافق |
| **Async/await** | كل الخدمات async | ViewModel تستخدم async Commands ✅ | ✅ متوافق |

---

## ختم الملف

| البند | الحالة |
|:------|:-------|
| **إعداد** | Software Designer Agent (مُصمم) |
| **تاريخ الإصدار** | 2026-07-13 |
| **الحالة** | ✅ Module Baseline Approved |
| **عدد الشاشات المفصّلة** | **5 شاشات رئيسية** (S1–S5) |
| **عدد العناصر (Controls) الموثقة** | 62+ عنصراً |
| **عدد الأوامر (Commands) الموثقة** | 35+ أمراً |
| **عدد الـ Converters المطلوبة** | 8 Converters |
| **الاعتماد** | ⏳ بانتظار مراجعة TeraAgent و Majed |

---

**روابط ذات صلة:**
- [08_TECHNICAL_ARCHITECTURE.md](./08_TECHNICAL_ARCHITECTURE.md) — القرارات المعمارية
- [06_DATA_MODEL_PREPARATION.md](./06_DATA_MODEL_PREPARATION.md) — نموذج البيانات
- [APPLICATION_BLUEPRINT.md](./APPLICATION_BLUEPRINT.md) — المخطط العام
- [28_UI_UX_GUIDELINES.md](./28_UI_UX_GUIDELINES.md) — إرشادات التصميم (قالب)
- [TERA_PROJECT_DECISION.md](../project-control/TERA_PROJECT_DECISION.md) — قرارات المشروع
