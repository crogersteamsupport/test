[1mdiff --git a/WebApp/Frames/AdminWorkflow.aspx b/WebApp/Frames/AdminWorkflow.aspx[m
[1mindex be173ef6..7902aa40 100644[m
[1m--- a/WebApp/Frames/AdminWorkflow.aspx[m
[1m+++ b/WebApp/Frames/AdminWorkflow.aspx[m
[36m@@ -5,6 +5,41 @@[m
   <style type="text/css">[m
     body{background:#fff;}[m
   </style>[m
[32m+[m	[32m<script language="javascript" type="text/javascript">[m[41m[m
[32m+[m		[32mfunction OnClientSelectedTicketTypeIndexChanged(sender, eventArgs) {[m[41m[m
[32m+[m			[32mdocument.getElementById('<%= SelectedTicketTypeIndexHidden.ClientID %>').value = sender._selectedIndex;[m[41m[m
[32m+[m		[32m}[m[41m[m
[32m+[m[41m[m
[32m+[m		[32mfunction OnClientSelectedTicketStatusIndexChanged(sender, eventArgs) {[m[41m[m
[32m+[m			[32mdocument.getElementById('<%= SelectedTicketStatusIndexHidden.ClientID %>').value = sender._selectedIndex;[m[41m[m
[32m+[m		[32m}[m[41m[m
[32m+[m[41m[m
[32m+[m		[32mfunction MoveUp(id) {[m[41m[m
[32m+[m			[32mPageMethods.MoveUp(id, function(result) {[m[41m[m
[32m+[m				[32mif (result) {[m[41m[m
[32m+[m					[32m__doPostBack('<%= SelectedTicketTypeIndexHidden.ClientID %>', '');[m[41m[m
[32m+[m				[32m}[m[41m[m
[32m+[m			[32m});[m[41m[m
[32m+[m		[32m}[m[41m[m
[32m+[m[41m[m
[32m+[m		[32mfunction MoveDown(id) {[m[41m[m
[32m+[m			[32mPageMethods.MoveDown(id, function(result) {[m[41m[m
[32m+[m				[32mif (result) {[m[41m[m
[32m+[m					[32m__doPostBack('<%= SelectedTicketTypeIndexHidden.ClientID %>', '');[m[41m[m
[32m+[m				[32m}[m[41m[m
[32m+[m			[32m});[m[41m[m
[32m+[m		[32m}[m[41m[m
[32m+[m[41m[m
[32m+[m		[32mfunction Delete(id) {[m[41m[m
[32m+[m			[32mif (confirm('Are you sure you would like to delete this next ticket status?')) {[m[41m[m
[32m+[m				[32mPageMethods.Delete(id, function(result) {[m[41m[m
[32m+[m					[32mif (result) {[m[41m[m
[32m+[m						[32m__doPostBack('<%= SelectedTicketTypeIndexHidden.ClientID %>', '');[m[41m[m
[32m+[m					[32m}[m[41m[m
[32m+[m				[32m});[m[41m[m
[32m+[m			[32m}[m[41m[m
[32m+[m		[32m}[m[41m[m
[32m+[m	[32m</script>[m[41m[m
 </asp:Content>[m
 <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">[m
   <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" UpdatePanelsRenderMode="Inline">[m
[36m@@ -29,7 +64,7 @@[m
       <div>[m
         Ticket Type:</div>[m
       <div>[m
[31m-        <telerik:RadComboBox ID="cmbTicketTypes" runat="server" AutoPostBack="True">[m
[32m+[m[32m        <telerik:RadComboBox ID="cmbTicketTypes" runat="server" AutoPostBack="True" onclientselectedindexchanged="OnClientSelectedTicketTypeIndexChanged">[m[41m[m
           <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>[m
         </telerik:RadComboBox>[m
       </div>[m
[36m@@ -38,7 +73,7 @@[m
       <div>[m
         Ticket Status:</div>[m
       <div>[m
[31m-        <telerik:RadComboBox ID="cmbStatuses" runat="server" AutoPostBack="True">[m
[32m+[m[32m        <telerik:RadComboBox ID="cmbStatuses" runat="server" AutoPostBack="True" onclientselectedindexchanged="OnClientSelectedTicketStatusIndexChanged">[m[41m[m
           <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>[m
         </telerik:RadComboBox>[m
       </div>[m
[36m@@ -58,7 +93,7 @@[m
       </div>[m
       <div class="groupBodyWrapperDiv">[m
         <div class="groupBodyDiv">[m
[31m-          <telerik:RadGrid ID="gridNext" runat="server" Width="100%" Height="100%" AutoGenerateColumns="False" GridLines="None" OnItemCommand="gridNext_ItemCommand" BorderWidth="0px" ShowHeader="False">[m
[32m+[m[32m          <telerik:RadGrid ID="gridNext" runat="server" Width="100%" Height="100%" AutoGenerateColumns="False" GridLines="None" OnItemCommand="gridNext_ItemCommand" OnItemDataBound="gridNext_ItemDataBound" BorderWidth="0px" ShowHeader="False">[m[41m[m
             <MasterTableView DataKeyNames="TicketNextStatusID" ClientDataKeyNames="TicketNextStatusID">[m
               <RowIndicatorColumn>[m
                 <HeaderStyle Width="20px"></HeaderStyle>[m
[36m@@ -102,6 +137,7 @@[m
     </telerik:RadComboBox>[m
   </telerik:RadToolTip>[m
 </div>[m
[31m-[m
[32m+[m	[32m<asp:HiddenField ID="SelectedTicketTypeIndexHidden" runat="server" Value="0" />[m[41m[m
[32m+[m	[32m<asp:HiddenField ID="SelectedTicketStatusIndexHidden" runat="server" Value="0" />[m[41m[m
 </asp:Content>[m
 [m
[1mdiff --git a/WebApp/Frames/AdminWorkflow.aspx.cs b/WebApp/Frames/AdminWorkflow.aspx.cs[m
[1mindex 4aea4712..5602e26b 100644[m
[1m--- a/WebApp/Frames/AdminWorkflow.aspx.cs[m
[1m+++ b/WebApp/Frames/AdminWorkflow.aspx.cs[m
[36m@@ -1,40 +1,61 @@[m
 ﻿using System;[m
[31m-using System.Collections;[m
[31m-using System.Configuration;[m
[31m-using System.Data;[m
[31m-using System.Linq;[m
[31m-using System.Web;[m
[31m-using System.Web.Security;[m
[31m-using System.Web.UI;[m
[31m-using System.Web.UI.HtmlControls;[m
[32m+[m[32musing System.Web.Services;[m[41m[m
 using System.Web.UI.WebControls;[m
[31m-using System.Web.UI.WebControls.WebParts;[m
[31m-using System.Xml.Linq;[m
 using TeamSupport.Data;[m
 using TeamSupport.WebUtils;[m
 using Telerik.Web.UI;[m
[32m+[m[32musing log4net;[m[41m[m
 [m
 public partial class Frames_AdminWorkflow : BaseFramePage[m
 {[m
[31m-[m
[32m+[m	[32mprivate static readonly ILog _log = LogManager.GetLogger("RollingLogFileAppenderApp");[m[41m[m
[32m+[m	[32mprivate static int _nextStatusSelected = 0;[m[41m[m
   public int SelectedTicketTypeIndex[m
   {[m
[31m-    get { return Settings.Session.ReadInt("AdminWorkflowTicketTypeIndex", 0); }[m
[31m-    set { Settings.Session.WriteInt("AdminWorkflowTicketTypeIndex", value); }[m
[31m-  }[m
[32m+[m		[32mget[m[41m[m
[32m+[m		[32m{[m[41m[m
[32m+[m			[32mint index = 0;[m[41m[m
[32m+[m			[32mint.TryParse(SelectedTicketTypeIndexHidden.Value, out index);[m[41m[m
[32m+[m			[32mreturn index;[m[41m[m
[32m+[m		[32m}[m[41m[m
[32m+[m		[32mset[m[41m[m
[32m+[m		[32m{[m[41m[m
[32m+[m			[32mSelectedTicketTypeIndexHidden.Value = value.ToString();[m[41m[m
[32m+[m		[32m}[m[41m[m
[32m+[m	[32m}[m[41m[m
 [m
   public int SelectedTicketStatusIndex[m
   {[m
[31m-    get { return Settings.Session.ReadInt("AdminWorkflowStatusIndex", 0); }[m
[31m-    set { Settings.Session.WriteInt("AdminWorkflowStatusIndex", value); }[m
[32m+[m		[32mget[m[41m[m
[32m+[m		[32m{[m[41m[m
[32m+[m			[32mint index = 0;[m[41m[m
[32m+[m			[32mint.TryParse(SelectedTicketStatusIndexHidden.Value, out index);[m[41m[m
[32m+[m			[32mreturn index;[m[41m[m
[32m+[m		[32m}[m[41m[m
[32m+[m		[32mset[m[41m[m
[32m+[m		[32m{[m[41m[m
[32m+[m			[32mSelectedTicketStatusIndexHidden.Value = value.ToString();[m[41m[m
[32m+[m		[32m}[m[41m[m
   }[m
 [m
   protected override void OnInit(EventArgs e)[m
   {[m
     base.OnInit(e);[m
[31m-[m
[32m+[m	[32mlog4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "logging.config"));[m[41m[m
     LoadTicketTypes();[m
[31m-    if (cmbTicketTypes.Items.Count > 0)[m
[32m+[m[32m  }[m[41m[m
[32m+[m[41m[m
[32m+[m[32m  protected override void OnLoad(EventArgs e)[m[41m[m
[32m+[m[32m  {[m[41m[m
[32m+[m[32m    base.OnLoad(e);[m[41m[m
[32m+[m[41m[m
[32m+[m[32m    bool isAdmin = UserSession.CurrentUser.IsSystemAdmin;[m[41m[m
[32m+[m[32m    gridNext.Columns[0].Visible = isAdmin;[m[41m[m
[32m+[m[32m    gridNext.Columns[1].Visible = isAdmin;[m[41m[m
[32m+[m[32m    gridNext.Columns[2].Visible = isAdmin;[m[41m[m
[32m+[m[32m    lnkAddStatus.Visible = isAdmin;[m[41m[m
[32m+[m[41m[m
[32m+[m	[32mif (cmbTicketTypes.Items.Count > 0)[m[41m[m
     {[m
       cmbTicketTypes.SelectedIndex = SelectedTicketTypeIndex;[m
       LoadStatuses(int.Parse(cmbTicketTypes.SelectedValue));[m
[36m@@ -49,18 +70,6 @@[m [mpublic partial class Frames_AdminWorkflow : BaseFramePage[m
       cmbStatuses.Items.Clear();[m
     }[m
 [m
[31m-  }[m
[31m-[m
[31m-  protected override void OnLoad(EventArgs e)[m
[31m-  {[m
[31m-    base.OnLoad(e);[m
[31m-[m
[31m-    bool isAdmin = UserSession.CurrentUser.IsSystemAdmin;[m
[31m-    gridNext.Columns[0].Visible = isAdmin;[m
[31m-    gridNext.Columns[1].Visible = isAdmin;[m
[31m-    gridNext.Columns[2].Visible = isAdmin;[m
[31m-    lnkAddStatus.Visible = isAdmin;[m
[31m-[m
     if (SelectedTicketTypeIndex != cmbTicketTypes.SelectedIndex)[m
     {[m
       SelectedTicketTypeIndex = cmbTicketTypes.SelectedIndex;[m
[36m@@ -118,13 +127,20 @@[m [mpublic partial class Frames_AdminWorkflow : BaseFramePage[m
     TicketStatuses statuses = new TicketStatuses(UserSession.LoginUser);[m
     statuses.LoadNotNextStatuses(statusID);[m
 [m
[32m+[m	[32mstring postBackControl = this.Request.Params["__EVENTTARGET"];[m[41m[m
[32m+[m[41m[m
[32m+[m	[32mif (postBackControl.EndsWith("cmbNewStatus"))[m[41m[m
[32m+[m	[32m{[m[41m[m
[32m+[m		[32mint.TryParse(cmbNewStatus.SelectedValue, out _nextStatusSelected);[m[41m[m
[32m+[m	[32m}[m[41m[m
[32m+[m[41m[m
     cmbNewStatus.Items.Clear();[m
     cmbNewStatus.Items.Add(new RadComboBoxItem("[Select a status]", "-1"));[m
[32m+[m[41m[m
     foreach (TicketStatus status in statuses)[m
     {[m
       cmbNewStatus.Items.Add(new RadComboBoxItem(status.Name, status.TicketStatusID.ToString()));[m
     }[m
[31m-[m
   }[m
 [m
   private void LoadNextStatuses(int ticketStatusID)[m
[36m@@ -137,7 +153,8 @@[m [mpublic partial class Frames_AdminWorkflow : BaseFramePage[m
     LoadAvailableStatuses(ticketStatusID);[m
   }[m
 [m
[31m-  protected void gridNext_ItemCommand(object source, GridCommandEventArgs e)[m
[32m+[m	[32m//This does not seem to be working. I could not find a way to have this method fired by the RadGrid control, except only for the RowClick which does not help.[m[41m[m
[32m+[m	[32mprotected void gridNext_ItemCommand(object source, GridCommandEventArgs e)[m[41m[m
   {[m
     if (e.CommandName == RadGrid.DeleteCommandName)[m
     {[m
[36m@@ -161,13 +178,99 @@[m [mpublic partial class Frames_AdminWorkflow : BaseFramePage[m
 [m
   protected void cmbNewStatus_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)[m
   {[m
[31m-    if (cmbNewStatus.SelectedIndex < 1) return;[m
[31m-    TicketNextStatuses ticketNextStatuses = new TicketNextStatuses(UserSession.LoginUser);[m
[31m-    TicketNextStatus ticketNextStatus = ticketNextStatuses.AddNewTicketNextStatus();[m
[31m-    ticketNextStatus.CurrentStatusID = GetSelectedStatusID();[m
[31m-    ticketNextStatus.NextStatusID = int.Parse(cmbNewStatus.SelectedValue);[m
[31m-    ticketNextStatus.Position = ticketNextStatuses.GetMaxPosition(ticketNextStatus.CurrentStatusID) + 1;[m
[31m-    ticketNextStatuses.Save();[m
[31m-    LoadNextStatuses(GetSelectedStatusID());[m
[32m+[m	[32mif (_nextStatusSelected > 0)[m[41m[m
[32m+[m	[32m{[m[41m[m
[32m+[m		[32mTicketNextStatuses ticketNextStatuses = new TicketNextStatuses(UserSession.LoginUser);[m[41m[m
[32m+[m		[32mTicketNextStatus ticketNextStatus = ticketNextStatuses.AddNewTicketNextStatus();[m[41m[m
[32m+[m		[32mticketNextStatus.CurrentStatusID = GetSelectedStatusID();[m[41m[m
[32m+[m		[32mticketNextStatus.NextStatusID = _nextStatusSelected;[m[41m[m
[32m+[m		[32mticketNextStatus.Position = ticketNextStatuses.GetMaxPosition(ticketNextStatus.CurrentStatusID) + 1;[m[41m[m
[32m+[m		[32mticketNextStatuses.Save();[m[41m[m
[32m+[m		[32mLoadNextStatuses(GetSelectedStatusID());[m[41m[m
[32m+[m	[32m}[m[41m[m
   }[m
[32m+[m[41m[m
[32m+[m	[32mprotected void gridNext_ItemDataBound(object sender, GridItemEventArgs e)[m[41m[m
[32m+[m	[32m{[m[41m[m
[32m+[m		[32mif (e.Item is GridDataItem)[m[41m[m
[32m+[m		[32m{[m[41m[m
[32m+[m			[32mint nextStatusId = (int)e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["TicketNextStatusID"];[m[41m[m
[32m+[m[41m[m
[32m+[m			[32mGridDataItem dataItem = (GridDataItem)e.Item;[m[41m[m
[32m+[m			[32mImageButton deleteButton = (ImageButton)dataItem["columnDelete"].Controls[0];[m[41m[m
[32m+[m			[32mdeleteButton.Attributes.Add("onclick", string.Format("Delete('{0}');", nextStatusId));[m[41m[m
[32m+[m			[32mImageButton upButton = (ImageButton)dataItem["columnMoveUp"].Controls[0];[m[41m[m
[32m+[m			[32mupButton.Attributes.Add("onclick", string.Format("MoveUp('{0}');", nextStatusId));[m[41m[m
[32m+[m			[32mImageButton downButton = (ImageButton)dataItem["columnMoveDown"].Controls[0];[m[41m[m
[32m+[m			[32mdownButton.Attributes.Add("onclick", string.Format("MoveDown('{0}');", nextStatusId));[m[41m[m
[32m+[m		[32m}[m[41m[m
[32m+[m	[32m}[m[41m[m
[32m+[m[41m[m
[32m+[m	[32m[WebMethod][m[41m[m
[32m+[m	[32mpublic static bool MoveUp(int nextStatusId)[m[41m[m
[32m+[m	[32m{[m[41m[m
[32m+[m		[32mbool moved = false;[m[41m[m
[32m+[m[41m[m
[32m+[m		[32mif (!UserSession.CurrentUser.IsSystemAdmin) return moved;[m[41m[m
[32m+[m[41m[m
[32m+[m[41m[m
[32m+[m		[32mtry[m[41m[m
[32m+[m		[32m{[m[41m[m
[32m+[m			[32mTicketNextStatuses statuses = new TicketNextStatuses(UserSession.LoginUser);[m[41m[m
[32m+[m			[32mstatuses.MovePositionUp(nextStatusId);[m[41m[m
[32m+[m			[32mmoved = true;[m[41m[m
[32m+[m		[32m}[m[41m[m
[32m+[m		[32mcatch (Exception ex)[m[41m[m
[32m+[m		[32m{[m[41m[m
[32m+[m			[32m_log.ErrorFormat("AdminWorkflow.MoveUp: {0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace);[m[41m[m
[32m+[m			[32mmoved = false;[m[41m[m
[32m+[m		[32m}[m[41m[m
[32m+[m[41m		[m
[32m+[m[41m		[m
[32m+[m		[32mreturn moved;[m[41m[m
[32m+[m	[32m}[m[41m[m
[32m+[m[41m[m
[32m+[m	[32m[WebMethod][m[41m[m
[32m+[m	[32mpublic static bool MoveDown(int nextStatusId)[m[41m[m
[32m+[m	[32m{[m[41m[m
[32m+[m		[32mbool moved = false;[m[41m[m
[32m+[m[41m[m
[32m+[m		[32mif (!UserSession.CurrentUser.IsSystemAdmin) return moved;[m[41m[m
[32m+[m[41m[m
[32m+[m		[32mtry[m[41m[m
[32m+[m		[32m{[m[41m[m
[32m+[m			[32mTicketNextStatuses statuses = new TicketNextStatuses(UserSession.LoginUser);[m[41m[m
[32m+[m			[32mstatuses.MovePositionDown(nextStatusId);[m[41m[m
[32m+[m			[32mmoved = true;[m[41m[m
[32m+[m		[32m}[m[41m[m
[32m+[m		[32mcatch (Exception ex)[m[41m[m
[32m+[m		[32m{[m[41m[m
[32m+[m			[32m_log.ErrorFormat("AdminWorkflow.MoveDown: {0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace);[m[41m[m
[32m+[m			[32mmoved = false;[m[41m[m
[32m+[m		[32m}[m[41m[m
[32m+[m[41m[m
[32m+[m		[32mreturn moved;[m[41m[m
[32m+[m	[32m}[m[41m[m
[32m+[m[41m[m
[32m+[m	[32m[WebMethod][m[41m[m
[32m+[m	[32mpublic static bool Delete(int nextStatusId)[m[41m[m
[32m+[m	[32m{[m[41m[m
[32m+[m		[32mbool deleted = false;[m[41m[m
[32m+[m[41m[m
[32m+[m		[32mif (!UserSession.CurrentUser.IsSystemAdmin) return deleted;[m[41m[m
[32m+[m[41m[m
[32m+[m		[32mtry[m[41m[m
[32m+[m		[32m{[m[41m[m
[32m+[m			[32mTicketNextStatuses statuses = new TicketNextStatuses(UserSession.LoginUser);[m[41m[m
[32m+[m			[32mstatuses.DeleteFromDB(nextStatusId);[m[41m[m
[32m+[m			[32mdeleted = true;[m[41m[m
[32m+[m		[32m}[m[41m[m
[32m+[m		[32mcatch (Exception ex)[m[41m[m
[32m+[m		[32m{[m[41m[m
[32m+[m			[32m_log.ErrorFormat("AdminWorkflow.Delete: {0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace);[m[41m[m
[32m+[m			[32mdeleted = false;[m[41m[m
[32m+[m		[32m}[m[41m[m
[32m+[m[41m[m
[32m+[m		[32mreturn deleted;[m[41m[m
[32m+[m	[32m}[m[41m[m
 }[m
[1mdiff --git a/WebApp/logging.config b/WebApp/logging.config[m
[1mindex 357115c2..e3e18312 100644[m
[1m--- a/WebApp/logging.config[m
[1m+++ b/WebApp/logging.config[m
[36m@@ -11,8 +11,21 @@[m
 				<conversionPattern value="%date{h:mm:ss tt}: %-5level- %message%newline" />[m
 			</layout>[m
 		</appender>[m
[32m+[m		[32m<appender name="RollingLogFileAppenderApp" type="log4net.Appender.RollingFileAppender" >[m
[32m+[m			[32m<file value="Logs\ " />[m
[32m+[m			[32m<appendToFile value="true" />[m
[32m+[m			[32m<lockingModel type="log4net.Appender.RollingFileAppender+MinimalLock" />[m
[32m+[m			[32m<rollingStyle value="Date" />[m
[32m+[m			[32m<datePattern value ="'WebApp_'yyyyMMdd'.log'" />[m
[32m+[m			[32m<staticLogFileName value="false" />[m
[32m+[m			[32m<MaxDateRollBackups value="1" />[m
[32m+[m			[32m<layout type="log4net.Layout.PatternLayout">[m
[32m+[m				[32m<conversionPattern value="%date{h:mm:ss tt}: %-5level- %message%newline" />[m
[32m+[m			[32m</layout>[m
[32m+[m		[32m</appender>[m
 		<root>[m
 			<level value="DEBUG" />[m
 			<appender-ref ref="RollingLogFileAppender" />[m
[32m+[m			[32m<appender-ref ref="RollingLogFileAppenderApp" />[m
 		</root>[m
 	</log4net>[m
\ No newline at end of file[m
