using Microsoft.AspNetCore.Mvc;
using StoreServices.CurrencyServices;

namespace OnlineStoreAPIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrency _currency;
        public CurrencyController(ICurrency currency)
        {
            _currency = currency;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _currency.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> Get(string code)
        {
            var item = await _currency.GetByCodeAsync(code);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Upsert([FromBody] CurrencyRateDto dto)
        {
            var saved = await _currency.UpsertAsync(dto);
            return Ok(saved);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var ok = await _currency.DeactivateAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}


