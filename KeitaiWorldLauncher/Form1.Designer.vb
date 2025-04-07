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
        MenuStrip1 = New MenuStrip()
        FileToolStripMenuItem = New ToolStripMenuItem()
        RefreshToolStripMenuItem = New ToolStripMenuItem()
        ExitToolStripMenuItem = New ToolStripMenuItem()
        ConfigToolStripMenuItem1 = New ToolStripMenuItem()
        KeyConfiguratorToolStripMenuItem = New ToolStripMenuItem()
        ShaderGlassConfigToolStripMenuItem = New ToolStripMenuItem()
        AppConfigToolStripMenuItem = New ToolStripMenuItem()
        NetworkUIDConfigToolStripMenuItem = New ToolStripMenuItem()
        ToolsToolStripMenuItem = New ToolStripMenuItem()
        BatchDownloadToolStripMenuItem = New ToolStripMenuItem()
        GamesToolStripMenuItem = New ToolStripMenuItem()
        MachiCharaToolStripMenuItem = New ToolStripMenuItem()
        XMLCreationToolStripMenuItem = New ToolStripMenuItem()
        GamelistToolStripMenuItem = New ToolStripMenuItem()
        XMLUpdateToolStripMenuItem = New ToolStripMenuItem()
        AddGameToolStripMenuItem = New ToolStripMenuItem()
        AddGamesToolStripMenuItem = New ToolStripMenuItem()
        HelpToolStripMenuItem = New ToolStripMenuItem()
        ControlsToolStripMenuItem = New ToolStripMenuItem()
        AboutToolStripMenuItem = New ToolStripMenuItem()
        OpenFileDialog1 = New OpenFileDialog()
        selectionTimer = New Timer(components)
        FolderBrowserDialog1 = New FolderBrowserDialog()
        btnMachiCharaLaunch = New Button()
        GroupBox2 = New GroupBox()
        lbxMachiCharaList = New ListBox()
        lblAudioWarning = New Label()
        Label4 = New Label()
        Label3 = New Label()
        cbxStarSDK = New ComboBox()
        cbxDojaSDK = New ComboBox()
        chkbxShaderGlass = New CheckBox()
        Label1 = New Label()
        cobxAudioType = New ComboBox()
        GroupBox1 = New GroupBox()
        lblFilteredGameCount = New Label()
        ListViewGamesVariants = New ListView()
        cbxFilterType = New ComboBox()
        txtLVSearch = New TextBox()
        ListViewGames = New ListView()
        lblTotalGameCount = New Label()
        chkbxHidePhoneUI = New CheckBox()
        gbxGameInfo = New GroupBox()
        panelDynamic = New Panel()
        pbGameDL = New ProgressBar()
        btnLaunchGame = New Button()
        MaterialTabControl1 = New ReaLTaiizor.Controls.MaterialTabControl()
        tpGames = New TabPage()
        cbxShaderGlassScaling = New ComboBox()
        chkbxLocalEmulator = New CheckBox()
        tpMachiChara = New TabPage()
        OpenGameFolderToolStripMenuItem = New ToolStripMenuItem()
        cmsGameLV.SuspendLayout()
        MenuStrip1.SuspendLayout()
        GroupBox2.SuspendLayout()
        GroupBox1.SuspendLayout()
        gbxGameInfo.SuspendLayout()
        panelDynamic.SuspendLayout()
        MaterialTabControl1.SuspendLayout()
        tpGames.SuspendLayout()
        tpMachiChara.SuspendLayout()
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
        cmsGameLV.Items.AddRange(New ToolStripItem() {cmsGameLV_Launch, cmsGameLV_Download, cmsGameLV_Delete, cmsGameLV_Favorite, OpenGameFolderToolStripMenuItem})
        cmsGameLV.Name = "cmsGameLV"
        cmsGameLV.Size = New Size(181, 136)
        ' 
        ' cmsGameLV_Launch
        ' 
        cmsGameLV_Launch.Name = "cmsGameLV_Launch"
        cmsGameLV_Launch.Size = New Size(180, 22)
        cmsGameLV_Launch.Text = "Launch"
        ' 
        ' cmsGameLV_Download
        ' 
        cmsGameLV_Download.Name = "cmsGameLV_Download"
        cmsGameLV_Download.Size = New Size(180, 22)
        cmsGameLV_Download.Text = "Download"
        ' 
        ' cmsGameLV_Delete
        ' 
        cmsGameLV_Delete.Name = "cmsGameLV_Delete"
        cmsGameLV_Delete.Size = New Size(180, 22)
        cmsGameLV_Delete.Text = "Delete"
        ' 
        ' cmsGameLV_Favorite
        ' 
        cmsGameLV_Favorite.Name = "cmsGameLV_Favorite"
        cmsGameLV_Favorite.Size = New Size(180, 22)
        cmsGameLV_Favorite.Text = "Favorite"
        ' 
        ' MenuStrip1
        ' 
        MenuStrip1.BackColor = Color.Gainsboro
        MenuStrip1.Font = New Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        MenuStrip1.ImageScalingSize = New Size(20, 20)
        MenuStrip1.Items.AddRange(New ToolStripItem() {FileToolStripMenuItem, ConfigToolStripMenuItem1, ToolsToolStripMenuItem, HelpToolStripMenuItem})
        MenuStrip1.Location = New Point(0, 65)
        MenuStrip1.Name = "MenuStrip1"
        MenuStrip1.Padding = New Padding(5, 2, 0, 2)
        MenuStrip1.Size = New Size(1047, 27)
        MenuStrip1.TabIndex = 5
        MenuStrip1.Text = "MenuStrip1"
        ' 
        ' FileToolStripMenuItem
        ' 
        FileToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {RefreshToolStripMenuItem, ExitToolStripMenuItem})
        FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        FileToolStripMenuItem.Size = New Size(41, 23)
        FileToolStripMenuItem.Text = "File"
        ' 
        ' RefreshToolStripMenuItem
        ' 
        RefreshToolStripMenuItem.Name = "RefreshToolStripMenuItem"
        RefreshToolStripMenuItem.Size = New Size(123, 24)
        RefreshToolStripMenuItem.Text = "Refresh"
        ' 
        ' ExitToolStripMenuItem
        ' 
        ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        ExitToolStripMenuItem.Size = New Size(123, 24)
        ExitToolStripMenuItem.Text = "Exit"
        ' 
        ' ConfigToolStripMenuItem1
        ' 
        ConfigToolStripMenuItem1.DropDownItems.AddRange(New ToolStripItem() {KeyConfiguratorToolStripMenuItem, ShaderGlassConfigToolStripMenuItem, AppConfigToolStripMenuItem, NetworkUIDConfigToolStripMenuItem})
        ConfigToolStripMenuItem1.Name = "ConfigToolStripMenuItem1"
        ConfigToolStripMenuItem1.Size = New Size(61, 23)
        ConfigToolStripMenuItem1.Text = "Config"
        ' 
        ' KeyConfiguratorToolStripMenuItem
        ' 
        KeyConfiguratorToolStripMenuItem.Name = "KeyConfiguratorToolStripMenuItem"
        KeyConfiguratorToolStripMenuItem.Size = New Size(213, 24)
        KeyConfiguratorToolStripMenuItem.Text = "Key2Pad Configurator"
        ' 
        ' ShaderGlassConfigToolStripMenuItem
        ' 
        ShaderGlassConfigToolStripMenuItem.Name = "ShaderGlassConfigToolStripMenuItem"
        ShaderGlassConfigToolStripMenuItem.Size = New Size(213, 24)
        ShaderGlassConfigToolStripMenuItem.Text = "ShaderGlass Config"
        ' 
        ' AppConfigToolStripMenuItem
        ' 
        AppConfigToolStripMenuItem.Name = "AppConfigToolStripMenuItem"
        AppConfigToolStripMenuItem.Size = New Size(213, 24)
        AppConfigToolStripMenuItem.Text = "Application Config"
        ' 
        ' NetworkUIDConfigToolStripMenuItem
        ' 
        NetworkUIDConfigToolStripMenuItem.Name = "NetworkUIDConfigToolStripMenuItem"
        NetworkUIDConfigToolStripMenuItem.Size = New Size(213, 24)
        NetworkUIDConfigToolStripMenuItem.Text = "Network UID Config"
        ' 
        ' ToolsToolStripMenuItem
        ' 
        ToolsToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {BatchDownloadToolStripMenuItem, XMLCreationToolStripMenuItem, XMLUpdateToolStripMenuItem, AddGamesToolStripMenuItem})
        ToolsToolStripMenuItem.Name = "ToolsToolStripMenuItem"
        ToolsToolStripMenuItem.Size = New Size(52, 23)
        ToolsToolStripMenuItem.Text = "Tools"
        ' 
        ' BatchDownloadToolStripMenuItem
        ' 
        BatchDownloadToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {GamesToolStripMenuItem, MachiCharaToolStripMenuItem})
        BatchDownloadToolStripMenuItem.Enabled = False
        BatchDownloadToolStripMenuItem.Name = "BatchDownloadToolStripMenuItem"
        BatchDownloadToolStripMenuItem.Size = New Size(178, 24)
        BatchDownloadToolStripMenuItem.Text = "Batch Download"
        BatchDownloadToolStripMenuItem.Visible = False
        ' 
        ' GamesToolStripMenuItem
        ' 
        GamesToolStripMenuItem.Name = "GamesToolStripMenuItem"
        GamesToolStripMenuItem.Size = New Size(155, 24)
        GamesToolStripMenuItem.Text = "Games"
        ' 
        ' MachiCharaToolStripMenuItem
        ' 
        MachiCharaToolStripMenuItem.Name = "MachiCharaToolStripMenuItem"
        MachiCharaToolStripMenuItem.Size = New Size(155, 24)
        MachiCharaToolStripMenuItem.Text = "Machi Chara"
        ' 
        ' XMLCreationToolStripMenuItem
        ' 
        XMLCreationToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {GamelistToolStripMenuItem})
        XMLCreationToolStripMenuItem.Enabled = False
        XMLCreationToolStripMenuItem.Name = "XMLCreationToolStripMenuItem"
        XMLCreationToolStripMenuItem.Size = New Size(178, 24)
        XMLCreationToolStripMenuItem.Text = "XML Creation"
        XMLCreationToolStripMenuItem.Visible = False
        ' 
        ' GamelistToolStripMenuItem
        ' 
        GamelistToolStripMenuItem.Name = "GamelistToolStripMenuItem"
        GamelistToolStripMenuItem.Size = New Size(131, 24)
        GamelistToolStripMenuItem.Text = "Gamelist"
        ' 
        ' XMLUpdateToolStripMenuItem
        ' 
        XMLUpdateToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {AddGameToolStripMenuItem})
        XMLUpdateToolStripMenuItem.Enabled = False
        XMLUpdateToolStripMenuItem.Name = "XMLUpdateToolStripMenuItem"
        XMLUpdateToolStripMenuItem.Size = New Size(178, 24)
        XMLUpdateToolStripMenuItem.Text = "XML Update"
        XMLUpdateToolStripMenuItem.Visible = False
        ' 
        ' AddGameToolStripMenuItem
        ' 
        AddGameToolStripMenuItem.Name = "AddGameToolStripMenuItem"
        AddGameToolStripMenuItem.Size = New Size(143, 24)
        AddGameToolStripMenuItem.Text = "Add Game"
        ' 
        ' AddGamesToolStripMenuItem
        ' 
        AddGamesToolStripMenuItem.Name = "AddGamesToolStripMenuItem"
        AddGamesToolStripMenuItem.Size = New Size(178, 24)
        AddGamesToolStripMenuItem.Text = "Add Games"
        ' 
        ' HelpToolStripMenuItem
        ' 
        HelpToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {ControlsToolStripMenuItem, AboutToolStripMenuItem})
        HelpToolStripMenuItem.Name = "HelpToolStripMenuItem"
        HelpToolStripMenuItem.Size = New Size(49, 23)
        HelpToolStripMenuItem.Text = "Help"
        ' 
        ' ControlsToolStripMenuItem
        ' 
        ControlsToolStripMenuItem.Name = "ControlsToolStripMenuItem"
        ControlsToolStripMenuItem.Size = New Size(130, 24)
        ControlsToolStripMenuItem.Text = "Controls"
        ' 
        ' AboutToolStripMenuItem
        ' 
        AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        AboutToolStripMenuItem.Size = New Size(130, 24)
        AboutToolStripMenuItem.Text = "About"
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
        btnMachiCharaLaunch.Location = New Point(257, 28)
        btnMachiCharaLaunch.Margin = New Padding(2)
        btnMachiCharaLaunch.Name = "btnMachiCharaLaunch"
        btnMachiCharaLaunch.Size = New Size(110, 54)
        btnMachiCharaLaunch.TabIndex = 3
        btnMachiCharaLaunch.Text = "Launch"
        btnMachiCharaLaunch.UseVisualStyleBackColor = True
        ' 
        ' GroupBox2
        ' 
        GroupBox2.Controls.Add(lbxMachiCharaList)
        GroupBox2.Location = New Point(12, 8)
        GroupBox2.Margin = New Padding(2)
        GroupBox2.Name = "GroupBox2"
        GroupBox2.Padding = New Padding(2)
        GroupBox2.Size = New Size(239, 486)
        GroupBox2.TabIndex = 1
        GroupBox2.TabStop = False
        GroupBox2.Text = "Available Machi Chara"
        ' 
        ' lbxMachiCharaList
        ' 
        lbxMachiCharaList.Enabled = False
        lbxMachiCharaList.FormattingEnabled = True
        lbxMachiCharaList.ItemHeight = 15
        lbxMachiCharaList.Location = New Point(5, 22)
        lbxMachiCharaList.Margin = New Padding(2)
        lbxMachiCharaList.Name = "lbxMachiCharaList"
        lbxMachiCharaList.Size = New Size(230, 409)
        lbxMachiCharaList.TabIndex = 0
        ' 
        ' lblAudioWarning
        ' 
        lblAudioWarning.FlatStyle = FlatStyle.Flat
        lblAudioWarning.Font = New Font("Segoe UI", 9F)
        lblAudioWarning.ForeColor = Color.Firebrick
        lblAudioWarning.Location = New Point(719, 514)
        lblAudioWarning.Margin = New Padding(2, 0, 2, 0)
        lblAudioWarning.Name = "lblAudioWarning"
        lblAudioWarning.Size = New Size(145, 50)
        lblAudioWarning.TabIndex = 11
        lblAudioWarning.Text = "Possible Performance Issues"
        lblAudioWarning.TextAlign = ContentAlignment.TopCenter
        lblAudioWarning.Visible = False
        ' 
        ' Label4
        ' 
        Label4.FlatStyle = FlatStyle.Flat
        Label4.Font = New Font("Segoe UI", 9F)
        Label4.Location = New Point(886, 517)
        Label4.Margin = New Padding(2, 0, 2, 0)
        Label4.Name = "Label4"
        Label4.Size = New Size(51, 23)
        Label4.TabIndex = 10
        Label4.Text = "Star SDK"
        Label4.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' Label3
        ' 
        Label3.FlatStyle = FlatStyle.Flat
        Label3.Font = New Font("Segoe UI", 9F)
        Label3.Location = New Point(882, 489)
        Label3.Margin = New Padding(2, 0, 2, 0)
        Label3.Name = "Label3"
        Label3.Size = New Size(55, 23)
        Label3.TabIndex = 9
        Label3.Text = "Doja SDK"
        Label3.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' cbxStarSDK
        ' 
        cbxStarSDK.DropDownStyle = ComboBoxStyle.DropDownList
        cbxStarSDK.FlatStyle = FlatStyle.Flat
        cbxStarSDK.Font = New Font("Segoe UI", 9F)
        cbxStarSDK.FormattingEnabled = True
        cbxStarSDK.Location = New Point(941, 518)
        cbxStarSDK.Margin = New Padding(2)
        cbxStarSDK.Name = "cbxStarSDK"
        cbxStarSDK.Size = New Size(85, 23)
        cbxStarSDK.TabIndex = 8
        ' 
        ' cbxDojaSDK
        ' 
        cbxDojaSDK.DropDownStyle = ComboBoxStyle.DropDownList
        cbxDojaSDK.FlatStyle = FlatStyle.Flat
        cbxDojaSDK.Font = New Font("Segoe UI", 9F)
        cbxDojaSDK.FormattingEnabled = True
        cbxDojaSDK.Location = New Point(941, 489)
        cbxDojaSDK.Margin = New Padding(2)
        cbxDojaSDK.Name = "cbxDojaSDK"
        cbxDojaSDK.Size = New Size(85, 23)
        cbxDojaSDK.TabIndex = 7
        ' 
        ' chkbxShaderGlass
        ' 
        chkbxShaderGlass.AutoSize = True
        chkbxShaderGlass.Enabled = False
        chkbxShaderGlass.FlatStyle = FlatStyle.Flat
        chkbxShaderGlass.Font = New Font("Segoe UI", 9F)
        chkbxShaderGlass.Location = New Point(589, 504)
        chkbxShaderGlass.Margin = New Padding(2)
        chkbxShaderGlass.Name = "chkbxShaderGlass"
        chkbxShaderGlass.Size = New Size(86, 19)
        chkbxShaderGlass.TabIndex = 6
        chkbxShaderGlass.Text = "ShaderGlass"
        chkbxShaderGlass.UseVisualStyleBackColor = True
        ' 
        ' Label1
        ' 
        Label1.FlatStyle = FlatStyle.Flat
        Label1.Font = New Font("Segoe UI", 9F)
        Label1.Location = New Point(719, 489)
        Label1.Margin = New Padding(2, 0, 2, 0)
        Label1.Name = "Label1"
        Label1.Size = New Size(67, 23)
        Label1.TabIndex = 5
        Label1.Text = "Audio Type"
        Label1.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' cobxAudioType
        ' 
        cobxAudioType.DropDownStyle = ComboBoxStyle.DropDownList
        cobxAudioType.Enabled = False
        cobxAudioType.FlatStyle = FlatStyle.Flat
        cobxAudioType.Font = New Font("Segoe UI", 9F)
        cobxAudioType.FormattingEnabled = True
        cobxAudioType.Items.AddRange(New Object() {"Standard", "903i"})
        cobxAudioType.Location = New Point(790, 489)
        cobxAudioType.Margin = New Padding(2)
        cobxAudioType.Name = "cobxAudioType"
        cobxAudioType.Size = New Size(74, 23)
        cobxAudioType.TabIndex = 4
        ' 
        ' GroupBox1
        ' 
        GroupBox1.Controls.Add(lblFilteredGameCount)
        GroupBox1.Controls.Add(ListViewGamesVariants)
        GroupBox1.Controls.Add(cbxFilterType)
        GroupBox1.Controls.Add(txtLVSearch)
        GroupBox1.Controls.Add(ListViewGames)
        GroupBox1.Controls.Add(lblTotalGameCount)
        GroupBox1.FlatStyle = FlatStyle.Flat
        GroupBox1.Location = New Point(6, 5)
        GroupBox1.Margin = New Padding(2)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Padding = New Padding(2)
        GroupBox1.Size = New Size(465, 567)
        GroupBox1.TabIndex = 0
        GroupBox1.TabStop = False
        GroupBox1.Text = "Available Games"
        ' 
        ' lblFilteredGameCount
        ' 
        lblFilteredGameCount.FlatStyle = FlatStyle.Flat
        lblFilteredGameCount.Location = New Point(210, 543)
        lblFilteredGameCount.Margin = New Padding(2, 0, 2, 0)
        lblFilteredGameCount.Name = "lblFilteredGameCount"
        lblFilteredGameCount.Size = New Size(105, 22)
        lblFilteredGameCount.TabIndex = 11
        lblFilteredGameCount.Text = "Filtered: 0"
        lblFilteredGameCount.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' ListViewGamesVariants
        ' 
        ListViewGamesVariants.LargeImageList = ImageListGames
        ListViewGamesVariants.Location = New Point(5, 424)
        ListViewGamesVariants.Margin = New Padding(2)
        ListViewGamesVariants.Name = "ListViewGamesVariants"
        ListViewGamesVariants.Size = New Size(456, 117)
        ListViewGamesVariants.TabIndex = 10
        ListViewGamesVariants.UseCompatibleStateImageBehavior = False
        ListViewGamesVariants.View = View.SmallIcon
        ' 
        ' cbxFilterType
        ' 
        cbxFilterType.DropDownStyle = ComboBoxStyle.DropDownList
        cbxFilterType.FlatStyle = FlatStyle.Flat
        cbxFilterType.FormattingEnabled = True
        cbxFilterType.Items.AddRange(New Object() {"All", "Favorites", "Installed", "Custom", "Doja", "Star"})
        cbxFilterType.Location = New Point(360, 20)
        cbxFilterType.Margin = New Padding(2)
        cbxFilterType.Name = "cbxFilterType"
        cbxFilterType.Size = New Size(101, 23)
        cbxFilterType.TabIndex = 9
        ' 
        ' txtLVSearch
        ' 
        txtLVSearch.ForeColor = SystemColors.ControlText
        txtLVSearch.Location = New Point(5, 19)
        txtLVSearch.Margin = New Padding(2)
        txtLVSearch.Name = "txtLVSearch"
        txtLVSearch.PlaceholderText = "Search"
        txtLVSearch.Size = New Size(351, 23)
        txtLVSearch.TabIndex = 7
        ' 
        ' ListViewGames
        ' 
        ListViewGames.Activation = ItemActivation.OneClick
        ListViewGames.ContextMenuStrip = cmsGameLV
        ListViewGames.FullRowSelect = True
        ListViewGames.HeaderStyle = ColumnHeaderStyle.Nonclickable
        ListViewGames.LargeImageList = ImageListGames
        ListViewGames.Location = New Point(5, 50)
        ListViewGames.Margin = New Padding(2)
        ListViewGames.Name = "ListViewGames"
        ListViewGames.Size = New Size(456, 370)
        ListViewGames.TabIndex = 6
        ListViewGames.UseCompatibleStateImageBehavior = False
        ListViewGames.View = View.Details
        ' 
        ' lblTotalGameCount
        ' 
        lblTotalGameCount.FlatStyle = FlatStyle.Flat
        lblTotalGameCount.Location = New Point(5, 543)
        lblTotalGameCount.Margin = New Padding(2, 0, 2, 0)
        lblTotalGameCount.Name = "lblTotalGameCount"
        lblTotalGameCount.Size = New Size(105, 22)
        lblTotalGameCount.TabIndex = 1
        lblTotalGameCount.Text = "Total: 0"
        lblTotalGameCount.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' chkbxHidePhoneUI
        ' 
        chkbxHidePhoneUI.AutoSize = True
        chkbxHidePhoneUI.Enabled = False
        chkbxHidePhoneUI.FlatStyle = FlatStyle.Flat
        chkbxHidePhoneUI.Font = New Font("Segoe UI", 9F)
        chkbxHidePhoneUI.Location = New Point(589, 485)
        chkbxHidePhoneUI.Margin = New Padding(2)
        chkbxHidePhoneUI.Name = "chkbxHidePhoneUI"
        chkbxHidePhoneUI.Size = New Size(99, 19)
        chkbxHidePhoneUI.TabIndex = 3
        chkbxHidePhoneUI.Text = "Hide Phone UI"
        chkbxHidePhoneUI.UseVisualStyleBackColor = True
        ' 
        ' gbxGameInfo
        ' 
        gbxGameInfo.Controls.Add(panelDynamic)
        gbxGameInfo.FlatStyle = FlatStyle.Flat
        gbxGameInfo.Location = New Point(475, 5)
        gbxGameInfo.Margin = New Padding(2)
        gbxGameInfo.Name = "gbxGameInfo"
        gbxGameInfo.Padding = New Padding(2)
        gbxGameInfo.Size = New Size(553, 481)
        gbxGameInfo.TabIndex = 1
        gbxGameInfo.TabStop = False
        gbxGameInfo.Text = "Game Info"
        ' 
        ' panelDynamic
        ' 
        panelDynamic.AutoScroll = True
        panelDynamic.Controls.Add(pbGameDL)
        panelDynamic.Dock = DockStyle.Fill
        panelDynamic.Location = New Point(2, 18)
        panelDynamic.Name = "panelDynamic"
        panelDynamic.Size = New Size(549, 461)
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
        btnLaunchGame.Font = New Font("Segoe UI", 9F)
        btnLaunchGame.Location = New Point(475, 489)
        btnLaunchGame.Margin = New Padding(2)
        btnLaunchGame.Name = "btnLaunchGame"
        btnLaunchGame.Size = New Size(110, 77)
        btnLaunchGame.TabIndex = 2
        btnLaunchGame.Text = "Launch"
        btnLaunchGame.UseVisualStyleBackColor = True
        ' 
        ' MaterialTabControl1
        ' 
        MaterialTabControl1.Controls.Add(tpGames)
        MaterialTabControl1.Controls.Add(tpMachiChara)
        MaterialTabControl1.Depth = 0
        MaterialTabControl1.Location = New Point(3, 96)
        MaterialTabControl1.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER
        MaterialTabControl1.Multiline = True
        MaterialTabControl1.Name = "MaterialTabControl1"
        MaterialTabControl1.SelectedIndex = 0
        MaterialTabControl1.Size = New Size(1041, 609)
        MaterialTabControl1.TabIndex = 6
        ' 
        ' tpGames
        ' 
        tpGames.Controls.Add(cbxShaderGlassScaling)
        tpGames.Controls.Add(chkbxLocalEmulator)
        tpGames.Controls.Add(lblAudioWarning)
        tpGames.Controls.Add(gbxGameInfo)
        tpGames.Controls.Add(Label4)
        tpGames.Controls.Add(btnLaunchGame)
        tpGames.Controls.Add(Label3)
        tpGames.Controls.Add(chkbxHidePhoneUI)
        tpGames.Controls.Add(cbxStarSDK)
        tpGames.Controls.Add(GroupBox1)
        tpGames.Controls.Add(cbxDojaSDK)
        tpGames.Controls.Add(cobxAudioType)
        tpGames.Controls.Add(chkbxShaderGlass)
        tpGames.Controls.Add(Label1)
        tpGames.Location = New Point(4, 24)
        tpGames.Name = "tpGames"
        tpGames.Padding = New Padding(3)
        tpGames.Size = New Size(1033, 581)
        tpGames.TabIndex = 0
        tpGames.Text = "Games"
        tpGames.UseVisualStyleBackColor = True
        ' 
        ' cbxShaderGlassScaling
        ' 
        cbxShaderGlassScaling.Enabled = False
        cbxShaderGlassScaling.FormattingEnabled = True
        cbxShaderGlassScaling.Items.AddRange(New Object() {"1x", "1.5x", "2x", "2.5x", "3x", "3.5x", "4x"})
        cbxShaderGlassScaling.Location = New Point(590, 528)
        cbxShaderGlassScaling.Name = "cbxShaderGlassScaling"
        cbxShaderGlassScaling.Size = New Size(47, 23)
        cbxShaderGlassScaling.TabIndex = 13
        ' 
        ' chkbxLocalEmulator
        ' 
        chkbxLocalEmulator.Checked = True
        chkbxLocalEmulator.CheckState = CheckState.Checked
        chkbxLocalEmulator.Location = New Point(886, 547)
        chkbxLocalEmulator.Name = "chkbxLocalEmulator"
        chkbxLocalEmulator.Size = New Size(136, 24)
        chkbxLocalEmulator.TabIndex = 12
        chkbxLocalEmulator.Text = "Locale Emulator"
        chkbxLocalEmulator.UseVisualStyleBackColor = True
        ' 
        ' tpMachiChara
        ' 
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
        ' OpenGameFolderToolStripMenuItem
        ' 
        OpenGameFolderToolStripMenuItem.Name = "OpenGameFolderToolStripMenuItem"
        OpenGameFolderToolStripMenuItem.Size = New Size(180, 22)
        OpenGameFolderToolStripMenuItem.Text = "Open game folder"
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(96F, 96F)
        AutoScaleMode = AutoScaleMode.Dpi
        ClientSize = New Size(1047, 709)
        Controls.Add(MaterialTabControl1)
        Controls.Add(MenuStrip1)
        DrawerTabControl = MaterialTabControl1
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MainMenuStrip = MenuStrip1
        Margin = New Padding(2)
        MaximizeBox = False
        Name = "Form1"
        Padding = New Padding(0, 65, 0, 3)
        Sizable = False
        StartPosition = FormStartPosition.CenterScreen
        Text = "Keitai World Launcher"
        cmsGameLV.ResumeLayout(False)
        MenuStrip1.ResumeLayout(False)
        MenuStrip1.PerformLayout()
        GroupBox2.ResumeLayout(False)
        GroupBox1.ResumeLayout(False)
        GroupBox1.PerformLayout()
        gbxGameInfo.ResumeLayout(False)
        panelDynamic.ResumeLayout(False)
        MaterialTabControl1.ResumeLayout(False)
        tpGames.ResumeLayout(False)
        tpGames.PerformLayout()
        tpMachiChara.ResumeLayout(False)
        ResumeLayout(False)
        PerformLayout()
    End Sub
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents FileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents HelpToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BatchDownloadToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents GamesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MachiCharaToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents XMLCreationToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AboutToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents GamelistToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OpenFileDialog1 As OpenFileDialog
    Friend WithEvents ImageListGames As ImageList
    Friend WithEvents RefreshToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ConfigToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents AppConfigToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents KeyConfiguratorToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents cmsGameLV As ContextMenuStrip
    Friend WithEvents cmsGameLV_Launch As ToolStripMenuItem
    Friend WithEvents cmsGameLV_Download As ToolStripMenuItem
    Friend WithEvents cmsGameLV_Delete As ToolStripMenuItem
    Friend WithEvents XMLUpdateToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AddGameToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ShaderGlassConfigToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents cmsGameLV_Favorite As ToolStripMenuItem
    Friend WithEvents selectionTimer As Timer
    Friend WithEvents NetworkUIDConfigToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AddGamesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents FolderBrowserDialog1 As FolderBrowserDialog
    Friend WithEvents btnMachiCharaLaunch As Button
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents lbxMachiCharaList As ListBox
    Friend WithEvents Label4 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents cbxStarSDK As ComboBox
    Friend WithEvents cbxDojaSDK As ComboBox
    Friend WithEvents chkbxShaderGlass As CheckBox
    Friend WithEvents Label1 As Label
    Friend WithEvents cobxAudioType As ComboBox
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents ListViewGamesVariants As ListView
    Friend WithEvents cbxFilterType As ComboBox
    Friend WithEvents ListViewGames As ListView
    Friend WithEvents lblTotalGameCount As Label
    Friend WithEvents chkbxHidePhoneUI As CheckBox
    Friend WithEvents gbxGameInfo As GroupBox
    Friend WithEvents pbGameDL As ProgressBar
    Friend WithEvents btnLaunchGame As Button
    Friend WithEvents ControlsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents lblAudioWarning As Label
    Friend WithEvents txtLVSearch As TextBox
    Friend WithEvents lblFilteredGameCount As Label
    Friend WithEvents MaterialTabControl1 As ReaLTaiizor.Controls.MaterialTabControl
    Friend WithEvents tpGames As TabPage
    Friend WithEvents tpMachiChara As TabPage
    Friend WithEvents panelDynamic As Panel
    Friend WithEvents chkbxLocalEmulator As CheckBox
    Friend WithEvents cbxShaderGlassScaling As ComboBox
    Friend WithEvents Label2 As Label
    Friend WithEvents OpenGameFolderToolStripMenuItem As ToolStripMenuItem
End Class
