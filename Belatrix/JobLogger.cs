using System; 
using System.Linq; 
using System.Text;

namespace Belatrix
{    
    public class JobLogger
    {
        private static bool _logToFile; 
        private static bool _logToConsole; 
        private static bool _logMessage; 
        private static bool _logWarning; 
        private static bool _logError; 
        private static bool LogToDatabase; 
        private bool _initialized;
        public JobLogger (bool logToFile,bool logToConsole,bool logToDatabase,bool logMessage,bool logWarning,bool logWarning)
	{

	}
        public JobLogger(bool logToFile,bool logToConsole,bool logToDatabase,bool logMessage,bool logWarning,bool logError) { 
            _logError= logError; 
            _logMessage=logMessage; 
            _logWarning=logWarning; 
            LogToDatabase=logToDatabase; 
            _logToFile=logToFile; 
            _logToConsole=logToConsole; 
        } 
    }
}
