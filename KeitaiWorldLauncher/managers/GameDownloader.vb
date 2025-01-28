Imports System.Net
Imports System.IO
Imports System.IO.Compression
Imports System.Threading
Imports KeitaiWorldLauncher.My.Managers

Namespace My.Managers
    Public Class GameDownloader
        Private WithEvents client As WebClient
        Private progressBar As ProgressBar
        Private downloadFilePath As String
        Private extractFolderPath As String
        Private selectedGameZipPath As String
        Private ReadJam As String
        Private Isthisbatchdownload As Boolean

        Public Sub New(progressBarControl As ProgressBar)
            client = New WebClient()
            progressBar = progressBarControl
        End Sub

        Public Async Function DownloadGameAsync(url As String, savePath As String, extractTo As String, JAMLocation As String, BatchDownload As Boolean) As Task
            Try
                ' Set paths for use in completion event
                downloadFilePath = savePath
                extractFolderPath = extractTo
                ReadJam = JAMLocation
                Isthisbatchdownload = BatchDownload

                ' Reset and show the progress bar
                progressBar.Value = 0
                progressBar.Visible = True

                ' Attach event handlers
                AddHandler client.DownloadProgressChanged, AddressOf DownloadProgressChanged
                AddHandler client.DownloadFileCompleted, AddressOf DownloadFileCompleted

                ' Start downloading the file asynchronously
                Await client.DownloadFileTaskAsync(New Uri(url), savePath)
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
                MessageBox.Show($"Failed to download the game: {e.Error.Message}", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                If Isthisbatchdownload = False Then
                    'MessageBox.Show("Game downloaded successfully!", "Download Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    ' Extract the downloaded file
                    Try
                        Thread.Sleep(100) ' Optional: Wait briefly to ensure the file is fully written
                        ZipFile.ExtractToDirectory(downloadFilePath, extractFolderPath)
                        'MessageBox.Show("Game extracted successfully!", "Extraction Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)

                        ' Delete the ZIP file after extraction
                        File.Delete(downloadFilePath)

                        UtilManager.GenerateDynamicControlsFromLines(ReadJam, Form1.gbxGameInfo)
                    Catch ex As Exception
                        MessageBox.Show($"Failed to extract the game: {ex.Message}", "Extraction Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try

                ElseIf Isthisbatchdownload = True Then
                    ' Extract the downloaded file
                    Try
                        Thread.Sleep(100) ' Optional: Wait briefly to ensure the file is fully written
                        ZipFile.ExtractToDirectory(downloadFilePath, extractFolderPath)
                        ' Delete the ZIP file after extraction
                        File.Delete(downloadFilePath)
                    Catch ex As Exception
                        MessageBox.Show($"Failed to extract the game: {ex.Message}", "Extraction Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try
                End If
            End If

            ' Detach event handlers to avoid memory leaks
            RemoveHandler client.DownloadProgressChanged, AddressOf DownloadProgressChanged
            RemoveHandler client.DownloadFileCompleted, AddressOf DownloadFileCompleted
        End Sub
    End Class
End Namespace
