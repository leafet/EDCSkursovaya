using System.ComponentModel.DataAnnotations;

namespace KursavayaECS.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email необходим")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль необходим")]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
    }
}
