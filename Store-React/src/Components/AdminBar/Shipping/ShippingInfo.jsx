import React, { useEffect, useState } from "react";
import API_BASE_URL from "../../Constant";
import { useI18n } from "../../i18n/I18nContext";

const ShippingInfo = () => {
  const [shippingData, setShippingData] = useState([]);
  const [selectedGovernorate, setSelectedGovernorate] = useState("");
  const [newPrice, setNewPrice] = useState("");
  const { t } = useI18n();
  const token = typeof window !== "undefined" ? sessionStorage.getItem("token") : null;

  // جلب بيانات الشحن من API باستخدام fetch
  useEffect(() => {
    const fetchShippingData = async () => {
      try {
        const token = sessionStorage.getItem("token"); // جلب التوكن من التخزين المحلي

        const response = await fetch(
          `${API_BASE_URL}ShippingInfo/GetShippingInfo`,
          {
            method: "GET",
            headers: {
              "Content-Type": "application/json",
              Authorization: `Bearer ${token}`, // ⬅️ إرسال التوكن هنا
            },
          }
        );

        if (!response.ok) throw new Error("فشل جلب البيانات");

        const data = await response.json();
        setShippingData(data);
      } catch (error) {
        console.error("حدث خطأ أثناء جلب البيانات:", error);
      }
    };

    fetchShippingData();
  }, []);

  // تحديث السعر باستخدام fetch
  const handleUpdatePrice = async () => {
    if (!selectedGovernorate || newPrice.trim() === "" || isNaN(newPrice)) {
      alert("يرجى اختيار المحافظة وإدخال سعر صالح!");
      return;
    }

    const priceValue = parseFloat(newPrice);

    if (priceValue <= 0) {
      alert("يجب أن يكون السعر أكبر من 0!");
      return;
    }

    const token = sessionStorage.getItem("token"); // جلب التوكن من التخزين المحلي

    try {
      const response = await fetch(
        `${API_BASE_URL}ShippingInfo/UpdateShippingPrice/?Governorate=${selectedGovernorate}&NewPrice=${Number(
          priceValue
        )}`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json; charset=utf-8",
            Authorization: `Bearer ${token}`, // ⬅️ إرسال التوكن هنا
          },
        }
      );

      const responseData = await response.json();
      console.log("✅ استجابة API:", responseData);

      if (!response.ok) throw new Error("فشل تحديث السعر");

      // تحديث البيانات بعد التعديل
      setShippingData((prevData) =>
        prevData.map((item) =>
          item.governorate === selectedGovernorate
            ? { ...item, price: priceValue }
            : item
        )
      );

      alert("تم تحديث السعر بنجاح!");
    } catch (error) {
      console.error("❌ خطأ أثناء تحديث السعر:", error);
      alert("فشل التحديث، حاول مرة أخرى!");
    }
  };

  const handleResetToEmirates = async () => {
    try {
      const res = await fetch(`${API_BASE_URL}ShippingInfo/ResetToEmirates`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: token ? `Bearer ${token}` : undefined,
        },
      });
      if (!res.ok) throw new Error("reset failed");
      // reload list
      const response = await fetch(`${API_BASE_URL}ShippingInfo/GetShippingInfo`, {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: token ? `Bearer ${token}` : undefined,
        },
      });
      const data = await response.json();
      setShippingData(data);
      alert("تم ضبط الإمارات السبع بنجاح");
    } catch {
      alert("فشل إعادة الضبط");
    }
  };

  return (
    <div className="max-w-5xl mx-auto p-3 md:p-6">
      <div className="rounded-2xl p-4 md:p-5 shadow-lg border mb-5" style={{ background: 'linear-gradient(to right, #ff7a00, #ea580c)' }}>
        <h2 className="text-2xl md:text-3xl font-extrabold text-white tracking-wide">🛒 {t("shippingPrices", "أسعار الشحن")}</h2>
      </div>
      
      <div className="bg-white rounded-2xl shadow p-4 md:p-6 border">
        <div className="flex items-center justify-between mb-3 gap-3 flex-wrap">
          <h3 className="text-lg font-semibold">✏️ {t("updatePrice", "تحديث السعر")}</h3>
          <button onClick={handleResetToEmirates} className="px-4 py-2 rounded-xl bg-orange-600 hover:bg-orange-700 text-white font-semibold shadow">
            {t("resetToEmirates", "إعادة ضبط الإمارات السبع")}
          </button>
        </div>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-3">
          <select
            value={selectedGovernorate}
            onChange={(e) => setSelectedGovernorate(e.target.value)}
            className="border rounded-xl px-3 py-2"
          >
            <option value="">{t("selectGovernorate", "اختر الإمارة")}</option>
            {shippingData.map((item) => (
              <option key={item.id} value={item.governorate}>
                {item.governorate}
              </option>
            ))}
          </select>
          <input
            type="number"
            placeholder={t("enterNewPrice", "أدخل السعر الجديد")}
            value={newPrice}
            onChange={(e) => setNewPrice(e.target.value)}
            className="border rounded-xl px-3 py-2"
          />
          <button onClick={handleUpdatePrice} className="bg-[#0a2540] hover:bg-[#13345d] text-white rounded-xl px-4 py-2 font-semibold shadow">
            {t("updatePrice", "تحديث السعر")}
          </button>
        </div>
      </div>

      <div className="mt-6 bg-white rounded-2xl shadow overflow-hidden border">
        <table className="min-w-full">
          <thead className="bg-gray-50">
            <tr>
              <th className="text-right p-3">{t("price", "السعر")}</th>
              <th className="text-right p-3">{t("governorate", "الإمارة")}</th>
            </tr>
          </thead>
          <tbody>
            {shippingData.map((item) => (
              <tr key={item.id} className="border-t hover:bg-gray-50/60">
                <td className="p-3">{item.price}</td>
                <td className="p-3">{item.governorate}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default ShippingInfo;
