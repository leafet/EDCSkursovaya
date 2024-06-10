using KursavayaECS.Data;
using System.ComponentModel.DataAnnotations;

namespace KursavayaECS.Models
{
    public class TeacherApproveModel
    {
        [Required(ErrorMessage = "Специальность необходима")]
        [Display(Name = "Специальность")]
        public required string Specialization { get; set; }

        [Required(ErrorMessage = "Категория необходима")]
        [Display(Name = "Категория")]
        public required string Category { get; set; }

        [Required(ErrorMessage = "Опыт необходим")]
        [Display(Name = "Опыт работы (года)")]
        public required int ExpirenceYears { get; set; }
    }
}
