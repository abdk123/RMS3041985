using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoanManagement.Models;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;
namespace LoanManagement.Controllers
{
    
    public class FormsController : TemplateController
    {
       

        public ActionResult Index()
        {
            return View();
        }
      
        

    }
}
