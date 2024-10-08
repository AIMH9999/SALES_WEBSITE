using System.ComponentModel.DataAnnotations;

namespace DATN_SalesWebsite.Models.ViewModels
{
    public class ProductDetailsViewModel
    {
        public ProductModel ProductDetail { get; set; }
        [Required, MinLength(4, ErrorMessage = "Yêu cầu nhập đánh giá sản phẩm")]
        public string Comment { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập tên")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập Email")]
        public string Email { get; set; }

        public int Star { get; set; }
    }
}
