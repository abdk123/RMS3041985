using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoanManagement.Models;
namespace LoanManagement.Controllers
{
   

    public class ReminderController : TemplateController
    {
        public ActionResult GetDataToGrid(string sidx, string sord, int page, int rows, string filters)
        {

           
            using (LoanDataDataContext database = new LoanDataDataContext())
            {
                // var loan = from d in database.Client_Loan_Infos select d;
                long uid =  long.Parse(TGSession.ClientInfo.UID);

                

                //This work sucess
                var loan = (from d in database.Daily_Follow_Ups

                              join t in database.Tracks on d.Tracks_ID equals t.ID
                              join ci in database.Client_Loan_Infos on t.ClientLoanID equals ci.ID
                              join l in database.Loan_Infos on ci.LoanID equals l.ID
                              join c in database.CLIENT_PERINFOs on ci.ID equals c.ID
                              join tr in database.Trackers on t.ID equals tr.TracksID
                            where t.CurrTraker == uid && ((d.Remainder_Date.Value.DayOfYear == DateTime.Now.Date.DayOfYear || d.Remainder_Date.Value.DayOfYear > DateTime.Now.Date.DayOfYear) && d.Remainder_Date.Value.Year == DateTime.Now.Date.Year)
                              select new
                              {
                                  ID = t.ID,
                                  Client_Info = c,
                                  Follow_Ups = d,
                                  track = t,
                                  loan_info = ci,
                                  loan=l,
                                  tracker=tr,
                                  leg_state=t.SendLegalState.HasValue ? t.SendLegalState.Value : 0
                              }).GroupBy(r=>r.ID).Select(g=>g.First()).ToList();


                string accountId = Convert.ToString(TempData["accountId"]);
                if (!String.IsNullOrEmpty(accountId))
                    loan = loan.Where(x => x.loan_info.AccountID == accountId).ToList();

  //              .GroupBy(p => p.PersonId)
  //.Select(g => g.First())
  //.ToList();
                // depts.AddRange(dep.ToList());
                if (!string.IsNullOrEmpty(filters))
                {
                    var search = filters.Split('&');
                    if (!string.IsNullOrEmpty(search[0]))
                    {
                        var temp = from l in loan where l.Client_Info.AccountID == search[0] select l;
                        //loan = loan.Select(l=>l.Client_Acc==AccID).ToList();
                        loan = temp.ToList();
                    }
                    if (!string.IsNullOrEmpty(search[1]))
                    {
                        var temp = from l in loan where l.Client_Info.FULLNAME.Contains(search[1]) select l;
                        //loan = loan.Select(l=>l.Client_Acc==AccID).ToList();
                        loan = temp.ToList();
                    }
                }

                
                int pageIndex = Convert.ToInt32(page) - 1;
                int pageSize = rows;

                int totalRecords = loan.Count();




                var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
                loan = loan.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                //if (sord.ToUpper() == "DESC")
                //{
                //    loan_res = loan.OrderByDescending(s => s.ID).ToList();
                //    loan_res = loan.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                //}
                //else
                //{
                //    loan_res = loan.OrderBy(s => s.ID).ToList();
                //    loan_res = loan.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                //}
                //  processResults =SortIQueryable(processResults,sidx, sord);



                var jsonData = new
                {
                    total = totalPages,
                    page,
                    records = totalRecords,
                    rows = loan
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }


        }
        public ActionResult GetDataToGridAddFollow(string sidx, string sord, int page, int rows, string filters)
        {

           
            using (LoanDataDataContext database = new LoanDataDataContext())
            {
                // var loan = from d in database.Client_Loan_Infos select d;

                //This work sucess
                long tid=0;
                if (!string.IsNullOrEmpty(filters))
                {
                    tid = long.Parse(filters);
                }
                var follow = (from f in database.Daily_Follow_Ups
                             where f.Tracks_ID == tid
                             select f).ToList();


               

                int pageIndex = Convert.ToInt32(page) - 1;
                int pageSize = rows;

                int totalRecords = follow.Count();




                var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

               

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
        //
        // GET: /FollowUp/

        public ActionResult Index(string accountID)
        {
            TempData["accountId"] = accountID;
            return View();
        }
        public ActionResult AddFollow(int id)
        {
            return PartialView("AddFollow");
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

                              
                            }).FirstOrDefault();




                return Json(loan, JsonRequestBehavior.AllowGet);
            }


        }
        [HttpPost]
        public ActionResult InsertFollowUp(FormCollection collection)
        {
            try
            {

                using (LoanDataDataContext database = new LoanDataDataContext())
                {
                    Daily_Follow_Up followUp = new Daily_Follow_Up();
                    long cID = 0;
                    long.TryParse(collection.Get("TrackID"), out cID);
                    followUp.Tracks_ID = cID;
                    followUp.User_Follow = 1;
                    followUp.Follow_Date = DateTime.Now;
                    followUp.Follow_Action = int.Parse(collection.Get("Follow_Action"));
                    followUp.Follow_Note = collection.Get("Follow_Note");
                    followUp.Remainder_Date = DateTime.Parse(collection.Get("Remainder_Date"));// DateTime.ParseExact(collection.Get("Remainder_Date"), "dd/MM/yyyy", null);// DateTime.Parse();
                    followUp.Remainder_Note = collection.Get("Remainder_Note");
                    //   track.Tracking_USER = long.Parse(collection.Get("Tracking_USER"));


                    database.Daily_Follow_Ups.InsertOnSubmit(followUp);
                    database.SubmitChanges();

                 

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
        [HttpGet]
        public ActionResult getFollowAction(string searchTerm)
        {
            using (LoanDataDataContext udatabase = new LoanDataDataContext())
            {
                var res = from r in udatabase.Actions where r.Name.Contains(searchTerm) && r.ActionType == 2 select new { id = r.ID, text = r.Name };

                return Json(res.ToList(), JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult SendToLegal(int id)
        {
            return PartialView("SendToLegal");
        }
        

    }
}
