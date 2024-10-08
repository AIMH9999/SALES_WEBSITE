using DATN_SalesWebsite.Models;
using DATN_SalesWebsite.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DATN_SalesWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "ADMIN")]
    public class SliderController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public SliderController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.sliders.OrderByDescending(b => b.Id).ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderModel slider)
        {
            if (ModelState.IsValid)
            {
                if (slider.ImageUpload != null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/sliders");
                    string imageName = Guid.NewGuid().ToString() + "_" + slider.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imageName);
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await slider.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    slider.Image = imageName;
                }
                _dataContext.Add(slider);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm sản phẩm thành công";
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
        }

        public async Task<IActionResult> Edit(int Id)
        {
            SliderModel slider = await _dataContext.sliders.FindAsync(Id);
            return View(slider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id, SliderModel slider)
        {
            var existed_slider = _dataContext.sliders.Find(slider.Id);

            if (ModelState.IsValid)
            {
                if (slider.ImageUpload != null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/sliders");
                    string imageName = Guid.NewGuid().ToString() + "_" + slider.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imageName);

                    string oldfileImg = Path.Combine(uploadDir, existed_slider.Image);
                    try
                    {
                        if (System.IO.File.Exists(oldfileImg))
                        {
                            System.IO.File.Delete(oldfileImg);
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "An error occurred while deleting the slider image.");
                    }

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await slider.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    existed_slider.Image = imageName;


                }

                existed_slider.Name = slider.Name;
                existed_slider.Description = slider.Description;
                existed_slider.Status = slider.Status;


                _dataContext.Update(existed_slider);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Cập nhập slider thành công";
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
        }

        public async Task<IActionResult> Delete(int Id)
        {
            SliderModel slider = await _dataContext.sliders.FindAsync(Id);

            if (slider == null)
            {
                return NotFound();
            }

            _dataContext.sliders.Remove(slider);
            await _dataContext.SaveChangesAsync();
            TempData["error"] = "Đã xóa Slider";
            return RedirectToAction("Index");
        }
    }
}
