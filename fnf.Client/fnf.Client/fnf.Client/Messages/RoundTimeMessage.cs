namespace fnf.Client.Messages
{
    public class RoundTimeMessage 
    {
        public long Timestamp { get; set; }
        public long RoundDuration { get; set; }
        public string Team { get; set; }
        public string Track { get; set; }
    }
}
