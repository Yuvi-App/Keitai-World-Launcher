﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        GroupBox1 = New GroupBox()
        ListViewGamesVariants = New ListView()
        ImageListGames = New ImageList(components)
        cbxFilterType = New ComboBox()
        Label2 = New Label()
        txtLVSearch = New TextBox()
        ListViewGames = New ListView()
        cmsGameLV = New ContextMenuStrip(components)
        cmsGameLV_Launch = New ToolStripMenuItem()
        cmsGameLV_Download = New ToolStripMenuItem()
        cmsGameLV_Delete = New ToolStripMenuItem()
        FavoriteToolStripMenuItem = New ToolStripMenuItem()
        lblTotalGameCount = New Label()
        pbGameDL = New ProgressBar()
        gbxGameInfo = New GroupBox()
        btnLaunchGame = New Button()
        chkbxHidePhoneUI = New CheckBox()
        TabControl1 = New TabControl()
        tpGames = New TabPage()
        Label4 = New Label()
        Label3 = New Label()
        cbxStarSDK = New ComboBox()
        cbxDojaSDK = New ComboBox()
        chkbxShaderGlass = New CheckBox()
        Label1 = New Label()
        cobxAudioType = New ComboBox()
        tpMachiChara = New TabPage()
        btnMachiCharaLaunch = New Button()
        GroupBox2 = New GroupBox()
        lbxMachiCharaList = New ListBox()
        MenuStrip1 = New MenuStrip()
        FileToolStripMenuItem = New ToolStripMenuItem()
        RefreshToolStripMenuItem = New ToolStripMenuItem()
        ExitToolStripMenuItem = New ToolStripMenuItem()
        ConfigToolStripMenuItem1 = New ToolStripMenuItem()
        KeyConfiguratorToolStripMenuItem = New ToolStripMenuItem()
        AppConfigToolStripMenuItem = New ToolStripMenuItem()
        ShaderGlassConfigToolStripMenuItem = New ToolStripMenuItem()
        ToolsToolStripMenuItem = New ToolStripMenuItem()
        BatchDownloadToolStripMenuItem = New ToolStripMenuItem()
        GamesToolStripMenuItem = New ToolStripMenuItem()
        MachiCharaToolStripMenuItem = New ToolStripMenuItem()
        XMLCreationToolStripMenuItem = New ToolStripMenuItem()
        GamelistToolStripMenuItem = New ToolStripMenuItem()
        XMLUpdateToolStripMenuItem = New ToolStripMenuItem()
        AddGameToolStripMenuItem = New ToolStripMenuItem()
        HelpToolStripMenuItem = New ToolStripMenuItem()
        AboutToolStripMenuItem = New ToolStripMenuItem()
        OpenFileDialog1 = New OpenFileDialog()
        selectionTimer = New Timer(components)
        GroupBox1.SuspendLayout()
        cmsGameLV.SuspendLayout()
        TabControl1.SuspendLayout()
        tpGames.SuspendLayout()
        tpMachiChara.SuspendLayout()
        GroupBox2.SuspendLayout()
        MenuStrip1.SuspendLayout()
        SuspendLayout()
        ' 
        ' GroupBox1
        ' 
        GroupBox1.Controls.Add(ListViewGamesVariants)
        GroupBox1.Controls.Add(cbxFilterType)
        GroupBox1.Controls.Add(Label2)
        GroupBox1.Controls.Add(txtLVSearch)
        GroupBox1.Controls.Add(ListViewGames)
        GroupBox1.Controls.Add(lblTotalGameCount)
        GroupBox1.Location = New Point(6, 5)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Size = New Size(399, 667)
        GroupBox1.TabIndex = 0
        GroupBox1.TabStop = False
        GroupBox1.Text = "Available Games"
        ' 
        ' ListViewGamesVariants
        ' 
        ListViewGamesVariants.LargeImageList = ImageListGames
        ListViewGamesVariants.Location = New Point(6, 491)
        ListViewGamesVariants.Name = "ListViewGamesVariants"
        ListViewGamesVariants.Size = New Size(387, 149)
        ListViewGamesVariants.TabIndex = 10
        ListViewGamesVariants.UseCompatibleStateImageBehavior = False
        ListViewGamesVariants.View = View.SmallIcon
        ' 
        ' ImageListGames
        ' 
        ImageListGames.ColorDepth = ColorDepth.Depth32Bit
        ImageListGames.ImageSize = New Size(36, 36)
        ImageListGames.TransparentColor = Color.Transparent
        ' 
        ' cbxFilterType
        ' 
        cbxFilterType.FormattingEnabled = True
        cbxFilterType.Items.AddRange(New Object() {"All", "Favorites", "Installed", "Doja", "Star"})
        cbxFilterType.Location = New Point(295, 25)
        cbxFilterType.Name = "cbxFilterType"
        cbxFilterType.Size = New Size(98, 28)
        cbxFilterType.TabIndex = 9
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(10, 29)
        Label2.Name = "Label2"
        Label2.Size = New Size(56, 20)
        Label2.TabIndex = 8
        Label2.Text = "Search:"
        ' 
        ' txtLVSearch
        ' 
        txtLVSearch.Location = New Point(72, 27)
        txtLVSearch.Name = "txtLVSearch"
        txtLVSearch.Size = New Size(217, 27)
        txtLVSearch.TabIndex = 7
        ' 
        ' ListViewGames
        ' 
        ListViewGames.ContextMenuStrip = cmsGameLV
        ListViewGames.LargeImageList = ImageListGames
        ListViewGames.Location = New Point(6, 59)
        ListViewGames.Name = "ListViewGames"
        ListViewGames.Size = New Size(387, 425)
        ListViewGames.TabIndex = 6
        ListViewGames.UseCompatibleStateImageBehavior = False
        ListViewGames.View = View.Details
        ' 
        ' cmsGameLV
        ' 
        cmsGameLV.ImageScalingSize = New Size(20, 20)
        cmsGameLV.Items.AddRange(New ToolStripItem() {cmsGameLV_Launch, cmsGameLV_Download, cmsGameLV_Delete, FavoriteToolStripMenuItem})
        cmsGameLV.Name = "cmsGameLV"
        cmsGameLV.Size = New Size(148, 100)
        ' 
        ' cmsGameLV_Launch
        ' 
        cmsGameLV_Launch.Name = "cmsGameLV_Launch"
        cmsGameLV_Launch.Size = New Size(147, 24)
        cmsGameLV_Launch.Text = "Launch"
        ' 
        ' cmsGameLV_Download
        ' 
        cmsGameLV_Download.Name = "cmsGameLV_Download"
        cmsGameLV_Download.Size = New Size(147, 24)
        cmsGameLV_Download.Text = "Download"
        ' 
        ' cmsGameLV_Delete
        ' 
        cmsGameLV_Delete.Name = "cmsGameLV_Delete"
        cmsGameLV_Delete.Size = New Size(147, 24)
        cmsGameLV_Delete.Text = "Delete"
        ' 
        ' FavoriteToolStripMenuItem
        ' 
        FavoriteToolStripMenuItem.Name = "FavoriteToolStripMenuItem"
        FavoriteToolStripMenuItem.Size = New Size(147, 24)
        FavoriteToolStripMenuItem.Text = "Favorite"
        ' 
        ' lblTotalGameCount
        ' 
        lblTotalGameCount.AutoSize = True
        lblTotalGameCount.Location = New Point(6, 644)
        lblTotalGameCount.Name = "lblTotalGameCount"
        lblTotalGameCount.Size = New Size(57, 20)
        lblTotalGameCount.TabIndex = 1
        lblTotalGameCount.Text = "Total: 0"
        ' 
        ' pbGameDL
        ' 
        pbGameDL.Location = New Point(16, 757)
        pbGameDL.Name = "pbGameDL"
        pbGameDL.Size = New Size(1095, 29)
        pbGameDL.TabIndex = 1
        pbGameDL.Visible = False
        ' 
        ' gbxGameInfo
        ' 
        gbxGameInfo.Location = New Point(411, 5)
        gbxGameInfo.Name = "gbxGameInfo"
        gbxGameInfo.Size = New Size(677, 601)
        gbxGameInfo.TabIndex = 1
        gbxGameInfo.TabStop = False
        gbxGameInfo.Text = "Game Info"
        ' 
        ' btnLaunchGame
        ' 
        btnLaunchGame.Enabled = False
        btnLaunchGame.Location = New Point(411, 612)
        btnLaunchGame.Name = "btnLaunchGame"
        btnLaunchGame.Size = New Size(137, 67)
        btnLaunchGame.TabIndex = 2
        btnLaunchGame.Text = "Launch"
        btnLaunchGame.UseVisualStyleBackColor = True
        ' 
        ' chkbxHidePhoneUI
        ' 
        chkbxHidePhoneUI.AutoSize = True
        chkbxHidePhoneUI.Enabled = False
        chkbxHidePhoneUI.Location = New Point(554, 612)
        chkbxHidePhoneUI.Name = "chkbxHidePhoneUI"
        chkbxHidePhoneUI.Size = New Size(127, 24)
        chkbxHidePhoneUI.TabIndex = 3
        chkbxHidePhoneUI.Text = "Hide phone UI"
        chkbxHidePhoneUI.UseVisualStyleBackColor = True
        ' 
        ' TabControl1
        ' 
        TabControl1.Controls.Add(tpGames)
        TabControl1.Controls.Add(tpMachiChara)
        TabControl1.Location = New Point(11, 31)
        TabControl1.Name = "TabControl1"
        TabControl1.SelectedIndex = 0
        TabControl1.Size = New Size(1103, 721)
        TabControl1.TabIndex = 4
        ' 
        ' tpGames
        ' 
        tpGames.BackColor = Color.WhiteSmoke
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
        tpGames.Location = New Point(4, 29)
        tpGames.Name = "tpGames"
        tpGames.Padding = New Padding(3)
        tpGames.Size = New Size(1095, 688)
        tpGames.TabIndex = 0
        tpGames.Text = "Games"
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Location = New Point(910, 657)
        Label4.Name = "Label4"
        Label4.Size = New Size(67, 20)
        Label4.TabIndex = 10
        Label4.Text = "Star SDK"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Location = New Point(904, 620)
        Label3.Name = "Label3"
        Label3.Size = New Size(73, 20)
        Label3.TabIndex = 9
        Label3.Text = "Doja SDK"
        ' 
        ' cbxStarSDK
        ' 
        cbxStarSDK.FormattingEnabled = True
        cbxStarSDK.Location = New Point(983, 654)
        cbxStarSDK.Name = "cbxStarSDK"
        cbxStarSDK.Size = New Size(105, 28)
        cbxStarSDK.TabIndex = 8
        ' 
        ' cbxDojaSDK
        ' 
        cbxDojaSDK.FormattingEnabled = True
        cbxDojaSDK.Location = New Point(983, 617)
        cbxDojaSDK.Name = "cbxDojaSDK"
        cbxDojaSDK.Size = New Size(105, 28)
        cbxDojaSDK.TabIndex = 7
        ' 
        ' chkbxShaderGlass
        ' 
        chkbxShaderGlass.AutoSize = True
        chkbxShaderGlass.Enabled = False
        chkbxShaderGlass.Location = New Point(687, 612)
        chkbxShaderGlass.Name = "chkbxShaderGlass"
        chkbxShaderGlass.Size = New Size(111, 24)
        chkbxShaderGlass.TabIndex = 6
        chkbxShaderGlass.Text = "ShaderGlass"
        chkbxShaderGlass.UseVisualStyleBackColor = True
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(554, 659)
        Label1.Name = "Label1"
        Label1.Size = New Size(84, 20)
        Label1.TabIndex = 5
        Label1.Text = "Audio Type"
        ' 
        ' cobxAudioType
        ' 
        cobxAudioType.Enabled = False
        cobxAudioType.FormattingEnabled = True
        cobxAudioType.Items.AddRange(New Object() {"Standard", "903i"})
        cobxAudioType.Location = New Point(644, 654)
        cobxAudioType.Name = "cobxAudioType"
        cobxAudioType.Size = New Size(92, 28)
        cobxAudioType.TabIndex = 4
        ' 
        ' tpMachiChara
        ' 
        tpMachiChara.BackColor = Color.WhiteSmoke
        tpMachiChara.Controls.Add(btnMachiCharaLaunch)
        tpMachiChara.Controls.Add(GroupBox2)
        tpMachiChara.Location = New Point(4, 29)
        tpMachiChara.Name = "tpMachiChara"
        tpMachiChara.Padding = New Padding(3)
        tpMachiChara.Size = New Size(1095, 688)
        tpMachiChara.TabIndex = 1
        tpMachiChara.Text = "Machi Chara"
        ' 
        ' btnMachiCharaLaunch
        ' 
        btnMachiCharaLaunch.Enabled = False
        btnMachiCharaLaunch.Location = New Point(317, 37)
        btnMachiCharaLaunch.Name = "btnMachiCharaLaunch"
        btnMachiCharaLaunch.Size = New Size(137, 67)
        btnMachiCharaLaunch.TabIndex = 3
        btnMachiCharaLaunch.Text = "Launch"
        btnMachiCharaLaunch.UseVisualStyleBackColor = True
        ' 
        ' GroupBox2
        ' 
        GroupBox2.Controls.Add(lbxMachiCharaList)
        GroupBox2.Location = New Point(11, 12)
        GroupBox2.Name = "GroupBox2"
        GroupBox2.Size = New Size(299, 607)
        GroupBox2.TabIndex = 1
        GroupBox2.TabStop = False
        GroupBox2.Text = "Available Machi Chara"
        ' 
        ' lbxMachiCharaList
        ' 
        lbxMachiCharaList.FormattingEnabled = True
        lbxMachiCharaList.Location = New Point(6, 27)
        lbxMachiCharaList.Name = "lbxMachiCharaList"
        lbxMachiCharaList.Size = New Size(287, 564)
        lbxMachiCharaList.TabIndex = 0
        ' 
        ' MenuStrip1
        ' 
        MenuStrip1.ImageScalingSize = New Size(20, 20)
        MenuStrip1.Items.AddRange(New ToolStripItem() {FileToolStripMenuItem, ConfigToolStripMenuItem1, ToolsToolStripMenuItem, HelpToolStripMenuItem})
        MenuStrip1.Location = New Point(0, 0)
        MenuStrip1.Name = "MenuStrip1"
        MenuStrip1.Padding = New Padding(6, 3, 0, 3)
        MenuStrip1.Size = New Size(1125, 30)
        MenuStrip1.TabIndex = 5
        MenuStrip1.Text = "MenuStrip1"
        ' 
        ' FileToolStripMenuItem
        ' 
        FileToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {RefreshToolStripMenuItem, ExitToolStripMenuItem})
        FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        FileToolStripMenuItem.Size = New Size(46, 24)
        FileToolStripMenuItem.Text = "File"
        ' 
        ' RefreshToolStripMenuItem
        ' 
        RefreshToolStripMenuItem.Name = "RefreshToolStripMenuItem"
        RefreshToolStripMenuItem.Size = New Size(141, 26)
        RefreshToolStripMenuItem.Text = "Refresh"
        ' 
        ' ExitToolStripMenuItem
        ' 
        ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        ExitToolStripMenuItem.Size = New Size(141, 26)
        ExitToolStripMenuItem.Text = "Exit"
        ' 
        ' ConfigToolStripMenuItem1
        ' 
        ConfigToolStripMenuItem1.DropDownItems.AddRange(New ToolStripItem() {KeyConfiguratorToolStripMenuItem, AppConfigToolStripMenuItem, ShaderGlassConfigToolStripMenuItem})
        ConfigToolStripMenuItem1.Name = "ConfigToolStripMenuItem1"
        ConfigToolStripMenuItem1.Size = New Size(67, 24)
        ConfigToolStripMenuItem1.Text = "Config"
        ' 
        ' KeyConfiguratorToolStripMenuItem
        ' 
        KeyConfiguratorToolStripMenuItem.Name = "KeyConfiguratorToolStripMenuItem"
        KeyConfiguratorToolStripMenuItem.Size = New Size(236, 26)
        KeyConfiguratorToolStripMenuItem.Text = "Key2Pad Configurator"
        ' 
        ' AppConfigToolStripMenuItem
        ' 
        AppConfigToolStripMenuItem.Name = "AppConfigToolStripMenuItem"
        AppConfigToolStripMenuItem.Size = New Size(236, 26)
        AppConfigToolStripMenuItem.Text = "Application Config"
        ' 
        ' ShaderGlassConfigToolStripMenuItem
        ' 
        ShaderGlassConfigToolStripMenuItem.Name = "ShaderGlassConfigToolStripMenuItem"
        ShaderGlassConfigToolStripMenuItem.Size = New Size(236, 26)
        ShaderGlassConfigToolStripMenuItem.Text = "ShaderGlass Config"
        ' 
        ' ToolsToolStripMenuItem
        ' 
        ToolsToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {BatchDownloadToolStripMenuItem, XMLCreationToolStripMenuItem, XMLUpdateToolStripMenuItem})
        ToolsToolStripMenuItem.Name = "ToolsToolStripMenuItem"
        ToolsToolStripMenuItem.Size = New Size(58, 24)
        ToolsToolStripMenuItem.Text = "Tools"
        ' 
        ' BatchDownloadToolStripMenuItem
        ' 
        BatchDownloadToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {GamesToolStripMenuItem, MachiCharaToolStripMenuItem})
        BatchDownloadToolStripMenuItem.Name = "BatchDownloadToolStripMenuItem"
        BatchDownloadToolStripMenuItem.Size = New Size(202, 26)
        BatchDownloadToolStripMenuItem.Text = "Batch Download"
        ' 
        ' GamesToolStripMenuItem
        ' 
        GamesToolStripMenuItem.Name = "GamesToolStripMenuItem"
        GamesToolStripMenuItem.Size = New Size(174, 26)
        GamesToolStripMenuItem.Text = "Games"
        ' 
        ' MachiCharaToolStripMenuItem
        ' 
        MachiCharaToolStripMenuItem.Name = "MachiCharaToolStripMenuItem"
        MachiCharaToolStripMenuItem.Size = New Size(174, 26)
        MachiCharaToolStripMenuItem.Text = "Machi Chara"
        ' 
        ' XMLCreationToolStripMenuItem
        ' 
        XMLCreationToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {GamelistToolStripMenuItem})
        XMLCreationToolStripMenuItem.Name = "XMLCreationToolStripMenuItem"
        XMLCreationToolStripMenuItem.Size = New Size(202, 26)
        XMLCreationToolStripMenuItem.Text = "XML Creation"
        ' 
        ' GamelistToolStripMenuItem
        ' 
        GamelistToolStripMenuItem.Name = "GamelistToolStripMenuItem"
        GamelistToolStripMenuItem.Size = New Size(150, 26)
        GamelistToolStripMenuItem.Text = "Gamelist"
        ' 
        ' XMLUpdateToolStripMenuItem
        ' 
        XMLUpdateToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {AddGameToolStripMenuItem})
        XMLUpdateToolStripMenuItem.Name = "XMLUpdateToolStripMenuItem"
        XMLUpdateToolStripMenuItem.Size = New Size(202, 26)
        XMLUpdateToolStripMenuItem.Text = "XML Update"
        ' 
        ' AddGameToolStripMenuItem
        ' 
        AddGameToolStripMenuItem.Name = "AddGameToolStripMenuItem"
        AddGameToolStripMenuItem.Size = New Size(163, 26)
        AddGameToolStripMenuItem.Text = "Add Game"
        ' 
        ' HelpToolStripMenuItem
        ' 
        HelpToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {AboutToolStripMenuItem})
        HelpToolStripMenuItem.Name = "HelpToolStripMenuItem"
        HelpToolStripMenuItem.Size = New Size(55, 24)
        HelpToolStripMenuItem.Text = "Help"
        ' 
        ' AboutToolStripMenuItem
        ' 
        AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        AboutToolStripMenuItem.Size = New Size(133, 26)
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
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(120F, 120F)
        AutoScaleMode = AutoScaleMode.Dpi
        ClientSize = New Size(1125, 801)
        Controls.Add(pbGameDL)
        Controls.Add(TabControl1)
        Controls.Add(MenuStrip1)
        MainMenuStrip = MenuStrip1
        Name = "Form1"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Keitai World Launcher"
        GroupBox1.ResumeLayout(False)
        GroupBox1.PerformLayout()
        cmsGameLV.ResumeLayout(False)
        TabControl1.ResumeLayout(False)
        tpGames.ResumeLayout(False)
        tpGames.PerformLayout()
        tpMachiChara.ResumeLayout(False)
        GroupBox2.ResumeLayout(False)
        MenuStrip1.ResumeLayout(False)
        MenuStrip1.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents pbGameDL As ProgressBar
    Friend WithEvents gbxGameInfo As GroupBox
    Friend WithEvents btnLaunchGame As Button
    Friend WithEvents chkbxHidePhoneUI As CheckBox
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents tpGames As TabPage
    Friend WithEvents tpMachiChara As TabPage
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents FileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents HelpToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents lbxMachiCharaList As ListBox
    Friend WithEvents btnMachiCharaLaunch As Button
    Friend WithEvents BatchDownloadToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents GamesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MachiCharaToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents XMLCreationToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents lblTotalGameCount As Label
    Friend WithEvents AboutToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents GamelistToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OpenFileDialog1 As OpenFileDialog
    Friend WithEvents ListViewGames As ListView
    Friend WithEvents ImageListGames As ImageList
    Friend WithEvents cobxAudioType As ComboBox
    Friend WithEvents Label1 As Label
    Friend WithEvents RefreshToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ConfigToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents AppConfigToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents KeyConfiguratorToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents Label2 As Label
    Friend WithEvents txtLVSearch As TextBox
    Friend WithEvents cbxFilterType As ComboBox
    Friend WithEvents cmsGameLV As ContextMenuStrip
    Friend WithEvents cmsGameLV_Launch As ToolStripMenuItem
    Friend WithEvents cmsGameLV_Download As ToolStripMenuItem
    Friend WithEvents cmsGameLV_Delete As ToolStripMenuItem
    Friend WithEvents chkbxShaderGlass As CheckBox
    Friend WithEvents XMLUpdateToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AddGameToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ShaderGlassConfigToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ListViewGamesVariants As ListView
    Friend WithEvents FavoriteToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents selectionTimer As Timer
    Friend WithEvents Label4 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents cbxStarSDK As ComboBox
    Friend WithEvents cbxDojaSDK As ComboBox
End Class
