Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading
Imports System.Windows.Forms

Module Program
    <STAThread>
    Function Main(args As String()) As Integer
        Dim log As New UpdaterLogger()
        Dim warnings As New List(Of String)()
        Dim stage As String = Nothing
        Dim failure = RunUpdate(args, log, warnings, stage)

        ' Show results after cleanup so the diagnostic log includes the entire attempt.
        If failure IsNot Nothing Then
            ShowResult("The update failed while " & stage & "." & Environment.NewLine & failure.Message,
                       warnings, log, "KWL Update Failed", MessageBoxIcon.Error)
            Return 1
        End If

        If warnings.Count > 0 Then
            ShowResult("The update completed with warnings.", warnings, log,
                       "KWL Update Warning", MessageBoxIcon.Warning)
        End If

        Return 0
    End Function

    Private Function RunUpdate(args As String(), log As UpdaterLogger, warnings As List(Of String),
                               ByRef stage As String) As Exception
        Dim tempDirectory As String = Nothing
        Dim failure As Exception = Nothing
        stage = "initializing the updater"

        Try
            log.Write("INFO", $"KWL Updater started. Runtime={RuntimeInformation.FrameworkDescription}; Architecture={RuntimeInformation.ProcessArchitecture}")
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)

            If args.Length < 3 Then
                Throw New ArgumentException("Launch the updater from KWL, or use: KWL-Updater.exe <ZipURL> <TargetFolder> <AppToRestart>")
            End If

            Dim zipUrl = args(0)
            Dim targetDir = Path.GetFullPath(args(1))
            Dim appExe = Path.GetFullPath(args(2))
            log.Write("INFO", $"Target folder: {targetDir}{Environment.NewLine}Application to restart: {appExe}")

            tempDirectory = Path.Combine(Path.GetTempPath(), "kwl_update_" & Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(tempDirectory)
            Dim tempZip = Path.Combine(tempDirectory, "update.zip")
            Dim tempExtract = Path.Combine(tempDirectory, "extracted")

            stage = "downloading the update package"
            log.Write("INFO", "Downloading update package...")
            Using client As New Net.WebClient()
                client.DownloadFile(zipUrl, tempZip)
            End Using

            stage = "extracting the update package"
            log.Write("INFO", "Extracting update package...")
            ZipFile.ExtractToDirectory(tempZip, tempExtract)

            stage = "applying the update files"
            log.Write("INFO", "Applying update...")
            Dim currentExePath = Environment.ProcessPath

            For Each f In Directory.GetFiles(tempExtract, "*", SearchOption.AllDirectories)
                Dim relative = Path.GetRelativePath(tempExtract, f)
                Dim destPath = Path.Combine(targetDir, relative)

                ' Skip overwriting the currently running updater EXE
                If String.Equals(destPath, currentExePath, StringComparison.OrdinalIgnoreCase) Then
                    Continue For
                End If

                Directory.CreateDirectory(Path.GetDirectoryName(destPath))
                File.Copy(f, destPath, True)
            Next

            log.Write("INFO", "Update files applied successfully.")
            Thread.Sleep(1000)
            Try
                Dim configPath As String = Path.Combine(targetDir, "configs", "appconfig.xml")

                If File.Exists(configPath) Then
                    Dim sjis = Encoding.GetEncoding("shift-jis")
                    Dim xmlText As String = File.ReadAllText(configPath, sjis)
                    Dim doc As New Xml.XmlDocument()
                    doc.LoadXml(xmlText)

                    Dim node = doc.SelectSingleNode("//Setting[@name='FirstRun']")
                    If node IsNot Nothing AndAlso node.InnerText.Trim().ToLowerInvariant() = "true" Then
                        node.InnerText = "false"

                        ' Save in SJIS with flush
                        Using fs As New FileStream(configPath, FileMode.Create, FileAccess.Write, FileShare.None)
                            Using writer As New StreamWriter(fs, sjis, 1024, leaveOpen:=True)
                                doc.Save(writer)
                                writer.Flush()
                                fs.Flush(True)
                            End Using
                        End Using
                        log.Write("INFO", "FirstRun setting updated successfully.")
                    Else
                        log.Write("INFO", "FirstRun setting not found or already false.")
                    End If
                Else
                    log.Write("INFO", "appconfig.xml not found at: " & configPath)
                End If
            Catch ex As Exception
                log.Write("WARNING", "Failed to update FirstRun setting.", ex)
                warnings.Add("Failed to update FirstRun setting: " & ex.Message)
            End Try

            Thread.Sleep(2000)
            stage = "restarting KWL"
            log.Write("INFO", "Launching KWL...")
            Process.Start(appExe)
            log.Write("INFO", "KWL launched successfully.")
        Catch ex As Exception
            failure = ex
            log.Write("ERROR", "Update failed while " & stage & ".", ex)
        Finally
            Try
                If tempDirectory IsNot Nothing AndAlso Directory.Exists(tempDirectory) Then
                    Directory.Delete(tempDirectory, True)
                    log.Write("INFO", "Temporary update files cleaned up.")
                End If
            Catch cleanupEx As Exception
                log.Write("WARNING", "Failed to clean up temporary update files.", cleanupEx)
                warnings.Add("Failed to clean up temporary update files: " & cleanupEx.Message)
            End Try
        End Try

        Return failure
    End Function

    Private Sub ShowResult(message As String, warnings As List(Of String), log As UpdaterLogger,
                           title As String, icon As MessageBoxIcon)
        If warnings.Count > 0 Then
            message &= Environment.NewLine & Environment.NewLine & String.Join(Environment.NewLine, warnings)
        End If

        MessageBox.Show(message & Environment.NewLine & Environment.NewLine & log.LocationMessage,
                        title, MessageBoxButtons.OK, icon)
    End Sub
End Module
