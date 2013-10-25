<%@ Page Language="C#" %>
<%@ import Namespace="TeamSupport.WebUtils" %>
<%@ Import Namespace="System.IO" %>
<script runat="server">
	/**
	 * This is login example page check the input parameters and sets up a session.
	 * Use this page together with the Moxiecode.Manager.Authenticators.SessionAuthenticator.
	 */

	string message = "";
    
	void Page_Load(object sender, EventArgs e) {
			// Set the sessions that the SessionAuthenticatorImpl class checks for
			Session["mc_isLoggedIn"] = "true";
			Session["mc_user"] = UserSession.LoginUser.UserID.ToString();
			Session["mc_groups"] = "";
            Directory.CreateDirectory("C:/TSData/WikiDocs/" + UserSession.LoginUser.OrganizationID + "/images");
            Session["imagemanager.filesystem.rootpath"] = "C:/TSData/WikiDocs/" + UserSession.LoginUser.OrganizationID + "/images";
            Session["imagemanager.preview.wwwroot"] = "C:/TSData/WikiDocs/" + UserSession.LoginUser.OrganizationID + "/images";
            Session["imagemanager.preview.urlprefix"] = "{proto}://{host}/Wiki/WikiDocs/" + UserSession.LoginUser.OrganizationID + "/images";

            if (!TSAuthentication.IsSystemAdmin)
            {
                Session["imagemanager.general.tools"] = "upload,refresh,insert,preview,edit,createdir";
                Session["imagemanager.general.disabled_tools"] = "addfavorite,removefavorite,addfavorites,delete";
            }
        
        Response.Redirect(Request["return_url"], true);
	}
</script>

<html>
<head>
<title>Sample login page (SessionAuthenticator)</title>
<style>
body { font-family: Arial, Verdana; font-size: 11px; }
fieldset { display: block; width: 170px; }
legend { font-weight: bold; }
label { display: block; }
div { margin-bottom: 10px; }
div.last { margin: 0; }
div.container { position: absolute; top: 50%; left: 50%; margin: -100px 0 0 -85px; }
h1 { font-size: 14px; }
.button { border: 1px solid gray; font-family: Arial, Verdana; font-size: 11px; }
.error { color: red; margin: 0; margin-top: 10px; }
</style>
</head>
<body>

<div class="container">
	<form action="login_session_auth.aspx" method="post">
		<input type="hidden" name="return_url" value="<%= Server.HtmlEncode(Request["return_url"]) %>" />

		<fieldset>

<% if (message != "") { %>
			<div class="error">
				<%= message %>
			</div>
<% } %>
		</fieldset>
	</form>
</div>

</body>
</html>
