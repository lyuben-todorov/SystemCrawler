using Newtonsoft.Json.Linq;
using RestSharp.Extensions.MonoHttp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SystemsDB
{
    class ESIGenericRequests
    {
        public static string BASEURI = "https://esi.tech.ccp.is";

        public static async Task<JObject> GetSystemFromGate(string GateID)
        {
            return JObject.Parse(await Requests.GETRequest(BASEURI, string.Format(@"/latest/universe/stargates/{0}/?datasource=tranquility", GateID)));
        }
        public static async Task<string> ESISearch(string name, string category)
        {
            JObject jObject = JObject.Parse(await Requests.GETRequest(BASEURI, string.Format("/latest/search/?categories={0}&search={1}&strict=false", category, HttpUtility.UrlEncode(name))));
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
            return JObject.Parse(await Requests.GETRequest(BASEURI, $"/latest/universe/constellations/{ConstID}/?datasource=tranquility"));
        }
        public static async Task<JObject> GetRegionInfo(string RegionID)
        {
            return JObject.Parse(await Requests.GETRequest(BASEURI, $"/latest/universe/regions/{RegionID}/?datasource=tranquility"));
        }
        public static async Task<JArray> GetIDInfoPOST(List<string> IDList)
        {
            JArray jArray = new JArray();
            foreach (string id in IDList)
            {
                jArray.Add(Convert.ToInt32(id));
            }
            return JArray.Parse(await Requests.POSTRequest(BASEURI, "/latest/universe/names/?datasource=tranquility", jArray.ToString()));
        }
        public static async Task<List<string>> GetKRegionList()
        {
            List<string> RList = new List<string>();
            JArray jRegions = JArray.Parse(await Requests.GETRequest(BASEURI, "/latest/universe/regions/?datasource=tranquility"));
            foreach(var element in jRegions)
            {
                //k-regions start with 10
                string sub = element.ToString().Substring(0, 2);
                if(sub == "10")RList.Add(element.ToString());
            }
            return RList;
        }
        public static async Task<List<string>> GetJRegionList()
        {
            List<string> JRList = new List<string>();
            JArray jJRegions = JArray.Parse(await Requests.GETRequest(BASEURI, "/latest/universe/regions/?datasource=tranquility"));
            foreach (var element in jJRegions)
            {
                //k-regions start with 10
                string sub = element.ToString().Substring(0, 2);
                if (sub == "11") JRList.Add(element.ToString());
            }
            return JRList;
        }
        public static async Task<JObject> GetSystemInfo(string SystemID)
        {
            return JObject.Parse(await Requests.GETRequest(BASEURI, string.Format("/latest/universe/systems/{0}/?datasource=tranquility&language=en-us", SystemID)));
        }

    }
}
