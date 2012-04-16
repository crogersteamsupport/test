using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using System.Collections;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Text;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Runtime.Serialization;
using System.Globalization;

namespace TSWebServices
{
    [ScriptService]
    [WebService(Namespace = "http://teamsupport.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class UserService : System.Web.Services.WebService
    {

        public UserService() { }

        [WebMethod]
        public UserProxy UpdateUserStatusComment(string comment)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            User user = TSAuthentication.GetUser(loginUser);
            user.InOfficeComment = comment;
            user.Collection.Save();


            WaterCooler watercooler = new WaterCooler(loginUser);
            WaterCoolerItem item = watercooler.AddNewWaterCoolerItem();
            item.Message = string.Format("<strong>{0} - </strong>{1}", user.FirstLastName, user.InOfficeComment);
            item.OrganizationID = user.OrganizationID;
            item.TimeStamp = DateTime.UtcNow;
            item.UserID = user.UserID;
            watercooler.Save();

            return user.GetProxy();
        }

        [WebMethod]
        public void SetIsClassicView(bool value)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            User user = TSAuthentication.GetUser(loginUser);
            user.IsClassicView = value;
            user.Collection.Save();
        }

        //tells us whether the logged in user can edit the specified user's account
        [WebMethod]
        public bool AllowUserEdit(int userID)
        {
            return userID == TSAuthentication.GetLoginUser().UserID;
        }

        [WebMethod]
        public UserProxy GetUser(int userID)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return null;
            return user.GetProxy();
        }

        [WebMethod]
        public string GetUserPhoto(int userID)
        {
            string path;
            //return Attachments.GetAttachmentPath(TSAuthentication.GetLoginUser(), ReferenceType.UserPhoto, userID);

            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            Attachments att = new Attachments(TSAuthentication.GetLoginUser());
            att.LoadByReference(ReferenceType.UserPhoto, userID);

            if (att.Count > 0)
            {
                path = String.Format("../dc/{0}/avatar/{1}", user.OrganizationID, att[0].AttachmentID); ;//att[0].Path;
            }
            else
                path = "../images/blank_avatar.png";
            return path;
        }

        [WebMethod]
        public AddressProxy[] GetUserAddresses(int userID)
        {
            List<AddressProxy> addresses = new List<AddressProxy>();

            Addresses addrs = new Addresses(TSAuthentication.GetLoginUser());
            addrs.LoadByID(userID, ReferenceType.Users);

            foreach (Address address in addrs)
            {
                addresses.Add(address.GetProxy());
            }

            return addresses.ToArray();
        }

        [WebMethod]
        public PhoneNumberProxy[] GetUserPhoneNumbers(int userID)
        {
            List<PhoneNumberProxy> phones = new List<PhoneNumberProxy>();

            PhoneNumbers numbers = new PhoneNumbers(TSAuthentication.GetLoginUser());
            numbers.LoadByID(userID, ReferenceType.Users);

            foreach (PhoneNumber phone in numbers)
            {
                phones.Add(phone.GetProxy());
            }

            return phones.ToArray();
        }

        [WebMethod]
        public GroupProxy[] GetUserGroups(int userID)
        {
            Groups groups = new Groups(TSAuthentication.GetLoginUser());
            groups.LoadByUserID(userID);
            return groups.GetGroupProxies();
        }

        [WebMethod]
        public bool UpdateUserStatus(bool value)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            User user = TSAuthentication.GetUser(loginUser);
            user.InOffice = value;
            user.Collection.Save();
            return value;
            /*
            WaterCooler watercooler = new WaterCooler(loginUser);
            WaterCoolerItem item = watercooler.AddNewWaterCoolerItem();
            item.Message = string.Format("<strong>{0}</strong> {1}", user.FirstLastName, user.InOffice ? "is now in the office." : "has left the office.");
            item.OrganizationID = user.OrganizationID;
            item.TimeStamp = DateTime.UtcNow;
            item.UserID = user.UserID;
            watercooler.Save();
            */
        }

        [WebMethod]
        public UserProxy ToggleUserStatus()
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            User user = TSAuthentication.GetUser(loginUser);
            user.InOffice = !user.InOffice;
            user.Collection.Save();

            WaterCooler watercooler = new WaterCooler(loginUser);
            WaterCoolerItem item = watercooler.AddNewWaterCoolerItem();
            item.Message = string.Format("<strong>{0}</strong> {1}", user.FirstLastName, user.InOffice ? "is now in the office." : "has left the office.");
            item.OrganizationID = user.OrganizationID;
            item.TimeStamp = DateTime.UtcNow;
            item.UserID = user.UserID;
            //watercooler.Save();
            return user.GetProxy();
        }

        [WebMethod]
        public ChatUserSettingProxy ToggleUserChatStatus()
        {
            ChatUserSetting setting = ChatUserSettings.GetChatUserSetting(TSAuthentication.GetLoginUser(), TSAuthentication.UserID);
            setting.IsAvailable = !setting.IsAvailable;
            setting.Collection.Save();
            return setting.GetProxy();
        }

        [WebMethod]
        public BasicUser[] GetUsers()
        {
            Users users = new Users(TSAuthentication.GetLoginUser());
            users.LoadByOrganizationID(TSAuthentication.OrganizationID, true);
            List<BasicUser> result = new List<BasicUser>();

            foreach (User user in users)
            {
                BasicUser basic = new BasicUser();
                basic.Name = user.FirstName + " " + user.LastName;
                basic.UserID = user.UserID;
                result.Add(basic);
            }
            return result.ToArray();
        }

        [WebMethod]
        public void CreateDefaultUsers()
        {
            Users.CreateDefaultUsers(TSAuthentication.GetLoginUser());
        }

        [WebMethod]
        public GroupProxy[] GetGroups()
        {
            Groups groups = new Groups(TSAuthentication.GetLoginUser());
            groups.LoadByOrganizationID(TSAuthentication.OrganizationID);
            return groups.GetGroupProxies();
        }

        [WebMethod]
        public AutocompleteItem[] SearchUsers(string searchTerm)
        {
            UsersView users = new UsersView(TSAuthentication.GetLoginUser());
            users.LoadByTerm(TSAuthentication.OrganizationID, searchTerm, 15);

            List<AutocompleteItem> list = new List<AutocompleteItem>();
            foreach (UsersViewItem user in users)
            {
                list.Add(new AutocompleteItem(user.FirstName + " " + user.LastName, user.UserID.ToString()));
            }

            return list.ToArray();
        }

        [WebMethod]
        public bool ShowIntroVideo()
        {
            if (Settings.UserDB.ReadBool("ShowIntroVideo", true) && Organizations.GetUserCount(TSAuthentication.GetLoginUser(), TSAuthentication.OrganizationID) == 1)
            {
                Settings.UserDB.WriteBool("ShowIntroVideo", false);
                return true;
            }

            return false;

        }



        [WebMethod]
        public string SaveUserName(int userID, string firstName, string middleName, string lastName)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return null;

            user.FirstName = firstName;
            user.MiddleName = middleName;
            user.LastName = lastName;
            user.Collection.Save();
            return firstName + ' ' + lastName;
        }

        [WebMethod]
        public string SaveUserInfo(int userID, string info)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return null;
            user.UserInformation = info;
            user.Collection.Save();
            return info;
        }

        [WebMethod]
        public string SaveUserTitle(int userID, string title)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return null;
            user.Title = title;
            user.Collection.Save();
            return title;
        }

        [WebMethod]
        public string SaveUserEmail(int userID, string email)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return null;

            if (email.Length < 1 || email.IndexOf('@') < 0 || email.IndexOf('.') < 0)
                return "_error";


            user.Email = email;
            user.Collection.Save();
            return user.Email;
        }

        [WebMethod]
        public bool SetIsActive(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            user.IsActive = value;
            user.Collection.Save();
            return user.IsActive;
        }

        [WebMethod]
        public bool SetEmailNotify(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;

            user.ReceiveTicketNotifications = value;
            user.Collection.Save();
            return user.ReceiveTicketNotifications;
        }

        [WebMethod]
        public bool SetSubscribeTickets(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;

            user.SubscribeToNewTickets = value;
            user.Collection.Save();
            return user.SubscribeToNewTickets;
        }

        [WebMethod]
        public bool SetSubscribeActions(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;

            user.SubscribeToNewActions = value;
            user.Collection.Save();
            return user.SubscribeToNewActions;
        }

        [WebMethod]
        public bool SetAutoSubscribe(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;

            user.DoNotAutoSubscribe = value;
            user.Collection.Save();
            return user.DoNotAutoSubscribe;
        }

        [WebMethod]
        public bool SetGroupNotify(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;

            user.ReceiveAllGroupNotifications = value;
            user.Collection.Save();
            return user.ReceiveAllGroupNotifications;
        }

        [WebMethod]
        public bool SetSysAdmin(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            user.IsSystemAdmin = value;
            user.Collection.Save();
            return user.IsSystemAdmin;
        }

        [WebMethod]
        public string SetChatUser(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Organization organization = Organizations.GetOrganization(loginUser, loginUser.OrganizationID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value.ToString();
            if (!TSAuthentication.IsSystemAdmin) return value.ToString();

            if (value && Organizations.GetChatCount(loginUser, loginUser.OrganizationID) >= organization.ChatSeats)
                return "error";

            user.IsChatUser = value;
            user.Collection.Save();
            return user.IsChatUser.ToString();
        
        }

        [WebMethod]
        public string SetTimezone(int userID, string value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            TimeZoneInfo tzinfo;
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;

            user.TimeZoneID = value;
            user.Collection.Save();
            try
            {
                tzinfo = TimeZoneInfo.FindSystemTimeZoneById(value);
            }
            catch (Exception)
            {
                tzinfo = null;
            }

            return tzinfo.DisplayName;
        }

        [WebMethod]
        public string[] GetTimezone()
        {
            List<string> list = new List<string>();
            System.Collections.ObjectModel.ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
            foreach (TimeZoneInfo info in timeZones)
            {
                list.Add(info.DisplayName);
                list.Add(info.Id);
            }
            return list.ToArray();
        }

        [WebMethod]
        public string SetCulture(int userID, string value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;

            string cu = CultureInfo.GetCultureInfo(int.Parse(value)).Name;

            user.CultureName = cu;
            user.Collection.Save();
            return CultureInfo.GetCultureInfo(int.Parse(value)).DisplayName;
        }

        [WebMethod]
        public string[] GetCultures()
        {
            List<string> list = new List<string>();
            foreach (CultureInfo info in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                if (!info.IsNeutralCulture)
                {
                    list.Add(info.DisplayName + "_" + info.LCID);
                }
            }
            list.Sort();
            return list.ToArray();
        }

        [WebMethod]
        public string SetUserInformation(int userID, string value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;

            user.UserInformation = value;
            user.Collection.Save();
            return user.UserInformation;
        }

        [WebMethod]
        public string ResetEmailPW(int userID)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return ("There was an error resetting the password.");
            try
            {
                if (DataUtils.ResetPassword(TSAuthentication.GetLoginUser(), user, false))
                {
                return ("A new password has been sent to " + user.FirstName + " " + user.LastName);

                }
            }
            catch(Exception e)
            {
                return ("There was an error resetting the password.");
            }

            return ("There was an error resetting the password.");
        }



        [DataContract]
        public class BasicUser
        {
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public int UserID { get; set; }
        }


    }
}