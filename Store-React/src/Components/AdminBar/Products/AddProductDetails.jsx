import React, { useState, useRef, useEffect } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import API_BASE_URL, { ServerPath } from "../../Constant";
import { colors, sizes } from "../../utils";
import { FiPlus, FiImage, FiCheck, FiArrowRight, FiUpload, FiInfo } from "react-icons/fi";
import WebSiteLogo from "../../WebsiteLogo/WebsiteLogo.jsx";
import { useI18n } from "../../i18n/I18nContext";

const watermarkAssetPath = "/ProjectImages/SouqLogoEn.webp";
let cachedWatermarkPromise = null;

const loadImageFromSrc = (src) =>
  new Promise((resolve, reject) => {
    const img = new Image();
    img.crossOrigin = "anonymous";
    img.onload = () => resolve(img);
    img.onerror = () => reject(new Error("فشل في تحميل شعار المتجر."));
    img.src = src;
  });

const loadImageFromFile = (file) =>
  new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = () => {
      const img = new Image();
      img.onload = () => resolve(img);
      img.onerror = () => reject(new Error("تعذر قراءة ملف الصورة."));
      img.src = reader.result;
    };
    reader.onerror = () => reject(new Error("تعذر قراءة ملف الصورة."));
    reader.readAsDataURL(file);
  });

const getWatermarkImage = () => {
  if (!cachedWatermarkPromise) {
    cachedWatermarkPromise = loadImageFromSrc(watermarkAssetPath);
  }
  return cachedWatermarkPromise;
};

const addWatermarkToFile = async (file) => {
  if (!file) {
    throw new Error("لم يتم اختيار صورة.");
  }
  const [baseImage, watermarkImage] = await Promise.all([
    loadImageFromFile(file),
    getWatermarkImage(),
  ]);

  if (!baseImage.width || !baseImage.height) {
    throw new Error("ملف الصورة غير صالح.");
  }

  const canvas = document.createElement("canvas");
  canvas.width = baseImage.width;
  canvas.height = baseImage.height;
  const ctx = canvas.getContext("2d");

  ctx.drawImage(baseImage, 0, 0, canvas.width, canvas.height);

  const maxWatermarkWidth = canvas.width * 0.4;
  const maxWatermarkHeight = canvas.height * 0.4;
  const scale = Math.min(
    maxWatermarkWidth / watermarkImage.width,
    maxWatermarkHeight / watermarkImage.height,
    1
  );

  const watermarkWidth = watermarkImage.width * scale;
  const watermarkHeight = watermarkImage.height * scale;
  const x = (canvas.width - watermarkWidth) / 2;
  const y = (canvas.height - watermarkHeight) / 2;

  ctx.globalAlpha = 0.4;
  ctx.drawImage(watermarkImage, x, y, watermarkWidth, watermarkHeight);
  ctx.globalAlpha = 1;

  const preferredType =
    file.type === "image/png"
      ? "image/png"
      : file.type === "image/webp"
      ? "image/webp"
      : "image/jpeg";

  const blob = await new Promise((resolve, reject) => {
    canvas.toBlob(
      (result) => {
        if (result) resolve(result);
        else reject(new Error("تعذر تجهيز الصورة بعد إضافة الشعار."));
      },
      preferredType,
      preferredType === "image/jpeg" ? 0.92 : 1
    );
  });

  const baseName = file.name?.replace(/\.[^/.]+$/, "") || "product";
  const extension =
    preferredType === "image/png"
      ? ".png"
      : preferredType === "image/webp"
      ? ".webp"
      : ".jpg";

  return new File([blob], `${baseName}_logo${extension}`, {
    type: preferredType,
  });
};

export default function AddProductDetails() {
  const navigate = useNavigate();
  const location = useLocation();
  const { t } = useI18n();

  // استلام productId من الصفحة السابقة
  const { productId } = location.state || {};
  if (!productId) {
    return (
      <div className="min-h-screen bg-[#F9F6EF] flex items-center justify-center">
        <div className="bg-[#F9F6EF] rounded-2xl shadow-xl p-8 text-center max-w-md border border-[#0A2C52]/20">
          <div className="bg-red-100 w-16 h-16 rounded-full flex items-center justify-center mx-auto mb-4">
            <FiPlus className="text-red-600" size={24} />
          </div>
          <h3 className="text-xl font-bold text-[#0A2C52] mb-2">
            {t("addProductDetails.noProductTitle", "خطأ في البيانات")}
          </h3>
          <p className="text-[#0A2C52]/80 mb-6">
            {t("addProductDetails.noProductDesc", "لا يوجد معرف للمنتج. الرجاء العودة والمحاولة مرة أخرى.")}
          </p>
          <button 
            onClick={() => navigate(-1)}
            className="bg-gradient-to-r from-[#0A2C52] to-[#0A2C52] hover:from-[#13345d] hover:to-[#0A2C52] text-white font-bold py-3 px-6 rounded-xl transition-all duration-300"
          >
            {t("general.back", "العودة")}
          </button>
        </div>
      </div>
    );
  }

  // حالات النموذج
  const [colorId, setColorId] = useState("");
  const [sizeId, setSizeId] = useState("");
  const [quantity, setQuantity] = useState(1);
  const [productImage, setProductImage] = useState("");
  const [selectedFile, setSelectedFile] = useState(null);
  const [message, setMessage] = useState("");
  const [messageType, setMessageType] = useState("");
  const [detailsAdded, setDetailsAdded] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [usePreviousImage, setUsePreviousImage] = useState(false);
  const [localPreviewUrl, setLocalPreviewUrl] = useState("");

  const fileInputRef = useRef(null);
  const baseButtonClasses =
    "bg-[#0A2C52] hover:bg-[#13345d] text-white font-bold rounded-xl transition-all duration-300 shadow-lg hover:shadow-xl disabled:bg-gray-400 disabled:text-white/80 disabled:cursor-not-allowed disabled:shadow-none flex items-center justify-center gap-2";
  
  useEffect(() => {
    window.scrollTo(0, 0);
  }, [message]);

  // دالة رفع الصورة إلى الخادم
  const handleImageUpload = async (file) => {
    const token = sessionStorage.getItem("token");
    const formData = new FormData();
    formData.append("imageFile", file);
    try {
      const response = await fetch(
        `${API_BASE_URL}Product/UploadProductImage`,
        {
          method: "POST",
          headers: {
            Authorization: `Bearer ${token}`,
          },
          body: formData,
        }
      );
      if (!response.ok) {
        throw new Error("فشل رفع الصورة");
      }
      const data = await response.json();
      return data.imageUrl;
    } catch (error) {
      console.error("Error uploading image:", error);
      return "";
    }
  };

  useEffect(() => {
    return () => {
      if (localPreviewUrl) {
        URL.revokeObjectURL(localPreviewUrl);
      }
    };
  }, [localPreviewUrl]);

  const handleFileChange = async (e) => {
    const file = e.target.files?.[0];
    if (!file) return;
    try {
      const stampedFile = await addWatermarkToFile(file);
      setSelectedFile(stampedFile);
      if (localPreviewUrl) {
        URL.revokeObjectURL(localPreviewUrl);
      }
      const preview = URL.createObjectURL(stampedFile);
      setLocalPreviewUrl(preview);
      setMessage(t("addProductDetails.messages.watermarkAdded", "تم إضافة شعار المتجر إلى الصورة تلقائياً قبل رفعها."));
      setMessageType("info");
    } catch (error) {
      console.error("Watermark error:", error);
      setSelectedFile(null);
      if (fileInputRef.current) {
        fileInputRef.current.value = null;
      }
      setMessage(error.message || t("addProductDetails.messages.watermarkFailed", "تعذر تجهيز الصورة. الرجاء المحاولة مرة أخرى."));
      setMessageType("error");
    }
  };

  // دالة إرسال تفاصيل المنتج
  const handleSubmit = async (e) => {
    e.preventDefault();

    setIsLoading(true);

    let uploadedImageUrl = productImage;
    if (selectedFile) {
      uploadedImageUrl = await handleImageUpload(selectedFile);
      if (!uploadedImageUrl) {
        setMessage(t("addProductDetails.messages.uploadFailed", "فشل رفع الصورة. الرجاء المحاولة مرة أخرى."));
        setMessageType("error");
        setIsLoading(false);
        return;
      }
      setProductImage(uploadedImageUrl);
    }
    
    const productDetails = {
      productDetailsId: 0,
      productId: productId,
      colorId: Number(colorId),
      sizeId: Number(sizeId),
      quantity: Number(quantity),
      productImage: uploadedImageUrl,
    };

    try {
      const token = sessionStorage.getItem("token");
      const response = await fetch(
        `${API_BASE_URL}Product/PostProductDetails`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify(productDetails),
        }
      );

      if (!response.ok) {
        throw new Error("فشل إضافة تفاصيل المنتج");
      }

      const data = await response.json();
      setMessage(t("addProductDetails.messages.addSuccess", "تمت الإضافة بنجاح ✓"));
      setMessageType("success");
      setDetailsAdded(true);
      
      // إعادة ضبط الحقول بعد الإضافة
      setColorId("");
      setSizeId("");
      setQuantity(1);
      setSelectedFile(null);
      if (localPreviewUrl) {
        URL.revokeObjectURL(localPreviewUrl);
        setLocalPreviewUrl("");
      }
      if (fileInputRef.current) {
        fileInputRef.current.value = null;
      }
    } catch (error) {
      setMessage(`${t("addProductDetails.messages.errorPrefix", "خطأ")}: ${error.message}`);
      setMessageType("error");
    } finally {
      setIsLoading(false);
    }
  };

  // زر "أضف تفاصيل أخرى" يعيد المستخدم إلى نفس النموذج
  const handleAddMore = () => {
    setDetailsAdded(false);
    setMessage("");
    setMessageType("");
    setSelectedFile(null);
    if (localPreviewUrl) {
      URL.revokeObjectURL(localPreviewUrl);
      setLocalPreviewUrl("");
    }
    if (fileInputRef.current) {
      fileInputRef.current.value = null;
    }
  };

  // زر "إنهاء" ينقل المستخدم إلى صفحة أخرى
  const handleFinish = () => {
    navigate(`/productDetails/${productId}`);
  };

  return (
    <div className="min-h-screen bg-[#F9F6EF] py-8 px-4">
      <div className="max-w-2xl mx-auto">
        {/* Header */}
        <div className="bg-[#F9F6EF] rounded-2xl shadow-xl p-6 mb-8 border border-[#0A2C52]/20">
          <div className="flex flex-col items-center mb-4">
            <WebSiteLogo width={300} height={100} className="mb-4" />
          </div>
          <div className="flex items-center justify-center">
            <div className="flex items-center gap-3">
              <div className="bg-[#F55A00] p-3 rounded-full">
                <FiPlus className="text-white" size={24} />
              </div>
              <div>
                <h1 className="text-2xl font-bold text-[#0A2C52] text-center">
                  {t("addProductDetails.headerTitle", "إضافة تفاصيل المنتج")}
                </h1>
                <p className="text-[#0A2C52]/80 text-sm mt-1 text-center">
                  {t("addProductDetails.headerSubtitle", "أضف الألوان والمقاسات والصور للمنتج")}
                </p>
              </div>
            </div>
          </div>
        </div>

        {/* Message Alert */}
        {message && (
          <div
            className={`rounded-2xl p-4 mb-6 shadow-lg ${
              messageType === "success"
                ? "bg-green-100 border border-green-200 text-green-800"
                : messageType === "info"
                ? "bg-blue-100 border border-blue-200 text-blue-800"
                : "bg-red-100 border border-red-200 text-red-800"
            }`}
          >
            <div className="flex items-center gap-2">
              {messageType === "success" ? (
                <FiCheck className="text-green-600" size={20} />
              ) : messageType === "info" ? (
                <FiInfo className="text-blue-600" size={20} />
              ) : (
                <div className="w-5 h-5 rounded-full bg-red-600 flex items-center justify-center">
                  <span className="text-white text-xs">!</span>
                </div>
              )}
              <span className="font-medium">{message}</span>
            </div>
          </div>
        )}

        {/* Loading Spinner */}
        {isLoading && (
          <div className="bg-[#F9F6EF] rounded-2xl shadow-lg p-8 text-center mb-6 border border-[#0A2C52]/20">
            <div className="flex flex-col items-center justify-center">
              <div className="w-16 h-16 border-4 border-[#F55A00] border-t-transparent rounded-full animate-spin mb-4"></div>
              <p className="text-[#0A2C52] font-semibold">
                {t("addProductDetails.loading", "جاري إضافة تفاصيل المنتج...")}
              </p>
            </div>
          </div>
        )}

        {/* Form */}
        {!isLoading && (
          <div className="bg-[#F9F6EF] rounded-2xl shadow-lg p-6 mb-6 border border-[#0A2C52]/20">
            <form onSubmit={handleSubmit}>
              {/* Color Selection */}
              <div className="mb-6">
                <label htmlFor="colorId" className="block text-lg font-bold text-[#0A2C52] mb-3">
                  {t("addProductDetails.fields.color", "اختر اللون")} <span className="text-red-500">*</span>
                </label>
                <select
                  id="colorId"
                  name="colorId"
                  value={colorId}
                  onChange={(e) => setColorId(e.target.value)}
                  required
                  className="w-full p-4 border-2 border-[#0A2C52]/30 rounded-xl focus:ring-2 focus:ring-[#F55A00] focus:border-[#F55A00] transition-all duration-200 bg-white"
                >
                  <option value="">{t("addProductDetails.placeholders.color", "اختر اللون من القائمة")}</option>
                  {colors.map((color) => (
                    <option key={color.ColorId} value={color.ColorId}>
                      {color.ColorName}
                    </option>
                  ))}
                </select>
              </div>

              {/* Size Selection */}
              <div className="mb-6">
                <label htmlFor="sizeId" className="block text-lg font-bold text-[#0A2C52] mb-3">
                  {t("addProductDetails.fields.size", "اختر المقاس")}{" "}
                  <span className="text-gray-500 text-sm">{t("general.optional", "(اختياري)")}</span>
                </label>
                <select
                  id="sizeId"
                  name="sizeId"
                  value={sizeId}
                  onChange={(e) => setSizeId(e.target.value)}
                  className="w-full p-4 border-2 border-[#0A2C52]/30 rounded-xl focus:ring-2 focus:ring-[#F55A00] focus:border-[#F55A00] transition-all duration-200 bg-white"
                >
                  <option value="">{t("addProductDetails.placeholders.size", "اختر المقاس من القائمة")}</option>
                  {sizes.map((size) => (
                    <option key={size.SizeId} value={size.SizeId}>
                      {size.SizeName}
                    </option>
                  ))}
                </select>
              </div>

              {/* Quantity Input */}
              <div className="mb-6">
                <label htmlFor="quantity" className="block text-lg font-bold text-[#0A2C52] mb-3">
                  {t("addProductDetails.fields.quantity", "الكمية المتاحة")} <span className="text-red-500">*</span>
                </label>
                <input
                  type="number"
                  id="quantity"
                  name="quantity"
                  value={quantity}
                  onChange={(e) => setQuantity(e.target.value)}
                  required
                  min="1"
                  className="w-full p-4 border-2 border-[#0A2C52]/30 rounded-xl focus:ring-2 focus:ring-[#F55A00] focus:border-[#F55A00] transition-all duration-200 bg-white"
                  placeholder={t("addProductDetails.placeholders.quantity", "أدخل الكمية المتاحة")}
                />
              </div>

              {/* Image Upload */}
              <div className="mb-6">
                <label htmlFor="productImage" className="block text-lg font-bold text-[#0A2C52] mb-3">
                  {t("addProductDetails.fields.image", "صورة المنتج")}
                </label>
                <div className="border-2 border-dashed border-[#0A2C52]/30 rounded-2xl p-6 text-center bg-white hover:bg-[#F9F6EF] transition-all duration-200">
                  <FiImage className="text-[#F55A00] mx-auto mb-3" size={32} />
                  <input
                    type="file"
                    id="productImage"
                    name="productImage"
                    accept="image/*"
                    onChange={handleFileChange}
                    ref={fileInputRef}
                    className="hidden"
                  />
                  <label 
                    htmlFor="productImage"
                    className={`cursor-pointer px-6 py-3 ${baseButtonClasses}`}
                  >
                    <FiUpload size={18} />
                    {t("addProductDetails.buttons.chooseImage", "اختر صورة")}
                  </label>
                  {selectedFile && (
                    <p className="text-green-600 font-medium mt-3">
                      ✓ {selectedFile.name}
                    </p>
                  )}
                </div>
              </div>

              {localPreviewUrl && (
                <div className="mb-6">
                  <label className="block text-lg font-bold text-[#0A2C52] mb-3">
                    {t("addProductDetails.preview.watermarkedTitle", "معاينة بعد إضافة شعار المتجر")}
                  </label>
                  <div className="bg-white rounded-2xl p-4 border-2 border-[#0A2C52]/20">
                    <img
                      src={localPreviewUrl}
                      alt="معاينة الصورة بعد إضافة الشعار"
                      className="w-full max-h-96 object-contain rounded-xl border border-[#0A2C52]/10"
                    />
                    <p className="text-sm text-[#0A2C52]/80 mt-2">
                      {t("addProductDetails.preview.watermarkedHint", "يتم حفظ الصورة مع شعار Gomango في المنتصف لحماية حقوقك عند تنزيلها.")}
                    </p>
                  </div>
                </div>
              )}

              {/* Previous Image Preview */}
              {productImage && (
                <div className="mb-6">
                  <label className="block text-lg font-bold text-[#0A2C52] mb-3">
                    {t("addProductDetails.preview.currentTitle", "معاينة الصورة الحالية")}
                  </label>
                  <div className="bg-white rounded-2xl p-4 border-2 border-[#0A2C52]/20">
                    <img
                      src={ServerPath + productImage}
                      alt="معاينة الصورة"
                      className="w-32 h-32 object-cover rounded-xl mx-auto border-2 border-[#0A2C52]/30"
                    />
                  </div>
                </div>
              )}

              {/* Use Previous Image Checkbox */}
              <div className="mb-6">
                <label className="flex items-center gap-3 cursor-pointer">
                  <div className="relative">
                    <input
                      type="checkbox"
                      id="usePreviousImage"
                      checked={usePreviousImage}
                      onChange={(e) => setUsePreviousImage(e.target.checked)}
                      className="sr-only"
                    />
                    <div className={`w-6 h-6 rounded border-2 transition-all duration-200 ${
                      usePreviousImage 
                        ? 'bg-[#F55A00] border-[#F55A00]' 
                        : 'bg-white border-[#0A2C52]/30'
                    }`}>
                      {usePreviousImage && (
                        <FiCheck className="text-white" size={16} />
                      )}
                    </div>
                  </div>
                  <span className="text-[#0A2C52] font-medium">
                    {t("addProductDetails.fields.usePreviousImage", "استخدام الصورة السابقة")}
                  </span>
                </label>
              </div>

              {/* Submit Button */}
              {!detailsAdded && (
                <button
                  type="submit"
                  disabled={isLoading}
                  className={`w-full py-4 px-6 transform hover:-translate-y-1 ${baseButtonClasses} ${isLoading ? "opacity-80 cursor-wait" : ""}`}
                >
                  <FiPlus size={20} />
                  {t("addProductDetails.buttons.submit", "إضافة تفاصيل المنتج")}
                </button>
              )}
            </form>
          </div>
        )}

        {/* Success Actions */}
        {detailsAdded && (
          <div className="bg-[#F9F6EF] rounded-2xl shadow-lg p-6 border border-[#0A2C52]/20">
            <div className="text-center mb-6">
              <div className="bg-green-100 w-16 h-16 rounded-full flex items-center justify-center mx-auto mb-4">
                <FiCheck className="text-green-600" size={32} />
              </div>
              <h3 className="text-xl font-bold text-green-800 mb-2">
                {t("addProductDetails.successTitle", "تمت الإضافة بنجاح!")}
              </h3>
              <p className="text-[#0A2C52]">
                {t("addProductDetails.successSubtitle", "يمكنك إضافة المزيد من التفاصيل أو إنهاء العملية")}
              </p>
            </div>
            
            <div className="flex flex-col sm:flex-row gap-4">
              <button
                onClick={handleAddMore}
                className={`flex-1 py-4 px-6 transform hover:-translate-y-1 ${baseButtonClasses}`}
              >
                <FiPlus size={20} />
                {t("addProductDetails.buttons.addMore", "أضف تفاصيل أخرى")}
              </button>
              <button
                onClick={handleFinish}
                className={`flex-1 py-4 px-6 transform hover:-translate-y-1 ${baseButtonClasses}`}
              >
                <FiArrowRight size={20} />
                {t("addProductDetails.buttons.finish", "إنهاء والعرض")}
              </button>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}