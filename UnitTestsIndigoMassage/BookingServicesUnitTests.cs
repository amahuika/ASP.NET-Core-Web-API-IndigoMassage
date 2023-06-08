using Indigo.BuisnessLogic.Services;
using Indigo.Models.Model;

namespace UnitTestsIndigoMassage
{
    public class BookingServicesUnitTests
    {

        BookingService _bookingServices = new BookingService();


        // test the changeTimeFormatFunction
        [Fact]
        public void ChangeTimeFormatTest()
        {


            // arrange
            var time = new TimeSpan(10, 30, 0);

            // act
            var result = _bookingServices.ChangeTimeFormat(time);

            // assert
            Assert.Equal("10:30am", result);
        }

        // test the GetAvailableTimes Function with availability
        [Fact]
        public void GetAvailableTimesTest()
        {
            // arrange
            var bookings = new List<Booking>();
            var availabilities = new List<Availability>();

            // add a start time of 10:30am and end time of 11:30am
            availabilities.Add(new Availability { StartTime = new TimeSpan(10, 30, 0), EndTime = new TimeSpan(11, 30, 0) });

            // add a duration of 30min
            var price = new Price
            {
                Duration = 30,
            };
            var date = new DateTime(2021, 1, 1);

            // act
            var result = _bookingServices.GetAvailableTimes(bookings, availabilities, price, date);

            var expected = "10:30am";

            // assert
            Assert.Equal(expected, result[0].StartTime);
        }

        // test GetAvailableTimes with to return with no times
        [Fact]
        public void GetAvailableTimesTestError()
        {
            // arrange
            var bookings = new List<Booking>();
            var availabilities = new List<Availability>();



            var price = new Price();
            var date = new DateTime(2021, 1, 1);

            // act
            var result = _bookingServices.GetAvailableTimes(bookings, availabilities, price, date);

            var expected = "No available times for this date";

            // assert
            Assert.Equal(expected, result[0].StartTime);
        }




    }
}