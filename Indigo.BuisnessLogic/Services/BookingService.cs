using Indigo.Models.DTO;
using Indigo.Models.Model;

namespace Indigo.BuisnessLogic.Services
{
    public class BookingService : IBookingService
    {



        public List<AvailableSlotsDTO> GetAvailableTimes(List<Booking> bookings, List<Availability> availabilities, Price price, DateTime date)
        {
            List<AvailableSlotsDTO> availableSlots = new List<AvailableSlotsDTO>();

            if (availabilities == null || availabilities.Count == 0)
            {
                var error = new AvailableSlotsDTO
                {
                    StartTime = "No available times for this date",

                    EndTime = ""
                };

                availableSlots.Add(error);

                return availableSlots;
            }




            // add 15min to service length if it is less than 90min and add 30min if it is greater than 90min
            int serviceLength = price.Duration >= 90 ? price.Duration + 30 : price.Duration + 15;

            // create timespan object for the service length
            TimeSpan lengthOfServiceToAdd = TimeSpan.FromMinutes(serviceLength);



            // get the start and end times from the availabilities of the massage therapist
            var shiftStart = availabilities.FirstOrDefault()?.StartTime;
            var ShiftEnd = availabilities.FirstOrDefault()?.EndTime;

            // check if the date is today if it is then  check if the current time is after the start time of the first availability and before the end time of the last availability if it is then set the start time to the current time plus 2hours.

            // get todays date in New Zealand time
            TimeZoneInfo nzTimeZone = TimeZoneInfo.FindSystemTimeZoneById("New Zealand Standard Time");
            DateTime todayDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, nzTimeZone).Date;

            if (date == todayDate)
            {

                // get the current time in New Zealand time
                DateTime currentDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, nzTimeZone);




                // add 3 hours to the current time as the new start time

                var newShiftStart = currentDateTime.TimeOfDay.Add(TimeSpan.FromHours(3));

                // round it to the closest 15min
                newShiftStart = new TimeSpan(newShiftStart.Hours, newShiftStart.Minutes - (newShiftStart.Minutes % 15), 0);

                // check if new shift start is after shift end
                if (newShiftStart > ShiftEnd)
                {

                    var error = new AvailableSlotsDTO
                    {
                        StartTime = "No available times for this date",

                        EndTime = ""
                    };

                    availableSlots.Add(error);

                    return availableSlots;

                }


                shiftStart = newShiftStart;

            }





            // create 15min timespan object, each available appointment will be 15min apart.
            TimeSpan timeToAdd = TimeSpan.FromMinutes(15);

            // convert duration to timeSpan for latest possible start time
            TimeSpan duration = TimeSpan.FromMinutes(price.Duration);

            // get the latest possible start time for the service.
            TimeSpan latestStartTime = (TimeSpan)(ShiftEnd - duration);
            var timeSlot = shiftStart;

            // create all the available times based on the availability, current bookings and the service length
            while (timeSlot <= latestStartTime)
            {

                // default to true timeSlot available
                bool isAvailable = true;

                foreach (var existingBooking in bookings)
                {
                    // new booking end time for if statements.
                    TimeSpan newBookingEndTime = (TimeSpan)(timeSlot + lengthOfServiceToAdd);



                    if (timeSlot >= existingBooking.EndTime || newBookingEndTime <= existingBooking.StartTime)
                    {
                        continue;
                    }
                    else if (timeSlot >= existingBooking.StartTime && timeSlot < existingBooking.EndTime)
                    {

                        isAvailable = false;
                        break;
                    }
                    else if (newBookingEndTime > existingBooking.StartTime && newBookingEndTime < existingBooking.EndTime)
                    {
                        isAvailable = false;
                        break;
                    }else if (existingBooking.StartTime > timeSlot && existingBooking.EndTime >= newBookingEndTime)
                    {
                        isAvailable = false;
                        break;
                    }

                }

                if (isAvailable)
                {

                    // convert timeSpan to string 

                    var availability = new AvailableSlotsDTO
                    {

                        StartTime = ChangeTimeFormat((TimeSpan)timeSlot),
                        EndTime = ChangeTimeFormat((TimeSpan)(timeSlot + lengthOfServiceToAdd)),

                    };

                    availableSlots.Add(availability);
                    // availableTimes.Add((TimeSpan)timeSlot);
                }


                timeSlot = timeSlot + timeToAdd;
            }

            if (availableSlots.Count == 0)
            {

                availableSlots.Add(new AvailableSlotsDTO
                {
                    StartTime = "No available times for this date",
                    EndTime = ""
                });

            }



            return availableSlots;






        }


        public string ChangeTimeFormat(TimeSpan time)
        {
            var hours = time.Hours;
            var minutes = time.Minutes;
            var amPmDesignator = "am";
            if (hours == 12)
            {
                amPmDesignator = "pm";
            }
            else if (hours > 12)
            {
                hours -= 12;
                amPmDesignator = "pm";

            }


            return String.Format("{0}:{1:00}{2}", hours, minutes, amPmDesignator);
        }


        // convert list of bookings to booking response dto
        public List<BookingResponseDTO> ConvertBookingsToBookingResponseDTO(List<Booking> bookings)
        {
            List<BookingResponseDTO> bookingResponseDTOs = new List<BookingResponseDTO>();

            foreach (var booking in bookings)
            {

                if (booking.Price == null || booking.User == null || booking.Service == null)
                {
                    continue;
                }


                // if the duration is 90min or more then subtract 30min to the end time otherwise subtract 15min
                TimeSpan endTime = booking.Price.Duration >= 90 ? booking.EndTime.Subtract(TimeSpan.FromMinutes(30)) : booking.EndTime.Subtract(TimeSpan.FromMinutes(15));


                var bookingResponseDTO = new BookingResponseDTO
                {
                    BookingId = booking.Id,
                    FirstName = booking.User.FirstName,
                    LastName = booking.User.LastName,
                    Email = booking.User.Email,
                    PhoneNumber = booking.User.PhoneNumber,
                    Notes = booking.Notes,
                    serviceName = booking.Service.Name,
                    Date = booking.Date.ToString("dd/MM/yyyy"),
                    Time = ChangeTimeFormat(booking.StartTime) + " - " + ChangeTimeFormat(endTime),
                    StartTime = booking.StartTime.ToString(),
                    EndTime = endTime.ToString(),

                    Cost = booking.Price.Cost.ToString("C"),
                    Duration = booking.Price.Duration.ToString(),
                    Status = booking.Status
                };

                bookingResponseDTOs.Add(bookingResponseDTO);
            }

            return bookingResponseDTOs;
        }


    }
}
