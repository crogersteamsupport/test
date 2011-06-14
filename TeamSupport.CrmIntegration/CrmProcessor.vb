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

      Public Sub New(ByVal organizationID As Integer)
        _organizationID = organizationID
        IsLoop = False
      End Sub

      Public Overrides Sub Run()
        Dim crmLinkTable As New CRMLinkTable(LoginUser)
        crmLinkTable.LoadByOrganizationID(OrganizationID)
        If Not crmLinkTable.IsEmpty Then

          Try
            'ProcessOrganization(crmLinkTable(0))
            System.Threading.Thread.Sleep(5000)

          Catch ex As Exception
            ex.Data("OrganizationID") = OrganizationID
            ExceptionLogs.LogException(LoginUser, ex, "Service - " & ServiceName, crmLinkTable(0).Row)
          End Try

          crmLinkTable(0).LastProcessed = DateTime.UtcNow
          crmLinkTable.Save()
        End If


      End Sub


            Public Sub ProcessOrganization(ByVal crmLinkTableItem As CRMLinkTableItem)
                Dim Success As Boolean = True

                If IsStopped Then
                    Return
                End If

                Dim CRMType As IntegrationType = [Enum].Parse(GetType(IntegrationType), crmLinkTableItem.CRMType)

                'set up log per organization
                Dim Log As New SyncLog(Path.Combine(Settings.ReadString("Log File Path", "C:\CrmLogs\"), crmLinkTableItem.OrganizationID.ToString()))
                Dim CRM As Integration

                'only process bb right now (6/13/2011)
                Select Case CRMType
                    Case IntegrationType.Batchbook
                        CRM = New BatchBook(crmLinkTableItem, Log, LoginUser)
                        Success = CRM.PerformSync()

                        If Success Then
                            Success = CRM.SendTicketData()
                        End If

                        If Success Then
                            crmLinkTableItem.LastLink = DateTime.UtcNow
                            crmLinkTableItem.Collection.Save()
                        Else
                            Log.Write("Error reported in BatchBook sync. Last link date/time not updated.")
                        End If
                    Case IntegrationType.Highrise
                    Case IntegrationType.SalesForce
                End Select
            End Sub

        End Class

  End Namespace
End Namespace


