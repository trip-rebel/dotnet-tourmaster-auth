using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using IdentityModel.Client;

namespace Daemon
{
    public class Program
    {
        private static string tourMasterUsername = Environment.GetEnvironmentVariable("TM_USERNAME");
        private static string tourMasterPassword = Environment.GetEnvironmentVariable("TM_PASSWORD");

        //
        // To authenticate to Tourmaster, the client needs to know the service's App ID URI.
        //
        private static string tourMasterAuthUrl = Environment.GetEnvironmentVariable("TM_AUTH_URL");
        private static string tourMasterUrl = Environment.GetEnvironmentVariable("TM_URL");

        private static HttpClient httpClient = new HttpClient();

        public static void Main(string[] args)
        {
            Environment.SetEnvironmentVariable("TM_AUTH_URL", "http://tourmaster.services.triprebel.com/auth/token");
            Environment.SetEnvironmentVariable("TM_URL", "http://tourmaster.services.triprebel.com/orders/test");
            Environment.SetEnvironmentVariable("TM_USERNAME", "username");
            Environment.SetEnvironmentVariable("TM_PASSWORD", "password");

            TestUrl().Wait();
        }

        public static async Task TestUrl()
        {
            var token = await GetToken();

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.GetAsync(tourMasterUrl);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Successful request");
            }
            else
            {
                Console.WriteLine("Failed to execute request\nError: {0}\n", response.ReasonPhrase);
            }
        }

        public static async Task<string> GetToken()
        {
            var client = new TokenClient(
                tourMasterAuthUrl,
                "client-id",
                "client-secret");
            
            var response = await client.RequestResourceOwnerPasswordAsync(tourMasterUsername, tourMasterPassword);
            var token = response.AccessToken;

            return token;
        }
    }
}
