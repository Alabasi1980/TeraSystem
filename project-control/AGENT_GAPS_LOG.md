# AGENT_GAPS_LOG.md

## الغرض

سجل مركزي خاص بفجوات ومشكلات واقتراحات تطوير العملاء الأساسيين في منظومة Tera.

هذا السجل مخصص لتطوير منظومة Tera وعملائها الأساسيين فقط، وليس لتتبع أعمال تطبيقات العملاء أو مهام المشاريع.

يستخدمه `TeraSystemEvolutionAgent` كمصدر أساسي لتحسين العملاء، لأن أفضل تطوير هو تصحيح الأخطاء والفجوات الفعلية.

يشمل ذلك صراحةً:

- `TeraAgent`
- `TeraClientEngagementAgent`
- `TeraSystemEvolutionAgent`
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
