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
        GroupBox2 = New GroupBox()
        ContextMenuStrip1 = New ContextMenuStrip(components)
        DeleteToolStripMenuItem = New ToolStripMenuItem()
        btnBackup = New ReaLTaiizor.Controls.MaterialButton()
        btnRestoreBackup = New ReaLTaiizor.Controls.MaterialButton()
        lbxBackupSaves = New ReaLTaiizor.Controls.MaterialListBox()
        lbxInstalledAppli = New ReaLTaiizor.Controls.MaterialListBox()
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
        ' btnBackup
        ' 
        btnBackup.AutoSize = False
        btnBackup.AutoSizeMode = AutoSizeMode.GrowAndShrink
        btnBackup.Density = ReaLTaiizor.Controls.MaterialButton.MaterialButtonDensity.Default
        btnBackup.Depth = 0
        btnBackup.HighEmphasis = True
        btnBackup.Icon = Nothing
        btnBackup.IconType = ReaLTaiizor.Controls.MaterialButton.MaterialIconType.Rebase
        btnBackup.Location = New Point(416, 200)
        btnBackup.Margin = New Padding(4, 6, 4, 6)
        btnBackup.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER
        btnBackup.Name = "btnBackup"
        btnBackup.NoAccentTextColor = Color.Empty
        btnBackup.Size = New Size(111, 53)
        btnBackup.TabIndex = 4
        btnBackup.Text = "Backup -->"
        btnBackup.Type = ReaLTaiizor.Controls.MaterialButton.MaterialButtonType.Contained
        btnBackup.UseAccentColor = False
        btnBackup.UseVisualStyleBackColor = True
        ' 
        ' btnRestoreBackup
        ' 
        btnRestoreBackup.AutoSize = False
        btnRestoreBackup.AutoSizeMode = AutoSizeMode.GrowAndShrink
        btnRestoreBackup.Density = ReaLTaiizor.Controls.MaterialButton.MaterialButtonDensity.Default
        btnRestoreBackup.Depth = 0
        btnRestoreBackup.HighEmphasis = True
        btnRestoreBackup.Icon = Nothing
        btnRestoreBackup.IconType = ReaLTaiizor.Controls.MaterialButton.MaterialIconType.Rebase
        btnRestoreBackup.Location = New Point(416, 345)
        btnRestoreBackup.Margin = New Padding(4, 6, 4, 6)
        btnRestoreBackup.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER
        btnRestoreBackup.Name = "btnRestoreBackup"
        btnRestoreBackup.NoAccentTextColor = Color.Empty
        btnRestoreBackup.Size = New Size(111, 53)
        btnRestoreBackup.TabIndex = 5
        btnRestoreBackup.Text = "<-- Restore"
        btnRestoreBackup.Type = ReaLTaiizor.Controls.MaterialButton.MaterialButtonType.Contained
        btnRestoreBackup.UseAccentColor = False
        btnRestoreBackup.UseVisualStyleBackColor = True
        ' 
        ' lbxBackupSaves
        ' 
        lbxBackupSaves.BackColor = Color.White
        lbxBackupSaves.BorderColor = Color.LightGray
        lbxBackupSaves.ContextMenuStrip = ContextMenuStrip1
        lbxBackupSaves.Depth = 0
        lbxBackupSaves.Dock = DockStyle.Fill
        lbxBackupSaves.Font = New Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel)
        lbxBackupSaves.Location = New Point(3, 19)
        lbxBackupSaves.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER
        lbxBackupSaves.Name = "lbxBackupSaves"
        lbxBackupSaves.SelectedIndex = -1
        lbxBackupSaves.SelectedItem = Nothing
        lbxBackupSaves.Size = New Size(454, 442)
        lbxBackupSaves.TabIndex = 0
        ' 
        ' lbxInstalledAppli
        ' 
        lbxInstalledAppli.BackColor = Color.White
        lbxInstalledAppli.BorderColor = Color.LightGray
        lbxInstalledAppli.ContextMenuStrip = ContextMenuStrip1
        lbxInstalledAppli.Depth = 0
        lbxInstalledAppli.Dock = DockStyle.Fill
        lbxInstalledAppli.Font = New Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel)
        lbxInstalledAppli.Location = New Point(3, 19)
        lbxInstalledAppli.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER
        lbxInstalledAppli.Name = "lbxInstalledAppli"
        lbxInstalledAppli.SelectedIndex = -1
        lbxInstalledAppli.SelectedItem = Nothing
        lbxInstalledAppli.Size = New Size(398, 445)
        lbxInstalledAppli.TabIndex = 0
        ' 
        ' SaveDataManagerForm
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(999, 540)
        Controls.Add(btnRestoreBackup)
        Controls.Add(btnBackup)
        Controls.Add(GroupBox2)
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
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents DeleteToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents btnBackup As ReaLTaiizor.Controls.MaterialButton
    Friend WithEvents btnRestoreBackup As ReaLTaiizor.Controls.MaterialButton
    Friend WithEvents lbxInstalledAppli As ReaLTaiizor.Controls.MaterialListBox
    Friend WithEvents lbxBackupSaves As ReaLTaiizor.Controls.MaterialListBox
End Class
