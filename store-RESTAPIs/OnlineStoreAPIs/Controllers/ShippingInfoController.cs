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
        [Authorize(Roles = "Admin,Manager,Shipping Manager")]

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
