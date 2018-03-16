using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoanManagement.Models;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;

using System.Linq.Dynamic;
using System.Web.Script.Serialization;

namespace LoanManagement.Controllers
{

    public class FollowUpController : TemplateController
    {
        public ActionResult GetDataToGrid(string sidx, string sord, int page, int rows, string filters)
        {
            string ClientId = Convert.ToString(TempData["ClientId"]);
            if (!String.IsNullOrEmpty(ClientId))
                filters = ClientId + "&";

            using (LoanDataDataContext database = new LoanDataDataContext())
            {
                // var loan = from d in database.Client_Loan_Infos select d;
                long uid = long.Parse(TGSession.ClientInfo.UID);

                //This work sucess
                var loan = (from d in database.Tracks

                            join cl in database.Client_Loan_Infos on d.ClientLoanID equals cl.ID
                            join l in database.Loan_Infos on cl.LoanID equals l.ID
                            join c in database.CLIENT_PERINFOs on cl.ClientID equals c.ID
                            join g in database.Guarantees_Infos on cl.GuaranteesID equals g.ID
                            //join track in database.Sys_Users on d.CurrTraker equals track.id
                            where d.CurrTraker == uid
                            select new
                            {
                                ID = d.ID,
                                Client_Info = c,
                                Loan_Info = cl,
                                guarant = g,
                                tracks = d,
                                trackers = database.Trackers.FirstOrDefault(t => t.Trackers == d.CurrTraker),
                                loan = l,
                                legal_state = Util.GetLegalStatus(d.SendLegalState.Value),
                                lg_stae = d.SendLegalState.HasValue ? d.SendLegalState.Value : 0
                                //   Trackers=database.Trackers.Where(t=>t.TracksID==d.ID).OrderByDescending(t=>t.ID).FirstOrDefault().Tracking_USER
                                //  Last_Tracking = d.NOTE
                            });


                try
                {
                    if (filters != null)
                    {
                        var serializer = new JavaScriptSerializer();
                        var filterObj = serializer.DeserializeObject(filters);

                        //var filterPara = (Dictionary<string,object>)filterObj;
                        //var filterRules = (object[])filterPara["rules"];
                        //string query=BuildFilterQuery(filterRules[0]);
                    }
                }
                catch { }

                try
                {


                    // depts.AddRange(dep.ToList());
                    if (!string.IsNullOrEmpty(filters))
                    {
                        var search = filters.Split('&');
                        if (!string.IsNullOrEmpty(search[0]))
                        {
                            var temp = from l in loan where l.Client_Info.AccountID == search[0] select l;
                            //loan = loan.Select(l=>l.Client_Acc==AccID).ToList();
                            loan = temp;
                        }
                        if (!string.IsNullOrEmpty(search[1]))
                        {
                            var temp = from l in loan where l.Client_Info.FULLNAME.Contains(search[1]) select l;
                            //loan = loan.Select(l=>l.Client_Acc==AccID).ToList();
                            loan = temp;
                        }
                    }
                }
                catch (Exception ex)
                {

                }

                int pageIndex = Convert.ToInt32(page) - 1;
                int pageSize = rows;

                int totalRecords = loan.Count();




                var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

                if (string.IsNullOrEmpty(sidx))
                    sidx = "Client_Info.AccountID";

                loan = loan.OrderBy(sidx + " " + sord);
                loan = loan.Skip(pageIndex * pageSize).Take(pageSize);

                //processResults = SortIQueryable(processResults, sidx, sord);



                var jsonData = new
                {
                    total = totalPages,
                    page,
                    records = totalRecords,
                    rows = loan.ToList().Count != 0 ? loan.ToList() : null
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }


        }

        private string BuildFilterQuery(object filterRules)
        {

            var values = (object[])filterRules;
            var field = (KeyValuePair<string, string>)values[0];

            return "";
        }

        private static readonly IDictionary<string, string> operators = new Dictionary<string, string>
        {
            {"eq", "="},
            {"neq", "!="},
            {"lt", "<"},
            {"lte", "<="},
            {"gt", ">"},
            {"gte", ">="},
            {"startswith", "StartsWith"},
            {"endswith", "EndsWith"},
            {"contains", "Contains"},
            {"doesnotcontain", "Contains"}
        };

        public ActionResult GetDataToGridAddFollow(string sidx, string sord, int page, int rows, string filters)
        {


            using (LoanDataDataContext database = new LoanDataDataContext())
            {
                // var loan = from d in database.Client_Loan_Infos select d;

                //This work sucess
                long tid = 0;
                if (!string.IsNullOrEmpty(filters))
                {
                    tid = long.Parse(filters);
                }
                var follow = (from f in database.Daily_Follow_Ups
                              join us in database.Sys_Users on f.User_Follow equals us.id
                              where f.Tracks_ID == tid
                              select new
                              {
                                  ID = f.ID,
                                  Follow_Date = f.Follow_Date,
                                  Follow_Action = database.Actions.FirstOrDefault(a => a.ID == f.Follow_Action).Name,
                                  Follow_Note = f.Follow_Note,
                                  Remainder_Date = f.Remainder_Date,
                                  Remainder_Note = f.Remainder_Note,
                                  RO_Name = us.RO_Name
                              }).ToList();




                int pageIndex = Convert.ToInt32(page) - 1;
                int pageSize = rows;

                int totalRecords = follow.Count();




                var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
                follow = follow.Skip(pageIndex * pageSize).Take(pageSize).ToList();


                var jsonData = new
                {
                    total = totalPages,
                    page,
                    records = totalRecords,
                    rows = follow
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }


        }
        public ActionResult GetLegalProcedure(long LegID)
        {


            using (LoanDataDataContext database = new LoanDataDataContext())
            {
                // var loan = from d in database.Client_Loan_Infos select d;

                //This work sucess
                var lp = (from d in database.Legal_Procedures


                          where d.LegalRepID == LegID
                          select d);

                string result = string.Empty;
                string temp = @"<tr legproitem='true'> <td>
                                         <span type='text' >{desc}</span>
                                     </td>
                                      <td>
                                         <span type='text' >{pro}</span>
                                     </td></tr>";

                foreach (var item in lp)
                {
                    string it = temp;
                    it = it.Replace("{desc}", item.Legal_Desc);
                    it = it.Replace("{pro}", item.Proced);
                    result += it;
                }

                var res = new { htdata = result };
                return Json(res, JsonRequestBehavior.AllowGet); ;
            }


        }
        public ActionResult GetLoanHistory(string accID)
        {


            using (LoanDataDataContext database = new LoanDataDataContext())
            {
                // var loan = from d in database.Client_Loan_Infos select d;

                //This work sucess
                var lp = (from d in database.Tracking_Histories


                          where d.Account_ID == accID
                          select d);

                string result = string.Empty;
                string temp = @"<tr histitem='true'> <td>
                                         <span type='text' >{desc}</span>
                                     </td>
                                      <td>
                                         <span type='text' >{pro}</span>
                                     </td></tr>";

                foreach (var item in lp)
                {
                    string it = temp;
                    it = it.Replace("{desc}", item.Tacking_Date.Value.ToShortDateString());
                    it = it.Replace("{pro}", item.Tracking_Notes);
                    result += it;
                }

                var res = new { htdata = result };
                return Json(res, JsonRequestBehavior.AllowGet); ;
            }


        }

        [HttpPost]
        public ActionResult AddLegalNote(FormCollection collection)
        {
            try
            {

                using (LoanDataDataContext database = new LoanDataDataContext())
                {

                    long cID = 0;
                    long.TryParse(collection.Get("LegalNoteID"), out cID);

                    string legalNote = collection.Get("legalNote");




                    var LegalN = database.LegalNotes.FirstOrDefault(l => l.ID == cID);
                    if (LegalN != null)
                    {
                        LegalN.LegalNote1 = legalNote;
                        var track = (from t in database.Tracks where t.ID == LegalN.TrakID select t).FirstOrDefault();

                        if (track != null)
                        {
                            track.LegalReceivDate = DateTime.Now;
                            track.SendLegalState = 4;
                            track.Status = 4;
                            database.SubmitChanges();
                        }
                    }



                }

                var jsonObj = new { Message = "Legal Info Added" };
                return Json(jsonObj);
            }
            catch (Exception ex)
            {

                var jsonObj = new { Message = ex.Message };
                return Json(jsonObj);
            }


        }
        //
        // GET: /FollowUp/

        public ActionResult Index(string ClientId)
        {
            TempData["ClientId"] = ClientId;
            return View();
        }
        public ActionResult AddFollow(int id)
        {
            return PartialView("AddFollow");
        }
        public ActionResult View_History_Notes(string accID)
        {
            return PartialView("View_History_Notes");
        }
        public ActionResult GetDataToAddFollow(long TrackID)
        {

            List<Daily_Follow_Up> loan_res = new List<Daily_Follow_Up>();
            using (LoanDataDataContext database = new LoanDataDataContext())
            {
                // var loan = from d in database.Client_Loan_Infos select d;

                //This work sucess
                var loan = (from d in database.Tracks

                            join cl in database.Client_Loan_Infos on d.ClientLoanID equals cl.ID
                            join li in database.Loan_Infos on cl.LoanID equals li.ID
                            // join c in database.CLIENT_PERINFOs on cl.ClientID equals c.ID
                            join g in database.Guarantees_Infos on cl.GuaranteesID equals g.ID
                            where d.ID == TrackID
                            select new
                            {
                                ID = d.ID,
                                Client_Info = ClientManagementController.GetLastClientDataObj(cl.ID),
                                Client_Loan_Info = cl,
                                Loan_Info = li,
                                guarant = g,
                                tracks = d,
                                dat_down = li.Date_Downgrading==null?"":li.Date_Downgrading.Value.ToShortDateString()

                            }).FirstOrDefault();




                return Json(loan, JsonRequestBehavior.AllowGet);
            }


        }
        public ActionResult ViewGuruantees(string ACCID)
        {
            return PartialView("ViewGuruantees");
        }

        public ActionResult View_Legal_Notes(int id)
        {
            return PartialView("View_Legal_Notes");
        }


        public ActionResult GetLegalNote(string LegID)
        {


            using (LoanDataDataContext database = new LoanDataDataContext())
            {
                // var loan = from d in database.Client_Loan_Infos select d;

                //This work sucess
                var loan = (from d in database.Legal_Reports


                            where d.AccountID == LegID
                            select d).FirstOrDefault();




                return Json(loan, JsonRequestBehavior.AllowGet);
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
        [HttpPost]
        public ActionResult SendDataToLegal(FormCollection collection)
        {
            try
            {

                using (LoanDataDataContext database = new LoanDataDataContext())
                {
                    LegalNote upd = new LegalNote();
                    long cID = 0;
                    long.TryParse(collection.Get("TrakID"), out cID);
                    upd.TrakID = cID;
                    upd.EmpNotes = collection.Get("EmpNotes");
                    var papers = collection.Get("papers");

                    TemplateData[] res = JsonConvert.DeserializeObject<TemplateData[]>(papers);

                    //  upd.LegalID = long.Parse(collection.Get("legalID"));
                    upd.TrakerID = long.Parse(TGSession.ClientInfo.UID);
                    upd.Legal_Action = collection.Get("Legal_Action");
                    upd.SendingDate = DateTime.Now;
                    Track tr = database.Tracks.FirstOrDefault(t => t.ID == cID);
                    tr.SendLegalState = 2;

                    database.LegalNotes.InsertOnSubmit(upd);
                    database.SubmitChanges();
                    foreach (var item in res)
                    {
                        Legal_Paper paper = new Legal_Paper();
                        paper.LegalNoteID = upd.ID;
                        paper.paperName = item.text;
                        database.Legal_Papers.InsertOnSubmit(paper);
                        database.SubmitChanges();
                    }




                }

                var jsonObj = new { Message = "Legal Info Sended" };
                return Json(jsonObj);
            }
            catch (Exception ex)
            {

                var jsonObj = new { Message = ex.Message };
                return Json(jsonObj);
            }


        }
        [HttpPost]
        public ActionResult InsertFollowUp(FormCollection collection)
        {
            try
            {

                using (LoanDataDataContext database = new LoanDataDataContext())
                {
                    string accountid = collection.Get("ACCOUNTID");
                    var tracks = from tr in database.Client_Loan_Infos
                                 join cl in database.Tracks on tr.ID equals cl.ClientLoanID
                                 where tr.AccountID == accountid
                                 select cl;

                    foreach (var item in tracks)
                    {
                        Daily_Follow_Up followUp = new Daily_Follow_Up();
                        // long cID = 0;
                        //  long.TryParse(collection.Get("TrackID"), out cID);
                        // followUp.Tracks_ID = cID;
                        followUp.Tracks_ID = item.ID;
                        followUp.User_Follow = long.Parse(TGSession.ClientInfo.UID);
                        followUp.Follow_Date = DateTime.Now;
                        followUp.Follow_Action = int.Parse(collection.Get("Follow_Action"));
                        followUp.Follow_Note = collection.Get("Follow_Note");
                        string[] formats = { "dd/MM/yyyy" };
                        var dateTime = DateTime.ParseExact(collection.Get("Remainder_Date"), formats, new CultureInfo("en-US"), DateTimeStyles.None);
                        //  followUp.Remainder_Date = DateTime.Parse(collection.Get("Remainder_Date"));// DateTime.ParseExact(collection.Get("Remainder_Date"), "dd/MM/yyyy", null);// DateTime.Parse();
                        followUp.Remainder_Date = dateTime;
                        followUp.Remainder_Note = collection.Get("Remainder_Note");
                        //   track.Tracking_USER = long.Parse(collection.Get("Tracking_USER"));


                        database.Daily_Follow_Ups.InsertOnSubmit(followUp);
                        database.SubmitChanges();
                    }
                    //  Daily_Follow_Up followUp = new Daily_Follow_Up();
                    //  long cID = 0;
                    //  long.TryParse(collection.Get("TrackID"), out cID);
                    //  followUp.Tracks_ID = cID;
                    //  followUp.User_Follow = long.Parse(TGSession.ClientInfo.UID);
                    //  followUp.Follow_Date = DateTime.Now;
                    //  followUp.Follow_Action = int.Parse(collection.Get("Follow_Action"));
                    //  followUp.Follow_Note = collection.Get("Follow_Note");
                    //  string[] formats = { "dd/MM/yyyy" };
                    //  var dateTime = DateTime.ParseExact(collection.Get("Remainder_Date"), formats, new CultureInfo("en-US"), DateTimeStyles.None);
                    ////  followUp.Remainder_Date = DateTime.Parse(collection.Get("Remainder_Date"));// DateTime.ParseExact(collection.Get("Remainder_Date"), "dd/MM/yyyy", null);// DateTime.Parse();
                    //  followUp.Remainder_Date = dateTime;
                    //  followUp.Remainder_Note = collection.Get("Remainder_Note");
                    //  //   track.Tracking_USER = long.Parse(collection.Get("Tracking_USER"));


                    //  database.Daily_Follow_Ups.InsertOnSubmit(followUp);
                    //  database.SubmitChanges();



                }

                var jsonObj = new { Message = "Follow Up inserted suc" };
                return Json(jsonObj);
            }
            catch (Exception ex)
            {

                var jsonObj = new { Error = ex.Message };
                return Json(jsonObj);
            }

        }

        [HttpPost]
        public ActionResult InsertSchedule(FormCollection collection)
        {
            try
            {

                using (LoanDataDataContext database = new LoanDataDataContext())
                {

                    //long cID = 0;
                    //long.TryParse(collection.Get("CLID"), out cID);
                    //Client_Loan_Info cl = database.Client_Loan_Infos.FirstOrDefault(c=>c.ID==cID);
                    string accountID = collection.Get("ACCOUNTID");
                    var client_loans = from cl in database.Client_Loan_Infos where cl.AccountID == accountID select cl;
                    string schedule = collection.Get("Scheduled");
                    if (!string.IsNullOrEmpty(schedule))
                    {

                        foreach (var item in client_loans)
                        {


                            Client_Loan_Info cl = database.Client_Loan_Infos.FirstOrDefault(c => c.ID == item.ID);
                            cl.Scheduled = schedule.ToLower() == "true" ? true : false;
                            cl.Down_Payment = double.Parse(collection.Get("Down_Payment"));
                            //Reschedule_Amount     Monthly_Payment    Duration     Schedule_Date
                            if (!string.IsNullOrEmpty(collection.Get("Reschedule_Amount")))
                            {
                                cl.Reschedule_Amount = double.Parse(collection.Get("Reschedule_Amount"));

                            }
                            if (!string.IsNullOrEmpty(collection.Get("Monthly_Payment")))
                            {
                                cl.Monthly_Payment = double.Parse(collection.Get("Monthly_Payment"));
                            }

                            cl.Duration = collection.Get("Duration");

                            if (!string.IsNullOrEmpty(collection.Get("Schedule_Date")))
                            {
                                cl.Schedule_Date = DateTime.Parse(collection.Get("Schedule_Date"));
                            }


                            database.SubmitChanges();




                        }
                    }

                    var res = new { LData = client_loans.ToList().FirstOrDefault(), Message = "Scheduled inserted" };
                    return Json(res);
                    //if (cl!=null)
                    //{
                    //    string schedule = collection.Get("Scheduled");
                    //    if (!string.IsNullOrEmpty(schedule))
                    //    {
                    //       cl.Scheduled  = schedule == "true" ? true : false;
                    //       cl.Down_Payment = double.Parse(collection.Get("Down_Payment"));
                    //       database.SubmitChanges();
                    //       var res = new { LData = cl, Message = "Scheduled inserted suc" };
                    //       return Json(res);
                    //    }


                    //}

                    //   track.Tracking_USER = long.Parse(collection.Get("Tracking_USER"));







                }



            }
            catch (Exception ex)
            {

                var jsonObj = new { Error = ex.Message };
                return Json(jsonObj);
            }

        }


        [HttpPost]
        public ActionResult CloseLoan(FormCollection collection)
        {
            try
            {

                using (LoanDataDataContext database = new LoanDataDataContext())
                {

                    long cID = 0;
                    long.TryParse(collection.Get("TrackID"), out cID);
                    Track cl = database.Tracks.FirstOrDefault(c => c.ID == cID);
                    if (cl != null)
                    {
                        cl.Status = 5;
                        database.SubmitChanges();

                    }

                    //   track.Tracking_USER = long.Parse(collection.Get("Tracking_USER"));



                    var noaction = new { Message = "OK" };
                    return Json(noaction);



                }



            }
            catch (Exception ex)
            {

                var jsonObj = new { Error = ex.Message };
                return Json(jsonObj);
            }

        }



        [HttpGet]
        public ActionResult getFollowAction(string searchTerm)
        {
            using (LoanDataDataContext udatabase = new LoanDataDataContext())
            {
                var res = from r in udatabase.Actions where r.Name.Contains(searchTerm) && r.ActionType == 2 select new { id = r.ID, text = r.Name };

                return Json(res.ToList(), JsonRequestBehavior.AllowGet);
            }

        }


        public ActionResult Download(string file)
        {
            if (!System.IO.File.Exists(file))
            {
                return HttpNotFound();
            }
            string fn = Path.GetFileName(file).Split('.')[0];
            string extension = Path.GetExtension(file);
            var fileBytes = System.IO.File.ReadAllBytes(file);
            var response = new FileContentResult(fileBytes, "application/octet-stream")
            {
                FileDownloadName = fn + extension
            };
            return response;
        }
        public ActionResult GetInitialGuruantees(string ACCID)
        {


            using (LoanDataDataContext database = new LoanDataDataContext())
            {
                // var loan = from d in database.Client_Loan_Infos select d;

                //This work sucess
                var lp = (from d in database.Guarantees_Infos


                          where d.AccountID == ACCID
                          select d);

                string result = string.Empty;
                string temp = @"<tr guranrowitem='true'>
                                       <td>
                                         <span type='text' >{Gurantee_Type}</span>
                                     </td>
                                      <td>
                                         <span type='text' >{Applicant_Name}</span>
                                     </td>
                                      <td>
                                         <span type='text'>{Applicant_Account}</span>
                                     </td>
                                     <td>
                                         <span type='text'  >{Estimated_Amount}</span>
                                     </td>
                                     <td>
                                         <span type='text'  >{Estimate_date}</span>
                                     </td>
                                       <td>
                                         <span type='text' >{Initial_Amount}</span>
                                     </td>
                                       <td>
                                         <span type='text'  >{Gurantee_Description}</span>
                                     </td>
                                 </tr>";


                foreach (var item in lp.Take(2))
                {
                    string it = temp;
                    it = it.Replace("{Gurantee_Type}", item.Gurantee_Type);
                    it = it.Replace("{Applicant_Name}", item.Applicant_Name);
                    it = it.Replace("{Applicant_Account}", item.Applicant_Account);
                    it = it.Replace("{Estimated_Amount}", item.Estimated_Amount);
                    it = it.Replace("{Estimate_date}", item.Estimate_date.Value.ToShortDateString());
                    it = it.Replace("{Initial_Amount}", item.Initial_Amount);
                    it = it.Replace("{Gurantee_Description}", item.Gurantee_Description);
                    result += it;
                }
                if (lp.ToList().Count > 2)
                {
                    result += @"<tr guranrowitem='true'> 
                                      <td>
                                        <a AccID='{ACCID}' href='#' onclick='ViewGurunteesData(this); return false; '>View More</a>
                                     </td></tr>";
                    result = result.Replace("{ACCID}", ACCID);
                }

                var res = new { htdata = result };
                return Json(res, JsonRequestBehavior.AllowGet); ;
            }


        }


        //Whole Guruantees
        public ActionResult GetGuruantees(string ACCID)
        {


            using (LoanDataDataContext database = new LoanDataDataContext())
            {
                // var loan = from d in database.Client_Loan_Infos select d;

                //This work sucess
                var lp = (from d in database.Guarantees_Infos


                          where d.AccountID == ACCID
                          select d);

                string result = string.Empty;
                string temp = @"<tr guranrowitem='true'>
                                       <td>
                                         <span type='text' >{Gurantee_Type}</span>
                                     </td>
                                      <td>
                                         <span type='text' >{Applicant_Name}</span>
                                     </td>
                                      <td>
                                         <span type='text'>{Applicant_Account}</span>
                                     </td>
                                     <td>
                                         <span type='text'  >{Estimated_Amount}</span>
                                     </td>
                                     <td>
                                         <span type='text'  >{Estimate_date}</span>
                                     </td>
                                       <td>
                                         <span type='text' >{Initial_Amount}</span>
                                     </td>
                                       <td>
                                         <span type='text'  >{Gurantee_Description}</span>
                                     </td>
                                 </tr>";


                foreach (var item in lp)
                {
                    string it = temp;
                    it = it.Replace("{Gurantee_Type}", item.Gurantee_Type);
                    it = it.Replace("{Applicant_Name}", item.Applicant_Name);
                    it = it.Replace("{Applicant_Account}", item.Applicant_Account);
                    it = it.Replace("{Estimated_Amount}", item.Estimated_Amount);
                    it = it.Replace("{Estimate_date}", item.Estimate_date.Value.ToShortDateString());
                    it = it.Replace("{Initial_Amount}", item.Initial_Amount);
                    it = it.Replace("{Gurantee_Description}", item.Gurantee_Description);
                    result += it;
                }


                if (lp.ToList().Count > 2)
                {
                    result += @"<tr gurancontrol='true'> 
                                      <td>
                                        <a id='seeMoreRecords' onclick='showmore(); return false;'  href='#' >View More</a>
                                         <a id='seeLessRecords'  onclick='showLess(); return false;' href='#' >View Less</a>
                                     </td>
                                    </tr>";
                    // result = result.Replace("{ACCID}", ACCID);
                }



                var res = new { htdata = result };
                return Json(res, JsonRequestBehavior.AllowGet); ;
            }


        }


        //public static LambdaExpression CreateLambdaExpression(string propertyName, object value, string operation, Type type)
        //{
        //    ParameterExpression parameter = Expression.Parameter(type, "x");
        //    Expression property = Expression.Property(parameter, type.GetProperty(propertyName));
        //    Expression target = Expression.Constant(value);
        //    Expression equalsMethod = Expression.Call(property, operation, null, target);

        //    return Expression.Lambda(equalsMethod, parameter);
        //}




        public ActionResult SendToLegal(int id)
        {
            return PartialView("SendToLegal");
        }


    }
}
