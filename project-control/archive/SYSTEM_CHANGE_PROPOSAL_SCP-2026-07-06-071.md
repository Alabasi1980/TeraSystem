## SCP-071 — إضافة A.6.8 Failsafe Recovery Protocol (مخرج الطوارئ)

Title:
Add Failsafe Recovery Protocol — emergency exit when process compliance fails

Request Type:
Owner improvement request based on external audit finding #10

Problem:
The TCEA file has extensive preventive controls (Self-Check, Uncertainty Protocol, Confidence Threshold, Hard/Soft Block, Master Rules, Gates) but ZERO recovery controls. If the model:
- Realizes mid-task it skipped a required step
- Discovers it produced output with low-confidence information
- Gets stuck in a loop or contradiction
- Violates a process rule inadvertently

...there is no documented "what now?" path. The model either freezes, guesses recovery steps, or continues in a broken state.

Evidence:
External audit finding #10: "ماذا يفعل النموذج عندما يخطئ ويعرف أنه أخطأ؟ أضف خطة استرداد واضحة — ليس مجرد 'توقف' بل 'اعترف، تراجع، استعد'."

Affected Files:
- `.opencode/agents/tera-client-engagement.md`
- `project-control/SYSTEM_EVOLUTION_LOG.md`

Proposed Change:
**Add A.6.8 Failsafe Recovery — خطة استرداد الطوارئ** (after A.6.7 Confidence Threshold)

Content:

1. **متى تفعّل Failsafe:**
   - تكتشف أنك تخطيت خطوة إلزامية في A.4 (التدفق)
   - تكتشف أن مخرجاتك تعتمد على معلومات Low-Confidence دون UNCERTAINTY_NOTICE
   - تكتشف أنك أنتجت مستنداً (مثل DRAFT_QUOTATION) دون تجاوز البوابة المطلوبة
   - تكتشف أنك تواصلت مع العميل مباشرة أو مع TeraAgent — خطأ لا يمكن التراجع عنه
   - تصل إلى حالة contradiction: قاعدتان في هذا الملف تتعارضان في موقفك الحالي

2. **خطوات الاسترداد الثلاث:**
   ```
   خطوة 1 — اعترف (Acknowledge)
   - أوقف كل الإجراءات الحالية فوراً
   - اعترف صراحةً: "لقد انحرفت عن المنهج — أحتاج لتصحيح المسار"
   
   خطوة 2 — شخّص (Diagnose)
   - حدد بالضبط: ما الخطوة التي تخطيتها؟ أي قاعدة انتهكت؟ أي معلومة كانت منخفضة الثقة؟
   - سجّل التشخيص في CLIENT_DECISION_LOG.md بحالة Deviation
   
   خطوة 3 — استعد (Recover)
   - ارجع إلى آخر Checkpoint معروف (آخر نقطة PASS في أي Gate)
   - إذا لم يوجد Checkpoint → ارجع إلى بداية Mode الحالي (A.3)
   - إذا كان الخطأ لا يمكن التراجع عنه (مثلاً: أرسلت معلومات للعميل) → اعترف لـ Majed فوراً
   ```

3. **متى ترفع لـ Majed مباشرة (بدون محاولة استرداد):**
   ```text
   - تواصل مباشر مع العميل أو TeraAgent → إبلاغ فوري + Hard Stop
   - إصدار سعر أو عقد بدون اعتماد → إبلاغ فوري + Hard Stop
   - إنتاج TERA_HANDOFF_PACKAGE.md بدون Approval → إبلاغ فوري + Hard Stop
   - تدمير أو حذف بيانات عميل → إبلاغ فوري (هذا يتطلب تدخل Majed)
   ```

4. **Checkpoint Logging:**
   ```text
   سجّل Checkpoints تلقائياً بعد:
   - PASS في أي Gate (B.1, B.2, B.3, B.4, B.6, B.7)
   - اعتماد Majed على CLIENT_INTAKE.md
   - اعتماد Majed على DISCOVERY_COVERAGE_SUMMARY.md
   - اعتماد Majed على DRAFT_QUOTATION.md
   
   التنسيق:
   CHECKPOINT [التاريخ والوقت]: B.1 Discovery Coverage Gate = PASS
   ```

Why This Is Necessary:
Prevention alone is insufficient. The model needs a documented recovery path when prevention fails. Without it, the model either freezes (wasting tokens and Majed's time) or continues broken (producing unreliable output).

Rejected Alternatives:
- Add recovery logic to each Gate: Would bloat B.1–B.7 with recovery instructions; recovery is a cross-cutting concern
- Create standalone C.11 section: Recovery is a runtime protocol, not an operations appendix — belongs in A.6
- Add to A.6.2 Uncertainty Protocol: Uncertainty Protocol is about proactive stopping; Failsafe is about reactive recovery

Anti-Bloat Check:
- Problem: Zero recovery controls — if prevention fails, model has no documented path
- Why not edit existing file? We are editing existing file — adding A.6.8
- Why not use existing agent? No new agent needed
- Does this reduce complexity? Yes — replaces model guessing with 3 clear steps
- Token impact: ~55 lines added
- Smaller way: This IS minimal — acknowledge + diagnose + recover + escalation + checkpoint

Risk:
Low — additive only. References existing gates and modes.

Rollback Plan:
1. git checkout -- .opencode/agents/tera-client-engagement.md
2. git checkout -- project-control/SYSTEM_EVOLUTION_LOG.md
3. Remove-Item -LiteralPath "project-control/SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-06-071.md"

Approval Required:
Yes — Majed approval before implementation
