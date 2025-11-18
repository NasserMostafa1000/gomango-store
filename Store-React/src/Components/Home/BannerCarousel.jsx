import React, { useEffect, useState } from "react";
import API_BASE_URL, { ServerPath } from "../Constant";
import { useI18n } from "../i18n/I18nContext";

export default function BannerCarousel() {
  const [banners, setBanners] = useState([]);
  const [idx, setIdx] = useState(0);
  const { lang } = useI18n();

  useEffect(() => {
    const load = async () => {
      try {
        const res = await fetch(`${API_BASE_URL}Banners/active?lang=${lang}`);
        const data = await res.json();
        setBanners(data || []);
      } catch (e) {
        console.error("Failed to load banners", e);
      }
    };
    load();
  }, [lang]);

  useEffect(() => {
    if (!banners.length) return;
    const t = setInterval(() => setIdx((p) => (p + 1) % banners.length), 4000);
    return () => clearInterval(t);
  }, [banners]);

  if (!banners.length) return null;
  const current = banners[idx];
  const imageSrc = current.imageUrl?.startsWith("http")
    ? current.imageUrl
    : `${ServerPath}${current.imageUrl ?? ""}`;
  const clickUrl = (current.linkUrl || current.clickUrl)?.trim();

  const Wrapper = ({ children }) =>
    clickUrl ? (
      <a
        href={clickUrl}
        target="_blank"
        rel="noopener noreferrer"
        className="block group focus:outline-none focus-visible:ring-4 focus-visible:ring-brand-orange/40 rounded-md"
      >
        {children}
      </a>
    ) : (
      <div>{children}</div>
    );

  return (
    <Wrapper>
      <div
        className={`relative w-full h-[40vh] md:h-[60vh] overflow-hidden rounded-md ${
          clickUrl ? "cursor-pointer" : ""
        }`}
      >
        <img src={imageSrc} alt={current.title} className="w-full h-full object-cover" />
        {(current.title || current.subTitle) && (
          <div className="absolute inset-0 bg-gradient-to-t from-[#F9F6EF]/80 to-transparent flex items-end">
            <div className="p-4 md:p-8 text-[#0A2C52]">
              {current.title && (
                <h2 className="text-2xl md:text-4xl font-bold text-[#0A2C52]">
                  {current.title}
                </h2>
              )}
              {current.subTitle && (
                <p className="mt-2 text-sm md:text-base text-[#0A2C52] opacity-90">
                  {current.subTitle}
                </p>
              )}
            </div>
          </div>
        )}
      </div>
    </Wrapper>
  );
}


