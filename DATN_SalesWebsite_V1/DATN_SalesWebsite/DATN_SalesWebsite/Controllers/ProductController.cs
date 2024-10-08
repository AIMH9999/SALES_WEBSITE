using DATN_SalesWebsite.Models;
using DATN_SalesWebsite.Models.ViewModels;
using DATN_SalesWebsite.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATN_SalesWebsite.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        public ProductController(DataContext context)
        {
            _dataContext = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(string searchTerm, int pg)
        {
            var products = await _dataContext.Products.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm) || p.Brand.Name.Contains(searchTerm) || p.Category.Name.Contains(searchTerm)).ToListAsync();
            const int pageSize = 15;

            if (pg < 1)
            {
                pg = 1;
            }
            int recsCount = products.Count();

            var pager = new Paginate(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            //category.Skip(20).Take(10).ToList()

            var data = products.Skip(recSkip).Take(pager.PageSize).ToList();
            var sliders = _dataContext.sliders.Where(s => s.Status == 1).ToList();
            ViewBag.Sliders = sliders;
            ViewBag.Pager = pager;
            ViewBag.Keyword = searchTerm;
            return View(data);
        }

        public async Task<IActionResult> Details(int Id)
        {
            if (Id == null) return RedirectToAction("Index");
            //var productsById = _dataContext.Products.Include(p => p.Rating).Where(p => p.Id == Id).Include("Category").Include("Brand").FirstOrDefault();
            var productsById = _dataContext.Products.Include("Category").Include("Brand").Where(pbi => pbi.Id == Id).FirstOrDefault();
            var viewModel = new ProductDetailsViewModel
            {
                ProductDetail = productsById,
            };
            //var productsByBrand = _dataContext.Products.Where(p => p.Brand == productsById.Brand).ToList();
            //ViewBag.ProductsByBrand = productsByBrand;

            var relatedProducts = await _dataContext.Products.Where(p => p.CategoryId == productsById.CategoryId && p.Id != productsById.Id).Take(10).ToListAsync();
            ViewBag.relatedProducts = relatedProducts;
            ViewBag.Review = await _dataContext.Ratings.Where(r => r.ProductId == productsById.Id).ToListAsync();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductComment(RatingModel rating)
        {
            if (ModelState.IsValid)
            {
                var productById = await _dataContext.Products
            .Include("Category")
            .Include("Brand")
            .Where(pbi => pbi.Id == rating.ProductId)
            .FirstOrDefaultAsync();

                if (productById == null)
                {
                    TempData["error"] = "Sản phẩm không tồn tại.";
                    return RedirectToAction("Details", new { id = rating.ProductId });
                }
                var ratingEntity = new RatingModel
                {
                    ProductId = rating.ProductId,
                    Name = rating.Name,
                    Email = rating.Email,
                    Comment = rating.Comment,
                    Star = rating.Star,
                    Product = productById,
                    CreatedDate = DateTime.Now
                };
                _dataContext.Ratings.Add(ratingEntity);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm đánh giá thành công";
                return Redirect(Request.Headers["Referer"]);
            }
            else
            {
                TempData["error"] = "Không thêm được đánh giá";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n ", errors);
                return RedirectToAction("Details", new { id = rating.ProductId });
            }
            return Redirect(Request.Headers["Referer"]);
        }
    }
}
