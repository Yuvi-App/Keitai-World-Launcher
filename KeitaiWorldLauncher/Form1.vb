Imports System.IO
Imports System.Net.Security
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports KeitaiWorldLauncher.My.logger
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

        ' Setup Logging
        Logger.InitializeLogger()
        Logger.LogInfo("Application started.")

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
        Logger.LogInfo("Getting App Update")
        If autoUpdate = True Then
            UtilManager.CheckForUpdates(versionCheckUrl)
        End If

        ' Get Updated Game List  
        Logger.LogInfo("Getting Gamelist.xml")
        If autoUpdateGameList = True Then
            GameListManager.DownloadGameList(gameListUrl)
        End If

        ' Get Updated MachiChara List  
        Logger.LogInfo("Getting Machichara.xml")
        If autoUpdatemachicharaList = True Then
            MachiCharaListManager.DownloadMachiCharaList(machicharaListUrl)
        End If

        ' Load Game List
        Logger.LogInfo("Processing gamelist.xml")
        Try
            games = gameListManager.LoadGames()
            lblTotalGameCount.Text = "Total: " & games.Count
            ' Clear the ListView and ImageList
            ListViewGames.Items.Clear()
            ImageListGames.Images.Clear()
            ListViewGamesVarients.Clear()
            ListViewGames.Columns.Add("Title", GroupBox1.Width - 20, HorizontalAlignment.Left)
            ListViewGamesVarients.Columns.Add("Varients", GroupBox1.Width - 20, HorizontalAlignment.Left)

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
            Logger.LogError("Failed to Load Game List", ex)
        End Try

        ' Load MachiChara List
        Logger.LogInfo("Processing machichara.xml")
        Try
            machicharas = machicharaListManager.LoadMachiChara()
            For Each mc In machicharas
                lbxMachiCharaList.Items.Add(mc.ENTitle)
            Next
        Catch ex As Exception
            MessageBox.Show($"Failed to Load MachiChara List:{vbCrLf}{ex}")
            Logger.LogError("Failed to Load MachiChara List", ex)
        End Try

        ' Setup any Config Suff
        chkbxHidePhoneUI.Checked = DojaHideUI
        Dim atindex As Integer = cobxAudioType.FindStringExact(DOJASoundType)
        cobxAudioType.SelectedIndex = atindex
        chkbxShaderGlass.Checked = UseShaderGlass
        cbxFilterType.SelectedIndex = 0

        'Last Step
        RefreshGameHighlighting()
    End Sub

    ' General Other Function
    Private Sub EnableButtons()
        ' Enable game launch button and checkbox
        btnLaunchGame.Enabled = True
        chkbxHidePhoneUI.Enabled = True
        cobxAudioType.Enabled = True
        chkbxShaderGlass.Enabled = True
    End Sub
    Private Sub FilterListView()
        ' Get the selected emulator filter and search term
        Dim selectedFilter As String = cbxFilterType.SelectedItem.ToString().ToLower()
        Dim searchTerm As String = txtLVSearch.Text.Trim().ToLower()

        ' Clear the ListView
        ListViewGames.Items.Clear()

        If selectedFilter = "favorites" Then
            ' Ensure the favorites file exists
            Dim favoritesFile As String = "configs\favorites.txt"
            If Not File.Exists(favoritesFile) Then Return

            ' Load favorites into a HashSet for fast lookups
            Dim favoriteGames As HashSet(Of String) = File.ReadAllLines(favoritesFile).
                                              Select(Function(fav) fav.Trim()).
                                              ToHashSet(StringComparer.OrdinalIgnoreCase)

            ' Loop through the games and check if they are in favorites
            For Each game In games
                If favoriteGames.Contains(game.ENTitle) Then
                    Dim matchesSearch As Boolean = game.ENTitle.ToLower().Contains(searchTerm)

                    ' Add to ListView only if search term matches
                    If matchesSearch Then
                        Dim item As New ListViewItem(game.ENTitle)

                        ' Assign the appropriate icon based on the emulator type
                        item.ImageKey = game.ENTitle ' Assign the correct icon
                        ListViewGames.Items.Add(item)
                    End If
                End If
            Next
        ElseIf selectedFilter = "installed" Then
            ' Get the list of installed game folders
            Dim installedGames = Directory.GetDirectories(DownloadsFolder).
                                  Select(Function(folder) Path.GetFileName(folder)).
                                  ToHashSet(StringComparer.OrdinalIgnoreCase) ' HashSet for fast lookups

            ' Loop through all games and check if they are installed
            For Each game In games
                If installedGames.Contains(Path.GetFileNameWithoutExtension(game.ZIPName)) Then
                    Dim matchesSearch As Boolean = game.ENTitle.ToLower().Contains(searchTerm)

                    ' Add to ListView only if search term matches
                    If matchesSearch Then
                        Dim item As New ListViewItem(game.ENTitle)

                        ' Assign the appropriate icon based on the emulator type
                        item.ImageKey = game.ENTitle ' Assign the correct icon
                        ListViewGames.Items.Add(item)
                    End If
                End If
            Next
        Else
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
        End If
        RefreshGameHighlighting()
    End Sub
    Private Sub DownloadGame(ContextDownload As Boolean)
        ' Get the selected game title from the ListView
        Dim selectedGameTitle As String = ListViewGames.SelectedItems(0).Text
        Dim selectedGame As Game = games.FirstOrDefault(Function(g) g.ENTitle = selectedGameTitle)

        If selectedGame IsNot Nothing Then
            CurrentSelectedGameJAM = $"{DownloadsFolder}\{Path.GetFileNameWithoutExtension(selectedGame.ZIPName)}\bin\{Path.GetFileNameWithoutExtension(selectedGame.ZIPName)}.jam"
            CurrentSelectedGameJAR = $"{DownloadsFolder}\{Path.GetFileNameWithoutExtension(selectedGame.ZIPName)}\bin\{Path.GetFileNameWithoutExtension(selectedGame.ZIPName)}.jar"

            ' Check if the game is already downloaded
            Logger.LogInfo($"Checking for {CurrentSelectedGameJAR}")
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
                    Logger.LogInfo($"Starting Download for {selectedGame.DownloadURL}")
                    Dim GameDownloader As New GameDownloader(pbGameDL)
                    Dim ExtractFolder As String = $"{DownloadsFolder}\{Path.GetFileNameWithoutExtension(selectedGame.ZIPName)}"
                    GameDownloader.DownloadGameAsync(selectedGame.DownloadURL, DownloadFileZipPath, ExtractFolder, selectedGame, CurrentSelectedGameJAM, False)
                End If
            End If
        End If
    End Sub
    Private Sub DownloadMachiChara()
        ' Get the selected game
        Dim selectedMachiCharaTitle As String = lbxMachiCharaList.SelectedItem.ToString()
        Dim selectedMachiChara As MachiChara = machicharas.FirstOrDefault(Function(g) g.ENTitle = selectedMachiCharaTitle)

        If selectedMachiChara IsNot Nothing Then
            Logger.LogInfo($"Checking for {DownloadsFolder}\{selectedMachiChara.CFDName}")
            CurrentSelectedMachiCharaCFD = $"{DownloadsFolder}\{selectedMachiChara.CFDName}"

            ' Check if the MC is already downloaded
            Dim localFilePath As String = CurrentSelectedMachiCharaCFD
            Dim DownloadFilePath As String = $"{DownloadsFolder}\{selectedMachiChara.CFDName}"
            If File.Exists(localFilePath) Then

            Else
                ' Download the machi chara 
                Dim result = MessageBox.Show($"The Machi Chara '{selectedMachiChara.ENTitle} ({selectedMachiChara.CFDName})' is not downloaded. Would you like to download it?", "Download Machi Chara", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                If result = DialogResult.Yes Then
                    Logger.LogInfo($"Starting Download for {selectedMachiChara.DownloadURL}")
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
            RefreshGameHighlighting()
        End If
    End Sub
    Public Sub RefreshGameHighlighting()
        Dim favoritesManager As New FavoritesManager()

        For Each item As ListViewItem In ListViewGames.Items
            ' Get the corresponding game object
            Dim selectedGame As Game = games.FirstOrDefault(Function(g) g.ENTitle = item.Text)

            If selectedGame IsNot Nothing Then
                ' Check if the game is favorited
                Dim isFavorited As Boolean = favoritesManager.IsGameFavorited(item.Text)

                ' Check if the game is installed
                Dim gameFolder As String = Path.Combine(DownloadsFolder, Path.GetFileNameWithoutExtension(selectedGame.ZIPName))
                Dim isInstalled As Boolean = Directory.Exists(gameFolder)

                ' Determine the appropriate highlighting
                If isInstalled AndAlso isFavorited Then
                    item.BackColor = Color.LightSeaGreen ' Both installed and favorited (customizable color)
                ElseIf isInstalled Then
                    item.BackColor = Color.LightGreen ' Only installed
                ElseIf isFavorited Then
                    item.BackColor = Color.LightGoldenrodYellow ' Only favorited
                Else
                    item.BackColor = Color.White ' Neither installed nor favorited
                End If
            End If
        Next
    End Sub
    Public Function VerifyGameDownloaded()
        Dim selectedGameTitle As String = ListViewGames.SelectedItems(0).Text
        Dim selectedGame As Game = games.FirstOrDefault(Function(g) g.ENTitle = selectedGameTitle)
        For Each f In Directory.GetDirectories(DownloadsFolder)
            If Path.GetFileName(f) = Path.GetFileNameWithoutExtension(selectedGame.ZIPName) Then
                Return True
            End If
        Next
        Return False
    End Function

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
        EnableButtons()
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
    Private Sub cbxEmuType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxFilterType.SelectedIndexChanged
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
    Private Sub FavoriteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FavoriteToolStripMenuItem.Click
        If ListViewGames.SelectedItems.Count = 0 Then Return

        Dim selectedGameTitle As String = ListViewGames.SelectedItems(0).Text
        Dim favoritesManager As New FavoritesManager()

        If favoritesManager.IsGameFavorited(selectedGameTitle) Then
            favoritesManager.RemoveFromFavorites(selectedGameTitle)
            MessageBox.Show($"{selectedGameTitle} has been removed from favorites.", "Favorite Removed", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            favoritesManager.AddToFavorites(selectedGameTitle)
            MessageBox.Show($"{selectedGameTitle} has been added to favorites.", "Favorite Added", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

        ' Optionally refresh the UI to indicate favorite status
        RefreshGameHighlighting()
    End Sub

    'Launch Game
    Private Sub btnLaunchGame_Click(sender As Object, e As EventArgs) Handles btnLaunchGame.Click, ListViewGames.DoubleClick, cmsGameLV_Launch.Click
        ' Get the selected game
        Try
            'Verify Its Downloaded
            If VerifyGameDownloaded() = True Then
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
            End If
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
    Private Sub AddGameToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddGameToolStripMenuItem.Click
        Dim ENTitle = InputBox("Enter the English name of the game:", "English Game Name").Trim()

        ' Validate English Game Name
        If String.IsNullOrWhiteSpace(ENTitle) Then
            MsgBox("The English game name cannot be empty. Exiting the operation.", MsgBoxStyle.Exclamation, "Input Error")
            Exit Sub
        End If

        Dim JPTitle = InputBox("Enter the Japanese name of the game (leave blank if same as English):", "Japanese Game Name").Trim()
        If String.IsNullOrWhiteSpace(JPTitle) Then
            JPTitle = ENTitle
        End If

        Dim ZIPName = InputBox("Enter the name of the zip file (include .zip at the end):", "Zip File Name").Trim()

        ' Validate ZIP Name
        If String.IsNullOrWhiteSpace(ZIPName) Then
            MsgBox("The ZIP file name cannot be empty. Exiting the operation.", MsgBoxStyle.Exclamation, "Input Error")
            Exit Sub
        End If

        If Not ZIPName.EndsWith(".zip") Then
            ZIPName += ".zip"
        End If

        Dim DownloadURL = InputBox("Enter the download URL for the zip file:", "Download URL").Trim()

        ' Validate Download URL
        If String.IsNullOrWhiteSpace(DownloadURL) Then
            MsgBox("The download URL cannot be empty. Exiting the operation.", MsgBoxStyle.Exclamation, "Input Error")
            Exit Sub

        End If

        Dim AppIconURL = InputBox("Enter the URL of a custom app icon (24x24) or leave blank to use the default icon:", "App Icon URL").Trim()
        Dim SDCardData = InputBox("Enter the SD Card data zip URL or leave blank if not applicable:", "SD Card Data URL").Trim()
        Dim Emulator = InputBox("Enter the emulator type (doja or star):", "Emulator").Trim().ToLower()

        ' Validate Emulator Type
        While Emulator <> "doja" AndAlso Emulator <> "star"
            Emulator = InputBox("Invalid input. Please enter only 'doja' or 'star':", "Emulator").Trim().ToLower()
            If String.IsNullOrWhiteSpace(Emulator) Then
                MsgBox("The emulator type cannot be empty. Exiting the operation.", MsgBoxStyle.Exclamation, "Input Error")
                Exit Sub
            End If
        End While

        Dim newGame As New Game() With {
            .ENTitle = ENTitle,
            .JPTitle = JPTitle,
            .ZIPName = ZIPName,
            .DownloadURL = DownloadURL,
            .CustomAppIconURL = AppIconURL,
            .SDCardDataURL = SDCardData,
            .Emulator = Emulator
        }
        gameListManager.AddGame(newGame)

        MessageBox.Show("Added, Make sure you download this AppConfig and upload")
    End Sub
    Private Sub ShaderGlassConfigToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShaderGlassConfigToolStripMenuItem.Click
        Dim Apppath = $"cmd"
        Dim startInfo As New ProcessStartInfo()
        startInfo.FileName = Apppath
        startInfo.Arguments = $"/C {Chr(34)}notepad {AppDomain.CurrentDomain.BaseDirectory}\{ToolsFolder}\shaderglass\keitai.sgp {Chr(34)}"
        startInfo.UseShellExecute = False
        startInfo.CreateNoWindow = True
        startInfo.RedirectStandardOutput = True
        startInfo.RedirectStandardError = True
        startInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
        Dim process As Process = Process.Start(startInfo)
    End Sub

End Class