using System.ComponentModel.DataAnnotations;

namespace AuthenPractice.Models.Entities
{
    public class Role
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Name { get; set; }
    }
}
