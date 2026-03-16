Imports System.IO

Namespace My.Models
    ''' <summary>
    ''' Resolves file system paths for a selected game + variant combination.
    ''' </summary>
    Public Class GamePaths
        Public Property JAM As String = String.Empty
        Public Property JAR As String = String.Empty
        Public Property SP As String = String.Empty
        Public Property KJX As String = String.Empty
        Public Property InstallKey As String = String.Empty
        Public Property GameBaseFolder As String = String.Empty
        Public Property ZipFilePath As String = String.Empty
        Public Property BaseFileName As String = String.Empty
        Public Property ResolvedVariant As String = String.Empty
        Public ReadOnly Property IsDownloaded As Boolean
            Get
                Return Not String.IsNullOrEmpty(JAR) AndAlso File.Exists(JAR)
            End Get
        End Property
        Public ReadOnly Property IsFolderPresent As Boolean
            Get
                Return Not String.IsNullOrEmpty(GameBaseFolder) AndAlso Directory.Exists(GameBaseFolder)
            End Get
        End Property

    End Class
End Namespace
