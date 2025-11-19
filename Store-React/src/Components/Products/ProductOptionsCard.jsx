import React, { useState } from "react";
import BtnAddToCart from "../Cart/BtnAddToCart.jsx";
import { FiShoppingBag } from "react-icons/fi";

// دالة لتحويل اسم اللون إلى لون hex
const getColorHex = (colorName) => {
  const colorMap = {
    "أحمر": "#FF0000",
    "أزرق": "#0000FF",
    "أخضر": "#008000",
    "أصفر": "#FFFF00",
    "أسود": "#000000",
    "أبيض": "#FFFFFF",
    "رمادي": "#808080",
    "برتقالي": "#FFA500",
    "بنفسجي": "#800080",
    "وردي": "#FFC0CB",
    "بني": "#A52A2A",
    "ذهبي": "#FFD700",
    "فضي": "#C0C0C0",
    "تركواز": "#40E0D0",
    "نيلي": "#4B0082",
    "كحلي": "#000080",
    "عنابي": "#800000",
    "بيج": "#F5F5DC",
    "خردلي": "#FFDB58",
    "فيروزي": "#30D5C8",
    "زهري": "#FF69B4",
    "أرجواني": "#FF00FF",
    "لافندر": "#E6E6FA",
    "موف": "#E0B0FF",
    "ليموني": "#32CD32",
    "أخضر زيتي": "#808000",
    "أخضر فاتح": "#90EE90",
    "أزرق سماوي": "#00CED1",
    "أزرق ملكي": "#4169E1",
    "قرمزي": "#DC143C",
    "red": "#FF0000",
    "blue": "#0000FF",
    "green": "#008000",
    "yellow": "#FFFF00",
    "black": "#000000",
    "white": "#FFFFFF",
    "gray": "#808080",
    "orange": "#FFA500",
    "purple": "#800080",
    "pink": "#FFC0CB",
    "brown": "#A52A2A",
    "gold": "#FFD700",
    "silver": "#C0C0C0",
    "teal": "#40E0D0",
    "navy": "#000080",
    "maroon": "#800000",
    "beige": "#F5F5DC",
    "khaki": "#FFDB58",
    "cyan": "#30D5C8",
    "magenta": "#FF00FF",
    "lime": "#32CD32",
    "olive": "#808000",
  };
  return colorMap[colorName] || "#CCCCCC";
};

export default function ProductOptionsCard({
  t,
  Colors,
  CurrentColor,
  setCurrentColor,
  Sizes,
  CurrentSize,
  setCurrentSize,
  Quantity,
  setQuantity,
  availableQuantity,
  handlBuyClick,
  DetailsId,
  translateColor,
  isRTL,
  showBanner,
  productId,
  onColorChange,
  onSizeChange,
}) {
  const [banner, setBanner] = useState(null);
  
  // دالة للتحقق من توفر اللون مع المقاس المحدد
  const isColorAvailable = (color) => {
    // إذا لم يكن هناك مقاس محدد، اللون متاح
    if (!CurrentSize || Sizes.length === 0) return true;
    // سيتم التحقق من التوفر في ProductDetails عند تغيير اللون
    return true;
  };

  // دالة للتحقق من توفر المقاس مع اللون المحدد
  const isSizeAvailable = (size) => {
    // إذا لم يكن هناك لون محدد، المقاس متاح
    if (!CurrentColor || Colors.length === 0) return true;
    // سيتم التحقق من التوفر في ProductDetails عند تغيير المقاس
    return true;
  };

  const handleColorClick = (color) => {
    if (isColorAvailable(color)) {
      setCurrentColor(color);
      if (onColorChange) {
        onColorChange(color);
      }
    } else {
      const message = t("productDetails.colorNotAvailable", "هذا اللون غير متوفر مع المقاس المحدد");
      if (showBanner) {
        showBanner(message, "error");
      } else {
        setBanner({ text: message, tone: "error" });
        setTimeout(() => setBanner(null), 4000);
      }
    }
  };

  const handleSizeClick = (size) => {
    if (isSizeAvailable(size)) {
      setCurrentSize(size);
      if (onSizeChange) {
        onSizeChange(size);
      }
    } else {
      const message = t("productDetails.sizeNotAvailable", "هذا المقاس غير متوفر مع اللون المحدد");
      if (showBanner) {
        showBanner(message, "error");
      } else {
        setBanner({ text: message, tone: "error" });
        setTimeout(() => setBanner(null), 4000);
      }
    }
  };

  return (
    <div className="bg-white rounded-2xl shadow-xl p-6 space-y-6 relative">
      {banner && (
        <div
          className={`fixed top-20 left-1/2 transform -translate-x-1/2 px-6 py-3 rounded-lg shadow-lg z-50 text-white ${
            banner.tone === "error" ? "bg-red-500" : "bg-emerald-600"
          }`}
        >
          {banner.text}
        </div>
      )}
      
      {/* الألوان */}
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-3">
          {t("productDetails.color", "اللون")}
        </label>
        {Colors.length === 1 ? (
          <div className="flex items-center gap-2 p-3 bg-gray-50 rounded-lg">
            <div 
              className="w-8 h-8 rounded border-2 border-gray-300"
              style={{ backgroundColor: getColorHex(CurrentColor) }}
            ></div>
            <span className="text-gray-900">{translateColor(CurrentColor)}</span>
          </div>
        ) : (
          <div className="flex flex-wrap gap-2">
            {Colors.map((color, index) => {
              const isSelected = CurrentColor === color;
              const isAvailable = isColorAvailable(color);
              return (
                <button
                  key={index}
                  type="button"
                  onClick={() => handleColorClick(color)}
                  disabled={!isAvailable}
                  className={`relative flex flex-col items-center justify-center w-16 h-16 rounded-lg border-2 transition-all duration-200 ${
                    isSelected
                      ? "border-[#0A2C52] shadow-lg scale-105"
                      : isAvailable
                      ? "border-gray-300 hover:border-[#0A2C52] hover:shadow-md"
                      : "border-gray-200 opacity-50 cursor-not-allowed"
                  }`}
                  title={translateColor(color)}
                >
                  <div
                    className="w-10 h-10 rounded border border-gray-200"
                    style={{ backgroundColor: getColorHex(color) }}
                  ></div>
                  <span className="text-xs mt-1 text-gray-700 font-medium truncate w-full text-center px-1">
                    {translateColor(color)}
                  </span>
                  {!isAvailable && (
                    <div className="absolute inset-0 flex items-center justify-center">
                      <span className="text-red-500 text-lg">✕</span>
                    </div>
                  )}
                </button>
              );
            })}
          </div>
        )}
      </div>

      {/* المقاسات */}
      {Sizes.length > 0 && Sizes[0] !== null && (
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-3">
            {t("productDetails.size", "المقاس")}
          </label>
          {Sizes.length === 1 ? (
            <div className="p-3 bg-gray-50 rounded-lg text-gray-900">
              {CurrentSize}
            </div>
          ) : (
            <div className="flex flex-wrap gap-2">
              {Sizes.map((size, index) => {
                const isSelected = CurrentSize === size;
                const isAvailable = isSizeAvailable(size);
                return (
                  <button
                    key={index}
                    type="button"
                    onClick={() => handleSizeClick(size)}
                    disabled={!isAvailable}
                    className={`px-4 py-2 rounded-lg border-2 transition-all duration-200 font-medium group ${
                      isSelected
                        ? "border-[#0A2C52] bg-[#0A2C52] shadow-lg scale-105"
                        : isAvailable
                        ? "border-gray-300 hover:border-[#0A2C52] hover:bg-[#0A2C52] hover:shadow-md"
                        : "border-gray-200 bg-gray-100 opacity-50 cursor-not-allowed"
                    }`}
                    style={!isSelected && isAvailable ? { backgroundColor: '#FFFFFF' } : {}}
                    title={size}
                  >
                    {isSelected ? (
                      <span style={{ color: '#FFFFFF' }}>{size}</span>
                    ) : isAvailable ? (
                      <span className="group-hover:text-white" style={{ color: '#000000' }}>{size}</span>
                    ) : (
                      <span style={{ color: '#6B7280' }}>{size}</span>
                    )}
                    {!isAvailable && (
                      <span className="ml-1 text-red-500">✕</span>
                    )}
                  </button>
                );
              })}
            </div>
          )}
        </div>
      )}

      {/* الكمية */}
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-3">
          {t("productDetails.quantity", "الكمية")}
        </label>
        <div className="flex items-center gap-4">
          <button
            onClick={() => setQuantity(Math.max(1, Quantity - 1))}
            className="w-12 h-12 flex items-center justify-center bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors border border-gray-300"
          >
            <span className="text-xl font-bold">-</span>
          </button>
          <input
            type="number"
            min="1"
            max={availableQuantity}
            value={Quantity}
            onChange={(e) => {
              const newValue = Number(e.target.value);
              if (newValue > availableQuantity) {
                const message = t("productDetails.maxQuantityReached", "لا توجد كمية أخرى متاحة");
                if (showBanner) {
                  showBanner(message, "error");
                } else {
                  setBanner({ text: message, tone: "error" });
                  setTimeout(() => setBanner(null), 4000);
                }
                setQuantity(availableQuantity);
                return;
              }
              setQuantity(
                Math.min(
                  Math.max(1, newValue),
                  availableQuantity
                )
              );
            }}
            className="w-20 p-3 border border-gray-300 rounded-lg text-center text-black focus:border-orange-500 focus:ring-2 focus:ring-orange-200"
          />
          <button
            onClick={() => {
              if (Quantity >= availableQuantity) {
                const message = t("productDetails.maxQuantityReached", "لا توجد كمية أخرى متاحة");
                if (showBanner) {
                  showBanner(message, "error");
                } else {
                  setBanner({ text: message, tone: "error" });
                  setTimeout(() => setBanner(null), 4000);
                }
                return;
              }
              setQuantity(Math.min(availableQuantity, Quantity + 1));
            }}
            className="w-12 h-12 flex items-center justify-center bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors border border-gray-300"
          >
            <span className="text-xl font-bold">+</span>
          </button>
        </div>
      </div>

      {/* الأزرار */}
      {availableQuantity > 0 && (
        <div className="flex flex-col sm:flex-row gap-4">
          <button
            className="flex-1 flex items-center justify-center bg-[#0A2C52] hover:bg-[#13345d] text-white font-bold py-4 px-6 rounded-lg transition duration-300 shadow-lg"
            onClick={handlBuyClick}
          >
            <FiShoppingBag className={isRTL ? "ml-2" : "mr-2"} />
            {t("productDetails.buyNow", "شراء الآن")}
          </button>

          <BtnAddToCart
            className="flex-1 bg-orange-500 hover:bg-orange-600 text-white font-bold py-4 px-6 rounded-lg transition duration-300 shadow-lg flex items-center justify-center"
            productDetailsId={DetailsId}
            Quantity={Quantity}
          />
        </div>
      )}
    </div>
  );
}
