using System;
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
            try { task.Wait(); }
            catch (Exception e) { Console.WriteLine(e); }
            finally { SystemDB.textlog.Close(); SystemDB.regionlog.Close(); SystemDB.constlog.Close(); SystemDB.systemlog.Close(); SystemDB.gatelog.Close(); Console.Read(); }
        }
    }

    public class SystemDB
    {
        //logging
        public static StreamWriter textlog= new StreamWriter("logfile.txt");
        //caching   
        public static StreamWriter regionlog = new StreamWriter("regioninfo.json");
        public static StreamWriter constlog = new StreamWriter("constinfo.json");
        public static StreamWriter systemlog = new StreamWriter("systeminfo.json");
        public static StreamWriter gatelog = new StreamWriter("gateinfo.json");
        public static async Task Start()
        {
            //Set of regions
            //await AddMapChunk(await GetSystemList.MapChunk(new List<string> { "10000046" }));
            
            //Complete map
            await AddMapChunk(await GetSystemList.MapChunk(await ESIGenericRequests.GetCurrentRegionList()));
        }
        public static async Task AddMapChunk(MapChunk MC)
        {
            //populate nodes
            foreach (Region region in MC.RList)
            {
                foreach (Constellation Const in region.CList)
                {
                    foreach (System system in Const.SList)
                    {
                        DBStuff.CreateSystem(system);
                    }
                }
            }
            //populate connections
            foreach (Region region in MC.RList)
            {
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
}