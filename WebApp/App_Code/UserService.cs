﻿using System;
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
using System.Linq;
using System.Diagnostics;

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

        [WebMethod]
        public void SetMenuItems(int userID, string values)
        {
          if (!TSAuthentication.IsSystemAdmin) return;
          User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
          if (user.OrganizationID != TSAuthentication.OrganizationID) return;
          user.MenuItems = values;
          user.Collection.Save();
        }

      //tells us whether the logged in user can edit the specified user's account
        [WebMethod]
        public bool AllowUserEdit(int userID)
        {
            return userID == TSAuthentication.GetLoginUser().UserID;
        }

        [WebMethod]
        public void HideWelcomePage()
        {
          User user = TSAuthentication.GetUser(TSAuthentication.GetLoginUser());
          user.ShowWelcomePage = false;
          user.Collection.Save();

        }

        [WebMethod]
        public void SetTicketRights(int userID, int value)
        {
          if (!TSAuthentication.IsSystemAdmin) return;
          User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
          if (user.OrganizationID != TSAuthentication.OrganizationID) return;
          user.TicketRights = (TicketRightType)value;
          user.Collection.Save();
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

            if (userID == -99)
                userID = TSAuthentication.GetLoginUser().UserID;

            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            Attachments att = new Attachments(TSAuthentication.GetLoginUser());
            att.LoadByReference(ReferenceType.UserPhoto, userID);

            if (att.Count > 0)
            {
                path = String.Format("/dc/{0}/avatar/{1}", user.OrganizationID, att[0].AttachmentID);
            }
            else
                path = "../images/blank_avatar.png";
            return path;
        }

        [WebMethod]
        public string GetUserSignature(int userID)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);

            return user.Signature;
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
                basic.InOfficeComment = user.InOfficeComment;
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
        public GroupProxy[] GetTicketGroups()
        {
            var watch = Stopwatch.StartNew();

            Groups groups = new Groups(TSAuthentication.GetLoginUser());
            groups.LoadByOrganizationID(TSAuthentication.OrganizationID);
            List<GroupProxy> proxies = groups.GetGroupProxies().OrderBy(gp => gp.Name).ToList();

            Groups userGroups = new Groups(TSAuthentication.GetLoginUser());
            userGroups.LoadByUserID(TSAuthentication.UserID);

            foreach (Group userGroup in userGroups.OrderBy(ug => ug.Name))
            {
                GroupProxy proxy = proxies.Find(p => p.GroupID == userGroup.GroupID);

                if (proxy != null)
                {
                    proxies.Remove(proxy);
                    proxies.Insert(0, proxy);
                }
            }

            return proxies.ToArray();
        }

        [WebMethod]
        public BasicUser[] GetGroupUsers(int groupID)
        {
          Users users = new Users(TSAuthentication.GetLoginUser());
          users.LoadByGroupID(groupID);
          List<BasicUser> result = new List<BasicUser>();

          foreach (User user in users)
          {
            BasicUser basic = new BasicUser();
            basic.Name = user.FirstName + " " + user.LastName;
            basic.UserID = user.UserID;
            basic.InOfficeComment = user.InOfficeComment;
            result.Add(basic);
          }
          return result.ToArray();
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
        public string SaveUserSignature(int userID, string signature)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return null;
            user.Signature = signature;
            user.Collection.Save();
            return signature;
        }

        [WebMethod]
        public string SaveUserTitle(int userID, string title)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return null;
            user.Title = title.Trim();
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
        public string SaveUserLinkedin(int userID, string linkedin)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return null;

            user.LinkedIn = linkedin;
            user.Collection.Save();

            if (!linkedin.StartsWith("http://"))
                linkedin = "http://" + linkedin;
            return linkedin;
        }

        [WebMethod]
        public bool SetIsActive(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            if (value == false)
            {
                Organizations orgs = new Organizations(TSAuthentication.GetLoginUser());
                orgs.ResetDefaultSupportUser(TSAuthentication.GetLoginUser(), user.UserID);
            }

            user.IsActive = value;
            user.Collection.Save();
            user.EmailCountToMuroc(value);
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
        public bool SetUnassignedGroupNotify(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;

            user.ReceiveUnassignedGroupEmails = value;
            user.Collection.Save();
            return user.ReceiveUnassignedGroupEmails;
        }

        [WebMethod]
        public bool SetBlockEmail(int userID, bool value)
        {
          User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
          if (user.OrganizationID != TSAuthentication.OrganizationID) return value;

          user.BlockInboundEmail = value;
          user.Collection.Save();
          return user.BlockInboundEmail;
        }
        [WebMethod]
        public bool SetOnlyEmailAfterHours(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;

            user.OnlyEmailAfterHours = value;
            user.Collection.Save();
            return user.OnlyEmailAfterHours;
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
        public bool SetRestrictUserFromEditingAnyActions(int userID, bool value)
        {
          User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
          if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
          if (!TSAuthentication.IsSystemAdmin) return !value;

          user.RestrictUserFromEditingAnyActions = value;
          user.Collection.Save();
          return user.RestrictUserFromEditingAnyActions;
        }

        [WebMethod]
        public bool SetRestrictUserFromExportingData(int userID, bool value)
        {
          User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
          if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
          if (!TSAuthentication.IsSystemAdmin) return !value;

          user.DisableExporting = value;
          user.Collection.Save();
          return user.DisableExporting;
        }

        [WebMethod]
        public bool SetAllowUserToEditAnyAction(int userID, bool value)
        {
          User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
          if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
          if (!TSAuthentication.IsSystemAdmin) return !value;

          user.AllowUserToEditAnyAction = value;
          user.Collection.Save();
          return user.AllowUserToEditAnyAction;
        }

        [WebMethod]
        public bool SetUserCanPinAction(int userID, bool value)
        {
          User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
          if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
          if (!TSAuthentication.IsSystemAdmin) return !value;

          user.UserCanPinAction = value;
          user.Collection.Save();
          return user.UserCanPinAction;
        }

        [WebMethod]
        public bool SetChangeTicketVisibility(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            user.ChangeTicketVisibility = value;
            user.Collection.Save();
            return user.ChangeTicketVisibility;
        }

        [WebMethod]
        public bool SetInactiveFilter(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;

            user.FilterInactive = value;
            user.Collection.Save();
            return user.FilterInactive;
        }

        [WebMethod]
        public bool SetChangeCommunityVisibility(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            user.CanChangeCommunityVisibility = value;
            user.Collection.Save();
            return user.CanChangeCommunityVisibility;
        }

        [WebMethod]
        public bool SetChangeCanCreateCompany(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            user.CanCreateCompany = value;
            user.Collection.Save();
            return user.CanCreateCompany;
        }

        [WebMethod]
        public bool SetChangeCanEditCompany(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            user.CanEditCompany = value;
            user.Collection.Save();
            return user.CanEditCompany;
        }

        [WebMethod]
        public bool SetChangeCanCreateContacts(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            user.CanCreateContact = value;
            user.Collection.Save();
            return user.CanCreateContact;
        }

        [WebMethod]
        public bool SetChangeCanEditContacts(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            user.CanEditContact = value;
            user.Collection.Save();
            return user.CanEditContact;
        }

        [WebMethod]
        public bool SetChangeCanEditAssets(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            user.CanEditAsset = value;
            user.Collection.Save();
            return user.CanEditAsset;
        }

        [WebMethod]
        public bool SetChangeCanCreateAssets(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            user.CanCreateAsset = value;
            user.Collection.Save();
            return user.CanCreateAsset;
        }

        [WebMethod]
        public bool SetChangeCanCreateProducts(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            user.CanCreateProducts = value;
            user.Collection.Save();
            return user.CanCreateProducts;
        }

        [WebMethod]
        public bool SetChangeCanEditProducts(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            user.CanEditProducts = value;
            user.Collection.Save();
            return user.CanEditProducts;
        }

        [WebMethod]
        public bool SetChangeCanCreateVersions(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            user.CanCreateVersions = value;
            user.Collection.Save();
            return user.CanCreateVersions;
        }

        [WebMethod]
        public bool SetChangeCanEditVersions(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            user.CanEditVersions = value;
            user.Collection.Save();
            return user.CanEditVersions;
        }

        [WebMethod]
        public bool SetChangeKbVisibility(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            user.ChangeKBVisibility = value;
            user.Collection.Save();
            return user.ChangeKBVisibility;
        }

        [WebMethod]
        public bool SetAllowAnyTicketCustomer(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            user.AllowAnyTicketCustomer = value;
            user.Collection.Save();
            return user.AllowAnyTicketCustomer;
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

        [WebMethod]
        public string CreateNewContact(string emailAddress, string firstName, string lastName, string companyName, string phone, bool createNewCompany)
        {
            User user;
            Users users = new Users(TSAuthentication.GetLoginUser());

            OrganizationService orgService = new OrganizationService();
            
            Organization organization;
            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());

            int newOrgID = GetIDByExactName(companyName);
            string errorMsg, resultStr="";
            string email = emailAddress;

            if (createNewCompany == false)
            {
                // Check Process and Flow
                if (firstName.Length < 1 && lastName.Length < 1 && (email.Length < 1 || email.IndexOf('@') < 0 || email.IndexOf('.') < 0) && newOrgID == -1)
                {
                    errorMsg = "The company you have specified is invalid.";
                    return errorMsg;
                }

                if ((firstName.Length < 1 || lastName.Length < 1))
                {
                    errorMsg = "The name you have specified is invalid.  Please enter a valid name.";
                    return errorMsg;
                }

                /*if ((email.Length < 1 || email.IndexOf('@') < 0 || email.IndexOf('.') < 0))
                {
                    errorMsg = "The email you have specified is invalid.  Please choose another email.";
                    return errorMsg;
                }*/

                if (email != "" && !users.IsEmailValid(email, -1, newOrgID))
                {
                    errorMsg = "The email you have specified is already in use.  Please choose another email.";
                    return errorMsg;
                }

                if (companyName.Length > 0 && newOrgID == -1 && createNewCompany == false)
                {
                    errorMsg = "The company you have specified is invalid.";
                    return errorMsg;
                }

                if (firstName.Length < 1 && lastName.Length < 1 && email.Length < 1 && newOrgID != -1)
                {
                    errorMsg = "The company you have specified is already in use.  Please choose another company name.";
                    return errorMsg;
                }
            }
            // Create Org or Use Unknown
            if (companyName.Length < 1)
            {
                newOrgID = orgService.GetIDByName("_Unknown Company");
            }
            else if (createNewCompany == true)
            {

                organization = organizations.AddNewOrganization();
                organization.ParentID = TSAuthentication.GetLoginUser().OrganizationID;
                organization.PrimaryUserID = null;

                organization.ExtraStorageUnits = 0;
                organization.PortalSeats = 0;
                organization.UserSeats = 0;
                organization.IsCustomerFree = false;
                organization.ProductType = ProductType.Express;
                organization.HasPortalAccess = false;
                organization.IsActive = true;
                organization.IsBasicPortal = true;

                int? id = -1;
                organization.PrimaryUserID = id < 0 ? null : id;
                id = -1;
                organization.DefaultSupportUserID = id < 0 ? null : id;
                id = -1;
                organization.DefaultPortalGroupID = id < 0 ? null : id;
                id = -1;
                organization.DefaultSupportGroupID = id < 0 ? null : id;

                organization.TimeZoneID = "Dateline Standard Time";

                try
                {
                    TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(organization.TimeZoneID);
                }
                catch (Exception)
                {
                }
                organization.Name = companyName.Trim();
                organization.Website = "";
                organization.CompanyDomains = "";
                organization.Description = "";
                organization.SAExpirationDate = DataUtils.DateToUtc(TSAuthentication.GetLoginUser(), null);
                organization.SlaLevelID = null;

                organization.Collection.Save();

                newOrgID = organization.OrganizationID;

                resultStr = "o" + organization.OrganizationID.ToString();
            }

            // Create User
            if (firstName.Length > 0 && lastName.Length > 0)
            {
                user = users.AddNewUser();
                user.OrganizationID = newOrgID;
                user.LastLogin = DateTime.UtcNow;
                user.LastActivity = DateTime.UtcNow.AddHours(-1);
                user.IsPasswordExpired = true;
                user.ReceiveTicketNotifications = true;
                user.Email = emailAddress.Trim();
                user.FirstName = firstName.Trim();
                user.LastName = lastName.Trim();
                user.Title = "";
                user.MiddleName = "";
                user.IsActive = true;
                user.ActivatedOn = DateTime.UtcNow;
                user.IsPortalUser = false;
                user.EnforceSingleSession = true;
                
                user.Collection.Save();

                PhoneTypes phoneTypes = new PhoneTypes(TSAuthentication.GetLoginUser());
                phoneTypes.LoadByOrganizationID(TSAuthentication.OrganizationID);
                

                PhoneNumber p = new PhoneNumbers(TSAuthentication.GetLoginUser()).AddNewPhoneNumber();
                p.PhoneTypeID = phoneTypes[0].PhoneTypeID;
                p.Number = phone;
                p.RefType = ReferenceType.Users;
                p.RefID = user.UserID;

                p.Collection.Save();

                string password = DataUtils.GenerateRandomPassword();
                user.CryptedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5");
                user.IsPasswordExpired = true;
                user.Collection.Save();

                resultStr = "u" + user.UserID.ToString();
            }

            return resultStr;
        }
        
        [WebMethod]
        public OrganizationProxy[] GetUserCustomers(int userID)
        {
          Organizations orgs = new Organizations(TSAuthentication.GetLoginUser());
          orgs.LoadByUserRights(userID);
          return orgs.GetOrganizationProxies();
        }

        [WebMethod]
        public OrganizationProxy[] RemoveUserCustomer(int userID, int organizationID)
        {

          User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
          if (user.OrganizationID == TSAuthentication.OrganizationID && TSAuthentication.IsSystemAdmin)
          {
            user.Collection.RemoveUserCustomer(userID, organizationID);
            Organizations orgs = new Organizations(TSAuthentication.GetLoginUser());
            orgs.LoadByUserRights(userID);
            return orgs.GetOrganizationProxies();
          }
          return null;

        }

        [WebMethod]
        public OrganizationProxy[] AddUserCustomer(int userID, int organizationID)
        {
          User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
          if (user.OrganizationID == TSAuthentication.OrganizationID && TSAuthentication.IsSystemAdmin)
          {
            user.Collection.AddUserCustomer(userID, organizationID);
            Organizations orgs = new Organizations(TSAuthentication.GetLoginUser());
            orgs.LoadByUserRights(userID);
            return orgs.GetOrganizationProxies();
          }
          return null;
        }

        [WebMethod]
        public CustomValueProxy[] GetCustomValues(int userID)
        {
            User users = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (users.OrganizationID != TSAuthentication.OrganizationID) return null;
            CustomValues values = new CustomValues(users.Collection.LoginUser);
            values.LoadByReferenceType(TSAuthentication.OrganizationID, ReferenceType.Users, users.UserID);

            return values.GetCustomValueProxies();
        }

        [WebMethod]
        public AutocompleteItem[] AdminQueryUsers(int orgID, string query)
        {
          List<AutocompleteItem> result = new List<AutocompleteItem>();
          if (TSAuthentication.OrganizationID != 1078 && TSAuthentication.OrganizationID != 1088) return result.ToArray();
          Users users = new Users(TSAuthentication.GetLoginUser());
          users.LoadByName(query, orgID, false, false, false);
          foreach (User user in users)
          {
            result.Add(new AutocompleteItem(string.Format("{0} - {1} ({2:D})", user.FirstLastName, user.Email, user.UserID) , user.UserID.ToString()));
          }

          return result.ToArray();
        }

        [WebMethod]
        public UserProxy[] AdminGetUsers(int orgID)
        {
          if (TSAuthentication.OrganizationID != 1078 && TSAuthentication.OrganizationID != 1088) return null;
          Users users = new Users(TSAuthentication.GetLoginUser());
          users.LoadByOrganizationID(orgID, false);
          return users.GetUserProxies();
        }

      [WebMethod]
        public UserProxy AdminGetUser(int userID)
        {
          if (TSAuthentication.OrganizationID != 1078 && TSAuthentication.OrganizationID != 1088) return null;
          User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
          return user.GetProxy();
        }

        [WebMethod]
        public void SetSingleSessionEnforcement(int userID, bool value)
        {
          if (TSAuthentication.OrganizationID != 1078 && TSAuthentication.OrganizationID != 1088) return;
          User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
          user.EnforceSingleSession = value;
          user.Collection.Save();
        }

        [WebMethod]
        public void AdminSetActive(int userID, bool value)
        {
          if (TSAuthentication.OrganizationID != 1078 && TSAuthentication.OrganizationID != 1088) return;
          User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
          user.IsActive = value;
          user.Collection.Save();
        }

        public int GetIDByExactName(string name)
        {
            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByOrganizationNameActive(name, TSAuthentication.OrganizationID);
            if (organizations.IsEmpty) return -1;
            return organizations[0].OrganizationID;
        }

        [WebMethod]
        public string SetFontFamily(int userID, string value)
        {
          User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
          if (user.OrganizationID != TSAuthentication.OrganizationID) return value;

          user.FontFamily = (FontFamily)Convert.ToInt16(value);
          user.Collection.Save();

          return Enums.GetDescription(user.FontFamily);
        }

        [WebMethod]
        public string SetFontSize(int userID, string value)
        {
          User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
          if (user.OrganizationID != TSAuthentication.OrganizationID) return value;

          user.FontSize = (FontSize)Convert.ToInt16(value);
          user.Collection.Save();

          return Enums.GetDescription(user.FontSize);
        }

        [WebMethod]
        public string GetShortNameFromID(int userID)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);

            if (user.FirstLastName.Length > 10)
                return user.FirstLastName.Substring(0, 10).ToString() + "...";
            else
                return user.FirstLastName.ToString();
        }


        [WebMethod]
        public string GetUsersSearch(int orgID, string query, bool showInactive)
        {
            Users users = new Users(TSAuthentication.GetLoginUser());

            if (query == "")
                users.LoadByOrganizationID(orgID, !showInactive);
            else
                users.LoadByName(query, orgID, !showInactive, false, false);

            StringBuilder html = new StringBuilder();

            foreach (UserProxy u in users.GetUserProxies())
            {
                ChatUserSetting setting = ChatUserSettings.GetChatUserSetting(TSAuthentication.GetLoginUser(), u.UserID);
                string chatsetting = "";

                Users tempUser = new Users(TSAuthentication.GetLoginUser());
                int total = tempUser.GetUserTicketCount(u.UserID, false);
                

                if (setting != null)
                {
                    chatsetting = setting.IsAvailable  && u.IsChatUser ? "<i class='user-tooltip fa-comments-o fa color-red' title='Customer Chat Online'></i>" : "";
                }

                html.AppendFormat(@"<li>
                    <div class='row'>
                        <div class='col-xs-2 pl0'>
                            <div class='avatar'>
                                <img id='userPhoto' src='{0}'>
                            </div>
                        </div>
                        <div class='col-xs-10'>
                            <strong><a class='user' uid='{5}'>{1} ({7}){6}</a></strong>
                            <div>{2}<div class='pull-right'>{3}{4}</div></div>
                        </div>
                    </div></li>",
                                u.Avatar,
                                u.FirstName + " " + u.LastName,
                                u.Title,
                                u.AppChatStatus == true ? chatsetting:"",
                                (u.AppChatStatus == true && u.UserID != TSAuthentication.GetLoginUser().UserID) ? "<i class='user-tooltip fa-comment fa color-green user-chat' cid='"+u.UserID+"' title='Online to Chat'></i>":"",
                                u.UserID,
                                u.IsActive ? "":"<i>(Inactive)</i>",
                                total
                                );
            }

            return html.ToString();

        }


        [WebMethod]
        public string GetNewUserMessage()
        {
            int userCount = Organizations.GetUserCount(TSAuthentication.GetLoginUser(), TSAuthentication.GetLoginUser().OrganizationID);
            Organization organization = (Organization)Organizations.GetOrganization(TSAuthentication.GetLoginUser(), TSAuthentication.GetLoginUser().OrganizationID);
            string newusermessage="";

            User u = Users.GetUser(TSAuthentication.GetLoginUser(),TSAuthentication.GetLoginUser().UserID);

            if (organization.UserSeats <= userCount)
            {
                if (u.IsFinanceAdmin)
                {
                    newusermessage = "You have exceeded the number of user seat licenses.  If you would like to add additional users to your account, please contact our sales team at 800.596.2820 x806, or send an email to sales@teamsupport.com";
                }
                else
                {
                    Users users = new Users(TSAuthentication.GetLoginUser());
                    users.LoadFinanceAdmins(TSAuthentication.GetLoginUser().OrganizationID);
                    if (users.IsEmpty)
                    {
                        newusermessage = "Please ask your billing administrator to purchase additional user seat licenses.";
                    }
                    else
                    {
                        newusermessage = "Please ask your billing administrator (" + users[0].FirstLastName + ") to purchase additional user seat licenses.";
                    }
                }
            }
            else
            {
                newusermessage = "";
            }

            return newusermessage;
        }


      [DataContract]
        public class BasicUser
        {
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public int UserID { get; set; }
            [DataMember]
            public string InOfficeComment { get; set; }
        }


    }
}