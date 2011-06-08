' 7/14/09 - Adding address information
' 8/20/09 - Fixed two issues with the SendTicketData query (1284/1285)
' 11/18/09 - Users no longer have to have an e-mail
' 1/29/10- Modification to fix requirement for SSL on Highrise
' 5/7/10 - Org phone numbers should be working now.
' 5/20/10 - Bug fix for occasional syncing problem with phone numbers
' 10/5/10 - Should be getting phone numbers from contacts now 

Imports System.Net
Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization
Imports System.Text
Imports TeamSupport.Data

Namespace TeamSupport
    Namespace CrmIntegration
        Public Class Highrise
            Inherits Integration

            Dim SyncError As Boolean = False 'tracks global errors so we can not update the sync date if there's a problem
            Dim UseSSL As Boolean = True

            Public Sub New(ByVal crmLinkOrg As CRMLinkTableItem, ByVal crmLog As SyncLog, ByVal thisUser As LoginUser)
                MyBase.New(crmLinkOrg, crmLog, thisUser, IntegrationType.Highrise)
            End Sub

            Public Overrides Function PerformSync() As Boolean
                Dim MyXML As XmlDocument
                Dim Key As String = CRMLinkRow.SecurityToken
                Dim CompanyName As String = CRMLinkRow.Username
                Dim ParentOrgID As String = CRMLinkRow.OrganizationID
                Dim TagsToMatch As String = CRMLinkRow.TypeFieldMatch

                Dim CountAccounts As Integer = 0
                Dim AccountName() As String
                Dim AccountID() As String
                Dim LastUpdated() As Date

                SyncError = False
                UseSSL = True

                Dim City(20, 1), Country(20, 1), State(20, 1), Street(20, 1), Zip(20, 1), Phone(20, 1) As String 'rightmost is the company
                'Note:  Redim can only adjust the right side, so we're making that the account # since it could be large.  The left side is only
                ' for multiple addresses, and it's unlikely we will have more than 10 addresses per company.

                'Interesting - Found one company with 15 addresses per company!  Changed left side from 10 to 20.  12/11/09

                Dim LastUpdate As DateTime

                Try
                    Dim TagIDs As New List(Of String)()

                    If TagsToMatch.Contains(",") Then
                        'process multiple tags where present
                        For Each TagToMatch As String In TagsToMatch.Split(",")
                            Dim TagID As String = GetTagID(Key, CompanyName, TagToMatch, ParentOrgID)

                            If TagID IsNot Nothing Then
                                TagIDs.Add(TagID)
                            End If
                        Next
                    Else
                        Dim TagID As String = GetTagID(Key, CompanyName, TagsToMatch, ParentOrgID)

                        If TagID IsNot Nothing Then
                            TagIDs.Add(TagID)
                        End If
                    End If

                    If TagIDs.Count > 0 Then
                        For Each TagID As String In TagIDs

                            Dim AddressCount() As Integer
                            Dim WorkPhone(), FaxPhone() As String
                            Dim LastLocation As String

                            'Dim Phone(3000) As Integer

                            ReDim AccountName(0 To 1) 'just set it up

                            Dim XMLReader As XmlNodeReader

                            Dim XMLLoopCount As Integer = 0 'counts the number of records that we got in this loop
                            Dim NeedToGetMore As Boolean = True

                            Log.Write("Starting GetXML")
                            MyXML = GetXML(Key, CompanyName, "companies.xml?tag_id=" + TagID, ParentOrgID)  'note that capitalization DOES matter
                            Log.Write("Finished GetXML")


                            If MyXML IsNot Nothing Then

                                While NeedToGetMore
                                    XMLReader = New XmlNodeReader(MyXML)

                                    XMLReader.MoveToContent()

                                    Log.Write("Processing company information...")
                                    While XMLReader.Read
                                        If (XMLReader.NodeType = XmlNodeType.Element) Then
                                            If (XMLReader.Name = "id") And (XMLReader.Depth = 2) Then 'the main ID is on depth 2 - This prevents us from updating this on an Address or e-mail id
                                                CountAccounts = CountAccounts + 1
                                                XMLLoopCount = XMLLoopCount + 1 'tracks the number we've gotten in this loop

                                                ReDim Preserve AddressCount(0 To CountAccounts)
                                                AddressCount(CountAccounts) = 0

                                                ReDim Preserve AccountID(0 To CountAccounts)
                                                AccountID(CountAccounts) = XMLReader.ReadString

                                            ElseIf (XMLReader.Name = "name") And (XMLReader.Depth = 2) Then 'company name '1/7/11 - Looks like HR added NAME to the tags section - We need to add depth (2) here
                                                ReDim Preserve AccountName(0 To CountAccounts)
                                                AccountName(CountAccounts) = XMLReader.ReadString
                                            ElseIf (XMLReader.Name = "address") And (XMLReader.Depth = 4) Then ' I think this is righ
                                                ReDim Preserve AddressCount(0 To CountAccounts)
                                                AddressCount(CountAccounts) = AddressCount(CountAccounts) + 1

                                            ElseIf XMLReader.Name = "city" Then
                                                ReDim Preserve City(20, CountAccounts)
                                                City(AddressCount(CountAccounts), CountAccounts) = XMLReader.ReadString
                                            ElseIf XMLReader.Name = "country" Then
                                                ReDim Preserve Country(20, CountAccounts)
                                                Country(AddressCount(CountAccounts), CountAccounts) = XMLReader.ReadString
                                                'ElseIf XMLreader.Name = "location" Then 'hmm - There are multiple locations.
                                                'Location(CountAccounts, AddressCount) = XMLreader.ReadString
                                            ElseIf XMLReader.Name = "state" Then
                                                ReDim Preserve State(20, CountAccounts)
                                                State(AddressCount(CountAccounts), CountAccounts) = XMLReader.ReadString
                                            ElseIf XMLReader.Name = "street" Then
                                                ReDim Preserve Street(20, CountAccounts)
                                                Street(AddressCount(CountAccounts), CountAccounts) = XMLReader.ReadString
                                            ElseIf XMLReader.Name = "zip" Then
                                                ReDim Preserve Zip(20, CountAccounts)
                                                Zip(AddressCount(CountAccounts), CountAccounts) = XMLReader.ReadString
                                            ElseIf XMLReader.Name = "location" Then
                                                LastLocation = XMLReader.ReadString 'this will store the last location field.  Used to identify the phone number

                                            ElseIf XMLReader.Name = "number" Then
                                                'we've found a number, hopefully a phone number.  Based on the last value of the Location field, we will now assign it to the correct phone type
                                                If LastLocation = "Work" Then
                                                    ReDim Preserve WorkPhone(CountAccounts)
                                                    If WorkPhone(CountAccounts) = "" Then
                                                        'we only want the first work number - Don't replace if we already have one there.
                                                        WorkPhone(CountAccounts) = XMLReader.ReadString
                                                    End If


                                                ElseIf LastLocation = "Fax" Then
                                                    ReDim Preserve FaxPhone(CountAccounts)
                                                    FaxPhone(CountAccounts) = XMLReader.ReadString
                                                End If


                                            ElseIf XMLReader.Name = "updated-at" Then
                                                ReDim Preserve LastUpdated(0 To CountAccounts)
                                                LastUpdated(CountAccounts) = Date.Parse(XMLReader.ReadString) 'gets date from XML  Should (?) remain in Zulu time

                                            End If
                                        End If

                                    End While

                                    'Need to do a redmin here just to make sure we create the entire array.
                                    '  If there was not a phone and/or fax number on the final account, then this would not get updated and we 
                                    '  would have an error later on.
                                    ReDim Preserve WorkPhone(CountAccounts)
                                    ReDim Preserve FaxPhone(CountAccounts)

                                    XMLReader.Close()


                                    If XMLLoopCount = 500 Then

                                        'get the next 500 records
                                        Log.Write("Getting next set of records..." + CountAccounts.ToString)
                                        MyXML = GetXML(Key, CompanyName, "companies.xml?tag_id=" + TagID + "&n=" + CountAccounts.ToString, ParentOrgID)  'note that capitalization DOES matter

                                        If MyXML IsNot Nothing Then


                                            XMLReader = New XmlNodeReader(MyXML)

                                            XMLReader.MoveToContent()

                                            XMLLoopCount = 0
                                            NeedToGetMore = True
                                        End If
                                    Else
                                        NeedToGetMore = False 'don't get any more!

                                    End If

                                End While



                                Log.Write("Processed " + CountAccounts.ToString + " accounts.")


                                'LastUpdate = TS.ConvertDateTimeToHR(TS.GetLastCRMUpdate(ParentOrgID)) 'returns last update time formatted correctly for HR
                                LastUpdate = Date.Parse(CRMLinkRow.LastLink).ToLocalTime() 'returns the last update, in local time


                                Log.Write("Updating account information...")
                                For accounts As Integer = 1 To CountAccounts

                                    'Go through all accounts we just processed and add to the TS database

                                    If AddressCount(accounts) > 0 Then

                                        'TS.PublicAddText("Adding/Updating Account Information (y) (" + accounts.ToString + ")", FormName)
                                        'Grab the first address as the one we use in TS
                                        If LastUpdated(accounts).AddMinutes(30) > LastUpdate Then
                                            'Only update if the data is new
                                            'TODO:   TS.AddOrUpdateAccountInformation(AccountID(accounts), AccountName(accounts), Street(1, accounts), "", City(1, accounts), State(1, accounts), Zip(1, accounts), Country(1, accounts), WorkPhone(accounts), ParentOrgID)
                                            Log.Write("Updated w/ Address:  " + AccountName(accounts))
                                        End If

                                    Else
                                        'TS.PublicAddText("Adding/Updating Account Information (n) (" + accounts.ToString + ")", FormName)
                                        If LastUpdated(accounts).AddMinutes(30) > LastUpdate Then
                                            'Only update if the data is new
                                            '     TODO:         TS.AddOrUpdateAccountInformation(AccountID(accounts), AccountName(accounts), "", "", "", "", "", "", "", ParentOrgID)
                                            Log.Write("Updated w/out Address:  " + AccountName(accounts))
                                        End If

                                    End If

                                Next


                                Log.Write("Finished updating account information.")
                                Log.Write("Updating people information...")

                                For x As Integer = 1 To CountAccounts
                                    Try
                                        'Now get the individuals from the company
                                        'TS.PublicAddText("Getting People, CountAccounts=" + x.ToString, FormName)
                                        If LastUpdated(x).AddMinutes(30) > LastUpdate Then
                                            'Only update if the data is new
                                            GetPeople(Key, CompanyName, AccountID(x), ParentOrgID)
                                            Log.Write("Updated people information for " + AccountID(x).ToString())
                                        End If

                                    Catch ex As Exception
                                        'SyncError = True
                                        Log.Write("Error in Updating People loop:" + ex.Message)
                                    End Try
                                Next
                                Log.Write("Finished updating people information")

                            End If

                        Next
                    Else 'no tags so we can't query any customers
                        Return True
                    End If

                    'Clear out the dynamic arrays we've been using
                    Erase AccountName, AccountID, LastUpdated, City, Country, State, Street, Zip

                Catch ex As Exception
                    SyncError = True

                    Log.Write("Error in Perform Highrise Sync: " + ex.Message)

                End Try

                Return Not SyncError

            End Function

            Public Overrides Function SendTicketData() As Boolean
                '8/8/09 - CHanged this so that we only make one call to our database then update tickets based on that one call.  
                Try

                    If CRMLinkRow.SendBackTicketData Then
                        'get tickets created after the last link date
                        Dim tickets As New Tickets(User)
                        tickets.LoadByCRMLinkItem(CRMLinkRow)

                        If tickets IsNot Nothing Then
                            For Each thisTicket As Ticket In tickets
                                'highrise strips out html but not comments from notes
                                Dim description As Action = Actions.GetTicketDescription(User, thisTicket.TicketID)
                                Dim customers As New OrganizationsView(User)
                                customers.LoadByTicketID(thisTicket.TicketID)

                                'Add the new tickets to the company record
                                Dim NoteBody As String = String.Format("A ticket has been created for this organization entitled ""{0}"".{3}{2}{3}Click here to access the ticket information: https://app.teamsupport.com/Ticket.aspx?ticketid={1}", _
                                                                       thisTicket.Name, thisTicket.TicketID, Utilities.StripComments(description.Description), Environment.NewLine)

                                For Each customer As OrganizationsViewItem In customers
                                    If customer.CRMLinkID <> "" Then
                                        Log.Write("Creating a note...")
                                        CreateNote(CRMLinkRow.SecurityToken, CRMLinkRow.Username, customer.CRMLinkID, NoteBody)
                                        Log.Write("Note created successfully.")
                                    End If
                                Next
                            Next
                        End If

                    Else
                        Log.Write("Ticket data not sent since SendBackTicketData is set to FALSE for this organization.")
                    End If
                Catch ex As Exception
                    Log.Write("Error in Send Ticket Data.  Message=" + ex.Message)
                End Try

                Return True
            End Function


            Public Function GetXML(ByVal Token As String, ByVal CompanyName As String, ByVal URL As String, ByVal ParentOrgID As String) As XmlDocument
                Dim request As HttpWebRequest
                Dim response As HttpWebResponse = Nothing
                Dim reader As StreamReader

                Dim WebString As String

                If UseSSL Then
                    WebString = "https://" + CompanyName + ".highrisehq.com/" + URL
                Else
                    'No s.
                    WebString = "http://" + CompanyName + ".highrisehq.com/" + URL
                End If


                If CompanyName <> "" Then
                    Try


                        System.Threading.Thread.Sleep(100) '1 ms sleep to see if there are issues with the API and throttling

                        ' Create the web request  
                        request = DirectCast(WebRequest.Create(WebString), HttpWebRequest)
                        ' Add authentication to request  
                        request.Credentials = New NetworkCredential(Token, "x")
                        request.Timeout = 7000 'was 5, changed to 7 11/18/09
                        request.Method = "GET" 'not sure on this one?
                        'request.Proxy = GlobalProxySelection.GetEmptyWebProxy
                        request.KeepAlive = False

                        request.UserAgent = "Muroc Client"


                        ' Get response  
                        response = DirectCast(request.GetResponse(), HttpWebResponse)

                        If request.HaveResponse = True AndAlso Not (response Is Nothing) Then


                            ' Get the response stream into a reader  
                            reader = New StreamReader(response.GetResponseStream())

                            'Load the stream into an xml doc
                            Dim myxml As XmlDocument = New XmlDocument
                            myxml.LoadXml(reader.ReadToEnd)

                            reader.Close()

                            Return myxml
                        End If

                        response.Close()


                    Catch wex As WebException

                        If UseSSL Then
                            '' If UseSSL is true then it means this is probably the first time we've called this.  Set to false then
                            ''  have the calling routine try again.
                            UseSSL = False

                        Else
                            'Don't need to raise the rror flag the first time through.

                            Log.Write("Error in GetXML: " + wex.ToString)
                            
                            SyncError = True

                        End If

                        Return Nothing
                    End Try
                Else
                    'Probably should throw exception here
                    Return Nothing
                End If


            End Function

            Public Sub CreateNote(ByVal SecurityToken As String, ByVal CompanyName As String, ByVal AccountID As String, ByVal NoteBody As String)
                Try


                    Dim urlPost As String

                    If UseSSL Then
                        urlPost = "https://" + CompanyName + ".highrisehq.com/companies/" + AccountID + "/notes.xml"
                    Else
                        urlPost = "http://" + CompanyName + ".highrisehq.com/companies/" + AccountID + "/notes.xml"
                    End If

                    'Dim postData As String = "<note><body>Hello world</body><subject-id type=""integer"">4</subject-id><subject-type>Party</subject-type></note>"
                    Dim postData As String = "<note><body><![CDATA[" + NoteBody + "]]></body></note>"
                    Dim encoding As New UTF8Encoding()

                    Dim PostStream As Stream = Nothing

                    'Dim Response As HttpWebResponse = Nothing

                    Dim Reader As StreamReader


                    Dim Request As HttpWebRequest



                    'Dim byteData As Byte() = encoding.GetBytes(postData)
                    Dim ByteData = UTF8Encoding.UTF8.GetBytes(postData)


                    'System.Threading.Thread.Sleep(1000) '1 second sleep to see if there are issues with the API and throttling


                    'Dim request As HttpWebRequest = DirectCast(WebRequest.Create(urlPost), HttpWebRequest)

                    'request = DirectCast(WebRequest.Create(urlPost), HttpWebRequest)

                    Request = WebRequest.Create(urlPost)

                    Request.Method = "POST"

                    Request.ContentType = "application/xml"

                    Request.ContentLength = ByteData.Length

                    Request.PreAuthenticate = True

                    'Dim cred As NetworkCredential = New NetworkCredential(SecurityToken, "x")

                    Request.Credentials = New NetworkCredential(SecurityToken, "xx") ' cred.GetCredential(Request.RequestUri, "Basic") '




                    'OK, let's write the data
                    Try
                        PostStream = Request.GetRequestStream
                        PostStream.Write(ByteData, 0, ByteData.Length)
                    Catch ex As Exception
                        If Not PostStream Is Nothing Then PostStream.Close()

                        'Removing sync error from creating notes
                        'SyncError = True

                        Log.Write("Create Note: " + ex.Message.ToString)

                    End Try

                    Try

                        Log.Write("Create Note URI: " + urlPost)
                        Log.Write("Create Note Post Data: " + postData)

                        'Dim response As HttpWebResponse
                        'Using response As HttpWebResponse = Request.GetResponse
                        'End Using
                        Dim response As HttpWebResponse
                        response = Request.GetResponse



                        'Response = DirectCast(request.GetResponse(), HttpWebRequest)

                        'Dim response2 As HttpWebResponse = request.GetResponse 'I think this forces the message to be sent
                        Reader = New StreamReader(response.GetResponseStream())


                        'ts.PublicAddText(response.StatusCode.ToString, formname)
                        response.Close()

                    Catch wex As WebException
                        'removing Sync error from creating notes.
                        'SyncError = True

                        Log.Write("Error in create Note 2: " + wex.Message.ToString)
                    End Try

                    'Response.Close()
                    PostStream.Close()

                Catch ex As Exception
                    Log.Write("Error in Create Note (Main routine).  Error = " + ex.Message)
                End Try

            End Sub


            Public Function GetTagID(ByVal token As String, ByVal companyname As String, ByVal TagString As String, ByVal parentorgid As String) As String
                'We use Tags in Highrise to identify the companies we want synced from HR to TS.
                '  This tag shows up as a string in HR, and we need to figure out what the ID of that tag is.
                '  This routine takes a text tag name and returns the (integer) id.


                Dim MyXML As XmlDocument

                Dim TagID As String
                Dim TagName As String


                MyXML = GetXML(token, companyname, "tags.xml", parentorgid)  'note that capitalization DOES matter


                If MyXML IsNot Nothing Then

                    Dim XMLreader As XmlNodeReader = New XmlNodeReader(MyXML)

                    Dim FoundTag As Boolean = False

                    While XMLreader.Read And (Not FoundTag)
                        If (XMLreader.NodeType = XmlNodeType.Element) Then
                            If XMLreader.Name = "id" Then
                                TagID = XMLreader.ReadString
                            ElseIf XMLreader.Name = "name" Then
                                TagName = XMLreader.ReadString

                                If TagName = TagString Then
                                    FoundTag = True 'exit if we've found the tag
                                End If

                            End If
                        End If


                    End While


                    If FoundTag Then
                        Return TagID
                    Else
                        Return Nothing
                    End If


                Else
                    Return Nothing
                End If

            End Function

            Public Sub GetPeople(ByVal token As String, ByVal CompanyName As String, ByVal AccountID As String, ByVal ParentOrgID As String)

                Dim MyXML As XmlDocument


                Dim CountPeople As Integer = 0

                Dim FirstName() As String
                Dim LastName() As String
                Dim email() As String 'hmmm - Need to explore this
                Dim Title() As String
                Dim WorkPhone() As String
                Dim MobilePhone() As String
                Dim FaxPhone() As String

                Dim ContactEMail() As String

                Dim TestAddress As String

                Dim FoundEMail As Boolean = False

                Dim LastLocation As String = "nothing"



                MyXML = GetXML(token, CompanyName, "companies/" + AccountID + "/people.xml", ParentOrgID)  'List of people in this company

                If MyXML IsNot Nothing Then

                    Dim XMLreader As XmlNodeReader = New XmlNodeReader(MyXML)

                    While XMLreader.Read
                        If (XMLreader.NodeType = XmlNodeType.Element) Then
                            If XMLreader.Name = "person" Then
                                CountPeople = CountPeople + 1

                                FoundEMail = False 'reset found e-mail counter

                                'AccountID(CountAccounts) = XMLreader.ReadString
                            ElseIf XMLreader.Name = "first-name" Then
                                ReDim Preserve FirstName(CountPeople)
                                FirstName(CountPeople) = XMLreader.ReadString

                            ElseIf XMLreader.Name = "last-name" Then
                                ReDim Preserve LastName(CountPeople)
                                LastName(CountPeople) = XMLreader.ReadString
                            ElseIf XMLreader.Name = "title" Then
                                ReDim Preserve Title(CountPeople)
                                Title(CountPeople) = XMLreader.ReadString
                            ElseIf XMLreader.Name = "location" Then
                                LastLocation = XMLreader.ReadString 'this will store the last location field.  Used to identify the phone number

                            ElseIf XMLreader.Name = "number" Then
                                'we've found a number, hopefully a phone number.  Based on the last value of the Location field, we will now assign it to the correct phone type
                                If LastLocation = "Work" Then
                                    ReDim Preserve WorkPhone(CountPeople)
                                    If WorkPhone(CountPeople) = "" Then
                                        'we only want the first work number - Don't replace if we already have one there.
                                        WorkPhone(CountPeople) = XMLreader.ReadString
                                    End If


                                ElseIf LastLocation = "Fax" Then
                                    ReDim Preserve FaxPhone(CountPeople)
                                    FaxPhone(CountPeople) = XMLreader.ReadString


                                ElseIf LastLocation = "Mobile" Then
                                    ReDim Preserve MobilePhone(CountPeople)
                                    MobilePhone(CountPeople) = XMLreader.ReadString
                                End If



                            ElseIf XMLreader.Name = "address" Then
                                'do something
                                'Problem is that there are 2 address attributes, and both are at the same depth
                                'How about we search for an @??

                                'Found e-mail should make sure we only grab the first e-mail (hopefully work)

                                If Not FoundEMail Then
                                    TestAddress = XMLreader.ReadString
                                    If TestAddress.Contains("@") Then
                                        ReDim Preserve email(CountPeople)
                                        email(CountPeople) = TestAddress
                                        FoundEMail = True


                                    End If
                                End If

                            End If

                        End If

                    End While

                    XMLreader.Close()

                    Dim TempEMail, TempFirstName, TempLastName, TempWorkPhone, TempFaxPhone, TempMobilePhone, TempTitle As String


                    For x As Integer = 1 To CountPeople

                        'Since we are using dynamic arrays, it's possible the array values may not exist.  If this is the case, we will get an exception and set the temp value to nothing



                        Try
                            TempFirstName = FirstName(x)
                        Catch ex As Exception
                            TempFirstName = "None"
                        End Try

                        Try
                            TempLastName = LastName(x)
                        Catch ex As Exception
                            TempLastName = "None"
                        End Try


                        Try
                            TempEMail = email(x)
                        Catch ex As Exception
                            Try
                                TempEMail = LastName(x) + "." + FirstName(x) 'added 11/18/09
                            Catch ex2 As Exception
                                TempEMail = ""
                            End Try

                        End Try

                        Try
                            TempWorkPhone = WorkPhone(x)
                        Catch ex As Exception
                            TempWorkPhone = "None"
                        End Try


                        Try
                            TempFaxPhone = FaxPhone(x)
                        Catch ex As Exception
                            TempFaxPhone = "None"
                        End Try


                        Try
                            TempMobilePhone = MobilePhone(x)
                        Catch ex As Exception
                            TempMobilePhone = "None"
                        End Try


                        Try
                            TempTitle = Title(x)
                        Catch ex As Exception
                            TempTitle = "None"
                        End Try

                        'TODO:   TS.AddOrUpdateContactInformation(AccountID, TempEMail, TempFirstName, TempLastName, TempWorkPhone, TempFaxPhone, TempMobilePhone, TempTitle, 0, ParentOrgID)
                    Next
                End If


                Erase FirstName, LastName, email, Title, WorkPhone, MobilePhone, FaxPhone, ContactEMail


            End Sub

        End Class
    End Namespace
End Namespace