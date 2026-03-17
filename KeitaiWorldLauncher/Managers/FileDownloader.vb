Imports System.Net

Namespace My.Managers
    Public Class FileDownloader
        Private WithEvents client As WebClient
        Private progressBar As ProgressBar
        Private downloadFilePath As String
        Private contentTypeName As String
        Dim overlay As New Panel With {
                    .Dock = DockStyle.Fill,
                    .BackColor = Color.FromArgb(160, Color.White),
                    .Visible = True
                }

        Public Sub New(progressBarControl As ProgressBar)
            client = New WebClient()
            progressBar = progressBarControl
        End Sub

        Public Async Function DownloadFileAsync(url As String, savePath As String, contentName As String) As Task
            Try
                contentTypeName = contentName
                downloadFilePath = savePath

                ' Create overlay

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

                progressBar.Value = 0

                Await client.DownloadFileTaskAsync(New Uri(url), savePath)

            Catch ex As Exception
                logger.Logger.LogError($"[Download] Exception occurred during download:{vbCrLf}{ex}")
                MessageBox.Show($"Failed to start download: {ex.Message}", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                If overlay IsNot Nothing Then
                    If progressBar IsNot Nothing Then
                        overlay.Controls.Remove(progressBar)
                        progressBar.Visible = False
                    End If

                    MainForm.Controls.Remove(overlay)
                    overlay.Dispose()
                    overlay = Nothing
                End If
            End Try
        End Function
    End Class
End Namespace