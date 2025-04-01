<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SplashScreen
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        lblMessage = New Label()
        ProgressBar1 = New ProgressBar()
        Label1 = New Label()
        SuspendLayout()
        ' 
        ' lblMessage
        ' 
        lblMessage.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        lblMessage.ForeColor = Color.White
        lblMessage.Location = New Point(0, 164)
        lblMessage.Name = "lblMessage"
        lblMessage.Size = New Size(650, 36)
        lblMessage.TabIndex = 1
        lblMessage.Text = "Label1"
        lblMessage.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' ProgressBar1
        ' 
        ProgressBar1.Location = New Point(0, 124)
        ProgressBar1.Name = "ProgressBar1"
        ProgressBar1.Size = New Size(650, 37)
        ProgressBar1.Style = ProgressBarStyle.Continuous
        ProgressBar1.TabIndex = 2
        ' 
        ' Label1
        ' 
        Label1.Font = New Font("Segoe UI", 14F, FontStyle.Bold)
        Label1.Location = New Point(147, 50)
        Label1.Name = "Label1"
        Label1.Size = New Size(356, 60)
        Label1.TabIndex = 3
        Label1.Text = "Booting Keitai World Launcher"
        Label1.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' SplashScreen
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.Gray
        ClientSize = New Size(650, 250)
        Controls.Add(Label1)
        Controls.Add(ProgressBar1)
        Controls.Add(lblMessage)
        ForeColor = Color.White
        FormBorderStyle = FormBorderStyle.None
        Name = "SplashScreen"
        StartPosition = FormStartPosition.CenterScreen
        Text = "SplashScreen"
        TopMost = True
        ResumeLayout(False)
    End Sub

    Friend WithEvents lblMessage As Label
    Friend WithEvents ProgressBar1 As ProgressBar
    Friend WithEvents Label1 As Label
End Class
