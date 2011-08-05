Imports TeamSupport.Data
Imports System.Xml
Imports System.Net
Imports System.Text

Namespace TeamSupport
    Namespace CrmIntegration

        Public Class MailChimp
            Inherits Integration

            Private Const BatchSize As Integer = 4000
            Private datacenter As String

            Public Sub New(ByVal crmLinkOrg As CRMLinkTableItem, ByVal crmLog As SyncLog, ByVal thisUser As LoginUser, ByVal thisProcessor As CrmProcessor)
                MyBase.New(crmLinkOrg, crmLog, thisUser, thisProcessor, IntegrationType.MailChimp)

                'parse the api key to get datacenter for url
                datacenter = CRMLinkRow.SecurityToken.Substring(CRMLinkRow.SecurityToken.LastIndexOf("-") + 1)
            End Sub


            Public Overrides Function PerformSync() As Boolean
                'get the mailchimp list to sync to
                Dim listID As String = GetImportListID()

                If Processor.IsStopped Then
                    Return False
                End If

                If listID IsNot Nothing Then
                    Log.Write("list id: " & listID)
                    'get customer email addresses for this account
                    Dim theseCustomers As New Organizations(User)
                    theseCustomers.LoadByParentID(CRMLinkRow.OrganizationID, False)

                    Dim emailBatches As New List(Of StringBuilder)()
                    Dim emailBatch As StringBuilder = Nothing
                    Dim unsubBatch As StringBuilder = Nothing
                    Dim emailIndex As Integer = 0
                    Dim unsubIndex As Integer = 0

                    For Each customer As Organization In theseCustomers

                        Dim theseContacts As New Users(User)
                        theseContacts.LoadByOrganizationID(customer.OrganizationID, False)

                        For Each contact As User In theseContacts
                            If Processor.IsStopped Then
                                Return False
                            End If

                            If contact.IsActive And customer.IsActive Then

                                If (CRMLinkRow.LastLink Is Nothing Or CType(contact.Row("DateModified"), DateTime).AddMinutes(30) > CRMLinkRow.LastLink _
                                    Or CType(customer.Row("DateModified"), DateTime).AddMinutes(30) > CRMLinkRow.LastLink) And Not contact.MarkDeleted Then
                                    If emailBatch Is Nothing Then
                                        emailBatch = New StringBuilder("&apikey=" & CRMLinkRow.SecurityToken _
                                                                       & "&id=" & listID _
                                                                       & "&double_optin=false")
                                    End If

                                    emailBatch.Append("&batch[" & emailIndex & "][EMAIL]=" & contact.Email)
                                    emailBatch.Append("&batch[" & emailIndex & "][EMAIL_TYPE]=html")
                                    emailBatch.Append("&batch[" & emailIndex & "][FNAME]=" & contact.FirstName)
                                    emailBatch.Append("&batch[" & emailIndex & "][LNAME]=" & contact.LastName)
                                    emailIndex += 1
                                End If

                                If emailIndex = BatchSize Then
                                    emailBatches.Add(emailBatch)

                                    emailIndex = 0
                                    emailBatch = Nothing
                                End If

                            Else
                                If CRMLinkRow.LastLink Is Nothing Or CType(contact.Row("DateModified"), DateTime).AddMinutes(30) > CRMLinkRow.LastLink _
                                    Or CType(customer.Row("DateModified"), DateTime).AddMinutes(30) > CRMLinkRow.LastLink Then

                                    If unsubBatch Is Nothing Then
                                        unsubBatch = New StringBuilder("&apikey=" & CRMLinkRow.SecurityToken _
                                                                       & "&id=" & listID _
                                                                       & "&send_goodbye=false")
                                    End If

                                    unsubBatch.Append("&emails[" & unsubIndex & "]=" & contact.Email)
                                    unsubIndex += 1
                                End If

                            End If
                        Next
                    Next

                    If emailBatch IsNot Nothing Then
                        emailBatches.Add(emailBatch)
                    End If

                    If emailBatches.Count > 0 Then
                        'send the lists to mailchimp
                        For Each mailList As StringBuilder In emailBatches
                            If Not BatchSubscribe(mailList.ToString()) Then
                                Log.Write("Error in BatchSubscribe.")
                                Return False
                            End If
                        Next

                        Log.Write("Addresses sent to Mailchimp in " & emailBatches.Count.ToString() & " batches.")
                    Else
                        Log.Write("No new email addresses to sync.")
                    End If

                    If unsubBatch IsNot Nothing Then
                        If Not BatchUnsubscribe(unsubBatch.ToString()) Then
                            Log.Write("Error in BatchUnsubscribe.")
                            Return False
                        End If

                        Log.Write("Unsubscribed " & unsubIndex.ToString() & " addresses.")
                    End If

                Else
                    LogSyncResult("Error: The specified list does not exist in MailChimp.")
                    Return False
                End If

                Return True
            End Function

            Private Function GetImportListID() As String
                Dim returnID As String = Nothing

                Dim Lists As XmlDocument = GetMailChimpXML("?method=lists&apikey=" & CRMLinkRow.SecurityToken)
                Dim ImportList As XElement = Nothing

                If Lists IsNot Nothing Then
                    Dim alllists As XElement = XElement.Load(New XmlNodeReader(Lists))

                    If alllists.Descendants("struct").Count > 0 Then
                        For Each thisList As XElement In alllists.Descendants("struct")
                            If Trim(thisList.Element("name").Value).ToLower() = Trim(CRMLinkRow.Username).ToLower() Then
                                ImportList = thisList
                                Exit For
                            End If
                        Next
                    End If

                    If ImportList IsNot Nothing Then
                        returnID = ImportList.Element("id").Value
                    End If
                End If

                Return returnID
            End Function

            Private Function GetMailChimpXML(ByVal PathAndQuery As String) As XmlDocument
                Dim MCUri As New Uri("https://" & datacenter & ".api.mailchimp.com/1.3/" & PathAndQuery & "&output=xml")

                Return GetXML(Nothing, MCUri)
            End Function

            Private Function BatchSubscribe(ByVal postString As String) As Boolean
                Dim MCUri As New Uri("https://" & datacenter & ".api.mailchimp.com/1.3/?method=listBatchSubscribe")
                Dim postData As String = "&output=xml" & postString

                Return PostQueryString(Nothing, MCUri, postData) = HttpStatusCode.OK
            End Function

            Private Function BatchUnsubscribe(ByVal postString As String) As Boolean
                Dim MCUri As New Uri("https://" & datacenter & ".api.mailchimp.com/1.3/?method=listBatchUnsubscribe")
                Dim postData As String = "&output=xml" & postString

                Return PostQueryString(Nothing, MCUri, postData) = HttpStatusCode.OK
            End Function
        End Class

    End Namespace
End Namespace