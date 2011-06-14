﻿Imports TeamSupport.Data
Imports TeamSupport.ServiceLibrary
Imports System.IO

Namespace TeamSupport
  Namespace CrmIntegration

    Public Class CrmPool
      Inherits ServiceThread

      Private _threads As New List(Of CrmProcessor)()

      Public Sub New()
        RunHandlesStop = True
      End Sub

      Public Overrides ReadOnly Property ServiceName As String
        Get
          Return "CrmPool"
        End Get
      End Property

      Public Overrides Sub Run()
        Dim maxThreads As Integer = Settings.ReadInt("Max Worker Processes", 1)


        If _threads.Count > 0 Then
          For index = _threads.Count - 1 To 0 Step -1
            If IsStopped Then
              If Not _threads(index).IsStopped Then
                _threads(index).Stop()
              End If
              'Stopping, remove threads
              _threads.RemoveAt(index)
            ElseIf (_threads(index).IsStopped) Then
              'Remove completed threads
              _threads.RemoveAt(index)
            End If
          Next
        End If

        If IsStopped Then Return


        'See if we have reached our max thread coutn
        If _threads.Count >= maxThreads Then
          Return
        End If

        'Load the waiting links to process
        Dim links As New CRMLinkTable(_loginUser)
        links.LoadActive()

        For Each link As CRMLinkTableItem In links
          If Not IsAlreadyProcessing(link.OrganizationID) Then
            Dim crmProcessor As New CrmProcessor(link.OrganizationID)
            crmProcessor.Start()
            _threads.Add(crmProcessor)
            Return
          End If
        Next
      End Sub


      Private Function IsAlreadyProcessing(ByVal organizationID As Integer) As Boolean
        For Each thread As CrmProcessor In _threads
          If thread.OrganizationID = organizationID Then
            Return True
          End If
        Next
        Return False
      End Function


    End Class

  End Namespace
End Namespace


