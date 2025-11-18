import { useEffect, useRef, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import API_BASE_URL from "../Constant";
import { useI18n } from "../i18n/I18nContext";
import SuccessForm from "./SuccessForm";

export default function PaymentSuccess() {
  const [searchParams] = useSearchParams();
  const sessionId = searchParams.get("session_id");
  const { t, lang } = useI18n();
  const [status, setStatus] = useState(sessionId ? "pending" : "error");
  const [orderId, setOrderId] = useState(null);
  const [discountCode, setDiscountCode] = useState(null);
  const [error, setError] = useState("");
  const [showSuccessForm, setShowSuccessForm] = useState(false);
  const [successMessage, setSuccessMessage] = useState("");
  const rewardRequestedRef = useRef(false);
  const completedRef = useRef(false);

  const fetchRewardCode = async () => {
    if (rewardRequestedRef.current) return;
    rewardRequestedRef.current = true;
    try {
      const response = await fetch(`${API_BASE_URL}ShippingDiscountsCodes/get-random-active-code`);
      if (!response.ok) {
        throw new Error("no_code");
      }
      const payload = await response.json();
      if (payload?.code) {
        setDiscountCode(payload.code);
      }
    } catch (err) {
      console.error("Failed to fetch reward code", err);
    }
  };

  useEffect(() => {
    if (!sessionId) {
      setError(t("payments.invalidSession", "لا يمكن العثور على معرف الدفع."));
      return;
    }

    const token = sessionStorage.getItem("token");
    if (!token) {
      setError(t("productDetails.loginRequired", "يجب تسجيل الدخول لمتابعة عملية الشراء."));
      setStatus("error");
      return;
    }

    const interval = setInterval(async () => {
      try {
        const response = await fetch(`${API_BASE_URL}Payments/CheckoutStatus?sessionId=${sessionId}`, {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        if (!response.ok) {
          throw new Error("failed");
        }
        const data = await response.json();
        if (data.status === "completed") {
          if (completedRef.current) {
            clearInterval(interval);
            return;
          }
          completedRef.current = true;
          setOrderId(data.orderId);
          setStatus("completed");
          const messageText = `${t("payments.completedBody", "تم تسجيل طلبك بنجاح. رقم طلبك")} #${data.orderId ?? ""}`;
          setSuccessMessage(messageText);
          if (data.discountCode) {
            setDiscountCode(data.discountCode);
          } else {
            setDiscountCode(null);
            if (!rewardRequestedRef.current) {
              fetchRewardCode();
            }
          }
          setShowSuccessForm(true);
          clearInterval(interval);
        } else if (data.status === "not_found") {
          if (completedRef.current) {
            clearInterval(interval);
            return;
          }
          setError(t("payments.notFound", "لم يتم العثور على الطلب، يرجى التواصل مع الدعم."));
          setStatus("error");
          clearInterval(interval);
        } else {
          if (!completedRef.current) {
            setStatus("pending");
          }
        }
      } catch (err) {
        if (!completedRef.current) {
          setError(t("payments.checkStatusError", "حدث خطأ أثناء التحقق من حالة الدفع."));
          setStatus("error");
        }
        clearInterval(interval);
      }
    }, 3000);

    return () => clearInterval(interval);
  }, [sessionId, t]);

  return (
    <>
      {showSuccessForm && status === "completed" && (
        <SuccessForm
          message={successMessage || `${t("payments.completedBody", "تم تسجيل طلبك بنجاح. رقم طلبك")} #${orderId ?? ""}`}
          onClose={() => setShowSuccessForm(false)}
          discountCode={discountCode}
          showDiscountCode={!!discountCode}
        />
      )}
      <div className="min-h-screen bg-gradient-to-br from-blue-900 to-blue-800 flex items-center justify-center px-4">
      {!(status === "completed" && showSuccessForm) && (
        <div className="bg-white rounded-3xl shadow-2xl p-8 max-w-lg w-full text-center space-y-6">
        <div className="w-16 h-16 rounded-full mx-auto flex items-center justify-center" style={{ backgroundColor: status === "completed" ? "#16a34a" : status === "error" ? "#dc2626" : "#f97316" }}>
          {status === "completed" ? "✓" : status === "error" ? "!" : "…"}
        </div>
        <h1 className="text-2xl font-bold text-blue-900">
          {status === "completed"
            ? t("payments.successTitle", "تم الدفع بنجاح")
            : status === "error"
            ? t("payments.errorTitle", "حدث خطأ أثناء الدفع")
            : t("payments.pendingTitle", "ننتظر تأكيد الدفع")}
        </h1>
        {status === "pending" && (
          <p className="text-blue-700">
            {t("payments.pendingBody", "جاري تأكيد عملية الدفع من خلال Stripe، سيتم تحديث الصفحة تلقائياً.")}
          </p>
        )}
        {status === "completed" && (
          <>
            {discountCode && (
              <div className="mt-4 p-4 bg-gradient-to-r from-orange-100 to-orange-50 border-2 border-orange-300 rounded-xl shadow-lg">
                <div className="text-center space-y-2">
                  <p className="text-orange-800 font-bold text-lg">
                    {lang === "ar" ? "🎉 مبروك! حصلت على كود خصم الشحن" : "🎉 Congratulations! You got a shipping discount code"}
                  </p>
                  <div className="bg-white p-3 rounded-lg border-2 border-dashed border-orange-400">
                    <p className="text-orange-700 font-semibold mb-1">
                      {lang === "ar" ? "كود الخصم" : "Discount Code"}
                    </p>
                    <p className="text-2xl font-bold text-orange-600 tracking-wider">
                      {discountCode}
                    </p>
                  </div>
                  <p className="text-orange-700 text-sm">
                    {lang === "ar" 
                      ? "💡 استخدم هذا الكود في المرة القادمة للحصول على شحن مجاني!" 
                      : "💡 Use this code in your next purchase to get free shipping!"}
                  </p>
                  <button
                    onClick={() => {
                      navigator.clipboard.writeText(discountCode);
                      alert(lang === "ar" ? "تم نسخ الكود!" : "Code copied!");
                    }}
                    className="mt-2 bg-orange-500 hover:bg-orange-600 text-white font-semibold py-2 px-4 rounded-lg transition"
                  >
                    📋 {lang === "ar" ? "نسخ الكود" : "Copy Code"}
                  </button>
                </div>
              </div>
            )}
            <p className="text-blue-700 mt-4">
              {t("payments.completedBody", "تم تسجيل طلبك بنجاح. رقم طلبك")}{" "}
              <span className="font-bold text-orange-600">#{orderId}</span>
            </p>
          </>
        )}
        {status === "error" && (
          <p className="text-red-600">{error}</p>
        )}

        <div className="space-y-3">
          <Link
            to="/MyPurchases"
            className="block w-full bg-gradient-to-r from-orange-500 to-orange-600 hover:from-orange-600 hover:to-orange-700 text-white font-semibold py-3 rounded-xl transition"
          >
            {t("payments.goToOrders", "الانتقال إلى طلباتي")}
          </Link>
          <Link
            to="/"
            className="block w-full bg-gray-100 hover:bg-gray-200 text-gray-700 font-semibold py-3 rounded-xl transition"
          >
            {t("payments.backHome", "العودة للرئيسية")}
          </Link>
        </div>
        </div>
      )}
    </div>
    </>
  );
}

