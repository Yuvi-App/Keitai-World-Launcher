Imports System.IO

Namespace My.Managers
    Public Class FavoritesManager
        Private Const FavoritesFile As String = "configs\favorites.txt"

        ' Add a game to the favorites list
        Public Sub AddToFavorites(gameTitle As String)
            ' Ensure the file exists
            If Not File.Exists(FavoritesFile) Then
                File.Create(FavoritesFile).Dispose()
            End If

            ' Check if the game is already in favorites
            If Not IsGameFavorited(gameTitle) Then
                File.AppendAllText(FavoritesFile, gameTitle & Environment.NewLine)
            End If
        End Sub

        ' Remove a game from the favorites list
        Public Sub RemoveFromFavorites(gameTitle As String)
            If File.Exists(FavoritesFile) Then
                Dim favorites = File.ReadAllLines(FavoritesFile).ToList()
                favorites.RemoveAll(Function(title) title.Trim().ToLower() = gameTitle.Trim().ToLower())
                File.WriteAllLines(FavoritesFile, favorites)
            End If
        End Sub

        ' Check if a game is in the favorites list
        Public Function IsGameFavorited(gameTitle As String) As Boolean
            If File.Exists(FavoritesFile) Then
                Dim favorites = File.ReadAllLines(FavoritesFile)
                Return favorites.Any(Function(title) title.Trim().ToLower() = gameTitle.Trim().ToLower())
            End If
            Return False
        End Function

        ' Get all favorited games
        Public Function GetAllFavorites() As List(Of String)
            If File.Exists(FavoritesFile) Then
                Return File.ReadAllLines(FavoritesFile).Select(Function(title) title.Trim()).ToList()
            End If
            Return New List(Of String)()
        End Function
    End Class
End Namespace

