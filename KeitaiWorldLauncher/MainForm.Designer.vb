<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        ImageListGames = New ImageList(components)
        cmsGameLV = New ContextMenuStrip(components)
        cmsGameLV_Launch = New ToolStripMenuItem()
        cmsGameLV_Download = New ToolStripMenuItem()
        cmsGameLV_Delete = New ToolStripMenuItem()
        cmsGameLV_Favorite = New ToolStripMenuItem()
        BackupSaveToolStripMenuItem = New ToolStripMenuItem()
        OpenGameFolderToolStripMenuItem = New ToolStripMenuItem()
        cmsBombermanPuzzle = New ToolStripMenuItem()
        ImportStageToolStripMenuItem = New ToolStripMenuItem()
        tsmBPSImportStage1 = New ToolStripMenuItem()
        tsmBPSImportStage2 = New ToolStripMenuItem()
        tsmBPSImportStage3 = New ToolStripMenuItem()
        tsmBPSImportStage4 = New ToolStripMenuItem()
        tsmBPSImportStage5 = New ToolStripMenuItem()
        tsmBPSImportStage6 = New ToolStripMenuItem()
        tsmBPSImportStage7 = New ToolStripMenuItem()
        tsmBPSImportStage8 = New ToolStripMenuItem()
        tsmBPSImportStage9 = New ToolStripMenuItem()
        tsmBPSImportStage10 = New ToolStripMenuItem()
        ExportStageToolStripMenuItem = New ToolStripMenuItem()
        tsmBPSExportStage1 = New ToolStripMenuItem()
        tsmBPSExportStage2 = New ToolStripMenuItem()
        tsmBPSExportStage3 = New ToolStripMenuItem()
        tsmBPSExportStage4 = New ToolStripMenuItem()
        tsmBPSExportStage5 = New ToolStripMenuItem()
        tsmBPSExportStage6 = New ToolStripMenuItem()
        tsmBPSExportStage7 = New ToolStripMenuItem()
        tsmBPSExportStage8 = New ToolStripMenuItem()
        tsmBPSExportStage9 = New ToolStripMenuItem()
        tsmBPSExportStage10 = New ToolStripMenuItem()
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
        tpCharaDen = New TabPage()
        chkboxCharadenLocalEmulator = New CheckBox()
        btnCharaDenLaunch = New Button()
        GroupBox10 = New GroupBox()
        ListViewCharaDen = New ListView()
        cmsCharadenLV = New ContextMenuStrip(components)
        DownloadCMS_CharaDen = New ToolStripMenuItem()
        DeleteCMS_CharaDen = New ToolStripMenuItem()
        lblCharadenTotalCount = New Label()
        tpConfig = New TabPage()
        GroupBox11 = New GroupBox()
        lblInvalidTID = New Label()
        Label16 = New Label()
        txtCurrentTID = New TextBox()
        chkboxNetworkModifyURLS = New CheckBox()
        lblInvalidUID = New Label()
        Label14 = New Label()
        txtCurrentUID = New TextBox()
        Label13 = New Label()
        btnUpdateNetworkUID = New Button()
        GroupBox6 = New GroupBox()
        btnSaveDataManagement = New Button()
        btnAddCustomApps = New Button()
        btnLoadShaderGlassConfig = New Button()
        btnLaunchAppConfig = New Button()
        btnLaunchKey2Pad = New Button()
        GroupBox5 = New GroupBox()
        gbxSJMELaunchOptions = New GroupBox()
        Label10 = New Label()
        cbxSJMEScaling = New ComboBox()
        btnSJMEUpdate = New Button()
        lblSJMELaunchOptionsText = New Label()
        Label9 = New Label()
        cbxSJMELaunchOption = New ComboBox()
        chkbxHidePhoneUI = New CheckBox()
        lblAudioWarning = New Label()
        Label1 = New Label()
        cbxAudioType = New ComboBox()
        GroupBox4 = New GroupBox()
        cbxEZWebEZPlusSDK = New ComboBox()
        Label15 = New Label()
        cbxSoftbankSDK = New ComboBox()
        Label12 = New Label()
        cbxAirEdgeSDK = New ComboBox()
        Label11 = New Label()
        cbxVodafoneSDK = New ComboBox()
        Label8 = New Label()
        cbxFlashSDK = New ComboBox()
        Label7 = New Label()
        Label6 = New Label()
        cbxJSKYSDK = New ComboBox()
        Label5 = New Label()
        Label3 = New Label()
        cbxStarSDK = New ComboBox()
        cbxDojaSDK = New ComboBox()
        Label4 = New Label()
        tpStats = New TabPage()
        GroupBox9 = New GroupBox()
        lvwPlaytimes = New ListView()
        tpHelp = New TabPage()
        btnVisitKeitaiArchive = New Button()
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
        tpCharaDen.SuspendLayout()
        GroupBox10.SuspendLayout()
        cmsCharadenLV.SuspendLayout()
        tpConfig.SuspendLayout()
        GroupBox11.SuspendLayout()
        GroupBox6.SuspendLayout()
        GroupBox5.SuspendLayout()
        gbxSJMELaunchOptions.SuspendLayout()
        GroupBox4.SuspendLayout()
        tpStats.SuspendLayout()
        GroupBox9.SuspendLayout()
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
        cmsGameLV.Items.AddRange(New ToolStripItem() {cmsGameLV_Launch, cmsGameLV_Download, cmsGameLV_Delete, cmsGameLV_Favorite, BackupSaveToolStripMenuItem, OpenGameFolderToolStripMenuItem, cmsBombermanPuzzle})
        cmsGameLV.Name = "cmsGameLV"
        cmsGameLV.Size = New Size(248, 158)
        ' 
        ' cmsGameLV_Launch
        ' 
        cmsGameLV_Launch.Name = "cmsGameLV_Launch"
        cmsGameLV_Launch.Size = New Size(247, 22)
        cmsGameLV_Launch.Text = "Launch"
        ' 
        ' cmsGameLV_Download
        ' 
        cmsGameLV_Download.Name = "cmsGameLV_Download"
        cmsGameLV_Download.Size = New Size(247, 22)
        cmsGameLV_Download.Text = "Download"
        ' 
        ' cmsGameLV_Delete
        ' 
        cmsGameLV_Delete.Name = "cmsGameLV_Delete"
        cmsGameLV_Delete.Size = New Size(247, 22)
        cmsGameLV_Delete.Text = "Delete"
        ' 
        ' cmsGameLV_Favorite
        ' 
        cmsGameLV_Favorite.Name = "cmsGameLV_Favorite"
        cmsGameLV_Favorite.Size = New Size(247, 22)
        cmsGameLV_Favorite.Text = "Favorite"
        ' 
        ' BackupSaveToolStripMenuItem
        ' 
        BackupSaveToolStripMenuItem.Name = "BackupSaveToolStripMenuItem"
        BackupSaveToolStripMenuItem.Size = New Size(247, 22)
        BackupSaveToolStripMenuItem.Text = "Backup Save (SP)"
        ' 
        ' OpenGameFolderToolStripMenuItem
        ' 
        OpenGameFolderToolStripMenuItem.Name = "OpenGameFolderToolStripMenuItem"
        OpenGameFolderToolStripMenuItem.Size = New Size(247, 22)
        OpenGameFolderToolStripMenuItem.Text = "Open game folder"
        ' 
        ' cmsBombermanPuzzle
        ' 
        cmsBombermanPuzzle.DropDownItems.AddRange(New ToolStripItem() {ImportStageToolStripMenuItem, ExportStageToolStripMenuItem})
        cmsBombermanPuzzle.Name = "cmsBombermanPuzzle"
        cmsBombermanPuzzle.Size = New Size(247, 22)
        cmsBombermanPuzzle.Text = "Bomberman Puzzle Special Tools"
        cmsBombermanPuzzle.Visible = False
        ' 
        ' ImportStageToolStripMenuItem
        ' 
        ImportStageToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {tsmBPSImportStage1, tsmBPSImportStage2, tsmBPSImportStage3, tsmBPSImportStage4, tsmBPSImportStage5, tsmBPSImportStage6, tsmBPSImportStage7, tsmBPSImportStage8, tsmBPSImportStage9, tsmBPSImportStage10})
        ImportStageToolStripMenuItem.Name = "ImportStageToolStripMenuItem"
        ImportStageToolStripMenuItem.Size = New Size(142, 22)
        ImportStageToolStripMenuItem.Text = "Import Stage"
        ' 
        ' tsmBPSImportStage1
        ' 
        tsmBPSImportStage1.Name = "tsmBPSImportStage1"
        tsmBPSImportStage1.Size = New Size(118, 22)
        tsmBPSImportStage1.Text = "Stage 1"
        ' 
        ' tsmBPSImportStage2
        ' 
        tsmBPSImportStage2.Name = "tsmBPSImportStage2"
        tsmBPSImportStage2.Size = New Size(118, 22)
        tsmBPSImportStage2.Text = "Stage 2"
        ' 
        ' tsmBPSImportStage3
        ' 
        tsmBPSImportStage3.Name = "tsmBPSImportStage3"
        tsmBPSImportStage3.Size = New Size(118, 22)
        tsmBPSImportStage3.Text = "Stage 3"
        ' 
        ' tsmBPSImportStage4
        ' 
        tsmBPSImportStage4.Name = "tsmBPSImportStage4"
        tsmBPSImportStage4.Size = New Size(118, 22)
        tsmBPSImportStage4.Text = "Stage 4"
        ' 
        ' tsmBPSImportStage5
        ' 
        tsmBPSImportStage5.Name = "tsmBPSImportStage5"
        tsmBPSImportStage5.Size = New Size(118, 22)
        tsmBPSImportStage5.Text = "Stage 5"
        ' 
        ' tsmBPSImportStage6
        ' 
        tsmBPSImportStage6.Name = "tsmBPSImportStage6"
        tsmBPSImportStage6.Size = New Size(118, 22)
        tsmBPSImportStage6.Text = "Stage 6"
        ' 
        ' tsmBPSImportStage7
        ' 
        tsmBPSImportStage7.Name = "tsmBPSImportStage7"
        tsmBPSImportStage7.Size = New Size(118, 22)
        tsmBPSImportStage7.Text = "Stage 7"
        ' 
        ' tsmBPSImportStage8
        ' 
        tsmBPSImportStage8.Name = "tsmBPSImportStage8"
        tsmBPSImportStage8.Size = New Size(118, 22)
        tsmBPSImportStage8.Text = "Stage 8"
        ' 
        ' tsmBPSImportStage9
        ' 
        tsmBPSImportStage9.Name = "tsmBPSImportStage9"
        tsmBPSImportStage9.Size = New Size(118, 22)
        tsmBPSImportStage9.Text = "Stage 9"
        ' 
        ' tsmBPSImportStage10
        ' 
        tsmBPSImportStage10.Name = "tsmBPSImportStage10"
        tsmBPSImportStage10.Size = New Size(118, 22)
        tsmBPSImportStage10.Text = "Stage 10"
        ' 
        ' ExportStageToolStripMenuItem
        ' 
        ExportStageToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {tsmBPSExportStage1, tsmBPSExportStage2, tsmBPSExportStage3, tsmBPSExportStage4, tsmBPSExportStage5, tsmBPSExportStage6, tsmBPSExportStage7, tsmBPSExportStage8, tsmBPSExportStage9, tsmBPSExportStage10})
        ExportStageToolStripMenuItem.Name = "ExportStageToolStripMenuItem"
        ExportStageToolStripMenuItem.Size = New Size(142, 22)
        ExportStageToolStripMenuItem.Text = "Export Stage"
        ' 
        ' tsmBPSExportStage1
        ' 
        tsmBPSExportStage1.Name = "tsmBPSExportStage1"
        tsmBPSExportStage1.Size = New Size(118, 22)
        tsmBPSExportStage1.Text = "Stage 1"
        ' 
        ' tsmBPSExportStage2
        ' 
        tsmBPSExportStage2.Name = "tsmBPSExportStage2"
        tsmBPSExportStage2.Size = New Size(118, 22)
        tsmBPSExportStage2.Text = "Stage 2"
        ' 
        ' tsmBPSExportStage3
        ' 
        tsmBPSExportStage3.Name = "tsmBPSExportStage3"
        tsmBPSExportStage3.Size = New Size(118, 22)
        tsmBPSExportStage3.Text = "Stage 3"
        ' 
        ' tsmBPSExportStage4
        ' 
        tsmBPSExportStage4.Name = "tsmBPSExportStage4"
        tsmBPSExportStage4.Size = New Size(118, 22)
        tsmBPSExportStage4.Text = "Stage 4"
        ' 
        ' tsmBPSExportStage5
        ' 
        tsmBPSExportStage5.Name = "tsmBPSExportStage5"
        tsmBPSExportStage5.Size = New Size(118, 22)
        tsmBPSExportStage5.Text = "Stage 5"
        ' 
        ' tsmBPSExportStage6
        ' 
        tsmBPSExportStage6.Name = "tsmBPSExportStage6"
        tsmBPSExportStage6.Size = New Size(118, 22)
        tsmBPSExportStage6.Text = "Stage 6"
        ' 
        ' tsmBPSExportStage7
        ' 
        tsmBPSExportStage7.Name = "tsmBPSExportStage7"
        tsmBPSExportStage7.Size = New Size(118, 22)
        tsmBPSExportStage7.Text = "Stage 7"
        ' 
        ' tsmBPSExportStage8
        ' 
        tsmBPSExportStage8.Name = "tsmBPSExportStage8"
        tsmBPSExportStage8.Size = New Size(118, 22)
        tsmBPSExportStage8.Text = "Stage 8"
        ' 
        ' tsmBPSExportStage9
        ' 
        tsmBPSExportStage9.Name = "tsmBPSExportStage9"
        tsmBPSExportStage9.Size = New Size(118, 22)
        tsmBPSExportStage9.Text = "Stage 9"
        ' 
        ' tsmBPSExportStage10
        ' 
        tsmBPSExportStage10.Name = "tsmBPSExportStage10"
        tsmBPSExportStage10.Size = New Size(118, 22)
        tsmBPSExportStage10.Text = "Stage 10"
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
        btnMachiCharaLaunch.Location = New Point(405, 20)
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
        GroupBox2.Location = New Point(2, 2)
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
        cbxFilterType.Items.AddRange(New Object() {"All", "Favorites", "Installed", "Custom", "Fan-Translations", "Doja", "Star", "JSky", "SoftBank", "AirEdge", "Vodafone", "EZplus", "Flash"})
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
        MaterialTabControl1.Controls.Add(tpCharaDen)
        MaterialTabControl1.Controls.Add(tpConfig)
        MaterialTabControl1.Controls.Add(tpStats)
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
        chkbxDialpadNumpad.Enabled = False
        chkbxDialpadNumpad.FlatStyle = FlatStyle.Flat
        chkbxDialpadNumpad.Location = New Point(84, 45)
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
        Label2.Location = New Point(337, 102)
        Label2.Margin = New Padding(2, 0, 2, 0)
        Label2.Name = "Label2"
        Label2.Size = New Size(120, 23)
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
        cbxShaderGlassScaling.Location = New Point(461, 102)
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
        chkbxLocalEmulator.Location = New Point(84, 70)
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
        chkboxMachiCharaLocalEmulator.Location = New Point(406, 79)
        chkboxMachiCharaLocalEmulator.Name = "chkboxMachiCharaLocalEmulator"
        chkboxMachiCharaLocalEmulator.Size = New Size(108, 19)
        chkboxMachiCharaLocalEmulator.TabIndex = 4
        chkboxMachiCharaLocalEmulator.Text = "Locale Emulator"
        chkboxMachiCharaLocalEmulator.UseVisualStyleBackColor = True
        ' 
        ' tpCharaDen
        ' 
        tpCharaDen.Controls.Add(chkboxCharadenLocalEmulator)
        tpCharaDen.Controls.Add(btnCharaDenLaunch)
        tpCharaDen.Controls.Add(GroupBox10)
        tpCharaDen.Location = New Point(4, 24)
        tpCharaDen.Name = "tpCharaDen"
        tpCharaDen.Size = New Size(1033, 581)
        tpCharaDen.TabIndex = 5
        tpCharaDen.Text = "Chara-Den"
        tpCharaDen.UseVisualStyleBackColor = True
        ' 
        ' chkboxCharadenLocalEmulator
        ' 
        chkboxCharadenLocalEmulator.AutoSize = True
        chkboxCharadenLocalEmulator.FlatStyle = FlatStyle.Flat
        chkboxCharadenLocalEmulator.Location = New Point(406, 79)
        chkboxCharadenLocalEmulator.Name = "chkboxCharadenLocalEmulator"
        chkboxCharadenLocalEmulator.Size = New Size(108, 19)
        chkboxCharadenLocalEmulator.TabIndex = 6
        chkboxCharadenLocalEmulator.Text = "Locale Emulator"
        chkboxCharadenLocalEmulator.UseVisualStyleBackColor = True
        ' 
        ' btnCharaDenLaunch
        ' 
        btnCharaDenLaunch.Enabled = False
        btnCharaDenLaunch.Location = New Point(405, 20)
        btnCharaDenLaunch.Margin = New Padding(2)
        btnCharaDenLaunch.Name = "btnCharaDenLaunch"
        btnCharaDenLaunch.Size = New Size(110, 54)
        btnCharaDenLaunch.TabIndex = 5
        btnCharaDenLaunch.Text = "Launch"
        btnCharaDenLaunch.UseVisualStyleBackColor = True
        ' 
        ' GroupBox10
        ' 
        GroupBox10.Controls.Add(ListViewCharaDen)
        GroupBox10.Controls.Add(lblCharadenTotalCount)
        GroupBox10.Location = New Point(2, 2)
        GroupBox10.Margin = New Padding(2)
        GroupBox10.Name = "GroupBox10"
        GroupBox10.Padding = New Padding(2)
        GroupBox10.Size = New Size(399, 568)
        GroupBox10.TabIndex = 2
        GroupBox10.TabStop = False
        GroupBox10.Text = "Available Chara-den"
        ' 
        ' ListViewCharaDen
        ' 
        ListViewCharaDen.Activation = ItemActivation.OneClick
        ListViewCharaDen.ContextMenuStrip = cmsCharadenLV
        ListViewCharaDen.Dock = DockStyle.Fill
        ListViewCharaDen.FullRowSelect = True
        ListViewCharaDen.HeaderStyle = ColumnHeaderStyle.Nonclickable
        ListViewCharaDen.Location = New Point(2, 18)
        ListViewCharaDen.Margin = New Padding(2)
        ListViewCharaDen.Name = "ListViewCharaDen"
        ListViewCharaDen.Size = New Size(395, 516)
        ListViewCharaDen.Sorting = SortOrder.Ascending
        ListViewCharaDen.TabIndex = 2
        ListViewCharaDen.UseCompatibleStateImageBehavior = False
        ListViewCharaDen.View = View.Details
        ' 
        ' cmsCharadenLV
        ' 
        cmsCharadenLV.Items.AddRange(New ToolStripItem() {DownloadCMS_CharaDen, DeleteCMS_CharaDen})
        cmsCharadenLV.Name = "cmsCharadenLV"
        cmsCharadenLV.Size = New Size(129, 48)
        ' 
        ' DownloadCMS_CharaDen
        ' 
        DownloadCMS_CharaDen.Name = "DownloadCMS_CharaDen"
        DownloadCMS_CharaDen.Size = New Size(128, 22)
        DownloadCMS_CharaDen.Text = "Download"
        ' 
        ' DeleteCMS_CharaDen
        ' 
        DeleteCMS_CharaDen.Name = "DeleteCMS_CharaDen"
        DeleteCMS_CharaDen.Size = New Size(128, 22)
        DeleteCMS_CharaDen.Text = "Delete"
        ' 
        ' lblCharadenTotalCount
        ' 
        lblCharadenTotalCount.Dock = DockStyle.Bottom
        lblCharadenTotalCount.Location = New Point(2, 534)
        lblCharadenTotalCount.Name = "lblCharadenTotalCount"
        lblCharadenTotalCount.Size = New Size(395, 32)
        lblCharadenTotalCount.TabIndex = 1
        lblCharadenTotalCount.Text = "Total: 0"
        lblCharadenTotalCount.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' tpConfig
        ' 
        tpConfig.Controls.Add(GroupBox11)
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
        ' GroupBox11
        ' 
        GroupBox11.Controls.Add(lblInvalidTID)
        GroupBox11.Controls.Add(Label16)
        GroupBox11.Controls.Add(txtCurrentTID)
        GroupBox11.Controls.Add(chkboxNetworkModifyURLS)
        GroupBox11.Controls.Add(lblInvalidUID)
        GroupBox11.Controls.Add(Label14)
        GroupBox11.Controls.Add(txtCurrentUID)
        GroupBox11.Controls.Add(Label13)
        GroupBox11.Controls.Add(btnUpdateNetworkUID)
        GroupBox11.Location = New Point(566, 3)
        GroupBox11.Name = "GroupBox11"
        GroupBox11.Size = New Size(314, 196)
        GroupBox11.TabIndex = 18
        GroupBox11.TabStop = False
        GroupBox11.Text = "Network Config"
        ' 
        ' lblInvalidTID
        ' 
        lblInvalidTID.ForeColor = Color.Firebrick
        lblInvalidTID.Location = New Point(87, 139)
        lblInvalidTID.Name = "lblInvalidTID"
        lblInvalidTID.Size = New Size(135, 23)
        lblInvalidTID.TabIndex = 10
        lblInvalidTID.Text = "Invalid TID"
        lblInvalidTID.TextAlign = ContentAlignment.TopCenter
        lblInvalidTID.Visible = False
        ' 
        ' Label16
        ' 
        Label16.Location = New Point(6, 112)
        Label16.Name = "Label16"
        Label16.Size = New Size(75, 23)
        Label16.TabIndex = 9
        Label16.Text = "Current TID:"
        Label16.TextAlign = ContentAlignment.MiddleRight
        ' 
        ' txtCurrentTID
        ' 
        txtCurrentTID.Location = New Point(87, 113)
        txtCurrentTID.Name = "txtCurrentTID"
        txtCurrentTID.ReadOnly = True
        txtCurrentTID.Size = New Size(135, 23)
        txtCurrentTID.TabIndex = 8
        ' 
        ' chkboxNetworkModifyURLS
        ' 
        chkboxNetworkModifyURLS.AutoSize = True
        chkboxNetworkModifyURLS.Checked = True
        chkboxNetworkModifyURLS.CheckState = CheckState.Checked
        chkboxNetworkModifyURLS.Location = New Point(6, 165)
        chkboxNetworkModifyURLS.Name = "chkboxNetworkModifyURLS"
        chkboxNetworkModifyURLS.Size = New Size(248, 19)
        chkboxNetworkModifyURLS.TabIndex = 7
        chkboxNetworkModifyURLS.Text = "Modify URL's for supported online games."
        chkboxNetworkModifyURLS.UseVisualStyleBackColor = True
        ' 
        ' lblInvalidUID
        ' 
        lblInvalidUID.ForeColor = Color.Firebrick
        lblInvalidUID.Location = New Point(87, 87)
        lblInvalidUID.Name = "lblInvalidUID"
        lblInvalidUID.Size = New Size(135, 23)
        lblInvalidUID.TabIndex = 6
        lblInvalidUID.Text = "Invalid UID"
        lblInvalidUID.TextAlign = ContentAlignment.TopCenter
        lblInvalidUID.Visible = False
        ' 
        ' Label14
        ' 
        Label14.Location = New Point(6, 60)
        Label14.Name = "Label14"
        Label14.Size = New Size(75, 23)
        Label14.TabIndex = 5
        Label14.Text = "Current UID:"
        Label14.TextAlign = ContentAlignment.MiddleRight
        ' 
        ' txtCurrentUID
        ' 
        txtCurrentUID.Location = New Point(87, 61)
        txtCurrentUID.Name = "txtCurrentUID"
        txtCurrentUID.ReadOnly = True
        txtCurrentUID.Size = New Size(135, 23)
        txtCurrentUID.TabIndex = 4
        ' 
        ' Label13
        ' 
        Label13.Location = New Point(6, 22)
        Label13.Name = "Label13"
        Label13.Size = New Size(278, 36)
        Label13.TabIndex = 0
        Label13.Text = "To access online features in supported games, please create a Network UID and Terminal ID."
        ' 
        ' btnUpdateNetworkUID
        ' 
        btnUpdateNetworkUID.Location = New Point(228, 61)
        btnUpdateNetworkUID.Name = "btnUpdateNetworkUID"
        btnUpdateNetworkUID.Size = New Size(80, 98)
        btnUpdateNetworkUID.TabIndex = 3
        btnUpdateNetworkUID.Text = "Update Network Config"
        btnUpdateNetworkUID.UseVisualStyleBackColor = True
        ' 
        ' GroupBox6
        ' 
        GroupBox6.Controls.Add(btnSaveDataManagement)
        GroupBox6.Controls.Add(btnAddCustomApps)
        GroupBox6.Controls.Add(btnLoadShaderGlassConfig)
        GroupBox6.Controls.Add(btnLaunchAppConfig)
        GroupBox6.Controls.Add(btnLaunchKey2Pad)
        GroupBox6.Location = New Point(3, 3)
        GroupBox6.Name = "GroupBox6"
        GroupBox6.Size = New Size(557, 121)
        GroupBox6.TabIndex = 17
        GroupBox6.TabStop = False
        GroupBox6.Text = "General Config"
        ' 
        ' btnSaveDataManagement
        ' 
        btnSaveDataManagement.Location = New Point(6, 22)
        btnSaveDataManagement.Name = "btnSaveDataManagement"
        btnSaveDataManagement.Size = New Size(201, 55)
        btnSaveDataManagement.TabIndex = 5
        btnSaveDataManagement.Text = "Save Data Management"
        btnSaveDataManagement.UseVisualStyleBackColor = True
        ' 
        ' btnAddCustomApps
        ' 
        btnAddCustomApps.Location = New Point(213, 22)
        btnAddCustomApps.Name = "btnAddCustomApps"
        btnAddCustomApps.Size = New Size(80, 55)
        btnAddCustomApps.TabIndex = 4
        btnAddCustomApps.Text = "Add Custom Apps"
        btnAddCustomApps.UseVisualStyleBackColor = True
        ' 
        ' btnLoadShaderGlassConfig
        ' 
        btnLoadShaderGlassConfig.Location = New Point(471, 22)
        btnLoadShaderGlassConfig.Name = "btnLoadShaderGlassConfig"
        btnLoadShaderGlassConfig.Size = New Size(80, 55)
        btnLoadShaderGlassConfig.TabIndex = 2
        btnLoadShaderGlassConfig.Text = "ShaderGlass Config"
        btnLoadShaderGlassConfig.UseVisualStyleBackColor = True
        ' 
        ' btnLaunchAppConfig
        ' 
        btnLaunchAppConfig.Location = New Point(385, 22)
        btnLaunchAppConfig.Name = "btnLaunchAppConfig"
        btnLaunchAppConfig.Size = New Size(80, 55)
        btnLaunchAppConfig.TabIndex = 1
        btnLaunchAppConfig.Text = "AppConfig"
        btnLaunchAppConfig.UseVisualStyleBackColor = True
        ' 
        ' btnLaunchKey2Pad
        ' 
        btnLaunchKey2Pad.Location = New Point(299, 22)
        btnLaunchKey2Pad.Name = "btnLaunchKey2Pad"
        btnLaunchKey2Pad.Size = New Size(80, 55)
        btnLaunchKey2Pad.TabIndex = 0
        btnLaunchKey2Pad.Text = "Key2Pad App"
        btnLaunchKey2Pad.UseVisualStyleBackColor = True
        ' 
        ' GroupBox5
        ' 
        GroupBox5.Controls.Add(gbxSJMELaunchOptions)
        GroupBox5.Controls.Add(chkbxHidePhoneUI)
        GroupBox5.Controls.Add(lblAudioWarning)
        GroupBox5.Controls.Add(Label1)
        GroupBox5.Controls.Add(cbxAudioType)
        GroupBox5.Location = New Point(245, 131)
        GroupBox5.Name = "GroupBox5"
        GroupBox5.Size = New Size(315, 262)
        GroupBox5.TabIndex = 16
        GroupBox5.TabStop = False
        GroupBox5.Text = "Doja/Star SDK Options"
        ' 
        ' gbxSJMELaunchOptions
        ' 
        gbxSJMELaunchOptions.Controls.Add(Label10)
        gbxSJMELaunchOptions.Controls.Add(cbxSJMEScaling)
        gbxSJMELaunchOptions.Controls.Add(btnSJMEUpdate)
        gbxSJMELaunchOptions.Controls.Add(lblSJMELaunchOptionsText)
        gbxSJMELaunchOptions.Controls.Add(Label9)
        gbxSJMELaunchOptions.Controls.Add(cbxSJMELaunchOption)
        gbxSJMELaunchOptions.Location = New Point(5, 93)
        gbxSJMELaunchOptions.Name = "gbxSJMELaunchOptions"
        gbxSJMELaunchOptions.Size = New Size(304, 162)
        gbxSJMELaunchOptions.TabIndex = 16
        gbxSJMELaunchOptions.TabStop = False
        gbxSJMELaunchOptions.Text = "SqurrielJME Emulator Config"
        ' 
        ' Label10
        ' 
        Label10.Location = New Point(6, 95)
        Label10.Name = "Label10"
        Label10.Size = New Size(86, 23)
        Label10.TabIndex = 8
        Label10.Text = "Scaling"
        Label10.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' cbxSJMEScaling
        ' 
        cbxSJMEScaling.FormattingEnabled = True
        cbxSJMEScaling.Items.AddRange(New Object() {"1", "2", "3", "4", "5", "6", "7", "8", "9", "10"})
        cbxSJMEScaling.Location = New Point(98, 95)
        cbxSJMEScaling.Name = "cbxSJMEScaling"
        cbxSJMEScaling.Size = New Size(77, 23)
        cbxSJMEScaling.TabIndex = 7
        ' 
        ' btnSJMEUpdate
        ' 
        btnSJMEUpdate.Location = New Point(6, 124)
        btnSJMEUpdate.Name = "btnSJMEUpdate"
        btnSJMEUpdate.Size = New Size(292, 33)
        btnSJMEUpdate.TabIndex = 6
        btnSJMEUpdate.Text = "Update SqurrielJME"
        btnSJMEUpdate.UseVisualStyleBackColor = True
        ' 
        ' lblSJMELaunchOptionsText
        ' 
        lblSJMELaunchOptionsText.Location = New Point(6, 52)
        lblSJMELaunchOptionsText.Name = "lblSJMELaunchOptionsText"
        lblSJMELaunchOptionsText.Size = New Size(292, 35)
        lblSJMELaunchOptionsText.TabIndex = 2
        lblSJMELaunchOptionsText.Text = "N/A"
        lblSJMELaunchOptionsText.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' Label9
        ' 
        Label9.Location = New Point(6, 26)
        Label9.Name = "Label9"
        Label9.Size = New Size(86, 23)
        Label9.TabIndex = 1
        Label9.Text = "Launch Option"
        Label9.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' cbxSJMELaunchOption
        ' 
        cbxSJMELaunchOption.FormattingEnabled = True
        cbxSJMELaunchOption.Items.AddRange(New Object() {"SpringCoat", "Hosted"})
        cbxSJMELaunchOption.Location = New Point(98, 26)
        cbxSJMELaunchOption.Name = "cbxSJMELaunchOption"
        cbxSJMELaunchOption.Size = New Size(156, 23)
        cbxSJMELaunchOption.TabIndex = 0
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
        ' cbxAudioType
        ' 
        cbxAudioType.DropDownStyle = ComboBoxStyle.DropDownList
        cbxAudioType.FlatStyle = FlatStyle.Flat
        cbxAudioType.Font = New Font("Segoe UI", 9F)
        cbxAudioType.FormattingEnabled = True
        cbxAudioType.Items.AddRange(New Object() {"Standard", "903i"})
        cbxAudioType.Location = New Point(95, 22)
        cbxAudioType.Margin = New Padding(2)
        cbxAudioType.Name = "cbxAudioType"
        cbxAudioType.Size = New Size(85, 23)
        cbxAudioType.TabIndex = 13
        ' 
        ' GroupBox4
        ' 
        GroupBox4.Controls.Add(cbxEZWebEZPlusSDK)
        GroupBox4.Controls.Add(Label15)
        GroupBox4.Controls.Add(cbxSoftbankSDK)
        GroupBox4.Controls.Add(Label12)
        GroupBox4.Controls.Add(cbxAirEdgeSDK)
        GroupBox4.Controls.Add(Label11)
        GroupBox4.Controls.Add(cbxVodafoneSDK)
        GroupBox4.Controls.Add(Label8)
        GroupBox4.Controls.Add(cbxFlashSDK)
        GroupBox4.Controls.Add(Label7)
        GroupBox4.Controls.Add(Label6)
        GroupBox4.Controls.Add(cbxJSKYSDK)
        GroupBox4.Controls.Add(Label5)
        GroupBox4.Controls.Add(Label3)
        GroupBox4.Controls.Add(cbxStarSDK)
        GroupBox4.Controls.Add(cbxDojaSDK)
        GroupBox4.Controls.Add(Label4)
        GroupBox4.Location = New Point(3, 131)
        GroupBox4.Name = "GroupBox4"
        GroupBox4.Size = New Size(236, 447)
        GroupBox4.TabIndex = 15
        GroupBox4.TabStop = False
        GroupBox4.Text = "SDK Configuration"
        ' 
        ' cbxEZWebEZPlusSDK
        ' 
        cbxEZWebEZPlusSDK.DropDownStyle = ComboBoxStyle.DropDownList
        cbxEZWebEZPlusSDK.FlatStyle = FlatStyle.Flat
        cbxEZWebEZPlusSDK.Font = New Font("Segoe UI", 9F)
        cbxEZWebEZPlusSDK.FormattingEnabled = True
        cbxEZWebEZPlusSDK.Location = New Point(117, 228)
        cbxEZWebEZPlusSDK.Margin = New Padding(2)
        cbxEZWebEZPlusSDK.Name = "cbxEZWebEZPlusSDK"
        cbxEZWebEZPlusSDK.Size = New Size(113, 23)
        cbxEZWebEZPlusSDK.TabIndex = 26
        ' 
        ' Label15
        ' 
        Label15.FlatStyle = FlatStyle.Flat
        Label15.Font = New Font("Segoe UI", 9F)
        Label15.Location = New Point(6, 228)
        Label15.Margin = New Padding(2, 0, 2, 0)
        Label15.Name = "Label15"
        Label15.Size = New Size(106, 23)
        Label15.TabIndex = 27
        Label15.Text = "EZWeb EZPlus"
        Label15.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' cbxSoftbankSDK
        ' 
        cbxSoftbankSDK.DropDownStyle = ComboBoxStyle.DropDownList
        cbxSoftbankSDK.FlatStyle = FlatStyle.Flat
        cbxSoftbankSDK.Font = New Font("Segoe UI", 9F)
        cbxSoftbankSDK.FormattingEnabled = True
        cbxSoftbankSDK.Location = New Point(117, 120)
        cbxSoftbankSDK.Margin = New Padding(2)
        cbxSoftbankSDK.Name = "cbxSoftbankSDK"
        cbxSoftbankSDK.Size = New Size(113, 23)
        cbxSoftbankSDK.TabIndex = 24
        ' 
        ' Label12
        ' 
        Label12.FlatStyle = FlatStyle.Flat
        Label12.Font = New Font("Segoe UI", 9F)
        Label12.Location = New Point(6, 120)
        Label12.Margin = New Padding(2, 0, 2, 0)
        Label12.Name = "Label12"
        Label12.Size = New Size(106, 23)
        Label12.TabIndex = 25
        Label12.Text = "Softbank SDK"
        Label12.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' cbxAirEdgeSDK
        ' 
        cbxAirEdgeSDK.DropDownStyle = ComboBoxStyle.DropDownList
        cbxAirEdgeSDK.FlatStyle = FlatStyle.Flat
        cbxAirEdgeSDK.Font = New Font("Segoe UI", 9F)
        cbxAirEdgeSDK.FormattingEnabled = True
        cbxAirEdgeSDK.Location = New Point(117, 201)
        cbxAirEdgeSDK.Margin = New Padding(2)
        cbxAirEdgeSDK.Name = "cbxAirEdgeSDK"
        cbxAirEdgeSDK.Size = New Size(113, 23)
        cbxAirEdgeSDK.TabIndex = 22
        ' 
        ' Label11
        ' 
        Label11.FlatStyle = FlatStyle.Flat
        Label11.Font = New Font("Segoe UI", 9F)
        Label11.Location = New Point(6, 201)
        Label11.Margin = New Padding(2, 0, 2, 0)
        Label11.Name = "Label11"
        Label11.Size = New Size(106, 23)
        Label11.TabIndex = 23
        Label11.Text = "AirEdge"
        Label11.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' cbxVodafoneSDK
        ' 
        cbxVodafoneSDK.DropDownStyle = ComboBoxStyle.DropDownList
        cbxVodafoneSDK.FlatStyle = FlatStyle.Flat
        cbxVodafoneSDK.Font = New Font("Segoe UI", 9F)
        cbxVodafoneSDK.FormattingEnabled = True
        cbxVodafoneSDK.Location = New Point(117, 174)
        cbxVodafoneSDK.Margin = New Padding(2)
        cbxVodafoneSDK.Name = "cbxVodafoneSDK"
        cbxVodafoneSDK.Size = New Size(113, 23)
        cbxVodafoneSDK.TabIndex = 20
        ' 
        ' Label8
        ' 
        Label8.FlatStyle = FlatStyle.Flat
        Label8.Font = New Font("Segoe UI", 9F)
        Label8.Location = New Point(6, 174)
        Label8.Margin = New Padding(2, 0, 2, 0)
        Label8.Name = "Label8"
        Label8.Size = New Size(106, 23)
        Label8.TabIndex = 21
        Label8.Text = "Vodafone"
        Label8.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' cbxFlashSDK
        ' 
        cbxFlashSDK.DropDownStyle = ComboBoxStyle.DropDownList
        cbxFlashSDK.FlatStyle = FlatStyle.Flat
        cbxFlashSDK.Font = New Font("Segoe UI", 9F)
        cbxFlashSDK.FormattingEnabled = True
        cbxFlashSDK.Location = New Point(117, 255)
        cbxFlashSDK.Margin = New Padding(2)
        cbxFlashSDK.Name = "cbxFlashSDK"
        cbxFlashSDK.Size = New Size(113, 23)
        cbxFlashSDK.TabIndex = 18
        ' 
        ' Label7
        ' 
        Label7.FlatStyle = FlatStyle.Flat
        Label7.Font = New Font("Segoe UI", 9F)
        Label7.Location = New Point(6, 255)
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
        cbxJSKYSDK.Location = New Point(117, 147)
        cbxJSKYSDK.Margin = New Padding(2)
        cbxJSKYSDK.Name = "cbxJSKYSDK"
        cbxJSKYSDK.Size = New Size(113, 23)
        cbxJSKYSDK.TabIndex = 15
        ' 
        ' Label5
        ' 
        Label5.FlatStyle = FlatStyle.Flat
        Label5.Font = New Font("Segoe UI", 9F)
        Label5.Location = New Point(6, 147)
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
        ' tpStats
        ' 
        tpStats.Controls.Add(GroupBox9)
        tpStats.Location = New Point(4, 24)
        tpStats.Name = "tpStats"
        tpStats.Size = New Size(1033, 581)
        tpStats.TabIndex = 4
        tpStats.Text = "Stats"
        tpStats.UseVisualStyleBackColor = True
        ' 
        ' GroupBox9
        ' 
        GroupBox9.Controls.Add(lvwPlaytimes)
        GroupBox9.Dock = DockStyle.Fill
        GroupBox9.Location = New Point(0, 0)
        GroupBox9.Name = "GroupBox9"
        GroupBox9.Size = New Size(1033, 581)
        GroupBox9.TabIndex = 0
        GroupBox9.TabStop = False
        GroupBox9.Text = "Playtime"
        ' 
        ' lvwPlaytimes
        ' 
        lvwPlaytimes.Font = New Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        lvwPlaytimes.FullRowSelect = True
        lvwPlaytimes.GridLines = True
        lvwPlaytimes.HideSelection = True
        lvwPlaytimes.Location = New Point(3, 19)
        lvwPlaytimes.Name = "lvwPlaytimes"
        lvwPlaytimes.Size = New Size(1027, 559)
        lvwPlaytimes.TabIndex = 0
        lvwPlaytimes.UseCompatibleStateImageBehavior = False
        lvwPlaytimes.View = View.Details
        ' 
        ' tpHelp
        ' 
        tpHelp.Controls.Add(btnVisitKeitaiArchive)
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
        ' btnVisitKeitaiArchive
        ' 
        btnVisitKeitaiArchive.Location = New Point(3, 131)
        btnVisitKeitaiArchive.Name = "btnVisitKeitaiArchive"
        btnVisitKeitaiArchive.Size = New Size(231, 37)
        btnVisitKeitaiArchive.TabIndex = 5
        btnVisitKeitaiArchive.Text = "Visit Keitai Archive"
        btnVisitKeitaiArchive.UseVisualStyleBackColor = True
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
        GroupBox7.Size = New Size(231, 122)
        GroupBox7.TabIndex = 3
        GroupBox7.TabStop = False
        GroupBox7.Text = "About App"
        ' 
        ' lblHelp_AppVer
        ' 
        lblHelp_AppVer.Dock = DockStyle.Fill
        lblHelp_AppVer.Location = New Point(3, 19)
        lblHelp_AppVer.Name = "lblHelp_AppVer"
        lblHelp_AppVer.Size = New Size(225, 100)
        lblHelp_AppVer.TabIndex = 0
        lblHelp_AppVer.Text = "App Version"
        lblHelp_AppVer.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' btnControls
        ' 
        btnControls.Location = New Point(3, 174)
        btnControls.Name = "btnControls"
        btnControls.Size = New Size(231, 72)
        btnControls.TabIndex = 2
        btnControls.Text = "Keybinds Controls"
        btnControls.UseVisualStyleBackColor = True
        ' 
        ' MaterialTabSelector1
        ' 
        MaterialTabSelector1.BaseTabControl = MaterialTabControl1
        MaterialTabSelector1.CausesValidation = False
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
        MaterialTabSelector1.TabIndex = 9
        MaterialTabSelector1.Text = "MaterialTabSelector1"
        ' 
        ' MainForm
        ' 
        AutoScaleDimensions = New SizeF(96F, 96F)
        AutoScaleMode = AutoScaleMode.Dpi
        ClientSize = New Size(1040, 710)
        Controls.Add(MaterialTabSelector1)
        Controls.Add(MaterialTabControl1)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Margin = New Padding(2)
        MaximizeBox = False
        Name = "MainForm"
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
        tpCharaDen.ResumeLayout(False)
        tpCharaDen.PerformLayout()
        GroupBox10.ResumeLayout(False)
        cmsCharadenLV.ResumeLayout(False)
        tpConfig.ResumeLayout(False)
        GroupBox11.ResumeLayout(False)
        GroupBox11.PerformLayout()
        GroupBox6.ResumeLayout(False)
        GroupBox5.ResumeLayout(False)
        GroupBox5.PerformLayout()
        gbxSJMELaunchOptions.ResumeLayout(False)
        GroupBox4.ResumeLayout(False)
        tpStats.ResumeLayout(False)
        GroupBox9.ResumeLayout(False)
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
    Friend WithEvents cbxAudioType As ComboBox
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
    Friend WithEvents tpStats As TabPage
    Friend WithEvents GroupBox9 As GroupBox
    Friend WithEvents lvwPlaytimes As ListView
    Friend WithEvents cmsBombermanPuzzle As ToolStripMenuItem
    Friend WithEvents ImportStageToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ExportStageToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents tsmBPSImportStage1 As ToolStripMenuItem
    Friend WithEvents tsmBPSImportStage2 As ToolStripMenuItem
    Friend WithEvents tsmBPSImportStage3 As ToolStripMenuItem
    Friend WithEvents tsmBPSImportStage4 As ToolStripMenuItem
    Friend WithEvents tsmBPSImportStage5 As ToolStripMenuItem
    Friend WithEvents tsmBPSImportStage6 As ToolStripMenuItem
    Friend WithEvents tsmBPSImportStage7 As ToolStripMenuItem
    Friend WithEvents tsmBPSImportStage8 As ToolStripMenuItem
    Friend WithEvents tsmBPSImportStage9 As ToolStripMenuItem
    Friend WithEvents tsmBPSImportStage10 As ToolStripMenuItem
    Friend WithEvents tsmBPSExportStage1 As ToolStripMenuItem
    Friend WithEvents tsmBPSExportStage2 As ToolStripMenuItem
    Friend WithEvents tsmBPSExportStage3 As ToolStripMenuItem
    Friend WithEvents tsmBPSExportStage4 As ToolStripMenuItem
    Friend WithEvents tsmBPSExportStage5 As ToolStripMenuItem
    Friend WithEvents tsmBPSExportStage6 As ToolStripMenuItem
    Friend WithEvents tsmBPSExportStage7 As ToolStripMenuItem
    Friend WithEvents tsmBPSExportStage8 As ToolStripMenuItem
    Friend WithEvents tsmBPSExportStage9 As ToolStripMenuItem
    Friend WithEvents tsmBPSExportStage10 As ToolStripMenuItem
    Friend WithEvents tpCharaDen As TabPage
    Friend WithEvents chkboxCharadenLocalEmulator As CheckBox
    Friend WithEvents btnCharaDenLaunch As Button
    Friend WithEvents GroupBox10 As GroupBox
    Friend WithEvents ListViewCharaDen As ListView
    Friend WithEvents lblCharadenTotalCount As Label
    Friend WithEvents cmsCharadenLV As ContextMenuStrip
    Friend WithEvents DownloadCMS_CharaDen As ToolStripMenuItem
    Friend WithEvents DeleteCMS_CharaDen As ToolStripMenuItem
    Friend WithEvents cbxVodafoneSDK As ComboBox
    Friend WithEvents Label8 As Label
    Friend WithEvents gbxSJMELaunchOptions As GroupBox
    Friend WithEvents Label9 As Label
    Friend WithEvents cbxSJMELaunchOption As ComboBox
    Friend WithEvents lblSJMELaunchOptionsText As Label
    Friend WithEvents btnSJMEUpdate As Button
    Friend WithEvents Label10 As Label
    Friend WithEvents cbxSJMEScaling As ComboBox
    Friend WithEvents cbxAirEdgeSDK As ComboBox
    Friend WithEvents Label11 As Label
    Friend WithEvents btnVisitKeitaiArchive As Button
    Friend WithEvents cbxSoftbankSDK As ComboBox
    Friend WithEvents Label12 As Label
    Friend WithEvents GroupBox11 As GroupBox
    Friend WithEvents Label13 As Label
    Friend WithEvents lblInvalidUID As Label
    Private WithEvents Label14 As Label
    Friend WithEvents txtCurrentUID As TextBox
    Friend WithEvents chkboxNetworkModifyURLS As CheckBox
    Friend WithEvents lblInvalidTID As Label
    Private WithEvents Label16 As Label
    Friend WithEvents txtCurrentTID As TextBox
    Friend WithEvents cbxEZWebEZPlusSDK As ComboBox
    Friend WithEvents Label15 As Label
End Class
