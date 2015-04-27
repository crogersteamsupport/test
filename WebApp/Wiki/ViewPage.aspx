<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ViewPage.aspx.vb" Inherits="_Default" %>

<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html style="height:100%">  
<head id="Head1" runat="server">
    <title></title>
      <style type="text/css">



  </style>
<style type="text/css">



 

    .strong
    {
        text-align: center;
        font-size: x-large;
    }

 

    .topTitle
    {
        border: thin #DBE6F4 solid;
        color: #15428B;
        background-color: #DBE6F4;
        text-align: right;
    }
    

 

    .tableStyle
    {
        width: 100%;
        border: thick #808000;
    }

    .leftBar
    {
        width: 174px;
        background-color: #DBE6F4;
        height:100%
    }
    
div.NoRoundedCornerEnds, div.NoRoundedCornerEnds .rtbOuter, div.NoRoundedCornerEnds .rtbMiddle, div.NoRoundedCornerEnds .rtbInner, div.NoRoundedCornerEnds RadToolBar_Office2007
{
border-width: 0px 0px 1px 0px;
display: block;
}
div.NoRoundedCornerEnds .rtbMiddle
{
    height: 30px;
    }
div.NoRoundedCornerEnds .rtbOuter
{
    height: 31px;    }


</style>
</head>
<body style="margin:0px;height:100%;overflow:hidden;" scroll="yes">
    <form id="form1" runat="server" style="height:100%;margin:0px">
    
    
     <telerik:RadStyleSheetManager ID="RadStyleSheetManager1" runat="server">
     </telerik:RadStyleSheetManager>
        

         <telerik:RadScriptManager ID="RadScriptManager1" Runat="server">
         </telerik:RadScriptManager>
        

     <telerik:RadSplitter ID="TopTitleSplitter" Runat="server" Height="36px" 
         Width="100%">
         <telerik:RadPane ID="RadPane2" Runat="server" Scrolling="None">
            
            
            
            
             <div class="topTitle">
                 
                 
                 
                 &nbsp;<asp:LinkButton ID="ArticleLink" runat="server">Internal Link</asp:LinkButton>
                 <asp:LinkButton ID="ExternalLink" runat="server"> External Link</asp:LinkButton>
                 <asp:ImageButton ID="EditButton" runat="server" 
                     ImageUrl="Images/edit-48x48.png" ToolTip="Edit Wiki article" Height="32px" 
                     Width="32px" />
                 <asp:ImageButton ID="Createnew" runat="server" 
                     ImageUrl="Images/newfileicon.png" ToolTip="Create new wiki article" 
                     Height="32px" Width="32px" />

                 <asp:ImageButton ID="btn_Help" runat="server" Height="32px" 
                     ImageUrl="./Images/Help.png" Width="32px" />


             </div>
         </telerik:RadPane>
         
    
    
    
     </telerik:RadSplitter>

     <telerik:RadSplitter ID="RadSplitter1" Runat="server" Height="93%" 
         Width="100%" Skin="Web20">
         <telerik:RadPane ID="NavigationPane" Runat="server" Width="200px" 
             class="leftBar" BackColor="#DBE6F4" EnableEmbeddedBaseStylesheet="False" 
             EnableEmbeddedSkins="False" Height="" Index="0" Skin="" SkinID="Web20" 
             Scrolling="Y">
             <div style="font-weight: 700; text-align: center">Article Navigation</div>
                    <telerik:RadTreeView ID="RadTreeView1" Runat="server" DataFieldID="ArticleID" 
                        DataFieldParentID="ParentID" DataSourceID="NavigationSource" 
                        DataTextField="ArticleName" DataValueField="ArticleID" Skin="Vista">
                    </telerik:RadTreeView>
                         <asp:HiddenField ID="hidden_ArticleID" runat="server" />

         </telerik:RadPane>
         <telerik:RadSplitBar ID="RadSplitBar1" Runat="server" />
    
         <telerik:RadPane ID="ContentPane" Runat="server" Width="100%" 
             Height="100%" style="background-color: #FFFFFF">
             <div style="height:100%;background-color: #FFFFFF">
               <asp:Literal ID="ArticleBody" runat="server"></asp:Literal>
             </div>
         </telerik:RadPane>
       </telerik:RadSplitter>   
     
   
 
     <asp:SqlDataSource ID="NavigationSource" runat="server" 
         ConnectionString="<%$ ConnectionStrings:TeamSupportConnectionString %>" 
         
         
         SelectCommand="


SELECT [ArticleID], [ArticleName], 

Case 
  When parentid in (select articleid from wikiarticles where organizationid = @OrganizationID and  isnull(isdeleted,0) &lt;&gt; 1 and  ((IsNull(Private,0)=0) or (CreatedBy=@UserID)) )
    then ParentID
  else
   NULL
 end as ParentID




FROM [WikiArticles] 
WHERE ([OrganizationID] = @OrganizationID) 
and ((IsNull(Private,0)=0) or (CreatedBy=@UserID))
and IsNull(IsDeleted,0)=0
Order by ArticleName">
         <SelectParameters>
             <asp:Parameter Name="OrganizationID" Type="Int32" />
             <asp:Parameter Name="UserID" />
         </SelectParameters>
     </asp:SqlDataSource>
 
    
   
 

 
    
   
 

 
    
   
 

 
    
   
 
     <telerik:RadToolTip ID="RadToolTip1" runat="server" Animation="Resize" 
         Skin="Web20" TargetControlID="ArticleLink" Title="https://app.teamsupport.com?articleID=45" 
          AutoCloseDelay="15000" 
         HideDelay="500" ShowDelay="100">
     </telerik:RadToolTip>
 
    
       <telerik:RadToolTip ID="RadToolTip2" runat="server" Animation="Resize" 
         Skin="Web20" TargetControlID="ExternalLink" Title="https://app.teamsupport.com?articleID=45" 
          AutoCloseDelay="15000" 
         HideDelay="500" ShowDelay="100">
     </telerik:RadToolTip>
 

 
    
   
 

 
    
   
 

 
    
   
 
    </form>
</body>
</html>
