using System.ComponentModel.DataAnnotations;

namespace KursavayaECS.Models
{
    public class IndexApproveModel
    {
        [Key]
        public Guid ID { get; set; }

        [Required(ErrorMessage = "Имя необходимо")]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Фамилия необходима")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Телефон необходим")]
        [Display(Name = "Телефон")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Дата рождения необходима")]
        [Display(Name = "Дата рождения")]
        public DateOnly BirthDate { get; set; }


    }
}
