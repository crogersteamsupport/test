<%@ Page Title="" Language="C#" MasterPageFile="~/FrameContainer.master" AutoEventWireup="true"
  CodeFile="Wikis.aspx.cs" Inherits="Frames_Wikis" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

  <script type="text/javascript" language="javascript">
    function OpenWiki(id) {
      top.Ts.MainPage.openWiki(id);
    }
  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <telerik:RadGrid ID="gridWikis" runat="server" AllowPaging="True" AllowSorting="True"
    AutoGenerateColumns="False" BorderWidth="0px" GridLines="None" Height="100%" PageSize="20"
    Width="100%" AllowCustomPaging="False" OnItemDataBound="gridWikis_ItemDataBound"
    OnNeedDataSource="gridWikis_NeedDataSource">
    <PagerStyle AlwaysVisible="true" Mode="NextPrevAndNumeric" />
    <ExportSettings ExportOnlyData="True" IgnorePaging="True" FileName="Tickets" Excel-Format="ExcelML">
      <Excel Format="ExcelML" />
    </ExportSettings>
    <GroupingSettings CaseSensitive="false" />
    <MasterTableView ClientDataKeyNames="ArticleID" DataKeyNames="ArticleID" TableLayout="Fixed"
      AllowCustomSorting="True" AutoGenerateColumns="false" CellSpacing="-1">
      <Columns>
        <telerik:GridButtonColumn ButtonType="ImageButton" ImageUrl="../images/icons/open.png"
          UniqueName="ButtonOpen" CommandName="ShowWiki">
          <HeaderStyle Width="32px" />
        </telerik:GridButtonColumn>
        <telerik:GridBoundColumn DataField="ArticleID" HeaderText="ArticleID" UniqueName="ArticleID" Visible="false"></telerik:GridBoundColumn>
        <telerik:GridBoundColumn DataField="ArticleName" HeaderText="Article Name" UniqueName="ArticleName"></telerik:GridBoundColumn>
        <telerik:GridBoundColumn DataField="ModifiedDate" HeaderText="Last Modified On" UniqueName="ModifiedDate"></telerik:GridBoundColumn>
        <telerik:GridBoundColumn DataField="Modifier" HeaderText="Last Modified By" UniqueName="Modifier"></telerik:GridBoundColumn>
        
        
      </Columns>
    </MasterTableView>
    <ClientSettings AllowColumnsReorder="False">
      <Selecting AllowRowSelect="True" />
      <ClientEvents />
      <Scrolling AllowScroll="True" UseStaticHeaders="True" EnableVirtualScrollPaging="false"
        SaveScrollPosition="true" />
    </ClientSettings>
  </telerik:RadGrid>
</asp:Content>
