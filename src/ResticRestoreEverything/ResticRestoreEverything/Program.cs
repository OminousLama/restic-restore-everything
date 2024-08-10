using CommandLine;
using NLog;
using NLog.Config;
using NLog.Targets;
using ResticRestoreEverything.Models.Etc;
using ResticRestoreEverything.Services;

namespace ResticRestoreEverything;

static class Program
{
    public static int Main(string[] args)
    {
        #region Bootstrap
        var result = Parser.Default.ParseArguments<Options>(args);

        if (result.Errors.Any()) return 1;

        #region Logging
        Logger loggerSource = LogManager.GetLogger("Main");
        var config = new LoggingConfiguration();

        var consoleTarget = new ColoredConsoleTarget("console")
        {
            Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message} ${exception:format=ToString,StackTrace}"
        };

        config.AddTarget(consoleTarget);
        config.AddRule(result.Value.Verbose ? LogLevel.Debug : LogLevel.Info, LogLevel.Fatal, consoleTarget);
        LogManager.Configuration = config;
        LoggerService log = new LoggerService(loggerSource);

        #endregion
        #endregion
        #region Business Logic
        RestoreService rs = new(log, result.Value.Repo, result.Value.Secret, result.Value.Target, false);

        rs.IndexFindings(result.Value.FindingsFile);
        rs.RestoreFiles();
        #endregion
        return 0;
    }
}