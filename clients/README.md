# Clients Workspace

This folder stores external client records, application approval packages, client assets, communication summaries, and delivery material.

Default structure:

```text
clients/
  CLIENT-[client-name-or-id]/
    CLIENT_PROFILE.md
    CONTACTS.md
    applications/
      APP-[app-name-or-id]/
        client-approval/
        client-assets/
        client-communications/
        delivery/
```

Rules:

- Each external client gets one dedicated folder.
- Each application for that client gets one dedicated application folder.
- Client-facing approval files belong in `client-approval/`.
- Client assets such as logos, images, references, or supplied documents belong in `client-assets/`.
- Communication summaries belong in `client-communications/`.
- Final handover material belongs in `delivery/`.
- For Phase 7 external client closure, create `delivery/CLIENT_HANDOVER_PACKAGE.md` using `clients/CLIENT_HANDOVER_PACKAGE_TEMPLATE.md` as the base.
- Do not store secrets, passwords, API keys, or private credentials here.
- Default client-facing language is Arabic unless Majed decides otherwise.

Implementation must not start for external client projects before the mandatory client approval package is complete and approved according to Tera policies.

Project closure must not be recorded for external client projects before the Phase 7 Client Handover Package and Final Acceptance Checklist are complete or explicitly deferred by an approved decision.
