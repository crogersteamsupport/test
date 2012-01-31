<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RegExTest.aspx.cs" Inherits="RegExTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <script src="js_5/jquery-1.4.2.min.js" type="text/javascript"></script>
  <script src="vcr/141/Js/jquery-ui-1.8.14.custom.min.js" type="text/javascript"></script>
  <link href="css_5/jquery-ui-latest.custom.css" rel="stylesheet" type="text/css" />
  <link href="css_5/ui.css" rel="stylesheet" type="text/css" />
  <title>RegEx Tester</title>
  
  <style type="text/css">
    #results { padding-top: 10px; font-weight:bold; line-height: 20px;}
    h2 { line-height: normal; font-size: 16px; margin: 0; margin-top: 10px;}
  </style>
  
<script type="text/javascript" language="javascript">

  $(document).ready(function () {
    $('button').button();

    $('#btnExecute').click(function (e) { e.preventDefault(); calc(); });
    $('#btnTest').click(function (e) {
      e.preventDefault();
      PageMethods.Test($('#textPattern').val(), $('#textRep').val(), $('#textInput').val(),
        function (result) { $('#results').html(result); }
        );

    });

    $('#textPattern').val('((www\.|(http|https|ftp|news|file)+\:\/\/)[&#95;.a-z0-9-]+\.[a-z0-9\/&#95;:@=.+?,##%&~-]*[^.|\'|\# |!|\(|?|,| |>|<|;|\)])');
    $('#textRep').val('<a href="$1">$1</a>');
    $('#textInput').val('https://app.teamsupport.com/ticket.aspx?ticketid=1235');
  });

  function calc() {
    PageMethods.GetRegEx($('#textPattern').val(), $('#textRep').val(), $('#textInput').val(), 
      function(result) {
        var ex = new RegExp($('#textPattern').val(), 'gim');
        var str = $('#textInput').val().replace(ex, $('#textRep').val());
        $('#results').html('<h2>.NET</h2><div>' + result + '</div><h2>JavaScript</h2><div>' + str + '</div>');
      }
    );
    }

  
</script>
</head>
<body>
  <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true">
    </asp:ScriptManager>
    <h2>Pattern:</h2>
    <textarea id="textPattern" rows="3" cols="75"></textarea>
    <h2>Replacement:</h2>
    <textarea id="textRep" rows="2" cols="75"></textarea>
    <h2>Input:</h2>
    <textarea id="textInput" rows="5" cols="75"></textarea>
    <br />
    <br />
    <button id="btnExecute">Execute</button> <button id="btnTest">Test</button>
    <div id="results"></div>
    CheatSheets: <a href="images/regex.png" target="_blank">1</a> | <a href="http://regexlib.com/CheatSheet.aspx" target="_blank">2</a> | <a href="http://nregex.com" target="_blank">.NET Tester</a> | <a href="http://regexpal.com" target="_blank">JS Tester</a>
    
    </form>
</body>
</html>
