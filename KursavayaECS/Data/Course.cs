using System.ComponentModel.DataAnnotations;

namespace KursavayaECS.Data
{
    public class Course
    {
        public required Guid ID { get; set; }
        
        [MaxLength(50)]
        public required string CourseName { get; set; }
        public required string CourseDescription { get; set; }
        public required int CoursePrice { get; set; }
        public Teacher ?Teacher { get; set; }
    }
}
