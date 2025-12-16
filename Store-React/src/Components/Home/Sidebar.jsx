import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import {
  FiHome,
  FiUser,
  FiBox,
  FiClipboard,
  FiUsers,
  FiTruck,
  FiTool,
  FiSettings,
  FiSearch,
  FiInfo,
  FiPhone,
  FiX,
  FiEye,
  FiTag,
  FiFileText,
  FiVolume2,
} from 'react-icons/fi';
import { getRoleFromToken, GetUserNameFromToken } from '../utils';
import { useI18n } from '../i18n/I18nContext';
import WebSiteLogo from '../WebsiteLogo/WebsiteLogo';
import CurrencySelector from '../Currency/CurrencySelector';
import API_BASE_URL from '../Constant';
import { trackViewCategory } from '../utils/facebookPixel';

function Sidebar({ isOpen, setIsOpen }) {
  const { t, lang } = useI18n();
  const navigate = useNavigate();
  const location = useLocation();
  const isRTL = lang === 'ar';
  const token = sessionStorage.getItem("token");
  const currentRole = getRoleFromToken(token);
  const rolesList = (currentRole || "").split(/[,\s]+/).filter(Boolean);
  const isAdmin = rolesList.includes("Admin");
  const isManager = rolesList.includes("Manager");
  const isShipping = rolesList.includes("Shipping") || rolesList.includes("ShippingMan");
  const isLoggedIn = !!token;
  
  const [isDesktop, setIsDesktop] = useState(window.innerWidth >= 1024);
  const [categories, setCategories] = useState([]);
  const [categoriesLoading, setCategoriesLoading] = useState(true);

  useEffect(() => {
    const handleResize = () => {
      setIsDesktop(window.innerWidth >= 1024);
    };
    window.addEventListener("resize", handleResize);
    return () => window.removeEventListener("resize", handleResize);
  }, []);

  const handleLogout = () => {
    sessionStorage.removeItem("token");
    window.location.reload();
  };

  const handleNavigation = (path) => {
    navigate(path);
    if (window.innerWidth < 1024) {
      setIsOpen(false);
    }
  };

  const handleCategoryClick = (category, displayLabel) => {
    const query = (category || "").trim();
    if (!query) return;
    const label = (displayLabel || category || "").trim() || query;
    // تتبع ViewCategory لـ Facebook Pixel
    trackViewCategory(label);
    const path = `/FindProducts?q=${encodeURIComponent(query)}`;
    navigate(path, { state: { searchQuery: label, apiQuery: query } });
    if (window.innerWidth < 1024) {
      setIsOpen(false);
    }
  };

  useEffect(() => {
    let isMounted = true;
    const fetchCategories = async () => {
      try {
        setCategoriesLoading(true);
        const response = await fetch(
          `${API_BASE_URL}categories?lang=${lang || "ar"}`
        );
        if (!response.ok) throw new Error("failed");
        const data = await response.json();
        if (isMounted) {
          setCategories(data);
        }
      } catch {
        if (isMounted) setCategories([]);
      } finally {
        if (isMounted) setCategoriesLoading(false);
      }
    };
    fetchCategories();
    return () => {
      isMounted = false;
    };
  }, [lang]);

  const getMenuItems = () => {
    if (isAdmin) {
      return [
        { path: '/', label: t("home", "الرئيسية"), icon: FiHome },
        { path: '/MyProfile', label: t("profile", "الملف الشخصي"), icon: FiUser },
        { path: '/admin/banners', label: t("banners", "البانرز"), icon: FiBox },
        { path: '/admin/currency', label: t("currencies", "العملات"), icon: FiSettings },
        { path: '/admins/AddProduct', label: t("products", "المنتجات"), icon: FiBox },
        { path: '/admin/categories', label: t("categories", "التصنيفات"), icon: FiTag },
        { path: '/admin/announcement-bar', label: t("sidebar.announcement", "إدارة شريط الإعلانات"), icon: FiVolume2 },
        { path: '/admin/legal-content', label: t("sidebar.legal", "الشروط والخصوصية"), icon: FiFileText },
        { path: '/admin/UpdateAdminInfo', label: t("sidebar.adminInfo", "معلومات التواصل"), icon: FiPhone },
        { path: '/Admin/Orders', label: t("orders", "الطلبات"), icon: FiClipboard },
        { path: '/Admin/Clients', label: t("clients", "العملاء"), icon: FiUsers },
        { path: '/Admin/Visitors', label: t("visitors", "الزوار"), icon: FiEye },
        { path: '/Admin/ClientsSearshing', label: t("clientSearches", "بحث العملاء"), icon: FiSearch },
        { path: '/Admin/shipping-prices', label: t("shipping", "الشحن"), icon: FiTruck },
        { path: '/admin/reviews', label: t("reviews.title", "التعليقات"), icon: FiBox },
        { path: '/admins/Employees', label: t("employees", "الموظفين"), icon: FiTool },
      ];
    } else if (isManager) {
      return [
        { path: '/', label: t("home", "الرئيسية"), icon: FiHome },
        { path: '/MyProfile', label: t("profile", "الملف الشخصي"), icon: FiUser },
        { path: '/admin/banners', label: t("banners", "البانرز"), icon: FiBox },
        { path: '/admin/currency', label: t("currencies", "العملات"), icon: FiSettings },
        { path: '/admin/reviews', label: t("reviews.title", "التعليقات"), icon: FiBox },
        { path: '/admins/AddProduct', label: t("products", "المنتجات"), icon: FiBox },
        { path: '/admin/categories', label: t("categories", "التصنيفات"), icon: FiTag },
        { path: '/admin/announcement-bar', label: t("sidebar.announcement", "إدارة شريط الإعلانات"), icon: FiVolume2 },
        { path: '/admin/legal-content', label: t("sidebar.legal", "الشروط والخصوصية"), icon: FiFileText },
        { path: '/admin/UpdateAdminInfo', label: t("sidebar.adminInfo", "معلومات التواصل"), icon: FiPhone },
        { path: '/Admin/Orders', label: t("orders", "الطلبات"), icon: FiClipboard },
        { path: '/Admin/Visitors', label: t("visitors", "الزوار"), icon: FiEye },
      ];
    } else if (isShipping) {
      return [
        { path: '/', label: t("home", "الرئيسية"), icon: FiHome },
        { path: '/MyProfile', label: t("profile", "الملف الشخصي"), icon: FiUser },
        { path: '/Admin/Orders', label: t("orders", "الطلبات"), icon: FiClipboard },
      ];
    } else if (isLoggedIn) {
      return [
        { path: '/', label: t("home", "الرئيسية"), icon: FiHome },
        { path: '/MyProfile', label: t("profile", "الملف الشخصي"), icon: FiUser },
        { path: '/MyPurchases', label: t("myPurchases.title", "طلباتي"), icon: FiClipboard },
        { path: '/MyReviews', label: t("myReviews.title", "تعليقاتي"), icon: FiBox },
      ];
    } else {
      return [
        { path: '/', label: t("home", "الرئيسية"), icon: FiHome },
      ];
    }
  };

  const menuItems = getMenuItems();

  return (
    <>
      {/* Overlay يظهر فقط على الجوال */}
      {!isDesktop && isOpen && (
        <div
          className="fixed inset-0 bg-black bg-opacity-50 z-[55]"
          onClick={() => setIsOpen(false)}
        ></div>
      )}

            {/* Sidebar */}
            <aside
        className={`fixed top-0 h-screen shadow-xl transform transition-transform duration-300 ease-in-out ${
          isDesktop
            ? (isRTL
                ? (isOpen ? 'translate-x-0 right-0' : 'translate-x-full right-0')
                : (isOpen ? 'translate-x-0 left-0' : '-translate-x-full left-0'))
            : (isRTL
                  ? (isOpen ? 'translate-x-0 right-0' : 'translate-x-full right-0')
                : (isOpen ? 'translate-x-0 left-0' : '-translate-x-full left-0'))
              }`}
        style={{ 
          width: '280px', 
          background: 'linear-gradient(180deg, rgba(255,250,245,0.96), rgba(248,244,235,0.9))',
          backdropFilter: 'blur(16px)',
          WebkitBackdropFilter: 'blur(16px)',
          borderRight: isRTL ? '1px solid rgba(17,24,39,0.08)' : undefined,
          borderLeft: !isRTL ? '1px solid rgba(17,24,39,0.08)' : undefined,
          zIndex: isDesktop ? 100 : 60
        }}
        dir={isRTL ? 'rtl' : 'ltr'}
            >
        <div className="flex flex-col h-full">
          {/* Header */}
          <div className="p-6 border-b" style={{ borderColor: 'rgba(15, 23, 42, 0.08)' }}>
            <div className={`flex items-center justify-between mb-4 ${isRTL ? 'flex-row-reverse' : ''}`} dir={isRTL ? 'rtl' : 'ltr'}>
              <button
                onClick={() => {
                  handleNavigation('/');
                }}
                className={`flex items-center gap-3 ${isRTL ? 'flex-row-reverse' : ''}`}
                aria-label={t("home", "الرئيسية")}
              >
                <WebSiteLogo width={40} height={40} className="object-contain" />
                <h2 className="text-lg font-bold text-gray-900" style={{ textAlign: isRTL ? 'right' : 'left' }}>
                  {t("home", "الرئيسية")}
                </h2>
              </button>
              <button
                onClick={() => setIsOpen(false)}
                className="text-gray-700 hover:text-gray-900"
              >
                <FiX size={24} />
              </button>
            </div>
            
            {isLoggedIn && (
              <div className={`flex items-center gap-3 mt-4 pt-4 ${isRTL ? 'flex-row-reverse' : ''}`} style={{ borderTop: '1px solid rgba(15, 23, 42, 0.08)' }} dir={isRTL ? 'rtl' : 'ltr'}>
                <div className="w-12 h-12 rounded-full flex items-center justify-center flex-shrink-0 bg-[#0a2540]/10">
                  <FiUser className="w-6 h-6 text-[#0a2540]" />
                </div>
                <div className="flex-1 min-w-0">
                  <p className="text-gray-900 font-semibold text-sm truncate" style={{ textAlign: isRTL ? 'right' : 'left' }}>
                    {GetUserNameFromToken(token) || t("account", "حسابي")}
                  </p>
                  <p className="text-gray-600 text-xs truncate" style={{ textAlign: isRTL ? 'right' : 'left' }}>
                    {currentRole || t("user", "مستخدم")}
                  </p>
                </div>
              </div>
            )}
          </div>

          {/* Menu */}
          <nav className="flex-1 overflow-y-auto p-4" dir={isRTL ? 'rtl' : 'ltr'}>
            <ul className="space-y-2">
              {menuItems.map((item) => {
                const isActive = location.pathname === item.path;
                const Icon = item.icon;
                return (
                  <li key={item.path}>
                    <button
                      onClick={() => handleNavigation(item.path)}
                      className={`w-full flex items-center gap-3 px-4 py-3 rounded-lg transition-all whitespace-nowrap border ${
                        isActive
                          ? 'bg-[#0a2540] text-white border-transparent shadow-lg'
                          : 'bg-white/70 text-gray-900 hover:bg-white border-transparent shadow-sm'
                      } ${isRTL ? 'flex-row-reverse' : ''}`}
                      dir={isRTL ? 'rtl' : 'ltr'}
                      style={{ 
                        textAlign: isRTL ? 'right' : 'left',
                        direction: isRTL ? 'rtl' : 'ltr'
                      }}
                    >
                      <Icon className={`w-5 h-5 flex-shrink-0 ${isActive ? 'text-white' : 'text-[#0a2540]'}`} style={{ order: isRTL ? 2 : 0 }} />
                      <span className="text-sm font-medium flex-1" style={{ textAlign: isRTL ? 'right' : 'left', direction: isRTL ? 'rtl' : 'ltr' }}>{item.label}</span>
                    </button>
                  </li>
                );
              })}

              {/* Login/Logout Button - Mobile Only */}
              <li className="lg:hidden">
                <button
                  onClick={() => {
                    if (isLoggedIn) {
                      handleLogout();
                    } else {
                      handleNavigation('/login');
                    }
                  }}
                  className={`w-full flex items-center gap-3 px-4 py-3 rounded-lg transition-all whitespace-nowrap border ${
                    isLoggedIn
                      ? 'bg-red-600 hover:bg-red-700 text-white border-transparent shadow-lg'
                      : 'bg-[#0a2540] hover:bg-[#13345d] text-white border-transparent shadow-sm'
                  } ${isRTL ? 'flex-row-reverse' : ''}`}
                  dir={isRTL ? 'rtl' : 'ltr'}
                  style={{ 
                    textAlign: isRTL ? 'right' : 'left',
                    direction: isRTL ? 'rtl' : 'ltr'
                  }}
                >
                  <FiUser className={`w-5 h-5 flex-shrink-0 text-white`} style={{ order: isRTL ? 2 : 0 }} />
                  <span className="text-sm font-medium flex-1" style={{ textAlign: isRTL ? 'right' : 'left', direction: isRTL ? 'rtl' : 'ltr' }}>
                    {isLoggedIn ? t("logout", "تسجيل خروج") : t("login", "تسجيل دخول")}
                  </span>
                </button>
              </li>

              {(currentRole === "User" || !isLoggedIn) && (
                <li>
                  <div className="px-4 py-2" dir={isRTL ? 'rtl' : 'ltr'}>
                    <p className={`text-gray-600 text-xs mb-2`} style={{ textAlign: isRTL ? 'right' : 'left', direction: isRTL ? 'rtl' : 'ltr' }}>
                      {t("categories", "الأقسام")}
                    </p>
                    <div className="space-y-1">
                      {categoriesLoading ? (
                        <p className="text-gray-700 text-sm" style={{ textAlign: isRTL ? 'right' : 'left', direction: isRTL ? 'rtl' : 'ltr' }}>
                          {t("loadingCategories", "جارٍ التحميل...")}
                        </p>
                      ) : categories.length === 0 ? (
                        <p className="text-gray-700 text-sm" style={{ textAlign: isRTL ? 'right' : 'left', direction: isRTL ? 'rtl' : 'ltr' }}>
                          {t("noCategories", "لا توجد أقسام")}
                        </p>
                      ) : (
                        categories.map((category) => {
                          const searchValue = (category.categoryNameEn || category.nameEn || category.slug || category.name || category.categoryNameAr || "").trim();
                          const displayLabel = (category.name || category.categoryNameAr || searchValue).trim();
                          return (
                          <button
                            key={category.categoryId}
                            onClick={() => handleCategoryClick(searchValue, displayLabel)}
                            className={`w-full px-3 py-2 rounded-lg text-gray-900 text-sm font-medium bg-white/70 hover:bg-white transition-all`}
                            style={{ textAlign: isRTL ? 'right' : 'left', direction: isRTL ? 'rtl' : 'ltr' }}
                          >
                            {category.name}
                          </button>
                        );
                        })
                      )}
                    </div>
                  </div>
                </li>
              )}

              <li>
                <div className="px-4 py-2" dir={isRTL ? 'rtl' : 'ltr'}>
                  <p className={`text-gray-600 text-xs mb-2`} style={{ textAlign: isRTL ? 'right' : 'left', direction: isRTL ? 'rtl' : 'ltr' }}>
                    {t("more", "المزيد")}
                  </p>
                  <div className="space-y-1">
                    <button
                      onClick={() => handleNavigation('/about-us')}
                      className={`w-full flex items-center gap-3 px-3 py-2 rounded-lg text-gray-900 text-sm font-medium bg-white/70 hover:bg-white transition-all ${
                        isRTL ? 'flex-row-reverse' : ''
                      }`}
                      style={{ 
                        textAlign: isRTL ? 'right' : 'left',
                        direction: isRTL ? 'rtl' : 'ltr'
                      }}
                    >
                      <FiInfo className="w-4 h-4 flex-shrink-0 text-[#0a2540]" style={{ order: isRTL ? 2 : 0 }} />
                      <span className="flex-1" style={{ textAlign: isRTL ? 'right' : 'left', direction: isRTL ? 'rtl' : 'ltr' }}>{t("about.title", "من نحن")}</span>
                    </button>
                    <button
                      onClick={() => handleNavigation('/contact')}
                      className={`w-full flex items-center gap-3 px-3 py-2 rounded-lg text-gray-900 text-sm font-medium bg-white/70 hover:bg-white transition-all ${
                        isRTL ? 'flex-row-reverse' : ''
                      }`}
                      style={{ 
                        textAlign: isRTL ? 'right' : 'left',
                        direction: isRTL ? 'rtl' : 'ltr'
                      }}
                    >
                      <FiPhone className="w-4 h-4 flex-shrink-0 text-[#0a2540]" style={{ order: isRTL ? 2 : 0 }} />
                      <span className="flex-1" style={{ textAlign: isRTL ? 'right' : 'left', direction: isRTL ? 'rtl' : 'ltr' }}>{t("contact.title", "خدمة الدعم")}</span>
                    </button>
                    <button
                      onClick={() => handleNavigation('/terms')}
                      className={`w-full flex items-center gap-3 px-3 py-2 rounded-lg text-gray-900 text-sm font-medium bg-white/70 hover:bg-white transition-all ${
                        isRTL ? 'flex-row-reverse' : ''
                      }`}
                      style={{ 
                        textAlign: isRTL ? 'right' : 'left',
                        direction: isRTL ? 'rtl' : 'ltr'
                      }}
                    >
                      <FiFileText className="w-4 h-4 flex-shrink-0 text-[#0a2540]" style={{ order: isRTL ? 2 : 0 }} />
                      <span className="flex-1" style={{ textAlign: isRTL ? 'right' : 'left', direction: isRTL ? 'rtl' : 'ltr' }}>{t("sidebar.legal", "الشروط والخصوصية")}</span>
                    </button>
                  </div>
                </div>
              </li>
            </ul>
          </nav>

          {/* Footer */}
          <div className="p-4 border-t space-y-3" style={{ borderColor: 'rgba(15, 23, 42, 0.08)' }}>
            <div className="flex justify-center">
              <CurrencySelector />
            </div>
          </div>
        </div>
      </aside>
    </>
  );
}

export default Sidebar;
