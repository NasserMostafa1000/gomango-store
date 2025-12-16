import React, { useState, useEffect, useRef, useCallback } from "react";
import { Helmet } from "react-helmet";
import { useNavigate, useLocation } from "react-router-dom";
import StoreLayout from "../Home/StoreLayout";
import API_BASE_URL, { SiteName } from "../Constant.js";
import { getRoleFromToken } from "../utils.js";
import { useI18n } from "../i18n/I18nContext";
import ProductItem from "./ProductItem.jsx";
import BackButton from "../Common/BackButton";
import { trackSearch, trackViewCategory } from "../utils/facebookPixel";

export default function FindProducts() {
  const location = useLocation();
  const params = new URLSearchParams(location.search);
  const rawQuery = params.get("q") ?? "";
  const stateDisplayQuery = typeof location.state?.searchQuery === "string" ? location.state.searchQuery : null;
  const stateApiQuery = typeof location.state?.apiQuery === "string" ? location.state.apiQuery : null;
  const displayQuery = (stateDisplayQuery ?? rawQuery ?? "").trim();
  const apiQuery = (stateApiQuery ?? rawQuery ?? "").trim();
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(false);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [loadingMore, setLoadingMore] = useState(false);
  const navigate = useNavigate();
  const { t, lang } = useI18n();
  const observerTarget = React.useRef(null);
  const allProductsRef = React.useRef([]);

  const fetchProducts = useCallback(async (pageNum, reset = false) => {
    if (!apiQuery) {
      setProducts([]);
      setLoading(false);
      allProductsRef.current = [];
      return;
    }
    
    try {
      if (reset) {
        setLoading(true);
        // جلب جميع المنتجات مرة واحدة
        const response = await fetch(
          `${API_BASE_URL}Product/GetProductsByName?Name=${encodeURIComponent(apiQuery)}&lang=${lang}`
        );
        if (!response.ok) {
          throw new Error("Network response was not ok");
        }
        const data = await response.json();
        const normalized = Array.isArray(data)
          ? data.map((product) => ({
              ...product,
              shortName: product.shortName || product.productName,
              productName:
                product.productName ||
                product.productNameAr ||
                product.productNameEn ||
                "",
            }))
          : [];
        
        allProductsRef.current = normalized;
        const itemsPerPage = 10;
        const startIndex = 0;
        const endIndex = startIndex + itemsPerPage;
        setProducts(normalized.slice(startIndex, endIndex));
        setPage(2);
        setHasMore(normalized.length > itemsPerPage);
        
        // تتبع Search أو ViewCategory لـ Facebook Pixel
        if (apiQuery) {
          // إذا كان الاستعلام يشبه اسم قسم (يمكن تحسين هذا المنطق)
          const isCategory = normalized.length > 0 && normalized[0]?.categoryName;
          if (isCategory) {
            trackViewCategory(displayQuery || apiQuery, normalized);
          } else {
            trackSearch(displayQuery || apiQuery, normalized.length);
          }
        }
      } else {
        setLoadingMore(true);
        // استخدام البيانات المحفوظة
        const itemsPerPage = 10;
        const startIndex = (pageNum - 1) * itemsPerPage;
        const endIndex = startIndex + itemsPerPage;
        const nextItems = allProductsRef.current.slice(startIndex, endIndex);
        
        if (nextItems.length > 0) {
          setProducts((prev) => {
            const existingIds = new Set(prev.map(p => p.productId));
            return [...prev, ...nextItems.filter(p => !existingIds.has(p.productId))];
          });
          setPage((prev) => prev + 1);
          setHasMore(endIndex < allProductsRef.current.length);
        } else {
          setHasMore(false);
        }
      }
    } catch (error) {
      console.error("Error fetching data:", error);
      setHasMore(false);
    } finally {
      setLoading(false);
      setLoadingMore(false);
    }
  }, [apiQuery, lang]);

  useEffect(() => {
    window.scrollTo(0, 0);
    setProducts([]);
    setPage(1);
    setHasMore(true);
    fetchProducts(1, true);
  }, [apiQuery, lang]);

  useEffect(() => {
    const observer = new IntersectionObserver(
      (entries) => {
        if (entries[0].isIntersecting && hasMore && !loadingMore && !loading) {
          fetchProducts(page, false);
        }
      },
      { threshold: 0.1 }
    );

    const currentTarget = observerTarget.current;
    if (currentTarget) {
      observer.observe(currentTarget);
    }

    return () => {
      if (currentTarget) {
        observer.unobserve(currentTarget);
      }
    };
  }, [hasMore, loadingMore, loading, page, fetchProducts]);

  const handleProductClick = (product) => {
    navigate(`/ProductDetails/${product.productId}`, { state: { product } });
  };

  const CurrentRole = getRoleFromToken(sessionStorage.getItem("token"));

  return (
    <StoreLayout>
      <div className="bg-blue-50">
        <Helmet>
          <title>
            {t("findProducts.metaTitle", "نتائج البحث")}{" "}
            {displayQuery ? `'${displayQuery}'` : ""} | {SiteName}
          </title>
          <meta
            name="description"
            content={t(
              "findProducts.metaDesc",
              "استكشف نتائج البحث لكلمة '{query}' والعروض الحصرية في موقعنا."
            ).replace("{query}", displayQuery || "")}
          />
          <meta name="robots" content="noindex, follow" />
        </Helmet>
        
        {/* العنوان الرئيسي - بدون مسافات */}
        <div className="bg-white shadow-sm border-b border-blue-200 sticky top-0 z-20">
          <div className="max-w-7xl mx-auto px-2 sm:px-4 py-2">
            <div className="flex items-center gap-4 mb-2">
              <BackButton />
            </div>
          </div>
        </div>

        {/* محتوى النتائج - يبدأ مباشرة بدون مسافات */}
        <div className="max-w-7xl mx-auto px-2 sm:px-4 pt-1 pb-4">
        {loading && products.length === 0 ? (
          <div className="flex justify-center items-center py-8">
            <div className="animate-spin rounded-full h-10 w-10 border-b-2 border-orange-500"></div>
            <span className="mr-3 text-blue-700 font-semibold text-sm">
              {t("findProducts.loading", "جارٍ التحميل...")}
            </span>
          </div>
        ) : (
          <>
            <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-3 sm:gap-4 md:gap-6">
              {products.map((product) => (
                <ProductItem
                  key={product.productId}
                  product={product}
                  CurrentRole={CurrentRole}
                  layout="grid"
                  onClick={() => handleProductClick(product)}
                  onDeleted={(deletedId) =>
                    setProducts((prev) =>
                      prev.filter(
                        (item) => item.productId !== (deletedId ?? product.productId)
                      )
                    )
                  }
                />
              ))}
            </div>
            
            {/* Infinite scroll trigger */}
            {hasMore && (
              <div ref={observerTarget} className="flex justify-center items-center py-8">
                {loadingMore && (
                  <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-orange-500"></div>
                )}
              </div>
            )}
          </>
        )}

        {/* حالة عدم وجود نتائج */}
        {!loading && products.length === 0 && (
          <div className="text-center py-6">
            <div className="bg-white rounded-xl shadow-lg p-6 max-w-md mx-auto border border-blue-200">
              <div className="text-5xl mb-3">🔍</div>
              <h3 className="text-lg font-bold text-blue-900 mb-2">
                {lang === "ar" ? "لم يتم العثور على نتائج البحث" : "No search results found"}
              </h3>
              <button
                onClick={() => navigate("/")}
                className="bg-orange-500 hover:bg-orange-600 text-white font-semibold py-2 px-6 rounded-full transition duration-300 transform hover:-translate-y-1"
              >
                {t("findProducts.backToHome", "العودة للرئيسية")}
              </button>
            </div>
          </div>
        )}
        </div>
      </div>
    </StoreLayout>
  );
}