Imports System.IO
Imports System.IO.Compression
Imports System.Net
Imports System.Net.Http
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Security.Principal
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports KeitaiWorldLauncher.My.logger
Imports KeitaiWorldLauncher.My.Models
Imports Microsoft.Win32
Imports SharpDX.XInput

Namespace My.Managers
    Public Class UtilManager
        Private Shared LaunchOverlay As Panel = Nothing
        Private Shared LaunchOverlayLabel As Label = Nothing
        Dim gameManager As New GameManager()
        Private Shared _appliEditWarningShown As Boolean = False

        'PreReq Check
        Public Shared Async Function CheckforPreReqAsync() As Task(Of Boolean)
            ' Check for administrator privileges before continuing
            If Not Await IsRunningAsAdminAsync() Then
                MessageBox.Show(owner:=SplashScreen, "For the first-time setup, this application requires administrator privileges to configure necessary settings." & vbCrLf & vbCrLf &
                        "Please restart the application as an Administrator by right-clicking the executable and selecting 'Run as administrator'.",
                        "Administrator Privileges Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainForm.QuitApplication()
                Return False
            End If

            ' === Begin pre-req checks ===
            Dim baseDir As String = AppDomain.CurrentDomain.BaseDirectory
            Dim DOJAEmulator = MainForm.DOJAEXE
            Dim StarEmulator = MainForm.STAREXE
            Dim localeEmuLoc As String = Path.Combine(baseDir, "data", "tools", "locale_emulator", "LEProc.exe")
            Dim shaderGlassLoc As String = Path.Combine(baseDir, "data", "tools", "shaderglass", "ShaderGlass.exe")


            ' Setup DOJA and STAR Reg
            Dim toolDojaDirs = Directory.GetDirectories(MainForm.ToolsFolder, "idkDoja*")
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

            Dim toolStarDirs = Directory.GetDirectories(MainForm.ToolsFolder, "idkStar*")
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

            ' Check for Locale Emulator
            My.logger.Logger.LogInfo("Checking for LEPROC")
            If Not File.Exists(localeEmuLoc) Then
                MessageBox.Show(owner:=SplashScreen, $"Missing Locale Emulator... Download is required{vbCrLf}LocaleEmu Files need to be located at {localeEmuLoc}")
                My.logger.Logger.LogInfo("Missing Locale Emulator")
                Await OpenURLAsync("https://github.com/xupefei/Locale-Emulator/releases")
                MainForm.QuitApplication()
                Return False
            End If

            ' Check for ShaderGlass
            My.logger.Logger.LogInfo("Checking for ShaderGlass")
            If Not File.Exists(shaderGlassLoc) Then
                MessageBox.Show(owner:=SplashScreen, $"Missing ShaderGlass... Download is required{vbCrLf}ShaderGlass Files need to be located at {shaderGlassLoc}")
                My.logger.Logger.LogInfo("Missing ShaderGlass")
                Await OpenURLAsync("https://github.com/mausimus/ShaderGlass/releases")
                MainForm.QuitApplication()
                Return False
            End If

            ' Check for Java 1.8
            My.logger.Logger.LogInfo("Checking for Java 8")
            If Not Await EnsureJava1_8IsConfiguredAsync() Then
                MessageBox.Show(owner:=SplashScreen, "Missing JAVA 8 (JDK8u152)... Download is required")
                My.logger.Logger.LogInfo("Missing JAVA 8")
                MainForm.QuitApplication()
                Return False
            End If

            ' Check for Java 21+
            My.logger.Logger.LogInfo("Checking for Java 21+")
            If Not Await DetectJava21PlusAsync() Then
                Dim result = MessageBox.Show(owner:=SplashScreen, text:="Java 21+ is required for OpenDoja features." & Environment.NewLine & "Click OK to open the download page.", caption:="Java 21+ Required", buttons:=MessageBoxButtons.OKCancel, icon:=MessageBoxIcon.Warning)
                My.logger.Logger.LogInfo("Missing Java 21+")
                If result = DialogResult.OK Then
                    Await OpenURLAsync("https://adoptium.net/temurin/releases/?os=windows&arch=x64&package=jdk&version=26&mode=filter")
                End If
            End If

            ' Check for Visual C++ Runtimes
            My.logger.Logger.LogInfo("Checking for C++ Runtimes")
            If Not Await IsVCRuntime2022InstalledAsync() Then
                MessageBox.Show(owner:=SplashScreen, "Unable to Detect C++ Runtimes... To ensure compatibility, we recommend you install this Runtime AIO Package. A REBOOT might be required after installing.")
                My.logger.Logger.LogInfo("Missing C++ Runtimes")
                Await OpenURLAsync("https://www.techpowerup.com/download/visual-c-redistributable-runtime-package-all-in-one/")
                MainForm.QuitApplication()
                Return False
            End If

            ' Check for .net 8 Runtime
            My.logger.Logger.LogInfo("Checking for .Net 8.0.15+ Runtimes")
            If Not Await IsDotNet8Installed() Then
                MessageBox.Show(owner:=SplashScreen, "Unable to Detect .Net 8.0.15+ Runtimes... Download is required. A REBOOT might be required after installing.")
                My.logger.Logger.LogInfo("Missing .Net 8.0.15+ Runtimes")
                Await OpenURLAsync("https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-8.0.15-windows-x86-installer?cid=getdotnetcore")
                MainForm.QuitApplication()
                Return False
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
                    "Java Compatibility Issue",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning)
                    End If
                End Using
            Next

            ' Step 1: Locate JDK 1.8 first
            Dim jdkPath As String = Await Task.Run(Function()
                                                       Dim jdkKeys As String() = {
                                                   "SOFTWARE\JavaSoft\Java Development Kit\1.8",
                                                   "SOFTWARE\WOW6432Node\JavaSoft\Java Development Kit\1.8"
                                               }

                                                       For Each regPath In jdkKeys
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

            Dim javaPath As String = Nothing

            If Not String.IsNullOrEmpty(jdkPath) Then
                ' Found JDK
                MainForm.UsingJDK1_8 = True
                javaPath = jdkPath
                My.logger.Logger.LogInfo($"Found JDK 1.8 at: {jdkPath}")
            Else
                ' Fall back to JRE
                Dim jrePath As String = Await Task.Run(Function()
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

                If String.IsNullOrEmpty(jrePath) Then
                    Dim result = MessageBox.Show(
                        owner:=SplashScreen,
                        text:="Missing Java 8 (JDK or JRE v1.8). A Java 8 installation is required." & Environment.NewLine &
                              "JDK 8u152 x86 is recommended." & Environment.NewLine & Environment.NewLine &
                              "Click OK to open the download page now.",
                        caption:="Java 8 Required",
                        buttons:=MessageBoxButtons.OKCancel,
                        icon:=MessageBoxIcon.Error
                    )

                    My.logger.Logger.LogInfo("Missing Java 8 (JDK and JRE v1.8)")

                    If result = DialogResult.OK Then
                        Await OpenURLAsync("https://archive.org/download/Java-Archive/Java%20SE%208%20%288u202%20and%20earlier%29/8u152/JDK/jdk-8u152-windows-i586.exe")
                        My.logger.Logger.LogInfo("User chose to open Java 8 download URL.")
                    Else
                        My.logger.Logger.LogInfo("User cancelled Java 8 download prompt.")
                    End If

                    Return False
                End If

                MainForm.UsingJDK1_8 = False
                javaPath = jrePath
                My.logger.Logger.LogWarning("JDK 1.8 not found. Falling back to JRE 1.8.")
                Dim jdkResult = MessageBox.Show(
                    owner:=SplashScreen,
                    text:="Java Development Kit (JDK) 1.8 was not found." & Environment.NewLine &
                          "STAR features will be unavailable until it is installed." & Environment.NewLine & Environment.NewLine &
                          "Click OK to open the JDK 8 download page.",
                    caption:="JDK 1.8 Not Detected",
                    buttons:=MessageBoxButtons.OKCancel,
                    icon:=MessageBoxIcon.Information
                )

                If jdkResult = DialogResult.OK Then
                    Await OpenURLAsync("https://archive.org/download/Java-Archive/Java%20SE%208%20%288u202%20and%20earlier%29/8u152/JDK/jdk-8u152-windows-i586.exe")
                    My.logger.Logger.LogInfo("User chose to download JDK 1.8.")
                Else
                    My.logger.Logger.LogInfo("User skipped JDK 1.8 download.")
                End If
            End If

            ' Step 2: Set Java Bin Path
            MainForm.Java1_8BinFolderPath = Path.Combine(javaPath, "bin")
            My.logger.Logger.LogInfo($"Using Java 1.8 bin folder: {MainForm.Java1_8BinFolderPath}")

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
                        Else
                            My.logger.Logger.LogWarning($"Could not open HKLM\{path} to update CurrentVersion.")
                        End If
                    End Using
                Catch ex As Exception
                    My.logger.Logger.LogError($"Failed to update CurrentVersion in {path}: {ex.Message}")
                End Try
            Next

            ' Step 4: Verify
            Using regKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Wow6432Node\JavaSoft\Java Runtime Environment")
                Dim currentVersion = regKey?.GetValue("CurrentVersion")?.ToString()
                My.logger.Logger.LogInfo("Checking for JRE CurrentVersion regKey 1.8")

                If currentVersion <> "1.8" Then
                    MessageBox.Show(owner:=SplashScreen,
                            "Java Runtime CurrentVersion registry key must be set to 1.8. Please rerun the app as Administrator.",
                            "Java Registry Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
                    Return False
                End If
            End Using

            Return True
        End Function
        Public Shared Async Function DetectJava21PlusAsync() As Task(Of Boolean)
            Dim result = Await Task.Run(
        Function()
            Dim bestVersion As Version = Nothing
            Dim bestPath As String = Nothing

            ' === Phase 1: Registry sweep across all known JDK vendors ===
            Dim baseKeys As String() = {
                "SOFTWARE\JavaSoft\JDK",
                "SOFTWARE\WOW6432Node\JavaSoft\JDK",
                "SOFTWARE\Eclipse Adoptium\JDK",
                "SOFTWARE\WOW6432Node\Eclipse Adoptium\JDK",
                "SOFTWARE\Eclipse Foundation\JDK",
                "SOFTWARE\WOW6432Node\Eclipse Foundation\JDK",
                "SOFTWARE\AdoptOpenJDK\JDK",
                "SOFTWARE\WOW6432Node\AdoptOpenJDK\JDK",
                "SOFTWARE\Microsoft\JDK",
                "SOFTWARE\WOW6432Node\Microsoft\JDK",
                "SOFTWARE\Amazon\JDK",
                "SOFTWARE\WOW6432Node\Amazon\JDK"
            }

            For Each basePath In baseKeys
                Try
                    Using baseKey As RegistryKey = Registry.LocalMachine.OpenSubKey(basePath)
                        If baseKey Is Nothing Then Continue For
                        For Each subName In baseKey.GetSubKeyNames()
                            Dim ver As Version = Nothing
                            If Version.TryParse(subName, ver) AndAlso ver.Major >= 21 Then
                                Using subKey = baseKey.OpenSubKey(subName)
                                    Dim home = subKey?.GetValue("JavaHome")?.ToString()
                                    If home IsNot Nothing AndAlso
                                       File.Exists(Path.Combine(home, "bin", "java.exe")) AndAlso
                                       (bestVersion Is Nothing OrElse ver > bestVersion) Then
                                        bestVersion = ver
                                        bestPath = home
                                    End If
                                End Using
                            End If
                        Next
                    End Using
                Catch ex As System.Security.SecurityException
                    ' Insufficient permissions to read this key, skip it
                End Try
            Next

            ' === Phase 2: Filesystem scan of common install locations ===
            If bestPath Is Nothing Then
                Dim programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
                Dim searchRoots As String() = {
                    Path.Combine(programFiles, "Java"),
                    Path.Combine(programFiles, "Eclipse Adoptium"),
                    Path.Combine(programFiles, "Microsoft"),
                    Path.Combine(programFiles, "Amazon Corretto"),
                    Path.Combine(programFiles, "Zulu"),
                    Path.Combine(programFiles, "BellSoft"),
                    Path.Combine(programFiles, "Liberica")
                }

                For Each root In searchRoots
                    Try
                        If Not Directory.Exists(root) Then Continue For
                        For Each jdkDir In Directory.GetDirectories(root)
                            Dim javaExe = Path.Combine(jdkDir, "bin", "java.exe")
                            If Not File.Exists(javaExe) Then Continue For

                            Dim folderName = Path.GetFileName(jdkDir)
                            Dim ver = TryParseJdkFolderVersion(folderName)
                            If ver IsNot Nothing AndAlso ver.Major >= 21 AndAlso
       (bestVersion Is Nothing OrElse ver > bestVersion) Then
                                bestVersion = ver
                                bestPath = jdkDir
                            End If
                        Next
                    Catch ex As UnauthorizedAccessException
                        ' Can't read this directory, skip it
                    End Try
                Next
            End If

            Return bestPath
        End Function)

            If Not String.IsNullOrEmpty(result) Then
                MainForm.Java21PlusBinFolderPath = Path.Combine(result, "bin")
                My.logger.Logger.LogInfo($"Found Java 21+ at: {result}")
                Return True
            Else
                MainForm.Java21PlusBinFolderPath = Nothing
                My.logger.Logger.LogWarning("Java 21+ not found.")
                Return False
            End If
        End Function
        Private Shared Function TryParseJdkFolderVersion(folderName As String) As Version
            ' Handles: "jdk-21.0.2", "jdk-21", "21.0.2", "temurin-21.0.2+13", etc.
            Dim cleaned = folderName
            ' Strip common prefixes
            For Each prefix In {"jdk-", "jdk", "jre-", "jre", "temurin-", "zulu-", "corretto-"}
                If cleaned.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) Then
                    cleaned = cleaned.Substring(prefix.Length)
                    Exit For
                End If
            Next
            ' Strip build metadata: "+13", "_392", etc.
            Dim plusIdx = cleaned.IndexOfAny({"+"c, "_"c})
            If plusIdx >= 0 Then cleaned = cleaned.Substring(0, plusIdx)

            Dim ver As Version = Nothing
            If Version.TryParse(cleaned, ver) Then Return ver
            ' Handle single number like "21"
            Dim major As Integer
            If Integer.TryParse(cleaned, major) Then Return New Version(major, 0)
            Return Nothing
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
            Dim minVersion As New Version(8, 0, 0)

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
        Public Shared Sub CheckForSpacesInPath()
            Dim exePath As String = Assembly.GetExecutingAssembly().Location
            Dim invalidChars As Char() = {" "c, "("c, ")"c, "{"c, "}"c}

            If exePath.IndexOfAny(invalidChars) <> -1 Then
                MessageBox.Show(owner:=SplashScreen,
            "The path to the KeitaiWorldLauncher contains invalid characters:" & Environment.NewLine &
            """" & exePath & """" & Environment.NewLine &
            "Due to LocaleEmulator please move it to a location without spaces, parentheses (), or braces {}.",
            "Warning - Invalid Path Characters",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning)
            End If
        End Sub
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
        Public Shared Async Sub SendKWLLaunchStats()
            Try
                Dim dotNetVersion As String = Environment.Version.ToString()
                Dim javaVersion As String = GetJavaVersion()
                Dim version As String = KeitaiWorldLauncher.My.Application.Info.Version.ToString
                Dim platform As String = Environment.OSVersion.Platform.ToString()
                Dim json As String = $"{{""version"":""{version}"",""platform"":""{platform}"",""dotNetVersion"":""{dotNetVersion}"",""javaVersion"":""{javaVersion}"",""source"":""KWL""}}"

                Dim content As New StringContent(json, Encoding.UTF8, "application/json")
                Await Http.PostAsync("https://script.google.com/macros/s/AKfycbwtXDCzN-V2XYV-zDHkYfxJdDd6xPyR8xhHSpKrXhMFDQklkxgENVUvXSTcgablGoOvmQ/exec", content)
            Catch
                ' Fail silently
            End Try
        End Sub
        Public Shared Async Sub SendAppLaunch(inputAppName As String)
            Try
                Dim json As String = $"{{""appName"":""{inputAppName}""}}"
                Dim content As New StringContent(json, Encoding.UTF8, "application/json")
                Await Http.PostAsync("https://script.google.com/macros/s/AKfycbwtXDCzN-V2XYV-zDHkYfxJdDd6xPyR8xhHSpKrXhMFDQklkxgENVUvXSTcgablGoOvmQ/exec", content)
            Catch
                ' Fail silently
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
        Public Shared Async Function IsInternetAvailableAsync(urls As String()) As Task(Of Boolean)
            Dim tasks As New List(Of Task(Of Boolean))(
        urls.Select(Function(url) Task.Run(Async Function() As Task(Of Boolean)
                                               Try
                                                   Using request As New Net.Http.HttpRequestMessage(Net.Http.HttpMethod.Head, url)
                                                       Using response = Await Http.SendAsync(
                        request,
                        Net.Http.HttpCompletionOption.ResponseHeadersRead)
                                                           Return True
                                                       End Using
                                                   End Using
                                               Catch ex As Net.Http.HttpRequestException
                                                   My.logger.Logger.LogWarning($"Internet check failed for {url}: {ex.Message}")
                                                   Return False
                                               Catch ex As TaskCanceledException
                                                   My.logger.Logger.LogWarning($"Internet check timed out for {url}")
                                                   Return False
                                               Catch ex As Exception
                                                   My.logger.Logger.LogError($"Internet check exception for {url}: {ex.Message}")
                                                   Return False
                                               End Try
                                           End Function))
    )

            While tasks.Count > 0
                Dim completed = Await Task.WhenAny(tasks)
                If completed.Result Then Return True
                tasks.Remove(completed)
            End While

            Return False
        End Function
        Public Shared Async Function CanReachFileAsync(url As String) As Task(Of Boolean)
            Try
                Using cts As New CancellationTokenSource(TimeSpan.FromSeconds(10))
                    Using request As New HttpRequestMessage(HttpMethod.Head, url)
                        Using response = Await Http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cts.Token)
                            Return response.IsSuccessStatusCode OrElse
                           (CInt(response.StatusCode) >= 300 AndAlso CInt(response.StatusCode) < 400)
                        End Using
                    End Using
                End Using
            Catch
                Return False
            End Try
        End Function
        Public Shared Sub ShowSnackBar(InputString As String)
            Dim snackBar As New ReaLTaiizor.Controls.MaterialSnackBar(InputString)
            snackBar.Show(MainForm)
        End Sub

        'Generate Controls
        Public Shared Sub GenerateDynamicControlsFromLines(JAMFile As String, container As Panel, Optional gameTitle As String = "")
            Try
                container.SuspendLayout()
                container.Controls.Clear()

                ' Update the GroupBox title if we have a game name
                If TypeOf container.Parent Is GroupBox Then
                    Dim gbx = DirectCast(container.Parent, GroupBox)
                    gbx.Text = If(String.IsNullOrWhiteSpace(gameTitle), "Appli Info", $"Appli Info - {gameTitle}")
                End If

                ' SWF files have no metadata to show
                If JAMFile.EndsWith(".swf", StringComparison.OrdinalIgnoreCase) Then
                    Return
                End If

                ' Determine file encoding and delimiter
                Dim lines As String()
                Dim delimiter As Char
                Dim encoding As Encoding

                If JAMFile.EndsWith(".jam", StringComparison.OrdinalIgnoreCase) Then
                    encoding = Encoding.GetEncoding(932)
                    lines = File.ReadAllLines(JAMFile, encoding)
                    delimiter = "="c
                ElseIf JAMFile.EndsWith(".jad", StringComparison.OrdinalIgnoreCase) Then
                    encoding = Encoding.UTF8
                    lines = File.ReadAllLines(JAMFile, encoding)
                    delimiter = ":"c
                ElseIf JAMFile.EndsWith(".kjx", StringComparison.OrdinalIgnoreCase) Then
                    lines = ExtractKjxMetadata(JAMFile)
                    delimiter = ":"c
                    If lines Is Nothing OrElse lines.Length = 0 Then
                        Dim noDataLabel As New Label() With {
                            .Text = "No metadata found in KJX file",
                            .AutoSize = False,
                            .Dock = DockStyle.Fill,
                            .TextAlign = ContentAlignment.MiddleCenter,
                            .ForeColor = Color.DarkRed,
                            .Font = New Font("Segoe UI", 10, FontStyle.Bold)
                        }
                        container.Controls.Add(noDataLabel)
                        Return
                    End If
                Else
                    logger.Logger.LogInfo($"Unknown File Type ({JAMFile}) unable to generate controls.")
                    Return
                End If

                ' Build a ListView to display the key-value pairs
                Dim lv As New ListView() With {
                    .View = View.Details,
                    .Dock = DockStyle.Fill,
                    .FullRowSelect = True,
                    .GridLines = True,
                    .HeaderStyle = ColumnHeaderStyle.Nonclickable,
                    .Font = New Font("Segoe UI", 9)
                }

                ' Enable double-buffering
                Dim prop = GetType(ListView).GetProperty("DoubleBuffered",
            Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)
                prop.SetValue(lv, True)

                ' Two columns
                lv.Columns.Add("Property", 140, HorizontalAlignment.Left)
                lv.Columns.Add("Value", container.ClientSize.Width - 160, HorizontalAlignment.Left)

                ' Build the context menu
                Dim cms As New ContextMenuStrip()
                Dim editItem As New ToolStripMenuItem("Edit Value")
                cms.Items.Add(editItem)
                lv.ContextMenuStrip = cms

                ' Store file info in the ListView's Tag so the handler can access it
                lv.Tag = New Dictionary(Of String, Object) From {
                    {"FilePath", JAMFile},
                    {"Delimiter", delimiter},
                    {"Encoding", encoding}
                }

                ' Only show menu when right-clicking on a row, and not for KJX files
                AddHandler cms.Opening, Sub(sender, e)
                                            If lv.SelectedItems.Count = 0 Then
                                                e.Cancel = True
                                                Return
                                            End If

                                            Dim fileInfo = DirectCast(lv.Tag, Dictionary(Of String, Object))
                                            Dim filePath = fileInfo("FilePath").ToString()
                                            If filePath.EndsWith(".kjx", StringComparison.OrdinalIgnoreCase) Then
                                                e.Cancel = True
                                            End If
                                        End Sub

                ' Handle the edit click
                AddHandler editItem.Click, Sub(sender, e)
                                               If lv.SelectedItems.Count = 0 Then Return
                                               ' Show one-time warning per app launch
                                               If Not _appliEditWarningShown Then
                                                   Dim accepted As Boolean = False
                                                   Dim warningForm As New ReaLTaiizor.Forms.MaterialForm() With {
                                                       .Text = "Appli Info Editor",
                                                       .Size = New Size(480, 230),
                                                       .StartPosition = FormStartPosition.CenterParent,
                                                       .FormBorderStyle = FormBorderStyle.FixedDialog,
                                                       .Sizable = False,
                                                       .MaximizeBox = False,
                                                       .MinimizeBox = False
                                                   }
                                                   Dim lblWarning As New Label() With {
                                                       .Text = "Modifying Appli Info values can break games or cause" & vbCrLf &
                                                               "unexpected behavior." & vbCrLf & vbCrLf &
                                                               "Please do not modify this unless you know what you are doing.",
                                                       .Font = New Font("Segoe UI", 10),
                                                       .Left = 20,
                                                       .Top = 76,
                                                       .AutoSize = True
                                                   }
                                                   Dim btnContinue As New ReaLTaiizor.Controls.MaterialButton() With {
                                                       .Text = "CONTINUE",
                                                       .Left = warningForm.ClientSize.Width - 230,
                                                       .Top = warningForm.ClientSize.Height - 52,
                                                       .Width = 110,
                                                       .HighEmphasis = True,
                                                       .Type = ReaLTaiizor.Controls.MaterialButton.MaterialButtonType.Contained
                                                   }
                                                   Dim btnCancel As New ReaLTaiizor.Controls.MaterialButton() With {
                                                       .Text = "CANCEL",
                                                       .Left = warningForm.ClientSize.Width - 110,
                                                       .Top = warningForm.ClientSize.Height - 52,
                                                       .Width = 100,
                                                       .HighEmphasis = False,
                                                       .Type = ReaLTaiizor.Controls.MaterialButton.MaterialButtonType.Text
                                                   }
                                                   AddHandler btnContinue.Click, Sub()
                                                                                     accepted = True
                                                                                     warningForm.Close()
                                                                                 End Sub
                                                   AddHandler btnCancel.Click, Sub()
                                                                                   warningForm.Close()
                                                                               End Sub

                                                   warningForm.Controls.Add(lblWarning)
                                                   warningForm.Controls.Add(btnContinue)
                                                   warningForm.Controls.Add(btnCancel)
                                                   warningForm.ShowDialog()
                                                   warningForm.Dispose()
                                                   If Not accepted Then Return
                                                   _appliEditWarningShown = True
                                               End If
                                               Dim selectedRow = lv.SelectedItems(0)
                                               Dim propertyName = selectedRow.Text
                                               Dim currentValue = selectedRow.SubItems(1).Text
                                               Dim fileInfo = DirectCast(lv.Tag, Dictionary(Of String, Object))
                                               Dim filePath = fileInfo("FilePath").ToString()
                                               Dim fileDelimiter = CChar(fileInfo("Delimiter"))
                                               Dim fileEncoding = DirectCast(fileInfo("Encoding"), Encoding)

                                               ' Show edit dialog
                                               Dim newValue = ShowEditDialog(propertyName, currentValue)
                                               If newValue Is Nothing Then Return  ' User cancelled

                                               ' Update the file
                                               Try
                                                   Dim fileLines = File.ReadAllLines(filePath, fileEncoding).ToList()
                                                   Dim prefix = propertyName & " " & fileDelimiter & " "
                                                   Dim prefixNoSpace = propertyName & fileDelimiter

                                                   For i = 0 To fileLines.Count - 1
                                                       Dim trimmed = fileLines(i).TrimStart()
                                                       If trimmed.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) OrElse
                                                  trimmed.StartsWith(prefixNoSpace, StringComparison.OrdinalIgnoreCase) Then
                                                           fileLines(i) = $"{propertyName} {fileDelimiter} {newValue}"
                                                           Exit For
                                                       End If
                                                   Next

                                                   File.WriteAllLines(filePath, fileLines, fileEncoding)

                                                   ' Update the ListView row
                                                   selectedRow.SubItems(1).Text = newValue
                                                   logger.Logger.LogInfo($"Updated '{propertyName}' to '{newValue}' in {filePath}")

                                               Catch ex As Exception
                                                   logger.Logger.LogError($"Failed to update {propertyName}: {ex.Message}")
                                                   MessageBox.Show($"Failed to save change: {ex.Message}",
                                                           "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                               End Try
                                           End Sub

                ' Populate rows
                lv.BeginUpdate()

                For Each line As String In lines
                    If String.IsNullOrWhiteSpace(line) Then Continue For

                    Dim parts As String() = line.Split(New Char() {delimiter}, 2)
                    If parts.Length <> 2 Then Continue For

                    Dim key As String = parts(0).Trim()
                    Dim value As String = parts(1).Trim()

                    Dim item As New ListViewItem(key)
                    item.SubItems.Add(value)
                    lv.Items.Add(item)
                Next

                lv.EndUpdate()
                container.Controls.Add(lv)

            Catch ex As Exception
                logger.Logger.LogError($"Error generating controls for JAM/JAD: {ex.Message}")
            Finally
                container.ResumeLayout(True)
            End Try
        End Sub
        Private Shared Function ShowEditDialog(propertyName As String, currentValue As String) As String
            Dim frm As New ReaLTaiizor.Forms.MaterialForm() With {
        .Text = $"Edit Property",
        .Size = New Size(520, 250),
        .StartPosition = FormStartPosition.CenterParent,
        .FormBorderStyle = FormBorderStyle.FixedDialog,
        .Sizable = False,
        .MaximizeBox = False,
        .MinimizeBox = False
    }

            Dim lblProperty As New Label() With {
        .Text = propertyName,
        .Font = New Font("Segoe UI", 11, FontStyle.Bold),
        .Left = 20,
        .Top = 76,
        .AutoSize = True
    }

            Dim txtValue As New ReaLTaiizor.Controls.MaterialTextBoxEdit() With {
        .Text = currentValue,
        .Left = 20,
        .Top = lblProperty.Top + lblProperty.Height + 12,
        .Size = New Size(frm.ClientSize.Width - 40, 40),
        .Hint = "Enter new value...",
        .MaxLength = 500,
        .UseSystemPasswordChar = False
    }

            Dim btnSave As New ReaLTaiizor.Controls.MaterialButton() With {
        .Text = "SAVE",
        .DialogResult = DialogResult.OK,
        .Left = frm.ClientSize.Width - 220,
        .Top = txtValue.Top + txtValue.Height + 20,
        .Width = 100,
        .HighEmphasis = True,
        .Type = ReaLTaiizor.Controls.MaterialButton.MaterialButtonType.Contained
    }

            Dim btnCancel As New ReaLTaiizor.Controls.MaterialButton() With {
        .Text = "CANCEL",
        .DialogResult = DialogResult.Cancel,
        .Left = frm.ClientSize.Width - 110,
        .Top = txtValue.Top + txtValue.Height + 20,
        .Width = 100,
        .HighEmphasis = False,
        .Type = ReaLTaiizor.Controls.MaterialButton.MaterialButtonType.Text
    }

            frm.AcceptButton = btnSave
            frm.CancelButton = btnCancel

            frm.Controls.Add(lblProperty)
            frm.Controls.Add(txtValue)
            frm.Controls.Add(btnSave)
            frm.Controls.Add(btnCancel)

            AddHandler frm.Shown, Sub() txtValue.SelectAll()

            Dim result As String = Nothing
            If frm.ShowDialog() = DialogResult.OK Then
                result = txtValue.Text
            End If

            frm.Dispose()
            Return result
        End Function

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
            If MainForm.ListViewGames.SmallImageList Is Nothing Then
                MainForm.ListViewGames.SmallImageList = New ImageList()
                MainForm.ListViewGames.SmallImageList.ImageSize = New Size(24, 24)
            End If

            ' Load the new icon
            If File.Exists(newIconPath) Then
                Dim newIcon As Image = Image.FromFile(newIconPath)
                Dim newImageKey As String = Path.GetFileNameWithoutExtension(newIconPath)

                ' Add the new icon to the ImageList (if not already added)
                If Not MainForm.ListViewGames.SmallImageList.Images.ContainsKey(newImageKey) Then
                    MainForm.ListViewGames.SmallImageList.Images.Add(newImageKey, newIcon)
                End If

                ' Find and update the ListView item
                For Each item As ListViewItem In MainForm.ListViewGames.Items
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
                UtilManager.ShowLaunchOverlay(MainForm, "Launching...")


                ' Construct all paths
                Dim baseDir As String = AppDomain.CurrentDomain.BaseDirectory
                Dim useLocaleEmulator As Boolean = MainForm.chkbxLocalEmulator.Checked
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
                If Not Await UpdateDeviceSkin(DOJAPATH, "doja", MainForm.chkbxHidePhoneUI.Checked) Then
                    logger.Logger.LogError("[Launch] Failed to update DOJA skins.")
                    MessageBox.Show("Failed to update DOJA skins.", "Skin Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If

                ' Get DOJA version
                Dim VerIndex As Integer = DOJAPATH.LastIndexOf("\iDKDoJa", StringComparison.OrdinalIgnoreCase)
                Dim DOJAVER As Integer = DOJAPATH.Substring(VerIndex + 8, 1)

                ' Update draw size for DOJA5
                If DOJAVER = 5 Then
                    Dim dimensions = ExtractDrawAreaFromJam(jamPath, 240, 240)
                    Dim width As Integer = dimensions.Item1
                    Dim height As Integer = dimensions.Item2
                    Await UpdatedDOJADrawSize(DOJAPATH, width, height)
                    logger.Logger.LogInfo($"[Launch] Updated DOJA draw size to {width}x{height}")
                End If

                ' Update sound config
                Await UpdateSoundConf(DOJAPATH, MainForm.cbxAudioType.SelectedItem.ToString())
                logger.Logger.LogInfo($"[Launch] Updated DOJA sound config to {MainForm.cbxAudioType.SelectedItem}")

                ' Update app config and JAM entries
                Dim NoGameJameUpdatedList As List(Of String) = gameManager.NoUpdateJAMGames
                If Not NoGameJameUpdatedList.Contains(Path.GetFileNameWithoutExtension(GameJAM)) Then
                    Await UpdateDOJAAppconfig(DOJAPATH, jamPath)

                    If DOJAVER = 3 Then
                        Await PrepareJamFileForLaunchAsync(jamPath, "doja3", MainForm.NetworkUID, MainForm.chkbxModifyJamFiles.Checked, MainForm.chkboxNetworkModifyURLS.Checked)
                        ' Convert SP to SCR
                        Dim rootFolder = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(GameJAM), ".."))
                        Dim SPFile = Path.Combine(rootFolder, "sp", Path.GetFileNameWithoutExtension(GameJAM) & ".sp")
                        Await ProcessDOJA3SPtoSCR(SPFile)
                    Else
                        Await PrepareJamFileForLaunchAsync(jamPath, "doja5", MainForm.NetworkUID, MainForm.chkbxModifyJamFiles.Checked, MainForm.chkboxNetworkModifyURLS.Checked)
                    End If
                End If

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
                If MainForm.chkbxShaderGlass.Checked Then
                    Await LaunchShaderGlass(Path.GetFileNameWithoutExtension(jamPath))
                    logger.Logger.LogInfo("[ShaderGlass] ShaderGlass launched and monitoring started.")
                End If

                If MainForm.chkbxEnableController.Checked = True Then
                    Await LaunchControllerProfileAMGP()
                    If MainForm.chkboxControllerVibration.Checked = True Then
                        MainForm.vibrationManager.StartMonitoring(Path.Combine(DOJAPATH, "lib", "skin", "device1", "vibrator.bmp"))
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
                UtilManager.ShowLaunchOverlay(MainForm, "Launching...")

                ' Construct all paths
                Dim baseDir As String = AppDomain.CurrentDomain.BaseDirectory
                Dim useLocaleEmulator As Boolean = MainForm.chkbxLocalEmulator.Checked
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
                Dim selectedSJMELaunchOption = MainForm.cbxSJMELaunchOption.SelectedItem.ToString
                Dim selectedSJMEScaling = MainForm.cbxSJMEScaling.SelectedItem.ToString
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


                ' Let filesystem settle (especially important on slower drives)
                Await Task.Delay(500)

                ' Launch SJME with retry logic
                Dim success As Boolean = Await LaunchEmulatorWithRetry(
                    appPath,
                    arguments,
                    SJMEProcessWatch,
                    baseDir,
                    Async Function()
                        Return Await WaitForProcessWindowAsync({SJMEProcessWatch}, "WaitForSQUIRREKJMEToStart")
                    End Function,
                    initialDelayMs:=1500,
                    captureOutput:=False
                )

                If Not success Then
                    MessageBox.Show("Failed to launch squirreljme after retrying.", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If

                ' ShaderGlass launch if enabled
                If MainForm.chkbxShaderGlass.Checked Then
                    Await LaunchShaderGlass(AppName)
                    logger.Logger.LogInfo("[ShaderGlass] ShaderGlass launched and monitoring started.")
                End If

                If MainForm.chkbxEnableController.Checked = True Then
                    Await LaunchControllerProfileAMGP()
                    If MainForm.chkboxControllerVibration.Checked = True Then
                        MessageBox.Show("Disabling Vibration due to no supported with SquirrelJME")
                        MainForm.chkboxControllerVibration.Checked = False
                    End If
                End If
                ProcessManager.StartMonitoring(jamPath)

            Catch ex As Exception
                logger.Logger.LogError($"[Launch] Exception occurred: {ex}")
                MessageBox.Show($"Failed to launch the game: {ex.Message}", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
        Public Async Sub LaunchCustom_OpenDojaGameCommand(OpenDojaPath As String, OpenDojaEXELocation As String, GameJAM As String)
            Try
                logger.Logger.LogInfo("[Launch] Starting OpenDoja game launch sequence...")

                ' Validate inputs
                If String.IsNullOrWhiteSpace(OpenDojaPath) OrElse String.IsNullOrWhiteSpace(OpenDojaEXELocation) OrElse String.IsNullOrWhiteSpace(GameJAM) Then
                    Throw New ArgumentException("One or more required parameters are missing.")
                End If

                'Start overlay
                UtilManager.ShowLaunchOverlay(MainForm, "Launching...")

                ' Prepare all paths
                Dim baseDir = AppDomain.CurrentDomain.BaseDirectory
                Dim useLocaleEmulator As Boolean = MainForm.chkbxLocalEmulator.Checked
                Dim appPath As String
                Dim arguments As String

                ' Make Full Paths
                Dim Java32EXE As String = Path.Combine(MainForm.Java21PlusBinFolderPath, "java.exe")
                Dim exePath As String = OpenDojaEXELocation.Trim
                Dim jamPath As String = Path.Combine(baseDir, GameJAM).Trim()
                Dim jarPath As String = Path.Combine(Path.GetDirectoryName(jamPath), Path.GetFileNameWithoutExtension(jamPath) & ".jar")

                If jamPath.Length > 240 Then
                    logger.Logger.LogWarning($"[Launch] JAD file path exceeds 240 characters: {jamPath}")
                    MessageBox.Show("The file path length exceeds 240 characters. You might experience issues running. Try moving Keitai World Emulator to the root of C:/", "Path Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

                ' Form arguments based on launch method
                appPath = Java32EXE
                arguments = $"{Path.GetFileName(exePath)} ""{jamPath}"""
                logger.Logger.LogInfo($"[Launch] Launching OpenDoja directly without Locale Emulator: {arguments}")

                ' Config updates / ' Extract AppName from JAM
                Dim AppName As String
                If GameJAM.EndsWith(".jam") Then
                    AppName = ExtractAppNamefromJAM(jamPath)
                    Await PrepareJamFileForLaunchAsync(jamPath, "opendoja", MainForm.NetworkUID, MainForm.chkbxModifyJamFiles.Checked, MainForm.chkboxNetworkModifyURLS.Checked)
                End If

                ' Launch OpenDoja JAVA with retry logic
                Dim success = Await LaunchOpenDojaAppAsync(Java32EXE, OpenDojaEXELocation, jamPath)
                If Not success Then
                    HideLaunchOverlay()
                    MessageBox.Show("Failed to launch OpenDoja after retrying.", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If

                ' ShaderGlass launch if enabled
                If MainForm.chkbxShaderGlass.Checked Then
                    logger.Logger.LogInfo("[ShaderGlass] Not Supported for OpenDoja, Disabling")
                    MainForm.chkbxShaderGlass.Checked = False
                    MessageBox.Show("ShaderGlass is not supported with OpenDoja. Please manually resize the OpenDoja window by rightclicking on the window")
                End If
                If MainForm.chkbxEnableController.Checked = True Then
                    Await LaunchControllerProfileAMGP()
                End If
                ProcessManager.StartMonitoring(jamPath)
                HideLaunchOverlay()

            Catch ex As ArgumentException
                logger.Logger.LogError($"[Launch] Invalid input: {ex.Message}")
                MessageBox.Show($"Invalid input: {ex.Message}", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)

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
                UtilManager.ShowLaunchOverlay(MainForm, "Launching...")

                ' Prepare all paths
                Dim baseDir = AppDomain.CurrentDomain.BaseDirectory
                Dim useLocaleEmulator As Boolean = MainForm.chkbxLocalEmulator.Checked
                Dim appPath As String
                Dim arguments As String

                ' Make Full Paths
                Dim Java32EXE As String = Path.Combine(MainForm.Java1_8BinFolderPath, "java.exe")
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
                    Await AddJADMIDletEntriesAsync(jadjamPath, MainForm.NetworkUID, MainForm.TerminalID)
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
                If MainForm.chkbxShaderGlass.Checked Then
                    logger.Logger.LogInfo("[ShaderGlass] Not Supported for KEmulator, Disabling")
                    MainForm.chkbxShaderGlass.Checked = False
                    MessageBox.Show("ShaderGlass is not supported with KEmulator. Please manually resize the KEmulator window by dragging it larger.")
                End If
                If MainForm.chkbxEnableController.Checked = True Then
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
                UtilManager.ShowLaunchOverlay(MainForm, "Launching...")

                ' Prepare all paths
                Dim baseDir = AppDomain.CurrentDomain.BaseDirectory
                Dim useLocaleEmulator As Boolean = MainForm.chkbxLocalEmulator.Checked
                Dim appPath As String
                Dim arguments As String

                Dim exePath As String = STAREXELocation.Trim
                Dim jamPath As String = Path.Combine(baseDir, GameJAM).Trim()

                If jamPath.Length > 240 Then
                    logger.Logger.LogWarning($"[Launch] JAM file path exceeds 240 characters: {jamPath}")
                    MessageBox.Show("The file path length exceeds 240 characters. You might experience issues running. Try moving Keitai World Emulator to the root of C:/", "Path Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

                ' If HardwareRendering Enabled
                Dim useHardwareRendering As Boolean = MainForm.chkboxEnforceHardwareRendering.Checked


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
                If Not Await UpdateDeviceSkin(STARPATH, "star", MainForm.chkbxHidePhoneUI.Checked) Then
                    logger.Logger.LogError("[Launch] Failed to update STAR skins.")
                    MessageBox.Show("Failed to update STAR skins.", "Skin Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If

                ' Screen size
                Dim dimensions = ExtractDrawAreaFromJam(jamPath, 480, 480)
                Await UpdatedSTARDrawSize(STARPATH, dimensions.Item1, dimensions.Item2)
                logger.Logger.LogInfo($"[Launch] STAR draw size set to {dimensions.Item1}x{dimensions.Item2}")

                ' Config updates
                Await UpdateSoundConf(STARPATH, MainForm.cbxAudioType.SelectedItem.ToString())
                logger.Logger.LogInfo($"[Launch] STAR sound config set to {MainForm.cbxAudioType.SelectedItem}")
                Await UpdateSTARAppconfig(STARPATH, GameJAM)
                Await PrepareJamFileForLaunchAsync(GameJAM, "star", MainForm.NetworkUID, MainForm.chkbxModifyJamFiles.Checked, MainForm.chkboxNetworkModifyURLS.Checked)
                logger.Logger.LogInfo("[Launch] STAR app configuration and JAM entries updated.")

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
                Dim envVars As Dictionary(Of String, String) = Nothing
                If MainForm.chkboxEnforceHardwareRendering.Checked Then

                End If

                ' Launch STAR with retry logic
                Dim success As Boolean = Await LaunchEmulatorWithRetry(
                    appPath,
                    arguments,
                    "star",
                    baseDir,
                    Async Function()
                        Return Await WaitForProcessWindowAsync({"star"}, "WaitForSTARToStart")
                    End Function,
                    environmentVars:=envVars
                )

                If Not success Then
                    MessageBox.Show("Failed to launch STAR after retrying.", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If

                ' ShaderGlass launch if enabled
                If MainForm.chkbxShaderGlass.Checked Then
                    Await LaunchShaderGlass(Path.GetFileNameWithoutExtension(jamPath))
                    logger.Logger.LogInfo("[ShaderGlass] ShaderGlass launched and monitoring started.")
                End If

                If MainForm.chkbxEnableController.Checked = True Then
                    Await LaunchControllerProfileAMGP()
                    If MainForm.chkboxControllerVibration.Checked = True Then
                        MainForm.vibrationManager.StartMonitoring(Path.Combine(STARPATH, "lib", "skin", "device1", "vibrator.bmp"))
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
                UtilManager.ShowLaunchOverlay(MainForm, "Launching...")

                ' Prepare all paths
                Dim baseDir = AppDomain.CurrentDomain.BaseDirectory
                Dim useLocaleEmulator As Boolean = MainForm.chkbxLocalEmulator.Checked
                Dim appPath As String
                Dim arguments As String

                ' Make Full Paths
                Dim Java32EXE As String = Path.Combine(MainForm.Java1_8BinFolderPath, "java.exe")
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
                Await AddJADMIDletEntriesAsync(jadPath, MainForm.NetworkUID, MainForm.TerminalID)
                Await Task.Delay(500) ' Let the filesystem settle

                ' Launch JSKY JAVA with retry logic
                Dim success = Await LaunchJSkyAppAsync(Java32EXE, JSKYEXELocation, jadPath)
                If Not success Then
                    HideLaunchOverlay()
                    MessageBox.Show("Failed to launch JSKY after retrying.", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If

                ' ShaderGlass launch if enabled
                If MainForm.chkbxShaderGlass.Checked Then
                    logger.Logger.LogInfo("[ShaderGlass] Waiting for JSKY to become idle...")
                    If Await WaitForProcessWindowAsync({"java"}, "WaitForJavaToStart") Then
                        Await LaunchShaderGlass("J-SKY Application Emulator")
                        logger.Logger.LogInfo("[ShaderGlass] ShaderGlass launched and monitoring started.")
                    Else
                        logger.Logger.LogError("[ShaderGlass] Failed to detect JSKY running.")
                        MessageBox.Show("Failed to detect JSKY running.", "ShaderGlass Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                End If

                If MainForm.chkbxEnableController.Checked = True Then
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
                UtilManager.ShowLaunchOverlay(MainForm, "Launching...")

                ' Prepare all paths
                Dim baseDir = AppDomain.CurrentDomain.BaseDirectory
                Dim useLocaleEmulator As Boolean = MainForm.chkbxLocalEmulator.Checked
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
                Await AddJADMIDletEntriesAsync(jadPath, MainForm.NetworkUID, MainForm.TerminalID)
                Await Task.Delay(500) ' Let the filesystem settle

                ' Launch VODAFONE  with retry logic
                Dim success As Boolean = Await LaunchEmulatorWithRetry(
                    appPath,
                    arguments,
                    "emulator",
                    baseDir,
                    Async Function()
                        Return Await WaitForProcessWindowAsync({"emulator"}, "WaitForVODAFONEToStart")
                    End Function,
                    initialDelayMs:=1500,
                    captureOutput:=False
                )
                If Not success Then
                    HideLaunchOverlay()
                    MessageBox.Show("Failed to launch VODAFONE after retrying.", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If

                ' ShaderGlass launch if enabled
                If MainForm.chkbxShaderGlass.Checked Then
                    Await LaunchShaderGlass("V-appli Emulator")
                    logger.Logger.LogInfo("[ShaderGlass] ShaderGlass launched and monitoring started.")
                End If

                If MainForm.chkbxEnableController.Checked = True Then
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
        Public Async Sub LaunchCustom_FreeJ2MEGameCommand(freej2mePATH As String, freej2meEXELocation As String, GameJAM As String)
            Try
                logger.Logger.LogInfo("[Launch] Starting FreeJ2ME game launch sequence...")

                ' Validate inputs
                If String.IsNullOrWhiteSpace(freej2mePATH) OrElse String.IsNullOrWhiteSpace(freej2meEXELocation) OrElse String.IsNullOrWhiteSpace(GameJAM) Then
                    Throw New ArgumentException("One or more required parameters are missing.")
                End If

                'Start overlay
                UtilManager.ShowLaunchOverlay(MainForm, "Launching...")

                ' Prepare all paths
                Dim baseDir = AppDomain.CurrentDomain.BaseDirectory
                Dim useLocaleEmulator As Boolean = MainForm.chkbxLocalEmulator.Checked
                Dim appPath As String
                Dim arguments As String

                ' Make Full Paths
                Dim Java32EXE As String = Path.Combine(MainForm.Java1_8BinFolderPath, "java.exe")
                Dim exePath As String = freej2meEXELocation.Trim
                Dim jadjamPath As String = ""
                Dim jarPath As String = ""
                Dim kjxpath As String = ""
                If GameJAM.EndsWith(".kjx") Then
                    kjxpath = Path.Combine(baseDir, GameJAM).Trim()
                Else
                    jadjamPath = Path.Combine(baseDir, GameJAM).Trim()
                    jarPath = Path.Combine(Path.GetDirectoryName(jadjamPath), Path.GetFileNameWithoutExtension(jadjamPath) & ".jar")
                End If

                If jadjamPath.Length > 240 Or kjxpath.Length > 240 Then
                    logger.Logger.LogWarning($"[Launch] KJX/JAD file path exceeds 240 characters: {jadjamPath}")
                    MessageBox.Show("The file path length exceeds 240 characters. You might experience issues running. Try moving Keitai World Emulator to the root of C:/", "Path Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

                ' Form arguments based on launch method
                appPath = Java32EXE
                arguments = $"{Path.GetFileName(exePath)} ""{jadjamPath}"""
                logger.Logger.LogInfo($"[Launch] Launching freej2me directly without Locale Emulator: {arguments}")

                ' Config updates
                Dim AppName As String
                If GameJAM.EndsWith(".jad") Then
                    AppName = GetMidletNameFromJad(jadjamPath)
                    Await UpdateJADJarURLAsync(jadjamPath)
                    Await AddJADMIDletEntriesAsync(jadjamPath, MainForm.NetworkUID, MainForm.TerminalID)
                ElseIf GameJAM.EndsWith(".jam") Then
                    AppName = ExtractAppNamefromJAM(jadjamPath)
                End If

                ' Launch freej2me JAVA with retry logic, KJX or JAR/JAD
                If kjxpath <> "" Then
                    Dim success = Await LaunchFreeJ2MEAppAsync(Java32EXE, freej2meEXELocation, kjxpath)
                    If Not success Then
                        HideLaunchOverlay()
                        MessageBox.Show("Failed to launch FreeJ2ME after retrying.", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Exit Sub
                    End If
                Else
                    Dim success = Await LaunchFreeJ2MEAppAsync(Java32EXE, freej2meEXELocation, jarPath)
                    If Not success Then
                        HideLaunchOverlay()
                        MessageBox.Show("Failed to launch FreeJ2ME after retrying.", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Exit Sub
                    End If
                End If

                ' ShaderGlass launch if enabled
                If MainForm.chkbxShaderGlass.Checked Then
                    logger.Logger.LogInfo("[ShaderGlass] Not Supported for FreeJ2ME, Disabling")
                    MainForm.chkbxShaderGlass.Checked = False
                    MessageBox.Show("ShaderGlass is not supported with FreeJ2ME. Please manually resize the FreeJ2ME window by dragging it larger.")
                End If
                If MainForm.chkbxEnableController.Checked = True Then
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
        Public Async Sub LaunchCustomFLASHGameCommand(FLASHPATH As String, FLASHEXELocation As String, GameJAM As String)
            Try
                logger.Logger.LogInfo("[Launch] Starting FLASH game launch sequence...")

                ' Validate inputs
                If String.IsNullOrWhiteSpace(FLASHPATH) OrElse String.IsNullOrWhiteSpace(FLASHEXELocation) OrElse String.IsNullOrWhiteSpace(GameJAM) Then
                    Throw New ArgumentException("One or more required parameters are missing.")
                End If

                'Start overlay
                UtilManager.ShowLaunchOverlay(MainForm, "Launching...")

                ' Prepare all paths
                Dim baseDir = AppDomain.CurrentDomain.BaseDirectory
                Dim useLocaleEmulator As Boolean = MainForm.chkbxLocalEmulator.Checked
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
                If MainForm.chkbxShaderGlass.Checked Then
                    Await LaunchShaderGlass("Adobe Flash Player 10")
                    logger.Logger.LogInfo("[ShaderGlass] ShaderGlass launched and monitoring started.")
                End If

                If MainForm.chkbxEnableController.Checked = True Then
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

                If MainForm.chkboxMachiCharaLocalEmulator.Checked = True Then
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

                If MainForm.chkboxCharadenLocalEmulator.Checked = True Then
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
        Public Async Function LaunchControllerProfileAMGP() As Task
            Dim selectedProfile As String = TryCast(MainForm.cbxControllerProfile.SelectedItem, String)
            Dim selectedController As String = TryCast(MainForm.cbxGameControllers.SelectedItem, String)

            If String.IsNullOrEmpty(selectedProfile) OrElse String.IsNullOrEmpty(selectedController) Then
                Return
            End If

            Dim controllerIndex As Integer = MainForm.cbxGameControllers.SelectedIndex

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
        Public Async Function LaunchOpenDojaAppAsync(javapath As String, opendojaExePath As String, jamPath As String) As Task(Of Boolean)
            Try
                Dim arguments As String
                If MainForm.chkbxOpenDojaLaunchGUI.Checked Then
                    arguments = $"-jar ""{Path.GetFileName(opendojaExePath)}"""
                Else
                    Dim scalePercent As Integer = Integer.Parse(MainForm.cbxOpenDojaHostScale.SelectedItem.ToString().Replace("%", "").Trim())
                    Dim scaleValue As String = (scalePercent \ 100).ToString()
                    Dim synthValue As String = MainForm.cbxOpenDojaAudioType.SelectedItem.ToString().Trim()
                    arguments = String.Join(" ",
                        $"-Dopendoja.hostScale={scaleValue}",
                        $"-Dopendoja.mldSynth={synthValue}",
                        $"-Dopendoja.userId={MainForm.NetworkUID}",
                        $"-Dopendoja.terminalId={MainForm.TerminalID}",
                        $"-jar ""{Path.GetFileName(opendojaExePath)}""",
                        $"--run-jam ""{jamPath}""")
                End If

                Dim psi As New ProcessStartInfo(javapath) With {
                    .Arguments = arguments,
                    .UseShellExecute = False,
                    .CreateNoWindow = True,
                    .WorkingDirectory = Path.GetDirectoryName(opendojaExePath)
                }

                Dim process As Process = Process.Start(psi)

                ' Optional delay to allow Java process to spawn
                Await Task.Delay(500)

                ' Check for any java process
                Dim javaRunning As Boolean = Process.GetProcessesByName("java").Any()

                Return javaRunning

            Catch ex As Exception
                logger.Logger.LogError($"[JavaLaunch] Failed to start Java app: {ex.Message}")
                Return False
            End Try
        End Function
        Public Async Function LaunchFreeJ2MEAppAsync(javapath As String, FreeJ2MEExePath As String, kjxjarPath As String) As Task(Of Boolean)
            Try
                ' Proper quoting for cmd.exe
                Dim arguments As String = $"/C """"{javapath}"" -jar ""{Path.GetFileName(FreeJ2MEExePath)}"" ""{kjxjarPath}"""""

                Dim psi As New ProcessStartInfo("cmd.exe", arguments) With {
                    .UseShellExecute = False,
                    .WorkingDirectory = Path.GetDirectoryName(FreeJ2MEExePath)
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
            waitFunction As Func(Of Task(Of Boolean)),
            Optional initialDelayMs As Integer = 500,
            Optional captureOutput As Boolean = True,
            Optional environmentVars As Dictionary(Of String, String) = Nothing
        ) As Task(Of Boolean)

            Dim startInfo As New ProcessStartInfo With {
                .FileName = fileName,
                .Arguments = arguments,
                .UseShellExecute = False,
                .CreateNoWindow = True,
                .RedirectStandardOutput = captureOutput,
                .RedirectStandardError = captureOutput,
                .RedirectStandardInput = Not captureOutput,
                .WorkingDirectory = workingDir
            }

            ' Apply any custom environment variables
            If environmentVars IsNot Nothing Then
                For Each kvp In environmentVars
                    startInfo.Environment(kvp.Key) = kvp.Value
                Next
            End If

            For attempt = 1 To 2
                Try
                    Dim process As Process = Process.Start(startInfo)
                    Await Task.Delay(initialDelayMs)

                    If process Is Nothing Then
                        logger.Logger.LogError($"[LaunchHelper] Attempt {attempt} Failed to start process.")
                        Continue For
                    End If

                    If captureOutput Then
                        Try
                            process.WaitForInputIdle()
                        Catch ex As Exception
                            ' Some apps don't support WaitForInputIdle
                        End Try
                        AddHandler process.OutputDataReceived, Sub(sender, e)
                                                                   If e.Data IsNot Nothing Then logger.Logger.LogInfo($"[{processNameToCheck.ToUpper()} STDOUT] {e.Data}")
                                                               End Sub
                        AddHandler process.ErrorDataReceived, Sub(sender, e)
                                                                  If e.Data IsNot Nothing Then logger.Logger.LogError($"[{processNameToCheck.ToUpper()} STDERR] {e.Data}")
                                                              End Sub
                        process.BeginOutputReadLine()
                        process.BeginErrorReadLine()
                    End If

                    logger.Logger.LogInfo($"[LaunchHelper] Waiting for {processNameToCheck} to become ready...")

                    ' App is Running
                    If Await waitFunction() Then
                        logger.Logger.LogInfo($"[LaunchHelper] {processNameToCheck} is running and ready.")
                        UtilManager.HideLaunchOverlay()
                        Return True
                    End If

                    ' Wait timed out — but only kill and retry if the process actually died
                    Dim stillRunning = Process.GetProcessesByName(processNameToCheck).Length > 0
                    If stillRunning Then
                        ' Process exists but window handle wasn't ready yet — treat as success
                        logger.Logger.LogInfo($"[LaunchHelper] {processNameToCheck} process found (no window handle yet). Treating as success.")
                        UtilManager.HideLaunchOverlay()
                        Return True
                    End If

                    ' Process genuinely failed to start — clean up and retry
                    logger.Logger.LogWarning($"[LaunchHelper] {processNameToCheck} not found on attempt {attempt}. Retrying...")
                    Try
                        If Not process.HasExited Then
                            process.Kill()
                            process.WaitForExit(2000)
                        End If
                    Catch
                        ' Already exited
                    End Try
                    process.Dispose()
                    Await Task.Delay(500)

                Catch ex As Exception
                    logger.Logger.LogError($"[LaunchHelper] Exception during attempt {attempt} {ex.Message}")
                End Try
            Next

            ' Final safety check — the process might have started on the last attempt
            If Process.GetProcessesByName(processNameToCheck).Length > 0 Then
                logger.Logger.LogInfo($"[LaunchHelper] {processNameToCheck} detected running after retries. Treating as success.")
                UtilManager.HideLaunchOverlay()
                Return True
            End If

            logger.Logger.LogError($"[LaunchHelper] Failed to start {processNameToCheck} after 2 attempts.")
            UtilManager.HideLaunchOverlay()
            Return False
        End Function
        Public Shared Sub ShowLaunchOverlay(parentForm As Form, Text As String)
            If LaunchOverlay Is Nothing Then
                LaunchOverlay = New Panel With {
                .BackColor = Color.FromArgb(128, Color.LightGray),
                .Dock = DockStyle.Fill,
                .Cursor = Cursors.WaitCursor
            }

                LaunchOverlayLabel = New Label With {
                .ForeColor = Color.Black,
                .Font = New Font("Segoe UI", 16, FontStyle.Bold),
                .BackColor = Color.Transparent,
                .AutoSize = True
            }

                ' Center the label after the overlay is added
                LaunchOverlay.Controls.Add(LaunchOverlayLabel)
                AddHandler LaunchOverlay.Resize, Sub()
                                                     If LaunchOverlay IsNot Nothing AndAlso LaunchOverlayLabel IsNot Nothing Then
                                                         LaunchOverlayLabel.Left = (LaunchOverlay.Width - LaunchOverlayLabel.Width) \ 2
                                                         LaunchOverlayLabel.Top = (LaunchOverlay.Height - LaunchOverlayLabel.Height) \ 2
                                                     End If
                                                 End Sub
            End If
            If LaunchOverlayLabel IsNot Nothing Then
                LaunchOverlayLabel.Text = Text
                LaunchOverlayLabel.Left = (LaunchOverlay.Width - LaunchOverlayLabel.Width) \ 2
                LaunchOverlayLabel.Top = (LaunchOverlay.Height - LaunchOverlayLabel.Height) \ 2
            End If

            If Not parentForm.Controls.Contains(LaunchOverlay) Then
                parentForm.Controls.Add(LaunchOverlay)
            End If

            LaunchOverlay.BringToFront()
            LaunchOverlay.Visible = True
        End Sub
        Public Shared Sub HideLaunchOverlay()
            If LaunchOverlay IsNot Nothing Then
                LaunchOverlay.Visible = False
            End If
        End Sub

        'Asynchronous method to wait for the emulator process's to start
        Private Async Function WaitForProcessWindowAsync(processNames As IEnumerable(Of String), tag As String, Optional timeoutMilliseconds As Integer = 10000) As Task(Of Boolean)

            Dim startTime As DateTime = DateTime.Now

            While (DateTime.Now - startTime).TotalMilliseconds < timeoutMilliseconds
                For Each name In processNames
                    Dim procs As Process() = Process.GetProcessesByName(name)
                    For Each proc In procs
                        Try
                            If proc.MainWindowHandle <> IntPtr.Zero Then
                                logger.Logger.LogInfo($"[{tag}] '{name}' process ready with MainWindowHandle.")
                                Return True
                            End If
                        Catch
                            ' Process may have exited between GetProcessesByName and handle check
                        End Try
                    Next
                Next

                Await Task.Delay(250)
            End While

            logger.Logger.LogWarning($"[{tag}] Timed out waiting for {String.Join("/", processNames)} window.")
            Return False
        End Function

        'EXE Get/Patchers
        Public Function ReadRawStringFromExe(exePath As String, offset As Long, Optional length As Integer = 37) As String
            If Not File.Exists(exePath) Then
                Throw New FileNotFoundException("EXE not found.", exePath)
            End If

            Using fs As New FileStream(exePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                If offset < 0 OrElse offset + length > fs.Length Then
                    Throw New ArgumentOutOfRangeException(
                    NameOf(offset),
                    $"Offset out of range. File length={fs.Length}"
                )
                End If

                fs.Seek(offset, SeekOrigin.Begin)

                Dim buffer(length - 1) As Byte
                fs.Read(buffer, 0, buffer.Length)

                Return Encoding.ASCII.GetString(buffer)
            End Using
        End Function
        Public Shared Function PatchTerminalAndUidInExe(exePath As String, offset As Long, terminalId As String, networkUid As String) As Boolean
            Try
                If Not File.Exists(exePath) Then Throw New FileNotFoundException("EXE not found.", exePath)
                terminalId = terminalId.Trim()
                networkUid = networkUid.Trim()

                If terminalId.Length <> 15 Then
                    Throw New ArgumentException($"TerminalID must be exactly 15 characters. Got {terminalId.Length}.")
                End If

                ' UID: truncate if too long, otherwise pad with '-' to exactly 20
                If networkUid.Length > 20 Then
                    networkUid = networkUid.Substring(0, 20)
                ElseIf networkUid.Length < 20 Then
                    networkUid = networkUid.PadRight(20, "-"c)
                End If

                Dim patchText As String = terminalId & "#" & networkUid & "#"
                Dim patchBytes As Byte() = Encoding.GetEncoding(932).GetBytes(patchText) ' 37 bytes

                Using bw As New BinaryWriter(File.Open(exePath, FileMode.Open), Encoding.GetEncoding(932))
                    bw.BaseStream.Position = offset
                    bw.Write(patchBytes)
                    bw.Flush()
                End Using
                logger.Logger.LogInfo($"Successfully patched EXE for UID/TID at offset {offset:X} in {exePath}")
                Return True
            Catch ex As Exception
                logger.Logger.LogError($"Failed to patch EXE for UID/TID: {ex.Message}")
                Return False
            End Try
        End Function

        'DOJA Helpers
        Public Async Function UpdatedDOJADrawSize(DOJALOCATION As String, width As Integer, height As Integer) As Task
            Dim deviceInfoFile As String = Path.Combine(DOJALOCATION, "lib", "skin", "deviceinfo", "device1")
            Dim newValue As String = $"device1,{width},{height},120,120"
            Await File.WriteAllTextAsync(deviceInfoFile, newValue)
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
                                       logger.Logger.LogError("'\bin' not found in path.")
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
                                   logger.Logger.LogError($"Error updating DOJA AppConfig: {ex.Message}")
                               End Try
                           End Sub)
        End Function
        Public Async Function ProcessDOJA3SPtoSCR(inputSpPath As String, Optional idkdoja2 As Boolean = False) As Task(Of List(Of String))

            Const HEADER_SIZE As Integer = &H40

            If String.IsNullOrWhiteSpace(inputSpPath) Then
                Throw New ArgumentException("inputSpPath is required.")
                Return Nothing
            End If

            If Not File.Exists(inputSpPath) Then
                Throw New FileNotFoundException("Input .sp file not found.", inputSpPath)
                Return Nothing
            End If

            Dim outputDir As String = Path.GetDirectoryName(inputSpPath)
            Dim baseName As String = Path.GetFileNameWithoutExtension(inputSpPath)

            ' 🔹 Async read
            Dim spData As Byte() = Await File.ReadAllBytesAsync(inputSpPath)

            If spData.Length < HEADER_SIZE Then
                Throw New InvalidDataException("File is smaller than the required 0x40 header.")
            End If

            ' Parse partition sizes
            Dim sizes As New List(Of UInteger)
            For offset As Integer = 0 To HEADER_SIZE - 1 Step 4
                Dim size As UInteger = BitConverter.ToUInt32(spData, offset)
                If size = &HFFFFFFFFUI Then Exit For
                sizes.Add(size)
            Next

            ' Validate total size
            Dim totalSize As ULong = 0
            For Each s In sizes
                totalSize += s
            Next

            If totalSize <> CULng(spData.Length - HEADER_SIZE) Then
                Throw New InvalidDataException("Header sizes do not match file length.")
            End If

            ' DoJa 2.x restriction
            If idkdoja2 AndAlso sizes.Count > 1 Then
                Throw New InvalidDataException(
                "DoJa 2.x allows only one or zero scratchpad partitions.")
            End If

            Dim writtenFiles As New List(Of String)
            Dim dataOffset As Integer = HEADER_SIZE

            For i As Integer = 0 To sizes.Count - 1
                Dim partSize As Integer = CInt(sizes(i))

                Dim outFileName As String =
                If(idkdoja2,
                   $"{baseName}.scr",
                   $"{baseName}{i}.scr")

                Dim outPath As String = Path.Combine(outputDir, outFileName)

                ' Skip existing files
                If File.Exists(outPath) Then
                    dataOffset += partSize
                    Continue For
                End If

                ' 🔹 Async write
                Using fs As New FileStream(
                outPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize:=81920,
                useAsync:=True)

                    Await fs.WriteAsync(spData, dataOffset, partSize)
                End Using

                writtenFiles.Add(outPath)
                dataOffset += partSize
            Next

            Return writtenFiles
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
        Public Async Function UpdatedSTARDrawSize(STARLOCATION As String, X As Integer, Y As Integer) As Task
            Dim device1InfoFile As String = Path.Combine(STARLOCATION, "lib", "skin", "deviceinfo", "device1")
            Dim newValue As String = $"device1,{X},{Y},120,120,0,2,0,1,3"
            Await File.WriteAllTextAsync(device1InfoFile, newValue)
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
                                       logger.Logger.LogError("'\bin' not found in path.")
                                       Return
                                   End If

                                   If Not File.Exists(appConfigFile) OrElse Not File.Exists(appConfigFCFile) Then
                                       MessageBox.Show("Missing STAR AppSettings files")
                                       Return
                                   End If

                                   File.Copy(appConfigFile, Path.Combine(gameDirectory, gameName), True)
                                   File.Copy(appConfigFCFile, Path.Combine(gameDirectory, $"{gameName}.fc"), True)

                               Catch ex As Exception
                                   logger.Logger.LogError($"Error updating STAR AppConfig: {ex.Message}")
                               End Try
                           End Sub)
        End Function

        'DOJA/STAR Helpers
        Public Async Function PrepareJamFileForLaunchAsync(
            jamFile As String,
            mode As String,
            networkUID As String,
            modifyJam As Boolean,
            modifyUrls As Boolean
        ) As Task

            If Not File.Exists(jamFile) Then
                logger.Logger.LogError($"JAM file not found: {jamFile}")
                Return
            End If

            Dim enc = Encoding.GetEncoding(932)
            Dim lines = (Await File.ReadAllLinesAsync(jamFile, enc)).ToList()
            Dim modified As Boolean = False

            ' DOJA3 special case: strip entries and exit early 
            If mode = "doja3" Then
                Dim entriesToRemove = {"TrustedAPID =", "MessageCode ="}
                Dim filtered = lines.Where(Function(l) Not entriesToRemove.Any(
            Function(e) l.StartsWith(e, StringComparison.OrdinalIgnoreCase))).ToList()

                If filtered.Count <> lines.Count Then
                    Await File.WriteAllLinesAsync(jamFile, filtered, enc)
                    logger.Logger.LogInfo($"Removed DOJA3 jam entries from {jamFile}")
                End If
                Return
            End If

            ' Normalize and ensure entries (if user allows JAM modifications)
            If modifyJam Then
                ' Remove empty lines
                Dim before = lines.Count
                lines = lines.Where(Function(l) Not String.IsNullOrWhiteSpace(l)).ToList()
                If lines.Count <> before Then modified = True

                ' Normalize spacing around equals signs
                Dim kvPattern As New Regex("^([^=]+?)(\s*=\s*)(.*)$")
                For i = 0 To lines.Count - 1
                    Dim m = kvPattern.Match(lines(i))
                    If m.Success Then
                        Dim key = m.Groups(1).Value.TrimEnd()
                        Dim value = m.Groups(3).Value.TrimStart()
                        Dim normalized = $"{key} = {value}"
                        If lines(i) <> normalized Then
                            lines(i) = normalized
                            modified = True
                        End If
                    End If
                Next

                ' Check MiniApp status
                Dim isMiniApp = lines.Any(Function(l) Regex.IsMatch(l, "^AppType\s*=\s*MiniApp$", RegexOptions.IgnoreCase))

                ' Remove MessageCode if MiniApp
                If isMiniApp Then
                    before = lines.Count
                    lines = lines.Where(Function(l) Not Regex.IsMatch(l, "^MessageCode\s*=", RegexOptions.IgnoreCase)).ToList()
                    If lines.Count < before Then modified = True
                End If

                ' Build required entries based on mode
                Dim requiredEntries As New Dictionary(Of String, String)

                If mode = "star" Then
                    requiredEntries.Add("UseNetwork", "yes")
                End If

                If Not isMiniApp Then
                    requiredEntries.Add("TrustedAPID", "00000000000")
                    requiredEntries.Add("MessageCode", "0000000000")
                End If

                For Each entry In requiredEntries
                    If Not lines.Any(Function(l) Regex.IsMatch(l, $"^{Regex.Escape(entry.Key)}\s*=")) Then
                        lines.Add($"{entry.Key} = {entry.Value}")
                        modified = True
                    End If
                Next

                ' Fix PackageURL
                Dim pkgPattern As New Regex("^PackageURL\s*=\s*(.+)$", RegexOptions.IgnoreCase)
                For i = 0 To lines.Count - 1
                    Dim m = pkgPattern.Match(lines(i))
                    If m.Success Then
                        Dim val = m.Groups(1).Value.Trim()
                        If Not val.StartsWith("http://", StringComparison.OrdinalIgnoreCase) AndAlso
                   Not val.StartsWith("https://", StringComparison.OrdinalIgnoreCase) Then
                            Dim fn = Path.GetFileName(val)
                            lines(i) = $"PackageURL = http://localhost/{Path.GetFileNameWithoutExtension(fn)}/{fn}"
                            modified = True
                        End If
                        Exit For
                    End If
                Next

                ' Fix SPsize spaces
                Dim spPattern As New Regex("^SPsize\s*=\s*(.+)$", RegexOptions.IgnoreCase)
                For i = 0 To lines.Count - 1
                    Dim m = spPattern.Match(lines(i))
                    If m.Success Then
                        Dim cleaned = m.Groups(1).Value.Replace(" "c, "")
                        If lines(i) <> $"SPsize = {cleaned}" Then
                            lines(i) = $"SPsize = {cleaned}"
                            modified = True
                        End If
                        Exit For
                    End If
                Next
            End If

            ' Replace NetworkUID 
            If Not String.IsNullOrWhiteSpace(networkUID) AndAlso
       Not networkUID.Equals("NULLGWDOCOMO", StringComparison.OrdinalIgnoreCase) Then
                For i = 0 To lines.Count - 1
                    If Regex.IsMatch(lines(i), "NULLGWDOCOMO", RegexOptions.IgnoreCase) Then
                        lines(i) = Regex.Replace(lines(i), "NULLGWDOCOMO", networkUID, RegexOptions.IgnoreCase)
                        modified = True
                    End If
                Next
            End If

            ' Rewrite network URLs
            If modifyUrls Then
                Dim domainMap = GetNetworkDomainMap()
                Dim urlRegex As New Regex("(https?://)([^/]+)", RegexOptions.IgnoreCase Or RegexOptions.Compiled)

                For i = 0 To lines.Count - 1
                    If urlRegex.IsMatch(lines(i)) Then
                        Dim original = lines(i)
                        lines(i) = urlRegex.Replace(lines(i), Function(m)
                                                                  Dim host = m.Groups(2).Value
                                                                  For Each kvp In domainMap
                                                                      If host.EndsWith(kvp.Key, StringComparison.OrdinalIgnoreCase) Then
                                                                          Return m.Groups(1).Value & kvp.Value
                                                                      End If
                                                                  Next
                                                                  Return m.Value
                                                              End Function)
                        If lines(i) <> original Then modified = True
                    End If
                Next
            End If

            If modified Then
                Await File.WriteAllLinesAsync(jamFile, lines, enc)
                logger.Logger.LogInfo($"JAM file updated: {jamFile}")
            Else
                logger.Logger.LogInfo($"JAM file unchanged: {jamFile}")
            End If
        End Function
        Public Async Function UpdateSoundConf(emulatorLocation As String, soundType As String) As Task
            If soundType = "903i-HP" Then
                soundType = "903i"
            End If

            Dim soundPath = Path.Combine(emulatorLocation, "lib", "SoundConf.properties")
            If Not File.Exists(soundPath) Then
                logger.Logger.LogError($"File not found: {soundPath}")
                Return
            End If

            Try
                Dim enc = Encoding.GetEncoding("shift-jis")
                Dim conf = Await File.ReadAllTextAsync(soundPath, enc)

                conf = Regex.Replace(conf, "MODE=.", "MODE=0")
                Dim soundLibValue = If(soundType = "903i", "002", "001")
                conf = Regex.Replace(conf, "SOUNDLIB=...", $"SOUNDLIB={soundLibValue}")

                Await File.WriteAllTextAsync(soundPath, conf, enc)
            Catch ex As Exception
                logger.Logger.LogError($"Error updating sound configuration: {ex.Message}")
            End Try
        End Function
        Public Function ExtractDrawAreaFromJam(filePath As String, Optional defaultWidth As Integer = 240, Optional defaultHeight As Integer = 240) As (Integer, Integer)
            Dim width = defaultWidth
            Dim height = defaultHeight

            Try
                For Each line In File.ReadLines(filePath, Encoding.GetEncoding("shift-jis"))
                    If line.StartsWith("DrawArea =", StringComparison.OrdinalIgnoreCase) Then
                        Dim parts = line.Split("="c)(1).Trim().Split("x"c)
                        If parts.Length = 2 Then
                            width = Convert.ToInt32(parts(0).Trim())
                            height = Convert.ToInt32(parts(1).Trim())
                        End If
                        Exit For
                    End If
                Next
            Catch ex As Exception
                logger.Logger.LogError($"Error reading draw area: {ex.Message}")
            End Try

            Return (width, height)
        End Function
        Public Async Function UpdateDeviceSkin(emulatorLocation As String, emulatorType As String, hideUI As Boolean) As Task(Of Boolean)
            Return Await Task.Run(Function()
                                      Try
                                          ' Target: {emulatorLocation}/lib/skin/device1
                                          Dim targetSkinFolder = Path.Combine(emulatorLocation, "lib", "skin", "device1")

                                          ' Clear or create target folder
                                          If Directory.Exists(targetSkinFolder) Then
                                              For Each f In Directory.GetFiles(targetSkinFolder, "*.*", SearchOption.TopDirectoryOnly)
                                                  File.Delete(f)
                                              Next
                                          Else
                                              Directory.CreateDirectory(targetSkinFolder)
                                          End If

                                          ' Build source skin path based on emulator type
                                          Dim baseSkinsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "tools", "skins")
                                          Dim sourceFolder As String

                                          Select Case emulatorType.ToLower()
                                              Case "doja"
                                                  Dim sdkFolderName = Path.GetFileName(emulatorLocation)
                                                  Dim uiSubfolder = If(hideUI, "noui", "ui")
                                                  sourceFolder = Path.Combine(baseSkinsFolder, "doja", uiSubfolder, sdkFolderName)

                                              Case "star"
                                                  Dim skinSubfolder = If(hideUI, "noui\star-device1-noui", "ui\star-device1-ui")
                                                  sourceFolder = Path.Combine(baseSkinsFolder, "star", skinSubfolder)

                                              Case Else
                                                  logger.Logger.LogWarning($"[Skin] Unknown emulator type: {emulatorType}")
                                                  Return False
                                          End Select

                                          ' Validate source exists
                                          If Not Directory.Exists(sourceFolder) Then
                                              logger.Logger.LogError($"Skin folder missing: {sourceFolder}")
                                              MessageBox.Show($"Skin folder missing: {sourceFolder}")
                                              Return False
                                          End If

                                          ' Copy skins
                                          For Each skinFile In Directory.GetFiles(sourceFolder)
                                              File.Copy(skinFile, Path.Combine(targetSkinFolder, Path.GetFileName(skinFile)), True)
                                          Next

                                          Return True
                                      Catch ex As Exception
                                          logger.Logger.LogError($"[Skin Update Error] {emulatorType}: {ex.Message}")
                                          Return False
                                      End Try
                                  End Function)
        End Function
        Public Async Function EnableDisableHardwareRendering(STARLOCATION As String, DOJALOCATION As String, TOOLFOLDER As String, EnableDisable As Boolean) As Task
            Await Task.Run(
        Sub()
            Dim HardwareRenderFolder As String = Path.Combine(TOOLFOLDER, "highperformancemodules", "hardware")

            Dim STARBinFolder As String = Path.Combine(STARLOCATION, "bin")
            Dim binLibEGL As String = Path.Combine(STARBinFolder, "libEGL.dll")
            Dim binLibGLES As String = Path.Combine(STARBinFolder, "libGLES_CM_NoE.dll")
            Dim binINI As String = Path.Combine(STARBinFolder, "gles_hw_accel.ini")
            Dim binLibEGLOrig As String = Path.Combine(STARBinFolder, "libEGL_orig.dll")
            Dim binLibGLESOrig As String = Path.Combine(STARBinFolder, "libGLES_CM_NoE_orig.dll")


            Dim hardwareLibEGL As String = Path.Combine(HardwareRenderFolder, "libEGL.dll")
            Dim hardwareLibGLES As String = Path.Combine(HardwareRenderFolder, "libGLES_CM_NoE.dll")
            Dim hardwareini As String = Path.Combine(HardwareRenderFolder, "gles_hw_accel.ini")


            If Not Directory.Exists(STARBinFolder) Then
                Throw New DirectoryNotFoundException($"STAR bin folder not found: {STARBinFolder}")
            End If

            If EnableDisable Then
                ' Enable hardware rendering

                Try
                    If Not Directory.Exists(HardwareRenderFolder) Then
                        Throw New DirectoryNotFoundException($"Hardware rendering folder not found: {HardwareRenderFolder}")
                    End If

                    If Not File.Exists(hardwareLibEGL) OrElse Not File.Exists(hardwareLibGLES) Then
                        Throw New FileNotFoundException("One or more hardware rendering DLLs are missing.")
                    End If
                Catch ex As Exception
                    MessageBox.Show($"Cannot enable hardware rendering: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End Try

                ' Rename original DLLs only if they exist and backups do not already exist
                If File.Exists(binLibEGL) AndAlso Not File.Exists(binLibEGLOrig) Then
                    File.Move(binLibEGL, binLibEGLOrig)
                End If

                If File.Exists(binLibGLES) AndAlso Not File.Exists(binLibGLESOrig) Then
                    File.Move(binLibGLES, binLibGLESOrig)
                End If

                ' Remove current hardware DLLs if already present so copy succeeds cleanly
                If File.Exists(binLibEGL) Then
                    File.Delete(binLibEGL)
                End If

                If File.Exists(binLibGLES) Then
                    File.Delete(binLibGLES)
                End If

                ' Copy hardware DLLs into bin
                File.Copy(hardwareLibEGL, binLibEGL, True)
                File.Copy(hardwareLibGLES, binLibGLES, True)
                File.Copy(hardwareini, binINI, True)
            Else
                ' Disable hardware rendering / restore software rendering
                ' Delete hardware DLLs currently in bin
                If File.Exists(binLibEGL) Then
                    File.Delete(binLibEGL)
                End If

                If File.Exists(binLibGLES) Then
                    File.Delete(binLibGLES)
                End If

                If File.Exists(binINI) Then
                    File.Delete(binINI)
                End If

                ' Restore originals if backups exist
                If File.Exists(binLibEGLOrig) Then
                    File.Move(binLibEGLOrig, binLibEGL)
                End If

                If File.Exists(binLibGLESOrig) Then
                    File.Move(binLibGLESOrig, binLibGLES)
                End If
            End If
        End Sub)
        End Function
        Public Async Function EnableDisableHP903iSound(STARLOCATION As String, DOJALOCATION As String, TOOLFOLDER As String, EnableDisable As Boolean) As Task
            Dim SoundHPFolder As String = Path.Combine(TOOLFOLDER, "highperformancemodules", "sound")
            Dim emulatorLocations() As String = {STARLOCATION, DOJALOCATION}

            For Each emulatorLocation As String In emulatorLocations
                Dim targetSoundFolder As String = Path.Combine(emulatorLocation, "bin", "soundlib", "lib002", "Lib")
                Dim sourceDLL As String = Path.Combine(targetSoundFolder, "MFiSoundLibMFi5.dll")
                Dim sourceORIGDLL As String = Path.Combine(targetSoundFolder, "MFiSoundLibMFi5_orig.dll")
                Dim hpDLL As String = Path.Combine(SoundHPFolder, "MFiSoundLibMFi5.dll")

                If EnableDisable Then
                    ' Enable HP: back up original, then copy HP DLL in
                    If File.Exists(sourceDLL) AndAlso Not File.Exists(sourceORIGDLL) Then
                        File.Move(sourceDLL, sourceORIGDLL)
                    End If

                    If File.Exists(hpDLL) Then
                        File.Copy(hpDLL, sourceDLL, True)
                    End If
                Else
                    ' Disable HP: restore the original DLL
                    If File.Exists(sourceORIGDLL) Then
                        If File.Exists(sourceDLL) Then
                            File.Delete(sourceDLL)
                        End If
                        File.Move(sourceORIGDLL, sourceDLL)
                    End If
                End If
            Next
        End Function
        Public Async Function EnableDisableHighPerformanceEmulators(STARLOCATION As String, DOJALOCATION As String, TOOLFOLDER As String, EnableDisable As Boolean) As Task
            Dim DojaBinFolder As String = Path.Combine(DOJALOCATION, "bin")
            Dim StarBinFolder As String = Path.Combine(STARLOCATION, "bin")

            'High Performance Files
            Dim HPStarFile As String = Path.Combine(TOOLFOLDER, "highperformancemodules", "hardware", "star.exe")
            Dim HPFullFile As String = Path.Combine(TOOLFOLDER, "highperformancemodules", "hardware", "full.exe")
            Dim HPDojaFile As String = Path.Combine(TOOLFOLDER, "highperformancemodules", "hardware", "doja.exe")

            'Current files in bin folders
            Dim StarExe As String = Path.Combine(StarBinFolder, "star.exe")
            Dim FullExe As String = Path.Combine(StarBinFolder, "full.exe")
            Dim PerfDat As String = Path.Combine(StarBinFolder, "_performance.dat")
            Dim DojaExe As String = Path.Combine(DojaBinFolder, "doja.exe")

            'Renamed originals
            Dim StarOrigExe As String = Path.Combine(StarBinFolder, "star_orig.exe")
            Dim FullOrigExe As String = Path.Combine(StarBinFolder, "full_orig.exe")
            Dim PerfOrigDat As String = Path.Combine(StarBinFolder, "_performance_orig.dat")
            Dim DojaOrigExe As String = Path.Combine(DojaBinFolder, "doja_orig.exe")

            Await Task.Run(
        Sub()
            If EnableDisable Then
                ' --- ENABLE: back up originals, then copy HP files in ---

                ' Rename originals in StarBinFolder
                If File.Exists(StarExe) AndAlso Not File.Exists(StarOrigExe) Then
                    File.Move(StarExe, StarOrigExe)
                End If

                If File.Exists(FullExe) AndAlso Not File.Exists(FullOrigExe) Then
                    File.Move(FullExe, FullOrigExe)
                End If

                If File.Exists(PerfDat) AndAlso Not File.Exists(PerfOrigDat) Then
                    File.Move(PerfDat, PerfOrigDat)
                End If

                ' Rename original in DojaBinFolder
                If File.Exists(DojaExe) AndAlso Not File.Exists(DojaOrigExe) Then
                    File.Move(DojaExe, DojaOrigExe)
                End If

                ' Copy high performance files in
                File.Copy(HPStarFile, StarExe, overwrite:=True)
                File.Copy(HPFullFile, FullExe, overwrite:=True)
                File.Copy(HPDojaFile, DojaExe, overwrite:=True)

            Else
                ' --- DISABLE: remove HP files, restore originals ---

                ' Delete the HP copies
                If File.Exists(StarExe) Then File.Delete(StarExe)
                If File.Exists(FullExe) Then File.Delete(FullExe)
                If File.Exists(DojaExe) Then File.Delete(DojaExe)

                ' Restore originals in StarBinFolder
                If File.Exists(StarOrigExe) Then
                    File.Move(StarOrigExe, StarExe)
                End If

                If File.Exists(FullOrigExe) Then
                    File.Move(FullOrigExe, FullExe)
                End If

                If File.Exists(PerfOrigDat) Then
                    File.Move(PerfOrigDat, PerfDat)
                End If

                ' Restore original in DojaBinFolder
                If File.Exists(DojaOrigExe) Then
                    File.Move(DojaOrigExe, DojaExe)
                End If
            End If
        End Sub)
        End Function

        'EZWeb Helper
        Private Shared Function ExtractKjxMetadata(kjxPath As String) As String()
            Try
                Dim raw As Byte() = File.ReadAllBytes(kjxPath)

                ' Find start of metadata (first MIDlet or MicroEdition key)
                Dim metaStart As Integer = -1
                Dim searchTerms = {"MIDlet-", "MicroEdition-"}

                For Each term In searchTerms
                    Dim termBytes = Encoding.ASCII.GetBytes(term)
                    For i = 0 To raw.Length - termBytes.Length
                        Dim match = True
                        For j = 0 To termBytes.Length - 1
                            If raw(i + j) <> termBytes(j) Then
                                match = False
                                Exit For
                            End If
                        Next
                        If match Then
                            If metaStart = -1 OrElse i < metaStart Then
                                metaStart = i
                            End If
                            Exit For
                        End If
                    Next
                Next

                If metaStart = -1 Then Return Array.Empty(Of String)()

                ' Find end of metadata (PK zip signature)
                Dim metaEnd As Integer = raw.Length
                For i = metaStart To raw.Length - 2
                    If raw(i) = &H50 AndAlso raw(i + 1) = &H4B Then  ' "PK"
                        metaEnd = i
                        Exit For
                    End If
                Next

                ' Decode and split into lines, keeping only key:value metadata lines
                Dim text As String = Encoding.ASCII.GetString(raw, metaStart, metaEnd - metaStart)
                Return text.Split({vbLf(0), vbCr(0)}, StringSplitOptions.RemoveEmptyEntries) _
                   .Where(Function(line) line.Contains(":"c)) _
                   .ToArray()

            Catch ex As Exception
                logger.Logger.LogError($"Failed to extract KJX metadata from {kjxPath}: {ex.Message}")
                Return Array.Empty(Of String)()
            End Try
        End Function

        'JSKY Helpers
        Public Shared Async Function UpdateJADJarURLAsync(jadFilePath As String) As Task(Of Boolean)
            If Not File.Exists(jadFilePath) Then
                logger.Logger.LogError($"JAD file not found: {jadFilePath}")
                Return False
            End If

            Try
                Dim enc As Encoding
                If Path.GetExtension(jadFilePath).Equals(".jad", StringComparison.OrdinalIgnoreCase) Then
                    enc = Encoding.UTF8
                Else
                    enc = Encoding.GetEncoding(932)
                End If
                Dim lines As String() = Await File.ReadAllLinesAsync(jadFilePath, enc)
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
                    Await File.WriteAllLinesAsync(jadFilePath, updatedLines, enc)
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
        Public Async Function AddJADMIDletEntriesAsync(txtFilePath As String, uid As String, terminalId As String) As Task
            Dim enc As Encoding
            If Path.GetExtension(txtFilePath).Equals(".jad", StringComparison.OrdinalIgnoreCase) Then
                enc = Encoding.UTF8
            Else
                enc = Encoding.GetEncoding("Shift_JIS")
            End If

            Dim lines As New List(Of String)(Await File.ReadAllLinesAsync(txtFilePath, enc))

            Dim hasUID As Boolean = lines.Any(Function(l) l.StartsWith("MIDlet_UID:"))
            Dim hasUCODE As Boolean = lines.Any(Function(l) l.StartsWith("MIDlet_UCODE:"))

            If Not hasUID Then
                lines.Add("MIDlet_UID: " & uid)
            End If

            If Not hasUCODE Then
                lines.Add("MIDlet_UCODE: " & terminalId)
            End If

            If Not hasUID OrElse Not hasUCODE Then
                Await File.WriteAllLinesAsync(txtFilePath, lines, enc)
            End If
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
                    updatedLines.Add("DefaultFont=\uFF2D\uFF33 \u30B4\u30B7\u30C3\u30AF")
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
            Await UpdateShaderGlassConfig(argumentFile, captureWindowName:=AppName, scalingSelection:=MainForm.cbxShaderGlassScaling.SelectedItem.ToString())

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
        Public Function GetCurrentSetShader() As String
            Dim baseDir As String = AppDomain.CurrentDomain.BaseDirectory
            Dim ShaderGlassProfile As String = Path.Combine(baseDir, "data", "tools", "shaderglass", "keitai.sgp")
            Dim shaderCategory As String = ""
            Dim shaderName As String = ""
            For Each line As String In IO.File.ReadAllLines(ShaderGlassProfile)
                Dim trimmed = line.Trim()
                If trimmed.StartsWith("ShaderCategory ") Then
                    shaderCategory = trimmed.Substring("ShaderCategory ".Length).Trim(""""c)
                ElseIf trimmed.StartsWith("ShaderName ") Then
                    shaderName = trimmed.Substring("ShaderName ".Length).Trim(""""c)
                End If
            Next
            Return $"{shaderCategory}//{shaderName}"
        End Function
        Public Sub SetCurrentShader(CombinedShader As String)
            Dim baseDir As String = AppDomain.CurrentDomain.BaseDirectory
            Dim ShaderGlassProfile As String = Path.Combine(baseDir, "data", "tools", "shaderglass", "keitai.sgp")
            Dim separatorIndex As Integer = CombinedShader.IndexOf("//")
            If separatorIndex < 0 Then Return
            Dim shaderCategory As String = CombinedShader.Substring(0, separatorIndex)
            Dim shaderName As String = CombinedShader.Substring(separatorIndex + 2)
            Dim lines() As String = IO.File.ReadAllLines(ShaderGlassProfile)
            For i As Integer = 0 To lines.Length - 1
                Dim trimmed = lines(i).Trim()
                If trimmed.StartsWith("ShaderCategory ") Then
                    lines(i) = $"ShaderCategory ""{shaderCategory}"""
                ElseIf trimmed.StartsWith("ShaderName ") Then
                    lines(i) = $"ShaderName ""{shaderName}"""
                End If
            Next
            IO.File.WriteAllLines(ShaderGlassProfile, lines)
        End Sub
        Public Async Function UpdateShaderGlassConfig(filePath As String, Optional captureWindowName As String = Nothing, Optional shaderName As String = Nothing, Optional scalingSelection As String = Nothing) As Task

            If Not File.Exists(filePath) Then
                logger.Logger.LogError($"ShaderGlass config file not found: {filePath}")
                Return
            End If

            ' Resolve scaling text to integer value
            Dim scaleValue As Integer = -1
            If scalingSelection IsNot Nothing Then
                Select Case scalingSelection
                    Case "1x" : scaleValue = 100
                    Case "1.5x" : scaleValue = 150
                    Case "2x" : scaleValue = 200
                    Case "2.5x" : scaleValue = 250
                    Case "3x" : scaleValue = 300
                    Case "3.5x" : scaleValue = 350
                    Case "4x" : scaleValue = 400
                    Case Else : scaleValue = 100
                End Select
            End If

            Dim lines = (Await File.ReadAllLinesAsync(filePath)).ToList()

            For i = 0 To lines.Count - 1
                If captureWindowName IsNot Nothing AndAlso lines(i).StartsWith("CaptureWindow") Then
                    lines(i) = $"CaptureWindow ""{captureWindowName}"""

                ElseIf shaderName IsNot Nothing AndAlso lines(i).StartsWith("ShaderName") Then
                    lines(i) = $"ShaderName ""{shaderName.ToLower()}"""

                ElseIf scaleValue >= 0 AndAlso lines(i).StartsWith("OutputScale") Then
                    lines(i) = $"OutputScale ""{scaleValue}"""
                End If
            Next

            Await File.WriteAllLinesAsync(filePath, lines)
        End Function

        'Network Helpers
        Public Shared Function GetNetworkDomainMap() As Dictionary(Of String, String)
            Return New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase) From {
                {"mbga.jp", "mobage.gs.keitaiarchive.org"},
                {"gree.jp", "gree.gs.keitaiarchive.org"},
                {"m-app.jp", "moco.gs.keitaiarchive.org"},
                {"i-simple100.channel.or.jp", "simple100.gs.keitaiarchive.org"},
                {"*.shiftup.net", "shiftup.gs.keitaiarchive.org"},
                {"*.gp.commseed.jp", "upss.gs.keitaiarchive.org"},
                {"igc.cave.co.jp", "cave.gs.keitaiarchive.org"},
                {"icd.i.konami.net", "konami.gs.keitaiarchive.org"}
            }
        End Function
    End Class
End Namespace
