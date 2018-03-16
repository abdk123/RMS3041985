using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoanManagement.Models;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;
using System.Data;
using System.Data.OleDb;
using ClosedXML.Excel;

namespace LoanManagement.Controllers
{
    
    public class ReportController : TemplateController
    {
     

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetManagementReport() 
        {
            try
            {

                using (LoanDataDataContext database = new LoanDataDataContext())
                {
                    var loan = (from d in database.Tracks

                                join cl in database.Client_Loan_Infos on d.ClientLoanID equals cl.ID
                                join li in database.Loan_Infos on cl.LoanID equals li.ID
                                join c in database.CLIENT_PERINFOs on cl.ClientID equals c.ID
                                join r in database.Sys_Users on d.CurrTraker equals r.id
                                join g in database.Guarantees_Infos on cl.GuaranteesID equals g.ID
                              
                                select new
                                {
                                    ID = d.ID,
                                    Client_Info =c,
                                    Client_Loan_Info = cl,
                                    Loan_Info = li,
                                    guarant = g,
                                    tracks = d,
                                    RO=r,
                                    dat_down = li.Date_Downgrading.Value.ToShortDateString()

                                }).ToList();

                  
                  
             DataTable dt = new DataTable();
                    //Here Create Columns
          dt.Columns.AddRange(new DataColumn[9] 
          {
              new DataColumn("RO Code", typeof(string)),
          new DataColumn("RO name", typeof(string)),
          new DataColumn("Branch",typeof(string)), 
           new DataColumn("ID",typeof(string)), 
            new DataColumn("Client Name",typeof(string)), 
             new DataColumn("Loan Added month",typeof(string)), 
              new DataColumn("NPL",typeof(string)), 
               new DataColumn("Provision",typeof(string)), 
                new DataColumn("UID",typeof(string))
          });

          //Here Insert Row
          foreach (var item in loan)
          {
              dt.Rows.Add(item.RO.RO_Code, item.RO.RO_Name, item.Loan_Info.Branch, item.Client_Loan_Info.AccountID, item.Client_Info.FULLNAME, item.Client_Loan_Info.CDate.Value.ToShortDateString(), item.Loan_Info.NPL_Amount, item.Loan_Info.Provision_Amount, item.Loan_Info.UID);
            
          }
       
          //Exporting to Excel
          string folderPath = Server.MapPath("~/FileUploaded/Report");
          if (!Directory.Exists(folderPath))
          {
              Directory.CreateDirectory(folderPath);
          }
          //Codes for the Closed XML
          using (XLWorkbook wb = new XLWorkbook())
          {
              wb.Worksheets.Add(dt, "Management");

              //wb.SaveAs(folderPath + "DataGridViewExport.xlsx");
              string myName = Server.UrlEncode("Rep" + "_" +
              DateTime.Now.ToString("ddMMyyyyhhmm") + ".xlsx");
              MemoryStream stream = GetStream(wb);// The method is defined below
              Response.Clear();
              Response.Buffer = true;
              Response.AddHeader("content-disposition",
              "attachment; filename=" + myName);
              Response.ContentType = "application/vnd.ms-excel";
              Response.BinaryWrite(stream.ToArray());
              Response.End();
           }

      }

                  return Json(null, JsonRequestBehavior.AllowGet);

                

             
            }
            catch (Exception ex)
            {

                var jsonObj = new { Error = ex.Message };
                return Json(jsonObj, JsonRequestBehavior.AllowGet);
            }
        }

public MemoryStream GetStream(XLWorkbook excelWorkbook)
        {
            MemoryStream fs = new MemoryStream();
            excelWorkbook.SaveAs(fs);
            fs.Position = 0;
            return fs;
        }
        public ActionResult GetLegalReport()
        {
            try
            {
                //here should be updated
                using (LoanDataDataContext database = new LoanDataDataContext())
                {
                    var loan = (from d in database.Tracks

                                join cl in database.Client_Loan_Infos on d.ClientLoanID equals cl.ID
                                join li in database.Loan_Infos on cl.LoanID equals li.ID
                               join ln in database.LegalNotes on d.ID equals ln.TrakID
                                join ro in database.Sys_Users on d.CurrTraker equals ro.id
                                join lr in database.Legal_Reports on cl.AccountID equals lr.AccountID

                                select new
                                {
                                    ID = d.ID,
                                    Client_ID =cl.AccountID,
                                    Client_Loan_Info = cl,
                                    Loan_Info = li,
                                    RO=ro,
                                    tracks = d,
                                    Legal_Rep =lr,
                                    legal_note=ln

                                }).ToList();

                    DataTable dt = new DataTable();
                    //Here Create Columns
                    dt.Columns.AddRange(new DataColumn[12] 
                      {
                          new DataColumn("Client ID", typeof(string)),
                      new DataColumn("RO Code", typeof(string)),
                      new DataColumn("RO name",typeof(string)), 
                       new DataColumn("Branch",typeof(string)), 
                        new DataColumn("Loan Type",typeof(string)), 
                         new DataColumn("NPL Amount",typeof(string)), 
                          new DataColumn("Lawyer",typeof(string)), 
                           new DataColumn("Start legal action Date",typeof(string)), 
                            new DataColumn("Sending date to Legal",typeof(string)),
                             new DataColumn("Last legal note",typeof(string)),
                              new DataColumn("Travel ban",typeof(string)),
                                 new DataColumn("Travel ban Date",typeof(string))
                      });

                    //Here Insert Row
                    foreach (var item in loan)
                    {
                        dt.Rows.Add(item.Client_ID, item.RO.RO_Code, item.RO.RO_Name, item.Loan_Info.Branch, item.Loan_Info.FacilityTyp, item.Loan_Info.NPL_Amount, item.Legal_Rep.Lawer, item.Legal_Rep.lawsuitNOFirst, item.legal_note.SendingDate, item.legal_note.LegalNote1, item.Legal_Rep.PrevintingTravel, item.Legal_Rep.PrevTravDate);

                    }

                    //Exporting to Excel
                    string folderPath = Server.MapPath("~/FileUploaded/Report");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    //Codes for the Closed XML
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(dt, "Legal");

                        //wb.SaveAs(folderPath + "DataGridViewExport.xlsx");
                        string myName = Server.UrlEncode("Rep" + "_" +
                        DateTime.Now.ToString("ddMMyyyyhhmm") + ".xlsx");
                        MemoryStream stream = GetStream(wb);// The method is defined below
                        Response.Clear();
                        Response.Buffer = true;
                        Response.AddHeader("content-disposition",
                        "attachment; filename=" + myName);
                        Response.ContentType = "application/vnd.ms-excel";
                        Response.BinaryWrite(stream.ToArray());
                        Response.End();
                    }











                

                }

                var jsonObj = new { Message = "Track inserted suc" };
                return Json(jsonObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                var jsonObj = new { Error = ex.Message };
                return Json(jsonObj);
            }
        }
        public ActionResult GetDatatReport()
        {
            try
            {

                using (LoanDataDataContext database = new LoanDataDataContext())
                {
                    var loan = (from d in database.Tracks

                                join cl in database.Client_Loan_Infos on d.ClientLoanID equals cl.ID
                                join li in database.Loan_Infos on cl.LoanID equals li.ID
                             //   join c in database.LegalNotes on d.ID equals c.TrakID
                                join g in database.Guarantees_Infos on cl.GuaranteesID equals g.ID

                                select new
                                {
                                    ID = d.ID,
                                  
                                    Client_Loan_Info = cl,
                                    Loan_Info = li,
                                    guarant = g,
                                    tracks = d,
                                 //   legal_note =c

                                }).ToList();



                    DataTable dt = new DataTable();
                    //Here Create Columns
                    dt.Columns.AddRange(new DataColumn[12] 
                      {
                          new DataColumn("Unpaid Amount", typeof(string)),
                      new DataColumn("Charges", typeof(string)),
                      new DataColumn("NPL Amount",typeof(string)), 
                       new DataColumn("UID",typeof(string)), 
                        new DataColumn("Provision",typeof(string)), 
                         new DataColumn("Guarantee Type",typeof(string)), 
                          new DataColumn("Estimated amount",typeof(string)), 
                           new DataColumn("Initial Amount",typeof(string)), 
                            new DataColumn("Action",typeof(string)),
                             new DataColumn("Reschedule",typeof(string)),
                              new DataColumn("Legal Note",typeof(string)),
                                 new DataColumn("Last RO note",typeof(string))
                      });

                    //Here Insert Row
                    foreach (var item in loan)
                    {
                        string status = database.Status.FirstOrDefault(s => s.ID == item.tracks.Status) != null ? database.Status.FirstOrDefault(s => s.ID == item.tracks.Status).Name : "No Status";
                        string legal_n = database.LegalNotes.FirstOrDefault(l => l.TrakID == item.ID) != null ? database.LegalNotes.FirstOrDefault(l => l.TrakID == item.ID).LegalNote1 : "";
                        string emp_note = database.LegalNotes.FirstOrDefault(l => l.TrakID == item.ID) != null ? database.LegalNotes.FirstOrDefault(l => l.TrakID == item.ID).EmpNotes : "";
                        dt.Rows.Add(item.Loan_Info.Unpaid_Amount, item.Loan_Info.Charges, item.Loan_Info.NPL_Amount, item.Loan_Info.UID, item.Loan_Info.Provision_Amount, item.guarant.Gurantee_Type, item.guarant.Estimated_Amount, item.guarant.Initial_Amount, status, item.Client_Loan_Info.Scheduled, legal_n, emp_note);

                    }

                    //Exporting to Excel
                    string folderPath = Server.MapPath("~/FileUploaded/Report");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    //Codes for the Closed XML
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(dt, "Data");

                        //wb.SaveAs(folderPath + "DataGridViewExport.xlsx");
                        string myName = Server.UrlEncode("Rep" + "_" +
                        DateTime.Now.ToString("ddMMyyyyhhmm") + ".xlsx");
                        MemoryStream stream = GetStream(wb);// The method is defined below
                        Response.Clear();
                        Response.Buffer = true;
                        Response.AddHeader("content-disposition",
                        "attachment; filename=" + myName);
                        Response.ContentType = "application/vnd.ms-excel";
                        Response.BinaryWrite(stream.ToArray());
                        Response.End();
                    }





                  

                }

                var jsonObj = new { Message = "Track inserted suc" };
                return Json(jsonObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                var jsonObj = new { Error = ex.Message };
                return Json(jsonObj);
            }
        }
        public ActionResult GetTrackingReport()
        {
            try
            {

                using (LoanDataDataContext database = new LoanDataDataContext())
                {
                    var loan = (from d in database.Tracks

                                join cl in database.Client_Loan_Infos on d.ClientLoanID equals cl.ID
                                join li in database.Loan_Infos on cl.LoanID equals li.ID
                                join c in database.Sys_Users on d.CurrTraker equals c.id


                                select new
                                {
                                    ID = d.ID,

                                    Client_Loan_Info = cl,
                                    Loan_Info = li,
                                    last_tracking_date = database.Daily_Follow_Ups.OrderByDescending(lt => lt.ID).FirstOrDefault(f => f.Tracks_ID == d.ID),
                                    tracks = d,
                                    Ro = c,
                                    //No_Calls = database.Daily_Follow_Ups.Where(f => f.Follow_Action == database.Actions.FirstOrDefault(ac => ac.ACTION_CODE == "CALL").ID).ToList().Count,
                                    //No_Visit = database.Daily_Follow_Ups.Where(f => f.Follow_Action == database.Actions.FirstOrDefault(ac => ac.ACTION_CODE == "VISIT").ID).ToList().Count
                                    No_Calls = database.Daily_Follow_Ups.Where(f => f.Tracks_ID == d.ID && f.Follow_Action == database.Actions.FirstOrDefault(ac => ac.ACTION_CODE == "CALL").ID).ToList().Count,
                                    No_Visit = database.Daily_Follow_Ups.Where(f => f.Tracks_ID == d.ID && f.Follow_Action == database.Actions.FirstOrDefault(ac => ac.ACTION_CODE == "VISIT").ID).ToList().Count

                                });

                    DataTable dt = new DataTable();
                    //Here Create Columns
                    dt.Columns.AddRange(new DataColumn[9] 
                      {
                          new DataColumn("RO Code", typeof(string)),
                      new DataColumn("RO name", typeof(string)),
                      new DataColumn("Client ID",typeof(string)), 
                       new DataColumn("Latest tracking date",typeof(string)), 
                        new DataColumn("Follow up date",typeof(string)), 
                         new DataColumn("Last Action",typeof(string)), 
                          new DataColumn("Reschedule",typeof(string)), 
                           new DataColumn("No. of Calls",typeof(string)), 
                            new DataColumn("Visit",typeof(string))
                          
                      });

                    //Here Insert Row
                    foreach (var item in loan)
                    {
                        string status = database.Status.FirstOrDefault(s => s.ID == item.tracks.Status) != null ? database.Status.FirstOrDefault(s => s.ID == item.tracks.Status).Name : "No Status";
                        Daily_Follow_Up dil = item.last_tracking_date;
                        string last_date = dil != null ? dil.Follow_Date.Value.ToShortDateString() : "";
                      //  string action= (from a in database.Actions where a.ID==item.last_tracking_date.Follow_Action select a).FirstOrDefault().Name;
                        dt.Rows.Add(item.Ro.RO_Code, item.Ro.RO_Name, item.Loan_Info.AccountID, item.tracks.Tracking_Date, last_date, status, item.Client_Loan_Info.Scheduled, item.No_Calls, item.No_Visit);

                    }

                    //Exporting to Excel
                    string folderPath = Server.MapPath("~/FileUploaded/Report");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    //Codes for the Closed XML
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(dt, "Tracking");

                        //wb.SaveAs(folderPath + "DataGridViewExport.xlsx");
                        string myName = Server.UrlEncode("Rep" + "_" +
                        DateTime.Now.ToString("ddMMyyyyhhmm") + ".xlsx");
                        MemoryStream stream = GetStream(wb);// The method is defined below
                        Response.Clear();
                        Response.Buffer = true;
                        Response.AddHeader("content-disposition",
                        "attachment; filename=" + myName);
                        Response.ContentType = "application/vnd.ms-excel";
                        Response.BinaryWrite(stream.ToArray());
                        Response.End();
                    }


                  

                }

                var jsonObj = new { Message = "Report Generated" };
                return Json(jsonObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                var jsonObj = new { Error = ex.Message };
                return Json(jsonObj, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetTrackingForROReport(string ROCode)
        {
            try
            {
                long id = 0;
                long.TryParse(ROCode, out id);

                using (LoanDataDataContext database = new LoanDataDataContext())
                {
                    var loan = (from d in database.Tracks

                                join cl in database.Client_Loan_Infos on d.ClientLoanID equals cl.ID
                                join li in database.Loan_Infos on cl.LoanID equals li.ID
                                join c in database.Sys_Users on d.CurrTraker equals c.id
                                where d.CurrTraker==id

                                select new
                                {
                                    ID = d.ID,

                                    Client_Loan_Info = cl,
                                    Loan_Info = li,
                                    last_tracking_date = database.Daily_Follow_Ups.OrderByDescending(lt => lt.ID).FirstOrDefault(f => f.Tracks_ID == d.ID),
                                    tracks = d,
                                    Ro = c,
                                    //No_Calls = database.Daily_Follow_Ups.Where(f => f.Follow_Action == database.Actions.FirstOrDefault(ac => ac.ACTION_CODE == "CALL").ID).ToList().Count,
                                    //No_Visit = database.Daily_Follow_Ups.Where(f => f.Follow_Action == database.Actions.FirstOrDefault(ac => ac.ACTION_CODE == "VISIT").ID).ToList().Count
                                    No_Calls = database.Daily_Follow_Ups.Where(f => f.Tracks_ID == d.ID && f.Follow_Action == database.Actions.FirstOrDefault(ac => ac.ACTION_CODE == "CALL").ID).ToList().Count,
                                    No_Visit = database.Daily_Follow_Ups.Where(f => f.Tracks_ID == d.ID && f.Follow_Action == database.Actions.FirstOrDefault(ac => ac.ACTION_CODE == "VISIT").ID).ToList().Count

                                }).ToList();

                    DataTable dt = new DataTable();
                    //Here Create Columns
                    dt.Columns.AddRange(new DataColumn[9] 
                      {
                          new DataColumn("RO Code", typeof(string)),
                      new DataColumn("RO name", typeof(string)),
                      new DataColumn("Client ID",typeof(string)), 
                       new DataColumn("Latest tracking date",typeof(string)), 
                        new DataColumn("Follow up date",typeof(string)), 
                         new DataColumn("Last Action",typeof(string)), 
                          new DataColumn("Reschedule",typeof(string)), 
                           new DataColumn("No. of Calls",typeof(string)), 
                            new DataColumn("Visit",typeof(string))
                          
                      });

                    //Here Insert Row
                    foreach (var item in loan)
                    {
                        string status = database.Status.FirstOrDefault(s => s.ID == item.tracks.Status) != null ? database.Status.FirstOrDefault(s => s.ID == item.tracks.Status).Name : "No Status";
                        Daily_Follow_Up dil = item.last_tracking_date;
                        string last_date = dil != null ? dil.Follow_Date.Value.ToShortDateString() : "";
                        //  string action= (from a in database.Actions where a.ID==item.last_tracking_date.Follow_Action select a).FirstOrDefault().Name;
                        dt.Rows.Add(item.Ro.RO_Code, item.Ro.RO_Name, item.Loan_Info.AccountID, item.tracks.Tracking_Date, last_date, status, item.Client_Loan_Info.Scheduled, item.No_Calls, item.No_Visit);

                    }

                    //Exporting to Excel
                    string folderPath = Server.MapPath("~/FileUploaded/Report");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    //Codes for the Closed XML
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(dt, "Tracking");

                        //wb.SaveAs(folderPath + "DataGridViewExport.xlsx");
                        string myName = Server.UrlEncode("Rep" + "_" +
                        DateTime.Now.ToString("ddMMyyyyhhmm") + ".xlsx");
                        MemoryStream stream = GetStream(wb);// The method is defined below
                        Response.Clear();
                        Response.Buffer = true;
                        Response.AddHeader("content-disposition",
                        "attachment; filename=" + myName);
                        Response.ContentType = "application/vnd.ms-excel";
                        Response.BinaryWrite(stream.ToArray());
                        Response.End();
                    }


                  

                }

                var jsonObj = new { Message = "Report Generated" };
                return Json(jsonObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                var jsonObj = new { Error = ex.Message };
                return Json(jsonObj, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetTrackingForRCurrentRo(string accountId)
        {
            try
            {
               
                using (LoanDataDataContext database = new LoanDataDataContext())
                {
                    //var currentUser = database.Sys_Users.FirstOrDefault(x => x.user_name ==TGSession.ClientInfo.UserName);
                    //long id = currentUser.id;
                    var loan = (from d in database.Tracks

                                join cl in database.Client_Loan_Infos on d.ClientLoanID equals cl.ID
                                join li in database.Loan_Infos on cl.LoanID equals li.ID
                                join c in database.Sys_Users on d.CurrTraker equals c.id
                                where cl.AccountID == accountId

                                select new
                                {
                                    ID = d.ID,

                                    Client_Loan_Info = cl,
                                    Loan_Info = li,
                                    last_tracking_date = database.Daily_Follow_Ups.OrderByDescending(lt => lt.ID).FirstOrDefault(f => f.Tracks_ID == d.ID),
                                    tracks = d,
                                    Ro = c,
                                    //No_Calls = database.Daily_Follow_Ups.Where(f => f.Follow_Action == database.Actions.FirstOrDefault(ac => ac.ACTION_CODE == "CALL").ID).ToList().Count,
                                    //No_Visit = database.Daily_Follow_Ups.Where(f => f.Follow_Action == database.Actions.FirstOrDefault(ac => ac.ACTION_CODE == "VISIT").ID).ToList().Count
                                    No_Calls = database.Daily_Follow_Ups.Where(f => f.Tracks_ID == d.ID && f.Follow_Action == database.Actions.FirstOrDefault(ac => ac.ACTION_CODE == "CALL").ID).ToList().Count,
                                    No_Visit = database.Daily_Follow_Ups.Where(f => f.Tracks_ID == d.ID && f.Follow_Action == database.Actions.FirstOrDefault(ac => ac.ACTION_CODE == "VISIT").ID).ToList().Count

                                });

                    DataTable dt = new DataTable();
                    //Here Create Columns
                    dt.Columns.AddRange(new DataColumn[9] 
                      {
                          new DataColumn("RO Code", typeof(string)),
                      new DataColumn("RO name", typeof(string)),
                      new DataColumn("Client ID",typeof(string)), 
                       new DataColumn("Latest tracking date",typeof(string)), 
                        new DataColumn("Follow up date",typeof(string)), 
                         new DataColumn("Last Action",typeof(string)), 
                          new DataColumn("Reschedule",typeof(string)), 
                           new DataColumn("No. of Calls",typeof(string)), 
                            new DataColumn("Visit",typeof(string))
                          
                      });

                    //Here Insert Row
                    foreach (var item in loan)
                    {
                        string status = database.Status.FirstOrDefault(s => s.ID == item.tracks.Status) != null ? database.Status.FirstOrDefault(s => s.ID == item.tracks.Status).Name : "No Status";
                        Daily_Follow_Up dil = item.last_tracking_date;
                        string last_date = dil != null ? dil.Follow_Date.Value.ToShortDateString() : "";
                        //  string action= (from a in database.Actions where a.ID==item.last_tracking_date.Follow_Action select a).FirstOrDefault().Name;
                        dt.Rows.Add(item.Ro.RO_Code, item.Ro.RO_Name, item.Loan_Info.AccountID, item.tracks.Tracking_Date, last_date, status, item.Client_Loan_Info.Scheduled, item.No_Calls, item.No_Visit);

                    }

                    //Exporting to Excel
                    string folderPath = Server.MapPath("~/FileUploaded/Report");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    //Codes for the Closed XML
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(dt, "Tracking");

                        //wb.SaveAs(folderPath + "DataGridViewExport.xlsx");
                        string myName = Server.UrlEncode("Rep" + "_" +
                        DateTime.Now.ToString("ddMMyyyyhhmm") + ".xlsx");
                        MemoryStream stream = GetStream(wb);// The method is defined below
                        Response.Clear();
                        Response.Buffer = true;
                        Response.AddHeader("content-disposition",
                        "attachment; filename=" + myName);
                        Response.ContentType = "application/vnd.ms-excel";
                        Response.BinaryWrite(stream.ToArray());
                        Response.End();
                    }




                }

                var jsonObj = new { Message = "Report Generated" };
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
