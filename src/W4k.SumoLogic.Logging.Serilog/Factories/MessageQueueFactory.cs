using SumoLogic.Logging.Common.Log;
using SumoLogic.Logging.Common.Queue;
using W4k.SumoLogic.Logging.Serilog.Config;

namespace W4k.SumoLogic.Logging.Serilog.Factories
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
