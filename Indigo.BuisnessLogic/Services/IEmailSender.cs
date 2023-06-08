using Indigo.Models.DTO;

namespace Indigo.BuisnessLogic.Services
{
    public interface IEmailSender
    {

        void SendBookingEmail(BookingResponseDTO bookingResponse);

        void SendBookingEmailToAdmin(BookingResponseDTO bookingResponse);
    }
}
