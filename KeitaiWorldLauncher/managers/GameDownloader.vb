Imports System.Net
Imports System.IO
Imports System.IO.Compression
Imports System.Threading
Imports KeitaiWorldLauncher.My.Managers
Imports KeitaiWorldLauncher.My.Models
Imports System.Security.Policy
Imports System.Text
Imports ReaLTaiizor.Controls

Namespace My.Managers
    Public Class GameDownloader
        Dim utilManager As New UtilManager
        Private WithEvents client1 As WebClient
        Private WithEvents client2 As WebClient
        Private progressBar As ProgressBar
        Private downloadFilePath As String
        Private extractFolderPath As String
        Private selectedGameZipPath As String
        Private ReadJam As String
        Private ReadJar As String
        Private Isthisbatchdownload As Boolean
        Private SelectedGame As Game
        Private SDCardDownloadFile As String
        Dim overlay As New Panel With {
            .Dock = DockStyle.Fill,
            .BackColor = Color.FromArgb(160, Color.Black), ' Semi-transparent black
            .Visible = False
        }
        Private expectClient2Download As Boolean = False
        Private client1Completed As Boolean = False
        Private client2Completed As Boolean = False

        Public Sub New(progressBarControl As ProgressBar)
            client1 = New WebClient()
            client2 = New WebClient()
            progressBar = progressBarControl
        End Sub

        Public Async Function DownloadGameAsync(
            url As String,
            savePath As String,
            extractTo As String,
            Inputgame As Game,
            JAMLocation As String,
            JARLocation As String,
            BatchDownload As Boolean
        ) As Task
            Try
                logger.Logger.LogInfo($"[Download] Starting download for: {Inputgame.ENTitle} from {url}")

                ' Set Global Vars
                downloadFilePath = savePath
                extractFolderPath = extractTo
                ReadJam = JAMLocation
                ReadJar = JARLocation
                Isthisbatchdownload = BatchDownload
                SelectedGame = Inputgame

                ' Create overlay panel
                overlay = New Panel With {
                    .Dock = DockStyle.Fill,
                    .BackColor = Color.FromArgb(160, Color.White),
                    .Visible = True
                }
                Form1.Controls.Add(overlay)
                overlay.BringToFront()
                Dim loadingLabel As New Label With {
                    .Text = "Downloading game...",
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


                ' Start Main Logic - Download main ZIP
                Await client1.DownloadFileTaskAsync(New Uri(url), savePath)
                logger.Logger.LogInfo($"[Download] Main file downloaded to: {savePath}")

                ' Extract game ZIP and Get Icons
                Try
                    logger.Logger.LogInfo($"[Download] Extracting ZIP to: {extractTo}")
                    ZipFile.ExtractToDirectory(savePath, extractTo, True)
                    logger.Logger.LogInfo($"[Download] Extraction complete. Deleting ZIP: {savePath}")
                    File.Delete(savePath)
                    If Not BatchDownload Then
                        logger.Logger.LogInfo($"[Download] Generating dynamic controls from: {JAMLocation}")
                        UtilManager.GenerateDynamicControlsFromLines(JAMLocation, Form1.panelDynamic)
                    End If
                    logger.Logger.LogInfo("[Download] Refreshing game highlighting.")
                    Form1.RefreshGameHighlighting()
                    logger.Logger.LogInfo("[Download] Extracting and resizing app icon.")
                    utilManager.ExtractAndResizeAppIcon(JARLocation, JAMLocation, Inputgame)

                Catch ex As Exception
                    logger.Logger.LogError($"[Download] Failed to extract or process game files: {ex}")
                    MessageBox.Show($"Failed to extract the game: {ex.Message}", "Extraction Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try

                ' Handle optional SD Card data
                If Not String.IsNullOrWhiteSpace(Inputgame.SDCardDataURL) Then
                    SDCardDownloadFile = $"data\downloads\{Path.GetFileName(Inputgame.SDCardDataURL)}"
                    logger.Logger.LogInfo($"[Download] Downloading SD Card Data from: {Inputgame.SDCardDataURL}")

                    Await client2.DownloadFileTaskAsync(New Uri(Inputgame.SDCardDataURL), SDCardDownloadFile)

                    Try
                        Dim destinationPath As String = ""
                        Dim SDCardDataFolderName = $"SVC0000{Path.GetFileName(ReadJam)}"

                        Select Case Inputgame.Emulator.ToLower()
                            Case "doja"
                                destinationPath = $"{Form1.Dojapath}\lib\storagedevice\ext0\SD_BIND\{SDCardDataFolderName}"
                            Case "star"
                                destinationPath = $"{Form1.Starpath}\lib\storagedevice\ext0\SD_BIND\{SDCardDataFolderName}"
                            Case Else
                                logger.Logger.LogWarning("[Download] Unknown emulator type when handling SD Card data.")
                        End Select

                        If Not String.IsNullOrEmpty(destinationPath) Then
                            ZipFile.ExtractToDirectory(SDCardDownloadFile, destinationPath, Encoding.GetEncoding(932), True)
                            logger.Logger.LogInfo($"[Download] SD Card data extracted to: {destinationPath}")
                            File.Delete(SDCardDownloadFile)
                            logger.Logger.LogInfo($"[Download] SD Card zip file deleted: {SDCardDownloadFile}")
                        End If
                    Catch ex As Exception
                        logger.Logger.LogError($"[Download] Failed to handle SD Card data:{vbCrLf}{ex}")
                        MessageBox.Show($"Failed to handle SD Card data:{vbCrLf}{ex}", "SD Card Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try
                Else
                    logger.Logger.LogInfo("[Download] No SD Card Data to download.")
                End If

            Catch ex As Exception
                logger.Logger.LogError($"[Download] Exception occurred during download:{vbCrLf}{ex}")
                MessageBox.Show($"Failed to start download: {ex.Message}", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                If overlay IsNot Nothing Then
                    Form1.Controls.Remove(overlay)
                    overlay.Dispose()
                End If

                If progressBar IsNot Nothing Then
                    overlay.Controls.Remove(progressBar)
                    progressBar.Visible = False
                End If
            End Try
        End Function
        Private Sub DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs)
            ' Update the progress bar
            progressBar.Value = e.ProgressPercentage
        End Sub
        Private Sub DownloadFileCompleted(sender As Object, e As System.ComponentModel.AsyncCompletedEventArgs)
            If sender Is client1 Then
                client1Completed = True
            ElseIf sender Is client2 Then
                client2Completed = True
            End If

            ' If client2 is expected, wait for both. Otherwise, just wait for client1.
            If Not client1Completed OrElse (expectClient2Download AndAlso Not client2Completed) Then
                Exit Sub
            End If

            ' Hide overlay
            progressBar.Visible = False
            Form1.Controls.Remove(overlay)

            If e.Error IsNot Nothing Then
                MessageBox.Show($"Failed to download the game: {e.Error.Message}", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                Try
                    ZipFile.ExtractToDirectory(downloadFilePath, extractFolderPath, True)
                    File.Delete(downloadFilePath)

                    If Isthisbatchdownload = False Then
                        UtilManager.GenerateDynamicControlsFromLines(ReadJam, Form1.panelDynamic)
                    End If

                    Form1.RefreshGameHighlighting()
                    utilManager.ExtractAndResizeAppIcon(ReadJar, ReadJam, SelectedGame)

                Catch ex As Exception
                    MessageBox.Show($"Failed to extract the game: {ex.Message}", "Extraction Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If

            ' Detach event handlers
            RemoveHandler client1.DownloadProgressChanged, AddressOf DownloadProgressChanged
            RemoveHandler client1.DownloadFileCompleted, AddressOf DownloadFileCompleted
            RemoveHandler client2.DownloadProgressChanged, AddressOf DownloadProgressChanged
            RemoveHandler client2.DownloadFileCompleted, AddressOf DownloadFileCompleted
        End Sub
    End Class
End Namespace
