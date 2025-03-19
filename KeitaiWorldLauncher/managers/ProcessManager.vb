Imports System.Threading
Imports System.Xml
Imports KeitaiWorldLauncher.My.logger

Namespace My.Managers
    Public Class ProcessManager
        Private Shared monitorThread As Thread
        Private Shared monitoring As Boolean = True
        Public Shared Sub StartMonitoring()
            Thread.Sleep(10000)
            monitorThread = New Thread(AddressOf MonitorProcesses)
            monitorThread.IsBackground = True
            monitorThread.Start()
        End Sub

        Private Shared Sub MonitorProcesses()
            Try
                While monitoring
                    ' Check if doja.exe and star.exe are running
                    Dim dojaRunning As Boolean = Process.GetProcessesByName("doja").Length > 0
                    Dim starRunning As Boolean = Process.GetProcessesByName("star").Length > 0

                    ' If BOTH are NOT running, close shaderglass.exe
                    If Not dojaRunning AndAlso Not starRunning Then
                        CloseProcess("shaderglass")
                        monitoring = False ' Stop monitoring
                        Exit While
                    End If

                    Thread.Sleep(2000) ' Check every 2 seconds
                End While
            Catch ex As Exception
                MessageBox.Show("Error monitoring processes: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                monitoring = False ' Ensure flag is reset
            End Try
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


