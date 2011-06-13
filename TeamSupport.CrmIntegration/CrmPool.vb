Imports TeamSupport.Data
Imports TeamSupport.ServiceLibrary
Imports System.IO

Namespace TeamSupport
  Namespace CrmIntegration

    Public Class CrmPool
      Inherits ServiceThread


      Public Overrides ReadOnly Property ServiceName As String
        Get
          Return "CrmPool"
        End Get
      End Property

      Public Overrides Sub Run()
        Dim maxThreads As Integer = Settings.ReadInt("Max Worker Processes", 1)
        Dim threads As New List(Of CrmProcessor)()

        'Check if stopped, if so spread the word
        If IsStopped Then
          For Each thread As CrmProcessor In threads
            If Not thread.IsStopped Then
              thread.Stop()
            End If
          Next
          Return
        End If


        'Remove completed threads
        For Each thread As CrmProcessor In threads
          If thread.IsComplete Then
            threads.Remove(thread)
          End If
        Next

        'See if we have reached our max thread coutn
        If threads.Count >= maxThreads Then
          Return
        End If

        'Load the waiting links to process
        Dim links As New CRMLinkTable(_loginUser)
        links.LoadActive()

        For Each link As CRMLinkTableItem In links
          If Not IsAlreadyProcessing(threads, link.OrganizationID) Then
            Dim crmProcessor As New CrmProcessor(link.OrganizationID)
            crmProcessor.Start()
            threads.Add(crmProcessor)
            Return
          End If
        Next
      End Sub


      Private Function IsAlreadyProcessing(ByRef threads As List(Of CrmProcessor), ByVal organizationID As Integer) As Boolean
        For Each thread As CrmProcessor In threads
          If thread.OrganizationID = organizationID Then
            Return True
          End If
        Next
        Return False
      End Function


    End Class

  End Namespace
End Namespace


