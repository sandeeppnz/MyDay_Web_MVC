using SANSurveyWebAPI.BLL;
using SANSurveyWebAPI.Models;
using SANSurveyWebAPI.Models.Api;
using SANSurveyWebAPI.ViewModels.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SANSurveyWebAPI.DTOs;
using System.Data.Entity;

namespace SANSurveyWebAPI.Controllers
{
    public class KidsController : BaseController
    {
        private Survey3Service surveyService;
        private UserHomeService userHomeService;
        private ProfileService profileService;
        private KidsService kidsService;

        private ApplicationDbContext db = new ApplicationDbContext();

        public KidsController()
        {
            this.surveyService = new Survey3Service();
            this.userHomeService = new UserHomeService();
            this.profileService = new ProfileService();
            this.kidsService = new KidsService();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            { }
            base.Dispose(disposing);
        }
        // GET: Kids
        public ActionResult Index()
        {
            Session["ProgressValueValue"] = (double) Constants.ProgressBarValue.Zero;
            return View();
        }

        #region "MAIN KIDS SURVEY - WELCOME SCREEN"

        [HttpGet]
        public ActionResult KidsSurvey(string Id)
        {
            KidsSurveyVM k = new KidsSurveyVM();
            k.CurrentDate = String.Format(format: "{0:dddd, d MMMM, yyyy}", arg0: DateTime.Today);
            Session["SurveyDate"] = k.CurrentDate;
            Session["KidsSurveySpan"] = "3pm - 7pm";
            return View(k); }

        //Here were generating the temporary user id and creating a kids survey for them for demo purposes.
        //Upon creation it will take user to New Kids Survey layout
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> KidsSurvey()
        {
            try {
                string currentSurveyID = string.Empty;
                string demoForUser = "kids";

                if (ModelState.IsValid)
                {
                    #region I.Create Random Kids Profile
                    //Generate random email
                    string kidsEmail = string.Empty;
                    Random randomGenerator = new Random();
                    int randomNumber = randomGenerator.Next(6999);
                    kidsEmail = demoForUser + randomNumber + "@aut.ac.nz";
                    Session["kidsEmail"] = kidsEmail;
                    Session["kidsPassword"] = "******";

                    Profile newProfile = new Profile();
                    //Genrate UID
                    Guid uid = Guid.NewGuid();

                    //Which Client
                    if (demoForUser == "kids")
                    {
                        newProfile.ClientInitials = "kids";
                        newProfile.ClientName = "John and Hopkins";
                    }                  

                    //Encryption
                    newProfile.LoginEmail = StringCipher.EncryptRfc2898(kidsEmail);

                    newProfile.CreatedDateTimeUtc = DateTime.UtcNow;
                    newProfile.RegistrationProgressNext = Constants.StatusRegistrationProgress.Completed.ToString();
                    newProfile.UserId = db.Users.Where(x => x.Email == Constants.AdminEmail.ToString()).SingleOrDefault().Id;
                    newProfile.Uid = uid.ToString();
                    newProfile.Name = demoForUser + randomNumber;
                    newProfile.MaxStep = 10;
                    newProfile.OffSetFromUTC = 13;
                    newProfile.Incentive = 0;
                    newProfile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                    newProfile.RegisteredDateTimeUtc = DateTime.UtcNow;
                    db.Profiles.Add(newProfile);
                    await db.SaveChangesAsync();
                    db.Entry(newProfile).GetDatabaseValues(); //Get PROFILE

                    Session["ProfileId"] = newProfile.Id;
                    Session["UID"] = newProfile.Uid;
                    ViewBag.UserId = new SelectList(db.Users, "Id", "Email", newProfile.UserId);
                    //----End of Create Profile (will generate Profile ID)

                    #endregion

                    #region II. Generate Survey
                    //---------------------------------------------
                    //Previous Day Work Shift from 9 to 5
                    DateTime currentDate = DateTime.Now;
                    DateTime previousDate = currentDate.Date.AddDays(-1);
                    previousDate = previousDate.AddHours(9);
                    DateTime endDate = currentDate.Date.AddDays(-1);
                    endDate = endDate.Date.AddHours(18);

                    DateTime previousDateUTC = previousDate.AddHours(-13);
                    DateTime EndDateUTC = endDate.AddHours(-13);

                    KidsSurvey kids = new KidsSurvey();
                    kids.ProfileId = newProfile.Id;
                    kids.Uid = newProfile.Uid;
                    kids.SurveyProgress = Constants.KidsSurveyStatus.NewSurvey.ToString();
                    kids.SurveyDate = previousDate;
                    Session["KidsSurveyDate"] = previousDate;
                    kids.EntryStartCurrent = DateTime.Now;
                    //Session["KSStartDateTime"] = kids.EntryStartCurrent;
                    kids.EntryStartUTC = DateTime.UtcNow;
                    db.KidsSurveys.Add(kids);
                    await db.SaveChangesAsync();

                    #endregion                    
                }
                return RedirectToAction("KidsSignUp", "Account", new { uid = Session["UID"].ToString() });
            }
            catch (Exception ex)
            {
                string EMsg = "KidsSurvey:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }

        #endregion

        #region FOR WIZARD INITIALIZATION

        public async void InitializeWizard(string progress, int maxStep)
        {
            try {
                int profileId = (int) Session["ProfileId"];
                int KidsSurveyId = (int) Session["KidsSurveyId"];

                //for setting Wizard values and status update

                var kidsProfile = kidsService.GetKidsProfile(profileId);
                kidsProfile.MaxStepKidsSurvey = maxStep;
                kidsProfile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                kidsService.UpdateKidsProfile(kidsProfile);

                var kidsSurveyUpdate = kidsService.GetKidsSurvey(KidsSurveyId);                
                kidsSurveyUpdate.SurveyProgress = progress;
                kidsService.UpdateKidsSurvey(kidsSurveyUpdate);
                //end of Wizard update
            }
            catch (Exception ex)
            {
                string EMsg = "InitializeWizard:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");                
            }
        }
        public void GetProfileSession()
        {
            try {
                int profileId = (int) Session["ProfileId"];
                KidsProfileUpdatesVM k = new KidsProfileUpdatesVM();
                if (Session["ProfileSession"] == null)
                { Session["ProfileSession"] = kidsService.GetProfileById(profileId); }
                ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                k.ProfileId = profileId;
                k.MaxStepKidsSurvey = profile.MaxStepKidsSurvey;
            }
            catch { }
        }
        #endregion

        #region "NEW KIDS SURVEY SCREEN"

        [HttpGet]
        public async Task<ActionResult> NewSurvey(string uid)
        {
            try
            {
                int profileId = (int) Session["ProfileId"];
                Session["ProgressValueValue"] = (double)Constants.ProgressBarValue.Zero;

                GetProfileSession();
                if (!string.IsNullOrEmpty(uid))
                {
                    var z = db.KidsSurveys.Where(u => u.Uid == uid
                                                    && u.ProfileId == profileId)
                                           .Select(u => u).FirstOrDefault();

                    Session["KidsSurveyId"] = z.Id;
                }
                else { Session["KidsSurveyId"] = ""; }
                return View();
            }
            catch (Exception ex)
            {
                string EMsg = "NewSurvey:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> NewSurvey()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int profileId = (int) Session["ProfileId"];
                    int KidsSurveyId = (int) Session["KidsSurveyId"];

                    InitializeWizard(Constants.KidsSurveyStatus.Location.ToString(), 1);

                    Session["StartKidsWindow"] = "3:00 pm";

                    return RedirectToAction("Location");
                }
                catch (Exception ex)
                {
                    string EMsg = "NewSurvey:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        #endregion
                     
      
                                    
        #region LOCATION

        [HttpGet]
        public async Task<ActionResult> Location()
        {            
            try
            {
                if (Session["KidsSurveyId"] != null)
                {
                    int kidsSurveyId = (int) Session["KidsSurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    int hours, mins;
                    GetProfileSession();

                    KidsLocationVM k = new KidsLocationVM();                    

                    //setting checkbox values
                    k.LocationOptionList = kidsService.GetLocationOptions();

                    //checking values in Locations table to perform following logic
                    var t = db.KidsLocations.Where(u => u.ProfileId == profileId
                                                    && u.KidsSurveyId == kidsSurveyId)
                                            .Select(u => u).ToList().OrderByDescending(u => u.LocationSequence);                   

                    if (t.Count() == 0)
                    {
                        //Initializing locationsequence to 1 when the survey is started                        
                        Session["LocationCount"] = 1;                        
                    }
                    else if (t.Count() > 0)
                    {
                        //here if location details are saved for particular profile with specific survey Id
                        // then using the last saved locationsequence to increment and save it in session
                        //this will process till the total hours doesn't equal to 4 hours
                        var c = t.OrderByDescending(u => u.LocationSequence).First();
                        int lc = c.LocationSequence;// (int) Session["LocationCount"];
                        lc++;
                        Session["LocationCount"] = lc;                        
                    }

                    int locationSequence = (int) Session["LocationCount"];
                    if (IsEven(locationSequence))
                    {
                        k.PageQuestion = "At "+ Session["StartKidsWindow"].ToString() + ",where did you go next?";
                        Session["ProgressValueValue"] = (double)Constants.ProgressBarValue.Thirty;
                    }
                    else if (IsOdd(locationSequence))
                    {
                        k.PageQuestion = "Thinking back to " + Session["StartKidsWindow"].ToString() + ", where were you?";
                        Session["ProgressValueValue"] = (double)Constants.ProgressBarValue.Ten;
                    }
                   //string a =  CalculateTimeStamp("17:59 pm");
                    if (Request.IsAjaxRequest())
                    { return PartialView(k); }
                    return View(k);
                }
                return null;
            }
            catch (Exception ex)
            {
                string EMsg = "Kids Location GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }        
        [HttpPost]
        public async Task<ActionResult> Location(KidsLocationVM k)
        {
            try
            {
                if (Session["KidsSurveyId"] != null)
                {
                    int kidsSurveyId = (int) Session["KidsSurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    int locationSequence = (int) Session["LocationCount"];
                    string locationName = string.Empty;
                    int locationId, seq = 0; string otherlocname = string.Empty;
                    IDictionary<int?, string> iDict = new Dictionary<int?, string>();
                    iDict = null;

                    var z = db.KidsLocations.Where(u => u.ProfileId == profileId
                                                    && u.KidsSurveyId == kidsSurveyId
                                                    && u.LocationSequence == locationSequence)
                                            .Select(u => u).ToList();
                       
                    KidsLocationDto f = new KidsLocationDto();
                    f.ProfileId = profileId;
                    f.KidsSurveyId = kidsSurveyId;

                    f.Location = k.selectedLocation;
                    f.OtherLocation = k.OtherLocation;                       
                    f.LocationSequence = locationSequence;

                    if (IsOdd(locationSequence))
                    {
                        if (!string.IsNullOrEmpty(k.selectedLocation))
                            Session["FromLocation"] = k.selectedLocation;
                        else
                            Session["FromLocation"] = k.OtherLocation;

                        if (k.selectedLocation == "Other")
                        {
                            Session["FromOtherLocation"] = k.OtherLocation;
                            otherlocname = Session["FromOtherLocation"].ToString();
                        }

                        locationName = Session["FromLocation"].ToString();
                        seq = 1;
                    }
                    else if (IsEven(locationSequence))
                    {
                        if (!string.IsNullOrEmpty(k.selectedLocation))
                            Session["ToLocation"] = k.selectedLocation;
                        else
                            Session["ToLocation"] = k.OtherLocation;

                        if (k.selectedLocation == "Other")
                        {
                            Session["FromOtherLocation"] = k.OtherLocation;
                            otherlocname = Session["FromOtherLocation"].ToString();
                        }

                        locationName = Session["ToLocation"].ToString();
                        seq = 3;
                    }
                    
                    //EMoTracking
                    locationId = kidsService.SaveKidsLocation(f);
                    Session["LocationId"] = locationId;

                    //UpdateEMoTrack(locationId, "location", null, null, null, locationName, 
                    //    null, iDict, null, true, false, seq,null, otherlocname, null);
                    //End of EmoTracking
                    
                    if (IsOdd(locationSequence))
                    {
                        InitializeWizard(Constants.KidsSurveyStatus.TimeSpent.ToString(), 2);
                        return RedirectToAction("TimeSpent");
                    }
                    else if (IsEven(locationSequence))
                    {
                        InitializeWizard(Constants.KidsSurveyStatus.Travel.ToString(), 4);
                        return RedirectToAction("Travel");
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                string EMsg = "Kids Location POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }
        
        #endregion

        #region TIME SPENT

        [HttpGet]
        public async Task<ActionResult> TimeSpent()
        {            
            try
            {
                if (Session["KidsSurveyId"] != null)
                {
                    KidsTimeSpentVM k = new KidsTimeSpentVM();
                    int locationSequence = (int) Session["LocationCount"];

                    if (IsOdd(locationSequence))
                    {
                        k.Location = Session["FromLocation"].ToString();
                        Session["ProgressValueValue"] = (double)Constants.ProgressBarValue.Twenty;
                    }
                    else if (IsEven(locationSequence))
                    {
                        k.Location = Session["ToLocation"].ToString();
                        Session["ProgressValueValue"] = (double)Constants.ProgressBarValue.Fifty;
                    }

                    if (k.Location == "Other")
                        k.OtherLocation = Session["FromOtherLocation"].ToString();

                    if (Request.IsAjaxRequest())
                    { return PartialView(k); }
                    return View(k);
                }
                return null;
            }
            catch (Exception ex)
            {
                string EMsg = "Kids TimeSpent GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> TimeSpent(KidsTimeSpentVM k)
        {
            try
            {
                if (Session["KidsSurveyId"] != null)
                {
                    int kidsSurveyId = (int) Session["KidsSurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    int locationSequence = (int) Session["LocationCount"];
                    int locationId = (int) Session["LocationId"];

                    int totalHours = 0, totalMins = 0, totalTime = 0;
                    string startedAtTime = Session["StartKidsWindow"].ToString();
                    int startHour = 0; 
                    //DateTime dt, dts, dte;
                    //bool res = DateTime.TryParse("01:00 PM", out dt);

                    string locationName = string.Empty;

                    //Fetching data using locationsequence number and updating the hours and minutes spent
                    //by kids in this location
                    var m = db.KidsLocations.Where(u => u.ProfileId == profileId
                                                    && u.KidsSurveyId == kidsSurveyId
                                                    && u.Id == locationId
                                                    && u.LocationSequence == locationSequence)
                                            .Select(u => u).FirstOrDefault();

                    locationName = (m.Location != "Other") ? m.Location : m.OtherLocation;
                    
                    m.TimeSpentInHours = k.TotalHours;
                    m.TimeSpentInMins = k.TotalMins;
                    //bool res = DateTime.TryParse(startedAtTime, out dts);
                    //m.StartedAt = dts.ToShortTimeString();//startedAtTime;                    
                    m.StartedAt = startedAtTime;

                    IList<string> duraCal = startedAtTime.Split(' ').Reverse().ToList<string>();
                    string strColon = ":";
                    int hrs = 0, mins = 0, actualmins = 0; string actualExp = string.Empty;
                    if (duraCal[1].ToString().Any(strColon.Contains))
                    {
                        IList<string> hrmin = duraCal[1].ToString().Split(':').Reverse().ToList<string>();
                        int endHr = Convert.ToInt32(hrmin[1].ToString()) + k.TotalHours;
                        startHour = endHr;

                        int endMin = Convert.ToInt32(hrmin[0].ToString()) + k.TotalMins;

                        if (endMin > 60)
                        {
                            hrs = endMin / 60;
                            mins = endMin % 60;
                            //actualmins = mins < 10 ? Convert.ToInt32(("0" + mins)) : mins;

                            actualmins = mins < 10 ? 0 : 1;
                            actualExp = actualmins == 0 ? ("0" + mins.ToString()) : mins.ToString();


                            m.EndedAt = (startHour + hrs).ToString() + ":" + actualExp + " pm";
                        }
                        else if (endMin < 10)
                        {
                            //actualmins = endMin < 10 ? Convert.ToInt32(("0" + endMin)) : endMin;

                            actualmins = endMin < 10 ? 0 : 1;
                            actualExp = actualmins == 0 ? ("0" + endMin.ToString()) : endMin.ToString();

                            m.EndedAt = (startHour + hrs).ToString() + ":" + actualExp + " pm";
                        }
                        else { m.EndedAt = startHour.ToString() + ":" + endMin + " pm"; }                        
                    }
                    else
                    {
                        startHour = Convert.ToInt32(duraCal[1].ToString()) + k.TotalHours;

                        if (k.TotalMins != 0)
                        {
                            m.EndedAt = startHour.ToString() + ":" + k.TotalMins + " pm";
                        }
                        else { m.EndedAt = startHour.ToString() + ":00 pm"; }
                    }

                    m.EndedAt = CalculateTimeStamp(m.EndedAt);

                    Session["StartKidsWindow"] = m.EndedAt;
                    Session["KidsSurveySpan"] = "3pm - " + m.EndedAt;
                    db.Entry(m).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    //end of updating the same

                    //UpdateEMoTrack(locationId, "location", m.StartedAt,
                    //    m.EndedAt, null, locationName, null,null,null,false,false, null, null,null,null);

                    if (IsOdd(locationSequence))
                    {
                        InitializeWizard(Constants.KidsSurveyStatus.ToLocation.ToString(), 3);
                        return RedirectToAction("Location");
                    }
                    else if (IsEven(locationSequence))
                    {
                        InitializeWizard(Constants.KidsSurveyStatus.LocationSummary.ToString(), 6);
                        return RedirectToAction("LocationSummary");
                    }
                   
                    //end of calculations based on total time
                }
                return null;
            }
            catch (Exception ex)
            {
                string EMsg = "Kids TimeSpent POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }

        #endregion                   

        #region TRAVEL

        [HttpGet]
        public async Task<ActionResult> Travel()
        {
            try
            {
                if (Session["KidsSurveyId"] != null)
                {
                    int kidsSurveyId = (int) Session["KidsSurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    int locationSequence = (int) Session["LocationCount"];

                    KidsTravelVM k = new KidsTravelVM();

                    var getLocation = kidsService.GetKidsLocationByProfileAndSurveyId(profileId,kidsSurveyId);

                    if (getLocation.Count() > 0)
                    {
                        k.FromLocation = getLocation.Where(u => u.LocationSequence == 1).Select(u => u.Location).FirstOrDefault().ToString();                        
                        k.HiddFromLocationId = getLocation.Where(u => u.LocationSequence == 1).Select(u => u.Id).FirstOrDefault();

                        if(k.FromLocation == "Other")
                            k.OtherFromLocation = getLocation.Where(u => u.LocationSequence == 1).Select(u => u.OtherLocation).FirstOrDefault().ToString();

                        if (IsEven(locationSequence))
                        {
                            k.ToLocation = getLocation.Where(u => u.LocationSequence == 2).Select(u => u.Location).FirstOrDefault().ToString();
                            k.HiddToLocationId = getLocation.Where(u => u.LocationSequence == 2).Select(u => u.Id).FirstOrDefault();

                            if (k.ToLocation == "Other")
                                k.OtherToLocation  = getLocation.Where(u => u.LocationSequence == 2).Select(u => u.OtherLocation).FirstOrDefault().ToString();
                        }
                    }
                    k.TransportOptionList = kidsService.GetTransportOptions();
                    Session["ProgressValueValue"] = (double)Constants.ProgressBarValue.Fourty;

                    if (Request.IsAjaxRequest())
                    { return PartialView(k); }
                    return View(k);
                }
                return null;
            }
            catch (Exception ex)
            {
                string EMsg = "Kids TimeSpent GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> Travel(KidsTravelVM k)
        {
            try {
                if (Session["KidsSurveyId"] != null)
                {
                    int kidsSurveyId = (int) Session["KidsSurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    string travelStartedAt = Session["StartKidsWindow"].ToString();
                    int travelstartHour = 0;
                    int travelId;

                    Session["SequenceEmo"] = 1;

                    KidsTravelDto v = new KidsTravelDto();
                    v.ProfileId = profileId;
                    v.KidsSurveyId = kidsSurveyId;
                    v.FromLocationId = k.HiddFromLocationId;
                    v.ToLocationId = k.HiddToLocationId;
                    v.ModeOfTransport = k.HiddenTransport;
                    v.OtherModeOfTransport = k.OtherTransport;
                    v.TravelTimeInHours = k.TravelInHours;
                    v.TravelTimeInMins = k.TravelInMins;
                    v.TravelStartedAt = travelStartedAt;

                    IList<string> duraCal = travelStartedAt.Split(' ').Reverse().ToList<string>();
                    string strColon = ":";
                    int hrs = 0, mins = 0, actualmins = 0; string actualExp = string.Empty;
                    if (duraCal[1].ToString().Any(strColon.Contains))
                    {
                        IList<string> hrmin = duraCal[1].ToString().Split(':').Reverse().ToList<string>();
                        int endHr = Convert.ToInt32(hrmin[1].ToString()) + k.TravelInHours;
                        travelstartHour = endHr;

                        int endMin = Convert.ToInt32(hrmin[0].ToString()) + k.TravelInMins;

                        if (endMin > 60)
                        {
                            hrs = endMin / 60;
                            mins = endMin % 60;

                            actualmins = mins < 10 ? 0 : 1;
                            actualExp = actualmins == 0 ? ("0" + mins.ToString()) : mins.ToString();

                            v.TravelEndedAt = (travelstartHour + hrs).ToString() + ":" + actualExp + " pm";
                        }
                        else if (endMin < 10)
                        {
                            //actualmins = endMin < 10 ? Convert.ToInt32(("0" + endMin)) : endMin;

                            actualmins = endMin < 10 ? 0 : 1;
                            actualExp = actualmins == 0 ? ("0" + endMin.ToString()) : endMin.ToString();

                            v.TravelEndedAt = (travelstartHour + hrs).ToString() + ":" + actualExp + " pm";
                        }
                        else { v.TravelEndedAt = travelstartHour.ToString() + ":" + endMin + " pm";}                        
                    }
                    else {
                        travelstartHour = Convert.ToInt32(duraCal[1].ToString()) + k.TravelInHours;

                        if (k.TravelInMins != 0)
                        {
                            v.TravelEndedAt = travelstartHour.ToString() + ":" + k.TravelInMins + " pm";
                        }
                        else { v.TravelEndedAt = travelstartHour.ToString() + ":00 pm"; }
                    }
                    v.TravelEndedAt = CalculateTimeStamp(v.TravelEndedAt);

                    Session["StartKidsWindow"] = v.TravelEndedAt;
                    Session["KidsSurveySpan"] = "3pm - " + v.TravelEndedAt;
                    travelId = kidsService.SaveKidsTravelDetails(v);

                    UpdateEMoTrack(travelId, "travel", v.TravelStartedAt, v.TravelEndedAt, 
                                    k.ToLocation, k.OtherToLocation, v.ModeOfTransport,
                                    v.OtherModeOfTransport, null, null, "during");
                    //UpdateEMoTrack("travel", v.TravelStartedAt, v.TravelEndedAt, travelId, null, -1, null, true);
                    return RedirectToAction("TimeSpent");
                }
            }
            catch (Exception ex)
            {
                string EMsg = "Kids Travel POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
            return View(); }
        
        #endregion

        #region LOCATION SUMMARY

        [HttpGet]
        public async Task<ActionResult> LocationSummary()
        {
            KidsLocationSummaryVM v = new KidsLocationSummaryVM();
            try
            {
                if (Session["KidsSurveyId"] != null)
                {
                    int kidsSurveyId = (int) Session["KidsSurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    v.SurveyDate = String.Format(format: "{0:dddd, d MMMM, yyyy}", arg0: DateTime.Today);

                    v.AllKidsLocationListObj = kidsService.GetKidsLocationByProfileAndSurveyId(profileId, kidsSurveyId).ToList();
                    v.AllKidsTravelListObj = kidsService.GetKidsTravelByProfileAndSurveyId(profileId, kidsSurveyId);

                    var fn = v.AllKidsLocationListObj.Where(u => u.LocationSequence == 1)
                                .Select(u => u).First();

                    Session["LongFromLocation"] = (fn.Location == "Other") ? fn.OtherLocation : fn.Location;

                    var tn = v.AllKidsLocationListObj.Where(u => u.LocationSequence == 2)
                                .Select(u => u).First();

                    Session["LongToLocation"] = (tn.Location == "Other") ? tn.OtherLocation : tn.Location;

                    var tr = v.AllKidsTravelListObj.First();

                    Session["LongMOT"] = (tr.ModeOfTransport == "Other") ? tr.OtherModeOfTransport : tr.ModeOfTransport;


                    foreach (var r in v.AllKidsLocationListObj)
                    {
                        if (IsOdd(r.LocationSequence))
                        { v.StartTime = r.StartedAt; }
                        if (IsEven(r.LocationSequence))
                        { v.EndTime = r.EndedAt; }
                    }
                }
            }
            catch (Exception ex)
            {
                string EMsg = "Kids LocationSummary GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
            return View(v);
        }
        [HttpPost]
        public async Task<ActionResult> LocationSummary(KidsLocationSummaryVM v)
        {
            try {
                InitializeWizard(Constants.KidsSurveyStatus.Tasks1.ToString(), 7);
                return RedirectToAction("Tasks1");
            }
            catch (Exception ex)
            {
                string EMsg = "Kids LocationSummary POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
            return View(v);
        }

        #endregion

        #region TASKS

        [HttpGet]
        public async Task<ActionResult> Tasks1()
        {
            try
            {
                if (Session["KidsSurveyId"] != null)
                {
                    int kidsSurveyId = (int) Session["KidsSurveyId"];
                    int profileId = (int) Session["ProfileId"];

                    Session["ProgressValueValue"] = (double)Constants.ProgressBarValue.Sixty;

                    KidsTasksLocationVM k = new KidsTasksLocationVM();

                    //setting checkbox values
                    k.TaskOptionList = kidsService.GetTaskOptions();

                    var tasksEntered = kidsService.GetKidsLocationByProfileAndSurveyId(profileId, kidsSurveyId);

                    var checkTasks = tasksEntered.Where(u => u.IsTasksEntered == false)
                                                 .OrderBy(u => u.LocationSequence)
                                                 .Select(u => u).FirstOrDefault();

                    k.QLocationId = checkTasks.Id;

                    if (!string.IsNullOrEmpty(checkTasks.Location) && checkTasks.Location != "Other")
                        k.QLocation = checkTasks.Location;
                    else
                    {
                        k.QLocation = checkTasks.Location;
                        k.QOtherLocation = checkTasks.OtherLocation;
                    }

                    k.SpentStartTime = checkTasks.StartedAt;
                    k.SpentEndTime = checkTasks.EndedAt;
                   
                    if (Request.IsAjaxRequest())
                    { return PartialView(k); }
                    return View(k);
                }
                return null;
            }
            catch (Exception ex)
            {
                string EMsg = "Kids Tasks1 GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> Tasks1(KidsTasksLocationVM k)
        {
            try
            {
                if (Session["KidsSurveyId"] != null)
                {
                    int kidsSurveyId = (int) Session["KidsSurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    int taskId; string taskName = string.Empty;
                    string oTasks = string.Empty;

                    //for entering the multiple tasks in the kids tasks table
                    IList<string> TotalTasks = k.SelectedTasks.Split(',').Reverse().ToList<string>();
                    IDictionary<int?, string> ArrayOfTasks = new Dictionary<int?,string>();

                    foreach (var r in TotalTasks)
                    {
                        KidsTasksOnLocationDto v = new KidsTasksOnLocationDto();
                        v.ProfileId = profileId;
                        v.KidsSurveyId = kidsSurveyId;
                        v.KidsLocationId = k.QLocationId;
                        v.LocationName = k.QLocation;
                        v.OtherLocationName = k.QOtherLocation;
                        v.SpentStartTime = k.SpentStartTime;
                        v.SpentEndTime = k.SpentEndTime;
                        v.TasksDone = r;
                        if (r == "Other")
                        { v.TaskOther = k.OtherTasks; }
                        else v.TaskOther = null;

                        taskId = kidsService.SaveKidsTasksOnLocation(v);
                        //for sending the taskIds to emo tracking table
                        ArrayOfTasks.Add(taskId, r);
                    }
                    taskName = k.SelectedTasks;

                    //Upon entering the tasks, in tracking table inserting true for task entered
                    var te = db.KidsLocations.Where(u => u.Id == k.QLocationId).Select(u => u).First();
                    te.IsTasksEntered = true;
                    db.Entry(te).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    UpdateEMoTrack(k.QLocationId, "location", k.SpentStartTime, 
                                k.SpentEndTime, k.QLocation, k.QOtherLocation, null, null,
                        ArrayOfTasks, k.OtherTasks, "from");

                    InitializeWizard(Constants.KidsSurveyStatus.Tasks2.ToString(), 8);

                    return RedirectToAction("Tasks2");
                    if (Request.IsAjaxRequest())
                    { return PartialView(k); }
                    return View(k);
                }
                return null;
            }
            catch (Exception ex)
            {
                string EMsg = "Kids Tasks1 POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }
        [HttpGet]
        public async Task<ActionResult> Tasks2()
        {
            try
            {
                if (Session["KidsSurveyId"] != null)
                {
                    int kidsSurveyId = (int) Session["KidsSurveyId"];
                    int profileId = (int) Session["ProfileId"];

                    Session["ProgressValueValue"] = (double)Constants.ProgressBarValue.Seventy;

                    KidsTasksLocationVM k = new KidsTasksLocationVM();

                    //setting checkbox values
                    k.TaskOptionList = kidsService.GetTaskOptions();

                    var tasksEntered = kidsService.GetKidsLocationByProfileAndSurveyId(profileId, kidsSurveyId);

                    var checkTasks = tasksEntered.Where(u => u.IsTasksEntered == false)
                                                 .OrderBy(u => u.LocationSequence)
                                                 .Select(u => u).FirstOrDefault();

                    k.QLocationId = checkTasks.Id;

                    if (!string.IsNullOrEmpty(checkTasks.Location) && checkTasks.Location != "Other")
                        k.QLocation = checkTasks.Location;
                    else
                    {
                        k.QLocation = checkTasks.Location;
                        k.QOtherLocation = checkTasks.OtherLocation;
                    }

                    k.SpentStartTime = checkTasks.StartedAt;
                    k.SpentEndTime = checkTasks.EndedAt;

                    if (Request.IsAjaxRequest())
                    { return PartialView(k); }
                    return View(k);
                }
                return null;
            }
            catch (Exception ex)
            {
                string EMsg = "Kids Tasks2 GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> Tasks2(KidsTasksLocationVM k)
        {
            try
            {
                if (Session["KidsSurveyId"] != null)
                {
                    int kidsSurveyId = (int) Session["KidsSurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    int taskId; string taskName = string.Empty;
                    string oTasks = string.Empty;

                    //for entering the multiple tasks in the kids tasks table
                    IList<string> TotalTasks = k.SelectedTasks.Split(',').Reverse().ToList<string>();
                    IDictionary<int?, string> ArrayOfTasks = new Dictionary<int?, string>();

                    foreach (var r in TotalTasks)
                    {
                        KidsTasksOnLocationDto v = new KidsTasksOnLocationDto();
                        v.ProfileId = profileId;
                        v.KidsSurveyId = kidsSurveyId;
                        v.KidsLocationId = k.QLocationId;
                        v.LocationName = k.QLocation;
                        v.OtherLocationName = k.QOtherLocation;
                        v.SpentStartTime = k.SpentStartTime;
                        v.SpentEndTime = k.SpentEndTime;
                        v.TasksDone = r;
                        if (r == "Other")
                        { v.TaskOther = k.OtherTasks; }
                        else v.TaskOther = null;

                        taskId = kidsService.SaveKidsTasksOnLocation(v);
                        //for sending the taskIds to emo tracking table
                        ArrayOfTasks.Add(taskId, r);
                    }
                    taskName = k.SelectedTasks;

                    //Upon entering the tasks, in tracking table inserting true for task entered
                    var te = db.KidsLocations.Where(u => u.Id == k.QLocationId).Select(u => u).First();
                    te.IsTasksEntered = true;
                    db.Entry(te).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    UpdateEMoTrack(k.QLocationId, "location", k.SpentStartTime,
                               k.SpentEndTime, k.QLocation, k.QOtherLocation, null, null,
                       ArrayOfTasks, k.OtherTasks, "to");

                    InitializeWizard(Constants.KidsSurveyStatus.Tasks2.ToString(), 8);

                    return RedirectToAction("TaskSummaries");
                    if (Request.IsAjaxRequest())
                    { return PartialView(k); }
                    return View(k);
                }
                return null;
            }
            catch (Exception ex)
            {
                string EMsg = "Kids Tasks2 POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }

        #endregion

        #region TASK SUMMARIES

        [HttpGet]
        public async Task<ActionResult> TaskSummaries()
        {
            KidsLocationSummaryVM v = new KidsLocationSummaryVM();
            try
            {
                if (Session["KidsSurveyId"] != null)
                {
                    int kidsSurveyId = (int) Session["KidsSurveyId"];
                    int profileId = (int) Session["ProfileId"];

                    v.SurveyDate = String.Format(format: "{0:dddd, d MMMM, yyyy}", arg0: DateTime.Today);

                    v.AllKidsLocationListObj = kidsService.GetKidsLocationByProfileAndSurveyId(profileId, kidsSurveyId).ToList();
                    v.AllKidsTravelListObj = kidsService.GetKidsTravelByProfileAndSurveyId(profileId, kidsSurveyId);
                    v.AllKidsTasksOnLocationListObj = kidsService.GetKidsTasksByProfileAndSurveyId(profileId, kidsSurveyId);

                    foreach (var r in v.AllKidsLocationListObj)
                    {
                        if (IsOdd(r.LocationSequence))
                        { v.StartTime = r.StartedAt; }
                        if (IsEven(r.LocationSequence))
                        { v.EndTime = r.EndedAt; }
                    }
                    string strColon = ",";
                    IList<TasksByLocation> atl = new List<TasksByLocation>();                    

                    foreach (var r in v.AllKidsTasksOnLocationListObj)
                    {
                        if (r.TasksDone.Any(strColon.Contains))
                        {
                            IList<string> tasks = r.TasksDone.Split(',').Reverse().ToList<string>();
                            if (!string.IsNullOrEmpty(r.TaskOther))
                            {
                                tasks.Add("Other: " +r.TaskOther);
                                tasks.Remove(tasks[0]);
                            }

                            foreach (var i in tasks)
                            {
                                TasksByLocation tl = new TasksByLocation();
                                tl.TaskLocId = r.Id;
                                tl.LocationId = r.KidsLocationId;
                                tl.LocationName = r.LocationName;
                                if (r.TasksDone == "Other")
                                    tl.TaskName = r.TaskOther;
                                else tl.TaskName = i;
                                atl.Add(tl);
                            }
                        }
                        else
                        {
                            TasksByLocation tl = new TasksByLocation();
                            tl.TaskLocId = r.Id;
                            tl.LocationId = r.KidsLocationId;
                            tl.LocationName = r.LocationName;
                            if (r.TasksDone == "Other")
                                tl.TaskName = r.TaskOther;
                            else tl.TaskName = r.TasksDone;
                            atl.Add(tl);
                        }
                    }

                    v.AllKidsTasksByLocationObj = atl;

                    Session["pvCalValue"] = 0;
                }
            }
            catch (Exception ex)
            {
                string EMsg = "Kids TaskSummaries GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
            return View(v);
        }
        [HttpPost]
        public async Task<ActionResult> TaskSummaries(KidsLocationSummaryVM v)
        {
            try {
                return RedirectToAction("ReactO");
            }
            catch (Exception ex)
            {
                string EMsg = "Kids TaskSummaries POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
            return View();
        }
        #endregion

        #region KIDS REACTION

        [HttpGet]
        public async Task<ActionResult> ReactO()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["KidsSurveyId"] != null)
                    {
                        KidsReactOVM v = new KidsReactOVM();

                        int kidsSurveyId = (int) Session["KidsSurveyId"];
                        int profileId = (int) Session["ProfileId"];
                        string uid = Session["UID"].ToString();
                        Double pv = Convert.ToDouble(Session["ProgressValueValue"]);
                        string otherT = string.Empty;
                        Double pvCal = 0; int pvDiff = 0;

                        if (Session["pvCalValue"].ToString() != "" && Session["pvCalValue"].ToString() != "0")
                        { pvCal = Convert.ToDouble(Session["pvCalValue"]); }

                        v.KidsEMoTrackListObj = db.KidsEmoTracked
                                                  .Where(u => u.ProfileId == profileId
                                                            && u.KidsSurveyId == kidsSurveyId
                                                            && u.IsEmoAffStageCompleted == false)
                                                  .Select(u => u).ToList();

                        if (pvCal == 0)
                        {
                            if (v.KidsEMoTrackListObj.Count() != 0)
                            {
                                pvDiff = 97 - Convert.ToInt32(pv);
                                pvCal = pvDiff / v.KidsEMoTrackListObj.Count();
                                Session["ProgressValueValue"] = pv + pvCal;
                                Session["pvCalValue"] = pvCal;
                            }
                            else
                            {
                                pv = pv + 9;
                                Session["ProgressValueValue"] = pv;
                            }
                        }
                        else
                        {
                            Session["ProgressValueValue"] = pv + pvCal;
                        }
                        if (v.KidsEMoTrackListObj.Count() != 0)
                        {
                            var firstRow = v.KidsEMoTrackListObj.OrderBy(u => u.SequenceToQEmo).First();
                            v.QStartedAt = (firstRow.TaskStartTime).Trim();
                            v.QEndedAt = (firstRow.TaskEndTime).Trim().ToString() + ",";
                            if (firstRow.TaskPerformed != null)
                                v.QTasks = firstRow.TaskPerformed;
                            else v.QTasks = "";

                            if(firstRow.LocationName == "Other")
                            { v.QLocation = firstRow.OtherLocationName; }
                            else v.QLocation = firstRow.LocationName;

                            v.KidsEmoTrackId = firstRow.Id;

                            v.KidsLocationId = firstRow.KidsLocationId;
                            v.KidsTravelId = firstRow.KidsTravelId;
                            v.TaskId = firstRow.KidsTaskId;
                            v.SurveyDate = firstRow.SurveyDate;
                            v.TaskName = firstRow.TaskPerformed;

                            v.LongTaskWhile = firstRow.TaskWhile;
                            v.LongTravelDetails = firstRow.TravelDetails;
                            if (firstRow.TaskPerformed == "Other")
                            { otherT = firstRow.OtherTask; }
                            else otherT = null;
                            v.IfOtherMOT = firstRow.ModeOfTransport;

                        }
                        string mod = string.Empty;
                        mod = Session["LongMOT"].ToString();

                        if (v.IfOtherMOT != "Other" && v.IfOtherMOT != null)
                        {
                            var lmot = kidsService.GetTransportOptions();
                            string lt = lmot.Where(u => u.Value == mod).Select(u => u.LongName).First().ToString();
                            v.LongNameModeOfTransport = lt.ToString();
                        }
                        else if (v.IfOtherMOT == null)
                        { v.LongNameModeOfTransport = ""; }
                        else
                        { v.LongNameModeOfTransport = mod.ToString(); }

                        if (v.QTasks != "")
                        {
                            string ts = string.Empty;
                            var taasks = kidsService.GetTaskOptions();
                            if (v.QTasks != "Other" && v.QTasks != "other")
                            {
                                ts = taasks.Where(u => u.Value == v.QTasks).Select(u => u.LongName).First().ToString();
                                v.LongTaskActivity = ts.ToString() + " " + otherT;
                            }
                            else if (v.QTasks == "Other" || v.QTasks == "other")
                            { v.LongTaskActivity = otherT; }
                            else { v.LongTaskActivity = ts.ToString(); }
                        }

                        v.LongFromLocation = Session["LongFromLocation"].ToString();
                        v.LongToLocation = Session["LongToLocation"].ToString();

                        v.Uid = Session["UID"].ToString();
                        v.ResponseStartTime = DateTime.Now;

                        v.Q1Ans = Constants.NA_7Rating;
                        v.Q2Ans = Constants.NA_7Rating;
                        v.Q3Ans = Constants.NA_7Rating;
                        v.Q4Ans = Constants.NA_7Rating;
                        v.Q5Ans = Constants.NA_7Rating;

                        if (Request.IsAjaxRequest())
                        { return PartialView(v); }
                        return View(v);
                    }
                    await LogMyDayError(Session["UID"].ToString(), "ReactO GET: Survey UID not found!", "SurveyError");
                    return RedirectToAction("SurveyError");
                }
                catch (Exception ex)
                {
                    string EMsg = "ReactO GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            return View();
        }        
        [HttpPost]
        public async Task<ActionResult> ReactO(KidsReactOVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int kidsSurveyId = (int) Session["KidsSurveyId"];
                    int profileId = (int) Session["ProfileId"];

                    #region KIDS-REACTION

                    KidsReactionDto r1 = new KidsReactionDto();
                    r1.ProfileId = profileId;
                    r1.KidsSurveyId = kidsSurveyId;
                    r1.SurveyDate = v.SurveyDate;
                    r1.ResponseStartTime = v.ResponseStartTime;
                    r1.ResponseEndTime = DateTime.Now;
                    r1.KidsLocationId = v.KidsLocationId;
                    r1.KidsTravelId = v.KidsTravelId;
                    r1.KidsTaskId = v.TaskId;
                    r1.KidsEmoTrackId = v.KidsEmoTrackId;
                    r1.TasksPerformed = v.TaskName;
                    r1.QuestionId = v.Q1DB;
                    r1.Answer = Constants.GetText7ScaleRating(v.Q1Ans);
                    r1.TaskStartTime = v.QStartedAt;
                    r1.TaskEndTime = v.QEndedAt;                   
                    kidsService.AddKidsReaction(r1);

                    KidsReactionDto r2 = new KidsReactionDto();
                    r2.ProfileId = profileId;
                    r2.KidsSurveyId = kidsSurveyId;
                    r2.SurveyDate = v.SurveyDate;
                    r2.ResponseStartTime = v.ResponseStartTime;
                    r2.ResponseEndTime = DateTime.Now;
                    r2.KidsLocationId = v.KidsLocationId;
                    r2.KidsTravelId = v.KidsTravelId;
                    r2.KidsTaskId = v.TaskId;
                    r2.KidsEmoTrackId = v.KidsEmoTrackId;
                    r2.TasksPerformed = v.TaskName;
                    r2.QuestionId = v.Q2DB;
                    r2.Answer = Constants.GetText7ScaleRating(v.Q2Ans);
                    r2.TaskStartTime = v.QStartedAt;
                    r2.TaskEndTime = v.QEndedAt;
                    kidsService.AddKidsReaction(r2);

                    KidsReactionDto r3 = new KidsReactionDto();
                    r3.ProfileId = profileId;
                    r3.KidsSurveyId = kidsSurveyId;
                    r3.SurveyDate = v.SurveyDate;
                    r3.ResponseStartTime = v.ResponseStartTime;
                    r3.ResponseEndTime = DateTime.Now;
                    r3.KidsLocationId = v.KidsLocationId;
                    r3.KidsTravelId = v.KidsTravelId;
                    r3.KidsTaskId = v.TaskId;
                    r3.KidsEmoTrackId = v.KidsEmoTrackId;
                    r3.TasksPerformed = v.TaskName;
                    r3.QuestionId = v.Q3DB;
                    r3.Answer = Constants.GetText7ScaleRating(v.Q3Ans);
                    r3.TaskStartTime = v.QStartedAt;
                    r3.TaskEndTime = v.QEndedAt;
                    kidsService.AddKidsReaction(r3);

                    KidsReactionDto r4 = new KidsReactionDto();
                    r4.ProfileId = profileId;
                    r4.KidsSurveyId = kidsSurveyId;
                    r4.SurveyDate = v.SurveyDate;
                    r4.ResponseStartTime = v.ResponseStartTime;
                    r4.ResponseEndTime = DateTime.Now;
                    r4.KidsLocationId = v.KidsLocationId;
                    r4.KidsTravelId = v.KidsTravelId;
                    r4.KidsTaskId = v.TaskId;
                    r4.KidsEmoTrackId = v.KidsEmoTrackId;
                    r4.TasksPerformed = v.TaskName;
                    r4.QuestionId = v.Q4DB;
                    r4.Answer = Constants.GetText7ScaleRating(v.Q4Ans);
                    r4.TaskStartTime = v.QStartedAt;
                    r4.TaskEndTime = v.QEndedAt;
                    kidsService.AddKidsReaction(r4);

                    KidsReactionDto r5 = new KidsReactionDto();
                    r5.ProfileId = profileId;
                    r5.KidsSurveyId = kidsSurveyId;
                    r5.SurveyDate = v.SurveyDate;
                    r5.ResponseStartTime = v.ResponseStartTime;
                    r5.ResponseEndTime = DateTime.Now;
                    r5.KidsLocationId = v.KidsLocationId;
                    r5.KidsTravelId = v.KidsTravelId;
                    r5.KidsTaskId = v.TaskId;
                    r5.KidsEmoTrackId = v.KidsEmoTrackId;
                    r5.TasksPerformed = v.TaskName;
                    r5.QuestionId = v.Q5DB;
                    r5.Answer = Constants.GetText7ScaleRating(v.Q5Ans);
                    r5.TaskStartTime = v.QStartedAt;
                    r5.TaskEndTime = v.QEndedAt;
                    kidsService.AddKidsReaction(r5);
                    
                    #endregion

                    kidsService.SaveKidsReaction();

                    return RedirectToAction("ReactT");
                }
                catch (Exception ex)
                {
                    string EMsg = "ReactO POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            { return PartialView(v); }
            return View(v);
        }
        [HttpGet]
        public async Task<ActionResult> ReactT()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["KidsSurveyId"] != null)
                    {
                        KidsReactTVM v = new KidsReactTVM();

                        int kidsSurveyId = (int) Session["KidsSurveyId"];
                        int profileId = (int) Session["ProfileId"];
                        string uid = Session["UID"].ToString();
                        string otherT = string.Empty;

                        v.KidsEMoTrackListObj = db.KidsEmoTracked
                                                  .Where(u => u.ProfileId == profileId
                                                            && u.KidsSurveyId == kidsSurveyId
                                                            && u.IsEmoAffStageCompleted == false)
                                                  .Select(u => u).ToList();

                        if (v.KidsEMoTrackListObj.Count() != 0)
                        {
                            var firstRow = v.KidsEMoTrackListObj.OrderBy(u => u.SequenceToQEmo).First();
                            v.QStartedAt = firstRow.TaskStartTime;
                            v.QEndedAt = (firstRow.TaskEndTime).Trim().ToString() + ",";
                            if (firstRow.TaskPerformed != null)
                                //v.QTasks = (firstRow.TaskPerformed).ToLower().ToString();
                                v.QTasks = firstRow.TaskPerformed;
                            else v.QTasks = "";

                            if (firstRow.LocationName == "Other")
                            { v.QLocation = firstRow.OtherLocationName; }
                            else v.QLocation = firstRow.LocationName;

                            v.KidsEmoTrackId = firstRow.Id;

                            v.KidsLocationId = firstRow.KidsLocationId;
                            v.KidsTravelId = firstRow.KidsTravelId;
                            v.TaskId = firstRow.KidsTaskId;
                            v.SurveyDate = firstRow.SurveyDate;
                            v.TaskName = firstRow.TaskPerformed;

                            v.LongTaskWhile = firstRow.TaskWhile;
                            v.LongTravelDetails = firstRow.TravelDetails;
                            otherT = firstRow.OtherTask;
                            v.IfOtherMOT = firstRow.ModeOfTransport;
                        }
                        string mod = string.Empty;
                        mod = Session["LongMOT"].ToString();

                        if (v.IfOtherMOT != "Other" && v.IfOtherMOT != null)
                        {
                            var lmot = kidsService.GetTransportOptions();
                            string lt = lmot.Where(u => u.Value == mod).Select(u => u.LongName).First().ToString();
                            v.LongNameModeOfTransport = lt.ToString();
                        }
                        else if (v.IfOtherMOT == null)
                        { v.LongNameModeOfTransport = ""; }
                        else
                        { v.LongNameModeOfTransport = mod.ToString(); }
                        if (v.QTasks != "")
                        {
                            string ts = string.Empty;
                            var taasks = kidsService.GetTaskOptions();
                            if (v.QTasks != "Other" && v.QTasks != "other")
                            {
                                ts = taasks.Where(u => u.Value == v.QTasks).Select(u => u.LongName).First().ToString();
                                v.LongTaskActivity = ts.ToString() + " " + otherT;
                            }
                            else if (v.QTasks == "Other" || v.QTasks == "other")
                            { v.LongTaskActivity = otherT; }
                            else { v.LongTaskActivity = ts.ToString(); }
                        }

                        v.LongFromLocation = Session["LongFromLocation"].ToString();
                        v.LongToLocation = Session["LongToLocation"].ToString();

                        v.Uid = Session["UID"].ToString();
                        v.ResponseStartTime = DateTime.Now;

                        v.Q6Ans = Constants.NA_7Rating;
                        v.Q7Ans = Constants.NA_7Rating;
                        v.Q8Ans = Constants.NA_7Rating;
                        v.Q9Ans = Constants.NA_7Rating;
                        v.Q10Ans = Constants.NA_7Rating;
                        v.Q12Ans = Constants.NA_7Rating;

                        if (Request.IsAjaxRequest())
                        { return PartialView(v); }
                        return View(v);
                    }
                    await LogMyDayError(Session["UID"].ToString(), "ReactT GET: Survey UID not found!", "SurveyError");
                    return RedirectToAction("SurveyError");
                }
                catch (Exception ex)
                {
                    string EMsg = "ReactT GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> ReactT(KidsReactTVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int kidsSurveyId = (int) Session["KidsSurveyId"];
                    int profileId = (int) Session["ProfileId"];

                    #region KIDS-REACTION

                    KidsReactionDto r6 = new KidsReactionDto();
                    r6.ProfileId = profileId;
                    r6.KidsSurveyId = kidsSurveyId;
                    r6.SurveyDate = v.SurveyDate;
                    r6.ResponseStartTime = v.ResponseStartTime;
                    r6.ResponseEndTime = DateTime.Now;
                    r6.KidsLocationId = v.KidsLocationId;
                    r6.KidsTravelId = v.KidsTravelId;
                    r6.KidsTaskId = v.TaskId;
                    r6.KidsEmoTrackId = v.KidsEmoTrackId;
                    r6.TasksPerformed = v.TaskName;
                    r6.QuestionId = v.Q6DB;
                    r6.Answer = Constants.GetText7ScaleRating(v.Q6Ans);
                    r6.TaskStartTime = v.QStartedAt;
                    r6.TaskEndTime = v.QEndedAt;
                    kidsService.AddKidsReaction(r6);

                    KidsReactionDto r7 = new KidsReactionDto();
                    r7.ProfileId = profileId;
                    r7.KidsSurveyId = kidsSurveyId;
                    r7.SurveyDate = v.SurveyDate;
                    r7.ResponseStartTime = v.ResponseStartTime;
                    r7.ResponseEndTime = DateTime.Now;
                    r7.KidsLocationId = v.KidsLocationId;
                    r7.KidsTravelId = v.KidsTravelId;
                    r7.KidsTaskId = v.TaskId;
                    r7.KidsEmoTrackId = v.KidsEmoTrackId;
                    r7.TasksPerformed = v.TaskName;
                    r7.QuestionId = v.Q7DB;
                    r7.Answer = Constants.GetText7ScaleRating(v.Q7Ans);
                    r7.TaskStartTime = v.QStartedAt;
                    r7.TaskEndTime = v.QEndedAt;
                    kidsService.AddKidsReaction(r7);

                    KidsReactionDto r8 = new KidsReactionDto();
                    r8.ProfileId = profileId;
                    r8.KidsSurveyId = kidsSurveyId;
                    r8.SurveyDate = v.SurveyDate;
                    r8.ResponseStartTime = v.ResponseStartTime;
                    r8.ResponseEndTime = DateTime.Now;
                    r8.KidsLocationId = v.KidsLocationId;
                    r8.KidsTravelId = v.KidsTravelId;
                    r8.KidsTaskId = v.TaskId;
                    r8.KidsEmoTrackId = v.KidsEmoTrackId;
                    r8.TasksPerformed = v.TaskName;
                    r8.QuestionId = v.Q8DB;
                    r8.Answer = Constants.GetText7ScaleRating(v.Q8Ans);
                    r8.TaskStartTime = v.QStartedAt;
                    r8.TaskEndTime = v.QEndedAt;
                    kidsService.AddKidsReaction(r8);

                    KidsReactionDto r9 = new KidsReactionDto();
                    r9.ProfileId = profileId;
                    r9.KidsSurveyId = kidsSurveyId;
                    r9.SurveyDate = v.SurveyDate;
                    r9.ResponseStartTime = v.ResponseStartTime;
                    r9.ResponseEndTime = DateTime.Now;
                    r9.KidsLocationId = v.KidsLocationId;
                    r9.KidsTravelId = v.KidsTravelId;
                    r9.KidsTaskId = v.TaskId;
                    r9.KidsEmoTrackId = v.KidsEmoTrackId;
                    r9.TasksPerformed = v.TaskName;
                    r9.QuestionId = v.Q9DB;
                    r9.Answer = Constants.GetText7ScaleRating(v.Q9Ans);
                    r9.TaskStartTime = v.QStartedAt;
                    r9.TaskEndTime = v.QEndedAt;
                    kidsService.AddKidsReaction(r9);

                    KidsReactionDto r10 = new KidsReactionDto();
                    r10.ProfileId = profileId;
                    r10.KidsSurveyId = kidsSurveyId;
                    r10.SurveyDate = v.SurveyDate;
                    r10.ResponseStartTime = v.ResponseStartTime;
                    r10.ResponseEndTime = DateTime.Now;
                    r10.KidsLocationId = v.KidsLocationId;
                    r10.KidsTravelId = v.KidsTravelId;
                    r10.KidsTaskId = v.TaskId;
                    r10.KidsEmoTrackId = v.KidsEmoTrackId;
                    r10.TasksPerformed = v.TaskName;
                    r10.QuestionId = v.Q10DB;
                    r10.Answer = Constants.GetText7ScaleRating(v.Q10Ans);
                    r10.TaskStartTime = v.QStartedAt;
                    r10.TaskEndTime = v.QEndedAt;
                    kidsService.AddKidsReaction(r10);
                     
                    KidsReactionDto r12 = new KidsReactionDto();
                    r12.ProfileId = profileId;
                    r12.KidsSurveyId = kidsSurveyId;
                    r12.SurveyDate = v.SurveyDate;
                    r12.ResponseStartTime = v.ResponseStartTime;
                    r12.ResponseEndTime = DateTime.Now;
                    r12.KidsLocationId = v.KidsLocationId;
                    r12.KidsTravelId = v.KidsTravelId;
                    r12.KidsTaskId = v.TaskId;
                    r12.KidsEmoTrackId = v.KidsEmoTrackId;
                    r12.TasksPerformed = v.TaskName;
                    r12.QuestionId = v.Q12DB;
                    r12.Answer = Constants.GetText7ScaleRating(v.Q12Ans);
                    r12.TaskStartTime = v.QStartedAt;
                    r12.TaskEndTime = v.QEndedAt;
                    kidsService.AddKidsReaction(r12);

                    #endregion

                    kidsService.SaveKidsReaction();

                    var emoStageComp = db.KidsEmoTracked.Where(u => u.ProfileId == profileId
                                                                    && u.KidsSurveyId == kidsSurveyId
                                                                    && u.Id == v.KidsEmoTrackId)
                                                        .Select(u => u).First();
                    emoStageComp.IsEmoAffStageCompleted = true;
                    db.Entry(emoStageComp).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    var totalEmo = db.KidsEmoTracked.Where(u => u.ProfileId == profileId
                                                                    && u.KidsSurveyId == kidsSurveyId
                                                                    && u.IsEmoAffStageCompleted == false)
                                                         .Select(u => u).ToList();
                    if (totalEmo.Count() > 0)
                        return RedirectToAction("ReactO");
                    else
                        return RedirectToAction("Feedback");
                }
                catch (Exception ex)
                {
                    string EMsg = "ReactO POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            { return PartialView(v); }
            return View(v);
        }
        #endregion

        #region "FEEDBACK"

        public async Task<ActionResult> Feedback()
        {
            try
            {
                if (Session["KidsSurveyId"] != null)
                {
                    int kidsSurveyId = (int) Session["KidsSurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    Session["ProgressValueValue"] = (double)Constants.ProgressBarValue.Hundred;

                    KidsFeedbackVM v = new KidsFeedbackVM();


                    if (Request.IsAjaxRequest())
                    { return PartialView(v); }
                    return View(v);
                }
                return null;
            }
            catch (Exception ex)
            {
                string EMsg = "Kids Feedback GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Feedback(KidsFeedbackVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int kidsSurveyId = (int) Session["KidsSurveyId"];
                    int profileId = (int) Session["ProfileId"];

                    KidsFeedbackDto f = new KidsFeedbackDto();
                    f.ProfileId = profileId;
                    f.KidsSurveyId = kidsSurveyId;
                    f.Comments = v.Comment;
                    kidsService.AddKidsFeedback(f);
                    kidsService.SaveKidsFeedback();

                    //return RedirectToAction("Summary");
                    return RedirectToAction("EmoSummary");
                }

                catch (Exception ex)
                {
                    string EMsg = "Kids Feedback POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            { return PartialView(v); }
            return View(v);
        }
        #endregion

        #region SUMMARY

        [HttpGet]
        public async Task<ActionResult> Summary()
        {
            try
            {
                if (Session["KidsSurveyId"] != null)
                {
                    int kidsSurveyId = (int) Session["KidsSurveyId"];
                    int profileId = (int) Session["ProfileId"];

                    AllSummarziedVM v = new AllSummarziedVM();                    

                    var KidsComments = kidsService.GetKidsCommentByProfileandSurveyId(profileId, kidsSurveyId);
                    v.Comment = KidsComments.Comments;

                    //GET KIDS TASKLIST PERFORMED
                    //KidsSurveyTaskDurationVM tdresponse = new KidsSurveyTaskDurationVM();
                    //tdresponse.kidsTasksObj = new List<KidsTasks>();
                    //tdresponse.kidsTasksObj = kidsService.GetKidsTaskByProfileandSurveyId(profileId, kidsSurveyId).ToList();
                    //Session["TaskList"] = tdresponse.kidsTasksObj;//to be used on Edit Response

                    //GET KIDS REACTION
                    var genericReaction = kidsService.GetGenericKidsReactions(kidsSurveyId, profileId);

                    KidsReactOVM ref1 = new KidsReactOVM();
                    KidsReactTVM ref2 = new KidsReactTVM();
                    Dictionary<string, string> refTbl = new Dictionary<string, string>();

                    refTbl.Add(ref1.Q1DB, ref1.Q1DisplayShort);
                    refTbl.Add(ref1.Q2DB, ref1.Q2DisplayShort);
                    refTbl.Add(ref1.Q3DB, ref1.Q3DisplayShort);
                    refTbl.Add(ref1.Q4DB, ref1.Q4DisplayShort);
                    refTbl.Add(ref1.Q5DB, ref1.Q5DisplayShort);
                    refTbl.Add(ref2.Q6DB, ref2.Q6DisplayShort);
                    refTbl.Add(ref2.Q7DB, ref2.Q7DisplayShort);
                    refTbl.Add(ref2.Q8DB, ref2.Q8DisplayShort);
                    refTbl.Add(ref2.Q9DB, ref2.Q9DisplayShort);
                    refTbl.Add(ref2.Q10DB, ref2.Q10DisplayShort);
                    refTbl.Add(ref2.Q12DB, ref2.Q12DisplayShort);

                    List<Tuple<string, KidsReactionVM>> genericList = new List<Tuple<string, KidsReactionVM>>();

                    //Generic
                    foreach (var k in genericReaction)
                    {
                        KidsReactionVM r = new KidsReactionVM();

                        var emoTrack = db.KidsEmoTracked.Where(u => u.ProfileId == profileId
                                                                    && u.KidsSurveyId == kidsSurveyId
                                                                    && u.Id == k.KidsEmoTrackId)
                                                        .Select(u => u).Single();                    

                        r.Location = emoTrack.LocationName;
                        r.OtherLocation = emoTrack.OtherLocationName;
                        r.TaskPerformed = emoTrack.TaskPerformed;
                        r.StartTime = emoTrack.TaskStartTime;
                        r.EndTime = emoTrack.TaskEndTime;
                        r.SurveyDate = emoTrack.SurveyDate;
                        r.Id = emoTrack.Id;
                        r.Question = refTbl[k.QuestionId].Trim(); //get display qns
                        r.Answer = k.Answer;
                        r.RatingString = surveyService.GetRatingString(k.Answer);

                        r.EmoId = emoTrack.Id;
                        r.OtherModeOfTransport = emoTrack.OtherModeOfTransport;
                        r.KidsTravelId = emoTrack.KidsTravelId;
                        r.ModeOfTransport = emoTrack.ModeOfTransport;
                        r.OtherTasks = emoTrack.OtherTask;
                        r.KidsLocationId = emoTrack.KidsLocationId;

                        genericList.Add(Tuple.Create(r.TaskPerformed + " STARTDATESAN " + emoTrack.TaskStartTime, r));
                    }
                    v.FullResponseList = (Lookup<string, KidsReactionVM>) genericList.ToLookup(t => t.Item1, t => t.Item2);


                    if (Request.IsAjaxRequest())
                    { return PartialView(v); }
                    return View(v);
                }
                return null;
            }
            catch (Exception ex)
            {
                string EMsg = "Kids SUMMARY GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }

        #endregion

        #region EMOSUMMARY

        [HttpGet]
        public async Task<ActionResult> EmoSummary()
        {
            try
            {
                if (Session["KidsSurveyId"] != null)
                {
                    int kidsSurveyId = (int) Session["KidsSurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    int? travelSeq = 0, fromL = 0, toL = 0;
                    EmoSummaryVM emoVM = new EmoSummaryVM();

                    //Getting KidsComments to add to EmoSummaryVM list
                    var KidsComments = kidsService.GetKidsCommentByProfileandSurveyId(profileId, kidsSurveyId);
                    emoVM.Comment = KidsComments.Comments;

                    var emoTrackCount = db.KidsEmoTracked.Where(u => u.ProfileId == profileId
                                                                    && u.KidsSurveyId == kidsSurveyId) 
                                                        .Select(u => u)
                                                        .ToList();
                    if (emoTrackCount.Count() != 0)
                    {
                        //from travel details
                        var TravelDet = emoTrackCount.Where(u => u.KidsTravelId != null
                                                && u.TaskWhile == "travel"
                                                && u.TravelDetails == "during").Select(u => u).First();
                        travelSeq = TravelDet.KidsTravelId;
                        
                        //From location details
                        var FromDet = emoTrackCount.Where(u => u.TaskWhile == "location"
                                                && u.KidsTravelId == null
                                                && u.TravelDetails == "from").Select(u => u).First();
                        fromL = FromDet.KidsLocationId;
                        emoVM.FromLocationName = (FromDet.LocationName == "Other")? FromDet.OtherLocationName : FromDet.LocationName;

                        //To location details
                        var ToDet = emoTrackCount.Where(u => u.TaskWhile == "location"
                                               && u.KidsTravelId == null
                                               && u.TravelDetails == "to").Select(u => u).First();

                        toL = ToDet.KidsLocationId;
                        emoVM.ToLocationName = (ToDet.LocationName == "Other")? ToDet.OtherLocationName : ToDet.LocationName;
                        emoVM.TravelFromTo = "Traveling from " + emoVM.FromLocationName + " to " + emoVM.ToLocationName;
                    }
                    //Getting all the KidsReaction based on profileId and KidsSurveyId
                    var genericTravel = kidsService.GetGenericKidsReactionsByTravelId(kidsSurveyId, profileId, travelSeq);                    

                    var genericFromLoc = kidsService.GetGenericKidsReactionsByLocationId(kidsSurveyId, profileId, fromL);
                    var genericToLoc = kidsService.GetGenericKidsReactionsByLocationId(kidsSurveyId, profileId, toL);

                    //Creating a dictionary object to save all the values from KidsReaction Table
                    KidsReactOVM ref1 = new KidsReactOVM();
                    KidsReactTVM ref2 = new KidsReactTVM();
                    Dictionary<string, string> refTbl = new Dictionary<string, string>();

                    refTbl.Add(ref1.Q1DB, ref1.Q1DisplayShort);
                    refTbl.Add(ref1.Q2DB, ref1.Q2DisplayShort);
                    refTbl.Add(ref1.Q3DB, ref1.Q3DisplayShort);
                    refTbl.Add(ref1.Q4DB, ref1.Q4DisplayShort);
                    refTbl.Add(ref1.Q5DB, ref1.Q5DisplayShort);
                    refTbl.Add(ref2.Q6DB, ref2.Q6DisplayShort);
                    refTbl.Add(ref2.Q7DB, ref2.Q7DisplayShort);
                    refTbl.Add(ref2.Q8DB, ref2.Q8DisplayShort);
                    refTbl.Add(ref2.Q9DB, ref2.Q9DisplayShort);
                    refTbl.Add(ref2.Q10DB, ref2.Q10DisplayShort);
                    refTbl.Add(ref2.Q12DB, ref2.Q12DisplayShort);

                    #region TRAVEL LIST
                    //Creating a genericlist to send out to view
                    List<Tuple<string, KidsReactionVM>> genericTravelList = new List<Tuple<string, KidsReactionVM>>();
                    //Generic Reactions
                    foreach (var k in genericTravel)
                    {

                        KidsReactionVM kidsReactVM = new KidsReactionVM();
                        var emoTrack = db.KidsEmoTracked.Where(u => u.ProfileId == profileId
                                                                    && u.KidsSurveyId == kidsSurveyId
                                                                    && u.KidsTravelId == travelSeq
                                                                    && u.KidsTravelId != null
                                                                    && u.TravelDetails == "during")
                                                            .Select(u => u)
                                                            .ToList();

                        if (emoTrack.Count() != 0)
                        {
                            foreach (var i in emoTrack)
                            {
                                kidsReactVM.Location = i.LocationName;
                                kidsReactVM.OtherLocation = i.OtherLocationName;
                                kidsReactVM.TaskPerformed = i.TaskPerformed;
                                kidsReactVM.StartTime = i.TaskStartTime;
                                kidsReactVM.EndTime = i.TaskEndTime;
                                kidsReactVM.SurveyDate = i.SurveyDate;
                                kidsReactVM.Id = i.Id;
                                kidsReactVM.Question = refTbl[k.QuestionId].Trim(); //get display qns
                                kidsReactVM.Answer = k.Answer;
                                kidsReactVM.RatingString = surveyService.GetRatingString(k.Answer);

                                kidsReactVM.EmoId = i.Id;
                                kidsReactVM.OtherModeOfTransport = i.OtherModeOfTransport;
                                kidsReactVM.KidsTravelId = i.KidsTravelId;
                                kidsReactVM.ModeOfTransport = i.ModeOfTransport;
                                kidsReactVM.OtherTasks = i.OtherTask;
                                kidsReactVM.KidsLocationId = i.KidsLocationId;

                                genericTravelList.Add(Tuple.Create(kidsReactVM.TaskPerformed + " STARTDATESAN " + i.TaskStartTime, kidsReactVM));
                            }
                        }
                    }
                    emoVM.FullTravelList = (Lookup<string, KidsReactionVM>) genericTravelList.ToLookup(t => t.Item1, t => t.Item2);

                    #endregion

                    #region FROM LOCATION

                    //Creating a genericlist to send out to view
                    List<Tuple<string, KidsReactionVM>> genericFromLocList = new List<Tuple<string, KidsReactionVM>>();
                    //Generic Reactions
                    foreach (var k in genericFromLoc)
                    {
                        KidsReactionVM kidsReactVM = new KidsReactionVM();

                        var emoTrack = db.KidsEmoTracked.Where(u => u.ProfileId == profileId
                                                                    && u.KidsSurveyId == kidsSurveyId
                                                                    && u.KidsLocationId == fromL
                                                                    && u.TravelDetails == "from"
                                                                    && u.KidsTaskId == k.KidsTaskId)
                                                        .Select(u => u)
                                                        .ToList();
                        if (emoTrack.Count() != 0)
                        {
                            foreach (var i in emoTrack)
                            {
                                kidsReactVM.Location = i.LocationName;
                                kidsReactVM.OtherLocation = i.OtherLocationName;
                                kidsReactVM.TaskPerformed = i.TaskPerformed;
                                kidsReactVM.StartTime = i.TaskStartTime;
                                kidsReactVM.EndTime = i.TaskEndTime;
                                kidsReactVM.SurveyDate = i.SurveyDate;
                                kidsReactVM.Id = i.Id;
                                kidsReactVM.Question = refTbl[k.QuestionId].Trim(); //get display qns
                                kidsReactVM.Answer = k.Answer;
                                kidsReactVM.RatingString = surveyService.GetRatingString(k.Answer);

                                kidsReactVM.EmoId = i.Id;
                                kidsReactVM.OtherModeOfTransport = i.OtherModeOfTransport;
                                kidsReactVM.KidsTravelId = i.KidsTravelId;
                                kidsReactVM.ModeOfTransport = i.ModeOfTransport;
                                kidsReactVM.OtherTasks = i.OtherTask;
                                kidsReactVM.KidsLocationId = i.KidsLocationId;

                                genericFromLocList.Add(Tuple.Create(kidsReactVM.TaskPerformed + " STARTDATESAN " + i.TaskStartTime, kidsReactVM));
                            }
                        }
                    }
                    emoVM.FullFromLocationList = (Lookup<string, KidsReactionVM>) genericFromLocList.ToLookup(t => t.Item1, t => t.Item2);

                    #endregion

                    #region TO LOCATION LIST

                    //Creating a genericlist to send out to view
                    List<Tuple<string, KidsReactionVM>> genericToLocList = new List<Tuple<string, KidsReactionVM>>();
                    //Generic Reactions
                    foreach (var k in genericToLoc)
                    {
                        KidsReactionVM kidsReactVM = new KidsReactionVM();

                        var emoTrack = db.KidsEmoTracked.Where(u => u.ProfileId == profileId
                                                                    && u.KidsSurveyId == kidsSurveyId
                                                                    && u.KidsLocationId == toL
                                                                    && u.TravelDetails == "to"
                                                                    && u.KidsTaskId == k.KidsTaskId)
                                                        .Select(u => u)
                                                        .ToList();
                        if (emoTrack.Count() != 0)
                        {
                            foreach (var i in emoTrack)
                            {
                                kidsReactVM.Location = i.LocationName;
                                kidsReactVM.OtherLocation = i.OtherLocationName;
                                kidsReactVM.TaskPerformed = i.TaskPerformed;
                                kidsReactVM.StartTime = i.TaskStartTime;
                                kidsReactVM.EndTime = i.TaskEndTime;
                                kidsReactVM.SurveyDate = i.SurveyDate;
                                kidsReactVM.Id = i.Id;
                                kidsReactVM.Question = refTbl[k.QuestionId].Trim(); //get display qns
                                kidsReactVM.Answer = k.Answer;
                                kidsReactVM.RatingString = surveyService.GetRatingString(k.Answer);

                                kidsReactVM.EmoId = i.Id;
                                kidsReactVM.OtherModeOfTransport = i.OtherModeOfTransport;
                                kidsReactVM.KidsTravelId = i.KidsTravelId;
                                kidsReactVM.ModeOfTransport = i.ModeOfTransport;
                                kidsReactVM.OtherTasks = i.OtherTask;
                                kidsReactVM.KidsLocationId = i.KidsLocationId;

                                genericToLocList.Add(Tuple.Create(kidsReactVM.TaskPerformed + " STARTDATESAN " + i.TaskStartTime, kidsReactVM));
                            }
                        }
                    }
                    emoVM.FullToLocationList = (Lookup<string, KidsReactionVM>) genericToLocList.ToLookup(t => t.Item1, t => t.Item2);

                    #endregion

                    if (Request.IsAjaxRequest())
                    { return PartialView(emoVM); }
                    return View(emoVM);
                }
                return null;
            }
            catch (Exception ex)
            {
                string EMsg = "Kids SUMMARY GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }


        #endregion

        #region EDIT REACTION

        [HttpGet]
        public async Task<ActionResult> EditReaction(int emoId, DateTime taskStartDate)
        {
            string suid = string.Empty;
            try
            {
                KidsEditReactionVM v = new KidsEditReactionVM();

                int kidsSurveyId = (int) Session["KidsSurveyId"];
                int profileId = (int) Session["ProfileId"];                

                var emoDetails = db.KidsEmoTracked.Where(u => u.Id == emoId).Select(u => u).First();

                v.StartTime = emoDetails.TaskStartTime;
                v.EndTime = emoDetails.TaskEndTime;

                v.TaskName = emoDetails.TaskPerformed;
                v.EMoID = emoId;

                bool isAny = false;

                var listOfReactions = kidsService.GetAllKidsReactions(profileId, kidsSurveyId, emoId);
                var response = listOfReactions.Where(t => t.QuestionId == v.Q1DB).SingleOrDefault();

                if (response != null)
                {
                    v.Q1Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else { v.Q1Ans = Constants.NA_7Rating; }
                response = listOfReactions.Where(t => t.QuestionId == v.Q2DB).SingleOrDefault();

                if (response != null)
                { v.Q2Ans = Constants.GetInt7ScaleRating(response.Answer); isAny = true; }
                else { v.Q2Ans = Constants.NA_7Rating; }

                response = listOfReactions.Where(t => t.QuestionId == v.Q3DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q3Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else { v.Q3Ans = Constants.NA_7Rating; }

                response = listOfReactions.Where(t => t.QuestionId == v.Q4DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q4Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else { v.Q4Ans = Constants.NA_7Rating; }

                response = listOfReactions.Where(t => t.QuestionId == v.Q5DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q5Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else { v.Q5Ans = Constants.NA_7Rating; }

                response = listOfReactions.Where(t => t.QuestionId == v.Q6DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q6Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else { v.Q6Ans = Constants.NA_7Rating; }

                response = listOfReactions.Where(t => t.QuestionId == v.Q7DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q7Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else { v.Q7Ans = Constants.NA_7Rating; }

                response = listOfReactions.Where(t => t.QuestionId == v.Q8DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q8Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else { v.Q8Ans = Constants.NA_7Rating; }

                response = listOfReactions.Where(t => t.QuestionId == v.Q9DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q9Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else { v.Q9Ans = Constants.NA_7Rating; }

                response = listOfReactions.Where(t => t.QuestionId == v.Q10DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q10Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else { v.Q10Ans = Constants.NA_7Rating; }

                if (isAny)
                { v.IsExist = true; }

                if (Request.IsAjaxRequest())
                { return PartialView(v); }
                return View(v);
            }
            catch (Exception ex)
            {
                string EMsg = "Kids EDIT-REACTION GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> EditReaction(KidsEditReactionVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int kidsSurveyId = (int) Session["KidsSurveyId"];
                    int profileId = (int) Session["ProfileId"];                   

                    // Save to db
                    #region update Question  
                    var listOfReactions = kidsService.GetAllKidsReactions(profileId, kidsSurveyId, v.EMoID);
                    var response = listOfReactions.Where(r => r.QuestionId == v.Q1DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q1Ans);
                        kidsService.UpdateKidsReaction(response);
                    }

                    response = listOfReactions.Where(r => r.QuestionId == v.Q2DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q2Ans);
                        kidsService.UpdateKidsReaction(response);
                    }
                    response = listOfReactions.Where(r => r.QuestionId == v.Q3DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q3Ans);
                        kidsService.UpdateKidsReaction(response);
                    }
                    response = listOfReactions.Where(r => r.QuestionId == v.Q4DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q4Ans);
                        kidsService.UpdateKidsReaction(response);
                    }
                    response = listOfReactions.Where(r => r.QuestionId == v.Q5DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q5Ans);
                        kidsService.UpdateKidsReaction(response);
                    }
                    response = listOfReactions.Where(r => r.QuestionId == v.Q6DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q6Ans);
                        kidsService.UpdateKidsReaction(response);
                    }
                    response = listOfReactions.Where(r => r.QuestionId == v.Q7DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q7Ans);
                        kidsService.UpdateKidsReaction(response);
                    }
                    response = listOfReactions.Where(r => r.QuestionId == v.Q8DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q8Ans);
                        kidsService.UpdateKidsReaction(response);
                    }
                    response = listOfReactions.Where(r => r.QuestionId == v.Q9DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q9Ans);
                        kidsService.UpdateKidsReaction(response);
                    }
                    response = listOfReactions.Where(r => r.QuestionId == v.Q10DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q10Ans);
                        kidsService.UpdateKidsReaction(response);
                    }
                    kidsService.SaveKidsResponses();
                    #endregion                    
                    return RedirectToAction("EmoSummary");
                }
                catch (Exception ex)
                {
                    string EMsg = "Kids EDIT-REACTION POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            { return PartialView(v); }
            return View(v);
        }

        #endregion

        #region "LOG ERRORS TO DB"

        private async Task LogMyDayError(string uid, string errorMsg, string action)
        {
            var mydayVM = new MyDayErrorLogViewModel();

            var mydayDto = new MyDayErrorLogs_Dto();
            if (Session["ProfileId"].ToString() != string.Empty || Session["ProfileId"].ToString() != null)
            { mydayDto.ProfileId = Convert.ToInt32(Session["ProfileId"].ToString()); }

            mydayDto.SurveyUID = uid.ToString();
            mydayDto.AccessedDateTime = DateTime.Now;
            mydayDto.ErrorMessage = errorMsg;
            mydayDto.HtmlContent = action;

            if (mydayDto != null)
            {
                mydayVM.SurveyUID = mydayDto.SurveyUID;
                mydayVM.AccessedDateTime = mydayDto.AccessedDateTime;
                mydayVM.ErrorMessage = mydayDto.ErrorMessage;
                mydayVM.HtmlContent = mydayDto.HtmlContent;
            }
            await surveyService.Save_MyDayErrorLogs(mydayDto);
        }
        [HttpGet]
        public ActionResult SurveyError()
        {
            if (Request.IsAjaxRequest())
            { return PartialView(); }
            return View();
        }
        [HttpPost]
        public ActionResult SurveyError(SurveyError v)
        {
            if (Request.IsAjaxRequest())
            { return RedirectToAction("Index", "Home"); }
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region MULTIPLE METHOD CALL

        public static bool IsOdd(int value)
        {
            return value % 2 != 0;
        }
        public static bool IsEven(int value)
        {
            return value % 2 == 0;
        }
        public void UpdateEMoTrack(Nullable<int> locOrTravelId, string taskWhile, 
                                   string taskStartTime, string taskEndTime,
                                   string locationName, string otherLocationName, 
                                   string modeOfTransport, string otherModeOfTransport, 
                                   IDictionary<int?, string> ta, string otherTask, string traveldetails)
        {
            //function called to save record in track table for the purpose of emotional affect

            try
            {
                int kidsSurveyId = (int) Session["KidsSurveyId"];
                int profileId = (int) Session["ProfileId"];
                string surveyDate = Session["SurveyDate"].ToString();
                int seq = (int) Session["SequenceEmo"]; 

                int EmoId; IList<int> emoIds = new List<int>();

                //status equals true then save the data
                //this if works when there's a new entry from location page and travel details page
                if (taskWhile == "location")
                {
                    seq++;
                    foreach (var r in ta)
                    {
                        KidsEmoStageTrackedDto e = new KidsEmoStageTrackedDto();
                        e.ProfileId = profileId;
                        e.KidsSurveyId = kidsSurveyId;
                        e.SurveyDate = surveyDate;
                        e.TaskWhile = taskWhile;
                                                
                        e.KidsLocationId = locOrTravelId;
                        e.LocationName = locationName;
                        e.OtherLocationName = otherLocationName;

                        e.KidsTravelId = null;
                        e.ModeOfTransport = modeOfTransport;                        
                        e.OtherModeOfTransport = otherModeOfTransport;

                        e.KidsTaskId = r.Key;
                        e.TaskPerformed = r.Value.ToString();
                        if (r.Value.ToString() == "Other")
                        { e.OtherTask = otherTask; }
                        else e.OtherTask = null;

                        e.TaskStartTime = taskStartTime;
                        e.TaskEndTime = taskEndTime;

                        e.IsEmoAffStageCompleted = false;
                        e.SequenceToQEmo = seq++;

                        e.TravelDetails = traveldetails;

                        Session["SequenceEmo"] = e.SequenceToQEmo;

                        kidsService.SaveEmoTrack(e);                        
                    }
                }
                else if(taskWhile == "travel")
                {
                    KidsEmoStageTrackedDto e = new KidsEmoStageTrackedDto();
                    e.ProfileId = profileId;
                    e.KidsSurveyId = kidsSurveyId;
                    e.SurveyDate = surveyDate;
                    e.TaskWhile = taskWhile;

                    e.KidsLocationId = null;
                    e.LocationName = locationName;
                    e.OtherLocationName = otherLocationName;

                    e.KidsTravelId = locOrTravelId;
                    e.ModeOfTransport = modeOfTransport;
                    e.OtherModeOfTransport = otherModeOfTransport;

                    e.KidsTaskId = null;
                    e.TaskPerformed = null;
                    e.OtherTask = otherTask;

                    e.TaskStartTime = taskStartTime;
                    e.TaskEndTime = taskEndTime;

                    e.IsEmoAffStageCompleted = false;
                    e.SequenceToQEmo = seq;

                    e.TravelDetails = traveldetails;

                    kidsService.SaveEmoTrack(e);
                }
                  
            }
            catch (Exception ex)
            {
                string EMsg = "Kids UpdateEMO Method:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
            }
        }
        
        public Decimal ChangeNumber(Decimal number)
        {
            Decimal newNumber = 0;

            try
            {

                if (number == 12) { newNumber = 12; }
                else if (number == 13) { newNumber = 1; }
                else if (number == 14) { newNumber = 2; }
                else if (number == 15) { newNumber = 3; }
                else if (number == 16) { newNumber = 4; }
                else if (number == 17) { newNumber = 5; }
                else if (number == 18) { newNumber = 6; }
                else { newNumber = number; }
            }
            catch (Exception ex)
            {
                string EMsg = "ChangeNumber METHOD:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
            }
            return newNumber;
        }

        public string CalculateTimeStamp(string time)
        {
            string calc = string.Empty;
            string trimTimestr = string.Empty;
            string repCol = string.Empty;
            double tn = 0; int n = 0;
            string finalResult = string.Empty;
            Decimal tnn, tempn;
            string[] tarr;
            try
            {
                char[] strtrim = { 'p', 'm' };
                trimTimestr = time.Trim(strtrim).Trim().ToString();
                calc = trimTimestr.ToString();

                repCol = calc.Replace(":", ".");
                tn = Convert.ToDouble(repCol);
                tnn = Convert.ToDecimal(tn);

                tarr = calc.Split(':');

                if (tn > (11.59))
                {
                    tempn = ChangeNumber(Decimal.Truncate(tnn));
                    n = Convert.ToInt32(tempn);
                    finalResult = n.ToString() + ":" + tarr[1].ToString() + " am";
                }
                else
                { finalResult = time.ToString(); }
            }
            catch (Exception ex)
            {
                string EMsg = "CalculateTimeStamp METHOD:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
            }
            return finalResult;
        }

        #endregion

        #region PREVIOUS KIDS VERSION (NOT IN USE)

        #region TIMELINE

        [HttpGet]
        public async Task<ActionResult> TimeLine()
        {
            try
            {
                int profileId = (int) Session["ProfileId"];
                int KidsSurveyId = (int) Session["KidsSurveyId"];

                KidsSurveyTimelineVM v = new KidsSurveyTimelineVM();

                if (!string.IsNullOrEmpty(KidsSurveyId.ToString()))
                {
                    var y = db.KidsTaskS.Where(u => u.KidsSurveyId == KidsSurveyId
                                                    && u.ProfileId == profileId)
                                        .Select(u => u).ToList();

                    v.AllKidsTasksObj = y.ToList();
                }
                v.PeopleOptions = kidsService.GetPeopleOptions();

                if (Request.IsAjaxRequest())
                { return PartialView(v); }

                return View(v);
            }
            catch (Exception ex)
            {
                string EMsg = "TimeLine:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }
        public async Task<ActionResult> AddNewTask(KidsTasks tasks)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int surveyId = (int) Session["KidsSurveyId"];
                    int profileId = (int) Session["ProfileId"];

                    tasks.ProfileId = profileId;
                    tasks.KidsSurveyId = surveyId;
                    tasks.SurveyDate = (DateTime) Session["KidsSurveyDate"];
                    db.KidsTaskS.Add(tasks);
                    await db.SaveChangesAsync();
                }
                if (Request.IsAjaxRequest())
                { return PartialView(); }
                return View("Timeline", tasks);
            }
            catch (Exception ex)
            {
                string EMsg = "AddNewTask:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> TimeLine(KidsSurveyTimelineVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int kidsSurveyId = (int) Session["KidsSurveyId"];
                    int profileId = (int) Session["ProfileId"];

                    IList<KidsTasks> result;
                    result = kidsService.GetKidsTaskByProfileandSurveyId(profileId, kidsSurveyId);

                    var totalTaskSelectionlimit = db.MasterDataS.Select(u => u).FirstOrDefault();

                    IList<KidsTasks> GetRandomSelect = RandomTaskGenerationService
                                                    .GetRandom(result, totalTaskSelectionlimit.RecurrentSurveyTaskSelectionLimit)
                                                    .ToList();
                    foreach (var i in GetRandomSelect)
                    {
                        var randomRow = db.KidsTaskS.Where(u => u.ProfileId == profileId && u.Id == i.Id).First();
                        randomRow.IsRandomlySelected = true;
                        db.Entry(randomRow).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                    return RedirectToAction("Responses1");
                }
                catch (Exception ex)
                {
                    string EMsg = "Responses1 GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            return View();
        }
        #endregion

        #region TASK TIME

        [HttpGet]
        public async Task<ActionResult> TaskTime()
        {
            KidsTaskTimeVM k = new KidsTaskTimeVM();
            return View(k);
        }

        [HttpPost]
        public async Task<ActionResult> TaskTime(string a)
        { return View(); }

        #endregion

        #region TASK LOCATION

        [HttpGet]
        public async Task<ActionResult> TaskLocation()
        {
            KidsTaskLocationVM k = new KidsTaskLocationVM();
            k.YesNoOptionList = kidsService.GetYesNoOptions();
            k.InOutOptionList = kidsService.GetInOutOptions();
            k.TravelOptionList = kidsService.GetTransportOptions();

            return View(k);
        }

        [HttpPost]
        public async Task<ActionResult> TaskLocation(string a)
        { return View(); }

        #endregion

        #region "KIDS RESPONSES"
        [HttpGet]
        public async Task<ActionResult> Responses1()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["KidsSurveyId"] != null)
                    {
                        KidsResponsesOneVM v = new KidsResponsesOneVM();

                        int kidsSurveyId = (int) Session["KidsSurveyId"];
                        int profileId = (int) Session["ProfileId"];
                        string shiftSpan = (string) Session["ShiftSpan"];
                        string surveySpan = (string) Session["SurveySpan"];
                        string uid = Session["UID"].ToString();

                        List<KidsTasks> result;
                        result = kidsService.GetKidsRandomlySelectedTasksByProfileAndSurveyId
                                        (profileId, kidsSurveyId, true, false);
                        if (result.Count() != 0)
                        {
                            var kidsEmoSelect = result.First();
                            Session["KidsTaskStart"] = kidsEmoSelect.StartTime;
                            Session["KidsTaskEnd"] = kidsEmoSelect.EndTime;

                            v.TaskName = kidsEmoSelect.TaskName;
                            Session["KidsTaskSelected"] = kidsEmoSelect.TaskName;
                            Session["KidsTaskId"] = kidsEmoSelect.Id;
                        }

                        v.Uid = Session["UID"].ToString();
                        v.ShiftSpan = shiftSpan;
                        v.SurveySpan = surveySpan;
                        v.ResponseStartDateTimeUtc = DateTime.UtcNow;

                        v.Q1Ans = Constants.NA_7Rating;
                        v.Q2Ans = Constants.NA_7Rating;
                        v.Q3Ans = Constants.NA_7Rating;
                        v.Q4Ans = Constants.NA_7Rating;
                        v.Q5Ans = Constants.NA_7Rating;

                        if (Request.IsAjaxRequest())
                        { return PartialView(v); }
                        return View(v);
                    }
                    await LogMyDayError(Session["UID"].ToString(), "Responses1 GET: Survey UID not found!", "SurveyError");
                    return RedirectToAction("SurveyError");
                }
                catch (Exception ex)
                {
                    string EMsg = "Responses1 GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Responses1(KidsResponsesOneVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int kidsSurveyId = (int) Session["KidsSurveyId"];
                    int profileId = (int) Session["ProfileId"];

                    #region KidsResponses

                    KidsResponsesDto r1 = new KidsResponsesDto();
                    r1.ProfileId = profileId;
                    r1.KidsSurveyId = kidsSurveyId;
                    r1.QuestionId = v.Q1DB;
                    r1.Answer = Constants.GetText7ScaleRating(v.Q1Ans);
                    r1.ResponseStartTimeUTC = DateTime.UtcNow;
                    r1.ResponseEndTimeUTC = DateTime.UtcNow;
                    r1.KidsTaskId = Convert.ToInt32(Session["KidsTaskId"].ToString());
                    r1.TaskName = Session["KidsTaskSelected"].ToString();
                    r1.SurveyDate = (DateTime) Session["KidsSurveyDate"];
                    kidsService.AddKidsResponses(r1);

                    KidsResponsesDto r2 = new KidsResponsesDto();
                    r2.ProfileId = profileId;
                    r2.KidsSurveyId = kidsSurveyId;
                    r2.QuestionId = v.Q2DB;
                    r2.Answer = Constants.GetText7ScaleRating(v.Q2Ans);
                    r2.ResponseStartTimeUTC = DateTime.UtcNow;
                    r2.ResponseEndTimeUTC = DateTime.UtcNow;
                    r2.KidsTaskId = Convert.ToInt32(Session["KidsTaskId"].ToString());
                    r2.TaskName = Session["KidsTaskSelected"].ToString();
                    r2.SurveyDate = (DateTime) Session["KidsSurveyDate"];
                    kidsService.AddKidsResponses(r2);

                    KidsResponsesDto r3 = new KidsResponsesDto();
                    r3.ProfileId = profileId;
                    r3.KidsSurveyId = kidsSurveyId;
                    r3.QuestionId = v.Q3DB;
                    r3.Answer = Constants.GetText7ScaleRating(v.Q3Ans);
                    r3.ResponseStartTimeUTC = DateTime.UtcNow;
                    r3.ResponseEndTimeUTC = DateTime.UtcNow;
                    r3.KidsTaskId = Convert.ToInt32(Session["KidsTaskId"].ToString());
                    r3.TaskName = Session["KidsTaskSelected"].ToString();
                    r3.SurveyDate = (DateTime) Session["KidsSurveyDate"];
                    kidsService.AddKidsResponses(r3);

                    KidsResponsesDto r4 = new KidsResponsesDto();
                    r4.ProfileId = profileId;
                    r4.KidsSurveyId = kidsSurveyId;
                    r4.QuestionId = v.Q4DB;
                    r4.Answer = Constants.GetText7ScaleRating(v.Q4Ans);
                    r4.ResponseStartTimeUTC = DateTime.UtcNow;
                    r4.ResponseEndTimeUTC = DateTime.UtcNow;
                    r4.KidsTaskId = Convert.ToInt32(Session["KidsTaskId"].ToString());
                    r4.TaskName = Session["KidsTaskSelected"].ToString();
                    r4.SurveyDate = (DateTime) Session["KidsSurveyDate"];
                    kidsService.AddKidsResponses(r4);

                    KidsResponsesDto r5 = new KidsResponsesDto();
                    r5.ProfileId = profileId;
                    r5.KidsSurveyId = kidsSurveyId;
                    r5.QuestionId = v.Q5DB;
                    r5.Answer = Constants.GetText7ScaleRating(v.Q5Ans);
                    r5.ResponseStartTimeUTC = DateTime.UtcNow;
                    r5.ResponseEndTimeUTC = DateTime.UtcNow;
                    r5.KidsTaskId = Convert.ToInt32(Session["KidsTaskId"].ToString());
                    r5.TaskName = Session["KidsTaskSelected"].ToString();
                    r5.SurveyDate = (DateTime) Session["KidsSurveyDate"];
                    kidsService.AddKidsResponses(r5);

                    #endregion

                    kidsService.SaveKidsResponses();

                    return RedirectToAction("Responses2");
                }
                catch (Exception ex)
                {
                    string EMsg = "Responses1 POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            { return PartialView(v); }
            return View(v);
        }
        [HttpGet]
        public async Task<ActionResult> Responses2()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["KidsSurveyId"] != null)
                    {
                        KidsResponsesTwoVM v = new KidsResponsesTwoVM();

                        int kidsSurveyId = (int) Session["KidsSurveyId"];
                        int profileId = (int) Session["ProfileId"];
                        string shiftSpan = (string) Session["ShiftSpan"];
                        string surveySpan = (string) Session["SurveySpan"];
                        string uid = Session["UID"].ToString();

                        v.Uid = Session["UID"].ToString();
                        v.ShiftSpan = shiftSpan;
                        v.SurveySpan = surveySpan;
                        v.ResponseStartDateTimeUtc = DateTime.UtcNow;
                        v.TaskName = Session["KidsTaskSelected"].ToString();

                        v.Q6Ans = Constants.NA_7Rating;
                        v.Q7Ans = Constants.NA_7Rating;
                        v.Q8Ans = Constants.NA_7Rating;
                        v.Q9Ans = Constants.NA_7Rating;
                        v.Q10Ans = Constants.NA_7Rating;

                        if (Request.IsAjaxRequest())
                        { return PartialView(v); }
                        return View(v);
                    }
                    await LogMyDayError(Session["UID"].ToString(), "Responses2 GET: Survey UID not found!", "SurveyError");
                    return RedirectToAction("SurveyError");
                }
                catch (Exception ex)
                {
                    string EMsg = "Responses2 GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Responses2(KidsResponsesTwoVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int kidsSurveyId = (int) Session["KidsSurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    int kidsTaskId = (int) Session["KidsTaskId"];
                    #region KidsResponses

                    KidsResponsesDto r6 = new KidsResponsesDto();
                    r6.ProfileId = profileId;
                    r6.KidsSurveyId = kidsSurveyId;
                    r6.QuestionId = v.Q6DB;
                    r6.Answer = Constants.GetText7ScaleRating(v.Q6Ans);
                    r6.ResponseStartTimeUTC = DateTime.UtcNow;
                    r6.ResponseEndTimeUTC = DateTime.UtcNow;
                    r6.KidsTaskId = Convert.ToInt32(Session["KidsTaskId"].ToString());
                    r6.TaskName = Session["KidsTaskSelected"].ToString();
                    r6.SurveyDate = (DateTime) Session["KidsSurveyDate"];
                    kidsService.AddKidsResponses(r6);

                    KidsResponsesDto r7 = new KidsResponsesDto();
                    r7.ProfileId = profileId;
                    r7.KidsSurveyId = kidsSurveyId;
                    r7.QuestionId = v.Q7DB;
                    r7.Answer = Constants.GetText7ScaleRating(v.Q7Ans);
                    r7.ResponseStartTimeUTC = DateTime.UtcNow;
                    r7.ResponseEndTimeUTC = DateTime.UtcNow;
                    r7.KidsTaskId = Convert.ToInt32(Session["KidsTaskId"].ToString());
                    r7.TaskName = Session["KidsTaskSelected"].ToString();
                    r7.SurveyDate = (DateTime) Session["KidsSurveyDate"];
                    kidsService.AddKidsResponses(r7);

                    KidsResponsesDto r8 = new KidsResponsesDto();
                    r8.ProfileId = profileId;
                    r8.KidsSurveyId = kidsSurveyId;
                    r8.QuestionId = v.Q8DB;
                    r8.Answer = Constants.GetText7ScaleRating(v.Q8Ans);
                    r8.ResponseStartTimeUTC = DateTime.UtcNow;
                    r8.ResponseEndTimeUTC = DateTime.UtcNow;
                    r8.KidsTaskId = Convert.ToInt32(Session["KidsTaskId"].ToString());
                    r8.TaskName = Session["KidsTaskSelected"].ToString();
                    r8.SurveyDate = (DateTime) Session["KidsSurveyDate"];
                    kidsService.AddKidsResponses(r8);

                    KidsResponsesDto r9 = new KidsResponsesDto();
                    r9.ProfileId = profileId;
                    r9.KidsSurveyId = kidsSurveyId;
                    r9.QuestionId = v.Q9DB;
                    r9.Answer = Constants.GetText7ScaleRating(v.Q9Ans);
                    r9.ResponseStartTimeUTC = DateTime.UtcNow;
                    r9.ResponseEndTimeUTC = DateTime.UtcNow;
                    r9.KidsTaskId = Convert.ToInt32(Session["KidsTaskId"].ToString());
                    r9.TaskName = Session["KidsTaskSelected"].ToString();
                    r9.SurveyDate = (DateTime) Session["KidsSurveyDate"];
                    kidsService.AddKidsResponses(r9);

                    KidsResponsesDto r10 = new KidsResponsesDto();
                    r10.ProfileId = profileId;
                    r10.KidsSurveyId = kidsSurveyId;
                    r10.QuestionId = v.Q10DB;
                    r10.Answer = Constants.GetText7ScaleRating(v.Q10Ans);
                    r10.ResponseStartTimeUTC = DateTime.UtcNow;
                    r10.ResponseEndTimeUTC = DateTime.UtcNow;
                    r10.KidsTaskId = Convert.ToInt32(Session["KidsTaskId"].ToString());
                    r10.TaskName = Session["KidsTaskSelected"].ToString();
                    r10.SurveyDate = (DateTime) Session["KidsSurveyDate"];
                    kidsService.AddKidsResponses(r10);

                    #endregion

                    kidsService.SaveKidsResponses();

                    //To update the Emotional Affect stage completed to true for randomly selected task
                    var kidsTasks = db.KidsTaskS
                                      .Where(w => w.KidsSurveyId == kidsSurveyId
                                               && w.ProfileId == profileId
                                               && w.Id == kidsTaskId)
                                      .First();
                    kidsTasks.IsEmotionalStageCompleted = true;
                    db.Entry(kidsTasks).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    //To get the remaining emotional affect stages pending to be filled by the user
                    var totalEmoStagesRemaining = db.KidsTaskS
                                                    .Where(u => u.ProfileId == profileId
                                                             && u.KidsSurveyId == kidsSurveyId
                                                             && u.IsRandomlySelected == true
                                                             && u.IsEmotionalStageCompleted == false)
                                                    .Select(u => u).ToList();

                    //To get the task selection limit set in master page
                    var mstData = db.MasterDataS.Select(b => b).FirstOrDefault();

                    //To check if the emotional affect stage is not completed for particular task
                    //then redirect user to the emotional affect page or else redirect to feedback page upon completion
                    if (totalEmoStagesRemaining.Count() < mstData.RecurrentSurveyTaskSelectionLimit
                                && totalEmoStagesRemaining.Count != 0)
                    {
                        return RedirectToAction("Responses1");
                    }
                    else { return RedirectToAction("Feedback"); }
                }
                catch (Exception ex)
                {
                    string EMsg = "Responses2 POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            { return PartialView(v); }
            return View(v);
        }
        #endregion

        #region KIDS TASk SUMMARY

        [HttpGet]
        public async Task<ActionResult> TaskSummary()
        {
            try
            {
                if (Session["KidsSurveyId"] != null)
                {
                    int kidsSurveyId = (int) Session["KidsSurveyId"];
                    int profileId = (int) Session["ProfileId"];

                    KidsSummaryVM v = new KidsSummaryVM();

                    var kidsSurvey = kidsService.GetKidsSurveyById(kidsSurveyId);

                    var KidsComments = kidsService.GetKidsCommentByProfileandSurveyId(profileId, kidsSurveyId);
                    v.Comment = KidsComments.Comments;

                    KidsSurveyTaskDurationVM tdresponse = new KidsSurveyTaskDurationVM();
                    tdresponse.kidsTasksObj = new List<KidsTasks>();
                    tdresponse.kidsTasksObj = kidsService.GetKidsTaskByProfileandSurveyId(profileId, kidsSurveyId).ToList();

                    var genericResponses = kidsService.GetGenericKidsResponses(kidsSurveyId, profileId);
                    var tasklist = kidsService.GetKidsTaskByProfileandSurveyId(profileId, kidsSurveyId);
                    Session["TaskList"] = tdresponse.kidsTasksObj;//to be used on Edit Response

                    //v.ShiftSpan = shiftSpan;
                    //v.SurveySpan = surveySpan;

                    KidsSurveyTaskDurationVM ref1 = new KidsSurveyTaskDurationVM();
                    KidsResponsesOneVM ref2 = new KidsResponsesOneVM();
                    KidsResponsesTwoVM ref3 = new KidsResponsesTwoVM();
                    Dictionary<string, string> referenceTable = new Dictionary<string, string>();

                    //referenceTable.Add(ref1.QDB, ref1.QDisplayShort);
                    referenceTable.Add(ref2.Q1DB, ref2.Q1DisplayShort);
                    referenceTable.Add(ref2.Q2DB, ref2.Q2DisplayShort);
                    referenceTable.Add(ref2.Q3DB, ref2.Q3DisplayShort);
                    referenceTable.Add(ref2.Q4DB, ref2.Q4DisplayShort);
                    referenceTable.Add(ref2.Q5DB, ref2.Q5DisplayShort);
                    referenceTable.Add(ref3.Q6DB, ref3.Q6DisplayShort);
                    referenceTable.Add(ref3.Q7DB, ref3.Q7DisplayShort);
                    referenceTable.Add(ref3.Q8DB, ref3.Q8DisplayShort);
                    referenceTable.Add(ref3.Q9DB, ref3.Q9DisplayShort);
                    referenceTable.Add(ref3.Q10DB, ref3.Q10DisplayShort);

                    List<Tuple<string, KidsResponsesVM>> genericList =
                                                            new List<Tuple<string, KidsResponsesVM>>();
                    //                    DateTime? taskStartDate = null;
                    int totalMins = 0;

                    //Generic
                    foreach (var k in genericResponses)
                    {
                        KidsResponsesVM r = new KidsResponsesVM();

                        var taskDet = tasklist
                                            .Where(m => m.Id == k.KidsTaskId)
                                            .Select(m => m)
                                            .Single();

                        r.TaskId = k.KidsTaskId;
                        //r.TaskStartDateTime = taskDet.StartTime;
                        r.TaskStartDate = taskDet.SurveyDate.ToLongDateString();
                        r.TaskStartTime = taskDet.StartTime;

                        r.Question = referenceTable[k.QuestionId].Trim(); //get display qns
                        r.Answer = k.Answer;
                        r.RatingString = surveyService.GetRatingString(k.Answer);
                        r.TaskName = taskDet.TaskName;

                        r.TaskDescription = taskDet.TaskName;
                        foreach (var z in tdresponse.kidsTasksObj)
                        {
                            if (z.Id == k.KidsTaskId)
                            {
                                //DateTime dt;
                                //bool res = DateTime.TryParse("01:00 PM", out dt);                                
                                DateTime a, b;
                                bool r1 = DateTime.TryParse(z.StartTime, out a);
                                bool r2 = DateTime.TryParse(z.EndTime, out b);

                                r.TaskDuration = z.StartTime + " - " + z.EndTime;
                            }
                        }
                        r.TaskTimeSpan = taskDet.StartTime + " - " + taskDet.EndTime;
                        genericList.Add(Tuple.Create(r.TaskName + " STARTDATESAN " + taskDet.StartTime, r));
                    }
                    v.FullResponseList = (Lookup<string, KidsResponsesVM>) genericList.ToLookup(t => t.Item1, t => t.Item2);


                    if (Request.IsAjaxRequest())
                    { return PartialView(v); }
                    return View(v);
                }
                return null;
            }
            catch (Exception ex)
            {
                string EMsg = "Kids TASK SUMMARY GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }

        [HttpGet]
        public async Task<ActionResult> EditResponse(int taskId, DateTime taskStartDate)
        {
            string suid = string.Empty;
            try
            {
                KidsEditResponseVM v = new KidsEditResponseVM();

                int surveyId = (int) Session["KidsSurveyId"];
                int profileId = (int) Session["ProfileId"];
                string shiftSpan = (string) Session["ShiftSpan"];
                string surveySpan = (string) Session["SurveySpan"];

                ////TODO: save to a session task list for quick retrieval
                ////or get it from Results()
                KidsTasksDto task = null;

                if (Session["TaskList"] != null)
                {
                    var taskList = (List<KidsTasks>) Session["TaskList"];
                    //var taskList = (IList<TaskVM>) Session["TaskList"];

                    task = taskList.Where(m => m.Id == taskId)
                                            .Select(m => new KidsTasksDto()
                                            {
                                                Id = m.Id,
                                                TaskName = m.TaskName,
                                                StartTime = m.StartTime,
                                                EndTime = m.EndTime
                                            })
                                            .Single();
                    Session["editCurrTaskStartTime"] = task.StartTime;
                    Session["editCurrTaskEndTime"] = task.EndTime;
                }
                else
                {
                    task = kidsService.GetKidsTaskByTaskId(taskId);
                }

                v.TaskName = task.TaskName;

                Session["CurrTask"] = task.Id;
                Session["CurrTaskStartTime"] = taskStartDate;
                bool isAny = false;
                var listOfResponses = kidsService.GetAllKidsResponses(taskId, profileId, surveyId, taskStartDate);
                var response = listOfResponses.Where(t => t.QuestionId == v.Q1DB).SingleOrDefault();

                v.ShiftSpan = shiftSpan;
                v.SurveySpan = surveySpan;

                if (response != null)
                {
                    v.Q1Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else { v.Q1Ans = Constants.NA_7Rating; }
                response = listOfResponses.Where(t => t.QuestionId == v.Q2DB).SingleOrDefault();

                if (response != null)
                { v.Q2Ans = Constants.GetInt7ScaleRating(response.Answer); isAny = true; }
                else { v.Q2Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.QuestionId == v.Q3DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q3Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else { v.Q3Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.QuestionId == v.Q4DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q4Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else { v.Q4Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.QuestionId == v.Q5DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q5Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else { v.Q5Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.QuestionId == v.Q6DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q6Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else { v.Q6Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.QuestionId == v.Q7DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q7Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else { v.Q7Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.QuestionId == v.Q8DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q8Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else { v.Q8Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.QuestionId == v.Q9DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q9Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else { v.Q9Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.QuestionId == v.Q10DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q10Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else { v.Q10Ans = Constants.NA_7Rating; }

                if (isAny)
                { v.IsExist = true; }

                if (Request.IsAjaxRequest())
                { return PartialView(v); }
                return View(v);
            }
            catch (Exception ex)
            {
                string EMsg = "Kids EDIT-RESPONSE GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> EditResponse(KidsEditResponseVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int) Session["KidsSurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    int taskId = (int) Session["CurrTask"];
                    DateTime startTime = (DateTime) Session["CurrTaskStartTime"];

                    // Save to db
                    #region update Question  
                    var listOfResponses = kidsService.GetAllKidsResponses(taskId, profileId, surveyId, startTime);
                    var response = listOfResponses.Where(r => r.QuestionId == v.Q1DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q1Ans);
                        kidsService.UpdateKidsResponse(response);
                    }

                    response = listOfResponses.Where(r => r.QuestionId == v.Q2DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q2Ans);
                        kidsService.UpdateKidsResponse(response);
                    }
                    response = listOfResponses.Where(r => r.QuestionId == v.Q3DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q3Ans);
                        kidsService.UpdateKidsResponse(response);
                    }
                    response = listOfResponses.Where(r => r.QuestionId == v.Q4DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q4Ans);
                        kidsService.UpdateKidsResponse(response);
                    }
                    response = listOfResponses.Where(r => r.QuestionId == v.Q5DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q5Ans);
                        kidsService.UpdateKidsResponse(response);
                    }
                    response = listOfResponses.Where(r => r.QuestionId == v.Q6DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q6Ans);
                        kidsService.UpdateKidsResponse(response);
                    }
                    response = listOfResponses.Where(r => r.QuestionId == v.Q7DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q7Ans);
                        kidsService.UpdateKidsResponse(response);
                    }
                    response = listOfResponses.Where(r => r.QuestionId == v.Q8DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q8Ans);
                        kidsService.UpdateKidsResponse(response);
                    }
                    response = listOfResponses.Where(r => r.QuestionId == v.Q9DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q9Ans);
                        kidsService.UpdateKidsResponse(response);
                    }
                    response = listOfResponses.Where(r => r.QuestionId == v.Q10DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q10Ans);
                        kidsService.UpdateKidsResponse(response);
                    }
                    kidsService.SaveKidsResponses();
                    #endregion                    
                    return RedirectToAction("TaskSummary");
                }
                catch (Exception ex)
                {
                    string EMsg = "Kids EDIT-RESPONSE POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            { return PartialView(v); }
            return View(v);
        }

        #endregion

        #endregion

    }
}