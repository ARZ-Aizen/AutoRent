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
            Dim userCode As String = Nothing
            Dim storedUsername As String = Nothing
            Dim storedFullName As String = Nothing
            Dim storedBranch As String = Nothing
            Dim passwordHash As String = Nothing
            Dim roleName As String = Nothing
            Dim isActive As Boolean
            Dim isDeleted As Boolean
            Dim createdAt As DateTime
            Dim userFound As Boolean = False

            Using connection = AccessDatabaseConnection.CreateConnection()
                connection.Open()

                Using command = connection.CreateCommand()
                    command.CommandText = "SELECT [user_id], [user_code], [username], [full_name], [branch], [password_hash], [role], [is_active], [is_deleted], [created_at] FROM [Users] WHERE [username] = ?"
                    command.Parameters.Add("username", OleDbType.VarWChar, 100).Value = username.Trim()

                    Using reader = command.ExecuteReader()
                        While reader.Read()
                            Dim candidateUsername = Convert.ToString(reader("username"))
                            ' Strict case-sensitive username validation:
                            ' MS Access OleDb queries are case-insensitive by default (e.g. 'admin' matches 'Admin' or 'ADMIN').
                            ' An exact ordinal match (StringComparison.Ordinal) is strictly required so that capital
                            ' letters in a username reject small caps, and lowercase usernames reject uppercase variants.
                            If String.Equals(username.Trim(), candidateUsername, StringComparison.Ordinal) Then
                                userId = Convert.ToInt32(reader("user_id"))
                                userCode = If(reader("user_code") Is DBNull.Value OrElse String.IsNullOrWhiteSpace(Convert.ToString(reader("user_code"))), $"USR-{userId:D5}", Convert.ToString(reader("user_code")).Trim())
                                storedUsername = candidateUsername
                                storedFullName = If(reader("full_name") Is DBNull.Value OrElse String.IsNullOrWhiteSpace(Convert.ToString(reader("full_name"))), storedUsername, Convert.ToString(reader("full_name")).Trim())
                                storedBranch = If(reader("branch") Is DBNull.Value OrElse String.IsNullOrWhiteSpace(Convert.ToString(reader("branch"))), "HQ", Convert.ToString(reader("branch")).Trim())
                                passwordHash = Convert.ToString(reader("password_hash"))
                                roleName = Convert.ToString(reader("role"))
                                isActive = Convert.ToBoolean(reader("is_active"))
                                isDeleted = Convert.ToBoolean(reader("is_deleted"))
                                createdAt = Convert.ToDateTime(reader("created_at"))
                                userFound = True
                                Exit While
                            End If
                        End While
                    End Using
                End Using
            End Using

            If Not userFound Then Return Nothing

            If Not PasswordHasher.VerifyPassword(password, passwordHash) OrElse isDeleted Then Return Nothing

            Dim userRole As UserRole = UserRole.User
            If roleName.IndexOf("Admin", StringComparison.OrdinalIgnoreCase) >= 0 Then
                userRole = UserRole.Admin
            ElseIf roleName.IndexOf("Manager", StringComparison.OrdinalIgnoreCase) >= 0 OrElse roleName.IndexOf("Owner", StringComparison.OrdinalIgnoreCase) >= 0 Then
                userRole = UserRole.Manager
            Else
                userRole = UserRole.User
            End If

            Dim loginTime = DateTime.Now
            Dim needsRehash = Not PasswordHasher.IsOpaqueHash(passwordHash)
            Dim upgradedHash = If(needsRehash, PasswordHasher.HashPassword(password), passwordHash)

            Using connection = AccessDatabaseConnection.CreateConnection()
                connection.Open()

                Using command = connection.CreateCommand()
                    If needsRehash Then
                        command.CommandText = "UPDATE [Users] SET [last_login] = ?, [is_active] = True, [password_hash] = ? WHERE [user_id] = ? AND [is_deleted] = False"
                        command.Parameters.Add("last_login", OleDbType.Date).Value = loginTime
                        command.Parameters.Add("password_hash", OleDbType.VarWChar, 255).Value = upgradedHash
                        command.Parameters.Add("user_id", OleDbType.Integer).Value = userId
                    Else
                        command.CommandText = "UPDATE [Users] SET [last_login] = ?, [is_active] = True WHERE [user_id] = ? AND [is_deleted] = False"
                        command.Parameters.Add("last_login", OleDbType.Date).Value = loginTime
                        command.Parameters.Add("user_id", OleDbType.Integer).Value = userId
                    End If
                    If command.ExecuteNonQuery() <> 1 Then Return Nothing
                End Using
            End Using

            Return New User With {
                .UserId = userId,
                .UserCode = userCode,
                .Username = storedUsername,
                .FullName = storedFullName,
                .Branch = storedBranch,
                .PasswordHash = upgradedHash,
                .Role = userRole,
                .RoleName = roleName,
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

                    Dim rng As New Random()
                    Dim adminUserId As Integer = rng.Next(10000, 100000)
                    Dim adminUserCode As String = $"USR-{adminUserId:D5}"

                    Using insertCommand = connection.CreateCommand()
                        insertCommand.Transaction = transaction
                        insertCommand.CommandText = "INSERT INTO [Users] ([user_id], [user_code], [username], [password_hash], [role], [is_active], [is_deleted], [last_login], [created_at]) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)"
                        insertCommand.Parameters.Add("user_id", OleDbType.Integer).Value = adminUserId
                        insertCommand.Parameters.Add("user_code", OleDbType.VarWChar, 20).Value = adminUserCode
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
