using FileUpload.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication24.Models;

namespace WebApplication24.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=Images;Integrated Security=true;";
        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult upload(IFormFile image, string password)
        {
            string fileName = $"{Guid.NewGuid()}-{image.FileName}";
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
            using var fs = new FileStream(filePath, FileMode.CreateNew);
            image.CopyTo(fs);
            Image i = new Image
            {
                Path = fileName,
                Password = password
            };
            ImageRepository ir = new(_connectionString);
            i.Id = ir.AddImage(i);
            return View(new ViewModel { Image = i });
        }
        public IActionResult getPassword(int id)
        {
            ImageRepository ir = new(_connectionString);

            List<int> Ids = HttpContext.Session.Get<List<int>>("ImageIds");
            if (Ids == null)
            {
                Ids = new List<int>();
            }
            if (Ids.Contains(id))
            {
                return Redirect($"/home/ViewImage?id={id}&password={ir.GetImageById(id).Password}");
            }

            return View(new ViewModel { Image = ir.GetImageById(id), IncorrectPasswordMessage = (string)TempData["incorrectPassword"] });
        }

        public IActionResult ViewImage(int id, string password)
        {
            ImageRepository ir = new(_connectionString);

            Image image = ir.VerifyPassword(id, password);
            if (image == null)
            {
                TempData["incorrectPassword"] = "Invalid password";
                return Redirect($"/home/getPassword?id={id}");
            }

            List<int> ImageIds = HttpContext.Session.Get<List<int>>("ImageIds");
            if (ImageIds == null)
            {
                ImageIds = new List<int>();
            }
            ImageIds.Add(id);

            HttpContext.Session.Set("ImageIds", ImageIds);
            return View(new ViewModel { Image = image });
        }
    }
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);
            return value == null ? default(T) :
                JsonConvert.DeserializeObject<T>(value);
        }
    }
}
