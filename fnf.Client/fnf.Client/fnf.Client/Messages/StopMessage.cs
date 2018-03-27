namespace fnf.Client.Messages
{
    public class StopMessage
    {
        public long TimeStamp { get; set; }
        public string TrackId { get; set; }
        public string TeamId { get; set; }
        public string RaceType { get; set; }
    }
}
