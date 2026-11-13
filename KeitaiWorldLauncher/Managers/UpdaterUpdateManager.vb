Imports System.Diagnostics
Imports System.IO
Imports System.Net.Http
Imports System.Reflection.PortableExecutable
Imports System.Threading

Namespace My.Managers
    Public NotInheritable Class UpdaterUpdateManager
        Private Const UpdaterFileName As String = "KWL-Updater.exe"
        Private Const MaximumDownloadBytes As Long = 64L * 1024 * 1024
        Private Shared ReadOnly UpdateLock As New SemaphoreSlim(1, 1)

        Private Sub New()
        End Sub

        ' Return True only when a new updater was installed.
        Public Shared Async Function CheckAndUpdateAsync(versionCheckUrl As String,
                                                         Optional installationDirectory As String = Nothing) As Task(Of Boolean)
            If Not Await UpdateLock.WaitAsync(0) Then Return False

            Dim stagedPath As String = Nothing
            Try
                Dim manifestUri = GetHttpUri(versionCheckUrl)
                Dim baseDirectory = Path.GetFullPath(If(installationDirectory, AppContext.BaseDirectory))
                Dim updaterPath = Path.Combine(baseDirectory, UpdaterFileName)

                Using timeout As New CancellationTokenSource(TimeSpan.FromSeconds(30))
                    logger.Logger.LogInfo("[Updater] Checking version file: " & manifestUri.AbsoluteUri)
                    Dim manifest = Await Http.GetStringAsync(manifestUri, timeout.Token)
                    Dim lines = manifest.Trim().Split({vbCrLf, vbLf}, StringSplitOptions.None)
                    Dim latestVersion As Version = Nothing
                    If lines.Length <> 2 OrElse Not Version.TryParse(lines(0).Trim(), latestVersion) Then
                        Throw New InvalidDataException("The updater version file must contain a version on line 1 and a download URL on line 2.")
                    End If

                    latestVersion = NormalizeVersion(latestVersion)
                    Dim downloadUri = GetHttpUri(lines(1).Trim())
                    Dim installedVersion = ReadInstalledVersion(updaterPath)
                    If installedVersion IsNot Nothing AndAlso installedVersion >= latestVersion Then
                        logger.Logger.LogInfo($"[Updater] Installed version {installedVersion} is current.")
                        Return False
                    End If
                    stagedPath = Path.Combine(baseDirectory, $"KWL-Updater.{Guid.NewGuid():N}.download.exe")
                    logger.Logger.LogInfo($"[Updater] Downloading version {latestVersion}.")
                    Await DownloadUpdaterAsync(downloadUri, stagedPath, timeout.Token)
                    ValidateDownload(stagedPath, latestVersion)

                    For attempt = 1 To 10
                        timeout.Token.ThrowIfCancellationRequested()
                        Try
                            Dim currentVersion = ReadInstalledVersion(updaterPath)
                            If currentVersion IsNot Nothing AndAlso currentVersion >= latestVersion Then Return False

                            If File.Exists(updaterPath) Then
                                File.Replace(stagedPath, updaterPath, updaterPath & ".bak")
                            Else
                                File.Move(stagedPath, updaterPath)
                            End If

                            stagedPath = Nothing
                            logger.Logger.LogInfo($"[Updater] Installed version {latestVersion} successfully.")
                            Return True
                        Catch ex As IOException When attempt < 10
                            ' Await must be outside Catch in VB.
                        End Try
                        Await Task.Delay(200, timeout.Token)
                    Next
                End Using
            Catch ex As Exception
                logger.Logger.LogError("[Updater] Automatic updater refresh failed; continuing KWL startup. " & ex.ToString())
            Finally
                Try
                    If stagedPath IsNot Nothing AndAlso File.Exists(stagedPath) Then File.Delete(stagedPath)
                Catch ex As Exception
                    logger.Logger.LogWarning("[Updater] Could not remove staged download: " & ex.ToString())
                End Try
                UpdateLock.Release()
            End Try

            Return False
        End Function

        Private Shared Function GetHttpUri(value As String) As Uri
            Dim result As Uri = Nothing
            If Not Uri.TryCreate(value, UriKind.Absolute, result) OrElse
               (result.Scheme <> Uri.UriSchemeHttp AndAlso result.Scheme <> Uri.UriSchemeHttps) Then
                Throw New InvalidDataException("Updater URLs must be absolute HTTP or HTTPS URLs.")
            End If
            Return result
        End Function

        Private Shared Function NormalizeVersion(value As Version) As Version
            Return New Version(value.Major, value.Minor, Math.Max(0, value.Build), Math.Max(0, value.Revision))
        End Function

        Private Shared Function ReadInstalledVersion(updaterPath As String) As Version
            If Not File.Exists(updaterPath) Then Return Nothing

            Try
                Dim version = FileVersionInfo.GetVersionInfo(updaterPath)
                If Not IsUpdater(version) Then Return Nothing
                Return New Version(version.FileMajorPart, version.FileMinorPart, version.FileBuildPart, version.FilePrivatePart)
            Catch ex As Exception When TypeOf ex Is IOException OrElse TypeOf ex Is ArgumentException
                Return Nothing
            End Try
        End Function

        Private Shared Function IsUpdater(version As FileVersionInfo) As Boolean
            Return String.Equals(version.OriginalFilename, "KWL-Updater.dll", StringComparison.OrdinalIgnoreCase) OrElse
                   String.Equals(version.OriginalFilename, UpdaterFileName, StringComparison.OrdinalIgnoreCase)
        End Function

        Private Shared Sub ValidateDownload(filePath As String, expectedVersion As Version)
            Using stream = File.OpenRead(filePath)
                Using executable As New PEReader(stream)
                    If executable.PEHeaders.PEHeader Is Nothing OrElse
                       executable.PEHeaders.CoffHeader.Machine <> Machine.Amd64 OrElse
                       (executable.PEHeaders.CoffHeader.Characteristics And Characteristics.Dll) <> 0 Then
                        Throw New InvalidDataException("The updater download is not a 64-bit Windows executable.")
                    End If
                End Using
            End Using

            Dim downloadedVersion = FileVersionInfo.GetVersionInfo(filePath)
            If Not IsUpdater(downloadedVersion) OrElse
               New Version(downloadedVersion.FileMajorPart, downloadedVersion.FileMinorPart,
                           downloadedVersion.FileBuildPart, downloadedVersion.FilePrivatePart) <> expectedVersion Then
                Throw New InvalidDataException("The downloaded updater identity or version does not match the version file.")
            End If
        End Sub

        Private Shared Async Function DownloadUpdaterAsync(downloadUri As Uri, filePath As String,
                                                           cancellationToken As CancellationToken) As Task
            Using response = Await Http.GetAsync(downloadUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                response.EnsureSuccessStatusCode()
                Dim expectedLength = response.Content.Headers.ContentLength
                If expectedLength.HasValue AndAlso (expectedLength.Value <= 0 OrElse expectedLength.Value > MaximumDownloadBytes) Then
                    Throw New InvalidDataException("The updater download size is invalid.")
                End If

                Using source = Await response.Content.ReadAsStreamAsync(cancellationToken)
                    Using destination As New FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None,
                                                        81920, FileOptions.Asynchronous)
                        Dim buffer(81919) As Byte
                        Dim totalBytes As Long = 0
                        While True
                            Dim bytesRead = Await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken)
                            If bytesRead = 0 Then Exit While
                            totalBytes += bytesRead
                            If totalBytes > MaximumDownloadBytes Then Throw New InvalidDataException("The updater download is too large.")
                            Await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken)
                        End While

                        If totalBytes = 0 OrElse (expectedLength.HasValue AndAlso totalBytes <> expectedLength.Value) Then
                            Throw New InvalidDataException("The updater download is incomplete.")
                        End If
                    End Using
                End Using
            End Using
        End Function
    End Class
End Namespace
