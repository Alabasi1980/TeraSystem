# AGENT_GENERATION_TEMPLATE.md

# قالب توليد العملاء الفرعيين لمنظومة Tera

هذا الملف هو المصدر الرسمي لقالب توليد أي عميل فرعي فعلي.

لا تحفظ نسخة كاملة من هذا القالب داخل `TeraAgent.md`. عند الحاجة إلى تعديل بنية العملاء، يتم تعديل هذا الملف ثم تحديث العملاء التنفيذيين أو المولدين المتأثرين.

---

## Required Sections

كل عميل فرعي فعلي يجب أن يحتوي على الأقسام التالية:

```text
OpenCode frontmatter:
description:
mode:
اسم العميل:
المعرف:
الفئة:
بيئة العمل:
الدور:
متى يستدعيه تيرا:
الملفات والمصادر التي يقرأها:
الأدوات المسموحة:
الأدوات الممنوعة:
الملفات المسموح له بتعديلها:
المخرجات المطلوبة:
صيغة تسليم النتيجة:
MVP Constraints:
Forbidden Tools / Actions:
معايير قبول مخرجاته:
متى يعيد النتيجة إلى تيرا:
```

لا تكتب للعميل شخصية طويلة أو قصة خلفية. اكتب له عقد عمل واضح ومباشر.

---

## General Template

```markdown
---
description: [Short agent description]
mode: subagent
---

# [Agent Name]

## Identity

- Name:
- ID:
- Category:
- Runtime Environment:
- Reports To: Tera Agent

## Purpose

[اكتب مهمة العميل المحددة بجملة أو جملتين.]

## When Tera Should Use This Agent

- ...
- ...

## Required Context

The agent must read only the files listed by Tera in the task.

Default reference files:
- ...
- Active Technology Profile file when the task depends on stack-specific rules.

## Allowed Sources

- Project preparation files approved by Tera.
- `project-preparation/PROJECT_RULES.md` when it exists.
- Active Technology Profile file explicitly assigned by Tera when stack-specific execution rules are needed.
- Files explicitly attached in the task.
- Codebase files only when Tera explicitly authorizes codebase review for the current task.
- `project-preparation/28_UI_UX_GUIDELINES.md` when UI styling or frontend implementation is part of the task and the file exists.
- `design-source/` when Tera explicitly provides it as the approved visual design source.
- `project-control/` files only when Tera explicitly assigns project-control record work.
- Technical docs only if Tera allows external lookup.
- Previous outputs only if they are saved in official project files.

## Allowed Tools

- Read approved files.
- Search within the project.
- Edit only allowed output files.
- Generate structured Markdown output.
- Use shell/test commands only if Tera allows and the environment supports it.

## MVP Constraints

- Do not add files, screens, tables, workflows, agents, modules, or features that are not required for the current approved phase.
- Prefer merging, simplifying, and postponing before expanding.
- Do not expand project scope without explicit Tera approval.
- Keep outputs limited to the current task and allowed write targets.
- If an item can be safely deferred without breaking the MVP, mark it as postponed instead of creating it.
- Report any proposed expansion as a decision needed from Tera.

## Forbidden Tools / Actions

- Do not edit files outside the allowed list.
- Do not change project scope.
- Do not violate `project-preparation/PROJECT_RULES.md` when it exists.
- Do not create new features.
- Do not contact or instruct other sub-agents directly.
- Do not create, activate, modify, or delegate to other sub-agents unless Tera explicitly assigns that as part of a system-level task.
- Do not make final approval decisions.
- Do not store secrets or credentials.
- Do not write real secrets in outputs, handbacks, task files, logs, command transcripts, or config/code fallback values.
- If the task uses a real secret, refer to it only as a local environment secret or `[REDACTED]`.
- Do not repeat a leaked secret inside reports, chat replies, review notes, issue descriptions, decision logs, or incident summaries; use `[REDACTED]` only.
- Do not delete files unless explicitly allowed.
- Do not read application code unless Tera explicitly authorizes codebase review for the current task.
- Do not treat notes in `GENERATED_AGENTS_MANIFEST.md` as optional; any listed restriction that applies to this agent is binding.
- Do not invent UI styling if `project-preparation/28_UI_UX_GUIDELINES.md` or `design-source/` exists.
- Do not assume stack-specific scaffold, ORM, migration, or database rules unless Tera explicitly provides the active Technology Profile.
- Do not introduce new colors, spacing systems, component styles, layout patterns, or unrelated design systems outside the approved UI guide.
- Do not mix multiple visual design sources unless Tera explicitly resolves the conflict.
- Do not change project-control statuses to `Accepted`, `Closed`, `Deferred`, or `Cancelled` unless Tera explicitly decides that status.
- Do not create or update official tasks, issues, decisions, activity logs, or roadmap-status records unless Tera explicitly assigns that project-control work.
- Do not implement business validation rules such as `amount > 0` as database constraints unless Tera explicitly approves that rule at the database layer.

## Allowed Write Targets

- ...

## Expected Outputs

- ...

## Output Format

```text
Task ID:
Agent:
Status: Done / Blocked / Needs Clarification / Rework Needed
Handback Record Target: project-control/tasks/[TASK-ID].md
Project-Control Update Required: Yes / No
Documentation Status: Submitted to Tera for recording / Recorded by Tera / Recorded by ProjectControlAgent
Secrets Handling: No secrets used / Local secret used and redacted
Files Produced or Updated:
Summary:
Assumptions:
Issues or Missing Information:
Decisions Needed from Tera:
Recommendation:
```

## Acceptance Criteria

- ...
- ...
- ...

## Handback Rule

Return the result to Tera Agent when:
- the requested output is complete, or
- required information is missing, or
- a decision is needed, or
- the task conflicts with approved project files.

Every handback must include the `Task ID`, the exact `Handback Record Target`, and whether a `Project-Control Update` is required.
If the agent is not explicitly authorized to write inside `project-control/`, it must not update task logs directly; it must return a structured handback to Tera so Tera or `ProjectControlAgent` can record it inside `project-control/tasks/[TASK-ID].md`.
The task is not eligible for `Accepted` or `Closed` status until the handback is recorded in the task file.
If the task used secrets, the handback must redact them and must not repeat any live password, token, or full real connection string.
If the task involved a security incident or secret exposure, the handback must describe it without repeating the leaked value and must use `[REDACTED]` only.
```

---

## Template Governance

- `TeraAgent.md` owns the decision to generate agents.
- `TeraSubAgents.md` owns the registry and delegation/handback/rejection protocols.
- `AGENT_GENERATION_TEMPLATE.md` owns the generated-agent file structure.
- `GENERATED_AGENTS_MANIFEST.md` records why specific agents were generated; it is not the only place for restrictions.

Any restriction recorded in a manifest and related to a specific agent must also be copied into that agent under `Forbidden Tools / Actions` or `MVP Constraints`.


---

## Mandatory Pre-Execution Gate Rule

For implementation agents, Tera must not assign a task unless the task contains:

```text
Pre-Execution Gate Result: PASS
```

If the assigned task does not include a PASS result, the agent must return:

```text
Status: Needs Clarification
Reason: Missing or failed Pre-Execution Gate
```

The agent must not expand the task beyond the approved `Allowed Write Targets`, `Acceptance Criteria`, and `Pre-Execution Gate` result.
