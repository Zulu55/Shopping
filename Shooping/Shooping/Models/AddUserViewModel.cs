using Shooping.Enums;
using System.ComponentModel.DataAnnotations;

namespace Shooping.Models
{
    public class AddUserViewModel : EditUserViewModel
    {
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Debes ingresar un correo válido.")]
        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "El campo {0} debe tener entre {2} y {1} carácteres.")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "La contraseña y la confirmación no son iguales.")]
        [Display(Name = "Confirmación de contraseña")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "El campo {0} debe tener entre {2} y {1} carácteres.")]
        public string PasswordConfirm { get; set; }

        [Display(Name = "Tipo de usuario")]
        public UserType UserType { get; set; }
    }
}
