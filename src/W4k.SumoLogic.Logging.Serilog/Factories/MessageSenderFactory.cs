using System.Net.Http;
using SumoLogic.Logging.Common.Log;
using SumoLogic.Logging.Common.Sender;
using W4k.SumoLogic.Logging.Serilog.Config;

namespace W4k.SumoLogic.Logging.Serilog.Factories
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
