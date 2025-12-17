Imports ReaLTaiizor.Controls
Imports ReaLTaiizor.Forms.MaterialForm
Public Class UIDialogManager

    Public Function ShowMaterialDialogOk(
        ownerForm As Form,
        title As String,
        message As String,
        Optional okText As String = "OK",
        Optional useAccent As Boolean = True
    ) As DialogResult
        Using dlg As New MaterialDialog(ownerForm, title, message, okText, False, "Cancel", useAccent)
            Return dlg.ShowDialog(ownerForm)
        End Using
    End Function

    Public Function ShowMaterialDialogYesNo(
        ownerForm As Form,
        title As String,
        message As String,
        Optional yesText As String = "YES",
        Optional noText As String = "NO",
        Optional useAccent As Boolean = True
    ) As DialogResult
        Using dlg As New MaterialDialog(ownerForm, title, message, yesText, True, noText, useAccent)
            Dim res = dlg.ShowDialog(ownerForm)
            Return If(res = DialogResult.OK, DialogResult.Yes, DialogResult.No)
        End Using
    End Function

    Public Sub ShowMaterialError(owner As Form, message As String)
        Using dlg As New MaterialDialog(
        owner,
        "Error",
        message,
        "OK",
        False,
        "",
        True
    )
            dlg.ShowDialog(owner)
        End Using
    End Sub

End Class
