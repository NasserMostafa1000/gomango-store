import React, { useState, useEffect } from 'react';
import Sidebar from './Sidebar';
import MobileHeader from './MobileHeader';
import { Link, useNavigate } from 'react-router-dom';
import SearchBar from './SearchBar';
import { getRoleFromToken, GetUserNameFromToken, getOrCreateSessionId } from '../utils';
import WebSiteLogo from '../WebsiteLogo/WebsiteLogo';
import CurrencySelector from '../Currency/CurrencySelector';
import { useI18n } from '../i18n/I18nContext';
import { FiUser } from 'react-icons/fi';
import API_BASE_URL from '../Constant';

function StoreLayout({ children }) {
  // السايد بار مغلق افتراضياً ويمكن فتحه/إغلاقه
  const [isSidebarOpen, setIsSidebarOpen] = useState(false);
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [cartCount, setCartCount] = useState(0);
  const navigate = useNavigate();
  const token = sessionStorage.getItem("token");
  const currentRole = getRoleFromToken(token);
  const { lang, setLang, t } = useI18n();

  const HandleSearhOn = (query, displayLabel) => {
    const searchValue = (query || "").trim();
    if (!searchValue) return;
    const label = (displayLabel || query || "").trim() || searchValue;
    const path = `/FindProducts?q=${encodeURIComponent(searchValue)}`;
    navigate(path, { state: { searchQuery: label, apiQuery: searchValue } });
  };

  useEffect(() => {
    if (token) {
      setIsLoggedIn(true);
    }

    // تتبع الزوار الجدد وتحديث نشاطهم
    const trackVisitor = async () => {
      try {
        // التحقق من وجود visitorId في localStorage
        let visitorId = localStorage.getItem('visitorId');
        
        if (!visitorId) {
          // زائر جديد - إضافة زائر جديد
          const ipResponse = await fetch('https://api.ipify.org?format=json');
          const ipData = await ipResponse.json();
          const ipAddress = ipData.ip;

          // محاولة الحصول على معلومات الموقع الجغرافي
          const geoResponse = await fetch(`https://ipapi.co/${ipAddress}/json/`);
          const geoData = await geoResponse.json();

          const visitorData = {
            ipAddress: ipAddress,
            country: geoData.country_name || null,
            countryCode: geoData.country_code || null,
            city: geoData.city || null,
            region: geoData.region || null,
            userAgent: navigator.userAgent,
            referrer: document.referrer || null,
            clientId: null
          };

          const response = await fetch(`${API_BASE_URL}Visitors/AddVisitor`, {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
              ...(token && { Authorization: `Bearer ${token}` }),
            },
            body: JSON.stringify(visitorData),
          });

          if (response.ok) {
            const result = await response.json();
            visitorId = result.visitorId?.toString();
            if (visitorId) {
              localStorage.setItem('visitorId', visitorId);
            }
          }
        } else {
          // زائر موجود - تحديث نشاطه
          try {
            await fetch(`${API_BASE_URL}Visitors/UpdateActivity/${visitorId}`, {
              method: 'PUT',
              headers: {
                'Content-Type': 'application/json',
                ...(token && { Authorization: `Bearer ${token}` }),
              },
            });
          } catch (updateError) {
            console.error('Error updating visitor activity:', updateError);
          }
        }
      } catch (error) {
        // في حالة الفشل، نرسل البيانات الأساسية فقط
        try {
          const visitorData = {
            ipAddress: null,
            country: null,
            countryCode: null,
            city: null,
            region: null,
            userAgent: navigator.userAgent,
            referrer: document.referrer || null,
            clientId: null
          };
          const response = await fetch(`${API_BASE_URL}Visitors/AddVisitor`, {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
              ...(token && { Authorization: `Bearer ${token}` }),
            },
            body: JSON.stringify(visitorData),
          });
          
          if (response.ok) {
            const result = await response.json();
            const visitorId = result.visitorId?.toString();
            if (visitorId) {
              localStorage.setItem('visitorId', visitorId);
            }
          }
        } catch (err) {
          console.error('Error tracking visitor:', err);
        }
      }
    };

    // استدعاء تتبع الزوار مرة واحدة عند تحميل الصفحة
    trackVisitor();
    
    // تحديث نشاط الزوار كل 30 ثانية
    const activityInterval = setInterval(() => {
      const visitorId = localStorage.getItem('visitorId');
      if (visitorId) {
        fetch(`${API_BASE_URL}Visitors/UpdateActivity/${visitorId}`, {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
            ...(token && { Authorization: `Bearer ${token}` }),
          },
        }).catch(err => console.error('Error updating visitor activity:', err));
      }
    }, 30000);

    const fetchCartDetails = async () => {
      try {
        let response;
        
        if (token && currentRole === "User") {
          // المستخدم مسجل دخول - جلب عدد المنتجات من السلة العادية
          response = await fetch(`${API_BASE_URL}Carts/GetCartCount`, {
            method: "GET",
            headers: {
              "Content-Type": "application/json",
              Authorization: `Bearer ${token}`,
            },
          });
        } else if (!token) {
          // المستخدم غير مسجل - جلب عدد المنتجات من السلة المؤقتة
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
          }
          return;
        } else {
          return;
        }
        
        if (!response.ok) throw new Error("Network response was not ok");
        const data = await response.json();
        if (data !== null && data !== undefined) {
          setCartCount(data);
        }
      } catch (error) {
        console.error("Error fetching cart details:", error.message);
      }
    };

    fetchCartDetails();
    
    return () => {
      clearInterval(activityInterval);
    };
  }, [token, currentRole]);

  // إغلاق السايد بار عند تصغير الشاشة للجوال
  useEffect(() => {
    const handleResize = () => {
      if (window.innerWidth < 1024) {
        setIsSidebarOpen(false);
      }
    };

    window.addEventListener('resize', handleResize);
    return () => window.removeEventListener('resize', handleResize);
  }, []);

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

  const isRTL = lang === 'ar';
  const logoWidth = isRTL ? 180 : 200;
  const logoHeight = isRTL ? 58 : 65;

  return (
    <div className="flex min-h-screen bg-[#F9F6EF]">
      {/* Sidebar */}
      <Sidebar isOpen={isSidebarOpen} setIsOpen={setIsSidebarOpen} />

      {/* Main Content */}
      <div className={`flex-1 flex flex-col min-h-screen transition-all duration-300 ${isSidebarOpen ? (isRTL ? 'lg:mr-[280px]' : 'lg:ml-[280px]') : ''}`}>
        {/* Desktop Header */}
        <header className="hidden lg:flex bg-[#F9F6EF] shadow-sm border-b items-center px-4 py-3 flex-shrink-0 z-50 gap-4" style={{ borderColor: '#e5e7eb' }}>
          <div className="flex items-center gap-3 flex-shrink-0">
            <button
              onClick={() => setIsSidebarOpen(!isSidebarOpen)}
              className="flex items-center justify-center w-10 h-10 rounded-lg bg-[#F9F6EF] border border-gray-300 text-[#0A2C52] hover:bg-gray-50 font-bold shadow-sm"
              title={isSidebarOpen ? t("closeMenu", "إغلاق القائمة") : t("openMenu", "فتح القائمة")}
            >
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                {isSidebarOpen ? (
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                ) : (
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
                )}
              </svg>
            </button>
            <button
              onClick={() => navigate("/")}
              className="flex items-center focus:outline-none"
              aria-label={t("home", "الرئيسية")}
            >
              <WebSiteLogo
                width={logoWidth}
                height={logoHeight}
                className={`object-contain transition-all duration-200 ${isRTL ? "max-w-[180px]" : "max-w-[210px]"} xl:max-w-none`}
                style={{ backgroundColor: 'transparent' }}
              />
            </button>
          </div>

          <div className="flex-1 max-w-2xl">
            <SearchBar onSearch={HandleSearhOn} searchType="products" />
          </div>

          <div className="flex items-center gap-3 flex-shrink-0 ml-auto">
            {/* Left Group: Language & Login */}
            <div className="flex items-center gap-2">
              {/* Language Switcher */}
              <button
                onClick={() => setLang(lang === "ar" ? "en" : "ar")}
                className="flex items-center justify-center px-3 py-2 rounded-lg bg-[#F9F6EF] border border-gray-300 text-[#0A2C52] hover:bg-gray-50 font-bold shadow-sm text-xs whitespace-nowrap transition-colors"
                title={lang === "ar" ? t("switchToEnglish", "التبديل إلى الإنجليزية") : t("switchToArabic", "Switch to Arabic")}
              >
                {lang === "ar" ? "EN" : "العربية"}
              </button>

              {/* Login/Logout Button */}
              <button
                onClick={handleLoginLogout}
                className={`px-3 py-2 rounded-lg text-white font-semibold transition duration-200 text-xs shadow-md whitespace-nowrap ${
                  isLoggedIn 
                    ? "bg-red-600 hover:bg-red-700" 
                    : "bg-[#0A2C52] hover:bg-[#13345d]"
                }`}
              >
                {isLoggedIn ? t("logout", "خروج") : t("login", "دخول")}
              </button>
            </div>

            {/* Middle Group: Currency & Profile */}
            <div className="flex items-center gap-2">
              {/* Currency Selector */}
              <CurrencySelector />

              {/* User Profile */}
              {isLoggedIn && (
                <Link 
                  to="/MyProfile" 
                  className="flex items-center gap-1.5 bg-emerald-600 hover:bg-emerald-700 text-white px-3 py-2 rounded-lg transition duration-200 text-xs font-semibold shadow-md"
                >
                  <FiUser size={14} />
                  <span className="max-w-20 truncate">
                    {GetUserNameFromToken(token)}
                  </span>
                </Link>
              )}
            </div>

            {/* Right Group: Cart */}
            {currentRole === "User" && (
              <div className="flex items-center">
                <button 
                  className="relative flex items-center gap-1.5 text-[#F55A00] hover:text-[#E04F00] px-3 py-2 rounded-lg transition duration-200 text-xs font-semibold"
                  onClick={HandleCartClick}
                >
                  <span className="hidden xl:inline">{t("cart.title", "السلة")}</span>
                  <img 
                    src="/ProjectImages/gomangoCart.png" 
                    alt="Cart" 
                    className="w-4 h-4 object-contain"
                  />
                  {cartCount > 0 && (
                    <span className="absolute -top-1 -right-1 bg-red-500 text-white text-xs rounded-full w-4 h-4 flex items-center justify-center font-bold shadow-lg text-[10px]">
                      {cartCount}
                    </span>
                  )}
                </button>
              </div>
            )}
          </div>
        </header>

        {/* Mobile Header */}
        <MobileHeader onMenuClick={() => setIsSidebarOpen(!isSidebarOpen)} />

        {/* Mobile Search Bar */}
        <div className="lg:hidden px-4 py-3 bg-[#F9F6EF] border-b" style={{ borderColor: '#e5e7eb' }}>
          <SearchBar onSearch={HandleSearhOn} searchType="products" />
        </div>

        {/* Page Content */}
        <main className="flex-1 bg-[#F9F6EF]">
          <div className="w-full">
            {children}
          </div>
        </main>
      </div>
    </div>
  );
}

export default StoreLayout;

