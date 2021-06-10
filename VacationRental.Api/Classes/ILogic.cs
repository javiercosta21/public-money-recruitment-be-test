using System;
using System.Collections.Generic;
using VacationRental.Api.Models;

namespace VacationRental.Api.Classes
{
    public interface ILogic
    {
        void ValidateOccupation(BookingBindingModel model, RentalViewModel rental, IDictionary<int, RentalViewModel> _rentals,
            IDictionary<int, BookingViewModel> _bookings);
        void ValidateOccupationPut(BookingBindingModel model, RentalViewModel rental,
            IDictionary<int, BookingViewModel> _bookings);
        CalendarViewModel GetCalendar(int rentalId, DateTime start, int nights, IDictionary<int, BookingViewModel> bookings);
    }
}