using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreBusinessLayer.Shipping;
using StoreServices.ShippingServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineStoreAPIs.Controllers
{
    [Route("api/ShippingInfo")]
    [ApiController]
    public class ShippingInfoController : ControllerBase
    {
        private readonly IShipping _shippingRepo;

        public ShippingInfoController(IShipping shippingBL)
        {
            _shippingRepo = shippingBL;
        }

        /// <summary>
        /// إرجاع قائمة بأسعار الشحن حسب المحافظة
        /// </summary>
        [HttpGet("GetShippingInfo")]
        public async Task<ActionResult> GetShippingInfo()
        {
            try
            {
                var shippingData = await _shippingRepo.GetShippingInfo();

                if (shippingData == null || shippingData.Count == 0)
                    return NotFound("لم يتم العثور على بيانات الشحن.");

                return Ok(shippingData);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"خطأ في الخادم: {ex.Message}");
            }
        }

        /// <summary>
        /// تحديث سعر الشحن لمحافظة معينة
        /// </summary>
        /// 
        [HttpPut("UpdateShippingPrice")]
        [Authorize(Roles = "Admin,Manager,Shipping Manager")]
        public async Task<ActionResult> UpdateShippingPrice(string Governorate,decimal NewPrice)
        {
            try
            {

                var isUpdated = await _shippingRepo.UpdateShippingPrice(Governorate, NewPrice);

                if (!isUpdated)
                    return NotFound($"لم يتم العثور على المحافظة: {Governorate}");

                return Ok(new { message = "تم تحديث السعر بنجاح" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"خطأ في الخادم: {ex.Message}");
            }
        }

        /// <summary>
        /// تحديث فترة الوصول لمنطقة شحن معينة
        /// </summary>
        [HttpPut("UpdateDeliveryTime")]
        [Authorize(Roles = "Admin,Manager,Shipping Manager")]
        public async Task<ActionResult> UpdateDeliveryTime(string Governorate, int DeliveryTimeDays)
        {
            try
            {
                var isUpdated = await _shippingRepo.UpdateDeliveryTime(Governorate, DeliveryTimeDays);

                if (!isUpdated)
                    return NotFound($"لم يتم العثور على المحافظة: {Governorate}");

                return Ok(new { message = "تم تحديث فترة الوصول بنجاح" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"خطأ في الخادم: {ex.Message}");
            }
        }

        /// <summary>
        /// إضافة منطقة شحن جديدة
        /// </summary>
        [HttpPost("AddShippingArea")]
        [Authorize(Roles = "Admin,Manager,Shipping Manager")]
        public async Task<ActionResult> AddShippingArea(string Governorate, decimal Price, int DeliveryTimeDays)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Governorate))
                    return BadRequest("اسم المحافظة مطلوب");

                if (Price <= 0)
                    return BadRequest("السعر يجب أن يكون أكبر من 0");

                if (DeliveryTimeDays <= 0)
                    return BadRequest("فترة الوصول يجب أن تكون أكبر من 0");

                var isAdded = await _shippingRepo.AddShippingArea(Governorate, Price, DeliveryTimeDays);

                if (!isAdded)
                    return Conflict($"المحافظة '{Governorate}' موجودة بالفعل");

                return Ok(new { message = "تمت إضافة المنطقة بنجاح" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"خطأ في الخادم: {ex.Message}");
            }
        }

        /// <summary>
        /// حذف منطقة شحن
        /// </summary>
        [HttpDelete("DeleteShippingArea")]
        [Authorize(Roles = "Admin,Manager,Shipping Manager")]
        public async Task<ActionResult> DeleteShippingArea(string Governorate)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Governorate))
                    return BadRequest("اسم المحافظة مطلوب");

                var isDeleted = await _shippingRepo.DeleteShippingArea(Governorate);

                if (!isDeleted)
                    return NotFound($"لم يتم العثور على المحافظة: {Governorate}");

                return Ok(new { message = "تم حذف المنطقة بنجاح" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"خطأ في الخادم: {ex.Message}");
            }
        }

        /// <summary>
        /// إعادة ضبط بيانات الشحن إلى الإمارات السبع (AED)
        /// </summary>
        [HttpPost("ResetToEmirates")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> ResetToEmirates()
        {
            try
            {
                await _shippingRepo.ResetToEmiratesAsync();
                return Ok(new { message = "تمت إعادة ضبط الإمارات السبع بنجاح." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"خطأ في الخادم: {ex.Message}");
            }
        }
    }
}
