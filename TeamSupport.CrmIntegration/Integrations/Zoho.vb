Imports TeamSupport.Data
Imports System.Net
Imports System.Web
Imports System.Xml
Imports System.Text
Imports System.IO

Namespace TeamSupport
    Namespace CrmIntegration

        Public MustInherit Class Zoho
            Inherits Integration

            Protected APITicket As String

            Protected Sub New(ByVal crmLinkOrg As CRMLinkTableItem, ByVal crmLog As SyncLog, ByVal thisUser As LoginUser, ByVal thisProcessor As CrmProcessor, ByVal thisType As IntegrationType)
                MyBase.New(crmLinkOrg, crmLog, thisUser, thisProcessor, thisType)
            End Sub

            Public MustOverride Overrides Function PerformSync() As Boolean

            Protected Function GenerateTicket() As Boolean
                Log.Write("Generating new ticket...")
                Dim ZohoUri As New Uri(String.Format("https://accounts.zoho.com/login?servicename={2}&FROM_AGENT=true&LOGIN_ID={0}&PASSWORD={1}", _
                                                     HttpUtility.UrlEncode(CRMLinkRow.Username), HttpUtility.UrlEncode(CRMLinkRow.Password), Type.ToString()))

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

            Protected Sub Logout()
                Log.Write("Logging out.")
                Dim ZohoUri As New Uri("https://accounts.zoho.com/logout?FROM_AGENT=true&ticket=" & APITicket)

                GetHTTPData(Nothing, ZohoUri)
            End Sub
        End Class


        Public Class ZohoCRM
            Inherits Zoho

            Public Sub New(ByVal crmLinkOrg As CRMLinkTableItem, ByVal crmLog As SyncLog, ByVal thisUser As LoginUser, ByVal thisProcessor As CrmProcessor)
                MyBase.New(crmLinkOrg, crmLog, thisUser, thisProcessor, IntegrationType.ZohoCRM)
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

                        If Success Then 'send ticket data
                            Success = SendTicketData(AddressOf CreateNote)
                        End If
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

            Private Function GetZohoCRMXML(ByVal PathAndQuery As String) As XmlDocument
                Dim ZohoUri As New Uri("https://crm.zoho.com/crm/private/xml/" & PathAndQuery & "&apikey=" & CRMLinkRow.SecurityToken & "&ticket=" & APITicket)

                Return GetXML(ZohoUri)
            End Function

            Private Function ParseZohoCompanyXML(ByVal CompaniesToSync As XmlDocument) As List(Of CompanyData)
                Dim CompanySyncData As List(Of CompanyData) = Nothing
                Dim tagsToMatch As String() = Array.ConvertAll(CRMLinkRow.TypeFieldMatch.ToLower().Split(","), Function(p As String) p.Trim())
                Dim allaccounts As XElement = XElement.Load(New XmlNodeReader(CompaniesToSync))

                If allaccounts.Descendants("row").Count > 0 Then
                    CompanySyncData = New List(Of CompanyData)()
                End If

                For Each company As XElement In allaccounts.Descendants("row")
                    If Processor.IsStopped Then
                        Return Nothing
                    End If

                    Dim thisCustomer As New CompanyData()
                    Dim add As Boolean = False

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
                                Case "Account Type"
                                    add = tagsToMatch.Contains(dataitem.Value.ToLower())
                            End Select
                        Next
                    End With

                    If add Then
                        CompanySyncData.Add(thisCustomer)
                    End If
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

            Private Function GetZohoCompanyXML() As XmlDocument
                Dim ZohoPath As String = "Accounts/getRecords?newFormat=1"

                If CRMLinkRow.LastLink IsNot Nothing Then
                    ZohoPath &= "&lastModifiedTime=" & CRMLinkRow.LastLink.Value.AddMinutes(-30).ToString("s").Replace("T", " ")
                End If

                Log.Write("querying " & ZohoPath)
                Return GetZohoCRMXML(ZohoPath)
            End Function

            Private Function GetZohoPeopleXML(ByVal AccountID As String) As XmlDocument
                Return GetZohoCRMXML("Contacts/getSearchRecordsByPDC?newFormat=1&searchColumn=accountid&searchValue=" & AccountID)
            End Function

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


        Public Class ZohoReports
            Inherits Zoho

            Private databaseName As String = "TeamSupport"

            Public Sub New(ByVal crmLinkOrg As CRMLinkTableItem, ByVal crmLog As SyncLog, ByVal thisUser As LoginUser, ByVal thisProcessor As CrmProcessor)
                MyBase.New(crmLinkOrg, crmLog, thisUser, thisProcessor, IntegrationType.ZohoReports)
            End Sub

            Public Overrides Function PerformSync() As Boolean
                Dim Success As Boolean = False

                If GenerateTicket() Then

                    Try
                        Success = SendReportData()

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

            Private Function SendReportData() As Boolean
                Dim success As Boolean = True

                'load data from ticketsview into a csv
                Dim tix As New TicketsView(User)
                tix.LoadByOrganizationID(CRMLinkRow.OrganizationID)
                Dim ticketsViewBatchSize As Integer = 50000

                Dim batches As List(Of String) = GetCSVBatches(tix.Table, ticketsViewBatchSize)

                'now that we have data, send it to zoho
                Dim zohoUri As New Uri(String.Format("https://reportsapi.zoho.com/api/{0}/{3}/TicketsView?ZOHO_ACTION=IMPORT&ZOHO_OUTPUT_FORMAT=XML&ZOHO_ERROR_FORMAT=XML&ZOHO_API_KEY={1}&ticket={2}&ZOHO_API_VERSION=1.0", _
                                                     CRMLinkRow.Username, CRMLinkRow.SecurityToken, APITicket, databaseName))

                For Each batch As String In batches
                    Dim byteData As Byte() = UTF8Encoding.UTF8.GetBytes(batch)

                    Dim postParameters As New Dictionary(Of String, Object)()
                    postParameters.Add("ZOHO_IMPORT_TYPE", "UPDATEADD")
                    postParameters.Add("ZOHO_MATCHING_COLUMNS", "TicketID")
                    postParameters.Add("ZOHO_AUTO_IDENTIFY", "false")
                    postParameters.Add("ZOHO_ON_IMPORT_ERROR", "ABORT")
                    postParameters.Add("ZOHO_DELIMITER", "0")
                    postParameters.Add("ZOHO_QUOTED", "2")
                    postParameters.Add("ZOHO_CREATE_TABLE", "true")
                    postParameters.Add("ZOHO_FILE", byteData)

                    Try
                        Using response As HttpWebResponse = WebHelpers.MultipartFormDataPost(zohoUri, Client, postParameters)

                            If response.StatusCode <> HttpStatusCode.OK Then
                                Log.Write("Error posting query string: " & response.StatusDescription)
                            Else
                                Log.Write(response.StatusDescription)
                            End If

                        End Using
                    Catch ex As WebException
                        Log.Write("Error contacting " & zohoUri.ToString() & ": " & ex.Message)
                        Log.Write(New StreamReader(ex.Response.GetResponseStream()).ReadToEnd())

                        success = False
                    End Try

                    'we have to delete the dummy row we insert with each import
                    Log.Write("Deleting dummy row...")
                    If DeleteZohoTableRow("TicketsView", "-1") Then
                        Log.Write("row -1 deleted successfully")
                    Else
                        Log.Write("Error deleting dummy row.")
                        success = False
                    End If
                Next

                Return success
            End Function

            Private Function GetCSVBatches(ByVal thisTable As DataTable, ByVal batchSize As Integer) As List(Of String)
                Dim csvBatches As New List(Of String)()
                Dim csvContent As StringBuilder = Nothing
                Dim csvHeader As New StringBuilder()
                Dim rowIndex As Integer = 0

                'column names in first row
                For i As Integer = 0 To thisTable.Columns.Count - 1
                    csvHeader.Append(thisTable.Columns(i).ColumnName)
                    csvHeader.Append(If(i < thisTable.Columns.Count - 1, ",", Environment.NewLine))
                Next

                'append a dummy row so zoho knows what data type each column is--in case this is the first import
                For Each thisCol As DataColumn In thisTable.Columns
                    If thisCol.DataType Is GetType(String) Then
                        csvHeader.Append("""String""")
                    ElseIf thisCol.DataType Is GetType(DateTime) Then
                        csvHeader.Append("""" & DateTime.MinValue.ToString() & """")
                    ElseIf thisCol.DataType Is GetType(Boolean) Then
                        csvHeader.Append("true")
                    ElseIf thisCol.DataType Is GetType(Decimal) Then
                        csvHeader.Append("0.0")
                    Else
                        csvHeader.Append("-1")
                    End If
                    csvHeader.Append(If(thisCol.Ordinal < thisTable.Columns.Count - 1, ",", Environment.NewLine))
                Next

                For Each thisRow As DataRow In thisTable.Rows
                    If CRMLinkRow.LastLink IsNot Nothing AndAlso thisRow("DateModified") > CRMLinkRow.LastLink.Value.AddMinutes(-30) Then

                        If csvContent Is Nothing Then
                            csvContent = New StringBuilder(csvHeader.ToString())
                        End If

                        For i As Integer = 0 To thisTable.Columns.Count - 1
                            'TODO: check isdbnull(thisrow(i))?
                            If thisTable.Columns(i).DataType Is GetType(String) Then
                                csvContent.Append("""" & thisRow(i).ToString() & """")
                            Else
                                csvContent.Append(thisRow(i).ToString())
                            End If
                            csvContent.Append(If(i < thisTable.Columns.Count - 1, ",", Environment.NewLine))
                        Next
                        rowIndex += 1

                        If rowIndex = batchSize Then
                            csvBatches.Add(csvContent.ToString())

                            rowIndex = 0
                            csvContent = Nothing
                        End If

                    End If
                Next

                If csvContent IsNot Nothing Then
                    csvBatches.Add(csvContent.ToString())
                End If

                Return csvBatches
            End Function

            'only works for ticketsview right now
            Private Function DeleteZohoTableRow(ByVal tableName As String, ByVal rowKey As String) As Boolean
                Dim DeletePath As String = String.Format("{0}/{1}/{2}?ZOHO_ACTION=DELETE&ZOHO_OUTPUT_FORMAT=XML&ZOHO_ERROR_FORMAT=XML&ZOHO_API_KEY={3}&ZOHO_API_VERSION=1.0", CRMLinkRow.Username, databaseName, tableName, CRMLinkRow.SecurityToken)
                Dim DeleteParameters As String = "&ZOHO_CRITERIA=(""TicketID"" = " & rowKey & ")"

                Return PostZohoReports(DeletePath, DeleteParameters) = HttpStatusCode.OK
            End Function

            Private Function PostZohoReports(ByVal PathAndQuery As String, ByVal PostParameters As String) As HttpStatusCode
                Dim ZohoUri As New Uri("https://reportsapi.zoho.com/api/" & PathAndQuery & "&apikey=" & CRMLinkRow.SecurityToken & "&ticket=" & APITicket)

                Return PostQueryString(Nothing, ZohoUri, PostParameters)
            End Function

        End Class
    End Namespace
End Namespace