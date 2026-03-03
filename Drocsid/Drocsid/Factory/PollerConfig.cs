namespace Drocsid.Factory
{
    internal class PollerConfig
    {
        public Guid PollerId { get; set; }
        public string Url { get; init; } = "";
        public TimeSpan Interval { get; init; }
        public int ChatId { get; set; }
    }
}
