Option Strict On
Option Explicit On

Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports Microsoft.Web.WebView2.Core
Imports Microsoft.Web.WebView2.WinForms
Imports System.Diagnostics
Imports System.Text.Json


Public Class HomepageManager
    Private Class WebMsg
        Public Property action As String
        Public Property url As String
    End Class


    Private ReadOnly _homepageUrl As String
    Private _tp As TabPage
    Private _web As WebView2

    ' Cache layout:
    '   <AppData>\YourApp\homepage-cache\
    '       index.html               (rewritten offline-safe)
    '       raw.html                 (optional raw snapshot)
    '       assets\<hash>.<ext>      (downloaded same-origin assets)
    Private ReadOnly _cacheRoot As String
    Private ReadOnly _cacheIndexPath As String
    Private ReadOnly _cacheRawPath As String
    Private ReadOnly _cacheAssetsDir As String

    ' WebView2 user data folder (separate from our offline cache)
    Private ReadOnly _wvUserDataDir As String

    Private ReadOnly _http As HttpClient
    Private _initDone As Boolean

    Public Sub New(homepageUrl As String)
        If String.IsNullOrWhiteSpace(homepageUrl) Then Throw New ArgumentException("Homepage URL cannot be empty.", NameOf(homepageUrl))
        _homepageUrl = homepageUrl.Trim()

        Dim baseAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                       "KeitaiWorldLauncher") ' change if you want
        _cacheRoot = Path.Combine(baseAppData, "homepage-cache")
        _cacheAssetsDir = Path.Combine(_cacheRoot, "assets")
        _cacheIndexPath = Path.Combine(_cacheRoot, "index.html")
        _cacheRawPath = Path.Combine(_cacheRoot, "raw.html")

        _wvUserDataDir = Path.Combine(baseAppData, "webview2-userdata")

        Dim handler As New HttpClientHandler() With {
            .AutomaticDecompression = DecompressionMethods.GZip Or DecompressionMethods.Deflate
        }
        _http = New HttpClient(handler) With {
            .Timeout = TimeSpan.FromSeconds(15)
        }
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("KeitaiWorldLauncher/1.0 (HomepageManager)")
    End Sub

    Public Async Function InitializeAsync(tpHomepage As TabPage) As Task
        If tpHomepage Is Nothing Then Throw New ArgumentNullException(NameOf(tpHomepage))
        If _initDone Then Return

        _tp = tpHomepage
        _tp.Controls.Clear()

        _web = New WebView2() With {.Dock = DockStyle.Fill}
        _tp.Controls.Add(_web)

        Directory.CreateDirectory(_cacheRoot)
        Directory.CreateDirectory(_cacheAssetsDir)
        Directory.CreateDirectory(_wvUserDataDir)

        ' Initialize WebView2 environment
        Dim env = Await CoreWebView2Environment.CreateAsync(userDataFolder:=_wvUserDataDir)
        Await _web.EnsureCoreWebView2Async(env)
        ' Allow our page JS to call window.chrome.webview.postMessage(...)
        _web.CoreWebView2.Settings.IsWebMessageEnabled = True

        ' Handle messages from the page (open external browser, etc.)
        AddHandler _web.CoreWebView2.WebMessageReceived, AddressOf CoreWebView2_WebMessageReceived

        ' Optional but recommended: stop in-webview navigation to external sites
        AddHandler _web.CoreWebView2.NavigationStarting, AddressOf CoreWebView2_NavigationStarting

        ' Optional: tighten some defaults
        _web.CoreWebView2.Settings.IsStatusBarEnabled = False
        _web.CoreWebView2.Settings.AreDefaultContextMenusEnabled = True
        _web.CoreWebView2.Settings.AreDevToolsEnabled = False

        ' If navigation fails, fall back to cached
        AddHandler _web.CoreWebView2.NavigationCompleted,
            Sub(sender, e)
                If Not e.IsSuccess Then
                    ' Try cached if exists
                    If File.Exists(_cacheIndexPath) Then
                        NavigateToCachedFile()
                    Else
                    End If
                End If
            End Sub

        _initDone = True
    End Function

    Public Async Function LoadAsync(Optional forceRefresh As Boolean = False) As Task
        If Not _initDone Then Throw New InvalidOperationException("Call InitializeAsync(tpHomepage) first.")

        Dim online As Boolean = Await IsOnlineAndReachableAsync(_homepageUrl)

        If online AndAlso (forceRefresh OrElse Not File.Exists(_cacheIndexPath)) Then
            Dim ok = Await UpdateOfflineCacheAsync(_homepageUrl)
            If ok Then
                ' Prefer loading the real URL when online (fresh + interactive)
                _web.CoreWebView2.Navigate(_homepageUrl)
                Return
            End If

            ' If cache update failed, still try online navigation
            _web.CoreWebView2.Navigate(_homepageUrl)
            Return
        End If

        If online Then
            _web.CoreWebView2.Navigate(_homepageUrl)
            Return
        End If

        ' Offline path
        If File.Exists(_cacheIndexPath) Then
            NavigateToCachedFile()
        Else
            ShowOfflinePlaceholder()
        End If
    End Function
    Private Sub NavigateToCachedFile()
        Dim uri = New Uri(_cacheIndexPath).AbsoluteUri ' file:///
        _web.CoreWebView2.Navigate(uri)
    End Sub
    Private Sub ShowOfflinePlaceholder()
        Dim html As String =
            "<html><head><meta charset='utf-8'><style>" &
            "body{font-family:Segoe UI,Arial;margin:20px;}" &
            ".box{padding:16px;border:1px solid #ccc;border-radius:12px;max-width:720px;}" &
            "h2{margin:0 0 8px 0;}" &
            "</style></head><body>" &
            "<div class='box'>" &
            "<h2>Homepage unavailable (offline)</h2>" &
            "<p>This tab will cache the homepage after you connect at least once.</p>" &
            "<p>When online, click <b>Refresh</b> to update the offline cache.</p>" &
            "</div></body></html>"
        _web.CoreWebView2.NavigateToString(html)
    End Sub


    ' --- Online checks -------------------------------------------------------

    Private Async Function IsOnlineAndReachableAsync(url As String) As Task(Of Boolean)
        If Not My.Computer.Network.IsAvailable Then Return False

        ' Try a lightweight request to the homepage host
        Try
            Using cts As New CancellationTokenSource(TimeSpan.FromSeconds(6))
                Using req As New HttpRequestMessage(HttpMethod.Head, url)
                    Using resp = Await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, cts.Token)
                        Return resp.IsSuccessStatusCode
                    End Using
                End Using
            End Using
        Catch
        End Try

        Try
            Using cts As New CancellationTokenSource(TimeSpan.FromSeconds(8))
                Using resp = Await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cts.Token)
                    Return resp.IsSuccessStatusCode
                End Using
            End Using
        Catch
            Return False
        End Try
    End Function

    ' --- Offline cache builder ----------------------------------------------

    Private Async Function UpdateOfflineCacheAsync(homeUrl As String) As Task(Of Boolean)
        Try
            Directory.CreateDirectory(_cacheRoot)
            Directory.CreateDirectory(_cacheAssetsDir)
            Dim baseUri As New Uri(homeUrl)
            Dim html As String
            Using cts As New CancellationTokenSource(TimeSpan.FromSeconds(20))
                html = Await _http.GetStringAsync(homeUrl, cts.Token)
            End Using
            File.WriteAllText(_cacheRawPath, html, New UTF8Encoding(encoderShouldEmitUTF8Identifier:=False))
            ' Find asset URLs (basic: src= and href= for same-origin)
            Dim assetUrls = ExtractAssetUrls(html, baseUri)
            ' Download assets and rewrite HTML references to local cached paths
            Dim map As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
            For Each asset In assetUrls
                Dim localRel As String = Await DownloadAssetIfAllowedAsync(asset, baseUri)
                If Not String.IsNullOrEmpty(localRel) Then
                    map(asset.AbsoluteUri) = localRel
                End If
            Next
            Dim rewritten = RewriteHtmlReferences(html, baseUri, map)
            rewritten = EnsureBaseTag(rewritten, baseUri)
            File.WriteAllText(_cacheIndexPath, rewritten, New UTF8Encoding(encoderShouldEmitUTF8Identifier:=False))
            Return True
        Catch
            Return False
        End Try
    End Function
    Private Function ExtractAssetUrls(html As String, baseUri As Uri) As List(Of Uri)
        Dim results As New List(Of Uri)
        Dim rx As New Regex("(?:src|href)\s*=\s*[""']([^""']+)[""']",
                            RegexOptions.IgnoreCase Or RegexOptions.Compiled)
        For Each m As Match In rx.Matches(html)
            Dim raw = m.Groups(1).Value.Trim()
            AddUriIfValid(results, raw, baseUri)
        Next

        Dim rxSrcSet As New Regex("srcset\s*=\s*[""']([^""']+)[""']",
                                  RegexOptions.IgnoreCase Or RegexOptions.Compiled)
        For Each m As Match In rxSrcSet.Matches(html)
            Dim raw = m.Groups(1).Value
            Dim parts = raw.Split(","c)
            For Each part In parts
                Dim p = part.Trim()
                If p.Length = 0 Then Continue For
                Dim firstToken = p.Split(" "c, ControlChars.Tab)(0).Trim()
                AddUriIfValid(results, firstToken, baseUri)
            Next
        Next

        ' Dedup
        Dim dedup = results.GroupBy(Function(u) u.AbsoluteUri, StringComparer.OrdinalIgnoreCase) _
                           .Select(Function(g) g.First()) _
                           .ToList()
        Return dedup
    End Function
    Private Sub AddUriIfValid(list As List(Of Uri), raw As String, baseUri As Uri)
        If String.IsNullOrWhiteSpace(raw) Then Return
        If raw.StartsWith("data:", StringComparison.OrdinalIgnoreCase) Then Return
        If raw.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase) Then Return
        If raw.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase) Then Return

        Try
            Dim u As Uri
            If Uri.TryCreate(raw, UriKind.Absolute, u) Then
                list.Add(u)
            ElseIf Uri.TryCreate(baseUri, raw, u) Then
                list.Add(u)
            End If
        Catch
        End Try
    End Sub
    Private Async Function DownloadAssetIfAllowedAsync(assetUri As Uri, baseUri As Uri) As Task(Of String)
        If Not String.Equals(assetUri.Host, baseUri.Host, StringComparison.OrdinalIgnoreCase) Then
            Return Nothing
        End If
        Dim ext = Path.GetExtension(assetUri.AbsolutePath)
        If String.IsNullOrEmpty(ext) Then ext = ".bin"
        Dim hashName = Sha1Hex(assetUri.AbsoluteUri)
        Dim localFile = Path.Combine(_cacheAssetsDir, hashName & ext)
        If File.Exists(localFile) Then
            Return "assets/" & Path.GetFileName(localFile)
        End If

        Try
            Using cts As New CancellationTokenSource(TimeSpan.FromSeconds(20))
                Using resp = Await _http.GetAsync(assetUri, HttpCompletionOption.ResponseHeadersRead, cts.Token)
                    If Not resp.IsSuccessStatusCode Then Return Nothing

                    Dim bytes = Await resp.Content.ReadAsByteArrayAsync(cts.Token)
                    Dim tmp = localFile & ".tmp"
                    File.WriteAllBytes(tmp, bytes)
                    If File.Exists(localFile) Then File.Delete(tmp) Else File.Move(tmp, localFile)

                    Return "assets/" & Path.GetFileName(localFile)
                End Using
            End Using
        Catch
            Return Nothing
        End Try
    End Function
    Private Function RewriteHtmlReferences(html As String, baseUri As Uri, map As Dictionary(Of String, String)) As String
        Dim rewritten = html
        For Each kvp In map
            rewritten = rewritten.Replace(kvp.Key, kvp.Value)
        Next
        Dim rx As New Regex("(?:src|href)\s*=\s*[""']([^""']+)[""']",
                            RegexOptions.IgnoreCase Or RegexOptions.Compiled)

        rewritten = rx.Replace(rewritten,
            Function(m As Match)
                Dim raw = m.Groups(1).Value
                Dim abs As Uri = Nothing
                Try
                    If Uri.TryCreate(raw, UriKind.Absolute, abs) Then
                        Return m.Value
                    End If
                    If Uri.TryCreate(baseUri, raw, abs) Then
                        Dim key = abs.AbsoluteUri
                        If map.ContainsKey(key) Then
                            Dim prefix = m.Value.Substring(0, m.Value.IndexOf("="c) + 1)
                            Dim quote = If(m.Value.Contains(""""), """", "'")
                            Return prefix & " " & quote & map(key) & quote
                        End If
                    End If
                Catch
                End Try
                Return m.Value
            End Function)

        Return rewritten
    End Function
    Private Function EnsureBaseTag(html As String, baseUri As Uri) As String
        If Regex.IsMatch(html, "<base\s", RegexOptions.IgnoreCase) Then Return html

        Dim baseTag = $"<base href=""{baseUri.GetLeftPart(UriPartial.Authority)}/"">"

        Dim rxHead As New Regex("<head[^>]*>", RegexOptions.IgnoreCase)
        If rxHead.IsMatch(html) Then
            Return rxHead.Replace(html, Function(m) m.Value & baseTag, 1)
        End If

        Return "<head>" & baseTag & "</head>" & html
    End Function
    Private Function Sha1Hex(s As String) As String
        Using sha As SHA1 = SHA1.Create()
            Dim bytes = Encoding.UTF8.GetBytes(s)
            Dim hash = sha.ComputeHash(bytes)
            Dim sb As New StringBuilder(hash.Length * 2)
            For Each b In hash
                sb.Append(b.ToString("x2"))
            Next
            Return sb.ToString()
        End Using
    End Function
    Private Sub CoreWebView2_WebMessageReceived(sender As Object, e As CoreWebView2WebMessageReceivedEventArgs)
        Try
            Dim json As String = e.WebMessageAsJson()
            If String.IsNullOrWhiteSpace(json) Then Return

            Dim msg = JsonSerializer.Deserialize(Of WebMsg)(json)
            If msg Is Nothing Then Return

            If String.Equals(msg.action, "openExternal", StringComparison.OrdinalIgnoreCase) Then
                Dim url = If(msg.url, "").Trim()
                If url.Length = 0 Then Return

                If Not (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) OrElse
                    url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) Then
                    Return
                End If

                Try
                    Process.Start(New ProcessStartInfo(url) With {.UseShellExecute = True})
                Catch
                End Try
            End If

        Catch
        End Try
    End Sub
    Private Sub CoreWebView2_NavigationStarting(sender As Object, e As CoreWebView2NavigationStartingEventArgs)
        Try
            Dim target = If(e.Uri, "").Trim()
            If target.Length = 0 Then Return
            Dim allow As Boolean =
            target.StartsWith(_homepageUrl, StringComparison.OrdinalIgnoreCase) OrElse
            target.StartsWith("file:///", StringComparison.OrdinalIgnoreCase)

            If Not allow Then
                e.Cancel = True

                If target.StartsWith("http://", StringComparison.OrdinalIgnoreCase) OrElse
               target.StartsWith("https://", StringComparison.OrdinalIgnoreCase) Then
                    Process.Start(New ProcessStartInfo(target) With {.UseShellExecute = True})
                End If
            End If
        Catch
            ' ignore
        End Try
    End Sub
    Public Sub Dispose()
        Try
            If _web IsNot Nothing AndAlso _web.CoreWebView2 IsNot Nothing Then
                RemoveHandler _web.CoreWebView2.WebMessageReceived, AddressOf CoreWebView2_WebMessageReceived
                RemoveHandler _web.CoreWebView2.NavigationStarting, AddressOf CoreWebView2_NavigationStarting
            End If
        Catch
        End Try
    End Sub

End Class

Friend Module HttpClientExtensions
    <Runtime.CompilerServices.Extension>
    Public Async Function GetStringAsync(client As HttpClient, requestUri As String, ct As CancellationToken) As Task(Of String)
        Using resp = Await client.GetAsync(requestUri, HttpCompletionOption.ResponseContentRead, ct)
            resp.EnsureSuccessStatusCode()
            Return Await resp.Content.ReadAsStringAsync(ct)
        End Using
    End Function
End Module
