using log4net.Core;
using Microsoft.Extensions.Logging;

namespace Nanoleaf_Plugin.Plugin.Logging
{
    public sealed class LumosLogWrapperLogLevelTranslator
    {
        public static Level TranslateLogLevel(LogLevel logLevel, bool criticalEqualsFatal = false)
        {
            Level log4NetLevel = null;
            switch (logLevel)
            {
                case LogLevel.Critical:
                    log4NetLevel = criticalEqualsFatal
                                ? Level.Fatal
                                : Level.Critical;
                    break;
                case LogLevel.Debug:
                    log4NetLevel = Level.Debug;
                    break;
                case LogLevel.Error:
                    log4NetLevel = Level.Error;
                    break;
                case LogLevel.Information:
                    log4NetLevel = Level.Info;
                    break;
                case LogLevel.Warning:
                    log4NetLevel = Level.Warn;
                    break;
                case LogLevel.Trace:
                    log4NetLevel = Level.Trace;
                    break;
            }

            return log4NetLevel;
        }
    }
}
