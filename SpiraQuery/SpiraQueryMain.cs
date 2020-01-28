using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace SpiraQuery
{
    class SpiraQueryMain
    {
        private const string DownloadPrompt = "Do you want to download a list of Test Cases?  y/n";
        private const string DownloadPromptInci = "Do you want to download a list of Incidents?  y/n";
        private const string DownloadPromptReq = "Do you want to download a list of Requirements?  y/n";
        private const string ConnectionPrompt = "\r\n" + "*** Connection Established Successfully *** " + "\r\n";
        private const string ExitPrompt = "Press any key to exit. .";
        private const string TestCaseFilePath = @"\SpiraFolder\TestCaseList.csv";
        private const string IncidentStatsPath = @"\SpiraFolder\IncidentStats.csv";
        private const string RequirmentsFilePath = @"\SpiraFolder\RequirementsList.csv";
        private const string SaveLocationPrompt = "ALL FILES WILL BE SAVED TO YOUR DOCUMENTS FOLDER" + "\r\n";

        private const string SpiraQueryAscii = @"  _________       .__              ________                              " + "\r\n" +
                                               @" /   _____/_____  |__|__________   \_____  \  __ __   ___________ ___.__." + "\r\n" +
                                                @" \_____  \\____ \ |  \_ __ \__  \   /  / \  \|  |  \_/ __ \_  __ <   |  |  " + "\r\n" +
                                                @"  /        \  |_> >  ||  |\/ / __\_/   \_/.  \  |  /\  ___/|  | \/\___  |" + "\r\n" +
                                               @" /_______  /   __/|__||__|  (____  /\_____ \_/____/  \___  >__|    / ___|" + "\r\n" +
                                               @"         \/|__|                  \/       \__>           \/        \/     ";

        static void Main(string[] args)
        {

            //establish connections

            EstablishConnection();

            Console.WriteLine(ExitPrompt);
            Console.ReadKey();
            //ReadKey so the Console doesn't immediately exit in the event of an error occuring
        }

        private static void EstablishConnection()
        {//this method initialized all files and the connection
            try
            {

                Console.WriteLine("Version 1.31");
                Console.WriteLine(SpiraQueryAscii);
                System.Threading.Thread.Sleep(2000);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\r\n*** Connecting to SpiraTest Database *** " + "\r\n");
                Console.ResetColor();
                for (int i = 0; i <= 40; i++)
                {
                    Thread.Sleep(150);
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.Write(" ");
                }
                Console.ResetColor();
                Console.WriteLine("");

                //for saving results
                var filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                //Create SpiraFolder in documents
                Directory.CreateDirectory(filePath + "\\SpiraFolder\\");
                string incidentFile = filePath + IncidentStatsPath;
                //create test case file
                string testCaseFile = filePath + TestCaseFilePath;
                //create Requirements File
                string requirementsFile = filePath + RequirmentsFilePath;

                //Building the connection string
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
                {
                    DataSource = @"MRPGSQLDB1\MRPGSQLDB1",
                    IntegratedSecurity = true,//windows authentication - AD Login --Ballistix does not support SQL Auth
                    InitialCatalog = "SpiraTest"
                };

                //Establish the connection to Ballistix
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    System.Threading.Thread.Sleep(2000);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(ConnectionPrompt);
                    System.Threading.Thread.Sleep(2000);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(SaveLocationPrompt);
                    Console.ResetColor();
                    //Console.WriteLine(ContinuePrompt);

                    GetIncidents getIncidents = new GetIncidents();
                    GetProjects getProjects = new GetProjects();
                    GetTestCases getTestCases = new GetTestCases();
                    GetRequirements getRequirements = new GetRequirements();

                    string isWriteIncidentsTrue = "";
                    System.Threading.Thread.Sleep(2000);
                    Console.WriteLine(DownloadPromptInci);
                    isWriteIncidentsTrue = Console.ReadLine();

                    if (isWriteIncidentsTrue != "y")
                    {
                        Console.WriteLine("Moving On . . .");
                    }
                    else
                    {
                        getProjects.ReturnAllProjects(connection);
                        getIncidents.WriteResultsToFile(incidentFile, connection);
                    }


                    //Console.Clear();
                    //method WriteListOfTestCases
                    string isTcListTrue = "";
                    Console.WriteLine("\r\n" + DownloadPrompt);
                    isTcListTrue = Console.ReadLine();

                    if (isTcListTrue != "y")
                    {
                        Console.WriteLine("Moving On . . .");
                    }
                    else
                    {
                        Console.Clear();
                        getProjects.ReturnAllProjects(connection);

                        getTestCases.WriteListOfTestCases(testCaseFile, connection);
                    }

                    //Console.Clear();
                    //method writeRequirementsToFile
                    string isDownloadRequirements = "";
                    Console.WriteLine("\r\n" + DownloadPromptReq);
                    isDownloadRequirements = Console.ReadLine();

                    if (isDownloadRequirements != "y")
                    {
                        Console.WriteLine(ExitPrompt);
                    }
                    else
                    {
                        Console.Clear();
                        getProjects.ReturnAllProjects(connection);

                        getRequirements.WriteRequirementsToFile(requirementsFile, connection);
                    }
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("Connection Failed");
            }
        }
    }
}
