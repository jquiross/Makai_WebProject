namespace Makai_APIProject.Models
{
    public class CartItemModel
    {
        public int user_id { get; set; }
        public int product_id { get; set; }
        public int quantity { get; set; }
        public DateTime added_at { get; set; }
    }
}
