using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace SystemsDB
{
    public class System
    {
        public string name;
        public double security;
        public string constid;
        public string regionid;
        public string constname;
        public string regionname;
        public string id;
        public List<Connection> connections;
        public System(JObject _SystemInfo, List<Connection> _connections)
        {
            name = _SystemInfo["name"].ToString();
            id = _SystemInfo["system_id"].ToString();
            constid = _SystemInfo["constellation_id"].ToString();
            security = Math.Round((double)_SystemInfo["security_status"],1);
            connections = _connections;

        }
        public static async Task<System> CreateAsync(JObject _SystemInfo, List<Connection> _connections)
        {
            var ret = new System(_SystemInfo,_connections);
            JObject jConst = await ESIGenericRequests.GetConstInfo(ret.constid);
            ret.constname = jConst["name"].ToString();
            ret.regionid = jConst["region_id"].ToString();
            JObject jRegion = await ESIGenericRequests.GetRegionInfo(ret.regionid);


            File.AppendAllText(@"regioninfo.json", jRegion.ToString());


            ret.regionname = jRegion["name"].ToString();
            return ret;
            
        }
    }

    public class Connection
    {
        public string systemName;
        public string systemID;
        public string gateID;
        public Connection(JObject ConnectionInfo)
        {
            gateID = ConnectionInfo["stargate_id"].ToString();
            systemID = ConnectionInfo["system_id"].ToString();
            systemName = ConnectionInfo["name"].ToString().Split('(', ')')[1];
        }
    }
}
