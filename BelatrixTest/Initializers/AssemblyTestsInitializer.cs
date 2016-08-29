using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelatrixTest.Initializers
{
    [TestClass]
    public class AssemblyTestsInitializer
    {
        [AssemblyInitialize()]
        public static void ResetDatabase(TestContext context)
        {
            DeleteAllRecordsOfLogTable();
        }

        private static void DeleteAllRecordsOfLogTable(){
            string _testConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(_testConnectionString))
            {
                string sqlCommand = "DELETE FROM LogMessage;";
                SqlCommand command = new SqlCommand(sqlCommand, connection); 

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
