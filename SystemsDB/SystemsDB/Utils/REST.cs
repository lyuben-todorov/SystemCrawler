using RestSharp;
using System.Threading.Tasks;
using System;
using System.Threading;
namespace SystemsDB
{
    public class Requests
    {
        public static async Task<string> POSTRequest(string BaseURL, string ResourceURI, string data)
        {
            var client = new RestClient(BaseURL);
            var request = new RestRequest(ResourceURI, Method.POST);
            request.AddParameter("data", data, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;
            var cancellationTokenSource = new CancellationTokenSource();
            
            var restResponse = await client.ExecuteTaskAsync(request, cancellationTokenSource.Token);
            return restResponse.Content;
        }



        public static async Task<string> GETRequest(string BaseURL, string ResourceURI)
        {
            var client = new RestClient(BaseURL);
            var request = new RestRequest(ResourceURI, Method.GET);
            Console.WriteLine(BaseURL+ResourceURI);
            request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
            var cancellationTokenSource = new CancellationTokenSource();
            
            var restResponse = await client.ExecuteTaskAsync(request, cancellationTokenSource.Token);
            return restResponse.Content;

        }


    }
}
