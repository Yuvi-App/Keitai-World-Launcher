Imports System.IO

Namespace My.logger
    Public Class Logger
        Private Shared logFilePath As String = "logs\app_log.txt"
        ' Initialize the logger with a custom log file path (optional)
        Public Shared Sub InitializeLogger(Optional filePath As String = "logs\app_log.txt")
            logFilePath = filePath
        End Sub

        ' Method to log general messages
        Public Shared Sub LogInfo(message As String)
            Log("INFO", message)
        End Sub

        ' Method to log warnings
        Public Shared Sub LogWarning(message As String)
            Log("WARNING", message)
        End Sub

        ' Method to log errors
        Public Shared Sub LogError(message As String, Optional ex As Exception = Nothing)
            If ex IsNot Nothing Then
                Log("ERROR", $"{message}: {ex.Message}{vbNewLine}{ex.StackTrace}")
            Else
                Log("ERROR", message)
            End If
        End Sub

        ' Core logging method
        Private Shared Sub Log(logType As String, message As String)
            Dim logMessage As String = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logType}] {message}"
            Try
                SyncLock GetType(Logger)  ' Ensures thread safety when multiple threads write to the log
                    File.AppendAllText(logFilePath, logMessage & Environment.NewLine)
                End SyncLock
            Catch ioEx As IOException
                ' Handle file IO exceptions if needed
                Console.WriteLine($"Failed to write to log: {ioEx.Message}")
            End Try
        End Sub
    End Class
End Namespace

