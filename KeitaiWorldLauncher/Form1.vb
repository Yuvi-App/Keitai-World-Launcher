Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports KeitaiWorldLauncher.My.Managers
Imports KeitaiWorldLauncher.My.Models

Public Class Form1
    'Global Vars
    Dim configManager As New ConfigManager()
    Dim utilManager As New UtilManager
    Dim gameListManager As New GameListManager()
    Dim machicharaListManager As New MachiCharaListManager()
    Dim zipManager As New ZipManager()
    Dim config As Dictionary(Of String, String)
    Dim games As List(Of Game)
    Dim machicharas As List(Of MachiChara)

    'Directory Var
    Dim DownloadsFolder = "data\downloads"
    Dim ToolsFolder = "data\tools"

    'Index Vars Can Change
    Dim CurrentSelectedGameJAM As String
    Dim CurrentSelectedGameJAR As String
    Dim CurrentSelectedMachiCharaCFD As String

    'Config Vars
    Dim versionCheckUrl As String
    Dim autoUpdate As Boolean
    Dim gameListUrl As String
    Dim autoUpdateGameList As Boolean
    Dim machicharaListUrl As String
    Dim autoUpdatemachicharaList As Boolean
    Dim UseShaderGlass As Boolean
    Public Dojapath As String
    Public DojaAppPath As String
    Public DojaEXE As String
    Dim DojaHideUI As Boolean
    Dim DOJASoundType As String
    Public Starpath As String
    Public StarAppPath As String
    Public StarEXE As String
    Dim MachiCharapath As String
    Dim MachiCharaExe As String

    ' FORM LOAD
    Private Sub Form1_Closing(sender As Object, e As EventArgs) Handles MyBase.Closing
        UtilManager.CheckAndCloseDoja()
        UtilManager.CheckAndCloseStar()
    End Sub
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Setup SJIS 
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)

        ' Setup Dirs
        UtilManager.SetupDIRS()

        ' Check PreREQs
        UtilManager.CheckforPreReq()

        ' Load Config
        config = configManager.LoadConfig()

        ' Access Config settings
        versionCheckUrl = config("VersionCheckURL")
        autoUpdate = Boolean.Parse(config("AutoUpdate"))
        gameListUrl = config("GamelistURL")
        autoUpdateGameList = Boolean.Parse(config("AutoUpdateGameList"))
        machicharaListUrl = config("MachiCharalistURL")
        autoUpdatemachicharaList = Boolean.Parse(config("AutoUpdateMachiCharaList"))
        UseShaderGlass = Boolean.Parse(config("UseShaderGlass"))
        Dojapath = config("DOJAPath")
        DojaAppPath = Dojapath & "\app"
        DojaEXE = config("DOJAEXEPath")
        DojaHideUI = Boolean.Parse(config("DOJAHideUI"))
        DOJASoundType = config("DOJASoundType")
        Starpath = config("STARPath")
        StarAppPath = StarAppPath & "\app"
        StarEXE = config("STAREXEPath")
        MachiCharapath = config("MachiCharaPath")
        MachiCharaExe = config("MachiCharaEXEPath")

        ' Check for App update
        If autoUpdate = True Then
            UtilManager.CheckForUpdates(versionCheckUrl)
        End If

        ' Get Updated Game List  
        If autoUpdateGameList = True Then
            GameListManager.DownloadGameList(gameListUrl)
        End If

        ' Get Updated MachiChara List  
        If autoUpdatemachicharaList = True Then
            MachiCharaListManager.DownloadMachiCharaList(machicharaListUrl)
        End If

        ' Load Game List
        Try
            games = gameListManager.LoadGames()
            lblTotalGameCount.Text = "Total: " & games.Count
            ' Clear the ListView and ImageList
            ListViewGames.Items.Clear()
            ImageListGames.Images.Clear()
            ListViewGames.Columns.Add("Title", GroupBox1.Width - 20, HorizontalAlignment.Left)

            ' Add icons to the ImageList
            Dim DojaIcon = $"{ToolsFolder}\icons\doja.gif"
            Dim StarIcon = $"{ToolsFolder}\icons\star.gif"
            If File.Exists(DojaIcon) = False Or File.Exists(StarIcon) = False Then
                MessageBox.Show("Missing Doja/Star Defualt icons.")
            End If
            For Each game In games
                If game.Emulator.ToLower = "doja" Then
                    ImageListGames.Images.Add(game.ENTitle, Image.FromFile(DojaIcon))
                ElseIf game.Emulator.ToLower = "star" Then
                    ImageListGames.Images.Add(game.ENTitle, Image.FromFile(StarIcon))
                End If
            Next

            ' Assign the ImageList to the ListView
            ListViewGames.SmallImageList = ImageListGames

            ' Add games to the ListView
            For Each game In games
                Dim item As New ListViewItem(game.ENTitle)
                item.ImageKey = game.ENTitle ' Use the game title as the key for the icon
                ListViewGames.Items.Add(item)
            Next
        Catch ex As Exception
            MessageBox.Show($"Failed to Load Game List:{vbCrLf}{ex}")
        End Try

        ' Load MachiChara List
        Try
            machicharas = machicharaListManager.LoadMachiChara()
            For Each mc In machicharas
                lbxMachiCharaList.Items.Add(mc.ENTitle)
            Next
        Catch ex As Exception
            MessageBox.Show($"Failed to Load MachiChara List:{vbCrLf}{ex}")
        End Try

        ' Setup any Config Suff
        chkbxHidePhoneUI.Checked = DojaHideUI
        Dim atindex As Integer = cobxAudioType.FindStringExact(DOJASoundType)
        cobxAudioType.SelectedIndex = atindex
        chkbxShaderGlass.Checked = UseShaderGlass
        cbxEmuType.SelectedIndex = 0
    End Sub

    ' General Other Function
    Private Sub FilterListView()
        ' Get the selected emulator filter and search term
        Dim selectedFilter As String = cbxEmuType.SelectedItem.ToString().ToLower()
        Dim searchTerm As String = txtLVSearch.Text.Trim().ToLower()

        ' Clear the ListView
        ListViewGames.Items.Clear()

        ' Filter games based on the selected emulator and search term
        For Each game In games
            Dim matchesEmulator As Boolean = (selectedFilter = "all" OrElse game.Emulator.ToLower() = selectedFilter)
            Dim matchesSearch As Boolean = game.ENTitle.ToLower().Contains(searchTerm)

            ' Add to ListView only if both conditions are met
            If matchesEmulator AndAlso matchesSearch Then
                Dim item As New ListViewItem(game.ENTitle)

                ' Assign the appropriate icon based on the emulator type
                item.ImageKey = game.ENTitle ' Assign the correct icon
                ListViewGames.Items.Add(item)
            End If
        Next
    End Sub
    Private Sub DownloadGame(ContextDownload As Boolean)
        ' Get the selected game title from the ListView
        Dim selectedGameTitle As String = ListViewGames.SelectedItems(0).Text
        Dim selectedGame As Game = games.FirstOrDefault(Function(g) g.ENTitle = selectedGameTitle)

        If selectedGame IsNot Nothing Then
            CurrentSelectedGameJAM = $"{DownloadsFolder}\{Path.GetFileNameWithoutExtension(selectedGame.ZIPName)}\bin\{Path.GetFileNameWithoutExtension(selectedGame.ZIPName)}.jam"
            CurrentSelectedGameJAR = $"{DownloadsFolder}\{Path.GetFileNameWithoutExtension(selectedGame.ZIPName)}\bin\{Path.GetFileNameWithoutExtension(selectedGame.ZIPName)}.jar"

            ' Check if the game is already downloaded
            Dim localFilePath As String = CurrentSelectedGameJAR
            Dim DownloadFileZipPath As String = $"{DownloadsFolder}\{selectedGame.ZIPName}"
            If File.Exists(localFilePath) Then
                If ContextDownload = True Then
                    Dim result = MessageBox.Show($"The game '{selectedGame.ENTitle} ({selectedGame.ZIPName})' is already downloaded. Would you like to download it again? {vbCrLf}{vbCrLf}This could delete you're save data so please becareful", "Download Game Again", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    If result = DialogResult.Yes Then
                        Dim GameDownloader As New GameDownloader(pbGameDL)
                        Dim ExtractFolder As String = $"{DownloadsFolder}\{Path.GetFileNameWithoutExtension(selectedGame.ZIPName)}"
                        GameDownloader.DownloadGameAsync(selectedGame.DownloadURL, DownloadFileZipPath, ExtractFolder, selectedGame, CurrentSelectedGameJAM, False)
                        MessageBox.Show($"Completed redownload of '{selectedGame.ENTitle}'")
                    End If
                End If
                ' Load JAM Controls
                UtilManager.GenerateDynamicControlsFromLines(CurrentSelectedGameJAM, gbxGameInfo)
            Else
                ' Download the game, extract it, and load JAM controls
                Dim result = MessageBox.Show($"The game '{selectedGame.ENTitle} ({selectedGame.ZIPName})' is not downloaded. Would you like to download it?", "Download Game", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                If result = DialogResult.Yes Then
                    Dim GameDownloader As New GameDownloader(pbGameDL)
                    Dim ExtractFolder As String = $"{DownloadsFolder}\{Path.GetFileNameWithoutExtension(selectedGame.ZIPName)}"
                    GameDownloader.DownloadGameAsync(selectedGame.DownloadURL, DownloadFileZipPath, ExtractFolder, selectedGame, CurrentSelectedGameJAM, False)
                End If
            End If

            ' Enable game launch button and checkbox
            btnLaunchGame.Enabled = True
            chkbxHidePhoneUI.Enabled = True
            cobxAudioType.Enabled = True
            chkbxShaderGlass.Enabled = True
        End If
    End Sub
    Private Sub DownloadMachiChara()
        ' Get the selected game
        Dim selectedMachiCharaTitle As String = lbxMachiCharaList.SelectedItem.ToString()
        Dim selectedMachiChara As MachiChara = machicharas.FirstOrDefault(Function(g) g.ENTitle = selectedMachiCharaTitle)

        If selectedMachiChara IsNot Nothing Then
            CurrentSelectedMachiCharaCFD = $"{DownloadsFolder}\{selectedMachiChara.CFDName}"

            ' Check if the MC is already downloaded
            Dim localFilePath As String = CurrentSelectedMachiCharaCFD
            Dim DownloadFilePath As String = $"{DownloadsFolder}\{selectedMachiChara.CFDName}"
            If File.Exists(localFilePath) Then

            Else
                ' Download the machi chara 
                Dim result = MessageBox.Show($"The Machi Chara '{selectedMachiChara.ENTitle} ({selectedMachiChara.CFDName})' is not downloaded. Would you like to download it?", "Download Machi Chara", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                If result = DialogResult.Yes Then
                    Dim MachiCharaDownloader As New MachiCharaDownloader(pbGameDL)
                    MachiCharaDownloader.DownloadMachiCharaAsync(selectedMachiChara.DownloadURL, DownloadFilePath, False)
                End If
            End If
            btnMachiCharaLaunch.Enabled = True
        End If
    End Sub
    Private Sub DeleteGame()
        ' Get the selected game title from the ListView
        Dim selectedGameTitle As String = ListViewGames.SelectedItems(0).Text
        Dim selectedGame As Game = games.FirstOrDefault(Function(g) g.ENTitle = selectedGameTitle)

        If selectedGame IsNot Nothing Then
            Dim CurrentSelectedGameFolder = $"{DownloadsFolder}\{Path.GetFileNameWithoutExtension(selectedGame.ZIPName)}"

            ' Check if the game is already downloaded
            If Directory.Exists(CurrentSelectedGameFolder) Then
                Dim result = MessageBox.Show($"'{selectedGame.ENTitle} ({selectedGame.ZIPName})' will be deleted is that okay?", "Delete Game", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                If result = DialogResult.Yes Then
                    My.Computer.FileSystem.DeleteDirectory(CurrentSelectedGameFolder, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                    MessageBox.Show($"Deleted '{selectedGame.ENTitle}' successfully.")
                End If
            Else
                MessageBox.Show($"'{selectedGame.ENTitle} ({selectedGame.ZIPName})' is not Downloaded, Unable to Delete.")
            End If
        End If
    End Sub

    ' LISTBOX/LISTVIEW CHANGES
    'Private Sub ListBoxGames_SelectedIndexChanged(sender As Object, e As EventArgs)
    '    If ListBoxGames.SelectedIndex = -1 Then Return

    '    ' Get the selected game
    '    Dim selectedGameTitle = ListBoxGames.SelectedItem.ToString
    '    Dim selectedGame = games.FirstOrDefault(Function(g) g.ENTitle = selectedGameTitle)

    '    If selectedGame IsNot Nothing Then
    '        CurrentSelectedGameJAM = $"{DownloadsFolder}\{Path.GetFileNameWithoutExtension(selectedGame.ZIPName)}\bin\{Path.GetFileNameWithoutExtension(selectedGame.ZIPName)}.jam"
    '        CurrentSelectedGameJAR = $"{DownloadsFolder}\{Path.GetFileNameWithoutExtension(selectedGame.ZIPName)}\bin\{Path.GetFileNameWithoutExtension(selectedGame.ZIPName)}.jar"
    '        ' Check if the game is already downloaded
    '        Dim localFilePath = CurrentSelectedGameJAR
    '        Dim DownloadFileZipPath = $"{DownloadsFolder}\{selectedGame.ZIPName}"
    '        If File.Exists(localFilePath) Then 'Load JAM Controls
    '            'MessageBox.Show($"The game '{selectedGame.ENTitle} ({Path.GetFileNameWithoutExtension(selectedGame.ZIPName)})' is already downloaded.", "Game Found", MessageBoxButtons.OK, MessageBoxIcon.Information)
    '            UtilManager.GenerateDynamicControlsFromLines(CurrentSelectedGameJAM, gbxGameInfo)
    '        Else
    '            ' Download the game & Extract it & Load JAM Controls
    '            Dim result = MessageBox.Show($"The game '{selectedGame.ENTitle} ({selectedGame.ZIPName})' is not downloaded. Would you like to download it?", "Download Game", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
    '            If result = DialogResult.Yes Then
    '                Dim GameDownloader As New GameDownloader(pbGameDL)
    '                GameDownloader.DownloadGameAsync(selectedGame.DownloadURL, DownloadFileZipPath, $"{DownloadsFolder}\{Path.GetFileNameWithoutExtension(selectedGame.ZIPName)}", CurrentSelectedGameJAM, False)
    '            End If
    '        End If

    '        btnLaunchGame.Enabled = True
    '        chkbxHidePhoneUI.Enabled = True
    '    End If
    'End Sub
    Private Sub ListViewGames_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListViewGames.SelectedIndexChanged
        If ListViewGames.SelectedItems.Count = 0 Then Return
        DownloadGame(False)
    End Sub
    Private Sub lbxMachiCharaList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lbxMachiCharaList.SelectedIndexChanged
        If lbxMachiCharaList.SelectedIndex = -1 Then Return
        DownloadMachiChara()
    End Sub

    ' CheckBox Changes
    Private Sub chkbxHidePhoneUI_CheckedChanged(sender As Object, e As EventArgs) Handles chkbxHidePhoneUI.CheckedChanged
        configManager.UpdateDOJAHideUISetting(chkbxHidePhoneUI.Checked)
    End Sub
    Private Sub chkbxShaderGlass_CheckedChanged(sender As Object, e As EventArgs) Handles chkbxShaderGlass.CheckedChanged
        configManager.UpdateUseShaderGlassSetting(chkbxShaderGlass.Checked)
    End Sub

    ' ComboBox Changes
    Private Sub cobxAudioType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cobxAudioType.SelectedIndexChanged
        If cobxAudioType.SelectedItem.ToString = "Standard" Or cobxAudioType.SelectedItem.ToString = "903i" Then
            configManager.UpdateDOJASoundSetting(cobxAudioType.SelectedItem.ToString)
        End If
    End Sub
    Private Sub cbxEmuType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxEmuType.SelectedIndexChanged
        FilterListView()
    End Sub


    'Textbox Changes
    Private Sub txtLVSearch_TextChanged(sender As Object, e As EventArgs) Handles txtLVSearch.TextChanged
        FilterListView()
    End Sub

    'ContextMenuStrip Changes
    Private Sub cmsGameLV_Download_Click(sender As Object, e As EventArgs) Handles cmsGameLV_Download.Click
        DownloadGame(True)
    End Sub
    Private Sub cmsGameLV_Delete_Click(sender As Object, e As EventArgs) Handles cmsGameLV_Delete.Click
        DeleteGame()
    End Sub

    'Launch Game
    Private Sub btnLaunchGame_Click(sender As Object, e As EventArgs) Handles btnLaunchGame.Click, ListViewGames.DoubleClick, cmsGameLV_Launch.Click
        ' Get the selected game
        Try
            Dim selectedGameTitle As String = ListViewGames.SelectedItems(0).Text
            Dim selectedGame As Game = games.FirstOrDefault(Function(g) g.ENTitle = selectedGameTitle)
            Select Case selectedGame.Emulator.ToLower
                Case "doja"
                    Dim IsDojaRunning = UtilManager.CheckAndCloseDoja
                    If IsDojaRunning = False Then
                        utilManager.LaunchCustomDOJAGameCommand(Dojapath, DojaEXE, CurrentSelectedGameJAM)
                    End If
                Case "star"
                    Dim IsStarRunning = UtilManager.CheckAndCloseStar
                    If IsStarRunning = False Then
                        utilManager.LaunchCustomSTARGameCommand(Starpath, StarEXE, CurrentSelectedGameJAM)
                    End If
            End Select
        Catch ex As Exception
            MessageBox.Show($"Error Launching Game:{vbCrLf}{ex}")
        End Try
    End Sub
    Private Sub btnMachiCharaLaunch_Click(sender As Object, e As EventArgs) Handles btnMachiCharaLaunch.Click, lbxMachiCharaList.DoubleClick
        ' Get the selected MachiChara
        Dim selectedMachiCharaTitle As String = lbxMachiCharaList.SelectedItem.ToString()
        Dim selectedMachiChara As MachiChara = machicharas.FirstOrDefault(Function(g) g.ENTitle = selectedMachiCharaTitle)
        utilManager.LaunchCustomMachiCharaCommand(MachiCharaExe, CurrentSelectedMachiCharaCFD)
    End Sub

    'Menu Strip Items
    Private Async Sub GamesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GamesToolStripMenuItem.Click
        'Batch Download all games in game List
        Dim result = MessageBox.Show($"This will download all games... This might take awhile are you sure?", "Download All Games", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If result = DialogResult.Yes Then
            Await DownloadGamesAsync(games)
        End If
    End Sub
    Public Async Function DownloadGamesAsync(games As List(Of Game)) As Task
        'Batch Download all games in game List
        Dim GameDownloader As New GameDownloader(pbGameDL)
        Dim SuccessDLCount = 0
        Dim SkippedCount = 0
        For Each GURL In games
            Dim DownloadFileZipPath As String = $"{DownloadsFolder}\{GURL.ZIPName}"

            ' Check if already downloaded
            If File.Exists($"{DownloadsFolder}\{Path.GetFileNameWithoutExtension(GURL.ZIPName)}\bin\{Path.GetFileNameWithoutExtension(GURL.ZIPName)}.jar") Then
                SkippedCount += 1
            Else
                ' Download and wait for it to finish
                Await GameDownloader.DownloadGameAsync(GURL.DownloadURL, DownloadFileZipPath, $"{DownloadsFolder}\{Path.GetFileNameWithoutExtension(GURL.ZIPName)}", GURL, CurrentSelectedGameJAM, True)

                ' Check if downloaded and set up correctly
                If File.Exists($"{DownloadsFolder}\{Path.GetFileNameWithoutExtension(GURL.ZIPName)}\bin\{Path.GetFileNameWithoutExtension(GURL.ZIPName)}.jar") Then
                    SuccessDLCount += 1
                End If
            End If
        Next
        MessageBox.Show($"Total Games Available {games.Count}{vbCrLf}Downloaded Successfully: {SuccessDLCount}{vbCrLf}Skipped: {SkippedCount}")
    End Function
    Private Async Sub MachiCharaToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MachiCharaToolStripMenuItem.Click
        'Batch Download all MachiCharas in MC List
        Dim result = MessageBox.Show($"This will download all MachiChara's... This might take awhile are you sure?", "Download All MachiChara's", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If result = DialogResult.Yes Then
            Await DownloadMachiCharaAsync(machicharas)
        End If
    End Sub
    Public Async Function DownloadMachiCharaAsync(machicharas As List(Of MachiChara)) As Task
        Dim MachiCharaDownloader As New MachiCharaDownloader(pbGameDL)
        Dim SuccessDLCount = 0
        Dim SkippedCount = 0
        For Each MCURL In machicharas
            Dim DownloadFilePath As String = $"{DownloadsFolder}\{MCURL.CFDName}"

            ' Check if already downloaded
            If File.Exists($"{DownloadsFolder}\{MCURL.CFDName}") Then
                SkippedCount += 1
            Else
                ' Download and wait for it to finish
                Await MachiCharaDownloader.DownloadMachiCharaAsync(MCURL.DownloadURL, DownloadFilePath, True)

                ' Check if downloaded and set up correctly
                If File.Exists($"{DownloadsFolder}\{MCURL.CFDName}") Then
                    SuccessDLCount += 1
                End If
            End If
        Next
        MessageBox.Show($"Total MachiChara Available {machicharas.Count}{vbCrLf}Downloaded Successfully: {SuccessDLCount}{vbCrLf}Skipped: {SkippedCount}")
    End Function
    Private Sub AboutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutToolStripMenuItem.Click
        MessageBox.Show($"Keitai.World Launcher{vbCrLf}Created by Yuvi{vbCrLf}Ver:{KeitaiWorldLauncher.My.Application.Info.Version.ToString}")
    End Sub
    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Application.Exit()
    End Sub
    Private Sub GamelistToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GamelistToolStripMenuItem.Click
        OpenFileDialog1.Title = "Select Master Game Zip file"
        If OpenFileDialog1.ShowDialog = DialogResult.OK Then
            utilManager.ProcessZipFileforGamelist(OpenFileDialog1.FileName)
        End If
    End Sub
    Private Sub RefreshToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RefreshToolStripMenuItem.Click
        Application.Restart()
    End Sub
    Private Sub KeyConfiguratorToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles KeyConfiguratorToolStripMenuItem.Click
        Dim Apppath = $"{ToolsFolder}\antimicrox\bin\antimicrox.exe"
        Dim startInfo As New ProcessStartInfo()
        startInfo.FileName = Apppath
        startInfo.UseShellExecute = False
        startInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory ' Set working directory
        Dim process As Process = Process.Start(startInfo)
    End Sub
    Private Sub AppConfigToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AppConfigToolStripMenuItem.Click
        Dim Apppath = $"cmd"
        Dim startInfo As New ProcessStartInfo()
        startInfo.FileName = Apppath
        startInfo.Arguments = $"/C {Chr(34)}notepad {AppDomain.CurrentDomain.BaseDirectory}\configs\appconfig.xml {Chr(34)}"
        startInfo.UseShellExecute = False
        startInfo.CreateNoWindow = True
        startInfo.RedirectStandardOutput = True
        startInfo.RedirectStandardError = True
        startInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
        Dim process As Process = Process.Start(startInfo)
    End Sub
End Class