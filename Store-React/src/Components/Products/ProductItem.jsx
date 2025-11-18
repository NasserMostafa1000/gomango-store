import React from "react";
import { FaEdit, FaTrash } from "react-icons/fa";
import { useNavigate } from "react-router-dom";
import API_BASE_URL, { ServerPath } from "../../Components/Constant.js";
import { useCurrency } from "../Currency/CurrencyContext";
import { useI18n } from "../i18n/I18nContext";

export default function ProductItem({ product, CurrentRole, onDeleted, layout = "rail", onClick }) {
  const {
    productId,
    productName, // API returns the correct language already
    shortName,
    productPrice,
    priceAfterDiscount,
    discountPercentage,
    productImage,
  } = product;

  const navigate = useNavigate();
  const { format } = useCurrency();
  const { t } = useI18n();

  const MoveToUpdateProductPage = (event) => {
    event.stopPropagation();
    navigate("/admin/edit-product", { state: { productId } });
  };

  const handleDelete = async (event) => {
    event.stopPropagation();
    const confirmDelete = window.confirm(t("productItem.confirmDelete", "هل أنت متأكد من حذف هذا المنتج؟"));
    if (!confirmDelete) return;

    const token = sessionStorage.getItem("token");
    if (!token) {
      alert(t("productItem.loginRequired", "يجب تسجيل الدخول"));
      return;
    }

    try {
      const res = await fetch(`${API_BASE_URL}Product/${productId}`, {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
      if (res.status === 204) {
        if (typeof onDeleted === "function") {
          onDeleted(productId);
        } else {
          window.location.reload();
        }
      } else if (res.status === 404) {
        alert(t("productItem.notFound", "المنتج غير موجود"));
      } else if (res.status === 403 || res.status === 401) {
        alert(t("productItem.noPermission", "ليس لديك الصلاحية لحذف المنتج"));
      } else {
        const body = await res.json().catch(() => null);
        alert(body?.message ?? t("productItem.deleteFailed", "فشل حذف المنتج"));
      }
    } catch (error) {
      console.error("خطأ أثناء حذف المنتج", error);
      alert(t("productItem.deleteError", "حدث خطأ أثناء حذف المنتج"));
    }
  };

  const isGrid = layout === "grid";
  const containerClass = isGrid
    ? "relative flex flex-col h-full bg-white border border-gray-200 rounded-xl shadow-sm hover:shadow-xl transition-all duration-300 overflow-hidden group hover:-translate-y-1"
    : "relative bg-white rounded-lg shadow-md hover:shadow-xl transition-all duration-300 select-none overflow-hidden border border-gray-100 group cursor-pointer";
  const sizingClass = isGrid ? "w-full" : "min-w-[170px] max-w-[200px]";
  const imageWrapperClass = isGrid
    ? "relative w-full h-56 sm:h-60 bg-white flex items-center justify-center p-4 border-b border-gray-100"
    : "relative h-40 md:h-44 overflow-hidden bg-gray-100";
  const infoWrapperClass = isGrid ? "p-4 flex flex-col gap-2 flex-1" : "p-3 space-y-1";
  const titleClass = isGrid
    ? "text-lg font-semibold text-gray-900 line-clamp-2 group-hover:text-brand-orange transition-colors"
    : "text-base font-semibold text-gray-900 line-clamp-1 group-hover:text-brand-orange transition-colors";
  const subtitleClass = isGrid
    ? "text-sm text-gray-600 line-clamp-2 min-h-[36px]"
    : "text-xs text-gray-500 line-clamp-2 min-h-[32px]";
  const priceWrapperClass = isGrid ? "mt-auto space-y-1" : "mt-3 space-y-1";

  return (
    <div
      className={`${containerClass} ${sizingClass} ${
        onClick ? "cursor-pointer focus:outline-none focus:ring-2 focus:ring-brand-orange/40" : ""
      }`}
      onClick={onClick}
      role={onClick ? "button" : undefined}
      tabIndex={onClick ? 0 : undefined}
    >
      {(CurrentRole === "Admin" || CurrentRole === "Manager") && (
        <div className="absolute top-3 left-3 flex flex-col gap-2 z-10">
          <button
            onClick={MoveToUpdateProductPage}
            className="inline-flex items-center gap-2 text-xs font-semibold px-3 py-1.5 rounded-full bg-white/90 text-[#0a2540] hover:bg-white shadow-lg transition-all border border-[#0a2540]/10"
          >
            <FaEdit className="text-[#0a2540]" size={12} />
            {t("productItem.edit", "تعديل")}
          </button>
          <button
            onClick={handleDelete}
            className="inline-flex items-center gap-2 text-xs font-semibold px-3 py-1.5 rounded-full bg-red-500 text-white hover:bg-red-600 shadow-lg transition-all border border-red-500/20"
          >
            <FaTrash size={12} />
            {t("productItem.delete", "حذف")}
          </button>
        </div>
      )}

      {productImage && (
        <div className={imageWrapperClass}>
          <img
            src={ServerPath + productImage}
            alt={productName}
            className={`${
              isGrid
                ? "h-full w-full object-contain transition-transform duration-500 group-hover:scale-105"
                : "w-full h-full object-cover group-hover:scale-110 transition-transform duration-300"
            }`}
            loading="lazy"
          />
          {discountPercentage > 0 && (
            <div className="absolute top-2 right-2 bg-red-500 text-white text-xs font-bold px-2 py-1 rounded-full shadow-lg">
              %{discountPercentage}
            </div>
          )}
        </div>
      )}

      <div className={infoWrapperClass}>
        <div className={titleClass}>
          {shortName || productName}
        </div>
        <div className={subtitleClass}>
          {productName}
        </div>

        <div className={priceWrapperClass}>
          {discountPercentage > 0 && (
            <div className="text-xs md:text-sm line-through opacity-50 text-gray-500">
              {format(productPrice)}
            </div>
          )}
          <div className="text-brand-orange font-bold text-base md:text-lg">
            {format(discountPercentage > 0 ? priceAfterDiscount : productPrice)}
          </div>
        </div>
      </div>
    </div>
  );
}
