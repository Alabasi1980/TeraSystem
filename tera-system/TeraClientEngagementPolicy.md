# Tera Client Engagement Policy

## 1. Purpose

This policy defines how Tera handles external client projects before formal preparation and implementation.

Tera must treat client work as a documented relationship, not only a technical build request.

## 2. Core Rule

```text
Spoken client input is not final until documented.
```

Client statements, preferences, approvals, and changes must be converted into files before they affect scope, design, implementation, or delivery.

## 3. Client Workspace Structure

Client-facing and client-management files belong under:

```text
clients/
```

Required structure:

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

- Each client must have a dedicated folder.
- Each client application must have a dedicated application folder.
- Do not mix client-facing approval material with internal `project-preparation/` files.
- Do not store secrets, passwords, access tokens, or private credentials in client files.

## 4. Required Client Profile

Every client folder must include:

```text
CLIENT_PROFILE.md
```

Minimum content:

- client name
- client type: individual / company / organization
- business domain
- default client-facing language: Arabic unless the user decides otherwise
- technical familiarity: low / medium / high / unknown
- decision style and communication notes
- project sensitivity: low / medium / high / critical
- preferred approval method
- general relationship notes

## 5. Required Contacts File

Every client folder must include:

```text
CONTACTS.md
```

Minimum content per contact:

- name
- role at client side
- decision authority: decision maker / reviewer / technical contact / finance / other
- phone number if available
- email if available
- preferred communication channel
- approval authority: yes / no / unknown
- communication notes

Tera must not treat approval from a contact as final unless that contact has documented approval authority or the user explicitly confirms it.

## 6. Client Discovery Mode

When a new external client project starts, Tera must enter Client Discovery before implementation.

Client Discovery gathers:

- client identity
- contacts and approval authority
- application idea
- client goals
- client pain points
- preferred style and visual direction
- reference apps, sites, documents, colors, or brand assets
- non-preferred examples
- expected approval process
- budget/time sensitivity if the user chooses to provide it
- client-specific risks

## 7. Majed as Client Intermediary

The normal operating model is:

- Tera asks Majed short, direct client questions.
- Majed asks the client.
- Majed returns the answers to Tera.
- Tera documents material answers in the correct files.

Tera must phrase questions so they can be forwarded to the client with minimal editing.

## 8. Improvement Suggestions

Tera may suggest improvements, additions, or simplifications only if they:

- support the client's original goal
- do not change the core project idea
- are clearly marked as suggestions
- are separated from confirmed scope
- are offered for Majed to review with the client

Suggestions do not become scope until approved and documented.

## 9. Relationship to Internal Tera Files

Client files are official relationship and approval records.

Internal Tera files remain responsible for execution planning:

- `project-inputs/`
- `project-preparation/`
- `project-control/`

If a client-facing file conflicts with an internal Tera file, Tera must stop affected work and ask the user to resolve the contradiction.

## 10. Final Rule

```text
No documented client context = No client project preparation.
No documented approval authority = No final client approval.
```
