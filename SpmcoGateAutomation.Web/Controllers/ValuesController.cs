using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SpmcoGateAutomation.Web.Controllers
{
    [ApiController]
    public class ValuesController : Controller
    {
        [HttpGet("Index")]
        public string Index()
        {
            return "asdf";
        }
    }
}
