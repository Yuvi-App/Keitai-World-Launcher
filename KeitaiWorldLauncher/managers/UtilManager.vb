Imports System.IO
Imports System.IO.Compression
Imports System.Net
Imports System.Reflection
Imports System.Security.Principal
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports KeitaiWorldLauncher.My.Models
Imports Microsoft.Win32
Imports SharpDX.XInput

Namespace My.Managers
    Public Class UtilManager
        Private Shared LaunchOverlay As Panel = Nothing
        Dim gameManager As New GameManager()

        'PreReq Check
        Public Shared Async Function CheckforPreReqAsync() As Task(Of Boolean)
            ' Check for administrator privileges before continuing
            If Not Await IsRunningAsAdminAsync() Then
                MessageBox.Show(owner:=SplashScreen, "For the first-time setup, this application requires administrator privileges to configure necessary settings." & vbCrLf & vbCrLf &
                        "Please restart the application as an Administrator by right-clicking the executable and selecting 'Run as administrator'.",
                        "Administrator Privileges Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Form1.QuitApplication()
                Return False
            End If

            ' === Begin pre-req checks ===
            Dim DOJAEmulator = Form1.DOJAEXE
            Dim StarEmulator = Form1.STAREXE
            Dim localeEmuLoc = "data\tools\locale_emulator\LEProc.exe"
            Dim ShaderGlassLoc = "data\tools\shaderglass\ShaderGlass.exe"

            ' Setup DOJA and STAR Reg
            Dim toolDojaDirs = Directory.GetDirectories(Form1.ToolsFolder, "idkDoja*")
            For Each dir As String In toolDojaDirs
                Dim regFile As String = Path.Combine(dir, "doja.reg")
                Dim dojaExe As String = Path.Combine(dir, "bin", "doja.exe")
                If File.Exists(regFile) Then
                    Await UtilManager.ImportRegFileAsync(regFile)
                End If
                If File.Exists(dojaExe) AndAlso Not Await UtilManager.IsDpiScalingSetAsync(dojaExe) Then
                    Await UtilManager.SetDpiScalingAsync(dojaExe)
                End If
            Next

            Dim toolStarDirs = Directory.GetDirectories(Form1.ToolsFolder, "idkStar*")
            For Each dir As String In toolStarDirs
                Dim regFile As String = Path.Combine(dir, "star.reg")
                Dim starExe As String = Path.Combine(dir, "bin", "star.exe")
                If File.Exists(regFile) Then
                    Await UtilManager.ImportRegFileAsync(regFile)
                End If
                If File.Exists(starExe) AndAlso Not Await UtilManager.IsDpiScalingSetAsync(starExe) Then
                    Await UtilManager.SetDpiScalingAsync(starExe)
                End If
            Next

            ' Check for DOJA Emulator
            My.logger.Logger.LogInfo("Checking for DOJA Emu")
            If Not File.Exists(DOJAEmulator) Then
                MessageBox.Show(owner:=SplashScreen, $"Missing DOJA 5.1 Emulator... Download is required{vbCrLf}Emulator Files need to be located at {DOJAEmulator}")
                My.logger.Logger.LogInfo("Missing DOJA 5.1 Emulator")
                Await OpenURLAsync("https://archive.org/details/iappli-tool-dev-tools")
                Form1.QuitApplication()
            End If

            ' Check for STAR Emulator
            My.logger.Logger.LogInfo("Checking for STAR Emu")
            If Not File.Exists(StarEmulator) Then
                MessageBox.Show(owner:=SplashScreen, $"Missing STAR 2.0 Emulator... Download is required{vbCrLf}Emulator Files need to be located at {StarEmulator}")
                My.logger.Logger.LogInfo("Missing STAR 2.0 Emulator")
                Await OpenURLAsync("https://archive.org/details/iappli-tool-dev-tools")
                Form1.QuitApplication()
            End If

            ' Check for Locale Emulator
            My.logger.Logger.LogInfo("Checking for LEPROC")
            If Not File.Exists(localeEmuLoc) Then
                MessageBox.Show(owner:=SplashScreen, $"Missing Locale Emulator... Download is required{vbCrLf}LocaleEmu Files need to be located at {localeEmuLoc}")
                My.logger.Logger.LogInfo("Missing Locale Emulator")
                Await OpenURLAsync("https://github.com/xupefei/Locale-Emulator/releases")
                Form1.QuitApplication()
            End If

            ' Check for ShaderGlass
            My.logger.Logger.LogInfo("Checking for ShaderGlass")
            If Not File.Exists(ShaderGlassLoc) Then
                MessageBox.Show(owner:=SplashScreen, $"Missing ShaderGlass... Download is required{vbCrLf}ShaderGlass Files need to be located at {ShaderGlassLoc}")
                My.logger.Logger.LogInfo("Missing ShaderGlass")
                Await OpenURLAsync("https://github.com/mausimus/ShaderGlass/releases")
                Form1.QuitApplication()
            End If

            ' Check for Java 1.8
            My.logger.Logger.LogInfo("Checking for Java 8")
            If Not Await EnsureJava1_8IsConfiguredAsync() Then
                MessageBox.Show(owner:=SplashScreen, "Missing JAVA 8 (JDK8u152)... Download is required")
                My.logger.Logger.LogInfo("Missing JAVA 8")
                Await OpenURLAsync("https://scratchpad.keitaiwiki.com/s/AxTty326itLfEwE?dir=/Utilities/Java")
                Form1.QuitApplication()
            End If

            ' Check for Visual C++ Runtimes
            My.logger.Logger.LogInfo("Checking for C++ Runtimes")
            If Not Await IsVCRuntime2022InstalledAsync() Then
                MessageBox.Show(owner:=SplashScreen, "Unable to Detect C++ Runtimes... To ensure compatibility, we recommend you install this Runtime AIO Package.")
                My.logger.Logger.LogInfo("Missing C++ Runtimes")
                Await OpenURLAsync("https://www.techpowerup.com/download/visual-c-redistributable-runtime-package-all-in-one/")
                Form1.QuitApplication()
            End If

            ' Check for .net 8 Runtime
            My.logger.Logger.LogInfo("Checking for .Net 8.0.15+ Runtimes")
            If Not Await IsDotNet8Installed() Then
                MessageBox.Show(owner:=SplashScreen, "Unable to Detect .Net 8.0.15+ Runtimes... Download is required")
                My.logger.Logger.LogInfo("Missing .Net 8.0.15+ Runtimes")
                Await OpenURLAsync("https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-8.0.15-windows-x86-installer?cid=getdotnetcore")
                Form1.QuitApplication()
            End If
            Return True
        End Function
        Public Shared Async Function EnsureJava1_8IsConfiguredAsync() As Task(Of Boolean)
            ' Step 0: Check for problematic Java 1.4 installation
            Dim java14Keys As String() = {
                "SOFTWARE\JavaSoft\Java Runtime Environment\1.4",
                "SOFTWARE\WOW6432Node\JavaSoft\Java Runtime Environment\1.4"
            }

            For Each regPath In java14Keys
                Using key As RegistryKey = Registry.LocalMachine.OpenSubKey(regPath)
                    If key IsNot Nothing Then
                        My.logger.Logger.LogWarning("Detected incompatible Java 1.4 installation.")
                        MessageBox.Show(owner:=SplashScreen,
                    "Java 1.4 is installed on this system, which is incompatible with Keitai World Launcher. We recommend you uninstall it and reinstall Java 1.8.",
                    "Java Compatibility Issue", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                End Using
            Next

            ' Step 1: Locate Java 1.8 path
            Dim javaPath As String = Await Task.Run(Function()
                                                        Dim javaVersions As String() = {
                                                            "SOFTWARE\JavaSoft\Java Runtime Environment\1.8",
                                                            "SOFTWARE\WOW6432Node\JavaSoft\Java Runtime Environment\1.8"
                                                        }

                                                        For Each regPath In javaVersions
                                                            Using key As RegistryKey = Registry.LocalMachine.OpenSubKey(regPath)
                                                                If key IsNot Nothing Then
                                                                    Dim javaHome = key.GetValue("JavaHome")
                                                                    If javaHome IsNot Nothing Then
                                                                        Return javaHome.ToString()
                                                                    End If
                                                                End If
                                                            End Using
                                                        Next

                                                        Return Nothing
                                                    End Function)

            If String.IsNullOrEmpty(javaPath) Then
                MessageBox.Show(owner:=SplashScreen, "Missing JAVA 8 (v1_8)... Download is required")
                My.logger.Logger.LogInfo("Missing JAVA 8 (v1_8)")
                Await OpenURLAsync("https://scratchpad.keitaiarchive.org/s/AxTty326itLfEwE?dir=/Utilities/Java")
                Return False
            End If

            ' Step 2: Set Java Bin Path
            Form1.Java1_8BinFolderPath = Path.Combine(javaPath, "bin")

            ' Step 3: Update CurrentVersion registry key to 1.8
            Dim keyPaths As String() = {
                "SOFTWARE\Wow6432Node\JavaSoft\Java Runtime Environment"
            }

            For Each path In keyPaths
                Try
                    Using baseKey As RegistryKey = Registry.LocalMachine.OpenSubKey(path, writable:=True)
                        If baseKey IsNot Nothing Then
                            baseKey.SetValue("CurrentVersion", "1.8", RegistryValueKind.String)
                            My.logger.Logger.LogInfo($"Set CurrentVersion to 1.8 in HKLM\{path}")
                        End If
                    End Using
                Catch ex As Exception
                    My.logger.Logger.LogError($"Failed to update CurrentVersion in {path}: {ex.Message}")
                End Try
            Next

            ' Step 4: Verify
            Dim regKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Wow6432Node\JavaSoft\Java Runtime Environment")
            Dim currentVersion = regKey?.GetValue("CurrentVersion")?.ToString()
            My.logger.Logger.LogInfo("Checking for JRE CurrentVersion regKey 1.8")

            If currentVersion <> "1.8" Then
                My.logger.Logger.LogInfo("Incorrect JRE CurrentVersion regKey")
                MessageBox.Show(owner:=SplashScreen,
                "Java Runtime CurrentVersion Registry key must be set to 1.8. Please rerun app as admin to update it.",
                "Java Registry Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If

            Return True
        End Function
        Public Shared Async Function IsVCRuntime2022InstalledAsync() As Task(Of Boolean)
            Return Await Task.Run(Function()
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
                                  End Function)
        End Function
        Public Shared Async Function IsDotNet8Installed() As Task(Of Boolean)
            Dim minVersion As New Version(8, 0, 15)

            Try
                Dim startInfo As New ProcessStartInfo()
                startInfo.FileName = "dotnet"
                startInfo.Arguments = "--list-runtimes"
                startInfo.UseShellExecute = False
                startInfo.RedirectStandardOutput = True
                startInfo.CreateNoWindow = True

                Using proc As Process = Process.Start(startInfo)
                    Dim output As String = proc.StandardOutput.ReadToEnd()
                    proc.WaitForExit()

                    ' Regex to capture versions like 8.0.15
                    Dim versionPattern As String = "\s(\d+\.\d+\.\d+)\s"
                    Dim matches = Regex.Matches(output, versionPattern)

                    For Each match As Match In matches
                        Dim versionText As String = match.Groups(1).Value
                        Dim parsedVersion As Version = Nothing
                        If Version.TryParse(versionText, parsedVersion) Then
                            If parsedVersion.Major = 8 AndAlso parsedVersion >= minVersion Then
                                Return True
                            End If
                        End If
                    Next
                End Using

            Catch ex As Exception
                ' If dotnet is not found or any other error occurs, assume not installed.
            End Try

            Return False
        End Function
        Public Shared Async Function OpenURLAsync(url As String) As Task
            Await Task.Run(Sub()
                               Try
                                   Process.Start(New ProcessStartInfo With {
                               .FileName = url,
                               .UseShellExecute = True
                           })
                               Catch ex As Exception
                                   MessageBox.Show("Failed to open the URL: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                               End Try
                           End Sub)
        End Function
        Public Shared Async Function CheckForSpacesInPathAsync() As Task
            Await Task.Run(Sub()
                               Dim exePath As String = Assembly.GetExecutingAssembly().Location
                               Dim invalidChars As Char() = {" "c, "("c, ")"c, "{"c, "}"c}

                               If exePath.IndexOfAny(invalidChars) <> -1 Then
                                   ' Show the MessageBox on the UI thread
                                   If SplashScreen.InvokeRequired Then
                                       SplashScreen.Invoke(Sub()
                                                               MessageBox.Show(owner:=SplashScreen,
                                                                       "The path to the KeitaiWikiLauncher contains invalid characters:" & Environment.NewLine &
                                                                       """" & exePath & """" & Environment.NewLine &
                                                                       "Due to LocaleEmulator please move it to a location without spaces, parentheses (), or braces {}.",
                                                                       "Warning - Invalid Path Characters",
                                                                       MessageBoxButtons.OK,
                                                                       MessageBoxIcon.Warning)
                                                           End Sub)
                                   Else
                                       MessageBox.Show(owner:=SplashScreen,
                                               "The path to the KeitaiWikiLauncher contains invalid characters:" & Environment.NewLine &
                                               """" & exePath & """" & Environment.NewLine &
                                               "Due to LocaleEmulator please move it to a location without spaces, parentheses (), or braces {}.",
                                               "Warning - Invalid Path Characters",
                                               MessageBoxButtons.OK,
                                               MessageBoxIcon.Warning)
                                   End If
                               End If
                           End Sub)
        End Function
        Public Shared Async Function SetupDIRSAsync() As Task
            Await Task.Run(Sub()
                               Dim directories As String() = {
                           "data",
                           "configs",
                           "logs",
                           Path.Combine("data", "sp_backups"),
                           Path.Combine("data", "downloads"),
                           Path.Combine("data", "tools"),
                           Path.Combine("data", "tools", "icons"),
                           Path.Combine("data", "tools", "icons", "defaults"),
                           Path.Combine("data", "tools", "skins"),
                           Path.Combine("data", "tools", "skins", "doja", "ui"),
                           Path.Combine("data", "tools", "skins", "doja", "noui"),
                           Path.Combine("data", "tools", "skins", "star", "ui"),
                           Path.Combine("data", "tools", "skins", "star", "noui"),
                           Path.Combine("data", "tools", "controller-profiles")
                       }

                               For Each D In directories
                                   Directory.CreateDirectory(D)
                               Next
                           End Sub)
        End Function

        ' Stats
        Public Shared Sub SendKWLLaunchStats()
            Dim whUrl = "https://script.google.com/macros/s/AKfycbwtXDCzN-V2XYV-zDHkYfxJdDd6xPyR8xhHSpKrXhMFDQklkxgENVUvXSTcgablGoOvmQ/exec"
            Dim dotNetVersion As String = Environment.Version.ToString()
            Dim javaVersion As String = GetJavaVersion()
            Dim version As String = KeitaiWorldLauncher.My.Application.Info.Version.ToString
            Dim platform As String = Environment.OSVersion.Platform.ToString()
            Dim source As String = "KWL"
            Dim json As String = $"{{""version"":""{version}"",""platform"":""{platform}"",""dotNetVersion"":""{dotNetVersion}"",""javaVersion"":""{javaVersion}"",""source"":""{source}""}}"
            Dim client As New WebClient()
            client.Headers(HttpRequestHeader.ContentType) = "application/json"
            Try
                client.UploadStringAsync(New Uri(whUrl), "POST", json)
            Catch ex As Exception
                ' Fail silently
            End Try
        End Sub
        Public Shared Sub SendAppLaunch(inputAppName As String)
            Dim telemetryUrl As String = "https://script.google.com/macros/s/AKfycbwtXDCzN-V2XYV-zDHkYfxJdDd6xPyR8xhHSpKrXhMFDQklkxgENVUvXSTcgablGoOvmQ/exec"

            Dim appName As String = inputAppName

            Dim json As String = $"{{""appName"":""{appName}""}}"

            Dim client As New Net.WebClient()
            client.Headers(HttpRequestHeader.ContentType) = "application/json"

            Try
                client.UploadStringAsync(New Uri(telemetryUrl), "POST", json)
            Catch ex As Exception
                ' Silent fail
            End Try
        End Sub
        Public Shared Function GetJavaVersion() As String
            Try
                Dim psi As New ProcessStartInfo("java", "-version") With {
            .RedirectStandardError = True,
            .UseShellExecute = False,
            .CreateNoWindow = True
        }

                Using process As Process = Process.Start(psi)
                    Using reader As IO.StreamReader = process.StandardError
                        Dim output As String = reader.ReadToEnd()
                        ' Example output line: java version "1.8.0_321"
                        Dim match = System.Text.RegularExpressions.Regex.Match(output, "java version ""([\d._]+)""")
                        If match.Success Then
                            Return match.Groups(1).Value
                        End If
                    End Using
                End Using

                Return "Java installed, version not detected"
            Catch ex As Exception
                Return "Java not found"
            End Try
        End Function

        ' MISC
        Public Shared Async Function IsDpiScalingSetAsync(exePath As String) As Task(Of Boolean)
            Return Await Task.Run(Function()
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
                                  End Function)
        End Function
        Public Shared Async Function SetDpiScalingAsync(exePath As String) As Task
            Await Task.Run(Sub()
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

                                   logger.Logger.LogInfo($"Set DPIAware: {exePath}")
                               Catch ex As Exception
                                   logger.Logger.LogInfo($"Failed to Set DPIAware: {exePath}")
                                   MessageBox.Show("Error modifying registry: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                               End Try
                           End Sub)
        End Function
        Public Shared Async Function ImportRegFileAsync(filePath As String) As Task
            If IO.File.Exists(filePath) Then
                logger.Logger.LogInfo($"Importing Reg: {filePath}")
                Await Task.Run(Sub()
                                   Dim proc As New Process()
                                   proc.StartInfo.FileName = "regedit.exe"
                                   proc.StartInfo.Arguments = "/s """ & filePath & """"
                                   proc.StartInfo.UseShellExecute = True
                                   proc.StartInfo.Verb = "runas" ' Runs as administrator
                                   proc.Start()
                                   proc.WaitForExit()
                               End Sub)
            Else
                logger.Logger.LogInfo($"Failed to Find Reg: {filePath}")
                Throw New IO.FileNotFoundException("Registry file not found: " & filePath)
            End If
        End Function
        Public Shared Async Function IsRunningAsAdminAsync() As Task(Of Boolean)
            Return Await Task.Run(Function()
                                      Try
                                          Dim identity = WindowsIdentity.GetCurrent()
                                          Dim principal = New WindowsPrincipal(identity)
                                          Return principal.IsInRole(WindowsBuiltInRole.Administrator)
                                      Catch ex As Exception
                                          'MessageBox.Show("Error checking privileges: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                          Return False
                                      End Try
                                  End Function)
        End Function
        Public Shared Async Function DeleteLogIfTooLargeAsync(logFilePath As String) As Task
            Await Task.Run(Sub()
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
                           End Sub)
        End Function
        Public Shared Async Function IsInternetAvailableAsync(InputUrl As String, Optional timeout As Integer = 90000) As Task(Of Boolean)
            Try
                Using client As New Net.Http.HttpClient()
                    client.Timeout = TimeSpan.FromMilliseconds(timeout)

                    Dim response As Net.Http.HttpResponseMessage = Await client.GetAsync(InputUrl)
                    If response.IsSuccessStatusCode Then
                        Return True
                    Else
                        My.logger.Logger.LogWarning($"Internet check failed: Status code {(CInt(response.StatusCode))} from {InputUrl}")
                        Return False
                    End If
                End Using
            Catch ex As Exception
                My.logger.Logger.LogError($"Internet check exception for {InputUrl}: {ex.Message}")
                Return False
            End Try
        End Function
        Public Shared Sub ShowSnackBar(InputString As String)
            Dim snackBar As New ReaLTaiizor.Controls.MaterialSnackBar(InputString)
            snackBar.Show(Form1)
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

                ' Read the file lines and choose the right delimiter
                Dim lines As String()
                Dim delimiter As Char
                If JAMFile.EndsWith(".jam", StringComparison.OrdinalIgnoreCase) Then
                    lines = File.ReadAllLines(JAMFile, Encoding.GetEncoding(932))
                    delimiter = "="c
                ElseIf JAMFile.EndsWith(".jad", StringComparison.OrdinalIgnoreCase) Then
                    lines = File.ReadAllLines(JAMFile, Encoding.UTF8)
                    delimiter = ":"c
                ElseIf JAMFile.EndsWith(".swf", StringComparison.OrdinalIgnoreCase) Then
                    'logger.Logger.LogInfo("SWF File unable to generate controls.")
                    Return
                Else
                    logger.Logger.LogInfo($"Uknown File Type ({JAMFile}) unable to generate controls.")
                    MessageBox.Show("Unsupported file type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If

                Dim rowIndex As Integer = 0
                For Each line As String In lines
                    ' Skip empty lines
                    If String.IsNullOrWhiteSpace(line) Then Continue For

                    ' Split by appropriate delimiter
                    Dim parts As String() = line.Split(New Char() {delimiter}, 2)
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

                    ' Increase row count and add a new row style
                    tableLayout.RowCount = rowIndex + 1
                    tableLayout.RowStyles.Add(New RowStyle(SizeType.AutoSize))

                    ' Add the label and textbox to the layout
                    tableLayout.Controls.Add(lbl, 0, rowIndex)
                    tableLayout.Controls.Add(txt, 1, rowIndex)
                    rowIndex += 1
                Next

                ' Add the layout to the container
                container.Controls.Add(tableLayout)
            Catch ex As Exception
                MessageBox.Show($"Error generating controls for JAM/JAD: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        'Get App Icons
        Public Async Function ExtractAndResizeAppIconAsync(jarFilePath As String, jamFilePath As String, SelectedGame As Game) As Task
            Try
                Dim iconOutputFolder As String = "data\tools\icons"
                If Not Directory.Exists(iconOutputFolder) Then
                    Directory.CreateDirectory(iconOutputFolder)
                End If

                If jamFilePath.EndsWith(".swf") Then
                    Return
                End If

                Using archive As ZipArchive = Await Task.Run(Function() ZipFile.OpenRead(jarFilePath))
                    ' Check for JSky emulator
                    If SelectedGame.Emulator?.ToLower() = "jsky" Then
                        ' Look for MIDlet-Icon entry in the JAM file
                        Dim iconLine As String = Await Task.Run(Function()
                                                                    Return File.ReadLines(jamFilePath).
                                                                FirstOrDefault(Function(line) line.StartsWith("MIDlet-Icon:", StringComparison.OrdinalIgnoreCase))
                                                                End Function)

                        If iconLine Is Nothing Then
                            logger.Logger.LogWarning("No MIDlet-Icon entry found in the JAM file.")
                            Return
                        End If

                        Dim iconFileName As String = iconLine.Split(":"c)(1).Trim()
                        Dim pngEntry As ZipArchiveEntry = archive.GetEntry(iconFileName)

                        If pngEntry IsNot Nothing Then
                            Using originalStream As Stream = pngEntry.Open()
                                Dim originalIcon As Image = Await Task.Run(Function() Image.FromStream(originalStream))

                                ' Resize to 36x36
                                Dim resizedIcon As New Bitmap(36, 36)
                                Using graphics As Graphics = Graphics.FromImage(resizedIcon)
                                    graphics.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                                    graphics.DrawImage(originalIcon, 0, 0, 36, 36)
                                End Using

                                ' Save as .gif
                                Dim outputFileName As String = Path.GetFileNameWithoutExtension(jarFilePath) & ".gif"
                                Dim outputFilePath As String = Path.Combine(iconOutputFolder, outputFileName)
                                Await Task.Run(Sub() resizedIcon.Save(outputFilePath, Imaging.ImageFormat.Gif))

                                UpdateListViewItemIcon(SelectedGame.ENTitle, outputFilePath)
                                logger.Logger.LogInfo($"JSky icon extracted and resized: {outputFilePath}")
                            End Using
                            Return
                        Else
                            logger.Logger.LogWarning($"Icon file '{iconFileName}' not found in the archive.")
                            Return
                        End If
                    End If

                    ' Continue with normal DoJa/Star icon extraction
                    Dim appIconLine As String = Await Task.Run(Function()
                                                                   Return File.ReadLines(jamFilePath).
                                                               FirstOrDefault(Function(line) line.StartsWith("AppIcon"))
                                                               End Function)

                    If appIconLine Is Nothing Then
                        logger.Logger.LogError("No AppIcon entry found in the JAM file.")
                        Return
                    End If

                    Dim iconFileNameDoja As String = appIconLine.Split("="c)(1).Split(","c)(0).Trim()
                    Dim iconEntry As ZipArchiveEntry = archive.GetEntry(iconFileNameDoja)

                    If iconEntry IsNot Nothing Then
                        Using originalStream As Stream = iconEntry.Open()
                            Dim originalIcon As Image = Await Task.Run(Function() Image.FromStream(originalStream))

                            Dim resizedIcon As New Bitmap(36, 36)
                            Using graphics As Graphics = Graphics.FromImage(resizedIcon)
                                graphics.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                                graphics.DrawImage(originalIcon, 0, 0, 36, 36)
                            End Using

                            Dim outputFileName As String = Path.GetFileNameWithoutExtension(jarFilePath) & ".gif"
                            Dim outputFilePath As String = Path.Combine(iconOutputFolder, outputFileName)

                            ' Ensure directory exists
                            Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath))

                            ' Delete file if already exists (avoid overwrite error)
                            If File.Exists(outputFilePath) Then
                                File.Delete(outputFilePath)
                            End If

                            Await Task.Run(Sub()
                                               resizedIcon.Save(outputFilePath, Imaging.ImageFormat.Gif)
                                           End Sub)

                            UpdateListViewItemIcon(SelectedGame.ENTitle, outputFilePath)
                            logger.Logger.LogInfo($"Icon successfully extracted and resized: {outputFilePath}")
                        End Using
                    Else
                        logger.Logger.LogInfo($"Icon '{iconFileNameDoja}' not found in the .jar file.")
                    End If
                End Using
            Catch ex As Exception
                logger.Logger.LogError($"Error extracting and resizing the icon: {ex.Message}")
                MessageBox.Show($"Error extracting and resizing the icon: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Function
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

        'Launch App Functions
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
                    Dim dimensions = Await ExtractDOJAWidthHeightAsync(jamPath)
                    Dim width As Integer = dimensions.Item1
                    Dim height As Integer = dimensions.Item2
                    Await UpdatedDOJADrawSize(DOJAPATH, width, height)
                    logger.Logger.LogInfo($"[Launch] Updated DOJA draw size to {width}x{height}")
                End If

                ' Update sound config
                Await UpdateDOJASoundConf(DOJAPATH, Form1.cbxAudioType.SelectedItem.ToString())
                logger.Logger.LogInfo($"[Launch] Updated DOJA sound config to {Form1.cbxAudioType.SelectedItem}")

                ' Update app config and JAM entries
                Dim NoGameJameUpdatedList As List(Of String) = gameManager.NoUpdateJAMGames
                If NoGameJameUpdatedList.Contains(Path.GetFileNameWithoutExtension(GameJAM)) = False Then
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
                End If
                Await UpdateNetworkUIDinJAMAsync(GameJAM)

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
                        Async Function()
                            Return Await WaitForProcessWindowAsync({"doja"}, "WaitForDojaToStart")
                        End Function
                )

                If Not success Then
                    MessageBox.Show("Failed to launch DOJA after retrying.", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If

                ' ShaderGlass launch if enabled
                If Form1.chkbxShaderGlass.Checked Then
                    Await LaunchShaderGlass(Path.GetFileNameWithoutExtension(jamPath))
                    logger.Logger.LogInfo("[ShaderGlass] ShaderGlass launched and monitoring started.")
                End If

                If Form1.chkbxEnableController.Checked = True Then
                    Await LaunchControllerProfileAMGP()
                    If Form1.chkboxControllerVibration.Checked = True Then
                        Await StartVibratorBmpMonitorAsync(Path.Combine(DOJAPATH, "lib", "skin", "device1", "vibrator.bmp"))
                    End If
                End If
                ProcessManager.StartMonitoring(jamPath)
            Catch ex As Exception
                logger.Logger.LogError($"[Launch] Exception occurred: {ex}")
                MessageBox.Show($"Failed to launch the game: {ex.Message}", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
        Public Async Sub LaunchCustomDOJA_SJMEGameCommand(DOJASJMEPATH As String, DOJASJMEEXELocation As String, GameJAM As String)
            Try
                ' Validate inputs
                If String.IsNullOrWhiteSpace(DOJASJMEPATH) OrElse String.IsNullOrWhiteSpace(DOJASJMEEXELocation) OrElse String.IsNullOrWhiteSpace(GameJAM) Then
                    Throw New ArgumentException("One or more required parameters are missing.")
                End If

                'Start overlay
                UtilManager.ShowLaunchOverlay(Form1)


                ' Construct all paths
                Dim baseDir As String = AppDomain.CurrentDomain.BaseDirectory
                Dim useLocaleEmulator As Boolean = Form1.chkbxLocalEmulator.Checked
                Dim appPath As String
                Dim arguments As String
                Dim dojaExePath As String = DOJASJMEEXELocation.Trim
                Dim jamPath As String = Path.Combine(baseDir, GameJAM).Trim
                Dim jarPath As String = Path.Combine(baseDir, GameJAM.Substring(0, GameJAM.Length - 4) & ".jar").Trim
                Dim binDir As String = Path.GetDirectoryName(jamPath)
                Dim gameDir As String = Path.GetDirectoryName(binDir)
                Dim gameName As String = Path.GetFileNameWithoutExtension(jamPath)
                Dim spPath As String = Path.Combine(gameDir, "sp", gameName & ".sp").Trim()

                ' Extract AppName from JAM
                Dim AppName As String = ExtractAppNamefromJAM(jamPath)

                ' Warn about long JAM paths
                If jamPath.Length > 240 Then
                    logger.Logger.LogWarning($"[Launch] Potentially long JAM path: {jamPath}")
                    MessageBox.Show("The file path length exceeds 240 characters. You may experience issues running. Try moving Keitai World Emulator to the root of C:/", "Path Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

                ' Determine launch method
                Dim SJMELaunchMethod As String
                Dim SJMEProcessWatch As String
                Dim selectedSJMELaunchOption = Form1.cbxSJMELaunchOption.SelectedItem.ToString
                Dim selectedSJMEScaling = Form1.cbxSJMEScaling.SelectedItem.ToString
                If selectedSJMELaunchOption = "SpringCoat" Then
                    SJMELaunchMethod = "springcoat"
                    SJMEProcessWatch = "squirreljme"
                ElseIf selectedSJMELaunchOption = "Hosted" Then
                    SJMELaunchMethod = "hosted"
                    SJMEProcessWatch = "java"
                End If
                If useLocaleEmulator Then
                    ' Launch using Locale Emulator
                    appPath = Path.Combine(baseDir, "data", "tools", "locale_emulator", "LEProc.exe").Trim()
                    Dim guidArg As String = "-runas ad1a7fe1-4f95-45ba-b563-9ba60c3642d3"
                    arguments = $"{guidArg} ""{dojaExePath}"" -Xemulator:{SJMELaunchMethod} -Xlibraries:""{spPath}"" -jar ""{jarPath}"" -Dcc.squirreljme.scale: {selectedSJMEScaling}"
                    logger.Logger.LogInfo($"[Launch] Launching via Locale Emulator with arguments: {arguments}")
                Else
                    ' Launch directly
                    appPath = dojaExePath
                    arguments = $"-Xemulator:{SJMELaunchMethod} -Xlibraries:""{spPath}"" -jar ""{jarPath}"" -Dcc.squirreljme.scale: {selectedSJMEScaling}"
                    logger.Logger.LogInfo($"[Launch] Launching directly without Locale Emulator: {arguments}")
                End If

                ' Update NetworkURLS in Jam
                Await UpdateNetworkUIDinJAMAsync(GameJAM)

                ' Let filesystem settle (especially important on slower drives)
                Await Task.Delay(500)

                ' Launch SJME with retry logic
                Dim success As Boolean = Await LaunchEmulatorWithRetryandDelay(
                    appPath,
                    arguments,
                    SJMEProcessWatch,
                    baseDir,
                    Async Function()
                        Return Await WaitForProcessWindowAsync({SJMEProcessWatch}, "WaitForSQUIRREKJMEToStart")
                    End Function
                )

                If Not success Then
                    MessageBox.Show("Failed to launch squirreljme after retrying.", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If

                ' ShaderGlass launch if enabled
                If Form1.chkbxShaderGlass.Checked Then
                    Await LaunchShaderGlass(AppName)
                    logger.Logger.LogInfo("[ShaderGlass] ShaderGlass launched and monitoring started.")
                End If

                If Form1.chkbxEnableController.Checked = True Then
                    Await LaunchControllerProfileAMGP()
                    If Form1.chkboxControllerVibration.Checked = True Then
                        MessageBox.Show("Disabling Vibration due to no supported with SquirrelJME")
                        Form1.chkboxControllerVibration.Checked = False
                    End If
                End If
                ProcessManager.StartMonitoring(jamPath)

            Catch ex As Exception
                logger.Logger.LogError($"[Launch] Exception occurred: {ex}")
                MessageBox.Show($"Failed to launch the game: {ex.Message}", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
        Public Async Sub LaunchCustom_KEmulatorGameCommand(KEMUPATH As String, KEMUEXELocation As String, GameJAM As String)
            Try
                logger.Logger.LogInfo("[Launch] Starting KEmulator game launch sequence...")

                ' Validate inputs
                If String.IsNullOrWhiteSpace(KEMUPATH) OrElse String.IsNullOrWhiteSpace(KEMUEXELocation) OrElse String.IsNullOrWhiteSpace(GameJAM) Then
                    Throw New ArgumentException("One or more required parameters are missing.")
                End If

                'Start overlay
                UtilManager.ShowLaunchOverlay(Form1)

                ' Prepare all paths
                Dim baseDir = AppDomain.CurrentDomain.BaseDirectory
                Dim useLocaleEmulator As Boolean = Form1.chkbxLocalEmulator.Checked
                Dim appPath As String
                Dim arguments As String

                ' Make Full Paths
                Dim Java32EXE As String = Path.Combine(Form1.Java1_8BinFolderPath, "java.exe")
                Dim exePath As String = KEMUEXELocation.Trim
                Dim jadjamPath As String = Path.Combine(baseDir, GameJAM).Trim()
                Dim jarPath As String = Path.Combine(Path.GetDirectoryName(jadjamPath), Path.GetFileNameWithoutExtension(jadjamPath) & ".jar")

                If jadjamPath.Length > 240 Then
                    logger.Logger.LogWarning($"[Launch] JAD file path exceeds 240 characters: {jadjamPath}")
                    MessageBox.Show("The file path length exceeds 240 characters. You might experience issues running. Try moving Keitai World Emulator to the root of C:/", "Path Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

                ' Form arguments based on launch method
                appPath = Java32EXE
                arguments = $"{Path.GetFileName(exePath)} ""{jadjamPath}"""
                logger.Logger.LogInfo($"[Launch] Launching Kemulator directly without Locale Emulator: {arguments}")

                ' Config updates / ' Extract AppName from JAM
                Dim AppName As String
                If GameJAM.EndsWith(".jad") Then
                    AppName = GetMidletNameFromJad(jadjamPath)
                    Await UpdateJADJarURLAsync(jadjamPath)
                ElseIf GameJAM.EndsWith(".jam") Then
                    AppName = ExtractAppNamefromJAM(jadjamPath)
                End If

                ' Update KEMULATOR config
                Dim Kemu_PropertyFile = Path.Combine(Path.GetDirectoryName(exePath), "property.txt")
                Await EnsureKEmulatorProperties(Kemu_PropertyFile)

                Await Task.Delay(500) ' Let the filesystem settle

                ' Launch KEMULATOR JAVA with retry logic
                Dim success = Await LaunchKemulatorAppAsync(Java32EXE, KEMUEXELocation, jarPath)
                If Not success Then
                    HideLaunchOverlay()
                    MessageBox.Show("Failed to launch KEmulator after retrying.", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If

                ' ShaderGlass launch if enabled
                If Form1.chkbxShaderGlass.Checked Then
                    logger.Logger.LogInfo("[ShaderGlass] Not Supported for KEmulator, Disabling")
                    Form1.chkbxShaderGlass.Checked = False
                    MessageBox.Show("ShaderGlass is not supported with KEmulator. Please manually resize the KEmulator window by dragging it larger.")
                End If
                If Form1.chkbxEnableController.Checked = True Then
                    Await LaunchControllerProfileAMGP()
                End If
                ProcessManager.StartMonitoring(jadjamPath)
                HideLaunchOverlay()
            Catch ex As ArgumentException
                logger.Logger.LogError($"[Launch] Invalid input: {ex.Message}")
                MessageBox.Show($"Invalid input: {ex.Message}", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)

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
                Await UpdateSTARSoundConf(STARPATH, Form1.cbxAudioType.SelectedItem.ToString())
                logger.Logger.LogInfo($"[Launch] STAR sound config set to {Form1.cbxAudioType.SelectedItem}")
                Await UpdateSTARAppconfig(STARPATH, GameJAM)
                Await EnsureSTARJamFileEntries(GameJAM)
                logger.Logger.LogInfo("[Launch] STAR app configuration and JAM entries updated.")
                Await UpdateNetworkUIDinJAMAsync(GameJAM)

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
                    Async Function()
                        Return Await WaitForProcessWindowAsync({"star"}, "WaitForSTARToStart")
                    End Function
                )

                If Not success Then
                    MessageBox.Show("Failed to launch STAR after retrying.", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If

                ' ShaderGlass launch if enabled
                If Form1.chkbxShaderGlass.Checked Then
                    Await LaunchShaderGlass(Path.GetFileNameWithoutExtension(jamPath))
                    logger.Logger.LogInfo("[ShaderGlass] ShaderGlass launched and monitoring started.")
                End If

                If Form1.chkbxEnableController.Checked = True Then
                    Await LaunchControllerProfileAMGP()
                    If Form1.chkboxControllerVibration.Checked = True Then
                        Await StartVibratorBmpMonitorAsync(Path.Combine(STARPATH, "lib", "skin", "device1", "vibrator.bmp"))
                    End If
                End If
                ProcessManager.StartMonitoring(jamPath)
            Catch ex As ArgumentException
                logger.Logger.LogError($"[Launch] Invalid input: {ex.Message}")
                MessageBox.Show($"Invalid input: {ex.Message}", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)

            Catch ex As Exception
                logger.Logger.LogError($"[Launch] Exception occurred: {ex}")
                MessageBox.Show($"Failed to launch the game: {ex.Message}", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
        Public Async Sub LaunchCustomJSKYGameCommand(JSKYPATH As String, JSKYEXELocation As String, GameJAM As String)
            Try
                logger.Logger.LogInfo("[Launch] Starting JSKY game launch sequence...")

                ' Validate inputs
                If String.IsNullOrWhiteSpace(JSKYPATH) OrElse String.IsNullOrWhiteSpace(JSKYEXELocation) OrElse String.IsNullOrWhiteSpace(GameJAM) Then
                    Throw New ArgumentException("One or more required parameters are missing.")
                End If

                'Start overlay
                UtilManager.ShowLaunchOverlay(Form1)

                ' Prepare all paths
                Dim baseDir = AppDomain.CurrentDomain.BaseDirectory
                Dim useLocaleEmulator As Boolean = Form1.chkbxLocalEmulator.Checked
                Dim appPath As String
                Dim arguments As String

                ' Make Full Paths
                Dim Java32EXE As String = Path.Combine(Form1.Java1_8BinFolderPath, "java.exe")
                Dim exePath As String = JSKYEXELocation.Trim
                Dim jadPath As String = Path.Combine(baseDir, GameJAM).Trim()

                If jadPath.Length > 240 Then
                    logger.Logger.LogWarning($"[Launch] JAD file path exceeds 240 characters: {jadPath}")
                    MessageBox.Show("The file path length exceeds 240 characters. You might experience issues running. Try moving Keitai World Emulator to the root of C:/", "Path Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

                ' Form arguments based on launch method
                appPath = Java32EXE
                arguments = $"{Path.GetFileName(exePath)} ""{jadPath}"""
                logger.Logger.LogInfo($"[Launch] Launching JSKY directly without Locale Emulator: {arguments}")

                ' Config updates
                Await UpdateJADJarURLAsync(jadPath)

                Await Task.Delay(500) ' Let the filesystem settle

                ' Launch JSKY JAVA with retry logic
                Dim success = Await LaunchJSkyAppAsync(Java32EXE, JSKYEXELocation, jadPath)
                If Not success Then
                    HideLaunchOverlay()
                    MessageBox.Show("Failed to launch JSKY after retrying.", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If

                ' ShaderGlass launch if enabled
                If Form1.chkbxShaderGlass.Checked Then
                    logger.Logger.LogInfo("[ShaderGlass] Waiting for JSKY to become idle...")
                    If Await WaitForProcessWindowAsync({"java"}, "WaitForJavaToStart") Then
                        Await LaunchShaderGlass("J-SKY Application Emulator")
                        logger.Logger.LogInfo("[ShaderGlass] ShaderGlass launched and monitoring started.")
                    Else
                        logger.Logger.LogError("[ShaderGlass] Failed to detect JSKY running.")
                        MessageBox.Show("Failed to detect JSKY running.", "ShaderGlass Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                End If

                If Form1.chkbxEnableController.Checked = True Then
                    Await LaunchControllerProfileAMGP()
                End If
                ProcessManager.StartMonitoring(jadPath)
                HideLaunchOverlay()
            Catch ex As ArgumentException
                logger.Logger.LogError($"[Launch] Invalid input: {ex.Message}")
                MessageBox.Show($"Invalid input: {ex.Message}", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)

            Catch ex As Exception
                logger.Logger.LogError($"[Launch] Exception occurred: {ex}")
                MessageBox.Show($"Failed to launch the game: {ex.Message}", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
        Public Async Sub LaunchCustomVODAFONEGameCommand(VODAFONEPATH As String, VODAFONEEXELocation As String, GameJAM As String)
            Try
                logger.Logger.LogInfo("[Launch] Starting VODAFONE game launch sequence...")

                ' Validate inputs
                If String.IsNullOrWhiteSpace(VODAFONEPATH) OrElse String.IsNullOrWhiteSpace(VODAFONEEXELocation) OrElse String.IsNullOrWhiteSpace(GameJAM) Then
                    Throw New ArgumentException("One or more required parameters are missing.")
                End If

                'Start overlay
                UtilManager.ShowLaunchOverlay(Form1)

                ' Prepare all paths
                Dim baseDir = AppDomain.CurrentDomain.BaseDirectory
                Dim useLocaleEmulator As Boolean = Form1.chkbxLocalEmulator.Checked
                Dim appPath As String
                Dim arguments As String

                ' Make Full Paths
                Dim exePath As String = VODAFONEEXELocation.Trim
                Dim jadPath As String = Path.Combine(baseDir, GameJAM).Trim()

                If jadPath.Length > 240 Then
                    logger.Logger.LogWarning($"[Launch] JAD file path exceeds 240 characters: {jadPath}")
                    MessageBox.Show("The file path length exceeds 240 characters. You might experience issues running. Try moving Keitai World Emulator to the root of C:/", "Path Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

                ' Form arguments based on launch method
                appPath = exePath
                arguments = $"-Xdescriptor:""{jadPath}"""
                logger.Logger.LogInfo($"[Launch] Launching VODAFONE directly without Locale Emulator: {arguments}")

                ' Config updates
                Await UpdateJADJarURLAsync(jadPath)

                Await Task.Delay(500) ' Let the filesystem settle

                ' Launch VODAFONE  with retry logic
                Dim success As Boolean = Await LaunchEmulatorWithRetryandDelay(
                    appPath,
                    arguments,
                    "emulator",
                    baseDir,
                    Async Function()
                        Return Await WaitForProcessWindowAsync({"emulator"}, "WaitForVODAFONEToStart")
                    End Function
                )
                If Not success Then
                    HideLaunchOverlay()
                    MessageBox.Show("Failed to launch VODAFONE after retrying.", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If

                ' ShaderGlass launch if enabled
                If Form1.chkbxShaderGlass.Checked Then
                    Await LaunchShaderGlass("V-appli Emulator")
                    logger.Logger.LogInfo("[ShaderGlass] ShaderGlass launched and monitoring started.")
                End If

                If Form1.chkbxEnableController.Checked = True Then
                    Await LaunchControllerProfileAMGP()
                End If
                ProcessManager.StartMonitoring(jadPath)
                HideLaunchOverlay()
            Catch ex As ArgumentException
                logger.Logger.LogError($"[Launch] Invalid input: {ex.Message}")
                MessageBox.Show($"Invalid input: {ex.Message}", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)

            Catch ex As Exception
                logger.Logger.LogError($"[Launch] Exception occurred: {ex}")
                MessageBox.Show($"Failed to launch the game: {ex.Message}", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
        Public Async Sub LaunchCustomFLASHGameCommand(FLASHPATH As String, FLASHEXELocation As String, GameJAM As String)
            Try
                logger.Logger.LogInfo("[Launch] Starting FLASH game launch sequence...")

                ' Validate inputs
                If String.IsNullOrWhiteSpace(FLASHPATH) OrElse String.IsNullOrWhiteSpace(FLASHEXELocation) OrElse String.IsNullOrWhiteSpace(GameJAM) Then
                    Throw New ArgumentException("One or more required parameters are missing.")
                End If

                'Start overlay
                UtilManager.ShowLaunchOverlay(Form1)

                ' Prepare all paths
                Dim baseDir = AppDomain.CurrentDomain.BaseDirectory
                Dim useLocaleEmulator As Boolean = Form1.chkbxLocalEmulator.Checked
                Dim appPath As String
                Dim arguments As String

                ' Make Full Paths
                Dim exePath As String = FLASHEXELocation.Trim
                Dim swfPath As String = Path.Combine(baseDir, GameJAM).Trim()

                If FLASHPATH.Length > 240 Then
                    logger.Logger.LogWarning($"[Launch] JAD file path exceeds 240 characters: {FLASHPATH}")
                    MessageBox.Show("The file path length exceeds 240 characters. You might experience issues running. Try moving Keitai World Emulator to the root of C:/", "Path Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

                ' Form arguments based on launch method
                appPath = exePath
                arguments = $"""{swfPath}"""
                logger.Logger.LogInfo($"[Launch] Launching FLASH directly without Locale Emulator: {arguments}")

                Await Task.Delay(500) ' Let the filesystem settle

                ' Launch FLASHPLAYER with retry logic
                Dim startInfo As New ProcessStartInfo With {
                    .FileName = appPath,
                    .Arguments = arguments,
                    .UseShellExecute = False,
                    .CreateNoWindow = True,
                    .RedirectStandardError = True,
                    .RedirectStandardOutput = True,
                    .WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                }

                ' Launch FlashPlayer with retry logic
                Dim success As Boolean = Await LaunchEmulatorWithRetry(
                    appPath,
                    arguments,
                    "flashplayer",
                    baseDir,
                        Async Function()
                            Return Await WaitForProcessWindowAsync({"flashplayer"}, "WaitForFlashPlayerToStart")
                        End Function
                )
                If Not success Then
                    HideLaunchOverlay()
                    MessageBox.Show("Failed to launch FLASH after retrying.", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If

                ' ShaderGlass launch if enabled
                If Form1.chkbxShaderGlass.Checked Then
                    Await LaunchShaderGlass("Adobe Flash Player 10")
                    logger.Logger.LogInfo("[ShaderGlass] ShaderGlass launched and monitoring started.")
                End If

                If Form1.chkbxEnableController.Checked = True Then
                    Await LaunchControllerProfileAMGP()
                End If
                ProcessManager.StartMonitoring(GameJAM)
                HideLaunchOverlay()
            Catch ex As ArgumentException
                logger.Logger.LogError($"[Launch] Invalid input: {ex.Message}")
                MessageBox.Show($"Invalid input: {ex.Message}", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)

            Catch ex As Exception
                logger.Logger.LogError($"[Launch] Exception occurred: {ex}")
                MessageBox.Show($"Failed to launch the game: {ex.Message}", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
        Public Sub LaunchCustomMachiCharaCommand(MachiCharaEXE As String, CFDFile As String)
            Try
                ' Build full paths
                Dim LocalEmulatorPath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data\tools\locale_emulator\LEProc.exe")
                Dim guidArg As String = "-runas ad1a7fe1-4f95-45ba-b563-9ba60c3642d3"
                Dim machicharaexePath As String = MachiCharaEXE
                Dim CFDPath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CFDFile)

                Dim processInfo As New ProcessStartInfo()

                If Form1.chkboxMachiCharaLocalEmulator.Checked = True Then
                    ' Launch using Locale Emulator
                    processInfo.FileName = LocalEmulatorPath
                    processInfo.Arguments = $"{guidArg} ""{machicharaexePath}"" ""{CFDPath}"""
                Else
                    ' Launch directly
                    processInfo.FileName = machicharaexePath
                    processInfo.Arguments = $"""{CFDPath}"""
                End If

                ' Common settings
                processInfo.UseShellExecute = False
                processInfo.CreateNoWindow = True
                processInfo.RedirectStandardOutput = True
                processInfo.RedirectStandardError = True
                processInfo.WorkingDirectory = Path.GetDirectoryName(machicharaexePath)

                ' Start the process
                Process.Start(processInfo)

            Catch ex As Exception
                MessageBox.Show($"Failed to launch the command: {ex.Message}", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
        Public Sub LaunchCustomCharaDenCommand(CharaDenEXE As String, AFDFile As String)
            Try
                ' Build full paths
                Dim LocalEmulatorPath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data\tools\locale_emulator\LEProc.exe")
                Dim guidArg As String = "-runas ad1a7fe1-4f95-45ba-b563-9ba60c3642d3"
                Dim charadenexePath As String = CharaDenEXE
                Dim AFDPath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AFDFile)

                Dim processInfo As New ProcessStartInfo()

                If Form1.chkboxCharadenLocalEmulator.Checked = True Then
                    ' Launch using Locale Emulator
                    processInfo.FileName = LocalEmulatorPath
                    processInfo.Arguments = $"{guidArg} ""{charadenexePath}"" ""{AFDPath}"""
                Else
                    ' Launch directly
                    processInfo.FileName = charadenexePath
                    processInfo.Arguments = $"""{AFDPath}"""
                End If

                ' Common settings
                processInfo.UseShellExecute = False
                processInfo.CreateNoWindow = True
                processInfo.RedirectStandardOutput = True
                processInfo.RedirectStandardError = True
                processInfo.WorkingDirectory = Path.GetDirectoryName(charadenexePath)

                ' Start the process
                Process.Start(processInfo)

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
        Public Async Function LaunchControllerProfileAMGP() As Task
            Dim selectedProfile As String = TryCast(Form1.cbxControllerProfile.SelectedItem, String)
            Dim selectedController As String = TryCast(Form1.cbxGameControllers.SelectedItem, String)

            If String.IsNullOrEmpty(selectedProfile) OrElse String.IsNullOrEmpty(selectedController) Then
                Return
            End If

            Dim controllerIndex As Integer = Form1.cbxGameControllers.SelectedIndex

            Dim baseDir As String = AppDomain.CurrentDomain.BaseDirectory
            Dim appPath As String = Path.Combine(baseDir, "data", "tools", "antimicrox", "bin", "antimicrox.exe")
            Dim argumentFile As String = Path.Combine(baseDir, "data", "tools", "controller-profiles", selectedProfile & ".gamecontroller.amgp")

            If Not File.Exists(appPath) Then
                logger.Logger.LogInfo("AntimicroX executable not found: " & appPath)
                Return
            End If

            If Not File.Exists(argumentFile) Then
                logger.Logger.LogInfo("Profile file not found: " & argumentFile)
                Return
            End If

            Dim startInfo As New ProcessStartInfo() With {
        .FileName = appPath,
        .Arguments = $"--profile ""{argumentFile}"" --profile-controller {controllerIndex} --hidden",
        .UseShellExecute = True,
        .WorkingDirectory = baseDir
    }

            logger.Logger.LogInfo($"Launching AntimicroX with profile '{selectedProfile}' and controller index {controllerIndex}")
            Try
                Process.Start(startInfo)
            Catch ex As Exception
                MessageBox.Show("Failed to launch AntimicroX: " & ex.Message, "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                logger.Logger.LogInfo("Failed to launch AntimicroX: " & ex.Message)
            End Try
        End Function
        Public Async Function LaunchJSkyAppAsync(javapath As String, JskyExePath As String, jadPath As String) As Task(Of Boolean)
            Try
                ' Proper quoting for cmd.exe
                Dim arguments As String = $"/C """"{javapath}"" -jar ""{Path.GetFileName(JskyExePath)}"" ""{jadPath}"""""

                Dim psi As New ProcessStartInfo("cmd.exe", arguments) With {
                    .UseShellExecute = True,
                    .WorkingDirectory = Path.GetDirectoryName(JskyExePath),
                    .CreateNoWindow = True
                }

                Dim process As Process = Process.Start(psi)

                ' Optional delay to allow Java process to spawn
                Await Task.Delay(1000)

                ' Check for any java process
                Dim javaRunning As Boolean = Process.GetProcessesByName("java").Any()

                Return javaRunning

            Catch ex As Exception
                logger.Logger.LogError($"[JavaLaunch] Failed to start Java app: {ex.Message}")
                Return False
            End Try
        End Function
        Public Async Function LaunchKemulatorAppAsync(javapath As String, KemulatorExePath As String, jarPath As String) As Task(Of Boolean)
            Try
                ' Proper quoting for cmd.exe
                Dim arguments As String = $"/C """"{javapath}"" -jar ""{Path.GetFileName(KemulatorExePath)}"" -jar ""{jarPath}"""""

                Dim psi As New ProcessStartInfo("cmd.exe", arguments) With {
                    .UseShellExecute = False,
                    .WorkingDirectory = Path.GetDirectoryName(KemulatorExePath)
                }

                Dim process As Process = Process.Start(psi)

                ' Optional delay to allow Java process to spawn
                Await Task.Delay(1000)

                ' Check for any java process
                Dim javaRunning As Boolean = Process.GetProcessesByName("java").Any()

                Return javaRunning

            Catch ex As Exception
                logger.Logger.LogError($"[JavaLaunch] Failed to start Java app: {ex.Message}")
                Return False
            End Try
        End Function

        'Launch App Helpers
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
                    Await Task.Delay(500)
                    If process Is Nothing Then
                        logger.Logger.LogError($"[LaunchHelper] Attempt {attempt} Failed to start process.")
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
                        logger.Logger.LogInfo($"[LaunchHelper] {processNameToCheck} Is running And ready.")
                        UtilManager.HideLaunchOverlay()
                        Return True
                    Else
                        logger.Logger.LogWarning($"[LaunchHelper] {processNameToCheck} failed to become ready on attempt {attempt}. Retrying...")
                        process.Kill()
                        process.Dispose()
                        Await Task.Delay(500)
                    End If
                Catch ex As Exception
                    logger.Logger.LogError($"[LaunchHelper] Exception during attempt {attempt} {ex.Message}")
                End Try
            Next

            logger.Logger.LogError($"[LaunchHelper] Failed to start {processNameToCheck} after 2 attempts.")
            UtilManager.HideLaunchOverlay()
            Return False
        End Function
        Public Async Function LaunchEmulatorWithRetryandDelay(
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
            .RedirectStandardInput = True,
            .RedirectStandardError = True,
            .RedirectStandardOutput = True,
            .CreateNoWindow = True,
            .WorkingDirectory = workingDir
        }

            For attempt = 1 To 2
                Try
                    Dim process As Process = Process.Start(startInfo)
                    Await Task.Delay(1500)
                    If process Is Nothing Then
                        logger.Logger.LogError($"[LaunchHelper] Attempt {attempt} Failed to start process.")
                        Continue For
                    End If

                    If Await waitFunction() Then
                        logger.Logger.LogInfo($"[LaunchHelper] {processNameToCheck} Is running And ready.")
                        UtilManager.HideLaunchOverlay()
                        Return True
                    Else
                        logger.Logger.LogWarning($"[LaunchHelper] {processNameToCheck} failed to become ready on attempt {attempt}. Retrying...")
                        process.Kill()
                        process.Dispose()
                        Await Task.Delay(500)
                    End If
                Catch ex As Exception
                    logger.Logger.LogError($"[LaunchHelper] Exception during attempt {attempt} {ex.Message}")
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

        'Check and Close Functions
        Shared Function CheckAndCloseAllEmulators() As Boolean
            Dim emulatorProcesses As New Dictionary(Of String, String()) From {
                {"DOCO Emulator", {"doja"}},
                {"SquirrelJME", {"squirreljme"}},
                {"Star Emulator", {"star"}},
                {"Java-based runtime", {"java", "javaw", "jbime"}},
                {"Vodafone Emulator", {"emulator"}},
                {"Flashplayer", {"flashplayer"}}
            }

            Dim currentPid = Process.GetCurrentProcess().Id
            Dim anyStillRunning As Boolean = False

            For Each kvp In emulatorProcesses
                Dim friendlyName = kvp.Key
                Dim processNames = kvp.Value
                Dim foundProcesses As New List(Of Process)

                For Each name In processNames
                    foundProcesses.AddRange(Process.GetProcessesByName(name))
                Next

                If foundProcesses.Count = 0 Then
                    logger.Logger.LogInfo($"{friendlyName} is not currently running.")
                    Continue For
                End If

                logger.Logger.LogWarning($"Found {foundProcesses.Count} instance(s) of {friendlyName} running.")
                Dim result = MessageBox.Show($"{friendlyName} is currently running. Do you want to close it?",
                                     "Confirm Close",
                                     MessageBoxButtons.YesNo,
                                     MessageBoxIcon.Question)

                If result = DialogResult.Yes Then
                    Try
                        logger.Logger.LogInfo($"User agreed to close {friendlyName}.")
                        CheckAndCloseShaderGlass()
                        CheckAndCloseAMX()

                        Dim closedCount As Integer = 0

                        For Each proc In foundProcesses
                            If proc.Id = currentPid Then
                                logger.Logger.LogInfo($"Skipping self (PID={proc.Id})")
                                Continue For
                            End If

                            Try
                                logger.Logger.LogInfo($"Closing PID={proc.Id} Name={proc.ProcessName}")
                                proc.Kill()
                                proc.WaitForExit()
                                closedCount += 1
                            Catch ex As Exception
                                logger.Logger.LogError($"Failed to close {proc.ProcessName} (PID={proc.Id}): {ex.Message}")
                                anyStillRunning = True
                            End Try
                        Next

                        logger.Logger.LogInfo($"Closed {closedCount} of {foundProcesses.Count} {friendlyName} process(es).")
                    Catch ex As Exception
                        logger.Logger.LogError($"Error closing {friendlyName}: {ex}")
                        MessageBox.Show($"An error occurred while closing {friendlyName}: {ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        anyStillRunning = True
                    End Try
                Else
                    logger.Logger.LogInfo($"User chose not to close {friendlyName}.")
                    anyStillRunning = True
                End If
            Next

            Return anyStillRunning
        End Function
        Shared Function CheckAndCloseAMX() As Boolean
            Dim AMXProcesses = Process.GetProcessesByName("antimicrox")
            If AMXProcesses.Length = 0 Then
                logger.Logger.LogInfo("antimicrox.exe is not currently running.")
                Return False
            End If

            logger.Logger.LogWarning($"Found {AMXProcesses.Length} instance(s) of antimicrox.exe running.")
            Try
                For Each process As Process In AMXProcesses
                    logger.Logger.LogInfo($"Attempting to close process PID={process.Id} Name={process.ProcessName}")
                    process.Kill()
                    process.WaitForExit()
                Next

                logger.Logger.LogInfo("All antimicrox.exe processes closed successfully.")
                Return False ' No longer running
            Catch ex As Exception
                logger.Logger.LogError("Error while closing antimicrox.exe: " & ex.ToString())
                MessageBox.Show("An error occurred while trying to close antimicrox.exe: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
                Return True ' Still running
            End Try
        End Function
        Shared Function CheckAndCloseAHK() As Boolean
            Dim AHKProcesses = Process.GetProcessesByName("AutoHotkey32")
            If AHKProcesses.Length = 0 Then
                logger.Logger.LogInfo("AutoHotkey32.exe is not currently running.")
                Return False
            End If

            logger.Logger.LogWarning($"Found {AHKProcesses.Length} instance(s) of AutoHotkey32.exe running.")
            Try
                For Each process As Process In AHKProcesses
                    logger.Logger.LogInfo($"Attempting to close process PID={process.Id} Name={process.ProcessName}")
                    process.Kill()
                    process.WaitForExit()
                Next

                logger.Logger.LogInfo("All AutoHotkey32.exe processes closed successfully.")
                Return False ' No longer running
            Catch ex As Exception
                logger.Logger.LogError("Error while closing AutoHotkey32.exe: " & ex.ToString())
                MessageBox.Show("An error occurred while trying to close AutoHotkey32.exe: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
                Return True ' Still running
            End Try
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

        'Asynchronous method to wait for the emulator process's to start
        Private Async Function WaitForProcessWindowAsync(processNames As IEnumerable(Of String), tag As String, Optional timeoutMilliseconds As Integer = 4000) As Task(Of Boolean)
            Dim startTime As DateTime = DateTime.Now

            While (DateTime.Now - startTime).TotalMilliseconds < timeoutMilliseconds
                For Each name In processNames
                    Dim procs As Process() = Process.GetProcessesByName(name)
                    For Each proc In procs
                        If proc.MainWindowHandle <> IntPtr.Zero Then
                            logger.Logger.LogInfo($"[{tag}] '{name}' process ready with MainWindowHandle.")
                            Return True
                        End If
                    Next
                Next

                Await Task.Delay(500)
            End While

            logger.Logger.LogError($"[{tag}] Timed out waiting for {String.Join("/", processNames)} window.")
            Return False
        End Function

        'Iappli Helpers
        Public Shared Async Function UpdateNetworkUIDinJAMAsync(JamFile As String) As Task(Of Boolean)
            If Not File.Exists(JamFile) Then
                logger.Logger.LogError($"ERROR Did Not find {JamFile} to update networkUID")
                Return False
            End If

            If Form1.NetworkUID.ToLower.Trim = "nullgwdocomo" Then
                logger.Logger.LogInfo("Skipping update to NetworkUID in Jam due to it Not being set (still NULLGWDOCOMO).")
                Return True
            End If

            Dim originalLines As String() = Await File.ReadAllLinesAsync(JamFile)
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

            Await File.WriteAllLinesAsync(JamFile, lines)
            logger.Logger.LogInfo($"Successfully updated {JamFile}")
            Return True
        End Function

        'DOJA Helpers
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
        Public Async Function ExtractDOJAWidthHeightAsync(filePath As String) As Task(Of (Integer, Integer))
            Return Await Task.Run(Function()
                                      Dim width As Integer = 240
                                      Dim height As Integer = 240

                                      Try
                                          For Each line As String In File.ReadLines(filePath, Encoding.GetEncoding("shift-jis"))
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
                                          Console.WriteLine($"Error reading width and height: {ex.Message}")
                                      End Try

                                      Return (width, height)
                                  End Function)
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
        Public Function ExtractAppNamefromJAM(GAMEJAM As String) As String
            ' 1) Make sure the file exists
            If Not File.Exists(GAMEJAM) Then
                Throw New FileNotFoundException($"Could not find .jam file at '{GAMEJAM}'.", GAMEJAM)
            End If

            ' 2) Load all lines with Shift‑JIS encoding
            Dim sjis As Encoding = Encoding.GetEncoding("shift_jis")
            Dim lines As String() = File.ReadAllLines(GAMEJAM, sjis)

            ' 3) Look for the AppName line
            Dim rx As New Regex("^\s*AppName\s*=\s*(.+)$", RegexOptions.IgnoreCase)
            For Each line As String In lines
                Dim m As Match = rx.Match(line)
                If m.Success Then
                    ' Group(1) is everything after the '='
                    Return m.Groups(1).Value.Trim()
                End If
            Next

            ' 4) If no AppName= line was found
            Return String.Empty
        End Function

        'STAR Helpers
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
        Public Function ExtractSTARWidthHeight(filePath As String) As (Integer, Integer)
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

            Dim enc = Encoding.GetEncoding(932)
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

        'JSKY Helpers
        Public Shared Async Function UpdateJADJarURLAsync(jadFilePath As String) As Task(Of Boolean)
            If Not File.Exists(jadFilePath) Then
                logger.Logger.LogError($"JAD file not found: {jadFilePath}")
                Return False
            End If

            Try
                Dim lines As String() = Await File.ReadAllLinesAsync(jadFilePath)
                Dim updatedLines As New List(Of String)
                Dim updatedJarUrl As Boolean = False
                Dim updatedDescription As Boolean = False
                Dim jarFileName As String = Path.GetFileNameWithoutExtension(jadFilePath) & ".jar"

                For Each line In lines
                    If line.StartsWith("MIDlet-Jar-URL", StringComparison.OrdinalIgnoreCase) Then
                        updatedLines.Add($"MIDlet-Jar-URL: {jarFileName}")
                        updatedJarUrl = True
                    ElseIf line.StartsWith("MIDlet-Description", StringComparison.OrdinalIgnoreCase) Then
                        updatedLines.Add("MIDlet-Description:")
                        updatedDescription = True
                    Else
                        updatedLines.Add(line)
                    End If
                Next

                If updatedJarUrl OrElse updatedDescription Then
                    Await File.WriteAllLinesAsync(jadFilePath, updatedLines)
                    logger.Logger.LogInfo($"Updated JAD file: MIDlet-Jar-URL set to {jarFileName}, MIDlet-Description cleared in {jadFilePath}")
                    Return True
                Else
                    logger.Logger.LogWarning($"No changes made to {jadFilePath}. Required entries not found.")
                    Return False
                End If
            Catch ex As Exception
                logger.Logger.LogError($"Failed to update JAD file: {ex.Message}")
                Return False
            End Try
        End Function

        'KEmulator Helpers
        Public Shared Function GetMidletNameFromJad(jadFilePath As String) As String
            Try
                Dim lines = File.ReadAllLines(jadFilePath, Encoding.UTF8)

                ' Search for the MIDlet-Name line
                For Each line In lines
                    If line.StartsWith("MIDlet-Name:", StringComparison.OrdinalIgnoreCase) Then
                        Return line.Substring("MIDlet-Name:".Length).Trim()
                    End If
                Next

                Return Nothing ' Not found
            Catch ex As Exception
                MessageBox.Show($"Error reading JAD file: {ex.Message}", "Read Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return Nothing
            End Try
        End Function
        Public Shared Function ExtractKEmulatorVersion(logFilePath As String) As String
            Try
                If Not File.Exists(logFilePath) Then Return Nothing

                ' Open with read-share in case another process is writing
                Using fs As New FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                    Using reader As New StreamReader(fs)
                        While Not reader.EndOfStream
                            Dim line As String = reader.ReadLine()

                            If line.Contains("KEmulator") Then
                                ' Split the line by space and find the first part that starts with "v"
                                Dim parts = line.Split(" "c)
                                For Each part In parts
                                    If part.StartsWith("v") Then
                                        Return part.Trim()
                                    End If
                                Next
                            End If
                        End While
                    End Using
                End Using

                Return Nothing ' Version not found
            Catch ex As Exception
                MessageBox.Show($"Error reading log file: {ex.Message}", "Read Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return Nothing
            End Try
        End Function
        Public Shared Async Function EnsureKEmulatorProperties(propertyFilePath As String) As Task(Of Boolean)
            Dim lines As New List(Of String)(File.ReadAllLines(propertyFilePath, Encoding.GetEncoding("shift_jis")))
            Dim updatedLines As New List(Of String)
            Dim defaultFontSet As Boolean = False
            Dim fileEncodingSet As Boolean = False

            For Each line In lines
                If line.StartsWith("DefaultFont=", StringComparison.OrdinalIgnoreCase) Then
                    updatedLines.Add("DefaultFont=Meiryo UI")
                    defaultFontSet = True
                ElseIf line.StartsWith("FileEncoding=", StringComparison.OrdinalIgnoreCase) Then
                    updatedLines.Add("FileEncoding=Shift_JIS")
                    fileEncodingSet = True
                Else
                    updatedLines.Add(line)
                End If
            Next

            ' Add the entries if they weren't found
            If Not defaultFontSet Then updatedLines.Add("DefaultFont=Meiryo UI")
            If Not fileEncodingSet Then updatedLines.Add("FileEncoding=Shift_JIS")

            File.WriteAllLines(propertyFilePath, updatedLines, Encoding.GetEncoding("shift_jis"))
            Return True
        End Function

        'ShaderGlass Helpers
        Public Async Function ModifyCaptureWindow(filePath As String, AppName As String) As Task
            If Not File.Exists(filePath) Then
                Console.WriteLine($"ShaderGlass config file Not found {filePath}")
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
        Public Async Function ModifySelectedShader(filePath As String, ShaderName As String) As Task
            If Not File.Exists(filePath) Then
                Console.WriteLine($"ShaderGlass config file Not found {filePath}")
                Return
            End If

            Dim lines As List(Of String) = (Await File.ReadAllLinesAsync(filePath)).ToList()

            For i As Integer = 0 To lines.Count - 1
                If lines(i).StartsWith("ShaderName") Then
                    lines(i) = $"ShaderName ""{ShaderName.ToLower}"""
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
                Console.WriteLine($"ShaderGlass config file Not found {filePath}")
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

        'AMX - Vibration Functions
        Private _cancellationSource As CancellationTokenSource
        Private _lastAccessTime As DateTime
        Private _vibrationLock As New Object()
        Private _vibrating As Boolean = False
        Public Async Function StartVibratorBmpMonitorAsync(bmpPath As String) As Task
            _lastAccessTime = File.GetLastAccessTime(bmpPath)
            _cancellationSource = New CancellationTokenSource()
            Dim token = _cancellationSource.Token

            Try
                While Not token.IsCancellationRequested
                    Await Task.Delay(100, token)

                    Dim currentAccess = File.GetLastAccessTime(bmpPath)
                    If currentAccess > _lastAccessTime Then
                        _lastAccessTime = currentAccess
                        Await TriggerVibrationAsync(token)
                    End If
                End While
            Catch ex As TaskCanceledException
                ' Expected on cancel
            End Try
        End Function
        Public Sub StopVibratorBmpMonitor()
            Try
                _cancellationSource?.Cancel()
            Catch ex As ObjectDisposedException
                ' Already disposed
            End Try

            _cancellationSource = Nothing

            ' Force stop any vibration
            Try
                Dim controller As New Controller(UserIndex.One)
                If controller.IsConnected Then
                    controller.SetVibration(New Vibration())
                End If
            Catch
                ' Ignore controller errors on shutdown
            End Try
        End Sub
        Public Async Function TriggerVibrationAsync(token As CancellationToken) As Task
            SyncLock _vibrationLock
                If _vibrating Then Exit Function ' Skip if already vibrating
                _vibrating = True
            End SyncLock

            Try
                Dim controller As New Controller(UserIndex.One)
                If controller.IsConnected Then
                    controller.SetVibration(New Vibration With {
                .LeftMotorSpeed = 65535,
                .RightMotorSpeed = 65535
            })

                    Await Task.Delay(250, token) ' Respect cancellation
                    controller.SetVibration(New Vibration())
                Else
                    Console.WriteLine("XInput controller not connected.")
                End If
            Catch ex As TaskCanceledException
                ' If cancelled mid-vibration, stop the motor
                Dim controller As New Controller(UserIndex.One)
                If controller.IsConnected Then
                    controller.SetVibration(New Vibration())
                End If
            Finally
                SyncLock _vibrationLock
                    _vibrating = False
                End SyncLock
            End Try
        End Function


    End Class
End Namespace