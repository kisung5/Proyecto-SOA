using System.ComponentModel.DataAnnotations;

#nullable disable

namespace API_service.Models
{
    public partial class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        [Required]
        [StringLength(50)]
        public string Email { get; set; }
    }
}
