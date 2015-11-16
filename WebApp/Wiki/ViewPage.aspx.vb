' Wiki project started 9/18/09 by RJ
'
' Copyright (c) 2009 by Muroc Systems, Inc.
'
' 10/13/09 - Version .9
'          - Fix for 1504 (Directory not being created)
'          - Added line at bottom of ViewPage to show when article was last modified and by whom (1500)
'          - Changed how we display previous versions - Now show date & person (1493)
' 10/14/09 - Minor fix to allow document manager to handle any file type (set SearchPattern) (1516)
' 10/15/09 - Minor fix or error 1526
' 11/13/09 - Added ability to display pages in advanced portal
' 11/24/09 - Added ability to delete an article
' 12/18/09 - Times are now converted to local times. (1852)
'          - Attempt to fix display issue in ie7 (1794, 1792)
'          - Fixed issue with last modified not showing correctly in article display (1866)
' 2/25/10  - Added ticket link to edit page (untested)
' 3/24/10  - Europen date support
' 4/1/13   - Changed max upload to 100mb
'          - Modified a few settings
'          - Fixed 7590, 7853, 8984






Imports System.Data.SqlClient
Imports TeamSupport.WebUtils
Imports Telerik.Web.UI
Imports TSWebUtilities


Partial Class _Default
    Inherits System.Web.UI.Page

    Public Function GetOrgID() As String
        Dim OrgID As String

        Try
            OrgID = UserSession.LoginUser.OrganizationID.ToString()
        Catch ex As Exception
            Try
                OrgID = Request.QueryString("OrganizationID")
            Catch ex2 As Exception
                OrgID = ""
            End Try
        End Try

        Return OrgID


    End Function

 

    Public Function GetArticleID() As String
        Dim ArticleID As String

        Try
            ArticleID = Request.QueryString("ArticleID")
        Catch ex As Exception
            ArticleID = "0"
        End Try

        Return ArticleID

    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not Page.IsPostBack Then





            Dim OrgID As String = GetOrgID()
            Dim ArticleID As String = GetArticleID()

            If OrgID <> "" And ArticleID <> "" Then
                EditButton.Visible = True
                hidden_ArticleID.Value = ArticleID
                'Display the article
                If CanViewArticle(ArticleID, OrgID) Then
                    ArticleBody.Text = DisplayArticle(ArticleID, OrgID)
                Else
                    ArticleBody.Text = "You are not authorized to view this article."
                End If






            Else

                EditButton.Visible = False


            End If


            'set the navigation sql source
            NavigationSource.SelectParameters("OrganizationID").DefaultValue = OrgID
            NavigationSource.SelectParameters("UserID").DefaultValue = GetUserID()
            NavigationSource.DataBind()

            RadTreeView1.DataBind()

            'This will find the node of the selected artile and make sure it's selected and displayed
            Dim Node As RadTreeNode

            Node = RadTreeView1.FindNodeByValue(ArticleID)

            While Node IsNot Nothing
                If Node.ParentNode IsNot Nothing Then
                    node.ParentNode.Expanded = True
                End If

                Node = Node.parentnode
            End While


            Try
                Node = RadTreeView1.FindNodeByValue(ArticleID)
                Node.Selected = True
            Catch ex As Exception

            End Try

            'set the URL for the view URL tooltip
            Try
                If GetArticleID() <> "" Then
                    RadToolTip1.Title = "https://app.teamsupport.com?articleid=" + GetArticleID()
                    ArticleLink.Visible = True
                Else
                    ArticleLink.Visible = False
                End If

            Catch ex As Exception
                ArticleLink.Visible = False
            End Try

            Try


                If IsPagePublic(GetArticleID, GetOrgID) Then
                    RadToolTip2.Visible = True
                    RadToolTip2.Title = "https://app.teamsupport.com/wiki/justarticle.aspx?Organizationid=" + GetOrgID() + "&ArticleID=" + GetArticleID()
                Else
                    ExternalLink.Visible = False
                End If
            Catch ex As Exception
                ExternalLink.Visible = False
            End Try

            'End If

            'We need to figure out secutiry for external views.


            Dim tsweb As New TSWebUtilities
            'This should set the page's culture info and display dates & times correctly.
            tsweb.ConnectionString = ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString
            Page.Culture = tsweb.GetCultureInfo(GetOrgID, GetUserID)

        End If
    End Sub







    Protected Function GetUserID() As String

        Dim TempUserID As String

        Try
            TempUserID = UserSession.LoginUser.UserID.ToString
        Catch ex As Exception
            TempUserID = "-1"
        End Try


      
        Return TempUserID

    End Function

    Protected Sub EditButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles EditButton.Click

        Dim OrgID As String = GetOrgID()




        Response.Redirect(".\editpage.aspx?organizationid=" + OrgID + "&articleid=" + hidden_ArticleID.Value)
    End Sub







    ' Protected Sub DataList1_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataListCommandEventArgs) Handles NavigationList.ItemCommand
    'Dim OrgID As String = GetOrgID()

    'Dim ArticleID As String = e.CommandArgument

    'Response.Redirect(".\viewpage.aspx?organizationid=" + OrgID + "&ArticleID=" & ArticleID)


    'Display the article body
    '   ArticleBody.Text = DisplayArticle(ArticleID, OrgID)

    '  EditButton.Visible = True

    ' hidden_ArticleID.Value = ArticleID


    'End Sub

    Protected Sub SubArticleList_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataListCommandEventArgs)
        Dim OrgID As String = GetOrgID()

        Dim ArticleID As String = e.CommandArgument

        'Response.Redirect(".\viewpage.aspx?organizationid=" + OrgID + "&ArticleID=" & ArticleID)


        'Display the article body
        ArticleBody.Text = DisplayArticle(ArticleID, OrgID)

        EditButton.Visible = True

        hidden_ArticleID.Value = ArticleID

    End Sub


    Public Function GetTimeZone(ByRef OrganizationID As String) As String
        'This will return the Font Family stored in Portal Options

        Dim sqlStatement As String = "select TimeZoneID from Organizations where organizationid = @orgid"

        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@OrgID", OrganizationID))

        command.Connection.Open()
        Try



            Dim reader As SqlDataReader = command.ExecuteReader


            'return the string
            Try


                Try

                    If reader.Read Then
                        Return reader(0)
                    Else
                        Return "Central Standard Time"
                    End If
                Catch ex As Exception
                    Return "Central Standard Time"
                End Try

            Finally
                command.Connection.Close()
                connection.Close()
                reader.Close()
            End Try

            command.Connection.Close()
        Catch ex As Exception
            Return True 'use recaptcha if there's an error
        End Try
    End Function

    Public Function ConvertToLocalTime(ByVal UTCDateTime As DateTime) As DateTime

        Try
        
            Dim TZ As System.TimeZoneInfo

            Try
                TZ = TimeZoneInfo.FindSystemTimeZoneById(GetTimeZone(GetOrgID()))
            Catch ex As Exception
                TZ = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")
            End Try

            Return TimeZoneInfo.ConvertTimeFromUtc(UTCDateTime, TZ).ToString()
        Catch ex As Exception
            Return UTCDateTime

        End Try


    End Function

    Public Function DisplayArticle(ByVal ArticleID As String, ByVal OrganizationID As String) As String



        Dim sqlStatement As String = "select wikiarticles.Body, wikiarticles.ModifiedDate, users.firstname+' '+users.lastname as username from WikiArticles left outer join users on wikiarticles.modifiedby = users.userid where ArticleID = @ArticleID and wikiarticles.OrganizationID = @OrganizationID"

        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@OrganizationID", OrganizationID))
        command.Parameters.Add(New SqlParameter("@ArticleID", ArticleID))

        command.Connection.Open()
        'Try



        Dim reader As SqlDataReader = command.ExecuteReader


        'return the string
        Try


            Try

                If reader.Read Then


                    Dim tsweb As New TSWebUtilities
                    tsweb.ConnectionString = ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString
                    Dim CultureName As String = tsweb.GetCultureInfo(GetOrgID, GetUserID)
                    Dim CultureInfo As New Globalization.CultureInfo(cultureName)
                    'This should make the style have a white background and override the main page's style
                    Return "<div  style=""background-color: #ffffff;"">" + ProcessWikiBody(reader(0).ToString) + "<div><br/><i>Article Last Modified on " + ConvertToLocalTime(reader(1)).ToString("g", CultureInfo) + " by " + reader(2).ToString + "</i></div></div>"


                Else
                    Return ""
                End If
            Catch ex As Exception
                Return ""
            End Try

        Finally
            command.Connection.Close()
            connection.Close()
            reader.Close()
        End Try

        command.Connection.Close()
        'Catch ex As Exception
        'Return ""
        'End Try

    End Function

    Function GetColonToBracket(ByVal StrToParse As String, ByVal StartLocation As Integer) As String
        'Pass the start location (from an instr, for example) and this will find the text between the colon and the end bracket

        Dim NewString As String = ""
        Dim ColonLocation As Integer

        'scan to find colon
        For x As Integer = StartLocation To StrToParse.Length
            If StrToParse(x) = ":" Then
                ColonLocation = x
                Exit For
            End If
        Next


        If Not (ColonLocation >= StrToParse.Length) Then 'if the colon location's not at the end of the string...

            'Now we should have colon location
            Dim y As Integer = ColonLocation + 1
            Try
                'scan to find closing bracket
                While StrToParse(y) <> "]"
                    NewString = NewString + StrToParse(y)
                    y = y + 1
                End While

                Return NewString.Trim 'trim takes care of leading and trailing spaces

            Catch ex As Exception
                Return "" 'if we don't get a trailing ] then we should error out and return nothing
            End Try
        Else
            Return "" 'looks like a problem - Return nothing
        End If

    End Function

    Function GetTextAfterSpace(ByVal StrToParse As String, ByVal StartLocation As Integer) As String
        'Pass the start location (from an instr, for example) and this will find the text between the colon and the first space

        Dim NewString As String = ""
        Dim SpaceLocation As Integer

        'scan to find space
        For x As Integer = StartLocation To StrToParse.Length
            If StrToParse(x) = " " Then
                SpaceLocation = x
                Exit For
            End If
        Next


        If Not (SpaceLocation >= StrToParse.Length) Then 'if the colon location's not at the end of the string...

            'Now we should have colon location
            Dim y As Integer = SpaceLocation + 1
            Try
                'scan to find closing bracket
                While StrToParse(y) <> "]"
                    NewString = NewString + StrToParse(y)
                    y = y + 1
                End While

                Return NewString.Trim 'trim takes care of leading and trailing spaces

            Catch ex As Exception
                Return "" 'if we don't get a trailing ] then we should error out and return nothing
            End Try
        Else
            Return "" 'looks like a problem - Return nothing
        End If

    End Function

    Function GetFinalBracketLocation(ByVal StrToParse As String, ByVal StartLocation As Integer) As Integer
        'Pass the start location (from an instr, for example) and this will find the text between the colon and the end bracket

        Dim NewString As String = ""
        Dim BracketLocation As Integer

        'scan to find colon
        For x As Integer = StartLocation To StrToParse.Length
            If StrToParse(x) = "]" Then
                BracketLocation = x
                Exit For
            End If
        Next


        If Not (BracketLocation >= StrToParse.Length) Then
            Return BracketLocation
        Else
            Return 0
        End If

    End Function

    Public Function ChangePageLinks(ByRef body As String) As Boolean
        'returns TRUE is we have changed something 

        Dim OrgID As String = GetOrgID()

        Dim TempBody As String = body

        Dim PageLink As Integer = InStr(body.ToUpper, "[ARTICLE:")

        If PageLink > 0 Then
            'if we have found the text in the string...
            'Return GetColonToBracket(DescriptionText, VersionLocation)
            Dim LinkLocation As String = GetColonToBracket(TempBody, PageLink) 'this should be the articleid
            Dim FinalBracket As Integer = GetFinalBracketLocation(TempBody, PageLink)

            Dim TextforLink As String = GetTextAfterSpace(TempBody, PageLink)
            If TextforLink = "" Then TextforLink = LinkLocation



            'Now we can cut from PageLink to FinalBracket
            TempBody = TempBody.Remove(PageLink - 1, FinalBracket - PageLink + 2)
            TempBody = TempBody.Insert(PageLink - 1, "<a href=""./viewpage.aspx?Organizationid=" + OrgID + "&articleid=" + LinkLocation + """>" + TextforLink + "</a>")
            body = TempBody
            Return True
        Else
            Return False 'return false if we didn't find anything
        End If


    End Function

    Public Function ProcessWikiBody(ByVal Body As String) As String
        'This routine will process the text in a Wiki article and replace certain text with the correct html

        'for now just return the same text


        While ChangePageLinks(Body)
            'should get all of the page links and replace them with <a> tags to the correct page
        End While

        'lets replace all instances of the viewpage call with viewarticle (so we just display in this pane)
        'Note:  The edit page should default to viewpage (not justarticle), but just in case let's put this in there
        Body = Strings.Replace(Body, "justarticle", "viewpage", 1, -1, CompareMethod.Text)



        Return Body



    End Function

    Public Function GetSubArticles(ByVal ArticleID As String) As SqlDataSource
        'This will return a datasource with all of the children articles for the given main article
        'Used to show nested data in the main article view
        Dim DataSource1 As New SqlDataSource
        DataSource1.ConnectionString = ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString
        DataSource1.SelectCommand = "select articlename, articleID from wikiarticles where parentid=" + ArticleID.ToString + " and ((IsNull(Private,0)=0) or (CreatedBy=" + GetUserID() + "))"

        Return DataSource1



    End Function


    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles EditButton.Click
        Dim OrgID As String = GetOrgID()




        Response.Redirect("editpage.aspx?organizationid=" + OrgID + "&articleid=" + hidden_ArticleID.Value)
    End Sub

    Protected Sub ImageButton2_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles Createnew.Click
        Dim OrgID As String = GetOrgID()
        Dim ArticleID As String = GetArticleID()

        Response.Redirect("editpage.aspx?organizationid=" + OrgID + "&new=yes")
    End Sub

    Protected Sub RadTreeView1_NodeClick(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadTreeNodeEventArgs) Handles RadTreeView1.NodeClick
        Dim OrgID As String = GetOrgID()
        Dim ArticleID As String = e.Node.Value


        Response.Redirect(".\viewpage.aspx?organizationid=" + OrgID + "&ArticleID=" & ArticleID)



    End Sub

    Public Function CanViewArticle(ByVal ArticleID As String, ByVal OrganizationID As String) As Boolean

        'Will return true if this user can view this artcile (ie not private), otherwise false

        Dim sqlStatement As String = "select isnull(Private,0), CreatedBy from WikiArticles where ArticleID = @ArticleID and OrganizationID = @OrganizationID"

        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@OrganizationID", OrganizationID))
        command.Parameters.Add(New SqlParameter("@ArticleID", ArticleID))

        command.Connection.Open()
        'Try



        Dim reader As SqlDataReader = command.ExecuteReader


        'return the string
        Try


            Try

                If reader.Read Then
                    Return (reader(0).ToString = "False") Or (reader(1).ToString = GetUserID())
                Else
                    Return True
                End If
            Catch ex As Exception
                Return 0
            End Try

        Finally
            command.Connection.Close()
            connection.Close()
            reader.Close()
        End Try

        command.Connection.Close()
    End Function

    Protected Sub ArticleLink_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ArticleLink.Click
        'This is the link to the internal page - Used to e-mail page or embed in a ticket
        ' Response.Redirect("https://app.teamsupport.com?articleid=" + GetArticleID())


    End Sub

    Protected Sub ExternalLink_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ExternalLink.Click
        'This is the link to an external page
        'Should only be shown if the page is marked public
        'Response.Redirect("https://app.teamsupport.com/wiki/justarticle.aspx?Organizationid=" + GetOrgID() + "&ArticleID=" + GetArticleID())

        Dim url As String = "https://app.teamsupport.com/wiki/justarticle.aspx?Organizationid=" + GetOrgID() + "&ArticleID=" + GetArticleID()
        Response.Write("<script>" & vbCrLf)
        Response.Write("window.open('" & url & "');" & vbCrLf)
        Response.Write("</script>")


    End Sub

    Public Function IsPagePublic(ByVal ArticleID As String, ByVal OrganizationID As String) As String

        Dim sqlStatement As String = "select Body, ModifiedDate, PublicView from WikiArticles where ArticleID = @ArticleID and OrganizationID = @OrganizationID"

        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@OrganizationID", OrganizationID))
        command.Parameters.Add(New SqlParameter("@ArticleID", ArticleID))

        command.Connection.Open()
        Try



            Dim reader As SqlDataReader = command.ExecuteReader


            'return the string
            Try


                Try

                    If reader.Read Then
                        If reader(2).ToString = "True" Then
                            Return True
                        Else
                            Return False
                        End If

                    Else
                        Return ""
                    End If
                Catch ex As Exception
                    Return ""
                End Try

            Finally
                command.Connection.Close()
                connection.Close()
                reader.Close()
            End Try

            command.Connection.Close()
        Catch ex As Exception
            Return False
        End Try

    End Function




    Protected Sub btn_Help_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btn_Help.Click
        Response.Write("<script>window.open('http://help.teamsupport.com/wiki','_blank');</script>")
    End Sub
End Class
