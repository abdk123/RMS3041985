using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoanManagement.Models;
using System.Collections;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.IO;
using LoanManagement.Models.Security;

namespace LoanManagement.Controllers
{
    public class UserManagementController : TemplateController
    {
        //here to get all users

        public ActionResult GetUserToGrid(string sidx, string sord, int page, int rows, string filters)
        {

            // List<Sys_User> depts = new List<Sys_User>();
            using (LoanDataDataContext database = new LoanDataDataContext())
            {
                var usr = (from d in database.Sys_Users
                               // join dep in database.Sys_Depts on d.Dept equals dep.ID
                           where d.id != 1
                           select new
                           {
                               id = d.id,
                               user_name = d.user_name,
                               name = d.name,
                               dept_name = database.Sys_Depts.FirstOrDefault(dep => dep.ID == d.Dept) != null ? database.Sys_Depts.FirstOrDefault(dep => dep.ID == d.Dept).Name : "",
                               IsDeptAdmin = d.IsDeptAdmin,
                               RO_Name = d.RO_Name,
                               RO_Code = d.RO_Code,
                               Activate = d.Activate
                           }).ToList();


                // depts.AddRange(usr.ToList());
                if (!string.IsNullOrEmpty(filters))
                {
                    var search = filters.Split('&');
                    if (!string.IsNullOrEmpty(search[0]))
                    {
                        var temp = from l in usr where l.user_name == search[0] select l;
                        //loan = loan.Select(l=>l.Client_Acc==AccID).ToList();
                        usr = temp.ToList();
                    }
                    if (!string.IsNullOrEmpty(search[1]))
                    {
                        var temp = from l in usr where l.RO_Code == search[1] select l;
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

        //  
        [HttpGet]
        public ActionResult getEmpdataToTrack(string searchTerm)
        {
            using (LoanDataDataContext udatabase = new LoanDataDataContext())
            {
                var res = from r in udatabase.Sys_Users where r.name.Contains(searchTerm) && (r.Typ == 2 || r.Typ == 3) select new { id = r.id, text = r.name };

                return Json(res.ToList(), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpGet]
        public ActionResult getRoTrack(string searchTerm)
        {
            using (LoanDataDataContext udatabase = new LoanDataDataContext())
            {
                var res = from r in udatabase.Sys_Users where r.name.Contains(searchTerm) && (r.Typ == 2 || r.Typ == 3) select new { id = r.id, text = r.name + "_" + (r.RO_Code != null ? r.RO_Code : "") };

                return Json(res.ToList(), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpGet]
        public ActionResult getDeptsForUser(string searchTerm)
        {
            using (LoanDataDataContext udatabase = new LoanDataDataContext())
            {
                var res = from r in udatabase.Sys_Depts where r.Name.Contains(searchTerm) select new { id = r.ID, text = r.Name };

                return Json(res.ToList(), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpGet]
        public ActionResult getLegal(string searchTerm)
        {
            using (LoanDataDataContext udatabase = new LoanDataDataContext())
            {
                var res = from r in udatabase.Sys_Users where r.Typ == 4 select new { id = r.id, text = r.name };

                return Json(res.ToList(), JsonRequestBehavior.AllowGet);
            }

        }

        //[HttpGet]
        //public ActionResult getPrntUser(string searchTerm)
        //{
        //    using (UserManagement udatabase = new UserManagement())
        //    {
        //        var res = from r in udatabase.AMJ_USER where r.USRNAME.Contains(searchTerm) select new { id = r.USRNAME, text = r.USRNAME };

        //        return Json(res.ToList(), JsonRequestBehavior.AllowGet);
        //    }

        //}
        //here should 
        //[HttpGet]
        //public ActionResult getGroupsForUser(string searchTerm)
        //{
        //    using (UserManagement udatabase = new UserManagement())
        //    {
        //        ArrayList lstArr = new ArrayList();
        //        lstArr.Add(new { id=-1,text="عدم اختيار قيمة"});
        //        var res = from r in udatabase.AMJ_GROUP where r.NAME.Contains(searchTerm) select new { id = r.ID, text = r.NAME };
        //        lstArr.AddRange(res.ToList());
        //        return Json(lstArr, JsonRequestBehavior.AllowGet);
        //    }

        //}

        // [HttpGet]
        //public ActionResult ProfileDDL(string searchTerm)
        //{
        //    using (UserManagement udatabase = new UserManagement())
        //    {
        //        ArrayList lstArr = new ArrayList();
        //        lstArr.Add(new { id = -1, text = "عدم اختيار قيمة" });
        //        var res = from r in udatabase.AMJ_PROFILE where r.NAME.Contains(searchTerm) select new { id = r.ID, text = r.NAME };
        //        lstArr.AddRange(res.ToList());
        //        return Json(res.ToList(), JsonRequestBehavior.AllowGet);
        //    }

        //}
        //  [HttpGet]
        // public ActionResult GetGroupForUsr(string ff)
        //{
        //    using (UserManagement udatabase = new UserManagement())
        //    {
        //        int i = int.Parse(ff);
        //        var res = from r in udatabase.AMJ_GROUP where r.ID==i select new { id = r.ID, text = r.NAME };

        //        return Json(res.ToList(), JsonRequestBehavior.AllowGet);
        //    }

        //}

        //
        // GET: /UserManagement/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /UserManagement/Details/5

        public ActionResult CreateWindow()
        {
            return PartialView("Create");
        }
        public ActionResult ChangePWD()
        {
            return PartialView("CHANGEPWD");
        }

        public ActionResult CheckIfUsrNameExist(string userName)
        {
            try
            {

                using (LoanDataDataContext database = new LoanDataDataContext())
                {

                    var usr = database.Sys_Users.FirstOrDefault(un => un.user_name == userName);
                    if (usr != null)
                    {
                        var results = new { Message = "OK" };
                        return Json(results);
                    }

                }

                var jsonObj = new { Message = "NO" };
                return Json(jsonObj);
            }
            catch (Exception ex)
            {

                var jsonObj = new { Message = ex.Message };
                return Json(jsonObj);
            }
        }

        //
        // POST: /UserManagement/Create

        [HttpPost]
        public ActionResult CreateUser(FormCollection collection)
        {
            try
            {

                string user_name = collection.Get("user_name");
                string password = collection.Get("password");

                string name = collection.Get("name");

                string sur_name = collection.Get("sur_name");
                //   string father_name = collection.Get("father_name");
                //   string mother_name = collection.Get("mother_name");

                string Typ = collection.Get("Typ");
                string dept = collection.Get("Dept");
                string isAdmin = collection.Get("IsDeptAdmin");
                string RON = collection.Get("RO_Name");
                string ROC = collection.Get("RO_Code");
                string Activate = collection.Get("Activate");


                using (LoanDataDataContext udatabase = new LoanDataDataContext())
                {
                    Sys_User usr = new Sys_User();
                    usr.user_name = user_name;
                    usr.password = RmsEncryption.Encrypt(password);
                    usr.sur_name = sur_name;
                    usr.name = name;
                    //   usr.mother_name = mother_name;
                    //  usr.father_name = father_name;
                    //  usr.PRNTID = PRNTDDL;
                    usr.RO_Name = RON;
                    usr.RO_Code = ROC;
                    if (!string.IsNullOrEmpty(Typ))
                    {
                        usr.Typ = short.Parse(Typ);
                    }

                    if (!string.IsNullOrEmpty(dept))
                    {
                        usr.Dept = int.Parse(dept);
                    }


                    if (!string.IsNullOrEmpty(isAdmin))
                    {
                        usr.IsDeptAdmin = isAdmin.ToLower() == "true" ? true : false;
                    }
                    if (!string.IsNullOrEmpty(Activate))
                    {
                        usr.Activate = Activate.ToLower() == "true" ? true : false;
                    }
                    //  usr.IS_ADMIN = IS_ADMIN =="True" ? "Y" : "N";

                    udatabase.Sys_Users.InsertOnSubmit(usr);
                    udatabase.SubmitChanges();
                }
                var jsonObj = new { test = "User Has Been Created" };
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

            return PartialView("UpdateUser");

        }
        public ActionResult DeleteWindow(int id)
        {
            return PartialView("DeleteUser");
        }

        [HttpPost]
        public ActionResult UpdateUser(FormCollection collection)
        {
            try
            {

                string UID = collection.Get("UserID");
                string password = collection.Get("password");

                string name = collection.Get("name");

                string sur_name = collection.Get("sur_name");

                string Typ = collection.Get("Typ");
                string dept = collection.Get("Dept");
                string isAdmin = collection.Get("IsDeptAdmin");
                string RON = collection.Get("RO_Name");
                string Activate = collection.Get("Activate");



                using (LoanDataDataContext udatabase = new LoanDataDataContext())
                {
                    Sys_User usr = udatabase.Sys_Users.FirstOrDefault(u => u.id == long.Parse(UID));

                    usr.password = RmsEncryption.Encrypt(password);
                    usr.sur_name = sur_name;
                    usr.name = name;
                    usr.RO_Name = RON;

                    if (!string.IsNullOrEmpty(Typ))
                    {
                        usr.Typ = short.Parse(Typ);
                    }

                    if (!string.IsNullOrEmpty(dept))
                    {
                        usr.Dept = int.Parse(dept);
                    }


                    if (!string.IsNullOrEmpty(isAdmin))
                    {
                        usr.IsDeptAdmin = isAdmin.ToLower() == "true" ? true : false;
                    }

                    if (!string.IsNullOrEmpty(Activate))
                    {
                        usr.Activate = Activate.ToLower() == "true" ? true : false;
                    }

                    udatabase.SubmitChanges();
                }
                var jsonObj = new { test = "User Has Been Updated" };
                return Json(new { message = jsonObj }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exc)
            {
                var jsonObj = new { Error = exc.Message };
                return Json(new { message = jsonObj }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ChangePassword(string userName, string pass)
        {
            try
            {


                using (LoanDataDataContext udatabase = new LoanDataDataContext())
                {
                    Sys_User usr = udatabase.Sys_Users.FirstOrDefault(u => u.user_name == userName);

                    usr.password = RmsEncryption.Encrypt(pass); ;

                    udatabase.SubmitChanges();
                }
                var jsonObj = new { test = "User Has Been Updated" };
                return Json(new { message = jsonObj }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exc)
            {
                var jsonObj = new { Error = exc.Message };
                return Json(new { message = jsonObj }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult DeleteUser(FormCollection collection)
        {
            try
            {
                string UID = collection.Get("id");
                using (LoanDataDataContext udatabase = new LoanDataDataContext())
                {
                    Sys_User usr = udatabase.Sys_Users.FirstOrDefault(u => u.id == long.Parse(UID));
                    udatabase.Sys_Users.DeleteOnSubmit(usr);
                    udatabase.SubmitChanges();
                }
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }


        }

        public ActionResult GetUserByID(long uID)
        {
            using (LoanDataDataContext database = new LoanDataDataContext())
            {
                var track = (from d in database.Sys_Users

                             where d.id == uID
                             select d).FirstOrDefault();
                return Json(track);
            }


        }


        //
        // GET: /UserManagement/Edit/5

        //public ActionResult EditWind(string usrNam)
        //{
        //    using (UserManagement udatabase=new UserManagement())
        //    {
        //        AMJ_USER usr = udatabase.AMJ_USER.SingleOrDefault(u=>u.USRNAME==usrNam);
        //        //string groupe=string.IsNullOrEmpty(usr.GROUPID)
        //        //here should create a json object to retrive group name and profile name 
        //        string groupNm=usr.GROUPID.HasValue ?udatabase.AMJ_GROUP.SingleOrDefault(g=>g.ID==usr.GROUPID).NAME : "لم يتم اضافة مجموعة" ;
        //        string ProfName = usr.PROFILEID.HasValue ? udatabase.AMJ_PROFILE.SingleOrDefault(p => p.ID == usr.PROFILEID.Value).NAME : "لم يتم تحديد برفايل";
        //       // string f=usr.GNDR.ToLower()=="f" ? "checked='checked'"
        //        string f="", m="";
        //        if (usr.GNDR!=null)
        //        {
        //            if (usr.GNDR.ToLower() == "f")
        //            {
        //                f = "checked=checked";
        //            }
        //            else if (usr.GNDR.ToLower() == "m")
        //            {
        //                m = "checked=checked";
        //            }

        //        }

        //        var jsonObject = new { 
        //            userName=usr.USRNAME,
        //            pwd=usr.PWD,
        //            GroupName = groupNm,
        //            profile=ProfName,
        //            note=usr.NOTE,
        //            fullName=usr.FULLNAME,
        //           F=f,
        //           M=m
        //        };
        //        return PartialView("Edit", jsonObject);
        //    }

        //}

        //
        // POST: /UserManagement/Edit/5

        //[HttpPost]
        //public ActionResult Edit(FormCollection collection)
        //{
        //    try
        //    {

        //        string userName = collection.Get("USRNAME");
        //        string note = collection.Get("NOTE");
        //        string PWD = collection.Get("PWD");
        //        // string PCIP = collection.Get("PCIP");
        //        // string PCNAME = collection.Get("PCNAME");
        //        string FULLNAME = collection.Get("FULLNAME");
        //        // string PRNTDDL = collection.Get("PRNTDDL");
        //        string GRPDDL = collection.Get("GRPDDL");
        //        string ROLES = collection.Get("ROLES");
        //        string ProfileDDL = collection.Get("ProfileDDL");
        //        // string IS_ADMIN = collection.Get("IS_ADMIN");
        //        string GNDR = collection.Get("GNDR");

        //        //using (UserManagement udatabase = new UserManagement())
        //        //{
        //        //    AMJ_USER usr = udatabase.AMJ_USER.SingleOrDefault(u=>u.USRNAME==userName);

        //        //    usr.PWD = PWD;
        //        //    usr.FULLNAME = FULLNAME;
        //        //    usr.GNDR = GNDR;
        //        //    usr.NOTE = note;
        //        //    if (!string.IsNullOrEmpty(GRPDDL))
        //        //    {
        //        //        if (GRPDDL != "-1")
        //        //        {
        //        //            usr.GROUPID = long.Parse(GRPDDL);
        //        //        }
        //        //    }
        //        //    if (!string.IsNullOrEmpty(ProfileDDL))
        //        //    {
        //        //        if (ProfileDDL != "-1")
        //        //        {
        //        //            usr.PROFILEID = int.Parse(ProfileDDL);
        //        //        }   
        //        //    }

        //        //    udatabase.SaveChanges();
        //        //    //here where should delete 
        //        //    var allPrevRole = from ur in udatabase.AMJ_USER_ROLE where ur.USRNAME == userName select ur;
        //        //    foreach (var item in allPrevRole)
        //        //    {
        //        //        udatabase.AMJ_USER_ROLE.Remove(item);
        //        //    }
        //        //    udatabase.SaveChanges();
        //        //    foreach (var item in ROLES.Split(','))
        //        //    {
        //        //        AMJ_USER_ROLE usrRole = new AMJ_USER_ROLE();
        //        //        usrRole.ID = ProxyUtility.GetSEQID();
        //        //        usrRole.ROLEID = decimal.Parse(item);
        //        //        usrRole.USRNAME = userName;
        //        //        usrRole.GRANTDAT = DateTime.Now;
        //        //        udatabase.AMJ_USER_ROLE.Add(usrRole);

        //        //    }
        //        //    udatabase.SaveChanges();


        //        }
        //        var jsonObj = new { test = "Ok" };
        //        return Json(jsonObj);

        //    }
        //    catch(Exception exc)
        //    {
        //        var jsonObj = new { Error = exc.Message };
        //        return Json(jsonObj);
        //    }
        //}

        //
        // GET: /UserManagement/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /UserManagement/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        

    }
}
