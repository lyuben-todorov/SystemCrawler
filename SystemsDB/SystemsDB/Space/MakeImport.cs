using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace SystemsDB
{
    class MakeImport
    {
        public static async Task<MapChunk> MapChunk(List<string> RIDList)
        {
            List<Region> RList = new List<Region>();
            foreach (string region in RIDList)
            {
                RList.Add(await Region(region));
            }
            MapChunk MC = new MapChunk(RList);
            return MC;
        }
        public static async Task<Region> Region(string ID)
        {

            JObject jRegion = await ESIGenericRequests.GetRegionInfo(ID);

            string RN = jRegion["name"].ToString();
            string RID = jRegion["region_id"].ToString();

            await SystemDB.regionlog.WriteLineAsync(jRegion.ToString());

            JArray jConsts = (JArray)jRegion["constellations"];
            List<string> ConstIDList = new List<string>();
            foreach (var element in jConsts)
            {
                ConstIDList.Add(element.ToString());
            }
            List<Constellation> ConstList = new List<Constellation>();
            foreach (string constellation in ConstIDList)
            {
                ConstList.Add(await Constellation(constellation, RN, RID));
            }
            Region region = new Region(jRegion["name"].ToString(), ConstList);

            return region;
        }
        public static async Task<Constellation> Constellation(string ID, string RN = null, string RID = null)
        {

            JObject jConst = await ESIGenericRequests.GetConstInfo(ID);
            if (RN == null && RID == null)
            {
                RID = jConst["region_id"].ToString();
                var r = await ESIGenericRequests.GetRegionInfo(RID);
                RN = r["name"].ToString();
            }
            string CN = jConst["name"].ToString();
            string CID = jConst["constellation_id"].ToString();

            await SystemDB.constlog.WriteLineAsync(jConst.ToString());

            JArray jSystems = (JArray)jConst["systems"];
            List<System> SystemList = new List<System>();
            foreach (var element in jSystems)
            {
                SystemList.Add(System.Create(await _System(element.ToString()), CN, CID, RN, RID));
            }

            Constellation constellation = new Constellation(jConst["name"].ToString(), SystemList);
            return constellation;
        }
        public static async Task<System> _System(string SystemID, bool JSystem = false)
        {
            JObject jSystem = await ESIGenericRequests.GetSystemInfo(SystemID);
            return new System(jSystem, await SystemConnectsTo(jSystem));

        }
        private static async Task<List<Connection>> SystemConnectsTo(JObject jSystem)
        {
            JArray jConnections = (JArray)jSystem["stargates"];
            await SystemDB.systemlog.WriteLineAsync(jSystem.ToString());
            List<Connection> connections = new List<Connection>();
            foreach (var element in jConnections.Children())
            {
                JObject jConnection = await ESIGenericRequests.GetSystemFromGate(element.ToString());
                await SystemDB.gatelog.WriteLineAsync(jConnection.ToString());
                Connection _connection = new Connection(jConnection);
                connections.Add(_connection);
            }
            return connections;
        }
    }
}
