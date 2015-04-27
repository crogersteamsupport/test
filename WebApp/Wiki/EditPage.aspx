<%@ Page Language="VB" AutoEventWireup="false" CodeFile="EditPage.aspx.vb" Inherits="EditPage" %>

<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">



<html style="margin:0px;height:100%;overflow:hidden;" scroll="no">  



<head runat="server">
    <title></title>
    <style type="text/css">
        .style1
        {
            width: 70%;
        }
        .style2
        {
            width: 118px;
            text-align: right;
            font-weight: bold;
        }
        .style3
        {
            width: 100%;
        }
        .blueBackground
        {
        background-color: #DBE6F4;
        }
        
        .style4
        {
            width: 80%;
        }
        
        </style>
        
        <style type="text/css">  
    html, form, body{ height: 100%; margin: 0; }   
    </style> 
</head>
<body >  
    <form id="form1" runat="server" style="height:100%;margin:0px" >
    
    
    
         <telerik:RadSplitter ID="TopTitleSplitter" Runat="server" Height="200px" 
         Width="100%">
                 <telerik:RadPane ID="TitlePane" Runat="server" Scrolling="None" Width="100%">
    <div style="text-align: center; font-weight: 700; font-size: x-large" 
        class="blueBackground">
        <table class="style3">
            <tr>
                <td class="style4">
                    Edit Wiki Page<telerik:RadToolTip ID="RadToolTip1" runat="server" 
                        style="display: none;">
                    </telerik:RadToolTip>
            <telerik:RadScriptManager ID="RadScriptManager1" Runat="server" >
        </telerik:RadScriptManager>
    
                </td>
                <td width="20%" style="text-align: right">
                    <asp:ImageButton ID="btn_SaveArticle" runat="server" 
                        ImageUrl="Images/saveicon.png" ToolTip="Save article" Height="32px" 
                        Width="32px" />
                    <asp:ImageButton ID="btn_CancelEdit" runat="server" CausesValidation="False" 
                        Height="32px" ImageUrl="Images/cancell2.png" ToolTip="Cancel edit" 
                        validation="false" Width="32px" />
                    <asp:ImageButton ID="btn_DeleteArticle" runat="server" Height="32px" 
                        ImageUrl="./Images/Trash Can.png" Width="32px" />
                    <asp:ImageButton ID="ImageButton1" runat="server" Height="32px" 
                        ImageUrl="./Images/Help.png" Width="32px" />
                </td>
            </tr>
        </table>
    </div>
 
    
    
  
    
    <div class="blueBackground">
        
        <table align="center" class="style1">
            <tr>
                <td class="style2">
                    Page Name:</td>
                <td>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                        ControlToValidate="tb_PageName" 
                        ErrorMessage="Please enter a name for this page!&lt;br/&gt;"></asp:RequiredFieldValidator>
                    <asp:TextBox ID="tb_PageName" runat="server" Width="95%"></asp:TextBox>
                </td>
            </tr>
            <tr>
              <td class="style2">
                 Sub Page of:
              </td>
              <td>
                  <asp:DropDownList ID="dd_ParentID" runat="server" AppendDataBoundItems="True" 
                      DataSourceID="MasterPageSource" DataTextField="ArticleName" 
                      DataValueField="ArticleID" Height="25px" Width="480px">
                      <asp:ListItem Value="0">--None--</asp:ListItem>
                  </asp:DropDownList>
                </td>
            </tr>
            </table>
        <div style="text-align: center">
        <fieldset style="width: 70%;margin:0 auto"> <legend style="text-align: left"><b>Article Security</b></legend>
            <table class="style3">
                <tr>
                    <td colspan="2">
                        <asp:CheckBox ID="cb_private" runat="server" 
                            Text="Private Article (only I can see it)" />
                    </td>
                    
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox ID="cb_AdvPortal" runat="server" 
                            Text="Visible on Portals and Public KB" />
                    </td>
                    <td>
                        <asp:CheckBox ID="cb_basicPortal" runat="server" 
                            Text="Publicly Visible" />
                        &nbsp;(<asp:HyperLink ID="JustArticleLink" runat="server">Link</asp:HyperLink>
                        )</td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox ID="cb_AdvPortalEdit" runat="server" 
                            Text="Editable on Advanced Portal" Visible="False" />
                    </td>
                    <td>
                        <asp:CheckBox ID="cb_BasicPortalEdit" runat="server" 
                            Text="Publically Editable" Visible="False" />
                    </td>
                </tr>
            </table>
            </fieldset> 
            </div >
        </div>
        
         </telerik:RadPane>
                 
      
         </telerik:RadSplitter>        
                 
                 
                 
          <telerik:RadSplitter ID="ContentPane" Runat="server"  
            Width="100%" Height="100%" SplitBarsSize="" OnClientLoad="ResizeContentPane">            
                 
                 <telerik:RadPane ID="NavigationPane" Runat="server" Width="150px" 
             class="leftBar" BackColor="#DBE6F4" EnableEmbeddedBaseStylesheet="False" 
             EnableEmbeddedSkins="False" Height="" Index="0" Skin="" SkinID="Web20" 
                     Scrolling="Y">
             <div style="text-align:center"><b>Previous Versions</b></div>
                     <div><asp:HiddenField ID="VersionViewing" runat="server" />
                         <asp:DataList ID="DataList1" runat="server" 
                             DataSourceID="PreviousVersionSource" Width="147px" Font-Size="Small"><ItemTemplate>
                                 <asp:LinkButton ID="ShowArticle" runat="server" 
                                 CommandArgument='<%# Eval("HistoryID") %>'>
                                     <div style="border-style:groove ;border-width:2px;"><%#DisplayLocalDateFormat(ConvertToLocalTime(Eval("ModifiedDate")))%> by <%#Eval("UserName")%></div>
                                     </asp:LinkButton></ItemTemplate></asp:DataList></div></telerik:RadPane>
             
         
         
         
        
                 
        
        
        
        <telerik:RadPane ID="EditorPanel" Runat="server"  Scrolling="None">
        
<telerik:RadEditor 
                         ID="RadEditor1" Runat="server" Skin="Web20" 
                OnClientLoad="ResizeEditor" Width="100%" Height="50px" CssClass="blueBackground"><ImageManager 
                             MaxUploadFileSize="5120000" SearchPatterns="*.*" UploadPaths="~/WikiImages" 
                             ViewPaths="~/WikiImages" /><Tools>
                           <telerik:EditorToolGroup>    
                          <telerik:EditorTool  Name="InsertTicketLink" text="Insert Ticket Link" showtext="true" showicon="true"/>
                         </telerik:EditorToolGroup>  
                             
                             <telerik:EditorToolGroup><telerik:EditorTool 
                                 Name="Cut" /><telerik:EditorTool Name="Copy" /><telerik:EditorTool 
                                 Name="Paste" /><telerik:EditorTool Name="PasteAsHtml" /><telerik:EditorTool 
                                 Name="PasteFromWord" /><telerik:EditorTool Name="Redo" /><telerik:EditorTool 
                                 Name="Undo" /><telerik:EditorTool Name="ConvertToUpper" /><telerik:EditorTool 
                                 Name="ConvertToLower" /><telerik:EditorTool Name="SelectAll" /><telerik:EditorTool 
                                 Name="FindAndReplace" /><telerik:EditorTool Name="AjaxSpellCheck" /></telerik:EditorToolGroup><telerik:EditorToolGroup><telerik:EditorTool 
                                     Name="Bold" /><telerik:EditorTool Name="Italic" /><telerik:EditorTool 
                                     Name="Underline" /><telerik:EditorTool Name="StrikeThrough" /><telerik:EditorTool 
                                     Name="FontSize" /><telerik:EditorTool Name="ForeColor" /><telerik:EditorTool 
                                     Name="BackColor" />   <telerik:EditorTool 
                                     Name="FontName" /><telerik:EditorTool Name="InsertUnorderedList" /><telerik:EditorTool 
                                     Name="InsertOrderedList" /><telerik:EditorTool Name="JustifyCenter" /><telerik:EditorTool 
                                     Name="JustifyRight" /><telerik:EditorTool Name="JustifyLeft" /><telerik:EditorTool 
                                     Name="JustifyNone" /><telerik:EditorTool Name="JustifyFull" /></telerik:EditorToolGroup><telerik:EditorToolGroup><telerik:EditorTool 
                                     Name="TableWizard" /><telerik:EditorTool Name="InsertTable" /><telerik:EditorTool 
                                     Name="InsertRowAbove" /><telerik:EditorTool Name="InsertRowBelow" /><telerik:EditorTool 
                                     Name="InsertColumnLeft" /><telerik:EditorTool Name="InsertColumnRight" /><telerik:EditorTool 
                                     Name="DeleteTable" /><telerik:EditorTool Name="DeleteCell" /><telerik:EditorTool 
                                     Name="DeleteColumn" /><telerik:EditorTool Name="DeleteRow" /></telerik:EditorToolGroup><telerik:EditorToolGroup><telerik:EditorTool 
                                     Name="FormatBlock" /><telerik:EditorTool Name="FormatCodeBlock" /><telerik:EditorTool 
                                     Name="Indent" /><telerik:EditorTool Name="Outdent" /><telerik:EditorTool Name="InsertGroupbox" /><telerik:EditorTool 
                                     Name="InsertHorizontalRule" /><telerik:EditorTool Name="LinkManager" /><telerik:EditorTool 
                                     Name="DocumentManager" /><telerik:EditorTool Name="ImageManager" /><telerik:EditorTool 
                                     Name="ImageMapDialog" /><telerik:EditorTool Name="InsertSymbol" /></telerik:EditorToolGroup><telerik:EditorToolGroup><telerik:EditorTool 
                                     Name="InsertCustomLink" /><telerik:EditorTool Name="ToggleScreenMode" /></telerik:EditorToolGroup></Tools><Content>
</Content></telerik:RadEditor></telerik:RadPane>
        </telerik:RadSplitter>
    
    

    <asp:SqlDataSource ID="PreviousVersionSource" runat="server" 
        SelectCommand="select top 25  wikihistory.articleid, wikihistory.historyid, wikihistory.organizationid, wikihistory.articlename, wikihistory.body, wikihistory.version, wikihistory.createdby, wikihistory.createddate, isnull(wikihistory.modifieddate,'12/1/1980') as modifieddate, wikihistory.modifiedby, users.firstname+' '+users.lastname as UserName from wikihistory left outer join users on wikihistory.modifiedby = users.userid where wikihistory.ArticleID=@ArticleID and wikihistory.OrganizationID=@OrganizationID
Order by wikihistory.HistoryID desc" 
        ConnectionString="<%$ ConnectionStrings:TeamSupportConnectionString %>" 
        
             
             
             
             
             ProviderName="<%$ ConnectionStrings:TeamSupportConnectionString.ProviderName %>">
        <SelectParameters>
            <asp:Parameter Name="ArticleID" />
            <asp:Parameter Name="OrganizationID" />
        </SelectParameters>
    </asp:SqlDataSource>
    
    

    <asp:SqlDataSource ID="MasterPageSource" runat="server" 
        ConnectionString="<%$ ConnectionStrings:TeamSupportConnectionString %>" 
        
        
             
             
             ProviderName="<%$ ConnectionStrings:TeamSupportConnectionString.ProviderName %>" SelectCommand="Select * from wikiarticles where Organizationid=@OrgID and ArticleID&lt;&gt;@ArticleID
and ( (IsNull(Private,0)=0) or (CreatedBy=@UserID)) and IsNull(IsDeleted,0)=0
Order by ArticleName">
        <SelectParameters>
            <asp:Parameter Name="OrgID" />
            <asp:Parameter Name="ArticleID" />
            <asp:Parameter Name="UserID" />
        </SelectParameters>
    </asp:SqlDataSource>
    
    
<script type="text/javascript" >


    function ResizeEditor() {

        var oFun = function() {

            var editor = $find("<%=RadEditor1.ClientID%>");

            var pane = $get("<%=EditorPanel.ClientID%>");



            var _paneWidth = ((pane.height == '') && (pane.width) && (pane.width != '')) ? pane.width : pane.offsetWidth;

            var _panelHeight2 = document.body.clientHeight - $get("TopTitleSplitter").clientHeight



            editor.setSize(_paneWidth, _panelHeight2);



        }

        window.setTimeout(oFun, 500);



    }



    function ResizeContentPane() {

        var oFun = function() {

            var contentPane = $find("<%=ContentPane.ClientID%>");

            var totalPage = document.body.clientHeight;





            var _paneWidth = ((pane.height == '') && (pane.width) && (pane.width != '')) ? pane.width : pane.offsetWidth;

            var _panelHeight2 = document.body.clientHeight - $get("TopTitleSplitter").clientHeight





            contentPane.setSize(document.body.clientWidth, _panelHeight2);



        }

        window.setTimeout(oFun, 500);



    }


    Telerik.Web.UI.Editor.CommandList["InsertTicketLink"] = function(commandName, editor, args) {
        var elem = editor.getSelectedElement(); //returns the selected element.

        if (elem.tagName == "A") {
            editor.selectElement(elem);
            argument = elem;
        }
        else {
            var content = editor.getSelectionHtml();
            var link = editor.get_document().createElement("A");
            link.innerHTML = content;
            argument = link;
        }

        var myCallbackFunction = function(sender, args) {
            editor.pasteHtml(String.format("<a href={0} target='{1}' class='{2}'>{3}</a> ", args.href, args.target, args.className, args.name))
        }

        editor.showExternalDialog(
            '../Editor/SelectTicket.aspx',
            argument,
            290,
            200,
            myCallbackFunction,
            null,
            'Insert Ticket Link',
            true,
            Telerik.Web.UI.WindowBehaviors.Close + Telerik.Web.UI.WindowBehaviors.Move,
            false,
            false);
    }

    
</script>

    </form>
</body>
</html>
