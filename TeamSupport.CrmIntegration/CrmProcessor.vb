Imports TeamSupport.Data
Imports TeamSupport.ServiceLibrary
Imports System.IO

Namespace TeamSupport
    Namespace CrmIntegration

        Public Class CrmProcessor
            Inherits ServiceThread

            Public Overrides ReadOnly Property ServiceName As String
                Get
                    Return "CrmProcessor"
                End Get
            End Property

            Public Overridable ReadOnly Property IsStopped() As Boolean
                Get
                    SyncLock Me
                        Return _isStopped Or ServiceStopped Or CrmPool.CrmPoolStopped
                    End SyncLock
                End Get
            End Property

            Private _crmLinkID As Integer
            Public ReadOnly Property CrmLinkID() As Integer
                Get
                    Dim result As Integer
                    SyncLock Me
                        result = _crmLinkID
                    End SyncLock
                    Return result
                End Get
            End Property

            Public Sub New(ByVal cmrLinkId As Integer)
                _crmLinkID = cmrLinkId
                IsLoop = False
            End Sub

            Public Overrides Sub Run()
                Dim CrmLinkRow As CRMLinkTableItem = CRMLinkTable.GetCRMLinkTableItem(LoginUser, _crmLinkID)

                Try
                    ProcessCrmLink(CrmLinkRow)
                    'System.Threading.Thread.Sleep(5000)

                Catch ex As Exception
                    ex.Data("CRMLinkID") = _crmLinkID
                    ExceptionLogs.LogException(LoginUser, ex, "Service - " & ServiceName, CrmLinkRow.Row)
                End Try

                'update last processed date/time
                CrmLinkRow.LastProcessed = DateTime.UtcNow
                CrmLinkRow.Collection.Save()

            End Sub

            ''' <summary>
            ''' Process a given integration
            ''' </summary>
            ''' <param name="CRMLinkTableItem">A row from CRMLinkTable, which maps to a single integration for a given company</param>
            ''' <remarks></remarks>
            Public Sub ProcessCrmLink(ByVal CRMLinkTableItem As CRMLinkTableItem)
                If IsStopped Then
                    Return
                End If

                Try
                    'CrmLinkTableItem.CRMType should match up with one of the items in the IntegrationType enumeration
                    Dim CRMType As IntegrationType = [Enum].Parse(GetType(IntegrationType), CRMLinkTableItem.CRMType, True)

                    'set up log per crm link item
                    Dim Log As New SyncLog(Path.Combine(Settings.ReadString("Log File Path", "C:\CrmLogs\"), CRMLinkTableItem.OrganizationID.ToString()), CRMType)
                    Dim CRM As Integration = Nothing

                    Select Case CRMType
                        Case IntegrationType.Batchbook
                            If Settings.ReadBool("BatchBook Enabled", True) Then
                                CRM = New BatchBook(CRMLinkTableItem, Log, LoginUser, Me)
                            Else
                                Return
                            End If
                        Case IntegrationType.Highrise
                            If Settings.ReadBool("Highrise Enabled", True) Then
                                CRM = New Highrise(CRMLinkTableItem, Log, LoginUser, Me)
                            Else
                                Return
                            End If
                        Case IntegrationType.SalesForce
                            If Settings.ReadBool("SalesForce Enabled", True) Then
                                CRM = New SalesForce(CRMLinkTableItem, Log, LoginUser, Me)
                            Else
                                Return
                            End If
                        Case IntegrationType.MailChimp
                            If Settings.ReadBool("MailChimpEnabled", True) Then
                                CRM = New MailChimp(CRMLinkTableItem, Log, LoginUser, Me)
                            Else
                                Return
                            End If
                        Case IntegrationType.ZohoCRM
                            If Settings.ReadBool("ZohoCRMEnabled", True) Then
                                CRM = New ZohoCRM(CRMLinkTableItem, Log, LoginUser, Me)
                            Else
                                Return
                            End If
                        Case IntegrationType.ZohoReports
                            If Settings.ReadBool("ZohoReportsEnabled", True) Then
                                CRM = New ZohoReports(CRMLinkTableItem, Log, LoginUser, Me)
                            Else
                                Return
                            End If
                    End Select

                    If CRM IsNot Nothing Then
                        Log.Write(String.Format("Begin processing {0} sync.", CRMType.ToString()))

                        Try
                            'if sync processed successfully, log that message. otherwise log an error
                            If CRM.PerformSync() Then
                                CRMLinkTableItem.LastLink = DateTime.UtcNow
                                CRMLinkTableItem.Collection.Save()
                                Log.Write("Finished processing successfully.")

                                Integration.LogSyncResult(CRMType.ToString() & " Sync Completed", CRMLinkTableItem.OrganizationID, LoginUser)
                            Else
                                'migrating towards using IntegrationException instead of IntegrationError
                                If CRM.Exception IsNot Nothing Then
                                    CRM.LogSyncResult(String.Format("Error reported in {0} sync: {1}", CRMType.ToString(), CRM.Exception.Message))
                                    Log.Write(String.Format("Error reported in {0} sync: {1}", CRMType.ToString(), CRM.Exception.Message))
                                ElseIf CRM.ErrorCode <> IntegrationError.None And CRM.ErrorCode <> IntegrationError.Unknown Then
                                    Integration.LogSyncResult(String.Format("Error reported in {0} sync: {1}", CRMType.ToString(), CRM.ErrorCode.ToString()), CRMLinkTableItem.OrganizationID, LoginUser)
                                    Log.Write(String.Format("Error reported in {0} sync: {1}", CRMType.ToString(), CRM.ErrorCode.ToString()))
                                Else
                                    Integration.LogSyncResult(String.Format("Error reported in {0} sync. Last link date/time not updated.", CRMType.ToString()), CRMLinkTableItem.OrganizationID, LoginUser)
                                    Log.Write(String.Format("Error reported in {0} sync. Last link date/time not updated.", CRMType.ToString()))
                                End If
                            End If
                        Catch ex As Exception
                            Log.Write(String.Format("Sync Error: {0}", ex.StackTrace))
                            Throw ex
                        End Try
                    End If
                Catch ex As Exception
                    Integration.LogSyncResult(String.Format("Sync Error Reported."), CRMLinkTableItem.OrganizationID, LoginUser)
                End Try
            End Sub

        End Class

    End Namespace
End Namespace


