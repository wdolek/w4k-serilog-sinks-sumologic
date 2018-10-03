using System.Net.Http;
using SumoLogic.Logging.Common.Log;
using SumoLogic.Logging.Common.Sender;
using W4k.Serilog.Sinks.SumoLogic.Config;

namespace W4k.Serilog.Sinks.SumoLogic.Factories
{
    internal static class MessageSenderFactory
    {
        public static SumoLogicMessageSender CreateMessageSender(
            ILog log,
            HttpMessageHandler messageHandler,
            SumoLogicConnection connection) =>
            new SumoLogicMessageSender(messageHandler, log)
            {
                Url = connection.Uri,
                ClientName = connection.ClientName,
                ConnectionTimeout = connection.ConnectionTimeout,
                RetryInterval = connection.RetryInterval,
            };
    }
}
