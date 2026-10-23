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

        ' Tells the nav item template to show/hide titles (and enable tooltips)
        NavList.Tag = If(visible, "Expanded", "Collapsed")
    End Sub

    ' ---------- Menus ----------

    Private Sub LoadAdminView()
        _currentViewIsAdmin = True
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

    Private Sub LoadClientView()
        _currentViewIsAdmin = False
        CurrentViewBadgeText.Text = "CLIENT PORTAL"
        CurrentViewBadgeText.Foreground = New SolidColorBrush(Color.FromRgb(111, 168, 232))
        SwitchText.Text = "Switch to Admin Console"

        NavList.ItemsSource = New List(Of NavItem) From {
            New NavItem(ChrW(&HE80F), "Browse Cars", "ClientBrowse"),
            New NavItem(ChrW(&HE787), "My Reservations", "ClientReservations"),
            New NavItem(ChrW(&HE8C7), "Billing", "ClientBilling"),
            New NavItem(ChrW(&HE81C), "Rental History", "ClientHistory")
        }
        NavList.SelectedIndex = 0
    End Sub

    ' ---------- Navigation ----------

    Private Sub NavList_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles NavList.SelectionChanged
        Dim item = TryCast(NavList.SelectedItem, NavItem)
        If item Is Nothing Then Return

        If _currentViewIsAdmin Then
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
        Else
            Select Case item.Key
                ' Case "ClientBrowse"       : MainContentHost.Content = New ClientBrowseView()
                ' Case "ClientReservations" : MainContentHost.Content = New ClientReservationsView()
                ' Case "ClientBilling"      : MainContentHost.Content = New ClientBillingView()
                ' Case "ClientHistory"      : MainContentHost.Content = New ClientHistoryView()
                Case Else
                    MainContentHost.Content = New ClientView()  ' temporary fallback
            End Select
        End If
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