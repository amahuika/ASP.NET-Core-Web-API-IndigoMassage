using Indigo.Models.DTO;
using Newtonsoft.Json;
using System.Net;

namespace UnitTestsIndigoMassage
{
    public class ControllerIntigrationTests : IClassFixture<TestingWebAppFactory<Program>>
    {


        private readonly HttpClient _client;

        public ControllerIntigrationTests(TestingWebAppFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        // test the GET ALL Prices endpoint
        [Fact]
        public async Task GetPricesTest()
        {
            var response = await _client.GetAsync("api/prices");
            response.EnsureSuccessStatusCode();

            // convert to my apiResponseDTO
            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponseDTO>(responseContent);

            Assert.True(apiResponse.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, apiResponse.StatusCode);

            // check if the result is populated
            Assert.NotNull(apiResponse.Result);

        }

        // test the POST new price
        // NOTE COMMENT OUT [Authorize] ATTRIBUTE IN CONTROLLER WHEN TESTING THIS TEST.
        /*    [Fact]
            public async Task PostReturnsApiResponseDTO()
            {
                // Arrange
                var newPrice = new PriceDTO
                {
                    Cost = 45,
                    Duration = 40,
                    ServiceId = 3,
                };
                var multipartContent = new MultipartFormDataContent();
                multipartContent.Add(new StringContent(newPrice.Cost.ToString()), "Cost");
                multipartContent.Add(new StringContent(newPrice.Duration.ToString()), "Duration");
                multipartContent.Add(new StringContent(newPrice.ServiceId.ToString()), "ServiceId");


                // Act
                var response = await _client.PostAsync("api/Price", multipartContent);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponseDTO>(responseContent);

                // Assert
                Assert.True(apiResponse.IsSuccess);
                Assert.Equal(HttpStatusCode.OK, apiResponse.StatusCode);


            }
    */
        // test the GET Price by id endpoint
        [Fact]
        public async Task GetPriceByIdReturnResponseAPI()
        {
            // form body with id 


            var response = await _client.GetAsync("api/prices/1");
            response.EnsureSuccessStatusCode();

            // convert to my apiResponseDTO
            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponseDTO>(responseContent);

            Assert.True(apiResponse.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, apiResponse.StatusCode);
            Assert.NotNull(apiResponse.Result);



        }

        // services controller tests /////////////////////////////
        [Fact]
        public async Task GetServiceByIdReturnResponseAPI()
        {
            // form body with id 


            var response = await _client.GetAsync("api/services/3");
            response.EnsureSuccessStatusCode();

            // convert to my apiResponseDTO
            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponseDTO>(responseContent);

            Assert.True(apiResponse.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, apiResponse.StatusCode);
            Assert.NotNull(apiResponse.Result);

            // check if result contains same id
            /* var service = JsonConvert.DeserializeObject<Service>(apiResponse.Result.ToString());
             Assert.Equal(3, service.Id);*/

        }

        // Get services test
        [Fact]
        public async Task GetServicesTest()
        {
            var response = await _client.GetAsync("api/services");
            response.EnsureSuccessStatusCode();

            // convert to my apiResponseDTO
            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponseDTO>(responseContent);

            Assert.True(apiResponse.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, apiResponse.StatusCode);

            // check if the result is populated
            Assert.NotNull(apiResponse.Result);

        }




        // test the POST new service
        // NOTE COMMENT OUT [Authorize] ATTRIBUTE IN CONTROLLER WHEN TESTING THIS TEST.
        /*      [Fact]
              public async Task PostReturnsApiResponseDTO()
              {
                  // Arrange
                  var newService = new ServiceDTO
                  {
                      Name = "Test Service",
                      Description = "Test description",
                      Image = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("Test file")), 0, 1, "Data", "test.jpg")
                  };
                  var multipartContent = new MultipartFormDataContent();
                  multipartContent.Add(new StringContent(newService.Name), "Name");
                  multipartContent.Add(new StringContent(newService.Description), "Description");
                  multipartContent.Add(new StreamContent(newService.Image.OpenReadStream()), "Image", newService.Image.FileName);

                  // Act
                  var response = await _client.PostAsync("api/services", multipartContent);
                  response.EnsureSuccessStatusCode();
                  var responseContent = await response.Content.ReadAsStringAsync();
                  var apiResponse = JsonConvert.DeserializeObject<ApiResponseDTO>(responseContent);

                  // Assert
                  Assert.True(apiResponse.IsSuccess);
                  Assert.Equal(HttpStatusCode.OK, apiResponse.StatusCode);


              }*/



    }
}

