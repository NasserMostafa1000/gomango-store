import React, { useEffect, useState } from "react";
import API_BASE_URL from "../Constant";
import ProductItem from "./ProductItem";
import { getRoleFromToken } from "../utils";
import { useNavigate } from "react-router-dom";
import { useI18n } from "../i18n/I18nContext";

export default function RelatedProducts({ product }) {
  const [items, setItems] = useState([]);
  const navigate = useNavigate();
  const token = sessionStorage.getItem("token");
  const role = getRoleFromToken(token);
  const { t, lang } = useI18n();

  useEffect(() => {
    const load = async () => {
      try {
        // جلب جميع المنتجات
        const res = await fetch(`${API_BASE_URL}Product/GetAllProductsWithLimit?page=1&limit=50&lang=${lang}`);
        const data = await res.json();
        let list = data || [];
        
        // فلترة المنتجات بناءً على نفس الكاتيجوري
        const categoryId = product?.categoryId || product?.CategoryId;
        
        if (categoryId != null) {
          // فلترة المنتجات من نفس الكاتيجوري
          list = list.filter(p => {
            const pCategoryId = p.categoryId || p.CategoryId;
            return pCategoryId === categoryId;
          });
        }
        
        // استبعاد المنتج الحالي
        list = list.filter(p => p.productId !== product?.productId);
        
        // إذا لم نجد منتجات من نفس الكاتيجوري، نعرض منتجات عشوائية
        if (list.length === 0) {
          const allRes = await fetch(`${API_BASE_URL}Product/GetAllProductsWithLimit?page=1&limit=20&lang=${lang}`);
          const allData = await allRes.json();
          list = (allData || []).filter(p => p.productId !== product?.productId);
        }
        
        // أخذ أول 8 منتجات
        setItems(list.slice(0, 8));
      } catch (e) {
        console.error("Failed to load related products", e);
        setItems([]);
      }
    };
    if (product) load();
  }, [product, lang]);

  const open = (p) => {
    navigate(`/productDetails/${p.productId}`, { state: { product: p }, replace: true });
  };

  if (!items.length) return null;
  return (
    <div className="mt-8 bg-white rounded-xl shadow-lg p-4 md:p-6 border border-gray-200">
      <div className="flex items-center justify-between mb-4">
        <h3 className="text-xl md:text-2xl font-bold text-gray-900">{t("relatedProducts.title", "منتجات ذات صلة")}</h3>
        <div className="flex items-center gap-2 text-sm text-gray-600">
          <svg className="w-5 h-5 text-orange-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
          </svg>
          <span>{items.length} {t("relatedProducts.products", "منتج")}</span>
        </div>
      </div>
      <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-4 gap-4">
        {items.map(p => (
          <div 
            key={p.productId} 
            onClick={() => open(p)} 
            className="cursor-pointer transform hover:scale-105 transition-all duration-200 hover:shadow-lg rounded-lg overflow-hidden"
          >
            <ProductItem product={p} CurrentRole={role} />
          </div>
        ))}
      </div>
    </div>
  );
}


