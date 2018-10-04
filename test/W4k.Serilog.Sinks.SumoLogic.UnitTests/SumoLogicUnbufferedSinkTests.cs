using System;
using System.Globalization;
using Serilog;
using Serilog.Core;
using Serilog.Formatting.Display;
using SumoLogic.Logging.Common.Sender;
using W4k.Serilog.Sinks.SumoLogic.Config;
using Xunit;

namespace W4k.Serilog.Sinks.SumoLogic.UnitTests
{
    /// <summary>
    /// Sumo logic target test implementation.   
    /// </summary>
    [Collection("Serilog tests")]
    public class SumoLogicUnbufferedSinkTests : IDisposable
    {
        /// <summary>
        /// The HTTP messages handler mock.
        /// </summary>
        private readonly MockHttpMessageHandler _messagesHandler;
               
        /// <summary>
        /// The SumoLogic sink.
        /// </summary>
        private readonly SumoLogicUnbufferedSink _sink;

        /// <summary>
        /// The Serilog logger.
        /// </summary>
        private readonly Logger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SumoLogicUnbufferedSink"/> class.
        /// </summary>
        public SumoLogicUnbufferedSinkTests()
        {
            _messagesHandler = new MockHttpMessageHandler();

            _sink = new SumoLogicUnbufferedSink(
                null,
                _messagesHandler,
                new SumoLogicConnection
                {
                    Uri = new Uri("http://www.fakeadress.com"),
                    ClientName = "SumoLogicSinkTest",
                },
                new SumoLogicSource
                {
                    SourceName = "SumoLogicSinkTest",
                    SourceCategory = "SumoLogicSinkSourceCategory",
                    SourceHost = "SumoLogicSinkSourceHost",
                },
                new MessageTemplateTextFormatter("{Level:u}: {Message}", CultureInfo.InvariantCulture));

            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Sink(_sink)
                .CreateLogger();
        }
       
        /// <summary>
        /// Test logging of a single message.
        /// </summary>
        [Fact]
        public void SingleMessageTest()
        {
            _logger.Information("This is a message");           
            Assert.Equal(1, _messagesHandler.ReceivedRequests.Count);
            Assert.Equal("INFORMATION: This is a message\r\n", _messagesHandler.LastReceivedRequest.Content.ReadAsStringAsync().Result);
        }

        /// <summary>
        /// Test logging of multiple messages.
        /// </summary>
        [Fact]
        public void MultipleMessagesTest()
        {
            var numMessages = 50;
            for (var i = 0; i < numMessages / 5; i++)
            {               
                _logger.Debug(i.ToString());
                _logger.Information(i.ToString());
                _logger.Warning(i.ToString());
                _logger.Error(i.ToString());
                _logger.Fatal(i.ToString());
            }

            Assert.Equal(numMessages, _messagesHandler.ReceivedRequests.Count);
        }        

        /// <summary>
        /// Test do not logging on the trace level.
        /// </summary>
        [Fact]
        public void NoLogOnTheLevelTraceTest()
        {
            _logger.Verbose("This is message");
            Assert.Equal(0, _messagesHandler.ReceivedRequests.Count);
        }

        /// <summary>
        /// Test logging multiple message and checking the request content.
        /// </summary>
        [Fact]
        public void CheckedRequestContentTest()
        {
            _logger.Debug("This is first message");
            _logger.Information("This is second message");
            _logger.Warning("This is third message");
            _logger.Error("This is fourth message");
            _logger.Fatal("This is fifh message");
            Assert.Equal("FATAL: This is fifh message\r\n", _messagesHandler.ReceivedRequests[4].Content.ReadAsStringAsync().Result);
            Assert.Equal("ERROR: This is fourth message\r\n", _messagesHandler.ReceivedRequests[3].Content.ReadAsStringAsync().Result);
            Assert.Equal("WARNING: This is third message\r\n", _messagesHandler.ReceivedRequests[2].Content.ReadAsStringAsync().Result);
            Assert.Equal("INFORMATION: This is second message\r\n", _messagesHandler.ReceivedRequests[1].Content.ReadAsStringAsync().Result);
            Assert.Equal("DEBUG: This is first message\r\n", _messagesHandler.ReceivedRequests[0].Content.ReadAsStringAsync().Result);
        }
        
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used and optionally disposes of the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to releases only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _sink.Dispose();
                _messagesHandler.Dispose();
            }
        }
    }
}
