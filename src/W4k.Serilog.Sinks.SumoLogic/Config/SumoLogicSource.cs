namespace W4k.Serilog.Sinks.SumoLogic.Config
{
    /// <summary>
    /// SumoLogic event source describer.
    /// </summary>
    public class SumoLogicSource
    {
        /// <summary>
        /// Gets or sets the name used for messages sent to SumoLogic server (sent as X-Sumo-Name header).
        /// </summary>
        public string SourceName { get; set; }

        /// <summary>
        /// Gets or sets the source category used for messages sent to SumoLogic server (sent as X-Sumo-Category header).
        /// </summary>
        public string SourceCategory { get; set; }

        /// <summary>
        /// Gets or sets the source host used for messages sent to SumoLogic server (sent as X-Sumo-Host header).
        /// </summary>
        public string SourceHost { get; set; }
    }
}
