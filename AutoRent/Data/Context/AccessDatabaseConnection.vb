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

            ' Portable fallback for cloned projects and published builds.
            ' Never depend on a particular developer's Windows username.
            Dim searchFolder As DirectoryInfo = New DirectoryInfo(AppContext.BaseDirectory)
            For depth As Integer = 0 To 5
                If searchFolder Is Nothing Then Exit For
                Dim candidate = Path.Combine(searchFolder.FullName, "Resources", "Data", "AutoRent.accdb.accdb")
                If File.Exists(candidate) Then Return candidate
                searchFolder = searchFolder.Parent
            Next

            Throw New FileNotFoundException(
                "The AutoRent Access database could not be found. Verify Resources\Data\AutoRent.accdb.accdb beside your project.",
                projectSourcePath)
        End Function

        Public Shared Function CreateConnection() As OleDbConnection
            Dim databasePath = GetDatabasePath()
            Dim connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={databasePath};Persist Security Info=False;"
            Return New OleDbConnection(connectionString)
        End Function
    End Class
End Namespace
