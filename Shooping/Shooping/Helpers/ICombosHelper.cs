using Microsoft.AspNetCore.Mvc.Rendering;

namespace Shooping.Helpers
{
    public interface ICombosHelper
    {
        IEnumerable<SelectListItem> GetComboCategories();
    }
}
