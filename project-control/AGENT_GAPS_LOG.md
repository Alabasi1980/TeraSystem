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

لا توجد إدخالات حالياً.
