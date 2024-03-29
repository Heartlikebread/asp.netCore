﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FileUpload.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Hosting;

namespace FileUpload.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IHostEnvironment _env; //抓專案位置
        public HomeController(ILogger<HomeController> logger, IHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult SingleFile(IFormFile file) {
            var dir = _env.ContentRootPath;
            string path = @"D:\Test.txt";

            // 建立 FileStream 並實例化
            // 開啟或建立指定路徑檔案，訪問權限唯讀
            FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read);

            StreamReader reader = new StreamReader(file.OpenReadStream());
            String content = reader.ReadToEnd();
            String name = file.FileName;
            String filename = @"D:/Test/" + name;
            if (System.IO.File.Exists(filename))
            {
                System.IO.File.Delete(filename);
            }
            using (FileStream fss = System.IO.File.Create(filename))
            {
           
                file.CopyTo(fss);

                fss.Flush();
            }
            //using (var fileStream = new FileStream(Path.Combine(@"d:\", file.FileName), FileMode.Create, FileAccess.Write))
            //{
            //    file.CopyTo(fileStream);
            //}



                return RedirectToAction("Index");
        }

        public IActionResult MultipleFiles(IEnumerable<IFormFile> files)
        {
            int i = 0;
            foreach (var file in files) {
                using (var fileStream = new FileStream(Path.Combine(@"d:\Img", $"file{i++}.png"), FileMode.Create, FileAccess.Write))
                {
                    file.CopyTo(fileStream);
                }                
            }
            return RedirectToAction("Index");
        }

        public IActionResult FileModel(SomeForm someForm)
        {
            var dir = _env.ContentRootPath;
            using (var fileStream = new FileStream(Path.Combine(@"d:\Img",
                someForm.Name +".png"),
                FileMode.Create,
                FileAccess.Write))
            {
                someForm.File.CopyTo(fileStream);
            }
            return RedirectToAction("Index");
        }
    }
}
