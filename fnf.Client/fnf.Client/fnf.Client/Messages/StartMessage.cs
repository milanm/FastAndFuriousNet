namespace fnf.Client.Messages
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
