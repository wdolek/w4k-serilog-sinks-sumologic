using SumoLogic.Logging.Common.Log;
using SumoLogic.Logging.Common.Queue;
using SumoLogic.Logging.Common.Sender;
using W4k.Serilog.Sinks.SumoLogic.Config;

namespace W4k.Serilog.Sinks.SumoLogic.Factories
{
    internal static class FlushBufferTaskFactory
    {
        public static SumoLogicMessageSenderBufferFlushingTask CreateFlushBufferTask(
            ILog log,
            SumoLogicMessageSender messageSender,
            BufferWithEviction<string> messageQueue,
            SumoLogicConnection connection,
            SumoLogicSource source) =>
            new SumoLogicMessageSenderBufferFlushingTask(
                messageQueue,
                messageSender,
                connection.MaxFlushInterval,
                connection.MessagesPerRequest,
                source.SourceName,
                source.SourceCategory,
                source.SourceHost,
                log);
    }
}
