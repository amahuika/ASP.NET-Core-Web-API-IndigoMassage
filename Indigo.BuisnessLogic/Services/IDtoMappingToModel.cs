using Indigo.Models.DTO;
using Indigo.Models.Model;

namespace Indigo.BuisnessLogic.Services
{
    public interface IDtoMappingToModel
    {
        Availability MapToAvailability(DateTime Date, TimeSpan start, TimeSpan End, string UserId);
        Booking MapToBookingModel(BookingCreateDTO bookingCreateDTO, string userId);

        Price MapToPriceModel(PriceDTO priceDTO);

        Service MapToServiceModel(ServiceDTO serviceDTO, string imageUrl);
    }
}