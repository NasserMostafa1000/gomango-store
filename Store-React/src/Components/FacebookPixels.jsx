import React, { useEffect } from "react";
import { useLocation } from "react-router-dom";
import ReactPixel from "react-facebook-pixel";
import { trackPageView } from "./utils/facebookPixel";

const FacebookPixel = () => {
  const location = useLocation();

  useEffect(() => {
    const options = {
      autoConfig: true,
      debug: false,
    };
    ReactPixel.init("1147309077566646", undefined, options);
    ReactPixel.pageView(); // يسجل زيارة الصفحة تلقائيًا
  }, []);

  // تتبع تغييرات الصفحات
  useEffect(() => {
    trackPageView();
  }, [location.pathname]);

  return null; // لا يظهر شيء
};

export default FacebookPixel;
