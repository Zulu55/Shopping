using System.ComponentModel.DataAnnotations;

namespace Shooping.Models
{
    public class ResendTokenViewModel
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debes ingresar un correo válido.")]
        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
