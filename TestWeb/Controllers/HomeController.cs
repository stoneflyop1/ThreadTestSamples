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
        private readonly string Host = "https://www.baidu.com";
        // no lock
        public async Task<ActionResult> Index()
        {
            var content = await GetContentAsync(Host);
            return View((object)Host);
        }
        //no lock
        public ActionResult Result()
        {
            var content = GetContent(Host);
            return View((object)Host);
        }
        // lock
        public ActionResult Lock()
        {
            var content = GetContentAsync(Host).Result;
            return View((object)Host);
        }

        //no lock
        public ActionResult Pool()
        {
            var content = GetContentWithPoolAsync(Host).Result;
            return View((object)Host);
        }
        // no lock
        public async Task<ActionResult> PoolAsync()
        {
            var content = await GetContentWithPoolAsync(Host);
            return View((object)Host);
        }

        //no lock
        public ActionResult WaitFalse()
        {
            var content = GetContentConfigWaitFalseAsync(Host).Result;
            return View((object)Host);
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

        private static async Task<string> GetContentConfigWaitFalseAsync(string url)
        {
            using (var client = new HttpClient())
            {
                var res = await client.GetAsync(url).ConfigureAwait(false);
                return await res.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
        }
    }
}