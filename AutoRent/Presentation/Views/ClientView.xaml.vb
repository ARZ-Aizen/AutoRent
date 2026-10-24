Imports System.Collections.ObjectModel
Imports System.Linq
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media

Namespace Presentation.Views
    Partial Public Class ClientView
        Inherits UserControl

        ' ===================== EVENTS FOR THE HOST WINDOW =====================
        Public Event SwitchToAdminRequested As EventHandler
        Public Event SettingsRequested As EventHandler
        Public Event SignOutRequested As EventHandler

        ' ===================== SAMPLE DATA MODELS =====================
        Public Class FleetItem
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

        ' ===================== DATA STORES =====================
        Private ReadOnly _fleet As New ObservableCollection(Of FleetItem)
        Private ReadOnly _customers As New ObservableCollection(Of CustomerItem)
        Private ReadOnly _reservations As New ObservableCollection(Of ReservationItem)
        Private ReadOnly _dispatch As New ObservableCollection(Of DispatchItem)
        Private ReadOnly _inspections As New ObservableCollection(Of InspectionItem)

        Private _nextReservationNo As Integer = 1006
        Private _currentActiveButton As Button = Nothing
        Private _initialized As Boolean = False

        ' ===================== USER INFO (set from the host window) =====================
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

        ' Lets the host hide "Switch to Admin Console" for non-admin users
        Public Property CanSwitchToAdmin As Boolean
            Get
                Return BtnSwitchAdmin.Visibility = Visibility.Visible
            End Get
            Set(value As Boolean)
                BtnSwitchAdmin.Visibility = If(value, Visibility.Visible, Visibility.Collapsed)
            End Set
        End Property

        ' ===================== STARTUP =====================
        Public Sub New()
            InitializeComponent()

            SeedSampleData()

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

        Private Sub SeedSampleData()
            Dim today As Date = Date.Today

            _fleet.Add(New FleetItem("NBC 4821", "Toyota Vios 2023", "Sedan", 1800D, "Rented"))
            _fleet.Add(New FleetItem("ABX 1932", "Honda City 2022", "Sedan", 1900D, "Rented"))
            _fleet.Add(New FleetItem("TRK 7710", "Toyota Innova 2023", "MPV", 2800D, "Available"))
            _fleet.Add(New FleetItem("CDE 5521", "Mitsubishi Xpander 2022", "MPV", 2600D, "Reserved"))
            _fleet.Add(New FleetItem("SUV 3308", "Toyota Fortuner 2023", "SUV", 4200D, "Rented"))
            _fleet.Add(New FleetItem("PLM 9045", "Ford Ranger 2022", "Pickup", 3800D, "Available"))
            _fleet.Add(New FleetItem("HYD 2276", "Hyundai Accent 2021", "Sedan", 1700D, "Maintenance"))
            _fleet.Add(New FleetItem("NSN 6683", "Nissan Navara 2022", "Pickup", 3600D, "Available"))
            _fleet.Add(New FleetItem("KIA 1459", "Kia Seltos 2023", "SUV", 3000D, "Reserved"))
            _fleet.Add(New FleetItem("HND 8812", "Honda BR-V 2022", "MPV", 2400D, "Available"))

            _customers.Add(New CustomerItem("Juan Dela Cruz", "0917 555 0101", "juan.delacruz@email.com", "N01-23-456789"))
            _customers.Add(New CustomerItem("Maria Santos", "0918 555 0102", "maria.santos@email.com", "N02-34-567890"))
            _customers.Add(New CustomerItem("Carlo Reyes", "0919 555 0103", "carlo.reyes@email.com", "N03-45-678901"))
            _customers.Add(New CustomerItem("Angela Lim", "0920 555 0104", "angela.lim@email.com", "N04-56-789012"))
            _customers.Add(New CustomerItem("Paolo Garcia", "0921 555 0105", "paolo.garcia@email.com", "N05-67-890123"))

            ' Newest first
            _reservations.Add(New ReservationItem("R-1005", "Juan Dela Cruz", "NBC 4821 - Toyota Vios 2023", "NBC 4821", today, today.AddDays(2), 3600D, "Active"))
            _reservations.Add(New ReservationItem("R-1004", "Paolo Garcia", "KIA 1459 - Kia Seltos 2023", "KIA 1459", today, today.AddDays(3), 9000D, "Confirmed"))
            _reservations.Add(New ReservationItem("R-1003", "Angela Lim", "CDE 5521 - Mitsubishi Xpander 2022", "CDE 5521", today, today.AddDays(2), 5200D, "Confirmed"))
            _reservations.Add(New ReservationItem("R-1002", "Carlo Reyes", "SUV 3308 - Toyota Fortuner 2023", "SUV 3308", today.AddDays(-3), today, 12600D, "Active"))
            _reservations.Add(New ReservationItem("R-1001", "Maria Santos", "ABX 1932 - Honda City 2022", "ABX 1932", today.AddDays(-2), today.AddDays(1), 5700D, "Active"))
            _reservations.Add(New ReservationItem("R-1000", "Angela Lim", "HND 8812 - Honda BR-V 2022", "HND 8812", today.AddDays(-10), today.AddDays(-8), 4800D, "Completed"))

            _dispatch.Add(New DispatchItem("07:45 AM", "Pickup", "Juan Dela Cruz", "Toyota Vios 2023", "NBC 4821", "Main Branch", "Completed"))
            _dispatch.Add(New DispatchItem("08:30 AM", "Pickup", "Angela Lim", "Mitsubishi Xpander 2022", "CDE 5521", "Main Branch", "Scheduled"))
            _dispatch.Add(New DispatchItem("10:00 AM", "Return", "Carlo Reyes", "Toyota Fortuner 2023", "SUV 3308", "Main Branch", "Scheduled"))
            _dispatch.Add(New DispatchItem("01:30 PM", "Pickup", "Paolo Garcia", "Kia Seltos 2023", "KIA 1459", "Airport Branch", "Scheduled"))

            _inspections.Add(New InspectionItem(today.AddDays(-1).AddHours(14), "HYD 2276 - Hyundai Accent 2021", "Ramon Dizon", "1/2", "Brakes", "Needs Attention"))
            _inspections.Add(New InspectionItem(today.AddDays(-2).AddHours(9), "HND 8812 - Honda BR-V 2022", "Ramon Dizon", "Full", "All checks passed", "Pass"))
        End Sub

        ' ===================== NAVIGATION =====================
        Private Sub SidebarOption_Click(sender As Object, e As RoutedEventArgs)
            Dim btn = TryCast(sender, Button)
            If btn Is Nothing Then Return
            NavigateTo(btn.Tag?.ToString(), btn)
        End Sub

        Private Sub NavigateTo(optionName As String, clickedButton As Button)
            SetActiveButton(clickedButton)
            HideAllPages()

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
                _currentActiveButton.ClearValue(BackgroundProperty)
            End If

            _currentActiveButton = activeBtn
            _currentActiveButton.Background = New SolidColorBrush(Color.FromRgb(58, 48, 50))
        End Sub

        ' ===================== FOOTER BUTTONS =====================
        Private Sub BtnSwitchAdmin_Click(sender As Object, e As RoutedEventArgs)
            RaiseEvent SwitchToAdminRequested(Me, EventArgs.Empty)
        End Sub

        Private Sub BtnSettings_Click(sender As Object, e As RoutedEventArgs)
            RaiseEvent SettingsRequested(Me, EventArgs.Empty)
        End Sub

        Private Sub BtnSignOut_Click(sender As Object, e As RoutedEventArgs)
            RaiseEvent SignOutRequested(Me, EventArgs.Empty)
        End Sub

        ' ===================== 1. HOME (DASHBOARD) =====================
        Private Sub RefreshDashboard()
            TxtHomeDate.Text = Date.Today.ToString("dddd, MMMM d, yyyy")
            TxtStatTotal.Text = _fleet.Count.ToString()
            TxtStatAvailable.Text = _fleet.Where(Function(f) f.Status = "Available").Count().ToString()
            TxtStatReservations.Text = _reservations.Where(Function(r) r.Status = "Confirmed" OrElse r.Status = "Active").Count().ToString()
            TxtStatDispatch.Text = _dispatch.Where(Function(d) d.Status = "Scheduled").Count().ToString()
            DgRecent.ItemsSource = _reservations.Take(5).ToList()
        End Sub

        ' ===================== 2. FLEET AVAILABILITY =====================
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

        ' ===================== 3. CUSTOMER MANAGEMENT =====================
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

            _customers.Add(New CustomerItem(name, phone, TxtCustEmail.Text.Trim(), TxtCustLicense.Text.Trim()))

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

            Dim hasOpen = _reservations.Any(Function(r) r.Customer = selected.Name AndAlso
                                                (r.Status = "Confirmed" OrElse r.Status = "Active"))
            If hasOpen Then
                MessageBox.Show($"{selected.Name} still has an open reservation and cannot be deleted.",
                                "Delete Customer", MessageBoxButton.OK, MessageBoxImage.Warning)
                Return
            End If

            If MessageBox.Show($"Delete {selected.Name}?", "Delete Customer",
                               MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.Yes Then
                _customers.Remove(selected)
                ApplyCustomerFilter()
            End If
        End Sub

        ' ===================== 4. RESERVATIONS =====================
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
            Dim id As String = "R-" & _nextReservationNo.ToString()
            _nextReservationNo += 1

            _reservations.Insert(0, New ReservationItem(id, cust.Name, veh.Display, veh.Plate,
                                                        pickup, dropoff, total, "Confirmed"))
            veh.Status = "Reserved"

            CmbResCustomer.SelectedIndex = -1
            CmbResVehicle.SelectedIndex = -1
            RefreshReservationVehicles()
            DgReservations.Items.Refresh()
        End Sub

        Private Sub BtnCancelReservation_Click(sender As Object, e As RoutedEventArgs)
            Dim res = TryCast(DgReservations.SelectedItem, ReservationItem)
            If Not CanChangeReservation(res) Then Return

            res.Status = "Cancelled"
            SetVehicleStatus(res.Plate, "Available")
            RefreshReservationVehicles()
            DgReservations.Items.Refresh()
        End Sub

        Private Sub BtnCompleteReservation_Click(sender As Object, e As RoutedEventArgs)
            Dim res = TryCast(DgReservations.SelectedItem, ReservationItem)
            If Not CanChangeReservation(res) Then Return

            res.Status = "Completed"
            SetVehicleStatus(res.Plate, "Available")
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

        Private Sub SetVehicleStatus(plate As String, status As String)
            Dim veh = _fleet.FirstOrDefault(Function(f) f.Plate = plate)
            If veh IsNot Nothing Then veh.Status = status
        End Sub

        ' ===================== 5. DAILY DISPATCH =====================
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

            item.Status = "Completed"
            SetVehicleStatus(item.Plate, If(item.DispatchType = "Pickup", "Rented", "Available"))
            ApplyDispatchFilter()
        End Sub

        ' ===================== 6. VEHICLE INSPECTION =====================
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

            _inspections.Insert(0, New InspectionItem(Date.Now, veh.Display, inspector, fuel, findings,
                                                      If(passed, "Pass", "Needs Attention")))

            If passed Then
                MessageBox.Show("Inspection saved: Pass.", "Vehicle Inspection",
                                MessageBoxButton.OK, MessageBoxImage.Information)
            Else
                veh.Status = "Maintenance"
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
End Namespace