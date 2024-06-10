using System.ComponentModel.DataAnnotations;

namespace KursavayaECS.Models
{
    public class RegisterViewModel
    {
        [Key]
        public Guid ID { get; set; }

        [Required(ErrorMessage = "Имя необходимо")]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Фамилия необходима")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Неверный формат Email")]
        [Required(ErrorMessage = "Email необходим")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"(^8|7|\+7)((\d{10})|(\s\(\d{3}\)\s\d{3}\s\d{2}\s\d{2}))", ErrorMessage = "Неверный формат телефона")]
        [Required(ErrorMessage = "Телефон необходим")]
        [Display(Name = "Телефон")]
        public string Phone { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Дата рождения необходима")]
        [Display(Name = "Дата рождения")]
        public DateOnly BirthDate { get; set; }

        [Required(ErrorMessage = "Пароль необходим")]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
    }
}
