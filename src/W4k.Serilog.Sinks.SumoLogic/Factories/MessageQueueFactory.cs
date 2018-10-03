using SumoLogic.Logging.Common.Log;
using SumoLogic.Logging.Common.Queue;
using W4k.Serilog.Sinks.SumoLogic.Config;

namespace W4k.Serilog.Sinks.SumoLogic.Factories
{
    internal static class MessageQueueFactory
    {
        public static BufferWithFifoEviction<string> CreateMessageQueue(ILog log, SumoLogicConnection connection) =>
            new BufferWithFifoEviction<string>(
                connection.MaxQueueSizeBytes,
                new StringLengthCostAssigner(),
                log);
    }
}
