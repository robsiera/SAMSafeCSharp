
namespace SamSAFE.Interfaces
{
    public interface ILogger
    {
        int LoggingLevel { get; set; }

        string Output(string level, string message);

        void Warning(string message);

        void Info(string message);

        void Error(string message);

    }
}
