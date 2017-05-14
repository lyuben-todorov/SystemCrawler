using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace SystemsDB
{
    public class System
    {
        public string name;
        public string id;
        public List<Connection> connections;
        public System(JObject _SystemInfo, List<Connection> _connections)
        {
            name = _SystemInfo["name"].ToString();
            id = _SystemInfo["system_id"].ToString();
            //security = _SystemInfo["security_status"].ToString();
            connections = _connections;

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
