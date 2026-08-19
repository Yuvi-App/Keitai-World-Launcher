Imports System.IO
Imports KeitaiWorldLauncher.My.Models

Partial Public Class MainForm
    Private _compactLibraryInitialized As Boolean
    Private _appLibraryGrid As TableLayoutPanel
    Private _machiLibraryGrid As TableLayoutPanel
    Private _charaLibraryGrid As TableLayoutPanel
    Private _libraryRoot As TableLayoutPanel
    Private _libraryCategoryTabs As TabControl
    Private _downloadQueueBorder As Panel
    Private _downloadQueueTitle As Label
    Private _downloadQueueStatus As Label
    Private _downloadQueueCount As Label

    Private _gameActionBar As FlowLayoutPanel
    Private _btnGameActions As Button
    Private _gameActionsMenu As ContextMenuStrip
    Private _actionRedownload As ToolStripMenuItem
    Private _actionBackupSave As ToolStripMenuItem
    Private _actionFavorite As ToolStripMenuItem
    Private _actionOpenFolder As ToolStripMenuItem
    Private _actionDelete As ToolStripMenuItem
    Private _txtMachiSearch As TextBox
    Private _lblMachiTitle As Label
    Private _lblMachiMetadata As Label
    Private _lblMachiStatus As Label
    Private _btnMachiActions As Button
    Private _machiActionsMenu As ContextMenuStrip
    Private _actionMachiRedownload As ToolStripMenuItem
    Private _actionMachiDelete As ToolStripMenuItem

    Private _txtCharaSearch As TextBox
    Private _lblCharaTitle As Label
    Private _lblCharaMetadata As Label
    Private _lblCharaStatus As Label
    Private _btnCharaActions As Button
    Private _charaActionsMenu As ContextMenuStrip
    Private _actionCharaRedownload As ToolStripMenuItem
    Private _actionCharaDelete As ToolStripMenuItem

    Private _lblActivityTotalTime As Label
    Private _lblActivityTotalTimeHint As Label
    Private _lblActivitySessions As Label
    Private _lblActivitySessionsHint As Label
    Private _lblActivityApps As Label
    Private _lblActivityAppsHint As Label
    Private _lblActivityMostPlayed As Label
    Private _lblActivityMostPlayedHint As Label
    Private _lblActivityHistoryCount As Label
    Private _activityEmptyState As Label

    Private _settingsNavigation As ListBox
    Private _settingsPages As List(Of Panel)

    Private Sub InitializeCompactLibrary()
        If _compactLibraryInitialized Then Return
        _compactLibraryInitialized = True

        SuspendLayout()
        Try
            ClientSize = New Size(1280, 800)
            MinimumSize = New Size(1100, 720)
            MaximizeBox = True
            Sizable = True
            BackColor = CompactUiTheme.AppBackground

            tpAppli.Text = "Library"
            tpConfig.Text = "Settings"
            tpStats.Text = "Activity"
            tpAppli.BackColor = CompactUiTheme.AppBackground

            BuildUnifiedLibraryTabs()
            BuildCompactAppLayout()
            BuildCompactMachiCharaLayout()
            BuildCompactCharaDenLayout()
            StyleActivityView()
            StyleSettingsView()

            AddHandler Resize, AddressOf MainForm_ResponsiveResize
            AddHandler ListViewGames.SizeChanged, AddressOf LibraryList_SizeChanged
            AddHandler ListViewMachiChara.SizeChanged, AddressOf LibraryList_SizeChanged
            AddHandler ListViewCharaDen.SizeChanged, AddressOf LibraryList_SizeChanged

            ShowNoGameSelected()
            ShowNoMachiCharaSelected()
            ShowNoCharaDenSelected()
            UpdateResponsiveShellLayout()
        Finally
            ResumeLayout(True)
        End Try
    End Sub

    Private Sub StyleActivityView()
        tpStats.SuspendLayout()
        tpStats.Controls.Clear()
        tpStats.BackColor = CompactUiTheme.AppBackground
        tpStats.Padding = New Padding(16, 14, 16, 16)

        Dim root As New TableLayoutPanel With {
            .BackColor = CompactUiTheme.AppBackground,
            .ColumnCount = 1,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0),
            .Padding = New Padding(0),
            .RowCount = 3
        }
        root.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        root.RowStyles.Add(New RowStyle(SizeType.Absolute, 58.0F))
        root.RowStyles.Add(New RowStyle(SizeType.Absolute, 116.0F))
        root.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        Dim header As New Panel With {
            .BackColor = CompactUiTheme.AppBackground,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0)
        }
        Dim titleLabel As New Label With {
            .AutoSize = False,
            .Font = New Font("Segoe UI Semibold", 17.0F, FontStyle.Bold),
            .ForeColor = CompactUiTheme.TextPrimary,
            .Location = New Point(0, 0),
            .Size = New Size(340, 31),
            .Text = "Activity overview",
            .TextAlign = ContentAlignment.MiddleLeft
        }
        Dim subtitleLabel As New Label With {
            .AutoSize = False,
            .Font = New Font("Segoe UI", 9.0F),
            .ForeColor = CompactUiTheme.TextSecondary,
            .Location = New Point(2, 32),
            .Size = New Size(420, 20),
            .Text = "Your play history across the launcher.",
            .TextAlign = ContentAlignment.MiddleLeft
        }
        Dim refreshButton = CompactUiTheme.CreateCompactButton("Refresh")
        refreshButton.AccessibleName = "Refresh activity"
        refreshButton.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        refreshButton.Size = New Size(86, 32)
        refreshButton.Location = New Point(Math.Max(0, header.Width - refreshButton.Width), 8)
        AddHandler refreshButton.Click, AddressOf ActivityRefresh_Click
        AddHandler header.Resize,
            Sub()
                refreshButton.Left = Math.Max(0, header.ClientSize.Width - refreshButton.Width)
            End Sub
        header.Controls.AddRange(New Control() {titleLabel, subtitleLabel, refreshButton})

        Dim cards As New TableLayoutPanel With {
            .BackColor = CompactUiTheme.AppBackground,
            .ColumnCount = 4,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0, 0, 0, 12),
            .Padding = New Padding(0),
            .RowCount = 1
        }
        For index = 0 To 3
            cards.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25.0F))
        Next
        cards.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        Dim totalCard = CreateActivityStatCard(
            "TOTAL PLAY TIME", "0m", "Across all tracked apps", CompactUiTheme.Primary,
            _lblActivityTotalTime, _lblActivityTotalTimeHint)
        Dim sessionsCard = CreateActivityStatCard(
            "SESSIONS", "0", "Times apps were launched", CompactUiTheme.Accent,
            _lblActivitySessions, _lblActivitySessionsHint)
        Dim appsCard = CreateActivityStatCard(
            "APPS PLAYED", "0", "Apps with recorded activity", CompactUiTheme.Success,
            _lblActivityApps, _lblActivityAppsHint)
        Dim mostPlayedCard = CreateActivityStatCard(
            "MOST PLAYED", ChrW(&H2014), "No playtime recorded", Color.FromArgb(104, 78, 153),
            _lblActivityMostPlayed, _lblActivityMostPlayedHint)

        totalCard.Margin = New Padding(0, 0, 9, 0)
        sessionsCard.Margin = New Padding(3, 0, 6, 0)
        appsCard.Margin = New Padding(6, 0, 3, 0)
        mostPlayedCard.Margin = New Padding(9, 0, 0, 0)
        cards.Controls.Add(totalCard, 0, 0)
        cards.Controls.Add(sessionsCard, 1, 0)
        cards.Controls.Add(appsCard, 2, 0)
        cards.Controls.Add(mostPlayedCard, 3, 0)

        Dim historyBorder As New Panel With {
            .BackColor = CompactUiTheme.Border,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0),
            .Padding = New Padding(1)
        }
        Dim historySurface As New TableLayoutPanel With {
            .BackColor = CompactUiTheme.Surface,
            .ColumnCount = 1,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0),
            .Padding = New Padding(0),
            .RowCount = 3
        }
        historySurface.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        historySurface.RowStyles.Add(New RowStyle(SizeType.Absolute, 54.0F))
        historySurface.RowStyles.Add(New RowStyle(SizeType.Absolute, 1.0F))
        historySurface.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        historyBorder.Controls.Add(historySurface)

        Dim historyHeader As New Panel With {
            .BackColor = CompactUiTheme.Surface,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0),
            .Padding = New Padding(16, 0, 16, 0)
        }
        Dim historyTitle As New Label With {
            .Dock = DockStyle.Fill,
            .Font = New Font("Segoe UI Semibold", 11.0F, FontStyle.Bold),
            .ForeColor = CompactUiTheme.TextPrimary,
            .Text = "Play history",
            .TextAlign = ContentAlignment.MiddleLeft
        }
        _lblActivityHistoryCount = New Label With {
            .Dock = DockStyle.Right,
            .Font = New Font("Segoe UI", 9.0F),
            .ForeColor = CompactUiTheme.TextSecondary,
            .Size = New Size(120, 54),
            .Text = "0 apps",
            .TextAlign = ContentAlignment.MiddleRight
        }
        historyHeader.Controls.Add(historyTitle)
        historyHeader.Controls.Add(_lblActivityHistoryCount)
        _lblActivityHistoryCount.BringToFront()

        Dim historyDivider As New Panel With {
            .BackColor = CompactUiTheme.Border,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0)
        }
        Dim historyContent As New Panel With {
            .BackColor = CompactUiTheme.Surface,
            .Dock = DockStyle.Fill,
            .Padding = New Padding(12, 4, 12, 10)
        }
        Dim historyBody As New TableLayoutPanel With {
            .BackColor = CompactUiTheme.Surface,
            .ColumnCount = 1,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0),
            .Padding = New Padding(0),
            .RowCount = 2
        }
        historyBody.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        historyBody.RowStyles.Add(New RowStyle(SizeType.Absolute, 30.0F))
        historyBody.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        Dim historyColumns As New TableLayoutPanel With {
            .BackColor = Color.FromArgb(248, 249, 251),
            .ColumnCount = 3,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0),
            .Padding = New Padding(12, 0, 12, 0),
            .RowCount = 1
        }
        historyColumns.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 71.5F))
        historyColumns.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 20.0F))
        historyColumns.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 8.5F))
        historyColumns.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        historyColumns.Controls.Add(CreateActivityColumnLabel("APP", ContentAlignment.MiddleLeft), 0, 0)
        historyColumns.Controls.Add(CreateActivityColumnLabel("TOTAL TIME", ContentAlignment.MiddleLeft), 1, 0)
        historyColumns.Controls.Add(CreateActivityColumnLabel("SESSIONS", ContentAlignment.MiddleCenter), 2, 0)

        GroupBox9.Controls.Remove(lvwPlaytimes)
        lvwPlaytimes.Dock = DockStyle.Fill
        lvwPlaytimes.Font = New Font("Segoe UI", 9.5F)
        lvwPlaytimes.BackColor = CompactUiTheme.Surface
        lvwPlaytimes.BorderStyle = BorderStyle.None
        lvwPlaytimes.ForeColor = CompactUiTheme.TextPrimary
        lvwPlaytimes.GridLines = False
        lvwPlaytimes.HeaderStyle = ColumnHeaderStyle.None
        lvwPlaytimes.HideSelection = False
        lvwPlaytimes.MultiSelect = False
        EnableDoubleBuffering(lvwPlaytimes)

        _activityEmptyState = New Label With {
            .BackColor = CompactUiTheme.Surface,
            .Dock = DockStyle.Fill,
            .Font = New Font("Segoe UI", 10.0F),
            .ForeColor = CompactUiTheme.TextSecondary,
            .Text = "No activity yet" & Environment.NewLine & Environment.NewLine &
                    "Play an app and its history will appear here.",
            .TextAlign = ContentAlignment.MiddleCenter,
            .Visible = True
        }
        historyContent.Controls.Add(lvwPlaytimes)
        historyContent.Controls.Add(_activityEmptyState)
        _activityEmptyState.BringToFront()

        historyBody.Controls.Add(historyColumns, 0, 0)
        historyBody.Controls.Add(historyContent, 0, 1)

        historySurface.Controls.Add(historyHeader, 0, 0)
        historySurface.Controls.Add(historyDivider, 0, 1)
        historySurface.Controls.Add(historyBody, 0, 2)

        root.Controls.Add(header, 0, 0)
        root.Controls.Add(cards, 0, 1)
        root.Controls.Add(historyBorder, 0, 2)
        tpStats.Controls.Add(root)

        AddHandler lvwPlaytimes.SizeChanged, AddressOf ActivityList_SizeChanged
        tpStats.ResumeLayout(True)
    End Sub

    Private Function CreateActivityColumnLabel(text As String, alignment As ContentAlignment) As Label
        Return New Label With {
            .Dock = DockStyle.Fill,
            .Font = New Font("Segoe UI Semibold", 8.0F, FontStyle.Bold),
            .ForeColor = CompactUiTheme.TextSecondary,
            .Margin = New Padding(6, 0, 6, 0),
            .Text = text,
            .TextAlign = alignment
        }
    End Function

    Private Function CreateActivityStatCard(
        title As String,
        initialValue As String,
        hint As String,
        accentColor As Color,
        ByRef valueLabel As Label,
        ByRef hintLabel As Label
    ) As Panel
        Dim border As New Panel With {
            .BackColor = CompactUiTheme.Border,
            .Dock = DockStyle.Fill,
            .Padding = New Padding(1)
        }
        Dim surface As New Panel With {
            .BackColor = CompactUiTheme.Surface,
            .Dock = DockStyle.Fill
        }
        Dim accent As New Panel With {
            .BackColor = accentColor,
            .Dock = DockStyle.Left,
            .Width = 4
        }
        Dim content As New TableLayoutPanel With {
            .BackColor = CompactUiTheme.Surface,
            .ColumnCount = 1,
            .Dock = DockStyle.Fill,
            .Padding = New Padding(15, 10, 12, 8),
            .RowCount = 3
        }
        content.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        content.RowStyles.Add(New RowStyle(SizeType.Absolute, 18.0F))
        content.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        content.RowStyles.Add(New RowStyle(SizeType.Absolute, 18.0F))

        Dim titleLabel As New Label With {
            .Dock = DockStyle.Fill,
            .Font = New Font("Segoe UI Semibold", 8.0F, FontStyle.Bold),
            .ForeColor = CompactUiTheme.TextSecondary,
            .Text = title,
            .TextAlign = ContentAlignment.MiddleLeft
        }
        valueLabel = New Label With {
            .AutoEllipsis = True,
            .Dock = DockStyle.Fill,
            .Font = New Font("Segoe UI Semibold", 17.0F, FontStyle.Bold),
            .ForeColor = CompactUiTheme.TextPrimary,
            .Text = initialValue,
            .TextAlign = ContentAlignment.MiddleLeft
        }
        hintLabel = New Label With {
            .AutoEllipsis = True,
            .Dock = DockStyle.Fill,
            .Font = New Font("Segoe UI", 8.5F),
            .ForeColor = CompactUiTheme.TextSecondary,
            .Text = hint,
            .TextAlign = ContentAlignment.MiddleLeft
        }
        content.Controls.Add(titleLabel, 0, 0)
        content.Controls.Add(valueLabel, 0, 1)
        content.Controls.Add(hintLabel, 0, 2)
        surface.Controls.Add(content)
        surface.Controls.Add(accent)
        accent.BringToFront()
        border.Controls.Add(surface)
        Return border
    End Function

    Private Sub ActivityRefresh_Click(sender As Object, e As EventArgs)
        LoadPlaytimesToListView()
    End Sub

    Private Sub UpdateActivitySummary(entries As IEnumerable(Of PlaytimeEntry))
        Dim activityEntries = If(entries, Enumerable.Empty(Of PlaytimeEntry)()).ToList()
        Dim totalTime = activityEntries.Aggregate(TimeSpan.Zero, Function(total, entry) total.Add(entry.PlayTime))
        Dim totalSessions = activityEntries.Sum(Function(entry) entry.Sessions)
        Dim mostPlayed = activityEntries.OrderByDescending(Function(entry) entry.PlayTime).FirstOrDefault()

        _lblActivityTotalTime.Text = FormatActivityDuration(totalTime)
        _lblActivitySessions.Text = totalSessions.ToString("N0")
        _lblActivityApps.Text = activityEntries.Count.ToString("N0")
        _lblActivityMostPlayed.Text = If(mostPlayed Is Nothing, ChrW(&H2014), mostPlayed.AppName.Replace("_", " "))
        _lblActivityMostPlayedHint.Text = If(
            mostPlayed Is Nothing,
            "No playtime recorded",
            $"{FormatActivityDuration(mostPlayed.PlayTime)}  {ChrW(&H2022)}  {mostPlayed.Sessions} session{If(mostPlayed.Sessions = 1, String.Empty, "s")}")

        _lblActivityHistoryCount.Text = $"{activityEntries.Count:N0} app{If(activityEntries.Count = 1, String.Empty, "s")}"
        Dim hasActivity = activityEntries.Count > 0
        lvwPlaytimes.Visible = hasActivity
        _activityEmptyState.Visible = Not hasActivity
        If Not hasActivity Then _activityEmptyState.BringToFront()
    End Sub

    Private Function FormatActivityDuration(duration As TimeSpan) As String
        If duration <= TimeSpan.Zero Then Return "0m"
        If duration.TotalHours >= 1 Then Return $"{Math.Floor(duration.TotalHours):0}h {duration.Minutes}m"
        If duration.TotalMinutes >= 1 Then Return $"{Math.Floor(duration.TotalMinutes):0}m {duration.Seconds}s"
        Return $"{Math.Max(1, duration.Seconds)}s"
    End Function

    Private Sub ActivityList_SizeChanged(sender As Object, e As EventArgs)
        ResizeActivityColumns()
    End Sub

    Private Sub ResizeActivityColumns()
        If lvwPlaytimes Is Nothing OrElse lvwPlaytimes.Columns.Count < 3 Then Return
        Dim available = Math.Max(420, lvwPlaytimes.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 4)
        Dim sessionsWidth = 104
        Dim timeWidth = Math.Max(160, CInt(available * 0.2F))
        lvwPlaytimes.Columns(0).Width = Math.Max(180, available - timeWidth - sessionsWidth)
        lvwPlaytimes.Columns(1).Width = timeWidth
        lvwPlaytimes.Columns(2).Width = sessionsWidth
    End Sub

    Private Sub StyleSettingsView()
        tpConfig.SuspendLayout()
        tpConfig.Controls.Clear()
        tpConfig.BackColor = CompactUiTheme.AppBackground
        tpConfig.Padding = New Padding(16, 14, 16, 16)

        Dim root As New TableLayoutPanel With {
            .BackColor = CompactUiTheme.AppBackground,
            .ColumnCount = 1,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0),
            .Padding = New Padding(0),
            .RowCount = 2
        }
        root.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        root.RowStyles.Add(New RowStyle(SizeType.Absolute, 58.0F))
        root.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        Dim header As New Panel With {
            .BackColor = CompactUiTheme.AppBackground,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0)
        }
        Dim titleLabel As New Label With {
            .AutoSize = False,
            .Font = New Font("Segoe UI Semibold", 17.0F, FontStyle.Bold),
            .ForeColor = CompactUiTheme.TextPrimary,
            .Location = New Point(0, 0),
            .Size = New Size(340, 31),
            .Text = "Settings",
            .TextAlign = ContentAlignment.MiddleLeft
        }
        Dim subtitleLabel As New Label With {
            .AutoSize = False,
            .Font = New Font("Segoe UI", 9.0F),
            .ForeColor = CompactUiTheme.TextSecondary,
            .Location = New Point(2, 32),
            .Size = New Size(560, 20),
            .Text = "Configure the launcher, emulators, and connected services.",
            .TextAlign = ContentAlignment.MiddleLeft
        }
        header.Controls.AddRange(New Control() {titleLabel, subtitleLabel})

        Dim shellBorder As New Panel With {
            .BackColor = CompactUiTheme.Border,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0),
            .Padding = New Padding(1)
        }
        Dim shell As New TableLayoutPanel With {
            .BackColor = CompactUiTheme.AppBackground,
            .ColumnCount = 3,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0),
            .Padding = New Padding(0),
            .RowCount = 1
        }
        shell.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 210.0F))
        shell.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 1.0F))
        shell.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        shell.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        shellBorder.Controls.Add(shell)

        Dim navigationSurface As New TableLayoutPanel With {
            .BackColor = CompactUiTheme.Surface,
            .ColumnCount = 1,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0),
            .Padding = New Padding(12, 14, 12, 12),
            .RowCount = 2
        }
        navigationSurface.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        navigationSurface.RowStyles.Add(New RowStyle(SizeType.Absolute, 30.0F))
        navigationSurface.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        Dim navigationTitle As New Label With {
            .Dock = DockStyle.Fill,
            .Font = New Font("Segoe UI Semibold", 8.0F, FontStyle.Bold),
            .ForeColor = CompactUiTheme.TextSecondary,
            .Text = "CATEGORIES",
            .TextAlign = ContentAlignment.MiddleLeft
        }
        _settingsNavigation = New ListBox With {
            .BackColor = CompactUiTheme.Surface,
            .BorderStyle = BorderStyle.None,
            .Dock = DockStyle.Fill,
            .DrawMode = DrawMode.OwnerDrawFixed,
            .Font = New Font("Segoe UI", 10.0F),
            .ForeColor = CompactUiTheme.TextPrimary,
            .IntegralHeight = False,
            .ItemHeight = 44,
            .Margin = New Padding(0),
            .Name = "SettingsNavigation"
        }
        _settingsNavigation.Items.AddRange(New Object() {
            "General",
            "SDKs",
            "Emulators",
            "Display",
            "Network",
            "Help & About"
        })
        AddHandler _settingsNavigation.DrawItem, AddressOf SettingsNavigation_DrawItem
        AddHandler _settingsNavigation.SelectedIndexChanged, AddressOf SettingsNavigation_SelectedIndexChanged
        navigationSurface.Controls.Add(navigationTitle, 0, 0)
        navigationSurface.Controls.Add(_settingsNavigation, 0, 1)

        Dim separator As New Panel With {
            .BackColor = CompactUiTheme.Border,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0)
        }
        Dim pagesHost As New Panel With {
            .BackColor = CompactUiTheme.AppBackground,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0)
        }

        Dim generalPage = BuildGeneralSettingsPage()
        Dim sdkPage = BuildSdkSettingsPage()
        Dim emulatorPage = BuildEmulatorSettingsPage()
        Dim displayPage = BuildDisplaySettingsPage()
        Dim networkPage = BuildNetworkSettingsPage()
        Dim helpPage = BuildHelpSettingsPage()
        _settingsPages = New List(Of Panel) From {
            generalPage,
            sdkPage,
            emulatorPage,
            displayPage,
            networkPage,
            helpPage
        }
        For Each page In _settingsPages
            page.Visible = False
            pagesHost.Controls.Add(page)
        Next

        shell.Controls.Add(navigationSurface, 0, 0)
        shell.Controls.Add(separator, 1, 0)
        shell.Controls.Add(pagesHost, 2, 0)
        root.Controls.Add(header, 0, 0)
        root.Controls.Add(shellBorder, 0, 1)
        tpConfig.Controls.Add(root)

        _settingsNavigation.SelectedIndex = 0
        tpConfig.ResumeLayout(True)
    End Sub

    Private Sub SettingsNavigation_DrawItem(sender As Object, e As DrawItemEventArgs)
        If e.Index < 0 Then Return

        Dim selected = (e.State And DrawItemState.Selected) = DrawItemState.Selected
        Dim background = If(selected, Color.FromArgb(235, 238, 251), CompactUiTheme.Surface)
        Dim foreground = If(selected, CompactUiTheme.Primary, CompactUiTheme.TextPrimary)
        Using backgroundBrush As New SolidBrush(background)
            e.Graphics.FillRectangle(backgroundBrush, e.Bounds)
        End Using
        If selected Then
            Using accentBrush As New SolidBrush(CompactUiTheme.Primary)
                e.Graphics.FillRectangle(accentBrush, New Rectangle(e.Bounds.Left, e.Bounds.Top, 4, e.Bounds.Height))
            End Using
        End If

        Dim textBounds = New Rectangle(e.Bounds.Left + 16, e.Bounds.Top, e.Bounds.Width - 22, e.Bounds.Height)
        Using navigationFont As New Font("Segoe UI", 9.5F, If(selected, FontStyle.Bold, FontStyle.Regular))
            TextRenderer.DrawText(
                e.Graphics,
                _settingsNavigation.Items(e.Index).ToString(),
                navigationFont,
                textBounds,
                foreground,
                TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis Or TextFormatFlags.NoPrefix)
        End Using
    End Sub

    Private Sub SettingsNavigation_SelectedIndexChanged(sender As Object, e As EventArgs)
        If _settingsPages Is Nothing Then Return
        For index = 0 To _settingsPages.Count - 1
            Dim selected = index = _settingsNavigation.SelectedIndex
            _settingsPages(index).Visible = selected
            If selected Then _settingsPages(index).BringToFront()
        Next
    End Sub

    Private Function CreateSettingsPage(title As String, subtitle As String, ByRef content As FlowLayoutPanel) As Panel
        Dim page As New Panel With {
            .BackColor = CompactUiTheme.AppBackground,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0)
        }
        Dim layout As New TableLayoutPanel With {
            .BackColor = CompactUiTheme.AppBackground,
            .ColumnCount = 1,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0),
            .Padding = New Padding(18, 14, 10, 10),
            .RowCount = 2
        }
        layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 62.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        Dim pageHeader As New Panel With {
            .BackColor = CompactUiTheme.AppBackground,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0)
        }
        Dim pageTitle As New Label With {
            .AutoSize = False,
            .Font = New Font("Segoe UI Semibold", 15.0F, FontStyle.Bold),
            .ForeColor = CompactUiTheme.TextPrimary,
            .Location = New Point(0, 0),
            .Size = New Size(440, 28),
            .Text = title,
            .TextAlign = ContentAlignment.MiddleLeft,
            .UseMnemonic = False
        }
        Dim pageSubtitle As New Label With {
            .AutoSize = False,
            .Font = New Font("Segoe UI", 9.0F),
            .ForeColor = CompactUiTheme.TextSecondary,
            .Location = New Point(1, 30),
            .Size = New Size(760, 22),
            .Text = subtitle,
            .TextAlign = ContentAlignment.MiddleLeft
        }
        pageHeader.Controls.AddRange(New Control() {pageTitle, pageSubtitle})

        content = New FlowLayoutPanel With {
            .AutoScroll = True,
            .BackColor = CompactUiTheme.AppBackground,
            .Dock = DockStyle.Fill,
            .FlowDirection = FlowDirection.LeftToRight,
            .Margin = New Padding(0),
            .Padding = New Padding(0, 0, 0, 12),
            .WrapContents = True
        }
        AddHandler content.Resize, AddressOf SettingsFlow_Resize
        layout.Controls.Add(pageHeader, 0, 0)
        layout.Controls.Add(content, 0, 1)
        page.Controls.Add(layout)
        Return page
    End Function

    Private Function CreateSettingsCard(
        title As String,
        subtitle As String,
        height As Integer,
        widthMode As String,
        ByRef content As Panel
    ) As Panel
        Dim border As New Panel With {
            .BackColor = CompactUiTheme.Border,
            .Margin = New Padding(0, 0, 12, 12),
            .Padding = New Padding(1),
            .Size = New Size(640, height),
            .Tag = widthMode
        }
        Dim surface As New Panel With {
            .BackColor = CompactUiTheme.Surface,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0)
        }
        Dim cardLayout As New TableLayoutPanel With {
            .BackColor = CompactUiTheme.Surface,
            .ColumnCount = 1,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0),
            .Padding = New Padding(16, 11, 16, 14),
            .RowCount = 3
        }
        cardLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        cardLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 27.0F))
        cardLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 23.0F))
        cardLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        Dim cardTitle As New Label With {
            .Dock = DockStyle.Fill,
            .Font = New Font("Segoe UI Semibold", 11.0F, FontStyle.Bold),
            .ForeColor = CompactUiTheme.TextPrimary,
            .Text = title,
            .TextAlign = ContentAlignment.MiddleLeft
        }
        Dim cardSubtitle As New Label With {
            .AutoEllipsis = True,
            .Dock = DockStyle.Fill,
            .Font = New Font("Segoe UI", 8.8F),
            .ForeColor = CompactUiTheme.TextSecondary,
            .Text = subtitle,
            .TextAlign = ContentAlignment.MiddleLeft
        }
        content = New Panel With {
            .BackColor = CompactUiTheme.Surface,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0, 4, 0, 0)
        }
        cardLayout.Controls.Add(cardTitle, 0, 0)
        cardLayout.Controls.Add(cardSubtitle, 0, 1)
        cardLayout.Controls.Add(content, 0, 2)
        surface.Controls.Add(cardLayout)
        border.Controls.Add(surface)
        Return border
    End Function

    Private Sub SettingsFlow_Resize(sender As Object, e As EventArgs)
        ResizeSettingsCards(TryCast(sender, FlowLayoutPanel))
    End Sub

    Private Sub ResizeSettingsCards(flow As FlowLayoutPanel)
        If flow Is Nothing Then Return
        Dim innerWidth = Math.Max(360, flow.ClientSize.Width - flow.Padding.Horizontal - SystemInformation.VerticalScrollBarWidth - 4)
        Dim fullWidth = Math.Max(360, innerWidth - 12)
        Dim halfWidth = If(innerWidth >= 780, Math.Max(360, (innerWidth - 24) \ 2), fullWidth)

        For Each card As Control In flow.Controls
            card.Width = If(String.Equals(TryCast(card.Tag, String), "half", StringComparison.OrdinalIgnoreCase), halfWidth, fullWidth)
        Next
    End Sub

    Private Function CreateSettingsForm(Optional labelPercent As Single = 42.0F) As TableLayoutPanel
        Dim form As New TableLayoutPanel With {
            .BackColor = CompactUiTheme.Surface,
            .ColumnCount = 2,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0),
            .Padding = New Padding(0),
            .RowCount = 0
        }
        form.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, labelPercent))
        form.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F - labelPercent))
        Return form
    End Function

    Private Sub EnsureSettingsRow(table As TableLayoutPanel, row As Integer, Optional height As Single = 34.0F)
        table.RowCount = Math.Max(table.RowCount, row + 1)
        While table.RowStyles.Count <= row
            table.RowStyles.Add(New RowStyle(SizeType.Absolute, height))
        End While
    End Sub

    Private Function CreateSettingsFieldLabel(text As String) As Label
        Return New Label With {
            .Dock = DockStyle.Fill,
            .Font = New Font("Segoe UI", 9.0F),
            .ForeColor = CompactUiTheme.TextPrimary,
            .Margin = New Padding(4, 0, 8, 0),
            .Text = text,
            .TextAlign = ContentAlignment.MiddleLeft
        }
    End Function

    Private Sub AddSettingsField(table As TableLayoutPanel, row As Integer, labelText As String, control As Control)
        EnsureSettingsRow(table, row)
        PrepareSettingsInput(control)
        table.Controls.Add(CreateSettingsFieldLabel(labelText), 0, row)
        table.Controls.Add(control, 1, row)
    End Sub

    Private Sub AddSettingsCheck(table As TableLayoutPanel, row As Integer, checkBox As CheckBox)
        EnsureSettingsRow(table, row, 32.0F)
        PrepareSettingsInput(checkBox)
        table.Controls.Add(checkBox, 0, row)
        table.SetColumnSpan(checkBox, table.ColumnCount)
    End Sub

    Private Sub PrepareSettingsInput(control As Control)
        control.Font = New Font("Segoe UI", 9.0F)
        control.ForeColor = CompactUiTheme.TextPrimary
        If TypeOf control Is ComboBox Then
            Dim combo = DirectCast(control, ComboBox)
            combo.BackColor = Color.FromArgb(246, 247, 250)
            combo.Dock = DockStyle.Fill
            combo.FlatStyle = FlatStyle.Flat
            combo.Margin = New Padding(4, 5, 4, 5)
        ElseIf TypeOf control Is TextBox Then
            Dim textBox = DirectCast(control, TextBox)
            textBox.BackColor = Color.FromArgb(249, 250, 252)
            textBox.BorderStyle = BorderStyle.FixedSingle
            textBox.Dock = DockStyle.Fill
            textBox.Margin = New Padding(4, 5, 4, 5)
        ElseIf TypeOf control Is CheckBox Then
            Dim checkBox = DirectCast(control, CheckBox)
            checkBox.AutoSize = False
            checkBox.BackColor = CompactUiTheme.Surface
            checkBox.Dock = DockStyle.Fill
            checkBox.FlatStyle = FlatStyle.Flat
            checkBox.Margin = New Padding(4, 2, 4, 2)
            checkBox.UseVisualStyleBackColor = False
        End If
    End Sub

    Private Sub StyleSettingsButton(button As Button, Optional primary As Boolean = False)
        button.AutoSize = False
        button.FlatStyle = FlatStyle.Flat
        button.FlatAppearance.BorderSize = 1
        button.Font = New Font("Segoe UI", 9.0F, If(primary, FontStyle.Bold, FontStyle.Regular))
        button.Height = 36
        button.Margin = New Padding(4, 4, 8, 4)
        button.Padding = New Padding(10, 0, 10, 0)
        button.UseVisualStyleBackColor = False
        If primary Then
            CompactUiTheme.StylePrimaryButton(button)
        Else
            CompactUiTheme.StyleSecondaryButton(button)
        End If
    End Sub

    Private Function BuildGeneralSettingsPage() As Panel
        Dim flow As FlowLayoutPanel = Nothing
        Dim page = CreateSettingsPage(
            "General",
            "Common launcher tools and library management shortcuts.",
            flow)

        Dim cardContent As Panel = Nothing
        Dim actionsCard = CreateSettingsCard(
            "Launcher tools",
            "Open frequently used management tools without leaving Settings.",
            154,
            "full",
            cardContent)
        Dim actions As New FlowLayoutPanel With {
            .BackColor = CompactUiTheme.Surface,
            .Dock = DockStyle.Fill,
            .FlowDirection = FlowDirection.LeftToRight,
            .Margin = New Padding(0),
            .Padding = New Padding(0, 5, 0, 0),
            .WrapContents = True
        }
        btnSaveDataManagement.Text = "Manage Save Data"
        btnLaunchKey2Pad.Text = "Controller Mapping"
        btnAddCustomApps.Text = "Add Custom Apps"
        For Each button In New Button() {btnSaveDataManagement, btnLaunchKey2Pad, btnAddCustomApps}
            StyleSettingsButton(button)
            button.Width = 210
            actions.Controls.Add(button)
        Next
        cardContent.Controls.Add(actions)
        flow.Controls.Add(actionsCard)
        ResizeSettingsCards(flow)
        Return page
    End Function

    Private Function BuildSdkSettingsPage() As Panel
        Dim flow As FlowLayoutPanel = Nothing
        Dim page = CreateSettingsPage(
            "SDKs",
            "Choose the default runtime used for each supported mobile platform.",
            flow)

        Dim cardContent As Panel = Nothing
        Dim sdkCard = CreateSettingsCard(
            "Default SDKs",
            "These defaults are used when launching apps from the Library.",
            240,
            "full",
            cardContent)
        Dim sdkGrid As New TableLayoutPanel With {
            .BackColor = CompactUiTheme.Surface,
            .ColumnCount = 4,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0),
            .Padding = New Padding(0),
            .RowCount = 4
        }
        sdkGrid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 19.0F))
        sdkGrid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 31.0F))
        sdkGrid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 19.0F))
        sdkGrid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 31.0F))
        For row = 0 To 3
            sdkGrid.RowStyles.Add(New RowStyle(SizeType.Percent, 25.0F))
        Next
        AddSdkSettingsField(sdkGrid, 0, 0, "DoJa", cbxDojaSDK)
        AddSdkSettingsField(sdkGrid, 0, 2, "Star", cbxStarSDK)
        AddSdkSettingsField(sdkGrid, 1, 0, "SoftBank", cbxSoftbankSDK)
        AddSdkSettingsField(sdkGrid, 1, 2, "J-Sky", cbxJSKYSDK)
        AddSdkSettingsField(sdkGrid, 2, 0, "Vodafone", cbxVodafoneSDK)
        AddSdkSettingsField(sdkGrid, 2, 2, "AirEdge", cbxAirEdgeSDK)
        AddSdkSettingsField(sdkGrid, 3, 0, "EZWeb EZPlus", cbxEZWebEZPlusSDK)
        AddSdkSettingsField(sdkGrid, 3, 2, "Flash", cbxFlashSDK)
        cardContent.Controls.Add(sdkGrid)
        flow.Controls.Add(sdkCard)
        ResizeSettingsCards(flow)
        Return page
    End Function

    Private Sub AddSdkSettingsField(table As TableLayoutPanel, row As Integer, column As Integer, labelText As String, combo As ComboBox)
        PrepareSettingsInput(combo)
        table.Controls.Add(CreateSettingsFieldLabel(labelText), column, row)
        table.Controls.Add(combo, column + 1, row)
    End Sub

    Private Function BuildEmulatorSettingsPage() As Panel
        Dim flow As FlowLayoutPanel = Nothing
        Dim page = CreateSettingsPage(
            "Emulators",
            "Fine-tune runtime behavior for the emulators used by the launcher.",
            flow)

        Dim runtimeContent As Panel = Nothing
        Dim runtimeCard = CreateSettingsCard(
            "DoJa / Star runtime",
            "Audio, interface, and rendering behavior.",
            282,
            "half",
            runtimeContent)
        Dim runtimeForm = CreateSettingsForm(43.0F)
        Dim audioPanel As New TableLayoutPanel With {
            .BackColor = CompactUiTheme.Surface,
            .ColumnCount = 2,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0),
            .RowCount = 1
        }
        audioPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 112.0F))
        audioPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        audioPanel.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        PrepareSettingsInput(cbxAudioType)
        cbxAudioType.Margin = New Padding(4, 5, 6, 5)
        lblAudioWarning.AutoSize = False
        lblAudioWarning.Dock = DockStyle.Fill
        lblAudioWarning.Font = New Font("Segoe UI", 7.8F)
        lblAudioWarning.ForeColor = CompactUiTheme.Danger
        lblAudioWarning.Margin = New Padding(0)
        lblAudioWarning.TextAlign = ContentAlignment.MiddleLeft
        audioPanel.Controls.Add(cbxAudioType, 0, 0)
        audioPanel.Controls.Add(lblAudioWarning, 1, 0)
        AddSettingsField(runtimeForm, 0, "Audio type", audioPanel)
        AddSettingsCheck(runtimeForm, 1, chkbxHidePhoneUI)
        AddSettingsCheck(runtimeForm, 2, chkbxModifyJamFiles)
        AddSettingsCheck(runtimeForm, 3, chkbxEnableHighPerformanceEmulator)
        AddSettingsCheck(runtimeForm, 4, chkboxEnforceHardwareRendering)
        AddSettingsField(runtimeForm, 5, "Rendering resolution", cbxInternalRenderingResolution)
        runtimeContent.Controls.Add(runtimeForm)

        Dim openDojaContent As Panel = Nothing
        Dim openDojaCard = CreateSettingsCard(
            "OpenDoJa",
            "Configure scaling, audio, fonts, and launch behavior.",
            250,
            "half",
            openDojaContent)
        Dim openDojaForm = CreateSettingsForm(43.0F)
        AddSettingsField(openDojaForm, 0, "Host scale", cbxOpenDojaHostScale)
        AddSettingsField(openDojaForm, 1, "Audio type", cbxOpenDojaAudioType)
        AddSettingsField(openDojaForm, 2, "Font type", cbxOpenDojaFontType)
        AddSettingsField(openDojaForm, 3, "Launch type", cbxOpenDojaLaunchType)
        AddSettingsCheck(openDojaForm, 4, chkbxOpenDojaLaunchGUI)
        openDojaContent.Controls.Add(openDojaForm)

        Dim sjmeContent As Panel = Nothing
        Dim sjmeCard = CreateSettingsCard(
            "SquirrelJME",
            "Options become available when SquirrelJME is the selected DoJa SDK.",
            190,
            "half",
            sjmeContent)
        Dim sjmeForm = CreateSettingsForm(43.0F)
        AddSettingsField(sjmeForm, 0, "Launch option", cbxSJMELaunchOption)
        AddSettingsField(sjmeForm, 1, "Scaling", cbxSJMEScaling)
        EnsureSettingsRow(sjmeForm, 2, 40.0F)
        StyleSettingsButton(btnSJMEUpdate)
        btnSJMEUpdate.Dock = DockStyle.Fill
        btnSJMEUpdate.Margin = New Padding(4, 3, 4, 3)
        sjmeForm.Controls.Add(btnSJMEUpdate, 0, 2)
        sjmeForm.SetColumnSpan(btnSJMEUpdate, 2)
        sjmeContent.Controls.Add(sjmeForm)

        Dim remexaContent As Panel = Nothing
        Dim remexaCard = CreateSettingsCard(
            "ReMEXA",
            "Open the ReMEXA interface for advanced options not yet exposed here.",
            190,
            "half",
            remexaContent)
        Dim remexaActions As New FlowLayoutPanel With {
            .BackColor = CompactUiTheme.Surface,
            .Dock = DockStyle.Fill,
            .FlowDirection = FlowDirection.LeftToRight,
            .Padding = New Padding(0, 6, 0, 0),
            .WrapContents = False
        }
        btnReMEXALaunchGUI.Text = "Open ReMEXA"
        StyleSettingsButton(btnReMEXALaunchGUI, True)
        btnReMEXALaunchGUI.Width = 180
        remexaActions.Controls.Add(btnReMEXALaunchGUI)
        remexaContent.Controls.Add(remexaActions)

        flow.Controls.Add(runtimeCard)
        flow.Controls.Add(openDojaCard)
        flow.Controls.Add(sjmeCard)
        flow.Controls.Add(remexaCard)
        ResizeSettingsCards(flow)
        Return page
    End Function

    Private Function BuildDisplaySettingsPage() As Panel
        Dim flow As FlowLayoutPanel = Nothing
        Dim page = CreateSettingsPage(
            "Display",
            "Manage visual enhancement and shader preferences.",
            flow)

        Dim shaderContent As Panel = Nothing
        Dim shaderCard = CreateSettingsCard(
            "ShaderGlass",
            "Select the shader applied when ShaderGlass is enabled in Play Options.",
            160,
            "full",
            shaderContent)
        Dim shaderForm = CreateSettingsForm(24.0F)
        AddSettingsField(shaderForm, 0, "Default shader", cbxShaderGlass_Shader)
        shaderContent.Controls.Add(shaderForm)
        flow.Controls.Add(shaderCard)
        ResizeSettingsCards(flow)
        Return page
    End Function

    Private Function BuildNetworkSettingsPage() As Panel
        Dim flow As FlowLayoutPanel = Nothing
        Dim page = CreateSettingsPage(
            "Network",
            "Manage the identifiers and URL handling used by supported online apps.",
            flow)

        Dim networkContent As Panel = Nothing
        Dim networkCard = CreateSettingsCard(
            "Network identity",
            "A Network UID and Terminal ID are required for supported online features.",
            280,
            "full",
            networkContent)
        Dim networkForm As New TableLayoutPanel With {
            .BackColor = CompactUiTheme.Surface,
            .ColumnCount = 3,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0),
            .Padding = New Padding(0),
            .RowCount = 4
        }
        networkForm.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 130.0F))
        networkForm.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 310.0F))
        networkForm.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        networkForm.RowStyles.Add(New RowStyle(SizeType.Absolute, 38.0F))
        networkForm.RowStyles.Add(New RowStyle(SizeType.Absolute, 38.0F))
        networkForm.RowStyles.Add(New RowStyle(SizeType.Absolute, 34.0F))
        networkForm.RowStyles.Add(New RowStyle(SizeType.Absolute, 44.0F))

        PrepareSettingsInput(txtCurrentUID)
        PrepareSettingsInput(txtCurrentTID)
        txtCurrentUID.AccessibleName = "Current Network UID"
        txtCurrentTID.AccessibleName = "Current Terminal ID"
        For Each warning In New Label() {lblInvalidUID, lblInvalidTID}
            warning.AutoSize = False
            warning.Dock = DockStyle.Fill
            warning.Font = New Font("Segoe UI", 8.5F, FontStyle.Bold)
            warning.ForeColor = CompactUiTheme.Danger
            warning.Margin = New Padding(10, 0, 4, 0)
            warning.TextAlign = ContentAlignment.MiddleLeft
        Next
        networkForm.Controls.Add(CreateSettingsFieldLabel("Current UID"), 0, 0)
        networkForm.Controls.Add(txtCurrentUID, 1, 0)
        networkForm.Controls.Add(lblInvalidUID, 2, 0)
        networkForm.Controls.Add(CreateSettingsFieldLabel("Current TID"), 0, 1)
        networkForm.Controls.Add(txtCurrentTID, 1, 1)
        networkForm.Controls.Add(lblInvalidTID, 2, 1)
        PrepareSettingsInput(chkboxNetworkModifyURLS)
        networkForm.Controls.Add(chkboxNetworkModifyURLS, 0, 2)
        networkForm.SetColumnSpan(chkboxNetworkModifyURLS, 3)
        btnUpdateNetworkUID.Text = "Update Network Identity"
        StyleSettingsButton(btnUpdateNetworkUID, True)
        btnUpdateNetworkUID.Width = 220
        btnUpdateNetworkUID.Margin = New Padding(4, 4, 4, 4)
        networkForm.Controls.Add(btnUpdateNetworkUID, 1, 3)
        networkContent.Controls.Add(networkForm)
        flow.Controls.Add(networkCard)
        ResizeSettingsCards(flow)
        Return page
    End Function

    Private Function BuildHelpSettingsPage() As Panel
        Dim flow As FlowLayoutPanel = Nothing
        Dim page = CreateSettingsPage(
            "Help & About",
            "Version information, guides, and first-line troubleshooting.",
            flow)

        Dim aboutContent As Panel = Nothing
        Dim aboutCard = CreateSettingsCard(
            "About Keitai World Launcher",
            "Project information and useful links.",
            280,
            "half",
            aboutContent)
        Dim aboutLayout As New TableLayoutPanel With {
            .BackColor = CompactUiTheme.Surface,
            .ColumnCount = 1,
            .Dock = DockStyle.Fill,
            .RowCount = 2
        }
        aboutLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        aboutLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        aboutLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 48.0F))
        lblHelp_AppVer.Dock = DockStyle.Fill
        lblHelp_AppVer.Font = New Font("Segoe UI", 10.0F)
        lblHelp_AppVer.ForeColor = CompactUiTheme.TextPrimary
        lblHelp_AppVer.TextAlign = ContentAlignment.MiddleCenter
        Dim helpActions As New FlowLayoutPanel With {
            .BackColor = CompactUiTheme.Surface,
            .Dock = DockStyle.Fill,
            .FlowDirection = FlowDirection.LeftToRight,
            .Padding = New Padding(0, 2, 0, 0),
            .WrapContents = False
        }
        btnVisitKeitaiArchive.Text = "Visit Keitai Archive"
        btnControls.Text = "Keyboard && Controller Guide"
        StyleSettingsButton(btnVisitKeitaiArchive)
        StyleSettingsButton(btnControls)
        btnVisitKeitaiArchive.Width = 150
        btnControls.Width = 220
        helpActions.Controls.Add(btnVisitKeitaiArchive)
        helpActions.Controls.Add(btnControls)
        aboutLayout.Controls.Add(lblHelp_AppVer, 0, 0)
        aboutLayout.Controls.Add(helpActions, 0, 1)
        aboutContent.Controls.Add(aboutLayout)

        Dim troubleshootingContent As Panel = Nothing
        Dim troubleshootingCard = CreateSettingsCard(
            "Troubleshooting",
            "Try these checks before requesting support.",
            410,
            "half",
            troubleshootingContent)
        lblHelp_troubleshooting.Dock = DockStyle.Fill
        lblHelp_troubleshooting.Font = New Font("Segoe UI", 9.0F)
        lblHelp_troubleshooting.ForeColor = CompactUiTheme.TextPrimary
        lblHelp_troubleshooting.Padding = New Padding(4, 4, 4, 4)
        lblHelp_troubleshooting.TextAlign = ContentAlignment.TopLeft
        troubleshootingContent.Controls.Add(lblHelp_troubleshooting)

        flow.Controls.Add(aboutCard)
        flow.Controls.Add(troubleshootingCard)
        ResizeSettingsCards(flow)
        Return page
    End Function

    Private Sub SetSjmeSettingsEnabled(enabled As Boolean)
        cbxSJMELaunchOption.Enabled = enabled
        cbxSJMEScaling.Enabled = enabled
        btnSJMEUpdate.Enabled = enabled
    End Sub

    Private Sub BuildUnifiedLibraryTabs()
        Dim appsPage As New TabPage("Apps") With {.BackColor = CompactUiTheme.AppBackground, .Padding = New Padding(4)}
        Dim machiPage As New TabPage("Machi-Chara") With {.BackColor = CompactUiTheme.AppBackground, .Padding = New Padding(4)}
        Dim charaPage As New TabPage("Chara-Den") With {.BackColor = CompactUiTheme.AppBackground, .Padding = New Padding(4)}

        _libraryCategoryTabs = New TabControl With {
            .Dock = DockStyle.Fill,
            .Font = New Font("Segoe UI", 9.5F),
            .ItemSize = New Size(128, 28),
            .Padding = New Point(14, 4),
            .SizeMode = TabSizeMode.Fixed
        }
        _libraryCategoryTabs.TabPages.AddRange(New TabPage() {appsPage, machiPage, charaPage})

        _libraryRoot = New TableLayoutPanel With {
            .BackColor = CompactUiTheme.AppBackground,
            .ColumnCount = 1,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0),
            .Padding = New Padding(0),
            .RowCount = 2
        }
        _libraryRoot.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        _libraryRoot.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        _libraryRoot.RowStyles.Add(New RowStyle(SizeType.Absolute, 0.0F))
        _libraryRoot.Controls.Add(_libraryCategoryTabs, 0, 0)

        tpAppli.Controls.Clear()
        tpAppli.Controls.Add(_libraryRoot)

        ' Keep the original controls and event wiring, but present the three
        ' content types as compact sub-tabs inside one top-level Library.
        MaterialTabControl1.TabPages.Remove(tpMachiChara)
        MaterialTabControl1.TabPages.Remove(tpCharaDen)

        _appLibraryGrid = CreateLibraryGrid()
        _machiLibraryGrid = CreateLibraryGrid()
        _charaLibraryGrid = CreateLibraryGrid()
        appsPage.Controls.Add(_appLibraryGrid)
        machiPage.Controls.Add(_machiLibraryGrid)
        charaPage.Controls.Add(_charaLibraryGrid)

        AddHandler appsPage.Resize, AddressOf LibraryPage_Resize
        AddHandler machiPage.Resize, AddressOf LibraryPage_Resize
        AddHandler charaPage.Resize, AddressOf LibraryPage_Resize
    End Sub

    Private Function CreateLibraryGrid() As TableLayoutPanel
        Dim grid As New TableLayoutPanel With {
            .BackColor = CompactUiTheme.AppBackground,
            .ColumnCount = 2,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0),
            .Padding = New Padding(0),
            .RowCount = 1
        }
        grid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 39.0F))
        grid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 61.0F))
        grid.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        Return grid
    End Function

    Private Sub BuildCompactAppLayout()
        GroupBox1.Text = "Apps"
        GroupBox1.Dock = DockStyle.Fill
        GroupBox1.Margin = New Padding(0)
        gbxGameInfo.Text = "App details"
        gbxGameInfo.Dock = DockStyle.Fill
        gbxGameInfo.Margin = New Padding(0, 0, 0, 4)
        GroupBox3.Text = "Play options"
        GroupBox3.Dock = DockStyle.Bottom
        GroupBox3.Height = 174
        GroupBox3.Margin = New Padding(0)

        Dim rightPanel As New Panel With {.Dock = DockStyle.Fill, .Margin = New Padding(6, 0, 0, 0)}
        _appLibraryGrid.Controls.Add(GroupBox1, 0, 0)
        rightPanel.Controls.Add(gbxGameInfo)
        rightPanel.Controls.Add(GroupBox3)
        GroupBox3.BringToFront()
        _appLibraryGrid.Controls.Add(rightPanel, 1, 0)

        BuildGameActionBar()
        BuildDownloadQueueBar()
        RepositionCompactLaunchOptions()

        AddHandler GroupBox1.Resize, AddressOf GroupBox1_Resize
        AddHandler GroupBox3.Resize, AddressOf PlayOptions_Resize
        AddHandler ListViewGamesVariants.VisibleChanged, AddressOf Variants_VisibleChanged
        txtLVSearch.PlaceholderText = "Search English or Japanese titles"
        cbxFilterType.AccessibleName = "Library filter"
        ListViewGames.MultiSelect = False
        ListViewGames.HideSelection = False
    End Sub

    Private Sub BuildDownloadQueueBar()
        If _libraryRoot Is Nothing OrElse _downloadQueueBorder IsNot Nothing Then Return

        _downloadQueueBorder = New Panel With {
            .BackColor = CompactUiTheme.Border,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(4, 6, 4, 4),
            .Padding = New Padding(1)
        }
        Dim surface As New Panel With {
            .BackColor = CompactUiTheme.Surface,
            .Dock = DockStyle.Fill,
            .Padding = New Padding(14, 8, 14, 7)
        }
        Dim accent As New Panel With {
            .BackColor = CompactUiTheme.Primary,
            .Dock = DockStyle.Left,
            .Width = 4
        }
        Dim statusGlyph As New Label With {
            .BackColor = Color.FromArgb(235, 239, 252),
            .Font = New Font("Segoe UI Semibold", 12.0F, FontStyle.Bold),
            .ForeColor = CompactUiTheme.Primary,
            .Location = New Point(16, 10),
            .Size = New Size(36, 36),
            .Text = ChrW(&H2193),
            .TextAlign = ContentAlignment.MiddleCenter
        }
        _downloadQueueTitle = New Label With {
            .AutoEllipsis = True,
            .Font = New Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
            .ForeColor = CompactUiTheme.TextPrimary,
            .Location = New Point(66, 7),
            .Size = New Size(520, 22),
            .Text = "Preparing download",
            .TextAlign = ContentAlignment.MiddleLeft
        }
        _downloadQueueStatus = New Label With {
            .AutoEllipsis = True,
            .Font = New Font("Segoe UI", 8.7F),
            .ForeColor = CompactUiTheme.TextSecondary,
            .Location = New Point(66, 28),
            .Size = New Size(620, 20),
            .Text = "Starting...",
            .TextAlign = ContentAlignment.MiddleLeft
        }
        _downloadQueueCount = New Label With {
            .Anchor = AnchorStyles.Top Or AnchorStyles.Right,
            .Font = New Font("Segoe UI Semibold", 8.7F, FontStyle.Bold),
            .ForeColor = CompactUiTheme.TextSecondary,
            .Size = New Size(130, 34),
            .Text = "Current download",
            .TextAlign = ContentAlignment.MiddleRight
        }

        If pbGameDL.Parent IsNot Nothing Then pbGameDL.Parent.Controls.Remove(pbGameDL)
        pbGameDL.Anchor = AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Bottom
        pbGameDL.Height = 5
        pbGameDL.Minimum = 0
        pbGameDL.Maximum = 100
        pbGameDL.MarqueeAnimationSpeed = 24
        pbGameDL.Style = ProgressBarStyle.Marquee
        pbGameDL.Visible = True

        surface.Controls.AddRange(New Control() {pbGameDL, _downloadQueueCount, _downloadQueueStatus, _downloadQueueTitle, statusGlyph, accent})
        AddHandler surface.Resize,
            Sub()
                _downloadQueueCount.Location = New Point(Math.Max(690, surface.ClientSize.Width - _downloadQueueCount.Width - 12), 11)
                Dim textRight = Math.Max(160, _downloadQueueCount.Left - 76)
                _downloadQueueTitle.Width = Math.Max(100, textRight)
                _downloadQueueStatus.Width = Math.Max(100, textRight)
                pbGameDL.SetBounds(66, surface.ClientSize.Height - 10, Math.Max(100, surface.ClientSize.Width - 82), 5)
            End Sub

        _downloadQueueBorder.Controls.Add(surface)
        _libraryRoot.Controls.Add(_downloadQueueBorder, 0, 1)
        _downloadQueueBorder.Visible = False
    End Sub

    Private Sub UpdateDownloadQueuePanel(title As String, status As String, percentage As Integer, queuedCount As Integer)
        If _downloadQueueBorder Is Nothing OrElse _libraryRoot Is Nothing Then Return

        _downloadQueueTitle.Text = title
        _downloadQueueStatus.Text = If(String.IsNullOrWhiteSpace(status), "Working...", status)
        _downloadQueueCount.Text = If(queuedCount > 0, $"{queuedCount} queued", "Current download")
        If percentage >= 0 Then
            pbGameDL.Style = ProgressBarStyle.Continuous
            pbGameDL.Value = Math.Max(pbGameDL.Minimum, Math.Min(pbGameDL.Maximum, percentage))
        Else
            pbGameDL.Style = ProgressBarStyle.Marquee
            pbGameDL.MarqueeAnimationSpeed = 24
        End If

        _libraryRoot.RowStyles(1).Height = 74.0F
        _downloadQueueBorder.Visible = True
        _downloadQueueBorder.BringToFront()
    End Sub

    Private Sub HideDownloadQueuePanel()
        If _downloadQueueBorder Is Nothing OrElse _libraryRoot Is Nothing Then Return
        _downloadQueueBorder.Visible = False
        _libraryRoot.RowStyles(1).Height = 0.0F
        pbGameDL.Style = ProgressBarStyle.Continuous
        pbGameDL.Value = 0
    End Sub

    Private Sub BuildGameActionBar()
        btnLaunchGame.Text = "Play / Download"
        btnLaunchGame.Size = New Size(200, 46)
        btnLaunchGame.Margin = New Padding(3)
        btnLaunchGame.FlatStyle = FlatStyle.Flat
        btnLaunchGame.UseVisualStyleBackColor = False
        CompactUiTheme.StylePrimaryButton(btnLaunchGame)

        _btnGameActions = CompactUiTheme.CreateCompactButton("Actions  ▾")
        _btnGameActions.Name = "btnGameActions"
        _btnGameActions.Size = New Size(200, 32)

        _actionRedownload = New ToolStripMenuItem("Redownload")
        _actionBackupSave = New ToolStripMenuItem("Back up save")
        _actionFavorite = New ToolStripMenuItem("Favorite")
        _actionOpenFolder = New ToolStripMenuItem("Open folder")
        _actionDelete = New ToolStripMenuItem("Delete")
        _actionDelete.ForeColor = CompactUiTheme.Danger

        _gameActionsMenu = New ContextMenuStrip()
        _gameActionsMenu.Items.AddRange(New ToolStripItem() {
            _actionRedownload,
            _actionBackupSave,
            New ToolStripSeparator(),
            _actionFavorite,
            _actionOpenFolder,
            New ToolStripSeparator(),
            _actionDelete
        })

        _gameActionBar = New FlowLayoutPanel With {
            .AutoScroll = False,
            .BackColor = Color.Transparent,
            .Dock = DockStyle.Left,
            .FlowDirection = FlowDirection.TopDown,
            .Padding = New Padding(8, 8, 8, 6),
            .Width = 224,
            .WrapContents = False
        }
        _gameActionBar.Controls.AddRange(New Control() {
            btnLaunchGame,
            _btnGameActions
        })
        GroupBox3.Controls.Add(_gameActionBar)
        _gameActionBar.BringToFront()

        AddHandler _btnGameActions.Click, AddressOf GameActions_Click
        AddHandler _actionRedownload.Click, AddressOf DownloadGame_Click
        AddHandler _actionBackupSave.Click, AddressOf BackupSaveToolStripMenuItem_Click
        AddHandler _actionFavorite.Click, AddressOf FavoriteGame_Click
        AddHandler _actionOpenFolder.Click, AddressOf OpenGameFolder_Click
        AddHandler _actionDelete.Click, AddressOf DeleteGame_Click
        ' ItemActivate is the ListView's semantic double-click/default action.
        ' It also keeps the behavior available to keyboard and accessibility users.
        AddHandler ListViewGames.ItemActivate, AddressOf ListViewGames_Activate
    End Sub

    Private Sub RepositionCompactLaunchOptions()
        Const optionsLeft As Integer = 232
        Const outerRightMargin As Integer = 10
        Const columnGap As Integer = 8

        chkbxLocalEmulator.Location = New Point(optionsLeft, 29)
        chkbxShaderGlass.Location = New Point(optionsLeft + 120, 29)
        Label2.Location = New Point(optionsLeft + 216, 25)
        Label2.Size = New Size(54, 23)
        Label2.Text = "Scaling"
        cbxShaderGlassScaling.Location = New Point(optionsLeft + 270, 26)
        cbxShaderGlassScaling.Width = Math.Max(64, Math.Min(85, GroupBox3.ClientSize.Width - cbxShaderGlassScaling.Left - outerRightMargin))

        chkbxDialpadNumpad.Location = New Point(optionsLeft, 61)
        chkbxDialpadRotated.Location = New Point(optionsLeft + 120, 61)
        chkbxEnableController.Location = New Point(optionsLeft + 194, 61)
        chkboxControllerVibration.Location = New Point(optionsLeft + 322, 61)

        Dim availableWidth = Math.Max(250, GroupBox3.ClientSize.Width - optionsLeft - outerRightMargin)
        Dim comboWidth = Math.Max(120, CInt(Math.Floor((availableWidth - columnGap) / 2.0)))
        cbxGameControllers.Location = New Point(optionsLeft, 91)
        cbxGameControllers.Width = comboWidth
        cbxControllerProfile.Location = New Point(optionsLeft + comboWidth + columnGap, 91)
        cbxControllerProfile.Width = Math.Max(120, GroupBox3.ClientSize.Width - cbxControllerProfile.Left - outerRightMargin)
    End Sub

    Private Sub PlayOptions_Resize(sender As Object, e As EventArgs)
        RepositionCompactLaunchOptions()
    End Sub

    Private Sub BuildCompactMachiCharaLayout()
        GroupBox2.Text = "Machi-Chara"
        GroupBox2.Dock = DockStyle.Fill
        _machiLibraryGrid.Controls.Add(GroupBox2, 0, 0)
        BuildCharacterListHost(GroupBox2, ListViewMachiChara, lblMachiCharaTotalCount, _txtMachiSearch, "Search Machi-Chara")

        Dim detailPanel = BuildCharacterDetailPanel(
            _lblMachiTitle,
            _lblMachiMetadata,
            _lblMachiStatus,
            btnMachiCharaLaunch,
            _btnMachiActions,
            chkboxMachiCharaLocalEmulator,
            "Machi-Chara details")
        detailPanel.Margin = New Padding(6, 0, 0, 0)
        _machiLibraryGrid.Controls.Add(detailPanel, 1, 0)
        BuildMachiActionsMenu()

        AddHandler _txtMachiSearch.TextChanged, AddressOf MachiSearch_TextChanged
        AddHandler ListViewMachiChara.ItemActivate, AddressOf MachiList_Activate
    End Sub

    Private Sub BuildCompactCharaDenLayout()
        GroupBox10.Text = "Chara-Den"
        GroupBox10.Dock = DockStyle.Fill
        _charaLibraryGrid.Controls.Add(GroupBox10, 0, 0)
        BuildCharacterListHost(GroupBox10, ListViewCharaDen, lblCharadenTotalCount, _txtCharaSearch, "Search Chara-Den")

        Dim detailPanel = BuildCharacterDetailPanel(
            _lblCharaTitle,
            _lblCharaMetadata,
            _lblCharaStatus,
            btnCharaDenLaunch,
            _btnCharaActions,
            chkboxCharadenLocalEmulator,
            "Chara-Den details")
        detailPanel.Margin = New Padding(6, 0, 0, 0)
        _charaLibraryGrid.Controls.Add(detailPanel, 1, 0)
        BuildCharaActionsMenu()

        AddHandler _txtCharaSearch.TextChanged, AddressOf CharaSearch_TextChanged
        AddHandler ListViewCharaDen.ItemActivate, AddressOf CharaList_Activate
    End Sub

    Private Sub BuildCharacterListHost(
        host As GroupBox,
        list As ListView,
        countLabel As Label,
        ByRef searchBox As TextBox,
        placeholder As String)

        searchBox = New TextBox With {
            .Dock = DockStyle.Fill,
            .Font = New Font("Segoe UI", 9.0F),
            .Margin = New Padding(0),
            .PlaceholderText = placeholder
        }

        list.MultiSelect = False
        list.HideSelection = False
        Dim searchHost As New Panel With {.Dock = DockStyle.Fill, .Padding = New Padding(4, 3, 4, 3)}
        searchHost.Controls.Add(searchBox)

        Dim layout As New TableLayoutPanel With {
            .ColumnCount = 1,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0),
            .Padding = New Padding(0),
            .RowCount = 3
        }
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 32))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 28))

        list.Dock = DockStyle.Fill
        countLabel.Dock = DockStyle.Fill
        countLabel.Height = 28
        host.Controls.Clear()
        layout.Controls.Add(searchHost, 0, 0)
        layout.Controls.Add(list, 0, 1)
        layout.Controls.Add(countLabel, 0, 2)
        host.Controls.Add(layout)
    End Sub

    Private Function BuildCharacterDetailPanel(
        ByRef titleLabel As Label,
        ByRef metadataLabel As Label,
        ByRef statusLabel As Label,
        launchButton As Button,
        ByRef actionsButton As Button,
        localeCheckbox As CheckBox,
        heading As String) As Control

        titleLabel = New Label With {
            .AutoEllipsis = True,
            .Dock = DockStyle.Fill,
            .Font = New Font("Segoe UI", 16.0F, FontStyle.Bold),
            .ForeColor = CompactUiTheme.TextPrimary,
            .Text = heading,
            .TextAlign = ContentAlignment.BottomLeft
        }
        metadataLabel = New Label With {
            .AutoEllipsis = True,
            .Dock = DockStyle.Fill,
            .Font = New Font("Segoe UI", 9.5F),
            .ForeColor = CompactUiTheme.TextSecondary,
            .TextAlign = ContentAlignment.TopLeft
        }
        statusLabel = New Label With {.Anchor = AnchorStyles.Left, .Size = New Size(112, 28)}
        CompactUiTheme.SetStatusBadge(statusLabel, "No selection", False)

        launchButton.Text = "Play / Download"
        launchButton.Size = New Size(220, 46)
        launchButton.Margin = New Padding(0, 0, 0, 4)
        launchButton.FlatStyle = FlatStyle.Flat
        launchButton.UseVisualStyleBackColor = False
        CompactUiTheme.StylePrimaryButton(launchButton)

        actionsButton = CompactUiTheme.CreateCompactButton("Actions  ▾")
        actionsButton.Size = New Size(220, 32)
        actionsButton.Margin = New Padding(0)

        Dim actions As New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .FlowDirection = FlowDirection.TopDown,
            .Padding = New Padding(0),
            .WrapContents = False
        }
        actions.Controls.AddRange(New Control() {launchButton, actionsButton})

        localeCheckbox.AutoSize = True
        localeCheckbox.Anchor = AnchorStyles.Left

        Dim layout As New TableLayoutPanel With {
            .BackColor = CompactUiTheme.Surface,
            .ColumnCount = 1,
            .Dock = DockStyle.Fill,
            .Padding = New Padding(24),
            .RowCount = 6
        }
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 48))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 60))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 34))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 86))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 34))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        layout.Controls.Add(titleLabel, 0, 0)
        layout.Controls.Add(metadataLabel, 0, 1)
        layout.Controls.Add(statusLabel, 0, 2)
        layout.Controls.Add(actions, 0, 3)
        layout.Controls.Add(localeCheckbox, 0, 4)
        Return layout
    End Function

    Private Sub BuildMachiActionsMenu()
        _actionMachiRedownload = New ToolStripMenuItem("Redownload")
        _actionMachiDelete = New ToolStripMenuItem("Delete") With {.ForeColor = CompactUiTheme.Danger}
        _machiActionsMenu = New ContextMenuStrip()
        _machiActionsMenu.Items.AddRange(New ToolStripItem() {
            _actionMachiRedownload,
            New ToolStripSeparator(),
            _actionMachiDelete
        })

        AddHandler _btnMachiActions.Click, AddressOf MachiActions_Click
        AddHandler _actionMachiRedownload.Click, AddressOf MachiRedownload_Click
        AddHandler _actionMachiDelete.Click, AddressOf MachiDelete_Click
    End Sub

    Private Sub BuildCharaActionsMenu()
        _actionCharaRedownload = New ToolStripMenuItem("Redownload")
        _actionCharaDelete = New ToolStripMenuItem("Delete") With {.ForeColor = CompactUiTheme.Danger}
        _charaActionsMenu = New ContextMenuStrip()
        _charaActionsMenu.Items.AddRange(New ToolStripItem() {
            _actionCharaRedownload,
            New ToolStripSeparator(),
            _actionCharaDelete
        })

        AddHandler _btnCharaActions.Click, AddressOf CharaActions_Click
        AddHandler _actionCharaRedownload.Click, AddressOf CharaRedownload_Click
        AddHandler _actionCharaDelete.Click, AddressOf CharaDelete_Click
    End Sub

    Private Sub MainForm_ResponsiveResize(sender As Object, e As EventArgs)
        UpdateResponsiveShellLayout()
    End Sub

    Private Sub LibraryPage_Resize(sender As Object, e As EventArgs)
        RefreshCompactLibraryLayout()
    End Sub

    Private Sub GroupBox1_Resize(sender As Object, e As EventArgs)
        RefreshCompactLibraryLayout()
    End Sub

    Private Sub Variants_VisibleChanged(sender As Object, e As EventArgs)
        RefreshCompactLibraryLayout()
    End Sub

    Private Sub LibraryList_SizeChanged(sender As Object, e As EventArgs)
        ResizeLibraryColumns()
    End Sub

    Public Sub UpdateResponsiveShellLayout()
        If MaterialTabControl1 Is Nothing OrElse MaterialTabSelector1 Is Nothing Then Return
        Dim contentTop = MaterialTabSelector1.Bottom + 5
        MaterialTabControl1.SetBounds(0, contentTop, ClientSize.Width, Math.Max(0, ClientSize.Height - contentTop - 3))
        RefreshCompactLibraryLayout()
        ResizeActivityColumns()
    End Sub

    Public Sub RefreshCompactLibraryLayout()
        If Not _compactLibraryInitialized OrElse GroupBox1 Is Nothing Then Return

        Dim contentWidth = Math.Max(160, GroupBox1.ClientSize.Width - 4)
        cbxFilterType.Width = Math.Min(136, Math.Max(110, contentWidth \ 3))
        cbxFilterType.Left = GroupBox1.ClientSize.Width - cbxFilterType.Width - 4
        txtLVSearch.Left = 5
        txtLVSearch.Width = Math.Max(120, cbxFilterType.Left - txtLVSearch.Left - 5)

        Dim footerTop = Panel1.Top
        If footerTop <= 0 Then footerTop = GroupBox1.ClientSize.Height - Panel1.Height - 2
        Dim listTop = 47
        If ListViewGamesVariants.Visible Then
            Dim variantHeight = Math.Min(116, Math.Max(80, GroupBox1.ClientSize.Height \ 5))
            ListViewGamesVariants.SetBounds(2, footerTop - variantHeight - 4, contentWidth, variantHeight)
            ListViewGames.SetBounds(2, listTop, contentWidth, Math.Max(80, ListViewGamesVariants.Top - listTop - 3))
        Else
            ListViewGames.SetBounds(2, listTop, contentWidth, Math.Max(80, footerTop - listTop - 3))
        End If

        ResizeLibraryColumns()
    End Sub

    Private Sub ResizeLibraryColumns()
        ResizeGameColumns()
        ResizeCharacterColumns(ListViewMachiChara)
        ResizeCharacterColumns(ListViewCharaDen)

        If ListViewGamesVariants IsNot Nothing AndAlso ListViewGamesVariants.Columns.Count > 0 Then
            ListViewGamesVariants.Columns(0).Width = Math.Max(120, ListViewGamesVariants.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 4)
        End If
    End Sub

    Private Sub ResizeGameColumns()
        If ListViewGames Is Nothing OrElse ListViewGames.Columns.Count < 3 Then Return
        Dim available = Math.Max(300, ListViewGames.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 4)
        Dim platformWidth = 76
        Dim statusWidth = Math.Min(128, Math.Max(104, available \ 4))
        ListViewGames.Columns(0).Width = Math.Max(140, available - statusWidth - platformWidth)
        ListViewGames.Columns(1).Width = statusWidth
        ListViewGames.Columns(2).Width = platformWidth
    End Sub

    Private Sub ResizeCharacterColumns(list As ListView)
        If list Is Nothing OrElse list.Columns.Count < 2 Then Return
        Dim available = Math.Max(240, list.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 4)
        Dim statusWidth = 96
        list.Columns(0).Width = Math.Max(140, available - statusWidth)
        list.Columns(1).Width = statusWidth
    End Sub

    Public Function GetGameKey(game As Game) As String
        If game Is Nothing OrElse String.IsNullOrWhiteSpace(game.ZIPName) Then Return String.Empty
        Return $"{Path.GetFileNameWithoutExtension(game.ZIPName)}_{game.Emulator}"
    End Function

    Public Function IsGameInstalled(game As Game) As Boolean
        If game Is Nothing OrElse String.IsNullOrWhiteSpace(game.ZIPName) Then Return False
        Try
            Dim paths = pathResolver.Resolve(game, String.Empty, DownloadsFolder)
            Return Not String.IsNullOrWhiteSpace(paths.JAM) AndAlso
                   Not String.IsNullOrWhiteSpace(paths.JAR) AndAlso
                   File.Exists(paths.JAM) AndAlso
                   File.Exists(paths.JAR)
        Catch
            Return False
        End Try
    End Function

    Public Sub ApplyGameListItemStatus(
        item As ListViewItem,
        game As Game,
        isInstalled As Boolean,
        isFavorited As Boolean,
        isCustom As Boolean)

        While item.SubItems.Count < 3
            item.SubItems.Add(String.Empty)
        End While

        Dim downloadStatus = GetGameDownloadStatus(game)
        Dim states As New List(Of String)
        If downloadStatus Is Nothing Then
            If isInstalled Then states.Add("Installed")
            If isFavorited Then states.Add("Favorite")
            If isCustom Then states.Add("Custom")
            If states.Count = 0 Then states.Add("Available")
        ElseIf downloadStatus.State = LibraryDownloadState.Failed AndAlso isInstalled Then
            states.Add("Installed + Update failed")
        Else
            states.Add(DownloadStateText(downloadStatus))
        End If

        item.UseItemStyleForSubItems = False
        item.BackColor = Color.White
        item.ForeColor = CompactUiTheme.TextPrimary
        item.SubItems(0).BackColor = Color.White
        item.SubItems(1).Text = String.Join(" + ", states)
        If downloadStatus IsNot Nothing AndAlso downloadStatus.State = LibraryDownloadState.Failed Then
            item.SubItems(1).ForeColor = CompactUiTheme.Danger
        ElseIf downloadStatus IsNot Nothing Then
            item.SubItems(1).ForeColor = CompactUiTheme.Accent
        Else
            item.SubItems(1).ForeColor = If(isInstalled, CompactUiTheme.Success, CompactUiTheme.TextSecondary)
        End If
        item.SubItems(1).BackColor = Color.White
        item.SubItems(2).Text = If(game?.Emulator, String.Empty)
        item.SubItems(2).ForeColor = CompactUiTheme.TextSecondary
        item.SubItems(2).BackColor = Color.White
    End Sub

    Public Sub ApplyCharacterListItemStatus(item As ListViewItem, isInstalled As Boolean)
        While item.SubItems.Count < 2
            item.SubItems.Add(String.Empty)
        End While
        item.UseItemStyleForSubItems = False
        item.BackColor = Color.White
        item.ForeColor = CompactUiTheme.TextPrimary
        item.SubItems(0).BackColor = Color.White
        Dim downloadStatus As LibraryDownloadStatus = Nothing
        If TypeOf item.Tag Is MachiChara Then
            downloadStatus = GetMachiDownloadStatus(DirectCast(item.Tag, MachiChara))
        ElseIf TypeOf item.Tag Is CharaDen Then
            downloadStatus = GetCharaDownloadStatus(DirectCast(item.Tag, CharaDen))
        End If

        item.SubItems(1).Text = If(downloadStatus Is Nothing, If(isInstalled, "Installed", "Available"), DownloadStateText(downloadStatus))
        If downloadStatus IsNot Nothing AndAlso downloadStatus.State = LibraryDownloadState.Failed Then
            item.SubItems(1).ForeColor = CompactUiTheme.Danger
        ElseIf downloadStatus IsNot Nothing Then
            item.SubItems(1).ForeColor = CompactUiTheme.Accent
        Else
            item.SubItems(1).ForeColor = If(isInstalled, CompactUiTheme.Success, CompactUiTheme.TextSecondary)
        End If
        item.SubItems(1).BackColor = Color.White
    End Sub

    Public Sub ShowNoGameSelected()
        If panelDynamic Is Nothing Then Return
        panelDynamic.Controls.Clear()
        gbxGameInfo.Text = "App details"
        panelDynamic.Controls.Add(CreateEmptyStateLabel("Select an app to see details and available actions."))
        SetGameActionAvailability(False, False)
    End Sub

    Public Sub ShowSelectedGameSummary(game As Game, installed As Boolean, offline As Boolean)
        If game Is Nothing Then
            ShowNoGameSelected()
            Return
        End If

        panelDynamic.Controls.Clear()
        gbxGameInfo.Text = $"App details — {game.ENTitle}"

        Dim title As New Label With {
            .AutoEllipsis = True,
            .Dock = DockStyle.Fill,
            .Font = New Font("Segoe UI", 17.0F, FontStyle.Bold),
            .ForeColor = CompactUiTheme.TextPrimary,
            .Text = game.ENTitle,
            .TextAlign = ContentAlignment.BottomLeft
        }
        Dim subtitleText = If(String.IsNullOrWhiteSpace(game.JPTitle), "No Japanese title available", game.JPTitle)
        Dim subtitle As New Label With {
            .AutoEllipsis = True,
            .Dock = DockStyle.Fill,
            .Font = New Font("Segoe UI", 10.0F),
            .ForeColor = CompactUiTheme.TextSecondary,
            .Text = subtitleText,
            .TextAlign = ContentAlignment.TopLeft
        }
        Dim facts As New Label With {
            .Dock = DockStyle.Fill,
            .Font = New Font("Segoe UI", 9.5F),
            .ForeColor = CompactUiTheme.TextPrimary,
            .Text = $"Platform: {game.Emulator}{Environment.NewLine}Status: {If(installed, "Installed", "Available to download")}",
            .TextAlign = ContentAlignment.TopLeft
        }
        Dim guidance As New Label With {
            .Dock = DockStyle.Fill,
            .Font = New Font("Segoe UI", 9.5F),
            .ForeColor = CompactUiTheme.TextSecondary,
            .Text = If(offline AndAlso Not installed,
                       "This app is not installed. Connect to the internet to download it.",
                       "Select Download to install this app."),
            .TextAlign = ContentAlignment.TopLeft
        }

        Dim layout As New TableLayoutPanel With {
            .BackColor = CompactUiTheme.Surface,
            .ColumnCount = 1,
            .Dock = DockStyle.Fill,
            .Padding = New Padding(24),
            .RowCount = 5
        }
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 58))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 46))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 58))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 70))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        layout.Controls.Add(title, 0, 0)
        layout.Controls.Add(subtitle, 0, 1)
        layout.Controls.Add(facts, 0, 2)
        layout.Controls.Add(guidance, 0, 3)
        panelDynamic.Controls.Add(layout)
    End Sub

    Private Function CreateEmptyStateLabel(text As String) As Label
        Return New Label With {
            .BackColor = CompactUiTheme.Surface,
            .Dock = DockStyle.Fill,
            .Font = New Font("Segoe UI", 10.0F),
            .ForeColor = CompactUiTheme.TextSecondary,
            .Padding = New Padding(24),
            .Text = text,
            .TextAlign = ContentAlignment.MiddleCenter
        }
    End Function

    Public Sub UpdateGameSelectionState(game As Game)
        If game Is Nothing Then
            ShowNoGameSelected()
            Return
        End If

        Dim installed = IsGameInstalled(game)
        Dim gameKey = GetGameKey(game)
        Dim favorited = Not String.IsNullOrWhiteSpace(gameKey) AndAlso favoritesManager.IsGameFavorited(gameKey)
        Dim canDownload = isOnline AndAlso Not String.IsNullOrWhiteSpace(game.ZIPName)
        Dim downloadStatus = GetGameDownloadStatus(game)

        If downloadStatus IsNot Nothing AndAlso downloadStatus.State <> LibraryDownloadState.Failed Then
            btnLaunchGame.Text = DownloadStateText(downloadStatus)
            btnLaunchGame.Enabled = False
            If _btnGameActions IsNot Nothing Then _btnGameActions.Enabled = True
            _actionRedownload.Visible = installed
            _actionRedownload.Enabled = False
            _actionBackupSave.Enabled = False
            _actionFavorite.Text = If(favorited, "Unfavorite", "Favorite")
            _actionFavorite.Enabled = True
            _actionOpenFolder.Enabled = False
            _actionDelete.Enabled = False
            cmsGameLV_Launch.Enabled = False
            cmsGameLV_Download.Enabled = False
            cmsGameLV_Delete.Enabled = False
            OpenGameFolderToolStripMenuItem.Enabled = False
            Return
        End If

        SetGameActionAvailability(True, installed, canDownload)
        btnLaunchGame.Text = If(installed, "Play", "Download")
        _actionRedownload.Visible = installed
        _actionRedownload.Enabled = canDownload
        _actionBackupSave.Enabled = installed
        _actionFavorite.Text = If(favorited, "Unfavorite", "Favorite")
        _actionFavorite.Enabled = True
        _actionOpenFolder.Enabled = installed
        _actionDelete.Enabled = installed
        cmsGameLV_Launch.Enabled = installed
        cmsGameLV_Download.Enabled = canDownload
        cmsGameLV_Download.Text = If(installed, "Redownload", "Download")
        cmsGameLV_Delete.Enabled = installed
        OpenGameFolderToolStripMenuItem.Enabled = installed
    End Sub

    Private Sub SetGameActionAvailability(hasSelection As Boolean, installed As Boolean, Optional canDownload As Boolean = False)
        btnLaunchGame.Enabled = hasSelection AndAlso (installed OrElse canDownload)
        If Not hasSelection Then btnLaunchGame.Text = "Play / Download"
        If _btnGameActions IsNot Nothing Then _btnGameActions.Enabled = hasSelection
        If _actionRedownload IsNot Nothing Then _actionRedownload.Enabled = hasSelection AndAlso installed AndAlso canDownload
        If _actionBackupSave IsNot Nothing Then _actionBackupSave.Enabled = hasSelection AndAlso installed
        If _actionFavorite IsNot Nothing Then _actionFavorite.Enabled = hasSelection
        If _actionOpenFolder IsNot Nothing Then _actionOpenFolder.Enabled = hasSelection AndAlso installed
        If _actionDelete IsNot Nothing Then _actionDelete.Enabled = hasSelection AndAlso installed
    End Sub

    Private Sub GameActions_Click(sender As Object, e As EventArgs)
        If _btnGameActions Is Nothing OrElse _gameActionsMenu Is Nothing OrElse Not _btnGameActions.Enabled Then Return
        _gameActionsMenu.Show(_btnGameActions, New Point(0, _btnGameActions.Height))
    End Sub

    Private Async Sub DownloadGame_Click(sender As Object, e As EventArgs)
        Await DownloadGames(True, True)
        If ListViewGames.SelectedItems.Count > 0 Then
            UpdateGameSelectionState(TryCast(ListViewGames.SelectedItems(0).Tag, Game))
            RefreshGameHighlighting()
        End If
    End Sub

    Private Sub FavoriteGame_Click(sender As Object, e As EventArgs)
        cmsGameLV_Favorite.PerformClick()
        If ListViewGames.SelectedItems.Count > 0 Then
            UpdateGameSelectionState(TryCast(ListViewGames.SelectedItems(0).Tag, Game))
        End If
    End Sub

    Private Sub OpenGameFolder_Click(sender As Object, e As EventArgs)
        OpenGameFolderToolStripMenuItem.PerformClick()
    End Sub

    Private Async Sub DeleteGame_Click(sender As Object, e As EventArgs)
        Await DeleteGamesAsync()
        If ListViewGames.SelectedItems.Count > 0 Then
            UpdateGameSelectionState(TryCast(ListViewGames.SelectedItems(0).Tag, Game))
        Else
            ShowNoGameSelected()
        End If
    End Sub

    Private Async Sub ListViewGames_Activate(sender As Object, e As EventArgs)
        If ListViewGames.SelectedItems.Count = 0 Then Return
        Dim game = TryCast(ListViewGames.SelectedItems(0).Tag, Game)
        If game Is Nothing Then Return

        If IsGameDownloadBusy(game) Then
            NotificationManager.ShowInformation(Me, "Download in progress", $"'{game.ENTitle}' must finish installing before it can be opened.")
            Return
        End If

        If IsGameInstalled(game) Then
            btnLaunchGame.PerformClick()
        Else
            Await DownloadGames(True, True)
            If ListViewGames.SelectedItems.Count > 0 Then
                UpdateGameSelectionState(TryCast(ListViewGames.SelectedItems(0).Tag, Game))
                RefreshGameHighlighting()
            End If
        End If
    End Sub

    Public Sub ShowNoMachiCharaSelected()
        If _lblMachiTitle Is Nothing Then Return
        _lblMachiTitle.Text = "Machi-Chara details"
        _lblMachiMetadata.Text = "Select an item to see its file and availability."
        CompactUiTheme.SetStatusBadge(_lblMachiStatus, "No selection", False)
        btnMachiCharaLaunch.Text = "Play / Download"
        btnMachiCharaLaunch.Enabled = False
        _btnMachiActions.Enabled = False
        _actionMachiRedownload.Enabled = False
        _actionMachiDelete.Enabled = False
    End Sub

    Public Sub UpdateMachiCharaSelectionState(item As MachiChara)
        If item Is Nothing Then
            ShowNoMachiCharaSelected()
            Return
        End If
        Dim installed = File.Exists(Path.Combine(DownloadsFolder, item.CFDName))
        Dim downloadStatus = GetMachiDownloadStatus(item)
        _lblMachiTitle.Text = item.ENTitle
        _lblMachiMetadata.Text = BuildCharacterMetadata(item.JPTitle, item.CFDName)
        If downloadStatus IsNot Nothing AndAlso downloadStatus.State <> LibraryDownloadState.Failed Then
            CompactUiTheme.SetStatusBadge(_lblMachiStatus, DownloadStateText(downloadStatus), False)
            _lblMachiStatus.ForeColor = CompactUiTheme.Accent
            btnMachiCharaLaunch.Text = DownloadStateText(downloadStatus)
            btnMachiCharaLaunch.Enabled = False
            _btnMachiActions.Enabled = False
            _actionMachiRedownload.Enabled = False
            _actionMachiDelete.Enabled = False
            DownloadCMS_MachiChara.Enabled = False
            DeleteCMS_MachiChara.Enabled = False
            Return
        End If

        Dim downloadFailed = downloadStatus IsNot Nothing
        CompactUiTheme.SetStatusBadge(
            _lblMachiStatus,
            If(downloadFailed, "Download failed", If(installed, "Installed", If(isOnline, "Available", "Offline"))),
            installed AndAlso Not downloadFailed)
        If downloadFailed Then _lblMachiStatus.ForeColor = CompactUiTheme.Danger

        btnMachiCharaLaunch.Text = If(installed, "Play", If(downloadFailed, "Try again", "Download"))
        btnMachiCharaLaunch.Enabled = installed OrElse isOnline
        _btnMachiActions.Enabled = installed
        _actionMachiRedownload.Enabled = installed AndAlso isOnline
        _actionMachiDelete.Enabled = installed
        DownloadCMS_MachiChara.Text = If(installed, "Redownload", If(downloadFailed, "Try again", "Download"))
        DownloadCMS_MachiChara.Enabled = isOnline
        DeleteCMS_MachiChara.Enabled = installed
    End Sub

    Public Sub ShowNoCharaDenSelected()
        If _lblCharaTitle Is Nothing Then Return
        _lblCharaTitle.Text = "Chara-Den details"
        _lblCharaMetadata.Text = "Select an item to see its file and availability."
        CompactUiTheme.SetStatusBadge(_lblCharaStatus, "No selection", False)
        btnCharaDenLaunch.Text = "Play / Download"
        btnCharaDenLaunch.Enabled = False
        _btnCharaActions.Enabled = False
        _actionCharaRedownload.Enabled = False
        _actionCharaDelete.Enabled = False
    End Sub

    Public Sub UpdateCharaDenSelectionState(item As CharaDen)
        If item Is Nothing Then
            ShowNoCharaDenSelected()
            Return
        End If
        Dim installed = File.Exists(Path.Combine(DownloadsFolder, item.AFDName))
        Dim downloadStatus = GetCharaDownloadStatus(item)
        _lblCharaTitle.Text = item.ENTitle
        _lblCharaMetadata.Text = BuildCharacterMetadata(item.JPTitle, item.AFDName)
        If downloadStatus IsNot Nothing AndAlso downloadStatus.State <> LibraryDownloadState.Failed Then
            CompactUiTheme.SetStatusBadge(_lblCharaStatus, DownloadStateText(downloadStatus), False)
            _lblCharaStatus.ForeColor = CompactUiTheme.Accent
            btnCharaDenLaunch.Text = DownloadStateText(downloadStatus)
            btnCharaDenLaunch.Enabled = False
            _btnCharaActions.Enabled = False
            _actionCharaRedownload.Enabled = False
            _actionCharaDelete.Enabled = False
            DownloadCMS_CharaDen.Enabled = False
            DeleteCMS_CharaDen.Enabled = False
            Return
        End If

        Dim downloadFailed = downloadStatus IsNot Nothing
        CompactUiTheme.SetStatusBadge(
            _lblCharaStatus,
            If(downloadFailed, "Download failed", If(installed, "Installed", If(isOnline, "Available", "Offline"))),
            installed AndAlso Not downloadFailed)
        If downloadFailed Then _lblCharaStatus.ForeColor = CompactUiTheme.Danger

        btnCharaDenLaunch.Text = If(installed, "Play", If(downloadFailed, "Try again", "Download"))
        btnCharaDenLaunch.Enabled = installed OrElse isOnline
        _btnCharaActions.Enabled = installed
        _actionCharaRedownload.Enabled = installed AndAlso isOnline
        _actionCharaDelete.Enabled = installed
        DownloadCMS_CharaDen.Text = If(installed, "Redownload", If(downloadFailed, "Try again", "Download"))
        DownloadCMS_CharaDen.Enabled = isOnline
        DeleteCMS_CharaDen.Enabled = installed
    End Sub

    Private Function BuildCharacterMetadata(japaneseTitle As String, fileName As String) As String
        Dim title = If(String.IsNullOrWhiteSpace(japaneseTitle), "No Japanese title available", japaneseTitle)
        Return $"{title}{Environment.NewLine}File: {fileName}"
    End Function

    Private Sub MachiActions_Click(sender As Object, e As EventArgs)
        If Not _btnMachiActions.Enabled Then Return
        _machiActionsMenu.Show(_btnMachiActions, New Point(0, _btnMachiActions.Height))
    End Sub

    Private Sub MachiRedownload_Click(sender As Object, e As EventArgs)
        If ListViewMachiChara.SelectedItems.Count = 0 Then Return
        DownloadMachiChara(TryCast(ListViewMachiChara.SelectedItems(0).Tag, MachiChara), True)
    End Sub

    Private Async Sub MachiDelete_Click(sender As Object, e As EventArgs)
        Await DeleteMachiCharaAsync()
        If ListViewMachiChara.SelectedItems.Count > 0 Then
            UpdateMachiCharaSelectionState(TryCast(ListViewMachiChara.SelectedItems(0).Tag, MachiChara))
        End If
    End Sub

    Private Sub CharaActions_Click(sender As Object, e As EventArgs)
        If Not _btnCharaActions.Enabled Then Return
        _charaActionsMenu.Show(_btnCharaActions, New Point(0, _btnCharaActions.Height))
    End Sub

    Private Sub CharaRedownload_Click(sender As Object, e As EventArgs)
        If ListViewCharaDen.SelectedItems.Count = 0 Then Return
        DownloadCharaDen(TryCast(ListViewCharaDen.SelectedItems(0).Tag, CharaDen), True)
    End Sub

    Private Async Sub CharaDelete_Click(sender As Object, e As EventArgs)
        Await DeleteCharadenAsync()
        If ListViewCharaDen.SelectedItems.Count > 0 Then
            UpdateCharaDenSelectionState(TryCast(ListViewCharaDen.SelectedItems(0).Tag, CharaDen))
        End If
    End Sub

    Private Sub MachiList_Activate(sender As Object, e As EventArgs)
        ActivateSelectedMachiChara()
    End Sub

    Private Sub CharaList_Activate(sender As Object, e As EventArgs)
        ActivateSelectedCharaDen()
    End Sub

    Private Sub MachiSearch_TextChanged(sender As Object, e As EventArgs)
        FilterMachiCharaList(_txtMachiSearch.Text)
    End Sub

    Private Sub CharaSearch_TextChanged(sender As Object, e As EventArgs)
        FilterCharaDenList(_txtCharaSearch.Text)
    End Sub

    Private Sub FilterMachiCharaList(searchText As String)
        If machicharas Is Nothing Then Return
        Dim query = searchText.Trim()
        ListViewMachiChara.BeginUpdate()
        Try
            ListViewMachiChara.Items.Clear()
            For Each item In machicharas
                If MatchesCharacterSearch(item.ENTitle, item.JPTitle, query) Then
                    Dim row As New ListViewItem(item.ENTitle) With {.Tag = item}
                    ApplyCharacterListItemStatus(row, File.Exists(Path.Combine(DownloadsFolder, item.CFDName)))
                    ListViewMachiChara.Items.Add(row)
                End If
            Next
            lblMachiCharaTotalCount.Text = $"{ListViewMachiChara.Items.Count:N0} items"
        Finally
            ListViewMachiChara.EndUpdate()
        End Try
    End Sub

    Private Sub FilterCharaDenList(searchText As String)
        If charadens Is Nothing Then Return
        Dim query = searchText.Trim()
        ListViewCharaDen.BeginUpdate()
        Try
            ListViewCharaDen.Items.Clear()
            For Each item In charadens
                If MatchesCharacterSearch(item.ENTitle, item.JPTitle, query) Then
                    Dim row As New ListViewItem(item.ENTitle) With {.Tag = item}
                    ApplyCharacterListItemStatus(row, File.Exists(Path.Combine(DownloadsFolder, item.AFDName)))
                    ListViewCharaDen.Items.Add(row)
                End If
            Next
            lblCharadenTotalCount.Text = $"{ListViewCharaDen.Items.Count:N0} items"
        Finally
            ListViewCharaDen.EndUpdate()
        End Try
    End Sub

    Private Function MatchesCharacterSearch(englishTitle As String, japaneseTitle As String, query As String) As Boolean
        If String.IsNullOrWhiteSpace(query) Then Return True
        Return (Not String.IsNullOrWhiteSpace(englishTitle) AndAlso englishTitle.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) OrElse
               (Not String.IsNullOrWhiteSpace(japaneseTitle) AndAlso japaneseTitle.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
    End Function
End Class
