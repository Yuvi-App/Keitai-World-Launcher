Imports System.IO
Imports System.Threading
Imports SharpDX.XInput

Namespace My.Managers
    Public Class VibrationManager
        Private _cancellationSource As CancellationTokenSource
        Private _monitorTask As Task = Nothing
        Private _vibrationLock As New Object()
        Private ReadOnly _stateLock As New Object()
        Private _vibrating As Boolean = False
        Private _controller As Controller = Nothing

        Public Sub StartMonitoring(bmpPath As String)
            ' Ensure we never have multiple monitor loops running at the same time.
            StopMonitoring()

            If Not File.Exists(bmpPath) Then
                logger.Logger.LogWarning($"[Vibration] BMP file not found: {bmpPath}")
                Return
            End If

            ' Get the selected controller index from the UI
            Dim selectedName = MainForm.cbxGameControllers.SelectedItem?.ToString()
            Dim controllerIndex As Integer = 0
            If selectedName IsNot Nothing AndAlso MainForm.XInputDevices.ContainsKey(selectedName) Then
                controllerIndex = MainForm.XInputDevices(selectedName)
            End If

            _controller = New Controller(CType(controllerIndex, UserIndex))
            If Not _controller.IsConnected Then
                logger.Logger.LogWarning("[Vibration] Selected controller not connected.")
                Return
            End If

            Dim cts As New CancellationTokenSource()
            Dim token = cts.Token

            ' Start polling on a background task
            Dim tasks = Task.Run(Function() MonitorLoopAsync(bmpPath, token))
            SyncLock _stateLock
                _cancellationSource = cts
                _monitorTask = tasks
            End SyncLock

            logger.Logger.LogInfo($"[Vibration] Monitoring started on controller {controllerIndex}")
        End Sub

        Private Async Function MonitorLoopAsync(bmpPath As String, token As CancellationToken) As Task
            Dim lastAccessTime = File.GetLastAccessTime(bmpPath)

            Try
                While Not token.IsCancellationRequested
                    Await Task.Delay(100, token)

                    Dim currentAccess = File.GetLastAccessTime(bmpPath)
                    If currentAccess > lastAccessTime Then
                        lastAccessTime = currentAccess
                        Await TriggerVibrationAsync(token)
                    End If
                End While
            Catch ex As TaskCanceledException
                ' Expected on cancel
            Catch ex As Exception
                logger.Logger.LogError($"[Vibration] Monitor error: {ex.Message}")
            End Try
        End Function

        Public Sub StopMonitoring()
            Dim ctsToCancel As CancellationTokenSource = Nothing
            Dim taskToObserve As Task = Nothing

            SyncLock _stateLock
                ctsToCancel = _cancellationSource
                taskToObserve = _monitorTask
                _cancellationSource = Nothing
                _monitorTask = Nothing
            End SyncLock

            Try
                ctsToCancel?.Cancel()
            Catch
            End Try

            Try
                ctsToCancel?.Dispose()
            Catch
            End Try

            ' Force stop any vibration
            Try
                If _controller IsNot Nothing AndAlso _controller.IsConnected Then
                    _controller.SetVibration(New Vibration())
                End If
            Catch
            End Try
            SyncLock _vibrationLock
                _vibrating = False
            End SyncLock
            _controller = Nothing

            ' Observe completion/faults without blocking callers (UI thread safe).
            If taskToObserve IsNot Nothing Then
                taskToObserve.ContinueWith(
                    Sub(t)
                        If t.IsFaulted AndAlso t.Exception IsNot Nothing Then
                            logger.Logger.LogError($"[Vibration] Monitor task faulted while stopping: {t.Exception.GetBaseException().Message}")
                        End If
                    End Sub,
                    TaskScheduler.Default)
            End If
        End Sub

        Private Async Function TriggerVibrationAsync(token As CancellationToken) As Task
            SyncLock _vibrationLock
                If _vibrating Then Return
                _vibrating = True
            End SyncLock

            Try
                If _controller IsNot Nothing AndAlso _controller.IsConnected Then
                    _controller.SetVibration(New Vibration With {
                        .LeftMotorSpeed = 65535,
                        .RightMotorSpeed = 65535
                    })

                    Await Task.Delay(250, token)

                    _controller.SetVibration(New Vibration())
                End If
            Catch ex As TaskCanceledException
                If _controller IsNot Nothing AndAlso _controller.IsConnected Then
                    _controller.SetVibration(New Vibration())
                End If
            Finally
                SyncLock _vibrationLock
                    _vibrating = False
                End SyncLock
            End Try
        End Function
    End Class
End Namespace
