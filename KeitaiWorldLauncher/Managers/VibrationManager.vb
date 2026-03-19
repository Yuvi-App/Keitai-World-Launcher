Imports System.IO
Imports System.Threading
Imports SharpDX.XInput

Namespace My.Managers
    Public Class VibrationManager
        Private _cancellationSource As CancellationTokenSource
        Private _vibrationLock As New Object()
        Private _vibrating As Boolean = False
        Private _controller As Controller = Nothing

        Public Sub StartMonitoring(bmpPath As String)
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

            _cancellationSource = New CancellationTokenSource()
            Dim token = _cancellationSource.Token

            ' Start polling on a background task
            Task.Run(Function() MonitorLoopAsync(bmpPath, token))

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
            Try
                _cancellationSource?.Cancel()
                _cancellationSource?.Dispose()
            Catch
            End Try
            _cancellationSource = Nothing

            ' Force stop any vibration
            Try
                If _controller IsNot Nothing AndAlso _controller.IsConnected Then
                    _controller.SetVibration(New Vibration())
                End If
            Catch
            End Try
            _controller = Nothing
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