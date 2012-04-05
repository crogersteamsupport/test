using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Security.Cryptography;
using Telerik.Web.UI;
using TeamSupport.Data;
using TeamSupport.WebUtils;

public partial class SignUp : System.Web.UI.Page
{
  RadAjaxManager _manager;

  protected override void OnInit(EventArgs e)
  {
    base.OnInit(e);


  }

  protected void Page_Load(object sender, EventArgs e)
  {
    _manager = RadAjaxManager.GetCurrent(Page);
    _manager.AjaxSettings.Clear();
    _manager.AjaxSettings.AddAjaxSetting(cbAgree, btnSubmit);

    if (!IsPostBack)
    {
      cmbVersion.Items.Add(new RadComboBoxItem(DataUtils.ProductTypeString(ProductType.Enterprise), ((int)ProductType.Enterprise).ToString()));
      cmbVersion.Items.Add(new RadComboBoxItem(DataUtils.ProductTypeString(ProductType.HelpDesk), ((int)ProductType.HelpDesk).ToString()));
      //cmbVersion.Items.Add(new RadComboBoxItem(DataUtils.ProductTypeString(ProductType.BugTracking), ((int)ProductType.BugTracking).ToString()));
      cmbVersion.Items.Add(new RadComboBoxItem(DataUtils.ProductTypeString(ProductType.Express), ((int)ProductType.Express).ToString()));
    }

    if (!IsPostBack)
    {
      fieldPostToken.Value = UserSession.PostAuthenticationToken;
    }
    else
    {
      if (fieldPostToken == null || fieldPostToken.Value != UserSession.PostAuthenticationToken)
      {
        Response.Write("You are unauthorized.");
        Response.End();
        return;
      }
    }
    btnSubmit.Enabled = cbAgree.Checked;
  }

  private bool IsCompanyValid(LoginUser loginUser, string company)
  {
    Organizations organizations = new Organizations(loginUser);
    organizations.LoadByOrganizationName(company.Trim());
    if (!organizations.IsEmpty)
    {
      _manager.Alert(company + " is already registered with TeamSupport.com");
      return false;
    }
    return true;
  }

  protected void btnSubmit_Click(object sender, EventArgs e)
  {

    LoginUser loginUser = new LoginUser(UserSession.ConnectionString, -1, -1, null);

    if (!IsCompanyValid(loginUser, Ecom_BillTo_Postal_Company.Text))
    {
      return;
    }


    Organizations organizations = new Organizations(loginUser);
    organizations.LoadTeamSupport();
    if (organizations.IsEmpty) return;
    Organization teamSupport = organizations[0];


    organizations = new Organizations(loginUser);
    Organization organization = organizations.AddNewOrganization();
    organization.Name = Ecom_BillTo_Postal_Company.Text.Trim();
    organization.ParentID = teamSupport.OrganizationID;
    organization.ProductType = (ProductType)int.Parse(cmbVersion.SelectedValue);
    organization.PortalSeats = organization.ProductType == ProductType.Enterprise || organization.ProductType == ProductType.HelpDesk ? 999999 : 0;
    organization.IsApiActive = organization.ProductType != ProductType.Express;
    organization.IsApiEnabled = true;
    organization.IsAdvancedPortal = organization.PortalSeats > 0;
    organization.IsInventoryEnabled = organization.ProductType == ProductType.Enterprise;
    organization.HasPortalAccess = true;
    organization.IsBasicPortal = true;
    organization.ShowWiki = true;
    organization.WhereHeard = textHeard.Text;
    organization.TimeZoneID = "Central Standard Time";
    organization.IsActive = true;
    organization.IsCustomerFree = false;
    organization.BusinessDays = 62;
    organization.BusinessDayStart = new DateTime(2010, 1, 1, 9, 0, 0, DateTimeKind.Utc);
    organization.BusinessDayEnd = new DateTime(2010, 1, 1, 17, 0, 0, DateTimeKind.Utc);
    organization.ExtraStorageUnits = 0;
    organization.UserSeats = 100;
    organization.ChatSeats = 999999;
    organization.APIRequestLimit = 5000;
    organization.CultureName = "en-US";
    organization.PromoCode = textPromoCode.Text;
    organization.PrimaryInterest = cmbInterest.Text;
    organization.PotentialSeats = cmbSeats.Text;
    organization.EvalProcess = cmbEval.Text;
    organization.RequireKnownUserForNewEmail = false;
    organization.RequireNewKeyword = false;
    organization.ChangeStatusIfClosed = true;
    organization.AddAdditionalContacts = true;
    organization.MatchEmailSubject = false;
    organization.IsPublicArticles = true;
    organizations.Save();


    Users users = new Users(loginUser);
    User user = users.AddNewUser();
    user.ActivatedOn = DateTime.UtcNow;
    string password = textPassword.Text;// DataUtils.GenerateRandomPassword();
    user.CryptedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5");
    user.Email = Ecom_BillTo_Online_Email.Text.Trim();
    user.FirstName = Ecom_BillTo_Postal_Name_First.Text.Trim();
    user.InOffice = true;
    user.InOfficeComment = "";
    user.IsActive = true;
    user.IsSystemAdmin = true;
    user.IsFinanceAdmin = true;
    user.IsChatUser = true;
    user.IsPasswordExpired = false;
    user.LastLogin = DateTime.UtcNow;
    user.LastActivity = DateTime.UtcNow;
    user.LastName = Ecom_BillTo_Postal_Name_Last.Text.Trim();
    user.MiddleName = "";
    user.OrganizationID = organization.OrganizationID;
    user.ReceiveTicketNotifications = true;
    user.ShowWelcomePage = true;
    user.Collection.Save();

    loginUser = new LoginUser(UserSession.ConnectionString, user.UserID, user.OrganizationID, null);

    OrganizationSettings.WriteString(loginUser, "DisableStatusNotification", true.ToString());


    PhoneNumber phoneNumber = (new PhoneNumbers(loginUser)).AddNewPhoneNumber();
    phoneNumber.Number = Ecom_BillTo_Telecom_Phone_Number.Text;
    phoneNumber.Extension = "";
    phoneNumber.RefID = organization.OrganizationID;
    phoneNumber.RefType = ReferenceType.Organizations;
    phoneNumber.Collection.Save();

    WikiArticle wiki = (new WikiArticles(loginUser)).AddNewWikiArticle();
    wiki.ArticleName = "What is TeamSupport?";
    wiki.Body = @"<center><img alt="""" src=""http://www.teamsupport.com/images/team_support_logo.jpg"" /></center>
<h2 style=""font-weight: bold; font-size: 19px; margin: 20px 30px 0px; color: #494a4c; line-height: 21px; font-family: 'lucida grande',arial,'lucida sans unicode',helvetica,'microsoft sans serif',arial; text-align: left;""></h2>
<h2 style=""font-weight: bold; font-size: 19px; margin: 20px 30px 0px; color: #494a4c; line-height: 21px; font-family: 'lucida grande',arial,'lucida sans unicode',helvetica,'microsoft sans serif',arial; text-align: left;"">Support Software That Lets your Team Work Together!</h2>
<p style=""font-weight: normal; font-size: 12px; margin: 5px 30px 25px; color: #494a4c; line-height: 16px; font-family: 'lucida grande',arial,'lucida sans unicode',helvetica,'microsoft sans serif'; float: right; text-align: left;""><a target=""_blank"" style=""font-weight: bold; text-decoration: none; color: #00953e;"" href=""http://www.teamsupport.com/blog/""><img width=""96"" height=""96"" style=""border-width: 0px; border-style: solid;"" id=""thebuzz"" name=""thebuzz"" alt=""read our blog"" src=""http://www.teamsupport.com/images/icon_thebuzz.gif"" /></a></p>
<p style=""font-weight: normal; font-size: 12px; margin: 5px 30px 25px; color: #494a4c; line-height: 16px; font-family: 'lucida grande',arial,'lucida sans unicode',helvetica,'microsoft sans serif'; text-align: left;""><strong>TeamSupport is a web based, enterprise class customer support management system designed for B2B technology
companies as well as institutions providing internal support.</strong>&nbsp;&nbsp;In short, we help companies excel at managing their
customer support communications through the channel most comfortable to them - Phone, web, e-mail or chat.</p>
<p style=""font-weight: normal; font-size: 12px; margin: 5px 30px 25px; color: #494a4c; line-height: 16px; font-family: 'lucida grande',arial,'lucida sans unicode',helvetica,'microsoft sans serif'; text-align: left;"">TeamSupport also helps companies break down departmental silos. &nbsp;<strong>Customer Support, Product Development,
Product Management and various IT teams can easily share data across the enterprise.</strong>&nbsp;&nbsp;This keeps everyone in
the loop - including the customer base - and dramatically increases customer satisfaction.</p>
<span style=""line-height: 14px; font-size: 12px; font-family: 'lucida grande',arial,'lucida sans unicode',helvetica,'microsoft sans serif'; color: #494a4c;"">
<div style=""text-align: center;""><span xmlns=""http://www.w3.org/1999/xhtml""><br />
</span></div>
</span><span style=""line-height: 14px; font-size: 12px; font-family: 'lucida grande',arial,'lucida sans unicode',helvetica,'microsoft sans serif'; color: #494a4c;"">
<div style=""text-align: center;""><span style=""line-height: normal; font-size: medium; font-family: times; color: #000000;"">
<table width=""240"" cellspacing=""0"" cellpadding=""10"" border=""0"">
    <tbody>
        <tr>
            <td style=""text-align: left;""><a style=""font-weight: bold; text-decoration: none; color: #00953e;"" target=""_blank"" href=""http://twitter.com/TeamSupport""><img width=""48"" height=""48"" style=""border-width: 0px; border-style: solid;"" alt=""twitter teamsupport"" src=""http://www.teamsupport.com/images/icon_twitter.gif"" /></a></td>
            <td style=""text-align: left;""><a style=""font-weight: bold; text-decoration: none; color: #00953e;"" target=""_blank"" href=""http://www.facebook.com/pages/Dallas-TX/TeamSupport/219801852819?ref=ts""><img width=""48"" height=""48"" style=""border-width: 0px; border-style: solid;"" alt=""friend teamsupport"" src=""http://www.teamsupport.com/images/icon_facebook.gif"" /></a></td>
            <td style=""text-align: left;""><a style=""font-weight: bold; text-decoration: none; color: #00953e;"" href=""mailto:sales@teamsupport.com""><img width=""48"" height=""48"" style=""border-width: 0px; border-style: solid;"" alt=""email teamsupport"" src=""http://www.teamsupport.com/images/icon_TSemail.gif"" /></a></td>
        </tr>
    </tbody>
</table>
</span></div>
</span>";
    wiki.OrganizationID = organization.OrganizationID;
    wiki.Version = 1;
    wiki.PortalEdit = false;
    wiki.PublicEdit = false;
    wiki.PublicView = false;
    wiki.PortalView = false;
    wiki.Private = false;
    wiki.ModifiedBy = user.UserID;
    wiki.CreatedBy = user.UserID;
    wiki.CreatedDate = DateTime.UtcNow;
    wiki.ModifiedDate = DateTime.UtcNow;
    wiki.Collection.Save();

    organization.PrimaryUserID = user.UserID;
    organization.DefaultWikiArticleID = wiki.ArticleID;
    organization.Collection.Save();


    PortalOptions portalOptions = new PortalOptions(loginUser);
    PortalOption portalOption = portalOptions.AddNewPortalOption();
    portalOption.OrganizationID = organization.OrganizationID;
    portalOption.PortalName = PortalOptions.ValidatePortalNameChars(organization.Name);
    portalOption.UseCompanyInBasic = false;
    portalOption.CompanyRequiredInBasic = false;
    portalOption.HideGroupAssignedTo = true;
    portalOption.HideUserAssignedTo = true;
    portalOption.UseRecaptcha = false;
    portalOption.KBAccess = true;
    portalOption.DisplayGroups = true;
    portalOption.DisplayProducts = true;
    portalOption.DisplayProductVersion = true;
    portalOption.DisplayAdvKB = true;
    portalOption.DisplayLandingPage = true;
    portalOption.EnableScreenr = true;
    portalOption.DeflectionEnabled = true;
    portalOption.LandingPageHtml = @"
<div style=""margin: 0 auto;font: 12px arial"">
	<h1 style=""font-size:20px;font-weight:bold;color:#666;margin:0px"">
		Support Center</h1>Welcome our Customer Portal!
		<br />
	<div class=""widgetcontainer"">
		<div class=""LandingPageContent"">
			<h3 style=""color:#3079c6"">
			Recent KnowledgeBase Articles
			</h3>
			<div class=""LandingPageInnerContent"">
				<ts_latestknowledgebase,5,../Images/bulb.png>
			</div>
			<h3 style=""color:#3079c6"">
			Popular KnowledgeBase Articles
			</h3>
			<div class=""LandingPageInnerContent"">
				<ts_knowledgebase,5,../Images/bulb.png>
			</div>
			<h3 style=""color:#3079c6"">
			Recent Tickets
			</h3>
			<div class=""LandingPageInnerContent"">
				<ts_alltickets,5,../Images/file.png>
			</div>
    </div>
	</div>
</div>
";
    portalOption.PublicLandingPageBody = @"<div style=""width:1000px;margin: 0 auto;font: 12px arial"">
	<div class=""widgetcontainer"">
		<div class=""LandingPageContent"">
			<h3 style=""color:#3079c6"">Popular KnowledgeBase Articles</h3>
			<div class=""LandingPageInnerContent"">
				<ts_knowledgebase,5,Images/file.png>
			</div>
			<h3 style=""color:#3079c6"">Recent KnowledgeBase Articles</h3>
			<div class=""LandingPageInnerContent"">
				<ts_latestknowledgebase,5,Images/file.png>
			</div>
		</div>
	</div>
</div>
";
    portalOption.PublicLandingPageHeader =  @"<div style=""width:1000px;margin:0 auto;"">
    	<div style=""font: 20px arial;text-align:center;color:#666"">
		Welcome to our customer support center!
		<br />
		<div style=""width:1000px;margin:0 auto;"">
		<ul class=""icon"">
			<li>
			<a title=""Home"" href=""<ts_home>"">
				<img alt="""" src=""Images/plp_home.png"" />
				<span>Home</span>
			</a>
			</li>
			<li>
			<a title=""Create New Ticket"" href=""<ts_newticketlink>"">
				<img alt="""" src=""Images/ico-newticket1.png"" />
				<span>New Ticket</span>
			</a>
			</li>
			<li>
			<a title=""KnowledgeBase"" href=""<ts_knowlegebaselink>"">
				<img alt="""" src=""Images/ico-kb.png"" />
				<span>Knowledgebase</span>
			</a>
			</li>
			<li style=""display:none;"">
			<a title=""Articles"" href=""<ts_articlelink>"">
				<img alt="""" src=""Images/ico-articles1.png"" />
				<span>Articles</span>
			</a>
			</li>
			<li>
			<ts_chatlink>
				<img alt="""" src=""Images/ico-chat2.png"" />
				<span>Chat</span>
			</a>
			</li>
			<li>
			<a title=""My Tickets"" href=""#"" id=""button"">
				<img alt="""" src=""Images/mytickets.png"" />
				<span>My Tickets</span>
			</a>
			</li>
			</ul>
		</div> 
	</div>
</div>";
    portalOptions.Save();

    EmailPosts.SendWelcomeNewSignup(loginUser, user.UserID, password);
    EmailPosts.SendSignUpNotification(loginUser, user.UserID);
    AddToMuroc(organization, user, phoneNumber.Number);
    //_manager.Redirect("http://teamsupport.com/ThankYouSignUp.aspx");
    Response.Redirect("SignUpThanks.aspx");
  }

  private static void AddToMuroc(Organization tsOrg, User tsUser, string phoneNumber)
  {
    Organization mOrg = (new Organizations(tsOrg.Collection.LoginUser)).AddNewOrganization();
    mOrg.ParentID = 1078;
    mOrg.Name = tsOrg.Name;
    mOrg.ImportID = tsOrg.OrganizationID.ToString();
    mOrg.HasPortalAccess = true;
    mOrg.IsActive = true;
    mOrg.Collection.Save();

    User mUser = (new Users(tsOrg.Collection.LoginUser)).AddNewUser();
    mUser.OrganizationID = mOrg.OrganizationID;
    mUser.FirstName = tsUser.FirstName;
    mUser.LastName = tsUser.LastName;
    mUser.Email = tsUser.Email;
    mUser.IsActive = true;
    mUser.IsPortalUser = true;
    mUser.ImportID = tsUser.UserID.ToString();
    mUser.Collection.Save();

    mOrg.PrimaryUserID = mUser.UserID;
    mOrg.Collection.Save();

    PhoneNumber phone = (new PhoneNumbers(tsOrg.Collection.LoginUser)).AddNewPhoneNumber();
    phone.RefID = mOrg.OrganizationID;
    phone.RefType = ReferenceType.Organizations;
    phone.Number = phoneNumber;
    phone.Collection.Save();

    OrganizationProduct op = null;
    OrganizationProducts ops = new OrganizationProducts(tsOrg.Collection.LoginUser);
    op = ops.AddNewOrganizationProduct();
    op.OrganizationID = mOrg.OrganizationID;
    op.ProductID = 219; //TeamSupport
    op.ProductVersionID = null;
    op.IsVisibleOnPortal = true;

    op = ops.AddNewOrganizationProduct();
    op.OrganizationID = mOrg.OrganizationID;
    op.ProductID = 233;  //Email Handler
    op.ProductVersionID = null;
    op.IsVisibleOnPortal = true;

    op = ops.AddNewOrganizationProduct();
    op.OrganizationID = mOrg.OrganizationID;
    op.ProductID = 234;  // Adv Portal
    op.ProductVersionID = null;
    op.IsVisibleOnPortal = true;

    op = ops.AddNewOrganizationProduct();
    op.OrganizationID = mOrg.OrganizationID;
    op.ProductID = 1068;  // Basic Portal
    op.ProductVersionID = null;
    op.IsVisibleOnPortal = true;

    op = ops.AddNewOrganizationProduct();
    op.OrganizationID = mOrg.OrganizationID;
    op.ProductID = 1970;  //Chat
    op.ProductVersionID = null;
    op.IsVisibleOnPortal = true;

    op = ops.AddNewOrganizationProduct();
    op.OrganizationID = mOrg.OrganizationID;
    op.ProductID = 2580;  // KB
    op.ProductVersionID = null;
    op.IsVisibleOnPortal = true;

    op = ops.AddNewOrganizationProduct();
    op.OrganizationID = mOrg.OrganizationID;
    op.ProductID = 1877;  // API
    op.ProductVersionID = null;
    op.IsVisibleOnPortal = true;
    ops.Save();


  }

}
