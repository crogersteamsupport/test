Imports TeamSupport.Data

Namespace TeamSupport
    Namespace CrmIntegration
        Public MustInherit Class Integration
            Protected CRMLinkRow As CRMLinkTableItem
            Protected Log As SyncLog
            Protected User As LoginUser
            Protected ReadOnly Type As IntegrationType

            Protected Sub New(ByVal crmLinkOrg As CRMLinkTableItem, ByVal crmLog As SyncLog, ByVal thisUser As LoginUser, ByVal thisType As IntegrationType)
                CRMLinkRow = crmLinkOrg
                Log = crmLog
                User = thisUser
                Type = thisType
            End Sub

            Public MustOverride Function PerformSync() As Boolean
            Public MustOverride Function SendTicketData() As Boolean

            Protected Sub UpdateOrgInfo(ByVal company As CompanyData, ByVal ParentOrgID As String)
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
                        'if still not, add new
                        
                        thisCompany = (New Organizations(User)).AddNewOrganization()
                        thisCompany.ParentID = ParentOrgID
                        thisCompany.Name = company.AccountName
                        thisCompany.CRMLinkID = company.AccountID
                        thisCompany.IsActive = True
                        thisCompany.Collection.Save()

                        Log.Write("Added a new account.")
                    End If

                End If

                Dim findAddress As New Addresses(User)
                Dim thisAddress As Address
                findAddress.LoadByID(thisCompany.OrganizationID, ReferenceType.Organizations)

                If findAddress.Count > 0 Then
                    thisAddress = findAddress(0)

                    Log.Write("Address information updated.")
                Else
                    thisAddress = (New Addresses(User)).AddNewAddress()
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
                End With

                findAddress.Save()

                If company.Phone <> "" Then
                    Log.Write("Adding/updating account phone numbers.")
                    Log.Write("AccountID=" & company.AccountID & ", OrgID=" & thisCompany.OrganizationID)
                    Dim thisPhone As New PhoneNumbers(User)
                    thisPhone.LoadByOrganizationID(thisCompany.OrganizationID)

                    If thisPhone.Count > 0 Then
                        Log.Write("Found a phone record, updating...")
                        thisPhone(0).Number = company.Phone
                        Log.Write("Organization phone number updated.")
                    Else
                        Log.Write("No phone record found--adding a new record.")

                        Dim newPhone As PhoneNumber = (New PhoneNumbers(User)).AddNewPhoneNumber()
                        newPhone.Number = company.Phone
                        newPhone.Collection.Save()
                    End If


                Else
                    Log.Write("There is no phone number to add.")
                End If

                Log.Write("Updated w/ Address:  " + company.AccountName)
            End Sub

            Protected Sub UpdateContactInfo(ByVal person As EmployeeData, ByVal companyID As String, ByVal ParentOrgID As String)
                If person.Email = "" Then
                    Return
                End If

                Dim findCompany As New Organizations(User)

                'search for the crmlinkid = accountid in db to see if it already exists
                findCompany.LoadByCRMLinkID(companyID)
                If findCompany.Count > 0 Then
                    Dim thisCompany As Organization = findCompany(0)


                    'If Not MatchContactEmail(OrgID, Email) Then
                    '    'add the contact
                    '    AddContact(OrgID, Email, FirstName, LastName, Phone, title, isdeleted)
                    '    'Now that we've created the contact, we need to add the phone information
                    '    UpdatePhoneIfNeeded(OrgID, Email, Phone, "Work", ParentOrganizationID)
                    '    UpdatePhoneIfNeeded(OrgID, Email, mobilephone, "Mobile", ParentOrganizationID)
                    '    UpdatePhoneIfNeeded(OrgID, Email, fax, "Fax", ParentOrganizationID)

                    'Else
                    '    'we've found the contact - Let's update the data
                    '    UpdateContactInfoIfNeeded(OrgID, Email, FirstName, LastName, title, isdeleted)
                    '    UpdatePhoneIfNeeded(OrgID, Email, Phone, "Work", ParentOrganizationID)
                    '    UpdatePhoneIfNeeded(OrgID, Email, mobilephone, "Mobile", ParentOrganizationID)
                    '    UpdatePhoneIfNeeded(OrgID, Email, fax, "Fax", ParentOrganizationID)


                    'End If
                End If

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
    End Namespace
End Namespace