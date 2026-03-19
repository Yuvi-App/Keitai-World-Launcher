Imports System.IO
Imports System.Net.Http

Namespace My.Managers
    Public Class FileDownloader
        Private progressBar As ProgressBar
        Private overlay As Panel

        Public Sub New(progressBarControl As ProgressBar)
            progressBar = progressBarControl
        End Sub

        Public Async Function DownloadFileAsync(url As String, savePath As String, contentName As String) As Task
            Try
                ' Create overlay
                overlay = New Panel With {
                    .Dock = DockStyle.Fill,
                    .BackColor = Color.FromArgb(160, Color.White),
                    .Visible = True
                }
                MainForm.Controls.Add(overlay)
                overlay.BringToFront()

                Dim loadingLabel As New Label With {
                    .Text = $"Downloading {contentName}...",
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

                ' Download using shared HttpClient
                Using response = Await Http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead)
                    response.EnsureSuccessStatusCode()
                    Using contentStream = Await response.Content.ReadAsStreamAsync()
                        Using fileStream As New FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None)
                            Await contentStream.CopyToAsync(fileStream)
                        End Using
                    End Using
                End Using

            Catch ex As Exception
                logger.Logger.LogError($"[Download] Exception occurred during download:{vbCrLf}{ex}")
                MessageBox.Show($"Failed to download {contentName}: {ex.Message}", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
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