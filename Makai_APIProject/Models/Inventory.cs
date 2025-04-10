namespace Makai_APIProject.Models
{
    public class Inventory


    {

        public int Product_id { get; set; }
        public int Category_id { get; set; }
        public string? Description { get; set; }

        public string? Name { get; set; }

        public decimal Price { get; set; }

        public int Stock_quantity { get; set; }

        public string? Image_url   { get; set; }

        public DateTime Created_at { get; set; }

        public string Category_name { get; set; }

    }
}
