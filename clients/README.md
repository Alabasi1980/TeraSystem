# clients/

## Role
Client records, approval packages, assets, communications, and delivery material.

Each client has a dedicated folder following this structure:

```
clients/
  CLIENT-XXXXX/                  ← Client code (e.g., CLIENT-MAWTHOOQ)
    applications/
      APP-XXXXX/                 ← Application code
        client-approval/         ← Signed approvals, acceptance records
        client-documents/        ← Client-facing output (proposals, quotes, letters)
        client-engagement/       ← Discovery, intake, handoff packages
        delivery/                ← Release notes, deployment guides, handover reports
        generated-agents/        ← Generated sub-agents for this project
        project-control/         ← Tasks, decisions, activity logs
        project-inputs/          ← Raw intake inputs
        project-preparation/     ← Internal preparation files
```

## Rules
- **Internal policies remain in `tera-system/`** — do not duplicate policy files inside `clients/`.
- **Each application folder is self-contained** — it should not reference files from other applications.
- **Do not commit client secrets** — configuration files, API keys, or passwords to the repository.

## Source of Truth
`tera-system/TeraPolicyMap.md` governs the official file mapping.  
`tera-system/TeraClientPolicy.md` governs client engagement policies.
