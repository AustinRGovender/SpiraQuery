using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiraQuery
{
    public class GetRequirements
    {
        private const string EnterProjectPrompt = "Enter The Project Name Or A Few Characters (eg. International MFP --> mfp). . .";
        public void WriteRequirementsToFile(string requirmentsFile, SqlConnection connection)
        {

            string getProject="";
            string returnRequirements = null;

            Console.WriteLine(EnterProjectPrompt);
            getProject = Console.ReadLine();

            returnRequirements = @"select Requirement_id, replace(NAME,',','-'), AUTHOR_NAME, COVERAGE_COUNT_TOTAL from dbo.RPT_REQUIREMENTS" + 
                                    @" where PROJECT_NAME like '%"+getProject+"%' and IS_DELETED <> 1 " +
                                    @"order by Requirement_id, NAME, AUTHOR_NAME, COVERAGE_COUNT_TOTAL";

            using (SqlCommand command = new SqlCommand(returnRequirements, connection))
            {
                //Display the ResultSet to the Console Window
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    //When writing to file, if incident name has a comma then it will regard that scalar value as a column value

                    using (StreamWriter sw = new StreamWriter(requirmentsFile, false))
                    {
                        //set all text in created csv to Empty to avoid appending to a huge list.
                        sw.WriteLine(string.Empty);
                    }

                    while (reader.Read())
                    {
                        //Write the results to the console
                        Console.WriteLine(string.Format("{0},{1},{2},{3}", reader[0], reader[1], reader[2], reader[3]));
                        using (StreamWriter file = new StreamWriter(requirmentsFile, true))
                        {
                            //write the results to the csv file
                            file.WriteLine(string.Format("{0},{1},{2},{3}", reader[0], reader[1], reader[2], reader[3]));
                        }
                    }
                }
                finally
                {
                    //close reader to stop connections and get requests from the db
                    reader.Close();
                }
            }//close using
        }//close writeListOfTestCases

    }


    }
