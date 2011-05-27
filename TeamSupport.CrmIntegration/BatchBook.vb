Imports System.Xml
Imports System.Net
Imports System.IO

Public Class BatchBook
    Private Const success As String = "Completed"
    Private Const Integration As IntegrationType = IntegrationType.Batchbook

    Public Shared Function PerformBatchBookSync(ByVal Key As String, ByVal CompanyName As String, ByVal ParentOrgID As String, ByVal TagsToMatch As String, ByRef AppLog As TextBox) As String
        Dim TS As New TSCheckData()
        Dim LastSync As Date = Date.Parse(TSCheckData.GetLastCRMUpdate(ParentOrgID, Integration))
        Dim CompaniesToSync As XmlDocument
        Dim CompanySyncData As List(Of CompanyData) = Nothing

        'first retrieve a list of company data from batchbook
        If TagsToMatch.Contains(",") Then
            'process multiple tags where present
            For Each TagToMatch As String In TagsToMatch.Split(",")
                CompaniesToSync = GetBatchBookXML(Key, CompanyName, "companies.xml?tag=" & Trim(TagToMatch))

                If CompaniesToSync IsNot Nothing Then
                    If CompanySyncData Is Nothing Then
                        CompanySyncData = New List(Of CompanyData)()
                    End If

                    Dim thisCompanyData As List(Of CompanyData) = ParseCompanyXML(CompaniesToSync, LastSync)

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
                CompanySyncData = ParseCompanyXML(CompaniesToSync, LastSync)
            End If
        End If

        'then use company data to update teamsupport
        If CompanySyncData IsNot Nothing Then
            TS.PublicAddText(String.Format("Processed {0} accounts.", CompanySyncData.Count), AppLog)

            For Each company As CompanyData In CompanySyncData
                TS.AddOrUpdateAccountInformation(company.AccountID, company.AccountName, company.Street, "", company.City, company.State, company.Zip, company.Country, company.Phone, ParentOrgID)
                TS.PublicAddText("Updated w/ Address:  " + company.AccountName, AppLog)
            Next

            TS.PublicAddText("Finished updating account information.", AppLog)
            TS.PublicAddText("Updating people information...", AppLog)

            For Each company As CompanyData In CompanySyncData
                Dim PeopleToSync As XmlDocument = GetBatchBookXML(Key, CompanyName, "companies/" & company.AccountID & "/people.xml")
                If PeopleToSync IsNot Nothing Then
                    Dim PeopleSyncData As List(Of EmployeeData) = ParsePeopleXML(PeopleToSync)

                    If PeopleSyncData IsNot Nothing Then
                        For Each person As EmployeeData In PeopleSyncData
                            TS.AddOrUpdateContactInformation(company.AccountID, person.Email, person.FirstName, person.LastName, person.Phone, person.Fax, person.Cell, person.Title, 0, ParentOrgID)
                        Next
                    End If
                End If

                TS.PublicAddText("Updated people information for " + company.AccountID, AppLog)
            Next

            TS.PublicAddText("Finished updating people information", AppLog)
        End If

        Return success
    End Function

    Public Shared Sub SendTicketData(ByVal Key As String, ByVal CompanyName As String, ByVal ParentOrgID As String, ByRef AppLog As TextBox)
        Dim TS As New TSCheckData()

        If TSCheckData.SendBackTicketData(ParentOrgID, Integration) Then
            Dim tickets As DataTable

            'get tickets created after the last link date
            tickets = TSCheckData.GetLatestTickets(ParentOrgID, Integration)

            If tickets IsNot Nothing Then
                For Each thisTicket As DataRow In tickets.Rows
                    'strip out HTML from descriptions
                    Dim NoteBody As String = String.Format("A ticket has been created for this organization entitled ""{0}"".{3}{2}{3}Click here to access the ticket information: https://app.teamsupport.com/Ticket.aspx?ticketid={1}", _
                                                           thisTicket(1).ToString(), thisTicket(0).ToString(), Utilities.StripHTML(thisTicket(4).ToString()), Environment.NewLine)

                    TS.PublicAddText("Creating a comment...", AppLog)

                    If CreateComment(Key, CompanyName, thisTicket(3).ToString(), NoteBody) Then
                        TS.PublicAddText("Comment created successfully.", AppLog)
                    End If
                Next
            End If
        Else
            TS.PublicAddText("Ticket data not sent since SendBackTicketData is set to FALSE for this organization.", AppLog)
        End If

    End Sub

    Private Shared Function ParseCompanyXML(ByRef CompaniesToSync As XmlDocument, ByVal LastSync As Date) As List(Of CompanyData)
        Dim CompanySyncData As List(Of CompanyData) = Nothing

        'parse the xml doc to get information about each customer
        Dim allcust As XElement = XElement.Load(New XmlNodeReader(CompaniesToSync))

        If allcust.Descendants("company").Count > 0 Then
            CompanySyncData = New List(Of CompanyData)()
        End If

        For Each company As XElement In allcust.Descendants("company")
            If Date.ParseExact(company.Element("updated_at").Value, "ddd MMM dd HH:mm:ss 'UTC' yyyy", Nothing).AddMinutes(30) > LastSync Then
                Dim thisCustomer As New CompanyData()
                Dim address As XElement = company.Element("locations").Element("location")

                With thisCustomer
                    .AccountID = company.Element("id").Value
                    .AccountName = company.Element("name").Value

                    If address IsNot Nothing Then
                        .City = address.Element("city").Value
                        .Country = address.Element("country").Value
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

    Private Shared Function ParsePeopleXML(ByRef PeopleToSync As XmlDocument) As List(Of EmployeeData)
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

    Private Shared Function GetBatchBookXML(ByVal Key As String, ByVal CompanyName As String, ByVal PathAndQuery As String) As XmlDocument
        Dim BBUri As New Uri("https://" & CompanyName & ".batchbook.com/service/" & PathAndQuery)
        Dim returnXML As XmlDocument = Nothing

        If CompanyName <> "" Then
            returnXML = Utilities.GetXML(New NetworkCredential(Key, "X"), BBUri)
        End If
        Return returnXML
    End Function

    'returns a boolean value to indicate whether or not comment was created successfully
    Private Shared Function CreateComment(ByVal Key As String, ByVal CompanyName As String, ByVal AccountID As String, ByVal NoteBody As String) As Boolean
        Dim success As Boolean = False
        Dim statusCode As HttpStatusCode

        Dim BBUri As New Uri("https://" & CompanyName & ".batchbook.com/service/companies/" & AccountID & "/comments.xml")
        Dim postData As String = "<comment><comment><![CDATA[" & NoteBody & "]]></comment></comment>"
        
        If CompanyName <> "" Then
            statusCode = Utilities.PostXML(New NetworkCredential(Key, "X"), BBUri, postData)
        End If

        success = statusCode = HttpStatusCode.Created

        Return success
    End Function
End Class

Public Class CompanyData
    Property City As String
    Property Country As String
    Property State As String
    Property Street As String
    Property Zip As String
    Property Phone As String
    Property AccountID As String
    Property AccountName As String

    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        If Not TypeOf (obj) Is CompanyData Then
            Return False
        End If

        Dim compareObj As CompanyData = CType(obj, CompanyData)
        If compareObj.City = Me.City And compareObj.Country = Me.Country And compareObj.State = Me.State And compareObj.Street = Me.Street And compareObj.Zip = Me.Zip _
                And compareObj.Phone = Me.Phone And compareObj.AccountID = Me.AccountID And compareObj.AccountName = Me.AccountName Then
            Return True
        Else
            Return False
        End If
    End Function
End Class

Public Class EmployeeData
    Property FirstName As String
    Property LastName As String
    Property Title As String
    Property Email As String
    Property Phone As String
    Property Cell As String
    Property Fax As String
End Class