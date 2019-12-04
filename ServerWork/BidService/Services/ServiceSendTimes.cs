using System;

namespace ServerWork
{
    /// <summary>
    /// Class for Send Times
    /// </summary>
    public class ServiceSendTimes
    {
        /// <summary>
        /// ID sendtime
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// ID service for sendtime
        /// </summary>
        public int IdService { get; set; }

        /// <summary>
        /// Send Time for service
        /// </summary>
        public DateTime Time { get; set; }
    }
}
