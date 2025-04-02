using System.ComponentModel.DataAnnotations;

namespace AuthenPractice.Models.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Salt { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Guid RoleId { get; set; }
        public Role Role { get; set; }
    }
}
