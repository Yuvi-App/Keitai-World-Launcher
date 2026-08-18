Imports System.IO
Imports System.Net.Http

Namespace My.Managers
    Public Class FileDownloader
        Private ReadOnly _progress As IProgress(Of DownloadProgressInfo)

        Public Sub New(progress As IProgress(Of DownloadProgressInfo))
            _progress = progress
        End Sub

        Public Async Function DownloadFileAsync(url As String, savePath As String, contentName As String) As Task
            Dim partialPath = savePath & ".part"
            Try
                Dim parentFolder = Path.GetDirectoryName(savePath)
                If Not String.IsNullOrWhiteSpace(parentFolder) Then Directory.CreateDirectory(parentFolder)
                If File.Exists(partialPath) Then File.Delete(partialPath)
                ReportProgress($"Downloading {contentName}...", 0)

                ' Download using shared HttpClient
                Using response = Await Http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead)
                    response.EnsureSuccessStatusCode()
                    Dim totalBytes = response.Content.Headers.ContentLength
                    Using contentStream = Await response.Content.ReadAsStreamAsync()
                        Using fileStream As New FileStream(partialPath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, True)
                            Dim buffer(81919) As Byte
                            Dim downloadedBytes As Long = 0
                            While True
                                Dim bytesRead = Await contentStream.ReadAsync(buffer.AsMemory(0, buffer.Length))
                                If bytesRead = 0 Then Exit While
                                Await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead))
                                downloadedBytes += bytesRead

                                Dim percentage = -1
                                If totalBytes.HasValue AndAlso totalBytes.Value > 0 Then
                                    percentage = Math.Min(100, CInt((downloadedBytes * 100L) \ totalBytes.Value))
                                End If
                                ReportProgress($"Downloading {contentName}...", percentage)
                            End While
                        End Using
                    End Using
                End Using

                File.Move(partialPath, savePath, True)

            Catch ex As Exception
                logger.Logger.LogError($"[Download] Exception occurred during download:{vbCrLf}{ex}")
                Throw
            Finally
                If File.Exists(partialPath) Then File.Delete(partialPath)
            End Try
        End Function

        Private Sub ReportProgress(statusText As String, percentage As Integer)
            _progress?.Report(New DownloadProgressInfo With {
                .Phase = DownloadOperationPhase.Downloading,
                .StatusText = statusText,
                .Percentage = percentage
            })
        End Sub
    End Class
End Namespace
