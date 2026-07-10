# سجل المهام — تحويل OpenCode إلى TeraOpenCode
## ملف: task-registry.md
## المسار: .tera-workspace/TASKS/
## التاريخ: 2026-07-10

---

## المهمة 001: فحص المشروع والتأكد من التشغيل ✅

| الحقل | القيمة |
|---|---|
| الحالة | ✅ مكتملة |
| المسؤول | TeraAgent |
| الأولوية | عالية |
| الوصف | تشغيل bun install والتأكد أن المشروع يشتغل حالياً قبل أي تعديل |

### النتيجة:
| الخطوة | النتيجة |
|---|---|
| تثبيت Bun | ✅ Bun v1.3.14 مثبت |
| تشغيل bun install | ✅ 4196 حزمة — نجاح |
| تشغيل bun run dev | ✅ الـ TUI بدأ واستمر في العمل |
| حفظ التغييرات | ✅ commit b936f5c — .tera-workspace/ مضاف للـ git |

### ملاحظات:
- Bun لم يكن مثبتاً مسبقاً — تم تثبيته عبر npm install -g bun
- الـ git repo الرئيسي هو TeraSystem (وليس TeraAi/)
- المشكلة: "husky: .git can't be found" وهذا متوقع لأننا في subfolder

---

## المهمة 002: إنشاء مستودع git مستقل ✅

| الحقل | القيمة |
|---|---|
| الحالة | ✅ مكتملة |
| المسؤول | TeraAgent |
| الأولوية | عالية |

### النتيجة:

| الخطوة | التفاصيل |
|---|---|
| استخراج التاريخ | `git subtree split` — 117 commit من تاريخ OpenCode |
| مستودع GitHub | **https://github.com/Alabasi1980/TeraOpenCode** |
| النوع | Public |
| الوصف | TeraOpenCode — Fork of OpenCode AI Coding Agent. Standalone engine for TeraSystem. |
| الفرع | `master` (من `tera-opencode-extracted`) |
| TAG | `fork-baseline-v1.17.18` — علامة نقطة الفصل |
| Remote | `tera-opencode` → https://github.com/Alabasi1980/TeraOpenCode.git |

---

## المهمة 003: إعادة تسمية Branding (المرحلة 2) — قيد التنفيذ

| الحقل | القيمة |
|---|---|
| الحالة | 🔄 قيد التنفيذ |
| المسؤول | TeraAgent |
| الأولوية | عالية |

### ما تم:
| الدفعة | التفاصيل | الحالة |
|---|---|---|
| 1 | Root package.json, README, حذف install/flake/sst.config | ✅ commit 6114f23 |
| 2 | CLI binary rename (opencode → tera) | ✅ commit 00cb003 |
| 3 | Scope @opencode-ai/ → @tera-system/ في 1286 ملف | ✅ commit a14c57f + fix 6372729 |
| 4 | النصوص الظاهرية | ⏳ معلقة |
| 5 | تنظيف الملفات | ⏳ معلقة |

### ملاحظات هامة:
- المشروع لا يزال يعمل بعد تغيير الـ scope (تم اختبار bun install + bun run dev)
- 5184 استبدال في 1253 ملف
- بقي: Batch 4 (نصوص ظاهرية) + Batch 5 (تنظيف) |
