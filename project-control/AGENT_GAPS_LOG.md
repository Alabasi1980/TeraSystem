# AGENT_GAPS_LOG.md

## الغرض

سجل مركزي خاص بفجوات ومشكلات واقتراحات تطوير العملاء الأساسيين في منظومة Tera.

هذا السجل مخصص لتطوير منظومة Tera وعملائها الأساسيين فقط، وليس لتتبع أعمال تطبيقات العملاء أو مهام المشاريع.

يستخدمه `TeraSystemEvolutionAgent` كمصدر أساسي لتحسين العملاء، لأن أفضل تطوير هو تصحيح الأخطاء والفجوات الفعلية.

يشمل ذلك صراحةً:

- `TeraAgent`
- `TeraClientEngagementAgent`
- `TeraSystemEvolutionAgent`
- `ApplicationBlueprintAgent`
- `SoftwareDesignerAgent`
- `Auditor`
- `Monitor`
- `DesignReviewer`

---

## القاعدة الحاكمة

```text
Agents observe and report.
TeraSystemEvolutionAgent reviews and proposes.
Majed approves.
Only then changes are implemented.
```

لا يجوز لأي عميل استخدام هذا السجل لتعديل ملفه أو زيادة صلاحياته مباشرة.

---

## الحالات المعتمدة

| Status | المعنى | من يحددها |
|---|---|---|
| `Pending` | إدخال جديد لم يُراجع بعد | العميل المُبلِّغ |
| `Under Review` | قيد تحليل TeraSystemEvolutionAgent | TeraSystemEvolutionAgent |
| `Approved` | مقبول مبدئياً ويحتاج SYSTEM_CHANGE_PROPOSAL | TeraSystemEvolutionAgent بعد موافقة/توجيه المالك |
| `Applied` | تم تطبيقه وتسجيله في SYSTEM_EVOLUTION_LOG.md | TeraSystemEvolutionAgent |
| `Rejected` | مرفوض مع سبب واضح | TeraSystemEvolutionAgent |
| `Duplicate` | مكرر ويرتبط بإدخال سابق | TeraSystemEvolutionAgent |
| `Deferred` | مؤجل لدورة مراجعة لاحقة | TeraSystemEvolutionAgent |

---

## قاعدة منع التكرار

قبل تسجيل Gap جديد، يجب على العميل:

1. قراءة هذا الملف.
2. البحث عن Gap مشابه.
3. إذا وجد Gap مشابه بحالة `Rejected` أو `Duplicate` أو `Applied`، لا يسجل إدخالاً جديداً.
4. إذا وجد Gap مشابه بحالة `Pending` أو `Under Review` أو `Approved`، لا يسجل إدخالاً جديداً؛ يضيف ملاحظة داعمة فقط إذا كانت ضرورية.
5. إذا لم يجد Gap مشابه، يسجل إدخالاً جديداً.

---

## صيغة الإدخال

```md
## [YYYY-MM-DD] — [Agent Name] — GAP-XXX

- Title:
- Agent: TeraAgent / TeraClientEngagementAgent / Auditor / Monitor / DesignReviewer / TeraSystemEvolutionAgent
- Gap Type: Bug / Missing Capability / Policy Gap / Tool Gap / Permission Gap / Performance / Documentation Gap / Improvement Suggestion
- Issue:
- Impact on agent performance:
- Suggested direction (optional):
- Status: Pending / Under Review / Approved / Applied / Rejected / Duplicate / Deferred
- Resolution Notes:
```

---

## السجل

## 2026-07-04 — TeraAgent — GAP-003

- Title: **غياب سجل امتثال (Compliance Record) لكل TASK-ID يربط Handback + Git diff + القواعد — ويفقده Monitor و Auditor مرجعاً موضوعياً للتدقيق**
- Agent: TeraAgent
- Gap Type: Process Gap / Missing Capability / Improvement Suggestion
- Issue: حالياً بعد تنفيذ مهمة، ينتج TeraAgent:
  1. Handback نصياً في TASK-ID file (ما يقول إنه فعله)
  2. Git diff (ما تغير فعلاً في التطبيق)
  3. لا يوجد Compliance Record يربط هذين المصدرين بالقواعد (Pre-Execution Gate PASS، التزام بـ Allowed Write Targets، عدم وجود secrets، إلخ)

  هذا يخلق ثلاث مشاكل:
  - Monitor لا يملك مرجعاً موحداً ليتحقق منه — يضطر لمقارنة Handback مع Git diff يدوياً بدون checklist واضح
  - Auditor لا يملك سجل امتثال ليحكم على الالتزام بالقواعد
  - TeraAgent نفسه قد يتجاوز خطوة أو ينسى توثيقها دون أن يكتشفه أحد لعدم وجود ورقة مطابقة إلزامية
- Impact on agent performance: يضعف فعالية Monitor و Auditor كرقيبين، ويبقي TeraAgent عرضة للانحراف عن القواعد تدريجياً مع طول الجلسات دون كشف.
- Suggested direction (optional): إلزام كل TASK-ID بـ Compliance Record (قائمة تفتيش امتثال) قبل الإغلاق، وأن يكون هذا السجل هو المرجع المعتمد لـ Monitor و Auditor للمطابقة مع Git diff. المقترح بالتفصيل:

  ```
  TASK-COD-XXX — Compliance Record
  ──────────────────────────────────
  □ Pre-Execution Gate: PASS (معرف في TASK-ID file)
  □ Allowed Write Targets: ملتزم
  □ Secrets check: لا أسرار في الملفات أو السجلات
  □ Design Source Decision: N/A (أو مصدر معتمد)
  □ Post-Execution Review: PASS
  □ PROJECT_ACTIVITY_LOG.md: محدث
  □ Handback: مسجل في TASK-ID file
  □ Git diff يطابق Handback: (يتحقق منه Monitor)
  □ Commands run: (إن وجدت — أدناه)

  Commands:
  - npx ... (إن وجد)
  ```

  القاعدة الإلزامية المقترحة (تضاف إلى .opencode/agents/tera.md §12 أو §13):
  ```
  No task may be Accepted or Closed without:
  1. Handback مسجل في TASK-ID file
  2. Compliance Record (gates checked + commands run)
  3. Git diff يطابق الـ Handback (يتحقق منه Monitor)
  ```

- Status: Applied
- Resolution Notes: تم تطبيق الحل عبر SCP-2026-07-04-026. تم تحديث `TERA_RUNTIME_PROTOCOLS.md` (قاعدة Compliance Record إلزامية)، `TERA_RUNTIME_TEMPLATES.md` (قالب §33)، `.opencode/agents/tera.md` §12 (تحديث قاعدة الإغلاق)، و `.opencode/agents/monitor.md` (إضافة مسؤوليات التحقق). الـ Compliance Record داخل ملف TASK-ID نفسه — لا ملفات جديدة.

## 2026-07-04 — TeraClientEngagementAgent — GAP-001

- Title: **تخطي بوابة اعتماد الفهم (Understanding Confirmation Gate) قبل إنتاج ملفات النطاق**
- Agent: TeraClientEngagementAgent
- Gap Type: Process Gap / Improvement Suggestion
- Issue: TCEA انتقل من الحوار الاستكشافي (أسئلة وأجوبة) مباشرة إلى إنتاج CLIENT_BRIEF.md و SCOPE_SUMMARY.md دون عرض ملخص الإجابات أولاً على Majed وطلب اعتمادها كفهم صحيح. هذا يخالف مبدأ "Spoken client input is not final until documented and confirmed" في TeraClientPolicy.md §2، ويخاطر ببناء ملفات نطاق على أساس غير مؤكد.
- Impact on agent performance: خطر بناء مستندات على فهم غير معتمد، مما قد يؤدي إلى إعادة عمل إذا تبين أن هناك سوء فهم. يضعف بوابات الحوكمة ويزيد احتمالية الأخطاء في التسليم لـ TeraAgent.
- Suggested direction (optional): إضافة خطوة إلزامية في تدفق العمل: بعد اكتشاف Domain 1 & 2 essentials وقبل إنتاج أي ملف نطاق، يجب على TCEA عرض ملخص إجابات معتمد على Majed بصيغة "هذا فهمي — هل هو صحيح؟" والحصول على تأكيد صريح قبل المتابعة. يمكن توثيق هذه الخطوة في CLIENT_INTAKE.md كحقل "Understanding Confirmed by Majed: Yes/No".
- Status: Applied
- Resolution Notes: Reviewed and confirmed by TeraSystemEvolutionAgent on 2026-07-04. Fixed via SCP-2026-07-04-018 by adding an explicit Understanding Confirmation Gate to `tera-system/TeraClientEngagement.md` and `.opencode/agents/tera-client-engagement.md`. Current application files were also remediated: `CLIENT_INTAKE.md` now records pending understanding confirmation, and `CLIENT_BRIEF.md` + `SCOPE_SUMMARY.md` were marked as non-baseline until Majed confirms the understanding summary.

## 2026-07-04 — TeraClientEngagementAgent — GAP-002

- Title: **غياب إطار تغطية Discovery إلزامي يمنع الانتقال المبكر إلى النطاق أو التسعير أو الهاندوف**
- Agent: TeraClientEngagementAgent
- Gap Type: Process Gap / Missing Capability / Improvement Suggestion
- Issue: بعد إصلاح Understanding Confirmation Gate، بقيت فجوة تشغيلية أعمق: TCEA يستطيع الانتقال من فهم عام مؤكد إلى ملفات النطاق أو Draft Quotation أو TERA_HANDOFF_PACKAGE دون إثبات أن مجالات الاكتشاف الأساسية كلها قد غُطيت أو صُنفت بوضوح. المشكلة ظهرت في تجربة Alfares حيث كانت بعض المجالات مغطاة جزئياً أو غائبة دون Discovery Completeness Matrix أو Gate تمنع الانتقال.
- Impact on agent performance: يضعف الدور الاستشاري لـ TCEA، ويخاطر بإنتاج Scope / Pricing / Handoff على أساس تغطية غير مكتملة، ويزيد احتمالية التخمين لاحقاً لدى ApplicationBlueprintAgent و TeraAgent.
- Suggested direction (optional): إضافة TCEA Mandatory 13-Domain Client Discovery Framework + Discovery Coverage Summary + Discovery Coverage Gate + Quotation Readiness Gate + Tera Handoff Readiness Gate مع قاعدة Mandatory Coverage ≠ Mandatory Deep Interview.
- Status: Applied
- Resolution Notes: Reviewed by TeraSystemEvolutionAgent and implemented on 2026-07-04 via SCP-2026-07-04-022. TCEA source-of-truth and runtime were updated to require the 13-domain discovery framework, `DISCOVERY_COVERAGE_SUMMARY.md`, quotation/handoff readiness gates, and anti-bloat depth scaling. Supporting references were also updated (`TeraApplicationQuestionBank.md`, `TeraClientPolicy.md`, `TeraPricingPolicy.md`, `TeraApplicationBlueprint.md`, `TeraPolicyMap.md`, `TERA_RUNTIME_TEMPLATES.md`).

## 2026-07-04 — TeraClientEngagementAgent — GAP-004

- Title: **6 اقتراحات تحسين لملف TCEA المصدر والملفات المرتبطة**
- Agent: TeraClientEngagementAgent
- Gap Type: Improvement Suggestion / Policy Gap
- Issue: TCEA حدد 6 فجوات في ملفه المصدر وبعض الملفات المرتبطة:
  1. عدم تطابق بين Handoff Readiness Gate (§3.6.1) و Handoff Package Fields (§6.2) — 8 حقول ناقصة من الـ Gate
  2. لا يوجد قالب تنسيقي لـ DISCOVERY_COVERAGE_SUMMARY.md — كل عميل يأخذ تنسيقاً مختلفاً
  3. لا توجد عملية لطلبات توضيح من ApplicationBlueprintAgent — §5.2 يعالج TeraAgent فقط
  4. لا توجد قاعدة لتحديث DISCOVERY_COVERAGE_SUMMARY بعد تغير حالة Discovery
  5. المجال 13 (Acceptance, Commercials & Warranty) مركب جداً ويُعامل كحقل واحد
  6. ميزانية الأسئلة (Question Budget) غير مذكورة في ملف TCEA
- Impact on agent performance: كل فجوة تضعف جانباً محدداً من دقة أو تناسق عمل TCEA
- Suggested direction (optional): جميع الاقتراحات وردت من TCEA نفسه مع حلول مقترحة لكل منها
- Status: Applied
- Resolution Notes: تم تحليل الـ 6 اقتراحات بواسطة TeraSystemEvolutionAgent ووجدت جميعها حقيقية. تم تطبيقها عبر SCP-2026-07-04-027:
  1. §3.6.1: استبدال قائمة الـ 17 بند منفصلة بإشارة إلى §6.2 كقائمة كاملة
  2. §3.2.3: إضافة ملاحظة المجال 13 المركب (3 جوانب داخلية)
  3. §3.2.4: إضافة قاعدة تحديث Discovery Coverage بعد اعتمادها
  4. §3.2.5: إضافة Question Budget (Small 10-15, Medium 20-35, Complex deeper)
  5. §5.2: إضافة مسار توضيح لـ ApplicationBlueprintAgent (نفس آلية TeraAgent)
  6. TERA_RUNTIME_TEMPLATES.md §35: إضافة قالب Discovery Coverage Summary بجدول 13 صفاً

## 2026-07-04 — TeraClientEngagementAgent + Monitor — GAP-005

- Title: **غياب بروتوكول الشك (صلاحية "لا أعرف") + Self-Check الداخلي + مراجعة Discovery الخارجية**
- Agent: TeraClientEngagementAgent, Monitor
- Gap Type: Process Gap / Missing Capability
- Issue: TCEA اعترف بنفسه بثلاث فجوات هيكلية:
  1. لا صلاحية صريحة ليقول "لا أعرف" بدلاً من التخمين
  2. لا Self-Check داخلي يمنع الاستعجال — الـ Gates الحالية خارجية فقط (تعتمد على Majed)
  3. لا مراجعة خارجية غير متوقعة للـ Discovery — Monitor لا يراجع DISCOVERY_COVERAGE_SUMMARY.md
- Impact on agent performance: TCEA يخمن في domains غير مؤكدة، ينتقل لمخرجات دون تثبت، ولا يوجد من يراجع اكتشافاته
- Suggested direction (optional): Self-Check Protocol (3 أسئلة قبل Complete) + Uncertainty Protocol + تمديد Monitor للمراجعة العشوائية
- Status: Applied
- Resolution Notes: تم التحليل والتطبيق عبر SCP-2026-07-04-028:
  1. TeraClientEngagement.md §3.2.6: Self-Check Protocol (المصدر / تأكيد Majed / الخطورة) + قاعدة الحظر
  2. TeraClientEngagement.md §3.2.7: Uncertainty Protocol (3 حالات توقف + UNCERTAINTY_NOTICE + Websearch عند الشك)
  3. TERA_RUNTIME_TEMPLATES.md §35: إضافة 3 أعمدة Self-Check إلى Domain Coverage Matrix
  4. monitor.md: إضافة Random Discovery Audit (بأمر Majed)

## 2026-07-04 — ApplicationBlueprintAgent — GAP-006

- Title: **غياب 5 ضوابط هيكلية في ملف ABA — بوابة النزاهة، مؤشرات الانحراف، التدقيق الذاتي، التريث، و"لا أعلم"**
- Agent: ApplicationBlueprintAgent
- Gap Type: Missing Capability / Process Gap / Improvement Suggestion
- Issue: ABA حلل ملفه الحالي وحدد 5 فجوات:
  1. لا بروتوكول "لا أعلم" إلزامي → تخمين بدون اعتراف
  2. لا تدقيق ذاتي قبل التسليم → توصيات غير دقيقة
  3. لا مؤشرات انحراف أثناء العمل → انحراف بدون تنبيه
  4. لا قاعدة إيقاف عند نقص المعلومات → إنتاج رغم عدم الاكتمال
  5. لا شرط تريث → استعجال بدون داعٍ
- Impact on agent performance: يضعف موثوقية blueprint ويزيد احتمالية تسليم خرائط تطبيقية غير دقيقة أو مبنية على افتراضات غير معلنة.
- Suggested direction (optional): إضافة 3 أقسام جديدة إلى TeraApplicationBlueprint.md (بوابة النزاهة، مؤشرات الانحراف، التدقيق الذاتي) + تعديل المخرجات + مزامنة الرنتايم.
- Status: Applied
- Resolution Notes: تم اعتماد التحسينات عبر SCP-2026-07-04-031. تم رفض طلب إنشاء ملف TERA_AGENT_QUALITY_CONTROL.md (Anti-Bloat) — دُمج المحتوى في TeraApplicationBlueprint.md. تم تنفيذ SCP-031 بتاريخ 2026-07-04.

## 2026-07-04 — DesignReviewer — GAP-007

- Title: **3 فجوات في DesignReviewer: غياب المعاينة البصرية الفعلية + غياب فحص توكينز منهجي + مرجع WORKSPACE_GOVERNANCE_MODEL.md ميت**
- Agent: DesignReviewer
- Gap Type: Missing Capability / Process Gap / Stale Reference
- Issue: ناقد حدد 3 مشاكل في دوره:
  1. لا يستطيع رؤية الواجهة المنفذة بصرياً — دوره "مراجع تصميم" لكنه "أعمى" (webfetch يحول إلى نص فقط).
  2. لا توجد عملية منهجية لفحص توكينز التصميم — عنده grep/glob لكن بدون خطوات واضحة.
  3. WORKSPACE_GOVERNANCE_MODEL.md مذكور في ملفه كأول ملف قراءة (سطر 43) لكنه غير موجود — وهذا يؤثر على 3 عملاء (ناقد، مدقق، رقيب).
- Impact on agent performance: يحد من فعالية ناقد في اكتشاف انحرافات التصميم قبل أن تصل إلى الإنتاج. مرجع ميت يربك تدفق القراءة.
- Suggested direction (optional):
  1. رفع webfetch من ask إلى allow + بروتوكول معاينة محدودة (webfetch لـ HTML، والمعاينة البصرية الكاملة لـ Majed للتطبيقات المعقدة).
  2. إضافة خطوة Design Token Verification كـ Process (grep/glob — لا أداة جديدة).
  3. حذف WORKSPACE_GOVERNANCE_MODEL.md من العملاء الثلاثة وإنشاء Source of Truth مخصص لـ ناقد (TeraDesignReviewer.md).
- Status: Applied
- Resolution Notes: تم تنفيذ جميع التحسينات عبر SCP-2026-07-04-032. تم رفع webfetch إلى allow، إضافة Limited Preview Protocol و Design Token Verification، حذف WORKSPACE_GOVERNANCE_MODEL.md من 3 عملاء، وإنشاء TeraDesignReviewer.md كـ Source of Truth.

## 2026-07-04 — DesignReviewer — GAP-008

- Title: **4 فجوات في DesignReviewer: غياب قاعدة معايير موحدة + غياب المساعدة البصرية + غياب البروتوتايب + فحص توكينز محدود**
- Agent: DesignReviewer
- Gap Type: Missing Capability / Process Gap / Improvement Suggestion
- Issue: بعد SCP-032، ناقد يستطيع المعاينة البصرية عبر Playwright MCP ويملك Source of Truth مستقل. لكن تبقى 4 فجوات:
  1. لا قاعدة معايير تدقيق منهجية — ناقد يعتمد على اجتهاده الشخصي في كل مراجعة.
  2. لا بروتوكول لطلب المساعدة البصرية من Majed — عندما يحتاج تأكيداً لا يستطيع الحصول عليه آلياً.
  3. لا قدرة على بناء بروتوتايب HTML/CSS للمراجعة قبل التنفيذ.
  4. فحص التوكينز لا يغطي التسلسل الهرمي (3-layer: Primitive → Semantic → Component).
- Impact on agent performance: يحد من دقة المراجعة واتساقها، ويترك ناقد بدون أدوات للتعامل مع الحالات التي لا يستطيع فيها المتصفح أو webfetch تقديم إجابة كاملة.
- Suggested direction (optional):
  1. إنشاء DESIGN_REVIEW_STANDARDS.md — قاعدة معايير ب 9 أقسام.
  2. إضافة Visual Assistance Protocol — طلب صور/تأكيد من Majed.
  3. إضافة Prototype Protocol — بناء HTML/CSS مؤقت للمراجعة.
  4. توسيع فحص التوكينز ليشمل 3-layer architecture.
  5. رفع write من deny إلى ask للسماح ببناء البروتوتايب.
- Status: Applied
- Resolution Notes: تم تنفيذ جميع التحسينات عبر SCP-2026-07-04-033. تم إنشاء DESIGN_REVIEW_STANDARDS.md (9 أقسام)، تحديث TeraDesignReviewer.md (بروتوتايب + مساعدة بصرية + توكينز معمّق)، تحديث design-reviewer.md (write: ask + 4 بروتوكولات + Output format مطوّر)، وتحديث ENGINEERING_AGENT_RESPONSIBILITIES.md §11.

## 2026-07-04 — Monitor — GAP-009

- Title: **غياب صلاحية Bash/Git — لا يستطيع Monitor مقارنة Handback vs Git Diff رغم أن هذا مطلوب في تعريفه**
- Agent: Monitor
- Gap Type: Permission Gap / Process Gap
- Issue: تعريف Monitor (سطر 62) يلزمه بـ "Cross-check Handback vs Git diff for each closed task"، لكن صلاحية `bash: deny` تمنعه من تشغيل أي أمر git. لا يستطيع:
  1. تشغيل `git diff --name-only` لمقارنة الملفات المغيَّرة مع Handback
  2. تشغيل `git log --oneline` لتتبع تاريخ التغييرات
  3. تشغيل `git show --stat` لفحص الـ commits
  4. تنفيذ أي أمر شل ضروري للتدقيق الفني
- Impact on agent performance: يجعل البند الأساسي (Cross-check Handback vs Git Diff) غير قابل للتنفيذ. يبقى Monitor رقيباً وثائقياً فقط بدون قدرة على التحقق الفعلي من التطابق.
- Suggested direction (optional): رفع `bash: deny` → `bash: ask` + إضافة Git Audit Protocol يحدد الأوامر المسموح بها (git diff, git log, git show — قراءة فقط).
- Status: Applied
- Resolution Notes: تم تنفيذ الحل عبر SCP-2026-07-04-034. تم رفع bash إلى ask في monitor.md، إضافة Git Audit Protocol مع قائمة الأوامر المسموح بها وانضباط ذاتي لقراءة فقط.

## 2026-07-04 — Monitor — GAP-010

- Title: **غياب Source of Truth وميثاق تدقيق لـ Monitor (رقيب) — لا دستور مكتوب، لا قواعد ثابتة، لا تدرج مرجعي، لا صلاحية رفض**
- Agent: Monitor
- Gap Type: Missing Capability / Process Gap / Documentation Gap
- Issue: Monitor (رقيب) لا يملك Source of Truth نظامياً يحدد:
  1. دستور عمله وقواعده الثابتة — يعمل باجتهاد شخصي.
  2. تدرجاً هرمياً للمراجع (أي ملف يعلو أي ملف عند التعارض).
  3. صلاحية رفض خطة معيبة — يوافق على انحرافات بدون تفويض واضح.
  4. آلية تدقيق تراكمي — يعيد من الصفر في كل جلسة.
  5. علاقته مع بقية العملاء في سياق التدقيق.
  بالمقابل: DesignReviewer (ناقد) حصل على TeraDesignReviewer.md في SCP-032.
- Impact on agent performance: يبقى Monitor عرضة للاجتهاد الشخصي، بدون دستور يلتزم به، وبدون صلاحية لرفض خطة معيبة — وهذا خطر على المنظومة لأنه دور حساس.
- Suggested direction (optional): إنشاء `tera-system/TeraMonitor.md` يجمع: الهوية، الغرض، التدرج الهرمي للمراجع، القواعد السبعة الثابتة، صلاحية الرفض، العلاقات، والتحسين المستمر.
- Status: Applied
- Resolution Notes: تم تنفيذ الحل عبر SCP-2026-07-04-035. تم إنشاء TeraMonitor.md (8 أقسام) في tera-system/، تحديث monitor.md بإضافة System Reference واختصار What you do، تحديث ENGINEERING_AGENT_RESPONSIBILITIES.md §6، وتحديث TeraPolicyMap.md بإضافة إدخال Monitor.

## 2026-07-05 — Auditor — GAP-011

- Title: **غياب Source of Truth لـ Auditor (مدقق) — لا دستور مكتوب، لا منهجية تدقيق متدرجة، لا تدرج مرجعي، لا بروتوكول عدم يقين، لا آلية تراكم**
- Agent: Auditor
- Gap Type: Missing Capability / Process Gap / Documentation Gap
- Issue: Auditor (مدقق) لا يملك Source of Truth نظامياً رغم أن كل عميل حوكمة آخر لديه:
  - Monitor → TeraMonitor.md ✅
  - DesignReviewer → TeraDesignReviewer.md ✅
  - TCEA → TeraClientEngagement.md ✅
  - **Auditor → ❌ لا يوجد**

  الفجوات الناتجة:
  1. لا دستور عمل مكتوب — قراراته اجتهادية بدون معايير واضحة.
  2. لا منهجية تدقيق متدرجة — يبدأ من الصفر في كل دورة.
  3. لا جدول تصنيف نتائج موحد — يستخدم 3 حالات فقط (PASS/NEEDS_FIX/BLOCKED) بدون DEFERRED.
  4. لا بروتوكول عدم يقين — قد يخمن أو يتجاوز.
  5. لا آلية تراكم — لا يبني على تدقيقات سابقة.
  6. ENGINEERING_AGENT_RESPONSIBILITIES.md §5 تعريفه مختصر جداً (13 سطراً فقط).
- Impact on agent performance: يضعف موثوقية واتساق تدقيقاته مع مرور الوقت. يمنعه من التراكم المعرفي. يجعله عرضة للاجتهاد الشخصي.
- Suggested direction (optional): إنشاء `tera-system/TeraAuditor.md` كـ Source of Truth بـ 10 أقسام (الهوية، الموقع، الغرض، المراجع، منهجية 6 مراحل، جدول تصنيف، بروتوكول عدم يقين وبحث، تراكم، علاقات، تحسين مستمر). رفض AUDIT_TRAIL.md (استخدام PROJECT_ACTIVITY_LOG.md).
- Status: Applied
- Resolution Notes: تم تنفيذ الحل عبر SCP-2026-07-05-036. تم إنشاء TeraAuditor.md (10 أقسام) في tera-system/، تحديث auditor.md بإضافة System Reference + بروتوكول التراكم + تحديث Output Format (DEFERRED)، تحديث ENGINEERING_AGENT_RESPONSIBILITIES.md §5 (توسيع كامل: منهجية + تراكم + بروتوكول عدم يقين + مرجع TeraAuditor.md)، وتحديث TeraPolicyMap.md بإضافة إدخال Auditor.

## 2026-07-12 — TeraAgent — GAP-013

- Title: **TeraAgent كتب ملفات محددة للمشروع في المجلد الرئيسي (project-preparation/ و project-control/) بدلاً من مجلد تطبيق العميل**
- Agent: TeraAgent
- Gap Type: Process Gap / Policy Gap
- Issue: عند استلام مشروع WarehouseDashboard، كتب TeraAgent الملفات التالية في المجلد الرئيسي بدلاً من مجلد تطبيق العميل:
  1. `project-preparation/00_PROJECT_INPUTS.md` — ملخص مُوحّد
  2. `project-preparation/TERA_PROJECT_DECISION.md` — قرار المشروع
  3. `project-control/PROJECT_STATE.md` — حالة المشروع (عكّس القالب الأصلي)
  4. `project-control/TERA_ACTIVE_CONTEXT.md` — سياق الجلسة (عكّس القالب الأصلي)
  5. `project-control/DECISIONS_LOG.md` — سجل القرارات (عكّس القالب الأصلي)
  6. `project-control/PROJECT_ACTIVITY_LOG.md` — سجل النشاط (أضاف إدخال مشروع)

  المجلدات `project-preparation/` و `project-control/` في الجذر تحتوي ملفات نظام عامة (قوالب، بروتوكولات، سجلات نظام). الملفات الخاصة بالمشاريع يجب أن تذهب إلى `clients/.../applications/APP-xxx/project-preparation/` و `clients/.../applications/APP-xxx/project-control/`.
- Impact on agent performance: يخلط بين ملفات النظام وملفات المشاريع. يجعل من الصعب فصل مشاريع مختلفة. يعرض ملفات القوالب للتعويض بالبيانات الخاصة بالمشروع.
- Suggested direction (optional): قاعدة صريحة في `.opencode/agents/tera.md` أو `TERA_RUNTIME_PROTOCOLS.md`:
  ```
  TeraAgent writes ONLY inside client application folders:
  clients/.../applications/APP-xxx/project-preparation/
  clients/.../applications/APP-xxx/project-control/

  Root-level project-preparation/ and project-control/ = system templates and protocols ONLY.
  ```
- Status: Approved (SCP-2026-07-12-092)
- Resolution Notes: تم نقل الملفات إلى `clients/الماجد-لادارة-المستودعات/applications/APP-WarehouseDashboard/project-preparation/` و `project-control/`. تم استعادة ملفات القالب الأصلي في الجذر. تم إنتاج SCP-2026-07-12-092 لتعديل `tera.md` و `TERA_RUNTIME_PROTOCOLS.md` و `TERA_RUNTIME_CHECKLISTS.md` لإضافة قاعدة Write Location Rule تمنع تكرار المشكلة.

## 2026-07-07 — DomainExpertAgent + DomainResearchAgent — GAP-012

- Title: **DomainExpertAgent و DomainResearchAgent يفتقران لملفات تعريف مستقلة في `.opencode/agents/` مع Dual Mode (Software + Consulting)**
- Agent: DomainExpertAgent, DomainResearchAgent
- Gap Type: Missing Capability / Documentation Gap / Process Gap
- Issue: تقييم TCEA (تم تحليله من حارس) كشف أن DomainExpertAgent موجود كإشارة فقط في TeraSubAgents.md §6.13 لكنه ليس عميلاً قابلاً للاستدعاء الفعلي. لا يوجد له ملف `.opencode/agents/domain-expert-agent.md` ولا ملف `domain-research-agent.md`. بالإضافة إلى ذلك:
  1. كلاهما يدعم فقط Software Mode (تصنيف MVP) ولا يدعمان Consulting Mode (تصنيف معرفي) رغم أن TCEA يحتاجهم لاكتشاف Value-Added Proposals.
  2. DomainExpertAgent Consulting Mode يحتاج Knowledge Structure + Gap Analysis كمخرجات إضافية.
  3. DomainResearchAgent Consulting Mode يحتاج بروتوكول بحث مختلف (تقارير متعددة، ترتيب مصادر، معالجة أخطاء 404).
  4. TeraSubAgents.md §6.12/§6.13 لا يذكران ملفات العملاء الجديدة ولا وضعي التشغيل.
  5. TERA_RUNTIME_TEMPLATES.md §10 لا يحتوي قوالب Consulting Mode.
  6. TERA_RUNTIME_PROTOCOLS.md §12 (D) لا يميز بين Software Mode و Consulting Mode.
- Impact on agent performance: DomainExpertAgent و DomainResearchAgent غير جاهزين للاستدعاء الفعلي. TCEA لا يستطيع استخدامهما في Consulting Mode لاكتشاف الفرص التجارية. منظومة Tera لا تملك وكيل تحليل مجال متكامل.
- Suggested direction (optional): إنشاء ملفات `.opencode/agents/domain-research-agent.md` و `domain-expert-agent.md` مع Dual Mode. تحديث TeraSubAgents.md §6.12/§6.13. تحديث TERA_RUNTIME_TEMPLATES.md §10. تحديث TERA_RUNTIME_PROTOCOLS.md §12(D). كلها ضمن SCP-2026-07-07-090.
- Status: Applied
- Resolution Notes: تم تنفيذ الحل عبر SCP-2026-07-07-090. تم إنشاء `.opencode/agents/domain-research-agent.md` و `domain-expert-agent.md` مع Dual Mode و Consulting Mode outputs. تم تحديث TeraSubAgents.md §6.12 و §6.13. تحديث TERA_RUNTIME_TEMPLATES.md و TERA_RUNTIME_PROTOCOLS.md جارٍ.
