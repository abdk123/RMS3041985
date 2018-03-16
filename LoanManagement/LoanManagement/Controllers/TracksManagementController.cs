using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoanManagement.Models;
using System.Collections;

namespace LoanManagement.Controllers
{
    public class TracksManagementController : TemplateController
    {

        public ActionResult GetDataToGrid(string sidx, string sord, int page, int rows, string filters)
        {

            List<Client_Loan_Info> loan_res = new List<Client_Loan_Info>();
            using (LoanDataDataContext database = new LoanDataDataContext())
            {
                // var loan = from d in database.Client_Loan_Infos select d;
                //ArrayList lst = new ArrayList();
                //foreach (var item in database.Client_Loan_Infos)
                //{
                //    if (database.Tracks.FirstOrDefault(c => c.ClientLoanID == item.ID) != null)
                //    {
                //        var i = new
                //        {
                //            ID = item.ID,
                //            Client_Acc = database.Loan_Infos.FirstOrDefault(de => de.ID == item.LoanID).AccountID,//OK
                //           Client_Name = database.CLIENT_PERINFOs.FirstOrDefault(cin => cin.ID == item.ClientID).FULLNAME,//OK
                //            ////  Status = database.Tracks.FirstOrDefault(t => t.ClientLoanID == item.ID) != null ? database.Status.FirstOrDefault(s => s.ID == database.Tracks.FirstOrDefault(t => t.ClientLoanID == item.ID).Status).Name : "No Status",
                //           // Branch = (database.Loan_Infos.FirstOrDefault(c => c.ID == item.LoanID).Branch != null) ? database.Loan_Infos.FirstOrDefault(c => c.ID == item.ID).Branch : "Not specified",//Here Error
                //            Tracking_Date = (database.Tracks.Where(c => c.ClientLoanID == item.ID).ToList().Count > 0) ? database.Tracks.Where(c => c.ClientLoanID == item.ID).OrderByDescending(r => r.Tracking_Date.Value).FirstOrDefault().Tracking_Date : null,//Here OK
                //            StatusID = database.Tracks.FirstOrDefault(t => t.ClientLoanID == item.ID) != null ? database.Status.FirstOrDefault(s => s.ID == database.Tracks.FirstOrDefault(t => t.ClientLoanID == item.ID).Status).ID : 1,//OK
                //            RO = item.RO,//Here OK
                //            //  Trackers = database.Trackers.Where(t => t.TracksID == database.Tracks.FirstOrDefault(r => r.ClientLoanID == item.ID).ID).OrderByDescending(t => t.ID).FirstOrDefault().Trackers//Error 
                //            //  Last_Tracking = d.NOTE
                //        };
                //        lst.Add(i);
                //    }

                //}

                //End Test


                //This work sucess
                var loan = (from d in database.Client_Loan_Infos
                            select new
                            {
                                ID = d.ID,
                                Client_Acc = database.Loan_Infos.FirstOrDefault(de => de.ID == d.LoanID).AccountID,
                                Client_Name = database.CLIENT_PERINFOs.FirstOrDefault(cin => cin.ID == d.ClientID).FULLNAME,
                                Status = database.Tracks.FirstOrDefault(t => t.ClientLoanID == d.ID) != null ? database.Status.FirstOrDefault(s => s.ID == database.Tracks.FirstOrDefault(t => t.ClientLoanID == d.ID).Status).Name : "Not Assigned",
                                Branch = (database.Loan_Infos.FirstOrDefault(c => c.ID == d.LoanID).Branch != null) ? database.Loan_Infos.FirstOrDefault(c => c.ID == d.LoanID).Branch : "Not specified",
                                Tracking_Date = (database.Tracks.Where(c => c.ClientLoanID == d.ID).ToList().Count > 0) ? database.Tracks.Where(c => c.ClientLoanID == d.ID).OrderByDescending(r => r.Tracking_Date.Value).FirstOrDefault().Tracking_Date : null,
                                StatusID = database.Tracks.FirstOrDefault(t => t.ClientLoanID == d.ID) != null ? database.Status.FirstOrDefault(s => s.ID == database.Tracks.FirstOrDefault(t => t.ClientLoanID == d.ID).Status).ID : 1,
                                RO = database.Tracks.FirstOrDefault(t => t.ClientLoanID == d.ID) != null ? database.Sys_Users.FirstOrDefault(s => s.id == database.Tracks.FirstOrDefault(t => t.ClientLoanID == d.ID).CurrTraker).RO_Code : "No Trackers",
                                //    Trackers=database.Trackers.Where(t=>t.TracksID==database.Tracks.FirstOrDefault(r=>r.ClientLoanID==d.ID).ID).OrderByDescending(t=>t.ID).FirstOrDefault().Trackers
                                //  Last_Tracking = d.NOTE
                            }).ToList();


                // depts.AddRange(dep.ToList());
                if (!string.IsNullOrEmpty(filters))
                {
                    var search = filters.Split('&');
                    if (!string.IsNullOrEmpty(search[0]))
                    {
                        var temp = from l in loan where l.Client_Acc == search[0] select l;
                        //loan = loan.Select(l=>l.Client_Acc==AccID).ToList();
                        loan = temp.ToList();
                    }
                    if (!string.IsNullOrEmpty(search[1]))
                    {
                        var temp = from l in loan where l.Client_Name.Contains(search[1]) select l;
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


        public ActionResult GetClientLoanInfoByID(long tID)
        {
            using (LoanDataDataContext database = new LoanDataDataContext())
            {
                var track = (from d in database.Client_Loan_Infos
                             where d.ID == tID
                             select new
                             {
                                 ID = d.ID,
                                 Client_Acc = database.Loan_Infos.FirstOrDefault(de => de.ID == d.LoanID).AccountID,
                                 Client_Name = database.CLIENT_PERINFOs.FirstOrDefault(cin => cin.ID == d.ClientID).FULLNAME,
                                 Status = database.Tracks.FirstOrDefault(t => t.ClientLoanID == d.ID).Status.HasValue ? database.Status.FirstOrDefault(s => s.ID == database.Tracks.FirstOrDefault(t => t.ClientLoanID == d.ID).Status).Name : "Not Pending",
                                 StatusID = database.Tracks.FirstOrDefault(t => t.ClientLoanID == d.ID).Status.HasValue ? database.Status.FirstOrDefault(s => s.ID == database.Tracks.FirstOrDefault(t => t.ClientLoanID == d.ID).Status).ID : 1,
                                 Branch = (database.Loan_Infos.FirstOrDefault(c => c.ID == d.LoanID).Branch != null) ? database.Loan_Infos.FirstOrDefault(c => c.ID == d.ID).Branch : "Not specified",
                                 Last_Tracking = (database.Tracks.Where(c => c.ID == d.ID).ToList().Count > 0) ? database.Tracks.Where(c => c.ID == d.ID).OrderByDescending(r => r.Tracking_Date).FirstOrDefault().Tracking_Date : null,
                                 Pending_Tracks = (database.Tracks.Where(c => c.ID == d.ID).ToList().Count > 0) ? "Pending Tracks" : "Not Tracked",
                                 RO = d.RO,
                                 //  Trackers = database.Tracks.Where(t => t.ClientLoanID == d.ID).OrderByDescending(t => t.ID).FirstOrDefault().Tracking_USER
                                 //  Last_Tracking = d.NOTE
                             }).FirstOrDefault();
                return Json(track);
            }


        }

        public ActionResult GetTrackByLoanID(long tID)
        {
            using (LoanDataDataContext database = new LoanDataDataContext())
            {
                var track = (from d in database.Tracks
                             join l in database.Client_Loan_Infos on d.ClientLoanID equals l.ID
                             where d.ClientLoanID == tID
                             select new
                             {
                                 ID = d.ID,
                                 Client_Acc = database.Loan_Infos.FirstOrDefault(de => de.ID == l.LoanID).AccountID,
                                 Client_Name = database.CLIENT_PERINFOs.FirstOrDefault(cin => cin.ID == l.ClientID).FULLNAME,
                                 Status = d.Status.HasValue ? database.Status.FirstOrDefault(s => s.ID == d.Status).Name : "No Status",
                                 Branch = (database.Loan_Infos.FirstOrDefault(c => c.ID == l.LoanID).Branch != null) ? database.Loan_Infos.FirstOrDefault(c => c.ID == d.ID).Branch : "Not specified",
                                 Last_Tracking = (database.Tracks.Where(c => c.ID == tID).ToList().Count > 0) ? database.Tracks.Where(c => c.ID == d.ID).OrderByDescending(r => r.Tracking_Date).FirstOrDefault().Tracking_Date : null,
                                 Pending_Tracks = (database.Tracks.Where(c => c.ID == d.ID).ToList().Count > 0) ? "Pending Tracks" : "Not Tracked",
                                 RO = l.RO,
                                 TrackData = d
                                 //Trackers = database.Tracks.Where(t => t.ClientLoanID == d.ID).OrderByDescending(t => t.ID).FirstOrDefault().Tracking_USER
                                 //    Last_Tracking = d.NOTE
                             }).FirstOrDefault();
                return Json(track);
            }


        }


        [HttpGet]
        public ActionResult getTrackAction(string searchTerm)
        {
            using (LoanDataDataContext udatabase = new LoanDataDataContext())
            {
                var res = from r in udatabase.Actions where r.Name.Contains(searchTerm) && r.ActionType == 1 select new { id = r.ID, text = r.Name };

                return Json(res.ToList(), JsonRequestBehavior.AllowGet);
            }

        }

        //
        // GET: /TracksManagement/

        public ActionResult Index()
        {

            return View();
        }
        public ActionResult AddTrack(int id)
        {

            return PartialView("AddTrack");

        }
        public ActionResult UpdateTrack(int id)
        {

            return PartialView("UpdateTrack");

        }
        public ActionResult History(long id)
        {

            return PartialView("History");

        }
        [HttpPost]
        public ActionResult InsertTrack(FormCollection collection)
        {
            try
            {

                using (LoanDataDataContext database = new LoanDataDataContext())
                {
                    Track track = new Track();
                    long cID = 0;
                    long.TryParse(collection.Get("ClientLoanID"), out cID);
                    track.ClientLoanID = cID;
                    track.Tracking_Action = int.Parse(collection.Get("Tracking_Action"));
                    track.Tracking_Date = DateTime.Now;
                    //   track.Tracking_USER = long.Parse(collection.Get("Tracking_USER"));
                    track.Tracking_Details = collection.Get("Tracking_Details");
                    //  track.Admin_Declare = long.Parse(collection.Get("Admin_Declare"));
                    track.Status = 2;
                    track.CurrTraker = long.Parse(collection.Get("Tracking_USER")); ;
                    database.Tracks.InsertOnSubmit(track);
                    database.SubmitChanges();

                    Tracker tracker = new Tracker();
                    tracker.TracksID = track.ID;
                    tracker.Declared_Date = DateTime.Now;
                    tracker.Admin_Notes = "";
                    tracker.Admin_Declare = long.Parse(TGSession.ClientInfo.UID);
                    tracker.Trackers = long.Parse(collection.Get("Tracking_USER"));
                    database.Trackers.InsertOnSubmit(tracker);
                    database.SubmitChanges();

                }

                var jsonObj = new { Message = "Track inserted suc" };
                return Json(jsonObj);
            }
            catch (Exception ex)
            {

                var jsonObj = new { Error = ex.Message };
                return Json(jsonObj);
            }

        }
        [HttpPost]
        public ActionResult UpdateUserTracker(FormCollection collection)
        {
            try
            {

                using (LoanDataDataContext database = new LoanDataDataContext())
                {
                    long cID = 0;
                    long.TryParse(collection.Get("TrackID"), out cID);
                    Track track = database.Tracks.FirstOrDefault(t => t.ID == cID);
                    track.CurrTraker = long.Parse(collection.Get("Tracking_USER"));
                    Tracker tracker = new Tracker();
                    tracker.TracksID = track.ID;
                    tracker.Declared_Date = DateTime.Now;
                    tracker.Admin_Notes = collection.Get("Admin_Notes");
                    tracker.Admin_Declare = long.Parse(TGSession.ClientInfo.UID);
                    tracker.Trackers = long.Parse(collection.Get("Tracking_USER"));
                    database.Trackers.InsertOnSubmit(tracker);
                    database.SubmitChanges();

                    var clientLoanInfos = database.Client_Loan_Infos.Where(x => x.ID == track.ClientLoanID).ToList();
                    foreach (var clientLoanInfo in clientLoanInfos)
                    {
                        clientLoanInfo.Read_Note = false;
                        database.SubmitChanges();
                    }

                }

                var jsonObj = new { Message = "Tracker Updated" };
                return Json(jsonObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                var jsonObj = new { Error = ex.Message };
                return Json(jsonObj, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult GetDataToHistory(long TrackID)
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
                            where cl.ID == TrackID
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

                var res = new { htdata = result };
                return Json(res, JsonRequestBehavior.AllowGet); ;
            }


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

                if (loan != null)
                {
                    return Json(loan, JsonRequestBehavior.AllowGet);
                }

                return Json(new { Res = "Empty" }, JsonRequestBehavior.AllowGet);

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

        public ActionResult AssignLoans()
        {
            return View();
        }

        public ActionResult GetDataToAssignGrid(string sidx, string sord, int page, int rows, string filters)
        {
            int id = 0;
            if (int.TryParse(filters, out id))
            {
                List<Client_Loan_Info> loan_res = new List<Client_Loan_Info>();
                using (LoanDataDataContext database = new LoanDataDataContext())
                {
                    var selectedRo = database.Sys_Users.FirstOrDefault(x => x.id == id);
                    var allLoans = (from d in database.Client_Loan_Infos

                                    select new
                                    {
                                        ID = d.ID,
                                        Client_Acc = database.Loan_Infos.FirstOrDefault(de => de.ID == d.LoanID).AccountID,
                                        Client_Name = database.CLIENT_PERINFOs.FirstOrDefault(cin => cin.ID == d.ClientID).FULLNAME,
                                        Status = database.Tracks.FirstOrDefault(t => t.ClientLoanID == d.ID) != null ? database.Status.FirstOrDefault(s => s.ID == database.Tracks.FirstOrDefault(t => t.ClientLoanID == d.ID).Status).Name : "Not Assigned",
                                        Track_ID = database.Tracks.FirstOrDefault(t => t.ClientLoanID == d.ID) != null ? database.Tracks.FirstOrDefault(t => t.ClientLoanID == d.ID).ID : 0,
                                        Branch = (database.Loan_Infos.FirstOrDefault(c => c.ID == d.LoanID).Branch != null) ? database.Loan_Infos.FirstOrDefault(c => c.ID == d.LoanID).Branch : "Not specified",
                                        Tracking_Date = (database.Tracks.Where(c => c.ClientLoanID == d.ID).ToList().Count > 0) ? database.Tracks.Where(c => c.ClientLoanID == d.ID).OrderByDescending(r => r.Tracking_Date.Value).FirstOrDefault().Tracking_Date : null,
                                        StatusID = database.Tracks.FirstOrDefault(t => t.ClientLoanID == d.ID) != null ? database.Status.FirstOrDefault(s => s.ID == database.Tracks.FirstOrDefault(t => t.ClientLoanID == d.ID).Status).ID : 1,
                                        RO = database.Tracks.FirstOrDefault(t => t.ClientLoanID == d.ID) != null ? database.Sys_Users.FirstOrDefault(s => s.id == database.Tracks.FirstOrDefault(t => t.ClientLoanID == d.ID).CurrTraker).RO_Code : "No Trackers",

                                    });

                    var loan = allLoans.ToList().Where(x => x.RO == selectedRo.RO_Code).ToList();


                    int pageIndex = Convert.ToInt32(page) - 1;
                    int pageSize = rows;

                    int totalRecords = loan.Count();




                    var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
                    loan = loan.Skip(pageIndex * pageSize).Take(pageSize).ToList();

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

            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult AssignToNewRo(string TrackIDs, int roId)
        {
            string[] arr = TrackIDs.Split(',');

            try
            {
                foreach (var id in arr)
                {
                    using (LoanDataDataContext database = new LoanDataDataContext())
                    {
                        long cID = 0;
                        long.TryParse(id, out cID);
                        if (cID != 0)
                        {
                            Track track = database.Tracks.FirstOrDefault(t => t.ID == cID);
                            track.CurrTraker = roId;
                            Tracker tracker = new Tracker();
                            tracker.TracksID = track.ID;
                            tracker.Declared_Date = DateTime.Now;
                            tracker.Admin_Notes = "";
                            tracker.Admin_Declare = long.Parse(TGSession.ClientInfo.UID);
                            tracker.Trackers = roId;
                            database.Trackers.InsertOnSubmit(tracker);
                            database.SubmitChanges();

                            var clientLoanInfos = database.Client_Loan_Infos.Where(x => x.ID == track.ClientLoanID).ToList();
                            foreach (var clientLoanInfo in clientLoanInfos)
                            {
                                clientLoanInfo.Read_Note = false;
                                database.SubmitChanges();
                            }
                        }


                    }


                }
                var jsonObj = new { Message = "Success" };
                return Json(jsonObj, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                var jsonObj = new { Error = ex.Message };
                return Json(jsonObj, JsonRequestBehavior.AllowGet);
            }

        }
    }
}
