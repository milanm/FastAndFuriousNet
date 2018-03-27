namespace fnf.Client.Messages
{
    public class PenaltyMessage 
    {
        public float ActualSpeed { get; set; }
        public float SpeedLimit { get; set; }
        public long PenaltyMs { get; set; }
        public string Barrier { get; set; }
        public string RaceTrack { get; set; }
    }
}
