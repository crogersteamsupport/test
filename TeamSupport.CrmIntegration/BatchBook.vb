Imports System.Xml
Imports System.Net
Imports System.IO
Imports TeamSupport.Data

Namespace TeamSupport
    Namespace CrmIntegration

        Public Class BatchBook
            Inherits Integration

            Public Sub New(ByVal crmLinkOrg As CRMLinkTableItem, ByVal crmLog As SyncLog, ByVal thisUser As LoginUser, ByVal thisProcessor As CrmProcessor)
                MyBase.New(crmLinkOrg, crmLog, thisUser, thisProcessor, IntegrationType.Batchbook)
            End Sub

            Public Overrides Function PerformSync() As Boolean
                Dim Success As Boolean = True

                Success = SyncAccounts()

                If Success Then
                    Success = SendTicketData()
                End If

                Return Success
            End Function

            Private Function SyncAccounts() As Boolean
                Dim Key As String = CRMLinkRow.SecurityToken
                Dim CompanyName As String = CRMLinkRow.Username
                Dim ParentOrgID As String = CRMLinkRow.OrganizationID
                Dim TagsToMatch As String = CRMLinkRow.TypeFieldMatch

                Dim CompaniesToSync As XmlDocument
                Dim CompanySyncData As List(Of CompanyData) = Nothing

                'first retrieve a list of company data from batchbook
                If TagsToMatch.Contains(",") Then
                    'process multiple tags where present
                    For Each TagToMatch As String In TagsToMatch.Split(",")
                        If Processor.IsStopped Then
                            Return False
                        End If

                        CompaniesToSync = GetBatchBookXML(Key, CompanyName, "companies.xml?tag=" & Trim(TagToMatch))

                        If CompaniesToSync IsNot Nothing Then
                            If CompanySyncData Is Nothing Then
                                CompanySyncData = New List(Of CompanyData)()
                            End If

                            Dim thisCompanyData As List(Of CompanyData) = ParseCompanyXML(CompaniesToSync, CRMLinkRow.LastLink)

                            'avoid duplicates by only adding companies that aren't already in the list
                            For Each newData As CompanyData In thisCompanyData
                                Dim alreadyExists As Boolean = False
                                For Each existingData As CompanyData In CompanySyncData
                                    If newData.Equals(existingData) Then
                                        alreadyExists = True
                                    End If
                                Next

                                If Not alreadyExists Then
                                    CompanySyncData.Add(newData)
                                End If
                            Next
                        End If
                    Next
                Else
                    'only one tag, don't need to loop
                    CompaniesToSync = GetBatchBookXML(Key, CompanyName, "companies.xml?tag=" & TagsToMatch)

                    If CompaniesToSync IsNot Nothing Then
                        CompanySyncData = New List(Of CompanyData)()
                        CompanySyncData = ParseCompanyXML(CompaniesToSync, CRMLinkRow.LastLink)
                    End If
                End If

                'then use company data to update teamsupport
                If CompanySyncData IsNot Nothing Then
                    Log.Write(String.Format("Processed {0} accounts.", CompanySyncData.Count))

                    For Each company As CompanyData In CompanySyncData
                        UpdateOrgInfo(company, ParentOrgID)
                    Next

                    Log.Write("Finished updating account information.")
                    Log.Write("Updating people information...")

                    For Each company As CompanyData In CompanySyncData
                        Dim PeopleToSync As XmlDocument = GetBatchBookXML(Key, CompanyName, "companies/" & company.AccountID & "/people.xml")
                        If PeopleToSync IsNot Nothing Then
                            Dim PeopleSyncData As List(Of EmployeeData) = ParsePeopleXML(PeopleToSync)

                            If PeopleSyncData IsNot Nothing Then
                                For Each person As EmployeeData In PeopleSyncData
                                    UpdateContactInfo(person, company.AccountID, ParentOrgID)
                                Next
                            End If
                        End If

                        Log.Write("Updated people information for " & company.AccountID)
                    Next

                    Log.Write("Finished updating people information")
                End If

                Return True
            End Function

            Private Function SendTicketData() As Boolean
                Dim ParentOrgID As String = CRMLinkRow.OrganizationID

                If CRMLinkRow.SendBackTicketData Then
                    'get tickets created after the last link date
                    Dim tickets As New Tickets(User)
                    tickets.LoadByCRMLinkItem(CRMLinkRow)

                    If tickets IsNot Nothing Then
                        For Each thisTicket As Ticket In tickets
                            If Processor.IsStopped Then
                                Return False
                            End If

                            Dim description As Action = Actions.GetTicketDescription(User, thisTicket.TicketID)
                            Dim customers As New OrganizationsView(User)
                            customers.LoadByTicketID(thisTicket.TicketID)

                            Dim NoteBody As String = String.Format("A ticket has been created for this organization entitled ""{0}"".{3}{2}{3}Click here to access the ticket information: https://app.teamsupport.com/Ticket.aspx?ticketid={1}", _
                                                                   thisTicket.Name, thisTicket.TicketID.ToString(), Utilities.StripHTML(description.Description), Environment.NewLine)

                            For Each customer As OrganizationsViewItem In customers
                                If customer.CRMLinkID <> "" Then
                                    Log.Write("Creating a comment...")
                                    If CreateComment(CRMLinkRow.SecurityToken, CRMLinkRow.Username, customer.CRMLinkID, NoteBody) Then
                                        Log.Write("Comment created successfully.")

                                        CRMLinkRow.LastTicketID = thisTicket.TicketID
                                        CRMLinkRow.Collection.Save()
                                    End If
                                End If
                            Next
                        Next
                    Else
                        Log.Write("No new tickets to sync.")
                    End If
                Else
                    Log.Write("Ticket data not sent since SendBackTicketData is set to FALSE for this organization.")
                End If

                Return True
            End Function

            Private Function ParseCompanyXML(ByRef CompaniesToSync As XmlDocument, ByVal LastSync As Date?) As List(Of CompanyData)
                Dim CompanySyncData As List(Of CompanyData) = Nothing

                'parse the xml doc to get information about each customer
                Dim allcust As XElement = XElement.Load(New XmlNodeReader(CompaniesToSync))

                If allcust.Descendants("company").Count > 0 Then
                    CompanySyncData = New List(Of CompanyData)()
                End If

                For Each company As XElement In allcust.Descendants("company")
                    If LastSync Is Nothing Or Date.ParseExact(company.Element("updated_at").Value, "ddd MMM dd HH:mm:ss 'UTC' yyyy", Nothing).AddMinutes(30) > LastSync Then
                        Dim thisCustomer As New CompanyData()
                        Dim address As XElement = company.Element("locations").Element("location")

                        With thisCustomer
                            .AccountID = company.Element("id").Value
                            .AccountName = company.Element("name").Value

                            If address IsNot Nothing Then
                                .City = address.Element("city").Value
                                .Country = address.Element("country").Value
                                .State = address.Element("state").Value
                                .Street = address.Element("street_1").Value
                                .Zip = address.Element("postal_code").Value
                                .Phone = address.Element("phone").Value
                            End If
                        End With

                        CompanySyncData.Add(thisCustomer)

                    End If
                    'TODO: handle large numbers of customers?
                Next

                Return CompanySyncData
            End Function

            Private Function ParsePeopleXML(ByRef PeopleToSync As XmlDocument) As List(Of EmployeeData)
                Dim EmployeeSyncData As List(Of EmployeeData) = Nothing

                Dim allpeople As XElement = XElement.Load(New XmlNodeReader(PeopleToSync))

                If allpeople.Descendants("person").Count > 0 Then
                    EmployeeSyncData = New List(Of EmployeeData)()
                End If

                For Each person As XElement In allpeople.Descendants("person")
                    Dim thisPerson As New EmployeeData()
                    Dim address As XElement = person.Element("locations").Element("location")

                    With thisPerson
                        .FirstName = person.Element("first_name").Value
                        .LastName = person.Element("last_name").Value
                        .Title = person.Element("title").Value

                        If address IsNot Nothing Then
                            .Email = address.Element("email").Value
                            .Phone = address.Element("phone").Value
                            .Cell = address.Element("phone").Value
                            .Fax = address.Element("fax").Value
                        End If
                    End With

                    EmployeeSyncData.Add(thisPerson)
                Next

                Return EmployeeSyncData
            End Function

            Private Function GetBatchBookXML(ByVal Key As String, ByVal CompanyName As String, ByVal PathAndQuery As String) As XmlDocument
                Dim BBUri As New Uri("https://" & CompanyName & ".batchbook.com/service/" & PathAndQuery)
                Dim returnXML As XmlDocument = Nothing

                If CompanyName <> "" Then
                    returnXML = GetXML(New NetworkCredential(Key, "X"), BBUri)
                End If
                Return returnXML
            End Function

            'returns a boolean value to indicate whether or not comment was created successfully
            Private Function CreateComment(ByVal Key As String, ByVal CompanyName As String, ByVal AccountID As String, ByVal NoteBody As String) As Boolean
                Dim success As Boolean = False
                Dim statusCode As HttpStatusCode

                Dim BBUri As New Uri("https://" & CompanyName & ".batchbook.com/service/companies/" & AccountID & "/comments.xml")
                Dim postData As String = "<comment><comment><![CDATA[" & NoteBody & "]]></comment></comment>"

                If CompanyName <> "" Then
                    statusCode = PostXML(New NetworkCredential(Key, "X"), BBUri, postData)
                End If

                success = statusCode = HttpStatusCode.Created

                Return success
            End Function

        End Class

    End Namespace
End Namespace