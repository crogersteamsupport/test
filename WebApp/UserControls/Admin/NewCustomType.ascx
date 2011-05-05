<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NewCustomType.ascx.cs" Inherits="UserControls_Admin_NewCustomType" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<div style="width: 300px; padding: 10px 10px 30px 10px;">
  <div style="padding-bottom:5px;">
    <span>Name</span>
    <span><telerik:RadTextBox ID="textName" runat="server" Width="100%" 
          Text='<%# DataBinder.Eval( Container, "DataItem.Name") %>' 
          EmptyMessage="[Enter Name]">
        </telerik:RadTextBox></span>
  </div>
  <div>
    <span>Description</span>
    <span>
      <telerik:RadTextBox ID="textDescription" runat="server" Width="100%" 
          Text='<%# DataBinder.Eval( Container, "DataItem.Description") %>'
          EmptyMessage="[Enter Description]" TextMode="MultiLine" Height="100px">
        </telerik:RadTextBox></span>
  </div>
  
  <div style="padding:10px 0 10px 0;">
     <asp:CheckBox ID="cbClosed" runat="server" Text="Closed: "
      Checked='<%# (DataItem is System.Data.DataRowView) ? (((System.Data.DataRowView)DataItem).Row.Table.Columns.IndexOf("IsClosed")>=0 ? DataBinder.Eval(Container, "DataItem.IsClosed") : false) : false %>'
      Visible='<%# (DataItem is System.Data.DataRowView) ? (((System.Data.DataRowView)DataItem).Row.Table.Columns.IndexOf("IsClosed")>=0) : false %>'
      TextAlign="Left" />

     <asp:CheckBox ID="cbIsTimed" runat="server" Text="Is Action Timed: "
      Checked='<%# (DataItem is System.Data.DataRowView) ? (((System.Data.DataRowView)DataItem).Row.Table.Columns.IndexOf("IsTimed")>=0 ? DataBinder.Eval(Container, "DataItem.IsTimed") : false) : false %>'
      Visible='<%# (DataItem is System.Data.DataRowView) ? (((System.Data.DataRowView)DataItem).Row.Table.Columns.IndexOf("IsTimed")>=0) : false %>'
      TextAlign="Left" />

    <asp:CheckBox ID="cbShipping" runat="server" Text="Shipping: "
      Checked='<%# (DataItem is System.Data.DataRowView) ? (((System.Data.DataRowView)DataItem).Row.Table.Columns.IndexOf("IsShipping")>=0 ? DataBinder.Eval(Container, "DataItem.IsShipping") : false) : false %>'
      Visible='<%# (DataItem is System.Data.DataRowView) ? (((System.Data.DataRowView)DataItem).Row.Table.Columns.IndexOf("IsShipping")>=0) : false %>'
      TextAlign="Left" />&nbsp
      
    <asp:CheckBox ID="cbDiscontinued" runat="server" Text="Discontinued: "
      Checked='<%# (DataItem is System.Data.DataRowView) ? (((System.Data.DataRowView)DataItem).Row.Table.Columns.IndexOf("IsDiscontinued")>=0 ? DataBinder.Eval(Container, "DataItem.IsDiscontinued") : false) : false %>'
      Visible='<%# (DataItem is System.Data.DataRowView) ? (((System.Data.DataRowView)DataItem).Row.Table.Columns.IndexOf("IsDiscontinued")>=0) : false %>'
      TextAlign="Left" />
      
  </div>
  <div style="padding: 5px 0 5px 5px; float:right;">
    <asp:button id="btnUpdate" text="Update" runat="server" CommandName="Update" Visible='<%# !(DataItem is Telerik.Web.UI.GridInsertionObject) %>'></asp:button>
    <asp:button id="btnInsert" text="Insert" runat="server" CommandName="PerformInsert" Visible='<%# (DataItem is Telerik.Web.UI.GridInsertionObject) %>'></asp:button>&nbsp;
    <asp:button id="btnCancel" text="Cancel" runat="server" CausesValidation="False" CommandName="Cancel"></asp:button>
  </div>
</div>
