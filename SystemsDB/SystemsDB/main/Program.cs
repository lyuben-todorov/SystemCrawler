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
            //add a list of systems (region,constellation, etc.)
            //await AddMapChunk(await GetSystemList.Region("10000002")); // The Forge
            //await AddMapChunk(await GetSystemList.Region("10000038")); // The Bleak Lands
            //await AddMapChunk(await GetSystemList.Region("10000028"));// Molden Heath
            await AddMapChunk(await GetSystemList.Constellation("20000003"));
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
                DBStuff.CreateSystem(system);
            }
            //populate connections
            foreach (System system in systems)
            {
                foreach(Connection connection in system.connections)
                {
                    DBStuff.CreateConnection(system.name, connection.systemName, connection.gateID);
                }
            }
        }
    }
}
