using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoanManagement.Models;
namespace LoanManagement.Controllers
{
    public class DeptController : TemplateController
    {

        public ActionResult GetDeptByID(long uID)
        {
            using (LoanDataDataContext database = new LoanDataDataContext())
            {
                var track = (from d in database.Sys_Depts

                             where d.ID == uID
                             select d).FirstOrDefault();
                return Json(track);
            }


        }
        //
        // GET: /Indexs/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetDeptToGrid(string sidx, string sord, int page, int rows, string filters)
        {

            // List<Sys_User> depts = new List<Sys_User>();
            using (LoanDataDataContext database = new LoanDataDataContext())
            {
                var usr = (from d in database.Sys_Depts
                         
                          
                           select d).ToList();


                // depts.AddRange(usr.ToList());
                if (!string.IsNullOrEmpty(filters))
                {
                   
                    
                        var temp = from l in usr where l.Name.Contains(filters) select l;
                        //loan = loan.Select(l=>l.Client_Acc==AccID).ToList();
                        usr = temp.ToList();
                    
                   
                }

                int pageIndex = Convert.ToInt32(page) - 1;
                int pageSize = rows;

                int totalRecords = usr.Count();




                var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
                usr = usr.Skip(pageIndex * pageSize).Take(pageSize).ToList();

                var jsonData = new
                {
                    total = totalPages,
                    page,
                    records = totalRecords,
                    rows = usr
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }


        }
        public ActionResult CreateWindow()
        {
            return PartialView("CreateDept");
        }

        [HttpPost]
        public ActionResult CreateDept(FormCollection collection)
        {
            try
            {

                string name = collection.Get("Name");
                string note = collection.Get("Note");

             
              


                using (LoanDataDataContext udatabase = new LoanDataDataContext())
                {
                    Sys_Dept act = new Sys_Dept();
                    act.Name = name;
                    act.Note = note;
                    

                    udatabase.Sys_Depts.InsertOnSubmit(act);
                    udatabase.SubmitChanges();
                }
                var jsonObj = new { test = "Department Has Been Created" };
                return Json(jsonObj);
            }
            catch (Exception exc)
            {
                var jsonObj = new { Error = exc.Message };
                return Json(jsonObj);
            }
        }


        public ActionResult UpdateWindow(int id)
        {

            return PartialView("UpdateDept");

        }
       
        [HttpPost]
        public ActionResult UpdateDept(FormCollection collection)
        {
            try
            {

                string id = collection.Get("ID");
                string name = collection.Get("Name");
                string note = collection.Get("Note");

               


                using (LoanDataDataContext udatabase = new LoanDataDataContext())
                {
                    Sys_Dept act = udatabase.Sys_Depts.FirstOrDefault(u => u.ID == int.Parse(id));

                    act.Name = name;
                    act.Note = note;
                    
                


                    udatabase.SubmitChanges();
                }
                var jsonObj = new { test = "Department Has Been Updated" };
                return Json(jsonObj);
            }
            catch (Exception exc)
            {
                var jsonObj = new { Error = exc.Message };
                return Json(jsonObj);
            }
        }

    }
}
