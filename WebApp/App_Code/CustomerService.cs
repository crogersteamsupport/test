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
    public class CustomerService : System.Web.Services.WebService
    {

        public CustomerService() { }

        [WebMethod]
        public UserProxy GetUser(int userID)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            return user.GetProxy();
        }

        [WebMethod]
        public string SetCompanyName(int orgID, string value)
        {
            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
            o.Name = value;
            o.Collection.Save();
            return value;
        }
        [WebMethod]
        public string SetCompanyTimezone(int orgID, string value)
        {
            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
            o.TimeZoneID = value;
            o.Collection.Save();

            return value;
        }

        [WebMethod]
        public string SetCompanyWeb(int orgID, string value)
        {
            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
            o.Website = value;
            o.Collection.Save();
            return value;
        }
        [WebMethod]
        public int SetCompanyPrimaryContact(int orgID, int value)
        {
            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
            o.PrimaryUserID = value;
            o.Collection.Save();
            return value;
        }
        [WebMethod]
        public int SetCompanyDefaultSupportUser(int orgID, int value)
        {
            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
            o.DefaultSupportUserID = value;
            o.Collection.Save();
            return value;
        }
        [WebMethod]
        public int SetCompanyDefaultSupportGroup(int orgID, int value)
        {
            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
            o.DefaultSupportGroupID = value;
            o.Collection.Save();
            return value;
        }
        [WebMethod]
        public int SetCompanyDefaultPortalGroup(int orgID, int value)
        {
            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
            o.DefaultPortalGroupID = value;
            o.Collection.Save();
            return value;
        }
        [WebMethod]
        public string SetCompanyDomain(int orgID, string value)
        {
            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
            o.CompanyDomains = value;
            o.Collection.Save();
            return value;
        }
        [WebMethod]
        public bool SetCompanyActive(int orgID, bool value)
        {
            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
            o.IsActive = value;
            o.Collection.Save();
            return value;
        }
        [WebMethod]
        public bool SetCompanyPortalAccess(int orgID, bool value)
        {
            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
            o.HasPortalAccess = value;
            o.Collection.Save();
            return value;
        }
        [WebMethod]
        public bool SetCompanyAPIEnabled(int orgID, bool value)
        {
            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
            o.IsApiActive = value;
            o.Collection.Save();
            return value;
        }
        [WebMethod]
        public string SetCompanySAE(int orgID, string value)
        {
            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
            o.SAExpirationDate = DataUtils.DateToUtc(TSAuthentication.GetLoginUser(), Convert.ToDateTime(value));
            o.Collection.Save();
            return value;
        }
        [WebMethod]
        public int SetCompanySLA(int orgID, int value)
        {
            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
            o.SlaLevelID = value;
            o.Collection.Save();
            return value;
        }
        [WebMethod]
        public int SetCompanySupportHours(int orgID, int value)
        {
            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
            o.SupportHoursMonth = value;
            o.Collection.Save();
            return value;
        }
        [WebMethod]
        public string SetCompanyDescription(int orgID, string value)
        {
            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
            o.Description = value;
            o.Collection.Save();
            return value;
        }
        [WebMethod]
        public string SetCompanyInactive(int orgID, string value)
        {
            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
            o.InActiveReason = value;
            o.Collection.Save();
            return value;
        }

        [WebMethod]
        public string SetContactEmail(int userID, string email)
        {
            User u = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            u.Email = email;
            u.Collection.Save();
            return email != "" ? email : "Empty"; ;
        }
        [WebMethod]
        public string SetContactName(int userID, string fname, string mname, string lname)
        {
            User u = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            u.FirstName = fname;
            u.MiddleName = mname;
            u.LastName = lname;
            u.Collection.Save();
            return u.FirstLastName;
        }

        [WebMethod]
        public string SetContactCompany(int userID, int value)
        {
            User u = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            u.OrganizationID = value;
            u.Collection.Save();
            Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), value);
            return organization.Name != "" ? organization.Name : "Empty";
        }
        [WebMethod]
        public string SetContactTitle(int userID, string title)
        {
            User u = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            u.Title = title;
            u.Collection.Save();
            return title != "" ? title : "Empty";
        }
        [WebMethod]
        public bool SetContactActive(int userID, bool value)
        {
            User u = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            u.IsActive = value;
            u.Collection.Save();
            return value;
        }
        [WebMethod]
        public bool SetContactPortalUser(int userID, bool value)
        {
            User u = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            u.IsPortalUser = value;
            u.Collection.Save();
            return value;
        }
        [WebMethod]
        public bool SetContactPreventEmail(int userID, bool value)
        {
            User u = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            u.BlockInboundEmail = value;
            u.Collection.Save();
            return value;
        }
        [WebMethod]
        public bool SetContactSystemAdmin(int userID, bool value)
        {
            User u = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            u.IsSystemAdmin = value;
            u.Collection.Save();
            return value;
        }
        [WebMethod]
        public bool SetContactFinancialAdmin(int userID, bool value)
        {
            User u = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            u.IsFinanceAdmin = value;
            u.Collection.Save();
            return value;
        }
        [WebMethod]
        public bool IsTSUser()
        {
            bool _isAdmin = false;
            //_isAdmin = (UserSession.LoginUser.OrganizationID != _organizationID) || UserSession.CurrentUser.IsSystemAdmin;
            //_isAdmin = _isAdmin && (UserSession.CurrentUser.IsSystemAdmin || !UserSession.CurrentUser.IsAdminOnlyCustomers);

            return _isAdmin;
        }

        [WebMethod]
        public bool CanEdit()
        {
            return TSAuthentication.GetOrganization(TSAuthentication.GetLoginUser()).ParentID == null;
        }

        [WebMethod]
        public string LoadContactProperties(int userID)
        {
            StringBuilder html = new StringBuilder("");
            Users users = new Users(TSAuthentication.GetLoginUser());
            users.LoadByUserID(userID);
            User user = users[0];

            html.AppendLine(CreateFormElement("Email", user.Email, "editable"));
            html.AppendLine(CreateFormElement("Title", user.Title, "editable"));
            html.AppendLine(CreateFormElement("Active", user.IsActive, "editable"));
            html.AppendLine(CreateFormElement("Portal User", user.IsPortalUser, "editable"));

            Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), user.OrganizationID);
            html.AppendLine(CreateFormElement("Company", organization.Name, "editable"));
            html.AppendLine(CreateFormElement("Prevent email from creating tickets", user.BlockInboundEmail, "editable"));

            if (TSAuthentication.GetOrganization(TSAuthentication.GetLoginUser()).ParentID == null)
            {
                html.AppendLine(CreateFormElement("System Administrator:", user.IsSystemAdmin, "editable"));
                html.AppendLine(CreateFormElement("Financial Administrator:", user.IsFinanceAdmin, "editable"));
                //html.AppendLine(CreateFormElement("Portal User:", user.IsPortalUser));
                html.AppendLine(CreateFormElement("Last Logged In", user.LastLogin.ToString("g", TSAuthentication.GetLoginUser().CultureInfo), "db"));
                html.AppendLine(CreateFormElement("In Office", user.InOffice, "db"));
                html.AppendLine(CreateFormElement("In Office Comment", user.InOfficeComment, "db"));
                if (user.IsActive)
                    html.AppendLine(CreateFormElement("Activated On", user.ActivatedOn.ToString("g", TSAuthentication.GetLoginUser().CultureInfo), "db"));
                else
                {
                    if (user.DeactivatedOn != null)
                    {
                        DateTime dateTime = (DateTime)user.DeactivatedOn;
                        html.AppendLine(CreateFormElement("Deactivated On", dateTime.ToString("g", TSAuthentication.GetLoginUser().CultureInfo), "db"));
                    }
                    else
                    {
                        html.AppendLine(CreateFormElement("Deactivated On", ""));
                    }
                }
            }
            else
            {
                if (TSAuthentication.GetOrganization(TSAuthentication.GetLoginUser()).HasPortalAccess)
                {
                    //html.AppendLine(CreateFormElement("Portal User:", user.IsPortalUser));
                    if (user.IsPortalUser)
                        html.AppendLine(CreateFormElement("Last Logged In", user.LastLogin.ToString("g", TSAuthentication.GetLoginUser().CultureInfo), "db"));
                }
            }



            return html.ToString();

        }

        public string CreateFormElement(string fieldTitle, object fieldValue, string editable="")
        {
            StringBuilder form = new StringBuilder("");

            if (fieldValue is bool)
            {
                if ((bool)fieldValue == true)
                    fieldValue = "Yes";
                else if ((bool)fieldValue == false)
                    fieldValue = "No";

            }

            if(editable == "db")
            form.AppendFormat(@"<div class='form-group'>
                                <label class='col-xs-4 control-label' for='field{0}'>{1}</label>
                                <div class='col-xs-8'>
                                <span class='form-control-static {3}' id='field{0}'>{2}</span>
                                </div>
                                </div>",fieldTitle.Replace(" ",""), fieldTitle, fieldValue != null ? fieldValue : "Empty", editable );
            else
                form.AppendFormat(@"<div class='form-group'>
                                <label class='col-xs-4 control-label' for='field{0}'>{1}</label>
                                <div class='col-xs-8'>
                                <p class='form-control-static {3}' id='field{0}'>{2}</p>
                                </div>
                                </div>", fieldTitle.Replace(" ", ""), fieldTitle, fieldValue != null ? fieldValue : "Empty", editable);

            return form.ToString();
        }

        [WebMethod]
        public string GetSearchResults(string filter, int startIndex = 1)
        {
            StringBuilder builder = new StringBuilder();
            SearchService s = new SearchService();
            if (filter == "")
                filter = "xfirstword";

            CompaniesAndContactsSearchResults test = s.SearchCompaniesAndContacts(filter, startIndex, 20, true, true);

            if (test.Items.Length > 0)
            {
                foreach (CompanyOrContact item in test.Items)
                {
                    if(item.ReferenceType == ReferenceType.Organizations)
                        builder.Append(CreateCompanyBox(item.Id));
                    else if(item.ReferenceType == ReferenceType.Contacts)
                        builder.Append(CreateContactBox(item.Id));
                }

                return builder.ToString();
            }
            else
            {
                return CreateNoResults();
            }

        }

        [WebMethod]
        public string GetCompanies(string filter, int startIndex = 1)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            StringBuilder builder = new StringBuilder();
            Organizations organizations = new Organizations(loginUser);
            int i = 0;

            //organizations.CustomerLoadByLikeOrganizationName(loginUser.OrganizationID, filter, false, startIndex, true);

            SearchService s = new SearchService();
            if (filter == "")
                filter = "xfirstword";
            CompaniesAndContactsSearchResults test = s.SearchCompaniesAndContacts(filter, startIndex, 20, true, false);

            if (test.Items.Length > 0)
            {
                foreach (CompanyOrContact item in test.Items)
                {
                    builder.Append(CreateCompanyBox(item.Id));
                }

                return builder.ToString();
            }
            else
            {
                return CreateNoResults();
            }
            //if (organizations.Count > 0)
            //{

            //    foreach (Organization item in organizations)
            //    {
            //            builder.Append(CreateCompanyBox(item));
            //    }

            //    return builder.ToString();
            //}
            //else
            //{
            //    return CreateNoResults();
            //}
            
        }

        [WebMethod]
        public string GetContacts(string filter, int startIndex = 1)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            StringBuilder builder = new StringBuilder();
            UsersView users = new UsersView(loginUser);

            if (filter == "")
                filter = "xfirstword";

            SearchService s = new SearchService();
            CompaniesAndContactsSearchResults test = s.SearchCompaniesAndContacts(filter, startIndex, 20, false, true);

            if (test.Items.Length > 0)
            {
                foreach (CompanyOrContact item in test.Items)
                {
                    builder.Append(CreateContactBox(item.Id));
                }

                return builder.ToString();
            }
            else
            {
                return CreateNoResults();
            }


            //users.CustomerLoadByLikeName(loginUser.OrganizationID, filter, startIndex, true);

            //if (users.Count > 0)
            //{            
            
            //    foreach (UsersViewItem item in users)
            //    {
            //            builder.Append(CreateContactBox(item));
            //    }

            //    return builder.ToString();
            //}
            //else
            //{
            //    return CreateNoResults();
            //}

        }

        [WebMethod]
        public string GetMoreResults(string filterType, string filter, int startIndex)
        {
            string results = "";

            switch (filterType)
            {
                case "all":
                    results = GetSearchResults(filter, startIndex);
                    break;
                case "customers":
                    results = GetCompanies(filter, startIndex);
                    break;
                case "contacts":
                    results = GetContacts(filter, startIndex);
                    break;
                
            }

            if (results.Contains("No Search Results Found!"))
                return "";
            else
                return results;
        }

        [WebMethod]
        public string UpdateRecentlyViewed(string viewid)
        {
            int refType, refID;

            if (viewid.StartsWith("u"))
                refType = 0;
            else
                refType = 1;

            refID = Convert.ToInt32(viewid.Substring(1));
            
            RecentlyViewedItem recent = (new RecentlyViewedItems(TSAuthentication.GetLoginUser()).AddNewRecentlyViewedItem());
            
            
            recent.RefID = refID;
            recent.RefType = refType;
            recent.DateViewed = DateTime.UtcNow;
            recent.UserID = TSAuthentication.GetLoginUser().UserID;
            recent.BaseCollection.Save();

            return GetRecentlyViewed();
        }

        [WebMethod]
        public string GetRecentlyViewed()
        {
            StringBuilder builder = new StringBuilder();
            RecentlyViewedItems recent = new RecentlyViewedItems(TSAuthentication.GetLoginUser());
            recent.LoadRecent(TSAuthentication.GetLoginUser().UserID);

            builder.Append(@"<ul class=""recent-list"">");
            foreach (RecentlyViewedItem item in recent)
            {
                builder.Append(CreateRecentlyViewed(item));
            }
            builder.Append("</ul>");
            return builder.ToString();
        }

        [WebMethod]
        public SlaLevelProxy[] LoadSlas()
        {
            if (!TSAuthentication.IsSystemAdmin) return null;
            SlaLevels table = new SlaLevels(TSAuthentication.GetLoginUser());
            table.LoadByOrganizationID(TSAuthentication.OrganizationID);
            return table.GetSlaLevelProxies();
        }

        [WebMethod]
        public SlaLevelProxy[] LoadOrgSlas()
        {
            if (!TSAuthentication.IsSystemAdmin) return null;
            SlaLevels table = new SlaLevels(TSAuthentication.GetLoginUser());
            table.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID);
            return table.GetSlaLevelProxies();
        }

        [WebMethod]
        public System.Collections.ObjectModel.ReadOnlyCollection<TimeZoneInfo> LoadTimeZones()
        {
            System.Collections.ObjectModel.ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
            return timeZones;
        }

        [WebMethod]
        public UserProxy[] LoadUsers()
        {
            Users users = new Users(TSAuthentication.GetLoginUser());
            users.LoadByOrganizationID(TSAuthentication.OrganizationID, true);

            return users.GetUserProxies();
        }

        [WebMethod]
        public UserProxy[] LoadOrgUsers(int organizationID)
        {
            Users users = new Users(TSAuthentication.GetLoginUser());
            users.LoadByOrganizationID(organizationID, true);
            return users.GetUserProxies();
        }

        [WebMethod]
        public UserProxy[] LoadOrgSupportUsers(int organizationID)
        {
            Users users = new Users(TSAuthentication.GetLoginUser());
            users.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID, true);
            return users.GetUserProxies();
        }

        [WebMethod]
        public GroupProxy[] LoadOrgGroups(int organizationID)
        {
            Groups groups = new Groups(TSAuthentication.GetLoginUser());
            groups.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID);
            return groups.GetGroupProxies();
        }

        [WebMethod]
        public GroupProxy[] LoadGroups()
        {
            Groups groups = new Groups(TSAuthentication.GetLoginUser());
            groups.LoadByOrganizationID(TSAuthentication.OrganizationID);

            return groups.GetGroupProxies();
        }

        [WebMethod]
        public CustomValueProxy[] GetCustomValues(int refID, ReferenceType refType)
        {
            CustomValues values = new CustomValues(TSAuthentication.GetLoginUser());
            values.LoadByReferenceType(TSAuthentication.OrganizationID, refType, refID);
            return values.GetCustomValueProxies();
        }

        [WebMethod]
        public string LoadCustomProperties(int refID, ReferenceType refType){
            CustomFields fields = new CustomFields(TSAuthentication.GetLoginUser());
            fields.LoadByReferenceType(TSAuthentication.OrganizationID, refType, -1);
            
            CustomFieldCategories cats = new CustomFieldCategories(TSAuthentication.GetLoginUser());
            cats.LoadByRefType(refType, -1);

            StringBuilder htmltest = new StringBuilder("");
            foreach (CustomField field in fields){
                if (field.CustomFieldCategoryID == null){
                    switch (field.FieldType)
                    {
                        case CustomFieldType.Text: htmltest.AppendLine(CreateTextControl(field, true, refID)); break;
                        case CustomFieldType.Number: htmltest.AppendLine(CreateNumberControl(field, true, refID)); break;
                        case CustomFieldType.DateTime: htmltest.AppendLine(CreateDateTimeControl(field, true, refID)); break;
                        case CustomFieldType.Boolean: htmltest.AppendLine(CreateBooleanControl(field, true, refID)); break;
                        case CustomFieldType.PickList: htmltest.AppendLine(CreatePickListControl(field, true, refID)); break;
                        default: break;
                    }
                }
            }

            foreach (CustomFieldCategory cat in cats){

                foreach (CustomField field in fields)
                {
                    if (field.CustomFieldCategoryID == cat.CustomFieldCategoryID)
                    {
                        switch (field.FieldType)
                        {
                            case CustomFieldType.Text: htmltest.AppendLine(CreateTextControl(field, true, refID)); break;
                            case CustomFieldType.Number: htmltest.AppendLine(CreateNumberControl(field, true, refID)); break;
                            case CustomFieldType.DateTime: htmltest.AppendLine(CreateDateTimeControl(field, true, refID)); break;
                            case CustomFieldType.Boolean: htmltest.AppendLine(CreateBooleanControl(field, true, refID)); break;
                            case CustomFieldType.PickList: htmltest.AppendLine(CreatePickListControl(field, true, refID)); break;
                            default: break;
                        }
                    }
                }
            }


            return htmltest.ToString();
        }

        [WebMethod]
        public string LoadCustomControls(ReferenceType refType){
            CustomFields fields = new CustomFields(TSAuthentication.GetLoginUser());
            fields.LoadByReferenceType(TSAuthentication.OrganizationID, refType, -1);
            int count = 0;
            CustomFieldCategories cats = new CustomFieldCategories(TSAuthentication.GetLoginUser());
            cats.LoadByRefType(refType, -1);

            StringBuilder htmltest = new StringBuilder("");

            htmltest.Append("<div class='form-group'>");

            foreach (CustomField field in fields)
            {
                if (count == 0)
                {
                    htmltest.Append("<div class='row'>");
                    count++;
                }

                if (field.CustomFieldCategoryID == null)
                {
                    htmltest.AppendFormat("<div class='col-xs-4'><label for='{0}' class='col-xs-4 control-label'>{1}</label>", field.CustomFieldID, field.Name);
                    switch (field.FieldType)
                    {
                        case CustomFieldType.Text: htmltest.AppendLine(CreateTextControl(field)); break;
                        case CustomFieldType.Number: htmltest.AppendLine(CreateNumberControl(field)); break;
                        case CustomFieldType.DateTime: htmltest.AppendLine(CreateDateTimeControl(field)); break;
                        case CustomFieldType.Boolean: htmltest.AppendLine(CreateBooleanControl(field)); break;
                        case CustomFieldType.PickList: htmltest.AppendLine(CreatePickListControl(field)); break;
                        default: break;
                    }
                    htmltest.Append("</div>");
                    count++;
                }

                if (count % 4 == 0)
                {
                    htmltest.Append("</div>"); //end row
                    count = 0;
                }
            }
            if (count != 0)
            {
                count = 0;
                htmltest.Append("</div>"); // end row if not closed
            }
            htmltest.Append("</div>"); //end form-group

            count = 0;

            foreach (CustomFieldCategory cat in cats)
            {

                htmltest.AppendFormat("<h3>{0}</h3>",cat.Category);
                htmltest.Append("<div class='form-group'>");

                foreach (CustomField field in fields)
                {
                    if (count == 0)
                    {
                        htmltest.Append("<div class='row'>");
                        count++;
                    }

                    if (field.CustomFieldCategoryID == cat.CustomFieldCategoryID)
                    {
                        htmltest.AppendFormat("<div class='col-xs-4'><label for='{0}' class='col-xs-4 control-label'>{1}</label>", field.CustomFieldID, field.Name);
                        switch (field.FieldType)
                        {
                            case CustomFieldType.Text: htmltest.AppendLine(CreateTextControl(field)); break;
                            case CustomFieldType.Number: htmltest.AppendLine(CreateNumberControl(field)); break;
                            case CustomFieldType.DateTime: htmltest.AppendLine(CreateDateTimeControl(field)); break;
                            case CustomFieldType.Boolean: htmltest.AppendLine(CreateBooleanControl(field)); break;
                            case CustomFieldType.PickList: htmltest.AppendLine(CreatePickListControl(field)); break;
                            default: break;
                        }
                        htmltest.Append("</div>");
                        count++;
                    }

                    if (count % 4 == 0)
                    {
                        htmltest.Append("</div>");
                        count = 0;
                    }
                }
                if (count != 0){
                    count = 0;
                    htmltest.Append("</div>");
                }
                htmltest.Append("</div>");
            }

            return htmltest.ToString();
        }

        [WebMethod]
        public string LoadCustomContactControls()
        {
            int count = 0;
            StringBuilder htmltest = new StringBuilder("");
            CustomFields fields = new CustomFields(TSAuthentication.GetLoginUser());
            fields.LoadByReferenceType(TSAuthentication.OrganizationID, ReferenceType.Contacts, -1);

            if (fields.Count > 0)
                htmltest.Append("<div class='form-group'>");

                foreach (CustomField field in fields)
                {
                    if (count == 0)
                    {
                        htmltest.Append("<div class='row'>");
                        count++;
                    }


                    htmltest.AppendFormat("<div class='col-xs-4'><label for='{0}' class='col-xs-4 control-label'>{1}</label>", field.CustomFieldID, field.Name);
                    switch (field.FieldType)
                    {
                        case CustomFieldType.Text: htmltest.AppendLine(CreateTextControl(field)); break;
                        case CustomFieldType.Number: htmltest.AppendLine(CreateTextControl(field)); break;
                        case CustomFieldType.DateTime: htmltest.AppendLine(CreateDateTimeControl(field)); break;
                        case CustomFieldType.Boolean: htmltest.AppendLine(CreateBooleanControl(field)); break;
                        case CustomFieldType.PickList: htmltest.AppendLine(CreatePickListControl(field)); break;
                        default: break;
                    }
                    htmltest.Append("</div>");
                    count++;


                    if (count % 4 == 0)
                    {
                        htmltest.Append("</div>");
                        count = 0;
                    }
                }
                if (count != 0)
                {
                    count = 0;
                    htmltest.Append("</div>");
                }
                htmltest.Append("</div>");

            return htmltest.ToString();
        }

        [WebMethod]
        public string[] LoadcustomProductHeaders(int organizationID)
        {
            List<string> columns = new List<string>();
            string col;
            CustomFields fields = new CustomFields(TSAuthentication.GetLoginUser());
            fields.LoadByReferenceType(TSAuthentication.GetLoginUser().OrganizationID, ReferenceType.OrganizationProducts);
            int count = 0;
           

            foreach (CustomField field in fields)
            {
                if (count >= 25) break;
                columns.Add(field.Name);
                count++;
            }
            

            return columns.ToArray();
        }

        [WebMethod]
        public ActionLogProxy[] LoadHistory(int organizationID)
        {
            ActionLogs actionLogs = new ActionLogs(TSAuthentication.GetLoginUser());
            actionLogs.LoadByOrganizationID(organizationID);

            return actionLogs.GetActionLogProxies();
        }

        [WebMethod]
        public NoteProxy[] LoadNotes(int refID, ReferenceType refType)
        {
            Notes notes = new Notes(TSAuthentication.GetLoginUser());
            notes.LoadByReferenceType(refType, refID);

            return notes.GetNoteProxies();
        }

        [WebMethod]
        public NoteProxy LoadNote(int noteID)
        {
            Notes notes = new Notes(TSAuthentication.GetLoginUser());
            notes.LoadByNoteID(noteID);

            return notes[0].GetProxy();
        }

        [WebMethod]
        public AttachmentProxy[] LoadFiles(int refID, ReferenceType refType)
        {
            Attachments attachments = new Attachments(TSAuthentication.GetLoginUser());
            attachments.LoadByReference(refType, refID);

            return attachments.GetAttachmentProxies();
        }

        [WebMethod]
        public OrganizationCustomProduct[] LoadProducts(int organizationID)
        {
            OrganizationProducts organizationProducts = new OrganizationProducts(TSAuthentication.GetLoginUser());
            organizationProducts.LoadForCustomerProductGrid(organizationID);
            List<OrganizationCustomProduct> list = new List<OrganizationCustomProduct>();
            CustomFields fields = new CustomFields(TSAuthentication.GetLoginUser());
            fields.LoadByReferenceType(TSAuthentication.GetLoginUser().OrganizationID, ReferenceType.OrganizationProducts);


            foreach (DataRow row in organizationProducts.Table.Rows)
            {
                OrganizationCustomProduct test = new OrganizationCustomProduct();
                test.ProductName = row["ProductName"].ToString();
                test.VersionNumber = row["VersionNumber"].ToString();
                test.SupportExpiration = row["SupportExpiration"].ToString() != "" ? DataUtils.DateToLocal(TSAuthentication.GetLoginUser(), (DateTime)row["SupportExpiration"]).ToString(GetDateFormat()) : "";
                test.VersionStatus = row["VersionStatus"].ToString();
                test.IsReleased = row["IsReleased"].ToString();
                test.ReleaseDate = row["ReleaseDate"].ToString() != "" ? DataUtils.DateToLocal(TSAuthentication.GetLoginUser(), (DateTime)row["ReleaseDate"]).ToString(GetDateFormat()) : "";
                test.OrganizationProductID = (int)row["OrganizationProductID"];
                test.CustomFields = new List<string>();
                foreach (CustomField field in fields)
                {
                    CustomValue customValue = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, test.OrganizationProductID);
                    test.CustomFields.Add(customValue.Value);
                }


                list.Add(test);
            }


            return list.ToArray();
        }

        
        [WebMethod]
        public OrganizationCustomProduct LoadProduct(int productID)
        {
            OrganizationProduct organizationProduct = (OrganizationProduct)OrganizationProducts.GetOrganizationProduct(TSAuthentication.GetLoginUser(), productID);
            OrganizationCustomProduct custProd = new OrganizationCustomProduct();

            custProd.ProductName = organizationProduct.ProductID.ToString();
            custProd.VersionNumber = organizationProduct.ProductVersionID.HasValue ? organizationProduct.ProductVersionID.ToString() : "-1";

            if (organizationProduct.SupportExpiration.HasValue)
                custProd.SupportExpiration = ((DateTime)organizationProduct.SupportExpiration).ToString(GetDateFormat());
            else
                custProd.SupportExpiration = "";

            custProd.VersionStatus = "";
            custProd.IsReleased = "";
            custProd.ReleaseDate = null;
            custProd.OrganizationProductID = organizationProduct.OrganizationProductID;
            custProd.ProductID = organizationProduct.ProductID;

            return custProd;
        }

        [WebMethod]
        public PhoneNumberProxy[] LoadPhoneNumbers(int refID, ReferenceType refType)
        {
            PhoneNumbers phoneNumbers = new PhoneNumbers(TSAuthentication.GetLoginUser());
            phoneNumbers.LoadByID(refID, refType);

            return phoneNumbers.GetPhoneNumberProxies();
        }

        [WebMethod]
        public ProductProxy[] LoadProductTypes()
        {
            Products products = new Products(TSAuthentication.GetLoginUser());
            products.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID);

            return products.GetProductProxies();
        }

        [WebMethod]
        public ProductVersionProxy[] LoadProductVersions(int productID)
        {
            ProductVersions productVersions = new ProductVersions(TSAuthentication.GetLoginUser());
            productVersions.LoadByProductID(productID);

            return productVersions.GetProductVersionProxies();
        }

        [WebMethod]
        public List<CustomValueProxy> LoadCustomProductFields(int productID)
        {
            CustomFields fields = new CustomFields(TSAuthentication.GetLoginUser());
            List<CustomValueProxy> custfield = new List<CustomValueProxy>();
            fields.LoadByReferenceType(TSAuthentication.GetLoginUser().OrganizationID, ReferenceType.OrganizationProducts);
            foreach (CustomField field in fields)
            {
                CustomValue customValue = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, productID);
                custfield.Add(customValue.GetProxy());
            }

            return custfield;
        }

        [WebMethod]
        public PhoneNumberProxy[] LoadPhoneNumber(int phoneNumberID)
        {
            PhoneNumbers phoneNumbers = new PhoneNumbers(TSAuthentication.GetLoginUser());
            phoneNumbers.LoadByPhoneID(phoneNumberID);

            return phoneNumbers.GetPhoneNumberProxies();
        }

        [WebMethod]
        public AddressProxy[] LoadAddresses(int refID, ReferenceType refType)
        {
            Addresses addresses = new Addresses(TSAuthentication.GetLoginUser());
            addresses.LoadByID(refID, refType);

            return addresses.GetAddressProxies();
        }

        [WebMethod]
        public AddressProxy[] LoadAddress(int addressID)
        {
            Addresses addresses = new Addresses(TSAuthentication.GetLoginUser());
            addresses.LoadByAddressID(addressID);

            return addresses.GetAddressProxies();
        }

        [WebMethod]
        public PhoneTypeProxy[] LoadPhoneTypes(int organizationID)
        {
            PhoneTypes phoneTypes = new PhoneTypes(TSAuthentication.GetLoginUser());
            phoneTypes.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID);

            return phoneTypes.GetPhoneTypeProxies();
        }

        [WebMethod]
        public OrgProp GetProperties(int organizationID)
        {
            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByOrganizationID(organizationID);
            Users users = new Users(TSAuthentication.GetLoginUser());

            OrgProp orgProp = new OrgProp();

            if (organizations.IsEmpty) return null;

            orgProp.SAED = organizations[0].SAExpirationDate == null ? "[None]" : ((DateTime)organizations[0].SAExpirationDate).ToLongDateString();
            if (organizations[0].SlaLevelID == null)
                orgProp.SLA="[None]";
            else
            {
              SlaLevel level = SlaLevels.GetSlaLevel(TSAuthentication.GetLoginUser(), (int)organizations[0].SlaLevelID);
              if (level != null) 
                  orgProp.SLA = level.Name;
            }
            string primaryUser = "Empty";
            if (organizations[0].PrimaryUserID != null)
            {
                users.LoadByUserID((int)organizations[0].PrimaryUserID);
                primaryUser = users.IsEmpty ? "" : users[0].LastName + ", " + users[0].FirstName;
            }
            orgProp.PrimaryUser = primaryUser;
            orgProp.orgproxy = organizations[0].GetProxy();

            if (organizations[0].DefaultSupportUserID != null)
            {
                User supportUser = Users.GetUser(TSAuthentication.GetLoginUser(), (int)organizations[0].DefaultSupportUserID);
                orgProp.DefaultSupportUser = supportUser.FirstLastName;
            }
            else
            {
                orgProp.DefaultSupportUser = "Empty";
            }

            if (organizations[0].DefaultSupportGroupID != null)
            {
                Group supportGroup = (Group)Groups.GetGroup(TSAuthentication.GetLoginUser(), (int)organizations[0].DefaultSupportGroupID);
                if (supportGroup != null)
                    orgProp.SupportGroup = supportGroup.Name;
                else
                    orgProp.SupportGroup = "Empty";
            }
            else
            {
                orgProp.SupportGroup = "Empty";
            }

            return orgProp;

        }

        [WebMethod]
        public string LoadContacts(int organizationID){
            StringBuilder htmlresults = new StringBuilder("");
            StringBuilder phoneResults = new StringBuilder("");
            Users users = new Users(TSAuthentication.GetLoginUser());
            users.LoadByOrganizationID(organizationID, false);

            foreach (User u in users)
            {

                PhoneNumbers phoneNumbers = new PhoneNumbers(TSAuthentication.GetLoginUser());
                phoneNumbers.LoadByID(u.UserID, ReferenceType.Users);

                foreach (PhoneNumber p in phoneNumbers)
                {
                    phoneResults.AppendFormat("<p class='list-group-item-text'><span class='glyphicon glyphicon-earphone'></span> {0}: {1} {2}</p>", p.PhoneTypeName, p.Number, p.Extension == "" ? "":"Ext:" + p.Extension);
                }

                htmlresults.AppendFormat(@"<div class='list-group-item'>
                            <span class='pull-right {0}'>{1}</span><a href='#' id='{7}' class='contactlink'><h4 class='list-group-item-heading'>{2}</h4></a>
                            <div class='row'>
                                <div class='col-xs-6'>
                                    <p class='list-group-item-text'><span class='glyphicon glyphicon-envelope'></span> Email: {3}</p>
                                    {6}
                                </div>
                                <div class='col-xs-6'>
                                    <p class='list-group-item-text'>Open Tickets: {4}</p>
                                    <p class='list-group-item-text'>Closed Tickets: {5}</p>                            
                                </div>
                            </div>
                            </div>"

                    , u.IsActive ? "user-active":"user-inactive", u.IsActive ? "Active":"Inactive", u.FirstLastName, u.Email != "" ? "<a href='mailto:"+u.Email+"'>"+u.Email+"</a>" : "Empty", GetContactTickets(u.UserID, 0), GetContactTickets(u.UserID, 1),  phoneResults, u.UserID);
            }

            return htmlresults.ToString();
        }

        [WebMethod]
        public int SaveCustomer(string data)
        {
            NewCustomerSave info;
            try
            {
                info = Newtonsoft.Json.JsonConvert.DeserializeObject<NewCustomerSave>(data);
            }
            catch (Exception e)
            {
                return -1;
            }

            Organization organization;
            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());

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

            organization.DefaultSupportUserID = info.DefaultSupportUserID < 0 ? null : info.DefaultSupportUserID;
            organization.DefaultPortalGroupID = null;
            organization.DefaultSupportGroupID = info.DefaultSupportGroupID < 0 ? null : info.DefaultSupportGroupID;
            organization.TimeZoneID = info.TimeZoneID;
            organization.Name = info.Name;
            organization.Website = info.Website;
            organization.CompanyDomains = info.CompanyDomains;

            organization.Description = info.Description;
            organization.SAExpirationDate = DataUtils.DateToUtc(TSAuthentication.GetLoginUser(), info.SAExpirationDate);

            organization.SlaLevelID = info.SlaLevelID < 0 ? null : info.SlaLevelID;

            int shm = 0;
            int.TryParse(info.SupportHoursMonth.ToString(), out shm);

            organization.SupportHoursMonth = shm;

            if (TSAuthentication.IsSystemAdmin)
            {
                organization.HasPortalAccess = info.PortalAccess;
                organization.IsActive = info.Active;
                organization.IsApiActive = info.APIEnabled;
                organization.IsApiEnabled = info.APIEnabled;
                organization.InActiveReason = info.InactiveReason;
            }
            organization.Collection.Save();

            foreach (CustomFieldSaveInfo field in info.Fields)
            {
                CustomValue customValue = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, organization.OrganizationID);
                if (field.Value == null)
                {
                    customValue.Value = "";
                }
                else
                {
                    if (customValue.FieldType == CustomFieldType.DateTime)
                    {
                        //customValue.Value = ((DateTime)field.Value).ToString();
                        DateTime dt;
                        if (DateTime.TryParse(((string)field.Value).Replace("UTC", "GMT"), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out dt))
                        {
                            customValue.Value = dt.ToUniversalTime().ToString();
                        }
                    }
                    else
                    {
                        customValue.Value = field.Value.ToString();
                    }

                }

                customValue.Collection.Save();
            }

            return organization.OrganizationID;

        }

        [WebMethod]
        public int SaveContact(string data)
        {
            User user;
            Users users = new Users(TSAuthentication.GetLoginUser());
            NewContactSave info;
            int newOrgID;

            info = Newtonsoft.Json.JsonConvert.DeserializeObject<NewContactSave>(data);

            if(info.Company != null)
                newOrgID = int.Parse(info.Company);
            else
                newOrgID = Organizations.GetUnknownCompanyID(TSAuthentication.GetLoginUser());

            if (!users.IsEmailValid(info.Email, -1, newOrgID))
            {
                return -1;
            }

            user = users.AddNewUser();
            user.OrganizationID = newOrgID;
            if (info.Active) user.ActivatedOn = DateTime.UtcNow;
            user.LastLogin = DateTime.UtcNow;
            user.LastActivity = DateTime.UtcNow.AddHours(-1);
            user.IsPasswordExpired = true;
            user.ReceiveTicketNotifications = true;

            user.Email = info.Email;
            user.FirstName = info.FirstName;
            user.LastName = info.LastName;
            user.Title = info.Title;
            user.MiddleName = info.MiddleName;
            user.BlockInboundEmail = info.BlockInboundEmail;
            user.IsActive = info.Active;

            if (TSAuthentication.GetOrganization(TSAuthentication.GetLoginUser()).HasPortalAccess && TSAuthentication.IsSystemAdmin)
                user.IsPortalUser = info.IsPortalUser;

            if (TSAuthentication.GetOrganization(TSAuthentication.GetLoginUser()).ParentID == null && TSAuthentication.IsSystemAdmin)
            {
                user.IsSystemAdmin = info.IsSystemAdmin;
                user.IsFinanceAdmin = info.IsFinanceAdmin;
            }
            user.Collection.Save();

            if (info.SyncAddress)
            {
                Addresses orgAddresses = new Addresses(TSAuthentication.GetLoginUser());
                orgAddresses.LoadByID(user.OrganizationID, ReferenceType.Organizations);
                Addresses newAddresses = new Addresses(TSAuthentication.GetLoginUser());


                foreach (Address add in orgAddresses)
                {
                    Address address = newAddresses.AddNewAddress();
                    address.RefID = user.UserID;
                    address.RefType = ReferenceType.Users;
                    address.Addr1 = add.Addr1;
                    address.Addr2 = add.Addr2;
                    address.Addr3 = add.Addr3;
                    address.City = add.City;
                    address.Country = add.Country;
                    address.Description = add.Description;
                    address.State = add.State;
                    address.Zip = add.Zip;
                }
                newAddresses.Save();

            }


            if (info.SyncPhone)
            {
                PhoneNumbers oldPhoneNumbers = new PhoneNumbers(TSAuthentication.GetLoginUser());
                PhoneNumbers newPhoneNumbers = new PhoneNumbers(TSAuthentication.GetLoginUser());
                oldPhoneNumbers.LoadByID(user.OrganizationID, ReferenceType.Organizations);

                foreach (PhoneNumber phone in oldPhoneNumbers)
                {
                    PhoneNumber phoneNumber = newPhoneNumbers.AddNewPhoneNumber();
                    phoneNumber.RefID = user.UserID;
                    phoneNumber.RefType = ReferenceType.Users;
                    phoneNumber.Extension = phone.Extension;
                    phoneNumber.Number = phone.Number;
                    phoneNumber.PhoneTypeID = phone.PhoneTypeID;
                }

                newPhoneNumbers.Save();
            }


            foreach (CustomFieldSaveInfo field in info.Fields)
            {
                        CustomValue customValue = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, user.UserID);
                if (field.Value == null)
                {
                    customValue.Value = "";
                }
                else
                {
                    if (customValue.FieldType == CustomFieldType.DateTime)
                    {
                        //customValue.Value = ((DateTime)field.Value).ToString();
                        DateTime dt;
                        if (DateTime.TryParse(((string)field.Value).Replace("UTC", "GMT"), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out dt))
                        {
                            customValue.Value = dt.ToUniversalTime().ToString();
                        }
                    }
                    else
                    {
                        customValue.Value = field.Value.ToString();
                    }

                }

                customValue.Collection.Save();
            }

            string password = DataUtils.GenerateRandomPassword();
            user.CryptedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5");
            user.IsPasswordExpired = true;
            //user.Collection.Save();
            //if (TSAuthentication.GetOrganization(TSAuthentication.GetLoginUser()).ParentID == null && info.Email)
            //{
            //    EmailPosts.SendWelcomeTSUser(UserSession.LoginUser, user.UserID, password);
            //}
            //else if (cbEmail.Checked && info.IsPortalUser)
            //    EmailPosts.SendWelcomePortalUser(UserSession.LoginUser, user.UserID, password);

            return user.UserID;
        }

        [WebMethod]
        public void SavePhoneNumber(string data, int refID, ReferenceType refType)
        {
            PhoneSave info;
            info = Newtonsoft.Json.JsonConvert.DeserializeObject<PhoneSave>(data);

            PhoneNumbers phoneNumbers = new PhoneNumbers(TSAuthentication.GetLoginUser());

            if (info.PhoneID != -1)
            {
                phoneNumbers.LoadByPhoneID((int)info.PhoneID);
                if (!phoneNumbers.IsEmpty)
                {
                    PhoneNumber phoneNumber = phoneNumbers[0];
                    phoneNumber.Extension = info.Extension;
                    phoneNumber.Number = info.Number;
                    phoneNumber.PhoneTypeID = info.PhoneTypeID;
                }
            }
            else
            {
                PhoneNumber phoneNumber = phoneNumbers.AddNewPhoneNumber();
                phoneNumber.RefID = refID;
                phoneNumber.RefType = refType;
                phoneNumber.Extension = info.Extension;
                phoneNumber.Number = info.Number;
                phoneNumber.PhoneTypeID = info.PhoneTypeID;
            }
            phoneNumbers.Save();
        }

        [WebMethod]
        public void SaveAddress(string data, int refID, ReferenceType refType)
        {
            AddressSave info;
            info = Newtonsoft.Json.JsonConvert.DeserializeObject<AddressSave>(data);

            Addresses addresses = new Addresses(TSAuthentication.GetLoginUser());

            if (info.addressID != -1)
            {
                addresses.LoadByAddressID(info.addressID);
                if (!addresses.IsEmpty)
                {
                    Address address = addresses[0];
                    address.Addr1 = info.Addr1;
                    address.Addr2 = info.Addr2;
                    address.Addr3 = info.Addr3;
                    address.City = info.City;
                    address.Country = info.Country;
                    address.Description = info.Description;
                    address.State = info.State;
                    address.Zip = info.Zip;
                }
            }
            else
            {
                Address address = addresses.AddNewAddress();
                address.RefID = refID;
                address.RefType = refType;
                address.Addr1 = info.Addr1;
                address.Addr2 = info.Addr2;
                address.Addr3 = info.Addr3;
                address.City = info.City;
                address.Country = info.Country;
                address.Description = info.Description;
                address.State = info.State;
                address.Zip = info.Zip;
            }
            addresses.Save();
        }

        [WebMethod]
        public void SaveNote(string title, string noteText, int noteID, int refID, ReferenceType refType)
        {
            Note note = null;
            if (noteID > -1)
            {
                note = (Note)Notes.GetNote(TSAuthentication.GetLoginUser(), noteID);
            }
            else
            {
                note = (new Notes(TSAuthentication.GetLoginUser())).AddNewNote();
                note.RefID = refID;
                note.RefType = refType;
            }

            if (note != null)
            {
                note.Description = noteText;
                note.Title = title;
                note.Collection.Save();
            }

        }

        [WebMethod]
        public void SaveProduct(string data)
        {
            NewProductSave info = null;
            OrganizationProduct organizationProduct = null;

            try
            {
                info = Newtonsoft.Json.JsonConvert.DeserializeObject<NewProductSave>(data);
            }
            catch (Exception e)
            {
                string a = e.Message;
            }
            if (info.OrganizationProductID < 0)
            {
                organizationProduct = (new OrganizationProducts(TSAuthentication.GetLoginUser())).AddNewOrganizationProduct();
            }
            else
            {
                organizationProduct = (OrganizationProduct)OrganizationProducts.GetOrganizationProduct(TSAuthentication.GetLoginUser(), info.OrganizationProductID);
            }

            organizationProduct.OrganizationID = int.Parse(info.OrganizationID);
            organizationProduct.ProductID = int.Parse(info.ProductID);
            if (info.Version > 0)
                organizationProduct.ProductVersionID = info.Version;
            else
            {
                organizationProduct.ProductVersionID = null;
            }
                
            if(info.SupportExpiration != "")
                organizationProduct.SupportExpiration = DataUtils.DateToUtc(TSAuthentication.GetLoginUser(), DateTime.ParseExact(info.SupportExpiration, GetDateFormat(),null));

            organizationProduct.Collection.Save();

            foreach (CustomFieldSaveInfo field in info.Fields)
            {
                CustomValue customValue = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, organizationProduct.OrganizationProductID);
                if (field.Value == null)
                {
                    customValue.Value = "";
                }
                else
                {
                    if (customValue.FieldType == CustomFieldType.DateTime)
                    {
                        //customValue.Value = ((DateTime)field.Value).ToString();
                        DateTime dt;
                        if (DateTime.TryParse(((string)field.Value).Replace("UTC", "GMT"), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out dt))
                        {
                            customValue.Value = dt.ToUniversalTime().ToString();
                        }
                    }
                    else
                    {
                        customValue.Value = field.Value.ToString();
                    }

                }
                customValue.Collection.Save();
            }
                

        }

        [WebMethod]
        public int LoadCDI(int organizationID)
        {
            Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), organizationID);

            return organization.CustDisIndex;
        }

        [WebMethod]
        public string LoadChartData(int organizationID, bool open)
        {

            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID);

            TicketTypes ticketTypes = new TicketTypes(TSAuthentication.GetLoginUser());
            ticketTypes.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID, organizations[0].ProductType);

            int total = 0;
            StringBuilder chartString = new StringBuilder("");

            foreach (TicketType ticketType in ticketTypes)
            {
                int count;
                if(open)
                     count = Tickets.GetOrganizationOpenTicketCount(TSAuthentication.GetLoginUser(), organizationID, ticketType.TicketTypeID);
                else
                    count = Tickets.GetOrganizationClosedTicketCount(TSAuthentication.GetLoginUser(), organizationID, ticketType.TicketTypeID);
                total += count;

                if (count > 0)
                    chartString.AppendFormat("{0},{1},", ticketType.Name, count.ToString());
                    //chartString.AppendFormat("['{0}',{1}],",ticketType.Name, count.ToString());
            }
            if (chartString.ToString().EndsWith(","))
            {
                chartString.Remove(chartString.Length - 1, 1);
            }

            return chartString.ToString();
        }

        [WebMethod]
        public string LoadContactChartData(int userID, bool open)
        {

            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID);

            TicketTypes ticketTypes = new TicketTypes(TSAuthentication.GetLoginUser());
            ticketTypes.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID, organizations[0].ProductType);

            int total = 0;
            StringBuilder chartString = new StringBuilder("");

            foreach (TicketType ticketType in ticketTypes)
            {
                int count;
                if (open)
                    count = Tickets.GetContactOpenTicketCount(TSAuthentication.GetLoginUser(), userID, ticketType.TicketTypeID);
                else
                    count = Tickets.GetContactClosedTicketCount(TSAuthentication.GetLoginUser(), userID, ticketType.TicketTypeID);
                total += count;

                if (count > 0)
                    chartString.AppendFormat("{0},{1},", ticketType.Name, count.ToString());
                //chartString.AppendFormat("['{0}',{1}],",ticketType.Name, count.ToString());
            }
            if (chartString.ToString().EndsWith(","))
            {
                chartString.Remove(chartString.Length - 1, 1);
            }

            return chartString.ToString();
        }

        [WebMethod]
        public string GetDateFormat(bool lower=false)
        {
            CultureInfo us = new CultureInfo(TSAuthentication.GetLoginUser().CultureInfo.ToString());
            if (lower)
                return us.DateTimeFormat.ShortDatePattern.ToLower();
            else
                return us.DateTimeFormat.ShortDatePattern;
        }

        [WebMethod]
        public string SendWelcome(int userID)
        {
            Users users = new Users(TSAuthentication.GetLoginUser());
            users.LoadByUserID(userID);
            EmailPosts.SendWelcomeNewSignup(TSAuthentication.GetLoginUser(), userID, "");
            return ("A welcome message has been sent to " + users[0].FirstName + " " + users[0].LastName);
        }

        [WebMethod]
        public string PasswordReset(int userID)
        {
            Users users = new Users(TSAuthentication.GetLoginUser());
            users.LoadByUserID(userID);
            if (!users.IsEmpty)
            {
                if (users[0].CryptedPassword == "")
                {
                    string password = DataUtils.GenerateRandomPassword();
                    users[0].CryptedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5");
                    users[0].IsPasswordExpired = true;
                    users[0].Collection.Save();
                    if (TSAuthentication.GetOrganization(TSAuthentication.GetLoginUser()).ParentID == null)
                        EmailPosts.SendWelcomeTSUser(TSAuthentication.GetLoginUser(), users[0].UserID, password);
                    else
                        EmailPosts.SendWelcomePortalUser(TSAuthentication.GetLoginUser(), users[0].UserID, password);

                    return ("A new password has been sent to " + users[0].FirstName + " " + users[0].LastName);

                }
                else
                {
                    if (DataUtils.ResetPassword(TSAuthentication.GetLoginUser(), users[0], !(TSAuthentication.GetOrganization(TSAuthentication.GetLoginUser()).ParentID == null)))
                    {
                        return("A new password has been sent to " + users[0].FirstName + " " + users[0].LastName);
                    }
                }
            }
            return("There was an error sending the password.");
        }

        public string CreateCompanyBox(int orgID) //Organization org
        {

            Organization org = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);

            PhoneNumbers phone = new PhoneNumbers(TSAuthentication.GetLoginUser());
            phone.LoadByID(org.OrganizationID, ReferenceType.Organizations);

            string boxhtml = @"<li>
                                    <div class=""peopleinfo"">
                                        <div class=""pull-right""><span>Open Tickets:</span> {5}</div>
                                        <h4><span class=""fa fa-building-o""></span><a class=""companylink"" id=""o{0}"" href="""">{1}</a></h4>
                                        <ul>
                                            <li><span>Phone:</span> {2}</li>
                                            <li><span>Portal Access</span> {3}</li>
                                            <li><span>Website:</span> {4}</li>
                                        </ul>
                                    </div>
                            </li>";

            return string.Format(boxhtml, org.OrganizationID, org.Name, phone.IsEmpty ? "Empty" : phone[0].Number, org.HasPortalAccess, org.Website, GetCustomerOpenTickets(org));
        }

        public string CreateContactBox(int userID)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            Organization org = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), user.OrganizationID);
            PhoneNumbers phone = new PhoneNumbers(TSAuthentication.GetLoginUser());
            phone.LoadByID(user.UserID, ReferenceType.Users);

            string boxhtml = @"<li>
                                    <div class=""peopleinfo"">
                                        <div class=""pull-right""><span>Open Tickets:</span> {7}</div>
                                        <h4><span class=""fa fa-user""></span><a class=""contactlink"" id=""u{0}"" href="""">{1} {2} {3}</a></h4>
                                        
                                        <ul>
                                            {8}
                                            <li><span>Email:</span> {4}</li>
                                            <li><span>Phone:</span> {5}</li>
                                            <li><span>Portal Access:</span> {6}</li>
                                        </ul>
                                    </div>

                            </li>";

            return string.Format(boxhtml, user.UserID, user.FirstName, user.LastName, string.IsNullOrEmpty(user.Title) ? "" : "[" + user.Title + "]", "<a href='mailto:" + user.Email + "'>" + user.Email + "</a>", phone.IsEmpty ? "Empty" : phone[0].Number, user.IsPortalUser, GetContactTickets(user.UserID, 0), org.Name != "_Unknown Company" ? "<li><span>Company: </span><a href='#' class='viewOrg' id='" + user.OrganizationID + "'>" + org.Name + "</a></li><li>&nbsp;</li>" : "");
        }

        public string CreateNoResults()
        {
            string boxhtml = @"<div class=""col-xs-12"">
                            <div class=""peoplewrapper"">
                                <h2 class=""text-center"">No Search Results Found!</h2>
                            </div>
                            </div>";
            return boxhtml;
        }

        public string CreateRecentlyViewed(RecentlyViewedItem recent)
        {
            string recentHTML;

            //user
            if(recent.RefType == 0){
                Users u = new Users(TSAuthentication.GetLoginUser());
                u.LoadByUserID(recent.RefID);
                PhoneNumbers phone = new PhoneNumbers(TSAuthentication.GetLoginUser());
                phone.LoadByID(u[0].UserID, ReferenceType.Users);
                recentHTML = @" 
                <li>
                        <div class=""recent-info"">
                            <h4> <span class=""fa fa-user""></span><a class=""contactlink"" id=""u{3}"" href="""">{0}</a></h4>
                            <ul>
                                <li><span>Email:</span> {1}</li>
                                <li><span>Phone:</span> {2}</li>
                            </ul>
                        </div>
                </li>";
                return string.Format(recentHTML, u[0].FirstLastName, u[0].Email, phone.IsEmpty ? "Empty" : phone[0].Number, u[0].UserID);
            }
            else{
                Organizations org = new Organizations(TSAuthentication.GetLoginUser());
                org.LoadByOrganizationID(recent.RefID);
                PhoneNumbers phone = new PhoneNumbers(TSAuthentication.GetLoginUser());
                phone.LoadByID(org[0].OrganizationID, ReferenceType.Users);

                recentHTML = @" 
                <li>
                        <div class=""recent-info"">
                            <h4><span class=""fa fa-building-o""></span><a class=""companylink"" id=""o{2}"" href="""">{0}</a></h4>
                            <ul>
                                <li><span>Phone:</span> {1}</li>
                            </ul>
                        </div>
                </li>";
                return string.Format(recentHTML, org[0].Name, phone.IsEmpty ? "Empty" : phone[0].Number, org[0].OrganizationID);
            }
        }

        public string GetCustomerOpenTickets(Organization org)
        {
            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID);

            TicketTypes ticketTypes = new TicketTypes(TSAuthentication.GetLoginUser());
            ticketTypes.LoadByOrganizationID(organizations[0].OrganizationID, organizations[0].ProductType);

            int total = 0;

            foreach (TicketType ticketType in ticketTypes)
            {
                int count = Tickets.GetOrganizationOpenTicketCount(TSAuthentication.GetLoginUser(), org.OrganizationID, ticketType.TicketTypeID);
                total += count;
            }

            return total.ToString();
        }

        [WebMethod]
        public string GetContactTickets(int userID, int closed)
        {
            TicketsView tickets = new TicketsView(TSAuthentication.GetLoginUser());

            return tickets.GetUserTicketCount(userID, closed).ToString();
        }

        public string CreateTextControl(CustomField field, bool isEditable = false, int organizationID = -1)
        {
            StringBuilder html = new StringBuilder();

            if (isEditable)
            {
                CustomValue value = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, organizationID);
                html.AppendFormat(@"<div class='form-group'> 
                                        <label for='{0}' class='col-xs-4 control-label'>{1}</label> 
                                        <div class='col-xs-8'> 
                                            <p class='form-control-static'><a class='editable' id='{0}' data-type='text'>{2}</a></p> 
                                        </div> 
                                    </div>", field.CustomFieldID, field.Name, value.Value  );
            }
            else
            {
                html.AppendFormat("<div class='col-xs-8'><input class='form-control col-xs-10 customField {1}' id='{0}' name='{0}'></div>", field.CustomFieldID, field.IsRequired ? "required":"");
            }
            return html.ToString();
        }

        public string CreateNumberControl(CustomField field, bool isEditable = false, int organizationID = -1)
        {
            StringBuilder html = new StringBuilder();
            if (isEditable)
            {
                CustomValue value = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, organizationID);
                html.AppendFormat(@"<div class='form-group'> 
                                        <label for='{0}' class='col-xs-4 control-label'>{1}</label> 
                                        <div class='col-xs-8'> 
                                            <p class='form-control-static'><a class='editable' id='{0}' data-type='text'>{2}</a></p> 
                                        </div> 
                                    </div>", field.CustomFieldID, field.Name, value.Value);
            }
            else
                html.AppendFormat("<div class='col-xs-8'><input class='form-control col-xs-10 customField number {1}' id='{0}'  name='{0}'></div>", field.CustomFieldID, field.IsRequired ? "required" : "");

            return html.ToString();
        }

        public string CreateDateTimeControl(CustomField field, bool isEditable = false, int organizationID = -1)
        {
            StringBuilder html = new StringBuilder();
            if (isEditable)
            {
                CustomValue value = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, organizationID);
                html.AppendFormat(@"<div class='form-group'> 
                                        <label for='{0}' class='col-xs-4 control-label'>{1}</label> 
                                        <div class='col-xs-8'> 
                                            <p class='form-control-static'><a class='editable' id='{0}' data-type='text'>{2}</a></p> 
                                        </div> 
                                    </div>", field.CustomFieldID, field.Name, value.Value);
            }
            else
                html.AppendFormat("<div class='col-xs-8'><input class='form-control datepicker col-xs-10 customField {1}' id='{0}'  name='{0}'></div>", field.CustomFieldID, field.IsRequired ? "required" : "");
            
            return html.ToString();
        }

        public string CreateBooleanControl(CustomField field, bool isEditable = false, int organizationID = -1)
        {
            StringBuilder html = new StringBuilder();
            if (isEditable)
            {
                CustomValue value = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, organizationID);
                html.AppendFormat(@"<div class='form-group'> 
                                        <label for='{0}' class='col-xs-4 control-label'>{1}</label> 
                                        <div class='col-xs-8'> 
                                            <p class='form-control-static'><a class='editable' id='{0}' data-type='text'>{2}</a></p> 
                                        </div> 
                                    </div>", field.CustomFieldID, field.Name, value.Value);
            }
            else
            {
                html.AppendFormat("<div class='col-xs-1'><label><input class='customField' id='{0}' type='checkbox'></label></div>", field.CustomFieldID);
            }
            return html.ToString();
        }

        public string CreatePickListControl(CustomField field, bool isEditable = false, int organizationID = -1)
        {
            StringBuilder html = new StringBuilder();
            string[] items = field.ListValues.Split('|');
            if (isEditable)
            {
                CustomValue value = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, organizationID);
                html.AppendFormat(@"<div class='form-group'> 
                                        <label for='{0}' class='col-xs-4 control-label'>{1}</label> 
                                        <div class='col-xs-8'> 
                                            <p class='form-control-static'><a class='editable' id='{0}' data-type='select'>{2}</a></p> 
                                        </div> 
                                    </div>", field.CustomFieldID, field.Name, value.Value);
            }
            else
            {
                html.AppendFormat("<div class='col-xs-8'><select class='form-control customField' id='{0}'  name='{0}' type='picklist'>", field.CustomFieldID);
                foreach (string item in items)
                {
                    html.AppendFormat("<option value='{0}'>{1}</option>", item, item);
                }
                html.Append("</select></div>");
            }
            return html.ToString();
        }

        public string AddControl(CustomField field)
        {
            StringBuilder htmltest = new StringBuilder();
            switch (field.FieldType)
            {
                case CustomFieldType.Text: htmltest.AppendLine(CreateTextControl(field)); break;
                case CustomFieldType.Number: htmltest.AppendLine(CreateTextControl(field)); break;
                case CustomFieldType.DateTime: htmltest.AppendLine(CreateDateTimeControl(field)); break;
                case CustomFieldType.Boolean: htmltest.AppendLine(CreateBooleanControl(field)); break;
                case CustomFieldType.PickList: htmltest.AppendLine(CreatePickListControl(field)); break;
                default: break;
            }
            return htmltest.ToString();
        }

        public class NewCustomerSave
        {
            public NewCustomerSave() { }
            [DataMember] public string Name{ get; set; }
            [DataMember] public string Website{ get; set; }
            [DataMember] public string CompanyDomains{ get; set; }
            [DataMember] public int? DefaultSupportUserID{ get; set; }
            [DataMember] public int? DefaultSupportGroupID{ get; set; }
            [DataMember] public string TimeZoneID{ get; set; }
            [DataMember] public DateTime? SAExpirationDate{ get; set; }
            [DataMember] public int? SlaLevelID{ get; set; }
            [DataMember] public int SupportHoursMonth{ get; set; }
            [DataMember] public bool Active{ get; set; }
            [DataMember] public bool PortalAccess{ get; set; }
            [DataMember] public bool APIEnabled{ get; set; }
            [DataMember] public string InactiveReason{ get; set; }
            [DataMember] public string Description{ get; set; }
            [DataMember] public List<CustomFieldSaveInfo> Fields { get; set; }
        }

        public class NewProductSave
        {
            public NewProductSave() { }
            [DataMember] public string OrganizationID { get; set; }
            [DataMember] public string ProductID { get; set; }
            [DataMember] public int Version { get; set; }
            [DataMember] public string SupportExpiration { get; set; }
            [DataMember] public int OrganizationProductID { get; set; }
            [DataMember] public List<CustomFieldSaveInfo> Fields { get; set; }
        }

        public class NewContactSave
        {
            public NewContactSave() { }
            [DataMember] public string Company { get; set; }
            [DataMember] public bool Active { get; set; }
            [DataMember] public string Email { get; set; }
            [DataMember] public string FirstName { get; set; }
            [DataMember] public string LastName { get; set; }
            [DataMember] public string Title { get; set; }
            [DataMember] public string MiddleName { get; set; }
            [DataMember] public bool BlockInboundEmail { get; set; }
            [DataMember] public bool IsPortalUser { get; set; }
            [DataMember] public bool IsSystemAdmin { get; set; }
            [DataMember] public bool IsFinanceAdmin { get; set; }
            [DataMember] public bool SyncAddress { get; set; }
            [DataMember] public bool SyncPhone { get; set; }
            [DataMember] public List<CustomFieldSaveInfo> Fields { get; set; }
        }

        public class PhoneSave
        {
            public PhoneSave() { }
            [DataMember]
            public string Extension { get; set; }
            [DataMember]
            public string Number { get; set; }
            [DataMember]
            public int PhoneTypeID { get; set; }
            [DataMember]
            public int? PhoneID { get; set; }
        }

        public class AddressSave
        {
            public AddressSave() { }
            [DataMember] public string Addr1 { get; set; }
            [DataMember]
            public string Addr2 { get; set; }
            [DataMember]
            public string Addr3 { get; set; }
            [DataMember]
            public string City { get; set; }
            [DataMember]
            public string Country { get; set; }
            [DataMember]
            public string Description { get; set; }
            [DataMember]
            public string State { get; set; }
            [DataMember]
            public string Zip { get; set; }
            [DataMember]
            public int addressID { get; set; }
        }

        public class CustComp
        {
            public string Name { get; set; }
            public object userinfo { get; set; }

        }

        public class CustomFieldSaveInfo
        {
            public CustomFieldSaveInfo() { }
            [DataMember]
            public int CustomFieldID { get; set; }
            [DataMember]
            public object Value { get; set; }
        }

        public class OrganizationCustomProduct
        {
            [DataMember] public string ProductName { get; set; }
            [DataMember]
            public string VersionNumber { get; set; }
            [DataMember]
            public string SupportExpiration { get; set; }
            [DataMember]
            public string VersionStatus { get; set; }
            [DataMember]
            public string IsReleased { get; set; }
            [DataMember]
            public string ReleaseDate { get; set; }
            [DataMember]
            public int ProductID { get; set; }
            [DataMember]
            public int OrganizationProductID { get; set; }
            [DataMember]
            public List<string> CustomFields { get; set; }

        }

        public class OrgProp
        {
            [DataMember]
            public OrganizationProxy orgproxy { get; set; }
            [DataMember]
            public string SLA { get; set; }
            [DataMember]
            public string PrimaryUser { get; set; }
            [DataMember]
            public string SupportGroup { get; set; }
            [DataMember]
            public string DefaultSupportUser { get; set; }
            [DataMember]
            public string SAED { get; set; }
        }

    }

}