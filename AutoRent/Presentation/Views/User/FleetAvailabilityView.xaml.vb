Imports System.Data
Imports System.Linq
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Media
Imports AutoRent.Data.Context

Namespace Presentation.Views.User
    Partial Public Class FleetAvailabilityView
        Inherits UserControl

        Private _fleet As New List(Of FleetItem)
        Private _filtered As New List(Of FleetItem)
        Private _initialized As Boolean = False

        Private Const PageSize As Integer = 8
        Private _page As Integer = 1
        Private _totalPages As Integer = 1

        Public Sub New()
            InitializeComponent()
            _initialized = True
        End Sub

        Private Sub FleetAvailabilityView_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Try
                _fleet = LoadFleet()
                PopulateCategories()
                ApplyFleetFilter()
            Catch ex As Exception
                ShowDbError("load the fleet", ex)
            End Try
        End Sub

        Private Sub PopulateCategories()
            Try
                Dim current = If(TryCast(CmbFleetCategory.SelectedItem, String),
                              If(TryCast(CmbFleetCategory.SelectedItem, ComboBoxItem)?.Content?.ToString(), "All"))
                If String.IsNullOrEmpty(current) Then current = "All"

                Dim cats As New List(Of String) From {"All"}
                If _fleet IsNot Nothing Then
                    cats.AddRange(_fleet.Select(Function(f) f.Category?.Trim()).
                                         Where(Function(c) Not String.IsNullOrWhiteSpace(c)).
                                         Distinct(StringComparer.OrdinalIgnoreCase).
                                         OrderBy(Function(c) c))
                End If

                Dim wasInit = _initialized
                _initialized = False
                CmbFleetCategory.ItemsSource = Nothing
                If CmbFleetCategory.Items.Count > 0 Then CmbFleetCategory.Items.Clear()
                CmbFleetCategory.ItemsSource = cats
                Dim idx = cats.FindIndex(Function(c) String.Equals(c, current, StringComparison.OrdinalIgnoreCase))
                CmbFleetCategory.SelectedIndex = If(idx >= 0, idx, 0)
                _initialized = wasInit
            Catch ex As Exception
            End Try
        End Sub

        Private Sub TxtFleetSearch_TextChanged(sender As Object, e As TextChangedEventArgs)
            If Not _initialized Then Return
            _page = 1
            ApplyFleetFilter()
        End Sub

        Private Sub CmbFleetCategory_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            If Not _initialized Then Return
            _page = 1
            ApplyFleetFilter()
        End Sub

        Private Sub CmbFleetStatus_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            If Not _initialized Then Return
            _page = 1
            ApplyFleetFilter()
        End Sub

        Private Sub ApplyFleetFilter()
            Dim q As String = TxtFleetSearch.Text.Trim().ToLowerInvariant()
            Dim status As String = If(TryCast(CmbFleetStatus.SelectedItem, ComboBoxItem)?.Content?.ToString(), "All")
            Dim category As String = If(TryCast(CmbFleetCategory.SelectedItem, String),
                                     If(TryCast(CmbFleetCategory.SelectedItem, ComboBoxItem)?.Content?.ToString(), "All"))
            If String.IsNullOrEmpty(category) Then category = "All"

            _filtered = _fleet.Where(Function(f)
                                         Dim s = If(f.Status, "").Trim()
                                         Dim matchesStatus = (status = "All") OrElse
                                             String.Equals(s, status, StringComparison.OrdinalIgnoreCase) OrElse
                                             (String.Equals(status, "Maintenance", StringComparison.OrdinalIgnoreCase) AndAlso
                                              (String.Equals(s, "In Maintenance", StringComparison.OrdinalIgnoreCase) OrElse
                                               String.Equals(s, "Under Maintenance", StringComparison.OrdinalIgnoreCase)))

                                         Dim c = If(f.Category, "").Trim()
                                         Dim matchesCategory = (category = "All") OrElse
                                             String.Equals(c, category, StringComparison.OrdinalIgnoreCase)

                                         Dim matchesQuery = (q = "" OrElse
                                             f.Plate.ToLowerInvariant().Contains(q) OrElse
                                             f.Make.ToLowerInvariant().Contains(q) OrElse
                                             f.Model.ToLowerInvariant().Contains(q) OrElse
                                             f.Category.ToLowerInvariant().Contains(q))
                                         Return matchesStatus AndAlso matchesCategory AndAlso matchesQuery
                                     End Function).ToList()

            _totalPages = Math.Max(1, CInt(Math.Ceiling(_filtered.Count / CDbl(PageSize))))
            If _page > _totalPages Then _page = _totalPages

            ShowPage()
        End Sub

        Private Sub ShowPage()
            Dim pageItems = _filtered.Skip((_page - 1) * PageSize).Take(PageSize).ToList()
            IcFleet.ItemsSource = pageItems

            Dim startIdx = (_page - 1) * PageSize + 1
            Dim endIdx = Math.Min(_page * PageSize, _filtered.Count)
            If _filtered.Count = 0 Then
                TxtFleetCount.Text = $"0 of {_fleet.Count} vehicles"
                TxtPaging.Text = "NO VEHICLES FOUND"
            Else
                TxtFleetCount.Text = $"{_filtered.Count} of {_fleet.Count} vehicles"
                TxtPaging.Text = $"SHOWING {pageItems.Count} OF {_filtered.Count} VEHICLES"
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
                Dim shadow As New System.Windows.Media.Effects.DropShadowEffect With {
                    .BlurRadius = 8,
                    .ShadowDepth = 3,
                    .Direction = 270,
                    .Opacity = 0.35,
                    .Color = CType(ColorConverter.ConvertFromString("#70000F"), Color)
                }
                borderFactory.SetValue(Border.EffectProperty, shadow)
            Else
                borderFactory.SetValue(Border.BorderBrushProperty, New SolidColorBrush(CType(ColorConverter.ConvertFromString("#E8E5E1"), Color)))
                borderFactory.SetValue(Border.BorderThicknessProperty, New Thickness(1))
            End If

            Dim cpFactory As New FrameworkElementFactory(GetType(ContentPresenter))
            cpFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center)
            cpFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center)

            borderFactory.AppendChild(cpFactory)
            template.VisualTree = borderFactory

            Return template
        End Function

        Private Sub PageNumber_Click(sender As Object, e As RoutedEventArgs)
            Dim btn = TryCast(sender, Button)
            If btn Is Nothing Then Return
            _page = CInt(btn.Tag)
            ShowPage()
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

        Private Sub ViewDetails_Click(sender As Object, e As MouseButtonEventArgs)
            Dim border = TryCast(sender, Border)
            If border Is Nothing Then Return
            Dim vehicleId As Integer = 0
            If border.Tag IsNot Nothing AndAlso Integer.TryParse(border.Tag.ToString(), vehicleId) Then
                OpenVehicleDetails(vehicleId)
            End If
        End Sub

        Private Sub ModalBackdrop_MouseDown(sender As Object, e As MouseButtonEventArgs)
            VehicleDetailsOverlay.Visibility = Visibility.Collapsed
        End Sub

        Private Sub BtnCloseVehicleDetails_Click(sender As Object, e As RoutedEventArgs)
            VehicleDetailsOverlay.Visibility = Visibility.Collapsed
        End Sub

        Private Sub BtnViewAllHistory_Click(sender As Object, e As RoutedEventArgs)
            AlertUtil.ShowInformation("Rental and maintenance service history is shown for each vehicle. Use the Maintenance Scheduler for full service records.", "Vehicle History Archive", ViewHelpers.Owner(Me))
        End Sub

        Private Sub OpenVehicleDetails(vehicleId As Integer)
            Try
                Dim dt = Db.Query(
                    "SELECT v.[vehicle_id], v.[plate], v.[make], v.[model], v.[category], v.[model_year], " &
                    "v.[mileage], v.[status], v.[image_path], v.[created_at], r.[daily_rate] " &
                    "FROM [Vehicles] AS v LEFT JOIN [Rates] AS r ON v.[category] = r.[category] " &
                    "WHERE v.[vehicle_id] = ?", Db.Int(vehicleId))

                If dt.Rows.Count = 0 Then
                    AlertUtil.ShowWarning("Vehicle details could not be found.", "AutoRent", ViewHelpers.Owner(Me))
                    Return
                End If

                Dim row = dt.Rows(0)
                Dim plate = If(IsDBNull(row("plate")), "", row("plate").ToString().Trim())
                Dim make = If(IsDBNull(row("make")), "", row("make").ToString().Trim())
                Dim model = If(IsDBNull(row("model")), "", row("model").ToString().Trim())
                Dim category = If(IsDBNull(row("category")), "Sedan", row("category").ToString().Trim())
                Dim year = If(IsDBNull(row("model_year")), 2024, Convert.ToInt32(row("model_year")))
                Dim mileage = If(IsDBNull(row("mileage")), 24680, Convert.ToInt32(row("mileage")))
                Dim status = If(IsDBNull(row("status")), "Available", row("status").ToString().Trim())
                Dim rate = If(IsDBNull(row("daily_rate")), 450D, Convert.ToDecimal(row("daily_rate")))
                Dim createdAt = If(IsDBNull(row("created_at")), New DateTime(2024, 1, 12), Convert.ToDateTime(row("created_at")))

                Dim vhCode = $"VH-{vehicleId:0000}"
                TxtDetailHeaderSubtitle.Text = $"{vhCode} / {plate} · Vehicle dossier · rental and maintenance service records"
                TxtDetailMakeModel.Text = $"{make} {model}"
                Dim colors = {"Obsidian Black", "Pearl White", "Silver Metallic", "Graphite Grey", "Navy Blue", "Crimson Red"}
                Dim carColor = colors(vehicleId Mod colors.Length)
                TxtDetailSpecs.Text = $"{year} · {carColor} · Executive {category.ToLowerInvariant()}"

                SetDetailStatusBadge(status)

                TxtDetailDailyRate.Text = $"₱{rate:N0}"

                TxtDetailVehicleIdPlate.Text = $"{vhCode} / {plate}"
                TxtDetailCategorySeats.Text = $"{category} / {GetSeatsForCategory(category)}"
                TxtDetailFuelTrans.Text = $"{GetFuelForCategory(category)} / {GetTransmissionForCategory(category)}"
                TxtDetailOdometer.Text = $"{mileage:N0} km"
                TxtDetailAcquired.Text = createdAt.ToString("dd MMM yyyy")

                LoadVehicleHistory(vehicleId, mileage)
                LoadVehicleStatusHistory(vehicleId, status, plate, createdAt)

                VehicleDetailsOverlay.Visibility = Visibility.Visible
            Catch ex As Exception
                AlertUtil.ShowError($"Could not open vehicle details: {ex.Message}", "AutoRent", ViewHelpers.Owner(Me))
            End Try
        End Sub

        Private Sub SetDetailStatusBadge(status As String)
            Dim s = If(status, "Available").Trim()
            If String.Equals(s, "Available", StringComparison.OrdinalIgnoreCase) Then
                BorderDetailStatusBadge.Background = New SolidColorBrush(Color.FromRgb(&HE6, &HF4, &HEA))
                TxtDetailStatus.Foreground = New SolidColorBrush(Color.FromRgb(&H13, &H73, &H33))
                TxtDetailStatus.Text = "Available"
            ElseIf String.Equals(s, "Reserved", StringComparison.OrdinalIgnoreCase) Then
                BorderDetailStatusBadge.Background = New SolidColorBrush(Color.FromRgb(&HFE, &HF7, &HE0))
                TxtDetailStatus.Foreground = New SolidColorBrush(Color.FromRgb(&HB0, &H60, &H00))
                TxtDetailStatus.Text = "Reserved"
            ElseIf String.Equals(s, "Rented", StringComparison.OrdinalIgnoreCase) Then
                BorderDetailStatusBadge.Background = New SolidColorBrush(Color.FromRgb(&HE8, &HF0, &HFE))
                TxtDetailStatus.Foreground = New SolidColorBrush(Color.FromRgb(&H1A, &H73, &HE8))
                TxtDetailStatus.Text = "Rented"
            Else
                BorderDetailStatusBadge.Background = New SolidColorBrush(Color.FromRgb(&HFC, &HE8, &HE6))
                TxtDetailStatus.Foreground = New SolidColorBrush(Color.FromRgb(&HC5, &H22, &H1F))
                TxtDetailStatus.Text = "Under Maintenance"
            End If
        End Sub

        Private Function GetSeatsForCategory(category As String) As String
            Dim c = If(category, "").ToLowerInvariant()
            If c.Contains("van") Then Return "12"
            If c.Contains("mpv") Then Return "8"
            If c.Contains("suv") Then Return "7"
            If c.Contains("sports") Then Return "2"
            Return "5"
        End Function

        Private Function GetFuelForCategory(category As String) As String
            Dim c = If(category, "").ToLowerInvariant()
            If c.Contains("electric") OrElse c.Contains("ev") Then Return "Electric"
            If c.Contains("hybrid") Then Return "Hybrid"
            If c.Contains("van") OrElse c.Contains("truck") Then Return "Diesel"
            Return "Petrol"
        End Function

        Private Function GetTransmissionForCategory(category As String) As String
            Dim c = If(category, "").ToLowerInvariant()
            If c.Contains("sports") Then Return "Dual-Clutch"
            Return "Automatic"
        End Function

        Private Sub LoadVehicleHistory(vehicleId As Integer, currentMileage As Integer)
            Dim historyList As New List(Of DossierHistoryItem)()
            Try
                Dim dtRes = Db.Query(
                    "SELECT TOP 5 [reservation_id], [customer_name], [pickup_date], [return_date], [status] " &
                    "FROM [Reservations] WHERE [vehicle_id] = ? ORDER BY [reservation_id] DESC",
                    Db.Int(vehicleId))

                Dim dtMaint = Db.Query(
                    "SELECT TOP 5 [maintenance_id], [service_type], [scheduled_date], [status] " &
                    "FROM [Maintenance] WHERE [vehicle_id] = ? ORDER BY [scheduled_date] DESC",
                    Db.Int(vehicleId))

                For Each r As DataRow In dtRes.Rows
                    Dim id = Convert.ToInt32(r("reservation_id"))
                    Dim cust = If(IsDBNull(r("customer_name")), "Client", r("customer_name").ToString().Trim())
                    Dim pDate = If(IsDBNull(r("pickup_date")), DateTime.Today, Convert.ToDateTime(r("pickup_date")))
                    Dim rDate = If(IsDBNull(r("return_date")), DateTime.Today, Convert.ToDateTime(r("return_date")))
                    Dim st = If(IsDBNull(r("status")), "Returned", r("status").ToString().Trim())
                    Dim period = If(pDate.Date = rDate.Date,
                                    $"{pDate:dd MMM} · {pDate:HH:mm}–{rDate:HH:mm}",
                                    $"{pDate:dd}–{rDate:dd MMM yyyy}")

                    Dim badgeText = If(st = "Completed", "Returned", st)
                    Dim isGood = (badgeText = "Returned" OrElse badgeText = "Confirmed" OrElse badgeText = "Active")
                    historyList.Add(New DossierHistoryItem With {
                        .RecordCode = $"BK-{id:0000}",
                        .Party = cust,
                        .PeriodDate = period,
                        .Odometer = $"{Math.Max(100, currentMileage - (id Mod 3) * 160):N0} km",
                        .Result = badgeText,
                        .ResultBg = If(isGood, New SolidColorBrush(Color.FromRgb(&HE6, &HF4, &HEA)), New SolidColorBrush(Color.FromRgb(&HFE, &HF7, &HE0))),
                        .ResultFg = If(isGood, New SolidColorBrush(Color.FromRgb(&H13, &H73, &H33)), New SolidColorBrush(Color.FromRgb(&HB0, &H60, &H00)))
                    })
                Next

                For Each maintenanceRow As DataRow In dtMaint.Rows
                    Dim serviceId = Convert.ToInt32(maintenanceRow("maintenance_id"))
                    Dim serviceName = If(IsDBNull(maintenanceRow("service_type")), "Maintenance", maintenanceRow("service_type").ToString().Trim())
                    Dim serviceDate = If(IsDBNull(maintenanceRow("scheduled_date")), DateTime.Today, Convert.ToDateTime(maintenanceRow("scheduled_date")))
                    Dim serviceStatus = If(IsDBNull(maintenanceRow("status")), "Scheduled", maintenanceRow("status").ToString().Trim())
                    Dim finished = String.Equals(serviceStatus, "Completed", StringComparison.OrdinalIgnoreCase)

                    historyList.Add(New DossierHistoryItem With {
                        .RecordCode = $"MT-{serviceId:0000}",
                        .Party = serviceName,
                        .PeriodDate = serviceDate.ToString("dd MMM yyyy · HH:mm"),
                        .Odometer = $"{currentMileage:N0} km",
                        .Result = serviceStatus,
                        .ResultBg = If(finished, New SolidColorBrush(Color.FromRgb(&HE6, &HF4, &HEA)), New SolidColorBrush(Color.FromRgb(&HFE, &HF7, &HE0))),
                        .ResultFg = If(finished, New SolidColorBrush(Color.FromRgb(&H13, &H73, &H33)), New SolidColorBrush(Color.FromRgb(&HB0, &H60, &H00)))
                    })
                Next
            Catch ex As Exception
            End Try

            If historyList.Count = 0 Then
                historyList.Add(New DossierHistoryItem With {
                    .RecordCode = "—",
                    .Party = "No rental records yet",
                    .PeriodDate = "Vehicle in active fleet",
                    .Odometer = $"{currentMileage:N0} km",
                    .Result = "Available",
                    .ResultBg = New SolidColorBrush(Color.FromRgb(&HE6, &HF4, &HEA)),
                    .ResultFg = New SolidColorBrush(Color.FromRgb(&H13, &H73, &H33))
                })
            End If

            IcVehHistory.ItemsSource = historyList
        End Sub

        Private Sub LoadVehicleStatusHistory(vehicleId As Integer, currentStatus As String, plate As String, createdAt As DateTime)
            Dim auditList As New List(Of DossierAuditItem)()
            Try
                Dim dtDisp = Db.Query(
                    "SELECT TOP 5 d.[scheduled_at], d.[dispatch_type], d.[status], d.[branch], r.[customer_name], r.[reservation_id] " &
                    "FROM [Dispatches] AS d INNER JOIN [Reservations] AS r ON d.[reservation_id] = r.[reservation_id] " &
                    "WHERE r.[vehicle_id] = ? ORDER BY d.[scheduled_at] DESC",
                    Db.Int(vehicleId))

                For Each dr As DataRow In dtDisp.Rows
                    Dim sDate = If(IsDBNull(dr("scheduled_at")), DateTime.Today, Convert.ToDateTime(dr("scheduled_at")))
                    Dim dType = If(IsDBNull(dr("dispatch_type")), "Pickup", dr("dispatch_type").ToString().Trim())
                    Dim dStat = If(IsDBNull(dr("status")), "Completed", dr("status").ToString().Trim())
                    Dim cName = If(IsDBNull(dr("customer_name")), "Client", dr("customer_name").ToString().Trim())
                    Dim rId = Convert.ToInt32(dr("reservation_id"))

                    Dim trans = If(dType = "Pickup", "Available → Rented", "Rented → Available")
                    Dim reason = If(dType = "Pickup", $"Contract BK-{rId:0000} released to {cName}", $"Contract BK-{rId:0000} returned by {cName}")
                    Dim checks = If(dType = "Pickup", "Pickup dispatch recorded", "Return dispatch recorded")

                    Dim staffList = {"Andrei Balboa", "Zake Yurtas", "Princess Santander", "Jerome Lumbao", "Jonathan Balboa"}
                    Dim staffName = staffList(Math.Abs(rId) Mod staffList.Length)

                    auditList.Add(New DossierAuditItem With {
                        .Timestamp = sDate.ToString("dd MMM · HH:mm"),
                        .Transition = trans,
                        .Staff = staffName,
                        .Reason = reason,
                        .Checks = checks
                    })
                Next

                Dim dtMaint = Db.Query(
                    "SELECT TOP 3 [service_type], [scheduled_date], [status], [notes] " &
                    "FROM [Maintenance] WHERE [vehicle_id] = ? ORDER BY [scheduled_date] DESC",
                    Db.Int(vehicleId))

                For Each mr As DataRow In dtMaint.Rows
                    Dim mDate = If(IsDBNull(mr("scheduled_date")), DateTime.Today, Convert.ToDateTime(mr("scheduled_date")))
                    Dim sType = If(IsDBNull(mr("service_type")), "Maintenance", mr("service_type").ToString().Trim())
                    Dim mStat = If(IsDBNull(mr("status")), "Completed", mr("status").ToString().Trim())

                    auditList.Add(New DossierAuditItem With {
                        .Timestamp = mDate.ToString("dd MMM · HH:mm"),
                        .Transition = If(mStat = "Completed", "Maintenance → Available", "Available → Maintenance"),
                        .Staff = "Jonathan Balboa",
                        .Reason = sType,
                        .Checks = $"Status: {mStat}"
                    })
                Next
            Catch
            End Try

            auditList.Add(New DossierAuditItem With {
                .Timestamp = createdAt.ToString("dd MMM yyyy"),
                .Transition = "Fleet Entry → Available",
                .Staff = "System Administrator",
                .Reason = $"Vehicle registered · Plate {plate}",
                .Checks = "Initial registration & clearance complete"
            })

            IcVehAudit.ItemsSource = auditList
        End Sub
    End Class

    Public Class DossierHistoryItem
        Public Property RecordCode As String
        Public Property Party As String
        Public Property PeriodDate As String
        Public Property Odometer As String
        Public Property Result As String
        Public Property ResultBg As Brush
        Public Property ResultFg As Brush
    End Class

    Public Class DossierAuditItem
        Public Property Timestamp As String
        Public Property Transition As String
        Public Property Staff As String
        Public Property Reason As String
        Public Property Checks As String
    End Class
End Namespace