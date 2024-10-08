using DATN_SalesWebsite.Models;
using DATN_SalesWebsite.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DATN_SalesWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, DataContext context)
        {
            _logger = logger;
            _dataContext = context;
        }

        //public IActionResult Index()
        //{
        //    var products = _dataContext.Products.Include("Category").Include("Brand").ToList();
        //    return View(products);
        //}

        public IActionResult Index(int pg = 1)
        {
            var products = _dataContext.Products.Include("Category").Include("Brand").ToList();


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
            ViewBag.Sliders = sliders;
            return View(data);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statusCode)
        {
           if(statusCode == 404)
            {
                return View("NotFound");
            }
           else
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
            
        }
    }
}
