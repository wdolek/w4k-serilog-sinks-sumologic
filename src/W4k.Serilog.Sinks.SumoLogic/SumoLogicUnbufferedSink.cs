using System;
using System.Net.Http;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using SumoLogic.Logging.Common.Log;
using SumoLogic.Logging.Common.Sender;
using W4k.Serilog.Sinks.SumoLogic.Config;
using W4k.Serilog.Sinks.SumoLogic.Extensions;
using W4k.Serilog.Sinks.SumoLogic.Factories;

namespace W4k.Serilog.Sinks.SumoLogic
{
    /// <summary>
    /// SumoLogic Serilog sink (without buffering).
    /// </summary>
    public sealed class SumoLogicUnbufferedSink : ILogEventSink, IDisposable
    {
        /// <summary>
        /// Log service.
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        /// Event log text formatter.
        /// </summary>
        private readonly ITextFormatter _formatter;

        /// <summary>
        /// SumoLogic message sender.
        /// </summary>
        private readonly SumoLogicMessageSender _messageSender;

        /// <summary>
        /// SumoLogic event source describer.
        /// </summary>
        private readonly SumoLogicSource _source;

        /// <summary>
        /// Initializes a new instance of the <see cref="SumoLogicUnbufferedSink"/> class.
        /// </summary>
        /// <param name="log">The log service.</param>
        /// <param name="httpMessageHandler">HTTP message handler.</param>
        /// <param name="connection">Connection configuration.</param>
        /// <param name="source">Event source describer.</param>
        /// <param name="formatter">Text formatter.</param>
        public SumoLogicUnbufferedSink(
            ILog log,
            HttpMessageHandler httpMessageHandler,
            SumoLogicConnection connection,
            SumoLogicSource source,
            ITextFormatter formatter)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            _source = source ?? throw new ArgumentNullException(nameof(source));
            _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));

            _log = log ?? new DummyLog();
            _messageSender = MessageSenderFactory.CreateMessageSender(_log, httpMessageHandler, connection);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="logEvent"/> is <c>null</c>.</exception>
        public void Emit(LogEvent logEvent)
        {
            if (logEvent is null)
            {
                throw new ArgumentNullException(nameof(logEvent));
            }

            if (!_messageSender.CanTrySend)
            {
                _log.Warn("Sink not initialized. Dropping log entry");

                return;
            }

            _messageSender.TrySend(
                _formatter.Format(logEvent),
                _source.SourceName,
                _source.SourceCategory,
                _source.SourceHost);
        }

        /// <inheritdoc/>
        public void Dispose() => _messageSender?.Dispose();
    }
}
