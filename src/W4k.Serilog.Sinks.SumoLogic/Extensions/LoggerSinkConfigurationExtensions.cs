﻿using System;
using System.Globalization;
using System.Net.Http;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;
using W4k.Serilog.Sinks.SumoLogic.Config;

namespace W4k.Serilog.Sinks.SumoLogic.Extensions
{
    /// <summary>
    /// Extension methods of <see cref="LoggerConfiguration"/>.
    /// </summary>
    public static class LoggerSinkConfigurationExtensions
    {
        /// <summary>
        /// The default output template.
        /// </summary>
        private const string DefaultOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message} {Exception}";

        /// <summary>
        /// Adds the <c>WriteTo.SumoLogic()</c> extension method to <see cref="LoggerConfiguration"/>.
        /// </summary>
        /// <param name="sinkConfiguration">Logger sink configuration.</param>
        /// <param name="endpointUrl">SumoLogic endpoint URL.</param>
        /// <param name="outputTemplate">
        /// A message template describing the format used to write to the sink.
        /// Default output template is <see cref="DefaultOutputTemplate"/>, set only if <paramref name="formatter"/> is kept <c>null</c>.
        /// </param>
        /// <param name="sourceName">The name used for messages sent to SumoLogic server.</param>
        /// <param name="sourceCategory">The source category for messages sent to SumoLogic server.</param>
        /// <param name="sourceHost">The source host for messages sent to SumoLogic Server.</param>
        /// <param name="clientName">The client name value that is included in each request (used for telemetry).</param>
        /// <param name="connectionTimeout">The connection timeout, in milliseconds.</param>
        /// <param name="retryInterval">The send message retry interval, in milliseconds.</param>
        /// <param name="maxFlushInterval">The maximum interval between flushes, in milliseconds.</param>
        /// <param name="flushingAccuracy">How often the messages queue is checked for messages to send, in milliseconds.</param>
        /// <param name="messagesPerRequest">How many messages need to be in the queue before flushing.</param>
        /// <param name="maxQueueSizeBytes">
        /// The messages queue capacity, in bytes. Messages are dropped When the queue capacity is exceeded.
        /// </param>
        /// <param name="httpMessageHandler">Override HTTP message handler which manages requests to SumoLogic.</param>
        /// <param name="formatter">
        /// Controls the rendering of log events into text, for example to log JSON.
        /// To control plain text formatting supply method with <paramref name="outputTemplate"/> and keep this <c>null</c>.
        /// </param>
        /// <param name="levelSwitch">A switch allowing the pass-through minimum level to be changed at runtime.</param>
        /// <param name="restrictedToMinimumLevel">The minimum level for events passed through the sink. Ignored when <paramref name="levelSwitch"/> is specified.</param>
        /// <returns>
        /// Configuration object allowing method chaining.
        /// </returns>
        public static LoggerConfiguration SumoLogic(
            this LoggerSinkConfiguration sinkConfiguration,
            Uri endpointUrl,
            string outputTemplate = null,
            string sourceName = null,
            string sourceCategory = null,
            string sourceHost = null,
            string clientName = null,
            long? connectionTimeout = null,
            long? retryInterval = null,
            long? maxFlushInterval = null,
            long? flushingAccuracy = null,
            long messagesPerRequest = 100,
            long maxQueueSizeBytes = 1_000_000,
            HttpMessageHandler httpMessageHandler = null,
            ITextFormatter formatter = null,
            LoggingLevelSwitch levelSwitch = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {
            if (endpointUrl is null)
            {
                throw new ArgumentNullException(nameof(endpointUrl));
            }

            var sink = new SumoLogicSink(
                null,
                httpMessageHandler,
                CreateConnection(
                    endpointUrl,
                    clientName,
                    connectionTimeout,
                    retryInterval,
                    maxFlushInterval,
                    flushingAccuracy,
                    messagesPerRequest,
                    maxQueueSizeBytes),
                CreateSource(sourceName, sourceCategory, sourceHost),
                formatter ?? CreateTextFormatter(outputTemplate));

            return sinkConfiguration.Sink(sink, restrictedToMinimumLevel, levelSwitch);
        }

        /// <summary>
        /// Adds the <c>WriteTo.SumoLogicUnbuffered()</c> extension method to <see cref="LoggerConfiguration"/>.
        /// </summary>
        /// <param name="sinkConfiguration">Logger sink configuration.</param>
        /// <param name="endpointUrl">SumoLogic endpoint URL.</param>
        /// <param name="outputTemplate">
        /// A message template describing the format used to write to the sink.
        /// Default output template is <see cref="DefaultOutputTemplate"/>, set only if <paramref name="formatter"/> is kept <c>null</c>.
        /// </param>
        /// <param name="sourceName">The name used for messages sent to SumoLogic server.</param>
        /// <param name="sourceCategory">The source category for messages sent to SumoLogic server.</param>
        /// <param name="sourceHost">The source host for messages sent to SumoLogic Server.</param>
        /// <param name="clientName">The client name value that is included in each request (used for telemetry).</param>
        /// <param name="connectionTimeout">The connection timeout, in milliseconds.</param>
        /// <param name="httpMessageHandler">Override HTTP message handler which manages requests to SumoLogic.</param>
        /// <param name="formatter">
        /// Controls the rendering of log events into text, for example to log JSON.
        /// To control plain text formatting supply method with <paramref name="outputTemplate"/> and keep this <c>null</c>.
        /// </param>
        /// <param name="levelSwitch">A switch allowing the pass-through minimum level to be changed at runtime.</param>
        /// <param name="restrictedToMinimumLevel">The minimum level for events passed through the sink. Ignored when <paramref name="levelSwitch"/> is specified.</param>
        /// <returns>
        /// Configuration object allowing method chaining.
        /// </returns>
        public static LoggerConfiguration SumoLogicUnbuffered(
            this LoggerSinkConfiguration sinkConfiguration,
            Uri endpointUrl,
            string outputTemplate = null,
            string sourceName = null,
            string sourceCategory = null,
            string sourceHost = null,
            string clientName = null,
            long? connectionTimeout = null,
            HttpMessageHandler httpMessageHandler = null,
            ITextFormatter formatter = null,
            LoggingLevelSwitch levelSwitch = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {
            if (endpointUrl is null)
            {
                throw new ArgumentNullException(nameof(endpointUrl));
            }

            var sink = new SumoLogicUnbufferedSink(
                null,
                httpMessageHandler,
                CreateConnection(
                    endpointUrl,
                    clientName,
                    connectionTimeout),
                CreateSource(sourceName, sourceCategory, sourceHost),
                formatter ?? CreateTextFormatter(outputTemplate));

            return sinkConfiguration.Sink(sink, restrictedToMinimumLevel, levelSwitch);
        }

        /// <summary>
        /// Creates SumoLogic connection describer.
        /// </summary>
        /// <returns>
        /// SumoLogic connection describer.
        /// </returns>
        private static SumoLogicConnection CreateConnection(
            Uri endpointUrl,
            string clientName = null,
            long? connectionTimeout = null,
            long? retryInterval = null,
            long? maxFlushInterval = null,
            long? flushAccuracy = null,
            long messagesPerRequest = 100,
            long maxQueueSizeBytes = 1_000_000)
        {
            var connection = new SumoLogicConnection
            {
                Uri = endpointUrl,
                ClientName = clientName,
                MessagesPerRequest = messagesPerRequest,
                MaxQueueSizeBytes = maxQueueSizeBytes,
            };

            connection = connection.SetTimeSpanIfNotEmpty(c => c.ConnectionTimeout, connectionTimeout)
                .SetTimeSpanIfNotEmpty(c => c.RetryInterval, retryInterval)
                .SetTimeSpanIfNotEmpty(c => c.MaxFlushInterval, maxFlushInterval)
                .SetTimeSpanIfNotEmpty(c => c.FlushingAccuracy, flushAccuracy);

            return connection;
        }

        /// <summary>
        /// Creates SumoLogic source describer.
        /// </summary>
        /// <returns>
        /// SumoLogic source describer.
        /// </returns>
        private static SumoLogicSource CreateSource(
            string sourceName = null,
            string sourceCategory = null,
            string sourceHost = null) =>
            new SumoLogicSource
            {
                SourceName = sourceName,
                SourceCategory = sourceCategory,
                SourceHost = sourceHost,
            };

        /// <summary>
        /// Creates text formatter for supplied output template.
        /// </summary>
        /// <param name="outputTemplate">Event log output template.</param>
        /// <returns>
        /// Text formatter for provided output template.
        /// </returns>
        private static ITextFormatter CreateTextFormatter(string outputTemplate)
        {
            outputTemplate = string.IsNullOrWhiteSpace(outputTemplate)
                ? DefaultOutputTemplate
                : outputTemplate;

            return new MessageTemplateTextFormatter(outputTemplate, CultureInfo.InvariantCulture);
        }
    }
}
