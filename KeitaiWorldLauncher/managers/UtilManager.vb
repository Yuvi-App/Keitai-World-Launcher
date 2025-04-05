Imports System.Net
Imports System.IO
Imports System.Diagnostics
Imports System.Text
Imports Windows.Win32.UI
Imports System.IO.Compression
Imports System.Xml
Imports System.Xml.Xsl
Imports System.Text.RegularExpressions
Imports System.Runtime.InteropServices
Imports System.Threading
Imports KeitaiWorldLauncher.My.logger
Imports KeitaiWorldLauncher.My.Models
Imports Microsoft.Win32
Imports System.Security.Principal
Imports System.Reflection

Namespace My.Managers
    Public Class UtilManager
        Private Shared LaunchOverlay As Panel = Nothing


        Public Shared Sub SetupDIRS()
            Dim directories As String() = {
                "data",
                "configs",
                "logs",
                Path.Combine("data", "downloads"),
                Path.Combine("data", "tools"),
                Path.Combine("data", "tools", "icons"),
                Path.Combine("data", "tools", "icons", "defaults"),
                Path.Combine("data", "tools", "skins"),
                Path.Combine("data", "tools", "skins", "doja", "ui"),
                Path.Combine("data", "tools", "skins", "doja", "noui"),
                Path.Combine("data", "tools", "skins", "star", "ui"),
                Path.Combine("data", "tools", "skins", "star", "noui")
            }

            For Each D In directories
                Directory.CreateDirectory(D)
            Next
        End Sub

        'PreReq Check
        Public Shared Function CheckforPreReq() As Boolean
            ' Check for administrator privileges before continuing
            If Not IsRunningAsAdmin() Then
                MessageBox.Show(owner:=SplashScreen, "For the first-time setup, this application requires administrator privileges to configure necessary settings." & vbCrLf & vbCrLf &
                        "Please restart the application as an Administrator by right-clicking the executable and selecting 'Run as administrator'.",
                        "Administrator Privileges Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Form1.QuitApplication()
                Return False
            End If

            ' === Begin pre-req checks ===
            Dim DOJAEmulator = Form1.DojaEXE
            Dim StarEmulator = Form1.StarEXE
            Dim localeEmuLoc = "data\tools\locale_emulator\LEProc.exe"
            Dim ShaderGlassLoc = "data\tools\shaderglass\ShaderGlass.exe"

            ' Setup DOJA and STAR Reg
            Dim toolDojaDirs = Directory.GetDirectories(Form1.ToolsFolder, "idkDoja*")
            For Each dir As String In toolDojaDirs
                Dim regFile As String = Path.Combine(dir, "doja.reg")
                Dim dojaExe As String = Path.Combine(dir, "bin", "doja.exe")
                If File.Exists(regFile) Then
                    UtilManager.ImportRegFile(regFile)
                End If
                If File.Exists(dojaExe) AndAlso Not UtilManager.IsDpiScalingSet(dojaExe) Then
                    UtilManager.SetDpiScaling(dojaExe)
                End If
            Next

            Dim toolStarDirs = Directory.GetDirectories(Form1.ToolsFolder, "idkStar*")
            For Each dir As String In toolStarDirs
                Dim regFile As String = Path.Combine(dir, "star.reg")
                Dim starExe As String = Path.Combine(dir, "bin", "star.exe")
                If File.Exists(regFile) Then
                    UtilManager.ImportRegFile(regFile)
                End If
                If File.Exists(starExe) AndAlso Not UtilManager.IsDpiScalingSet(starExe) Then
                    UtilManager.SetDpiScaling(starExe)
                End If
            Next

            ' Check for DOJA Emulator
            My.logger.Logger.LogInfo("Checking for DOJA Emu")
            If Not File.Exists(DOJAEmulator) Then
                MessageBox.Show(owner:=SplashScreen, $"Missing DOJA 5.1 Emulator... Download is required{vbCrLf}Emulator Files need to be located at {DOJAEmulator}")
                My.logger.Logger.LogInfo("Missing DOJA 5.1 Emulator")
                OpenURL("https://archive.org/details/iappli-tool-dev-tools")
                Form1.QuitApplication()
            End If

            ' Check for STAR Emulator
            My.logger.Logger.LogInfo("Checking for STAR Emu")
            If Not File.Exists(StarEmulator) Then
                MessageBox.Show(owner:=SplashScreen, $"Missing STAR 2.0 Emulator... Download is required{vbCrLf}Emulator Files need to be located at {StarEmulator}")
                My.logger.Logger.LogInfo("Missing STAR 2.0 Emulator")
                OpenURL("https://archive.org/details/iappli-tool-dev-tools")
                Form1.QuitApplication()
            End If

            ' Check for Locale Emulator
            My.logger.Logger.LogInfo("Checking for LEPROC")
            If Not File.Exists(localeEmuLoc) Then
                MessageBox.Show(owner:=SplashScreen, $"Missing Locale Emulator... Download is required{vbCrLf}LocaleEmu Files need to be located at {localeEmuLoc}")
                My.logger.Logger.LogInfo("Missing Locale Emulator")
                OpenURL("https://github.com/xupefei/Locale-Emulator/releases")
                Form1.QuitApplication()
            End If

            ' Check for ShaderGlass
            My.logger.Logger.LogInfo("Checking for ShaderGlass")
            If Not File.Exists(ShaderGlassLoc) Then
                MessageBox.Show(owner:=SplashScreen, $"Missing ShaderGlass... Download is required{vbCrLf}ShaderGlass Files need to be located at {ShaderGlassLoc}")
                My.logger.Logger.LogInfo("Missing ShaderGlass")
                OpenURL("https://github.com/mausimus/ShaderGlass/releases")
                Form1.QuitApplication()
            End If

            ' Check for Java 8
            My.logger.Logger.LogInfo("Checking for Java 8")
            If Not IsJava8Update152Installed() Then
                MessageBox.Show(owner:=SplashScreen, "Missing JAVA 8... Download is required")
                My.logger.Logger.LogInfo("Missing JAVA 8")
                OpenURL("https://mega.nz/file/FxUFjTLD#lPYnDLjytnFfBJqqvb60osAxg10RjQAkt7CMjEG4MXw")
                Form1.QuitApplication()
            End If

            ' Check for Visual C++ Runtimes
            My.logger.Logger.LogInfo("Checking for C++ Runtimes")
            If Not IsVCRuntime2022Installed() Then
                MessageBox.Show(owner:=SplashScreen, "Unable to Detect C++ Runtimes... To ensure compatibility, we recommend you install this Runtime AIO Package.")
                My.logger.Logger.LogInfo("Missing C++ Runtimes")
                OpenURL("https://www.techpowerup.com/download/visual-c-redistributable-runtime-package-all-in-one/")
                Form1.QuitApplication()
            End If

            Return True
        End Function
        Shared Function IsJava8Update152Installed() As Boolean
            Dim javaVersions As String() = {
                "SOFTWARE\JavaSoft\Java Runtime Environment\1.8.0_152",
                "SOFTWARE\WOW6432Node\JavaSoft\Java Runtime Environment\1.8.0_152"
            }

            For Each regPath In javaVersions
                Using key As RegistryKey = Registry.LocalMachine.OpenSubKey(regPath)
                    If key IsNot Nothing Then
                        Return True ' Java 8 Update 152 is installed
                    End If
                End Using
            Next

            Return False ' Java 8 Update 152 is not installed
        End Function
        Shared Function IsVCRuntime2022Installed() As Boolean
            Dim vcPaths As String() = {
                "SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\x64", ' 64-bit
                "SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\x86"  ' 32-bit
            }

            For Each regPath In vcPaths
                Using key As RegistryKey = Registry.LocalMachine.OpenSubKey(regPath)
                    If key IsNot Nothing Then
                        Dim installed As Object = key.GetValue("Installed")
                        If installed IsNot Nothing AndAlso installed.ToString() = "1" Then
                            Return True ' VC++ Runtime 2022 is installed
                        End If
                    End If
                End Using
            Next

            Return False ' VC++ Runtime 2022 is NOT installed
        End Function
        Shared Sub OpenURL(url As String)
            Try
                Process.Start(New ProcessStartInfo With {
                    .FileName = url,
                    .UseShellExecute = True
                })
            Catch ex As Exception
                MessageBox.Show("Failed to open the URL: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
        Public Shared Sub CheckForSpacesInPath()
            Dim exePath As String = Assembly.GetExecutingAssembly().Location
            Dim invalidChars As Char() = {" "c, "("c, ")"c, "{"c, "}"c}

            If exePath.IndexOfAny(invalidChars) <> -1 Then
                MessageBox.Show(owner:=SplashScreen, "The path to the KeitaiWikiLauncher contains invalid characters:" & Environment.NewLine &
                                """" & exePath & """" & Environment.NewLine &
                                "Due to LocaleEmulator please move it to a location without spaces, parentheses (), or braces {}.",
                                "Warning - Invalid Path Characters",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning)
            End If
        End Sub

        'MISC
        Public Shared Function IsDpiScalingSet(exePath As String) As Boolean
            Try
                Dim regPath As String = "Software\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers"

                ' Open the registry key for reading
                Using key As RegistryKey = Registry.CurrentUser.OpenSubKey(regPath, False)
                    If key IsNot Nothing Then
                        Dim value As Object = key.GetValue(exePath)
                        ' Check if the key exists and contains the expected DPI scaling flag
                        If value IsNot Nothing AndAlso value.ToString().Contains("HIGHDPIAWARE") Then
                            Return True ' DPI Scaling override is set
                        End If
                    End If
                End Using
            Catch ex As Exception
                MessageBox.Show("Error reading registry: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

            Return False ' DPI Scaling override is not set
        End Function
        Public Shared Sub SetDpiScaling(exePath As String)
            Try
                ' Define the registry path
                Dim regPath As String = "Software\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers"

                ' Open the registry key
                Using key As RegistryKey = Registry.CurrentUser.OpenSubKey(regPath, True)
                    If key Is Nothing Then
                        ' Create the key if it doesn't exist
                        Using newKey As RegistryKey = Registry.CurrentUser.CreateSubKey(regPath)
                            newKey.SetValue(exePath, "~ HIGHDPIAWARE", RegistryValueKind.String)
                        End Using
                    Else
                        ' Set the value for the EXE path
                        key.SetValue(exePath, "~ HIGHDPIAWARE", RegistryValueKind.String)
                    End If
                End Using

                'MessageBox.Show("DPI Scaling override set successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                logger.Logger.LogInfo($"Set DPIAware: {exePath}")
            Catch ex As Exception
                logger.Logger.LogInfo($"Failed to Set DPIAware: {exePath}")
                MessageBox.Show("Error modifying registry: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
        Public Shared Sub ImportRegFile(filePath As String)
            If IO.File.Exists(filePath) Then
                logger.Logger.LogInfo($"Importing Reg: {filePath}")
                Dim proc As New Process()
                proc.StartInfo.FileName = "regedit.exe"
                proc.StartInfo.Arguments = "/s """ & filePath & """"
                proc.StartInfo.UseShellExecute = True
                proc.StartInfo.Verb = "runas" ' Runs as administrator
                proc.Start()
                proc.WaitForExit()
            Else
                logger.Logger.LogInfo($"Failed to Find Reg: {filePath}")
                Throw New IO.FileNotFoundException("Registry file not found: " & filePath)
            End If
        End Sub
        Public Shared Function IsRunningAsAdmin() As Boolean
            Try
                Dim identity = WindowsIdentity.GetCurrent()
                Dim principal = New WindowsPrincipal(identity)
                Return principal.IsInRole(WindowsBuiltInRole.Administrator)
            Catch ex As Exception
                'MessageBox.Show("Error checking privileges: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End Try
        End Function
        Public Shared Sub DeleteLogIfTooLarge(logFilePath As String)
            Try
                ' Ensure the file exists before checking
                If File.Exists(logFilePath) Then
                    ' Get the file size in bytes
                    Dim fileInfo As New FileInfo(logFilePath)
                    Dim fileSizeInMB As Double = fileInfo.Length / (1024 * 1024) ' Convert bytes to MB

                    ' Check if the file size exceeds 10MB
                    If fileSizeInMB >= 10 Then
                        File.Delete(logFilePath)
                        'MessageBox.Show("Log file deleted as it exceeded 10MB.", "Log Cleanup", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                End If

            Catch ex As Exception
                MessageBox.Show("Error cleaning up log file: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
        Public Shared Function IsInternetAvailable(InputUrl As String, Optional timeout As Integer = 60000) As Boolean
            Try
                Dim request As Net.HttpWebRequest = CType(Net.WebRequest.Create(InputUrl), Net.HttpWebRequest)
                request.Timeout = timeout
                request.ReadWriteTimeout = timeout
                request.Method = "GET"
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)" ' Common User-Agent
                request.AllowAutoRedirect = True

                Using response As Net.HttpWebResponse = CType(request.GetResponse(), Net.HttpWebResponse)
                    Dim code As Integer = CInt(response.StatusCode)
                    If code >= 200 AndAlso code < 300 Then
                        Return True
                    Else
                        My.logger.Logger.LogWarning($"Internet check failed: Status code {code} from {InputUrl}")
                        Return False
                    End If
                End Using
            Catch ex As Exception
                My.logger.Logger.LogError($"Internet check exception for {InputUrl}: {ex.Message}")
                Return False
            End Try
        End Function
        Public Shared Sub ShowSnackBar(InputString)
            Dim SnackBarMessage As New ReaLTaiizor.Controls.MaterialSnackBar(InputString)
            SnackBarMessage.Show(Form1)
        End Sub

        'Check for APP Updates
        Shared Sub CheckForUpdates(latestVersionUrl As String)
            Dim currentVersion As String = KeitaiWorldLauncher.My.Application.Info.Version.ToString ' Get the current version of the app

            Try
                Dim client As New WebClient()
                Dim latestVersion As String = client.DownloadString(latestVersionUrl).Trim()

                If currentVersion <> latestVersion Then
                    MessageBox.Show($"You are running an outdated version ({currentVersion}){vbCrLf}A new version ({latestVersion}) is available to download.")
                    'Dim result = MessageBox.Show($"A new version ({latestVersion}) is available. Would you like to update?", "Update Available", MessageBoxButtons.YesNo, MessageBoxIcon.Information)

                    'If result = DialogResult.Yes Then
                    '    Process.Start("https://example.com/download") ' Link to download the new version
                    'End If
                Else
                    'MessageBox.Show("Launcher is up to date.", "No Updates", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            Catch ex As Exception
                MessageBox.Show(owner:=SplashScreen, $"Failed to check for updates. Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        'Generate Controls
        Public Shared Sub GenerateDynamicControlsFromLines(JAMFile As String, container As Panel)
            Try
                ' Clear any existing controls in the container (Panel)
                container.Controls.Clear()

                ' Create and configure a TableLayoutPanel
                Dim tableLayout As New TableLayoutPanel() With {
            .ColumnCount = 2,
            .AutoSize = True,
            .Dock = DockStyle.Top
        }
                ' Define the column widths: first fixed, second fills remaining space
                tableLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 150))
                tableLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100))

                ' Read the file lines (using the proper encoding)
                Dim lines = File.ReadAllLines(JAMFile, Encoding.GetEncoding(932))
                Dim rowIndex As Integer = 0

                For Each line As String In lines
                    ' Skip empty lines
                    If String.IsNullOrWhiteSpace(line) Then Continue For

                    ' Split the line into key and value
                    Dim parts As String() = line.Split(New Char() {"="c}, 2)
                    If parts.Length <> 2 Then Continue For ' Skip invalid lines

                    Dim key As String = parts(0).Trim()
                    Dim value As String = parts(1).Trim()

                    ' Create a label for the key
                    Dim lbl As New Label() With {
                        .Text = key,
                        .Width = 150,
                        .AutoSize = False,
                        .AutoEllipsis = True,
                        .Anchor = AnchorStyles.Left
                    }

                    ' Create a textbox for the value
                    Dim txt As New TextBox() With {
                        .Text = value,
                        .Dock = DockStyle.Fill,
                        .ReadOnly = True
                    }

                    ' Increase row count and add a new row style for each row
                    tableLayout.RowCount = rowIndex + 1
                    tableLayout.RowStyles.Add(New RowStyle(SizeType.AutoSize))

                    ' Add controls to the tableLayoutPanel
                    tableLayout.Controls.Add(lbl, 0, rowIndex)
                    tableLayout.Controls.Add(txt, 1, rowIndex)
                    rowIndex += 1
                Next

                ' Add the TableLayoutPanel to the container (Panel)
                container.Controls.Add(tableLayout)
            Catch ex As Exception
                MessageBox.Show($"Error generating controls for JAM: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub



        'Get App Icons
        Public Sub ExtractAndResizeAppIcon(jarFilePath As String, jamFilePath As String, SelectedGame As Game)
            Try
                ' Define the target icon path
                Dim iconOutputFolder As String = "data\tools\icons"
                If Not Directory.Exists(iconOutputFolder) Then
                    Directory.CreateDirectory(iconOutputFolder)
                End If

                ' Read the JAM file and extract the AppIcon line
                Dim appIconLine As String = File.ReadLines(jamFilePath).FirstOrDefault(Function(line) line.StartsWith("AppIcon"))
                If appIconLine Is Nothing Then
                    logger.Logger.LogError("No AppIcon entry found in the JAM file.")
                    Return
                End If

                ' Extract the first icon filename using regex or string parsing
                Dim iconFileName As String = appIconLine.Split("="c)(1).Split(","c)(0).Trim()

                ' Open the .jar file (ZIP archive)
                Using archive As ZipArchive = ZipFile.OpenRead(jarFilePath)
                    ' Find the icon inside the .jar using the filename extracted from the JAM file
                    Dim iconEntry As ZipArchiveEntry = archive.GetEntry(iconFileName)

                    If iconEntry IsNot Nothing Then
                        ' Extract the icon and load it as an image
                        Using originalStream As Stream = iconEntry.Open()
                            Dim originalIcon As Image = Image.FromStream(originalStream)

                            ' Resize the icon to 24x24
                            Dim resizedIcon As New Bitmap(36, 36)
                            Using graphics As Graphics = Graphics.FromImage(resizedIcon)
                                graphics.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                                graphics.DrawImage(originalIcon, 0, 0, 36, 36)
                            End Using

                            ' Generate the output path and save the resized icon
                            Dim outputFileName As String = Path.GetFileNameWithoutExtension(jarFilePath) & ".gif"
                            Dim outputFilePath As String = Path.Combine(iconOutputFolder, outputFileName)
                            resizedIcon.Save(outputFilePath, System.Drawing.Imaging.ImageFormat.Gif)
                            UpdateListViewItemIcon(SelectedGame.ENTitle, outputFilePath)
                            logger.Logger.LogInfo($"Icon successfully extracted and resized: {outputFilePath}")
                        End Using
                    Else
                        logger.Logger.LogInfo($"Icon '{iconFileName}' not found in the .jar file.")
                        MessageBox.Show($"Icon '{iconFileName}' not found in the .jar file.", "Icon Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                End Using
            Catch ex As Exception
                logger.Logger.LogError($"Error extracting and resizing the icon: {ex.Message}")
                MessageBox.Show($"Error extracting and resizing the icon: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
        Private Sub UpdateListViewItemIcon(gameTitle As String, newIconPath As String)
            ' Ensure the ImageList exists
            If Form1.ListViewGames.SmallImageList Is Nothing Then
                Form1.ListViewGames.SmallImageList = New ImageList()
                Form1.ListViewGames.SmallImageList.ImageSize = New Size(24, 24)
            End If

            ' Load the new icon
            If File.Exists(newIconPath) Then
                Dim newIcon As Image = Image.FromFile(newIconPath)
                Dim newImageKey As String = Path.GetFileNameWithoutExtension(newIconPath)

                ' Add the new icon to the ImageList (if not already added)
                If Not Form1.ListViewGames.SmallImageList.Images.ContainsKey(newImageKey) Then
                    Form1.ListViewGames.SmallImageList.Images.Add(newImageKey, newIcon)
                End If

                ' Find and update the ListView item
                For Each item As ListViewItem In Form1.ListViewGames.Items
                    If item.Text = gameTitle Then
                        item.ImageKey = newImageKey ' Update the item's ImageKey
                        Exit For
                    End If
                Next
            Else
                MessageBox.Show("Icon file not found: " & newIconPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End Sub

        'Launch App
        Public Function IsProcessRunning(processName As String) As Boolean
            ' Get the list of all processes with the given name
            Dim runningProcesses As Process() = Process.GetProcessesByName(processName)

            ' Return true if any matching process is found, otherwise false
            Return runningProcesses.Length > 0
        End Function
        Shared Function CheckAndCloseDoja() As Boolean
            Dim dojaProcesses = Process.GetProcessesByName("doja")

            If dojaProcesses.Length = 0 Then
                logger.Logger.LogInfo("doja.exe is not currently running.")
                Return False
            End If

            logger.Logger.LogWarning($"Found {dojaProcesses.Length} instance(s) of doja.exe running.")
            Dim result = MessageBox.Show("doja.exe is currently running. Do you want to close it?",
                                 "Confirm Close",
                                 MessageBoxButtons.YesNo,
                                 MessageBoxIcon.Question)

            If result = DialogResult.Yes Then
                Try
                    logger.Logger.LogInfo("User agreed to close doja.exe.")
                    CheckAndCloseShaderGlass()

                    For Each process As Process In dojaProcesses
                        logger.Logger.LogInfo($"Attempting to close process PID={process.Id} Name={process.ProcessName}")
                        process.Kill()
                        process.WaitForExit()
                    Next

                    logger.Logger.LogInfo("All doja.exe processes closed successfully.")
                    Return False ' No longer running
                Catch ex As Exception
                    logger.Logger.LogError("Error while closing doja.exe: " & ex.ToString())
                    MessageBox.Show("An error occurred while trying to close doja.exe: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
                    Return True ' Still running
                End Try
            Else
                logger.Logger.LogInfo("User chose not to close doja.exe.")
                Return True ' Still running
            End If
        End Function
        Shared Function CheckAndCloseStar() As Boolean
            Dim starProcesses = Process.GetProcessesByName("star")

            If starProcesses.Length = 0 Then
                logger.Logger.LogInfo("star.exe is not currently running.")
                Return False
            End If

            logger.Logger.LogWarning($"Found {starProcesses.Length} instance(s) of star.exe running.")
            Dim result = MessageBox.Show("star.exe is currently running. Do you want to close it?",
                                 "Confirm Close",
                                 MessageBoxButtons.YesNo,
                                 MessageBoxIcon.Question)

            If result = DialogResult.Yes Then
                Try
                    logger.Logger.LogInfo("User agreed to close star.exe.")
                    CheckAndCloseShaderGlass()

                    For Each process As Process In starProcesses
                        logger.Logger.LogInfo($"Attempting to close process PID={process.Id} Name={process.ProcessName}")
                        process.Kill()
                        process.WaitForExit()
                    Next

                    logger.Logger.LogInfo("All star.exe processes closed successfully.")
                    Return False ' No longer running
                Catch ex As Exception
                    logger.Logger.LogError("Error while closing star.exe: " & ex.ToString())
                    MessageBox.Show("An error occurred while trying to close star.exe: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
                    Return True ' Still running
                End Try
            Else
                logger.Logger.LogInfo("User chose not to close star.exe.")
                Return True ' Still running
            End If
        End Function
        Shared Function CheckAndCloseShaderGlass() As Boolean
            Dim shaderglassProcesses = Process.GetProcessesByName("shaderglass")

            If shaderglassProcesses.Length = 0 Then
                logger.Logger.LogInfo("shaderglass.exe is not currently running.")
                Return False
            End If

            logger.Logger.LogWarning($"Found {shaderglassProcesses.Length} instance(s) of shaderglass.exe running.")

            Try
                For Each process As Process In shaderglassProcesses
                    logger.Logger.LogInfo($"Attempting to close process PID={process.Id} Name={process.ProcessName}")
                    process.Kill()
                    process.WaitForExit()
                Next

                logger.Logger.LogInfo("All shaderglass.exe processes closed successfully.")
                Return False ' No longer running
            Catch ex As Exception
                logger.Logger.LogError("Error while closing shaderglass.exe: " & ex.ToString())
                MessageBox.Show("An error occurred while trying to close shaderglass.exe: " & ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error)
                Return True ' Still running
            End Try
        End Function
        Public Async Sub LaunchCustomDOJAGameCommand(DOJAPATH As String, DOJAEXELocation As String, GameJAM As String)
            Try
                ' Validate inputs
                If String.IsNullOrWhiteSpace(DOJAPATH) OrElse String.IsNullOrWhiteSpace(DOJAEXELocation) OrElse String.IsNullOrWhiteSpace(GameJAM) Then
                    Throw New ArgumentException("One or more required parameters are missing.")
                End If

                'Start overlay
                UtilManager.ShowLaunchOverlay(Form1)


                ' Construct all paths
                Dim baseDir As String = AppDomain.CurrentDomain.BaseDirectory
                Dim useLocaleEmulator As Boolean = Form1.chkbxLocalEmulator.Checked
                Dim appPath As String
                Dim arguments As String
                Dim dojaExePath As String = DOJAEXELocation.Trim
                Dim jamPath As String = Path.Combine(baseDir, GameJAM).Trim

                ' Warn about long JAM paths
                If jamPath.Length > 240 Then
                    logger.Logger.LogWarning($"[Launch] Potentially long JAM path: {jamPath}")
                    MessageBox.Show("The file path length exceeds 240 characters. You may experience issues running. Try moving Keitai World Emulator to the root of C:/", "Path Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

                ' Determine launch method
                If useLocaleEmulator Then
                    ' Launch using Locale Emulator
                    appPath = Path.Combine(baseDir, "data", "tools", "locale_emulator", "LEProc.exe").Trim()
                    Dim guidArg As String = "-runas ad1a7fe1-4f95-45ba-b563-9ba60c3642d3"
                    arguments = $"{guidArg} ""{dojaExePath}"" -i ""{jamPath}"" -s device1"
                    logger.Logger.LogInfo($"[Launch] Launching via Locale Emulator with arguments: {arguments}")
                Else
                    ' Launch directly
                    appPath = dojaExePath
                    arguments = $"-i ""{jamPath}"" -s device1"
                    logger.Logger.LogInfo($"[Launch] Launching directly without Locale Emulator: {arguments}")
                End If

                ' Update UI skin
                If Not Await UpdateDOJADeviceSkin(DOJAPATH, Form1.chkbxHidePhoneUI.Checked) Then
                    logger.Logger.LogError("[Launch] Failed to update DOJA skins.")
                    MessageBox.Show("Failed to update DOJA skins.", "Skin Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If

                ' Get DOJA version
                Dim VerIndex As Integer = DOJAPATH.LastIndexOf("\iDKDoJa", StringComparison.OrdinalIgnoreCase)
                Dim DOJAVER As Integer = DOJAPATH.Substring(VerIndex + 8, 1)

                ' Update draw size for DOJA5
                If DOJAVER = 5 Then
                    Dim dimensions = ExtractDOJAWidthHeight(jamPath)
                    Dim width As Integer = dimensions.Item1
                    Dim height As Integer = dimensions.Item2
                    Await UpdatedDOJADrawSize(DOJAPATH, width, height)
                    logger.Logger.LogInfo($"[Launch] Updated DOJA draw size to {width}x{height}")
                End If

                ' Update sound config
                Await UpdateDOJASoundConf(DOJAPATH, Form1.cobxAudioType.SelectedItem.ToString())
                logger.Logger.LogInfo($"[Launch] Updated DOJA sound config to {Form1.cobxAudioType.SelectedItem}")

                ' Update app config and JAM entries
                Await UpdateDOJAAppconfig(DOJAPATH, jamPath)
                If DOJAVER = 3 Then
                    Await RemoveDOJAJamFileEntries(jamPath)
                    logger.Logger.LogInfo("[Launch] Removed DOJA3 jam entries")
                    'convert SP to SCR
                    Dim rootFolder As String = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(GameJAM), ".."))
                    Dim gameName As String = Path.GetFileNameWithoutExtension(GameJAM)
                    Dim SPFile As String = Path.Combine(rootFolder, "sp", gameName & ".sp")
                    Await ProcessDoja3SPtoSCR(SPFile)
                Else
                    Await EnsureDOJAJamFileEntries(jamPath)
                    logger.Logger.LogInfo("[Launch] Ensured DOJA jam entries")
                End If
                UpdateNetworkUIDinJAM(GameJAM)

                ' Let filesystem settle (especially important on slower drives)
                Await Task.Delay(500)

                ' Launch the emulator using Locale Emulator
                Dim startInfo As New ProcessStartInfo With {
                    .FileName = appPath,
                    .Arguments = arguments,
                    .UseShellExecute = False,
                    .CreateNoWindow = True,
                    .RedirectStandardError = True,
                    .RedirectStandardOutput = True,
                    .WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                }

                ' Launch DOJA with retry logic
                Dim success As Boolean = Await LaunchEmulatorWithRetry(
                    appPath,
                    arguments,
                    "doja",
                    baseDir,
                    AddressOf WaitForDojaToStart
                )

                If Not success Then
                    MessageBox.Show("Failed to launch DOJA after retrying.", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If

                ' ShaderGlass launch if enabled
                If Form1.chkbxShaderGlass.Checked Then
                    logger.Logger.LogInfo("[ShaderGlass] Waiting for DOJA to become idle...")
                    If Await WaitForDojaToStart() Then
                        Await LaunchShaderGlass(Path.GetFileNameWithoutExtension(jamPath))
                        ProcessManager.StartMonitoring()
                        logger.Logger.LogInfo("[ShaderGlass] ShaderGlass launched and monitoring started.")
                    Else
                        logger.Logger.LogError("[ShaderGlass] Failed to detect DOJA running.")
                        MessageBox.Show("Failed to detect DOJA running.", "ShaderGlass Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                End If

            Catch ex As Exception
                logger.Logger.LogError($"[Launch] Exception occurred: {ex}")
                MessageBox.Show($"Failed to launch the game: {ex.Message}", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
        Public Async Sub LaunchCustomSTARGameCommand(STARPATH As String, STAREXELocation As String, GameJAM As String)
            Try
                logger.Logger.LogInfo("[Launch] Starting STAR game launch sequence...")

                ' Validate inputs
                If String.IsNullOrWhiteSpace(STARPATH) OrElse String.IsNullOrWhiteSpace(STAREXELocation) OrElse String.IsNullOrWhiteSpace(GameJAM) Then
                    Throw New ArgumentException("One or more required parameters are missing.")
                End If

                'Start overlay
                UtilManager.ShowLaunchOverlay(Form1)

                ' Prepare all paths
                Dim baseDir = AppDomain.CurrentDomain.BaseDirectory
                Dim useLocaleEmulator As Boolean = Form1.chkbxLocalEmulator.Checked
                Dim appPath As String
                Dim arguments As String

                Dim exePath As String = STAREXELocation.Trim
                Dim jamPath As String = Path.Combine(baseDir, GameJAM).Trim()

                If jamPath.Length > 240 Then
                    logger.Logger.LogWarning($"[Launch] JAM file path exceeds 240 characters: {jamPath}")
                    MessageBox.Show("The file path length exceeds 240 characters. You might experience issues running. Try moving Keitai World Emulator to the root of C:/", "Path Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

                ' Form arguments based on launch method
                If useLocaleEmulator Then
                    appPath = Path.Combine(baseDir, "data", "tools", "locale_emulator", "LEProc.exe").Trim()
                    Dim guidArg As String = "-runas ad1a7fe1-4f95-45ba-b563-9ba60c3642d3" ' your custom profile GUID
                    arguments = $"{guidArg} ""{exePath}"" -i ""{jamPath}"""
                    logger.Logger.LogInfo($"[Launch] Launching STAR via Locale Emulator with arguments: {arguments}")
                Else
                    appPath = exePath
                    arguments = $"-i ""{jamPath}"""
                    logger.Logger.LogInfo($"[Launch] Launching STAR directly without Locale Emulator: {arguments}")
                End If

                ' Update STAR UI skin
                If Not Await UpdateSTARDeviceSkin(STARPATH, Form1.chkbxHidePhoneUI.Checked) Then
                    logger.Logger.LogError("[Launch] Failed to update STAR skins.")
                    MessageBox.Show("Failed to update STAR skins.", "Skin Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If

                ' Screen size
                Dim dimensions = ExtractSTARWidthHeight(jamPath)
                Await UpdatedSTARDrawSize(STARPATH, dimensions.Item1, dimensions.Item2)
                logger.Logger.LogInfo($"[Launch] STAR draw size set to {dimensions.Item1}x{dimensions.Item2}")

                ' Config updates
                Await UpdateSTARSoundConf(STARPATH, Form1.cobxAudioType.SelectedItem.ToString())
                logger.Logger.LogInfo($"[Launch] STAR sound config set to {Form1.cobxAudioType.SelectedItem}")
                Await UpdateSTARAppconfig(STARPATH, GameJAM)
                Await EnsureSTARJamFileEntries(GameJAM)
                logger.Logger.LogInfo("[Launch] STAR app configuration and JAM entries updated.")
                UpdateNetworkUIDinJAM(GameJAM)

                Await Task.Delay(500) ' Let the filesystem settle

                ' Create process info
                Dim startInfo As New ProcessStartInfo With {
                    .FileName = appPath,
                    .Arguments = arguments,
                    .UseShellExecute = False,
                    .CreateNoWindow = True,
                    .RedirectStandardError = True,
                    .RedirectStandardOutput = True,
                    .WorkingDirectory = baseDir
                }

                ' Launch STAR with retry logic
                Dim success As Boolean = Await LaunchEmulatorWithRetry(
                    appPath,
                    arguments,
                    "star",
                    baseDir,
                    AddressOf WaitForSTARToStart
                )

                If Not success Then
                    MessageBox.Show("Failed to launch STAR after retrying.", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If

                ' ShaderGlass launch if enabled
                If Form1.chkbxShaderGlass.Checked Then
                    logger.Logger.LogInfo("[ShaderGlass] Waiting for STAR to become idle...")
                    If Await WaitForSTARToStart() Then
                        Await LaunchShaderGlass(Path.GetFileNameWithoutExtension(jamPath))
                        ProcessManager.StartMonitoring()
                        logger.Logger.LogInfo("[ShaderGlass] ShaderGlass launched and monitoring started.")
                    Else
                        logger.Logger.LogError("[ShaderGlass] Failed to detect STAR running.")
                        MessageBox.Show("Failed to detect STAR running.", "ShaderGlass Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                End If

            Catch ex As ArgumentException
                logger.Logger.LogError($"[Launch] Invalid input: {ex.Message}")
                MessageBox.Show($"Invalid input: {ex.Message}", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)

            Catch ex As Exception
                logger.Logger.LogError($"[Launch] Exception occurred: {ex}")
                MessageBox.Show($"Failed to launch the game: {ex.Message}", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
        Public Sub LaunchCustomMachiCharaCommand(MachiCharaEXE, CFDFile)
            Try
                ' Paths and arguments
                Dim appPath As String = AppDomain.CurrentDomain.BaseDirectory & "data\tools\locale_emulator\LEProc.exe"
                Dim guidArg As String = "-runas ad1a7fe1-4f95-45ba-b563-9ba60c3642d3"
                Dim machicharaexePath As String = MachiCharaEXE
                Dim CFDPath As String = AppDomain.CurrentDomain.BaseDirectory & CFDFile

                ' Combine arguments
                Dim arguments As String = $"""{machicharaexePath}"" ""{CFDPath}"""

                ' Set up process start info
                Dim processInfo As New ProcessStartInfo()
                processInfo.FileName = "cmd.exe" ' Use cmd.exe to run the command
                processInfo.Arguments = "/C " & Chr(34) & arguments & Chr(34) ' /C runs the command and exits
                processInfo.UseShellExecute = False ' Do not use the OS shell
                processInfo.CreateNoWindow = True ' Do not create a visible cmd window
                processInfo.RedirectStandardOutput = True ' Redirect output if needed
                processInfo.RedirectStandardError = True ' Redirect errors if needed
                processInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory ' Set working directory

                ' Start the process
                Dim process As Process = Process.Start(processInfo)

            Catch ex As Exception
                MessageBox.Show($"Failed to launch the command: {ex.Message}", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
        Public Async Function LaunchShaderGlass(AppName As String) As Task
            Dim baseDir As String = AppDomain.CurrentDomain.BaseDirectory
            Dim appPath As String = Path.Combine(baseDir, "data", "tools", "shaderglass", "ShaderGlass.exe")
            Dim argumentFile As String = Path.Combine(baseDir, "data", "tools", "shaderglass", "keitai.sgp")

            If Not File.Exists(appPath) Then
                MessageBox.Show("ShaderGlass executable not found at: " & appPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            If Not File.Exists(argumentFile) Then
                MessageBox.Show("Argument file not found at: " & argumentFile, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Await ModifyCaptureWindow(argumentFile, AppName)
            Await ModifyScalingWindow(argumentFile)

            Dim startInfo As New ProcessStartInfo() With {
                .FileName = appPath,
                .Arguments = $"""{argumentFile}""",
                .UseShellExecute = True,
                .WorkingDirectory = baseDir
            }

            Await Task.Delay(1000)

            Try
                Process.Start(startInfo)
            Catch ex As Exception
                MessageBox.Show("Failed to launch ShaderGlass: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Function
        Public Async Function ModifyCaptureWindow(filePath As String, AppName As String) As Task
            If Not File.Exists(filePath) Then
                Console.WriteLine($"ShaderGlass config file not found: {filePath}")
                Return
            End If

            Dim lines As List(Of String) = (Await File.ReadAllLinesAsync(filePath)).ToList()

            For i As Integer = 0 To lines.Count - 1
                If lines(i).StartsWith("CaptureWindow") Then
                    lines(i) = $"CaptureWindow ""{AppName}"""
                    Exit For
                End If
            Next

            Await File.WriteAllLinesAsync(filePath, lines)
        End Function
        Public Async Function ModifyScalingWindow(filePath As String) As Task
            Dim selectedValue As String = Form1.cbxShaderGlassScaling.SelectedItem.ToString()
            Dim scaleValue As Integer

            Select Case selectedValue
                Case "1x"
                    scaleValue = 100
                Case "1.5x"
                    scaleValue = 150
                Case "2x"
                    scaleValue = 200
                Case "2.5x"
                    scaleValue = 250
                Case "3x"
                    scaleValue = 300
                Case "3.5x"
                    scaleValue = 350
                Case "4x"
                    scaleValue = 400
                Case Else
                    scaleValue = 100 ' default value
            End Select

            If Not File.Exists(filePath) Then
                Console.WriteLine($"ShaderGlass config file not found: {filePath}")
                Return
            End If

            Dim lines As List(Of String) = (Await File.ReadAllLinesAsync(filePath)).ToList()

            For i As Integer = 0 To lines.Count - 1
                If lines(i).StartsWith("OutputScale") Then
                    lines(i) = $"OutputScale ""{scaleValue}"""
                    Exit For
                End If
            Next

            Await File.WriteAllLinesAsync(filePath, lines)
        End Function


        'Asynchronous method to wait for the "doja/star" process to start
        Private Async Function WaitForDojaToStart(Optional timeoutMilliseconds As Integer = 4000) As Task(Of Boolean)
            Dim startTime As DateTime = DateTime.Now

            While (DateTime.Now - startTime).TotalMilliseconds < timeoutMilliseconds
                Dim dojaProcesses As Process() = Process.GetProcessesByName("doja")

                For Each proc In dojaProcesses
                    If proc.MainWindowHandle <> IntPtr.Zero Then
                        logger.Logger.LogInfo("[WaitForDojaToStart] DOJA process ready with MainWindowHandle.")
                        Return True
                    End If
                Next

                Await Task.Delay(500)
            End While

            logger.Logger.LogError("[WaitForDojaToStart] Timed out waiting for DOJA window.")
            Return False
        End Function
        Private Async Function WaitForSTARToStart(Optional timeoutMilliseconds As Integer = 4000) As Task(Of Boolean)
            Dim startTime As DateTime = DateTime.Now

            While (DateTime.Now - startTime).TotalMilliseconds < timeoutMilliseconds
                Dim dojaProcesses As Process() = Process.GetProcessesByName("star")

                For Each proc In dojaProcesses
                    If proc.MainWindowHandle <> IntPtr.Zero Then
                        logger.Logger.LogInfo("[WaitForSTARToStart] STAR process ready with MainWindowHandle.")
                        Return True
                    End If
                Next

                Await Task.Delay(500)
            End While

            logger.Logger.LogError("[WaitForSTARToStart] Timed out waiting for STAR window.")
            Return False
        End Function

        'Helpers
        Public Async Function LaunchEmulatorWithRetry(
            fileName As String,
            arguments As String,
            processNameToCheck As String,
            workingDir As String,
            waitFunction As Func(Of Task(Of Boolean))
        ) As Task(Of Boolean)

            Dim startInfo As New ProcessStartInfo With {
            .FileName = fileName,
            .Arguments = arguments,
            .UseShellExecute = False,
            .CreateNoWindow = True,
            .RedirectStandardOutput = True,
            .RedirectStandardError = True,
            .WorkingDirectory = workingDir
        }

            For attempt = 1 To 2
                Try
                    Dim process As Process = Process.Start(startInfo)

                    If process Is Nothing Then
                        logger.Logger.LogError($"[LaunchHelper] Attempt {attempt}: Failed to start process.")
                        Continue For
                    End If

                    process.WaitForInputIdle()

                    AddHandler process.OutputDataReceived, Sub(sender, e)
                                                               If e.Data IsNot Nothing Then logger.Logger.LogInfo($"[{processNameToCheck.ToUpper()} STDOUT] {e.Data}")
                                                           End Sub
                    AddHandler process.ErrorDataReceived, Sub(sender, e)
                                                              If e.Data IsNot Nothing Then logger.Logger.LogError($"[{processNameToCheck.ToUpper()} STDERR] {e.Data}")
                                                          End Sub
                    process.BeginOutputReadLine()
                    process.BeginErrorReadLine()

                    logger.Logger.LogInfo($"[LaunchHelper] Waiting for {processNameToCheck} to become ready...")

                    If Await waitFunction() Then
                        logger.Logger.LogInfo($"[LaunchHelper] {processNameToCheck} is running and ready.")
                        UtilManager.HideLaunchOverlay()
                        Return True
                    Else
                        logger.Logger.LogWarning($"[LaunchHelper] {processNameToCheck} failed to become ready on attempt {attempt}. Retrying...")
                        process.Kill()
                        process.Dispose()
                        Await Task.Delay(500)
                    End If
                Catch ex As Exception
                    logger.Logger.LogError($"[LaunchHelper] Exception during attempt {attempt}: {ex.Message}")
                End Try
            Next

            logger.Logger.LogError($"[LaunchHelper] Failed to start {processNameToCheck} after 2 attempts.")
            UtilManager.HideLaunchOverlay()
            Return False
        End Function
        Public Shared Sub ShowLaunchOverlay(parentForm As Form)
            If LaunchOverlay Is Nothing Then
                LaunchOverlay = New Panel With {
                .BackColor = Color.FromArgb(128, Color.LightGray),
                .Dock = DockStyle.Fill,
                .Cursor = Cursors.WaitCursor
            }

                ' Optional "Launching..." text
                Dim loadingLabel As New Label With {
                .Text = "Launching...",
                .ForeColor = Color.Black,
                .Font = New Font("Segoe UI", 16, FontStyle.Bold),
                .BackColor = Color.Transparent,
                .AutoSize = True
            }

                ' Center the label after the overlay is added
                LaunchOverlay.Controls.Add(loadingLabel)
                AddHandler LaunchOverlay.Resize, Sub()
                                                     loadingLabel.Left = (LaunchOverlay.Width - loadingLabel.Width) \ 2
                                                     loadingLabel.Top = (LaunchOverlay.Height - loadingLabel.Height) \ 2
                                                 End Sub
            End If

            If Not parentForm.Controls.Contains(LaunchOverlay) Then
                parentForm.Controls.Add(LaunchOverlay)
            End If

            LaunchOverlay.BringToFront()
            LaunchOverlay.Visible = True
            Application.DoEvents()
        End Sub
        Public Shared Sub HideLaunchOverlay()
            If LaunchOverlay IsNot Nothing Then
                LaunchOverlay.Visible = False
            End If
        End Sub
        Public Shared Function UpdateNetworkUIDinJAM(JamFile As String) As Boolean
            If Not File.Exists(JamFile) Then
                logger.Logger.LogError($"ERROR: Did not find {JamFile} to update networkUID")
                Return False
            End If

            If Form1.NetworkUID.ToLower.Trim = "nullgwdocomo" Then
                logger.Logger.LogInfo("Skipping update to NetworkUID in Jam due to it not being set (still NULLGWDOCOMO).")
                Return True
            End If

            Dim originalLines = File.ReadAllLines(JamFile)
            Dim lines As New List(Of String)
            Dim replacementCount As Integer = 0

            For Each line In originalLines
                If Regex.IsMatch(line, "NULLGWDOCOMO", RegexOptions.IgnoreCase) Then
                    replacementCount += Regex.Matches(line, "NULLGWDOCOMO", RegexOptions.IgnoreCase).Count
                    Dim newLine = Regex.Replace(line, "NULLGWDOCOMO", Form1.NetworkUID, RegexOptions.IgnoreCase)
                    lines.Add(newLine)
                Else
                    lines.Add(line)
                End If
            Next

            If replacementCount > 0 Then
                logger.Logger.LogInfo($"Replaced {replacementCount} occurrence(s) of 'NULLGWDOCOMO' with '{Form1.NetworkUID}' in {JamFile}")
            Else
                logger.Logger.LogInfo($"No occurrences of 'NULLGWDOCOMO' found in {JamFile}. No changes made.")
            End If

            File.WriteAllLines(JamFile, lines)
            logger.Logger.LogInfo($"Successfully updated {JamFile}")
            Return True
        End Function

        'DOJA EXTRAS
        Public Async Function UpdateDOJADeviceSkin(DOJALOCATION As String, hideUI As Boolean) As Task(Of Boolean)
            Return Await Task.Run(Function()
                                      Try
                                          Dim dojaSkinFolder As String = Path.Combine(DOJALOCATION, "lib", "skin", "device1")
                                          Dim DojaPath = Path.GetFileName(DOJALOCATION)

                                          ' Clear or create skin folder
                                          If Directory.Exists(dojaSkinFolder) Then
                                              For Each fi In Directory.GetFiles(dojaSkinFolder)
                                                  File.Delete(fi)
                                              Next
                                          Else
                                              Directory.CreateDirectory(dojaSkinFolder)
                                          End If

                                          ' Choose correct UI skin folder
                                          Dim skinUIFolder As String = If(hideUI, "noui", "ui")
                                          Dim ourSkinsFolder As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "tools", "skins", "doja", skinUIFolder, DojaPath)

                                          If Not Directory.Exists(ourSkinsFolder) Then
                                              MessageBox.Show($"Skin folder missing: {ourSkinsFolder}")
                                              logger.Logger.LogError($"Skin folder missing: {ourSkinsFolder}")
                                              Return False
                                          End If

                                          ' Copy skins
                                          For Each skinFile In Directory.GetFiles(ourSkinsFolder)
                                              Dim destFile = Path.Combine(dojaSkinFolder, Path.GetFileName(skinFile))
                                              File.Copy(skinFile, destFile, True)
                                          Next

                                          Return True
                                      Catch ex As Exception
                                          logger.Logger.LogError($"[Skin Update Error] {ex.Message}")
                                          Return False
                                      End Try
                                  End Function)
        End Function
        Public Function ExtractDOJAWidthHeight(filePath As String) As (Integer, Integer)
            Dim width As Integer = 240
            Dim height As Integer = 240
            Try
                For Each line As String In File.ReadLines(filePath)
                    If line.StartsWith("DrawArea =", StringComparison.OrdinalIgnoreCase) Then
                        Dim dimensions = line.Split("="c)(1).Trim().Split("x"c)
                        If dimensions.Length = 2 Then
                            width = Convert.ToInt32(dimensions(0).Trim())
                            height = Convert.ToInt32(dimensions(1).Trim())
                        End If
                        Exit For
                    End If
                Next
            Catch ex As Exception
                Console.WriteLine($"Error reading width And height: {ex.Message}")
            End Try

            Return (width, height)
        End Function
        Public Async Function UpdatedDOJADrawSize(DOJALOCATION As String, width As Integer, height As Integer) As Task
            Dim deviceInfoFile As String = Path.Combine(DOJALOCATION, "lib", "skin", "deviceinfo", "device1")
            Dim newValue As String = $"device1,{width},{height},120,120"
            Await File.WriteAllTextAsync(deviceInfoFile, newValue)
        End Function
        Public Async Function UpdateDOJASoundConf(DOJALOCATION As String, soundType As String) As Task
            Dim soundPath As String = Path.Combine(DOJALOCATION, "lib", "SoundConf.properties")
            If Not File.Exists(soundPath) Then
                Console.WriteLine($"File Not found: {soundPath}")
                Return
            End If

            Try
                ' Read the file with Shift-JIS encoding
                Dim encoding = Text.Encoding.GetEncoding("shift-jis")
                Dim conf As String = Await File.ReadAllTextAsync(soundPath, encoding)

                ' Update mode and sound library
                conf = Regex.Replace(conf, "MODE=.", "MODE=0")

                Dim soundLibValue As String = If(soundType = "903i", "002", "001")
                conf = Regex.Replace(conf, "SOUNDLIB=...", $"SOUNDLIB={soundLibValue}")

                ' Write it back
                Await File.WriteAllTextAsync(soundPath, conf, encoding)

            Catch ex As Exception
                Console.WriteLine($"Error updating sound configuration: {ex.Message}")
            End Try
        End Function
        Public Async Function UpdateDOJAAppconfig(DOJALOCATION As String, GAMEJAM As String) As Task
            Await Task.Run(Sub()
                               Try
                                   Dim AppConfigFile = Path.Combine(DOJALOCATION, "AppSetting")
                                   Dim AppConfigPROPFile = Path.Combine(DOJALOCATION, "AppSetting.properties")
                                   Dim GameDirectory As String = ""
                                   Dim GameName = Path.GetFileNameWithoutExtension(GAMEJAM)

                                   Dim binIndex As Integer = GAMEJAM.LastIndexOf("\bin")
                                   If binIndex <> -1 Then
                                       GameDirectory = GAMEJAM.Substring(0, binIndex)
                                   Else
                                       Console.WriteLine("'\bin' not found in path.")
                                       Return
                                   End If

                                   Dim VerIndex As Integer = DOJALOCATION.LastIndexOf("\iDKDoJa")
                                   Dim DOJAVER As Integer = DOJALOCATION.Substring(VerIndex + 8, 1)

                                   Dim targetAppConfig = Path.Combine(GameDirectory, GameName)
                                   Dim targetPropConfig = Path.Combine(GameDirectory, $"{GameName}.properties")

                                   If DOJAVER = 3 Then
                                       File.Copy(AppConfigFile, targetAppConfig, True)
                                       File.Copy(AppConfigPROPFile, targetPropConfig, True)
                                   ElseIf DOJAVER = 5 Then
                                       File.Copy(AppConfigFile, targetAppConfig, True)
                                       If File.Exists(targetPropConfig) Then
                                           File.Delete(targetPropConfig)
                                       End If
                                   End If
                               Catch ex As Exception
                                   Console.WriteLine($"Error updating DOJA AppConfig: {ex.Message}")
                               End Try
                           End Sub)
        End Function
        Public Async Function EnsureDOJAJamFileEntries(GAMEJAM As String) As Task
            If Not File.Exists(GAMEJAM) Then
                Throw New FileNotFoundException($"The file '{GAMEJAM}' does not exist.")
            End If

            Dim enc = Encoding.GetEncoding("shift-jis")
            Dim lines As List(Of String) = (Await File.ReadAllLinesAsync(GAMEJAM, enc)).ToList()
            lines.Add(vbCrLf)
            Dim modified As Boolean = False

            ' Remove empty or whitespace-only lines
            lines = lines.Where(Function(line) Not String.IsNullOrWhiteSpace(line)).ToList()

            ' Normalize spacing around equals signs
            Dim keyValuePattern As New Regex("^(\S+)\s*=\s*(.*)$")
            For i As Integer = 0 To lines.Count - 1
                Dim match As Match = keyValuePattern.Match(lines(i))
                If match.Success Then
                    Dim key As String = match.Groups(1).Value
                    Dim value As String = match.Groups(2).Value
                    lines(i) = $"{key} = {value}"
                    modified = True
                End If
            Next

            ' Required entries
            Dim requiredEntries As New Dictionary(Of String, String) From {
        {"TrustedAPID", "00000000000"},
        {"MessageCode", "0000000000"}
    }

            ' Add missing entries
            For Each entry In requiredEntries
                If Not lines.Any(Function(line) Regex.IsMatch(line, $"^{Regex.Escape(entry.Key)}\s*=")) Then
                    lines.Add(entry.Key & " = " & entry.Value)
                    modified = True
                End If
            Next

            ' Fix PackageURL
            Dim packageUrlPattern As New Regex("^PackageURL\s*=\s*(.+)$", RegexOptions.IgnoreCase)
            For i As Integer = 0 To lines.Count - 1
                Dim match As Match = packageUrlPattern.Match(lines(i))
                If match.Success Then
                    Dim value As String = match.Groups(1).Value.Trim()
                    If Not value.StartsWith("http://", StringComparison.OrdinalIgnoreCase) AndAlso
               Not value.StartsWith("https://", StringComparison.OrdinalIgnoreCase) Then

                        Dim fileName As String = Path.GetFileName(value)
                        Dim folderName As String = Path.GetFileNameWithoutExtension(fileName)
                        lines(i) = $"PackageURL = http://localhost/{folderName}/{fileName}"
                        modified = True
                    End If
                    Exit For
                End If
            Next

            ' Always write back
            Await File.WriteAllLinesAsync(GAMEJAM, lines, enc)
        End Function
        Public Async Function RemoveDOJAJamFileEntries(GAMEJAM As String) As Task
            If Not File.Exists(GAMEJAM) Then
                Throw New FileNotFoundException($"The file '{GAMEJAM}' does not exist.")
            End If

            Dim enc = Encoding.GetEncoding("shift-jis")
            Dim originalLines As List(Of String) = (Await File.ReadAllLinesAsync(GAMEJAM, enc)).ToList()

            Dim entriesToRemove As List(Of String) = New List(Of String) From {
        "TrustedAPID =",
        "MessageCode ="
    }

            ' Perform filtering on background thread
            Dim filteredLines As List(Of String) = Await Task.Run(Function()
                                                                      Return originalLines.Where(Function(line) Not entriesToRemove.Any(Function(entry) line.StartsWith(entry))).ToList()
                                                                  End Function)

            If filteredLines.Count <> originalLines.Count Then
                Await File.WriteAllLinesAsync(GAMEJAM, filteredLines, enc)
            End If
        End Function
        Public Async Function ProcessDoja3SPtoSCR(inputFilePath As String) As Task
            ' Check if SP file exists
            If Not File.Exists(inputFilePath) Then
                logger.Logger.LogInfo("File not found: " & inputFilePath)
                Exit Function
            End If

            Dim SCRFile = inputFilePath.Replace(".sp", "0.scr")
            If File.Exists(SCRFile) Then
                logger.Logger.LogInfo("SCR already found: " & SCRFile)
                Exit Function
            End If

            Dim fileBytes() As Byte = Await File.ReadAllBytesAsync(inputFilePath)

            ' Check if the file is at least 8 bytes long to inspect bytes 4-7
            If fileBytes.Length < 8 Then
                logger.Logger.LogInfo("File is too short to process: " & inputFilePath)
                Exit Function
            End If

            ' Check if bytes 4 to 7 are all 0xFF
            If fileBytes(4) = &HFF AndAlso fileBytes(5) = &HFF AndAlso fileBytes(6) = &HFF AndAlso fileBytes(7) = &HFF Then
                ' Remove the first 0x40 bytes (64 bytes)
                If fileBytes.Length <= &H40 Then
                    logger.Logger.LogInfo("File is too short to strip 0x40 bytes: " & inputFilePath)
                    Exit Function
                End If

                Dim trimmedBytes(fileBytes.Length - &H41) As Byte
                Array.Copy(fileBytes, &H40, trimmedBytes, 0, trimmedBytes.Length)

                ' Generate new file name with .scr extension
                Dim newFilePath As String = Path.Combine(Path.GetDirectoryName(inputFilePath),
                                                  Path.GetFileNameWithoutExtension(inputFilePath) & "0.scr")

                Await File.WriteAllBytesAsync(newFilePath, trimmedBytes)
                logger.Logger.LogInfo("File processed and saved as: " & newFilePath)
            Else
                logger.Logger.LogInfo("Bytes 4-7 are not all 0xFF. No changes made: " & inputFilePath)
            End If
        End Function


        'STAR EXTRAS
        Public Async Function UpdateSTARDeviceSkin(STARLOCATION As String, hideUI As Boolean) As Task(Of Boolean)
            Return Await Task.Run(Function()
                                      Try
                                          Dim StarSkinFolder = Path.Combine(STARLOCATION, "lib", "skin", "device1")

                                          If Directory.Exists(StarSkinFolder) Then
                                              For Each deleteFile In Directory.GetFiles(StarSkinFolder, "*.*", SearchOption.TopDirectoryOnly)
                                                  File.Delete(deleteFile)
                                              Next
                                          Else
                                              Directory.CreateDirectory(StarSkinFolder)
                                          End If

                                          Dim OurSkinsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "tools", "skins", "star")
                                          If Not Directory.Exists(OurSkinsFolder) Then
                                              MessageBox.Show($"Skin folder missing: {OurSkinsFolder}")
                                              logger.Logger.LogError($"Skin folder missing: {OurSkinsFolder}")
                                              Return False
                                          End If

                                          Dim skinSubfolder = If(hideUI, "noui\star-device1-noui", "ui\star-device1-ui")
                                          Dim sourceFolder = Path.Combine(OurSkinsFolder, skinSubfolder)

                                          If Not Directory.Exists(sourceFolder) Then
                                              MessageBox.Show($"Sub skin folder missing: {sourceFolder}")
                                              logger.Logger.LogError($"Sub skin folder missing: {sourceFolder}")
                                              Return False
                                          End If

                                          For Each F In Directory.GetFiles(sourceFolder)
                                              File.Copy(F, Path.Combine(StarSkinFolder, Path.GetFileName(F)), True)
                                          Next

                                          Return True
                                      Catch ex As Exception
                                          logger.Logger.LogError($"[STAR Skin Update] Error: {ex.Message}")
                                          Return False
                                      End Try
                                  End Function)
        End Function
        Function ExtractSTARWidthHeight(filePath As String) As (Integer, Integer)
            Dim width As Integer = 0
            Dim height As Integer = 0

            Try
                For Each line As String In IO.File.ReadLines(filePath)
                    If line.StartsWith("DrawArea =") Then
                        Dim parts() As String = line.Split("="c)(1).Trim().Split("x"c)
                        If parts.Length = 2 Then
                            width = Convert.ToInt32(parts(0).Trim())
                            height = Convert.ToInt32(parts(1).Trim())
                        End If
                        Exit For
                    End If
                Next
                If width = 0 Or height = 0 Then
                    width = 480
                    height = 480
                End If
            Catch ex As Exception
                Console.WriteLine("Error: " & ex.Message)
            End Try

            Return (width, height)
        End Function
        Public Async Function UpdatedSTARDrawSize(STARLOCATION As String, X As Integer, Y As Integer) As Task
            Dim device1InfoFile As String = Path.Combine(STARLOCATION, "lib", "skin", "deviceinfo", "device1")
            Dim newValue As String = $"device1,{X},{Y},120,120,0,2,0,1,3"
            Await File.WriteAllTextAsync(device1InfoFile, newValue)
        End Function
        Public Async Function UpdateSTARSoundConf(STARLOCATION As String, soundType As String) As Task
            Dim soundPath As String = Path.Combine(STARLOCATION, "lib", "SoundConf.properties")

            If Not File.Exists(soundPath) Then
                Console.WriteLine($"File not found: {soundPath}")
                Return
            End If

            Try
                Dim enco = Text.Encoding.GetEncoding("shift-jis")
                Dim conf As String = Await File.ReadAllTextAsync(soundPath, enco)

                ' Apply substitutions
                conf = Regex.Replace(conf, "MODE=.", "MODE=0")
                Dim soundLibValue As String = If(soundType = "903i", "002", "001")
                conf = Regex.Replace(conf, "SOUNDLIB=...", $"SOUNDLIB={soundLibValue}")

                ' Write updated config
                Await File.WriteAllTextAsync(soundPath, conf, enco)

            Catch ex As Exception
                Console.WriteLine($"Error updating STAR sound config: {ex.Message}")
            End Try
        End Function
        Public Async Function UpdateSTARAppconfig(STARLOCATION As String, GAMEJAM As String) As Task
            Await Task.Run(Sub()
                               Try
                                   Dim appConfigFile = Path.Combine(STARLOCATION, "AppSetting")
                                   Dim appConfigFCFile = Path.Combine(STARLOCATION, "AppSetting.fc")
                                   Dim gameName = Path.GetFileNameWithoutExtension(GAMEJAM)
                                   Dim binIndex As Integer = GAMEJAM.LastIndexOf("\bin")
                                   Dim gameDirectory As String = ""

                                   If binIndex <> -1 Then
                                       gameDirectory = GAMEJAM.Substring(0, binIndex)
                                   Else
                                       Console.WriteLine("'\bin' not found in path.")
                                       Return
                                   End If

                                   If Not File.Exists(appConfigFile) OrElse Not File.Exists(appConfigFCFile) Then
                                       MessageBox.Show("Missing STAR AppSettings files")
                                       Return
                                   End If

                                   File.Copy(appConfigFile, Path.Combine(gameDirectory, gameName), True)
                                   File.Copy(appConfigFCFile, Path.Combine(gameDirectory, $"{gameName}.fc"), True)

                               Catch ex As Exception
                                   Console.WriteLine($"Error updating STAR AppConfig: {ex.Message}")
                               End Try
                           End Sub)
        End Function
        Public Async Function EnsureSTARJamFileEntries(GAMEJAM As String) As Task
            If Not File.Exists(GAMEJAM) Then
                Throw New FileNotFoundException($"The file '{GAMEJAM}' does not exist.")
            End If

            Dim enc = Encoding.GetEncoding("shift-jis")
            Dim lines As List(Of String) = (Await File.ReadAllLinesAsync(GAMEJAM, enc)).ToList()
            Dim modified As Boolean = False

            ' Remove empty or whitespace-only lines
            lines = lines.Where(Function(line) Not String.IsNullOrWhiteSpace(line)).ToList()

            ' Normalize spacing around equal signs
            Dim keyValuePattern As New Regex("^(\S+)\s*=\s*(.*)$")
            For i As Integer = 0 To lines.Count - 1
                Dim match As Match = keyValuePattern.Match(lines(i))
                If match.Success Then
                    Dim key As String = match.Groups(1).Value
                    Dim value As String = match.Groups(2).Value
                    lines(i) = $"{key} = {value}"
                    modified = True
                End If
            Next

            ' Required entries
            Dim requiredEntries As New Dictionary(Of String, String) From {
        {"UseNetwork", "yes"},
        {"TrustedAPID", "00000000000"},
        {"MessageCode", "0000000000"}
    }

            For Each entry In requiredEntries
                If Not lines.Any(Function(line) Regex.IsMatch(line, $"^{Regex.Escape(entry.Key)}\s*=")) Then
                    lines.Add($"{entry.Key} = {entry.Value}")
                    modified = True
                End If
            Next

            ' Handle PackageURL
            Dim packageUrlPattern As New Regex("^PackageURL\s*=\s*(.+)$", RegexOptions.IgnoreCase)
            For i As Integer = 0 To lines.Count - 1
                Dim match As Match = packageUrlPattern.Match(lines(i))
                If match.Success Then
                    Dim value As String = match.Groups(1).Value.Trim()
                    If Not value.StartsWith("http://", StringComparison.OrdinalIgnoreCase) AndAlso
               Not value.StartsWith("https://", StringComparison.OrdinalIgnoreCase) Then
                        Dim fileName As String = Path.GetFileName(value)
                        Dim folderName As String = Path.GetFileNameWithoutExtension(fileName)
                        lines(i) = $"PackageURL = http://localhost/{folderName}/{fileName}"
                        modified = True
                    End If
                    Exit For
                End If
            Next

            ' Write back if modified
            If modified Then
                Await File.WriteAllLinesAsync(GAMEJAM, lines, enc)
            End If
        End Function


        'MISC
        'Generate XML for List
        Dim BadFolders As New List(Of String)
        Dim emulator As String = "doja"
        Dim variants As New List(Of String)
        Public Async Function ProcessZipFileforGamelistAsync(inputZipPath As String) As Task
            BadFolders.Clear()
            If Not File.Exists(inputZipPath) Then
                Throw New FileNotFoundException($"The file '{inputZipPath}' does not exist.")
            End If

            Dim tempFolder As String = Path.Combine(Path.GetTempPath(), "ZipProcessing")
            Dim extractedFolder As String = Path.Combine(tempFolder, "Extracted")
            Dim xmlFilePath As String = Path.Combine(Path.GetDirectoryName(inputZipPath), "gamelist.xml")

            If Directory.Exists(tempFolder) Then
                Directory.Delete(tempFolder, True)
            End If

            Directory.CreateDirectory(extractedFolder)
            ZipFile.ExtractToDirectory(inputZipPath, extractedFolder)

            Dim xmlSettings As New XmlWriterSettings() With {
                .Encoding = Encoding.GetEncoding("shift-jis"),
                .Indent = True
            }

            Dim xmlDoc As New XmlDocument()
            If File.Exists(xmlFilePath) Then
                xmlDoc.Load(xmlFilePath)
            Else
                Dim root As XmlElement = xmlDoc.CreateElement("Gamelist")
                xmlDoc.AppendChild(root)
            End If

            Try
                For Each rootFolderPathOriginal As String In Directory.GetDirectories(extractedFolder, "*", SearchOption.TopDirectoryOnly)
                    Dim originalFolderName As String = Path.GetFileName(rootFolderPathOriginal)
                    Dim urlSafeFolderName As String = MakeUrlSafeName(originalFolderName)

                    Dim rootFolderPath As String = rootFolderPathOriginal
                    If originalFolderName <> urlSafeFolderName Then
                        Dim newFolderPath As String = Path.Combine(Path.GetDirectoryName(rootFolderPathOriginal), urlSafeFolderName)

                        ' Cleanup if folder already exists
                        If Directory.Exists(newFolderPath) Then
                            Directory.Delete(newFolderPath, True)
                        End If

                        Directory.Move(rootFolderPathOriginal, newFolderPath)
                        rootFolderPath = newFolderPath
                    End If
                    variants.Clear()
                    Dim enTitle As String = originalFolderName ' For display in XML    
                    Dim zipFileName As String = Nothing
                    Dim zipSDFileName As String = Nothing
                    emulator = "doja"

                    Dim subFolders = Directory.GetDirectories(rootFolderPath, "*", SearchOption.TopDirectoryOnly)
                    Dim jamFiles = Directory.GetFiles(rootFolderPath, "*.jam", SearchOption.TopDirectoryOnly)
                    Dim jarFiles = Directory.GetFiles(rootFolderPath, "*.jar", SearchOption.TopDirectoryOnly)
                    Dim spFiles = Directory.GetFiles(rootFolderPath, "*.sp", SearchOption.TopDirectoryOnly)
                    Dim scrFiles = Directory.GetFiles(rootFolderPath, "*.scr", SearchOption.TopDirectoryOnly)

                    If subFolders.Length = 1 AndAlso jamFiles.Length = 0 AndAlso jarFiles.Length = 0 AndAlso spFiles.Length = 0 AndAlso scrFiles.Length = 0 Then
                        zipFileName = Await ProcessSingleVarientAsync(subFolders(0), urlSafeFolderName, inputZipPath, tempFolder)

                    ElseIf subFolders.Length = 0 AndAlso jamFiles.Length = 1 AndAlso jarFiles.Length = 1 Then
                        zipFileName = Await ProcessSingleVarientAsync(rootFolderPath, urlSafeFolderName, inputZipPath, tempFolder)

                    ElseIf subFolders.Length > 1 AndAlso jamFiles.Length = 0 AndAlso jarFiles.Length = 0 Then
                        Dim zipFileNamesTuple = Await ProcessMultipleVarientAsync(rootFolderPath, urlSafeFolderName, variants, inputZipPath, tempFolder)
                        zipFileName = zipFileNamesTuple.Item1
                        If zipFileNamesTuple.Item2 IsNot Nothing Then
                            zipSDFileName = zipFileNamesTuple.Item2
                        End If
                    Else
                        BadFolders.Add(originalFolderName)
                    End If

                    ' Add game entry to XML
                    Dim root As XmlElement = xmlDoc.DocumentElement
                    Dim gameElement As XmlElement = xmlDoc.CreateElement("Game")

                    Dim enTitleElement As XmlElement = xmlDoc.CreateElement("ENTitle")
                    enTitleElement.InnerText = enTitle
                    gameElement.AppendChild(enTitleElement)

                    Dim jpTitleElement As XmlElement = xmlDoc.CreateElement("JPTitle")
                    jpTitleElement.InnerText = enTitle
                    gameElement.AppendChild(jpTitleElement)

                    Dim zipNameElement As XmlElement = xmlDoc.CreateElement("ZIPName")
                    zipNameElement.InnerText = zipFileName
                    gameElement.AppendChild(zipNameElement)

                    Dim downloadUrlElement As XmlElement = xmlDoc.CreateElement("DownloadURL")
                    downloadUrlElement.InnerText = $"iappli/{zipFileName}"
                    gameElement.AppendChild(downloadUrlElement)

                    Dim customAppIconURLElement As XmlElement = xmlDoc.CreateElement("CustomAppIconURL")
                    customAppIconURLElement.InnerText = ""
                    gameElement.AppendChild(customAppIconURLElement)

                    Dim sdCardDataURLElement As XmlElement = xmlDoc.CreateElement("SDCardDataURL")
                    sdCardDataURLElement.InnerText = If(zipSDFileName IsNot Nothing, $"iappli/sdcarddata/{Path.GetFileNameWithoutExtension(zipFileName)}_sdcarddata.zip", "")
                    gameElement.AppendChild(sdCardDataURLElement)

                    Dim emulatorElement As XmlElement = xmlDoc.CreateElement("Emulator")
                    emulatorElement.InnerText = emulator
                    gameElement.AppendChild(emulatorElement)

                    Dim variantsElement As XmlElement = xmlDoc.CreateElement("Variants")
                    variantsElement.InnerText = String.Join(",", variants)
                    gameElement.AppendChild(variantsElement)

                    root.AppendChild(gameElement)
                Next

            Catch ex As Exception
                MessageBox.Show($"Error Creating ZIP: {ex}")
            End Try

            Using writer As XmlWriter = XmlWriter.Create(xmlFilePath, xmlSettings)
                xmlDoc.Save(writer)
            End Using

            Directory.Delete(tempFolder, True)

            If BadFolders.Count > 0 Then
                Dim outputString = $"Completed XML Creation with Bad {BadFolders.Count} Folders{vbCrLf}"
                For Each f In BadFolders
                    outputString += $"{f}{vbCrLf}"
                Next
                MessageBox.Show(outputString)
            Else
                MessageBox.Show("Completed XML Creation with no Errors")
            End If
        End Function
        Private Async Function ProcessSingleVarientAsync(folderPath As String, inputENTitle As String, inputZipPath As String, tempFolder As String) As Task(Of String)
            Await Task.Run(Sub() RenameFilesRecursively(folderPath, inputENTitle))

            Dim jamFile As String = Directory.GetFiles(folderPath, "*.jam", SearchOption.TopDirectoryOnly).FirstOrDefault()
            Dim jarFile As String = Directory.GetFiles(folderPath, "*.jar", SearchOption.TopDirectoryOnly).FirstOrDefault()
            Dim spFile As String = Directory.GetFiles(folderPath, "*.sp", SearchOption.TopDirectoryOnly).FirstOrDefault()
            Dim scrFile As String = Directory.GetFiles(folderPath, "*.scr", SearchOption.TopDirectoryOnly).FirstOrDefault()

            If String.IsNullOrEmpty(jarFile) Then Return Nothing

            If Not String.IsNullOrEmpty(jamFile) Then
                Dim jamLines = Await File.ReadAllLinesAsync(jamFile, Encoding.GetEncoding("shift-jis"))
                If jamLines.Any(Function(line) line.StartsWith("AppType = ")) Then
                    emulator = "star"
                Else
                    emulator = "doja"
                End If
            End If

            Dim binFolder = Path.Combine(tempFolder, "bin")
            Dim spFolder = Path.Combine(tempFolder, "sp")
            Directory.CreateDirectory(binFolder)
            Directory.CreateDirectory(spFolder)

            Await Task.Run(Sub()
                               If Not String.IsNullOrEmpty(jamFile) Then File.Move(jamFile, Path.Combine(binFolder, Path.GetFileName(jamFile)), True)
                               If Not String.IsNullOrEmpty(jarFile) Then File.Move(jarFile, Path.Combine(binFolder, Path.GetFileName(jarFile)), True)
                               If Not String.IsNullOrEmpty(spFile) Then File.Move(spFile, Path.Combine(spFolder, Path.GetFileName(spFile)), True)
                               If Not String.IsNullOrEmpty(scrFile) Then File.Move(scrFile, Path.Combine(spFolder, Path.GetFileName(scrFile)), True)
                           End Sub)

            Dim zipFileName = Path.GetFileNameWithoutExtension(jarFile) & ".zip"
            Dim outputZipPath = Path.Combine(Path.GetDirectoryName(inputZipPath), zipFileName)
            If File.Exists(outputZipPath) Then File.Delete(outputZipPath)

            Dim tempZipFolder = Path.Combine(tempFolder, "ToZip")
            Directory.CreateDirectory(tempZipFolder)
            Await DirectoryCopyAsync(binFolder, Path.Combine(tempZipFolder, "bin"), True)
            Await DirectoryCopyAsync(spFolder, Path.Combine(tempZipFolder, "sp"), True)

            ZipFile.CreateFromDirectory(tempZipFolder, outputZipPath)
            Directory.Delete(tempZipFolder, True)
            Directory.Delete(binFolder, True)
            Directory.Delete(spFolder, True)

            Return zipFileName
        End Function
        Private Async Function ProcessMultipleVarientAsync(folderPath As String, RootFolderName As String, variants As List(Of String), inputZipPath As String, tempFolder As String) As Task(Of Tuple(Of String, String))
            Dim MasterJarName As String = ""
            Dim zipSDFileName As String = Nothing
            Dim combinedZipPath = Path.Combine(Path.GetDirectoryName(inputZipPath), RootFolderName & ".zip")
            Dim tempCombinedZipFolder = Path.Combine(tempFolder, "CombinedZip")
            Directory.CreateDirectory(tempCombinedZipFolder)

            For Each VariantFolder In Directory.GetDirectories(folderPath)
                Dim variantBaseName = Path.GetFileName(VariantFolder).Replace(" ", "_").Trim()

                ' Handle sdcarddata as a special case
                If variantBaseName.ToLower() = "sdcarddata" Then
                    Dim exportSDCardDataFolder = Path.Combine(Path.GetDirectoryName(inputZipPath), "sdcarddata")
                    Directory.CreateDirectory(exportSDCardDataFolder)
                    zipSDFileName = RootFolderName.Replace(" ", "_") & "_" & variantBaseName & ".zip"
                    Dim outputSDZipPath = Path.Combine(exportSDCardDataFolder, zipSDFileName)
                    If File.Exists(outputSDZipPath) Then File.Delete(outputSDZipPath)
                    ZipFile.CreateFromDirectory(VariantFolder, outputSDZipPath)
                    Continue For
                End If

                ' Check if variant folder has subfolders (e.g., Part 1, Part 2)
                Dim subVariantFolders = Directory.GetDirectories(VariantFolder)
                If subVariantFolders.Length > 0 Then
                    For Each subVariant In subVariantFolders
                        ' Check if subVariant contains required files
                        Dim hasValidFiles As Boolean =
                    Directory.GetFiles(subVariant, "*.jam", SearchOption.TopDirectoryOnly).Any() AndAlso
                    Directory.GetFiles(subVariant, "*.jar", SearchOption.TopDirectoryOnly).Any() AndAlso
                    (Directory.GetFiles(subVariant, "*.sp", SearchOption.TopDirectoryOnly).Any() OrElse
                     Directory.GetFiles(subVariant, "*.scr", SearchOption.TopDirectoryOnly).Any())

                        If Not hasValidFiles Then
                            BadFolders.Add($"Missing required files: {subVariant}")
                            Continue For
                        End If

                        Dim subName = Path.GetFileName(subVariant).Replace(" ", "_").Trim()
                        Dim fullVariantName = MakeUrlSafeName($"{variantBaseName}-{subName}")

                        If fullVariantName.ToLower() <> "sdcarddata" Then
                            variants.Add(fullVariantName)
                        End If

                        Dim variantTargetFolder = Path.Combine(tempCombinedZipFolder, fullVariantName)
                        Directory.CreateDirectory(variantTargetFolder)

                        Await ProcessVariantFolderAsync(subVariant, variantTargetFolder, Function(name) MasterJarName = name)
                    Next
                Else
                    ' Standard single-level variant
                    Dim fullVariantName = MakeUrlSafeName(variantBaseName)

                    If fullVariantName.ToLower() <> "sdcarddata" Then
                        variants.Add(fullVariantName)
                    End If

                    Dim variantTargetFolder = Path.Combine(tempCombinedZipFolder, fullVariantName)
                    Directory.CreateDirectory(variantTargetFolder)

                    Await ProcessVariantFolderAsync(VariantFolder, variantTargetFolder, Function(name) MasterJarName = name)
                End If
            Next

            ' Final ZIP
            Dim zipFileName = Path.GetFileNameWithoutExtension(RootFolderName).Replace(" ", "_") & ".zip"
            Dim outputZipPath = Path.Combine(Path.GetDirectoryName(inputZipPath), zipFileName)
            If File.Exists(outputZipPath) Then File.Delete(outputZipPath)

            Await Task.Run(Sub() RenameFilesRecursively(tempCombinedZipFolder, Path.GetFileNameWithoutExtension(zipFileName)))
            ZipFile.CreateFromDirectory(tempCombinedZipFolder, outputZipPath)
            Directory.Delete(tempCombinedZipFolder, True)

            Return Tuple.Create(zipFileName, zipSDFileName)
        End Function

        Private Async Function ProcessVariantFolderAsync(sourceFolder As String, variantTargetFolder As String, Optional setJarName As Action(Of String) = Nothing) As Task
            Dim jamFile = Directory.GetFiles(sourceFolder, "*.jam", SearchOption.TopDirectoryOnly).FirstOrDefault()
            Dim jarFile = Directory.GetFiles(sourceFolder, "*.jar", SearchOption.TopDirectoryOnly).FirstOrDefault()
            Dim spFile = Directory.GetFiles(sourceFolder, "*.sp", SearchOption.TopDirectoryOnly).FirstOrDefault()
            Dim scrFile = Directory.GetFiles(sourceFolder, "*.scr", SearchOption.TopDirectoryOnly).FirstOrDefault()

            If String.IsNullOrEmpty(jarFile) Then Return

            If setJarName IsNot Nothing Then setJarName(Path.GetFileNameWithoutExtension(jarFile))

            If Not String.IsNullOrEmpty(jamFile) Then
                Dim jamLines = Await File.ReadAllLinesAsync(jamFile, Encoding.GetEncoding("shift-jis"))
                If jamLines.Any(Function(line) line.StartsWith("AppType = ")) Then
                    emulator = "star"
                Else
                    emulator = "doja"
                End If
            End If

            Dim binFolder = Path.Combine(variantTargetFolder, "bin")
            Dim spFolder = Path.Combine(variantTargetFolder, "sp")
            Directory.CreateDirectory(binFolder)
            Directory.CreateDirectory(spFolder)

            Await Task.Run(Sub()
                               If Not String.IsNullOrEmpty(jamFile) Then File.Move(jamFile, Path.Combine(binFolder, Path.GetFileName(jamFile)), True)
                               If Not String.IsNullOrEmpty(jarFile) Then File.Move(jarFile, Path.Combine(binFolder, Path.GetFileName(jarFile)), True)
                               If Not String.IsNullOrEmpty(spFile) Then File.Move(spFile, Path.Combine(spFolder, Path.GetFileName(spFile)), True)
                               If Not String.IsNullOrEmpty(scrFile) Then File.Move(scrFile, Path.Combine(spFolder, Path.GetFileName(scrFile)), True)
                           End Sub)
        End Function
        Private Async Function DirectoryCopyAsync(sourceDirName As String, destDirName As String, copySubDirs As Boolean) As Task
            Await Task.Run(Sub()
                               Dim dir As New DirectoryInfo(sourceDirName)
                               Dim dirs As DirectoryInfo() = dir.GetDirectories()

                               If Not Directory.Exists(destDirName) Then
                                   Directory.CreateDirectory(destDirName)
                               End If

                               For Each file In dir.GetFiles()
                                   file.CopyTo(Path.Combine(destDirName, file.Name), True)
                               Next

                               If copySubDirs Then
                                   For Each subdir In dirs
                                       Dim tempPath = Path.Combine(destDirName, subdir.Name)
                                       DirectoryCopyAsync(subdir.FullName, tempPath, copySubDirs).Wait()
                                   Next
                               End If
                           End Sub)
        End Function
        Sub RenameFilesRecursively(ByVal directoryPath As String, newname As String)
            Try
                ' Replace spaces with underscores in the provided newname
                Dim sanitizedNewName As String = newname.Replace(" ", "_")

                ' Loop through all files in the current directory
                For Each filePath As String In Directory.GetFiles(directoryPath)
                    Dim fileExtension As String = Path.GetExtension(filePath).ToLower()

                    ' Check if the file extension matches .jar, .jam, .sp, or .scr
                    If fileExtension = ".jar" OrElse fileExtension = ".jam" OrElse fileExtension = ".sp" Then
                        ' Get the new file path with the sanitized new name
                        Dim newFilePath As String = Path.Combine(Path.GetDirectoryName(filePath), sanitizedNewName & fileExtension)

                        ' Rename the file
                        File.Move(filePath, newFilePath)

                    ElseIf fileExtension = ".scr" Then
                        ' Extract digits before .scr using a regex pattern
                        Dim fileNameWithoutExtension As String = Path.GetFileNameWithoutExtension(filePath)
                        Dim digitsMatch As Match = Regex.Match(fileNameWithoutExtension, "(\d{1,2})$")

                        Dim digits As String = If(digitsMatch.Success, digitsMatch.Value, "")

                        ' Construct the new file name with digits appended
                        Dim newFilePath As String = Path.Combine(Path.GetDirectoryName(filePath), sanitizedNewName & digits & fileExtension)

                        ' Rename the file
                        File.Move(filePath, newFilePath)
                    End If
                Next

                ' Recursively call this function for all subdirectories
                For Each subDir As String In Directory.GetDirectories(directoryPath)
                    RenameFilesRecursively(subDir, newname)
                Next

            Catch ex As Exception
                Console.WriteLine("Error: " & ex.Message)
            End Try
        End Sub
        Private Function MakeUrlSafeName(name As String) As String
            ' Converts a string to a URL-safe name by keeping only safe ASCII characters
            Dim builder As New System.Text.StringBuilder()
            For Each ch As Char In name
                Dim code As Integer = AscW(ch)
                If code >= 32 AndAlso code <= 126 AndAlso Not "{}&?^%$#".Contains(ch) Then
                    builder.Append(ch)
                Else
                    builder.Append("_")
                End If
            Next
            Return builder.ToString().Trim("_"c)
        End Function

    End Class
End Namespace