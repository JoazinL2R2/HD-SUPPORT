using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HD_SUPPORT.Controllers
{
    public class CadastroHelpDeskController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
