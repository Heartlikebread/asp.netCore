using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Notify20200619.Models;

namespace Notify20200619.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _tokenUrl;
        private readonly string _redirectUri;
        private readonly string _successUri;    
        private readonly string _notifyUrl;
        private readonly ILogger<HomeController> _logger;
        private  IConfiguration _config { get; set; }
        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            var lineConfig = _config.GetSection("LineNotify");
            _notifyUrl= lineConfig.GetValue<string>("notifyUrl");
            _tokenUrl = lineConfig.GetValue<string>("tokenUrl");
            _redirectUri = lineConfig.GetValue<string>("redirectUri");
            _successUri = lineConfig.GetValue<string>("successUri");
           
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy( string token)
        {
            ViewData["token"] = token;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

     

        /// <summary>
        /// 取得token
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <param name="error"></param>
        /// <param name="errorDescription"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCallback(
        [FromQuery]string code,
        [FromQuery]string state,
        [FromQuery]string error,
        [FromQuery][JsonProperty("error_description")]string errorDescription)
        {
            if (!string.IsNullOrEmpty(error))
                return new JsonResult(new
                {
                    error,
                    state,
                    errorDescription
                });

            ViewData["code"] = code;
            return View();
        }
        [HttpGet]
        /// <summary>取得使用者 Token</summary>
        /// <param name="code">用來取得 Access Tokens 的 Authorize Code</param>
        /// <returns></returns>
        public async Task<IActionResult> FetchToken([FromQuery]string code, [FromQuery]string clientId, [FromQuery]string clientSecret)
        {
            var token = "";
            using (var client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 0, 60);
                client.BaseAddress = new Uri(_tokenUrl);

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("redirect_uri", _redirectUri),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret)
                });
                var response = await client.PostAsync("", content);
                var data = await response.Content.ReadAsStringAsync();
                token = JsonConvert.DeserializeObject<JObject>(data)["access_token"].ToString();
            }
            Response.Redirect(_successUri + "?token=" + token);
            return new EmptyResult();
        }

        /// <summary>傳送文字訊息</summary>
        /// <param name="msg">訊息</param>
        [HttpGet]
        public async Task<IActionResult> SendMessage( string Token, string Message)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_notifyUrl);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);

                var form = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("message",Message)
                });

                await client.PostAsync("", form);
            }

            return new EmptyResult();
        }
    }
}
