Imports System.Collections.Generic
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

Public Module CompactUiTheme
    Private ReadOnly FocusCueButtons As New HashSet(Of Button)()

    Public ReadOnly AppBackground As Color = Color.FromArgb(246, 247, 251)
    Public ReadOnly Surface As Color = Color.White
    Public ReadOnly Border As Color = Color.FromArgb(218, 222, 230)
    Public ReadOnly Primary As Color = Color.FromArgb(63, 81, 181)
    Public ReadOnly PrimaryHover As Color = Color.FromArgb(52, 70, 165)
    Public ReadOnly Accent As Color = Color.FromArgb(238, 139, 34)
    Public ReadOnly TextPrimary As Color = Color.FromArgb(32, 35, 42)
    Public ReadOnly TextSecondary As Color = Color.FromArgb(92, 99, 112)
    Public ReadOnly Success As Color = Color.FromArgb(42, 125, 70)
    Public ReadOnly SuccessBackground As Color = Color.FromArgb(232, 245, 235)
    Public ReadOnly NeutralBackground As Color = Color.FromArgb(239, 241, 245)
    Public ReadOnly Danger As Color = Color.FromArgb(176, 46, 46)

    Public Function CreateCompactButton(text As String, Optional primaryAction As Boolean = False) As Button
        Dim button As New Button With {
            .AutoSize = False,
            .FlatStyle = FlatStyle.Flat,
            .Font = New Font("Segoe UI", 9.0F, If(primaryAction, FontStyle.Bold, FontStyle.Regular)),
            .Height = 32,
            .Margin = New Padding(3),
            .Padding = New Padding(8, 0, 8, 0),
            .Text = text,
            .UseVisualStyleBackColor = False
        }

        button.FlatAppearance.BorderSize = 1
        If primaryAction Then
            StylePrimaryButton(button)
        Else
            StyleSecondaryButton(button)
        End If
        Return button
    End Function

    Public Sub StylePrimaryButton(button As Button)
        EnableFocusCue(button)
        button.FlatAppearance.MouseOverBackColor = PrimaryHover
        button.FlatAppearance.MouseDownBackColor = PrimaryHover
        RemoveHandler button.EnabledChanged, AddressOf PrimaryButton_EnabledChanged
        AddHandler button.EnabledChanged, AddressOf PrimaryButton_EnabledChanged
        ApplyPrimaryButtonState(button)
    End Sub

    Private Sub PrimaryButton_EnabledChanged(sender As Object, e As EventArgs)
        ApplyPrimaryButtonState(TryCast(sender, Button))
    End Sub

    Private Sub ApplyPrimaryButtonState(button As Button)
        If button Is Nothing Then Return
        button.BackColor = If(button.Enabled, Primary, Color.FromArgb(224, 227, 234))
        button.ForeColor = If(button.Enabled, Color.White, TextSecondary)
        button.FlatAppearance.BorderColor = If(button.Enabled, Primary, Border)
    End Sub

    Public Sub StyleSecondaryButton(button As Button)
        EnableFocusCue(button)
        button.BackColor = Surface
        button.ForeColor = TextPrimary
        button.FlatAppearance.BorderColor = Border
        button.FlatAppearance.MouseOverBackColor = NeutralBackground
        button.FlatAppearance.MouseDownBackColor = Color.FromArgb(229, 232, 238)
    End Sub

    Public Sub StyleDangerButton(button As Button)
        StyleSecondaryButton(button)
        button.ForeColor = Danger
        button.FlatAppearance.BorderColor = Color.FromArgb(225, 184, 184)
    End Sub

    Public Sub SetStatusBadge(label As Label, text As String, installed As Boolean)
        label.AutoSize = False
        label.Text = text
        label.TextAlign = ContentAlignment.MiddleCenter
        label.Font = New Font("Segoe UI", 8.5F, FontStyle.Bold)
        label.Padding = New Padding(6, 0, 6, 0)
        label.BackColor = If(installed, SuccessBackground, NeutralBackground)
        label.ForeColor = If(installed, Success, TextSecondary)
        label.AccessibleRole = AccessibleRole.StaticText
        label.AccessibleName = $"Status: {text}"
        label.AccessibleDescription = text
    End Sub

    Public Sub EnableFocusCue(button As Button)
        If button Is Nothing OrElse FocusCueButtons.Contains(button) Then Return

        FocusCueButtons.Add(button)
        button.AccessibleRole = AccessibleRole.PushButton
        If String.IsNullOrWhiteSpace(button.AccessibleName) Then
            button.AccessibleName = button.Text.Replace("&", String.Empty).Replace(ChrW(&H25BE), String.Empty).Trim()
        End If
        AddHandler button.Enter, AddressOf FocusCueButton_Changed
        AddHandler button.Leave, AddressOf FocusCueButton_Changed
        AddHandler button.Paint, AddressOf FocusCueButton_Paint
        AddHandler button.Disposed, AddressOf FocusCueButton_Disposed
    End Sub

    Private Sub FocusCueButton_Changed(sender As Object, e As EventArgs)
        TryCast(sender, Button)?.Invalidate()
    End Sub

    Private Sub FocusCueButton_Paint(sender As Object, e As PaintEventArgs)
        Dim button = TryCast(sender, Button)
        If button Is Nothing OrElse Not button.Focused OrElse Not button.Enabled Then Return

        Dim focusColor = If(SystemInformation.HighContrast, SystemColors.Highlight, Accent)
        Dim focusBounds = Rectangle.Inflate(button.ClientRectangle, -3, -3)
        If focusBounds.Width <= 0 OrElse focusBounds.Height <= 0 Then Return

        Using focusPen As New Pen(focusColor, 2.0F) With {.DashStyle = DashStyle.Dot}
            e.Graphics.DrawRectangle(focusPen, focusBounds.Left, focusBounds.Top, focusBounds.Width - 1, focusBounds.Height - 1)
        End Using
    End Sub

    Private Sub FocusCueButton_Disposed(sender As Object, e As EventArgs)
        Dim button = TryCast(sender, Button)
        If button Is Nothing Then Return
        FocusCueButtons.Remove(button)
    End Sub
End Module
