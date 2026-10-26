Imports System.Linq
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Media.Animation

Namespace Presentation.Views.User
    Partial Public Class ClientView
        Inherits UserControl

        Public Event SwitchToAdminRequested As EventHandler
        Public Event SettingsRequested As EventHandler
        Public Event SignOutRequested As EventHandler

        Private _currentActiveButton As Button = Nothing

        Public Property UserName As String
            Get
                Return TxtUserName.Text
            End Get
            Set(value As String)
                TxtUserName.Text = value
            End Set
        End Property

        Public Property UserRole As String
            Get
                Return TxtUserRole.Text
            End Get
            Set(value As String)
                TxtUserRole.Text = value.ToUpper()
            End Set
        End Property

        Public Property CanSwitchToAdmin As Boolean
            Get
                Return BtnSwitchAdmin.Visibility = Visibility.Visible
            End Get
            Set(value As Boolean)
                BtnSwitchAdmin.Visibility = If(value, Visibility.Visible, Visibility.Collapsed)
            End Set
        End Property

        Public Sub New()
            InitializeComponent()
            SetLabelsVisible(False)
        End Sub

        Private Sub ClientView_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            If _currentActiveButton Is Nothing Then
                NavigateTo("Home (Dashboard)", BtnHome)
            End If
        End Sub

        Private Sub SidebarOption_Click(sender As Object, e As RoutedEventArgs)
            Dim btn = TryCast(sender, Button)
            If btn Is Nothing Then Return
            NavigateTo(btn.Tag?.ToString(), btn)
        End Sub

        ' A fresh view is created on every click, so its data is reloaded each time.
        Private Sub NavigateTo(optionName As String, clickedButton As Button)
            SetActiveButton(clickedButton)

            Select Case optionName
                Case "Home (Dashboard)"
                    ContentHost.Content = New ClientHomeView()
                Case "Fleet Availability"
                    ContentHost.Content = New FleetAvailabilityView()
                Case "Customer Management"
                    ContentHost.Content = New CustomerManagementView()
                Case "Reservations"
                    ContentHost.Content = New ReservationsView()
                Case "Daily Dispatch"
                    ContentHost.Content = New DailyDispatchView()
                Case "Vehicle Inspection"
                    ContentHost.Content = New VehicleInspectionView()
            End Select
        End Sub

        Private Sub SetActiveButton(activeBtn As Button)
            If activeBtn Is Nothing Then Return

            If _currentActiveButton IsNot Nothing AndAlso _currentActiveButton IsNot activeBtn Then
                StyleNavButton(_currentActiveButton, False)
            End If

            _currentActiveButton = activeBtn
            StyleNavButton(_currentActiveButton, True)
        End Sub

        Private Shared Sub StyleNavButton(btn As Button, active As Boolean)
            If active Then
                btn.Background = New SolidColorBrush(Color.FromRgb(&H36, &H33, &H31))
                btn.Foreground = Brushes.White
            Else
                btn.ClearValue(BackgroundProperty)
                btn.ClearValue(ForegroundProperty)
            End If

            btn.ApplyTemplate()
            Dim indicator = TryCast(btn.Template.FindName("Indicator", btn), Border)
            If indicator IsNot Nothing Then indicator.Visibility = If(active, Visibility.Visible, Visibility.Collapsed)

            Dim panel = TryCast(btn.Content, StackPanel)
            Dim icon = TryCast(panel?.Children(0), TextBlock)
            If icon IsNot Nothing Then
                If active Then
                    icon.Foreground = New SolidColorBrush(Color.FromRgb(&HC0, &H39, &H4B))
                Else
                    icon.ClearValue(TextBlock.ForegroundProperty)
                End If
            End If
        End Sub

        Private Const ExpandedWidth As Double = 264
        Private Const CollapsedWidth As Double = 72
        Private _isExpanded As Boolean = False
        Private _hoverExpanded As Boolean = False

        Private Sub ClientSidebar_MouseEnter(sender As Object, e As System.Windows.Input.MouseEventArgs)
            If Not _isExpanded Then
                _hoverExpanded = True
                SetSidebarExpanded(True)
            End If
        End Sub

        Private Sub ClientSidebar_MouseLeave(sender As Object, e As System.Windows.Input.MouseEventArgs)
            If _hoverExpanded Then
                _hoverExpanded = False
                SetSidebarExpanded(False)
            End If
        End Sub

        Private Sub LogoToggle_Click(sender As Object, e As RoutedEventArgs)
            If _hoverExpanded Then
                _hoverExpanded = False
            Else
                SetSidebarExpanded(Not _isExpanded)
            End If
        End Sub

        Private Sub SetSidebarExpanded(expand As Boolean)
            _isExpanded = expand

            If Not _isExpanded Then SetLabelsVisible(False)

            Dim anim As New DoubleAnimation With {
                .To = If(_isExpanded, ExpandedWidth, CollapsedWidth),
                .Duration = TimeSpan.FromMilliseconds(200),
                .EasingFunction = New CubicEase With {.EasingMode = EasingMode.EaseInOut}
            }
            If _isExpanded Then
                AddHandler anim.Completed, Sub(s, args) SetLabelsVisible(True)
            End If
            ClientSidebar.BeginAnimation(FrameworkElement.WidthProperty, anim)
        End Sub

        Private Sub SetLabelsVisible(visible As Boolean)
            Dim v = If(visible, Visibility.Visible, Visibility.Collapsed)
            BrandText.Visibility = v
            UserText.Visibility = v

            For Each btn In {BtnHome, BtnFleetAvailability, BtnCustomerManagement, BtnReservations, BtnDailyDispatch,
                             BtnVehicleInspection, BtnSwitchAdmin, BtnSettings, BtnSignOut}
                Dim panel = TryCast(btn.Content, StackPanel)
                If panel Is Nothing Then Continue For
                For Each label In panel.Children.OfType(Of TextBlock)().Where(Function(t) Equals(t.Tag, "Label"))
                    label.Visibility = v
                Next
            Next

            For Each btn In {BtnHome, BtnFleetAvailability, BtnCustomerManagement, BtnReservations, BtnDailyDispatch, BtnVehicleInspection}
                btn.ToolTip = If(visible, Nothing, btn.Tag)
            Next
        End Sub

        Private Sub BtnSwitchAdmin_Click(sender As Object, e As RoutedEventArgs)
            RaiseEvent SwitchToAdminRequested(Me, EventArgs.Empty)
        End Sub

        Private Sub BtnSettings_Click(sender As Object, e As RoutedEventArgs)
            RaiseEvent SettingsRequested(Me, EventArgs.Empty)
        End Sub

        Private Sub BtnSignOut_Click(sender As Object, e As RoutedEventArgs)
            RaiseEvent SignOutRequested(Me, EventArgs.Empty)
        End Sub

    End Class

    Public Class UserView
        Inherits ClientView
    End Class
End Namespace