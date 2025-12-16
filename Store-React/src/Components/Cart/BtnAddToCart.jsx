import API_BASE_URL from "../Constant";
import { FaShoppingCart, FaCheck } from "react-icons/fa";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useI18n } from "../i18n/I18nContext";
import { getOrCreateSessionId } from "../utils";
import { trackAddToCart } from "../utils/facebookPixel";

export default function AddToCart({ productDetailsId, Quantity, className = "", product = null }) {
  const [isLoading, setIsLoading] = useState(false);
  const [message, setMessage] = useState("");
  const [showSuccess, setShowSuccess] = useState(false);
  const navigate = useNavigate();
  const { t, lang } = useI18n();

  const handleCartClick = async () => {
    const token = sessionStorage.getItem("token");
    setIsLoading(true);
    setMessage("");
    
    try {
      let response;
      
      if (token) {
        // المستخدم مسجل دخول - استخدام السلة العادية
        response = await fetch(`${API_BASE_URL}Carts/PostCartDetails`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          productDetailsId: Number(productDetailsId),
          quantity: Number(Quantity),
        }),
      });
      } else {
        // المستخدم غير مسجل - استخدام السلة المؤقتة
        const sessionId = getOrCreateSessionId();
        response = await fetch(`${API_BASE_URL}Carts/PostGuestCartDetails`, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            sessionId: sessionId,
          },
          body: JSON.stringify({
            productDetailsId: Number(productDetailsId),
            quantity: Number(Quantity),
          }),
        });
      }

      if (!response.ok) {
        throw new Error("Network response was not ok");
      }

      const responseData = await response.json();
      
      // تتبع AddToCart لـ Facebook Pixel
      if (product && product.productId) {
        // استخدام بيانات المنتج الممررة مباشرة
        trackAddToCart(product, Number(Quantity));
      } else if (productDetailsId) {
        // محاولة جلب بيانات المنتج من productDetailsId
        try {
          // جلب productDetails للحصول على productId
          const detailResponse = await fetch(
            `${API_BASE_URL}Product/GetProductDetailsById?Id=${productDetailsId}`
          );
          if (detailResponse.ok) {
            const detailData = await detailResponse.json();
            const productId = detailData.productId;
            
            if (productId) {
              // جلب بيانات المنتج الكاملة باستخدام productId
              const productResponse = await fetch(
                `${API_BASE_URL}Product/GetProductById?ID=${productId}&lang=${lang || "ar"}`
              );
              
              if (productResponse.ok) {
                const productData = await productResponse.json();
                const price = detailData.unitPriceAfterDiscount || detailData.unitPrice || productData.priceAfterDiscount || productData.productPrice || 0;
                
                trackAddToCart({
                  productId: productId,
                  productName: productData.productName || productData.name || "",
                  priceAfterDiscount: price,
                  productPrice: productData.productPrice || 0,
                  productImage: detailData.image || detailData.productImage || productData.productImage || "",
                  categoryName: productData.categoryName || productData.categoryNameAr || productData.categoryNameEn || "",
                }, Number(Quantity));
              } else {
                // إذا فشل جلب المنتج الكامل، استخدم بيانات detail فقط
                console.warn("Could not fetch full product data, using detail data only");
                if (detailData.productId) {
                  trackAddToCart({
                    productId: detailData.productId,
                    productName: detailData.productName || detailData.name || "",
                    priceAfterDiscount: detailData.unitPriceAfterDiscount || detailData.unitPrice || 0,
                    productPrice: detailData.unitPrice || 0,
                    productImage: detailData.image || detailData.productImage || "",
                    categoryName: detailData.categoryName || "",
                  }, Number(Quantity));
                }
              }
            } else {
              console.warn("No productId found in productDetails response");
            }
          } else {
            console.warn("Failed to fetch productDetails for tracking");
          }
        } catch (error) {
          console.error("Error fetching product for tracking:", error);
        }
      }
      
      // عرض رسالة النجاح
      setShowSuccess(true);
      
      setTimeout(() => {
        setShowSuccess(false);
        navigate("/cart");
      }, 1500);
      
    } catch (error) {
      console.error("Error adding product to cart:", error.message);
      setMessage(t("addToCart.error", "حدث خطأ أثناء إضافة المنتج للسلة"));
      
      // إخفاء رسالة الخطأ بعد 3 ثواني
      setTimeout(() => {
        setMessage("");
      }, 3000);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <>
      {/* نافذة التحميل */}
      {isLoading && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-blue-900/30 backdrop-blur-sm">
          <div className="bg-white rounded-2xl p-8 flex flex-col items-center gap-4 shadow-2xl">
            <div className="h-16 w-16 animate-spin rounded-full border-4 border-orange-500 border-t-transparent"></div>
            <p className="text-blue-800 font-medium">{t("addToCart.adding", "جاري إضافة المنتج إلى السلة...")}</p>
          </div>
        </div>
      )}

      {/* رسالة الخطأ */}
      {message && (
        <div className="fixed top-6 left-1/2 z-50 w-80 -translate-x-1/2 transform">
          <div className="rounded-lg bg-red-500 px-6 py-4 text-white shadow-lg flex items-center gap-3 animate-fade-in">
            <div className="h-6 w-6 rounded-full bg-white/20 flex items-center justify-center">
              <span className="text-sm">!</span>
            </div>
            <span>{message}</span>
          </div>
        </div>
      )}

      {/* رسالة النجاح */}
      {showSuccess && (
        <div className="fixed top-6 left-1/2 z-50 w-80 -translate-x-1/2 transform">
          <div className="rounded-lg bg-green-500 px-6 py-4 text-white shadow-lg flex items-center gap-3 animate-fade-in">
            <FaCheck className="text-white" />
            <span>{t("addToCart.success", "تمت الإضافة بنجاح!")}</span>
          </div>
        </div>
      )}

      {/* زر إضافة إلى السلة */}
      <button
        onClick={handleCartClick}
        disabled={isLoading}
        className={`
          inline-flex items-center gap-3 rounded-xl px-6 py-4 font-bold text-lg text-white
          transition-all duration-300 transform hover:scale-105 active:scale-95
          shadow-lg hover:shadow-xl disabled:opacity-50 disabled:cursor-not-allowed
          ${showSuccess 
            ? 'bg-green-600 hover:bg-green-700' 
            : 'bg-gradient-to-l from-[#0a2540] to-[#13345d] hover:from-[#13345d] hover:to-[#0a2540]'
          }
          border-2 border-transparent hover:border-[#0a2540]/30
          min-w-[160px] justify-center
          ${className || 'flex-1'}
        `}
        style={{ color: 'white' }}
      >
        {showSuccess ? (
          <>
            <FaCheck className="animate-bounce" size={20} style={{ color: 'white' }} />
            <span className="font-bold animate-pulse" style={{ color: 'white' }}>{t("addToCart.added", "تمت الإضافة")}</span>
          </>
        ) : (
          <>
            <FaShoppingCart className={isLoading ? 'animate-pulse' : ''} size={20} style={{ color: 'white' }} />
            <span className="font-bold" style={{ color: 'white' }}>{isLoading ? t("addToCart.adding", "جاري الإضافة...") : t("addToCart.button", "أضف إلى السلة")}</span>
          </>
        )}
      </button>
    </>
  );
}