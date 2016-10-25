using System;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Daemon
{
    public class Program
    {//
        // The Client ID is used by the application to uniquely identify itself to Azure AD.
        // The App Key is a credential used by the application to authenticate to Azure AD.
        // The Authority is the sign-in URL of the tenant.
        //
        private static string clientId = Environment.GetEnvironmentVariable("AD_CLIENTID");
        private static string appKey = Environment.GetEnvironmentVariable("AD_APPKEY");
        private static string authority = Environment.GetEnvironmentVariable("AD_AUTHORITY");

        //
        // To authenticate to Tourmaster, the client needs to know the service's App ID URI.
        //
        private static string tourMasterResourceId = Environment.GetEnvironmentVariable("TM_RESOURCEID");
        private static string tourMasterUrl = Environment.GetEnvironmentVariable("TM_URL");

        private static HttpClient httpClient = new HttpClient();
        private static AuthenticationContext authContext = null;
        private static ClientCredential clientCredential = null;

        public static void Main(string[] args)
        {
            Environment.SetEnvironmentVariable("AD_AUTHORITY", "https://login.microsoftonline.com/accounts.triprebel.com");
            Environment.SetEnvironmentVariable("TM_URL", "http://tourmaster.services.triprebel.com/orders/test");
            Environment.SetEnvironmentVariable("AD_CLIENTID", "CLIENT_ID");
            Environment.SetEnvironmentVariable("AD_APPKEY", "APP_KEY");
            Environment.SetEnvironmentVariable("TM_RESOURCEID", "633dca92-ffe8-4591-a9d4-c13e97445c31");

            authContext = new AuthenticationContext(authority);
            clientCredential = new ClientCredential(clientId, appKey);

            TestUrl().Wait();
        }

        public static async Task TestUrl()
        {
            var result = await GetToken();

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);

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

        public static async Task<AuthenticationResult> GetToken()
        {
            // ADAL includes an in memory cache, so this call will only send a message to the server if the cached token is expired.
            return await authContext.AcquireTokenAsync(tourMasterResourceId, clientCredential);
        }
    }
}
