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

                    Dim LastModifiedDateTime As DateTime
                    Dim LastUpdateSFFormat As String 'format for SF query for time is 2011-01-26T16:57:00.000Z  ('yyyy'-'MM'-'dd'T'HH': 'mm': 'ss.fffffff'Z' )
                    Dim TempTime As DateTime
                    TempTime = Date.Parse(CRMLinkRow.LastLink)
                    TempTime = DateAdd(DateInterval.Hour, -1, TempTime) 'push last update time back three hours to make sure we catch every change
                    LastUpdateSFFormat = TempTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fff'Z'")


                    'SalesForce returns the LastModifed time in the Local Timezone of the organization.  Therefor, we need to store the LastUpdate
                    ' time as UTC and convert it to the local organization's TZ so they both match.
                    '**If the TS org's TZ does not match SF's TZ then we're in a world of hurt.

                    Log.Write("LastUpdate (last time CRM Sync Ran, in the organization's local timezone) = " + CRMLinkRow.LastLink.ToString())

                    Try

                        'This code should let us support multiple account types separated by a comma
                        Dim TypeString As String = ""
                        Dim AccountTypeString As String = ""
                        Dim MatchArray As String() = TagsToMatch.Split(",")
                        For z As Integer = 0 To MatchArray.Length - 1
                            If z > 0 Then
                                TypeString = TypeString + " or "
                                AccountTypeString = AccountTypeString + " or "
                            End If

                            TypeString = TypeString + " type like '%" + Trim(MatchArray(z)) + "%'"
                            AccountTypeString = AccountTypeString + " Account.Type like '%" + Trim(MatchArray(z)) + "%'"
                        Next

                        Log.Write("TypeString = " + TypeString)

                        Dim SFQuery As String
                        Dim done As Boolean = False
                        Dim HasAddress As Boolean = True

                        Dim BindingRanOK As Boolean
                        Try
                            Try
                                'OK, lets try this with the shipping addresses
                                SFQuery = "select ID, Name, type, ShippingStreet, ShippingCity, ShippingState, ShippingPostalCode, ShippingCountry, Phone, LastModifiedDate, SystemModStamp from Account where SystemModStamp >= " + LastUpdateSFFormat + " and (" + TypeString + ")"
                                Log.Write("SF Query String = " + SFQuery)

                                qr = Binding.query(SFQuery)
                                Log.Write("qr.size = " + qr.size.ToString)
                                BindingRanOK = True
                            Catch ex As Exception
                                'Hmm...No shipping addresses.  Let's try billing addresses
                                'This doesn't appear to work either...? 7/1/10
                                Try
                                    Log.Write("Shipping Address information not found - Attempting to find Billing Address information instead.")
                                    'SFQuery = "select ID, Name, type, BillingStreet, BillingCity, BillingState, BillingPostalCode, BillingCountry, Phone, LastModifiedDate from Account where " + TypeString + " and LastModifiedDate >= 2012-01-07T20:27:18.000Z"
                                    SFQuery = "select ID, Name, type, BillingStreet, BillingCity, BillingState, BillingPostalCode, BillingCountry, Phone, LastModifiedDate, SystemModStamp from Account where SystemModStamp >= " + LastUpdateSFFormat + " and (" + TypeString + ")" ' and " + TypeString
                                    Log.Write("SF Query String = " + SFQuery)

                                    qr = Binding.query(SFQuery)
                                    Log.Write("qr.size = " + qr.size.ToString)
                                    BindingRanOK = True
                                Catch ex2 As Exception
                                    'Well crap - No billing address either.
                                    Log.Write("Billing Address information not found - Attempting to process company with no address info at all.")
                                    'SFQuery = "select ID, Name, type, Phone, LastModifiedDate from Account where " + TypeString
                                    SFQuery = "select ID, Name, type,Phone, LastModifiedDate, SystemModStamp from Account where SystemModStamp >= " + LastUpdateSFFormat + " and (" + TypeString + ")" ' and " + TypeString
                                    Log.Write("SF Query String = " + SFQuery)

                                    qr = Binding.query(SFQuery)
                                    Log.Write("qr.size = " + qr.size.ToString)
                                    HasAddress = False 'Set this to false so we can set local variables correctly
                                    BindingRanOK = True
                                End Try
                            End Try
                        Catch ex As Exception
                            'OK, we have a major error binding the query.
                            BindingRanOK = False
                            Log.Write("Error when attempting to bind the Query: " + ex.Message)
                        End Try


                        If (BindingRanOK) And (qr.size > 0) Then
                            'We now have a list of all accounts that have been modified in the last 3 hours and that match our account types. Let's update.
                            While Not done

                                Log.Write("Begining While Loop to get companies.  qr.records.length = " + qr.records.Length.ToString)
                                Log.Write("Updating company information, qr.size = " + qr.size.ToString)


                                For i As Integer = 0 To qr.records.Length - 1
                                    Dim thisCompany As New CompanyData()
                                    Log.Write("In for loop iteration " + i.ToString)

                                    Dim records As sObject() = qr.records
                                    Dim contact As sObject = records(i)

                                    With thisCompany
                                        .AccountID = records(i).Any(0).InnerText
                                        .AccountName = records(i).Any(1).InnerText

                                        If HasAddress Then
                                            .Street = records(i).Any(3).InnerText
                                            .City = records(i).Any(4).InnerText
                                            .State = records(i).Any(5).InnerText
                                            .Zip = records(i).Any(6).InnerText
                                            .Country = records(i).Any(7).InnerText
                                            .Phone = records(i).Any(8).InnerText
                                            LastModifiedDateTime = Date.Parse(records(i).Any(9).InnerText)
                                        Else
                                            .Phone = records(i).Any(3).InnerText
                                            LastModifiedDateTime = Date.Parse(records(i).Any(4).InnerText)

                                        End If
                                    End With

                                    Log.Write("Company name=" + thisCompany.AccountName)
                                    Log.Write("Record Last Modified in SF= " + LastModifiedDateTime.ToString)

                                    Log.Write("Company " + thisCompany.AccountName + " has been modified.")

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
                    Catch ex As Exception
                        LogSyncResult("Error in ProcessSalesForceAccountInformation: " + ex.ToString)
                        Log.Write("**Error:  " + ex.Message)
                        Return False
                    End Try


                    'Added to see if it solved problem with 2+ SalesForce accounts being active.
                    Binding.logout()
                    Binding.logoutAsync()

                    Return True
                Else
                    Return False
                End If

            End Function


            Public Overrides Function SendTicketData() As Boolean
                If CRMLinkRow.SendBackTicketData Then

                    If login(Trim(CRMLinkRow.Username), Trim(CRMLinkRow.Password), Trim(CRMLinkRow.SecurityToken)) = "OK" Then

                        'get tickets created since last ticket synced
                        Dim tickets As New Tickets(User)
                        tickets.LoadByCRMLinkItem(CRMLinkRow)

                        If tickets IsNot Nothing Then
                            Log.Write(String.Format("Found {0} tickets to sync.", tickets.Count.ToString()))

                            For Each thisTicket As Ticket In tickets
                                If Processor.IsStopped Then
                                    Return False
                                End If

                                Dim description As Action = Actions.GetTicketDescription(User, thisTicket.TicketID)
                                Dim customers As New OrganizationsView(User)
                                customers.LoadByTicketID(thisTicket.TicketID)

                                'Add the new tickets to the company record
                                Dim NoteBody As String = String.Format("A new support ticket has been created for this account entitled ""{0}"".{3}{2}{3}Click here to access the ticket information: https://app.teamsupport.com/Ticket.aspx?ticketid={1}", _
                                                                 thisTicket.Name, thisTicket.TicketID.ToString(), Utilities.StripHTML(description.Description), Environment.NewLine)


                                For Each customer As OrganizationsViewItem In customers
                                    If customer.CRMLinkID <> "" Then
                                        Log.Write("Creating a note...")

                                        If CreateNote(customer.CRMLinkID, "Support Issue: " & thisTicket.Name, NoteBody, CRMLinkRow.OrganizationID) Then
                                            Log.Write("Note created successfully.")

                                            CRMLinkRow.LastTicketID = thisTicket.TicketID
                                            CRMLinkRow.Collection.Save()
                                        End If
                                    End If
                                Next

                            Next
                        Else
                            Log.Write("No new tickets to sync.")
                        End If

                        Binding.logout()
                        Binding.logoutAsync()
                    End If

                Else
                    Log.Write("SendBackTicketData set to FALSE for this organization.")
                End If

                Return True
            End Function



            Public Function login(ByVal username As String, ByVal password As String, ByVal securitytoken As String) As String
                'Set the partner WSDL

                Dim co As New CallOptions()
                co.client = "Muroc/TSCom/" '**this is our unique TS address
                Binding.CallOptionsValue = co

                ' Timeout after a minute
                binding.Timeout = 60000

                ' Try logging in
                Dim lr As LoginResult
                Try
                    lr = binding.login(username, password + securitytoken)
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

                binding.SessionHeaderValue = New SessionHeader()
                binding.SessionHeaderValue.sessionId = lr.sessionId

                ' Return true to indicate that we are logged in, pointed
                ' at the right URL and have our security token in place.
                Return "OK"
            End Function


            Public Sub GetContactInformation(ByVal ParentOrgID As String, ByVal LastUpdate As String, ByVal TypeString As String, ByVal AccountIDToUpdate As String, ByVal ForceUpdate As Boolean)
                'This will get the contact information from SalesForce for a given account ID
                'If ForceUpdate is set them we will change the query so that we grab all contact information from the AccountID company (otherwise AccountID is not used)

                Log.Write("Getting Contact Information...")

                'The results will be placed in qr
                Dim qr As QueryResult = Nothing

                'We are going to increase our return batch size to 250 items
                'Setting is a recommendation only, different batch sizes may
                'be returned depending on data, to keep performance optimized.
                binding.QueryOptionsValue = New QueryOptions()
                binding.QueryOptionsValue.batchSize = 250
                binding.QueryOptionsValue.batchSizeSpecified = True

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
                        'SystemModstamp or LastModifiedDate
                        If Not ForceUpdate Then
                            qr = Binding.query("select email, FirstName, LastName, Phone, MobilePhone, Phone, Title, IsDeleted, SystemModstamp, Account.ID from Contact where SystemModStamp >= " + LastUpdate + " and (" + TypeString + ")")
                        Else
                            qr = Binding.query("select email, FirstName, LastName, Phone, MobilePhone, Phone, Title, IsDeleted, SystemModstamp, Account.ID from Contact where Account.ID = " + AccountIDToUpdate)
                        End If

                        'Note - Added second phone just so we have something in that space..
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
                    Else
                        Log.Write("No records found.")
                    End If


                    Log.Write("All done with contact records!")

                Catch ex As Exception
                    Log.Write("Failed to execute query succesfully, error message was: " & ex.Message)
                    LogSyncResult("Error in GetContactInformation: " + ex.ToString)

                End Try
            End Sub

            Public Function CreateNote(ByVal accountid As String, ByVal Title As String, ByVal Body As String, ByVal ParentOrgID As String) As Boolean
                Dim Success As Boolean = True

                Try

                    'Attach a note, which will get re-parented
                    Dim note As sForce.sObject = New sObject()
                    note.type = "Note"
                    note.Any = New System.Xml.XmlElement() {GetNewXmlElement("ParentId", accountid), _
                        GetNewXmlElement("Body", Body), _
                        GetNewXmlElement("Title", Title)}

                    Dim noteSave As SaveResult = Binding.create(New sObject() {note})(0)

                    Success = noteSave.success
                Catch ex As Exception
                    Log.Write("Error in CreateNote: " & ex.Message)
                    LogSyncResult("Error in CreateNote: " & ex.Message)

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

            Public Sub GetProductAndLicenseInfo(ByVal SFLastUpdateTime As String)
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

                Dim ProductName, LicenseType, LicenseStatus, AccountID As String
                Dim ExpirationDate As Date

                Try
                    Dim done As Boolean = False

                    Try
                        qr = Binding.query("select Product__c, Expiration_Date__c, License_type__c, Status__c, Account__c from License__c where SystemModStamp >= " + SFLastUpdateTime + " ORDER BY Product__c") 'added order by 3/25/11

                        'and SystemModStamp >= 2011-01-22T20:50:18.000Z" '"
                        done = False

                        Log.Write("Found " + qr.size.ToString + " license records.")
                    Catch ex As Exception
                        'Problem - Not sure what.
                        done = True

                        Log.Write("Error in GetProductAndLicenseInfo. " + ex.Message)
                    End Try

                    Dim LastExpirationDate As Date = DateTime.Parse("Jan 01, 1970 12:00:00 PM") 'set to default date way back
                    Dim LastProduct As String = ""

                    If qr.size > 0 Then
                        While Not done
                            For i As Integer = 0 To qr.records.Length - 1

                                Log.Write("In GetProductAndLicenseInfo routine - i=" + i.ToString)

                                Dim records As sObject() = qr.records
                                Dim Products As sObject = records(i)

                                ProductName = records(i).Any(0).InnerText
                                ExpirationDate = Date.Parse(records(i).Any(1).InnerText)
                                LicenseType = records(i).Any(2).InnerText
                                LicenseStatus = records(i).Any(3).InnerText
                                AccountID = records(i).Any(4).InnerText

                                'OK, what do we do now?
                                ' 1) See if we can match a product in TS with the product name
                                '     select ProductID from products where name = {ProductName} and organizationid=305383

                                Dim thisProduct As Product
                                Dim findProduct As New Products(User)
                                findProduct.LoadByOrganizationID(CRMLinkRow.OrganizationID)

                                thisProduct = findProduct.FindByName(ProductName)

                                If thisProduct IsNot Nothing Then
                                    Log.Write("ProductName = " + ProductName + ", ExpirationDate=" + ExpirationDate.ToString() + ", ProductID=" + thisProduct.ProductID.ToString())

                                    ' 2) If we can, lets see if the product ID is assigned to this customer
                                    '   select OrganizationProductID from OrganizationProducts where Organizationid = {clientorgid} and productid={productid from above}

                                    Dim findCompany As New Organizations(User)
                                    Dim thisCompany As Organization

                                    'make sure the company already exists
                                    findCompany.LoadByCRMLinkID(AccountID)

                                    If findCompany.Count > 0 Then
                                        thisCompany = findCompany(0)
                                        Dim findOrgProd As New OrganizationProducts(User)
                                        Dim thisOrgProd As OrganizationProduct

                                        findOrgProd.LoadByOrganizationAndProductID(thisCompany.OrganizationID, thisProduct.ProductID)

                                        If findOrgProd.Count > 0 Then
                                            thisOrgProd = findOrgProd(0)
                                            'The company already has the product associated with them.
                                            ' We now just need to update the waranty expiration date 
                                            '  Note that the waranty expiration date is a custom field just for this customer...
                                            '      3) Update the waranty expiration date (this should be a custom field on product)
                                            '         Update CustomValue set CustomValue={ExpirationDate} where CustomFieldID = 3761 and RefID={OrganizationProductID}

                                            If (ProductName <> LastProduct) Or ExpirationDate > LastExpirationDate Then 'test to see if we are using the most up to date expiration date (only use product/expiration date that is the most recent)
                                                thisOrgProd.SupportExpiration = ExpirationDate
                                                thisOrgProd.Collection.Save()

                                                'License Type - CustomFieldID is 3770 (test value 101)
                                                UpdateCustomValue(3770, thisOrgProd.OrganizationProductID, LicenseType)

                                                'License Status - CustomFieldID is 3771 (test value 102)
                                                UpdateCustomValue(3771, thisOrgProd.OrganizationProductID, LicenseStatus)

                                                LastProduct = ProductName

                                                Log.Write("Product updated.")
                                            Else
                                                Log.Write("Date information not updated since there is a later expiration date.")
                                            End If

                                            LastExpirationDate = ExpirationDate 'changed from just expirationdate to date.parse(expirationdate) 4/4/11
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
                                            Log.Write("Product updated.")
                                        End If
                                    End If

                                End If

                            Next
                            done = True

                        End While
                    Else
                        Log.Write("No records found.")
                    End If


                Catch ex As Exception
                    Log.Write("Error in GetProductAndLicenseInfo : Failed to execute succesfully, error message was: " & ex.Message & " " & ex.StackTrace)
                End Try


            End Sub

            Private Sub UpdateCustomValue(ByVal customFieldID As Integer, ByVal OrgProdID As Integer, ByVal Value As String)
                Dim findCustom As New CustomValues(User)
                Dim thisCustom As CustomValue

                findCustom.LoadByFieldID(customFieldID, OrgProdID)
                If findCustom.Count > 0 Then
                    thisCustom = findCustom(0)
                Else
                    thisCustom = (New CustomValues(User)).AddNewCustomValue()
                    thisCustom.CustomFieldID = customFieldID
                    thisCustom.RefID = OrgProdID
                End If

                thisCustom.Value = Value
                thisCustom.Collection.Save()
            End Sub

            Protected Overrides Sub Finalize()
                MyBase.Finalize()
            End Sub

        End Class

    End Namespace
End Namespace