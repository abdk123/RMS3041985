using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoanManagement.Models;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
namespace LoanManagement.Controllers
{
    public class LegalController : TemplateController
    {

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
                    upd.SendingDate = DateTime.Now;
                      var papers = collection.Get("papers");

                      TemplateData[] res = JsonConvert.DeserializeObject<TemplateData[]>(papers);
                     
                  //  upd.LegalID = long.Parse(collection.Get("legalID"));
                    upd.TrakerID = long.Parse(TGSession.ClientInfo.UID);
                    upd.Legal_Action = collection.Get("Legal_Action");
                    Track tr = database.Tracks.FirstOrDefault(t=>t.ID==cID);
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
        //Here to Get Lagal Notes
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

                var res=new {htdata=result};
                return Json(res, JsonRequestBehavior.AllowGet);;
            }


        }



        //
        public ActionResult GetDataToGrid(string sidx, string sord, int page, int rows, string filters)
        {


            using (LoanDataDataContext database = new LoanDataDataContext())
            {
                // var loan = from d in database.Client_Loan_Infos select d;
                long uid = long.Parse(TGSession.ClientInfo.UID);

                //This work sucess
                var loan = (from d in database.LegalNotes
                            join ro in database.Sys_Users on d.TrakerID equals ro.id
                            join cl in database.Tracks on d.TrakID equals cl.ID
                           join c in database.Client_Loan_Infos on cl.ClientLoanID equals c.ID
                           join client in database.CLIENT_PERINFOs on c.ClientID equals client.ID
                          // where d.LegalID==uid
                            select new
                            {
                                ID = d.ID,
                                Note=d.EmpNotes,
                                Track = cl,
                                Acc_ID = c.AccountID,
                                Client_Name=client.FULLNAME,
                               RO_Name=ro.RO_Name,
                               track_status=cl.Status,
                               legal_state=Util.GetLegalStatus(cl.SendLegalState),
                               receiv_date=cl.LegalReceivDate,
                                SendingDate=d.SendingDate,
                                Legal_Action=d.Legal_Action
                                //  Trackers=database.Trackers.Where(t=>t.TracksID==d.ID).OrderByDescending(t=>t.ID).FirstOrDefault().Tracking_USER
                                //  Last_Tracking = d.NOTE
                            }).ToList();


                if (!string.IsNullOrEmpty(filters))
                {
                    var search = filters.Split('&');
                    if (!string.IsNullOrEmpty(search[0]))
                    {
                        var temp = from l in loan where l.Acc_ID == search[0] select l;
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
        public ActionResult Receive(int id)
        {
            return PartialView("Receive");
        }
        public ActionResult View_Legal_Notes(int id)
        {
            return PartialView("View_Legal_Notes");
        }

        public ActionResult Add_Legal_Notes(int id)
        {
            return PartialView("Add_Legal_Notes");
        }

        public ActionResult GetSendingPapersToLegal(long noteID)
        {


            using (LoanDataDataContext database = new LoanDataDataContext())
            {
               
                var legal_note = (from d in database.Legal_Papers

                           where d.LegalNoteID==noteID
                            select new
                            {
                               id=d.ID,
                               text=d.paperName


                                //  Trackers=database.Trackers.Where(t=>t.TracksID==d.ID).OrderByDescending(t=>t.ID).FirstOrDefault().Tracking_USER
                                //  Last_Tracking = d.NOTE
                            }).ToList();






                return Json(legal_note);
            }


        }

        [HttpPost]
        public ActionResult ReceivePapers(FormCollection collection)
        {
            try
            {

                using (LoanDataDataContext database = new LoanDataDataContext())
                {

                    long uid = long.Parse(TGSession.ClientInfo.UID);
                    long cID = 0;
                    long.TryParse(collection.Get("LegalNoteID"), out cID);
                   
                    var papers = collection.Get("papers");
                   // var legalID = collection.Get("papers");
                    TemplateData[] res = JsonConvert.DeserializeObject<TemplateData[]>(papers);
                    var papersRec = from p in database.Legal_Papers where p.LegalNoteID == cID select p;


                    foreach (var item in papersRec)
                    {

                        var pap = from r in res where long.Parse(r.id) == item.ID select r;
                        if (pap.ToList().Count > 0)
                        {
                            Legal_Paper updPaper = database.Legal_Papers.FirstOrDefault(p => p.ID == item.ID);
                            updPaper.IsRecived = true;
                            database.SubmitChanges();
                        }

                    }
                    var legalNote = database.LegalNotes.FirstOrDefault(l=>l.ID==cID);
                    legalNote.LegalID = uid;
                    var track = (from t in database.Tracks where t.ID == legalNote.TrakID select t).FirstOrDefault();

                    if (track!=null)
                    {
                        track.LegalReceivDate = DateTime.Now;
                        track.SendLegalState = 3;
                        track.Status = 3;
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
        public ActionResult AddLegalNote(FormCollection collection)
        {
            try
            {

                using (LoanDataDataContext database = new LoanDataDataContext())
                {
                   
                    long cID = 0;
                    long.TryParse(collection.Get("LegalNoteID"), out cID);

                    string legalNote = collection.Get("legalNote");

                  

                   
                    var LegalN = database.LegalNotes.FirstOrDefault(l=>l.ID==cID);
                    if (LegalN!=null)
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
        // GET: /Legal/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Cases()
        {
            return View();
        }

        public ActionResult view_procedure(int id)
        {
            return PartialView("view_procedure");
        }
        public ActionResult GetCasesDataToGrid(string sidx, string sord, int page, int rows, string filters)
        {


            using (LoanDataDataContext database = new LoanDataDataContext())
            {
                // var loan = from d in database.Client_Loan_Infos select d;
                // long uid = long.Parse(TGSession.ClientInfo.UID);

                //This work sucess
                var loan = (from d in database.Legal_Reports
                           
                            select d).ToList();


                if (!string.IsNullOrEmpty(filters))
                {
                    var search = filters.Split('&');
                    if (!string.IsNullOrEmpty(search[0]))
                    {
                        var temp = from l in loan where l.AccountID == search[0] select l;
                        //loan = loan.Select(l=>l.Client_Acc==AccID).ToList();
                        loan = temp.ToList();
                    }
                    if (!string.IsNullOrEmpty(search[1]))
                    {
                        var temp = from l in loan where l.Lawer.Contains(search[1]) select l;
                        //loan = loan.Select(l=>l.Client_Acc==AccID).ToList();
                        loan = temp.ToList();
                    }
                }



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

    }
}
