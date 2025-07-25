Imports System.IO
Imports System.Text
Imports System.Xml.Serialization
Imports KeitaiWorldLauncher.My.models

Namespace My.Managers
    Public Class MachiCharaListManager
        Private Const MachiCharaListPath As String = "configs/machicharalist.xml"

        Public Shared Async Function DownloadMachiCharaListAsync(url As String) As Task
            Try
                Using client As New Net.Http.HttpClient()
                    Dim fileBytes As Byte() = Await client.GetByteArrayAsync(url)
                    Await File.WriteAllBytesAsync(MachiCharaListPath, fileBytes)
                    'MessageBox.Show("Updated Machi Chara list downloaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End Using
            Catch ex As Exception
                MessageBox.Show(owner:=SplashScreen, $"Failed to download Machi Chara list: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Function


        ' Load MachiChara from XML
        Public Function LoadMachiChara() As List(Of MachiChara)
            If File.Exists(MachiCharaListPath) Then
                Try
                    Dim serializer As New XmlSerializer(GetType(List(Of MachiChara)), New XmlRootAttribute("MachiCharalist"))
                    Using reader As New StreamReader(MachiCharaListPath, Encoding.GetEncoding("Shift-JIS"))
                        Return CType(serializer.Deserialize(reader), List(Of MachiChara))
                    End Using
                Catch ex As Exception
                    logger.Logger.LogError("Failed to load MachiCharaList.XML...")
                End Try

            Else
                ' Return an empty list if the file doesn't exist
                Return New List(Of MachiChara)()
            End If
        End Function

        ' Save games to XML with Shift-JIS encoding
        Public Sub SaveMachiChara(MCs As List(Of MachiChara))
            Dim serializer As New XmlSerializer(GetType(List(Of MachiChara)), New XmlRootAttribute("MachiCharalist"))
            Using writer As New StreamWriter(MachiCharaListPath, False, Encoding.GetEncoding("Shift-JIS"))
                serializer.Serialize(writer, MCs)
            End Using
        End Sub

        ' Add a new MachiChara to the list
        Public Sub AddMachiChara(MC As MachiChara)
            Dim MCs = LoadMachiChara()
            MCs.Add(MC)
            SaveMachiChara(MCs)
        End Sub

    End Class
End Namespace

