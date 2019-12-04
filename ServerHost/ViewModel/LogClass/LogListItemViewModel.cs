using ServerWork;
using System;

namespace ServerHost
{
    public class LogListItemViewModel : BaseViewModel
    {
        /// <summary>
        /// Source
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Message log
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Type of the log
        /// </summary>
        public LogType TypeLog { get; set; }

        /// <summary>
        /// Color of the type log (#ffffff)
        /// </summary>
        public string LogTypeRGB { get; set; }

        /// <summary>
        /// Datetime log
        /// </summary>
        public DateTime DatetimeLog { get; set; }
    }
}
