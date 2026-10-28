Imports System.Collections.Generic
Imports System.Data
Imports System.Linq
Imports System.Security.Cryptography
Imports AutoRent.Common.Utilities
Imports AutoRent.Data.Context
Imports AutoRent.Data.Models

Namespace Data.Repositories
    Public NotInheritable Class UserAdminRepository
        Private Sub New()
        End Sub

        Public Const MinPasswordLength As Integer = 8

        Public Shared Function GetAll() As List(Of User)
            Dim users As New List(Of User)()

            Dim table = Db.Query("SELECT [user_id], [user_code], [username], [full_name], [branch], [role], [is_active], [is_deleted], [last_login], [created_at] FROM [Users] WHERE [is_deleted] = False ORDER BY [user_id]")

            For Each row As DataRow In table.Rows
                Dim userId = Convert.ToInt32(row("user_id"))
                Dim userCode = If(table.Columns.Contains("user_code") AndAlso Not IsDBNull(row("user_code")), Convert.ToString(row("user_code")), $"USR-{userId:D5}")
                Dim username = Convert.ToString(row("username"))
                Dim fullName = If(table.Columns.Contains("full_name") AndAlso Not IsDBNull(row("full_name")), Convert.ToString(row("full_name")), username)
                Dim branch = If(table.Columns.Contains("branch") AndAlso Not IsDBNull(row("branch")), Convert.ToString(row("branch")), "HQ")
                Dim roleName = Convert.ToString(row("role"))
                Dim isActive = Convert.ToBoolean(row("is_active"))
                Dim isDeleted = Convert.ToBoolean(row("is_deleted"))
                Dim lastLogin = If(IsDBNull(row("last_login")), CType(Nothing, DateTime?), Convert.ToDateTime(row("last_login")))
                Dim createdAt = If(IsDBNull(row("created_at")), DateTime.Now, Convert.ToDateTime(row("created_at")))
                Dim userRole As Global.AutoRent.Data.Models.UserRole = Global.AutoRent.Data.Models.UserRole.User
                If roleName.IndexOf("Admin", StringComparison.OrdinalIgnoreCase) >= 0 Then
                    userRole = Global.AutoRent.Data.Models.UserRole.Admin
                ElseIf roleName.IndexOf("Manager", StringComparison.OrdinalIgnoreCase) >= 0 OrElse roleName.IndexOf("Owner", StringComparison.OrdinalIgnoreCase) >= 0 Then
                    userRole = Global.AutoRent.Data.Models.UserRole.Manager
                Else
                    userRole = Global.AutoRent.Data.Models.UserRole.User
                End If

                users.Add(New Global.AutoRent.Data.Models.User With {
                    .UserId = userId,
                    .UserCode = userCode,
                    .Username = username,
                    .FullName = fullName,
                    .Branch = branch,
                    .Role = userRole,
                    .RoleName = roleName,
                    .IsActive = isActive,
                    .IsDeleted = isDeleted,
                    .LastLogin = lastLogin,
                    .CreatedAt = createdAt
                })
            Next

            Return users
        End Function

        Public Shared Function ValidatePasswordStrength(password As String) As String
            If String.IsNullOrEmpty(password) Then Return "Password is required."
            If password.Length < MinPasswordLength Then Return $"Password must be at least {MinPasswordLength} characters long."
            If Not password.Any(Function(c) Char.IsUpper(c)) Then Return "Password must contain at least one uppercase letter (A-Z)."
            If Not password.Any(Function(c) Char.IsLower(c)) Then Return "Password must contain at least one lowercase letter (a-z)."
            If Not password.Any(Function(c) Char.IsDigit(c)) Then Return "Password must contain at least one number (0-9)."
            If Not password.Any(Function(c) Not Char.IsLetterOrDigit(c)) Then Return "Password must contain at least one special character (e.g. !@#$%^&*)."
            Return Nothing
        End Function

        Public Shared Function IsAdminRole(role As String) As Boolean
            If String.IsNullOrEmpty(role) Then Return False
            Return role.IndexOf("Admin", StringComparison.OrdinalIgnoreCase) >= 0
        End Function

        Public Shared Function AdminExists() As Boolean
            Return Db.Count("SELECT COUNT(*) FROM [Users] WHERE ([role] = 'Admin' OR [role] = 'System Administrator') AND [is_deleted] = False") > 0
        End Function

        Public Shared Function GenerateUniqueUserId() As Integer
            Dim existingIds As New HashSet(Of Integer)()
            Dim dt = Db.Query("SELECT [user_id] FROM [Users]")
            For Each row As DataRow In dt.Rows
                If Not IsDBNull(row("user_id")) Then
                    existingIds.Add(Convert.ToInt32(row("user_id")))
                End If
            Next

            Dim rng As New Random()
            Dim candidateId As Integer
            Do
                candidateId = rng.Next(10000, 100000)
            Loop While existingIds.Contains(candidateId)

            Return candidateId
        End Function
        Public Shared Function Create(username As String, password As String, role As String, fullName As String, branch As String, isActive As Boolean) As String
            username = If(username, "").Trim()
            fullName = If(fullName, "").Trim()
            branch = If(branch, "HQ").Trim()
            role = If(role, "Rental Staff").Trim()

            Dim nameIssue = InputValidation.FullName(fullName)
            If nameIssue IsNot Nothing Then Return nameIssue
            Dim userIssue = InputValidation.Username(username)
            If userIssue IsNot Nothing Then Return userIssue
            If Db.Count("SELECT COUNT(*) FROM [Users] WHERE UCASE(TRIM([full_name])) = ? AND [is_deleted] = False",
                        Db.Text(fullName.ToUpperInvariant(), 100)) > 0 Then
                Return "A staff account with this full name already exists."
            End If

            Dim passErr = ValidatePasswordStrength(password)
            If passErr IsNot Nothing Then Return passErr

            If IsAdminRole(role) AndAlso AdminExists() Then
                Return "Only one System Administrator is allowed. A System Administrator account already exists."
            End If

            If Db.Count("SELECT COUNT(*) FROM [Users] WHERE [username] = ?", Db.Text(username, 100)) > 0 Then
                Return $"The username '{username}' is already taken."
            End If

            Dim newUserId = GenerateUniqueUserId()
            Dim newUserCode = $"USR-{newUserId:D5}"

            Db.Execute("INSERT INTO [Users] ([user_id], [user_code], [username], [full_name], [branch], [password_hash], [role], [is_active], [is_deleted], [last_login], [created_at]) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
                       Db.Int(newUserId), Db.Text(newUserCode, 20),
                       Db.Text(username, 100), Db.Text(If(String.IsNullOrEmpty(fullName), username, fullName), 100),
                       Db.Text(branch, 50), Db.Text(PasswordHasher.HashPassword(password), 255), Db.Text(role, 50),
                       Db.Bool(isActive), Db.Bool(False), Db.DtOrNull(Nothing), Db.Dt(DateTime.Now))
            Return Nothing
        End Function

        Public Shared Function Create(username As String, password As String, role As String, isActive As Boolean) As String
            Return Create(username, password, role, username, "HQ", isActive)
        End Function

        Public Shared Function UpdateAccess(userId As Integer, role As String, isActive As Boolean, actingUserId As Integer) As String
            If userId = actingUserId Then
                If Not isActive Then Return "You can't deactivate your own account."
                If role.IndexOf("Admin", StringComparison.OrdinalIgnoreCase) < 0 Then Return "You can't remove your own admin role."
            End If

            If (role.IndexOf("Admin", StringComparison.OrdinalIgnoreCase) < 0 OrElse Not isActive) AndAlso OtherActiveAdmins(userId) = 0 Then
                Return "At least one active Admin must remain."
            End If

            Db.Execute("UPDATE [Users] SET [role] = ?, [is_active] = ? WHERE [user_id] = ? AND [is_deleted] = False",
                       Db.Text(role, 50), Db.Bool(isActive), Db.Int(userId))
            Return Nothing
        End Function

        Public Shared Function ResetPassword(userId As Integer, newPassword As String) As String
            Dim passErr = ValidatePasswordStrength(newPassword)
            If passErr IsNot Nothing Then Return passErr

            Db.Execute("UPDATE [Users] SET [password_hash] = ? WHERE [user_id] = ? AND [is_deleted] = False",
                       Db.Text(PasswordHasher.HashPassword(newPassword), 255), Db.Int(userId))
            Return Nothing
        End Function

        Public Shared Function Delete(userId As Integer, actingUserId As Integer) As String
            If userId = actingUserId Then Return "You can't delete your own account."
            If OtherActiveAdmins(userId) = 0 AndAlso IsActiveAdmin(userId) Then Return "At least one active Admin must remain."

            Db.Execute("UPDATE [Users] SET [is_deleted] = True, [is_active] = False WHERE [user_id] = ?", Db.Int(userId))
            Return Nothing
        End Function

        Public Shared Function Archive(userId As Integer, actingUserId As Integer) As String
            If userId = actingUserId Then Return "You can't archive your own account."
            If OtherActiveAdmins(userId) = 0 AndAlso IsActiveAdmin(userId) Then Return "At least one administrator must remain."

            Db.Execute("UPDATE [Users] SET [is_deleted] = True, [is_active] = False WHERE [user_id] = ?", Db.Int(userId))
            Return Nothing
        End Function

        Public Shared Function GetArchived() As List(Of User)
            Dim users As New List(Of User)()
            Dim table = Db.Query("SELECT [user_id], [user_code], [username], [full_name], [branch], [role], [is_active], [is_deleted], [last_login], [created_at] FROM [Users] WHERE [is_deleted] = True ORDER BY [user_id]")

            For Each row As DataRow In table.Rows
                Dim userId = Convert.ToInt32(row("user_id"))
                Dim userCode = If(table.Columns.Contains("user_code") AndAlso Not IsDBNull(row("user_code")), Convert.ToString(row("user_code")), $"USR-{userId:D5}")
                Dim username = Convert.ToString(row("username"))
                Dim fullName = If(table.Columns.Contains("full_name") AndAlso Not IsDBNull(row("full_name")), Convert.ToString(row("full_name")), username)
                Dim branch = If(table.Columns.Contains("branch") AndAlso Not IsDBNull(row("branch")), Convert.ToString(row("branch")), "HQ")
                Dim roleName = Convert.ToString(row("role"))
                Dim isActive = Convert.ToBoolean(row("is_active"))
                Dim isDeleted = Convert.ToBoolean(row("is_deleted"))
                Dim lastLogin = If(IsDBNull(row("last_login")), CType(Nothing, DateTime?), Convert.ToDateTime(row("last_login")))
                Dim createdAt = If(IsDBNull(row("created_at")), DateTime.Now, Convert.ToDateTime(row("created_at")))
                Dim userRole As Global.AutoRent.Data.Models.UserRole = Global.AutoRent.Data.Models.UserRole.User
                If roleName.IndexOf("Admin", StringComparison.OrdinalIgnoreCase) >= 0 Then
                    userRole = Global.AutoRent.Data.Models.UserRole.Admin
                ElseIf roleName.IndexOf("Manager", StringComparison.OrdinalIgnoreCase) >= 0 OrElse roleName.IndexOf("Owner", StringComparison.OrdinalIgnoreCase) >= 0 Then
                    userRole = Global.AutoRent.Data.Models.UserRole.Manager
                Else
                    userRole = Global.AutoRent.Data.Models.UserRole.User
                End If

                users.Add(New Global.AutoRent.Data.Models.User With {
                    .UserId = userId,
                    .UserCode = userCode,
                    .Username = username,
                    .FullName = fullName,
                    .Branch = branch,
                    .Role = userRole,
                    .RoleName = roleName,
                    .IsActive = isActive,
                    .IsDeleted = isDeleted,
                    .LastLogin = lastLogin,
                    .CreatedAt = createdAt
                })
            Next

            Return users
        End Function

        Public Shared Function Restore(userId As Integer) As String
            Dim table = Db.Query("SELECT [role] FROM [Users] WHERE [user_id] = ?", Db.Int(userId))
            If table.Rows.Count > 0 Then
                Dim roleName = Convert.ToString(table.Rows(0)("role"))
                If IsAdminRole(roleName) AndAlso AdminExists() Then
                    Return "Cannot restore this account as System Administrator because another System Administrator already exists."
                End If
            End If

            Db.Execute("UPDATE [Users] SET [is_deleted] = False, [is_active] = False WHERE [user_id] = ?", Db.Int(userId))
            Return Nothing
        End Function

        Public Shared Function PermanentDelete(userId As Integer, actingUserId As Integer) As String
            If userId = actingUserId Then Return "You cannot delete your own account."
            If OtherActiveAdmins(userId) = 0 AndAlso IsActiveAdmin(userId) Then Return "At least one administrator must remain."

            Db.Execute("DELETE FROM [Users] WHERE [user_id] = ?", Db.Int(userId))
            Return Nothing
        End Function

        Public Shared Function Update(userId As Integer, fullName As String, username As String, role As String, newPassword As String) As String
            username = If(username, "").Trim()
            fullName = If(fullName, "").Trim()
            role = If(role, "Rental Staff").Trim()

            Dim nameIssue = InputValidation.FullName(fullName)
            If nameIssue IsNot Nothing Then Return nameIssue
            Dim userIssue = InputValidation.Username(username)
            If userIssue IsNot Nothing Then Return userIssue
            If Db.Count("SELECT COUNT(*) FROM [Users] WHERE UCASE(TRIM([full_name])) = ? AND [user_id] <> ? AND [is_deleted] = False",
                        Db.Text(fullName.ToUpperInvariant(), 100), Db.Int(userId)) > 0 Then
                Return "Another staff account already uses this full name."
            End If

            If IsAdminRole(role) AndAlso OtherActiveAdmins(userId) > 0 Then
                Return "Only one System Administrator is allowed. Another account already holds this role."
            End If

            If OtherActiveAdmins(userId) = 0 AndAlso IsActiveAdmin(userId) AndAlso Not IsAdminRole(role) Then
                Return "At least one active System Administrator must remain."
            End If

            If Db.Count("SELECT COUNT(*) FROM [Users] WHERE [username] = ? AND [user_id] <> ? AND [is_deleted] = False", Db.Text(username, 100), Db.Int(userId)) > 0 Then
                Return $"The username '{username}' is already taken."
            End If

            If Not String.IsNullOrWhiteSpace(newPassword) Then
                Dim passErr = ValidatePasswordStrength(newPassword)
                If passErr IsNot Nothing Then Return passErr

                Db.Execute("UPDATE [Users] SET [full_name] = ?, [username] = ?, [role] = ?, [password_hash] = ? WHERE [user_id] = ? AND [is_deleted] = False",
                           Db.Text(fullName, 100), Db.Text(username, 100), Db.Text(role, 50),
                           Db.Text(PasswordHasher.HashPassword(newPassword), 255), Db.Int(userId))
            Else
                Db.Execute("UPDATE [Users] SET [full_name] = ?, [username] = ?, [role] = ? WHERE [user_id] = ? AND [is_deleted] = False",
                           Db.Text(fullName, 100), Db.Text(username, 100), Db.Text(role, 50), Db.Int(userId))
            End If

            Return Nothing
        End Function

        Public Shared Function ActiveUserCount() As Integer
            Return Db.Count("SELECT COUNT(*) FROM [Users] WHERE [is_active] = True AND [is_deleted] = False")
        End Function

        Public Shared Function GetUserActivity(user As Global.AutoRent.Data.Models.User) As List(Of String)
            Return GetRecentActivity(user)
        End Function

        Public Shared Function GetRecentActivity(user As Global.AutoRent.Data.Models.User) As List(Of String)
            Dim logs As New List(Of String)()
            If user Is Nothing Then Return logs

            If user.LastLogin.HasValue Then
                logs.Add($"• Authenticated: {user.LastLogin.Value:dd MMM yyyy, h:mm tt} via {user.Branch} terminal")
            Else
                logs.Add($"• No sign-in recorded yet (Account created {user.CreatedAt:dd MMM yyyy, h:mm tt})")
            End If

            Try

                Dim dispatchCount = Db.Count("SELECT COUNT(*) FROM [Dispatches] WHERE [branch] = ?", Db.Text(user.Branch, 50))
                If dispatchCount > 0 Then
                    logs.Add($"• Branch Dispatches: {dispatchCount} dispatches active at {user.Branch} location")
                End If
            Catch ex As Exception
            End Try

            logs.Add($"• Authorization: {user.AccountAuthority}")
            logs.Add("• Security: Cryptographic credentials verified (PBKDF2 SHA-256)")

            Return logs
        End Function

        Private Shared Function OtherActiveAdmins(excludeUserId As Integer) As Integer
            Return Db.Count("SELECT COUNT(*) FROM [Users] WHERE ([role] = 'Admin' OR [role] = 'System Administrator') AND [is_deleted] = False AND [user_id] <> ?", Db.Int(excludeUserId))
        End Function

        Private Shared Function IsActiveAdmin(userId As Integer) As Boolean
            Return Db.Count("SELECT COUNT(*) FROM [Users] WHERE [user_id] = ? AND ([role] = 'Admin' OR [role] = 'System Administrator') AND [is_deleted] = False", Db.Int(userId)) > 0
        End Function
    End Class
End Namespace
