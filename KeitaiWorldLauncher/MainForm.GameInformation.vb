Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Xml
Imports KeitaiWorldLauncher.My.logger
Imports KeitaiWorldLauncher.My.Managers
Imports KeitaiWorldLauncher.My.Models

Partial Public Class MainForm
    Private Function CreateGameInformationSection(game As Game) As Control
        Dim infoXmlPath = pathResolver.Resolve(game, String.Empty, DownloadsFolder).InfoXml
        If Not File.Exists(infoXmlPath) Then Return Nothing

        Dim fields As IReadOnlyList(Of KeyValuePair(Of String, String)) = Nothing
        Dim statusText As String = Nothing
        Try
            fields = GameInfoManager.LoadFields(infoXmlPath)
            If fields.Count = 0 Then statusText = "No additional game information is included."
        Catch ex As Exception When TypeOf ex Is XmlException OrElse TypeOf ex Is InvalidDataException OrElse
                                   TypeOf ex Is IOException OrElse TypeOf ex Is UnauthorizedAccessException
            Logger.LogWarning($"Unable to load game information from '{infoXmlPath}': {ex.Message}")
            statusText = "The game information could not be loaded."
        End Try

        Dim section As New TableLayoutPanel With {
            .AccessibleName = "Game information",
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .BackColor = CompactUiTheme.Surface,
            .ColumnCount = 1,
            .Dock = DockStyle.Top,
            .Margin = New Padding(0),
            .Name = "GameInformationSection",
            .RowCount = 2
        }
        section.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        section.RowStyles.Add(New RowStyle(SizeType.Absolute, 42.0F))
        section.RowStyles.Add(New RowStyle(SizeType.AutoSize))

        Dim header As New TableLayoutPanel With {
            .BackColor = CompactUiTheme.NeutralBackground,
            .ColumnCount = 2,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0, 0, 0, 1),
            .Padding = New Padding(12, 5, 8, 5),
            .RowCount = 1
        }
        header.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        header.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 112.0F))
        header.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        header.Controls.Add(New Label With {
            .AutoEllipsis = True,
            .Dock = DockStyle.Fill,
            .Font = FixedLayout.CreateFont("Segoe UI Semibold", 9.2F, FontStyle.Bold),
            .ForeColor = CompactUiTheme.TextPrimary,
            .Text = "Game Information",
            .TextAlign = ContentAlignment.MiddleLeft
        }, 0, 0)

        Dim toggleButton = CompactUiTheme.CreateCompactButton("Hide details")
        toggleButton.AccessibleName = "Hide game information"
        toggleButton.AccessibleDescription = "Shows or hides the additional information included with this game."
        toggleButton.Dock = DockStyle.Fill
        toggleButton.Margin = New Padding(0)
        toggleButton.Name = "GameInformationToggle"
        header.Controls.Add(toggleButton, 1, 0)

        Dim content As New TableLayoutPanel With {
            .AccessibleName = "Game information fields",
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .ColumnCount = 2,
            .Dock = DockStyle.Top,
            .Margin = New Padding(0),
            .Name = "GameInformationFields",
            .Padding = New Padding(12, 8, 12, 8),
            .RowCount = 0
        }
        content.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 132.0F))
        content.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))

        If statusText IsNot Nothing Then
            Dim status = CreateGameInformationLabel(statusText, False)
            content.RowCount = 1
            content.RowStyles.Add(New RowStyle(SizeType.AutoSize))
            content.Controls.Add(status, 0, 0)
            content.SetColumnSpan(status, 2)
        Else
            For Each field In fields
                Dim rowIndex = content.RowCount
                content.RowCount += 1
                content.RowStyles.Add(New RowStyle(SizeType.AutoSize))
                Dim name = Regex.Replace(field.Key, "([a-z0-9])([A-Z])", "$1 $2").Replace("_", " ").Replace("-", " ")
                name = name.ToUpperInvariant()
                content.Controls.Add(CreateGameInformationLabel(name, True), 0, rowIndex)
                content.Controls.Add(CreateGameInformationLabel(field.Value, False), 1, rowIndex)
            Next
        End If

        Dim expanded = True
        AddHandler toggleButton.Click,
            Sub()
                expanded = Not expanded
                section.SuspendLayout()
                content.Visible = expanded
                section.RowStyles(1).SizeType = If(expanded, SizeType.AutoSize, SizeType.Absolute)
                section.RowStyles(1).Height = 0
                toggleButton.Text = If(expanded, "Hide details", "Show details")
                toggleButton.AccessibleName = If(expanded, "Hide game information", "Show game information")
                section.ResumeLayout(True)
            End Sub

        section.Controls.Add(header, 0, 0)
        section.Controls.Add(content, 0, 1)
        Return section
    End Function

    Private Function CreateGameInformationLabel(text As String, isFieldName As Boolean) As Label
        Return New Label With {
            .AccessibleName = text,
            .AutoSize = True,
            .Dock = DockStyle.Fill,
            .Font = FixedLayout.CreateFont("Segoe UI", 9.0F, If(isFieldName, FontStyle.Bold, FontStyle.Regular)),
            .ForeColor = If(isFieldName, CompactUiTheme.TextSecondary, CompactUiTheme.TextPrimary),
            .Margin = New Padding(0, 4, If(isFieldName, 12, 0), 8),
            .Text = text,
            .UseMnemonic = False
        }
    End Function
End Class
