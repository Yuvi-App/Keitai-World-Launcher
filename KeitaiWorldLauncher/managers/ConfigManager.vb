Imports System.IO
Imports System.Xml

Namespace My.Managers
    Public Class ConfigManager
        Private Const ConfigFilePath As String = "configs/appconfig.xml"

        ' Default configuration values
        Private ReadOnly DefaultConfig As New Dictionary(Of String, String) From {
        {"VersionCheckURL", "latest_version.txt"},
        {"AutoUpdate", "true"},
        {"FirstRun", "true"},
        {"GamelistURL", "gamelist.xml"},
        {"AutoUpdateGameList", "true"},
        {"MachiCharalistURL", "machicharalist.xml"},
        {"AutoUpdateMachiCharaList", "true"},
        {"CharaDenlistURL", "charadenlist.xml"},
        {"AutoUpdateCharaDenList", "true"},
        {"UseShaderGlass", "true"},
        {"UseDialPad", "true"},
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
        {"FlashPlayerEXEPath", "JSKY.exe"},
        {"MachiCharaPath", "c:\machichara"},
        {"MachiCharaEXEPath", "machicharaemu.exe"},
        {"CharaDenPath", "c:\charaden"},
        {"CharaDenEXEPath", "CharaDenemu.exe"}
    }

        ' Load configuration from XML
        Public Async Function LoadConfigAsync() As Task(Of Dictionary(Of String, String))
            Return Await Task.Run(Function()
                                      Dim config As New Dictionary(Of String, String)(DefaultConfig)

                                      If IO.File.Exists(ConfigFilePath) Then
                                          Dim doc As New XmlDocument()
                                          doc.Load(ConfigFilePath)

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
        Public Sub UpdateDOJAHideUISetting(isChecked As Boolean)
            Try
                ' Load the XML document
                Dim doc As New XmlDocument()
                doc.Load(ConfigFilePath)

                ' Find the DOJAHideUI setting
                Dim settingNode As XmlNode = doc.SelectSingleNode("//Setting[@name='DOJAHideUI']")
                If settingNode IsNot Nothing Then
                    ' Update the value based on the checkbox state
                    settingNode.InnerText = isChecked.ToString().ToLower()

                    ' Save the updated XML file
                    doc.Save(ConfigFilePath)
                Else
                    'MessageBox.Show("DOJAHideUI setting not found in the configuration file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Catch ex As Exception
                MessageBox.Show($"Failed to update the DOJAHideUI setting: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                logger.Logger.LogError($"Failed to update the DOJAHideUI setting: {ex.Message}")
            End Try
        End Sub
        Public Sub UpdateUseShaderGlassSetting(isChecked As Boolean)
            Try
                ' Load the XML document
                Dim doc As New XmlDocument()
                doc.Load(ConfigFilePath)
                Dim settingNode As XmlNode = doc.SelectSingleNode("//Setting[@name='UseShadeGlass']")
                If settingNode IsNot Nothing Then
                    settingNode.InnerText = isChecked.ToString().ToLower()
                    doc.Save(ConfigFilePath)
                Else
                End If
            Catch ex As Exception
                MessageBox.Show($"Failed to update the UseShadeGlass setting: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                logger.Logger.LogError($"Failed to update the UseShadeGlass setting: {ex.Message}")
            End Try
        End Sub
        Public Sub UpdateUseDialPadSetting(isChecked As Boolean)
            Try
                ' Load the XML document
                Dim doc As New XmlDocument()
                doc.Load(ConfigFilePath)
                Dim settingNode As XmlNode = doc.SelectSingleNode("//Setting[@name='UseDialPad']")
                If settingNode IsNot Nothing Then
                    settingNode.InnerText = isChecked.ToString().ToLower()
                    doc.Save(ConfigFilePath)
                Else
                End If
            Catch ex As Exception
                MessageBox.Show($"Failed to update the UseDialPad setting: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                logger.Logger.LogError($"Failed to update the UseDialPad setting: {ex.Message}")
            End Try
        End Sub
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
        Public Async Function UpdateFirstRunSettingAsync(FirstRun As String) As Task
            Await Task.Run(Sub()
                               Try
                                   ' Load the XML document
                                   Dim doc As New XmlDocument()
                                   doc.Load(ConfigFilePath)
                                   Dim settingNode As XmlNode = doc.SelectSingleNode("//Setting[@name='FirstRun']")
                                   If settingNode IsNot Nothing Then
                                       settingNode.InnerText = FirstRun
                                       doc.Save(ConfigFilePath)
                                   End If
                               Catch ex As Exception
                                   MessageBox.Show($"Failed to update the FirstRun setting: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                   logger.Logger.LogError($"Failed to update the FirstRun setting: {ex.Message}")
                               End Try
                           End Sub)
        End Function
        Public Sub UpdateDOJASoundSetting(SoundType As String)
            Try
                ' Load the XML document
                Dim doc As New XmlDocument()
                doc.Load(ConfigFilePath)

                ' Find the DOJAHideUI setting
                Dim settingNode As XmlNode = doc.SelectSingleNode("//Setting[@name='DOJASoundType']")
                If settingNode IsNot Nothing Then
                    ' Update the value based on the checkbox state
                    settingNode.InnerText = SoundType

                    ' Save the updated XML file
                    doc.Save(ConfigFilePath)
                Else
                End If
            Catch ex As Exception
                MessageBox.Show($"Failed to update the DOJASoundType setting: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                logger.Logger.LogError($"Failed to update the DOJASoundType setting: {ex.Message}")
            End Try
        End Sub
    End Class
End Namespace