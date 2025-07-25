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
        Public Async Function FF7_DOCLE_SetupAsync(DojaPath As String, GamePath As String) As Task(Of Boolean)
            logger.Logger.LogInfo("Starting FF7_DOCLE Helper Function")

            Dim MultimediaFolder As String = Path.Combine(DojaPath, "lib", "multimedia")
            Dim myPictureFolder As String = Path.Combine(MultimediaFolder, "mypicture")
            Dim gifSourceFolder As String = Path.Combine(GamePath, "DoCLE Gifs")
            Dim requiredGif As String = Path.Combine(gifSourceFolder, "Image0000000A.gif")
            Dim targetGif As String = Path.Combine(myPictureFolder, "Image0000000A.gif")

            ' Validate required GIF exists
            If Not File.Exists(requiredGif) Then
                logger.Logger.LogError($"Error FF7_DOCLE Helper Function: Missing GIFS{vbCrLf}{requiredGif}")
                Return False
            End If

            ' Skip if already copied
            If File.Exists(targetGif) Then
                Return True
            End If

            Try
                Await Task.Run(Sub()
                                   ' Clean and prepare destination folder
                                   DeleteDirectoryContents(MultimediaFolder)
                                   Directory.CreateDirectory(myPictureFolder)

                                   ' Copy all GIFs
                                   For Each gifPath As String In Directory.GetFiles(gifSourceFolder)
                                       Dim fileName As String = Path.GetFileName(gifPath)
                                       File.Copy(gifPath, Path.Combine(myPictureFolder, fileName))
                                   Next
                               End Sub)

                logger.Logger.LogInfo("Completed FF7_DOCLE Helper Function")

                MessageBox.Show(
            "FF7_DOCLE will ask you to select images. Once it does you will need to select the images from the File Explorer that will pop up." &
            $"{vbCrLf}For Example: Select Image File 1 = Image00000001.gif{vbCrLf}Select Image File 10 = Image0000000A.gif",
            "FF7_DOCLE Setup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)

                Return True

            Catch ex As Exception
                logger.Logger.LogError($"Error FF7_DOCLE Helper Function{vbCrLf}{ex}")
                Return False
            End Try
        End Function

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
        Public Sub DeleteDirectoryContents(inputDirectory As String)
            If Directory.Exists(inputDirectory) Then
                ' Delete all files in the directory
                For Each f As String In Directory.GetFiles(inputDirectory)
                    Try
                        File.Delete(f)
                    Catch ex As Exception
                        logger.Logger.LogError($"Error deleting file {f}: {ex.Message}")
                    End Try
                Next

                ' Delete all subdirectories recursively
                For Each dir As String In Directory.GetDirectories(inputDirectory)
                    Try
                        Directory.Delete(dir, True) ' True ensures recursive deletion
                    Catch ex As Exception
                        logger.Logger.LogError($"Error deleting directory {dir}: {ex.Message}")
                    End Try
                Next
            Else
                Console.WriteLine("Directory does not exist.")
            End If
        End Sub
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

