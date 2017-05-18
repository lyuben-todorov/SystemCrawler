using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;


namespace SystemsDB
{
    public class GetSystem
    {
        public static async Task<System> GetSystemInfo(string SystemID)
        {
            JObject jSystem = await ESIGenericRequests.GetSystemInfo(SystemID);
            return await System.CreateAsync(jSystem, await SystemConnectsTo(jSystem));
            
        }
        private static async Task<List<Connection>> SystemConnectsTo(JObject jSystem)
        {
            JArray jConnections = (JArray)jSystem["stargates"];
            File.AppendAllText(@"systeminfo.json", jSystem.ToString());
            List<Connection> connections = new List<Connection>();
            foreach (var element in jConnections.Children())
            {
                JObject jConnection = await ESIGenericRequests.GetSystemFromGate(element.ToString());
                Connection _connection = new Connection(jConnection);
                connections.Add(_connection);
            }
            return connections;
        }
    }
}
