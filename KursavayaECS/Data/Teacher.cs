using System.ComponentModel.DataAnnotations;

namespace KursavayaECS.Data
{
    public class Teacher
    {
        public required Guid ID { get; set; }
        public required AppUser TeacherUser { get; set; }
        [MaxLength(50)]
        public required string Specialization { get; set; }
        [MaxLength(50)]
        public required string Category { get; set; }
        public required int ExpirenceYears { get; set; }
    }
}
