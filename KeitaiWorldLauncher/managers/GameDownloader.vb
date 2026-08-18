Imports System.IO
Imports System.IO.Compression
Imports System.Net.Http
Imports System.Text
Imports KeitaiWorldLauncher.My.Models

Namespace My.Managers
    Public Class GameDownloader
        Private utilManager As New UtilManager
        Private ReadOnly _progress As IProgress(Of DownloadProgressInfo)

        Public Sub New(progress As IProgress(Of DownloadProgressInfo))
            _progress = progress
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
            Dim stagingFolder = extractTo & ".installing"
            Try
                logger.Logger.LogInfo($"[Download] Starting download for: {game.ENTitle} from {url}")

                ' Download main ZIP
                Await DownloadFileAsync(url, savePath, $"Downloading {game.ENTitle}...")
                logger.Logger.LogInfo($"[Download] Main file downloaded to: {savePath}")

                ' Extract and clean up
                Try
                    ReportProgress(DownloadOperationPhase.Extracting, "Extracting app files...", -1)
                    If Directory.Exists(stagingFolder) Then Directory.Delete(stagingFolder, True)
                    logger.Logger.LogInfo($"[Download] Extracting ZIP to staging folder: {stagingFolder}")
                    Await Task.Run(Sub() ZipFile.ExtractToDirectory(savePath, stagingFolder, True))

                    Dim stagedJamPath = GetStagedPath(extractTo, stagingFolder, jamLocation)
                    Dim stagedJarPath = GetStagedPath(extractTo, stagingFolder, jarLocation)
                    If Not File.Exists(stagedJamPath) OrElse Not File.Exists(stagedJarPath) Then
                        Throw New InvalidDataException("The package does not contain the expected launch files.")
                    End If

                    logger.Logger.LogInfo($"[Download] Extraction complete. Deleting ZIP: {savePath}")
                    File.Delete(savePath)
                Catch ex As Exception
                    logger.Logger.LogError($"[Download] Failed to extract or process game files: {ex}")
                    Throw New InvalidDataException("The downloaded package could not be extracted.", ex)
                End Try

                ' Handle optional SD Card data
                Await HandleSDCardDataAsync(game, jamLocation)

                ReportProgress(DownloadOperationPhase.Installing, "Installing app files...", -1)
                Await Task.Run(Sub() CommitStagedInstall(stagingFolder, extractTo))

                ' Finish up without touching the current Library selection. The
                ' queue owns the UI refresh because the user may be viewing a
                ' different app by the time this installation completes.
                ReportProgress(DownloadOperationPhase.Installing, "Finishing installation...", -1)
                logger.Logger.LogInfo("[Download] Extracting and resizing app icon.")
                Await utilManager.ExtractAndResizeAppIconAsync(jarLocation, jamLocation, game)

            Catch ex As Exception
                logger.Logger.LogError($"[Download] Exception occurred during download:{vbCrLf}{ex}")
                Throw
            Finally
                Try
                    If File.Exists(savePath) Then File.Delete(savePath)
                    If File.Exists(savePath & ".part") Then File.Delete(savePath & ".part")
                    If Directory.Exists(stagingFolder) Then Directory.Delete(stagingFolder, True)
                Catch cleanupException As Exception
                    logger.Logger.LogWarning($"[Download] Could not remove temporary package: {cleanupException.Message}")
                End Try
            End Try
        End Function

        Private Async Function HandleSDCardDataAsync(game As Game, jamLocation As String) As Task
            If String.IsNullOrWhiteSpace(game.SDCardDataURL) Then
                logger.Logger.LogInfo("[Download] No SD Card Data to download.")
                Return
            End If

            Dim sdDownloadPath = $"data\downloads\{Path.GetFileName(game.SDCardDataURL)}"
            logger.Logger.LogInfo($"[Download] Downloading SD Card Data from: {game.SDCardDataURL}")
            Await DownloadFileAsync(game.SDCardDataURL, sdDownloadPath, "Downloading optional SD card data...")

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
                Throw New InvalidDataException("The optional SD card data could not be installed.", ex)
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

        Private Async Function DownloadFileAsync(url As String, savePath As String, statusText As String) As Task
            Dim partialPath = savePath & ".part"
            Dim parentFolder = Path.GetDirectoryName(savePath)
            If Not String.IsNullOrWhiteSpace(parentFolder) Then Directory.CreateDirectory(parentFolder)
            If File.Exists(partialPath) Then File.Delete(partialPath)

            Try
                ReportProgress(DownloadOperationPhase.Downloading, statusText, 0)
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
                                ReportProgress(DownloadOperationPhase.Downloading, statusText, percentage)
                            End While
                        End Using
                    End Using
                End Using

                File.Move(partialPath, savePath, True)
            Finally
                If File.Exists(partialPath) Then File.Delete(partialPath)
            End Try
        End Function

        Private Sub ReportProgress(phase As DownloadOperationPhase, statusText As String, percentage As Integer)
            _progress?.Report(New DownloadProgressInfo With {
                .Phase = phase,
                .StatusText = statusText,
                .Percentage = percentage
            })
        End Sub

        Private Shared Function GetStagedPath(finalFolder As String, stagingFolder As String, finalPath As String) As String
            Dim relativePath = Path.GetRelativePath(finalFolder, finalPath)
            If relativePath.StartsWith("..", StringComparison.Ordinal) OrElse Path.IsPathRooted(relativePath) Then
                Throw New InvalidDataException("An expected launch file resolves outside the app folder.")
            End If
            Return Path.Combine(stagingFolder, relativePath)
        End Function

        Private Shared Sub CommitStagedInstall(stagingFolder As String, finalFolder As String)
            Dim previousFolder = finalFolder & ".previous"
            If Directory.Exists(previousFolder) Then
                If Directory.Exists(finalFolder) Then
                    Directory.Delete(previousFolder, True)
                Else
                    Directory.Move(previousFolder, finalFolder)
                End If
            End If

            If Directory.Exists(finalFolder) Then
                PreserveSaveData(finalFolder, stagingFolder)
                Directory.Move(finalFolder, previousFolder)
            End If

            Try
                Directory.Move(stagingFolder, finalFolder)
            Catch
                If Not Directory.Exists(finalFolder) AndAlso Directory.Exists(previousFolder) Then
                    Directory.Move(previousFolder, finalFolder)
                End If
                Throw
            End Try

            If Directory.Exists(previousFolder) Then
                Try
                    Directory.Delete(previousFolder, True)
                Catch ex As Exception
                    logger.Logger.LogWarning($"[Download] The previous app folder could not be removed: {ex.Message}")
                End Try
            End If
        End Sub

        Private Shared Sub PreserveSaveData(sourceFolder As String, destinationFolder As String)
            For Each extension In New String() {"*.sp", "*.rms"}
                For Each sourcePath In Directory.GetFiles(sourceFolder, extension, SearchOption.AllDirectories)
                    Dim relativePath = Path.GetRelativePath(sourceFolder, sourcePath)
                    Dim destinationPath = Path.Combine(destinationFolder, relativePath)
                    Dim destinationDirectory = Path.GetDirectoryName(destinationPath)
                    If Not String.IsNullOrWhiteSpace(destinationDirectory) Then Directory.CreateDirectory(destinationDirectory)
                    File.Copy(sourcePath, destinationPath, True)
                Next
            Next
        End Sub
    End Class
End Namespace
