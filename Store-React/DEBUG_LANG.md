# دليل التحقق من مشكلة اللغة - Language Debugging Guide

## المشكلة
المنتجات لا تزال تُعرض بالعربية دائماً حتى عند تغيير اللغة.

## خطوات التحقق

### 1. افتح Developer Console في المتصفح (F12)
### 2. تحقق من Console Logs:
عند تحميل الصفحة، يجب أن ترى:
```
Fetching products with lang: en URL: http://localhost:5042/api/Product/GetAllProductsWithLimit?page=1&limit=10&lang=en
Products received: 10 First product: [اسم المنتج بالإنجليزية]
```

### 3. تحقق من Network Tab:
- افتح Network tab في Developer Tools
- ابحث عن طلبات `GetAllProductsWithLimit` أو `GetDiscountProducts`
- انقر على الطلب وتحقق من:
  - **Request URL**: يجب أن يحتوي على `&lang=en` أو `&lang=ar`
  - **Response**: تحقق من أن `productName` في الـ response باللغة الصحيحة

### 4. تحقق من قيمة `lang` في React:
افتح Console وأكتب:
```javascript
// في أي component
const { lang } = useI18n();
console.log('Current lang:', lang);
```

### 5. تحقق من API Response:
في Network tab، انقر على Response وتحقق من:
```json
[
  {
    "productId": 1,
    "productName": "اسم المنتج باللغة المختارة",
    "moreDetails": "...",
    ...
  }
]
```

## الحلول المحتملة

### إذا كان `lang` دائماً "ar":
1. تحقق من `I18nContext.jsx` - قد تكون القيمة الافتراضية "ar"
2. تحقق من أن تغيير اللغة يعمل بشكل صحيح
3. أضف console.log في `I18nContext` للتحقق من تغيير اللغة

### إذا كان `lang` صحيح لكن API يُرجع العربية:
1. تحقق من API logs في Visual Studio Output
2. تحقق من أن `lang` parameter يصل للـ API بشكل صحيح
3. تحقق من أن منطق اختيار اللغة في `ProductController` يعمل

### إذا كان API يُرجع اللغة الصحيحة لكن React لا يعرضها:
1. تحقق من أن `ProductItem.jsx` يستخدم `productName` مباشرة
2. تحقق من أن البيانات لا يتم cache
3. أعد تحميل الصفحة بعد تغيير اللغة

## إصلاح سريع

إذا كانت المشكلة مستمرة، جرب:

1. **مسح Cache**:
   - اضغط Ctrl+Shift+R (أو Cmd+Shift+R على Mac)
   - أو افتح DevTools > Application > Clear Storage > Clear site data

2. **إعادة تشغيل التطبيق**:
   - أوقف Backend API
   - أوقف Frontend React
   - أعد تشغيلهما

3. **التحقق من SQL Script**:
   - تأكد من تنفيذ `QUICK_FIX.sql` أو `CREATE_AnnouncementBars_TABLE.sql`

