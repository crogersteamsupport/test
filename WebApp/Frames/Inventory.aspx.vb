Imports TeamSupport.Data
Imports TeamSupport.WebUtils
Imports System.Globalization

Partial Class Frames_Inventory
    Inherits System.Web.UI.Page

  Protected Sub gridProducts_NeedDataSource(ByVal source As Object, ByVal e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles gridProducts.NeedDataSource
    Dim products As New Products(UserSession.LoginUser)
    products.LoadByOrganizationID(UserSession.LoginUser.OrganizationID)
    gridProducts.DataSource = products

    'Heres how to get the connections string, userid, & orgID
    Dim connectionString As String = UserSession.LoginUser.ConnectionString
    Dim userID As Integer = UserSession.CurrentUser.UserID
    Dim orgID As Integer = UserSession.CurrentUser.OrganizationID
    Dim isAdmin As Boolean = UserSession.CurrentUser.IsSystemAdmin
    Dim productType As ProductType = UserSession.CurrentUser.ProductType


    If productType = TeamSupport.Data.ProductType.Enterprise Then

      'Do something if Enterprise

      'You need to import this unit 
      'Imports System.Globalization

      'get Culture Name from Users.CultureInfo, if null get from Organizations.CultureInfo
      Dim cultureName As String = "en-US"
      'This will tell the grids to use this specific culture
      Page.Culture = cultureName

      'You can manually get the date time string. 
      'This is the link to find the standard datetime strings
      'http://msdn.microsoft.com/en-us/library/az4se3k1.aspx
      Dim cultureInfo As CultureInfo = New CultureInfo(cultureName)
      DateTime.Now.ToString("g", cultureInfo)




    End If

  End Sub
End Class
