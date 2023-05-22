using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using WebAPI.Models;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebAPI.Models;
using static WebAPI.Models.JSONObj;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System.Xml.Linq;

namespace WebAPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        string baseURL = "https://jsonplaceholder.typicode.com/users";

        //Added object and list outside, to be accessible from any controller, since I am not working with real DB, so I needed that for the purpose of storing changes.
        JSONObj obj = new JSONObj();
        List<JSONObj> data = new List<JSONObj>();
        string json;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            //Calling the web API and populating data in view using DataTable
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage getData = await client.GetAsync(baseURL);

                if (getData.IsSuccessStatusCode)
                {
                    var addedUser = TempData["stringObj"];
                    string results = getData.Content.ReadAsStringAsync().Result;

                    data = JsonConvert.DeserializeObject<List<JSONObj>>(results);
                    json = System.Text.Json.JsonSerializer.Serialize(data);
                    TempData["json"] = json;
                    if (addedUser != null)
                    {
                        var desObj = JsonConvert.DeserializeObject<JSONObj>((string)addedUser);
                        data.Add(desObj);
                        json = System.Text.Json.JsonSerializer.Serialize(data);
                        TempData["json"] = json;
                    }
                }
                else
                {
                    Console.WriteLine("Error calling web API");
                }
                if (data != null)
                {
                    ViewData.Model = data;
                }
            }

            return View();
        }

        public async Task<ActionResult<string>> AddUser(JSONObj user)
        {
            if (user != null)
            {
                //This can be also a list to store a multiple users, instead od current object (obj), but this is for demo purposes (the new user will be added to the main list).
                obj = new JSONObj()
                {
                    name = user.name,
                    username = user.username,
                    email = user.email,
                    address = user.address,
                    phone = user.phone,
                    company = user.company,
                };
            }

            TempData["stringObj"] = JsonConvert.SerializeObject(obj);

            if (obj.name != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public async Task<IActionResult> SaveTable()
        {
            json = (string)TempData["json"];
            //The stored file destination is inside of the project destionation, that can be also change for example: File.WriteAllText(@"D:\path.json", json);
            System.IO.File.WriteAllText("SavedJSON.json", json);

            return RedirectToAction("Index", "Home");
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
    }
}