using System.ComponentModel.DataAnnotations;

namespace KursavayaECS.Models
{
    public class StudentApproveModel
    {
        [Required(ErrorMessage = "Предпочитаемая специальность необходима")]
        [Display(Name = "Предпочитаемая специальность")]
        public required string PrefSpec { get; set; }
    }
}
