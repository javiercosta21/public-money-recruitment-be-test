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
    public class GetCalendarTests
    {
        private readonly HttpClient _client;

        public GetCalendarTests(IntegrationFixture fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenGetCalendar_ThenAGetReturnsTheCalculatedCalendar()
        {
            var postRentalRequest = new RentalBindingModel
            {
                Units = 2
            };

            ResourceIdViewModel postRentalResult;
            using (var postRentalResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", postRentalRequest))
            {
                Assert.True(postRentalResponse.IsSuccessStatusCode);
                postRentalResult = await postRentalResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var postBooking1Request = new BookingBindingModel
            {
                RentalId = postRentalResult.Id,
                Nights = 2,
                Start = new DateTime(2000, 01, 02)
            };

            ResourceIdViewModel postBooking1Result;
            using (var postBooking1Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking1Request))
            {
                Assert.True(postBooking1Response.IsSuccessStatusCode);
                postBooking1Result = await postBooking1Response.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var postBooking2Request = new BookingBindingModel
            {
                RentalId = postRentalResult.Id,
                Nights = 2,
                Start = new DateTime(2000, 01, 03)
            };

            ResourceIdViewModel postBooking2Result;
            using (var postBooking2Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking2Request))
            {
                Assert.True(postBooking2Response.IsSuccessStatusCode);
                postBooking2Result = await postBooking2Response.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            using (var getCalendarResponse = await _client.GetAsync($"/api/v1/calendar?rentalId={postRentalResult.Id}&start=2000-01-01&nights=5"))
            {
                Assert.True(getCalendarResponse.IsSuccessStatusCode);

                var getCalendarResult = await getCalendarResponse.Content.ReadAsAsync<CalendarViewModel>();

                Assert.Equal(postRentalResult.Id, getCalendarResult.RentalId);
                Assert.Equal(5, getCalendarResult.Dates.Count);

                Assert.Equal(new DateTime(2000, 01, 01), getCalendarResult.Dates[0].Date);
                Assert.Empty(getCalendarResult.Dates[0].Bookings);

                Assert.Equal(new DateTime(2000, 01, 02), getCalendarResult.Dates[1].Date);
                Assert.Single(getCalendarResult.Dates[1].Bookings);
                Assert.Contains(getCalendarResult.Dates[1].Bookings, x => x.Id == postBooking1Result.Id);

                Assert.Equal(new DateTime(2000, 01, 03), getCalendarResult.Dates[2].Date);
                Assert.Equal(2, getCalendarResult.Dates[2].Bookings.Count);
                Assert.Contains(getCalendarResult.Dates[2].Bookings, x => x.Id == postBooking1Result.Id);
                Assert.Contains(getCalendarResult.Dates[2].Bookings, x => x.Id == postBooking2Result.Id);

                Assert.Equal(new DateTime(2000, 01, 04), getCalendarResult.Dates[3].Date);
                Assert.Single(getCalendarResult.Dates[3].Bookings);
                Assert.Contains(getCalendarResult.Dates[3].Bookings, x => x.Id == postBooking2Result.Id);

                Assert.Equal(new DateTime(2000, 01, 05), getCalendarResult.Dates[4].Date);
                Assert.Empty(getCalendarResult.Dates[4].Bookings);
            }
        }

        [Fact]
        public async Task GetCalendarOkTest()
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

                var postBooking1Request2 = new BookingBindingModel
                {
                    RentalId = postRentalResult.Id,
                    Nights = 2,
                    Start = new DateTime(2000, 01, 05)
                };

                ResourceIdViewModel postBooking1Result2;
                using (var postBooking1Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking1Request2))
                {
                    Assert.True(postBooking1Response.IsSuccessStatusCode);
                    postBooking1Result2 = await postBooking1Response.Content.ReadAsAsync<ResourceIdViewModel>();
                }

                var postBooking2Request3 = new BookingBindingModel
                {
                    RentalId = postRentalResult.Id,
                    Nights = 2,
                    Start = new DateTime(2000, 01, 03)
                };

                ResourceIdViewModel postBooking2Result3;
                using (var postBooking2Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking2Request3))
                {
                    Assert.True(postBooking2Response.IsSuccessStatusCode);
                    postBooking2Result3 = await postBooking2Response.Content.ReadAsAsync<ResourceIdViewModel>();
                }

                var postBooking2Request4 = new BookingBindingModel
                {
                    RentalId = postRentalResult.Id,
                    Nights = 1,
                    Start = new DateTime(2000, 01, 01)
                };

                ResourceIdViewModel postBooking2Result4;
                using (var postBooking2Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking2Request4))
                {
                    Assert.True(postBooking2Response.IsSuccessStatusCode);
                    postBooking2Result4 = await postBooking2Response.Content.ReadAsAsync<ResourceIdViewModel>();
                }
            
                using (var getCalendarResponse = await _client.GetAsync($"/api/v1/calendar?rentalId={postRentalResult.Id}&start=2000-01-01&nights=5"))
                {
                    Assert.True(getCalendarResponse.IsSuccessStatusCode);

                    var getCalendarResult = await getCalendarResponse.Content.ReadAsAsync<CalendarViewModel>();

                    Assert.True(getCalendarResult.RentalId == postRentalResult.Id, "Values are not the same.");
                }

        }


        [Fact]
        public async Task GetCalendarFailTest()
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

            Exception ex = null;
            try
            {
                using (var getCalendarResponse = await _client.GetAsync($"/api/v1/calendar?rentalId=99&start=2000-01-01&nights=5"))
                {
                    Assert.True(getCalendarResponse.IsSuccessStatusCode);

                    var getCalendarResult = await getCalendarResponse.Content.ReadAsAsync<CalendarViewModel>();

                    Assert.True(getCalendarResult.RentalId == postRentalResult.Id, "Values are not the same.");
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
