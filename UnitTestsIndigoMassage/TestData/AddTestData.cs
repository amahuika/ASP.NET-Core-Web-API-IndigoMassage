using Indigo.DataAccess.Data;
using Indigo.Models.Model;
using System.Text.Json;

namespace UnitTestsIndigoMassage.TestData
{
    public static class AddTestData
    {

        public static void AddPriceData(ApplicationDbContext context)
        {
            var jsonString = File.ReadAllText("TestData/TestPriceData.json");

            //need to stop it being case sensitive the model is capital case and
            // the json is not
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            var list = JsonSerializer.Deserialize<Price[]>(jsonString, options);
            if (list != null)
            {


                {
                    foreach (var item in list)
                    {
                        context.Prices.Add(item);
                    }
                    //save to the in memory database
                    context.SaveChanges();
                }
            }



        }

        public static void AddServicesData(ApplicationDbContext context)
        {
            var jsonString = File.ReadAllText("TestData/TestServiceData.json");

            //need to stop it being case sensitive the model is capital case and
            // the json is not
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            var list = JsonSerializer.Deserialize<Service[]>(jsonString, options);
            if (list != null)
            {
                {
                    foreach (var item in list)
                    {
                        context.Services.Add(item);
                    }
                    //save to the in memory database
                    context.SaveChanges();
                }
            }



        }
    }
}
