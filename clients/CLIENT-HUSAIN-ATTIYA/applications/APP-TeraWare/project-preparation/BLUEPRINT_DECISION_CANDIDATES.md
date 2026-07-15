# BLUEPRINT_DECISION_CANDIDATES.md — TeraWare

> This file captures blueprint-level candidates only. Nothing here is a final technical decision.

## 1. Solution Shape Candidate

- **Candidate:** two-surface helper solution
  - Surface A: Desktop operations hub
  - Surface B: Web/mobile request portal
- **Reason:** distinct operational contexts and input patterns
- **Downstream validation needed:** shared auth model, deployment separation, support burden

## 2. Desktop Interaction Candidate

- **Candidate:** WPF desktop shell with embedded WebView2
- **Why it currently leads:** Oracle APEX enhancement is a first-class requirement, not a side utility
- **Tradeoff:** Windows-only operational dependence vs fast, rich internal integration

## 3. Web Portal Candidate

- **Candidate:** ASP.NET Core Razor Pages + Bootstrap RTL
- **Why it currently leads:** request flow is form-heavy and mobile-friendly rather than SPA-heavy
- **Tradeoff:** less front-end dynamism than a full SPA, but simpler operational delivery

## 4. Oracle Access Boundary Candidate

- **Candidate:** direct Oracle access from helper applications using Oracle.ManagedDataAccess
- **Why it currently leads:** upstream explicitly rejected replication/API middleware for this stage
- **Tradeoff:** stronger coupling to Oracle schema and database-side behavior

## 5. SQL Server Ownership Candidate

- **Candidate:** SQL Server stores helper-owned data only
  - request headers/items
  - browser settings
  - selected captured operational data
- **Why it currently leads:** preserves ERP authority while enabling helper workflows
- **Tradeoff:** requires clear link ownership rules between SQL Server and Oracle records

## 6. APEX Enhancement Candidate

- **Candidate:** global JavaScript injector with page-aware logic
- **Why it currently leads:** many pages need augmentation and URL/page ID cues are available
- **Tradeoff:** sensitive to APEX DOM changes, version changes, and PPR behavior

## 7. Material Request Governance Candidate

- **Candidate:** helper request first, manual Oracle reference later
- **Why it currently leads:** reduces immediate ERP write risk and keeps the Oracle workflow authoritative
- **Tradeoff:** introduces an operational linking step that must be governed and auditable

## 8. Security / Identity Candidate

- **Candidate:** consume Oracle-origin users/roles as baseline identity source
- **Why it currently leads:** aligned with current enterprise reality and avoids parallel user silos early
- **Tradeoff:** helper-specific permissions may later require an additional mapping layer

## 9. Formal Preparation Focus Recommendation

Most valuable early downstream validations:
1. Oracle schema and update safety review
2. APEX target-page technical spike
3. request state/data model definition
4. role matrix and permission coverage
