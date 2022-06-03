using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Shooping.Data.Entities
{
    public class State
    {
        public int Id { get; set; }

        [Display(Name = "Departamento / Estado")]
        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Name { get; set; }

        [JsonIgnore]
        public Country Country { get; set; }

        public ICollection<City> Cities { get; set; }

        [Display(Name = "Ciudades")]
        public int CitiesNumber => Cities == null ? 0 : Cities.Count;
    }
}
