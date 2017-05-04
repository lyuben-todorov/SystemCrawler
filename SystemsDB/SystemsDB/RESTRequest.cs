using RestSharp;

namespace Systems
{
    public class Requests
    {
        public static string POSTRequest(string BaseURL, string ResourceURI, string data)
        {
            var client = new RestClient(BaseURL);
            var request = new RestRequest(ResourceURI, Method.POST);
            request.AddParameter("data", data, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;
            var response = client.Execute(request);
            return response.Content.ToString();
        }
        public static string GETRequest(string BaseURL, string ResourceURI)
        {
            var client = new RestClient(BaseURL);
            var request = new RestRequest(ResourceURI, Method.GET);
            request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
            var response = client.Execute(request);
            return response.Content.ToString();
        }

    }
}
