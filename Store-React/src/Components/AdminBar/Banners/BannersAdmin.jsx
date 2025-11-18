import React, { useEffect, useMemo, useState } from "react";
import API_BASE_URL, { ServerPath } from "../../Constant";
import { getRoleFromToken } from "../../utils";
import { useI18n } from "../../i18n/I18nContext";
import WebSiteLogo from "../../WebsiteLogo/WebsiteLogo.jsx";

export default function BannersAdmin() {
  const token = sessionStorage.getItem("token");
  const role = useMemo(() => getRoleFromToken(token), [token]);
  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(true);
  const [form, setForm] = useState({
    id: 0,
    titleAr: "",
    titleEn: "",
    subTitleAr: "",
    subTitleEn: "",
    imageUrl: "",
    linkUrl: "",
    isActive: true,
    displayOrder: 0,
  });
  const [submitting, setSubmitting] = useState(false);
  const [imageFile, setImageFile] = useState(null);
  const [imagePreview, setImagePreview] = useState("");
  const [uploadingImage, setUploadingImage] = useState(false);
  const { t } = useI18n();

  useEffect(() => {
    const load = async () => {
      setLoading(true);
      try {
        const res = await fetch(`${API_BASE_URL}Banners`);
        const data = await res.json();
        setItems(data || []);
      } catch (e) {
        console.error(e);
      } finally {
        setLoading(false);
      }
    };
    load();
  }, []);

  if (role !== "Admin" && role !== "Manager") {
    return <div className="p-6 text-center">غير مصرح لك بالدخول</div>;
  }

  const resetForm = () => {
    setForm({ id: 0, titleAr: "", titleEn: "", subTitleAr: "", subTitleEn: "", imageUrl: "", linkUrl: "", isActive: true, displayOrder: 0 });
    setImageFile(null);
    setImagePreview("");
  };

  const handleImageChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      setImageFile(file);
      const reader = new FileReader();
      reader.onloadend = () => {
        setImagePreview(reader.result);
      };
      reader.readAsDataURL(file);
    }
  };

  const uploadImage = async () => {
    if (!imageFile) return form.imageUrl;
    
    setUploadingImage(true);
    try {
      const formData = new FormData();
      formData.append("imageFile", imageFile);
      
      const headers = {};
      if (token) {
        headers["Authorization"] = `Bearer ${token}`;
      }
      // لا تضيف Content-Type header - المتصفح سيفعل ذلك تلقائياً مع FormData
      
      const res = await fetch(`${API_BASE_URL}Banners/UploadBannerImage`, {
        method: "POST",
        headers: headers,
        body: formData,
      });
      if (!res.ok) throw new Error("فشل رفع الصورة");
      const data = await res.json();
      return data.imageUrl;
    } catch (e) {
      console.error(e);
      alert("فشل رفع الصورة");
      return form.imageUrl;
    } finally {
      setUploadingImage(false);
    }
  };

  const submit = async (e) => {
    e.preventDefault();
    setSubmitting(true);
    try {
      // رفع الصورة إذا كان هناك ملف جديد
      let imageUrl = form.imageUrl;
      if (imageFile) {
        imageUrl = await uploadImage();
      }
      
      const method = form.id ? "PUT" : "POST";
      const url = form.id ? `${API_BASE_URL}Banners/${form.id}` : `${API_BASE_URL}Banners`;
      const res = await fetch(url, {
        method,
        headers: { "Content-Type": "application/json", Authorization: token ? `Bearer ${token}` : undefined },
        body: JSON.stringify({ ...form, imageUrl }),
      });
      if (!res.ok) throw new Error("failed");
      // reload
      const list = await (await fetch(`${API_BASE_URL}Banners`)).json();
      setItems(list || []);
      resetForm();
    } catch (e) {
      console.error(e);
      alert("حدث خطأ أثناء الحفظ");
    } finally {
      setSubmitting(false);
    }
  };

  const edit = (b) => {
    setForm({
      id: b.id,
      titleAr: b.titleAr || "",
      titleEn: b.titleEn || "",
      subTitleAr: b.subTitleAr || "",
      subTitleEn: b.subTitleEn || "",
      imageUrl: b.imageUrl || "",
      linkUrl: b.linkUrl || "",
      isActive: !!b.isActive,
      displayOrder: b.displayOrder || 0,
    });
    setImageFile(null);
    setImagePreview(b.imageUrl || "");
  };

  const remove = async (id) => {
    if (!confirm("حذف البانر؟")) return;
    try {
      const res = await fetch(`${API_BASE_URL}Banners/${id}`, { method: "DELETE", headers: { Authorization: token ? `Bearer ${token}` : undefined } });
      if (!res.ok) throw new Error("failed");
      setItems((prev) => prev.filter((x) => x.id !== id));
    } catch (e) {
      console.error(e);
    }
  };

  return (
    <div className="max-w-6xl mx-auto p-3 md:p-6">
      <div className="rounded-2xl p-4 md:p-6 mb-5 shadow-lg border bg-[#F9F6EF]">
        <div className="flex flex-col items-center mb-4">
          <WebSiteLogo width={300} height={100} className="mb-4" />
        </div>
        <h1 className="text-2xl md:text-3xl font-extrabold text-[#0A2C52] tracking-wide text-center">{t("bannersAdmin", "إدارة البانرز")}</h1>
        <p className="text-[#0A2C52]/80 mt-1 text-center">{t("list", "القائمة")}</p>
      </div>

      <form onSubmit={submit} className="bg-[#F9F6EF] rounded-2xl shadow p-3 md:p-5 grid grid-cols-1 md:grid-cols-2 gap-4 border border-[#0A2C52]/20">
        <div>
          <label className="block text-sm mb-1 font-semibold text-[#0A2C52]">العنوان (عربي) <span className="text-red-500">*</span></label>
          <input className="border border-[#0A2C52]/30 rounded w-full px-3 py-2 bg-white text-[#0A2C52]" value={form.titleAr} onChange={(e) => setForm({ ...form, titleAr: e.target.value })} required />
        </div>
        <div>
          <label className="block text-sm mb-1 font-semibold text-[#0A2C52]">العنوان (إنجليزي) <span className="text-red-500">*</span></label>
          <input className="border border-[#0A2C52]/30 rounded w-full px-3 py-2 bg-white text-[#0A2C52]" value={form.titleEn} onChange={(e) => setForm({ ...form, titleEn: e.target.value })} required />
        </div>
        <div>
          <label className="block text-sm mb-1 font-semibold text-[#0A2C52]">النص الفرعي (عربي)</label>
          <input className="border border-[#0A2C52]/30 rounded w-full px-3 py-2 bg-white text-[#0A2C52]" value={form.subTitleAr} onChange={(e) => setForm({ ...form, subTitleAr: e.target.value })} />
        </div>
        <div>
          <label className="block text-sm mb-1 font-semibold text-[#0A2C52]">النص الفرعي (إنجليزي)</label>
          <input className="border border-[#0A2C52]/30 rounded w-full px-3 py-2 bg-white text-[#0A2C52]" value={form.subTitleEn} onChange={(e) => setForm({ ...form, subTitleEn: e.target.value })} />
        </div>
        <div className="md:col-span-2">
          <label className="block text-sm mb-1 font-semibold text-[#0A2C52]">{t("bannerImage", "صورة البانر")}</label>
          <input 
            type="file" 
            accept="image/*" 
            className="border border-[#0A2C52]/30 rounded w-full px-3 py-2 bg-white text-[#0A2C52]" 
            onChange={handleImageChange}
            required={!form.id}
          />
          {imagePreview && (
            <div className="mt-2">
              <img src={imagePreview} alt="Preview" className="max-w-full h-32 object-contain border rounded" />
            </div>
          )}
          {form.imageUrl && !imagePreview && (
            <div className="mt-2">
              <img src={`${ServerPath}${form.imageUrl}`} alt="Current" className="max-w-full h-32 object-contain border rounded" />
            </div>
          )}
        </div>
        <div className="md:col-span-2">
          <label className="block text-sm mb-1 font-semibold text-[#0A2C52]">{t("clickUrl", "رابط عند الضغط (اختياري)")}</label>
          <input className="border border-[#0A2C52]/30 rounded w-full px-3 py-2 bg-white text-[#0A2C52]" value={form.linkUrl} onChange={(e) => setForm({ ...form, linkUrl: e.target.value })} />
        </div>
        <div>
          <label className="block text-sm mb-1 font-semibold text-[#0A2C52]">{t("displayOrder", "ترتيب العرض")}</label>
          <input type="number" className="border border-[#0A2C52]/30 rounded w-full px-3 py-2 bg-white text-[#0A2C52]" value={form.displayOrder} onChange={(e) => setForm({ ...form, displayOrder: Number(e.target.value) })} />
        </div>
        <div className="flex items-center gap-2">
          <input id="isActive" type="checkbox" checked={form.isActive} onChange={(e) => setForm({ ...form, isActive: e.target.checked })} />
          <label htmlFor="isActive" className="font-semibold text-[#0A2C52]">{t("active", "مفعل")}</label>
        </div>
        <div className="md:col-span-2 flex gap-2 justify-start">
          <button disabled={submitting || uploadingImage} className="bg-[#0A2C52] hover:bg-[#13345d] text-white font-bold px-5 py-2.5 rounded-xl transition-all shadow-md disabled:opacity-50 disabled:cursor-not-allowed">
            {uploadingImage ? t("uploadingImage", "جاري رفع الصورة...") : submitting ? t("saving", "جاري الحفظ...") : (form.id ? t("edit", "تعديل") : t("add", "إضافة"))}
          </button>
          {form.id ? (
            <button type="button" className="px-5 py-2.5 rounded-xl bg-gray-200 hover:bg-gray-300 text-gray-800 font-semibold transition-all shadow-md" onClick={resetForm}>{t("cancel", "إلغاء")}</button>
          ) : null}
        </div>
      </form>

      <div className="mt-6">
        <h2 className="text-xl font-semibold mb-2">{t("list", "القائمة")}</h2>
        {loading ? (
          <div className="p-4 text-center">{t("loading", "جاري التحميل...")}</div>
        ) : items.length === 0 ? (
          <div className="p-4 text-center">{t("noBanners", "لا توجد بانرز")}</div>
        ) : (
          <table className="min-w-full bg-[#F9F6EF] rounded-2xl shadow overflow-hidden border border-[#0A2C52]/20">
      <thead className="bg-[#0A2C52] text-white">
  <tr>
    <th className="text-right p-3 font-bold">#</th>
    <th className="text-right p-3 font-bold">{t("title", "العنوان")}</th>
    <th className="text-right p-3 font-bold">{t("displayOrder", "ترتيب العرض")}</th>
    <th className="text-right p-3 font-bold">{t("status", "حالة")}</th>
    <th className="text-right p-3 font-bold">{t("actions", "إجراءات")}</th>
  </tr>
</thead>

            <tbody>
              {items.map((b) => (
                <tr key={b.id} className="border-t border-[#0A2C52]/20 hover:bg-[#F9F6EF]">
                  <td className="p-3 text-[#0A2C52] font-medium">{b.id}</td>
                  <td className="p-3 text-[#0A2C52] font-medium">{b.titleAr || b.titleEn || "—"}</td>
                  <td className="p-3 text-[#0A2C52] font-medium">{b.displayOrder}</td>
                  <td className="p-3 text-[#0A2C52] font-medium">{b.isActive ? t("active", "مفعل") : "—"}</td>
                  <td className="p-3 flex gap-2">
                    <button className="px-4 py-2 rounded-xl bg-[#0A2C52] hover:bg-[#13345d] text-white font-semibold transition-all shadow-md" onClick={() => edit(b)}>{t("edit", "تعديل")}</button>
                    <button className="px-4 py-2 rounded-xl bg-red-600 hover:bg-red-700 text-white font-semibold transition-all shadow-md" onClick={() => remove(b.id)}>{t("delete", "حذف")}</button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </div>
  );
}


