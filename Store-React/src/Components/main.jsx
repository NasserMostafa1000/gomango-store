import React from "react";
import "../index.css";
import ReactDOM from "react-dom/client"; // تأكد من استخدام ReactDOM/client
import { BrowserRouter as Router, Routes, Route } from "react-router-dom"; // استيراد Route أيضًا
import { GoogleOAuthProvider } from "@react-oauth/google";
import App from "./App";
import ForgotPassword from "../Components/Login&Register/ForgotPassword";
import Signup from "../Components/Login&Register/Signup";
import Home from "./Home/Home";
import Login from "../Components/Login&Register/Login";
import FindProducts from "../Components/Products/FindProducts";
import ProductDetails from "../Components/Products/ProductDetails";
import Cart from "./Cart/Cart";
import PurchaseDetailsOperation from "./CreateOrder/PurchaseDetailsOperation";
import PaymentSuccess from "./CreateOrder/PaymentSuccess";
import PaymentCancel from "./CreateOrder/PaymentCancel";
import MyPurchases from "./Clients/MyPurchases";
import PurchaseDetails from "./Clients/PurchaseDetails";
import MyProfile from "./Clients/MyProfile";
import Orders from "./AdminBar/Orders/Orders";
import OrderDetails from "./AdminBar/Orders/OrderDetails";
import Visitors from "./AdminBar/Visitors/Visitors";
import ShippingInfo from "./AdminBar/Shipping/ShippingInfo";
import Employees from "./AdminBar/Managers/Employees";
import AddProduct from "./AdminBar/Products/AddProduct";
import AddProductDetails from "./AdminBar/Products/AddProductDetails";
import ProductForm from "./AdminBar/Products/UpdateProduct/ProductForm";
import CategoriesAdmin from "./AdminBar/Products/CategoriesAdmin";
import Clients from "./AdminBar/Clients";
import UpdateAdminInfo from "./AdminBar/UpdateContactUs";
import LegalContentEditor from "./AdminBar/LegalContent/LegalContentEditor.jsx";
import ContactUsCom from "./Contact_About/ContactUsCom";
import AboutUs from "./Contact_About/AboutUs";
import PrivacyAndTerms from "./Privacy_Terms/PrivacyAndTerms";
import ClientSearching from "./Clients/CLientSearching";
import { CurrencyProvider } from "./Currency/CurrencyContext";
import BannersAdmin from "./AdminBar/Banners/BannersAdmin";
import AnnouncementBarAdmin from "./AdminBar/Banners/AnnouncementBarAdmin";
import CurrencyAdmin from "./AdminBar/Currency/CurrencyAdmin";
import MyReviews from "./Clients/MyReviews";
import ReviewsAdmin from "./AdminBar/Reviews/ReviewsAdmin";
import { I18nProvider } from "./i18n/I18nContext";

// دالة لإصلاح خلفيات الأزرار
function fixButtonBackgrounds() {
  if (typeof document === 'undefined') return;
  
  const buttons = document.querySelectorAll('button');
  buttons.forEach(button => {
    const classes = button.className || '';
    
    // إصلاح الأزرار الخضراء
    if ((classes.includes('from-green') || classes.includes('bg-green')) && !button.style.background) {
      if (classes.includes('bg-gradient-to-l')) {
        button.style.background = 'linear-gradient(to left, #16a34a, #15803d)';
      } else if (classes.includes('bg-gradient-to-r')) {
        button.style.background = 'linear-gradient(to right, #16a34a, #15803d)';
      } else if (classes.includes('bg-green-600')) {
        button.style.background = '#16a34a';
      } else if (classes.includes('bg-green-700')) {
        button.style.background = '#15803d';
      }
    }
    
    // إصلاح الأزرار البرتقالية
    if ((classes.includes('from-orange') || classes.includes('bg-orange')) && !button.style.background) {
      if (classes.includes('bg-gradient-to-l')) {
        button.style.background = 'linear-gradient(to left, #f97316, #ea580c)';
      } else if (classes.includes('bg-gradient-to-r')) {
        button.style.background = 'linear-gradient(to right, #f97316, #ea580c)';
      } else if (classes.includes('bg-orange-500')) {
        button.style.background = '#f97316';
      } else if (classes.includes('bg-orange-600')) {
        button.style.background = '#ea580c';
      }
    }
    
    // إصلاح الأزرار الحمراء
    if ((classes.includes('from-red') || classes.includes('bg-red')) && !button.style.background) {
      if (classes.includes('bg-gradient-to-l')) {
        button.style.background = 'linear-gradient(to left, #dc2626, #b91c1c)';
      } else if (classes.includes('bg-gradient-to-r')) {
        button.style.background = 'linear-gradient(to right, #dc2626, #b91c1c)';
      } else if (classes.includes('bg-red-500')) {
        button.style.background = '#dc2626';
      } else if (classes.includes('bg-red-600')) {
        button.style.background = '#b91c1c';
      }
    }
    
    // إصلاح الأزرار الزرقاء
    if ((classes.includes('from-blue') || classes.includes('bg-blue')) && !button.style.background) {
      if (classes.includes('bg-gradient-to-l')) {
        button.style.background = 'linear-gradient(to left, #2563eb, #1e40af)';
      } else if (classes.includes('bg-gradient-to-r')) {
        button.style.background = 'linear-gradient(to right, #2563eb, #1e40af)';
      } else if (classes.includes('bg-blue-600')) {
        button.style.background = '#2563eb';
      } else if (classes.includes('bg-blue-700')) {
        button.style.background = '#1e40af';
      } else if (classes.includes('bg-blue-800')) {
        button.style.background = '#1e3a8a';
      } else if (classes.includes('bg-blue-900')) {
        button.style.background = '#1e3a8a';
      }
    }
    
    // إصلاح الأزرار الداكنة
    if ((classes.includes('from-[#0a2540]') || classes.includes('to-[#13345d]') || classes.includes('bg-[#0a2540]') || classes.includes('bg-[#13345d]')) && !button.style.background) {
      if (classes.includes('bg-gradient-to-l')) {
        button.style.background = 'linear-gradient(to left, #0a2540, #13345d)';
      } else if (classes.includes('bg-gradient-to-r')) {
        button.style.background = 'linear-gradient(to right, #0a2540, #13345d)';
      } else if (classes.includes('bg-[#0a2540]')) {
        button.style.background = '#0a2540';
      } else if (classes.includes('bg-[#13345d]')) {
        button.style.background = '#13345d';
      }
    }
    
    // ضمان النص الأبيض
    if (classes.includes('text-white') && (classes.includes('bg-gradient') || classes.includes('bg-green') || classes.includes('bg-blue') || classes.includes('bg-red') || classes.includes('bg-orange'))) {
      button.style.color = 'white';
      const spans = button.querySelectorAll('span');
      const svgs = button.querySelectorAll('svg');
      spans.forEach(span => span.style.color = 'white');
      svgs.forEach(svg => {
        svg.style.color = 'white';
        svg.style.stroke = 'white';
      });
    }
  });
}

// تشغيل الدالة عند تحميل الصفحة وبعد تحديث DOM
if (typeof window !== 'undefined') {
  setTimeout(() => {
    fixButtonBackgrounds();
    const observer = new MutationObserver(() => {
      setTimeout(fixButtonBackgrounds, 100);
    });
    if (document.body) {
      observer.observe(document.body, { childList: true, subtree: true });
    }
  }, 100);
}

const root = ReactDOM.createRoot(document.getElementById("root"));
root.render(
  <GoogleOAuthProvider clientId="1002692311708-dv44b5us60jlovbgdcv87rbuvgfs01vo.apps.googleusercontent.com">
    <I18nProvider>
      <CurrencyProvider>
        <Router>
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/forgot-password" element={<ForgotPassword />} />
            <Route path="/register" element={<Signup />} />
            <Route path="/Login" element={<Login />} />
            <Route path="/FindProducts" element={<FindProducts />} />
            <Route path="/productDetails/:id" element={<ProductDetails />} />
            <Route path="/Cart" element={<Cart />} />
            <Route path="/PurchaseDetails" element={<PurchaseDetailsOperation />} />
            <Route path="/payment-success" element={<PaymentSuccess />} />
            <Route path="/payment-cancelled" element={<PaymentCancel />} />
            <Route path="/MyPurchases" element={<MyPurchases />} />
            <Route path="/Admin/Orders" element={<Orders />} />
            <Route path="/Admin/Visitors" element={<Visitors />} />
            <Route path="/Admin/shipping-prices" element={<ShippingInfo />} />
            <Route path="/admins/Employees" element={<Employees />} />
            <Route path="/admins/AddProduct" element={<AddProduct />} />
            <Route path="/admin/categories" element={<CategoriesAdmin />} />
            <Route path="/admin/edit-product" element={<ProductForm />} />
            <Route path="/admin/Clients" element={<Clients />} />
            <Route path="/admin/UpdateAdminInfo" element={<UpdateAdminInfo />} />
            <Route path="/admin/legal-content" element={<LegalContentEditor />} />
            <Route path="/admin/banners" element={<BannersAdmin />} />
            <Route path="/admin/announcement-bar" element={<AnnouncementBarAdmin />} />
            <Route path="/admin/currency" element={<CurrencyAdmin />} />
            <Route path="/Contact" element={<ContactUsCom />} />
            <Route path="/about-us" element={<AboutUs />} />
            <Route path="/terms" element={<PrivacyAndTerms />} />
            <Route path="/Admin/ClientsSearshing" element={<ClientSearching />} />
            <Route
              path="/admins/AddProductDetails"
              element={<AddProductDetails />}
            />
            <Route
              path="/Admin/order-details/:orderId"
              element={<OrderDetails />}
            />
            <Route
              path="/Purchase-Details/:orderId"
              element={<PurchaseDetails />}
            />
            <Route path="/MyProfile" element={<MyProfile />} />
            <Route path="/MyReviews" element={<MyReviews />} />
            <Route path="/admin/reviews" element={<ReviewsAdmin />} />
          </Routes>
        </Router>
      </CurrencyProvider>
    </I18nProvider>
  </GoogleOAuthProvider>
);
