Imports System.IO
Imports System.Xml
Imports KeitaiWorldLauncher.My.logger
Imports ReaLTaiizor.Controls

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
        {"UseShaderGlass", "true"},
        {"DOJAPath", "c:\doja"},
        {"DOJAEXEPath", "doja.exe"},
        {"DOJAHideUI", "true"},
        {"DOJASoundType", "standard"},
        {"STARPath", "c:\star"},
        {"STAREXEPath", "star.exe"},
        {"JSKYPath", "c:\JSKY"},
        {"JSKYEXEPath", "JSKY.exe"},
        {"FlashPlayerPath", "c:\flashplayer"},
        {"FlashPlayerEXEPath", "JSKY.exe"},
        {"MachiCharaPath", "c:\machichara"},
        {"MachiCharaEXEPath", "machicharaemu.exe"}
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
        Public Sub UpdateNetworkUIDSetting(NewNetworkUID As String)
            Try
                Dim CurrentNetworkUID = Form1.NetworkUID
                NewNetworkUID = NewNetworkUID.Trim
                File.WriteAllText(Form1.NetworkUIDTxtFile, NewNetworkUID)
                logger.Logger.LogInfo($"Updated NetworkUID from {CurrentNetworkUID} to {NewNetworkUID}")
            Catch ex As Exception
                MessageBox.Show($"Failed to update the NetworkUID setting: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                logger.Logger.LogError($"Failed to update the NetworkUID setting: {ex.Message}")
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