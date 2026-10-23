Imports System.Windows
Imports System.Windows.Media
Imports AutoRent.Common.Utilities
Imports AutoRent.Data.Models
Imports AutoRent.Presentation.Views

Class MainWindow
    Private _currentViewIsAdmin As Boolean = False

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim app = CType(System.Windows.Application.Current, Global.AutoRent.Application)
        Dim currentUser = app.CurrentUser

        If currentUser IsNot Nothing Then
            UsernameDisplay.Text = currentUser.Username
            UserRoleDisplay.Text = currentUser.Role.ToString().ToUpperInvariant()
            Me.Title = $"AutoRent - {currentUser.Username} ({currentUser.Role})"

            If currentUser.Role = UserRole.Admin Then
                SwitchViewButton.Visibility = Visibility.Visible
                LoadAdminView()
            Else
                SwitchViewButton.Visibility = Visibility.Collapsed
                LoadClientView()
            End If
        Else
            ' Fallback if started without a session
            LoadClientView()
        End If
    End Sub

    Private Sub LoadAdminView()
        _currentViewIsAdmin = True
        MainContentHost.Content = New AdminView()
        CurrentViewBadgeText.Text = "ADMIN CONSOLE"
        CurrentViewBadge.Background = CType(FindResource("MaroonSubtle"), Brush)
        CurrentViewBadgeText.Foreground = CType(FindResource("Maroon"), Brush)
        SwitchText.Text = "Preview Client View"
    End Sub

    Private Sub LoadClientView()
        _currentViewIsAdmin = False
        MainContentHost.Content = New ClientView()
        CurrentViewBadgeText.Text = "CLIENT PORTAL"
        CurrentViewBadge.Background = New SolidColorBrush(Color.FromRgb(232, 241, 252))
        CurrentViewBadgeText.Foreground = New SolidColorBrush(Color.FromRgb(31, 86, 155))
        SwitchText.Text = "Switch to Admin Console"
    End Sub

    Private Sub SwitchViewButton_Click(sender As Object, e As RoutedEventArgs) Handles SwitchViewButton.Click
        If _currentViewIsAdmin Then
            LoadClientView()
        Else
            LoadAdminView()
        End If
    End Sub

    Private Sub LogoutButton_Click(sender As Object, e As RoutedEventArgs) Handles LogoutButton.Click
        If AlertUtil.Confirm("Are you sure you want to log out of your session?", "Sign Out", Me) Then
            Dim app = CType(System.Windows.Application.Current, Global.AutoRent.Application)
            app.CurrentUser = Nothing

            Dim loginWindow As New Global.AutoRent.AutoRent.LoginWindow()
            loginWindow.Show()
            Me.Close()
        End If
    End Sub
End Class
