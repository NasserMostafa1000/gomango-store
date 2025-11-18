import React, { useState, useEffect } from "react";
import { FiTrash2, FiShoppingCart, FiPlus, FiMinus } from "react-icons/fi";
import API_BASE_URL, { ServerPath, SiteName } from "../Constant.js";
import { useNavigate } from "react-router-dom";
import { Helmet } from "react-helmet";
import { useCurrency } from "../Currency/CurrencyContext";
import CurrencySelector from "../Currency/CurrencySelector";
import { useI18n } from "../i18n/I18nContext";
import { getOrCreateSessionId, mergeGuestCartToUserCart } from "../utils";
import BackButton from "../Common/BackButton";

export default function Cart() {
  const [cartItems, setCartItems] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const navigate = useNavigate();
  const { format } = useCurrency();
  const { t } = useI18n();
  
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

  // دالة إنشاء كائن الطلب من عناصر السلة
  function CreateOrdersObj() {
    if (cartItems.length === 1) {
      const item = {
        productDetailsId: cartItems[0].productDetailsId,
        quantity: cartItems[0].quantity,
        unitPrice: cartItems[0].unitPriceAfterDiscount,
        orderId: 0,
      };
      return item;
    }
    const products = [];
    for (let i = 0; i < cartItems.length; i++) {
      products.push({
        productDetailsId: cartItems[i].productDetailsId,
        quantity: cartItems[i].quantity,
        unitPrice: cartItems[i].unitPriceAfterDiscount,
        orderId: 0,
      });
    }
    return products;
  }

  // دالة الانتقال إلى صفحة تفاصيل الشراء
  async function handleBuyAll() {
    const token = sessionStorage.getItem("token");
    if (!token) {
      // إذا لم يكن مسجل دخول، توجهه لتسجيل الدخول
      navigate("/login");
      return;
    }
    
    setIsLoading(true);
    const Product = CreateOrdersObj();
    Product.totalPrice = totalCartPrice;
    navigate("/PurchaseDetails", { state: { Product, fromCart: true } });
    setIsLoading(false);
  }

  // جلب تفاصيل السلة عند تحميل المكون
  useEffect(() => {
    const token = sessionStorage.getItem("token");
    const fetchCartDetails = async () => {
      try {
        let response;
        
        if (token) {
          // المستخدم مسجل دخول - جلب السلة العادية
          response = await fetch(`${API_BASE_URL}Carts/GetCartDetails`, {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
        });
        } else {
          // المستخدم غير مسجل - جلب السلة المؤقتة
          const sessionId = getOrCreateSessionId();
          response = await fetch(`${API_BASE_URL}Carts/GetGuestCartDetails`, {
            method: "GET",
            headers: {
              "Content-Type": "application/json",
              sessionId: sessionId,
            },
          });
        }
        
        if (!response.ok) {
          throw new Error("Network response was not ok");
        }
        const data = await response.json();
        setCartItems(data || []);
      } catch (error) {
        console.error("Error fetching cart details:", error.message);
        setCartItems([]);
      }
    };
    
    fetchCartDetails();
    
    // إذا كان المستخدم مسجل دخول ولديه sessionId، قم بدمج السلة المؤقتة
    if (token) {
      const sessionId = localStorage.getItem("guestSessionId");
      if (sessionId) {
        mergeGuestCartToUserCart(sessionId, token).then((merged) => {
          if (merged) {
            // إعادة جلب السلة بعد الدمج
            fetchCartDetails();
          }
        });
      }
    }
  }, []);

  // دالة حذف منتج من السلة
  const handleDeleteProduct = async (cartDetailsId) => {
    const token = sessionStorage.getItem("token");
    try {
      let response;
      
      if (token) {
        // المستخدم مسجل دخول
        response = await fetch(
        `${API_BASE_URL}Carts/RemoveProduct/${cartDetailsId}`,
        {
          method: "DELETE",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
        }
      );
      } else {
        // المستخدم غير مسجل - استخدام السلة المؤقتة
        const sessionId = getOrCreateSessionId();
        response = await fetch(
          `${API_BASE_URL}Carts/RemoveGuestProduct/${cartDetailsId}`,
          {
            method: "DELETE",
            headers: {
              "Content-Type": "application/json",
              sessionId: sessionId,
            },
          }
        );
      }
      
      if (!response.ok) {
        throw new Error("Failed to delete product");
      }
      // تحديث حالة السلة بعد الحذف
      setCartItems(
        cartItems.filter((item) => item.cartDetailsId !== cartDetailsId)
      );
    } catch (error) {
      console.error("Error deleting product:", error.message);
    }
  };

  // دالة حذف جميع المنتجات من السلة
  const handleDeleteAllCart = async () => {
    const token = sessionStorage.getItem("token");
    try {
      let response;
      
      if (token) {
        // المستخدم مسجل دخول
        response = await fetch(
        `${API_BASE_URL}Carts/RemoveCartDetails/${cartItems[0]?.cartId}`,
        {
          method: "DELETE",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
        }
      );
      } else {
        // المستخدم غير مسجل - حذف السلة المؤقتة
        const sessionId = getOrCreateSessionId();
        // نحتاج إلى cartId من السلة المؤقتة
        if (cartItems.length > 0 && cartItems[0].cartId) {
          response = await fetch(
            `${API_BASE_URL}Carts/RemoveCartDetails/${cartItems[0].cartId}`,
            {
              method: "DELETE",
              headers: {
                "Content-Type": "application/json",
                sessionId: sessionId,
              },
            }
          );
        } else {
          // إذا لم يكن هناك cartId، فقط امسح القائمة محلياً
          setCartItems([]);
          return;
        }
      }
      
      if (!response.ok) {
        throw new Error("Failed to delete all items in cart");
      }
      setCartItems([]);
    } catch (error) {
      console.error("Error deleting all items:", error.message);
    }
  };

  // حساب إجمالي سعر السلة
  const totalCartPrice = cartItems.reduce(
    (acc, item) => acc + item.totalPrice,
    0
  );

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 to-orange-50 py-8">
      <Helmet>
        <title>{t("cart.metaTitle", "سلة التسوق")} | {SiteName}</title>
        <meta
          name="description"
          content={t("cart.metaDesc", "عرض تفاصيل السلة وإجمالي السعر للمنتجات المُختارة في الملف الشخصي")}
        />
      </Helmet>
      
      <div className="container mx-auto px-4 max-w-4xl">
        {/* Header */}
        <div className="rounded-2xl shadow-xl p-6 mb-8" style={{ background: 'linear-gradient(to right, #1e3a8a, #1e40af)' }}>
          <div className="flex items-center justify-between flex-wrap gap-4">
            <BackButton className="bg-white/20 hover:bg-white/30 text-white border-white/30" />
            
            <div className="flex items-center gap-3">
              <div className="bg-orange-500 p-3 rounded-full">
                <FiShoppingCart className="text-white" size={24} />
              </div>
              <h1 className="text-2xl font-bold" style={{ color: 'white' }}>{t("cart.title", "سلة التسوق")}</h1>
            </div>
            
            <div className="flex items-center gap-4">
              <CurrencySelector />
              {cartItems.length > 0 && (
                <div className="text-right">
                  <p className="text-sm" style={{ color: '#bfdbfe' }}>{t("cart.total", "الإجمالي")}</p>
                  <p className="text-2xl font-bold" style={{ color: '#fb923c' }}>
                    {format(totalCartPrice)}
                  </p>
                </div>
              )}
            </div>
          </div>
        </div>

        {/* Empty State */}
        {cartItems.length === 0 ? (
          <div className="bg-white rounded-2xl shadow-lg p-12 text-center">
            <div className="bg-blue-100 w-24 h-24 rounded-full flex items-center justify-center mx-auto mb-6">
              <FiShoppingCart className="text-blue-600" size={40} />
            </div>
            <h3 className="text-2xl font-bold text-gray-800 mb-4">{t("cart.empty", "سلة التسوق فارغة")}</h3>
            <p className="text-gray-600 mb-8">{t("cart.emptyDesc", "لم تقم بإضافة أي منتجات إلى سلة التسوق بعد")}</p>
            <button 
              onClick={() => navigate("/")}
              className="bg-gradient-to-r from-orange-500 to-orange-600 hover:from-orange-600 hover:to-orange-700 text-white font-bold py-3 px-8 rounded-full transition-all duration-300 transform hover:-translate-y-1 shadow-lg hover:shadow-xl"
            >
              {t("cart.browseProducts", "تصفح المنتجات")}
            </button>
          </div>
        ) : (
          <div className="space-y-6">
            {/* Action Buttons */}
            {cartItems.length > 1 && (
              <div className="flex gap-4 justify-between items-center">
                <button 
                  onClick={handleDeleteAllCart}
                  className="px-6 py-3 font-bold rounded-xl transition-all duration-300 transform hover:-translate-y-1 shadow-lg hover:shadow-xl flex items-center gap-2"
                  style={{ 
                    color: 'white',
                    background: 'linear-gradient(to right, #dc2626, #b91c1c)'
                  }}
                  onMouseEnter={(e) => {
                    e.target.style.background = 'linear-gradient(to right, #b91c1c, #991b1b)';
                  }}
                  onMouseLeave={(e) => {
                    e.target.style.background = 'linear-gradient(to right, #dc2626, #b91c1c)';
                  }}
                >
                  <FiTrash2 size={18} style={{ color: 'white' }} />
                  <span style={{ color: 'white' }}>{t("cart.deleteAll", "حذف الكل")}</span>
                </button>
              </div>
            )}

            {/* Cart Items */}
            <div className="space-y-4">
              {cartItems.map((item, index) => (
                <div key={index} className="bg-white rounded-2xl shadow-lg overflow-hidden border border-blue-100 hover:shadow-xl transition-all duration-300">
                  <div className="p-6">
                    <div className="flex gap-6">
                      {/* Product Image */}
                      <div className="flex-shrink-0">
                        <div className="w-24 h-24 rounded-xl overflow-hidden border-2 border-blue-200">
                          <img
                            src={`${ServerPath + item.image}`}
                            alt={item.productName}
                            className="w-full h-full object-cover"
                          />
                        </div>
                      </div>

                      {/* Product Details */}
                      <div className="flex-1">
                        <div className="flex justify-between items-start mb-4 gap-4">
                          <h3 className="text-lg font-bold text-blue-900 flex-1">{item.productName}</h3>
                          <button
                            onClick={() => handleDeleteProduct(item.cartDetailsId)}
                            className="px-4 py-2 rounded-lg transition-all duration-200 flex items-center gap-2 shadow-md hover:shadow-lg flex-shrink-0"
                            style={{ 
                              color: 'white',
                              backgroundColor: '#dc2626'
                            }}
                            onMouseEnter={(e) => {
                              e.target.style.backgroundColor = '#b91c1c';
                            }}
                            onMouseLeave={(e) => {
                              e.target.style.backgroundColor = '#dc2626';
                            }}
                            title={t("cart.deleteProduct", "حذف المنتج")}
                          >
                            <FiTrash2 size={18} style={{ color: 'white' }} />
                            <span className="text-sm font-semibold" style={{ color: 'white' }}>{t("cart.delete", "حذف")}</span>
                          </button>
                        </div>

                        {/* Attributes */}
                        <div className="grid grid-cols-2 gap-4 mb-4">
                          <div className="bg-blue-50 rounded-lg p-3">
                            <p className="text-sm text-blue-600 font-medium">{t("cart.color", "اللون")}</p>
                            <p className="text-blue-900 font-semibold">{item.color ? translateColor(item.color) : t("cart.notAvailable", "غير متوفر")}</p>
                          </div>
                          <div className="bg-blue-50 rounded-lg p-3">
                            <p className="text-sm text-blue-600 font-medium">{t("cart.size", "المقاس")}</p>
                            <p className="text-blue-900 font-semibold">{item.size || t("cart.notAvailable", "غير متوفر")}</p>
                          </div>
                        </div>

                        {/* Price Details */}
                        <div className="bg-gradient-to-r from-blue-50 to-orange-50 rounded-xl p-4 border border-blue-200">
                          <div className="grid grid-cols-2 gap-4 text-sm">
                            <div className="space-y-2">
                              <div className="flex justify-between">
                                <span className="text-blue-600">{t("cart.originalPrice", "السعر الأصلي")}:</span>
                                <span className="text-blue-900 font-medium line-through">{format(item.unitPriceBeforeDiscount)}</span>
                              </div>
                              <div className="flex justify-between">
                                <span className="text-blue-600">{t("cart.discount", "الخصم")}:</span>
                                <span className="text-green-600 font-medium">{item.discountPercentage}%</span>
                              </div>
                            </div>
                            <div className="space-y-2">
                              <div className="flex justify-between">
                                <span className="text-blue-600">{t("cart.priceAfterDiscount", "السعر بعد الخصم")}:</span>
                                <span className="text-orange-600 font-medium">{format(item.unitPriceAfterDiscount)}</span>
                              </div>
                              <div className="flex justify-between">
                                <span className="text-blue-600">{t("cart.quantity", "الكمية")}:</span>
                                <span className="text-blue-900 font-medium">{item.quantity}</span>
                              </div>
                            </div>
                          </div>
                          
                          {/* Total Price */}
                          <div className="mt-3 pt-3 border-t border-blue-200">
                            <div className="flex justify-between items-center">
                              <span className="text-blue-800 font-semibold">{t("cart.total", "الإجمالي")}:</span>
                              <span className="text-xl font-bold text-orange-600">{format(item.totalPrice)}</span>
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              ))}
            </div>

            {/* Summary Card */}
            <div className="rounded-2xl shadow-xl p-6" style={{ background: 'linear-gradient(to right, #1e3a8a, #1e40af)' }}>
              <div className="flex justify-between items-center mb-4">
                <span className="text-lg" style={{ color: 'white' }}>{t("cart.cartTotal", "إجمالي السلة")}:</span>
                <span className="text-2xl font-bold" style={{ color: '#fb923c' }}>{format(totalCartPrice)}</span>
              </div>
              
              {/* Buy Button - Green and at the bottom */}
              {cartItems.length > 0 && (
                <button 
                  onClick={handleBuyAll}
                  className="w-full px-6 py-4 font-bold rounded-xl transition-all duration-300 transform hover:-translate-y-1 shadow-lg hover:shadow-xl flex items-center justify-center gap-2 mt-4"
                  style={{ 
                    color: 'white',
                    background: 'linear-gradient(to right, #16a34a, #15803d)'
                  }}
                  onMouseEnter={(e) => {
                    e.target.style.background = 'linear-gradient(to right, #15803d, #166534)';
                  }}
                  onMouseLeave={(e) => {
                    e.target.style.background = 'linear-gradient(to right, #16a34a, #15803d)';
                  }}
                >
                  <span style={{ color: 'white' }}>{t("cart.buyAll", "شراء الكل")}</span>
                </button>
              )}
            </div>
          </div>
        )}

        {/* Loading Overlay */}
        {isLoading && (
          <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
            <div className="bg-white p-8 rounded-2xl shadow-2xl">
              <div className="flex items-center gap-4">
                <div className="w-8 h-8 border-4 border-orange-500 border-t-transparent rounded-full animate-spin"></div>
                <span className="text-blue-900 font-semibold">{t("cart.loading", "جاري التحميل...")}</span>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}