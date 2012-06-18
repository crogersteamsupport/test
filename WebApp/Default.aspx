<%@ Page Language="C#" AutoEventWireup="False" CodeFile="Default.aspx.cs" Inherits="_Default"
  EnableEventValidation="false" EnableViewState="false" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<%@ Register TagPrefix="scriptSvc" Namespace="ScriptReferenceProfiler" Assembly="ScriptReferenceProfiler, Version=1.1.0.0, Culture=neutral" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<html>
<head id="Head1" runat="server">
  <title>Team Support</title>
  <link rel="SHORTCUT ICON" href="~/favicon.ico" />
  <link href="vcr/142/Css/jquery-ui-latest.custom.css" rel="stylesheet" type="text/css" />
  <link href="vcr/142/Css/jquery-ui-enhanced.css" rel="stylesheet" type="text/css" />
  <link href="vcr/142/Css/jquery.jgrowl.css" rel="stylesheet" type="text/css" />
  <link href="vcr/142/Css/jquery.ketchup.css" rel="stylesheet" type="text/css" />
  <link href="vcr/142/Css/ts.ui.css" rel="stylesheet" type="text/css" />
  <link href="vcr/142/Css/jquery.ui.combobox.css" rel="stylesheet" type="text/css" />
  <link href="vcr/142/Css/jquery.ui.timepicker.css" rel="stylesheet" type="text/css" />
  <!--[if IE 7]><link href="vcr/142/Css/ts.ui.ie7.css" rel="stylesheet" type="text/css" /><![endif]--><!--[if IE 8]><link href="vcr/142/Css/ts.ui.ie8.css" rel="stylesheet" type="text/css" /><![endif]-->
  <link href="vcr/142/Css/ts.mainpage.css" rel="stylesheet" type="text/css" />

  <script src="vcr/142/Js/jquery-1.5.1.min.js" type="text/javascript"></script>
  <script src="vcr/142/Js/jquery-ui-1.8.14.custom.min.js" type="text/javascript"></script>

  <script type="text/javascript" language="javascript">

    var g_uac = ''

    var privateServices;
    var g_PrivateServices;
    var _selectContactID = -1;
    var _selectCustomerID = -1;

    function teamSupportLoad() {
      g_uac = $('#fieldAuth').val();
      Ts.MainPage = new Ts.Pages.Main();
      Ts.MainPage.init();

      g_PrivateServices = privateServices = new TeamSupport.Services.PrivateServices();
      g_PrivateServices.set_defaultSucceededCallback(function (result) { });
      g_PrivateServices.set_defaultFailedCallback(function (error, userContext, methodName) {
        //if (error !== null) { alert("An error occurred: " + error.get_message()) };
      });
      //document.write(unescape("%3Cscript charset='utf-8' id='screenr_recorder' src='https://teamsupport.viewscreencasts.com/api/recorder' type='text/javascript'%3E%3C/script%3E"));
      /*$.getScript('https://teamsupport.viewscreencasts.com/api/recorder', function () {
      alert('loaded');
      var recorder = Screenr.Recorder({ id: "b67bdeab7c084032bc4f37e5308eae1e", hideAllFields: true, maxTimeLimit: 300 });
      recorder.record();
      });*/



    }

    

  </script>

  <style type="text/css">
    html, body, form { height: 100%; margin: 0; padding: 0; overflow: hidden; }
  </style>
</head>
<body>
  <form id="form1" runat="server">
  <asp:ScriptManager ID="ScriptManager1" runat="server" ScriptMode="Release" 
    EnableScriptGlobalization="True" >
    <services>
        <asp:ServiceReference Path="~/Services/TSSystem.asmx" />
        <asp:ServiceReference Path="~/Services/SettingService.asmx" />
        <asp:ServiceReference Path="~/Services/TicketService.asmx" />
        <asp:ServiceReference Path="~/Services/CustomFieldsService.asmx" />
        <asp:ServiceReference Path="~/Services/AutomationService.asmx" />
        <asp:ServiceReference Path="~/Services/AdminService.asmx" />
        <asp:ServiceReference Path="~/Services/UserService.asmx" />
        <asp:ServiceReference Path="~/Services/WaterCoolerService.asmx" />
        <asp:ServiceReference Path="~/Services/OrganizationService.asmx" />
        <asp:ServiceReference Path="~/Services/ProductService.asmx" />
        <asp:ServiceReference Path="~/Services/AssetService.asmx" />

        <asp:ServiceReference Path="~/Services/PrivateServices.asmx" />
      </services>
    <scripts>
        
          <asp:ScriptReference Path="vcr/142/Js/json2.js" />
          <asp:ScriptReference Path="vcr/142/Js/jquery.layout.min.js" />
          <asp:ScriptReference Path="vcr/142/Js/jquery.jgrowl_minimized.js" />
          <asp:ScriptReference Path="vcr/142/Js/jquery.editlabel.js" />
          <asp:ScriptReference Path="vcr/142/Js/jquery.ui.combobox.js" />
          <asp:ScriptReference Path="vcr/142/Js/jquery.ui.timepicker.js" />
          <asp:ScriptReference Path="vcr/142/Js/jquery.cookie.js" />
          <asp:ScriptReference Path="vcr/142/Js/ts/ts.system.js" />
          <asp:ScriptReference Path="vcr/142/Js/ts/ts.utils.js" />
          <asp:ScriptReference Path="vcr/142/Js/ts/ts.cache.js" />
          <asp:ScriptReference Path="vcr/142/Js/ts/ts.ui.tabs.js" />
          <asp:ScriptReference Path="vcr/142/Js/ts/ts.ui.menutree.js" />
          <asp:ScriptReference Path="vcr/142/Js/ts/ts.pages.main.js" />
          <asp:ScriptReference Path="js_5/dialogs.js" />

      </scripts>
    <compositescript scriptmode="Release">
        <Scripts>
          <asp:ScriptReference name="MicrosoftAjax.js"/>
	        <asp:ScriptReference name="MicrosoftAjaxWebForms.js"/>
          
        </Scripts>
      </compositescript>
  </asp:ScriptManager>
  <asp:HiddenField ID="fieldSID" runat="server" Value="1234"></asp:HiddenField>
  <div class="main-loading ts-loading">
  </div>
  <div class="main-container ui-widget ui-helper-hidden">
    <div class="main-content">
      <div class="ui-helper-hidden classic-tabs"></div>
      <div class="main-tab-content">
        <div class="ts-loading">
        </div>
      </div>
    </div>
    <div class="main-header new-view-header">
      <div class="new-view">
        <div class="main-header-title">
          <div class="main-header-content-wrapper">
            <div class="main-header-content">
              <div class="main-company-logo"></div>
              <a class="main-header-new-ticket main-link-newticket" href="#"><span></span></a>
              <input class="ui-corner-all ui-widget-content main-quick-ticket" type="text" />
            </div>
          </div>
          <div class="main-header-right">
            <div class="main-header-menu">
            <ul>
              <li class="menu-chatstatus notlast"><a href="#"><span class="ts-icon"></span><span class="menu-chatstatus-text"></span></a></li>
              <li class="menu-officestatus notlast"><a href="#"><span class="ts-icon"></span><span>Office Status</span><span class="ui-icon ui-icon-triangle-1-s"></span></a></li>
              <li class="menu-help notlast"><a href="#"><span>Help</span><span class="ui-icon ui-icon-triangle-1-s"></span></a></li>
              <li class="menu-signout"><a href="#"><span>Sign Out</span></a></li>
            </ul>
            </div>
          </div>
        </div>
        <div class="main-tabs ui-widget-header"></div>
      </div>
      <div class="ui-helper-hidden classic-view">
        <div class="main-header-left">
          <div class="main-header-logo"></div>
          <div class="main-header-status">
            <span>My Status:</span> <span class="main-status-chat ts-icon ts-icon-chat ts-clickable">
            </span><span class="main-status-online ts-icon ts-icon-offline ts-clickable"></span>
            <span class="main-header-status-text ts-clickable">What is your status?</span>
          </div>
        </div>
        <div class="main-header-links">
          <div class="old-header-links">
            <a href="http://help.teamsupport.com" target="TSHelp">Documentation</a>
            <a class="menu-help-chat" href="#">Chat With Us</a>
            <a class="menu-help-support" href="#">Support</a>
            <a class="menu-signout" href="#">Sign Out</a>
          </div>
          <ul class="ui-helper-hidden">
            <li><a class="menu-help" href="#"><span>Help</span><span class="ui-icon ui-icon-triangle-1-s"></span></a></li>
            <li><a class="menu-signout" href="#">Sign Out</a></li>
          </ul>
        </div>
      </div>
    </div>
    <div class="main-footer ui-widget-header">
      <div class="main-status-left">
      </div>
      <div class="main-status-right">
        <span class="status-version status-last">Version: </span>
        <span class="status-expiration ui-helper-hidden"></span>
        <span class="status-frame ui-helper-hidden"></span>
        <span class="status-debug ui-helper-hidden"></span>
      </div>
    </div>
    <div class="main-info ui-widget-content ts-noborder">
      <div class="ui-widget-header">
        <a href="#" class="main-info-close">Hide this window</a></div>
      <div class="main-info-content">
      </div>
    </div>
    <div class="main-nav ui-widget-content ts-noborder">
      <div class="main-quick-panel ui-widget-header ui-helper-hidden classic-view">
        <div>
          <a href="#" class="main-link-newticket ui-corner-all ui-state-default"><span class="ts-icon ts-icon-new">
          </span>Create New Ticket</a></div>
        <div>
          <input type="text" class="main-quick-ticket text ui-widget-content ui-corner-all" /></div>
      </div>
      <div class="main-menutree">
      </div>
    </div>
  </div>


  <telerik:RadWindowManager ID="WindowManager" runat="server" Behaviors="Move, Close"
    Modal="True" Overlay="True" VisibleStatusbar="False" IconUrl="~/images/icons/TeamSupportLogo16.png"
    ReloadOnShow="True" InitialBehavior="None" ShowContentDuringLoad="False" KeepInScreenBounds="True"
    AutoSize="False" DestroyOnClose="False">
    <Windows>
      <telerik:RadWindow ID="wndDialog" runat="server" NavigateUrl="~/Dialog.aspx" Style="display: none;"
        AutoSize="true">
      </telerik:RadWindow>
      <telerik:RadWindow ID="wndNewTicket" runat="server" Style="display: none;">
      </telerik:RadWindow>
      <telerik:RadWindow ID="wndSelectOrganization" runat="server" Style="display: none;">
      </telerik:RadWindow>
      <telerik:RadWindow ID="wndActionOld" runat="server" Style="display: none;">
      </telerik:RadWindow>
      <telerik:RadWindow ID="wndHistory" runat="server" Style="display: none;">
      </telerik:RadWindow>
      <telerik:RadWindow ID="wndAddress" runat="server" Style="display: none;">
      </telerik:RadWindow>
      <telerik:RadWindow ID="wndPhone" runat="server" Style="display: none;">
      </telerik:RadWindow>
      <telerik:RadWindow ID="wndUser" runat="server" Style="display: none;">
      </telerik:RadWindow>
      <telerik:RadWindow ID="wndContact" runat="server" Style="display: none;">
      </telerik:RadWindow>
      <telerik:RadWindow ID="wndSelectGroup" runat="server" Style="display: none;">
      </telerik:RadWindow>
      <telerik:RadWindow ID="wndGroup" runat="server" Style="display: none;">
      </telerik:RadWindow>
      <telerik:RadWindow ID="wndSelectUser" runat="server" Style="display: none;">
      </telerik:RadWindow>
      <telerik:RadWindow ID="wndOrganization" runat="server" Style="display: none;">
      </telerik:RadWindow>
      <telerik:RadWindow ID="wndMyCompany" runat="server" Style="display: none;">
      </telerik:RadWindow>
      <telerik:RadWindow ID="wndCRMProperties" runat="server" Style="display: none;">
      </telerik:RadWindow>
      <telerik:RadWindow ID="wndChatProperties" runat="server" Style="display: none;">
      </telerik:RadWindow>
      <telerik:RadWindow ID="wndOrganizationProduct" runat="server" Style="display: none;">
      </telerik:RadWindow>
      <telerik:RadWindow ID="wndAttachFile" runat="server" Style="display: none;">
      </telerik:RadWindow>
      <telerik:RadWindow ID="wndNote" runat="server" Style="display: none;">
      </telerik:RadWindow>
      <telerik:RadWindow ID="wndProduct" runat="server" Style="display: none;">
      </telerik:RadWindow>
      <telerik:RadWindow ID="wndVersion" runat="server" Style="display: none;">
      </telerik:RadWindow>
      <telerik:RadWindow ID="wndCustomField" runat="server" Style="display: none;">
      </telerik:RadWindow>
      <telerik:RadWindow ID="wndAccount" runat="server" Style="display: none;">
      </telerik:RadWindow>
      <telerik:RadWindow ID="wndPortalOptions" runat="server" Style="display: none;">
      </telerik:RadWindow>
      <telerik:RadWindow ID="wndReportEditor" runat="server" Style="display: none;" Behaviors="Move, Close">
      </telerik:RadWindow>
      <telerik:RadWindow ID="wndSlaLevel" runat="server" Style="display: none;">
      </telerik:RadWindow>
      <telerik:RadWindow ID="wndSlaTrigger" runat="server" Style="display: none;">
      </telerik:RadWindow>
      <telerik:RadWindow ID="wndHelp" runat="server" Style="display: none;">
      </telerik:RadWindow>
      <telerik:RadWindow ID="wndAction" runat="server" NavigateUrl="Dialogs/TicketAction.aspx"
        Width="800px" Height="500px" Animation="None" KeepInScreenBounds="False" VisibleStatusbar="False"
        VisibleTitlebar="True" IconUrl="~/images/icons/action.png" VisibleOnPageLoad="false"
        ShowContentDuringLoad="False" Modal="True">
      </telerik:RadWindow>
    </Windows>
  </telerik:RadWindowManager> <telerik:RadWindow ID="wndIntroVideo" runat="server" VisibleStatusbar="False" VisibleTitlebar="True"
    Width="460" Height="415" Behaviors="Close,Move" AutoSize="False" VisibleOnPageLoad="False"
    NavigateUrl="Videos\Intro.aspx" IconUrl="images/icons/TeamSupportLogo16.png" Title="Introduction"
    DestroyOnClose="True">
  </telerik:RadWindow>
  <telerik:RadWindow ID="wndSelectUserDialog" runat="server" Width="300px" Height="150px"
    Animation="None" KeepInScreenBounds="True" VisibleStatusbar="False" VisibleTitlebar="True"
    OnClientPageLoad="" Title="Select a User" Behaviors="Close,Move" IconUrl="images/icons/TeamSupportLogo16.png"
    VisibleOnPageLoad="false" ShowContentDuringLoad="False" Modal="False" DestroyOnClose="True">
    <ContentTemplate>
      <div style="padding: 7px 20px;">
        <div style="font-weight: bold;">
          User Name:</div>
        <div style="padding: 7px 0;">
          <telerik:RadComboBox ID="cmbUser" runat="server" Width="100%" AllowCustomText="True"
            CssClass="wndSelectUserDialogCombo" MarkFirstMatch="True" ShowToggleImage="false"
            EnableLoadOnDemand="True" LoadingMessage="Loading users..." OnClientItemsRequesting="function(sender,args){ args.get_context()['FilterString'] = _userDialogFilter + ',' + args.get_text();}"
            OnClientDropDownClosed="function(sender,args){sender.clearItems();}" EmptyMessage="Enter a user name..."
            ShowDropDownOnTextboxClick="False" CausesValidation="False" ChangeTextOnKeyBoardNavigation="False"
            ShowMoreResultsBox="False">
            <ExpandAnimation Type="None" />
            <WebServiceSettings Path="Default.aspx" Method="GetUsers" />
            <CollapseAnimation Duration="200" Type="None" />
          </telerik:RadComboBox>
        </div>
        <div style="float: right;">
          <asp:Button ID="btnOk" runat="server" Text="OK" OnClientClick="CloseSelectUserDialog(true); return false;" />&nbsp
          <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClientClick="CloseSelectUserDialog(false); return false;" />
        </div>
      </div>
      <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

        <script type="text/javascript" language="javascript">
          var _userDialogFilter;

          function CloseSelectUserDialog(accepted) {
            var wnd = $find("<%=wndSelectUserDialog.ClientID%>");
            wnd.argument = accepted ? $find('<%= wndSelectUserDialog.ContentContainer.FindControl("cmbUser").ClientID %>').get_value() : null;
            wnd.close();
          }
        </script>

      </telerik:RadCodeBlock>
    </ContentTemplate>
  </telerik:RadWindow>
  <telerik:RadWindow ID="wndSelectTicketDialog" runat="server" Width="300px" Height="150px"
    Animation="None" KeepInScreenBounds="True" VisibleStatusbar="False" VisibleTitlebar="True"
    OnClientPageLoad="" Title="Select a Ticket" Behaviors="Close,Move" IconUrl="images/icons/TeamSupportLogo16.png"
    VisibleOnPageLoad="false" ShowContentDuringLoad="False" Modal="False" DestroyOnClose="True">
    <ContentTemplate>
      <div style="padding: 7px 20px;">
        <div style="font-weight: bold;">
          Ticket Number or Description:</div>
        <div style="padding: 7px 0;">
          <telerik:RadComboBox ID="cmbTicket" runat="server" Width="100%" AllowCustomText="True"
            CssClass="wndSelectTicketDialogCombo" MarkFirstMatch="True" ShowToggleImage="false"
            EnableLoadOnDemand="True" LoadingMessage="Loading tickets..." OnClientItemsRequesting="function(sender,args){ args.get_context()['FilterString'] = args.get_text();}"
            OnClientDropDownClosed="function(sender,args){sender.clearItems();}" EmptyMessage="Search Tickets..."
            ShowDropDownOnTextboxClick="False" CausesValidation="False" ChangeTextOnKeyBoardNavigation="False"
            ShowMoreResultsBox="False">
            <ExpandAnimation Type="None" />
            <WebServiceSettings Path="Services/PrivateServices.asmx" Method="GetQuickTicket" />
            <CollapseAnimation Duration="200" Type="None" />
          </telerik:RadComboBox>
        </div>
        <div style="float: right;">
          <asp:Button ID="btnOk" runat="server" Text="OK" OnClientClick="CloseSelectTicketDialog(true); return false;" />&nbsp
          <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClientClick="CloseSelectTicketDialog(false); return false;" />
        </div>
      </div>
      <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

        <script type="text/javascript" language="javascript">
          function CloseSelectTicketDialog(accepted) {
            var wnd = $find("<%=wndSelectTicketDialog.ClientID%>");
            wnd.argument = accepted ? $find('<%= wndSelectTicketDialog.ContentContainer.FindControl("cmbTicket").ClientID %>').get_value() : null;
            wnd.close();
          }
        </script>

      </telerik:RadCodeBlock>
    </ContentTemplate>
  </telerik:RadWindow>

  <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

    <script type="text/javascript" language="javascript">
      function get_wndSelectUserDialogClientID() { return "<%= wndSelectUserDialog.ClientID %>"; }
      function get_wndSelectTicketDialogClientID() { return "<%= wndSelectTicketDialog.ClientID %>"; }
      function ShowUserDialog(filter, isModal, callback, title) {
        var wnd = $find(get_wndSelectUserDialogClientID());
        _userDialogFilter = filter;
        ShowDialog(wnd, isModal, callback, title);
      }

      function ShowTicketDialog(isModal, callback, title) {
        var wnd = $find(get_wndSelectTicketDialogClientID());
        ShowDialog(wnd, isModal, callback, title);
      }

      function ShowDialog(wnd, isModal, callback, title) {
        if (title && title != '') wnd.set_title(title);
        wnd.set_modal(isModal);
        if (callback) {
          var fn = function (sender, args) { sender.remove_close(fn); callback(sender.argument); }
          wnd.add_close(fn);
        }
        wnd.show();
      }

    </script>

  </telerik:RadCodeBlock>

  <div class="dialog-select-ticket ui-helper-hidden" title="Select a Ticket">
	  <p>Enter a ticket number or description:</p>
    <input type="text" class="text ui-corner-all ui-widget-content" style="width:90%;"/>
  </div>

  <div class="dialog-reminder ui-helper-hidden" title="Reminder">
    <div class="ts-loading"></div>
    <div class="dialog-reminder-form ui-helper-hidden">
      <div class="label-block" style="float:none; display:block;">
        <span class="label">What is this reminder all about?</span>
        <input type="text" class="text ui-corner-all ui-widget-content reminder-description" style="width:95%;"/>
      </div>
      <div class="label-block" style="float:none; display:block;">
        <span class="label">When do you want to be reminded?</span>
        <input type="text" class="text ui-corner-all ui-widget-content reminder-date" style="width:95%"/>
      </div>
      <div class="label-block" style="float:none; display:block;">
        <span class="label">Who do you want to remind?</span>
        <select class="reminder-user"></select>
      </div>
    </div>
  </div>

  <div class="menu-popup menu-popup-officestatus ui-widget-content ui-widget ui-corner-bottom new-view-menu">
    <ul>
      <li class="menu-office-online"><a class="ts-link ui-state-default" href="#"><span class="ts-icon ts-icon-online"></span><span>Available</span></a><div class="ts-clearfix"></div></li>
      <li class="menu-office-offline"><a class=" ts-link ui-state-default" href="#"><span class="ts-icon ts-icon-offline"></span><span>Busy</span></a><div class="ts-clearfix"></div></li>
    </ul>
    <div class="ts-clearfix"></div>
    <div class="ts-separator ui-widget-content"></div>
    <div class="label-block label-block-nofloat">
      <span class="label">My Status</span>
      <input type="text"  class="ui-widget-content ui-corner-all text menu-status-text menu-input" style="width: 200px"/>
    </div>
    <div class="menu-office-status-action ui-helper-hidden">
      <button class="menu-office-save">Save Status</button> or <a href="#" class="ts-link ui-state-default menu-office-cancel">Cancel</a>
    </div>
  </div>

  <div class="menu-popup menu-popup-help ui-widget-content ui-widget ui-corner-bottom new-view-menu">
    <ul>
      <li><a class="ts-link ui-state-default" href="http://help.teamsupport.com" target="TSHelp">Documentation</a></li>
      <li><a class="menu-help-chat ts-link ui-state-default" href="#">Chat with us</a></li>
      <li><a class="menu-help-support ts-link ui-state-default" href="#">Support portal</a></li>
      <li><div class="ui-widget-content ts-separator"></div></li>
      <li><a class="menu-help-switch ts-link ui-state-default" href="#">Switch to classic view</a></li>
      <input type="text"  class="menu-input menu-input-hidden"/>
    </ul>
  </div>
  </form>
</body>
</html>
