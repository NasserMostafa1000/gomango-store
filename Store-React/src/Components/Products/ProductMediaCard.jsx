import React from "react";

export default function ProductMediaCard({ imageUrl, productName }) {
  return (
    <div className="bg-[#FAFAFA] rounded-2xl shadow-xl p-6 flex items-center justify-center h-full">
      <img
        src={imageUrl}
        alt={productName}
        className="w-full max-w-md h-auto object-contain rounded-xl"
      />
    </div>
  );
}

