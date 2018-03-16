using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoanManagement.Models;
using System.Web.Routing;
namespace LoanManagement.Controllers
{
    public class NotificationController : Controller
    {
        /**
         *   <li id="alert_notificatoin_bar" class="dropdown">
                        <a data-toggle="dropdown" class="dropdown-toggle" href="#">

                            <i class="icon-bell-l"></i>
                            <span class="badge bg-important">2</span>
                        </a>
                        <ul class="dropdown-menu extended notification">
                            <div class="notify-arrow notify-arrow-blue"></div>
                            <li>
                                <p class="blue">You have 2 Reminding Loan Follow Up</p>
                            </li>
                            <li>
                                <a href="#">
                                    <span class="label label-primary"><i class="icon_profile"></i></span> 
                                   Client Name
                                    <span class="small italic pull-right">5 mins</span>
                                </a>
                            </li>
                            <li>
                                <a href="#">
                                    <span class="label label-warning"><i class="icon_pin"></i></span>  
                                   Client Name
                                    <span class="small italic pull-right">50 mins</span>
                                </a>
                            </li>
                                               
                            <li>
                                <a href="#">See all Reminding</a>
                            </li>
                        </ul>
                    </li>
         * **/
        public static  string GetReminderLoan() 
        {
            string reminder = @" <a data-toggle='dropdown' class='dropdown-toggle' href='#'>

                            <i class='icon-bell-l'></i>
                            <span class='badge bg-important'> {COUNT} </span>
                        </a>
                        <ul class='dropdown-menu extended notification'>
                            <div class='notify-arrow notify-arrow-blue'></div>
                            <li>
                                <p class='blue'>You have {COUNT} Reminding Loan Follow Up</p>
                            </li>
                            {ITEMS}
                                               
                            <li>
                                <a href='#'>See all Reminding</a>
                            </li>
                        </ul>";
            string itemTemplate = @"<li>
                                <a href='/Reminder/Index?accountId={accid}'>
                                    <span class='label label-primary'><i class='icon_profile'></i></span> 
                                   Account ID:{accid}-{fullname}
                                    <span class='small italic pull-right'>{date}</span>
                                </a>
                            </li>";

            //
            if (TGSession.ClientInfo==null)
            {
                return "";
            }
            using (LoanDataDataContext database = new LoanDataDataContext())
            {
                // var loan = from d in database.Client_Loan_Infos select d;
                long uid = long.Parse(TGSession.ClientInfo.UID);

                //This work sucess
                var notify = (from d in database.Daily_Follow_Ups

                            join t in database.Tracks on d.Tracks_ID equals t.ID
                            join ci in database.Client_Loan_Infos on t.ClientLoanID equals ci.ID
                            join c in database.CLIENT_PERINFOs on ci.ID equals c.ID
                              where t.CurrTraker == uid && (d.Remainder_Date.Value.DayOfYear == DateTime.Now.Date.DayOfYear && d.Remainder_Date.Value.Year == DateTime.Now.Date.Year)
                            select new
                            {
                                ID = d.ID,
                                Client_Info = c,
                                Follow_Ups = d,
                                track = t,
                                loan_info=ci

                               
                            }).ToList();
                int noticount = notify.Count;
                string rem = reminder.Replace("{COUNT}", noticount.ToString());
                string items = string.Empty;
                foreach (var item in notify)
                {
                    string temp = string.Empty;
                    string accID = item.loan_info.AccountID;
                    string remDate = item.Follow_Ups.Remainder_Date.Value.ToShortDateString();
                    string fullname = item.Client_Info.FULLNAME;
                    temp = itemTemplate.Replace("{accid}", accID);
                    temp = temp.Replace("{fullname}", fullname);
                    temp = temp.Replace("{date}", remDate);
                    items += temp;
                }

                rem = rem.Replace("{ITEMS}", items);
                return rem;
            }

           

        }

        /**
         *  <a data-toggle="dropdown" class="dropdown-toggle" href="#">
                            <i class="icon-envelope-l"></i>
                            <span class="badge bg-important">3</span>
                        </a>
                        <ul class="dropdown-menu extended inbox">
                            <div class="notify-arrow notify-arrow-blue"></div>
                            <li>
                                <p class="blue">You have 3 Loan Declared to tracks</p>
                            </li>
                            <li>
                                <a href="#">
                                    <span class="photo"></span>
                                    <span class="subject">
                                    <span class="from">Administrator</span>
                                  
                                    </span>
                                    <span class="message">
                                        Note Of Tracks
                                    </span>
                                </a>
                            </li>
                            <li>
                                <a href="#">
                                    <span class="photo"></span>
                                    <span class="subject">
                                    <span class="from">Administrator</span>
                                   
                                    </span>
                                    <span class="message">
                                    Notes of Tracks
                                    </span>
                                </a>
                            </li>
                            <li>
                                <a href="#">
                                    <span class="photo"></span>
                                    <span class="subject">
                                    <span class="from">Administrator</span>
                                    
                                    </span>
                                    <span class="message">
                                      Notes of Tracks
                                    </span>
                                </a>
                            </li>
                        
                            <li>
                                <a href="#">See all Declared Tracks</a>
                            </li>
                        </ul>
         * **/
        //Decalred Notify
        public static string GetDeclaredLoan()
        {
            // <a href='#'>See all Declared Tracks</a>
            string reminder = @"  <a data-toggle='dropdown' class='dropdown-toggle' href='#'>
                            <i class='icon-envelope-l'></i>
                            <span class='badge bg-important'>{COUNT}</span>
                        </a>
                         <ul class='dropdown-menu extended inbox'>
                            <div class='notify-arrow notify-arrow-blue'></div>
                            <li>
                                <p class='blue'>You have {COUNT} Loan Declared to tracks</p>
                            </li>
                            {ITEMS}
                                               
                           <li>
                              {LINK}
                            </li>
                        </ul>";
            string itemTemplate = @" <li>
                                <a href='#' class='navigateToFollowUp'>
                                    <span class='photo'></span>
                                    <span class='subject'>
                                    <span class='from'>Client ID :{ACCID}</span>
                                    Client Name : {NAME}
                                    </span>
                                    <span class='message'>
                                     {TRACKNOTE}
                                    </span>
                                </a>
                            </li>";

            //
            if (TGSession.ClientInfo == null)
            {
                return "";
            }
            using (LoanDataDataContext database = new LoanDataDataContext())
            {
                // var loan = from d in database.Client_Loan_Infos select d;
                long uid = long.Parse(TGSession.ClientInfo.UID);
                string user_role = TGSession.ClientInfo.UserRole;
                var notify =(from d in database.Tracks


                                       join ci in database.Client_Loan_Infos on d.ClientLoanID equals ci.ID
                                       join c in database.CLIENT_PERINFOs on ci.ID equals c.ID
                                       join tr in database.Trackers on d.ID equals tr.TracksID
                             where d.CurrTraker == uid && (!database.Daily_Follow_Ups.Any(dial => dial.Tracks_ID == d.ID)) && database.Client_Loan_Infos.Any(x => x.Read_Note == false)
                                       select new
                                       {
                                           ID = d.ID,
                                           Client_Info = c,

                                           track = d,
                                           loan_info = ci,
                                           tracker = tr


                                       }).ToList();
                //This work sucess
                if (user_role=="4" || user_role=="5")
                {
                    notify = (from d in database.Tracks


                              join ci in database.Client_Loan_Infos on d.ClientLoanID equals ci.ID
                              join c in database.CLIENT_PERINFOs on ci.ID equals c.ID
                              join tr in database.Trackers on d.ID equals tr.TracksID
                              where (!database.Daily_Follow_Ups.Any(dial => dial.Tracks_ID == d.ID))
                              select new
                              {
                                  ID = d.ID,
                                  Client_Info = c,

                                  track = d,
                                  loan_info = ci,
                                  tracker = tr


                              }).ToList();
                }





               //101600
                notify=notify.Where(x=>x.loan_info.Read_Note==false).ToList();
                int noticount = notify.Count;
                string rem = reminder.Replace("{COUNT}", noticount.ToString());
                string items = string.Empty;
                foreach (var item in notify.Take(4))
                {
                    string temp = string.Empty;
                    string accID = item.loan_info.AccountID;
                    string trackNote = item.tracker.Admin_Notes;
                    string fullname = item.Client_Info.FULLNAME;
                    temp = itemTemplate.Replace("{ACCID}", accID);
                    temp = temp.Replace("{NAME}", fullname);
                    temp = temp.Replace("{TRACKNOTE}", trackNote);
                    items += temp;
                }

                rem = rem.Replace("{ITEMS}", items);
                string link = string.Empty;
                //@"<a href='#'>See all Declared Tracks</a>"
                if (user_role=="4" || user_role=="5")
                {
                    link=@"<a href='/TrackLoans'>See all Declared Tracks</a>";
                }
                else if (user_role=="2" || user_role=="3")
                {
                     link=@"<a href='/FollowUp'>See all Declared Tracks</a>";
                }
                rem = rem.Replace("{LINK}", link);
                return rem;
            }



        }
        //
        // GET: /Notification/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RedirectToFollowUp(string clientId)
        {
            //abd alrahman//
            using (var db = new LoanDataDataContext())
            {
                var clientLoanInfos = db.Client_Loan_Infos.Where(x=>x.AccountID==clientId).ToList();
                foreach (var cli in clientLoanInfos)
                {
                    cli.Read_Note = true;
                }
                db.SubmitChanges();
            }
            return Json(new { clientId=clientId},JsonRequestBehavior.AllowGet);  
        }

    }
}
