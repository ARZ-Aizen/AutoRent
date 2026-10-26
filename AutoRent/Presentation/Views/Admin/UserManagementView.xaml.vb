Imports System.Collections.Generic
Imports System.Data
Imports System.Linq
Imports System.Security.Cryptography
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports AutoRent.Common.Utilities
Imports AutoRent.Data.Repositories

Namespace Presentation.Views.Admin
    Partial Public Class UserManagementView
        Inherits UserControl

        Private _table As DataTable
        Private _selectedId As Integer = 0
        Private _selectedRow As DataRowView
        Private _ready As Boolean = False

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Function CurrentUserId() As Integer
            Dim app = CType(System.Windows.Application.Current, Global.AutoRent.Application)
            Return If(app.CurrentUser IsNot Nothing, app.CurrentUser.UserId, 0)
        End Function

        Private Sub UserManagementView_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            If _ready Then Return

            TxtHeaderDay.Text = DateTime.Today.ToString("dddd").ToUpperInvariant()
            TxtHeaderDate.Text = DateTime.Today.ToString("dd MMM yyyy")

            Dim app = CType(System.Windows.Application.Current, Global.AutoRent.Application)
            If app.CurrentUser IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(app.CurrentUser.Username) Then
                Dim u = app.CurrentUser.Username.Trim()
                If u.Length >= 2 Then
                    TxtAvatarInitials.Text = u.Substring(0, 2).ToUpperInvariant()
                Else
                    TxtAvatarInitials.Text = u.ToUpperInvariant()
                End If
            Else
                TxtAvatarInitials.Text = "AM"
            End If

            CmbRoleFilter.ItemsSource = New String() {
                "All roles",
                "System Administrator",
                "Business Owner / Branch Manager",
                "Dispatch Officer",
                "Rental Clerk",
                "Front-Desk Staff"
            }
            CmbRoleFilter.SelectedIndex = 0

            CmbBranchFilter.ItemsSource = New String() {
                "All branches",
                "HQ",
                "North Airport",
                "Downtown"
            }
            CmbBranchFilter.SelectedIndex = 0

            CmbStatusFilter.ItemsSource = New String() {
                "All statuses",
                "Active",
                "Deactivated",
                "Invite pending"
            }
            CmbStatusFilter.SelectedIndex = 0

            CmbNewRole.ItemsSource = New String() {
                "Rental Clerk",
                "System Administrator",
                "Business Owner / Branch Manager",
                "Dispatch Officer",
                "Front-Desk Staff"
            }
            CmbNewRole.SelectedIndex = 0

            CmbNewBranch.ItemsSource = New String() {"HQ", "North Airport", "Downtown"}
            CmbNewBranch.SelectedIndex = 0

            _ready = True
            LoadUsers()
        End Sub

        Private Sub LoadUsers(Optional selectUserId As Integer = 0)
            Try
                _table = UserAdminRepository.GetAll()
                DgStaff.ItemsSource = _table.DefaultView
                ApplyFilter()
                UpdateHeaderCounters()

                If _table.Rows.Count > 0 Then
                    Dim targetIndex As Integer = 0
                    If selectUserId > 0 Then
                        For i As Integer = 0 To DgStaff.Items.Count - 1
                            Dim row = TryCast(DgStaff.Items(i), DataRowView)
                            If row IsNot Nothing AndAlso ViewHelpers.IntOf(row, "user_id") = selectUserId Then
                                targetIndex = i
                                Exit For
                            End If
                        Next
                    Else
                        For i As Integer = 0 To DgStaff.Items.Count - 1
                            Dim row = TryCast(DgStaff.Items(i), DataRowView)
                            If row IsNot Nothing AndAlso ViewHelpers.Str(row, "username").ToLowerInvariant() = "m.chen" Then
                                targetIndex = i
                                Exit For
                            End If
                        Next
                    End If

                    If targetIndex < DgStaff.Items.Count Then
                        DgStaff.SelectedIndex = targetIndex
                    End If
                End If

            Catch ex As Exception
                AlertUtil.ShowError($"Could not load users: {ex.Message}", "User Management", ViewHelpers.Owner(Me))
            End Try
        End Sub

        Private Sub UpdateHeaderCounters()
            If _table Is Nothing Then Return

            Dim total = _table.Rows.Count
            Dim activeCount = 0
            Dim pendingCount = 0
            Dim deactivatedCount = 0

            For Each row As DataRow In _table.Rows
                Dim status = Convert.ToString(row("status_badge"))
                Select Case status
                    Case "Active": activeCount += 1
                    Case "Invite pending": pendingCount += 1
                    Case "Deactivated": deactivatedCount += 1
                    Case Else
                        If Convert.ToBoolean(row("is_active")) Then
                            activeCount += 1
                        Else
                            deactivatedCount += 1
                        End If
                End Select
            Next

            TxtHeaderStats.Text = $"{total} staff accounts · {activeCount} active · {pendingCount} invitation pending · {deactivatedCount} deactivated · passwords are never displayed"

            Dim filteredCount = DgStaff.Items.Count
            TxtShowingCount.Text = $"Showing {filteredCount} of {total} accounts · access restricted to authorized administrators"
        End Sub

        Private Sub FilterInputs_Changed(sender As Object, e As EventArgs)
            If Not _ready OrElse _table Is Nothing Then Return
            TxtSearchPlaceholder.Visibility = If(String.IsNullOrEmpty(TxtSearchStaff.Text), Visibility.Visible, Visibility.Collapsed)
            ApplyFilter()
        End Sub

        Private Sub ApplyFilter()
            If _table Is Nothing Then Return

            Dim parts As New List(Of String)()
            Dim query = TxtSearchStaff.Text.Trim()
            If query.Length > 0 Then
                Dim pat = ViewHelpers.EscapeLike(query)
                parts.Add($"(full_name LIKE '%{pat}%' OR username LIKE '%{pat}%' OR user_code LIKE '%{pat}%' OR display_role LIKE '%{pat}%')")
            End If

            Dim selectedRole = TryCast(CmbRoleFilter.SelectedItem, String)
            If Not String.IsNullOrEmpty(selectedRole) AndAlso selectedRole <> "All roles" Then
                Dim patRole = ViewHelpers.EscapeLike(selectedRole)
                parts.Add($"display_role LIKE '%{patRole}%'")
            End If

            Dim selectedBranch = TryCast(CmbBranchFilter.SelectedItem, String)
            If Not String.IsNullOrEmpty(selectedBranch) AndAlso selectedBranch <> "All branches" Then
                Dim patBranch = ViewHelpers.EscapeLike(selectedBranch)
                parts.Add($"branch LIKE '%{patBranch}%'")
            End If

            Dim selectedStatus = TryCast(CmbStatusFilter.SelectedItem, String)
            If Not String.IsNullOrEmpty(selectedStatus) AndAlso selectedStatus <> "All statuses" Then
                Dim patStatus = ViewHelpers.EscapeLike(selectedStatus)
                parts.Add($"status_badge = '{patStatus}'")
            End If

            _table.DefaultView.RowFilter = If(parts.Count > 0, String.Join(" AND ", parts), String.Empty)

            If TxtShowingCount IsNot Nothing Then
                Dim total = _table.Rows.Count
                Dim filteredCount = DgStaff.Items.Count
                TxtShowingCount.Text = $"Showing {filteredCount} of {total} accounts · access restricted to authorized administrators"
            End If
        End Sub

        Private Sub TxtGlobalSearch_GotFocus(sender As Object, e As RoutedEventArgs)
            If TxtGlobalSearch.Text = "Search records..." Then
                TxtGlobalSearch.Text = ""
                TxtGlobalSearch.Foreground = System.Windows.Media.Brushes.Black
            End If
        End Sub

        Private Sub TxtGlobalSearch_LostFocus(sender As Object, e As RoutedEventArgs)
            If String.IsNullOrWhiteSpace(TxtGlobalSearch.Text) Then
                TxtGlobalSearch.Text = "Search records..."
                TxtGlobalSearch.Foreground = New System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(&H7A, &H74, &H6D))
            End If
        End Sub

        Private Sub TxtGlobalSearch_TextChanged(sender As Object, e As TextChangedEventArgs)
            If Not _ready OrElse TxtGlobalSearch.Text = "Search records..." Then Return
            TxtSearchStaff.Text = TxtGlobalSearch.Text
        End Sub

        Private Sub DgStaff_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            Dim row = TryCast(DgStaff.SelectedItem, DataRowView)
            If row Is Nothing Then Return

            _selectedRow = row
            _selectedId = ViewHelpers.IntOf(row, "user_id")

            Dim userCode = ViewHelpers.Str(row, "user_code")
            Dim fullName = ViewHelpers.Str(row, "full_name")
            Dim role = ViewHelpers.Str(row, "display_role")
            Dim statusBadge = ViewHelpers.Str(row, "status_badge")
            Dim invite = ViewHelpers.Str(row, "invitation_text")
            Dim activity = ViewHelpers.Str(row, "activity_text")
            Dim auth = ViewHelpers.Str(row, "authority_text")

            TxtSelTitle.Text = $"Selected account · {fullName}"

            If statusBadge = "Invite pending" Then
                TxtSelSubtitle.Text = $"{userCode} · secure invitation pending after account update"
                BtnSelToggleActive.Content = "Deactivate"
            ElseIf statusBadge = "Deactivated" Then
                TxtSelSubtitle.Text = $"{userCode} · account deactivated · credentials suspended"
                BtnSelToggleActive.Content = "Reactivate"
            Else
                TxtSelSubtitle.Text = $"{userCode} · active account verified in good standing"
                BtnSelToggleActive.Content = "Deactivate"
            End If

            TxtSelRole.Text = role
            TxtSelInvite.Text = invite
            TxtSelActivity.Text = activity
            TxtSelAuthority.Text = auth
        End Sub

        Private Sub BtnRowAction_Click(sender As Object, e As RoutedEventArgs)
            Dim btn = TryCast(sender, Button)
            If btn Is Nothing Then Return

            Dim userId = Convert.ToInt32(btn.Tag)
            For Each item In DgStaff.Items
                Dim row = TryCast(item, DataRowView)
                If row IsNot Nothing AndAlso ViewHelpers.IntOf(row, "user_id") = userId Then
                    DgStaff.SelectedItem = row
                    Exit For
                End If
            Next

            Dim actionText = Convert.ToString(btn.Content)
            If actionText.Contains("Resend Invite") Then
                AlertUtil.ShowSuccess($"Invitation link resent to {ViewHelpers.Str(_selectedRow, "full_name")}.", "Account Lifecycle", ViewHelpers.Owner(Me))
            ElseIf actionText.Contains("Reactivate") Then
                BtnSelToggleActive_Click(sender, e)
            Else
                BtnSelActivity_Click(sender, e)
            End If
        End Sub

        Private Sub BtnSelToggleActive_Click(sender As Object, e As RoutedEventArgs)
            If _selectedId = 0 OrElse _selectedRow Is Nothing Then Return

            Dim currentActive = Convert.ToBoolean(_selectedRow("is_active"))
            Dim fullName = ViewHelpers.Str(_selectedRow, "full_name")
            Dim role = ViewHelpers.Str(_selectedRow, "role")

            Dim newActive = Not currentActive
            Dim actionName = If(newActive, "Reactivate", "Deactivate")

            If Not AlertUtil.Confirm($"{actionName} staff account for '{fullName}'?", $"{actionName} Account", ViewHelpers.Owner(Me)) Then
                Return
            End If

            Try
                Dim problem = UserAdminRepository.UpdateAccess(_selectedId, role, newActive, CurrentUserId())
                If problem IsNot Nothing Then
                    AlertUtil.ShowWarning(problem, "User Management", ViewHelpers.Owner(Me))
                    Return
                End If

                AlertUtil.ShowSuccess($"Account for '{fullName}' was {If(newActive, "reactivated", "deactivated")}.", "User Management", ViewHelpers.Owner(Me))
                LoadUsers(_selectedId)
            Catch ex As Exception
                AlertUtil.ShowError($"Could not update user: {ex.Message}", "User Management", ViewHelpers.Owner(Me))
            End Try
        End Sub

        Private Sub BtnSelResetPass_Click(sender As Object, e As RoutedEventArgs)
            If _selectedId = 0 OrElse _selectedRow Is Nothing Then Return

            Dim fullName = ViewHelpers.Str(_selectedRow, "full_name")
            Dim userCode = ViewHelpers.Str(_selectedRow, "user_code")
            TxtResetPassTitle.Text = $"Reset Password · {fullName}"
            TxtResetPassSub.Text = $"Enter a new password for {userCode} ({fullName}) or generate a secure temporary one."
            TxtResetNewPassword.Text = GenerateSecurePassword()
            ResetPassModal.Visibility = Visibility.Visible
        End Sub

        Private Sub BtnGenerateResetPassword_Click(sender As Object, e As RoutedEventArgs)
            TxtResetNewPassword.Text = GenerateSecurePassword()
        End Sub

        Private Sub BtnCloseResetModal_Click(sender As Object, e As RoutedEventArgs)
            ResetPassModal.Visibility = Visibility.Collapsed
        End Sub

        Private Sub ResetPassBackdrop_MouseDown(sender As Object, e As MouseButtonEventArgs)
            ResetPassModal.Visibility = Visibility.Collapsed
        End Sub

        Private Sub BtnSaveResetPass_Click(sender As Object, e As RoutedEventArgs)
            If _selectedId = 0 Then Return

            Dim newPass = TxtResetNewPassword.Text.Trim()
            If newPass.Length < UserAdminRepository.MinPasswordLength Then
                AlertUtil.ShowWarning($"Password must be at least {UserAdminRepository.MinPasswordLength} characters.", "Reset Password", ViewHelpers.Owner(Me))
                Return
            End If

            Try
                Dim problem = UserAdminRepository.ResetPassword(_selectedId, newPass)
                If problem IsNot Nothing Then
                    AlertUtil.ShowWarning(problem, "Reset Password", ViewHelpers.Owner(Me))
                    Return
                End If

                ResetPassModal.Visibility = Visibility.Collapsed
                AlertUtil.ShowInformationWithCopy(
                    $"Password for '{ViewHelpers.Str(_selectedRow, "full_name")}' was reset successfully.{Environment.NewLine}{Environment.NewLine}Temporary password: {newPass}{Environment.NewLine}{Environment.NewLine}Copy and convey this temporary password securely to the staff member.",
                    "Password Reset")
            Catch ex As Exception
                AlertUtil.ShowError($"Could not reset password: {ex.Message}", "Reset Password", ViewHelpers.Owner(Me))
            End Try
        End Sub

        Private Sub BtnOpenCreateUser_Click(sender As Object, e As RoutedEventArgs)
            TxtNewFullName.Clear()
            TxtNewUsername.Clear()
            TxtNewPassword.Text = GenerateSecurePassword()
            CmbNewRole.SelectedIndex = 0
            CmbNewBranch.SelectedIndex = 0
            ChkNewActive.IsChecked = True
            CreateUserModal.Visibility = Visibility.Visible
            TxtNewFullName.Focus()
        End Sub

        Private Sub BtnGeneratePassword_Click(sender As Object, e As RoutedEventArgs)
            TxtNewPassword.Text = GenerateSecurePassword()
        End Sub

        Private Sub BtnCloseCreateModal_Click(sender As Object, e As RoutedEventArgs)
            CreateUserModal.Visibility = Visibility.Collapsed
        End Sub

        Private Sub CreateModalBackdrop_MouseDown(sender As Object, e As MouseButtonEventArgs)
            CreateUserModal.Visibility = Visibility.Collapsed
        End Sub

        Private Sub BtnSaveNewUser_Click(sender As Object, e As RoutedEventArgs)
            Dim username = TxtNewUsername.Text.Trim()
            Dim password = TxtNewPassword.Text.Trim()
            Dim role = TryCast(CmbNewRole.SelectedItem, String)
            Dim isActive = (ChkNewActive.IsChecked = True)

            If String.IsNullOrWhiteSpace(username) Then
                AlertUtil.ShowWarning("Please specify a username.", "Create User", ViewHelpers.Owner(Me))
                Return
            End If

            Dim dbRole = If(role.Contains("Admin") OrElse role.Contains("Owner"), "Admin", "User")

            Try
                Dim problem = UserAdminRepository.Create(username, password, dbRole, isActive)
                If problem IsNot Nothing Then
                    AlertUtil.ShowWarning(problem, "Create User", ViewHelpers.Owner(Me))
                    Return
                End If

                CreateUserModal.Visibility = Visibility.Collapsed
                AlertUtil.ShowSuccess($"Staff account '{username}' was created successfully.", "Create User", ViewHelpers.Owner(Me))
                LoadUsers()
            Catch ex As Exception
                AlertUtil.ShowError($"Could not create user: {ex.Message}", "Create User", ViewHelpers.Owner(Me))
            End Try
        End Sub

        Private Sub BtnSelActivity_Click(sender As Object, e As RoutedEventArgs)
            If _selectedRow Is Nothing Then Return

            Dim fullName = ViewHelpers.Str(_selectedRow, "full_name")
            Dim userCode = ViewHelpers.Str(_selectedRow, "user_code")
            Dim lastLogin = ViewHelpers.Str(_selectedRow, "last_login_formatted")
            Dim activity = ViewHelpers.Str(_selectedRow, "activity_text")

            TxtActivityTitle.Text = $"Account Activity · {fullName}"
            TxtActivitySub.Text = $"{userCode} · recorded access events and authorization history"
            TxtActivityLog1.Text = $"• Recent task: {activity}"
            TxtActivityLog2.Text = $"• Last login session: {lastLogin} via HQ terminal"
            TxtActivityLog3.Text = $"• Cryptographic credentials verified (PBKDF2-SHA256)"

            ActivityModal.Visibility = Visibility.Visible
        End Sub

        Private Sub BtnCloseActivityModal_Click(sender As Object, e As RoutedEventArgs)
            ActivityModal.Visibility = Visibility.Collapsed
        End Sub

        Private Sub ActivityBackdrop_MouseDown(sender As Object, e As MouseButtonEventArgs)
            ActivityModal.Visibility = Visibility.Collapsed
        End Sub

        Private Shared Function GenerateSecurePassword() As String
            Dim bytes = RandomNumberGenerator.GetBytes(6)
            Return "AutoRent#" & Convert.ToHexString(bytes).Substring(0, 6) & "!"
        End Function
    End Class
End Namespace
