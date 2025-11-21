import React, { useState, useEffect, useRef } from "react";
import { Helmet } from "react-helmet";
import { useLocation, useNavigate, useParams } from "react-router-dom";
import API_BASE_URL, {
  ServerPath,
  SiteName,
} from "../../Components/Constant.js";
import getDeliveryDate from "../utils.js";
import StoreLayout from "../Home/StoreLayout";
import Reviews from "./Reviews.jsx";
import RelatedProducts from "./RelatedProducts.jsx";
import { useCurrency } from "../Currency/CurrencyContext";
import CurrencySelector from "../Currency/CurrencySelector";
import { useI18n } from "../i18n/I18nContext";
import ProductMediaCard from "./ProductMediaCard.jsx";
import ProductInfoCard from "./ProductInfoCard.jsx";
import ProductOptionsCard from "./ProductOptionsCard.jsx";
import BackButton from "../Common/BackButton";

export default function ProductDetails() {
  const location = useLocation();
  const { id } = useParams();
  const navigate = useNavigate();
  const [product, setProduct] = useState(location.state?.product || null);
  const [Img, setImg] = useState("");
  const [productImages, setProductImages] = useState([]);
  const [loading, setLoading] = useState(!product);
  const [Colors, setColors] = useState([]);
  const [DetailsId, setDetailsId] = useState(0);
  const [CurrentColor, setCurrentColor] = useState("");
  const [Sizes, setSizes] = useState([]);
  const [CurrentSize, setCurrentSize] = useState("");
  const [availableQuantity, setAvailableQuantity] = useState(0);
  const [Quantity, setQuantity] = useState(1);
  const [banner, setBanner] = useState(null);
  const detailsRequestController = useRef(null);
  const { format } = useCurrency();
  const { t, lang } = useI18n();

  const showBanner = (text, tone = "info") => {
    setBanner({ text, tone });
    setTimeout(() => setBanner(null), 4000);
  };
  
  // Function to translate color names
  const translateColor = (colorName) => {
    if (!colorName) return colorName;
    
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
    
    const colorKey = colorMap[colorName] || colorName.toLowerCase().trim();
    return t(`colors.${colorKey}`, colorName);
  };

  useEffect(() => {
    let isMounted = true;
    const stateProduct = location.state?.product;

    // Always fetch product when language changes to get correct language
    const fetchProduct = async () => {
      try {
        setLoading(true);
        const response = await fetch(
          `${API_BASE_URL}Product/GetProductById?ID=${Number(id)}&lang=${lang}`
        );
        if (!response.ok) throw new Error("Network response was not ok");
        const data = await response.json();
        if (!isMounted) return;
        setProduct(data);
        setAvailableQuantity(data.quantity);
        setDetailsId(data.productDetailsId);
        setImg(data.productImage);
        // Fetch additional images for the initial product details
        if (data.productDetailsId) {
          try {
            const imagesResponse = await fetch(
              `${API_BASE_URL}Product/GetProductDetailImages/${data.productDetailsId}`
            );
            if (imagesResponse.ok) {
              const imagesData = await imagesResponse.json();
              if (imagesData && Array.isArray(imagesData) && imagesData.length > 0) {
                setProductImages(imagesData.map(img => img.imageUrl));
              } else {
                setProductImages([]);
              }
            }
          } catch (error) {
            console.error("Error fetching product images:", error);
            setProductImages([]);
          }
        }
      } catch (error) {
        if (isMounted) {
          console.error("Error fetching product:", error);
          // Fallback to state product if fetch fails
          if (stateProduct) {
            setProduct(stateProduct);
            setImg(stateProduct.productImage || "");
            if (stateProduct.quantity != null) {
              setAvailableQuantity(stateProduct.quantity);
            }
            if (stateProduct.productDetailsId != null) {
              setDetailsId(stateProduct.productDetailsId);
            }
          }
        }
      } finally {
        if (isMounted) {
          setLoading(false);
        }
      }
    };

    fetchProduct();

    return () => {
      isMounted = false;
    };
  }, [id, location.state?.product, lang]);

  const GetDetailsOfCurrentSizeAndColor = async (
    color = CurrentColor,
    size = CurrentSize
  ) => {
    if (!product || !color || !size) return;
    const trimmedColor = color?.trim();
    const trimmedSize = size?.trim();
    if (!trimmedColor || !trimmedSize) return;
    detailsRequestController.current?.abort();
    const controller = new AbortController();
    detailsRequestController.current = controller;
    setLoading(true);
    try {
      const response = await fetch(
        `${API_BASE_URL}Product/GetDetailsBy?ProductId=${Number(
          product?.productId
        )}&ColorName=${encodeURIComponent(trimmedColor)}&SizeName=${encodeURIComponent(trimmedSize)}`,
        { signal: controller.signal }
      );
      if (!response.ok) throw new Error("Network response was not ok");
      const data = await response.json();
      setAvailableQuantity(data.quantity);
      setDetailsId(data.productDetailsId);
      setImg(data.image);
      // Fetch additional images for the product details
      if (data.productDetailsId) {
        try {
          const imagesResponse = await fetch(
            `${API_BASE_URL}Product/GetProductDetailImages/${data.productDetailsId}`
          );
          if (imagesResponse.ok) {
            const imagesData = await imagesResponse.json();
            if (imagesData && Array.isArray(imagesData) && imagesData.length > 0) {
              setProductImages(imagesData.map((img) => img.imageUrl || img));
            } else {
              setProductImages([]);
            }
          } else {
            setProductImages([]);
          }
        } catch (error) {
          console.error("Error fetching product images:", error);
          setProductImages([]);
        }
      } else {
        setProductImages([]);
      }
    } catch (error) {
      if (error?.name === "AbortError") {
        return;
      }
      console.error(error.message);
    } finally {
      if (detailsRequestController.current === controller) {
        detailsRequestController.current = null;
        setLoading(false);
      }
    }
  };

  function CurrentProduct() {
    const Price =
      product?.discountPercentage === 0
        ? product?.productPrice
        : product?.priceAfterDiscount;
    return {
      productDetailsId: DetailsId,
      quantity: Quantity,
      unitPrice: Price,
      totalPrice: Price * Quantity,
    };
  }

  const handlBuyClick = () => {
    const token = sessionStorage.getItem("token");
    if (!token) {
      showBanner(t("productDetails.loginRequired", "يجب تسجيل الدخول لمتابعة عملية الشراء."), "error");
      navigate("/Login", { state: { path: `/ProductDetails/${id}` } });
      return;
    }
    const Product = CurrentProduct();
    navigate("/PurchaseDetails", { state: { Product, fromCart: false } });
  };

  const handleShareProduct = async () => {
    if (!product?.productId) return;
    const shareEndpoint = `${API_BASE_URL}Product/ShareProduct/${product.productId}?lang=${lang}`;
    const shareTitle = productName || SiteName;
    const shareText = moreDetails || shareTitle;

    try {
      if (typeof navigator !== "undefined" && navigator.share) {
        await navigator.share({
          title: shareTitle,
          text: shareText,
          url: shareEndpoint,
        });
        showBanner(t("productDetails.shareSuccess", "تمت مشاركة المنتج بنجاح."), "success");
      } else if (typeof navigator !== "undefined" && navigator.clipboard) {
        await navigator.clipboard.writeText(shareEndpoint);
        showBanner(t("productDetails.shareCopied", "تم نسخ رابط المشاركة."), "success");
      } else {
        window.open(shareEndpoint, "_blank");
      }
    } catch (error) {
      if (error?.name === "AbortError") {
        return;
      }
      showBanner(t("productDetails.shareError", "تعذر مشاركة المنتج حالياً."), "error");
    }
  };

  useEffect(() => {
    if (!product) return;

    const fetchProducts = async () => {
      try {
        const response = await fetch(
          `${API_BASE_URL}Product/GetProductDetailsById?Id=${product?.productId}`
        );
        if (!response.ok) throw new Error("Network response was not ok");
        const data = await response.json();
        setCurrentColor(data.color);
        setDetailsId(data.productDetailsId);
        setCurrentSize(data.size);
        setAvailableQuantity(data.quantity);
      } catch (error) {
        console.error("Error fetching product details:", error);
      } finally {
        setLoading(false);
      }
    };

    const fetchColorsAndSizes = async () => {
      try {
        const sizesResponse = await fetch(
          `${API_BASE_URL}Product/GetSizesByProductId?productId=${product?.productId}`
        );
        if (!sizesResponse.ok) throw new Error("Network response was not ok");
        const sizesData = await sizesResponse.json();
        setSizes(sizesData);
        if (sizesData.length === 1) setCurrentSize(sizesData[0]);

        const colorsResponse = await fetch(
          `${API_BASE_URL}Product/GetColorsByProductId?productId=${product?.productId}`
        );
        if (!colorsResponse.ok) throw new Error("Network response was not ok");
        const colorsData = await colorsResponse.json();
        setColors(colorsData);
        if (colorsData.length === 1) setCurrentColor(colorsData[0]);
      } catch (error) {
        console.error("Error fetching colors and sizes:", error);
      }
    };

    fetchProducts();
    fetchColorsAndSizes();
  }, [product, lang]);

  useEffect(() => {
    const fetchDetails = async () => {
      if (!CurrentSize) return;
      setLoading(true);
      try {
        const response = await fetch(
          `${API_BASE_URL}Product/GetColorsBelongsToSpecificSize?ProductId=${product?.productId}&SizeName=${CurrentSize}`
        );
        if (!response.ok) throw new Error("Network response was not ok");
        const data = await response.json();
        setColors(data);
        if (data.length === 1) setCurrentColor(data[0]);
      } catch (error) {
        console.error("Error fetching product details:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchDetails();
  }, [CurrentSize]);

  useEffect(() => {
    if (CurrentColor && CurrentSize) {
      GetDetailsOfCurrentSizeAndColor(CurrentColor, CurrentSize);
    }
  }, [CurrentSize, CurrentColor]);

  useEffect(() => {
    return () => {
      detailsRequestController.current?.abort();
    };
  }, []);

  useEffect(() => {
    const middleY = window.innerHeight / 2;
    window.scrollTo({
      top: middleY - 500,
      behavior: "smooth",
    });
  });

  if (loading) return (
    <div className="flex justify-center items-center min-h-screen">
      <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-orange-500"></div>
    </div>
  );

  // دالة لمعالجة النص للحفاظ على الأسطر الفارغة
  const formatDescription = (text) => {
    if (!text)
      return <p className="text-black">{t("productDetails.noDetails", "لا توجد تفاصيل إضافية")}</p>;

    return text.split("\n").map((line, index) =>
      line.trim() === "" ? (
        <br key={index} />
      ) : (
        <p key={index} className="text-black">
          {line}
        </p>
      )
    );
  };

  const Price = product?.discountPercentage === 0 ? product?.productPrice : product?.priceAfterDiscount;
  const isRTL = lang === "ar";
  const deliveryText = `${t("productDetails.deliveryTime", "التوصيل خلال")} ${getDeliveryDate(lang === "en" ? "en" : "ar")}`;
  const returnPolicyText = t("productDetails.returnPolicy", "إرجاع خلال 14 يوم");
  
  // API returns the correct language already
  const productName = product?.productName || "";
  const moreDetails = product?.moreDetails || "";

  return (
    <div dir={isRTL ? "rtl" : "ltr"}>
      <Helmet>
        <script type="application/ld+json">
          {JSON.stringify({
            "@context": "https://schema.org",
            "@type": "Product",
            name: productName,
            image: [`${ServerPath}${Img}`],
            description: moreDetails,
            sku: id,
            brand: {
              "@type": "Brand",
              name: SiteName,
            },
            offers: {
              "@type": "Offer",
              url: window.location.href,
              priceCurrency: "EGP",
              price: Price,
              priceValidUntil: "2025-12-31",
              availability:
                availableQuantity > 0
                  ? "https://schema.org/InStock"
                  : "https://schema.org/OutOfStock",
              itemCondition: "https://schema.org/NewCondition",
            },
          })}
        </script>

        <title>
          {productName || t("productDetails.product", "منتج")} | {SiteName}
        </title>
        <meta
          name="description"
          content={`${productName} - ${moreDetails}. ${t("productDetails.availableNow", "متاح الآن على")} ${SiteName}. ${t("productDetails.discoverMore", "اكتشف المزيد من المنتجات المميزة بأفضل الأسعار.")}`}
        />
        <link rel="canonical" href={window.location.href} />
        <meta property="og:type" content="product" />
        <meta property="og:title" content={productName} />
        <meta property="og:description" content={moreDetails} />
        <meta property="og:image" content={`${ServerPath}${Img}`} />
        <meta property="og:url" content={window.location.href} />
        <meta property="og:site_name" content={SiteName} />
        <meta property="og:locale" content={isRTL ? "ar_AE" : "en_US"} />
        <meta name="twitter:card" content="summary_large_image" />
        <meta name="twitter:title" content={productName} />
        <meta name="twitter:description" content={moreDetails} />
        <meta name="twitter:image" content={`${ServerPath}${Img}`} />
        <meta name="twitter:url" content={window.location.href} />
      </Helmet>

      <StoreLayout>
        {banner && (
          <div
            className={`fixed top-20 left-1/2 transform -translate-x-1/2 px-6 py-3 rounded-lg shadow-lg z-50 text-white ${
              banner.tone === "error" ? "bg-red-500" : "bg-emerald-600"
            }`}
          >
            {banner.text}
          </div>
        )}

        <div className="max-w-7xl mx-auto px-4 py-8 space-y-10">
        {/* Back Button */}
        <div className="mb-4">
          <BackButton />
        </div>
        
        {/* Mobile Layout: Vertical Stack */}
        <div className="flex flex-col lg:hidden space-y-6">
          {/* الصور */}
          <ProductMediaCard
            imageUrl={ServerPath + Img}
            productName={productName}
            additionalImages={productImages.map(img => ServerPath + img)}
          />

          {/* الكمية والألوان والمخزون */}
          <div className="space-y-6">
            <ProductInfoCard
              productName={productName || ""}
              priceFormatted={format(Price)}
              originalPriceFormatted={format(product?.productPrice)}
              discountPercentage={product?.discountPercentage || 0}
              availableQuantity={availableQuantity}
              isRTL={isRTL}
              t={t}
              deliveryText={deliveryText}
              returnPolicyText={returnPolicyText}
              CurrencySelectorComponent={CurrencySelector}
              onShare={handleShareProduct}
            />

            <ProductOptionsCard
              t={t}
              Colors={Colors}
              CurrentColor={CurrentColor}
              setCurrentColor={setCurrentColor}
              Sizes={Sizes}
              CurrentSize={CurrentSize}
              setCurrentSize={setCurrentSize}
              Quantity={Quantity}
              setQuantity={setQuantity}
              availableQuantity={availableQuantity}
              handlBuyClick={handlBuyClick}
              DetailsId={DetailsId}
              translateColor={translateColor}
              isRTL={isRTL}
              showBanner={showBanner}
              productId={product?.productId}
              onColorChange={(color) => {
                setCurrentColor(color);
              }}
              onSizeChange={(size) => {
                setCurrentSize(size);
              }}
            />
          </div>

          {/* قسم التفاصيل الإضافية */}
          <div className="bg-[#FAFAFA] rounded-2xl shadow-xl p-6">
            <h3 className="text-2xl font-bold text-gray-900 mb-6">
              {t("productDetails.productDetails", "تفاصيل المنتج")}
            </h3>
            <div className="text-gray-700 leading-relaxed text-lg">
              {formatDescription(moreDetails)}
            </div>
          </div>

          {/* التعليقات */}
          <div className="bg-[#FAFAFA] rounded-2xl shadow-xl p-6">
            <Reviews productId={Number(product?.productId || id)} />
          </div>

          {/* المنتجات ذات الصلة */}
          <div className="bg-[#FAFAFA] rounded-2xl shadow-xl p-6">
            <RelatedProducts product={product} />
          </div>
        </div>

        {/* Desktop Layout: Two Columns */}
        <div className={`hidden lg:grid grid-cols-1 lg:grid-cols-2 gap-8 ${isRTL ? "" : "lg:flex-row-reverse"}`}>
          <ProductMediaCard
            imageUrl={ServerPath + Img}
            productName={productName}
            additionalImages={productImages.map(img => ServerPath + img)}
          />

          <div className="space-y-6">
            <ProductInfoCard
              productName={productName || ""}
              priceFormatted={format(Price)}
              originalPriceFormatted={format(product?.productPrice)}
              discountPercentage={product?.discountPercentage || 0}
              availableQuantity={availableQuantity}
              isRTL={isRTL}
              t={t}
              deliveryText={deliveryText}
              returnPolicyText={returnPolicyText}
              CurrencySelectorComponent={CurrencySelector}
              onShare={handleShareProduct}
            />

            <ProductOptionsCard
              t={t}
              Colors={Colors}
              CurrentColor={CurrentColor}
              setCurrentColor={setCurrentColor}
              Sizes={Sizes}
              CurrentSize={CurrentSize}
              setCurrentSize={setCurrentSize}
              Quantity={Quantity}
              setQuantity={setQuantity}
              availableQuantity={availableQuantity}
              handlBuyClick={handlBuyClick}
              DetailsId={DetailsId}
              translateColor={translateColor}
              isRTL={isRTL}
              showBanner={showBanner}
              productId={product?.productId}
              onColorChange={(color) => {
                setCurrentColor(color);
              }}
              onSizeChange={(size) => {
                setCurrentSize(size);
              }}
            />
          </div>
        </div>

        {/* Desktop: التفاصيل والتعليقات والمنتجات ذات الصلة */}
        <div className="hidden lg:block space-y-10">
          {/* قسم التفاصيل الإضافية */}
          <div className="bg-[#FAFAFA] rounded-2xl shadow-xl p-6">
            <h3 className="text-2xl font-bold text-gray-900 mb-6">
              {t("productDetails.productDetails", "تفاصيل المنتج")}
            </h3>
            <div className="text-gray-700 leading-relaxed text-lg">
              {formatDescription(moreDetails)}
            </div>
          </div>

          {/* التعليقات */}
          <div className="bg-[#FAFAFA] rounded-2xl shadow-xl p-6">
            <Reviews productId={Number(product?.productId || id)} />
          </div>

          {/* المنتجات ذات الصلة */}
          <div className="bg-[#FAFAFA] rounded-2xl shadow-xl p-6">
            <RelatedProducts product={product} />
          </div>
        </div>
        </div>
      </StoreLayout>
    </div>
  );
}