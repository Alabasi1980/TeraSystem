# رابط المستودع البعيد — Remote URL

> غيّر الرابط أدناه لكل مشروع عميل جديد.
>
> أول شيء تفعله في مشروع جديد: اكتب رابط المستودع هنا.

```
https://github.com/Alabasi1980/TeraSystem
```

---

## كيفية التحديث

### يدويًا (عن طريقك):

افتح هذا الملف واستبدل الرابط بالرابط الجديد.

### عن طريق Tera:

اطلب:

```
غير رابط المستودع إلى https://github.com/account/new-repo.git
```

Tera ستحدّث هذا الملف وتنفذ:
```
git remote set-url origin https://github.com/account/new-repo.git
```

---

## قواعد الاستخدام

- لكل مشروع عميل رابط واحد.
- لا يُستخدم هذا الملف لأكثر من remote في نفس الوقت.
- إذا بدأت مشروع عميل جديد، غيّر الرابط قبل أول commit and push.
- Tera مسؤولة عن قراءة هذا الملف قبل أي push أو release tag push أو GitHub Release.
- المستخدم يوافق أو يرفض الرفع؛ إدارة git والـ tags والتحقق من remote مسؤولية Tera.
- لا يتم إنشاء أو رفع release tag أو GitHub Release بدون موافقة صريحة.
