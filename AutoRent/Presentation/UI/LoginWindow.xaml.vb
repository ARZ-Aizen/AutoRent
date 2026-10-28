Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Threading.Tasks
Imports AutoRent.Common.Utilities
Imports AutoRent.Data.Repositories

Namespace AutoRent
    Partial Public Class LoginWindow
        Inherits Window

        Public Sub New()
            InitializeComponent()

            InputBehavior.Attach(UsernameTextBox, "username", 32)

            AddHandler UsernameTextBox.TextChanged, AddressOf UsernameTextBox_TextChanged
            AddHandler UsernameTextBox.PreviewKeyDown, AddressOf UsernameTextBox_PreviewKeyDown
            AddHandler PasswordBox.PasswordChanged, AddressOf PasswordBox_PasswordChanged
            AddHandler PasswordBox.PreviewKeyDown, AddressOf PasswordField_PreviewKeyDown
            AddHandler PasswordRevealBox.TextChanged, AddressOf PasswordRevealBox_TextChanged
            AddHandler PasswordRevealBox.PreviewKeyDown, AddressOf PasswordField_PreviewKeyDown
            AddHandler TogglePasswordButton.Click, AddressOf TogglePasswordButton_Click
            AddHandler LoginButton.Click, AddressOf LoginButton_Click
            AddHandler Loaded, Sub() UsernameTextBox.Focus()
        End Sub

        Private Sub UsernameTextBox_PreviewKeyDown(sender As Object, e As KeyEventArgs)
            If e.Key = Key.Enter Then
                e.Handled = True
                FocusPasswordField()
            End If
        End Sub

        Private Sub PasswordField_PreviewKeyDown(sender As Object, e As KeyEventArgs)
            If e.Key = Key.Enter Then
                e.Handled = True
                If LoginButton.IsEnabled Then
                    LoginButton_Click(LoginButton, New RoutedEventArgs())
                End If
            End If
        End Sub

        Private Sub FocusPasswordField()
            If PasswordBox.Visibility = Visibility.Visible Then
                PasswordBox.Focus()
                PasswordBox.SelectAll()
            Else
                PasswordRevealBox.Focus()
                PasswordRevealBox.SelectAll()
            End If
        End Sub

        Private Sub UsernameTextBox_TextChanged(sender As Object, e As TextChangedEventArgs)
            UsernamePlaceholder.Visibility = If(String.IsNullOrEmpty(UsernameTextBox.Text), Visibility.Visible, Visibility.Collapsed)
        End Sub

        Private Sub PasswordBox_PasswordChanged(sender As Object, e As RoutedEventArgs)
            UpdatePasswordPlaceholder()
        End Sub

        Private Sub PasswordRevealBox_TextChanged(sender As Object, e As TextChangedEventArgs)
            If PasswordRevealBox.Visibility = Visibility.Visible Then
                PasswordBox.Password = PasswordRevealBox.Text
            End If
            UpdatePasswordPlaceholder()
        End Sub

        Private Sub UpdatePasswordPlaceholder()
            Dim passwordIsEmpty = If(PasswordBox.Visibility = Visibility.Visible,
                                     String.IsNullOrEmpty(PasswordBox.Password),
                                     String.IsNullOrEmpty(PasswordRevealBox.Text))
            PasswordPlaceholder.Visibility = If(passwordIsEmpty, Visibility.Visible, Visibility.Collapsed)
        End Sub

        Private Sub TogglePasswordButton_Click(sender As Object, e As RoutedEventArgs)
            If PasswordBox.Visibility = Visibility.Visible Then
                PasswordRevealBox.Text = PasswordBox.Password
                PasswordBox.Visibility = Visibility.Collapsed
                PasswordRevealBox.Visibility = Visibility.Visible
                PasswordRevealBox.Focus()
                PasswordRevealBox.CaretIndex = PasswordRevealBox.Text.Length
                EyeIcon.Text = ChrW(&HED1A)
            Else
                PasswordBox.Password = PasswordRevealBox.Text
                PasswordRevealBox.Visibility = Visibility.Collapsed
                PasswordBox.Visibility = Visibility.Visible
                PasswordBox.Focus()
                EyeIcon.Text = ChrW(&HE7B3)
            End If
        End Sub

        Private Function ValidateCredentials(username As String, password As String) As Boolean
            If String.IsNullOrWhiteSpace(username) Then
                AlertUtil.ShowWarning("Enter your username.", "Login validation", Me)
                UsernameTextBox.Focus()
                Return False
            End If

            Dim userFormatError = InputValidation.Username(username)
            If userFormatError IsNot Nothing Then
                AlertUtil.ShowWarning(userFormatError, "Login validation", Me)
                UsernameTextBox.Focus()
                Return False
            End If

            If String.IsNullOrEmpty(password) Then
                AlertUtil.ShowWarning("Enter your password.", "Login validation", Me)
                If PasswordBox.Visibility = Visibility.Visible Then
                    PasswordBox.Focus()
                Else
                    PasswordRevealBox.Focus()
                End If
                Return False
            End If

            If password.Length > 256 Then
                AlertUtil.ShowWarning("Password cannot exceed 256 characters.", "Login validation", Me)
                Return False
            End If

            Return True
        End Function

        Private Async Sub LoginButton_Click(sender As Object, e As RoutedEventArgs)
            Dim username = UsernameTextBox.Text.Trim()
            Dim password = If(PasswordBox.Visibility = Visibility.Visible, PasswordBox.Password, PasswordRevealBox.Text)

            If Not ValidateCredentials(username, password) Then Return

            LoginButton.IsEnabled = False
            Try
                Dim authenticatedUser = Await Task.Run(Function() UserRepository.Authenticate(username, password))
                If authenticatedUser Is Nothing Then
                    AlertUtil.ShowError("Incorrect username or password.", "Login failed", Me)
                    Return
                End If

                Dim application = CType(System.Windows.Application.Current, Global.AutoRent.Application)
                application.CurrentUser = authenticatedUser
                AlertUtil.ShowSuccess($"Login successful. Welcome, {authenticatedUser.Username}!", "Welcome", Me)

                Dim mainWindow As New Global.AutoRent.MainWindow()
                mainWindow.Title = $"AutoRent - {authenticatedUser.Username}"
                application.MainWindow = mainWindow
                _isLoggingIn = True
                mainWindow.Show()
                Close()
            Catch ex As Exception
                AlertUtil.ShowError($"Sign-in failed because the user database could not be accessed: {ex.Message}", "Login Error", Me)
            Finally
                LoginButton.IsEnabled = True
            End Try
        End Sub

        Private _isLoggingIn As Boolean = False
        Private _isPromptingExit As Boolean = False

        Private Sub LoginWindow_PreviewKeyDown(sender As Object, e As KeyEventArgs) Handles Me.PreviewKeyDown
            If Keyboard.Modifiers.HasFlag(ModifierKeys.Alt) AndAlso e.SystemKey = Key.F4 Then
                e.Handled = True
                Me.Close()
                Return
            End If
        End Sub

        Private Sub LoginWindow_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.Closing
            If _isLoggingIn Then Return
            If _isPromptingExit Then Return

            _isPromptingExit = True
            Try
                If Not AlertUtil.Confirm("Are you sure you want to exit AutoRent?", "Exit Application", Me) Then
                    e.Cancel = True
                    Return
                End If
            Finally
                _isPromptingExit = False
            End Try
        End Sub

    End Class
End Namespace
