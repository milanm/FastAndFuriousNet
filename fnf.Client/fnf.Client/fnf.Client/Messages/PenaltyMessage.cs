using System;
using System.Collections.Generic;
using System.Text;

namespace fnf.Client
{
    public class PenaltyMessage : EventArgs
    {
        public float ActualSpeed { get; set; }
        public float SpeedLimit { get; set; }
        public long Penalty_ms { get; set; }
        public string Barrier { get; set; }
        public string RaceTrack { get; set; }
    }
}
