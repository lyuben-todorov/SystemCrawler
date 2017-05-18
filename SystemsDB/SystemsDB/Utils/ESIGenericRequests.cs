using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using RestSharp.Extensions.MonoHttp;

namespace SystemsDB
{
    class ESIGenericRequests
    {
        public static async Task<JObject> GetSystemFromGate(string GateID)
        {
            return JObject.Parse(await Requests.GETRequest(@"https://esi.tech.ccp.is", string.Format(@"/latest/universe/stargates/{0}/?datasource=tranquility", GateID)));
        }
        public static async Task<string> ESISearch(string name, string category)
        {
            JObject jObject = JObject.Parse(await Requests.GETRequest("https://esi.tech.ccp.is", string.Format("/latest/search/?categories={0}&search={1}&strict=false", category, HttpUtility.UrlEncode(name))));
            File.AppendAllText(@"esisearch.json", jObject.ToString());
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
        public static async Task<JObject> GetConstInfo(string ConstID)
        {
            return JObject.Parse(await Requests.GETRequest(@"https://esi.tech.ccp.is", $"/latest/universe/constellations/{ConstID}/?datasource=tranquility"));
        }
        public static async Task<JObject> GetRegionInfo(string RegionID)
        {
            return JObject.Parse(await Requests.GETRequest(@"https://esi.tech.ccp.is", $"/latest/universe/regions/{RegionID}/?datasource=tranquility"));
        }
        public static async Task<JArray> GetIDInfoPOST(List<string> IDList)
        {
            JArray jArray = new JArray();
            foreach (string id in IDList)
            {
                jArray.Add(Convert.ToInt32(id));
            }
            Console.WriteLine(jArray.ToString());
            return JArray.Parse(await Requests.POSTRequest(@"https://esi.tech.ccp.is", "/latest/universe/names/?datasource=tranquility", jArray.ToString()));
        }

        //this is retarded
        public static async Task<JObject> GetSystemInfo(string SystemID)
        {
            return JObject.Parse(await Requests.GETRequest(@"https://esi.tech.ccp.is", string.Format("/latest/universe/systems/{0}/?datasource=tranquility&language=en-us", SystemID)));
        }
        public static async Task<string> GetSystemName(string SystemID)
        {
            return JObject.Parse(await Requests.GETRequest(@"https://esi.tech.ccp.is", string.Format("/latest/universe/systems/{0}/?datasource=tranquility&language=en-us", SystemID)))["name"].ToString();
        }

    }
}
