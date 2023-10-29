using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Runtime.Versioning;

namespace Nanoleaf_Plugin.Plugin.Logging
{
    [UnsupportedOSPlatform("browser")]
    [ProviderAlias("LumosLogWrapperProvider")]
    public class LumosLogWrapperProvider : ILoggerProvider, ISupportExternalScope
    {
        private readonly string _name;

        /// <summary>
        /// The loggers collection.
        /// </summary>
        private readonly ConcurrentDictionary<string, LumosLogWrapperLogger> loggers = new();

        /// <summary>
        /// Prevents to dispose the object more than single time.
        /// </summary>
        private bool disposedValue = false;

        /// <summary>
        /// The external logging scope provider.
        /// </summary>
        /// <remarks>
        /// Reading the offical logging implementations, it seems like we need to handle the case that this might never be set.
        /// We handle it with a NullScopeProvider instead of null checks, to make the process of implementing interfaces like
        /// <see cref="ILog4NetLoggingEventFactory"/> less error prone for consumers.
        /// </remarks>
        public IExternalScopeProvider ExternalScopeProvider { get; private set; } = NullScopeProvider.Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="LumosLogWrapperProvider"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <exception cref="ArgumentNullException">options</exception>
        /// <exception cref="NotSupportedException">Wach cannot be true when you are overwriting config file values with values from configuration section.</exception>
        public LumosLogWrapperProvider(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Finalizes the instance of the <see cref="LumosLogWrapperProvider"/> object.
        /// </summary>
        ~LumosLogWrapperProvider()
        {
            Dispose(false);
        }

        /// <summary>
        /// Creates the logger.
        /// </summary>
        /// <returns>An instance of the <see cref="ILogger"/>.</returns>
        public ILogger CreateLogger()
            => this.CreateLogger(_name);

        /// <summary>
        /// Creates the logger.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <returns>An instance of the <see cref="ILogger"/>.</returns>
        public ILogger CreateLogger(string categoryName)
            => this.loggers.GetOrAdd(categoryName, this.CreateLoggerImplementation);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.loggers.Clear();
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Creates the logger implementation.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The <see cref="LumosLogWrapperLogger"/> instance.</returns>
        private LumosLogWrapperLogger CreateLoggerImplementation(string name)
        {
            return new LumosLogWrapperLogger(name, ExternalScopeProvider);
        }

        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            ExternalScopeProvider = scopeProvider;
        }
    }
}
