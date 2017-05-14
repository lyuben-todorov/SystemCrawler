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
            //await AddSystem("Itamo");
            //await AddRegion(await GetSystemList.Region("10000002"));
            await AddMapChunk(await GetSystemList.Region("10000038"));

            //await AddSystem("30000129");
        }
        public static async Task AddMapChunk(List<string> SystemIDList)
        {
            List<System> systems = new List<System>();
            //populate systems list
            foreach(string system in SystemIDList)
            {
                systems.Add(await GetSystem.GetSystemInfo(system));
            }
            //populate nodes
            foreach (System system in systems)
            {
                DBStuff.CreateNode(system.name);
            }
            //populate connections
            foreach (System system in systems)
            {
                foreach(Connection connection in system.connections)
                {
                    DBStuff.CreateEdge(system.name, connection.systemName, connection.gateID);
                }
            }
        }
    }
}
