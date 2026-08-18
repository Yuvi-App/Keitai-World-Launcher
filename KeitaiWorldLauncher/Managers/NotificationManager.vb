Imports System.Drawing
Imports System.Windows.Forms

Public NotInheritable Class NotificationManager
    Private Shared ReadOnly PendingNotifications As New Queue(Of NotificationRequest)()
    Private Shared _activeNotification As CompactNotificationForm

    Private Sub New()
    End Sub

    Public Shared Sub ShowInformation(owner As Form, title As String, message As String)
        Show(owner, title, message, CompactDialogTone.Information)
    End Sub

    Public Shared Sub ShowSuccess(owner As Form, title As String, message As String)
        Show(owner, title, message, CompactDialogTone.Success)
    End Sub

    Public Shared Sub ShowWarning(owner As Form, title As String, message As String)
        Show(owner, title, message, CompactDialogTone.Warning)
    End Sub

    Public Shared Sub ShowFailure(owner As Form, title As String, message As String)
        Show(owner, title, message, CompactDialogTone.Error)
    End Sub

    Public Shared Sub Show(
        owner As Form,
        title As String,
        message As String,
        Optional tone As CompactDialogTone = CompactDialogTone.Information,
        Optional durationMilliseconds As Integer = 0
    )
        Dim resolvedOwner = ResolveOwner(owner)
        If resolvedOwner IsNot Nothing AndAlso resolvedOwner.InvokeRequired Then
            resolvedOwner.BeginInvoke(
                New Action(Sub() Show(resolvedOwner, title, message, tone, durationMilliseconds)))
            Return
        End If

        Dim duration = durationMilliseconds
        If duration <= 0 Then
            duration = If(tone = CompactDialogTone.Error OrElse tone = CompactDialogTone.Danger, 6500, 4500)
        End If

        PendingNotifications.Enqueue(New NotificationRequest With {
            .Owner = resolvedOwner,
            .Title = If(title, String.Empty),
            .Message = If(message, String.Empty),
            .Tone = tone,
            .DurationMilliseconds = duration
        })
        ShowNextNotification()
    End Sub

    Private Shared Sub ShowNextNotification()
        If _activeNotification IsNot Nothing OrElse PendingNotifications.Count = 0 Then Return

        Dim request = PendingNotifications.Dequeue()
        Dim owner = ResolveOwner(request.Owner)
        Dim notification As New CompactNotificationForm(
            request.Title,
            request.Message,
            request.Tone,
            request.DurationMilliseconds)

        _activeNotification = notification
        AddHandler notification.FormClosed,
            Sub()
                If ReferenceEquals(_activeNotification, notification) Then _activeNotification = Nothing
                ShowNextNotification()
            End Sub

        If owner Is Nothing Then
            notification.Show()
        Else
            notification.Show(owner)
        End If
    End Sub

    Private Shared Function ResolveOwner(preferredOwner As Form) As Form
        If preferredOwner IsNot Nothing AndAlso Not preferredOwner.IsDisposed Then Return preferredOwner
        If Form.ActiveForm IsNot Nothing AndAlso Not Form.ActiveForm.IsDisposed Then Return Form.ActiveForm

        For Each openForm As Form In Application.OpenForms
            If openForm.Visible AndAlso Not openForm.IsDisposed Then Return openForm
        Next
        Return Nothing
    End Function

    Private NotInheritable Class NotificationRequest
        Public Property Owner As Form
        Public Property Title As String
        Public Property Message As String
        Public Property Tone As CompactDialogTone
        Public Property DurationMilliseconds As Integer
    End Class

    Private NotInheritable Class CompactNotificationForm
        Inherits Form

        Private Const NotificationWidth As Integer = 410
        Private ReadOnly _durationMilliseconds As Integer
        Private ReadOnly _toneColor As Color
        Private ReadOnly _dismissTimer As Timer
        Private _elapsedMilliseconds As Integer

        Public Sub New(title As String, message As String, tone As CompactDialogTone, durationMilliseconds As Integer)
            _durationMilliseconds = Math.Max(1000, durationMilliseconds)
            _toneColor = ToneColor(tone)

            Dim messageFont As New Font("Segoe UI", 9.0F)
            Dim messageSize = TextRenderer.MeasureText(
                message,
                messageFont,
                New Size(300, 0),
                TextFormatFlags.WordBreak Or TextFormatFlags.TextBoxControl)
            Dim bodyHeight = Math.Max(38, Math.Min(76, messageSize.Height + 4))
            Dim notificationHeight = Math.Max(104, 52 + bodyHeight)

            AccessibleName = $"{ToneName(tone)} notification"
            AccessibleRole = AccessibleRole.Alert
            AutoScaleMode = AutoScaleMode.Dpi
            BackColor = CompactUiTheme.Surface
            ClientSize = New Size(NotificationWidth, notificationHeight)
            ControlBox = False
            Font = New Font("Segoe UI", 9.0F)
            FormBorderStyle = FormBorderStyle.None
            MaximizeBox = False
            MinimizeBox = False
            Padding = New Padding(1)
            ShowIcon = False
            ShowInTaskbar = False
            StartPosition = FormStartPosition.Manual
            Text = If(String.IsNullOrWhiteSpace(title), ToneName(tone), title)

            Dim accentBar As New Panel With {
                .BackColor = _toneColor,
                .Dock = DockStyle.Left,
                .Width = 4
            }
            Dim iconSurface As New Panel With {
                .BackColor = ToneBackground(tone),
                .Location = New Point(19, 18),
                .Size = New Size(40, 40)
            }
            Dim iconLabel As New Label With {
                .Dock = DockStyle.Fill,
                .Font = New Font("Segoe UI Semibold", 14.0F, FontStyle.Bold),
                .ForeColor = _toneColor,
                .Text = ToneGlyph(tone),
                .TextAlign = ContentAlignment.MiddleCenter
            }
            iconSurface.Controls.Add(iconLabel)

            Dim titleLabel As New Label With {
                .AutoEllipsis = True,
                .Font = New Font("Segoe UI Semibold", 10.0F, FontStyle.Bold),
                .ForeColor = CompactUiTheme.TextPrimary,
                .Location = New Point(74, 14),
                .Size = New Size(NotificationWidth - 120, 25),
                .Text = If(String.IsNullOrWhiteSpace(title), ToneName(tone), title),
                .TextAlign = ContentAlignment.MiddleLeft,
                .UseMnemonic = False
            }
            Dim messageLabel As New Label With {
                .AutoEllipsis = True,
                .Font = messageFont,
                .ForeColor = CompactUiTheme.TextSecondary,
                .Location = New Point(74, 40),
                .Size = New Size(NotificationWidth - 96, bodyHeight),
                .Text = message,
                .TextAlign = ContentAlignment.TopLeft,
                .UseMnemonic = False
            }
            Dim closeButton As New Button With {
                .AccessibleName = "Dismiss notification",
                .BackColor = CompactUiTheme.Surface,
                .FlatStyle = FlatStyle.Flat,
                .Font = New Font("Segoe UI", 12.0F),
                .ForeColor = CompactUiTheme.TextSecondary,
                .Location = New Point(NotificationWidth - 40, 8),
                .Size = New Size(28, 28),
                .TabStop = False,
                .Text = ChrW(&HD7),
                .UseVisualStyleBackColor = False
            }
            closeButton.FlatAppearance.BorderSize = 0
            closeButton.FlatAppearance.MouseOverBackColor = CompactUiTheme.NeutralBackground
            closeButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(229, 232, 238)

            Controls.AddRange(New Control() {messageLabel, titleLabel, iconSurface, closeButton, accentBar})
            AddHandler closeButton.Click, Sub() Close()

            _dismissTimer = New Timer With {.Interval = 50}
            AddHandler _dismissTimer.Tick, AddressOf DismissTimer_Tick
        End Sub

        Protected Overrides ReadOnly Property ShowWithoutActivation As Boolean
            Get
                Return True
            End Get
        End Property

        Protected Overrides ReadOnly Property CreateParams As CreateParams
            Get
                Const WsExNoActivate As Integer = &H8000000
                Dim parameters = MyBase.CreateParams
                parameters.ExStyle = parameters.ExStyle Or WsExNoActivate
                Return parameters
            End Get
        End Property

        Protected Overrides Sub OnShown(e As EventArgs)
            MyBase.OnShown(e)
            PositionNearOwner()
            _dismissTimer.Start()
        End Sub

        Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)
            _dismissTimer.Stop()
            _dismissTimer.Dispose()
            MyBase.OnFormClosed(e)
        End Sub

        Protected Overrides Sub OnPaint(e As PaintEventArgs)
            MyBase.OnPaint(e)
            Using borderPen As New Pen(CompactUiTheme.Border)
                e.Graphics.DrawRectangle(borderPen, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1)
            End Using

            Dim remainingRatio = Math.Max(0.0R, 1.0R - (_elapsedMilliseconds / CDbl(_durationMilliseconds)))
            Dim progressWidth = CInt((ClientSize.Width - 2) * remainingRatio)
            If progressWidth > 0 Then
                Using progressBrush As New SolidBrush(_toneColor)
                    e.Graphics.FillRectangle(progressBrush, 1, ClientSize.Height - 3, progressWidth, 2)
                End Using
            End If
        End Sub

        Private Sub DismissTimer_Tick(sender As Object, e As EventArgs)
            If Not Bounds.Contains(Cursor.Position) Then _elapsedMilliseconds += _dismissTimer.Interval
            If _elapsedMilliseconds >= _durationMilliseconds Then
                Close()
                Return
            End If
            Invalidate(New Rectangle(0, ClientSize.Height - 4, ClientSize.Width, 4))
        End Sub

        Private Sub PositionNearOwner()
            Dim workingArea As Rectangle
            Dim anchorBounds As Rectangle
            If Owner IsNot Nothing AndAlso Owner.Visible AndAlso Not Owner.IsDisposed Then
                workingArea = Screen.FromControl(Owner).WorkingArea
                anchorBounds = Owner.Bounds
            Else
                workingArea = Screen.FromPoint(Cursor.Position).WorkingArea
                anchorBounds = workingArea
            End If

            Dim targetX = anchorBounds.Right - Width - 18
            Dim targetY = anchorBounds.Bottom - Height - 18
            targetX = Math.Max(workingArea.Left + 12, Math.Min(targetX, workingArea.Right - Width - 12))
            targetY = Math.Max(workingArea.Top + 12, Math.Min(targetY, workingArea.Bottom - Height - 12))
            Location = New Point(targetX, targetY)
        End Sub

        Private Shared Function ToneColor(tone As CompactDialogTone) As Color
            Select Case tone
                Case CompactDialogTone.Success
                    Return CompactUiTheme.Success
                Case CompactDialogTone.Warning
                    Return CompactUiTheme.Accent
                Case CompactDialogTone.Error, CompactDialogTone.Danger
                    Return CompactUiTheme.Danger
                Case Else
                    Return CompactUiTheme.Primary
            End Select
        End Function

        Private Shared Function ToneBackground(tone As CompactDialogTone) As Color
            Select Case tone
                Case CompactDialogTone.Success
                    Return CompactUiTheme.SuccessBackground
                Case CompactDialogTone.Warning
                    Return Color.FromArgb(255, 244, 226)
                Case CompactDialogTone.Error, CompactDialogTone.Danger
                    Return Color.FromArgb(252, 235, 235)
                Case Else
                    Return Color.FromArgb(235, 239, 252)
            End Select
        End Function

        Private Shared Function ToneGlyph(tone As CompactDialogTone) As String
            Select Case tone
                Case CompactDialogTone.Success
                    Return ChrW(&H2713)
                Case CompactDialogTone.Warning, CompactDialogTone.Error, CompactDialogTone.Danger
                    Return "!"
                Case Else
                    Return "i"
            End Select
        End Function

        Private Shared Function ToneName(tone As CompactDialogTone) As String
            Select Case tone
                Case CompactDialogTone.Success
                    Return "Success"
                Case CompactDialogTone.Warning
                    Return "Warning"
                Case CompactDialogTone.Error, CompactDialogTone.Danger
                    Return "Something went wrong"
                Case Else
                    Return "Information"
            End Select
        End Function
    End Class
End Class
