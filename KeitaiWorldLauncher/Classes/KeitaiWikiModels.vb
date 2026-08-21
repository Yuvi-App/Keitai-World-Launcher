Namespace My.Models
    Public Enum KeitaiWikiLookupKind
        NoMatch
        Match
        NeedsConfirmation
        Failed
    End Enum

    Public Class KeitaiWikiMetadata
        Public Property QueryTitle As String
        Public Property CanonicalTitle As String
        Public Property PageId As Integer
        Public Property PageUrl As String
        Public Property ThumbnailUrl As String
        Public Property Extract As String
        Public Property RevisionId As Integer
        Public Property RevisionTimestamp As DateTimeOffset?
        Public Property MatchScore As Double

        Public Overrides Function ToString() As String
            Return If(String.IsNullOrWhiteSpace(CanonicalTitle), "KeitaiWiki page", CanonicalTitle)
        End Function
    End Class

    Public Class KeitaiWikiLookupResult
        Public Property Kind As KeitaiWikiLookupKind
        Public Property Metadata As KeitaiWikiMetadata
        Public Property Candidates As List(Of KeitaiWikiMetadata) = New List(Of KeitaiWikiMetadata)()
        Public Property ErrorMessage As String
        Public Property FromCache As Boolean
    End Class
End Namespace
