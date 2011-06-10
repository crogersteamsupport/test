Imports System.Xml
Imports System.Net
Imports System.IO
Imports System.Text

Namespace TeamSupport
    Namespace CrmIntegration
        Public Class Utilities

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

        Public Class SyncLog
            Private LogPath As String
            Private FileName As String

            Public Sub New(ByVal Path As String)
                LogPath = Path
                FileName = "CRM Sync Debug File - " & Today.Month.ToString() & Today.Day.ToString() & Today.Year.ToString() & ".txt"

                If (Not Directory.Exists(LogPath)) Then
                    Directory.CreateDirectory(LogPath)
                End If
            End Sub

            Public Sub Write(ByVal Text As String)
                File.AppendAllText(LogPath & "\" & FileName, Now.ToString + ": " + Text + Environment.NewLine)
            End Sub
        End Class
    End Namespace
End Namespace