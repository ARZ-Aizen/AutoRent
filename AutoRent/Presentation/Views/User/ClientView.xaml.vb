Imports System.Collections.ObjectModel
Imports System.Linq
Imports System.Windows
Imports System.Windows.Controls
Imports System.Data
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports AutoRent.Data.Repositories

Namespace Presentation.Views.User
    Partial Public Class ClientView
        Inherits UserControl

        Public Event SwitchToAdminRequested As EventHandler
        Public Event SettingsRequested As EventHandler
        Public Event SignOutRequested As EventHandler

        Public Class FleetItem
            Public Property DbId As Integer
            Public Property Plate As String
            Public Property Model As String
            Public Property Category As String
            Public Property DailyRate As Decimal
            Public Property Status As String

            Public ReadOnly Property Display As String
                Get
                    Return Plate & " - " & Model
                End Get
            End Property

            Public Sub New(plate As String, model As String, category As String, dailyRate As Decimal, status As String)
                Me.Plate = plate
                Me.Model = model
                Me.Category = category
                Me.DailyRate = dailyRate
                Me.Status = status
            End Sub
        End Class

        Public Class CustomerItem
            Public Property DbId As Integer
            Public Property Name As String
            Public Property Phone As String
            Public Property Email As String
            Public Property License As String

            Public Sub New(name As String, phone As String, email As String, license As String)
                Me.Name = name
                Me.Phone = phone
                Me.Email = email
                Me.License = license
            End Sub
        End Class

        Public Class ReservationItem
            Public Property DbId As Integer
            Public Property VehicleId As Integer
            Public Property Id As String
            Public Property Customer As String
            Public Property Vehicle As String
            Public Property Plate As String
            Public Property PickupDate As Date
            Public Property ReturnDate As Date
            Public Property Total As Decimal
            Public Property Status As String

            Public Sub New(id As String, customer As String, vehicle As String, plate As String,
                           pickupDate As Date, returnDate As Date, total As Decimal, status As String)
                Me.Id = id
                Me.Customer = customer
                Me.Vehicle = vehicle
                Me.Plate = plate
                Me.PickupDate = pickupDate
                Me.ReturnDate = returnDate
                Me.Total = total
                Me.Status = status
            End Sub
        End Class

        Public Class DispatchItem
            Public Property DbId As Integer
            Public Property ReservationId As Integer
            Public Property VehicleId As Integer
            Public Property Time As String
            Public Property DispatchType As String
            Public Property Customer As String
            Public Property Vehicle As String
            Public Property Plate As String
            Public Property Location As String
            Public Property Status As String

            Public Sub New(time As String, dispatchType As String, customer As String, vehicle As String,
                           plate As String, location As String, status As String)
                Me.Time = time
                Me.DispatchType = dispatchType
                Me.Customer = customer
                Me.Vehicle = vehicle
                Me.Plate = plate
                Me.Location = location
                Me.Status = status
            End Sub
        End Class

        Public Class InspectionItem
            Public Property InspectedOn As Date
            Public Property Vehicle As String
            Public Property Inspector As String
            Public Property Fuel As String
            Public Property Findings As String
            Public Property Result As String

            Public Sub New(inspectedOn As Date, vehicle As String, inspector As String,
                           fuel As String, findings As String, result As String)
                Me.InspectedOn = inspectedOn
                Me.Vehicle = vehicle
                Me.Inspector = inspector
                Me.Fuel = fuel
                Me.Findings = findings
                Me.Result = result
            End Sub
        End Class

        Private ReadOnly _fleet As New ObservableCollection(Of FleetItem)
        Private ReadOnly _customers As New ObservableCollection(Of CustomerItem)
        Private ReadOnly _reservations As New ObservableCollection(Of ReservationItem)
        Private ReadOnly _dispatch As New ObservableCollection(Of DispatchItem)
        Private ReadOnly _inspections As New ObservableCollection(Of InspectionItem)

        Private _currentActiveButton As Button = Nothing
        Private _initialized As Boolean = False

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

            DgFleet.ItemsSource = _fleet
            DgCustomers.ItemsSource = _customers
            DgReservations.ItemsSource = _reservations
            DgDispatch.ItemsSource = _dispatch
            DgInspections.ItemsSource = _inspections

            CmbResCustomer.ItemsSource = _customers
            CmbInspVehicle.ItemsSource = _fleet

            DpPickup.SelectedDate = Date.Today
            DpReturn.SelectedDate = Date.Today.AddDays(1)

            _initialized = True
        End Sub

        Private Sub ClientView_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            If _currentActiveButton Is Nothing Then
                NavigateTo("Home (Dashboard)", BtnHome)
            End If
        End Sub

        Private Shared Function Txt(row As DataRow, column As String) As String
            If IsDBNull(row(column)) Then Return String.Empty
            Return Convert.ToString(row(column))
        End Function

        Private Shared Function Num(row As DataRow, column As String) As Integer
            If IsDBNull(row(column)) Then Return 0
            Return Convert.ToInt32(row(column))
        End Function

        Private Shared Function Money(row As DataRow, column As String) As Decimal
            If IsDBNull(row(column)) Then Return 0D
            Return Convert.ToDecimal(row(column))
        End Function

        Private Shared Function Stamp(row As DataRow, column As String) As Date
            If IsDBNull(row(column)) Then Return Date.MinValue
            Return Convert.ToDateTime(row(column))
        End Function

        Private Shared Function VehicleName(row As DataRow) As String
            Dim parts As New List(Of String)
            For Each col In {"make", "model", "model_year"}
                Dim part = Txt(row, col).Trim()
                If part <> "" Then parts.Add(part)
            Next
            Return String.Join(" ", parts)
        End Function

        Private Shared Function VehicleLabel(row As DataRow) As String
            Dim plate = Txt(row, "plate")
            If plate = "" Then Return "(vehicle removed)"
            Return plate & " - " & VehicleName(row)
        End Function

        Private Sub ReloadData()
            Try
                Dim fleet = ClientRepository.GetFleet()
                Dim customers = ClientRepository.GetCustomers()
                Dim reservations = ClientRepository.GetReservations()
                Dim dispatch = ClientRepository.GetDispatchBoard()
                Dim inspections = ClientRepository.GetInspections()

                _fleet.Clear()
                For Each r As DataRow In fleet.Rows
                    _fleet.Add(New FleetItem(Txt(r, "plate"), VehicleName(r), Txt(r, "category"), Money(r, "daily_rate"),
                                             If(Txt(r, "status") = "", "Available", Txt(r, "status"))) With {.DbId = Num(r, "vehicle_id")})
                Next

                _customers.Clear()
                For Each r As DataRow In customers.Rows
                    _customers.Add(New CustomerItem(Txt(r, "full_name"), Txt(r, "phone"), Txt(r, "email"), Txt(r, "license_no")) With {.DbId = Num(r, "customer_id")})
                Next

                _reservations.Clear()
                For Each r As DataRow In reservations.Rows
                    Dim id = Num(r, "reservation_id")
                    _reservations.Add(New ReservationItem("R-" & id.ToString("0000"), Txt(r, "customer_name"), VehicleLabel(r), Txt(r, "plate"),
                                                          Stamp(r, "pickup_date"), Stamp(r, "return_date"), Money(r, "total"), Txt(r, "status")) With {.DbId = id, .VehicleId = Num(r, "vehicle_id")})
                Next

                _dispatch.Clear()
                For Each r As DataRow In dispatch.Rows
                    Dim at = Stamp(r, "scheduled_at")
                    Dim timeText = If(at.Date = Date.Today, at.ToString("hh:mm tt"), at.ToString("MMM d, hh:mm tt"))
                    _dispatch.Add(New DispatchItem(timeText, Txt(r, "dispatch_type"), Txt(r, "customer_name"), VehicleName(r), Txt(r, "plate"),
                                                   Txt(r, "branch"), Txt(r, "status")) With {.DbId = Num(r, "dispatch_id"), .ReservationId = Num(r, "reservation_id"), .VehicleId = Num(r, "vehicle_id")})
                Next

                _inspections.Clear()
                For Each r As DataRow In inspections.Rows
                    _inspections.Add(New InspectionItem(Stamp(r, "inspected_at"), VehicleLabel(r), Txt(r, "inspector"), Txt(r, "fuel"),
                                                        Txt(r, "findings"), Txt(r, "inspection_result")))
                Next
            Catch ex As Exception
                MessageBox.Show("Could not load data from the database: " & ex.Message, "AutoRent",
                                MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
        End Sub

        Private Sub ShowDbError(action As String, ex As Exception)
            MessageBox.Show($"Could not {action}: {ex.Message}", "AutoRent", MessageBoxButton.OK, MessageBoxImage.Error)
        End Sub

        Private Sub SidebarOption_Click(sender As Object, e As RoutedEventArgs)
            Dim btn = TryCast(sender, Button)
            If btn Is Nothing Then Return
            NavigateTo(btn.Tag?.ToString(), btn)
        End Sub

        Private Sub NavigateTo(optionName As String, clickedButton As Button)
            SetActiveButton(clickedButton)
            HideAllPages()
            ReloadData()

            Select Case optionName
                Case "Home (Dashboard)"
                    RefreshDashboard()
                    PageHome.Visibility = Visibility.Visible

                Case "Fleet Availability"
                    ApplyFleetFilter()
                    PageFleet.Visibility = Visibility.Visible

                Case "Customer Management"
                    ApplyCustomerFilter()
                    PageCustomers.Visibility = Visibility.Visible

                Case "Reservations"
                    RefreshReservationVehicles()
                    DgReservations.Items.Refresh()
                    PageReservations.Visibility = Visibility.Visible

                Case "Daily Dispatch"
                    ApplyDispatchFilter()
                    PageDispatch.Visibility = Visibility.Visible

                Case "Vehicle Inspection"
                    PageInspection.Visibility = Visibility.Visible
            End Select
        End Sub

        Private Sub HideAllPages()
            For Each page In {PageHome, PageFleet, PageCustomers, PageReservations, PageDispatch, PageInspection}
                page.Visibility = Visibility.Collapsed
            Next
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

        Private Sub RefreshDashboard()
            TxtHomeDate.Text = Date.Today.ToString("dddd, MMMM d, yyyy")
            TxtStatTotal.Text = _fleet.Count.ToString()
            TxtStatAvailable.Text = _fleet.Where(Function(f) f.Status = "Available").Count().ToString()
            TxtStatReservations.Text = _reservations.Where(Function(r) r.Status = "Confirmed" OrElse r.Status = "Active").Count().ToString()
            TxtStatDispatch.Text = _dispatch.Where(Function(d) d.Status = "Scheduled").Count().ToString()
            DgRecent.ItemsSource = _reservations.Take(5).ToList()
        End Sub

        Private Sub TxtFleetSearch_TextChanged(sender As Object, e As TextChangedEventArgs)
            If Not _initialized Then Return
            ApplyFleetFilter()
        End Sub

        Private Sub CmbFleetStatus_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            If Not _initialized Then Return
            ApplyFleetFilter()
        End Sub

        Private Sub ApplyFleetFilter()
            Dim q As String = TxtFleetSearch.Text.Trim().ToLowerInvariant()
            Dim status As String = If(TryCast(CmbFleetStatus.SelectedItem, ComboBoxItem)?.Content?.ToString(), "All")

            Dim result = _fleet.Where(Function(f) (status = "All" OrElse f.Status = status) AndAlso
                                          (q = "" OrElse
                                           f.Plate.ToLowerInvariant().Contains(q) OrElse
                                           f.Model.ToLowerInvariant().Contains(q) OrElse
                                           f.Category.ToLowerInvariant().Contains(q))).ToList()

            DgFleet.ItemsSource = result
            TxtFleetCount.Text = $"{result.Count} of {_fleet.Count} vehicles"
        End Sub

        Private Sub TxtCustSearch_TextChanged(sender As Object, e As TextChangedEventArgs)
            If Not _initialized Then Return
            ApplyCustomerFilter()
        End Sub

        Private Sub ApplyCustomerFilter()
            Dim q As String = TxtCustSearch.Text.Trim().ToLowerInvariant()

            Dim result = _customers.Where(Function(c) q = "" OrElse
                                              c.Name.ToLowerInvariant().Contains(q) OrElse
                                              c.Phone.ToLowerInvariant().Contains(q) OrElse
                                              c.Email.ToLowerInvariant().Contains(q) OrElse
                                              c.License.ToLowerInvariant().Contains(q)).ToList()

            DgCustomers.ItemsSource = result
        End Sub

        Private Sub BtnAddCustomer_Click(sender As Object, e As RoutedEventArgs)
            Dim name As String = TxtCustName.Text.Trim()
            Dim phone As String = TxtCustPhone.Text.Trim()

            If name = "" OrElse phone = "" Then
                MessageBox.Show("Full name and phone are required.", "Add Customer",
                                MessageBoxButton.OK, MessageBoxImage.Warning)
                Return
            End If

            Try
                Dim problem = ClientRepository.AddCustomer(name, phone, TxtCustEmail.Text.Trim(), TxtCustLicense.Text.Trim())
                If problem IsNot Nothing Then
                    MessageBox.Show(problem, "Add Customer", MessageBoxButton.OK, MessageBoxImage.Warning)
                    Return
                End If
            Catch ex As Exception
                ShowDbError("save the customer", ex)
                Return
            End Try
            ReloadData()

            TxtCustName.Clear()
            TxtCustPhone.Clear()
            TxtCustEmail.Clear()
            TxtCustLicense.Clear()

            ApplyCustomerFilter()
        End Sub

        Private Sub BtnDeleteCustomer_Click(sender As Object, e As RoutedEventArgs)
            Dim selected = TryCast(DgCustomers.SelectedItem, CustomerItem)
            If selected Is Nothing Then
                MessageBox.Show("Select a customer first.", "Delete Customer",
                                MessageBoxButton.OK, MessageBoxImage.Information)
                Return
            End If

            If MessageBox.Show($"Delete {selected.Name}?", "Delete Customer",
                               MessageBoxButton.YesNo, MessageBoxImage.Question) <> MessageBoxResult.Yes Then Return

            Try
                Dim problem = ClientRepository.DeleteCustomer(selected.DbId, selected.Name)
                If problem IsNot Nothing Then
                    MessageBox.Show(problem, "Delete Customer", MessageBoxButton.OK, MessageBoxImage.Warning)
                    Return
                End If
            Catch ex As Exception
                ShowDbError("delete the customer", ex)
                Return
            End Try
            ReloadData()
            ApplyCustomerFilter()
        End Sub

        Private Sub RefreshReservationVehicles()
            CmbResVehicle.ItemsSource = _fleet.Where(Function(f) f.Status = "Available").ToList()
        End Sub

        Private Sub BtnCreateReservation_Click(sender As Object, e As RoutedEventArgs)
            Dim cust = TryCast(CmbResCustomer.SelectedItem, CustomerItem)
            Dim veh = TryCast(CmbResVehicle.SelectedItem, FleetItem)

            If cust Is Nothing OrElse veh Is Nothing OrElse
               Not DpPickup.SelectedDate.HasValue OrElse Not DpReturn.SelectedDate.HasValue Then
                MessageBox.Show("Please select a customer, an available vehicle, and both dates.",
                                "New Reservation", MessageBoxButton.OK, MessageBoxImage.Warning)
                Return
            End If

            Dim pickup As Date = DpPickup.SelectedDate.Value
            Dim dropoff As Date = DpReturn.SelectedDate.Value

            If dropoff <= pickup Then
                MessageBox.Show("The return date must be after the pickup date.",
                                "New Reservation", MessageBoxButton.OK, MessageBoxImage.Warning)
                Return
            End If

            Dim days As Integer = (dropoff - pickup).Days
            Dim total As Decimal = days * veh.DailyRate

            Try
                Dim problem = ClientRepository.CreateReservation(cust.Name, veh.DbId, pickup, dropoff, total)
                If problem IsNot Nothing Then
                    MessageBox.Show(problem, "New Reservation", MessageBoxButton.OK, MessageBoxImage.Warning)
                    ReloadData()
                    RefreshReservationVehicles()
                    Return
                End If
            Catch ex As Exception
                ShowDbError("create the reservation", ex)
                Return
            End Try
            ReloadData()

            CmbResCustomer.SelectedIndex = -1
            CmbResVehicle.SelectedIndex = -1
            RefreshReservationVehicles()
            DgReservations.Items.Refresh()
        End Sub

        Private Sub BtnCancelReservation_Click(sender As Object, e As RoutedEventArgs)
            Dim res = TryCast(DgReservations.SelectedItem, ReservationItem)
            If Not CanChangeReservation(res) Then Return

            If Not CloseReservation(res, "Cancelled") Then Return
            RefreshReservationVehicles()
            DgReservations.Items.Refresh()
        End Sub

        Private Sub BtnCompleteReservation_Click(sender As Object, e As RoutedEventArgs)
            Dim res = TryCast(DgReservations.SelectedItem, ReservationItem)
            If Not CanChangeReservation(res) Then Return

            If Not CloseReservation(res, "Completed") Then Return
            RefreshReservationVehicles()
            DgReservations.Items.Refresh()
        End Sub

        Private Function CanChangeReservation(res As ReservationItem) As Boolean
            If res Is Nothing Then
                MessageBox.Show("Select a reservation first.", "Reservations",
                                MessageBoxButton.OK, MessageBoxImage.Information)
                Return False
            End If

            If res.Status = "Cancelled" OrElse res.Status = "Completed" Then
                MessageBox.Show($"Reservation {res.Id} is already {res.Status.ToLower()}.", "Reservations",
                                MessageBoxButton.OK, MessageBoxImage.Information)
                Return False
            End If

            Return True
        End Function

        Private Function CloseReservation(res As ReservationItem, newStatus As String) As Boolean
            Try
                ClientRepository.CloseReservation(res.DbId, res.VehicleId, newStatus)
            Catch ex As Exception
                ShowDbError("update the reservation", ex)
                Return False
            End Try
            ReloadData()
            Return True
        End Function

        Private Sub CmbDispatchType_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            If Not _initialized Then Return
            ApplyDispatchFilter()
        End Sub

        Private Sub ApplyDispatchFilter()
            Dim kind As String = If(TryCast(CmbDispatchType.SelectedItem, ComboBoxItem)?.Content?.ToString(), "All")

            DgDispatch.ItemsSource = _dispatch.Where(Function(d) kind = "All" OrElse d.DispatchType = kind).ToList()

            Dim pickups = _dispatch.Where(Function(d) d.DispatchType = "Pickup").Count()
            Dim returns = _dispatch.Where(Function(d) d.DispatchType = "Return").Count()
            Dim pending = _dispatch.Where(Function(d) d.Status = "Scheduled").Count()

            TxtDispatchSummary.Text = $"Today, {Date.Today:MMMM d}  •  {pickups} pickups  •  {returns} returns  •  {pending} pending"
        End Sub

        Private Sub BtnMarkDispatchDone_Click(sender As Object, e As RoutedEventArgs)
            Dim item = TryCast(DgDispatch.SelectedItem, DispatchItem)
            If item Is Nothing Then
                MessageBox.Show("Select a dispatch first.", "Daily Dispatch",
                                MessageBoxButton.OK, MessageBoxImage.Information)
                Return
            End If

            If item.Status = "Completed" Then Return

            Try
                ClientRepository.CompleteDispatch(item.DbId, item.ReservationId, item.VehicleId, item.DispatchType)
            Catch ex As Exception
                ShowDbError("update the dispatch", ex)
                Return
            End Try
            ReloadData()
            ApplyDispatchFilter()
        End Sub

        Private Sub BtnSaveInspection_Click(sender As Object, e As RoutedEventArgs)
            Dim veh = TryCast(CmbInspVehicle.SelectedItem, FleetItem)
            Dim inspector As String = TxtInspector.Text.Trim()

            If veh Is Nothing OrElse inspector = "" Then
                MessageBox.Show("Please select a vehicle and enter the inspector's name.",
                                "Vehicle Inspection", MessageBoxButton.OK, MessageBoxImage.Warning)
                Return
            End If

            Dim checks As CheckBox() = {ChkExterior, ChkTires, ChkLights, ChkInterior, ChkBrakes, ChkFluids}
            Dim failed = checks.Where(Function(c) Not c.IsChecked.GetValueOrDefault()) _
                               .Select(Function(c) c.Content.ToString()).ToList()

            Dim passed As Boolean = (failed.Count = 0)
            Dim fuel As String = If(TryCast(CmbFuel.SelectedItem, ComboBoxItem)?.Content?.ToString(), "")

            Dim findings As String = If(passed, "All checks passed", "Failed: " & String.Join(", ", failed))
            Dim notes As String = TxtInspNotes.Text.Trim()
            If notes <> "" Then findings &= " | " & notes

            Try
                ClientRepository.AddInspection(veh.DbId, inspector, fuel, findings, passed)
            Catch ex As Exception
                ShowDbError("save the inspection", ex)
                Return
            End Try
            ReloadData()

            If passed Then
                MessageBox.Show("Inspection saved: Pass.", "Vehicle Inspection",
                                MessageBoxButton.OK, MessageBoxImage.Information)
            Else
                MessageBox.Show("Inspection saved: Needs Attention." & Environment.NewLine &
                                "The vehicle was marked as Maintenance.", "Vehicle Inspection",
                                MessageBoxButton.OK, MessageBoxImage.Warning)
            End If

            For Each c In checks
                c.IsChecked = False
            Next
            TxtInspNotes.Clear()
            CmbInspVehicle.SelectedIndex = -1
            RefreshReservationVehicles()
        End Sub

    End Class

    Public Class UserView
        Inherits ClientView
    End Class
End Namespace
