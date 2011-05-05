<%@ Page Title="Thank You" Language="C#" MasterPageFile="~/Standard.master" AutoEventWireup="true" CodeFile="SignUpThanks.aspx.cs" Inherits="SignUpThanks" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <div style="font-size: 30px; padding: 50px 0 50px 0; line-height: 50px;">
    You have successfully signed up.<br/>
    Thank you for taking the time to try out our application.<br/>
    Please continue to sign in.
  </div>
  <div ID="pnlContinue" runat="server" style="margin: 0 auto; text-align:center; width: 100%;">
    <asp:Button ID="btnContinue" runat="server" Text="Continue" onclick="btnContinue_Click"/>
  </div>
  <!-- Google Code for Sign Up Conversion Page -->
<script type="text/javascript">
  var gaJsHost = (("https:" == document.location.protocol) ? "https://ssl." : "http://www.");
  document.write(unescape("%3Cscript src='" + gaJsHost + "google-analytics.com/ga.js' type='text/javascript'%3E%3C/script%3E"));
</script>
<script type="text/javascript">
  try {
    var pageTracker = _gat._getTracker("UA-5124925-1");
    pageTracker._setDomainName(".teamsupport.com");
    pageTracker._setAllowHash(false);
    pageTracker._trackPageview(); 
  } catch (err) { }
  </script>

<!--IndustryBrains Tracking Tag-->

<script type="text/javascript">

  var AccountID = "1132967860";

  var ActionType = "LED";

  var RefUrl = "";

  RefUrl = (RefUrl.length > 2) ? RefUrl.replace(/\s/, '_') : location.href;

  document.write('<img style="visibility:hidden;width:1;height:1;" src="https://braintrack.industrybrains.com/sc/RsJavaScript.aspx?accountID=' + escape(AccountID) + ';actionId=' + escape(ActionType) + ';refUrl=' + escape(RefUrl) + '" />');

</script>

<!--End IndustryBrains Tracking Tag-->

<script type="text/javascript">
  var capterra_vkey = "e862106e3c2641c2067b167355995d3a";
  var capterra_vid = "2056082";
  var capterra_prefix = (("https:" == document.location.protocol) ? "https://ct.capterra.com" : "http://ct.capterra.com"); document.write(unescape("%3Cscript src='" + capterra_prefix + "/capterra_tracker.js?vid=" + capterra_vid + "&vkey=" + capterra_vkey + "' type='text/javascript'%3E%3C/script%3E"));
</script>

  </asp:Content>

