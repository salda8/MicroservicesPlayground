using EventFlow.Logs;
using NLog;
using System;

namespace SchedulingApi
{
    public class MyLogger : Log
    {
        private readonly ILogger logger;

        public MyLogger(ILogger logger)
        {
            this.logger = logger;

        }
        protected override bool IsDebugEnabled { get; }
        protected override bool IsInformationEnabled { get; }

        protected override bool IsVerboseEnabled { get; }

        public override void Write(EventFlow.Logs.LogLevel logLevel, string format, params object[] args)
        {
            logger.Log(NLog.LogLevel.FromOrdinal((int)logLevel), string.Format(format, args));
        }

        public override void Write(EventFlow.Logs.LogLevel logLevel, Exception exception, string format, params object[] args)
        {
            logger.Log(NLog.LogLevel.FromOrdinal((int)logLevel), exception, string.Format(format, args));


        }
    }
}