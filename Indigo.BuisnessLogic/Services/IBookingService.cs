using Indigo.Models.DTO;
using Indigo.Models.Model;

namespace Indigo.BuisnessLogic.Services
{
    public interface IBookingService
    {
        List<AvailableSlotsDTO> GetAvailableTimes(List<Booking> bookings, List<Availability> availabilities, Price price, DateTime date);

        List<BookingResponseDTO> ConvertBookingsToBookingResponseDTO(List<Booking> bookings);
        string ChangeTimeFormat(TimeSpan time);

    }
}
