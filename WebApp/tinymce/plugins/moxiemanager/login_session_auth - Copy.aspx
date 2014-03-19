<%@ Page Language="C#" %>
<%@ import Namespace="TeamSupport.WebUtils" %>
<%@ Import Namespace="System.IO" %>
<script runat="server">
	string message = "";
    
	void Page_Load(object sender, EventArgs e) {
			Session["isLoggedIn"] = "true";
			Session["user"] = UserSession.LoginUser.UserID.ToString();
      Directory.CreateDirectory("C:/TSData/WikiDocs/" + UserSession.LoginUser.OrganizationID + "/images");
      Session["moxiemanager.filesystem.rootpath"] = "C:/Leonardo/Repositories/Main/branches/leonardo/WebApp/tinymce/files/" + UserSession.LoginUser.OrganizationID + "/images";
      Session["moxiemanager.local.wwwroot"] = "C:/Leonardo/Repositories/Main/branches/leonardo/WebApp";
      Session["moxiemanager.local.urlprefix"] = "{proto}://{host}/leonardo/leonardo";

      if (TSAuthentication.IsSystemAdmin)
      {
        Session["moxiemanager.general.hidden_tools"] = string.Empty;
      }
        
      Response.Redirect("default.aspx", true);
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
