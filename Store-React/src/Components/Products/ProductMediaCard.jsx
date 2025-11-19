import React, { useState, useEffect } from "react";

export default function ProductMediaCard({ imageUrl, productName, additionalImages = [] }) {
  const [selectedImageIndex, setSelectedImageIndex] = useState(0);
  const allImages = [imageUrl, ...additionalImages].filter(Boolean);
  const currentImage = allImages[selectedImageIndex] || imageUrl;

  // Reset selected image when images change
  useEffect(() => {
    setSelectedImageIndex(0);
  }, [imageUrl, additionalImages.join(',')]);

  return (
    <div className="bg-[#FAFAFA] rounded-2xl shadow-xl p-6 h-full">
      {/* Main Image */}
      <div className="flex items-center justify-center mb-4">
        <img
          src={currentImage}
          alt={productName}
          className="w-full max-w-md h-auto object-contain rounded-xl"
        />
      </div>
      
      {/* Thumbnail Images */}
      {allImages.length > 1 && (
        <div className="flex gap-2 justify-center flex-wrap mt-4">
          {allImages.map((img, index) => (
            <button
              key={index}
              onClick={() => setSelectedImageIndex(index)}
              className={`w-16 h-16 rounded-lg overflow-hidden border-2 transition-all ${
                selectedImageIndex === index
                  ? "border-[#F55A00] ring-2 ring-[#F55A00]"
                  : "border-gray-300 hover:border-[#F55A00]"
              }`}
            >
              <img
                src={img}
                alt={`${productName} - ${index + 1}`}
                className="w-full h-full object-cover"
              />
            </button>
          ))}
        </div>
      )}
    </div>
  );
}

