namespace Database.Entities
{
    public class Session
    {
        public string SessionId { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public DateTime ExpireAt { get; set; }
    }
}
