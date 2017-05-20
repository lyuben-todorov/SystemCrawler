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
