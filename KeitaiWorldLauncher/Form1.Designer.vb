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
        GroupBox1.Location = New Point(5, 4)
        GroupBox1.Margin = New Padding(3, 2, 3, 2)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Padding = New Padding(3, 2, 3, 2)
        GroupBox1.Size = New Size(349, 500)
        GroupBox1.TabIndex = 0
        GroupBox1.TabStop = False
        GroupBox1.Text = "Available Games"
        ' 
        ' ListViewGamesVariants
        ' 
        ListViewGamesVariants.LargeImageList = ImageListGames
        ListViewGamesVariants.Location = New Point(5, 368)
        ListViewGamesVariants.Margin = New Padding(3, 2, 3, 2)
        ListViewGamesVariants.Name = "ListViewGamesVariants"
        ListViewGamesVariants.Size = New Size(339, 113)
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
        cbxFilterType.Location = New Point(258, 19)
        cbxFilterType.Margin = New Padding(3, 2, 3, 2)
        cbxFilterType.Name = "cbxFilterType"
        cbxFilterType.Size = New Size(86, 23)
        cbxFilterType.TabIndex = 9
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(9, 22)
        Label2.Name = "Label2"
        Label2.Size = New Size(45, 15)
        Label2.TabIndex = 8
        Label2.Text = "Search:"
        ' 
        ' txtLVSearch
        ' 
        txtLVSearch.Location = New Point(63, 20)
        txtLVSearch.Margin = New Padding(3, 2, 3, 2)
        txtLVSearch.Name = "txtLVSearch"
        txtLVSearch.Size = New Size(190, 23)
        txtLVSearch.TabIndex = 7
        ' 
        ' ListViewGames
        ' 
        ListViewGames.ContextMenuStrip = cmsGameLV
        ListViewGames.LargeImageList = ImageListGames
        ListViewGames.Location = New Point(5, 44)
        ListViewGames.Margin = New Padding(3, 2, 3, 2)
        ListViewGames.Name = "ListViewGames"
        ListViewGames.Size = New Size(339, 320)
        ListViewGames.TabIndex = 6
        ListViewGames.UseCompatibleStateImageBehavior = False
        ListViewGames.View = View.Details
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
        ' lblTotalGameCount
        ' 
        lblTotalGameCount.AutoSize = True
        lblTotalGameCount.Location = New Point(5, 483)
        lblTotalGameCount.Name = "lblTotalGameCount"
        lblTotalGameCount.Size = New Size(45, 15)
        lblTotalGameCount.TabIndex = 1
        lblTotalGameCount.Text = "Total: 0"
        ' 
        ' pbGameDL
        ' 
        pbGameDL.Location = New Point(14, 568)
        pbGameDL.Margin = New Padding(3, 2, 3, 2)
        pbGameDL.Name = "pbGameDL"
        pbGameDL.Size = New Size(958, 22)
        pbGameDL.TabIndex = 1
        pbGameDL.Visible = False
        ' 
        ' gbxGameInfo
        ' 
        gbxGameInfo.Location = New Point(360, 4)
        gbxGameInfo.Margin = New Padding(3, 2, 3, 2)
        gbxGameInfo.Name = "gbxGameInfo"
        gbxGameInfo.Padding = New Padding(3, 2, 3, 2)
        gbxGameInfo.Size = New Size(592, 451)
        gbxGameInfo.TabIndex = 1
        gbxGameInfo.TabStop = False
        gbxGameInfo.Text = "Game Info"
        ' 
        ' btnLaunchGame
        ' 
        btnLaunchGame.Enabled = False
        btnLaunchGame.Location = New Point(360, 459)
        btnLaunchGame.Margin = New Padding(3, 2, 3, 2)
        btnLaunchGame.Name = "btnLaunchGame"
        btnLaunchGame.Size = New Size(120, 50)
        btnLaunchGame.TabIndex = 2
        btnLaunchGame.Text = "Launch"
        btnLaunchGame.UseVisualStyleBackColor = True
        ' 
        ' chkbxHidePhoneUI
        ' 
        chkbxHidePhoneUI.AutoSize = True
        chkbxHidePhoneUI.Enabled = False
        chkbxHidePhoneUI.Location = New Point(485, 459)
        chkbxHidePhoneUI.Margin = New Padding(3, 2, 3, 2)
        chkbxHidePhoneUI.Name = "chkbxHidePhoneUI"
        chkbxHidePhoneUI.Size = New Size(102, 19)
        chkbxHidePhoneUI.TabIndex = 3
        chkbxHidePhoneUI.Text = "Hide phone UI"
        chkbxHidePhoneUI.UseVisualStyleBackColor = True
        ' 
        ' TabControl1
        ' 
        TabControl1.Controls.Add(tpGames)
        TabControl1.Controls.Add(tpMachiChara)
        TabControl1.Location = New Point(10, 23)
        TabControl1.Margin = New Padding(3, 2, 3, 2)
        TabControl1.Name = "TabControl1"
        TabControl1.SelectedIndex = 0
        TabControl1.Size = New Size(965, 541)
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
        tpGames.Location = New Point(4, 24)
        tpGames.Margin = New Padding(3, 2, 3, 2)
        tpGames.Name = "tpGames"
        tpGames.Padding = New Padding(3, 2, 3, 2)
        tpGames.Size = New Size(957, 513)
        tpGames.TabIndex = 0
        tpGames.Text = "Games"
        ' 
        ' chkbxShaderGlass
        ' 
        chkbxShaderGlass.AutoSize = True
        chkbxShaderGlass.Enabled = False
        chkbxShaderGlass.Location = New Point(601, 459)
        chkbxShaderGlass.Margin = New Padding(3, 2, 3, 2)
        chkbxShaderGlass.Name = "chkbxShaderGlass"
        chkbxShaderGlass.Size = New Size(89, 19)
        chkbxShaderGlass.TabIndex = 6
        chkbxShaderGlass.Text = "ShaderGlass"
        chkbxShaderGlass.UseVisualStyleBackColor = True
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(601, 489)
        Label1.Name = "Label1"
        Label1.Size = New Size(67, 15)
        Label1.TabIndex = 5
        Label1.Text = "Audio Type"
        ' 
        ' cobxAudioType
        ' 
        cobxAudioType.Enabled = False
        cobxAudioType.FormattingEnabled = True
        cobxAudioType.Items.AddRange(New Object() {"Standard", "903i"})
        cobxAudioType.Location = New Point(485, 487)
        cobxAudioType.Margin = New Padding(3, 2, 3, 2)
        cobxAudioType.Name = "cobxAudioType"
        cobxAudioType.Size = New Size(112, 23)
        cobxAudioType.TabIndex = 4
        ' 
        ' tpMachiChara
        ' 
        tpMachiChara.BackColor = Color.WhiteSmoke
        tpMachiChara.Controls.Add(btnMachiCharaLaunch)
        tpMachiChara.Controls.Add(GroupBox2)
        tpMachiChara.Location = New Point(4, 24)
        tpMachiChara.Margin = New Padding(3, 2, 3, 2)
        tpMachiChara.Name = "tpMachiChara"
        tpMachiChara.Padding = New Padding(3, 2, 3, 2)
        tpMachiChara.Size = New Size(957, 513)
        tpMachiChara.TabIndex = 1
        tpMachiChara.Text = "Machi Chara"
        ' 
        ' btnMachiCharaLaunch
        ' 
        btnMachiCharaLaunch.Enabled = False
        btnMachiCharaLaunch.Location = New Point(277, 28)
        btnMachiCharaLaunch.Margin = New Padding(3, 2, 3, 2)
        btnMachiCharaLaunch.Name = "btnMachiCharaLaunch"
        btnMachiCharaLaunch.Size = New Size(120, 50)
        btnMachiCharaLaunch.TabIndex = 3
        btnMachiCharaLaunch.Text = "Launch"
        btnMachiCharaLaunch.UseVisualStyleBackColor = True
        ' 
        ' GroupBox2
        ' 
        GroupBox2.Controls.Add(lbxMachiCharaList)
        GroupBox2.Location = New Point(10, 9)
        GroupBox2.Margin = New Padding(3, 2, 3, 2)
        GroupBox2.Name = "GroupBox2"
        GroupBox2.Padding = New Padding(3, 2, 3, 2)
        GroupBox2.Size = New Size(262, 455)
        GroupBox2.TabIndex = 1
        GroupBox2.TabStop = False
        GroupBox2.Text = "Available Machi Chara"
        ' 
        ' lbxMachiCharaList
        ' 
        lbxMachiCharaList.FormattingEnabled = True
        lbxMachiCharaList.ItemHeight = 15
        lbxMachiCharaList.Location = New Point(5, 20)
        lbxMachiCharaList.Margin = New Padding(3, 2, 3, 2)
        lbxMachiCharaList.Name = "lbxMachiCharaList"
        lbxMachiCharaList.Size = New Size(252, 424)
        lbxMachiCharaList.TabIndex = 0
        ' 
        ' MenuStrip1
        ' 
        MenuStrip1.ImageScalingSize = New Size(20, 20)
        MenuStrip1.Items.AddRange(New ToolStripItem() {FileToolStripMenuItem, ConfigToolStripMenuItem1, ToolsToolStripMenuItem, HelpToolStripMenuItem})
        MenuStrip1.Location = New Point(0, 0)
        MenuStrip1.Name = "MenuStrip1"
        MenuStrip1.Padding = New Padding(5, 2, 0, 2)
        MenuStrip1.Size = New Size(984, 24)
        MenuStrip1.TabIndex = 5
        MenuStrip1.Text = "MenuStrip1"
        ' 
        ' FileToolStripMenuItem
        ' 
        FileToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {RefreshToolStripMenuItem, ExitToolStripMenuItem})
        FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        FileToolStripMenuItem.Size = New Size(37, 20)
        FileToolStripMenuItem.Text = "File"
        ' 
        ' RefreshToolStripMenuItem
        ' 
        RefreshToolStripMenuItem.Name = "RefreshToolStripMenuItem"
        RefreshToolStripMenuItem.Size = New Size(113, 22)
        RefreshToolStripMenuItem.Text = "Refresh"
        ' 
        ' ExitToolStripMenuItem
        ' 
        ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        ExitToolStripMenuItem.Size = New Size(113, 22)
        ExitToolStripMenuItem.Text = "Exit"
        ' 
        ' ConfigToolStripMenuItem1
        ' 
        ConfigToolStripMenuItem1.DropDownItems.AddRange(New ToolStripItem() {KeyConfiguratorToolStripMenuItem, AppConfigToolStripMenuItem, ShaderGlassConfigToolStripMenuItem})
        ConfigToolStripMenuItem1.Name = "ConfigToolStripMenuItem1"
        ConfigToolStripMenuItem1.Size = New Size(55, 20)
        ConfigToolStripMenuItem1.Text = "Config"
        ' 
        ' KeyConfiguratorToolStripMenuItem
        ' 
        KeyConfiguratorToolStripMenuItem.Name = "KeyConfiguratorToolStripMenuItem"
        KeyConfiguratorToolStripMenuItem.Size = New Size(190, 22)
        KeyConfiguratorToolStripMenuItem.Text = "Key2Pad Configurator"
        ' 
        ' AppConfigToolStripMenuItem
        ' 
        AppConfigToolStripMenuItem.Name = "AppConfigToolStripMenuItem"
        AppConfigToolStripMenuItem.Size = New Size(190, 22)
        AppConfigToolStripMenuItem.Text = "Application Config"
        ' 
        ' ShaderGlassConfigToolStripMenuItem
        ' 
        ShaderGlassConfigToolStripMenuItem.Name = "ShaderGlassConfigToolStripMenuItem"
        ShaderGlassConfigToolStripMenuItem.Size = New Size(190, 22)
        ShaderGlassConfigToolStripMenuItem.Text = "ShaderGlass Config"
        ' 
        ' ToolsToolStripMenuItem
        ' 
        ToolsToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {BatchDownloadToolStripMenuItem, XMLCreationToolStripMenuItem, XMLUpdateToolStripMenuItem})
        ToolsToolStripMenuItem.Name = "ToolsToolStripMenuItem"
        ToolsToolStripMenuItem.Size = New Size(47, 20)
        ToolsToolStripMenuItem.Text = "Tools"
        ' 
        ' BatchDownloadToolStripMenuItem
        ' 
        BatchDownloadToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {GamesToolStripMenuItem, MachiCharaToolStripMenuItem})
        BatchDownloadToolStripMenuItem.Name = "BatchDownloadToolStripMenuItem"
        BatchDownloadToolStripMenuItem.Size = New Size(161, 22)
        BatchDownloadToolStripMenuItem.Text = "Batch Download"
        ' 
        ' GamesToolStripMenuItem
        ' 
        GamesToolStripMenuItem.Name = "GamesToolStripMenuItem"
        GamesToolStripMenuItem.Size = New Size(141, 22)
        GamesToolStripMenuItem.Text = "Games"
        ' 
        ' MachiCharaToolStripMenuItem
        ' 
        MachiCharaToolStripMenuItem.Name = "MachiCharaToolStripMenuItem"
        MachiCharaToolStripMenuItem.Size = New Size(141, 22)
        MachiCharaToolStripMenuItem.Text = "Machi Chara"
        ' 
        ' XMLCreationToolStripMenuItem
        ' 
        XMLCreationToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {GamelistToolStripMenuItem})
        XMLCreationToolStripMenuItem.Name = "XMLCreationToolStripMenuItem"
        XMLCreationToolStripMenuItem.Size = New Size(161, 22)
        XMLCreationToolStripMenuItem.Text = "XML Creation"
        ' 
        ' GamelistToolStripMenuItem
        ' 
        GamelistToolStripMenuItem.Name = "GamelistToolStripMenuItem"
        GamelistToolStripMenuItem.Size = New Size(120, 22)
        GamelistToolStripMenuItem.Text = "Gamelist"
        ' 
        ' XMLUpdateToolStripMenuItem
        ' 
        XMLUpdateToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {AddGameToolStripMenuItem})
        XMLUpdateToolStripMenuItem.Name = "XMLUpdateToolStripMenuItem"
        XMLUpdateToolStripMenuItem.Size = New Size(161, 22)
        XMLUpdateToolStripMenuItem.Text = "XML Update"
        ' 
        ' AddGameToolStripMenuItem
        ' 
        AddGameToolStripMenuItem.Name = "AddGameToolStripMenuItem"
        AddGameToolStripMenuItem.Size = New Size(130, 22)
        AddGameToolStripMenuItem.Text = "Add Game"
        ' 
        ' HelpToolStripMenuItem
        ' 
        HelpToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {AboutToolStripMenuItem})
        HelpToolStripMenuItem.Name = "HelpToolStripMenuItem"
        HelpToolStripMenuItem.Size = New Size(44, 20)
        HelpToolStripMenuItem.Text = "Help"
        ' 
        ' AboutToolStripMenuItem
        ' 
        AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        AboutToolStripMenuItem.Size = New Size(107, 22)
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
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(984, 601)
        Controls.Add(pbGameDL)
        Controls.Add(TabControl1)
        Controls.Add(MenuStrip1)
        MainMenuStrip = MenuStrip1
        Margin = New Padding(3, 2, 3, 2)
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
End Class
