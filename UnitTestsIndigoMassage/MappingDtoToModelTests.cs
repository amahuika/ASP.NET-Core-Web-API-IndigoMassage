using Indigo.BuisnessLogic.Services;
using Indigo.Models.DTO;
using Indigo.Models.Model;

namespace UnitTestsIndigoMassage
{
    public class MappingDtoToModelTests
    {

        DtoMappingToModel dtoMappingToModel = new DtoMappingToModel();


        // test the functions that map the DTO to the model
        [Fact]
        public void MapToAvailabilityTest()
        {
            // Arrange
            DateTime date = new DateTime(2021, 1, 1);
            TimeSpan start = new TimeSpan(10, 0, 0);
            TimeSpan end = new TimeSpan(11, 0, 0);
            string userId = "1";

            // Act
            Availability availability = dtoMappingToModel.MapToAvailability(date, start, end, userId);

            // Assert
            Assert.Equal(date, availability.Date);
            Assert.Equal(start, availability.StartTime);
            Assert.Equal(end, availability.EndTime);
            Assert.Equal(userId, availability.ApplicationUserId);
        }

        // test the functions that map the DTO to the model for booking
        [Fact]
        public void MapToBookingModelTest()
        {
            // Arrange
            var bookingCreateDTO = new BookingCreateDTO
            {
                Date = "2021-01-01",
                StartTime = "10:00am",
                EndTime = "11:00am",
                ServiceId = 1,
                PriceId = 1,
                Notes = "test",
            };
            string userId = "1";

            // Act
            Booking booking = dtoMappingToModel.MapToBookingModel(bookingCreateDTO, userId);

            // Assert


            Assert.Equal(bookingCreateDTO.ServiceId, booking.ServiceId);
            Assert.Equal(bookingCreateDTO.PriceId, booking.PriceId);
            Assert.Equal(bookingCreateDTO.Notes, booking.Notes);
            Assert.Equal(userId, booking.UserId);
        }

        // test the functions that map the DTO to the model for price
        [Fact]
        public void MapToPriceModelTest()
        {
            // Arrange
            var priceDTO = new PriceDTO
            {
                Cost = 100,
                Duration = 60,
                ServiceId = 1,
            };

            // Act
            Price price = dtoMappingToModel.MapToPriceModel(priceDTO);

            // Assert
            Assert.Equal(priceDTO.Cost, price.Cost);
            Assert.Equal(priceDTO.Duration, price.Duration);
            Assert.Equal(priceDTO.ServiceId, price.ServiceId);
        }

        // test the functions that map the DTO to the model for service
        [Fact]
        public void MapToServiceModelTest()
        {
            // Arrange
            var serviceDTO = new ServiceDTO
            {
                Name = "test",
                Description = "test",

            };
            string imageUrl = "test";

            // Act
            Service service = dtoMappingToModel.MapToServiceModel(serviceDTO, imageUrl);

            // Assert
            Assert.Equal(serviceDTO.Name, service.Name);
            Assert.Equal(serviceDTO.Description, service.Description);
            Assert.Equal(imageUrl, service.ImageUrl);
        }




    }
}
