Imports System.Data
Imports System.Windows
Imports System.Windows.Controls
Imports AutoRent.Common.Utilities
Imports AutoRent.Data.Repositories

Namespace Presentation.Views
    Partial Public Class UserManagementView
        Inherits UserControl

        Private _selectedId As Integer = 0
        Private _selectedName As String = ""
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
            CmbRole.ItemsSource = New String() {"User", "Admin"}
            _ready = True
            LoadUsers()
            ClearForm()
        End Sub

        Private Sub LoadUsers()
            Try
                Dim table = UserAdminRepository.GetAll()
                DgUsers.ItemsSource = table.DefaultView
                TxtCount.Text = $"Users ({table.Rows.Count})"
            Catch ex As Exception
                AlertUtil.ShowError($"Could not load users: {ex.Message}", "User Management", ViewHelpers.Owner(Me))
            End Try
        End Sub

        Private Sub ClearForm()
            _selectedId = 0
            _selectedName = ""
            DgUsers.SelectedItem = Nothing
            TxtUsername.Clear()
            TxtUsername.IsReadOnly = False
            TxtPassword.Clear()
            CmbRole.SelectedItem = "User"
            ChkActive.IsChecked = True
            LblPassword.Text = "PASSWORD"
            TxtHint.Text = $"New accounts need a password of at least {UserAdminRepository.MinPasswordLength} characters."
            BtnAdd.IsEnabled = True
            BtnSave.IsEnabled = False
            BtnReset.IsEnabled = False
            BtnDelete.IsEnabled = False
        End Sub

        Private Sub DgUsers_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles DgUsers.SelectionChanged
            Dim row = TryCast(DgUsers.SelectedItem, DataRowView)
            If row Is Nothing Then Return

            _selectedId = ViewHelpers.IntOf(row, "user_id")
            _selectedName = ViewHelpers.Str(row, "username")
            TxtUsername.Text = _selectedName
            TxtUsername.IsReadOnly = True
            TxtPassword.Clear()
            CmbRole.SelectedItem = ViewHelpers.Str(row, "role")
            ChkActive.IsChecked = Convert.ToBoolean(row("is_active"))
            LblPassword.Text = "NEW PASSWORD"
            TxtHint.Text = "Change the role or status and press Save, or type a new password and press Reset Password."

            BtnAdd.IsEnabled = False
            BtnSave.IsEnabled = True
            BtnReset.IsEnabled = True
            BtnDelete.IsEnabled = True
        End Sub

        Private Sub BtnAdd_Click(sender As Object, e As RoutedEventArgs) Handles BtnAdd.Click
            Try
                Dim problem = UserAdminRepository.Create(TxtUsername.Text, TxtPassword.Password, TryCast(CmbRole.SelectedItem, String), ChkActive.IsChecked = True)
                If problem IsNot Nothing Then
                    AlertUtil.ShowWarning(problem, "User Management", ViewHelpers.Owner(Me))
                    Return
                End If
                AlertUtil.ShowSuccess($"User '{TxtUsername.Text.Trim()}' was created.", "User Management", ViewHelpers.Owner(Me))
                LoadUsers()
                ClearForm()
            Catch ex As Exception
                AlertUtil.ShowError($"Could not create the user: {ex.Message}", "User Management", ViewHelpers.Owner(Me))
            End Try
        End Sub

        Private Sub BtnSave_Click(sender As Object, e As RoutedEventArgs) Handles BtnSave.Click
            If _selectedId = 0 Then Return

            Try
                Dim problem = UserAdminRepository.UpdateAccess(_selectedId, TryCast(CmbRole.SelectedItem, String), ChkActive.IsChecked = True, CurrentUserId())
                If problem IsNot Nothing Then
                    AlertUtil.ShowWarning(problem, "User Management", ViewHelpers.Owner(Me))
                    Return
                End If
                LoadUsers()
                ClearForm()
            Catch ex As Exception
                AlertUtil.ShowError($"Could not save the user: {ex.Message}", "User Management", ViewHelpers.Owner(Me))
            End Try
        End Sub

        Private Sub BtnReset_Click(sender As Object, e As RoutedEventArgs) Handles BtnReset.Click
            If _selectedId = 0 Then Return

            Try
                Dim problem = UserAdminRepository.ResetPassword(_selectedId, TxtPassword.Password)
                If problem IsNot Nothing Then
                    AlertUtil.ShowWarning(problem, "User Management", ViewHelpers.Owner(Me))
                    Return
                End If
                AlertUtil.ShowSuccess($"Password for '{_selectedName}' was reset.", "User Management", ViewHelpers.Owner(Me))
                TxtPassword.Clear()
            Catch ex As Exception
                AlertUtil.ShowError($"Could not reset the password: {ex.Message}", "User Management", ViewHelpers.Owner(Me))
            End Try
        End Sub

        Private Sub BtnClear_Click(sender As Object, e As RoutedEventArgs) Handles BtnClear.Click
            ClearForm()
        End Sub

        Private Sub BtnDelete_Click(sender As Object, e As RoutedEventArgs) Handles BtnDelete.Click
            If _selectedId = 0 Then Return
            If Not AlertUtil.Confirm($"Delete user '{_selectedName}'? They will no longer be able to sign in.", "Delete User", ViewHelpers.Owner(Me)) Then Return

            Try
                Dim problem = UserAdminRepository.Delete(_selectedId, CurrentUserId())
                If problem IsNot Nothing Then
                    AlertUtil.ShowWarning(problem, "User Management", ViewHelpers.Owner(Me))
                    Return
                End If
                LoadUsers()
                ClearForm()
            Catch ex As Exception
                AlertUtil.ShowError($"Could not delete the user: {ex.Message}", "User Management", ViewHelpers.Owner(Me))
            End Try
        End Sub
    End Class
End Namespace
