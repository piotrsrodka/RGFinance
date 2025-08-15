using System.ComponentModel.DataAnnotations;

namespace Database.Entities
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
