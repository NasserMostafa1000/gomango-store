import React, { useState, useEffect, useMemo } from "react";
import { Helmet } from "react-helmet";
import { useLocation, useNavigate } from "react-router-dom";
import getDeliveryDate, {
  playNotificationSound,
  SendSignalMessageForOrders,
  startConnection,
} from "../utils.js";
import AddressSelector from "./AddressSelector.jsx";
import PhoneNumberModal from "./PhoneModel.jsx";
import OrderSummary from "./PurchaseSummray.jsx";
import {
  fetchAddresses,
  fetchShipOrderInfo,
  fetchClientPhone,
  postOrder,
  postOrderDetails,
  PostListOfOrdersDetails,
} from "./api.js";
import SuccessForm from "./SuccessForm.jsx";
import API_BASE_URL, { SiteName } from "../Constant.js";
import { useCurrency } from "../Currency/CurrencyContext";
import CurrencySelector from "../Currency/CurrencySelector";
import { useI18n } from "../i18n/I18nContext";

export default function PurchaseOperationDetails() {
  const [ShipPrice, SetShiPrice] = useState(0);
  const [addresses, setAddresses] = useState({});
  const [selectedAddressId, setSelectedAddressId] = useState("");
  const [showAddAddressModal, setShowAddAddressModal] = useState(false);
  const [clientPhone, setClientPhone] = useState("");
  const [showPhoneModal, setShowPhoneModal] = useState(false);
  const [newPhoneNumber, setNewPhoneNumber] = useState("");
  const [newAddress, setNewAddress] = useState({
    governorate: "أبوظبي",
    city: "",
    street: "",
  });
  const [loading, setLoading] = useState(true);
  const [purchaseLoading, setPurchaseLoading] = useState(false);
  const [message, setMessage] = useState("");
  const [showSuccessForm, setShowSuccessForm] = useState(false);
  const [paymentMethod, setPaymentMethod] = useState("cod");

  const navigate = useNavigate();
  const location = useLocation();
  const { format, convertFromAED } = useCurrency();
  const { t, lang } = useI18n();

  const fromCart = Boolean(location.state?.fromCart);
  const Products = location.state?.Product;
  const normalizedProducts = useMemo(() => {
    if (!Products) return [];
    return Array.isArray(Products) ? Products : [Products];
  }, [Products]);
  const productPrice = useMemo(() => {
    return Array.isArray(Products)
    ? Products.reduce((sum, p) => sum + p.unitPrice * p.quantity, 0)
    : Products.unitPrice * Products.quantity;
  }, [Products]);

  // إذا كان سعر المنتج أعلى من أو يساوي 200 درهم، الشحن مجاني
  const actualShipPrice = useMemo(() => {
    return productPrice >= 200 ? 0 : ShipPrice;
  }, [productPrice, ShipPrice]);
  
  // ضريبة الدفع عند الاستلام
  const codTax = 10;

  const finalPrice = useMemo(() => {
    return productPrice + actualShipPrice + (paymentMethod === "cod" ? codTax : 0);
  }, [productPrice, actualShipPrice, codTax, paymentMethod]);

  useEffect(() => {
    const _fetchAddresses = async () => {
      try {
        const token = sessionStorage.getItem("token");
        const Jsonresponse = await fetchAddresses(token);
        const fetchedAddresses = Jsonresponse.addresses;
        if (Object.keys(fetchedAddresses).length > 0) {
          setAddresses(fetchedAddresses);
          setSelectedAddressId(Object.keys(fetchedAddresses)[0]);
        }
      } catch (error) {
        console.error("Error fetching addresses:", error.message);
      }
    };
    _fetchAddresses();
  }, []);

  useEffect(() => {
    window.scrollTo(0, 0);
  }, [message]);

  useEffect(() => {
    const _fetchShipOrderInfo = async () => {
      try {
        const token = sessionStorage.getItem("token");
        if (!addresses[selectedAddressId]) return;
        const Governorate = addresses[selectedAddressId].split("-")[0];
        const JsonResponse = await fetchShipOrderInfo(token, Governorate);
        SetShiPrice(JsonResponse.shipPrice);
        setShipPriceBeforeDiscount(JsonResponse.shipPrice);
      } catch (error) {
        console.error(error.message);
      }
    };
    _fetchShipOrderInfo();
  }, [selectedAddressId, addresses]);

  useEffect(() => {
    const _fetchClientPhone = async () => {
      try {
        const token = sessionStorage.getItem("token");
        const JsonResponse = await fetchClientPhone(token);
        setClientPhone(JsonResponse.phoneNumber);
      } catch (error) {
        setShowPhoneModal(true);
      } finally {
        setLoading(false);
      }
    };
    _fetchClientPhone();
  }, []);

  function CreateOrderDetails(OrderId) {
    return {
      productDetailsId: Products.productDetailsId,
      quantity: Products.quantity,
      unitPrice: Products.unitPrice,
      orderId: OrderId,
    };
  }

  const handleStripeCheckout = async () => {
    if (!selectedAddressId || !addresses[selectedAddressId]) {
      setMessage(t("orderSummary.selectAddressForFinal", "يرجى اختيار العنوان لحساب السعر النهائي"));
      return;
    }

    const token = sessionStorage.getItem("token");
    if (!token) {
      setMessage(t("productDetails.loginRequired", "يجب تسجيل الدخول لمتابعة عملية الشراء."));
      navigate("/Login", { state: { path: "/PurchaseDetails" } });
      return;
    }

    if (!normalizedProducts.length) {
      setMessage(t("payments.notFound", "لم يتم العثور على الطلب، يرجى التواصل مع الدعم."));
      return;
    }

    setPurchaseLoading(true);
    try {
      const response = await fetch(`${API_BASE_URL}Payments/CreateStripeCheckout`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          address: addresses[selectedAddressId],
          totalPrice: Number((productPrice + actualShipPrice).toFixed(2)),
          ShippingCoast: actualShipPrice,
          paymentMethodId: 1,
          currency: "aed",
          fromCart,
          discountCode: discountApplied && discountCode ? discountCode : null,
          products: normalizedProducts.map((item) => ({
            productDetailsId: item.productDetailsId,
            quantity: item.quantity,
            unitPrice: item.unitPrice,
          })),
        }),
      });

      if (!response.ok) {
        const errorData = await response.json().catch(() => ({}));
        throw new Error(errorData.message || "فشل تجهيز جلسة الدفع الإلكتروني");
      }

      const data = await response.json();
      if (data.checkoutUrl) {
        window.location.href = data.checkoutUrl;
      } else {
        throw new Error("رابط الدفع غير متاح حالياً.");
      }
    } catch (error) {
      setMessage(
        `${t("purchaseDetails.errorMessage", "❌ حدث خطأ أثناء إتمام الطلب. الرجاء المحاولة مرة أخرى.")} ${error.message || ""}`
      );
    } finally {
      setPurchaseLoading(false);
    }
  };

  async function HandleBuyClick() {
    if (paymentMethod === "online") {
      await handleStripeCheckout();
      return;
    }

    if (!selectedAddressId || !addresses[selectedAddressId]) {
      setMessage(t("orderSummary.selectAddressForFinal", "يرجى اختيار العنوان لحساب السعر النهائي"));
      return;
    }

    setPurchaseLoading(true);
    const token = sessionStorage.getItem("token");

    const orderData = {
      address: addresses[selectedAddressId],
      totalPrice: finalPrice,
      ShippingCoast: actualShipPrice,
      paymentMethodId: 2,
      transactionNumber: "",
    };

    try {
      const OrderId = await postOrder(token, orderData);
      await startConnection();
      await SendSignalMessageForOrders("new Order" + OrderId);

      if (Array.isArray(Products) && Products.length > 1) {
        await PostListOfOrdersDetails(OrderId, token, Products);
      } else {
        const orderDetails = CreateOrderDetails(OrderId);
        if (orderDetails) {
          await postOrderDetails(token, OrderId, orderDetails);
        }
      }
      playNotificationSound();
      setMessage(
        t("purchaseDetails.successMessage", "✅ تم الطلب بنجاح! يمكنك متابعة طلبك في قسم طلباتي، ولأي خدمة أخرى يمكنك التواصل مع الدعم الفني من خلال قسم تواصل معنا")
      );
      
      setTimeout(() => {
        setShowSuccessForm(true);
      }, 100);
    } catch (error) {
      console.error("❌ خطأ أثناء إتمام الطلب:", error);
      setMessage(t("purchaseDetails.errorMessage", "❌ حدث خطأ أثناء إتمام الطلب. الرجاء المحاولة مرة أخرى."));
    } finally {
      setPurchaseLoading(false);
    }
  }
  const [discountCode, setDiscountCode] = useState("");
  const [discountMessage, setDiscountMessage] = useState("");
  const [isDiscountValid, setIsDiscountValid] = useState(false);
  const [discountApplied, setDiscountApplied] = useState(false);
  const [shipPriceBeforeDiscount, setShipPriceBeforeDiscount] =
    useState(ShipPrice);

  const handleCheckDiscountCode = async () => {
    if (!discountCode.trim()) {
      setDiscountMessage(t("purchaseDetails.enterDiscount", "يرجى إدخال كود الخصم"));
      setIsDiscountValid(false);
      return;
    }

    try {
      const response = await fetch(
        `${API_BASE_URL}ShippingDiscountsCodes/verify-code`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(discountCode.trim()),
        }
      );

      if (response.ok) {
        setIsDiscountValid(true);
        setDiscountMessage(t("purchaseDetails.discountActivated", "تم تفعيل الكود وتم استخدامه بنجاح، الشحن مجاني!"));
        setDiscountApplied(true);
        SetShiPrice(0);
        alert(t("purchaseDetails.discountActivatedAlert", "تم تفعيل كود الخصم بنجاح. الشحن أصبح مجاني."));
      } else {
        const errorData = await response.json();
        setIsDiscountValid(false);
        setDiscountMessage(errorData.message);
        setDiscountApplied(false);
        SetShiPrice(shipPriceBeforeDiscount);
      }
    } catch (error) {
      setIsDiscountValid(false);
      setDiscountMessage(error.message);
      setDiscountApplied(false);
      SetShiPrice(shipPriceBeforeDiscount);
    }
  };

  if (loading) return <div className="flex justify-center items-center min-h-screen bg-blue-900 text-white">{t("purchaseDetails.loading", "جاري التحميل...")}</div>;

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-900 to-blue-800 py-8 px-4" dir={lang === "ar" ? "rtl" : "ltr"}>
      <Helmet>
        <title>{t("purchaseDetails.metaTitle", "تفاصيل الطلب")} | {SiteName} </title>
        <meta
          name="description"
          content={t("purchaseDetails.metaDesc", "تفاصيل الطلب في موقع تابع طلبك وتواصل مع الدعم الفني.")}
        />
      </Helmet>

      {showSuccessForm && (
        <SuccessForm
          message={message}
          onClose={() => {
            setShowSuccessForm(false);
          }}
          discountCode={null}
          showDiscountCode={false}
        />
      )}

      {message && !showSuccessForm && (
        <div
          className={`max-w-4xl mx-auto mb-6 p-4 rounded-lg text-white text-center font-bold ${
            message.startsWith("✅") ? "bg-green-600" : "bg-red-600"
          }`}
        >
          {message}
        </div>
      )}

      <div className="max-w-4xl mx-auto bg-white rounded-2xl shadow-2xl overflow-hidden">
        <div className="bg-orange-500 py-4 px-6">
          <h1 className="text-2xl font-bold text-white text-center">{t("purchaseDetails.title", "تفاصيل الطلب")}</h1>
        </div>

        <div className="p-6 space-y-6">
          <AddressSelector
            addresses={addresses}
            selectedAddressId={selectedAddressId}
            setSelectedAddressId={setSelectedAddressId}
            setShowAddAddressModal={setShowAddAddressModal}
            showAddAddressModal={showAddAddressModal}
            newAddress={newAddress}
            setNewAddress={setNewAddress}
            setAddresses={setAddresses}
          />

          <div className="flex justify-between items-center bg-blue-50 p-4 rounded-lg border border-blue-200">
            <span className="text-blue-900 font-semibold">{t("purchaseDetails.yourPhone", "هاتفك للاتصال")}:</span>
            <button
              onClick={() => setShowPhoneModal(true)}
              className="text-orange-600 hover:text-orange-700 font-medium underline"
            >
              {clientPhone}
            </button>
          </div>

          <div className="bg-blue-900 py-3 px-4 rounded-lg flex items-center justify-between">
            <h3 className="text-xl font-bold text-white">{t("purchaseDetails.shippingDetails", "تفاصيل الشحنة")}</h3>
            <CurrencySelector />
          </div>
          
          <OrderSummary 
            Products={Products} 
            ShipPrice={actualShipPrice} 
            isFreeShipping={productPrice >= 200 && actualShipPrice === 0}
          />

          <div className="flex justify-between items-center bg-blue-50 p-4 rounded-lg border border-blue-200">
            <span className="text-blue-900 font-semibold">{t("purchaseDetails.shipTo", "شحن إلى")}:</span>
            <span className="text-blue-800 font-medium">
              {addresses[selectedAddressId] || t("orderSummary.selectAddress", "يرجى اختيار العنوان أولاً")}
            </span>
          </div>



          <div className="space-y-2 bg-blue-50 p-4 rounded-lg border border-blue-200">
            <div className="flex justify-between items-center">
              <span className="text-blue-900 font-semibold">
                {t("purchaseDetails.deliveryDeadline", "الموعد النهائي للاستلام")}:
              </span>
              <span className="text-blue-800 font-medium">
                {getDeliveryDate(lang === "en" ? "en" : "ar")}
              </span>
            </div>
            <p className="text-sm text-blue-700">
              {t(
                "purchaseDetails.deliveryTime",
                "سيصل الطلب خلال ساعات قليلة أو ربما دقائق حسب وزن الشحنة"
              )}
            </p>
          </div>

          <div className="bg-orange-50 border border-orange-200 p-4 rounded-lg space-y-3">
            <label htmlFor="discountCode" className="block text-orange-800 font-semibold">
              {t("purchaseDetails.discountCode", "كود الخصم")}:
            </label>
            <input
              type="text"
              id="discountCode"
              value={discountCode}
              onChange={(e) => setDiscountCode(e.target.value)}
              placeholder={t("purchaseDetails.enterDiscountCode", "أدخل كود الخصم هنا")}
              className="w-full p-3 border border-orange-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-orange-500"
            />
            <p className="text-red-500 text-sm">
              {t("purchaseDetails.discountWarning", "لا تضغط علي تحقق ان كنت لن تشتري لانه سيتم تفعيل الخصم والغاء صلاحيه الكود")}
            </p>
            {!discountApplied && (
              <button 
                onClick={handleCheckDiscountCode}
                className="bg-orange-500 hover:bg-orange-600 text-white font-bold py-2 px-6 rounded-lg transition duration-200"
              >
                {t("purchaseDetails.verify", "تحقق")}
              </button>
            )}
            {discountMessage && (
              <div
                className={`p-3 rounded-lg font-medium ${
                  isDiscountValid ? "bg-green-100 text-green-800" : "bg-red-100 text-red-800"
                }`}
              >
                {discountMessage}
              </div>
            )}
          </div>

          <div className="space-y-4">
            <div className="bg-blue-900 py-3 px-4 rounded-lg">
              <h3 className="text-xl font-bold text-white">{t("purchaseDetails.paymentMethods", "طرق الدفع")}</h3>
            </div>
            
            <div className="space-y-3">
              <label className={`flex items-center p-3 border border-blue-200 rounded-lg hover:bg-blue-50 cursor-pointer ${lang === "ar" ? "space-x-3 space-x-reverse" : "space-x-3"}`}>
                <input
                  type="radio"
                  value="online"
                  checked={paymentMethod === "online"}
                  onChange={() => setPaymentMethod("online")}
                  className="text-orange-500 focus:ring-orange-500"
                />
                <span className="text-blue-900 font-medium">{t("purchaseDetails.onlinePayment", "الدفع الإلكتروني")}</span>
              </label>
              {paymentMethod === "online" && (
                <div className="bg-blue-50 p-4 rounded-lg border border-blue-200 space-y-3">
                  <div className="flex justify-between items-center bg-white p-3 rounded-lg">
                    <span className="text-blue-900 font-semibold">{t("purchaseDetails.finalPrice", "السعر النهائي")}:</span>
                    <strong className="text-orange-600 text-lg">{format(productPrice + actualShipPrice)}</strong>
                  </div>
                  <p className="text-blue-700 text-sm">
                    {t(
                      "purchaseDetails.noteText",
                      "يجب تحويل المبلغ المستحق بالكامل إلى هذا الرقم قبل الضغط على زر شراء"
                    )}
                  </p>
                </div>
              )}

              <label className={`flex items-center p-3 border border-blue-200 rounded-lg hover:bg-blue-50 cursor-pointer ${lang === "ar" ? "space-x-3 space-x-reverse" : "space-x-3"}`}>
                <input
                  type="radio"
                  value="cod"
                  checked={paymentMethod === "cod"}
                  onChange={() => setPaymentMethod("cod")}
                  className="text-orange-500 focus:ring-orange-500"
                />
                <span className="text-blue-900 font-medium">{t("purchaseDetails.codPayment", "الدفع عند الاستلام")}</span>
              </label>
              {paymentMethod === "cod" && (
                <div className={`bg-blue-50 p-4 rounded-lg border border-blue-200 space-y-3 ${lang === "ar" ? "space-x-3 space-x-reverse" : "space-x-3"}`}>
                  <div className="flex items-center gap-3">
                    <img
                      src="/Icons/الدفع-عند-الاستلام.ico"
                      alt="Cash on Delivery"
                      title="Cash on Delivery"
                      className="w-12 h-12"
                    />
                    <div className="flex justify-between items-center bg-white p-3 rounded-lg flex-1">
                      <span className="text-blue-900 font-semibold">{t("purchaseDetails.codTax", "ضريبة الدفع عند الاستلام")}:</span>
                      <strong className="text-orange-600">{format(codTax)}</strong>
                    </div>
                  </div>
                </div>
              )}
            </div>
          </div>

          <div className="flex justify-between items-center p-4 rounded-lg text-white" style={{ background: 'linear-gradient(to right, #f97316, #ea580c)' }}>
            <span className="text-lg font-bold" style={{ color: 'white' }}>{t("purchaseDetails.finalPrice", "السعر النهائي")}:</span>
            <strong className="text-2xl" style={{ color: 'white' }}>{format(finalPrice)}</strong>
          </div>

          {showPhoneModal && (
            <PhoneNumberModal
              setShowPhoneModal={setShowPhoneModal}
              setClientPhone={setClientPhone}
              newPhoneNumber={newPhoneNumber}
              setNewPhoneNumber={setNewPhoneNumber}
            />
          )}

          <button
            className={`w-full text-white font-bold py-4 px-6 rounded-lg text-lg transition duration-200 disabled:opacity-50 disabled:cursor-not-allowed ${
              paymentMethod === "online"
                ? "bg-[#0A2C52] hover:bg-[#13345d]"
                : "bg-gradient-to-r from-orange-500 to-orange-600 hover:from-orange-600 hover:to-orange-700"
            }`}
            onClick={HandleBuyClick}
            disabled={purchaseLoading}
            style={{ color: 'white' }}
            type="button"
          >
            <span className="text-white" style={{ color: 'white' }}>
              {purchaseLoading
                ? t("purchaseDetails.processing", "جاري المعالجة...")
                : paymentMethod === "online"
                ? t("purchaseDetails.buyNow", "شراء")
                : t("purchaseDetails.completePurchase", "اتمام الشراء")}
            </span>
          </button>

          {purchaseLoading && (
            <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
              <div className={`bg-white p-6 rounded-lg flex items-center ${lang === "ar" ? "space-x-3 space-x-reverse" : "space-x-3"}`}>
                <div className="w-8 h-8 border-4 border-orange-500 border-t-transparent rounded-full animate-spin"></div>
                <span className="text-blue-900 font-medium">{t("purchaseDetails.processingOrder", "جاري معالجة طلبك...")}</span>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}