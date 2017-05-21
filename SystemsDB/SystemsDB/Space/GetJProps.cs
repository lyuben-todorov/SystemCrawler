using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;
namespace SystemsDB
{
    //read info from systems.json 
    public class GetJProps
    {
        public static List<string> GetStaticList(string SystemID)
        {
            List<string> statics = new List<string>();
            try
            {
                
                using (StreamReader file = File.OpenText(@"jsystems.json"))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject staticlist = (JObject)JToken.ReadFrom(reader);
                    JArray jStatics = (JArray)staticlist[$"{SystemID}"]["statics"];
                    foreach (var element in jStatics)
                    {
                        statics.Add(element.ToString());
                    }
                }
            }catch(Exception e)
            {
                Console.WriteLine(e);
                Console.Read();
            }
            return statics;
        }
        public static string GetClass(string SystemID)
        {
            try
            {
                using (StreamReader file = File.OpenText(@"jsystems.json"))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject staticlist = (JObject)JToken.ReadFrom(reader);
                    return (string)staticlist[$"{SystemID}"]["class"];
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.Read();
                return null;
            }
        }
        public static string GetEffect(string SystemID)
        {
            try {
            using (StreamReader file = File.OpenText(@"jsystems.json"))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject staticlist = (JObject)JToken.ReadFrom(reader);
                return (string)staticlist[$"{SystemID}"]["effect"];
            }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.Read();
                return null;
            }
        }
    }
}
