using DATN_SalesWebsite.Migrations;
using DATN_SalesWebsite.Models;
using DATN_SalesWebsite.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATN_SalesWebsite.Controllers
{
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;
        public BrandController(DataContext context)
        {
            _dataContext = context;
        }

        //public async Task<IActionResult> Index(string Slug = "")
        //{
        //    BrandModel brand = _dataContext.Brands.Where(b => b.Slug == Slug).FirstOrDefault();
        //    if (brand == null) return RedirectToAction("Index");
        //    var productsByBrand = _dataContext.Products.Where(pbb => pbb.BrandId == brand.Id);
        //    return View(await productsByBrand.OrderByDescending(pbc => pbc.Id).ToListAsync());
        //}

        public async Task<IActionResult> Index(string Slug = "", int pg = 1)
        {
            BrandModel brand = _dataContext.Brands.Where(b => b.Slug == Slug).FirstOrDefault();
            if (brand == null) return RedirectToAction("Index");
            var productsByBrand = _dataContext.Products.Where(pbb => pbb.BrandId == brand.Id);
            var products = await productsByBrand.OrderByDescending(pbc => pbc.Id).ToListAsync();

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
            ViewBag.Pager = pager;
            ViewBag.Slug = Slug;
            ViewBag.Sliders = sliders;
            return View(data);
        }
    }
}
