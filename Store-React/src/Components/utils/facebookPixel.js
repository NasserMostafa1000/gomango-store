import ReactPixel from "react-facebook-pixel";

/**
 * تتبع حدث ViewContent - عند فتح صفحة منتج
 * @param {Object} product - بيانات المنتج
 * @param {string} product.productId - معرف المنتج
 * @param {string} product.productName - اسم المنتج
 * @param {number} product.priceAfterDiscount - السعر بعد الخصم
 * @param {number} product.productPrice - السعر الأصلي
 * @param {string} product.productImage - صورة المنتج
 * @param {string} product.categoryName - اسم القسم (اختياري)
 */
export const trackViewContent = (product) => {
  if (!product || !product.productId) return;

  try {
    const contentData = {
      content_name: product.productName || "",
      content_ids: [String(product.productId)],
      content_type: "product",
      value: product.priceAfterDiscount || product.productPrice || 0,
      currency: "AED",
    };

    if (product.productImage) {
      contentData.contents = [
        {
          id: String(product.productId),
          quantity: 1,
          item_price: product.priceAfterDiscount || product.productPrice || 0,
        },
      ];
    }

    if (product.categoryName) {
      contentData.content_category = product.categoryName;
    }

    ReactPixel.track("ViewContent", contentData);
    console.log("Facebook Pixel: ViewContent tracked", contentData);
  } catch (error) {
    console.error("Error tracking ViewContent:", error);
  }
};

/**
 * تتبع حدث AddToCart - عند إضافة منتج للسلة
 * @param {Object} product - بيانات المنتج
 * @param {string} product.productId - معرف المنتج
 * @param {string} product.productName - اسم المنتج
 * @param {number} product.priceAfterDiscount - السعر بعد الخصم
 * @param {number} product.productPrice - السعر الأصلي
 * @param {number} quantity - الكمية المضافة
 * @param {string} product.productImage - صورة المنتج (اختياري)
 * @param {string} product.categoryName - اسم القسم (اختياري)
 */
export const trackAddToCart = (product, quantity = 1) => {
  if (!product || !product.productId) {
    console.warn("Facebook Pixel: AddToCart - Missing product or productId", product);
    return;
  }

  try {
    const price = product.priceAfterDiscount || product.productPrice || 0;
    const contentData = {
      content_name: product.productName || "",
      content_ids: [String(product.productId)],
      content_type: "product",
      value: price * quantity,
      currency: "AED",
      contents: [
        {
          id: String(product.productId),
          quantity: quantity,
          item_price: price,
        },
      ],
    };

    if (product.categoryName) {
      contentData.content_category = product.categoryName;
    }

    ReactPixel.track("AddToCart", contentData);
    console.log("✅ Facebook Pixel: AddToCart tracked successfully", contentData);
  } catch (error) {
    console.error("❌ Error tracking AddToCart:", error);
  }
};

/**
 * تتبع حدث ViewCategory - عند تصفح قسم
 * @param {string} categoryName - اسم القسم
 * @param {Array} products - قائمة المنتجات في القسم (اختياري)
 */
export const trackViewCategory = (categoryName, products = []) => {
  if (!categoryName) return;

  try {
    const contentData = {
      content_name: categoryName,
      content_type: "product_group",
      content_category: categoryName,
    };

    if (products.length > 0) {
      contentData.content_ids = products
        .slice(0, 10)
        .map((p) => String(p.productId || p.id));
    }

    ReactPixel.track("ViewCategory", contentData);
    console.log("Facebook Pixel: ViewCategory tracked", contentData);
  } catch (error) {
    console.error("Error tracking ViewCategory:", error);
  }
};

/**
 * تتبع حدث Search - عند البحث عن منتجات
 * @param {string} searchString - نص البحث
 * @param {number} resultsCount - عدد النتائج (اختياري)
 */
export const trackSearch = (searchString, resultsCount = 0) => {
  if (!searchString) return;

  try {
    const contentData = {
      search_string: searchString,
      content_type: "product",
    };

    if (resultsCount > 0) {
      contentData.num_items = resultsCount;
    }

    ReactPixel.track("Search", contentData);
    console.log("Facebook Pixel: Search tracked", contentData);
  } catch (error) {
    console.error("Error tracking Search:", error);
  }
};

/**
 * تتبع حدث InitiateCheckout - عند الذهاب لصفحة الشراء
 * @param {Array} products - قائمة المنتجات في السلة
 * @param {number} totalValue - إجمالي قيمة الطلب
 */
export const trackInitiateCheckout = (products = [], totalValue = 0) => {
  try {
    const contentData = {
      content_type: "product",
      value: totalValue,
      currency: "AED",
      num_items: products.length,
    };

    if (products.length > 0) {
      contentData.content_ids = products.map((p) =>
        String(p.productId || p.productDetailsId || p.id)
      );
      contentData.contents = products.map((p) => ({
        id: String(p.productId || p.productDetailsId || p.id),
        quantity: p.quantity || 1,
        item_price: p.unitPrice || p.unitPriceAfterDiscount || p.price || 0,
      }));
    }

    ReactPixel.track("InitiateCheckout", contentData);
    console.log("Facebook Pixel: InitiateCheckout tracked", contentData);
  } catch (error) {
    console.error("Error tracking InitiateCheckout:", error);
  }
};

/**
 * تتبع حدث Purchase - عند إتمام الشراء
 * @param {string} orderId - رقم الطلب
 * @param {Array} products - قائمة المنتجات المشتراة
 * @param {number} totalValue - إجمالي قيمة الطلب
 * @param {string} currency - العملة (افتراضي: AED)
 */
export const trackPurchase = (orderId, products = [], totalValue = 0, currency = "AED") => {
  if (!orderId) return;

  try {
    const contentData = {
      content_type: "product",
      value: totalValue,
      currency: currency,
      num_items: products.length,
    };

    if (products.length > 0) {
      contentData.content_ids = products.map((p) =>
        String(p.productId || p.productDetailsId || p.id)
      );
      contentData.contents = products.map((p) => ({
        id: String(p.productId || p.productDetailsId || p.id),
        quantity: p.quantity || 1,
        item_price: p.unitPrice || p.unitPriceAfterDiscount || p.price || 0,
      }));
    }

    ReactPixel.track("Purchase", {
      ...contentData,
      order_id: String(orderId),
    });
    console.log("Facebook Pixel: Purchase tracked", {
      ...contentData,
      order_id: String(orderId),
    });
  } catch (error) {
    console.error("Error tracking Purchase:", error);
  }
};

/**
 * تتبع حدث PageView - عند تغيير الصفحة
 */
export const trackPageView = () => {
  try {
    ReactPixel.pageView();
    console.log("Facebook Pixel: PageView tracked");
  } catch (error) {
    console.error("Error tracking PageView:", error);
  }
};

