using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
namespace Systems
{
    class Systems
    {
        public static List<string> VistitedSystems = new List<string>();
        static void Main(string[] args)
        {
            SystemCrawler("30000142");
            Console.Read();
        }
        public static async void SystemCrawler(string SystemID)
        {
            VistitedSystems.Add(SystemID);
            foreach (string System in await SystemConnectsTo(SystemID))
            {
                Console.Write("\r{0} Systems parsed", VistitedSystems.Count);
                if (!VistitedSystems.Contains(System))SystemCrawler(System);
            }
        }
        public static async Task<List<string>> SystemConnectsTo(string SystemID) {
            string json = await Requests.GETRequest(@"https://esi.tech.ccp.is", string.Format("/latest/universe/systems/{0}/?datasource=tranquility&language=en-us", SystemID));
            JObject jSystem = JObject.Parse(json);
            List<string> Gates = new List<string>();
            foreach (var element in jSystem["stargates"])
            {
                Gates.Add(element.ToString());
            }
            List<string> Systems = new List<string>();
            foreach(string gate in Gates)
            {
                Systems.Add(await GetSystemFromGate(gate));
            }
            return Systems;
        }
        public static async Task<string> GetSystemFromGate(string GateID)
        {
            string json = await Requests.GETRequest(@"https://esi.tech.ccp.is", string.Format(@"/latest/universe/stargates/{0}/?datasource=tranquility", GateID));
            JObject jGate = JObject.Parse(json);
            return jGate["destination"]["system_id"].ToString();
        }
        public static async Task<string> GetName(string SystemID)
        {
            string json = await Requests.GETRequest(@"https://esi.tech.ccp.is", string.Format("/latest/universe/systems/{0}/?datasource=tranquility&language=en-us", SystemID));
            JObject jSystem = JObject.Parse(json);
            return jSystem["name"].ToString();
        }
    }
}
