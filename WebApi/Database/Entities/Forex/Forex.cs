using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities
{
    public class Forex : BaseEntity
    {
        public string Time { get; set; } = string.Empty;
        public decimal Usd { get; set; }
        public decimal Eur { get; set; }
        public decimal Gold { get; set; }

        [NotMapped]
        public bool Online { get; set; }
    }
}
