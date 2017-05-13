using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp.Extensions.MonoHttp;

namespace SystemsDB
{
    class ESIGenericRequests
    {
        public static async Task<JObject> GetSystemFromGate(string GateID)
        {
            string json = await Requests.GETRequest(@"https://esi.tech.ccp.is", string.Format(@"/latest/universe/stargates/{0}/?datasource=tranquility", GateID));
            JObject jGate = JObject.Parse(json);
            return jGate;
        }
        public static async Task<string> ESISearch(string name, string category)
        {
            JObject jObject = JObject.Parse(await Requests.GETRequest("https://esi.tech.ccp.is", string.Format("/latest/search/?categories={0}&search={1}&strict=false", category, HttpUtility.UrlEncode(name))));
            JToken token = jObject[category];
            if (token != null)
            {
                return token.First.ToString();
            }
            else
            {
                return null;
            }
        }


        //this is retarded
        public static async Task<JObject> GetSystemInfo(string SystemID)
        {
            string json = await Requests.GETRequest(@"https://esi.tech.ccp.is", string.Format("/latest/universe/systems/{0}/?datasource=tranquility&language=en-us", SystemID));
            return JObject.Parse(json);
        }
        public static async Task<string> GetSystemName(string SystemID)
        {
            string json = await Requests.GETRequest(@"https://esi.tech.ccp.is", string.Format("/latest/universe/systems/{0}/?datasource=tranquility&language=en-us", SystemID));
            return JObject.Parse(json)["name"].ToString();
        }

    }
}
