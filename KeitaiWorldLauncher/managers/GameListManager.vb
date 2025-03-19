Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Xml.Serialization
Imports KeitaiWorldLauncher.My.models

Namespace My.Managers
    Public Class GameListManager
        Private Const GameListPath As String = "configs/gamelist.xml"

        Public Shared Sub DownloadGameList(url As String)
            Try
                Using client As New WebClient()
                    ' Download the file from the specified URL
                    client.DownloadFile(url, GameListPath)
                    logger.Logger.LogInfo("Updated Game list downloaded successfully!")
                End Using
            Catch ex As Exception
                ' Handle errors such as network issues or invalid URL
                logger.Logger.LogWarning($"Failed to download gamelist: {ex.Message}")
                MessageBox.Show($"Failed to download gamelist: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
        ' Load games from XML
        Public Function LoadGames() As List(Of Game)
            If File.Exists(GameListPath) Then
                Dim serializer As New XmlSerializer(GetType(List(Of Game)), New XmlRootAttribute("Gamelist"))
                Using reader As New StreamReader(GameListPath, Encoding.GetEncoding("Shift-JIS"))
                    Return CType(serializer.Deserialize(reader), List(Of Game))
                End Using
            Else
                ' Return an empty list if the file doesn't exist
                Return New List(Of Game)()
            End If
        End Function

        ' Save games to XML with Shift-JIS encoding
        Public Sub SaveGames(games As List(Of Game))
            Dim serializer As New XmlSerializer(GetType(List(Of Game)), New XmlRootAttribute("Gamelist"))
            Using writer As New StreamWriter(GameListPath, False, Encoding.GetEncoding("Shift-JIS"))
                serializer.Serialize(writer, games)
            End Using
        End Sub

        ' Add a new game to the list
        Public Sub AddGame(game As Game)
            Dim games = LoadGames()
            games.Add(game)
            SaveGames(games)
        End Sub

    End Class
End Namespace

