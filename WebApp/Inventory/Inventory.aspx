<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Inventory.aspx.vb" Inherits="Inventory_Inventory" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../css_5/ui.css" rel="stylesheet" type="text/css" />
    <link href="../css_5/frame.css" rel="stylesheet" type="text/css" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
    </telerik:RadScriptManager>
    <telerik:RadSplitter ID="RadSplitter1" runat="server" Width="100%" Height="100%"
        BorderSize="0" Orientation="Horizontal">
        <telerik:RadPane ID="paneTabs" runat="server" Scrolling="None" Height="29px">
            <telerik:RadTabStrip ID="tsMain" runat="server" SelectedIndex="0" ShowBaseLine="True"
                Width="100%" PerTabScrolling="True" ScrollChildren="True" MultiPageID="RadMultiPage1">
                <Tabs>
                    <telerik:RadTab runat="server" Text="Tab1">
                    </telerik:RadTab>
                    <telerik:RadTab runat="server" Selected="True" Text="Tab2">
                    </telerik:RadTab>
                    <telerik:RadTab runat="server" Text="Tab3">
                    </telerik:RadTab>
                    <telerik:RadTab runat="server" Text="Tab4">
                    </telerik:RadTab>
                    <telerik:RadTab runat="server" Text="Tab5">
                    </telerik:RadTab>
                </Tabs>
            </telerik:RadTabStrip>
        </telerik:RadPane>
        
        <telerik:RadPane ID="panePage" runat="server" Height="100%" Width="100%">
            <telerik:RadMultiPage ID="RadMultiPage1" Runat="server" SelectedIndex="0" Height="100%">
            <telerik:RadPageView id="RadPageView1" runat="server" Height="100%">
           <!--Assigned Inventory-->
           <!--Start here-->
                <telerik:RadSplitter ID="RadSplitter2" runat="server" Height="100%" Width="100%"
                Orientation="Horizontal" BorderSize="0">
                <telerik:RadPane ID="paneToolbar" runat="server" Scrolling="None" Height="31px">
                    <telerik:RadToolBar ID="toolBar" runat="server" CssClass="NoRoundedCornerEnds" OnClientButtonClicked="toolBar_OnClientButtonClicked">
                        <Items>
                            <telerik:RadToolBarButton runat="server" Text="Action" Value="actionValue">
                            </telerik:RadToolBarButton>
                        </Items>
                    </telerik:RadToolBar>
                </telerik:RadPane>
                <telerik:RadPane ID="paneGrid" runat="server">
                    <div class="stretchContentHolderDiv">
                        
                        <telerik:RadGrid ID="gridAssignedInventory" runat="server" BorderWidth="0px" GridLines="None"
                            Height="100%" Width="100%" DataSourceID="sqlAssignedInventory" >
                            <HeaderContextMenu>
                                <CollapseAnimation Duration="200" Type="OutQuint" />
                            </HeaderContextMenu>
                            <MasterTableView TableLayout="Fixed" CellSpacing="-1" 
                                DataSourceID="sqlAssignedInventory">
                            </MasterTableView>
                            <ClientSettings>
                                <Scrolling AllowScroll="true" />
                            </ClientSettings>
                            <FilterMenu>
                                <CollapseAnimation Duration="200" Type="OutQuint" />
                            </FilterMenu>
                        </telerik:RadGrid>
                        <asp:SqlDataSource ID="sqlAssignedInventory" runat="server" 
                            ConnectionString="<%$ ConnectionStrings:MainConnection %>" 
                            ProviderName="<%$ ConnectionStrings:MainConnection.ProviderName %>" 
                            SelectCommand="select * from products"></asp:SqlDataSource>
                    </div>
                </telerik:RadPane>
            </telerik:RadSplitter>
            
            <!--End here-->
            
            </telerik:RadPageView>
            <telerik:RadPageView id="RadPageView2" runat="server" Height="100%">

                       <!--Warehouse (unassigned) Inventory -->            
                       <!--Start here-->
                       
                <telerik:RadSplitter ID="RadSplitter3" runat="server" Height="100%" Width="100%"
                Orientation="Horizontal" BorderSize="0">
                <telerik:RadPane ID="RadPane1" runat="server" Scrolling="None" Height="31px">
                    <telerik:RadToolBar ID="RadToolBar1" runat="server" CssClass="NoRoundedCornerEnds" OnClientButtonClicked="toolBar_OnClientButtonClicked">
                        <Items>
                            <telerik:RadToolBarButton runat="server" Text="Action" Value="actionValue">
                            </telerik:RadToolBarButton>
                        </Items>
                    </telerik:RadToolBar>
                </telerik:RadPane>
                <telerik:RadPane ID="RadPane2" runat="server">
                    <div class="stretchContentHolderDiv">
                        
                        <telerik:RadGrid ID="RadGrid1" runat="server" BorderWidth="0px" GridLines="None"
                            Height="100%" Width="100%" DataSourceID="sqlAssignedInventory" >
                            <HeaderContextMenu>
                                <CollapseAnimation Duration="200" Type="OutQuint" />
                            </HeaderContextMenu>
                            <MasterTableView TableLayout="Fixed" CellSpacing="-1" 
                                DataSourceID="sqlAssignedInventory">
                            </MasterTableView>
                            <ClientSettings>
                                <Scrolling AllowScroll="true" />
                            </ClientSettings>
                            <FilterMenu>
                                <CollapseAnimation Duration="200" Type="OutQuint" />
                            </FilterMenu>
                        </telerik:RadGrid>
                        <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
                            ConnectionString="<%$ ConnectionStrings:MainConnection %>" 
                            ProviderName="<%$ ConnectionStrings:MainConnection.ProviderName %>" 
                            SelectCommand="select * from products"></asp:SqlDataSource>
                    </div>
                </telerik:RadPane>
            </telerik:RadSplitter>
            
            <!--End here-->
            
            
            </telerik:RadPageView>
            <telerik:RadPageView id="RadPageView3" runat="server"   Height="100%">
            
            
                       <!--Junkyard-->
                       <!--Start here-->
                       
                <telerik:RadSplitter ID="RadSplitter4" runat="server" Height="100%" Width="100%"
                Orientation="Horizontal" BorderSize="0">
                <telerik:RadPane ID="RadPane3" runat="server" Scrolling="None" Height="31px">
                    <telerik:RadToolBar ID="RadToolBar2" runat="server" CssClass="NoRoundedCornerEnds" OnClientButtonClicked="toolBar_OnClientButtonClicked">
                        <Items>
                            <telerik:RadToolBarButton runat="server" Text="Action" Value="actionValue">
                            </telerik:RadToolBarButton>
                        </Items>
                    </telerik:RadToolBar>
                </telerik:RadPane>
                <telerik:RadPane ID="RadPane4" runat="server">
                    <div class="stretchContentHolderDiv">
                        
                        <telerik:RadGrid ID="RadGrid2" runat="server" BorderWidth="0px" GridLines="None"
                            Height="100%" Width="100%" DataSourceID="sqlAssignedInventory" >
                            <HeaderContextMenu>
                                <CollapseAnimation Duration="200" Type="OutQuint" />
                            </HeaderContextMenu>
                            <MasterTableView TableLayout="Fixed" CellSpacing="-1" 
                                DataSourceID="sqlAssignedInventory">
                            </MasterTableView>
                            <ClientSettings>
                                <Scrolling AllowScroll="true" />
                            </ClientSettings>
                            <FilterMenu>
                                <CollapseAnimation Duration="200" Type="OutQuint" />
                            </FilterMenu>
                        </telerik:RadGrid>
                        <asp:SqlDataSource ID="SqlDataSource2" runat="server" 
                            ConnectionString="<%$ ConnectionStrings:MainConnection %>" 
                            ProviderName="<%$ ConnectionStrings:MainConnection.ProviderName %>" 
                            SelectCommand="select * from products"></asp:SqlDataSource>
                    </div>
                </telerik:RadPane>
            </telerik:RadSplitter>
            
            <!--End here-->
            
            
            
            </telerik:RadPageView>
        </telerik:RadMultiPage>
        </telerik:RadPane>
    </telerik:RadSplitter>
    <telerik:RadWindow ID="wndAction" runat="server" Behaviors="Move, Close" Modal="True"
        Overlay="True" VisibleStatusbar="False" IconUrl="~/images/icons/TeamSupportLogo16.png"
        ReloadOnShow="True" InitialBehavior="None" ShowContentDuringLoad="False" KeepInScreenBounds="True"
        AutoSize="False" DestroyOnClose="False" NavigateUrl="../dialogs/inventoryaction.aspx">
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
                    $find("<%=gridAssignedInventory.ClientID %>").rebind();  // refreshes the grid

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
