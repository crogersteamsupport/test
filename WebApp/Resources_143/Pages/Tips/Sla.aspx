<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Sla.aspx.cs" Inherits="Resources_143_Pages_Tips_Sla" EnableViewState="false"  %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title></title>
</head>
<body>
  <div>
    <h1><span id="tipSla" runat="server" style="float:left; line-height:16px; margin-right: 3px;"></span><a id="tipNumber" runat="server" href="#" style="float:left; line-height:16px;"></a></h1>
    <div class="ui-helper-clearfix"></div>
    <h2 id="tipName" runat="server" style="padding-top:.5em;"></h2>
    <div class="ui-widget-content ts-separator"></div>

    <table cellspacing="0" cellpadding="0" border="0" class="tip-sla-props">
      <thead>
        <tr class="ts-color-bg-accent">
          <th>SLA Type</th>
          <th>Warning</th>
          <th>Violation</th>
        </tr>
      </thead>
      <tbody>
        <tr>
          <td>Last Action</td>
          <td id="wLast" runat="server"></td>
          <td id="vLast" runat="server"></td>
        </tr>
        <tr>
          <td>Time To Close</td>
          <td id="wClose" runat="server"></td>
          <td id="vClose" runat="server"></td>
        </tr>
        <tr>
          <td>Initial Response</td>
          <td id="wInit" runat="server"></td>
          <td id="vInit" runat="server"></td>
        </tr>
      </tbody>
      <tfoot style="font-weight:bold;">
        <tr>
          <td>Soonest</td>
          <td id="wNext" runat="server"></td>
          <td id="vNext" runat="server"></td>
        </tr>
      </tfoot>
    </table>
    <dl id="tipProps" runat="server"></dl>
    <div class="ui-helper-clearfix"></div>
  </div>
</body>
</html>
