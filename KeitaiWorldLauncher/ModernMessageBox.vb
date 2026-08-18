Imports System.Windows.Forms

Public NotInheritable Class MessageBox
    Private Sub New()
    End Sub

    Public Shared Function Show(text As String) As DialogResult
        Return ShowInternal(Nothing, text, "Keitai World Launcher", MessageBoxButtons.OK, MessageBoxIcon.None)
    End Function

    Public Shared Function Show(text As String, caption As String) As DialogResult
        Return ShowInternal(Nothing, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None)
    End Function

    Public Shared Function Show(text As String, caption As String, buttons As MessageBoxButtons) As DialogResult
        Return ShowInternal(Nothing, text, caption, buttons, MessageBoxIcon.None)
    End Function

    Public Shared Function Show(
        text As String,
        caption As String,
        buttons As MessageBoxButtons,
        icon As MessageBoxIcon
    ) As DialogResult
        Return ShowInternal(Nothing, text, caption, buttons, icon)
    End Function

    Public Shared Function Show(
        text As String,
        caption As String,
        buttons As MessageBoxButtons,
        icon As MessageBoxIcon,
        defaultButton As MessageBoxDefaultButton
    ) As DialogResult
        Return ShowInternal(Nothing, text, caption, buttons, icon, defaultButton)
    End Function

    Public Shared Function Show(owner As IWin32Window, text As String) As DialogResult
        Return ShowInternal(owner, text, "Keitai World Launcher", MessageBoxButtons.OK, MessageBoxIcon.None)
    End Function

    Public Shared Function Show(owner As IWin32Window, text As String, caption As String) As DialogResult
        Return ShowInternal(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None)
    End Function

    Public Shared Function Show(
        owner As IWin32Window,
        text As String,
        caption As String,
        buttons As MessageBoxButtons
    ) As DialogResult
        Return ShowInternal(owner, text, caption, buttons, MessageBoxIcon.None)
    End Function

    Public Shared Function Show(
        owner As IWin32Window,
        text As String,
        caption As String,
        buttons As MessageBoxButtons,
        icon As MessageBoxIcon,
        Optional defaultButton As MessageBoxDefaultButton = MessageBoxDefaultButton.Button1
    ) As DialogResult
        Return ShowInternal(owner, text, caption, buttons, icon, defaultButton)
    End Function

    Private Shared Function ShowInternal(
        owner As IWin32Window,
        text As String,
        caption As String,
        buttons As MessageBoxButtons,
        icon As MessageBoxIcon,
        Optional defaultButton As MessageBoxDefaultButton = MessageBoxDefaultButton.Button1
    ) As DialogResult
        Dim ownerForm = ResolveOwner(owner)
        Dim dialogs As New UIDialogManager()
        Dim dialogTitle = If(String.IsNullOrWhiteSpace(caption), "Keitai World Launcher", caption)
        Dim tone = ToneFromIcon(icon)

        Select Case buttons
            Case MessageBoxButtons.YesNo
                Return dialogs.ShowConfirmation(
                    ownerForm,
                    dialogTitle,
                    text,
                    "Yes",
                    "No",
                    tone,
                    defaultButton = MessageBoxDefaultButton.Button2)
            Case MessageBoxButtons.OKCancel
                Dim result = dialogs.ShowConfirmation(
                    ownerForm,
                    dialogTitle,
                    text,
                    "Continue",
                    "Cancel",
                    tone,
                    defaultButton = MessageBoxDefaultButton.Button2)
                Return If(result = DialogResult.Yes, DialogResult.OK, DialogResult.Cancel)
            Case Else
                Return dialogs.ShowNotice(ownerForm, dialogTitle, text, "OK", tone)
        End Select
    End Function

    Private Shared Function ResolveOwner(owner As IWin32Window) As Form
        If TypeOf owner Is Form Then Return DirectCast(owner, Form)
        If owner IsNot Nothing AndAlso owner.Handle <> IntPtr.Zero Then
            Dim ownerControl = Control.FromHandle(owner.Handle)
            If ownerControl IsNot Nothing Then Return ownerControl.FindForm()
        End If

        If Form.ActiveForm IsNot Nothing Then Return Form.ActiveForm
        For Each openForm As Form In Application.OpenForms
            If openForm.Visible AndAlso Not openForm.IsDisposed Then Return openForm
        Next
        Return Nothing
    End Function

    Private Shared Function ToneFromIcon(icon As MessageBoxIcon) As CompactDialogTone
        Select Case icon
            Case MessageBoxIcon.Error
                Return CompactDialogTone.Error
            Case MessageBoxIcon.Warning
                Return CompactDialogTone.Warning
            Case MessageBoxIcon.Information
                Return CompactDialogTone.Information
            Case Else
                Return CompactDialogTone.Information
        End Select
    End Function
End Class
