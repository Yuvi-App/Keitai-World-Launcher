Imports System.IO
Imports System.IO.Compression
Imports System.Net.Http
Imports System.Text
Imports KeitaiWorldLauncher.My.Models

Namespace My.Managers
    Public Class GameDownloader
        Private utilManager As New UtilManager
        Private progressBar As ProgressBar
        Private overlay As Panel = Nothing

        Public Sub New(progressBarControl As ProgressBar)
            progressBar = progressBarControl
        End Sub

        Public Async Function DownloadGameAsync(
            url As String,
            savePath As String,
            extractTo As String,
            game As Game,
            jamLocation As String,
            jarLocation As String,
            batchDownload As Boolean
        ) As Task
            Try
                logger.Logger.LogInfo($"[Download] Starting download for: {game.ENTitle} from {url}")

                ' Create overlay
                ShowOverlay()

                ' Download main ZIP
                Await DownloadFileAsync(url, savePath)
                logger.Logger.LogInfo($"[Download] Main file downloaded to: {savePath}")

                ' Extract and clean up
                Try
                    logger.Logger.LogInfo($"[Download] Extracting ZIP to: {extractTo}")
                    ZipFile.ExtractToDirectory(savePath, extractTo, True)
                    logger.Logger.LogInfo($"[Download] Extraction complete. Deleting ZIP: {savePath}")
                    File.Delete(savePath)
                Catch ex As Exception
                    logger.Logger.LogError($"[Download] Failed to extract or process game files: {ex}")
                    MessageBox.Show($"Failed to extract the game: {ex.Message}", "Extraction Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try

                ' Handle optional SD Card data
                Await HandleSDCardDataAsync(game, jamLocation)

                ' Finish up
                If Not batchDownload Then
                    logger.Logger.LogInfo($"[Download] Generating dynamic controls from: {jamLocation}")
                    UtilManager.GenerateDynamicControlsFromLines(jamLocation, MainForm.panelDynamic, game.ENTitle)
                End If
                logger.Logger.LogInfo("[Download] Refreshing game highlighting.")
                MainForm.RefreshGameHighlighting()
                logger.Logger.LogInfo("[Download] Extracting and resizing app icon.")
                Await utilManager.ExtractAndResizeAppIconAsync(jarLocation, jamLocation, game)

            Catch ex As Exception
                logger.Logger.LogError($"[Download] Exception occurred during download:{vbCrLf}{ex}")
                MessageBox.Show($"Failed to start download: {ex.Message}", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                HideOverlay()
            End Try
        End Function

        Private Async Function HandleSDCardDataAsync(game As Game, jamLocation As String) As Task
            If String.IsNullOrWhiteSpace(game.SDCardDataURL) Then
                logger.Logger.LogInfo("[Download] No SD Card Data to download.")
                Return
            End If

            Dim sdDownloadPath = $"data\downloads\{Path.GetFileName(game.SDCardDataURL)}"
            logger.Logger.LogInfo($"[Download] Downloading SD Card Data from: {game.SDCardDataURL}")
            Await DownloadFileAsync(game.SDCardDataURL, sdDownloadPath)

            Try
                Dim sdFolder = $"SVC0000{Path.GetFileName(jamLocation)}"
                Dim destinationPath As String = ""

                Select Case game.Emulator.ToLower()
                    Case "doja"
                        destinationPath = $"{MainForm.DOJApath}\lib\storagedevice\ext0\SD_BIND\{sdFolder}"
                    Case "star"
                        destinationPath = $"{MainForm.STARpath}\lib\storagedevice\ext0\SD_BIND\{sdFolder}"
                    Case "jsky"
                        logger.Logger.LogWarning("[Download] JSKY SD Card URL Detected but we are not handling this yet.")
                        Return
                    Case Else
                        logger.Logger.LogWarning($"[Download] Unknown emulator {game.Emulator} type when handling SD Card data.")
                        Return
                End Select

                ZipFile.ExtractToDirectory(sdDownloadPath, destinationPath, Encoding.GetEncoding(932), True)
                logger.Logger.LogInfo($"[Download] SD Card data extracted to: {destinationPath}")
                File.Delete(sdDownloadPath)
                logger.Logger.LogInfo($"[Download] SD Card zip file deleted: {sdDownloadPath}")
            Catch ex As Exception
                logger.Logger.LogError($"[Download] Failed to handle SD Card data:{vbCrLf}{ex}")
                MessageBox.Show($"Failed to handle SD Card data:{vbCrLf}{ex}", "SD Card Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Function

        Private Async Function DownloadFileAsync(url As String, savePath As String) As Task
            Using response = Await Http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead)
                response.EnsureSuccessStatusCode()
                Using contentStream = Await response.Content.ReadAsStreamAsync()
                    Using fileStream As New FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, True)
                        Await contentStream.CopyToAsync(fileStream)
                    End Using
                End Using
            End Using
        End Function

        Private Sub ShowOverlay()
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
        End Sub

        Private Sub HideOverlay()
            If overlay IsNot Nothing Then
                If progressBar IsNot Nothing Then
                    overlay.Controls.Remove(progressBar)
                    progressBar.Visible = False
                End If
                MainForm.Controls.Remove(overlay)
                overlay.Dispose()
                overlay = Nothing
            End If
        End Sub
    End Class
End Namespace
