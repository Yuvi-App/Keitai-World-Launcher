﻿Imports System.IO
Imports System.Reflection.Metadata.Ecma335
Imports System.Text
Imports KeitaiWorldLauncher.My.logger
Imports KeitaiWorldLauncher.My.Managers
Imports KeitaiWorldLauncher.My.Models
Imports ReaLTaiizor.[Enum].Poison
Imports SharpDX.DirectInput

Public Class Form1
    'Global Vars
    Private splash As SplashScreen
    Dim isDebug As Boolean = False
    Dim isOnline As Boolean = False
    Dim configManager As New ConfigManager()
    Dim utilManager As New UtilManager
    Dim gameListManager As New GameListManager()
    Dim gameManager As New GameManager()
    Dim machicharaListManager As New MachiCharaListManager()
    Dim zipManager As New ZipManager()
    Dim config As Dictionary(Of String, String)
    Dim games As List(Of Game)
    Dim machicharas As List(Of MachiChara)

    'Directory Var
    Public DownloadsFolder As String = "data\downloads"
    Public ToolsFolder As String = "data\tools"
    Public ConfigsFolder As String = "configs"
    Public LogFolder As String = "logs"
    Public FavoritesTxtFile As String = Path.Combine(ConfigsFolder, "favorites.txt")
    Public CustomGamesTxtFile As String = Path.Combine(ConfigsFolder, "customgames.txt")

    'Index Vars Can Change
    Dim CurrentSelectedGameJAM As String
    Dim CurrentSelectedGameJAR As String
    Dim CurrentSelectedMachiCharaCFD As String

    'Config Vars
    Public versionCheckUrl As String
    Public autoUpdate As Boolean
    Public FirstRun As Boolean
    Public gameListUrl As String
    Public autoUpdateGameList As Boolean
    Public machicharaListUrl As String
    Public autoUpdatemachicharaList As Boolean
    Public UseShaderGlass As Boolean
    Public NetworkUID As String
    Public Dojapath As String
    Public DojaAppPath As String
    Public DojaEXE As String
    Public DojaHideUI As Boolean
    Public DOJASoundType As String
    Public Starpath As String
    Public StarAppPath As String
    Public StarEXE As String
    Public MachiCharapath As String
    Public MachiCharaExe As String

    ' FORM LOAD
    Private Sub Form1_Closing(sender As Object, e As EventArgs) Handles MyBase.Closing
        UtilManager.CheckAndCloseDoja()
        UtilManager.CheckAndCloseStar()
    End Sub
    Private Async Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Opacity = 0
        Application.EnableVisualStyles()

        'Summon Splash
        SplashScreen.ShowSplash()

        ' Adjust Form
        AdjustFormPadding()

        ' Check if Debug
#If DEBUG Then
        isDebug = True
        XMLUpdateToolStripMenuItem.Visible = True
        XMLCreationToolStripMenuItem.Visible = True
        BatchDownloadToolStripMenuItem.Visible = True
        XMLUpdateToolStripMenuItem.Enabled = True
        XMLCreationToolStripMenuItem.Enabled = True
        BatchDownloadToolStripMenuItem.Enabled = True
#End If

        ' Setup DIRs
        Await UtilManager.SetupDIRSAsync()

        ' Setup SJIS 
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)

        ' Setup Logging
        ' Check log File Size and Cleanup
        Await UtilManager.DeleteLogIfTooLargeAsync(Path.Join(LogFolder, "app_log.txt"))
        Logger.InitializeLogger()
        Logger.LogInfo("Application started.")

        ' Load Config
        config = Await configManager.LoadConfigAsync()

        ' Access Config settings
        versionCheckUrl = config("VersionCheckURL")
        autoUpdate = Boolean.Parse(config("AutoUpdate"))
        FirstRun = Boolean.Parse(config("FirstRun"))
        gameListUrl = config("GamelistURL")
        autoUpdateGameList = Boolean.Parse(config("AutoUpdateGameList"))
        machicharaListUrl = config("MachiCharalistURL")
        autoUpdatemachicharaList = Boolean.Parse(config("AutoUpdateMachiCharaList"))
        UseShaderGlass = Boolean.Parse(config("UseShaderGlass"))
        NetworkUID = config("NetworkUID")
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

        ' Check PreREQs if First Run
        Await UtilManager.CheckForSpacesInPathAsync()
        If FirstRun = True Then
            Logger.LogInfo("Detected First Run - Checking for Admin")
            Logger.LogInfo("Starting PreReq Check")
            Dim AllGood As Boolean = Await UtilManager.CheckforPreReqAsync()
            If AllGood = True Then
                Logger.LogInfo("PreReq All Good")
                Await configManager.UpdateFirstRunSettingAsync("false")
            End If
        ElseIf FirstRun = False Then
            Logger.LogInfo("Starting app Normally")
        Else
            Logger.LogInfo("Invalid FirstStart Parameter in Config")
            MessageBox.Show("Invalid FirstStart paramete in AppConfig.xml, Please set to true and relaunch app.")
            Form1.QuitApplication()
        End If

        'Needs Internet If none we skip and use local file
        Dim uri As New Uri(versionCheckUrl)
        Dim domainOnly As String = uri.Scheme & "://" & uri.Host
        Logger.LogInfo("Checking internet connectivity...")
        If Await UtilManager.IsInternetAvailableAsync("http://neverssl.com") Then
            'Set the User to online
            isOnline = True

            ' Check for App update
            Logger.LogInfo("Getting App Update")
            If autoUpdate = True Then
                Await UtilManager.CheckForUpdatesAsync(versionCheckUrl)
            End If

            ' Get Updated Game List  
            Logger.LogInfo("Getting Gamelist.xml")
            If autoUpdateGameList = True Then
                Await GameListManager.DownloadGameListAsync(gameListUrl)
            End If

            ' Get Updated MachiChara List  
            Logger.LogInfo("Getting Machichara.xml")
            If autoUpdatemachicharaList = True Then
                Await MachiCharaListManager.DownloadMachiCharaListAsync(machicharaListUrl)
            End If

            ' Send Launch Stats
            UtilManager.SendKWLLaunchStats()
        Else
            isOnline = False
            Me.Text = "Keitai World Launcher - Offline"
            Logger.LogWarning($"No internet connection to Domain {domainOnly}. Skipping online checks.")
        End If

        ' Load Custom Games
        Await LoadCustomGamesAsync()

        ' Load Game List
        Await LoadGameListFirstTimeAsync()

        ' Load MachiChara List
        Await LoadMachiCharaListFirsTimeASync()

        'Last Step
        Await GetSDKsAsync()

        ' Setup any Config Suff
        chkbxHidePhoneUI.Checked = DojaHideUI
        Dim atindex As Integer = cobxAudioType.FindStringExact(DOJASoundType)
        cobxAudioType.SelectedIndex = atindex
        chkbxShaderGlass.Checked = UseShaderGlass
        cbxFilterType.SelectedIndex = 0
        cbxShaderGlassScaling.SelectedIndex = 2

        ' Close the splash screen
        SplashScreen.CloseSplash()
        Await Task.Delay(300)
        Me.Opacity = 1
    End Sub

    ' General Other Function
    Private Async Function GetSDKsAsync() As Task
        Dim dojaDefault As String = "iDKDoJa5.1"
        Dim starDefault As String = "iDKStar2.0"
        Dim dojaFound As Boolean = False
        Dim starFound As Boolean = False

        ' Run the directory check on a background thread
        Dim sdkFolders As String() = Await Task.Run(Function() Directory.GetDirectories(ToolsFolder))

        ' Clear existing items on the UI thread
        cbxDojaSDK.Items.Clear()
        cbxStarSDK.Items.Clear()

        For Each SSDK In sdkFolders
            Dim folder As String = Path.GetFileName(SSDK)

            If folder.StartsWith("idkstar", StringComparison.OrdinalIgnoreCase) Then
                cbxStarSDK.Items.Add(folder)
                If folder.Equals(starDefault, StringComparison.OrdinalIgnoreCase) Then
                    starFound = True
                End If
            ElseIf folder.StartsWith("idkdoja", StringComparison.OrdinalIgnoreCase) Then
                cbxDojaSDK.Items.Add(folder)
                If folder.Equals(dojaDefault, StringComparison.OrdinalIgnoreCase) Then
                    dojaFound = True
                End If
            End If
        Next

        ' Set defaults or show warnings
        If starFound Then
            cbxStarSDK.SelectedItem = starDefault
        Else
            MessageBox.Show(owner:=SplashScreen, $"The default SDK '{starDefault}' was not found. Please download and set it up.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If

        If dojaFound Then
            cbxDojaSDK.SelectedItem = dojaDefault
        Else
            MessageBox.Show(owner:=SplashScreen, $"The default SDK '{dojaDefault}' was not found. Please download and set it up.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Function
    Private Async Function LoadGameIconsAsync() As Task
        Dim DojaIconPath As String = Path.Combine(ToolsFolder, "icons", "defaults", "doja.gif")
        Dim StarIconPath As String = Path.Combine(ToolsFolder, "icons", "defaults", "star.gif")

        ' Validate default icons
        If Not File.Exists(DojaIconPath) OrElse Not File.Exists(StarIconPath) Then
            MessageBox.Show("Missing Doja/Star Default icons.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Function
        End If

        ' Load all icons on a background thread
        Dim imageDict As Dictionary(Of String, Image) = Await Task.Run(Function()
                                                                           Dim result As New Dictionary(Of String, Image)(StringComparer.OrdinalIgnoreCase)

                                                                           Dim availableIcons As HashSet(Of String) = Directory.GetFiles(Path.Combine(ToolsFolder, "icons")).
            Select(Function(iconPath) Path.GetFileNameWithoutExtension(iconPath).ToLower()).
            ToHashSet()

                                                                           For Each game In games
                                                                               Dim iconFileName As String = Path.GetFileNameWithoutExtension(game.ZIPName).ToLower()
                                                                               Dim customIconPath As String = Path.Combine(ToolsFolder, "icons", $"{iconFileName}.gif")

                                                                               Try
                                                                                   Dim iconToUse As Image

                                                                                   If availableIcons.Contains(iconFileName) AndAlso File.Exists(customIconPath) Then
                                                                                       Using fs As New FileStream(customIconPath, FileMode.Open, FileAccess.Read)
                                                                                           iconToUse = Image.FromStream(fs)
                                                                                       End Using
                                                                                   Else
                                                                                       Dim defaultIconPath As String = If(game.Emulator.Equals("doja", StringComparison.OrdinalIgnoreCase), DojaIconPath, StarIconPath)
                                                                                       Using fs As New FileStream(defaultIconPath, FileMode.Open, FileAccess.Read)
                                                                                           iconToUse = Image.FromStream(fs)
                                                                                       End Using
                                                                                   End If

                                                                                   result(game.ENTitle) = iconToUse

                                                                               Catch ex As Exception
                                                                                   Logger.LogError($"Failed to load icon for game: {game.ENTitle}", ex)
                                                                               End Try
                                                                           Next

                                                                           Return result
                                                                       End Function)

        ' Safely add the images to the ImageList on the UI thread
        For Each kvp In imageDict
            ImageListGames.Images.Add(kvp.Key, kvp.Value)
        Next
    End Function
    Private Async Function LoadGameListFirstTimeAsync() As Task
        Logger.LogInfo("Processing gamelist.xml")

        Try
            ' Ensure the favorites file exists
            If Not File.Exists(FavoritesTxtFile) Then
                Using stream = File.Create(FavoritesTxtFile)
                    ' Immediately close after creation
                End Using
            End If

            ' Load games on a background thread
            games = Await Task.Run(Function() gameListManager.LoadGamesAsync())

            lblTotalGameCount.Text = $"Total: {games.Count}"

            ' Clear ListView and ImageList
            ListViewGames.BeginUpdate()
            ImageListGames.Images.Clear()
            ListViewGames.Items.Clear()
            ListViewGamesVariants.Clear()

            ' Adjust column width dynamically
            ListViewGames.Columns.Clear()
            ListViewGames.Columns.Add("Title", ListViewGames.ClientSize.Width - SystemInformation.VerticalScrollBarWidth, HorizontalAlignment.Left)

            ' Load Game Icons
            Await LoadGameIconsAsync()
            ListViewGames.SmallImageList = ImageListGames

            ' Track games with missing ZIP names
            Dim BustedGames As New List(Of String)

            ' Add games to ListView
            For Each game In games
                If String.IsNullOrWhiteSpace(game.ZIPName) Then
                    BustedGames.Add(game.ENTitle)
                End If

                Dim item As New ListViewItem(game.ENTitle) With {
                .ImageKey = game.ENTitle
            }
                ListViewGames.Items.Add(item)
            Next

            ' Notify about busted games if any
            If BustedGames.Count > 0 Then
                Dim message As New System.Text.StringBuilder("Busted Games Needing Fixed:" & vbCrLf)
                For Each g In BustedGames
                    message.AppendLine(g)
                Next
                MessageBox.Show(message.ToString(), "Missing Game Data", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            ListViewGames.EndUpdate()

        Catch ex As Exception
            MessageBox.Show(owner:=SplashScreen, $"Failed to Load Game List:{vbCrLf}{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Logger.LogError("Failed to Load Game List", ex)
        End Try
    End Function
    Private Async Function LoadMachiCharaListFirsTimeASync() As Task(Of Boolean)
        Logger.LogInfo("Processing machicharalist.xml")
        ListViewMachiChara.View = View.Details
        ListViewMachiChara.FullRowSelect = True
        ListViewMachiChara.Columns.Clear()
        ListViewMachiChara.Columns.Add("Title", ListViewMachiChara.ClientSize.Width - SystemInformation.VerticalScrollBarWidth, HorizontalAlignment.Left)
        Try
            machicharas = machicharaListManager.LoadMachiChara()
            If machicharas IsNot Nothing Then
                ListViewMachiChara.Items.Clear()
                For Each mc In machicharas
                    Dim item As New ListViewItem(mc.ENTitle)
                    item.Tag = mc ' Store the full object for later access
                    ListViewMachiChara.Items.Add(item)

                    'Check if Downloaded already
                    Dim CFDPath = Path.Combine(DownloadsFolder, mc.CFDName)
                    If File.Exists(CFDPath) Then
                        item.BackColor = Color.LightGreen
                    End If
                Next
                lblMachiCharaTotalCount.Text = $"Total: {ListViewMachiChara.Items.Count}"
            End If
        Catch ex As Exception
            Logger.LogError("Failed to Load MachiChara List", ex)
        End Try
    End Function
    Private Sub EnableButtons()
        ' Enable game launch button and checkbox
        btnLaunchGame.Enabled = True
        chkbxHidePhoneUI.Enabled = True
        cobxAudioType.Enabled = True
        chkbxShaderGlass.Enabled = True
        cbxShaderGlassScaling.Enabled = True
        cbxDojaSDK.Enabled = True
        cbxStarSDK.Enabled = True
        chkbxLocalEmulator.Enabled = True
        chkbxEnableController.Enabled = True
    End Sub
    Private Async Function FilterAndHighlightGamesAsync() As Task
        ' Get filter and search term from UI (must be done on UI thread)
        Dim selectedFilter As String = cbxFilterType.SelectedItem?.ToString().ToLower()
        Dim searchTerm As String = txtLVSearch.Text.Trim()

        ' Run filtering logic in background
        Dim filteredItems As List(Of ListViewItem) = Await Task.Run(Function()
                                                                        Dim result As New List(Of ListViewItem)

                                                                        ' Load favorites and custom games
                                                                        Dim favoriteGames As HashSet(Of String) =
            If(File.Exists(FavoritesTxtFile),
               File.ReadAllLines(FavoritesTxtFile).Select(Function(f) f.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase),
               New HashSet(Of String)(StringComparer.OrdinalIgnoreCase))

                                                                        Dim customGames As HashSet(Of String) =
            If(File.Exists(CustomGamesTxtFile),
               File.ReadAllLines(CustomGamesTxtFile).Select(Function(f) f.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase),
               New HashSet(Of String)(StringComparer.OrdinalIgnoreCase))

                                                                        ' Load installed game folders
                                                                        Dim installedGames As HashSet(Of String) =
            Directory.GetDirectories(DownloadsFolder).
            Select(Function(folder) Path.GetFileName(folder)).
            ToHashSet(StringComparer.OrdinalIgnoreCase)

                                                                        For Each game In games
                                                                            Dim gameTitle As String = game.ENTitle
                                                                            Dim emulatorType As String = game.Emulator.ToLower()
                                                                            Dim zipFileName As String = Path.GetFileNameWithoutExtension(game.ZIPName)

                                                                            Dim matchesSearch As Boolean = gameTitle.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0
                                                                            Dim isFavorited As Boolean = favoriteGames.Contains(gameTitle)
                                                                            Dim isCustom As Boolean = customGames.Contains(gameTitle)
                                                                            Dim isInstalled As Boolean = Not String.IsNullOrWhiteSpace(game.ZIPName) AndAlso installedGames.Contains(zipFileName)

                                                                            Dim matchesFilter As Boolean = selectedFilter = "all" OrElse
                                           (selectedFilter = "favorites" AndAlso isFavorited) OrElse
                                           (selectedFilter = "custom" AndAlso isCustom) OrElse
                                           (selectedFilter = "installed" AndAlso isInstalled) OrElse
                                           (emulatorType = selectedFilter)

                                                                            If matchesSearch AndAlso matchesFilter Then
                                                                                Dim item As New ListViewItem(gameTitle) With {
                    .ImageKey = gameTitle
                }

                                                                                If isInstalled AndAlso isFavorited Then
                                                                                    item.BackColor = Color.LightSeaGreen
                                                                                ElseIf isCustom Then
                                                                                    item.BackColor = Color.LightSteelBlue
                                                                                ElseIf isInstalled Then
                                                                                    item.BackColor = Color.LightGreen
                                                                                ElseIf isFavorited Then
                                                                                    item.BackColor = Color.LightGoldenrodYellow
                                                                                Else
                                                                                    item.BackColor = Color.White
                                                                                End If

                                                                                result.Add(item)
                                                                            End If
                                                                        Next

                                                                        Return result
                                                                    End Function)

        ' Update UI
        ListViewGames.BeginUpdate()
        ListViewGames.Items.Clear()
        ListViewGames.BackColor = SystemColors.Window

        ListViewGames.Items.AddRange(filteredItems.ToArray())
        lblFilteredGameCount.Text = $"Filtered: {filteredItems.Count}"
        ListViewGames.EndUpdate()

        ' Optionally load variants after a selection
        'Await LoadGameVariantsAsync()
    End Function
    Private Async Function LoadGameVariantsAsync() As Task
        ' Ensure the ListView view mode is set to Details
        ListViewGamesVariants.View = View.Details
        ListViewGamesVariants.Items.Clear()
        ListViewGamesVariants.Columns.Clear()
        ListViewGamesVariants.Columns.Add("Game Variants", ListViewGamesVariants.ClientSize.Width - SystemInformation.VerticalScrollBarWidth, HorizontalAlignment.Left)
        ListViewGamesVariants.BackColor = SystemColors.Window

        ' Ensure a game is selected
        If ListViewGames.SelectedItems.Count = 0 Then Return

        ' Get selected game title
        Dim selectedGameTitle As String = ListViewGames.SelectedItems(0).Text

        ' Find the game on a background thread
        Dim selectedGame As Game = Await Task.Run(Function()
                                                      Return games.FirstOrDefault(Function(g) g.ENTitle = selectedGameTitle)
                                                  End Function)

        If selectedGame Is Nothing OrElse String.IsNullOrWhiteSpace(selectedGame.Variants) Then Return

        ' Split the variants string safely
        Dim variants As String() = selectedGame.Variants.Split(","c)

        ' Add to ListView on the UI thread
        For Each v As String In variants
            ListViewGamesVariants.Items.Add(New ListViewItem(v.Trim()))
        Next
        lblTotalVariantCount.Text = $"Variants: {ListViewGamesVariants.Items.Count.ToString}"
    End Function
    Private Async Function DownloadGames(ContextDownload As Boolean) As Task

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

        ' Check if the game is already downloaded
        If isOnline = False Then
            Exit Function
        End If
        Logger.LogInfo($"Checking for {CurrentSelectedGameJAR}")
        If File.Exists(CurrentSelectedGameJAR) Then
            If ContextDownload Then
                Dim result As DialogResult = MessageBox.Show(
                    $"The game '{selectedGame.ENTitle} ({selectedGame.ZIPName})' is already downloaded. Would you like to download it again?{vbCrLf}{vbCrLf}" &
                    "This could delete your save data, so please be careful.",
                    "Download Game Again", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

                If result = DialogResult.Yes Then
                    Await StartGameDownload(selectedGame, downloadFileZipPath, gameBasePath, CurrentSelectedGameJAM, CurrentSelectedGameJAR)
                    Logger.LogInfo($"Starting redownload for {selectedGame.DownloadURL}")
                    MessageBox.Show($"Completed redownload of '{selectedGame.ENTitle}'", "Download Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
                If File.Exists(CurrentSelectedGameJAM) Then
                    UtilManager.GenerateDynamicControlsFromLines(CurrentSelectedGameJAM, panelDynamic)
                    ListViewGames.SelectedItems(0).BackColor = Color.LightGreen
                Else
                    Logger.LogError($"Download completed but JAM file not found at: {CurrentSelectedGameJAM}")
                End If
            End If
            UtilManager.GenerateDynamicControlsFromLines(CurrentSelectedGameJAM, panelDynamic)
        Else
            If selectedGame.ZIPName = String.Empty Or selectedGame.ZIPName Is Nothing Then
                Logger.LogError($"{selectedGame.ENTitle} has invalid gamelist values, unable to download.")
                MessageBox.Show($"{selectedGame.ENTitle} has invalid gamelist values, unable to download.")
                Return
            End If
            ' Game not downloaded - prompt user to download it
            Dim result As DialogResult = MessageBox.Show(
                $"The game '{selectedGame.ENTitle} ({selectedGame.ZIPName})' is not downloaded. Would you like to download it?",
                "Download Game", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

            If result = DialogResult.Yes Then
                Logger.LogInfo($"Starting download for {selectedGame.DownloadURL}")
                Await StartGameDownload(selectedGame, downloadFileZipPath, gameBasePath, CurrentSelectedGameJAM, CurrentSelectedGameJAR)
                If File.Exists(CurrentSelectedGameJAM) Then
                    UtilManager.GenerateDynamicControlsFromLines(CurrentSelectedGameJAM, panelDynamic)
                    ListViewGames.SelectedItems(0).BackColor = Color.LightGreen
                Else
                    Logger.LogError($"Download completed but JAM file not found at: {CurrentSelectedGameJAM}")
                End If
            End If
        End If
    End Function
    Private Async Function StartGameDownload(
        selectedGame As Game,
        downloadFileZipPath As String,
        extractFolder As String,
        jamFilePath As String,
        jarFilePath As String
    ) As Task
        Try
            Dim gameDownloader As New GameDownloader(pbGameDL)
            Await gameDownloader.DownloadGameAsync(
            selectedGame.DownloadURL,
            downloadFileZipPath,
            extractFolder,
            selectedGame,
            jamFilePath,
            jarFilePath,
            False
        )
        Catch ex As Exception
            Logger.LogError($"Failed to download or extract game '{selectedGame.ENTitle}': {ex.Message}")
            MessageBox.Show($"An error occurred while downloading '{selectedGame.ENTitle}'. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Function
    Private Async Sub DownloadMachiChara(selectedMachiChara As MachiChara)
        If selectedMachiChara IsNot Nothing Then
            Logger.LogInfo($"Checking for {DownloadsFolder}\{selectedMachiChara.CFDName}")
            CurrentSelectedMachiCharaCFD = Path.Combine(DownloadsFolder, selectedMachiChara.CFDName)

            ' Check if the MachiChara is already downloaded
            Dim localFilePath As String = CurrentSelectedMachiCharaCFD
            Dim downloadFilePath As String = Path.Combine(DownloadsFolder, selectedMachiChara.CFDName)

            If File.Exists(localFilePath) Then
                ' File already exists, nothing to do (or maybe inform the user)
            Else
                Dim result = MessageBox.Show($"The Machi Chara '{selectedMachiChara.ENTitle} ({selectedMachiChara.CFDName})' is not downloaded. Would you like to download it?", "Download Machi Chara", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                If result = DialogResult.Yes Then
                    Logger.LogInfo($"Starting Download for {selectedMachiChara.DownloadURL}")
                    Dim MachiCharaDownloader As New MachiCharaDownloader(pbGameDL)
                    Await MachiCharaDownloader.DownloadMachiCharaAsync(selectedMachiChara.DownloadURL, downloadFilePath, False)
                    HighlightMachiChara()
                End If
            End If
            btnMachiCharaLaunch.Enabled = True
        End If
    End Sub
    Private Async Function DeleteGamesAsync() As Task
        If ListViewGames.SelectedItems.Count = 0 Then
            MessageBox.Show("Please select at least one game to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Gather all selected games
        Dim gamesToDelete As New List(Of Game)
        For Each item As ListViewItem In ListViewGames.SelectedItems
            Dim selectedGameTitle As String = item.Text
            Dim selectedGame As Game = games.FirstOrDefault(Function(g) g.ENTitle = selectedGameTitle)
            If selectedGame IsNot Nothing Then
                gamesToDelete.Add(selectedGame)
            End If
        Next

        ' Display confirmation
        Dim gameList As String = String.Join(Environment.NewLine, gamesToDelete.Select(Function(g) $"{g.ENTitle} ({g.ZIPName})"))
        Dim result As DialogResult = MessageBox.Show($"The following games will be deleted:{Environment.NewLine}{Environment.NewLine}{gameList}{Environment.NewLine}{Environment.NewLine}Do you want to proceed?", "Delete Games", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If result = DialogResult.Yes Then
            Dim deletedGames As New List(Of String)
            Dim failedGames As New List(Of String)

            ' Run deletion work in background
            Await Task.Run(Sub()
                               For Each game In gamesToDelete
                                   Try
                                       Dim gameFolder As String = Path.Combine(DownloadsFolder, Path.GetFileNameWithoutExtension(game.ZIPName))

                                       If Directory.Exists(gameFolder) Then
                                           My.Computer.FileSystem.DeleteDirectory(gameFolder, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                                           deletedGames.Add(game.ENTitle)

                                           ' Check if SD Card game
                                           If Not String.IsNullOrEmpty(game.SDCardDataURL) Then
                                               For Each emu In cbxDojaSDK.Items
                                                   Dim CheckSDPath = Path.Combine(ToolsFolder, Path.GetFileName(emu), "lib", "storagedevice", "ext0", "SD_BIND", "SVC0000" & Path.GetFileNameWithoutExtension(game.ZIPName) & ".jam")
                                                   If Directory.Exists(CheckSDPath) Then
                                                       My.Computer.FileSystem.DeleteDirectory(CheckSDPath, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                                                   End If
                                               Next
                                           End If

                                           ' Update customgames.txt
                                           Dim configPath As String = Path.Combine(ConfigsFolder, "customgames.txt")
                                           If File.Exists(configPath) Then
                                               Dim lines As List(Of String) = File.ReadAllLines(configPath).ToList()
                                               If lines.Remove(Path.GetFileNameWithoutExtension(game.ZIPName)) Then
                                                   File.WriteAllLines(configPath, lines)
                                               End If
                                           End If
                                       Else
                                           failedGames.Add($"{game.ENTitle} (Not downloaded)")
                                       End If
                                   Catch ex As Exception
                                       failedGames.Add($"{game.ENTitle} (Error: {ex.Message})")
                                   End Try
                               Next
                           End Sub)

            ' Show results (on UI thread)
            If deletedGames.Count > 0 Then
                MessageBox.Show($"Successfully deleted:{Environment.NewLine}{String.Join(Environment.NewLine, deletedGames)}", "Deletion Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

            If failedGames.Count > 0 Then
                MessageBox.Show($"Could not delete the following games:{Environment.NewLine}{String.Join(Environment.NewLine, failedGames)}", "Deletion Errors", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            Await FilterAndHighlightGamesAsync()
        End If
    End Function
    Public Async Function DeleteMachiCharaAsync() As Task
        If ListViewMachiChara.SelectedItems.Count = 0 Then
            MessageBox.Show("Please select at least one MachiChara to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' ❗ Safely copy selected items to a list on the UI thread
        Dim selectedCharaList As New List(Of MachiChara)
        For Each item As ListViewItem In ListViewMachiChara.SelectedItems
            Dim mc As MachiChara = CType(item.Tag, MachiChara)
            If mc IsNot Nothing Then selectedCharaList.Add(mc)
        Next

        Dim deletedFiles As New List(Of String)

        ' Run deletion logic in background
        Await Task.Run(Sub()
                           For Each selectedMachiChara In selectedCharaList
                               Dim cfdPath As String = Path.Combine(DownloadsFolder, selectedMachiChara.CFDName)
                               If File.Exists(cfdPath) Then
                                   Try
                                       File.Delete(cfdPath)
                                       Logger.LogInfo($"Deleted MachiChara: {selectedMachiChara.CFDName}")
                                       SyncLock deletedFiles
                                           deletedFiles.Add(selectedMachiChara.CFDName)
                                       End SyncLock
                                   Catch ex As Exception
                                       Logger.LogError($"Failed to delete MachiChara: {selectedMachiChara.CFDName}", ex)
                                   End Try
                               End If
                           Next
                       End Sub)

        ' UI-safe call
        HighlightMachiChara()

        If deletedFiles.Count > 0 Then
            MessageBox.Show("Deleted the following MachiCharas:" & vbCrLf & String.Join(vbCrLf, deletedFiles),
                        "MachiChara Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Function
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
    Public Async Function VerifyGameDownloadedAsync() As Task(Of Boolean)
        ' Check if any item is selected in ListViewGames
        If ListViewGames.SelectedItems.Count = 0 Then
            MessageBox.Show("Please select a game.")
            Return False
        End If

        ' Get selected game
        Dim selectedGameTitle As String = ListViewGames.SelectedItems(0).Text
        Dim selectedGame As Game = games.FirstOrDefault(Function(g) g.ENTitle = selectedGameTitle)
        If selectedGame Is Nothing Then
            Return False
        End If

        ' Check variant selection
        Dim selectedVariant As String = String.Empty
        If ListViewGamesVariants.SelectedItems.Count > 0 Then
            selectedVariant = ListViewGamesVariants.SelectedItems(0).Text.Trim()
        ElseIf Not String.IsNullOrEmpty(selectedGame.Variants) Then
            MessageBox.Show("Please select a game variant.")
            Return False
        End If

        ' Run directory check on background thread
        Return Await Task.Run(Function()
                                  Dim expectedFolderName As String = Path.GetFileNameWithoutExtension(selectedGame.ZIPName)

                                  For Each f In Directory.GetDirectories(DownloadsFolder)
                                      If Path.GetFileName(f).Equals(expectedFolderName, StringComparison.OrdinalIgnoreCase) Then
                                          If String.IsNullOrEmpty(selectedVariant) Then
                                              Return True
                                          Else
                                              For Each variantDir In Directory.GetDirectories(f)
                                                  If Path.GetFileName(variantDir).Equals(selectedVariant, StringComparison.OrdinalIgnoreCase) Then
                                                      Return True
                                                  End If
                                              Next
                                          End If
                                      End If
                                  Next

                                  Return False
                              End Function)
    End Function
    Public Function VerifyEmulatorType(GameJAM As String)
        ' Extract emulator details from the .jam file
        If Not String.IsNullOrEmpty(GameJAM) Then
            Dim jamLines As String() = File.ReadAllLines(GameJAM, Encoding.GetEncoding("shift-jis"))
            Dim appTypeLine As String = jamLines.FirstOrDefault(Function(line) line.StartsWith("AppType = "))
            If Not String.IsNullOrEmpty(appTypeLine) Then
                Return "star"
            Else
                Return "doja"
            End If
        End If
    End Function
    Public Async Function LoadCustomGamesAsync() As Task
        Dim customGamesFile As String = Path.Combine(ConfigsFolder, "customgames.txt")

        ' Ensure the custom games file exists
        If Not File.Exists(customGamesFile) Then
            Using stream = File.Create(customGamesFile)
                ' Close immediately after creation
            End Using
        End If

        ' Deduplicate CustomGames.txt
        Dim allLines As String() = Await File.ReadAllLinesAsync(customGamesFile)
        Dim customGameLines As List(Of String) = allLines.
        Select(Function(line) line.Trim()).
        Where(Function(line) Not String.IsNullOrWhiteSpace(line)).
        Distinct(StringComparer.OrdinalIgnoreCase).
        ToList()

        ' Write back only if duplicates were removed
        If customGameLines.Count <> allLines.Length Then
            Await File.WriteAllLinesAsync(customGamesFile, customGameLines)
        End If

        ' Process each folder name
        For Each gameFolderName In customGameLines
            Dim gameFolderPath As String = Path.Combine(DownloadsFolder, gameFolderName)

            If Directory.Exists(gameFolderPath) Then
                Dim game As New Game With {
                .ENTitle = gameFolderName,
                .ZIPName = gameFolderName & ".zip",
                .DownloadURL = "",
                .CustomAppIconURL = "",
                .SDCardDataURL = "",
                .Emulator = "doja",
                .Variants = ""
            }

                Await gameListManager.AddGameAsync(game)
            End If
        Next
    End Function
    Public Sub AdjustFormPadding()
        Try
            ' Get the system DPI scaling factor
            Dim g As Graphics = Me.CreateGraphics()
            Dim dpiScale As Single = g.DpiX / 96.0F ' 96 DPI is default (100%)
            g.Dispose()

            ' Get the menu bar height dynamically (if a MenuStrip exists)
            Dim menuHeight As Integer = If(Me.MainMenuStrip IsNot Nothing, Me.MainMenuStrip.Height, 0)

            ' Calculate adjusted top padding based on DPI
            Dim adjustedPadding As Integer
            If dpiScale = 1 Then
                adjustedPadding = 65
            ElseIf dpiScale = 1.25 Then
                adjustedPadding = 65
            ElseIf dpiScale = 1.5 Then
                adjustedPadding = 65
            ElseIf dpiScale = 1.75 Then
                adjustedPadding = 65
            ElseIf dpiScale = 2 Then
                adjustedPadding = 65
            End If

            ' Apply padding to the form
            Me.Padding = New Padding(0, adjustedPadding, 0, 3)

            ' Adjust TabControl position
            If MaterialTabControl1 IsNot Nothing Then
                MaterialTabControl1.Top = Me.MainMenuStrip.Bottom
                MaterialTabControl1.Left = Me.MainMenuStrip.Left

                ' Now, adjust the form's client size so that MaterialTabControl1 has 3 pixels of space on the right and bottom.
                Dim newClientWidth As Integer = MaterialTabControl1.Left + MaterialTabControl1.Width - 8
                Dim newClientHeight As Integer = MaterialTabControl1.Top + MaterialTabControl1.Height - 16

                Me.ClientSize = New Size(newClientWidth, newClientHeight)
            End If
        Catch ex As Exception
            MessageBox.Show("Error adjusting form padding: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Public Sub HighlightMachiChara()
        For Each item As ListViewItem In ListViewMachiChara.Items
            Dim mc As MachiChara = CType(item.Tag, MachiChara)
            If mc IsNot Nothing Then
                Dim cfdPath As String = Path.Combine(DownloadsFolder, mc.CFDName)
                If File.Exists(cfdPath) Then
                    item.BackColor = Color.LightGreen
                Else
                    item.BackColor = Color.White ' Optional: reset if not found
                End If
            End If
        Next
    End Sub
    Public Function LoadConnectedControllers() As Boolean
        Dim directInput = New DirectInput()
        Dim devices = directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly)

        For Each deviceInfo In devices
            cbxGameControllers.Items.Add(deviceInfo.InstanceName)
            Logger.LogInfo($"Detected controller: {deviceInfo.InstanceName}")
        Next

        If cbxGameControllers.Items.Count > 0 Then
            cbxGameControllers.SelectedIndex = 0
            cbxGameControllers.Enabled = True
            Return True
        Else
            MessageBox.Show("No controllers detected... Please connect one and try again.")
            Return False
        End If
    End Function
    Public Function LoadControllerProfiles() As Boolean
        Dim profilePath As String = Path.Combine(ToolsFolder, "controller-profiles")

        If Not Directory.Exists(profilePath) Then
            Logger.LogInfo($"Controller profile directory not found: {profilePath}")
            MessageBox.Show("Profile folder not found.")
            Return False
        End If

        For Each file In Directory.GetFiles(profilePath, "*.gamecontroller.amgp")
            Dim profileName = Path.GetFileNameWithoutExtension(file).Replace(".gamecontroller", "")
            cbxControllerProfile.Items.Add(profileName)
            Logger.LogInfo($"Loaded profile: {profileName}")
        Next

        If cbxControllerProfile.Items.Count > 0 Then
            cbxControllerProfile.SelectedIndex = 0
            cbxControllerProfile.Enabled = True
            Return True
        Else
            MessageBox.Show("No controller profiles detected... Please download one before enabling.")
            Return False
        End If
    End Function


    ' LISTBOX/LISTVIEW CHANGES
    Private Sub ListViewGames_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListViewGames.SelectedIndexChanged
        ' Restart the timer on selection change
        selectionTimer.Stop()
        selectionTimer.Start()
    End Sub
    Private Async Sub SelectionTimer_Tick(sender As Object, e As EventArgs) Handles selectionTimer.Tick
        selectionTimer.Stop() ' Stop the timer to avoid repeated triggering

        ' Check if any items are selected
        If ListViewGames.SelectedItems.Count = 0 Then Return

        ' Perform actions once after all selections are done
        Await LoadGameVariantsAsync()
        Await DownloadGames(False)
        EnableButtons()
    End Sub
    Private Async Sub ListViewGamesVariants_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListViewGamesVariants.SelectedIndexChanged
        Await DownloadGames(False)
    End Sub
    Private Sub lbxMachiCharaList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListViewMachiChara.SelectedIndexChanged
        If ListViewMachiChara.SelectedItems.Count = 0 Then Return
        Dim selectedItem As ListViewItem = ListViewMachiChara.SelectedItems(0)
        Dim selectedMachiChara As MachiChara = CType(selectedItem.Tag, MachiChara)
        DownloadMachiChara(selectedMachiChara)
    End Sub


    ' CheckBox Changes
    Private Sub chkbxHidePhoneUI_CheckedChanged(sender As Object, e As EventArgs) Handles chkbxHidePhoneUI.CheckedChanged
        configManager.UpdateDOJAHideUISetting(chkbxHidePhoneUI.Checked)
    End Sub
    Private Sub chkbxShaderGlass_CheckedChanged(sender As Object, e As EventArgs) Handles chkbxShaderGlass.CheckedChanged
        configManager.UpdateUseShaderGlassSetting(chkbxShaderGlass.Checked)
    End Sub
    Private Sub chkEnableController_CheckedChanged(sender As Object, e As EventArgs) Handles chkbxEnableController.CheckedChanged
        If Not chkbxEnableController.Checked Then
            Logger.LogInfo("Controller support disabled by user.")

            cbxControllerProfile.Items.Clear()
            cbxGameControllers.Items.Clear()
            cbxGameControllers.Enabled = False
            cbxControllerProfile.Enabled = False
            Return
        End If

        Logger.LogInfo("Controller support enabled by user.")

        cbxControllerProfile.Items.Clear()
        cbxGameControllers.Items.Clear()

        If Not LoadConnectedControllers() Then
            Logger.LogInfo("No controllers detected. Disabling controller support.")
            chkbxEnableController.Checked = False
            Return
        End If

        If Not LoadControllerProfiles() Then
            Logger.LogInfo("No controller profiles found. Disabling controller support.")
            chkbxEnableController.Checked = False
        End If
    End Sub

    ' ComboBox Changes
    Private Sub cobxAudioType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cobxAudioType.SelectedIndexChanged
        If cobxAudioType.SelectedItem.ToString = "Standard" Or cobxAudioType.SelectedItem.ToString = "903i" Then
            configManager.UpdateDOJASoundSetting(cobxAudioType.SelectedItem.ToString)
            If cobxAudioType.SelectedItem.ToString = "903i" Then
                lblAudioWarning.Visible = True
            Else
                lblAudioWarning.Visible = False
            End If
        End If
    End Sub
    Private Async Sub cbxEmuType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxFilterType.SelectedIndexChanged
        Await FilterAndHighlightGamesAsync()
    End Sub
    Private Sub cbxStarSDK_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxStarSDK.SelectedIndexChanged
        ' Ensure the SDKs are selected
        If cbxStarSDK.SelectedItem Is Nothing Then
            MessageBox.Show("Please select a Star SDK before launching.")
            Return
        End If
        ' Store selected SDKs in variables
        Dim selectedStarSDK As String = cbxStarSDK.SelectedItem.ToString()
        Starpath = Path.Combine(ToolsFolder, selectedStarSDK)
        StarEXE = Path.Combine(ToolsFolder, selectedStarSDK, "bin", "star.exe")
    End Sub
    Private Sub cbxDojaSDK_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxDojaSDK.SelectedIndexChanged
        ' Ensure the SDKs are selected
        If cbxDojaSDK.SelectedItem Is Nothing Then
            MessageBox.Show("Please select a Doja SDK before launching.")
            Return
        End If
        ' Store selected SDKs in variables
        Dim selectedDojaSDK As String = cbxDojaSDK.SelectedItem.ToString()
        Dojapath = Path.Combine(ToolsFolder, selectedDojaSDK)
        DojaEXE = Path.Combine(ToolsFolder, selectedDojaSDK, "bin", "doja.exe")

        If cbxDojaSDK.SelectedItem.ToString().Contains("3.5") Then
            If chkbxShaderGlass.Checked = True Then
                MessageBox.Show("Doja 3.5 does not work with ShaderGlass Disabling.")
                chkbxShaderGlass.Checked = False
            End If
        End If
    End Sub

    'Textbox Changes
    Private Async Sub txtLVSearch_TextChanged(sender As Object, e As EventArgs) Handles txtLVSearch.TextChanged
        Await FilterAndHighlightGamesAsync()
    End Sub

    'ContextMenuStrip Changes
    'Appli CMS
    Private Async Sub cmsGameLV_Download_Click(sender As Object, e As EventArgs) Handles cmsGameLV_Download.Click
        Await DownloadGames(True)
    End Sub
    Private Async Sub cmsGameLV_Delete_Click(sender As Object, e As EventArgs) Handles cmsGameLV_Delete.Click
        Await DeleteGamesAsync()
    End Sub
    Private Async Sub cmsGameLV_Favorite_Click(sender As Object, e As EventArgs) Handles cmsGameLV_Favorite.Click
        If ListViewGames.SelectedItems.Count = 0 Then Return

        Dim favoritesManager As New FavoritesManager()
        Dim addedCount As Integer = 0
        Dim removedCount As Integer = 0

        For Each item As ListViewItem In ListViewGames.SelectedItems
            Dim selectedGameTitle As String = item.Text

            If favoritesManager.IsGameFavorited(selectedGameTitle) Then
                favoritesManager.RemoveFromFavorites(selectedGameTitle)
                removedCount += 1
            Else
                favoritesManager.AddToFavorites(selectedGameTitle)
                addedCount += 1
            End If
        Next

        ' Show summary of what happened
        Dim message As String = ""
        If addedCount > 0 Then message &= $"{addedCount} added to favorites. "
        If removedCount > 0 Then message &= $"{removedCount} removed from favorites."
        If message <> "" Then
            UtilManager.ShowSnackBar(message.Trim())
        End If

        ' Refresh the UI to reflect the new favorite status
        RefreshGameHighlighting()
    End Sub
    Private Async Sub cmsGameLV_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles cmsGameLV.Opening
        If ListViewGames.SelectedItems.Count = 0 Then Return

        Dim selectedGameTitle As String = ListViewGames.SelectedItems(0).Text
        Dim favoritesManager As New FavoritesManager()

        ' Simulate async favorite check (future-safe)
        Dim isFavorited As Boolean = Await Task.Run(Function()
                                                        Return favoritesManager.IsGameFavorited(selectedGameTitle)
                                                    End Function)

        cmsGameLV_Favorite.Text = If(isFavorited, "Unfavorite", "Favorite")

        ' Check background color to determine if it's a "redownload" scenario
        Dim itemColor As Color = ListViewGames.SelectedItems(0).BackColor
        Dim isNormalWindowColor As Boolean = itemColor.Equals(SystemColors.Window)
        Dim isHighlightColor As Boolean = itemColor.Equals(SystemColors.Highlight)
        Dim isWhiteColor As Boolean = itemColor.Equals(Color.White)

        cmsGameLV_Download.Text = If(Not isNormalWindowColor AndAlso Not isHighlightColor AndAlso Not isWhiteColor,
                                 "Redownload", "Download")
    End Sub
    Private Sub OpenGameFolderToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenGameFolderToolStripMenuItem.Click
        If ListViewGames.SelectedItems.Count = 0 Then Return
        Dim selectedGameTitle As String = ListViewGames.SelectedItems(0).Text
        Dim selectedGame As Game = games.FirstOrDefault(Function(g) g.ENTitle = selectedGameTitle)
        Dim gameFolder As String = Path.Combine(DownloadsFolder, Path.GetFileNameWithoutExtension(selectedGame.ZIPName.Replace(".zip", "")))
        If Directory.Exists(gameFolder) Then
            Process.Start("explorer.exe", gameFolder)
        End If
    End Sub
    'MachiChara CMS
    Private Sub DownloadCMS_MachiChara_Click(sender As Object, e As EventArgs) Handles DownloadCMS_MachiChara.Click
        If ListViewMachiChara.SelectedItems.Count = 0 Then Return
        Dim selectedItem As ListViewItem = ListViewMachiChara.SelectedItems(0)
        Dim selectedMachiChara As MachiChara = CType(selectedItem.Tag, MachiChara)
        DownloadMachiChara(selectedMachiChara)
    End Sub
    Private Async Sub DeleteCMS_MachiChara_Click(sender As Object, e As EventArgs) Handles DeleteCMS_MachiChara.Click
        Await DeleteMachiCharaAsync()
    End Sub

    'Launch Apps Buttons
    Private Async Sub btnLaunchGame_Click(sender As Object, e As EventArgs) Handles btnLaunchGame.Click, ListViewGames.DoubleClick, cmsGameLV_Launch.Click
        Try
            Logger.LogInfo("Attempting to launch game...")

            ' Ensure a game is selected
            If ListViewGames.SelectedItems.Count = 0 Then
                Logger.LogWarning("Launch failed: No game selected.")
                MessageBox.Show("Please select a game before launching.")
                Return
            End If

            ' Ensure the SDKs are selected
            If cbxDojaSDK.SelectedItem Is Nothing Then
                Logger.LogWarning("Launch failed: No Doja SDK selected.")
                MessageBox.Show("Please select a Doja SDK before launching.")
                Return
            End If

            If cbxStarSDK.SelectedItem Is Nothing Then
                Logger.LogWarning("Launch failed: No Star SDK selected.")
                MessageBox.Show("Please select a Star SDK before launching.")
                Return
            End If

            ' Store selected SDKs in variables
            Dim selectedDojaSDK As String = cbxDojaSDK.SelectedItem.ToString()
            Dim selectedStarSDK As String = cbxStarSDK.SelectedItem.ToString()
            Logger.LogInfo($"Selected Doja SDK: {selectedDojaSDK}")
            Logger.LogInfo($"Selected Star SDK: {selectedStarSDK}")

            ' Verify the game is downloaded
            If Not Await VerifyGameDownloadedAsync() Then
                Logger.LogWarning("Game launch aborted: Game not downloaded.")
                Return
            End If

            ' Get the selected game
            Dim selectedGameTitle As String = ListViewGames.SelectedItems(0).Text
            Logger.LogInfo($"Selected Game: {selectedGameTitle}")

            Dim selectedGame As Game = games.FirstOrDefault(Function(g) g.ENTitle = selectedGameTitle)
            If selectedGame Is Nothing Then
                Logger.LogError("Game not found in the games list.")
                MessageBox.Show("Game not found.")
                Return
            End If

            ' Determine correct emulator
            Dim CorrectedEmulator = VerifyEmulatorType(CurrentSelectedGameJAM)
            Logger.LogInfo($"Detected emulator type: {CorrectedEmulator}")

            ' Get Game Directory Path
            Dim GameDirectory As String = ""
            Dim binIndex As Integer = CurrentSelectedGameJAM.LastIndexOf("\bin")
            If binIndex <> -1 Then
                GameDirectory = CurrentSelectedGameJAM.Substring(0, binIndex)
                Logger.LogInfo($"Game directory resolved to: {GameDirectory}")
            Else
                Logger.LogWarning("Could not determine GameDirectory: '\bin' not found in CurrentSelectedGameJAM path.")
            End If

            ' Check for Helper Scripts
            If selectedGame.ENTitle.Contains("Dirge of Cerberus") Then
                Logger.LogInfo("Setting up FF7 Doja Helper Script...")
                Await gameManager.FF7_DOCLE_SetupAsync(Dojapath, GameDirectory)
            End If

            ' Start Launching Game
            UtilManager.ShowSnackBar($"Launching '{selectedGameTitle}'")
            UtilManager.SendAppLaunch(Path.GetFileName(CurrentSelectedGameJAM))
            Select Case CorrectedEmulator.ToLower()
                Case "doja"
                    Dim isDojaRunning As Boolean = UtilManager.CheckAndCloseDoja()
                    Dim isStarRunning As Boolean = UtilManager.CheckAndCloseStar()
                    If Not isDojaRunning AndAlso Not isStarRunning Then
                        Logger.LogInfo("Launching game using DOJA emulator.")
                        utilManager.LaunchCustomDOJAGameCommand(Dojapath, DojaEXE, CurrentSelectedGameJAM)
                        Logger.LogInfo($"Launched with: DojaPath={Dojapath}, DojaEXE={DojaEXE}, GamePath={CurrentSelectedGameJAM}")
                    Else
                        Logger.LogWarning("DOJA emulator or STAR emulator is already running. Skipping launch.")
                    End If

                Case "star"
                    Dim isStarRunning As Boolean = UtilManager.CheckAndCloseStar()
                    Dim isDojaRunning As Boolean = UtilManager.CheckAndCloseDoja()
                    If Not isStarRunning AndAlso Not isDojaRunning Then
                        Logger.LogInfo("Launching game using STAR emulator.")
                        utilManager.LaunchCustomSTARGameCommand(Starpath, StarEXE, CurrentSelectedGameJAM)
                        Logger.LogInfo($"Launched with: StarPath={Starpath}, StarEXE={StarEXE}, GamePath={CurrentSelectedGameJAM}")
                    Else
                        Logger.LogWarning("STAR emulator or DOJA emulator is already running. Skipping launch.")
                    End If

                Case Else
                    Logger.LogError($"Unknown emulator type: '{CorrectedEmulator}'. Aborting launch.")
                    MessageBox.Show("Unknown emulator type detected. Cannot launch the game.")
            End Select

        Catch ex As Exception
            Logger.LogError($"Error Launching Game:{vbCrLf}{ex}")
            MessageBox.Show($"Error Launching Game:{vbCrLf}{ex.Message}")
        End Try
    End Sub
    Private Sub btnMachiCharaLaunch_Click(sender As Object, e As EventArgs) Handles btnMachiCharaLaunch.Click
        If ListViewMachiChara.SelectedItems.Count = 0 Then
            MessageBox.Show("Please select a MachiChara to launch.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim selectedItem As ListViewItem = ListViewMachiChara.SelectedItems(0)
        Dim selectedMachiChara As MachiChara = CType(selectedItem.Tag, MachiChara)

        If selectedMachiChara IsNot Nothing Then
            CurrentSelectedMachiCharaCFD = Path.Combine(DownloadsFolder, selectedMachiChara.CFDName)
            UtilManager.ShowSnackBar($"Launching '{CurrentSelectedMachiCharaCFD}'")
            UtilManager.SendAppLaunch(Path.GetFileName(CurrentSelectedMachiCharaCFD))
            utilManager.LaunchCustomMachiCharaCommand(MachiCharaExe, CurrentSelectedMachiCharaCFD)
        End If
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
        ' Create a new MaterialForm for About
        Dim aboutForm As New ReaLTaiizor.Forms.MaterialForm With {
        .Text = "About",
        .Size = New Size(400, 250),
        .StartPosition = FormStartPosition.CenterScreen,
        .Sizable = False,
        .FormBorderStyle = FormBorderStyle.FixedDialog,
        .MaximizeBox = False,
        .MinimizeBox = False
    }

        ' Build the About text
        Dim aboutText As String =
        "Keitai World Launcher" & Environment.NewLine & Environment.NewLine &
        $"Version: B{KeitaiWorldLauncher.My.Application.Info.Version.ToString}"

        ' Create label for content
        Dim lblAbout As New Label With {
        .Text = aboutText,
        .AutoSize = False,
        .Left = 20,
        .Top = 80,
        .Width = aboutForm.ClientSize.Width - 40,
        .Height = 80,
        .Font = New Font("Segoe UI", 10, FontStyle.Regular),
        .ForeColor = Color.Black
    }

        ' Create Close button
        Dim btnClose As New ReaLTaiizor.Controls.MaterialButton With {
        .Text = "Close",
        .Width = 100,
        .Height = 36,
        .Left = aboutForm.ClientSize.Width - 120,
        .Top = aboutForm.ClientSize.Height - 60,
        .HighEmphasis = True,
        .Type = ReaLTaiizor.Controls.MaterialButton.MaterialButtonType.Contained
    }
        AddHandler btnClose.Click, Sub() aboutForm.Close()

        ' Add controls and show
        aboutForm.Controls.Add(lblAbout)
        aboutForm.Controls.Add(btnClose)
        aboutForm.ShowDialog()
    End Sub
    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Application.Exit()
    End Sub
    Public Shared Sub QuitApplication()
        Application.Exit()
    End Sub
    Private Async Sub GamelistToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GamelistToolStripMenuItem.Click
        OpenFileDialog1.Title = "Select Master Game Zip file"
        If OpenFileDialog1.ShowDialog = DialogResult.OK Then
            Await utilManager.ProcessZipFileforGamelistAsync(OpenFileDialog1.FileName)
        End If
    End Sub
    Private Sub RefreshToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RefreshToolStripMenuItem.Click
        Application.Restart()
    End Sub
    Private Sub KeyConfiguratorToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles KeyConfiguratorToolStripMenuItem.Click
        Dim processName As String = "antimicrox" ' No .exe
        Dim isRunning As Boolean = process.GetProcessesByName(processName).Length > 0

        If isRunning = False Then
            Dim Apppath = $"{ToolsFolder}\antimicrox\bin\antimicrox.exe"
            Dim startInfo As New ProcessStartInfo()
            startInfo.FileName = Apppath
            startInfo.UseShellExecute = False
            startInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory ' Set working directory
            Dim process As Process = Process.Start(startInfo)
        Else
            Dim Apppath = $"{ToolsFolder}\antimicrox\bin\antimicrox.exe"
            Dim args = $"--show"
            Dim startInfo As New ProcessStartInfo()
            startInfo.FileName = Apppath
            startInfo.Arguments = args
            startInfo.UseShellExecute = False
            startInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory ' Set working directory
            Dim process As Process = Process.Start(startInfo)
        End If
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
    Private Async Sub AddGameToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddGameToolStripMenuItem.Click
        Dim ENTitle = InputBox("Enter the English name of the game:", "English Game Name").Trim()

        If String.IsNullOrWhiteSpace(ENTitle) Then
            MsgBox("The English game name cannot be empty. Exiting the operation.", MsgBoxStyle.Exclamation, "Input Error")
            Exit Sub
        End If

        Dim JPTitle = InputBox("Enter the Japanese name of the game (leave blank if same as English):", "Japanese Game Name").Trim()
        If String.IsNullOrWhiteSpace(JPTitle) Then JPTitle = ENTitle

        Dim ZIPName = InputBox("Enter the name of the zip file (include .zip at the end):", "Zip File Name").Trim()
        If String.IsNullOrWhiteSpace(ZIPName) Then
            MsgBox("The ZIP file name cannot be empty. Exiting the operation.", MsgBoxStyle.Exclamation, "Input Error")
            Exit Sub
        End If
        If Not ZIPName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) Then
            ZIPName += ".zip"
        End If

        Dim DownloadURL = InputBox("Enter the download URL for the zip file:", "Download URL").Trim()
        If String.IsNullOrWhiteSpace(DownloadURL) Then
            MsgBox("The download URL cannot be empty. Exiting the operation.", MsgBoxStyle.Exclamation, "Input Error")
            Exit Sub
        End If

        Dim AppIconURL = InputBox("Enter the URL of a custom app icon (24x24) or leave blank to use the default icon:", "App Icon URL").Trim()
        Dim SDCardData = InputBox("Enter the SD Card data zip URL or leave blank if not applicable:", "SD Card Data URL").Trim()
        Dim Emulator = InputBox("Enter the emulator type (doja or star):", "Emulator").Trim().ToLower()

        While Emulator <> "doja" AndAlso Emulator <> "star"
            Emulator = InputBox("Invalid input. Please enter only 'doja' or 'star':", "Emulator").Trim().ToLower()
            If String.IsNullOrWhiteSpace(Emulator) Then
                MsgBox("The emulator type cannot be empty. Exiting the operation.", MsgBoxStyle.Exclamation, "Input Error")
                Exit Sub
            End If
        End While

        Dim newGame As New Game With {
        .ENTitle = ENTitle,
        .JPTitle = JPTitle,
        .ZIPName = ZIPName,
        .DownloadURL = DownloadURL,
        .CustomAppIconURL = AppIconURL,
        .SDCardDataURL = SDCardData,
        .Emulator = Emulator
    }

        ' ✅ Await the actual async save operation
        Await gameListManager.AddGameAsync(newGame)

        MessageBox.Show("Added. Make sure you download this AppConfig and upload.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
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
    Private Sub NetworkUIDConfigToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NetworkUIDConfigToolStripMenuItem.Click
        Dim promptForm As New ReaLTaiizor.Forms.MaterialForm With {
        .Text = "Enter Network UID",
        .Size = New Size(540, 320),
        .FormBorderStyle = FormBorderStyle.FixedDialog,
        .StartPosition = FormStartPosition.CenterScreen,
        .Sizable = False
    }

        ' Push everything down by starting higher top margin
        Dim lblInstructions As New Label With {
        .Text = "How to get your Network UID:" & Environment.NewLine &
                "1. Join the Keitai Wiki Discord." & Environment.NewLine &
                "2. Navigate to the #i-mode channel." & Environment.NewLine &
                "3. Type '/get-uid' and ButlerSheep will DM you your UID.",
        .AutoSize = False,
        .Left = 20,
        .Top = 80, ' increased spacing from top
        .Width = 490,
        .Height = 90
    }

        Dim txtInput As New ReaLTaiizor.Controls.MaterialTextBoxEdit With {
        .Left = 20,
        .Top = lblInstructions.Top + lblInstructions.Height + 20,
        .Size = New Size(490, 40),
        .Text = NetworkUID,
        .MaxLength = 50,
        .UseSystemPasswordChar = False,
        .Hint = "Enter your Network UID..."
    }

        Dim btnOk As New ReaLTaiizor.Controls.MaterialButton With {
        .Text = "OK",
        .DialogResult = DialogResult.OK,
        .Left = 290,
        .Top = txtInput.Top + txtInput.Height + 25,
        .Width = 100,
        .HighEmphasis = True,
        .Type = ReaLTaiizor.Controls.MaterialButton.MaterialButtonType.Contained
    }

        Dim btnCancel As New ReaLTaiizor.Controls.MaterialButton With {
        .Text = "CANCEL",
        .DialogResult = DialogResult.Cancel,
        .Left = 400,
        .Top = txtInput.Top + txtInput.Height + 25,
        .Width = 100,
        .HighEmphasis = False,
        .Type = ReaLTaiizor.Controls.MaterialButton.MaterialButtonType.Text
    }

        promptForm.Controls.Add(lblInstructions)
        promptForm.Controls.Add(txtInput)
        promptForm.Controls.Add(btnOk)
        promptForm.Controls.Add(btnCancel)
        promptForm.AcceptButton = btnOk
        promptForm.CancelButton = btnCancel

        If promptForm.ShowDialog() = DialogResult.OK Then
            Dim newNetworkUID As String = txtInput.Text.Trim().ToUpper
            configManager.UpdateNetworkUIDSetting(newNetworkUID)
            NetworkUID = newNetworkUID
        End If
    End Sub
    Private Sub AddGamesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddGamesToolStripMenuItem.Click
        Dim Result = MessageBox.Show($"Select the folder with the games you want to add.{vbCrLf}Ensure the .jar/.jam/.sp are named the same, and located in the same folder.", "Add Custom Games", MessageBoxButtons.OKCancel)

        If Result = DialogResult.Cancel Then
            Exit Sub
        End If

        Using folderDialog As New FolderBrowserDialog()
            folderDialog.Description = "Select the folder containing .jar files"
            If folderDialog.ShowDialog() = DialogResult.OK Then
                Dim selectedFolder As String = folderDialog.SelectedPath
                Dim jarFiles As String() = Directory.GetFiles(selectedFolder, "*.jar")

                Dim validJarFiles As New List(Of String)()
                Dim skippedJarFiles As New List(Of String)()
                Dim processedFolders As New List(Of String)()

                ' Check which .jar files have matching .jam and .sp files
                Dim filesToProcess As New Dictionary(Of String, Tuple(Of String, String, String))()

                For Each jarFile As String In jarFiles
                    Dim fileNameWithoutExt As String = Path.GetFileNameWithoutExtension(jarFile)
                    Dim jamFile As String = Path.Combine(selectedFolder, fileNameWithoutExt & ".jam")
                    Dim spFile As String = Path.Combine(selectedFolder, fileNameWithoutExt & ".sp")
                    Dim destinationPath As String = Path.Combine("data", "downloads", fileNameWithoutExt)

                    ' Ensure .jam and .sp files exist
                    If File.Exists(jamFile) AndAlso File.Exists(spFile) Then
                        ' Check if destination folder already exists
                        If Directory.Exists(destinationPath) Then
                            skippedJarFiles.Add(Path.GetFileName(jarFile))
                        Else
                            validJarFiles.Add(Path.GetFileName(jarFile))
                            filesToProcess.Add(fileNameWithoutExt, Tuple.Create(jarFile, jamFile, spFile))
                        End If
                    End If
                Next

                ' If no valid files to process, notify and exit
                If validJarFiles.Count = 0 AndAlso skippedJarFiles.Count = 0 Then
                    MessageBox.Show("No valid .jar files found with matching .jam and .sp files.", "No Files Found", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Exit Sub
                End If

                ' Build confirmation message
                Dim message As String = ""

                If validJarFiles.Count > 0 Then
                    message &= "The following .jar files will be added:" & Environment.NewLine &
                           String.Join(Environment.NewLine, validJarFiles) & Environment.NewLine & Environment.NewLine
                End If

                If skippedJarFiles.Count > 0 Then
                    message &= "The following .jar files will be skipped (destination folder already exists):" & Environment.NewLine &
                           String.Join(Environment.NewLine, skippedJarFiles) & Environment.NewLine & Environment.NewLine
                End If

                message &= "Do you want to proceed?"

                ' Ask for confirmation
                If MessageBox.Show(message, "Confirm Processing", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                    Exit Sub
                End If

                ' Process only the valid files that passed the validation
                For Each entry In filesToProcess
                    Dim fileNameWithoutExt As String = entry.Key
                    Dim jarFile As String = entry.Value.Item1
                    Dim jamFile As String = entry.Value.Item2
                    Dim spFile As String = entry.Value.Item3
                    Dim destinationPath As String = Path.Combine("data", "downloads", fileNameWithoutExt)

                    Dim binPath As String = Path.Combine(destinationPath, "bin")
                    Dim spPath As String = Path.Combine(destinationPath, "sp")

                    ' Create directories if they don't exist
                    Directory.CreateDirectory(binPath)
                    Directory.CreateDirectory(spPath)

                    ' Move the files
                    File.Copy(jarFile, Path.Combine(binPath, Path.GetFileName(jarFile)), True)
                    File.Copy(jamFile, Path.Combine(binPath, Path.GetFileName(jamFile)), True)
                    File.Copy(spFile, Path.Combine(spPath, Path.GetFileName(spFile)), True)

                    ' Add to processed list
                    processedFolders.Add(fileNameWithoutExt)
                Next

                ' Append processed folder names to customgames.txt
                If processedFolders.Count > 0 Then
                    Dim configPath As String = "configs/customgames.txt"

                    ' Ensure 'configs' folder exists
                    Directory.CreateDirectory(Path.GetDirectoryName(configPath))

                    ' Append processed folder names to the file
                    File.AppendAllLines(configPath, processedFolders)
                End If

                MessageBox.Show("Added complete!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Application.Restart()
            End If
        End Using
    End Sub
    Private Sub ControlsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ControlsToolStripMenuItem.Click
        ' Create a new MaterialForm
        Dim keybindForm As New ReaLTaiizor.Forms.MaterialForm With {
        .Text = "Keybinds",
        .Size = New Size(900, 500),
        .StartPosition = FormStartPosition.CenterScreen,
        .Sizable = False,
        .FormBorderStyle = FormBorderStyle.FixedDialog,
        .MaximizeBox = False,
        .MinimizeBox = False
    }

        ' Keybinds content
        Dim keybindText As String =
        "Doja & Star Keybinds:" & Environment.NewLine &
        "--------------------------" & Environment.NewLine &
        "Phone Button        Keyboard" & Environment.NewLine &
        "--------------------------" & Environment.NewLine &
        "UP                 → Up Arrow" & Environment.NewLine &
        "DOWN               → Down Arrow" & Environment.NewLine &
        "LEFT               → Left Arrow" & Environment.NewLine &
        "RIGHT              → Right Arrow" & Environment.NewLine &
        "Top Left Button    → A" & Environment.NewLine &
        "Top Center Button   → D" & Environment.NewLine &
        "Top Right Button   → S" & Environment.NewLine &
        "Center Button      → Enter" & Environment.NewLine &
        "123456789*#        → 123456789*#"

        ' Add a label to display the keybinds
        Dim lblKeybinds As New Label With {
        .Text = keybindText,
        .AutoSize = False,
        .Left = 20,
        .Top = 80,
        .Width = 360,
        .Height = 300,
        .Font = New Font("Consolas", 10, FontStyle.Regular),
        .ForeColor = Color.Black
    }

        ' Load the image if it exists
        Dim imagePath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "tools", "skins", "doja", "doja_controls.png")
        Dim picControls As New PictureBox With {
        .Left = 400,
        .Top = 80,
        .Size = New Size(360, 300),
        .SizeMode = PictureBoxSizeMode.AutoSize,
        .Visible = File.Exists(imagePath)
    }

        If picControls.Visible Then
            picControls.Image = Image.FromFile(imagePath)
        Else
            Logger.LogInfo("Image not found for Keybind Form: " & imagePath)
        End If

        ' Add a close button
        Dim btnClose As New ReaLTaiizor.Controls.MaterialButton With {
        .Text = "Close",
        .Width = 100,
        .Height = 36,
        .Left = keybindForm.ClientSize.Width - 120,
        .Top = keybindForm.ClientSize.Height - 60,
        .HighEmphasis = True,
        .Type = ReaLTaiizor.Controls.MaterialButton.MaterialButtonType.Contained
    }
        AddHandler btnClose.Click, Sub() keybindForm.Close()

        ' Add controls to the form
        keybindForm.Controls.Add(lblKeybinds)
        keybindForm.Controls.Add(picControls)
        keybindForm.Controls.Add(btnClose)

        ' Show the form
        keybindForm.ShowDialog()
    End Sub
    Private Sub TroubleshootingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TroubleshootingToolStripMenuItem.Click
        ' Create a new MaterialForm
        Dim TroubleShootingForm As New ReaLTaiizor.Forms.MaterialForm With {
            .Text = "Troubleshooting",
            .Size = New Size(600, 500),
            .StartPosition = FormStartPosition.CenterScreen,
            .Sizable = False,
            .FormBorderStyle = FormBorderStyle.FixedDialog,
            .MaximizeBox = False,
            .MinimizeBox = False
        }

        Dim troubleshootingMessage As String =
            "   TROUBLESHOOTING DOJA/STAR ISSUES" & vbCrLf & vbCrLf &
        "Grey Screen After Launching App:" & vbCrLf &
        "If the app launches and DOJA/STAR appears but the screen stays grey," & vbCrLf &
        "it usually means the required registry entries are missing." & vbCrLf &
        "To fix this:" & vbCrLf &
        "  • Navigate to: data/tools/idkDOJA5.1" & vbCrLf &
        "    → Run: doja.reg" & vbCrLf &
        "  • Navigate to: data/tools/idkSTAR2.0" & vbCrLf &
        "    → Run: star.reg" & vbCrLf & vbCrLf &
        """Failed to Detect Doja/Star Running"" Error:" & vbCrLf &
        "This error typically means one of the following:" & vbCrLf &
        "  1. The download is corrupted" & vbCrLf &
        "  2. Java is not correctly installed" & vbCrLf &
        "  3. Your file path contains spaces" & vbCrLf & vbCrLf &
        "Steps to fix:" & vbCrLf &
        "  • Try redownloading the game:" & vbCrLf &
        "    → Right-click the game title, choose 'Redownload'" & vbCrLf &
        "  • Make sure Java 8 (32-bit) is installed" & vbCrLf &
        "  • Ensure the folder path has NO spaces" & vbCrLf & vbCrLf &
        "If you're still having trouble," & vbCrLf &
        "ask for help in the #troubleshooting channel."

        Dim rtbMessage As New RichTextBox With {
        .Text = troubleshootingMessage,
        .ReadOnly = True,
        .Multiline = True,
        .ScrollBars = RichTextBoxScrollBars.Vertical,
        .BorderStyle = BorderStyle.None,
        .Font = New Font("Consolas", 10, FontStyle.Regular),
        .ForeColor = Color.Black,
        .BackColor = TroubleShootingForm.BackColor,
        .Left = 20,
        .Top = 70,
        .Width = TroubleShootingForm.ClientSize.Width - 40,
        .Height = TroubleShootingForm.ClientSize.Height - 80,
        .Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Bottom
    }

        Dim btnClose As New ReaLTaiizor.Controls.MaterialButton With {
            .Text = "Close",
            .Width = 100,
            .Height = 36,
            .Left = TroubleShootingForm.ClientSize.Width - 120,
            .Top = TroubleShootingForm.ClientSize.Height - 50,
            .HighEmphasis = True,
            .Type = ReaLTaiizor.Controls.MaterialButton.MaterialButtonType.Contained,
            .Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        }

        AddHandler btnClose.Click, Sub() TroubleShootingForm.Close()

        ' Add controls to form and show
        TroubleShootingForm.Controls.Add(rtbMessage)
        TroubleShootingForm.Controls.Add(btnClose)
        TroubleShootingForm.ShowDialog()
    End Sub
    Private Sub MachiCharaCreateXMLToolStripMenu_Click(sender As Object, e As EventArgs) Handles MachiCharaCreateXMLToolStripMenu.Click
        If FolderBrowserDialog1.ShowDialog = DialogResult.OK Then
            UtilManager.ProcessMachiChara_CFDFiles(FolderBrowserDialog1.SelectedPath)
        End If
    End Sub
End Class