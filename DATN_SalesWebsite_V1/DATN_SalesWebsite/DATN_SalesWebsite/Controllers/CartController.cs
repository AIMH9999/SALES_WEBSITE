using DATN_SalesWebsite.Models;
using DATN_SalesWebsite.Models.ViewModels;
using DATN_SalesWebsite.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATN_SalesWebsite.Controllers
{
    public class CartController : Controller
    {
        private readonly DataContext _dataContext;

        public CartController(DataContext _context) 
        { 
        _dataContext = _context;
        }
        //public IActionResult Index()
        //{
        //    List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
        //    CartItemViewModel cartVM = new()
        //    {
        //        CartItems = cartItems,
        //        GrandTotal = cartItems.Sum(x => x.Quantity*x.Price),

        //    };
        //    return View(cartVM);
        //}

        public IActionResult Index(string Slug = "", int pg = 1)
        {
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemViewModel cartVM = new()
            {
                CartItems = cartItems,
                GrandTotal = cartItems.Sum(x => x.Quantity * x.Price),

            };

            const int pageSize = 15;

            if (pg < 1)
            {
                pg = 1;
            }
            int recsCount = cartVM.CartItems.Count();

            var pager = new Paginate(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            //category.Skip(20).Take(10).ToList()

            cartVM.CartItems = cartVM.CartItems.Skip(recSkip).Take(pager.PageSize).ToList();


            ViewBag.Pager = pager;


            return View(cartVM);
        }

        public IActionResult Checkout()
        {
            return View("~/Checkout/Index.csthml");
        }

        public async Task<IActionResult> Add(int Id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(Id);
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItems = cart.Where(c => c.ProductId == Id).FirstOrDefault();
            if(cartItems == null)
            {
                cart.Add(new CartItemModel(product));
            }
            else
            {
                cartItems.Quantity += 1;

            }
            HttpContext.Session.SetJson("Cart", cart);
            TempData["success"] = "Add to cart Successfully";

            return Redirect(Request.Headers["Referer"].ToString());
        }

        public async Task<IActionResult> Decrease(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();
            if(cartItem.Quantity > 0) 
            {
                --cartItem.Quantity;
            }
            if (cartItem.Quantity == 0)
            {
                cart.RemoveAll(p => p.ProductId == Id);
            }
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Increase(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();
            if (cartItem.Quantity > 0)
            {
                ++cartItem.Quantity;
            }
            if (cartItem.Quantity == 0)
            {
                cart.RemoveAll(p => p.ProductId == Id);
            }
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            cart.RemoveAll(p => p.ProductId == Id);
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }
            TempData["success"] = "Remove Item Successfully";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Clear(int Id)
        {
            HttpContext.Session.Remove("Cart");
            TempData["success"] = "Clear cart Successfully";
            return RedirectToAction("Index");
        }
    }
}
