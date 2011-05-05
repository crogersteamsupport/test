Imports TeamSupport.Data
Imports TeamSupport.WebUtils
Partial Class Inventory_Inventory
    Inherits System.Web.UI.Page

    'Protected Sub gridInventory_NeedDataSource(ByVal source As Object, ByVal e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles gridAssignedInventory.NeedDataSource
    'Dim products As New Products(UserSession.LoginUser)
    'products.LoadByOrganizationID(UserSession.LoginUser.OrganizationID)
    'gridInventory.DataSource = products

    'End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'gridAssignedInventory.DataBind()

    End Sub

    Protected Sub SqlDataSource1_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.SqlDataSourceSelectingEventArgs) Handles sqlAssignedInventory.Selecting

    End Sub
End Class
