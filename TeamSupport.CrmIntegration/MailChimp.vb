Imports TeamSupport.Data
Imports System.Xml
Imports System.Net
Imports System.Text

Namespace TeamSupport
    Namespace CrmIntegration

        Public Class MailChimp
            Inherits Integration

            Private datacenter As String

            Public Sub New(ByVal crmLinkOrg As CRMLinkTableItem, ByVal crmLog As SyncLog, ByVal thisUser As LoginUser, ByVal thisProcessor As CrmProcessor)
                MyBase.New(crmLinkOrg, crmLog, thisUser, thisProcessor, IntegrationType.MailChimp)

                'parse the api key to get datacenter for url
                datacenter = CRMLinkRow.SecurityToken.Substring(CRMLinkRow.SecurityToken.LastIndexOf("-") + 1)
            End Sub


            Public Overrides Function PerformSync() As Boolean
                'get the mailchimp list to sync to
                Dim listID As String = GetImportListID()

                If listID IsNot Nothing Then
                    Log.Write("list id: " & listID)
                    'get customer email addresses for this account
                    Dim theseCustomers As New Organizations(User)
                    theseCustomers.LoadByParentID(CRMLinkRow.OrganizationID, True)

                    Dim emailBatch As StringBuilder = Nothing
                    Dim emailIndex As Integer = 0

                    For Each customer As Organization In theseCustomers

                        Dim theseContacts As New Users(User)
                        theseContacts.LoadByOrganizationID(customer.OrganizationID, True)

                        For Each contact As User In theseContacts
                            If CRMLinkRow.LastLink Is Nothing Or contact.DateModified.AddMinutes(30) > CRMLinkRow.LastLink Then
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
                        Next
                    Next

                    If emailBatch IsNot Nothing Then
                        'send the list to mailchimp
                        If BatchSubscribe(emailBatch.ToString()) Then
                            Log.Write(emailIndex.ToString() & " email addresses sent to Mailchimp.")
                        Else
                            Log.Write("Error in BatchSubscribe.")
                            Return False
                        End If
                    Else
                        Log.Write("No new email addresses to sync.")
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

        End Class

    End Namespace
End Namespace