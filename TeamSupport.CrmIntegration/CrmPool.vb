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
        MyBase.Logs.WriteEvent("CrmPool is running...", true)
        Dim maxThreads As Integer = Settings.ReadInt("Max Worker Processes", 1)
        MyBase.Logs.WriteEvent("maxThreads: " + maxThreads.ToString(), true)

        MyBase.Logs.WriteEvent("IsStopped: " + IsStopped.ToString(), true)
        MyBase.Logs.WriteEvent("Threads count: " + _threads.Count.ToString(), true)
        If _threads.Count > 0 Then
          For index = _threads.Count - 1 To 0 Step -1
            MyBase.Logs.WriteEvent("Checking thread index: " + index.ToString(), true)
            If IsStopped Then
              _threads(index).Stop()
              MyBase.Logs.WriteEvent("Thread index: " + index.ToString() + " stopped.", true)
              _threads.RemoveAt(index)
              MyBase.Logs.WriteEvent("Thread index: " + index.ToString() + " removed.", true)
            ElseIf (_threads(index).IsStopped) Then
              MyBase.Logs.WriteEvent("Thread index: " + index.ToString() + " is stopped.", true)
              _threads.RemoveAt(index)
              MyBase.Logs.WriteEvent("Thread index: " + index.ToString() + " removed.", true)
            End If
          Next
        End If

        If IsStopped Then
          MyBase.Logs.WriteEvent("CrmPool finishing as is stopped", true)
          Return
        End If


        'See if we have reached our max thread coutn
        If _threads.Count >= maxThreads Then
          MyBase.Logs.WriteEvent("CrmPool finishing as its threads count exceeds maxThreads", true)
          Return
        End If

        'Load the waiting links to process
        Dim links As New CRMLinkTable(_loginUser)
        links.LoadActive(Settings.ReadInt("CRMLinkTable Process Interval", 15))

        MyBase.Logs.WriteEvent("Active links count: " + links.Count().ToString(), true)
        Dim isAlreadyProcessingValue As Boolean = False
        For Each link As CRMLinkTableItem In links
          MyBase.Logs.WriteEvent("Checking link type: " + link.CRMType + " of OrgID: " + link.OrganizationID.ToString(), true)
          If _threads.Count >= maxThreads Then
            MyBase.Logs.WriteEvent("CrmPool finishing as its threads count exceeds maxThreads", true)
            Return
          End If

          isAlreadyProcessingValue = IsAlreadyProcessing(link.CRMLinkID)
          MyBase.Logs.WriteEvent("isAlreadyProcessing: " + isAlreadyProcessingValue.ToString(), true)
          If Not isAlreadyProcessingValue Then

            Dim crmProcessor As New CrmProcessor(link.CRMLinkID)
            MyBase.Logs.WriteEvent("link CrmProcessor instantiated.", true)
            crmProcessor.Start()
            MyBase.Logs.WriteEvent("link CrmProcessor started.", true)
            _threads.Add(crmProcessor)
            MyBase.Logs.WriteEvent("link CrmProcessor thread added.", true)
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


