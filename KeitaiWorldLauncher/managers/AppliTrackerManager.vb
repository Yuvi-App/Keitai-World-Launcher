Imports System.IO
Imports System.Text
Imports KeitaiWorldLauncher.My.Models

Namespace My.Managers

    Public Class AppliTrackerManager
        Shared trackerStartTime As DateTime
        Shared trackedAppliName As String
        Shared trackedAppliPath As String
        Shared filePath As String = "configs/playtimes.txt"
        Shared isTracking As Boolean

        Public Shared Function LoadPlaytimes(filePath As String) As List(Of PlaytimeEntry)
            Dim entries As New List(Of PlaytimeEntry)
            If Not File.Exists(filePath) Then Return entries

            For Each line In File.ReadLines(filePath)
                Dim parts = line.Split("|"c)
                If parts.Length < 2 Then Continue For

                Dim appName = parts(0).Trim()
                If String.IsNullOrWhiteSpace(appName) Then Continue For

                Dim playTime As TimeSpan = TimeSpan.Zero
                If Not TimeSpan.TryParse(parts(1), playTime) OrElse playTime < TimeSpan.Zero Then Continue For

                Dim sessions = 1
                If parts.Length >= 3 Then
                    Dim parsedSessions As Integer
                    If Integer.TryParse(parts(2), parsedSessions) AndAlso parsedSessions > 0 Then
                        sessions = parsedSessions
                    End If
                End If

                entries.Add(New PlaytimeEntry With {
                    .AppName = appName,
                    .PlayTime = playTime,
                    .Sessions = sessions
                })
            Next
            Return entries
        End Function

        Public Sub StartTrackingAppli(AppliPath As String)
            trackedAppliPath = AppliPath
            trackedAppliName = Path.GetFileNameWithoutExtension(AppliPath)
            trackerStartTime = DateTime.Now
            isTracking = Not String.IsNullOrWhiteSpace(trackedAppliName)
        End Sub

        Public Async Function StopTrackingAppliAsync() As Task
            If Not isTracking OrElse String.IsNullOrWhiteSpace(trackedAppliName) Then Return

            isTracking = False
            Dim trackedTime As TimeSpan = DateTime.Now - trackerStartTime
            Dim completedAppliName = trackedAppliName
            trackedAppliName = Nothing
            trackedAppliPath = Nothing
            If trackedTime <= TimeSpan.Zero Then Return

            Await SavePlaytimeAsync(completedAppliName, trackedTime)
        End Function

        Private Async Function SavePlaytimeAsync(appliName As String, sessionTime As TimeSpan) As Task
            Dim lines As New List(Of String)()
            Dim updated As Boolean = False

            If File.Exists(filePath) Then
                Using reader As New StreamReader(filePath, Encoding.UTF8)
                    While Not reader.EndOfStream
                        lines.Add(Await reader.ReadLineAsync())
                    End While
                End Using
            End If

            For i As Integer = 0 To lines.Count - 1
                If lines(i).StartsWith(appliName & "|") Then
                    Dim parts = lines(i).Split("|"c)
                    Dim prevTime As TimeSpan = TimeSpan.Parse(parts(1))
                    Dim sessionCount As Integer = If(parts.Length >= 3, Integer.Parse(parts(2)), 0)

                    Dim newTime As TimeSpan = prevTime.Add(sessionTime)
                    sessionCount += 1

                    lines(i) = $"{appliName}|{newTime}|{sessionCount}"
                    updated = True
                    Exit For
                End If
            Next

            If Not updated Then
                lines.Add($"{appliName}|{sessionTime}|1")
            End If

            Using writer As New StreamWriter(filePath, False, Encoding.UTF8)
                For Each line In lines
                    Await writer.WriteLineAsync(line)
                Next
            End Using
        End Function
    End Class

End Namespace
