Imports TeamSupport.Data
Imports System.Net
Imports System.Web
Imports System.Xml

Namespace TeamSupport
    Namespace CrmIntegration

        Public Class Zoho
            Inherits Integration

            Private APITicket As String

            Public Sub New(ByVal crmLinkOrg As CRMLinkTableItem, ByVal crmLog As SyncLog, ByVal thisUser As LoginUser, ByVal thisProcessor As CrmProcessor)
                MyBase.New(crmLinkOrg, crmLog, thisUser, thisProcessor, IntegrationType.Zoho)
            End Sub

            Public Overrides Function PerformSync() As Boolean
                Dim Success As Boolean = False

                If GenerateTicket() Then

                    Try
                        'sync accounts
                        Success = NewSyncAccounts(AddressOf GetZohoCompanyXML, _
                                                  AddressOf ParseZohoCompanyXML, _
                                                  AddressOf GetZohoPeopleXML, _
                                                  AddressOf ParseZohoPeopleXML)

                        'send ticket data
                        Success = SendTicketData(AddressOf CreateNote)
                    Catch ex As Exception
                        Log.Write("exception: " & ex.Message & ": " & ex.StackTrace)
                        Success = False
                    Finally
                        Logout()
                    End Try
                Else
                    Log.Write("Error generating API Ticket.")
                End If

                Return Success
            End Function

            Private Function ParseZohoCompanyXML(ByVal CompaniesToSync As XmlDocument) As List(Of CompanyData)
                Dim CompanySyncData As List(Of CompanyData) = Nothing

                Dim allaccounts As XElement = XElement.Load(New XmlNodeReader(CompaniesToSync))

                If allaccounts.Descendants("row").Count > 0 Then
                    CompanySyncData = New List(Of CompanyData)()
                End If

                For Each company As XElement In allaccounts.Descendants("row")
                    If Processor.IsStopped Then
                        Return Nothing
                    End If

                    Dim thisCustomer As New CompanyData()

                    With thisCustomer
                        'Zoho's XML is really ugly so we have to parse it in a really ugly way
                        For Each dataitem As XElement In company.Descendants("FL")
                            Select Case dataitem.Attribute("val").Value
                                Case "ACCOUNTID"
                                    .AccountID = dataitem.Value
                                Case "Account Name"
                                    .AccountName = dataitem.Value
                                Case "Shipping City"
                                    .City = dataitem.Value
                                Case "Shipping Country"
                                    .Country = dataitem.Value
                                Case "Shipping State"
                                    .State = dataitem.Value
                                Case "Shipping Street"
                                    .Street = dataitem.Value
                                Case "Shipping Code"
                                    .Zip = dataitem.Value
                                Case "Phone"
                                    .Phone = dataitem.Value
                            End Select
                        Next
                    End With

                    CompanySyncData.Add(thisCustomer)
                Next

                Return CompanySyncData
            End Function

            Private Function ParseZohoPeopleXML(ByVal PeopleToSync As XmlDocument) As List(Of EmployeeData)
                Dim EmployeeSyncData As List(Of EmployeeData) = Nothing

                Dim allpeople As XElement = XElement.Load(New XmlNodeReader(PeopleToSync))

                If allpeople.Descendants("row").Count > 0 Then
                    EmployeeSyncData = New List(Of EmployeeData)()
                End If

                For Each person As XElement In allpeople.Descendants("row")
                    If Processor.IsStopped Then
                        Return Nothing
                    End If

                    Dim thisPerson As New EmployeeData()
                    
                    With thisPerson
                        'see above re: xml formatting/processing
                        For Each dataitem As XElement In person.Descendants("FL")
                            Select Case dataitem.Attribute("val").Value
                                Case "First Name"
                                    .FirstName = dataitem.Value
                                Case "Last Name"
                                    .LastName = dataitem.Value
                                Case "Title"
                                    .Title = dataitem.Value
                                Case "Email"
                                    .Email = dataitem.Value
                                Case "Phone"
                                    .Phone = dataitem.Value
                                Case "Mobile"
                                    .Cell = dataitem.Value
                                Case "Fax"
                                    .Fax = dataitem.Value
                            End Select
                        Next

                    End With

                    EmployeeSyncData.Add(thisPerson)
                Next

                Return EmployeeSyncData
            End Function

            Private Function GetZohoCompanyXML(ByVal TagToMatch As String) As XmlDocument
                Return GetZohoXML("Accounts/getSearchRecords?newFormat=1&selectColumns=All&searchCondition=(Account Type|=|" & TagToMatch & ")")
            End Function

            Private Function GetZohoPeopleXML(ByVal AccountID As String) As XmlDocument
                Return GetZohoXML("Contacts/getSearchRecordsByPDC?newFormat=1&searchColumn=accountid&searchValue=" & AccountID)
            End Function

            Private Function GetZohoXML(ByVal PathAndQuery As String) As XmlDocument
                Dim ZohoUri As New Uri("https://crm.zoho.com/crm/private/xml/" & PathAndQuery & "&apikey=" & CRMLinkRow.SecurityToken & "&ticket=" & APITicket)

                Return GetXML(Nothing, ZohoUri)
            End Function

            Private Function GenerateTicket() As Boolean
                Log.Write("Generating new ticket...")
                Dim ZohoUri As New Uri(String.Format("https://accounts.zoho.com/login?servicename=ZohoCRM&FROM_AGENT=true&LOGIN_ID={0}&PASSWORD={1}", _
                                                     HttpUtility.UrlEncode(CRMLinkRow.Username), HttpUtility.UrlEncode(CRMLinkRow.Password)))

                Dim TicketResult As String = GetHTTPData(Nothing, ZohoUri)
                
                If TicketResult Is Nothing Then
                    Return False
                ElseIf TicketResult.ToLower().Contains("result=false") Then
                    ErrorCode = IntegrationError.InvalidLogin
                    Return False
                Else
                    'parse out the result to get the ticket
                    TicketResult = TicketResult.ToLower()
                    APITicket = TicketResult.Substring(TicketResult.IndexOf("ticket=") + Len("ticket="), 32)
                End If

                Return True
            End Function

            Private Sub Logout()
                Log.Write("Logging out.")
                Dim ZohoUri As New Uri("https://accounts.zoho.com/logout?FROM_AGENT=true&ticket=" & APITicket)

                GetHTTPData(Nothing, ZohoUri)
            End Sub

            Private Function CreateNote(ByVal accountid As String, ByVal thisTicket As Ticket) As Boolean
                Dim description As Action = Actions.GetTicketDescription(User, thisTicket.TicketID)
                Dim NoteBody As String = String.Format("A ticket has been created for this organization entitled ""{0}"".{3}{2}{3}Click here to access the ticket information: https://app.teamsupport.com/Ticket.aspx?ticketid={1}", _
                                                                             thisTicket.Name, thisTicket.TicketID.ToString(), HtmlUtility.StripHTML(description.Description), Environment.NewLine)

                Dim ZohoUri As New Uri("https://crm.zoho.com/crm/private/xml/Notes/insertRecords?newFormat=1&apikey=" & CRMLinkRow.SecurityToken & "&ticket=" & APITicket)

                Dim postData As String = String.Format("<Notes><row no=""1"">" & _
                                        "<FL val=""entityId"">{0}</FL>" & _
                                        "<FL val=""Note Title"">Support Issue: {1}</FL>" & _
                                         "<FL val=""Note Content""><![CDATA[{2}]]></FL></row></Notes>", _
                                         accountid, thisTicket.Name, NoteBody)

                postData = "&xmlData=" & HttpUtility.UrlEncode(postData)

                Return PostQueryString(Nothing, ZohoUri, postData) = HttpStatusCode.OK
            End Function
        End Class
    End Namespace
End Namespace