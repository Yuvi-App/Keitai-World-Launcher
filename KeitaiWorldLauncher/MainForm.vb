Imports System.IO
Imports System.Text
Imports KeitaiWorldLauncher.My.logger
Imports KeitaiWorldLauncher.My.Managers
Imports KeitaiWorldLauncher.My.Models
Imports ReaLTaiizor.Controls
Imports ReaLTaiizor.[Enum].Poison
Imports ReaLTaiizor.Forms
Imports SharpDX.XInput

Public Class MainForm
    'Global Vars
    Private splash As SplashScreen
    Dim isDebug As Boolean = False
    Dim isOnline As Boolean = False
    Dim ApploadManager As New AppLoadManager()
    Dim applitrackerManager As New AppliTrackerManager
    Dim configManager As New ConfigManager()
    Dim utilManager As New UtilManager
    Dim gameListManager As New GameListManager()
    Dim gameManager As New GameManager()
    Dim machicharaListManager As New MachiCharaListManager()
    Dim charadenListManager As New CharaDenListManager()
    Dim SaveDataManager As New SaveDataManager()
    Dim zipManager As New ZipManager()
    Dim config As Dictionary(Of String, String)
    Dim games As List(Of Game)
    Dim machicharas As List(Of MachiChara)
    Dim charadens As List(Of CharaDen)
    Dim XInputDevices As New Dictionary(Of String, Integer)
    Private Shared isGameDownloadInProgress As Boolean = False
    Dim CompletedBootSequence As Boolean = False
    Public Shared UsingJDK1_8 As Boolean = False

    'Directory Var
    Public DownloadsFolder As String = "data\downloads"
    Public ToolsFolder As String = "data\tools"
    Public ConfigsFolder As String = "configs"
    Public LogFolder As String = "logs"
    Public NetworkUIDTxtFile As String = Path.Combine(ConfigsFolder, "networkuid.txt")
    Public FavoritesTxtFile As String = Path.Combine(ConfigsFolder, "favorites.txt")
    Public CustomGamesTxtFile As String = Path.Combine(ConfigsFolder, "customgames.txt")
    Public PlaytimesTxtFile As String = Path.Combine(ConfigsFolder, "playtimes.txt")

    'Index Vars Can Change
    Dim CurrentSelectedGameJAM As String
    Dim CurrentSelectedGameJAR As String
    Dim CurrentSelectedGameSP As String
    Dim CurrentSelectedMachiCharaCFD As String
    Dim CurrentSelectedCharaDenAFD As String

    'Config Vars
    Public versionCheckUrl As String
    Public autoUpdate As Boolean
    Public FirstRun As Boolean
    Public gameListUrl As String
    Public autoUpdateGameList As Boolean
    Public machicharaListUrl As String
    Public autoUpdatemachicharaList As Boolean
    Public charadenListUrl As String
    Public autoUpdatecharadenList As Boolean
    Public UseShaderGlass As Boolean
    Public UseDialPad As Boolean
    Public NetworkUID As String
    Public DOJApath As String
    Public DOJAEXE As String
    Public DOJAHideUI As Boolean
    Public DOJASoundType As String
    Public STARpath As String
    Public STAREXE As String
    Public JSKYpath As String
    Public JSKYEXE As String
    Public SOFTBANKpath As String
    Public SOFTBANKEXE As String
    Public VODAFONEpath As String
    Public VODAFONEEXE As String
    Public AIREDGEpath As String
    Public AIREDGEEXE As String
    Public FlashPlayerpath As String
    Public FlashPlayerEXE As String
    Public MachiCharapath As String
    Public MachiCharaExe As String
    Public CharaDenpath As String
    Public CharaDenExe As String
    Public Java1_8BinFolderPath As String

    ' FORM Close/Load
    Private Sub MainForm_Closing(sender As Object, e As EventArgs) Handles MyBase.FormClosing, MyBase.Closing
        UtilManager.CheckAndCloseAllEmulators()
        UtilManager.CheckAndCloseAMX()
        UtilManager.CheckAndCloseAHK()
    End Sub
    Private Async Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Opacity = 0
        Application.EnableVisualStyles()

        'Set Labels
        SetupLabelsinOptions()

        'Summon Splash
        SplashScreen.ShowSplash()

        ' Setup SJIS 
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)

        ' Adjust Form
        AdjustFormPadding()

        ' Check if Debug
#If DEBUG Then
        isDebug = True
#End If


        ' Setup DIRs
        Await UtilManager.SetupDIRSAsync()

        ' Setup Logging
        ' Check log File Size and Cleanup
        Await UtilManager.DeleteLogIfTooLargeAsync(Path.Join(LogFolder, "app_log.txt"))
        Logger.InitializeLogger()
        Logger.LogInfo("Application started.")

        ' Load Config
        config = Await configManager.LoadConfigAsync()

        ' Access and Assign Config settings
        AppLoadManager.LoadConfigValues(config)

        ' Get NetworkUIDConfig
        AppLoadManager.LoadNetworkUID()

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
            MessageBox.Show("Invalid FirstStart parameter in AppConfig.xml, Please set to true and relaunch app.")
            MainForm.QuitApplication()
        End If

        'Needs Internet If none we skip and use local file
        Dim uri As New Uri(versionCheckUrl)
        Dim domainOnly As String = uri.Scheme & "://" & uri.Host
        Logger.LogInfo("Checking internet connectivity...")
        If Await UtilManager.IsInternetAvailableAsync("https://google.com") Then
            'Set the User to online
            isOnline = True

            ' Check for App update
            Logger.LogInfo("Getting App Update")
            If autoUpdate = True Then
                Await UpdateManager.CheckAndLaunchUpdaterAsync(versionCheckUrl, SplashScreen)
            End If

            ' Get Updated Game List  
            Logger.LogInfo("Getting gamelist.xml")
            If autoUpdateGameList = True Then
                Await GameListManager.DownloadGameListAsync(gameListUrl)
            End If

            ' Get Updated MachiChara List  
            Logger.LogInfo("Getting machicharalist.xml")
            If autoUpdatemachicharaList = True Then
                Await MachiCharaListManager.DownloadMachiCharaListAsync(machicharaListUrl)
            End If

            ' Get Updated Charaden List  
            Logger.LogInfo("Getting charadenlist.xml")
            If autoUpdatemachicharaList = True Then
                Await CharaDenListManager.DownloadCharaDenListAsync(charadenListUrl)
            End If

            ' Send Launch Stats
            UtilManager.SendKWLLaunchStats()
        Else
            isOnline = False
            Me.Text = "Keitai World Launcher - Offline"
            Logger.LogWarning($"No internet connection to Domain {domainOnly}. Skipping online checks.")
        End If

        'We need to check for java every run, since these get used for all emu's
        Dim javaReady = Await UtilManager.EnsureJava1_8IsConfiguredAsync()
        If Not javaReady Then
            MainForm.QuitApplication()
            Return
        End If
        Logger.LogInfo($"Using JDK: {UsingJDK1_8}")

        ' Load Custom Games
        Await LoadCustomGamesAsync()

        ' Load Game List
        Await LoadGameListFirstTimeAsync()

        ' Load MachiChara List
        Await LoadMachiCharaListFirsTimeASync()

        ' Load Charaden List
        Await LoadCharaDenListFirsTimeASync()

        'Last Step
        Await GetSDKsAsync()

        ' Setup any Config Suff
        chkbxHidePhoneUI.Checked = DOJAHideUI
        Dim atindex As Integer = cbxAudioType.FindStringExact(DOJASoundType)
        cbxAudioType.SelectedIndex = atindex
        chkbxShaderGlass.Checked = UseShaderGlass
        chkbxDialpadNumpad.Checked = UseDialPad
        cbxFilterType.SelectedIndex = 0
        cbxShaderGlassScaling.SelectedIndex = 2
        cbxSJMELaunchOption.SelectedIndex = 0
        cbxSJMEScaling.SelectedIndex = 1

        ' Set we Completed Boot Sequence
        CompletedBootSequence = True

        ' Close the splash screen
        Await SplashScreen.CloseSplashAsync()
        Me.Opacity = 1
    End Sub

    ' General Other Function
    Private Sub EnableButtons(SelectedGame As Game)
        ' Enable game launch button and checkbox
        btnLaunchGame.Enabled = True
        chkbxHidePhoneUI.Enabled = True
        cbxAudioType.Enabled = True
        chkbxShaderGlass.Enabled = True
        cbxShaderGlassScaling.Enabled = True
        cbxDojaSDK.Enabled = True
        cbxStarSDK.Enabled = True
        chkbxLocalEmulator.Enabled = True
        chkbxEnableController.Enabled = True
        chkbxDialpadNumpad.Enabled = True

        ' Check if we can support LocaleEmu
        Select Case SelectedGame.Emulator.ToLower()
            Case "doja", "star"
                chkbxLocalEmulator.Enabled = True

            Case "jsky", "flash"
                If chkbxLocalEmulator.Enabled AndAlso chkbxLocalEmulator.Checked Then
                    chkbxLocalEmulator.Enabled = False
                End If

                If chkboxControllerVibration.Enabled AndAlso chkboxControllerVibration.Checked Then
                    chkboxControllerVibration.Enabled = False
                End If
        End Select

    End Sub
    Private Async Function GetSDKsAsync() As Task
        Dim dojaDefault As String = "iDKDoJa5.1"
        Dim starDefault As String = "iDKStar2.0"
        Dim jskyDefault As String = "JSky_0.1.5B"
        Dim softbankDefault As String = "kemnnx64"
        Dim vodafoneDefault As String = "kemnnx64"
        Dim airedgeDefault As String = "kemnnx64"
        Dim FlashDefault As String = "FlashPlayer"
        Dim dojaFound As Boolean = False
        Dim starFound As Boolean = False
        Dim softbankFound As Boolean = False
        Dim jskyFound As Boolean = False
        Dim airedgeFound As Boolean = False
        Dim vodafoneFound As Boolean = False
        Dim flashFound As Boolean = False

        ' Run the directory check on a background thread
        Dim sdkFolders As String() = Await Task.Run(Function() Directory.GetDirectories(ToolsFolder))

        ' Clear existing items on the UI thread
        cbxDojaSDK.Items.Clear()
        cbxStarSDK.Items.Clear()
        cbxJSKYSDK.Items.Clear()
        cbxVodafoneSDK.Items.Clear()
        cbxFlashSDK.Items.Clear()
        cbxAirEdgeSDK.Items.Clear()
        cbxSoftbankSDK.Items.Clear()

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
            ElseIf folder.StartsWith("squirreljme", StringComparison.OrdinalIgnoreCase) Then
                cbxDojaSDK.Items.Add(folder)
                If folder.Equals(dojaDefault, StringComparison.OrdinalIgnoreCase) Then
                    dojaFound = True
                End If
            ElseIf folder.StartsWith("JSky_", StringComparison.OrdinalIgnoreCase) Then
                cbxJSKYSDK.Items.Add(folder)
                If folder.Equals(jskyDefault, StringComparison.OrdinalIgnoreCase) Then
                    jskyFound = True
                End If
            ElseIf folder.StartsWith("kemnnx64", StringComparison.OrdinalIgnoreCase) Then 'Kemulator handles more then just 1 type
                cbxJSKYSDK.Items.Add(folder)
                cbxDojaSDK.Items.Add(folder)
                cbxAirEdgeSDK.Items.Add(folder)
                cbxSoftbankSDK.Items.Add(folder)
                cbxVodafoneSDK.Items.Add(folder)
                If folder.Equals(airedgeDefault, StringComparison.OrdinalIgnoreCase) Then
                    airedgeFound = True
                End If
                If folder.Equals(softbankDefault, StringComparison.OrdinalIgnoreCase) Then
                    softbankFound = True
                End If
                If folder.Equals(vodafoneDefault, StringComparison.OrdinalIgnoreCase) Then
                    vodafoneFound = True
                End If
            ElseIf folder.StartsWith("Flash", StringComparison.OrdinalIgnoreCase) Then
                cbxFlashSDK.Items.Add(folder)
                If folder.Equals(FlashDefault, StringComparison.OrdinalIgnoreCase) Then
                    flashFound = True
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

        If jskyFound Then
            cbxJSKYSDK.SelectedItem = jskyDefault
        Else
            MessageBox.Show(owner:=SplashScreen, $"The default SDK '{jskyDefault}' was not found. Please download and set it up.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If

        If softbankFound Then
            cbxSoftbankSDK.SelectedItem = softbankDefault
        Else
            MessageBox.Show(owner:=SplashScreen, $"The default SDK '{softbankDefault}' was not found. Please download and set it up.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If

        If vodafoneFound Then
            cbxVodafoneSDK.SelectedItem = vodafoneDefault
        Else
            MessageBox.Show(owner:=SplashScreen, $"The default SDK '{vodafoneDefault}' was not found. Please download and set it up.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If

        If airedgeFound Then
            cbxAirEdgeSDK.SelectedItem = airedgeDefault
        Else
            MessageBox.Show(owner:=SplashScreen, $"The default SDK '{airedgeDefault}' was not found. Please download and set it up.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If

        If flashFound Then
            cbxFlashSDK.SelectedItem = FlashDefault
        Else
            MessageBox.Show(owner:=SplashScreen, $"The default SDK '{FlashDefault}' was not found. Please download and set it up.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Function
    Private Async Function LoadGameIconsAsync() As Task
        Dim DojaIconPath As String = Path.Combine(ToolsFolder, "icons", "defaults", "doja.gif")
        Dim StarIconPath As String = Path.Combine(ToolsFolder, "icons", "defaults", "star.gif")
        Dim JskyIconPath As String = Path.Combine(ToolsFolder, "icons", "defaults", "jsky.gif")
        Dim SoftBankIconPath As String = Path.Combine(ToolsFolder, "icons", "defaults", "softbank.gif")
        Dim vodafoneIconPath As String = Path.Combine(ToolsFolder, "icons", "defaults", "vodafone.gif")
        Dim airedgeIconPath As String = Path.Combine(ToolsFolder, "icons", "defaults", "airedge.gif")
        Dim FlashIconPath As String = Path.Combine(ToolsFolder, "icons", "defaults", "flash.gif")

        ' Validate default icons
        If Not File.Exists(DojaIconPath) OrElse Not File.Exists(StarIconPath) OrElse Not File.Exists(JskyIconPath) Then
            MessageBox.Show("Missing Doja/Star/Jsky/Flash Default icons.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
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
                                                                                       Dim defaultIconPath As String

                                                                                       Select Case game.Emulator.ToLower()
                                                                                           Case "doja"
                                                                                               defaultIconPath = DojaIconPath
                                                                                           Case "star"
                                                                                               defaultIconPath = StarIconPath
                                                                                           Case "jsky"
                                                                                               defaultIconPath = JskyIconPath
                                                                                           Case "softbank"
                                                                                               defaultIconPath = SoftBankIconPath
                                                                                           Case "vodafone"
                                                                                               defaultIconPath = vodafoneIconPath
                                                                                           Case "airedge"
                                                                                               defaultIconPath = airedgeIconPath
                                                                                           Case "flash"
                                                                                               defaultIconPath = FlashIconPath
                                                                                           Case Else
                                                                                               defaultIconPath = DojaIconPath ' fallback
                                                                                       End Select

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

            ' Sort games A-Z by ENTitle
            games = games.OrderBy(Function(g)
                                      If g.ENTitle.StartsWith("[") Then
                                          Return "ZZZZZZZZZ" & g.ENTitle ' Push to bottom
                                      Else
                                          Return g.ENTitle
                                      End If
                                  End Function).ToList()

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
                Return True
            End If
        Catch ex As Exception
            Logger.LogError("Failed to Load MachiChara List", ex)
            Return False
        End Try
    End Function
    Private Async Function LoadCharaDenListFirsTimeASync() As Task(Of Boolean)
        Logger.LogInfo("Processing charadenlist.xml")
        ListViewCharaDen.View = View.Details
        ListViewCharaDen.FullRowSelect = True
        ListViewCharaDen.Columns.Clear()
        ListViewCharaDen.Columns.Add("Title", ListViewCharaDen.ClientSize.Width - SystemInformation.VerticalScrollBarWidth, HorizontalAlignment.Left)
        Try
            charadens = charadenListManager.LoadCharaden()
            If charadens IsNot Nothing Then
                ListViewCharaDen.Items.Clear()
                For Each cd In charadens
                    Dim item As New ListViewItem(cd.ENTitle)
                    item.Tag = cd ' Store the full object for later access
                    ListViewCharaDen.Items.Add(item)

                    'Check if Downloaded already
                    Dim AFDPath = Path.Combine(DownloadsFolder, cd.AFDName)
                    If File.Exists(AFDPath) Then
                        item.BackColor = Color.LightGreen
                    End If
                Next
                lblCharadenTotalCount.Text = $"Total: {ListViewCharaDen.Items.Count}"
                Return True
            End If
        Catch ex As Exception
            Logger.LogError("Failed to Load MachiChara List", ex)
            Return False
        End Try
    End Function
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
                                                                               File.ReadAllLines(CustomGamesTxtFile) _
                                                                                   .Select(Function(line) line.Trim()) _
                                                                                   .Where(Function(line) Not String.IsNullOrWhiteSpace(line)) _
                                                                                   .Select(Function(line) line.Split("|"c)(0).Trim()) _
                                                                                   .ToHashSet(StringComparer.OrdinalIgnoreCase),
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
                                                                            Dim isFanTranslation As Boolean = gameTitle.IndexOf("Patch", StringComparison.OrdinalIgnoreCase) >= 0

                                                                            Dim matchesFilter As Boolean = selectedFilter = "all" OrElse
                                                                               (selectedFilter = "favorites" AndAlso isFavorited) OrElse
                                                                               (selectedFilter = "custom" AndAlso isCustom) OrElse
                                                                               (selectedFilter = "installed" AndAlso isInstalled) OrElse
                                                                               (selectedFilter = "fan-translations" AndAlso isFanTranslation) OrElse
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
    End Function
    Private Async Function LoadGameVariantsAsync() As Task
        ' Ensure the ListView view mode is set to Details
        ListViewGamesVariants.View = View.Details
        ListViewGamesVariants.Items.Clear()
        ListViewGamesVariants.Columns.Clear()
        ListViewGamesVariants.Columns.Add("Appli Variants", ListViewGamesVariants.ClientSize.Width - SystemInformation.VerticalScrollBarWidth, HorizontalAlignment.Left)
        ListViewGamesVariants.BackColor = SystemColors.Window

        ' Ensure a game is selected
        If ListViewGames.SelectedItems.Count = 0 Then
            lblTotalVariantCount.Text = "Variants: 0"
            Return
        End If

        ' Get selected game title
        Dim selectedGameTitle As String = ListViewGames.SelectedItems(0).Text

        ' Find the game on a background thread
        Dim selectedGame As Game = Await Task.Run(Function()
                                                      Return games.FirstOrDefault(Function(g) g.ENTitle = selectedGameTitle)
                                                  End Function)

        If selectedGame Is Nothing OrElse String.IsNullOrWhiteSpace(selectedGame.Variants) Then
            lblTotalVariantCount.Text = "Variants: 0"
            Return
        End If

        ' Split the variants string safely
        Dim variants As String() = selectedGame.Variants.Split(","c)

        ' Add to ListView on the UI thread
        For Each v As String In variants
            ListViewGamesVariants.Items.Add(New ListViewItem(v.Trim()))
        Next

        If ListViewGamesVariants.Items.Count = 1 Then
            ListViewGamesVariants.Items(0).Selected = True
        End If

        lblTotalVariantCount.Text = $"Variants: {ListViewGamesVariants.Items.Count}"
    End Function
    Private Async Function DownloadGames(ContextDownload As Boolean) As Task
        ' Ensure a game is selected
        If ListViewGames.SelectedItems.Count = 0 Then
            MessageBox.Show("Please select a game", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Get the selected game
        Dim selectedGameTitle As String = ListViewGames.SelectedItems(0).Text
        Dim selectedGame As Game = games.FirstOrDefault(Function(g) g.ENTitle = selectedGameTitle)

        If selectedGame Is Nothing Then
            MessageBox.Show("Selected game could not be found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' Get the selected variant (if any)
        Dim selectedGameVariant As String = String.Empty
        If ListViewGamesVariants.SelectedItems.Count > 0 Then
            selectedGameVariant = ListViewGamesVariants.SelectedItems(0).Text.Trim()
        End If

        ' Get emulator type
        Dim emulator As String = selectedGame.Emulator

        ' Construct paths for game files
        ' Build shared paths
        Dim zipNameNoExt As String = Path.GetFileNameWithoutExtension(selectedGame.ZIPName)
        Dim gameBasePath As String = Path.Combine(DownloadsFolder, zipNameNoExt)
        Dim gameVariantsRaw As String = selectedGame.Variants
        Dim gameVariantsSplit() As String = If(Not String.IsNullOrEmpty(gameVariantsRaw), gameVariantsRaw.Split(","c), {})
        Dim downloadFileZipPath As String = $"{DownloadsFolder}\{selectedGame.ZIPName}"

        ' Determine fallback variant (if needed)
        Dim fallbackVariant As String = If(gameVariantsSplit.Length > 0, gameVariantsSplit(0), "")

        ' Select the correct variant path
        Dim variantPath As String = ""
        If String.IsNullOrWhiteSpace(gameVariantsRaw) Then
            variantPath = "" ' No variant structure
        ElseIf Not String.IsNullOrWhiteSpace(selectedGameVariant) Then
            variantPath = selectedGameVariant
        Else
            variantPath = fallbackVariant
        End If

        ' Set paths based on emulator
        If emulator = "doja" OrElse emulator = "star" Then
            CurrentSelectedGameJAM = Path.Combine(gameBasePath, variantPath, "bin", $"{zipNameNoExt}.jam")
            CurrentSelectedGameJAR = Path.Combine(gameBasePath, variantPath, "bin", $"{zipNameNoExt}.jar")
            CurrentSelectedGameSP = Path.Combine(gameBasePath, variantPath, "sp", $"{zipNameNoExt}.sp")

        ElseIf emulator = "jsky" Or emulator = "vodafone" Or emulator = "airedge" Or emulator = "softbank" Then
            If String.IsNullOrWhiteSpace(variantPath) Then
                CurrentSelectedGameJAM = Path.Combine(gameBasePath, $"{zipNameNoExt}.jad")
                CurrentSelectedGameJAR = Path.Combine(gameBasePath, $"{zipNameNoExt}.jar")
            Else
                CurrentSelectedGameJAM = Path.Combine(gameBasePath, variantPath, $"{zipNameNoExt}.jad")
                CurrentSelectedGameJAR = Path.Combine(gameBasePath, variantPath, $"{zipNameNoExt}.jar")
            End If
        ElseIf emulator = "flash" Then
            If String.IsNullOrWhiteSpace(variantPath) Then
                CurrentSelectedGameJAM = Path.Combine(gameBasePath, $"{zipNameNoExt}.swf")
                CurrentSelectedGameJAR = Path.Combine(gameBasePath, $"{zipNameNoExt}.swf")
            Else
                CurrentSelectedGameJAM = Path.Combine(gameBasePath, variantPath, $"{zipNameNoExt}.swf")
                CurrentSelectedGameJAR = Path.Combine(gameBasePath, variantPath, $"{zipNameNoExt}.swf")
            End If
        End If

        'Check if download is happening already.
        If isGameDownloadInProgress Then
            Logger.LogInfo("Skipping game file check — a download is already in progress.")
            Return
        End If

        ' Check if the game is already downloaded
        If isOnline = False Then
            Exit Function
        End If
        Logger.LogInfo($"Checking for {CurrentSelectedGameJAR}")
        If File.Exists(CurrentSelectedGameJAR) Then
            If ContextDownload Then
                Dim result As DialogResult = MessageBox.Show(
                    $"The game '{selectedGame.ENTitle}' is already downloaded. Would you like to download it again?{vbCrLf}{vbCrLf}" &
                    "This could delete your save data, we will attempt to backup you're save, so please be careful.",
                    "Download Game Again", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                Try
                    If result = DialogResult.Yes Then
                        Dim gameFolder As String = Path.Combine(DownloadsFolder, Path.GetFileNameWithoutExtension(selectedGame.ZIPName.Replace(".zip", "")))
                        Await SaveDataManager.BackupSaveAsync(gameFolder, selectedGame.Emulator)
                        isGameDownloadInProgress = True
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
                Catch ex As Exception
                    Logger.LogError($"[UI] Error during game download: {ex.Message}")
                    MessageBox.Show("An error occurred while downloading the game. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Finally
                    isGameDownloadInProgress = False
                End Try
            End If
            UtilManager.GenerateDynamicControlsFromLines(CurrentSelectedGameJAM, panelDynamic)
        Else
            If selectedGame.ZIPName = String.Empty OrElse selectedGame.ZIPName Is Nothing Then
                Logger.LogError($"{selectedGame.ENTitle} has invalid gamelist values, unable to download.")
                MessageBox.Show($"{selectedGame.ENTitle} has invalid gamelist values, unable to download.")
                Return
            End If

            ' Game not downloaded - prompt user to download it
            Dim result As DialogResult = MessageBox.Show(
                $"The game '{selectedGame.ENTitle} ({selectedGame.ZIPName})' is not downloaded. Would you like to download it?",
                "Download Game", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

            If result = DialogResult.Yes Then
                isGameDownloadInProgress = True
                Try
                    Logger.LogInfo($"Starting download for {selectedGame.DownloadURL}")
                    Await StartGameDownload(selectedGame, downloadFileZipPath, gameBasePath, CurrentSelectedGameJAM, CurrentSelectedGameJAR)

                    If File.Exists(CurrentSelectedGameJAM) Then
                        UtilManager.GenerateDynamicControlsFromLines(CurrentSelectedGameJAM, panelDynamic)
                        ListViewGames.SelectedItems(0).BackColor = Color.LightGreen
                    Else
                        Logger.LogError($"Download completed but JAM file not found at: {CurrentSelectedGameJAM}")
                    End If
                Catch ex As Exception
                    Logger.LogError($"[UI] Error during game download: {ex.Message}")
                    MessageBox.Show("An error occurred while downloading the game. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Finally
                    isGameDownloadInProgress = False
                End Try
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
    Private Async Sub DownloadCharaDen(selectedCharaDen As CharaDen)
        If selectedCharaDen IsNot Nothing Then
            Logger.LogInfo($"Checking for {DownloadsFolder}\{selectedCharaDen.AFDName}")
            CurrentSelectedCharaDenAFD = Path.Combine(DownloadsFolder, selectedCharaDen.AFDName)

            ' Check if the Chara-den is already downloaded
            Dim localFilePath As String = CurrentSelectedCharaDenAFD
            Dim downloadFilePath As String = Path.Combine(DownloadsFolder, selectedCharaDen.AFDName)

            If File.Exists(localFilePath) Then
                ' File already exists, nothing to do (or maybe inform the user)
            Else
                Dim result = MessageBox.Show($"The Chara-den '{selectedCharaDen.ENTitle} ({selectedCharaDen.AFDName})' is not downloaded. Would you like to download it?", "Download Chara-den", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                If result = DialogResult.Yes Then
                    Logger.LogInfo($"Starting Download for {selectedCharaDen.DownloadURL}")
                    Dim CharaDenDownloader As New CharaDenDownloader(pbGameDL)
                    Await CharaDenDownloader.DownloadCharaDenAsync(selectedCharaDen.DownloadURL, downloadFilePath, False)
                    HighlightCharaDen()
                End If
            End If
            btnCharaDenLaunch.Enabled = True
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
            Dim selectedGameTitle = item.Text
            Dim selectedGame = games.FirstOrDefault(Function(g) g.ENTitle = selectedGameTitle)
            If selectedGame IsNot Nothing Then
                gamesToDelete.Add(selectedGame)
            End If
        Next

        ' Display confirmation
        Dim gameList = String.Join(Environment.NewLine, gamesToDelete.Select(Function(g) $"{g.ENTitle} ({g.ZIPName})"))
        Dim result = MessageBox.Show(
            $"The following games will be deleted:{Environment.NewLine}{Environment.NewLine}{gameList}{Environment.NewLine}{Environment.NewLine}Do you want to proceed?",
            "Delete Games", MessageBoxButtons.YesNo, MessageBoxIcon.Question
        )
        If result <> DialogResult.Yes Then Return
        UtilManager.ShowLaunchOverlay(Me, "Deleting...")
        Dim deletedGames As New List(Of String)
        Dim failedGames As New List(Of String)

        ' === Run all deletions in background and truly await completion ===
        Await Task.Run(Async Function() As Task
                           For Each game In gamesToDelete
                               Try
                                   Dim baseName = Path.GetFileNameWithoutExtension(game.ZIPName)
                                   Dim gameFolder = Path.Combine(DownloadsFolder, baseName)

                                   If Directory.Exists(gameFolder) Then
                                       ' 1) Remove the downloaded folder
                                       My.Computer.FileSystem.DeleteDirectory(
                                            gameFolder,
                                            FileIO.UIOption.OnlyErrorDialogs,
                                            FileIO.RecycleOption.DeletePermanently
                                        )
                                       deletedGames.Add(game.ENTitle)

                                       ' 2) If it's an SD-card title, clean up the SD_BIND folder
                                       If Not String.IsNullOrEmpty(game.SDCardDataURL) Then
                                           For Each emu As String In cbxDojaSDK.Items
                                               Dim checkSD = Path.Combine(
                                                    ToolsFolder,
                                                    Path.GetFileName(emu),
                                                    "lib", "storagedevice", "ext0", "SD_BIND",
                                                    "SVC0000" & baseName & ".jam"
                                                )
                                               If Directory.Exists(checkSD) Then
                                                   My.Computer.FileSystem.DeleteDirectory(
                                                        checkSD,
                                                        FileIO.UIOption.OnlyErrorDialogs,
                                                        FileIO.RecycleOption.DeletePermanently
                                                    )
                                               End If
                                           Next
                                       End If

                                       ' 3) Remove its line from customgames.txt and notify your manager
                                       Dim configPath = CustomGamesTxtFile
                                       If File.Exists(configPath) Then
                                           Dim lines = File.ReadAllLines(configPath).ToList()
                                           Dim filtered = lines.Where(Function(line)
                                                                          Dim parts = line.Split("|"c)
                                                                          Return Not parts(0).Trim().
                                                              Equals(baseName, StringComparison.OrdinalIgnoreCase)
                                                                      End Function).ToList()

                                           If filtered.Count <> lines.Count Then
                                               File.WriteAllLines(configPath, filtered)
                                               Await gameListManager.RemoveGameAsync(baseName)
                                           End If
                                       End If
                                   Else
                                       failedGames.Add($"{game.ENTitle} (Not downloaded)")
                                   End If
                               Catch ex As Exception
                                   failedGames.Add($"{game.ENTitle} (Error: {ex.Message})")
                               End Try
                           Next
                       End Function)
        ' === End Task.Run ===

        ' Show results (back on the UI thread)
        If deletedGames.Count > 0 Then
            MessageBox.Show(
            $"Successfully deleted:{Environment.NewLine}{String.Join(Environment.NewLine, deletedGames)}",
            "Deletion Complete", MessageBoxButtons.OK, MessageBoxIcon.Information
        )
            Await FilterAndHighlightGamesAsync()
        End If

        If failedGames.Count > 0 Then
            MessageBox.Show(
            $"Could not delete the following games:{Environment.NewLine}{String.Join(Environment.NewLine, failedGames)}",
            "Deletion Errors", MessageBoxButtons.OK, MessageBoxIcon.Warning
        )
        End If
        UtilManager.HideLaunchOverlay()
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
    Public Async Function DeleteCharadenAsync() As Task
        If ListViewCharaDen.SelectedItems.Count = 0 Then
            MessageBox.Show("Please select at least one Chara-den to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' ❗ Safely copy selected items to a list on the UI thread
        Dim selectedCharaList As New List(Of CharaDen)
        For Each item As ListViewItem In ListViewCharaDen.SelectedItems
            Dim cd As CharaDen = CType(item.Tag, CharaDen)
            If cd IsNot Nothing Then selectedCharaList.Add(cd)
        Next

        Dim deletedFiles As New List(Of String)

        ' Run deletion logic in background
        Await Task.Run(Sub()
                           For Each selectedCharaden In selectedCharaList
                               Dim afdPath As String = Path.Combine(DownloadsFolder, selectedCharaden.AFDName)
                               If File.Exists(afdPath) Then
                                   Try
                                       File.Delete(afdPath)
                                       Logger.LogInfo($"Deleted Chara-den: {selectedCharaden.AFDName}")
                                       SyncLock deletedFiles
                                           deletedFiles.Add(selectedCharaden.AFDName)
                                       End SyncLock
                                   Catch ex As Exception
                                       Logger.LogError($"Failed to delete Chara-den: {selectedCharaden.AFDName}", ex)
                                   End Try
                               End If
                           Next
                       End Sub)

        ' UI-safe call
        HighlightCharaDen()

        If deletedFiles.Count > 0 Then
            MessageBox.Show("Deleted the following Chara-den:" & vbCrLf & String.Join(vbCrLf, deletedFiles),
                        "Chara-den Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information)
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
    Public Function VerifyEmulatorType_JAM(GameJAM As String) As String
        Try
            If String.IsNullOrWhiteSpace(GameJAM) OrElse Not File.Exists(GameJAM) Then
                Return "unknown"
            End If

            Dim jamLines As String() = File.ReadAllLines(GameJAM, Encoding.GetEncoding("shift-jis"))
            Dim appTypeLine As String = jamLines.FirstOrDefault(Function(line) line.TrimStart().StartsWith("AppType =", StringComparison.OrdinalIgnoreCase))

            If Not String.IsNullOrEmpty(appTypeLine) Then
                Return "star"
            Else
                Return "doja"
            End If
        Catch ex As Exception
            ' Log the error if needed
            Console.WriteLine($"Error reading JAM file: {ex.Message}")
            Return "unknown"
        End Try
    End Function
    Public Function VerifyEmulatorType_JAD(GameJAD As String) As String
        Try
            If String.IsNullOrWhiteSpace(GameJAD) OrElse Not File.Exists(GameJAD) Then
                Return "unknown"
            End If

            Dim OCLType = GetMIDletOCL(GameJAD)
            If OCLType.StartsWith("JSCL") Then
                Return "vodafone"
            Else
                Return "jsky"
            End If

        Catch ex As Exception
            ' Log the error if needed
            Console.WriteLine($"Error reading JAD file: {ex.Message}")
            Return "unknown"
        End Try
    End Function
    Public Function GetMIDletOCL(filePath As String) As String
        Const key As String = "MIDlet-OCL"
        Dim enc As Encoding = Encoding.GetEncoding(932)

        For Each line As String In File.ReadAllLines(filePath, enc)
            If line.StartsWith(key & ":", StringComparison.OrdinalIgnoreCase) Then
                Return line.Substring(key.Length + 1).Trim()
            End If
        Next
        Return String.Empty
    End Function
    Public Async Function LoadCustomGamesAsync() As Task
        Dim customGamesFile As String = CustomGamesTxtFile

        ' 1) Ensure the file exists
        If Not File.Exists(customGamesFile) Then
            Using stream = File.Create(customGamesFile)
            End Using
        End If

        ' 2) Read + dedupe (still on full lines: "appName | emulator")
        Dim allLines As String() = Await File.ReadAllLinesAsync(customGamesFile)
        Dim customGameLines As List(Of String) = allLines _
        .Select(Function(line) line.Trim()) _
        .Where(Function(line) Not String.IsNullOrWhiteSpace(line)) _
        .Distinct(StringComparer.OrdinalIgnoreCase) _
        .ToList()

        If customGameLines.Count <> allLines.Length Then
            Await File.WriteAllLinesAsync(customGamesFile, customGameLines)
        End If

        ' 3) Supported emulator set
        Dim supportedEmus = New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
        "doja", "star", "jsky", "airedge", "vodafone", "flash"
    }

        ' 4) Process each line
        For Each entry In customGameLines
            ' Split at '|' → [0] = appName, [1] = emulator (if present)
            Dim parts = entry.Split("|"c)
            Dim appName = parts(0).Trim()
            Dim emulatorValue As String = "doja"                                 ' default
            If parts.Length > 1 AndAlso Not String.IsNullOrWhiteSpace(parts(1)) Then
                Dim candidate = parts(1).Trim().ToLowerInvariant()
                If supportedEmus.Contains(candidate) Then
                    emulatorValue = candidate
                End If
            End If

            ' Only add if the folder actually exists
            Dim gameFolderPath = Path.Combine(DownloadsFolder, appName)
            If Directory.Exists(gameFolderPath) Then
                Dim game As New Game With {
                .ENTitle = appName,
                .ZIPName = appName & ".zip",
                .DownloadURL = "",
                .CustomAppIconURL = "",
                .SDCardDataURL = "",
                .Emulator = emulatorValue,
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

            ' Calculate adjusted top padding based on DPI
            Dim adjustedPadding As Integer
            If dpiScale = 1 Then
                adjustedPadding = 63
            ElseIf dpiScale = 1.25 Then
                adjustedPadding = 63
            ElseIf dpiScale = 1.5 Then
                adjustedPadding = 63
            ElseIf dpiScale = 1.75 Then
                adjustedPadding = 63
            ElseIf dpiScale = 2 Then
                adjustedPadding = 63
            End If

            ' Apply padding to the form
            Me.Padding = New Padding(0, adjustedPadding, 0, 3)

            ' Adjust TabControl position
            If MaterialTabControl1 IsNot Nothing Then
                MaterialTabControl1.Top = MaterialTabSelector1.Top + 35
                ' Now, adjust the form's client size so that MaterialTabControl1 has 3 pixels of space on the right and bottom.
                Dim newClientWidth As Integer = MaterialTabControl1.Left + MaterialTabControl1.Width - 8
                Dim newClientHeight As Integer = MaterialTabControl1.Top + MaterialTabControl1.Height - 30

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
    Public Sub HighlightCharaDen()
        For Each item As ListViewItem In ListViewCharaDen.Items
            Dim cd As CharaDen = CType(item.Tag, CharaDen)
            If cd IsNot Nothing Then
                Dim afdPath As String = Path.Combine(DownloadsFolder, cd.AFDName)
                If File.Exists(afdPath) Then
                    item.BackColor = Color.LightGreen
                Else
                    item.BackColor = Color.White ' Optional: reset if not found
                End If
            End If
        Next
    End Sub
    Public Function LoadConnectedControllers() As Boolean
        cbxGameControllers.Items.Clear()
        XInputDevices.Clear()

        ' Loop through 4 possible XInput devices (0 to 3)
        For i As Integer = 0 To 3
            Dim controller As New Controller(CType(i, UserIndex))
            If controller.IsConnected Then
                Dim capabilities = controller.GetCapabilities(DeviceQueryType.Gamepad)
                Dim controllerName As String = $"XInput Controller {i + 1}"
                cbxGameControllers.Items.Add(controllerName)
                XInputDevices(controllerName) = i ' Save the controller index
                Logger.LogInfo($"Detected controller: {controllerName}")
            End If
        Next

        If cbxGameControllers.Items.Count > 0 Then
            cbxGameControllers.SelectedIndex = 0
            cbxGameControllers.Enabled = True
            Return True
        Else
            MessageBox.Show("No XInput controllers detected... Please connect one and try again.")
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
    Private Sub LoadPlaytimesToListView()
        lvwPlaytimes.Items.Clear()
        lvwPlaytimes.Columns.Clear()
        lvwPlaytimes.Columns.Add("Appli Name", 750, HorizontalAlignment.Left)
        lvwPlaytimes.Columns.Add("Play Time", 200, HorizontalAlignment.Left)
        lvwPlaytimes.Columns.Add("Sessions", 100, HorizontalAlignment.Center)
        lvwPlaytimes.Columns(0).Width = CInt(lvwPlaytimes.Width * 0.7)  ' App Name: 70%
        lvwPlaytimes.Columns(1).Width = CInt(lvwPlaytimes.Width * 0.2)  ' Play Time: 20%
        lvwPlaytimes.Columns(2).Width = CInt(lvwPlaytimes.Width * 0.1)  ' Sessions: 10%
        lvwPlaytimes.Scrollable = True
        lvwPlaytimes.FullRowSelect = True
        lvwPlaytimes.View = View.Details
        Dim filePath As String = PlaytimesTxtFile
        If Not File.Exists(filePath) Then Exit Sub

        Dim totalTime As TimeSpan = TimeSpan.Zero
        Dim totalSessions As Integer = 0

        For Each line In File.ReadLines(filePath)
            Dim parts = line.Split("|"c)
            If parts.Length >= 2 Then
                Dim appName As String = parts(0)
                Dim playTime As TimeSpan = TimeSpan.Zero
                Dim sessionCount As Integer = If(parts.Length >= 3, Integer.Parse(parts(2)), 1)

                If TimeSpan.TryParse(parts(1), playTime) Then
                    totalTime = totalTime.Add(playTime)
                    totalSessions += sessionCount

                    Dim formattedTime As String = $"{Math.Floor(playTime.TotalHours)}h {playTime.Minutes}m {playTime.Seconds}s"

                    Dim item As New ListViewItem(appName)
                    item.SubItems.Add(formattedTime)
                    item.SubItems.Add(sessionCount.ToString())
                    lvwPlaytimes.Items.Add(item)
                End If
            End If
        Next
        Dim dividerItem As New ListViewItem(New String() {"", "", ""})
        dividerItem.BackColor = Color.LightGray
        lvwPlaytimes.Items.Add(dividerItem)
        ' Add total row
        Dim totalFormatted As String = $"{Math.Floor(totalTime.TotalHours)}h {totalTime.Minutes}m {totalTime.Seconds}s"
        Dim totalItem As New ListViewItem("TOTAL")
        totalItem.SubItems.Add(totalFormatted)
        totalItem.SubItems.Add(totalSessions.ToString())

        ' ✅ Standout styling
        totalItem.ForeColor = Color.White
        Dim totalBackColor As Color = ColorTranslator.FromHtml("#3f51b5")
        totalItem.BackColor = totalBackColor
        totalItem.Font = New Font(lvwPlaytimes.Font.FontFamily, lvwPlaytimes.Font.Size, FontStyle.Bold)

        lvwPlaytimes.Items.Add(totalItem)
    End Sub
    Private Sub ShowCopyableDialogBox(Title As String, Text As String, CopyableText As String)
        ' Create a new MaterialForm
        Dim aboutForm As New ReaLTaiizor.Forms.MaterialForm With {
        .Text = Title,
        .Size = New Size(600, 350),
        .StartPosition = FormStartPosition.CenterScreen,
        .Sizable = False,
        .FormBorderStyle = FormBorderStyle.FixedDialog,
        .MaximizeBox = False,
        .MinimizeBox = False
    }

        ' Padding constants
        Dim margin As Integer = 20
        Dim spacing As Integer = 10

        ' Create label for top description
        Dim lblTop As New Label With {
        .Text = Text,
        .Left = margin,
        .Top = margin + 60, ' Allow some room from MaterialForm header
        .Width = aboutForm.ClientSize.Width - (margin * 2),
        .Height = 50,
        .Font = New Font("Segoe UI", 10, FontStyle.Regular),
        .ForeColor = Color.Black
    }

        ' Create TextBox for copyable content
        Dim txtcopyable As New TextBox With {
        .Text = CopyableText,
        .Multiline = True,
        .ReadOnly = True,
        .Left = margin,
        .Top = lblTop.Bottom + spacing,
        .Width = aboutForm.ClientSize.Width - (margin * 2),
        .Height = 140,
        .Font = New Font("Segoe UI", 10, FontStyle.Regular),
        .ScrollBars = ScrollBars.Vertical
    }

        ' Select all on click for convenience
        AddHandler txtcopyable.Click, Sub() txtcopyable.SelectAll()

        ' Create Close button
        Dim btnClose As New MaterialButton With {
        .Text = "Close",
        .Width = 100,
        .Height = 36,
        .Left = (aboutForm.ClientSize.Width - 100) \ 2,
        .Top = txtcopyable.Bottom + spacing + 10,
        .HighEmphasis = True,
        .Type = MaterialButton.MaterialButtonType.Contained
    }
        AddHandler btnClose.Click, Sub() aboutForm.Close()

        ' Add controls in proper Z-order
        aboutForm.Controls.Add(lblTop)
        aboutForm.Controls.Add(txtcopyable)
        aboutForm.Controls.Add(btnClose)

        ' Show dialog
        aboutForm.ShowDialog()
    End Sub

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

        ' Get selected game title
        Dim selectedGameTitle As String = ListViewGames.SelectedItems(0).Text

        ' Find the game on a background thread
        Dim selectedGame As Game = Await Task.Run(Function()
                                                      Return games.FirstOrDefault(Function(g) g.ENTitle = selectedGameTitle)
                                                  End Function)

        ' Perform actions once after all selections are done
        Await LoadGameVariantsAsync()
        Await DownloadGames(False)
        EnableButtons(selectedGame)
    End Sub
    Private Async Sub ListViewGamesVariants_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListViewGamesVariants.SelectedIndexChanged
        Await DownloadGames(False)
    End Sub
    Private Sub lbxMachiCharaList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListViewMachiChara.SelectedIndexChanged
        If ListViewMachiChara.SelectedItems.Count = 0 Then Return
        Dim selectedItem = ListViewMachiChara.SelectedItems(0)
        Dim selectedMachiChara = CType(selectedItem.Tag, MachiChara)
        DownloadMachiChara(selectedMachiChara)
    End Sub
    Private Sub lbxCharadenList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListViewCharaDen.SelectedIndexChanged
        If ListViewCharaDen.SelectedItems.Count = 0 Then Return
        Dim selectedItem = ListViewCharaDen.SelectedItems(0)
        Dim selectedCharaDen = CType(selectedItem.Tag, CharaDen)
        DownloadCharaDen(selectedCharaDen)
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
            chkboxControllerVibration.Enabled = False
            chkboxControllerVibration.Checked = False
            Return
        End If

        Logger.LogInfo("Controller support enabled by user.")

        cbxControllerProfile.Items.Clear()
        cbxGameControllers.Items.Clear()

        If Not LoadConnectedControllers() Then
            Logger.LogInfo("No controllers detected. Disabling controller support.")
            chkbxEnableController.Checked = False
            chkboxControllerVibration.Checked = False
            chkboxControllerVibration.Enabled = False
            Return
        End If

        If Not LoadControllerProfiles() Then
            Logger.LogInfo("No controller profiles found. Disabling controller support.")
            chkbxEnableController.Checked = False
            chkboxControllerVibration.Checked = False
            chkboxControllerVibration.Enabled = False
            Return
        End If
        chkboxControllerVibration.Checked = True
        chkboxControllerVibration.Enabled = True
    End Sub
    Private Sub chkbxDialpadNumpad_CheckedChanged(sender As Object, e As EventArgs) Handles chkbxDialpadNumpad.CheckedChanged
        Dim AHKFolder = Path.Combine(ToolsFolder, "autohotkey")
        Dim AHKScript = Path.Combine(AHKFolder, "AutoHotkey32.ahk")
        Dim AHKExe = Path.Combine(AHKFolder, "AutoHotkey32.exe")

        If chkbxDialpadNumpad.Checked = True Then
            ' Create the AHK script if it doesn't exist
            If Not File.Exists(AHKScript) Then
                Dim scriptContent As String =
                "Numpad7::Send 1" & vbCrLf &
                "Numpad8::Send 2" & vbCrLf &
                "Numpad9::Send 3" & vbCrLf &
                "Numpad4::Send 4" & vbCrLf &
                "Numpad5::Send 5" & vbCrLf &
                "Numpad6::Send 6" & vbCrLf &
                "Numpad1::Send 7" & vbCrLf &
                "Numpad2::Send 8" & vbCrLf &
                "Numpad3::Send 9" & vbCrLf &
                "Numpad0::Send 0"
                Directory.CreateDirectory(AHKFolder) ' Ensure the folder exists
                File.WriteAllText(AHKScript, scriptContent)
            End If

            ' Start the AHK script
            Try
                Dim ahkProc As New Process()
                ahkProc.StartInfo.FileName = AHKExe
                ahkProc.StartInfo.Arguments = """" & AHKScript & """"
                ahkProc.StartInfo.UseShellExecute = False
                ahkProc.StartInfo.CreateNoWindow = True
                ahkProc.Start()
            Catch ex As Exception
                MessageBox.Show("Failed to start AutoHotkey: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Else
            ' Kill any running AHK related processes
            UtilManager.CheckAndCloseAHK()
        End If
        configManager.UpdateUseDialPadSetting(chkbxDialpadNumpad.Checked)
    End Sub

    ' ComboBox Changes
    Private Sub cbxAudioType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxAudioType.SelectedIndexChanged
        If cbxAudioType.SelectedItem.ToString = "Standard" Or cbxAudioType.SelectedItem.ToString = "903i" Then
            configManager.UpdateDOJASoundSetting(cbxAudioType.SelectedItem.ToString)
            If cbxAudioType.SelectedItem.ToString = "903i" Then
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
        Dim selectedStarSDK = cbxStarSDK.SelectedItem.ToString
        STARpath = Path.Combine(ToolsFolder, selectedStarSDK)
        STAREXE = Path.Combine(ToolsFolder, selectedStarSDK, "bin", "star.exe")
    End Sub
    Private Sub cbxDojaSDK_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxDojaSDK.SelectedIndexChanged
        If cbxDojaSDK.SelectedItem Is Nothing Then
            MessageBox.Show("Please select a Doja SDK before launching.")
            Return
        End If

        Dim selectedSDK As String = cbxDojaSDK.SelectedItem.ToString()
        Dim sdkLower As String = selectedSDK.ToLowerInvariant()
        DOJApath = Path.Combine(ToolsFolder, selectedSDK)

        ' Default: Disable SJME options unless enabled later
        gbxSJMELaunchOptions.Enabled = False

        If sdkLower.StartsWith("idkdoja") Then
            DOJAEXE = Path.Combine(DOJApath, "bin", "doja.exe")

            If selectedSDK.Contains("3.5") AndAlso chkbxShaderGlass.Checked Then
                MessageBox.Show("Doja 3.5 does not work with ShaderGlass. Disabling it.")
                chkbxShaderGlass.Checked = False
            End If

        ElseIf sdkLower.StartsWith("squirreljme") Then
            If CompletedBootSequence = True Then
                MessageBox.Show(
                "DISCLAIMER: SquirrelJME support is still in active development." & vbCrLf &
                "You may encounter unexpected errors, UI glitches, or slowdowns" & vbCrLf &
                "when launching or interacting with the emulator. Use at your own risk."
                )
            End If
            DOJAEXE = Path.Combine(DOJApath, "squirreljme.exe")
            gbxSJMELaunchOptions.Enabled = True

        ElseIf sdkLower.StartsWith("kemnnx64") Then
            If CompletedBootSequence = True Then
                Logger.LogWarning("[kemulator] Disclaimer: This feature is in development and may exhibit issues or performance slowness.")
                MessageBox.Show(
                    "DISCLAIMER: Kemulator support is still in active development." & vbCrLf &
                    "You may encounter unexpected errors, UI glitches, or slowdowns" & vbCrLf &
                    "when launching or interacting with the emulator. Use at your own risk."
                    )
            End If
            DOJAEXE = Path.Combine(DOJApath, "KEmulator.jar")
        End If
    End Sub
    Private Sub cbxSoftbankSDK_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxSoftbankSDK.SelectedIndexChanged
        If cbxSoftbankSDK.SelectedItem Is Nothing Then
            MessageBox.Show("Please select a SOFTBANK SDK before launching.")
            Return
        End If

        Dim selectedSDK As String = cbxSoftbankSDK.SelectedItem.ToString()
        Dim sdkLower As String = selectedSDK.ToLowerInvariant()
        SOFTBANKpath = Path.Combine(ToolsFolder, selectedSDK)

        If sdkLower.StartsWith("kemnnx64") Then
            If CompletedBootSequence = True Then
                Logger.LogWarning("[kemulator] Disclaimer: This feature is in development and may exhibit issues or performance slowness.")
                MessageBox.Show(
                    "DISCLAIMER: Kemulator support is still in active development." & vbCrLf &
                    "You may encounter unexpected errors, UI glitches, or slowdowns" & vbCrLf &
                    "when launching or interacting with the emulator. Use at your own risk."
                    )
            End If
            SOFTBANKEXE = Path.Combine(SOFTBANKpath, "KEmulator.jar")
        End If
    End Sub
    Private Sub cbxJSKYSDK_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxJSKYSDK.SelectedIndexChanged
        If cbxJSKYSDK.SelectedItem Is Nothing Then
            MessageBox.Show("Please select a JSKY SDK before launching.")
            Return
        End If

        Dim selectedSDK As String = cbxJSKYSDK.SelectedItem.ToString()
        Dim sdkLower As String = selectedSDK.ToLowerInvariant()
        JSKYpath = Path.Combine(ToolsFolder, selectedSDK)

        If sdkLower.StartsWith("jsky_") Then
            JSKYEXE = Path.Combine(JSKYpath, "jbmidp.jar")

        ElseIf sdkLower.StartsWith("kemnnx64") Then
            If CompletedBootSequence = True Then
                Logger.LogWarning("[kemulator] Disclaimer: This feature is in development and may exhibit issues or performance slowness.")
                MessageBox.Show(
                    "DISCLAIMER: Kemulator support is still in active development." & vbCrLf &
                    "You may encounter unexpected errors, UI glitches, or slowdowns" & vbCrLf &
                    "when launching or interacting with the emulator. Use at your own risk."
                    )
            End If
            JSKYEXE = Path.Combine(JSKYpath, "KEmulator.jar")
        End If
    End Sub
    Private Sub cbxVODAFONESDK_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxVodafoneSDK.SelectedIndexChanged
        If cbxVodafoneSDK.SelectedItem Is Nothing Then
            MessageBox.Show("Please select a SOFTBANK SDK before launching.")
            Return
        End If

        Dim selectedSDK As String = cbxVodafoneSDK.SelectedItem.ToString()
        Dim sdkLower As String = selectedSDK.ToLowerInvariant()
        VODAFONEpath = Path.Combine(ToolsFolder, selectedSDK)

        If sdkLower.StartsWith("kemnnx64") Then
            If CompletedBootSequence = True Then
                Logger.LogWarning("[kemulator] Disclaimer: This feature is in development and may exhibit issues or performance slowness.")
                MessageBox.Show(
                "DISCLAIMER: Kemulator support is still in active development." & vbCrLf &
                "You may encounter unexpected errors, UI glitches, or slowdowns" & vbCrLf &
                "when launching or interacting with the emulator. Use at your own risk."
                )
            End If
            VODAFONEEXE = Path.Combine(VODAFONEpath, "KEmulator.jar")
        End If
    End Sub
    Private Async Sub cbxGameControllers_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxGameControllers.SelectedIndexChanged
        Dim selectedName = cbxGameControllers.SelectedItem?.ToString()

        If Not String.IsNullOrEmpty(selectedName) AndAlso XInputDevices.ContainsKey(selectedName) Then
            Dim controllerIndex As Integer = XInputDevices(selectedName)
            Dim controller As New Controller(CType(controllerIndex, UserIndex))

            If controller.IsConnected Then
                ' Set vibration - values range from 0 to 65535
                Dim vibration As New Vibration() With {
                .LeftMotorSpeed = 65535,
                .RightMotorSpeed = 65535
            }

                controller.SetVibration(vibration)

                ' Wait 250ms before turning vibration off
                Await Task.Delay(250)

                controller.SetVibration(New Vibration())
            Else
                MessageBox.Show("Controller is not connected.")
            End If
        Else
            MessageBox.Show("Selected controller does not support XInput vibration.")
        End If
    End Sub
    Private Sub cbxSJMELaunchOption_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxSJMELaunchOption.SelectedIndexChanged
        Dim selectedSJMELaunchOption = cbxSJMELaunchOption.SelectedItem.ToString
        If selectedSJMELaunchOption = "SpringCoat" Then
            lblSJMELaunchOptionsText.Text = "Optimized for compatibility, slower but more reliable."
        ElseIf selectedSJMELaunchOption = "Hosted" Then
            lblSJMELaunchOptionsText.Text = "Uses the system JVM, fastest but disregards any compatibility."
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

        'To show custom game tools we create when needed
        If selectedGameTitle.ToLower.Contains("bomberman puzzle special") Then
            If cmsGameLV_Download.Text = "Redownload" Then
                cmsBombermanPuzzle.Visible = True
            Else
                cmsBombermanPuzzle.Visible = False
            End If
        Else
            cmsBombermanPuzzle.Visible = False
        End If
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
    Private Async Sub BackupSaveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BackupSaveToolStripMenuItem.Click
        If ListViewGames.SelectedItems.Count = 0 Then Return
        Dim selectedGameTitle As String = ListViewGames.SelectedItems(0).Text
        Dim selectedGame As Game = games.FirstOrDefault(Function(g) g.ENTitle = selectedGameTitle)
        Dim gameFolder As String = Path.Combine(DownloadsFolder, Path.GetFileNameWithoutExtension(selectedGame.ZIPName.Replace(".zip", "")))
        If Directory.Exists(gameFolder) = True Then
            Await SaveDataManager.BackupSaveAsync(gameFolder, selectedGame.Emulator)
        End If
    End Sub
    'Game Specific Context Menu Options
    Private Async Sub tsmBPSImportStage_Click(sender As Object, e As EventArgs) Handles tsmBPSImportStage1.Click, tsmBPSImportStage2.Click, tsmBPSImportStage3.Click, tsmBPSImportStage4.Click, tsmBPSImportStage5.Click, tsmBPSImportStage6.Click, tsmBPSImportStage7.Click, tsmBPSImportStage8.Click, tsmBPSImportStage9.Click, tsmBPSImportStage10.Click
        Dim clickedItem As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        Dim stageNumber As Integer = Integer.Parse(clickedItem.Name.Replace("tsmBPSImportStage", ""))
        If ListViewGames.SelectedItems.Count = 0 Then Return
        Dim selectedGameTitle As String = ListViewGames.SelectedItems(0).Text
        Dim selectedGame As Game = games.FirstOrDefault(Function(g) g.ENTitle = selectedGameTitle)
        Dim gameFolder As String = Path.Combine(DownloadsFolder, Path.GetFileNameWithoutExtension(selectedGame.ZIPName.Replace(".zip", "")))
        Dim gameName As String = selectedGame.ZIPName.Replace(".zip", "")
        'Variant Selection
        If ListViewGamesVariants.SelectedItems.Count = 0 Then Return
        Dim selectedGameVariant As String = ListViewGamesVariants.SelectedItems(0).Text
        Dim SPFilepath = Path.Combine(gameFolder, selectedGameVariant, "sp", gameName & ".sp")
        Dim StageCode = InputBox("Import Stage Code", "Enter the stage code you want to import:", "")
        If String.IsNullOrEmpty(StageCode) = False Then
            Await gameManager.BomberManPuzzleSpecial_ImportStage(SPFilepath, stageNumber, StageCode)
            MessageBox.Show("Import Stage Success!")
        End If
    End Sub
    Private Async Sub tsmBPSExportStage_Click(sender As Object, e As EventArgs) Handles tsmBPSExportStage1.Click, tsmBPSExportStage2.Click, tsmBPSExportStage3.Click, tsmBPSExportStage4.Click, tsmBPSExportStage5.Click, tsmBPSExportStage6.Click, tsmBPSExportStage7.Click, tsmBPSExportStage8.Click, tsmBPSExportStage9.Click, tsmBPSExportStage10.Click
        Dim clickedItem As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        Dim stageNumber As Integer = Integer.Parse(clickedItem.Name.Replace("tsmBPSExportStage", ""))
        If ListViewGames.SelectedItems.Count = 0 Then Return
        Dim selectedGameTitle As String = ListViewGames.SelectedItems(0).Text
        Dim selectedGame As Game = games.FirstOrDefault(Function(g) g.ENTitle = selectedGameTitle)
        Dim gameFolder As String = Path.Combine(DownloadsFolder, Path.GetFileNameWithoutExtension(selectedGame.ZIPName.Replace(".zip", "")))
        Dim gameName As String = selectedGame.ZIPName.Replace(".zip", "")
        'Variant Selection
        If ListViewGamesVariants.SelectedItems.Count = 0 Then Return
        Dim selectedGameVariant As String = ListViewGamesVariants.SelectedItems(0).Text

        Dim SPFilepath = Path.Combine(gameFolder, selectedGameVariant, "sp", gameName & ".sp")
        Dim CompressedString = Await gameManager.BomberManPuzzleSpecial_ExportStage(SPFilepath, stageNumber)
        ShowCopyableDialogBox($"Exported Stage {stageNumber.ToString}", $"Here's your stage code — share it so your friends can play your level!", CompressedString)
    End Sub
    'MachiChara CMS
    Private Sub DownloadCMS_MachiChara_Click(sender As Object, e As EventArgs) Handles DownloadCMS_MachiChara.Click
        If ListViewMachiChara.SelectedItems.Count = 0 Then Return
        Dim selectedItem = ListViewMachiChara.SelectedItems(0)
        Dim selectedMachiChara = CType(selectedItem.Tag, MachiChara)
        DownloadMachiChara(selectedMachiChara)
    End Sub
    Private Async Sub DeleteCMS_MachiChara_Click(sender As Object, e As EventArgs) Handles DeleteCMS_MachiChara.Click
        Await DeleteMachiCharaAsync()
    End Sub
    'Charaden CMS
    Private Sub DownloadCMS_CharaDen_Click(sender As Object, e As EventArgs) Handles DownloadCMS_CharaDen.Click
        If ListViewCharaDen.SelectedItems.Count = 0 Then Return
        Dim selectedItem = ListViewCharaDen.SelectedItems(0)
        Dim selectedCharaden = CType(selectedItem.Tag, CharaDen)
        DownloadCharaDen(selectedCharaden)
    End Sub
    Private Async Sub DeleteCMS_CharaDen_Click(sender As Object, e As EventArgs) Handles DeleteCMS_CharaDen.Click
        Await DeleteCharadenAsync()
    End Sub
    'Launch Apps Buttons
    Private Async Sub btnLaunchGame_Click(sender As Object, e As EventArgs) Handles btnLaunchGame.Click, ListViewGames.DoubleClick, cmsGameLV_Launch.Click
        Try
            Logger.LogInfo("Attempting To launch game...")
            If chkbxEnableController.Checked Then
                utilManager.StopVibratorBmpMonitor()
            End If

            ' Ensure a game is selected
            If ListViewGames.SelectedItems.Count = 0 Then
                Logger.LogWarning("Launch failed: No game selected.")
                MessageBox.Show("Please select a game before launching.")
                Return
            End If

            ' Ensure CBX SDKs are selected
            ' Define a dictionary to hold the selected SDKs and their keys
            Dim sdkComboBoxes As New Dictionary(Of ComboBox, String) From {
                {cbxDojaSDK, "Doja"},
                {cbxStarSDK, "Star"},
                {cbxJSKYSDK, "JSKY"},
                {cbxSoftbankSDK, "SOFTBANK"},
                {cbxAirEdgeSDK, "AIREDGE"},
                {cbxVodafoneSDK, "VODAFONE"}, ' This will be treated as optional
                {cbxFlashSDK, "FLASH"}
            }

            Dim selectedSDKs As New Dictionary(Of String, String)

            ' Validate and collect selected SDKs
            For Each kvp In sdkComboBoxes
                Dim comboBox = kvp.Key
                Dim sdkName = kvp.Value
                Dim selectedItem = comboBox.SelectedItem

                ' Skip validation for Vodafone
                If sdkName = "VODAFONE" AndAlso selectedItem Is Nothing Then
                    Logger.LogInfo("Vodafone SDK not selected; continuing without it.")
                    Continue For
                End If

                ' Validate required SDKs
                If selectedItem Is Nothing Then
                    Logger.LogWarning($"Launch failed: No {sdkName} SDK selected.")
                    MessageBox.Show($"Please select a {sdkName} SDK before launching.")
                    Return
                End If

                selectedSDKs(sdkName) = selectedItem.ToString()
                Logger.LogInfo($"Selected {sdkName} SDK: {selectedItem}")
            Next

            ' Access the selected SDKs (Vodafone may be missing)
            Dim selectedDojaSDK = selectedSDKs("Doja")
            Dim selectedStarSDK = selectedSDKs("Star")
            Dim selectedAIREDGESDK = selectedSDKs("AIREDGE")
            Dim selectedJSKYSDK = selectedSDKs("JSKY")
            Dim selectedSOFTBANKSDK = selectedSDKs("SOFTBANK")
            Dim selectedFlashSDK = selectedSDKs("FLASH")
            Dim selectedVODAFONESDK As String = selectedSDKs.GetValueOrDefault("VODAFONE", "")

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
            Dim CorrectedEmulator As String = ""
            If CurrentSelectedGameJAM.ToLower.EndsWith(".jam") Then
                CorrectedEmulator = VerifyEmulatorType_JAM(CurrentSelectedGameJAM)
            ElseIf CurrentSelectedGameJAM.ToLower.EndsWith(".jad") Then
                'Need to find a better way to verify JAD files and the emulator they need to use
                'CorrectedEmulator = VerifyEmulatorType_JAD(CurrentSelectedGameJAM)
                CorrectedEmulator = selectedGame.Emulator
            ElseIf CurrentSelectedGameJAM.ToLower.EndsWith(".swf") Then
                CorrectedEmulator = "flash"
            Else
                CorrectedEmulator = selectedGame.Emulator
            End If
            Logger.LogInfo($"Detected emulator type: {CorrectedEmulator}")

            ' Get Game Directory Path
            Dim GameDirectory As String = ""
            If CorrectedEmulator = "doja" Or CorrectedEmulator = "star" Then
                Dim binIndex As Integer = CurrentSelectedGameJAM.LastIndexOf("\bin")
                If binIndex <> -1 Then
                    GameDirectory = CurrentSelectedGameJAM.Substring(0, binIndex)
                    Logger.LogInfo($"Game directory resolved to: {GameDirectory}")
                Else
                    Logger.LogWarning("Could not determine GameDirectory: '\bin' not found in CurrentSelectedGameJAM path.")
                End If
            End If

            ' Check for Helper Scripts
            If selectedGame.ENTitle.Contains("Dirge of Cerberus") Then
                Logger.LogInfo("Setting up FF7 Doja Helper Script...")
                Await gameManager.FF7_DOCLE_SetupAsync(DOJApath, GameDirectory)
            End If

            ' Start Launching Game
            UtilManager.ShowSnackBar($"Launching '{selectedGameTitle}'")
            UtilManager.SendAppLaunch(Path.GetFileName(CurrentSelectedGameJAM))
            Dim isEmulatorsRunning As Boolean = UtilManager.CheckAndCloseAllEmulators()
            If isEmulatorsRunning Then
                Logger.LogWarning("An emulator is still running. Aborting launch.")
                Return
            End If
            Select Case CorrectedEmulator.ToLower()
                Case "doja"
                    Logger.LogInfo("Launching game using DOJA emulator.")
                    Dim lowerDojaPath = DOJApath.ToLowerInvariant()

                    If lowerDojaPath.Contains("idkdoja") Then
                        utilManager.LaunchCustomDOJAGameCommand(DOJApath, DOJAEXE, CurrentSelectedGameJAM)
                    ElseIf lowerDojaPath.Contains("squirreljme") Then
                        utilManager.LaunchCustomDOJA_SJMEGameCommand(DOJApath, DOJAEXE, CurrentSelectedGameJAM)
                    ElseIf lowerDojaPath.Contains("kemnnx64") Then
                        utilManager.LaunchCustom_KEmulatorGameCommand(DOJApath, DOJAEXE, CurrentSelectedGameJAM)
                    End If
                    Logger.LogInfo($"Launched with: DojaPath={DOJApath}, DojaEXE={DOJAEXE}, GamePath={CurrentSelectedGameJAM}")

                Case "star"
                    Logger.LogInfo("Launching game using STAR emulator.")
                    utilManager.LaunchCustomSTARGameCommand(STARpath, STAREXE, CurrentSelectedGameJAM)
                    Logger.LogInfo($"Launched with: StarPath={STARpath}, StarEXE={STAREXE}, GamePath={CurrentSelectedGameJAM}")

                Case "jsky"
                    Logger.LogInfo("Launching game using JSKY emulator.")
                    Dim lowerJSKYPath = JSKYpath.ToLowerInvariant()

                    If lowerJSKYPath.Contains("jsky_") Then
                        utilManager.LaunchCustomJSKYGameCommand(JSKYpath, JSKYEXE, CurrentSelectedGameJAM)
                    ElseIf lowerJSKYPath.Contains("kemnnx64") Then
                        utilManager.LaunchCustom_KEmulatorGameCommand(JSKYpath, JSKYEXE, CurrentSelectedGameJAM)
                    End If
                    Logger.LogInfo($"Launched with: JSKYPath={JSKYpath}, JSKYEXE={JSKYEXE}, GamePath={CurrentSelectedGameJAM}")

                Case "vodafone"
                    Logger.LogInfo("Launching game using VODAFONE emulator.")
                    utilManager.LaunchCustom_KEmulatorGameCommand(VODAFONEpath, VODAFONEEXE, CurrentSelectedGameJAM)
                    Logger.LogInfo($"Launched with: VODAFONEPath={VODAFONEpath}, VODAFONEEXE={VODAFONEEXE}, GamePath={CurrentSelectedGameJAM}")

                Case "airedge"
                    Logger.LogInfo("Launching game using AIREDGE emulator.")
                    utilManager.LaunchCustom_KEmulatorGameCommand(AIREDGEpath, AIREDGEEXE, CurrentSelectedGameJAM)
                    Logger.LogInfo($"Launched with: AIREDGEPath={AIREDGEpath}, AIREDGEEXE={AIREDGEEXE}, GamePath={CurrentSelectedGameJAM}")

                Case "softbank"
                    Logger.LogInfo("Launching game using SOFTBANK emulator.")
                    utilManager.LaunchCustom_KEmulatorGameCommand(SOFTBANKpath, SOFTBANKEXE, CurrentSelectedGameJAM)
                    Logger.LogInfo($"Launched with: SOFTBANKPath={SOFTBANKpath}, SOFTBANKEXE={SOFTBANKEXE}, GamePath={CurrentSelectedGameJAM}")

                Case "flash"
                    Logger.LogInfo("Launching game using flash Player.")
                    utilManager.LaunchCustomFLASHGameCommand(FlashPlayerpath, FlashPlayerEXE, CurrentSelectedGameJAM)
                    Logger.LogInfo($"Launched with: flashPath={FlashPlayerpath}, flashEXE={FlashPlayerEXE}, SWFPath={CurrentSelectedGameJAM}")

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

        Dim selectedItem = ListViewMachiChara.SelectedItems(0)
        Dim selectedMachiChara = CType(selectedItem.Tag, MachiChara)

        If selectedMachiChara IsNot Nothing Then
            CurrentSelectedMachiCharaCFD = Path.Combine(DownloadsFolder, selectedMachiChara.CFDName)
            UtilManager.ShowSnackBar($"Launching '{CurrentSelectedMachiCharaCFD}'")
            UtilManager.SendAppLaunch(Path.GetFileName(CurrentSelectedMachiCharaCFD))
            utilManager.LaunchCustomMachiCharaCommand(MachiCharaExe, CurrentSelectedMachiCharaCFD)
        End If
    End Sub
    Private Sub btnCharaDenLaunch_Click(sender As Object, e As EventArgs) Handles btnCharaDenLaunch.Click
        If ListViewCharaDen.SelectedItems.Count = 0 Then
            MessageBox.Show("Please select a Chara-den to launch.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim selectedItem = ListViewCharaDen.SelectedItems(0)
        Dim selectedCharaDen = CType(selectedItem.Tag, CharaDen)

        If selectedCharaDen IsNot Nothing Then
            CurrentSelectedCharaDenAFD = Path.Combine(DownloadsFolder, selectedCharaDen.AFDName)
            UtilManager.ShowSnackBar($"Launching '{CurrentSelectedCharaDenAFD}'")
            UtilManager.SendAppLaunch(Path.GetFileName(CurrentSelectedCharaDenAFD))
            utilManager.LaunchCustomCharaDenCommand(CharaDenExe, CurrentSelectedCharaDenAFD)
        End If
    End Sub

    'Menu Strip Items
    Private Async Sub GamesToolStripMenuItem_Click(sender As Object, e As EventArgs)
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
    Private Async Sub MachiCharaToolStripMenuItem_Click(sender As Object, e As EventArgs)
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
    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Application.Exit()
    End Sub
    Public Shared Sub QuitApplication()
        Application.Exit()
    End Sub
    Private Sub RefreshToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Application.Restart()
    End Sub
    Private Async Sub AddGameToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Dim ENTitle = InputBox("Enter the English name of the game:", "English Game Name").Trim

        If String.IsNullOrWhiteSpace(ENTitle) Then
            MsgBox("The English game name cannot be empty. Exiting the operation.", MsgBoxStyle.Exclamation, "Input Error")
            Exit Sub
        End If

        Dim JPTitle = InputBox("Enter the Japanese name of the game (leave blank if same as English):", "Japanese Game Name").Trim
        If String.IsNullOrWhiteSpace(JPTitle) Then JPTitle = ENTitle

        Dim ZIPName = InputBox("Enter the name of the zip file (include .zip at the end):", "Zip File Name").Trim
        If String.IsNullOrWhiteSpace(ZIPName) Then
            MsgBox("The ZIP file name cannot be empty. Exiting the operation.", MsgBoxStyle.Exclamation, "Input Error")
            Exit Sub
        End If
        If Not ZIPName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) Then
            ZIPName += ".zip"
        End If

        Dim DownloadURL = InputBox("Enter the download URL for the zip file:", "Download URL").Trim
        If String.IsNullOrWhiteSpace(DownloadURL) Then
            MsgBox("The download URL cannot be empty. Exiting the operation.", MsgBoxStyle.Exclamation, "Input Error")
            Exit Sub
        End If

        Dim AppIconURL = InputBox("Enter the URL of a custom app icon (24x24) or leave blank to use the default icon:", "App Icon URL").Trim
        Dim SDCardData = InputBox("Enter the SD Card data zip URL or leave blank if not applicable:", "SD Card Data URL").Trim
        Dim Emulator = InputBox("Enter the emulator type (doja or star):", "Emulator").Trim.ToLower

        While Emulator <> "doja" AndAlso Emulator <> "star"
            Emulator = InputBox("Invalid input. Please enter only 'doja' or 'star':", "Emulator").Trim.ToLower
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

    'Config Option in TabPage
    Private Sub btnLaunchKey2Pad_Click(sender As Object, e As EventArgs) Handles btnLaunchKey2Pad.Click
        Dim processName As String = "antimicrox" ' No .exe
        Dim isRunning As Boolean = Process.GetProcessesByName(processName).Length > 0

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
    Private Sub btnLaunchAppConfig_Click(sender As Object, e As EventArgs) Handles btnLaunchAppConfig.Click
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
    Private Sub btnLoadShaderGlassConfig_Click(sender As Object, e As EventArgs) Handles btnLoadShaderGlassConfig.Click
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
    Private Sub btnUpdateNetworkUID_Click(sender As Object, e As EventArgs) Handles btnUpdateNetworkUID.Click
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
        FillCurrnetUIDLabel()
    End Sub
    Private Sub btnAddCustomApps_Click(sender As Object, e As EventArgs) Handles btnAddCustomApps.Click
        ' —— 1) Pick emulator via a MaterialForm + ComboBox + Recursive checkbox —— '
        Dim emus() As String = {"Doja", "Star", "JSky", "AirEdge", "Softbank", "Vodafone", "Flash"}

        Dim picker As New MaterialForm() With {
        .Text = "Choose Emulator",
        .Size = New Size(300, 260),
        .FormBorderStyle = FormBorderStyle.FixedDialog,
        .StartPosition = FormStartPosition.CenterParent,
        .MaximizeBox = False,
        .MinimizeBox = False,
        .KeyPreview = True
    }

        Dim cmb As New MaterialComboBox() With {
        .DataSource = emus,
        .DropDownStyle = ComboBoxStyle.DropDownList,
        .Location = New Point(35, 80),
        .Width = 240
    }

        Dim chkRecursive As New MaterialCheckBox() With {
        .Text = "Recursive",
        .Location = New Point(35, 130),
        .Width = 240
    }

        Dim btnOK As New MaterialButton() With {
        .Text = "OK",
        .Location = New Point(35, 175),
        .Width = 90
    }
        Dim btnCancel As New MaterialButton() With {
        .Text = "Cancel",
        .Location = New Point(155, 175),
        .Width = 90
    }

        AddHandler btnOK.Click, Sub()
                                    picker.DialogResult = DialogResult.OK
                                    picker.Close()
                                End Sub
        AddHandler btnCancel.Click, Sub()
                                        picker.DialogResult = DialogResult.Cancel
                                        picker.Close()
                                    End Sub

        picker.Controls.AddRange(New Control() {cmb, chkRecursive, btnOK, btnCancel})

        If picker.ShowDialog(Me) <> DialogResult.OK Then
            Exit Sub
        End If

        Dim selectedEmulator = cmb.SelectedItem.ToString().ToLower()
        Dim isRecursive = chkRecursive.Checked
        Dim searchOpt = If(isRecursive, SearchOption.AllDirectories, SearchOption.TopDirectoryOnly)

        ' —— 2) Determine extensions —— '
        Dim requiredExts As New List(Of String)()
        Dim optionalExts As New List(Of String)()
        Select Case selectedEmulator
            Case "doja", "star"
                requiredExts.AddRange({".jar", ".jam"})
                optionalExts.Add(".sp")
            Case "jsky", "airedge", "vodafone", "softbank"
                requiredExts.AddRange({".jar", ".jad"})
                optionalExts.Add(".rms")
            Case "flash"
                requiredExts.Add(".swf")
            Case Else
                MessageBox.Show("Unsupported emulator.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
        End Select

        ' —— 3) Pick folder & scan —— '
        If MessageBox.Show(
        $"Select the folder with your games.{vbCrLf}Ensure {String.Join("/", requiredExts)} share the same base name.",
        "Add Custom Games", MessageBoxButtons.OKCancel) = DialogResult.Cancel Then
            Exit Sub
        End If

        Using dlg As New FolderBrowserDialog() With {.Description = $"Locate *{requiredExts(0)} files"}
            If dlg.ShowDialog() <> DialogResult.OK Then Exit Sub
            Dim folder = dlg.SelectedPath

            ' grab all primary files (recursive if requested)
            Dim primaries = Directory.GetFiles(folder, "*" & requiredExts(0), searchOpt)

            Dim toDo = New Dictionary(Of String, Tuple(Of String(), String()))()
            Dim validBases = New List(Of String)()
            Dim skipped = New List(Of String)()

            For Each f In primaries
                Dim name = Path.GetFileNameWithoutExtension(f)
                Dim dest = Path.Combine("data", "downloads", name)

                ' find other required files anywhere under the folder
                Dim otherReq = requiredExts.Skip(1).
                Select(Function(ext)
                           Dim matches = Directory.GetFiles(folder, name & ext, searchOpt)
                           Return If(matches.Any(), matches(0), Nothing)
                       End Function).
                ToArray()

                If otherReq.All(Function(p) p IsNot Nothing) Then
                    If Directory.Exists(dest) Then
                        skipped.Add(name)
                    Else
                        ' find optional files the same way
                        Dim opts = optionalExts.
                        Select(Function(ext)
                                   Dim matches = Directory.GetFiles(folder, name & ext, searchOpt)
                                   Return If(matches.Any(), matches(0), Nothing)
                               End Function).
                        Where(Function(p) p IsNot Nothing).
                        ToArray()

                        Dim allReq = (New String() {f}).Concat(otherReq).ToArray()
                        toDo.Add(name, Tuple.Create(allReq, opts))
                        validBases.Add(name)
                    End If
                End If
            Next

            If Not validBases.Any() AndAlso Not skipped.Any() Then
                MessageBox.Show("No valid sets found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            End If

            ' —— 4) Confirm —— '
            Dim msg = ""
            If validBases.Any() Then msg &= "Will add:" & vbCrLf & String.Join(vbCrLf, validBases) & vbCrLf & vbCrLf
            If skipped.Any() Then msg &= "Skipping (exists):" & vbCrLf & String.Join(vbCrLf, skipped) & vbCrLf & vbCrLf
            msg &= "Proceed?"
            If MessageBox.Show(msg, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then Exit Sub

            ' —— 5) Copy files —— '
            Dim done = New List(Of String)()
            For Each kvp In toDo
                Dim name = kvp.Key
                Dim dest = Path.Combine("data", "downloads", name)

                If selectedEmulator = "doja" OrElse selectedEmulator = "star" Then
                    Dim binDir = Path.Combine(dest, "bin")
                    Dim spDir = Path.Combine(dest, "sp")
                    Directory.CreateDirectory(binDir)
                    Directory.CreateDirectory(spDir)

                    For Each reqFile In kvp.Value.Item1
                        File.Copy(reqFile, Path.Combine(binDir, Path.GetFileName(reqFile)), True)
                    Next
                    For Each optFile In kvp.Value.Item2
                        File.Copy(optFile, Path.Combine(spDir, Path.GetFileName(optFile)), True)
                    Next
                Else
                    Directory.CreateDirectory(dest)
                    For Each f2 In kvp.Value.Item1.Concat(kvp.Value.Item2)
                        File.Copy(f2, Path.Combine(dest, Path.GetFileName(f2)), True)
                    Next
                End If

                done.Add($"{name} | {selectedEmulator}")
            Next

            ' —— 6) Log to configs/customgames.txt —— '
            If done.Any() Then
                Dim cfg = CustomGamesTxtFile
                Directory.CreateDirectory(Path.GetDirectoryName(cfg))
                File.AppendAllLines(cfg, done)
            End If

            MessageBox.Show("Import complete!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Application.Restart()
        End Using
    End Sub
    Private Sub btnSaveDataManagement_Click(sender As Object, e As EventArgs) Handles btnSaveDataManagement.Click
        SaveDataManagerForm.ShowDialog()
    End Sub
    Private Sub btnSJMEUpdate_Click(sender As Object, e As EventArgs) Handles btnSJMEUpdate.Click
        Dim Apppath = $"cmd"
        Dim SJMEApppath As String = $"{ToolsFolder}\squirreljme\squirreljme-updater.exe"
        Dim startInfo As New ProcessStartInfo()
        startInfo.FileName = Apppath
        startInfo.Arguments = $"/C {SJMEApppath}"
        startInfo.UseShellExecute = False
        startInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
        Dim process As Process = Process.Start(startInfo)
    End Sub
    Private Sub FillCurrnetUIDLabel()
        txtCurrentUID.Text = $"{NetworkUID}"
        If txtCurrentUID.Text = "" Or txtCurrentUID.Text.ToUpper = "NULLGWDOCOMO" Then
            lblInvalidUID.Visible = True
        Else
            lblInvalidUID.Visible = False
        End If
    End Sub
    'Help Options in TabPage
    Private Sub SetupLabelsinOptions()
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
        "  • Make sure Java 8 (32-bit) is installed or try reinstalling" & vbCrLf &
        "  • Ensure the folder path has NO spaces" & vbCrLf & vbCrLf &
        "If you're still having trouble," & vbCrLf &
        "ask for help in the #troubleshooting channel."
        lblHelp_troubleshooting.Text = troubleshootingMessage

        Dim aboutText As String =
        "Keitai World Launcher" & Environment.NewLine & Environment.NewLine &
        $"Version: B{KeitaiWorldLauncher.My.Application.Info.Version.ToString}" & Environment.NewLine & Environment.NewLine &
        $"Website: KeitaiArchive.org"
        lblHelp_AppVer.Text = aboutText

    End Sub
    Private Sub btnVisitKeitaiArchive_Click(sender As Object, e As EventArgs) Handles btnVisitKeitaiArchive.Click
        Dim url As String = "https://keitaiarchive.org"
        Dim psi As New ProcessStartInfo(url) With {.UseShellExecute = True}
        Process.Start(psi)
    End Sub
    Private Sub btnControls_Click(sender As Object, e As EventArgs) Handles btnControls.Click
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

    'TabPage Changes
    Private Sub tab(sender As Object, e As EventArgs) Handles MaterialTabControl1.SelectedIndexChanged

        If MaterialTabControl1.SelectedTab Is tpStats Then
            LoadPlaytimesToListView()
        ElseIf MaterialTabControl1.SelectedTab Is tpConfig Then
            FillCurrnetUIDLabel()
        End If
    End Sub
End Class