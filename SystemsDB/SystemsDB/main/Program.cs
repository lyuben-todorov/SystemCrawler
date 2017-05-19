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
            await AddMapChunk(await GetSystemList.Region("10000046"));
    }
        public static async Task AddMapChunk(Region region = null, Constellation constellation = null)
        {

            //populate nodes
            foreach(Constellation Const in region.CList)
            {
                foreach (System system in Const.SList)
                {
                    DBStuff.CreateSystem(system);
                }
            }
            //populate connections
            foreach (Constellation Const in region.CList)
            {
                foreach (System system in Const.SList)
                {
                    foreach (Connection connection in system.connections)
                    {
                        DBStuff.CreateConnection(system.name, connection.systemName, connection.gateID);
                    }
                }
            }
        }
    }
}
