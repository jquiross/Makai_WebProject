using System;

namespace Makai_WebProject.Models
{
    public class ProductInventoryViewModel
    {
        public int inventory_id { get; set; }
        public int product_id { get; set; }
        public int stock { get; set; }
        public DateTime? last_updated { get; set; }
    }
}
