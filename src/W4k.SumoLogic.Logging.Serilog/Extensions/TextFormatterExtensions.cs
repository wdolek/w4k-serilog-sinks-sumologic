using System.Globalization;
using System.IO;
using System.Text;
using Serilog.Events;
using Serilog.Formatting;

namespace W4k.SumoLogic.Logging.Serilog.Extensions
{
    internal static class TextFormatterExtensions
    {
        public static string Format(this ITextFormatter formatter, LogEvent logEvent)
        {
            var bodyBuilder = new StringBuilder();
            using (var textWriter = new StringWriter(bodyBuilder, CultureInfo.InvariantCulture))
            {
                formatter.Format(logEvent, textWriter);
                textWriter.WriteLine();
            }

            return bodyBuilder.ToString();
        }
    }
}
