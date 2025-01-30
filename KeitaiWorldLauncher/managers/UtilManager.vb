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

Namespace My.Managers
    Public Class UtilManager
        Public Shared Sub SetupDIRS()
            Directory.CreateDirectory("data")
            Directory.CreateDirectory("configs")
            Directory.CreateDirectory("logs")
            Directory.CreateDirectory("data\downloads")
        End Sub

        'PreReq Checks
        Shared Sub CheckforPreReq()
            Dim DOJAEmulator = "data\tools\iDKDoJa5.1\bin\doja.exe"
            Dim localeEmu = "data\tools\locale_emulator\LEProc.exe"


            'Check for DOJA
            If File.Exists(DOJAEmulator) = False Then
                MessageBox.Show("Missing DOJA Emulator... Download is required")
            End If

            'Check for LEProc
            If File.Exists(localeEmu) = False Then
                MessageBox.Show("Missing Locale Emulator... Download is required")
            End If
        End Sub

        'Check for APP Updates
        Shared Sub CheckForUpdates(latestVersionUrl As String)
            Dim currentVersion As String = KeitaiWorldLauncher.My.Application.Info.Version.ToString ' Get the current version of the app

            Try
                Dim client As New WebClient()
                Dim latestVersion As String = client.DownloadString(latestVersionUrl).Trim()

                If currentVersion <> latestVersion Then
                    Dim result = MessageBox.Show($"A new version ({latestVersion}) is available. Would you like to update?", "Update Available", MessageBoxButtons.YesNo, MessageBoxIcon.Information)

                    If result = DialogResult.Yes Then
                        Process.Start("https://example.com/download") ' Link to download the new version
                    End If
                Else
                    'MessageBox.Show("Launcher is up to date.", "No Updates", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            Catch ex As Exception
                MessageBox.Show($"Failed to check for updates. Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        'Generate Controls
        Public Shared Sub GenerateDynamicControlsFromLines(JAMFile As String, groupBox As GroupBox)
            Try
                ' Clear any existing controls in the GroupBox
                groupBox.Controls.Clear()

                ' Start position for dynamic controls
                Dim startY As Integer = 25
                Dim spacing As Integer = 30

                Dim lines = File.ReadAllLines(JAMFile, Encoding.GetEncoding(932))

                ' Loop through each line
                For Each line As String In lines
                    ' Skip empty lines
                    If String.IsNullOrWhiteSpace(line) Then Continue For

                    ' Split the line into key and value
                    Dim parts As String() = line.Split(New Char() {"="c}, 2)
                    If parts.Length <> 2 Then Continue For ' Skip invalid lines

                    Dim key As String = parts(0).Trim()
                    Dim value As String = parts(1).Trim()

                    ' Create a label for the key
                    Dim lbl As New Label()
                    lbl.Text = key
                    lbl.AutoSize = True
                    lbl.Location = New Point(5, startY)

                    ' Create a textbox for the value
                    Dim txt As New TextBox()
                    txt.Text = value
                    txt.Width = groupBox.Width - 50 ' Adjust width to fit within the group box
                    txt.Location = New Point(120, startY)

                    ' Add controls to the group box
                    groupBox.Controls.Add(lbl)
                    groupBox.Controls.Add(txt)

                    ' Move to the next row
                    startY += spacing
                Next
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
        Shared Function CheckAndCloseDoja()
            ' Check if doja.exe is currently running
            Dim dojaProcesses = Process.GetProcessesByName("doja")

            If dojaProcesses.Length > 0 Then
                ' Prompt the user to confirm closing the application
                Dim result = MessageBox.Show("doja.exe is currently running. Do you want to close it?",
                                     "Confirm Close",
                                     MessageBoxButtons.YesNo,
                                     MessageBoxIcon.Question)
                If result = DialogResult.Yes Then
                    Try
                        CheckAndCloseShaderGlass()
                        ' Close each instance of doja.exe
                        For Each process As Process In dojaProcesses
                            process.Kill()
                            process.WaitForExit() ' Ensure the process has exited
                        Next
                        Return False
                        'MessageBox.Show("doja.exe has been closed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Catch ex As Exception
                        MessageBox.Show("An error occurred while trying to close doja.exe: " & ex.Message,
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error)
                    End Try
                Else
                    Return True
                    'MessageBox.Show("doja.exe will remain running.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            Else
                'MessageBox.Show("doja.exe is not currently running.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End Function
        Shared Function CheckAndCloseStar()
            ' Check if doja.exe is currently running
            Dim dojaProcesses = Process.GetProcessesByName("star")
            If dojaProcesses.Length > 0 Then
                ' Prompt the user to confirm closing the application
                Dim result = MessageBox.Show("star.exe is currently running. Do you want to close it?",
                                     "Confirm Close",
                                     MessageBoxButtons.YesNo,
                                     MessageBoxIcon.Question)
                If result = DialogResult.Yes Then
                    Try
                        CheckAndCloseShaderGlass()

                        ' Close each instance of star.exe
                        For Each process As Process In dojaProcesses
                            process.Kill()
                            process.WaitForExit() ' Ensure the process has exited
                        Next
                        Return False
                        'MessageBox.Show("star.exe has been closed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Catch ex As Exception
                        MessageBox.Show("An error occurred while trying to close star.exe: " & ex.Message,
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error)
                    End Try
                Else
                    Return True
                    'MessageBox.Show("star.exe will remain running.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            Else
                'MessageBox.Show("star.exe is not currently running.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End Function
        Shared Function CheckAndCloseShaderGlass()
            ' Check if doja.exe is currently running
            Dim shaderglassProcesses = Process.GetProcessesByName("shaderglass")

            If shaderglassProcesses.Length > 0 Then
                Try
                    ' Close each instance of star.exe
                    For Each process As Process In shaderglassProcesses
                        process.Kill()
                        process.WaitForExit() ' Ensure the process has exited
                    Next
                    Return False
                    'MessageBox.Show("star.exe has been closed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch ex As Exception
                    MessageBox.Show("An error occurred while trying to close star.exe: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
                End Try
            Else
                'MessageBox.Show("star.exe is not currently running.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End Function
        Public Sub LaunchApplication(appPath As String, arguments As String)
            Try
                ' Create a new process start info
                Dim startInfo As New ProcessStartInfo()
                startInfo.FileName = appPath
                startInfo.Arguments = arguments

                ' Launch the application
                Dim process As Process = Process.Start(startInfo)

                MessageBox.Show($"Application launched successfully: {appPath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show($"Failed to launch the application: {ex.Message}", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
        Public Async Sub LaunchCustomDOJAGameCommand(DOJAPATH, DOJAEXELocation, GameJAM)
            Try
                ' Paths and arguments
                Dim appPath As String = AppDomain.CurrentDomain.BaseDirectory & "data\tools\locale_emulator\LEProc.exe"
                Dim guidArg As String = "-runas ad1a7fe1-4f95-45ba-b563-9ba60c3642d3"
                Dim dojaexePath As String = DOJAEXELocation
                Dim jamPath As String = AppDomain.CurrentDomain.BaseDirectory & GameJAM

                ' Combine arguments
                Dim arguments As String = $"{guidArg} ""{dojaexePath}"" -i ""{jamPath}"""

                'Update Device Launch Settings
                If Form1.chkbxHidePhoneUI.Checked = True Then
                    UpdateDOJADeviceSkin(DOJAPATH, True)
                Else
                    UpdateDOJADeviceSkin(DOJAPATH, False)
                End If
                'Update Device Draw Size
                Dim JAMDrawArea = ExtractDOJAWidthHeight(jamPath)
                UpdatedDOJADrawSize(DOJAPATH, JAMDrawArea.Item1, JAMDrawArea.Item2)
                'Updated SoundType
                UpdateDOJASoundConf(DOJAPATH, Form1.cobxAudioType.SelectedItem.ToString)

                ' Set up process start info
                Dim startInfo As New ProcessStartInfo()
                startInfo.FileName = appPath
                startInfo.Arguments = arguments
                startInfo.UseShellExecute = True
                startInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory ' Set working directory

                ' Start the process
                Dim process As Process = Process.Start(startInfo)
                ' Ensure Doja has started properly
                process.WaitForInputIdle() ' Wait until the app is ready

                ' Start ShaderGlass if selected
                If Form1.chkbxShaderGlass.Checked Then
                    ' Wait asynchronously for the "STAR" process to be detected
                    Dim isRunning As Boolean = Await WaitForDojaToStart()

                    If isRunning Then
                        ' Launch ShaderGlass after Doja is running
                        LaunchShaderGlass(Path.GetFileNameWithoutExtension(jamPath))
                    Else
                        MessageBox.Show("Failed to detect STAR running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                End If
                'MessageBox.Show("Command launched successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show($"Failed to launch the command: {ex.Message}", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
        Public Async Sub LaunchCustomSTARGameCommand(STARPATH, STAREXELocation, GameJAM)
            Try
                ' Paths and arguments
                Dim appPath As String = AppDomain.CurrentDomain.BaseDirectory & "data\tools\locale_emulator\LEProc.exe"
                Dim guidArg As String = "-runas ad1a7fe1-4f95-45ba-b563-9ba60c3642d3"
                Dim dojaexePath As String = STAREXELocation
                Dim jamPath As String = AppDomain.CurrentDomain.BaseDirectory & GameJAM

                ' Combine arguments
                Dim arguments As String = $"{guidArg} ""{dojaexePath}"" -i ""{jamPath}"""

                'Update Device Launch Settings
                If Form1.chkbxHidePhoneUI.Checked = True Then
                    UpdateSTARDeviceSkin(STARPATH, True)
                Else
                    UpdateSTARDeviceSkin(STARPATH, False)
                End If
                'Update Device Draw Size
                Dim JAMDrawArea = ExtractSTARWidthHeight(jamPath)
                UpdatedSTARDrawSize(STARPATH, JAMDrawArea.Item1, JAMDrawArea.Item2)
                'Updated SoundType
                UpdateSTARSoundConf(STARPATH, Form1.cobxAudioType.SelectedItem.ToString)
                'Set AppSettings
                UpdateSTARAppconfig(STARPATH, GameJAM) 'this setups opengl and more to ensure everything works
                EnsureSTARJamFileEntries(GameJAM)

                ' Set up process start info
                Dim startInfo As New ProcessStartInfo()
                startInfo.FileName = appPath
                startInfo.Arguments = arguments
                startInfo.UseShellExecute = False
                startInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory ' Set working directory

                ' Start the process
                Dim process As Process = Process.Start(startInfo)
                ' Ensure STAR has started properly
                process.WaitForInputIdle() ' Wait until the app is ready

                ' Start ShaderGlass if selected
                If Form1.chkbxShaderGlass.Checked Then
                    ' Wait asynchronously for the "STAR" process to be detected
                    Dim isRunning As Boolean = Await WaitForSTARToStart()

                    If isRunning Then
                        ' Launch ShaderGlass after Doja is running
                        LaunchShaderGlass(Path.GetFileNameWithoutExtension(jamPath))
                    Else
                        MessageBox.Show("Failed to detect STAR running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                End If
                'MessageBox.Show("Command launched successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show($"Failed to launch the command: {ex.Message}", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
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
        Public Sub LaunchShaderGlass(AppName As String)
            Thread.Sleep(1000)
            Dim appPath As String = AppDomain.CurrentDomain.BaseDirectory & "data\tools\shaderglass\ShaderGlass.exe"
            Dim arguments As String = AppDomain.CurrentDomain.BaseDirectory & "data\tools\shaderglass\keitai.sgp"
            ModifyCaptureWindow(arguments, AppName)
            Dim startInfo As New ProcessStartInfo()
            startInfo.FileName = appPath
            startInfo.Arguments = arguments
            startInfo.UseShellExecute = True
            startInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory ' Set working directory

            ' Start the process
            Dim process As Process = Process.Start(startInfo)
        End Sub
        Public Sub ModifyCaptureWindow(filePath As String, AppName As String)
            ' Read all lines from the file
            Dim lines As List(Of String) = File.ReadAllLines(filePath).ToList()

            ' Find and modify the line that starts with "CaptureWindow"
            For i As Integer = 0 To lines.Count - 1
                If lines(i).StartsWith("CaptureWindow") Then
                    ' Replace the line with the new value
                    lines(i) = $"CaptureWindow ""{AppName}"""
                    Exit For ' Stop after finding the first occurrence
                End If
            Next

            ' Write the modified lines back to the file
            File.WriteAllLines(filePath, lines)
        End Sub

        ' Asynchronous method to wait for the "doja/star" process to start
        Private Async Function WaitForDojaToStart(Optional timeoutMilliseconds As Integer = 10000) As Task(Of Boolean)
            Dim startTime As DateTime = DateTime.Now

            ' Check periodically if the "doja" process is running
            While (DateTime.Now - startTime).TotalMilliseconds < timeoutMilliseconds
                If IsProcessRunning("doja") Then
                    Return True ' Process found, return success
                End If

                Await Task.Delay(500) ' Wait 500 ms before checking again (non-blocking)
            End While

            Return False ' Timed out, process not found
        End Function
        Private Async Function WaitForSTARToStart(Optional timeoutMilliseconds As Integer = 10000) As Task(Of Boolean)
            Dim startTime As DateTime = DateTime.Now

            ' Check periodically if the "doja" process is running
            While (DateTime.Now - startTime).TotalMilliseconds < timeoutMilliseconds
                If IsProcessRunning("star") Then
                    Return True ' Process found, return success
                End If

                Await Task.Delay(500) ' Wait 500 ms before checking again (non-blocking)
            End While

            Return False ' Timed out, process not found
        End Function

        'DOJA EXTRAS
        Public Sub UpdateDOJADeviceSkin(DOJALOCATION As String, hideUI As Boolean)
            Dim DojaSkinFolder = $"{DOJALOCATION}\lib\skin\device1"
            If Directory.Exists(DojaSkinFolder) Then
                For Each deleteFile In Directory.GetFiles(DojaSkinFolder, "*.*", SearchOption.TopDirectoryOnly)
                    File.Delete(deleteFile)
                Next
            Else
                Directory.CreateDirectory(DojaSkinFolder)
            End If

            Dim OurSkinsFolder = AppDomain.CurrentDomain.BaseDirectory & "data\tools\skins"
            If hideUI = True Then
                For Each F In Directory.GetFiles(OurSkinsFolder & "\doja-device1-noui")
                    File.Copy(F, $"{DojaSkinFolder}\{Path.GetFileName(F)}")
                Next
            ElseIf hideUI = False Then
                For Each F In Directory.GetFiles(OurSkinsFolder & "\doja-device1-ui")
                    File.Copy(F, $"{DojaSkinFolder}\{Path.GetFileName(F)}")
                Next
            End If
        End Sub
        Function ExtractDOJAWidthHeight(filePath As String) As (Integer, Integer)
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
                    width = 240
                    height = 240
                End If
            Catch ex As Exception
                Console.WriteLine("Error: " & ex.Message)
            End Try

            Return (width, height)
        End Function
        Public Sub UpdatedDOJADrawSize(DOJALOOCATION As String, X As Integer, Y As Integer)
            Dim Device1InfoFile = $"{DOJALOOCATION}\lib\skin\deviceinfo\device1"
            Dim NewValue = $"device1,{X},{Y},120,120"
            File.WriteAllText(Device1InfoFile, NewValue)
        End Sub
        Sub UpdateDOJASoundConf(DOJALOOCATION As String, SoundType As String)
            Dim SoundPath As String = $"{DOJALOOCATION}\lib\SoundConf.properties"
            If SoundType = "Standard" Then
                If File.Exists(SoundPath) Then
                    ' Read the file with Shift-JIS encoding
                    Dim conf As String = File.ReadAllText(SoundPath, Text.Encoding.GetEncoding("shift-jis"))

                    ' Perform the substitutions
                    conf = Regex.Replace(conf, "MODE=.", "MODE=0")
                    conf = Regex.Replace(conf, "SOUNDLIB=...", "SOUNDLIB=001")

                    ' Write the updated content back to the file
                    File.WriteAllText(SoundPath, conf, Text.Encoding.GetEncoding("shift-jis"))
                Else
                    Console.WriteLine($"File not found: {SoundPath}")
                End If
            ElseIf SoundType = "903i" Then
                If File.Exists(SoundPath) Then
                    ' Read the file with Shift-JIS encoding
                    Dim conf As String = File.ReadAllText(SoundPath, Text.Encoding.GetEncoding("shift-jis"))

                    ' Perform the substitutions
                    conf = Regex.Replace(conf, "MODE=.", "MODE=0")
                    conf = Regex.Replace(conf, "SOUNDLIB=...", "SOUNDLIB=002")

                    ' Write the updated content back to the file
                    File.WriteAllText(SoundPath, conf, Text.Encoding.GetEncoding("shift-jis"))
                Else
                    Console.WriteLine($"File not found: {SoundPath}")
                End If
            End If
        End Sub

        'STAR EXTRAS
        Public Sub UpdateSTARDeviceSkin(STARLOCATION As String, hideUI As Boolean)
            Dim StarSkinFolder = $"{STARLOCATION}\lib\skin\device1"
            If Directory.Exists(StarSkinFolder) Then
                For Each deleteFile In Directory.GetFiles(StarSkinFolder, "*.*", SearchOption.TopDirectoryOnly)
                    File.Delete(deleteFile)
                Next
            Else
                Directory.CreateDirectory(StarSkinFolder)
            End If


            Dim OurSkinsFolder = AppDomain.CurrentDomain.BaseDirectory & "data\tools\skins"
            If hideUI = True Then
                For Each F In Directory.GetFiles(OurSkinsFolder & "\star-device1-noui")
                    File.Copy(F, $"{StarSkinFolder}\{Path.GetFileName(F)}")
                Next
            ElseIf hideUI = False Then
                For Each F In Directory.GetFiles(OurSkinsFolder & "\star-device1-ui")
                    File.Copy(F, $"{StarSkinFolder}\{Path.GetFileName(F)}")
                Next
            End If
        End Sub
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
        Public Sub UpdatedSTARDrawSize(STARLOOCATION As String, X As Integer, Y As Integer)
            Dim Device1InfoFile = $"{STARLOOCATION}\lib\skin\deviceinfo\device1"
            Dim NewValue = $"device1,{X},{Y},120,120,0,2,0,1,3"
            File.WriteAllText(Device1InfoFile, NewValue)
        End Sub
        Sub UpdateSTARSoundConf(STARLOOCATION As String, SoundType As String)
            Dim SoundPath As String = $"{STARLOOCATION}\lib\SoundConf.properties"
            If SoundType = "Standard" Then
                If File.Exists(SoundPath) Then
                    ' Read the file with Shift-JIS encoding
                    Dim conf As String = File.ReadAllText(SoundPath, Text.Encoding.GetEncoding("shift-jis"))

                    ' Perform the substitutions
                    conf = Regex.Replace(conf, "MODE=.", "MODE=0")
                    conf = Regex.Replace(conf, "SOUNDLIB=...", "SOUNDLIB=001")

                    ' Write the updated content back to the file
                    File.WriteAllText(SoundPath, conf, Text.Encoding.GetEncoding("shift-jis"))
                Else
                    Console.WriteLine($"File not found: {SoundPath}")
                End If
            ElseIf SoundType = "903i" Then
                If File.Exists(SoundPath) Then
                    ' Read the file with Shift-JIS encoding
                    Dim conf As String = File.ReadAllText(SoundPath, Text.Encoding.GetEncoding("shift-jis"))

                    ' Perform the substitutions
                    conf = Regex.Replace(conf, "MODE=.", "MODE=0")
                    conf = Regex.Replace(conf, "SOUNDLIB=...", "SOUNDLIB=002")

                    ' Write the updated content back to the file
                    File.WriteAllText(SoundPath, conf, Text.Encoding.GetEncoding("shift-jis"))
                Else
                    Console.WriteLine($"File not found: {SoundPath}")
                End If
            End If

        End Sub
        Public Sub UpdateSTARAppconfig(STARLOCATION As String, GAMEJAM As String)
            Dim AppConfigFile = $"{STARLOCATION}\AppSetting"
            Dim AppConfigFCFile = $"{STARLOCATION}\AppSetting.fc"
            Dim GameDirectory As String
            Dim GameName = Path.GetFileNameWithoutExtension(GAMEJAM)
            Dim binIndex As Integer = GAMEJAM.IndexOf("\bin")
            If binIndex <> -1 Then
                GameDirectory = GAMEJAM.Substring(0, binIndex)
            Else
                Console.WriteLine("'\bin' not found in path.")
            End If

            If File.Exists(AppConfigFile) = False Or File.Exists(AppConfigFCFile) = False Then
                MessageBox.Show("Missing STAR AppSettingsFiles")
                Exit Sub
            Else
                If File.Exists($"{GameDirectory}\{GameName}") = False Then
                    File.Copy(AppConfigFile, $"{GameDirectory}\{GameName}")
                End If
                If File.Exists($"{GameDirectory}\{GameName}.fc") = False Then
                    File.Copy(AppConfigFCFile, $"{GameDirectory}\{GameName}.fc")
                End If
            End If
        End Sub
        Public Sub EnsureSTARJamFileEntries(GAMEJAM As String)
            ' Ensure the file exists
            If Not File.Exists(GAMEJAM) Then
                Throw New FileNotFoundException($"The file '{GAMEJAM}' does not exist.")
            End If

            ' Read the file contents using Shift-JIS encoding
            Dim lines As List(Of String) = File.ReadAllLines(GAMEJAM, Encoding.GetEncoding("shift-jis")).ToList()
            Dim modified As Boolean = False

            ' Define the required entries
            Dim requiredEntries As New Dictionary(Of String, String) From {
        {"UseNetwork", "yes"},
        {"TrustedAPID", "00000000000"},
        {"MessageCode", "0000000000"}
    }

            ' Check and add missing entries
            For Each entry In requiredEntries
                If Not lines.Any(Function(line) line.StartsWith(entry.Key & " = ")) Then
                    lines.Add(entry.Key & " = " & entry.Value)
                    modified = True
                End If
            Next

            ' Write back to the file if modifications were made
            If modified Then
                File.WriteAllLines(GAMEJAM, lines, Encoding.GetEncoding("shift-jis"))
            End If
        End Sub

        'Generate XML for List
        Public Sub ProcessZipFileforGamelist(inputZipPath As String)
            ' Ensure the input file exists
            If Not File.Exists(inputZipPath) Then
                Throw New FileNotFoundException($"The file '{inputZipPath}' does not exist.")
            End If

            ' Define paths
            Dim tempFolder As String = Path.Combine(Path.GetTempPath(), "ZipProcessing")
            Dim extractedFolder As String = Path.Combine(tempFolder, "Extracted")
            Dim xmlFilePath As String = Path.Combine(Path.GetDirectoryName(inputZipPath), "gamelist.xml")

            ' Clean up temp folder if it already exists
            If Directory.Exists(tempFolder) Then
                Directory.Delete(tempFolder, True)
            End If

            ' Extract the ZIP file to the temp folder
            Directory.CreateDirectory(extractedFolder)
            ZipFile.ExtractToDirectory(inputZipPath, extractedFolder)

            ' Load or create the XML file with Shift-JIS encoding
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

            ' Process each folder in the extracted directory
            Try
                For Each folderPath As String In Directory.GetDirectories(extractedFolder, "*", SearchOption.TopDirectoryOnly)
                    Dim parentFolder As String = Path.GetFileName(Path.GetDirectoryName(folderPath & Path.DirectorySeparatorChar))
                    ' Look for the .jam, .jar, and .sp files in the folder and its subfolders
                    Dim jamFile As String = Directory.GetFiles(folderPath, "*.jam", SearchOption.AllDirectories).FirstOrDefault()
                    Dim jarFile As String = Directory.GetFiles(folderPath, "*.jar", SearchOption.AllDirectories).FirstOrDefault()
                    Dim spFile As String = Directory.GetFiles(folderPath, "*.sp", SearchOption.AllDirectories).FirstOrDefault()

                    ' Skip if none of the files exist
                    If String.IsNullOrEmpty(jamFile) AndAlso String.IsNullOrEmpty(jarFile) AndAlso String.IsNullOrEmpty(spFile) Then
                        Continue For
                    End If

                    ' Extract AppName from the .jam file
                    Dim appName As String = "Unknown"
                    Dim profilever As String = "doja"
                    Dim Emulator As String = "doja"
                    If Not String.IsNullOrEmpty(jamFile) Then
                        Dim jamLines As String() = File.ReadAllLines(jamFile, Encoding.GetEncoding("shift-jis"))
                        Dim appNameLine As String = jamLines.FirstOrDefault(Function(line) line.StartsWith("AppName = "))
                        Dim ProfileVerLine As String = jamLines.FirstOrDefault(Function(line) line.StartsWith("ProfileVer = "))
                        Dim AppTypeLine As String = jamLines.FirstOrDefault(Function(line) line.StartsWith("AppType = "))
                        If Not String.IsNullOrEmpty(appNameLine) Then
                            appName = appNameLine.Replace("AppName = ", "").Trim()
                        End If
                        If Not String.IsNullOrEmpty(ProfileVerLine) Then
                            profilever = ProfileVerLine.Replace("ProfileVer = ", "").Trim()
                        End If
                        ' If AppType exists, set emulator to "star", as AppType on exist in star apps
                        If Not String.IsNullOrEmpty(AppTypeLine) Then
                            Emulator = "star"
                        End If
                    End If

                    ' Create "bin" and "sp" folders in the current folder
                    Dim binFolder As String = Path.Combine(folderPath, "bin")
                    Dim spFolder As String = Path.Combine(folderPath, "sp")
                    Directory.CreateDirectory(binFolder)
                    Directory.CreateDirectory(spFolder)

                    ' Move files to their respective folders
                    ' Check and move the files only if they exist
                    If Not String.IsNullOrEmpty(jamFile) AndAlso File.Exists(jamFile) Then
                        Dim targetPath As String = Path.Combine(binFolder, Path.GetFileName(jamFile))
                        If File.Exists(targetPath) Then File.Delete(targetPath) ' Ensure no duplicates
                        File.Move(jamFile, targetPath)
                    End If

                    If Not String.IsNullOrEmpty(jarFile) AndAlso File.Exists(jarFile) Then
                        Dim targetPath As String = Path.Combine(binFolder, Path.GetFileName(jarFile))
                        If File.Exists(targetPath) Then File.Delete(targetPath)
                        File.Move(jarFile, targetPath)
                    End If

                    If Not String.IsNullOrEmpty(spFile) AndAlso File.Exists(spFile) Then
                        Dim targetPath As String = Path.Combine(spFolder, Path.GetFileName(spFile))
                        If File.Exists(targetPath) Then File.Delete(targetPath)
                        File.Move(spFile, targetPath)
                    End If

                    ' Create a new ZIP file with the same name as the .jar file
                    If Not String.IsNullOrEmpty(jarFile) Then
                        Dim newZipName As String = Path.GetFileNameWithoutExtension(jarFile) & ".zip"
                        Dim outputZipPath As String = Path.Combine(Path.GetDirectoryName(inputZipPath), newZipName)
                        If File.Exists(outputZipPath) Then
                            File.Delete(outputZipPath)
                        End If

                        ' Create a temporary folder for zipping only bin and sp folders
                        Dim tempZipFolder As String = Path.Combine(tempFolder, "ToZip")
                        Directory.CreateDirectory(tempZipFolder)

                        ' Copy bin and sp folders to the temp zip folder
                        If Directory.Exists(binFolder) Then
                            DirectoryCopy(binFolder, Path.Combine(tempZipFolder, "bin"), True)
                        End If
                        If Directory.Exists(spFolder) Then
                            DirectoryCopy(spFolder, Path.Combine(tempZipFolder, "sp"), True)
                        End If

                        ' Zip only the bin and sp folders
                        ZipFile.CreateFromDirectory(tempZipFolder, outputZipPath)

                        ' Clean up the temp zip folder
                        Directory.Delete(tempZipFolder, True)

                        ' Add entry to the XML file
                        Dim root As XmlElement = xmlDoc.DocumentElement
                        Dim gameElement As XmlElement = xmlDoc.CreateElement("Game")

                        Dim enTitleElement As XmlElement = xmlDoc.CreateElement("ENTitle")
                        enTitleElement.InnerText = parentFolder
                        gameElement.AppendChild(enTitleElement)

                        Dim jpTitleElement As XmlElement = xmlDoc.CreateElement("JPTitle")
                        jpTitleElement.InnerText = appName
                        gameElement.AppendChild(jpTitleElement)

                        Dim zipNameElement As XmlElement = xmlDoc.CreateElement("ZIPName")
                        zipNameElement.InnerText = newZipName
                        gameElement.AppendChild(zipNameElement)

                        Dim downloadUrlElement As XmlElement = xmlDoc.CreateElement("DownloadURL")
                        downloadUrlElement.InnerText = $"https://s3.inferia.world/launcher/{newZipName}"
                        gameElement.AppendChild(downloadUrlElement)

                        Dim CustomAppIconURLElement As XmlElement = xmlDoc.CreateElement("CustomAppIconURL")
                        CustomAppIconURLElement.InnerText = $""
                        gameElement.AppendChild(CustomAppIconURLElement)

                        Dim SDCardDataURLElement As XmlElement = xmlDoc.CreateElement("SDCardDataURL")
                        SDCardDataURLElement.InnerText = $""
                        gameElement.AppendChild(SDCardDataURLElement)

                        Dim emulatorElement As XmlElement = xmlDoc.CreateElement("Emulator")
                        emulatorElement.InnerText = Emulator
                        gameElement.AppendChild(emulatorElement)

                        root.AppendChild(gameElement)
                    End If

                    ' Clean up empty folders
                    If Directory.Exists(binFolder) AndAlso Directory.GetFiles(binFolder).Length = 0 Then
                        Directory.Delete(binFolder, True)
                    End If

                    If Directory.Exists(spFolder) AndAlso Directory.GetFiles(spFolder).Length = 0 Then
                        Directory.Delete(spFolder, True)
                    End If
                Next

            Catch ex As Exception
                MessageBox.Show($"An error occurred while creating the ZIP or updating the XML:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

            ' Save the updated XML file with Shift-JIS encoding
            Using writer As XmlWriter = XmlWriter.Create(xmlFilePath, xmlSettings)
                xmlDoc.Save(writer)
            End Using
            ' Clean up temp folder
            Directory.Delete(tempFolder, True)
            MessageBox.Show("Completed XML Creation")
        End Sub
        Private Sub DirectoryCopy(sourceDirName As String, destDirName As String, copySubDirs As Boolean)
            Dim dir As DirectoryInfo = New DirectoryInfo(sourceDirName)
            Dim dirs As DirectoryInfo() = dir.GetDirectories()

            If Not dir.Exists Then
                Throw New DirectoryNotFoundException($"Source directory does not exist or could not be found: {sourceDirName}")
            End If

            If Not Directory.Exists(destDirName) Then
                Directory.CreateDirectory(destDirName)
            End If

            Dim files As FileInfo() = dir.GetFiles()
            For Each file As FileInfo In files
                Dim tempPath As String = Path.Combine(destDirName, file.Name)
                file.CopyTo(tempPath, False)
            Next

            If copySubDirs Then
                For Each subdir As DirectoryInfo In dirs
                    Dim tempPath As String = Path.Combine(destDirName, subdir.Name)
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs)
                Next
            End If
        End Sub
    End Class
End Namespace