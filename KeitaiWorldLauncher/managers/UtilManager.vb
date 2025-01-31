﻿Imports System.Net
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
            Directory.CreateDirectory("data\tools")
            Directory.CreateDirectory("data\tools\icons")
            Directory.CreateDirectory("data\tools\icons\defaults")
            Directory.CreateDirectory("data\tools\skins")
            Directory.CreateDirectory("data\tools\skins\doja\ui")
            Directory.CreateDirectory("data\tools\skins\doja\noui")
            Directory.CreateDirectory("data\tools\skins\star\ui")
            Directory.CreateDirectory("data\tools\skins\star\noui")
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
                    Catch ex As Exception
                        MessageBox.Show("An error occurred while trying to close doja.exe: " & ex.Message,
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error)
                    End Try
                Else
                    Return True
                End If
            Else
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
                    Catch ex As Exception
                        MessageBox.Show("An error occurred while trying to close star.exe: " & ex.Message,
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error)
                    End Try
                Else
                    Return True
                End If
            Else
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
        Public Async Sub LaunchCustomDOJAGameCommand(DOJAPATH As String, DOJAEXELocation As String, GameJAM As String)
            Try
                ' Paths and arguments
                Dim appPath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "tools", "locale_emulator", "LEProc.exe").Trim
                Dim dojaExePath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DOJAEXELocation).Trim
                Dim jamPath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GameJAM).Trim
                Dim guidArg As String = "-runas ad1a7fe1-4f95-45ba-b563-9ba60c3642d3"
                Dim arguments As String = $"{guidArg} ""{dojaExePath}"" -i ""{jamPath}"""

                ' Update device settings based on user selections
                If UpdateDOJADeviceSkin(DOJAPATH, Form1.chkbxHidePhoneUI.Checked) = False Then
                    Exit Sub
                End If

                ' Update device screen size
                If DOJAPATH.Contains("5.1") Then
                    Dim dimensions = ExtractDOJAWidthHeight(jamPath)
                    Dim width As Integer = dimensions.Item1
                    Dim height As Integer = dimensions.Item2
                    UpdatedDOJADrawSize(DOJAPATH, width, height)
                End If

                ' Update sound configuration
                UpdateDOJASoundConf(DOJAPATH, Form1.cobxAudioType.SelectedItem.ToString())

                ' Set up process start info
                Dim startInfo As New ProcessStartInfo With {
                    .FileName = appPath,
                    .Arguments = arguments,
                    .UseShellExecute = False,
                    .CreateNoWindow = True,
                    .WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                }

                Dim process As Process = Process.Start(startInfo)
                If process IsNot Nothing Then
                    process.WaitForInputIdle()
                Else
                    MessageBox.Show("Failed to start process.")
                End If

                ' Launch ShaderGlass if enabled
                If Form1.chkbxShaderGlass.Checked Then
                    If Await WaitForDojaToStart() Then
                        LaunchShaderGlass(Path.GetFileNameWithoutExtension(jamPath))
                    Else
                        MessageBox.Show("Failed to detect DOJA running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                End If

            Catch ex As Exception
                logger.Logger.LogError($"Failed to launch the command: {ex.Message}")
                MessageBox.Show($"Failed to launch the command: {ex.Message}", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
        Public Async Sub LaunchCustomSTARGameCommand(STARPATH As String, STAREXELocation As String, GameJAM As String)
            Try
                ' Validate inputs
                If String.IsNullOrWhiteSpace(STARPATH) OrElse String.IsNullOrWhiteSpace(STAREXELocation) OrElse String.IsNullOrWhiteSpace(GameJAM) Then
                    Throw New ArgumentException("One or more required parameters are missing.")
                End If

                ' Paths and arguments
                Dim appPath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data\tools\locale_emulator\LEProc.exe")
                Dim guidArg As String = "-runas ad1a7fe1-4f95-45ba-b563-9ba60c3642d3"
                Dim jamPath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GameJAM)
                Dim arguments As String = $"{guidArg} ""{STAREXELocation}"" -i ""{jamPath}"""

                ' Update device launch settings
                Dim hideUI As Boolean = Form1.chkbxHidePhoneUI.Checked
                If Not UpdateSTARDeviceSkin(STARPATH, hideUI) Then Exit Sub

                ' Update device draw size
                Dim JAMDrawArea = ExtractSTARWidthHeight(jamPath)
                UpdatedSTARDrawSize(STARPATH, JAMDrawArea.Item1, JAMDrawArea.Item2)

                ' Update sound configuration
                UpdateSTARSoundConf(STARPATH, Form1.cobxAudioType.SelectedItem.ToString())

                ' Update app configuration
                UpdateSTARAppconfig(STARPATH, GameJAM)
                EnsureSTARJamFileEntries(GameJAM)

                ' Set up process start info
                Dim startInfo As New ProcessStartInfo With {
                    .FileName = appPath,
                    .Arguments = arguments,
                    .UseShellExecute = False,
                    .CreateNoWindow = True,
                    .WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                }

                ' Launch process
                Using process As Process = Process.Start(startInfo)
                    If process IsNot Nothing Then
                        process.WaitForInputIdle()
                    Else
                        Throw New Exception("Failed to start process.")
                    End If
                End Using

                ' Launch ShaderGlass if selected
                If Form1.chkbxShaderGlass.Checked Then
                    If Await WaitForSTARToStart() Then
                        LaunchShaderGlass(Path.GetFileNameWithoutExtension(jamPath))
                    Else
                        MessageBox.Show("Failed to detect STAR running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                End If

            Catch ex As ArgumentException
                logger.Logger.LogError($"Invalid input: {ex.Message}")
                MessageBox.Show($"Invalid input: {ex.Message}", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Catch ex As Exception
                logger.Logger.LogError($"Failed to launch the command: {ex.Message}")
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
        Public Function UpdateDOJADeviceSkin(DOJALOCATION As String, hideUI As Boolean)
            Dim dojaSkinFolder As String = Path.Combine(DOJALOCATION, "lib", "skin", "device1")
            Dim DojaPath = Path.GetFileName(DOJALOCATION)

            ' Clear the skin folder or create it if it doesn't exist
            If Directory.Exists(dojaSkinFolder) Then
                Directory.GetFiles(dojaSkinFolder).ToList().ForEach(Sub(files) File.Delete(files))
            Else
                Directory.CreateDirectory(dojaSkinFolder)
            End If

            ' Select the correct skin folder based on UI visibility
            Dim skinUIFolder As String = If(hideUI, "noui", "ui")
            Dim ourSkinsFolder As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "tools", "skins", "doja", skinUIFolder, DojaPath)
            If Directory.Exists(ourSkinsFolder) = False Then
                MessageBox.Show($"Skin folder missing: {ourSkinsFolder}")
                logger.Logger.LogError($"Skin folder missing: {ourSkinsFolder}")
                Return False
            End If
            ' Copy the files from the selected skin folder to the target folder
            For Each skinFile In Directory.GetFiles(ourSkinsFolder)
                File.Copy(skinFile, Path.Combine(dojaSkinFolder, Path.GetFileName(skinFile)), True)
            Next
            Return True
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
                Console.WriteLine($"Error reading width and height: {ex.Message}")
            End Try

            Return (width, height)
        End Function
        Public Sub UpdatedDOJADrawSize(DOJALOCATION As String, width As Integer, height As Integer)
            Dim deviceInfoFile As String = Path.Combine(DOJALOCATION, "lib", "skin", "deviceinfo", "device1")
            Dim newValue As String = $"device1,{width},{height},120,120"
            File.WriteAllText(deviceInfoFile, newValue)
        End Sub
        Public Sub UpdateDOJASoundConf(DOJALOCATION As String, soundType As String)
            Dim soundPath As String = Path.Combine(DOJALOCATION, "lib", "SoundConf.properties")
            If Not File.Exists(soundPath) Then
                Console.WriteLine($"File not found: {soundPath}")
                Return
            End If

            Try
                ' Read the file with Shift-JIS encoding
                Dim conf As String = File.ReadAllText(soundPath, Text.Encoding.GetEncoding("shift-jis"))

                ' Update mode and sound library based on sound type
                conf = Regex.Replace(conf, "MODE=.", "MODE=0")

                Dim soundLibValue As String = If(soundType = "903i", "002", "001")
                conf = Regex.Replace(conf, "SOUNDLIB=...", $"SOUNDLIB={soundLibValue}")

                ' Write the updated configuration back to the file
                File.WriteAllText(soundPath, conf, Text.Encoding.GetEncoding("shift-jis"))
            Catch ex As Exception
                Console.WriteLine($"Error updating sound configuration: {ex.Message}")
            End Try
        End Sub

        'STAR EXTRAS
        Public Function UpdateSTARDeviceSkin(STARLOCATION As String, hideUI As Boolean)
            Dim StarSkinFolder = $"{STARLOCATION}\lib\skin\device1"
            If Directory.Exists(StarSkinFolder) Then
                For Each deleteFile In Directory.GetFiles(StarSkinFolder, "*.*", SearchOption.TopDirectoryOnly)
                    File.Delete(deleteFile)
                Next
            Else
                Directory.CreateDirectory(StarSkinFolder)
            End If


            Dim OurSkinsFolder = AppDomain.CurrentDomain.BaseDirectory & "data\tools\skins\star"
            If Directory.Exists(OurSkinsFolder) = False Then
                MessageBox.Show($"Skin folder missing: {OurSkinsFolder}")
                logger.Logger.LogError($"Skin folder missing: {OurSkinsFolder}")
                Return False
            End If
            If hideUI = True Then
                For Each F In Directory.GetFiles(OurSkinsFolder & "\noui\star-device1-noui")
                    File.Copy(F, $"{StarSkinFolder}\{Path.GetFileName(F)}")
                Next
            ElseIf hideUI = False Then
                For Each F In Directory.GetFiles(OurSkinsFolder & "\ui\star-device1-ui")
                    File.Copy(F, $"{StarSkinFolder}\{Path.GetFileName(F)}")
                Next
            End If
            Return True
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


        'MISC


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
                For Each rootFolderPath As String In Directory.GetDirectories(extractedFolder, "*", SearchOption.TopDirectoryOnly)
                    Dim enTitle As String = Path.GetFileName(rootFolderPath)
                    Dim variants As New List(Of String)
                    Dim zipFileName As String = Nothing
                    Dim emulator As String = "doja"

                    ' Determine if the root folder contains files directly or multiple subfolders
                    Dim subFolders = Directory.GetDirectories(rootFolderPath, "*", SearchOption.TopDirectoryOnly)
                    Dim jamFiles = Directory.GetFiles(rootFolderPath, "*.jam", SearchOption.TopDirectoryOnly)
                    Dim jarFiles = Directory.GetFiles(rootFolderPath, "*.jar", SearchOption.TopDirectoryOnly)
                    Dim spFiles = Directory.GetFiles(rootFolderPath, "*.sp", SearchOption.TopDirectoryOnly)
                    Dim scrFiles = Directory.GetFiles(rootFolderPath, "*.scr", SearchOption.TopDirectoryOnly)

                    ' Case 1: Only one subfolder inside the root folder, go into that subfolder
                    If subFolders.Length = 1 AndAlso jamFiles.Length = 0 AndAlso jarFiles.Length = 0 AndAlso spFiles.Length = 0 AndAlso scrFiles.Length = 0 Then
                        zipFileName = ProcessSingleVarient(subFolders(0), enTitle, inputZipPath, tempFolder, emulator)

                        ' Case 2: Files directly in the root folder
                    ElseIf subFolders.Length = 0 Then
                        zipFileName = ProcessSingleVarient(rootFolderPath, enTitle, inputZipPath, tempFolder, emulator)

                        ' Case 3: Multiple subfolders, treat them as variants
                    Else
                        For Each variantFolder In subFolders
                            Dim variantName As String = Path.GetFileName(variantFolder).Replace(" ", "_")
                            variants.Add(variantName)
                        Next
                        zipFileName = ProcessMultipleVarient(rootFolderPath, enTitle, variants, inputZipPath, tempFolder, emulator)
                    End If

                    ' Add the game entry to the XML
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
                    downloadUrlElement.InnerText = $"https://s3.inferia.world/launcher/games/{zipFileName}"
                    gameElement.AppendChild(downloadUrlElement)

                    Dim customAppIconURLElement As XmlElement = xmlDoc.CreateElement("CustomAppIconURL")
                    customAppIconURLElement.InnerText = ""
                    gameElement.AppendChild(customAppIconURLElement)

                    Dim sdCardDataURLElement As XmlElement = xmlDoc.CreateElement("SDCardDataURL")
                    sdCardDataURLElement.InnerText = ""
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

            ' Save the updated XML file with Shift-JIS encoding
            Using writer As XmlWriter = XmlWriter.Create(xmlFilePath, xmlSettings)
                xmlDoc.Save(writer)
            End Using
            ' Clean up temp folder
            Directory.Delete(tempFolder, True)
            MessageBox.Show("Completed XML Creation")
        End Sub
        Private Function ProcessSingleVarient(folderPath As String, inputENTitle As String, inputZipPath As String, tempFolder As String, ByRef emulator As String) As String
            ' Rename
            RenameFilesRecursively(folderPath, inputENTitle)

            ' Locate .jam, .jar, and .sp files
            Dim jamFile As String = Directory.GetFiles(folderPath, "*.jam", SearchOption.TopDirectoryOnly).FirstOrDefault()
            Dim jarFile As String = Directory.GetFiles(folderPath, "*.jar", SearchOption.TopDirectoryOnly).FirstOrDefault()
            Dim spFile As String = Directory.GetFiles(folderPath, "*.sp", SearchOption.TopDirectoryOnly).FirstOrDefault()
            Dim scrFile As String = Directory.GetFiles(folderPath, "*.scr", SearchOption.TopDirectoryOnly).FirstOrDefault()


            ' Skip if no files are found
            If String.IsNullOrEmpty(jarFile) Then Return Nothing

            ' Extract emulator and app details from the .jam file
            If Not String.IsNullOrEmpty(jamFile) Then
                Dim jamLines As String() = File.ReadAllLines(jamFile, Encoding.GetEncoding("shift-jis"))
                Dim appTypeLine As String = jamLines.FirstOrDefault(Function(line) line.StartsWith("AppType = "))
                If Not String.IsNullOrEmpty(appTypeLine) Then
                    emulator = "star"
                End If
            End If

            ' Create bin and sp folders
            Dim binFolder As String = Path.Combine(tempFolder, "bin")
            Dim spFolder As String = Path.Combine(tempFolder, "sp")
            Directory.CreateDirectory(binFolder)
            Directory.CreateDirectory(spFolder)

            ' Move files to bin and sp folders
            If Not String.IsNullOrEmpty(jamFile) Then File.Move(jamFile, Path.Combine(binFolder, Path.GetFileName(jamFile)), True)
            If Not String.IsNullOrEmpty(jarFile) Then File.Move(jarFile, Path.Combine(binFolder, Path.GetFileName(jarFile)), True)
            If Not String.IsNullOrEmpty(spFile) Then File.Move(spFile, Path.Combine(spFolder, Path.GetFileName(spFile)), True)
            If Not String.IsNullOrEmpty(scrFile) Then File.Move(scrFile, Path.Combine(spFolder, Path.GetFileName(scrFile)), True)

            ' Create the ZIP file
            Dim zipFileName As String = Path.GetFileNameWithoutExtension(jarFile) & ".zip"
            Dim outputZipPath As String = Path.Combine(Path.GetDirectoryName(inputZipPath), zipFileName)
            If File.Exists(outputZipPath) Then File.Delete(outputZipPath)

            Dim tempZipFolder As String = Path.Combine(tempFolder, "ToZip")
            Directory.CreateDirectory(tempZipFolder)
            DirectoryCopy(binFolder, Path.Combine(tempZipFolder, "bin"), True)
            DirectoryCopy(spFolder, Path.Combine(tempZipFolder, "sp"), True)

            ZipFile.CreateFromDirectory(tempZipFolder, outputZipPath)
            Directory.Delete(tempZipFolder, True)
            Directory.Delete(binFolder, True)
            Directory.Delete(spFolder, True)
            Return zipFileName
        End Function
        Private Function ProcessMultipleVarient(folderPath As String, RootFolderName As String, variants As List(Of String), inputZipPath As String, tempFolder As String, ByRef emulator As String) As String
            Dim MasterJarName As String = ""
            ' Setup DIRS
            Dim combinedZipPath As String = Path.Combine(Path.GetDirectoryName(inputZipPath), RootFolderName & ".zip")
            Dim tempCombinedZipFolder As String = Path.Combine(tempFolder, "CombinedZip")
            Directory.CreateDirectory(tempCombinedZipFolder)

            For Each VariantFolder In Directory.GetDirectories(folderPath)
                Dim variantName As String = Path.GetFileName(VariantFolder).Replace(" ", "_")
                Dim tempCombinedZipVariantFolder As String = Path.Combine(tempCombinedZipFolder, variantName)
                Directory.CreateDirectory(tempCombinedZipVariantFolder)

                ' Locate .jam, .jar, and .sp files
                Dim jamFile As String = Directory.GetFiles(VariantFolder, "*.jam", SearchOption.TopDirectoryOnly).FirstOrDefault()
                Dim jarFile As String = Directory.GetFiles(VariantFolder, "*.jar", SearchOption.TopDirectoryOnly).FirstOrDefault()
                Dim spFile As String = Directory.GetFiles(VariantFolder, "*.sp", SearchOption.TopDirectoryOnly).FirstOrDefault()
                Dim scrFile As String = Directory.GetFiles(VariantFolder, "*.scr", SearchOption.TopDirectoryOnly).FirstOrDefault()

                ' Skip if no files are found
                If String.IsNullOrEmpty(jarFile) Then Return Nothing
                MasterJarName = Path.GetFileNameWithoutExtension(jarFile)
                ' Extract emulator and app details from the .jam file
                If Not String.IsNullOrEmpty(jamFile) Then
                    Dim jamLines As String() = File.ReadAllLines(jamFile, Encoding.GetEncoding("shift-jis"))
                    Dim appTypeLine As String = jamLines.FirstOrDefault(Function(line) line.StartsWith("AppType = "))
                    If Not String.IsNullOrEmpty(appTypeLine) Then
                        emulator = "star"
                    End If
                End If
                ' Create bin and sp folders
                Dim binFolder As String = Path.Combine(tempCombinedZipVariantFolder, "bin")
                Dim spFolder As String = Path.Combine(tempCombinedZipVariantFolder, "sp")
                Directory.CreateDirectory(binFolder)
                Directory.CreateDirectory(spFolder)

                ' Move files to bin and sp folders
                If Not String.IsNullOrEmpty(jamFile) Then File.Move(jamFile, Path.Combine(binFolder, Path.GetFileName(jamFile)), True)
                If Not String.IsNullOrEmpty(jarFile) Then File.Move(jarFile, Path.Combine(binFolder, Path.GetFileName(jarFile)), True)
                If Not String.IsNullOrEmpty(spFile) Then File.Move(spFile, Path.Combine(spFolder, Path.GetFileName(spFile)), True)
                If Not String.IsNullOrEmpty(scrFile) Then File.Move(scrFile, Path.Combine(spFolder, Path.GetFileName(scrFile)), True)
            Next

            ' Create the ZIP file
            Dim zipFileName As String = Path.GetFileNameWithoutExtension(RootFolderName).Replace(" ", "_") & ".zip"
            Dim outputZipPath As String = Path.Combine(Path.GetDirectoryName(inputZipPath), zipFileName)
            If File.Exists(outputZipPath) Then File.Delete(outputZipPath)

            ' Need to rename all files in the combinedzip to be the same
            RenameFilesRecursively(tempCombinedZipFolder, Path.GetFileNameWithoutExtension(zipFileName))

            ' Create the final ZIP containing all variants
            ZipFile.CreateFromDirectory(tempCombinedZipFolder, outputZipPath)
            Directory.Delete(tempCombinedZipFolder, True)
            zipFileName = Path.GetFileName(outputZipPath)
            Return zipFileName
        End Function
        Private Sub DirectoryCopy(sourceDirName As String, destDirName As String, copySubDirs As Boolean)
            Dim dir As DirectoryInfo = New DirectoryInfo(sourceDirName)
            Dim dirs As DirectoryInfo() = dir.GetDirectories()

            If Not Directory.Exists(destDirName) Then
                Directory.CreateDirectory(destDirName)
            End If

            For Each file In dir.GetFiles()
                file.CopyTo(Path.Combine(destDirName, file.Name), True)
            Next

            If copySubDirs Then
                For Each subdir In dirs
                    Dim tempPath As String = Path.Combine(destDirName, subdir.Name)
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs)
                Next
            End If
        End Sub
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

    End Class
End Namespace