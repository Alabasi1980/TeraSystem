# Tera Application Question Bank

## Purpose

> **ملاحظة نظامية:** بنك الأسئلة هذا يُستخدم أساساً من قبل `TeraClientEngagementAgent` خلال Discovery و Smart Interview قبل إنتاج Handoff Package.
> بعد وجود `TERA_HANDOFF_PACKAGE.md` معتمد من TCEA، يتخطى TeraAgent **Client Discovery Mode و Smart Interview تمامًا** — ويبدأ مباشرة من مرحلة القرار (`Phase 2 — Project Decision`).
> في هذه الحالة، لا يُستخدم هذا البنك من قبل TeraAgent، بل يكون قد استُخدم بالفعل من قبل TeraClientEngagementAgent خلال جلسة الاستكشاف.

This is the reference question bank for the **Client Discovery and Smart Interview** process — a two-stage approach that first explores the client's idea through open conversation, then follows with adaptive structured questioning across all domains.

Tera does not ask all questions. Tera selects the right questions at the right time based on previous answers, and always starts with open listening before any structured questioning.

## How to use this bank

1. **Start with Client Discovery Mode** — open listening, understanding summary, confirmation (see `TERA_RUNTIME_PROTOCOLS.md` Section 18).
2. **Enter Smart Interview only if gaps remain** after the Discovery summary is confirmed.
3. **Select questions based on gaps** — not all domains, not all questions.
4. **When the client doesn't know** — propose a default, document as `Assumption`, do not treat as final decision.
5. **Small projects: 10–15 questions. Medium: 20–35. Complex: deeper per domain.**

## Assumption Handling

When the client cannot answer a question, use this pattern:

### Response format

```
إذا لم تكن متأكدًا، أقترح الافتراض التالي:
- الخيار الموصى به: [الخيار]
- السبب: [شرح مختصر]
- الحالة: Assumption — يحتاج تأكيد لاحق
```

### Documentation format

Record all assumptions in a table (inside `project-inputs/` or in a separate file for medium/large projects):

```text
| المجال | السؤال | افتراض تيرا | الحالة | يحتاج مراجعة قبل |
|--------|--------|-------------|--------|------------------|
| تقني | Web/Mobile | Web App | يحتاج تأكيد | التوقيع على النطاق |
| صلاحيات | عدد الأدوار | Admin + User | يحتاج تأكيد | التنفيذ |
```

### Classification

| الحالة | المعنى |
|--------|--------|
| `مؤكد` | العميل أجاب بوضوح |
| `يحتاج تأكيد` | اقتراح Tera — لم يؤكده العميل بعد |
| `مؤجل` | العميل قال "ربما لاحقاً" |
| `غير محدد حاليًا` | لم يُطرح السؤال بعد أو العميل لم يقرر |

## 🔍 Research Triggers

Throughout the Question Bank, some questions are marked with `🔍 Research recommended`. This means:

When the client's answer to this question is unclear, uncertain, or the client defers to Tera:
1. **Search first** before proposing a default.
2. Present findings with sources.
3. Let the client choose based on research.

### When research is unavailable or inconclusive
- Document the limitation clearly.
- Present as `Unknown — needs client decision` or `Inconclusive research — proceed with caution`.
- Never present an unresearched opinion as a reliable recommendation.

### Common research triggers by domain

| Domain | Topics likely needing research |
|--------|-------------------------------|
| Administrative | Competitor analysis, market standards |
| Functional | Industry-specific workflows, compliance processes |
| Data/Reporting | Standard report formats, data regulations |
| Technical | 🔍 Best libraries, frameworks, APIs, hosting providers |
| Design/UX | 🔍 Design trends, accessibility standards, UI patterns |
| Security | 🔍 Regulatory requirements, security standards |
| Operational | 🔍 Hosting providers, DevOps tools, pricing |

### Quick reference: When to search vs when to use judgment

| Scenario | Action |
|---|---|
| Client: "أي بوابة دفع تنصح؟" | 🔍 **Search** — compare options with sources |
| Client: "كم عدد المستخدمين؟ 5 فقط" | Use judgment — clear answer |
| Client: "ما أفضل قاعدة بيانات؟" | 🔍 **Search** — depends on stack, volume, budget |
| Client: "التطبيق بلاريل" | Use judgment — clear technology |
| Client: "هل أحتاج ERP؟" | 🔍 **Search** — depends on business processes |
| Client: "موظفين 10، بدون صلاحيات" | Use judgment — simple scenario |
| Client: "أحتاج استضافة في السعودية" | 🔍 **Search** — local providers, regulations |
| Client: "ما رأيك في React؟" | Use judgment — well-known technology |

---

## Domain 1: Administrative / Business Context

*Understanding the client, the problem, and the business goal.*

### Essential
- Q1.1: What is the client name? (if external)
- Q1.2: Is the client an individual, company, or organization?
- Q1.3: What is the client's business domain?
- Q1.4: What problem does this application solve?
- Q1.5: Who benefits from this application?
- Q1.6: Is this a new product or replacing an existing system?
- Q1.7: Is this a paid service, internal tool, or public benefit?
- Q1.8: Is there a deadline or time constraint?

### Deep dive (when needed)
- Q1.9: What is the expected number of users at launch? After 6 months?
- Q1.10: Is there a budget range or cost sensitivity?
- Q1.11: Are there competitors or similar apps the client knows?
- Q1.12: Does the client have a business model or monetization plan?
- Q1.13: Who are the decision makers and stakeholders?
- Q1.14: What success metrics matter most? (users, revenue, adoption, etc.)

---

## Domain 2: Functional / Operational

*Understanding users, roles, workflows, and feature expectations.*

### Essential
- Q2.1: Who will use this application? (list user types)
- Q2.2: What are the 3 most important things each user type does?
- Q2.3: Do users have different permission levels or roles?
- Q2.4: Is there a registration / login requirement?
- Q2.5: What is the main workflow from start to finish?
- Q2.6: Does the application need to send notifications? (email, SMS, in-app)
- Q2.7: Does the application need to support multiple languages? Which?

### Deep dive (when needed)
- Q2.8: Describe each workflow step-by-step: trigger, actions, decisions, outcomes.
- Q2.9: Are there approval workflows? (e.g., manager approves expense)
- Q2.10: Does the application need a search or filter feature?
- Q2.11: Is there a dashboard or analytics view?
- Q2.12: Does the application manage inventory, stock, or resources?
- Q2.13: Are there scheduled or automated tasks? (cron, batch jobs)
- Q2.14: Does the application need a mobile-responsive version?
- Q2.15: Are there offline use cases?
- Q2.16: Describe any exceptional or edge-case scenarios.

---

## Domain 3: Data and Reporting

*Understanding data inputs, outputs, storage, and reporting.*

### Essential
- Q3.1: What data does the application collect or manage?
- Q3.2: What are the main data entities? (e.g., users, orders, products)
- Q3.3: Does the application need to import data from external sources?
- Q3.4: Does the application need to export data? In what format? (PDF, Excel, CSV)
- Q3.5: Are there required reports or summaries?

### Deep dive (when needed)
- Q3.6: What is the expected data volume? (records per day/month)
- Q3.7: How long should data be retained?
- Q3.8: Are there data archival or deletion requirements?
- Q3.9: Are there real-time data requirements?
- Q3.10: Does the application need to generate invoices, receipts, or financial documents?
- Q3.11: Are there data validation rules or business calculations?
- Q3.12: Does the client need a data backup or recovery plan?
- Q3.13: Are there complex reporting queries or cross-table aggregations?

---

## Domain 4: Technical

*Understanding technology stack, integrations, and environment.*

### Essential
- Q4.1: Is the technology stack already decided? What is it?
- Q4.2: What type of application? (web, mobile, desktop, hybrid)
- Q4.3: What database is required? (if any)
- Q4.4: 🔍 Are there external integrations or APIs? (payment gateways, third-party services, legacy systems)
- Q4.5: 🔍 What is the target environment? (cloud, on-premise, hybrid)
- Q4.6: Are there any forbidden technologies or libraries?

### Deep dive (when needed)
- Q4.7: Is there an existing codebase, database, or design system?
- Q4.8: 🔍 Does the application need to be containerized? (Docker, Kubernetes)
- Q4.9: 🔍 Are there performance or scalability requirements?
- Q4.10: What is the expected uptime or SLA?
- Q4.11: 🔍 Does the application need CI/CD pipeline?
- Q4.12: 🔍 Are there specific hosting providers preferred or required?
- Q4.13: Does the client prefer specific frameworks or languages?
- Q4.14: Are there API versioning requirements?
- Q4.15: Does the application need webhooks or event-driven architecture?
- Q4.16: Is there a dev/test/prod environment requirement?

---

## Domain 5: Design and UX

*Understanding visual identity, user experience, and design references.*

### Essential
- Q5.1: Does the client have a logo, brand colors, or existing style guide?
- Q5.2: 🔍 Are there reference applications or websites the client likes?
- Q5.3: Are there examples the client explicitly dislikes?
- Q5.4: 🔍 Does the client prefer a specific design style? (modern, minimal, corporate, playful)
- Q5.5: Does the application need to be RTL-friendly? (Arabic, Hebrew)
- Q5.6: Does the client have existing UI components or a design system?

### Deep dive (when needed)
- Q5.7: Are there specific color palettes or hex codes?
- Q5.8: What typography or fonts are preferred?
- Q5.9: Are there icons or illustrations to use?
- Q5.10: 🔍 Does the client want a mobile-first design?
- Q5.11: 🔍 Are there accessibility requirements? (WCAG, screen readers)
- Q5.12: Does the client have wireframes, mockups, or Figma designs?
- Q5.13: Describe the desired layout or navigation style. (sidebar, top nav, tabbed)
- Q5.14: Are there form-heavy screens or content-heavy screens?
- Q5.15: What is the tone of the interface? (professional, casual, educational)
- Q5.16: Should Tera propose 2-3 design directions using internal kits or `getdesign.md` references?
- Q5.17: Does the client approve using an external DESIGN.md as inspiration, with project-specific branding overrides?

---

## Domain 6: Security and Permissions

*Understanding authentication, authorization, data sensitivity, and compliance.*

### Essential
- Q6.1: 🔍 Does the application need user authentication? (email/password, SSO, OAuth, social login)
- Q6.2: Are there different user roles with different permissions?
- Q6.3: 🔍 Does the application handle sensitive personal data?
- Q6.4: 🔍 Is there a compliance requirement? (GDPR, HIPAA, SOC2, local regulations)
- Q6.5: Does the application need audit logs or activity tracking?
- Q6.6: 🔍 Are there file uploads or document storage? What type?

### Deep dive (when needed)
- Q6.7: What is the authentication method? (JWT, session-based, OAuth2, SAML)
- Q6.8: Are there multi-factor authentication requirements?
- Q6.9: Describe the role hierarchy and permission model.
- Q6.10: Are there IP restrictions or geographic restrictions?
- Q6.11: Does the application need encryption at rest and in transit?
- Q6.12: Are there third-party identity providers? (Azure AD, Okta, Google)
- Q6.13: Are there data anonymization or pseudonymization needs?
- Q6.14: What is the data classification level? (public, internal, confidential, restricted)
- Q6.15: Are there security penetration testing requirements?

---

## Domain 7: Operational and Deployment

*Understanding hosting, deployment, maintenance, and delivery.*

### Essential
- Q7.1: 🔍 Where will the application be hosted? (cloud provider, on-premise, hybrid)
- Q7.2: Who will maintain the application after delivery?
- Q7.3: Is there a domain name or SSL certificate already available?
- Q7.4: 🔍 Are there email service requirements? (transactional emails, notifications)
- Q7.5: What is the expected timeline for the first release?

### Deep dive (when needed)
- Q7.6: Is there an IT team that will manage deployment?
- Q7.7: Are there specific DevOps or infrastructure tools in use?
- Q7.8: Does the application need monitoring and alerting? (errors, performance, uptime)
- Q7.9: Are there backup and disaster recovery requirements?
- Q7.10: Is there a maintenance or support period after delivery?
- Q7.11: Does the application need a custom domain or subdomain?
- Q7.12: What is the deployment frequency expectation?
- Q7.13: Are there API rate limiting or throttling requirements?
- Q7.14: Does the client need a staging or UAT environment?
- Q7.15: Are there specific logging or observability requirements?

---

## After Questions: Suggestions and Improvements Phase

Once all relevant domains have been covered and Tera has a complete picture, proceed to:

1. **Summarize understanding** — Confirm with the user/client.
2. **Identify gaps** — Any area that still needs clarification.
3. **Propose improvements** — Tera may suggest:
   - Features that improve the core goal
   - Technologies that fit the requirements
   - Simplifications or scope reductions
   - Deferrals for non-essential items
4. **Classify each suggestion** as:
   - `Recommended — Include now`
   - `Recommended — Consider for later phase`
   - `Informational — For awareness only`
   - `Deferred — Not needed for MVP`
   - `Out of scope — Does not align with goals`
5. **Document all suggestions separately** from confirmed client requirements.
6. **Final confirmation** before moving to formal preparation.

---

## Usage Rules

1. **Tera does not ask all questions.** Select the essential ones first, then deep-dive only in domains that need it.
2. **Start with Domain 1 (Administrative), Domain 2 (Functional), and Domain 4 (Technical) essentials.** These give the core picture.
3. **Analyze after each batch.** Identify gaps, dependencies, and unexpected areas before choosing the next batch.
4. **Group questions in small batches.** 4-7 questions per round is comfortable.
5. **Document answers immediately** in `project-inputs/` files.
6. **After the interview is complete**, transition to the Suggestions and Improvements phase.
7. **For external clients**, Majed acts as intermediary. Tera phrases questions for forwarding.
8. **Suggested improvements must be clearly separated** from confirmed requirements.
