namespace Resteurant_API.Dtos
{
    public class CreateDishDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int ResteurantId { get; set; }
    }
}
