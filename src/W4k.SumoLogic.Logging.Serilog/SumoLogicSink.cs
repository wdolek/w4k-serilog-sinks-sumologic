using System;
using System.Net.Http;
using System.Threading;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using SumoLogic.Logging.Common.Log;
using SumoLogic.Logging.Common.Queue;
using SumoLogic.Logging.Common.Sender;
using W4k.SumoLogic.Logging.Serilog.Config;
using W4k.SumoLogic.Logging.Serilog.Extensions;
using W4k.SumoLogic.Logging.Serilog.Factories;

namespace W4k.SumoLogic.Logging.Serilog
{
    public sealed class SumoLogicSink : ILogEventSink, IDisposable
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
        /// Message queue.
        /// </summary>
        private readonly BufferWithEviction<string> _messageQueue;

        /// <summary>
        /// The Sumo HTTP sender executor.
        /// </summary>
        private readonly Timer _flushBufferTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SumoLogicSink"/> class.
        /// </summary>
        /// <param name="log">The log service.</param>
        /// <param name="httpMessageHandler">HTTP message handler.</param>
        /// <param name="connection">Connection configuration.</param>
        /// <param name="source">Event source describer.</param>
        /// <param name="formatter">Text formatter.</param>
        public SumoLogicSink(
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

            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            _log = log ?? new DummyLog();

            _messageSender = MessageSenderFactory.CreateMessageSender(_log, httpMessageHandler, connection);
            _messageQueue = MessageQueueFactory.CreateMessageQueue(_log, connection);
            SumoLogicMessageSenderBufferFlushingTask flushBufferTask = FlushBufferTaskFactory.CreateFlushBufferTask(
                _log,
                _messageSender,
                _messageQueue,
                connection,
                source);

            _flushBufferTimer = new Timer(
                _ => flushBufferTask.Run(),
                null,
                TimeSpan.FromMilliseconds(0),
                connection.FlushingAccuracy);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="logEvent"/> is <c>null</c>.</exception>
        public void Emit(LogEvent logEvent)
        {
            if (logEvent is null)
            {
                throw new ArgumentNullException(nameof(logEvent));
            }

            if (!_messageSender.CanSend)
            {
                _log.Warn("Sink not initialized. Dropping log entry");

                return;
            }

            _messageQueue.Add(_formatter.FormatEvent(logEvent));
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _messageSender?.Dispose();
            _flushBufferTimer?.Dispose();
        }
    }
}
