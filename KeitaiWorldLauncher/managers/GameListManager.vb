Imports System.IO
Imports System.Text
Imports System.Xml.Serialization
Imports KeitaiWorldLauncher.My.models

Namespace My.Managers
    Public Class GameListManager
        Private Const GameListPath As String = "configs/gamelist.xml"

        ' Download gamelist from URL and save to local path
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

        ' Remove Game from List (for use with custom game list)
        Public Async Function RemoveGameAsync(gameTitle As String) As Task
            ' 1) Load existing list
            Dim games = Await LoadGamesAsync()

            ' 2) Filter out the one(s) matching our title
            Dim filtered = games.
                Where(Function(g) Not g.ENTitle.Equals(gameTitle, StringComparison.OrdinalIgnoreCase)) _
                .ToList()

            ' 3) Only re-save if something actually changed
            If filtered.Count <> games.Count Then
                Await SaveGamesAsync(filtered)
            End If
        End Function

        ' Load custom game to list (for use with custom game list)
        Public Async Function LoadCustomGamesAsync(customGamesFile As String, downloadsFolder As String) As Task
            ' 1) Ensure the file exists
            If Not File.Exists(customGamesFile) Then
                Using stream = File.Create(customGamesFile)
                End Using
            End If

            ' 2) Read + dedupe
            Dim allLines As String() = Await File.ReadAllLinesAsync(customGamesFile)
            Dim customGameLines As List(Of String) = allLines _
            .Select(Function(line) line.Trim()) _
            .Where(Function(line) Not String.IsNullOrWhiteSpace(line)) _
            .Distinct(StringComparer.OrdinalIgnoreCase) _
            .ToList()

            If customGameLines.Count <> allLines.Length Then
                Await File.WriteAllLinesAsync(customGamesFile, customGameLines)
            End If

            ' 3) Supported emulator set
            Dim supportedEmus = New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
                "doja", "star", "jsky", "airedge", "vodafone", "softbank", "flash"
            }

            ' 4) Process each line
            For Each entry In customGameLines
                Dim parts = entry.Split("|"c)

                Dim appName As String = parts(0).Trim()
                If String.IsNullOrWhiteSpace(appName) Then Continue For

                Dim emulatorValue As String = "doja" ' default
                If parts.Length > 1 AndAlso Not String.IsNullOrWhiteSpace(parts(1)) Then
                    Dim candidate = parts(1).Trim().ToLowerInvariant()
                    If supportedEmus.Contains(candidate) Then
                        emulatorValue = candidate
                    End If
                End If

                ' NEW format folder name: appName_emulator
                Dim gameKey As String = $"{appName}_{emulatorValue}"
                Dim gameFolderPath As String = Path.Combine(downloadsFolder, gameKey)

                ' Only add if the folder actually exists
                If Directory.Exists(gameFolderPath) Then
                    Dim game As New Game With {
                    .ENTitle = appName,
                    .ZIPName = appName & ".zip",
                    .DownloadURL = "",
                    .CustomAppIconURL = "",
                    .SDCardDataURL = "",
                    .Emulator = emulatorValue,
                    .Variants = ""
                }

                    Await AddGameAsync(game)
                End If
            Next
        End Function

    End Class
End Namespace

