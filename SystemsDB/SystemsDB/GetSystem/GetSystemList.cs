using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace SystemsDB
{
    class GetSystemList
    {
        public static async Task<List<string>> Region(string ID)
        {
            List<string> ConstList = new List<string>();

            JObject jRegion = JObject.Parse(await Requests.GETRequest(@"https://esi.tech.ccp.is", $"/latest/universe/regions/{ID}/?datasource=tranquility"));


            JArray jConsts = (JArray)jRegion["constellations"];
            foreach (var element in jConsts)
            {
                ConstList.Add(element.ToString());
            }
            
            List<string> SystemList = new List<string>();
            foreach(string constellation in ConstList)
            {
                SystemList.AddRange(await Constellation(constellation));
            }
                
            return SystemList;
        }
        public static async Task<List<string>> Constellation(string ID)
        {
            List<string> SystemList = new List<string>();

            JObject jConst = await ESIGenericRequests.GetConstInfo(ID);


            File.AppendAllText(@"constinfo.json", jConst.ToString());


            JArray jSystems = (JArray)jConst["systems"];
            foreach(var element in jSystems)
            {
                SystemList.Add(element.ToString());
   
            }
            return SystemList;
        }
 
    }
}
