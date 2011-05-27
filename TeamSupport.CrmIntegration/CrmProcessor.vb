Imports TeamSupport.Data
Imports TeamSupport.ServiceLibrary

Namespace TeamSupport
  Namespace CrmIntegration

    Public Class CrmProcessor
      Inherits ServiceThread

      ''' <summary>
      ''' This function is called by the service to run a specified interval.  This function is called in its own thread.
      ''' </summary>
      ''' <remarks>Check the property IsStopped to see if you need to stop processing.</remarks>
      Public Overrides Sub Run()
        Dim crmLinkTable As New CRMLinkTable(LoginUser)
        'This loads a DataTable with the Active CRMLinkTable records
        crmLinkTable.LoadActive()

        'You can iterate through the table
        For Each crmLinkTableItem As CRMLinkTableItem In crmLinkTable
          Try

            If IsStopped Then
              Return
            End If

            ProcessOrganization(crmLinkTableItem)



            'You can update a record like this
            crmLinkTableItem.LastLink = DateTime.UtcNow
            'This saves the whole table, not just this record.  It can be called at the end of the loop on the outside as well
            crmLinkTableItem.Collection.Save()

            'There is a bug currently to add a new record to an already loaded datatable (i have just never fixed it)
            'You can do it in this way
            Dim newItem As CRMLinkTableItem = (New CRMLinkTable(LoginUser)).AddNewCRMLinkTableItem()
            newItem.Active = True
            newItem.OrganizationID = 1234
            'set the rest of the properties here
            newItem.Collection.Save()



          Catch ex As Exception
            'This is an example of how to log exceptions into the ExceptionLogs table

            'This will store extra data in the exception and write it to the table
            'You can see that we can access the OrganizationID field as a property
            ex.Data("OrganizationID") = crmLinkTableItem.OrganizationID

            'This will log the exception and the Row of the DataTable
            ExceptionLogs.LogException(LoginUser, ex, "Service - " & ServiceName, crmLinkTableItem.Row)
          End Try


        Next






      End Sub

      Public Sub ProcessOrganization(ByVal crmLinkTableItem As CRMLinkTableItem)
        'Process the org here

      End Sub





    End Class
  End Namespace
End Namespace


