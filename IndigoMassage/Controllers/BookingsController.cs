using Indigo.BuisnessLogic.Services;
using Indigo.BuisnessLogic.Utilities;
using Indigo.DataAccess.Data;
using Indigo.Models.DTO;
using Indigo.Models.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net;

namespace IndigoMassage.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBookingService _bookingsService;
        private readonly IEmailSender _emailSender;
        private readonly IDtoMappingToModel _dtoMappingToModel;
        private ApiResponseDTO _response;

        public BookingsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBookingService bookingService, IEmailSender emailSender, IDtoMappingToModel dtoMappingToModel)
        {
            _context = context;
            _response = new ApiResponseDTO();
            _userManager = userManager;
            _bookingsService = bookingService;
            _emailSender = emailSender;
            _dtoMappingToModel = dtoMappingToModel;
        }

        // GET: api/ServiceBookings
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<ActionResult<ApiResponseDTO>> GetServicesBookings()
        {
            try
            {
                // create a list of booking response dto's
                IEnumerable<BookingResponseDTO> bookingResponseDTOs;

                // get all bookings from db
                var bookingsFromDb = await _context.Bookings.Include(b => b.User).Include(b => b.Service).Include(b => b.Price).ToListAsync();

                // if no bookings found, return error message
                if (bookingsFromDb == null || bookingsFromDb.Count == 0)
                {
                    _response.ErrorMessages.Add("No bookings found");
                    _response.Result = bookingsFromDb;
                }
                else
                {

                    // map each booking to a booking response dto
                    bookingResponseDTOs = bookingsFromDb.Select(b => new BookingResponseDTO
                    {
                        BookingId = b.Id,
                        FirstName = b.User.FirstName,
                        LastName = b.User.LastName,
                        Email = b.User.Email,
                        PhoneNumber = b.User.PhoneNumber,
                        Notes = b.Notes,
                        serviceName = b.Service.Name,
                        ServiceId = b.ServiceId,
                        PriceId = b.PriceId,
                        DisplayDate = b.Date.ToString("ddd, dd MMM yyyy"),
                        Date = b.Date.ToString("yyyy-MM-dd"),
                        StartTime = b.StartTime.ToString("hh\\:mm"),
                        EndTime = updateEndTime(b.Price.Duration, b.EndTime),
                        Cost = b.Price.Cost.ToString("C"),
                        Duration = b.Price.Duration.ToString(),
                        Status = b.Status,
                        Time = $"{_bookingsService.ChangeTimeFormat(b.StartTime)} - {_bookingsService.ChangeTimeFormat(b.Price.Duration >= 90 ? b.EndTime.Subtract(TimeSpan.FromMinutes(30)) : b.EndTime.Subtract(TimeSpan.FromMinutes(15)))}"


                    });
                    _response.Result = bookingResponseDTOs;
                }

                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add($"Error message: {ex.Message}");
                _response.IsSuccess = false;
                return BadRequest(_response);
            }
        }

        // GET: api/ServiceBookings/5
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDTO>> GetServiceBookings(int id)
        {

            var serviceBookings = await _context.Bookings.FindAsync(id);

            if (serviceBookings == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = serviceBookings;
            _response.IsSuccess = true;

            return Ok(_response);
        }

        // PUT: api/ServiceBookings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServiceBookings(int id, BookingUpdateDTO bookingUpdateDTO)
        {
            if (!ModelState.IsValid)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;

                return BadRequest(_response);
            }

            if (id == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }
            try
            {
                var bookingFromDb = await _context.Bookings.Include(b => b.User).Include(b => b.Service).Include(b => b.Price).FirstOrDefaultAsync(b => b.Id == id);
                if (bookingFromDb == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("No booking found in database");
                    return BadRequest(_response);
                }
                if (bookingUpdateDTO.Status != null)
                {
                    bookingFromDb.Status = bookingUpdateDTO.Status;
                    await _context.SaveChangesAsync();

                    BookingResponseDTO bookingResponse = new BookingResponseDTO()
                    {
                        FirstName = bookingFromDb.User.FirstName,
                        LastName = bookingFromDb.User.LastName,
                        Email = bookingFromDb.User.Email,
                        PhoneNumber = bookingFromDb.User.PhoneNumber,
                        Notes = bookingFromDb.Notes,
                        serviceName = bookingFromDb.Service.Name,
                        Cost = bookingFromDb.Price.Cost.ToString(),
                        Duration = bookingFromDb.Price.Duration.ToString(),
                        Date = bookingFromDb.Date.ToString("dddd, dd MMM yyyy"),
                        Time = $"{_bookingsService.ChangeTimeFormat(bookingFromDb.StartTime)} ",
                        Status = bookingFromDb.Status,


                    };


                    // send email confirmation to user
                    _emailSender.SendBookingEmail(bookingResponse);


                    _response.StatusCode = HttpStatusCode.OK;
                    return Ok(_response);
                }
                else
                {


                    TimeSpan startTime = TimeSpan.Parse(bookingUpdateDTO.StartTime);

                    TimeSpan endTime = startTime.Add(TimeSpan.FromMinutes((double)bookingUpdateDTO.Duration));

                    bookingFromDb.Notes = bookingUpdateDTO.Notes;
                    bookingFromDb.Date = DateTime.Parse(bookingUpdateDTO.Date);
                    bookingFromDb.ServiceId = bookingUpdateDTO.ServiceId;
                    bookingFromDb.PriceId = bookingUpdateDTO.PriceId;
                    bookingFromDb.StartTime = startTime;
                    bookingFromDb.EndTime = bookingUpdateDTO.Duration >= 90 ? endTime.Add(TimeSpan.FromMinutes(30)) : endTime.Add(TimeSpan.FromMinutes(15));


                    await _context.SaveChangesAsync();
                    _response.StatusCode = HttpStatusCode.OK;
                    return Ok(_response);


                }





            }
            catch (Exception ex)
            {

                _response.ErrorMessages.Add(ex.Message);
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }

        }

        // POST: api/ServiceBookings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Booking>> PostServiceBookings([FromBody] BookingCreateDTO bookingCreateDto)
        {

            _response.IsSuccess = false;
            _response.StatusCode = HttpStatusCode.BadRequest;

            // check model state

            if (!ModelState.IsValid)
            {
                _response.ErrorMessages = ModelState.Values.SelectMany(m => m.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(_response);
            }



            // check username exists
            var user = await _userManager.FindByEmailAsync(bookingCreateDto.Email);
            string? userId = null;
            if (user == null)
            {


                // Create new user
                var newUser = new ApplicationUser
                {
                    UserName = bookingCreateDto.Email,
                    Email = bookingCreateDto.Email,
                    FirstName = bookingCreateDto.FirstName,
                    LastName = bookingCreateDto.LastName,
                    PhoneNumber = bookingCreateDto.PhoneNumber,
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(newUser);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser, StaticDetails.SD_Roles_User);
                }
                userId = newUser.Id;
            }
            else
            {
                // check if user is in user role
                var userRoles = await _userManager.GetRolesAsync(user);

                if (userRoles.Contains(StaticDetails.SD_Roles_User))
                {
                    // update user details
                    user.FirstName = bookingCreateDto.FirstName;
                    user.LastName = bookingCreateDto.LastName;
                    user.PhoneNumber = bookingCreateDto.PhoneNumber;
                    await _userManager.UpdateAsync(user);
                }



            }
            try
            {
                if (userId == null) userId = user.Id;




                // create new booking
                // parse Date

                DateTime date = DateTime.Parse(bookingCreateDto.Date);

                // Converting Strings to TimeSpan for database.
                TimeSpan startTime = DateTime.ParseExact(bookingCreateDto.StartTime, "h:mmtt", CultureInfo.InvariantCulture).TimeOfDay;

                TimeSpan endTime = DateTime.ParseExact(bookingCreateDto.EndTime, "h:mmtt", CultureInfo.InvariantCulture).TimeOfDay;

                // check if booking exists
                var bookingExists = await _context.Bookings.Where(b => b.Date == date && b.StartTime == startTime && b.EndTime == endTime).ToListAsync();

                // if booking exists return error
                if (bookingExists.Any())
                {
                    _response.ErrorMessages.Add("Booking already exists at this time.");

                    return BadRequest(_response);
                }



                // get price and service
                var price = await _context.Prices.FindAsync(bookingCreateDto.PriceId);

                var service = await _context.Services.FindAsync(bookingCreateDto.ServiceId);

                if (price == null || service == null)
                {
                    _response.ErrorMessages.Add("Something went wrong");

                    return BadRequest(_response);
                }


                // map to booking modal
                var newBooking = _dtoMappingToModel.MapToBookingModel(bookingCreateDto, userId);


                _context.Bookings.Add(newBooking);

                await _context.SaveChangesAsync();





                // create new bookingResponseDto

                BookingResponseDTO bookingResponseDTO = new BookingResponseDTO()
                {
                    FirstName = bookingCreateDto.FirstName,
                    LastName = bookingCreateDto.LastName,
                    Email = bookingCreateDto.Email,
                    PhoneNumber = bookingCreateDto.PhoneNumber,
                    Notes = bookingCreateDto.Notes,
                    serviceName = service.Name,
                    Date = date.ToString("dddd, dd MMMM yyyy"),
                    Time = bookingCreateDto.StartTime,
                    Cost = price.Cost.ToString(),
                    Duration = price.Duration.ToString(),
                    Status = StaticDetails.SD_BookingStatus_Pending,
                    BookingId = newBooking.Id,

                };

                // send email to user 
                _emailSender.SendBookingEmail(bookingResponseDTO);

                // send email to admin
                _emailSender.SendBookingEmailToAdmin(bookingResponseDTO);




                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = bookingResponseDTO;
                _response.IsSuccess = true;

                return CreatedAtAction("GetServiceBookings", new { id = newBooking.Id }, _response);

            }
            catch (Exception ex)
            {

                _response.ErrorMessages.Add(ex.Message);

                return BadRequest(_response);
            }
        }

        // DELETE: api/ServiceBookings/5
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceBookings(int id)
        {
            try
            {
                var serviceBookings = await _context.Bookings.FindAsync(id);
                if (serviceBookings == null)
                {
                    _response.ErrorMessages.Add("Booking not found.");
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    return NotFound(_response);
                }

                _context.Bookings.Remove(serviceBookings);
                await _context.SaveChangesAsync();

                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {

                _response.ErrorMessages.Add(ex.Message);
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }
        }

        // get bookings from a given date
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetBookingsFromDate")]
        public async Task<ActionResult> GetBookingsFromDate(string Date)
        {


            DateTime date = DateTime.Parse(Date);

            var bookings = await _context.Bookings.Where(b => b.Date == date).ToListAsync();
            if (bookings == null || bookings.Count == 0)
            {
                return NotFound();
            }

            return Ok(bookings);
        }


        // get available booking times for a service on a given date based on  current bookings and availabilities

        [HttpGet("AvailableTimes/{priceId}")]
        public async Task<ActionResult<ApiResponseDTO>> GetAvailableBookingTimes(int priceId, string Date)
        {
            try
            {



                DateTime date = DateTime.Parse(Date);

                // get todays date in new zealand time
                TimeZoneInfo nzTimeZone = TimeZoneInfo.FindSystemTimeZoneById("New Zealand Standard Time");

                DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, nzTimeZone).Date;



                //  DateTime today = DateTime.Today;

                // check if date is in the past
                if (date < today)
                {
                    _response.ErrorMessages.Add("Date cannot be in the past");
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }

                // get days on each side or 2 infront
                DateTime date1 = date.AddDays(1);
                DateTime date2 = date == today ? date.AddDays(2) : date.AddDays(-1);



                // get all bookings
                var bookings = await _context.Bookings.Where(b => b.Date == date || b.Date == date1 || b.Date == date2).ToListAsync();




                // get availabilities from that date
                var availabilities = await _context.Availabilities.Where(b => b.Date == date || b.Date == date1 || b.Date == date2).ToListAsync();



                // get the service duration
                var price = await _context.Prices.FindAsync(priceId);

                if (price == null)
                {
                    _response.ErrorMessages.Add("Price not found");
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }


                // filter bookings and availabilities by date
                var bookings1 = bookings.Where(b => b.Date == date).ToList();
                var bookings2 = bookings.Where(b => b.Date == date1).ToList();
                var bookings3 = bookings.Where(b => b.Date == date2).ToList();

                var availabilities1 = availabilities.Where(b => b.Date == date).ToList();
                var availabilities2 = availabilities.Where(b => b.Date == date1).ToList();
                var availabilities3 = availabilities.Where(b => b.Date == date2).ToList();

                AvailableTimeSlotsDTO timeSlotsDTO = new AvailableTimeSlotsDTO()
                {
                    SortOrder = 1,
                    DayOfWeek = date.ToString("dddd"),
                    DisplayDate = date.ToString("dd MMMM"),
                    Date = date.ToString("yyyy-MM-dd"),
                    AvailableTimeSlots = _bookingsService.GetAvailableTimes(bookings1, availabilities1, price, date)
                };
                AvailableTimeSlotsDTO timeSlotsDTO1 = new AvailableTimeSlotsDTO()
                {
                    SortOrder = 2,
                    DayOfWeek = date1.ToString("dddd"),
                    DisplayDate = date1.ToString("dd MMMM"),
                    Date = date1.ToString("yyyy-MM-dd"),
                    AvailableTimeSlots = _bookingsService.GetAvailableTimes(bookings2, availabilities2, price, date1)
                };
                AvailableTimeSlotsDTO timeSlotsDTO2 = new AvailableTimeSlotsDTO()
                {
                    SortOrder = date == today ? 3 : 0,
                    DayOfWeek = date2.ToString("dddd"),
                    DisplayDate = date2.ToString("dd MMMM"),
                    Date = date2.ToString("yyyy-MM-dd"),
                    AvailableTimeSlots = _bookingsService.GetAvailableTimes(bookings3, availabilities3, price, date2)
                };

                // get available times for each day
                var timeSlotsList = new List<AvailableTimeSlotsDTO>
                {
                    timeSlotsDTO,
                    timeSlotsDTO1,
                    timeSlotsDTO2
                };





                _response.Result = timeSlotsList;


                //                _response.Result = _bookingsService.GetAvailableTimes(bookings, availabilities, price);
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {

                _response.ErrorMessages.Add(ex.Message);
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }

        }


        private string updateEndTime(int duration, TimeSpan endTime)
        {

            if (duration >= 90)
            {
                return endTime.Subtract(TimeSpan.FromMinutes(30)).ToString("hh\\:mm");
            }

            return endTime.Subtract(TimeSpan.FromMinutes(15)).ToString("hh\\:mm");
        }

        private bool ServiceBookingsExists(int id)
        {
            return (_context.Bookings?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
