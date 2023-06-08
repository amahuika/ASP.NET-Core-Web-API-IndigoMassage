using Indigo.BuisnessLogic.Services;
using Indigo.DataAccess.Data;
using Indigo.Models.DTO;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace IndigoMassage.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class AvailabilitiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private ApiResponseDTO _response;
        private readonly IDtoMappingToModel _dtoMappingToModel;

        public AvailabilitiesController(ApplicationDbContext context, IDtoMappingToModel dtoMappingToModel)
        {
            _context = context;
            _response = new ApiResponseDTO();
            _dtoMappingToModel = dtoMappingToModel;
        }

        // GET: api/Availabilities
        [HttpGet]
        public async Task<ActionResult<ApiResponseDTO>> GetAvailabilities()
        {
            try
            {
                var availabilitiesFromDb = await _context.Availabilities.ToListAsync();
                // date: "2023-04-03",
                // start: "09:00",
                // end: "17:00",
                // format the date to be in the format of yyyy-MM-dd and the time to be in the format of hh:mm from the availabilities from database
                var availabilityResponse = availabilitiesFromDb.Select(a => new AvailabilityResponseDTO
                {
                    Id = a.Id,
                    Date = a.Date.ToString("yyyy-MM-dd"),
                    StartTime = a.StartTime.ToString("hh\\:mm"),
                    EndTime = a.EndTime.ToString("hh\\:mm")
                }).ToList();




                _response.Result = availabilityResponse;
                _response.StatusCode = HttpStatusCode.OK;
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

        // GET: api/Availabilities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDTO>> GetAvailability(int id)
        {
            try
            {
                var availability = await _context.Availabilities.FindAsync(id);

                if (availability == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                _response.Result = availability;
                _response.StatusCode = HttpStatusCode.OK;
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

        // PUT: api/Availabilities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAvailability(int id, [FromBody] AvailabilityDTO availabilityDto)
        {
            try
            {
                var availabilityFromDb = await _context.Availabilities.FindAsync(id);

                if (availabilityFromDb == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                DateTime date = DateTime.Parse(availabilityDto.Date);
                TimeSpan start = TimeSpan.Parse(availabilityDto.StartTime);
                TimeSpan end = TimeSpan.Parse(availabilityDto.EndTime);

                availabilityFromDb.Date = date;
                availabilityFromDb.StartTime = start;
                availabilityFromDb.EndTime = end;


                await _context.SaveChangesAsync();

                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.ErrorMessages.Add($"Error message: {ex.Message}");
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }


        }

        // POST: api/Availabilities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        [HttpPost]
        public async Task<ActionResult<ApiResponseDTO>> PostAvailability([FromBody] AvailabilityDTO availabilityDto)
        {
            _response.IsSuccess = false;
            _response.StatusCode = HttpStatusCode.BadRequest;
            try
            {
                if (ModelState.IsValid)
                {
                    // parse date string yyyy-mm-dd to datetime
                    DateTime date = DateTime.Parse(availabilityDto.Date);
                    TimeSpan start = TimeSpan.Parse(availabilityDto.StartTime);
                    TimeSpan end = TimeSpan.Parse(availabilityDto.EndTime);

                    // check if date is in the future
                    if (date.Date < DateTime.Now.Date)
                    {
                        _response.ErrorMessages.Add($"Date must be in the future");
                        return BadRequest(_response);
                    }




                    // get availability from db
                    var availabilityFromDb = await _context.Availabilities.FirstOrDefaultAsync(a => a.Date == date);

                    // check if there is already a availability for the same date
                    if (availabilityFromDb != null)
                    {
                        _response.ErrorMessages.Add("Availability already set for current date.");
                        return BadRequest(_response);
                    }


                    // check if new availability does not overlap with existing availability
                    if (availabilityFromDb != null)
                    {


                        if (start >= availabilityFromDb.StartTime && end <= availabilityFromDb.EndTime ||
                           end <= availabilityFromDb.EndTime && end >= availabilityFromDb.StartTime ||
                           start >= availabilityFromDb.StartTime && start <= availabilityFromDb.EndTime ||
                           start < availabilityFromDb.StartTime && end > availabilityFromDb.EndTime)
                        {
                            _response.ErrorMessages.Add($"Availability already exists");

                            return BadRequest(_response);
                        }


                    }

                    // access the user and get the user id

                    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (userId == null)
                    {
                        _response.ErrorMessages.Add($"User not found");
                        return BadRequest(_response);
                    }


                    // map the dto to the model
                    var availability = _dtoMappingToModel.MapToAvailability(date, start, end, userId);


                    // add the availability to the database
                    _context.Availabilities.Add(availability);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction("GetAvailability", new { id = availability.Id }, availability);
                }
            }
            catch (Exception ex)
            {
                _response.ErrorMessages.Add($"Error {ex.Message}");

                return BadRequest(_response);
            }

            return BadRequest(_response);
        }

        // DELETE: api/Availabilities/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDTO>> DeleteAvailability(int id)
        {
            if (id == 0)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            try
            {
                var availability = await _context.Availabilities.FindAsync(id);
                if (availability == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                // 


                _context.Availabilities.Remove(availability);
                await _context.SaveChangesAsync();

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


    }
}
