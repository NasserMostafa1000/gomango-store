using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreBusinessLayer.Clients;
using StoreBusinessLayer.Users;
using StoreServices.ClientsServices;
using static StoreBusinessLayer.Clients.ClientsDtos;

namespace OnlineStoreAPIs.Controllers
{
    [Route("api/Clients")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClient _ClientsRepo;
        public ClientsController(IClient ClientsRepo)
        {
            _ClientsRepo = ClientsRepo;
        }

        [HttpPost("PostClient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddNewClient(ClientsDtos.PostClientReq dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "معلومات مفقوده تأكد من انك قد ارسلت كل المعلومات" });
            }
            try
            {
                return Ok(new {ID=  await _ClientsRepo.AddNewClient(dto)});

            }
            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message.ToString() });
            }
           
        }


        [HttpGet("Count")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult>Count()
        {
            try
            {
                return Ok(new { Count = await _ClientsRepo.Count() });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message.ToString() });
                            }
        }

        [HttpGet("GetAddresses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "User,Admin,Manager,Shipping Man")]
        public async Task<ActionResult>GetClientAddresses()
        {
            int UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            int ClientId =await _ClientsRepo.GetClientIdByUserId(UserId);
            if(ClientId>=1)
            return Ok(new { Addresses =await _ClientsRepo.GetClientAddresses(ClientId) });
             else
            {
                return NotFound(nameof(ClientId));
            }
        }
        [HttpPost("PostNewAddress")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "User,Admin,Manager,Shipping Man")]
        public async Task<ActionResult> PostAddress(ClientsDtos.PostAddressReq req)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            int UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            int ClientId = await _ClientsRepo.GetClientIdByUserId(UserId);
            if(ClientId>=1)
            return Ok(new { AddressId = await _ClientsRepo.AddNewAddress(req, ClientId) });
            else
            {
                return NotFound(nameof(ClientId));
            }
        }


        [HttpGet("GetClientAddresses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "User,Admin,Manager,Shipping Man")]
        public async Task<ActionResult> GetClientPhoneAndAddresses()
        {
            try
            {

            int UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            int ClientId = await _ClientsRepo.GetClientIdByUserId(UserId);
                if (ClientId >= 1) return Ok(new { Addresses = await _ClientsRepo.GetClientAddresses(ClientId) });
                else
                {
                    return NotFound( nameof(ClientId));
                }
            }catch(Exception ex)
            {
                return StatusCode(500,new {message=ex.Message.ToString() });

            }
        }
        [HttpGet("GetClientPhone")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "User,Admin,Manager,Shipping Man")]
        public async Task<ActionResult> GetClientPhone()
        {
            try
            {
            int UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            int ClientId = await _ClientsRepo.GetClientIdByUserId(UserId);
            if (ClientId >= 0)
            {
                var PhoneNumber  = await _ClientsRepo.GetClientPhoneNumberById(ClientId);
                if(PhoneNumber!=null)
                {
                  return Ok(new{ phoneNumber= await _ClientsRepo.GetClientPhoneNumberById(ClientId)});
                }
                    return NotFound(nameof(PhoneNumber));
            }
                return NotFound(nameof(ClientId));

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message.ToString() });
            }
        }

        [HttpPost("PostClientPhone")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "User,Admin,Manager,Shipping Man")]
        public async Task<ActionResult> PostClientPhone(string PhoneNumber)
        {
            try
            {
            int UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            int ClientId = await _ClientsRepo.GetClientIdByUserId(UserId);
            if(ClientId>=0)
            {
              return Ok(await _ClientsRepo.AddOrUpdatePhoneToClientById(ClientId, PhoneNumber));
            }

            return NotFound(nameof(ClientId));
            }catch(Exception ex)
            {
                return StatusCode(500, new { message = ex.Message.ToString() });
            }

        }
        [HttpGet("GetClientById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "User,Admin,Manager,Shipping Man")]
        public async Task<ActionResult>GetClientById()
        {
            try
            {

            int UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            int ClientId = await _ClientsRepo.GetClientIdByUserId(UserId);
            if (ClientId >= 0)
            {
                return Ok(await _ClientsRepo.GetClientById(ClientId));
            }
                return NotFound(nameof(ClientId));
            }catch(Exception ex)
            {
                return StatusCode(500,new { message = ex.Message.ToString() });
            }


        }


        [HttpPut("PutClientName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "User,Admin,Manager,Shipping Man")]
        public async Task<ActionResult> GetClientName(string FirstName,string LastName)
        {
            try
            {

                int UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                int ClientId = await _ClientsRepo.GetClientIdByUserId(UserId);
                if (ClientId >= 0)
                {
                    bool IsUpdated = await _ClientsRepo.UpdateClientName(FirstName, LastName, ClientId);
                    return IsUpdated? Ok(): StatusCode(500, new { message = "An error occurred while updating names" });
                }
                return NotFound(nameof(ClientId));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message.ToString() });
            }


        }

        [HttpGet("GetClients")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetClients(int PageNum)
        {
            try
            {
                var clients = await _ClientsRepo.GetClientsAsync(PageNum);
                if (clients == null || !clients.Any())
                    return NotFound(new { message = "لا يوجد عملاء"});
                return Ok(clients);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message.ToString() });
            }
        }



    }
}
