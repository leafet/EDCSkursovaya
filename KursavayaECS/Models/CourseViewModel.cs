using KursavayaECS.Data;
using System.ComponentModel.DataAnnotations;

namespace KursavayaECS.Models
{
    public class CourseViewModel
    {
        [Key]
        public Guid ID { get; set; }

        [Required(ErrorMessage = "Название курса необходимо")]
        [Display(Name = "Название")]
        public required string CourseName { get; set; }

        [Required(ErrorMessage = "Описание курса необходимо")]
        [Display(Name = "Описание")]
        public required string CourseDescription { get; set; }

        [Required(ErrorMessage = "Цена необходима")]
        [Display(Name = "Цена")]
        public required int CoursePrice { get; set; }

        [Display(Name = "Преподаватель")]
        public Teacher? Teacher { get; set; }
    }
}
