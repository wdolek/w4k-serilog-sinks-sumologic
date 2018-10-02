using System;
using System.Net.Http;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using SumoLogic.Logging.Common.Log;
using SumoLogic.Logging.Common.Sender;
using W4k.SumoLogic.Logging.Serilog.Config;
using W4k.SumoLogic.Logging.Serilog.Extensions;
using W4k.SumoLogic.Logging.Serilog.Factories;

namespace W4k.SumoLogic.Logging.Serilog
{
    /// <summary>
    /// SumoLogic Serilog event sink.
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
            if (httpMessageHandler is null)
            {
                throw new ArgumentNullException(nameof(httpMessageHandler));
            }

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
                _formatter.FormatEvent(logEvent),
                _source.SourceName,
                _source.SourceCategory,
                _source.SourceHost);
        }

        /// <inheritdoc/>
        public void Dispose() => _messageSender?.Dispose();
    }
}
