using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoanManagement.Models;
namespace LoanManagement.Controllers
{
    public class IndexsController : TemplateController
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

        public ActionResult GetActionsToGrid(string sidx, string sord, int page, int rows, string filters)
        {

            // List<Sys_User> depts = new List<Sys_User>();
            using (LoanDataDataContext database = new LoanDataDataContext())
            {
                var usr = (from d in database.Actions
                         
                          
                           select d).ToList();


                // depts.AddRange(usr.ToList());
                if (!string.IsNullOrEmpty(filters))
                {
                    var search = filters.Split('&');
                    if (!string.IsNullOrEmpty(search[0]))
                    {
                        var temp = from l in usr where l.Name == search[0] select l;
                        //loan = loan.Select(l=>l.Client_Acc==AccID).ToList();
                        usr = temp.ToList();
                    }
                    if (!string.IsNullOrEmpty(search[1]))
                    {
                        var temp = from l in usr where l.ActionType.ToString() == search[1] select l;
                        //loan = loan.Select(l=>l.Client_Acc==AccID).ToList();
                        usr = temp.ToList();
                    }
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
            return PartialView("Create");
        }

        [HttpPost]
        public ActionResult CreateAction(FormCollection collection)
        {
            try
            {

                string name = collection.Get("Name");
                string code = collection.Get("ACTION_CODE");

                int type = int.Parse(collection.Get("ActionType"));

              


                using (LoanDataDataContext udatabase = new LoanDataDataContext())
                {
                    LoanManagement.Models.Action act = new LoanManagement.Models.Action();
                    act.Name = name;
                    act.ACTION_CODE = code;
                    act.ActionType = type;
                 

                    udatabase.Actions.InsertOnSubmit(act);
                    udatabase.SubmitChanges();
                }
                var jsonObj = new { test = "Action Has Been Created" };
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

            return PartialView("Update");

        }
        public ActionResult GetActionByID(int aID)
        {
            using (LoanDataDataContext database = new LoanDataDataContext())
            {
                var track = (from d in database.Actions

                             where d.ID == aID
                             select d).FirstOrDefault();
                return Json(track);
            }


        }
        [HttpPost]
        public ActionResult UpdateAction(FormCollection collection)
        {
            try
            {

                string id = collection.Get("ID");
                string name = collection.Get("Name");
                string code = collection.Get("ACTION_CODE");

                int type = int.Parse(collection.Get("ActionType"));




                using (LoanDataDataContext udatabase = new LoanDataDataContext())
                {
                    LoanManagement.Models.Action act = udatabase.Actions.FirstOrDefault(u => u.ID == int.Parse(id));

                    act.Name = name;
                    act.ACTION_CODE = code;
                    act.ActionType = type;
                 
                


                    udatabase.SubmitChanges();
                }
                var jsonObj = new { test = "Action Has Been Updated" };
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
