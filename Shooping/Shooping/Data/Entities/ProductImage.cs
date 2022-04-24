using System.ComponentModel.DataAnnotations;

namespace Shooping.Data.Entities
{
    public class ProductImage
    {
        public int Id { get; set; }

        public Product Product { get; set; }

        [Display(Name = "Foto")]
        public Guid ImageId { get; set; }

        [Display(Name = "Foto")]
        public string ImageFullPath => ImageId == Guid.Empty
            ? $"https://shoppingzulu.azurewebsites.net/images/noimage.png"
            : $"https://shoppingprep.blob.core.windows.net/products/{ImageId}";
    }
}
