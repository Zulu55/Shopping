using Shooping.Common;
using Shooping.Data.Entities;

namespace Shooping.Models
{
    public class HomeViewModel
    {
        public PaginatedList<Product> Products { get; set; }

        public float Quantity { get; set; }

        public IEnumerable<Category> Categories { get; set; }
    }
}
