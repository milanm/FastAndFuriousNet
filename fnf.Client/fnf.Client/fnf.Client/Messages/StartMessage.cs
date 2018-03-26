using System;
using System.Collections.Generic;
using System.Text;

namespace fnf.Client
{
    public class StartMessage
    {
        public long TimeStamp { get; set; }
        public string TrackId { get; set; }
        public string Type { get; set; }
        public string TeamId { get; set; }
        public string Description { get; set; }
        public bool RecordData { get; set; }
        public string MetaData { get; set; }
    }
}
