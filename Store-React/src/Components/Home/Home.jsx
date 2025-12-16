import React, { useState, useEffect, useRef, useCallback, useMemo, forwardRef } from "react";
import { Helmet } from "react-helmet";
import { useNavigate } from "react-router-dom";
import StoreLayout from "./StoreLayout";
import ProductItem from "../Products/ProductItem.jsx";
import API_BASE_URL, { SiteName } from "../Constant.js";
import { getRoleFromToken } from "../utils.js";
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

const LazySectionPlaceholder = ({ message, helper }) => (
  <div className="flex flex-col items-center justify-center gap-3 py-16 text-center text-gray-500">
    <div className="h-10 w-10 border-4 border-gray-300 border-t-transparent rounded-full animate-spin" />
    <p className="text-sm font-semibold">{message}</p>
    {helper && <p className="text-xs text-gray-400 max-w-sm">{helper}</p>}
  </div>
);

const ProductsRail = forwardRef(({ items, emptyMessage, isLoadingMore, onRemove, handleProductClick, t, getRoleFromToken }, ref) => {
  const [canScrollLeft, setCanScrollLeft] = useState(false);
  const [canScrollRight, setCanScrollRight] = useState(true);

  const checkScrollability = useCallback(() => {
    if (!ref.current) return;
    const { scrollLeft, scrollWidth, clientWidth } = ref.current;
    const isRtl = getComputedStyle(ref.current).direction === "rtl";
    
    if (isRtl) {
      setCanScrollLeft(scrollLeft > 0);
      setCanScrollRight(scrollLeft < scrollWidth - clientWidth - 10);
    } else {
      setCanScrollLeft(scrollLeft > 0);
      setCanScrollRight(scrollLeft < scrollWidth - clientWidth - 10);
    }
  }, [ref]);

  useEffect(() => {
    const scrollContainer = ref.current;
    if (!scrollContainer) return;
    
    checkScrollability();
    scrollContainer.addEventListener('scroll', checkScrollability);
    window.addEventListener('resize', checkScrollability);
    
    return () => {
      scrollContainer.removeEventListener('scroll', checkScrollability);
      window.removeEventListener('resize', checkScrollability);
    };
  }, [ref, checkScrollability, items.length]);

  const scrollLeft = () => {
    if (!ref.current) return;
    const isRtl = getComputedStyle(ref.current).direction === "rtl";
    const scrollAmount = 400;
    if (isRtl) {
      ref.current.scrollBy({ left: scrollAmount, behavior: 'smooth' });
    } else {
      ref.current.scrollBy({ left: -scrollAmount, behavior: 'smooth' });
    }
  };

  const scrollRight = () => {
    if (!ref.current) return;
    const isRtl = getComputedStyle(ref.current).direction === "rtl";
    const scrollAmount = 400;
    if (isRtl) {
      ref.current.scrollBy({ left: -scrollAmount, behavior: 'smooth' });
    } else {
      ref.current.scrollBy({ left: scrollAmount, behavior: 'smooth' });
    }
  };

  return (
    <div className="px-4 sm:px-6 md:px-8 lg:px-16 relative">
      {items.length === 0 && isLoadingMore ? (
        <p className="text-center text-sm text-gray-600 py-6 sm:py-8">{t("loadingProducts", "جارٍ التحميل...")}</p>
      ) : items.length === 0 ? (
        <p className="text-center text-sm text-gray-600 py-6 sm:py-8">{emptyMessage}</p>
      ) : (
        <div className="relative">
          {/* Left Arrow - Desktop Only */}
          <button
            onClick={scrollLeft}
            disabled={!canScrollLeft}
            className={`hidden lg:flex absolute left-0 top-1/2 -translate-y-1/2 z-20 bg-white rounded-full p-3 shadow-lg hover:shadow-xl transition-all duration-200 hover:scale-110 items-center justify-center border border-gray-200 ${
              !canScrollLeft ? 'opacity-50 cursor-not-allowed' : ''
            }`}
            aria-label="Scroll left"
          >
            <svg className="w-6 h-6 text-gray-700" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
            </svg>
          </button>
          
          {/* Right Arrow - Desktop Only */}
          <button
            onClick={scrollRight}
            disabled={!canScrollRight}
            className={`hidden lg:flex absolute right-0 top-1/2 -translate-y-1/2 z-20 bg-white rounded-full p-3 shadow-lg hover:shadow-xl transition-all duration-200 hover:scale-110 items-center justify-center border border-gray-200 ${
              !canScrollRight ? 'opacity-50 cursor-not-allowed' : ''
            }`}
            aria-label="Scroll right"
          >
            <svg className="w-6 h-6 text-gray-700" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
            </svg>
          </button>

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
                className="snap-start flex-shrink-0 w-48 sm:w-56 md:w-64 lg:w-72"
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
        </div>
      )}
    </div>
  );
});

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
  const [gridProducts, setGridProducts] = useState([]);
  const [gridPage, setGridPage] = useState(1);
  const [hasMoreGrid, setHasMoreGrid] = useState(true);
  const [loadingGrid, setLoadingGrid] = useState(false);
  const [loading, setLoading] = useState(false);
  const [sectionsReady, setSectionsReady] = useState({
    discounts: false,
    accessories: false,
    grid: false,
  });
  const navigate = useNavigate();
  const { t, lang } = useI18n();
  const featuredLoadingRef = useRef(false);
  const discountLoadingRef = useRef(false);
  const clothesLoadingRef = useRef(false);
  const gridLoadingRef = useRef(false);
  const gridObserverTarget = useRef(null);
  const fetchedPagesRef = useRef({
    featured: new Set(),
    discount: new Set(),
    clothes: new Set(),
    grid: new Set(),
  });
  
  // استخدام useRef لمنع استدعاء useEffect مرتين
  const hasInitializedFeatured = useRef(false);
  const hasInitializedDiscount = useRef(false);
  const hasInitializedClothes = useRef(false);
  const hasInitializedGrid = useRef(false);
  const lastLangRef = useRef(null);

  const scrollToSection = useCallback((sectionId) => {
    const target = document.getElementById(sectionId);
    if (target) {
      target.scrollIntoView({ behavior: "smooth", block: "start" });
    }
  }, []);

  const footerLinks = useMemo(
    () => [
      { key: "home", label: t("footer.home", "المتجر"), path: "/" },
      { key: "contact", label: t("footer.contact", "اتصل بنا"), path: "/Contact" },
      { key: "about", label: t("footer.about", "معلومات عنا"), path: "/about-us" },
      { key: "terms", label: t("footer.terms", "الشروط والخصوصية"), path: "/terms" },
      { key: "categories", label: t("footer.categories", "الأقسام"), sectionId: "home-categories" },
    ],
    [t]
  );

  const getPageKey = (page) => `${lang || "default"}-${page}`;

  const mergeUniqueProducts = (prev, next) => {
    const existing = new Set(prev.map((item) => item.productId));
    return [...prev, ...next.filter((item) => !existing.has(item.productId))];
  };

  useEffect(() => {
    const observers = [];
    const createObserver = (ref, key) => {
      if (!ref.current) return;
      const observer = new IntersectionObserver(
        (entries) => {
          if (entries[0].isIntersecting) {
            setSectionsReady((prev) => (prev[key] ? prev : { ...prev, [key]: true }));
            observer.disconnect();
          }
        },
        { threshold: 0.2, rootMargin: "0px 0px -10% 0px" }
      );

      observer.observe(ref.current);
      observers.push(observer);
    };

    createObserver(discountSectionRef, "discounts");
    createObserver(accessoriesSectionRef, "accessories");
    createObserver(gridSectionRef, "grid");

    return () => {
      observers.forEach((observer) => observer.disconnect());
    };
  }, []);

  const resetFetchedPages = () => {
    fetchedPagesRef.current = {
      featured: new Set(),
      discount: new Set(),
      clothes: new Set(),
      grid: new Set(),
    };
  };

  const fetchFeaturedProducts = useCallback(async (page, reset = false) => {
    if (featuredLoadingRef.current) return;
    const pageKey = getPageKey(page);
    if (!reset && fetchedPagesRef.current.featured.has(pageKey)) return;
    featuredLoadingRef.current = true;
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
          fetchedPagesRef.current.featured.clear();
          setFeaturedProducts(data);
          setProductsPage(2);
        } else {
          setFeaturedProducts((prev) => mergeUniqueProducts(prev, data));
          setProductsPage((prev) => prev + 1);
        }
        fetchedPagesRef.current.featured.add(pageKey);
      }
    } catch (error) {
      console.error("Error fetching other products:", error);
    } finally {
      featuredLoadingRef.current = false;
      setLoadingFeatured(false);
    }
  }, [lang]);

  const fetchDiscountProducts = useCallback(async (page, reset = false) => {
    if (discountLoadingRef.current) return;
    const pageKey = getPageKey(page);
    if (!reset && fetchedPagesRef.current.discount.has(pageKey)) return;
    discountLoadingRef.current = true;
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
          fetchedPagesRef.current.discount.clear();
          setDiscountProducts(data);
          setDiscountProductsPage(2);
        } else {
          setDiscountProducts((prev) => mergeUniqueProducts(prev, data));
          setDiscountProductsPage((prev) => prev + 1);
        }
        fetchedPagesRef.current.discount.add(pageKey);
      }
    } catch (error) {
      console.error("Error fetching discount products:", error);
    } finally {
      discountLoadingRef.current = false;
      setloadingDiscountProducts(false);
    }
  }, [lang]);

  // Cache for accessories data to avoid re-fetching on pagination
  const accessoriesDataCache = useRef(null);
  const accessoriesLangCache = useRef(null);

  const fetchClothesProducts = useCallback(async (page, reset = false) => {
    if (clothesLoadingRef.current) return;
    const pageKey = getPageKey(page);
    if (!reset && fetchedPagesRef.current.clothes.has(pageKey)) return;
    clothesLoadingRef.current = true;
    setLoadingClothes(true);

    try {
      // استخدام البحث بكلمة "إاكسسوارات" للحصول على منتجات الإكسسوارات
      const searchQuery = lang === 'ar' ? 'حقائب نسائية واكسسوارات' : "Accessories & Women's Bags";
      
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
          fetchedPagesRef.current.clothes.clear();
          setClothesProducts(paginatedData);
          setClothesPage(2);
        } else {
          setClothesProducts((prev) => mergeUniqueProducts(prev, paginatedData));
          setClothesPage((prev) => prev + 1);
        }
        fetchedPagesRef.current.clothes.add(pageKey);
      }
      // إذا كانت البيانات أقل من endIndex، لا يوجد المزيد
      if (data.length <= endIndex) {
        setHasMoreClothes(false);
      } else {
        setHasMoreClothes(true);
      }
    } catch (error) {
      console.error("Error fetching accessories products:", error);
    } finally {
      clothesLoadingRef.current = false;
      setLoadingClothes(false);
    }
  }, [lang]);

  const fetchGridProducts = useCallback(async (page, reset = false) => {
    if (gridLoadingRef.current) return;
    const pageKey = getPageKey(page);
    if (!reset && fetchedPagesRef.current.grid.has(pageKey)) return;
    gridLoadingRef.current = true;
    setLoadingGrid(true);
    try {
      const url = `${API_BASE_URL}Product/GetAllProductsWithLimit?page=${page}&limit=9&lang=${lang}`;
      const response = await fetch(url);
      if (!response.ok) throw new Error("Network error");
      const data = await response.json();
      if (data.length === 0) {
        setHasMoreGrid(false);
      } else {
        if (reset) {
          fetchedPagesRef.current.grid.clear();
          setGridProducts(data);
          setGridPage(2);
        } else {
          setGridProducts((prev) => mergeUniqueProducts(prev, data));
          setGridPage((prev) => prev + 1);
        }
        fetchedPagesRef.current.grid.add(pageKey);
      }
      if (data.length === 0 && !reset) {
        setHasMoreGrid(false);
      }
    } catch (error) {
      console.error("Error fetching grid products:", error);
    } finally {
      gridLoadingRef.current = false;
      setLoadingGrid(false);
    }
  }, [lang]);

  const handleScroll = useCallback((ref, fetchMore, hasMore, isLoading) => {
    if (!ref.current || !hasMore || isLoading) return;
    const { scrollLeft, scrollWidth, clientWidth } = ref.current;
    const isRtl = getComputedStyle(ref.current).direction === "rtl";

    // زيادة الحساسية للأجهزة المحمولة
    const threshold = window.innerWidth < 768 ? 100 : 150;
    
    const isAtEnd = isRtl
      ? scrollLeft <= threshold
      : scrollLeft + clientWidth >= scrollWidth - threshold;

    if (isAtEnd) {
      fetchMore();
    }
  }, []);

  useEffect(() => {
    if (!lang) return;
    
    // منع الاستدعاء المكرر لنفس اللغة
    if (lastLangRef.current === lang && hasInitializedFeatured.current) return;
    lastLangRef.current = lang;
    hasInitializedFeatured.current = true;
    
    accessoriesDataCache.current = null;
    accessoriesLangCache.current = null;
    resetFetchedPages();
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

    fetchFeaturedProducts(1, true);
    
    return () => {
      // إعادة تعيين عند تغيير اللغة
      if (lastLangRef.current !== lang) {
        hasInitializedFeatured.current = false;
        hasInitializedDiscount.current = false;
        hasInitializedClothes.current = false;
        hasInitializedGrid.current = false;
      }
    };
  }, [lang, fetchFeaturedProducts]);

  useEffect(() => {
    if (!lang || !sectionsReady.discounts) return;
    
    // منع الاستدعاء المكرر
    const key = `${lang}-${sectionsReady.discounts}`;
    if (hasInitializedDiscount.current === key) return;
    hasInitializedDiscount.current = key;
    
    fetchDiscountProducts(1, true);
  }, [lang, sectionsReady.discounts, fetchDiscountProducts]);

  useEffect(() => {
    if (!lang || !sectionsReady.accessories) return;
    
    // منع الاستدعاء المكرر
    const key = `${lang}-${sectionsReady.accessories}`;
    if (hasInitializedClothes.current === key) return;
    hasInitializedClothes.current = key;
    
    fetchClothesProducts(1, true);
  }, [lang, sectionsReady.accessories, fetchClothesProducts]);

  useEffect(() => {
    if (!lang || !sectionsReady.grid) return;
    
    // منع الاستدعاء المكرر
    const key = `${lang}-${sectionsReady.grid}`;
    if (hasInitializedGrid.current === key) return;
    hasInitializedGrid.current = key;
    
    fetchGridProducts(1, true);
  }, [lang, sectionsReady.grid, fetchGridProducts]);

  const DiscountProductsRef = useRef(null);
  const ProductsRef = useRef(null);
  const clothesRef = useRef(null);
  const discountSectionRef = useRef(null);
  const accessoriesSectionRef = useRef(null);
  const gridSectionRef = useRef(null);
  
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
      handleScroll(
        DiscountProductsRef, 
        () => {
          const currentPage = discountPageRef.current;
          if (hasMoreDisCountProducts && !loadingDiscountProducts) {
            fetchDiscountProducts(currentPage, false);
          }
        },
        hasMoreDisCountProducts,
        loadingDiscountProducts
      );
    };
    const clothesScrollHandler = () => {
      handleScroll(
        clothesRef, 
        () => {
          const currentPage = clothesPageRef.current;
          if (hasMoreClothes && !loadingClothes) {
            fetchClothesProducts(currentPage, false);
          }
        },
        hasMoreClothes,
        loadingClothes
      );
    };
    const productsScrollHandler = () => {
      handleScroll(
        ProductsRef, 
        () => {
          const currentPage = productsPageRef.current;
          if (hasMoreFeatured && !loadingFeatured) {
            fetchFeaturedProducts(currentPage, false);
          }
        },
        hasMoreFeatured,
        loadingFeatured
      );
    };

    if (sectionsReady.discounts && DiscountProductsDiv)
      DiscountProductsDiv.addEventListener(
        "scroll",
        DiscountProductsScrollHandler
      );
    if (sectionsReady.accessories && clothesDiv)
      clothesDiv.addEventListener("scroll", clothesScrollHandler);
    if (productsDiv)
      productsDiv.addEventListener("scroll", productsScrollHandler);

    return () => {
      if (sectionsReady.discounts && DiscountProductsDiv)
        DiscountProductsDiv.removeEventListener(
          "scroll",
          DiscountProductsScrollHandler
        );
      if (sectionsReady.accessories && clothesDiv)
        clothesDiv.removeEventListener("scroll", clothesScrollHandler);
      if (productsDiv)
        productsDiv.removeEventListener("scroll", productsScrollHandler);
    };
  }, [
    handleScroll,
    fetchDiscountProducts,
    fetchClothesProducts,
    fetchFeaturedProducts,
    hasMoreDisCountProducts,
    hasMoreClothes,
    hasMoreFeatured,
    loadingDiscountProducts,
    loadingClothes,
    loadingFeatured,
    sectionsReady.discounts,
    sectionsReady.accessories,
  ]);

  // Infinite scroll for grid products
  useEffect(() => {
    if (!sectionsReady.grid) return;
    const observer = new IntersectionObserver(
      (entries) => {
        if (entries[0].isIntersecting && hasMoreGrid && !loadingGrid && gridPage > 1) {
          fetchGridProducts(gridPage, false);
        }
      },
      { threshold: 0.1 }
    );

    const currentTarget = gridObserverTarget.current;
    if (currentTarget) {
      observer.observe(currentTarget);
    }

    return () => {
      if (currentTarget) {
        observer.unobserve(currentTarget);
      }
    };
  }, [hasMoreGrid, loadingGrid, gridPage, fetchGridProducts, sectionsReady.grid]);

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

          {/* Banner Section - First thing after Announcement Bar */}
          <section className="relative bg-gradient-to-r from-orange-50 via-orange-100 to-orange-50">
            <div className="relative z-10 max-w-screen-xl mx-auto px-4 sm:px-6 md:px-8 lg:px-16 py-6 sm:py-8 md:py-10 w-full">
              <div className="w-full rounded-2xl overflow-hidden shadow-2xl border border-[#0a2540]/10 bg-white/70 backdrop-blur">
                <BannerCarousel />
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
            <ProductsRail
              ref={ProductsRef}
              items={featuredProducts}
              emptyMessage={t("noFeaturedProducts", "لا توجد منتجات مميزة حالياً.")}
              isLoadingMore={loadingFeatured}
              onRemove={handleRemoveFromFeatured}
              handleProductClick={handleProductClick}
              t={t}
              getRoleFromToken={getRoleFromToken}
            />
          </section>

          {/* Discount Products Section */}
          <section
            ref={discountSectionRef}
            className="py-6 sm:py-8 md:py-12 space-y-6 sm:space-y-8 bg-[#F9F6EF]"
          >
            <SectionHeader
              eyebrow={t("homePage.newThisWeek", "جديد هذا الأسبوع")}
              title={t("homePage.discountsUpTo", "خصومات تصل إلى 60%")}
              description={t("homePage.discountsDesc", "عروض مختارة بعناية لتلبية احتياجات الأسرة  مع جودة عالية وأسعار منافسة.")}
            />
            {sectionsReady.discounts ? (
              <ProductsRail
                ref={DiscountProductsRef}
                items={Discountproducts}
                emptyMessage={t("noDiscountProducts", "لا توجد منتجات عليها خصومات حالياً.")}
                isLoadingMore={loadingDiscountProducts}
                onRemove={handleRemoveFromDiscount}
                handleProductClick={handleProductClick}
                t={t}
                getRoleFromToken={getRoleFromToken}
              />
            ) : (
              <LazySectionPlaceholder
                message={t("homePage.scrollForDiscounts", "تابع التمرير لعرض أحدث الخصومات")}
                helper={t("homePage.lazySectionHint", "نقوم بتحميل هذا القسم فقط عند ظهوره لضمان أفضل أداء.")}
              />
            )}
          </section>

          {/* Accessories Section */}
          <section
            ref={accessoriesSectionRef}
            className="py-6 sm:py-8 md:py-12 space-y-6 sm:space-y-8 bg-[#F9F6EF]"
          >
            <SectionHeader
              eyebrow={t("homePage.latestAccessories", "حقائب نسائية واكسسوارات")}
              title={t("homePage.latestAccessories", "حقائب نسائية واكسسوارات")}
              description={t("homePage.clothesDesc", "صيحات الموسم مع خامات مريحة وتصاميم تناسب جميع الأعمار. التوصيل لجميع المحافظات.")}
            />
            {sectionsReady.accessories ? (
              <ProductsRail
                ref={clothesRef}
                items={clothesProducts}
                emptyMessage={t("noClothesProducts", "لا توجد منتجات ل إكسسوارات حالياً.")}
                isLoadingMore={loadingClothes}
                onRemove={handleRemoveFromClothes}
                handleProductClick={handleProductClick}
                t={t}
                getRoleFromToken={getRoleFromToken}
              />
            ) : (
              <LazySectionPlaceholder
                message={t("homePage.scrollForAccessories", "قم بالتمرير لمشاهدة أحدث الإكسسوارات")}
                helper={t("homePage.lazySectionHint", "سيتم تحميل المنتجات بمجرد وصولك إلى هذا القسم.")}
              />
            )}
          </section>

          {/* Grid Products Section */}
          <section
            ref={gridSectionRef}
            className="py-6 sm:py-8 md:py-12 bg-[#F9F6EF]"
          >
            <SectionHeader
              eyebrow={t("homePage.allProductsEyebrow", "تصفح الكل")}
              title={t("homePage.allProductsTitle", "كل المنتجات في مكان واحد")}
            />
            <div className="px-4 sm:px-6 md:px-8 lg:px-16">
              {!sectionsReady.grid ? (
                <LazySectionPlaceholder
                  message={t("homePage.scrollForCatalog", "تابع التمرير للوصول إلى كل المنتجات")}
                  helper={t("homePage.lazySectionHint", "نعرض المنتجات عند الحاجة فقط للحفاظ على سرعة التصفح.")}
                />
              ) : gridProducts.length === 0 && loadingGrid ? (
                <p className="text-center text-sm text-gray-600 py-6 sm:py-8">
                  {t("loadingProducts", "جارٍ التحميل...")}
                </p>
              ) : gridProducts.length === 0 ? (
                <p className="text-center text-sm text-gray-600 py-6 sm:py-8">
                  {t("homePage.noProducts", "لا توجد منتجات متاحة حالياً.")}
                </p>
              ) : (
                <>
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
                  {hasMoreGrid && (
                    <div ref={gridObserverTarget} className="flex justify-center mt-6">
                      {loadingGrid && (
                        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-orange-500"></div>
                      )}
                    </div>
                  )}
                </>
              )}
            </div>
          </section>

          <footer className="bg-white text-[#0A2C52] border-t border-blue-100 mt-10" aria-label={t("footer.heading", "روابط صفحاتنا")}>
            <div className="max-w-screen-xl mx-auto px-4 sm:px-6 md:px-8 lg:px-16 py-10 flex flex-col gap-8 md:flex-row md:items-center md:justify-between">
              <div className="space-y-3 max-w-xl">
                <WebSiteLogo
                  width={lang === "ar" ? 140 : 160}
                  height={48}
                  className="object-contain drop-shadow-sm"
                />
                <p className="text-[#1e293b] text-sm leading-relaxed">
                  {t(
                    "footer.description",
                    "تصفح كل صفحات متجرنا بسهولة، وتابع جديد الأقسام والعروض أولاً بأول."
                  )}
                </p>
              </div>
              <div className="flex flex-wrap gap-3">
                {footerLinks.map((link) => (
                  <button
                    key={link.key}
                    onClick={() => {
                      if (link.sectionId) {
                        scrollToSection(link.sectionId);
                      } else if (link.path) {
                        navigate(link.path);
                      }
                    }}
                    className="px-5 py-2.5 rounded-full bg-[#0A2C52]/5 hover:bg-[#0A2C52]/10 border border-[#0A2C52]/10 text-sm font-semibold text-[#0A2C52] transition-colors duration-200"
                  >
                    {link.label}
                  </button>
                ))}
              </div>
            </div>
            {/* Copyright Section */}
            <div className="border-t border-blue-100 mt-6 pt-6">
              <div className="max-w-screen-xl mx-auto px-4 sm:px-6 md:px-8 lg:px-16">
                <p className="text-center text-sm text-[#1e293b]">
                  {lang === "ar" 
                    ? `جميع الحقوق محفوظة © ${new Date().getFullYear()} جومانجو`
                    : `All rights reserved © ${new Date().getFullYear()} Gomango`
                  }
                </p>
              </div>
            </div>
          </footer>
        </div>
      </StoreLayout>
    </>
  );
}