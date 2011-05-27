' 9/21/09 - We are now appexchange certified.  Working to modify code to work with SF's partner WSDL
' 9/24/09 - A note is now created when a ticket is created in TeamSupport (similar to HR implementation)
' 9/24/09 - Made a few changes to the code so that we won't bombard our SQL server.  I think we're ready to release into the wild.
' 4/15/10 - Change to phone numbers.  Organization phone numbers are now brought over.  Fixed bug with individual phone numbers if there were duplicate e-mails in teh database.
' 10/14/10 - Working on some custom code for Axceler (not a customer yet, but a good posibility)
' 11/15/10 - First pass at custom code for Axceler
'             - Note:  I think we need to add a Waranty Expiration field in products as a real (not custom) field.  Otherwise the code for restricting a user from the portal will only be good for Axceler.
'11/16/10 - Added permanant SupportExpiration field
' 1/3/11  - Minor fix for Axceler to handle multiple of the same produts in a customer record.  We now only take the latest expiration date information.
'1/27/11  - MAJOR changes to how we deal with SF syncing.  This should be much faster now as we are only requesting the records that actually changed.
'3/9/11   - We now force an update to the contact information when we add a new account
'3/25/11  - Minor change in Axceler GetProductAndLicenseInfoFailed routine (added Order by) for ticket 4620)


Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Web.Services.Protocols
Imports System.Threading
Imports CRMLink.sForce


Public Class SalesForce
    Private Const Integration As IntegrationType = IntegrationType.SalesForce

    Public Function login(ByVal username As String, ByVal password As String, ByVal securitytoken As String, ByRef binding As sForce.SforceService) As String

        'Dim Username As String = "rjohnson@murocsystems.com"
        'Dim Password As String = "sun222dataQpcOCr3FWe1jDmcm3fUSS0IEJ"
        Dim a As New sForce.sObject




        'Console.Write("Enter username: ")
        'Dim username As String = Console.ReadLine()
        'Console.Write("Enter password: ")
        'Dim password As String = Console.ReadLine()

        ' Create a service object
        'binding = New sForce.SforceService()



        'Set the partner WSDL

        'String clientID = "your_clientid_is_case_sensitive";
        'CallOptions co = new CallOptions();
        'co.client = clientID;
        'binding.CallOptionsValue = co;

        Dim ClientID As String = "Muroc/TSCom/" '**this is our unique TS address
        Dim co As New CRMLink.sForce.CallOptions()
        co.client = ClientID
        binding.CallOptionsValue = co




        ' Timeout after a minute
        binding.Timeout = 60000

        ' Try logging in
        Dim lr As sForce.LoginResult
        Try
            'Console.WriteLine("LOGGING IN NOW...")
            lr = binding.login(username, password + securitytoken)
        Catch e As SoapException

            Dim ErrorMessage As String
            ' ApiFault is a proxy stub generated from the WSDL contract when
            ' the web service was imported
            ' Write the fault code to the console
            'Console.WriteLine(e.Code)

            ErrorMessage = e.Message




            ' Write the fault message to the console
            'Console.WriteLine("An unexpected error has occurred: " & e.Message)

            ' Write the stack trace to the console
            'Console.WriteLine(e.StackTrace)

            ' Return False to indicate that the login was not successful
            Return ErrorMessage
        End Try

        ' Check if the password has expired
        If lr.passwordExpired Then
            'Console.WriteLine("An error has occurred. Your password has expired.")
            Return "Password expired"
        End If

        '* Once the client application has logged in successfully, it will use
        '             * the results of the login call to reset the endpoint of the service
        '             * to the virtual server instance that is servicing your organization
        '             

        binding.Url = lr.serverUrl

        '* The sample client application now has an instance of the SforceService
        '             * that is pointing to the correct endpoint. Next, the sample client
        '             * application sets a persistent SOAP header (to be included on all
        '             * subsequent calls that are made with SforceService) that contains the
        '             * valid sessionId for our login credentials. To do this, the sample
        '             * client application creates a new SessionHeader object and persist it to
        '             * the SforceService. Add the session ID returned from the login to the
        '             * session header
        '             

        binding.SessionHeaderValue = New sForce.SessionHeader()
        binding.SessionHeaderValue.sessionId = lr.sessionId

        ' Return true to indicate that we are logged in, pointed
        ' at the right URL and have our security token in place.
        Return "OK"
    End Function




    

    Public Sub CheckTSAccount(ByVal AccountID As String, ByVal Name As String)
        'This should check the SF record against the record in SF

        'Steps:
        ' 1) Search to see if the Account ID exists - Need CRMID field
        '    If Yes:
        '      a) Check each element to see if it has changed and update if it has
        '    If No:
        '      a) Check to see if a company with the same account NAME exists (string match)
        '          If so, then set the CRMID to the SalesForce Account ID 
        '         Otherwise...Create a new Customer record in TS with the data
    End Sub

    Public Sub GetContactInformation(ByVal ParentOrgID As String, ByVal LastUpdate As String, ByVal TypeString As String, ByRef binding As sForce.SforceService, ByVal AccountIDToUpdate As String, ByVal ForceUpdate As Boolean)


        'This will get the contact information from SalesForce for a given account ID
        'If ForceUpdate is set them we will change the query so that we grab all contact information from the AccountID company (otherwise AccountID is not used)

        Form1.AddText("Getting Contact Information...")

        'The results will be placed in qr
        Dim qr As sForce.QueryResult = Nothing

        'binding = New sForce.SforceService()

        'We are going to increase our return batch size to 250 items
        'Setting is a recommendation only, different batch sizes may
        'be returned depending on data, to keep performance optimized.
        binding.QueryOptionsValue = New sForce.QueryOptions()
        binding.QueryOptionsValue.batchSize = 250
        binding.QueryOptionsValue.batchSizeSpecified = True

        'Dim contacts As New System.Collections.ArrayList()



        Dim TS As New TSCheckData


        Dim email, FirstName, LastName, Phone, IsDeleted, Title, mobilephone, fax, LastModified, AccountID As String
        Dim LastModifiedDateTime As DateTime

        'Dim LastUpdate = Date.Parse(TS.GetLastCRMUpdate(ParentOrgID)).ToLocalTime 'returns the last update, in local time
        'Dim LastUpdate As DateTime
        'Dim OrgTimeZone As String '= TS.GetTimeZone(ParentOrgID)
        'OrgTimeZone = "Central Standard Time"
        'The SF API converts to local time (which is CST on our servers), so we need to convert the CRMUpdate time (stored in GMT) to CST
        'LastUpdate = TS.ConvertToLocalTime(TS.GetLastCRMUpdate(ParentOrgID), ParentOrgID, OrgTimeZone) 'SF should return the last modified as a Zulu time, but they don't - Appears they return as local... Ugh.

        'Dim RawLastUpdate As DateTime = TS.GetLastCRMUpdate(ParentOrgID)



        Try
            'qr = Binding.query("select FirstName, LastName from Contact")

            Dim HasFax As Boolean = True
            Dim done As Boolean = False

            Try
                'SystemModstamp or LastModifiedDate
                'qr = binding.query("select email, FirstName, LastName, Phone, MobilePhone, Fax, Title, IsDeleted, SystemModstamp from Contact where AccountID = '" + AccountID + "'")

                If Not ForceUpdate Then 'this the normal query
                    'This should give us a list of all the contact that in our account group that have been modified since the last time (- a few hours) we ran this
                    qr = binding.query("select email, FirstName, LastName, Phone, MobilePhone, Fax, Title, IsDeleted, SystemModstamp, Account.ID from Contact where SystemModStamp >= " + LastUpdate + " and (" + TypeString + ")")
                Else
                    Form1.AddText("Using force update query")
                    qr = binding.query("select email, FirstName, LastName, Phone, MobilePhone, Fax, Title, IsDeleted, SystemModstamp, Account.ID from Contact where Account.ID = '" + AccountIDToUpdate + "'")
                End If

                done = False

                HasFax = True

                Form1.AddText("Found " + qr.size.ToString + " contact records.")
            Catch ex As Exception
                'Uh Oh - Error.  Probably no fax number, so we'll try without the fax number
                Form1.AddDebugText("Error getting contact - Trying without fax number.")
                'SystemModstamp or LastModifiedDate
                If Not ForceUpdate Then
                    qr = binding.query("select email, FirstName, LastName, Phone, MobilePhone, Phone, Title, IsDeleted, SystemModstamp, Account.ID from Contact where SystemModStamp >= " + LastUpdate + " and (" + TypeString + ")")
                Else
                    qr = binding.query("select email, FirstName, LastName, Phone, MobilePhone, Phone, Title, IsDeleted, SystemModstamp, Account.ID from Contact where Account.ID = " + AccountIDToUpdate)
                End If

                'Note - Added second phone just so we have something in that space..
                done = False

                Form1.AddDebugText("Found " + qr.size.ToString + " contact records (no fax).")
                HasFax = False

            End Try


            If qr.size > 0 Then
                'Console.WriteLine("Logged-in user can see " & qr.records.Length & " contact records.")
                While Not done
                    'Console.WriteLine("")
                    For i As Integer = 0 To qr.records.Length - 1

                        Dim records As sForce.sObject() = qr.records
                        Dim contact As sForce.sObject = records(i)

                        'StuffToReturn = records(i).Any(0).InnerText + " / " + records(i).Any(1).InnerText + " / " + records(i).Any(2).InnerText + " / " + records(i).Any(3).InnerText + " / " + records(i).Any(4).InnerText + " / " + records(i).Any(5).InnerText + " / " + records(i).Any(6).InnerText + " / " + records(i).Any(7).InnerText + " / " + records(i).Any(8).InnerText

                        email = records(i).Any(0).InnerText
                        FirstName = records(i).Any(1).InnerText
                        LastName = records(i).Any(2).InnerText
                        Phone = records(i).Any(3).InnerText
                        mobilephone = records(i).Any(4).InnerText
                        AccountID = records(i).Any(9).InnerText
                        If AccountID.Length > 18 Then
                            'I have no idea why, but SF returns the ID here as something like "Account0018000000eM3oOAAS0018000000eM3oOAAS" instead of the standard 18 character account id
                            'This will return just the final 18 characters which should work.
                            AccountID = AccountID.Substring(AccountID.Length - 18)
                        End If
                        If HasFax Then
                            fax = records(i).Any(5).InnerText
                            Title = records(i).Any(6).InnerText
                            IsDeleted = records(i).Any(7).InnerText
                            LastModified = records(i).Any(8).InnerText
                            LastModifiedDateTime = Date.Parse(records(i).Any(8).InnerText)
                        Else
                            fax = ""
                            Title = records(i).Any(5).InnerText
                            IsDeleted = records(i).Any(6).InnerText
                            LastModified = records(i).Any(7).InnerText
                            LastModifiedDateTime = Date.Parse(records(i).Any(7).InnerText)
                        End If

                        'Now we have the info from salesforce, let's see if it matches with anything in the TS database and update as needed
                        'TS.UpdateAddressInfoIfNeeded(ID, Name, ShippingStreet, "", ShippingCity, ShippingState, ShippingPostalCode, ShippingCountry)
                        'TS.AddOrUpdateAccountInformation(ID, Name, ShippingStreet, "", ShippingCity, ShippingState, ShippingPostalCode, ShippingCountry, "1078") '**1078 is temporary

                        '**Now we need to grab the contacts and update them as needed
                        'Update if the last modified time is recent, OR if we call a forceupdate - This would be if the main account was modified

                        '*** The SalesForce Last Modified Time field AUTOMATICALLY converts to the computer's LOCAL timezone!!!  So, since the computer
                        '*** is running in central, all timezone info should be converted to central first.
                        '*** I have no idea why SF does this since all documentation shows that the return values are Zulu... (11/30/10)

                        ' Form1.AddDebugText("Contact LastModifiedDateTime =" + LastModifiedDateTime.ToString + ".  LastUpdate = " + LastUpdate.ToString + ".  SFLastUpdate = " +  + ".  Difference in Minutes = " + DateDiff(DateInterval.Minute, LastModifiedDateTime, LastUpdate).ToString)


                        'Since we are now only looking at a list of accounts that have been updated, we can run the add/update routine on all of them
                        TS.AddOrUpdateContactInformation(AccountID, email, FirstName, LastName, Phone, fax, mobilephone, Title, IsDeleted, ParentOrgID)  '**1078 is temporary

                        'If ((LastModifiedDateTime > LastUpdate) Or (DateDiff(DateInterval.Minute, LastModifiedDateTime, LastUpdate) <= 60)) Or ForceUpdate Then
                        'Form1.AddDebugText("Updating record since it apparently has changed.")
                        'TS.AddOrUpdateContactInformation(AccountID, email, FirstName, LastName, Phone, fax, mobilephone, Title, IsDeleted, ParentOrgID)  '**1078 is temporary
                        'Else
                        'Form1.AddDebugText("Record has not changed so we're not updating.")
                        'End If

                    Next
                    If qr.done Then
                        done = True
                    Else
                        qr = binding.queryMore(qr.queryLocator)
                    End If
                End While
            Else
                Console.WriteLine("No records found.")
            End If


            Form1.AddText("All done with contact records!")

        Catch ex As Exception
            'Console.WriteLine(vbLf & "Failed to execute query succesfully," & "error message was: " & vbLf & "{0}", ex.Message)
            Form1.AddText("Failed to execute query succesfully," & "error message was: " & ex.Message)
            TS.WriteToCRMResults(ParentOrgID, "Error in GetContactInformation: " + ex.ToString)
            'Return ex.Message
        End Try
        'Console.WriteLine(vbLf & vbLf & "Hit enter to exit...")
        'Console.ReadLine()
        'Return "Done"


    End Sub

    Private Function GetNewXmlElement(ByVal Name As String, ByVal nodeValue As String) As System.Xml.XmlElement
        Dim doc As System.Xml.XmlDocument = New System.Xml.XmlDocument
        Dim xmlel As System.Xml.XmlElement = doc.CreateElement(Name)
        xmlel.InnerText = nodeValue
        Return xmlel
    End Function


    Public Function ProcessSalesForceAccountInformation(ByVal ParentOrgID As String, ByVal TypeToMatch As String, ByVal username As String, ByVal password As String, ByVal SecurityToken As String) As String

        Form1.AddDebugText("Binding to SF object")

        Dim binding As New sForce.SforceService

        Form1.AddDebugText("Attepmting to login")

        Dim LoginReturn As String = login(Trim(username), Trim(password), Trim(SecurityToken), binding)



        If LoginReturn = "OK" Then

            Form1.AddDebugText("Logged in OK")


            'The results will be placed in qr
            Dim qr As sForce.QueryResult = Nothing


            binding.QueryOptionsValue = New sForce.QueryOptions()

            'setting this to an absurdly high value.
            binding.QueryOptionsValue.batchSize = 2000 '**What happens if we return more information than the batch size??
            binding.QueryOptionsValue.batchSizeSpecified = True

            'Dim contacts As New System.Collections.ArrayList()



            Dim TS As New TSCheckData


            Dim ID, Name, Type, ShippingStreet, ShippingCity, ShippingState, ShippingPostalCode, ShippingCountry, Phone As String
            Dim LastModifiedDateTime As DateTime

            Dim LastUpdate As DateTime
            'LastUpdate = Date.Parse(TS.GetLastCRMUpdate(ParentOrgID)).ToLocalTime) 'returns the last update, in local computer time
            Dim OrgTimeZone As String '= TS.GetTimeZone(ParentOrgID)
            OrgTimeZone = "Central Standard Time"
            LastUpdate = TS.ConvertToLocalTime(TSCheckData.GetLastCRMUpdate(ParentOrgID, Integration), ParentOrgID, OrgTimeZone)

            Dim LastUpdateSFFormat As String 'format for SF query for time is 2011-01-26T16:57:00.000Z  ('yyyy'-'MM'-'dd'T'HH': 'mm': 'ss.fffffff'Z' )
            Dim TempTime As DateTime
            TempTime = Date.Parse(TSCheckData.GetLastCRMUpdate(ParentOrgID, Integration))
            TempTime = DateAdd(DateInterval.Hour, -3, TempTime) 'push last update time back three hours to make sure we catch every change
            LastUpdateSFFormat = TempTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fff'Z'")


            'SalesForce returns the LastModifed time in the Local Timezone of the organization.  Therefor, we need to store the LastUpdate
            ' time as UTC and convert it to the local organization's TZ so they both match.
            '**If the TS org's TZ does not match SF's TZ then we're in a world of hurt.

            Form1.AddDebugText("LastUpdate (last time CRM Sync Ran, in the organization's local timezone) = " + LastUpdate)

            Try

                'This code should let us support multiple account types separated by a comma
                Dim TypeString As String = ""
                Dim AccountTypeString As String = ""
                Dim MatchArray As String() = TypeToMatch.Split(",")
                For z As Integer = 0 To MatchArray.Length - 1
                    If z > 0 Then
                        TypeString = TypeString + " or "
                        AccountTypeString = AccountTypeString + " or "
                    End If

                    TypeString = TypeString + " type like '%" + Trim(MatchArray(z)) + "%'"
                    AccountTypeString = AccountTypeString + " Account.Type like '%" + Trim(MatchArray(z)) + "%'"
                Next

                Form1.AddDebugText("TypeString = " + TypeString)

                'qr = Binding.query("select FirstName, LastName from Contact")

                Dim SFQuery As String
                Dim done As Boolean = False
                Dim HasAddress As Boolean = True

                Dim BindingRanOK As Boolean
                Try
                    Try

                        'OK, lets try this with the shipping addresses
                        'SFQuery = "select ID, Name, type, ShippingStreet, ShippingCity, ShippingState, ShippingPostalCode, ShippingCountry, Phone, LastModifiedDate, SystemModStamp from Account where " + TypeString ' + " and SystemModStamp >= 2011-01-22T20:50:18.000Z" '" + TypeString + " and 
                        SFQuery = "select ID, Name, type, ShippingStreet, ShippingCity, ShippingState, ShippingPostalCode, ShippingCountry, Phone, LastModifiedDate, SystemModStamp from Account where SystemModStamp >= " + LastUpdateSFFormat + " and (" + TypeString + ")" ' and " + TypeString
                        Form1.AddDebugText("SF Query String = " + SFQuery)

                        qr = binding.query(SFQuery)
                        Form1.AddText("qr.size = " + qr.size.ToString)
                        BindingRanOK = True
                    Catch ex As Exception
                        'Hmm...No shipping addresses.  Let's try billing addresses
                        'This doesn't appear to work either...? 7/1/10
                        Try
                            Form1.AddDebugText("Shipping Address information not found - Attempting to find Billing Address information instead.")
                            'SFQuery = "select ID, Name, type, BillingStreet, BillingCity, BillingState, BillingPostalCode, BillingCountry, Phone, LastModifiedDate from Account where " + TypeString + " and LastModifiedDate >= 2012-01-07T20:27:18.000Z"
                            SFQuery = "select ID, Name, type, BillingStreet, BillingCity, BillingState, BillingPostalCode, BillingCountry, Phone, LastModifiedDate, SystemModStamp from Account where SystemModStamp >= " + LastUpdateSFFormat + " and (" + TypeString + ")" ' and " + TypeString
                            Form1.AddDebugText("SF Query String = " + SFQuery)

                            qr = binding.query(SFQuery)
                            Form1.AddText("qr.size = " + qr.size.ToString)
                            BindingRanOK = True
                        Catch ex2 As Exception
                            'Well crap - No billing address either.
                            Form1.AddDebugText("Billing Address information not found - Attempting to process company with no address info at all.")
                            'SFQuery = "select ID, Name, type, Phone, LastModifiedDate from Account where " + TypeString
                            SFQuery = "select ID, Name, type,Phone, LastModifiedDate, SystemModStamp from Account where SystemModStamp >= " + LastUpdateSFFormat + " and (" + TypeString + ")" ' and " + TypeString
                            Form1.AddDebugText("SF Query String = " + SFQuery)

                            qr = binding.query(SFQuery)
                            Form1.AddText("qr.size = " + qr.size.ToString)
                            HasAddress = False 'Set this to false so we can set local variables correctly
                            BindingRanOK = True
                        End Try
                    End Try
                Catch ex As Exception
                    'OK, we have a major error binding the query.
                    BindingRanOK = False
                    Form1.AddText("Error when attempting to bind the Query: " + ex.Message)
                End Try


                If (BindingRanOK) And (qr.size > 0) Then
                    'We now have a list of all accounts that have been modified in the last 3 hours and that match our account types. Let's update.
                    While Not done
                        'Console.WriteLine("")

                        Form1.AddDebugText("Begining While Loop to get companies.  qr.records.length = " + qr.records.Length.ToString)
                        Form1.AddText("Updating company information, qr.size = " + qr.size.ToString)
                        For i As Integer = 0 To qr.records.Length - 1

                            Form1.AddDebugText("In for loop iteration " + i.ToString)

                            Dim records As sForce.sObject() = qr.records
                            Dim contact As sForce.sObject = records(i)

                            'StuffToReturn = records(i).Any(0).InnerText + " / " + records(i).Any(1).InnerText + " / " + records(i).Any(2).InnerText + " / " + records(i).Any(3).InnerText + " / " + records(i).Any(4).InnerText + " / " + records(i).Any(5).InnerText + " / " + records(i).Any(6).InnerText + " / " + records(i).Any(7).InnerText + " / " + records(i).Any(8).InnerText

                            ID = records(i).Any(0).InnerText
                            Name = records(i).Any(1).InnerText
                            Type = records(i).Any(2).InnerText
                            If HasAddress Then
                                ShippingStreet = records(i).Any(3).InnerText
                                ShippingCity = records(i).Any(4).InnerText
                                ShippingState = records(i).Any(5).InnerText
                                ShippingPostalCode = records(i).Any(6).InnerText
                                ShippingCountry = records(i).Any(7).InnerText
                                Phone = records(i).Any(8).InnerText
                                LastModifiedDateTime = Date.Parse(records(i).Any(9).InnerText)
                            Else
                                'No address
                                ShippingStreet = ""
                                ShippingCity = ""
                                ShippingState = ""
                                ShippingPostalCode = ""
                                ShippingCountry = ""
                                Phone = records(i).Any(3).InnerText
                                LastModifiedDateTime = Date.Parse(records(i).Any(4).InnerText)
                            End If
                            'test code
                            If Name = "TeamSupport Test" Then
                                Dim test1 As String
                                test1 = Name
                            End If
                            'end test

                            Form1.AddDebugText("Company name=" + Name)
                            Form1.AddDebugText("Record Last Modified in SF= " + LastModifiedDateTime.ToString)

   

                            Form1.AddDebugText("Company " + Name + " has been modified.")

                            'Now we have the info from salesforce, let's see if it matches with anything in the TS database and update as needed
                            TS.AddOrUpdateAccountInformation(ID, Name, ShippingStreet, "", ShippingCity, ShippingState, ShippingPostalCode, ShippingCountry, Phone, ParentOrgID)
                            Form1.AddDebugText("Completed AddOrUpdateAccountInformation for company " + Name)
                            'Let's force an update of contact information for this company
                            GetContactInformation(ParentOrgID, LastUpdateSFFormat, AccountTypeString, binding, ID, True)
                            Form1.AddDebugText("Completed force update contact info for " + Name)
                            'If we have to update the account, we need to force the updating of contact info
                            'GetContactInformation(ID, ParentOrgID, True, binding) 88moving to another routine
                            'Form1.AddDebugText("Completed GetContactInformation for company " + Name)
                           

                            'Now lets update the ticket information


                            


                        Next
                        If qr.done Then
                            done = True
                        Else
                            Form1.AddDebugText("Requesting more records (should be more than 2000 companies?)")
                            qr = binding.queryMore(qr.queryLocator)
                        End If
                    End While

                    Form1.AddText("All done updating company information.")

 
                Else
                    Console.WriteLine("No records found.")
                    Form1.AddDebugText("**No matching record found!!")
                End If

                If BindingRanOK Then
                    'Breaking this out separately since they were not running if there were no companies that had changed.

                    'We updated all of the ACCOUNT information above
                    'Now let's update all of the contact information
                    GetContactInformation(ParentOrgID, LastUpdateSFFormat, AccountTypeString, binding, "0", False)

                    'Now lets send the ticket data.
                    'This will list all tickets created in the ParentOrgID since the last time we ran a CRMLink and post those to the correct record in SF.
                    Form1.AddDebugText("Starting SendTicketData")
                    SendTicketData(ParentOrgID, binding)


                    'Code for Axceler to update their product and license information
                    If (ParentOrgID = 305383) Then 'Or (ParentOrgID = 1078) Then
                        'axceler org id is 305383

                        'Now search out and get product and license information that has been updated.
                        GetProductAndLicenseInfo(LastUpdateSFFormat, binding, ParentOrgID)
                    End If
                End If
            Catch ex As Exception
                'Console.WriteLine(vbLf & "Failed to execute query succesfully," & "error message was: " & vbLf & "{0}", ex.Message)
                TS.WriteToCRMResults(ParentOrgID, "Error in ProcessSalesForceAccountInformation: " + ex.ToString)
                Form1.AddDebugText("**Error:  " + ex.Message)
                Return ex.Message
            End Try


            'Added to see if it solved problem with 2+ SalesForce accounts being active.
            binding.logout()
            binding.logoutAsync()

            Return "Completed"
        Else
            Return LoginReturn
        End If



    End Function








    Public Sub CreateNote(ByVal accountid As String, ByVal Title As String, ByVal Body As String, ByVal ParentOrgID As String, ByRef binding As sForce.SforceService)

        'This will create a note for the SalesForce account

        Try

            'Attach a note, which will get re-parented
            Dim note As sObject = New sObject()
            note.type = "Note"
            note.Any = New System.Xml.XmlElement() {GetNewXmlElement("ParentId", accountid), _
                GetNewXmlElement("Body", Body), _
                GetNewXmlElement("Title", Title)}

            Dim noteSave As SaveResult = binding.create(New sObject() {note})(0)

        Catch ex As Exception
            Form1.AddText("Error in CreateNote: " & ex.Message)
            Dim ts As New TSCheckData

            ts.WriteToCRMResults(ParentOrgID, "Error in CreateNote: " + ex.Message)
        End Try
    End Sub

    Public Sub SendTicketData(ByVal ParentOrgID As String, ByRef binding As sForce.SforceService)
        '8/8/09 - CHanged this so that we only make one call to our database then update tickets based on that one call.  
        '3/25/10 - We should no longer resend all notes if the year is 19 something (should prevent the massing re-creation of notes)


        Dim TS As New TSCheckData

        If TSCheckData.SendBackTicketData(ParentOrgID, Integration) Then

            If Not TSCheckData.GetLastCRMUpdate(ParentOrgID, Integration).StartsWith("1/1/1980") Then 'if we do a resync, the year will be set to 1990 or 1970 - In either case, we don't want to resend the notes, so don't run this if the year is in the 19's

                Dim tickets As DataTable

                'get tickets which were created after the last link date
                tickets = TSCheckData.GetLatestTickets(ParentOrgID, Integration)

                If tickets IsNot Nothing Then
                    For Each thisTicket As DataRow In tickets.Rows

                        'Add the new tickets to the company record
                        Dim NoteBody As String = "A new support ticket has been created for this Account entitled """ + thisTicket(1).ToString + """.  Click here to access the ticket information: https://app.teamsupport.com/Ticket.aspx?ticketid=" + thisTicket(0).ToString

                        Form1.AddText("Creating a note...")
                        CreateNote(thisTicket(3).ToString, "Support Issue: " + thisTicket(1).ToString, NoteBody, ParentOrgID, binding)

                        Application.DoEvents()
                    Next
                End If
            End If

        Else
            Form1.AddText("SendBackTicketData set to FALSE for this organization.")
        End If

    End Sub

    Public Sub GetProductAndLicenseInfo(ByVal SFLastUpdateTime As String, ByRef binding As sForce.SforceService, ByVal ParentOrgID As String)

        'This is *** CUSTOM CODE *** for Axceler to see if we can get their license and product information


        'This will get the contact information from SalesForce for a given account ID

        Form1.AddDebugText("In GetProductAndLicenseInfoF routine")

        'The results will be placed in qr
        Dim qr As sForce.QueryResult = Nothing

        'binding = New sForce.SforceService()

        'We are going to increase our return batch size to 250 items
        'Setting is a recommendation only, different batch sizes may
        'be returned depending on data, to keep performance optimized.
        binding.QueryOptionsValue = New sForce.QueryOptions()
        binding.QueryOptionsValue.batchSize = 250
        binding.QueryOptionsValue.batchSizeSpecified = True





        Dim TS As New TSCheckData

        Dim ProductName, ExpirationDate, LicenseType, LicenseStatus, AccountID As String

        'Dim email, FirstName, LastName, Phone, IsDeleted, Title, mobilephone, fax, LastModified As String
        'Dim LastModifiedDateTime As DateTime

        'Dim LastUpdate = Date.Parse(TS.GetLastCRMUpdate(ParentOrgID)).ToLocalTime 'returns the last update, in local time
        'Dim LastUpdate As DateTime
        'Dim OrgTimeZone As String = TS.GetTimeZone(ParentOrgID)
        'LastUpdate = TS.ConvertToLocalTime(TS.GetLastCRMUpdate(ParentOrgID), ParentOrgID, OrgTimeZone)
        'LastUpdate = TS.GetLastCRMUpdate(ParentOrgID) 'changing to be UTC instead of converting from local times


        Try
            'qr = Binding.query("select FirstName, LastName from Contact")

            'Dim HasFax As Boolean = True
            Dim done As Boolean = False

            Try
                'qr = binding.query("select Product__c, Expiration_Date__c, License_type__c, Status__c from License__c where Account__c = '" + AccountID + "'")
                'qr = binding.query("select Product__c, Expiration_Date__c, License_type__c, Status__c from License__c where Account__c = '" + AccountID + "' and lastmodifieddate >= 2011-01-22T20:50:18.000Z")
                qr = binding.query("select Product__c, Expiration_Date__c, License_type__c, Status__c, Account__c from License__c where SystemModStamp >= " + SFLastUpdateTime + " ORDER BY Product__c") 'added order by 3/25/11

                'and SystemModStamp >= 2011-01-22T20:50:18.000Z" '"
                done = False



                Form1.AddDebugText("Found " + qr.size.ToString + " license records.")
            Catch ex As Exception
                'Problem - Not sure what.
                done = True

                Form1.AddDebugText("Error in GetProductAndLicenseInfo. " + ex.Message)


            End Try

            Dim LastExpirationDate As Date = DateTime.Parse("Jan 01, 1970 12:00:00 PM") 'set to default date way back
            Dim LastProduct As String = ""

            If qr.size > 0 Then
                'Console.WriteLine("Logged-in user can see " & qr.records.Length & " contact records.")
                While Not done
                    'Console.WriteLine("")
                    For i As Integer = 0 To qr.records.Length - 1

                        Form1.AddText("In GetProductAndLicenseInfo routine - i=" + i.ToString)

                        Dim records As sForce.sObject() = qr.records
                        Dim Products As sForce.sObject = records(i)

                        ProductName = records(i).Any(0).InnerText
                        ExpirationDate = records(i).Any(1).InnerText
                        LicenseType = records(i).Any(2).InnerText
                        LicenseStatus = records(i).Any(3).InnerText
                        AccountID = records(i).Any(4).InnerText


                        'OK, what do we do now?
                        ' 1) See if we can match a product in TS with the product name
                        '     select ProductID from products where name = {ProductName} and organizationid=305383
                        '     -If we can't get the product ID then we can skip the rest...

                        Dim ProductID As String = TS.GetProductIDFromName(ParentOrgID, ProductName)

                        Form1.AddText("ProductName = " + ProductName + ", ExpirationDate=" + ExpirationDate + ", ProductID=" + ProductID)

                        If ProductID <> "" Then
                            'OK, we have a product ID!  
                            ' 2) If we can, lets see if the product ID is assigned to this customer
                            '   select OrganizationProductID from OrganizationProducts where Organizationid = {clientorgid} and productid={productid from above}

                            Dim ClientOrgID As String = TS.GetOrgIDFromCRMLinkID(AccountID)

                            Dim OrganizationProductID As String = TS.GetOrganizationProductID(ClientOrgID, ProductID)

                            If OrganizationProductID <> "" Then
                                'The company already has the product associated with them.
                                ' We now just need to update the waranty expiration date 
                                '  Note that the waranty expiration date is a custom field just for this customer...
                                '      3) Update the waranty expiration date (this should be a custom field on product)
                                '         Update CustomValue set CustomValue={ExpirationDate} where CustomFieldID = 3761 and RefID={OrganizationProductID}

                                '**** Should be 3761 - Test value is 99
                                'Expiration Date - CustomFieldID is 3761 (test value 99)

                                If (ProductName <> LastProduct) Or (Date.Parse(ExpirationDate) > LastExpirationDate) Then 'test to see if we are using the most up to date expiration date (only use product/expiration date that is the most recent)
                                    'TS.UpdateCustomValue(Date.Parse(ExpirationDate).ToString, 3761, OrganizationProductID, ClientOrgID) '3761 is specific for Axceler
                                    TS.UpdateSupportExpiration(OrganizationProductID, Date.Parse(ExpirationDate).ToString)
                                    'License Type - CustomFieldID is 3770 (test value 101)
                                    TS.UpdateCustomValue(LicenseType, 3770, OrganizationProductID, ClientOrgID) '3761 is specific for Axceler
                                    'License Status - CustomFieldID is 3771 (test value 102)
                                    TS.UpdateCustomValue(LicenseStatus, 3771, OrganizationProductID, ClientOrgID) '3761 is specific for Axceler

                                    LastProduct = ProductName
                                Else
                                    Form1.AddText("Date information not updated since there is a later expiration date.")
                                End If 'testing to see which product we are on

                                LastExpirationDate = Date.Parse(ExpirationDate) 'changed from just expirationdate to date.parse(expirationdate) 4/4/11

                            Else
                                'We need to add the product to this company
                                '      3) Add the product to the organization
                                '         Insert into OrganizationProducts (OrganizationID, ProductID, IsVisibleOnPortal, DateCreated, DateModified, CreatorID, ModifierID) Values({ClientOrgID}, {ProductID}, 0, getutcdate(), getutcdate(), 47, 47)
                                TS.AddProductToOrganization(ClientOrgID, ProductID)

                                OrganizationProductID = TS.GetOrganizationProductID(ClientOrgID, ProductID)

                                'Product added, lets add/update the expiration date
                                'TS.UpdateCustomValue(Date.Parse(ExpirationDate).ToString, 3761, OrganizationProductID, ClientOrgID) '3761 is specific for Azceler
                                TS.UpdateSupportExpiration(OrganizationProductID, Date.Parse(ExpirationDate).ToString)
                                'License Type - CustomFieldID is 3770 (test value 101)
                                TS.UpdateCustomValue(LicenseType, 3770, OrganizationProductID, ClientOrgID) '3761 is specific for Axceler
                                'License Status - CustomFieldID is 3771 (test value 102)
                                TS.UpdateCustomValue(LicenseStatus, 3771, OrganizationProductID, ClientOrgID) '3761 is specific for Axceler


                            End If

                        End If



                    Next
                    'If qr.done Then
                    done = True
                    'Else
                    'qr = binding.queryMore(qr.queryLocator)
                    'End If
                End While
            Else
                Console.WriteLine("No records found.")
            End If




        Catch ex As Exception
            'Console.WriteLine(vbLf & "Failed to execute query succesfully," & "error message was: " & vbLf & "{0}", ex.Message)
            Form1.AddText("Error in GetProductAndLicenseInfo : Failed to execute query succesfully," & "error message was: " & ex.Message)
            'TS.WriteToCRMResults(ParentOrgID, "Error in GetContactInformation: " + ex.ToString)
            'Return ex.Message
        End Try
        'Console.WriteLine(vbLf & vbLf & "Hit enter to exit...")
        'Console.ReadLine()
        'Return "Done"


    End Sub


End Class
