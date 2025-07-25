Imports System.IO

Namespace My.Managers
    Public Class SaveDataManager
        Private Shared ReadOnly BackupFolder As String = "data\sp_backups"

        ''' <summary>
        ''' Backs up the file for the given game to the backup folder.
        ''' </summary>
        Public Shared Async Function BackupSaveAsync(GameFolder As String, emulator As String) As Task
            Try
                ' Ensure the backup folder exists
                If Not Directory.Exists(BackupFolder) Then
                    Directory.CreateDirectory(BackupFolder)
                End If

                ' Normalize paths
                Dim gameFolderName As String = Path.GetFileName(GameFolder)
                Dim searchExtension As String

                Select Case emulator.ToLower()
                    Case "doja", "star"
                        searchExtension = ".sp"
                    Case "jsky"
                        searchExtension = ".rms"
                    Case Else
                        MessageBox.Show($"Unsupported emulator type for SaveDataBackup: {emulator}", "Backup Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                End Select

                ' Find all matching files inside GameFolder (recursively)
                Dim saveFiles = Directory.GetFiles(GameFolder, $"*{searchExtension}", SearchOption.AllDirectories)

                If saveFiles.Length = 0 Then
                    MessageBox.Show($"No {searchExtension} save files found in {GameFolder}", "Backup Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If

                For Each saveFile In saveFiles
                    ' Figure out relative path from GameFolder
                    Dim relativePath = Path.GetRelativePath(GameFolder, saveFile)

                    ' Remove extension from relative path
                    Dim relativeFolderPath = Path.GetDirectoryName(relativePath)

                    ' Target folder inside backup folder
                    Dim targetFolder = Path.Combine(BackupFolder, gameFolderName, relativeFolderPath)

                    ' Create target folder if missing
                    If Not Directory.Exists(targetFolder) Then
                        Directory.CreateDirectory(targetFolder)
                    End If

                    ' Build backup file name
                    Dim baseFileName = Path.GetFileNameWithoutExtension(saveFile)
                    Dim backupFileName = $"{baseFileName}_{DateTime.Now:yyyyMMdd_HHmmss}{searchExtension}"
                    Dim backupFullPath = Path.Combine(targetFolder, backupFileName)

                    ' Perform the backup
                    Using sourceStream As FileStream = File.Open(saveFile, FileMode.Open, FileAccess.Read, FileShare.Read)
                        Using destinationStream As FileStream = File.Create(backupFullPath)
                            Await sourceStream.CopyToAsync(destinationStream)
                        End Using
                    End Using
                Next
                MessageBox.Show($"Backup completed: {saveFiles.Length} file(s) backed up.", "Backup Successful", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Catch ex As Exception
                MessageBox.Show($"Error during backup: {ex.Message}", "Backup Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Function
    End Class

End Namespace