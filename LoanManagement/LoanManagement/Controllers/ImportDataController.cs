using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoanManagement.Models;
using System.IO;
using System.Data;
using System.Data.OleDb;
using Excel;
using System.Globalization;
namespace LoanManagement.Controllers
{
    public class ImportDataController : TemplateController
    {

        public Loan_Info loan = new Loan_Info();
        //
        // GET: /ImportData/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult FormUpload()
        {
            return View();
        }

        

        public List<Loan_Info> ConvertToLoanObject(string fileName, string path, List<Loan_Info> loans)
        {
            var ws = Workbook.Worksheets(path);

            switch (fileName)
            {
                case "sab":

                    break;
                default:
                    break;
            }
            //new solution
            foreach (var worksheet in ws)
                foreach (var row in worksheet.Rows.Skip(1))
                {
                    Loan_Info l = new Loan_Info();

                }
            return null;
        }

        [HttpPost]
        public ActionResult SaveForm()
        {
            try
            {
                string UploadPath = HttpContext.Request.MapPath("~/FileUploaded/ROForms");


                foreach (string fileName in Directory.GetFiles(UploadPath))
                {
                    string path = fileName;

                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }

                }


                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i] as HttpPostedFileBase;
                    string fileName = file.FileName;

                    if (file.ContentLength == 0)
                        continue;
                    if (file.ContentLength > 0)
                    {
                        string path = Path.Combine(UploadPath, fileName);
                        string extension = Path.GetExtension(file.FileName);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        file.SaveAs(path);
                        
                    }
                }

                
            }
            catch (Exception ex)
            {

                return Json("Error : " + ex.Message);
            }


            return Json("Saved And OK");
        }

        [HttpPost]
        public ActionResult upload(string typ)
        {
            try
            {
                string UploadPath = HttpContext.Request.MapPath("~/FileUploaded/Temp");
                DataSet ds = new DataSet();
                //here should delete all files

                foreach (string fileName in Directory.GetFiles(UploadPath))
                {
                    string path = fileName;

                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }


                }


                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i] as HttpPostedFileBase;
                    string fileName = file.FileName;

                    if (file.ContentLength == 0)
                        continue;
                    if (file.ContentLength > 0)
                    {
                        // string path = Path.Combine(HttpContext.Request.MapPath(UploadPath), fileName);
                        string path = Path.Combine(UploadPath, fileName);
                        string extension = Path.GetExtension(file.FileName);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        //  Now Here Should Read Excel Data and add to database
                        file.SaveAs(path);






                    }
                }
                
            }
            catch (Exception ex)
            {

                return Json("Error : " + ex.Message);
            }

            readingExcelFile(typ);
            return Json("Saved And OK");
        }

        public string readingExcelFile(string isUpdate)
        {

            string UploadPath = HttpContext.Request.MapPath("~/FileUploaded/Temp");
            DataSet ds = new DataSet();
            foreach (string fileName in Directory.GetFiles(UploadPath))
            {
                string path = fileName;
                string extension = Path.GetExtension(fileName).ToLower();
                string fn = Path.GetFileName(fileName).Split('.')[0];
                //here start Reading
                //Now Read files
                if (extension == ".xls" || extension == ".xlsx")
                {

                    string excelConnectionString = string.Empty;
                    //  connection String for xls file format.
                    if (extension == ".xls")
                    {
                        excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;\"";
                    }
                    //connection String for xlsx file format.
                    else if (extension == ".xlsx")
                    {
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties='Excel 12.0;IMEX=1;'";
                    }
                    //Create Connection to Excel work book and add oledb namespace
                    OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                    excelConnection.Open();
                    DataTable dt = new DataTable();

                    dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dt == null)
                    {
                        return null;
                    }

                    String[] excelSheets = new String[dt.Rows.Count];
                    int t = 0;
                    //excel data saves in temp file here.
                    foreach (DataRow row in dt.Rows)
                    {
                        string sheetName = row["TABLE_NAME"].ToString();
                        if (sheetName.Contains("{ONE}"))
                        {
                            //{ONE}
                            excelSheets[0] = sheetName;
                        }

                        if (sheetName.Contains("_xlnm#_FilterDatabase"))
                            continue;
                        else
                        {
                            excelSheets[t] = row["TABLE_NAME"].ToString();
                            t++;
                        }

                    }
                    OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);
                    string query = string.Format("SELECT * FROM [{0}]", excelSheets[0]);
                    //if (fn=="risk")
                    //{
                    //    query = string.Format("SELECT * FROM [{0}]", excelSheets[1]);
                    //}

                    using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                    {
                        dataAdapter.Fill(ds, fn);

                    }
                    excelConnection.Close();
                    excelConnection1.Close();
                }


            }
            ///Start to add client
            
            DataTable client = ds.Tables["SAB"];
            //var ff=ds.Tables["SAB"].AsEnumerable().AsEnumerable<SABClass>();
            DataTable risk = ds.Tables["risk"];
            if (risk != null)
            {
                var lstrist = ds.Tables["risk"].AsEnumerable().ToList();
            }

            List<CLIENT_PERINFO> lstClient = new List<CLIENT_PERINFO>();
            List<Loan_Info> lstLoans = new List<Loan_Info>();
            List<string> notes = new List<string>();
            List<string> RO = new List<string>();
            if (client != null)
            {
                //update by abd alrahman
                foreach (DataRow row in client.Rows)
                {
                    //Here Is a Client
                    CLIENT_PERINFO cl = new CLIENT_PERINFO();
                    cl.FULLNAME = row["Arabic Full Name"].ToString();//
                    cl.ADDR1 = row["Adress"].ToString();//2
                    //cl.ADDR1 = row["Adress 1"].ToString();
                    //cl.ADDR2 = row["Adress 2"].ToString();
                    //cl.ADDR3 = row["Adress 3"].ToString();
                    cl.TEL = row["Telephone"].ToString();//3
                    cl.AccountID = row["Account ID"].ToString();//4


                    //Here Is Loan Info
                    Loan_Info l = new Loan_Info();
                    l.AccountID = row["Account ID"].ToString();//4
                    l.FacilityTyp = row["Type"].ToString();//5
                    // l.Branch = row["Branch"].ToString();
                    l.Charges = row["Charges"].ToString();//6
                    l.Unpaid_Amount = row["Total Unpaid"].ToString();//7
                    l.Outstanding_Amount = row["Outstanding"].ToString();//8
                    l.Monthely_Payment = row["Monthly payment"].ToString();
                    l.Rate_NPL = row["Risk Rate"].ToString();
                    l.NOUnpaid = row["No of UNpaid/Real"].ToString();

                    //here add from risk
                    //   string clientID = item["Account ID"].ToString();
                    //  var cl = lstLoans.Find(l => l.AccountID == clientID);
                    //var clup = lstClient.Find(l => l.AccountID == clientID);
                    // string accid = row["Account ID"].ToString() ;
                    //  var it = lstrist.Where(bb => bb.Field<string>(1).ToString() == accid);


                    if (risk != null)
                    {

                        var item = from r in risk.AsEnumerable() where (r.Field<string>("Account ID")) == row["Account ID"].ToString() select r;
                        if (item.ToList().Count > 0)
                        {
                            cl.EnglishName = item.FirstOrDefault()["Name of client"].ToString();
                            l.Branch = item.FirstOrDefault()["Branch"].ToString();

                            l.NPL_Amount = item.FirstOrDefault()["Total gross funded exposure NPL"].ToString();
                            l.Provision_Amount = item.FirstOrDefault()["Provisions balances"].ToString();
                            l.UID = item.FirstOrDefault()["UID"].ToString();
                            try
                            {
                                string dat = item.FirstOrDefault()["Date downgraded to NPL"].ToString();
                                DateTime datd;

                                if (DateTime.TryParse(dat, out datd))
                                {
                                    l.Date_Downgrading = datd;
                                }

                            }
                            catch
                            {

                            }


                            l.Note = item.FirstOrDefault()["RMD new comments"].ToString();

                            DateTime rDate;
                            try
                            {
                                if (item.FirstOrDefault()["File receiving date"].ToString() != null)
                                {
                                    DateTime.TryParse(item.FirstOrDefault()["File receiving date"].ToString(), out rDate);
                                    l.Receiving_Date = rDate;
                                }
                            }
                            catch
                            {

                            }

                            l.Relation_With_Other_Banks = item.FirstOrDefault()["Relation with other Banks"].ToString();
                            l.Remarks = item.FirstOrDefault()["Remarks"].ToString();
                            l.Previous_RO = item.FirstOrDefault()["Previous RO"].ToString();

                        }
                    }

                    //  var item = from r in risk.AsEnumerable() where (r.Field<string>("Account ID")) == row["Account ID"].ToString() select r;

                    // var item = from r in risk.AsEnumerable() where (r.Field<string>("Account ID")) == accid select r;
                    //here did not get data
                    //string expression="Account ID = '"+row["Account ID"].ToString()+"'";
                    //var item = risk.Select(expression);







                    lstClient.Add(cl);
                    lstLoans.Add(l);


                }
            }




            DataTable guarant = ds.Tables["guarantees"];
            List<Guarantees_Info> lstguan = new List<Guarantees_Info>();
            if (guarant != null)
            {
                foreach (DataRow item in guarant.Rows)
                {
                    Guarantees_Info guan = new Guarantees_Info();
                    guan.AccountID = item["Account ID"].ToString();
                    guan.Applicant_Account = item["Applicant Account"].ToString();
                    guan.Applicant_Name = item["Applicant Name"].ToString();
                    if (!string.IsNullOrEmpty(item["Estimate date"].ToString()))
                    {
                        DateTime estimateDate;
                        if (DateTime.TryParse(item["Estimate date"].ToString(), out estimateDate))
                        {
                            guan.Estimate_date = estimateDate;
                        }
                    }

                    guan.Estimated_Amount = item["Estimated Amount"].ToString();
                    guan.Gurantee_Description = item["Gurantee Description"].ToString();
                    guan.Gurantee_Type = item["Gurantee Type"].ToString();
                    guan.Initial_Amount = item["Initial Amount"].ToString();

                    lstguan.Add(guan);

                }
            }

            //now here legal
            DataTable legal = ds.Tables["legal"];
            List<Legal_Report> lstRep = new List<Legal_Report>();
            if (legal != null)
            {
                if (isUpdate == "false")
                {
                    foreach (DataRow item in legal.Rows)
                    {
                        Legal_Report rep = new Legal_Report();
                        string accID = item["Account ID"].ToString();
                        if (accID.Length < 7)
                        {
                            rep.AccountID = accID.PadLeft(7, '0');
                        }
                        else
                        {
                            rep.AccountID = item["Account ID"].ToString();
                        }

                        rep.Court = item["المحكمة "].ToString();
                        rep.lawsuit_typ = item["نوع الدعوى "].ToString();
                        rep.lawsuitNOFirst = item[6].ToString();
                        rep.lawsuitNOSec = item[7].ToString();
                        rep.PrevintingTravel = item["منع سفر"].ToString();
                        if (!string.IsNullOrEmpty(item["تاريخ منع السفر"].ToString()))
                        {
                            DateTime prevTravDate;
                            if (DateTime.TryParse(item["تاريخ منع السفر"].ToString(), out prevTravDate))
                            {
                                rep.PrevTravDate = prevTravDate;
                            }
                        }
                        rep.Lawer = item["المحامي "].ToString();
                        using (LoanDataDataContext database = new LoanDataDataContext())
                        {
                            database.Legal_Reports.InsertOnSubmit(rep);
                            database.SubmitChanges();
                            long len = item.ItemArray.Count();
                            List<Legal_Procedure> lstPro = new List<Legal_Procedure>();
                            //here should insert legal
                            for (int i = 12; i < len; i++)
                            {
                                Legal_Procedure lp = new Legal_Procedure();
                                lp.AccountID = rep.AccountID;
                                lp.LegalRepID = rep.ID;
                                lp.Legal_Desc = legal.Columns[i].Caption;
                                lp.Proced = item[i].ToString();
                                lstPro.Add(lp);
                            }
                            database.Legal_Procedures.InsertAllOnSubmit(lstPro);
                            database.SubmitChanges();
                        }


                        // lstRep.Add(rep);

                    }
                }
                else if (isUpdate == "true")
                {
                    using (LoanDataDataContext database = new LoanDataDataContext())
                    {
                        foreach (DataRow item in legal.Rows)
                        {
                            string accID = item["Account ID"].ToString();
                            if (accID.Length < 7)
                            {
                                accID = accID.PadLeft(7, '0');
                            }


                            Legal_Report rep = (from l in database.Legal_Reports where l.AccountID == accID select l).FirstOrDefault();
                            if (rep != null)
                            {
                                //  rep.AccountID =
                                rep.Court = item["المحكمة "].ToString();
                                rep.lawsuit_typ = item["نوع الدعوى "].ToString();
                                rep.lawsuitNOFirst = item[6].ToString();
                                rep.lawsuitNOSec = item[7].ToString();
                                rep.PrevintingTravel = item["منع سفر"].ToString();
                                if (!string.IsNullOrEmpty(item["تاريخ منع السفر"].ToString()))
                                {
                                    DateTime prevTravDate;
                                    if (DateTime.TryParse(item["تاريخ منع السفر"].ToString(), out prevTravDate))
                                    {
                                        rep.PrevTravDate = prevTravDate;
                                    }

                                }
                                rep.Lawer = item["المحامي "].ToString();
                                database.SubmitChanges();

                                long len = item.ItemArray.Count();
                                List<Legal_Procedure> lstPro = new List<Legal_Procedure>();
                                //here should insert legal
                                for (int i = 12; i < len; i++)
                                {
                                    var legalPr = from p in database.Legal_Procedures where p.Legal_Desc == legal.Columns[i].Caption && p.LegalRepID == rep.ID select p;
                                    if (legalPr.ToList().Count > 0)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        Legal_Procedure lp = new Legal_Procedure();
                                        lp.AccountID = rep.AccountID;
                                        lp.LegalRepID = rep.ID;
                                        lp.Legal_Desc = legal.Columns[i].Caption;
                                        lp.Proced = item[i].ToString();
                                        lstPro.Add(lp);
                                    }

                                }
                                database.Legal_Procedures.InsertAllOnSubmit(lstPro);
                                database.SubmitChanges();


                            }


                            // lstRep.Add(rep);

                        }
                    }

                }
            }






            //now Inser To DB
            using (LoanDataDataContext database = new LoanDataDataContext())
            {
                if (isUpdate == "false")
                {
                    for (int i = 0; i < lstClient.Count; i++)
                    {
                        CLIENT_PERINFO cl = lstClient[i];
                        Loan_Info l = lstLoans[i];
                        Guarantees_Info guan = lstguan[i];
                        // l.Receiving_Date = null;

                        database.CLIENT_PERINFOs.InsertOnSubmit(cl);
                        database.Loan_Infos.InsertOnSubmit(l);
                        database.Guarantees_Infos.InsertOnSubmit(guan);
                        database.SubmitChanges();

                        Client_Loan_Info cli = new Client_Loan_Info();
                        cli.AccountID = cl.AccountID;
                        cli.CDate = DateTime.Now;
                        cli.ClientID = cl.ID;
                        cli.Read_Note = false;
                        cli.Date_Rec = DateTime.Now;
                        cli.GuaranteesID = guan.ID;
                        cli.LoanID = l.ID;
                        // cli.Note = notes[i];
                        database.Client_Loan_Infos.InsertOnSubmit(cli);
                        database.SubmitChanges();
                        //var results = from myRow in myDataTable.AsEnumerable()
                        //              where myRow.Field<int>("RowNo") == 1
                        //              select myRow;



                        var ro = from r in risk.Select() where (r.Field<string>("Account ID")) == cl.AccountID select r;
                        if (ro.ToList().Count < 1)
                        {
                            continue;
                        }
                        else
                        {
                            Sys_User usr = database.Sys_Users.FirstOrDefault(u => u.RO_Code == ro.FirstOrDefault().Field<string>("RO Code"));
                            if (usr != null)
                            {
                                Track tr = new Track();
                                tr.ClientLoanID = cli.ID;
                                tr.CurrTraker = usr.id;
                                tr.Status = 2;
                                tr.Tracking_Action = 1;
                                tr.Tracking_Date = DateTime.Now;
                                tr.Tracking_Details = "";
                                database.Tracks.InsertOnSubmit(tr);
                                database.SubmitChanges();
                                Tracker tracker = new Tracker();
                                tracker.Admin_Declare = long.Parse(TGSession.ClientInfo.UID);
                                tracker.Declared_Date = DateTime.Now;
                                tracker.Trackers = usr.id;
                                tracker.TracksID = tr.ID;
                                //var not = from r in risk.AsEnumerable() where (r.Field<double>("Account ID")).ToString() == cl.AccountID.ToString() select r;

                                tracker.Admin_Notes = l.Note;
                                database.Trackers.InsertOnSubmit(tracker);
                                database.SubmitChanges();
                            }
                        }





                    }

                }
                else if (isUpdate == "true")
                {


                    if (risk != null)
                    {
                        foreach (DataRow item in risk.Rows)
                        {
                            string clientID = item["Account ID"].ToString();

                            Loan_Info updl = (from up in database.Loan_Infos
                                              where up.AccountID == clientID
                                              select up).FirstOrDefault();






                            if (updl != null)
                            {

                                //risk
                                if (!string.IsNullOrEmpty(item["Total gross funded exposure NPL"].ToString()))
                                {
                                    updl.NPL_Amount = item["Total gross funded exposure NPL"].ToString();
                                }

                                //risk
                                if (!string.IsNullOrEmpty(item["Provisions balances"].ToString()))
                                {
                                    updl.Provision_Amount = item["Provisions balances"].ToString();
                                }
                                //risk
                                if (!string.IsNullOrEmpty(item["Date downgraded to NPL"].ToString()))
                                {
                                    string dat = item["Date downgraded to NPL"].ToString();
                                    DateTime dateDowngrading;
                                    if (DateTime.TryParse(dat, out dateDowngrading))
                                    {
                                        updl.Date_Downgrading = dateDowngrading;
                                    }

                                }

                                //risk
                                if (!string.IsNullOrEmpty(item["UID"].ToString()))
                                {
                                    updl.UID = item["UID"].ToString();
                                }



                            }

                            //Here should Update

                            database.SubmitChanges();

                        }

                    }
                    if (client != null)
                    {
                        //now sab
                        foreach (DataRow item in client.Rows)
                        {
                            string clientID = item["Account ID"].ToString();

                            Loan_Info updl = (from up in database.Loan_Infos
                                              where up.AccountID == clientID
                                              select up).FirstOrDefault();

                            if (updl != null)
                            {
                                //sab
                                if (!string.IsNullOrEmpty(item["Charges"].ToString()))
                                {
                                    updl.Charges = item["Charges"].ToString();
                                }
                                //sab
                                if (!string.IsNullOrEmpty(item["Type"].ToString()))
                                {
                                    updl.FacilityTyp = item["Type"].ToString();
                                }
                                //sab
                                if (!string.IsNullOrEmpty(item["Monthly payment"].ToString()))
                                {
                                    updl.Monthely_Payment = item["Monthly payment"].ToString();
                                }
                                //sab
                                if (!string.IsNullOrEmpty(item["No of UNpaid/Real"].ToString()))
                                {
                                    updl.NOUnpaid = item["No of UNpaid/Real"].ToString();
                                }

                                //sab
                                if (!string.IsNullOrEmpty(item["Outstanding"].ToString()))
                                {
                                    updl.Outstanding_Amount = item["Outstanding"].ToString();
                                }

                                //sab
                                if (!string.IsNullOrEmpty(item["Risk Rate"].ToString()))
                                {
                                    updl.Rate_NPL = item["Risk Rate"].ToString();
                                }

                                //sab
                                if (!string.IsNullOrEmpty(item["Total Unpaid"].ToString()))
                                {
                                    updl.Unpaid_Amount = item["Total Unpaid"].ToString();
                                }


                            }

                            //Here should Update

                            database.SubmitChanges();

                        }

                    }





                    if (lstguan.Count > 0)
                    {


                        for (int i = 0; i < lstLoans.Count; i++)
                        {

                            Guarantees_Info guan = lstguan[i];
                            Guarantees_Info updl = (from up in database.Guarantees_Infos
                                                    where up.AccountID == guan.AccountID
                                                    select up).FirstOrDefault();
                            if (updl != null)
                            {
                                if (!string.IsNullOrEmpty(guan.Applicant_Account))
                                {
                                    updl.Applicant_Account = guan.Applicant_Account;
                                }
                                if (!string.IsNullOrEmpty(guan.Applicant_Name))
                                {
                                    updl.Applicant_Name = guan.Applicant_Name;
                                }
                                if (guan.Estimate_date.HasValue)
                                {
                                    updl.Estimate_date = guan.Estimate_date;
                                }
                                if (!string.IsNullOrEmpty(guan.Estimated_Amount))
                                {
                                    updl.Estimated_Amount = guan.Estimated_Amount;
                                }
                                if (!string.IsNullOrEmpty(guan.Gurantee_Description))
                                {
                                    updl.Gurantee_Description = guan.Gurantee_Description;
                                }
                                if (!string.IsNullOrEmpty(guan.Gurantee_Type))
                                {
                                    updl.Gurantee_Type = guan.Gurantee_Type;
                                }
                                if (!string.IsNullOrEmpty(guan.Initial_Amount))
                                {
                                    updl.Initial_Amount = guan.Initial_Amount;
                                }
                            }

                            //Here should Update

                            database.SubmitChanges();

                        }
                    }

                }


                //database.CLIENT_PERINFOs.InsertAllOnSubmit(lstClient);
                //database.Loan_Infos.InsertAllOnSubmit(lstLoans);
                //database.Guarantees_Infos.InsertAllOnSubmit(lstguan);
                //database.SubmitChanges();


                //here tracking
                ///
                DataTable tracking = ds.Tables["Tracking"];


                if (tracking != null)
                {
                    foreach (DataRow item in tracking.Rows)
                    {
                        Tracking_History history = new Tracking_History();
                        history.Account_ID = item["Client_AC"].ToString();

                        DateTime trackingDate;
                        if (DateTime.TryParse(item["Tracking Date"].ToString(), out trackingDate))
                        {
                            history.Tacking_Date = trackingDate;
                        }

                        history.Tracking_Notes = item["Tracking Details"].ToString();
                        database.Tracking_Histories.InsertOnSubmit(history);
                        database.SubmitChanges();
                    }

                }

            }



            //    int i= ds.Tables.Count;
            return "";

        }

    }
}
