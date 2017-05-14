﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4j.Driver.V1;

namespace SystemsDB
{
    class DBStuff
    {
        public static void CreateSystemConnection(string name, string connectsto=null, string gateid = null)
        {
            //proper credentials
            using (var driver = GraphDatabase.Driver(new Uri("bolt://localhost:7687"), AuthTokens.Basic("neo4j", "")))
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