using DATN_SalesWebsite.Migrations;
using DATN_SalesWebsite.Models;
using DATN_SalesWebsite.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace DATN_SalesWebsite.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;
        public CategoryController(DataContext context) 
        { 
            _dataContext = context;
        }
        //public async Task<IActionResult> Index(string Slug = "")
        //{
        //    CategoryModel category = _dataContext.Categories.Where(c => c.Slug == Slug).FirstOrDefault();
        //    if (category == null) return RedirectToAction("Index");
        //    var productsByCategory = _dataContext.Products.Where(pbc => pbc.CategoryId == category.Id);
        //    return View(await productsByCategory.OrderByDescending(pbc => pbc.Id).ToListAsync());
        //}

        public async Task<IActionResult> Index(string Slug = "", int pg = 1)
        {
            CategoryModel category = _dataContext.Categories.Where(c => c.Slug == Slug).FirstOrDefault();
            if (category == null) return RedirectToAction("Index");
            var productsByCategory = _dataContext.Products.Where(pbc => pbc.CategoryId == category.Id);
            var products = await productsByCategory.OrderByDescending(pbc => pbc.Id).ToListAsync();

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
