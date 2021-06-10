using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VacationRental.Api.Models;
using Xunit;

namespace VacationRental.Api.Tests
{
    [Collection("Integration")]
    public class GetBookingTests
    {
        private readonly HttpClient _client;

        public GetBookingTests(IntegrationFixture fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task GetBookingOkTest()
        {
                var postRentalRequest = new RentalBindingModel
                {
                    Units = 2,
                    PrepTimeInDays = 1
                };

                ResourceIdViewModel postRentalResult;
                using (var postRentalResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", postRentalRequest))
                {
                    Assert.True(postRentalResponse.IsSuccessStatusCode);
                    postRentalResult = await postRentalResponse.Content.ReadAsAsync<ResourceIdViewModel>();
                }

                var postBooking1Request1 = new BookingBindingModel
                {
                    RentalId = postRentalResult.Id,
                    Nights = 2,
                    Start = new DateTime(2000, 01, 02)
                };

                ResourceIdViewModel postBooking1Result1;
                using (var postBooking1Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking1Request1))
                {
                    Assert.True(postBooking1Response.IsSuccessStatusCode);
                    postBooking1Result1 = await postBooking1Response.Content.ReadAsAsync<ResourceIdViewModel>();
                }

                using (var getBookingResponse = await _client.GetAsync($"/api/v1/bookings/1"))
                {
                    Assert.True(getBookingResponse.IsSuccessStatusCode);

                    var getBookingResult = await getBookingResponse.Content.ReadAsAsync<BookingViewModel>();

                    Assert.True(postRentalResult.Id == getBookingResult.RentalId, "Id are not the same");
                }
            
        }

        [Fact]
        public async Task GetBookingFailTest()
        {
            Exception ex = null;
            try
            {
                using (var getBookingResponse = await _client.GetAsync($"/api/v1/bookings/1"))
                {

                    Assert.True(getBookingResponse.IsSuccessStatusCode);                    

                    var getBookingResult = await getBookingResponse.Content.ReadAsAsync<BookingViewModel>();
                }
            }
            catch (ApplicationException e)
            {
                ex = e;
            }

            Assert.True(ex != null,"An error was expected.");
            

        }

    }

}
