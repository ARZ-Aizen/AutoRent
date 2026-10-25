Imports System.Data
Imports AutoRent.Common.Utilities
Imports AutoRent.Data.Context

Namespace Data.Repositories
    ''' <summary>Admin operations on the [Users] table. Methods return an error message, or Nothing on success.</summary>
    Public NotInheritable Class UserAdminRepository
        Private Sub New()
        End Sub

        Public Const MinPasswordLength As Integer = 8

        Public Shared Function GetAll() As DataTable
            Dim table = Db.Query("SELECT [user_id], [username], [role], [is_active], [last_login], [created_at] FROM [Users] WHERE [is_deleted] = False ORDER BY [username]")
            table.Columns.Add("state", GetType(String))
            table.Columns.Add("last_login_text", GetType(String))
            For Each row As DataRow In table.Rows
                row("state") = If(Convert.ToBoolean(row("is_active")), "Active", "Inactive")
                row("last_login_text") = If(IsDBNull(row("last_login")), "Never", Convert.ToDateTime(row("last_login")).ToString("MMM dd, yyyy h:mm tt"))
            Next
            Return table
        End Function

        Public Shared Function Create(username As String, password As String, role As String, isActive As Boolean) As String
            username = If(username, "").Trim()
            If username.Length < 3 Then Return "Username must be at least 3 characters."
            If password Is Nothing OrElse password.Length < MinPasswordLength Then Return $"Password must be at least {MinPasswordLength} characters."
            If Db.Count("SELECT COUNT(*) FROM [Users] WHERE [username] = ?", Db.Text(username, 100)) > 0 Then
                Return $"The username '{username}' is already taken."
            End If

            Db.Execute("INSERT INTO [Users] ([username], [password_hash], [role], [is_active], [is_deleted], [last_login], [created_at]) VALUES (?, ?, ?, ?, ?, ?, ?)",
                       Db.Text(username, 100), Db.Text(PasswordHasher.HashPassword(password), 255), Db.Text(role, 50),
                       Db.Bool(isActive), Db.Bool(False), Db.DtOrNull(Nothing), Db.Dt(DateTime.Now))
            Return Nothing
        End Function

        Public Shared Function UpdateAccess(userId As Integer, role As String, isActive As Boolean, actingUserId As Integer) As String
            If userId = actingUserId Then
                If Not isActive Then Return "You can't deactivate your own account."
                If role <> "Admin" Then Return "You can't remove your own admin role."
            End If

            If (role <> "Admin" OrElse Not isActive) AndAlso OtherActiveAdmins(userId) = 0 Then
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

        ''' <summary>Soft delete: the row stays (the username index still reserves the name) but the user can no longer sign in.</summary>
        Public Shared Function Delete(userId As Integer, actingUserId As Integer) As String
            If userId = actingUserId Then Return "You can't delete your own account."
            If OtherActiveAdmins(userId) = 0 AndAlso IsActiveAdmin(userId) Then Return "At least one active Admin must remain."

            Db.Execute("UPDATE [Users] SET [is_deleted] = True, [is_active] = False WHERE [user_id] = ?", Db.Int(userId))
            Return Nothing
        End Function

        Public Shared Function ActiveUserCount() As Integer
            Return Db.Count("SELECT COUNT(*) FROM [Users] WHERE [is_active] = True AND [is_deleted] = False")
        End Function

        Private Shared Function OtherActiveAdmins(excludeUserId As Integer) As Integer
            Return Db.Count("SELECT COUNT(*) FROM [Users] WHERE [role] = 'Admin' AND [is_active] = True AND [is_deleted] = False AND [user_id] <> ?", Db.Int(excludeUserId))
        End Function

        Private Shared Function IsActiveAdmin(userId As Integer) As Boolean
            Return Db.Count("SELECT COUNT(*) FROM [Users] WHERE [user_id] = ? AND [role] = 'Admin' AND [is_active] = True AND [is_deleted] = False", Db.Int(userId)) > 0
        End Function
    End Class
End Namespace
