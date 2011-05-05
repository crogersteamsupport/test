<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Watercooler.aspx.cs" Inherits="Watercooler_Watercooler" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

  <script src="../js_5/jquery-1.4.2.min.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
      function pageLoad() {
        setInterval('Update();', 400);
      }

      function Update() {
        PageMethods.Update(function(result) {
          $('#sapndes').text(result);
        });
      }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
      <telerik:RadScriptManager ID="RadScriptManager1" runat="server" EnablePageMethods="true">
      </telerik:RadScriptManager>
      <div>welcom to the wc</div>
      <span id="sapndes"></span>
    </div>
    </form>
</body>
</html>
