import React, { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import API_BASE_URL, { ServerPath } from "../../../Constant";
import { colors, sizes } from "../../../utils";
import { FiPackage, FiImage, FiPlus, FiSave, FiLoader, FiArrowLeft, FiTag, FiDollarSign, FiPercent, FiEdit } from "react-icons/fi";
import { useI18n } from "../../../i18n/I18nContext";
import WebSiteLogo from "../../../WebsiteLogo/WebsiteLogo.jsx";

export default function ProductForm() {
  const [product, setProduct] = useState(null);
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState("");
  const [imageLoading, setImageLoading] = useState(null);
  const navigate = useNavigate();
  const location = useLocation();
  const productId = location.state?.productId;
  const { t } = useI18n();
  const primaryButtonClasses =
    "bg-[#0A2C52] hover:bg-[#13345d] text-white font-semibold rounded-xl transition-all duration-300 shadow-lg hover:shadow-xl disabled:bg-gray-400 disabled:text-white/80 disabled:cursor-not-allowed disabled:shadow-none flex items-center justify-center gap-2";
  
  // Function to translate color names (from Arabic to English or vice versa)
  const translateColor = (colorName) => {
    if (!colorName) return colorName;
    
    // Map Arabic color names to English keys
    const colorMap = {
      "أحمر": "red",
      "أزرق": "blue",
      "أخضر": "green",
      "أصفر": "yellow",
      "أسود": "black",
      "أبيض": "white",
      "وردي": "pink",
      "بنفسجي": "purple",
      "برتقالي": "orange",
      "بني": "brown",
      "رمادي": "gray",
      "كحلي": "navy",
      "بيج": "beige",
      "كاكي": "khaki",
      "كستنائي": "maroon",
      "سماوي": "cyan",
      "أرجواني": "magenta",
      "ليموني": "lime",
      "زيتوني": "olive",
      "تركواز": "teal",
      "فضي": "silver",
      "ذهبي": "gold",
      "نيلي": "navy",
      "عنابي": "maroon",
      "خردلي": "yellow",
      "فيروزي": "cyan",
      "زهري": "pink",
      "لافندر": "purple",
      "موف": "purple",
      "أخضر زيتي": "olive",
      "أخضر فاتح": "green",
      "أزرق سماوي": "cyan",
      "أزرق ملكي": "blue",
      "قرمزي": "red",
    };
    
    // Check if the color name is in Arabic
    const colorKey = colorMap[colorName] || colorName.toLowerCase().trim();
    return t(`colors.${colorKey}`, colorName);
  };

  useEffect(() => {
    fetchProduct();
    fetchCategories();
  }, [productId]);

  useEffect(() => {
    window.scrollTo(0, 0);
  }, [message]);

  const fetchProduct = async () => {
    setLoading(true);
    try {
      const response = await fetch(
        `${API_BASE_URL}Product/GetFullProduct?ProductId=${productId}`,
        {
          method: "GET",
          headers: {
            Authorization: `Bearer ${sessionStorage.getItem("token")}`,
            "Content-Type": "application/json",
          },
        }
      );
      const data = await response.json();
      setProduct(data);
    } catch (error) {
      setMessage("❌ " + t("productForm.loadError", "خطأ في تحميل المنتج."));
    }
    setLoading(false);
  };

  const fetchCategories = async () => {
    try {
      const response = await fetch(
        `${API_BASE_URL}Product/GetCategoriesNames`,
        {
          method: "GET",
          headers: {
            Authorization: `Bearer ${sessionStorage.getItem("token")}`,
            "Content-Type": "application/json",
          },
        }
      );
      const data = await response.json();
      setCategories(data);
    } catch (error) {
      console.error("❌ خطأ في تحميل التصنيفات.");
    }
  };

  const handleUpdate = async () => {
    setLoading(true);
    setMessage("⏳ " + t("productForm.updating", "جاري التحديث... يمكن أن يأخذ هذا بعض الوقت."));
    try {
      const response = await fetch(
        `${API_BASE_URL}Product/UpdateProduct?id=${productId}`,
        {
          method: "PUT",
          headers: {
            Authorization: `Bearer ${sessionStorage.getItem("token")}`,
            "Content-Type": "application/json",
          },
          body: JSON.stringify(product),
        }
      );
      if (response.ok) {
        setMessage("✅ " + t("productForm.updateSuccess", "تم تحديث المنتج بنجاح!"));
      } else {
        setMessage("❌ " + t("productForm.updateError", "حدث خطأ أثناء التحديث."));
      }
    } catch (error) {
      setMessage("❌ حدث خطأ أثناء التحديث.");
    }
    setLoading(false);
  };

  const handleImageUpdate = async (productDetailId) => {
    const fileInput = document.createElement("input");
    fileInput.type = "file";
    fileInput.accept = "image/*";

    fileInput.onchange = async (event) => {
      const file = event.target.files[0];
      if (!file) return;

      setImageLoading(productDetailId);
      const formData = new FormData();
      formData.append("imageFile", file);

      try {
        const response = await fetch(
          `${API_BASE_URL}Product/UpdateProductImage?ProductDetailsId=${productDetailId}`,
          {
            method: "PUT",
            headers: {
              Authorization: `Bearer ${sessionStorage.getItem("token")}`,
            },
            body: formData,
          }
        );

        if (response.ok) {
          const data = await response.json();
          setProduct((prevProduct) => ({
            ...prevProduct,
            productDetails: prevProduct.productDetails.map((detail) =>
              detail.productDetailId === productDetailId
                ? { ...detail, productImage: data.fileUrl }
                : detail
            ),
          }));
          setMessage("✅ " + t("productForm.imageUpdateSuccess", "تم تحديث الصورة بنجاح!"));
        } else {
          setMessage("❌ " + t("productForm.imageUpdateError", "حدث خطأ أثناء تحديث الصورة."));
        }
      } catch (error) {
        setMessage("❌ حدث خطأ أثناء تحديث الصورة.");
      } finally {
        setImageLoading(null);
      }
    };

    fileInput.click();
  };

  const handleAddDetails = () => {
    navigate("/admins/AddProductDetails", {
      state: { productId: product.productId },
    });
  };

  if (loading && !product) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-blue-50 to-orange-50 flex items-center justify-center">
        <div className="bg-white rounded-2xl shadow-lg p-8 flex flex-col items-center gap-4">
          <FiLoader className="animate-spin text-4xl text-orange-500" />
          <p className="text-gray-600 text-lg">{t("productForm.loading", "جاري تحميل بيانات المنتج...")}</p>
        </div>
      </div>
    );
  }

  if (!product) return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 to-orange-50 flex items-center justify-center">
      <div className="bg-white rounded-2xl shadow-lg p-8 text-center">
        <div className="text-6xl mb-4">⚠️</div>
        <p className="text-gray-700 text-lg">{t("productForm.notAvailable", "المنتج غير متوفر")}</p>
      </div>
    </div>
  );

  return (
    <div className="min-h-screen bg-[#F9F6EF] py-8 px-4">
      <div className="max-w-4xl mx-auto">
        {/* Header */}
        <div className="bg-[#F9F6EF] rounded-2xl shadow-lg p-6 mb-6 border border-[#0A2C52]/20">
          <div className="flex flex-col items-center mb-4">
            <WebSiteLogo width={300} height={100} className="mb-4" />
          </div>
          <div className="flex flex-col lg:flex-row lg:items-center lg:justify-between gap-4">
            <div className="flex items-center gap-3">
              <button
                onClick={() => navigate(-1)}
                className="p-2 bg-blue-100 text-blue-700 rounded-lg hover:bg-blue-200 transition-colors"
              >
                <FiArrowLeft className="text-xl" />
              </button>
              <div className="p-3 bg-gradient-to-r from-orange-500 to-orange-600 rounded-xl">
                <FiPackage className="text-white text-2xl" />
              </div>
              <div>
                <h1 className="text-2xl font-bold text-[#0A2C52]">{t("productForm.updateProduct", "تحديث المنتج")}</h1>
                <p className="text-gray-600 mt-1">{t("productForm.editData", "تعديل بيانات")} {product.productNameAr || product.productNameEn}</p>
              </div>
            </div>
            
            <button
              onClick={handleAddDetails}
              className={`px-6 py-3 ${primaryButtonClasses} transform hover:-translate-y-1`}
            >
              <FiPlus className="text-lg" />
              <span>{t("productForm.addDetails", "إضافة تفاصيل إضافية")}</span>
            </button>
          </div>
        </div>

        {/* Message Alert */}
        {message && (
          <div className={`rounded-2xl p-4 mb-6 shadow-lg ${
            message.includes("✅") 
              ? "bg-green-100 text-green-700 border border-green-200" 
              : message.includes("⏳")
              ? "bg-blue-100 text-blue-700 border border-blue-200"
              : "bg-red-100 text-red-700 border border-red-200"
          }`}>
            <div className="flex items-center gap-3">
              <div className={`text-xl ${
                message.includes("✅") ? "text-green-600" : 
                message.includes("⏳") ? "text-blue-600" : "text-red-600"
              }`}>
                {message.includes("✅") ? "✅" : message.includes("⏳") ? "⏳" : "❌"}
              </div>
              <span>{message.replace(/[✅❌⏳]/g, '').trim()}</span>
            </div>
          </div>
        )}

        {/* Product Form */}
        <div className="bg-[#F9F6EF] rounded-2xl shadow-lg p-6 mb-6 border border-[#0A2C52]/20">
          <h2 className="text-xl font-bold text-[#0A2C52] mb-6 flex items-center gap-2">
            <FiEdit />
            {t("productForm.basicInfo", "المعلومات الأساسية")}
          </h2>
          
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            {/* Product Name Arabic */}
            <div>
              <label className="block text-sm font-medium text-[#0A2C52] mb-2 flex items-center gap-2">
                <FiTag className="text-[#F55A00]" />
                {t("productForm.productName", "اسم المنتج")} (عربي) <span className="text-red-500">*</span>
              </label>
              <input
                type="text"
                className="w-full px-4 py-3 border border-[#0A2C52]/30 rounded-xl focus:ring-2 focus:ring-[#F55A00] focus:border-[#F55A00] transition-all duration-300 bg-[#F9F6EF]"
                value={product.productNameAr || ""}
                onChange={(e) =>
                  setProduct({ ...product, productNameAr: e.target.value })
                }
                required
              />
            </div>

            {/* Product Name English */}
            <div>
              <label className="block text-sm font-medium text-[#0A2C52] mb-2 flex items-center gap-2">
                <FiTag className="text-[#F55A00]" />
                {t("productForm.productName", "اسم المنتج")} (إنجليزي) <span className="text-red-500">*</span>
              </label>
              <input
                type="text"
                className="w-full px-4 py-3 border border-[#0A2C52]/30 rounded-xl focus:ring-2 focus:ring-[#F55A00] focus:border-[#F55A00] transition-all duration-300 bg-[#F9F6EF]"
                value={product.productNameEn || ""}
                onChange={(e) =>
                  setProduct({ ...product, productNameEn: e.target.value })
                }
                required
              />
            </div>

              {/* Short Name Arabic */}
              <div>
                <label className="block text-sm font-medium text-[#0A2C52] mb-2 flex items-center gap-2">
                  <FiTag className="text-[#F55A00]" />
                  {t("productForm.shortNameAr", "الاسم المختصر")} (عربي) <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  className="w-full px-4 py-3 border border-[#0A2C52]/30 rounded-xl focus:ring-2 focus:ring-[#F55A00] focus:border-[#F55A00] transition-all duration-300 bg-[#F9F6EF]"
                  value={product.shortNameAr || ""}
                  onChange={(e) =>
                    setProduct({ ...product, shortNameAr: e.target.value })
                  }
                  required
                />
              </div>

              {/* Short Name English */}
              <div>
                <label className="block text-sm font-medium text-[#0A2C52] mb-2 flex items-center gap-2">
                  <FiTag className="text-[#F55A00]" />
                  {t("productForm.shortNameEn", "Short name")} (English) <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  className="w-full px-4 py-3 border border-[#0A2C52]/30 rounded-xl focus:ring-2 focus:ring-[#F55A00] focus:border-[#F55A00] transition-all duration-300 bg-[#F9F6EF]"
                  value={product.shortNameEn || ""}
                  onChange={(e) =>
                    setProduct({ ...product, shortNameEn: e.target.value })
                  }
                  required
                />
              </div>

            {/* Product Price */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2 flex items-center gap-2">
                <FiDollarSign className="text-orange-500" />
                {t("productForm.price", "السعر")}
              </label>
              <input
                type="number"
                className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-orange-500 focus:border-orange-500 transition-all duration-300 bg-gray-50"
                value={product.productPrice || ""}
                onChange={(e) =>
                  setProduct({ ...product, productPrice: e.target.value })
                }
              />
            </div>

            {/* Category */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2 flex items-center gap-2">
                <FiPackage className="text-orange-500" />
                {t("productForm.category", "التصنيف")}
              </label>
              <select
                className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-orange-500 focus:border-orange-500 transition-all duration-300 bg-gray-50"
                value={product.categoryName || ""}
                onChange={(e) =>
                  setProduct({ ...product, categoryName: e.target.value })
                }
              >
                {categories.map((cat) => (
                  <option key={cat.categoryId} value={cat.categoryNameAr}>
                    {`${cat.categoryNameAr} / ${cat.categoryNameEn}`}
                  </option>
                ))}
              </select>
            </div>

            <div className="flex items-center justify-between">
              <span className="text-sm font-medium text-gray-700">
                {t("productForm.featured", "عرض المنتج في الصفحة الرئيسية")}
              </span>
              <label className="inline-flex items-center cursor-pointer">
                <input
                  type="checkbox"
                  className="sr-only peer"
                  checked={product.isFeatured || false}
                  onChange={(e) =>
                    setProduct({ ...product, isFeatured: e.target.checked })
                  }
                />
                <div className="w-11 h-6 bg-gray-200 rounded-full peer peer-focus:ring-4 peer-focus:ring-orange-300 transition after:content-[''] after:absolute after:h-5 after:w-5 after:bg-white after:rounded-full after:transition-all peer-checked:bg-orange-500 relative"></div>
                <span className="ms-3 text-sm font-medium text-gray-700">
                  {product.isFeatured ? t("yes", "نعم") : t("no", "لا")}
                </span>
              </label>
            </div>

            {/* Discount Percentage */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2 flex items-center gap-2">
                <FiPercent className="text-orange-500" />
                {t("productForm.discount", "نسبة الخصم")}
              </label>
              <input
                type="number"
                className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-orange-500 focus:border-orange-500 transition-all duration-300 bg-gray-50"
                value={product.discountPercentage || ""}
                onChange={(e) =>
                  setProduct({ ...product, discountPercentage: e.target.value })
                }
              />
            </div>
          </div>

          {/* More Details Arabic */}
          <div className="mt-6">
            <label className="block text-sm font-medium text-[#0A2C52] mb-2">
              {t("productForm.moreDetails", "التفاصيل الإضافية")} (عربي) <span className="text-red-500">*</span>
            </label>
            <textarea
              className="w-full px-4 py-3 border border-[#0A2C52]/30 rounded-xl focus:ring-2 focus:ring-[#F55A00] focus:border-[#F55A00] transition-all duration-300 bg-[#F9F6EF] min-h-[120px]"
              value={product.moreDetailsAr || ""}
              onChange={(e) =>
                setProduct({ ...product, moreDetailsAr: e.target.value })
              }
              required
            />
          </div>

          {/* More Details English */}
          <div className="mt-6">
            <label className="block text-sm font-medium text-[#0A2C52] mb-2">
              {t("productForm.moreDetails", "التفاصيل الإضافية")} (إنجليزي) <span className="text-red-500">*</span>
            </label>
            <textarea
              className="w-full px-4 py-3 border border-[#0A2C52]/30 rounded-xl focus:ring-2 focus:ring-[#F55A00] focus:border-[#F55A00] transition-all duration-300 bg-[#F9F6EF] min-h-[120px]"
              value={product.moreDetailsEn || ""}
              onChange={(e) =>
                setProduct({ ...product, moreDetailsEn: e.target.value })
              }
              required
            />
          </div>
        </div>

        {/* Product Details */}
        <div className="bg-[#F9F6EF] rounded-2xl shadow-lg p-6 border border-[#0A2C52]/20">
          <h3 className="text-xl font-bold text-[#0A2C52] mb-6 flex items-center gap-2">
            <FiImage />
            {t("productForm.productDetails", "تفاصيل المنتج")}
          </h3>

          <div className="space-y-6">
            {product.productDetails.map((detail, index) => (
              <div key={detail.productDetailId} className="border border-[#0A2C52]/20 rounded-2xl p-6 hover:border-[#F55A00] transition-all duration-300 bg-white">
                {/* Summary */}
                <div className="bg-[#F9F6EF] rounded-xl p-4 mb-4 border border-[#0A2C52]/10">
                  <p className="text-[#0A2C52] font-medium text-center">
                    {t("productForm.youHave", "لديك")} {detail.quantity} {t("productForm.pieces", "قطعة")} {t("productForm.of", "من")} {product.productNameAr || product.productNameEn} {t("productForm.withColor", "بلون")}{" "}
                    {translateColor(detail.colorName)}
                    {detail.sizeName !== "غير محدد" && ` ${t("productForm.withSize", "بمقاس")} ${detail.sizeName}`}
                  </p>
                </div>

                <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                  {/* Image Section */}
                  <div className="flex flex-col items-center">
                    <div className="relative bg-gray-100 rounded-2xl p-4 w-full max-w-xs">
                      {detail.productImage ? (
                        <img
                          src={ServerPath + detail.productImage}
                          alt={`${product.productNameAr || product.productNameEn}`}
                          className="w-full h-48 object-cover rounded-xl shadow-md"
                        />
                      ) : (
                        <div className="w-full h-48 bg-gray-200 rounded-xl flex items-center justify-center">
                          <p className="text-gray-500">🚫 لا توجد صورة متاحة</p>
                        </div>
                      )}
                      <button
                        onClick={() => handleImageUpdate(detail.productDetailId)}
                        disabled={imageLoading === detail.productDetailId}
                        className={`w-full mt-4 py-2 px-4 ${primaryButtonClasses} transform hover:-translate-y-1 ${imageLoading === detail.productDetailId ? "opacity-80 cursor-wait" : ""}`}
                      >
                        {imageLoading === detail.productDetailId ? (
                          <FiLoader className="animate-spin" />
                        ) : (
                          <FiImage />
                        )}
                        {t("productForm.updateImage", "تحديث الصورة")}
                      </button>
                    </div>
                  </div>

                  {/* Details Form */}
                  <div className="space-y-4">
                    {/* Color */}
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-2">
                        {t("productForm.color", "اللون")}
                      </label>
                      <select
                        className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-orange-500 focus:border-orange-500 transition-all duration-300 bg-gray-50"
                        value={detail.colorName || ""}
                        onChange={(e) =>
                          setProduct({
                            ...product,
                            productDetails: product.productDetails.map((d, i) =>
                              i === index ? { ...d, colorName: e.target.value } : d
                            ),
                          })
                        }
                      >
                        {colors.map((color) => (
                          <option key={color.ColorId} value={color.ColorName}>
                            {translateColor(color.ColorName)}
                          </option>
                        ))}
                      </select>
                    </div>

                    {/* Size */}
                    {detail.sizeName !== "غير محدد" && (
                      <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                          {t("productForm.size", "المقاس")}
                        </label>
                        <select
                          className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-orange-500 focus:border-orange-500 transition-all duration-300 bg-gray-50"
                          value={detail.sizeName || ""}
                          onChange={(e) =>
                            setProduct({
                              ...product,
                              productDetails: product.productDetails.map((d, i) =>
                                i === index ? { ...d, sizeName: e.target.value } : d
                              ),
                            })
                          }
                        >
                          {sizes.map((size) => (
                            <option key={size.SizeId} value={size.SizeName}>
                              {size.SizeName}
                            </option>
                          ))}
                        </select>
                      </div>
                    )}

                    {/* Quantity */}
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-2">
                        {t("productForm.quantity", "الكمية")}
                      </label>
                      <input
                        type="number"
                        className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-orange-500 focus:border-orange-500 transition-all duration-300 bg-gray-50"
                        value={detail.quantity || ""}
                        onChange={(e) =>
                          setProduct({
                            ...product,
                            productDetails: product.productDetails.map((d, i) =>
                              i === index ? { ...d, quantity: e.target.value } : d
                            ),
                          })
                        }
                      />
                    </div>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>

        {/* Save Button */}
        <div className="mt-6 flex justify-center">
          <button
            onClick={handleUpdate}
            disabled={loading}
            className={`min-w-[200px] px-8 py-4 text-lg font-medium ${primaryButtonClasses} transform hover:-translate-y-1 ${loading ? "opacity-80 cursor-wait" : ""}`}
          >
            {loading ? (
              <FiLoader className="animate-spin" />
            ) : (
              <FiSave />
            )}
            {loading ? t("productForm.updating", "جاري التحديث...") : t("productForm.saveChanges", "حفظ التعديلات")}
          </button>
        </div>
      </div>
    </div>
  );
}