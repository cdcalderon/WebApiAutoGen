using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.Session
{
    [Serializable]
    public class YPrimeSession
    {
        public static YPrimeSession Instance
        {
            get
            {
                System.Web.SessionState.HttpSessionState session = HttpContext.Current?.Session;

                YPrimeSession result = null;
                //guard code for patient portal
                if (session != null)
                {
                    if (session["YPrimeSessionInstance"] == null)
                    {
                        session["YPrimeSessionInstance"] = new YPrimeSession();
                    }
                    result = (YPrimeSession)session["YPrimeSessionInstance"];
                }

                return result;
            }

        }

        public YPrimeSession()
        {
            HasCheckedLandingPage = false;
        }

        public StudyUserDto CurrentUser { get; set; }
        public ClaimsIdentity CurrentUserAuth0 { get; set; }
        public bool SessionInitialized { get; set; }
        public string Protocol { get; set; }
        public Guid StudyId { get; set; }
        public string StudyType { get { return ConfigurationManager.AppSettings["StudyType"]; } }
        public string Sponsor { get; set; }
        public string GlobalDateFormat { get; set; }
        public string SupportChatURL { get; set; }
        public Guid ConfigurationId { get; set; }
        public Dictionary<string, string> StudySettingValues { get; set; }
        public bool HasCheckedLandingPage { get; set; } //this property remembers if the user has hit their initial landing page as configured
        public void Abandon()
        {
            HttpContext.Current.Session.Abandon();
        }
        public string SinglePatientAlias { get; set; }
        public string PluralPatientAlias { get; set; }
        public bool SupportChatEnabled { get; set; }
    }
}