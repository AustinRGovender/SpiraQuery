using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiraQuery
{
   public class GetProjects
    {
        public void ReturnAllProjects(SqlConnection connection)
        {
            //Add to new method or class
            //Return List of Project Names in Alphabetical Order
            Console.WriteLine("List of Project Names: ");
            string returnProjectNames = @"select distinct NAME from dbo.TST_PROJECT where IS_ACTIVE = 1 order by NAME";
            //Execute SQL Query Assigned to returnProjectNames
            using (SqlCommand command = new SqlCommand(returnProjectNames, connection))
            {
                //Display the ResultSet to the Console Window
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        Console.WriteLine(string.Format("{0}", reader[0]));
                    }
                }
                finally
                {
                    //close reader to stop connections and get requests from the db
                    reader.Close();
                }
            }
        }
    }

   

}
