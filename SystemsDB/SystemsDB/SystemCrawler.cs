using System;
using System.Collections.Generic;
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
        public static void SystemCrawler(string SystemID)
        {
            VistitedSystems.Add(SystemID);
            foreach (string System in SystemConnectsTo(SystemID))
            {
                Console.Write("\r{0} Systems parsed", VistitedSystems.Count);
                if (!VistitedSystems.Contains(System))SystemCrawler(System);
            }
        }
        public static List<string> SystemConnectsTo(string SystemID) {
            JObject jSystem = JObject.Parse(Requests.GETRequest(@"https://esi.tech.ccp.is", string.Format("/latest/universe/systems/{0}/?datasource=tranquility&language=en-us", SystemID)));
            List<string> Gates = new List<string>();
            foreach (var element in jSystem["stargates"])
            {
                Gates.Add(element.ToString());
            }
            List<string> Systems = new List<string>();
            foreach(string gate in Gates)
            {
                Systems.Add(GetSystemFromGate(gate));
            }
            return Systems;
        }
        public static string GetSystemFromGate(string GateID)
        {
            JObject jGate = JObject.Parse(Requests.GETRequest(@"https://esi.tech.ccp.is", string.Format(@"/latest/universe/stargates/{0}/?datasource=tranquility", GateID)));
            return jGate["destination"]["system_id"].ToString();
        }
        public static string GetName(string SystemID)
        {
            JObject jSystem = JObject.Parse(Requests.GETRequest(@"https://esi.tech.ccp.is", string.Format("/latest/universe/systems/{0}/?datasource=tranquility&language=en-us", SystemID)));
            return jSystem["name"].ToString();
        }
    }
}
