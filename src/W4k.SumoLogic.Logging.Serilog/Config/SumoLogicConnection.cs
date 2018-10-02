using System;

namespace W4k.SumoLogic.Logging.Serilog.Config
{
    /// <summary>
    /// SumoLogic connection properties.
    /// </summary>
    public class SumoLogicConnection
    {
        /// <summary>
        /// Gets or sets the SumoLogic server URL.
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// Gets or sets the client name value that is included in each request.
        /// This value is used for telemetry purposes to track usage of different clients.
        /// </summary>
        public string ClientName { get; set; } = "sumo-serilog-sender";

        /// <summary>
        /// Gets or sets the connection timeout.
        /// </summary>
        public TimeSpan ConnectionTimeout { get; set; } = TimeSpan.FromMilliseconds(60_000);

        /// <summary>
        /// Gets or sets the send message retry interval.
        /// </summary>
        public TimeSpan RetryInterval { get; set; } = TimeSpan.FromMilliseconds(10_000);

        /// <summary>
        /// Gets or sets the maximum interval between flushes.
        /// </summary>
        public TimeSpan MaxFlushInterval { get; set; } = TimeSpan.FromMilliseconds(10_000);

        /// <summary>
        /// Gets or sets how often the messages queue is checked for messages to send.
        /// </summary>
        public TimeSpan FlushingAccuracy { get; set; } = TimeSpan.FromMilliseconds(250);

        /// <summary>
        /// Gets or sets how many messages need to be in the queue before flushing.
        /// </summary>
        public long MessagesPerRequest { get; set; } = 100;

        /// <summary>
        /// Gets or sets the messages queue capacity, in bytes.
        /// </summary>
        /// <remarks>Messages are dropped When the queue capacity is exceeded.</remarks>
        public long MaxQueueSizeBytes { get; set; } = 1_000_000;
    }
}
