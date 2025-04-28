<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SaveDataManagerForm
    Inherits ReaLTaiizor.Forms.MaterialForm

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
        components = New ComponentModel.Container()
        GroupBox1 = New GroupBox()
        lbxInstalledAppli = New ListBox()
        btnBackup = New Button()
        btnRestoreBackup = New Button()
        GroupBox2 = New GroupBox()
        lbxBackupSaves = New ListBox()
        ContextMenuStrip1 = New ContextMenuStrip(components)
        DeleteToolStripMenuItem = New ToolStripMenuItem()
        GroupBox1.SuspendLayout()
        GroupBox2.SuspendLayout()
        ContextMenuStrip1.SuspendLayout()
        SuspendLayout()
        ' 
        ' GroupBox1
        ' 
        GroupBox1.Controls.Add(lbxInstalledAppli)
        GroupBox1.Location = New Point(6, 67)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Size = New Size(404, 467)
        GroupBox1.TabIndex = 0
        GroupBox1.TabStop = False
        GroupBox1.Text = "Installed Appli"
        ' 
        ' lbxInstalledAppli
        ' 
        lbxInstalledAppli.Dock = DockStyle.Fill
        lbxInstalledAppli.FormattingEnabled = True
        lbxInstalledAppli.ItemHeight = 15
        lbxInstalledAppli.Location = New Point(3, 19)
        lbxInstalledAppli.Name = "lbxInstalledAppli"
        lbxInstalledAppli.Size = New Size(398, 445)
        lbxInstalledAppli.TabIndex = 0
        ' 
        ' btnBackup
        ' 
        btnBackup.Location = New Point(416, 207)
        btnBackup.Name = "btnBackup"
        btnBackup.Size = New Size(111, 56)
        btnBackup.TabIndex = 1
        btnBackup.Text = "Backup -->"
        btnBackup.UseVisualStyleBackColor = True
        ' 
        ' btnRestoreBackup
        ' 
        btnRestoreBackup.Location = New Point(416, 318)
        btnRestoreBackup.Name = "btnRestoreBackup"
        btnRestoreBackup.Size = New Size(111, 64)
        btnRestoreBackup.TabIndex = 2
        btnRestoreBackup.Text = "<-- Restore"
        btnRestoreBackup.UseVisualStyleBackColor = True
        ' 
        ' GroupBox2
        ' 
        GroupBox2.Controls.Add(lbxBackupSaves)
        GroupBox2.Location = New Point(533, 67)
        GroupBox2.Name = "GroupBox2"
        GroupBox2.Size = New Size(460, 464)
        GroupBox2.TabIndex = 3
        GroupBox2.TabStop = False
        GroupBox2.Text = "Backup Saves"
        ' 
        ' lbxBackupSaves
        ' 
        lbxBackupSaves.ContextMenuStrip = ContextMenuStrip1
        lbxBackupSaves.Dock = DockStyle.Fill
        lbxBackupSaves.FormattingEnabled = True
        lbxBackupSaves.ItemHeight = 15
        lbxBackupSaves.Location = New Point(3, 19)
        lbxBackupSaves.Name = "lbxBackupSaves"
        lbxBackupSaves.Size = New Size(454, 442)
        lbxBackupSaves.TabIndex = 0
        ' 
        ' ContextMenuStrip1
        ' 
        ContextMenuStrip1.Items.AddRange(New ToolStripItem() {DeleteToolStripMenuItem})
        ContextMenuStrip1.Name = "ContextMenuStrip1"
        ContextMenuStrip1.Size = New Size(108, 26)
        ' 
        ' DeleteToolStripMenuItem
        ' 
        DeleteToolStripMenuItem.Name = "DeleteToolStripMenuItem"
        DeleteToolStripMenuItem.Size = New Size(107, 22)
        DeleteToolStripMenuItem.Text = "Delete"
        ' 
        ' SaveDataManagerForm
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(999, 540)
        Controls.Add(GroupBox2)
        Controls.Add(btnRestoreBackup)
        Controls.Add(btnBackup)
        Controls.Add(GroupBox1)
        MaximizeBox = False
        Name = "SaveDataManagerForm"
        Sizable = False
        StartPosition = FormStartPosition.CenterParent
        Text = "Save Data Manager"
        GroupBox1.ResumeLayout(False)
        GroupBox2.ResumeLayout(False)
        ContextMenuStrip1.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents lbxInstalledAppli As ListBox
    Friend WithEvents btnBackup As Button
    Friend WithEvents btnRestoreBackup As Button
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents lbxBackupSaves As ListBox
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents DeleteToolStripMenuItem As ToolStripMenuItem
End Class
