Imports TeamSupport.Data
Imports System.Xml
Imports System.IO
Imports System.Net
Imports System.Text

Namespace TeamSupport
    Namespace CrmIntegration
        Public MustInherit Class Integration
            Protected CRMLinkRow As CRMLinkTableItem
            Protected Log As SyncLog
            Protected User As LoginUser
            Protected Processor As CrmProcessor
            Protected ReadOnly Type As IntegrationType
            Protected Const Client As String = "Muroc Client"

            'tracks global errors so we can not update the sync date if there's a problem
            Private _syncError As Boolean = False
            Public Property SyncError As Boolean
                Get
                    Return _syncError
                End Get
                Protected Set(ByVal value As Boolean)
                    _syncError = value
                End Set
            End Property

            Protected Sub New(ByVal crmLinkOrg As CRMLinkTableItem, ByVal crmLog As SyncLog, ByVal thisUser As LoginUser, ByVal thisProcessor As CrmProcessor, ByVal thisType As IntegrationType)
                CRMLinkRow = crmLinkOrg
                Log = crmLog
                User = thisUser
                Type = thisType
                Processor = thisProcessor
            End Sub

            Public MustOverride Function PerformSync() As Boolean
            Public MustOverride Function SendTicketData() As Boolean

            Protected Sub UpdateOrgInfo(ByVal company As CompanyData, ByVal ParentOrgID As String)
                If Processor.IsStopped Then
                    Return
                End If

                Dim findCompany As New Organizations(User)
                Dim thisCompany As Organization

                'search for the crmlinkid = accountid in db to see if it already exists
                findCompany.LoadByCRMLinkID(company.AccountID)
                If findCompany.Count > 0 Then
                    'TODO: ask Kevin is this the right way to get an item from a group?
                    thisCompany = findCompany(0)
                    'it exists, so update the name on the account if it has changed.
                    thisCompany.Name = company.AccountName

                Else
                    'look for parentid = parentorgid and name = accountname, and use that
                    findCompany.LoadByParentID(ParentOrgID, False)
                    If findCompany.FindByName(company.AccountName) IsNot Nothing Then
                        thisCompany = findCompany.FindByName(company.AccountName)
                        'update accountid
                        thisCompany.CRMLinkID = company.AccountID

                    Else
                        'if still not found, add new

                        thisCompany = (New Organizations(User)).AddNewOrganization()
                        thisCompany.ParentID = ParentOrgID
                        thisCompany.Name = company.AccountName
                        thisCompany.CRMLinkID = company.AccountID
                        thisCompany.IsActive = True


                        With New Organizations(User)
                            .LoadByOrganizationID(ParentOrgID)
                            thisCompany.SlaLevelID = .Item(0).SlaLevelID
                        End With

                        thisCompany.HasPortalAccess = ParentOrgID = "305383" 'This is hack for now for Axceler.  Need to change to an option - 3/9/11

                        Log.Write("Added a new account.")
                    End If
                End If

                thisCompany.Collection.Save()

                Dim findAddress As New Addresses(User)
                Dim thisAddress As Address
                findAddress.LoadByID(thisCompany.OrganizationID, ReferenceType.Organizations)

                If findAddress.Count > 0 Then
                    thisAddress = findAddress(0)

                    Log.Write("Address information updated.")
                Else
                    thisAddress = (New Addresses(User)).AddNewAddress()
                    thisAddress.RefID = thisCompany.OrganizationID
                    thisAddress.RefType = ReferenceType.Organizations
                    thisAddress.Collection.Save()

                    Log.Write("Address information added.")
                End If

                With thisAddress
                    .Addr1 = company.Street
                    .Addr2 = company.Street2
                    .City = company.City
                    .State = company.State
                    .Zip = company.Zip
                    .Country = company.Country
                    .Collection.Save()
                End With

                If company.Phone <> "" Then
                    Log.Write("Adding/updating account phone numbers.")
                    Log.Write(String.Format("AccountID={0}, OrgID={1}", company.AccountID, thisCompany.OrganizationID))

                    Dim findPhone As New PhoneNumbers(User)
                    Dim thisPhone As PhoneNumber
                    findPhone.LoadByID(thisCompany.OrganizationID, ReferenceType.Organizations)

                    If findPhone.Count > 0 Then
                        Log.Write("Found a phone record, updating...")
                        thisPhone = findPhone(0)
                    Else
                        Log.Write("No phone record found--adding a new record.")

                        thisPhone = (New PhoneNumbers(User)).AddNewPhoneNumber()
                        thisPhone.RefID = thisCompany.OrganizationID
                        thisPhone.RefType = ReferenceType.Organizations
                        'TODO: update to handle fax numbers
                        thisPhone.Collection.Save()
                    End If

                    With thisPhone
                        .Number = company.Phone
                        .Collection.Save()
                        Log.Write("Organization phone number updated.")
                    End With
                Else
                    Log.Write("There is no phone number to add.")
                End If

                Log.Write("Updated w/ Address:" & company.AccountName)
            End Sub

            Protected Sub UpdateContactInfo(ByVal person As EmployeeData, ByVal companyID As String, ByVal ParentOrgID As String)
                If Processor.IsStopped Then
                    Return
                End If

                If person.Email = "" Then
                    'we don't add contacts with no email address
                    Return
                End If

                Log.Write(String.Format("Adding/updating contact information for {0} ({1},{2}).", person.Email, person.LastName, person.FirstName))

                Dim findCompany As New Organizations(User)

                'make sure the company already exists
                findCompany.LoadByCRMLinkID(companyID)
                If findCompany.Count > 0 Then
                    Dim thisCompany As Organization = findCompany(0)

                    Dim findUser As New Users(User)
                    Dim thisUser As User

                    findUser.LoadByOrganizationID(thisCompany.OrganizationID, False)
                    If findUser.FindByEmail(person.Email) IsNot Nothing Then
                        thisUser = findUser.FindByEmail(person.Email)

                    Else
                        'add the contact
                        thisUser = (New Users(User)).AddNewUser()
                        thisUser.OrganizationID = thisCompany.OrganizationID
                        thisUser.Collection.Save()
                    End If

                    With thisUser
                        .Email = person.Email
                        .FirstName = person.FirstName
                        .LastName = person.LastName
                        .Title = person.Title
                        .IsActive = True
                        .MarkDeleted = False
                        .CryptedPassword = "cfsdfewwgewff" 'not sure why this is done this way but keep as is for now
                        .Collection.Save()
                    End With

                    Log.Write("Updating phone information.")
                    Dim findPhone As New PhoneNumbers(User)

                    Dim thesePhoneTypes As New PhoneTypes(User)
                    thesePhoneTypes.LoadAllPositions(ParentOrgID)

                    findPhone.LoadByID(thisUser.UserID, ReferenceType.Users)

                    Dim workPhone, mobilePhone, faxPhone As PhoneNumber

                    If findPhone.Count > 0 Then
                        workPhone = findPhone.FindByPhoneTypeID(thesePhoneTypes.FindByName("Work").PhoneTypeID)
                        mobilePhone = findPhone.FindByPhoneTypeID(thesePhoneTypes.FindByName("Mobile").PhoneTypeID)
                        faxPhone = findPhone.FindByPhoneTypeID(thesePhoneTypes.FindByName("Fax").PhoneTypeID)
                    End If

                    If person.Phone IsNot Nothing Then
                        If workPhone Is Nothing Then
                            workPhone = (New PhoneNumbers(User).AddNewPhoneNumber())
                        End If

                        workPhone.Number = person.Phone
                        workPhone.PhoneTypeID = thesePhoneTypes.FindByName("Work").PhoneTypeID
                        workPhone.RefType = ReferenceType.Users
                        workPhone.RefID = thisUser.UserID
                        workPhone.Collection.Save()
                    End If

                    If person.Cell IsNot Nothing Then
                        If mobilePhone Is Nothing Then
                            mobilePhone = (New PhoneNumbers(User).AddNewPhoneNumber())
                        End If

                        mobilePhone.Number = person.Cell
                        mobilePhone.PhoneTypeID = thesePhoneTypes.FindByName("Mobile").PhoneTypeID
                        mobilePhone.RefType = ReferenceType.Users
                        mobilePhone.RefID = thisUser.UserID
                        mobilePhone.Collection.Save()
                    End If

                    If person.Fax IsNot Nothing Then
                        If faxPhone Is Nothing Then
                            faxPhone = (New PhoneNumbers(User).AddNewPhoneNumber())
                        End If

                        faxPhone.Number = person.Fax
                        faxPhone.PhoneTypeID = thesePhoneTypes.FindByName("Fax").PhoneTypeID
                        faxPhone.RefType = ReferenceType.Users
                        faxPhone.RefID = thisUser.UserID
                        faxPhone.Collection.Save()
                    End If


                    Log.Write("Phone information updated.")
                End If

            End Sub

            Protected Function GetXML(ByVal Key As NetworkCredential, ByVal Address As Uri) As XmlDocument
                Dim returnXML As XmlDocument = Nothing

                If Address IsNot Nothing Then
                    Dim request As HttpWebRequest = WebRequest.Create(Address)
                    request.Credentials = Key
                    request.Method = "GET"
                    request.KeepAlive = False
                    request.UserAgent = Client
                    request.Timeout = 7000

                    Using response As HttpWebResponse = request.GetResponse()

                        If request.HaveResponse AndAlso response IsNot Nothing Then
                            Using reader As New StreamReader(response.GetResponseStream())

                                returnXML = New XmlDocument()
                                returnXML.LoadXml(reader.ReadToEnd())
                            End Using
                        End If

                    End Using

                End If
                Return returnXML
            End Function

            Protected Function PostXML(ByVal Key As NetworkCredential, ByVal Address As Uri, ByVal Content As String) As HttpStatusCode
                Dim returnStatus As HttpStatusCode = Nothing

                If Address IsNot Nothing And Content <> "" Then
                    Dim byteData = UTF8Encoding.UTF8.GetBytes(Content)

                    Dim request As HttpWebRequest = WebRequest.Create(Address)
                    request.Credentials = Key
                    request.Method = "POST"
                    request.ContentType = "application/xml"
                    request.UserAgent = Client
                    request.ContentLength = byteData.Length

                    Using postStream As Stream = request.GetRequestStream()
                        postStream.Write(byteData, 0, byteData.Length)
                    End Using

                    Using response As HttpWebResponse = request.GetResponse()
                        If request.HaveResponse AndAlso response IsNot Nothing Then
                            returnStatus = response.StatusCode
                        End If
                    End Using

                End If

                Return returnStatus
            End Function

            Protected Sub LogSyncResult(ByVal ResultText As String)
                Dim result As CRMLinkResult
                result = (New CRMLinkResults(User)).AddNewCRMLinkResult()
                result.AttemptResult = ResultText
                result.OrganizationID = CRMLinkRow.OrganizationID
                result.AttemptDateTime = Now.ToUniversalTime()
                result.Collection.Save()
            End Sub

        End Class

        Public Class CompanyData
            Property City As String
            Property Country As String
            Property State As String
            Property Street As String
            Property Street2 As String
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

        Enum PhoneType
            Work
            Mobile
            Fax
        End Enum

        Public Class SyncLog

            Private LogPath As String
            Private FileName As String

            Public Sub New(ByVal Path As String)
                LogPath = Path
                FileName = "CRM Sync Debug File - " & Today.Month.ToString() & Today.Day.ToString() & Today.Year.ToString() & ".txt"

                If (Not Directory.Exists(LogPath)) Then
                    Directory.CreateDirectory(LogPath)
                End If
            End Sub

            Public Sub Write(ByVal Text As String)
                File.AppendAllText(LogPath & "\" & FileName, Now.ToString + ": " + Text + Environment.NewLine)
            End Sub

        End Class
    End Namespace
End Namespace