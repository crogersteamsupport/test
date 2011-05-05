<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TicketQueue.aspx.cs" Inherits="Frames_TicketQueue" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>Ticket Queue</title>
  <link href="../css_5/ui.css" rel="stylesheet" type="text/css" />
  <link href="../css_5/jquery-ui-latest.custom.css" rel="stylesheet" type="text/css" />
  <link href="../css_5/frame.css" rel="stylesheet" type="text/css" />
  <!--[if IE 7]><link href="../css_5/ui-ie7.css" rel="stylesheet" type="text/css" /><![endif]--><!--[if IE 8]><link href="../css_5/ui-ie8.css" rel="stylesheet" type="text/css" /><![endif]-->
  <script src="../js_5/jquery-1.4.2.min.js" type="text/javascript"></script>
  <script src="../js_5/jquery-ui-latest.custom.min.js" type="text/javascript"></script>

  <style type="text/css">
    body { background: #fff;}
    #sortable { list-style-type: none; margin: 0; padding: 0; width: 100%; }
    #list { min-width: 700px; width: 100%; padding-bottom: 20px; }
    #list-wrapper { height:100%; overflow:auto; }
    .q-item { border-bottom: solid 1px #AAC3E3; cursor: url(../cursors/move.cur), pointer; height: 17px; font-size: 12px; padding: 5px 5px; margin:0; line-height:17px; }
    li.ui-sortable-helper {cursor: url(../cursors/grab.cur), move !important;}
    li.q-item-notme { background: #f5f5f5; color: #888; border-bottom: solid 1px #ccc;}
    li.q-item-closed { text-decoration: line-through;}
    li.q-item-hover { background: #E3EFFF !important; }
    li.q-item-grab { cursor: url(../cursors/grab.cur), move; }
    li.q-item-header { cursor: default; background: #DBE6F4; border-bottom:solid 1px #8DB2E3; height: 22px; line-height:22px; font-size: 12px;}
    .q-item-place { border-bottom: solid 1px #eee; background:#fff0a5;  height: 17px; line-height:17px; padding: 5px 5px; }
    .q-item-icon { float:left; top: 0px; margin-right: 7px; width: 16px; position: relative; }
    .q-item div { float:left; overflow:hidden; white-space:nowrap;}
    .q-item-text { display:block; float:left; width: 97%; }
    .q-item-icons { width: 10%;}
    .q-item-name { width: 40%;}
    .q-item-user { width: 15%;}
    .q-item-severity { width: 15%;}
    .q-item-status { width: 15%;}
    .q-item-icons a.ts-link { font-weight:bold;}
  </style>

  <script language="javascript" type="text/javascript">

    $(function() {
      $('#sortable').sortable({ placeholder: 'q-item-place', items: 'li:not(.q-item-header)', stop: saveItems }).disableSelection(); ;
      loadItems();
    });

    function onShow() { loadItems(); }

    function getQueryParamValue(name, location) {
      params = location.search.substring(1);
      name = name.toLowerCase();
      param = params.split("&");
      for (i = 0; i < param.length; i++) {
        value = param[i].split("=");
        if (value[0].toLowerCase() == name) {
          return this.unescape(value[1]);
        }
      }
    }
    function getUserID() {
      return getQueryParamValue('userid', window.location);
    }

    function loadItems() {
      PageMethods.GetQueueItems(getUserID(), function(result) {
        var html = '<li class="q-item q-item-header"><div class="q-item-icons"><a href="#" class="ts-link enqueue">Add Ticket</a></div><div class="q-item-name">Ticket Name</div><div class="q-item-user">Assigned To</div><div class="q-item-status">Status</div><div class="q-item-severity">Severity</div</li>';
        for (var i = 0; i < result.length; i++) { html = html + getItemHtml(result[i].ID, result[i].Name, result[i].User, result[i].Status, result[i].Severity, result[i].UserID, result[i].IsClosed); }
        $('#sortable').html(html);
        $('.q-item').hover(function () { $('.q-item').removeClass('q-item-hover'); $(this).addClass('q-item-hover'); }, function () { $('.q-item').removeClass('q-item-hover'); });
        $('.q-item .ts-icon-top').click(function() { $(this).parent().parent().detach().insertAfter('.q-item-header'); saveItems(); });
        $('.q-item .ts-icon-close').click(function() { $(this).parent().parent().remove(); PageMethods.Dequeue(getID($(this).parent().parent()[0].id)); });
        $('.q-item .ts-icon-open').click(function() { PageMethods.GetTicketNumber(getID($(this).parent().parent()[0].id), function(result) { top.Ts.MainPage.openTicket(result); }); });
        $('.enqueue').click(function(event) { event.preventDefault(); addTicket(); });
      });
    }

    function addTicket() {
      top.ShowTicketDialog(true, function(ticketID) {
        PageMethods.Enqueue(ticketID, getUserID(), function() { loadItems(); });
      });
    }

    function saveItems() {
      var result = new Array();
      $('.q-item').each(function(index, element) {
        var id = getID(element.id);
        if (id != null) { result[result.length] = id; }
      });
      PageMethods.SaveItems(getUserID(), result);
    }

    function getID(elementID) {
      if (elementID != null && elementID != '') {
        return elementID.substr(4, elementID.length - 4);
      }
      return null;
    }

    function getItemHtml(id, name, user, status, severity, userID, isClosed) {
      var itemClass = userID == getUserID() ? '' : ' q-item-notme';
      if (isClosed) itemClass = itemClass + ' q-item-closed';
      return '<li id="item' + id + '" class="q-item'+itemClass+'"><div class="q-item-icons"> <span class="q-item-icon ts-icon ts-icon-top"></span><span class="q-item-icon ts-icon ts-icon-open"></span><span class="q-item-icon ts-icon ts-icon-close"></span></div>' +
        '<div class="q-item-name"><div class="q-item-text">' + name + '</div></div>' +
        '<div class="q-item-user"><div class="q-item-text">' + user + '</div></div>' +
        '<div class="q-item-status"><div class="q-item-text">' + status + '</div></div>' +
        '<div class="q-item-severity"><div class="q-item-text">' + severity + '</div></div>' +
        '</li>';
    }
  
  </script>

</head>
<body>
  <form id="form1" runat="server">
  <telerik:RadScriptManager ID="RadScriptManager1" runat="server" EnablePageMethods="true">
  </telerik:RadScriptManager>
  <div id="list-wrapper">
  <div id="list">
    <ul id="sortable">
      <li class="q-item q-item-header">
      <div class="q-item-icons"><a href="#" class="ts-link">Add Ticket</a></div>
      <div class="q-item-name">Ticket Name</div>
      <div class="q-item-user">Assigned To</div>
      <div class="q-item-severity">Severity</div>
      </li>
    </ul>
  </div>
  </div>
  </form>
</body>
</html>
