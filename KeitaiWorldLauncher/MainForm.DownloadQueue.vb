Imports System.IO
Imports KeitaiWorldLauncher.My.logger
Imports KeitaiWorldLauncher.My.Managers
Imports KeitaiWorldLauncher.My.Models

Partial Public Class MainForm
    Private Enum LibraryDownloadState
        Queued
        Downloading
        Extracting
        Installing
        Failed
    End Enum

    Private NotInheritable Class LibraryDownloadStatus
        Public Property State As LibraryDownloadState
        Public Property Percentage As Integer = -1
        Public Property StatusText As String = String.Empty
    End Class

    Private NotInheritable Class QueuedLibraryDownload
        Public Property Key As String
        Public Property Title As String
        Public Property Operation As Func(Of IProgress(Of DownloadProgressInfo), Task)
        Public Property Completed As Action(Of Boolean, String)
    End Class

    Private NotInheritable Class QueuedGameDownload
        Public Property Game As Game
        Public Property Key As String
        Public Property ZipPath As String
        Public Property ExtractFolder As String
        Public Property JamPath As String
        Public Property JarPath As String
        Public Property IsRedownload As Boolean
    End Class

    Private ReadOnly _libraryDownloadQueue As New Queue(Of QueuedLibraryDownload)()
    Private ReadOnly _libraryDownloadStates As New Dictionary(Of String, LibraryDownloadStatus)(StringComparer.OrdinalIgnoreCase)
    Private _activeLibraryDownload As QueuedLibraryDownload
    Private _downloadQueueProcessorRunning As Boolean

    Private Shared Function GameDownloadQueueKey(game As Game) As String
        If game Is Nothing OrElse String.IsNullOrWhiteSpace(game.ZIPName) Then Return String.Empty
        Return $"game:{Path.GetFileNameWithoutExtension(game.ZIPName)}_{game.Emulator}"
    End Function

    Private Shared Function MachiDownloadQueueKey(item As MachiChara) As String
        Return If(item Is Nothing, String.Empty, $"machi:{item.CFDName}")
    End Function

    Private Shared Function CharaDownloadQueueKey(item As CharaDen) As String
        Return If(item Is Nothing, String.Empty, $"chara:{item.AFDName}")
    End Function

    Private Function IsGameDownloadBusy(game As Game) As Boolean
        Return IsLibraryDownloadBusy(GameDownloadQueueKey(game))
    End Function

    Private Function IsMachiDownloadBusy(item As MachiChara) As Boolean
        Return IsLibraryDownloadBusy(MachiDownloadQueueKey(item))
    End Function

    Private Function IsCharaDownloadBusy(item As CharaDen) As Boolean
        Return IsLibraryDownloadBusy(CharaDownloadQueueKey(item))
    End Function

    Private Function IsLibraryDownloadBusy(key As String) As Boolean
        If String.IsNullOrWhiteSpace(key) Then Return False
        Dim status As LibraryDownloadStatus = Nothing
        If Not _libraryDownloadStates.TryGetValue(key, status) Then Return False
        Return status.State <> LibraryDownloadState.Failed
    End Function

    Private Function GetGameDownloadStatus(game As Game) As LibraryDownloadStatus
        Return GetLibraryDownloadStatus(GameDownloadQueueKey(game))
    End Function

    Private Function GetMachiDownloadStatus(item As MachiChara) As LibraryDownloadStatus
        Return GetLibraryDownloadStatus(MachiDownloadQueueKey(item))
    End Function

    Private Function GetCharaDownloadStatus(item As CharaDen) As LibraryDownloadStatus
        Return GetLibraryDownloadStatus(CharaDownloadQueueKey(item))
    End Function

    Private Function GetLibraryDownloadStatus(key As String) As LibraryDownloadStatus
        If String.IsNullOrWhiteSpace(key) Then Return Nothing
        Dim status As LibraryDownloadStatus = Nothing
        _libraryDownloadStates.TryGetValue(key, status)
        Return status
    End Function

    Private Shared Function DownloadStateText(status As LibraryDownloadStatus) As String
        If status Is Nothing Then Return String.Empty
        Select Case status.State
            Case LibraryDownloadState.Queued
                Return "Queued"
            Case LibraryDownloadState.Downloading
                Return If(status.Percentage >= 0, $"Downloading {status.Percentage}%", "Downloading")
            Case LibraryDownloadState.Extracting
                Return "Extracting"
            Case LibraryDownloadState.Installing
                Return "Installing"
            Case LibraryDownloadState.Failed
                Return "Download failed"
            Case Else
                Return String.Empty
        End Select
    End Function

    Private Function QueueLibraryDownload(
        key As String,
        title As String,
        operation As Func(Of IProgress(Of DownloadProgressInfo), Task),
        completed As Action(Of Boolean, String)
    ) As Boolean
        If String.IsNullOrWhiteSpace(key) OrElse operation Is Nothing Then Return False

        If IsLibraryDownloadBusy(key) Then
            NotificationManager.ShowInformation(Me, "Already in downloads", $"'{title}' is already queued or being installed.")
            Return False
        End If

        _libraryDownloadStates(key) = New LibraryDownloadStatus With {
            .State = LibraryDownloadState.Queued,
            .StatusText = "Waiting for the current download"
        }
        _libraryDownloadQueue.Enqueue(New QueuedLibraryDownload With {
            .Key = key,
            .Title = title,
            .Operation = operation,
            .Completed = completed
        })

        RefreshDownloadStateUi()
        If _activeLibraryDownload Is Nothing Then
            UpdateDownloadQueuePanel(title, "Starting download...", -1, _libraryDownloadQueue.Count)
        Else
            Dim activeStatus = _libraryDownloadStates(_activeLibraryDownload.Key)
            UpdateDownloadQueuePanel(
                _activeLibraryDownload.Title,
                activeStatus.StatusText,
                activeStatus.Percentage,
                _libraryDownloadQueue.Count)
        End If

        If Not _downloadQueueProcessorRunning Then
            Dim ignoredTask = ProcessLibraryDownloadQueueAsync()
        End If
        Return True
    End Function

    Private Async Function ProcessLibraryDownloadQueueAsync() As Task
        If _downloadQueueProcessorRunning Then Return
        _downloadQueueProcessorRunning = True

        Try
            While _libraryDownloadQueue.Count > 0
                _activeLibraryDownload = _libraryDownloadQueue.Dequeue()
                Dim active = _activeLibraryDownload
                Dim status = _libraryDownloadStates(active.Key)
                status.State = LibraryDownloadState.Downloading
                status.Percentage = -1
                status.StatusText = "Connecting..."

                UpdateDownloadQueuePanel(active.Title, status.StatusText, -1, _libraryDownloadQueue.Count)
                RefreshDownloadStateUi()

                Dim progress As New Progress(Of DownloadProgressInfo)(
                    Sub(update) ApplyDownloadProgress(active, update))
                Dim succeeded = False
                Dim failureMessage = String.Empty

                Try
                    Await active.Operation(progress)
                    succeeded = True
                    _libraryDownloadStates.Remove(active.Key)
                Catch ex As Exception
                    failureMessage = ex.Message
                    status.State = LibraryDownloadState.Failed
                    status.Percentage = -1
                    status.StatusText = failureMessage
                    Logger.LogError($"[DownloadQueue] '{active.Title}' failed: {ex}")
                End Try

                Try
                    active.Completed?.Invoke(succeeded, failureMessage)
                Catch completionException As Exception
                    Logger.LogError($"[DownloadQueue] Completion handler failed for '{active.Title}': {completionException}")
                End Try

                _activeLibraryDownload = Nothing
                RefreshDownloadStateUi()
            End While
        Finally
            _activeLibraryDownload = Nothing
            _downloadQueueProcessorRunning = False
            HideDownloadQueuePanel()
        End Try
    End Function

    Private Sub ApplyDownloadProgress(active As QueuedLibraryDownload, update As DownloadProgressInfo)
        If update Is Nothing OrElse _activeLibraryDownload Is Nothing OrElse
           Not String.Equals(_activeLibraryDownload.Key, active.Key, StringComparison.OrdinalIgnoreCase) Then Return

        Dim status = _libraryDownloadStates(active.Key)
        Dim previousState = status.State
        Select Case update.Phase
            Case DownloadOperationPhase.Extracting
                status.State = LibraryDownloadState.Extracting
            Case DownloadOperationPhase.Installing
                status.State = LibraryDownloadState.Installing
            Case Else
                status.State = LibraryDownloadState.Downloading
        End Select
        status.Percentage = update.Percentage
        status.StatusText = update.StatusText

        UpdateDownloadQueuePanel(active.Title, update.StatusText, update.Percentage, _libraryDownloadQueue.Count)
        If previousState <> status.State Then RefreshDownloadStateUi()
    End Sub

    Private Sub QueueGameDownload(request As QueuedGameDownload)
        Dim queueKey = GameDownloadQueueKey(request.Game)
        request.Key = queueKey

        QueueLibraryDownload(
            queueKey,
            request.Game.ENTitle,
            Async Function(progress)
                If request.IsRedownload Then
                    progress.Report(New DownloadProgressInfo With {
                        .Phase = DownloadOperationPhase.Installing,
                        .StatusText = "Backing up save data...",
                        .Percentage = -1
                    })
                    Await SaveDataManager.BackupSaveAsync(request.ExtractFolder, request.Game.Emulator, Me, False)
                End If

                Dim downloader As New GameDownloader(progress)
                Await downloader.DownloadGameAsync(
                    request.Game.DownloadURL,
                    request.ZipPath,
                    request.ExtractFolder,
                    request.Game,
                    request.JamPath,
                    request.JarPath,
                    False)

                If Not File.Exists(request.JamPath) OrElse Not File.Exists(request.JarPath) Then
                    Throw New InvalidDataException("The download finished, but the app's launch files are incomplete.")
                End If
            End Function,
            Sub(succeeded, failureMessage)
                If succeeded Then
                    Logger.LogInfo($"Download completed for {request.Game.ENTitle}")
                    If IsCurrentlySelectedGame(request.Game) Then
                        SetCurrentGamePaths(request.Game)
                        If String.Equals(CurrentSelectedGameJAM, request.JamPath, StringComparison.OrdinalIgnoreCase) AndAlso
                           File.Exists(CurrentSelectedGameJAM) Then
                            UtilManager.GenerateDynamicControlsFromLines(CurrentSelectedGameJAM, panelDynamic, request.Game.ENTitle)
                        End If
                    End If
                    NotificationManager.ShowSuccess(Me, "Download complete", $"'{request.Game.ENTitle}' is installed and ready to play.")
                Else
                    NotificationManager.ShowFailure(Me, "Download failed", $"'{request.Game.ENTitle}' could not be installed. You can try again.")
                End If
            End Sub)
    End Sub

    Private Sub QueueMachiCharaDownload(item As MachiChara, destinationPath As String)
        QueueLibraryDownload(
            MachiDownloadQueueKey(item),
            item.ENTitle,
            Async Function(progress)
                Dim downloader As New FileDownloader(progress)
                Await downloader.DownloadFileAsync(item.DownloadURL, destinationPath, "Machi-Chara")
                If Not File.Exists(destinationPath) Then Throw New InvalidDataException("The downloaded Machi-Chara file is missing.")
            End Function,
            Sub(succeeded, failureMessage)
                HighlightMachiChara()
                If succeeded Then
                    NotificationManager.ShowSuccess(Me, "Download complete", $"'{item.ENTitle}' is ready to use.")
                Else
                    NotificationManager.ShowFailure(Me, "Download failed", $"'{item.ENTitle}' could not be downloaded.")
                End If
            End Sub)
    End Sub

    Private Sub QueueCharaDenDownload(item As CharaDen, destinationPath As String)
        QueueLibraryDownload(
            CharaDownloadQueueKey(item),
            item.ENTitle,
            Async Function(progress)
                Dim downloader As New FileDownloader(progress)
                Await downloader.DownloadFileAsync(item.DownloadURL, destinationPath, "Chara-Den")
                If Not File.Exists(destinationPath) Then Throw New InvalidDataException("The downloaded Chara-Den file is missing.")
            End Function,
            Sub(succeeded, failureMessage)
                HighlightCharaDen()
                If succeeded Then
                    NotificationManager.ShowSuccess(Me, "Download complete", $"'{item.ENTitle}' is ready to use.")
                Else
                    NotificationManager.ShowFailure(Me, "Download failed", $"'{item.ENTitle}' could not be downloaded.")
                End If
            End Sub)
    End Sub

    Private Function IsCurrentlySelectedGame(game As Game) As Boolean
        If game Is Nothing OrElse ListViewGames.SelectedItems.Count = 0 Then Return False
        Dim selected = TryCast(ListViewGames.SelectedItems(0).Tag, Game)
        Return String.Equals(GetGameKey(selected), GetGameKey(game), StringComparison.OrdinalIgnoreCase)
    End Function

    Private Sub SetCurrentGamePaths(game As Game)
        Dim selectedVariant = If(ListViewGamesVariants.SelectedItems.Count > 0, ListViewGamesVariants.SelectedItems(0).Text.Trim(), String.Empty)
        currentGamePaths = pathResolver.Resolve(game, selectedVariant, DownloadsFolder)
        CurrentSelectedGameJAM = currentGamePaths.JAM
        CurrentSelectedGameJAR = currentGamePaths.JAR
        CurrentSelectedGameSP = currentGamePaths.SP
        CurrentSelectedGameKJX = currentGamePaths.KJX
    End Sub

    Private Sub RefreshDownloadStateUi()
        If ListViewGames IsNot Nothing Then RefreshGameHighlighting()
        If ListViewMachiChara IsNot Nothing Then HighlightMachiChara()
        If ListViewCharaDen IsNot Nothing Then HighlightCharaDen()

        If ListViewGames?.SelectedItems.Count > 0 Then
            UpdateGameSelectionState(TryCast(ListViewGames.SelectedItems(0).Tag, Game))
        End If
        If ListViewMachiChara?.SelectedItems.Count > 0 Then
            UpdateMachiCharaSelectionState(TryCast(ListViewMachiChara.SelectedItems(0).Tag, MachiChara))
        End If
        If ListViewCharaDen?.SelectedItems.Count > 0 Then
            UpdateCharaDenSelectionState(TryCast(ListViewCharaDen.SelectedItems(0).Tag, CharaDen))
        End If
    End Sub
End Class
