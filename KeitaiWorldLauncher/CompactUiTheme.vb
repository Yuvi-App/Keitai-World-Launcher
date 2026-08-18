Imports System.Drawing
Imports System.Windows.Forms

Public Module CompactUiTheme
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
    End Sub
End Module
