using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DATN_SalesWebsite.Models
{
    public class RatingModel
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        [Required, MinLength(4, ErrorMessage = "Yêu cầu nhập đánh giá sản phẩm")]
        public string Comment { get; set; }
        [Required(ErrorMessage ="Yêu cầu nhập tên")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập Email")]
        public string Email { get; set; }

        public int Star { get; set; }

        public DateTime CreatedDate { get; set; }

        [ForeignKey("ProductId")]
        public ProductModel Product { get; set; }
    }
}
