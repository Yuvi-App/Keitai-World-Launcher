Imports System.Linq
Imports System.Windows.Forms

Partial Public Class MainForm
    Private _productToolTips As ToolTip

    Private Sub InitializeAccessibilityPolish()
        KeyPreview = True
        _productToolTips = If(components Is Nothing, New ToolTip(), New ToolTip(components))
        _productToolTips.AutoPopDelay = 8000
        _productToolTips.InitialDelay = 450
        _productToolTips.ReshowDelay = 100
        _productToolTips.ShowAlways = True

        NormalizeProductTerminology()
        ConfigureAccessibleMetadata()
        ConfigurePrimaryTabOrder()
        ConfigureKeyboardNavigation()
        ConfigureActionMenuGuidance()
    End Sub

    Private Sub NormalizeProductTerminology()
        chkbxDialpadNumpad.Text = "Dialpad numpad"
        chkbxEnableController.Text = "Enable controller"
        chkbxHidePhoneUI.Text = "Hide phone UI"
        chkbxEnableHighPerformanceEmulator.Text = "Enable high-performance emulators (experimental)"
        chkboxEnforceHardwareRendering.Text = "Enforce hardware rendering (experimental)"
        chkbxOpenDojaLaunchGUI.Text = "Launch OpenDoJa GUI"
        chkboxNetworkModifyURLS.Text = "Modify URLs for supported online apps"

        cmsGameLV_Launch.Text = "Play"
        BackupSaveToolStripMenuItem.Text = "Back up save"
        OpenGameFolderToolStripMenuItem.Text = "Open folder"
        cmsBombermanPuzzle.Text = "Bomberman Puzzle tools"
        ImportStageToolStripMenuItem.Text = "Import stage"
        ExportStageToolStripMenuItem.Text = "Export stage"
    End Sub

    Private Sub ConfigureAccessibleMetadata()
        SetAccessible(
            MaterialTabSelector1,
            "Main navigation",
            "Home, Library, Settings, and Activity. Use the left and right arrow keys, or press Control plus 1 through 4, to switch sections.")
        MaterialTabSelector1.AccessibleRole = AccessibleRole.PageTabList
        MaterialTabSelector1.TabStop = True

        SetAccessible(
            _libraryCategoryTabs,
            "Library categories",
            "Apps, Machi-Chara, and Chara-Den. Use the left and right arrow keys to change categories.")
        SetAccessible(txtLVSearch, "Search apps", "Search titles. Press Control+F from the Library to return here.")
        SetAccessible(cbxFilterType, "Filter apps", "Choose which apps appear in the list.")
        SetAccessible(ListViewGames, "Apps", "Use the arrow keys to choose an app. Press Enter or double-click to play or download it.")
        SetAccessible(ListViewGamesVariants, "App variants", "Use the arrow keys to choose a variant. Press Enter to select it.")
        SetAccessible(btnLaunchGame, "Play or download selected app", "Select an app first.")
        SetAccessible(_btnGameActions, "Selected app actions", "Select an app to see its additional actions.")

        SetAccessible(chkbxLocalEmulator, "Use Locale Emulator", "Run supported SDKs through Locale Emulator.")
        SetAccessible(chkbxShaderGlass, "Use ShaderGlass", "Apply the selected ShaderGlass filter while the app is running.")
        SetAccessible(cbxShaderGlassScaling, "ShaderGlass scaling", "Choose the display scale used by ShaderGlass.")
        SetAccessible(chkbxDialpadNumpad, "Use dialpad numpad", "Map the phone dialpad to the keyboard numpad.")
        SetAccessible(chkbxDialpadRotated, "Rotate dialpad", "Use the rotated dialpad mapping.")
        SetAccessible(chkbxEnableController, "Enable controller", "Enable game-controller input for the selected app.")
        SetAccessible(chkboxControllerVibration, "Controller vibration", "Enable controller vibration when supported.")
        SetAccessible(cbxGameControllers, "Controller", "Choose the controller used for the selected app.")
        SetAccessible(cbxControllerProfile, "Controller profile", "Choose the button-mapping profile used by the selected controller.")

        SetAccessible(_txtMachiSearch, "Search Machi-Chara", "Search the Machi-Chara library. Press Control+F from this category to return here.")
        SetAccessible(ListViewMachiChara, "Machi-Chara", "Use the arrow keys to choose a Machi-Chara. Press Enter or double-click to play or download it.")
        SetAccessible(btnMachiCharaLaunch, "Play or download selected Machi-Chara", "Select a Machi-Chara first.")
        SetAccessible(_btnMachiActions, "Selected Machi-Chara actions", "Select a Machi-Chara to see its additional actions.")
        SetAccessible(chkboxMachiCharaLocalEmulator, "Use Locale Emulator", "Run the official SDK through Locale Emulator.")

        SetAccessible(_txtCharaSearch, "Search Chara-Den", "Search the Chara-Den library. Press Control+F from this category to return here.")
        SetAccessible(ListViewCharaDen, "Chara-Den", "Use the arrow keys to choose a Chara-Den. Press Enter or double-click to play or download it.")
        SetAccessible(btnCharaDenLaunch, "Play or download selected Chara-Den", "Select a Chara-Den first.")
        SetAccessible(_btnCharaActions, "Selected Chara-Den actions", "Select a Chara-Den to see its additional actions.")
        SetAccessible(chkboxCharadenLocalEmulator, "Use Locale Emulator", "Run Chara-Den through Locale Emulator.")

        SetAccessible(_settingsNavigation, "Settings categories", "Use the up and down arrow keys to change categories. Press Enter to move into the selected page.")
        SetAccessible(lvwPlaytimes, "Play history", "A list of apps with total play time and session count.")
        SetAccessible(_activityEmptyState, "No activity yet", "Play an app and its history will appear here.")
        SetAccessible(_lblActivityTotalTime, "Total play time", "Total time played across all tracked apps.")
        SetAccessible(_lblActivitySessions, "Sessions", "Total number of recorded play sessions.")
        SetAccessible(_lblActivityApps, "Apps played", "Number of apps with recorded activity.")
        SetAccessible(_lblActivityMostPlayed, "Most played app", "The app with the greatest recorded play time.")

        ConfigureSettingsAccessibleMetadata()

        SetGuidance(txtLVSearch, "Search apps (Control+F)")
        SetGuidance(cbxFilterType, "Filter the app list")
        SetGuidance(chkbxLocalEmulator, "Run supported SDKs through Locale Emulator")
        SetGuidance(chkbxShaderGlass, "Apply the selected ShaderGlass filter")
        SetGuidance(cbxShaderGlassScaling, "Choose the ShaderGlass display scale")
        SetGuidance(cbxGameControllers, "Choose a controller")
        SetGuidance(cbxControllerProfile, "Choose a controller profile")
        SetGuidance(_txtMachiSearch, "Search Machi-Chara (Control+F)")
        SetGuidance(_txtCharaSearch, "Search Chara-Den (Control+F)")
        SetGuidance(_cbxMachiCharaLauncher, "Choose the app used to open Machi-Chara files")
        ApplyAccessibleRoles(Me)
    End Sub

    Private Sub ConfigureSettingsAccessibleMetadata()
        SetAccessible(btnSaveDataManagement, "Manage save data", "Open the save-data manager.")
        SetAccessible(btnLaunchKey2Pad, "Controller mapping", "Open the keyboard and controller mapping tool.")
        SetAccessible(btnAddCustomApps, "Add custom apps", "Import an app that is not in the online library.")
        SetAccessible(cbxAudioType, "Audio type", "Choose the audio implementation used by DoJa and Star emulators.")
        SetAccessible(cbxInternalRenderingResolution, "Rendering resolution", "Choose the internal rendering resolution.")
        SetAccessible(cbxOpenDojaHostScale, "OpenDoJa host scale", "Choose the OpenDoJa display scale.")
        SetAccessible(cbxOpenDojaAudioType, "OpenDoJa audio type", "Choose the OpenDoJa audio implementation.")
        SetAccessible(cbxOpenDojaFontType, "OpenDoJa font type", "Choose the font implementation used by OpenDoJa.")
        SetAccessible(cbxOpenDojaLaunchType, "OpenDoJa launch type", "Choose how OpenDoJa starts apps.")
        SetAccessible(cbxSJMELaunchOption, "SquirrelJME launch option", "Available when SquirrelJME is the selected DoJa SDK.")
        SetAccessible(cbxSJMEScaling, "SquirrelJME scaling", "Available when SquirrelJME is the selected DoJa SDK.")
        SetAccessible(btnSJMEUpdate, "Update SquirrelJME", "Available when SquirrelJME is the selected DoJa SDK.")
        SetAccessible(btnReMEXALaunchGUI, "Open ReMEXA", "Open the ReMEXA configuration interface.")
        SetAccessible(cbxShaderGlass_Shader, "Default shader", "Choose the default ShaderGlass filter.")
        SetAccessible(btnUpdateNetworkUID, "Update network identity", "Change the Network UID and Terminal ID used by supported online apps.")
        SetAccessible(btnVisitKeitaiArchive, "Visit Keitai Archive", "Open the Keitai Archive website in your default browser.")
        SetAccessible(btnControls, "Keyboard and controller guide", "Open the keyboard and controller reference.")

        SetGuidance(cbxSJMELaunchOption, "Available when SquirrelJME is the selected DoJa SDK")
        SetGuidance(cbxSJMEScaling, "Available when SquirrelJME is the selected DoJa SDK")
        SetGuidance(btnSJMEUpdate, "Available when SquirrelJME is the selected DoJa SDK")
    End Sub

    Private Sub ConfigurePrimaryTabOrder()
        MaterialTabSelector1.TabIndex = 0
        MaterialTabControl1.TabIndex = 1
        MaterialTabControl1.TabStop = False

        _libraryCategoryTabs.TabIndex = 0
        _libraryCategoryTabs.TabStop = True

        GroupBox1.TabIndex = 0
        txtLVSearch.TabIndex = 0
        cbxFilterType.TabIndex = 1
        ListViewGames.TabIndex = 2
        ListViewGamesVariants.TabIndex = 3

        _gameActionBar.TabIndex = 0
        btnLaunchGame.TabIndex = 0
        _btnGameActions.TabIndex = 1
        chkbxLocalEmulator.TabIndex = 1
        chkbxShaderGlass.TabIndex = 2
        cbxShaderGlassScaling.TabIndex = 3
        chkbxDialpadNumpad.TabIndex = 4
        chkbxDialpadRotated.TabIndex = 5
        chkbxEnableController.TabIndex = 6
        chkboxControllerVibration.TabIndex = 7
        cbxGameControllers.TabIndex = 8
        cbxControllerProfile.TabIndex = 9

        ConfigureCharacterTabOrder(_machiLibraryGrid, GroupBox2, _txtMachiSearch, ListViewMachiChara, btnMachiCharaLaunch, _btnMachiActions)
        _cbxMachiCharaLauncher.TabIndex = 2
        chkboxMachiCharaLocalEmulator.TabIndex = 3
        ConfigureCharacterTabOrder(_charaLibraryGrid, GroupBox10, _txtCharaSearch, ListViewCharaDen, btnCharaDenLaunch, _btnCharaActions)
        chkboxCharadenLocalEmulator.TabIndex = 2

        _settingsNavigation.TabIndex = 0
        For Each page In _settingsPages
            NormalizeChildTabOrder(page)
        Next

        Dim activityRefresh = FindByAccessibleName(tpStats, "Refresh activity")
        If activityRefresh IsNot Nothing Then activityRefresh.TabIndex = 0
        lvwPlaytimes.TabIndex = 1
    End Sub

    Private Shared Sub ConfigureCharacterTabOrder(
        grid As TableLayoutPanel,
        listGroup As GroupBox,
        search As TextBox,
        list As ListView,
        primaryAction As Button,
        actions As Button)

        listGroup.TabIndex = 0
        search.TabIndex = 0
        list.TabIndex = 1
        primaryAction.TabIndex = 0
        actions.TabIndex = 1

        For Each child As Control In grid.Controls
            child.TabIndex = grid.GetColumn(child)
        Next
    End Sub

    Private Shared Sub NormalizeChildTabOrder(parent As Control)
        Dim children = parent.Controls.Cast(Of Control)()
        If TypeOf parent Is TableLayoutPanel Then
            Dim table = DirectCast(parent, TableLayoutPanel)
            children = children.OrderBy(Function(control) table.GetRow(control)).ThenBy(Function(control) table.GetColumn(control))
        ElseIf TypeOf parent Is FlowLayoutPanel Then
            children = children.OrderBy(Function(control) parent.Controls.GetChildIndex(control))
        Else
            children = children.OrderBy(Function(control) control.Top).ThenBy(Function(control) control.Left)
        End If

        Dim tabIndex = 0
        For Each child In children
            child.TabIndex = tabIndex
            tabIndex += 1
            If child.HasChildren Then NormalizeChildTabOrder(child)
        Next
    End Sub

    Private Sub ConfigureKeyboardNavigation()
        AddHandler MaterialTabSelector1.KeyDown, AddressOf MainNavigation_KeyDown
        AddHandler _libraryCategoryTabs.KeyDown, AddressOf LibraryCategoryTabs_KeyDown
        AddHandler txtLVSearch.KeyDown, AddressOf LibrarySearch_KeyDown
        AddHandler _txtMachiSearch.KeyDown, AddressOf LibrarySearch_KeyDown
        AddHandler _txtCharaSearch.KeyDown, AddressOf LibrarySearch_KeyDown
        AddHandler _settingsNavigation.KeyDown, AddressOf SettingsNavigation_KeyDown
    End Sub

    Private Sub MainNavigation_KeyDown(sender As Object, e As KeyEventArgs)
        If e.KeyCode = Keys.Enter OrElse e.KeyCode = Keys.Down Then
            FocusSelectedMainPage()
            e.Handled = True
            e.SuppressKeyPress = True
            Return
        End If
        If e.KeyCode <> Keys.Left AndAlso e.KeyCode <> Keys.Right Then Return

        Dim direction = If(e.KeyCode = Keys.Right, 1, -1)
        Dim nextIndex = Math.Max(0, Math.Min(MaterialTabControl1.TabPages.Count - 1, MaterialTabControl1.SelectedIndex + direction))
        MaterialTabControl1.SelectedIndex = nextIndex
        e.Handled = True
        e.SuppressKeyPress = True
    End Sub

    Private Sub LibraryCategoryTabs_KeyDown(sender As Object, e As KeyEventArgs)
        If e.KeyCode <> Keys.Enter AndAlso e.KeyCode <> Keys.Down Then Return
        FocusCurrentLibrarySearch()
        e.Handled = True
        e.SuppressKeyPress = True
    End Sub

    Private Sub LibrarySearch_KeyDown(sender As Object, e As KeyEventArgs)
        Dim search = TryCast(sender, TextBox)
        If search Is Nothing Then Return

        If e.KeyCode = Keys.Escape AndAlso search.TextLength > 0 Then
            search.Clear()
            e.Handled = True
            e.SuppressKeyPress = True
            Return
        End If
        If e.KeyCode <> Keys.Enter Then Return

        Dim target As ListView = Nothing
        If search Is txtLVSearch Then
            target = ListViewGames
        ElseIf search Is _txtMachiSearch Then
            target = ListViewMachiChara
        ElseIf search Is _txtCharaSearch Then
            target = ListViewCharaDen
        End If
        If target Is Nothing Then Return

        If target.Items.Count > 0 AndAlso target.SelectedItems.Count = 0 Then
            target.Items(0).Selected = True
            target.Items(0).Focused = True
            target.Items(0).EnsureVisible()
        End If
        target.Focus()
        e.Handled = True
        e.SuppressKeyPress = True
    End Sub

    Private Sub SettingsNavigation_KeyDown(sender As Object, e As KeyEventArgs)
        If e.KeyCode <> Keys.Enter OrElse _settingsNavigation.SelectedIndex < 0 Then Return
        FocusFirstFocusable(_settingsPages(_settingsNavigation.SelectedIndex))
        e.Handled = True
        e.SuppressKeyPress = True
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        Select Case keyData
            Case Keys.Control Or Keys.D1, Keys.Control Or Keys.NumPad1
                SelectMainSection(tpHomepage)
                Return True
            Case Keys.Control Or Keys.D2, Keys.Control Or Keys.NumPad2
                SelectMainSection(tpAppli)
                Return True
            Case Keys.Control Or Keys.D3, Keys.Control Or Keys.NumPad3
                SelectMainSection(tpConfig)
                Return True
            Case Keys.Control Or Keys.D4, Keys.Control Or Keys.NumPad4
                SelectMainSection(tpStats)
                Return True
            Case Keys.Control Or Keys.F
                If MaterialTabControl1.SelectedTab Is tpAppli Then
                    FocusCurrentLibrarySearch()
                    Return True
                End If
        End Select

        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Private Sub SelectMainSection(page As TabPage)
        If page Is Nothing OrElse Not MaterialTabControl1.TabPages.Contains(page) Then Return
        MaterialTabControl1.SelectedTab = page

        If page Is tpAppli Then
            _libraryCategoryTabs.Focus()
        ElseIf page Is tpConfig Then
            _settingsNavigation.Focus()
        ElseIf page Is tpStats Then
            Dim activityRefresh = FindByAccessibleName(tpStats, "Refresh activity")
            If activityRefresh IsNot Nothing Then activityRefresh.Focus() Else FocusFirstFocusable(tpStats)
        Else
            FocusFirstFocusable(page)
        End If
    End Sub

    Private Sub FocusSelectedMainPage()
        Dim page = MaterialTabControl1.SelectedTab
        If page Is tpAppli Then
            _libraryCategoryTabs.Focus()
        ElseIf page Is tpConfig Then
            _settingsNavigation.Focus()
        ElseIf page Is tpStats Then
            Dim activityRefresh = FindByAccessibleName(tpStats, "Refresh activity")
            If activityRefresh IsNot Nothing Then activityRefresh.Focus() Else FocusFirstFocusable(tpStats)
        Else
            FocusFirstFocusable(page)
        End If
    End Sub

    Private Sub FocusCurrentLibrarySearch()
        Dim search As TextBox = txtLVSearch
        If _libraryCategoryTabs.SelectedIndex = 1 Then
            search = _txtMachiSearch
        ElseIf _libraryCategoryTabs.SelectedIndex = 2 Then
            search = _txtCharaSearch
        End If
        search.Focus()
        search.SelectAll()
    End Sub

    Private Shared Function FocusFirstFocusable(parent As Control) As Boolean
        If parent Is Nothing Then Return False
        For Each child As Control In parent.Controls.Cast(Of Control)().OrderBy(Function(control) control.TabIndex)
            If child.Visible AndAlso child.Enabled AndAlso child.TabStop AndAlso child.CanSelect Then
                child.Focus()
                Return True
            End If
            If FocusFirstFocusable(child) Then Return True
        Next
        Return False
    End Function

    Private Sub ConfigureActionMenuGuidance()
        _gameActionsMenu.ShowItemToolTips = True
        _actionRedownload.ToolTipText = "Download a fresh copy. Available when the app is installed and the launcher is online."
        _actionBackupSave.ToolTipText = "Back up save data. Available after the app is installed."
        _actionFavorite.ToolTipText = "Add or remove the app from Favorites."
        _actionOpenFolder.ToolTipText = "Open the installed app files. Available after installation."
        _actionDelete.ToolTipText = "Remove the installed app files."

        _machiActionsMenu.ShowItemToolTips = True
        _actionMachiRedownload.ToolTipText = "Download a fresh copy. Available when installed and online."
        _actionMachiDelete.ToolTipText = "Remove the installed Machi-Chara file."
        _charaActionsMenu.ShowItemToolTips = True
        _actionCharaRedownload.ToolTipText = "Download a fresh copy. Available when installed and online."
        _actionCharaDelete.ToolTipText = "Remove the installed Chara-Den file."
    End Sub

    Private Sub UpdateActivityAccessibility()
        _lblActivityTotalTime.AccessibleName = $"Total play time: {_lblActivityTotalTime.Text}"
        _lblActivitySessions.AccessibleName = $"Sessions: {_lblActivitySessions.Text}"
        _lblActivityApps.AccessibleName = $"Apps played: {_lblActivityApps.Text}"
        _lblActivityMostPlayed.AccessibleName = $"Most played app: {_lblActivityMostPlayed.Text}"
        _lblActivityHistoryCount.AccessibleName = $"Play history: {_lblActivityHistoryCount.Text}"
    End Sub

    Private Sub SetActionGuidance(button As Button, description As String)
        If button Is Nothing Then Return
        button.AccessibleName = button.Text.Replace("&", String.Empty).Replace(ChrW(&H25BE), String.Empty).Trim()
        button.AccessibleDescription = description
        SetGuidance(button, description)
    End Sub

    Private Sub SetStatusGuidance(statusLabel As Label, description As String)
        If statusLabel Is Nothing Then Return
        statusLabel.AccessibleDescription = description
        SetGuidance(statusLabel, description)
    End Sub

    Private Sub SetGuidance(control As Control, text As String)
        If control Is Nothing OrElse _productToolTips Is Nothing Then Return
        _productToolTips.SetToolTip(control, text)
    End Sub

    Private Shared Sub SetAccessible(control As Control, accessibleName As String, description As String)
        If control Is Nothing Then Return
        control.AccessibleName = accessibleName
        control.AccessibleDescription = description
    End Sub

    Private Shared Sub ApplyAccessibleRoles(parent As Control)
        If parent Is Nothing Then Return
        For Each child As Control In parent.Controls
            If TypeOf child Is Button Then
                child.AccessibleRole = AccessibleRole.PushButton
            ElseIf TypeOf child Is CheckBox Then
                child.AccessibleRole = AccessibleRole.CheckButton
            ElseIf TypeOf child Is ComboBox Then
                child.AccessibleRole = AccessibleRole.ComboBox
            ElseIf TypeOf child Is TextBox Then
                child.AccessibleRole = AccessibleRole.Text
            ElseIf TypeOf child Is ListView OrElse TypeOf child Is ListBox Then
                child.AccessibleRole = AccessibleRole.List
            ElseIf TypeOf child Is TabControl Then
                child.AccessibleRole = AccessibleRole.PageTabList
            ElseIf TypeOf child Is Label Then
                child.AccessibleRole = AccessibleRole.StaticText
            ElseIf TypeOf child Is GroupBox Then
                child.AccessibleRole = AccessibleRole.Grouping
            End If
            If child.HasChildren Then ApplyAccessibleRoles(child)
        Next
    End Sub

    Private Shared Function FindByAccessibleName(parent As Control, accessibleName As String) As Control
        If parent Is Nothing Then Return Nothing
        For Each child As Control In parent.Controls
            If String.Equals(child.AccessibleName, accessibleName, StringComparison.OrdinalIgnoreCase) Then Return child
            Dim nested = FindByAccessibleName(child, accessibleName)
            If nested IsNot Nothing Then Return nested
        Next
        Return Nothing
    End Function
End Class
