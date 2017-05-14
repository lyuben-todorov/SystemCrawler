using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;


namespace SystemsDB
{
    public class GetSystem
    {

        public static async Task<System> GetSystemInfo(string SystemID)
        {
            
            return new System(await ESIGenericRequests.GetSystemInfo(SystemID),await SystemConnectsTo(SystemID));
            
        }
        private static async Task<List<Connection>> SystemConnectsTo(string SystemID)
        {
            JObject jSystem = await ESIGenericRequests.GetSystemInfo(SystemID);
            JArray jConnections = (JArray)jSystem["stargates"];
            List<Connection> connections = new List<Connection>();
            foreach (var element in jConnections.Children())
            {
                Connection _connection = new Connection(await ESIGenericRequests.GetSystemFromGate(element.ToString()));
                connections.Add(_connection);
            }
            return connections;
        }
    }
}
