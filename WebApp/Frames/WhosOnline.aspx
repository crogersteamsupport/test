<%@ Page Title="" Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true" CodeFile="WhosOnline.aspx.cs" Inherits="Frames_WhosOnline" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<telerik:RadGrid ID="gridUsers" runat="server" AllowPaging="True" AllowSorting="True" Height="100%" Width="100%"
AutoGenerateColumns="False" GridLines="None" onneeddatasource="gridUsers_NeedDataSource" ShowGroupPanel="True">
<HeaderContextMenu EnableTheming="True">
<CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
</HeaderContextMenu>

<MasterTableView>
<RowIndicatorColumn>
<HeaderStyle Width="20px"></HeaderStyle>
</RowIndicatorColumn>

<ExpandCollapseColumn>
<HeaderStyle Width="20px"></HeaderStyle>
</ExpandCollapseColumn>
  <Columns>
    <telerik:GridBoundColumn DataField="IdleName" HeaderText="Name" UniqueName="IdleName"></telerik:GridBoundColumn>
    <telerik:GridBoundColumn DataField="OrganizationName" HeaderText="OranizationName" UniqueName="OranizationName"></telerik:GridBoundColumn>
    <telerik:GridBoundColumn DataField="LoginDate" HeaderText="LoginDate" UniqueName="LoginDate"></telerik:GridBoundColumn>
    <telerik:GridBoundColumn DataField="Browser" HeaderText="Browser" UniqueName="Browser"></telerik:GridBoundColumn>
    <telerik:GridBoundColumn DataField="Version" HeaderText="Version" UniqueName="Version"></telerik:GridBoundColumn>
  </Columns>
</MasterTableView>

  <ClientSettings AllowDragToGroup="True">
    <Scrolling AllowScroll="True" UseStaticHeaders="True" />
  </ClientSettings>

<FilterMenu EnableTheming="True">
<CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
</FilterMenu>
</telerik:RadGrid></asp:Content>

