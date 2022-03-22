using Shooping.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Shooping.Models
{
    public class ShowCartViewModel
    {
        public User User { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Comentarios")]
        public string Remarks { get; set; }

        public ICollection<TemporalSale> TemporalSales { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Cantidad")]
        public float Quantity => TemporalSales == null ? 0 : TemporalSales.Sum(ts => ts.Quantity);

        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Valor")]
        public decimal Value => TemporalSales == null ? 0 : TemporalSales.Sum(ts => ts.Value);
    }
}
