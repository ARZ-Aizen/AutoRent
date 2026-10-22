Imports System.Data.OleDb
Imports System.IO

Namespace Data.Context
    Public NotInheritable Class AccessDatabaseConnection
        Private Sub New()
        End Sub

        Public Shared Function CreateConnection() As OleDbConnection
            Dim databasePath = Path.Combine(AppContext.BaseDirectory, "Resources", "Data", "AutoRent.accdb.accdb")

            If Not File.Exists(databasePath) Then
                Throw New FileNotFoundException("The AutoRent Access database could not be found.", databasePath)
            End If

            Dim connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={databasePath};Persist Security Info=False;"
            Return New OleDbConnection(connectionString)
        End Function
    End Class
End Namespace
