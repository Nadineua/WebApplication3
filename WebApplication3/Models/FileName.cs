namespace WebApplication3.Models
{
    using System.Net.Http;
    using System.Threading.Tasks;

    namespace WebApplication3.Model
    {
        public class AzureFunctionCaller
        {
            private readonly string functionUrl;
            private readonly string functionKey;

            public AzureFunctionCaller(string functionUrl, string functionKey)
            {
                this.functionUrl = functionUrl;
                this.functionKey = functionKey;
            }

            // Call the Azure Function and retrieve its response
            public async Task<string> CallFunctionAsync()
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(functionUrl + "?code=" + functionKey);
                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        return result;
                    }
                    else
                    {
                        return "Error: " + response.StatusCode;
                    }
                }
            }
        }
    }

}
