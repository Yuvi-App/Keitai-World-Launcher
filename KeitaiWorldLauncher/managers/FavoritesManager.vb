Imports System.IO

Namespace My.Managers
    Public Class FavoritesManager
        Private Const FavoritesFile As String = "configs\favorites.txt"
        Private ReadOnly _favorites As HashSet(Of String)

        Public Sub New()
            ' Load into memory
            If File.Exists(FavoritesFile) Then
                _favorites = File.ReadAllLines(FavoritesFile) _
                    .Select(Function(line) line.Trim()) _
                    .Where(Function(line) Not String.IsNullOrWhiteSpace(line)) _
                    .ToHashSet(StringComparer.OrdinalIgnoreCase)
            Else
                _favorites = New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            End If
        End Sub

        Public Sub AddToFavorites(gameKey As String)
            If _favorites.Add(gameKey) Then
                SaveToDisk()
            End If
        End Sub

        Public Sub RemoveFromFavorites(gameKey As String)
            If _favorites.Remove(gameKey) Then
                SaveToDisk()
            End If
        End Sub

        Public Function IsGameFavorited(gameKey As String) As Boolean
            Return _favorites.Contains(gameKey)
        End Function

        Public Function GetAllFavorites() As HashSet(Of String)
            Return _favorites
        End Function

        Private Sub SaveToDisk()
            Try
                Directory.CreateDirectory(Path.GetDirectoryName(FavoritesFile))
                File.WriteAllLines(FavoritesFile, _favorites)
            Catch ex As Exception
                logger.Logger.LogError($"Failed to save favorites: {ex.Message}")
            End Try
        End Sub
    End Class
End Namespace