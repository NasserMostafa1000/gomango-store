import React, { useState, useEffect, useRef, useCallback, useMemo } from "react";
import { Helmet } from "react-helmet";
import { useNavigate } from "react-router-dom";
import StoreLayout from "./StoreLayout";
import ProductItem from "../Products/ProductItem.jsx";
import API_BASE_URL, { SiteName } from "../Constant.js";
import CategoryItems from "./CategoryItems.jsx";
import { getRoleFromToken } from "../utils.js";
import ContactUs from "./ContactUs.jsx";
import WebSiteLogo from "../WebsiteLogo/WebsiteLogo.jsx";
import BannerCarousel from "./BannerCarousel";
import AnnouncementBar from "./AnnouncementBar";
import { useI18n } from "../i18n/I18nContext";

const SectionHeader = ({ title, eyebrow, description }) => (
  <div className="px-4 sm:px-6 md:px-8 lg:px-16 text-center space-y-2 md:space-y-3">
    {eyebrow && (
      <span className="inline-block rounded-full bg-orange-100 text-brand-orange px-3 py-1 text-xs sm:text-sm font-semibold">
        {eyebrow}
      </span>
    )}
    <h2 className="text-xl sm:text-2xl md:text-3xl lg:text-4xl font-bold text-gray-800">
      {title}
    </h2>
    {description && (
      <p className="text-xs sm:text-sm md:text-base text-gray-600 leading-relaxed max-w-3xl mx-auto px-2">
        {description}
      </p>
    )}
  </div>
);

export default function Home() {
  const [featuredProducts, setFeaturedProducts] = useState([]);
  const [Discountproducts, setDiscountProducts] = useState([]);
  const [clothesProducts, setClothesProducts] = useState([]);
  const [DiscountProductsPage, setDiscountProductsPage] = useState(1);
  const [ProductsPage, setProductsPage] = useState(1);
  const [ClothesPage, setClothesPage] = useState(1);
  const [hasMoreDisCountProducts, setHasMoreDiscountProducts] = useState(true);
  const [hasMoreFeatured, setHasMoreFeatured] = useState(true);
  const [hasMoreClothes, setHasMoreClothes] = useState(true);
  const [loadingClothes, setLoadingClothes] = useState(false);
  const [loadingFeatured, setLoadingFeatured] = useState(false);
  const [loadingDiscountProducts, setloadingDiscountProducts] = useState(false);
  const [loading, setLoading] = useState(false);
  const [gridProducts, setGridProducts] = useState([]);
  const [gridPage, setGridPage] = useState(1);
  const [hasMoreGrid, setHasMoreGrid] = useState(true);
  const [loadingGrid, setLoadingGrid] = useState(false);
  const navigate = useNavigate();
  const { t, lang } = useI18n();

  const fetchFeaturedProducts = useCallback(async (page, reset = false) => {
    if (loadingFeatured) return;
    setLoadingFeatured(true);
    try {
      const url = `${API_BASE_URL}Product/GetFeaturedProducts?page=${page}&limit=10&lang=${lang}`;
      if (process.env.NODE_ENV === 'development') {
        console.log('Fetching featured products with lang:', lang, 'page:', page, 'URL:', url);
      }
      const response = await fetch(url);
      if (!response.ok) throw new Error("Network error");

      const data = await response.json();
      if (process.env.NODE_ENV === 'development') {
        console.log('Products received:', data.length, 'First product:', data[0]?.productName);
      }
      if (data.length === 0) {
        setHasMoreFeatured(false);
      } else {
        if (reset) {
          setFeaturedProducts(data);
          setProductsPage(2); // Next page will be 2
        } else {
          setFeaturedProducts((prev) => [...prev, ...data]);
          setProductsPage((prev) => prev + 1);
        }
      }
    } catch (error) {
      console.error("Error fetching other products:", error);
    } finally {
      setLoadingFeatured(false);
    }
  }, [lang, loadingFeatured]);

  const fetchDiscountProducts = useCallback(async (page, reset = false) => {
    if (loadingDiscountProducts) return;
    setloadingDiscountProducts(true);
    try {
      const url = `${API_BASE_URL}Product/GetDiscountProducts?page=${page}&limit=10&lang=${lang}`;
      if (process.env.NODE_ENV === 'development') {
        console.log('Fetching discount products with lang:', lang, 'page:', page);
      }
      const response = await fetch(url);
      if (!response.ok) throw new Error("Network error");

      const data = await response.json();
      if (process.env.NODE_ENV === 'development') {
        console.log('Discount products received:', data.length);
      }
      if (data.length === 0) {
        setHasMoreDiscountProducts(false);
      } else {
        if (reset) {
          setDiscountProducts(data);
          setDiscountProductsPage(2); // Next page will be 2
        } else {
          setDiscountProducts((prev) => [...prev, ...data]);
          setDiscountProductsPage((prev) => prev + 1);
        }
      }
    } catch (error) {
      console.error("Error fetching discount products:", error);
    } finally {
      setloadingDiscountProducts(false);
    }
  }, [lang, loadingDiscountProducts]);

  // Cache for accessories data to avoid re-fetching on pagination
  const accessoriesDataCache = useRef(null);
  const accessoriesLangCache = useRef(null);

  const fetchClothesProducts = useCallback(async (page, reset = false) => {
    if (loadingClothes) return;
    setLoadingClothes(true);

    try {
      // استخدام البحث بكلمة "إاكسسوارات" للحصول على منتجات الإكسسوارات
      const searchQuery = lang === 'ar' ? 'إاكسسوارات' : 'accessories';
      
      // إذا تغيرت اللغة أو reset، نجلب البيانات من جديد
      if (reset || accessoriesLangCache.current !== lang || !accessoriesDataCache.current) {
        const url = `${API_BASE_URL}Product/GetProductsByName?Name=${encodeURIComponent(searchQuery)}&lang=${lang}`;
        if (process.env.NODE_ENV === 'development') {
          console.log('Fetching accessories products with lang:', lang, 'page:', page, 'reset:', reset);
        }
        const response = await fetch(url);
        if (!response.ok) throw new Error("Network error");

        const data = await response.json();
        if (process.env.NODE_ENV === 'development') {
          console.log('Accessories products received:', data.length);
        }
        
        // حفظ البيانات في cache
        accessoriesDataCache.current = data;
        accessoriesLangCache.current = lang;
        
        if (data.length === 0) {
          setHasMoreClothes(false);
          setLoadingClothes(false);
          return;
        }
      }

      // استخدام البيانات من cache
      const data = accessoriesDataCache.current || [];
      
      // تطبيق pagination يدوياً
      const startIndex = (page - 1) * 10;
      const endIndex = startIndex + 10;
      const paginatedData = data.slice(startIndex, endIndex);
      
      if (paginatedData.length === 0) {
        setHasMoreClothes(false);
      } else {
        if (reset) {
          setClothesProducts(paginatedData);
          setClothesPage(2); // Next page will be 2
        } else {
          setClothesProducts((prev) => [...prev, ...paginatedData]);
          setClothesPage((prev) => prev + 1);
        }
      }
      // إذا كانت البيانات أقل من endIndex، لا يوجد المزيد
      if (data.length <= endIndex) {
        setHasMoreClothes(false);
      }
    } catch (error) {
      console.error("Error fetching accessories products:", error);
    } finally {
      setLoadingClothes(false);
    }
  }, [lang, loadingClothes]);

  const fetchGridProducts = useCallback(async (page, reset = false) => {
    if (loadingGrid) return;
    setLoadingGrid(true);
    try {
      const url = `${API_BASE_URL}Product/GetAllProductsWithLimit?page=${page}&limit=9&lang=${lang}`;
      const response = await fetch(url);
      if (!response.ok) throw new Error("Network error");
      const data = await response.json();
      if (data.length === 0) {
        setHasMoreGrid(false);
      } else if (reset) {
        setGridProducts(data);
        setGridPage(2);
      } else {
        setGridProducts((prev) => [...prev, ...data]);
        setGridPage((prev) => prev + 1);
      }
      if (data.length === 0 && !reset) {
        setHasMoreGrid(false);
      }
    } catch (error) {
      console.error("Error fetching grid products:", error);
    } finally {
      setLoadingGrid(false);
    }
  }, [lang, loadingGrid]);

  const handleScroll = useCallback((ref, fetchMore) => {
    if (!ref.current) return;
    const { scrollLeft, scrollWidth, clientWidth } = ref.current;
    const isRtl = getComputedStyle(ref.current).direction === "rtl";

    // زيادة الحساسية للأجهزة المحمولة
    const threshold = window.innerWidth < 768 ? 200 : 300;
    
    const isAtEnd = isRtl
      ? scrollLeft <= threshold
      : scrollLeft + clientWidth >= scrollWidth - threshold;

    if (isAtEnd) fetchMore();
  }, []);

  // Initial load (only once on mount)
  useEffect(() => {
    if (lang) {
      fetchDiscountProducts(1, false);
      fetchClothesProducts(1, false);
      fetchFeaturedProducts(1, false);
      fetchGridProducts(1, false);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  // Track previous lang to avoid unnecessary refetches
  const prevLangRef = useRef(null);
  
  // Reset and refetch products when language changes
  useEffect(() => {
    if (!lang) return; // Skip if lang is not ready
    if (lang === prevLangRef.current) return; // Skip if lang hasn't changed
    
    // Update ref
    prevLangRef.current = lang;
    
    // Clear cache when language changes
    accessoriesDataCache.current = null;
    accessoriesLangCache.current = null;
    
    // Reset all states
    setFeaturedProducts([]);
    setDiscountProducts([]);
    setClothesProducts([]);
    setGridProducts([]);
    setProductsPage(1);
    setDiscountProductsPage(1);
    setClothesPage(1);
    setGridPage(1);
    setHasMoreFeatured(true);
    setHasMoreDiscountProducts(true);
    setHasMoreClothes(true);
    setHasMoreGrid(true);
    
    // Refetch products with new language (reset = true to replace, not append)
    // Use setTimeout to ensure state updates are complete
    const timer = setTimeout(() => {
      fetchDiscountProducts(1, true);
      fetchClothesProducts(1, true);
      fetchFeaturedProducts(1, true);
      fetchGridProducts(1, true);
    }, 100);
    
    return () => clearTimeout(timer);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [lang]);

  const DiscountProductsRef = useRef(null);
  const ProductsRef = useRef(null);
  const clothesRef = useRef(null);
  
  // Use refs to track current page numbers for scroll handlers
  const discountPageRef = useRef(DiscountProductsPage);
  const clothesPageRef = useRef(ClothesPage);
  const productsPageRef = useRef(ProductsPage);
  
  // Update refs when pages change
  useEffect(() => {
    discountPageRef.current = DiscountProductsPage;
  }, [DiscountProductsPage]);
  
  useEffect(() => {
    clothesPageRef.current = ClothesPage;
  }, [ClothesPage]);
  
  useEffect(() => {
    productsPageRef.current = ProductsPage;
  }, [ProductsPage]);

  useEffect(() => {
    const DiscountProductsDiv = DiscountProductsRef.current;
    const productsDiv = ProductsRef.current;
    const clothesDiv = clothesRef.current;

    const DiscountProductsScrollHandler = () => {
      handleScroll(DiscountProductsRef, () => {
        const currentPage = discountPageRef.current;
        fetchDiscountProducts(currentPage, false);
      });
    };
    const clothesScrollHandler = () => {
      handleScroll(clothesRef, () => {
        const currentPage = clothesPageRef.current;
        fetchClothesProducts(currentPage, false);
      });
    };
    const productsScrollHandler = () => {
      handleScroll(ProductsRef, () => {
        const currentPage = productsPageRef.current;
        fetchFeaturedProducts(currentPage, false);
      });
    };

    if (DiscountProductsDiv)
      DiscountProductsDiv.addEventListener(
        "scroll",
        DiscountProductsScrollHandler
      );
    if (clothesDiv) clothesDiv.addEventListener("scroll", clothesScrollHandler);
    if (productsDiv)
      productsDiv.addEventListener("scroll", productsScrollHandler);

    return () => {
      if (DiscountProductsDiv)
        DiscountProductsDiv.removeEventListener(
          "scroll",
          DiscountProductsScrollHandler
        );
      if (clothesDiv)
        clothesDiv.removeEventListener("scroll", clothesScrollHandler);
      if (productsDiv)
        productsDiv.removeEventListener("scroll", productsScrollHandler);
    };
  }, [
    handleScroll,
    fetchDiscountProducts,
    fetchClothesProducts,
    fetchFeaturedProducts,
  ]);

  function handleProductClick(product) {
    navigate(`/productDetails/${product.productId}`, {
      state: { product },
      replace: true,
    });
  }

  if (loading) {
    return (
      <div className="min-h-screen bg-[#F9F6EF] flex flex-col items-center justify-center gap-6">
        <h2 className="text-2xl sm:text-3xl md:text-4xl font-bold text-indigo-700 text-center px-4">
          {t("welcomeMessage", "جومانجو يرحب بكم")}
        </h2>
        <div className="w-32 sm:w-40 md:w-48">
          <WebSiteLogo width={200} height={100} />
        </div>
        <div className="h-10 w-10 sm:h-12 sm:w-12 border-4 border-orange-500 border-t-transparent rounded-full animate-spin"></div>
      </div>
    );
  }

  const handleRemoveFromDiscount = (id) =>
    setDiscountProducts((prev) => prev.filter((item) => item.productId !== id));
  const handleRemoveFromClothes = (id) =>
    setClothesProducts((prev) => prev.filter((item) => item.productId !== id));
  const handleRemoveFromFeatured = (id) =>
    setFeaturedProducts((prev) => prev.filter((item) => item.productId !== id));
  const handleRemoveFromGrid = (id) =>
    setGridProducts((prev) => prev.filter((item) => item.productId !== id));

  const renderProductsRail = (items, ref, emptyMessage, isLoadingMore, onRemove) => (
    <div className="px-4 sm:px-6 md:px-8 lg:px-16">
      {items.length === 0 && isLoadingMore ? (
        <p className="text-center text-sm text-gray-600 py-6 sm:py-8">{t("loadingProducts", "جارٍ التحميل...")}</p>
      ) : items.length === 0 ? (
        <p className="text-center text-sm text-gray-600 py-6 sm:py-8">{emptyMessage}</p>
      ) : (
        <div
          className="flex gap-3 sm:gap-4 overflow-x-auto pb-3 md:pb-4 snap-x snap-mandatory scrollbar-hide"
          ref={ref}
          style={{ 
            scrollbarWidth: 'none', 
            msOverflowStyle: 'none',
            WebkitOverflowScrolling: 'touch'
          }}
        >
          {items.map((product) => (
            <div
              onClick={() => handleProductClick(product)}
              key={product.productId}
              className="snap-start flex-shrink-0 w-48 sm:w-56 md:w-64 lg:w-72" // أحجام متجاوبة
            >
              <ProductItem
                product={product}
                CurrentRole={getRoleFromToken(sessionStorage.getItem("token"))}
                onDeleted={(deletedId) => {
                  if (typeof onRemove === "function") {
                    onRemove(deletedId ?? product.productId);
                  }
                }}
              />
            </div>
          ))}
          {isLoadingMore && (
            <div className="flex-shrink-0 w-48 sm:w-56 md:w-64 lg:w-72 flex items-center justify-center">
              <div className="h-6 w-6 sm:h-8 sm:w-8 border-2 border-brand-orange border-t-transparent rounded-full animate-spin"></div>
            </div>
          )}
        </div>
      )}
    </div>
  );

  return (
    <>
      <Helmet>
        <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=5.0, user-scalable=yes" />
        <title>{t("homePage.metaTitle", `تسوق الآن من ${SiteName} | خصومات على أفضل المنتجات`)}</title>
        <meta
          name="description"
          content={t("homePage.metaDesc", `تصفح مجموعة ضخمة من المنتجات الأصلية في {جومانجو}. احصل على أفضل العروض والخصومات حتى 50%. شحن سريع ودعم ممتاز.`)}
        />
        <meta
          name="keywords"
          content={t("homePage.metaKeywords", "تسوق, خصومات, عروض, ماركات, منتجات أصلية, متجر إلكتروني, جومانجو")}
        />
        <link rel="canonical" href={window.location.href} />

        <meta property="og:type" content="website" />
        <meta
          property="og:title"
          content={t("homePage.ogTitle", `تسوق الآن من ${SiteName} | عروض وخصومات مذهلة`)}
        />
        <meta
          property="og:description"
          content={t("homePage.ogDescription", "أفضل المنتجات الأصلية مع خصومات تصل إلى 50%. اكتشف العروض الآن!")}
        />
        <meta property="og:url" content={window.location.href} />
        <meta property="og:site_name" content={SiteName} />

        <meta name="twitter:card" content="summary_large_image" />
        <meta
          name="twitter:title"
          content={t("homePage.twitterTitle", `تسوق من ${SiteName} | أفضل الأسعار والعروض`)}
        />
        <meta
          name="twitter:description"
          content={t("homePage.twitterDescription", "منتجات أصلية وماركات عالمية مع خصومات تصل إلى 50%. سارع بالشراء!")}
        />
      </Helmet>

      <StoreLayout>
        <div className="w-full">
          {/* Announcement Bar */}
          <div className="relative z-10">
            <AnnouncementBar />
          </div>

          {/* Categories Section - moved above hero */}
          <section className="bg-[#F9F6EF] border-b border-blue-100 relative z-20">
            <div className="max-w-screen-xl mx-auto px-4 sm:px-6 md:px-8 lg:px-16 py-4 sm:py-6">
              <SectionHeader
                eyebrow={t("homePage.curatedCategories", "أقسام منتقاة")}
                title={t("homePage.shopPopular", "تسوّق فئاتنا الشائعة")}
                description={t("homePage.shopPopularDesc", "اكتشف مجموعتنا المختارة من الأقسام الشائعة")}
              />
              <div className="mt-4 sm:mt-6">
                <CategoryItems />
              </div>
            </div>
          </section>
          
          {/* Hero Section */}
          <section className="relative bg-gradient-to-r from-orange-50 via-orange-100 to-orange-50 min-h-[70vh] sm:min-h-[80vh] flex items-center">
            <div className="relative z-10 max-w-screen-xl mx-auto px-4 sm:px-6 md:px-8 lg:px-16 py-8 sm:py-12 md:py-16 text-[#0a2540] grid grid-cols-1 md:grid-cols-2 gap-6 md:gap-8 items-center w-full">
              <div className="space-y-4 md:space-y-6 text-center md:text-right order-2 md:order-1">
                <p className="text-xs sm:text-sm md:text-base uppercase tracking-[0.35em] text-[#13345d]/70">
                 {t("homePage.brand", "جومانجو")}
                </p>
                <h1 className="text-2xl sm:text-3xl md:text-4xl lg:text-5xl font-bold leading-tight text-[#0a2540]">
                  {t("homePage.heroTitle", "اكتشف منتجات  أصلية بخدمة توصيل تغطي كل بيت")}
                </h1>
                <p className="text-xs sm:text-sm md:text-base text-[#13345d]/80 leading-relaxed">
                  {t("homePage.heroDesc", "متجر إلكتروني متكامل يدعم تعدد العملات، بانرز مخصصة للعروض، وتجربة مريحة لعملائك على كل الشاشات.")}
                </p>
                <div className="flex flex-wrap justify-center md:justify-end gap-3">
                  <button
                    onClick={() => navigate("/Contact")}
                    className="bg-[#0a2540] text-white px-4 py-2 sm:px-6 sm:py-3 rounded-lg sm:rounded-xl font-semibold hover:bg-[#13345d] transition shadow-lg border-2 border-[#0a2540] text-sm sm:text-base"
                  >
                    {t("homePage.heroCtaContact", "تواصل معنا")}
                  </button>
                </div>
              </div>
              <div className="bg-[#F9F6EF]/80 rounded-xl sm:rounded-2xl backdrop-blur shadow-lg p-4 sm:p-6 md:p-8 flex flex-col gap-3 sm:gap-4 border border-[#0a2540]/10 order-1 md:order-2">
                <h3 className="text-base sm:text-lg md:text-xl font-semibold text-[#0a2540] text-center md:text-right">
                  {t("homePage.latestOffers", "أحدث عروضنا")}
                </h3>
                <BannerCarousel />
                <dl className="grid grid-cols-1 sm:grid-cols-2 gap-3 sm:gap-4 text-xs sm:text-sm">
                  <div>
                    <dt className="text-[#13345d]/70">{t("homePage.gulfCurrencies", "دعم كامل للعملات الخليجية")}</dt>
                    <dd className="text-[#0a2540] font-semibold">AED - SAR - QAR - OMR - USD</dd>
                  </div>
                  <div>
                    <dt className="text-[#13345d]/70">{t("homePage.customBanners", "بانرز مخصصة")}</dt>
                    <dd className="text-[#0a2540] font-semibold">{t("homePage.customBannersDesc", "إدارة مباشرة عبر لوحة التحكم")}</dd>
                  </div>
                </dl>
              </div>
            </div>
            <div className="absolute inset-x-0 bottom-0 h-12 sm:h-16 bg-gradient-to-t from-[#F9F6EF] to-transparent"></div>
          </section>

          {/* Featured Products Section */}
          <section className="py-6 sm:py-8 md:py-12 space-y-6 sm:space-y-8 bg-[#F9F6EF]">
            <SectionHeader
              eyebrow={t("homePage.gomangoPicks", "مختارات جومانجو")}
              title={t("homePage.featuredProducts", "المنتجات المميزة")}
              description={t("homePage.productsDesc", "منتجات أصلية معروضة بأسعار تنافسية، يتم تحديثها باستمرار لتلبية طلبات العملاء.")}
            />
            {renderProductsRail(
              featuredProducts,
              ProductsRef,
              t("noFeaturedProducts", "لا توجد منتجات مميزة حالياً."),
              loadingFeatured,
              handleRemoveFromFeatured
            )}
          </section>

          {/* Discount Products Section */}
          <section className="py-6 sm:py-8 md:py-12 space-y-6 sm:space-y-8 bg-[#F9F6EF]">
            <SectionHeader
              eyebrow={t("homePage.newThisWeek", "جديد هذا الأسبوع")}
              title={t("homePage.discountsUpTo", "خصومات تصل إلى 60%")}
              description={t("homePage.discountsDesc", "عروض مختارة بعناية لتلبية احتياجات الأسرة  مع جودة عالية وأسعار منافسة.")}
            />
            {renderProductsRail(
              Discountproducts,
              DiscountProductsRef,
              t("noDiscountProducts", "لا توجد منتجات عليها خصومات حالياً."),
              loadingDiscountProducts,
              handleRemoveFromDiscount
            )}
          </section>

          {/* Accessories Section */}
          <section className="py-6 sm:py-8 md:py-12 space-y-6 sm:space-y-8 bg-[#F9F6EF]">
            <SectionHeader
              eyebrow={t("homePage.latestAccessories", "الإكسسوارات")}
              title={t("homePage.latestAccessories", "الإكسسوارات")}
              description={t("homePage.clothesDesc", "صيحات الموسم مع خامات مريحة وتصاميم تناسب جميع الأعمار. التوصيل لجميع المحافظات.")}
            />
            {renderProductsRail(
              clothesProducts,
              clothesRef,
              t("noClothesProducts", "لا توجد منتجات للملابس حالياً."),
              loadingClothes,
              handleRemoveFromClothes
            )}
          </section>

          {/* Grid Products Section */}
          <section className="py-6 sm:py-8 md:py-12 bg-[#F9F6EF]">
            <SectionHeader
              eyebrow={t("homePage.allProductsEyebrow", "تصفح الكل")}
              title={t("homePage.allProductsTitle", "كل المنتجات في مكان واحد")}
              description={t("homePage.allProductsDesc", "شبكة منتجات ديناميكية تُظهر آخر الإضافات ويمكن تصفحها وتحميل المزيد بسهولة.")}
            />
            <div className="px-4 sm:px-6 md:px-8 lg:px-16">
              {gridProducts.length === 0 && loadingGrid ? (
                <p className="text-center text-sm text-gray-600 py-6 sm:py-8">
                  {t("loadingProducts", "جارٍ التحميل...")}
                </p>
              ) : gridProducts.length === 0 ? (
                <p className="text-center text-sm text-gray-600 py-6 sm:py-8">
                  {t("homePage.noProducts", "لا توجد منتجات متاحة حالياً.")}
                </p>
              ) : (
                <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-4 md:gap-6">
                  {gridProducts.map((product) => (
                    <ProductItem
                      key={product.productId}
                      product={product}
                      CurrentRole={getRoleFromToken(sessionStorage.getItem("token"))}
                      onDeleted={(deletedId) => {
                        handleRemoveFromGrid(deletedId ?? product.productId);
                      }}
                      layout="grid"
                      onClick={() => handleProductClick(product)}
                    />
                  ))}
                </div>
              )}
              {hasMoreGrid && (
                <div className="flex justify-center mt-6">
                  <button
                    onClick={() => fetchGridProducts(gridPage, false)}
                    disabled={loadingGrid}
                    className="px-6 py-2 rounded-lg bg-[#0a2540] text-white font-semibold hover:bg-[#13345d] transition disabled:opacity-50"
                  >
                    {loadingGrid
                      ? t("homePage.loadingMore", "جارٍ التحميل...")
                      : t("homePage.loadMore", "عرض المزيد")}
                  </button>
                </div>
              )}
            </div>
          </section>

          {/* Features Section */}


          {/* Contact Section */}

        </div>
      </StoreLayout>
    </>
  );
}