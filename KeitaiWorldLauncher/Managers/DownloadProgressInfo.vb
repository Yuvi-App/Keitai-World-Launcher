Namespace My.Managers
    Public Enum DownloadOperationPhase
        Downloading
        Extracting
        Installing
    End Enum

    Public NotInheritable Class DownloadProgressInfo
        Public Property Phase As DownloadOperationPhase
        Public Property StatusText As String = String.Empty
        Public Property Percentage As Integer = -1
    End Class
End Namespace
