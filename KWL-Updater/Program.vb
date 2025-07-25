Imports System.IO
Imports System.IO.Compression
Imports System.Text
Imports System.Threading

Module Program
    Sub Main(args As String())
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)

        If args.Length < 3 Then
            Console.WriteLine("Usage: KWL-Updater.exe <ZipURL> <TargetFolder> <AppToRestart>")
            Threading.Thread.Sleep(3000)
            Return
        End If

        Dim zipUrl = args(0)
        Dim targetDir = args(1)
        Dim appExe = args(2)

        Dim tempZip = Path.Combine(Path.GetTempPath(), "kwl_update.zip")
        Dim tempExtract = Path.Combine(Path.GetTempPath(), "kwl_update_extract")

        Try
            Console.WriteLine("Downloading update package...")
            Using client As New Net.WebClient()
                client.DownloadFile(zipUrl, tempZip)
            End Using

            Console.WriteLine("Extracting update package...")
            If Directory.Exists(tempExtract) Then Directory.Delete(tempExtract, True)
            ZipFile.ExtractToDirectory(tempZip, tempExtract)

            Console.WriteLine("Applying update...")
            Dim currentExePath = Process.GetCurrentProcess().MainModule.FileName

            For Each f In Directory.GetFiles(tempExtract, "*", SearchOption.AllDirectories)
                Dim relative = f.Substring(tempExtract.Length + 1)
                Dim destPath = Path.Combine(targetDir, relative)

                ' Skip overwriting the currently running updater EXE
                If String.Equals(destPath, currentExePath, StringComparison.OrdinalIgnoreCase) Then
                    Continue For
                End If

                Directory.CreateDirectory(Path.GetDirectoryName(destPath))
                File.Copy(f, destPath, True)
            Next

            Console.WriteLine("Update applied successfully.")
            Thread.Sleep(1000)
            Try
                Dim configPath As String = Path.Combine(targetDir, "configs", "appconfig.xml")

                If File.Exists(configPath) Then
                    Dim sjis = Encoding.GetEncoding("shift-jis")
                    Dim xmlText As String = File.ReadAllText(configPath, sjis)
                    Dim doc As New Xml.XmlDocument()
                    doc.LoadXml(xmlText)

                    Dim node = doc.SelectSingleNode("//Setting[@name='FirstRun']")
                    If node IsNot Nothing AndAlso node.InnerText.Trim().ToLower() = "true" Then
                        node.InnerText = "false"

                        ' Save in SJIS with flush
                        Using fs As New FileStream(configPath, FileMode.Create, FileAccess.Write, FileShare.None)
                            Dim writer As New StreamWriter(fs, sjis)
                            doc.Save(writer)
                            writer.Flush()
                            fs.Flush(True)
                        End Using
                    Else
                        Console.WriteLine("FirstRun setting not found or already false.")
                    End If
                Else
                    Console.WriteLine("appconfig.xml not found at: " & configPath)
                End If
            Catch ex As Exception
                Console.WriteLine("Failed to update FirstRun setting: " & ex.Message)
            End Try

            Threading.Thread.Sleep(2000)
            Console.WriteLine("Launching KWL...")
            Process.Start(appExe)
        Catch ex As Exception
            Console.WriteLine("Update failed: " & ex.Message)
        Finally
            ' Ensure cleanup
            Try
                If File.Exists(tempZip) Then File.Delete(tempZip)
                If Directory.Exists(tempExtract) Then Directory.Delete(tempExtract, True)
            Catch cleanupEx As Exception
                Console.WriteLine("Failed to clean up temp files: " & cleanupEx.Message)
            End Try
        End Try
    End Sub
End Module
