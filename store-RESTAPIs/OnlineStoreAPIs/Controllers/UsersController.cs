using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreBusinessLayer.Clients;
using StoreBusinessLayer.Users;
using StoreDataAccessLayer;
using StoreDataAccessLayer.Entities;
using StoreServices.ClientsServices;
using StoreServices.ILoginServices;
using StoreServices.UsersServices;

namespace OnlineStoreAPIs.Controllers
{
    [Route("api/Users")]
    [ApiController]

    public class UsersController : ControllerBase
    {
        private readonly IUser _usersRepo;
        private readonly IClient _ClientsRepo;

        public UsersController(IUser usersBL,IClient clients)
        {
            _usersRepo = usersBL;
            _ClientsRepo = clients;

        }
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Login(UsersDtos.LoginReq req)
        {
            if (string.IsNullOrEmpty(req.Token) && string.IsNullOrEmpty(req.Email))
            {
                return BadRequest(new { Message = "معلومات هامة مفقودة" });
            }
            try
            {
                return Ok(new { Token = await _usersRepo.Login(req) });
            }
            catch (Exception ex)
            {

                return BadRequest(new { Message = ex.Message.ToString() });
            }
        }
        [HttpPost("ForgotPassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ForgotPassword(UsersDtos.ForgotPasswordReq req)
        {
            if (string.IsNullOrEmpty(req.AuthProvider) || string.IsNullOrEmpty(req.UserProviderIdentifier))
            {
                // إرجاع رسالة خطأ واضحة في حال كان أحد التفاصيل مفقودًا
                return BadRequest(new { Message = "معلومات هامه مفقوده" });
            }

            try
            {
                // استدعاء الميثود الخاصة بإعادة تعيين كلمة المرور وإرسال إشعار
                bool IsNotificationSentSuccess = await _usersRepo.NotificationForForgotPassword(req.UserProviderIdentifier, req.AuthProvider);

                if (IsNotificationSentSuccess)
                {
                    return Ok(new { message = $"تم ارسال البسورد الجديد الي  {req.AuthProvider} " });
                }
                else
                {
                    return BadRequest(new { message = "فشل في ارسال البسورد الجديد" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message.ToString() });
            }
        }

        [HttpGet("GetUserInfo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles="User,Admin,Manager,Shipping Manager")]
        public async Task<ActionResult> GetUserInfo()
        {
            try
            {
                int UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (UserId >= 1) return Ok(await _usersRepo.GetUserInfo(UserId));
                else return NotFound(nameof(UserId));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message.ToString() });
            }
        }
        [HttpPut("ChangePassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ChangePassword(UsersDtos.ChangePasswordReq req)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "معلومات هامه مفقوده" });
            }

            try
            {
                if(string.IsNullOrWhiteSpace(req.CurrentPassword))
                {
                    var result =await _usersRepo.SetPasswordForFirstTime( req.NewPassword, req.Email);
                    return result != null ? Ok() : StatusCode(500, "error occurred");
                }
                bool IsUpdated = await _usersRepo.ChangeOrForgotPasswordAsync(req.Email, req.NewPassword, false, req.CurrentPassword);
                return IsUpdated ? Ok() : StatusCode(500, "error while updating Password");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message.ToString() });
            }
        }

        [HttpGet("get-Employees")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize (Roles="Admin")]
        public async Task<ActionResult> GetManagers()
        {
            try
            {
                var managers = await _usersRepo.GetEmployees();
                return Ok(managers);
            }catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message.ToString() });
            }
        }
        [HttpPost("PostManager")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> RegisterManager(UsersDtos.PostUserReq userToAdd)
        {

            try
            {
                //role id for manager =2
                int userId = await _usersRepo.PostUser(userToAdd, 2);
                return Ok(new { UserId = userId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("PostShippingMan")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> RegisterShippingMan(UsersDtos.PostUserReq userToAdd)
        {

            try
            {
                //role id for Shipping Man =4
                int userId = await _usersRepo.PostUser(userToAdd, 4);
                //   return Ok(new { UserId = userId });
                return Ok(new { UserId = userId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("PostCashierMan")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> RegisterCashierMan(UsersDtos.PostUserReq userToAdd)
        {

            try
            {
                //role id for Shipping Man =4
                int userId = await _usersRepo.PostUser(userToAdd, 5);
                //   return Ok(new { UserId = userId });
                return Ok(new { UserId = userId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("PostTechnicalSupport")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> RegisterTechnicalSupport(UsersDtos.PostUserReq userToAdd)
        {

            try
            {
                //role id for Shipping Man =4
                int userId = await _usersRepo.PostUser(userToAdd, 6);
                //   return Ok(new { UserId = userId });
                return Ok(new { UserId = userId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpDelete("RemoveEmployee")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles ="Admin,Manager")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> RemoveEmployee([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { message = "يجب إدخال البريد الإلكتروني" });
            }

            bool isDeleted = await _usersRepo.RemoveManager(email);
            if (!isDeleted)
            {
                return NotFound(new { message = "لم يتم العثور على الموظف" });
            }

            return Ok(new { message = "تم حذف المدير بنجاح" });
        }

    }
}
    

