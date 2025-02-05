using Humanizer.Localisation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TemplatePustokApp.Data;
using TemplatePustokApp.Helpers;
using TemplatePustokApp.Models;

namespace TemplatePustokApp.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize]
    public class SliderController : Controller
    {
        private readonly PustokAppDbContext _context;
        private readonly JwtServiceOption _jwtServiceOption;
        private readonly IWebHostEnvironment _env;
        #region 1
        //private readonly IConfiguration _configuration;appsettingsdekin oxumaga gore
        #endregion
        public SliderController(PustokAppDbContext context, IOptions<JwtServiceOption> option, IWebHostEnvironment env)
        {
            _context = context;
            _jwtServiceOption = option.Value;
            _env = env;
        }

        public IActionResult Index()
        {
            #region pagination qosmaq ucun
            //var query = _context.Sliders.AsQueryable();
            //return View(PaginatedList<Slider>.Create(query,take,page));
            #endregion
            return View(_context.Sliders.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Slider slider)
        {
            if (slider.Photo == null)
            {
                ModelState.AddModelError("Photo", "Photo is required");
            }
            if (!ModelState.IsValid)
            {
                return View();
            }
            var file = slider.Photo;
            #region dataannotaion ile yazdigimiz 
            //file.ContentType != "image/jpeg" && file.ContentType != "image/png"file.ContentType != "image/jpeg" && file.ContentType != "image/png"
            //if (!file.CheckType(new string[] { "image/jpeg", "image/png" }))
            //{
            //    ModelState.AddModelError("Photo", "File type is wrong");
            //    return View();
            //}
            //if (file.Length > 2 * 1024 * 1024)
            //{
            //    ModelState.AddModelError("Photo", "File legnth must be less than 2 MB");
            //    return View();
            //}
            #endregion
            if (_context.Sliders.Any(s => s.Title.Trim().ToLower() == slider.Title.Trim().ToLower()))
            {
                ModelState.AddModelError("Title", "This slider already exist");//xeta yalniz backendden qayidir
                return View();
            }
            #region yalniz oz kompda isleyecek
            //string path = "C:\\Users\\Asus\\source\\repos\\TemplatePustokApp\\TemplatePustokApp\\wwwroot\\assets\\image\\bg-images\\"+file.FileName;
            #endregion
            slider.Image = file.SaveImage(_env.WebRootPath, "assets/image/bg-images");
            slider.CreatedDate = DateTime.Now;
            _context.Sliders.Add(slider);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Edit(int? id)
        {
            if (id is null)
            {
                return BadRequest();
            }
            var slider = _context.Sliders.Find(id);
            if (slider == null)
            {
                return BadRequest();
            }
            return View(slider);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Slider slider)
        {
            if (!ModelState.IsValid)
            {
                return View(slider);
            }
            var existSlider = _context.Sliders.Find(slider.Id);
            if (existSlider == null)
            {
                return BadRequest();
            }
            var file = slider.Photo;
            string oldImage = existSlider.Image;
            if (file != null)
            {
                existSlider.Image = file.SaveImage(_env.WebRootPath, "assets/image/bg-images");
                var deletedImagePath = Path.Combine(_env.WebRootPath, "assets/image/bg-images", oldImage);
                if (!FileManager.DeleteFile(deletedImagePath))
                {
                    return BadRequest();
                }
            }
            existSlider.Title = slider.Title;
            existSlider.Description = slider.Description;
            existSlider.CreatedDate = DateTime.Now;
            existSlider.Order = slider.Order;
            existSlider.ButtonLink = slider.ButtonLink;
            existSlider.ButtonText = slider.ButtonText;
            existSlider.UpdatedDate = DateTime.Now;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int? id)
        {
            var existSlider = _context.Sliders.Find(id);
            if (existSlider is null)
            {
                return BadRequest();
            }
            var deletedImagePath = Path.Combine(_env.WebRootPath, "assets/image/bg-images", existSlider.Image);
            if (!FileManager.DeleteFile(deletedImagePath))
            {
                return BadRequest();
            }
            _context.Sliders.Remove(existSlider);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        #region ReadData configuration istifade etmekle 1
        //public IActionResult ReadData()
        //{
        //    var data1 = _configuration.GetSection("Jwt:Key").Value;
        //    var data2 = _configuration.GetSection("Jwt:Issuer").Value;
        //    return Json(new
        //    {
        //        key = data1,
        //        issuer = data2,
        //    });
        //}
        #endregion
        public IActionResult ReadData()
        {
            var data1 = _jwtServiceOption.Key;
            var data2 = _jwtServiceOption.Issuer;
            return Json(new
            {
                key = data1,
                issuer = data2,
            });
        }
    }
}
