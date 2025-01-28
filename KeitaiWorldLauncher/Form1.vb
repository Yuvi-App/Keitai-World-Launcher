Imports System.IO
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
    Dim Dojapath As String
    Dim DojaAppPath As String
    Dim DojaEXE As String
    Dim DojaHideUI As Boolean
    Dim DOJASoundType As String
    Dim Starpath As String
    Dim StarAppPath As String
    Dim StarEXE As String
    Dim MachiCharapath As String
    Dim MachiCharaExe As String

    ' FORM LOAD
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

        ' Setup any Config Suff
        chkbxHidePhoneUI.Checked = DojaHideUI
        Dim index As Integer = cobxAudioType.FindStringExact(DOJASoundType)
        cobxAudioType.SelectedIndex = index

        ' Load Game List
        Try
            games = gameListManager.LoadGames()
            lblTotalGameCount.Text = "Total: " & games.Count
            ' Clear the ListView and ImageList
            ListViewGames.Items.Clear()
            ImageListGames.Images.Clear()
            ListViewGames.Columns.Add("Title", GroupBox1.Width - 20, HorizontalAlignment.Left)

            ' Add icons to the ImageList
            For Each game In games
                Dim DefaulticonPath As String = "data\tools\icons\default.gif"
                If File.Exists(DefaulticonPath) Then
                    ImageListGames.Images.Add(game.ENTitle, Image.FromFile(DefaulticonPath))
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
            MachiCharas = machicharaListManager.LoadMachiChara()
            For Each mc In machicharas
                lbxMachiCharaList.Items.Add(mc.ENTitle)
            Next
        Catch ex As Exception
            MessageBox.Show($"Failed to Load MachiChara List:{vbCrLf}{ex}")
        End Try
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
                ' Load JAM Controls
                UtilManager.GenerateDynamicControlsFromLines(CurrentSelectedGameJAM, gbxGameInfo)
            Else
                ' Download the game, extract it, and load JAM controls
                Dim result = MessageBox.Show($"The game '{selectedGame.ENTitle} ({selectedGame.ZIPName})' is not downloaded. Would you like to download it?", "Download Game", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                If result = DialogResult.Yes Then
                    Dim GameDownloader As New GameDownloader(pbGameDL)
                    GameDownloader.DownloadGameAsync(selectedGame.DownloadURL, DownloadFileZipPath, $"{DownloadsFolder}\{Path.GetFileNameWithoutExtension(selectedGame.ZIPName)}", CurrentSelectedGameJAM, False)
                End If
            End If

            ' Enable game launch button and checkbox
            btnLaunchGame.Enabled = True
            chkbxHidePhoneUI.Enabled = True
            cobxAudioType.Enabled = True
        End If
    End Sub
    Private Sub lbxMachiCharaList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lbxMachiCharaList.SelectedIndexChanged
        If lbxMachiCharaList.SelectedIndex = -1 Then Return

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


    ' CheckBox Changes
    Private Sub chkbxHidePhoneUI_CheckedChanged(sender As Object, e As EventArgs) Handles chkbxHidePhoneUI.CheckedChanged
        configManager.UpdateDOJAHideUISetting(chkbxHidePhoneUI.Checked)
    End Sub

    ' ComboBox Changes
    Private Sub cobxAudioType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cobxAudioType.SelectedIndexChanged
        If cobxAudioType.SelectedItem.ToString = "Standard" Or cobxAudioType.SelectedItem.ToString = "903i" Then
            configManager.UpdateDOJASoundSetting(cobxAudioType.SelectedItem.ToString)
        End If
    End Sub

    ' Launch Game
    Private Sub btnLaunchGame_Click(sender As Object, e As EventArgs) Handles btnLaunchGame.Click
        ' Get the selected game
        Dim selectedGameTitle As String = ListViewGames.SelectedItems(0).Text
        Dim selectedGame As Game = games.FirstOrDefault(Function(g) g.ENTitle = selectedGameTitle)
        Dim IsDojaRunning = UtilManager.CheckAndCloseDoja
        If IsDojaRunning = False Then
            utilManager.LaunchCustomDOJAGameCommand(Dojapath, DojaEXE, CurrentSelectedGameJAM)
        End If
    End Sub
    Private Sub btnMachiCharaLaunch_Click(sender As Object, e As EventArgs) Handles btnMachiCharaLaunch.Click, lbxMachiCharaList.DoubleClick
        ' Get the selected MachiChara
        Dim selectedMachiCharaTitle As String = lbxMachiCharaList.SelectedItem.ToString()
        Dim selectedMachiChara As MachiChara = machicharas.FirstOrDefault(Function(g) g.ENTitle = selectedMachiCharaTitle)
        utilManager.LaunchCustomMachiCharaCommand(MachiCharaExe, CurrentSelectedMachiCharaCFD)
    End Sub



    ' Menu Strip Items
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
                Await GameDownloader.DownloadGameAsync(GURL.DownloadURL, DownloadFileZipPath, $"{DownloadsFolder}\{Path.GetFileNameWithoutExtension(GURL.ZIPName)}", CurrentSelectedGameJAM, True)

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
End Class