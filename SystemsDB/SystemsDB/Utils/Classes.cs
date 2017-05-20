using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

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
            security = Math.Round((double)_SystemInfo["security_status"], 1);
            connections = _connections;
        }
        public static System Create(System _system, string CN, string CID, string RN, string RID)
        {
            var ret = _system;
            ret.constname = CN;
            ret.constid = CID;
            ret.regionname = RN;
            ret.regionid = RID;
            return ret;
        }
    }
    public class Constellation
    {
        public string name;
        public List<System> SList;
        public Constellation(string _name, List<System> _SList)
        {
            name = _name;
            SList = _SList;
        }
    }
    public class Region
    {
        public string name;
        public List<Constellation> CList;
        public Region(string _name, List<Constellation> _CList)
        {
            name = _name;
            CList = _CList;
        }
    }
    public class MapChunk
    {
        public List<Region> RList;
        public MapChunk(List<Region> _RList)
        {
            RList = _RList;
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
