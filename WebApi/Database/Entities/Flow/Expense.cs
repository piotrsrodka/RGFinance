namespace Database.Entities
{
    public class Expense : ValueObject
    {
        public Rate Rate { get; set; } = Rate.Monthly;
    }
}
