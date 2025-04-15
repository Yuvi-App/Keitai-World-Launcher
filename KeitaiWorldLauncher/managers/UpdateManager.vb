Imports System.IO
Imports System.Net.Http
Imports System.Reflection

Namespace My.Managers
    Public Class UpdateManager
        'Check for APP Updates
        Public Shared Async Function CheckAndLaunchUpdaterAsync(versionCheckUrl As String, ownerWindow As IWin32Window) As Task
            Dim currentVersion As String = Assembly.GetExecutingAssembly().GetName().Version.ToString()
            logger.Logger.LogInfo($"Current app version: {currentVersion}")
            logger.Logger.LogInfo($"Checking version file at: {versionCheckUrl}")

            Try
                Using client As New HttpClient()
                    Dim versionData As String = (Await client.GetStringAsync(versionCheckUrl)).Trim()
                    Dim lines As String() = versionData.Split({vbCrLf, vbLf}, StringSplitOptions.RemoveEmptyEntries)

                    If lines.Length < 2 Then
                        logger.Logger.LogInfo("Update check file does not contain enough lines (version + URL).")
                        MessageBox.Show(ownerWindow, "Update check file is missing required information.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If

                    Dim latestVersion As String = lines(0).Trim()
                    Dim updatePackageUrl As String = lines(1).Trim()

                    logger.Logger.LogInfo($"Latest version found: {latestVersion}")
                    logger.Logger.LogInfo($"Update package URL: {updatePackageUrl}")

                    If currentVersion <> latestVersion Then
                        logger.Logger.LogInfo("Update required — prompting user.")
                        Dim confirm = MessageBox.Show(ownerWindow, $"A new version is available!" & vbCrLf &
                                                  $"Current: {currentVersion}" & vbCrLf &
                                                  $"Latest: {latestVersion}" & vbCrLf &
                                                  "Would you like to update now?",
                                                  "Update Available", MessageBoxButtons.YesNo, MessageBoxIcon.Information)

                        If confirm = DialogResult.Yes Then
                            Dim updaterPath = IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "KWL-Updater.exe")
                            Dim exePath = Process.GetCurrentProcess().MainModule.FileName
                            Dim updateArgs = $"{Quote(updatePackageUrl)} {Quote(AppDomain.CurrentDomain.BaseDirectory)} {Quote(exePath)}"

                            If IO.File.Exists(updaterPath) Then
                                Dim startInfo As New ProcessStartInfo() With {
                                    .FileName = updaterPath,
                                    .Arguments = updateArgs,
                                    .UseShellExecute = True,
                                    .WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                                }
                                Process.Start(startInfo)
                                Environment.Exit(0)
                            Else
                                logger.Logger.LogInfo("KWL-Updater.exe not found.")
                                MessageBox.Show(ownerWindow, "KWL-Updater.exe not found in the application directory.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            End If
                        Else
                            logger.Logger.LogInfo("User chose not to update.")
                        End If
                    Else
                        logger.Logger.LogInfo("Application is up-to-date.")
                    End If
                End Using
            Catch ex As Exception
                logger.Logger.LogInfo($"Exception occurred during update check: {ex.Message}")
                MessageBox.Show(ownerWindow, $"Failed to check for updates. Error: {ex.Message}", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Function
        Shared Function Quote(arg As String) As String
            If arg.Contains(" ") OrElse arg.Contains("""") Then
                Return $"""{arg.Replace("""", "\""")}"""
            End If
            Return arg
        End Function
    End Class
End Namespace

