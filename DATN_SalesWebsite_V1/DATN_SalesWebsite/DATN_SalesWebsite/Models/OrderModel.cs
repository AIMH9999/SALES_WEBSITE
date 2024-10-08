namespace DATN_SalesWebsite.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Status { get; set; }
    }
}
