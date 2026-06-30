# Clients Workspace

This folder stores isolated client/application workspaces. Every new application should live under a folder named by client and application so the application can later be exported or removed without polluting the Tera system root.

Default structure:

```text
clients/
  CLIENT-[client-name-or-id]/
    CLIENT_PROFILE.md
    CONTACTS.md
    applications/
      APP-[app-name-or-id]/
        project-inputs/
        project-preparation/
        project-control/
        generated-agents/opencode/
        client-approval/
        client-assets/
        client-communications/
        app-source/
        delivery/
```

Rules:

- Each external client gets one dedicated folder.
- Each application for that client gets one dedicated application folder.
- For internal/Majed-owned projects, use a clear client/owner folder such as `CLIENT-Majed` or the approved internal owner name.
- The application folder is the canonical workspace for that application.
- Application-specific intake, preparation, control records, generated agents, source code, approval material, assets, communications, and delivery records must stay inside the application folder.
- Root-level `project-inputs/`, `project-preparation/`, `project-control/`, and `generated-agents/` are template/bootstrap or Tera-system maintenance areas only after an application workspace is identified.
- Removing or exporting `clients/CLIENT-*/applications/APP-*/` should remove or export that application without modifying the Tera system root.
- Client-facing approval files belong in `client-approval/`.
- Client assets such as logos, images, references, or supplied documents belong in `client-assets/`.
- Communication summaries belong in `client-communications/`.
- Final handover material belongs in `delivery/`.
- For Phase 7 external client closure, create `delivery/CLIENT_HANDOVER_PACKAGE.md` using `clients/CLIENT_HANDOVER_PACKAGE_TEMPLATE.md` as the base.
- Do not store secrets, passwords, API keys, or private credentials here.
- Default client-facing language is Arabic unless Majed decides otherwise.

Implementation must not start for external client projects before the mandatory client approval package is complete and approved according to Tera policies.

Project closure must not be recorded for external client projects before the Phase 7 Client Handover Package and Final Acceptance Checklist are complete or explicitly deferred by an approved decision.
