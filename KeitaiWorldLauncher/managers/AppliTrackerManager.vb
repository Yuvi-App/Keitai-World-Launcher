Imports System.IO
Imports System.Text
Imports KeitaiWorldLauncher.My.Models

Namespace My.Managers

    Public Class AppliTrackerManager
        Shared trackerStartTime As DateTime
        Shared trackedAppliName As String
        Shared trackedAppliPath As String
        Shared filePath As String = "configs/playtimes.txt"

        Public Shared Function LoadPlaytimes(filePath As String) As List(Of PlaytimeEntry)
            Dim entries As New List(Of PlaytimeEntry)
            If Not File.Exists(filePath) Then Return entries

            For Each line In File.ReadLines(filePath)
                Dim parts = line.Split("|"c)
                If parts.Length >= 2 Then
                    Dim playTime As TimeSpan = TimeSpan.Zero
                    If TimeSpan.TryParse(parts(1), playTime) Then
                        entries.Add(New PlaytimeEntry With {
                    .AppName = parts(0),
                    .PlayTime = playTime,
                    .Sessions = If(parts.Length >= 3, Integer.Parse(parts(2)), 1)
                })
                    End If
                End If
            Next
            Return entries
        End Function

        Public Sub StartTrackingAppli(AppliPath As String)
            trackedAppliPath = AppliPath
            trackedAppliName = Path.GetFileNameWithoutExtension(AppliPath)
            trackerStartTime = DateTime.Now
        End Sub

        Public Async Function StopTrackingAppliAsync() As Task
            Dim trackedTime As TimeSpan = DateTime.Now - trackerStartTime
            Await SavePlaytimeAsync(trackedAppliName, trackedTime)
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
