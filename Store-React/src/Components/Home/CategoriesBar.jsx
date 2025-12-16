import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useI18n } from "../i18n/I18nContext";
import API_BASE_URL from "../Constant";
import { trackViewCategory } from "../utils/facebookPixel";

export default function CategoriesBar() {
  const navigate = useNavigate();
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const { t, lang } = useI18n();

  const handleSearch = (query, displayLabel) => {
    const searchValue = (query || "").trim();
    if (!searchValue) return;
    const label = (displayLabel || query || "").trim() || searchValue;
    // تتبع ViewCategory لـ Facebook Pixel
    trackViewCategory(label);
    const path = `/FindProducts?q=${encodeURIComponent(searchValue)}`;
    navigate(path, { state: { searchQuery: label, apiQuery: searchValue } });
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
            const cleanData = data.filter((item) => item && (item.name || item.categoryNameAr || item.categoryNameEn));
            setCategories(cleanData);
          } else {
            setCategories([]);
          }
        }
      } catch {
        if (isMounted) {
          setCategories([]);
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

  if (loading || categories.length === 0) {
    return null;
  }

  return (
    <div className="hidden lg:block bg-white border-b border-gray-200 shadow-sm sticky top-0 z-40">
      <div className="max-w-screen-xl mx-auto px-4">
        <div className="flex items-center gap-1 overflow-x-auto scrollbar-hide py-2">
          {categories.map((item) => {
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
            return (
              <button
                key={item.categoryId}
                onClick={() => handleSearch(searchValue, displayLabel)}
                className="px-3 py-1.5 text-sm font-medium text-gray-700 hover:text-brand-orange hover:bg-orange-50 rounded-md transition-all duration-200 whitespace-nowrap flex-shrink-0"
              >
                {item.name}
              </button>
            );
          })}
        </div>
      </div>
    </div>
  );
}


