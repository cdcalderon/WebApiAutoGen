using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using YPrime.BusinessLayer.Interfaces;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.StudyPortal.Helpers
{
    public static class ZenDeskHelper
    {
        public static bool CreateZendDeskTicket(ICorrectionRepository correctionRepository, DCFRequestDto dcfRequestDto, StudyUserDto userIdentity)
        {
            bool success = false;
            var serializer = new JavaScriptSerializer();
            var studySettingValues = new Dictionary<string, string>();
            var studySettingKeys = new[]
            {
                "Protocol",
                "StudySponsor",
                "ZenDeskDCFURL",
                "ZenDeskDCFUserID",
                "ZenDeskDCFGroupID",
                "ZenDeskDCFUserToken",
                "ZenDeskDCFCustomFields"
            };

            foreach (var key in studySettingKeys)
            {
                var configValue = ConfigurationManager.AppSettings[key];
                studySettingValues.Add(key, configValue);
            }

            var authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(string.Format("{0}/token:{1}",
                studySettingValues["ZenDeskDCFUserID"], studySettingValues["ZenDeskDCFUserToken"])));
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(studySettingValues["ZenDeskDCFURL"]);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.Headers.Add("Authorization", "Basic " + authInfo);

            //Email
            var userSiteIds = string.Join(",", userIdentity.Sites.Select(s => s.SiteNumber));
            var subject =
                $"DCF Request for {studySettingValues["StudySponsor"]} {studySettingValues["Protocol"]} from Site {dcfRequestDto.SiteNumber}";
            var body = $@"User Name: {dcfRequestDto.UserFirstLast} 
                            User Email: {dcfRequestDto.Username}
                            User Site(s): {userSiteIds}
                            Subject Number: {dcfRequestDto.PatientNumber}
                            Subject Site: {dcfRequestDto.SiteNumber}
                            Date Of Data Change: {dcfRequestDto.LastUpdate.Value.ToShortDateString()}
                            Type Of Data Change: {dcfRequestDto.TypeOfDataChange}
                            Old Value: {dcfRequestDto.OldValue}
                            New Value: {dcfRequestDto.NewValue}
                            Notes: {dcfRequestDto.Notes}";

            //Create ticket
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                var json = serializer.Serialize(new
                {
                    ticket = new
                    {
                        subject,
                        comment = new {body},
                        description =
                            $@"Sponsor Name: {studySettingValues["StudySponsor"]} Protocol Name: {studySettingValues["Protocol"]} Subject Number: {dcfRequestDto.PatientNumber}",
                        created_at = dcfRequestDto.LastUpdate,
                        requester = new {name = dcfRequestDto.UserFirstLast, email = dcfRequestDto.Username},
                        group_id = studySettingValues["ZenDeskDCFGroupID"],
                        priority = "normal",
                        recipient = dcfRequestDto.Username,
                        custom_fields = serializer.DeserializeObject(studySettingValues["ZenDeskDCFCustomFields"])
                    }
                });

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            //Get TicketNumber
            var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                {
                    var ss = serializer.Deserialize(result, typeof(object)) as Dictionary<string, object>;
                    if (ss != null && ss.Any())
                    {
                        var sss = ss.Values.First() as Dictionary<string, object>;
                        if (sss != null && sss.ContainsKey("id"))
                        {
                            dcfRequestDto.TicketNumber = sss["id"].ToString();
                        }
                    }
                }
            }

            //Mail & save
            if (dcfRequestDto.TicketNumber != null)
            {
                if (Debugger.IsAttached == false)
                {
                    //MailUtil.SendMail(dcfRequestDto.Username, null, null, subject, body, false);
                }

                correctionRepository.CreateDCFRequest(dcfRequestDto);
                success = true;
            }

            return success;
        }
    }
}