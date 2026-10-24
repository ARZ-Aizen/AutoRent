Imports System.Collections.Generic
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports AutoRent.Common.Utilities
Imports AutoRent.Data.Models
Imports AutoRent.Presentation.Views

Public Class NavItem
    Public Property Icon As String
    Public Property Title As String
    Public Property Key As String

    Public Sub New(icon As String, title As String, key As String)
        Me.Icon = icon
        Me.Title = title
        Me.Key = key
    End Sub
End Class

Class MainWindow
    Private Const ExpandedWidth As Double = 264
    Private Const CollapsedWidth As Double = 72

    Private _currentViewIsAdmin As Boolean = False
    Private _isExpanded As Boolean = False   ' starts collapsed
    Private _clientView As ClientView        ' created once, reused

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

    ' ---------- Sidebar expand / collapse ----------

    Private Sub LogoToggle_Click(sender As Object, e As RoutedEventArgs) Handles LogoToggle.Click
        SetSidebarExpanded(Not _isExpanded)
    End Sub

    Private Sub SetSidebarExpanded(expand As Boolean)
        _isExpanded = expand

        ' Collapsing: hide labels first so they don't get squished during the animation
        If Not expand Then SetLabelsVisible(False)

        Dim anim As New DoubleAnimation With {
            .To = If(expand, ExpandedWidth, CollapsedWidth),
            .Duration = TimeSpan.FromMilliseconds(200),
            .EasingFunction = New CubicEase With {.EasingMode = EasingMode.EaseInOut}
        }

        ' Expanding: show labels once there is room for them
        If expand Then
            AddHandler anim.Completed, Sub(s, args) SetLabelsVisible(True)
        End If

        Sidebar.BeginAnimation(FrameworkElement.WidthProperty, anim)
    End Sub

    Private Sub SetLabelsVisible(visible As Boolean)
        Dim v As Visibility = If(visible, Visibility.Visible, Visibility.Collapsed)

        BrandText.Visibility = v
        UserText.Visibility = v
        SettingsLabel.Visibility = v
        LogoutLabel.Visibility = v
        SwitchText.Visibility = v

        NavList.Tag = If(visible, "Expanded", "Collapsed")
    End Sub

    ' ---------- ADMIN VIEW (uses the MainWindow sidebar) ----------

    Private Sub LoadAdminView()
        _currentViewIsAdmin = True
        Sidebar.Visibility = Visibility.Visible       ' show the admin sidebar

        CurrentViewBadgeText.Text = "ADMIN CONSOLE"
        CurrentViewBadgeText.Foreground = New SolidColorBrush(Color.FromRgb(192, 57, 75))
        SwitchText.Text = "Preview Client View"

        NavList.ItemsSource = New List(Of NavItem) From {
            New NavItem(ChrW(&HE80F), "Home Dashboard", "Home"),
            New NavItem(ChrW(&HE804), "Fleet Management", "Fleet"),
            New NavItem(ChrW(&HE90F), "Maintenance Schedules", "Maintenance"),
            New NavItem(ChrW(&HE8EC), "Rate Management", "Rates"),
            New NavItem(ChrW(&HE9D9), "Financial Reports", "Reports"),
            New NavItem(ChrW(&HE716), "User Management", "Users")
        }
        NavList.SelectedIndex = 0   ' fires SelectionChanged -> loads first page
    End Sub

    ' ---------- CLIENT VIEW (ClientView has its own sidebar) ----------

    Private Sub LoadClientView()
        _currentViewIsAdmin = False
        Sidebar.Visibility = Visibility.Collapsed     ' hide the MainWindow sidebar

        If _clientView Is Nothing Then
            _clientView = New ClientView()
            AddHandler _clientView.SwitchToAdminRequested, AddressOf ClientView_SwitchToAdmin
            AddHandler _clientView.SettingsRequested, AddressOf ClientView_Settings
            AddHandler _clientView.SignOutRequested, AddressOf ClientView_SignOut
        End If

        Dim app = CType(System.Windows.Application.Current, Global.AutoRent.Application)
        Dim currentUser = app.CurrentUser
        If currentUser IsNot Nothing Then
            _clientView.UserName = currentUser.Username
            _clientView.UserRole = currentUser.Role.ToString()
            _clientView.CanSwitchToAdmin = (currentUser.Role = UserRole.Admin)
        Else
            _clientView.CanSwitchToAdmin = False
        End If

        MainContentHost.Content = _clientView
    End Sub

    Private Sub ClientView_SwitchToAdmin(sender As Object, e As EventArgs)
        LoadAdminView()
    End Sub

    Private Sub ClientView_Settings(sender As Object, e As EventArgs)
        SettingsButton_Click(sender, New RoutedEventArgs())
    End Sub

    Private Sub ClientView_SignOut(sender As Object, e As EventArgs)
        LogoutButton_Click(sender, New RoutedEventArgs())
    End Sub

    ' ---------- Navigation (admin sidebar only) ----------

    Private Sub NavList_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles NavList.SelectionChanged
        Dim item = TryCast(NavList.SelectedItem, NavItem)
        If item Is Nothing Then Return
        If Not _currentViewIsAdmin Then Return

        Select Case item.Key
            ' Uncomment each line as you create the UserControl
            ' Case "Home"        : MainContentHost.Content = New HomeDashboardView()
            ' Case "Fleet"       : MainContentHost.Content = New FleetManagementView()
            ' Case "Maintenance" : MainContentHost.Content = New MaintenanceSchedulesView()
            ' Case "Rates"       : MainContentHost.Content = New RateManagementView()
            ' Case "Reports"     : MainContentHost.Content = New FinancialReportsView()
            ' Case "Users"       : MainContentHost.Content = New UserManagementView()
            Case Else
                MainContentHost.Content = New AdminView()   ' temporary fallback
        End Select
    End Sub

    Private Sub SwitchViewButton_Click(sender As Object, e As RoutedEventArgs) Handles SwitchViewButton.Click
        If _currentViewIsAdmin Then
            LoadClientView()
        Else
            LoadAdminView()
        End If
    End Sub

    Private Sub SettingsButton_Click(sender As Object, e As RoutedEventArgs) Handles SettingsButton.Click
        ' TODO: open settings
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