using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVCTest.Models;

namespace MVCTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            #region 一、撰寫9*9乘法表 秀在 頁面上
            var _ValueList = new List<ValueList>();
            for (int i = 1; i <= 9; i++)
            {
                var newValue = new ValueList();
                newValue.value = new List<string>();
                newValue.num = i;
                for (int j = 1; j <= 9; j++)
                {
                    int ans = i * j;
                    string tns = String.Format(" {0}*{1}={2}",
                         i, j, ans);
                    newValue.value.Add(tns);
                }
                _ValueList.Add(newValue);
            }
            #endregion
            #region 二、判斷字串中若有乘數、被乘數與答案為5再帶回答案
            //var _newValueList = new List<ValueList>();
            //_ValueList.ForEach(x => {
            //  var valueLsit=new ValueList();
            //    valueLsit.value = new List<string>();                
            //    x.value.ForEach(y => {
            //        if (y.Contains("5")){
            //            valueLsit.value.Add(y);
            //            valueLsit.num = x.num;
            //        }                
            //    });
            //    _newValueList.Add(valueLsit);
            //});
            #endregion
            #region 三、使用九九乘法答案在做指定乘數為5的帶回答案
            //var _newValueList = new List<ValueList>();
            //_ValueList.ForEach(x => {
            //    var valueLsit = new ValueList();
            //    valueLsit.value = new List<string>();
            //    x.value.ForEach(y => {
            //        var arrx = y.Split('*');
            //        var arry= arrx[1].Split('=');

            //        if (arry[0]=="5")
            //        {
            //            valueLsit.value.Add(y);
            //            valueLsit.num = x.num;
            //        }
            //    });
            //    _newValueList.Add(valueLsit);
            //});
            #endregion
            return View(_ValueList);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
