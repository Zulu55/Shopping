using Microsoft.AspNetCore.Mvc.Rendering;
using Shooping.Data;

namespace Shooping.Helpers
{
    public class CombosHelper : ICombosHelper
    {
        private readonly DataContext _context;

        public CombosHelper(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<SelectListItem> GetComboCategories()
        {
            List<SelectListItem> list = _context.Categories.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = $"{x.Id}"
            })
                .OrderBy(x => x.Text)
                .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "[Seleccione una categoría...]",
                Value = "0"
            });

            return list;
        }
    }
}
