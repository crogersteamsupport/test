<%@ Page Title="" Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true" CodeFile="AdminAccount.aspx.cs" Inherits="Frames_AdminAccount" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div style="height:100%; width:100%; overflow-x:hidden; overflow-y:auto; background-color:#ffffff;">
    <div id="pnlInfo" runat="server" style="padding:20px 20px; height:100%; width:100%;">
      <h2>License Information</h2>
      <div style="padding: 10px 0 10px 0; font-weight:bold;">
        To change your license configuration, please contact our sales department at 800.596.2820 x806 or send an email to <a href="mailto:sales@teamsupport.com">sales@teamsupport.com</a>.
      </div>
      <div>
        <table cellpadding="0" cellspacing="5" border="0" style="font-size: 14px">
          <asp:Literal ID="litAccount" runat="server"></asp:Literal>
        </table>
        <asp:Button ID="btnInfo" runat="server" Text="Change Account Info" Visible="false"/>
      </div>
      <br />
      <h2>Billing Address</h2>
      <br />
      <div>
        <table cellpadding="0" cellspacing="0" border="0">
          <asp:Literal ID="litAddress" runat="server"></asp:Literal>
        </table>
        <br />
        <asp:Button ID="btnAddress" runat="server" Text="Change Address" />
        <br />
      </div>
      <br />
      <div style="display:none;">
      <h2>Credit Card Information</h2>
      <br />
      <p>
        <table cellpadding="0" cellspacing="0" border="0">
          <asp:Literal ID="litCredit" runat="server"></asp:Literal>
        </table>
        <br />
        <asp:Button ID="btnCredit" runat="server" Text="Change Credit Card" />
        <br />
      </p>
      <br />
      </div>
    </div>
</div>
  <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">

    <script type="text/javascript" language="javascript">
      function DialogClosed(sender, args) {
        sender.remove_close(DialogClosed);
        RefreshContent();
      }

      function ShowDialog(wnd) {
        wnd.add_close(DialogClosed);
        wnd.show();
      }

      function RefreshContent() {
        __doPostBack();
      }
    
    
    </script>

  </telerik:RadScriptBlock>
</asp:Content>

