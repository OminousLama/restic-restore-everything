using NLog;

namespace ResticRestoreEverything.Services;

public class LoggerService(Logger logger)
{

    public void LogDebug(string msg)
    {
        logger.Debug(msg);
    }
    
    public void LogInfo(string msg)
    {
        logger.Info(msg);
    }
    
    public void LogWarn(string msg)
    {
        logger.Warn(msg);
    }
    
    public void LogError(string msg)
    {
        logger.Error(msg);
    }

    public void LogFatal(string msg)
    {
        logger.Fatal(msg);
    }
}