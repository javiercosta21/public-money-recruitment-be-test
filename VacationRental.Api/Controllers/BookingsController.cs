using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Classes;
using VacationRental.Api.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/bookings")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IDictionary<int, RentalViewModel> _rentals;
        private readonly IDictionary<int, BookingViewModel> _bookings;
        private readonly ILogic _logic;

        public BookingsController(
            IDictionary<int, RentalViewModel> rentals,
            IDictionary<int, BookingViewModel> bookings,
            ILogic logic)
        {
            _rentals = rentals;
            _bookings = bookings;
            _logic = logic;
        }

        [HttpGet]
        [Route("{bookingId:int}")]
        public BookingViewModel Get(int bookingId)
        {

            if (!_bookings.ContainsKey(bookingId))
                throw new ApplicationException("Booking not found");

            return _bookings[bookingId];
        }

        [HttpPost]
        public ResourceIdViewModel Post(BookingBindingModel model)
        {
            var key = new ResourceIdViewModel();

            if (model.Nights <= 0)
                throw new ApplicationException("Nights must be positive");

            if (!_rentals.TryGetValue(model.RentalId, out RentalViewModel rental))
                throw new ApplicationException("Booking not found");

            try
            {
                _logic.ValidateOccupation(model, rental, _rentals, _bookings);
            }
            catch(Exception e)
            {
                throw new ApplicationException("There is no available units for the booking.");
            }
            key.Id = _bookings.Keys.Count + 1;

            _bookings.Add(key.Id, new BookingViewModel
            {
                Id = key.Id,
                Nights = model.Nights,
                RentalId = model.RentalId,
                Start = model.Start.Date,
                PrepTimeInDays = rental.PrepTimeInDays
            });

            return key;


        }
    }
}
