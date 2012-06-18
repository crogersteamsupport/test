<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Inventory.aspx.vb" Inherits="Frames_Inventory" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
  <link href="../css_5/frame.css" rel="stylesheet" type="text/css" />
  <link href="../css_5/jquery-ui-latest.custom.css" rel="stylesheet" type="text/css" />
  <link href="../css_5/ui.css" rel="stylesheet" type="text/css" />
  <script src="../js_5/jquery-1.4.2.min.js" type="text/javascript"></script>
  <script src="../vcr/142/Js/jquery-ui-1.8.14.custom.min.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
    </telerik:RadScriptManager>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
    </telerik:RadAjaxManager>
  <telerik:RadSplitter VisibleDuringInit="false" ID="splMain" runat="server" Orientation="Horizontal"
    Height="100%" Width="100%" BorderSize="0" HeightOffset="0" LiveResize="False">
    <telerik:RadPane ID="paneToolBar" runat="server" Height="30px" Scrolling="None">
      <telerik:RadToolBar ID="toolBar" runat="server" CssClass="NoRoundedCornerEnds" OnClientButtonClicked="toolBar_OnClientButtonClicked">
        <Items>
          <telerik:RadToolBarButton runat="server" Text="Action" Value="actionValue">
          </telerik:RadToolBarButton>
        </Items>
      </telerik:RadToolBar>
    </telerik:RadPane>
    <telerik:RadPane ID="paneGrid" runat="server" BackColor="#fafafa" Scrolling="None"
      Height="100%">
      <div class="stretchContentHolderDiv">
        <telerik:RadGrid ID="gridProducts" runat="server" AllowPaging="True" 
          AllowSorting="True" BorderWidth="0px" GridLines="None" Height="100%" PageSize="20"
          Width="100%">
          <PagerStyle AlwaysVisible="true" Mode="NextPrevAndNumeric" />
          <MasterTableView>
            <RowIndicatorColumn>
              <HeaderStyle Width="20px" />
            </RowIndicatorColumn>
            <ExpandCollapseColumn>
              <HeaderStyle Width="20px" />
            </ExpandCollapseColumn>
          </MasterTableView>
          <ClientSettings>
            <Scrolling AllowScroll="True" UseStaticHeaders="True" />
          </ClientSettings>
        </telerik:RadGrid>
      </div>
    </telerik:RadPane>
    </telerik:RadSplitter>
    <telerik:RadWindow ID="wndAction" runat="server" Behaviors="Move, Close"
    Modal="True" Overlay="True" VisibleStatusbar="False" IconUrl="~/images/icons/TeamSupportLogo16.png"
    ReloadOnShow="True" InitialBehavior="None" ShowContentDuringLoad="False" KeepInScreenBounds="True"
    AutoSize="False" DestroyOnClose="False"
      NavigateUrl="../dialogs/inventoryaction.aspx">
    </telerik:RadWindow>
    
    <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">
  <script type="text/javascript" language="javascript">
    function showDialog(wnd, isModal, title, callback) {
      if (title && title != '') wnd.set_title(title);
      wnd.set_modal(isModal);
      if (callback) {
        var fn = function(sender, args) { sender.remove_close(fn); callback(sender.argument); }
        wnd.add_close(fn);
      }
      wnd.show();
    }

    function showActionDialog() {
      showDialog($find("<%=wndAction.ClientID %>"), true, "My Action", function() {
        $find("<%=gridProducts.ClientID %>").rebind();  // refreshes the grid

      });
    }

    function toolBar_OnClientButtonClicked(sender, args) {
      var button = args.get_item();
      var value = button.get_value();
      if (value == 'actionValue') { showActionDialog(); }

    }

  </script>
    </telerik:RadScriptBlock>
    
    </form>
</body>
</html>
