using log4net.Core;
using LumosLIB.Kernel.Log;
using Microsoft.Extensions.Logging;
using System;
using IMELLogger = Microsoft.Extensions.Logging.ILogger;

namespace Nanoleaf_Plugin.Plugin.Logging
{
    public sealed class LumosLogWrapperLogger : IMELLogger
    {
        private readonly string _name;
        private readonly ILumosLog logger;
        private readonly IExternalScopeProvider externalScopeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4NetLogger"/> class.
        /// </summary>
        /// <param name="options">The log4net provider options.</param>
        public LumosLogWrapperLogger(string name, IExternalScopeProvider externalScopeProvider)
        {
            _name = name;
            this.externalScopeProvider = externalScopeProvider ?? throw new ArgumentNullException(nameof(externalScopeProvider));
            this.logger = LumosLogger.getInstance(_name);
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;

        public bool IsEnabled(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                case LogLevel.Information:
                case LogLevel.Warning:
                case LogLevel.Error:
                case LogLevel.Critical:
                    return true;
                case LogLevel.None:
                default: return false;
            }
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var candidate = new MessageCandidate<TState>(logLevel, eventId, state, exception, formatter);
            LoggingEvent loggingEvent = LumosLogWrapperEventFactory.getInstance().CreateLoggingEvent(in candidate, logger.AsILog().Logger, externalScopeProvider);
            if (loggingEvent == null)
                return;
            logger.AsILog().Logger.Log(loggingEvent);
        }
    }
}
