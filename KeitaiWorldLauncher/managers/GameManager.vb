Imports System.IO
Imports System.IO.Compression

Namespace My.Managers
    Public Class GameManager
        'Games that cannot have there Jam Updated for reasons
        Public Function NoUpdateJAMGames()
            Dim NoUpdateJAMGameslist As New List(Of String) From {"One_Piece_Mugiwara_Wars"}
            Return NoUpdateJAMGameslist
        End Function

        'Game Specific Functions
        Public Async Function BomberManPuzzleSpecial_ExportStage(spfilepath As String, stagenum As Integer) As Task(Of String)
            Using br As BinaryReader = New BinaryReader(File.Open(spfilepath, FileMode.Open))
                br.BaseStream.Position = 4
                Dim offset As Long
                If br.ReadByte = &HFF Then
                    ' Skip header
                    offset = &H9F + (147 * (stagenum - 1))
                Else
                    offset = &H5F + (147 * (stagenum - 1))
                End If
                br.BaseStream.Position = offset

                Dim StageData As Byte() = br.ReadBytes(147)
                Dim CompressedString As String = CompressAndEncode(StageData)
                Return CompressedString
            End Using
        End Function
        Public Async Function BomberManPuzzleSpecial_ImportStage(spfilepath As String, stagenum As Integer, inputstring As String) As Task(Of Boolean)
            Try
                Dim Decodedbytes = DecodeAndDecompress(inputstring)
                If Decodedbytes.Length = 147 Then
                    Using bw As BinaryWriter = New BinaryWriter(File.Open(spfilepath, FileMode.Open))
                        bw.BaseStream.Position = 4
                        Dim offset As Long
                        If bw.BaseStream.ReadByte = &HFF Then
                            ' Skip header
                            offset = &H9F + (147 * (stagenum - 1))
                        Else
                            offset = &H5F + (147 * (stagenum - 1))
                        End If
                        bw.BaseStream.Position = offset
                        bw.Write(Decodedbytes)
                        Return True
                    End Using
                Else
                    MessageBox.Show("Error decoding string, Please try again...")
                    logger.Logger.LogError($"Error decoding Input String for Bomberman Puzzle Speical: {inputstring}")
                    Return False
                End If
            Catch ex As Exception
                Return False
            End Try
        End Function

        ' Helper Functions
        Function CompressAndEncode(bytes() As Byte) As String
            Using ms As New MemoryStream()
                Using ds As New DeflateStream(ms, CompressionLevel.Optimal, True)
                    ds.Write(bytes, 0, bytes.Length)
                End Using
                Return Convert.ToBase64String(ms.ToArray())
            End Using
        End Function
        Function DecodeAndDecompress(base64 As String) As Byte()
            Dim compressed = Convert.FromBase64String(base64)
            Using ms = New MemoryStream(compressed)
                Using ds = New DeflateStream(ms, CompressionMode.Decompress)
                    Using output As New MemoryStream()
                        ds.CopyTo(output)
                        Return output.ToArray()
                    End Using
                End Using
            End Using
        End Function
    End Class
End Namespace

