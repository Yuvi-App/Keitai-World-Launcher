Imports System.IO

Public Class SaveDataManagerForm
    Dim DownloadFolder = Path.Combine("data", "downloads")
    Dim BackupFolder = Path.Combine("data", "sp_backups")
    Private Sub SaveDataManager_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        lbxInstalledAppli.Items.Clear()
        For Each F In Directory.GetDirectories(DownloadFolder)
            lbxInstalledAppli.Items.Add(Path.GetFileName(F))
        Next
    End Sub
    Private Sub lbxInstalledAppli_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lbxInstalledAppli.SelectedIndexChanged
        LoadSaves()
    End Sub
    Private Async Sub BtnBackup_Click(sender As Object, e As EventArgs) Handles btnBackup.Click
        Try
            ' Ensure the backup folder exists
            If Not Directory.Exists(BackupFolder) Then
                Directory.CreateDirectory(BackupFolder)
            End If

            If lbxInstalledAppli.SelectedIndex = -1 Then
                MessageBox.Show("Please select a game to backup.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            ' Get the selected game name
            Dim selectedGameFolder As String = lbxInstalledAppli.SelectedItem.ToString()
            Dim fullGameFolderPath As String = Path.Combine(DownloadFolder, selectedGameFolder)

            If Not Directory.Exists(fullGameFolderPath) Then
                MessageBox.Show($"Selected game folder does not exist: {fullGameFolderPath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            ' Find all .sp and .rms files recursively
            Dim saveFiles = Directory.GetFiles(fullGameFolderPath, "*.*", SearchOption.AllDirectories).
                                       Where(Function(f) f.EndsWith(".sp", StringComparison.OrdinalIgnoreCase) OrElse
                                                          f.EndsWith(".rms", StringComparison.OrdinalIgnoreCase)).
                                       ToList()

            If saveFiles.Count = 0 Then
                MessageBox.Show($"No .sp or .rms save files found in {selectedGameFolder}.", "Backup Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            For Each saveFile In saveFiles
                ' Figure out relative path from game folder
                Dim relativePath = Path.GetRelativePath(fullGameFolderPath, saveFile)
                Dim relativeFolderPath = Path.GetDirectoryName(relativePath)

                ' Build the target backup folder path
                Dim targetFolder = Path.Combine(BackupFolder, selectedGameFolder, relativeFolderPath)

                ' Create target folder if missing
                If Not Directory.Exists(targetFolder) Then
                    Directory.CreateDirectory(targetFolder)
                End If

                ' Build backup file name
                Dim baseFileName = Path.GetFileNameWithoutExtension(saveFile)
                Dim extension = Path.GetExtension(saveFile)
                Dim backupFileName = $"{baseFileName}_{DateTime.Now:yyyyMMdd_HHmmss}{extension}"
                Dim backupFullPath = Path.Combine(targetFolder, backupFileName)

                ' Perform the backup
                Using sourceStream As FileStream = File.Open(saveFile, FileMode.Open, FileAccess.Read, FileShare.Read)
                    Using destinationStream As FileStream = File.Create(backupFullPath)
                        Await sourceStream.CopyToAsync(destinationStream)
                    End Using
                End Using
            Next

            MessageBox.Show($"Backup completed: {saveFiles.Count} file(s) backed up.", "Backup Successful", MessageBoxButtons.OK, MessageBoxIcon.Information)
            LoadSaves()
        Catch ex As Exception
            MessageBox.Show($"Error during backup: {ex.Message}", "Backup Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub LoadSaves()
        ' Clear the backups list
        lbxBackupSaves.Items.Clear()

        If lbxInstalledAppli.SelectedIndex = -1 Then
            Exit Sub
        End If

        ' Get the selected game name
        Dim selectedGameFolder As String = lbxInstalledAppli.SelectedItem.ToString()

        ' Build the backup folder path for the selected game
        Dim gameBackupFolder As String = Path.Combine(BackupFolder, selectedGameFolder)

        ' Check if backup folder exists
        If Not Directory.Exists(gameBackupFolder) Then
            ' No backups for this game
            Exit Sub
        End If

        ' Get all .sp and .rms files recursively
        Dim backupFiles = Directory.GetFiles(gameBackupFolder, "*.*", SearchOption.AllDirectories).
                                   Where(Function(f) f.EndsWith(".sp", StringComparison.OrdinalIgnoreCase) OrElse
                                                      f.EndsWith(".rms", StringComparison.OrdinalIgnoreCase)).
                                   ToList()

        ' Add each file (relative path) to the listbox
        For Each filePath In backupFiles
            Dim relativePath As String = Path.GetRelativePath(gameBackupFolder, filePath)
            lbxBackupSaves.Items.Add(relativePath)
        Next
    End Sub
    Private Sub DeleteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteToolStripMenuItem.Click
        If lbxBackupSaves.SelectedIndex = -1 Then
            MessageBox.Show("Please select a backup save to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        If lbxInstalledAppli.SelectedIndex = -1 Then
            MessageBox.Show("Please select a game first.", "No Game Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim selectedGameFolder As String = lbxInstalledAppli.SelectedItem.ToString()
        Dim selectedBackupRelativePath As String = lbxBackupSaves.SelectedItem.ToString()
        Dim fullBackupFilePath As String = Path.Combine(BackupFolder, selectedGameFolder, selectedBackupRelativePath)

        If Not File.Exists(fullBackupFilePath) Then
            MessageBox.Show("Backup file does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        Dim result = MessageBox.Show($"Are you sure you want to delete the backup file: {selectedBackupRelativePath}?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

        If result = DialogResult.Yes Then
            Try
                File.Delete(fullBackupFilePath)
                MessageBox.Show("Backup file deleted successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information)

                ' Refresh the backup list
                lbxBackupSaves.Items.RemoveAt(lbxBackupSaves.SelectedIndex)

            Catch ex As Exception
                MessageBox.Show($"Error deleting file: {ex.Message}", "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub
    Private Function btnRestoreBackup_ClickAsync(sender As Object, e As EventArgs) As Task Handles btnRestoreBackup.Click
        RestoreSelectedBackup()
    End Function
    Private Async Sub RestoreSelectedBackup()
        If lbxBackupSaves.SelectedIndex = -1 Then
            MessageBox.Show("Please select a backup save to restore.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        If lbxInstalledAppli.SelectedIndex = -1 Then
            MessageBox.Show("Please select a game first.", "No Game Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim selectedGameFolder As String = lbxInstalledAppli.SelectedItem.ToString()
        Dim selectedBackupRelativePath As String = lbxBackupSaves.SelectedItem.ToString()

        Dim fullBackupFilePath As String = Path.Combine(BackupFolder, selectedGameFolder, selectedBackupRelativePath)
        Dim fullGameFolderPath As String = Path.Combine(DownloadFolder, selectedGameFolder)

        If Not File.Exists(fullBackupFilePath) Then
            MessageBox.Show("Backup file does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        ' Figure out target folder (based on relative path)
        Dim relativeFolder = Path.GetDirectoryName(selectedBackupRelativePath)

        ' Get the file extension of the backup (.sp or .rms)
        Dim backupExtension As String = Path.GetExtension(fullBackupFilePath)

        ' Build destination full path: [GameFolder]\[relativeFolder]\[GameFolderName].[ext]
        Dim destinationFolder As String = Path.Combine(fullGameFolderPath, relativeFolder)
        Dim destinationFileName As String = selectedGameFolder & backupExtension
        Dim destinationFullPath As String = Path.Combine(destinationFolder, destinationFileName)

        ' Confirm with the user
        Dim result = MessageBox.Show($"Are you sure you want to restore and overwrite: {destinationFileName}?", "Confirm Restore", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

        If result = DialogResult.Yes Then
            Try
                ' Ensure target folder exists (in case something weird happens)
                If Not Directory.Exists(destinationFolder) Then
                    Directory.CreateDirectory(destinationFolder)
                End If

                ' Perform the restore (overwrite the original)
                Using sourceStream As FileStream = File.Open(fullBackupFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)
                    Using destinationStream As FileStream = File.Create(destinationFullPath)
                        Await sourceStream.CopyToAsync(destinationStream)
                    End Using
                End Using

                MessageBox.Show("Restore completed successfully.", "Restore Successful", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Catch ex As Exception
                MessageBox.Show($"Error restoring file: {ex.Message}", "Restore Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

End Class