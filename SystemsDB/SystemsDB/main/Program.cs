using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

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
            //Set of k regions
            //await AddKMapChunk(await MakeImport.MapChunk(new List<string> { "10000046" }));

            //Set of j regions
            await AddJMapChunk(await MakeImport.MapChunk(new List<string> { "11000003" }));

            //Complete k map
            //await AddKMapChunk(await MakeImport.MapChunk(await ESIGenericRequests.GetKRegionList()));

            //Complete j map
            //await AddJMapChunk(await MakeImport.MapChunk(await ESIGenericRequests.GetJRegionList()));

        }
        public static async Task AddKMapChunk(MapChunk MC)
        {
            //populate nodes
            foreach (Region region in MC.RList)
            {
                foreach (Constellation Const in region.CList)
                {
                    foreach (System system in Const.SList)
                    {
                        Transactions.CreateSystem(system);
                    }
                }
            }
            //populate connections
            foreach (Region region in MC.RList)
            {
                foreach (Constellation constellation in region.CList)
                {
                    foreach (System system in constellation.SList)
                    {
                        foreach (Connection connection in system.connections)
                        {
                            Transactions.CreateConnection(system.name, connection.systemName, connection.gateID);
                        }
                    }
                }
            }
        }

        //j-space
        public static async Task AddJMapChunk(MapChunk MC)
        {
            //populate nodes
            foreach (Region region in MC.RList)
            {
                Transactions.CreateJRegion(region);
                foreach (Constellation constellation in region.CList)
                {
                    Transactions.CreateJConstellation(constellation);
                    foreach (System system in constellation.SList)
                    {
                        Transactions.CreateSystem(system, true);
                    }
                }
            }
            //populate connections
            foreach (Region region in MC.RList)
            {
                foreach (Constellation constellation in region.CList)
                {
                    Transactions.CreateJConnection(region.name, constellation.name);
                    foreach (System system in constellation.SList)
                    {
                        Transactions.CreateJConnection(constellation.name, system.name);
                    }
                }
            }
        }
    }
}