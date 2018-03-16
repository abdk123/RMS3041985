using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoanManagement.Models;
using System.Collections;
namespace LoanManagement.Controllers
{
    public class ClientManagementController : TemplateController
    {
        [HttpPost]
        public ActionResult InsertNewClientInfo(FormCollection collection)
        {
            try
            {
                //update from abd
                using (LoanDataDataContext database = new LoanDataDataContext())
                {

                    string accountID=collection.Get("ACCOUNTID");
                    var clients = from cl in database.CLIENT_PERINFOs where cl.AccountID == accountID select cl;
                    foreach (var item in clients)
                    {
                        CLIENT_INFOUPD upd = new CLIENT_INFOUPD();
                       
                        long cID = 0;
                        long.TryParse(collection.Get("TrackID"), out cID);
                        upd.TRACID = cID;
                        upd.ADDR1 = collection.Get("ADDR1");
                        upd.ADDR2 = collection.Get("ADDR2");
                        upd.ADDR3 = collection.Get("ADDR3");
                        upd.Another_Address = collection.Get("Another_Address");
                        upd.Another_Mobile = collection.Get("Another_Mobile");
                        upd.Another_TEL = collection.Get("Another_TEL");
                        DateTime rDate=new DateTime();
                        
                        
                        upd.CLIENTID = item.ID;//long.Parse(collection.Get("CLIENTID"));
                        upd.DATE_UPD = DateTime.Now;
                        //   track.Tracking_USER = long.Parse(collection.Get("Tracking_USER"));
                        upd.ADDITIONINFO = collection.Get("ADDITIONINFO");
                        upd.MOBILE = collection.Get("MOBILE");
                        upd.TEL = collection.Get("TEL");
                        upd.USR_REC = long.Parse(TGSession.ClientInfo.UID);

                        

                        database.CLIENT_INFOUPDs.InsertOnSubmit(upd);
                        database.SubmitChanges();

                        item.Another_Address = upd.Another_Address;
                        item.Another_Mobile = upd.Another_Mobile;
                        item.Another_TEL = upd.Another_TEL;
                       

                        database.SubmitChanges();
                    }

                 

                  

                }

                var jsonObj = new { Message = "Client Info Updated" };
                return Json(jsonObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                var jsonObj = new { Message = ex.Message };
                return Json(jsonObj, JsonRequestBehavior.AllowGet);
            }

        
        }
       // GetAllDataToGrid
        //
        public ActionResult GetAllClientData(string sidx, string sord, int page, int rows, string filters) 
        {
            try
            {
                long loanID = 0;
                if (!string.IsNullOrEmpty(filters))
                {
                   
                    loanID = long.Parse(filters);
                }
                ArrayList lst = new ArrayList();
                using (LoanDataDataContext database = new LoanDataDataContext())
                {
                    var cl = from d in database.Tracks
                            join ci in database.Client_Loan_Infos on d.ClientLoanID equals ci.ID
                              join c in database.CLIENT_PERINFOs on ci.ClientID equals c.ID
                            
                              where d.ID == loanID
                              select c;
                    lst.AddRange(cl.ToList());
                    var updclient = (from up in database.CLIENT_INFOUPDs where cl.ToList().FirstOrDefault().ID == up.CLIENTID select up).OrderByDescending(u=>u.ID).ToList();

                   // lst.AddRange(updclient.Skip(1).ToList());
                    lst.AddRange(updclient.ToList());
                }

                int pageIndex = Convert.ToInt32(page) - 1;
                int pageSize = rows;

                int totalRecords = lst.Count;




                var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

                //lst = lst.ToArray().Skip(pageIndex * pageSize).Take(pageSize).;


                var jsonData = new
                {
                    total = totalPages,
                    page,
                    records = totalRecords,
                    rows = lst
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
               
            }
            catch (Exception ex)
            {

                var jsonObj = new { Message = ex.Message };
                return Json(jsonObj, JsonRequestBehavior.AllowGet);
            }
        }
        // GET: /ClientManagement/
        public  ActionResult GetLastClientData(long loanID) 
        {
            ArrayList lst = new ArrayList();
            using (LoanDataDataContext database = new LoanDataDataContext())
            {
                var cl = from d in database.Client_Loan_Infos

                         join c in database.CLIENT_PERINFOs on d.ClientID equals c.ID

                         where d.ID == loanID
                         select c;
                lst.AddRange(cl.ToList());
                var updclient = (from up in database.CLIENT_INFOUPDs where cl.ToList().FirstOrDefault().ID == up.CLIENTID select up).OrderByDescending(u => u.ID).ToList();

                var result = new {
                    FULLNAME=cl.FirstOrDefault().FULLNAME,
                    ADDR = string.IsNullOrEmpty(updclient.FirstOrDefault().ADDR) ? cl.FirstOrDefault().ADDR : updclient.FirstOrDefault().ADDR,
                    MOBILE = string.IsNullOrEmpty(updclient.FirstOrDefault().MOBILE) ? cl.FirstOrDefault().MOBILE : updclient.FirstOrDefault().MOBILE,
                    TEL=string.IsNullOrEmpty(updclient.FirstOrDefault().TEL) ? cl.FirstOrDefault().TEL : updclient.FirstOrDefault().TEL,
                    ADDITIONINFO = string.IsNullOrEmpty(updclient.FirstOrDefault().ADDITIONINFO) ? cl.FirstOrDefault().ADDITIONINFO : updclient.FirstOrDefault().ADDITIONINFO,
                    ACCOUNTID = cl.FirstOrDefault().AccountID
                };
                return Json(result, JsonRequestBehavior.AllowGet); 
            }
                 var jsonObj = new { Message ="Error Exception Occur" };
                 return Json(jsonObj, JsonRequestBehavior.AllowGet);
        }
        public static object GetLastClientDataObj(long loanID) 
        {
           
            using (LoanDataDataContext database = new LoanDataDataContext())
            {
                //apdate by abd alrahman

                var cl = (from d in database.Client_Loan_Infos
                          join l in database.Loan_Infos on d.LoanID equals l.ID
                          join c in database.CLIENT_PERINFOs on d.ClientID equals c.ID

                          where d.ID == loanID
                          select new {c=c,l=l }).FirstOrDefault();
               
                var result = new
                {
                    FULLNAME = cl.c.FULLNAME,
                    ADDR = cl.c.ADDR,
                    Another_Address = cl.c.Another_Address,
                    Another_Mobile = cl.c.Another_Mobile,
                    Another_TEL =  cl.c.Another_TEL,
                    ADDR1 = cl.c.ADDR1,
                    ADDR2 =  cl.c.ADDR2,
                    ADDR3 =  cl.c.ADDR3,
                    MOBILE = cl.c.MOBILE,
                    TEL = cl.c.TEL,
                    ADDITIONINFO = cl.c.ADDITIONINFO,
                    ACCOUNTID = cl.c.AccountID,
                    Receiving_Date = cl.l.Receiving_Date,
                    Relation_With_Other_Banks = cl.l.Relation_With_Other_Banks,
                    Remarks = cl.l.Remarks,
                    Previous_RO = cl.l.Previous_RO
                };

                return result;
            }
           
          //  return null;
        }
        public ActionResult Index()
        {
            return View();
        }

    }
}
