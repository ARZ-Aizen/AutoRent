Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Effects
Imports AutoRent.Data.Repositories
Imports Microsoft.Win32

Namespace Presentation.Views.User
    Partial Public Class ReservationsView
        Inherits UserControl

        Private Const PageSize As Integer = 10

        Private _customers As New List(Of CustomerItem)
        Private _fleet As New List(Of FleetItem)
        Private _reservations As New List(Of ReservationItem)
        Private _filtered As New List(Of ReservationItem)
        Private _page As Integer = 1
        Private _totalPages As Integer = 1
        Private _initialized As Boolean = False
        Private _editingReservation As ReservationItem = Nothing

        Public Sub New()
            InitializeComponent()
            _initialized = True
        End Sub

        Private Sub ReservationsView_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            CmbStatusFilter.ItemsSource = Nothing
            CmbStatusFilter.Items.Clear()
            CmbStatusFilter.ItemsSource = New String() {"All Status", "Active", "Confirmed"}
            CmbStatusFilter.SelectedIndex = 0

            CmbSort.ItemsSource = Nothing
            CmbSort.Items.Clear()
            CmbSort.ItemsSource = New String() {"Newest First", "Oldest First", "Pickup Date (Soonest)", "Total (High to Low)"}
            CmbSort.SelectedIndex = 0

            ReloadData()
        End Sub

        Private Sub ReloadData()
            Try
                _customers = LoadCustomers()
                _fleet = LoadFleet()
                _reservations = LoadReservations()
            Catch ex As Exception
                ShowDbError("load reservations", ex)
                Return
            End Try

            UpdateStatCards()
            ApplyReservationFilter()
        End Sub

        Private Sub UpdateStatCards()
            Dim active As Integer = 0
            Dim confirmed As Integer = 0
            Dim completed As Integer = 0
            Dim cancelled As Integer = 0
            For Each reservation As ReservationItem In _reservations
                Select Case If(reservation.Status, "").Trim().ToLowerInvariant()
                    Case "active", "rented"
                        active += 1
                    Case "confirmed"
                        confirmed += 1
                    Case "completed"
                        completed += 1
                    Case "cancelled", "canceled"
                        cancelled += 1
                End Select
            Next
            TxtStatTotal.Text = _reservations.Count.ToString("N0")
            TxtStatActive.Text = active.ToString("N0")
            TxtStatConfirmed.Text = confirmed.ToString("N0")
            TxtStatCompleted.Text = completed.ToString("N0")
            TxtStatCancelled.Text = cancelled.ToString("N0")
        End Sub

        Private Sub TxtSearchRes_TextChanged(sender As Object, e As TextChangedEventArgs)
            If Not _initialized Then Return
            TxtSearchWatermark.Visibility = If(String.IsNullOrEmpty(TxtSearchRes.Text), Visibility.Visible, Visibility.Collapsed)
            _page = 1
            ApplyReservationFilter()
        End Sub

        Private Sub FilterInputs_Changed(sender As Object, e As SelectionChangedEventArgs)
            If Not _initialized Then Return
            _page = 1
            ApplyReservationFilter()
        End Sub

        Private Sub BtnResetFilters_Click(sender As Object, e As RoutedEventArgs)
            TxtSearchRes.Text = String.Empty
            TxtSearchWatermark.Visibility = Visibility.Visible
            CmbStatusFilter.SelectedIndex = 0
            CmbSort.SelectedIndex = 0
            _page = 1
            ApplyReservationFilter()
        End Sub

        Private Sub ApplyReservationFilter()
            If _reservations Is Nothing Then Return

            Dim q As String = TxtSearchRes.Text.Trim().ToLowerInvariant()
            Dim statusFilter As String = If(TryCast(CmbStatusFilter.SelectedItem, String), "All Status")
            Dim sortOption As String = If(TryCast(CmbSort.SelectedItem, String), "Newest First")

            Dim result = _reservations.Where(Function(r)
                                                 ' Historical records have their own windows.
                                                 If String.Equals(r.Status, "Completed", StringComparison.OrdinalIgnoreCase) OrElse
                                                    String.Equals(r.Status, "Cancelled", StringComparison.OrdinalIgnoreCase) OrElse
                                                    String.Equals(r.Status, "Canceled", StringComparison.OrdinalIgnoreCase) Then
                                                     Return False
                                                 End If
                                                 Dim matchQuery = (q = "" OrElse
                                                     (r.Id IsNot Nothing AndAlso r.Id.ToLowerInvariant().Contains(q)) OrElse
                                                     ($"r-{r.DbId:D4}".Contains(q)) OrElse
                                                     (r.DbId.ToString().Contains(q)) OrElse
                                                     (r.Customer IsNot Nothing AndAlso r.Customer.ToLowerInvariant().Contains(q)) OrElse
                                                     (r.Vehicle IsNot Nothing AndAlso r.Vehicle.ToLowerInvariant().Contains(q)) OrElse
                                                     (r.Plate IsNot Nothing AndAlso r.Plate.ToLowerInvariant().Contains(q)))

                                                 Dim matchStatus = (statusFilter = "All Status" OrElse
                                                     String.Equals(r.Status, statusFilter, StringComparison.OrdinalIgnoreCase))

                                                 Return matchQuery AndAlso matchStatus
                                             End Function)

            Select Case sortOption
                Case "Oldest First"
                    _filtered = result.OrderBy(Function(r) r.DbId).ToList()
                Case "Pickup Date (Soonest)"
                    _filtered = result.OrderBy(Function(r) r.PickupDate).ToList()
                Case "Total (High to Low)"
                    _filtered = result.OrderByDescending(Function(r) r.Total).ToList()
                Case Else '"Newest First"
                    _filtered = result.OrderByDescending(Function(r) r.DbId).ToList()
            End Select

            _totalPages = Math.Max(1, CInt(Math.Ceiling(_filtered.Count / CDbl(PageSize))))
            If _page > _totalPages Then _page = _totalPages
            If _page < 1 Then _page = 1

            ShowPage()
        End Sub

        Private Sub ShowPage()
            Dim pageItems = _filtered.Skip((_page - 1) * PageSize).Take(PageSize).ToList()
            DgReservations.ItemsSource = pageItems

            If _filtered.Count = 0 Then
                DgReservations.Visibility = Visibility.Collapsed
                BorderEmptyState.Visibility = Visibility.Visible
                TxtPaging.Text = "NO RESERVATIONS FOUND"
            Else
                DgReservations.Visibility = Visibility.Visible
                BorderEmptyState.Visibility = Visibility.Collapsed
                TxtPaging.Text = $"SHOWING {pageItems.Count} OF {_filtered.Count} RESERVATIONS"
            End If

            BtnPrev.IsEnabled = (_page > 1)
            BtnNext.IsEnabled = (_page < _totalPages)

            BuildPageButtons()
        End Sub

        Private Sub BuildPageButtons()
            IcPages.Items.Clear()

            For i = 1 To _totalPages
                Dim btn As New Button()
                btn.Width = 38
                btn.Height = 38
                btn.FontSize = 13
                btn.FontWeight = FontWeights.Bold
                btn.Cursor = Cursors.Hand
                btn.Content = i.ToString()
                btn.Margin = New Thickness(0, 0, 8, 0)
                btn.Tag = i

                If i = _page Then
                    btn.Template = CreatePageTemplate("#70000F", "White", True)
                    btn.Foreground = Brushes.White
                Else
                    btn.Template = CreatePageTemplate("White", "#7A746D", False)
                    btn.Foreground = New SolidColorBrush(CType(ColorConverter.ConvertFromString("#7A746D"), Color))
                End If

                AddHandler btn.Click, AddressOf PageNumber_Click
                IcPages.Items.Add(btn)
            Next
        End Sub

        Private Function CreatePageTemplate(bgColor As String, fgColor As String, hasShadow As Boolean) As ControlTemplate
            Dim template As New ControlTemplate(GetType(Button))

            Dim borderFactory As New FrameworkElementFactory(GetType(Border))
            borderFactory.SetValue(Border.BackgroundProperty, New SolidColorBrush(CType(ColorConverter.ConvertFromString(bgColor), Color)))
            borderFactory.SetValue(Border.CornerRadiusProperty, New CornerRadius(10))

            If hasShadow Then
                borderFactory.SetValue(Border.BorderThicknessProperty, New Thickness(0))
                Dim shadow As New DropShadowEffect With {
                    .BlurRadius = 8,
                    .ShadowDepth = 2,
                    .Direction = 270,
                    .Opacity = 0.2,
                    .Color = CType(ColorConverter.ConvertFromString(bgColor), Color)
                }
                borderFactory.SetValue(Border.EffectProperty, shadow)
            Else
                borderFactory.SetValue(Border.BorderBrushProperty, New SolidColorBrush(CType(ColorConverter.ConvertFromString("#E8E5E1"), Color)))
                borderFactory.SetValue(Border.BorderThicknessProperty, New Thickness(1))
            End If

            Dim contentFactory As New FrameworkElementFactory(GetType(ContentPresenter))
            contentFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center)
            contentFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center)
            borderFactory.AppendChild(contentFactory)

            template.VisualTree = borderFactory
            Return template
        End Function

        Private Sub PageNumber_Click(sender As Object, e As RoutedEventArgs)
            Dim btn = TryCast(sender, Button)
            If btn Is Nothing Then Return
            Dim targetPage As Integer = Convert.ToInt32(btn.Tag)
            If targetPage <> _page Then
                _page = targetPage
                ShowPage()
            End If
        End Sub

        Private Sub BtnPrev_Click(sender As Object, e As RoutedEventArgs)
            If _page > 1 Then
                _page -= 1
                ShowPage()
            End If
        End Sub

        Private Sub BtnNext_Click(sender As Object, e As RoutedEventArgs)
            If _page < _totalPages Then
                _page += 1
                ShowPage()
            End If
        End Sub

        ' ==========================================
        ' COMPLETED / CANCELLED RESERVATION WINDOWS
        ' ==========================================
        Private Sub BtnViewCompleted_Click(sender As Object, e As RoutedEventArgs)
            OpenReservationHistory("Completed")
        End Sub

        Private Sub BtnViewCancelled_Click(sender As Object, e As RoutedEventArgs)
            OpenReservationHistory("Cancelled")
        End Sub

        Private Sub OpenReservationHistory(requestedStatus As String)
            Try
                ' Read all reservations again, not only the currently filtered or paginated rows.
                Dim allReservations As List(Of ReservationItem) = LoadReservations()
                Dim matches As New List(Of ReservationItem)
                For Each res As ReservationItem In allReservations
                    Dim statusValue As String = If(res.Status, "").Trim()
                    If String.Equals(statusValue, requestedStatus, StringComparison.OrdinalIgnoreCase) OrElse
                       (requestedStatus = "Cancelled" AndAlso String.Equals(statusValue, "Canceled", StringComparison.OrdinalIgnoreCase)) Then
                        matches.Add(res)
                    End If
                Next
                Dim dlg As New ReservationHistoryWindow(requestedStatus, matches)
                Dim owner As Window = Window.GetWindow(Me)
                If owner IsNot Nothing Then dlg.Owner = owner
                dlg.ShowDialog()
                ReloadData()
            Catch ex As Exception
                ShowDbError("open " & requestedStatus.ToLowerInvariant() & " reservations", ex)
            End Try
        End Sub

        ' =========================
        ' NEW RESERVATION MODAL
        ' =========================

        Private Sub BtnOpenNewReservation_Click(sender As Object, e As RoutedEventArgs)
            _editingReservation = Nothing

            TxtModalIcon.Text = ChrW(&HE787)
            TxtModalTitle.Text = "New Vehicle Reservation"
            TxtModalSubtitle.Text = "Book an available fleet vehicle for a customer with a verified driver's license"
            BtnSaveReservation.Content = "Confirm Booking"

            Dim licensedCustomers = _customers.Where(Function(c) c.HasLicense).OrderBy(Function(c) c.Name).ToList()
            CmbModalCustomer.ItemsSource = licensedCustomers

            Dim availableCars = _fleet.Where(Function(f) String.Equals(f.Status, "Available", StringComparison.OrdinalIgnoreCase)).ToList()
            Dim categories = availableCars.Select(Function(f) If(f.Category, String.Empty).Trim()).Where(Function(c) c <> "").Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(Function(c) c, StringComparer.OrdinalIgnoreCase).ToList()

            CmbModalCategory.ItemsSource = categories
            CmbModalCategory.SelectedIndex = -1
            CmbModalCategory.IsEnabled = (categories.Count > 0)
            CmbModalVehicle.ItemsSource = Nothing
            CmbModalVehicle.SelectedIndex = -1
            CmbModalVehicle.IsEnabled = False
            TxtAvailableVehicleCount.Text = If(categories.Count = 0,
                                               "No vehicles are available for booking. Check Fleet Inventory.",
                                               "Select a category to see available vehicles.")

            DpModalPickup.SelectedDate = Date.Today
            DpModalReturn.SelectedDate = Date.Today.AddDays(1)

            CmbModalCustomer.SelectedIndex = -1
            CmbModalVehicle.SelectedIndex = -1

            BorderModalError.Visibility = Visibility.Collapsed
            BorderCustomerLicenseNotice.Visibility = Visibility.Collapsed
            ReservationModalOverlay.Visibility = Visibility.Visible
            RecalculateModalCost()
        End Sub

        Private Sub BtnEditRow_Click(sender As Object, e As RoutedEventArgs)
            Dim res = TryCast(CType(sender, FrameworkElement).Tag, ReservationItem)
            If res Is Nothing Then Return

            _editingReservation = res

            TxtModalIcon.Text = ChrW(&HE70F)
            TxtModalTitle.Text = "Edit Reservation"
            TxtModalSubtitle.Text = $"Modify booking details for reservation {res.Id}"
            BtnSaveReservation.Content = "Save Changes"

            Dim licensedCustomers = _customers.Where(Function(c) c.HasLicense).OrderBy(Function(c) c.Name).ToList()
            CmbModalCustomer.ItemsSource = licensedCustomers
            CmbModalCustomer.SelectedItem = licensedCustomers.FirstOrDefault(Function(c) String.Equals(c.Name, res.Customer, StringComparison.OrdinalIgnoreCase))

            Dim currentVeh = _fleet.FirstOrDefault(Function(f) f.DbId = res.VehicleId)
            Dim eligibleCars = _fleet.Where(Function(f) String.Equals(f.Status, "Available", StringComparison.OrdinalIgnoreCase) OrElse f.DbId = res.VehicleId).ToList()
            Dim categories = eligibleCars.Select(Function(f) If(f.Category, String.Empty).Trim()).Where(Function(c) c <> "").Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(Function(c) c, StringComparer.OrdinalIgnoreCase).ToList()

            CmbModalCategory.ItemsSource = categories
            If currentVeh IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(currentVeh.Category) Then
                CmbModalCategory.SelectedItem = categories.FirstOrDefault(Function(c) String.Equals(c, currentVeh.Category, StringComparison.OrdinalIgnoreCase))

                Dim matchingCars = eligibleCars.Where(Function(f) String.Equals(f.Category, currentVeh.Category, StringComparison.OrdinalIgnoreCase)).
                                                OrderBy(Function(f) f.Make, StringComparer.OrdinalIgnoreCase).
                                                ThenBy(Function(f) f.Model, StringComparer.OrdinalIgnoreCase).
                                                ThenBy(Function(f) f.Plate, StringComparer.OrdinalIgnoreCase).
                                                ToList()
                CmbModalVehicle.ItemsSource = matchingCars
                CmbModalVehicle.IsEnabled = (matchingCars.Count > 0)
                CmbModalVehicle.SelectedItem = matchingCars.FirstOrDefault(Function(f) f.DbId = res.VehicleId)
                Dim vehicleWord = If(matchingCars.Count = 1, "vehicle", "vehicles")
                TxtAvailableVehicleCount.Text = $"{matchingCars.Count} available {vehicleWord} in {currentVeh.Category}."
            Else
                CmbModalCategory.SelectedIndex = -1
                CmbModalVehicle.ItemsSource = Nothing
                CmbModalVehicle.IsEnabled = False
                TxtAvailableVehicleCount.Text = "Select a category to see available vehicles."
            End If

            DpModalPickup.SelectedDate = res.PickupDate
            DpModalReturn.SelectedDate = res.ReturnDate

            BorderModalError.Visibility = Visibility.Collapsed
            BorderCustomerLicenseNotice.Visibility = Visibility.Collapsed
            ReservationModalOverlay.Visibility = Visibility.Visible
            RecalculateModalCost()
        End Sub

        Private Sub CmbModalCategory_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            If CmbModalVehicle Is Nothing OrElse TxtAvailableVehicleCount Is Nothing Then Return

            CmbModalVehicle.ItemsSource = Nothing
            CmbModalVehicle.SelectedIndex = -1
            CmbModalVehicle.IsEnabled = False

            Dim selectedCategory = TryCast(CmbModalCategory.SelectedItem, String)
            If String.IsNullOrWhiteSpace(selectedCategory) Then
                TxtAvailableVehicleCount.Text = "Select a category to see available vehicles."
                RecalculateModalCost()
                Return
            End If

            Dim matchingCars = _fleet.Where(Function(f)
                                                Dim isCurrent = (_editingReservation IsNot Nothing AndAlso f.DbId = _editingReservation.VehicleId)
                                                Return (isCurrent OrElse String.Equals(f.Status, "Available", StringComparison.OrdinalIgnoreCase)) AndAlso
                                                       String.Equals(f.Category, selectedCategory, StringComparison.OrdinalIgnoreCase)
                                            End Function).
                                     OrderBy(Function(f) f.Make, StringComparer.OrdinalIgnoreCase).
                                     ThenBy(Function(f) f.Model, StringComparer.OrdinalIgnoreCase).
                                     ThenBy(Function(f) f.Plate, StringComparer.OrdinalIgnoreCase).
                                     ToList()

            CmbModalVehicle.ItemsSource = matchingCars
            CmbModalVehicle.IsEnabled = (matchingCars.Count > 0)
            Dim vehicleWord = If(matchingCars.Count = 1, "vehicle", "vehicles")
            TxtAvailableVehicleCount.Text = If(matchingCars.Count = 0,
                                               "No available vehicles in this category.",
                                               $"{matchingCars.Count} available {vehicleWord} in {selectedCategory}.")
            CmbModalVehicle.ToolTip = If(matchingCars.Count = 0,
                                         "No available vehicles in this category",
                                         "Choose an available vehicle")
            RecalculateModalCost()
        End Sub

        Private Sub CmbModalCustomer_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            Dim cust = TryCast(CmbModalCustomer.SelectedItem, CustomerItem)
            If cust Is Nothing Then
                BorderCustomerLicenseNotice.Visibility = Visibility.Collapsed
                Return
            End If

            If Not cust.HasLicense Then
                TxtCustomerLicenseNotice.Text = $"'{cust.Name}' does not have a verified driver's license on file and cannot rent. A valid driver's license must be provided under Customer Management before renting."
                BorderCustomerLicenseNotice.Visibility = Visibility.Visible
            Else
                BorderCustomerLicenseNotice.Visibility = Visibility.Collapsed
            End If
        End Sub

        Private Sub BtnCloseModal_Click(sender As Object, e As RoutedEventArgs)
            _editingReservation = Nothing
            ReservationModalOverlay.Visibility = Visibility.Collapsed
        End Sub

        Private Sub ModalBackdrop_MouseDown(sender As Object, e As MouseButtonEventArgs)
            _editingReservation = Nothing
            ReservationModalOverlay.Visibility = Visibility.Collapsed
        End Sub

        Private Sub ModalInputs_Changed(sender As Object, e As SelectionChangedEventArgs)
            RecalculateModalCost()
        End Sub

        Private Sub ModalDates_Changed(sender As Object, e As SelectionChangedEventArgs)
            RecalculateModalCost()
        End Sub

        Private Sub RecalculateModalCost()
            If TxtModalDailyRate Is Nothing Then Return

            Dim veh = TryCast(CmbModalVehicle.SelectedItem, FleetItem)
            Dim rate As Decimal = If(veh IsNot Nothing, veh.DailyRate, 0D)
            TxtModalDailyRate.Text = $"₱{rate:N2} / day"

            Dim pickup = DpModalPickup.SelectedDate.GetValueOrDefault(Date.Today)
            Dim dropoff = DpModalReturn.SelectedDate.GetValueOrDefault(Date.Today.AddDays(1))

            Dim days As Integer = Math.Max(1, (dropoff.Date - pickup.Date).Days)
            TxtModalDuration.Text = If(days = 1, "1 day", $"{days} days")

            Dim total As Decimal = days * rate
            TxtModalTotal.Text = $"₱{total:N2}"
        End Sub

        Private Sub ShowModalError(msg As String)
            TxtModalError.Text = msg
            BorderModalError.Visibility = Visibility.Visible
        End Sub

        Private Sub BtnSaveReservation_Click(sender As Object, e As RoutedEventArgs)
            Dim cust = TryCast(CmbModalCustomer.SelectedItem, CustomerItem)
            Dim selectedCategory = TryCast(CmbModalCategory.SelectedItem, String)
            Dim veh = TryCast(CmbModalVehicle.SelectedItem, FleetItem)

            If cust Is Nothing Then
                ShowModalError("Please select a registered customer.")
                Return
            End If

            If Not cust.HasLicense Then
                ShowModalError($"Customer '{cust.Name}' does not have a verified driver's license on file and cannot rent a vehicle. Please provide a valid driver's license in Customer Management before proceeding.")
                Return
            End If

            If String.IsNullOrWhiteSpace(selectedCategory) Then
                ShowModalError("Please select a vehicle category first.")
                Return
            End If

            If veh Is Nothing Then
                ShowModalError("Please select an available vehicle from that category.")
                Return
            End If

            If Not String.Equals(veh.Category, selectedCategory, StringComparison.OrdinalIgnoreCase) Then
                ShowModalError("Selected vehicle does not match the chosen category. Please choose again.")
                Return
            End If

            If Not DpModalPickup.SelectedDate.HasValue OrElse Not DpModalReturn.SelectedDate.HasValue Then
                ShowModalError("Please specify both pickup and return dates.")
                Return
            End If

            Dim pickup = DpModalPickup.SelectedDate.Value
            Dim dropoff = DpModalReturn.SelectedDate.Value

            If dropoff <= pickup Then
                ShowModalError("Return date must be strictly after the pickup date.")
                Return
            End If

            Dim days As Integer = (dropoff - pickup).Days
            Dim total As Decimal = days * veh.DailyRate

            Try
                If _editingReservation Is Nothing Then
                    Dim problem = ClientRepository.CreateReservation(cust.Name, veh.DbId, pickup, dropoff, total)
                    If problem IsNot Nothing Then
                        ShowModalError(problem)
                        Return
                    End If
                Else
                    Dim problem = ClientRepository.UpdateReservation(_editingReservation.DbId, cust.Name, veh.DbId, _editingReservation.VehicleId, pickup, dropoff, total)
                    If problem IsNot Nothing Then
                        ShowModalError(problem)
                        Return
                    End If
                End If
            Catch ex As Exception
                ShowModalError($"Database error: {ex.Message}")
                Return
            End Try

            _editingReservation = Nothing
            ReservationModalOverlay.Visibility = Visibility.Collapsed
            ReloadData()
        End Sub

        ' =========================
        ' ROW ACTIONS: COMPLETE / CANCEL
        ' =========================

        Private Sub BtnCompleteRow_Click(sender As Object, e As RoutedEventArgs)
            Dim res = TryCast(CType(sender, FrameworkElement).Tag, ReservationItem)
            If res Is Nothing Then Return

            Dim confirm = MessageBox.Show($"Mark reservation {res.Id} for {res.Customer} as Completed?" & vbCrLf &
                                          "The vehicle will be released and returned to Available status.",
                                          "Complete Reservation",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Question)
            If confirm <> MessageBoxResult.Yes Then Return

            Try
                ClientRepository.CloseReservation(res.DbId, res.VehicleId, "Completed")
            Catch ex As Exception
                ShowDbError("complete reservation", ex)
                Return
            End Try

            ReloadData()
        End Sub

        Private Sub BtnCancelRow_Click(sender As Object, e As RoutedEventArgs)
            Dim res = TryCast(CType(sender, FrameworkElement).Tag, ReservationItem)
            If res Is Nothing Then Return

            Dim confirm = MessageBox.Show($"Are you sure you want to cancel reservation {res.Id} for {res.Customer}?",
                                          "Cancel Reservation",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Question)
            If confirm <> MessageBoxResult.Yes Then Return

            Try
                ClientRepository.CloseReservation(res.DbId, res.VehicleId, "Cancelled")
            Catch ex As Exception
                ShowDbError("cancel reservation", ex)
                Return
            End Try

            ReloadData()
        End Sub

        ' =========================
        ' CSV EXPORT
        ' =========================

        Private Sub BtnExportCsv_Click(sender As Object, e As RoutedEventArgs)
            If _filtered Is Nothing OrElse _filtered.Count = 0 Then
                MessageBox.Show("There are no reservation records to export.", "Export CSV", MessageBoxButton.OK, MessageBoxImage.Information)
                Return
            End If

            Dim dialog As New SaveFileDialog() With {
                .FileName = $"AutoRent_Reservations_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
                .DefaultExt = ".csv",
                .Filter = "CSV documents (.csv)|*.csv"
            }

            If dialog.ShowDialog() = True Then
                Try
                    Dim sb As New StringBuilder()
                    sb.AppendLine("Reservation ID,Customer,Vehicle,Plate,Pickup Date,Return Date,Duration Days,Total Amount,Status")

                    For Each r In _filtered
                        Dim id = EscapeCsv(r.Id)
                        Dim cust = EscapeCsv(r.Customer)
                        Dim veh = EscapeCsv(r.Vehicle)
                        Dim plate = EscapeCsv(r.Plate)
                        Dim pickup = EscapeCsv(r.PickupDate.ToString("yyyy-MM-dd"))
                        Dim dropoff = EscapeCsv(r.ReturnDate.ToString("yyyy-MM-dd"))
                        Dim duration = EscapeCsv(r.DurationDisplay)
                        Dim total = EscapeCsv(r.Total.ToString("F2"))
                        Dim status = EscapeCsv(r.StatusUpper)
                        sb.AppendLine($"{id},{cust},{veh},{plate},{pickup},{dropoff},{duration},{total},{status}")
                    Next

                    File.WriteAllText(dialog.FileName, sb.ToString(), Encoding.UTF8)
                    MessageBox.Show($"Successfully exported {_filtered.Count} reservation records to CSV.", "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information)
                Catch ex As Exception
                    MessageBox.Show($"Could not export CSV: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error)
                End Try
            End If
        End Sub

        Private Function EscapeCsv(val As String) As String
            If String.IsNullOrEmpty(val) Then Return """" & """"
            If val.Contains(",") OrElse val.Contains("""") OrElse val.Contains(vbCr) OrElse val.Contains(vbLf) Then
                Return """" & val.Replace("""", """""") & """"
            End If
            Return """" & val & """"
        End Function
    End Class

    ' Read-only popup. Does not alter reservation status or payments.
    Public Class ReservationHistoryWindow
        Inherits Window

        Private ReadOnly _all As List(Of ReservationItem)
        Private ReadOnly _grid As New DataGrid()
        Private ReadOnly _search As New TextBox()
        Private ReadOnly _count As New TextBlock()
        Private ReadOnly _status As String

        Public Sub New(statusName As String, records As List(Of ReservationItem))
            _status = statusName
            _all = records.OrderByDescending(Function(r) r.DbId).ToList()
            Title = statusName & " Reservations - AutoRent"
            Width = 1070
            Height = 630
            MinWidth = 760
            MinHeight = 440
            WindowStartupLocation = WindowStartupLocation.CenterOwner
            Background = New SolidColorBrush(Color.FromRgb(&HF8, &HF7, &HF6))
            FontFamily = New FontFamily("Segoe UI")

            Dim layout As New Grid With {.Margin = New Thickness(22)}
            layout.RowDefinitions.Add(New RowDefinition With {.Height = GridLength.Auto})
            layout.RowDefinitions.Add(New RowDefinition With {.Height = GridLength.Auto})
            layout.RowDefinitions.Add(New RowDefinition With {.Height = New GridLength(1, GridUnitType.Star)})
            layout.RowDefinitions.Add(New RowDefinition With {.Height = GridLength.Auto})
            Content = layout

            Dim titleText As New TextBlock With {
                .Text = statusName & " Reservations",
                .FontSize = 23, .FontWeight = FontWeights.Bold,
                .Foreground = New SolidColorBrush(Color.FromRgb(&H30, &H2A, &H2B))
            }
            Grid.SetRow(titleText, 0)
            layout.Children.Add(titleText)

            Dim searchArea As New Grid With {.Margin = New Thickness(0, 14, 0, 12)}
            searchArea.ColumnDefinitions.Add(New ColumnDefinition With {.Width = New GridLength(1, GridUnitType.Star)})
            searchArea.ColumnDefinitions.Add(New ColumnDefinition With {.Width = GridLength.Auto})
            _search.Height = 36
            _search.VerticalContentAlignment = VerticalAlignment.Center
            _search.Padding = New Thickness(9, 0, 9, 0)
            _search.ToolTip = "Find by booking ID, customer, vehicle or plate number"
            Grid.SetColumn(_search, 0)
            searchArea.Children.Add(_search)
            _count.Margin = New Thickness(12, 0, 0, 0)
            _count.VerticalAlignment = VerticalAlignment.Center
            _count.Foreground = Brushes.DimGray
            Grid.SetColumn(_count, 1)
            searchArea.Children.Add(_count)
            Grid.SetRow(searchArea, 1)
            layout.Children.Add(searchArea)

            _grid.AutoGenerateColumns = False
            _grid.IsReadOnly = True
            _grid.CanUserAddRows = False
            _grid.CanUserDeleteRows = False
            _grid.RowHeight = 40
            _grid.ColumnHeaderHeight = 42
            _grid.GridLinesVisibility = DataGridGridLinesVisibility.None
            _grid.AlternatingRowBackground = New SolidColorBrush(Color.FromRgb(&HFC, &HFA, &HFA))
            _grid.Background = Brushes.White
            _grid.BorderBrush = New SolidColorBrush(Color.FromRgb(&HE8, &HE5, &HE1))
            _grid.SelectionUnit = DataGridSelectionUnit.FullRow
            _grid.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
            _grid.VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            AddColumn("Booking ID", "Id", 100)
            AddColumn("Customer", "Customer", 160)
            AddColumn("Plate", "Plate", 115)
            AddColumn("Vehicle", "Vehicle", 200)
            AddColumn("Pickup Date", "PickupDate", 125, "dd MMM yyyy")
            AddColumn("Return Date", "ReturnDate", 125, "dd MMM yyyy")
            AddColumn("Amount", "TotalDisplay", 130)
            AddColumn("Status", "Status", 105)
            Grid.SetRow(_grid, 2)
            layout.Children.Add(_grid)

            Dim close As New Button With {
                .Content = "Close", .Width = 105, .Height = 36,
                .HorizontalAlignment = HorizontalAlignment.Right,
                .Margin = New Thickness(0, 12, 0, 0),
                .Background = New SolidColorBrush(Color.FromRgb(&H70, &H0, &HF)),
                .Foreground = Brushes.White, .Cursor = Cursors.Hand
            }
            AddHandler close.Click, Sub(sender, e) Me.Close()
            Grid.SetRow(close, 3)
            layout.Children.Add(close)
            AddHandler _search.TextChanged, AddressOf SearchChanged
            ApplySearch()
        End Sub

        Private Sub AddColumn(heading As String, field As String, width As Double, Optional fmt As String = Nothing)
            Dim binding As New System.Windows.Data.Binding(field)
            If Not String.IsNullOrWhiteSpace(fmt) Then binding.StringFormat = fmt
            _grid.Columns.Add(New DataGridTextColumn With {.Header = heading, .binding = binding, .width = New DataGridLength(width)})
        End Sub

        Private Sub SearchChanged(sender As Object, e As TextChangedEventArgs)
            ApplySearch()
        End Sub

        Private Sub ApplySearch()
            Dim term As String = _search.Text.Trim()
            Dim filtered = _all.Where(Function(r)
                                          Return term.Length = 0 OrElse
                    If(r.Id, "").IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0 OrElse
                    If(r.Customer, "").IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0 OrElse
                    If(r.Vehicle, "").IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0 OrElse
                    If(r.Plate, "").IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0
                                      End Function).ToList()
            _grid.ItemsSource = filtered
            _count.Text = filtered.Count.ToString("N0") & " / " & _all.Count.ToString("N0") & " records"
        End Sub
    End Class

End Namespace
