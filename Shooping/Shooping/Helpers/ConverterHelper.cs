using Shooping.Data;
using Shooping.Data.Entities;
using Shooping.Models;

namespace Shooping.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;

        public ConverterHelper(DataContext context, ICombosHelper combosHelper)
        {
            _context = context;
            _combosHelper = combosHelper;
        }

        public async Task<Product> ToProductAsync(ProductViewModel model, Guid imageId, bool isNew)
        {
            return new Product
            {
                Id =  isNew ? 0 : model.Id,
                Description = model.Description,
                Name = model.Name,
                Price = model.Price,
                ProductCategories = new List<ProductCategory>() { new ProductCategory { Category = await _context.Categories.FindAsync(model.CategoryId) } },
                ProductImages = imageId != Guid.Empty ? new List<ProductImage>() { new ProductImage { ImageId = imageId } } : new List<ProductImage>(),
                Stock = model.Stock,
            };
        }

        public ProductViewModel ToProductViewModel(Product product)
        {
            return new ProductViewModel
            {
                Categories = _combosHelper.GetComboCategories(),
                CategoryId = product.ProductCategories.FirstOrDefault().Id,
                Description = product.Description,
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock,
            };
        }
    }
}
