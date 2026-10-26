Imports System.Data.OleDb
Imports System.Security.Cryptography
Imports AutoRent.Common.Utilities
Imports AutoRent.Data.Context
Imports AutoRent.Data.Models

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

        Public Shared Function Authenticate(username As String, password As String) As User
            If String.IsNullOrWhiteSpace(username) OrElse String.IsNullOrEmpty(password) Then Return Nothing

            Dim userId As Integer
            Dim storedUsername As String
            Dim passwordHash As String
            Dim roleName As String
            Dim isActive As Boolean
            Dim isDeleted As Boolean
            Dim createdAt As DateTime

            Using connection = AccessDatabaseConnection.CreateConnection()
                connection.Open()

                Using command = connection.CreateCommand()
                    command.CommandText = "SELECT [user_id], [username], [password_hash], [role], [is_active], [is_deleted], [created_at] FROM [Users] WHERE [username] = ?"
                    command.Parameters.Add("username", OleDbType.VarWChar, 100).Value = username.Trim()

                    Using reader = command.ExecuteReader()
                        If Not reader.Read() Then Return Nothing

                        userId = Convert.ToInt32(reader("user_id"))
                        storedUsername = Convert.ToString(reader("username"))
                        passwordHash = Convert.ToString(reader("password_hash"))
                        roleName = Convert.ToString(reader("role"))
                        isActive = Convert.ToBoolean(reader("is_active"))
                        isDeleted = Convert.ToBoolean(reader("is_deleted"))
                        createdAt = Convert.ToDateTime(reader("created_at"))
                    End Using
                End Using
            End Using

            If Not PasswordHasher.VerifyPassword(password, passwordHash) OrElse isDeleted Then Return Nothing

            Dim userRole As UserRole
            If Not System.Enum.TryParse(Of UserRole)(roleName, True, userRole) OrElse Not System.Enum.IsDefined(GetType(UserRole), userRole) Then Return Nothing

            Dim loginTime = DateTime.Now
            Using connection = AccessDatabaseConnection.CreateConnection()
                connection.Open()

                Using command = connection.CreateCommand()
                    command.CommandText = "UPDATE [Users] SET [last_login] = ?, [is_active] = True WHERE [user_id] = ? AND [is_deleted] = False"
                    command.Parameters.Add("last_login", OleDbType.Date).Value = loginTime
                    command.Parameters.Add("user_id", OleDbType.Integer).Value = userId
                    If command.ExecuteNonQuery() <> 1 Then Return Nothing
                End Using
            End Using

            Return New User With {
                .UserId = userId,
                .Username = storedUsername,
                .PasswordHash = passwordHash,
                .Role = userRole,
                .IsActive = True,
                .IsDeleted = isDeleted,
                .LastLogin = loginTime,
                .CreatedAt = createdAt
            }
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

        Public Shared Sub RecordLogout(userId As Integer)
            If userId <= 0 Then Return
            Try
                Using connection = AccessDatabaseConnection.CreateConnection()
                    connection.Open()
                    Using command = connection.CreateCommand()
                        command.CommandText = "UPDATE [Users] SET [is_active] = False WHERE [user_id] = ?"
                        command.Parameters.Add("user_id", OleDbType.Integer).Value = userId
                        command.ExecuteNonQuery()
                    End Using
                End Using
            Catch ex As Exception
            End Try
        End Sub
    End Class
End Namespace
