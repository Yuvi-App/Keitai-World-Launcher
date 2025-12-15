Imports System.IO

Namespace My.Managers
    Public Class AppLoadManager
        Dim utilManager As New UtilManager

        Public Shared Sub LoadConfigValues(cfg As Dictionary(Of String, String))
            MainForm.versionCheckUrl = cfg("VersionCheckURL")
            MainForm.autoUpdate = Boolean.Parse(cfg("AutoUpdate"))
            MainForm.FirstRun = Boolean.Parse(cfg("FirstRun"))
            MainForm.gameListUrl = cfg("GamelistURL")
            MainForm.autoUpdateGameList = Boolean.Parse(cfg("AutoUpdateGameList"))
            MainForm.machicharaListUrl = cfg("MachiCharalistURL")
            MainForm.autoUpdatemachicharaList = Boolean.Parse(cfg("AutoUpdateMachiCharaList"))
            MainForm.charadenListUrl = cfg("CharaDenlistURL")
            MainForm.autoUpdatecharadenList = Boolean.Parse(cfg("AutoUpdateCharaDenList"))
            MainForm.UseShaderGlass = Boolean.Parse(cfg("UseShaderGlass"))
            MainForm.UseDialPad = Boolean.Parse(cfg("UseDialPad"))
            MainForm.DOJApath = cfg("DOJAPath")
            MainForm.DOJAEXE = cfg("DOJAEXEPath")
            MainForm.DOJAHideUI = Boolean.Parse(cfg("DOJAHideUI"))
            MainForm.DOJASoundType = cfg("DOJASoundType")
            MainForm.STARpath = cfg("STARPath")
            MainForm.STAREXE = cfg("STAREXEPath")
            MainForm.JSKYpath = cfg("JSKYPath")
            MainForm.JSKYEXE = cfg("JSKYEXEPath")
            MainForm.SOFTBANKpath = cfg("SOFTBANKPath")
            MainForm.SOFTBANKEXE = cfg("SOFTBANKEXEPath")
            MainForm.VODAFONEpath = cfg("VODAFONEPath")
            MainForm.VODAFONEEXE = cfg("VODAFONEEXEPath")
            MainForm.AIREDGEpath = cfg("AIREDGEPath")
            MainForm.AIREDGEEXE = cfg("AIREDGEEXEPath")
            MainForm.FlashPlayerpath = cfg("FlashPlayerPath")
            MainForm.FlashPlayerEXE = cfg("FlashPlayerEXEPath")
            MainForm.MachiCharapath = cfg("MachiCharaPath")
            MainForm.MachiCharaExe = cfg("MachiCharaEXEPath")
            MainForm.CharaDenpath = cfg("CharaDenPath")
            MainForm.CharaDenExe = cfg("CharaDenEXEPath")
        End Sub
        Public Shared Sub LoadNetworkUIDTerminaID()
            Dim defaultNetworkUID As String = "NULLGWDOCOMO"
            Dim defaultTerminalID As String = "123456789ABCDEF"

            ' Create file if missing
            If Not File.Exists(MainForm.NetworkUIDTxtFile) Then
                File.WriteAllLines(MainForm.NetworkUIDTxtFile, {defaultNetworkUID, defaultTerminalID})
            End If
            Dim lines As New List(Of String)(File.ReadAllLines(MainForm.NetworkUIDTxtFile))
            Dim needsWriteBack As Boolean = False
            ' Ensure at least 2 lines (legacy upgrade)
            While lines.Count < 2
                lines.Add(String.Empty)
                needsWriteBack = True
            End While
            ' Line 1: Network UID
            If Not String.IsNullOrWhiteSpace(lines(0)) Then
                MainForm.NetworkUID = lines(0).Trim()
            Else
                MainForm.NetworkUID = defaultNetworkUID
                lines(0) = defaultNetworkUID
                needsWriteBack = True
            End If
            ' Line 2: Terminal ID
            If Not String.IsNullOrWhiteSpace(lines(1)) Then
                MainForm.TerminalID = lines(1).Trim()
            Else
                MainForm.TerminalID = defaultTerminalID
                lines(1) = defaultTerminalID
                needsWriteBack = True
            End If
            ' Write back only if we fixed something
            If needsWriteBack Then
                File.WriteAllLines(MainForm.NetworkUIDTxtFile, lines)
            End If
        End Sub
    End Class
End Namespace
