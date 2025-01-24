using BizKeeper360.Models.Enums;
using BizKeeper360.Resources;

namespace BizKeeper360.Servises
{
    public class CategoryLocalizationService
    {
        private readonly SharedLocalizationService _localizer;

        public CategoryLocalizationService(SharedLocalizationService localizer)
        {
            _localizer = localizer;
        }

        public IDictionary<Category, string> GetLocalizedCategories()
        {
            return Enum.GetValues(typeof(Category))
                .Cast<Category>()
                .ToDictionary(
                    category => category,
                    category => _localizer.Categories[category.ToString()].Value);
        }
    }
}
