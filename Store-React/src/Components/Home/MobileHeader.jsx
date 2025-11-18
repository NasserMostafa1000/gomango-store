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
          // المستخدم مسجل دخول - جلب عدد المنتجات من السلة العادية
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
          // المستخدم غير مسجل أو ليس User - جلب عدد المنتجات من السلة المؤقتة
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

  return (
    <header className="lg:hidden bg-[#F9F6EF] shadow-sm border-b sticky top-0 z-30 w-full" style={{ borderColor: '#e5e7eb', width: '100%', maxWidth: '100%', position: 'sticky', left: 0, right: 0 }}>
      <div className="flex items-center justify-between p-2 sm:p-3 gap-1 sm:gap-2">
        <div className="flex items-center gap-3 flex-1 min-w-0">
          <button
            onClick={onMenuClick}
            className="text-gray-700 hover:text-gray-900 flex-shrink-0"
          >
            <FiMenu size={20} />
          </button>
          <button
            onClick={() => navigate("/")}
            className="flex items-center justify-center flex-shrink-0 overflow-hidden"
            aria-label={t("home", "الرئيسية")}
          >
            <WebSiteLogo width={80} height={80} className="object-contain w-20 h-20" />
          </button>
        </div>
        
        {/* Right Side Actions */}
        <div className="flex items-center gap-1 sm:gap-2 flex-shrink-0">
          {/* Cart - Always visible */}
          <button 
            onClick={handleCartClick}
            className="relative flex items-center justify-center w-8 h-8 sm:w-9 sm:h-9 bg-transparent hover:bg-gray-100 rounded-lg transition duration-200 flex-shrink-0"
            title={t("cart", "السلة")}
          >
            <img 
              src="/ProjectImages/gomangoCart.png" 
              alt="Cart" 
              className="w-6 h-6 sm:w-7 sm:h-7 object-contain"
            />
            {cartCount > 0 && (
              <span className="absolute -top-1 -right-1 bg-red-500 text-white text-xs rounded-full w-4 h-4 flex items-center justify-center font-bold">
                {cartCount > 9 ? '9+' : cartCount}
              </span>
            )}
          </button>

          {/* Language Switcher */}
          <button
            onClick={() => setLang(lang === "ar" ? "en" : "ar")}
            className="flex items-center justify-center px-2 py-1.5 rounded-lg bg-[#F9F6EF] border border-gray-300 text-[#0A2C52] hover:bg-gray-50 font-bold text-xs shadow-sm flex-shrink-0"
            title={lang === "ar" ? t("switchToEnglish", "التبديل إلى الإنجليزية") : "عربي"}
          >
            {lang === "ar" ? "EN" : "عربي"}
          </button>

          {/* Login/Logout Button */}
          <button
            onClick={handleLoginLogout}
            className={`px-2 sm:px-3 py-1 sm:py-1.5 rounded-lg text-white font-semibold transition duration-200 text-xs shadow-sm whitespace-nowrap flex-shrink-0 ${
              isLoggedIn 
                ? "bg-red-600 hover:bg-red-700" 
                : "bg-[#0A2C52] hover:bg-[#13345d]"
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

