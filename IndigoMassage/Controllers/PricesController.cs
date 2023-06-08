using Indigo.BuisnessLogic.Services;
using Indigo.DataAccess.Data;
using Indigo.Models.DTO;
using Indigo.Models.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace IndigoMassage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PricesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IDtoMappingToModel _dtoMapToModel;
        private ApiResponseDTO _response;

        public PricesController(ApplicationDbContext context, IDtoMappingToModel dtoMappingToModel)
        {
            _context = context;
            _response = new ApiResponseDTO();
            _dtoMapToModel = dtoMappingToModel;
        }

        // GET: api/Prices
        [HttpGet]
        public async Task<ActionResult<ApiResponseDTO>> GetPrices()
        {
            try
            {

                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = await _context.Prices.ToListAsync();
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add(ex.Message);
                return BadRequest(_response);
            }

        }

        // GET: api/Prices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Price>> GetPrice(int id)
        {

            if (id == 0)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            try
            {
                var price = await _context.Prices.FindAsync(id);

                if (price == null)
                {
                    _response.ErrorMessages.Add("Could not find Price");
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = price;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add($"Error message: {ex.Message}");

                return BadRequest(_response);
            }
        }

        // PUT: api/Prices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrice(int id, [FromBody] Price price)
        {
            if (id != price.Id)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("Id does not match");
                return BadRequest(_response);
            }
            if (!ModelState.IsValid)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }


            _context.Entry(price).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = price;
                return Ok(_response);
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add($"Something went wrong. Error: {ex.Message}");
                return BadRequest(_response);
            }


        }

        // POST: api/Prices
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<ActionResult<Price>> PostPrice([FromBody] PriceDTO priceDto)
        {
            if (ModelState.IsValid)
            {
                try
                {


                    // map dto to model
                    var newPrice = _dtoMapToModel.MapToPriceModel(priceDto);

                    _context.Prices.Add(newPrice);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction("GetPrice", new { id = newPrice.Id }, newPrice);

                }
                catch (Exception ex)
                {
                    _response.ErrorMessages.Add($"Something went wrong. Error: {ex.Message}");
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest();
                }


            }

            _response.IsSuccess = false;
            _response.StatusCode = HttpStatusCode.BadRequest;

            return BadRequest(_response);
        }

        // DELETE: api/Prices/5
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrice(int id)
        {
            if (id == 0)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
            try
            {
                var price = await _context.Prices.FindAsync(id);
                if (price == null)
                {
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Could not find Price");
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);

                }

                _context.Prices.Remove(price);
                await _context.SaveChangesAsync();

                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add($"Error message: {ex.Message}");
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);

            }
        }

        // get all prices by service id
        [HttpGet("service/{id}")]
        public async Task<ActionResult<ApiResponseDTO>> GetPricesByServiceId(int id)
        {
            if (id == 0)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
            try
            {
                var prices = await _context.Prices.Where(p => p.ServiceId == id).ToListAsync();

                if (prices == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages.Add("Could not find any prices");
                    return NotFound(_response);
                }

                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = prices;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add($"Error message: {ex.Message}");
                return BadRequest(_response);
            }
        }



        private bool PriceExists(int id)
        {
            return (_context.Prices?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
