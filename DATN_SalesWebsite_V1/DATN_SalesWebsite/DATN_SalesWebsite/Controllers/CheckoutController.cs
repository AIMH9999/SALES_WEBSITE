using DATN_SalesWebsite.Areas.Admin.Repository;
using DATN_SalesWebsite.Models;
using DATN_SalesWebsite.Models.ViewModels;
using DATN_SalesWebsite.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DATN_SalesWebsite.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IEmailSender _emailSender;
        public CheckoutController(DataContext context, IEmailSender emailSender)
        {
            _dataContext = context;
            _emailSender = emailSender;
        }
        public async Task<IActionResult> Checkout()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            // Nếu người dùng chưa đăng nhập, chuyển hướng đến trang đăng nhập
            if (userName == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var orderCode = Guid.NewGuid().ToString();

                // Tạo đối tượng OrderModel
                var orderItem = new OrderModel
                {
                    OrderCode = orderCode,
                    UserName = userName,
                    UserEmail = userEmail,
                    Status = 1,
                    CreatedDate = DateTime.Now
                };
                _dataContext.Add(orderItem);
                _dataContext.SaveChanges();

                TempData["success"] = "Tạo đơn hàng thành công";

                // Lấy giỏ hàng từ session
                List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

                // Chuỗi để chứa chi tiết đơn hàng cho email
                var orderDetailsMessage = "Tạo đơn hàng thành công, vui lòng chờ xác nhận";
                orderDetailsMessage += $"Mã đơn hàng: {orderCode}\n\nDanh sách sản phẩm:\n";

                // Duyệt qua từng sản phẩm trong giỏ hàng và tạo chi tiết đơn hàng
                foreach (var item in cartItems)
                {
                    var orderDetails = new OrderDetails
                    {
                        UserName = userName,
                        UserEmail = userEmail,
                        OrderCode = orderCode,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.Price
                    };
                    _dataContext.Add(orderDetails);
                    _dataContext.SaveChanges();

                    // Lấy thông tin chi tiết sản phẩm từ ProductModel
                    var product = _dataContext.Products.FirstOrDefault(p => p.Id == item.ProductId);

                    // Thêm chi tiết sản phẩm vào message (bao gồm tên sản phẩm, số lượng, giá)
                    orderDetailsMessage += $"- Sản phẩm: {product?.Name ?? "Không rõ"}, Số lượng: {item.Quantity}, Giá: {item.Price * item.Quantity} VND\n";
                }

                HttpContext.Session.Remove("Cart");

                // Hoàn thiện message
                orderDetailsMessage += $"\nTổng cộng: {cartItems.Sum(x => x.Price * x.Quantity)} VND\n";

                // Gửi email
                var receiver = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var subject = "Tạo đơn hàng thành công";
                //await _emailSender.SendEmailAsync(receiver, subject, orderDetailsMessage);

                TempData["success"] = "Tạo đơn hàng thành công, vui lòng chờ xác nhận";
                return RedirectToAction("Index", "Cart");
            }

            return View();
        }

    }
}
