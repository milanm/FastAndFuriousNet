namespace fnf.Client.Messages
{
    public class SensorMessage 
    {
        public string RaceTrackId { get; set; }
        public long TimeStamp { get; set; }
        public int[] A { get; set; }
        public int[] G { get; set; }
        public int[] M { get; set; }
        public int T { get; set; }
    }
}
