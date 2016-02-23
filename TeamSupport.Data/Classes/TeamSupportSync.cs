using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Dynamic;
using Newtonsoft.Json;

namespace TeamSupport.Data
{
    public class TeamSupportSync
    {

        public static void SyncUser(int userID, int orgID, string company, string firstName, string lastName, string email)
        {
            dynamic payload = new ExpandoObject();
            payload.UserID = userID;
            payload.OrganizationID = orgID;
            payload.Company = company;
            payload.FirstName = firstName;
            payload.LastName = lastName;
            payload.Email = email;
            payload.PodName = SystemSettings.GetPodName();

            string json = JsonConvert.SerializeObject(payload);
            PostSyncData(BuildUrl("signup/syncUser"), json);

        }
                                               
        public static void SyncOrg(int orgID, string company, int userID, string firstName, string lastName, string email, string phoneNumber, string version)
        {
            dynamic payload = new ExpandoObject();
            payload.OrganizationID = orgID;
            payload.Company = company;
            payload.UserID = userID;
            payload.FirstName = firstName;
            payload.LastName = lastName;
            payload.Email = email;
            payload.PhoneNumber = phoneNumber;
            payload.Version = version;
            payload.PodName = SystemSettings.GetPodName();

            string json = JsonConvert.SerializeObject(payload);
            PostSyncData(BuildUrl("signup/syncOrg"), json);
        }
        private static void PostSyncData(string url, string json)
        {
            try
            {
                string result = "";
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    result = client.UploadString(url, "POST", json);
                }
                // possible to do something with result
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(LoginUser.Anonymous, ex, "TeamSupportSync", json);
            }


        }

        private static string BuildUrl(string path)
        {
            string baseUrl = SystemSettings.GetUserSyncUrl();
            return baseUrl.EndsWith("/") ? baseUrl + path : baseUrl + "/" + path;
        }
    }
}
