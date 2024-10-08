using DATN_SalesWebsite.Models;
using DATN_SalesWebsite.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using System.Threading.RateLimiting;

namespace DATN_SalesWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="ADMIN")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly DataContext _dataContext;

        public UserController(DataContext context, RoleManager<IdentityRole> roleManager, UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager, DataContext dataContext)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _dataContext = dataContext;

        }
        public async Task<IActionResult> Index()
        {
            var userWithRoles = await (from u in _dataContext.Users
                                       join ur in _dataContext.UserRoles on u.Id equals ur.UserId
                                       join r in _dataContext.Roles on ur.RoleId equals r.Id
                                       select new { User = u, RoleName = r.Name }).ToListAsync();
            return View(userWithRoles);
        }

        //public async Task<IActionResult> Index(int pg = 1)
        //{
        //    var userWithRoles = await (from u in _dataContext.Users
        //                              join ur in _dataContext.UserRoles on u.Id equals ur.UserId
        //                              join r in _dataContext.Roles on ur.RoleId equals r.Id
        //                              select new {User = u, RoleName = r.Name}).ToListAsync();

        //    const int pageSize = 10;

        //    if (pg < 1)
        //    {
        //        pg = 1;
        //    }
        //    int recsCount = userWithRoles.Count();

        //    var pager = new Paginate(recsCount, pg, pageSize);

        //    int recSkip = (pg - 1) * pageSize;

        //    //category.Skip(20).Take(10).ToList()

        //    var data = userWithRoles.Skip(recSkip).Take(pager.PageSize).ToList();

        //    ViewBag.Pager = pager;

        //    return View(data);
        //}
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(new AppUserModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppUserModel user)
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            if (ModelState.IsValid)
            {
                var createUserResult = await _userManager.CreateAsync(user, user.PasswordHash);
                if (createUserResult.Succeeded)
                {
                    var role = _roleManager.FindByIdAsync(user.RoleId);
                    var addToRoleResult = await _userManager.AddToRoleAsync(user, role.Result.Name);
                    if(!addToRoleResult.Succeeded)
                    {
                        foreach (var error in createUserResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    foreach (var error in createUserResult.Errors)
                    {
                        AddIdentityErrors(createUserResult);
                    }
                    return View(user);
                }
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
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                return NotFound();
            }
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(string Id, AppUserModel user)
        {
            var existingUser = await _userManager.FindByIdAsync(Id);
            if (existingUser == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                existingUser.UserName = user.UserName;
                existingUser.Email = user.Email;
                existingUser.RoleId = user.RoleId;

                var updateUserResult = await _userManager.UpdateAsync(existingUser);
                if (updateUserResult.Succeeded)
                {
                    // Tìm vai trò hiện tại của người dùng
                    var currentRoles = await _userManager.GetRolesAsync(existingUser);
                    if (currentRoles.Count > 0)
                    {
                        // Xóa tất cả các vai trò hiện tại trước khi thêm vai trò mới
                        var removeFromRolesResult = await _userManager.RemoveFromRolesAsync(existingUser, currentRoles);
                        if (!removeFromRolesResult.Succeeded)
                        {
                            foreach (var error in removeFromRolesResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            return View(existingUser);
                        }
                    }

                    // Tìm vai trò mới dựa trên RoleId
                    var role = await _roleManager.FindByIdAsync(existingUser.RoleId);
                    if (role != null)
                    {
                        // Thêm vai trò mới
                        var addToRoleResult = await _userManager.AddToRoleAsync(existingUser, role.Name);
                        if (!addToRoleResult.Succeeded)
                        {
                            foreach (var error in addToRoleResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            return View(existingUser);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Role not found.");
                        return View(existingUser);
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    AddIdentityErrors(updateUserResult);
                    return View(existingUser);
                }
            }

            // Lấy danh sách vai trò để hiển thị trong dropdown
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");

            // Xử lý lỗi
            TempData["error"] = "Model lỗi";
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage).ToList());
            string errorMassage = string.Join("\n", errors);
            return View(existingUser);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                return NotFound();
            }
            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                return View("Error");
            }
            if (User.Identity.Name == user.UserName)
            {
                await _signInManager.SignOutAsync(); // Đăng xuất người dùng
            }
            TempData["success"] = "Xóa user thành công";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            await _signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }
        private void AddIdentityErrors(IdentityResult identityResult)
        {
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
