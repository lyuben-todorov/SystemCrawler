using System;
using System.Collections.Generic;
using Neo4j.Driver.V1;
using System.Linq;

namespace SystemsDB
{
    class DBStuff
    {
        public static void CreateConnection(string from, string to, string gateid)
        {
            //auth
            using (var driver = GraphDatabase.Driver(new Uri("bolt://localhost:7687"), AuthTokens.Basic(Program.db_username, Program.db_password)))
            {
                //session
                using (var session = driver.Session())
                {
                    Console.WriteLine($"Connected {from} to {to} through {gateid}");
                    SystemDB.textlog.WriteLine($"Connected {from} to {to} through {gateid}");
                    session.WriteTransaction(tx =>
                    {
                        tx.Run($"MATCH (a:System),(b:System) WHERE a.name= '{from}' AND b.name = '{to}' MERGE (a)-[r:GATE {{gateid:'{gateid}'}}]->(b)");
                    });
                }
            }
        }
        public static void CreateSystem(System system)
        {
            using (var driver = GraphDatabase.Driver(new Uri("bolt://localhost:7687"), AuthTokens.Basic(Program.db_username, Program.db_password)))
            {
                using (var session = driver.Session())
                {
                    Console.WriteLine($"Added {system.name} {system.security} in {system.constname} in {system.regionname}");
                    SystemDB.textlog.WriteLine($"Added {system.name} {system.security} in {system.constname} in {system.regionname}");
                    session.WriteTransaction(tx =>
                    {
                        tx.Run($"CREATE (a:System{{name:'{system.name}', security:'{system.security}', region:'{system.regionname}', constellation:'{system.constname}', constid:'{system.constid}', regionid:'{system.regionid}', systemid:'{system.id}'}})");
                    });
                }
            }
        }
        public static List<string> GetSystems()
        {
            using (var driver = GraphDatabase.Driver(new Uri("bolt://localhost:7687"), AuthTokens.Basic(Program.db_username, Program.db_password)))
            {
                using (var session = driver.Session())
                {
                    return session.ReadTransaction(tx =>
                    {
                        var result = tx.Run("MATCH (a:System) RETURN a.name ORDER BY a.name");
                        return result.Select(record => record[0].As<string>()).ToList();
                    });
                }
            }
        }
        //returns nodes without property
        public static List<string> GetSystemsWithoutProperty(string property)
        {
            using (var driver = GraphDatabase.Driver(new Uri("bolt://localhost:7687"), AuthTokens.Basic(Program.db_username, Program.db_password)))
            {
                //session
                using (var session = driver.Session())
                {
                    return session.ReadTransaction(tx =>
                    {
                        var result = tx.Run($"MATCH(n) WHERE NOT EXISTS(n.{property}) RETURN n.name");
                        return result.Select(record => record[0].As<string>()).ToList();
                    });
                }
            }
        }
    
    }
}
