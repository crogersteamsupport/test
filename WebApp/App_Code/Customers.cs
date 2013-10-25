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
        public void GetSearchResults(string filter)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();

            if (filter != ""){
                filter = filter.Trim();
                if (filter.Length > 0 && filter.Length < 2) return;
            }
            StringBuilder builder = new StringBuilder();
            var theNames = new List<CustComp>();

            UsersView users = new UsersView(loginUser);
            users.CustomerLoadByLikeName(loginUser.OrganizationID, filter, 100, true);

            Organizations organizations = new Organizations(loginUser);
            organizations.LoadByLikeOrganizationName(loginUser.OrganizationID, filter, false, 100, true);

            int count = 0;

            foreach (Organization item in organizations)
            {
                if (++count > 20) break;
                //builder.Append(GetItemHtml("o" + item.OrganizationID, HttpUtility.HtmlEncode(item.Name), string.Format(data, item.OrganizationID.ToString(), "-1")));
                theNames.Add(new CustComp(){Name=item.Name, userinfo=item });
            }

            count = 0;
            foreach (UsersViewItem item in users)
            {
                if (++count > 20) break;
                //builder.Append(GetItemHtml("u" + item.UserID, item.LastName + ", " + item.FirstName + " [" + HttpUtility.HtmlEncode(item.Organization) + "]", string.Format(data, item.OrganizationID.ToString(), item.UserID.ToString())));
                theNames.Add(new CustComp(){Name=item.FirstName, userinfo=item });
            }

            theNames.Sort(delegate(CustComp c1, CustComp c2) { return c1.Name.CompareTo(c2.Name); });

            foreach (CustComp searchResults in theNames)
            {
                if (searchResults.userinfo is Organization)
                {

                    builder.AppendLine(CreateCompanyBox((Organization)searchResults.userinfo));
                }
                else
                {
                    builder.AppendLine(CreateContactBox((UsersViewItem)searchResults.userinfo));
                }
            }

            return;

        }

        [WebMethod]
        public string GetCompanies(string filter, int startIndex = 1)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            StringBuilder builder = new StringBuilder();
            Organizations organizations = new Organizations(loginUser);
            int i = 0;

            organizations.CustomerLoadByLikeOrganizationName(loginUser.OrganizationID, filter, false, startIndex, true);

            if (organizations.Count > 0)
            {

                foreach (Organization item in organizations)
                {
                        builder.Append(CreateCompanyBox(item));
                }

                return builder.ToString();
            }
            else
            {
                return CreateNoResults();
            }
            
        }

        [WebMethod]
        public string GetContacts(string filter, int startIndex = 1)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            StringBuilder builder = new StringBuilder();
            UsersView users = new UsersView(loginUser);

            users.CustomerLoadByLikeName(loginUser.OrganizationID, filter, startIndex, true);

            if (users.Count > 0)
            {            
            
                foreach (UsersViewItem item in users)
                {
                        builder.Append(CreateContactBox(item));
                }

                return builder.ToString();
            }
            else
            {
                return CreateNoResults();
            }

        }

        [WebMethod]
        public string GetMoreResults(string filterType, string filter, int startIndex)
        {
            string results = "";

            switch (filterType)
            {
                case "all":
                    //results = GetSearchResults(filter);
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

        public string CreateCompanyBox(Organization org)
        {
            string boxhtml = @"<li>
                                    <div class=""peopleinfo"">
                                        <h3><span class=""icon-building""></span><a class=""companylink"" id=""o{0}"" href="""">{1}</a>{2}</h3>
                                        <ul>
                                            <li><span>Organization ID:</span> {3}</li>
                                            <li><span>Portal Access</span> {4}</li>
                                            <li><span>Website:</span> {5}</li>
                                        </ul>
                                    </div>
                            </li>";

            return string.Format(boxhtml, org.OrganizationID, org.Name, org.IsActive ? @"<span class=""on"">Active</span>" : @"<span class=""off"">In-Active</span>", org.OrganizationID, org.HasPortalAccess, org.Website);
        }

        public string CreateContactBox(UsersViewItem user)
        {
            string boxhtml = @"<li>
                                    <div class=""peopleinfo"">
                                        <h3><span class=""icon-user""></span><a class=""contactlink"" id=""u{0}"" href="""">{1} {2}</a>{3}</h3>
                                        <ul>
                                            <li><span>Email:</span> {4}</li>
                                            <li><span>Portal Access</span> {5}</li>
                                            <li><span>Title:</span> {6}</li>
                                        </ul>
                                    </div>

                            </li>";

            return string.Format(boxhtml, user.UserID, user.FirstName, user.LastName, user.IsActive ? @"<span class=""on"">Active</span>" : @"<span class=""off"">In-Active</span>", user.Email, user.IsPortalUser, user.Title );
        }

        public string CreateNoResults()
        {
            string boxhtml = @"<div class=""col-md-12"">
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
                recentHTML = @" 
                <li>
                        <div class=""recent-info"">
                            <h3> <span class=""icon-user""></span><a href="""">{0}</a></h3>
                            <ul>
                                <li><span>Email:</span> xxx@xxx.com</li>
                                <li><span>Portal Access</span> True</li>
                            </ul>
                        </div>
                </li>";
                return string.Format(recentHTML, u[0].FirstLastName);
            }
            else{
                Organizations org = new Organizations(TSAuthentication.GetLoginUser());
                org.LoadByOrganizationID(recent.RefID);
                recentHTML = @" 
                <li>
                        <div class=""recent-info"">
                            <h3><span class=""icon-building""></span><a href="""">{0}</a></h3>
                            <ul>
                                <li><span>Email:</span> xxx@xxx.com</li>
                                <li><span>Portal Access</span> True</li>
                            </ul>
                        </div>
                </li>";
                return string.Format(recentHTML, org[0].Name);
            }


            
        }

        public class CustComp
        {
            public string Name { get; set; }
            public object userinfo { get; set; }

        }

    }

}