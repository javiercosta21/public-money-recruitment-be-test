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
    public class GetRentalTests
    {
        private readonly HttpClient _client;

        public GetRentalTests(IntegrationFixture fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task GetRentalOkTest()
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

            
            using (var getRentalResponse = await _client.GetAsync($"/api/v1/rentals/1"))
            {
                Assert.True(getRentalResponse.IsSuccessStatusCode);

                var getRentalResult = await getRentalResponse.Content.ReadAsAsync<RentalViewModel>();

                Assert.True(postRentalResult.Id == getRentalResult.Id, "Id are not the same");
            }

        }

        [Fact]
        public async Task GetRentalFailTest()
        {
            Exception ex = null;
            try
            {
                using (var getRentalResponse = await _client.GetAsync($"/api/v1/rentals/1"))
                {

                    Assert.True(getRentalResponse.IsSuccessStatusCode);

                    var getRentalResult = await getRentalResponse.Content.ReadAsAsync<RentalViewModel>();
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
