using DATN_SalesWebsite.Models;
using DATN_SalesWebsite.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATN_SalesWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "ADMIN")]
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;

        public BrandController(DataContext context)
        {
            _dataContext = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Brands.OrderByDescending(b => b.Id).ToListAsync());
        }

        //public async Task<IActionResult> Index(int pg = 1)
        //{
        //    List<BrandModel> Brands = _dataContext.Brands.ToList();


        //    const int pageSize = 10;

        //    if (pg < 1)
        //    {
        //        pg = 1;
        //    }
        //    int recsCount = Brands.Count();

        //    var pager = new Paginate(recsCount, pg, pageSize);

        //    int recSkip = (pg - 1) * pageSize;

        //    //category.Skip(20).Take(10).ToList()

        //    var data = Brands.Skip(recSkip).Take(pager.PageSize).ToList();

        //    ViewBag.Pager = pager;

        //    return View(data);
        //}

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandModel brand)
        {
            if (ModelState.IsValid)
            {
                brand.Slug = brand.Name.Replace(" ", "-");
                var slug = await _dataContext.Brands.FirstOrDefaultAsync(c => c.Slug == brand.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Thương hiệu đã tồn tại trong DataBase");
                    return View(brand);
                }

                _dataContext.Add(brand);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm Thương hiệu thành công";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Model đang bị lỗi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMassage = string.Join("\n", errors);
                return BadRequest(errorMassage);
            }

            return View(brand);
        }

        public async Task<IActionResult> Edit(int Id)
        {
            BrandModel brand = await _dataContext.Brands.FindAsync(Id);
            return View(brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BrandModel brand)
        {
            if (ModelState.IsValid)
            {
                brand.Slug = brand.Name.Replace(" ", "-");


                _dataContext.Update(brand);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Cập nhập Thương hiệu thành công";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Model đang bị lỗi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMassage = string.Join("\n", errors);
                return BadRequest(errorMassage);
            }

            return View(brand);
        }

        public async Task<IActionResult> Delete(int Id)
        {
            BrandModel brand = await _dataContext.Brands.FindAsync(Id);

            if (brand == null)
            {
                return NotFound();
            }

            _dataContext.Brands.Remove(brand);
            await _dataContext.SaveChangesAsync();
            TempData["error"] = "Đã xóa Thương hiệu";
            return RedirectToAction("Index");
        }

    }
}
