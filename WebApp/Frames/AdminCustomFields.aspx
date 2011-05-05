<%@ Page Title="" Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true" CodeFile="AdminCustomFields.aspx.cs" Inherits="Frames_AdminCustomFields" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
  <style type="text/css">
    body
    {
      background: #fff;
      overflow:auto;
    }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" UpdatePanelsRenderMode="Inline">
    <AjaxSettings>
      <telerik:AjaxSetting AjaxControlID="pnlMain">
        <UpdatedControls>
          <telerik:AjaxUpdatedControl ControlID="pnlMain" />
        </UpdatedControls>
      </telerik:AjaxSetting>
    </AjaxSettings>
  </telerik:RadAjaxManager>

<telerik:RadAjaxLoadingPanel ID="lpType" runat="server" Height="15px" Width="75px" IsSticky="true">
  <img alt="Loading..." src='<%= RadAjaxLoadingPanel.GetWebResourceUrl(Page, "Telerik.Web.UI.Skins.Default.Ajax.loading3.gif") %>' style="border: 0px; margin-left: 10px;" />
</telerik:RadAjaxLoadingPanel>
<div id="pnlMain" runat="server" style="overflow:auto; height:100%;">
  <div >
  <div style="padding: 10px 0 0 17px;">
    <div style="float: left; padding-right: 20px;">
      <div style="padding-bottom: 5px;">
        Field Type:</div>
      <div>
        <telerik:RadComboBox ID="cmbFieldTypes" runat="server" AutoPostBack="True" 
          onselectedindexchanged="cmbFieldTypes_SelectedIndexChanged">
          <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
        </telerik:RadComboBox>
      </div>
    </div>
    <div id="pnlTicket" runat="server" style="float: left; padding-right: 20px;">
      <div style="padding-bottom: 5px;">
        Ticket Type:</div>
      <div>
        <telerik:RadComboBox ID="cmbTicketTypes" runat="server" AutoPostBack="True" 
          onselectedindexchanged="cmbTicketTypes_SelectedIndexChanged">
          <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
        </telerik:RadComboBox>
      </div>
    </div>
     
    <div style="clear: both; padding-bottom: 10px;"> </div>
    <div class="groupDiv groupLightBlue">
      <div class="groupHeaderDiv">
        <span class="groupHeaderSpan"></span>
        <span class="groupCaptionSpan">Custom Fields</span><span class="groupButtonSpanWrapper"><span class="groupButtonsSpan">
          <asp:LinkButton ID="lnkAddStatus" runat="server" CssClass="groupButtonLink">
              <span class="groupButtonSpan">
                <img alt="" src="../images/icons/add.png" class="groupButtonImage" />
                <span class="groupButtonTextSpan">Add Custom Field</span> </span></asp:LinkButton>
        </span>
        </span>
      </div>
      <div class="groupBodyWrapperDiv">
        <div class="groupBodyDiv">
          <telerik:RadGrid ID="gridProperties" runat="server" Width="100%" 
            AutoGenerateColumns="False" GridLines="None" 
            OnItemCommand="gridProperties_ItemCommand" BorderWidth="0px" 
            onitemdatabound="gridProperties_ItemDataBound" 
            onneeddatasource="gridProperties_NeedDataSource">
            <MasterTableView DataKeyNames="CustomFieldID" 
              ClientDataKeyNames="CustomFieldID" CellSpacing="-1">
              <RowIndicatorColumn>
                <HeaderStyle Width="20px"></HeaderStyle>
              </RowIndicatorColumn>
              <ExpandCollapseColumn>
                <HeaderStyle Width="20px"></HeaderStyle>
              </ExpandCollapseColumn>
              <Columns>
                <telerik:GridButtonColumn ButtonType="ImageButton" CommandName="ButtonEdit" ImageUrl="../images/icons/edit.png" UniqueName="ButtonEdit">
                  <ItemStyle Width="16px" />
                </telerik:GridButtonColumn>
                <telerik:GridButtonColumn ButtonType="ImageButton" ImageUrl="../images/icons/trash.png" UniqueName="columnDelete" CommandName="Delete" ConfirmText="WARNING! All data will be lost! Are you sure you would like to delete this field?" ConfirmDialogType="RadWindow">
                  <ItemStyle Width="16px" />
                </telerik:GridButtonColumn>
                <telerik:GridButtonColumn ButtonType="ImageButton" ImageUrl="../images/icons/Arrow_Up.png" UniqueName="columnMoveUp" CommandName="MoveUp">
                  <ItemStyle Width="16px" />
                </telerik:GridButtonColumn>
                <telerik:GridButtonColumn ButtonType="ImageButton" ImageUrl="../images/icons/Arrow_Down.png" UniqueName="columnMoveDown" CommandName="MoveDown">
                  <ItemStyle Width="16px" />
                </telerik:GridButtonColumn>
                <telerik:GridBoundColumn DataField="CustomFieldID" UniqueName="CustomFieldID" Visible="False"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Position" UniqueName="Position" Visible="False"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Name" HeaderText="Name" UniqueName="Name"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="ApiFieldName" HeaderText="ApiFieldName" UniqueName="ApiFieldName"></telerik:GridBoundColumn>
                <telerik:GridTemplateColumn DataField="FieldTypeDisplay" HeaderText="Field Type" UniqueName="FieldTypeDisplay">
                  <ItemTemplate>
                    <asp:Label ID="lblFieldType" runat="server" Text="lblFieldType"></asp:Label>
                  </ItemTemplate>
                
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="FieldType" EmptyDataText="&amp;nbsp;" UniqueName="FieldType" Visible="False"></telerik:GridBoundColumn>
                <telerik:GridCheckBoxColumn DataField="IsVisibleOnPortal" 
                  DataType="System.Boolean" HeaderText="Visible On Portal" 
                  UniqueName="IsVisibleOnPortal"></telerik:GridCheckBoxColumn>
                <telerik:GridCheckBoxColumn DataField="IsRequired" 
                  DataType="System.Boolean" HeaderText="Is Required" 
                  UniqueName="IsRequired"></telerik:GridCheckBoxColumn>
              </Columns>
            </MasterTableView>
            <FilterMenu EnableTheming="True">
              <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
            </FilterMenu>
          </telerik:RadGrid>
        </div>
      </div>
    </div>
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
        var grid = $find("<%=gridProperties.ClientID %>").get_masterTableView();
        grid.rebind();
      }
      

      function EditRow(id) {
        ShowDialog(top.GetCustomFieldDialog(id, null, null));
      }
   
    
    </script>

  </telerik:RadScriptBlock>
</asp:Content>

