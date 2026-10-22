Imports System.Data.OleDb
Imports System.Security.Cryptography
Imports AutoRent.Common.Utilities
Imports AutoRent.Data.Context

Namespace Data.Repositories
    Public NotInheritable Class UserRepository
        Private Sub New()
        End Sub

        Public Shared Function HasAnyUsers() As Boolean
            Using connection = AccessDatabaseConnection.CreateConnection()
                connection.Open()

                Using command = connection.CreateCommand()
                    command.CommandText = "SELECT COUNT(*) FROM [Users]"
                    Return Convert.ToInt32(command.ExecuteScalar()) > 0
                End Using
            End Using
        End Function

        Public Shared Function SeedAdminIfEmpty() As String
            If HasAnyUsers() Then Return Nothing

            Dim generatedPassword = Convert.ToHexString(RandomNumberGenerator.GetBytes(24))
            Dim passwordHash = PasswordHasher.HashPassword(generatedPassword)

            Using connection = AccessDatabaseConnection.CreateConnection()
                connection.Open()

                Using transaction = connection.BeginTransaction()
                    Using countCommand = connection.CreateCommand()
                        countCommand.Transaction = transaction
                        countCommand.CommandText = "SELECT COUNT(*) FROM [Users]"
                        If Convert.ToInt32(countCommand.ExecuteScalar()) > 0 Then
                            transaction.Rollback()
                            Return Nothing
                        End If
                    End Using

                    Using insertCommand = connection.CreateCommand()
                        insertCommand.Transaction = transaction
                        insertCommand.CommandText = "INSERT INTO [Users] ([username], [password_hash], [role], [is_active], [is_deleted], [last_login], [created_at]) VALUES (?, ?, ?, ?, ?, ?, ?)"
                        insertCommand.Parameters.Add("username", OleDbType.VarWChar, 100).Value = "admin"
                        insertCommand.Parameters.Add("password_hash", OleDbType.VarWChar, 255).Value = passwordHash
                        insertCommand.Parameters.Add("role", OleDbType.VarWChar, 50).Value = "Admin"
                        insertCommand.Parameters.Add("is_active", OleDbType.Boolean).Value = True
                        insertCommand.Parameters.Add("is_deleted", OleDbType.Boolean).Value = False
                        insertCommand.Parameters.Add("last_login", OleDbType.Date).Value = DBNull.Value
                        insertCommand.Parameters.Add("created_at", OleDbType.Date).Value = DateTime.Now
                        insertCommand.ExecuteNonQuery()
                    End Using

                    transaction.Commit()
                End Using
            End Using

            Return generatedPassword
        End Function
    End Class
End Namespace
