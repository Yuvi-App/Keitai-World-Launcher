Imports System.Threading
Imports System.Xml
Imports KeitaiWorldLauncher.My.logger

Namespace My.Managers
    Public Class ProcessManager
        Private Shared monitorThread As Thread = Nothing
        Private Shared monitoring As Boolean = False
        Public Shared Sub StartMonitoring()
            ' If already monitoring, or the thread is still alive, don't start another one
            If monitoring OrElse (monitorThread IsNot Nothing AndAlso monitorThread.IsAlive) Then Exit Sub

            monitoring = True
            monitorThread = New Thread(AddressOf MonitorProcesses)
            monitorThread.IsBackground = True
            monitorThread.Start()
        End Sub

        Private Shared Sub MonitorProcesses()
            Try
                While monitoring
                    Dim dojaRunning As Boolean = Process.GetProcessesByName("doja").Length > 0
                    Dim starRunning As Boolean = Process.GetProcessesByName("star").Length > 0
                    Dim javaRunning As Boolean = Process.GetProcessesByName("java").Length > 0
                    Dim flashplayerRunning As Boolean = Process.GetProcessesByName("flashplayer").Length > 0
                    ' Only close shaderglass.exe if both processes are NOT running
                    If Not dojaRunning AndAlso Not starRunning AndAlso Not javaRunning AndAlso Not flashplayerRunning Then
                        CloseProcess("shaderglass")
                        monitoring = False
                        Exit While
                    End If

                    Thread.Sleep(2000) ' Check every 2 seconds
                End While
            Catch ex As Exception
                ' Optionally, log the error
                logger.Logger.LogError($"Failed to Close Shaderglass via processmanger {ex}")
            Finally
                ' Reset monitoring state so the monitor can be restarted later
                monitoring = False
                monitorThread = Nothing
            End Try
        End Sub

        Public Shared Sub StopMonitoring()
            monitoring = False
            If monitorThread IsNot Nothing AndAlso monitorThread.IsAlive Then
                monitorThread.Join()
                monitorThread = Nothing
            End If
        End Sub

        Private Shared Sub CloseProcess(processName As String)
            Try
                For Each proc As Process In Process.GetProcessesByName(processName)
                    proc.Kill() ' Forcefully close the process
                    proc.WaitForExit() ' Ensure it exits
                Next
            Catch ex As Exception
                MessageBox.Show("Error closing " & processName & ": " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
    End Class
End Namespace


