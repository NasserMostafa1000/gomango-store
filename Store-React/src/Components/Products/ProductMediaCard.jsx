import React, { useState, useEffect, useRef } from "react";
import { FiChevronLeft, FiChevronRight } from "react-icons/fi";

export default function ProductMediaCard({ imageUrl, productName, additionalImages = [] }) {
  const [selectedImageIndex, setSelectedImageIndex] = useState(0);
  const allImages = [imageUrl, ...additionalImages].filter(Boolean);
  const currentImage = allImages[selectedImageIndex] || imageUrl;
  const thumbnailContainerRef = useRef(null);

  // Reset selected image when images change
  useEffect(() => {
    setSelectedImageIndex(0);
  }, [imageUrl, additionalImages.join(',')]);

  const goToPrevious = () => {
    if (selectedImageIndex > 0) {
      setSelectedImageIndex(selectedImageIndex - 1);
    } else {
      setSelectedImageIndex(allImages.length - 1);
    }
  };

  const goToNext = () => {
    if (selectedImageIndex < allImages.length - 1) {
      setSelectedImageIndex(selectedImageIndex + 1);
    } else {
      setSelectedImageIndex(0);
    }
  };

  // Scroll thumbnail into view when image changes
  useEffect(() => {
    if (thumbnailContainerRef.current && allImages.length > 1) {
      const thumbnail = thumbnailContainerRef.current.children[selectedImageIndex];
      if (thumbnail) {
        thumbnail.scrollIntoView({ behavior: 'smooth', block: 'nearest', inline: 'center' });
      }
    }
  }, [selectedImageIndex, allImages.length]);

  return (
    <div className="bg-[#FAFAFA] rounded-2xl shadow-xl p-6 h-full">
      {/* Main Image with Navigation Arrows */}
      <div className="relative flex items-center justify-center mb-4">
        {allImages.length > 1 && (
          <>
            <button
              onClick={goToPrevious}
              className="absolute left-2 z-10 bg-white/90 hover:bg-white rounded-full p-2 shadow-lg transition-all duration-200 hover:scale-110"
              aria-label="Previous image"
            >
              <FiChevronLeft className="w-6 h-6 text-gray-800" />
            </button>
            <button
              onClick={goToNext}
              className="absolute right-2 z-10 bg-white/90 hover:bg-white rounded-full p-2 shadow-lg transition-all duration-200 hover:scale-110"
              aria-label="Next image"
            >
              <FiChevronRight className="w-6 h-6 text-gray-800" />
            </button>
          </>
        )}
        <img
          src={currentImage}
          alt={productName}
          className="w-full max-w-md h-auto object-contain rounded-xl"
        />
      </div>
      
      {/* Thumbnail Images */}
      {allImages.length > 1 && (
        <div className="mt-4">
          <div 
            ref={thumbnailContainerRef}
            className="flex gap-2 justify-start overflow-x-auto scrollbar-hide pb-2 snap-x snap-mandatory" 
            style={{ 
              scrollbarWidth: 'thin', 
              scrollbarColor: '#cbd5e0 transparent',
              direction: 'ltr',
              WebkitOverflowScrolling: 'touch'
            }}
            onWheel={(e) => {
              e.preventDefault();
              const container = e.currentTarget;
              container.scrollLeft += e.deltaY;
            }}
          >
            {allImages.map((img, index) => (
              <button
                key={index}
                onClick={() => setSelectedImageIndex(index)}
                className={`flex-shrink-0 w-16 h-16 rounded-lg overflow-hidden border-2 transition-all snap-start ${
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
        </div>
      )}
    </div>
  );
}

