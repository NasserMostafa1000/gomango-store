import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import API_BASE_URL from "../Constant";
import { Helmet } from "react-helmet";
import NavBar from "../Home/Nav";
import { useI18n } from "../i18n/I18nContext";

export default function ForgotPassword() {
  const [email, setEmail] = useState("");
  const [message, setMessage] = useState("");
  const [messageType, setMessageType] = useState("");
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const { t } = useI18n();

  useEffect(() => {
    window.scrollTo(0, 0);
  }, [message]);

  const RecetPassword = async (e) => {
    e.preventDefault();
    setLoading(true);
    setMessage("");

    try {
      const response = await fetch(`${API_BASE_URL}Users/ForgotPassword`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          userProviderIdentifier: email,
          authProvider: "Gmail",
        }),
      });

      const data = await response.json();

      if (response.ok) {
        setMessage(data.message || t("forgotPassword.success", "تم إرسال رابط إعادة تعيين كلمة المرور إلى بريدك الإلكتروني"));
        setMessageType("success");
        setEmail("");
      } else {
        setMessage(data.message || t("forgotPassword.error", "حدث خطأ أثناء إرسال الطلب"));
        setMessageType("error");
      }
    } catch (error) {
      setMessage(t("forgotPassword.serverError", "حدث خطأ أثناء الاتصال بالخادم. يرجى المحاولة مرة أخرى."));
      setMessageType("error");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <Helmet>
        <title>{t("forgotPassword.metaTitle", "استرجاع كلمة السر")} | جومانجو</title>
        <meta
          name="description"
          content={t("forgotPassword.metaDesc", "استعادة كلمة المرور في جومانجو للتمتع بتجربة تسوق مميزة.")}
        />
      </Helmet>

      <NavBar />

      <div className="container mx-auto px-4 py-8 md:py-16">
        <div className="max-w-md mx-auto">
          <div className="bg-white rounded-2xl shadow-xl p-6 md:p-8">
            <div className="text-center mb-6">
              <h2 className="text-2xl md:text-3xl font-bold text-gray-800 mb-2">
                {t("forgotPassword.title", "استعادة كلمة المرور")}
              </h2>
              <p className="text-gray-600 text-sm md:text-base">
                {t("forgotPassword.description", "أدخل بريدك الإلكتروني وسنرسل لك رابط إعادة تعيين كلمة المرور")}
              </p>
            </div>

            {message && (
              <div
                className={`mb-6 p-4 rounded-lg ${
                  messageType === "success"
                    ? "bg-green-50 text-green-800 border border-green-200"
                    : "bg-red-50 text-red-800 border border-red-200"
                }`}
              >
                <p className="text-sm md:text-base font-medium">{message}</p>
              </div>
            )}

            <form onSubmit={RecetPassword} className="space-y-6">
              <div>
                <label
                  htmlFor="email"
                  className="block text-sm font-semibold text-gray-700 mb-2"
                >
                  {t("forgotPassword.email", "البريد الإلكتروني")}
                </label>
                <input
                  type="email"
                  id="email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  placeholder={t("forgotPassword.emailPlaceholder", "أدخل بريدك الإلكتروني")}
                  required
                  className="w-full px-4 py-3 border-2 border-gray-300 rounded-xl focus:border-[#ff7a00] focus:outline-none focus:ring-2 focus:ring-[#ff7a00]/20 transition text-gray-800 placeholder-gray-400"
                  disabled={loading}
                />
              </div>

              <button
                type="submit"
                disabled={loading}
                className="w-full bg-[#ff7a00] hover:bg-orange-600 text-white font-semibold py-3 px-6 rounded-xl transition duration-200 shadow-lg hover:shadow-xl disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2"
              >
                {loading ? (
                  <>
                    <div className="w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin"></div>
                    <span>{t("forgotPassword.sending", "جاري الإرسال...")}</span>
                  </>
                ) : (
                  t("forgotPassword.submit", "إرسال رابط إعادة تعيين كلمة المرور")
                )}
              </button>
            </form>

            <div className="mt-6 text-center">
              <button
                onClick={() => navigate("/login")}
                className="text-[#0a2540] hover:text-[#ff7a00] font-medium text-sm transition"
              >
                {t("forgotPassword.backToLogin", "العودة إلى تسجيل الدخول")}
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
