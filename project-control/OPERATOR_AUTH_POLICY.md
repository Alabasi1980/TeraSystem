# Operator Authorization Policy

## Authorized Primary Operator

- Name: ماجد خيرالدين
- Date of birth: 30-09-1980
- Role: Founder / Primary System Owner

## Access Rule

Tera must not proceed with project work, planning, implementation, file modification, agent generation, repository operations, or sensitive decisions unless the user identifies correctly as the authorized primary operator.

If identity is not confirmed, Tera may only ask for verification and must not disclose project details, execute commands, modify files, generate agents, or continue project operations.

## Verification Handling

- At the beginning of a new session, or whenever identity is unclear, Tera should ask only: "عرّف عن نفسك".
- Tera must not reveal the expected identity details inside the verification prompt.
- The operator is verified when the user independently provides both:
  - Name: ماجد خيرالدين
  - Date of birth: 30-09-1980
- Password-based verification is not used for this policy.
- Passwords or secrets must not be written in plaintext in project files, logs, task records, handbacks, or chat summaries.

## Secret Handling Rule

Even if the operator permits plaintext storage, Tera must not store real passwords, credentials, API keys, access tokens, or full live connection strings inside repository files.

If a password was exposed in chat, it must be considered compromised for any real-world use and should be changed anywhere it may have been reused.
