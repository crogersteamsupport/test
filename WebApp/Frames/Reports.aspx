<%@ Page Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true"
  CodeFile="Reports.aspx.cs" Inherits="Frames_Reports" Title="Untitled Page" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <telerik:RadAjaxManager ID="ajaxManager" runat="server" UpdatePanelsRenderMode="Inline">
    <AjaxSettings>
      <telerik:AjaxSetting AjaxControlID="gridReportList">
        <UpdatedControls>
          <telerik:AjaxUpdatedControl ControlID="gridReportList" />
          <telerik:AjaxUpdatedControl ControlID="gridReport" />
        </UpdatedControls>
      </telerik:AjaxSetting>
      <telerik:AjaxSetting AjaxControlID="gridReport">
        <UpdatedControls>
          <telerik:AjaxUpdatedControl ControlID="gridReport" />
        </UpdatedControls>
      </telerik:AjaxSetting>
    </AjaxSettings>
  </telerik:RadAjaxManager>
  <telerik:RadSplitter ID="RadSplitter3" runat="server" Height="100%" Width="100%"
    BorderSize="0" Orientation="Horizontal">
    <telerik:RadPane ID="RadPane5" runat="server" Height="32px" Scrolling="None">
      <telerik:RadToolBar ID="tbUser" runat="server" CssClass="NoRoundedCornerEnds" Width="100%"
        OnClientButtonClicked="ButtonClicked">
        <Items>
          <telerik:RadToolBarButton runat="server" Text="New" Value="New" ImageUrl="~/images/icons/new.png">
          </telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Text="Edit" Value="Edit" Enabled="false"
            ImageUrl="~/images/icons/edit.png"></telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Text="Delete" Value="Delete" Enabled="false"
            ImageUrl="~/images/icons/trash.png"></telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Text="Export to PDF" ImageUrl="~/images/icons/Export.png"
            Value="ExportPDF" Visible="false"></telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Text="Export to Excel" ImageUrl="~/images/icons/Export.png"
            Value="ExportExcel"></telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Text="Export to CSV" ImageUrl="~/images/icons/Export.png"
            Value="ExportCSV"></telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Value="Help" ImageUrl="~/images/icons/Help.png"
            Text="Help" ToolTip="Help." Visible="false"></telerik:RadToolBarButton>
            
        </Items>
      </telerik:RadToolBar>
    </telerik:RadPane>
    <telerik:RadPane ID="RadPane6" runat="server" Height="100%" Scrolling="None">
      <telerik:RadSplitter ID="RadSplitter1" runat="server" Height="100%" Width="100%"
        BorderSize="0">
        <telerik:RadPane ID="paneGrid" runat="server" Width="200px" Scrolling="None" OnClientResized="GridResized">
          <div class="stretchContentHolderDiv">
            <telerik:RadGrid ID="gridReportList" runat="server" AutoGenerateColumns="False" GridLines="None"
              Height="100%" Width="100%" ShowHeader="False" BorderWidth="0px" OnNeedDataSource="gridReportList_NeedDataSource"
              OnDataBound="gridReportList_DataBound">
              <MasterTableView DataKeyNames="ReportID,ExternalURL" ClientDataKeyNames="ReportID,ExternalURL">
                <Columns>
                  <telerik:GridBoundColumn DataField="ReportID" UniqueName="ReportID" Visible="false">
                  </telerik:GridBoundColumn>
                  <telerik:GridBoundColumn DataField="ExternalURL" UniqueName="ExternalURL" Visible="false">
                  </telerik:GridBoundColumn>
                  <telerik:GridBoundColumn DataField="Name" UniqueName="Name">
                    <HeaderStyle Width="100%" />
                    <ItemStyle BorderWidth="0px" />
                  </telerik:GridBoundColumn>
                </Columns>
              </MasterTableView>
              <ClientSettings>
                <Selecting AllowRowSelect="True"></Selecting>
                <Scrolling AllowScroll="True" UseStaticHeaders="True" />
                <ClientEvents OnRowSelected="ReportSelected" />
              </ClientSettings>
            </telerik:RadGrid>
          </div>
        </telerik:RadPane>
        <telerik:RadSplitBar ID="RadSplitBar1" runat="server" />
        <telerik:RadPane ID="RadPane2" runat="server" Width="100%" Scrolling="None">
          <iframe id="frmReport" runat="server" scrolling="no" src="" frameborder="0" height="100%"
            width="100%"></iframe>
        </telerik:RadPane>
      </telerik:RadSplitter>
    </telerik:RadPane>
  </telerik:RadSplitter>
  <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

    <script type="text/javascript" language="javascript">

      var _lastRefreshTime = new Date();

      function refreshData() {
        if (_lastRefreshTime != null) {
          var now = new Date();
          var diff = (now - _lastRefreshTime) / 1000;
          if (diff < 300) return;
        }

        _lastRefreshTime = new Date();

        window.location = window.location;

      }

      function GridResized(sender, args) {
        top.privateServices.SetUserSetting('ReportsGridWidth', sender.get_width());
      }

      function ReportSelected(sender, args) {
        var id = args.getDataKeyValue('ReportID');
        top.privateServices.SetUserSetting('SelectedReportID', id);
        top.privateServices.CanEditReport(id, SetCanEdit);
        PageMethods.GetCsvUrl(function(result) {
          var toolBar = $find("<%= tbUser.ClientID %>");
          var button = toolBar.findItemByValue("ExportCSV");          
          button.set_navigateUrl(result);
        });


        LoadContentPage();
      }

      function SetCanEdit(result) {
        var toolBar = $find("<%=tbUser.ClientID %>");
        var itemEdit = toolBar.findItemByValue("Edit");
        var itemDelete = toolBar.findItemByValue("Delete");
        itemEdit.set_enabled(result);
        itemDelete.set_enabled(result);

      }

      function GetSelectedReportID() {
        var item = $find("<%=gridReportList.ClientID %>").get_masterTableView().get_selectedItems()[0];
        return item.getDataKeyValue('ReportID');
      }

      function SetSelectedReportID(id) {
        var item = $find("<%=gridReportList.ClientID %>").get_masterTableView().get_selectedItems()[0];
        return item.getDataKeyValue('ReportID');
      }

      function GetSelectedExternalURL() {
        var item = $find("<%=gridReportList.ClientID %>").get_masterTableView().get_selectedItems()[0];
        var url = item.getDataKeyValue('ExternalURL');
        if (!url || url == '') {
          return 'ReportResults.aspx'
        }
        else {
          return url;
        }
      }

      function LoadContentPage() {
        showLoadingPanel("<%=frmReport.ClientID %>");

        var id = GetSelectedReportID();
        var frame = $get("<%=frmReport.ClientID %>");
        frame.setAttribute('src', GetSelectedExternalURL() + '?ReportID=' + id);
        hideLoadingPanel("<%=frmReport.ClientID %>");

      }

      function ButtonClicked(sender, args) {
        var button = args.get_item();
        var value = button.get_value();
        if (value == 'ExportPDF') { ExportPDF(); }
        else if (value == 'ExportExcel') { ExportExcel(); }
        else if (value == 'ExportCSV') { ExportCSV(); }
        else if (value == 'New') { NewReport(); }
        else if (value == 'Edit') { EditReport(); }
        else if (value == 'Delete') { DeleteReport(); }
        else if (value == 'Help') { top.ShowHelpDialog(430); }
      }

      function NewReport() {
        var wnd = top.GetReportEditorDialog()
        wnd.add_close(DialogClosed);
        wnd.show();
      }

      function EditReport() {
        var wnd = top.GetReportEditorDialog(GetSelectedReportID())
        wnd.add_close(DialogClosed);
        wnd.show();
      }

      function DeleteReport() {
        if (confirm('Are you sure you would like to delete this report?')) {
          top.privateServices.DeleteReport(GetSelectedReportID());
        }
        setTimeout('RefreshGrid()', 1000);
      }

      function ExportPDF() {
        var frame = $get("<%=frmReport.ClientID %>");
        frame.contentWindow.ExportPDF();
      }

      function ExportExcel() {
        var frame = $get("<%=frmReport.ClientID %>");
        frame.contentWindow.ExportExcel();
      }

      function ExportCSV() {
        var frame = $get("<%=frmReport.ClientID %>");        
        frame.contentWindow.ExportCSV();
      }

      function DialogClosed(sender, args) {
        sender.remove_close(DialogClosed);
        //RefreshGrid();
        if (args.get_argument() > -1) __doPostBack();

      }

      function RefreshGrid() {
        var grid = $find("<%=gridReportList.ClientID %>").get_masterTableView();
        grid.rebind();

        setTimeout('LoadContentPage();', 500);

      }      
      
    </script>

  </telerik:RadCodeBlock>
</asp:Content>
