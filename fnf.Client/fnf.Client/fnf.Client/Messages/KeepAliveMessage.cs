namespace fnf.Client.Messages
{
    public class KeepAliveMessage
    {
        public string TeamId { get; set; }

        public string AccessCode { get; set; }

        public string OptionalUrl { get; set; }

        public long TimeStamp { get; set; }
    }
}
