using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BelatrixLogger;
using BelatrixLogger.Model;
using BelatrixLogger.Enum;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;
using Rhino.Mocks;
using SystemWrapper.IO;

namespace BelatrixTest
{
    [TestClass]
    public class JobLoggerTests
    {
        #region Validation Rules Category

        [TestMethod]
        [TestCategory("Validation Rules Category")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Given_LogIsNull_ThenInvalidException()
        {
            //Arrange
            Log logMessage = null;

            //Act
            JobLogger.LogMessage(logMessage);
        }

        [TestMethod]
        [TestCategory("Validation Rules Category")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Given_LogContentMessageIsNullOrWhiteSpace_ThenInvalidException()
        {
            //Arrange
            Log logMessage = new Log
            {
                ContentMessage = string.Empty,
                IsSendToConsole = true,
                LogType = LogType.Error
            };

            //Act (JobLogger is my System Under Tests)
            JobLogger.LogMessage(logMessage);
        }

        [TestMethod]
        [TestCategory("Validation Rules Category")]
        [ExpectedException(typeof(ApplicationException))]
        public void Given_LogTypeIsNull_ThenInvalidException()
        {
            //Arrange
            Log logMessage = new Log
            {
                ContentMessage = "Message to log.",
                IsSendToConsole = true,
                IsSendToFile = true,
                IsSendToDatabase = true
            };

            //Act
            JobLogger.LogMessage(logMessage);
        }

        [TestMethod]
        [TestCategory("Validation Rules Category")]
        [ExpectedException(typeof(ApplicationException))]
        public void Given_NoDestinyToLogIsSpecified_ThenInvalidException()
        {
            //Arrange
            Log logMessage = new Log
            {
                ContentMessage = "Message to log.",
                IsSendToConsole = false,
                IsSendToFile = false,
                IsSendToDatabase = false,
                LogType = LogType.Error
            };

            //Act
            JobLogger.LogMessage(logMessage);
        }

        #endregion

        #region Log Message To Console Category

        [TestMethod]
        [TestCategory("LogMessageToConsole")]
        public void Given_LogToConsoleMessageTypeError_ThenShouldWriteToConsoleFormattedMessageInRedColor()
        {
            using (StringWriter sw = new StringWriter())
            {
                //Arrange  
                Log logMessage = new Log
                {
                    ContentMessage = "Message to log.",
                    IsSendToConsole = true,
                    IsSendToFile = false,
                    IsSendToDatabase = false,
                    LogType = LogType.Error
                };
                Console.SetOut(sw);

                //Act
                JobLogger.LogMessage(logMessage);

                //Assert
                string creationDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                string expectedMessageLogged = string.Format("{0} {1}\r\n", creationDate, logMessage.ContentMessage);
                bool firstAssertion = expectedMessageLogged.Equals(sw.ToString());
                bool secondAssertion = Console.ForegroundColor == ConsoleColor.Red;

                Assert.IsTrue(firstAssertion && secondAssertion);
            }
        }

        [TestMethod]
        [TestCategory("LogMessageToConsole")]
        public void Given_LogToConsoleMessageTypeWarning_ThenShouldWriteToConsoleFormattedMessageInYellowColor()
        {
            using (StringWriter sw = new StringWriter())
            {
                //Arrange  
                Log logMessage = new Log
                {
                    ContentMessage = "Message to log.",
                    IsSendToConsole = true,
                    IsSendToFile = false,
                    IsSendToDatabase = false,
                    LogType = LogType.Warning
                };
                Console.SetOut(sw);

                //Act
                JobLogger.LogMessage(logMessage);

                //Assert
                string creationDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                string expectedMessageLogged = string.Format("{0} {1}\r\n", creationDate, logMessage.ContentMessage);
                bool firstAssertion = expectedMessageLogged.Equals(sw.ToString());
                bool secondAssertion = Console.ForegroundColor == ConsoleColor.Yellow;

                Assert.IsTrue(firstAssertion && secondAssertion);
            }
        }

        [TestMethod]
        [TestCategory("LogMessageToConsole")]
        public void Given_LogToConsoleMessageTypeMessage_ThenShouldWriteToConsoleFormattedMessageInWhiteColor()
        {
            using (StringWriter sw = new StringWriter())
            {
                //Arrange  
                Log logMessage = new Log
                {
                    ContentMessage = "Message to log.",
                    IsSendToConsole = true,
                    IsSendToFile = false,
                    IsSendToDatabase = false,
                    LogType = LogType.Message
                };
                Console.SetOut(sw);

                //Act
                JobLogger.LogMessage(logMessage);

                //Assert
                string creationDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                string expectedMessageLogged = string.Format("{0} {1}\r\n", creationDate, logMessage.ContentMessage);
                bool firstAssertion = expectedMessageLogged.Equals(sw.ToString());
                bool secondAssertion = Console.ForegroundColor == ConsoleColor.White;

                Assert.IsTrue(firstAssertion && secondAssertion);
            }
        }

        #endregion

        #region Log Message To DataBase Category

        [TestMethod]
        [TestCategory("LogMessageToDataBase")]
        public void Given_LogToDataBaseMessageTypeError_ThenShouldInsertOnTable()
        {
            //Arrange  
            Log logMessage = new Log
            {
                ContentMessage = "Message to log.",
                IsSendToConsole = false,
                IsSendToFile = false,
                IsSendToDatabase = true,
                LogType = LogType.Error
            };

            //Act
            JobLogger.LogMessage(logMessage);

            //Assert                                   
            int totalItems = GetTotalItemsFromTable();
            int expectedTotalItems = 1;
            Assert.AreEqual<int>(expectedTotalItems, totalItems);
        }

        #endregion
         
        #region Helper

        private int GetTotalItemsFromTable()
        {
            string _testConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(_testConnectionString))
            {
                string sqlCommand = "SELECT Count(*) FROM LogMessage;";
                SqlCommand command = new SqlCommand(sqlCommand, connection);
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());  
            }
        }

        #endregion         
    }
}
