<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
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
        cbxEmuType = New ComboBox()
        Label2 = New Label()
        txtLVSearch = New TextBox()
        ListViewGames = New ListView()
        cmsGameLV = New ContextMenuStrip(components)
        cmsGameLV_Launch = New ToolStripMenuItem()
        cmsGameLV_Download = New ToolStripMenuItem()
        cmsGameLV_Delete = New ToolStripMenuItem()
        ImageListGames = New ImageList(components)
        lblTotalGameCount = New Label()
        pbGameDL = New ProgressBar()
        gbxGameInfo = New GroupBox()
        btnLaunchGame = New Button()
        chkbxHidePhoneUI = New CheckBox()
        TabControl1 = New TabControl()
        tpGames = New TabPage()
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
        AppConfigToolStripMenuItem = New ToolStripMenuItem()
        KeyConfiguratorToolStripMenuItem = New ToolStripMenuItem()
        ToolsToolStripMenuItem = New ToolStripMenuItem()
        BatchDownloadToolStripMenuItem = New ToolStripMenuItem()
        GamesToolStripMenuItem = New ToolStripMenuItem()
        MachiCharaToolStripMenuItem = New ToolStripMenuItem()
        XMLCreationToolStripMenuItem = New ToolStripMenuItem()
        GamelistToolStripMenuItem = New ToolStripMenuItem()
        HelpToolStripMenuItem = New ToolStripMenuItem()
        AboutToolStripMenuItem = New ToolStripMenuItem()
        OpenFileDialog1 = New OpenFileDialog()
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
        GroupBox1.Controls.Add(cbxEmuType)
        GroupBox1.Controls.Add(Label2)
        GroupBox1.Controls.Add(txtLVSearch)
        GroupBox1.Controls.Add(ListViewGames)
        GroupBox1.Controls.Add(lblTotalGameCount)
        GroupBox1.Location = New Point(6, 6)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Size = New Size(399, 607)
        GroupBox1.TabIndex = 0
        GroupBox1.TabStop = False
        GroupBox1.Text = "Available Games"
        ' 
        ' cbxEmuType
        ' 
        cbxEmuType.FormattingEnabled = True
        cbxEmuType.Items.AddRange(New Object() {"All", "Doja", "Star"})
        cbxEmuType.Location = New Point(295, 25)
        cbxEmuType.Name = "cbxEmuType"
        cbxEmuType.Size = New Size(98, 28)
        cbxEmuType.TabIndex = 9
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
        txtLVSearch.Location = New Point(72, 26)
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
        ListViewGames.Size = New Size(387, 519)
        ListViewGames.TabIndex = 6
        ListViewGames.UseCompatibleStateImageBehavior = False
        ListViewGames.View = View.Details
        ' 
        ' cmsGameLV
        ' 
        cmsGameLV.ImageScalingSize = New Size(20, 20)
        cmsGameLV.Items.AddRange(New ToolStripItem() {cmsGameLV_Launch, cmsGameLV_Download, cmsGameLV_Delete})
        cmsGameLV.Name = "cmsGameLV"
        cmsGameLV.Size = New Size(148, 76)
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
        ' ImageListGames
        ' 
        ImageListGames.ColorDepth = ColorDepth.Depth32Bit
        ImageListGames.ImageSize = New Size(24, 24)
        ImageListGames.TransparentColor = Color.Transparent
        ' 
        ' lblTotalGameCount
        ' 
        lblTotalGameCount.AutoSize = True
        lblTotalGameCount.Location = New Point(6, 581)
        lblTotalGameCount.Name = "lblTotalGameCount"
        lblTotalGameCount.Size = New Size(57, 20)
        lblTotalGameCount.TabIndex = 1
        lblTotalGameCount.Text = "Total: 0"
        ' 
        ' pbGameDL
        ' 
        pbGameDL.Location = New Point(28, 695)
        pbGameDL.Name = "pbGameDL"
        pbGameDL.Size = New Size(287, 29)
        pbGameDL.TabIndex = 1
        ' 
        ' gbxGameInfo
        ' 
        gbxGameInfo.Location = New Point(411, 6)
        gbxGameInfo.Name = "gbxGameInfo"
        gbxGameInfo.Size = New Size(676, 529)
        gbxGameInfo.TabIndex = 1
        gbxGameInfo.TabStop = False
        gbxGameInfo.Text = "Game Info"
        ' 
        ' btnLaunchGame
        ' 
        btnLaunchGame.Enabled = False
        btnLaunchGame.Location = New Point(411, 541)
        btnLaunchGame.Name = "btnLaunchGame"
        btnLaunchGame.Size = New Size(137, 66)
        btnLaunchGame.TabIndex = 2
        btnLaunchGame.Text = "Launch"
        btnLaunchGame.UseVisualStyleBackColor = True
        ' 
        ' chkbxHidePhoneUI
        ' 
        chkbxHidePhoneUI.AutoSize = True
        chkbxHidePhoneUI.Enabled = False
        chkbxHidePhoneUI.Location = New Point(554, 541)
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
        TabControl1.Location = New Point(12, 31)
        TabControl1.Name = "TabControl1"
        TabControl1.SelectedIndex = 0
        TabControl1.Size = New Size(1103, 658)
        TabControl1.TabIndex = 4
        ' 
        ' tpGames
        ' 
        tpGames.BackColor = Color.WhiteSmoke
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
        tpGames.Size = New Size(1095, 625)
        tpGames.TabIndex = 0
        tpGames.Text = "Games"
        ' 
        ' chkbxShaderGlass
        ' 
        chkbxShaderGlass.AutoSize = True
        chkbxShaderGlass.Enabled = False
        chkbxShaderGlass.Location = New Point(687, 541)
        chkbxShaderGlass.Name = "chkbxShaderGlass"
        chkbxShaderGlass.Size = New Size(111, 24)
        chkbxShaderGlass.TabIndex = 6
        chkbxShaderGlass.Text = "ShaderGlass"
        chkbxShaderGlass.UseVisualStyleBackColor = True
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(687, 582)
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
        cobxAudioType.Location = New Point(554, 579)
        cobxAudioType.Name = "cobxAudioType"
        cobxAudioType.Size = New Size(127, 28)
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
        tpMachiChara.Size = New Size(1095, 625)
        tpMachiChara.TabIndex = 1
        tpMachiChara.Text = "Machi Chara"
        ' 
        ' btnMachiCharaLaunch
        ' 
        btnMachiCharaLaunch.Enabled = False
        btnMachiCharaLaunch.Location = New Point(317, 38)
        btnMachiCharaLaunch.Name = "btnMachiCharaLaunch"
        btnMachiCharaLaunch.Size = New Size(137, 66)
        btnMachiCharaLaunch.TabIndex = 3
        btnMachiCharaLaunch.Text = "Launch"
        btnMachiCharaLaunch.UseVisualStyleBackColor = True
        ' 
        ' GroupBox2
        ' 
        GroupBox2.Controls.Add(lbxMachiCharaList)
        GroupBox2.Location = New Point(12, 12)
        GroupBox2.Name = "GroupBox2"
        GroupBox2.Size = New Size(299, 607)
        GroupBox2.TabIndex = 1
        GroupBox2.TabStop = False
        GroupBox2.Text = "Available Machi Chara"
        ' 
        ' lbxMachiCharaList
        ' 
        lbxMachiCharaList.FormattingEnabled = True
        lbxMachiCharaList.Location = New Point(6, 26)
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
        MenuStrip1.Size = New Size(1124, 28)
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
        ConfigToolStripMenuItem1.DropDownItems.AddRange(New ToolStripItem() {AppConfigToolStripMenuItem, KeyConfiguratorToolStripMenuItem})
        ConfigToolStripMenuItem1.Name = "ConfigToolStripMenuItem1"
        ConfigToolStripMenuItem1.Size = New Size(67, 24)
        ConfigToolStripMenuItem1.Text = "Config"
        ' 
        ' AppConfigToolStripMenuItem
        ' 
        AppConfigToolStripMenuItem.Name = "AppConfigToolStripMenuItem"
        AppConfigToolStripMenuItem.Size = New Size(236, 26)
        AppConfigToolStripMenuItem.Text = "Application Config"
        ' 
        ' KeyConfiguratorToolStripMenuItem
        ' 
        KeyConfiguratorToolStripMenuItem.Name = "KeyConfiguratorToolStripMenuItem"
        KeyConfiguratorToolStripMenuItem.Size = New Size(236, 26)
        KeyConfiguratorToolStripMenuItem.Text = "Key2Pad Configurator"
        ' 
        ' ToolsToolStripMenuItem
        ' 
        ToolsToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {BatchDownloadToolStripMenuItem, XMLCreationToolStripMenuItem})
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
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1124, 738)
        Controls.Add(pbGameDL)
        Controls.Add(TabControl1)
        Controls.Add(MenuStrip1)
        MainMenuStrip = MenuStrip1
        Name = "Form1"
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
    Friend WithEvents cbxEmuType As ComboBox
    Friend WithEvents cmsGameLV As ContextMenuStrip
    Friend WithEvents cmsGameLV_Launch As ToolStripMenuItem
    Friend WithEvents cmsGameLV_Download As ToolStripMenuItem
    Friend WithEvents cmsGameLV_Delete As ToolStripMenuItem
    Friend WithEvents chkbxShaderGlass As CheckBox
End Class
