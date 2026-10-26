Imports System.Data.OleDb
Imports System.IO

Namespace Data.Context
    Public NotInheritable Class AccessDatabaseConnection
        Private Sub New()
        End Sub

        Public Shared Function GetDatabasePath() As String
            Dim projectSourcePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Resources", "Data", "AutoRent.accdb.accdb"))
            If File.Exists(projectSourcePath) Then
                Return projectSourcePath
            End If

            Dim binPath = Path.Combine(AppContext.BaseDirectory, "Resources", "Data", "AutoRent.accdb.accdb")
            If File.Exists(binPath) Then
                Return binPath
            End If

            Dim repoPath = "c:\Users\allan\source\repos\AutoRent\AutoRent\Resources\Data\AutoRent.accdb.accdb"
            If File.Exists(repoPath) Then
                Return repoPath
            End If

            Throw New FileNotFoundException("The AutoRent Access database could not be found.", projectSourcePath)
        End Function

        Public Shared Function CreateConnection() As OleDbConnection
            Dim databasePath = GetDatabasePath()
            Dim connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={databasePath};Persist Security Info=False;"
            Return New OleDbConnection(connectionString)
        End Function
    End Class
End Namespace
