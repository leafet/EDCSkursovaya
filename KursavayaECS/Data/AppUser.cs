using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace KursavayaECS.Data
{
    [Index(nameof(Email), nameof(ID))]
    public class AppUser
    {
        public required Guid ID { get; set; }
        [MaxLength(50)]
        public required string FirstName { get; set; }
        [MaxLength(50)]
        public required string LastName { get; set; }
        [MaxLength(50)]
        public required string Email { get; set; }
        [MaxLength(20)]
        public required string Phone { get; set; }
        public required string PasswordHash { get; set; }
        [MaxLength(15)]
        public required string Role { get; set; }
        public required DateOnly BirthDate { get; set; }
    }
}
