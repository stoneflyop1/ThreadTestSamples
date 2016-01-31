using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace TestWeb.Controllers
{
    public class HomeController : Controller
    {
        // no lock
        public async Task<ActionResult> Index()
        {
            var content = await GetContentAsync("https://www.baidu.com");
            return View((object)"https://www.baidu.com");
        }
        //no lock
        public ActionResult Result()
        {
            var content = GetContent("https://www.baidu.com");
            return View((object)"https://www.baidu.com");
        }
        // lock
        public ActionResult Lock()
        {
            var content = GetContentAsync("https://www.baidu.com").Result;
            return View((object)"https://www.baidu.com");
        }

        //no lock
        public ActionResult Pool()
        {
            var content = GetContentWithPoolAsync("https://www.baidu.com").Result;
            return View((object)"https://www.baidu.com");
        }

        private static async Task<string> GetContentAsync(string url)
        {
            using (var client = new HttpClient())
            {
                var res = await client.GetAsync(url);
                return await res.Content.ReadAsStringAsync();
            }
        }

        private static string GetContent(string url)
        {
            using (var client = new HttpClient())
            {
                var res = client.GetAsync(url).Result;
                return res.Content.ReadAsStringAsync().Result;
            }
        }

        private static Task<string> GetContentWithPoolAsync(string url)
        {
            return Task.Run(async () =>
            {
                using (var client = new HttpClient())
                {
                    var res = await client.GetAsync(url);
                    return await res.Content.ReadAsStringAsync();
                }
            });
        }
    }
}