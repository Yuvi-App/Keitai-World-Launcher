Imports System.IO
Imports KeitaiWorldLauncher.My.Models

Namespace My.Managers
    ''' <summary>
    ''' Resolves file system paths for a game + variant + emulator combination.
    Public Class GamePathResolver
        Public Function Resolve(game As Game, selectedVariant As String, downloadsFolder As String) As GamePaths

            If game Is Nothing Then Throw New ArgumentNullException(NameOf(game))
            If String.IsNullOrWhiteSpace(downloadsFolder) Then Throw New ArgumentException("Downloads folder cannot be empty.", NameOf(downloadsFolder))

            Dim result As New GamePaths()
            Dim emulator As String = If(game.Emulator, "").ToLowerInvariant()

            ' --- Shared base values ---
            result.BaseFileName = Path.GetFileNameWithoutExtension(game.ZIPName)
            result.InstallKey = $"{result.BaseFileName}_{emulator}"
            result.GameBaseFolder = Path.Combine(downloadsFolder, result.InstallKey)
            result.ZipFilePath = Path.Combine(downloadsFolder, game.ZIPName)

            ' --- Resolve variant ---
            result.ResolvedVariant = ResolveVariant(game.Variants, selectedVariant)

            ' --- Emulator-specific paths ---
            Select Case emulator
                Case "doja", "star"
                    ResolveDojaStarPaths(result)

                Case "jsky", "vodafone", "airedge", "softbank"
                    ResolveMidpPaths(result)

                Case "ezplus"
                    ResolveEzPlusPaths(result)

                Case "flash"
                    ResolveFlashPaths(result)

                Case Else
                    ' Unknown emulator — leave paths empty, caller can handle
                    logger.Logger.LogWarning($"GamePathResolver: Unknown emulator type '{emulator}' for game '{game.ENTitle}'")
            End Select

            Return result
        End Function

        Private Function ResolveVariant(variantsRaw As String, selectedVariant As String) As String

            ' No variants defined on this game
            If String.IsNullOrWhiteSpace(variantsRaw) Then
                Return String.Empty
            End If

            ' User explicitly picked one
            If Not String.IsNullOrWhiteSpace(selectedVariant) Then
                Return selectedVariant.Trim()
            End If

            ' Fall back to the first variant in the comma-separated list
            Dim parts() As String = variantsRaw.Split(","c)
            If parts.Length > 0 AndAlso Not String.IsNullOrWhiteSpace(parts(0)) Then
                Return parts(0).Trim()
            End If

            Return String.Empty
        End Function

        ''' <summary>
        ''' DoJa and Star use: {GameBase}/{Variant}/bin/{Name}.jam and .jar, plus /sp/{Name}.sp
        ''' </summary>
        Private Sub ResolveDojaStarPaths(paths As GamePaths)
            Dim binFolder As String = Path.Combine(paths.GameBaseFolder, paths.ResolvedVariant, "bin")
            Dim spFolder As String = Path.Combine(paths.GameBaseFolder, paths.ResolvedVariant, "sp")

            paths.JAM = Path.Combine(binFolder, $"{paths.BaseFileName}.jam")
            paths.JAR = Path.Combine(binFolder, $"{paths.BaseFileName}.jar")
            paths.SP = Path.Combine(spFolder, $"{paths.BaseFileName}.sp")
        End Sub

        ''' <summary>
        ''' J-SKY, Vodafone, AirEdge, and SoftBank use: {GameBase}/[Variant/]{Name}.jad and .jar
        ''' </summary>
        Private Sub ResolveMidpPaths(paths As GamePaths)
            Dim folder As String = GetVariantOrBaseFolder(paths)

            paths.JAM = Path.Combine(folder, $"{paths.BaseFileName}.jad")
            paths.JAR = Path.Combine(folder, $"{paths.BaseFileName}.jar")
        End Sub

        ''' <summary>
        ''' EZplus uses .kjx files. KJX is always at the game base; JAM/JAR point into variant if present.
        ''' </summary>
        Private Sub ResolveEzPlusPaths(paths As GamePaths)
            Dim folder As String = GetVariantOrBaseFolder(paths)

            ' KJX is always at the game base level (matches original behavior)
            paths.KJX = Path.Combine(paths.GameBaseFolder, $"{paths.BaseFileName}.kjx")

            ' JAM and JAR both point to the .kjx (used as the launch file)
            paths.JAM = Path.Combine(folder, $"{paths.BaseFileName}.kjx")
            paths.JAR = Path.Combine(folder, $"{paths.BaseFileName}.kjx")
        End Sub

        ''' <summary>
        ''' Flash uses .swf files: {GameBase}/[Variant/]{Name}.swf
        ''' </summary>
        Private Sub ResolveFlashPaths(paths As GamePaths)
            Dim folder As String = GetVariantOrBaseFolder(paths)

            paths.JAM = Path.Combine(folder, $"{paths.BaseFileName}.swf")
            paths.JAR = Path.Combine(folder, $"{paths.BaseFileName}.swf")
        End Sub

        ''' <summary>
        ''' Returns the variant subfolder if a variant is set, otherwise the game base folder.
        ''' Used by emulator types that don't have a /bin/ subdirectory structure.
        ''' </summary>
        Private Function GetVariantOrBaseFolder(paths As GamePaths) As String
            If String.IsNullOrWhiteSpace(paths.ResolvedVariant) Then
                Return paths.GameBaseFolder
            Else
                Return Path.Combine(paths.GameBaseFolder, paths.ResolvedVariant)
            End If
        End Function

    End Class
End Namespace
