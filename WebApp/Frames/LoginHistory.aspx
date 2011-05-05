<%@ Page Title="" Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true" CodeFile="LoginHistory.aspx.cs" Inherits="Frames_LoginHistory" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<telerik:RadGrid ID="gridUsers" runat="server" AllowPaging="True" AllowSorting="True" 
  AutoGenerateColumns="False" GridLines="None"  ShowGroupPanel="True" onneeddatasource="gridUsers_NeedDataSource" Height="100%" Width="100%">
<MasterTableView TableLayout="Fixed">
  <Columns>
    <telerik:GridBoundColumn DataField="UserName" HeaderText="UserName" UniqueName="UserName"></telerik:GridBoundColumn>
    <telerik:GridBoundColumn DataField="Company" HeaderText="Company" UniqueName="Company"></telerik:GridBoundColumn>
    <telerik:GridBoundColumn DataField="Browser" HeaderText="Browser" UniqueName="Browser"></telerik:GridBoundColumn>
    <telerik:GridBoundColumn DataField="Version" HeaderText="Version" UniqueName="Version"></telerik:GridBoundColumn>
    <telerik:GridBoundColumn DataField="DateCreated" HeaderText="Date" UniqueName="DateCreated"></telerik:GridBoundColumn>
  </Columns>
</MasterTableView>

  <ClientSettings AllowDragToGroup="True">
    <Scrolling AllowScroll="True" UseStaticHeaders="True" />
  </ClientSettings>

</telerik:RadGrid>
</asp:Content>

