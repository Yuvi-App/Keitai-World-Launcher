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
        Public Shared Sub LoadNetworkUID()
            ' Get NetworkUIDConfig
            If File.Exists(MainForm.NetworkUIDTxtFile) = False Then
                Using sw As StreamWriter = New StreamWriter(File.Open(MainForm.NetworkUIDTxtFile, FileMode.Create))
                    sw.WriteLine("NULLGWDOCOMO")
                    sw.Flush()
                    sw.Close()
                End Using
            End If
            MainForm.NetworkUID = File.ReadAllText(MainForm.NetworkUIDTxtFile).Trim
        End Sub

    End Class
End Namespace
