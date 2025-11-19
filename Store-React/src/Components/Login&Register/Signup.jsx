import { useEffect, useState } from "react";
import API_BASE_URL, { SiteName } from "../Constant";
import { Helmet } from "react-helmet";
import WebSiteLogo from "../WebsiteLogo/WebsiteLogo.jsx";
import { useI18n } from "../i18n/I18nContext";
import { FiEye, FiEyeOff } from "react-icons/fi";

export default function Signup() {
  const [formData, setFormData] = useState({
    firstName: "",
    lastName: "",
    email: "",
    phone: "",
    password: "",
    confirmPassword: "",
  });

  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [message, setMessage] = useState("");
  const [messageType, setMessageType] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const { t } = useI18n();

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  useEffect(() => {
    window.scrollTo(0, 0);
  }, [message]);

  const isPasswordStrong = (password) => {
    const passwordRegex =
      /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{8,}$/;
    return passwordRegex.test(password);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsLoading(true);

    if (formData.password !== formData.confirmPassword) {
      setMessage(t("signup.passwordMismatch", "كلمات المرور غير متطابقة"));
      setMessageType("error");
      setIsLoading(false);
      return;
    }

    if (!isPasswordStrong(formData.password)) {
      setMessage(
        t("signup.passwordRequirements", "كلمة المرور يجب أن تحتوي على: 8 أحرف على الأقل، حرف كبير، رقم، ورمز خاص")
      );
      setMessageType("error");
      setIsLoading(false);
      return;
    }

    try {
      const response = await fetch(`${API_BASE_URL}Clients/PostClient`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          firstName: formData.firstName,
          secondName: formData.lastName,
          email: formData.email,
          phoneNumber: formData.phone,
          password: formData.password,
        }),
      });
      const data = await response.json();
      if (response.ok) {
        setMessage(t("signup.success", "تم التسجيل بنجاح!"));
        setMessageType("success");
        setFormData({
          firstName: "",
          lastName: "",
          email: "",
          phone: "",
          password: "",
          confirmPassword: "",
        });
      } else {
        setMessage(data.message);
        setMessageType("error");
      }
    } catch (error) {
      setMessage(error.message);
      setMessageType("error");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 to-orange-50 py-8 rtl">
      <Helmet>
        <title>{t("signup.metaTitle", "انشاء حساب")} | {SiteName}</title>
        <meta name="description" content={t("signup.metaDesc", "انشاء حساب في متجرنا الالكتروني")} />
      </Helmet>

      <div className="container mx-auto px-4 max-w-md">
        <div className="bg-white rounded-2xl shadow-xl p-8 border-2 border-orange-500">
          {/* الشعار */}
          <div className="text-center mb-8">
            <div className="flex justify-center mb-4">
              <WebSiteLogo width={200} height={100} />
            </div>
            <h1 className="text-3xl font-bold text-blue-900">{t("signup.title", "إنشاء حساب جديد")}</h1>
            <p className="text-gray-600 mt-2">{t("signup.subtitle", "انضم إلى عائلة")} {SiteName}</p>
          </div>

          {/* رسالة التنبيه */}
          {message && (
            <div
              className={`p-4 rounded-xl mb-6 text-center font-semibold ${
                messageType === "success"
                  ? "bg-green-100 text-green-800 border border-green-200"
                  : "bg-red-100 text-red-800 border border-red-200"
              }`}
            >
              {message}
            </div>
          )}

          {/* النموذج */}
          <form onSubmit={handleSubmit} className="space-y-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-semibold text-blue-900 mb-2">
                  {t("signup.firstName", "الاسم الأول")}
                </label>
                <input
                  type="text"
                  name="firstName"
                  value={formData.firstName}
                  onChange={handleChange}
                  required
                  className="w-full px-4 py-3 border-2 border-gray-300 rounded-lg focus:border-orange-500 focus:outline-none focus:ring-2 focus:ring-orange-200 transition-colors"
                  placeholder={t("signup.firstNamePlaceholder", "أدخل الاسم الأول")}
                />
              </div>

              <div>
                <label className="block text-sm font-semibold text-blue-900 mb-2">
                  {t("signup.lastName", "الاسم الأخير")}
                </label>
                <input
                  type="text"
                  name="lastName"
                  value={formData.lastName}
                  onChange={handleChange}
                  required
                  className="w-full px-4 py-3 border-2 border-gray-300 rounded-lg focus:border-orange-500 focus:outline-none focus:ring-2 focus:ring-orange-200 transition-colors"
                  placeholder={t("signup.lastNamePlaceholder", "أدخل الاسم الأخير")}
                />
              </div>
            </div>

            <div>
              <label className="block text-sm font-semibold text-blue-900 mb-2">
                {t("signup.email", "البريد الإلكتروني")}
              </label>
              <input
                type="email"
                name="email"
                value={formData.email}
                onChange={handleChange}
                required
                className="w-full px-4 py-3 border-2 border-gray-300 rounded-lg focus:border-orange-500 focus:outline-none focus:ring-2 focus:ring-orange-200 transition-colors"
                placeholder="example@email.com"
              />
            </div>

            <div>
              <label className="block text-sm font-semibold text-blue-900 mb-2">
                {t("signup.phone", "رقم الهاتف")}
              </label>
              <input
                type="text"
                name="phone"
                value={formData.phone}
                onChange={handleChange}
                required
                className="w-full px-4 py-3 border-2 border-gray-300 rounded-lg focus:border-orange-500 focus:outline-none focus:ring-2 focus:ring-orange-200 transition-colors"
                placeholder="01XXXXXXXXX"
              />
            </div>

            <div>
              <label className="block text-sm font-semibold text-blue-900 mb-2">
                {t("signup.password", "كلمة المرور")}
              </label>
              <div className="relative">
                <input
                  type={showPassword ? "text" : "password"}
                  name="password"
                  value={formData.password}
                  onChange={handleChange}
                  required
                  className="w-full px-4 py-3 pr-12 border-2 border-gray-300 rounded-lg focus:border-orange-500 focus:outline-none focus:ring-2 focus:ring-orange-200 transition-colors"
                  placeholder={t("signup.passwordPlaceholder", "أدخل كلمة المرور")}
                />
                <button
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-500 hover:text-orange-500 transition-colors duration-200 focus:outline-none"
                  aria-label={showPassword ? "إخفاء كلمة المرور" : "إظهار كلمة المرور"}
                >
                  {showPassword ? <FiEyeOff size={20} /> : <FiEye size={20} />}
                </button>
              </div>
              <p className="text-xs text-gray-600 mt-2">
                {t("signup.passwordRequirements", "يجب أن تحتوي على: 8 أحرف على الأقل، حرف كبير، رقم، ورمز خاص")}
              </p>
            </div>

            <div>
              <label className="block text-sm font-semibold text-blue-900 mb-2">
                {t("signup.confirmPassword", "تأكيد كلمة المرور")}
              </label>
              <div className="relative">
                <input
                  type={showConfirmPassword ? "text" : "password"}
                  name="confirmPassword"
                  value={formData.confirmPassword}
                  onChange={handleChange}
                  required
                  className="w-full px-4 py-3 pr-12 border-2 border-gray-300 rounded-lg focus:border-orange-500 focus:outline-none focus:ring-2 focus:ring-orange-200 transition-colors"
                  placeholder={t("signup.confirmPasswordPlaceholder", "أعد إدخال كلمة المرور")}
                />
                <button
                  type="button"
                  onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                  className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-500 hover:text-orange-500 transition-colors duration-200 focus:outline-none"
                  aria-label={showConfirmPassword ? "إخفاء كلمة المرور" : "إظهار كلمة المرور"}
                >
                  {showConfirmPassword ? <FiEyeOff size={20} /> : <FiEye size={20} />}
                </button>
              </div>
            </div>

            <button
              type="submit"
              disabled={isLoading}
              className="w-full bg-gradient-to-l from-orange-500 to-blue-900 text-white py-4 rounded-xl font-semibold text-lg hover:from-orange-600 hover:to-blue-800 transition-all shadow-lg hover:shadow-xl disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {isLoading ? (
                <div className="flex items-center justify-center">
                  <div className="w-5 h-5 border-t-2 border-b-2 border-white rounded-full animate-spin ml-2"></div>
                  {t("signup.registering", "جاري التسجيل...")}
                </div>
              ) : (
                t("signup.submit", "تسجيل الحساب")
              )}
            </button>
          </form>

          {/* رابط تسجيل الدخول */}
          <div className="text-center mt-6 pt-6 border-t border-gray-200">
            <p className="text-gray-600">
              {t("signup.haveAccount", "لديك حساب بالفعل؟")}{" "}
              <a
                href="/login"
                className="text-orange-600 hover:text-blue-900 font-semibold transition-colors"
              >
                {t("signup.loginHere", "سجل الدخول هنا")}
              </a>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
