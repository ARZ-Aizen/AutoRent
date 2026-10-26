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
            Dim app = CType(System.Windows.Application.Current, Global.AutoRent.Application)
            Return If(app.CurrentUser IsNot Nothing, app.CurrentUser.UserId, 0)
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

            CmbNewRole.ItemsSource = New String() {
                "Rental Staff",
                "System Administrator",
                "Manager"
            }
            CmbNewRole.SelectedIndex = 0

            CmbEditRole.ItemsSource = New String() {
                "Rental Staff",
                "System Administrator",
                "Manager"
            }
            CmbEditRole.SelectedIndex = 0

            CmbEditRole.ItemsSource = New String() {
                "Rental Clerk",
                "System Administrator",
                "Business Owner / Branch Manager",
                "Dispatch Officer",
                "Front-Desk Staff"
            }
            CmbEditRole.SelectedIndex = 0

            _ready = True
            LoadUsers()
        End Sub

        Private Sub LoadUsers(Optional selectUserId As Integer = 0)
            Try
                _allUsers = UserAdminRepository.GetAll()
                ApplyFilter(selectUserId)
            Catch ex As Exception
                AlertUtil.ShowError($"Could not load users: {ex.Message}", "User Management", ViewHelpers.Owner(Me))
            End Try
        End Sub

        Private Sub UpdateHeaderCounters(filteredCount As Integer)
            Dim total = _allUsers.Count
            Dim activeCount = _allUsers.Where(Function(u) u.StatusBadge = "Active").Count()
<<<<<<< HEAD
            Dim inactiveCount = _allUsers.Where(Function(u) u.StatusBadge = "Inactive").Count()

            TxtHeaderStats.Text = $"{total} staff accounts · {activeCount} active · {inactiveCount} inactive · passwords are never displayed"
=======
            Dim pendingCount = _allUsers.Where(Function(u) u.StatusBadge = "Invite pending").Count()
            Dim deactivatedCount = _allUsers.Where(Function(u) u.StatusBadge = "Deactivated").Count()

            TxtHeaderStats.Text = $"{total} staff accounts · {activeCount} active · {pendingCount} invitation pending · {deactivatedCount} deactivated · passwords are never displayed"
>>>>>>> fb67b0a79680152b8c70af7d784c7652495f2813
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
<<<<<<< HEAD
=======
            Dim selectedBranch = TryCast(CmbBranchFilter.SelectedItem, String)
>>>>>>> fb67b0a79680152b8c70af7d784c7652495f2813
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

<<<<<<< HEAD
=======
                If Not String.IsNullOrEmpty(selectedBranch) AndAlso selectedBranch <> "All branches" Then
                    If Not u.Branch.Equals(selectedBranch, StringComparison.OrdinalIgnoreCase) Then Return False
                End If

>>>>>>> fb67b0a79680152b8c70af7d784c7652495f2813
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
<<<<<<< HEAD
            TxtSelCreated.Text = user.CreatedText
            TxtSelActivity.Text = user.LastActivityText
=======
            TxtSelInvite.Text = user.InvitationText
            TxtSelActivity.Text = user.LastActivityText
            TxtSelAuthority.Text = user.AccountAuthority
>>>>>>> fb67b0a79680152b8c70af7d784c7652495f2813
        End Sub

        Private Sub BtnEditAccount_Click(sender As Object, e As RoutedEventArgs)
            If _selectedUser Is Nothing Then Return

            TxtEditSubtitle.Text = $"Updating {_selectedUser.UserCode} · {_selectedUser.DisplayName}"
            TxtEditFullName.Text = _selectedUser.FullName
            TxtEditUsername.Text = _selectedUser.Username

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

            TxtEditPassword.Clear()
            EditUserModal.Visibility = Visibility.Visible
            TxtEditFullName.Focus()
        End Sub

        Private Sub BtnGenerateEditPassword_Click(sender As Object, e As RoutedEventArgs)
            TxtEditPassword.Text = GenerateSecurePassword()
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
            Dim newPassword = TxtEditPassword.Text.Trim()

            If String.IsNullOrWhiteSpace(fullName) Then
                AlertUtil.ShowWarning("Full name cannot be blank.", "Edit Account", ViewHelpers.Owner(Me))
                Return
            End If

            If String.IsNullOrWhiteSpace(username) Then
                AlertUtil.ShowWarning("Username cannot be blank.", "Edit Account", ViewHelpers.Owner(Me))
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
            TxtNewPassword.Text = GenerateSecurePassword()
            CmbNewRole.SelectedIndex = 0
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
            Dim fullName = TxtNewFullName.Text.Trim()
            Dim password = TxtNewPassword.Text.Trim()
            Dim role = TryCast(CmbNewRole.SelectedItem, String)
            Dim branch = TryCast(CmbNewBranch.SelectedItem, String)
            Dim isActive = (ChkNewActive.IsChecked = True)

            If String.IsNullOrWhiteSpace(username) Then
                AlertUtil.ShowWarning("Please specify a username.", "Create User", ViewHelpers.Owner(Me))
                Return
            End If

            Try
<<<<<<< HEAD
                Dim problem = UserAdminRepository.Create(username, password, role, fullName, "HQ", isActive)
=======
                Dim problem = UserAdminRepository.Create(username, password, role, fullName, branch, isActive)
>>>>>>> fb67b0a79680152b8c70af7d784c7652495f2813
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



        Private Shared Function GenerateSecurePassword() As String
            Dim bytes = RandomNumberGenerator.GetBytes(6)
            Return "AutoRent#" & Convert.ToHexString(bytes).Substring(0, 6) & "!"
        End Function
    End Class
End Namespace
