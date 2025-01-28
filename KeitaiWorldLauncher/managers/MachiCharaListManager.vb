Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Xml.Serialization
Imports KeitaiWorldLauncher.My.models

Namespace My.Managers
    Public Class MachiCharaListManager
        Private Const MachiCharaListPath As String = "configs/machicharalist.xml"

        Public Shared Sub DownloadMachiCharaList(url As String)
            Try
                Using client As New WebClient()
                    ' Download the file from the specified URL
                    client.DownloadFile(url, MachiCharaListPath)
                    'MessageBox.Show("Updated Machi Chara list downloaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End Using
            Catch ex As Exception
                ' Handle errors such as network issues or invalid URL
                MessageBox.Show($"Failed to download Machi Chara list: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
        ' Load games from XML
        Public Function LoadMachiChara() As List(Of MachiChara)
            If File.Exists(MachiCharaListPath) Then
                Dim serializer As New XmlSerializer(GetType(List(Of MachiChara)), New XmlRootAttribute("MachiCharalist"))
                Using reader As New StreamReader(MachiCharaListPath, Encoding.GetEncoding("Shift-JIS"))
                    Return CType(serializer.Deserialize(reader), List(Of MachiChara))
                End Using
            Else
                ' Return an empty list if the file doesn't exist
                Return New List(Of MachiChara)()
            End If
        End Function

        ' Save games to XML with Shift-JIS encoding
        Public Sub SaveGames(games As List(Of MachiChara))
            Dim serializer As New XmlSerializer(GetType(List(Of MachiChara)), New XmlRootAttribute("MachiCharalist"))
            Using writer As New StreamWriter(MachiCharaListPath, False, Encoding.GetEncoding("Shift-JIS"))
                serializer.Serialize(writer, games)
            End Using
        End Sub

        ' Add a new game to the list
        Public Sub AddGame(game As MachiChara)
            Dim games = LoadMachiChara()
            games.Add(game)
            SaveGames(games)
        End Sub

    End Class
End Namespace

