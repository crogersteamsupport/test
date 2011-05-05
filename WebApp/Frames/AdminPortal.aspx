<%@ Page Title="" Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true"
  CodeFile="AdminPortal.aspx.cs" Inherits="Frames_AdminPortal" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" UpdatePanelsRenderMode="Inline">
    <AjaxSettings>
      <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
        <UpdatedControls>
          <telerik:AjaxUpdatedControl ControlID="pnlContent" />
        </UpdatedControls>
      </telerik:AjaxSetting>
    </AjaxSettings>
  </telerik:RadAjaxManager>
  <div style="height:100%; width: 100%; background-color: #ffffff; overflow:auto; margin:0;">
    <div style="padding: 20px 20px;">
    <div><asp:Label ID="lblLicense" runat="server" Text="" Style="font-weight: bold; font-size: 14px;"></asp:Label></div>
    <br />
    <br />
    <div id="pnlContent" runat="server">
      <table cellpadding="0" cellspacing="15" border="0">
        <asp:Literal ID="litDetails" runat="server"></asp:Literal>
      </table>
    </div>
    <br />
    <asp:Button ID="btnEdit" runat="server" Text="Edit Portal Options" />
    
    </div>
  </div>
  <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">

    <script type="text/javascript">

      function DialogClosed(sender, args) {
        SendAjaxRequest();
        sender.remove_close(DialogClosed);
      }
      function ShowDialog(wnd) {
        wnd.add_close(DialogClosed);
        wnd.show();
      }
      function SendAjaxRequest() {
        var ajaxManager = $find("<%=RadAjaxManager.GetCurrent(Page).ClientID  %>");
        ajaxManager.ajaxRequest();
        return false;
      }
      
    
    </script>

  </telerik:RadScriptBlock>
</asp:Content>
