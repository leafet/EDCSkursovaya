using System.ComponentModel.DataAnnotations;

namespace KursavayaECS.Data
{
    public class Student
    {
        public required Guid ID { get; set; }
        public required AppUser StudentUser { get; set; }
        [MaxLength(150)]
        public required string PrefferedSpec { get; set; }
    }
}
