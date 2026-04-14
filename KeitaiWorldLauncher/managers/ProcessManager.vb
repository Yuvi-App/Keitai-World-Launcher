Imports System.Threading


Namespace My.Managers
    Public Class ProcessManager
        Private Shared applitrackerManager As New AppliTrackerManager()
        Private Shared _cts As CancellationTokenSource = Nothing
        Private Shared _monitorTask As Task = Nothing
        Private Shared ReadOnly _monitorLock As New Object()

        Public Shared ReadOnly EmulatorProcessNames As String() = {
            "doja", "squirreljme", "star", "java", "javaw", "jbime", "emulator", "flashplayer"
        }

        Public Shared Sub StartMonitoring(applipath As String)
            SyncLock _monitorLock
                ' If already monitoring, don't start another
                If _monitorTask IsNot Nothing AndAlso Not _monitorTask.IsCompleted Then Exit Sub

                _cts = New CancellationTokenSource()
                _monitorTask = MonitorProcessesAsync(_cts.Token)
            End SyncLock

            applitrackerManager.StartTrackingAppli(applipath)
        End Sub
        Private Shared Async Function MonitorProcessesAsync(token As CancellationToken) As Task
            Try
                While Not token.IsCancellationRequested
                    Dim anyRunning = EmulatorProcessNames.Any(
                        Function(name) Process.GetProcessesByName(name).Length > 0)

                    If Not anyRunning Then
                        KillProcessesByName("shaderglass", "shaderglass.exe")
                        Await applitrackerManager.StopTrackingAppliAsync()
                        Exit While
                    End If

                    Await Task.Delay(2000, token)
                End While
            Catch ex As TaskCanceledException
                ' Expected on cancellation
            Catch ex As Exception
                logger.Logger.LogError($"Error in process monitor: {ex}")
            End Try
        End Function
        Public Shared Sub StopMonitoring()
            Dim ctsToCancel As CancellationTokenSource = Nothing
            Dim taskToObserve As Task = Nothing

            SyncLock _monitorLock
                ctsToCancel = _cts
                taskToObserve = _monitorTask
                _cts = Nothing
                _monitorTask = Nothing
            End SyncLock

            Try
                ctsToCancel?.Cancel()
            Catch ex As ObjectDisposedException
                ' Already disposed
            End Try

            If ctsToCancel IsNot Nothing Then
                ctsToCancel.Dispose()
            End If

            ' Observe completion/faults without blocking UI thread.
            If taskToObserve IsNot Nothing Then
                taskToObserve.ContinueWith(
                    Sub(t)
                        If t.IsFaulted AndAlso t.Exception IsNot Nothing Then
                            logger.Logger.LogError($"Error while stopping process monitor: {t.Exception.GetBaseException().Message}")
                        End If
                    End Sub,
                    TaskScheduler.Default)
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
        Private Shared Function KillProcessesByName(processName As String, friendlyName As String) As Boolean
            Dim processes = Process.GetProcessesByName(processName)

            If processes.Length = 0 Then
                logger.Logger.LogInfo($"{friendlyName} is not currently running.")
                Return False
            End If

            logger.Logger.LogWarning($"Found {processes.Length} instance(s) of {friendlyName} running.")
            Try
                For Each proc As Process In processes
                    logger.Logger.LogInfo($"Closing PID={proc.Id} Name={proc.ProcessName}")
                    proc.Kill()
                    proc.WaitForExit()
                Next
                logger.Logger.LogInfo($"All {friendlyName} processes closed successfully.")
                Return False
            Catch ex As Exception
                logger.Logger.LogError($"Error closing {friendlyName}: {ex}")
                MessageBox.Show($"An error occurred while closing {friendlyName}: {ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return True
            End Try
        End Function



        ' Close Process
        Public Shared Function CheckAndCloseShaderGlass() As Boolean
            Return KillProcessesByName("shaderglass", "shaderglass.exe")
        End Function
        Public Shared Function CheckAndCloseAMX() As Boolean
            Return KillProcessesByName("antimicrox", "antimicrox.exe")
        End Function
        Public Shared Function CheckAndCloseAHK() As Boolean
            Return KillProcessesByName("AutoHotkey32", "AutoHotkey32.exe")
        End Function
        Public Shared Function CheckAndCloseAllEmulators() As Boolean
            Dim emulatorProcesses As New Dictionary(Of String, String()) From {
                {"DOJA Emulator", {"doja"}},
                {"SquirrelJME", {"squirreljme"}},
                {"Star Emulator", {"star", "full"}},
                {"Java-based runtime", {"java", "javaw", "jbime"}},
                {"Vodafone Emulator", {"emulator"}},
                {"Flashplayer", {"flashplayer"}}
            }

            Dim currentPid = Process.GetCurrentProcess().Id
            Dim anyStillRunning As Boolean = False

            For Each kvp In emulatorProcesses
                Dim friendlyName = kvp.Key
                Dim processNames = kvp.Value
                Dim foundProcesses As New List(Of Process)

                For Each name In processNames
                    foundProcesses.AddRange(Process.GetProcessesByName(name))
                Next

                If foundProcesses.Count = 0 Then
                    logger.Logger.LogInfo($"{friendlyName} is not currently running.")
                    Continue For
                End If

                logger.Logger.LogWarning($"Found {foundProcesses.Count} instance(s) of {friendlyName} running.")
                Dim result = MessageBox.Show($"{friendlyName} is currently running. Do you want to close it?",
                                             "Confirm Close", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

                If result = DialogResult.Yes Then
                    Try
                        logger.Logger.LogInfo($"User agreed to close {friendlyName}.")
                        CheckAndCloseShaderGlass()
                        CheckAndCloseAMX()

                        Dim closedCount As Integer = 0
                        For Each proc In foundProcesses
                            If proc.Id = currentPid Then
                                logger.Logger.LogInfo($"Skipping self (PID={proc.Id})")
                                Continue For
                            End If
                            Try
                                logger.Logger.LogInfo($"Closing PID={proc.Id} Name={proc.ProcessName}")
                                proc.Kill()
                                proc.WaitForExit()
                                closedCount += 1
                            Catch ex As Exception
                                logger.Logger.LogError($"Failed to close {proc.ProcessName} (PID={proc.Id}): {ex.Message}")
                                anyStillRunning = True
                            End Try
                        Next
                        logger.Logger.LogInfo($"Closed {closedCount} of {foundProcesses.Count} {friendlyName} process(es).")
                    Catch ex As Exception
                        logger.Logger.LogError($"Error closing {friendlyName}: {ex}")
                        MessageBox.Show($"An error occurred while closing {friendlyName}: {ex.Message}",
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        anyStillRunning = True
                    End Try
                Else
                    logger.Logger.LogInfo($"User chose not to close {friendlyName}.")
                    anyStillRunning = True
                End If
            Next

            Return anyStillRunning
        End Function
    End Class
End Namespace


