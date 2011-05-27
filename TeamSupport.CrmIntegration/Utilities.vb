Imports System.Xml
Imports System.Net
Imports System.IO
Imports System.Text

Public Class Utilities
    Protected Const Client As String = "Muroc Client"

    Public Shared Function GetXML(ByRef Key As NetworkCredential, ByRef Address As Uri) As XmlDocument
        Dim returnXML As XmlDocument = Nothing

        If Address IsNot Nothing Then
            Dim request As HttpWebRequest = WebRequest.Create(Address)
            request.Credentials = Key
            request.Method = "GET"
            request.KeepAlive = False
            request.UserAgent = Client

            Using response As HttpWebResponse = request.GetResponse()

                If request.HaveResponse AndAlso response IsNot Nothing Then
                    Using reader As New StreamReader(response.GetResponseStream())

                        returnXML = New XmlDocument()
                        returnXML.LoadXml(reader.ReadToEnd())
                    End Using
                End If

            End Using

        End If
        Return returnXML
    End Function

    Public Shared Function PostXML(ByRef Key As NetworkCredential, ByRef Address As Uri, ByRef Content As String) As HttpStatusCode
        Dim returnStatus As HttpStatusCode = Nothing

        If Address IsNot Nothing And Content <> "" Then
            Dim byteData = UTF8Encoding.UTF8.GetBytes(Content)

            Dim request As HttpWebRequest = WebRequest.Create(Address)
            request.Credentials = Key
            request.Method = "POST"
            request.ContentType = "application/xml"
            request.UserAgent = Client
            request.ContentLength = byteData.Length

            Using postStream As Stream = request.GetRequestStream()
                postStream.Write(byteData, 0, byteData.Length)
            End Using

            Using response As HttpWebResponse = request.GetResponse()
                If request.HaveResponse AndAlso response IsNot Nothing Then
                    returnStatus = response.StatusCode
                End If
            End Using

        End If

        Return returnStatus
    End Function

    Public Shared Function StripHTML(ByRef Content As String) As String
        Content = Web.HttpUtility.HtmlDecode(Content)
        Content = StripComments(Content)

        'strip other tags
        'regex based on http://stackoverflow.com/questions/787932/using-c-regular-expressions-to-remove-html-tags/787949#787949
        Content = RegularExpressions.Regex.Replace(Content, "</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>", "", RegularExpressions.RegexOptions.Singleline)

        Return Content
    End Function

    Public Shared Function StripComments(ByRef Content As String) As String
        Content = RegularExpressions.Regex.Replace(Content, "<!--.*?-->", " ")
        Return Content
    End Function
End Class
