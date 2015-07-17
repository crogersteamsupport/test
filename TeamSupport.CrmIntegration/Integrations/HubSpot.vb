Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports TeamSupport.Data
Imports Objects = TeamSupport.ServiceLibrary.HubSpotSources.Objects
Imports TeamSupport.ServiceLibrary

Namespace TeamSupport
  Namespace CrmIntegration
    Public Class HubSpot
      Inherits Integration

      Private _cmrLink As CRMLinkTableItem
      Private _integrationType As IntegrationType
      Private _thisUser As LoginUser
      Private _crmLogPath As String
      Private _hubspotAccountIds As List(Of String)

      Public Sub New(ByVal crmLinkOrg As CRMLinkTableItem, ByVal syncLog As SyncLog, ByVal crmLogPath As String, ByVal thisUser As LoginUser, ByVal thisProcessor As CrmProcessor)
        MyBase.New(crmLinkOrg, syncLog, thisUser, thisProcessor, IntegrationType.HubSpot)
        _cmrLink = crmLinkOrg
        _integrationType = IntegrationType.HubSpot
        _thisUser = thisUser
        _crmLogPath = crmLogPath
      End Sub

      Public Overrides Function PerformSync() As Boolean
          Dim Success As Boolean = True

          Success = SyncAccounts()

          If Success Then
            _hubspotAccountIds = New List(Of String)
            Success = SendTicketData(AddressOf CreateNote)
            _hubspotAccountIds.Clear()
          End If

          Return Success
      End Function

      Private Function SyncAccounts() As Boolean
          Dim hapiKey As String = CRMLinkRow.SecurityToken1
          Dim parentOrgId As String = CRMLinkRow.OrganizationID
          SyncError = False

          Try
            Dim crmLinkErrors As CRMLinkErrors = New CRMLinkErrors(Me.User)
            crmLinkErrors.LoadByOperation(CRMLinkRow.OrganizationID, CRMLinkRow.CRMType, "in", "company")
            Dim crmLinkError As CRMLinkError = Nothing

            If Processor.IsStopped Then
                Return False
            End If

            Dim hubspotApi As Companies = New Companies(apiKey:=hapiKey, logPath:=_crmLogPath)
            Dim offset As String = Nothing
            Dim maxCount As Integer = 100
            Dim hubspotCompanies As Objects.Companies.RootObject = New Objects.Companies.RootObject()
            Dim companySyncData As New List(Of CompanyData)()

            Log.Write(String.Format("Get and process only Companies in the lifecycle ""Customer"""))

            Do
              hubspotCompanies = hubspotApi.GetAllRecentlyModified(count:=maxCount, offset:=offset)
              offset = hubspotCompanies.offset

              For Each company As Objects.Companies.Result In hubspotCompanies.results
                If Processor.IsStopped Then
                  Return False
                End If

                'Get only the Companies that are the "Customer" lifecycle
                If (company.properties.lifecyclestage IsNot Nothing AndAlso Not String.IsNullOrEmpty(company.properties.lifecyclestage.value) AndAlso company.properties.lifecyclestage.value.ToLower() = "customer") Then
                  Dim modifiedValue As Long = company.properties.hs_lastmodifieddate.timestamp
                  Dim beginTicks As Long = New Date(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks
                  Dim modifiedDate As Date = New Date(beginTicks + modifiedValue * 10000)

                  If CRMLinkRow.LastLink Is Nothing OrElse modifiedDate.AddMinutes(30) > CRMLinkRow.LastLink Then
                    Dim thisCustomer As New CompanyData()

                    With thisCustomer
                      .AccountID = company.companyId
                      .AccountName = If(company.properties.name IsNot Nothing, company.properties.name.value, String.Empty)
                      .Street = If(company.properties.address IsNot Nothing, company.properties.address.value, String.Empty)
                      .Street2 = If(company.properties.address2 IsNot Nothing, company.properties.address2.value, String.Empty)
                      .City = If(company.properties.city IsNot Nothing, company.properties.city.value, String.Empty)
                      .State = If(company.properties.state IsNot Nothing, company.properties.state.value, String.Empty)
                      .Zip = If(company.properties.zip IsNot Nothing, company.properties.zip.value, String.Empty)
                      .Country = If(company.properties.country IsNot Nothing, company.properties.country.value, String.Empty)
                      .Phone = If(company.properties.phone IsNot Nothing, company.properties.phone.value, String.Empty)
                      .Fax = If(company.properties.fax IsNot Nothing, company.properties.fax.value, String.Empty)
                    End With

                    companySyncData.Add(thisCustomer)
                  End If
                Else
                  Log.Write(String.Format("Company {0} ({1}) not processed, is in lifecycle ""{2}""", company.properties.name.value, company.companyId.ToString(), If(company.properties.lifecyclestage IsNot Nothing, company.properties.lifecyclestage.value, "")))
                End If
              Next
            Loop While hubspotCompanies.hasMore

            Log.Write(companySyncData.Count.ToString() + " accounts found to update.")

            For Each company As CompanyData In companySyncData
              crmLinkError = crmLinkErrors.FindByObjectIDAndFieldName(company.AccountID, String.Empty)

              'Go through all accounts we just processed and add to the TS database
              Try
                UpdateOrgInfo(company, parentOrgId)

                If crmLinkError IsNot Nothing Then
                  crmLinkError.Delete()
                  crmLinkErrors.Save()
                End If
              Catch ex As Exception
                If crmLinkError Is Nothing Then
                  Dim newCrmLinkError As CRMLinkErrors = New CRMLinkErrors(Me.User)
                  crmLinkError = newCrmLinkError.AddNewCRMLinkError()
                  crmLinkError.OrganizationID = CRMLinkRow.OrganizationID
                  crmLinkError.CRMType = CRMLinkRow.CRMType
                  crmLinkError.Orientation = "in"
                  crmLinkError.ObjectType = "company"
                  crmLinkError.ObjectID = company.AccountID
                  crmLinkError.ObjectData = JsonConvert.SerializeObject(company)
                  crmLinkError.Exception = ex.ToString() + ex.StackTrace
                  crmLinkError.OperationType = "unknown"
                  newCrmLinkError.Save()
                Else
                  crmLinkError.ObjectData = JsonConvert.SerializeObject(company)
                  crmLinkError.Exception = ex.ToString() + ex.StackTrace
                End If
              End Try
            Next

            Log.Write("Finished updating account information. About to start updating contact information...")
            GetContacts()
            Log.Write("Finished updating contact information")
            crmLinkErrors.Save()
          Catch ex As Exception
              SyncError = True
              ErrorCode = IntegrationError.Unknown
              Log.Write(String.Format("Error in Perform HubSpot Sync: {0}{1}{2}", ex.Message, Environment.NewLine, ex.InnerException))
          End Try

        Return Not SyncError
      End Function

      Private Sub GetContacts()
        Dim hapiKey As String = CRMLinkRow.SecurityToken1
        Dim contactOffset As String = Nothing
        Dim maxCount As Integer = 100
        Dim hubSpotApi As Contacts = New Contacts(apiKey:=hapiKey, logPath:=_crmLogPath)
        Dim hubspotContacts As Objects.Contacts.RootObject = New Objects.Contacts.RootObject()
        Dim ContactSyncData As List(Of EmployeeData) = New List(Of EmployeeData)()
        Dim crmLinkContactErrors As CRMLinkErrors = New CRMLinkErrors(Me.User)
        crmLinkContactErrors.LoadByOperation(CRMLinkRow.OrganizationID, CRMLinkRow.CRMType, "in", "contact")

        Log.Write(String.Format("Get and process only Contacts in the lifecycle ""Customer"""))

        Do
          hubspotContacts = hubSpotApi.GetAllRecentlyModified(count:=maxCount, contactOffset:=contactOffset)
          contactOffset = hubspotContacts.offset

          If hubspotContacts.contacts IsNot Nothing AndAlso hubspotContacts.contacts.Count > 0 Then
            For Each companyContact As Objects.Contacts.Contact In hubspotContacts.contacts
              Try
                Dim modifiedValue As Long = companyContact.properties.lastmodifieddate.value
                Dim beginTicks As Long = New Date(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks
                Dim modifiedDate As Date = New Date(beginTicks + modifiedValue * 10000)

                If CRMLinkRow.LastLink Is Nothing OrElse modifiedDate.AddMinutes(30) > CRMLinkRow.LastLink Then
                  Dim thisPerson As New EmployeeData()
                  Dim hubSpotApiContact As Contacts = New Contacts(apiKey:=hapiKey, logPath:=_crmLogPath)
                  Dim contact As Objects.Contact.RootObject = hubSpotApiContact.GetContactById(companyContact.vid.ToString())

                  'Get only contacts associated to a company and also in the 'Customer' lifecycle
                  If contact.associatedCompany IsNot Nothing _
                    AndAlso contact.associatedCompany.companyId > 0 _
                    AndAlso Not String.IsNullOrEmpty(contact.properties.associatedcompanyid.value) _
                    AndAlso contact.properties.lifecyclestage IsNot Nothing _
                    AndAlso Not String.IsNullOrEmpty(contact.properties.lifecyclestage.value) _
                    AndAlso contact.properties.lifecyclestage.value.ToLower() = "customer" Then
                    With thisPerson
                      .HubSpotVid = contact.vid
                      .HubSpotId = contact.properties.associatedcompanyid.value
                      .FirstName = If(contact.properties.firstname IsNot Nothing, contact.properties.firstname.value, String.Empty)
                      .LastName = If(contact.properties.lastname IsNot Nothing, contact.properties.lastname.value, String.Empty)
                      .Title = If(contact.properties.title IsNot Nothing, contact.properties.title.value, String.Empty)
                      .Cell = If(contact.properties.mobilephone IsNot Nothing, contact.properties.mobilephone.value, String.Empty)
                      .Phone = If(contact.properties.phone IsNot Nothing, contact.properties.phone.value, String.Empty)
                      .Fax = If(contact.properties.fax IsNot Nothing, contact.properties.fax.value, String.Empty)
                      .Email = If(contact.properties.email IsNot Nothing, contact.properties.email.value, String.Empty)
                    End With

                    ContactSyncData.Add(thisPerson)
                  Else
                    Log.Write(String.Format("Contact {0} {1} ({2}) not processed, is in lifecycle ""{3}"" or is not associated to any company", contact.properties.firstname.value, contact.properties.lastname.value, contact.vid.ToString(), contact.properties.lifecyclestage.value))
                  End If
                End If
              Catch ex As Exception
                Log.Write(String.Format("Error getting contact data. vid: {0}{1}{2}{1}{3}", companyContact.vid, Environment.NewLine, ex.Message, Environment.NewLine, ex.InnerException))
              End Try
            Next
          End If
        Loop While hubspotContacts.HasMoreRecords

        If ContactSyncData IsNot Nothing AndAlso ContactSyncData.Count > 0 Then
            Log.Write(String.Format("{0} contacts to update...", ContactSyncData.Count.ToString()))

            For Each contact As EmployeeData In ContactSyncData
                Dim crmLinkError As CRMLinkError = Nothing
                crmLinkError = crmLinkContactErrors.FindByObjectIDAndFieldName(contact.HubSpotVid, String.Empty)

                Try
                  UpdateContactInfo(contact, contact.HubSpotId, CRMLinkRow.OrganizationID)

                  If crmLinkError IsNot Nothing Then
                    crmLinkError.Delete()
                    crmLinkContactErrors.Save()
                  End If
                Catch ex As Exception
                  If crmLinkError Is Nothing Then
                    Dim newCrmLinkError As CRMLinkErrors = New CRMLinkErrors(Me.User)
                    crmLinkError = newCrmLinkError.AddNewCRMLinkError()
                    crmLinkError.OrganizationID = CRMLinkRow.OrganizationID
                    crmLinkError.CRMType = CRMLinkRow.CRMType
                    crmLinkError.Orientation = "in"
                    crmLinkError.ObjectType = "contact"
                    crmLinkError.ObjectID = contact.HubSpotVid
                    crmLinkError.ObjectData = JsonConvert.SerializeObject(contact)
                    crmLinkError.Exception = ex.ToString() + ex.StackTrace
                    crmLinkError.OperationType = "unknown"
                    newCrmLinkError.Save()
                  Else
                    crmLinkError.ObjectData = JsonConvert.SerializeObject(contact)
                    crmLinkError.Exception = ex.ToString() + ex.StackTrace
                  End If
                End Try
            Next
        Else
            Log.Write("No contacts to update.")
        End If

        crmLinkContactErrors.Save()
      End Sub

      Private Function CreateNote(ByVal accountId As String, ByVal thisTicket As Ticket) As Boolean
        Dim isSuccessful = False
        Dim hapiKey As String = CRMLinkRow.SecurityToken1
        Dim hubSpotApiCompany As Companies = New Companies(apiKey:=hapiKey, logPath:=_crmLogPath)

        Try
          If Not _hubspotAccountIds.Contains(accountId) AndAlso hubSpotApiCompany.GetById(accountId, False).companyId > 0 Then
            _hubspotAccountIds.Add(accountId)
          End If


          If _hubspotAccountIds.Contains(accountId) Then
            Dim authorName As String = Nothing

            Using findAuthor As New Users(User)
                findAuthor.LoadByUserID(thisTicket.CreatorID)

                If findAuthor.Count > 0 Then
                  Dim author As User
                  author = findAuthor(0)

                  If author IsNot Nothing Then
                    authorName = author.FirstLastName
                  End If
                End If
            End Using

            Dim action As Action = Actions.GetTicketDescription(User, thisTicket.TicketID)
            Dim description = String.Empty

            If action IsNot Nothing Then
              description = HtmlUtility.StripHTML(action.Description)
            End If

            Dim noteBody As String = String.Format("A ticket has been created for this organization entitled ""{0}"".{3}{2}{3}Click here to access the ticket information: https://app.teamsupport.com/Ticket.aspx?ticketid={1}{3}{4}", _
                                  thisTicket.Name, thisTicket.TicketID, description, Environment.NewLine, If(authorName IsNot Nothing, "Created by " & authorName, ""))

            Dim hubSpotApi As Engagements = New Engagements(apiKey:=hapiKey, logPath:=_crmLogPath)

            Dim newEngagement As Objects.Engagement.RootObject = New Objects.Engagement.RootObject()
            newEngagement.engagement = New Objects.Engagement.EngagementItem()
            newEngagement.engagement.active = True
            newEngagement.engagement.type = "NOTE"
            newEngagement.engagement.timestamp = (DateTime.UtcNow - New DateTime(1970, 1, 1)).TotalMilliseconds
            newEngagement.associations = New Objects.Engagement.Associations()
            newEngagement.associations.companyIds = New List(Of Integer)(New Integer() {accountId})
            newEngagement.metadata = New Objects.Engagement.Metadata()
            newEngagement.metadata.body = noteBody
            Dim engagementCreated As Objects.Engagement.RootObject = hubSpotApi.Create(newEngagement)

            isSuccessful = engagementCreated.engagement.id > 0
          Else
            Log.Write(String.Format("The HubSpot accountId {0} in the crmlinkid field of ticket {1} (id: {2}) was not found in HubSpot. Note was not created.", accountId, thisTicket.TicketNumber, thisTicket.TicketID.ToString()))
          End If
        Catch ex As Exception
          Log.Write(String.Format("Exception creating NOTE: {0}{1}{2}", ex.Message, Environment.NewLine, ex.InnerException.ToString()))
          isSuccessful = False
        End Try

        Return isSuccessful
      End Function
    End Class
  End Namespace
End Namespace