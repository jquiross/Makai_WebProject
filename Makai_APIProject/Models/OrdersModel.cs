namespace Makai_APIProject.Models
{
    public class OrdersModel
    {
        public int order_id { get; set; }
        public int user_id { get; set; }
        public double total_price { get; set; }
        public bool status {  get; set; }
        public DateTime created_at { get; set; }
    }
}
