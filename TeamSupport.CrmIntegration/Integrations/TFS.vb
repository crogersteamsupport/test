Imports Newtonsoft.Json.Linq
Imports System.ComponentModel
Imports System.IO
Imports System.Net
Imports System.Reflection
Imports System.Text
Imports TeamSupport.Data
Imports Newtonsoft.Json

Namespace TeamSupport
    Namespace CrmIntegration
        Public Class TFS
            Inherits Integration

            Private _baseURI As String
            Private _encodedCredentials As String
            'Private _issueTypeFieldsList As Dictionary(Of IssueTypeFields, JObject) = New Dictionary(Of IssueTypeFields, JObject)
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
                '_issueTypeFieldsList = New Dictionary(Of IssueTypeFields, JObject)

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

                If CRMLinkRow.Username Is Nothing OrElse CRMLinkRow.Password Is Nothing Then
                    result = False
                    AddLog("Username and or Password are missing and they are required to sync.")
                Else
                    _encodedCredentials = DataUtils.GetEncodedCredentials(CRMLinkRow.Username, CRMLinkRow.Password)
                End If

                'Make sure credentials are good
                If (result) Then
                    'Try
                    '    'Dim jiraClient As JiraClient = New JiraClient(_baseURI.Replace("/rest/api/latest", ""), CRMLinkRow.Username, CRMLinkRow.Password)
                    '    'Dim serverInfo As ServerInfo = jiraClient.GetServerInfo()
                    '    AddLog("Tfs credentials ok.")
                    'Catch jiraEx As JiraClientException
                    '    result = False
                    '    _exception = New IntegrationException(jiraEx.InnerException.Message, jiraEx)
                    'End Try
                End If

                Return result
            End Function

            Private Function SyncTickets() As Boolean
                Dim numberOfWorkItemsToPullAsTickets As Integer = 0
                Dim ticketLinkToTFS As TicketLinkToTFS = New TicketLinkToTFS(User)
                Dim issuesToPullAsTickets As List(Of JObject) = New List(Of JObject)

                ticketLinkToTFS.LoadByCrmLinkId(CRMLinkRow.CRMLinkID, True)

                If ticketLinkToTFS.Any AndAlso ticketLinkToTFS.Count > 0 Then
                    Try
                        issuesToPullAsTickets = GetWorkItemsToPullAsTickets(ticketLinkToTFS, numberOfWorkItemsToPullAsTickets)
                    Catch ex As Exception
                        'AddLog("GetWorkItemsToPullAsTickets with POST failed, using old version now.")
                        'issuesToPullAsTickets = New List(Of JObject)
                        'numberOfIssuesToPullAsTickets = 0
                        'Try
                        '    issuesToPullAsTickets = GetIssuesToPullAsTickets(numberOfIssuesToPullAsTickets)
                        'Catch exception As Exception
                        Log.Write(exception.Message)
                            Log.Write(exception.StackTrace)
                            _exception = New IntegrationException(exception.Message, exception)
                            Return False
                        'End Try
                    End Try
                End If

                'Continue here
                'Dim ticketsLinksToJiraToPushAsIssues As TicketLinkToJira = Nothing
                'Dim ticketsToPushAsIssues As TicketsView = GetTicketsToPushAsIssues(ticketsLinksToJiraToPushAsIssues)
                'Dim allStatuses As TicketStatuses = New TicketStatuses(User)
                'Dim newActionsTypeID As Integer = 0

                'If ticketsToPushAsIssues.Count > 0 OrElse numberOfIssuesToPullAsTickets > 0 Then
                '    allStatuses.LoadByOrganizationID(CRMLinkRow.OrganizationID)
                '    newActionsTypeID = GetNewActionsTypeID(CRMLinkRow.OrganizationID)
                'End If

                'If ticketsToPushAsIssues.Count > 0 Then
                '    PushTicketsAndActionsAsIssuesAndComments(ticketsToPushAsIssues, ticketsLinksToJiraToPushAsIssues, allStatuses, newActionsTypeID)
                'End If

                'If numberOfIssuesToPullAsTickets > 0 Then
                '    For Each batchOfIssuesToPullAsTicket As JObject In issuesToPullAsTickets
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
            ''' <param name="tfsIdList">List of the tfs ids to search. These are the ones we know have been linked, already in TeamSupport in the TicketLinkToTFS table.</param>
            ''' <param name="numberOfWorkItemsToPull">variable that will have the count of the issues found.</param>
            ''' <returns>A list of JObject with the issues found based on the query.</returns>
            Private Function GetWorkItemsToPullAsTickets(ByRef ticketLinkToTFS As TicketLinkToTFS, ByRef numberOfWorkItemsToPull As Integer) As List(Of JObject)
                Dim tfsIdList As List(Of Integer) = ticketLinkToTFS.Where(Function(w) w.CrmLinkID IsNot Nothing).Select(Function(p) CType(p.TFSID, Integer)).ToList()
                Dim result As List(Of JObject) = New List(Of JObject)
                Dim recentClause As String = String.Empty

                If CRMLinkRow.LastLink IsNot Nothing Then
                    recentClause = "updated>-" + GetMinutesSinceLastLink().ToString() + "m"
                Else
                    Dim minutesSinceFirstSyncedTicket As Integer = GetMinutesSinceFirstSyncedTicket()

                    If minutesSinceFirstSyncedTicket > 0 Then
                        recentClause = "updated>-" + minutesSinceFirstSyncedTicket.ToString() + "m"
                    Else
                        Log.Write("No tickets have been synced, therefore no issues to pull exist.")
                        Return result
                    End If
                End If

                'Search only for the tfs ids we have, those are the ones linked and the only ones that we need to check for updates
                Dim URI As String = _baseURI + "/search"
                Dim tfsIdClause As String = String.Empty

                If (tfsIdList.Any() AndAlso tfsIdList.Count > 0) Then
                    tfsIdClause = String.Join(",", tfsIdList.ToArray())
                End If

                If (Not String.IsNullOrEmpty(tfsIdClause)) Then
                    Dim needToGetMore As Boolean = True
                    Dim maxResults As Integer? = Nothing
                    Dim body As StringBuilder = New StringBuilder()
                    Dim startAt As Integer = 0
                    Dim batch As JObject = New JObject
                    Dim listWasScrubbed As Boolean = False

                    While needToGetMore
                        listWasScrubbed = False
                        body.Append("{")
                        body.Append(String.Format("""jql"": ""id IN ({0}) AND {1} ORDER BY updated asc"",", tfsIdClause, recentClause))
                        body.Append("""fields"": [""*all""],")
                        body.Append(String.Format("""startAt"": {0}", startAt))
                        body.Append("}")

                        Try
                            'batch = GetAPIJObject(URI, "POST", body.ToString())
                        Catch webEx As WebException
                            'Dim jiraErrors As JiraErrorsResponse = JiraErrorsResponse.Get(webEx)

                            'If (jiraErrors IsNot Nothing AndAlso jiraErrors.HasErrors) Then
                            '    AddLog("Error when searching issues, scrubbing.")
                            '    ScrubIssues(jiraIdList, jiraErrors, TicketLinkToJira)

                            '    If (jiraIdList IsNot Nothing AndAlso jiraIdList.Count > 0 AndAlso jiraIdList.Any()) Then
                            '        jiraIdClause = String.Join(",", jiraIdList.ToArray())
                            '        startAt = 0
                            '        body.Clear()
                            '        batch = New JObject()
                            '        listWasScrubbed = True
                            '    Else
                            '        Exit While
                            '    End If
                            'Else
                            '    numberOfIssuesToPull = 0
                            '    Throw webEx
                            'End If
                        End Try

                        If (Not listWasScrubbed) Then
                            result.Add(batch)
                            body.Clear()
                            Dim batchTotal As Integer = batch("issues").Count
                            numberOfWorkItemsToPull += batchTotal

                            If maxResults Is Nothing Then
                                maxResults = CType(batch("maxResults").ToString(), Integer?)
                            End If

                            If batchTotal = maxResults Then
                                startAt = numberOfWorkItemsToPull
                            Else
                                needToGetMore = False
                            End If
                        End If
                    End While
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
        End Class
    End Namespace
End Namespace