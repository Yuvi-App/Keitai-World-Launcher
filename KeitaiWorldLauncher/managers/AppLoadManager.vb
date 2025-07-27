Imports System.IO

Namespace My.Managers
    Public Class AppLoadManager
        Dim utilManager As New UtilManager

        Public Shared Sub LoadConfigValues(cfg As Dictionary(Of String, String))
            Form1.versionCheckUrl = cfg("VersionCheckURL")
            Form1.autoUpdate = Boolean.Parse(cfg("AutoUpdate"))
            Form1.FirstRun = Boolean.Parse(cfg("FirstRun"))
            Form1.gameListUrl = cfg("GamelistURL")
            Form1.autoUpdateGameList = Boolean.Parse(cfg("AutoUpdateGameList"))
            Form1.machicharaListUrl = cfg("MachiCharalistURL")
            Form1.autoUpdatemachicharaList = Boolean.Parse(cfg("AutoUpdateMachiCharaList"))
            Form1.charadenListUrl = cfg("CharaDenlistURL")
            Form1.autoUpdatecharadenList = Boolean.Parse(cfg("AutoUpdateCharaDenList"))
            Form1.UseShaderGlass = Boolean.Parse(cfg("UseShaderGlass"))
            Form1.UseDialPad = Boolean.Parse(cfg("UseDialPad"))
            Form1.DOJApath = cfg("DOJAPath")
            Form1.DOJAEXE = cfg("DOJAEXEPath")
            Form1.DOJAHideUI = Boolean.Parse(cfg("DOJAHideUI"))
            Form1.DOJASoundType = cfg("DOJASoundType")
            Form1.STARpath = cfg("STARPath")
            Form1.STAREXE = cfg("STAREXEPath")
            Form1.JSKYpath = cfg("JSKYPath")
            Form1.JSKYEXE = cfg("JSKYEXEPath")
            Form1.VODAFONEpath = cfg("VODAFONEPath")
            Form1.VODAFONEEXE = cfg("VODAFONEEXEPath")
            Form1.AIREDGEpath = cfg("AIREDGEPath")
            Form1.AIREDGEEXE = cfg("AIREDGEEXEPath")
            Form1.FlashPlayerpath = cfg("FlashPlayerPath")
            Form1.FlashPlayerEXE = cfg("FlashPlayerEXEPath")
            Form1.MachiCharapath = cfg("MachiCharaPath")
            Form1.MachiCharaExe = cfg("MachiCharaEXEPath")
            Form1.CharaDenpath = cfg("CharaDenPath")
            Form1.CharaDenExe = cfg("CharaDenEXEPath")
        End Sub
        Public Shared Sub LoadNetworkUID()
            ' Get NetworkUIDConfig
            If File.Exists(Form1.NetworkUIDTxtFile) = False Then
                Using sw As StreamWriter = New StreamWriter(File.Open(Form1.NetworkUIDTxtFile, FileMode.Create))
                    sw.WriteLine("NULLGWDOCOMO")
                    sw.Flush()
                    sw.Close()
                End Using
            End If
            Form1.NetworkUID = File.ReadAllText(Form1.NetworkUIDTxtFile).Trim
        End Sub

    End Class
End Namespace
