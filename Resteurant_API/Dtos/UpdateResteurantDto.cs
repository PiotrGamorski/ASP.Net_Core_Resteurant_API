using System.ComponentModel.DataAnnotations;

namespace Resteurant_API.Dtos
{
    public class UpdateResteurantDto
    {
        [Required]
        [MaxLength(25)]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool HasDelivery { get; set; }
    }
}
