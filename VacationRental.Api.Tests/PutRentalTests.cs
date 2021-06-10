using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VacationRental.Api.Models;
using Xunit;

namespace VacationRental.Api.Tests
{
    [Collection("Integration")]
    public class PutRentalTests
    {
        private readonly HttpClient _client;

        public PutRentalTests(IntegrationFixture fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task PutRentalOkTest()
        {
            var request = new RentalBindingModel
            {
                Units = 25
            };

            ResourceIdViewModel postResult;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", request))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }


            var postBooking1Request1 = new BookingBindingModel
            {
                RentalId = postResult.Id,
                Nights = 2,
                Start = new DateTime(2000, 01, 02)
            };

            ResourceIdViewModel postBooking1Result1;
            using (var postBooking1Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking1Request1))
            {
                Assert.True(postBooking1Response.IsSuccessStatusCode);
                postBooking1Result1 = await postBooking1Response.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var putBookingRequest2 = new BookingBindingModel
            {
                RentalId = postResult.Id,
                Nights = 1,
                Start = new DateTime(2000, 01, 01)
            };
            using (var bookingsResponse = await _client.PutAsJsonAsync($"/api/v1/rentals?rentalId=1&Units=3&PreparationTime=1", putBookingRequest2))
            {
                var bookings = await bookingsResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }



            using (var getResponse = await _client.GetAsync($"/api/v1/rentals/{postResult.Id}"))
            {
                Assert.True(getResponse.IsSuccessStatusCode);

                var getResult = await getResponse.Content.ReadAsAsync<RentalViewModel>();
                Assert.True(request.Units!=getResult.Units,"Values shouldn´t be the same.");
            }
        }

        [Fact]
        public async Task PutRentalFailTest()
        {
            var request = new RentalBindingModel
            {
                Units = 2
            };

            ResourceIdViewModel postResult;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", request))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }


            var postBooking1Request1 = new BookingBindingModel
            {
                RentalId = postResult.Id,
                Nights = 2,
                Start = new DateTime(2000, 01, 02)
            };

            ResourceIdViewModel postBooking1Result1;
            using (var postBooking1Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking1Request1))
            {
                Assert.True(postBooking1Response.IsSuccessStatusCode);
                postBooking1Result1 = await postBooking1Response.Content.ReadAsAsync<ResourceIdViewModel>();
            }


            var postBooking1Request2 = new BookingBindingModel
            {
                RentalId = postResult.Id,
                Nights = 2,
                Start = new DateTime(2000, 01, 02)
            };

            ResourceIdViewModel postBooking1Result2;
            using (var postBooking2Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking1Request2))
            {
                Assert.True(postBooking2Response.IsSuccessStatusCode);
                postBooking1Result2 = await postBooking2Response.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            

            Exception ex = null;
            try
            {

                var putBookingRequest2 = new BookingBindingModel
                {
                    RentalId = postResult.Id,
                    Nights = 1,
                    Start = new DateTime(2000, 01, 01)
                };
                using (var bookingsResponse = await _client.PutAsJsonAsync($"/api/v1/rentals?rentalId=1&Units=1&PreparationTime=1", putBookingRequest2))
                {
                    var bookings = await bookingsResponse.Content.ReadAsAsync<ResourceIdViewModel>();
                }
            }
            catch (ApplicationException e)
            {
                ex = e;
            }

            Assert.True(ex != null, "An error was expected.");
        }
    }
}
