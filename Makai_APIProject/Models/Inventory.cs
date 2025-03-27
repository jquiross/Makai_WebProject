namespace Makai_APIProject.Models
{
    public class Inventory
    {
        public int inventory_id { get; set; }
        public int product_id { get; set; }
        public int stock { get; set; }
        public DateTime? last_updated { get; set; }
    }
}
