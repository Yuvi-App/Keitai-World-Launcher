Imports System.Drawing
Imports System.Windows.Forms

Public Enum CompactDialogTone
    Information
    Warning
    [Error]
    Danger
    Success
End Enum

Public Class UIDialogManager
    Public Function ShowConfirmation(
        owner As Form,
        title As String,
        message As String,
        Optional confirmText As String = "Continue",
        Optional cancelText As String = "Cancel",
        Optional tone As CompactDialogTone = CompactDialogTone.Information
    ) As DialogResult
        Using dialog As New CompactDialogForm(title, message, confirmText, cancelText, True, tone)
            Return dialog.ShowDialog(owner)
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
            Return dialog.ShowDialog(owner)
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
            tone As CompactDialogTone
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

            Dim messageLabel As New Label With {
                .Font = messageFont,
                .ForeColor = CompactUiTheme.TextSecondary,
                .Location = New Point(HorizontalPadding, messageTop),
                .Size = New Size(availableTextWidth, messageHeight),
                .Text = message,
                .TextAlign = ContentAlignment.TopLeft
            }

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

            If showSecondaryAction Then
                Dim secondaryButton = CompactUiTheme.CreateCompactButton(secondaryText)
                secondaryButton.DialogResult = DialogResult.No
                secondaryButton.Size = New Size(92, 34)
                secondaryButton.Location = New Point(primaryButton.Left - secondaryButton.Width - 10, 14)
                footer.Controls.Add(secondaryButton)
                CancelButton = secondaryButton
            Else
                CancelButton = closeButton
            End If

            AcceptButton = primaryButton
            Controls.AddRange(New Control() {
                accentBar,
                titleLabel,
                closeButton,
                divider,
                messageLabel,
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
