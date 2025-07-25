Imports System.IO
Imports System.Text
Imports System.Xml.Serialization
Imports KeitaiWorldLauncher.My.models

Namespace My.Managers
    Public Class GameListManager
        Private Const GameListPath As String = "configs/gamelist.xml"

        Public Shared Async Function DownloadGameListAsync(url As String) As Task
            Try
                Using client As New Net.Http.HttpClient()
                    Dim fileBytes As Byte() = Await client.GetByteArrayAsync(url)
                    Await File.WriteAllBytesAsync(GameListPath, fileBytes)
                    logger.Logger.LogInfo("Updated Game list downloaded successfully!")
                End Using
            Catch ex As Exception
                logger.Logger.LogWarning($"Failed to download gamelist: {ex.Message}")
                MessageBox.Show(owner:=SplashScreen, $"Failed to download gamelist: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Function

        ' Load games from XML
        Public Async Function LoadGamesAsync() As Task(Of List(Of Game))
            If File.Exists(GameListPath) Then
                Return Await Task.Run(Function()
                                          Dim serializer As New XmlSerializer(GetType(List(Of Game)), New XmlRootAttribute("Gamelist"))
                                          Using reader As New StreamReader(GameListPath, Encoding.GetEncoding("Shift-JIS"))
                                              Return CType(serializer.Deserialize(reader), List(Of Game))
                                          End Using
                                      End Function)
            Else
                Return New List(Of Game)()
            End If
        End Function


        ' Save games to XML with Shift-JIS encoding
        Public Async Function SaveGamesAsync(games As List(Of Game)) As Task
            Await Task.Run(Sub()
                               Dim serializer As New XmlSerializer(GetType(List(Of Game)), New XmlRootAttribute("Gamelist"))
                               Using writer As New StreamWriter(GameListPath, False, Encoding.GetEncoding("Shift-JIS"))
                                   serializer.Serialize(writer, games)
                               End Using
                           End Sub)
        End Function


        ' Add a new game to the list
        Public Async Function AddGameAsync(game As Game) As Task
            Dim games = Await LoadGamesAsync()
            games.Add(game)
            Await SaveGamesAsync(games)
        End Function

    End Class
End Namespace

