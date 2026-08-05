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

                Select Case game.Emulator.ToLowerInvariant()
                    Case "doja"
                        destinationPath = $"{MainForm.DOJApath}\lib\storagedevice\ext0\SD_BIND\{sdFolder}"
                    Case "star"
                        destinationPath = $"{MainForm.STARpath}\lib\storagedevice\ext0\SD_BIND\{sdFolder}"
                    Case "jsky", "vodafone", "softbank"
                        destinationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ReMEXA", "storage", "mc")
                    Case Else
                        logger.Logger.LogWarning($"[Download] Unknown emulator {game.Emulator} type when handling SD Card data.")
                        Return
                End Select

                Dim entryNameEncoding = DetectZipEntryNameEncoding(sdDownloadPath)
                logger.Logger.LogInfo($"[Download] Extracting SD Card data using {entryNameEncoding.WebName} filename encoding.")
                ZipFile.ExtractToDirectory(sdDownloadPath, destinationPath, entryNameEncoding, True)
                logger.Logger.LogInfo($"[Download] SD Card data extracted to: {destinationPath}")
                File.Delete(sdDownloadPath)
                logger.Logger.LogInfo($"[Download] SD Card zip file deleted: {sdDownloadPath}")
            Catch ex As Exception
                logger.Logger.LogError($"[Download] Failed to handle SD Card data:{vbCrLf}{ex}")
                MessageBox.Show($"Failed to handle SD Card data:{vbCrLf}{ex}", "SD Card Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Function

        Private Shared Function DetectZipEntryNameEncoding(zipPath As String) As Encoding
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)

            Const centralDirectoryHeaderSignature As UInteger = &H2014B50UI
            Const centralDirectoryHeaderLength As Integer = 46
            Const utf8EntryNameFlag As UShort = &H800US

            Dim zipBytes = File.ReadAllBytes(zipPath)
            Dim entryCount As Integer = 0
            Dim utf8FlaggedEntryCount As Integer = 0
            Dim nonAsciiEntryCount As Integer = 0
            Dim validUtf8NonAsciiEntryCount As Integer = 0
            Dim index As Integer = 0

            While index <= zipBytes.Length - centralDirectoryHeaderLength
                If BitConverter.ToUInt32(zipBytes, index) <> centralDirectoryHeaderSignature Then
                    index += 1
                    Continue While
                End If

                entryCount += 1

                Dim flags = BitConverter.ToUInt16(zipBytes, index + 8)
                If (flags And utf8EntryNameFlag) <> 0 Then
                    utf8FlaggedEntryCount += 1
                End If

                Dim fileNameLength = BitConverter.ToUInt16(zipBytes, index + 28)
                Dim extraFieldLength = BitConverter.ToUInt16(zipBytes, index + 30)
                Dim fileCommentLength = BitConverter.ToUInt16(zipBytes, index + 32)
                Dim fileNameBytes(fileNameLength - 1) As Byte
                Array.Copy(zipBytes, index + centralDirectoryHeaderLength, fileNameBytes, 0, fileNameLength)

                If Not IsAscii(fileNameBytes) Then
                    nonAsciiEntryCount += 1

                    If IsValidUtf8(fileNameBytes) Then
                        validUtf8NonAsciiEntryCount += 1
                    End If
                End If

                index += centralDirectoryHeaderLength + fileNameLength + extraFieldLength + fileCommentLength
            End While

            If entryCount > 0 AndAlso utf8FlaggedEntryCount > 0 Then
                Return Encoding.UTF8
            End If

            If nonAsciiEntryCount > 0 AndAlso validUtf8NonAsciiEntryCount = nonAsciiEntryCount Then
                Return Encoding.UTF8
            End If

            Return Encoding.GetEncoding(932)
        End Function

        Private Shared Function IsAscii(bytes As Byte()) As Boolean
            For Each value In bytes
                If value >= &H80 Then
                    Return False
                End If
            Next

            Return True
        End Function

        Private Shared Function IsValidUtf8(bytes As Byte()) As Boolean
            Try
                Dim strictUtf8Encoding As New UTF8Encoding(False, True)
                strictUtf8Encoding.GetString(bytes)
                Return True
            Catch ex As DecoderFallbackException
                Return False
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
