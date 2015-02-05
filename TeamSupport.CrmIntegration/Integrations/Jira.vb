Imports Newtonsoft.Json.Linq
Imports System.Collections.Generic
Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Xml
Imports TeamSupport.Data
Imports Newtonsoft.Json

Namespace TeamSupport
  Namespace CrmIntegration
    Public Class Jira
      Inherits Integration

      Private _baseURI As String
      Private _encodedCredentials As String

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
          result = false
          Log.Write("HostName is missing and it is required to sync.")
        Else
          Dim protocol As String = String.Empty
          If CRMLinkRow.HostName.Length > 4 AndAlso CRMLinkRow.HostName.Substring(0, 4).ToLower() <> "http" Then
            protocol = "https://"
          End If
          _baseURI = protocol + CRMLinkRow.HostName + "/rest/api/latest"
        End If

        If CRMLinkRow.Username Is Nothing OrElse CRMLinkRow.Password Is Nothing Then
          result = false
          Log.Write("Username and or Password are missing and they are required to sync.")
        Else
          _encodedCredentials = DataUtils.GetEncodedCredentials(CRMLinkRow.Username, CRMLinkRow.Password)
        End If

        Return result
      End Function

      Private Function SyncTickets() As Boolean
        Dim ticketsLinksToJiraToPushAsIssues As TicketLinkToJira = Nothing
        Dim ticketsToPushAsIssues As TicketsView  = GetTicketsToPushAsIssues(ticketsLinksToJiraToPushAsIssues)
        Dim numberOfIssuesToPullAsTickets As Integer = 0
        Dim issuesToPullAsTickets As List(Of JObject) = GetIssuesToPullAsTickets(numberOfIssuesToPullAsTickets)
        Dim allStatuses As TicketStatuses = New TicketStatuses(User)
        allStatuses.LoadByOrganizationID(CRMLinkRow.OrganizationID)
        Dim newActionsTypeID As Integer = GetNewActionsTypeID(CRMLinkRow.OrganizationID)

        If ticketsToPushAsIssues.Count > 0 Then
          PushTicketsAndActionsAsIssuesAndComments(ticketsToPushAsIssues, ticketsLinksToJiraToPushAsIssues, allStatuses, newActionsTypeID)
        End If

        If numberOfIssuesToPullAsTickets > 0 Then
          Dim domain As String = SystemSettings.ReadString(User, "AppDomain", "https://app.teamsupport.com")
          Dim indexOfTicketIDInRemoteLink As Integer = domain.Length + 22
          For Each batchOfIssuesToPullAsTicket As JObject In issuesToPullAsTickets
            PullIssuesAndCommentsAsTicketsAndActions(batchOfIssuesToPullAsTicket("issues"), allStatuses, newActionsTypeID, indexOfTicketIDInRemoteLink)
          Next
        End If
        Return Not SyncError
      End Function

      Private Function GetTicketsToPushAsIssues(ByRef ticketsLinksToJiraToPushAsIssues As TicketLinkToJira) As TicketsView
        Dim result As New TicketsView(User)
        result.LoadToPushToJira(CRMLinkRow)
        Log.Write("Got " + result.Count.ToString() + " Tickets To Push As Issues.")

        ticketsLinksToJiraToPushAsIssues = New TicketLinkToJira(User)
        ticketsLinksToJiraToPushAsIssues.LoadToPushToJira(CRMLinkRow)

        Return result
      End Function

      Private Function GetIssuesToPullAsTickets(ByRef numberOfIssuesToPull As Integer) As List(Of JObject)
        Dim result As List(Of JObject) = New List(Of JObject)

        Dim recentClause As String = String.Empty
        If CRMLinkRow.LastLink IsNot Nothing Then
          recentClause = "updated>-" + GetMinutesSinceLastLink().ToString() + "m+"
        Else
          Dim minutesSinceFirstSyncedTicket As Integer = GetMinutesSinceFirstSyncedTicket()
          If minutesSinceFirstSyncedTicket > 0 Then
            recentClause = "updated>-" + minutesSinceFirstSyncedTicket.ToString() + "m+"
          Else
            Log.Write("No tickets have been synced, therefore no issues to pull exist.")
            Return result  
          End If
        End If
        Dim needToGetMore As Boolean = True
        Dim startAt As String = String.Empty
        Dim maxResults As Integer? = Nothing

        While needToGetMore
          Dim URI As String = _baseURI + "/search?jql=" + recentClause + "order+by+updated+asc&fields=*all" + startAt
          Dim batch As JObject = GetAPIJObject(URI, "GET", string.Empty)
          result.Add(batch)

          Dim batchTotal As Integer = batch("issues").Count
          numberOfIssuesToPull += batchTotal

          If maxResults Is Nothing Then
            maxResults = CType(batch("maxResults").ToString(), Integer?)
          End If

          If batchTotal = maxResults Then
            startAt = "&startAt=" + (numberOfIssuesToPull).ToString()
          Else
            needToGetMore = False
          End If
        End While

        Log.Write("Got " + numberOfIssuesToPull.ToString() + " Issues To Pull As Tickets.")
        Return result
      End Function

        Private Function GetMinutesSinceLastLink() As Integer
          Dim datesDifference As TimeSpan = DateTime.UtcNow.Subtract(CRMLinkRow.LastLinkUtc)
          Return datesDifference.TotalMinutes + 30
        End Function

        Private Function GetMinutesSinceFirstSyncedTicket() As Integer
          Dim firstSyncedTicket As Tickets = New Tickets(MyBase.User)
          firstSyncedTicket.LoadFirstJiraSynced(CRMLinkRow.OrganizationID)
          Dim result As Integer = 0
          If firstSyncedTicket.Count > 0 Then
            Dim datesDifference As TimeSpan = DateTime.UtcNow.Subtract(firstSyncedTicket(0).DateCreatedUtc)
            result = datesDifference.TotalMinutes + 30
          End If
          Return result
        End Function

        Private Function GetAPIJObject(ByVal URI As String, ByVal verb As String, ByVal body As String) As JObject
          Log.Write("URI: " + URI)
          Log.Write("verb: " + verb)
          Log.Write("body: " + body)
          Dim response As HttpWebResponse = MakeHTTPRequest(_encodedCredentials, URI, verb, "application/json", Client, body)
          Dim responseReader As New StreamReader(response.GetResponseStream())
          Return JObject.Parse(responseReader.ReadToEnd)
        End Function

        Private Function GetAPIJArray(ByVal URI As String, ByVal verb As String, ByVal body As String) As JArray
          Log.Write("URI: " + URI)
          Log.Write("verb: " + verb)
          Log.Write("body: " + body)
          Dim response As HttpWebResponse = MakeHTTPRequest(_encodedCredentials, URI, verb, "application/json", Client, body)
          Dim responseReader As New StreamReader(response.GetResponseStream())
          Return Jarray.Parse(responseReader.ReadToEnd)
        End Function

        Private Function MakeHTTPRequest(
        ByVal encodedCredentials As string, 
        ByVal URI As String, 
        ByVal method As String, 
        ByVal contentType As String, 
        ByVal userAgent As String, 
        ByVal body As String) As HTTPWebResponse

        Dim bodyByteArray = UTF8Encoding.UTF8.GetBytes(body)
        Dim request As HttpWebRequest = WebRequest.Create(URI)
        request.Headers.Add("Authorization", "Basic " + encodedCredentials)
        request.Method = method
        request.ContentType = contentType
        request.UserAgent = userAgent

        If method.ToUpper = "POST" OrElse method.ToUpper = "PUT" Then
          request.ContentLength = bodyByteArray.Length

          Using requestStream As Stream = request.GetRequestStream()
            requestStream.Write(bodyByteArray, 0, bodyByteArray.Length)
            requestStream.Close()
          End Using
        End If

        Return request.GetResponse()
      End Function

      Private Sub PushTicketsAndActionsAsIssuesAndComments(
        ByVal ticketsToPushAsCases As TicketsView, 
        ByVal ticketsLinksToJiraToPushAsIssues As TicketLinkToJira, 
        ByVal allStatuses As TicketStatuses, 
        ByVal newActionsTypeID As Integer)

        Dim URI As String = _baseURI + "/issue"

        Dim attachmentFileSizeLimit As Integer = 0
        Dim attachmentEnabled As Boolean = GetAttachmentEnabled(attachmentFileSizeLimit)

        Dim crmLinkErrors As CRMLinkErrors = New CRMLinkErrors(Me.User)
        crmLinkErrors.LoadByOperation(CRMLinkRow.OrganizationID, CRMLinkRow.CRMType, "out", "ticket")
        Dim crmLinkError As CRMLinkError = Nothing
        Dim ticketData As StringBuilder = Nothing

        Dim crmLinkActionErrors As CRMLinkErrors = New CRMLinkErrors(Me.User)
        crmLinkActionErrors.LoadByOperation(CRMLinkRow.OrganizationID, CRMLinkRow.CRMType, "out", "action")

        Dim crmLinkAttachmentErrors As CRMLinkErrors = New CRMLinkErrors(Me.User)
        crmLinkAttachmentErrors.LoadByOperation(CRMLinkRow.OrganizationID, CRMLinkRow.CRMType, "out", "attachment")

        For Each ticket As TicketsViewItem In ticketsToPushAsCases
          Dim ticketLinkToJira As TicketLinkToJiraItem = ticketsLinksToJiraToPushAsIssues.FindByTicketID(ticket.TicketID)
          Log.Write("Processing ticket #" + ticket.TicketNumber.ToString())
          Dim updateTicketFlag As Boolean = False
          Dim sendCustomMappingFields As Boolean = False

          Dim issueFields As JObject = Nothing
          Try
            crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(ticket.TicketID, String.Empty)
            issueFields = GetIssueFields(ticket)
            If crmLinkError IsNot Nothing then
              crmLinkError.Delete()
              crmLinkErrors.Save()
            End If
          Catch ex As Exception
            If crmLinkError Is Nothing then
              Dim newCrmLinkError As CRMLinkErrors = New CRMLinkErrors(Me.User)
              crmLinkError = newCrmLinkError.AddNewCRMLinkError()
              crmLinkError.OrganizationID   = CRMLinkRow.OrganizationID
              crmLinkError.CRMType          = CRMLinkRow.CRMType
              crmLinkError.Orientation      = "out"
              crmLinkError.ObjectType       = "ticket"
              crmLinkError.ObjectFieldName  = String.Empty
              crmLinkError.ObjectID         = ticket.TicketID.ToString()
              crmLinkError.Exception        = ex.ToString() + ex.StackTrace
              crmLinkError.OperationType    = "unknown"
              newCrmLinkError.Save()
            Else
              crmLinkError.Exception        = ex.ToString() + ex.StackTrace                                               
            End If                                              

            Log.Write(crmLinkError.Exception)
            Continue For
          End Try

          Dim issue As JObject = Nothing
          'Create new issue
          If ticketLinkToJira.JiraKey Is Nothing OrElse ticketLinkToJira.JiraKey.IndexOf("Error") > -1 Then
            Try

              crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(ticket.TicketID, "create")

              Log.Write("No JiraKey. Creating issue...")
              URI = _baseURI + "/issue"
              ticketData = New StringBuilder()
              ticketData.Append(GetTicketData(ticket, issueFields))
              issue = GetAPIJObject(URI, "POST", ticketData.ToString())
              'The create issue response does not include status and we need it to initialize the synched ticket. So, we do a GET on the recently created issue.
              URI = _baseURI + "/issue/" + issue("key").ToString()
              issue = GetAPIJObject(URI, "GET", String.Empty)
              updateTicketFlag = True
              sendCustomMappingFields = True

              If crmLinkError IsNot Nothing then
                crmLinkError.Delete()
                crmLinkErrors.Save()
              End If

            Catch ex As Exception
              
              Dim updateLinkToJira As Boolean = True
              Dim errorMessage As String = String.Empty
              Select Case ex.Message
                Case "no project"
                  errorMessage = "Error: Specify Project (Product)."
                Case "type mismatch"
                  errorMessage = "Error: Specify valid Type."
                Case "project mismatch"
                  errorMessage = "Error: Specify valid Project (Product)."
                Case Else
                  'Throw ex 
                  updateLinkToJira = False
              End Select

              If updateLinkToJira Then
                ticketLinkToJira.JiraKey = errorMessage
                ticketLinkToJira.DateModifiedByJiraSync = DateTime.UtcNow()
                ticketLinkToJira.Collection.Save()
              End If

              If crmLinkError Is Nothing then
                Dim newCrmLinkError As CRMLinkErrors = New CRMLinkErrors(Me.User)
                crmLinkError = newCrmLinkError.AddNewCRMLinkError()
                crmLinkError.OrganizationID   = CRMLinkRow.OrganizationID
                crmLinkError.CRMType          = CRMLinkRow.CRMType
                crmLinkError.Orientation      = "out"
                crmLinkError.ObjectType       = "ticket"
                crmLinkError.ObjectFieldName  = "create"
                crmLinkError.ObjectID         = ticket.TicketID.ToString()
                crmLinkError.ObjectData       = ticketData.ToString()
                crmLinkError.Exception        = ex.ToString() + ex.StackTrace
                crmLinkError.OperationType    = "create"
                newCrmLinkError.Save()
              Else
                crmLinkError.ObjectData       = ticketData.ToString()
                crmLinkError.Exception        = ex.ToString() + ex.StackTrace                                               
              End If                                              

              Log.Write(errorMessage)
              Continue For

            End Try
          'Issue already exists. 
          'We are not updating issues, but if this is a second ticket relating to issue we add a remote link and update ticket fields for Jira
          ElseIf ticketLinkToJira.JiraID Is Nothing Then
            
            crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(ticket.TicketID, "update")
            
            Try
              URI = _baseURI + "/issue/" + ticketLinkToJira.JiraKey
              issue = GetAPIJObject(URI, "GET", String.Empty)
              updateTicketFlag = True
              Log.Write("No JiraID. We'll add link.")

              'An update error could has been caused here or below.
              'We'll only clear when successfull below.
            Catch ex As Exception

              If crmLinkError Is Nothing then
                Dim newCrmLinkError As CRMLinkErrors = New CRMLinkErrors(Me.User)
                crmLinkError = newCrmLinkError.AddNewCRMLinkError()
                crmLinkError.OrganizationID   = CRMLinkRow.OrganizationID
                crmLinkError.CRMType          = CRMLinkRow.CRMType
                crmLinkError.Orientation      = "out"
                crmLinkError.ObjectType       = "ticket"
                crmLinkError.ObjectFieldName  = "update"
                crmLinkError.ObjectID         = ticket.TicketID.ToString()
                crmLinkError.ObjectData       = URI
                crmLinkError.Exception        = ex.ToString() + ex.StackTrace
                crmLinkError.OperationType    = "update"
                newCrmLinkError.Save()
              Else
                crmLinkError.ObjectData       = URI
                crmLinkError.Exception        = ex.ToString() + ex.StackTrace
              End If

              Log.Write(ex.ToString() + ex.StackTrace)
              Continue For

            End Try
          Else
            Log.Write("Already linked. Doing nothing.")
          End If

          If updateTicketFlag Then

            crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(ticket.TicketID, "update")

            Try
              Dim creatorName As String = "TeamSupport"
              If ticketLinkToJira.CreatorID IsNot Nothing Then
                Dim creator As Users = New Users(User)
                creator.LoadByUserID(ticketLinkToJira.CreatorID)
                creatorName = creator(0).FirstName.Substring(0, 1) + ". " + creator(0).LastName + " linked"
              End If
              AddRemoteLinkInJira(issue("id").ToString(), ticket.TicketID.ToString(), ticket.TicketNumber.ToString(), ticket.Name, creatorName)
              Log.Write("Added remote link for ticket #" + ticket.TicketNumber.ToString())

              ticketLinkToJira.JiraID      = CType(issue("id").ToString(), Integer?)
              ticketLinkToJira.JiraKey     = issue("key").ToString()
              ticketLinkToJira.JiraLinkURL = CRMLinkRow.HostName + "/browse/" + ticketLinkToJira.JiraKey
              ticketLinkToJira.JiraStatus  = issue("fields")("status")("name").ToString()

              If CRMLinkRow.UpdateStatus Then
                Dim newStatus As TicketStatus = allStatuses.FindByName(ticketLinkToJira.JiraStatus, ticket.TicketTypeID)
                If newStatus IsNot Nothing Then
                  Dim updateTicket As Tickets = New Tickets(User)
                  updateTicket.LoadByTicketID(ticket.TicketID)
                  updateTicket(0).TicketStatusID = newStatus.TicketStatusID
                  ticketLinkToJira.DateModifiedByJiraSync = DateTime.UtcNow
                  updateTicket.Save()
                  Log.Write("Updated status with linked issue status.")
                Else
                  Log.Write("Status was not updated because """ + ticketLinkToJira.JiraStatus + """ does not exist in the current statuses.")
                End If
              Else
                Dim newAction As Actions = New Actions(User)
                newAction.AddNewAction()
                newAction(0).ActionTypeID = newActionsTypeID
                newAction(0).TicketID = ticket.TicketID
                newAction(0).Description = "Ticket has been synced with Jira's issue " + issue("key").ToString() + " with status """ + ticketLinkToJira.JiraStatus + """."
                ticketLinkToJira.DateModifiedByJiraSync = DateTime.UtcNow()
                newAction.Save()

                Dim newActionLinkToJira As ActionLinkToJira = New ActionLinkToJira(User)
                newActionLinkToJira.AddNewActionLinkToJiraItem()
                newActionLinkToJira(0).ActionID = newAction(0).ActionID
                newActionLinkToJira(0).JiraID = -1
                newActionLinkToJira(0).DateModifiedByJiraSync = ticketLinkToJira.DateModifiedByJiraSync
                newActionLinkToJira.Save()

                Log.Write("Added comment indicating linked issue status.")
              End If

              ticketLinkToJira.Collection.Save()
              Log.Write("Updated jira fields in ticket")

              If crmLinkError IsNot Nothing then
                crmLinkError.Delete()
                crmLinkErrors.Save()
              End If

            Catch ex As Exception

              If crmLinkError Is Nothing then
                Dim newCrmLinkError As CRMLinkErrors = New CRMLinkErrors(Me.User)
                crmLinkError = newCrmLinkError.AddNewCRMLinkError()
                crmLinkError.OrganizationID   = CRMLinkRow.OrganizationID
                crmLinkError.CRMType          = CRMLinkRow.CRMType
                crmLinkError.Orientation      = "out"
                crmLinkError.ObjectType       = "ticket"
                crmLinkError.ObjectFieldName  = "update"
                crmLinkError.ObjectID         = ticket.TicketID.ToString()
                'crmLinkError.ObjectData       = JsonConvert.SerializeObject(ticket)
                crmLinkError.Exception        = ex.ToString() + ex.StackTrace
                crmLinkError.OperationType    = "update"
                newCrmLinkError.Save()
              Else
                'crmLinkError.ObjectData     = JsonConvert.SerializeObject(ticket)
                crmLinkError.Exception      = ex.ToString() + ex.StackTrace                                               
              End If                                              

            End Try
          End If

          PushActionsAsComments(ticket.TicketID, ticket.TicketNumber, issue, ticketLinkToJira.JiraKey, attachmentEnabled, attachmentFileSizeLimit, crmLinkActionErrors, crmLinkAttachmentErrors)

          If sendCustomMappingFields Then
            'We are now updating the custom mapping fields. We do a call per field to minimize the impact of invalid values attempted to be assigned.
            Dim customMappingFields As New CRMLinkFields(User)
            customMappingFields.LoadByObjectTypeAndCustomFieldAuxID("Ticket", CRMLinkRow.CRMLinkID, ticket.TicketTypeID)

            Dim updateFieldRequestBody As StringBuilder

            If issueFields IsNot Nothing Then
            For Each field As KeyValuePair(Of String, JToken) In issueFields
              Dim cRMLinkField As CRMLinkField = customMappingFields.FindByCRMFieldName(field.Value("name").ToString())
              If cRMLinkField IsNot Nothing Then
                Dim value As String = Nothing
                If cRMLinkField.CustomFieldID IsNot Nothing Then
                  crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(ticket.TicketID, cRMLinkField.CustomFieldID.ToString())
                  Dim findCustom As New CustomValues(User)
                  findCustom.LoadByFieldID(cRMLinkField.CustomFieldID, ticket.TicketID)
                  If findCustom.Count > 0 Then
                    value = GetDataLineValue(field.Key.ToString(), field.Value("schema")("custom"), findCustom(0).Value)
                  Else
                    Log.Write(GetFieldNotIncludedMessage(ticket.TicketID, field.Value("name").ToString(), findCustom.Count = 0))                      
                  End If
                ElseIf cRMLinkField.TSFieldName IsNot Nothing Then
                  crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(ticket.TicketID, cRMLinkField.TSFieldName)
                  If ticket.Row(cRMLinkField.TSFieldName) IsNot Nothing Then
                    value = GetDataLineValue(field.Key.ToString(), field.Value("schema")("custom"), ticket.Row(cRMLinkField.TSFieldName))
                  Else
                    Log.Write(GetFieldNotIncludedMessage(ticket.TicketID, field.Value("name").ToString(), ticket.Row(cRMLinkField.TSFieldName) Is Nothing))                      
                  End If
                Else
                  Log.Write(
                    "TicketID " + ticket.TicketID.ToString() + "'s field '" + field.Value("name").ToString() + "' was not included because custom field " + 
                    cRMLinkField.CRMFieldID.ToString() + " CustomFieldID and TSFieldName are null.")
                End If

                If value IsNot Nothing Then
                  Try
                    updateFieldRequestBody = New StringBuilder()
                    updateFieldRequestBody.Append("{")
                    updateFieldRequestBody.Append("""fields"":{")

                    updateFieldRequestBody.Append("""" + field.Key.ToString() + """:" + value)                  

                    updateFieldRequestBody.Append("}")
                    updateFieldRequestBody.Append("}")
       
                    issue = GetAPIJObject(URI, "PUT", updateFieldRequestBody.ToString())

                    If crmLinkError IsNot Nothing then
                      crmLinkError.Delete()
                      crmLinkErrors.Save()
                    End If

                  Catch ex As Exception
                    Dim exBody As String = value
                    If updateFieldRequestBody IsNot Nothing Then
                      exBody = updateFieldRequestBody.ToString()
                    End If
                    If ex.Message <> "Error reading JObject from JsonReader. Path '', line 0, position 0." Then
                      Log.Write("Field " + field.Value("name").ToString() + " with body " + exBody + ", was not sent because an exception ocurred.")                      
                    End If

                    If crmLinkError Is Nothing then
                      Dim newCrmLinkError As CRMLinkErrors = New CRMLinkErrors(Me.User)
                      crmLinkError = newCrmLinkError.AddNewCRMLinkError()
                      crmLinkError.OrganizationID = CRMLinkRow.OrganizationID
                      crmLinkError.CRMType        = CRMLinkRow.CRMType
                      crmLinkError.Orientation    = "out"
                      crmLinkError.ObjectType     = "ticket"
                      crmLinkError.ObjectID       = ticket.TicketID.ToString()
                      If cRMLinkField.CustomFieldID IsNot Nothing Then
                        crmLinkError.ObjectFieldName = cRMLinkField.CustomFieldID.ToString()
                      Else
                        crmLinkError.ObjectFieldName = cRMLinkField.TSFieldName
                      End If
                      crmLinkError.ObjectData     = value
                      crmLinkError.Exception      = ex.ToString() + ex.StackTrace
                      crmLinkError.OperationType  = "unknown"
                      newCrmLinkError.Save()
                    Else
                      crmLinkError.ObjectData     = value
                      crmLinkError.Exception      = ex.ToString() + ex.StackTrace                                               
                    End If                                              

                  End Try
                End If
              End If
            Next
            End If

          End If
        Next
        crmLinkErrors.Save()
        crmLinkActionErrors.Save()
        crmLinkAttachmentErrors.Save()
      End Sub

        Private Function GetAttachmentEnabled(ByRef attachmentFileSizeLimit As Integer) As String
          Dim result As Boolean = False

          Dim URI As String = _baseURI + "/attachment/meta"
          Dim batch As JObject = GetAPIJObject(URI, "GET", string.Empty)
          result = Convert.ToBoolean(batch("enabled").ToString())
          attachmentFileSizeLimit = Convert.ToInt32(batch("uploadLimit").ToString())
          Log.Write("Attachment enabled is " + result.ToString())

          Return result
        End Function

            Private Function GetTicketData(ByVal ticket As TicketsViewItem, ByRef fields As JObject) As String
                Dim result As StringBuilder = New StringBuilder()
                Dim customField As StringBuilder = New StringBuilder()
                customField = BuildRequiredFields(ticket, fields)
                result.Append("{")
                result.Append("""fields"":{")
                result.Append("""summary"":""" + DataUtils.GetJsonCompatibleString(HtmlUtility.StripHTML(HtmlUtility.StripHTMLUsingAgilityPack(ticket.Name))) + """,")
                result.Append("""issuetype"":{""name"":""" + ticket.TicketTypeName + """},")
                If customField.ToString().Trim().Length > 0 Then
                    result.Append(customField.ToString() + ",")
                End If
                result.Append("""project"":{""key"":""" + GetProjectKey(ticket.ProductName) + """},")
                Dim description As String = HtmlUtility.StripHTML(HtmlUtility.StripHTMLUsingAgilityPack(Actions.GetTicketDescription(User, ticket.TicketID).Description))
                result.Append("""description"":""" + DataUtils.GetJsonCompatibleString(description) + """")
                result.Append("}")
                result.Append("}")

                Return result.ToString()
            End Function

          Private Function GetIssueFields(ByVal ticket As TicketsViewItem) As JObject
            Dim issueTypeName As String = ticket.TicketTypeName
            Dim projectKey As String = GetProjectKey(ticket.ProductName)
            Dim URI As String = 
              _baseURI + 
              "/issue/createmeta?projectKeys=" + 
              projectKey.ToUpper() + 
              "&issuetypeNames=" + issueTypeName +
              "&expand=projects.issuetypes.fields" 
              'View an example of this response in 
              'https://developer.atlassian.com/display/JIRADEV/JIRA+REST+API+Example+-+Discovering+meta-data+for+creating+issues
               
            Dim result As JObject = Nothing
            Try

              Dim fields = GetAPIJObject(URI, "GET", string.Empty)
              Dim issueTypeIndex As Integer? = Nothing

              For i = 0 To fields("projects")(0)("issuetypes").Count - 1
                If fields("projects")(0)("issuetypes")(i)("name").ToString().ToLower() = issueTypeName.ToLower() Then
                  issueTypeIndex = i
                  Exit For
                End If
              Next
              If issueTypeIndex Is Nothing Then
                'Throw New Exception("type mismatch")
                'See ticket #16955
                Log.Write("Type was not found in list of project types. If an exception ahead, chances are it was caused by missing type.")
              Else
                result = CType(fields("projects")(0)("issuetypes")(issueTypeIndex)("fields"), JObject)
              End If
            Catch ex As Exception
              Log.Write("Exception rised attempting to get createmeta.")
              Log.Write(ex.Message)
              Log.Write("URI: " + URI)
              Log.Write("Type: " + issueTypeName)
              'See ticket #16955
              'If ex.Message <> "type mismatch" Then
                Throw New Exception("project mismatch")
              'Else
              '  Throw ex
              'End If
            End Try

            Return result
          End Function

            Private Function GetProjectKey(ByVal productName As String) As String
              Dim result As String = productName
              If CRMLinkRow.AlwaysUseDefaultProjectKey OrElse String.IsNullOrEmpty(productName) Then
                result = CRMLinkRow.DefaultProject
                If String.IsNullOrEmpty(result) Then
                  Dim ex As Exception = New Exception("no project")
                  Throw ex
                End If
              End If
              Return result
            End Function

          Private Function GetDataLine(
            ByVal field As KeyValuePair(Of String, JToken), 
            ByVal ticket As TicketsViewItem,
            ByRef preffix As String) As String
            Dim result As String = String.Empty
        
              Select Case field.Value("name").ToString().Trim().ToLower()
                Case "project"
                  Dim projectKey As String = GetProjectKey(ticket.ProductName)
                  result = preffix + """project"":{""key"":""" + projectKey + """}"
                  If preffix = String.Empty Then
                    preffix = ","
                  End If
                Case "summary"
                  If ticket.Name IsNot Nothing Then               
                            result = preffix + """summary"":""" + DataUtils.GetJsonCompatibleString(HtmlUtility.StripHTML(HtmlUtility.StripHTMLUsingAgilityPack(ticket.Name))) + """"
                    If preffix = String.Empty Then
                      preffix = ","
                    End If
                  Else
                    Log.Write(GetFieldNotIncludedMessage(ticket.TicketID, field.Value("name").ToString(), ticket.Name Is Nothing))                      
                  End If
                Case "description"
                        Dim description As String = HtmlUtility.StripHTML(HtmlUtility.StripHTMLUsingAgilityPack(Actions.GetTicketDescription(User, ticket.TicketID).Description))
                  If description IsNot Nothing Then
                    result = preffix + """description"":""" + DataUtils.GetJsonCompatibleString(description) + """"
                    If preffix = String.Empty Then
                      preffix = ","
                    End If
                  Else
                    Log.Write(GetFieldNotIncludedMessage(ticket.TicketID, field.Value("name").ToString(), description Is Nothing))                      
                  End If
                Case "issue type"
                  result = preffix + """issuetype"":{""name"":""" + ticket.TicketTypeName + """}"
                  If preffix = String.Empty Then
                    preffix = ","
                  End If
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

              Dim message As StringBuilder = New StringBuilder("TicketID " + ticketID.ToString() + "'s field '" + fieldName + "' was not included because ")
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

            Private Function GetDataLineValue(ByVal fieldKey As String, ByVal fieldType As Object, ByVal fieldValue As String) As String
              Dim result As String = Nothing
              Select fieldKey.ToLower()
                Case "assignee", "reporter"
                  result = "{""emailAddress"":""" + fieldValue + """}"
                Case "issuetype", "status", "priority", "resolution"
                  result = "{""name"":""" + fieldValue + """}"
                Case "progress", "worklog"
                  result = "{""total"":""" + fieldValue + """}"
                Case "project"
                  result = "{""key"":""" + fieldValue + """}"
                Case "votes"
                  result = "{""votes"":""" + fieldValue + """}"
                Case "watches"
                  result = "{""watchCount"":""" + fieldValue + """}"
                Case "timetracking"
                  result = "{""timeSpentSeconds"":""" + fieldValue + """}"
                Case "aggregrateprogress"
                  result = "{""progress"":""" + fieldValue + """}"
                Case "attachment", "labels", "issuelinks", "versions", "fixversions", "subtasks", "components"
                  result = "[{""name"":""" + fieldValue + """}]"
                Case Else
                  result = """" + fieldValue + """"                
                  If fieldType IsNot Nothing Then
                    Dim fieldTypeString = fieldType.ToString()
                    If fieldTypeString.Length > 50 Then
                      Select fieldTypeString.substring(50, fieldTypeString.Length - 50).ToLower()
                        Case "select"
                          result = "{""value"":""" + fieldValue + """}"
                        Case "multiselect"
                          result = "[{""value"":""" + fieldValue + """}]"
                        Case "date"
                          result = """" + Convert.ToDateTime(fieldValue).ToString("'yyyy'-'MM'-'dd'") + """"
                        Case "datetime"
                          result = """" + Convert.ToDateTime(fieldValue).ToString("'yyyy'-'MM'-'dd'T'HH':'mm':'ss.fff'Z'") + """"
                        Case "float"
                          result = fieldValue
                        'text field (single-line)
                        Case "string"
                          result = "{""" + fieldValue + """}"
                        Case "radiobuttons"
                          If fieldValue = "True" Then
                            result = "{""value"":""Yes""}"
                          Else
                            result = "{""value"":""No""}"
                          End If
                      End Select
                    End If
                  End If                                               
              End Select                      
              Return result
            End Function

        Private Sub AddRemoteLinkInJira(ByVal issueID As String, ByVal ticketID As String, ByVal ticketNumber As String, ByVal ticketName As String, ByVal creatorName As String)
          Dim domain As String = SystemSettings.ReadString(User, "AppDomain", "https://app.teamsupport.com")
          Dim remoteLinkData As StringBuilder = New StringBuilder()
          remoteLinkData.Append("{")
            'Global ID initialized as documentation examples in two parts separated by &. First part is the domain and the second one the id.
            remoteLinkData.Append("""globalid"":""system=" + domain + "/Ticket.aspx?ticketid=&id=" + ticketID + """,")
            remoteLinkData.Append("""application"":{")
              remoteLinkData.Append("""name"":""Team Support""},")
            remoteLinkData.Append("""object"":{")
              remoteLinkData.Append("""icon"":{")
                remoteLinkData.Append("""url16x16"":""" + domain + "/vcr/1_6_5/Images/icons/TeamSupportLogo16.png"",")
                remoteLinkData.Append("""title"":""TeamSupport Logo""},")
              remoteLinkData.Append("""title"":""" + creatorName + " Ticket #" + ticketNumber + """,")
                remoteLinkData.Append("""summary"":""" + DataUtils.GetJsonCompatibleString(HtmlUtility.StripHTML(HtmlUtility.StripHTMLUsingAgilityPack(ticketName))) + """,")
              remoteLinkData.Append("""url"":""" + domain + "/Ticket.aspx?ticketid=" + ticketID + """")
            remoteLinkData.Append("}")
          remoteLinkData.Append("}")

          Dim URI As String = _baseURI + "/issue/" + issueID + "/remotelink"
          Log.Write("AddRemoteLink URI: " + URI)
          Log.Write("AddRemoteLink Data:" + remoteLinkData.ToString())
          Try
            Dim response As JObject = GetAPIJObject(URI, "POST", remoteLinkData.ToString())
          Catch ex As Exception
            Log.Write(ex.Message)
            Log.Write(ex.StackTrace)
            Throw ex
          End Try
        End Sub

        Private Sub PushActionsAsComments(
        ByVal ticketID As Integer, 
        ByVal ticketNumber As Integer, 
        ByVal issue As JObject, 
        ByVal issueKey As String,
        ByVal attachmentEnabled As Boolean,
        ByVal attachmentFileSizeLimit As Integer,
        ByRef crmLinkActionErrors As CRMLinkErrors,
        ByRef crmLinkAttachmentErrors As CRMLinkErrors)
          Dim actionsToPushAsComments As Actions = New Actions(User)
          actionsToPushAsComments.LoadToPushToJira(CRMLinkRow, ticketID)
          Log.Write("Found " + actionsToPushAsComments.Count.ToString() + " actions to push as comments.")

          Dim actionLinkToJira As ActionLinkToJira = New ActionLinkToJira(User)
          actionLinkToJira.LoadToPushToJira(CRMLinkRow, ticketID)

          Dim crmLinkError As CRMLinkError = Nothing
          Dim body As StringBuilder = Nothing
          
          For Each actionToPushAsComment As Action In actionsToPushAsComments

            crmLinkError = crmLinkActionErrors.FindByObjectIDAndFieldName(actionToPushAsComment.ActionID.ToString(), string.Empty)

            Dim actionLinkToJiraItem As ActionLinkToJiraItem = actionLinkToJira.FindByActionID(actionToPushAsComment.ActionID)

            Log.Write("Processing actionID: " + actionToPushAsComment.ActionID.ToString())
            Dim comment As JObject = Nothing
            
            Dim actionPosition As Integer =  Actions.GetActionPosition(User, actionToPushAsComment.ActionID)
            If actionLinkToJiraItem Is Nothing Then
              
              Try
                If issue Is Nothing Then
                  issue = GetAPIJObject(_baseURI + "/issue/" + issueKey, "GET",  String.Empty)
                End If

                Dim URI As String = _baseURI + "/issue/" + issue("id").ToString() + "/comment"
                body = New StringBuilder()
                body.Append(BuildCommentBody(ticketNumber, actionToPushAsComment.Description, actionPosition))
                comment = GetAPIJObject(URI, "POST", body.ToString())

                Dim newActionLinkToJira As ActionLinkToJira = New ActionLinkToJira(User)
                Dim newActionLinkToJiraItem As ActionLinkToJiraItem = newActionLinkToJira.AddNewActionLinkToJiraItem()
                newActionLinkToJiraItem.ActionID = actionToPushAsComment.ActionID
                newActionLinkToJiraItem.JiraID = CType(comment("id").ToString(), Integer?)
                newActionLinkToJiraItem.DateModifiedByJiraSync = DateTime.UtcNow
                newActionLinkToJira.Save()
                Log.Write("created comment for action")

                If crmLinkError IsNot Nothing then
                  crmLinkError.Delete()
                  crmLinkActionErrors.Save()
                End If

              Catch ex As Exception

                If crmLinkError Is Nothing then
                  Dim newCrmLinkError As CRMLinkErrors = New CRMLinkErrors(Me.User)
                  crmLinkError = newCrmLinkError.AddNewCRMLinkError()
                  crmLinkError.OrganizationID = CRMLinkRow.OrganizationID
                  crmLinkError.CRMType        = CRMLinkRow.CRMType
                  crmLinkError.Orientation    = "out"
                  crmLinkError.ObjectType     = "action"
                  crmLinkError.ObjectID       = actionToPushAsComment.ActionID.ToString()
                  If body IsNot Nothing
                    crmLinkError.ObjectData     = body.ToString()
                  End If
                  crmLinkError.Exception      = ex.ToString() + ex.StackTrace
                  crmLinkError.OperationType  = "create"
                  newCrmLinkError.Save()
                Else
                  crmLinkError.ObjectData     = body.ToString()
                  crmLinkError.Exception      = ex.ToString() + ex.StackTrace                                               
                End If                                              

                Log.Write(ex.ToString() + ex.StackTrace)
                Continue For

              End Try
            Else
              Try
                Log.Write("action.JiraID: " + actionLinkToJiraItem.JiraID.ToString())
                If actionLinkToJiraItem.JiraID <> -1 Then
                  If issue Is Nothing Then
                    issue = GetAPIJObject(_baseURI + "/issue/" + issueKey, "GET", String.Empty)
                  End If

                  Dim URI As String = _baseURI + "/issue/" + issue("id").ToString() + "/comment/" + actionLinkToJiraItem.JiraID.ToString()
                  body = New StringBuilder()
                  body.Append(BuildCommentBody(ticketNumber, actionToPushAsComment.Description, actionPosition))
                  comment = GetAPIJObject(URI, "PUT", body.ToString())
                  actionLinkToJiraItem.DateModifiedByJiraSync = DateTime.UtcNow
                  Log.Write("updated comment for actionID: " + actionToPushAsComment.ActionID.ToString())
                End If

                If crmLinkError IsNot Nothing then
                  crmLinkError.Delete()
                  crmLinkActionErrors.Save()
                End If

              Catch ex As Exception

                If crmLinkError Is Nothing then
                  Dim newCrmLinkError As CRMLinkErrors = New CRMLinkErrors(Me.User)
                  crmLinkError = newCrmLinkError.AddNewCRMLinkError()
                  crmLinkError.OrganizationID = CRMLinkRow.OrganizationID
                  crmLinkError.CRMType        = CRMLinkRow.CRMType
                  crmLinkError.Orientation    = "out"
                  crmLinkError.ObjectType     = "action"
                  crmLinkError.ObjectID       = actionToPushAsComment.ActionID.ToString()
                  If body IsNot Nothing
                    crmLinkError.ObjectData     = body.ToString()
                  End If 
                  crmLinkError.Exception      = ex.ToString() + ex.StackTrace
                  crmLinkError.OperationType  = "update"
                  newCrmLinkError.Save()
                Else
                  If (body IsNot Nothing)
                    crmLinkError.ObjectData     = body.ToString()
                  End If
                  crmLinkError.Exception      = ex.ToString() + ex.StackTrace                                               
                End If   
                                                           
                Log.Write(ex.ToString() + ex.StackTrace)
                Continue For

              End Try
            End If

            If (attachmentEnabled)
              PushAttachments(actionToPushAsComment.ActionID, ticketNumber, issue, issueKey, attachmentFileSizeLimit, actionPosition, crmLinkAttachmentErrors)
            End If
          Next
          actionLinkToJira.Save()
        End Sub
          
          Private Sub PushAttachments(
          ByVal actionID As Integer, 
          ByVal ticketNumber As Integer, 
          ByVal issue As JObject, 
          ByVal issueKey As String,
          ByVal fileSizeLimit As Integer,
          ByVal actionPosition As Integer,
          ByRef crmLinkAttachmentErrors As CRMLinkErrors)
            Dim attachments As Attachments = New Attachments(User)
            attachments.LoadForJira(actionID)
            Dim updateAttachments As Boolean = False
            Dim crmLinkError As CRMLinkError = Nothing

            For Each attachment As Attachment In attachments
              If (Not File.Exists(attachment.Path))
                Log.Write("Attachment """ + attachment.FileName + """ was not sent as it was not found on server")
              Else
                  Dim fs = new FileStream(attachment.Path, FileMode.Open, FileAccess.Read)
                  If (fs.Length > fileSizeLimit)
                    Log.Write(
                      "Attachment """ + attachment.FileName + 
                      """ was not sent as its file size (" + 
                      fs.Length.ToString() + 
                      ") exceeded the file size limit of " + 
                      fileSizeLimit)
                  Else
                    Dim URIString As String = _baseURI + "/issue/" + issue("id").ToString() + "/attachments/"
                    Dim request As HttpWebRequest = WebRequest.Create(URIString)
                    request.Headers.Add("Authorization", "Basic " + _encodedCredentials)
                    request.Headers.Add("X-Atlassian-Token", "nocheck")
                    request.Method = "POST"
                    Dim boundary As String = string.Format("----------{0:N}", Guid.NewGuid())
                    request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary)
                    request.UserAgent = Client

                    Dim content = new MemoryStream()
                      Dim writer = new StreamWriter(content)
                      writer.WriteLine("--{0}", boundary)
                      writer.WriteLine("Content-Disposition: form-data; name=""file""; filename=""{0}""", ("TeamSupport Ticket #" + ticketNumber.ToString() + " action #" + actionPosition.ToString() + " - " + attachment.FileName))
                      writer.WriteLine("Content-Type: application/octet-stream")
                      writer.WriteLine()
                      writer.Flush()
                      Dim data(fs.Length) As Byte
                      fs.Read(data, 0, data.Length)
                      fs.Close()
                      content.Write(data, 0, data.Length)
                      writer.WriteLine()
                      writer.WriteLine("--" + boundary + "--")
                      writer.Flush()
                    content.Seek(0, SeekOrigin.Begin)
                    request.ContentLength = content.Length

                    Using requestStream As Stream = request.GetRequestStream()
                      content.WriteTo(requestStream)
                      requestStream.Close()
                    End Using

                    crmLinkError = crmLinkAttachmentErrors.FindByObjectIDAndFieldName(attachment.AttachmentID.ToString(), string.Empty)
                    
                    Try
                      Dim response As HttpWebResponse = request.GetResponse()
                      Log.Write("Attachment """ + attachment.FileName + """ sent.")
                      attachment.SentToJira = True
                      updateAttachments = True                                 

                      If crmLinkError IsNot Nothing then
                        crmLinkError.Delete()
                        crmLinkAttachmentErrors.Save()
                      End If

                    Catch ex As Exception

                      If crmLinkError Is Nothing then
                        Dim newCrmLinkError As CRMLinkErrors = New CRMLinkErrors(Me.User)
                        crmLinkError = newCrmLinkError.AddNewCRMLinkError()
                        crmLinkError.OrganizationID = CRMLinkRow.OrganizationID
                        crmLinkError.CRMType        = CRMLinkRow.CRMType
                        crmLinkError.Orientation    = "out"
                        crmLinkError.ObjectType     = "attachment"
                        crmLinkError.ObjectID       = attachment.AttachmentID.ToString()
                        'crmLinkError.ObjectData     = JsonConvert.SerializeObject(thisCompany)
                        crmLinkError.Exception      = ex.ToString() + ex.StackTrace
                        crmLinkError.OperationType  = "create"
                        newCrmLinkError.Save()
                      Else
                        'crmLinkError.ObjectData     = JsonConvert.SerializeObject(thisCompany)
                        crmLinkError.Exception      = ex.ToString() + ex.StackTrace                                               
                      End If                                              

                    End Try
                  End If
                End If
            Next
            If updateAttachments then
              attachments.Save()
            End If
          End Sub

          Private Function BuildCommentBody(ByVal ticketNumber As String, ByVal actionDescription As String, ByVal actionPosition As Integer) As String
            Dim result As StringBuilder = New StringBuilder()
            result.Append("{")
                result.Append("""body"":""TeamSupport ticket #" + ticketNumber.ToString() + " comment #" + actionPosition.ToString() + ":\n\n")
                result.Append(DataUtils.GetJsonCompatibleString(HtmlUtility.StripHTML(HtmlUtility.StripHTMLUsingAgilityPack(actionDescription))))
            result.Append("""}")
            Return result.ToString()
          End Function

      Private Sub PullIssuesAndCommentsAsTicketsAndActions(ByVal issuesToPullAsTickets As JArray, ByVal allStatuses As TicketStatuses, ByVal newActionsTypeID As Integer, ByVal indexOfTicketIDInRemoteLink As Integer)
        Dim crmLinkErrors As CRMLinkErrors = New CRMLinkErrors(Me.User)
        crmLinkErrors.LoadByOperation(CRMLinkRow.OrganizationID, CRMLinkRow.CRMType, "in", "ticket")
        Dim crmLinkError As CRMLinkError = Nothing

        Dim crmLinkActionErrors As CRMLinkErrors = New CRMLinkErrors(Me.User)
        crmLinkActionErrors.LoadByOperation(CRMLinkRow.OrganizationID, CRMLinkRow.CRMType, "in", "action")

        For i = 0 To issuesToPullAsTickets.Count - 1
          Dim newComments As JArray = Nothing
          For Each ticketID As Integer In GetLinkedTicketIDs(issuesToPullAsTickets(i), indexOfTicketIDInRemoteLink)
            crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(ticketID.ToString(), string.Empty)
            Try
              UpdateTicketWithIssueData(ticketID, issuesToPullAsTickets(i), newActionsTypeID, allStatuses)

              If crmLinkError IsNot Nothing then
                crmLinkError.Delete()
                crmLinkErrors.Save()
              End If
            Catch ex As Exception
              If crmLinkError Is Nothing then
                Dim newCrmLinkError As CRMLinkErrors = New CRMLinkErrors(Me.User)
                crmLinkError = newCrmLinkError.AddNewCRMLinkError()
                crmLinkError.OrganizationID = CRMLinkRow.OrganizationID
                crmLinkError.CRMType        = CRMLinkRow.CRMType
                crmLinkError.Orientation    = "in"
                crmLinkError.ObjectType     = "ticket"
                crmLinkError.ObjectID       = ticketID.ToString()
                crmLinkError.ObjectData     = JsonConvert.SerializeObject(issuesToPullAsTickets(i))
                crmLinkError.Exception      = ex.ToString() + ex.StackTrace
                crmLinkError.OperationType  = "update"
                newCrmLinkError.Save()
              Else
                crmLinkError.ObjectData     = JsonConvert.SerializeObject(issuesToPullAsTickets(i))
                crmLinkError.Exception      = ex.ToString() + ex.StackTrace                                               
              End If                                              
            End Try
            If newComments Is Nothing Then
              newComments = GetNewComments(issuesToPullAsTickets(i)("fields")("comment"), ticketID)
            End If
            AddNewCommentsInTicket(ticketID, newComments, newActionsTypeID, crmLinkActionErrors)
          Next
        Next
        crmLinkErrors.Save()
        crmLinkActionErrors.Save()
      End Sub

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

        Private Function GetLinkedTicketIDs(ByVal issue As JObject, ByVal indexOfTicketIDInRemoteLink As Integer) As List(Of Integer)
          Dim result As List(Of Integer) = New List(Of Integer)()

          Dim URI As String = _baseURI + "/issue/" + issue("id").ToString() + "/remotelink"
          Dim remoteLinks As JArray = GetAPIJArray(URI, "GET", String.Empty)
          'For i = 0 To CType(remoteLinks("total"), Integer)
          Log.Write("remoteLinks.Count: " + remoteLinks.Count.ToString())
          For i = 0 To remoteLinks.Count - 1
            If remoteLinks(i)("application")("name") = "Team Support" Then
              Dim remoteLinkURL As String = remoteLinks(i)("object")("url").ToString()
              Log.Write("remoteLinkURL: " + remoteLinkURL)
              result.Add(CType(remoteLinks(i)("object")("url").ToString().Substring(indexOfTicketIDInRemoteLink), Integer))
            End If
          Next

          Return result
            End Function

            Private Function BuildRequiredFields(ByVal ticket As TicketsViewItem, ByRef fields As JObject)
                Dim result As StringBuilder = New StringBuilder()

                Try
                    Dim customMappingFields As New CRMLinkFields(User)
                    customMappingFields.LoadByObjectType("Ticket", CRMLinkRow.CRMLinkID)

                    For Each field As KeyValuePair(Of String, JToken) In fields
                        If field.Value("required") Then
                            If field.Key = "summary" OrElse field.Key = "issuetype" OrElse field.Key = "project" OrElse field.Key = "description" Then
                                Continue For
                            End If

                            Dim value As String = Nothing

                            Dim cRMLinkField As CRMLinkField = customMappingFields.FindByCRMFieldName(field.Value("name").ToString())
                            If cRMLinkField IsNot Nothing Then
                                If cRMLinkField.CustomFieldID IsNot Nothing Then
                                    Dim findCustom As New CustomValues(User)
                                    findCustom.LoadByFieldID(cRMLinkField.CustomFieldID, ticket.TicketID)
                                    If findCustom.Count > 0 Then
                                        value = GetDataLineValue(field.Key.ToString(), field.Value("schema")("custom"), findCustom(0).Value)
                                    Else
                                        Log.Write(GetFieldNotIncludedMessage(ticket.TicketID, field.Value("name").ToString(), findCustom.Count = 0))
                                    End If
                                ElseIf cRMLinkField.TSFieldName IsNot Nothing Then
                                    If ticket.Row(cRMLinkField.TSFieldName) IsNot Nothing Then
                                        value = GetDataLineValue(field.Key.ToString(), field.Value("schema")("custom"), ticket.Row(cRMLinkField.TSFieldName))
                                    Else
                                        Log.Write(GetFieldNotIncludedMessage(ticket.TicketID, field.Value("name").ToString(), ticket.Row(cRMLinkField.TSFieldName) Is Nothing))
                                    End If
                                Else
                                    Log.Write(
                                      "TicketID " + ticket.TicketID.ToString() + "'s field '" + field.Value("name").ToString() + "' was not included because custom field " +
                                      cRMLinkField.CRMFieldID.ToString() + " CustomFieldID and TSFieldName are null.")
                                End If
                            End If

                            If value IsNot Nothing Then
                                If result.ToString().Trim().Length > 0 Then
                                    result.Append(",")
                                End If
                                result.Append("""" + field.Key.ToString() + """:" + value)
                            Else
                                Log.Write("No value foud for the required field " + field.Value("name").ToString())
                            End If
                        End If
                    Next
                Catch ex As Exception
                    Log.Write("The following building required fields: " + ex.ToString() + ex.StackTrace)
                End Try

                Return result
            End Function

            Private Sub UpdateTicketWithIssueData(ByVal ticketID As Integer, ByVal issue As JObject, ByVal newActionsTypeID As Integer, ByVal allStatuses As TicketStatuses)
                Dim updateTicket As Tickets = New Tickets(User)
                updateTicket.LoadByTicketID(ticketID)

                If updateTicket.Count > 0 AndAlso updateTicket(0).OrganizationID = CRMLinkRow.OrganizationID Then
                    Dim ticketLinkToJira As TicketLinkToJira = New TicketLinkToJira(User)
                    ticketLinkToJira.LoadByTicketID(updateTicket(0).TicketID)

                    Dim customFields As New CRMLinkFields(User)
                    customFields.LoadByObjectTypeAndCustomFieldAuxID("Ticket", CRMLinkRow.CRMLinkID, updateTicket(0).TicketTypeID)

                    Dim ticketValuesChanged = False
                    Dim ticketView As TicketsView = New TicketsView(User)
                    ticketView.LoadByTicketID(ticketID)
                    Dim issueFields As JObject = GetIssueFields(ticketView(0))

                    For Each field As KeyValuePair(Of String, JToken) In CType(issue("fields"), JObject)
                        Dim value As String = Nothing
                        Try
                            value = GetFieldValue(field)
                        Catch ex As Exception
                            Log.Write("Field: """ + field.Key + """ was not updated because the following exception ocurred getting its value:")
                            Log.Write(ex.StackTrace)
                            Continue For
                        End Try

                        Dim cRMLinkField As CRMLinkField = customFields.FindByCRMFieldName(GetFieldNameByKey(field.Key.ToString(), issueFields))
                        If cRMLinkField IsNot Nothing Then
                            Try
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

                                    If IsDBNull(thisCustom.Value) OrElse thisCustom.Value <> translatedFieldValue Then
                                        thisCustom.Value = translatedFieldValue
                                        thisCustom.Collection.Save()
                                        ticketValuesChanged = True
                                    End If
                                ElseIf cRMLinkField.TSFieldName IsNot Nothing Then
                                    Try
                                        If IsDBNull(updateTicket(0).Row(cRMLinkField.TSFieldName)) OrElse updateTicket(0).Row(cRMLinkField.TSFieldName) <> translatedFieldValue Then
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
                                Log.Write(
                                  "The following exception was caught mapping the ticket field """ &
                                  field.Key &
                                  """ with """ &
                                  cRMLinkField.TSFieldName &
                                  """: " &
                                  mappingException.Message &
                                  " " & mappingException.StackTrace)
                            End Try
                        Else
                            Select Case field.Key.Trim().ToLower()
                                Case "issuetype"
                                    Dim allTypes As TicketTypes = New TicketTypes(User)
                                    allTypes.LoadByOrganizationID(CRMLinkRow.OrganizationID)

                                    Dim currentType As TicketType = allTypes.FindByTicketTypeID(updateTicket(0).TicketTypeID)
                                    Dim newType As TicketType = allTypes.FindByName(value)

                                    If newType IsNot Nothing AndAlso newType.TicketTypeID <> currentType.TicketTypeID Then
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
                                Case "status"
                                    Dim currentStatus As TicketStatus = allStatuses.FindByTicketStatusID(updateTicket(0).TicketStatusID)
                                    Dim newStatus As TicketStatus = allStatuses.FindByName(value, updateTicket(0).TicketTypeID)

                                    If CRMLinkRow.UpdateStatus AndAlso newStatus IsNot Nothing AndAlso newStatus.TicketStatusID <> currentStatus.TicketStatusID Then
                                        updateTicket(0).TicketStatusID = newStatus.TicketStatusID
                                        ticketValuesChanged = True
                                    ElseIf Not CRMLinkRow.UpdateStatus Then
                                        If ticketLinkToJira.Count > 0 AndAlso (ticketLinkToJira(0).JiraStatus Is Nothing OrElse ticketLinkToJira(0).JiraStatus <> value) Then
                                            Dim fromStatement As String = String.Empty
                                            If ticketLinkToJira(0).JiraStatus IsNot Nothing Then
                                                fromStatement = " from """ + ticketLinkToJira(0).JiraStatus + """"
                                            End If
                                            Dim newAction As Actions = New Actions(User)
                                            newAction.AddNewAction()
                                            newAction(0).ActionTypeID = newActionsTypeID
                                            newAction(0).TicketID = updateTicket(0).TicketID
                                            newAction(0).Description = "Jira's Issue " + issue("key").ToString() + "'s status changed" + fromStatement + " to """ + value + """."

                                            Dim actionLinkToJira As ActionLinkToJira = New ActionLinkToJira(User)
                                            Dim actionLinkToJiraItem As ActionLinkToJiraItem = actionLinkToJira.AddNewActionLinkToJiraItem()
                                            actionLinkToJiraItem.JiraID = -1

                                            actionLinkToJiraItem.DateModifiedByJiraSync = DateTime.UtcNow()
                                            newAction.Save()

                                            actionLinkToJiraItem.ActionID = newAction(0).ActionID
                                            actionLinkToJira.Save()

                                            ticketValuesChanged = True
                                        End If
                                    End If
                                    ticketLinkToJira(0).JiraStatus = value
                            End Select
                        End If
                    Next

                    If ticketValuesChanged Then
                        ticketLinkToJira(0).DateModifiedByJiraSync = DateTime.UtcNow
                        updateTicket.Save()
                        ticketLinkToJira.Save()
                        Dim actionLogDescription As String = "Updated Ticket with Jira Issue Key: '" + issue("key").ToString() + "' changes."
                        ActionLogs.AddActionLog(User, ActionLogType.Update, ReferenceType.Tickets, updateTicket(0).TicketID, actionLogDescription)
                    Else
                        Log.Write("ticketID: " + updateTicket(0).TicketID.ToString() + " values were not updated because have not changed.")
                    End If
                Else
                    Log.Write("Ticket with ID: """ + ticketID.ToString() + """ was not found to be updated.")
                End If
            End Sub

            Private Function GetFieldNameByKey(ByVal fieldKey As String, ByVal issueFields As JObject)
                Dim result As StringBuilder = New StringBuilder()
                For Each field As KeyValuePair(Of String, JToken) In issueFields
                    If fieldKey = field.Key Then
                        result.Append(field.Value("name").ToString())
                        Exit For
                    End If
                Next

                If result.Length = 0 Then
                    result.Append(fieldKey)
                End If

                Return result.ToString()
            End Function

            Private Function GetFieldValue(ByVal field As KeyValuePair(Of String, JToken)) As String
                Dim result As String = Nothing
                If field.Key.ToString().ToLower().Contains("customfield") = True Then
                    result = field.Value("value").ToString()
                    If result.ToLower() = "yes" Then
                        result = "True"
                    ElseIf result.ToLower() = "no" Then
                        result = "False"
                    End If
                Else
                    Select Case field.Key.ToString().ToLower()
                        Case "aggregatetimeestimate",
                          "aggregatetimeoriginalestimate",
                          "aggregatetimespent",
                          "created",
                          "description",
                          "duedate",
                          "environment",
                          "lastviewed",
                          "resolutiondate",
                          "summary",
                          "timeestimate",
                          "timeoriginalestimate",
                          "updated",
                          "workratio",
                          "timespent"
                            result = field.Value.ToString()
                        Case "assignee", "reporter"
                            result = field.Value("emailAddress").ToString()
                        Case "issuetype", "status", "priority", "resolution"
                            result = field.Value("name").ToString()
                        Case "progress", "worklog"
                            result = field.Value("total").ToString()
                        Case "project"
                            result = field.Value("key").ToString()
                        Case "votes"
                            result = field.Value("votes").ToString()
                        Case "watches"
                            result = field.Value("watchCount").ToString()
                        Case "timetracking"
                            result = field.Value("timeSpentSeconds").ToString()
                        Case "aggregrateprogress"
                            result = field.Value("progress").ToString()
                        Case "attachment"
                            Dim attachmentsArray As JArray = DirectCast(field.Value, JArray)
                            Dim resultBuilder As StringBuilder = New StringBuilder()
                            Dim preffix = String.Empty
                            For i = 0 To attachmentsArray.Count - 1
                                resultBuilder.Append(preffix)
                                resultBuilder.Append(attachmentsArray(i)("content").ToString())
                                If preffix = String.Empty Then
                                    preffix = ", "
                                End If
                            Next
                            result = resultBuilder.ToString()
                        Case "labels"
                            Dim labelsArray As JArray = DirectCast(field.Value, JArray)
                            Dim resultBuilder As StringBuilder = New StringBuilder()
                            Dim preffix = String.Empty
                            For i = 0 To labelsArray.Count - 1
                                resultBuilder.Append(preffix)
                                resultBuilder.Append(labelsArray(i).ToString())
                                If preffix = String.Empty Then
                                    preffix = ", "
                                End If
                            Next
                            result = resultBuilder.ToString()
                        Case "issuelinks"
                            Dim attachmentsArray As JArray = DirectCast(field.Value, JArray)
                            Dim resultBuilder As StringBuilder = New StringBuilder()
                            Dim preffix = String.Empty
                            For i = 0 To attachmentsArray.Count - 1
                                resultBuilder.Append(preffix)
                                If attachmentsArray(i)("outwardIssue") IsNot Nothing Then
                                    resultBuilder.Append(attachmentsArray(i)("outwardIssue")("key").ToString())
                                Else
                                    resultBuilder.Append(attachmentsArray(i)("inwardIssue")("key").ToString())
                                End If
                                If preffix = String.Empty Then
                                    preffix = ", "
                                End If
                            Next
                            result = resultBuilder.ToString()
                        Case "versions", "fixversions"
                            Dim attachmentsArray As JArray = DirectCast(field.Value, JArray)
                            Dim resultBuilder As StringBuilder = New StringBuilder()
                            Dim preffix = String.Empty
                            For i = 0 To attachmentsArray.Count - 1
                                resultBuilder.Append(preffix)
                                resultBuilder.Append(attachmentsArray(i)("description").ToString())
                                If preffix = String.Empty Then
                                    preffix = ", "
                                End If
                            Next
                            result = resultBuilder.ToString()
                        Case "subtasks"
                            Dim attachmentsArray As JArray = DirectCast(field.Value, JArray)
                            Dim resultBuilder As StringBuilder = New StringBuilder()
                            Dim preffix = String.Empty
                            For i = 0 To attachmentsArray.Count - 1
                                resultBuilder.Append(preffix)
                                resultBuilder.Append(attachmentsArray(i)("fields")("summary").ToString())
                                If preffix = String.Empty Then
                                    preffix = ", "
                                End If
                            Next
                            result = resultBuilder.ToString()
                        Case "components"
                            Dim attachmentsArray As JArray = DirectCast(field.Value, JArray)
                            Dim resultBuilder As StringBuilder = New StringBuilder()
                            Dim preffix = String.Empty
                            For i = 0 To attachmentsArray.Count - 1
                                resultBuilder.Append(preffix)
                                resultBuilder.Append(attachmentsArray(i)("name").ToString())
                                If preffix = String.Empty Then
                                    preffix = ", "
                                End If
                            Next
                            result = resultBuilder.ToString()
                    End Select
                End If
                Return result
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

                Dim ticketActionsLinked As ActionLinkToJira = New ActionLinkToJira(User)
                ticketActionsLinked.LoadByTicketID(ticketID)

                For i = 0 To comments("comments").Count - 1
                    If GetIsNewComment(comments("comments")(i), ticketActionsLinked) Then
                        result.Add(comments("comments")(i))
                    End If
                Next
                Return result
            End Function

            Private Function GetIsNewComment(ByVal comment As JObject, ByVal ticketActionsLinked As ActionLinkToJira) As Boolean
                Dim result As Boolean = False

                If comment("visibility") Is Nothing Then
                    If comment("body").ToString().Length < 20 OrElse comment("body").ToString().Substring(0, 20) <> "TeamSupport ticket #" Then
                        Dim pulledComment As ActionLinkToJiraItem = ticketActionsLinked.FindByJiraID(CType(comment("id").ToString(), Integer?))
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
                        updateActions(0).Description = newComments(i)("body").ToString()

                        Dim actionLinkToJira As ActionLinkToJira = New ActionLinkToJira(User)
                        Dim actionLinkToJiraItem As ActionLinkToJiraItem = actionLinkToJira.AddNewActionLinkToJiraItem()
                        actionLinkToJiraItem.JiraID = CType(newComments(i)("id").ToString(), Integer)
                        actionLinkToJiraItem.DateModifiedByJiraSync = DateTime.UtcNow
                        updateActions.Save()
                        actionLinkToJiraItem.ActionID = updateActions(0).ActionID
                        actionLinkToJira.Save()

                        If crmLinkError IsNot Nothing Then
                            crmLinkError.Delete()
                            crmLinkActionErrors.Save()
                        End If

                    Catch ex As Exception

                        If crmLinkError Is Nothing Then
                            Dim newCrmLinkError As CRMLinkErrors = New CRMLinkErrors(Me.User)
                            crmLinkError = newCrmLinkError.AddNewCRMLinkError()
                            crmLinkError.OrganizationID = CRMLinkRow.OrganizationID
                            crmLinkError.CRMType = CRMLinkRow.CRMType
                            crmLinkError.Orientation = "in"
                            crmLinkError.ObjectType = "action"
                            crmLinkError.ObjectID = newComments(i)("id").ToString()
                            crmLinkError.ObjectData = newComments(i)("body").ToString()
                            crmLinkError.Exception = ex.ToString() + ex.StackTrace
                            crmLinkError.OperationType = "create"
                            newCrmLinkError.Save()
                        Else
                            crmLinkError.ObjectData = newComments(i)("body").ToString()
                            crmLinkError.Exception = ex.ToString() + ex.StackTrace
                        End If

                    End Try
                Next
            End Sub

        End Class
  End Namespace
End Namespace