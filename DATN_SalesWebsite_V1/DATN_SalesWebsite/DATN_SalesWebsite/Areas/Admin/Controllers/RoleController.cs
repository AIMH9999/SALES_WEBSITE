using DATN_SalesWebsite.Models;
using DATN_SalesWebsite.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATN_SalesWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "ADMIN")]
    public class RoleController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleController(DataContext dataContext, RoleManager<IdentityRole> roleManager)
        {
            _dataContext = dataContext;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Roles.OrderByDescending(r => r.Id).ToListAsync());
        }

        //public async Task<IActionResult> Index(int pg = 1)
        //{
        //    List<IdentityRole> Roles = await _dataContext.Roles.OrderByDescending(r => r.Id).ToListAsync();


        //    const int pageSize = 10;

        //    if (pg < 1)
        //    {
        //        pg = 1;
        //    }
        //    int recsCount = Roles.Count();

        //    var pager = new Paginate(recsCount, pg, pageSize);

        //    int recSkip = (pg - 1) * pageSize;

        //    //category.Skip(20).Take(10).ToList()

        //    var data = Roles.Skip(recSkip).Take(pager.PageSize).ToList();

        //    ViewBag.Pager = pager;

        //    return View(data);
        //}

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IdentityRole role)
        {
            if (!_roleManager.RoleExistsAsync(role.Name).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(role.Name)).GetAwaiter().GetResult();
            }
            return Redirect("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Edit(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(Id);

            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string Id, IdentityRole model)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }
            if(ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(Id);
                if (role == null)
                {
                    return NotFound();
                }
                role.Name = model.Name;
                try
                {
                    await _roleManager.UpdateAsync(role);
                    TempData["success"] = "Edit thành công";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "có lỗi khi xóa role");
                }
            }
            return View(model ?? new IdentityRole { Id = Id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(Id);

            if (role == null)
            {
                return NotFound();
            }

            try
            {
                await _roleManager.DeleteAsync(role);
                TempData["success"] = "Xóa thành công";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "có lỗi khi xóa role");
            }
            return RedirectToAction("Index");
        }
    }
}
