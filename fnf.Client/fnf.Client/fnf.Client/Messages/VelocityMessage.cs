using System;
using System.Collections.Generic;
using System.Text;

namespace fnf.Client
{
    public class VelocityMessage : EventArgs
    {
        public string RaceTrackId { get; set; }
        public long TimeStamp { get; set; }
        public float Velocity { get; set; }
        public int T { get; set; }
    }
}
