Imports System.Drawing
Imports System.Runtime.InteropServices
Imports System.Windows.Forms

' KWL uses the designer's 96-DPI coordinates as physical pixels. PerMonitor
' awareness prevents Windows bitmap stretching; these forms deliberately opt
' out of WinForms resizing when the monitor DPI changes.
Public Module FixedLayout
    Public Function CreateFont(family As String, pointsAt96Dpi As Single, Optional style As FontStyle = FontStyle.Regular) As Font
        Return New Font(family, pointsAt96Dpi * 96.0F / 72.0F, style, GraphicsUnit.Pixel)
    End Function

    Friend Sub Configure(window As Form)
        window.AutoScaleMode = AutoScaleMode.None
        window.Font = CreateFont("Segoe UI", 9.0F)
        window.MaximizeBox = False
        window.SizeGripStyle = SizeGripStyle.Hide
        AddHandler window.DpiChanged, AddressOf PreservePixelLayout
    End Sub

    Private Sub PreservePixelLayout(sender As Object, e As DpiChangedEventArgs)
        ' AutoScaleMode.None alone does not cancel the suggested window bounds
        ' or the font scaling performed by Form.OnDpiChanged.
        e.Cancel = True
    End Sub
End Module

Public Class FixedLayoutForm
    Inherits Form

    Public Sub New()
        FixedLayout.Configure(Me)
        FormBorderStyle = FormBorderStyle.FixedDialog
    End Sub
End Class

Public Class FixedMaterialForm
    Inherits ReaLTaiizor.Forms.MaterialForm

    Private Const GwlStyle As Integer = -16
    Private Const WsThickFrame As Integer = &H40000
    Private Const WsMaximizeBox As Integer = &H10000
    Private Const WmSysCommand As Integer = &H112
    Private Const ScSize As Integer = &HF000
    Private Const ScMaximize As Integer = &HF030

    Public Sub New()
        FixedLayout.Configure(Me)
        Sizable = False
    End Sub

    Protected Overrides Sub OnCreateControl()
        MyBase.OnCreateControl()

        ' MaterialForm adds WS_THICKFRAME here even when Sizable is False.
        ' Remove it after the theme runs so Windows cannot resize/snap the form.
        Dim style = GetWindowLong(Handle, GwlStyle)
        SetWindowLong(Handle, GwlStyle, style And Not (WsThickFrame Or WsMaximizeBox))
        Const preserveBoundsAndActivateFrame As UInteger = &H1UI Or &H2UI Or &H4UI Or &H10UI Or &H20UI
        SetWindowPos(Handle, IntPtr.Zero, 0, 0, 0, 0, preserveBoundsAndActivateFrame)
    End Sub

    Protected Overrides Sub WndProc(ByRef m As Message)
        If m.Msg = WmSysCommand Then
            Dim command = CInt(m.WParam.ToInt64() And &HFFF0L)
            If command = ScSize OrElse command = ScMaximize Then
                m.Result = IntPtr.Zero
                Return
            End If
        End If
        MyBase.WndProc(m)
    End Sub

    <DllImport("user32.dll", EntryPoint:="GetWindowLongW")>
    Private Shared Function GetWindowLong(window As IntPtr, index As Integer) As Integer
    End Function

    <DllImport("user32.dll", EntryPoint:="SetWindowLongW")>
    Private Shared Function SetWindowLong(window As IntPtr, index As Integer, value As Integer) As Integer
    End Function

    <DllImport("user32.dll")>
    Private Shared Function SetWindowPos(window As IntPtr, insertAfter As IntPtr, x As Integer, y As Integer, width As Integer, height As Integer, flags As UInteger) As Boolean
    End Function
End Class

Public Class FixedPoisonForm
    Inherits ReaLTaiizor.Forms.PoisonForm

    Public Sub New()
        FixedLayout.Configure(Me)
        Resizable = False
    End Sub
End Class
