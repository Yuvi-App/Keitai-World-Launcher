<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits ReaLTaiizor.Forms.MaterialForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        ImageListGames = New ImageList(components)
        cmsGameLV = New ContextMenuStrip(components)
        cmsGameLV_Launch = New ToolStripMenuItem()
        cmsGameLV_Download = New ToolStripMenuItem()
        cmsGameLV_Delete = New ToolStripMenuItem()
        cmsGameLV_Favorite = New ToolStripMenuItem()
        BackupSaveToolStripMenuItem = New ToolStripMenuItem()
        OpenGameFolderToolStripMenuItem = New ToolStripMenuItem()
        OpenFileDialog1 = New OpenFileDialog()
        selectionTimer = New Timer(components)
        FolderBrowserDialog1 = New FolderBrowserDialog()
        btnMachiCharaLaunch = New Button()
        GroupBox2 = New GroupBox()
        ListViewMachiChara = New ListView()
        cmsMachiCharaLV = New ContextMenuStrip(components)
        DownloadCMS_MachiChara = New ToolStripMenuItem()
        DeleteCMS_MachiChara = New ToolStripMenuItem()
        lblMachiCharaTotalCount = New Label()
        chkbxShaderGlass = New CheckBox()
        GroupBox1 = New GroupBox()
        Panel1 = New Panel()
        lblTotalVariantCount = New Label()
        lblFilteredGameCount = New Label()
        lblTotalGameCount = New Label()
        ListViewGamesVariants = New ListView()
        cbxFilterType = New ComboBox()
        txtLVSearch = New TextBox()
        ListViewGames = New ListView()
        gbxGameInfo = New GroupBox()
        panelDynamic = New Panel()
        pbGameDL = New ProgressBar()
        btnLaunchGame = New Button()
        MaterialTabControl1 = New ReaLTaiizor.Controls.MaterialTabControl()
        tpAppli = New TabPage()
        GroupBox3 = New GroupBox()
        chkbxDialpadNumpad = New CheckBox()
        chkboxControllerVibration = New CheckBox()
        cbxControllerProfile = New ComboBox()
        chkbxEnableController = New CheckBox()
        cbxGameControllers = New ComboBox()
        Label2 = New Label()
        cbxShaderGlassScaling = New ComboBox()
        chkbxLocalEmulator = New CheckBox()
        tpMachiChara = New TabPage()
        chkboxMachiCharaLocalEmulator = New CheckBox()
        tpConfig = New TabPage()
        GroupBox6 = New GroupBox()
        btnSaveDataManagement = New Button()
        btnAddCustomApps = New Button()
        btnUpdateNetworkUID = New Button()
        btnLoadShaderGlassConfig = New Button()
        btnLaunchAppConfig = New Button()
        btnLaunchKey2Pad = New Button()
        GroupBox5 = New GroupBox()
        chkbxHidePhoneUI = New CheckBox()
        lblAudioWarning = New Label()
        Label1 = New Label()
        cobxAudioType = New ComboBox()
        GroupBox4 = New GroupBox()
        cbxFlashSDK = New ComboBox()
        Label7 = New Label()
        Label6 = New Label()
        cbxJSKYSDK = New ComboBox()
        Label5 = New Label()
        Label3 = New Label()
        cbxStarSDK = New ComboBox()
        cbxDojaSDK = New ComboBox()
        Label4 = New Label()
        tpHelp = New TabPage()
        GroupBox8 = New GroupBox()
        lblHelp_troubleshooting = New Label()
        GroupBox7 = New GroupBox()
        lblHelp_AppVer = New Label()
        btnControls = New Button()
        MaterialTabSelector1 = New ReaLTaiizor.Controls.MaterialTabSelector()
        cmsGameLV.SuspendLayout()
        GroupBox2.SuspendLayout()
        cmsMachiCharaLV.SuspendLayout()
        GroupBox1.SuspendLayout()
        Panel1.SuspendLayout()
        gbxGameInfo.SuspendLayout()
        panelDynamic.SuspendLayout()
        MaterialTabControl1.SuspendLayout()
        tpAppli.SuspendLayout()
        GroupBox3.SuspendLayout()
        tpMachiChara.SuspendLayout()
        tpConfig.SuspendLayout()
        GroupBox6.SuspendLayout()
        GroupBox5.SuspendLayout()
        GroupBox4.SuspendLayout()
        tpHelp.SuspendLayout()
        GroupBox8.SuspendLayout()
        GroupBox7.SuspendLayout()
        SuspendLayout()
        ' 
        ' ImageListGames
        ' 
        ImageListGames.ColorDepth = ColorDepth.Depth32Bit
        ImageListGames.ImageSize = New Size(36, 36)
        ImageListGames.TransparentColor = Color.Transparent
        ' 
        ' cmsGameLV
        ' 
        cmsGameLV.ImageScalingSize = New Size(20, 20)
        cmsGameLV.Items.AddRange(New ToolStripItem() {cmsGameLV_Launch, cmsGameLV_Download, cmsGameLV_Delete, cmsGameLV_Favorite, BackupSaveToolStripMenuItem, OpenGameFolderToolStripMenuItem})
        cmsGameLV.Name = "cmsGameLV"
        cmsGameLV.Size = New Size(171, 136)
        ' 
        ' cmsGameLV_Launch
        ' 
        cmsGameLV_Launch.Name = "cmsGameLV_Launch"
        cmsGameLV_Launch.Size = New Size(170, 22)
        cmsGameLV_Launch.Text = "Launch"
        ' 
        ' cmsGameLV_Download
        ' 
        cmsGameLV_Download.Name = "cmsGameLV_Download"
        cmsGameLV_Download.Size = New Size(170, 22)
        cmsGameLV_Download.Text = "Download"
        ' 
        ' cmsGameLV_Delete
        ' 
        cmsGameLV_Delete.Name = "cmsGameLV_Delete"
        cmsGameLV_Delete.Size = New Size(170, 22)
        cmsGameLV_Delete.Text = "Delete"
        ' 
        ' cmsGameLV_Favorite
        ' 
        cmsGameLV_Favorite.Name = "cmsGameLV_Favorite"
        cmsGameLV_Favorite.Size = New Size(170, 22)
        cmsGameLV_Favorite.Text = "Favorite"
        ' 
        ' BackupSaveToolStripMenuItem
        ' 
        BackupSaveToolStripMenuItem.Name = "BackupSaveToolStripMenuItem"
        BackupSaveToolStripMenuItem.Size = New Size(170, 22)
        BackupSaveToolStripMenuItem.Text = "Backup Save (SP)"
        ' 
        ' OpenGameFolderToolStripMenuItem
        ' 
        OpenGameFolderToolStripMenuItem.Name = "OpenGameFolderToolStripMenuItem"
        OpenGameFolderToolStripMenuItem.Size = New Size(170, 22)
        OpenGameFolderToolStripMenuItem.Text = "Open game folder"
        ' 
        ' OpenFileDialog1
        ' 
        OpenFileDialog1.FileName = "OpenFileDialog1"
        ' 
        ' selectionTimer
        ' 
        selectionTimer.Interval = 200
        ' 
        ' btnMachiCharaLaunch
        ' 
        btnMachiCharaLaunch.Enabled = False
        btnMachiCharaLaunch.Location = New Point(415, 26)
        btnMachiCharaLaunch.Margin = New Padding(2)
        btnMachiCharaLaunch.Name = "btnMachiCharaLaunch"
        btnMachiCharaLaunch.Size = New Size(110, 54)
        btnMachiCharaLaunch.TabIndex = 3
        btnMachiCharaLaunch.Text = "Launch"
        btnMachiCharaLaunch.UseVisualStyleBackColor = True
        ' 
        ' GroupBox2
        ' 
        GroupBox2.Controls.Add(ListViewMachiChara)
        GroupBox2.Controls.Add(lblMachiCharaTotalCount)
        GroupBox2.Location = New Point(12, 8)
        GroupBox2.Margin = New Padding(2)
        GroupBox2.Name = "GroupBox2"
        GroupBox2.Padding = New Padding(2)
        GroupBox2.Size = New Size(399, 568)
        GroupBox2.TabIndex = 1
        GroupBox2.TabStop = False
        GroupBox2.Text = "Available Machi Chara"
        ' 
        ' ListViewMachiChara
        ' 
        ListViewMachiChara.Activation = ItemActivation.OneClick
        ListViewMachiChara.ContextMenuStrip = cmsMachiCharaLV
        ListViewMachiChara.Dock = DockStyle.Fill
        ListViewMachiChara.FullRowSelect = True
        ListViewMachiChara.HeaderStyle = ColumnHeaderStyle.Nonclickable
        ListViewMachiChara.Location = New Point(2, 18)
        ListViewMachiChara.Margin = New Padding(2)
        ListViewMachiChara.Name = "ListViewMachiChara"
        ListViewMachiChara.Size = New Size(395, 516)
        ListViewMachiChara.Sorting = SortOrder.Ascending
        ListViewMachiChara.TabIndex = 2
        ListViewMachiChara.UseCompatibleStateImageBehavior = False
        ListViewMachiChara.View = View.Details
        ' 
        ' cmsMachiCharaLV
        ' 
        cmsMachiCharaLV.Items.AddRange(New ToolStripItem() {DownloadCMS_MachiChara, DeleteCMS_MachiChara})
        cmsMachiCharaLV.Name = "cmsMachiCharaLV"
        cmsMachiCharaLV.Size = New Size(129, 48)
        ' 
        ' DownloadCMS_MachiChara
        ' 
        DownloadCMS_MachiChara.Name = "DownloadCMS_MachiChara"
        DownloadCMS_MachiChara.Size = New Size(128, 22)
        DownloadCMS_MachiChara.Text = "Download"
        ' 
        ' DeleteCMS_MachiChara
        ' 
        DeleteCMS_MachiChara.Name = "DeleteCMS_MachiChara"
        DeleteCMS_MachiChara.Size = New Size(128, 22)
        DeleteCMS_MachiChara.Text = "Delete"
        ' 
        ' lblMachiCharaTotalCount
        ' 
        lblMachiCharaTotalCount.Dock = DockStyle.Bottom
        lblMachiCharaTotalCount.Location = New Point(2, 534)
        lblMachiCharaTotalCount.Name = "lblMachiCharaTotalCount"
        lblMachiCharaTotalCount.Size = New Size(395, 32)
        lblMachiCharaTotalCount.TabIndex = 1
        lblMachiCharaTotalCount.Text = "Total: 0"
        lblMachiCharaTotalCount.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' chkbxShaderGlass
        ' 
        chkbxShaderGlass.AutoSize = True
        chkbxShaderGlass.Enabled = False
        chkbxShaderGlass.FlatStyle = FlatStyle.Flat
        chkbxShaderGlass.Font = New Font("Segoe UI", 9F)
        chkbxShaderGlass.Location = New Point(84, 21)
        chkbxShaderGlass.Margin = New Padding(2)
        chkbxShaderGlass.Name = "chkbxShaderGlass"
        chkbxShaderGlass.Size = New Size(86, 19)
        chkbxShaderGlass.TabIndex = 2
        chkbxShaderGlass.Text = "ShaderGlass"
        chkbxShaderGlass.UseVisualStyleBackColor = True
        ' 
        ' GroupBox1
        ' 
        GroupBox1.Controls.Add(Panel1)
        GroupBox1.Controls.Add(ListViewGamesVariants)
        GroupBox1.Controls.Add(cbxFilterType)
        GroupBox1.Controls.Add(txtLVSearch)
        GroupBox1.Controls.Add(ListViewGames)
        GroupBox1.FlatStyle = FlatStyle.Flat
        GroupBox1.Location = New Point(6, 5)
        GroupBox1.Margin = New Padding(2)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Padding = New Padding(2)
        GroupBox1.Size = New Size(465, 567)
        GroupBox1.TabIndex = 0
        GroupBox1.TabStop = False
        GroupBox1.Text = "Available Appli"
        ' 
        ' Panel1
        ' 
        Panel1.Controls.Add(lblTotalVariantCount)
        Panel1.Controls.Add(lblFilteredGameCount)
        Panel1.Controls.Add(lblTotalGameCount)
        Panel1.Dock = DockStyle.Bottom
        Panel1.Location = New Point(2, 540)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(461, 25)
        Panel1.TabIndex = 11
        ' 
        ' lblTotalVariantCount
        ' 
        lblTotalVariantCount.FlatStyle = FlatStyle.Flat
        lblTotalVariantCount.Location = New Point(246, 0)
        lblTotalVariantCount.Margin = New Padding(2, 0, 2, 0)
        lblTotalVariantCount.Name = "lblTotalVariantCount"
        lblTotalVariantCount.Size = New Size(120, 25)
        lblTotalVariantCount.TabIndex = 12
        lblTotalVariantCount.Text = "Variants: 0"
        lblTotalVariantCount.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' lblFilteredGameCount
        ' 
        lblFilteredGameCount.FlatStyle = FlatStyle.Flat
        lblFilteredGameCount.Location = New Point(122, 0)
        lblFilteredGameCount.Margin = New Padding(2, 0, 2, 0)
        lblFilteredGameCount.Name = "lblFilteredGameCount"
        lblFilteredGameCount.Size = New Size(120, 25)
        lblFilteredGameCount.TabIndex = 11
        lblFilteredGameCount.Text = "Filtered: 0"
        lblFilteredGameCount.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' lblTotalGameCount
        ' 
        lblTotalGameCount.FlatStyle = FlatStyle.Flat
        lblTotalGameCount.Location = New Point(-2, 0)
        lblTotalGameCount.Margin = New Padding(2, 0, 2, 0)
        lblTotalGameCount.Name = "lblTotalGameCount"
        lblTotalGameCount.Size = New Size(120, 25)
        lblTotalGameCount.TabIndex = 1
        lblTotalGameCount.Text = "Total: 0"
        lblTotalGameCount.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' ListViewGamesVariants
        ' 
        ListViewGamesVariants.FullRowSelect = True
        ListViewGamesVariants.HeaderStyle = ColumnHeaderStyle.Nonclickable
        ListViewGamesVariants.LargeImageList = ImageListGames
        ListViewGamesVariants.Location = New Point(2, 420)
        ListViewGamesVariants.Margin = New Padding(2)
        ListViewGamesVariants.Name = "ListViewGamesVariants"
        ListViewGamesVariants.Size = New Size(461, 115)
        ListViewGamesVariants.TabIndex = 6
        ListViewGamesVariants.UseCompatibleStateImageBehavior = False
        ListViewGamesVariants.View = View.Details
        ' 
        ' cbxFilterType
        ' 
        cbxFilterType.DropDownStyle = ComboBoxStyle.DropDownList
        cbxFilterType.FlatStyle = FlatStyle.Flat
        cbxFilterType.FormattingEnabled = True
        cbxFilterType.Items.AddRange(New Object() {"All", "Favorites", "Installed", "Custom", "Fan-Translations", "Doja", "Star", "JSky", "Flash"})
        cbxFilterType.Location = New Point(340, 20)
        cbxFilterType.Margin = New Padding(2)
        cbxFilterType.Name = "cbxFilterType"
        cbxFilterType.Size = New Size(120, 23)
        cbxFilterType.TabIndex = 9
        ' 
        ' txtLVSearch
        ' 
        txtLVSearch.ForeColor = SystemColors.ControlText
        txtLVSearch.Location = New Point(5, 19)
        txtLVSearch.Margin = New Padding(2)
        txtLVSearch.Name = "txtLVSearch"
        txtLVSearch.PlaceholderText = "Search"
        txtLVSearch.Size = New Size(331, 23)
        txtLVSearch.TabIndex = 7
        ' 
        ' ListViewGames
        ' 
        ListViewGames.Activation = ItemActivation.OneClick
        ListViewGames.ContextMenuStrip = cmsGameLV
        ListViewGames.FullRowSelect = True
        ListViewGames.HeaderStyle = ColumnHeaderStyle.Nonclickable
        ListViewGames.LargeImageList = ImageListGames
        ListViewGames.Location = New Point(2, 47)
        ListViewGames.Margin = New Padding(2)
        ListViewGames.Name = "ListViewGames"
        ListViewGames.Size = New Size(461, 371)
        ListViewGames.TabIndex = 6
        ListViewGames.UseCompatibleStateImageBehavior = False
        ListViewGames.View = View.Details
        ' 
        ' gbxGameInfo
        ' 
        gbxGameInfo.Controls.Add(panelDynamic)
        gbxGameInfo.FlatStyle = FlatStyle.Flat
        gbxGameInfo.Location = New Point(475, 5)
        gbxGameInfo.Margin = New Padding(2)
        gbxGameInfo.Name = "gbxGameInfo"
        gbxGameInfo.Padding = New Padding(2)
        gbxGameInfo.Size = New Size(553, 420)
        gbxGameInfo.TabIndex = 1
        gbxGameInfo.TabStop = False
        gbxGameInfo.Text = "Appli Info"
        ' 
        ' panelDynamic
        ' 
        panelDynamic.AutoScroll = True
        panelDynamic.Controls.Add(pbGameDL)
        panelDynamic.Dock = DockStyle.Fill
        panelDynamic.Location = New Point(2, 18)
        panelDynamic.Name = "panelDynamic"
        panelDynamic.Size = New Size(549, 400)
        panelDynamic.TabIndex = 2
        ' 
        ' pbGameDL
        ' 
        pbGameDL.Location = New Point(2, 194)
        pbGameDL.Margin = New Padding(2)
        pbGameDL.Name = "pbGameDL"
        pbGameDL.Size = New Size(543, 65)
        pbGameDL.TabIndex = 1
        pbGameDL.Visible = False
        ' 
        ' btnLaunchGame
        ' 
        btnLaunchGame.Enabled = False
        btnLaunchGame.FlatStyle = FlatStyle.Flat
        btnLaunchGame.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        btnLaunchGame.Location = New Point(5, 21)
        btnLaunchGame.Margin = New Padding(2)
        btnLaunchGame.Name = "btnLaunchGame"
        btnLaunchGame.Size = New Size(75, 75)
        btnLaunchGame.TabIndex = 1
        btnLaunchGame.Text = "Launch ▶"
        btnLaunchGame.UseVisualStyleBackColor = True
        ' 
        ' MaterialTabControl1
        ' 
        MaterialTabControl1.Controls.Add(tpAppli)
        MaterialTabControl1.Controls.Add(tpMachiChara)
        MaterialTabControl1.Controls.Add(tpConfig)
        MaterialTabControl1.Controls.Add(tpHelp)
        MaterialTabControl1.Depth = 0
        MaterialTabControl1.Font = New Font("Segoe UI", 9F)
        MaterialTabControl1.Location = New Point(0, 95)
        MaterialTabControl1.Margin = New Padding(0)
        MaterialTabControl1.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER
        MaterialTabControl1.Multiline = True
        MaterialTabControl1.Name = "MaterialTabControl1"
        MaterialTabControl1.SelectedIndex = 0
        MaterialTabControl1.Size = New Size(1041, 609)
        MaterialTabControl1.TabIndex = 6
        ' 
        ' tpAppli
        ' 
        tpAppli.Controls.Add(GroupBox3)
        tpAppli.Controls.Add(gbxGameInfo)
        tpAppli.Controls.Add(GroupBox1)
        tpAppli.Location = New Point(4, 24)
        tpAppli.Name = "tpAppli"
        tpAppli.Padding = New Padding(3)
        tpAppli.Size = New Size(1033, 581)
        tpAppli.TabIndex = 0
        tpAppli.Text = "Appli"
        tpAppli.UseVisualStyleBackColor = True
        ' 
        ' GroupBox3
        ' 
        GroupBox3.Controls.Add(chkbxDialpadNumpad)
        GroupBox3.Controls.Add(chkboxControllerVibration)
        GroupBox3.Controls.Add(cbxControllerProfile)
        GroupBox3.Controls.Add(chkbxEnableController)
        GroupBox3.Controls.Add(cbxGameControllers)
        GroupBox3.Controls.Add(Label2)
        GroupBox3.Controls.Add(btnLaunchGame)
        GroupBox3.Controls.Add(cbxShaderGlassScaling)
        GroupBox3.Controls.Add(chkbxLocalEmulator)
        GroupBox3.Controls.Add(chkbxShaderGlass)
        GroupBox3.Location = New Point(475, 430)
        GroupBox3.Name = "GroupBox3"
        GroupBox3.Size = New Size(552, 142)
        GroupBox3.TabIndex = 2
        GroupBox3.TabStop = False
        GroupBox3.Text = "Launch Settings"
        ' 
        ' chkbxDialpadNumpad
        ' 
        chkbxDialpadNumpad.AutoSize = True
        chkbxDialpadNumpad.Checked = True
        chkbxDialpadNumpad.CheckState = CheckState.Checked
        chkbxDialpadNumpad.Enabled = False
        chkbxDialpadNumpad.FlatStyle = FlatStyle.Flat
        chkbxDialpadNumpad.Location = New Point(84, 70)
        chkbxDialpadNumpad.Name = "chkbxDialpadNumpad"
        chkbxDialpadNumpad.Size = New Size(113, 19)
        chkbxDialpadNumpad.TabIndex = 16
        chkbxDialpadNumpad.Text = "Dialpad Numpad"
        chkbxDialpadNumpad.UseVisualStyleBackColor = True
        ' 
        ' chkboxControllerVibration
        ' 
        chkboxControllerVibration.Enabled = False
        chkboxControllerVibration.FlatStyle = FlatStyle.Flat
        chkboxControllerVibration.Location = New Point(467, 24)
        chkboxControllerVibration.Name = "chkboxControllerVibration"
        chkboxControllerVibration.Size = New Size(79, 19)
        chkboxControllerVibration.TabIndex = 15
        chkboxControllerVibration.Text = "Vibration"
        chkboxControllerVibration.UseVisualStyleBackColor = True
        ' 
        ' cbxControllerProfile
        ' 
        cbxControllerProfile.DropDownStyle = ComboBoxStyle.DropDownList
        cbxControllerProfile.Enabled = False
        cbxControllerProfile.FlatStyle = FlatStyle.Flat
        cbxControllerProfile.Font = New Font("Segoe UI", 9F)
        cbxControllerProfile.FormattingEnabled = True
        cbxControllerProfile.Location = New Point(337, 75)
        cbxControllerProfile.Margin = New Padding(2)
        cbxControllerProfile.Name = "cbxControllerProfile"
        cbxControllerProfile.Size = New Size(210, 23)
        cbxControllerProfile.TabIndex = 7
        ' 
        ' chkbxEnableController
        ' 
        chkbxEnableController.Enabled = False
        chkbxEnableController.FlatStyle = FlatStyle.Flat
        chkbxEnableController.Location = New Point(337, 24)
        chkbxEnableController.Name = "chkbxEnableController"
        chkbxEnableController.Size = New Size(125, 19)
        chkbxEnableController.TabIndex = 5
        chkbxEnableController.Text = "Enable Controller"
        chkbxEnableController.UseVisualStyleBackColor = True
        ' 
        ' cbxGameControllers
        ' 
        cbxGameControllers.DropDownStyle = ComboBoxStyle.DropDownList
        cbxGameControllers.Enabled = False
        cbxGameControllers.FlatStyle = FlatStyle.Flat
        cbxGameControllers.Font = New Font("Segoe UI", 9F)
        cbxGameControllers.FormattingEnabled = True
        cbxGameControllers.Location = New Point(337, 48)
        cbxGameControllers.Margin = New Padding(2)
        cbxGameControllers.Name = "cbxGameControllers"
        cbxGameControllers.Size = New Size(210, 23)
        cbxGameControllers.TabIndex = 6
        ' 
        ' Label2
        ' 
        Label2.FlatStyle = FlatStyle.Flat
        Label2.Font = New Font("Segoe UI", 9F)
        Label2.Location = New Point(337, 100)
        Label2.Margin = New Padding(2, 0, 2, 0)
        Label2.Name = "Label2"
        Label2.Size = New Size(120, 38)
        Label2.TabIndex = 14
        Label2.Text = "ShaderGlass Scaling"
        Label2.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' cbxShaderGlassScaling
        ' 
        cbxShaderGlassScaling.DropDownStyle = ComboBoxStyle.DropDownList
        cbxShaderGlassScaling.Enabled = False
        cbxShaderGlassScaling.FlatStyle = FlatStyle.Flat
        cbxShaderGlassScaling.FormattingEnabled = True
        cbxShaderGlassScaling.Items.AddRange(New Object() {"1x", "1.5x", "2x", "2.5x", "3x", "3.5x", "4x"})
        cbxShaderGlassScaling.Location = New Point(461, 109)
        cbxShaderGlassScaling.Margin = New Padding(2)
        cbxShaderGlassScaling.Name = "cbxShaderGlassScaling"
        cbxShaderGlassScaling.Size = New Size(85, 23)
        cbxShaderGlassScaling.TabIndex = 11
        ' 
        ' chkbxLocalEmulator
        ' 
        chkbxLocalEmulator.AutoSize = True
        chkbxLocalEmulator.Checked = True
        chkbxLocalEmulator.CheckState = CheckState.Checked
        chkbxLocalEmulator.Enabled = False
        chkbxLocalEmulator.FlatStyle = FlatStyle.Flat
        chkbxLocalEmulator.Location = New Point(84, 45)
        chkbxLocalEmulator.Name = "chkbxLocalEmulator"
        chkbxLocalEmulator.Size = New Size(108, 19)
        chkbxLocalEmulator.TabIndex = 4
        chkbxLocalEmulator.Text = "Locale Emulator"
        chkbxLocalEmulator.UseVisualStyleBackColor = True
        ' 
        ' tpMachiChara
        ' 
        tpMachiChara.Controls.Add(chkboxMachiCharaLocalEmulator)
        tpMachiChara.Controls.Add(btnMachiCharaLaunch)
        tpMachiChara.Controls.Add(GroupBox2)
        tpMachiChara.Location = New Point(4, 24)
        tpMachiChara.Name = "tpMachiChara"
        tpMachiChara.Padding = New Padding(3)
        tpMachiChara.Size = New Size(1033, 581)
        tpMachiChara.TabIndex = 1
        tpMachiChara.Text = "MachiChara"
        tpMachiChara.UseVisualStyleBackColor = True
        ' 
        ' chkboxMachiCharaLocalEmulator
        ' 
        chkboxMachiCharaLocalEmulator.AutoSize = True
        chkboxMachiCharaLocalEmulator.FlatStyle = FlatStyle.Flat
        chkboxMachiCharaLocalEmulator.Location = New Point(416, 85)
        chkboxMachiCharaLocalEmulator.Name = "chkboxMachiCharaLocalEmulator"
        chkboxMachiCharaLocalEmulator.Size = New Size(108, 19)
        chkboxMachiCharaLocalEmulator.TabIndex = 4
        chkboxMachiCharaLocalEmulator.Text = "Locale Emulator"
        chkboxMachiCharaLocalEmulator.UseVisualStyleBackColor = True
        ' 
        ' tpConfig
        ' 
        tpConfig.Controls.Add(GroupBox6)
        tpConfig.Controls.Add(GroupBox5)
        tpConfig.Controls.Add(GroupBox4)
        tpConfig.Location = New Point(4, 24)
        tpConfig.Name = "tpConfig"
        tpConfig.Size = New Size(1033, 581)
        tpConfig.TabIndex = 2
        tpConfig.Text = "Config"
        tpConfig.UseVisualStyleBackColor = True
        ' 
        ' GroupBox6
        ' 
        GroupBox6.Controls.Add(btnSaveDataManagement)
        GroupBox6.Controls.Add(btnAddCustomApps)
        GroupBox6.Controls.Add(btnUpdateNetworkUID)
        GroupBox6.Controls.Add(btnLoadShaderGlassConfig)
        GroupBox6.Controls.Add(btnLaunchAppConfig)
        GroupBox6.Controls.Add(btnLaunchKey2Pad)
        GroupBox6.Location = New Point(3, 3)
        GroupBox6.Name = "GroupBox6"
        GroupBox6.Size = New Size(557, 84)
        GroupBox6.TabIndex = 17
        GroupBox6.TabStop = False
        GroupBox6.Text = "General Config"
        ' 
        ' btnSaveDataManagement
        ' 
        btnSaveDataManagement.Location = New Point(6, 22)
        btnSaveDataManagement.Name = "btnSaveDataManagement"
        btnSaveDataManagement.Size = New Size(93, 55)
        btnSaveDataManagement.TabIndex = 5
        btnSaveDataManagement.Text = "Save Data Management"
        btnSaveDataManagement.UseVisualStyleBackColor = True
        ' 
        ' btnAddCustomApps
        ' 
        btnAddCustomApps.Location = New Point(191, 22)
        btnAddCustomApps.Name = "btnAddCustomApps"
        btnAddCustomApps.Size = New Size(80, 55)
        btnAddCustomApps.TabIndex = 4
        btnAddCustomApps.Text = "Add Custom Apps"
        btnAddCustomApps.UseVisualStyleBackColor = True
        ' 
        ' btnUpdateNetworkUID
        ' 
        btnUpdateNetworkUID.Location = New Point(105, 22)
        btnUpdateNetworkUID.Name = "btnUpdateNetworkUID"
        btnUpdateNetworkUID.Size = New Size(80, 55)
        btnUpdateNetworkUID.TabIndex = 3
        btnUpdateNetworkUID.Text = "Update NetworkUID"
        btnUpdateNetworkUID.UseVisualStyleBackColor = True
        ' 
        ' btnLoadShaderGlassConfig
        ' 
        btnLoadShaderGlassConfig.Location = New Point(449, 22)
        btnLoadShaderGlassConfig.Name = "btnLoadShaderGlassConfig"
        btnLoadShaderGlassConfig.Size = New Size(80, 55)
        btnLoadShaderGlassConfig.TabIndex = 2
        btnLoadShaderGlassConfig.Text = "ShaderGlass Config"
        btnLoadShaderGlassConfig.UseVisualStyleBackColor = True
        ' 
        ' btnLaunchAppConfig
        ' 
        btnLaunchAppConfig.Location = New Point(363, 22)
        btnLaunchAppConfig.Name = "btnLaunchAppConfig"
        btnLaunchAppConfig.Size = New Size(80, 55)
        btnLaunchAppConfig.TabIndex = 1
        btnLaunchAppConfig.Text = "AppConfig"
        btnLaunchAppConfig.UseVisualStyleBackColor = True
        ' 
        ' btnLaunchKey2Pad
        ' 
        btnLaunchKey2Pad.Location = New Point(277, 22)
        btnLaunchKey2Pad.Name = "btnLaunchKey2Pad"
        btnLaunchKey2Pad.Size = New Size(80, 55)
        btnLaunchKey2Pad.TabIndex = 0
        btnLaunchKey2Pad.Text = "Key2Pad App"
        btnLaunchKey2Pad.UseVisualStyleBackColor = True
        ' 
        ' GroupBox5
        ' 
        GroupBox5.Controls.Add(chkbxHidePhoneUI)
        GroupBox5.Controls.Add(lblAudioWarning)
        GroupBox5.Controls.Add(Label1)
        GroupBox5.Controls.Add(cobxAudioType)
        GroupBox5.Location = New Point(245, 93)
        GroupBox5.Name = "GroupBox5"
        GroupBox5.Size = New Size(315, 93)
        GroupBox5.TabIndex = 16
        GroupBox5.TabStop = False
        GroupBox5.Text = "Doja/Star SDK Options"
        ' 
        ' chkbxHidePhoneUI
        ' 
        chkbxHidePhoneUI.AutoSize = True
        chkbxHidePhoneUI.FlatStyle = FlatStyle.Flat
        chkbxHidePhoneUI.Font = New Font("Segoe UI", 9F)
        chkbxHidePhoneUI.Location = New Point(5, 49)
        chkbxHidePhoneUI.Margin = New Padding(2)
        chkbxHidePhoneUI.Name = "chkbxHidePhoneUI"
        chkbxHidePhoneUI.Size = New Size(99, 19)
        chkbxHidePhoneUI.TabIndex = 15
        chkbxHidePhoneUI.Text = "Hide Phone UI"
        chkbxHidePhoneUI.UseVisualStyleBackColor = True
        ' 
        ' lblAudioWarning
        ' 
        lblAudioWarning.AutoSize = True
        lblAudioWarning.FlatStyle = FlatStyle.Flat
        lblAudioWarning.Font = New Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        lblAudioWarning.ForeColor = Color.Firebrick
        lblAudioWarning.Location = New Point(184, 28)
        lblAudioWarning.Margin = New Padding(2, 0, 2, 0)
        lblAudioWarning.Name = "lblAudioWarning"
        lblAudioWarning.Size = New Size(120, 13)
        lblAudioWarning.TabIndex = 14
        lblAudioWarning.Text = "⚠ Performance Issues"
        lblAudioWarning.TextAlign = ContentAlignment.TopCenter
        lblAudioWarning.Visible = False
        ' 
        ' Label1
        ' 
        Label1.FlatStyle = FlatStyle.Flat
        Label1.Font = New Font("Segoe UI", 9F)
        Label1.Location = New Point(5, 22)
        Label1.Margin = New Padding(2, 0, 2, 0)
        Label1.Name = "Label1"
        Label1.Size = New Size(85, 23)
        Label1.TabIndex = 12
        Label1.Text = "Audio Type"
        Label1.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' cobxAudioType
        ' 
        cobxAudioType.DropDownStyle = ComboBoxStyle.DropDownList
        cobxAudioType.FlatStyle = FlatStyle.Flat
        cobxAudioType.Font = New Font("Segoe UI", 9F)
        cobxAudioType.FormattingEnabled = True
        cobxAudioType.Items.AddRange(New Object() {"Standard", "903i"})
        cobxAudioType.Location = New Point(95, 22)
        cobxAudioType.Margin = New Padding(2)
        cobxAudioType.Name = "cobxAudioType"
        cobxAudioType.Size = New Size(85, 23)
        cobxAudioType.TabIndex = 13
        ' 
        ' GroupBox4
        ' 
        GroupBox4.Controls.Add(cbxFlashSDK)
        GroupBox4.Controls.Add(Label7)
        GroupBox4.Controls.Add(Label6)
        GroupBox4.Controls.Add(cbxJSKYSDK)
        GroupBox4.Controls.Add(Label5)
        GroupBox4.Controls.Add(Label3)
        GroupBox4.Controls.Add(cbxStarSDK)
        GroupBox4.Controls.Add(cbxDojaSDK)
        GroupBox4.Controls.Add(Label4)
        GroupBox4.Location = New Point(3, 93)
        GroupBox4.Name = "GroupBox4"
        GroupBox4.Size = New Size(236, 469)
        GroupBox4.TabIndex = 15
        GroupBox4.TabStop = False
        GroupBox4.Text = "SDK Configuration"
        ' 
        ' cbxFlashSDK
        ' 
        cbxFlashSDK.DropDownStyle = ComboBoxStyle.DropDownList
        cbxFlashSDK.FlatStyle = FlatStyle.Flat
        cbxFlashSDK.Font = New Font("Segoe UI", 9F)
        cbxFlashSDK.FormattingEnabled = True
        cbxFlashSDK.Location = New Point(117, 141)
        cbxFlashSDK.Margin = New Padding(2)
        cbxFlashSDK.Name = "cbxFlashSDK"
        cbxFlashSDK.Size = New Size(113, 23)
        cbxFlashSDK.TabIndex = 18
        ' 
        ' Label7
        ' 
        Label7.FlatStyle = FlatStyle.Flat
        Label7.Font = New Font("Segoe UI", 9F)
        Label7.Location = New Point(6, 141)
        Label7.Margin = New Padding(2, 0, 2, 0)
        Label7.Name = "Label7"
        Label7.Size = New Size(106, 23)
        Label7.TabIndex = 19
        Label7.Text = "Flash"
        Label7.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' Label6
        ' 
        Label6.Location = New Point(6, 22)
        Label6.Name = "Label6"
        Label6.Size = New Size(224, 46)
        Label6.TabIndex = 17
        Label6.Text = "Please select the verion of SDK you wish to use to load appli with." & vbCrLf
        Label6.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' cbxJSKYSDK
        ' 
        cbxJSKYSDK.DropDownStyle = ComboBoxStyle.DropDownList
        cbxJSKYSDK.FlatStyle = FlatStyle.Flat
        cbxJSKYSDK.Font = New Font("Segoe UI", 9F)
        cbxJSKYSDK.FormattingEnabled = True
        cbxJSKYSDK.Location = New Point(117, 118)
        cbxJSKYSDK.Margin = New Padding(2)
        cbxJSKYSDK.Name = "cbxJSKYSDK"
        cbxJSKYSDK.Size = New Size(113, 23)
        cbxJSKYSDK.TabIndex = 15
        ' 
        ' Label5
        ' 
        Label5.FlatStyle = FlatStyle.Flat
        Label5.Font = New Font("Segoe UI", 9F)
        Label5.Location = New Point(6, 118)
        Label5.Margin = New Padding(2, 0, 2, 0)
        Label5.Name = "Label5"
        Label5.Size = New Size(106, 23)
        Label5.TabIndex = 16
        Label5.Text = "JSky SDK"
        Label5.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' Label3
        ' 
        Label3.FlatStyle = FlatStyle.Flat
        Label3.Font = New Font("Segoe UI", 9F)
        Label3.Location = New Point(6, 70)
        Label3.Margin = New Padding(2, 0, 2, 0)
        Label3.Name = "Label3"
        Label3.Size = New Size(106, 23)
        Label3.TabIndex = 11
        Label3.Text = "Doja SDK"
        Label3.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' cbxStarSDK
        ' 
        cbxStarSDK.DropDownStyle = ComboBoxStyle.DropDownList
        cbxStarSDK.FlatStyle = FlatStyle.Flat
        cbxStarSDK.Font = New Font("Segoe UI", 9F)
        cbxStarSDK.FormattingEnabled = True
        cbxStarSDK.Location = New Point(117, 93)
        cbxStarSDK.Margin = New Padding(2)
        cbxStarSDK.Name = "cbxStarSDK"
        cbxStarSDK.Size = New Size(113, 23)
        cbxStarSDK.TabIndex = 13
        ' 
        ' cbxDojaSDK
        ' 
        cbxDojaSDK.DropDownStyle = ComboBoxStyle.DropDownList
        cbxDojaSDK.FlatStyle = FlatStyle.Flat
        cbxDojaSDK.Font = New Font("Segoe UI", 9F)
        cbxDojaSDK.FormattingEnabled = True
        cbxDojaSDK.Location = New Point(116, 70)
        cbxDojaSDK.Margin = New Padding(2)
        cbxDojaSDK.Name = "cbxDojaSDK"
        cbxDojaSDK.Size = New Size(114, 23)
        cbxDojaSDK.TabIndex = 12
        ' 
        ' Label4
        ' 
        Label4.FlatStyle = FlatStyle.Flat
        Label4.Font = New Font("Segoe UI", 9F)
        Label4.Location = New Point(6, 93)
        Label4.Margin = New Padding(2, 0, 2, 0)
        Label4.Name = "Label4"
        Label4.Size = New Size(106, 23)
        Label4.TabIndex = 14
        Label4.Text = "Star SDK"
        Label4.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' tpHelp
        ' 
        tpHelp.Controls.Add(GroupBox8)
        tpHelp.Controls.Add(GroupBox7)
        tpHelp.Controls.Add(btnControls)
        tpHelp.Location = New Point(4, 24)
        tpHelp.Name = "tpHelp"
        tpHelp.Size = New Size(1033, 581)
        tpHelp.TabIndex = 3
        tpHelp.Text = "Help"
        tpHelp.UseVisualStyleBackColor = True
        ' 
        ' GroupBox8
        ' 
        GroupBox8.Controls.Add(lblHelp_troubleshooting)
        GroupBox8.Location = New Point(240, 3)
        GroupBox8.Name = "GroupBox8"
        GroupBox8.Size = New Size(478, 575)
        GroupBox8.TabIndex = 4
        GroupBox8.TabStop = False
        GroupBox8.Text = "Troubleshooting"
        ' 
        ' lblHelp_troubleshooting
        ' 
        lblHelp_troubleshooting.Dock = DockStyle.Fill
        lblHelp_troubleshooting.Location = New Point(3, 19)
        lblHelp_troubleshooting.Name = "lblHelp_troubleshooting"
        lblHelp_troubleshooting.Size = New Size(472, 553)
        lblHelp_troubleshooting.TabIndex = 0
        lblHelp_troubleshooting.Text = "Label8"
        ' 
        ' GroupBox7
        ' 
        GroupBox7.Controls.Add(lblHelp_AppVer)
        GroupBox7.Location = New Point(3, 3)
        GroupBox7.Name = "GroupBox7"
        GroupBox7.Size = New Size(231, 97)
        GroupBox7.TabIndex = 3
        GroupBox7.TabStop = False
        GroupBox7.Text = "About App"
        ' 
        ' lblHelp_AppVer
        ' 
        lblHelp_AppVer.Dock = DockStyle.Fill
        lblHelp_AppVer.Location = New Point(3, 19)
        lblHelp_AppVer.Name = "lblHelp_AppVer"
        lblHelp_AppVer.Size = New Size(225, 75)
        lblHelp_AppVer.TabIndex = 0
        lblHelp_AppVer.Text = "App Version"
        lblHelp_AppVer.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' btnControls
        ' 
        btnControls.Location = New Point(6, 106)
        btnControls.Name = "btnControls"
        btnControls.Size = New Size(225, 72)
        btnControls.TabIndex = 2
        btnControls.Text = "Keybinds Controls"
        btnControls.UseVisualStyleBackColor = True
        ' 
        ' MaterialTabSelector1
        ' 
        MaterialTabSelector1.BaseTabControl = MaterialTabControl1
        MaterialTabSelector1.CharacterCasing = ReaLTaiizor.Controls.MaterialTabSelector.CustomCharacterCasing.Normal
        MaterialTabSelector1.Depth = 0
        MaterialTabSelector1.Dock = DockStyle.Top
        MaterialTabSelector1.Font = New Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel)
        MaterialTabSelector1.HeadAlignment = ReaLTaiizor.Controls.MaterialTabSelector.Alignment.Left
        MaterialTabSelector1.Location = New Point(0, 63)
        MaterialTabSelector1.Margin = New Padding(0)
        MaterialTabSelector1.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER
        MaterialTabSelector1.Name = "MaterialTabSelector1"
        MaterialTabSelector1.Size = New Size(1040, 27)
        MaterialTabSelector1.TabIndex = 7
        MaterialTabSelector1.Text = "MaterialTabSelector1"
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(96F, 96F)
        AutoScaleMode = AutoScaleMode.Dpi
        ClientSize = New Size(1040, 710)
        Controls.Add(MaterialTabSelector1)
        Controls.Add(MaterialTabControl1)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Margin = New Padding(2)
        MaximizeBox = False
        Name = "Form1"
        Padding = New Padding(0, 63, 0, 0)
        Sizable = False
        StartPosition = FormStartPosition.CenterScreen
        Text = "Keitai World Launcher"
        cmsGameLV.ResumeLayout(False)
        GroupBox2.ResumeLayout(False)
        cmsMachiCharaLV.ResumeLayout(False)
        GroupBox1.ResumeLayout(False)
        GroupBox1.PerformLayout()
        Panel1.ResumeLayout(False)
        gbxGameInfo.ResumeLayout(False)
        panelDynamic.ResumeLayout(False)
        MaterialTabControl1.ResumeLayout(False)
        tpAppli.ResumeLayout(False)
        GroupBox3.ResumeLayout(False)
        GroupBox3.PerformLayout()
        tpMachiChara.ResumeLayout(False)
        tpMachiChara.PerformLayout()
        tpConfig.ResumeLayout(False)
        GroupBox6.ResumeLayout(False)
        GroupBox5.ResumeLayout(False)
        GroupBox5.PerformLayout()
        GroupBox4.ResumeLayout(False)
        tpHelp.ResumeLayout(False)
        GroupBox8.ResumeLayout(False)
        GroupBox7.ResumeLayout(False)
        ResumeLayout(False)
    End Sub
    Friend WithEvents OpenFileDialog1 As OpenFileDialog
    Friend WithEvents ImageListGames As ImageList
    Friend WithEvents cmsGameLV As ContextMenuStrip
    Friend WithEvents cmsGameLV_Launch As ToolStripMenuItem
    Friend WithEvents cmsGameLV_Download As ToolStripMenuItem
    Friend WithEvents cmsGameLV_Delete As ToolStripMenuItem
    Friend WithEvents cmsGameLV_Favorite As ToolStripMenuItem
    Friend WithEvents selectionTimer As Timer
    Friend WithEvents FolderBrowserDialog1 As FolderBrowserDialog
    Friend WithEvents btnMachiCharaLaunch As Button
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents chkbxShaderGlass As CheckBox
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents ListViewGamesVariants As ListView
    Friend WithEvents cbxFilterType As ComboBox
    Friend WithEvents ListViewGames As ListView
    Friend WithEvents lblTotalGameCount As Label
    Friend WithEvents gbxGameInfo As GroupBox
    Friend WithEvents pbGameDL As ProgressBar
    Friend WithEvents btnLaunchGame As Button
    Friend WithEvents txtLVSearch As TextBox
    Friend WithEvents lblFilteredGameCount As Label
    Friend WithEvents MaterialTabControl1 As ReaLTaiizor.Controls.MaterialTabControl
    Friend WithEvents tpAppli As TabPage
    Friend WithEvents tpMachiChara As TabPage
    Friend WithEvents panelDynamic As Panel
    Friend WithEvents chkbxLocalEmulator As CheckBox
    Friend WithEvents cbxShaderGlassScaling As ComboBox
    Friend WithEvents Label2 As Label
    Friend WithEvents OpenGameFolderToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents Panel1 As Panel
    Friend WithEvents lblTotalVariantCount As Label
    Friend WithEvents lblMachiCharaTotalCount As Label
    Friend WithEvents cmsMachiCharaLV As ContextMenuStrip
    Friend WithEvents DownloadCMS_MachiChara As ToolStripMenuItem
    Friend WithEvents DeleteCMS_MachiChara As ToolStripMenuItem
    Friend WithEvents ListViewMachiChara As ListView
    Friend WithEvents chkboxMachiCharaLocalEmulator As CheckBox
    Friend WithEvents chkbxEnableController As CheckBox
    Friend WithEvents cbxGameControllers As ComboBox
    Friend WithEvents cbxControllerProfile As ComboBox
    Friend WithEvents chkboxControllerVibration As CheckBox
    Friend WithEvents tpConfig As TabPage
    Friend WithEvents GroupBox4 As GroupBox
    Friend WithEvents Label6 As Label
    Friend WithEvents cbxJSKYSDK As ComboBox
    Friend WithEvents Label5 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents cbxStarSDK As ComboBox
    Friend WithEvents cbxDojaSDK As ComboBox
    Friend WithEvents Label4 As Label
    Friend WithEvents GroupBox5 As GroupBox
    Friend WithEvents lblAudioWarning As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents cobxAudioType As ComboBox
    Friend WithEvents chkbxHidePhoneUI As CheckBox
    Friend WithEvents GroupBox6 As GroupBox
    Friend WithEvents btnLaunchKey2Pad As Button
    Friend WithEvents btnLoadShaderGlassConfig As Button
    Friend WithEvents btnLaunchAppConfig As Button
    Friend WithEvents btnUpdateNetworkUID As Button
    Friend WithEvents MaterialTabSelector1 As ReaLTaiizor.Controls.MaterialTabSelector
    Friend WithEvents tpHelp As TabPage
    Friend WithEvents btnControls As Button
    Friend WithEvents btnAddCustomApps As Button
    Friend WithEvents cbxFlashSDK As ComboBox
    Friend WithEvents Label7 As Label
    Friend WithEvents GroupBox7 As GroupBox
    Friend WithEvents GroupBox8 As GroupBox
    Friend WithEvents lblHelp_AppVer As Label
    Friend WithEvents lblHelp_troubleshooting As Label
    Friend WithEvents BackupSaveToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents btnSaveDataManagement As Button
    Friend WithEvents chkbxDialpadNumpad As CheckBox
End Class
