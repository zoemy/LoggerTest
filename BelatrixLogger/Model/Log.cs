using BelatrixLogger.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelatrixLogger.Model
{
    public class Log
    {
        public string ContentMessage { get; set; }
        public LogType LogType { get; set; }
        public bool IsSendToDatabase { get; set; }
        public bool IsSendToFile { get; set; }
        public bool IsSendToConsole { get; set; }
    }
}
