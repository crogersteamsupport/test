Imports System.Data.SqlClient

Public Class TSCheckData
    Private Shared CS As String = My.Settings.ConnectionString.ToString

    Public Function DoesAccountIDExist(ByVal AccountID As String) As Boolean
        'This will search the TS organizations table to see if account ID exists
    End Function

    Public Shared Function GetUserID(ByRef UserName As String, ByRef OrganizationID As String) As String
        'Can get username via user.identity.name

        'returns the user id from the username and org id
        Dim sqlStatement As String = "select UserID from users where email = '" + UserName + "' and organizationid = " + OrganizationID

        Dim ConnectionString As String = My.Settings.ConnectionString.ToString
        Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)

        command.Connection.Open()

        Dim reader As SqlDataReader = command.ExecuteReader
        Try
            'return the UserID
            If reader.Read Then
                Return reader(0)
            Else
                Return "-1" 'changed to return -1 (may cause issue in portal - Need to check.  2/12/09)
            End If
        Finally

            reader.Close()

            command.Connection.Close()

            If connection.State = ConnectionState.Open Then connection.Close()
        End Try

    End Function


    Public Function GetOrgIDFromCRMLinkID(ByVal CRMLinkID As String) As String

        'This will get the OrgID when passed the CRMLinkID

        Dim ConnectionString As String = My.Settings.ConnectionString.ToString

        Dim sqlStatement As String = "select top 1 OrganizationID from Organizations where CRMLinkID = @CRMLinkID"
        Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@CRMLinkID", CRMLinkID))

        command.Connection.Open()

        Dim reader As SqlDataReader = command.ExecuteReader

        Try


            'return the OrgID
            If reader.Read Then
                Return reader(0)
            Else
                Return "-1"  '-1 if we can't find the CRMLink
            End If
        Finally

            command.Connection.Close()
            command.Parameters.Clear()

            command.Dispose()
            reader.Close()


            If connection.State = ConnectionState.Open Then connection.Close()
        End Try

    End Function

    Public Function MatchAccountName(ByVal CRMLinkID As String, ByVal Name As String, ByVal ParentID As String) As String

        'This will get the OrgID if there is a matching name

        '7/10/09 - Now we only look for Orgs with the ParentID matching

        Dim ConnectionString As String = My.Settings.ConnectionString.ToString

        Dim sqlStatement As String = "select top 1 OrganizationID from Organizations where name = @Name and ParentID = @ParentID"
        Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@Name", Trim(Name)))
        command.Parameters.Add(New SqlParameter("@ParentID", ParentID))

        command.Connection.Open()

        Dim reader As SqlDataReader = command.ExecuteReader

        Try


            'return the OrgID
            If reader.Read Then
                Return reader(0) 'return OrgID
            Else
                Return "-1"  '-1 if we can't find a matching company name
            End If
        Finally

            reader.Close()

            command.Parameters.Clear()
            command.Connection.Close()
            command.Dispose()

            If connection.State = ConnectionState.Open Then connection.Close()
            connection.Dispose()
        End Try

    End Function

    Public Function MatchContactEmail(ByVal OrgID As String, ByVal email As String) As Boolean

        'This will get the OrgID if there is a matching name

        Dim ConnectionString As String = My.Settings.ConnectionString.ToString

        Dim sqlStatement As String = "select email from users where OrganizationID = @OrgID and email = @email"
        Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@OrgID", OrgID))
        command.Parameters.Add(New SqlParameter("@email", email))

        command.Connection.Open()

        Dim reader As SqlDataReader = command.ExecuteReader

        Try


            'return the OrgID
            If reader.Read Then
                Return True
            Else
                Return False  '-1 if we can't find the contact
            End If
        Finally

            reader.Close()
            command.Parameters.Clear()

            command.Connection.Close()
            command.Dispose()

            If connection.State = ConnectionState.Open Then connection.Close()
        End Try

    End Function



    Public Sub UpdateCRMLinkID(ByVal CRMLinkID As String, ByVal OrgID As String)

        'This will update the CRMLink ID for a given Org ID

        Dim ConnectionString As String = My.Settings.ConnectionString.ToString

        'RefType = 9 is the billing address for the company - Not sure if it's guaranteed to exist or not.
        Dim sqlStatement As String = _
        "update Organizations set CRMLinkID = @CRMLinkID where OrganizationID = @OrgID"


        Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)

        command.Parameters.Add(New SqlParameter("@OrgID", OrgID))
        command.Parameters.Add(New SqlParameter("@CRMLinkID", CRMLinkID))


        command.Connection.Open()

        command.ExecuteNonQuery()
        command.Parameters.Clear()

        command.Connection.Close()
        command.Dispose()

        If connection.State = ConnectionState.Open Then connection.Close()

    End Sub

    Function GetSLALevelID(ByVal ParentOrgID As String) As String
        'this will return the first SLA defined for the parent org id
        Dim ConnectionString As String = My.Settings.ConnectionString.ToString

        Dim sqlStatement As String = "select top 1 SLALevelID from SLALevels where OrganizationID = @OrgID order by SLALevelID"
        Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@OrgID", ParentOrgID))


        command.Connection.Open()

        Dim reader As SqlDataReader = command.ExecuteReader
        Try

            Try


                'return the OrgID
                If reader.Read Then
                    Return reader(0).ToString
                Else
                    Return ""
                End If
            Catch ex As Exception
                Return ""
            End Try
        Finally

            command.Parameters.Clear()

            command.Connection.Close()
            command.Dispose()

            reader.Close()

            If connection.State = ConnectionState.Open Then connection.Close()
        End Try
    End Function

    Public Sub AddNewAccount(ByVal AccountID As String, ByVal name As String, ByVal ParentOrganizationID As String)
        Try


            'This will add a new account (organization) in TS

            Dim ConnectionString As String = My.Settings.ConnectionString.ToString

            Dim SLALevalID As String = GetSLALevelID(ParentOrganizationID) 'Gets the first SLA level 
            Dim sqlStatement As String

            Dim GivePortalAccess As Boolean = False

            If ParentOrganizationID = "305383" Then GivePortalAccess = True 'This is hack for now for Axceler.  Need to change to an option - 3/9/11

            If SLALevalID <> "" Then 'If we have an SLA for the parent company then let's add it when we add the new company
                'RefType = 9 is the billing address for the company - Not sure if it's guaranteed to exist or not.
                sqlStatement = _
                "insert into  Organizations(name, ParentID, CRMLinkID, IsCustomerFree, UserSeats, PortalSeats, ExtraStorageUnits, IsActive, HasPortalAccess, ProductType, DateCreated, DateModified, CreatorID, ModifierID, SLALevelID) " & _
                "Values(@Name, @ParentID, @CRMLinkID, 0, 0, 0, 0, 1, @PortalAccess, 0, getutcdate(), getutcdate(), -1, -1, @SLALevelID)"


            Else
                'RefType = 9 is the billing address for the company - Not sure if it's guaranteed to exist or not.
                sqlStatement = _
                "insert into  Organizations(name, ParentID, CRMLinkID, IsCustomerFree, UserSeats, PortalSeats, ExtraStorageUnits, IsActive, HasPortalAccess, ProductType, DateCreated, DateModified, CreatorID, ModifierID) " & _
                "Values(@Name, @ParentID, @CRMLinkID, 0, 0, 0, 0, 1, @PortalAccess, 0, getutcdate(), getutcdate(), -1, -1)"

            End If




            Dim connection As New SqlConnection(ConnectionString)

            Dim command As New SqlCommand(sqlStatement, connection)

            command.Parameters.Add(New SqlParameter("@Name", name))
            command.Parameters.Add(New SqlParameter("@CRMLinkID", AccountID))
            command.Parameters.Add(New SqlParameter("@ParentID", ParentOrganizationID))
            command.Parameters.Add(New SqlParameter("@PortalAccess", GivePortalAccess))
            If SLALevalID <> "" Then command.Parameters.Add(New SqlParameter("@SLALevelID", SLALevalID))


            command.Connection.Open()

            command.ExecuteNonQuery()


            command.Parameters.Clear()

            command.Connection.Close()

            command.Dispose()


            If connection.State = ConnectionState.Open Then connection.Close()
            connection.Dispose()


            Form1.AddText("Added a new account.")
        Catch ex As Exception
            Form1.AddText("Error in AddNewAccount. Error = " + ex.Message)
        End Try
    End Sub

    Public Sub AddContact(ByVal OrganizationID As String, ByVal email As String, ByVal firstname As String, ByVal lastname As String, ByVal phone As String, ByVal title As String, ByVal isdeleted As String)

        'This will add a new contact in TS
        Try

            Form1.AddText("In AddContact routine for user " + email)
            Dim ConnectionString As String = My.Settings.ConnectionString.ToString

            ' Dim GivePortalAccess As Boolean = False

            'If ParentOrganizationID = "305383" Then GivePortalAccess = True 'This is hack for now for Axceler.  Need to change to an option - 3/9/11


            'RefType = 9 is the billing address for the company - Not sure if it's guaranteed to exist or not.
            Dim sqlStatement As String = _
            "insert into  Users(email, FirstName, MiddleName, LastName, Title, CryptedPassword, IsActive, LastLogin, LastActivity, IsSystemAdmin, IsFinanceAdmin, IsPasswordExpired, IsPortalUser, InOffice, InOfficeComment, ActivatedOn, OrganizationID, DateCreated, DateModified, CreatorID, ModifierID, MarkDeleted, LastWaterCoolerID, IsChatUser, ReceiveTicketNotifications, ReceiveAllGroupNotifications,SubscribeToNewTickets) " & _
            "Values(@email, @FirstName, '', @LastName, @Title, 'cfsdfewwgewff', 1, GetutcDate(), GetutcDate(), 0, 0, 1, 0, 1, '', GetutcDate(), @OrganizationID, GetutcDate(), GetutcDate(), -1, -1, 0, 0,0,0,0,0)"



            Dim connection As New SqlConnection(ConnectionString)

            Dim command As New SqlCommand(sqlStatement, connection)

            command.Parameters.Add(New SqlParameter("@email", email))
            command.Parameters.Add(New SqlParameter("@FirstName", firstname))
            command.Parameters.Add(New SqlParameter("@LastName", lastname))
            command.Parameters.Add(New SqlParameter("@Title", title))
            command.Parameters.Add(New SqlParameter("@OrganizationID", OrganizationID))

            command.Connection.Open()

            command.ExecuteNonQuery()
            command.Parameters.Clear()

            command.Connection.Close()
            command.Dispose()


            If connection.State = ConnectionState.Open Then connection.Close()

            Form1.AddText("Added a new contact.")
        Catch ex As Exception
            Form1.AddText("Error in AddContact.  Error = " + ex.Message)
        End Try
    End Sub




    Public Sub UpdateAddressInfoIfNeeded(ByVal AccountID As String, ByVal name As String, ByVal Addr1 As String, ByVal Addr2 As String, ByVal city As String, ByVal state As String, ByVal zip As String, ByVal country As String)
        'This will pull the info from the TS database and compare it to the info in the CRM and will update if needed

        Try


            Dim ConnectionString As String = My.Settings.ConnectionString.ToString

            'RefType = 9 is the billing address for the company - Not sure if it's guaranteed to exist or not.
            Dim sqlStatement As String = _
            "Select Addr1, Addr2, City, State, Zip, Country from Addresses where RefID = @OrgID and RefType = 9"


            Dim connection As New SqlConnection(ConnectionString)

            Dim command As New SqlCommand(sqlStatement, connection)

            command.Parameters.Add(New SqlParameter("@OrgID", GetOrgIDFromCRMLinkID(AccountID)))

            command.Connection.Open()

            Dim reader As SqlDataReader = command.ExecuteReader


            If reader.Read Then
                'If we have found an address record then see if all the fields match.  If they don't, then update as needed
                If (reader(0).ToString <> Addr1) Or (reader(1).ToString <> Addr2) Or (reader(2).ToString <> city) Or (reader(3).ToString <> state) Or (reader(4).ToString <> zip) Or (reader(5).ToString <> country) Then
                    'If any of the fields do not match then update the entire address record
                    UpdateTSAddressInfo(AccountID, name, Addr1, Addr2, city, state, zip, country)

                End If

            Else
                'Uh-oh - There is no address record for this account!  Let's add one
                If GetOrgIDFromCRMLinkID(AccountID) <> "-1" Then
                    'Only add if there's a match for the org id
                    AddTSAddressInfo(AccountID, name, Addr1, Addr2, city, state, zip, country)
                End If

            End If

            reader.Close()

            command.Parameters.Clear()
            command.Connection.Close()
            command.Dispose()


            If connection.State = ConnectionState.Open Then connection.Close()
            connection.Dispose()
        Catch ex As Exception
            Form1.AddText("Error in UpdateAddressInfoIfNeeded.  Error = " + ex.Message)
        End Try
    End Sub

    Public Sub UpdateAccountPhoneIfNeeded(ByVal AccountID As String, ByVal Phone As String, ByVal ParentOrgID As String)
        'Updates the Organization's phone number if needed
        Try


            Form1.AddDebugText("Running UpdateAccountPhoneIfNeeded")

            Dim ConnectionString As String = My.Settings.ConnectionString.ToString

            'Getst the first phone number from the PhoneNumber table that has a RefType of 22 (an orgaization's number as opposed to a person's) 
            Dim sqlStatement As String = _
            "select top 1 p.PhoneNumber, p.PhoneID from phonenumbers as p, PhoneTypes as pt where p.PhoneTypeID = pt.PhoneTypeID and p.refid=@OrgID and p.RefType = 9 order by pt.position"


            Dim connection As New SqlConnection(ConnectionString)

            Dim command As New SqlCommand(sqlStatement, connection)

            command.Parameters.Add(New SqlParameter("@OrgID", GetOrgIDFromCRMLinkID(AccountID)))
            Form1.AddDebugText("AccountID = " + AccountID + ", Org ID = " + GetOrgIDFromCRMLinkID(AccountID))
            command.Connection.Open()

            Dim reader As SqlDataReader = command.ExecuteReader


            If reader.Read Then
                'If we have found a phone record
                If (reader(0).ToString <> Phone) Then
                    'If any of the fields do not match then update phone number using the PhoneID field
                    Form1.AddDebugText("Found a phone record, updating...")
                    UpdateOrgPhoneNumber(reader(1).ToString, Phone)

                End If

            Else
                'Uh-oh - Appears we don't have any phone numbers for this account so we need to add one
                If GetOrgIDFromCRMLinkID(AccountID) <> "-1" Then
                    'Only add if there's a match for the org id
                    If Phone <> "" Then
                        Form1.AddDebugText("No phone record found - Adding a new record")
                        AddOrgPhoneNumber(ParentOrgID, AccountID, Phone)
                    Else
                        Form1.AddDebugText("There is no phone number to add")
                    End If

                End If

            End If

            reader.Close()

            command.Parameters.Clear()
            command.Connection.Close()
            command.Dispose()


            If connection.State = ConnectionState.Open Then connection.Close()
            connection.Dispose()
        Catch ex As Exception
            Form1.AddText("Error in UpdateAccountPhoneIfNeeded.  Error = " + ex.Message)
        End Try
    End Sub


    Public Sub UpdateAccountNameIfNeeded(ByVal AccountID As String, ByVal name As String)
        'This will pull the info from the TS database and compare it to the info in the CRM and will update if needed
        Try


            Dim ConnectionString As String = My.Settings.ConnectionString.ToString

            'RefType = 9 is the billing address for the company - Not sure if it's guaranteed to exist or not.
            Dim sqlStatement As String = _
            "Select Name from Organizations where CRMLinkID = @CRMLinkID"


            Dim connection As New SqlConnection(ConnectionString)

            Dim command As New SqlCommand(sqlStatement, connection)

            command.Parameters.Add(New SqlParameter("@CRMLinkID", AccountID))

            command.Connection.Open()

            Dim reader As SqlDataReader = command.ExecuteReader


            If reader.Read Then
                'If we have found an address record then see if all the fields match.  If they don't, then update as needed
                If (reader(0).ToString <> name) Then
                    'If any of the fields do not match then update the entire address record
                    UpdateTSAccountName(AccountID, name)

                End If

            Else


            End If

            command.Parameters.Clear()

            command.Connection.Close()
            command.Dispose()

            reader.Close()


            If connection.State = ConnectionState.Open Then connection.Close()
        Catch ex As Exception
            Form1.AddText("Error in UpdateAccountNameIfNeeded.  Error = " + ex.Message)
        End Try

    End Sub


    Public Sub UpdateTSAccountName(ByVal AccountID As String, ByVal name As String)

        'This will update the address information for a company
        Try


            Dim ConnectionString As String = My.Settings.ConnectionString.ToString

            'RefType = 9 is the billing address for the company - Not sure if it's guaranteed to exist or not.
            Dim sqlStatement As String = _
            "update Organizations set Name = @name where CRMLinkID = @CRMLinkID"


            Dim connection As New SqlConnection(ConnectionString)

            Dim command As New SqlCommand(sqlStatement, connection)

            command.Parameters.Add(New SqlParameter("@Name", name))
            command.Parameters.Add(New SqlParameter("@CRMLinkID", AccountID))


            command.Connection.Open()

            command.ExecuteNonQuery()
            command.Parameters.Clear()

            command.Connection.Close()
            command.Dispose()

            If connection.State = ConnectionState.Open Then connection.Close()

            Form1.AddText("Account name updated.")
        Catch ex As Exception
            Form1.AddText("Error in UpdateTSAccountName.  Error = " + ex.Message)

        End Try
    End Sub


    Public Sub UpdateTSAddressInfo(ByVal AccountID As String, ByVal name As String, ByVal Addr1 As String, ByVal Addr2 As String, ByVal city As String, ByVal state As String, ByVal zip As String, ByVal country As String)

        'This will update the address information for a company
        Try


            Dim ConnectionString As String = My.Settings.ConnectionString.ToString

            'RefType = 9 is the billing address for the company - Not sure if it's guaranteed to exist or not.
            Dim sqlStatement As String = _
            "update Addresses set Addr1=@Addr1, Addr2=@Addr2, City=@City, State=@State, Zip=@Zip, Country=@Country, datemodified=Getutcdate() where RefID = @OrgID and RefType = 9"


            Dim connection As New SqlConnection(ConnectionString)

            Dim command As New SqlCommand(sqlStatement, connection)

            command.Parameters.Add(New SqlParameter("@Addr1", Addr1))
            command.Parameters.Add(New SqlParameter("@Addr2", Addr2))
            command.Parameters.Add(New SqlParameter("@City", city))
            command.Parameters.Add(New SqlParameter("@State", state))
            command.Parameters.Add(New SqlParameter("@Zip", zip))
            command.Parameters.Add(New SqlParameter("@Country", country))
            command.Parameters.Add(New SqlParameter("@OrgID", GetOrgIDFromCRMLinkID(AccountID)))


            command.Connection.Open()

            command.ExecuteNonQuery()
            command.Parameters.Clear()

            command.Connection.Close()
            command.Dispose()


            If connection.State = ConnectionState.Open Then connection.Close()

            Form1.AddText("Address information updated.")
        Catch ex As Exception
            Form1.AddText("Error in UpdateTSAddressInfo.  Error = " + ex.Message)
        End Try
    End Sub


    Public Sub UpdateOrgPhoneNumber(ByVal PhoneID As String, ByVal PhoneNumber As String)

        'Updatest the phone number of an organization
        Try


            Form1.AddDebugText("Updating Org Phone Number. Number = " + PhoneNumber)

            Dim ConnectionString As String = My.Settings.ConnectionString.ToString

            'RefType = 9 is the billing address for the company - Not sure if it's guaranteed to exist or not.
            Dim sqlStatement As String = _
            "update PhoneNumbers set PhoneNumber=@PhoneNumber, datemodified=Getutcdate() where PhoneID = @PhoneID"


            Dim connection As New SqlConnection(ConnectionString)

            Dim command As New SqlCommand(sqlStatement, connection)

            command.Parameters.Add(New SqlParameter("@PhoneNumber", PhoneNumber))
            command.Parameters.Add(New SqlParameter("@PhoneID", PhoneID))


            command.Connection.Open()

            command.ExecuteNonQuery()
            command.Parameters.Clear()

            command.Connection.Close()
            command.Dispose()


            If connection.State = ConnectionState.Open Then connection.Close()

            Form1.AddText("Organization phone number updated.")
        Catch ex As Exception
            Form1.AddText("Error in UpdateOrgPhoneNumber: " + ex.Message)
        End Try
    End Sub

    Public Sub UpdateTSContactInfo(ByVal OrganizationID As String, ByVal email As String, ByVal FirstName As String, ByVal LastName As String, ByVal title As String)

        'This will update the address information for a company
        Try


            Dim ConnectionString As String = My.Settings.ConnectionString.ToString

            'RefType = 9 is the billing address for the company - Not sure if it's guaranteed to exist or not.
            Dim sqlStatement As String = _
            "update Users set FirstName=@FirstName, LastName=@LastName, Title=@Title, MarkDeleted=0 where email = @email and OrganizationID = @OrganizationID"


            Dim connection As New SqlConnection(ConnectionString)

            Dim command As New SqlCommand(sqlStatement, connection)

            command.Parameters.Add(New SqlParameter("@FirstName", FirstName))
            command.Parameters.Add(New SqlParameter("@LastName", LastName))
            command.Parameters.Add(New SqlParameter("@Title", title))
            command.Parameters.Add(New SqlParameter("@email", email))
            command.Parameters.Add(New SqlParameter("@OrganizationID", OrganizationID))


            command.Connection.Open()

            command.ExecuteNonQuery()

            command.Connection.Close()
            command.Parameters.Clear()
            command.Dispose()

            If connection.State = ConnectionState.Open Then connection.Close()

            Form1.AddText("Contact information updated.")
        Catch ex As Exception
            Form1.AddText("Error in UpdateTSContactInfo. Error = " + ex.Message)
        End Try
    End Sub


    Public Sub UpdateContactInfoIfNeeded(ByVal OrganizationID As String, ByVal email As String, ByVal Firstname As String, ByVal LastName As String, ByVal title As String, ByVal isdeleted As String)
        'This will pull the info from the TS database and compare it to the info in the CRM and will update if needed
        Try


            Dim ConnectionString As String = My.Settings.ConnectionString.ToString

            'RefType = 9 is the billing address for the company - Not sure if it's guaranteed to exist or not.
            Dim sqlStatement As String = _
            "Select FirstName, LastName, title from Users where email = @email and OrganizationID = @OrgID"


            Dim connection As New SqlConnection(ConnectionString)

            Dim command As New SqlCommand(sqlStatement, connection)

            command.Parameters.Add(New SqlParameter("@OrgID", OrganizationID))
            command.Parameters.Add(New SqlParameter("@email", email))


            command.Connection.Open()

            Dim reader As SqlDataReader = command.ExecuteReader


            If reader.Read Then
                'If we have found an address record then see if all the fields match.  If they don't, then update as needed
                If (reader(0).ToString <> Firstname) Or (reader(1).ToString <> LastName) Or (reader(2).ToString <> title) Then
                    'If any of the fields do not match then update the entire address record
                    UpdateTSContactInfo(OrganizationID, email, Firstname, LastName, title)
                End If

            End If

            reader.Close()
            command.Parameters.Clear()

            command.Connection.Close()
            command.Dispose()

            If connection.State = ConnectionState.Open Then connection.Close()
        Catch ex As Exception
            Form1.AddText("Error in UpdateContactInfoIfNeeded.  Error = " + ex.Message)
        End Try

    End Sub




    Public Sub AddTSAddressInfo(ByVal AccountID As String, ByVal name As String, ByVal Addr1 As String, ByVal Addr2 As String, ByVal city As String, ByVal state As String, ByVal zip As String, ByVal country As String)

        'This will add an address record for the company
        Try


            Dim ConnectionString As String = My.Settings.ConnectionString.ToString

            'RefType = 9 is the billing address for the company - Not sure if it's guaranteed to exist or not.
            Dim sqlStatement As String = _
            "insert into Addresses(Description, Addr1, Addr2, City, State, Zip, Country, RefID, RefType, DateCreated, DateModified, CreatorID, ModifierID) " & _
            "values('Shipping Address', @Addr1, @Addr2, @City, @State, @Zip, @Country, @OrgID, 9, GetUTCDate(), GetUTCDate(), -1, -1)"


            Dim connection As New SqlConnection(ConnectionString)

            Dim command As New SqlCommand(sqlStatement, connection)

            command.Parameters.Add(New SqlParameter("@Addr1", Addr1))
            command.Parameters.Add(New SqlParameter("@Addr2", Addr2))
            command.Parameters.Add(New SqlParameter("@City", city))
            command.Parameters.Add(New SqlParameter("@State", state))
            command.Parameters.Add(New SqlParameter("@Zip", zip))
            command.Parameters.Add(New SqlParameter("@Country", country))
            command.Parameters.Add(New SqlParameter("@OrgID", GetOrgIDFromCRMLinkID(AccountID)))


            command.Connection.Open()

            command.ExecuteNonQuery()
            command.Parameters.Clear()

            command.Connection.Close()
            command.Dispose()


            If connection.State = ConnectionState.Open Then connection.Close()
            connection.Dispose()

            Form1.AddText("Address information added.")
        Catch ex As Exception
            Form1.AddText("Error in AddTSAddressInfo.  Error = " + ex.Message)
        End Try
    End Sub

    Public Sub AddOrgPhoneNumber(ByVal ParentOrgID As String, ByVal AccountID As String, ByVal PhoneNumber As String)

        'This will add an address record for the company
        Try


            Form1.AddDebugText("Adding an Org Phone Number.  ParentOrgID = " + ParentOrgID)

            Dim FirstPhoneID = GetFirstOrgPhoneID(ParentOrgID)
            Form1.AddDebugText("Appears there are no phone IDs to add, so we're not going to try to add a phone number.")
            If FirstPhoneID = -1 Then

            Else


                Form1.AddDebugText("First Phone ID = " + FirstPhoneID.ToString)

                Form1.AddDebugText("Phone Number = " + PhoneNumber)

                Dim ConnectionString As String = My.Settings.ConnectionString.ToString

                'RefType = 9 is an organizaton's phone number (as opposed to an individuals)
                Dim sqlStatement As String = _
                "insert into PhoneNumbers(PhoneTypeID, RefID, RefType, PhoneNumber, DateCreated, DateModified, CreatorID, ModifierID)" & _
                "values(@FirstPhoneID, @OrganizationID, 9, @PhoneNumber, GetUTCDate(), GetUTCDate(), -1, -1)"


                Dim connection As New SqlConnection(ConnectionString)

                Dim command As New SqlCommand(sqlStatement, connection)

                command.Parameters.Add(New SqlParameter("@FirstPhoneID", FirstPhoneID))
                command.Parameters.Add(New SqlParameter("@OrganizationID", GetOrgIDFromCRMLinkID(AccountID)))
                command.Parameters.Add(New SqlParameter("@PhoneNumber", PhoneNumber))



                command.Connection.Open()

                command.ExecuteNonQuery()
                command.Parameters.Clear()

                command.Connection.Close()
                command.Dispose()


                If connection.State = ConnectionState.Open Then connection.Close()
                connection.Dispose()

                Form1.AddText("Phone information added.")
            End If
        Catch ex As Exception
            Form1.AddText("Error in AddOrgPhoneNumber.  Error = " + ex.Message)
        End Try
    End Sub

    Public Function GetFirstOrgPhoneID(ByVal OrgID As String) As String
        ' Get UserID from email and account id
        'This will get the OrgID if there is a matching name

        Form1.AddDebugText("Getting first Org Phone ID...")

        Dim ConnectionString As String = My.Settings.ConnectionString.ToString

        Dim sqlStatement As String = "select top 1 PhoneTypeID from phonetypes where organizationid=@OrgID order by position"

        Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@OrgID", OrgID))

        command.Connection.Open()

        Dim reader As SqlDataReader = command.ExecuteReader
        Try


            'return the PhoneType
            If reader.Read Then

                Return reader(0).ToString
            Else
                Return -1  '-1 if we can't find the contact
            End If

        Finally

            command.Parameters.Clear()

            command.Connection.Close()
            command.Dispose()

            reader.Close()

            If connection.State = ConnectionState.Open Then connection.Close()
        End Try

    End Function

    Public Function GetPhoneID(ByVal email As String, ByVal PhoneType As String) As String
        ' Get UserID from email and account id
        'This will get the OrgID if there is a matching name

        Dim ConnectionString As String = My.Settings.ConnectionString.ToString

        Dim sqlStatement As String = "select PhoneID, PhoneNumber, email from phonenumbers, phonetypes, users where reftype = 22 and phonenumbers.phonetypeid = phonetypes.phonetypeid and phonenumbers.refid = users.userid and phonetypes.name = @PhoneType  and email = @email"
        Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@PhoneType", "mobile"))
        command.Parameters.Add(New SqlParameter("@email", email))

        command.Connection.Open()

        Dim reader As SqlDataReader = command.ExecuteReader
        Try


            'return the OrgID
            If reader.Read Then
                Return True
            Else
                Return False  '-1 if we can't find the contact
            End If

        Finally

            command.Parameters.Clear()

            command.Connection.Close()
            command.Dispose()

            reader.Close()

            If connection.State = ConnectionState.Open Then connection.Close()
        End Try

    End Function

    Public Sub UpdatePhone(ByVal PhoneID As String, ByVal PhoneNumber As String)
        'Update the phone number based on the PhoneID
        Try


            Dim ConnectionString As String = My.Settings.ConnectionString.ToString

            If PhoneNumber = "" Then PhoneNumber = "None"

            'RefType = 9 is the billing address for the company - Not sure if it's guaranteed to exist or not.
            Dim sqlStatement As String = _
            "update phonenumbers set PhoneNumber = @PhoneNumber, Extension = '' where phoneid = @PhoneID"

            '"insert into Addresses(Description, Addr1, Addr2, City, State, Zip, Country, RefID, RefType, DateCreated, DateModified, CreatorID, ModifierID) " & _
            '"values('Shipping Address', @Addr1, @Addr2, @City, @State, @Zip, @Country, @OrgID, 9, GetUTCDate(), GetUTCDate(), -1, -1)"


            Dim connection As New SqlConnection(ConnectionString)

            Dim command As New SqlCommand(sqlStatement, connection)

            command.Parameters.Add(New SqlParameter("@PhoneNumber", PhoneNumber))
            command.Parameters.Add(New SqlParameter("@PhoneID", PhoneID))

            command.Connection.Open()

            command.ExecuteNonQuery()
            command.Parameters.Clear()

            command.Connection.Close()
            command.Dispose()

            If connection.State = ConnectionState.Open Then connection.Close()
            Form1.AddDebugText("UpdatePhone.  @PhoneNumber =" + PhoneNumber.ToString + ", @PhoneID =" + PhoneID.ToString)
            Form1.AddText("Phone information updated.")
        Catch ex As Exception
            'Note:  This is a trapped exception, so the problem will NOT cause the LastSync not to be updated.
            Form1.AddText("Error adding phone number: " + ex.Message)
        End Try
    End Sub


    Public Function GetPhoneType(ByVal ParentOrgID As String, ByVal PhoneName As String) As String
        Dim ConnectionString As String = My.Settings.ConnectionString.ToString

        Dim sqlStatement As String = "select PhoneTypeID from phonetypes where organizationid = @OrgID and name = @PhoneName"
        Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@PhoneName", PhoneName))
        command.Parameters.Add(New SqlParameter("@OrgID", ParentOrgID))

        command.Connection.Open()

        Dim reader As SqlDataReader = command.ExecuteReader

        Try
            'return the OrgID
            If reader.Read Then
                Return reader(0).ToString
            Else
                Return "-1"
            End If
        Finally

            command.Parameters.Clear()

            command.Connection.Close()
            command.Dispose()

            reader.Close()

            If connection.State = ConnectionState.Open Then connection.Close()
        End Try

    End Function

    Public Sub AddPhoneNumber(ByVal OrgID As String, ByVal email As String, ByVal PhoneType As Integer, ByVal PhoneNumber As String)
        Try


            Dim ConnectionString As String = My.Settings.ConnectionString.ToString

            Dim sqlStatement As String = "Insert into PhoneNumbers(RefID, PhoneTypeID, RefType, PhoneNumber, DateCreated, DateModified, CreatorID, ModifierID, Extension) Values(@refID, @PhoneTypeID, @RefType, @PhoneNumber, GetUTCDate(), GetUTCDate(), -1, -1, '')"
            Dim connection As New SqlConnection(ConnectionString)

            Dim command As New SqlCommand(sqlStatement, connection)
            command.Parameters.Add(New SqlParameter("@RefID", GetUserID(email, OrgID)))
            command.Parameters.Add(New SqlParameter("@RefType", 22)) '22=users
            command.Parameters.Add(New SqlParameter("@PhoneNumber", PhoneNumber))
            command.Parameters.Add(New SqlParameter("@PhoneTypeID", PhoneType))


            command.Connection.Open()
            command.ExecuteNonQuery()
            command.Parameters.Clear()

            command.Connection.Close()
            command.Dispose()

            If connection.State = ConnectionState.Open Then connection.Close()

            Form1.AddText("Phone number added.")
        Catch ex As Exception
            Form1.AddText("No phone number to add!") '10-27-09
        End Try
    End Sub



    Public Sub UpdatePhoneIfNeeded(ByVal OrgID As String, ByVal email As String, ByVal Phone As String, ByVal PhoneTypeString As String, ByVal ParentOrgID As String)
        ' Get UserID from email and account id
        'This will get the OrgID if there is a matching name

        Form1.AddDebugText("In UpdatePhoneIfNeeded")

        Dim ConnectionString As String = My.Settings.ConnectionString.ToString

        Dim sqlStatement As String = "select PhoneID, PhoneNumber, email from phonenumbers, phonetypes, users where reftype = 22 and phonenumbers.phonetypeid = phonetypes.phonetypeid and phonenumbers.refid = users.userid and phonetypes.name = @PhoneType  and email = @email and users.organizationid = @orgid"
        Dim connection As New SqlConnection(ConnectionString)
        Dim command As New SqlCommand(sqlStatement, connection)
        Try


            command.Parameters.Add(New SqlParameter("@PhoneType", PhoneTypeString))
            command.Parameters.Add(New SqlParameter("@email", email))
            command.Parameters.Add(New SqlParameter("@orgid", OrgID))

            command.Connection.Open()

            Dim reader As SqlDataReader = command.ExecuteReader

            'return the OrgID
            If reader.Read Then
                If reader(1).ToString <> Phone Then
                    Form1.AddDebugText("Updating phone number...")
                    UpdatePhone(reader(0).ToString, Phone)
                End If
            Else
                Form1.AddDebugText("Adding a new phone number...")
                AddPhoneNumber(OrgID, email, GetPhoneType(ParentOrgID, PhoneTypeString), Phone)
            End If

            reader.Close()

        Finally
            command.Connection.Close()
            command.Dispose()


            If connection.State = ConnectionState.Open Then connection.Close()


        End Try
        Form1.AddDebugText("Completed UpdatePhoneIfNeeded")
    End Sub


    Public Shared Function GetLastCRMUpdate(ByVal ParentOrgID As String, ByVal Integration As IntegrationType) As String
        'Gets the date/time (in UTC) the last link was done

        Dim sqlStatement As String = "select LastLink from CRMLinkTable where organizationid = @ParentOrgID and CRMType = @type"
        Dim connection As New SqlConnection(CS)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.AddWithValue("@ParentOrgID", ParentOrgID)
        command.Parameters.AddWithValue("@type", Integration.ToString())

        command.Connection.Open()

        Dim reader As SqlDataReader = command.ExecuteReader

        Try
            'return the OrgID
            If reader.Read Then
                If reader(0).ToString <> "" Then
                    Return reader(0).ToString
                Else
                    Return "1970-01-01 08:00:00.000" 'January 1970 should be before any tickets were created!
                End If

            Else
                Return DateSerial(1970, 1, 1)
            End If
        Finally

            command.Parameters.Clear()

            command.Connection.Close()
            command.Dispose()

            reader.Close()

            If connection.State = ConnectionState.Open Then connection.Close()
        End Try

    End Function

    Public Shared Sub UpdateLastLinkDateTime(ByVal OrgID As String, ByVal Integration As IntegrationType)
        Try
            Dim sqlStatement As String = "Update CRMLinkTable set LastLink = GetUTCDate() where OrganizationID = @OrgID and CRMType = @type"
            Dim connection As New SqlConnection(CS)

            Dim command As New SqlCommand(sqlStatement, connection)
            command.Parameters.AddWithValue("@OrgID", OrgID)
            command.Parameters.AddWithValue("@type", Integration.ToString())

            command.Connection.Open()
            command.ExecuteNonQuery()
            command.Parameters.Clear()

            command.Connection.Close()

            command.Dispose()

            If connection.State = ConnectionState.Open Then connection.Close()
        Catch ex As Exception
            Form1.AddText("Error in UpdateLastLinkDateTime.  Message = " + ex.Message)
        End Try
    End Sub

    Public Sub PublicAddText(ByRef TextStuff As String, ByVal FormName As TextBox)
        FormName.Text = FormName.Text + ControlChars.CrLf + Now.ToString + ": " + TextStuff
        Application.DoEvents()
    End Sub


    Public Sub AddOrUpdateAccountInformation(ByVal CRMLinkID As String, ByVal name As String, ByVal Addr1 As String, ByVal Addr2 As String, ByVal city As String, ByVal state As String, ByVal zip As String, ByVal country As String, ByVal PhoneNumber As String, ByVal ParentOrganizationID As String)
        '**Main routine called by SalesForce**

        'This will add an account if it doesn't exit in TS or update it if it does
        Try


            If Addr1 = Nothing Then Addr1 = ""
            If Addr2 = Nothing Then Addr2 = ""
            If city = Nothing Then city = ""
            If state = Nothing Then state = ""
            If zip = Nothing Then zip = ""
            If country = Nothing Then country = ""



            If GetOrgIDFromCRMLinkID(CRMLinkID) = "-1" Then 'Check to see if the CRMLinkID exists
                'The account does not exist
                If MatchAccountName(CRMLinkID, Trim(name), ParentOrganizationID) = "-1" Then
                    'we can't find a matching CRMLinkID or matching name...Let's add the account
                    AddNewAccount(CRMLinkID, name, ParentOrganizationID)
                    'Now let's update the address information
                    UpdateAddressInfoIfNeeded(CRMLinkID, Trim(name), Addr1, Addr2, city, state, zip, country)
                    UpdateAccountPhoneIfNeeded(CRMLinkID, PhoneNumber, ParentOrganizationID)
                Else
                    'update accountid
                    UpdateCRMLinkID(CRMLinkID, MatchAccountName(CRMLinkID, name, ParentOrganizationID))
                    'now lets go ahead and update the address information
                    UpdateAddressInfoIfNeeded(CRMLinkID, name, Addr1, Addr2, city, state, zip, country)
                    UpdateAccountPhoneIfNeeded(CRMLinkID, PhoneNumber, ParentOrganizationID)
                End If

            Else
                'The account exists - Let's check the address information
                UpdateAddressInfoIfNeeded(CRMLinkID, name, Addr1, Addr2, city, state, zip, country)
                UpdateAccountPhoneIfNeeded(CRMLinkID, PhoneNumber, ParentOrganizationID)

                'Let's update the name on the account if it has changed.
                UpdateAccountNameIfNeeded(CRMLinkID, name)

            End If
        Catch ex As Exception
            Form1.AddText("Error in AddOrUpdateAccountInformation.  Message = " + ex.Message)
        End Try

    End Sub

    Public Sub AddOrUpdateContactInformation(ByVal CRMLinkID As String, ByVal email As String, ByVal FirstName As String, ByVal LastName As String, ByVal Phone As String, ByVal fax As String, ByVal mobilephone As String, ByVal title As String, ByVal isdeleted As String, ByVal ParentOrganizationID As String)
        '**Update the contact information

        'This will add an account if it doesn't exit in TS or update it if it does
        Try

            Form1.AddText("In AddOrUpdateContactInformation for " + email + " (" + LastName + ", " + FirstName + ")")

            Dim OrgID As String = GetOrgIDFromCRMLinkID(CRMLinkID)

            If OrgID <> "-1" Then
                'We can't do anything if we don't know what the contact's Account ID is.

                If email <> "" Then
                    'we have to have an e-mail address since that's the link we use.  Simply ignore contact if it doesn't have an e-mail

                    'Note:  Since we added the ability to sync a user (11/18) without having an e-mail, there will be a problem here if we do end up adding an e-mail for them...

                    If Not MatchContactEmail(OrgID, email) Then
                        'add the contact
                        AddContact(OrgID, email, FirstName, LastName, Phone, title, isdeleted)
                        'Now that we've created the contact, we need to add the phone information
                        UpdatePhoneIfNeeded(OrgID, email, Phone, "Work", ParentOrganizationID)
                        UpdatePhoneIfNeeded(OrgID, email, mobilephone, "Mobile", ParentOrganizationID)
                        UpdatePhoneIfNeeded(OrgID, email, fax, "Fax", ParentOrganizationID)

                    Else
                        'we've found the contact - Let's update the data
                        UpdateContactInfoIfNeeded(OrgID, email, FirstName, LastName, title, isdeleted)
                        UpdatePhoneIfNeeded(OrgID, email, Phone, "Work", ParentOrganizationID)
                        UpdatePhoneIfNeeded(OrgID, email, mobilephone, "Mobile", ParentOrganizationID)
                        UpdatePhoneIfNeeded(OrgID, email, fax, "Fax", ParentOrganizationID)


                    End If


                End If

            End If
        Catch ex As Exception
            Form1.AddText("Error in AddOrUpdateContactInformation.  Error = " + ex.Message)
        End Try
    End Sub

    Public Function ConvertDateTimeToHR(ByVal ADateTime As Date) As String
        'returns a date varisable as yyyymmddhhmmss
        Return ADateTime.Year.ToString + ADateTime.Month.ToString.PadLeft(2, "0"c) + ADateTime.Day.ToString.PadLeft(2, "0"c) + ADateTime.Hour.ToString.PadLeft(2, "0"c) + ADateTime.Minute.ToString.PadLeft(2, "0"c) + ADateTime.Second.ToString.PadLeft(2, "0"c)

    End Function


    Public Function IsOrgActive(ByVal ParentOrgID As String) As Boolean
        'Checks the IsActive flag in the organizations table

        Dim ConnectionString As String = My.Settings.ConnectionString.ToString

        Dim sqlStatement As String = "select IsActive from Organizations where organizationid = @ParentOrgID"
        Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@ParentOrgID", ParentOrgID))

        command.Connection.Open()

        Dim reader As SqlDataReader = command.ExecuteReader

        Try
            'return the OrgID
            If reader.Read Then

                Return reader(0).ToString = "True"
            Else
                Return False
            End If


        Finally

            command.Parameters.Clear()

            command.Connection.Close()
            command.Dispose()

            reader.Close()

            If connection.State = ConnectionState.Open Then connection.Close()
        End Try

    End Function

    Public Sub WriteToCRMResults(ByVal OrgID As String, ByVal Results As String)

        Try


            Dim ConnectionString As String = My.Settings.ConnectionString.ToString

            Dim sqlStatement As String = _
            "insert into  CRMLinkResults(OrganizationID, AttemptDateTime, AttemptResult) " & _
            "Values(@OrgID, GetUTCDate(), @Result)"



            Dim connection As New SqlConnection(ConnectionString)

            Dim command As New SqlCommand(sqlStatement, connection)

            command.Parameters.Add(New SqlParameter("@OrgID", OrgID))
            command.Parameters.Add(New SqlParameter("@Result", Results))


            command.Connection.Open()

            command.ExecuteNonQuery()

            command.Connection.Close()

            command.Dispose()

            connection.Close()
        Catch ex As Exception
            Form1.AddText("Error in WriteToCRMResults.  Message = " + ex.Message)
        End Try

    End Sub

    Public Shared Function GetLatestTickets(ByVal organizationID As String, ByVal Integration As IntegrationType) As DataTable
        Dim tickets As New DataTable()

        Dim ConnectionString As String = My.Settings.ConnectionString.ToString

        'get tickets which were created after the last link date
        Dim sqlstatement As String = "select t.ticketid, t.name, o.organizationid, o.crmlinkid, a.description " & _
            "from tickets t " & _
            "inner join organizationtickets ot on t.ticketid = ot.ticketid " & _
            "inner join organizations o on o.organizationid = ot.organizationid " & _
            "left outer join (select a.ticketid, description from actions a " & _
            "inner join (select ticketid, min(datecreated) as 'datecreated' from actions group by ticketid) m on m.datecreated = a.datecreated and m.ticketid = a.ticketid) a on a.ticketid = t.ticketid " & _
            "where t.organizationid = @orgid and isnull(crmlinkid, '') <> '' and t.datecreated > @LastLinkDate order by t.datecreated desc"
        'changed query to sort tickets by datecreated (descending) 8/20/09

        Using connection As New SqlConnection(ConnectionString)

            Dim command As New SqlCommand(sqlstatement, connection)
            command.Parameters.Add(New SqlParameter("@OrgID", organizationID))
            command.Parameters.Add(New SqlParameter("@LastLinkDate", GetLastCRMUpdate(organizationID, Integration)))
            connection.Open()

            Using transaction As SqlTransaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted)
                command.Transaction = transaction

                Try
                    Using da As New SqlDataAdapter(command)
                        da.Fill(tickets)
                    End Using

                    transaction.Commit()

                Catch ex As Exception
                    transaction.Rollback()
                    ex.Data("CommandText") = command.CommandText

                    'have to do this for now to avoid rewriting WriteToCRMResults as shared
                    Dim TS As New TSCheckData()
                    TS.WriteToCRMResults(organizationID, "Error in sending ticket data: " + ex.ToString)

                    Return Nothing
                End Try
            End Using


        End Using

        Return tickets
    End Function

    Public Function GetTimeZone(ByRef OrganizationID As String) As String
        'This will return the Font Family stored in Portal Options

        Dim sqlStatement As String = "select TimeZoneID from Organizations where organizationid = @orgid"

        Dim ConnectionString As String = My.Settings.ConnectionString.ToString
        Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@OrgID", OrganizationID))

        command.Connection.Open()
        Try



            Dim reader As SqlDataReader = command.ExecuteReader


            'return the string
            Try


                Try

                    If reader.Read Then
                        Return reader(0)
                    Else
                        Return "Central Standard Time"
                    End If
                Catch ex As Exception
                    Return "Central Standard Time"
                End Try

            Finally
                command.Connection.Close()
                connection.Close()
                reader.Close()
            End Try

            command.Connection.Close()
        Catch ex As Exception
            Return True 'use recaptcha if there's an error
        End Try
    End Function


    Public Function ConvertToLocalTime(ByVal dt As DateTime, ByVal OrgID As String, ByVal timezone As String)
        Dim LocalTime As DateTime


        Dim TZ As System.TimeZoneInfo

        Try
            TZ = TimeZoneInfo.FindSystemTimeZoneById(timezone)
        Catch ex As Exception
            TZ = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")
        End Try

        LocalTime = TimeZoneInfo.ConvertTimeFromUtc(dt, TZ)

        Return LocalTime
    End Function


    'These are used for the custom salesforce stuff we are doing for Axceler
    Public Function GetProductIDFromName(ByVal OrganizationID As String, ByVal ProductName As String) As String
        'This will return the Product ID if there is a string match from the product name

        Dim sqlStatement As String = "select ProductID from products where name = @ProductName and organizationid=@OrgID"

        Dim ConnectionString As String = My.Settings.ConnectionString.ToString
        Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@OrgID", OrganizationID))
        command.Parameters.Add(New SqlParameter("@ProductName", ProductName))


        command.Connection.Open()
        Try



            Dim reader As SqlDataReader = command.ExecuteReader


            'return the string
            Try


                Try

                    If reader.Read Then
                        Return reader(0)
                    Else
                        Return ""
                    End If
                Catch ex As Exception
                    Return "Central Standard Time"
                End Try

            Finally
                command.Connection.Close()
                connection.Close()
                reader.Close()
            End Try

            command.Connection.Close()
        Catch ex As Exception
            Return "" '
        End Try
    End Function


    Public Function GetOrganizationProductID(ByVal ClientOrgID As String, ByVal ProductID As String) As String
        'This will return the Product ID if there is a string match from the product name

        Dim sqlStatement As String = "select OrganizationProductID from OrganizationProducts where Organizationid = @ClientOrgID and productid=@ProductID"

        Dim ConnectionString As String = My.Settings.ConnectionString.ToString
        Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@ClientOrgID", ClientOrgID))
        command.Parameters.Add(New SqlParameter("@ProductID", ProductID))


        command.Connection.Open()
        Try



            Dim reader As SqlDataReader = command.ExecuteReader


            'return the string
            Try


                Try

                    If reader.Read Then
                        Return reader(0)
                    Else
                        Return ""
                    End If
                Catch ex As Exception
                    Return "Central Standard Time"
                End Try

            Finally
                command.Connection.Close()
                connection.Close()
                reader.Close()
            End Try

            command.Connection.Close()
        Catch ex As Exception
            Return "" '
        End Try
    End Function


    Public Sub AddProductToOrganization(ByVal ClientOrgID As String, ByVal ProductID As String)
        'Adds a product id to a client organization
        Try


            Form1.AddText("Adding product to the organization.")

            Dim sqlStatement As String = "Insert into OrganizationProducts (OrganizationID, ProductID, IsVisibleOnPortal, DateCreated, DateModified, CreatorID, ModifierID) Values(@ClientOrgID, @ProductID, 0, getutcdate(), getutcdate(), 47, 47)"

            Dim ConnectionString As String = My.Settings.ConnectionString.ToString
            Dim connection As New SqlConnection(ConnectionString)

            Dim command As New SqlCommand(sqlStatement, connection)
            command.Parameters.Add(New SqlParameter("@ClientOrgID", ClientOrgID))
            command.Parameters.Add(New SqlParameter("@ProductID", ProductID))

            command.Connection.Open()

            command.ExecuteNonQuery()
            command.Parameters.Clear()

            command.Connection.Close()
            command.Dispose()

        Catch ex As Exception
            Form1.AddText("Error in AddProductToOrganization. Error = " + ex.Message)
        End Try
    End Sub

    Public Function DoesCustomValueExist(ByVal CustomFieldID As String, ByVal RefID As String) As Boolean
        'This will return True if this custom value already exists

        Dim sqlStatement As String = "select CustomValueID from CustomValues where CustomFieldID = @CustomFieldID and refid=@RefID"

        Dim ConnectionString As String = My.Settings.ConnectionString.ToString
        Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@CustomFieldID", CustomFieldID))
        command.Parameters.Add(New SqlParameter("@RefID", RefID))


        command.Connection.Open()
        Try



            Dim reader As SqlDataReader = command.ExecuteReader


            'return the string
            Try


                Try

                    If reader.Read Then
                        Return True
                    Else
                        Return False
                    End If
                Catch ex As Exception
                    Return False
                End Try

            Finally
                command.Connection.Close()
                connection.Close()
                reader.Close()
            End Try

            command.Connection.Close()
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Sub UpdateCustomValue(ByVal CustomValue As String, ByVal CustomFieldID As String, ByVal OrganizationProductID As String, ByVal ClientOrgID As String)
        'Updates and/or creates a custom value associated with the origanization's product

        Try




            If Not DoesCustomValueExist(CustomFieldID, OrganizationProductID) Then
                'We need to add this field
                'Try
                Form1.AddText("In UpdateCustomValue - Custom Value does NOT exist")

                Dim sqlStatement As String = "Insert into CustomValues (CustomFieldID, RefID, CustomValue, DateCreated, DateModified, CreatorID, ModifierID) Values(@CustomFieldID, @OrganizationProductID, @CustomValue, getutcdate(), getutcdate(),47,47)"

                Dim ConnectionString As String = My.Settings.ConnectionString.ToString
                Dim connection As New SqlConnection(ConnectionString)

                Dim command As New SqlCommand(sqlStatement, connection)
                command.Parameters.Add(New SqlParameter("@CustomFieldID", CustomFieldID))
                command.Parameters.Add(New SqlParameter("@OrganizationProductID", OrganizationProductID))
                command.Parameters.Add(New SqlParameter("@CustomValue", CustomValue))

                command.Connection.Open()

                command.ExecuteNonQuery()
                command.Parameters.Clear()

                command.Connection.Close()
                command.Dispose()
                'Catch ex As Exception

                ' End Try
            Else

                Form1.AddText("In UpdateCustomValue - Custom DOES exist")

                'just update it
                Dim sqlStatement As String = "Update CustomValues set CustomValue=@CustomValue, DateModified=Getutcdate(), ModifierID=47 where RefID=@OrganizationProductID and CustomFieldID = @CustomFieldID"

                Dim ConnectionString As String = My.Settings.ConnectionString.ToString
                Dim connection As New SqlConnection(ConnectionString)

                Dim command As New SqlCommand(sqlStatement, connection)
                'command.Parameters.Add(New SqlParameter("@ClientOrgID", ClientOrgID))
                command.Parameters.Add(New SqlParameter("@OrganizationProductID", OrganizationProductID))
                command.Parameters.Add(New SqlParameter("@CustomValue", CustomValue))
                command.Parameters.Add(New SqlParameter("@CustomFieldID", CustomFieldID))

                command.Connection.Open()

                command.ExecuteNonQuery()
                command.Parameters.Clear()

                command.Connection.Close()
                command.Dispose()


            End If

        Catch ex As Exception
            Form1.AddText("Error in UpdateCustomValue.  Message = " + ex.Message)
        End Try
    End Sub

    Public Sub UpdateSupportExpiration(ByVal OrganizationProductID As String, ByVal SupportExpiration As String)
        'Updates and/or creates a custom value associated with the origanization's product
        Try



            Form1.AddText("In UpdateSupportExpiration")

            'just update it
            Dim sqlStatement As String = "Update OrganizationProducts set SupportExpiration=@SupportExpiration, DateModified=Getutcdate(), ModifierID=47 where OrganizationProductID=@OrganizationProductID"

            Dim ConnectionString As String = My.Settings.ConnectionString.ToString
            Dim connection As New SqlConnection(ConnectionString)

            Dim command As New SqlCommand(sqlStatement, connection)
            'command.Parameters.Add(New SqlParameter("@ClientOrgID", ClientOrgID))
            command.Parameters.Add(New SqlParameter("@SupportExpiration", SupportExpiration))
            command.Parameters.Add(New SqlParameter("@OrganizationProductID", OrganizationProductID))

            command.Connection.Open()

            command.ExecuteNonQuery()
            command.Parameters.Clear()

            command.Connection.Close()
            command.Dispose()

        Catch ex As Exception
            Form1.AddText("Error in UpdateSupportExpiration.  Message = " + ex.Message)
        End Try

    End Sub


    Public Shared Function SendBackTicketData(ByRef OrganizationID As String, ByVal Integration As IntegrationType) As Boolean
        Dim sqlStatement As String = "select SendBackTicketData from CRMLinkTable where organizationid = @orgid and Active = 1 and CRMType = @type"

        Dim connection As New SqlConnection(CS)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.AddWithValue("@OrgID", OrganizationID)
        command.Parameters.AddWithValue("@type", Integration.ToString())

        command.Connection.Open()
        Try

            Dim reader As SqlDataReader = command.ExecuteReader


            'return the string
            Try


                Try

                    If reader.Read Then
                        Return reader(0)
                    Else
                        Return True
                    End If
                Catch ex As Exception
                    Return True
                End Try

            Finally
                command.Connection.Close()
                connection.Close()
                reader.Close()
            End Try

            command.Connection.Close()
        Catch ex As Exception
            Return True 'use recaptcha if there's an error
        End Try
    End Function

End Class

Public Enum IntegrationType
    SalesForce
    Highrise
    Batchbook
    FreshBooks
End Enum