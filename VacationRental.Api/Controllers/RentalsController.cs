using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Classes;
using VacationRental.Api.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/rentals")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly IDictionary<int, RentalViewModel> _rentals;
        private readonly IDictionary<int, BookingViewModel> _bookings;
        private readonly ILogic _logic;

        public RentalsController(IDictionary<int, RentalViewModel> rentals, IDictionary<int, BookingViewModel> bookings,
            ILogic logic)
        {
            _rentals = rentals;
            _bookings = bookings;
            _logic = logic;
        }

        [HttpGet]
        [Route("{rentalId:int}")]
        public RentalViewModel Get(int rentalId)
        {

            if (!_rentals.ContainsKey(rentalId))
                throw new ApplicationException("Rental not found");


            return _rentals[rentalId];
        }

        [HttpPost]
        public ResourceIdViewModel Post(RentalBindingModel model)
        {
            var key = new ResourceIdViewModel { Id = _rentals.Keys.Count + 1 };

            _rentals.Add(key.Id, new RentalViewModel
            {
                Id = key.Id,
                Units = model.Units,
                PrepTimeInDays = model.PrepTimeInDays
            }); ;


            return key;
        }

        [HttpPut]
        public ResourceIdViewModel Put(int rentalId, int Units, int PreparationTime, BookingBindingModel model)
        {
            var Id = new ResourceIdViewModel() { Id = rentalId };


            if (!_rentals.TryGetValue(model.RentalId, out RentalViewModel rental))
                throw new ApplicationException("Rental not found");

            rental.PrepTimeInDays = PreparationTime;
            rental.Units = Units;

            try
            {
                _logic.ValidateOccupationPut(model, rental, _bookings);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Rental cannot be modified because Unit and Preparation Time are not valid.");
            }

            _rentals[rentalId].Units = Units;
            _rentals[rentalId].PrepTimeInDays = PreparationTime;


            return Id;
        }
    }
}
