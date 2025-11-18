import React, { useState, useEffect } from "react";
import { Link, useNavigate } from "react-router-dom";
import SearchBar from "./SearchBar.jsx";
import { getRoleFromToken, GetUserNameFromToken } from "../utils.js";
import API_BASE_URL from "../Constant.js";
import WebSiteLogo from "../WebsiteLogo/WebsiteLogo.jsx";
import CurrencySelector from "../Currency/CurrencySelector";
import { useI18n } from "../i18n/I18nContext";
import Sidebar from "./Sidebar.jsx";
import MobileHeader from "./MobileHeader.jsx";
import { FiUser, FiMenu, FiX } from "react-icons/fi";
import { getOrCreateSessionId } from "../utils";

export default function NavBar() {
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [cartCount, setCartCount] = useState(0);
  const [isSidebarOpen, setIsSidebarOpen] = useState(false);
  const navigate = useNavigate();
  const token = sessionStorage.getItem("token");
  const currentRole = getRoleFromToken(token);
  const { lang, setLang, t } = useI18n();
  
  // Language switcher text
  const languageButtonText = lang === "ar" ? "EN" : "عربي";

  const HandleSearhOn = (query, displayLabel) => {
    const searchValue = (query || "").trim();
    if (!searchValue) return;
    const label = (displayLabel || query || "").trim() || searchValue;
    const path = `/FindProducts?q=${encodeURIComponent(searchValue)}`;
    navigate(path, { state: { searchQuery: label, apiQuery: searchValue } });
  };

  useEffect(() => {
    // تحديث حالة تسجيل الدخول
    setIsLoggedIn(!!token);

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

  // لا نغلق السايد بار تلقائياً - المستخدم يتحكم به

  const HandleCartClick = () => {
    // السماح بالوصول للسلة حتى بدون تسجيل دخول
      navigate("/Cart");
  };

  const handleLoginLogout = () => {
    if (isLoggedIn) {
      sessionStorage.removeItem("token");
      setIsLoggedIn(false);
      window.location.reload();
    } else {
      navigate("/login");
    }
  };

      return (
    <div className="relative w-full">
      {/* Sidebar */}
      <Sidebar isOpen={isSidebarOpen} setIsOpen={setIsSidebarOpen} />

      {/* Main Content Area */}
      <div className={`flex-1 flex flex-col transition-all duration-300 relative ${
        isSidebarOpen ? 'lg:ml-[280px]' : 'lg:ml-0'
      }`}>
        {/* Desktop Header */}
        <header className="hidden lg:flex bg-transparent/60 backdrop-blur border-b border-white/30 justify-between items-center px-6 py-4 flex-shrink-0 relative z-50">
          <div className="flex items-center gap-4 flex-1">
            {/* Menu Toggle Button */}
            <button
              onClick={() => setIsSidebarOpen(!isSidebarOpen)}
              className="flex items-center justify-center w-10 h-10 rounded-lg bg-white border border-gray-300 text-[#0A2C52] hover:bg-gray-50 font-bold shadow-sm transition-all duration-200"
              title={isSidebarOpen ? t("closeMenu", "إغلاق القائمة") : t("openMenu", "فتح القائمة")}
            >
              {isSidebarOpen ? (
                <FiX size={20} />
              ) : (
                <FiMenu size={20} />
              )}
            </button>
            
            {/* Logo */}
            <button
              onClick={() => navigate("/")}
              className="flex items-center gap-3 focus:outline-none"
              aria-label={t("home", "الرئيسية")}
            >
              <WebSiteLogo width={180} height={58} className="object-contain" />
            </button>

            {/* Search Bar */}
            <div className="flex-1 max-w-2xl mx-4">
              <SearchBar onSearch={HandleSearhOn} searchType="products" />
            </div>
          </div>

          {/* Right Side Actions */}
          <div className="flex items-center gap-3">
            {/* Language Switcher */}
            <button
              onClick={() => setLang(lang === "ar" ? "en" : "ar")}
              className="flex items-center justify-center px-3 py-2 rounded-xl bg-white border border-gray-300 text-[#0A2C52] hover:bg-gray-50 font-bold shadow-sm"
              title={lang === "ar" ? t("switchToEnglish", "التبديل إلى الإنجليزية") : "عربي"}
            >
              {languageButtonText}
            </button>

            {/* User Profile */}
            {isLoggedIn && (
              <Link 
                to="/MyProfile" 
                className="flex items-center gap-2 bg-emerald-600 hover:bg-emerald-700 text-white px-4 py-2.5 rounded-xl transition duration-200 text-sm font-semibold shadow-md"
              >
                <FiUser size={16} />
                <span className="max-w-24 truncate">
                  {GetUserNameFromToken(token)}
                </span>
              </Link>
            )}
            
            {/* Cart - Always visible for both logged in and guest users */}
              <button 
              className="relative flex items-center gap-2 bg-transparent hover:bg-gray-100 text-gray-700 px-4 py-2.5 rounded-xl transition duration-200 text-sm font-semibold"
                onClick={HandleCartClick}
              >
                <span>{t("cart", "السلة")}</span>
                <img 
                  src="/ProjectImages/gomangoCart.png" 
                  alt="Cart" 
                  className="w-8 h-8 object-contain"
                />
                {cartCount > 0 && (
                  <span className="absolute -top-2 -right-2 bg-red-500 text-white text-xs rounded-full w-5 h-5 flex items-center justify-center font-bold shadow-lg">
                    {cartCount}
                  </span>
                )}
              </button>
            
            {/* Currency Selector */}
              <CurrencySelector />
            
            {/* Login/Logout Button */}
            <button
              onClick={handleLoginLogout}
              className={`px-4 py-2.5 rounded-xl text-white font-semibold transition duration-200 text-sm shadow-md whitespace-nowrap ${
                isLoggedIn 
                  ? "bg-red-600 hover:bg-red-700" 
                  : "bg-[#0A2C52] hover:bg-[#13345d]"
              }`}
            >
              {isLoggedIn ? t("logout", "تسجيل خروج") : t("login", "تسجيل دخول")}
            </button>
          </div>
        </header>

        {/* Mobile Header */}
        <MobileHeader onMenuClick={() => setIsSidebarOpen(!isSidebarOpen)} />

        {/* Mobile Search Bar */}
        <div className="lg:hidden px-2 sm:px-4 py-2 bg-white border-b" style={{ borderColor: '#e5e7eb' }}>
          <SearchBar onSearch={HandleSearhOn} searchType="products" />
        </div>
      </div>
    </div>
  );
}
