<%@ Page Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true"
  CodeFile="Search.aspx.cs" Inherits="Frames_Search" Title="Untitled Page" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <telerik:RadAjaxManager ID="ajaxManager" runat="server" UpdatePanelsRenderMode="Inline"></telerik:RadAjaxManager>
  <telerik:RadSplitter ID="RadSplitter1" runat="server" Height="100%" Width="100%"
    BorderSize="0" Orientation="Horizontal">
    <telerik:RadPane ID="RadPane1" runat="server" Height="65px" Scrolling="None" BackColor="#BFDBFF">
      <div style="width: 100%; padding: 5px 0 0 10px;">
        <asp:Panel ID="pnlSearch" runat="server" DefaultButton="btnSearch">
          <table cellpadding="0" cellspacing="5" border="0">
            <tr>
              <td>
                Search:
              </td>
              <td>
                <telerik:RadTextBox ID="textSearch" runat="server" Width="300px"></telerik:RadTextBox>
              </td>
              <td>
                <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" />
              </td>
              <td>
                <asp:CheckBox ID="cbMatchAll" runat="server" Text="Match all of these words" CssClass="" />
              </td>
            </tr>
          </table>
        </asp:Panel>
        <div style="text-align: left; width: 100%;">
          <asp:Label ID="lblResults" class="results" runat="server" Text="" Visible="false" Font-Bold="true" Font-Size="12px"></asp:Label>
        </div>
      </div>
    </telerik:RadPane>
    <telerik:RadPane ID="RadPane3" runat="server" Height="27px" Scrolling="None">
      <telerik:RadTabStrip ID="tsMain" runat="server" AutoPostBack="True" 
        MultiPageID="mpSearch" ontabclick="tsMain_TabClick" SelectedIndex="0">
        <Tabs>
          <telerik:RadTab runat="server" Selected="True" Text="Tickets">
          </telerik:RadTab>
          <telerik:RadTab runat="server" Text="Wiki">
          </telerik:RadTab>
        </Tabs>
      </telerik:RadTabStrip>
    </telerik:RadPane>
    <telerik:RadPane ID="RadPane2" runat="server" Height="100%" Scrolling="None">
      <telerik:RadAjaxLoadingPanel ID="loadSearch" runat="server" Skin="Default" 
        BackColor="#CFE0FA">
      </telerik:RadAjaxLoadingPanel>
      
      <iframe id="ticketsFrame" runat="server" class="contentFrame" scrolling="no" src=""
        frameborder="0" height="100%" width="100%" style="background-color: #FFFFFF;">
      </iframe>
      
      
    </telerik:RadPane>
  </telerik:RadSplitter>
  
  <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">
  
  <script type="text/javascript" language="javascript">

    function setTicketCount(count) {
      $('.results').html(count + ' Tickets Found');
    
    }
  </script>
  </telerik:RadScriptBlock>
</asp:Content>
