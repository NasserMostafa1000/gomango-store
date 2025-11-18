import React, { useEffect, useState } from "react";
import API_BASE_URL from "../Constant";
import { useI18n } from "../i18n/I18nContext";

export default function AnnouncementBar() {
  const [announcement, setAnnouncement] = useState(null);
  const { lang } = useI18n();

  useEffect(() => {
    const load = async () => {
      try {
        const res = await fetch(`${API_BASE_URL}AnnouncementBar/active?lang=${lang}`);
        if (res.ok) {
          const data = await res.json();
          if (process.env.NODE_ENV === 'development') {
            console.log('AnnouncementBar received:', data);
          }
          // API returns { Id, Text, LinkUrl, IsActive }
          if (data && data.text) {
            setAnnouncement({
              id: data.id,
              text: data.text,
              linkUrl: data.linkUrl,
              isActive: data.isActive !== false // Default to true if not specified
            });
          } else {
            setAnnouncement(null);
          }
        } else if (res.status === 404) {
          // No active announcement - this is OK
          setAnnouncement(null);
        }
      } catch (e) {
        console.error("Failed to load announcement", e);
        setAnnouncement(null);
      }
    };
    load();
  }, [lang]);

  if (!announcement) return null;

  const isRTL = lang === 'ar';

  return (
    <div className="w-full bg-[#0A2540] text-white py-2 px-4 border-b border-[#05132b]" dir={isRTL ? "rtl" : "ltr"}>
      <marquee
        direction={isRTL ? "right" : "left"}
        scrollamount="6"
        className="font-medium text-sm md:text-base"
      >
        {announcement.linkUrl ? (
          <a href={announcement.linkUrl} className="hover:underline text-white">
            {announcement.text}
          </a>
        ) : (
          announcement.text
        )}
      </marquee>
    </div>
  );
}

