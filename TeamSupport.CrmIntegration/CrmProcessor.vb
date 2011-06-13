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
            ProcessOrganization(crmLinkTable(0))

          Catch ex As Exception
            ex.Data("OrganizationID") = OrganizationID
            ExceptionLogs.LogException(LoginUser, ex, "Service - " & ServiceName, crmLinkTable(0).Row)
          End Try

          crmLinkTable(0).LastProcessed = DateTime.UtcNow
          crmLinkTable.Save()
        End If


      End Sub


      Public Sub ProcessOrganization(ByVal crmLinkTableItem As CRMLinkTableItem)
        Dim CRMType As IntegrationType = [Enum].Parse(GetType(IntegrationType), crmLinkTableItem.CRMType)

        'set up log per organization
        Dim Log As New SyncLog(Path.Combine(Settings.ReadString("Log File Path", "C:\CrmLogs\"), crmLinkTableItem.OrganizationID.ToString()))
        Dim CRM As Integration

        Select Case CRMType
          Case IntegrationType.Batchbook
            CRM = New BatchBook(crmLinkTableItem, Log, LoginUser)
            CRM.PerformSync()
            CRM.SendTicketData()
          Case IntegrationType.Highrise
          Case IntegrationType.SalesForce
        End Select
      End Sub

    End Class

    'TODO: maybe we move this into teamsupport.data?
    Public Enum IntegrationType
      SalesForce
      Highrise
      Batchbook
      FreshBooks
    End Enum
  End Namespace
End Namespace


