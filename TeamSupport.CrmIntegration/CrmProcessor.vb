Imports TeamSupport.Data
Imports TeamSupport.ServiceLibrary
Imports System.IO

Namespace TeamSupport
    Namespace CrmIntegration

        Public Class CrmProcessor
            Inherits ServiceThread
            Private CrmLinkRow As CRMLinkTableItem

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

            Private _organizationID As Integer
            Public ReadOnly Property OrganizationID() As Integer
                Get
                    Dim result As Integer
                    SyncLock Me
                        result = _organizationID
                    End SyncLock
                    Return result
                End Get
            End Property

            Public Sub New(ByVal organization As CRMLinkTableItem)
                CrmLinkRow = organization
                _organizationID = CrmLinkRow.OrganizationID
                IsLoop = False
            End Sub

            Public Overrides Sub Run()

                Try
                    ProcessOrganization(CrmLinkRow)
                    'System.Threading.Thread.Sleep(5000)

                Catch ex As Exception
                    ex.Data("OrganizationID") = OrganizationID
                    ExceptionLogs.LogException(LoginUser, ex, "Service - " & ServiceName, CrmLinkRow.Row)
                End Try

                CrmLinkRow.LastProcessed = DateTime.UtcNow
                CrmLinkRow.Collection.Save()

            End Sub


            Public Sub ProcessOrganization(ByVal crmLinkTableItem As CRMLinkTableItem)
                Dim Success As Boolean = True

                If IsStopped Then
                    Return
                End If

                Dim CRMType As IntegrationType = [Enum].Parse(GetType(IntegrationType), crmLinkTableItem.CRMType, True)

                'set up log per organization
                Dim Log As New SyncLog(Path.Combine(Settings.ReadString("Log File Path", "C:\CrmLogs\"), crmLinkTableItem.OrganizationID.ToString()))
                Dim CRM As Integration

                Select Case CRMType
                    Case IntegrationType.Batchbook
                        CRM = New BatchBook(crmLinkTableItem, Log, LoginUser, Me)
                    Case IntegrationType.Highrise
                        CRM = New Highrise(crmLinkTableItem, Log, LoginUser, Me)
                    Case IntegrationType.SalesForce
                End Select

                'only process bb and hr
                If CRMType = IntegrationType.Batchbook Or CRMType = IntegrationType.Highrise Then
                    Log.Write(String.Format("Begin processing {0} sync.", CRMType.ToString()))

                    Success = CRM.PerformSync()

                    If Success Then
                        Success = CRM.SendTicketData()
                    End If

                    If Success Then
                        crmLinkTableItem.LastLink = DateTime.UtcNow
                        crmLinkTableItem.Collection.Save()
                        Log.Write("Finished processing successfully.")
                    Else
                        Log.Write(String.Format("Error reported in {0} sync. Last link date/time not updated.", CRMType.ToString()))
                    End If

                End If
            End Sub

        End Class

    End Namespace
End Namespace


