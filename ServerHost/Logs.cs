using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerHost
{
    public class Logs
    {
        public void WriteLog(string source, string message, EventLogEntryType type)
        {
            if (!EventLog.SourceExists(source))
                EventLog.CreateEventSource(source, "Server Application");

            EventLog.WriteEntry(source, message, type);
        }

        public List<string> ReadLogs(string source)
        {
            List<string> list = new List<string>();

            var evl = EventLog.GetEventLogs().Where(l => l.Source.Equals(source)).ToList();

            foreach (var eventlogs in evl)
            {
                foreach (var item in eventlogs.Entries)
	            {
                    EventLogEntry elEntry = item as EventLogEntry;

                    list.Add(string.Format("[{0}, {1}] {2}", 
                        elEntry.TimeGenerated.ToString(),
                        elEntry.UserName.ToString(),
                        elEntry.Message));
                }               
            }

            return list;
        }
    }
}
