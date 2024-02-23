using System.ComponentModel.DataAnnotations;

namespace Kitap.Models.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(100)]
        public required string Name { get; set; }
        [Range(0, int.MaxValue)]
        public int DisplayOrder { get; set; }
    }
}
