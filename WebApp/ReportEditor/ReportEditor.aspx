<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReportEditor.aspx.cs" Inherits="ReportEditor_ReportEditor" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="../UserControls/ReportFilterControl.ascx" TagName="ReportFilterControl"
  TagPrefix="uc1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>Report Editor</title>
  <link href="../css_5/ui.css" rel="stylesheet" type="text/css" />
  <style type="text/css">
    #QueryPanel1
    {
      background-image: none !important;
      background-color: White !important;
    }
  </style>
</head>
<body>
  <form id="form1" runat="server">
  <telerik:RadScriptManager ID="RadScriptManager1" runat="server" EnablePageMethods="true" ScriptMode="Release" OutputCompression="Forced">
  </telerik:RadScriptManager>
  <telerik:RadFormDecorator ID="RadFormDecorator1" runat="server" DecoratedControls="All" />
  <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" UpdatePanelsRenderMode="Inline">
    <AjaxSettings>
      <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
        <UpdatedControls>
          <telerik:AjaxUpdatedControl ControlID="mpMain" />
        </UpdatedControls>
      </telerik:AjaxSetting>
      <telerik:AjaxSetting AjaxControlID="divFilter">
        <UpdatedControls>
          <telerik:AjaxUpdatedControl ControlID="divFilter" />
        </UpdatedControls>
      </telerik:AjaxSetting>
      <telerik:AjaxSetting AjaxControlID="btnBack">
        <UpdatedControls>
          <telerik:AjaxUpdatedControl ControlID="mpMain" />
          <telerik:AjaxUpdatedControl ControlID="btnBack" />
          <telerik:AjaxUpdatedControl ControlID="btnNext" />
        </UpdatedControls>
      </telerik:AjaxSetting>
      <telerik:AjaxSetting AjaxControlID="btnNext">
        <UpdatedControls>
          <telerik:AjaxUpdatedControl ControlID="mpMain" />
          <telerik:AjaxUpdatedControl ControlID="btnBack" />
          <telerik:AjaxUpdatedControl ControlID="btnNext" />
        </UpdatedControls>
      </telerik:AjaxSetting>
    </AjaxSettings>
  </telerik:RadAjaxManager>
  <div style="height: 320px; width: 460px;">
    <telerik:RadMultiPage ID="mpMain" runat="server" SelectedIndex="0" Height="320px">
      <telerik:RadPageView ID="pvProperties" runat="server">
        <div style="padding: 10px 0 0 10px;">
          <div>
            Report Name</div>
          <div>
            <telerik:RadTextBox ID="textName" runat="server" Width="250px"></telerik:RadTextBox></div>
          <br />
          <div>
            Primary Table</div>
          <div>
            <telerik:RadComboBox ID="cmbCategory" runat="server" Width="250px" AutoPostBack="True"
              OnSelectedIndexChanged="cmbCategory_SelectedIndexChanged"></telerik:RadComboBox>
          </div>
          <br />
          <div>
            Secondary Table</div>
          <div>
            <telerik:RadComboBox ID="cmbSubCat" runat="server" Width="250px" AutoPostBack="True"
              OnSelectedIndexChanged="cmbSubCat_SelectedIndexChanged"></telerik:RadComboBox>
          </div>
        </div>
      </telerik:RadPageView>
      <telerik:RadPageView ID="pvFields" runat="server">
        <div style="padding: 10px 0 0 10px;">
          Columns</div>
        <div style="height: 280px; width: 460px; overflow-y: auto; overflow-x: hidden; border: Solid 1px #7793B9;
          margin: 3px 0 0 10px; background-color: #ffffff; position: relative;">
          <asp:CheckBoxList ID="cblFields" runat="server" BackColor="#ffffff" Width="100%"></asp:CheckBoxList>
        </div>
      </telerik:RadPageView>
      <telerik:RadPageView ID="pvFilter" runat="server">
        <div id="divFilter" runat="server">
          <div style="width: 450px; height: 280px; overflow-y: auto; overflow-x: hidden; background-color: #ffffff;
            margin: 10px 0 10px 10px; border: Solid 1px #7793B9; position: relative; padding: 10px 0 0 10px;">
            <uc1:ReportFilterControl ID="filterControl" runat="server" />
          </div>
        </div>
      </telerik:RadPageView>
    </telerik:RadMultiPage>
    <div style="width: 100%;">
      <div style="float: left; padding-left: 10px;">
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" />
      </div>
      <div style="float: right;">
        <asp:Button ID="btnBack" runat="server" Text="<< Back" Enabled="false" OnClick="btnBack_Click" />
        &nbsp
        <asp:Button ID="btnNext" runat="server" Text="Next >>" OnClick="btnNext_Click" />
      </div>
    </div>
  </div>
  <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

    <script language="javascript" type="text/javascript">
      function GetRadWindow() {
        if (window.radWindow) {
          return window.radWindow;
        }
        if (window.frameElement && window.frameElement.radWindow) {
          return window.frameElement.radWindow;
        }
        return null;
      }
      function Close(id) {
        GetRadWindow().Close(id);
        
      }


          
    </script>

  </telerik:RadCodeBlock>
  </form>
</body>
</html>
