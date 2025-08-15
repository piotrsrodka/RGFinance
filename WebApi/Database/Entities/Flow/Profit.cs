using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities
{
    public class Profit : ValueObject
    {
        public Rate Rate { get; set; } = Rate.Monthly;

        [NotMapped]
        public bool IsInterestProfit { get; set; }
    }
}
