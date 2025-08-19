using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities
{
    public class ValueObject : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public decimal Value { get; set; } = 0;
        public CurrencyType Currency { get; set; } = CurrencyType.Undefined;
        public string Tags { get; set; } = string.Empty;

        [NotMapped]
        public decimal CurrentCurrencyValue { get; set; } = 0;
     }
}
