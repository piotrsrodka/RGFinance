using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities
{
  public class ValueObject : BaseEntity
  {
    public string Name { get; set; } = string.Empty;
    public decimal Value { get; set; } = 0;
    [NotMapped]
    public decimal ValuePLN { get; set; } = 0;
    public string Currency { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
  }
}
