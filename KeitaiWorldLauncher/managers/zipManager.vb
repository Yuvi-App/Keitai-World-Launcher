Imports System.IO
Imports System.IO.Compression

Namespace My.Managers
    Public Class ZipManager
        Public Shared Sub ExtractZip(zipFilePath As String, destinationFolder As String)
            Try
                ' Get the name of the ZIP file without the extension
                Dim zipName As String = Path.GetFileNameWithoutExtension(zipFilePath)

                ' Create the destination folder using the ZIP file name
                Dim extractPath As String = Path.Combine(destinationFolder, zipName)

                ' Ensure the folder doesn't already exist
                If Not Directory.Exists(extractPath) Then
                    Directory.CreateDirectory(extractPath)
                End If

                ' Extract the ZIP file to the folder
                ZipFile.ExtractToDirectory(zipFilePath, extractPath)

                MessageBox.Show($"Extracted to: {extractPath}", "Extraction Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show($"Failed to extract the ZIP file: {ex.Message}", "Extraction Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
    End Class
End Namespace