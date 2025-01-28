Imports System.Net
Imports System.IO
Imports System.IO.Compression
Imports System.Threading
Imports KeitaiWorldLauncher.My.Managers

Namespace My.Managers
    Public Class MachiCharaDownloader
        Private WithEvents client As WebClient
        Private progressBar As ProgressBar
        Private downloadFilePath As String
        Private isBatchDownload As Boolean


        Public Sub New(progressBarControl As ProgressBar)
            client = New WebClient()
            progressBar = progressBarControl
        End Sub

        Public Async Function DownloadMachiCharaAsync(url As String, savePath As String, BatchDownload As Boolean) As Task
            Try
                ' Set paths for use in completion event
                downloadFilePath = savePath
                isBatchDownload = BatchDownload

                ' Reset and show the progress bar
                progressBar.Value = 0
                progressBar.Visible = True

                ' Attach event handlers
                AddHandler client.DownloadProgressChanged, AddressOf DownloadProgressChanged
                AddHandler client.DownloadFileCompleted, AddressOf DownloadFileCompleted

                ' Start downloading the file asynchronously
                client.DownloadFileTaskAsync(New Uri(url), savePath)
            Catch ex As Exception
                MessageBox.Show($"Failed to start download: {ex.Message}", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                progressBar.Visible = False
            End Try
        End Function

        Private Sub DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs)
            ' Update the progress bar
            progressBar.Value = e.ProgressPercentage
        End Sub

        Private Sub DownloadFileCompleted(sender As Object, e As System.ComponentModel.AsyncCompletedEventArgs)
            ' Hide the progress bar
            progressBar.Visible = False

            If e.Error IsNot Nothing Then
                MessageBox.Show($"Failed to download the Machi Chara: {e.Error.Message}", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                If isBatchDownload = False Then
                    MessageBox.Show("Machi Chara downloaded successfully!", "Download Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End If

            ' Detach event handlers to avoid memory leaks
            RemoveHandler client.DownloadProgressChanged, AddressOf DownloadProgressChanged
            RemoveHandler client.DownloadFileCompleted, AddressOf DownloadFileCompleted
        End Sub
    End Class
End Namespace
