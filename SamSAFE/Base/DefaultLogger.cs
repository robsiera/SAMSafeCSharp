using System;
using SamSAFE.Interfaces;

namespace SamSAFE.Base
{
    public class DefaultLogger : ILogger
    {
        public int LoggingLevel { get; set; }

        public string Output(string level, string message)
        {
            DateTime timestamp = DateTime.Now;
            var m = $"{level}: {timestamp}: {message}";
            Console.WriteLine(m);
            return m;
        }

        public void Warning(string message)
        {
            if (this.LoggingLevel <= 1)
            {
                this.Output("[WARNING]", message);
            }
        }
        public void Info(string message)
        {
            if (this.LoggingLevel <= 0)
            {
                this.Output("[INFO]", message);
            }
        }
        public void Error(string message)
        {
            if (this.LoggingLevel <= 0)
            {
                this.Output("[ERROR]", message);
            }
        }

    }
}