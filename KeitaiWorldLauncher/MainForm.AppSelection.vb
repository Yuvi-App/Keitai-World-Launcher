Imports System.Drawing
Imports System.IO
Imports System.Linq
Imports System.Windows.Forms
Imports KeitaiWorldLauncher.My.logger
Imports KeitaiWorldLauncher.My.Managers
Imports KeitaiWorldLauncher.My.Models

Partial Public Class MainForm
    Private _appDeletionInProgress As Boolean

    Private Function GetSelectedGames() As List(Of Game)
        Return ListViewGames.SelectedItems.Cast(Of ListViewItem)().
            Select(Function(item) TryCast(item.Tag, Game)).
            Where(Function(game) game IsNot Nothing).
            DistinctBy(Function(game) GetGameKey(game), StringComparer.OrdinalIgnoreCase).ToList()
    End Function

    Private Function CanDeleteGame(game As Game) As Boolean
        Dim key = GetGameKey(game)
        If String.IsNullOrWhiteSpace(key) Then Return False
        Return IsCustomGame(game) OrElse
            Directory.Exists(Path.Combine(DownloadsFolder, key))
    End Function

    Private Shared Function IsCustomGame(game As Game) As Boolean
        Return game IsNot Nothing AndAlso
            If(game.DownloadURL, String.Empty).StartsWith("custom://", StringComparison.OrdinalIgnoreCase)
    End Function

    Private Shared Function CustomGameEntryKey(line As String) As String
        Dim parts = line.Split("|"c)
        Dim name = parts(0).Trim()
        If String.IsNullOrWhiteSpace(name) Then Return String.Empty
        Dim emulator = "doja"
        If parts.Length > 1 Then
            Select Case parts(1).Trim().ToLowerInvariant()
                Case "doja", "star", "jsky", "airedge", "vodafone", "softbank", "flash", "ezplus"
                    emulator = parts(1).Trim().ToLowerInvariant()
            End Select
        End If
        Return $"{name}_{emulator}"
    End Function

    Private Sub ShowMultipleGamesSelected()
        Dim selectedGames = GetSelectedGames()
        CancelKeitaiWikiLookup(True)
        panelDynamic.Controls.Clear()
        panelDynamic.AutoScrollPosition = Point.Empty
        gbxGameInfo.Text = $"{ListViewGames.SelectedItems.Count} apps selected"
        panelDynamic.Controls.Add(CreateEmptyStateLabel(
            $"{ListViewGames.SelectedItems.Count} apps selected.{vbCrLf}{vbCrLf}" &
            "Choose Actions > Delete selected to remove their installed files and custom app entries." & vbCrLf & vbCrLf &
            "Select one app to play, download, or view its details."))

        _isProgrammaticVariantSelection = True
        Try
            ListViewGamesVariants.Items.Clear()
            ListViewGamesVariants.Visible = False
            lblTotalVariantCount.Text = "Variants: 0"
        Finally
            _isProgrammaticVariantSelection = False
        End Try
        currentGamePaths = Nothing
        CurrentSelectedGameJAM = Nothing
        CurrentSelectedGameJAR = Nothing
        CurrentSelectedGameSP = Nothing
        CurrentSelectedGameKJX = Nothing

        SetGameActionAvailability(True, False)
        btnLaunchGame.Text = "Select one app"
        _actionFavorite.Text = "Toggle favorites"
        cmsGameLV_Favorite.Text = "Toggle favorites"
        Dim canDelete = Not _appDeletionInProgress AndAlso selectedGames.Any(Function(game) CanDeleteGame(game)) AndAlso
            Not selectedGames.Any(Function(game) IsGameDownloadBusy(game))
        _actionDelete.Text = $"Delete selected ({selectedGames.Count})"
        _actionDelete.Enabled = canDelete
        cmsGameLV_Delete.Text = _actionDelete.Text
        cmsGameLV_Delete.Enabled = canDelete
        SetActionGuidance(btnLaunchGame, "Select exactly one app to play or download.")
        SetActionGuidance(_btnGameActions, "Delete the selected apps or update their favorites.")
    End Sub

    Private Async Sub AppList_KeyDown(sender As Object, e As KeyEventArgs)
        If e.Control AndAlso e.KeyCode = Keys.A Then
            e.Handled = True
            e.SuppressKeyPress = True
            ListViewGames.BeginUpdate()
            Try
                For Each item As ListViewItem In ListViewGames.Items
                    item.Selected = True
                Next
            Finally
                ListViewGames.EndUpdate()
            End Try
        ElseIf e.KeyCode = Keys.Delete Then
            e.Handled = True
            e.SuppressKeyPress = True
            Await DeleteGamesAsync()
        End If
    End Sub

    Private Async Function DeleteGamesAsync() As Task
        If _appDeletionInProgress Then Return
        Dim selectedGames = GetSelectedGames()
        If selectedGames.Count = 0 Then
            UIDialogManager.ShowNotice(Me, "Select an app", "Choose installed or custom apps before using Delete.", "OK", CompactDialogTone.Warning)
            Return
        End If

        Dim busyGame = selectedGames.FirstOrDefault(Function(game) IsGameDownloadBusy(game))
        If busyGame IsNot Nothing Then
            NotificationManager.ShowWarning(Me, "Download in progress", $"Wait for '{busyGame.ENTitle}' to finish installing before deleting it.")
            Return
        End If
        Dim gamesToDelete = selectedGames.Where(Function(game) CanDeleteGame(game)).ToList()
        If gamesToDelete.Count = 0 Then Return

        Dim gameList = String.Join(Environment.NewLine, gamesToDelete.Select(Function(game) $"{game.ENTitle} [{game.Emulator}]"))
        Dim skippedText = If(selectedGames.Count > gamesToDelete.Count,
            $"{vbCrLf}{vbCrLf}{selectedGames.Count - gamesToDelete.Count} selected apps have no installed files to remove.", String.Empty)
        Dim result = UIDialogManager.ShowDetailsConfirmation(
            Me,
            If(gamesToDelete.Count = 1, "Delete app?", $"Delete {gamesToDelete.Count} apps?"),
            $"The following apps' installed files and custom app entries will be removed:{vbCrLf}{vbCrLf}{gameList}{vbCrLf}{vbCrLf}" &
            "Save data inside their app folders will also be removed. Existing backups are kept." & skippedText,
            If(gamesToDelete.Count = 1, "Delete", "Delete selected"),
            "Cancel",
            CompactDialogTone.Danger)
        If result <> DialogResult.Yes Then Return

        _appDeletionInProgress = True
        UtilManager.ShowLaunchOverlay(Me, "Deleting selected apps...")
        Try
            Dim deletedGames As New List(Of Game)
            Dim deletedCustomKeys As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            Dim failedGames As New List(Of String)
            Dim downloadsRoot = Path.TrimEndingDirectorySeparator(Path.GetFullPath(DownloadsFolder))
            Dim sdkFolders = cbxDojaSDK.Items.Cast(Of String)().ToArray()
            Dim customKeys = If(File.Exists(CustomGamesTxtFile),
                File.ReadAllLines(CustomGamesTxtFile).Select(AddressOf CustomGameEntryKey).ToHashSet(StringComparer.OrdinalIgnoreCase),
                New HashSet(Of String)(StringComparer.OrdinalIgnoreCase))

            Await Task.Run(Sub()
                               For Each game In gamesToDelete
                                   Try
                                       Dim key = GetGameKey(game)
                                       If key.IndexOfAny({Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar}) >= 0 Then
                                           Throw New IOException("The app folder name contains a directory separator.")
                                       End If
                                       Dim gameFolder = Path.GetFullPath(Path.Combine(downloadsRoot, key))
                                       If Not String.Equals(Path.GetDirectoryName(gameFolder), downloadsRoot, StringComparison.OrdinalIgnoreCase) Then
                                           Throw New IOException("The app folder is outside the downloads directory.")
                                       End If
                                       Dim hasCustomEntry = customKeys.Contains(key) OrElse IsCustomGame(game)
                                       If Directory.Exists(gameFolder) Then Directory.Delete(gameFolder, recursive:=True)
                                       Dim zipPath = Path.Combine(downloadsRoot, key & ".zip")
                                       If File.Exists(zipPath) Then File.Delete(zipPath)

                                       If Not String.IsNullOrEmpty(game.SDCardDataURL) Then
                                           For Each sdk In sdkFolders
                                               Dim sdJamFile = Path.Combine(ToolsFolder, Path.GetFileName(sdk),
                                                   "lib", "storagedevice", "ext0", "SD_BIND",
                                                   "SVC0000" & Path.GetFileNameWithoutExtension(game.ZIPName) & ".jam")
                                               If File.Exists(sdJamFile) Then File.Delete(sdJamFile)
                                           Next
                                       End If
                                       If hasCustomEntry Then deletedCustomKeys.Add(key)
                                       deletedGames.Add(game)
                                   Catch ex As Exception
                                       failedGames.Add($"{game.ENTitle} [{game.Emulator}] (Error: {ex.Message})")
                                   End Try
                               Next

                               If deletedCustomKeys.Count > 0 AndAlso File.Exists(CustomGamesTxtFile) Then
                                   Try
                                       Dim remainingLines = File.ReadAllLines(CustomGamesTxtFile).
                                           Where(Function(line) Not deletedCustomKeys.Contains(CustomGameEntryKey(line))).ToArray()
                                       File.WriteAllLines(CustomGamesTxtFile, remainingLines)
                                   Catch ex As Exception
                                       failedGames.Add($"Custom app entries could not be removed: {ex.Message}")
                                       deletedGames.RemoveAll(Function(game) deletedCustomKeys.Contains(GetGameKey(game)))
                                       deletedCustomKeys.Clear()
                                   End Try
                               End If
                           End Sub)

            If deletedCustomKeys.Count > 0 Then
                games = games.Where(Function(game) Not (IsCustomGame(game) AndAlso
                    deletedCustomKeys.Contains(GetGameKey(game)))).ToList()
                lblTotalGameCount.Text = $"Total: {games.Count}"
            End If
            Await FilterAndHighlightGamesAsync()
            If deletedGames.Count > 0 Then
                NotificationManager.ShowSuccess(Me,
                    If(deletedGames.Count = 1, "App deleted", "Apps deleted"),
                    If(deletedGames.Count = 1, "The selected app was removed.", $"{deletedGames.Count} selected apps were removed."))
            End If
            If failedGames.Count > 0 Then
                UIDialogManager.ShowDetailsNotice(Me, "Some apps could not be deleted",
                    String.Join(Environment.NewLine, failedGames), "Close", CompactDialogTone.Warning)
            End If
        Catch ex As Exception
            Logger.LogError("Failed to delete the selected apps.", ex)
            UIDialogManager.ShowError(Me, "Could not delete apps", ex.Message)
        Finally
            _appDeletionInProgress = False
            UtilManager.HideLaunchOverlay()
        End Try
    End Function
End Class
