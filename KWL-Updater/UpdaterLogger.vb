Imports System.IO
Imports System.Text

Friend Class UpdaterLogger
    Private ReadOnly logFileName As String = $"updater_{DateTime.Now:yyyyMMdd_HHmmss_fff}_{Environment.ProcessId}.log"
    Private logFilePath As String

    Public ReadOnly Property LocationMessage As String
        Get
            If logFilePath Is Nothing Then
                Return "A log file could not be written. Please copy the error details shown here."
            End If

            Return "Diagnostic log:" & Environment.NewLine & logFilePath
        End Get
    End Property

    Public Sub Write(level As String, message As String, Optional ex As Exception = Nothing)
        Dim entry = $"{DateTimeOffset.Now:O} [{level}] {message}"
        If ex IsNot Nothing Then entry &= Environment.NewLine & ex.ToString()
        Console.WriteLine(entry)

        For Each useTempFolder In {False, True}
            Try
                Dim baseDirectory = If(useTempFolder, Path.GetTempPath(),
                                       Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData))
                If String.IsNullOrWhiteSpace(baseDirectory) Then Continue For

                Dim logDirectory = Path.Combine(baseDirectory, "KeitaiWorldLauncher", "logs")
                Directory.CreateDirectory(logDirectory)
                Dim candidatePath = Path.Combine(logDirectory, logFileName)
                File.AppendAllText(candidatePath, entry & Environment.NewLine, Encoding.UTF8)
                logFilePath = candidatePath
                Return
            Catch logEx As Exception
                ' Logging must never hide the update failure or prevent cleanup.
                Console.Error.WriteLine("Failed to write updater log: " & logEx.Message)
            End Try
        Next

        logFilePath = Nothing
    End Sub
End Class
