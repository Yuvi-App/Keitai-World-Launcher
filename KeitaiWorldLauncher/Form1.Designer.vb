﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
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
        ImageListGames = New ImageList(components)
        cmsGameLV = New ContextMenuStrip(components)
        cmsGameLV_Launch = New ToolStripMenuItem()
        cmsGameLV_Download = New ToolStripMenuItem()
        cmsGameLV_Delete = New ToolStripMenuItem()
        FavoriteToolStripMenuItem = New ToolStripMenuItem()
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
        tpMachiChara = New TabPage()
        btnMachiCharaLaunch = New Button()
        GroupBox2 = New GroupBox()
        lbxMachiCharaList = New ListBox()
        tpGames = New TabPage()
        lblAudioWarning = New Label()
        Label4 = New Label()
        Label3 = New Label()
        cbxStarSDK = New ComboBox()
        cbxDojaSDK = New ComboBox()
        chkbxShaderGlass = New CheckBox()
        Label1 = New Label()
        cobxAudioType = New ComboBox()
        GroupBox1 = New GroupBox()
        ListViewGamesVariants = New ListView()
        cbxFilterType = New ComboBox()
        Label2 = New Label()
        txtLVSearch = New TextBox()
        ListViewGames = New ListView()
        lblTotalGameCount = New Label()
        chkbxHidePhoneUI = New CheckBox()
        gbxGameInfo = New GroupBox()
        pbGameDL = New ProgressBar()
        btnLaunchGame = New Button()
        TabControl1 = New TabControl()
        cmsGameLV.SuspendLayout()
        MenuStrip1.SuspendLayout()
        tpMachiChara.SuspendLayout()
        GroupBox2.SuspendLayout()
        tpGames.SuspendLayout()
        GroupBox1.SuspendLayout()
        gbxGameInfo.SuspendLayout()
        TabControl1.SuspendLayout()
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
        cmsGameLV.Items.AddRange(New ToolStripItem() {cmsGameLV_Launch, cmsGameLV_Download, cmsGameLV_Delete, FavoriteToolStripMenuItem})
        cmsGameLV.Name = "cmsGameLV"
        cmsGameLV.Size = New Size(129, 92)
        ' 
        ' cmsGameLV_Launch
        ' 
        cmsGameLV_Launch.Name = "cmsGameLV_Launch"
        cmsGameLV_Launch.Size = New Size(128, 22)
        cmsGameLV_Launch.Text = "Launch"
        ' 
        ' cmsGameLV_Download
        ' 
        cmsGameLV_Download.Name = "cmsGameLV_Download"
        cmsGameLV_Download.Size = New Size(128, 22)
        cmsGameLV_Download.Text = "Download"
        ' 
        ' cmsGameLV_Delete
        ' 
        cmsGameLV_Delete.Name = "cmsGameLV_Delete"
        cmsGameLV_Delete.Size = New Size(128, 22)
        cmsGameLV_Delete.Text = "Delete"
        ' 
        ' FavoriteToolStripMenuItem
        ' 
        FavoriteToolStripMenuItem.Name = "FavoriteToolStripMenuItem"
        FavoriteToolStripMenuItem.Size = New Size(128, 22)
        FavoriteToolStripMenuItem.Text = "Favorite"
        ' 
        ' MenuStrip1
        ' 
        MenuStrip1.BackColor = Color.Gainsboro
        MenuStrip1.Font = New Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        MenuStrip1.ImageScalingSize = New Size(20, 20)
        MenuStrip1.Items.AddRange(New ToolStripItem() {FileToolStripMenuItem, ConfigToolStripMenuItem1, ToolsToolStripMenuItem, HelpToolStripMenuItem})
        MenuStrip1.Location = New Point(0, 51)
        MenuStrip1.Name = "MenuStrip1"
        MenuStrip1.Padding = New Padding(5, 2, 0, 2)
        MenuStrip1.Size = New Size(890, 27)
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
        ' tpMachiChara
        ' 
        tpMachiChara.BackColor = Color.WhiteSmoke
        tpMachiChara.Controls.Add(btnMachiCharaLaunch)
        tpMachiChara.Controls.Add(GroupBox2)
        tpMachiChara.Location = New Point(4, 28)
        tpMachiChara.Margin = New Padding(2)
        tpMachiChara.Name = "tpMachiChara"
        tpMachiChara.Padding = New Padding(2)
        tpMachiChara.Size = New Size(882, 575)
        tpMachiChara.TabIndex = 1
        tpMachiChara.Text = "Machi Chara"
        ' 
        ' btnMachiCharaLaunch
        ' 
        btnMachiCharaLaunch.Enabled = False
        btnMachiCharaLaunch.Location = New Point(254, 30)
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
        GroupBox2.Location = New Point(9, 10)
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
        lbxMachiCharaList.ItemHeight = 19
        lbxMachiCharaList.Location = New Point(5, 22)
        lbxMachiCharaList.Margin = New Padding(2)
        lbxMachiCharaList.Name = "lbxMachiCharaList"
        lbxMachiCharaList.Size = New Size(230, 422)
        lbxMachiCharaList.TabIndex = 0
        ' 
        ' tpGames
        ' 
        tpGames.BackColor = Color.WhiteSmoke
        tpGames.Controls.Add(lblAudioWarning)
        tpGames.Controls.Add(Label4)
        tpGames.Controls.Add(Label3)
        tpGames.Controls.Add(cbxStarSDK)
        tpGames.Controls.Add(cbxDojaSDK)
        tpGames.Controls.Add(chkbxShaderGlass)
        tpGames.Controls.Add(Label1)
        tpGames.Controls.Add(cobxAudioType)
        tpGames.Controls.Add(GroupBox1)
        tpGames.Controls.Add(chkbxHidePhoneUI)
        tpGames.Controls.Add(gbxGameInfo)
        tpGames.Controls.Add(btnLaunchGame)
        tpGames.Location = New Point(4, 28)
        tpGames.Margin = New Padding(2)
        tpGames.Name = "tpGames"
        tpGames.Padding = New Padding(2)
        tpGames.Size = New Size(882, 575)
        tpGames.TabIndex = 0
        tpGames.Text = "Games"
        ' 
        ' lblAudioWarning
        ' 
        lblAudioWarning.FlatStyle = FlatStyle.Flat
        lblAudioWarning.Font = New Font("Segoe UI", 9F)
        lblAudioWarning.ForeColor = Color.Firebrick
        lblAudioWarning.Location = New Point(573, 517)
        lblAudioWarning.Margin = New Padding(2, 0, 2, 0)
        lblAudioWarning.Name = "lblAudioWarning"
        lblAudioWarning.Size = New Size(145, 23)
        lblAudioWarning.TabIndex = 11
        lblAudioWarning.Text = "Can Cause Performance Issues"
        lblAudioWarning.TextAlign = ContentAlignment.TopCenter
        lblAudioWarning.Visible = False
        ' 
        ' Label4
        ' 
        Label4.FlatStyle = FlatStyle.Flat
        Label4.Font = New Font("Segoe UI", 9F)
        Label4.Location = New Point(742, 522)
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
        Label3.Location = New Point(738, 494)
        Label3.Margin = New Padding(2, 0, 2, 0)
        Label3.Name = "Label3"
        Label3.Size = New Size(55, 23)
        Label3.TabIndex = 9
        Label3.Text = "Doja SDK"
        Label3.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' cbxStarSDK
        ' 
        cbxStarSDK.FlatStyle = FlatStyle.Flat
        cbxStarSDK.Font = New Font("Segoe UI", 9F)
        cbxStarSDK.FormattingEnabled = True
        cbxStarSDK.Location = New Point(797, 523)
        cbxStarSDK.Margin = New Padding(2)
        cbxStarSDK.Name = "cbxStarSDK"
        cbxStarSDK.Size = New Size(85, 23)
        cbxStarSDK.TabIndex = 8
        ' 
        ' cbxDojaSDK
        ' 
        cbxDojaSDK.FlatStyle = FlatStyle.Flat
        cbxDojaSDK.Font = New Font("Segoe UI", 9F)
        cbxDojaSDK.FormattingEnabled = True
        cbxDojaSDK.Location = New Point(797, 494)
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
        chkbxShaderGlass.Location = New Point(443, 509)
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
        Label1.Location = New Point(573, 494)
        Label1.Margin = New Padding(2, 0, 2, 0)
        Label1.Name = "Label1"
        Label1.Size = New Size(67, 23)
        Label1.TabIndex = 5
        Label1.Text = "Audio Type"
        Label1.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' cobxAudioType
        ' 
        cobxAudioType.Enabled = False
        cobxAudioType.FlatStyle = FlatStyle.Flat
        cobxAudioType.Font = New Font("Segoe UI", 9F)
        cobxAudioType.FormattingEnabled = True
        cobxAudioType.Items.AddRange(New Object() {"Standard", "903i"})
        cobxAudioType.Location = New Point(644, 494)
        cobxAudioType.Margin = New Padding(2)
        cobxAudioType.Name = "cobxAudioType"
        cobxAudioType.Size = New Size(74, 23)
        cobxAudioType.TabIndex = 4
        ' 
        ' GroupBox1
        ' 
        GroupBox1.Controls.Add(ListViewGamesVariants)
        GroupBox1.Controls.Add(cbxFilterType)
        GroupBox1.Controls.Add(Label2)
        GroupBox1.Controls.Add(txtLVSearch)
        GroupBox1.Controls.Add(ListViewGames)
        GroupBox1.Controls.Add(lblTotalGameCount)
        GroupBox1.FlatStyle = FlatStyle.Flat
        GroupBox1.Location = New Point(5, 4)
        GroupBox1.Margin = New Padding(2)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Padding = New Padding(2)
        GroupBox1.Size = New Size(319, 567)
        GroupBox1.TabIndex = 0
        GroupBox1.TabStop = False
        GroupBox1.Text = "Available Games"
        ' 
        ' ListViewGamesVariants
        ' 
        ListViewGamesVariants.LargeImageList = ImageListGames
        ListViewGamesVariants.Location = New Point(5, 424)
        ListViewGamesVariants.Margin = New Padding(2)
        ListViewGamesVariants.Name = "ListViewGamesVariants"
        ListViewGamesVariants.Size = New Size(310, 117)
        ListViewGamesVariants.TabIndex = 10
        ListViewGamesVariants.UseCompatibleStateImageBehavior = False
        ListViewGamesVariants.View = View.SmallIcon
        ' 
        ' cbxFilterType
        ' 
        cbxFilterType.FlatStyle = FlatStyle.Flat
        cbxFilterType.FormattingEnabled = True
        cbxFilterType.Items.AddRange(New Object() {"All", "Favorites", "Installed", "Custom", "Doja", "Star"})
        cbxFilterType.Location = New Point(236, 19)
        cbxFilterType.Margin = New Padding(2)
        cbxFilterType.Name = "cbxFilterType"
        cbxFilterType.Size = New Size(79, 27)
        cbxFilterType.TabIndex = 9
        ' 
        ' Label2
        ' 
        Label2.FlatStyle = FlatStyle.Flat
        Label2.Location = New Point(9, 20)
        Label2.Margin = New Padding(2, 0, 2, 0)
        Label2.Name = "Label2"
        Label2.Size = New Size(53, 26)
        Label2.TabIndex = 8
        Label2.Text = "Search:"
        Label2.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' txtLVSearch
        ' 
        txtLVSearch.Location = New Point(58, 19)
        txtLVSearch.Margin = New Padding(2)
        txtLVSearch.Name = "txtLVSearch"
        txtLVSearch.Size = New Size(174, 26)
        txtLVSearch.TabIndex = 7
        ' 
        ' ListViewGames
        ' 
        ListViewGames.ContextMenuStrip = cmsGameLV
        ListViewGames.LargeImageList = ImageListGames
        ListViewGames.Location = New Point(5, 50)
        ListViewGames.Margin = New Padding(2)
        ListViewGames.Name = "ListViewGames"
        ListViewGames.Size = New Size(310, 370)
        ListViewGames.TabIndex = 6
        ListViewGames.UseCompatibleStateImageBehavior = False
        ListViewGames.View = View.Details
        ' 
        ' lblTotalGameCount
        ' 
        lblTotalGameCount.AutoSize = True
        lblTotalGameCount.FlatStyle = FlatStyle.Flat
        lblTotalGameCount.Location = New Point(9, 546)
        lblTotalGameCount.Margin = New Padding(2, 0, 2, 0)
        lblTotalGameCount.Name = "lblTotalGameCount"
        lblTotalGameCount.Size = New Size(53, 19)
        lblTotalGameCount.TabIndex = 1
        lblTotalGameCount.Text = "Total: 0"
        ' 
        ' chkbxHidePhoneUI
        ' 
        chkbxHidePhoneUI.AutoSize = True
        chkbxHidePhoneUI.Enabled = False
        chkbxHidePhoneUI.FlatStyle = FlatStyle.Flat
        chkbxHidePhoneUI.Font = New Font("Segoe UI", 9F)
        chkbxHidePhoneUI.Location = New Point(443, 490)
        chkbxHidePhoneUI.Margin = New Padding(2)
        chkbxHidePhoneUI.Name = "chkbxHidePhoneUI"
        chkbxHidePhoneUI.Size = New Size(99, 19)
        chkbxHidePhoneUI.TabIndex = 3
        chkbxHidePhoneUI.Text = "Hide Phone UI"
        chkbxHidePhoneUI.UseVisualStyleBackColor = True
        ' 
        ' gbxGameInfo
        ' 
        gbxGameInfo.Controls.Add(pbGameDL)
        gbxGameInfo.FlatStyle = FlatStyle.Flat
        gbxGameInfo.Location = New Point(329, 4)
        gbxGameInfo.Margin = New Padding(2)
        gbxGameInfo.Name = "gbxGameInfo"
        gbxGameInfo.Padding = New Padding(2)
        gbxGameInfo.Size = New Size(553, 481)
        gbxGameInfo.TabIndex = 1
        gbxGameInfo.TabStop = False
        gbxGameInfo.Text = "Game Info"
        ' 
        ' pbGameDL
        ' 
        pbGameDL.Location = New Point(4, 215)
        pbGameDL.Margin = New Padding(2)
        pbGameDL.Name = "pbGameDL"
        pbGameDL.Size = New Size(534, 46)
        pbGameDL.TabIndex = 1
        pbGameDL.Visible = False
        ' 
        ' btnLaunchGame
        ' 
        btnLaunchGame.Enabled = False
        btnLaunchGame.FlatStyle = FlatStyle.Flat
        btnLaunchGame.Font = New Font("Segoe UI", 9F)
        btnLaunchGame.Location = New Point(329, 494)
        btnLaunchGame.Margin = New Padding(2)
        btnLaunchGame.Name = "btnLaunchGame"
        btnLaunchGame.Size = New Size(110, 77)
        btnLaunchGame.TabIndex = 2
        btnLaunchGame.Text = "Launch"
        btnLaunchGame.UseVisualStyleBackColor = True
        ' 
        ' TabControl1
        ' 
        TabControl1.Controls.Add(tpGames)
        TabControl1.Controls.Add(tpMachiChara)
        TabControl1.Font = New Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        TabControl1.Location = New Point(0, 80)
        TabControl1.Margin = New Padding(2)
        TabControl1.Name = "TabControl1"
        TabControl1.SelectedIndex = 0
        TabControl1.Size = New Size(890, 607)
        TabControl1.TabIndex = 4
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(96F, 96F)
        AutoScaleMode = AutoScaleMode.Dpi
        ClientSize = New Size(890, 692)
        Controls.Add(TabControl1)
        Controls.Add(MenuStrip1)
        MainMenuStrip = MenuStrip1
        Margin = New Padding(2)
        Name = "Form1"
        Padding = New Padding(0, 51, 0, 3)
        Text = "Keitai World Launcher"
        cmsGameLV.ResumeLayout(False)
        MenuStrip1.ResumeLayout(False)
        MenuStrip1.PerformLayout()
        tpMachiChara.ResumeLayout(False)
        GroupBox2.ResumeLayout(False)
        tpGames.ResumeLayout(False)
        tpGames.PerformLayout()
        GroupBox1.ResumeLayout(False)
        GroupBox1.PerformLayout()
        gbxGameInfo.ResumeLayout(False)
        TabControl1.ResumeLayout(False)
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
    Friend WithEvents FavoriteToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents selectionTimer As Timer
    Friend WithEvents NetworkUIDConfigToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AddGamesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents FolderBrowserDialog1 As FolderBrowserDialog
    Friend WithEvents tpMachiChara As TabPage
    Friend WithEvents btnMachiCharaLaunch As Button
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents lbxMachiCharaList As ListBox
    Friend WithEvents tpGames As TabPage
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
    Friend WithEvents Label2 As Label
    Friend WithEvents txtLVSearch As TextBox
    Friend WithEvents ListViewGames As ListView
    Friend WithEvents lblTotalGameCount As Label
    Friend WithEvents chkbxHidePhoneUI As CheckBox
    Friend WithEvents gbxGameInfo As GroupBox
    Friend WithEvents pbGameDL As ProgressBar
    Friend WithEvents btnLaunchGame As Button
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents ControlsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents lblAudioWarning As Label
End Class
