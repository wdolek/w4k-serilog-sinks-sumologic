using System.Globalization;
using System.IO;
using System.Text;
using Serilog.Events;
using Serilog.Formatting;

namespace W4k.SumoLogic.Logging.Serilog.Extensions
{
    internal static class TextFormatterExtensions
    {
        /// <summary>
        /// Formats provided log event.
        /// </summary>
        /// <param name="formatter">Text formatter.</param>
        /// <param name="logEvent">Log event to format.</param>
        /// <returns>
        /// Log even string value.
        /// </returns>
        public static string FormatEvent(this ITextFormatter formatter, LogEvent logEvent)
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
