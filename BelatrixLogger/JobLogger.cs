using BelatrixLogger.Enum;
using BelatrixLogger.Model;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using BelatrixLogger.Resource;
using System.Data;

namespace BelatrixLogger
{
    public static class JobLogger
    {
        #region Private Members

        private static string _logFileDirectoryPath = ConfigurationManager.AppSettings["LogFileDirectoryPath"];
        private static string _connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        private static string _fileNameFormat = ConfigurationManager.AppSettings["FileNameFormat"];  
        private static string _logMessageFormat = ConfigurationManager.AppSettings["LogMessageFormat"];

        #endregion

        #region Public Method

        /// <summary>
        ///This method is in charge of log the message to console, text file and/or the database accordingly to its configuration.
        /// </summary>
        /// <param name="logMessage"></param>
        public static void LogMessage(Log logMessage)
        {
            ValidateLog(logMessage);

            logMessage.ContentMessage.Trim();
            DateTime creationDate = DateTime.Now;

            LogMessageToDatabase(logMessage, creationDate);
            LogMessageToFile(logMessage, creationDate);
            LogMessageToConsole(logMessage, creationDate);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method prints the log message in the console with a foreground color according to the log type (Error, Warning or Message).
        /// </summary>
        /// <param name="logMessage"></param>
        /// <param name="creationDate"></param>
        private static void LogMessageToConsole(Log logMessage, DateTime creationDate)
        {
            if (logMessage.IsSendToConsole)
            {
                switch (logMessage.LogType)
                {
                    case LogType.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case LogType.Warning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case LogType.Message:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                }  
                Console.WriteLine(string.Format(_logMessageFormat, creationDate.ToString("dd-MM-yyyy HH:mm:ss"), logMessage.ContentMessage));
            }
        }
        
        /// <summary>
        /// This method saves the log message in a text file.
        /// </summary>
        /// <param name="logMessage"></param>
        /// <param name="creationDate"></param>
        private static void LogMessageToFile(Log logMessage, DateTime creationDate)
        {
            if (logMessage.IsSendToFile)
            {
                string fileContent = string.Empty;
                string fileName = string.Format(_fileNameFormat, _logFileDirectoryPath, creationDate.ToString("dd-MM-yyyy"));

                if (!Directory.Exists(_logFileDirectoryPath))
                    Directory.CreateDirectory(_logFileDirectoryPath);
                if (File.Exists(fileName))
                    fileContent = File.ReadAllText(fileName);

                string messageToAdd = string.Format(_logMessageFormat, creationDate.ToString("dd-MM-yyyy HH:mm:ss"), logMessage.ContentMessage);
                fileContent += logMessage.ContentMessage; 

                File.WriteAllText(fileName, fileContent);
            }
        }
        
        /// <summary>
        /// This method insert the log message on the database.
        /// </summary>
        /// <param name="logMessage"></param>
        /// <param name="creationDate"></param>
        private static void LogMessageToDatabase(Log logMessage, DateTime creationDate)
        {
            if (logMessage.IsSendToDatabase)
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand("INSERT_LogMessage", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@message", logMessage.ContentMessage);
                        command.Parameters.AddWithValue("@logType", (int)logMessage.LogType);
                        command.Parameters.AddWithValue("@creationDate", creationDate);

                        connection.Open();
                        command.ExecuteNonQuery();        
                    }                                 
                }
            }
        }
        
        /// <summary>
        /// This method validates that the log message is properly set.
        /// </summary>
        /// <param name="logMessage"></param>
        private static void ValidateLog(Log logMessage)
        {
            if (logMessage == null)
                throw new ArgumentNullException(Messages.Log_Must_Be_Specified);
            if (string.IsNullOrWhiteSpace(logMessage.ContentMessage))
                throw new ArgumentNullException(Messages.Content_Log_Message_Must_Be_Specified);
            if (logMessage.LogType != LogType.Error && logMessage.LogType != LogType.Warning && logMessage.LogType != LogType.Message)
                throw new ApplicationException(Messages.Log_Type_Must_Be_Specefied);
            if (!logMessage.IsSendToConsole && !logMessage.IsSendToFile && !logMessage.IsSendToDatabase)
                throw new ApplicationException(Messages.At_Least_One_Log_Destination_Must_Be_Specified);
        }

        #endregion
    }
}
