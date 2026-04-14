Imports System.IO
Imports System.Xml

Namespace My.Managers
    Public Class ConfigManager
        Private _doc As XmlDocument = Nothing
        Private Const ConfigFilePath As String = "configs/appconfig.xml"

        ' Default configuration values
        Private ReadOnly DefaultConfig As New Dictionary(Of String, String) From {
        {"VersionCheckURL", "latest_version.txt"},
        {"AutoUpdate", "true"},
        {"FirstRun", "true"},
        {"PolicyAgreement", "false"},
        {"HomepageURL", "https://example.com"},
        {"GamelistURL", "gamelist.xml"},
        {"AutoUpdateGameList", "true"},
        {"MachiCharalistURL", "machicharalist.xml"},
        {"AutoUpdateMachiCharaList", "true"},
        {"CharaDenlistURL", "charadenlist.xml"},
        {"AutoUpdateCharaDenList", "true"},
        {"UseShaderGlass", "true"},
        {"UseDialPad", "true"},
        {"DojaStarHardwareRendering", "false"},
        {"DojaStarHighPerformanceEXE", "false"},
        {"DOJAPath", "c:\doja"},
        {"DOJAEXEPath", "doja.exe"},
        {"DOJAHideUI", "true"},
        {"DOJASoundType", "standard"},
        {"STARPath", "c:\star"},
        {"STAREXEPath", "star.exe"},
        {"JSKYPath", "c:\JSKY"},
        {"JSKYEXEPath", "JSKY.exe"},
        {"SOFTBANKPath", "c:\MEXA"},
        {"SOFTBANKEXEPath", "mexa.exe"},
        {"VODAFONEPath", "c:\vodafone"},
        {"VODAFONEEXEPath", "vodafone.exe"},
        {"AIREDGEPath", "c:\AIREDGE"},
        {"AIREDGEEXEPath", "AIREDGE.exe"},
        {"EZWEBEZPLUSpath", "c:\EZWEBEZPLUS"},
        {"EZWEBEZPLUSEXEPath", "EZWEBEZPLUS.exe"},
        {"FlashPlayerPath", "c:\flashplayer"},
        {"FlashPlayerEXEPath", "flashplayer.exe"},
        {"MachiCharaPath", "c:\machichara"},
        {"MachiCharaEXEPath", "machicharaemu.exe"},
        {"CharaDenPath", "c:\charaden"},
        {"CharaDenEXEPath", "CharaDenemu.exe"},
        {"OpenDojaPath", "c:\opendoja"},
        {"OpenDojaEXEPath", "opendoja.jar"}
    }

        ' Load configuration from XML
        Public Async Function LoadConfigAsync() As Task(Of Dictionary(Of String, String))
            Return Await Task.Run(Function()
                                      Dim config As New Dictionary(Of String, String)(DefaultConfig)

                                      If IO.File.Exists(ConfigFilePath) Then
                                          Dim doc As New XmlDocument()
                                          doc.Load(ConfigFilePath)
                                          _doc = doc
                                          Dim settings As XmlNodeList = doc.SelectNodes("//Setting")
                                          For Each setting As XmlNode In settings
                                              Dim name As String = setting.Attributes("name").Value
                                              Dim value As String = setting.InnerText
                                              If config.ContainsKey(name) Then
                                                  config(name) = value ' Update from the file
                                              End If
                                          Next
                                      Else
                                          ' Config file doesn't exist, save the default config
                                          SaveConfig(DefaultConfig)
                                          Dim freshDoc As New XmlDocument()
                                          freshDoc.Load(ConfigFilePath)
                                          _doc = freshDoc
                                      End If

                                      Return config
                                  End Function)
        End Function

        ' Save configuration to XML
        Public Sub SaveConfig(config As Dictionary(Of String, String))
            Dim doc As New XmlDocument()
            Dim root As XmlElement = doc.CreateElement("Config")

            For Each kvp In config
                Dim setting As XmlElement = doc.CreateElement("Setting")
                setting.SetAttribute("name", kvp.Key)
                setting.InnerText = kvp.Value
                root.AppendChild(setting)
            Next

            doc.AppendChild(root)
            doc.Save(ConfigFilePath)
        End Sub

        ' Update App Configuration settings
        Public Sub UpdateSetting(settingName As String, value As String)
            Try
                ' If _doc wasn't loaded yet
                If _doc Is Nothing Then
                    _doc = New XmlDocument()
                    _doc.Load(ConfigFilePath)
                End If

                Dim settingNode As XmlNode = _doc.SelectSingleNode($"//Setting[@name='{settingName}']")
                If settingNode IsNot Nothing Then
                    settingNode.InnerText = value
                    _doc.Save(ConfigFilePath)
                Else
                    logger.Logger.LogWarning($"Setting '{settingName}' not found in config file.")
                End If
            Catch ex As Exception
                MessageBox.Show($"Failed to update {settingName}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                logger.Logger.LogError($"Failed to update {settingName}: {ex.Message}")
            End Try
        End Sub
        Public Sub UpdateDOJAHideUISetting(isChecked As Boolean)
            UpdateSetting("DOJAHideUI", isChecked.ToString().ToLower())
        End Sub
        Public Sub UpdateUseShaderGlassSetting(isChecked As Boolean)
            UpdateSetting("UseShaderGlass", isChecked.ToString().ToLower())
        End Sub
        Public Sub UpdateUseDialPadSetting(isChecked As Boolean)
            UpdateSetting("UseDialPad", isChecked.ToString().ToLower())
        End Sub
        Public Sub UpdateDojaStarHardwareRenderingSetting(isChecked As Boolean)
            UpdateSetting("DojaStarHardwareRendering", isChecked.ToString().ToLower())
        End Sub
        Public Sub UpdateDojaStarHighPerformanceEXESetting(isChecked As Boolean)
            UpdateSetting("DojaStarHighPerformanceEXE", isChecked.ToString().ToLower())
        End Sub
        Public Async Function UpdateFirstRunSettingAsync(value As String) As Task
            UpdateSetting("FirstRun", value)
        End Function
        Public Async Function UpdatePolicyAgreementSettingAsync(value As String) As Task
            UpdateSetting("PolicyAgreement", value)
        End Function
        Public Sub UpdateDOJASoundSetting(SoundType As String)
            UpdateSetting("DOJASoundType", SoundType)
        End Sub

        ' Update Network UID and Terminal ID settings
        Public Sub UpdateNetworkAndTerminalIDSetting(NewNetworkUID As String, NewTerminalID As String)
            Try
                Dim currentNetworkUID As String = MainForm.NetworkUID
                Dim currentTerminalID As String = MainForm.TerminalID
                Dim networkUID As String = If(String.IsNullOrWhiteSpace(NewNetworkUID), currentNetworkUID, NewNetworkUID.Trim())
                Dim terminalID As String = If(String.IsNullOrWhiteSpace(NewTerminalID), currentTerminalID, NewTerminalID.Trim())
                Dim lines As New List(Of String)

                ' Load existing config if present
                If File.Exists(MainForm.NetworkUIDTxtFile) Then
                    lines.AddRange(File.ReadAllLines(MainForm.NetworkUIDTxtFile))
                End If

                ' Ensure file has two lines
                While lines.Count < 2
                    lines.Add(String.Empty)
                End While

                ' Line 1 = Network UID
                lines(0) = networkUID

                ' Line 2 = Terminal ID
                lines(1) = terminalID

                File.WriteAllLines(MainForm.NetworkUIDTxtFile, lines)

                logger.Logger.LogInfo(
                    $"Updated NetworkUID from '{currentNetworkUID}' to '{networkUID}', " &
                    $"TerminalID from '{currentTerminalID}' to '{terminalID}'"
                )

            Catch ex As Exception
                MessageBox.Show(
                    $"Failed to update Network/Terminal ID settings: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                )
                logger.Logger.LogError(
                    $"Failed to update Network/Terminal ID settings: {ex.Message}"
                )
            End Try
        End Sub
    End Class
End Namespace