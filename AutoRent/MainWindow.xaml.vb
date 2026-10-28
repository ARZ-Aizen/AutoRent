Imports System.Collections.Generic
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports AutoRent.Common.Utilities
Imports AutoRent.Data.Models
Imports AutoRent.Data.Repositories
Imports AutoRent.Presentation.Views.Admin
Imports AutoRent.Presentation.Views.User

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
    Private _isExpanded As Boolean = False
    Private _hoverExpanded As Boolean = False
    Private _clientView As ClientView

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim app = CType(System.Windows.Application.Current, Global.AutoRent.Application)
        Dim currentUser = app.CurrentUser

        If currentUser IsNot Nothing Then
            UsernameDisplay.Text = currentUser.DisplayName
            UserRoleDisplay.Text = currentUser.DisplayRole.ToUpperInvariant()
            Me.Title = $"AutoRent - {currentUser.DisplayName} ({currentUser.DisplayRole})"

            ' Role access rules:
            ' 1. Admin: Admin side ONLY
            ' 2. Manager: Can access BOTH sides (can switch)
            ' 3. Rental Staff: Client side ONLY
            If currentUser.IsAdmin Then
                SwitchViewButton.Visibility = Visibility.Collapsed
                LoadAdminView()
            ElseIf currentUser.IsManager Then
                SwitchViewButton.Visibility = Visibility.Visible
                SwitchText.Text = "Switch to Client"
                LoadAdminView()
            Else
                SwitchViewButton.Visibility = Visibility.Collapsed
                LoadClientView()
            End If
        Else
            LoadClientView()
        End If
    End Sub

    Private Sub LogoToggle_Click(sender As Object, e As RoutedEventArgs) Handles LogoToggle.Click
        If _hoverExpanded Then
            _hoverExpanded = False
        Else
            SetSidebarExpanded(Not _isExpanded)
        End If
    End Sub

    Private Sub Sidebar_MouseEnter(sender As Object, e As MouseEventArgs) Handles Sidebar.MouseEnter
        If Not _isExpanded Then
            _hoverExpanded = True
            SetSidebarExpanded(True)
        End If
    End Sub

    Private Sub Sidebar_MouseLeave(sender As Object, e As MouseEventArgs) Handles Sidebar.MouseLeave
        If _hoverExpanded Then
            _hoverExpanded = False
            SetSidebarExpanded(False)
        End If
    End Sub

    Private Sub SetSidebarExpanded(expand As Boolean)
        _isExpanded = expand

        If Not expand Then SetLabelsVisible(False)

        Dim anim As New DoubleAnimation With {
            .To = If(expand, ExpandedWidth, CollapsedWidth),
            .Duration = TimeSpan.FromMilliseconds(200),
            .EasingFunction = New CubicEase With {.EasingMode = EasingMode.EaseInOut}
        }

        If expand Then
            AddHandler anim.Completed, Sub(s, args) SetLabelsVisible(True)
        End If

        Sidebar.BeginAnimation(FrameworkElement.WidthProperty, anim)
    End Sub

    Private Sub SetLabelsVisible(visible As Boolean)
        Dim v As Visibility = If(visible, Visibility.Visible, Visibility.Collapsed)

        BrandText.Visibility = v
        UserText.Visibility = v
        LogoutLabel.Visibility = v
        SwitchText.Visibility = v

        NavList.Tag = If(visible, "Expanded", "Collapsed")
    End Sub

    Private Sub LoadAdminView()
        Dim app = CType(System.Windows.Application.Current, Global.AutoRent.Application)
        Dim currentUser = app.CurrentUser

        ' Access control guard: Only Admin and Manager can access Admin side
        If currentUser IsNot Nothing AndAlso Not currentUser.CanAccessAdmin Then
            AlertUtil.ShowWarning("Access Denied: Rental Staff accounts are restricted to the Client Operations portal.", "Access Restricted", Me)
            LoadClientView()
            Return
        End If

        _currentViewIsAdmin = True
        Sidebar.Visibility = Visibility.Visible
        SidebarColumn.Width = New GridLength(CollapsedWidth)

        CurrentViewBadgeText.Text = "ADMIN CONSOLE"
        CurrentViewBadgeText.Foreground = New SolidColorBrush(Color.FromRgb(192, 57, 75))

        ' Only Manager can switch between Admin and Client views
        If currentUser IsNot Nothing AndAlso currentUser.CanSwitchSides Then
            SwitchViewButton.Visibility = Visibility.Visible
            SwitchText.Text = "Switch to Client"
        Else
            SwitchViewButton.Visibility = Visibility.Collapsed
        End If

        Dim navItems As New List(Of NavItem) From {
            New NavItem(ChrW(&HE80F), "Home Dashboard", "Home"),
            New NavItem(ChrW(&HE804), "Fleet Management", "Fleet"),
            New NavItem(ChrW(&HE90F), "Maintenance Schedules", "Maintenance"),
            New NavItem(ChrW(&HE9D5), "Vehicle Inspection", "VehicleInspection"),
            New NavItem(ChrW(&HE81D), "Dispatch Reports", "DispatchReports"),
            New NavItem(ChrW(&HE8EC), "Rate Management", "Rates"),
            New NavItem(ChrW(&HE9D9), "Financial Reports", "Reports"),
            New NavItem(ChrW(&HE716), "User Management", "Users")
        }

        ' User Request: "transfer the settings along side the menu bar in admin (system administartor only, no manager)"
        If currentUser IsNot Nothing AndAlso currentUser.IsAdmin Then
            navItems.Add(New NavItem(ChrW(&HE713), "Settings", "Settings"))
        End If

        NavList.ItemsSource = navItems
        NavList.SelectedIndex = 0
    End Sub

    Private Sub LoadClientView()
        Dim app = CType(System.Windows.Application.Current, Global.AutoRent.Application)
        Dim currentUser = app.CurrentUser

        ' Access control guard: Admin MUST ONLY access admin side
        If currentUser IsNot Nothing AndAlso currentUser.IsAdmin Then
            AlertUtil.ShowWarning("Access Denied: Administrators are restricted to the Admin Console.", "Access Restricted", Me)
            LoadAdminView()
            Return
        End If

        _currentViewIsAdmin = False
        Sidebar.Visibility = Visibility.Collapsed
        SidebarColumn.Width = New GridLength(0)

        If _clientView Is Nothing Then
            _clientView = New ClientView()
            AddHandler _clientView.SwitchToAdminRequested, AddressOf ClientView_SwitchToAdmin
            AddHandler _clientView.SignOutRequested, AddressOf ClientView_SignOut
        End If

        If currentUser IsNot Nothing Then
            _clientView.UserName = currentUser.DisplayName
            _clientView.UserRole = currentUser.DisplayRole
            ' Only Manager can switch to Admin side from Client view
            _clientView.CanSwitchToAdmin = currentUser.CanSwitchSides
        Else
            _clientView.CanSwitchToAdmin = False
        End If

        MainContentHost.Content = _clientView
    End Sub

    Private Sub ClientView_SwitchToAdmin(sender As Object, e As EventArgs)
        Dim app = CType(System.Windows.Application.Current, Global.AutoRent.Application)
        Dim currentUser = app.CurrentUser

        ' Access control guard: Only Manager can switch to Admin side
        If currentUser IsNot Nothing AndAlso Not currentUser.CanSwitchSides Then
            AlertUtil.ShowWarning("Access Denied: Only Managers can access both Admin Console and Client portal.", "Access Restricted", Me)
            Return
        End If

        LoadAdminView()
    End Sub

    Private Sub ClientView_SignOut(sender As Object, e As EventArgs)
        LogoutButton_Click(sender, New RoutedEventArgs())
    End Sub

    Private Sub NavList_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles NavList.SelectionChanged
        Dim item = TryCast(NavList.SelectedItem, NavItem)
        If item Is Nothing Then Return
        If Not _currentViewIsAdmin Then Return

        Select Case item.Key
            Case "Home"                                : MainContentHost.Content = New HomeDashboardView()
            Case "Fleet"                               : MainContentHost.Content = New FleetManagementView()
            Case "Maintenance"                         : MainContentHost.Content = New MaintenanceSchedulesView()
            Case "VehicleInspection", "ConditionChecklist" : MainContentHost.Content = New Presentation.Views.Admin.VehicleConditionChecklistView()
            Case "DispatchReports"                     : MainContentHost.Content = New DispatchReportsView()
            Case "Rates"                               : MainContentHost.Content = New RateManagementView()
            Case "Reports"                             : MainContentHost.Content = New FinancialReportsView()
            Case "Users"                               : MainContentHost.Content = New UserManagementView()
            Case "Settings"
                Dim app = CType(System.Windows.Application.Current, Global.AutoRent.Application)
                If app.CurrentUser IsNot Nothing AndAlso app.CurrentUser.IsAdmin Then
                    MainContentHost.Content = New SettingsView()
                Else
                    AlertUtil.ShowWarning("Access Denied: Settings and centralized data exports are restricted to System Administrators only.", "Access Restricted", Me)
                End If
            Case Else
                MainContentHost.Content = New AdminView()
        End Select
    End Sub

    Private Sub SwitchViewButton_Click(sender As Object, e As RoutedEventArgs) Handles SwitchViewButton.Click
        Dim app = CType(System.Windows.Application.Current, Global.AutoRent.Application)
        Dim currentUser = app.CurrentUser

        ' Only Manager can switch views
        If currentUser IsNot Nothing AndAlso Not currentUser.CanSwitchSides Then
            Return
        End If

        If _currentViewIsAdmin Then
            LoadClientView()
        Else
            LoadAdminView()
        End If
    End Sub

    Private _isSigningOut As Boolean = False
    Private _isPromptingExit As Boolean = False

    Private Sub LogoutButton_Click(sender As Object, e As RoutedEventArgs) Handles LogoutButton.Click
        If AlertUtil.Confirm("Are you sure you want to log out of your session?", "Sign Out", Me) Then
            _isSigningOut = True
            Dim app = CType(System.Windows.Application.Current, Global.AutoRent.Application)
            If app IsNot Nothing AndAlso app.CurrentUser IsNot Nothing Then
                UserRepository.RecordLogout(app.CurrentUser.UserId)
                app.CurrentUser = Nothing
            End If

            Dim loginWindow As New Global.AutoRent.AutoRent.LoginWindow()
            loginWindow.Show()
            Me.Close()
        End If
    End Sub

    Private Sub MainWindow_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.Closing
        If _isSigningOut Then Return
        If _isPromptingExit Then Return

        _isPromptingExit = True
        Try
            Dim app = CType(System.Windows.Application.Current, Global.AutoRent.Application)
            Dim currentUser = If(app IsNot Nothing, app.CurrentUser, Nothing)
            Dim userName = If(currentUser IsNot Nothing, currentUser.DisplayName, "your session")

            If Not AlertUtil.Confirm($"Are you sure you want to exit AutoRent? Your session ({userName}) will be signed out and set to inactive.", "Exit Application", Me) Then
                e.Cancel = True
                Return
            End If

            If currentUser IsNot Nothing Then
                UserRepository.RecordLogout(currentUser.UserId)
                If app IsNot Nothing Then app.CurrentUser = Nothing
            End If
        Finally
            _isPromptingExit = False
        End Try
    End Sub

    Private Sub MainWindow_StateChanged(sender As Object, e As EventArgs) Handles Me.StateChanged
        If Me.WindowState <> WindowState.Maximized Then
            Me.WindowState = WindowState.Maximized
        End If
    End Sub

    Private Sub MainWindow_PreviewKeyDown(sender As Object, e As KeyEventArgs) Handles Me.PreviewKeyDown
        ' Intercept Alt+F4 to prompt confirmation and ensure active status is set to inactive before exit
        If Keyboard.Modifiers.HasFlag(ModifierKeys.Alt) AndAlso e.SystemKey = Key.F4 Then
            e.Handled = True
            Me.Close()
            Return
        End If

        If e.Key = Key.Escape OrElse e.Key = Key.F11 OrElse (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt) AndAlso e.SystemKey = Key.Enter) Then
            e.Handled = True
        End If
    End Sub
End Class
