Imports System.IO
Imports System.IO.Compression

Namespace My.Managers
    Public Class ZipManager
        Public Shared Async Function ExtractZipAsync(zipFilePath As String, destinationFolder As String) As Task
            Try
                ' Get the name of the ZIP file without the extension
                Dim zipName As String = Path.GetFileNameWithoutExtension(zipFilePath)

                ' Create the destination folder using the ZIP file name
                Dim extractPath As String = Path.Combine(destinationFolder, zipName)

                ' Ensure the folder exists
                If Not Directory.Exists(extractPath) Then
                    Directory.CreateDirectory(extractPath)
                End If

                ' Run extraction on background thread
                Await Task.Run(Sub()
                                   ZipFile.ExtractToDirectory(zipFilePath, extractPath)
                               End Sub)

                MessageBox.Show($"Extracted to: {extractPath}", "Extraction Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Catch ex As Exception
                MessageBox.Show($"Failed to extract the ZIP file: {ex.Message}", "Extraction Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Function

    End Class
End Namespace