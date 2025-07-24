Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Xml.Serialization
Imports KeitaiWorldLauncher.My.Models

Namespace My.Managers
    Public Class CharaDenListManager
        Private Const CharaDenListPath As String = "configs/charadenlist.xml"

        Public Shared Async Function DownloadCharaDenListAsync(url As String) As Task
            Try
                Using client As New Net.Http.HttpClient()
                    Dim fileBytes As Byte() = Await client.GetByteArrayAsync(url)
                    Await File.WriteAllBytesAsync(CharaDenListPath, fileBytes)
                End Using
            Catch ex As Exception
                MessageBox.Show(owner:=SplashScreen, $"Failed to download Chara-den list: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Function


        ' Load CharaDen from XML
        Public Function LoadCharaden() As List(Of CharaDen)
            If File.Exists(CharaDenListPath) Then
                Try
                    Dim serializer As New XmlSerializer(GetType(List(Of CharaDen)), New XmlRootAttribute("CharaDenList"))
                    Using reader As New StreamReader(CharaDenListPath, Encoding.GetEncoding("Shift-JIS"))
                        Return CType(serializer.Deserialize(reader), List(Of CharaDen))
                    End Using
                Catch ex As Exception
                    logger.Logger.LogError("Failed to load CharaDenList.xml...")
                End Try

            Else
                ' Return an empty list if the file doesn't exist
                Return New List(Of CharaDen)()
            End If
        End Function

        ' Save games to XML with Shift-JIS encoding
        Public Sub SaveCharaDen(CDs As List(Of CharaDen))
            Dim serializer As New XmlSerializer(GetType(List(Of CharaDen)), New XmlRootAttribute("CharaDenList"))
            Using writer As New StreamWriter(CharaDenListPath, False, Encoding.GetEncoding("Shift-JIS"))
                serializer.Serialize(writer, CDs)
            End Using
        End Sub

        ' Add a new CharaDen to the list
        Public Sub AddCharaDen(CD As CharaDen)
            Dim CDs = LoadCharaden()
            CDs.Add(CD)
            SaveCharaDen(CDs)
        End Sub

    End Class
End Namespace

