Imports System.Net
Imports System.IO
Imports System.IO.Compression
Imports System.Threading
Imports KeitaiWorldLauncher.My.Managers
Imports KeitaiWorldLauncher.My.Models
Imports System.Security.Policy

Namespace My.Managers
    Public Class GameDownloader
        Dim utilManager As New UtilManager
        Private WithEvents client1 As WebClient
        Private WithEvents client2 As WebClient
        Private progressBar As ProgressBar
        Private downloadFilePath As String
        Private extractFolderPath As String
        Private selectedGameZipPath As String
        Private ReadJam As String
        Private Isthisbatchdownload As Boolean
        Private SelectedGame As Game
        Private SDCardDownloadFile As String

        Public Sub New(progressBarControl As ProgressBar)
            client1 = New WebClient()
            client2 = New WebClient()
            progressBar = progressBarControl
        End Sub

        Public Async Function DownloadGameAsync(url As String, savePath As String, extractTo As String, Inputgame As Game, JAMLocation As String, BatchDownload As Boolean) As Task
            Try
                ' Set paths for use in completion event
                downloadFilePath = savePath
                extractFolderPath = extractTo
                ReadJam = JAMLocation
                Isthisbatchdownload = BatchDownload
                SelectedGame = Inputgame

                ' Reset and show the progress bar
                progressBar.Value = 0
                progressBar.Visible = True

                ' Attach event handlers
                AddHandler client1.DownloadProgressChanged, AddressOf DownloadProgressChanged
                AddHandler client1.DownloadFileCompleted, AddressOf DownloadFileCompleted
                AddHandler client2.DownloadProgressChanged, AddressOf DownloadProgressChanged
                AddHandler client2.DownloadFileCompleted, AddressOf DownloadFileCompleted

                ' Start downloading the file asynchronously
                Await client1.DownloadFileTaskAsync(New Uri(url), savePath)
                If SelectedGame.SDCardDataURL <> "" Then
                    SDCardDownloadFile = $"data\downloads\{Path.GetFileName(SelectedGame.SDCardDataURL)}"
                    Await client2.DownloadFileTaskAsync(New Uri(SelectedGame.SDCardDataURL), SDCardDownloadFile)
                    'If SD Card Data Process it
                    Try
                        'MessageBox.Show($"{SelectedGame.ENTitle} Needs SD Card Data, we will attempt to set it up automatically. The EMU will launch itself to process the data.")
                        If SelectedGame.Emulator.ToLower = "doja" Then
                            ZipFile.ExtractToDirectory(SDCardDownloadFile, $"{Form1.Dojapath}\lib\storagedevice\ext0\SD_BIND", True)
                            File.Delete(SDCardDownloadFile)
                        ElseIf SelectedGame.Emulator.ToLower = "star" Then
                            ZipFile.ExtractToDirectory(SDCardDownloadFile, $"{Form1.Starpath}\lib\storagedevice\ext0\SD_BIND", True)
                            File.Delete(SDCardDownloadFile)
                        End If
                    Catch ex As Exception
                        MessageBox.Show($"Failed to Handle SD Card Data: {vbCrLf}{ex}")
                    End Try
                Else
                End If
            Catch ex As Exception
                MessageBox.Show($"Failed to start download: {ex.Message}", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                progressBar.Visible = False
            End Try
        End Function
        Private Sub DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs)
            ' Update the progress bar
            progressBar.Value = e.ProgressPercentage
        End Sub
        Private Sub DownloadFileCompleted(sender As Object, e As System.ComponentModel.AsyncCompletedEventArgs)
            ' Hide the progress bar
            progressBar.Visible = False

            If e.Error IsNot Nothing Then
                MessageBox.Show($"Failed to download the game: {e.Error.Message}", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                If Isthisbatchdownload = False Then
                    'MessageBox.Show("Game downloaded successfully!", "Download Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    ' Extract the downloaded file
                    Try
                        Thread.Sleep(100) ' Optional: Wait briefly to ensure the file is fully written
                        ZipFile.ExtractToDirectory(downloadFilePath, extractFolderPath, True)
                        'MessageBox.Show("Game extracted successfully!", "Extraction Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)

                        ' Delete the ZIP file after extraction
                        File.Delete(downloadFilePath)
                        UtilManager.GenerateDynamicControlsFromLines(ReadJam, Form1.gbxGameInfo)
                    Catch ex As Exception
                        MessageBox.Show($"Failed to extract the game: {ex.Message}", "Extraction Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try
                ElseIf Isthisbatchdownload = True Then
                    ' Extract the downloaded file
                    Try
                        Thread.Sleep(100) ' Optional: Wait briefly to ensure the file is fully written
                        ZipFile.ExtractToDirectory(downloadFilePath, extractFolderPath, True)
                        ' Delete the ZIP file after extraction
                        File.Delete(downloadFilePath)
                    Catch ex As Exception
                        MessageBox.Show($"Failed to extract the game: {ex.Message}", "Extraction Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try
                End If
                Form1.RefreshGameHighlighting()
            End If

            ' Detach event handlers to avoid memory leaks
            RemoveHandler client1.DownloadProgressChanged, AddressOf DownloadProgressChanged
            RemoveHandler client1.DownloadFileCompleted, AddressOf DownloadFileCompleted
            RemoveHandler client2.DownloadProgressChanged, AddressOf DownloadProgressChanged
            RemoveHandler client2.DownloadFileCompleted, AddressOf DownloadFileCompleted
        End Sub
    End Class
End Namespace
