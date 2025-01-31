Imports System.IO
Imports System.Net.Security
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading
Imports System.Timers
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
            ListViewGamesVariants.Clear()
            ListViewGames.Columns.Add("Title", GroupBox1.Width - 20, HorizontalAlignment.Left)

            ' Add icons to the ImageList
            LoadGameIcons()

            ' Assign the ImageList to the ListView
            ListViewGames.SmallImageList = ImageListGames

            ' Add games to the ListView
            Dim BustedGames As New List(Of String)
            For Each game In games
                If game.ZIPName = String.Empty Or game.ZIPName Is Nothing Then
                    BustedGames.Add(game.ENTitle)
                End If
                Dim item As New ListViewItem(game.ENTitle)
                item.ImageKey = game.ENTitle ' Use the game title as the key for the icon
                ListViewGames.Items.Add(item)
            Next
            If BustedGames.Count > 0 Then
                Dim message = $"Busted Games Needing Fixed: {vbCrLf}"
                For Each g In BustedGames
                    message += $"{g}{vbCrLf}"
                Next
                MessageBox.Show(message)
            End If
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
    Private Sub LoadGameIcons()
        Dim DojaIcon = $"{ToolsFolder}\icons\defaults\doja.gif"
        Dim StarIcon = $"{ToolsFolder}\icons\defaults\star.gif"
        If File.Exists(DojaIcon) = False Or File.Exists(StarIcon) = False Then
            MessageBox.Show("Missing Doja/Star Defualt icons.")
        End If
        ' Get all available icon files once
        Dim availableIcons As HashSet(Of String) = Directory.GetFiles($"{ToolsFolder}\icons").
        Select(Function(iconPath) Path.GetFileNameWithoutExtension(iconPath).ToLower()).
        ToHashSet()

        ' Loop through each game and assign the appropriate icon
        For Each game In games
            Dim iconFileName As String = Path.GetFileNameWithoutExtension(game.ZIPName).ToLower()
            Dim iconPath As String = $"{ToolsFolder}\icons\{iconFileName}.gif"

            If availableIcons.Contains(iconFileName) AndAlso File.Exists(iconPath) Then
                ' Add the game's custom icon
                ImageListGames.Images.Add(game.ENTitle, Image.FromFile(iconPath))
            Else
                ' Assign default icon based on emulator type
                If game.Emulator.ToLower() = "doja" Then
                    ImageListGames.Images.Add(game.ENTitle, Image.FromFile(DojaIcon))
                ElseIf game.Emulator.ToLower() = "star" Then
                    ImageListGames.Images.Add(game.ENTitle, Image.FromFile(StarIcon))
                End If
            End If
        Next
    End Sub
    Private Sub EnableButtons()
        ' Enable game launch button and checkbox
        btnLaunchGame.Enabled = True
        chkbxHidePhoneUI.Enabled = True
        cobxAudioType.Enabled = True
        chkbxShaderGlass.Enabled = True
    End Sub
    Private Sub FilterAndHighlightGames()
        ' Get the selected filter and search term
        Dim selectedFilter As String = cbxFilterType.SelectedItem.ToString().ToLower()
        Dim searchTerm As String = txtLVSearch.Text.Trim().ToLower()

        ' Clear the ListView
        ListViewGames.Items.Clear()

        ' Load favorites and installed games into hash sets for quick lookup
        Dim favoriteGames As HashSet(Of String) = New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        If File.Exists("configs\favorites.txt") Then
            favoriteGames = File.ReadAllLines("configs\favorites.txt").
                         Select(Function(fav) fav.Trim()).
                         ToHashSet(StringComparer.OrdinalIgnoreCase)
        End If

        Dim installedGames As HashSet(Of String) = Directory.GetDirectories(DownloadsFolder).
                                               Select(Function(folder) Path.GetFileName(folder)).
                                               ToHashSet(StringComparer.OrdinalIgnoreCase)

        ' Loop through games and apply filtering and highlighting
        For Each game In games
            ' Check if the game matches the current filter and search term
            Dim matchesSearch As Boolean = game.ENTitle.ToLower().Contains(searchTerm)
            Dim matchesFilter As Boolean = (selectedFilter = "all" OrElse
                                        (selectedFilter = "favorites" AndAlso favoriteGames.Contains(game.ENTitle)) OrElse
                                        (selectedFilter = "installed" AndAlso Not String.IsNullOrWhiteSpace(game.ZIPName) AndAlso installedGames.Contains(Path.GetFileNameWithoutExtension(game.ZIPName))) OrElse
                                        (game.Emulator.ToLower() = selectedFilter))

            If matchesSearch AndAlso matchesFilter Then
                ' Create the ListView item
                Dim item As New ListViewItem(game.ENTitle) With {
                .ImageKey = game.ENTitle ' Assign the correct icon
            }

                ' Determine highlighting
                Dim isFavorited As Boolean = favoriteGames.Contains(game.ENTitle)
                Dim isInstalled As Boolean = Not String.IsNullOrWhiteSpace(game.ZIPName) AndAlso installedGames.Contains(Path.GetFileNameWithoutExtension(game.ZIPName))

                If isInstalled AndAlso isFavorited Then
                    item.BackColor = Color.LightSeaGreen ' Both installed and favorited
                ElseIf isInstalled Then
                    item.BackColor = Color.LightGreen ' Only installed
                ElseIf isFavorited Then
                    item.BackColor = Color.LightGoldenrodYellow ' Only favorited
                Else
                    item.BackColor = Color.White ' Neither installed nor favorited
                End If

                ' Add item to the ListView
                ListViewGames.Items.Add(item)
            End If
        Next
        LoadGameVariants()
    End Sub
    Private Sub LoadGameVariants()
        ' Ensure the ListView view mode is set to Details
        ListViewGamesVariants.View = View.Details
        ListViewGamesVariants.Clear()  ' Clear existing items if needed
        ListViewGamesVariants.Columns.Clear()
        ListViewGamesVariants.Columns.Add("Game Variants", -2, HorizontalAlignment.Left)  ' Add a column

        ' Get the selected game title from the ListView
        Dim selectedGameTitle As String
        Dim selectedGame As Game
        Try
            selectedGameTitle = ListViewGames.SelectedItems(0).Text
            selectedGame = games.FirstOrDefault(Function(g) g.ENTitle = selectedGameTitle)
            If selectedGame Is Nothing Then
                Return
            End If
        Catch ex As Exception
            Return
        End Try


        Dim GameVariants = selectedGame.Variants
        If GameVariants Is Nothing Then
            Return
        End If

        ' Split and add variants as rows in the ListView
        Dim GameVariantsSplit = GameVariants.Split(",")
        For Each va In GameVariantsSplit
            Dim item As New ListViewItem(va.Trim())
            ListViewGamesVariants.Items.Add(item)
        Next
    End Sub
    Private Sub DownloadGames(ContextDownload As Boolean)
        ' Get the selected game title from the ListViewGames
        Dim selectedGameTitle As String
        Dim selectedGame As Game
        Try
            selectedGameTitle = ListViewGames.SelectedItems(0).Text
            selectedGame = games.FirstOrDefault(Function(g) g.ENTitle = selectedGameTitle)
        Catch ex As Exception
            MessageBox.Show("Please select a game")
        End Try

        ' Get the selected game title from the ListViewVariants
        Dim selectedGameVariants As String = String.Empty
        Try
            ' Determine if a variant is selected
            If ListViewGamesVariants.SelectedItems.Count > 0 Then
                selectedGameVariants = ListViewGamesVariants.SelectedItems(0).Text.Trim()
            End If
        Catch ex As Exception

        End Try

        'Check if its nothing
        If selectedGame Is Nothing Then
            MessageBox.Show("Selected game could not be found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' Construct paths for game files
        Dim gameBasePath As String = $"{DownloadsFolder}\{Path.GetFileNameWithoutExtension(selectedGame.ZIPName)}"
        Dim downloadFileZipPath As String = $"{DownloadsFolder}\{selectedGame.ZIPName}"
        Dim GameVariants = selectedGame.Variants
        Dim GameVariantsSplit = selectedGame.Variants.Split(",")
        If GameVariants Is Nothing Or GameVariants = "" Then
            CurrentSelectedGameJAM = $"{gameBasePath}\bin\{Path.GetFileNameWithoutExtension(selectedGame.ZIPName)}.jam"
            CurrentSelectedGameJAR = $"{gameBasePath}\bin\{Path.GetFileNameWithoutExtension(selectedGame.ZIPName)}.jar"
        ElseIf ListViewGamesVariants.SelectedItems.Count > 0 Then
            CurrentSelectedGameJAM = $"{gameBasePath}\{selectedGameVariants}\bin\{Path.GetFileNameWithoutExtension(selectedGame.ZIPName)}.jam"
            CurrentSelectedGameJAR = $"{gameBasePath}\{selectedGameVariants}\bin\{Path.GetFileNameWithoutExtension(selectedGame.ZIPName)}.jar"
        Else
            CurrentSelectedGameJAM = $"{gameBasePath}\{GameVariantsSplit(0)}\bin\{Path.GetFileNameWithoutExtension(selectedGame.ZIPName)}.jam"
            CurrentSelectedGameJAR = $"{gameBasePath}\{GameVariantsSplit(0)}\bin\{Path.GetFileNameWithoutExtension(selectedGame.ZIPName)}.jar"
        End If

        Logger.LogInfo($"Checking for {CurrentSelectedGameJAR}")
        ' Check if the game is already downloaded
        If File.Exists(CurrentSelectedGameJAR) Then
            If ContextDownload Then
                Dim result As DialogResult = MessageBox.Show(
            $"The game '{selectedGame.ENTitle} ({selectedGame.ZIPName})' is already downloaded. Would you like to download it again?{vbCrLf}{vbCrLf}" &
            "This could delete your save data, so please be careful.",
            "Download Game Again", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

                If result = DialogResult.Yes Then
                    StartGameDownload(selectedGame, downloadFileZipPath, gameBasePath, CurrentSelectedGameJAM, CurrentSelectedGameJAR)
                    MessageBox.Show($"Completed redownload of '{selectedGame.ENTitle}'", "Download Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End If

            ' Load JAM controls
            UtilManager.GenerateDynamicControlsFromLines(CurrentSelectedGameJAM, gbxGameInfo)
        Else
            ' Game not downloaded - prompt user to download it
            Dim result As DialogResult = MessageBox.Show(
    $"The game '{selectedGame.ENTitle} ({selectedGame.ZIPName})' is not downloaded. Would you like to download it?",
    "Download Game", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

            If result = DialogResult.Yes Then
                Logger.LogInfo($"Starting download for {selectedGame.DownloadURL}")
                StartGameDownload(selectedGame, downloadFileZipPath, gameBasePath, CurrentSelectedGameJAM, CurrentSelectedGameJAR)
            End If
        End If
    End Sub
    Private Sub StartGameDownload(selectedGame As Game, downloadFileZipPath As String, extractFolder As String, jamFilePath As String, jarFilePath As String)
        Try
            Dim gameDownloader As New GameDownloader(pbGameDL)
            gameDownloader.DownloadGameAsync(selectedGame.DownloadURL, downloadFileZipPath, extractFolder, selectedGame, jamFilePath, jarFilePath, False)
        Catch ex As Exception
            Logger.LogError($"Failed to download or extract game '{selectedGame.ENTitle}': {ex.Message}")
            MessageBox.Show($"An error occurred while downloading '{selectedGame.ENTitle}'. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
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
    Private Sub DeleteGames()
        If ListViewGames.SelectedItems.Count = 0 Then
            MessageBox.Show("Please select at least one game to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Gather all the selected games
        Dim gamesToDelete As New List(Of Game)
        For Each item As ListViewItem In ListViewGames.SelectedItems
            Dim selectedGameTitle As String = item.Text
            Dim selectedGame As Game = games.FirstOrDefault(Function(g) g.ENTitle = selectedGameTitle)
            If selectedGame IsNot Nothing Then
                gamesToDelete.Add(selectedGame)
            End If
        Next

        ' Display confirmation message with a list of games to delete
        Dim gameList As String = String.Join(Environment.NewLine, gamesToDelete.Select(Function(g) $"{g.ENTitle} ({g.ZIPName})"))
        Dim result As DialogResult = MessageBox.Show($"The following games will be deleted:{Environment.NewLine}{Environment.NewLine}{gameList}{Environment.NewLine}{Environment.NewLine}Do you want to proceed?", "Delete Games", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If result = DialogResult.Yes Then
            Dim deletedGames As New List(Of String)
            Dim failedGames As New List(Of String)

            For Each game In gamesToDelete
                Dim CurrentSelectedGameFolder = $"{DownloadsFolder}\{Path.GetFileNameWithoutExtension(game.ZIPName)}"

                ' Check if the folder exists and attempt to delete it
                If Directory.Exists(CurrentSelectedGameFolder) Then
                    Try
                        My.Computer.FileSystem.DeleteDirectory(CurrentSelectedGameFolder, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                        deletedGames.Add(game.ENTitle)
                    Catch ex As Exception
                        failedGames.Add($"{game.ENTitle} (Error: {ex.Message})")
                    End Try
                Else
                    failedGames.Add($"{game.ENTitle} (Not downloaded)")
                End If
            Next

            ' Display results
            If deletedGames.Count > 0 Then
                MessageBox.Show($"Successfully deleted:{Environment.NewLine}{String.Join(Environment.NewLine, deletedGames)}", "Deletion Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

            If failedGames.Count > 0 Then
                MessageBox.Show($"Could not delete the following games:{Environment.NewLine}{String.Join(Environment.NewLine, failedGames)}", "Deletion Errors", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
            FilterAndHighlightGames()
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
    Public Function VerifyGameDownloaded() As Boolean
        ' Get the selected game title from ListViewGames
        Dim selectedGameTitle As String = ListViewGames.SelectedItems(0).Text
        Dim selectedGame As Game = games.FirstOrDefault(Function(g) g.ENTitle = selectedGameTitle)
        If selectedGame Is Nothing Then
            Return False
        End If

        ' Determine if a variant is selected
        Dim selectedVariant As String = String.Empty
        If ListViewGamesVariants.SelectedItems.Count > 0 Then
            selectedVariant = ListViewGamesVariants.SelectedItems(0).Text.Trim()
        Else
            ' Check if game needs has variant
            If selectedGame.Variants = String.Empty = False Then
                MessageBox.Show("Please select a game variant.")
                Return False
            End If
        End If

        ' Loop through directories in DownloadsFolder and check if the game (with or without variant) is downloaded
        For Each f In Directory.GetDirectories(DownloadsFolder)
            Dim expectedFolderName As String
            expectedFolderName = Path.GetFileNameWithoutExtension(selectedGame.ZIPName)
            ' Check if the directory matches the expected name
            If Path.GetFileName(f) = expectedFolderName Then
                If String.IsNullOrEmpty(selectedVariant) Then
                    Return True
                Else
                    For Each Fol In Directory.GetDirectories(f)
                        expectedFolderName = selectedVariant
                        If Path.GetFileName(Fol) = expectedFolderName Then
                            Return True
                        End If
                    Next
                End If
            End If
        Next
        ' Return false if no match is found
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
        ' Restart the timer on selection change
        selectionTimer.Stop()
        selectionTimer.Start()
    End Sub
    Private Sub SelectionTimer_Tick(sender As Object, e As EventArgs) Handles selectionTimer.tick
        selectionTimer.Stop() ' Stop the timer to avoid repeated triggering

        ' Check if any items are selected
        If ListViewGames.SelectedItems.Count = 0 Then Return

        ' Perform actions once after all selections are done
        LoadGameVariants()
        DownloadGames(False)
        EnableButtons()
    End Sub
    Private Sub lbxMachiCharaList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lbxMachiCharaList.SelectedIndexChanged
        If lbxMachiCharaList.SelectedIndex = -1 Then Return
        DownloadMachiChara()
    End Sub
    Private Sub ListViewGamesVariants_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListViewGamesVariants.SelectedIndexChanged
        DownloadGames(False)
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
        FilterAndHighlightGames()
    End Sub


    'Textbox Changes
    Private Sub txtLVSearch_TextChanged(sender As Object, e As EventArgs) Handles txtLVSearch.TextChanged
        FilterAndHighlightGames()
    End Sub

    'ContextMenuStrip Changes
    Private Sub cmsGameLV_Download_Click(sender As Object, e As EventArgs) Handles cmsGameLV_Download.Click
        DownloadGames(True)
    End Sub
    Private Sub cmsGameLV_Delete_Click(sender As Object, e As EventArgs) Handles cmsGameLV_Delete.Click
        DeleteGames()
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
                If ListViewGames.SelectedItems.Count = 0 Then
                    MessageBox.Show("Please select a game, before launching")
                    Return
                End If
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
            Logger.LogError($"Error Launching Game:{vbCrLf}{ex}")
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
                Await GameDownloader.DownloadGameAsync(GURL.DownloadURL, DownloadFileZipPath, $"{DownloadsFolder}\{Path.GetFileNameWithoutExtension(GURL.ZIPName)}", GURL, CurrentSelectedGameJAM, CurrentSelectedGameJAR, True)

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