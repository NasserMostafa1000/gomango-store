import React, { useState, useEffect } from "react";
import { Helmet } from "react-helmet";
import { useNavigate, useLocation } from "react-router-dom";
import StoreLayout from "../Home/StoreLayout";
import API_BASE_URL, { SiteName } from "../Constant.js";
import { getRoleFromToken } from "../utils.js";
import { useI18n } from "../i18n/I18nContext";
import ProductItem from "./ProductItem.jsx";
import BackButton from "../Common/BackButton";

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
  const navigate = useNavigate();
  const { t, lang } = useI18n();

  useEffect(() => {
    window.scrollTo(0, 0);

    let isMounted = true;
    const fetchProducts = async () => {
      if (!apiQuery) {
        if (isMounted) {
          setProducts([]);
          setLoading(false);
        }
        return;
      }
      try {
        setLoading(true);
        const response = await fetch(
          `${API_BASE_URL}Product/GetProductsByName?Name=${encodeURIComponent(apiQuery)}&lang=${lang}`
        );
        if (!response.ok) {
          throw new Error("Network response was not ok");
        }
        const data = await response.json();
        if (!isMounted) return;
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
        setProducts(normalized);
      } catch (error) {
        console.error("Error fetching data:", error);
        if (isMounted) {
          setProducts([]);
        }
      } finally {
        if (isMounted) {
          setLoading(false);
        }
      }
    };

    fetchProducts();
    return () => {
      isMounted = false;
    };
  }, [apiQuery, lang]);

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
        {loading ? (
          <div className="flex justify-center items-center py-8">
            <div className="animate-spin rounded-full h-10 w-10 border-b-2 border-orange-500"></div>
            <span className="mr-3 text-blue-700 font-semibold text-sm">
              {t("findProducts.loading", "جارٍ التحميل...")}
            </span>
          </div>
        ) : (
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