using System;
using System.Collections.Generic;
using Neo4j.Driver.V1;
using System.Linq;

namespace SystemsDB
{
    class Transactions
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
        public static void CreateSystem(System system, bool jSystem = false)
        {
            if (!jSystem)
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
            else // add class, statics and effect in case it's j-space
            {
                using (var driver = GraphDatabase.Driver(new Uri("bolt://localhost:7687"), AuthTokens.Basic(Program.db_username, Program.db_password)))
                {
                    using (var session = driver.Session())
                    {
                        Console.WriteLine($"Added {system.name} {system.security} in {system.constname} in {system.regionname}");
                        SystemDB.textlog.WriteLine($"Added {system.name} {system.security} in {system.constname} in {system.regionname}");
                        session.WriteTransaction(tx =>
                        {
                            tx.Run($"CREATE (a:System{{name:'{system.name}', security:'{system.security}', region:'{system.regionname}', constellation:'{system.constname}', constid:'{system.constid}', regionid:'{system.regionid}', systemid:'{system.id}', class:'{GetJProps.GetClass(system.id)}', effect: '{GetJProps.GetEffect(system.id)}'}})");
                            foreach(string _static in GetJProps.GetStaticList(system.id))
                            {
                                Console.WriteLine($"Added {_static} static to {system.name}");
                                SystemDB.textlog.WriteLine($"Added {_static} static to {system.name}");
                                tx.Run($"MATCH (a:System {{name:'{system.name}'}}) CREATE (b:Static {{name:'{_static}'}}) MERGE (a)-[r:HAS_STATIC]->(b)");
                            }
                        });
                    }
                }
            }


        }

        //J-space
        public static void CreateJRegion(Region region)
        {
            using (var driver = GraphDatabase.Driver(new Uri("bolt://localhost:7687"), AuthTokens.Basic(Program.db_username, Program.db_password)))
            {
                using (var session = driver.Session())
                {
                    Console.WriteLine($"Added {region.name}");
                    SystemDB.textlog.WriteLine($"Added {region.name}");
                    session.WriteTransaction(tx =>
                    {
                        tx.Run($"CREATE (a:JRegion{{name:'{region.name}'}})");
                    });
                }
            }
        }
        public static void CreateJConstellation(Constellation constellation)
        {
            using (var driver = GraphDatabase.Driver(new Uri("bolt://localhost:7687"), AuthTokens.Basic(Program.db_username, Program.db_password)))
            {
                using (var session = driver.Session())
                {
                    Console.WriteLine($"Added {constellation.name}");
                    SystemDB.textlog.WriteLine($"Added {constellation.name}");
                    session.WriteTransaction(tx =>
                    {
                        tx.Run($"CREATE (a:JConstellation{{name:'{constellation.name}', class:'{GetJProps.GetClass(constellation.SList.First().id)}'}})");
                    });
                }
            }
        }
        public static void CreateJConnection(string from, string to)
        {
            using (var driver = GraphDatabase.Driver(new Uri("bolt://localhost:7687"), AuthTokens.Basic(Program.db_username, Program.db_password)))
            {
                using (var session = driver.Session())
                {
                    Console.WriteLine($"Connected {from} to {to}");
                    SystemDB.textlog.WriteLine($"Connected {from} to {to}");
                    session.WriteTransaction(tx =>
                    {
                        tx.Run($"MATCH (a),(b) WHERE a.name= '{from}' AND b.name = '{to}' MERGE (a)-[r:CONTAINS]->(b)");
                    });
                }
            }
        }



        //response queries
        //returns all node properties
        public static List<string> GetSystems(string property)
        {
            using (var driver = GraphDatabase.Driver(new Uri("bolt://localhost:7687"), AuthTokens.Basic(Program.db_username, Program.db_password)))
            {
                using (var session = driver.Session())
                {
                    return session.ReadTransaction(tx =>
                    {
                        var result = tx.Run($"MATCH (a:System) RETURN a.{property} ORDER BY a.{property}");
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
