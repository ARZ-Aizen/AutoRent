Imports System.Collections.Generic
Imports System.Linq
Imports System.Security.Cryptography
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports AutoRent.Common.Utilities
Imports AutoRent.Data.Models
Imports AutoRent.Data.Repositories

Namespace Presentation.Views.Admin
    Partial Public Class UserManagementView
        Inherits UserControl

        Private _allUsers As New List(Of Global.AutoRent.Data.Models.User)()
        Private _selectedUser As Global.AutoRent.Data.Models.User
        Private _selectedId As Integer = 0
        Private _ready As Boolean = False

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Function CurrentUserId() As Integer
            Dim app = TryCast(System.Windows.Application.Current, Global.AutoRent.Application)
            Return If(app?.CurrentUser IsNot Nothing, app.CurrentUser.UserId, 0)
        End Function

        Private Sub UserManagementView_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            If _ready Then Return

            CmbRoleFilter.ItemsSource = New String() {
                "All roles",
                "System Administrator",
                "Manager",
                "Rental Staff"
            }
            CmbRoleFilter.SelectedIndex = 0

            CmbStatusFilter.ItemsSource = New String() {
                "All statuses",
                "Active",
                "Inactive"
            }
            CmbStatusFilter.SelectedIndex = 0

            If UserAdminRepository.AdminExists() Then
                CmbNewRole.ItemsSource = New String() {
                    "Rental Staff",
                    "Manager"
                }
            Else
                CmbNewRole.ItemsSource = New String() {
                    "Rental Staff",
                    "System Administrator",
                    "Manager"
                }
            End If
            CmbNewRole.SelectedIndex = 0

            CmbEditRole.ItemsSource = New String() {
                "Rental Staff",
                "System Administrator",
                "Manager"
            }
            CmbEditRole.SelectedIndex = 0

            _ready = True
            LoadUsers()
            UpdateArchivedBadge()
        End Sub

        Private Sub LoadUsers(Optional selectUserId As Integer = 0)
            Try
                _allUsers = UserAdminRepository.GetAll()
                ApplyFilter(selectUserId)
                UpdateArchivedBadge()
            Catch ex As Exception
                AlertUtil.ShowError($"Could not load users: {ex.Message}", "User Management", ViewHelpers.Owner(Me))
            End Try
        End Sub

        Private Sub UpdateHeaderCounters(filteredCount As Integer)
            Dim total = _allUsers.Count
            Dim activeCount = _allUsers.Where(Function(u) u.StatusBadge = "Active").Count()
            Dim inactiveCount = _allUsers.Where(Function(u) u.StatusBadge = "Inactive").Count()

            TxtHeaderStats.Text = $"{total} staff accounts · {activeCount} active · {inactiveCount} inactive · passwords are never displayed"
            TxtShowingCount.Text = $"Showing {filteredCount} of {total} accounts · access restricted to authorized administrators"
        End Sub

        Private Sub FilterInputs_Changed(sender As Object, e As EventArgs)
            If Not _ready Then Return
            TxtSearchPlaceholder.Visibility = If(String.IsNullOrEmpty(TxtSearchStaff.Text), Visibility.Visible, Visibility.Collapsed)
            ApplyFilter(_selectedId)
        End Sub

        Private Sub ApplyFilter(Optional preserveUserId As Integer = 0)
            If _allUsers Is Nothing Then Return

            Dim query = TxtSearchStaff.Text.Trim().ToLowerInvariant()
            Dim selectedRole = TryCast(CmbRoleFilter.SelectedItem, String)
            Dim selectedStatus = TryCast(CmbStatusFilter.SelectedItem, String)

            Dim filtered = _allUsers.Where(Function(u)
                If query.Length > 0 Then
                    Dim matches = u.DisplayName.ToLowerInvariant().Contains(query) OrElse
                                  u.Username.ToLowerInvariant().Contains(query) OrElse
                                  u.UserCode.ToLowerInvariant().Contains(query) OrElse
                                  u.DisplayRole.ToLowerInvariant().Contains(query)
                    If Not matches Then Return False
                End If

                If Not String.IsNullOrEmpty(selectedRole) AndAlso selectedRole <> "All roles" Then
                    If Not u.DisplayRole.Equals(selectedRole, StringComparison.OrdinalIgnoreCase) Then Return False
                End If

                If Not String.IsNullOrEmpty(selectedStatus) AndAlso selectedStatus <> "All statuses" Then
                    If Not u.StatusBadge.Equals(selectedStatus, StringComparison.OrdinalIgnoreCase) Then Return False
                End If

                Return True
            End Function).ToList()

            DgStaff.ItemsSource = filtered
            UpdateHeaderCounters(filtered.Count)

            If filtered.Count > 0 Then
                Dim targetUser As Global.AutoRent.Data.Models.User = Nothing

                If preserveUserId > 0 Then
                    targetUser = filtered.FirstOrDefault(Function(u) u.UserId = preserveUserId)
                End If

                If targetUser Is Nothing Then
                    targetUser = filtered.FirstOrDefault(Function(u) u.Username.ToLowerInvariant() = "m.chen")
                End If

                If targetUser Is Nothing Then
                    targetUser = filtered(0)
                End If

                DgStaff.SelectedItem = targetUser
            Else
                _selectedUser = Nothing
                _selectedId = 0
            End If
        End Sub

        Private Sub DgStaff_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            Dim user = TryCast(DgStaff.SelectedItem, Global.AutoRent.Data.Models.User)
            If user Is Nothing Then Return

            _selectedUser = user
            _selectedId = user.UserId

            TxtSelTitle.Text = $"Selected account · {user.DisplayName}"
            TxtSelSubtitle.Text = user.StatusSubtitle
            TxtSelRole.Text = user.DisplayRole
            TxtSelCreated.Text = user.CreatedText
            TxtSelActivity.Text = user.LastActivityText
        End Sub

        Private Sub BtnEditAccount_Click(sender As Object, e As RoutedEventArgs)
            If _selectedUser Is Nothing Then Return

            TxtEditSubtitle.Text = $"Updating {_selectedUser.UserCode} · {_selectedUser.DisplayName}"
            TxtEditFullName.Text = _selectedUser.FullName
            TxtEditUsername.Text = _selectedUser.Username

            Dim isCurrentAdmin = UserAdminRepository.IsAdminRole(_selectedUser.RoleName) OrElse UserAdminRepository.IsAdminRole(_selectedUser.DisplayRole)
            If isCurrentAdmin Then
                CmbEditRole.ItemsSource = New String() {
                    "System Administrator",
                    "Manager",
                    "Rental Staff"
                }
            Else
                If UserAdminRepository.AdminExists() Then
                    CmbEditRole.ItemsSource = New String() {
                        "Rental Staff",
                        "Manager"
                    }
                Else
                    CmbEditRole.ItemsSource = New String() {
                        "Rental Staff",
                        "System Administrator",
                        "Manager"
                    }
                End If
            End If

            Dim currentRole = _selectedUser.RoleName
            Dim roleIndex = CmbEditRole.Items.IndexOf(currentRole)
            If roleIndex >= 0 Then
                CmbEditRole.SelectedIndex = roleIndex
            Else
                Dim displayRoleIndex = CmbEditRole.Items.IndexOf(_selectedUser.DisplayRole)
                If displayRoleIndex >= 0 Then
                    CmbEditRole.SelectedIndex = displayRoleIndex
                Else
                    CmbEditRole.SelectedIndex = 0
                End If
            End If

            ClearPasswordField(TxtEditPassword, TxtEditPasswordReveal, EyeIconEditPass)
            ClearPasswordField(TxtEditConfirmPassword, TxtEditConfirmPasswordReveal, EyeIconEditConfirm)
            EditUserModal.Visibility = Visibility.Visible
            TxtEditFullName.Focus()
        End Sub

        Private Sub BtnCloseEditModal_Click(sender As Object, e As RoutedEventArgs)
            EditUserModal.Visibility = Visibility.Collapsed
        End Sub

        Private Sub EditModalBackdrop_MouseDown(sender As Object, e As MouseButtonEventArgs)
            EditUserModal.Visibility = Visibility.Collapsed
        End Sub

        Private Sub BtnSaveEditUser_Click(sender As Object, e As RoutedEventArgs)
            If _selectedUser Is Nothing Then Return

            Dim fullName = TxtEditFullName.Text.Trim()
            Dim username = TxtEditUsername.Text.Trim()
            Dim role = TryCast(CmbEditRole.SelectedItem, String)
            Dim newPassword = GetPasswordValue(TxtEditPassword, TxtEditPasswordReveal)
            Dim confirmPassword = GetPasswordValue(TxtEditConfirmPassword, TxtEditConfirmPasswordReveal)

            If String.IsNullOrWhiteSpace(fullName) Then
                AlertUtil.ShowWarning("Full name cannot be blank.", "Edit Account", ViewHelpers.Owner(Me))
                Return
            End If

            If String.IsNullOrWhiteSpace(username) Then
                AlertUtil.ShowWarning("Username cannot be blank.", "Edit Account", ViewHelpers.Owner(Me))
                Return
            End If

            If Not String.IsNullOrEmpty(newPassword) Then
                If newPassword <> confirmPassword Then
                    AlertUtil.ShowWarning("New password and confirmation password do not match.", "Edit Account", ViewHelpers.Owner(Me))
                    Return
                End If

                Dim passErr = UserAdminRepository.ValidatePasswordStrength(newPassword)
                If passErr IsNot Nothing Then
                    AlertUtil.ShowWarning(passErr, "Edit Account", ViewHelpers.Owner(Me))
                    Return
                End If
            End If

            If Not AlertUtil.Confirm($"Are you sure you want to save changes to the account for '{fullName}'?", "Save Changes", ViewHelpers.Owner(Me)) Then
                Return
            End If

            Try
                Dim problem = UserAdminRepository.Update(_selectedUser.UserId, fullName, username, role, newPassword)
                If problem IsNot Nothing Then
                    AlertUtil.ShowWarning(problem, "Edit Account", ViewHelpers.Owner(Me))
                    Return
                End If

                EditUserModal.Visibility = Visibility.Collapsed
                AlertUtil.ShowSuccess($"Account for '{fullName}' was updated successfully.", "Edit Account", ViewHelpers.Owner(Me))
                LoadUsers(_selectedUser.UserId)
            Catch ex As Exception
                AlertUtil.ShowError($"Could not update user: {ex.Message}", "Edit Account", ViewHelpers.Owner(Me))
            End Try
        End Sub

        Private Sub BtnArchiveAccount_Click(sender As Object, e As RoutedEventArgs)
            If _selectedUser Is Nothing Then Return

            If Not AlertUtil.Confirm($"Are you sure you want to archive the account for '{_selectedUser.DisplayName}' ({_selectedUser.UserCode})? Archived accounts will be suspended from signing in and removed from the active directory.", "Archive Account", ViewHelpers.Owner(Me)) Then
                Return
            End If

            Try
                Dim problem = UserAdminRepository.Archive(_selectedUser.UserId, CurrentUserId())
                If problem IsNot Nothing Then
                    AlertUtil.ShowWarning(problem, "Archive Account", ViewHelpers.Owner(Me))
                    Return
                End If

                AlertUtil.ShowSuccess($"Staff account '{_selectedUser.DisplayName}' was archived.", "Archive Account", ViewHelpers.Owner(Me))
                LoadUsers()
            Catch ex As Exception
                AlertUtil.ShowError($"Could not archive user: {ex.Message}", "Archive Account", ViewHelpers.Owner(Me))
            End Try
        End Sub

        Private Sub BtnOpenCreateUser_Click(sender As Object, e As RoutedEventArgs)
            TxtNewFullName.Clear()
            TxtNewUsername.Clear()
            ClearPasswordField(TxtNewPassword, TxtNewPasswordReveal, EyeIconNewPass)
            ClearPasswordField(TxtNewConfirmPassword, TxtNewConfirmPasswordReveal, EyeIconNewConfirm)

            If UserAdminRepository.AdminExists() Then
                CmbNewRole.ItemsSource = New String() {
                    "Rental Staff",
                    "Manager"
                }
            Else
                CmbNewRole.ItemsSource = New String() {
                    "Rental Staff",
                    "System Administrator",
                    "Manager"
                }
            End If
            CmbNewRole.SelectedIndex = 0

            CreateUserModal.Visibility = Visibility.Visible
            TxtNewFullName.Focus()
        End Sub

        Private Sub BtnCloseCreateModal_Click(sender As Object, e As RoutedEventArgs)
            CreateUserModal.Visibility = Visibility.Collapsed
        End Sub

        Private Sub CreateModalBackdrop_MouseDown(sender As Object, e As MouseButtonEventArgs)
            CreateUserModal.Visibility = Visibility.Collapsed
        End Sub

        Private Sub BtnSaveNewUser_Click(sender As Object, e As RoutedEventArgs)
            Dim username = TxtNewUsername.Text.Trim()
            Dim fullName = TxtNewFullName.Text.Trim()
            Dim password = GetPasswordValue(TxtNewPassword, TxtNewPasswordReveal)
            Dim confirmPassword = GetPasswordValue(TxtNewConfirmPassword, TxtNewConfirmPasswordReveal)
            Dim role = TryCast(CmbNewRole.SelectedItem, String)

            If String.IsNullOrWhiteSpace(fullName) Then
                AlertUtil.ShowWarning("Please specify a full name.", "Create User", ViewHelpers.Owner(Me))
                Return
            End If

            If String.IsNullOrWhiteSpace(username) Then
                AlertUtil.ShowWarning("Please specify a username.", "Create User", ViewHelpers.Owner(Me))
                Return
            End If

            If String.IsNullOrEmpty(password) Then
                AlertUtil.ShowWarning("Please specify a password.", "Create User", ViewHelpers.Owner(Me))
                Return
            End If

            If password <> confirmPassword Then
                AlertUtil.ShowWarning("Password and confirmation password do not match.", "Create User", ViewHelpers.Owner(Me))
                Return
            End If

            Dim passErr = UserAdminRepository.ValidatePasswordStrength(password)
            If passErr IsNot Nothing Then
                AlertUtil.ShowWarning(passErr, "Create User", ViewHelpers.Owner(Me))
                Return
            End If

            If Not AlertUtil.Confirm($"Are you sure you want to create a new staff account for '{fullName}' ({username}) with role '{role}'?", "Create Account", ViewHelpers.Owner(Me)) Then
                Return
            End If

            Try
                Dim problem = UserAdminRepository.Create(username, password, role, fullName, "HQ", False)
                If problem IsNot Nothing Then
                    AlertUtil.ShowWarning(problem, "Create User", ViewHelpers.Owner(Me))
                    Return
                End If

                CreateUserModal.Visibility = Visibility.Collapsed
                AlertUtil.ShowSuccess($"Staff account '{username}' was created successfully in the database.", "Create User", ViewHelpers.Owner(Me))
                LoadUsers()
            Catch ex As Exception
                AlertUtil.ShowError($"Could not create user: {ex.Message}", "Create User", ViewHelpers.Owner(Me))
            End Try
        End Sub

        Private Sub ToggleNewPassword_Click(sender As Object, e As RoutedEventArgs)
            TogglePasswordField(TxtNewPassword, TxtNewPasswordReveal, EyeIconNewPass)
        End Sub

        Private Sub ToggleNewConfirmPassword_Click(sender As Object, e As RoutedEventArgs)
            TogglePasswordField(TxtNewConfirmPassword, TxtNewConfirmPasswordReveal, EyeIconNewConfirm)
        End Sub

        Private Sub ToggleEditPassword_Click(sender As Object, e As RoutedEventArgs)
            TogglePasswordField(TxtEditPassword, TxtEditPasswordReveal, EyeIconEditPass)
        End Sub

        Private Sub ToggleEditConfirmPassword_Click(sender As Object, e As RoutedEventArgs)
            TogglePasswordField(TxtEditConfirmPassword, TxtEditConfirmPasswordReveal, EyeIconEditConfirm)
        End Sub

        Private Sub TxtNewPasswordReveal_TextChanged(sender As Object, e As TextChangedEventArgs)
            SyncPasswordReveal(TxtNewPassword, TxtNewPasswordReveal)
        End Sub

        Private Sub TxtNewConfirmPasswordReveal_TextChanged(sender As Object, e As TextChangedEventArgs)
            SyncPasswordReveal(TxtNewConfirmPassword, TxtNewConfirmPasswordReveal)
        End Sub

        Private Sub TxtEditPasswordReveal_TextChanged(sender As Object, e As TextChangedEventArgs)
            SyncPasswordReveal(TxtEditPassword, TxtEditPasswordReveal)
        End Sub

        Private Sub TxtEditConfirmPasswordReveal_TextChanged(sender As Object, e As TextChangedEventArgs)
            SyncPasswordReveal(TxtEditConfirmPassword, TxtEditConfirmPasswordReveal)
        End Sub

        Private Sub TogglePasswordField(passBox As PasswordBox, revealBox As TextBox, eyeIcon As TextBlock)
            If passBox.Visibility = Visibility.Visible Then
                revealBox.Text = passBox.Password
                passBox.Visibility = Visibility.Collapsed
                revealBox.Visibility = Visibility.Visible
                revealBox.Focus()
                revealBox.CaretIndex = revealBox.Text.Length
                eyeIcon.Text = ChrW(&HED1A)
            Else
                passBox.Password = revealBox.Text
                revealBox.Visibility = Visibility.Collapsed
                passBox.Visibility = Visibility.Visible
                passBox.Focus()
                eyeIcon.Text = ChrW(&HE7B3)
            End If
        End Sub

        Private Function GetPasswordValue(passBox As PasswordBox, revealBox As TextBox) As String
            If passBox.Visibility = Visibility.Visible Then
                Return passBox.Password.Trim()
            Else
                Return revealBox.Text.Trim()
            End If
        End Function

        Private Sub ClearPasswordField(passBox As PasswordBox, revealBox As TextBox, eyeIcon As TextBlock)
            passBox.Clear()
            revealBox.Clear()
            passBox.Visibility = Visibility.Visible
            revealBox.Visibility = Visibility.Collapsed
            eyeIcon.Text = ChrW(&HE7B3)
        End Sub

        Private Sub SyncPasswordReveal(passBox As PasswordBox, revealBox As TextBox)
            If revealBox.Visibility = Visibility.Visible Then
                passBox.Password = revealBox.Text
            End If
        End Sub

        Private Sub UpdateArchivedBadge()
            Try
                Dim archived = UserAdminRepository.GetArchived()
                TxtArchivedButtonLabel.Text = If(archived.Count > 0, $"Archived ({archived.Count})", "Archived")
            Catch ex As Exception
            End Try
        End Sub

        Private Sub BtnViewArchived_Click(sender As Object, e As RoutedEventArgs)
            LoadArchivedUsers()
            ArchivedUsersModal.Visibility = Visibility.Visible
        End Sub

        Private Sub BtnCloseArchivedModal_Click(sender As Object, e As RoutedEventArgs)
            ArchivedUsersModal.Visibility = Visibility.Collapsed
        End Sub

        Private Sub ArchivedModalBackdrop_MouseDown(sender As Object, e As MouseButtonEventArgs)
            ArchivedUsersModal.Visibility = Visibility.Collapsed
        End Sub

        Private Sub LoadArchivedUsers()
            Try
                Dim archived = UserAdminRepository.GetArchived()
                DgArchivedStaff.ItemsSource = archived
                TxtArchivedCount.Text = $"{archived.Count} archived account" & If(archived.Count = 1, "", "s")
                TxtArchivedButtonLabel.Text = If(archived.Count > 0, $"Archived ({archived.Count})", "Archived")

                If archived.Count = 0 Then
                    TxtNoArchivedUsers.Visibility = Visibility.Visible
                    BorderArchivedTable.Visibility = Visibility.Collapsed
                Else
                    TxtNoArchivedUsers.Visibility = Visibility.Collapsed
                    BorderArchivedTable.Visibility = Visibility.Visible
                End If
            Catch ex As Exception
                AlertUtil.ShowError($"Could not load archived accounts: {ex.Message}", "Archived Accounts", ViewHelpers.Owner(Me))
            End Try
        End Sub

        Private Sub BtnRestoreUser_Click(sender As Object, e As RoutedEventArgs)
            Dim btn = TryCast(sender, Button)
            If btn Is Nothing OrElse btn.Tag Is Nothing Then Return

            Dim userId As Integer
            If Not Integer.TryParse(btn.Tag.ToString(), userId) Then Return

            Dim archivedList = TryCast(DgArchivedStaff.ItemsSource, List(Of Global.AutoRent.Data.Models.User))
            Dim user = If(archivedList IsNot Nothing, archivedList.FirstOrDefault(Function(u) u.UserId = userId), Nothing)
            Dim targetName = If(user IsNot Nothing, $"{user.DisplayName} ({user.UserCode})", "this account")

            If Not AlertUtil.Confirm($"Are you sure you want to restore {targetName} to the staff directory?", "Restore Account", ViewHelpers.Owner(Me)) Then
                Return
            End If

            Try
                Dim problem = UserAdminRepository.Restore(userId)
                If problem IsNot Nothing Then
                    AlertUtil.ShowWarning(problem, "Restore Account", ViewHelpers.Owner(Me))
                    Return
                End If

                AlertUtil.ShowSuccess("Staff account was restored to the staff directory.", "Restore Account", ViewHelpers.Owner(Me))
                LoadArchivedUsers()
                LoadUsers(userId)
            Catch ex As Exception
                AlertUtil.ShowError($"Could not restore account: {ex.Message}", "Restore Account", ViewHelpers.Owner(Me))
            End Try
        End Sub

        Private Sub BtnPermanentDeleteUser_Click(sender As Object, e As RoutedEventArgs)
            Dim btn = TryCast(sender, Button)
            If btn Is Nothing OrElse btn.Tag Is Nothing Then Return

            Dim userId As Integer
            If Not Integer.TryParse(btn.Tag.ToString(), userId) Then Return

            Dim archivedList = TryCast(DgArchivedStaff.ItemsSource, List(Of Global.AutoRent.Data.Models.User))
            Dim user = If(archivedList IsNot Nothing, archivedList.FirstOrDefault(Function(u) u.UserId = userId), Nothing)
            Dim targetName = If(user IsNot Nothing, $"{user.DisplayName} ({user.UserCode})", "this account")

            If Not AlertUtil.Confirm($"Are you sure you want to PERMANENTLY delete {targetName}? This action cannot be undone and will erase the account from the database.", "Delete Account Permanently", ViewHelpers.Owner(Me)) Then
                Return
            End If

            Try
                Dim problem = UserAdminRepository.PermanentDelete(userId, CurrentUserId())
                If problem IsNot Nothing Then
                    AlertUtil.ShowWarning(problem, "Delete Account", ViewHelpers.Owner(Me))
                    Return
                End If

                AlertUtil.ShowSuccess("Staff account was permanently deleted.", "Delete Account", ViewHelpers.Owner(Me))
                LoadArchivedUsers()
                UpdateArchivedBadge()
            Catch ex As Exception
                AlertUtil.ShowError($"Could not delete account: {ex.Message}", "Delete Account", ViewHelpers.Owner(Me))
            End Try
        End Sub
    End Class
End Namespace
