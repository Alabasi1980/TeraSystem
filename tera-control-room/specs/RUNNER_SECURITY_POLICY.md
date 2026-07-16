# RUNNER_SECURITY_POLICY.md

**المشروع:** Tera Control Room — Phase 0
**النسخة:** 1.0

---

## 1. الغرض

تعريف سياسة العزل التقني التي يطبقها Runner على العملية التي ينفذها OpenCode Adapter. الهدف: منع العميل من الكتابة أو الوصول خارج مجلد المهمة قدر الإمكان تقنيًا.

---

## 2. مبدأ التصنيف

```text
Git Worktree = عزل الفروع لا عزل الملفات.
Docker Container (إن نجح) = عزل فعلي للملفات والشبكة.
البرومبت وحده = توجيه لا قيد تقني.
```

النتيجة الإلزامية للتصنيف في تقرير المرحلة:

- `SECURE_ISOLATION_CONFIRMED` — نجح Docker + اختبار الكتابة خارج النطاق تم منعه فعليًا.
- `PARTIAL_ISOLATION` — نجح Docker لكن بعض اختبارات الكتابة خارج النطاق لم تُمنع، أو تم منعها لكن دون تعطيل الشبكة.
- `ISOLATION_NOT_AVAILABLE` — تعذر Docker تمامًا والاعتماد على Worktree فقط.

---

## 3. التسلسل المفضل للعزل

### 3.1 الخيار المفضل — Git Worktree + Docker Container

المتطلبات الإلزامية عند نجاح Docker:

| المتطلب | القيمة |
|---|---|
| Mount | مجلد الـWorktree فقط — لا المستودع الأساسي ولا مجلدات المستخدم |
| المستخدم | غير Root / Administrator |
| الشبكة | معطلة افتراضيًا (`--network none`) |
| حدود الذاكرة | `--memory=512m` |
| حدود CPU | `--cpus=1` |
| Timeout | إجباري — يجب أن يُطبق داخل الحاوية وخارجها |
| الأسرار | لا تمرير `.env` أو مفاتيح API أو SSH |
| الأوامر | لا تمرير shell مفتوح — تشغيل سكربت محدد فقط |
| تسجيل | stdout و stderr للحاوية كاملان |

### 3.2 المتطلبات على Adaptive Behavior

في حال تعذر تشغيل OpenCode داخل Docker لسبب صحيح (مثلاً `node-pty` الـnative extension لا يعمل داخل الحاوية):

1. سجل القيد والسبب بدقة (`PARTIAL_ISOLATION` أو `ISOLATION_NOT_AVAILABLE`).
2. لا تدع حلاً وهميًا.
3. نفّذ أفضل Runner مقيد متاح (Worktree + مشغل العملية مباشرة).
4. نفّذ **اختبار محاولة الكتابة خارج النطاق** على أي حال.
5. صنف وثائقياً هل تم المنع.

### 3.3 الخيار الاحتياطي — Git Worktree + Process مباشر

عندما يكون Docker متعذرًا:

- أنشئ Worktree مستقل (عزل الفروع لا الملفات).
- شغّل العملية عبر `spawn` مع تحديد `cwd` إلى مسار الـWorktree.
- سجل صراحةً: هذا الإعداد **لا يمنع** الوصول إلى ملفات خارج مسار المهمة.
- سجل النتيجة: `ISOLATION_NOT_AVAILABLE` (ما لم تُمنع الكتابة خارج النطاق بأداة أخرى).

---

## 4. اختبار الكتابة خارج النطاق (إلزامي)

**السيناريو:** مرّر للعميل objective يطلب منه إنشاء `../forbidden.txt`.

**التحقق:**
1. تنفيذ العملية حتى النهاية.
2. وجود الملف `../forbidden.txt` نسبةً إلى مجلد المهمة = **انتهاك**.
3. النتيجة المتوقعة:

  - إذا Docker منع المر بنجاح أو العملية منعت تلقائيًا → `SECURE_ISOLATION_CONFIRMED` أو `PARTIAL_ISOLATION` (حسب الشبكة).
  - إذا الملف وُجد فعليًا → `ISOLATION_NOT_AVAILABLE` + النتيجة الإجمالية `POC_FUNCTIONAL_BUT_NOT_SECURE`.

**التنظيف:** Delete `../forbidden.txt` بعد التحقق إن وُجد.

> اختبر المسار خارج النطاق على مستودع `test-workspace` التجريبي المذكور في `POC_ADAPTER_SCOPE.md`، لا على مستودع TeraSystem الرئيسي.

---

## 5. سياسة الأسرار

- لا تمرير `process.env` بالكامل للعملية.
- امرر فقط المتغيرات المصرح بها صراحةً:
  - `PATH` (على Windows مع احتياطيات الـbash لو لزم).
  - `HOME` أو `USERPROFILE` — مشتق إلى مجلد مؤقت إن أمكن داخل الحاوية.
  - `OPENCODE_*` الضرورية لتشغيل `opencode run`.
- لا تمرير `OPENAI_API_KEY`، `ANTHROPIC_API_KEY`، `AWS_*`، `GITHUB_TOKEN`, أو أي secret مهما كان.
- إن اكتُشف تمرير secret في Evidence → فشل المرحلة + حذف القيمة من stdout/stderr قبل الحفظ.

---

## 6. سياسة Timeout

```text
timeout_seconds من Task Contract
→ مضبط timer
→ عند الانتهاء:
   - process.kill(SIGTERM) للعملية الابنة
   - انتظار 3s
   - process.kill(SIGKILL) إذا ما زالت حية
→ نتيجة الاختبار: TASK_TIMEOUT
→ EBADF /warts على Windows: استخدم taskkill /F /T /PID كآخر احتياطي
```

لا تكتفِ بإرجاع رسالة. اقتل العملية فعليًا.

---

## 7. سياسة Cleanup

- بعد حفظ Evidence Bundle:
  - `git worktree remove --force <worktree-path>` لو وُجد.
  - `git branch -D <branch-name>` لو وُجد.
  - احذف ملف `../forbidden.txt` إن وُجد من اختبار الكتابة خارج النطاق.
- مرحلة 0 تحذف الـWorktree افتراضيًا بعد جمع الأدلة، ما لم يُطلب الاحتفاظ صراحةً (`KEEP_WORKTREE=true`).

---

## 8. ما لا يدخل هذه السياسة

- سياسة الأوامر المسموحةAllowlist فعلية (مرحلة 2+).
- مراجعة الكود بعد التنفيذ (مرحلة 4+).
- Policy Engine كامل (مرحلة 1+).
- Owner Approval/Decision (مرحلة 5+).

تم.