Imports System.Net

Namespace My.Managers
    Public Class MachiCharaDownloader
        Private WithEvents client As WebClient
        Private progressBar As ProgressBar
        Private downloadFilePath As String
        Private isBatchDownload As Boolean
        Dim overlay As New Panel With {
            .Dock = DockStyle.Fill,
            .BackColor = Color.FromArgb(160, Color.Black), ' Semi-transparent black
            .Visible = False
        }
        Private expectClient2Download As Boolean = False
        Private client1Completed As Boolean = False
        Private client2Completed As Boolean = False

        Public Sub New(progressBarControl As ProgressBar)
            client = New WebClient()
            progressBar = progressBarControl
        End Sub

        Public Async Function DownloadMachiCharaAsync(url As String, savePath As String, BatchDownload As Boolean) As Task
            Try
                ' Create overlay panel
                overlay = New Panel With {
                    .Dock = DockStyle.Fill,
                    .BackColor = Color.FromArgb(160, Color.White),
                    .Visible = True
                }
                MainForm.Controls.Add(overlay)
                overlay.BringToFront()
                Dim loadingLabel As New Label With {
                    .Text = "Downloading...",
                    .ForeColor = Color.Black,
                    .Font = New Font("Segoe UI", 14, FontStyle.Bold),
                    .BackColor = Color.Transparent,
                    .AutoSize = True
                }
                overlay.Controls.Add(loadingLabel)
                progressBar.Style = ProgressBarStyle.Marquee
                progressBar.MarqueeAnimationSpeed = 30
                progressBar.Visible = True
                overlay.Controls.Add(progressBar)
                Dim centerControls = Sub()
                                         progressBar.Left = (overlay.Width - progressBar.Width) \ 2
                                         progressBar.Top = (overlay.Height - progressBar.Height) \ 2

                                         loadingLabel.Left = (overlay.Width - loadingLabel.Width) \ 2
                                         loadingLabel.Top = progressBar.Top - loadingLabel.Height - 10
                                     End Sub
                centerControls()
                AddHandler overlay.Resize, Sub() centerControls()

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
                Await client.DownloadFileTaskAsync(New Uri(url), savePath)
            Catch ex As Exception
                logger.Logger.LogError($"[Download] Exception occurred during download:{vbCrLf}{ex}")
                MessageBox.Show($"Failed to start download: {ex.Message}", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                If overlay IsNot Nothing Then
                    overlay.Visible = False
                End If

                If progressBar IsNot Nothing Then
                    progressBar.Visible = False
                End If
            End Try
        End Function

        Private Sub DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs)
            ' Update the progress bar
            progressBar.Value = e.ProgressPercentage
        End Sub

        Private Sub DownloadFileCompleted(sender As Object, e As System.ComponentModel.AsyncCompletedEventArgs)
            If e.Error IsNot Nothing Then
                MessageBox.Show($"Failed to download the Machi Chara: {e.Error.Message}", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else

            End If

            ' Detach event handlers to avoid memory leaks
            RemoveHandler client.DownloadProgressChanged, AddressOf DownloadProgressChanged
            RemoveHandler client.DownloadFileCompleted, AddressOf DownloadFileCompleted
        End Sub
    End Class
End Namespace
