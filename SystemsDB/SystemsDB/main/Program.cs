using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;

namespace SystemsDB
{
    public class Program
    {
        public static List<string> VisitedSystems = new List<string>();
        public static string db_password;
        public static string db_username;
        static void Main(string[] args)
        {
            
            //load credentials
            using (StreamReader file = File.OpenText(@".env.json"))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject o2 = (JObject)JToken.ReadFrom(reader);
                db_username = (string)o2["neo4j"]["username"];
                db_password = (string)o2["neo4j"]["password"];
            }

            var task = SystemDB.Start();
            //execute async
            try { task.Wait();}
            catch (Exception e){ Console.WriteLine(e); }
            finally { Console.Read(); }
            
        }
    }



    public class SystemDB
    {
        public static async Task Start()
        {
            //Uncomment for first system commit(for new DBs) 
            //DBStuff.CreateSystemConnection("Jita");

            //Add system's CONNECTIONS ONLY to graph
            await AddSystem("Itamo");

        }
        public static async Task AddSystem(string systemname)
        {
            System system = await Crawler.SystemCrawler(await ESIGenericRequests.ESISearch(systemname, "solarsystem"));
            Program.VisitedSystems.Add(systemname);
            foreach (var pair in system.connections)
            {
                //if (!Program.VisitedSystems.Contains(pair.systemName))
                //{
                    DBStuff.CreateSystemConnection(pair.systemName, system.name, pair.gateID);
                //}
                //else Console.WriteLine("omitted " + pair.systemName);
                //Console.WriteLine($"{pair.gateID},{pair.systemID} {pair.systemName}");
            }
        }


    }


    //classes
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
