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
        public static async Task<Region> Region(string ID)
        {
           
            JObject jRegion = await ESIGenericRequests.GetRegionInfo(ID);

            string RN = jRegion["name"].ToString();
            string RID = jRegion["region_id"].ToString();


            JArray jConsts = (JArray)jRegion["constellations"];
            List<string> ConstIDList = new List<string>();
            foreach (var element in jConsts)
            {
                ConstIDList.Add(element.ToString());
            }


            List<Constellation> ConstList = new List<Constellation>();
            foreach(string constellation in ConstIDList)
            {
                ConstList.Add(await Constellation(constellation, RN, RID));
            }

            Region region = new Region(jRegion["name"].ToString(),ConstList);

            return region;
        }


        public static async Task<Constellation> Constellation(string ID, string RN=null, string RID=null)
        {
            
            JObject jConst = await ESIGenericRequests.GetConstInfo(ID);
            if (RN == null && RID == null)
            {
                RID = jConst["region_id"].ToString();
                var r = await ESIGenericRequests.GetRegionInfo(RID);
                RN = r["name"].ToString();
            }
            string CN = jConst["name"].ToString();
            string CID= jConst["constellation_id"].ToString();

            File.AppendAllText(@"constinfo.json", jConst.ToString());

            JArray jSystems = (JArray)jConst["systems"];
            List<System> SystemList = new List<System>();
            foreach (var element in jSystems)
            {
                SystemList.Add(System.Create(await GetSystem.GetSystemInfo(element.ToString()),CN,CID,RN,RID));
            }

            Constellation constellation = new Constellation(jConst["name"].ToString(),SystemList);
            return constellation;
        }
    }
}
