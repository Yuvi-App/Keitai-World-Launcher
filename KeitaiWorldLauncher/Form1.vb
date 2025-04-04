Imports System.Diagnostics.Eventing.Reader
Imports System.Formats.Tar
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
Imports Microsoft.VisualBasic.FileIO
Imports ReaLTaiizor.Controls
Imports ReaLTaiizor.[Enum].Poison
Imports ReaLTaiizor.Forms
Imports ReaLTaiizor.Enum
Imports ReaLTaiizor.Manager
Imports ReaLTaiizor.Util

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
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Application.EnableVisualStyles()

        'Summon Splash
        splash = New SplashScreen()
        splash.Show()
        splash.Refresh()

        ' Setup material Theme
        Dim manager = MaterialSkinManager.Instance
        manager.AddFormToManage(Me)
        manager.Theme = MaterialSkinManager.Themes.LIGHT
        manager.ColorScheme = New ReaLTaiizor.Colors.MaterialColorScheme(ReaLTaiizor.Colors.MaterialPrimary.Indigo500, ReaLTaiizor.Colors.MaterialPrimary.Indigo700, ReaLTaiizor.Colors.MaterialPrimary.Indigo100, ReaLTaiizor.Colors.MaterialAccent.Pink200, MaterialTextShade.LIGHT)

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

        ' Setup SJIS 
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)

        ' Setup Dirs
        UtilManager.SetupDIRS()

        ' Setup Logging
        ' Check log File Size and Cleanup
        UtilManager.DeleteLogIfTooLarge(Path.Join(LogFolder, "app_log.txt"))
        Logger.InitializeLogger()
        Logger.LogInfo("Application started.")

        ' Load Config
        config = configManager.LoadConfig()

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
        UtilManager.CheckForSpacesInPath()
        If FirstRun = True Then
            Logger.LogInfo("Detected First Run - Checking for Admin")
            Logger.LogInfo("Starting PreReq Check")
            Dim AllGood = UtilManager.CheckforPreReq()
            If AllGood = True Then
                Logger.LogInfo("PreReq All Good")
                configManager.UpdateFirstRunSetting("false")
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
        If utilManager.IsInternetAvailable(domainOnly) Then
            'Set the User to online
            isOnline = True

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
        Else
            isOnline = False
            Me.Text = "Keitai World Launcher - Offline"
            Logger.LogWarning($"No internet connection to Domain {domainOnly}. Skipping online checks.")
        End If

        ' Load Custom Games
        LoadCustomGames()

        ' Load Game List
        LoadGameListFirstTime()

        ' Load MachiChara List
        Logger.LogInfo("Processing machichara.xml")
        Try
            machicharas = machicharaListManager.LoadMachiChara()
            If machicharas IsNot Nothing Then
                For Each mc In machicharas
                    lbxMachiCharaList.Items.Add(mc.ENTitle)
                Next
            End If
        Catch ex As Exception
            MessageBox.Show($"Failed to Load MachiChara List:{vbCrLf}{ex}")
            Logger.LogError("Failed to Load MachiChara List", ex)
        End Try

        'Last Step
        GetSDKs()

        ' Setup any Config Suff
        chkbxHidePhoneUI.Checked = DojaHideUI
        Dim atindex As Integer = cobxAudioType.FindStringExact(DOJASoundType)
        cobxAudioType.SelectedIndex = atindex
        chkbxShaderGlass.Checked = UseShaderGlass
        cbxFilterType.SelectedIndex = 0
        cbxShaderGlassScaling.SelectedIndex = 2

        ' Step 3: Close splash screen
        splash.Close()
    End Sub

    ' General Other Function
    Private Sub GetSDKs()
        Dim dojaDefault As String = "iDKDoJa5.1"
        Dim starDefault As String = "iDKStar2.0"
        Dim dojaFound As Boolean = False
        Dim starFound As Boolean = False

        ' Clear existing items
        cbxDojaSDK.Items.Clear()
        cbxStarSDK.Items.Clear()

        ' Iterate through the directories
        For Each SSDK In Directory.GetDirectories(ToolsFolder)
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

        ' Set the defaults if found
        If starFound Then
            cbxStarSDK.SelectedItem = starDefault
        Else
            MessageBox.Show($"The default SDK '{starDefault}' was not found. Please download and set it up.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If

        If dojaFound Then
            cbxDojaSDK.SelectedItem = dojaDefault
        Else
            MessageBox.Show($"The default SDK '{dojaDefault}' was not found. Please download and set it up.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub
    Private Sub LoadGameIcons()
        Dim DojaIconPath As String = Path.Combine(ToolsFolder, "icons", "defaults", "doja.gif")
        Dim StarIconPath As String = Path.Combine(ToolsFolder, "icons", "defaults", "star.gif")

        ' Validate default icons
        If Not File.Exists(DojaIconPath) OrElse Not File.Exists(StarIconPath) Then
            MessageBox.Show("Missing Doja/Star Default icons.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        ' Get all available icon files once
        Dim availableIcons As HashSet(Of String) = Directory.GetFiles(Path.Combine(ToolsFolder, "icons")).
        Select(Function(iconPath) Path.GetFileNameWithoutExtension(iconPath).ToLower()).
        ToHashSet()

        ' Loop through each game and assign the appropriate icon
        For Each game In games
            Dim iconFileName As String = Path.GetFileNameWithoutExtension(game.ZIPName).ToLower()
            Dim customIconPath As String = Path.Combine(ToolsFolder, "icons", $"{iconFileName}.gif")

            Try
                Dim iconToUse As Image
                If availableIcons.Contains(iconFileName) Then
                    ' Load custom icon
                    Using fs As New FileStream(customIconPath, FileMode.Open, FileAccess.Read)
                        iconToUse = Image.FromStream(fs)
                    End Using
                Else
                    ' Assign default icon based on emulator type
                    Dim defaultIconPath As String = If(game.Emulator.Equals("doja", StringComparison.OrdinalIgnoreCase), DojaIconPath, StarIconPath)
                    Using fs As New FileStream(defaultIconPath, FileMode.Open, FileAccess.Read)
                        iconToUse = Image.FromStream(fs)
                    End Using
                End If

                ' Add image to ImageList
                ImageListGames.Images.Add(game.ENTitle, iconToUse)

            Catch ex As Exception
                Logger.LogError($"Failed to load icon for game: {game.ENTitle}", ex)
            End Try
        Next
    End Sub
    Private Sub LoadGameListFirstTime()
        ' Load Game List
        Logger.LogInfo("Processing gamelist.xml")
        Try
            games = gameListManager.LoadGames()
            lblTotalGameCount.Text = $"Total: {games.Count}"

            ' Clear ListView and ImageList
            ListViewGames.BeginUpdate()
            ImageListGames.Images.Clear()
            ListViewGames.Items.Clear()
            ListViewGamesVariants.Clear()

            ' Adjust column width dynamically
            ListViewGames.Columns.Clear()
            ListViewGames.Columns.Add("Title", ListViewGames.ClientSize.Width - SystemInformation.VerticalScrollBarWidth, HorizontalAlignment.Left)

            ' Load icons into ImageList
            LoadGameIcons()
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
            MessageBox.Show($"Failed to Load Game List:{vbCrLf}{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Logger.LogError("Failed to Load Game List", ex)
        End Try
    End Sub
    Private Sub EnableButtons()
        ' Enable game launch button and checkbox
        btnLaunchGame.Enabled = True
        chkbxHidePhoneUI.Enabled = True
        cobxAudioType.Enabled = True
        chkbxShaderGlass.Enabled = True
        cbxShaderGlassScaling.Enabled = True
    End Sub
    Private Sub FilterAndHighlightGames()
        ' Get the selected filter and search term
        Dim selectedFilter As String = cbxFilterType.SelectedItem.ToString().ToLower()
        Dim searchTerm As String = txtLVSearch.Text.Trim()

        ' Clear the ListView
        ListViewGames.Items.Clear()

        ' Load favorites and installed games into hash sets for quick lookup
        Dim favoriteGames As HashSet(Of String) = If(File.Exists(Path.Combine("configs", "favorites.txt")),
        File.ReadAllLines(Path.Combine("configs", "favorites.txt")).Select(Function(fav) fav.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase),
        New HashSet(Of String)(StringComparer.OrdinalIgnoreCase))

        ' Load custom games into hash sets for quick lookup
        Dim customGames As HashSet(Of String) = If(File.Exists(Path.Combine("configs", "customgames.txt")),
        File.ReadAllLines(Path.Combine("configs", "customgames.txt")).Select(Function(fav) fav.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase),
        New HashSet(Of String)(StringComparer.OrdinalIgnoreCase))

        ' Load installed games into hash sets for quick lookup
        Dim installedGames As HashSet(Of String) = Directory.GetDirectories(DownloadsFolder).
        Select(Function(folder) Path.GetFileName(folder)).
        ToHashSet(StringComparer.OrdinalIgnoreCase)

        ' Loop through games and apply filtering and highlighting
        For Each game In games
            ' Extract relevant data
            Dim gameTitle As String = game.ENTitle
            Dim emulatorType As String = game.Emulator.ToLower()
            Dim zipFileName As String = Path.GetFileNameWithoutExtension(game.ZIPName)

            ' Check if the game matches the search term (case insensitive)
            Dim matchesSearch As Boolean = gameTitle.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0

            ' Check filter conditions
            Dim isFavorited As Boolean = favoriteGames.Contains(gameTitle)
            Dim isCustom As Boolean = customGames.Contains(gameTitle)
            Dim isInstalled As Boolean = Not String.IsNullOrWhiteSpace(game.ZIPName) AndAlso installedGames.Contains(zipFileName)
            Dim matchesFilter As Boolean = selectedFilter = "all" OrElse
                                    (selectedFilter = "favorites" AndAlso isFavorited) OrElse
                                    (selectedFilter = "custom" AndAlso isCustom) OrElse
                                    (selectedFilter = "installed" AndAlso isInstalled) OrElse
                                    (emulatorType = selectedFilter)

            ' Add matching games to ListView
            If matchesSearch AndAlso matchesFilter Then
                ' Create ListView item
                Dim item As New ListViewItem(gameTitle) With {
                .ImageKey = gameTitle ' Assign correct icon
            }

                ' Determine highlighting color
                If isInstalled AndAlso isFavorited Then
                    item.BackColor = Color.LightSeaGreen ' Both installed and favorited
                ElseIf isCustom Then
                    item.BackColor = Color.LightSteelBlue
                ElseIf isInstalled Then
                    item.BackColor = Color.LightGreen ' Only installed
                ElseIf isFavorited Then
                    item.BackColor = Color.LightGoldenrodYellow ' Only favorited
                Else
                    item.BackColor = Color.White ' Neither installed nor favorited
                End If

                ' Add item to ListView
                ListViewGames.Items.Add(item)
            End If
        Next

        ' Update filtered game count label
        lblFilteredGameCount.Text = $"Filtered: {ListViewGames.Items.Count}"

        ' Load game variants
        'LoadGameVariants()
    End Sub
    Private Sub LoadGameVariants()
        ' Ensure the ListView view mode is set to Details
        ListViewGamesVariants.View = View.Details
        ListViewGamesVariants.Items.Clear()
        ListViewGamesVariants.Columns.Clear()
        ListViewGamesVariants.Columns.Add("Game Variants", -2, HorizontalAlignment.Left) ' Add a column

        ' Ensure a game is selected
        If ListViewGames.SelectedItems.Count = 0 Then
            Return
        End If

        ' Get selected game
        Dim selectedGameTitle As String = ListViewGames.SelectedItems(0).Text
        Dim selectedGame As Game = games.FirstOrDefault(Function(g) g.ENTitle = selectedGameTitle)

        If selectedGame Is Nothing OrElse String.IsNullOrWhiteSpace(selectedGame.Variants) Then
            Return
        End If

        ' Split and add variants as rows in the ListView
        Dim GameVariantsSplit As String() = selectedGame.Variants.Split(","c)
        For Each va In GameVariantsSplit
            ListViewGamesVariants.Items.Add(New ListViewItem(va.Trim()))
        Next
    End Sub
    Private Async Sub DownloadGames(ContextDownload As Boolean)

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
            Exit Sub
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
    End Sub
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
                Dim CurrentSelectedGameFolder = Path.Combine(DownloadsFolder, Path.GetFileNameWithoutExtension(game.ZIPName))

                ' Check if the folder exists and attempt to delete it
                If Directory.Exists(CurrentSelectedGameFolder) Then
                    Try
                        My.Computer.FileSystem.DeleteDirectory(CurrentSelectedGameFolder, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                        deletedGames.Add(game.ENTitle)

                        ' Read CustomGame List
                        Dim configPath As String = Path.Combine(ConfigsFolder, "customgames.txt")
                        Dim CustomGamesLines As List(Of String) = File.ReadAllLines(configPath).ToList()

                        ' Remove the specific entry if it exists
                        If CustomGamesLines.Remove(Path.GetFileNameWithoutExtension(game.ZIPName)) Then
                            File.WriteAllLines(configPath, CustomGamesLines) ' Only write back if changes were made
                        End If

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
        ' Check if any item is selected in ListViewGames
        If ListViewGames.SelectedItems.Count = 0 Then
            MessageBox.Show("Please select a game.")
            Return False
        End If

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
            ' Check if the game has a variant
            If Not String.IsNullOrEmpty(selectedGame.Variants) Then
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
    Public Sub LoadCustomGames()
        Dim customGamesFile As String = Path.Combine(ConfigsFolder, "customgames.txt")

        ' Ensure the custom games file exists
        If Not File.Exists(customGamesFile) Then
            File.Create(customGamesFile).Close() ' Ensure the file is created and closed
        End If

        ' Deduplicate CustomGames.txt
        Dim customGameLines As List(Of String) = File.ReadAllLines(customGamesFile).
        Select(Function(line) line.Trim()).
        Where(Function(line) Not String.IsNullOrWhiteSpace(line)).
        Distinct(StringComparer.OrdinalIgnoreCase). ' Remove duplicates, case-insensitive
        ToList()

        ' Write back only if duplicates were removed
        If customGameLines.Count <> File.ReadAllLines(customGamesFile).Length Then
            File.WriteAllLines(customGamesFile, customGameLines)
        End If

        ' Process each folder name
        For Each gameFolderName In customGameLines
            Dim gameFolderPath As String = Path.Combine(DownloadsFolder, gameFolderName)

            ' Check if the folder exists
            If Directory.Exists(gameFolderPath) Then
                ' Create a new game object and add it using AddGames function
                Dim game As New Game With {
                .ENTitle = gameFolderName, ' Assuming folder name is the game title
                .ZIPName = gameFolderName & ".zip",
                .DownloadURL = "", ' Custom games may not have a download URL
                .CustomAppIconURL = "", ' Custom App Icon URL if needed
                .SDCardDataURL = "",
                .Emulator = "doja", ' Default, adjust based on your needs
                .Variants = ""
            }

                ' Add the game
                gameListManager.AddGame(game)
            End If
        Next
    End Sub
    Private Sub AdjustFormPadding()
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
                MaterialTabControl1.Top = Me.MainMenuStrip.Bottom + 5
                MaterialTabControl1.Left = Me.Padding.Left

                ' Now, adjust the form's client size so that MaterialTabControl1 has 3 pixels of space on the right and bottom.
                Dim newClientWidth As Integer = MaterialTabControl1.Left + MaterialTabControl1.Width + 3
                Dim newClientHeight As Integer = MaterialTabControl1.Top + MaterialTabControl1.Height - 20

                Me.ClientSize = New Size(newClientWidth, newClientHeight)
            End If
        Catch ex As Exception
            MessageBox.Show("Error adjusting form padding: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    ' LISTBOX/LISTVIEW CHANGES
    Private Sub ListViewGames_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListViewGames.SelectedIndexChanged
        ' Restart the timer on selection change
        selectionTimer.Stop()
        selectionTimer.Start()
    End Sub
    Private Sub SelectionTimer_Tick(sender As Object, e As EventArgs) Handles selectionTimer.Tick
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
            If cobxAudioType.SelectedItem.ToString = "903i" Then
                lblAudioWarning.Visible = True
            Else
                lblAudioWarning.Visible = False
            End If
        End If
    End Sub
    Private Sub cbxEmuType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxFilterType.SelectedIndexChanged
        FilterAndHighlightGames()
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
    Private Sub cmsGameLV_Favorite_Click(sender As Object, e As EventArgs) Handles cmsGameLV_Favorite.Click
        If ListViewGames.SelectedItems.Count = 0 Then Return

        Dim selectedGameTitle As String = ListViewGames.SelectedItems(0).Text
        Dim favoritesManager As New FavoritesManager()

        If favoritesManager.IsGameFavorited(selectedGameTitle) Then
            favoritesManager.RemoveFromFavorites(selectedGameTitle)
            utilManager.ShowSnackBar($"'{selectedGameTitle}' has been removed from favorites.")
        Else
            favoritesManager.AddToFavorites(selectedGameTitle)
            utilManager.ShowSnackBar($"'{selectedGameTitle}' has been added to favorites.")
        End If

        ' Optionally refresh the UI to indicate favorite status
        RefreshGameHighlighting()
    End Sub
    Private Sub cmsGameLV_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles cmsGameLV.Opening
        If ListViewGames.SelectedItems.Count = 0 Then Return
        Dim selectedGameTitle As String = ListViewGames.SelectedItems(0).Text
        Dim favoritesManager As New FavoritesManager()
        If FavoritesManager.IsGameFavorited(selectedGameTitle) Then
            cmsGameLV_Favorite.Text = "Unfavorite"
        Else
            cmsGameLV_Favorite.Text = "Favorite"
        End If
        Dim itemColor As Color = ListViewGames.SelectedItems(0).BackColor
        Dim isNormalWindowColor = itemColor.Equals(SystemColors.Window)
        Dim isHighlightColor = itemColor.Equals(SystemColors.Highlight)
        Dim isWhiteColor = itemColor.Equals(Color.White)

        If Not isNormalWindowColor AndAlso Not isHighlightColor AndAlso Not isWhiteColor Then
            cmsGameLV_Download.Text = "Redownload"
        Else
            cmsGameLV_Download.Text = "Download"
        End If
    End Sub

    'Launch Game
    Private Sub btnLaunchGame_Click(sender As Object, e As EventArgs) Handles btnLaunchGame.Click, ListViewGames.DoubleClick, cmsGameLV_Launch.Click
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
            If Not VerifyGameDownloaded() Then
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
                gameManager.FF7_DOCLE_Setup(Dojapath, GameDirectory)
            End If

            ' Start Launching Game
            UtilManager.ShowSnackBar($"Launching '{selectedGameTitle}'")

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
            Dim newNetworkUID As String = txtInput.Text.Trim()
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
            .Size = New Size(600, 400),
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
            "Top Right Button   → S" & Environment.NewLine &
            "Center Button      → Enter" & Environment.NewLine &
            "123456789*#        → 123456789*#"

        ' Add a MaterialLabel to display the keybinds
        Dim lblKeybinds As New Label With {
            .Text = keybindText,
            .AutoSize = False,
            .Left = 20,
            .Top = 80,
            .Width = keybindForm.ClientSize.Width - 40,
            .Height = 220,
            .Font = New Font("Consolas", 10, FontStyle.Regular),
            .ForeColor = Color.Black
        }

        ' Add a MaterialButton to close the form
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

        ' Add controls to form and show
        keybindForm.Controls.Add(lblKeybinds)
        keybindForm.Controls.Add(btnClose)
        keybindForm.ShowDialog()
    End Sub
End Class