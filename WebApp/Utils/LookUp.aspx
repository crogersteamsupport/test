<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LookUp.aspx.cs" Inherits="Utils_LookUp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
  <link href="../vcr/141/Css/jquery-ui-latest.custom.css" rel="stylesheet" type="text/css" />
  <link href="../vcr/141/Css/jquery-ui-enhanced.css" rel="stylesheet" type="text/css" />
  <link href="../vcr/141/Css/ts.ui.css" rel="stylesheet" type="text/css" />
  <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.5.1/jquery.min.js" type="text/javascript"></script>

  <script src="https://ajax.googleapis.com/ajax/libs/jqueryui/1.8.14/jquery-ui.min.js" type="text/javascript"></script>

    <script language="javascript" type="text/javascript">

      $(document).ready(function () {
        $('button').button();

        var execGetOrganization = null;
        function getOrganizations(request, response) {
          if (execGetOrganization) { execGetOrganization._executor.abort(); }
          execGetOrganization = PageMethods.GetOrganizations(request.term, function (result) { response(result); });
        }

        $('#textOrgID').autocomplete({
          minLength: 2,
          source: getOrganizations,
          select: function (event, ui) {
            $('#spanOrganizationID').text('Organization ID = ' + ui.item.id);
            $(this).data('itemID', ui.item.id);
          }

        });
      });


    
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>
    <div style="font-size: 12px; border:none; padding: 2em 2em;" class="ui-widget ui-widget-content">
    Organization Name: <input class="text ui-corner-all ui-widget-content" type="text" id="textOrgID" style="width: 250px;" />  <span id="spanOrganizationID"></span>
    </div>
    </form>
</body>
</html>
