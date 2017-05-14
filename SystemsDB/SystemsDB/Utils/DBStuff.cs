using System;

using Neo4j.Driver.V1;

namespace SystemsDB
{
    class DBStuff
    {

        public static void CreateEdge(string from, string to, string gateid)
        {
            //auth
            using (var driver = GraphDatabase.Driver(new Uri("bolt://localhost:7687"), AuthTokens.Basic(Program.db_username, Program.db_password)))
            {
                //session
                using (var session = driver.Session())
                {
                    //write connections
                    Console.WriteLine($"Connected {from} to {to} through {gateid}");
                    session.WriteTransaction(tx =>
                    {
                        tx.Run($"MATCH (a:System),(b:System) WHERE a.name= '{from}' AND b.name = '{to}' MERGE (a)<-[r:`{gateid}`]-(b)");

                    });
                    
                }
            }
        }
        public static void CreateNode(string name)
        {
            //auth
            using (var driver = GraphDatabase.Driver(new Uri("bolt://localhost:7687"), AuthTokens.Basic(Program.db_username, Program.db_password)))
            {
                //session
                using (var session = driver.Session())
                {
                    //write nodes
                    Console.WriteLine("Added: " + name);
                    session.WriteTransaction(tx =>
                    {
                        tx.Run($"CREATE (a:System) SET a.name = '{name}'");
                    });

                }
            }
        }
    }
}
