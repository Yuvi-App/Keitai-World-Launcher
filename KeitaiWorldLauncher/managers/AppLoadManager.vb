Imports System.IO
Imports KeitaiWorldLauncher.My.Models

Namespace My.Managers
    Public Class AppLoadManager
        Dim utilManager As New UtilManager
        Public Shared Sub LoadConfigValues(cfg As Dictionary(Of String, String))
            MainForm.versionCheckUrl = cfg("VersionCheckURL")
            MainForm.autoUpdate = Boolean.Parse(cfg("AutoUpdate"))
            MainForm.FirstRun = Boolean.Parse(cfg("FirstRun"))
            MainForm.gameListUrl = cfg("GamelistURL")
            MainForm.autoUpdateGameList = Boolean.Parse(cfg("AutoUpdateGameList"))
            MainForm.machicharaListUrl = cfg("MachiCharalistURL")
            MainForm.autoUpdatemachicharaList = Boolean.Parse(cfg("AutoUpdateMachiCharaList"))
            MainForm.charadenListUrl = cfg("CharaDenlistURL")
            MainForm.autoUpdatecharadenList = Boolean.Parse(cfg("AutoUpdateCharaDenList"))
            MainForm.UseShaderGlass = Boolean.Parse(cfg("UseShaderGlass"))
            MainForm.UseDialPad = Boolean.Parse(cfg("UseDialPad"))
            MainForm.DOJApath = cfg("DOJAPath")
            MainForm.DOJAEXE = cfg("DOJAEXEPath")
            MainForm.DOJAHideUI = Boolean.Parse(cfg("DOJAHideUI"))
            MainForm.DOJASoundType = cfg("DOJASoundType")
            MainForm.STARpath = cfg("STARPath")
            MainForm.STAREXE = cfg("STAREXEPath")
            MainForm.JSKYpath = cfg("JSKYPath")
            MainForm.JSKYEXE = cfg("JSKYEXEPath")
            MainForm.SOFTBANKpath = cfg("SOFTBANKPath")
            MainForm.SOFTBANKEXE = cfg("SOFTBANKEXEPath")
            MainForm.VODAFONEpath = cfg("VODAFONEPath")
            MainForm.VODAFONEEXE = cfg("VODAFONEEXEPath")
            MainForm.AIREDGEpath = cfg("AIREDGEPath")
            MainForm.AIREDGEEXE = cfg("AIREDGEEXEPath")
            MainForm.FlashPlayerpath = cfg("FlashPlayerPath")
            MainForm.FlashPlayerEXE = cfg("FlashPlayerEXEPath")
            MainForm.MachiCharapath = cfg("MachiCharaPath")
            MainForm.MachiCharaExe = cfg("MachiCharaEXEPath")
            MainForm.CharaDenpath = cfg("CharaDenPath")
            MainForm.CharaDenExe = cfg("CharaDenEXEPath")
        End Sub
        Public Shared Sub LoadNetworkUIDTerminaID()
            Dim defaultNetworkUID As String = "NULLGWDOCOMO"
            Dim defaultTerminalID As String = "123456789ABCDEF"

            ' Create file if missing
            If Not File.Exists(MainForm.NetworkUIDTxtFile) Then
                File.WriteAllLines(MainForm.NetworkUIDTxtFile, {defaultNetworkUID, defaultTerminalID})
            End If
            Dim lines As New List(Of String)(File.ReadAllLines(MainForm.NetworkUIDTxtFile))
            Dim needsWriteBack As Boolean = False
            ' Ensure at least 2 lines (legacy upgrade)
            While lines.Count < 2
                lines.Add(String.Empty)
                needsWriteBack = True
            End While
            ' Line 1: Network UID
            If Not String.IsNullOrWhiteSpace(lines(0)) Then
                MainForm.NetworkUID = lines(0).Trim()
            Else
                MainForm.NetworkUID = defaultNetworkUID
                lines(0) = defaultNetworkUID
                needsWriteBack = True
            End If
            ' Line 2: Terminal ID
            If Not String.IsNullOrWhiteSpace(lines(1)) Then
                MainForm.TerminalID = lines(1).Trim()
            Else
                MainForm.TerminalID = defaultTerminalID
                lines(1) = defaultTerminalID
                needsWriteBack = True
            End If
            ' Write back only if we fixed something
            If needsWriteBack Then
                File.WriteAllLines(MainForm.NetworkUIDTxtFile, lines)
            End If
        End Sub

        'Migration Script for older AppName
        Public Shared Sub MigrateDownloadsByZipPlusEmulator(
            downloadsFolder As String,
            games As List(Of Game),
            Optional doMove As Boolean = True  ' False = copy instead of move (safer)
        )
            If Not Directory.Exists(downloadsFolder) Then Return

            ' Group by ZipNameNoExt because that's the old folder key
            Dim groups = games.
                Where(Function(g) Not String.IsNullOrWhiteSpace(g.ZIPName)).
                GroupBy(Function(g) Path.GetFileNameWithoutExtension(g.ZIPName), StringComparer.OrdinalIgnoreCase)

            For Each grp In groups
                Dim zipNoExt = grp.Key
                Dim oldFolder = Path.Combine(downloadsFolder, zipNoExt)

                If Not Directory.Exists(oldFolder) Then
                    Continue For
                End If

                ' If only one game uses this ZIPName => unambiguous rename/move
                If grp.Count() = 1 Then
                    Dim g = grp.First()
                    Dim newFolder = Path.Combine(downloadsFolder, $"{zipNoExt}_{g.Emulator}")

                    If Directory.Exists(newFolder) Then
                        ' Already migrated
                        Continue For
                    End If

                    SafeMoveOrCopyFolder(oldFolder, newFolder, doMove)
                    Continue For
                End If

                ' Multiple emulators share the same ZIPName => attempt split by emulator file-type
                For Each g In grp
                    Dim newFolder = Path.Combine(downloadsFolder, $"{zipNoExt}_{g.Emulator}")
                    If Directory.Exists(newFolder) Then Continue For

                    Directory.CreateDirectory(newFolder)

                    ' Move/copy only files relevant to that emulator from the old folder
                    SplitInstallByEmulator(oldFolder, newFolder, g.Emulator, doMove)
                Next

                ' If we moved everything out, old folder may be empty. Leave it, or delete if desired.
            Next
            DeleteEmptyDirectories(downloadsFolder)
        End Sub
        Private Shared Sub SafeMoveOrCopyFolder(src As String, dst As String, doMove As Boolean)
            Directory.CreateDirectory(Path.GetDirectoryName(dst))

            If doMove Then
                Directory.Move(src, dst)
            Else
                CopyDirectoryRecursive(src, dst)
            End If
        End Sub
        Private Shared Sub SplitInstallByEmulator(oldFolder As String, newFolder As String, emulator As String, doMove As Boolean)
            Dim patterns As String() = GetPatternsForEmulator(emulator)
            If patterns.Length = 0 Then Exit Sub

            ' First pass: collect matching files
            Dim matches As New List(Of String)

            For Each pattern In patterns
                matches.AddRange(Directory.EnumerateFiles(oldFolder, pattern, SearchOption.AllDirectories))
            Next

            ' No files for this emulator => do NOT create folder
            If matches.Count = 0 Then Exit Sub

            ' Now we know we need the folder
            Directory.CreateDirectory(newFolder)

            ' Move/copy matches
            For Each filePath In matches.Distinct(StringComparer.OrdinalIgnoreCase)
                Dim rel = Path.GetRelativePath(oldFolder, filePath)
                Dim destPath = Path.Combine(newFolder, rel)
                Directory.CreateDirectory(Path.GetDirectoryName(destPath))

                If doMove Then
                    If File.Exists(destPath) Then Continue For
                    File.Move(filePath, destPath)
                Else
                    If File.Exists(destPath) Then Continue For
                    File.Copy(filePath, destPath)
                End If
            Next
        End Sub
        Private Shared Sub DeleteEmptyDirectories(root As String)
            If Not Directory.Exists(root) Then Exit Sub

            For Each DR In Directory.EnumerateDirectories(root, "*", SearchOption.AllDirectories).OrderByDescending(Function(d) d.Length)
                Try
                    If Not Directory.EnumerateFileSystemEntries(DR).Any() Then
                        Directory.Delete(DR, recursive:=False)
                    End If
                Catch
                    ' ignore locked/access issues
                End Try
            Next
        End Sub
        Private Shared Function GetPatternsForEmulator(emulator As String) As String()
            Select Case emulator.Trim().ToLowerInvariant()
                Case "doja", "star"
                    Return {"*.jam", "*.jar", "*.sp", "*.scr"}
                Case "jsky", "vodafone", "softbank", "airedge"
                    Return {"*.jad", "*.jar", "*.jam"} ' some packs still include jam
                Case "flash"
                    Return {"*.swf", "*.spl"}
                Case Else
                    ' Fallback: move/copy nothing specific
                    Return Array.Empty(Of String)()
            End Select
        End Function
        Private Shared Sub CopyDirectoryRecursive(sourceDir As String, targetDir As String)
            Directory.CreateDirectory(targetDir)

            For Each f In Directory.GetFiles(sourceDir)
                Dim dest = Path.Combine(targetDir, Path.GetFileName(f))
                If Not File.Exists(dest) Then File.Copy(f, dest)
            Next

            For Each d In Directory.GetDirectories(sourceDir)
                CopyDirectoryRecursive(Dir, Path.Combine(targetDir, Path.GetFileName(d)))
            Next
        End Sub
    End Class
End Namespace
