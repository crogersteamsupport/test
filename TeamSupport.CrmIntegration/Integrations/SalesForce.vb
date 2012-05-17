'changelog info at teamsupport.beanstalkapp.com/main

Imports System.Text
Imports System.Web.Services.Protocols
Imports System.Threading
Imports sForce
Imports TeamSupport.Data

Namespace TeamSupport
    Namespace CrmIntegration

        Public Class SalesForce
            Inherits Integration

            Private Binding As SforceService

            Public Sub New(ByVal crmLinkOrg As CRMLinkTableItem, ByVal crmLog As SyncLog, ByVal thisUser As LoginUser, ByVal thisProcessor As CrmProcessor)
                MyBase.New(crmLinkOrg, crmLog, thisUser, thisProcessor, IntegrationType.SalesForce)

                Log.Write("Binding to SF object")
                Binding = New SforceService()
            End Sub

            Public Overrides Function PerformSync() As Boolean
                Dim Success As Boolean = True

                Success = SyncAccounts()

                If Success Then
                    Success = SendSFTicketData()
                End If

                Return Success
            End Function

            Private Function SyncAccounts() As Boolean
                Dim SecurityToken As String = CRMLinkRow.SecurityToken
                Dim CompanyName As String = CRMLinkRow.Username
                Dim Password As String = CRMLinkRow.Password
                Dim ParentOrgID As String = CRMLinkRow.OrganizationID
                Dim TagsToMatch As String = CRMLinkRow.TypeFieldMatch

                Log.Write("Attempting to log in")

                Dim LoginReturn As String = login(Trim(CompanyName), Trim(Password), Trim(SecurityToken))

                If LoginReturn = "OK" Then

                    Log.Write("Logged in OK")

                    'The results will be placed in qr
                    Dim qr As QueryResult = Nothing

                    Binding.QueryOptionsValue = New QueryOptions()

                    'setting this to an absurdly high value.
                    Binding.QueryOptionsValue.batchSize = 2000 '**What happens if we return more information than the batch size??
                    Binding.QueryOptionsValue.batchSizeSpecified = True

                    Dim LastUpdateSFFormat As String
                    Dim TempTime As New DateTime(1900, 1, 1)

                    If CRMLinkRow.LastLink IsNot Nothing Then
                        TempTime = CRMLinkRow.LastLink.Value.AddHours(-1) 'push last update time back 1 hour to make sure we catch every change
                    End If

                    LastUpdateSFFormat = TempTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fff'Z'") 'format for SF query for time is 2011-01-26T16:57:00.000Z  ('yyyy'-'MM'-'dd'T'HH': 'mm': 'ss.fffffff'Z' )

                    'SalesForce returns the LastModifed time in the Local Timezone of the organization.  Therefor, we need to store the LastUpdate
                    ' time as UTC and convert it to the local organization's TZ so they both match.
                    '**If the TS org's TZ does not match SF's TZ then we're in a world of hurt.

                    Log.Write("LastUpdate (last time CRM Sync Ran, in the organization's local timezone) = " + CRMLinkRow.LastLink.ToString())

                    Try

                        'This code should let us support multiple account types separated by a comma
                        Dim TypeString As String = ""
                        Dim AccountTypeString As String = ""
                        Dim MatchArray As String() = Array.ConvertAll(TagsToMatch.Split(","), Function(p As String) p.Trim())
                        If MatchArray.Contains(String.Empty) Then
                            Log.Write("Missing Account Type to Link To TeamSupport (TypeFieldMatch).")
                            SyncError = True
                        Else
                            For z As Integer = 0 To MatchArray.Length - 1
                                If z > 0 Then
                                    TypeString = TypeString + " or "
                                    AccountTypeString = AccountTypeString + " or "
                                End If

                                Dim tagToMatch As String = MatchArray(z)
                                If tagToMatch.ToLower() = "none" Then
                                    tagToMatch = String.Empty
                                End If

                                TypeString = TypeString + " type = '" + tagToMatch + "'"
                                AccountTypeString = AccountTypeString + " Account.Type = '" + tagToMatch + "'"
                            Next

                            Log.Write("TypeString = " + TypeString)

                            Dim SFQuery As String
                            Dim done As Boolean = False
                            Dim HasAddress As Boolean = True
                            Dim hasFax As Boolean = False
                            Dim queriedShippingAddress As Boolean = False

                            Dim BindingRanOK As Boolean
                            Try
                                'These try catch blocks are implemented because some companies do not have all the fields been queried.
                                'I am adding trying with and without Fax by not knowing if it is included or not in all companies.
                                Try
                                    '1st attempt: with Shipping address and fax.
                                    SFQuery = "select ID, Name, type, ShippingStreet, ShippingCity, ShippingState, ShippingPostalCode, ShippingCountry, Phone, LastModifiedDate, SystemModStamp, Fax from Account where SystemModStamp >= " + LastUpdateSFFormat + " and (" + TypeString + ")"
                                    Log.Write("SF Query String = " + SFQuery)

                                    qr = Binding.query(SFQuery)
                                    Log.Write("qr.size = " + qr.size.ToString)
                                    BindingRanOK = True
                                    hasFax = True
                                    queriedShippingAddress = True
                                Catch ex As Exception
                                    Try
                                        '2nd attempt: with Shipping address and without fax.
                                        SFQuery = "select ID, Name, type, ShippingStreet, ShippingCity, ShippingState, ShippingPostalCode, ShippingCountry, Phone, LastModifiedDate, SystemModStamp from Account where SystemModStamp >= " + LastUpdateSFFormat + " and (" + TypeString + ")"
                                        Log.Write("SF Query String = " + SFQuery)

                                        qr = Binding.query(SFQuery)
                                        Log.Write("qr.size = " + qr.size.ToString)
                                        BindingRanOK = True
                                        hasFax = False
                                        queriedShippingAddress = True
                                    Catch ex1 As Exception
                                        Try
                                            '3rd attempt: With Billing address and fax
                                            Log.Write("Shipping Address information not found - Attempting to find Billing Address information instead.")
                                            SFQuery = "select ID, Name, type, BillingStreet, BillingCity, BillingState, BillingPostalCode, BillingCountry, Phone, LastModifiedDate, SystemModStamp, Fax from Account where SystemModStamp >= " + LastUpdateSFFormat + " and (" + TypeString + ")" ' and " + TypeString
                                            Log.Write("SF Query String = " + SFQuery)

                                            qr = Binding.query(SFQuery)
                                            Log.Write("qr.size = " + qr.size.ToString)
                                            BindingRanOK = True
                                            hasFax = True
                                        Catch ex2 As Exception
                                            Try
                                                '4th attempt: With Billing address and without fax
                                                Log.Write("Shipping Address information not found - Attempting to find Billing Address information instead.")
                                                SFQuery = "select ID, Name, type, BillingStreet, BillingCity, BillingState, BillingPostalCode, BillingCountry, Phone, LastModifiedDate, SystemModStamp from Account where SystemModStamp >= " + LastUpdateSFFormat + " and (" + TypeString + ")" ' and " + TypeString
                                                Log.Write("SF Query String = " + SFQuery)

                                                qr = Binding.query(SFQuery)
                                                Log.Write("qr.size = " + qr.size.ToString)
                                                BindingRanOK = True
                                                hasFax = False
                                            Catch ex3 As Exception
                                                Try
                                                    '5th attempt: Without address and with Fax
                                                    Log.Write("Billing Address information not found - Attempting to process company with no address info at all.")
                                                    SFQuery = "select ID, Name, type,Phone, LastModifiedDate, SystemModStamp, Fax from Account where SystemModStamp >= " + LastUpdateSFFormat + " and (" + TypeString + ")" ' and " + TypeString
                                                    Log.Write("SF Query String = " + SFQuery)

                                                    qr = Binding.query(SFQuery)
                                                    Log.Write("qr.size = " + qr.size.ToString)
                                                    HasAddress = False 'Set this to false so we can set local variables correctly
                                                    BindingRanOK = True
                                                    hasFax = True
                                                Catch ex4 As Exception
                                                    Log.Write("Billing Address information not found - Attempting to process company with no address info at all.")
                                                    SFQuery = "select ID, Name, type,Phone, LastModifiedDate, SystemModStamp from Account where SystemModStamp >= " + LastUpdateSFFormat + " and (" + TypeString + ")" ' and " + TypeString
                                                    Log.Write("SF Query String = " + SFQuery)

                                                    qr = Binding.query(SFQuery)
                                                    Log.Write("qr.size = " + qr.size.ToString)
                                                    HasAddress = False 'Set this to false so we can set local variables correctly
                                                    BindingRanOK = True
                                                    hasFax = False
                                                End Try
                                            End Try
                                        End Try
                                    End Try
                                End Try
                            Catch ex As Exception
                                'OK, we have a major error binding the query.
                                BindingRanOK = False
                                Log.Write("Error when attempting to bind the Query: " + ex.Message)
                                SyncError = True
                            End Try


                            If (BindingRanOK) And (qr.size > 0) Then
                                'We now have a list of all accounts that have been modified in the last 3 hours and that match our account types. Let's update.
                                While Not done

                                    Log.Write("Begining While Loop to get companies.  qr.records.length = " + qr.records.Length.ToString)
                                    Log.Write("Updating company information, qr.size = " + qr.size.ToString)


                                    For i As Integer = 0 To qr.records.Length - 1
                                        Dim thisCompany As New CompanyData()
                                        Log.Write("In for loop iteration " + i.ToString)
                                        Dim LastModifiedDateTime As DateTime

                                        Dim records As sObject() = qr.records
                                        Dim contact As sObject = records(i)

                                        With thisCompany
                                            .AccountID = records(i).Any(0).InnerText
                                            .AccountName = records(i).Any(1).InnerText

                                            If HasAddress Then
                                                If records(i).Any(3).InnerText = String.Empty AndAlso queriedShippingAddress Then
                                                    Dim billingAddressWithFax As Boolean = False
                                                    Dim billingAddress As sObject() = GetBillingAddress(.AccountID, billingAddressWithFax)
                                                    If billingAddress IsNot Nothing Then
                                                        .Street   = billingAddress(0).Any(0).InnerText
                                                        .City     = billingAddress(0).Any(1).InnerText
                                                        .State    = billingAddress(0).Any(2).InnerText
                                                        .Zip      = billingAddress(0).Any(3).InnerText
                                                        .Country  = billingAddress(0).Any(4).InnerText
                                                        .Phone    = billingAddress(0).Any(5).InnerText
                                                        If billingAddressWithFax Then
                                                            .Fax  = billingAddress(0).Any(6).InnerText
                                                        End If
                                                    End If
                                                Else
                                                    .Street   = records(i).Any(3).InnerText
                                                    .City     = records(i).Any(4).InnerText
                                                    .State    = records(i).Any(5).InnerText
                                                    .Zip      = records(i).Any(6).InnerText
                                                    .Country  = records(i).Any(7).InnerText
                                                    .Phone    = records(i).Any(8).InnerText
                                                    If hasFax Then
                                                        .Fax  = records(i).Any(11).InnerText
                                                    End If
                                                End If
                                                LastModifiedDateTime = Date.Parse(records(i).Any(9).InnerText)
                                            Else
                                                .Phone = records(i).Any(3).InnerText
                                                If hasFax Then
                                                    .Fax = records(i).Any(6).InnerText
                                                End If
                                                LastModifiedDateTime = Date.Parse(records(i).Any(4).InnerText)

                                            End If
                                        End With

                                        Log.Write("Company " & thisCompany.AccountName & " last modified on " & LastModifiedDateTime.ToString())

                                        UpdateOrgInfo(thisCompany, ParentOrgID)
                                        Log.Write("Completed AddOrUpdateAccountInformation for company " + thisCompany.AccountName)

                                        'Let's force an update of contact information for this company
                                        GetContactInformation(ParentOrgID, LastUpdateSFFormat, AccountTypeString, thisCompany.AccountID, True)

                                        Log.Write("Completed force update contact info for " + thisCompany.AccountName)

                                    Next
                                    If qr.done Then
                                        done = True
                                    Else
                                        Log.Write("Requesting more records (should be more than 2000 companies?)")
                                        qr = Binding.queryMore(qr.queryLocator)
                                    End If
                                End While

                                'check for existence of custom fields to sync
                                GetCustomFields("Account", TypeString, LastUpdateSFFormat)

                                Log.Write("All done updating company information.")

                            Else
                                Log.Write("**No matching record found!!")
                            End If

                            If BindingRanOK Then
                                'Breaking this out separately since they were not running if there were no companies that had changed.

                                'We updated all of the ACCOUNT information above
                                'Now let's update all of the contact information
                                GetContactInformation(ParentOrgID, LastUpdateSFFormat, AccountTypeString, "0", False)


                                'Code for Axceler to update their product and license information
                                If ParentOrgID = 305383 Then
                                    GetProductAndLicenseInfo(LastUpdateSFFormat)

                                End If
                            End If
                        End If
                    Catch ex As Exception
                        ErrorCode = IntegrationError.Unknown
                        Log.Write("**Error:  " + ex.Message)
                        Return False
                    End Try


                    'Added to see if it solved problem with 2+ SalesForce accounts being active.
                    Binding.logout()
                    Binding.logoutAsync()

                    Return Not SyncError
                Else
                    If LoginReturn.ToLower() = "password expired" Or LoginReturn.ToLower().Contains("invalid_login") Then
                        ErrorCode = IntegrationError.InvalidLogin
                    End If

                    Log.Write("Login failed: " & LoginReturn)
                    Return False
                End If

            End Function

            Private Function GetBillingAddress(ByVal organizationId As String, ByRef hasFax As Boolean) As sObject()
                Dim SFQuery As String = Nothing
                Dim qr As QueryResult = Nothing
                Try
                    'First try with Fax
                    Log.Write("Attempting to find Billing Address with fax of OrganizationId: " & organizationId.ToString() & ".")
                    SFQuery = "select BillingStreet, BillingCity, BillingState, BillingPostalCode, BillingCountry, Phone, Fax from Account where ID = '" + organizationId.ToString() + "'"
                    Log.Write("SF Query String = " + SFQuery)

                    qr = Binding.query(SFQuery)
                    Log.Write("qr.size = " + qr.size.ToString)
                    hasFax = True
                Catch ex1 As Exception
                    Try
                        'Try without fax
                        Log.Write("Attempting to find Billing Address without fax of OrganizationId: " & organizationId.ToString() & ".")
                        SFQuery = "select BillingStreet, BillingCity, BillingState, BillingPostalCode, BillingCountry, Phone from Account where ID = '" + organizationId.ToString() + "'"
                        Log.Write("SF Query String = " + SFQuery)

                        qr = Binding.query(SFQuery)
                        Log.Write("qr.size = " + qr.size.ToString)
                        hasFax = False
                    Catch ex2 As Exception
                        Log.Write("Error when attempting to bind the Query: " + ex2.Message)                        
                    End Try
                End Try

                Dim result As sObject() = Nothing
                If qr IsNot Nothing AndAlso qr.size > 0 Then
                    result = qr.records
                End If

                Return result
            End Function

            Private Function SendSFTicketData() As Boolean
                Dim Success As Boolean = True

                If login(Trim(CRMLinkRow.Username), Trim(CRMLinkRow.Password), Trim(CRMLinkRow.SecurityToken)) = "OK" Then
                    Success = SendTicketData(AddressOf CreateNote)

                    Binding.logout()
                    Binding.logoutAsync()
                End If

                Return Success
            End Function


            Private Function login(ByVal username As String, ByVal password As String, ByVal securitytoken As String) As String
                'Set the partner WSDL

                Dim co As New CallOptions()
                co.client = "Muroc/TSCom/" '**this is our unique TS address
                Binding.CallOptionsValue = co

                ' Timeout after a minute
                Binding.Timeout = 60000

                ' Try logging in
                Dim lr As LoginResult
                Try
                    lr = Binding.login(username, password + securitytoken)
                Catch e As SoapException
                    Return e.Message

                Catch e As Exception
                    Return e.Message
                End Try

                ' Check if the password has expired
                If lr.passwordExpired Then
                    Return "Password expired"
                End If

                '* Once the client application has logged in successfully, it will use
                '             * the results of the login call to reset the endpoint of the service
                '             * to the virtual server instance that is servicing your organization
                '             

                Binding.Url = lr.serverUrl

                '* The sample client application now has an instance of the SforceService
                '             * that is pointing to the correct endpoint. Next, the sample client
                '             * application sets a persistent SOAP header (to be included on all
                '             * subsequent calls that are made with SforceService) that contains the
                '             * valid sessionId for our login credentials. To do this, the sample
                '             * client application creates a new SessionHeader object and persist it to
                '             * the SforceService. Add the session ID returned from the login to the
                '             * session header
                '             

                Binding.SessionHeaderValue = New SessionHeader()
                Binding.SessionHeaderValue.sessionId = lr.sessionId

                ' Return true to indicate that we are logged in, pointed
                ' at the right URL and have our security token in place.
                Return "OK"
            End Function


            Private Sub GetContactInformation(ByVal ParentOrgID As String, ByVal LastUpdate As String, ByVal TypeString As String, ByVal AccountIDToUpdate As String, ByVal ForceUpdate As Boolean)
                'This will get the contact information from SalesForce for a given account ID
                'If ForceUpdate is set them we will change the query so that we grab all contact information from the AccountID company (otherwise AccountID is not used)

                Log.Write("Getting Contact Information...")

                'The results will be placed in qr
                Dim qr As QueryResult = Nothing

                'We are going to increase our return batch size to 250 items
                'Setting is a recommendation only, different batch sizes may
                'be returned depending on data, to keep performance optimized.
                Binding.QueryOptionsValue = New QueryOptions()
                Binding.QueryOptionsValue.batchSize = 250
                Binding.QueryOptionsValue.batchSizeSpecified = True

                Dim LastModifiedDateTime As DateTime

                Try
                    Dim HasFax As Boolean = True
                    Dim done As Boolean = False

                    Try

                        If Not ForceUpdate Then 'this the normal query
                            'This should give us a list of all the contact that in our account group that have been modified since the last time (- a few hours) we ran this
                            qr = Binding.query("select email, FirstName, LastName, Phone, MobilePhone, Fax, Title, IsDeleted, SystemModstamp, Account.ID from Contact where SystemModStamp >= " + LastUpdate + " and (" + TypeString + ")")
                        Else
                            Log.Write("Using force update query")
                            qr = Binding.query("select email, FirstName, LastName, Phone, MobilePhone, Fax, Title, IsDeleted, SystemModstamp, Account.ID from Contact where Account.ID = '" + AccountIDToUpdate + "'")
                        End If

                        done = False
                        HasFax = True

                        Log.Write("Found " + qr.size.ToString + " contact records.")
                    Catch ex As Exception
                        'Uh Oh - Error.  Probably no fax number, so we'll try without the fax number
                        Log.Write("Error getting contact - Trying without fax number.")
                        Log.Write(ex.Message & " " & ex.StackTrace)
                        'SystemModstamp or LastModifiedDate
                        'Note - Added second phone just so we have something in that space..
                        If Not ForceUpdate Then
                            qr = Binding.query("select email, FirstName, LastName, Phone, MobilePhone, Phone, Title, IsDeleted, SystemModstamp, Account.ID from Contact where SystemModStamp >= " + LastUpdate + " and (" + TypeString + ")")
                        Else
                            qr = Binding.query("select email, FirstName, LastName, Phone, MobilePhone, Phone, Title, IsDeleted, SystemModstamp, Account.ID from Contact where Account.ID = '" + AccountIDToUpdate + "'")
                        End If

                        done = False

                        Log.Write("Found " + qr.size.ToString + " contact records (no fax).")
                        HasFax = False

                    End Try

                    If qr.size > 0 Then
                        While Not done
                            For i As Integer = 0 To qr.records.Length - 1
                                Dim thisPerson As New EmployeeData()
                                Dim AccountID As String

                                Dim records As sObject() = qr.records

                                With thisPerson
                                    .Email = records(i).Any(0).InnerText
                                    .FirstName = records(i).Any(1).InnerText
                                    .LastName = records(i).Any(2).InnerText
                                    .Phone = records(i).Any(3).InnerText
                                    .Cell = records(i).Any(4).InnerText

                                    If HasFax Then
                                        .Fax = records(i).Any(5).InnerText
                                        .Title = records(i).Any(6).InnerText
                                    Else
                                        .Title = records(i).Any(5).InnerText
                                    End If

                                    AccountID = records(i).Any(9).InnerText
                                    If AccountID.Length > 18 Then
                                        'I have no idea why, but SF returns the ID here as something like "Account0018000000eM3oOAAS0018000000eM3oOAAS" instead of the standard 18 character account id
                                        'This will return just the final 18 characters which should work.
                                        AccountID = AccountID.Substring(AccountID.Length - 18)
                                    End If
                                End With


                                If HasFax Then
                                    LastModifiedDateTime = Date.Parse(records(i).Any(8).InnerText)
                                Else
                                    LastModifiedDateTime = Date.Parse(records(i).Any(7).InnerText)
                                End If

                                UpdateContactInfo(thisPerson, AccountID, ParentOrgID)

                            Next
                            If qr.done Then
                                done = True
                            Else
                                qr = Binding.queryMore(qr.queryLocator)
                            End If
                        End While

                        'check for existence of custom fields to sync
                        GetCustomFields("Contact", TypeString, LastUpdate)

                    Else
                        Log.Write("No records found.")
                    End If


                    Log.Write("All done with contact records!")

                Catch ex As Exception
                    Log.Write("Failed to execute query successfully, error message was: " & ex.Message)
                    ErrorCode = IntegrationError.Unknown
                    SyncError = True
                End Try
            End Sub


            Private Function CreateNote(ByVal accountid As String, ByVal thisTicket As Ticket) As Boolean
                Dim Success As Boolean = True

                Dim Title = "Support Issue: " & thisTicket.Name
                Dim description As Action = Actions.GetTicketDescription(User, thisTicket.TicketID)
                Dim NoteBody As String = String.Format("A new support ticket has been created for this account entitled ""{0}"".{3}{2}{3}Click here to access the ticket information: https://app.teamsupport.com/Ticket.aspx?ticketid={1}", _
                                                    thisTicket.Name, thisTicket.TicketID.ToString(), HtmlUtility.StripHTML(description.Description), Environment.NewLine)

                Try

                    'Attach a note, which will get re-parented
                    Dim note As sForce.sObject = New sObject()
                    note.type = "Note"
                    note.Any = New System.Xml.XmlElement() {GetNewXmlElement("ParentId", accountid), _
                        GetNewXmlElement("Body", NoteBody), _
                        GetNewXmlElement("Title", Title)}

                    Dim noteSave As SaveResult = Binding.create(New sObject() {note})(0)

                    Success = noteSave.success
                Catch ex As Exception
                    Log.Write("Error in CreateNote: " & ex.Message)
                    ErrorCode = IntegrationError.Unknown

                    Success = False
                End Try

                Return Success
            End Function

            Private Function GetNewXmlElement(ByVal Name As String, ByVal nodeValue As String) As System.Xml.XmlElement
                Dim doc As System.Xml.XmlDocument = New System.Xml.XmlDocument
                Dim xmlel As System.Xml.XmlElement = doc.CreateElement(Name)
                xmlel.InnerText = nodeValue
                Return xmlel
            End Function

            Private Sub GetProductAndLicenseInfo(ByVal SFLastUpdateTime As String)
                'This is *** CUSTOM CODE *** for Axceler to see if we can get their license and product information


                Log.Write("In GetProductAndLicenseInfo routine.")

                'The results will be placed in qr
                Dim qr As QueryResult = Nothing

                'We are going to increase our return batch size to 250 items
                'Setting is a recommendation only, different batch sizes may
                'be returned depending on data, to keep performance optimized.
                Binding.QueryOptionsValue = New QueryOptions()
                Binding.QueryOptionsValue.batchSize = 250
                Binding.QueryOptionsValue.batchSizeSpecified = True

                Try
                    Dim done As Boolean = False

                    Dim thisSettings As New ServiceLibrary.Settings(User, Processor.ServiceName)

                    Try
                        Dim queryString As String
                        If thisSettings.ReadBool("ResyncAllAxcelerLicenses", True) Then
                            queryString = "select Product__c, Expiration_Date__c, License_type__c, Status__c, Account__c from License__c ORDER BY Product__c"
                            Log.Write("Resyncing all products.")
                        Else
                            queryString = "select Product__c, Expiration_Date__c, License_type__c, Status__c, Account__c from License__c where SystemModStamp >= " + SFLastUpdateTime + " ORDER BY Product__c" 'added order by 3/25/11
                        End If

                        qr = Binding.query(queryString)

                        done = False

                        Log.Write("Found " + qr.size.ToString + " license records.")
                    Catch ex As Exception
                        'Problem - Not sure what.
                        done = True

                        Log.Write("Error in GetProductAndLicenseInfo. " + ex.Message)
                        SyncError = True
                    End Try

                    If qr.size > 0 Then
                        While Not done

                            For i As Integer = 0 To qr.records.Length - 1

                                Try
                                    Dim ProductName, LicenseType, LicenseStatus, AccountID As String
                                    Dim ExpirationDate As Date? = Nothing
                                    Dim tempExpiration As Date

                                    Log.Write("In GetProductAndLicenseInfo routine - i=" + i.ToString)

                                    Dim records As sObject() = qr.records
                                    Dim Products As sObject = records(i)

                                    ProductName = records(i).Any(0).InnerText

                                    If Date.TryParse(records(i).Any(1).InnerText, tempExpiration) Then
                                        ExpirationDate = tempExpiration
                                    Else
                                        ExpirationDate = Nothing
                                        Log.Write("failed to parse expiration date value: " & records(i).Any(1).InnerText)
                                    End If

                                    LicenseType = records(i).Any(2).InnerText
                                    LicenseStatus = records(i).Any(3).InnerText
                                    AccountID = records(i).Any(4).InnerText

                                    ' 1) See if we can match a product in TS with the product name
                                    Dim thisProduct As Product
                                    Dim findProduct As New Products(User)
                                    findProduct.LoadByOrganizationID(CRMLinkRow.OrganizationID)

                                    thisProduct = findProduct.FindByName(ProductName)

                                    If thisProduct IsNot Nothing Then
                                        Log.Write(String.Format("ProductName = {0}, ExpirationDate = {1}, ProductID = {2}, AccountID = {3}", _
                                                                ProductName, IIf(ExpirationDate IsNot Nothing, ExpirationDate.ToString(), ""), thisProduct.ProductID.ToString(), AccountID))

                                        ' 2) If we can, lets see if the product ID is assigned to this customer
                                        Dim findCompany As New Organizations(User)
                                        Dim thisCompany As Organization

                                        'make sure the company already exists
                                        findCompany.LoadByCRMLinkID(AccountID, CRMLinkRow.OrganizationID)

                                        If findCompany.Count > 0 Then
                                            thisCompany = findCompany(0)
                                            Dim findOrgProd As New OrganizationProducts(User)
                                            Dim thisOrgProd As OrganizationProduct

                                            Log.Write("Found product for " & thisCompany.Name)

                                            findOrgProd.LoadByOrganizationAndProductID(thisCompany.OrganizationID, thisProduct.ProductID)

                                            If findOrgProd.Count > 0 Then
                                                thisOrgProd = findOrgProd(0)
                                                'The company already has the product associated with them.
                                                ' We now just need to update the waranty expiration date 
                                                '  Note that the waranty expiration date is a custom field just for this customer...
                                                '      3) Update the waranty expiration date (this should be a custom field on product)
                                                '         Update CustomValue set CustomValue={ExpirationDate} where CustomFieldID = 3761 and RefID={OrganizationProductID}

                                                If ExpirationDate IsNot Nothing AndAlso ExpirationDate > thisOrgProd.SupportExpiration Then 'test to see if we are using the most up to date expiration date (only use product/expiration date that is the most recent)
                                                    thisOrgProd.SupportExpiration = ExpirationDate
                                                    thisOrgProd.Collection.Save()

                                                    'License Type - CustomFieldID is 3770 (test value 101)
                                                    UpdateCustomValue(3770, thisOrgProd.OrganizationProductID, LicenseType)

                                                    'License Status - CustomFieldID is 3771 (test value 102)
                                                    UpdateCustomValue(3771, thisOrgProd.OrganizationProductID, LicenseStatus)

                                                    Log.Write("Product updated.")
                                                Else
                                                    Log.Write("Date information not updated since there is a later expiration date.")
                                                End If

                                            Else
                                                'We need to add the product to this company
                                                thisOrgProd = (New OrganizationProducts(User)).AddNewOrganizationProduct()
                                                thisOrgProd.OrganizationID = thisCompany.OrganizationID
                                                thisOrgProd.ProductID = thisProduct.ProductID

                                                'Product added, lets add/update the expiration date
                                                thisOrgProd.SupportExpiration = ExpirationDate
                                                thisOrgProd.Collection.Save()

                                                'License Type - CustomFieldID is 3770 (test value 101)
                                                UpdateCustomValue(3770, thisOrgProd.OrganizationProductID, LicenseType)

                                                'License Status - CustomFieldID is 3771 (test value 102)
                                                UpdateCustomValue(3771, thisOrgProd.OrganizationProductID, LicenseStatus)
                                                Log.Write("Product added.")
                                            End If

                                        Else
                                            Log.Write("Product not updated because company does not exist.")
                                        End If

                                    End If

                                Catch ex As Exception
                                    Log.Write("Error updating product " & i.ToString() & ": " & ex.Message & " " & ex.StackTrace)
                                    SyncError = True
                                End Try

                            Next

                            done = qr.done

                            If Not done Then
                                qr = Binding.queryMore(qr.queryLocator)
                            End If

                        End While
                    Else
                        Log.Write("No records found.")
                    End If


                Catch ex As Exception
                    Log.Write("Error in GetProductAndLicenseInfo : Failed to execute succesfully, error message was: " & ex.Message & " " & ex.StackTrace)
                    SyncError = True
                End Try
            End Sub


            Private Sub GetCustomFields(ByVal objType As String, ByVal TypeString As String, ByVal LastUpdate As String)
                Try
                    Dim customFieldList As String = Nothing
                    Dim theseFields As New CRMLinkFields(User)

                    theseFields.LoadByObjectType(objType, CRMLinkRow.CRMLinkID)

                    If theseFields.Count > 0 Then
                        Dim objDescription = Binding.describeSObject(objType)

                        For Each cField As CRMLinkField In theseFields
                            If (
                                    objType = "Account" AndAlso cField.CRMFieldName.Trim().ToLower() <> "id"
                                ) OrElse (
                                    objType = "Contact" AndAlso cField.CRMFieldName.Trim().ToLower() <> "email"
                            ) Then
                                For Each apiField As Field In objDescription.fields
                                    If apiField.name.Trim().ToLower() = cField.CRMFieldName.Trim().ToLower() Then
                                        If customFieldList Is Nothing Then
                                            customFieldList = apiField.name
                                        'Any duplicate will raise an exception.
                                        ElseIf Not customFieldList.Contains(apiField.name) Then
                                            customFieldList &= ", " & apiField.name
                                        Else
                                            Log.Write("Custom field " & apiField.name & " is mapped more than one time.")
                                        End If
                                        'No need to continue after the field has been added.
                                        Exit For
                                    End If
                                Next
                            End If
                        Next
                    End If

                    If customFieldList IsNot Nothing Then
                        Dim customQuery As String = "select Account.ID, " & IIf(objType = "Account", "", "email, ") & customFieldList & " from " & objType & " where SystemModStamp >= " + LastUpdate + " and (" + TypeString + ")"

                        Log.Write(customQuery)

                        Dim qr As QueryResult = Binding.query(customQuery)

                        Log.Write(qr.size.ToString() & " records with custom fields to sync.")

                        If qr.size > 0 Then
                            For Each record As sObject In qr.records
                                Dim accountID As String

                                'find the object in OUR system
                                If objType = "Account" Then
                                    accountID = record.Id

                                ElseIf objType = "Contact" Then
                                    accountID = Array.Find(record.Any, Function(x As Xml.XmlElement) x.LocalName = "Account")("sf:Id").InnerText

                                Else
                                    Return
                                End If

                                Dim findAccount As New Organizations(User)
                                findAccount.LoadByCRMLinkID(accountID, CRMLinkRow.OrganizationID)

                                If findAccount.Count > 0 Then
                                    Dim thisAccount As Organization = findAccount(0)

                                    'update fields
                                    If objType = "Account" Then
                                        For Each thisField As Xml.XmlElement In record.Any
                                            Dim thisMapping As CRMLinkField = theseFields.FindByCRMFieldName(thisField.LocalName)

                                            If thisMapping IsNot Nothing Then
                                                Try
                                                    If thisMapping.CustomFieldID IsNot Nothing Then
                                                        Dim translatedFieldValue As String = TranslateFieldValue(thisMapping.CustomFieldID, thisAccount.OrganizationID, thisField.InnerText)
                                                        UpdateCustomValue(thisMapping.CustomFieldID, thisAccount.OrganizationID, translatedFieldValue)

                                                    ElseIf thisMapping.TSFieldName IsNot Nothing Then
                                                        thisAccount.Row(thisMapping.TSFieldName) = TranslateFieldValue(thisField.InnerText, thisAccount.Row(thisMapping.TSFieldName).GetType().Name)
                                                        thisAccount.BaseCollection.Save()
                                                    End If
                                                Catch mappingException As Exception
                                                    Log.Write(
                                                      "The following exception was caught mapping the account field """ &
                                                      thisField.LocalName &
                                                      """ with """ &
                                                      thisMapping.TSFieldName &
                                                      """: " &
                                                      mappingException.Message)
                                                End Try
                                            End If
                                        Next

                                    Else 'if it's not an account, it's a contact (otherwise we would have returned above)
                                        Dim email As String = Array.Find(record.Any, Function(x As Xml.XmlElement) x.LocalName.ToLower() = "email").InnerText

                                        Dim findContact As New Users(User)
                                        Dim thisContact As User = Nothing

                                        findContact.LoadByOrganizationID(thisAccount.OrganizationID, False)
                                        thisContact = findContact.FindByEmail(email)

                                        If thisContact IsNot Nothing Then

                                            For Each thisField As Xml.XmlElement In record.Any
                                                Dim thisMapping As CRMLinkField = theseFields.FindByCRMFieldName(thisField.LocalName)

                                                If thisMapping IsNot Nothing Then
                                                    Try
                                                        If thisMapping.CustomFieldID IsNot Nothing Then
                                                            Dim translatedFieldValue As String = TranslateFieldValue(thisMapping.CustomFieldID, thisContact.UserID, thisField.InnerText)
                                                            UpdateCustomValue(thisMapping.CustomFieldID, thisContact.UserID, translatedFieldValue)

                                                        ElseIf thisMapping.TSFieldName IsNot Nothing Then
                                                            thisContact.Row(thisMapping.TSFieldName) = TranslateFieldValue(thisField.InnerText, thisContact.Row(thisMapping.TSFieldName).GetType().Name)
                                                            thisContact.BaseCollection.Save()
                                                        End If
                                                    Catch mappingException As Exception
                                                        Log.Write(
                                                          "The following was exception caught mapping the contact field """ &
                                                          thisField.LocalName &
                                                          """ with """ &
                                                          thisMapping.TSFieldName &
                                                          """: " &
                                                          mappingException.Message)
                                                    End Try
                                                End If

                                            Next

                                        End If

                                    End If
                                End If

                            Next
                        End If
                    End If

                Catch ex As Exception
                    Log.Write("Exception caught in GetCustomFields: " & ex.Message)
                    Log.Write(ex.StackTrace)
                    SyncError = True
                End Try
            End Sub

            Private Overloads Function TranslateFieldValue(ByVal salesForceValue As String, ByVal teamSupportTypeName As String) As String
                Dim teamSupportValue As String

                Select Case teamSupportTypeName.ToLower()
                    Case "boolean"
                        teamSupportValue = TranslateBooleanFieldValue(salesForceValue)
                    Case "datetime"
                        teamSupportValue = TranslateDateTimeFieldValue(salesForceValue)
                    Case Else
                        teamSupportValue = salesForceValue
                End Select

                Return teamSupportValue
            End Function

            Private Overloads Function TranslateFieldValue(ByVal customFieldID As Integer, ByVal RefID As Integer, ByVal salesForceValue As String) As String
                Dim result As String

                Dim findCustom As New CustomValues(User)
                findCustom.LoadByFieldID(customFieldID, RefID)
                If findCustom.Count > 0 Then
                    result = TranslateFieldValue(salesForceValue, findCustom(0).FieldType.ToString())
                Else
                    result = salesForceValue
                End If

                Return result
            End Function

            Private Function TranslateBooleanFieldValue(ByVal salesForceValue As String) As String
                Dim teamSupportValue As String

                Dim salesForceComparableValue As String = salesForceValue.Trim().ToLower()

                If salesForceComparableValue = "yes" OrElse salesForceComparableValue = "1" Then
                    teamSupportValue = "true"
                ElseIf salesForceComparableValue = "no" OrElse salesForceComparableValue = "0" Then
                    teamSupportValue = "false"
                Else
                    teamSupportValue = salesForceValue
                End If

                Return teamSupportValue
            End Function

            Private Function TranslateDateTimeFieldValue(ByVal salesForceValue As String) As String
                Dim result As String

                Try
                    result = Convert.ToDateTime(salesForceValue).ToString()
                Catch ex As Exception
                    result = salesForceValue
                End Try

                Return result
            End Function

            Protected Overrides Sub Finalize()
                MyBase.Finalize()
            End Sub

        End Class

    End Namespace
End Namespace