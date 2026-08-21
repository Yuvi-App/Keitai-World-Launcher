Imports System.Drawing
Imports System.Globalization
Imports System.IO
Imports System.Net.Http
Imports System.Text.RegularExpressions
Imports System.Threading
Imports KeitaiWorldLauncher.My.Models
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Namespace My.Managers
    Public NotInheritable Class KeitaiWikiManager
        Private Const ApiEndpoint As String = "https://keitaiwiki.com/w/api.php"
        Private Const LookupStrategyVersion As Integer = 2
        Private Shared ReadOnly UserAgent As String =
            $"KeitaiWorldLauncher/{GetType(KeitaiWikiManager).Assembly.GetName().Version} (https://keitaiarchive.org)"
        Private Shared ReadOnly ReleaseQualifierSuffix As New Regex(
            "\s*(?:(?:[-–—:]\s*)(?:free\s+trial|trial|demo|sample|preview)(?:\s+(?:version|edition))?|" &
            "(?:[\(\[\{]\s*)(?:free\s+trial|trial|demo|sample|preview)(?:\s+(?:version|edition))?(?:\s*[\)\]\}])|" &
            "(?:free\s+trial|trial|demo|sample|preview)\s+(?:version|edition))\s*$",
            RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant)
        Private Shared ReadOnly AcceptedCacheLifetime As TimeSpan = TimeSpan.FromDays(30)
        Private Shared ReadOnly RejectedCacheLifetime As TimeSpan = TimeSpan.FromDays(7)

        Private ReadOnly _cachePath As String
        Private ReadOnly _thumbnailFolder As String
        Private ReadOnly _cacheLock As New Object()
        Private _cache As KeitaiWikiCacheDocument

        Public Sub New(
            Optional cachePath As String = "configs\keitaiwiki-cache.json",
            Optional thumbnailFolder As String = "data\cache\keitaiwiki")

            _cachePath = cachePath
            _thumbnailFolder = thumbnailFolder
            _cache = LoadCache()
        End Sub

        Public Async Function LookupAsync(
            title As String,
            allowNetwork As Boolean,
            cancellationToken As CancellationToken
        ) As Task(Of KeitaiWikiLookupResult)

            If String.IsNullOrWhiteSpace(title) Then Return NoMatchResult()
            Dim queryTitle = title.Trim()
            Dim lookupTitles = BuildLookupTitles(queryTitle)
            Dim cacheKey = NormalizeCacheKey(queryTitle)
            Dim cachedEntry = GetCacheEntry(cacheKey)
            Dim now = DateTimeOffset.UtcNow

            If cachedEntry IsNot Nothing Then
                If Not allowNetwork Then Return ResultFromCache(cachedEntry)
                Dim lifetime = If(cachedEntry.Rejected, RejectedCacheLifetime, AcceptedCacheLifetime)
                Dim usesCurrentStrategy = Not cachedEntry.Rejected OrElse
                                          cachedEntry.LookupStrategyVersion >= LookupStrategyVersion
                If usesCurrentStrategy AndAlso now - cachedEntry.CachedAtUtc <= lifetime Then
                    Return ResultFromCache(cachedEntry)
                End If
            End If

            If Not allowNetwork Then Return NoMatchResult()

            Try
                Dim candidatesByPageId As New Dictionary(Of Integer, KeitaiWikiMetadata)()

                For Each lookupTitle In lookupTitles
                    Dim exactMetadata = Await QueryExactTitleAsync(lookupTitle, cancellationToken)
                    If exactMetadata IsNot Nothing Then
                        exactMetadata.QueryTitle = queryTitle
                        exactMetadata.MatchScore = 1.0R
                        AcceptMatch(queryTitle, exactMetadata)
                        Return New KeitaiWikiLookupResult With {
                            .Kind = KeitaiWikiLookupKind.Match,
                            .Metadata = exactMetadata
                        }
                    End If

                    Dim searchResults = Await SearchAsync(lookupTitle, cancellationToken)
                    For Each candidate In searchResults
                        candidate.QueryTitle = queryTitle
                        candidate.MatchScore = CalculateTitleSimilarity(lookupTitle, candidate.CanonicalTitle)

                        Dim existingCandidate As KeitaiWikiMetadata = Nothing
                        If candidatesByPageId.TryGetValue(candidate.PageId, existingCandidate) Then
                            If candidate.MatchScore > existingCandidate.MatchScore Then
                                existingCandidate.MatchScore = candidate.MatchScore
                            End If
                        Else
                            candidatesByPageId(candidate.PageId) = candidate
                        End If
                    Next

                    Dim rankedSearchResults = searchResults.
                        OrderByDescending(Function(candidate) candidate.MatchScore).
                        ThenBy(Function(candidate) candidate.CanonicalTitle, StringComparer.OrdinalIgnoreCase).
                        ToList()
                    If rankedSearchResults.Count > 0 Then
                        Dim topSearchResult = rankedSearchResults(0)
                        Dim runnerUpScore = If(rankedSearchResults.Count > 1, rankedSearchResults(1).MatchScore, 0.0R)
                        If IsVeryStrongMatch(lookupTitle, topSearchResult, runnerUpScore) Then
                            AcceptMatch(queryTitle, topSearchResult)
                            Return New KeitaiWikiLookupResult With {
                                .Kind = KeitaiWikiLookupKind.Match,
                                .Metadata = topSearchResult
                            }
                        End If
                    End If
                Next

                Dim candidates = candidatesByPageId.Values.
                    OrderByDescending(Function(candidate) candidate.MatchScore).
                    ThenBy(Function(candidate) candidate.CanonicalTitle, StringComparer.OrdinalIgnoreCase).
                    Take(5).
                    ToList()
                If candidates.Count = 0 Then
                    SaveRejected(cacheKey)
                    Return NoMatchResult()
                End If

                Return New KeitaiWikiLookupResult With {
                    .Kind = KeitaiWikiLookupKind.NeedsConfirmation,
                    .Candidates = candidates
                }
            Catch ex As OperationCanceledException
                Throw
            Catch ex As Exception
                logger.Logger.LogWarning($"KeitaiWiki lookup failed for '{queryTitle}': {ex.Message}")
                If cachedEntry IsNot Nothing AndAlso Not cachedEntry.Rejected AndAlso cachedEntry.Metadata IsNot Nothing Then
                    Dim cachedResult = ResultFromCache(cachedEntry)
                    cachedResult.ErrorMessage = ex.Message
                    Return cachedResult
                End If
                Return New KeitaiWikiLookupResult With {
                    .Kind = KeitaiWikiLookupKind.Failed,
                    .ErrorMessage = ex.Message
                }
            End Try
        End Function

        Public Sub AcceptMatch(queryTitle As String, metadata As KeitaiWikiMetadata)
            If metadata Is Nothing OrElse String.IsNullOrWhiteSpace(queryTitle) Then Return
            Dim cacheKey = NormalizeCacheKey(queryTitle)
            metadata.QueryTitle = queryTitle.Trim()
            SyncLock _cacheLock
                _cache.Entries(cacheKey) = New KeitaiWikiCacheEntry With {
                    .CachedAtUtc = DateTimeOffset.UtcNow,
                    .LookupStrategyVersion = LookupStrategyVersion,
                    .Metadata = metadata,
                    .Rejected = False
                }
                SaveCacheUnsafe()
            End SyncLock
        End Sub

        Public Sub RejectMatch(queryTitle As String)
            If String.IsNullOrWhiteSpace(queryTitle) Then Return
            SaveRejected(NormalizeCacheKey(queryTitle))
        End Sub

        Public Async Function LoadThumbnailAsync(
            metadata As KeitaiWikiMetadata,
            cancellationToken As CancellationToken
        ) As Task(Of Image)

            If metadata Is Nothing OrElse metadata.PageId <= 0 OrElse String.IsNullOrWhiteSpace(metadata.ThumbnailUrl) Then Return Nothing
            Directory.CreateDirectory(_thumbnailFolder)
            Dim cacheFile = Path.Combine(_thumbnailFolder, $"{metadata.PageId}.img")

            If File.Exists(cacheFile) Then
                Try
                    Dim cachedBytes = Await File.ReadAllBytesAsync(cacheFile, cancellationToken)
                    Return CreateImage(cachedBytes)
                Catch ex As OperationCanceledException
                    Throw
                Catch
                    Try
                        File.Delete(cacheFile)
                    Catch
                    End Try
                End Try
            End If

            Using request As New HttpRequestMessage(HttpMethod.Get, metadata.ThumbnailUrl)
                request.Headers.TryAddWithoutValidation("User-Agent", UserAgent)
                request.Headers.TryAddWithoutValidation("Accept", "image/*")
                Using response = Await HttpService.Http.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken)
                    response.EnsureSuccessStatusCode()
                    If response.Content.Headers.ContentLength.HasValue AndAlso response.Content.Headers.ContentLength.Value > 5L * 1024L * 1024L Then
                        Throw New InvalidDataException("The KeitaiWiki thumbnail was unexpectedly large.")
                    End If
                    Dim bytes = Await response.Content.ReadAsByteArrayAsync(cancellationToken)
                    If bytes.Length > 5 * 1024 * 1024 Then Throw New InvalidDataException("The KeitaiWiki thumbnail was unexpectedly large.")
                    cancellationToken.ThrowIfCancellationRequested()
                    Await File.WriteAllBytesAsync(cacheFile, bytes, cancellationToken)
                    Return CreateImage(bytes)
                End Using
            End Using
        End Function

        Private Async Function QueryExactTitleAsync(title As String, cancellationToken As CancellationToken) As Task(Of KeitaiWikiMetadata)
            Dim parameters As New Dictionary(Of String, String) From {
                {"action", "query"},
                {"format", "json"},
                {"formatversion", "2"},
                {"titles", title},
                {"redirects", "1"}
            }
            AddMetadataParameters(parameters)
            Dim response = Await SendQueryAsync(parameters, cancellationToken)
            Return ParsePages(response).FirstOrDefault()
        End Function

        Private Async Function SearchAsync(title As String, cancellationToken As CancellationToken) As Task(Of List(Of KeitaiWikiMetadata))
            Dim parameters As New Dictionary(Of String, String) From {
                {"action", "query"},
                {"format", "json"},
                {"formatversion", "2"},
                {"generator", "search"},
                {"gsrsearch", title},
                {"gsrnamespace", "0"},
                {"gsrlimit", "5"}
            }
            AddMetadataParameters(parameters)
            Dim response = Await SendQueryAsync(parameters, cancellationToken)
            Return ParsePages(response)
        End Function

        Private Shared Sub AddMetadataParameters(parameters As Dictionary(Of String, String))
            parameters("prop") = "info|pageimages|extracts|revisions"
            parameters("inprop") = "url"
            parameters("piprop") = "thumbnail"
            parameters("pithumbsize") = "320"
            parameters("exintro") = "1"
            parameters("explaintext") = "1"
            parameters("rvprop") = "ids|timestamp"
        End Sub

        Private Shared Async Function SendQueryAsync(
            parameters As Dictionary(Of String, String),
            cancellationToken As CancellationToken
        ) As Task(Of JObject)

            Dim query = String.Join("&", parameters.Select(
                Function(pair) $"{Uri.EscapeDataString(pair.Key)}={Uri.EscapeDataString(pair.Value)}"))
            Using request As New HttpRequestMessage(HttpMethod.Get, $"{ApiEndpoint}?{query}")
                request.Headers.TryAddWithoutValidation("User-Agent", UserAgent)
                request.Headers.TryAddWithoutValidation("Accept", "application/json")
                Using response = Await HttpService.Http.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken)
                    response.EnsureSuccessStatusCode()
                    Dim json = Await response.Content.ReadAsStringAsync(cancellationToken)
                    Return JObject.Parse(json)
                End Using
            End Using
        End Function

        Private Shared Function ParsePages(response As JObject) As List(Of KeitaiWikiMetadata)
            Dim results As New List(Of KeitaiWikiMetadata)()
            Dim queryObject = TryCast(response("query"), JObject)
            If queryObject Is Nothing Then Return results
            Dim pages = TryCast(queryObject("pages"), JArray)
            If pages Is Nothing Then Return results

            For Each pageToken In pages
                Dim page = TryCast(pageToken, JObject)
                If page Is Nothing OrElse
                   page.Property("missing") IsNot Nothing OrElse
                   page.Property("invalid") IsNot Nothing Then Continue For

                Dim revisionId As Integer = 0
                Dim revisionTimestamp As DateTimeOffset? = Nothing
                Dim revisions = TryCast(page("revisions"), JArray)
                If revisions IsNot Nothing AndAlso revisions.Count > 0 Then
                    Dim revision = TryCast(revisions(0), JObject)
                    If revision IsNot Nothing Then
                        Integer.TryParse(revision.Value(Of String)("revid"), revisionId)
                        Dim timestampValue = revision.Value(Of String)("timestamp")
                        Dim parsedTimestamp As DateTimeOffset
                        If DateTimeOffset.TryParse(timestampValue, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, parsedTimestamp) Then
                            revisionTimestamp = parsedTimestamp
                        End If
                    End If
                End If

                Dim thumbnailUrl As String = Nothing
                Dim thumbnail = TryCast(page("thumbnail"), JObject)
                If thumbnail IsNot Nothing Then thumbnailUrl = thumbnail.Value(Of String)("source")

                results.Add(New KeitaiWikiMetadata With {
                    .CanonicalTitle = page.Value(Of String)("title"),
                    .PageId = page.Value(Of Integer)("pageid"),
                    .PageUrl = If(page.Value(Of String)("canonicalurl"), page.Value(Of String)("fullurl")),
                    .ThumbnailUrl = thumbnailUrl,
                    .Extract = page.Value(Of String)("extract"),
                    .RevisionId = revisionId,
                    .RevisionTimestamp = revisionTimestamp
                })
            Next
            Return results
        End Function

        Private Function LoadCache() As KeitaiWikiCacheDocument
            Try
                If File.Exists(_cachePath) Then
                    Dim loaded = JsonConvert.DeserializeObject(Of KeitaiWikiCacheDocument)(File.ReadAllText(_cachePath))
                    If loaded IsNot Nothing Then
                        If loaded.Entries Is Nothing Then loaded.Entries = New Dictionary(Of String, KeitaiWikiCacheEntry)(StringComparer.OrdinalIgnoreCase)
                        Return loaded
                    End If
                End If
            Catch ex As Exception
                logger.Logger.LogWarning($"KeitaiWiki cache could not be loaded: {ex.Message}")
            End Try
            Return New KeitaiWikiCacheDocument()
        End Function

        Private Function GetCacheEntry(cacheKey As String) As KeitaiWikiCacheEntry
            SyncLock _cacheLock
                Dim entry As KeitaiWikiCacheEntry = Nothing
                If _cache.Entries.TryGetValue(cacheKey, entry) Then Return entry
            End SyncLock
            Return Nothing
        End Function

        Private Sub SaveRejected(cacheKey As String)
            SyncLock _cacheLock
                _cache.Entries(cacheKey) = New KeitaiWikiCacheEntry With {
                    .CachedAtUtc = DateTimeOffset.UtcNow,
                    .LookupStrategyVersion = LookupStrategyVersion,
                    .Rejected = True
                }
                SaveCacheUnsafe()
            End SyncLock
        End Sub

        Private Sub SaveCacheUnsafe()
            Try
                Dim folder = Path.GetDirectoryName(_cachePath)
                If Not String.IsNullOrWhiteSpace(folder) Then Directory.CreateDirectory(folder)
                Dim temporaryPath = _cachePath & ".tmp"
                File.WriteAllText(temporaryPath, JsonConvert.SerializeObject(_cache, Formatting.Indented))
                File.Move(temporaryPath, _cachePath, True)
            Catch ex As Exception
                logger.Logger.LogWarning($"KeitaiWiki cache could not be saved: {ex.Message}")
            End Try
        End Sub

        Private Shared Function ResultFromCache(entry As KeitaiWikiCacheEntry) As KeitaiWikiLookupResult
            If entry.Rejected OrElse entry.Metadata Is Nothing Then
                Dim noMatch = NoMatchResult()
                noMatch.FromCache = True
                Return noMatch
            End If
            Return New KeitaiWikiLookupResult With {
                .FromCache = True,
                .Kind = KeitaiWikiLookupKind.Match,
                .Metadata = entry.Metadata
            }
        End Function

        Private Shared Function NoMatchResult() As KeitaiWikiLookupResult
            Return New KeitaiWikiLookupResult With {.Kind = KeitaiWikiLookupKind.NoMatch}
        End Function

        Private Shared Function NormalizeCacheKey(value As String) As String
            Return value.Trim().ToLowerInvariant()
        End Function

        Private Shared Function NormalizeComparableTitle(value As String) As String
            If String.IsNullOrWhiteSpace(value) Then Return String.Empty
            Return New String(value.
                Normalize(System.Text.NormalizationForm.FormKC).
                ToLowerInvariant().
                Where(Function(character) Char.IsLetterOrDigit(character)).
                ToArray())
        End Function

        Private Shared Function BuildLookupTitles(title As String) As List(Of String)
            Dim results As New List(Of String) From {title.Trim()}
            Dim baseTitle = ReleaseQualifierSuffix.Replace(title, String.Empty).Trim()
            If baseTitle.Length >= 3 AndAlso
               Not String.Equals(baseTitle, results(0), StringComparison.OrdinalIgnoreCase) Then
                results.Add(baseTitle)
            End If
            Return results
        End Function

        Private Shared Function IsVeryStrongMatch(
            queryTitle As String,
            candidate As KeitaiWikiMetadata,
            runnerUpScore As Double
        ) As Boolean

            Dim normalizedQuery = NormalizeComparableTitle(queryTitle)
            Dim normalizedCandidate = NormalizeComparableTitle(candidate.CanonicalTitle)
            Return String.Equals(normalizedQuery, normalizedCandidate, StringComparison.Ordinal) OrElse
                   (candidate.MatchScore >= 0.94R AndAlso candidate.MatchScore - runnerUpScore >= 0.08R)
        End Function

        Private Shared Function CalculateTitleSimilarity(leftTitle As String, rightTitle As String) As Double
            Dim left = NormalizeComparableTitle(leftTitle)
            Dim right = NormalizeComparableTitle(rightTitle)
            If left.Length = 0 OrElse right.Length = 0 Then Return 0.0R
            If String.Equals(left, right, StringComparison.Ordinal) Then Return 1.0R

            Dim previous(right.Length) As Integer
            Dim current(right.Length) As Integer
            For column = 0 To right.Length
                previous(column) = column
            Next

            For row = 1 To left.Length
                current(0) = row
                For column = 1 To right.Length
                    Dim substitutionCost = If(left(row - 1) = right(column - 1), 0, 1)
                    current(column) = Math.Min(
                        Math.Min(current(column - 1) + 1, previous(column) + 1),
                        previous(column - 1) + substitutionCost)
                Next
                Dim swap = previous
                previous = current
                current = swap
            Next

            Dim distance = previous(right.Length)
            Return 1.0R - (CDbl(distance) / Math.Max(left.Length, right.Length))
        End Function

        Private Shared Function CreateImage(bytes As Byte()) As Image
            Using stream As New MemoryStream(bytes),
                  source = Image.FromStream(stream)
                Return New Bitmap(source)
            End Using
        End Function

        Private NotInheritable Class KeitaiWikiCacheDocument
            Public Property Version As Integer = LookupStrategyVersion
            Public Property Entries As New Dictionary(Of String, KeitaiWikiCacheEntry)(StringComparer.OrdinalIgnoreCase)
        End Class

        Private NotInheritable Class KeitaiWikiCacheEntry
            Public Property CachedAtUtc As DateTimeOffset
            Public Property LookupStrategyVersion As Integer
            Public Property Metadata As KeitaiWikiMetadata
            Public Property Rejected As Boolean
        End Class
    End Class
End Namespace
