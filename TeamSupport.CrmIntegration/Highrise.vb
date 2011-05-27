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
Imports Microsoft.Http
Imports System.Text


Public Class Highrise
    Private Const Integration As IntegrationType = IntegrationType.Highrise

    Dim SyncError As Boolean = False 'tracks global errors so we can not update the sync date if there's a problem
    Dim UseSSL As Boolean = True

    Public Function GetXML(ByVal Token As String, ByVal CompanyName As String, ByVal URL As String, ByVal FormName As TextBox, ByVal ParentOrgID As String) As XmlDocument
        Dim request As HttpWebRequest
        Dim response As HttpWebResponse = Nothing
        Dim reader As StreamReader

        Dim ts As New TSCheckData


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

                    ts.PublicAddText("GetXML: " + wex.ToString, FormName)
                    ts.WriteToCRMResults(ParentOrgID, "Error in GetXML: " + wex.ToString)

                    SyncError = True

                End If

                Return Nothing
            End Try
        Else
            'Probably should throw exception here
            Return Nothing
        End If






    End Function

    Public Sub CreateNote(ByVal SecurityToken As String, ByVal CompanyName As String, ByVal AccountID As String, ByVal NoteBody As String, ByVal formname As TextBox, ByVal parentOrgid As String)
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

            Dim ts As New TSCheckData

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

                ts.PublicAddText("Create Note: " + ex.Message.ToString, formname)

            End Try

            Try

                ts.PublicAddText("Create Note URI: " + urlPost, formname)
                ts.PublicAddText("Create Note Post Data: " + postData, formname)

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

                ts.PublicAddText("Create Note 2: " + wex.Message.ToString, formname)
                ts.WriteToCRMResults(parentOrgid, "Error in create note 2: " + wex.ToString)
            End Try

            'Response.Close()
            PostStream.Close()


            'Dim requestStream As Stream = request.GetRequestStream()

            'requestStream.Write(byteData, 0, byteData.Length)

            'requestStream.Close()

            'Dim response As String = request.GetResponse.ToString 'message not sent until this line is called






            'Console.WriteLine(response)
        Catch ex As Exception
            Form1.AddText("Error in Create Note (Main routine).  Error = " + ex.Message)
        End Try

    End Sub

    Public Sub TestPostNote()
        Dim http As New HttpClient("https://murocsystems.highrisehq.com/")

        http.TransportSettings.Credentials = New NetworkCredential("3f4d842b0361f53ce3cb8c009b5a409a87edef8f", "xx")


        'Dim body As HttpContent
        'body = New HttpContent

        'Dim Resp As HttpResponseMessage = http.Post("companies/20502030/notes.xml", "<note><body>Hello world!</body><subject-id type=""integer"">4</subject-id><subject-type>Party</subject-type></note>")

        Dim form As New HttpUrlEncodedForm
        form.Add("body", "<note><body>Hello world!</body><subject-id type=""integer"">4</subject-id><subject-type>Party</subject-type></note>")
        http.Post("companies/20502030/notes.xml", form.CreateHttpContent())



        Dim req As WebRequest
        req = WebRequest.Create("xxx")

        req.Method = "POST"
        req.ContentType = "text/xml"

        Dim writer As StreamWriter = New StreamWriter(req.GetRequestStream)
        writer.WriteLine("xxxx")
        writer.Close()






    End Sub

    Public Sub TestGetCompanies()
        Dim http As New HttpClient("https://murocsystems.highrisehq.com/")

        http.TransportSettings.Credentials = New NetworkCredential("3f4d842b0361f53ce3cb8c009b5a409a87edef8f", "xx")

        System.Net.ServicePointManager.Expect100Continue = False 'no clue what this does

        'Get the companies
        Dim Resp As HttpResponseMessage = http.Get("companies.xml")
        'Resp.EnsureStatusIsSuccessful()


        Dim HRCompanies = Resp.Content.ReadAsXmlSerializable(Of companies)()

        Dim TestString, TestString2 As String

        'For Each HRCompany As companiesCompany In HRCompanies

        TestString = HRCompanies.company.name
        TestString2 = HRCompanies.company.id.ToString





        'TestString = HRCompany.company.name
        'TestString2 = HRCompany.company.id.ToString
        'TestString3 = "ff"



        'Next











    End Sub


    Public Function GetTagID(ByVal token As String, ByVal companyname As String, ByVal TagString As String, ByVal formname As TextBox, ByVal parentorgid As String) As String
        'We use Tags in Highrise to identify the companies we want synced from HR to TS.
        '  This tag shows up as a string in HR, and we need to figure out what the ID of that tag is.
        '  This routine takes a text tag name and returns the (integer) id.


        Dim MyXML As XmlDocument

        Dim TagID As String
        Dim TagName As String


        MyXML = GetXML(token, companyname, "tags.xml", formname, parentorgid)  'note that capitalization DOES matter


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
                Return "-1"
            End If


        Else
            Return Nothing
        End If

    End Function

    Public Sub GetPeople(ByVal token As String, ByVal CompanyName As String, ByVal AccountID As String, ByVal ParentOrgID As String, ByVal formname As TextBox)

        Dim MyXML As XmlDocument

        Dim TS As New TSCheckData


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



        MyXML = GetXML(token, CompanyName, "companies/" + AccountID + "/people.xml", formname, ParentOrgID)  'List of people in this company

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

                TS.AddOrUpdateContactInformation(AccountID, TempEMail, TempFirstName, TempLastName, TempWorkPhone, TempFaxPhone, TempMobilePhone, TempTitle, 0, ParentOrgID)
                Application.DoEvents()
            Next
        End If


        Erase FirstName, LastName, email, Title, WorkPhone, MobilePhone, FaxPhone, ContactEMail


    End Sub

    Public Sub SendTicketData(ByVal SecurityToken As String, ByVal companyname As String, ByVal ParentOrgID As String, ByVal formname As TextBox)
        '8/8/09 - CHanged this so that we only make one call to our database then update tickets based on that one call.  
        Try

            Dim TS As New TSCheckData

            If TSCheckData.SendBackTicketData(ParentOrgID, Integration) Then
                Dim tickets As DataTable

                'get tickets which were created after the last link date
                tickets = TSCheckData.GetLatestTickets(ParentOrgID, Integration)

                If tickets IsNot Nothing Then
                    For Each thisTicket As DataRow In tickets.Rows

                        'highrise strips out html but not comments from notes

                        'Add the new tickets to the company record
                        Dim NoteBody As String = String.Format("A ticket has been created for this organization entitled ""{0}"".{3}{2}{3}Click here to access the ticket information: https://app.teamsupport.com/Ticket.aspx?ticketid={1}", _
                                                               thisTicket(1).ToString, thisTicket(0).ToString, Utilities.StripComments(thisTicket(4).ToString()), Environment.NewLine)

                        TS.PublicAddText("Creating a note...", formname)
                        CreateNote(SecurityToken, companyname, thisTicket(3).ToString, NoteBody, formname, ParentOrgID)

                        Application.DoEvents()
                    Next
                End If

            Else
                Form1.AddText("Ticket data not sent since SendBackTicketData is set to FALSE for this organization.")
            End If
        Catch ex As Exception
            Form1.AddText("Error in Send Ticket Data.  Message=" + ex.Message)
        End Try
      
    End Sub

    Public Function PerformHighriseSync(ByVal token As String, ByVal CompanyName As String, ByVal ParentOrgID As String, ByVal TagToMatch As String, ByVal FormName As TextBox)

        Dim TS As New TSCheckData

        Dim MyXML As XmlDocument

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


            Dim TagID As String
            TagID = GetTagID(token, CompanyName, TagToMatch, FormName, ParentOrgID) 'caps matter here too
            If Not UseSSL Then
                ' UseSSL was true when we called this routine, so if it's false now we know we had an error
                'If we try to get tags the firsst time and get an error, the UseSSL flag will be set to false.
                '  Let's try to get the tag info again, but this time without SSL and see what happens.
                TagID = GetTagID(token, CompanyName, TagToMatch, FormName, ParentOrgID) 'caps matter here too
            End If

            If TagID <> "-1" Then

                Dim AddressCount() As Integer
                Dim WorkPhone(), FaxPhone() As String
                Dim LastLocation As String

                'Dim Phone(3000) As Integer

                ReDim AccountName(0 To 1) 'just set it up

                Dim XMLReader As XmlNodeReader

                Dim XMLLoopCount As Integer = 0 'counts the number of records that we got in this loop
                Dim NeedToGetMore As Boolean = True

                TS.PublicAddText("Starting GetXML", FormName)
                MyXML = GetXML(token, CompanyName, "companies.xml?tag_id=" + TagID, FormName, ParentOrgID)  'note that capitalization DOES matter
                TS.PublicAddText("Finished GetXML", FormName)


                If MyXML IsNot Nothing Then

                    While NeedToGetMore


                        'repeat until not needtogetmore

                        XMLReader = New XmlNodeReader(MyXML)

                        XMLReader.MoveToContent()

                        '**
                        'Dim SingleNode As XmlNode
                        'SingleNode = MyXML.SelectSingleNode("company")
                        'Dim nodevalue As String = SingleNode.InnerText

                        'Dim attrib As XmlAttribute = SingleNode.Attributes("name")
                        'Dim AttribValue As String = attrib.InnerText


                        '**





                        TS.PublicAddText("Processing company information...", FormName)
                        While XMLReader.Read
                            If (XMLReader.NodeType = XmlNodeType.Element) Then
                                If (XMLReader.Name = "id") And (XMLReader.Depth = 2) Then 'the main ID is on depth 2 - This prevents us from updating this on an Address or e-mail id
                                    CountAccounts = CountAccounts + 1
                                    XMLLoopCount = XMLLoopCount + 1 'tracks the number we've gotten in this loop

                                    ReDim Preserve AddressCount(0 To CountAccounts)
                                    AddressCount(CountAccounts) = 0

                                    ReDim Preserve AccountID(0 To CountAccounts)
                                    AccountID(CountAccounts) = XMLReader.ReadString

                                    'TS.PublicAddText("CountAccounts = " + CountAccounts.ToString, FormName)
                                    'ReDim AccountName(0 To CountAccounts)



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

                                    'TS.PublicAddText("Updated at = " + LastUpdated(CountAccounts).ToString + " for company: " + CountAccounts.ToString, FormName)
                                    'TS.PublicAddText("Plus 5 = " + LastUpdated(CountAccounts).AddMinutes(5).ToString + " for company: " + CountAccounts.ToString, FormName)
                                    'TS.PublicAddText("Plus 5(again) = " + LastUpdated(CountAccounts).AddMinutes(5).ToString + " for company: " + CountAccounts.ToString, FormName)

                                    'ElseIf XMLreader.Name = "number" Then
                                    '    If Phone(CountAccounts) = Nothing Then
                                    '**right now we don't do anything with the phone numbers.
                                    'Phone(CountAccounts) = XMLreader.ReadString

                                    'End If

                                End If


                            End If


                            Application.DoEvents()

                        End While

                        'Need to do a redmin here just to make sure we create the entire array.
                        '  If there was not a phone and/or fax number on the final account, then this would not get updated and we 
                        '  would have an error later on.
                        ReDim Preserve WorkPhone(CountAccounts)
                        ReDim Preserve FaxPhone(CountAccounts)

                        XMLReader.Close()


                        If XMLLoopCount = 500 Then

                            'get the next 500 records
                            TS.PublicAddText("Getting next set of records..." + CountAccounts.ToString, FormName)
                            MyXML = GetXML(token, CompanyName, "companies.xml?tag_id=" + TagID + "&n=" + CountAccounts.ToString, FormName, ParentOrgID)  'note that capitalization DOES matter

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



                    TS.PublicAddText("Processed " + CountAccounts.ToString + " accounts.", FormName)

                    'TS.PublicAddText("TEST: " + AccountName(5).ToString + ", Lastupdated: " + LastUpdated(5).ToString, FormName)


                    'Dim WorkAddressID As Integer

                    'LastUpdate = TS.ConvertDateTimeToHR(TS.GetLastCRMUpdate(ParentOrgID)) 'returns last update time formatted correctly for HR
                    LastUpdate = Date.Parse(TSCheckData.GetLastCRMUpdate(ParentOrgID, Integration)).ToLocalTime 'returns the last update, in local time


                    TS.PublicAddText("Updaing account information...", FormName)
                    For accounts As Integer = 1 To CountAccounts

                        'Go through all accounts we just processed and add to the TS database

                        If AddressCount(accounts) > 0 Then

                            'TS.PublicAddText("Adding/Updating Account Information (y) (" + accounts.ToString + ")", FormName)
                            'Grab the first address as the one we use in TS
                            If LastUpdated(accounts).AddMinutes(30) > LastUpdate Then
                                'Only update if the data is new
                                TS.AddOrUpdateAccountInformation(AccountID(accounts), AccountName(accounts), Street(1, accounts), "", City(1, accounts), State(1, accounts), Zip(1, accounts), Country(1, accounts), WorkPhone(accounts), ParentOrgID)
                                TS.PublicAddText("Updated w/ Address:  " + AccountName(accounts), FormName)
                            Else
                                'TS.PublicAddText("No update", FormName)
                            End If

                        Else
                            'TS.PublicAddText("Adding/Updating Account Information (n) (" + accounts.ToString + ")", FormName)
                            If LastUpdated(accounts).AddMinutes(30) > LastUpdate Then
                                'Only update if the data is new
                                TS.AddOrUpdateAccountInformation(AccountID(accounts), AccountName(accounts), "", "", "", "", "", "", "", ParentOrgID)
                                TS.PublicAddText("Updated w/out Address:  " + AccountName(accounts), FormName)
                            Else
                                'TS.PublicAddText("No update", FormName)
                            End If

                        End If


                        Application.DoEvents()
                    Next


                    TS.PublicAddText("Finished updaing account information.", FormName)


                    TS.PublicAddText("Updating people information...", FormName)
                    For x As Integer = 1 To CountAccounts
                        Try


                            'Now get the individuals from the company
                            'TS.PublicAddText("Getting People, CountAccounts=" + x.ToString, FormName)
                            If LastUpdated(x).AddMinutes(30) > LastUpdate Then
                                'Only update if the data is new
                                GetPeople(token, CompanyName, AccountID(x), ParentOrgID, FormName)
                                TS.PublicAddText("Updated people information for " + AccountID(x).ToString, FormName)
                            Else
                                'TS.PublicAddText("Company: " + AccountName(x) + ", LastUpdatedHR: " + LastUpdated(x).AddMinutes(5).ToString + ", LastUpdate: " + LastUpdate.ToString, FormName)
                            End If



                            'And let's put a note in the companies record if there have been any new tickets created since the last time we did a sync
                            'TS.PublicAddText("Sending tickets, CountAccounts=" + x.ToString, FormName)
                            'SendTicketData(token, CompanyName, AccountID(x), TS.GetOrgIDFromCRMLinkID(AccountID(x)), ParentOrgID, FormName)
                        Catch ex As Exception
                            'SyncError = True
                            TS.PublicAddText("Error in Updating People loop:" + ex.Message, FormName)
                        End Try
                    Next
                    TS.PublicAddText("Finished updating people information", FormName)
                Else
                    'No TagID so we can't grab the customers we need...
                End If '

            Else 'XML is not empty
                Return "No companies...."
            End If

            '**Moving to Form1 so we only call onece.
            'See what tickets have been created - If any, add.
            'TS.PublicAddText("Sending ticket data...", FormName)
            'SendTicketData(token, CompanyName, ParentOrgID, FormName)



            'Clear out the dynamic arrays we've been using
            Erase AccountName, AccountID, LastUpdated, City, Country, State, Street, Zip


            If Not SyncError Then
                Return "Completed"
            Else
                Return "Error"
            End If


        Catch ex As Exception
            SyncError = True

            TS.WriteToCRMResults(ParentOrgID, "Error in Perform Highrise Sync: " + ex.Message)

            Return ex.Message

        End Try

    End Function








    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3053"), _
     System.Diagnostics.DebuggerStepThroughAttribute(), _
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True), _
     System.Xml.Serialization.XmlRootAttribute([Namespace]:="", IsNullable:=False)> _
    Partial Public Class companies

        Private companyField As companiesCompany

        '''<remarks/>
        Public Property company() As companiesCompany
            Get
                Return Me.companyField
            End Get
            Set(ByVal value As companiesCompany)
                Me.companyField = value
            End Set
        End Property
    End Class
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3053"), _
     System.Diagnostics.DebuggerStepThroughAttribute(), _
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True)> _
    Partial Public Class companiesCompany

        Private idField As companiesCompanyID

        Private nameField As String

        Private backgroundField As String

        Private createdatField As companiesCompanyCreatedat

        Private updatedatField As companiesCompanyUpdatedat

        Private visibletoField As String

        Private owneridField As companiesCompanyOwnerid

        Private groupidField As companiesCompanyGroupid

        Private authoridField As companiesCompanyAuthorid

        Private contactdataField As companiesCompanyContactdata

        '''<remarks/>
        Public Property id() As companiesCompanyID
            Get
                Return Me.idField
            End Get
            Set(ByVal value As companiesCompanyID)
                Me.idField = value
            End Set
        End Property

        '''<remarks/>
        Public Property name() As String
            Get
                Return Me.nameField
            End Get
            Set(ByVal value As String)
                Me.nameField = value
            End Set
        End Property

        '''<remarks/>
        Public Property background() As String
            Get
                Return Me.backgroundField
            End Get
            Set(ByVal value As String)
                Me.backgroundField = value
            End Set
        End Property

        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("created-at")> _
        Public Property createdat() As companiesCompanyCreatedat
            Get
                Return Me.createdatField
            End Get
            Set(ByVal value As companiesCompanyCreatedat)
                Me.createdatField = value
            End Set
        End Property

        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("updated-at")> _
        Public Property updatedat() As companiesCompanyUpdatedat
            Get
                Return Me.updatedatField
            End Get
            Set(ByVal value As companiesCompanyUpdatedat)
                Me.updatedatField = value
            End Set
        End Property

        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("visible-to")> _
        Public Property visibleto() As String
            Get
                Return Me.visibletoField
            End Get
            Set(ByVal value As String)
                Me.visibletoField = value
            End Set
        End Property

        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("owner-id")> _
        Public Property ownerid() As companiesCompanyOwnerid
            Get
                Return Me.owneridField
            End Get
            Set(ByVal value As companiesCompanyOwnerid)
                Me.owneridField = value
            End Set
        End Property

        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("group-id")> _
        Public Property groupid() As companiesCompanyGroupid
            Get
                Return Me.groupidField
            End Get
            Set(ByVal value As companiesCompanyGroupid)
                Me.groupidField = value
            End Set
        End Property

        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("author-id")> _
        Public Property authorid() As companiesCompanyAuthorid
            Get
                Return Me.authoridField
            End Get
            Set(ByVal value As companiesCompanyAuthorid)
                Me.authoridField = value
            End Set
        End Property

        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("contact-data")> _
        Public Property contactdata() As companiesCompanyContactdata
            Get
                Return Me.contactdataField
            End Get
            Set(ByVal value As companiesCompanyContactdata)
                Me.contactdataField = value
            End Set
        End Property
    End Class
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3053"), _
     System.Diagnostics.DebuggerStepThroughAttribute(), _
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True)> _
    Partial Public Class companiesCompanyID

        Private typeField As String

        Private valueField As Byte

        '''<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute()> _
        Public Property type() As String
            Get
                Return Me.typeField
            End Get
            Set(ByVal value As String)
                Me.typeField = value
            End Set
        End Property

        '''<remarks/>
        <System.Xml.Serialization.XmlTextAttribute()> _
        Public Property Value() As Byte
            Get
                Return Me.valueField
            End Get
            Set(ByVal value As Byte)
                Me.valueField = value
            End Set
        End Property
    End Class
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3053"), _
     System.Diagnostics.DebuggerStepThroughAttribute(), _
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True)> _
    Partial Public Class companiesCompanyCreatedat

        Private typeField As String

        Private valueField As Date

        '''<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute()> _
        Public Property type() As String
            Get
                Return Me.typeField
            End Get
            Set(ByVal value As String)
                Me.typeField = value
            End Set
        End Property

        '''<remarks/>
        <System.Xml.Serialization.XmlTextAttribute()> _
        Public Property Value() As Date
            Get
                Return Me.valueField
            End Get
            Set(ByVal value As Date)
                Me.valueField = value
            End Set
        End Property
    End Class
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3053"), _
     System.Diagnostics.DebuggerStepThroughAttribute(), _
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True)> _
    Partial Public Class companiesCompanyUpdatedat

        Private typeField As String

        Private valueField As Date

        '''<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute()> _
        Public Property type() As String
            Get
                Return Me.typeField
            End Get
            Set(ByVal value As String)
                Me.typeField = value
            End Set
        End Property

        '''<remarks/>
        <System.Xml.Serialization.XmlTextAttribute()> _
        Public Property Value() As Date
            Get
                Return Me.valueField
            End Get
            Set(ByVal value As Date)
                Me.valueField = value
            End Set
        End Property
    End Class
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3053"), _
     System.Diagnostics.DebuggerStepThroughAttribute(), _
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True)> _
    Partial Public Class companiesCompanyOwnerid

        Private typeField As String

        '''<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute()> _
        Public Property type() As String
            Get
                Return Me.typeField
            End Get
            Set(ByVal value As String)
                Me.typeField = value
            End Set
        End Property
    End Class
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3053"), _
     System.Diagnostics.DebuggerStepThroughAttribute(), _
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True)> _
    Partial Public Class companiesCompanyGroupid

        Private typeField As String

        '''<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute()> _
        Public Property type() As String
            Get
                Return Me.typeField
            End Get
            Set(ByVal value As String)
                Me.typeField = value
            End Set
        End Property
    End Class
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3053"), _
     System.Diagnostics.DebuggerStepThroughAttribute(), _
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True)> _
    Partial Public Class companiesCompanyAuthorid

        Private typeField As String

        Private valueField As Byte

        '''<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute()> _
        Public Property type() As String
            Get
                Return Me.typeField
            End Get
            Set(ByVal value As String)
                Me.typeField = value
            End Set
        End Property

        '''<remarks/>
        <System.Xml.Serialization.XmlTextAttribute()> _
        Public Property Value() As Byte
            Get
                Return Me.valueField
            End Get
            Set(ByVal value As Byte)
                Me.valueField = value
            End Set
        End Property
    End Class
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3053"), _
     System.Diagnostics.DebuggerStepThroughAttribute(), _
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True)> _
    Partial Public Class companiesCompanyContactdata

        Private emailaddressesField As companiesCompanyContactdataEmailaddresses

        Private phonenumbersField() As companiesCompanyContactdataPhonenumber

        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("email-addresses")> _
        Public Property emailaddresses() As companiesCompanyContactdataEmailaddresses
            Get
                Return Me.emailaddressesField
            End Get
            Set(ByVal value As companiesCompanyContactdataEmailaddresses)
                Me.emailaddressesField = value
            End Set
        End Property

        '''<remarks/>
        <System.Xml.Serialization.XmlArrayAttribute("phone-numbers"), _
         System.Xml.Serialization.XmlArrayItemAttribute("phone-number", IsNullable:=False)> _
        Public Property phonenumbers() As companiesCompanyContactdataPhonenumber()
            Get
                Return Me.phonenumbersField
            End Get
            Set(ByVal value As companiesCompanyContactdataPhonenumber())
                Me.phonenumbersField = value
            End Set
        End Property
    End Class
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3053"), _
     System.Diagnostics.DebuggerStepThroughAttribute(), _
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True)> _
    Partial Public Class companiesCompanyContactdataEmailaddresses

        Private emailaddressField As companiesCompanyContactdataEmailaddressesEmailaddress

        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("email-address")> _
        Public Property emailaddress() As companiesCompanyContactdataEmailaddressesEmailaddress
            Get
                Return Me.emailaddressField
            End Get
            Set(ByVal value As companiesCompanyContactdataEmailaddressesEmailaddress)
                Me.emailaddressField = value
            End Set
        End Property
    End Class
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3053"), _
     System.Diagnostics.DebuggerStepThroughAttribute(), _
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True)> _
    Partial Public Class companiesCompanyContactdataEmailaddressesEmailaddress

        Private idField As companiesCompanyContactdataEmailaddressesEmailaddressID

        Private addressField As String

        Private locationField As String

        '''<remarks/>
        Public Property id() As companiesCompanyContactdataEmailaddressesEmailaddressID
            Get
                Return Me.idField
            End Get
            Set(ByVal value As companiesCompanyContactdataEmailaddressesEmailaddressID)
                Me.idField = value
            End Set
        End Property

        '''<remarks/>
        Public Property address() As String
            Get
                Return Me.addressField
            End Get
            Set(ByVal value As String)
                Me.addressField = value
            End Set
        End Property

        '''<remarks/>
        Public Property location() As String
            Get
                Return Me.locationField
            End Get
            Set(ByVal value As String)
                Me.locationField = value
            End Set
        End Property
    End Class
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3053"), _
     System.Diagnostics.DebuggerStepThroughAttribute(), _
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True)> _
    Partial Public Class companiesCompanyContactdataEmailaddressesEmailaddressID

        Private typeField As String

        Private valueField As Byte

        '''<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute()> _
        Public Property type() As String
            Get
                Return Me.typeField
            End Get
            Set(ByVal value As String)
                Me.typeField = value
            End Set
        End Property

        '''<remarks/>
        <System.Xml.Serialization.XmlTextAttribute()> _
        Public Property Value() As Byte
            Get
                Return Me.valueField
            End Get
            Set(ByVal value As Byte)
                Me.valueField = value
            End Set
        End Property
    End Class
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3053"), _
     System.Diagnostics.DebuggerStepThroughAttribute(), _
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True)> _
    Partial Public Class companiesCompanyContactdataPhonenumber

        Private idField As companiesCompanyContactdataPhonenumberID

        Private numberField As String

        Private locationField As String

        '''<remarks/>
        Public Property id() As companiesCompanyContactdataPhonenumberID
            Get
                Return Me.idField
            End Get
            Set(ByVal value As companiesCompanyContactdataPhonenumberID)
                Me.idField = value
            End Set
        End Property

        '''<remarks/>
        Public Property number() As String
            Get
                Return Me.numberField
            End Get
            Set(ByVal value As String)
                Me.numberField = value
            End Set
        End Property

        '''<remarks/>
        Public Property location() As String
            Get
                Return Me.locationField
            End Get
            Set(ByVal value As String)
                Me.locationField = value
            End Set
        End Property
    End Class
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3053"), _
     System.Diagnostics.DebuggerStepThroughAttribute(), _
     System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True)> _
    Partial Public Class companiesCompanyContactdataPhonenumberID

        Private typeField As String

        Private valueField As Byte

        '''<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute()> _
        Public Property type() As String
            Get
                Return Me.typeField
            End Get
            Set(ByVal value As String)
                Me.typeField = value
            End Set
        End Property

        '''<remarks/>
        <System.Xml.Serialization.XmlTextAttribute()> _
        Public Property Value() As Byte
            Get
                Return Me.valueField
            End Get
            Set(ByVal value As Byte)
                Me.valueField = value
            End Set
        End Property
    End Class



End Class
