Imports System.IO
Imports System.Xml
Imports System.Xml.Linq

Namespace My.Managers
    Public Class GameInfoManager
        Public Shared Function LoadFields(infoXmlPath As String) As IReadOnlyList(Of KeyValuePair(Of String, String))
            Dim fields As New List(Of KeyValuePair(Of String, String))()
            If Not File.Exists(infoXmlPath) Then Return fields

            Dim settings As New XmlReaderSettings With {
                .DtdProcessing = DtdProcessing.Prohibit,
                .XmlResolver = Nothing
            }
            Using reader = XmlReader.Create(infoXmlPath, settings)
                Dim document = XDocument.Load(reader)
                If document.Root Is Nothing OrElse document.Root.Name.LocalName <> "game" Then
                    Throw New InvalidDataException("Game information must have a <game> root element.")
                End If

                ' Discover fields from the document, Keep order.
                For Each element In document.Root.Descendants().Where(Function(node) Not node.HasElements)
                    Dim names = element.AncestorsAndSelf().Reverse().Skip(1).
                        Select(Function(node) node.Name.LocalName)
                    fields.Add(New KeyValuePair(Of String, String)(String.Join(" / ", names), element.Value.Trim()))
                Next
            End Using
            Return fields
        End Function
    End Class
End Namespace
