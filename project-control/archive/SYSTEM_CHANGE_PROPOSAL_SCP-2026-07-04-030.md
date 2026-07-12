# SYSTEM_CHANGE_PROPOSAL

## SCP-2026-07-04-030 — Replace ExecutionPreparationAgent with SoftwareDesignerAgent (6 Files)

**Request Type:** Architecture Cleanup / Agent Replacement / Cross-System Update

---

### Problem

`ExecutionPreparationAgent` (EPA) was **functionally replaced** by `SoftwareDesignerAgent` (SDA) in SCP-012, but the actual system files were **never updated**. Only `tera-software-designer.md` says `"(removed)"`. All other files still reference EPA as if it's an active agent, while SDA is missing from those same files.

This creates:

1. **Stale references** — TeraAgent, TeraSubAgents, and runtime protocols tell Tera to use an agent that doesn't exist.
2. **Missing SDA entries** — Activation matrix, permission model, and helper lists don't list SDA where EPA was.
3. **Systemic inconsistency** — 6 files contradict `tera-software-designer.md`.

---

### Evidence

| # | File | EPA Count | SDA Present? |
|---|------|-----------|--------------|
| 1 | `AGENT_ACTIVATION_MATRIX.md` | 4 | ❌ No |
| 2 | `AGENT_PERMISSION_MODEL.md` | 1 | ❌ No |
| 3 | `TeraAgent.md` (Helper list) | 1 | ❌ No |
| 4 | `TeraSubAgents.md` (§6.9) | Full section | ❌ No dedicated section |
| 5 | `TeraPreExecutionGate.md` | 2 | ❌ No |
| 6 | `TERA_RUNTIME_PROTOCOLS.md` | 4 | ❌ No |

---

### Affected Files

| File | Change Type | EPA → SDA |
|------|-------------|-----------|
| `tera-system/AGENT_ACTIVATION_MATRIX.md` | UPDATE | Replace 4 EPA references + add SDA row |
| `tera-system/AGENT_PERMISSION_MODEL.md` | UPDATE | Replace EPA row with SDA row |
| `tera-system/TeraAgent.md` | UPDATE | Replace EPA with SDA in Helper list |
| `tera-system/TeraSubAgents.md` | UPDATE | Replace §6.9 EPA with SDA section |
| `tera-system/TeraPreExecutionGate.md` | UPDATE | Replace 2 EPA references |
| `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md` | UPDATE | Replace 4 EPA references |

#### 7. TeraSubAgents.md — 2 additional references

**Line 94** (Model Capability Gate list):
```diff
-- `ExecutionPreparationAgent`
+- `SoftwareDesignerAgent`
```

**Line 1014** (QAAndAcceptanceAgent limits):
```diff
-- لا يجهز Task Package تنفيذية بدل `ExecutionPreparationAgent` إلا إذا كلفه Tera صراحةً كحل مؤقت.
+- لا يجهز Technical Specification بدل `SoftwareDesignerAgent` إلا إذا كلفه Tera صراحةً كحل مؤقت.
```

**Not modified** (historical records — safe):
- `project-control/archive/RESPONSE_TO_TEAM_REVIEW_SCP-2026-07-03-015.md`
- `project-control/archive/SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-03-016.md`
- `.opencode/agents/tera-software-designer.md` (already says "removed")

---

### Proposed Change

#### 1. AGENT_ACTIVATION_MATRIX.md

**§2.2 (line 63):** Replace EPA row with SDA row:

```diff
-| ExecutionPreparationAgent | `EXECUTION_PREPARATION_AGENT` | `COMPLEXITY_SIGNAL`: مهمة متعددة العملاء، أو تتجاوز 3 ملفات، أو تحمل مخاطر | 5–6 | إذا كانت المهمة بسيطة ويمكن لـ Tera تجهيز Task Package مباشرة | `TASK-ID` المقترح + ملفات التحليل المعتمدة |
+| SoftwareDesignerAgent | `SOFTWARE_DESIGNER_AGENT` | `COMPLEXITY_SIGNAL`: مهمة متعددة العملاء، أو تتجاوز 3 ملفات، أو تحمل مخاطر، أو تحتاج Technical Specification | 5–6 | إذا كانت المهمة بسيطة ويمكن لـ Tera تجهيز Technical Specification مباشرة | ملفات التحليل المعتمدة + `TECHNICAL_SPECIFICATION.md` |
```

**§3.2 Medium Project (line 117):** Replace
```diff
-| ExecutionPreparationAgent | اختياري | عند تعقيد المهمات |
+| SoftwareDesignerAgent | اختياري | عند تعقيد المهمات |
```

**§3.3 ERP Project (line 140):** Replace
```diff
-| ExecutionPreparationAgent | نعم | مهام متعددة ومعقدة |
+| SoftwareDesignerAgent | نعم | مهام متعددة ومعقدة تحتاج Technical Specification |
```

**§3.4 SaaS Project (line 166):** Replace
```diff
-| ExecutionPreparationAgent | اختياري | عند تعقيد المهمات |
+| SoftwareDesignerAgent | اختياري | عند تعقيد المهمات |
```

#### 2. AGENT_PERMISSION_MODEL.md

**Line 211:** Replace EPA row with SDA row:

```diff
-| ExecutionPreparationAgent | `EXECUTION_PREPARATION_AGENT` | `WRITE_DOCS` | إلى `PLAN_ONLY` إذا ما زالت الخطة غير ناضجة | يجهز Task Packages فقط |
+| SoftwareDesignerAgent | `SOFTWARE_DESIGNER_AGENT` | `WRITE_DOCS` | إلى `PLAN_ONLY` إذا ما زالت الخطة غير ناضجة | يجهز Technical Specification للمهام فقط |
```

#### 3. TeraAgent.md

**Line 1078:** Replace EPA with SDA in Helper Agents list:

```diff
-Helper Agents (authorized now): `ProjectControlAgent`, `ExecutionPreparationAgent`, `QualityReviewCoordinatorAgent`, `PlanComplianceReviewAgent`, `DocumentationHandoverAgent`.
+Helper Agents (authorized now): `ProjectControlAgent`, `SoftwareDesignerAgent`, `QualityReviewCoordinatorAgent`, `PlanComplianceReviewAgent`, `DocumentationHandoverAgent`.
```

#### 4. TeraSubAgents.md

**§6.9 (lines 1032-1094):** Replace the entire ExecutionPreparationAgent section with SoftwareDesignerAgent:

The new section should describe SDA's actual role (from `tera-software-designer.md`): produces `TECHNICAL_SPECIFICATION.md`, checks Lifecycle Header, defines scope/allowed write targets/acceptance criteria for each task, and is mandatory for impactful tasks.

#### 5. TeraPreExecutionGate.md

**Line 78:** Replace:
```diff
- - `ExecutionPreparationAgent`
+ - `SoftwareDesignerAgent`
```

**§ExecutionPreparationAgent Preparation Rule (lines 778-782):** Replace:
```diff
-### ExecutionPreparationAgent Preparation Rule
-Tera may use `ExecutionPreparationAgent` to prepare the initial task package before delegation.
-This agent may draft scope, references, write targets, acceptance criteria, risk notes, and reviewer suggestions only.
+### SoftwareDesignerAgent Preparation Rule
+Tera may use `SoftwareDesignerAgent` to prepare the technical specification before delegation.
+This agent produces `TECHNICAL_SPECIFICATION.md` covering scope, references, write targets, acceptance criteria, risk notes, and reviewer suggestions.
```

#### 6. TERA_RUNTIME_PROTOCOLS.md

**Decision Matrix (line 338):**
```diff
-| Multi-agent, >3 files, Backend+Frontend, scope-drift prone, or needs detailed acceptance criteria / write targets | `ExecutionPreparationAgent` |
+| Multi-agent, >3 files, Backend+Frontend, scope-drift prone, or needs detailed acceptance criteria / write targets | `SoftwareDesignerAgent` |
```

**Anti-over-delegation example (line 368):**
```diff
-- Bad default pattern: `Tera -> ExecutionPreparationAgent -> EngineeringAgent -> FrontendAgent -> SecurityAgent -> QAAndAcceptanceAgent -> ProjectControlAgent -> QualityReviewCoordinatorAgent`
+- Bad default pattern: `Tera -> SoftwareDesignerAgent -> EngineeringAgent -> FrontendAgent -> SecurityAgent -> QAAndAcceptanceAgent -> ProjectControlAgent -> QualityReviewCoordinatorAgent`
```

**Escalation example (line 381):**
```diff
-- Direct task → needs `ExecutionPreparationAgent`
+- Direct task → needs `SoftwareDesignerAgent`
```

**Helper agent authority (line 392):**
```diff
-- `ExecutionPreparationAgent`: prepares task packages only. Does not decide scope, timing, delegation, approval, acceptance, or closure.
+- `SoftwareDesignerAgent`: prepares technical specifications only. Does not decide scope, timing, delegation, approval, acceptance, or closure.
```

---

### Why This Is Necessary

1. **Systemic truth** — الدليل يقول EPA غير موجود، لكن 6 ملفات تخالف ذلك. هذا أقوى مصدر للخطأ في المنظومة حالياً.
2. **SDA غير متكامل** — SDA مفعل في `.opencode/agents/` لكن لا يمكن لـ Tera تفويضه حسب المصفوفة لأنها لا تعرفه.
3. **تضليل العملاء** — TeraAgent يشير إلى EPA في قائمة Helper Agents، لكن إذا حاول استخدامه سيفشل.
4. **استكمال SCP-012** — SCP-012 قال "SDA replaces EPA" لكن الاستبدال لم يكتمل إلا جزئياً.

---

### Rejected Alternatives

| البديل | سبب الرفض |
|--------|-----------|
| إبقاء EPA كـ Deprecated دون حذف | يبقي 6 مراجع قديمة ويضيف عبء توثيق "هذا قديم" — نصف حل |
| إنشاء SCP منفصل لكل ملف | 6 SCPs لنفس الهدف = تضخم إداري غير مبرر |
| حذف EPA بدون إضافة SDA | يترك المصفوفة بدون عميل للمهام المعقدة — خطر |

---

### Anti-Bloat Check

| السؤال | الإجابة |
|--------|---------|
| ما المشكلة التي تحلها؟ | 6 ملفات تخالف الواقع — EPA معدوم و SDA موجود لكن غير مدرج |
| لماذا لا يكفي تعديل ملف موجود؟ | كل التعديلات في ملفات موجودة — **لا ملفات جديدة** |
| لماذا لا يكفي عميل موجود؟ | 6 ملفات تحتاج تحديث — عميل واحد لا يحل التناقضات |
| هل الإضافة ستقلل التعقيد أم تزيده؟ | **تقلل التعقيد** — تزيل 14 reference قديمة وتضيف 7 SDA references دقيقة |
| الأثر على استهلاك التوكنز؟ | **أقل** — SDA موجود فعلاً، هذه التعديلات تجعله usable بدون تكرار |
| هل توجد طريقة أصغر؟ | لا — كل ملف من الـ 6 يحتاج تغييراً محدداً ليتماشى مع الواقع |

---

### Risk

| المخاطرة | الاحتمال | التأثير | التخفيف |
|-----------|----------|---------|---------|
| مهمة حالياً تستخدم EPA في تدفقها | معدوم — EPA لم يُستخدم فعلياً منذ SCP-012 | لا تأثير | EPA كان "removed" في الممارسة |
| SDA دوره أوسع من EPA (Technical Specification vs Task Package) | مؤكد — وهذا مقصود | الأدوار متوافقة، SDA يغطي أكثر | الفرق موثق في SDA definition |
| تفويت reference في ملف غير الـ 6 | منخفض — فحصنا كل الملفات | فجوة صغيرة | الفحص الشامل غطى كل الـ 24 occurrence |

---

### Rollback Plan

1. `AGENT_ACTIVATION_MATRIX.md`: إعادة EPA وحذف SDA في 4 أماكن
2. `AGENT_PERMISSION_MODEL.md`: إعادة EPA وحذف SDA
3. `TeraAgent.md`: إعادة EPA بدل SDA في قائمة Helper Agents
4. `TeraSubAgents.md`: إعادة §6.9 القديم
5. `TeraPreExecutionGate.md`: إعادة reference EPA في مكانين
6. `TERA_RUNTIME_PROTOCOLS.md`: إعادة 4 references إلى EPA

---

### Approval Required

- [x] Majed — موافقة مبدئية ("نعم اريد فتح SCP")
- [ ] تأكيد نهائي بعد قراءة الـ Proposal
