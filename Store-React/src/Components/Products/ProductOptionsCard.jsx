import React, { useState } from "react";
import BtnAddToCart from "../Cart/BtnAddToCart.jsx";
import { FiShoppingBag } from "react-icons/fi";

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
}) {
  const [banner, setBanner] = useState(null);
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
      <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-4 xl:gap-6">
        {/* اللون */}
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-3">
            {t("productDetails.color", "اللون")}
          </label>
          {Colors.length === 1 ? (
            <div className="p-3 bg-gray-50 rounded-lg text-gray-900">
              {translateColor(CurrentColor)}
            </div>
          ) : (
            <select
              value={CurrentColor}
              onChange={(e) => setCurrentColor(e.target.value)}
              className="w-full p-3 border border-gray-300 rounded-lg focus:border-orange-500 focus:ring-2 focus:ring-orange-200"
            >
              {Colors.map((color, index) => (
                <option key={index} value={color}>
                  {translateColor(color)}
                </option>
              ))}
            </select>
          )}
        </div>

        {/* المقاس */}
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
              <select
                value={CurrentSize}
                onChange={(e) => setCurrentSize(e.target.value)}
                className="w-full p-3 border border-gray-300 rounded-lg focus:border-orange-500 focus:ring-2 focus:ring-orange-200"
              >
                {Sizes.map((size, index) => (
                  <option key={index} value={size}>
                    {size}
                  </option>
                ))}
              </select>
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
      </div>

      {/* الأزرار */}
      {availableQuantity > 0 && (
        <div className="flex flex-col sm:flex-row gap-4">
          <button
            className="flex-1 flex items-center justify-center bg-green-600 hover:bg-green-700 text-white font-bold py-4 px-6 rounded-lg transition duration-300 shadow-lg"
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

