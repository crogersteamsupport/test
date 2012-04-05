Imports TeamSupport.Data
Imports TeamSupport.ServiceLibrary
Imports System.IO

Namespace TeamSupport
  Namespace CrmIntegration
    <Serializable()> Public Class CrmPool
      Inherits ServiceThread

      Private Shared _crmPoolStopped As Boolean = False
      Public Shared Property CrmPoolStopped() As Boolean
        Get
          SyncLock GetType(CrmPool)
            Return _crmPoolStopped
          End SyncLock
        End Get
        Set(ByVal value As Boolean)
          SyncLock GetType(CrmPool)
            _crmPoolStopped = value
          End SyncLock
        End Set
      End Property

      Private _threads As New List(Of CrmProcessor)()

      Public Sub New()
        RunHandlesStop = True
      End Sub

      Public Overrides Sub [Stop]()
        CrmPoolStopped = True
        MyBase.[Stop]()
      End Sub

      Public Overrides Sub Start()
        Dim service As Service = Services.GetService(LoginUser, "CrmProcessor")
        service.RunCount = 0
        service.RunTimeAvg = 0
        service.RunTimeMax = 0
        service.ErrorCount = 0
        service.LastError = ""
        service.LastResult = ""
        service.Collection.Save()
        MyBase.Start()
      End Sub


      Public Overrides Sub Run()
        Dim maxThreads As Integer = Settings.ReadInt("Max Worker Processes", 1)


        If _threads.Count > 0 Then
          For index = _threads.Count - 1 To 0 Step -1
            If IsStopped Then
              _threads(index).Stop()
              _threads.RemoveAt(index)
            ElseIf (_threads(index).IsStopped) Then
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
          If _threads.Count >= maxThreads Then
            Return
          End If

          If Not IsAlreadyProcessing(link.CRMLinkID) Then

            Dim crmProcessor As New CrmProcessor(link.CRMLinkID)
            crmProcessor.Start()
            _threads.Add(crmProcessor)
          End If
        Next
      End Sub


      Private Function IsAlreadyProcessing(ByVal crmLinkID As Integer) As Boolean
        For Each thread As CrmProcessor In _threads
          If thread.CrmLinkID = crmLinkID Then
            Return True
          End If
        Next
        Return False
      End Function


    End Class

  End Namespace
End Namespace


