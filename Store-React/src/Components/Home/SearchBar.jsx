import { useState } from "react";
import { FiSearch } from "react-icons/fi";
import API_BASE_URL from "../Constant";
import { useI18n } from "../i18n/I18nContext";

export default function SearchBar({ onSearch, searchType = "products" }) {
  const [searchQuery, setSearchQuery] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const { t, lang } = useI18n();
  const brandDarkBlue = "#0A2C52";
  const brandDarkerBlue = "#13294B";

  const handleSearch = async () => {
    if (!searchQuery.trim()) return;

    setIsLoading(true);
    const token = sessionStorage.getItem("token");

    try {
      const response = await fetch(
        `${API_BASE_URL}searchlogs/add?searchTerm=${encodeURIComponent(
          searchQuery
        )}`,
        {
          method: "POST",
          headers: token ? { Authorization: `Bearer ${token}` } : {},
        }
      );

      if (!response.ok) {
        const errorData = await response.text();
        console.error("خطأ من السيرفر:", errorData);
      }
    } catch (error) {
      console.error("فشل تسجيل البحث:", error);
    } finally {
      setIsLoading(false);
      onSearch(searchQuery);
    }
  };

  return (
    <div className="relative w-full max-w-2xl mx-auto">
      <div className="relative flex items-center group">
        <input
          type="text"
          placeholder={
            searchType === "products"
              ? t("searchProducts", "ابحث في المنتجات...")
              : t("searchOrder", "ابحث برقم الطلب...")
          }
          className="w-full py-3 pl-14 pr-4 text-right text-sm sm:text-base rounded-xl border-2 border-indigo-300 focus:border-indigo-500 focus:ring-2 focus:ring-indigo-200 bg-[#F9F6EF] backdrop-blur-sm text-gray-800 placeholder-gray-500 transition-all duration-300 shadow-lg group-hover:shadow-xl"
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          onKeyDown={(e) => {
            if (e.key === "Enter") {
              e.preventDefault();
              handleSearch();
            }
          }}
        />
        
        <button
          onClick={handleSearch}
          disabled={isLoading}
          className="absolute left-2 flex items-center justify-center w-12 h-12 rounded-xl focus:outline-none focus:ring-2 focus:ring-[#0A2C52]/30 focus:ring-offset-2 transition-all duration-300 disabled:opacity-70 disabled:cursor-not-allowed shadow-md transform group-hover:scale-105 gap-1.5"
          style={{
            background: `linear-gradient(to right, ${brandDarkBlue}, ${brandDarkerBlue})`,
            color: '#ffffff'
          }}
          onMouseEnter={(e) => {
            e.currentTarget.style.background = `linear-gradient(to right, ${brandDarkerBlue}, ${brandDarkBlue})`;
          }}
          onMouseLeave={(e) => {
            e.currentTarget.style.background = `linear-gradient(to right, ${brandDarkBlue}, ${brandDarkerBlue})`;
          }}
        >
          {isLoading ? (
            <div
              className="w-4 h-4 border-2 rounded-full animate-spin"
              style={{ borderColor: '#ffffff', borderTopColor: 'transparent' }}
            ></div>
          ) : (
            <>
              <span className="sr-only">{t("searchAction", "ابحث")}</span>
              <FiSearch size={18} style={{ color: '#60A5FA', stroke: '#60A5FA' }} />
            </>
          )}
        </button>

        {/* زر مسح للشاشات الكبيرة */}
        {searchQuery && (
          <button
            onClick={() => setSearchQuery("")}
            className="absolute left-12 flex items-center justify-center w-8 h-8 text-gray-500 hover:text-gray-700 transition-colors duration-200 hidden sm:flex"
          >
            ✕
          </button>
        )}
      </div>

      {/* إرشادات للشاشات المختلفة */}
      <div className="flex justify-between items-center mt-2 px-2">

        {searchQuery && (
          <span className="text-xs text-indigo-600 font-medium">
            {searchQuery.length}/50
          </span>
        )}
      </div>
    </div>
  );
}