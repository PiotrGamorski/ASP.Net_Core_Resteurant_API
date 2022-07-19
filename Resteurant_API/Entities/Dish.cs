using System.ComponentModel.DataAnnotations;

namespace Resteurant_API.Entities
{
    public class Dish
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }


        public int ResteurantId { get; set; }
        public Resteurant Resteurant { get; set; }
    }
}
