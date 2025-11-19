import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { FiMenu } from 'react-icons/fi';
import { useI18n } from '../i18n/I18nContext';
import WebSiteLogo from '../WebsiteLogo/WebsiteLogo';
import { getRoleFromToken, getOrCreateSessionId } from '../utils';
import API_BASE_URL from '../Constant';

function MobileHeader({ onMenuClick }) {
  const { t, lang, setLang } = useI18n();
  const navigate = useNavigate();
  const token = sessionStorage.getItem("token");
  const currentRole = getRoleFromToken(token);
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [cartCount, setCartCount] = useState(0);

  useEffect(() => {
    setIsLoggedIn(!!token);
  }, [token]);

  useEffect(() => {
    const fetchCartDetails = async () => {
      try {
        if (token && currentRole === "User") {
          const response = await fetch(`${API_BASE_URL}Carts/GetCartCount`, {
            method: "GET",
            headers: {
              "Content-Type": "application/json",
              Authorization: `Bearer ${token}`,
            },
          });
          
          if (response.ok) {
            const data = await response.json();
            if (data !== null && data !== undefined) {
              setCartCount(data);
            } else {
              setCartCount(0);
            }
          }
        } else {
          const sessionId = getOrCreateSessionId();
          const cartResponse = await fetch(`${API_BASE_URL}Carts/GetGuestCartDetails`, {
            method: "GET",
            headers: {
              "Content-Type": "application/json",
              sessionId: sessionId,
            },
          });
          
          if (cartResponse.ok) {
            const cartData = await cartResponse.json();
            setCartCount(cartData?.length || 0);
          } else {
            setCartCount(0);
          }
        }
      } catch (error) {
        console.error("Error fetching cart details:", error.message);
        setCartCount(0);
      }
    };

    fetchCartDetails();
  }, [token, currentRole]);

  const handleLoginLogout = () => {
    if (isLoggedIn) {
      sessionStorage.removeItem("token");
      setIsLoggedIn(false);
      window.location.reload();
    } else {
      navigate("/login");
    }
  };

  const handleCartClick = () => {
    navigate("/Cart");
  };

// ... (نفس الكود السابق حتى الـ return)

return (
  <header className="lg:hidden bg-[#F9F6EF] shadow-sm border-b border-[#e5e7eb] sticky top-0 z-30 w-full">
    <div className="grid grid-cols-3 items-center px-4 py-3 w-full">
      
      {/* الجانب الأيسر */}
      <div className="flex items-center gap-4 justify-self-start">
        <button
          onClick={onMenuClick}
          className="text-gray-700 hover:text-gray-900 p-2.5 rounded-xl hover:bg-white transition-all duration-200 shadow-sm border border-gray-200 bg-white/80"
          aria-label={t("menu", "القائمة")}
        >
          <FiMenu size={20} />
        </button>
        
        <button 
          onClick={handleCartClick}
          className="relative flex items-center justify-center p-2.5 bg-white/80 hover:bg-white rounded-xl transition-all duration-200 shadow-sm border border-gray-200"
          title={t("cart.title", "السلة")}
        >
          <img 
            src="/ProjectImages/gomangoCart.png" 
            alt="Cart" 
            className="w-6 h-6 object-contain"
          />
          {cartCount > 0 && (
            <span className="absolute -top-2 -right-2 bg-red-500 text-white text-[11px] rounded-full w-5 h-5 flex items-center justify-center font-bold border-2 border-white shadow-sm">
              {cartCount > 9 ? '9+' : cartCount}
            </span>
          )}
        </button>
      </div>

      {/* اللوجو في المنتصف */}
      <div className="flex items-center justify-center justify-self-center">
        <button
          onClick={() => navigate("/")}
          className="flex items-center justify-center hover:scale-105 transition-transform duration-200"
          aria-label={t("home", "الرئيسية")}
        >
          <WebSiteLogo width={90} height={90} className="object-contain drop-shadow-sm w-[90px] h-[90px]" />
        </button>
      </div>

      {/* الجانب الأيمن */}
      <div className="flex items-center gap-3 justify-self-end">
        <button
          onClick={() => setLang(lang === "ar" ? "en" : "ar")}
          className="flex items-center justify-center px-4 py-2.5 rounded-xl bg-white border border-gray-300 text-[#0A2C52] hover:bg-gray-50 font-semibold text-sm shadow-sm transition-all duration-200 min-w-[65px] hover:shadow-md"
          title={lang === "ar" ? t("switchToEnglish", "التبديل إلى الإنجليزية") : "عربي"}
        >
          {lang === "ar" ? "EN" : "عربي"}
        </button>

        <button
          onClick={handleLoginLogout}
          className={`px-5 py-2.5 rounded-xl text-white font-semibold transition-all duration-200 text-sm shadow-sm whitespace-nowrap min-w-[85px] hover:shadow-md ${
            isLoggedIn 
              ? "bg-gradient-to-r from-red-500 to-red-600 hover:from-red-600 hover:to-red-700" 
              : "bg-gradient-to-r from-[#0A2C52] to-[#13345d] hover:from-[#13345d] hover:to-[#1a4670]"
          }`}
        >
          {isLoggedIn ? t("logout", "خروج") : t("login", "دخول")}
        </button>
      </div>
    </div>
  </header>
);
}
export default MobileHeader;