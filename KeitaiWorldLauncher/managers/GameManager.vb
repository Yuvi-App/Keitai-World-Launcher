Imports System.IO
Imports ReaLTaiizor.Controls
Imports ReaLTaiizor.Forms

Namespace My.Managers
    Public Class GameManager
        'Game Specific Functions
        Public Function FF7_DOCLE_Setup(DojaPath As String, GamePath As String)
            logger.Logger.LogInfo($"Starting FF7_DOCLE Helper Function")
            Dim MultimediaFolder = Path.Combine(DojaPath, "lib", "multimedia")
            Dim myPictureFolder = Path.Combine(MultimediaFolder, "mypicture")

            'Check if they Already exist
            If File.Exists($"{Path.Combine(GamePath, "DoCLE Gifs")}\Image0000000A.gif") = False Then
                logger.Logger.LogError($"Error FF7_DOCLE Helper Function: Missing GIFS{vbCrLf}" & $"{Path.Combine(GamePath, "DoCLE Gifs")}\Image0000000A.gif")
                Return False
            End If
            If File.Exists($"{myPictureFolder}\Image0000000A.gif") Then
                Return True
            End If

            'Delete MultiMedia Contens
            Try
                DeleteDirectoryContents(MultimediaFolder)
                Directory.CreateDirectory(myPictureFolder)

                'Copy Contents to it
                Dim GIFLocation = Path.Combine(GamePath, "DoCLE Gifs")
                For Each Fi In Directory.GetFiles(GIFLocation)
                    File.Copy(Fi, Path.Combine(myPictureFolder, Path.GetFileName(Fi)))
                Next
                logger.Logger.LogInfo($"Completed FF7_DOCLE Helper Function")
                MessageBox.Show($"FF7_DOCLE will ask you to select images. Once it does you will need to select the images from the File explorer that will pop up.{vbCrLf}For Example: Select Image File 1 = Image00000001.gif{vbCrLf}Select Image File 10 = Image0000000A.gif")
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

