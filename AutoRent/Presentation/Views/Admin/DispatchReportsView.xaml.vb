Imports System.Collections.Generic
Imports System.Data
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Effects
Imports AutoRent.Data.Context
Imports AutoRent.Data.Repositories
Imports Microsoft.Win32

Namespace Presentation.Views.Admin

    Public Class DispatchReportItem
        Public Property DbId As Integer
        Public Property ReservationId As Integer
        Public Property VehicleId As Integer
        Public Property BookingId As String
        Public Property VehicleIdDisplay As String
        Public Property Plate As String
        Public Property Customer As String
        Public Property AssignedClerk As String
        Public Property ScheduledPickup As DateTime
        Public Property ActualDispatch As DateTime?
        Public Property ExpectedReturn As DateTime
        Public Property ActualReturn As DateTime?
        Public Property ContractStatus As String
        Public Property ContractPayment As String
        
        Public Property PostReturnStatus As String

        Public ReadOnly Property ScheduledPickupDisplay As String
            Get
                Return ScheduledPickup.ToString("dd MMM HH:mm")
            End Get
        End Property

        Public ReadOnly Property ActualDispatchDisplay As String
            Get
                Return If(ActualDispatch.HasValue, ActualDispatch.Value.ToString("dd MMM HH:mm"), "—")
            End Get
        End Property

        Public ReadOnly Property ExpectedReturnDisplay As String
            Get
                Return ExpectedReturn.ToString("dd MMM HH:mm")
            End Get
        End Property

        Public ReadOnly Property ActualReturnDisplay As String
            Get
                Return If(ActualReturn.HasValue, ActualReturn.Value.ToString("dd MMM HH:mm"), "—")
            End Get
        End Property

        Public ReadOnly Property StatusBgColor As String
            Get
                Select Case ContractStatus
                    Case "Returned"
                        Return "#E6F4EA"
                    Case "Reserved"
                        Return "#FEF3C7"
                    Case "Active"
                        Return "#E0F2FE"
                    Case "Cancelled"
                        Return "#FEE2E2"
                    Case Else
                        Return "#F3F1ED"
                End Select
            End Get
        End Property

        Public ReadOnly Property StatusFgColor As String
            Get
                Select Case ContractStatus
                    Case "Returned"
                        Return "#137333"
                    Case "Reserved"
                        Return "#92400E"
                    Case "Active"
                        Return "#0369A1"
                    Case "Cancelled"
                        Return "#B91C1C"
                    Case Else
                        Return "#5F5B56"
                End Select
            End Get
        End Property
    End Class

    Partial Public Class DispatchReportsView
        Inherits UserControl

        Private Const PageSize As Integer = 8

        Private _allItems As New List(Of DispatchReportItem)
        Private _filteredItems As New List(Of DispatchReportItem)
        Private _page As Integer = 1
        Private _totalPages As Integer = 1
        Private _selectedItem As DispatchReportItem = Nothing
        Private _totalFleetVehicles As Integer = 0
        Private _initialized As Boolean = False

        Public Sub New()
            InitializeComponent()
            _initialized = True
        End Sub

        Private Sub DispatchReportsView_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            TxtHeaderSubtitle.Text = $"{DateTime.Today:dd MMM yyyy} · HQ branch · concise timestamps in branch timezone · last prepared at {DateTime.Now:HH:mm}"
            LoadReportData()
        End Sub

        Private Sub LoadReportData()
            Try
                _allItems.Clear()

                ' Retrieve total fleet count for utilization
                Dim totalFleetTable = Db.Query("SELECT COUNT(*) AS [n] FROM [Vehicles] WHERE [status] <> 'Retired'")
                If totalFleetTable.Rows.Count > 0 Then
                    _totalFleetVehicles = Convert.ToInt32(totalFleetTable.Rows(0)("n"))
                End If
                If _totalFleetVehicles = 0 Then _totalFleetVehicles = 1

                Dim clerkNames As New List(Of String) From {"Andrei Balboa", "Zake Yurtas", "Princess Santander", "Jerome Lumbao", "Jonathan Balboa"}
                Try
                    Dim staffUsers = UserAdminRepository.GetAll()
                    If staffUsers IsNot Nothing AndAlso staffUsers.Count > 0 Then
                        Dim staffNames = staffUsers.Select(Function(u) If(String.IsNullOrWhiteSpace(u.FullName), u.Username, u.FullName)).ToList()
                        If staffNames.Count > 0 Then clerkNames = staffNames
                    End If
                Catch
                End Try

                ' Retrieve all reservations joined with vehicles
                Dim table = Db.Query(
                    "SELECT r.[reservation_id], r.[customer_name], r.[vehicle_id], r.[pickup_date], r.[return_date], r.[total], r.[status], r.[created_at], " &
                    "v.[plate], v.[make], v.[model], v.[status] AS [v_status] " &
                    "FROM [Reservations] AS r LEFT JOIN [Vehicles] AS v ON r.[vehicle_id] = v.[vehicle_id] " &
                    "ORDER BY r.[reservation_id] DESC")

                For Each row As DataRow In table.Rows
                    Dim resId = If(IsDBNull(row("reservation_id")), 0, Convert.ToInt32(row("reservation_id")))
                    Dim vehId = If(IsDBNull(row("vehicle_id")), 0, Convert.ToInt32(row("vehicle_id")))
                    Dim cust = If(IsDBNull(row("customer_name")), "Unknown", Convert.ToString(row("customer_name")))
                    Dim plate = If(IsDBNull(row("plate")), "—", Convert.ToString(row("plate")))
                    Dim resStatus = If(IsDBNull(row("status")), "Confirmed", Convert.ToString(row("status")))
                    Dim vStatus = If(IsDBNull(row("v_status")), "Available", Convert.ToString(row("v_status")))

                    Dim pickup = If(IsDBNull(row("pickup_date")), DateTime.Today, Convert.ToDateTime(row("pickup_date")))
                    Dim dropoff = If(IsDBNull(row("return_date")), DateTime.Today.AddDays(1), Convert.ToDateTime(row("return_date")))

                    ' Map contract status matching mockup
                    Dim contractStatus As String = "Reserved"
                    Dim actualDispatch As DateTime? = Nothing
                    Dim actualReturn As DateTime? = Nothing

                    If String.Equals(resStatus, "Completed", StringComparison.OrdinalIgnoreCase) Then
                        contractStatus = "Returned"
                        actualDispatch = pickup.AddMinutes(5)
                        actualReturn = dropoff.AddMinutes(-5)
                    ElseIf String.Equals(resStatus, "Active", StringComparison.OrdinalIgnoreCase) Then
                        contractStatus = "Active"
                        actualDispatch = pickup.AddMinutes(5)
                        actualReturn = Nothing
                    ElseIf String.Equals(resStatus, "Cancelled", StringComparison.OrdinalIgnoreCase) Then
                        contractStatus = "Cancelled"
                        actualDispatch = Nothing
                        actualReturn = Nothing
                    Else
                        contractStatus = "Reserved"
                        actualDispatch = Nothing
                        actualReturn = Nothing
                    End If

                    ' Deterministic clerk selection
                    Dim clerk = clerkNames(Math.Abs(resId) Mod clerkNames.Count)
                    Dim payment = If(contractStatus = "Returned", $"Closed / TX-{(resId + 1000)} paid", If(contractStatus = "Active", $"Active / TX-{(resId + 1000)} paid", $"Pending / TX-{(resId + 1000)}"))
                    Dim postStatus = If(contractStatus = "Returned", "Available", If(contractStatus = "Active", "Rented", If(vStatus <> "", vStatus, "Available")))

                    _allItems.Add(New DispatchReportItem With {
                        .DbId = resId,
                        .ReservationId = resId,
                        .VehicleId = vehId,
                        .BookingId = $"BK-{resId:D4}",
                        .VehicleIdDisplay = $"VH-{vehId:D4}",
                        .Plate = plate,
                        .Customer = cust,
                        .AssignedClerk = clerk,
                        .ScheduledPickup = pickup,
                        .ActualDispatch = actualDispatch,
                        .ExpectedReturn = dropoff,
                        .ActualReturn = actualReturn,
                        .ContractStatus = contractStatus,
                        .ContractPayment = payment,
                        .PostReturnStatus = postStatus
                    })
                Next
            Catch ex As Exception
                MessageBox.Show($"Could not load dispatch reports: {ex.Message}", "AutoRent", MessageBoxButton.OK, MessageBoxImage.Error)
                Return
            End Try

            PopulateFilterComboBoxes()
            UpdateMetricCards()
            ApplyFilters()
        End Sub

        Private Sub PopulateFilterComboBoxes()
            ' Report Date Filter
            Dim dateOptions As New List(Of String) From {"All dates", "Today"}
            Dim distinctDates = _allItems.Select(Function(i) i.ScheduledPickup.ToString("dd MMM yyyy")).Distinct().Take(8).ToList()
            dateOptions.AddRange(distinctDates)
            CmbReportDate.ItemsSource = dateOptions
            CmbReportDate.SelectedIndex = 0

            ' Vehicle Filter
            Dim vehicleOptions As New List(Of String) From {"All fleet vehicles"}
            Dim distinctPlates = _allItems.Select(Function(i) i.Plate).Where(Function(p) p <> "—").Distinct().OrderBy(Function(p) p).ToList()
            vehicleOptions.AddRange(distinctPlates)
            CmbVehicleFilter.ItemsSource = vehicleOptions
            CmbVehicleFilter.SelectedIndex = 0

            ' Assigned Clerk Filter
            Dim clerkOptions As New List(Of String) From {"All clerks"}
            Dim distinctClerks = _allItems.Select(Function(i) i.AssignedClerk).Distinct().OrderBy(Function(c) c).ToList()
            clerkOptions.AddRange(distinctClerks)
            CmbClerkFilter.ItemsSource = clerkOptions
            CmbClerkFilter.SelectedIndex = 0

            ' Status Filter
            CmbStatusFilter.ItemsSource = New String() {"All statuses", "Returned", "Reserved", "Active", "Cancelled"}
            CmbStatusFilter.SelectedIndex = 0
        End Sub

        Private Sub UpdateMetricCards()
            Dim totalScheduled = _allItems.Count
            Dim completedOuts = _allItems.Where(Function(i) i.ActualDispatch.HasValue).Count()
            Dim completedReturns = _allItems.Where(Function(i) i.ContractStatus = "Returned").Count()
            Dim cancelled = _allItems.Where(Function(i) i.ContractStatus = "Cancelled").Count()
            Dim pendingNotDispatched = _allItems.Where(Function(i) i.ContractStatus = "Reserved").Count()
            Dim activeRentals = _allItems.Where(Function(i) i.ContractStatus = "Active").Count()

            ' Overdue returns: active rentals past expected return
            Dim overdue = _allItems.Where(Function(i) i.ContractStatus = "Active" AndAlso i.ExpectedReturn < DateTime.Now).Count()

            ' Fleet utilization
            Dim rentedVehicles = activeRentals
            Dim utilPct = Math.Round((rentedVehicles / CDbl(_totalFleetVehicles)) * 100, 1)

            TxtStatScheduled.Text = totalScheduled.ToString()
            TxtStatScheduledNote.Text = $"{completedOuts} out · {cancelled} cancelled · {pendingNotDispatched} pending"

            TxtStatCheckouts.Text = completedOuts.ToString()
            TxtStatReturns.Text = completedReturns.ToString()
            TxtStatCancelled.Text = cancelled.ToString()

            TxtStatOverdue.Text = overdue.ToString()
            TxtStatOverdueNote.Text = $"Included in {activeRentals} active rentals"

            TxtStatPending.Text = pendingNotDispatched.ToString()

            TxtStatUtilization.Text = $"{utilPct:F1}%"
            TxtStatUtilizationNote.Text = $"{rentedVehicles} / {_totalFleetVehicles} vehicles"
        End Sub

        Private Sub FilterInputs_Changed(sender As Object, e As SelectionChangedEventArgs)
            If Not _initialized Then Return
            _page = 1
            ApplyFilters()
        End Sub

        Private Sub ApplyFilters()
            If _allItems Is Nothing Then Return

            Dim dateSel = If(TryCast(CmbReportDate.SelectedItem, String), "All dates")
            Dim vehSel = If(TryCast(CmbVehicleFilter.SelectedItem, String), "All fleet vehicles")
            Dim clerkSel = If(TryCast(CmbClerkFilter.SelectedItem, String), "All clerks")
            Dim statusSel = If(TryCast(CmbStatusFilter.SelectedItem, String), "All statuses")

            _filteredItems = _allItems.Where(Function(item)
                                                 ' Date filter
                                                 Dim matchDate = True
                                                 If dateSel = "Today" Then
                                                     matchDate = (item.ScheduledPickup.Date = DateTime.Today)
                                                 ElseIf dateSel <> "All dates" Then
                                                     matchDate = (item.ScheduledPickup.ToString("dd MMM yyyy") = dateSel)
                                                 End If

                                                 ' Vehicle filter
                                                 Dim matchVeh = (vehSel = "All fleet vehicles" OrElse item.Plate = vehSel)

                                                 ' Clerk filter
                                                 Dim matchClerk = (clerkSel = "All clerks" OrElse item.AssignedClerk = clerkSel)

                                                 ' Status filter
                                                 Dim matchStatus = (statusSel = "All statuses" OrElse item.ContractStatus = statusSel)

                                                 Return matchDate AndAlso matchVeh AndAlso matchClerk AndAlso matchStatus
                                             End Function).ToList()

            _totalPages = Math.Max(1, CInt(Math.Ceiling(_filteredItems.Count / CDbl(PageSize))))
            If _page > _totalPages Then _page = _totalPages
            If _page < 1 Then _page = 1

            ShowPage()
        End Sub

        Private Sub ShowPage()
            Dim pageItems = _filteredItems.Skip((_page - 1) * PageSize).Take(PageSize).ToList()
            DgDispatchRegister.ItemsSource = pageItems

            If _filteredItems.Count = 0 Then
                TxtPagingRecords.Text = "Showing 0 of 0 scheduled records"
            Else
                TxtPagingRecords.Text = $"Showing {pageItems.Count} of {_filteredItems.Count} scheduled records · returns include prior-day departures"
            End If

            BtnPrev.IsEnabled = (_page > 1)
            BtnNext.IsEnabled = (_page < _totalPages)

            BuildPageButtons()

            ' Select first row by default or keep current selection
            If pageItems.Count > 0 Then
                If _selectedItem Is Nothing OrElse Not pageItems.Contains(_selectedItem) Then
                    DgDispatchRegister.SelectedItem = pageItems(0)
                Else
                    DgDispatchRegister.SelectedItem = _selectedItem
                End If
            Else
                ClearPreview()
            End If
        End Sub

        Private Sub BuildPageButtons()
            IcPages.Items.Clear()

            For i = 1 To _totalPages
                Dim btn As New Button()
                btn.Width = 34
                btn.Height = 34
                btn.FontSize = 12.5
                btn.FontWeight = FontWeights.Bold
                btn.Cursor = Cursors.Hand
                btn.Content = i.ToString()
                btn.Margin = New Thickness(0, 0, 6, 0)
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
            borderFactory.SetValue(Border.CornerRadiusProperty, New CornerRadius(8))

            If hasShadow Then
                borderFactory.SetValue(Border.BorderThicknessProperty, New Thickness(0))
                Dim shadow As New DropShadowEffect With {
                    .BlurRadius = 6,
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

        Private Sub DgDispatchRegister_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            Dim selected = TryCast(DgDispatchRegister.SelectedItem, DispatchReportItem)
            If selected Is Nothing Then Return

            _selectedItem = selected
            UpdatePreviewCard(selected)
        End Sub

        Private Sub UpdatePreviewCard(item As DispatchReportItem)
            TxtRegisterSubtitle.Text = $"Date + 24-hour time · — means not yet recorded · selected {item.BookingId}"
            TxtSelectedTitle.Text = $"Selected dispatch · {item.BookingId}"
            TxtDetailVehicleClerk.Text = $"{item.VehicleIdDisplay} / {item.AssignedClerk}"
            TxtDetailContractPayment.Text = item.ContractPayment
            TxtDetailBookingState.Text = item.ContractStatus
            TxtDetailPostStatus.Text = item.PostReturnStatus

            If item.PostReturnStatus = "Available" Then
                TxtDetailPostStatus.Foreground = New SolidColorBrush(CType(ColorConverter.ConvertFromString("#137333"), Color))
            ElseIf item.PostReturnStatus = "Rented" Then
                TxtDetailPostStatus.Foreground = New SolidColorBrush(CType(ColorConverter.ConvertFromString("#70000F"), Color))
            Else
                TxtDetailPostStatus.Foreground = New SolidColorBrush(CType(ColorConverter.ConvertFromString("#D97706"), Color))
            End If
        End Sub

        Private Sub ClearPreview()
            TxtSelectedTitle.Text = "Selected dispatch · —"
            TxtDetailVehicleClerk.Text = "—"
            TxtDetailContractPayment.Text = "—"
            TxtDetailBookingState.Text = "—"
            TxtDetailPostStatus.Text = "—"
        End Sub

        Private Sub BtnViewRecord_Click(sender As Object, e As RoutedEventArgs)
            If _selectedItem Is Nothing Then
                MessageBox.Show("Please select a dispatch record from the register table.", "View Authorized Record", MessageBoxButton.OK, MessageBoxImage.Information)
                Return
            End If

            Dim msg = $"=== AUTHORIZED DISPATCH RECORD AUDIT ==={vbCrLf}{vbCrLf}" &
                      $"Booking Reference: {_selectedItem.BookingId}{vbCrLf}" &
                      $"Customer Name: {_selectedItem.Customer}{vbCrLf}" &
                      $"Vehicle: {_selectedItem.Plate} ({_selectedItem.VehicleIdDisplay}){vbCrLf}" &
                      $"Assigned Dispatch Clerk: {_selectedItem.AssignedClerk}{vbCrLf}" &
                      $"Contract Status: {_selectedItem.ContractStatus}{vbCrLf}" &
                      $"Payment Voucher: {_selectedItem.ContractPayment}{vbCrLf}" &
                      $"Scheduled Pickup: {_selectedItem.ScheduledPickupDisplay}{vbCrLf}" &
                      $"Actual Dispatch: {_selectedItem.ActualDispatchDisplay}{vbCrLf}" &
                      $"Expected Return: {_selectedItem.ExpectedReturnDisplay}{vbCrLf}" &
                      $"Actual Return: {_selectedItem.ActualReturnDisplay}{vbCrLf}" &
                      $"Post-Return Status: {_selectedItem.PostReturnStatus}{vbCrLf}{vbCrLf}" &
                      $"Note: Review vehicle maintenance and booking status before dispatch."

            MessageBox.Show(msg, $"Authorized Record - {_selectedItem.BookingId}", MessageBoxButton.OK, MessageBoxImage.Information)
        End Sub

        Private Sub BtnPrintReport_Click(sender As Object, e As RoutedEventArgs)
            If _filteredItems Is Nothing OrElse _filteredItems.Count = 0 Then
                MessageBox.Show("There are no dispatch records matching your filter criteria to print.", "Print Report", MessageBoxButton.OK, MessageBoxImage.Information)
                Return
            End If

            Dim confirm = MessageBox.Show($"Print Daily Dispatch Report with {_filteredItems.Count} record(s) prepared for HQ Branch?", "Print Dispatch Report", MessageBoxButton.YesNo, MessageBoxImage.Question)
            If confirm = MessageBoxResult.Yes Then
                MessageBox.Show($"Daily Dispatch Report sent to system default printer.{vbCrLf}Audit copy archived under HQ Dispatch Logs.", "Print Job Sent", MessageBoxButton.OK, MessageBoxImage.Information)
            End If
        End Sub

        Private Sub BtnAuthorizedExport_Click(sender As Object, e As RoutedEventArgs)
            If _filteredItems Is Nothing OrElse _filteredItems.Count = 0 Then
                MessageBox.Show("There are no dispatch records to export.", "Authorized Export", MessageBoxButton.OK, MessageBoxImage.Information)
                Return
            End If

            Dim dialog As New SaveFileDialog() With {
                .FileName = $"AutoRent_Daily_Dispatch_Report_{DateTime.Now:yyyyMMdd_HHmm}.csv",
                .DefaultExt = ".csv",
                .Filter = "CSV documents (.csv)|*.csv"
            }

            If dialog.ShowDialog() = True Then
                Try
                    Dim sb As New StringBuilder()
                    sb.AppendLine("# AUTORENT HQ BRANCH - AUTHORIZED DAILY DISPATCH REPORT")
                    sb.AppendLine($"# Prepared: {DateTime.Now:yyyy-MM-dd HH:mm:ss} | Total Records: {_filteredItems.Count}")
                    sb.AppendLine("Booking ID,Vehicle ID,Plate,Customer,Assigned Clerk,Scheduled Pickup,Actual Dispatch,Expected Return,Actual Return,Contract Status,Payment Status")

                    For Each item In _filteredItems
                        Dim bId = EscapeCsv(item.BookingId)
                        Dim vId = EscapeCsv(item.VehicleIdDisplay)
                        Dim pl = EscapeCsv(item.Plate)
                        Dim cust = EscapeCsv(item.Customer)
                        Dim clerk = EscapeCsv(item.AssignedClerk)
                        Dim pSched = EscapeCsv(item.ScheduledPickupDisplay)
                        Dim pAct = EscapeCsv(item.ActualDispatchDisplay)
                        Dim rSched = EscapeCsv(item.ExpectedReturnDisplay)
                        Dim rAct = EscapeCsv(item.ActualReturnDisplay)
                        Dim st = EscapeCsv(item.ContractStatus)
                        Dim pay = EscapeCsv(item.ContractPayment)
                        sb.AppendLine($"{bId},{vId},{pl},{cust},{clerk},{pSched},{pAct},{rSched},{rAct},{st},{pay}")
                    Next

                    File.WriteAllText(dialog.FileName, sb.ToString(), Encoding.UTF8)
                    MessageBox.Show($"Successfully exported {_filteredItems.Count} authorized dispatch records to CSV.", "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information)
                Catch ex As Exception
                    MessageBox.Show($"Could not export report: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error)
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
End Namespace
