using System.Globalization;
using Serilog.Formatting;
using Serilog.Formatting.Display;

namespace W4k.SumoLogic.Logging.Serilog.Extensions
{
    internal static class StringExtensions
    {
        /// <summary>
        /// Creates new <see cref="MessageTemplateTextFormatter"/> for provided output template.
        /// </summary>
        /// <param name="outputTemplate">Event log output template.</param>
        /// <returns>
        /// New text formatter.
        /// </returns>
        public static ITextFormatter ToTextFormatter(this string outputTemplate) =>
            new MessageTemplateTextFormatter(outputTemplate, CultureInfo.InvariantCulture);
    }
}
