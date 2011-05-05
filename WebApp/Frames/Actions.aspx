<%@ Page Title="" Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true" CodeFile="Actions.aspx.cs" Inherits="Frames_Actions" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
  <style type="text/css">
    body
    {
    	background-color:#ffffff;
    	}
        .actionCaptionDiv
    {
      font-size: 14px;
      font-weight: bold;
    }
    .actionBodyDiv
    {
      padding: 10px 40px 0 15px;
    }
    .actionAttachmentsDiv
    {
      padding-left: 15px;
    }
    .actionTimeDiv
    {
      padding-left: 15px;	
    }
    .actionPropertiesDiv
    {
      padding-left: 15px;	
    }

    .actionFooterDiv
    {
      font-style: italic;
      border-bottom: dotted 0px #97aad1;
      margin-right: 20px;
      padding-bottom: 7px;
      padding-left: 15px;
    }

  
  </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div id="actionDetailsDiv" style="width: 100%; height:100%; background-color:#ffffff;" runat="server">
        <asp:Repeater ID="rptActions" runat="server">
          <ItemTemplate>
            <br />
            <div class="groupBarDiv groupBarBlue" style="width: 98%; margin: 0 auto;">
              <div class="groupBarHeaderDiv">
                <span class="groupBarHeaderSpan"></span>
                <span class="groupBarCaptionSpan">
                  <%# DataBinder.Eval(Container.DataItem, "Name")%></span>
                <span class="groupBarButtonSpanWrapper">
                  <span class="groupBarButtonsSpan">
                    <a class="groupBarButtonLink" href="#" onclick="EditAction(<%# DataBinder.Eval(Container.DataItem, "TicketID")%>, <%# DataBinder.Eval(Container.DataItem, "ActionID")%>)">
                      <span class="groupBarButtonSpan">
                        <img alt="" src="../images/icons/edit.png" class="groupBarButtonImage" />
                        <span class="groupBarButtonTextSpan">Edit Action</span>
                      </span>
                    </a>
                  </span>
                </span>
              </div>
            </div>
            <br />
            <div class="actionBodyDiv">
              <%# DataBinder.Eval(Container.DataItem, "Description")%></div>
            <br />
            <div class="actionAttachmentsDiv">
              <strong>Attachments:</strong> &nbsp
              <%# DataBinder.Eval(Container.DataItem, "Attachments")%></div>
            <br />
            <%# DataBinder.Eval(Container.DataItem, "Time")%>
            <%# DataBinder.Eval(Container.DataItem, "Properties")%>
            <div class="actionFooterDiv">
              -&nbsp<span><%# DataBinder.Eval(Container.DataItem, "UserName")%></span>
              &nbsp&nbsp&nbsp<span><%# DataBinder.Eval(Container.DataItem, "Date")%></span>
            </div>
          </ItemTemplate>
        </asp:Repeater>
        <br />
    </div>
    <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">
      <script type="text/javascript" language="javascript">
        function EditAction(ticketID, actionID)
        {
          var wnd = top.GetActionDialog(ticketID, actionID);
          wnd.add_close(ActionWindowClosed);
          wnd.show();
        }
        
        function ActionWindowClosed(sender, args)
        {
          sender.remove_close(ActionWindowClosed);
          __doPostBack();
        }
      </script>
    
    </telerik:RadScriptBlock>
    
    
</asp:Content>