using Shooping.Data.Entities;

namespace Shooping.Models
{
    public class HomeViewModel
    {
        public ICollection<Product> Products { get; set; }

        public float Quantity { get; set; }
    }
}
