using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiraQuery
{
    public  class GetTestCases
    {
        
        private const string EnterProjectPrompt = "Enter The Project Name Or A Few Characters (eg. International MFP --> mfp). . .";

        public  void WriteListOfTestCases(string testCaseFile, SqlConnection connection)
        {
            //Add to new method or class
            //Ask User Which Project he/she wants to view
            string extractProject = "";
            string returnTestCases;
            Console.WriteLine(EnterProjectPrompt);


            extractProject = Console.ReadLine();

            //Must add in validation for numeric inputs| some projects may be named by means of a CR ? search for CR number
            //Must add in validation for the return of multiple projects??

            //input userProject var into query as limiter
            returnTestCases = "select Project_Name, Test_Case_Id, " +
                "coalesce(replace(Name,',',''),'value not specd') as Test_Name, " +
                "coalesce(replace(Description,',',''),'value not specd') as Test_Description, " +
                "Creation_Date,Last_Update_Date, Author_Name " +
                "from[dbo].[RPT_TESTCASES] " +
                "where Project_Name like '%" + extractProject + "%' and Is_Deleted<> 1";

            using (SqlCommand command = new SqlCommand(returnTestCases, connection))
            {
                //Display the ResultSet to the Console Window
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    //When writing to file, if incident name has a comma then it will regard that scalar value as a column value

                    using (StreamWriter sw = new StreamWriter(testCaseFile, false))
                    {
                        //set all text in created csv to Empty to avoid appending to a huge list.
                        sw.WriteLine(string.Empty);
                    }

                    while (reader.Read())
                    {
                        //Write the results to the console
                        Console.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6}", reader[0], reader[1], reader[2], reader[3], reader[4], reader[5], reader[6]));
                        using (StreamWriter file = new StreamWriter(testCaseFile, true))
                        {
                            //write the results to the csv file
                            file.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6}", reader[0], reader[1], reader[2], reader[3], reader[4], reader[5], reader[6]));
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
