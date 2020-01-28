using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiraQuery
{
    public class GetIncidents
    {
        //method to return incident stats for user selected project
        public void WriteResultsToFile(string incidentFile, SqlConnection connection)
        {
            //Add to new method or class
            //Ask User Which Project he/she wants to view
            string userProject = "";
            string returnProjectStats;
            Console.WriteLine("Enter The Project Name Or A Few Characters (eg. International MFP --> mfp). . .");


            userProject = Console.ReadLine();

            //Must add in validation for numeric inputs| some projects may be named by means of a CR ? search for CR number
            //Must add in validation for the return of multiple projects??

            //input userProject var into query as limiter
            //parameterize this to reduce chance of sql injection
            returnProjectStats = @"select Project_Name,Incident_id," +
                @"replace(Incident_Name, ',', '_') as Incident_Name," +
                @" coalesce(Severity_Name, 'DEFAULT') as Priority_Name, " +
                @"coalesce(Incident_Status_Name, 'DEFAULT') as Incident_Status_Name" +
                @", Creation_Date, DETECTED_RELEASE_VERSION_NUMBER " +
                @"from dbo.rpt_incidents " +
                @"where Project_Name like '%"
                + userProject + @"%' " +
                @"and is_deleted <> 1";

            using (SqlCommand command = new SqlCommand(returnProjectStats, connection))
            {
                //Display the ResultSet to the Console Window
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    //When writing to file, if incident name has a comma then it will regard that scalar value as a column value

                    using (StreamWriter sw = new StreamWriter(incidentFile, false))
                    {
                        //set all text in created csv to Empty to avoid appending to a huge list.
                        sw.WriteLine(string.Empty);
                    }

                    while (reader.Read())
                    {
                        //Write the results to the console
                        Console.WriteLine(String.Format("{0},{1},{2},{3},{4},{5},{6}", reader[0], reader[1], reader[2], reader[3], reader[4], reader[5], reader[6]));
                        using (StreamWriter file = new StreamWriter(incidentFile, true))
                        {
                            //write the results to the csv file
                            file.WriteLine(String.Format("{0},{1},{2},{3},{4},{5},{6}", reader[0], reader[1], reader[2], reader[3], reader[4], reader[5], reader[6]));
                        }
                    }
                }
                finally
                {
                    //close reader to stop connections and get requests from the db
                    reader.Close();
                }
            }//close using
        }//close writeToFile
    }
}
