<%@ Page Language="C#" AutoEventWireup="False" CodeFile="Default.aspx.cs" Inherits="_Default"
  EnableEventValidation="false" EnableViewState="false" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<%@ Register TagPrefix="scriptSvc" Namespace="ScriptReferenceProfiler" Assembly="ScriptReferenceProfiler, Version=1.1.0.0, Culture=neutral" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<html>
<head id="Head1" runat="server">
  <title>Team Support</title>
   <link rel="SHORTCUT ICON" href="~/favicon.ico" />
  <link href="vcr/1_9_0/Css/jquery-ui-latest.custom.css" rel="stylesheet" type="text/css" />
  <link href="vcr/1_9_0/Css/jquery-ui-enhanced.css" rel="stylesheet" type="text/css" />
  <link href="vcr/1_9_0/Css/jquery.jgrowl.css" rel="stylesheet" type="text/css" />
  <link href="vcr/1_9_0/Css/ts.ui.css" rel="stylesheet" type="text/css" />
  <link href="vcr/1_9_0/Css/jquery.ui.combobox.css" rel="stylesheet" type="text/css" />
  <link href="vcr/1_9_0/Css/jquery.ui.timepicker.css" rel="stylesheet" type="text/css" />
  <!--[if IE 7]><link href="vcr/1_9_0/Css/ts.ui.ie7.css" rel="stylesheet" type="text/css" /><![endif]--><!--[if IE 8]><link href="vcr/1_9_0/Css/ts.ui.ie8.css" rel="stylesheet" type="text/css" /><![endif]-->
  <link href="vcr/1_9_0/Css/ts.mainpage.css" rel="stylesheet" type="text/css" />
  <link href="vcr/1_9_0/Css/chat.css" rel="stylesheet" type="text/css" />
  <link href="vcr/1_9_0/Css/jquery.pnotify.default.icons.css" rel="stylesheet" />
  <link href="vcr/1_9_0/Css/jquery.pnotify.default.css" rel="stylesheet" />

  <script src="vcr/1_9_0/Js/jquery-latest.min.js" type="text/javascript"></script>
  <script src="vcr/1_9_0/Js/jquery-ui-latest.custom.min.js" type="text/javascript"></script>
  <script src="vcr/1_9_0/Js/chat.js" type="text/javascript"></script>
  <script src="vcr/1_9_0/Js/json2.min.js" type="text/javascript"></script>
  <script src="vcr/1_9_0/Js/browser.js" type="text/javascript"></script>
  <script src="vcr/1_9_0/Js/jquery.signalR-2.1.2.min.js" type="text/javascript"></script>
  <script src="vcr/1_9_0/Js/jquery.jplayer.min.js" type="text/javascript"></script>
  <script src="vcr/1_9_0/Js/jquery.pnotify.min.js" type="text/javascript"></script>
  <script src="vcr/1_9_0/Js/moment.min.js" type="text/javascript"></script>

  <script src="../js_5/imagepaste.js" type="text/javascript"></script>
  <script src="../js_5/jquery.Jcrop.js" type="text/javascript"></script>
  <script src="https://crypto-js.googlecode.com/svn/tags/3.1.2/build/rollups/aes.js"></script>
  <!-- Start Apptegic Code -->
  <script type="text/javascript">
    var _aaq = _aaq || [];
    var _evergageDataset = '';
    var _evergageAccount = 'teamsupport';
    if (window.location.hostname.indexOf('beta.teamsupport') > -1) { _evergageDataset = 'MainAppBeta' }
    else if (window.location.hostname.indexOf('alpha.teamsupport') > -1) { _evergageDataset = 'MainAppAlpha' }
    else if (window.location.hostname.indexOf('app.teamsupport') > -1) { _evergageDataset = 'MainApp' }
    else if (window.location.hostname.indexOf('tsdev') > -1) { _evergageDataset = 'MainApp_Dev' }
    if (_evergageDataset != '') {
      (function () {
        var d = document, g = d.createElement('script'), s = d.getElementsByTagName('script')[0];
        g.type = 'text/javascript'; g.defer = true; g.async = true;
        g.src = document.location.protocol + '//cdn.evergage.com/beacon/'
              + _evergageAccount + '/' + _evergageDataset + '/scripts/evergage.min.js';
        s.parentNode.insertBefore(g, s);
      })();
    }
  </script>
  <!-- End Apptegic Code -->

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
      /*
      try {
        window.onbeforeunload = function () {
          if (console && console.trace && console.log) {
            console.log('Refreshing stack trace:'); console.trace();
          } return "Warning!";
        }
      } catch (e) { }
      */
      g_PrivateServices = privateServices = new TeamSupport.Services.PrivateServices();
      g_PrivateServices.set_defaultSucceededCallback(function (result) { });
      g_PrivateServices.set_defaultFailedCallback(function (error, userContext, methodName) { });
      var signalRUrl = $("#SignalRUrl").val(); 
      if (BrowserDetect.browser != 'Safari' || BrowserDetect.isMobile != 1) {
        try {
          $.getScript(signalRUrl + "/hubs", function (data, textStatus, jqxhr) {
            $.getScript("vcr/1_9_0/Js/ts/ts.wc.signalr.js", function (data, textStatus, jqxhr) {
              if (loadSignalR) { loadSignalR(signalRUrl); }

            });
          });
        } catch (e) { }
      }
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
        <asp:ServiceReference Path="~/Services/TicketPageService.asmx" />
        <asp:ServiceReference Path="~/Services/CustomFieldsService.asmx" />
        <asp:ServiceReference Path="~/Services/AutomationService.asmx" />
        <asp:ServiceReference Path="~/Services/AdminService.asmx" />
        <asp:ServiceReference Path="~/Services/UserService.asmx" />
        <asp:ServiceReference Path="~/Services/ReportService.asmx" />
        <asp:ServiceReference Path="~/Services/WikiService.asmx" />
        <asp:ServiceReference Path="~/Services/WaterCoolerService.asmx" />
        <asp:ServiceReference Path="~/Services/OrganizationService.asmx" />
        <asp:ServiceReference Path="~/Services/ProductService.asmx" />
        <asp:ServiceReference Path="~/Services/AssetService.asmx" />
        <asp:ServiceReference Path="~/Services/SearchService.asmx" />
        <asp:ServiceReference Path="~/Services/CustomerService.asmx" />
        <asp:ServiceReference Path="~/Services/PublicService.asmx" />
        <asp:ServiceReference Path="~/Services/PrivateServices.asmx" />
        <asp:ServiceReference Path="~/Services/LoginService.asmx" />
      </services>
    <scripts>
    
          <asp:ScriptReference Path="vcr/1_9_0/Js/json2.js" />
          <asp:ScriptReference Path="vcr/1_9_0/Js/jquery.layout.min.js" />
          <asp:ScriptReference Path="vcr/1_9_0/Js/jquery.jgrowl_minimized.js" />
          <asp:ScriptReference Path="vcr/1_9_0/Js/jquery.editlabel.js" />
          <asp:ScriptReference Path="vcr/1_9_0/Js/jquery.ui.combobox.js" />
          <asp:ScriptReference Path="vcr/1_9_0/Js/jquery.ui.timepicker.js" />
          <asp:ScriptReference Path="vcr/1_9_0/Js/ts/ts.system.js" />
          <asp:ScriptReference Path="vcr/1_9_0/Js/ts/ts.utils.js" />
          <asp:ScriptReference Path="vcr/1_9_0/Js/ts/ts.cache.js" />
          <asp:ScriptReference Path="vcr/1_9_0/Js/ts/ts.ui.tabs.js" />
          <asp:ScriptReference Path="vcr/1_9_0/Js/ts/ts.ui.menutree.js" />
          <asp:ScriptReference Path="vcr/1_9_0/Js/ts/ts.pages.main.js" />
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
  <asp:HiddenField ID="SignalRUrl" runat="server" Value=""></asp:HiddenField>
  <div id="jquery_jplayer_1"></div>
  <div class="main-loading ts-loading">
  </div>
  <div class="main-container ui-widget ui-helper-hidden">
    <div class="main-content">
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
      <telerik:RadWindow ID="wndImagePaste" runat="server" Style="display: none;">
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
    </Windows>
  </telerik:RadWindowManager> 

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
          <asp:Button ID="btnOk1" runat="server" Text="OK" OnClientClick="CloseSelectTicketDialog(true); return false;" />&nbsp
          <asp:Button ID="btnCancel1" runat="server" Text="Cancel" OnClientClick="CloseSelectTicketDialog(false); return false;" />
        </div>
      </div>
      <telerik:RadCodeBlock ID="RadCodeBlock3" runat="server">

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

  <telerik:RadCodeBlock ID="RadCodeBlock2" runat="server">

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

  <div class="dialog-select-wiki ui-helper-hidden" title="Select a Wiki Article">
	  <p>Enter a wiki title:</p>
    <input type="text" class="text ui-corner-all ui-widget-content" style="width:90%;"/>
  </div>

  <div id="dialog-paste-imageID" class="dialog-paste-image ui-helper-hidden" title="Paste Image">
        <div class="image-cropper">
            <div id="mainImageDiv" style="height:245px;margin:0 auto;overflow:auto">
                <div class="paste-dialog-instructions"><p>Please click here and paste your image</p><br /> <p>Windows users Ctrl+V, Mac Users Cmd+V or from your browser menu select edit and then paste.</p></div>
                <img id="testImage" class="image" style="opacity:0" src="" />
            </div>
            <div id="imageOptions" style="display:none">
                <div style="text-align:center">
                    <button id="resizeButton">Resize Image</button>
                    <button id="cropButton">Crop Image</button>
                    <button id="clearButton">Clear Image</button>
                </div>

                <br />
                
                <div id="resizeOptions" style="text-align:center;display:none">
                    <input type="text" id="imgWidth" placeholder="width" />
                    <span>x</span>
                    <input type="text" id="imgHeight" placeholder="height" />   
                    <button id="saveResizeButton">Resize</button>   
                    <p><input type="checkbox" id="paste-dialog-aspectRatio" checked />Lock aspect ratio</p>                  
                </div>                
                <div style="text-align:center;display:none">
                    <div class="preview" ></div>
                    <input id="img1" type="hidden" class="result" value="" runat="server" />
                </div>
            </div>
        </div>
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
      <li><a class="ts-link ui-state-default menu-help-docs" href="http://help.teamsupport.com" target="TSHelp">Documentation</a></li>
      <li><a class="menu-help-chat ts-link ui-state-default" href="#">Chat with us</a></li>
      <li><a class="menu-help-support ts-link ui-state-default" href="#">Support portal</a></li>
      <li><div class="ui-widget-content ts-separator"></div></li>
      <input type="text"  class="menu-input menu-input-hidden"/>
    </ul>
  </div>
  </form>
</body>
</html>
