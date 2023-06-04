using EnsekApiTest.Models;
using FluentAssertions;
using Newtonsoft.Json;
using RestSharp;
using System.Net;
using System.Text.RegularExpressions;

namespace EnsekApiTest.Helpers
{
    public static class RequestHelpers
    {
        public static string BearerToken { get; set; }
        public static RestClient Client { get; set; }
        public static string SuccessMessage { get; set; }
        public static RestResponse Response { get; set; }

        public static async Task LoginAndGetBearerToken(string url)
        {
            Client = new RestClient(url);

            var request = new RestRequest("login");

            var loginDetails = new Login()
            {
                username = "test",
                password = "testing"
            };

            var loginJson = JsonConvert.SerializeObject(loginDetails);

            request.AddBody(loginJson);

            //could have used this to deserialize automatically but then wouldn't have access to check the status code
            //var response = await _client.PostAsync<LoginResponse>(request);

            var response = await Client.PostAsync(request);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseObject = JsonConvert.DeserializeObject<LoginResponse>(response.Content);

            responseObject.access_token.Should().NotBeEmpty();

            BearerToken = responseObject.access_token;
        }

        public static async Task<string> BuyEnergy(string energyType, int amount)
        {
            var request = new RestRequest($"buy/{energyType}/{amount}");
            request.AddHeader("Authorization", $"Bearer {BearerToken}");

            var response = await Client.PutAsync(request);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseObject = JsonConvert.DeserializeObject<BuyEnergyResponse>(response.Content);

            var orderId = GetGuidFromMessage(responseObject.message);

            SuccessMessage = responseObject.message;

            return orderId;
        }       

        public static async Task ResetData()
        {
            var request = new RestRequest("reset");
            request.AddHeader("Authorization", $"Bearer {BearerToken}");

            var response = await Client.PostAsync(request);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        public static async Task<List<Order>> GetOrders()
        {
            var request = new RestRequest("orders");
            request.AddHeader("Authorization", $"Bearer {BearerToken}");

            var response = await Client.GetAsync(request);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            List<Order> orders = JsonConvert.DeserializeObject<List<Order>>(response.Content);

            OrdersResponse ordersResponse = new OrdersResponse
            {
                Orders = orders
            };

            List<Order> orderList = ordersResponse.Orders;

            return orderList;
        }

        public static async Task<EnergyResponse> GetEnergy()
        {
            var request = new RestRequest("energy");
            request.AddHeader("Authorization", $"Bearer {BearerToken}");

            var response = await Client.GetAsync(request);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var energy = JsonConvert.DeserializeObject<EnergyResponse>(response.Content);

            return energy;
        }
        public static async Task<HttpStatusCode> SendBuyEnergyRequest(string endpoint, string method, string energyType, int amount)
        {
            var request = new RestRequest($"{endpoint}/{energyType}/{amount}");
            request.AddHeader("Authorization", $"Bearer {BearerToken}");

            RestResponse response;

            try
            {
                switch (method.ToLower())
                {
                    case "get":
                        response = await Client.GetAsync(request);
                        break;
                    case "put":
                        response = await Client.PutAsync(request);
                        break;
                    case "post":
                        response = await Client.PostAsync(request);
                        break;
                    default:
                        response = await Client.GetAsync(request);
                        break;
                }

                Response = response;
            }
            catch (HttpRequestException e)
            {
                return (HttpStatusCode)e.StatusCode;
            }

            return response.StatusCode;
        }


        public static async Task<HttpStatusCode> Login(string username, string password)
        {
            var request = new RestRequest("login");

            var loginDetails = new Login()
            {
                username = username,
                password = password
            };

            var loginJson = JsonConvert.SerializeObject(loginDetails);

            request.AddBody(loginJson);

            try
            {
                Response = await Client.PostAsync(request);
            }
            catch (HttpRequestException e)
            {
                return (HttpStatusCode)e.StatusCode;
            }

            return Response.StatusCode;
        }

        public static string GetGuidFromMessage(string message)
        {
            var match = Regex.Match(message, @"[{(]?[0-9a-f]{8}[-]?([0-9a-f]{4}[-]?){3}[0-9a-f]{12}[)}]?");
            if (match.Success)
                return match.Value;

            return null;
        }

        public static int GetNumberOfOrdersBeforeCurrentDate(List<Order> orders)
        {
            DateTime todaysDate = DateTime.Now.Date;

            int ordersBeforeToday = 0;

            foreach (var order in orders)
            {
                DateTime orderDateOnly = DateTime.ParseExact(order.time, "ddd, d MMM yyyy HH:mm:ss 'GMT'",
                    System.Globalization.CultureInfo.InvariantCulture).Date;

                if (orderDateOnly < todaysDate)
                {
                    ordersBeforeToday++;
                }
            }

            return ordersBeforeToday;
        }
    }
}
