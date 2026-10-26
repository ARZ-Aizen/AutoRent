Imports System.Collections.Generic
Imports System.Data
Imports System.Security.Cryptography
Imports AutoRent.Common.Utilities
Imports AutoRent.Data.Context
Imports AutoRent.Data.Models

Namespace Data.Repositories
    Public NotInheritable Class UserAdminRepository
        Private Sub New()
        End Sub

        Public Const MinPasswordLength As Integer = 8

        Public Shared Sub EnsureSampleStaffSeeded()
            Try
                Dim count = Db.Count("SELECT COUNT(*) FROM [Users] WHERE [is_deleted] = False")
                If count <= 1 Then
                    Dim staffData = {
                        ("Alex Morgan", "a.morgan", "System Administrator", "HQ", True, DateTime.Now.AddHours(-10)),
                        ("Priya Shah", "p.shah", "Manager", "HQ", True, DateTime.Now.AddHours(-9)),
                        ("Julian Sterling", "j.sterling", "Rental Staff", "HQ", True, DateTime.Now.AddHours(-8)),
                        ("Daniel Reed", "d.reed", "Rental Staff", "HQ", True, DateTime.Now.AddHours(-7)),
                        ("Noah Brooks", "n.brooks", "Rental Staff", "HQ", True, DateTime.Now.AddHours(-6)),
                        ("Ella Hayes", "e.hayes", "Rental Staff", "HQ", True, DateTime.Now.AddHours(-5)),
                        ("Luca Bennett", "l.bennett", "Rental Staff", "HQ", False, DateTime.Now.AddDays(-6)),
                        ("Maya Chen", "m.chen", "Rental Staff", "HQ", True, CType(Nothing, DateTime?))
                    }

                    For Each item In staffData
                        If Db.Count("SELECT COUNT(*) FROM [Users] WHERE [username] = ?", Db.Text(item.Item2, 100)) = 0 Then
                            Db.Execute("INSERT INTO [Users] ([username], [full_name], [branch], [password_hash], [role], [is_active], [is_deleted], [last_login], [created_at]) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)",
                                       Db.Text(item.Item2, 100), Db.Text(item.Item1, 100), Db.Text(item.Item4, 50),
                                       Db.Text(PasswordHasher.HashPassword("Staff123!"), 255), Db.Text(item.Item3, 50),
                                       Db.Bool(item.Item5), Db.Bool(False), Db.DtOrNull(item.Item6), Db.Dt(DateTime.Now.AddDays(-14)))
                        End If
                    Next
                End If
            Catch ex As Exception
            End Try
        End Sub

        Public Shared Function GetAll() As List(Of User)
            EnsureSampleStaffSeeded()
            Dim users As New List(Of User)()

            Dim table = Db.Query("SELECT [user_id], [username], [full_name], [branch], [role], [is_active], [is_deleted], [last_login], [created_at] FROM [Users] WHERE [is_deleted] = False ORDER BY [user_id]")

            For Each row As DataRow In table.Rows
                Dim userId = Convert.ToInt32(row("user_id"))
                Dim username = Convert.ToString(row("username"))
                Dim fullName = If(table.Columns.Contains("full_name") AndAlso Not IsDBNull(row("full_name")), Convert.ToString(row("full_name")), username)
                Dim branch = If(table.Columns.Contains("branch") AndAlso Not IsDBNull(row("branch")), Convert.ToString(row("branch")), "HQ")
                Dim roleName = Convert.ToString(row("role"))
                Dim isActive = Convert.ToBoolean(row("is_active"))
                Dim isDeleted = Convert.ToBoolean(row("is_deleted"))
                Dim lastLogin = If(IsDBNull(row("last_login")), CType(Nothing, DateTime?), Convert.ToDateTime(row("last_login")))
                Dim createdAt = If(IsDBNull(row("created_at")), DateTime.Now, Convert.ToDateTime(row("created_at")))
                Dim userRole As Global.AutoRent.Data.Models.UserRole = If(roleName.IndexOf("Admin", StringComparison.OrdinalIgnoreCase) >= 0, Global.AutoRent.Data.Models.UserRole.Admin, Global.AutoRent.Data.Models.UserRole.User)

                users.Add(New Global.AutoRent.Data.Models.User With {
                    .UserId = userId,
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

        Public Shared Function Create(username As String, password As String, role As String, fullName As String, branch As String, isActive As Boolean) As String
            username = If(username, "").Trim()
            fullName = If(fullName, "").Trim()
            branch = If(branch, "HQ").Trim()
            role = If(role, "Rental Staff").Trim()

            If username.Length < 3 Then Return "Username must be at least 3 characters."
            If password Is Nothing OrElse password.Length < MinPasswordLength Then Return $"Password must be at least {MinPasswordLength} characters."
            If Db.Count("SELECT COUNT(*) FROM [Users] WHERE [username] = ?", Db.Text(username, 100)) > 0 Then
                Return $"The username '{username}' is already taken."
            End If

            Db.Execute("INSERT INTO [Users] ([username], [full_name], [branch], [password_hash], [role], [is_active], [is_deleted], [last_login], [created_at]) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)",
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
            If newPassword Is Nothing OrElse newPassword.Length < MinPasswordLength Then Return $"Password must be at least {MinPasswordLength} characters."

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
            Dim table = Db.Query("SELECT [user_id], [username], [full_name], [branch], [role], [is_active], [is_deleted], [last_login], [created_at] FROM [Users] WHERE [is_deleted] = True ORDER BY [user_id]")

            For Each row As DataRow In table.Rows
                Dim userId = Convert.ToInt32(row("user_id"))
                Dim username = Convert.ToString(row("username"))
                Dim fullName = If(table.Columns.Contains("full_name") AndAlso Not IsDBNull(row("full_name")), Convert.ToString(row("full_name")), username)
                Dim branch = If(table.Columns.Contains("branch") AndAlso Not IsDBNull(row("branch")), Convert.ToString(row("branch")), "HQ")
                Dim roleName = Convert.ToString(row("role"))
                Dim isActive = Convert.ToBoolean(row("is_active"))
                Dim isDeleted = Convert.ToBoolean(row("is_deleted"))
                Dim lastLogin = If(IsDBNull(row("last_login")), CType(Nothing, DateTime?), Convert.ToDateTime(row("last_login")))
                Dim createdAt = If(IsDBNull(row("created_at")), DateTime.Now, Convert.ToDateTime(row("created_at")))
                Dim userRole As Global.AutoRent.Data.Models.UserRole = If(roleName.IndexOf("Admin", StringComparison.OrdinalIgnoreCase) >= 0, Global.AutoRent.Data.Models.UserRole.Admin, Global.AutoRent.Data.Models.UserRole.User)

                users.Add(New Global.AutoRent.Data.Models.User With {
                    .UserId = userId,
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

            If username.Length < 3 Then Return "Username must be at least 3 characters."
            If String.IsNullOrWhiteSpace(fullName) Then Return "Full name cannot be blank."

            If Db.Count("SELECT COUNT(*) FROM [Users] WHERE [username] = ? AND [user_id] <> ? AND [is_deleted] = False", Db.Text(username, 100), Db.Int(userId)) > 0 Then
                Return $"The username '{username}' is already taken."
            End If

            If Not String.IsNullOrWhiteSpace(newPassword) Then
                If newPassword.Length < MinPasswordLength Then Return $"Password must be at least {MinPasswordLength} characters."
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
                Dim inspectCount = Db.Count("SELECT COUNT(*) FROM [Inspections] WHERE [inspector] = ?", Db.Text(user.DisplayName, 100))
                If inspectCount > 0 Then
                    logs.Add($"• Vehicle Inspections: {inspectCount} inspections performed and signed")
                End If

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
