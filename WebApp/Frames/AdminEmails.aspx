<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AdminEmails.aspx.cs" Inherits="Frames_AdminEmails" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
  <title></title>
  <link href="../css_5/frame.css" rel="stylesheet" type="text/css" />
  <link href="../css_5/ui.css" rel="stylesheet" type="text/css" />
  <link href="../css_5/jquery-ui-latest.custom.css" rel="stylesheet" type="text/css" />
  <script src="../js_5/jquery-1.4.2.min.js" type="text/javascript"></script>
  <script src="../Resources_151/Js/jquery-ui-1.8.14.custom.min.js" type="text/javascript"></script>
  <style type="text/css">
    .buttons { padding: 10px 0; }
    .placeholder-title { font-size: 1.1em; margin-top: 20px; font-weight:bold; }
    .placeholder-description { font-size: 1em; border-bottom: solid 1px #a5bdd4; margin: 5px 0;}
    .placeholder-field {font-size: 1em; margin: 0 10px;}
    h3 { font-size: 1.4em; font-weight:bold; margin:0 0 5px 0;}
    h3 span { font-weight:normal; font-size: .75em; padding-left: 10px;}
    .dataTable thead { background-color: #D6E6F4; text-align: left; white-space: nowrap; }
    .dataTable { border-left: solid 1px #A2B8CE; border-bottom: solid 1px #A2B8CE;  }
    .dataTable th { border-top: solid 1px #A2B8CE; border-right: solid 1px #A2B8CE; text-align:left; padding: 10px 20px;}
    .dataTable td { border-top: solid 1px #A2B8CE; border-right: solid 1px #A2B8CE; padding: 10px 20px;}
    .dataTable img { cursor: pointer; }
    .dataTable th.headImage, .dataTable td.headImage { border-right: none; width: 16px; padding: 5px; }
    
  </style>

  <script type="text/javascript" language="javascript">

    $(function() {
      $('button').button();
      $('#divSettings input').change(function() { $('#divSettingsButtons').show(); }).keydown(function() { $('#divSettingsButtons').show(); });
      $('#fieldsTemplate input, #fieldsTemplate textarea').change(function() { $('#divTemplateButtons').show(); }).keydown(function() { $('#divTemplateButtons').show(); });
      $('#textHeader, #textFooter').blur(function() { updatePreview(); });
      $('#cbUseTemplate').click(function() { if ($('#cbUseTemplate')[0].checked) $('#divHeaderFooter').show(); else $('#divHeaderFooter').hide(); updatePreview(); });
    });

    function pageLoad() {
      loadSettings();
      loadAltEmails();
    }

    function loadSettings() {
      PageMethods.GetOrganization(function(result) {
      $('#lnkSystem').text(result.SystemEmailID + '@teamsupport.com').attr('href','mailto:' + result.SystemEmailID + '@teamsupport.com');
        $find('textReply').set_value(result.OrganizationReplyToAddress);
        $('#cbRequireNew')[0].checked = result.RequireNewKeyword;
        $('#cbRequireKnown')[0].checked = result.RequireKnownUserForNewEmail;
        $('#cbChangeStatus')[0].checked = result.ChangeStatusIfClosed;
        $('#cbAssociatePeople')[0].checked = result.AddAdditionalContacts;
        $('#cbMatchSubject')[0].checked = result.MatchEmailSubject;
        $('#divSettingsButtons').hide();
      });
    }

    function loadAltEmails() {
      PageMethods.GetAltEmails(function(result) {
        if (result.length < 1) {
          $('#divAltEmails').html('<div style="padding: 10px 10px;">There are no alternate emails to display.</div>')
          return;
        }



        var html = '<table class="dataTable" cellspacing="0" cellpadding="5" border="0"><thead><tr>' +
          '<th class="headImage" /><th class="headImage" /><th>Email Address</th><th>Description</th><th>Group</th><th>Ticket Type</th><th>Product</th></tr></thead><tbody>';
        for (var i = 0; i < result.length; i++) {
          var item = result[i];
          html = html + '<tr><td class="headImage"><img src="../images/icons/Edit.png" alt="Edit" onclick="showAltEmailDialog(\'' + item.Email + '\');" /></td>' +
          '<td class="headImage"><img src="../images/icons/Trash.png" alt="Delete" onclick="deleteAltEmail(\'' + item.Email + '\');" /></td><td>' +
          '<a href="mailto:' + item.Email + '@teamsupport.com">' + item.Email + '@teamsupport.com</a></td><td>' +
          item.Description + '</td><td>' +
          item.Group + '</td><td>' +
          item.TicketType + '</td><td>' +
          item.Product + '</td></tr>';
        }

        html = html + '</tbody></table>';
        $('#divAltEmails').html(html);
      });
    
    }

    function saveEmailSettings() {
      PageMethods.SaveEmailSettings($find('textReply').get_value(), $('#cbRequireNew')[0].checked, $('#cbRequireKnown')[0].checked, $('#cbChangeStatus')[0].checked, $('#cbAssociatePeople')[0].checked, $('#cbMatchSubject')[0].checked);
      $('#divSettingsButtons').hide();
    }

    function loadEmailTemplate(emailTemplateID) {
      $('#divHeaderFooter').show();
      PageMethods.GetEmailTemplate(emailTemplateID, function(template) {
        var subject = $find('textSubject');
        if (subject != null) subject.set_value(template.Subject);
        $find('textHeader').set_value(template.Header);
        $find('textFooter').set_value(template.Footer);
        if (template.IsEmail) $('#divIsEmail').show(); else $('#divIsEmail').hide();
        $('#divTemplateDesc').text(template.Description);
        $('#textBody').val(template.Body);
        $('#cbIsHtml')[0].checked = template.IsHtml;
        $('#cbUseTemplate')[0].checked = template.UseTemplate;
        if (!template.UseTemplate) $('#divHeaderFooter').hide();
        $('#divTemplateButtons').hide();
        updatePreview();
        loadPlaceHolders(emailTemplateID);
      });
    }


    function updatePreview(emailTemplateID) {
      if (!$('#cbUseTemplate')[0].checked) {
        $('#frameBody').contents().find('body').html($('#textBody').val());
      }
      else {
        $('#frameBody').contents().find('body').html('');
        PageMethods.GetBuiltBody($find('textHeader').get_value(), $find('textFooter').get_value(), $('#textBody').val(), function(result) {
          $('#frameBody').contents().find('body').html(result);
        });
      }

    }

    function resetTemplate() {
      if (!confirm('Are you sure you would like to restore this template to the default?')) return;
      PageMethods.DeleteTemplate(getSelectedTemplateID(), function() {
        loadEmailTemplate(getSelectedTemplateID());
      });
    }

    function getSelectedTemplateID() { return $find('cmbTemplate').get_value(); }
    function cmbTemplate_OnClientLoad(sender, args) { loadEmailTemplate(getSelectedTemplateID()); }
    function cmbTemplate_OnClientSelectedIndexChanged(sender, args) { loadEmailTemplate(sender.get_value()); }
    function cmbTemplate_OnClientSelectedIndexChanging(sender, args) {
      if ($('#divTemplateButtons').is(':visible')) {
        args.set_cancel(!confirm("Would you like to continue without saving?"));
        return;
      }
      args.set_cancel(false);
    }

    function saveEmailTemplate() {
      var text = $find('textSubject');
      var subject = text == null ? "" : text.get_value();

      PageMethods.SaveEmailTemplate(getSelectedTemplateID(),
        subject,
        $find('textHeader').get_value(),
        $find('textFooter').get_value(),
        $('#textBody').val(),
        $('#cbIsHtml')[0].checked,
        $('#cbUseTemplate')[0].checked
        );
      $('#divTemplateButtons').hide();
    }

    function loadPlaceHolders(emailTemplateID) {
      PageMethods.GetPlaceHolders(emailTemplateID, function(result) {
        $('#divPlaceHolders').html('');
        if (emailTemplateID != getSelectedTemplateID()) return;
        for (var i = 0; i < result.length; i++) {
          var phs = result[i];
          $('#divPlaceHolders').append('<div class="placeholder-title">' + phs.Name + '</div><div class="placeholder-description">' + phs.Description + '</div>');

          for (var j = 0; j < phs.Items.length; j++) {
            var ph = phs.Items[j];
            var field = '{{' + ph.Name + '}}';
            if (ph.Description != null && ph.Description != '') field = field + ' - ' + ph.Description;
            $('#divPlaceHolders').append('<div class="placeholder-field">' + field + '</div>');
          }
        }
      });
    }

    function setPreviewMode() {
      $('#divModeEdit').hide();
      $('#divModePreview').show();
      $('#frameBody').show();
      updatePreview(getSelectedTemplateID());
      $('#textBody').hide();
    }

    function setEditMode() {
      $('#divModeEdit').show();
      $('#divModePreview').hide();
      $('#frameBody').hide();
      $('#textBody').show();
    }


    function showDialog(wnd, isModal, callback, title) {
      if (title && title != '') wnd.set_title(title);
      wnd.set_modal(isModal);
      if (callback) {
        var fn = function(sender, args) { sender.remove_close(fn); callback(sender.argument); }
        wnd.add_close(fn);
      }
      wnd.show();
    }    
    
  
  </script>

</head>
<body>
  <form id="form1" runat="server">
  <telerik:RadScriptManager ID="RadScriptManager1" runat="server" EnablePageMethods="true">
  </telerik:RadScriptManager>
  <div style="height: 100%; overflow: auto; padding: 0;">
    <div class="panel">
      <div class="panel-caption">
        <span class="panel-title">Email Settings</span><div class="panel-caption-cap">
        </div>
      </div>
      <div class="panel-body" id="divSettings">
        <fieldset class="ts-fieldset">
          <label for="lnkSystem">System Email Address: </label>
          <a id="lnkSystem" class="ts-link value" href="#"></a>
          <p> This is your company's dropbox account. We recommend you forward your companies support address to this address and make sure you share it with the rest of your team.  <strong>Forward</strong> any email to your dropbox and TeamSupport will either create a new ticket or update an existing ticket.  If the email comes from your customer and we know who they are in TeamSupport, we will associate that contact to the ticket and include any attachments. BCC or CC your dropbox when you send an email and include the ticket number in brackets on the subject line and the email will be attached to that ticket.</p>
          <label for="textReply" class="text">Organization Reply to Address</label>
          <telerik:RadTextBox ID="textReply" runat="server" CssClass="text" name="textReply"
            Width="300px">
          </telerik:RadTextBox>
          <p>Enter your companies support address here.  This way, when TeamSupport sends emails to your customers, your address will be on the FROM line.</p>
          <asp:CheckBox ID="cbRequireNew" runat="server" CssClass="checkBox" Text="Require [New] keyword for emails" />
          <p>In some cases, our customers do not want tickets created via email from their end users - or potential spam mail creating tickets.  Setting this ON will only create a new ticket from email if we see [new] in the subject line.  The majority of our customers leave this OFF.</p>
          <asp:CheckBox ID="cbRequireKnown" runat="server" CssClass="checkBox" Text="Require a known email address for emails" />
          <p>This setting will only allow emails into the system if we know who the sender is - be it a internal TeamSupport user or anyone else.  As above, the majority of our customers leave this OFF.</p>
          <asp:CheckBox ID="cbChangeStatus" runat="server" CssClass="checkBox" Text="Auto Change Status of a Closed Ticket" />
          <p>When a Ticket is Closed and your Customer updates the ticket via an email, the status of the ticket will change to what you have set as the "email response" status (see <a target="_blank" href="http://help.teamsupport.com/tickets-1/ticket-status">here</a> for more about ticket status).  If you do not want the status of the ticket to change under this scenario, uncheck this setting.</p>
          <asp:CheckBox ID="cbAssociatePeople" runat="server" CssClass="checkBox" Text="Associate additional people to ticket" />
          <p>Automatically associate additional people who are on the To and CC lines of an email to the ticket.</p>
          <asp:CheckBox ID="cbMatchSubject" runat="server" CssClass="checkBox" Text="Match subject to existing tickets." />
          <p>Attempt to match e-mail subject to existing ticket</p>
        </fieldset>
        <div class="buttons ui-helper-hidden" id="divSettingsButtons">
          <button onclick="saveEmailSettings(); return false;">Save</button>
          <button onclick="loadSettings(); return false;">Cancel</button>
        </div>
      </div>
    </div>
      
    <div class="panel">
      <div class="panel-caption">
        <span class="panel-title">Alternate Emails</span>
        <div class="panel-caption-cap"></div>
        <div class="panel-caption-buttons">
            <a class="panel-caption-button" href="#" onclick="showAltEmailDialog();">
              <span class="panel-caption-button-contents">
                <span class="panel-caption-button-icon" style="background-image: url('../images/icons/add.png');"></span>
                <span class="panel-caption-button-text">Add</span>
              </span>
            </a>                  
        </div>  
      </div>
      <div class="panel-body">
        <div id="divAltEmails">
        </div>
<p>The alternate email feature is designed to allow multiple company email address to be forwarded and routed to your TeamSupport account and have the tickets associated with a certain group, product (enterprise and bug tracker editions only) and also what type of ticket to be created (issues, bugs, etc).</p>
<p>If your company has multiple support address that are used by your customers to contact you, you can add those here.  These may be addresses like "level1support@mycompany.com", or "escalation_team@mycompany.com".  Just click the Add icon and fill out the form.  Once you are done, you will see a dropbox email address in the grid above.  Simply forward your company's other support email addresses to these dropbox accounts to enforce the rules you have defined.</p>
      </div>
    </div>      
    
    <div class="panel">
      <div class="panel-caption">
        <span class="panel-title">Email Templates</span><div class="panel-caption-cap">
        </div>
      </div>
      <div class="panel-body" id="divTemplate">
        <fieldset class="ts-fieldset">
          <label for="cmbTemplate" class="text">Select an Email Template</label>
          <telerik:RadComboBox ID="cmbTemplate" runat="server" Width="250px" CssClass="text" OnClientSelectedIndexChanged="cmbTemplate_OnClientSelectedIndexChanged" OnClientSelectedIndexChanging="cmbTemplate_OnClientSelectedIndexChanging" OnClientLoad="cmbTemplate_OnClientLoad">
          </telerik:RadComboBox> &nbsp&nbsp <a class="ts-link" href="#" onclick="resetTemplate(); return false;">Reset to Default</a>
          <p id="divTemplateDesc">This describes the current template</p>
        </fieldset>
        <div style="width: 100; border-bottom: solid 1px #A5BDD4; margin-bottom: 10px;">
        </div>
        <fieldset id="fieldsTemplate" class="ts-fieldset">
          <div id="divIsEmail">
            <div runat="server" id="divSubject">
              <label for="textSubject" class="text">Email Subject</label>
              <telerik:RadTextBox ID="textSubject" runat="server" Width="100%"></telerik:RadTextBox>
              <p>This will be the subject of your email.</p>
            </div>
            
            <asp:CheckBox ID="cbIsHtml" runat="server" CssClass="checkBox" Text="Is the body HTML?" />
            <p>This will allow you to send either HTML or plain text emails.</p>
            <asp:CheckBox ID="cbUseTemplate" runat="server" CssClass="checkBox" Text="Use the global email template." />
            <p>Unchecking this box will allow you to completely customize the look and feel of this email, without using the global email template.</p>
            <div id="divHeaderFooter">
              <label for="textHeader" class="text">Header</label>
              <telerik:RadTextBox ID="textHeader" runat="server" Width="100%"></telerik:RadTextBox>
              <p>This value will replace the {{Header}} place holder in the Global Template.</p>
              <label for="textFooter" class="text">Footer</label>
              <telerik:RadTextBox ID="textFooter" runat="server" Width="100%"></telerik:RadTextBox>
              <p>This value will replace the {{Footer}} place holder in the Global Template.</p>
            </div>
          </div>

          <h3>Body <span id="divModeEdit">Edit | <a class="ts-link" style="font-weight:bold;" href="#" onclick="setPreviewMode(); return false;">Preview</a></span><span id="divModePreview" style="display:none;"><a class="ts-link" style="font-weight:bold;" href="#" onclick="setEditMode(); return false;">Edit</a> | Preview</span></h3>
          <textarea style="width: 99%; height: 300px; resize: none; border: solid 1px #A5BDD4;" id="textBody"></textarea>
          <iframe height="250px" width="100%" scrolling="auto" frameborder="0" id="frameBody" style="border: solid 1px #A5BDD4; display:none;"></iframe>
          <p></p>
        </fieldset>
        <div class="buttons ui-helper-hidden" id="divTemplateButtons">
          <button class="ui-state-highlight" onclick="saveEmailTemplate(); return false;">Save</button>
          <button onclick="loadEmailTemplate(getSelectedTemplateID()); return false;">Cancel</button></div>
          <h3>Available Placeholders</h3>
          <p style="color:#ff0000;">You can re-order place holders, add and remove them as needed.  Do not edit the text inside of them however.  Example: Do not change  {{Action.Description}} to {{Text.Description}}</p>
        <div id="divPlaceHolders">
          <h2 style="border-bottom: solid 1px #a5bdd4;">Available Placeholders</h2>
          <div class="placeholder-title"></div>
          <div class="placeholder-description"></div>
          <div class="placeholder-field"></div>
        </div>
      </div>
    </div>
  </div>
  
  <telerik:RadWindow ID="wndAltEmail" runat="server" Width="360px" Height="350px"
    Animation="None" KeepInScreenBounds="True" VisibleStatusbar="False" VisibleTitlebar="True"
    OnClientPageLoad="" Title="Alternate Email" Behaviors="Close,Move" IconUrl="../images/icons/TeamSupportLogo16.png"
    VisibleOnPageLoad="false" ShowContentDuringLoad="False" Modal="False" DestroyOnClose="True">
    <ContentTemplate>
      <div class="dialog">
        <fieldset class="ts-fieldset">
          <asp:HiddenField ID="fieldEAIID" runat="server" Value="" />
          <label for="textEAIDescription" class="text">Description</label>
          <telerik:RadTextBox ID="textEAIDescription" runat="server" CssClass="text" name="textEAIDescription"
            Width="300px">
          </telerik:RadTextBox>
          <p></p>
          <label for="cmbEAIGroup" class="text">Group to Assign Tickets</label>
          <telerik:RadComboBox ID="cmbEAIGroup" runat="server" Width="300px">
          </telerik:RadComboBox>
          <p></p>
          <label for="cmbEAITicket" class="text">Default Ticket Type</label>
          <telerik:RadComboBox ID="cmbEAITicket" runat="server" Width="300px">
          </telerik:RadComboBox>
          <p></p>
          <label for="cmbEAIProduct" class="text">Product</label>
          <telerik:RadComboBox ID="cmbEAIProduct" runat="server" Width="300px">
          </telerik:RadComboBox>
          <p></p>
        </fieldset>
        
        <div style="float: right;">
          <asp:Button ID="btnOk" runat="server" Text="OK" OnClientClick="saveAltEmail(); $find('wndAltEmail').close(); return false;" />&nbsp
          <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClientClick="$find('wndAltEmail').close(); return false;" />
        </div>
      </div>
      
    </ContentTemplate>
  </telerik:RadWindow>  
  </form>
  
  <script type="text/javascript" language="javascript">
    function deleteAltEmail(id) {
      if (!confirm('Are you sure you would like to delete ' + id + '@teamsupport.com?')) return;
      PageMethods.DeleteAltEmail(id, function() {
        loadAltEmails();
      });
    }
  
    function showAltEmailDialog(id) {
      if (id != null && id.length > 0) {
        PageMethods.GetAltEmail(id, function(result) {
          $get('<%= wndAltEmail.ContentContainer.FindControl("fieldEAIID").ClientID %>').value = result.Email;
          $find('<%= wndAltEmail.ContentContainer.FindControl("textEAIDescription").ClientID %>').set_value(result.Description);
          setDialogCombo('<%= wndAltEmail.ContentContainer.FindControl("cmbEAIGroup").ClientID %>', result.GroupID);
          setDialogCombo('<%= wndAltEmail.ContentContainer.FindControl("cmbEAITicket").ClientID %>', result.TicketTypeID);
          setDialogCombo('<%= wndAltEmail.ContentContainer.FindControl("cmbEAIProduct").ClientID %>', result.ProductID);
          showDialog($find('wndAltEmail'), true, null, 'Edit Alternate Email - ' + id);
        });
      }
      else {
        $get('<%= wndAltEmail.ContentContainer.FindControl("fieldEAIID").ClientID %>').value = '';
        showDialog($find('wndAltEmail'), true, null, 'New Alternate Email');
      }
    }

    function setDialogCombo(name, value) {
      var combo = $find(name);
      var item = combo.findItemByValue(value);
      if (item) item.select();
      else combo.get_items().getItem(0).select();
    }

    function saveAltEmail() {
      var id = $get('<%= wndAltEmail.ContentContainer.FindControl("fieldEAIID").ClientID %>').value;
      PageMethods.SaveAltEmail(id == '' ? null : id,
        $find('<%= wndAltEmail.ContentContainer.FindControl("textEAIDescription").ClientID %>').get_value(),
        $find('<%= wndAltEmail.ContentContainer.FindControl("cmbEAIGroup").ClientID %>').get_value(),
        $find('<%= wndAltEmail.ContentContainer.FindControl("cmbEAITicket").ClientID %>').get_value(),
        $find('<%= wndAltEmail.ContentContainer.FindControl("cmbEAIProduct").ClientID %>').get_value(),
        function(result) {
          loadAltEmails();
        });
    }
  
  </script>
</body>
</html>


