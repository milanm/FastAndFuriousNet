using System;
using System.Collections.Generic;
using System.Text;

namespace fnf.Client
{
    public class RoundTimeMessage
    {
        public long Timestamp { get; set; }
        public long RoundDuration { get; set; }
        public string Team { get; set; }
        public string Track { get; set; }
    }
}
