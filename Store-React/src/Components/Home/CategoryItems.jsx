import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useI18n } from "../i18n/I18nContext";
import API_BASE_URL, { ServerPath } from "../Constant";

export default function Categories() {
  const navigate = useNavigate();
  const [shuffledCategories, setShuffledCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const { t, lang } = useI18n();

  const handleSearch = (query, displayLabel) => {
    const searchValue = (query || "").trim();
    if (!searchValue) return;
    const label = (displayLabel || query || "").trim() || searchValue;
    const path = `/FindProducts?q=${encodeURIComponent(searchValue)}`;
    navigate(path, { state: { searchQuery: label, apiQuery: searchValue } });
  };

  // ✅ دالة الشَفْل الصحيحة (Fisher–Yates shuffle)
  const shuffleArray = (array) => {
    const result = [...array];
    for (let i = result.length - 1; i > 0; i--) {
      const j = Math.floor(Math.random() * (i + 1));
      [result[i], result[j]] = [result[j], result[i]];
    }
    return result;
  };

  useEffect(() => {
    let isMounted = true;
    const fetchCategories = async () => {
      try {
        setLoading(true);
        const response = await fetch(
          `${API_BASE_URL}categories?lang=${lang || "ar"}`
        );
        if (!response.ok) {
          throw new Error("failed");
        }
        const data = await response.json();

        if (isMounted) {
          if (Array.isArray(data)) {
            // ✅ تأكيد أن العناصر كلها سليمة
            const cleanData = data.filter((item) => item && item.imagePath);
            setShuffledCategories(shuffleArray(cleanData));
          } else {
            setShuffledCategories([]);
          }
        }
      } catch {
        if (isMounted) {
          setShuffledCategories([]);
        }
      } finally {
        if (isMounted) setLoading(false);
      }
    };
    fetchCategories();
    return () => {
      isMounted = false;
    };
  }, [lang]);

  return (
    <div className="w-full px-4 md:px-8 lg:px-16 py-6 md:py-8">
      <h1 className="text-2xl md:text-3xl font-bold text-center mb-6 md:mb-8 text-brand-navy">
        {t("whatAreYouLookingFor", "ماذا تبحث عنه؟")}
      </h1>

      {loading ? (
        <p className="text-center text-gray-600 py-6">
          {t("loadingCategories", "جارٍ تحميل الأقسام...")}
        </p>
      ) : shuffledCategories.length === 0 ? (
        <p className="text-center text-gray-600 py-6">
          {t("noCategories", "لا توجد أقسام متاحة حالياً.")}
        </p>
      ) : (
        <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-3 md:gap-4 lg:gap-6">
          {shuffledCategories.map((item) => (
            <div
              key={item.categoryId}
              className="group relative bg-[#F9F6EF] rounded-xl shadow-md hover:shadow-xl transition-all duration-300 overflow-hidden cursor-pointer border border-gray-100"
              onClick={() => {
                const searchValue =
                  item.categoryNameEn ||
                  item.nameEn ||
                  item.slug ||
                  item.name ||
                  item.categoryNameAr ||
                  "";
                const displayLabel =
                  lang === "ar"
                    ? item.name || item.categoryNameAr || searchValue
                    : item.name || searchValue;
                handleSearch(searchValue, displayLabel);
              }}
            >
              <div className="relative h-32 md:h-40 lg:h-48 overflow-hidden">
                <img
                  className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-300"
                  src={
                    item.imagePath?.startsWith("http")
                      ? item.imagePath
                      : `${ServerPath}${item.imagePath}`
                  }
                  alt={item.name}
                  loading="lazy"
                />
                <div className="absolute inset-0 bg-gradient-to-t from-black/60 via-black/20 to-transparent"></div>
              </div>
              <div className="p-3 md:p-4">
                <h3 className="text-sm md:text-base font-semibold text-gray-800 line-clamp-2 text-center group-hover:text-brand-orange transition-colors">
                  {item.name}
                </h3>
              </div>
              <div className="absolute top-2 right-2 bg-brand-orange text-white text-xs px-2 py-1 rounded-full font-semibold opacity-0 group-hover:opacity-100 transition-opacity">
                {t("browse", "تصفح")}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
