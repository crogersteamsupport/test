' 4/1/13 - Changed max upload to 100mb from 5mb

Imports System.Data.SqlClient
Imports System.IO
Imports TeamSupport.WebUtils
Imports TSWebUtilities

' 4/18/13 - PerviousVersionSource is limited to the last 50 items now.  We were throwing an error on some pages and I think it was because we were running the GetOrganizationCulture too many times.


Partial Class EditPage
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


        'OK, how do we manage user security?  
        ' - If we are logged into TeamSupport then it's easy.
        ' - If we are purely external it's easy since we can just check the public edit flag
        '    - We should as a recaptcha though
        ' - However, how do we tell a login from the portal???
        '    - Anything passed in the URL can be hacked...

    End Function
    Protected Function GetUserID() As String

        Dim TempUserID As String

        Try
            TempUserID = UserSession.LoginUser.UserID.ToString
        Catch ex As Exception
            TempUserID = "-1"
        End Try


        'temp to return user ID

        Return TempUserID

    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then

            Dim OrgID As String = GetOrgID()

            Dim ArticleID As String
            Try
                ArticleID = Request.QueryString("ArticleID")
            Catch ex As Exception
                ArticleID = "0"
            End Try

            If ArticleID = Nothing Then ArticleID = 0 'Solves problem where dd list was not showing anything on a new ticket

            Dim NewPage As Boolean
            Dim NewPageString As String
            Try
                NewPageString = Request.QueryString("New")
                NewPage = NewPageString = "yes"
            Catch ex As Exception
                NewPage = False
            End Try

            If OrgID <> "" And ArticleID <> "" Then
                If CanViewArticle(ArticleID, OrgID) Then
                    DisplayArticleForEdit(ArticleID, OrgID, NewPage)
                Else
                    'We can't see this article, so go back to view page
                    Response.Redirect("viewpage.aspx?Organizationid=" + OrgID + "&ArticleID=" + ArticleID)
                End If


            Else
                RadEditor1.Content = ""


            End If



                'Set the directories for the ImageManager and DocumentManager so we can upload docs
                SetUpDirectories(OrgID)




                'This populates the Links that allow hyperlinks to other Wiki pages
                SetUpLinks(OrgID)

                'Display the prior versions of this article 
                PreviousVersionSource.SelectParameters("ArticleID").DefaultValue = ArticleID
                PreviousVersionSource.SelectParameters("OrganizationID").DefaultValue = OrgID
                PreviousVersionSource.DataBind()

                'This is the list for the master page pulldown
                MasterPageSource.SelectParameters("OrgID").DefaultValue = OrgID
            MasterPageSource.SelectParameters("ArticleID").DefaultValue = ArticleID
            MasterPageSource.SelectParameters("UserID").DefaultValue = GetUserID()
                MasterPageSource.DataBind()


                If Not NewPage Then 'no need to set the parentid and security flags if this is a new page
                    'Set the pulldown for the parent ids
                    SetParentID(ArticleID)
                    'Lets set up the privacy flags for the article
                    SetSecurityFlags(ArticleID)

                    'If the page has sub pages, then we can't make this page a subpage of another one.
                'If DoesPageHaveSubPages(ArticleID) Then dd_ParentID.Enabled = False
                End If






                'RadEditor1.Width = 100%

            'Show the link to the publicly viewable page
            JustArticleLink.NavigateUrl = "justarticle.aspx?articleID=" + ArticleID + "&OrganizationID=" + OrgID
            JustArticleLink.Target = "_blank"


            'This will put a warning box up when you try and delete an article
            btn_DeleteArticle.Attributes.Add("onclick", "if(confirm('Are you sure you want to delete this article?')){}else{return false}")


            'Need to only display delete box if user is an admin and/or he created the article in the first place
            If (WhoCreatedArticle(ArticleID) = GetUserID()) Then
                'Display delete button if we're an admin or the creator of the article
                btn_DeleteArticle.Visible = True
            Else
                'no delete button
                btn_DeleteArticle.Visible = False
            End If

            'if the user is an admin then override and display button
            Try
                If (UserSession.CurrentUser.IsSystemAdmin) Then
                    btn_DeleteArticle.Visible = True
                End If
            Catch ex As Exception

            End Try

            Dim tsweb As New TSWebUtilities
            'This should set the page's culture info and display dates & times correctly.
            tsweb.ConnectionString = ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString
            Page.Culture = tsweb.GetCultureInfo(GetOrgID, GetUserID)

        End If 'postback
    End Sub

    Public Sub DisplayArticleForEdit(ByVal ArticleID As String, ByVal OrgID As String, ByVal NewPage As Boolean)
        If Not NewPage Then
            Dim NewestVersion As Integer = GetMostRecentVersion(ArticleID, OrgID)
            RadEditor1.Content = GetArticleForEdit(ArticleID, OrgID)
            VersionViewing.Value = NewestVersion
            tb_PageName.Visible = True
            tb_PageName.Text = GetNameFromID(ArticleID)


            'lbl_pageName.Text = ArticleID

        Else 'it is a new page
            VersionViewing.Value = 1
            'new page!
            tb_PageName.Visible = True
        End If
    End Sub

    Public Function GetArticleForEdit(ByVal ArticleID As String, ByVal OrganizationID As String) As String

        Dim sqlStatement As String = "select Body, ModifiedDate from WikiArticles where ArticleID = @ArticleID and OrganizationID = @OrganizationID"

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
                    Return reader(0).ToString
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

    Public Function GetPreviousArticle(ByVal HistoryID As String, ByVal OrganizationID As String) As String

        Dim sqlStatement As String = "select Body, ModifiedDate from WikiHistory where HistoryID = @HistoryID and OrganizationID = @OrganizationID"

        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@OrganizationID", OrganizationID))
        command.Parameters.Add(New SqlParameter("@HistoryID", HistoryID))
        'command.Parameters.Add(New SqlParameter("@version", version))

        command.Connection.Open()
        'Try



        Dim reader As SqlDataReader = command.ExecuteReader


        'return the string
        Try


            Try

                If reader.Read Then
                    Return reader(0).ToString
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


    Public Function GetMostRecentVersion(ByVal ArticleID As String, ByVal OrganizationID As String) As Integer

        Dim sqlStatement As String = "select Version from WikiArticles where ArticleID = @ArticleID and OrganizationID = @OrganizationID"

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
                    Return reader(0).ToString
                Else
                    Return 0
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

    Public Function GetNameFromID(ByVal ArticleID As String) As String

        Dim sqlStatement As String = "select ArticleName from WikiArticles where ArticleID = @ArticleID"

        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        'command.Parameters.Add(New SqlParameter("@OrganizationID", OrganizationID))
        command.Parameters.Add(New SqlParameter("@ArticleID", ArticleID))

        command.Connection.Open()
        'Try



        Dim reader As SqlDataReader = command.ExecuteReader


        'return the string
        Try


            Try

                If reader.Read Then
                    Return reader(0).ToString
                Else
                    Return ""
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

    Public Sub SavePreviousVersion(ByVal ArticleID As Integer, ByVal ArticleName As String, ByVal OrganizationID As String, ByVal Body As String, ByVal Version As Integer, ByVal UserID As String)




        Dim sqlStatement As String = "insert into WikiHistory(OrganizationID, ArticleName, Body, Version, ArticleID, ModifiedBy, ModifiedDate) Values(@OrganizationID, @ArticleName, @body, @version, @ArticleID, @UserID, GetUTCDate())"


        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@OrganizationID", OrganizationID))
        command.Parameters.Add(New SqlParameter("@ArticleName", ArticleName))
        command.Parameters.Add(New SqlParameter("@Body", GetArticleForEdit(ArticleID, OrganizationID))) 'grab the current version of the article to toss in history
        command.Parameters.Add(New SqlParameter("@Version", Version))
        command.Parameters.Add(New SqlParameter("@ArticleID", ArticleID))
        command.Parameters.Add(New SqlParameter("@UserID", UserID))

        command.Connection.Open()
        Try


            command.ExecuteNonQuery()

        Finally
            command.Connection.Close()
            connection.Close()

        End Try

        command.Connection.Close()
        'Catch ex As Exception
        'Return ""
        'End Try

    End Sub

    Public Sub SaveArticle(ByVal PageName As String, ByVal OrganizationID As String, ByVal Body As String, ByVal ArticleID As String, ByVal UserID As String)

        'Get the version of the current page
        Dim MostRecentVersion As Integer = GetMostRecentVersion(ArticleID, OrganizationID)
        'Save the current published version of the page
        SavePreviousVersion(ArticleID, PageName, OrganizationID, Body, MostRecentVersion, UserID)


        'now take the edited version of the page live.
        Dim sqlStatement As String = "update WikiArticles set ArticleName = @ArticleName, Body = @Body, Version=isnull(version,0)+1, ModifiedBy=@UserID, ModifiedDate=GetUTCDate() where ArticleID = @ArticleID and OrganizationID = @OrganizationID"


        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@OrganizationID", OrganizationID))
        command.Parameters.Add(New SqlParameter("@ArticleID", ArticleID))
        command.Parameters.Add(New SqlParameter("@Body", Body))
        command.Parameters.Add(New SqlParameter("@ArticleName", tb_PageName.Text))
        command.Parameters.Add(New SqlParameter("@UserID", UserID))

        command.Connection.Open()
        Try


            command.ExecuteNonQuery()

        Finally
            command.Connection.Close()
            connection.Close()

        End Try

        command.Connection.Close()
        'Catch ex As Exception
        'Return ""
        'End Try

    End Sub

    Public Sub SaveNewArticle(ByVal PageName As String, ByVal OrganizationID As String, ByVal Body As String, ByVal UserID As String)

        'This will save a NEW article




        'now take the edited version of the page live.
        Dim sqlStatement As String = "insert into WikiArticles(body, version, Articlename, organizationid,CreatedBy, CreatedDate, ModifiedBy,ModifiedDate,ParentID) values(@Body, @version, @Articlename, @organizationid, @UserID, GetUTCDate(), @UserID, GetUTCDate(),NULL)"



        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@OrganizationID", OrganizationID))
        command.Parameters.Add(New SqlParameter("@ArticleName", PageName))
        command.Parameters.Add(New SqlParameter("@Body", Body))
        command.Parameters.Add(New SqlParameter("@Version", "1")) 'first version
        command.Parameters.Add(New SqlParameter("@UserID", UserID))


        command.Connection.Open()
        'Try


        command.ExecuteNonQuery()

        'Finally
        'command.Connection.Close()
        'connection.Close()

        'End Try

        command.Connection.Close()
        'Catch ex As Exception
        'Return ""
        'End Try

    End Sub




    

   
    Protected Sub AddLink(ByVal LinkName As String, ByVal LinkAddress As String)
        Dim NewLink As New Telerik.Web.UI.EditorLink()
        NewLink.Name = LinkName 'article name
        NewLink.Href = LinkAddress
        RadEditor1.Links.Add(NewLink)

    End Sub



    Protected Sub SetUpLinks(ByRef OrgId As String)

        'This builds the Custom Links pulldown and allows for creation of links to other Wiki pages.

        'Clear out the old custom links
        RadEditor1.Links.Clear()

        ''''


        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)

        'get tickets which were created after the last link date
        'Dim sqlStatement As String = "select TicketID, name from tickets where ticketid in (select ticketid from organizationtickets where organizationid = @orgID) and datecreated > @LastLinkDate"

        Dim sqlstatement As String = "select articlename, articleid from wikiarticles where organizationid = @Organizationid and ((isnull(private,0)=0) or (CreatedBy=@UserID)) and isnull(isdeleted,0)=0 order by ArticleName"
        'changed query to sort tickets by datecreated (descending) 8/20/09
        'Dim connection As New SqlConnection(ConnectionString)

        Dim command As New SqlCommand(sqlstatement, connection)
        command.Parameters.Add(New SqlParameter("@OrganizationID", OrgId))
        command.Parameters.Add(New SqlParameter("@UserID", GetUserID))

        command.Connection.Open()

        Dim reader As SqlDataReader = command.ExecuteReader

        While reader.Read
            'Let's add a custom link for each existing wiki page
            AddLink(reader(0).ToString, "./JustArticle.aspx?Organizationid=" + OrgId + "&ArticleID=" + reader(1).ToString)

        End While

        reader.Close()

        command.Connection.Close()
        connection.Close()


    End Sub

    

    Protected Sub DataList1_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataListCommandEventArgs) Handles DataList1.ItemCommand
        'This will display a previous article

        Dim OrgID As String = GetOrgID()


        Dim HistoryID As String = e.CommandArgument

        'Response.Redirect(".\viewpage.aspx?organizationid=" + OrgID + "&ArticleID=" & ArticleID)

        RadEditor1.Content = GetPreviousArticle(HistoryID, OrgID)


    End Sub




    Public Sub SetSecurityFlags(ByVal articleID As String)
        'This will set the various security flags on the screen

        Dim sqlStatement As String = "select Private, PortalView, PortalEdit, PublicView, PublicEdit from WikiArticles where ArticleID = @ArticleID"

        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        'command.Parameters.Add(New SqlParameter("@OrganizationID", OrganizationID))
        command.Parameters.Add(New SqlParameter("@ArticleID", ArticleID))

        command.Connection.Open()
        'Try



        Dim reader As SqlDataReader = command.ExecuteReader


        'return the string
        Try
            If reader.Read Then


                cb_private.Checked = (reader(0).ToString = "True")
                cb_AdvPortal.Checked = (reader(1).ToString = "True")
                cb_AdvPortalEdit.Checked = (reader(2).ToString = "True")
                cb_basicPortal.Checked = (reader(3).ToString = "True")
                cb_BasicPortalEdit.Checked = (reader(4).ToString = "True")


            End If

        Finally
            command.Connection.Close()
            connection.Close()
            reader.Close()
        End Try

    End Sub

    Public Sub SaveSecurityFlags(ByVal articleID As String)
        'This will set the various security flags on the screen

        Dim sqlStatement As String = "Update WikiArticles set Private=@Private, PortalView=@PortalView, PortalEdit=@PortalEdit, PublicView=@PublicView, PublicEdit=@PublicEdit where Articleid = @ArticleID"

        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        'command.Parameters.Add(New SqlParameter("@OrganizationID", OrganizationID))
        command.Parameters.Add(New SqlParameter("@ArticleID", articleID))
        command.Parameters.Add(New SqlParameter("@Private", cb_private.Checked))
        command.Parameters.Add(New SqlParameter("@PortalView", cb_AdvPortal.Checked))
        command.Parameters.Add(New SqlParameter("@PortalEdit", cb_AdvPortalEdit.Checked))
        command.Parameters.Add(New SqlParameter("@PublicView", cb_basicPortal.Checked))
        command.Parameters.Add(New SqlParameter("@PublicEdit", cb_BasicPortalEdit.Checked))

        connection.Open()


        Try
            command.ExecuteNonQuery()

        Finally
            command.Connection.Close()
            connection.Close()

        End Try

    End Sub

    Public Sub DeleteArticle(ByVal articleID As String)
        'This will set the IsDeleted flag to true which effectively deletes the article

        Dim sqlStatement As String = "Update WikiArticles set IsDeleted=1 where Articleid = @ArticleID"

        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        'command.Parameters.Add(New SqlParameter("@OrganizationID", OrganizationID))
        command.Parameters.Add(New SqlParameter("@ArticleID", articleID))


        connection.Open()


        Try
            command.ExecuteNonQuery()

        Finally
            command.Connection.Close()
            connection.Close()

        End Try

    End Sub


    Public Function IsArticleDeleted(ByVal ArticleID As String) As Boolean
        'Will return true if article is delete

        Dim sqlStatement As String = "select IsDeleted from WikiArticles where ArticleID = @ArticleID"

        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        'command.Parameters.Add(New SqlParameter("@OrganizationID", OrganizationID))
        command.Parameters.Add(New SqlParameter("@ArticleID", ArticleID))

        command.Connection.Open()
        'Try



        Dim reader As SqlDataReader = command.ExecuteReader


        Try
            Try

                If reader.Read Then
                    Return reader(0)
                Else
                    Return False
                End If
            Catch ex As Exception
                Return False
            End Try

        Finally
            command.Connection.Close()
            connection.Close()
            reader.Close()
        End Try
    End Function

    Public Sub SetParentID(ByVal articleID As String)
        'This will set the SubPage drop down to the correct value if the page is a subpage

        Dim sqlStatement As String = "select ParentID from WikiArticles where ArticleID = @ArticleID"

        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        'command.Parameters.Add(New SqlParameter("@OrganizationID", OrganizationID))
        command.Parameters.Add(New SqlParameter("@ArticleID", articleID))

        command.Connection.Open()
        'Try



        Dim reader As SqlDataReader = command.ExecuteReader


        Try
            If reader.Read Then
                If reader(0).ToString <> "" Then
                    If IsArticleDeleted(reader(0).ToString) Then
                        'parent article no longer exists
                        dd_ParentID.SelectedIndex = 0
                    Else
                        'article not deleted, display Parent article
                        dd_ParentID.SelectedValue = reader(0).ToString
                    End If

                Else
                    dd_ParentID.SelectedIndex = 0

                End If



            End If

        Finally
            command.Connection.Close()
            connection.Close()
            reader.Close()
        End Try

    End Sub

    Public Sub SaveParentID(ByVal articleID As String)
        'This will set the various security flags on the screen

        Dim sqlStatement As String = "Update WikiArticles set ParentID=@ParentID where Articleid = @ArticleID"

        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        'command.Parameters.Add(New SqlParameter("@OrganizationID", OrganizationID))
        command.Parameters.Add(New SqlParameter("@ArticleID", articleID))

        If dd_ParentID.Text = 0 Then
            command.Parameters.Add(New SqlParameter("@ParentID", DBNull.Value))
        Else
            command.Parameters.Add(New SqlParameter("@ParentID", dd_ParentID.Text))
        End If


        connection.Open()


        Try
            command.ExecuteNonQuery()

        Finally
            command.Connection.Close()
            connection.Close()

        End Try

    End Sub

    Public Shared Function GetLastArticleID() As Integer
        Dim sqlStatement As String = "select ident_current('WikiArticles')"
        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)

        command.Connection.Open()

        Dim reader As SqlDataReader = command.ExecuteReader
        Try
            'return the ticketid
            If reader.Read Then
                Return reader(0)
            Else
                Return ""
            End If


        Finally
            reader.Close()
            command.Connection.Close()
            connection.Close()
        End Try

    End Function

    Public Function DoesPageHaveSubPages(ByVal ArticleID As String) As Boolean

        Dim sqlStatement As String = "select * from WikiArticles where ParentID = @ArticleID"

        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        'command.Parameters.Add(New SqlParameter("@OrganizationID", OrganizationID))
        command.Parameters.Add(New SqlParameter("@ArticleID", ArticleID))

        command.Connection.Open()
        'Try



        Dim reader As SqlDataReader = command.ExecuteReader


        'return the string
        Try


            Try

                If reader.Read Then
                    Return True
                Else
                    Return False
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

    Public Sub SetUpDirectories(ByVal OrgID As String)
        'Set up the directory for images and documents

        'Set the upload directory for images
        'Dim thisDir = Server.MapPath(".")
        Dim CreateWikiDirectory As String = "C:\TSData\WikiDocs\" + OrgID + "\images"

        'Lets create a directory for users to upload their images
        Dim di As DirectoryInfo = New DirectoryInfo(CreateWikiDirectory)
        ' Determine whether the directory exists.

        If Not di.Exists Then
            di.Create()
        End If


        Dim WikiDirectory As String = Page.ResolveUrl("../Wiki/WikiDocs/" + OrgID + "/images")



        'set the properties of the image mananger
        Dim viewImages As String() = New String() {WikiDirectory}
        Dim uploadImages As String() = New String() {WikiDirectory}
        'Only set this for Admins**
        'Only set the delete path for admins
        Try
            If UserSession.CurrentUser.IsSystemAdmin Then
                Dim deleteImages As String() = New String() {WikiDirectory}
                RadEditor1.ImageManager.DeletePaths = deleteImages
            End If
        Catch ex As Exception

        End Try

        'If Not IsPostBack Then
        RadEditor1.ImageManager.ViewPaths = viewImages
        RadEditor1.ImageManager.UploadPaths = uploadImages
        RadEditor1.ImageManager.MaxUploadFileSize = 100000000 'approx 100mb.  CHanged from 5 on April 1 2013.  RJ





        'Set up the directory for documents
        'Set the upload directory for images
        'thisDir = Server.MapPath(".")
        'changed 10/13/09 - Now just hard pointing to TSData directory
        CreateWikiDirectory = "C:\TSData\WikiDocs\" + OrgID + "\documents"

        'Lets create a directory for users to upload their images
        Dim di2 As DirectoryInfo = New DirectoryInfo(CreateWikiDirectory)
        ' Determine whether the directory exists.

        If Not di2.Exists Then
            di2.Create()
        End If


        Dim DocumentDirectory As String = Page.ResolveUrl("../Wiki/WikiDocs/" + OrgID + "/documents")

        'set the properties of the image mananger
        Dim viewdocuments As String() = New String() {DocumentDirectory}
        Dim uploaddocuments As String() = New String() {DocumentDirectory}

        'Only set the delete path for admins
        Try
            If UserSession.CurrentUser.IsSystemAdmin Then
                Dim deletedocuments As String() = New String() {DocumentDirectory}
                RadEditor1.DocumentManager.DeletePaths = deletedocuments
            End If
        Catch ex As Exception

        End Try


        RadEditor1.DocumentManager.ViewPaths = viewdocuments
        RadEditor1.DocumentManager.UploadPaths = uploaddocuments
        RadEditor1.DocumentManager.MaxUploadFileSize = 100000000 '100mb.  Changed from 5 4/1/13
        Dim SearchPatterns As String() = New String() {"*.*"}
        RadEditor1.DocumentManager.SearchPatterns = SearchPatterns


        

    End Sub

    Protected Sub btn_SaveArticle_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btn_SaveArticle.Click
        Dim OrgID As String = GetOrgID()


        Dim ArticleID As String
        Try
            ArticleID = Request.QueryString("ArticleID")
        Catch ex As Exception
            ArticleID = "default"
        End Try

        Dim NewPage As Boolean
        Dim NewPageString As String
        Try
            NewPageString = Request.QueryString("New")
            NewPage = NewPageString = "yes"
        Catch ex As Exception
            NewPage = False
        End Try



        If OrgID <> "" Then

            If Not NewPage Then
                SaveArticle(tb_PageName.Text, OrgID, RadEditor1.Content, ArticleID, GetUserID)
                SaveSecurityFlags(ArticleID)
                SaveParentID(ArticleID)
                Response.Redirect("viewpage.aspx?Organizationid=" + OrgID + "&ArticleID=" + ArticleID)
            Else
                'need to check the page name and make sure it's unique
                SaveNewArticle(tb_PageName.Text, OrgID, RadEditor1.Content, GetUserID)
                ArticleID = GetLastArticleID() 'This will return the article ID we just added
                SaveSecurityFlags(ArticleID)
                SaveParentID(ArticleID)
                Response.Redirect("viewpage.aspx?Organizationid=" + OrgID + "&ArticleID=" + ArticleID)
            End If


        End If

        'now lets go back and look at the page we just saved.

    End Sub




    Protected Overloads Overrides Sub OnInit(ByVal e As EventArgs)
        MyBase.OnInit(e)
        AddHandler Me.Page.PreRenderComplete, AddressOf Page_PreRenderComplete
    End Sub

    Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As EventArgs)
        'This will let us change some of the behavior of the various Telerik dialogs
        '  See:  http://www.telerik.com/help/aspnet-ajax/editor-resizing-dialogs.html

        'Dim linkManager As Telerik.Web.UI.DialogDefinition = RadEditor1.GetDialogDefinition("LinkManager")
        'linkManager.Height = Unit.Pixel(50)
        'linkManager.Width = Unit.Pixel(50)
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


    Protected Sub btn_CancelEdit_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btn_CancelEdit.Click
        'cancel - Don't save, just go back to the main page

        Dim OrgID As String = GetOrgID()



        Dim ArticleID As String
        Try
            ArticleID = Request.QueryString("ArticleID")
        Catch ex As Exception
            ArticleID = "default"
        End Try

        Response.Redirect("viewpage.aspx?Organizationid=" + OrgID + "&ArticleID=" + ArticleID)

    End Sub

    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btn_DeleteArticle.Click
        'delete the article

        Dim OrgID As String = GetOrgID()

        Dim ArticleID As String
        Try
            ArticleID = Request.QueryString("ArticleID")
        Catch ex As Exception
            ArticleID = "default"
        End Try

        DeleteArticle(ArticleID) 'should whack the article

        Response.Redirect("viewpage.aspx?Organizationid=" + OrgID) 'no article ID.  

    End Sub

    Public Function WhoCreatedArticle(ByVal ArticleID As String) As String

        Dim sqlStatement As String = "select CreatedBy from WikiArticles where ArticleID=@ArticleID"

        Dim connection As New SqlConnection(ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString)

        Dim command As New SqlCommand(sqlStatement, connection)
        command.Parameters.Add(New SqlParameter("@ArticleID", ArticleID))



        command.Connection.Open()
        'Try



        Dim reader As SqlDataReader = command.ExecuteReader


        'return the string
        Try


            Try

                If reader.Read Then
                    Return reader(0).ToString
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


    Protected Sub ImageButton1_Click1(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click
        'help
        Response.Write("<script>window.open('http://help.teamsupport.com/wiki','_blank');</script>")
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

    Public Function DisplayLocalDateFormat(ByVal LocalTime As DateTime) As String
        'takes datetime and returns as string in correct international format
        Dim tsweb As New TSWebUtilities
        tsweb.ConnectionString = ConfigurationManager.ConnectionStrings("TeamSupportConnectionString").ConnectionString
        Dim CultureName As String = tsweb.GetCultureInfo(GetOrgID, GetUserID)
        Dim CultureInfo As New Globalization.CultureInfo(CultureName)

        Return LocalTime.ToString("g", CultureInfo)


    End Function
End Class

