using EventFlow.Logs;
using Serilog;
using System;

namespace SchedulingApi
{
    public class SerilogLogger : EventFlow.Logs.Log
    {
        private readonly ILogger logger;

        public SerilogLogger(ILogger logger)
        {
            this.logger = logger;

        }
        protected override bool IsDebugEnabled { get; }
        protected override bool IsInformationEnabled { get; }

        protected override bool IsVerboseEnabled { get; }

        public override void Write(LogLevel logLevel, string format, params object[] args)
        {

            logger.Write((Serilog.Events.LogEventLevel)logLevel, string.Format(format, args));
        }

        public override void Write(LogLevel logLevel, Exception exception, string format, params object[] args)
        {
            logger.Write((Serilog.Events.LogEventLevel)logLevel, exception, string.Format(format, args));


        }
    }
}