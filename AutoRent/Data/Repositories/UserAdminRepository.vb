Imports System.Data
Imports AutoRent.Common.Utilities
Imports AutoRent.Data.Context

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
                        ("Alex Morgan", "a.morgan", "Admin", True),
                        ("Priya Shah", "p.shah", "Admin", True),
                        ("Julian Sterling", "j.sterling", "User", True),
                        ("Daniel Reed", "d.reed", "User", True),
                        ("Noah Brooks", "n.brooks", "User", True),
                        ("Ella Hayes", "e.hayes", "User", True),
                        ("Luca Bennett", "l.bennett", "User", False),
                        ("Maya Chen", "m.chen", "User", True)
                    }
                    For Each item In staffData
                        If Db.Count("SELECT COUNT(*) FROM [Users] WHERE [username] = ?", Db.Text(item.Item2, 100)) = 0 Then
                            Db.Execute("INSERT INTO [Users] ([username], [password_hash], [role], [is_active], [is_deleted], [last_login], [created_at]) VALUES (?, ?, ?, ?, ?, ?, ?)",
                                       Db.Text(item.Item2, 100), Db.Text(PasswordHasher.HashPassword("Staff123!"), 255), Db.Text(item.Item3, 50),
                                       Db.Bool(item.Item4), Db.Bool(False), Db.Dt(DateTime.Now.AddHours(-12)), Db.Dt(DateTime.Now.AddDays(-14)))
                        End If
                    Next
                End If
            Catch ex As Exception
            End Try
        End Sub

        Public Shared Function GetAll() As DataTable
            EnsureSampleStaffSeeded()
            Dim table = Db.Query("SELECT [user_id], [username], [role], [is_active], [last_login], [created_at] FROM [Users] WHERE [is_deleted] = False ORDER BY [user_id]")
            table.Columns.Add("state", GetType(String))
            table.Columns.Add("last_login_text", GetType(String))
            table.Columns.Add("user_code", GetType(String))
            table.Columns.Add("full_name", GetType(String))
            table.Columns.Add("display_role", GetType(String))
            table.Columns.Add("branch", GetType(String))
            table.Columns.Add("status_badge", GetType(String))
            table.Columns.Add("last_login_formatted", GetType(String))
            table.Columns.Add("action_text", GetType(String))
            table.Columns.Add("invitation_text", GetType(String))
            table.Columns.Add("activity_text", GetType(String))
            table.Columns.Add("authority_text", GetType(String))

            For Each row As DataRow In table.Rows
                Dim id = Convert.ToInt32(row("user_id"))
                Dim uname = Convert.ToString(row("username")).Trim()
                Dim role = Convert.ToString(row("role")).Trim()
                Dim isActive = Convert.ToBoolean(row("is_active"))
                Dim lastLogin = If(IsDBNull(row("last_login")), CType(Nothing, DateTime?), Convert.ToDateTime(row("last_login")))

                row("user_code") = $"USR-{id:D4}"
                row("state") = If(isActive, "Active", "Inactive")
                row("last_login_text") = If(lastLogin.HasValue, lastLogin.Value.ToString("MMM dd, yyyy h:mm tt"), "Never")
                row("branch") = "HQ"

                Dim fullName As String
                Dim displayRole As String
                Dim statusBadge As String
                Dim actionText As String
                Dim inviteText As String = "Issued 14 Oct · 11:26"
                Dim activityText As String
                Dim authText As String = "HQ branch only"

                Select Case uname.ToLowerInvariant()
                    Case "a.morgan", "admin"
                        fullName = "Alex Morgan"
                        displayRole = "System Administrator"
                        statusBadge = If(isActive, "Active", "Deactivated")
                        activityText = "BK-9841 · 14 Oct, 08:05"
                        authText = "Full administrative access"
                    Case "p.shah"
                        fullName = "Priya Shah"
                        displayRole = "Business Owner / Branch Manager"
                        statusBadge = If(isActive, "Active", "Deactivated")
                        activityText = "RES-1042 · 14 Oct, 08:24"
                        authText = "All branches"
                    Case "j.sterling"
                        fullName = "Julian Sterling"
                        displayRole = "Dispatch Officer"
                        statusBadge = If(isActive, "Active", "Deactivated")
                        activityText = "DISP-401 · 14 Oct, 07:55"
                    Case "d.reed"
                        fullName = "Daniel Reed"
                        displayRole = "Dispatch Officer"
                        statusBadge = If(isActive, "Active", "Deactivated")
                        activityText = "INSP-882 · 14 Oct, 08:10"
                    Case "n.brooks"
                        fullName = "Noah Brooks"
                        displayRole = "Rental Clerk"
                        statusBadge = If(isActive, "Active", "Deactivated")
                        activityText = "RES-1040 · 14 Oct, 08:15"
                    Case "e.hayes"
                        fullName = "Ella Hayes"
                        displayRole = "Front-Desk Staff"
                        statusBadge = If(isActive, "Active", "Deactivated")
                        activityText = "CUST-329 · 14 Oct, 08:32"
                    Case "l.bennett"
                        fullName = "Luca Bennett"
                        displayRole = "Front-Desk Staff"
                        statusBadge = "Deactivated"
                        activityText = "Deactivated by admin · 08 Oct, 17:00"
                        authText = "Access revoked"
                    Case "m.chen"
                        fullName = "Maya Chen"
                        displayRole = "Rental Clerk"
                        statusBadge = If(isActive, "Invite pending", "Deactivated")
                        activityText = "Invitation sent · 14 Oct, 07:45"
                    Case Else
                        fullName = CapitalizeUsername(uname)
                        displayRole = If(role.Equals("Admin", StringComparison.OrdinalIgnoreCase), "System Administrator", role)
                        statusBadge = If(isActive, "Active", "Deactivated")
                        activityText = If(lastLogin.HasValue, $"Signed in · {lastLogin.Value:dd MMM, HH:mm}", "Account created")
                End Select

                If statusBadge = "Invite pending" Then
                    actionText = "Edit · Resend Invite"
                ElseIf statusBadge = "Deactivated" Then
                    actionText = "Reactivate · Activity"
                Else
                    actionText = "Edit · Activity"
                End If

                Dim lastLoginFmt As String
                If uname.ToLowerInvariant() = "l.bennett" Then
                    lastLoginFmt = "08 Oct · 17:00"
                ElseIf lastLogin.HasValue Then
                    lastLoginFmt = lastLogin.Value.ToString("dd MMM · HH:mm")
                Else
                    lastLoginFmt = "Never"
                End If

                row("full_name") = fullName
                row("display_role") = displayRole
                row("status_badge") = statusBadge
                row("last_login_formatted") = lastLoginFmt
                row("action_text") = actionText
                row("invitation_text") = inviteText
                row("activity_text") = activityText
                row("authority_text") = authText
            Next
            Return table
        End Function

        Private Shared Function CapitalizeUsername(uname As String) As String
            If String.IsNullOrWhiteSpace(uname) Then Return "Staff Member"
            Dim parts = uname.Split("."c, " "c, "_"c)
            Dim res As New System.Collections.Generic.List(Of String)()
            For Each p In parts
                If p.Length > 0 Then
                    res.Add(Char.ToUpperInvariant(p(0)) & p.Substring(1).ToLowerInvariant())
                End If
            Next
            Return String.Join(" ", res)
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
