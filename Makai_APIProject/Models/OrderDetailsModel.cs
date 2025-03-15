namespace Makai_APIProject.Models
{
    public class OrderDetailsModel
    {
        public int order_detail_id { get; set; }
        public int order_id { get; set; }
        public int product_id { get; set; }
        public int quantity { get; set; }
        public double price { get; set; }
    }
}
