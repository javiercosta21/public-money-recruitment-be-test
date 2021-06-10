using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationRental.Api.Models;

namespace VacationRental.Api.Classes
{
    public class Logic : ILogic
    {
        public void ValidateOccupation(BookingBindingModel model,RentalViewModel rental, IDictionary<int, RentalViewModel> _rentals,
            IDictionary<int, BookingViewModel> _bookings)
        {
            

            var nightsPrepTimeInDays = model.Nights + rental.PrepTimeInDays - 1;

            for (var i = 0; i <= nightsPrepTimeInDays; i++)
            {
                var count = 0;
                foreach (var booking in _bookings.Values)
                {
                    if (booking.RentalId != model.RentalId) continue;

                    var bookingNightsPrepTimeInDays = booking.Nights + booking.PrepTimeInDays - 1;

                    if (model.Start.AddDays(i) >= booking.Start && model.Start.AddDays(i) <= booking.Start.AddDays(bookingNightsPrepTimeInDays))
                    {
                        count++;
                    }
                }

                if (count >= _rentals[model.RentalId].Units)
                    throw new ApplicationException("Not available");
            }

        }

        public void ValidateOccupationPut(BookingBindingModel model, RentalViewModel rental,
            IDictionary<int, BookingViewModel> _bookings)
        {


            var nightsPrepTimeInDays = model.Nights + rental.PrepTimeInDays - 1;

            for (var i = 0; i <= nightsPrepTimeInDays; i++)
            {
                var count = 0;
                foreach (var booking in _bookings.Values)
                {
                    if (booking.RentalId != model.RentalId) continue;

                    var bookingNightsPrepTimeInDays = booking.Nights + booking.PrepTimeInDays - 1;

                    if (model.Start.AddDays(i) >= booking.Start && model.Start.AddDays(i) <= booking.Start.AddDays(bookingNightsPrepTimeInDays))
                    {
                        count++;
                    }

                }

                if (count >= rental.Units)
                    throw new ApplicationException("Not available");
            }

        }

        public CalendarViewModel GetCalendar(int rentalId, DateTime start, int nights, IDictionary<int, BookingViewModel> bookings)
        {
            var result = new CalendarViewModel
            {
                RentalId = rentalId,
                Dates = new List<CalendarDateViewModel>()
            };
            for (var i = 0; i < nights; i++)
            {
                var date = new CalendarDateViewModel
                {
                    Date = start.Date.AddDays(i),
                    Bookings = new List<CalendarBookingViewModel>(),
                    PreparationTimes = new List<CalendarPreparationTimeViewModel>()
                };

                foreach (var booking in bookings.Values)
                {
                    if (booking.RentalId == rentalId)
                    {
                        var bookingEnd = booking.Start.AddDays(booking.Nights);

                        if (booking.Start <= date.Date && booking.Start.AddDays(booking.Nights) > date.Date)
                        {
                            date.Bookings.Add(new CalendarBookingViewModel { Id = booking.Id, Unit = date.Bookings.Count + 1 });
                        }
                        if (bookingEnd <= date.Date && bookingEnd.AddDays(booking.PrepTimeInDays) > date.Date)
                        {
                            date.PreparationTimes.Add(new CalendarPreparationTimeViewModel { Unit = date.PreparationTimes.Count + 1 });
                        }
                    }
                }

                result.Dates.Add(date);
            }

            return result;
        }
    }
}
