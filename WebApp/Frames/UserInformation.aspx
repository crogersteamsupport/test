<%@ Page Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true"
  CodeFile="UserInformation.aspx.cs" Inherits="Frames_UserInformation" Title="Untitled Page" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <telerik:RadWindowManager ID="RadWindowManager1" runat="server"></telerik:RadWindowManager>
  <telerik:RadSplitter ID="RadSplitter1" runat="server" BorderSize="0" Height="100%"
    Width="100%" VisibleDuringInit="false">
    <telerik:RadPane ID="RadPane1" runat="server" Scrolling="Y" Height="100%" Width="100%"
      BackColor="White">
      <div id="pnlUserProperties" runat="server">
        <div style="margin: 0 auto; padding: 0 40px 20px 20px;">
          <div class="groupDiv groupLightBlue" style="padding-top: 10px;">
            <div class="groupHeaderDiv">
              <span class="groupHeaderSpan"></span>
              <span class="groupCaptionSpan">Properties</span>
            <span class="groupButtonSpanWrapper">
                      <span class="groupButtonsSpan">
                        <asp:LinkButton ID="btnEditProperties" runat="server" CssClass="groupButtonLink">
                        <span class="groupButtonSpan">
                          <img alt="" src="../images/icons/edit.png" class="groupButtonImage" />
                          <span class="groupButtonTextSpan">Edit User</span> </span></asp:LinkButton>
                      </span>
                    </span>              
            </div>
            <div class="groupBodyWrapperDiv">
              <div class="groupBodyDiv">
                <div id="pnlProperties" runat="server" class="adminDiv" style="padding: 5px 5px 5px 5px;">
                  <asp:Label ID="lblProperties" runat="server" Text="There are no properties to display."></asp:Label>
                  <asp:Repeater ID="rptProperties" runat="server">
                    <ItemTemplate>
                      <div style="margin: 5px 5px 5px 15px; line-height: 20px;">
                        <span style="font-weight: bold;">
                          <%# DataBinder.Eval(Container.DataItem, "Name")%></span>
                        <span>
                          <%# DataBinder.Eval(Container.DataItem, "Value")%>
                          <br />
                        </span>
                      </div>
                    </ItemTemplate>
                  </asp:Repeater>
                </div>
              </div>
            </div>
          </div>
          <div class="groupDiv groupLightBlue" style="padding-top: 10px;">
            <div class="groupHeaderDiv">
              <span class="groupHeaderSpan"></span>
              <span class="groupCaptionSpan">Phone Numbers</span>
              <span class="groupButtonSpanWrapper">
                <span class="groupButtonsSpan">
                  <asp:LinkButton ID="btnNewPhone" runat="server" CssClass="groupButtonLink">
              <span class="groupButtonSpan">
                <img alt="" src="../images/icons/add.png" class="groupButtonImage" />
                <span class="groupButtonTextSpan">Add Phone Number</span> </span></asp:LinkButton>
                </span>
              </span>
            </div>
            <div class="groupBodyWrapperDiv">
              <div class="groupBodyDiv"">
                <div id="pnlPhone" runat="server" class="adminDiv" style="padding: 5px 5px 5px 5px;">
                  <asp:Label ID="lblPhone" runat="server" Text="There are no phone numbers to display."></asp:Label>
                  <asp:Repeater ID="rptPhone" runat="server">
                    <ItemTemplate>
                      <div style="margin: 5px 5px 5px 15px;">
                        <div class="repeaterItem">
                          <img src="../images/icons/Edit.png" alt="Edit" onclick="ShowDialog(top.GetPhoneDialog(<%# DataBinder.Eval(Container.DataItem, "PhoneID")%>));" />
                          <img src="../images/icons/Trash.png" alt="Trash" onclick="if (confirm('Are you sure you would like to remove this phone number?')){top.privateServices.DeletePhone(<%# DataBinder.Eval(Container.DataItem, "PhoneID")%>); RefreshContent();}" />
                          <span style="font-weight: bold;">
                            <%# DataBinder.Eval(Container.DataItem, "Type")%>: </span>
                          <span>
                            <%# DataBinder.Eval(Container.DataItem, "Number")%></span>
                          <span>
                            <%# DataBinder.Eval(Container.DataItem, "Ext")%></span>
                        </div>
                      </div>
                    </ItemTemplate>
                  </asp:Repeater>
                </div>
              </div>
            </div>
          </div>
          <div class="groupDiv groupLightBlue" style="padding-top: 10px;">
            <div class="groupHeaderDiv">
              <span class="groupHeaderSpan"></span>
              <span class="groupCaptionSpan">Addresses</span>
              <span class="groupButtonSpanWrapper">
                <span class="groupButtonsSpan">
                  <asp:LinkButton ID="btnNewAddress" runat="server" CssClass="groupButtonLink">
              <span class="groupButtonSpan">
                <img alt="" src="../images/icons/add.png" class="groupButtonImage" />
                <span class="groupButtonTextSpan">Add Address</span> </span></asp:LinkButton>
                </span>
              </span>
            </div>
            <div class="groupBodyWrapperDiv">
              <div class="groupBodyDiv">
                <div id="pnlAddress" runat="server" class="adminDiv" style="padding: 5px 5px 5px 5px;">
                  <asp:Label ID="lblAddresses" runat="server" Text="There are no addresses to display."></asp:Label>
                  <asp:Repeater ID="rptAddresses" runat="server">
                    <ItemTemplate>
                      <div style="margin: 5px 5px 15px 15px;">
                        <div class="repeaterItem" style="font-weight: bold;">
                          <img src="../images/icons/Edit.png" alt="Edit" onclick="ShowDialog(top.GetAddressDialog(<%# DataBinder.Eval(Container.DataItem, "AddressID")%>));" />
                          <img src="../images/icons/Trash.png" alt="Trash" onclick="if (confirm('Are you sure you would like to remove this address?')){top.privateServices.DeleteAddress(<%# DataBinder.Eval(Container.DataItem, "AddressID")%>); RefreshContent();}" />
                          <span>
                            <%# DataBinder.Eval(Container.DataItem, "Description")%></span>
                        </div>
                        <div>
                          <%# DataBinder.Eval(Container.DataItem, "Addr1")%></div>
                        <div>
                          <%# DataBinder.Eval(Container.DataItem, "Addr2")%></div>
                        <div>
                          <%# DataBinder.Eval(Container.DataItem, "Addr3")%></div>
                        <div>
                          <%# DataBinder.Eval(Container.DataItem, "City")%>,
                          <%# DataBinder.Eval(Container.DataItem, "State")%>
                          <%# DataBinder.Eval(Container.DataItem, "Zip")%></div>
                        <div>
                          <%# DataBinder.Eval(Container.DataItem, "Country")%></div>
                      </div>
                    </ItemTemplate>
                  </asp:Repeater>
                </div>
              </div>
            </div>
          </div>
          <div id="pnlGroups" runat="server" class="groupDiv groupLightBlue" style="padding-top: 10px;">
            <div class="groupHeaderDiv">
              <span class="groupHeaderSpan"></span>
              <span class="groupCaptionSpan">Groups</span>
              <span class="groupButtonSpanWrapper">
                <span class="groupButtonsSpan">
                  <asp:LinkButton ID="btnNewGroup" runat="server" CssClass="groupButtonLink">
              <span class="groupButtonSpan">
                <img alt="" src="../images/icons/add.png" class="groupButtonImage" />
                <span class="groupButtonTextSpan">Add Group</span> </span></asp:LinkButton>
                </span>
              </span>
            </div>
            <div class="groupBodyWrapperDiv">
              <div class="groupBodyDiv">
                <div id="pnlGroup" runat="server" class="adminDiv" style="padding: 5px 5px 5px 5px;">
                  <asp:Label ID="lblGroups" runat="server" Text="There are no groups to display."></asp:Label>
                  <asp:Repeater ID="rptGroups" runat="server">
                    <ItemTemplate>
                      <div style="margin: 5px 5px 5px 15px;">
                        <div class="repeaterItem" style="font-weight: bold;">
                          <img src="../images/icons/Trash.png" alt="Trash" onclick="radconfirm('Are you sure you would like to remove this group?', function(arg){if(arg){top.privateServices.DeleteGroupUser(<%# DataBinder.Eval(Container.DataItem, "GroupID")%>, <%# DataBinder.Eval(Container.DataItem, "UserID")%>, RefreshContent);}}, 250, 125, null, 'Delete Group'); return false;" />
                          <span>
                            <%# DataBinder.Eval(Container.DataItem, "Name")%></span>
                        </div>
                      </div>
                    </ItemTemplate>
                  </asp:Repeater>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div style="clear: both;">
        </div>
      </div>
    </telerik:RadPane>
  </telerik:RadSplitter>
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
        window.location = window.location;
      }
    
    
    </script>

  </telerik:RadScriptBlock>
</asp:Content>
