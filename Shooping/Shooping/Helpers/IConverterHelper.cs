using Shooping.Data.Entities;
using Shooping.Models;

namespace Shooping.Helpers
{
    public interface IConverterHelper
    {
        Task<Product> ToProductAsync(ProductViewModel model, Guid imageId, bool isNew);

        ProductViewModel ToProductViewModel(Product product);
    }
}
