using DATN_SalesWebsite.Repository.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DATN_SalesWebsite.Models
{
    public class SliderModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập Tên Slider")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập Mô tả Slider")]
        public string Description { get; set; }
        public int? Status { get; set; }

        public string Image { get; set; } = "noimage.jpg";

        [NotMapped]
        [FileExtension]

        public IFormFile? ImageUpload { get; set; }
    }
}
