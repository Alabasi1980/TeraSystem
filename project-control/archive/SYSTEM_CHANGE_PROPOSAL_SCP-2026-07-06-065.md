## تحويل قواعد المنع والتوقف المتكررة إلى Master Rules + إضافة مراجع

Title:
Introduce Master Rules (MR1–MR4) to centralize repeated stop/block rules across TCEA

Request Type:
Owner improvement request based on external audit finding

Problem:
Rules about "Confirmed by Majed only", "High-risk blocks progression", "Pending Approval blocks handoff", and "Uncertainty must stop" are repeated in 10+ locations across the file (A.6.1, A.6.2, A.6.5, B.1, B.3, B.4, B.5, B.6, B.7, A.8.3). This repetition creates:
1. Cognitive load on the model — it sees the same rule differently phrased
2. Maintenance burden — updating one requires finding all copies
3. Interpretation risk — weaker models may treat similar rules as different

Evidence:
- A.6.1: High-risk + Assumption/Unresolved → لا يجوز وضع Complete
- A.6.2: 3 حالات توقف إجباري (منها High-risk)
- A.6.5: Traps #1, #4, #5, #6 (4 different phrasings of "need Majed confirmation")
- B.1: Domain بخطورة High بدون تأكيد ← توقف
- B.3: ميزة In Scope موسومة بـ Research/Assumption ← توقف
- B.4: عناصر النطاق والتسعير يجب أن تكون Confirmed by Majed
- B.7: جميع العناصر في حزمة الهاندوف Confirmed by Majed + صفر Pending Approval
- A.8.3: Discovery Gate, Budget-to-Scope, missing items all repeat similar stop conditions

Affected Files:
- `.opencode/agents/tera-client-engagement.md`
- `project-control/SYSTEM_EVOLUTION_LOG.md`

Proposed Change:

1. **إضافة A.6.0 Master Rules — 4 قواعد رئيسية**:
   - MR1: Source Authority Rule — "كل عنصر يدخل النطاق أو التسعير أو الهاندوف يجب أن يكون معتمداً من Majed صراحة ([Confirmed by Majed])."
   - MR2: High-Risk Resolution Rule — "أي عنصر غير مؤكد (وسم Assumption أو Unresolved) مع خطورة High يمنع التقدم."
   - MR3: Pending Approval Block Rule — "أي ملف أو قرار بحالة Pending Approval يمنع إكمال Handoff."
   - MR4: Uncertainty Stop Rule — "عند أي شك مؤثر على النطاق أو السعر أو الهاندوف، توقف وأخرج قالب STOP — UNCERTAINTY BLOCK."

2. **تحديث المقاطع المكررة لإضافة "ينطبق Master Rule X"** بدلاً من إزالة النص الأصلي — الإشارة تضاف كسطر إضافي دون حذف النص المكرر، لأن الحذف الكامل قد يربك النماذج. هذا أسلوب "التدريج" بدلاً من "الاستبدال".

3. **تحديث قاعدة A.6.5 الذهبية** لترتبط بـ Master Rules.

Why This Is Necessary:
- يقلل الحمل المعرفي على النموذج بتوفير 4 قواعد مركزية بدلاً من 10+ تكرارات متفرقة
- يحسّن قابلية الصيانة — تحديث قاعدة واحدة يحدثها في كل مكان
- يزوّد النماذج الضعيفة بـ "مرجع ذهني" سريع قبل التشتت في التفاصيل

Rejected Alternatives:
- حذف كل التكرارات واستبدالها فقط بـ MR references: خطر على النماذج الضعيفة التي تحتاج التكرار كتأكيد
- إنشاء ملف سياسة منفصل MasterRules.md: Anti-Bloat — لا مبرر لملف جديد
- إضافة قواعد أكثر من 4: تكرار المشكلة الأصلية — الاكتناز مقصود

Anti-Bloat Check:
- المشكلة: تكرار 10+ مرة لنفس 4 قواعد أساسية
- لماذا لا يكفي تعديل ملف موجود؟ هذا ما سنفعله
- لماذا لا يكفي عميل موجود؟ لا حاجة لعميل جديد
- هل الإضافة تقلل التعقيد؟ نعم — 4 Master Rules مركزية تختصر 10+ تكرارات
- أثر التوكنز: إضافة ~15 سطراً + إشارات قصيرة، وإزالة لا شيء (نضيف إشارات لا نحذف)
- هل توجد طريقة أصغر؟ نعم — يمكن إضافة فقط Master Rules دون إشارات في المقاطع المكررة، لكن الإشارات ضرورية للربط

Risk:
Low-Medium — إضافة محتوى جديد مع بقاء النص الأصلي. لا إزالة أو تغيير في أي قاعدة أو صلاحية. الخطر الوحيد هو طول الملف (يزيد بـ ~20 سطراً فقط).

Rollback Plan:
1. Revert `.opencode/agents/tera-client-engagement.md`
2. Revert `project-control/SYSTEM_EVOLUTION_LOG.md`
3. Delete this proposal file

Approval Required:
Approved by Majed in-session on 2026-07-06
