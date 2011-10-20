<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ActionCleaner.aspx.cs" Inherits="Utils_ActionCleaner" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
  <link href="../Resources_148/Css/jquery-ui-latest.custom.css" rel="stylesheet" type="text/css" />
  <link href="../Resources_148/Css/jquery-ui-enhanced.css" rel="stylesheet" type="text/css" />
  <link href="../Resources_148/Css/ts.ui.css" rel="stylesheet" type="text/css" />
  <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.5.1/jquery.min.js" type="text/javascript"></script>

  <script src="https://ajax.googleapis.com/ajax/libs/jqueryui/1.8.14/jquery-ui.min.js" type="text/javascript"></script>

  <script language="javascript" type="text/javascript">

    $(document).ready(function () {
      var _ticket = null;
      $('button').button();
      $('#btnGetTicketByID').click(function (e) {
        e.preventDefault();
        PageMethods.GetTicketByID($('#textTicketID').val(), loadTicket);
      });

      $('#btnGetTicketByNumber').click(function (e) {
        e.preventDefault();
        PageMethods.GetTicketByNumber($('#textOrgID').data('itemID'), $('#textTicketNumber').val(), loadTicket);
      });

      $('#linkCleanEmail').click(function (e) {
        PageMethods.CleanAllEmailActions(_ticket.TicketID, function () { alert('Email actions cleaned.'); });

      });

      $('#linkCleanAll').click(function (e) {
        PageMethods.CleanAllActions(_ticket.TicketID, function () { alert('All actions cleaned.'); });
      });

      var execGetOrganization = null;
      function getOrganizations(request, response) {
        if (execGetOrganization) { execGetOrganization._executor.abort(); }
        execGetOrganization = PageMethods.GetOrganizations(request.term, function (result) { response(result); });
      }

      $('#textOrgID').autocomplete({
        minLength: 2,
        source: getOrganizations,
        select: function (event, ui) {
          $(this).data('itemID', ui.item.id);
        }

      });


      function loadTicket(ticket) {
        _ticket = ticket;
        if (!ticket) {
          $('#divTicket').hide();
          alert('Unable to find the ticket.');
          return;

        }

        $('#divTicket').show();
        $('#divTicket h1').text('Ticket ' + _ticket.TicketNumber);
        $('#divTicket h3').text(_ticket.Name);

        PageMethods.GetActions(ticket.TicketID, function (actions) {

          var html = '<table>'
          for (var i = 0; i < actions.length; i++) {
            html = html + '<tr>'
            html = html + '<td><a href="#" onclick="PageMethods.CleanAction(' + actions[i].ActionID + ', function(){ alert(\'Action cleaned.\');}); return false;">Clean</a></td>';
            html = html + '<td>' + actions[i].ActionID + '</td>';
            html = html + '<td>' + actions[i].ActionType + '</td>';
            html = html + '<td>' + actions[i].Name + '</td>';
            html = html + '</tr>'
          }


          $('#divActions').html(html);

        });
      }

    });
  </script>

</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager runat="server" EnablePageMethods="true"></asp:ScriptManager>
    <div style="font-size: 12px; border:none; padding: 2em 2em;" class="ui-widget ui-widget-content">
    <div>
    Ticket ID: <input class="text  ui-corner-all ui-widget-content" type="text" id="textTicketID" /> <button id="btnGetTicketByID">Get Ticket</button>
    </div>
    <h3> -- OR -- </h3>
    <div>
    Organization: <input class="text ui-corner-all ui-widget-content" type="text" id="textOrgID" style="width: 250px;" />  Ticket #: <input class="text ui-corner-all ui-widget-content" type="text" id="textTicketNumber" /> <button id="btnGetTicketByNumber">Get Ticket</button>
    </div>
    <div id="divTicket" class="ui-helper-hidden">
      <h1>Ticket #</h1>
      <h3>This is a ticket</h3>
      <div>
        <a href="#" id="linkCleanEmail">Clean All Email Actions</a> | <a href="#" id="linkCleanAll">Clean All Actions</a>
      </div>
      <h2>Actions</h2>
      <div id="divActions">
    
      </div>
    </div>
    </div>
    </form>
</body>
</html>
