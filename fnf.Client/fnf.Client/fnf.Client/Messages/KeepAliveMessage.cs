using System;
using System.Collections.Generic;
using System.Text;

namespace fnf.Client
{
    public class KeepAliveMessage
    {
        public string TeamId { get; set; }

        public string AccessCode { get; set; }

        public string OptionalUrl { get; set; }

        public long TimeStamp { get; set; }
    }
}
