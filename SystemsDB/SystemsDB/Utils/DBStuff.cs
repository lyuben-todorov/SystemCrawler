using System;

using Neo4j.Driver.V1;

namespace SystemsDB
{
    class DBStuff
    {

        public static void CreateSystemConnection(string name, string connectsto=null, string gateid = null)
        {
           
            
            using (var driver = GraphDatabase.Driver(new Uri("bolt://localhost:7687"), AuthTokens.Basic(Program.db_username, Program.db_password)))
            {
                using (var session = driver.Session())
                {
                    session.WriteTransaction(tx =>
                    {
                        if (connectsto != null)
                        {
                            tx.Run($"MERGE (n:System {{name: '{name}'}})");
                            tx.Run($"MATCH (a:System),(b:System) WHERE a.name= '{name}' AND b.name = '{connectsto}' MERGE (a)<-[r:`{gateid}`]-(b)");
                        }
                        else tx.Run($"CREATE (a:System) SET a.name = '{name}'");
                    });
                    
                }
            }
        }
    }
}
