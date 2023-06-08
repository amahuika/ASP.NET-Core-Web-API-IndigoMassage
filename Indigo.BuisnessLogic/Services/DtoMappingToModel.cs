using Indigo.BuisnessLogic.Utilities;
using Indigo.Models.DTO;
using Indigo.Models.Model;
using System.Globalization;

namespace Indigo.BuisnessLogic.Services
{
    public class DtoMappingToModel : IDtoMappingToModel
    {


        public Availability MapToAvailability(DateTime Date, TimeSpan start, TimeSpan End, string UserId)
        {

            Availability availability = new Availability
            {
                ApplicationUserId = UserId,
                Date = Date,
                StartTime = start,
                EndTime = End,

            };
            return availability;

        }

        public Booking MapToBookingModel(BookingCreateDTO bookingCreateDTO, string userId)
        {

            // Converting Strings to TimeSpan for database.
            TimeSpan startTime = DateTime.ParseExact(bookingCreateDTO.StartTime, "h:mmtt", CultureInfo.InvariantCulture).TimeOfDay;

            TimeSpan endTime = DateTime.ParseExact(bookingCreateDTO.EndTime, "h:mmtt", CultureInfo.InvariantCulture).TimeOfDay;

            DateTime date = DateTime.Parse(bookingCreateDTO.Date);
            var newBooking = new Booking
            {
                Date = date,
                StartTime = startTime,
                EndTime = endTime,
                ServiceId = bookingCreateDTO.ServiceId,
                PriceId = bookingCreateDTO.PriceId,
                UserId = userId,
                Notes = bookingCreateDTO.Notes,
                Status = StaticDetails.SD_BookingStatus_Pending,
            };

            return newBooking;


        }


        //  map to price model
        public Price MapToPriceModel(PriceDTO priceDTO)
        {


            return new Price
            {
                Cost = priceDTO.Cost,
                Duration = priceDTO.Duration,
                ServiceId = priceDTO.ServiceId,
            };
        }

        // map dto to service
        public Service MapToServiceModel(ServiceDTO serviceDTO, string imageUrl)
        {
            return new Service
            {
                Description = serviceDTO.Description,
                Name = serviceDTO.Name,
                ImageUrl = imageUrl
            };
        }


    }
}
