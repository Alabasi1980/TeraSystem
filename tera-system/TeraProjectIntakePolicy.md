# Project Intake Policy

> **ملاحظة نظامية:** هذا الملف يعرّف سياسة الـ intake و discovery قبل بدء TeraAgent في التخطيط والتنفيذ.
>
> في النموذج التشغيلي الحالي، `TeraClientEngagementAgent` هو المنفذ الأساسي لهذه السياسة للمشاريع الداخلية والخارجية عبر Majed، ثم ينتج `TERA_HANDOFF_PACKAGE.md` ويسلّمها إلى TeraAgent.
>
> `TeraAgent` لا يدير Discovery مباشرة، بل يبدأ بعد Handoff معتمد وينتقل إلى `Phase 2 — Project Decision`.
> راجع `tera-system/TeraClientEngagement.md` و `tera-system/TeraAgent.md §1.2` و `§4.0`.


## 1. Purpose

This policy defines the intake process before formal handoff to TeraAgent. In the current operating model, the responsible intake agent is `TeraClientEngagementAgent` acting through Majed.

The intake process has two sequential stages:

```
Stage 1: Client Discovery Mode  ← Open conversation + understanding summary (with Majed)
Stage 2: Smart Interview         ← Structured adaptive questioning (only if gaps remain)
```

These are defined in `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md` Section 18, with the question bank at `tera-system/TeraApplicationQuestionBank.md`.

The **Project Intake Gate** checks whether the information collected during these stages is ready for formal preparation.

## 2. Required Intake Files

The minimum intake package is:

- `project-inputs/01_APPLICATION_IDEA.md`
- `project-inputs/02_TECHNICAL_CONTEXT.md`

For every new application, these files must be created inside the active application workspace:

```text
project-inputs/
```

Root-level `project-inputs/` is only a template/bootstrap area unless the user explicitly approves operating without an isolated application workspace.

> **ملاحظة:** للمشاريع الخارجية، ملفات العميل (CLIENT_PROFILE.md، CONTACTS.md، إلخ) يديرها `TeraClientEngagementAgent`. TeraAgent لا ينشئها.

## 3. Application Idea File

`project-inputs/01_APPLICATION_IDEA.md` should capture at minimum:

- application description
- problem being solved
- expected users
- main workflows
- expected modules or screens
- required outputs
- MVP scope
- out-of-scope items
- user notes and open questions

## 4. Technical Context File

`project-inputs/02_TECHNICAL_CONTEXT.md` should capture at minimum:

- programming language
- framework
- application type
- database
- ORM / data access approach
- package manager / CLI
- UI framework or design system if known
- required external libraries
- forbidden libraries or technologies
- runtime environment
- deployment or hosting notes if known
- technical or security constraints
- technology profile candidate

## 5. Intake Readiness Status

Use these statuses:

- `Missing`
- `Partial`
- `Complete`

## 6. Intake Collection via TeraClientEngagementAgent

For both internal and external projects, if one or both intake files are missing or materially incomplete, the responsible discovery/intake owner is `TeraClientEngagementAgent`.

The discovery/intake flow is:

```text
TeraClientEngagementAgent
-> Client Discovery Mode
-> Smart Interview (if needed)
-> Intake completion
-> TERA_HANDOFF_PACKAGE.md
-> handoff to TeraAgent
```

This is a **two-stage process** defined in `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md` (Section 18):

### Stage 1: Client Discovery (mandatory first step)
1. **Open Listening** — Let the client or Majed explain the idea freely without interruption.
2. **Understanding Summary** — Summarize TCEA's understanding and ask: "هل هذا الفهم صحيح؟"
3. **Confirmation** — Do not proceed until the explanation is confirmed or corrected.
4. **Decision** — If the picture is clear enough, proceed to Intake Gate. If major gaps remain, proceed to Stage 2.

### Stage 2: Smart Interview (if gaps remain)
1. **Opening Round** — Essential questions from Domains 1, 2, 4 (5–7 questions).
2. **Analysis** — Identify gaps and select next domain.
3. **Adaptive Rounds** — Continue in small batches until picture is complete.
4. **Final Understanding Summary** — Confirm with the client or Majed.
5. **Suggestions and Improvements** — Propose improvements, classified separately from scope.

### Rules for both stages

- Do not start formal `project-preparation/` output.
- Do not create `TERA_PROJECT_DECISION.md`.
- Do not choose a final active Technology Profile.
- Do not generate sub-agents for implementation work.
- Do not create implementation `TASK-ID`s.
- Document each answer immediately in `project-inputs/` inside the active application workspace.
- **When the decision owner does not know**: propose a suitable default, document it as an `Assumption` (not as a final decision). See Question Bank for assumption documentation format.

`TeraAgent` does not perform this discovery flow directly. It starts only after approved handoff.

## 7. Minimum Questions During Discovery

When intake is incomplete, `TeraClientEngagementAgent` should ask only the shortest useful questions, such as:

- What is the application idea?
- Who will use it?
- What are the three most important workflows?
- Is the technology already decided?
- If not, should Tera propose it later?
- What database is required, if any?
- Is there any preferred or forbidden UI/design direction?

> **ملاحظة:** أسئلة العميل (الاسم، جهات الاتصال، الشعار، إلخ) يطرحها `TeraClientEngagementAgent`. TeraAgent لا يسأل هذه الأسئلة.

## 8. When Tera Can Proceed

Tera may proceed to formal preparation only when:

- `01_APPLICATION_IDEA.md` exists and is acceptable at minimum level.
- `02_TECHNICAL_CONTEXT.md` exists and is acceptable at minimum level, or clearly documents that the stack is still undecided.
- Tera has determined whether an existing Technology Profile can be used, or whether a new profile draft will be needed later.

> **ملاحظة:** للمشاريع الخارجية، Tera لا يمر بهذه البوابة. Tera يتحقق من اكتمال `TERA_HANDOFF_PACKAGE.md` فقط (من TCEA) ثم يبدأ التحضير. راجع `tera-system/TeraPreExecutionGate.md`.

## 9. Relationship with 00_PROJECT_INPUTS.md

`project-preparation/00_PROJECT_INPUTS.md` is not a replacement for the intake files.

It is Tera's normalized preparation summary derived from:

- `project-inputs/01_APPLICATION_IDEA.md`
- `project-inputs/02_TECHNICAL_CONTEXT.md`

## 10. Final Rule

```text
No Intake = No Project Preparation
No Technical Context = No Active Technology Profile
No Active Technology Profile = No Implementation
No documented client context = No client project preparation
No Client Approval Package = No Implementation
```
