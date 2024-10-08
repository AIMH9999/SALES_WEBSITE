using DATN_SalesWebsite.Models;
using DATN_SalesWebsite.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATN_SalesWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "ADMIN")]
    public class OrderController : Controller
    {
        private readonly DataContext _dataContext;
        public OrderController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Orders.OrderByDescending(o => o.Id).ToListAsync());
        }

        //public async Task<IActionResult> Index(int pg = 1)
        //{
        //    List<OrderModel> Orders = await _dataContext.Orders.OrderByDescending(o => o.Id).ToListAsync();


        //    const int pageSize = 10;

        //    if (pg < 1)
        //    {
        //        pg = 1;
        //    }
        //    int recsCount = Orders.Count();

        //    var pager = new Paginate(recsCount, pg, pageSize);

        //    int recSkip = (pg - 1) * pageSize;

        //    //category.Skip(20).Take(10).ToList()

        //    var data = Orders.Skip(recSkip).Take(pager.PageSize).ToList();

        //    ViewBag.Pager = pager;

        //    return View(data);
        //}

        public async Task<IActionResult> ViewOrder(string ordercode)
        {

            var OrderDetails = await _dataContext.OrderDetails.Include(od => od.Product).Where(od => od.OrderCode == ordercode).ToListAsync();
            ViewBag.Status = await _dataContext.Orders.Where(od => od.OrderCode == ordercode).Select(od => od.Status).FirstOrDefaultAsync();
            return View(OrderDetails);
        }

        //public async Task<IActionResult> ViewOrder(string ordercode, int pg = 1)
        //{

        //    var OrderDetails = await _dataContext.OrderDetails.Include(od => od.Product).Where(od => od.OrderCode == ordercode).ToListAsync();

        //    const int pageSize = 10;

        //    if (pg < 1)
        //    {
        //        pg = 1;
        //    }
        //    int recsCount = OrderDetails.Count();

        //    var pager = new Paginate(recsCount, pg, pageSize);

        //    int recSkip = (pg - 1) * pageSize;

        //    //category.Skip(20).Take(10).ToList()

        //    var data = OrderDetails.Skip(recSkip).Take(pager.PageSize).ToList();

        //    ViewBag.Pager = pager;

        //    return View(data);
        //}

        [HttpPost]
        public async Task<IActionResult> UpdateOrder(string ordercode, int status)
        {
            var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == ordercode);

            if (order == null)
            {
                return NotFound();
            }
            order.Status = status;

            try
            {
                await _dataContext.SaveChangesAsync();
                return Ok(new { success = true, message = "Cập nhập trạng thái đơn hàng thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "có lỗi trong khi cập nhập trạng thái đơn hàng");
            }
        }
    }
}
