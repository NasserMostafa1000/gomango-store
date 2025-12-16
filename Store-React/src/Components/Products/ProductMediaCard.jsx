import React, { useState, useEffect, useRef } from "react";
import { FiChevronLeft, FiChevronRight } from "react-icons/fi";

export default function ProductMediaCard({ imageUrl, productName, additionalImages = [] }) {
  const [selectedImageIndex, setSelectedImageIndex] = useState(0);
  const allImages = [imageUrl, ...additionalImages].filter(Boolean);
  const currentImage = allImages[selectedImageIndex] || imageUrl;
  const thumbnailContainerRef = useRef(null);
  const touchStartX = useRef(0);
  const touchEndX = useRef(0);
  
  // Zoom on hover states
  const [isHovering, setIsHovering] = useState(false);
  const [mousePosition, setMousePosition] = useState({ x: 0, y: 0 });
  const [zoomPosition, setZoomPosition] = useState({ x: 0, y: 0 });
  const imageContainerRef = useRef(null);
  const imageRef = useRef(null);
  const zoomContainerRef = useRef(null);

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

  // Handle touch events for swipe
  const handleTouchStart = (e) => {
    touchStartX.current = e.touches[0].clientX;
  };

  const handleTouchMove = (e) => {
    touchEndX.current = e.touches[0].clientX;
  };

  const handleTouchEnd = () => {
    if (!touchStartX.current || !touchEndX.current) return;
    
    const distance = touchStartX.current - touchEndX.current;
    const minSwipeDistance = 50; // Minimum distance for a swipe

    if (Math.abs(distance) > minSwipeDistance) {
      if (distance > 0) {
        // Swiped left, go to next
        goToNext();
      } else {
        // Swiped right, go to previous
        goToPrevious();
      }
    }

    // Reset
    touchStartX.current = 0;
    touchEndX.current = 0;
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

  // Handle mouse move for zoom effect (desktop only)
  const handleMouseMove = (e) => {
    // Only enable zoom on desktop screens (width >= 1024px)
    if (window.innerWidth < 1024) return;
    if (!imageContainerRef.current || !imageRef.current) return;
    
    const container = imageContainerRef.current;
    const rect = container.getBoundingClientRect();
    const x = ((e.clientX - rect.left) / rect.width) * 100;
    const y = ((e.clientY - rect.top) / rect.height) * 100;
    
    // Clamp values between 0 and 100
    const clampedX = Math.max(0, Math.min(100, x));
    const clampedY = Math.max(0, Math.min(100, y));
    
    setMousePosition({ x: clampedX, y: clampedY });
    
    // Calculate zoom position for the zoomed image (2x zoom = 200% background size)
    // When background-size is 200%, we need to adjust the position
    // The center of the zoom should be at the mouse position
    setZoomPosition({ 
      x: clampedX, 
      y: clampedY 
    });
  };

  const handleMouseEnter = () => {
    // Only enable zoom on desktop screens
    if (window.innerWidth >= 1024) {
      setIsHovering(true);
    }
  };

  const handleMouseLeave = () => {
    setIsHovering(false);
    setMousePosition({ x: 50, y: 50 }); // Reset to center
  };

  return (
    <div className="bg-[#FAFAFA] rounded-2xl shadow-xl p-6 h-full">
      {/* Main Image Container with Zoom */}
      <div className="flex flex-col lg:flex-row gap-4 mb-4 relative">
        {/* Original Image with Navigation Arrows */}
        <div 
          ref={imageContainerRef}
          className="relative flex items-center justify-center lg:aspect-square lg:h-[600px] lg:w-[600px] overflow-hidden rounded-xl lg:cursor-zoom-in flex-shrink-0"
          onTouchStart={handleTouchStart}
          onTouchMove={handleTouchMove}
          onTouchEnd={handleTouchEnd}
          onMouseMove={handleMouseMove}
          onMouseEnter={handleMouseEnter}
          onMouseLeave={handleMouseLeave}
        >
          {allImages.length > 1 && (
            <>
              <button
                onClick={goToPrevious}
                className="absolute left-2 z-20 bg-white/90 hover:bg-white rounded-full p-2 shadow-lg transition-all duration-200 hover:scale-110"
                aria-label="Previous image"
              >
                <FiChevronLeft className="w-6 h-6 text-gray-800" />
              </button>
              <button
                onClick={goToNext}
                className="absolute right-2 z-20 bg-white/90 hover:bg-white rounded-full p-2 shadow-lg transition-all duration-200 hover:scale-110"
                aria-label="Next image"
              >
                <FiChevronRight className="w-6 h-6 text-gray-800" />
              </button>
            </>
          )}
          <div className="relative w-full h-full overflow-hidden">
            <img
              ref={imageRef}
              src={currentImage}
              alt={productName}
              className="w-full h-full max-w-md lg:max-w-full object-contain rounded-xl select-none touch-none"
              draggable={false}
            />
            {/* Zoom lens indicator on original image */}
            {isHovering && (
              <div
                className="absolute pointer-events-none border-2 border-orange-500 bg-orange-500/20 rounded-full z-10"
                style={{
                  left: `${mousePosition.x}%`,
                  top: `${mousePosition.y}%`,
                  width: '100px',
                  height: '100px',
                  transform: 'translate(-50%, -50%)',
                }}
              />
            )}
          </div>
        </div>

        {/* Zoomed Image Container (Desktop only) */}
        {isHovering && (
          <div 
            ref={zoomContainerRef}
            className="hidden lg:flex items-center justify-center lg:aspect-square lg:h-[600px] lg:w-[600px] overflow-hidden rounded-xl border-2 border-gray-200 bg-white flex-shrink-0 shadow-lg z-[9999] relative"
          >
            <div 
              className="w-full h-full relative"
              style={{
                backgroundImage: `url(${currentImage})`,
                backgroundSize: '200%',
                backgroundPosition: `${zoomPosition.x}% ${zoomPosition.y}%`,
                backgroundRepeat: 'no-repeat',
                imageRendering: 'high-quality',
              }}
            >
              <img
                src={currentImage}
                alt={`${productName} - Zoomed`}
                className="w-full h-full object-contain opacity-0 pointer-events-none"
                draggable={false}
              />
            </div>
          </div>
        )}
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

