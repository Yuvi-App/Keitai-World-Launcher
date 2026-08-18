Imports System.Drawing
Imports System.Windows.Forms

Public Enum CompactDialogTone
    Information
    Warning
    [Error]
    Danger
    Success
End Enum

Public NotInheritable Class CompactChoiceDialogResult
    Public Property Accepted As Boolean
    Public Property SelectedValue As String
    Public Property OptionChecked As Boolean
End Class

Public Class UIDialogManager
    Public Function ShowConfirmation(
        owner As Form,
        title As String,
        message As String,
        Optional confirmText As String = "Continue",
        Optional cancelText As String = "Cancel",
        Optional tone As CompactDialogTone = CompactDialogTone.Information,
        Optional defaultToCancel As Boolean = False
    ) As DialogResult
        Using dialog As New CompactDialogForm(title, message, confirmText, cancelText, True, tone, defaultToCancel)
            Return ShowOwnedDialog(dialog, owner)
        End Using
    End Function

    Public Function ShowNotice(
        owner As Form,
        title As String,
        message As String,
        Optional buttonText As String = "OK",
        Optional tone As CompactDialogTone = CompactDialogTone.Information
    ) As DialogResult
        Using dialog As New CompactDialogForm(title, message, buttonText, String.Empty, False, tone)
            Return ShowOwnedDialog(dialog, owner)
        End Using
    End Function

    Public Sub ShowError(owner As Form, title As String, message As String)
        ShowNotice(owner, title, message, "OK", CompactDialogTone.Error)
    End Sub

    ' Preserve the original public helpers while routing them through the compact UI.
    Public Function ShowMaterialDialogOk(
        ownerForm As Form,
        title As String,
        message As String,
        Optional okText As String = "OK",
        Optional useAccent As Boolean = True
    ) As DialogResult
        Return ShowNotice(ownerForm, title, message, okText)
    End Function

    Public Function ShowMaterialDialogYesNo(
        ownerForm As Form,
        title As String,
        message As String,
        Optional yesText As String = "Yes",
        Optional noText As String = "No",
        Optional useAccent As Boolean = True
    ) As DialogResult
        Return ShowConfirmation(ownerForm, title, message, yesText, noText)
    End Function

    Public Sub ShowMaterialError(owner As Form, message As String)
        ShowError(owner, "Something went wrong", message)
    End Sub

    Public Function ShowNetworkIdentityDialog(
        owner As Form,
        currentNetworkUid As String,
        currentTerminalId As String,
        ByRef networkUid As String,
        ByRef terminalId As String
    ) As DialogResult
        Using dialog As New CompactNetworkIdentityDialog(currentNetworkUid, currentTerminalId)
            Dim result = ShowOwnedDialog(dialog, owner)
            If result = DialogResult.OK Then
                networkUid = dialog.NetworkUid.Trim().ToUpperInvariant()
                terminalId = dialog.TerminalId.Trim().ToUpperInvariant()
            End If
            Return result
        End Using
    End Function

    Public Function ShowChoice(
        owner As Form,
        title As String,
        message As String,
        options As IEnumerable(Of String),
        Optional optionText As String = "",
        Optional optionChecked As Boolean = False
    ) As CompactChoiceDialogResult
        Using dialog As New CompactChoiceDialog(title, message, options, optionText, optionChecked)
            Dim result = ShowOwnedDialog(dialog, owner)
            Return New CompactChoiceDialogResult With {
                .Accepted = result = DialogResult.OK,
                .SelectedValue = dialog.SelectedValue,
                .OptionChecked = dialog.OptionChecked
            }
        End Using
    End Function

    Public Function ShowDetailsConfirmation(
        owner As Form,
        title As String,
        details As String,
        Optional confirmText As String = "Continue",
        Optional cancelText As String = "Cancel",
        Optional tone As CompactDialogTone = CompactDialogTone.Information
    ) As DialogResult
        Using dialog As New CompactDetailsDialog(title, details, confirmText, cancelText, True, tone)
            Return ShowOwnedDialog(dialog, owner)
        End Using
    End Function

    Public Function ShowDetailsNotice(
        owner As Form,
        title As String,
        details As String,
        Optional buttonText As String = "Close",
        Optional tone As CompactDialogTone = CompactDialogTone.Information
    ) As DialogResult
        Using dialog As New CompactDetailsDialog(title, details, buttonText, String.Empty, False, tone)
            Return ShowOwnedDialog(dialog, owner)
        End Using
    End Function

    Public Sub ShowKeybindGuide(owner As Form, keybindText As String, imagePath As String)
        Dim guideImage As Image = Nothing
        If IO.File.Exists(imagePath) Then
            Using sourceImage = Image.FromFile(imagePath)
                guideImage = New Bitmap(sourceImage)
            End Using
        End If

        Try
            Using dialog As New CompactGuideDialog(keybindText, guideImage)
                ShowOwnedDialog(dialog, owner)
            End Using
        Finally
            guideImage?.Dispose()
        End Try
    End Sub

    Private Shared Function ShowOwnedDialog(dialog As Form, owner As Form) As DialogResult
        If owner Is Nothing OrElse owner.IsDisposed Then Return dialog.ShowDialog()
        If owner.InvokeRequired Then
            Return CType(owner.Invoke(New Func(Of DialogResult)(Function() dialog.ShowDialog(owner))), DialogResult)
        End If
        Return dialog.ShowDialog(owner)
    End Function

    Private MustInherit Class CompactCustomDialogForm
        Inherits Form

        Protected ReadOnly ContentHost As Panel
        Protected ReadOnly FooterHost As Panel

        Protected Sub New(title As String, dialogWidth As Integer, dialogHeight As Integer, tone As CompactDialogTone)
            AutoScaleMode = AutoScaleMode.Dpi
            BackColor = CompactUiTheme.Surface
            ClientSize = New Size(dialogWidth, dialogHeight)
            Font = New Font("Segoe UI", 9.0F)
            FormBorderStyle = FormBorderStyle.None
            KeyPreview = True
            MaximizeBox = False
            MinimizeBox = False
            Padding = New Padding(1)
            ShowIcon = False
            ShowInTaskbar = False
            StartPosition = FormStartPosition.CenterParent
            Text = title

            Dim layout As New TableLayoutPanel With {
                .BackColor = CompactUiTheme.Surface,
                .ColumnCount = 1,
                .Dock = DockStyle.Fill,
                .Margin = New Padding(0),
                .Padding = New Padding(0),
                .RowCount = 4
            }
            layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
            layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 4.0F))
            layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 54.0F))
            layout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
            layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 64.0F))

            Dim accentBar As New Panel With {
                .BackColor = ToneColor(tone),
                .Dock = DockStyle.Fill,
                .Margin = New Padding(0)
            }
            Dim header As New Panel With {
                .BackColor = CompactUiTheme.Surface,
                .Dock = DockStyle.Fill,
                .Margin = New Padding(0)
            }
            Dim titleLabel As New Label With {
                .AutoEllipsis = True,
                .Font = New Font("Segoe UI Semibold", 12.0F, FontStyle.Bold),
                .ForeColor = CompactUiTheme.TextPrimary,
                .Location = New Point(21, 9),
                .Size = New Size(dialogWidth - 86, 34),
                .Text = title,
                .TextAlign = ContentAlignment.MiddleLeft,
                .UseMnemonic = False
            }
            Dim closeButton As New Button With {
                .AccessibleName = "Close",
                .BackColor = CompactUiTheme.Surface,
                .DialogResult = DialogResult.Cancel,
                .FlatStyle = FlatStyle.Flat,
                .Font = New Font("Segoe UI", 13.0F),
                .ForeColor = CompactUiTheme.TextSecondary,
                .Location = New Point(dialogWidth - 47, 7),
                .Size = New Size(32, 32),
                .TabStop = False,
                .Text = ChrW(&HD7),
                .UseVisualStyleBackColor = False
            }
            closeButton.FlatAppearance.BorderSize = 0
            closeButton.FlatAppearance.MouseOverBackColor = CompactUiTheme.NeutralBackground
            closeButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(229, 232, 238)
            header.Controls.AddRange(New Control() {titleLabel, closeButton})

            ContentHost = New Panel With {
                .BackColor = CompactUiTheme.Surface,
                .Dock = DockStyle.Fill,
                .Margin = New Padding(0),
                .Padding = New Padding(22, 15, 22, 16)
            }
            FooterHost = New Panel With {
                .BackColor = CompactUiTheme.AppBackground,
                .Dock = DockStyle.Fill,
                .Margin = New Padding(0)
            }
            layout.Controls.Add(accentBar, 0, 0)
            layout.Controls.Add(header, 0, 1)
            layout.Controls.Add(ContentHost, 0, 2)
            layout.Controls.Add(FooterHost, 0, 3)
            Controls.Add(layout)
            CancelButton = closeButton
            AddHandler closeButton.Click, Sub() Close()
        End Sub

        Protected Function ConfigureActions(
            primaryText As String,
            primaryResult As DialogResult,
            Optional secondaryText As String = "",
            Optional secondaryResult As DialogResult = DialogResult.Cancel,
            Optional tone As CompactDialogTone = CompactDialogTone.Information
        ) As Button
            Dim primaryButton = CompactUiTheme.CreateCompactButton(primaryText, True)
            primaryButton.DialogResult = primaryResult
            primaryButton.Size = New Size(128, 34)
            primaryButton.Location = New Point(FooterHost.Width - primaryButton.Width - 18, 14)
            primaryButton.Anchor = AnchorStyles.Top Or AnchorStyles.Right
            If tone = CompactDialogTone.Danger Then
                primaryButton.BackColor = CompactUiTheme.Danger
                primaryButton.FlatAppearance.BorderColor = CompactUiTheme.Danger
                primaryButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(150, 38, 38)
                primaryButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(135, 32, 32)
            End If
            FooterHost.Controls.Add(primaryButton)
            AcceptButton = primaryButton

            If Not String.IsNullOrWhiteSpace(secondaryText) Then
                Dim secondaryButton = CompactUiTheme.CreateCompactButton(secondaryText)
                secondaryButton.DialogResult = secondaryResult
                secondaryButton.Size = New Size(100, 34)
                secondaryButton.Location = New Point(primaryButton.Left - secondaryButton.Width - 10, 14)
                secondaryButton.Anchor = AnchorStyles.Top Or AnchorStyles.Right
                FooterHost.Controls.Add(secondaryButton)
                CancelButton = secondaryButton
            End If
            Return primaryButton
        End Function

        Protected Overrides Sub OnShown(e As EventArgs)
            MyBase.OnShown(e)
            If Owner Is Nothing Then Return
            Dim workingArea = Screen.FromControl(Owner).WorkingArea
            Dim targetX = Owner.Left + ((Owner.Width - Width) \ 2)
            Dim targetY = Owner.Top + ((Owner.Height - Height) \ 2)
            targetX = Math.Max(workingArea.Left, Math.Min(targetX, workingArea.Right - Width))
            targetY = Math.Max(workingArea.Top, Math.Min(targetY, workingArea.Bottom - Height))
            Location = New Point(targetX, targetY)
        End Sub

        Protected Overrides Sub OnPaint(e As PaintEventArgs)
            MyBase.OnPaint(e)
            Using borderPen As New Pen(CompactUiTheme.Border)
                e.Graphics.DrawRectangle(borderPen, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1)
            End Using
        End Sub

        Private Shared Function ToneColor(tone As CompactDialogTone) As Color
            Select Case tone
                Case CompactDialogTone.Warning
                    Return CompactUiTheme.Accent
                Case CompactDialogTone.Error, CompactDialogTone.Danger
                    Return CompactUiTheme.Danger
                Case CompactDialogTone.Success
                    Return CompactUiTheme.Success
                Case Else
                    Return CompactUiTheme.Primary
            End Select
        End Function
    End Class

    Private NotInheritable Class CompactNetworkIdentityDialog
        Inherits CompactCustomDialogForm

        Private ReadOnly _networkUidTextBox As TextBox
        Private ReadOnly _terminalIdTextBox As TextBox
        Private ReadOnly _validationLabel As Label

        Public ReadOnly Property NetworkUid As String
            Get
                Return _networkUidTextBox.Text
            End Get
        End Property

        Public ReadOnly Property TerminalId As String
            Get
                Return _terminalIdTextBox.Text
            End Get
        End Property

        Public Sub New(currentNetworkUid As String, currentTerminalId As String)
            MyBase.New("Network identity", 560, 390, CompactDialogTone.Information)

            Dim formLayout As New TableLayoutPanel With {
                .BackColor = CompactUiTheme.Surface,
                .ColumnCount = 1,
                .Dock = DockStyle.Fill,
                .Margin = New Padding(0),
                .RowCount = 6
            }
            formLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
            formLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 68.0F))
            formLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 22.0F))
            formLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 36.0F))
            formLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 22.0F))
            formLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 36.0F))
            formLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

            Dim instructions As New Label With {
                .Dock = DockStyle.Fill,
                .Font = New Font("Segoe UI", 9.0F),
                .ForeColor = CompactUiTheme.TextSecondary,
                .Text = "Get your Network UID from the Keitai Wiki Discord:" & Environment.NewLine &
                        "1. Open the #Butler-sheep channel.  2. Select Get-UID.",
                .TextAlign = ContentAlignment.TopLeft
            }
            Dim uidLabel = CreateFieldLabel("Network UID")
            Dim tidLabel = CreateFieldLabel("Terminal ID")
            _networkUidTextBox = CreateTextBox(currentNetworkUid, "Network UID")
            _terminalIdTextBox = CreateTextBox(currentTerminalId, "Terminal ID")
            _validationLabel = New Label With {
                .Dock = DockStyle.Fill,
                .Font = New Font("Segoe UI", 8.5F, FontStyle.Bold),
                .ForeColor = CompactUiTheme.Danger,
                .Text = "Both values are required.",
                .TextAlign = ContentAlignment.MiddleLeft,
                .Visible = False
            }
            formLayout.Controls.Add(instructions, 0, 0)
            formLayout.Controls.Add(uidLabel, 0, 1)
            formLayout.Controls.Add(_networkUidTextBox, 0, 2)
            formLayout.Controls.Add(tidLabel, 0, 3)
            formLayout.Controls.Add(_terminalIdTextBox, 0, 4)
            formLayout.Controls.Add(_validationLabel, 0, 5)
            ContentHost.Controls.Add(formLayout)

            Dim saveButton = ConfigureActions("Save identity", DialogResult.None, "Cancel")
            AddHandler saveButton.Click, AddressOf SaveIdentity_Click
        End Sub

        Private Sub SaveIdentity_Click(sender As Object, e As EventArgs)
            If String.IsNullOrWhiteSpace(_networkUidTextBox.Text) OrElse String.IsNullOrWhiteSpace(_terminalIdTextBox.Text) Then
                _validationLabel.Visible = True
                Return
            End If
            DialogResult = DialogResult.OK
            Close()
        End Sub

        Private Shared Function CreateFieldLabel(text As String) As Label
            Return New Label With {
                .Dock = DockStyle.Fill,
                .Font = New Font("Segoe UI Semibold", 8.7F, FontStyle.Bold),
                .ForeColor = CompactUiTheme.TextPrimary,
                .Text = text,
                .TextAlign = ContentAlignment.MiddleLeft
            }
        End Function

        Private Shared Function CreateTextBox(value As String, accessibleName As String) As TextBox
            Return New TextBox With {
                .AccessibleName = accessibleName,
                .BackColor = Color.FromArgb(249, 250, 252),
                .BorderStyle = BorderStyle.FixedSingle,
                .Dock = DockStyle.Fill,
                .Font = New Font("Segoe UI", 9.5F),
                .Margin = New Padding(0, 2, 0, 3),
                .MaxLength = 50,
                .Text = value
            }
        End Function
    End Class

    Private NotInheritable Class CompactChoiceDialog
        Inherits CompactCustomDialogForm

        Private ReadOnly _choiceComboBox As ComboBox
        Private ReadOnly _optionCheckBox As CheckBox

        Public ReadOnly Property SelectedValue As String
            Get
                Return If(_choiceComboBox.SelectedItem?.ToString(), String.Empty)
            End Get
        End Property

        Public ReadOnly Property OptionChecked As Boolean
            Get
                Return _optionCheckBox.Checked
            End Get
        End Property

        Public Sub New(title As String, message As String, options As IEnumerable(Of String), optionText As String, optionChecked As Boolean)
            MyBase.New(title, 480, 300, CompactDialogTone.Information)

            Dim formLayout As New TableLayoutPanel With {
                .BackColor = CompactUiTheme.Surface,
                .ColumnCount = 1,
                .Dock = DockStyle.Fill,
                .Margin = New Padding(0),
                .RowCount = 4
            }
            formLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
            formLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 58.0F))
            formLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 22.0F))
            formLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 38.0F))
            formLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
            Dim messageLabel As New Label With {
                .Dock = DockStyle.Fill,
                .Font = New Font("Segoe UI", 9.0F),
                .ForeColor = CompactUiTheme.TextSecondary,
                .Text = message,
                .TextAlign = ContentAlignment.TopLeft
            }
            Dim choiceLabel As New Label With {
                .Dock = DockStyle.Fill,
                .Font = New Font("Segoe UI Semibold", 8.7F, FontStyle.Bold),
                .ForeColor = CompactUiTheme.TextPrimary,
                .Text = "Emulator",
                .TextAlign = ContentAlignment.MiddleLeft
            }
            _choiceComboBox = New ComboBox With {
                .AccessibleName = "Emulator",
                .BackColor = Color.FromArgb(246, 247, 250),
                .Dock = DockStyle.Fill,
                .DropDownStyle = ComboBoxStyle.DropDownList,
                .FlatStyle = FlatStyle.Flat,
                .Font = New Font("Segoe UI", 9.5F),
                .Margin = New Padding(0, 3, 0, 5)
            }
            For Each optionValue In options
                _choiceComboBox.Items.Add(optionValue)
            Next
            If _choiceComboBox.Items.Count > 0 Then _choiceComboBox.SelectedIndex = 0
            _optionCheckBox = New CheckBox With {
                .AutoSize = False,
                .BackColor = CompactUiTheme.Surface,
                .Checked = optionChecked,
                .Dock = DockStyle.Fill,
                .FlatStyle = FlatStyle.Flat,
                .Font = New Font("Segoe UI", 9.0F),
                .Text = optionText,
                .UseVisualStyleBackColor = False,
                .Visible = Not String.IsNullOrWhiteSpace(optionText)
            }
            formLayout.Controls.Add(messageLabel, 0, 0)
            formLayout.Controls.Add(choiceLabel, 0, 1)
            formLayout.Controls.Add(_choiceComboBox, 0, 2)
            formLayout.Controls.Add(_optionCheckBox, 0, 3)
            ContentHost.Controls.Add(formLayout)
            ConfigureActions("Continue", DialogResult.OK, "Cancel")
        End Sub
    End Class

    Private NotInheritable Class CompactDetailsDialog
        Inherits CompactCustomDialogForm

        Public Sub New(
            title As String,
            details As String,
            primaryText As String,
            secondaryText As String,
            showSecondaryAction As Boolean,
            tone As CompactDialogTone
        )
            MyBase.New(title, 560, 430, tone)
            Dim detailsBox As New TextBox With {
                .BackColor = CompactUiTheme.Surface,
                .BorderStyle = BorderStyle.None,
                .Dock = DockStyle.Fill,
                .Font = New Font("Segoe UI", 9.5F),
                .ForeColor = CompactUiTheme.TextSecondary,
                .Multiline = True,
                .ReadOnly = True,
                .ScrollBars = ScrollBars.Vertical,
                .TabStop = False,
                .Text = details
            }
            ContentHost.Controls.Add(detailsBox)
            Dim actionButton As Button
            If showSecondaryAction Then
                actionButton = ConfigureActions(primaryText, DialogResult.Yes, secondaryText, DialogResult.No, tone)
            Else
                actionButton = ConfigureActions(primaryText, DialogResult.OK, String.Empty, DialogResult.Cancel, tone)
            End If
            AddHandler Shown, Sub() actionButton.Focus()
        End Sub
    End Class

    Private NotInheritable Class CompactGuideDialog
        Inherits CompactCustomDialogForm

        Public Sub New(keybindText As String, guideImage As Image)
            MyBase.New("Keyboard & Controller Guide", 860, 520, CompactDialogTone.Information)

            Dim guideLayout As New TableLayoutPanel With {
                .BackColor = CompactUiTheme.Surface,
                .ColumnCount = 2,
                .Dock = DockStyle.Fill,
                .Margin = New Padding(0),
                .RowCount = 1
            }
            guideLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 44.0F))
            guideLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 56.0F))
            guideLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
            Dim bindingsBox As New TextBox With {
                .BackColor = Color.FromArgb(249, 250, 252),
                .BorderStyle = BorderStyle.FixedSingle,
                .Dock = DockStyle.Fill,
                .Font = New Font("Consolas", 9.3F),
                .ForeColor = CompactUiTheme.TextPrimary,
                .Margin = New Padding(0, 0, 12, 0),
                .Multiline = True,
                .ReadOnly = True,
                .ScrollBars = ScrollBars.Vertical,
                .TabStop = False,
                .Text = keybindText
            }
            Dim imageSurface As New Panel With {
                .BackColor = CompactUiTheme.NeutralBackground,
                .Dock = DockStyle.Fill,
                .Margin = New Padding(0),
                .Padding = New Padding(12)
            }
            If guideImage Is Nothing Then
                imageSurface.Controls.Add(New Label With {
                    .Dock = DockStyle.Fill,
                    .Font = New Font("Segoe UI", 9.0F),
                    .ForeColor = CompactUiTheme.TextSecondary,
                    .Text = "The control diagram is not installed.",
                    .TextAlign = ContentAlignment.MiddleCenter
                })
            Else
                imageSurface.Controls.Add(New PictureBox With {
                    .Dock = DockStyle.Fill,
                    .Image = guideImage,
                    .SizeMode = PictureBoxSizeMode.Zoom
                })
            End If
            guideLayout.Controls.Add(bindingsBox, 0, 0)
            guideLayout.Controls.Add(imageSurface, 1, 0)
            ContentHost.Controls.Add(guideLayout)
            Dim closeButton = ConfigureActions("Close", DialogResult.OK)
            AddHandler Shown, Sub() closeButton.Focus()
        End Sub
    End Class

    Private NotInheritable Class CompactDialogForm
        Inherits Form

        Private Const DialogWidth As Integer = 460
        Private Const HorizontalPadding As Integer = 22

        Public Sub New(
            title As String,
            message As String,
            primaryText As String,
            secondaryText As String,
            showSecondaryAction As Boolean,
            tone As CompactDialogTone,
            Optional defaultToSecondary As Boolean = False
        )
            AutoScaleMode = AutoScaleMode.Dpi
            BackColor = CompactUiTheme.Surface
            Font = New Font("Segoe UI", 9.0F)
            FormBorderStyle = FormBorderStyle.None
            KeyPreview = True
            MaximizeBox = False
            MinimizeBox = False
            Name = "CompactLauncherDialog"
            Padding = New Padding(1)
            ShowIcon = False
            ShowInTaskbar = False
            StartPosition = FormStartPosition.CenterParent
            Text = title

            Dim accentColor = GetToneColor(tone)
            Dim messageFont As New Font("Segoe UI", 9.5F)
            Dim availableTextWidth = DialogWidth - (HorizontalPadding * 2)
            Dim measuredMessage = TextRenderer.MeasureText(
                message,
                messageFont,
                New Size(availableTextWidth, 0),
                TextFormatFlags.WordBreak Or TextFormatFlags.TextBoxControl
            )
            Dim messageHeight = Math.Max(46, Math.Min(220, measuredMessage.Height + 6))
            Dim messageTop = 72
            Dim footerTop = messageTop + messageHeight + 20
            Dim dialogHeight = Math.Max(210, footerTop + 64)
            ClientSize = New Size(DialogWidth, dialogHeight)

            Dim accentBar As New Panel With {
                .BackColor = accentColor,
                .Location = New Point(1, 1),
                .Size = New Size(DialogWidth - 2, 4)
            }

            Dim titleLabel As New Label With {
                .AutoEllipsis = True,
                .Font = New Font("Segoe UI Semibold", 12.0F, FontStyle.Bold),
                .ForeColor = CompactUiTheme.TextPrimary,
                .Location = New Point(HorizontalPadding, 19),
                .Size = New Size(DialogWidth - 88, 28),
                .Text = title,
                .TextAlign = ContentAlignment.MiddleLeft
            }

            Dim closeButton As New Button With {
                .AccessibleName = "Close",
                .BackColor = CompactUiTheme.Surface,
                .DialogResult = DialogResult.Cancel,
                .FlatStyle = FlatStyle.Flat,
                .Font = New Font("Segoe UI", 13.0F),
                .ForeColor = CompactUiTheme.TextSecondary,
                .Location = New Point(DialogWidth - 47, 12),
                .Size = New Size(32, 32),
                .TabStop = False,
                .Text = ChrW(&HD7),
                .UseVisualStyleBackColor = False
            }
            closeButton.FlatAppearance.BorderSize = 0
            closeButton.FlatAppearance.MouseOverBackColor = CompactUiTheme.NeutralBackground
            closeButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(229, 232, 238)

            Dim divider As New Panel With {
                .BackColor = CompactUiTheme.Border,
                .Location = New Point(1, 57),
                .Size = New Size(DialogWidth - 2, 1)
            }

            Dim messageControl As Control
            If measuredMessage.Height + 6 > 220 Then
                messageControl = New TextBox With {
                    .BackColor = CompactUiTheme.Surface,
                    .BorderStyle = BorderStyle.None,
                    .Font = messageFont,
                    .ForeColor = CompactUiTheme.TextSecondary,
                    .Location = New Point(HorizontalPadding, messageTop),
                    .Multiline = True,
                    .ReadOnly = True,
                .ScrollBars = ScrollBars.Vertical,
                    .TabStop = False,
                    .Size = New Size(availableTextWidth, messageHeight),
                    .Text = message
                }
            Else
                messageControl = New Label With {
                    .Font = messageFont,
                    .ForeColor = CompactUiTheme.TextSecondary,
                    .Location = New Point(HorizontalPadding, messageTop),
                    .Size = New Size(availableTextWidth, messageHeight),
                    .Text = message,
                    .TextAlign = ContentAlignment.TopLeft
                }
            End If

            Dim footer As New Panel With {
                .BackColor = CompactUiTheme.AppBackground,
                .Location = New Point(1, footerTop),
                .Size = New Size(DialogWidth - 2, dialogHeight - footerTop - 1)
            }

            Dim primaryButton = CompactUiTheme.CreateCompactButton(primaryText, True)
            primaryButton.DialogResult = If(showSecondaryAction, DialogResult.Yes, DialogResult.OK)
            primaryButton.Size = New Size(112, 34)
            primaryButton.Location = New Point(footer.Width - primaryButton.Width - 18, 14)
            If tone = CompactDialogTone.Danger Then
                primaryButton.BackColor = CompactUiTheme.Danger
                primaryButton.FlatAppearance.BorderColor = CompactUiTheme.Danger
                primaryButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(150, 38, 38)
                primaryButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(135, 32, 32)
            End If
            footer.Controls.Add(primaryButton)
            AcceptButton = primaryButton

            If showSecondaryAction Then
                Dim secondaryButton = CompactUiTheme.CreateCompactButton(secondaryText)
                secondaryButton.DialogResult = DialogResult.No
                secondaryButton.Size = New Size(92, 34)
                secondaryButton.Location = New Point(primaryButton.Left - secondaryButton.Width - 10, 14)
                footer.Controls.Add(secondaryButton)
                CancelButton = secondaryButton
                If defaultToSecondary Then AcceptButton = secondaryButton
            Else
                CancelButton = closeButton
            End If
            Controls.AddRange(New Control() {
                accentBar,
                titleLabel,
                closeButton,
                divider,
                messageControl,
                footer
            })

            AddHandler closeButton.Click, Sub() Close()
        End Sub

        Protected Overrides Sub OnShown(e As EventArgs)
            MyBase.OnShown(e)
            CenterOverOwner()
        End Sub

        Protected Overrides Sub OnPaint(e As PaintEventArgs)
            MyBase.OnPaint(e)
            Using borderPen As New Pen(CompactUiTheme.Border)
                e.Graphics.DrawRectangle(borderPen, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1)
            End Using
        End Sub

        Private Sub CenterOverOwner()
            If Owner Is Nothing Then Return

            Dim workingArea = Screen.FromControl(Owner).WorkingArea
            Dim targetX = Owner.Left + ((Owner.Width - Width) \ 2)
            Dim targetY = Owner.Top + ((Owner.Height - Height) \ 2)
            targetX = Math.Max(workingArea.Left, Math.Min(targetX, workingArea.Right - Width))
            targetY = Math.Max(workingArea.Top, Math.Min(targetY, workingArea.Bottom - Height))
            Location = New Point(targetX, targetY)
        End Sub

        Private Shared Function GetToneColor(tone As CompactDialogTone) As Color
            Select Case tone
                Case CompactDialogTone.Warning
                    Return CompactUiTheme.Accent
                Case CompactDialogTone.Error
                    Return CompactUiTheme.Danger
                Case CompactDialogTone.Danger
                    Return CompactUiTheme.Danger
                Case CompactDialogTone.Success
                    Return CompactUiTheme.Success
                Case Else
                    Return CompactUiTheme.Primary
            End Select
        End Function
    End Class
End Class
