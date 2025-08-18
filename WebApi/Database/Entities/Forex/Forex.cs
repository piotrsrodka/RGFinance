namespace Database.Entities
{
    public class Forex
    {
        public string Time { get; set; } = string.Empty;
        public decimal Usd { get; set; }
        public decimal Eur { get; set; }
        public decimal Gold { get; set; }
    }
}
