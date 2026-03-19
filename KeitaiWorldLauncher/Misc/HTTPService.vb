Imports System.Net.Http

Namespace My.Managers
    Public Module HttpService
        Public ReadOnly Http As New HttpClient() With {
            .Timeout = TimeSpan.FromSeconds(30)
        }
    End Module
End Namespace