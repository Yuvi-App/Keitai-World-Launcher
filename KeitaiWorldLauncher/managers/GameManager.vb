Imports System.IO
Imports ReaLTaiizor.Controls
Imports ReaLTaiizor.Forms

Namespace My.Managers
    Public Class GameManager
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
    End Class
End Namespace

