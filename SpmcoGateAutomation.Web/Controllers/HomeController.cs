using Microsoft.AspNetCore.Mvc;
using SpmcoGateAutomation.Web.Models;
using System.Data;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Net.Sockets;
using System.Security.Permissions;
using System.Security;
using System.Text;
using System.Net;

namespace SpmcoGateAutomation.Web.Controllers
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

        public IActionResult GateIn()
        {
            return View();
        }

        public IActionResult GateOut()
        {
            return View();
        }

    }
}