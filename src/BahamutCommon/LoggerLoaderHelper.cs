using NLog.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Targets;

namespace BahamutCommon
{
    public class LoggerLoaderHelper
    {
        public static void LoadLoggerToLoggingConfig(LoggingConfiguration logConfig, IConfiguration Configuration,string sectionString)
        {
            var fileLoggerSection = Configuration.GetSection(sectionString).GetChildren();
            foreach (var logging in fileLoggerSection)
            {
                var target = new FileTarget()
                {
                    FileName = logging["fileName"],
                    Name = logging["name"],
                    Layout = logging["layoutFormat"]
                };
                var minLevel = logging["minLevel"] != null ? LogLevel.FromString(logging["minLevel"]) : null;
                LoggingRule rule = null; 
                if (minLevel != null)
                {
                    rule = new LoggingRule(logging["namePattern"], minLevel, target);
                }
                else
                {
                    rule = new LoggingRule(logging["namePattern"], target);
                }
                var useLevels = logging["logLevel"];
                if(string.IsNullOrWhiteSpace(useLevels) == false)
                {
                    var levels = useLevels.Split(',');
                    foreach (var level in levels)
                    {
                        rule.EnableLoggingForLevel(LogLevel.FromString(level));
                    }
                }
                logConfig.AddTarget(target);
                logConfig.LoggingRules.Add(rule);
            }
        }

        public static void AddConsoleLoggerToLogginConfig(LoggingConfiguration logConfig)
        {
            var consoleLogger = new ColoredConsoleTarget();
            consoleLogger.Name = "ConsoleLogger";
            consoleLogger.Layout = @"${date:format=yyyy-MM-dd HH\:mm\:ss} ${logger}:${message};${exception}";
            logConfig.AddTarget(consoleLogger);
            logConfig.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, consoleLogger));
        }
    }
}
