Imports Newtonsoft.Json.Linq
Imports System.ComponentModel
Imports System.IO
Imports System.Net
Imports System.Reflection
Imports System.Text
Imports TeamSupport.Data
Imports Newtonsoft.Json
Imports Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models
Imports TFSLibrary = TeamSupport.ServiceLibrary.TFS

Namespace TeamSupport
    Namespace CrmIntegration
        Public Class TFS
            Inherits Integration

            Private _baseURI As String
            Private _encodedCredentials As String
            Private _tfs As TFSLibrary = New TFSLibrary()
            Private _tfsExceptionMessageFormat As String = "TFS InnerException Message: {0}{1}{2}{2}{2}Jira ErrorResponse: {3}"
            Public Sub New(ByVal crmLinkOrg As CRMLinkTableItem, ByVal crmLog As SyncLog, ByVal thisUser As LoginUser, ByVal thisProcessor As CrmProcessor)
                MyBase.New(crmLinkOrg, crmLog, thisUser, thisProcessor, IntegrationType.Jira)
            End Sub

            Public Overrides Function PerformSync() As Boolean
                Dim Success As Boolean = True

                If ValidateSyncData() Then
                    Success = SyncTickets()
                Else
                    Success = False
                End If

                Return Success
            End Function

            Private Function ValidateSyncData() As Boolean
                Dim result As Boolean = True

                If CRMLinkRow.HostName Is Nothing Then
                    result = False
                    AddLog("HostName is missing and it is required to sync.")
                Else
                    Dim protocol As String = String.Empty
                    If CRMLinkRow.HostName.Length > 4 AndAlso CRMLinkRow.HostName.Substring(0, 4).ToLower() <> "http" Then
                        protocol = "https://"
                    End If
                    _baseURI = protocol + CRMLinkRow.HostName
                End If

                If CRMLinkRow.SecurityToken1 Is Nothing Then
                    result = False
                    AddLog("Security Token is missing and it is required to sync.")
                End If

                '//vv The following is not needed AS OF RIGHT NOW, if I end up using that library that needs the username and pasword then we'll also need to check for those credentials.
                'If CRMLinkRow.Username Is Nothing OrElse CRMLinkRow.Password Is Nothing Then
                '    result = False
                '    AddLog("Username and or Password are missing and they are required to sync.")
                'Else
                '    _encodedCredentials = DataUtils.GetEncodedCredentials(CRMLinkRow.Username, CRMLinkRow.Password)
                'End If

                'Make sure credentials are good
                If (result) Then
                    Try
                        _tfs = New TFSLibrary(_baseURI, CRMLinkRow.SecurityToken1)
                        If (Not String.IsNullOrEmpty(_tfs.GetProjects())) Then
                            AddLog("Tfs credentials ok.")
                        Else
                            AddLog("Tfs credentials didn't work.")
                        End If
                    Catch ex As Exception
                        result = False
                        _exception = New IntegrationException(ex.InnerException.Message, ex)
                    End Try
                End If

                Return result
            End Function

            Private Function SyncTickets() As Boolean
                Dim numberOfWorkItemsToPullAsTickets As Integer = 0
                Dim ticketLinkToTFS As TicketLinkToTFS = New TicketLinkToTFS(User)
                Dim workItemsToPullAsTickets As List(Of JObject) = New List(Of JObject)

                ticketLinkToTFS.LoadByCrmLinkId(CRMLinkRow.CRMLinkID, True)

                If ticketLinkToTFS.Any AndAlso ticketLinkToTFS.Count > 0 Then
                    Try
                        workItemsToPullAsTickets = GetWorkItemsToPullAsTickets(ticketLinkToTFS, numberOfWorkItemsToPullAsTickets)
                    Catch ex As Exception
                        Log.Write(Exception.Message)
                        Log.Write(Exception.StackTrace)
                        _exception = New IntegrationException(Exception.Message, Exception)
                        Return False
                    End Try
                End If

                Dim ticketsLinksToTFSToPushAsWorkItems As TicketLinkToTFS = Nothing
                Dim ticketsToPushAsWorkItems As TicketsView = GetTicketsToPushAsWorkItems(ticketsLinksToTFSToPushAsWorkItems)
                Dim allStatuses As TicketStatuses = New TicketStatuses(User)
                Dim newActionsTypeID As Integer = 0

                If ticketsToPushAsWorkItems.Count > 0 OrElse numberOfWorkItemsToPullAsTickets > 0 Then
                    allStatuses.LoadByOrganizationID(CRMLinkRow.OrganizationID)
                    newActionsTypeID = GetNewActionsTypeID(CRMLinkRow.OrganizationID)
                End If

                If ticketsToPushAsWorkItems.Count > 0 Then
                    PushTicketsAndActionsAsWorkItemsAndComments(ticketsToPushAsWorkItems, ticketsLinksToTFSToPushAsWorkItems, allStatuses, newActionsTypeID)
                End If

                'If numberOfWorkItemsToPullAsTickets > 0 Then
                '    For Each batchOfIssuesToPullAsTicket As JObject In workItemsToPullAsTickets
                '        PullIssuesAndCommentsAsTicketsAndActions(batchOfIssuesToPullAsTicket("issues"), allStatuses, newActionsTypeID)
                '    Next
                'End If

                Return Not SyncError
            End Function

            Private Sub AddLog(ByVal exception As String,
                            Optional ByVal logTo As LogType = LogType.Text,
                            Optional ByRef crmLinkError As CRMLinkError = Nothing,
                            Optional ByRef errorMessage As String = Nothing,
                            Optional ByVal orientation As Orientation = Orientation.OutToJira,
                            Optional ByVal objectType As ObjectType = ObjectType.Unknown,
                            Optional ByVal objectId As Integer = 0,
                            Optional ByVal objectFieldName As String = "",
                            Optional ByVal objectData As String = Nothing,
                            Optional ByVal operationType As OperationType = OperationType.Unknown)

                If (logTo = LogType.Text OrElse logTo = LogType.TextAndReport) Then
                    Log.Write(exception)
                End If

                If (logTo = LogType.Report OrElse logTo = LogType.TextAndReport) Then
                    Try
                        If crmLinkError Is Nothing Then
                            Dim newCrmLinkError As CRMLinkErrors = New CRMLinkErrors(Me.User)
                            crmLinkError = newCrmLinkError.AddNewCRMLinkError()
                            crmLinkError.OrganizationID = CRMLinkRow.OrganizationID
                            crmLinkError.CRMType = CRMLinkRow.CRMType
                            crmLinkError.Orientation = GetDescription(orientation)
                            crmLinkError.ObjectType = GetDescription(objectType)
                            crmLinkError.ObjectID = objectId.ToString()
                            crmLinkError.ObjectFieldName = objectFieldName
                            crmLinkError.ObjectData = objectData
                            crmLinkError.Exception = exception
                            crmLinkError.OperationType = GetDescription(operationType)
                            crmLinkError.ErrorCount = 1
                            crmLinkError.IsCleared = False
                            crmLinkError.ErrorMessage = errorMessage
                            newCrmLinkError.Save()
                        Else
                            crmLinkError.ErrorCount = crmLinkError.ErrorCount + 1
                            crmLinkError.IsCleared = False
                            crmLinkError.ObjectData = objectData
                            crmLinkError.Exception = exception
                            crmLinkError.Collection.Save()
                        End If
                    Catch errorException As Exception
                        Log.Write(errorException.ToString() + errorException.StackTrace)
                    End Try
                End If
            End Sub

            Shared Function GetDescription(ByVal EnumConstant As [Enum]) As String
                Dim fi As FieldInfo = EnumConstant.GetType().GetField(EnumConstant.ToString())
                Dim attr() As DescriptionAttribute = DirectCast(
            fi.GetCustomAttributes(GetType(DescriptionAttribute), False),
            DescriptionAttribute())

                If attr.Length > 0 Then
                    Return attr(0).Description
                Else
                    Return EnumConstant.ToString()
                End If
            End Function

            ''' <summary>
            ''' Search by POSTing the query
            ''' </summary>
            ''' <param name="ticketLinkToTFS">List of the tfs ids to search. These are the ones we know have been linked, already in TeamSupport in the TicketLinkToTFS table.</param>
            ''' <param name="numberOfWorkItemsToPull">variable that will have the count of the issues found.</param>
            ''' <returns>A list of JObject with the issues found based on the query.</returns>
            Private Function GetWorkItemsToPullAsTickets(ByRef ticketLinkToTFS As TicketLinkToTFS, ByRef numberOfWorkItemsToPull As Integer) As List(Of JObject)
                Dim tfsIdList As List(Of Integer) = ticketLinkToTFS.Where(Function(w) w.CrmLinkID IsNot Nothing).Select(Function(p) CType(p.TFSID, Integer)).ToList()
                Dim result As List(Of JObject) = New List(Of JObject)
                Dim recentClause As String = String.Empty
				numberOfWorkItemsToPull = 0

				Dim fields As List(Of String) = New List(Of String)

				'Search only for the tfs ids we have, those are the ones linked and the only ones that we need to check for updates
				If (tfsIdList IsNot Nothing AndAlso tfsIdList.Any()) Then
					Dim workItemsJsonString As String = _tfs.GetWorkItemsBy(tfsIdList, CRMLinkRow.LastLinkUtc)

					If (String.IsNullOrEmpty(workItemsJsonString)) Then
						Dim resultJson As JObject = JObject.Parse(workItemsJsonString)

						'//vv we might end up doing this in batches too. We'll see.
						result.Add(resultJson)

						Dim totalCount As Integer = resultJson("count")
						numberOfWorkItemsToPull += totalCount
					End If
				End If

				Log.Write("Got " + numberOfWorkItemsToPull.ToString() + " Work Items To Pull As Tickets.")
                Return result
            End Function

            Private Function GetMinutesSinceLastLink() As Integer
                Dim datesDifference As TimeSpan = DateTime.UtcNow.Subtract(CRMLinkRow.LastLinkUtc)
                Return datesDifference.TotalMinutes + 30
            End Function

            Private Function GetMinutesSinceFirstSyncedTicket() As Integer
                Dim firstSyncedTicket As Tickets = New Tickets(MyBase.User)
                firstSyncedTicket.LoadFirstTFSSynced(CRMLinkRow.OrganizationID)
                Dim result As Integer = 0
                If firstSyncedTicket.Count > 0 Then
                    Dim datesDifference As TimeSpan = DateTime.UtcNow.Subtract(firstSyncedTicket(0).DateCreatedUtc)
                    result = datesDifference.TotalMinutes + 30
                End If
                Return result
            End Function

            Private Function GetTicketsToPushAsWorkItems(ByRef ticketsLinksToTFSToPushAsWorkItems As TicketLinkToTFS) As TicketsView
                Dim result As New TicketsView(User)
                result.LoadToPushToTFS(CRMLinkRow)
                AddLog("Got " + result.Count.ToString() + " Tickets To Push As Work Items.")

                ticketsLinksToTFSToPushAsWorkItems = New TicketLinkToTFS(User)
				ticketsLinksToTFSToPushAsWorkItems.LoadToPushToTFS(CRMLinkRow)

				Return result
            End Function

            Private Function GetNewActionsTypeID(ByVal organizationID As Integer) As Integer
                Dim result As Integer = 0
                Dim actionTypes As ActionTypes = New ActionTypes(User)
                actionTypes.LoadByOrganizationID(organizationID)

                Dim newActionType As ActionType = actionTypes.FindByName("Comment")
                If newActionType IsNot Nothing Then
                    result = newActionType.ActionTypeID
                Else
                    actionTypes = New ActionTypes(User)
                    actionTypes.LoadByPosition(organizationID, 0)
                    result = actionTypes(0).ActionTypeID
                End If

                Return result
            End Function

            Private Sub PushTicketsAndActionsAsWorkItemsAndComments(ByVal ticketsToPushAsWorkItems As TicketsView,
                                                            ByVal ticketsLinksToTFSToPushAsWorkItems As TicketLinkToTFS,
                                                            ByVal allStatuses As TicketStatuses,
                                                            ByVal newActionsTypeID As Integer)
                Dim URI As String = _baseURI
                Dim attachmentFileSizeLimit As Integer = 0
                Dim attachmentEnabled As Boolean = GetAttachmentEnabled(attachmentFileSizeLimit)
                Dim crmLinkError As CRMLinkError = Nothing
                Dim ticketData As StringBuilder = New StringBuilder()
                Dim TFSProjectName As String = String.Empty
                Dim customMappingFields As New CRMLinkFields(User)
                Dim crmLinkErrors As CRMLinkErrors = New CRMLinkErrors(User)
                Dim crmLinkAttachmentErrors As CRMLinkErrors = New CRMLinkErrors(User)

                'Get the errors only for the tickets to be processed
                crmLinkErrors.LoadByOperationAndObjectIds(CRMLinkRow.OrganizationID,
                                                        CRMLinkRow.CRMType,
                                                        GetDescription(Orientation.OutToTFS),
                                                        GetDescription(ObjectType.Ticket),
                                                        ticketsToPushAsWorkItems.Select(Function(p) p.TicketID.ToString()).ToList(),
                                                        isCleared:=False)

                For Each ticket As TicketsViewItem In ticketsToPushAsWorkItems
                    Dim ticketLinkToTFS As TicketLinkToTFSItem = ticketsLinksToTFSToPushAsWorkItems.FindByTicketID(ticket.TicketID)
                    Dim ticketLinkToTFSVerify As TicketLinkToTFS = New TicketLinkToTFS(User)
                    ticketLinkToTFSVerify.LoadByTicketID(ticket.TicketID)

                    If (ticketLinkToTFSVerify.Count = 0) Then
                        AddLog("The ticket link record has been deleted. Link was removed or cancelled. Doing nothing.")
                        Continue For
                    End If

                    AddLog(String.Format("Processing ticket #{0}. TicketId: {1}", ticket.TicketNumber, ticket.TicketID))
                    customMappingFields = New CRMLinkFields(User)
                    customMappingFields.LoadByObjectTypeAndCustomFieldAuxID(GetDescription(ObjectType.Ticket), CRMLinkRow.CRMLinkID, ticket.TicketTypeID)

                    Dim updateTicketFlag As Boolean = False
                    Dim sendCustomMappingFields As Boolean = False
                    Dim workItemFields As List(Of TFSLibrary.WorkItemField)
                    Dim workItemValues As List(Of TFSLibrary.WorkItemField) = New List(Of TFSLibrary.WorkItemField)

                    Try
                        crmLinkError = crmLinkErrors.FindUnclearedByObjectIDAndFieldName(ticket.TicketID, String.Empty)
                        TFSProjectName = GetProjectName(ticket, crmLinkErrors)
                        workItemFields = GetWorkItemFields(ticket, TFSProjectName, crmLinkError, Orientation.OutToTFS)
                    Catch webEx As WebException
                        AddLog(webEx.ToString() + webEx.StackTrace)
                        Continue For
                    Catch ex As Exception
                        AddLog(String.Format("Exception in PushTicketsAndActionsAsWorkItemsAndComments: {0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace))
                        Continue For
                    End Try

                    Dim workItem As WorkItem = New WorkItem()
                    Dim isNew As Boolean = False

                    'Create new WorkItem
                    If ticketLinkToTFS.TFSID Is Nothing OrElse ticketLinkToTFS.TFSTitle.IndexOf("Error") > -1 Then
                        Try
                            crmLinkError = crmLinkErrors.FindUnclearedByObjectIDAndFieldName(ticket.TicketID, String.Empty)
                            AddLog("No TFS Id. Creating work item...")
                            isNew = True

                            Dim actionDescriptionId As Integer

                            workItemValues = GetTicketData(ticket, workItemFields, TFSProjectName, actionDescriptionId, customMappingFields, crmLinkErrors)
                            ticketData.Append(String.Join(",", workItemValues.Select(Function(p) String.Format("""{0}""=""{1}""", p.name, p.value)).ToArray()))
                            workItem = _tfs.CreateWorkItem(workItemValues, TFSProjectName, ticket.TicketTypeName)

                            'We don't know if the workItem's type has Description or not, so we will always add a Comment with it too.
                            If (workItem IsNot Nothing AndAlso workItem.Id > 0) Then
                                _tfs.CreateComment(workItem.Id, workItemValues.Where(Function(f) f.name = "Description").FirstOrDefault().value)
                            End If

                            updateTicketFlag = True
                            sendCustomMappingFields = CRMLinkRow.IncludeIssueNonRequired

                            'Check if Ticket Description Action has Attachment
                            If (attachmentEnabled AndAlso actionDescriptionId > 0) Then
                                Dim actionDescriptionAttachment As Data.Attachment = Attachments.GetAttachment(User, actionDescriptionId)
                                'The Action Description should always be 1, if for any reason this is not the case call: Actions.GetActionPosition(User, actionDescriptionId)
                                Dim actionPosition As Integer = 1
                                PushAttachments(actionDescriptionId, ticket.TicketNumber, workItem, attachmentFileSizeLimit, actionPosition)
                            End If

                            ClearCrmLinkError(crmLinkError)
                            'Catch webEx As WebException
                            '	'        Dim jiraErrors As JiraErrorsResponse = JiraErrorsResponse.Get(webEx)
                            '	Dim TFSErrors As String
                            '	'        If (jiraErrors IsNot Nothing AndAlso jiraErrors.HasErrors) Then
                            '	If (TFSErrors IsNot Nothing) Then
                            '		AddLog(String.Format(_tfsExceptionMessageFormat,
                            '							webEx.Message,
                            '							Environment.NewLine,
                            '							vbTab,
                            '							TFSErrors.ToString()))
                            '		AddLog(webEx.StackTrace,
                            '				LogType.Report,
                            '				crmLinkError,
                            '				String.Format("WorkItem was not created due to:{0}{1}", Environment.NewLine, TFSErrors.ToString()),
                            '				Orientation.OutToJira,
                            '				ObjectType.Ticket,
                            '				ticket.TicketID,
                            '				String.Empty,
                            '				ticketData.ToString(),
                            '				OperationType.Create)
                            '	Else
                            '		AddLog(webEx.ToString() + webEx.StackTrace)
                            '	End If

                            '	Continue For
                        Catch ex As Exception
                            Dim updateLinkToTFS As Boolean = True
                            Dim errorMessage As String = String.Empty

                            'Select Case ex.Message
                            '    Case "no project"
                            '        errorMessage = "Error: Specify Project (Product)."
                            '    Case "type mismatch"
                            '        errorMessage = "Error: Specify valid Type."
                            '    Case "project mismatch"
                            errorMessage = "Error: Specify valid Type and/or Project (Product)."
                            '    Case Else
                            '        errorMessage = ex.Message
                            '        updateLinkToTFS = False
                            'End Select

                            If updateLinkToTFS Then
                                ticketLinkToTFS.TFSTitle = errorMessage
                                ticketLinkToTFS.DateModifiedByTFSSync = DateTime.UtcNow()
                                ticketLinkToTFS.Collection.Save()
                            End If

                            AddLog(ex.ToString() + ex.StackTrace,
                                LogType.TextAndReport,
                                crmLinkError,
                                errorMessage,
                                Orientation.OutToJira,
                                ObjectType.Ticket,
                                ticket.TicketID.ToString(),
                                "create",
                                ticketData.ToString(),
                                OperationType.Create)
                            Continue For
                        End Try

                        'WorkItem already exists. 
                        'We are not updating WorkItems, but if this is a second ticket relating to WorkItem we add a link and update ticket fields for TFS
                    ElseIf ticketLinkToTFS.TFSID IsNot Nothing AndAlso Not ticketLinkToTFS.DateModifiedByTFSSync.HasValue Then
                        crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(ticket.TicketID, "update")

                        Try
                            workItem = _tfs.GetWorkItemBy(ticketLinkToTFS.TFSID)
                            updateTicketFlag = True
                            Log.Write("No TFSID. We'll add link.")

                            'An update error could has been caused here or below.
                            'We'll only clear when successfull below.
                        Catch webEx As WebException
                            Dim invalidTFSTitle As String = String.Empty

                            If ticketLinkToTFS.TFSTitle.ToLower().Contains("http") _
                            Or ticketLinkToTFS.TFSTitle.ToLower().Contains(":") _
                            Or ticketLinkToTFS.TFSTitle.ToLower().Contains("/") Then
                                invalidTFSTitle = "The TFS Work Item Title entered is not valid or does not exist in TFS."
                            End If

                            If (String.IsNullOrEmpty(invalidTFSTitle)) Then
                                '        Dim jiraErrors As JiraErrorsResponse = JiraErrorsResponse.Get(webEx)
                                Dim TFSErrors As String
                                '        If (jiraErrors IsNot Nothing AndAlso jiraErrors.HasErrors) Then
                                If (TFSErrors IsNot Nothing) Then
                                    AddLog(String.Format(_tfsExceptionMessageFormat,
                                                        webEx.Message,
                                                        Environment.NewLine,
                                                        vbTab,
                                                        TFSErrors.ToString()))
                                    AddLog(webEx.StackTrace,
                                            LogType.Report,
                                            crmLinkError,
                                            String.Format("Could not get TFS WorkItem:{0}{1}", Environment.NewLine, TFSErrors.ToString()),
                                            Orientation.OutToJira,
                                            ObjectType.Ticket,
                                            ticket.TicketID,
                                            "update",
                                            URI,
                                            OperationType.Create)
                                Else
                                    AddLog(String.Format("{0}{1}{2}", webEx.Message, Environment.NewLine, webEx.StackTrace))
                                End If
                            Else
                                AddLog(String.Format(_tfsExceptionMessageFormat,
                                                    webEx.Message,
                                                    Environment.NewLine,
                                                    vbTab,
                                                    invalidTFSTitle))
                                AddLog(webEx.StackTrace,
                                        LogType.Report,
                                        crmLinkError,
                                        String.Format("Could not get Jira Issue:{0}{1}", Environment.NewLine, invalidTFSTitle),
                                        Orientation.OutToJira,
                                        ObjectType.Ticket,
                                        ticket.TicketID,
                                        "update",
                                        URI,
                                        OperationType.Create)
                            End If

                            Continue For
                        Catch ex As Exception
                            AddLog(String.Format("{0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace))
                            Continue For
                        End Try
                    Else
                        Log.Write("Already linked. Doing nothing.")
                    End If

                    If updateTicketFlag Then
                        crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(ticket.TicketID, "update")

                        Try
                            Dim creatorName As String = "TeamSupport"

                            If ticketLinkToTFS.CreatorID IsNot Nothing AndAlso ticketLinkToTFS.CreatorID > 0 Then
                                Dim creator As Users = New Users(User)
                                creator.LoadByUserID(ticketLinkToTFS.CreatorID)
                                creatorName = String.Format("{0}.{1} {2} linked", creator(0).FirstName.Substring(0, 1),
                                                                                If(String.IsNullOrEmpty(creator(0).MiddleName), "", " " + creator(0).MiddleName.Substring(0, 1) + "."),
                                                                                creator(0).LastName)
                            End If

                            'Add the TS hyperlink to the workitem
                            Dim ticketName As String = DataUtils.GetJsonCompatibleString(HtmlUtility.StripHTML(HtmlUtility.StripHTMLUsingAgilityPack(ticket.Name)))
                            Dim domain As String = SystemSettings.ReadStringForCrmService(User, "AppDomain", "https://app.teamsupport.com")
                            Dim remoteLink As String = String.Format("{0}/Ticket.aspx?ticketid={1}", domain, ticket.TicketID.ToString())

                            _tfs.CreateTeamSupportHyperlink(workItem.Id, remoteLink, String.Format("{0} Ticket #{1} - {2}", creatorName, ticket.TicketNumber, ticketName))
                            ticketLinkToTFS.TFSID = workItem.Id

                            If (workItem.Fields.Where(Function(w) w.Key = "System.Title").Any()) Then
                                ticketLinkToTFS.TFSTitle = workItem.Fields.Where(Function(w) w.Key = "System.Title").Select(Function(p) p.Value)(0)
                            Else
                                ticketLinkToTFS.TFSTitle = ticketName
                            End If

                            If (workItem.Fields.Where(Function(w) w.Key = "System.Title").Any()) Then
                                ticketLinkToTFS.TFSState = workItem.Fields.Where(Function(w) w.Key = "System.State").Select(Function(p) p.Value)(0)
                            Else
                                ticketLinkToTFS.TFSState = "System.State not found in workitem" 'ToDo //vv what to do if the System.Title is not found?
                            End If

                            ticketLinkToTFS.TFSURL = workItem.Url

                            If CRMLinkRow.UpdateStatus Then
                                Dim newStatus As TicketStatus = allStatuses.FindByName(ticketLinkToTFS.TFSState, ticket.TicketTypeID)

                                If newStatus IsNot Nothing Then
                                    Dim updateTicket As Tickets = New Tickets(User)
                                    updateTicket.LoadByTicketID(ticket.TicketID)
                                    updateTicket(0).TicketStatusID = newStatus.TicketStatusID
                                    ticketLinkToTFS.DateModifiedByTFSSync = DateTime.UtcNow
                                    updateTicket.Save()
                                    AddLog(String.Format("Updated status with linked work item state: {0} ({1})", newStatus.Name, newStatus.TicketStatusID))
                                Else
                                    AddLog(String.Format("The Work Item State '{0}' in TFS does not exist for the Ticket Type {1}.", ticketLinkToTFS.TFSState, ticket.TicketTypeName),
                                        LogType.TextAndReport,
                                        crmLinkError,
                                        String.Format("State was not updated because Work Item State '{0}' does not exist in the current Ticket statuses.", ticketLinkToTFS.TFSState),
                                        Orientation.OutToJira,
                                        ObjectType.Ticket,
                                        ticket.TicketID.ToString(),
                                        "update",
                                        "Status",
                                        OperationType.Update)
                                End If
                            Else
                                Dim newAction As Actions = New Actions(User)
                                newAction.AddNewAction()
                                newAction(0).ActionTypeID = newActionsTypeID
                                newAction(0).TicketID = ticket.TicketID
                                newAction(0).Description = "Ticket has been synced with TFS' work item " + workItem.Fields.Where(Function(w) w.Key = "System.Title").Select(Function(p) p.Value).FirstOrDefault() + " with state """ + ticketLinkToTFS.TFSState + """."
                                ticketLinkToTFS.DateModifiedByTFSSync = DateTime.UtcNow()
                                newAction.Save()

                                Dim newActionLinkToTFS As ActionLinkToTFS = New ActionLinkToTFS(User)
                                newActionLinkToTFS.AddNewActionLinkToTFSItem()
                                newActionLinkToTFS(0).ActionID = newAction(0).ActionID
                                newActionLinkToTFS(0).TFSID = -1
                                newActionLinkToTFS(0).DateModifiedByTFSSync = ticketLinkToTFS.DateModifiedByTFSSync
                                newActionLinkToTFS.Save()

                                AddLog("Added comment indicating linked work item state.")
                            End If

                            ticketLinkToTFS.Collection.Save()
                            AddLog("Updated ticketLinkToTFS fields for ticket")

                            ClearCrmLinkError(crmLinkError)
                        Catch ex As Exception
                            AddLog(ex.ToString() + ex.StackTrace,
                                LogType.Report,
                                crmLinkError,
                                "Error creating the RemoteLink in the WorkItem. " + ex.Message,
                                Orientation.OutToJira,
                                ObjectType.Ticket,
                                ticket.TicketID.ToString(),
                                "update",
                                Nothing,
                                OperationType.Update)
                        End Try
                    End If

                    PushActionsAsComments(ticket.TicketID, ticket.TicketNumber, workItem, attachmentEnabled, attachmentFileSizeLimit)

                    If sendCustomMappingFields Then
                        'We are now updating the custom mapping fields. We do a call per field to minimize the impact of invalid values attempted to be assigned.
                        If workItemFields IsNot Nothing Then
                            For Each field As TFSLibrary.WorkItemField In workItemFields
                                UpdateWorkItemField(workItem.Id, customMappingFields, ticket, field, crmLinkErrors, URI)
                            Next
                        End If
                    ElseIf isNew Then
                        AddLog("Include Non-Required Fields On Issue Creation: Off. Only creating issue with required fields.")
                    End If
                Next
            End Sub

            Private Function GetAttachmentEnabled(ByRef attachmentFileSizeLimit As Integer) As String
                Dim result As Boolean = False

                'ToDo //vv It looks like TFS does not have a enable/disable for attachments, so it'll be true here until we find if TFS has this check. We will not set the attachmentFileSizeLimit yet either. However, there is a limit of 100 attachments per work item, we could check on this at some point.
                result = True
                Log.Write("Attachment enabled is " + result.ToString())

                Return result
            End Function

            Private Function GetProjectName(ByVal ticket As TicketsViewItem, ByVal crmLinkErrors As CRMLinkErrors) As String
                Dim TFSProjectName As String = CRMLinkRow.DefaultProject
                Dim crmLinkError As CRMLinkError = crmLinkErrors.FindUnclearedByObjectIDAndFieldName(ticket.TicketID, "ProjectName")

                If CRMLinkRow.AlwaysUseDefaultProjectKey Then
                    Log.Write(String.Format("Using Default Project Name ""{0}""", TFSProjectName))
                Else
                    Dim ticketProductVersion As ProductVersion

                    If Not ticket.ReportedVersionID Is Nothing AndAlso ticket.ReportedVersionID > 0 Then
                        ticketProductVersion = ProductVersions.GetProductVersion(User, ticket.ReportedVersionID)
                    End If

                    If Not ticket.ReportedVersionID Is Nothing AndAlso Not ticketProductVersion Is Nothing AndAlso Not String.IsNullOrEmpty(ticketProductVersion.TFSProjectName) Then
                        TFSProjectName = ticketProductVersion.TFSProjectName
                        Log.Write(String.Format("TFS Project Name ""{0}"" from Product Version {1}", TFSProjectName, ticket.ReportedVersion))
                    Else
                        Dim ticketProduct As Product

                        If Not ticket.ProductID Is Nothing Then
                            ticketProduct = Products.GetProduct(User, ticket.ProductID)

                            If Not String.IsNullOrEmpty(ticketProduct.TFSProjectName) Then
                                TFSProjectName = ticketProduct.TFSProjectName
                                Log.Write(String.Format("TFS Project Name ""{0}"" from Product {1}", TFSProjectName, ticket.ProductName))
                            Else
                                TFSProjectName = ticket.ProductName
                                Log.Write(String.Format("TFS Project Name ""{0}"" like Product Name", TFSProjectName))
                            End If
                        Else
                            TFSProjectName = CRMLinkRow.DefaultProject
                            Log.Write(String.Format("Default Project Name ""{0}"" to be used as TFS Project Name after not found in ProductVersion, Product, or Product Name", TFSProjectName))
                        End If
                    End If
                End If

                If String.IsNullOrEmpty(TFSProjectName) Then
                    Dim message As String = If(CRMLinkRow.AlwaysUseDefaultProjectKey, "AlwaysUseDefaultProjectKey but no Default Project.", "Couldn't find a TFS Project Name in ProductVersion, Product, Product Name in Ticket, or Default Project to use for integration.")
                    AddLog(message,
                        LogType.TextAndReport,
                        crmLinkError,
                        "Error attempting to get TFS project.",
                        Orientation.OutToJira,
                        ObjectType.Ticket,
                        ticket.TicketID,
                        "ProjectName",
                        Nothing,
                        OperationType.Unknown)
                    Dim ex As Exception = New Exception(message)
                    Throw ex
                End If

                ClearCrmLinkError(crmLinkError)

                Return TFSProjectName
            End Function

            Private Function ClearCrmLinkError(ByRef crmLinkError As CRMLinkError)
                If crmLinkError IsNot Nothing Then
                    crmLinkError.IsCleared = True
                    crmLinkError.DateModified = DateTime.UtcNow()
                    crmLinkError.Collection.Save()
                End If
            End Function

            Private Function GetWorkItemFields(ByVal ticket As TicketsViewItem,
                                        ByVal TFSProjectName As String,
                                        ByRef crmLinkError As CRMLinkError,
                                        ByVal orientation As Orientation) As List(Of TFSLibrary.WorkItemField) '//vv As JObject
                Dim workItemTypeName As String = ticket.TicketTypeName
                Dim result As JObject = Nothing
                Dim fields As List(Of TFSLibrary.WorkItemField)

                workItemTypeName = Replace(workItemTypeName, " ", "+")
                TFSProjectName = Replace(TFSProjectName, " ", "+")

                Try
                    fields = _tfs.WorkItemsFields

                    If fields Is Nothing OrElse Not fields.Any Then
                        '//vv TFS api does not return fields per type, it just returns ALL fields.
                        AddLog("TFS did not return any work item fields.",
                            LogType.Report,
                            crmLinkError,
                            "TFS did not return any work item fields.",
                            orientation,
                            ObjectType.Ticket,
                            ticket.TicketID,
                            "",
                            Nothing,
                            OperationType.Unknown)
                        AddLog("TFS did not return any work item fields.")
                    Else
                        'ToDo //vv ?? result = JObject.Parse(fields)
                        ClearCrmLinkError(crmLinkError)
                    End If
                Catch ex As Exception
                    AddLog(String.Format("Exception rised attempting to get fields for work items of project {0}{1}{2}{1}", TFSProjectName, Environment.NewLine, ex.Message))
                    Throw New Exception("project mismatch")
                End Try

                '//vv Return result
                Return fields
            End Function

            Private Function GetAPIJObject(ByVal URI As String, ByVal verb As String, ByVal body As String) As JObject
                AddLog(String.Format("{0} URI: {1}", verb, URI))

                If verb <> "GET" AndAlso Not String.IsNullOrEmpty(body) Then
                    AddLog("body: " + body)
                End If

                Dim result As JObject

                Using response As HttpWebResponse = MakeHTTPRequest(_encodedCredentials, URI, verb, "application/json", Client, body)
                    Dim responseReader As New StreamReader(response.GetResponseStream())
                    result = JObject.Parse(responseReader.ReadToEnd)
                    responseReader.Close()
                    response.Close()

                    AddLog("responseReader and response closed. Exiting Using.")
                End Using

                Return result
            End Function

            Private Function MakeHTTPRequest(
        ByVal encodedCredentials As String,
        ByVal URI As String,
        ByVal method As String,
        ByVal contentType As String,
        ByVal userAgent As String,
        ByVal body As String) As HttpWebResponse

                Dim request As HttpWebRequest = WebRequest.Create(URI)
                request.Headers.Add("Authorization", "Basic " + encodedCredentials)
                request.Method = method
                request.ContentType = contentType
                request.UserAgent = userAgent

                If CRMLinkRow.OrganizationID = 869700 OrElse CRMLinkRow.OrganizationID = 794765 OrElse CRMLinkRow.OrganizationID = 881342 Then
                    request.Timeout = 600000
                ElseIf CRMLinkRow.OrganizationID = 1081853 Then
                    request.Timeout = 180000
                Else
                    request.Timeout = 120000
                End If

                If method.ToUpper = "POST" OrElse method.ToUpper = "PUT" Then
                    Dim bodyByteArray = UTF8Encoding.UTF8.GetBytes(body)
                    request.ContentLength = bodyByteArray.Length

                    Using requestStream As Stream = request.GetRequestStream()
                        requestStream.Write(bodyByteArray, 0, bodyByteArray.Length)
                        requestStream.Close()
                        Log.Write("requestStream closed. Exiting Using.")
                    End Using
                End If

                Return request.GetResponse()
            End Function

            Private Function GetTicketData(ByVal ticket As TicketsViewItem,
                                        ByVal workItemFields As List(Of TFSLibrary.WorkItemField),
                                        ByVal TFSProjectName As String,
                                        ByRef actionDescriptionId As Integer,
                                        ByRef customMappingFields As CRMLinkFields,
                                        ByRef crmLinkErrors As CRMLinkErrors) As List(Of TFSLibrary.WorkItemField)
                Dim workItemValues As List(Of TFSLibrary.WorkItemField) = New List(Of TFSLibrary.WorkItemField)
                Dim fieldValue As TFSLibrary.WorkItemField

                fieldValue = workItemFields.Where(Function(w) w.name = "Title").FirstOrDefault
                fieldValue.value = DataUtils.GetJsonCompatibleString(HtmlUtility.StripHTML(HtmlUtility.StripHTMLUsingAgilityPack(ticket.Name)))
                workItemValues.Add(fieldValue)

                Dim actionDescription As Action = Actions.GetTicketDescription(User, ticket.TicketID)
                actionDescriptionId = actionDescription.ActionID

                Dim addLines As Boolean = True
                Dim description As String = HtmlUtility.StripHTML(HtmlUtility.StripHTMLUsingAgilityPack(actionDescription.Description), addLines)
                fieldValue = workItemFields.Where(Function(w) w.name = "Description").FirstOrDefault
                fieldValue.value = description
                workItemValues.Add(fieldValue)

                Dim customField As StringBuilder = New StringBuilder()
                'ToDo //vv ?? customField = BuildRequiredFields(ticket, fields, customMappingFields, crmLinkErrors)

                Return workItemValues
            End Function

            Private Function BuildRequiredFields(ByVal ticket As TicketsViewItem,
                                            ByRef fields As JObject,
                                            ByRef customMappingFields As CRMLinkFields,
                                            ByRef crmLinkErrors As CRMLinkErrors)
                Dim result As StringBuilder = New StringBuilder()
                Dim crmLinkError As CRMLinkError

                Try
                    If (fields IsNot Nothing) Then
                        Dim requiredFields As List(Of String) = New List(Of String)()

                        For Each field As KeyValuePair(Of String, JToken) In fields
                            If field.Value("required") Then
                                If field.Key = "summary" OrElse field.Key = "workitemtype" OrElse field.Key = "project" OrElse field.Key = "description" Then
                                    Continue For
                                End If

                                Dim value As String = Nothing
                                Dim fieldName As String = field.Value("name").ToString()
                                Dim notIncludedMessage As String = String.Empty
                                crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(ticket.TicketID.ToString(), fieldName)
                                Dim cRMLinkField As CRMLinkField = customMappingFields.FindByCRMFieldName(fieldName)
                                requiredFields.Add(fieldName)

                                If cRMLinkField IsNot Nothing Then
                                    If cRMLinkField.CustomFieldID IsNot Nothing Then
                                        Dim findCustom As New CustomValues(User)
                                        findCustom.LoadByFieldID(cRMLinkField.CustomFieldID, ticket.TicketID)
                                        If findCustom.Count > 0 Then
                                            'value = GetDataLineValue(field.Key.ToString(), field.Value("schema")("custom"), findCustom(0).Value)
                                            value = GetDataLineValue(field.Key.ToString(), field.Value, findCustom(0).Value)
                                        Else
                                            notIncludedMessage = GetFieldNotIncludedMessage(ticket.TicketID, fieldName, findCustom.Count = 0)
                                        End If
                                    ElseIf cRMLinkField.TSFieldName IsNot Nothing Then
                                        If ticket.Row(cRMLinkField.TSFieldName) IsNot Nothing Then
                                            'value = GetDataLineValue(field.Key.ToString(), field.Value("schema")("custom"), ticket.Row(cRMLinkField.TSFieldName))
                                            value = GetDataLineValue(field.Key.ToString(), field.Value, ticket.Row(cRMLinkField.TSFieldName))
                                        Else
                                            notIncludedMessage = GetFieldNotIncludedMessage(ticket.TicketID, fieldName, ticket.Row(cRMLinkField.TSFieldName) Is Nothing)
                                        End If
                                    Else
                                        AddLog("TicketID " + ticket.TicketID.ToString() + "'s field '" + fieldName + "' was not included because custom field " +
                                            cRMLinkField.CRMFieldID.ToString() + " CustomFieldID and TSFieldName are null.")
                                    End If
                                End If

                                If value IsNot Nothing Then
                                    If result.ToString().Trim().Length > 0 Then
                                        result.Append(",")
                                    End If

                                    result.Append("""" + field.Key.ToString() + """:" + value)
                                    ClearCrmLinkError(crmLinkError)
                                Else
                                    AddLog("No value found for the required field " + fieldName,
                                    LogType.TextAndReport,
                                    crmLinkError,
                                    If(String.IsNullOrEmpty(notIncludedMessage), "No value found for the required field " + fieldName, notIncludedMessage),
                                    Orientation.OutToJira,
                                    ObjectType.Ticket,
                                    ticket.TicketID,
                                    fieldName,
                                    Nothing,
                                    OperationType.Update)
                                End If
                            End If
                        Next

                        '//Clear required fields errors that are not required anymore
                        crmLinkErrors.ClearRequiredFieldErrors(ticket.TicketID, requiredFields)
                    Else
                        AddLog("Cannot build the required fields because there is no fields returned from Jira for the project and issue type to check.")
                    End If
                Catch ex As Exception
                    Dim requiredField As String = String.Empty
                    If (fields IsNot Nothing) Then
                        requiredField = fields.ToString()
                    End If

                    AddLog("Exception building the required fields: " + requiredField + Environment.NewLine + ex.ToString() + ex.StackTrace)
                End Try

                Return result
            End Function

            Private Function GetDataLineValue(ByVal fieldKey As String, ByVal fieldType As Object, ByVal fieldValue As String, Optional ByVal workItemID As Integer = 0) As String
                Dim result As String = Nothing

                '//vv What cases should we handle? we might need to be filling this over time
                Select Case fieldType.ToLower()
                    Case "select"
                    Case "multiselect"
                    Case "date"
                    Case "datetime"
                        result = Convert.ToDateTime(fieldValue).ToString("'yyyy'-'MM'-'dd'T'HH':'mm':'ss.fff'Z'")
                    Case "float"
                    Case "string"
                    Case "radiobuttons"
                    Case Else
                        result = fieldValue
                End Select

                Return result
            End Function

            Private Function GetFieldNotIncludedMessage(
              ByVal ticketID As Integer,
              ByVal fieldName As String,
              ByVal isNull As Boolean,
              Optional ByVal isNewIssue As Boolean = True,
              Optional ByVal isCreateable As Boolean = True,
              Optional ByVal isUpdateable As Boolean = True) As String

                Dim message As StringBuilder = New StringBuilder("Field '" + fieldName + "' was not included because ")
                If isNull Then
                    message.Append("it was null")
                End If
                If isNull AndAlso Not ((isNewIssue AndAlso isCreateable) OrElse isUpdateable) Then
                    message.Append(" and ")
                End If
                If Not ((isNewIssue AndAlso isCreateable) OrElse isUpdateable) Then
                    message.Append("the field is not updatable.")
                End If

                Return message.ToString()
            End Function

            Private Sub PushAttachments(
            ByVal actionID As Integer,
            ByVal ticketNumber As Integer,
            ByVal workItem As WorkItem,
            ByVal fileSizeLimit As Integer,
            ByVal actionPosition As Integer)

                Dim attachments As Attachments = New Attachments(User)
                attachments.LoadForTFS(actionID)

                Dim crmLinkAttachmentErrors As CRMLinkErrors = New CRMLinkErrors(User)
                crmLinkAttachmentErrors.LoadByOperationAndObjectIds(CRMLinkRow.OrganizationID,
                                                                CRMLinkRow.CRMType,
                                                                GetDescription(Orientation.OutToJira),
                                                                GetDescription(ObjectType.Attachment),
                                                                attachments.Select(Function(p) p.AttachmentID.ToString()).ToList(),
                                                                isCleared:=False)
                Dim updateAttachments As Boolean = False
                Dim crmLinkError As CRMLinkError = Nothing
                Dim attachmentError As String = String.Empty

                For Each attachment As Data.Attachment In attachments
                    crmLinkError = crmLinkAttachmentErrors.FindByObjectIDAndFieldName(attachment.AttachmentID.ToString(), "file")

                    If (Not File.Exists(attachment.Path)) Then
                        attachmentError = "Attachment was not sent as it was not found on server"
                        AddLog(attachmentError,
                            LogType.TextAndReport,
                            crmLinkError,
                            attachmentError,
                            Orientation.OutToJira,
                            ObjectType.Attachment,
                            attachment.AttachmentID,
                            "file",
                            attachment.FileName,
                            OperationType.Create)
                    Else
                        Dim fs = New FileStream(attachment.Path, FileMode.Open, FileAccess.Read)

                        'ToDo //vv attachmentFileSizeLimit is always zero at this point because I have not found if TFS limits the attachments or not.
                        If (fileSizeLimit > 0 AndAlso fs.Length > fileSizeLimit) Then
                            attachmentError = String.Format("Attachment was not sent as its file size ({0}) exceeded the file size limit of {1}", fs.Length.ToString(), fileSizeLimit.ToString())
                            AddLog(attachmentError,
                                LogType.TextAndReport,
                                crmLinkError,
                                attachmentError,
                                Orientation.OutToJira,
                                ObjectType.Attachment,
                                attachment.AttachmentID,
                                "file",
                                attachment.FileName,
                                OperationType.Create)
                        Else
                            Try
                                _tfs.UploadAttachment(workItem.Id, attachment.Path, attachment.FileName)

                                'Dim URIString As String = ""
                                'Dim request As HttpWebRequest = WebRequest.Create(URIString)
                                'request.Headers.Add("Authorization", "Basic " + _encodedCredentials)
                                ''request.Headers.Add("X-Atlassian-Token", "nocheck")
                                'request.Method = "POST"
                                'Dim boundary As String = String.Format("----------{0:N}", Guid.NewGuid())
                                'request.ContentType = String.Format("multipart/form-data; boundary={0}", boundary)
                                'request.UserAgent = Client

                                'Dim content = New MemoryStream()
                                'Dim writer = New StreamWriter(content)
                                'writer.WriteLine("--{0}", boundary)
                                'writer.WriteLine("Content-Disposition: form-data; name=""file""; filename=""{0}""", ("TeamSupport Ticket #" + ticketNumber.ToString() + " action #" + actionPosition.ToString() + " - " + attachment.FileName))
                                'writer.WriteLine("Content-Type: application/octet-stream")
                                'writer.WriteLine()
                                'writer.Flush()
                                'Dim data(fs.Length) As Byte
                                'fs.Read(data, 0, data.Length)
                                'fs.Close()
                                'content.Write(data, 0, data.Length)
                                'writer.WriteLine()
                                'writer.WriteLine("--" + boundary + "--")
                                'writer.Flush()
                                'content.Seek(0, SeekOrigin.Begin)
                                'request.ContentLength = content.Length

                                'Using requestStream As Stream = request.GetRequestStream()
                                '	content.WriteTo(requestStream)
                                '	requestStream.Close()
                                'End Using

                                'Using response As HttpWebResponse = request.GetResponse()
                                '	Log.Write("Attachment """ + attachment.FileName + """ sent.")
                                '	response.Close()
                                'End Using

                                'content.Flush()
                                'content.Close()
                                attachment.SentToTFS = True
                                updateAttachments = True

                                ClearCrmLinkError(crmLinkError)
                            Catch ex As Exception
                                AddLog(String.Format("{0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace),
                                    LogType.TextAndReport,
                                    crmLinkError,
                                    "Attachment could not be sent. " + ex.Message.ToString(),
                                    Orientation.OutToJira,
                                    ObjectType.Attachment,
                                    attachment.AttachmentID,
                                    "file",
                                    attachment.FileName,
                                    OperationType.Create)
                            End Try
                        End If
                    End If
                Next

                If updateAttachments Then
                    attachments.Save()
                End If
            End Sub

            Private Sub AddRemoteLinkInTFS(ByVal workItemID As String, ByVal workItemTitle As String, ByVal ticketID As String, ByVal ticketNumber As String, ByVal ticketName As String, ByVal creatorName As String)
                Dim domain As String = SystemSettings.ReadStringForCrmService(User, "AppDomain", "https://app.teamsupport.com")

                'Try
                Dim globalId As String = "system=" + domain + "/Ticket.aspx?ticketid=&id=" + ticketID
                'Dim jiraClient As JiraClient = New JiraClient(_baseURI.Replace("/rest/api/latest", ""), CRMLinkRow.Username, CRMLinkRow.Password)
                'Dim remoteLink As RemoteLink = New RemoteLink With {.url = domain + "/Ticket.aspx?ticketid=" + ticketID,
                '                                                .title = creatorName + " Ticket #" + ticketNumber,
                '                                                .summary = DataUtils.GetJsonCompatibleString(HtmlUtility.StripHTML(HtmlUtility.StripHTMLUsingAgilityPack(ticketName))),
                '                                                .icon = New Icon With {.title = "TeamSupport Logo",
                '                                                                        .url16x16 = domain + "/vcr/1_6_5/Images/icons/TeamSupportLogo16.png"}}
                'Dim issueRef As IssueRef = New IssueRef With {.id = issueID, .key = issueKey}

                Log.Write("Creating the RemoteLink with data:")
                'Log.Write(JsonConvert.SerializeObject(remoteLink))

                'Dim remoteLinkCreated As RemoteLink = jiraClient.CreateRemoteLink(issueRef, remoteLink, globalId)

                'If remoteLinkCreated IsNot Nothing AndAlso Not String.IsNullOrEmpty(remoteLinkCreated.id) AndAlso remoteLinkCreated.id <> "0" Then
                '    Log.Write("RemoteLink created. Id: " + remoteLinkCreated.id)
                'Else
                '    Log.Write("RemoteLink was not created and no error was sent back from Jira.")
                'End If
                'Catch jiraEx As JiraClientException
                '    AddLog(String.Format(_jiraExceptionMessageFormat,
                '                                    jiraEx.InnerException.Message,
                '                                    Environment.NewLine,
                '                                    vbTab,
                '                                    DirectCast(jiraEx.InnerException, JiraClientException).ErrorResponse))
                '    AddLog("Adding remote link with JiraClient object failed. Using old POST with JObject method.")

                '    Dim remoteLinkData As StringBuilder = New StringBuilder()
                '    remoteLinkData.Append("{")
                '    'Global ID initialized as documentation examples in two parts separated by &. First part is the domain and the second one the id.
                '    remoteLinkData.Append("""globalid"":""system=" + domain + "/Ticket.aspx?ticketid=&id=" + ticketID + """,")
                '    remoteLinkData.Append("""application"":{")
                '    remoteLinkData.Append("""name"":""Team Support""},")
                '    remoteLinkData.Append("""object"":{")
                '    remoteLinkData.Append("""icon"":{")
                '    remoteLinkData.Append("""url16x16"":""" + domain + "/vcr/1_6_5/Images/icons/TeamSupportLogo16.png"",")
                '    remoteLinkData.Append("""title"":""TeamSupport Logo""},")
                '    remoteLinkData.Append("""title"":""" + creatorName + " Ticket #" + ticketNumber + """,")
                '    remoteLinkData.Append("""summary"":""" + DataUtils.GetJsonCompatibleString(HtmlUtility.StripHTML(HtmlUtility.StripHTMLUsingAgilityPack(ticketName))) + """,")
                '    remoteLinkData.Append("""url"":""" + domain + "/Ticket.aspx?ticketid=" + ticketID + """")
                '    remoteLinkData.Append("}")
                '    remoteLinkData.Append("}")

                '    Dim URI As String = _baseURI + "/issue/" + issueID + "/remotelink"
                '    Log.Write("AddRemoteLink URI: " + URI)
                '    Log.Write("AddRemoteLink Data:" + remoteLinkData.ToString())
                '    Try
                '        Dim response As JObject = GetAPIJObject(URI, "POST", remoteLinkData.ToString())
                '    Catch ex As Exception
                '        AddLog(String.Format("{0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace))
                '        Throw ex
                '    End Try
                'End Try
            End Sub

            Private Sub PushActionsAsComments(
                ByVal ticketID As Integer,
                ByVal ticketNumber As Integer,
                ByVal workItem As WorkItem,
                ByVal attachmentEnabled As Boolean,
                ByVal attachmentFileSizeLimit As Integer)

                Dim actionsToPushAsComments As Actions = New Actions(User)
                actionsToPushAsComments.LoadToPushToTFS(CRMLinkRow, ticketID)
                Log.Write("Found " + actionsToPushAsComments.Count.ToString() + " actions to push as comments.")

                Dim actionLinkToTFS As ActionLinkToTFS = New ActionLinkToTFS(User)
                actionLinkToTFS.LoadToPushToTFS(CRMLinkRow, ticketID)

                Dim crmLinkActionErrors As CRMLinkErrors = New CRMLinkErrors(User)
                crmLinkActionErrors.LoadByOperationAndObjectIds(CRMLinkRow.OrganizationID,
                                                        CRMLinkRow.CRMType,
                                                        GetDescription(Orientation.OutToJira),
                                                        GetDescription(ObjectType.Action),
                                                        actionsToPushAsComments.Select(Function(p) p.ActionID.ToString()).ToList(),
                                                        isCleared:=False)
                Dim crmLinkError As CRMLinkError = Nothing

                For Each actionToPushAsComment As Action In actionsToPushAsComments
                    Dim actionLinkToTFSItem As ActionLinkToTFSItem = actionLinkToTFS.FindByActionID(actionToPushAsComment.ActionID)
                    Dim actionPosition As Integer = Actions.GetActionPosition(User, actionToPushAsComment.ActionID)
                    crmLinkError = crmLinkActionErrors.FindByObjectIDAndFieldName(actionToPushAsComment.ActionID.ToString(), String.Empty)
                    Log.Write("Processing actionID: " + actionToPushAsComment.ActionID.ToString())

                    If actionLinkToTFSItem Is Nothing Then
                        Try
                            Dim TFSComment As String = BuildCommentBody(ticketNumber, actionToPushAsComment.Description, actionPosition, actionToPushAsComment.CreatorID)
                            Dim commentId As Integer = _tfs.CreateComment(workItem.Id, TFSComment)
                            Dim newActionLinkToTFS As ActionLinkToTFS = New ActionLinkToTFS(User)
                            Dim newActionLinkToTFSItem As ActionLinkToTFSItem = newActionLinkToTFS.AddNewActionLinkToTFSItem()

                            newActionLinkToTFSItem.ActionID = actionToPushAsComment.ActionID
                            newActionLinkToTFSItem.TFSID = commentId
                            newActionLinkToTFSItem.DateModifiedByTFSSync = DateTime.UtcNow
                            newActionLinkToTFS.Save()
                            Log.Write("Created comment for action")

                            ClearCrmLinkError(crmLinkError)
                        Catch ex As Exception
                            AddLog(ex.ToString() + ex.StackTrace)
                            Continue For
                        End Try
                    Else
                        Try
                            Log.Write("action.TFSID: " + actionLinkToTFSItem.TFSID.ToString())
                            If actionLinkToTFSItem.TFSID <> -1 Then
                                Dim TFSComment As String = BuildCommentBody(ticketNumber, actionToPushAsComment.Description, actionPosition, actionToPushAsComment.CreatorID)
                                'ToDo //vv update a comment. Is it possible?
                                'Dim commentUpdated As Comment = jiraClient.UpdateComment(issueRef, ActionLinkToTFSItem.JiraID, body.ToString())

                                actionLinkToTFSItem.DateModifiedByTFSSync = DateTime.UtcNow
                                Log.Write("updated comment for actionID: " + actionToPushAsComment.ActionID.ToString())
                            End If

                            ClearCrmLinkError(crmLinkError)
                        Catch ex As Exception
                            AddLog(ex.ToString() + ex.StackTrace)
                            Continue For
                        End Try
                    End If
                    Dim test As String = workItem.Fields.Keys.Where(Function(w) w = "System.Title").Select(Function(p) p).ToString()
                    If (attachmentEnabled) Then
                        PushAttachments(actionToPushAsComment.ActionID, ticketNumber, workItem, attachmentFileSizeLimit, actionPosition)
                    End If
                Next

                actionLinkToTFS.Save()
            End Sub

            Private Function BuildCommentBody(ByVal ticketNumber As String, ByVal actionDescription As String, ByVal actionPosition As Integer, creatorId As Integer) As String
                Dim result As StringBuilder = New StringBuilder()
                Dim creatorUser As UsersViewItem = UsersView.GetUsersViewItem(User, creatorId)
                Dim creatorUserName As String = If(creatorUser IsNot Nothing, String.Format(" added by {0} {1}", creatorUser.FirstName, creatorUser.LastName), String.Empty)

                result.Append("TeamSupport ticket #" + ticketNumber.ToString() + " comment #" + actionPosition.ToString() + creatorUserName + ":")
                result.Append(Environment.NewLine)
                result.Append(Environment.NewLine)

                'Extract the code tag contents (if exists) and re-add after the html strip
                Dim codeSamples As New Dictionary(Of Integer, String)
                codeSamples = HtmlCleaner.ExtractCodeSamples(actionDescription)
                Dim addLines As Boolean = True
                actionDescription = HtmlUtility.StripHTML(HtmlUtility.StripHTMLUsingAgilityPack(actionDescription), addLines)
                HtmlCleaner.AddCodeSamples(actionDescription, codeSamples)

                result.Append(actionDescription)

                Return result.ToString()
            End Function

            Private Sub UpdateWorkItemField(ByRef workItemID As Integer,
            ByRef customMappingFields As CRMLinkFields,
            ByRef ticket As TicketsViewItem,
            ByRef field As TFSLibrary.WorkItemField,
            ByRef crmLinkErrors As CRMLinkErrors,
            Optional ByRef URI As String = Nothing)

                Dim updateFieldRequestBody As StringBuilder = New StringBuilder()
                Dim cRMLinkField As CRMLinkField = customMappingFields.FindByCRMFieldName(field.name)

                If cRMLinkField IsNot Nothing Then
                    Dim value As String = Nothing
                    Dim notIncludedMessage As String = String.Empty
                    Dim findCustom As New CustomValues(User)
                    Dim crmLinkError As CRMLinkError = crmLinkErrors.FindByObjectIDAndFieldName(ticket.TicketID, cRMLinkField.TSFieldName)

                    If cRMLinkField.CustomFieldID IsNot Nothing Then
                        findCustom.LoadByFieldID(cRMLinkField.CustomFieldID, ticket.TicketID)

                        If findCustom.Count > 0 Then
                            Dim customValue As String = findCustom(0).Value
                            value = GetDataLineValue(field.name, field.type, customValue)
                        Else
                            Dim customFields As New CustomFields(User)
                            customFields.LoadByCustomFieldID(cRMLinkField.CustomFieldID)
                            Dim isBooleanField As Boolean = customFields(0).FieldType = CustomFieldType.Boolean

                            'If the custom field is boolean and do not have any value then it is False. As seen in the UI
                            If isBooleanField Then
                                value = GetDataLineValue(field.name, field.type, False.ToString())
                            Else
                                notIncludedMessage = GetFieldNotIncludedMessage(ticket.TicketID, field.name, findCustom.Count = 0)
                            End If
                        End If
                    ElseIf cRMLinkField.TSFieldName IsNot Nothing Then
                        If ticket.Row(cRMLinkField.TSFieldName) IsNot Nothing Then
                            value = GetDataLineValue(field.name, field.type, ticket.Row(cRMLinkField.TSFieldName))
                        Else
                            notIncludedMessage = GetFieldNotIncludedMessage(ticket.TicketID, field.name, ticket.Row(cRMLinkField.TSFieldName) Is Nothing)
                        End If
                    Else
                        AddLog("Field '" + field.name + "' was not included because custom field " +
                                cRMLinkField.CRMFieldID.ToString() + " CustomFieldID and TSFieldName are null.")
                    End If

                    If value IsNot Nothing Then
                        Try
                            Dim workItemValues As List(Of TFSLibrary.WorkItemField) = New List(Of TFSLibrary.WorkItemField)

                            field.value = value
                            workItemValues.Add(field)
                            _tfs.UpdateWorkItem(workItemID, workItemValues)

                            ClearCrmLinkError(crmLinkError)
                        Catch ex As Exception
                            AddLog(ex.ToString() + ex.StackTrace,
                                LogType.TextAndReport,
                                crmLinkError,
                                ex.Message,
                                Orientation.OutToJira,
                                ObjectType.Ticket,
                                ticket.TicketID.ToString(),
                                "create",
                                JsonConvert.SerializeObject(field),
                                OperationType.Update)
                        End Try
                    Else
                        AddLog("No value found for the field " + field.name,
                                LogType.TextAndReport,
                                crmLinkError,
                                notIncludedMessage,
                                Orientation.OutToJira,
                                ObjectType.Ticket,
                                ticket.TicketID,
                                field.name,
                                Nothing,
                                OperationType.Update)
                    End If
                End If
            End Sub

            Private Sub PullWorkItemsAndCommentsAsTicketsAndActions(ByVal workItemsToPullAsTickets As JArray, ByVal allStatuses As TicketStatuses, ByVal newActionsTypeID As Integer, ByVal ticketLinkToTFS As TicketLinkToTFS)
                Dim crmLinkErrors As CRMLinkErrors = New CRMLinkErrors(Me.User)
                Dim ticketIds As List(Of Integer) = New List(Of Integer)()

                For i = 0 To workItemsToPullAsTickets.Count - 1
                    ticketIds.AddRange(GetLinkedTicketIDs(workItemsToPullAsTickets(i), ticketLinkToTFS))
                Next

                crmLinkErrors.LoadByOperationAndObjectIds(CRMLinkRow.OrganizationID,
                                                        CRMLinkRow.CRMType,
                                                        GetDescription(Orientation.IntoTeamSupport),
                                                        GetDescription(ObjectType.Ticket),
                                                        ticketIds.Select(Function(p) p.ToString()).ToList(),
                                                        isCleared:=False)

                Dim crmLinkError As CRMLinkError = Nothing
                Dim crmLinkActionErrors As CRMLinkErrors = New CRMLinkErrors(Me.User)
                crmLinkActionErrors.LoadByOperation(CRMLinkRow.OrganizationID,
                                                CRMLinkRow.CRMType,
                                                GetDescription(Orientation.IntoTeamSupport),
                                                GetDescription(ObjectType.Action),
                                                isCleared:=False)

                For i = 0 To workItemsToPullAsTickets.Count - 1
                    Dim newComments As JArray = Nothing

                    For Each ticketID As Integer In GetLinkedTicketIDs(workItemsToPullAsTickets(i), ticketLinkToTFS)
                        Dim updateTicket As Tickets = New Tickets(User)
                        updateTicket.LoadByTicketID(ticketID)

                        If updateTicket.Count > 0 AndAlso updateTicket(0).OrganizationID = CRMLinkRow.OrganizationID Then
                            crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(ticketID.ToString(), String.Empty)

                            Try
                                Dim ticketLinkToTFSItem As TicketLinkToTFSItem = ticketLinkToTFS.FindByTicketID(ticketID)
                                UpdateTicketWithWorkItemData(ticketID, workItemsToPullAsTickets(i), newActionsTypeID, allStatuses, crmLinkError, crmLinkErrors, ticketLinkToTFSItem)
                                ClearCrmLinkError(crmLinkError)
                            Catch ex As Exception
                                AddLog(ex.ToString() + ex.StackTrace,
                                    LogType.Text,
                                    crmLinkError,
                                    "Error when updating ticket with Work Item data.",
                                    Orientation.IntoTeamSupport,
                                    ObjectType.Ticket,
                                    ticketID.ToString(),
                                    String.Empty,
                                    JsonConvert.SerializeObject(workItemsToPullAsTickets(i)),
                                    OperationType.Update)
                            End Try

                            If newComments Is Nothing Then
                                newComments = GetNewComments(workItemsToPullAsTickets(i)("fields")("comment"), ticketID)
                            End If

                            AddNewCommentsInTicket(ticketID, newComments, newActionsTypeID, crmLinkActionErrors)
                        ElseIf updateTicket.Count > 0 Then
                            AddLog("Ticket with ID: """ + ticketID.ToString() + """ belongs to a different organization and was not updated.")
                        Else
                            AddLog("Ticket with ID: """ + ticketID.ToString() + """ was not found to be updated.")
                        End If
                    Next
                Next
            End Sub

            Private Function GetLinkedTicketIDs(ByVal workItem As JObject, ByVal ticketLinkToTFS As TicketLinkToTFS) As List(Of Integer)
                Dim result As List(Of Integer) = New List(Of Integer)()

                For Each ticketLinkToTFSItem As TicketLinkToTFSItem In ticketLinkToTFS
                    Try
                        'If ticketLinkToTFSItem.TFSID = CType(workItem("id"), Integer) Then
                        If ticketLinkToTFSItem.TFSID = Convert.ToInt32(workItem("id")) Then
                            result.Add(ticketLinkToTFSItem.TicketID)
                        End If
                    Catch ex As Exception
                        Log.Write("Exception finding work item id. Skipping work item " + workItem("id").ToString() + ", please review.")
                    End Try
                Next

                Return result
            End Function

            Private Sub UpdateTicketWithWorkItemData(ByVal ticketID As Integer, ByVal workItem As JObject, ByVal newActionsTypeID As Integer, ByVal allStatuses As TicketStatuses, ByRef crmLinkError As CRMLinkError, ByRef crmlinkErrors As CRMLinkErrors, ByVal ticketLinkToTFSItem As TicketLinkToTFSItem)
                Dim updateTicket As Tickets = New Tickets(User)
                updateTicket.LoadByTicketID(ticketID)

                If updateTicket.Count > 0 AndAlso updateTicket(0).OrganizationID = CRMLinkRow.OrganizationID Then
                    Dim ticketTypeId As Integer = 0
                    Dim customFields As New CRMLinkFields(User)
                    Dim allTypes As TicketTypes = New TicketTypes(User)
                    'Dim ticketLinkToTFS As TicketLinkToTFS = New TicketLinkToTFS(User)

                    'ticketLinkToTFS.LoadByTicketID(updateTicket(0).TicketID)
                    allTypes.LoadByOrganizationID(CRMLinkRow.OrganizationID)

                    'For Each field As KeyValuePair(Of String, JToken) In CType(workItem("fields"), JObject)
                    '    'If field.Key.Trim().ToLower() = "issuetype" Then
                    '    Dim issueTypeName As String = GetFieldValue(field)
                    '        Dim ticketType As TicketType = allTypes.FindByName(issueTypeName)

                    '        If ticketType IsNot Nothing Then
                    '            ticketTypeId = allTypes.FindByName(issueTypeName).TicketTypeID
                    '            customFields.LoadByObjectTypeAndCustomFieldAuxID(GetDescription(ObjectType.Ticket), CRMLinkRow.CRMLinkID, ticketTypeId)
                    '        End If

                    '        Exit For
                    '    End If
                    'Next

                    If ticketTypeId = 0 Then
                        customFields.LoadByObjectTypeAndCustomFieldAuxID(GetDescription(ObjectType.Ticket), CRMLinkRow.CRMLinkID, updateTicket(0).TicketTypeID)
                    End If

                    Dim ticketValuesChanged = False
                    Dim workItemStateChangedNotUpdatedInTicket = False
                    Dim ticketView As TicketsView = New TicketsView(User)
                    ticketView.LoadByTicketID(ticketID)
                    Dim tfsProjectName As String = GetProjectName(ticketView(0), crmlinkErrors)
                    Dim workItemFields As List(Of TFSLibrary.WorkItemField) = GetWorkItemFields(ticketView(0), tfsProjectName, crmLinkError, Orientation.IntoTeamSupport)
                    Dim ticketsFieldMap As Tickets = New Tickets(User)

                    For Each field As KeyValuePair(Of String, JToken) In CType(workItem("fields"), JObject)
                        Dim value As String = Nothing
                        Dim cRMLinkField As CRMLinkField 'ToDo //vv need to redo this to use the object properties instead of Jobject. = customFields.FindByCRMFieldName(GetFieldNameByKey(field.Key.ToString(), workItemFields))
                        Dim crmLinkCustomFieldError As CRMLinkError = Nothing

                        Try
                            'Verify the field is mapped or part of the Select Case below (if more added there then add them to this check too)
                            Dim isCustomMappedField As Boolean = cRMLinkField IsNot Nothing _
                                                            OrElse field.Key.Trim().ToLower() = "workitemtype" _
                                                            OrElse field.Key.Trim().ToLower() = "project" _
                                                            OrElse field.Key.Trim().ToLower() = "state"

                            If (isCustomMappedField) Then
                                crmLinkCustomFieldError = crmlinkErrors.FindByObjectIDAndFieldName(ticketID.ToString(), field.Key)
                                value = GetFieldValue(field)
                            Else
                                'Uncomment this line in case more logging is needed to troubleshoot fields not synced
                                'AddLog(String.Format("Issue field {0} is not mapped, so it was not processed.", field.Key))
                                Continue For
                            End If

                            ClearCrmLinkError(crmLinkCustomFieldError)
                        Catch ex As Exception
                            AddLog(ex.ToString() + ex.StackTrace,
                            LogType.TextAndReport,
                            crmLinkCustomFieldError,
                            String.Format("Field: ""{0}"" was not updated because the following exception ocurred getting its value: {1}", field.Key, ex.StackTrace),
                            Orientation.IntoTeamSupport,
                            ObjectType.Ticket,
                            updateTicket(0).TicketID.ToString(),
                            field.Key,
                            Nothing,
                            OperationType.Update)

                            Continue For
                        End Try

                        If cRMLinkField IsNot Nothing Then
                            Try
                                Dim fieldMapItem As FieldMapItem = ticketsFieldMap.FieldMap.Items.Find(Function(c) c.PrivateName = cRMLinkField.TSFieldName)
                                Dim canBeUpdated As Boolean = True

                                If fieldMapItem IsNot Nothing AndAlso (Not fieldMapItem.Insert Or Not fieldMapItem.Update) Then
                                    canBeUpdated = False
                                End If

                                Dim translatedFieldValue As String = value

                                If cRMLinkField.CustomFieldID IsNot Nothing Then
                                    Dim findCustom As New CustomValues(User)
                                    Dim thisCustom As CustomValue

                                    findCustom.LoadByFieldID(cRMLinkField.CustomFieldID, ticketID)

                                    If findCustom.Count > 0 Then
                                        thisCustom = findCustom(0)
                                    Else
                                        thisCustom = (New CustomValues(User)).AddNewCustomValue()
                                        thisCustom.CustomFieldID = cRMLinkField.CustomFieldID
                                        thisCustom.RefID = ticketID
                                    End If

                                    If (IsDBNull(thisCustom.Value) OrElse thisCustom.Value <> translatedFieldValue) And canBeUpdated Then
                                        thisCustom.Value = translatedFieldValue
                                        thisCustom.Collection.Save()
                                        ticketValuesChanged = True
                                    End If
                                ElseIf cRMLinkField.TSFieldName IsNot Nothing Then
                                    Try
                                        If (IsDBNull(updateTicket(0).Row(cRMLinkField.TSFieldName)) OrElse updateTicket(0).Row(cRMLinkField.TSFieldName) <> translatedFieldValue) And canBeUpdated Then
                                            updateTicket(0).Row(cRMLinkField.TSFieldName) = translatedFieldValue
                                            ticketValuesChanged = True
                                        End If
                                    Catch ex As Exception
                                        'When a ticket view field is mapped an exception migth be thrown when looking for it in the Tickets table
                                        'The following is to look for its reference field in the Tickets table (e.g. Severity ? TicketSeverityID) and the related value.
                                        Dim ticketsTableRelatedValue As Integer? = Nothing
                                        Dim ticketsTableRelatedFieldName As String = GetTicketsTableRelatedFieldName(cRMLinkField.TSFieldName, translatedFieldValue, ticketsTableRelatedValue)

                                        If Not String.IsNullOrEmpty(ticketsTableRelatedFieldName) AndAlso Not ticketsTableRelatedValue Is Nothing Then
                                            If IsDBNull(updateTicket(0).Row(ticketsTableRelatedFieldName)) OrElse updateTicket(0).Row(ticketsTableRelatedFieldName) <> ticketsTableRelatedValue Then
                                                updateTicket(0).Row(ticketsTableRelatedFieldName) = ticketsTableRelatedValue
                                                ticketValuesChanged = True
                                            End If
                                        End If
                                    End Try
                                End If
                            Catch mappingException As Exception
                                AddLog("The following exception was caught mapping the ticket field """ &
                                        field.Key &
                                        """ with """ &
                                        cRMLinkField.TSFieldName &
                                        """: " &
                                        mappingException.Message &
                                        " " & mappingException.StackTrace)
                            End Try
                        Else
                            Select Case field.Key.Trim().ToLower()
                                Case "workitemtype"
                                    Dim currentType As TicketType = allTypes.FindByTicketTypeID(updateTicket(0).TicketTypeID)
                                    Dim newType As TicketType = allTypes.FindByName(value)
                                    Dim updateType As Boolean = CRMLinkRow.UpdateTicketType

                                    If updateType AndAlso newType IsNot Nothing AndAlso newType.TicketTypeID <> currentType.TicketTypeID Then
                                        updateTicket(0).TicketTypeID = newType.TicketTypeID
                                        ticketValuesChanged = True
                                    End If
                                Case "project"
                                    Dim allProducts As Products = New Products(User)
                                    allProducts.LoadByOrganizationID(CRMLinkRow.OrganizationID)
                                    Dim newProduct As Product = allProducts.FindByName(value)

                                    If newProduct IsNot Nothing Then
                                        If updateTicket(0).ProductID IsNot Nothing Then
                                            If newProduct.ProductID <> updateTicket(0).ProductID Then
                                                updateTicket(0).ProductID = newProduct.ProductID
                                                ticketValuesChanged = True
                                            End If
                                        Else
                                            updateTicket(0).ProductID = newProduct.ProductID
                                            ticketValuesChanged = True
                                        End If
                                    End If
                                Case "state"
                                    Dim currentStatus As TicketStatus = allStatuses.FindByTicketStatusID(updateTicket(0).TicketStatusID)
                                    Dim newStatus As TicketStatus = allStatuses.FindByName(value, updateTicket(0).TicketTypeID)
                                    Dim isCurrentStatusExcluded As Boolean = False

                                    If (Not String.IsNullOrEmpty(CRMLinkRow.ExcludedTicketStatusUpdate)) Then
                                        Dim exclusionStatusList As List(Of Integer) = New List(Of Integer)

                                        For Each statusExcluded As String In CRMLinkRow.ExcludedTicketStatusUpdate.Split(",")
                                            exclusionStatusList.Add(CInt(statusExcluded))
                                        Next

                                        isCurrentStatusExcluded = exclusionStatusList.Where(Function(p) p = currentStatus.TicketStatusID).Any()
                                    End If

                                    If CRMLinkRow.UpdateStatus _
                                    AndAlso newStatus IsNot Nothing _
                                    AndAlso newStatus.TicketStatusID <> currentStatus.TicketStatusID _
                                    AndAlso Not isCurrentStatusExcluded Then
                                        updateTicket(0).TicketStatusID = newStatus.TicketStatusID
                                        ticketValuesChanged = True
                                    ElseIf Not CRMLinkRow.UpdateStatus Then
                                        If ticketLinkToTFSItem IsNot Nothing AndAlso (ticketLinkToTFSItem.TFSState Is Nothing OrElse ticketLinkToTFSItem.TFSState <> value) Then
                                            Dim fromStatement As String = String.Empty

                                            If ticketLinkToTFSItem.TFSState IsNot Nothing Then
                                                fromStatement = " from """ + ticketLinkToTFSItem.TFSState + """"
                                            End If

                                            Dim newAction As Actions = New Actions(User)
                                            newAction.AddNewAction()
                                            newAction(0).ActionTypeID = newActionsTypeID
                                            newAction(0).TicketID = updateTicket(0).TicketID
                                            newAction(0).Description = "TFS' Work Item " + workItem("title").ToString() + "'s state changed" + fromStatement + " to """ + value + """."

                                            Dim actionLinkToTFS As ActionLinkToTFS = New ActionLinkToTFS(User)
                                            Dim actionLinkToTFSItem As ActionLinkToTFSItem = actionLinkToTFS.AddNewActionLinkToTFSItem()

                                            actionLinkToTFSItem.TFSID = -1
                                            actionLinkToTFSItem.DateModifiedByTFSSync = DateTime.UtcNow()
                                            newAction.Save()
                                            actionLinkToTFSItem.ActionID = newAction(0).ActionID
                                            actionLinkToTFS.Save()
                                            ticketValuesChanged = True
                                        End If
                                    End If

                                    If (ticketLinkToTFSItem.TFSState.ToLower().Trim() <> value.ToLower().Trim()) Then
                                        ticketLinkToTFSItem.TFSState = value
                                        workItemStateChangedNotUpdatedInTicket = True
                                    End If
                            End Select
                        End If
                    Next

                    If ticketValuesChanged Then
                        ticketLinkToTFSItem.DateModifiedByTFSSync = DateTime.UtcNow
                        updateTicket.Save()
                        ticketLinkToTFSItem.Collection.Save()

                        Dim actionLogDescription As String = "Updated Ticket with TFS Work Item Title: '" + workItem("title").ToString() + "' changes."
                        ActionLogs.AddActionLog(User, ActionLogType.Update, ReferenceType.Tickets, updateTicket(0).TicketID, actionLogDescription)
                    Else
                        AddLog("ticketID: " + updateTicket(0).TicketID.ToString() + " values were not updated because have not changed.")

                        If workItemStateChangedNotUpdatedInTicket Then
                            ticketLinkToTFSItem.DateModifiedByTFSSync = DateTime.UtcNow
                            ticketLinkToTFSItem.Collection.Save()
                            AddLog("The ticket status in the TicketLinkToTFS was updated because it changed but was not updated in TeamSupport because either it is setup to be excluded or it does not exist. TFS State:" + ticketLinkToTFSItem.TFSState)
                        End If
                    End If
                Else
                    AddLog("Ticket with ID: """ + ticketID.ToString() + """ was not found to be updated.")
                End If
            End Sub

            Private Function GetFieldValue(ByVal field As KeyValuePair(Of String, JToken)) As String
                Dim result As String = Nothing

                'If field.Key.ToString().ToLower().Contains("customfield") = True Then
                '    If field.Value.HasValues AndAlso TypeOf (field.Value) Is JObject Then
                '        result = field.Value("value").ToString()
                '    ElseIf field.Value.HasValues AndAlso TypeOf (field.Value) Is JArray Then
                '        Try
                '            Dim fieldValue = field.Value(0).ToString()
                '            Dim fieldValueDictionary = fieldValue.Split(","c).Select(Function(kvp) kvp.Split("="c)).ToDictionary(
                '                                                                                                    Function(kvp) kvp(0),
                '                                                                                                    Function(kvp) kvp(1))

                '            If (fieldValueDictionary.Where(Function(p) p.Key.ToLower() = "name").Any()) Then
                '                result = fieldValueDictionary.Where(Function(p) p.Key.ToLower() = "name").Select(Function(p) p.Value).FirstOrDefault()
                '            Else
                '                result = field.Value.ToString()
                '            End If
                '        Catch ex As Exception
                '            Try
                '                'ToDo //vv This probably is a multiselect jira field. For now we are only taking the first value. We need to handle those fields correctly
                '                result = field.Value(0)("value").ToString()
                '            Catch innerEx As Exception
                '                AddLog(String.Format("Unhandled Jira field type {0} with value: {1}{2}", field.Key.ToString(), Environment.NewLine, field.Value(0).ToString()))
                '            End Try

                '        End Try

                '    Else
                '        result = field.Value.ToString()
                '    End If

                '    If result.ToLower() = "yes" Then
                '        result = "True"
                '    ElseIf result.ToLower() = "no" Then
                '        result = "False"
                '    End If
                'Else
                '    Select Case field.Key.ToString().ToLower()
                '        Case "aggregatetimeestimate",
                '    "aggregatetimeoriginalestimate",
                '    "aggregatetimespent",
                '    "created",
                '    "description",
                '    "duedate",
                '    "environment",
                '    "lastviewed",
                '    "resolutiondate",
                '    "summary",
                '    "updated",
                '    "workratio",
                '    "timeestimate",
                '    "timeoriginalestimate",
                '    "timespent"
                '            result = field.Value.ToString()
                '        Case "assignee"
                '            'Because some orgs are mapping to the Assignee Jira field and they seem to be storing the email address then we need to leave it here, for the rest we should use the Name so it also links from TS to Jira when creating the issue
                '            If (CRMLinkRow.OrganizationID = 461956 OrElse CRMLinkRow.OrganizationID = 869700 OrElse CRMLinkRow.OrganizationID = 884116) Then
                '                result = field.Value("emailAddress").ToString()
                '            Else
                '                result = field.Value("name").ToString()
                '            End If
                '        Case "reporter"
                '            'Because some orgs are mapping to the Assignee Jira field and they seem to be storing the email address then we need to leave it here, for the rest we should use the Name so it also links from TS to Jira when creating the issue
                '            If (CRMLinkRow.OrganizationID = 930653 OrElse CRMLinkRow.OrganizationID = 1028984 OrElse CRMLinkRow.OrganizationID = 1136748 OrElse CRMLinkRow.OrganizationID = 995322) Then
                '                result = field.Value("emailAddress").ToString()
                '            Else
                '                result = field.Value("name").ToString()
                '            End If
                '        Case "issuetype", "status", "priority", "resolution"
                '            result = field.Value("name").ToString()
                '        Case "progress", "worklog"
                '            result = field.Value("total").ToString()
                '        Case "project"
                '            result = field.Value("key").ToString()
                '        Case "votes"
                '            result = field.Value("votes").ToString()
                '        Case "watches"
                '            result = field.Value("watchCount").ToString()
                '        Case "timetracking"
                '            result = field.Value("timeSpentSeconds").ToString()
                '        Case "aggregrateprogress"
                '            result = field.Value("progress").ToString()
                '        Case "attachment"
                '            Dim attachmentsArray As JArray = DirectCast(field.Value, JArray)
                '            Dim resultBuilder As StringBuilder = New StringBuilder()
                '            Dim preffix = String.Empty

                '            For i = 0 To attachmentsArray.Count - 1
                '                resultBuilder.Append(preffix)
                '                resultBuilder.Append(attachmentsArray(i)("content").ToString())

                '                If preffix = String.Empty Then
                '                    preffix = ", "
                '                End If
                '            Next

                '            result = resultBuilder.ToString()
                '        Case "labels"
                '            Dim labelsArray As JArray = DirectCast(field.Value, JArray)
                '            Dim resultBuilder As StringBuilder = New StringBuilder()
                '            Dim preffix = String.Empty

                '            For i = 0 To labelsArray.Count - 1
                '                resultBuilder.Append(preffix)
                '                resultBuilder.Append(labelsArray(i).ToString())

                '                If preffix = String.Empty Then
                '                    preffix = ", "
                '                End If
                '            Next

                '            result = resultBuilder.ToString()
                '        Case "issuelinks"
                '            Dim issuelinksArray As JArray = DirectCast(field.Value, JArray)
                '            Dim resultBuilder As StringBuilder = New StringBuilder()
                '            Dim preffix = String.Empty

                '            For i = 0 To issuelinksArray.Count - 1
                '                resultBuilder.Append(preffix)

                '                If issuelinksArray(i)("outwardIssue") IsNot Nothing Then
                '                    resultBuilder.Append(issuelinksArray(i)("outwardIssue")("key").ToString())
                '                Else
                '                    resultBuilder.Append(issuelinksArray(i)("inwardIssue")("key").ToString())
                '                End If

                '                If preffix = String.Empty Then
                '                    preffix = ", "
                '                End If
                '            Next

                '            result = resultBuilder.ToString()
                '        Case "versions", "fixversions"
                '            Dim versionsArray As JArray = DirectCast(field.Value, JArray)
                '            Dim resultBuilder As StringBuilder = New StringBuilder()
                '            Dim preffix = String.Empty

                '            For i = 0 To versionsArray.Count - 1
                '                resultBuilder.Append(preffix)

                '                Try
                '                    resultBuilder.Append(versionsArray(i)("name").ToString())
                '                Catch ex As Exception
                '                    resultBuilder.Append(versionsArray(i)("description").ToString())
                '                End Try

                '                If preffix = String.Empty Then
                '                    preffix = ", "
                '                End If
                '            Next

                '            result = resultBuilder.ToString()
                '        Case "subtasks"
                '            Dim subtasksArray As JArray = DirectCast(field.Value, JArray)
                '            Dim resultBuilder As StringBuilder = New StringBuilder()
                '            Dim preffix = String.Empty

                '            For i = 0 To subtasksArray.Count - 1
                '                resultBuilder.Append(preffix)
                '                resultBuilder.Append(subtasksArray(i)("fields")("summary").ToString())

                '                If preffix = String.Empty Then
                '                    preffix = ", "
                '                End If
                '            Next

                '            result = resultBuilder.ToString()
                '        Case "components"
                '            Dim componentsArray As JArray = DirectCast(field.Value, JArray)
                '            Dim resultBuilder As StringBuilder = New StringBuilder()
                '            Dim preffix = String.Empty

                '            For i = 0 To componentsArray.Count - 1
                '                resultBuilder.Append(preffix)
                '                resultBuilder.Append(componentsArray(i)("name").ToString())

                '                If preffix = String.Empty Then
                '                    preffix = ", "
                '                End If
                '            Next

                '            result = resultBuilder.ToString()
                '    End Select
                'End If

                Return result
            End Function

            Private Function GetFieldNameByKey(ByVal fieldKey As String, ByVal workItemFields As JObject)
                Dim result As StringBuilder = New StringBuilder()

                If workItemFields IsNot Nothing Then
                    For Each field As KeyValuePair(Of String, JToken) In workItemFields
                        If field.Key IsNot Nothing AndAlso fieldKey = field.Key Then
                            result.Append(field.Value("name").ToString())
                            Exit For
                        End If
                    Next
                End If

                If result.Length = 0 Then
                    result.Append(fieldKey)
                End If

                Return result.ToString()
            End Function

            Private Function GetTicketsTableRelatedFieldName(ByVal mappedFieldName As String, ByVal value As String, ByRef ticketsTableRelatedValue As Integer?)
                Dim result As StringBuilder = New StringBuilder()
                Select Case mappedFieldName
                    'This is linked to issue product as long as status and type
                    'Case "ProductName"
                    '  result.Append("ProductID")
                    '  Dim products As Products = New Products(User)
                    '  products.LoadByProductName(CRMLinkRow.OrganizationID, value)
                    '  If products.Count > 0 Then
                    '    ticketsTableRelatedValue = products(0).ProductID
                    '  End If
                    Case "GroupName"
                        result.Append("GroupID")
                        Dim groups As Groups = New Groups(User)
                        groups.LoadByGroupName(CRMLinkRow.OrganizationID, value, 1)
                        If groups.Count > 0 Then
                            ticketsTableRelatedValue = groups(0).GroupID
                        End If
                    Case "UserName"
                        result.Append("UserID")
                        Dim givenUser As Users = New Users(User)
                        givenUser.LoadByName(value, CRMLinkRow.OrganizationID, False, False, False)
                        If givenUser.Count > 0 Then
                            ticketsTableRelatedValue = givenUser(0).UserID
                        End If
                    Case "Severity"
                        result.Append("TicketSeverityID")
                        Dim severity As TicketSeverities = New TicketSeverities(User)
                        severity.LoadByName(CRMLinkRow.OrganizationID, value)
                        If severity.Count > 0 Then
                            ticketsTableRelatedValue = severity(0).TicketSeverityID
                        End If
                End Select
                Return result.ToString()
            End Function

            Private Function GetNewComments(ByVal comments As JObject, ByVal ticketID As Integer) As JArray
                Dim result As JArray = New JArray()

                Dim ticketActionsLinked As ActionLinkToTFS = New ActionLinkToTFS(User)
                ticketActionsLinked.LoadByTicketID(ticketID)

                For i = 0 To comments("comments").Count - 1
                    If GetIsNewComment(comments("comments")(i), ticketActionsLinked) Then
                        result.Add(comments("comments")(i))
                    End If
                Next
                Return result
            End Function

            Private Function GetIsNewComment(ByVal comment As JObject, ByVal ticketActionsLinked As ActionLinkToTFS) As Boolean
                Dim result As Boolean = False

                If comment("visibility") Is Nothing Then
                    If comment("body").ToString().Length < 20 OrElse comment("body").ToString().Substring(0, 20) <> "TeamSupport ticket #" Then
                        Dim pulledComment As ActionLinkToTFSItem = ticketActionsLinked.FindByTFSID(CType(comment("id").ToString(), Integer?))
                        If pulledComment Is Nothing Then
                            result = True
                        End If
                    End If
                End If

                Return result
            End Function

            Private Sub AddNewCommentsInTicket(ByVal ticketID As Integer, ByRef newComments As JArray, ByVal newActionsTypeID As Integer, ByRef crmLinkActionErrors As CRMLinkErrors)
                Dim crmLinkError As CRMLinkError = Nothing

                For i = 0 To newComments.Count - 1
                    crmLinkError = crmLinkActionErrors.FindByObjectIDAndFieldName(newComments(i)("id").ToString(), String.Empty)

                    Try
                        Dim updateActions As Actions = New Actions(User)
                        updateActions.AddNewAction()
                        updateActions(0).TicketID = ticketID
                        updateActions(0).ActionTypeID = newActionsTypeID

                        Dim commentDescription = newComments(i)("body").ToString()
                        Dim firstLine As String = "<p><em>Comment added in TFS {0}{1}</em></p> <p>&nbsp;</p>"
                        Dim author As String = String.Empty

                        Try
                            author = newComments(i)("author")("displayName").ToString()
                        Catch ex As Exception
                            AddLog("The author displayName was not found for the comment.")
                        End Try

                        Dim addedOnTFSString As String = String.Empty

                        Try
                            Dim addedOnTFS As Date = Convert.ToDateTime(newComments(i)("created"))

                            If (DateDiff(DateInterval.Day, Today.Date, addedOnTFS.Date) <> 0) Then
                                addedOnTFSString = addedOnTFS.ToString()
                            End If
                        Catch ex As Exception
                            AddLog("The created date was not found for the comment.")
                        End Try

                        firstLine = String.Format(firstLine,
                                                If(String.IsNullOrEmpty(author), "", "by " + author),
                                                If(String.IsNullOrEmpty(addedOnTFSString), "", " on " + addedOnTFSString))
                        Dim TFSCommentId = CType(newComments(i)("id").ToString(), Integer)
                        commentDescription = firstLine + commentDescription
                        updateActions(0).Description = commentDescription
                        updateActions.ActionLogInstantMessage = "TFS Comment ID: " + TFSCommentId.ToString() + " Created In TeamSupport Action "
                        Dim actionLinkToTFS As ActionLinkToTFS = New ActionLinkToTFS(User)
                        Dim actionLinkToTFSItem As ActionLinkToTFSItem = actionLinkToTFS.AddNewActionLinkToTFSItem()
                        actionLinkToTFSItem.TFSID = TFSCommentId
                        actionLinkToTFSItem.DateModifiedByTFSSync = DateTime.UtcNow
                        updateActions.Save()
                        actionLinkToTFSItem.ActionID = updateActions(0).ActionID
                        actionLinkToTFS.Save()

                        ClearCrmLinkError(crmLinkError)
                    Catch ex As Exception
                        AddLog(ex.ToString() + ex.StackTrace,
                            LogType.TextAndReport,
                            crmLinkError,
                            ex.Message,
                            Orientation.IntoTeamSupport,
                            ObjectType.Action,
                            newComments(i)("id").ToString(),
                            String.Empty,
                            newComments(i)("body").ToString(),
                            OperationType.Create)
                    End Try
                Next
            End Sub
        End Class
    End Namespace
End Namespace