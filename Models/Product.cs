using codesphere_api.Common.Models;

namespace codesphere_api.Models
{
    public class Product : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
        public int Stock { get; private set; }
        public bool IsActive { get; private set; } = true;
    }

}
