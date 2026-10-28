
Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Input
Imports System.Windows.Media
Imports Microsoft.Win32
Imports AutoRent.Common.Utilities
Imports AutoRent.Data.Repositories

Namespace Presentation.Views.Admin

    Partial Public Class MaintenanceSchedulesView
        Inherits UserControl

        Private _records As DataTable
        Private _vehiclePickList As DataTable
        Private _selectedId As Integer = 0
        Private _ready As Boolean = False

        Private _displayedCalendarMonth As DateTime =
            New DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)

        Private _selectedCalendarDate As DateTime? = DateTime.Today
        Private _isSyncingRecordSelection As Boolean = False
        Private _calendarShowsTable As Boolean = False

        Private ReadOnly _burgundy As Brush =
            New SolidColorBrush(
                System.Windows.Media.Color.FromRgb(&H7A, &H8, &H1F)
            )

        Private ReadOnly _lightBackground As Brush =
            New SolidColorBrush(
                System.Windows.Media.Color.FromRgb(&HF5, &HF4, &HF3)
            )

        Public Sub New()
            InitializeComponent()
            InputBehavior.Attach(TxtCost, "money", 15)
            TxtNotes.MaxLength = 2000
        End Sub

        ' ==========================================
        ' INITIALIZE MAINTENANCE SCHEDULER
        ' ==========================================

        Private Sub MaintenanceSchedulesView_Loaded(
            sender As Object,
            e As RoutedEventArgs
        ) Handles Me.Loaded

            If _ready Then Return

            _ready = True

            ' Render calendar even without database records.
            RefreshCalendarMonth()

            CmbService.ItemsSource = New String() {
                "Oil change",
                "Tire service",
                "Brake service",
                "Battery check",
                "Routine service",
                "Engine check",
                "Body repair",
                "Other"
            }

            CmbStatus.ItemsSource =
                MaintenanceRepository.Statuses

            ClearForm()

            ' Load vehicles independently.
            LoadVehicles()

            ' Load maintenance records.
            LoadRecords()

        End Sub

        Private Sub LoadServiceTypes()
            Dim services As New List(Of String) From {
                "Oil change",
                "Tire service",
                "Brake service",
                "Battery check",
                "Routine service",
                "Engine check",
                "Body repair",
                "10,000 km PMS & Synthetic Oil Change",
                "Battery Replacement (Motolite Gold 12V)",
                "Brake System Bleeding & Sensor Replacement",
                "Executive Sports Car PMS & Brake Inspection",
                "Front & Rear Brake Pad Replacement",
                "Headlight Assembly Replacement",
                "Tire Patch & Valve Stem Overhaul",
                "Tire Rotation & High-Speed Wheel Alignment",
                "Other"
            }

            If _records IsNot Nothing Then
                For Each r As DataRow In _records.Rows
                    Dim st = SafeField(r, "service_type").Trim()
                    If Not String.IsNullOrEmpty(st) AndAlso Not services.Contains(st, StringComparer.OrdinalIgnoreCase) Then
                        services.Insert(services.Count - 1, st)
                    End If
                Next
            End If

            CmbService.ItemsSource = services
        End Sub

        ' ==========================================
        ' LOAD VEHICLE DROPDOWN
        ' ==========================================


        Private Sub LoadVehicles()

            Try

                ' Load vehicles and their categories.
                _vehiclePickList = VehicleRepository.GetAll()

                ' Add the display column used by CmbVehicle.
                If Not _vehiclePickList.Columns.Contains("display") Then
                    _vehiclePickList.Columns.Add("display", GetType(String))
                End If

                Dim categories As New SortedSet(Of String)(
            StringComparer.OrdinalIgnoreCase
        )

                For Each vehicle As DataRow In _vehiclePickList.Rows

                    Dim plate As String =
                Convert.ToString(vehicle("plate"))

                    Dim makeName As String =
                Convert.ToString(vehicle("make"))

                    Dim modelName As String =
                Convert.ToString(vehicle("model"))

                    Dim categoryName As String =
                Convert.ToString(vehicle("category")).Trim()

                    Dim statusValue As String =
                Convert.ToString(vehicle("status"))

                    vehicle("display") =
                plate & " - " & (makeName & " " & modelName).Trim()

                    ' Retired vehicles cannot be scheduled.
                    If Not String.Equals(
                statusValue,
                "Retired",
                StringComparison.OrdinalIgnoreCase
            ) AndAlso
               Not String.IsNullOrWhiteSpace(categoryName) Then

                        categories.Add(categoryName)

                    End If

                Next

                ' Populate categories, but leave Vehicle empty.
                CmbCategory.ItemsSource = categories.ToList()
                CmbCategory.SelectedIndex = -1
                CmbCategory.IsEnabled = (categories.Count > 0)

                CmbVehicle.ItemsSource = Nothing
                CmbVehicle.SelectedIndex = -1
                CmbVehicle.IsEnabled = False

                TxtCategoryVehicleCount.Text =
            If(
                categories.Count > 0,
                "Select a category to see vehicles.",
                "No eligible vehicles found. Check Fleet Management."
            )

                If categories.Count = 0 Then

                    AlertUtil.ShowWarning(
                "No eligible vehicles were found. " &
                "Please check Fleet Management.",
                "Maintenance Scheduler",
                ViewHelpers.Owner(Me)
            )

                End If

            Catch ex As Exception

                AlertUtil.ShowError(
            "Could not load vehicles: " & ex.Message,
            "Maintenance Scheduler",
            ViewHelpers.Owner(Me)
        )

            End Try

        End Sub

        ' ==========================================
        ' FILTER VEHICLES BY SELECTED CATEGORY
        ' ==========================================

        Private Sub CmbCategory_SelectionChanged(
    sender As Object,
    e As SelectionChangedEventArgs
)

            If CmbVehicle Is Nothing OrElse
       TxtCategoryVehicleCount Is Nothing Then
                Return
            End If

            ' Clear the previous vehicle selection.
            CmbVehicle.ItemsSource = Nothing
            CmbVehicle.SelectedIndex = -1
            CmbVehicle.IsEnabled = False

            Dim selectedCategory As String =
        TryCast(CmbCategory.SelectedItem, String)

            If _vehiclePickList Is Nothing OrElse
       String.IsNullOrWhiteSpace(selectedCategory) Then

                TxtCategoryVehicleCount.Text =
            "Select a category to see vehicles."

                Return

            End If

            Dim filteredVehicles As DataTable =
        _vehiclePickList.Clone()

            For Each vehicle As DataRow In _vehiclePickList.Rows

                Dim categoryName As String =
            Convert.ToString(vehicle("category"))

                Dim statusValue As String =
            Convert.ToString(vehicle("status"))

                If String.Equals(
            categoryName,
            selectedCategory,
            StringComparison.OrdinalIgnoreCase
        ) AndAlso
           Not String.Equals(
               statusValue,
               "Retired",
               StringComparison.OrdinalIgnoreCase
           ) Then

                    filteredVehicles.ImportRow(vehicle)

                End If

            Next

            CmbVehicle.ItemsSource =
        filteredVehicles.DefaultView

            CmbVehicle.IsEnabled =
        (filteredVehicles.Rows.Count > 0)

            Dim vehicleCount As Integer =
        filteredVehicles.Rows.Count

            If vehicleCount = 0 Then

                TxtCategoryVehicleCount.Text =
            "No eligible vehicles in this category."

            ElseIf vehicleCount = 1 Then

                TxtCategoryVehicleCount.Text =
            "1 vehicle available in " & selectedCategory & "."

            Else

                TxtCategoryVehicleCount.Text =
            vehicleCount.ToString() &
            " vehicles available in " &
            selectedCategory & "."

            End If

        End Sub


        ' ==========================================
        ' RESTORE CATEGORY WHEN EDITING A SCHEDULE
        ' ==========================================

        Private Sub SelectVehicleAndCategory(vehicleId As Integer)

            If _vehiclePickList Is Nothing Then Return

            For Each vehicle As DataRow In _vehiclePickList.Rows

                If Convert.ToInt32(vehicle("vehicle_id")) = vehicleId Then

                    Dim categoryName As String =
                Convert.ToString(vehicle("category")).Trim()

                    For Each category As String In CmbCategory.Items

                        If String.Equals(
                    category,
                    categoryName,
                    StringComparison.OrdinalIgnoreCase
                ) Then

                            ' This automatically filters the vehicle list.
                            CmbCategory.SelectedItem = category

                            ' Select the vehicle from the filtered list.
                            CmbVehicle.SelectedValue = vehicleId

                            Return

                        End If

                    Next

                End If

            Next

        End Sub


        ' ==========================================
        ' LOAD MAINTENANCE RECORDS
        ' ==========================================

        Private Sub LoadRecords()

            Try

                _records = MaintenanceRepository.GetAll()

                If Not _records.Columns.Contains("display_id") Then

                    _records.Columns.Add(
                        "display_id",
                        GetType(String)
                    )

                End If

                If Not _records.Columns.Contains("due_label") Then

                    _records.Columns.Add(
                        "due_label",
                        GetType(String)
                    )

                End If

                For Each record As DataRow In _records.Rows

                    Dim recordId As Integer =
                        Convert.ToInt32(
                            record("maintenance_id")
                        )

                    record("display_id") =
                        "MT-" & recordId.ToString("0000")

                    record("due_label") =
                        GetDueLabel(record)

                Next

                LoadServiceTypes()
                LoadUpcomingRecords()
                UpdateStatistics()
                UpdateSummary()
                RefreshCalendarMonth()

            Catch ex As Exception

                AlertUtil.ShowError(
                    "Could not load maintenance records: " &
                    ex.Message,
                    "Maintenance Scheduler",
                    ViewHelpers.Owner(Me)
                )

                ' Still display an empty calendar.
                RefreshCalendarMonth()

            End Try

        End Sub

        ' ==========================================
        ' MAINTENANCE DUE LABEL
        ' ==========================================

        Private Function GetDueLabel(
            record As DataRow
        ) As String

            If IsDBNull(record("scheduled_date")) Then
                Return String.Empty
            End If

            Dim statusValue As String =
                Convert.ToString(record("status"))

            If statusValue = "Completed" Then
                Return "Completed"
            End If

            If statusValue = "Cancelled" Then
                Return "Cancelled"
            End If

            Dim scheduledDate As DateTime =
                Convert.ToDateTime(
                    record("scheduled_date")
                ).Date

            Dim days As Integer =
                (scheduledDate - DateTime.Today).Days

            If days < 0 Then Return "Overdue"
            If days = 0 Then Return "Today"
            If days <= 7 Then Return "Within 7 days"

            Return "Upcoming"

        End Function

        ' ==========================================
        ' UPCOMING APPOINTMENTS TABLE
        ' ==========================================

        Private Sub LoadUpcomingRecords()

            If _records Is Nothing Then Return

            Dim upcoming As New DataView(_records)

            upcoming.RowFilter =
                "status IN ('Scheduled', 'In Progress')"

            upcoming.Sort = "scheduled_date ASC"

            DgMaintenance.ItemsSource = upcoming

            TxtUpcomingCount.Text =
                upcoming.Count.ToString("N0") &
                " open maintenance records"

        End Sub

        ' ==========================================
        ' STATISTICS CARDS
        ' ==========================================

        Private Sub UpdateStatistics()

            If _records Is Nothing Then Return

            Dim dueSoon As Integer = 0
            Dim overdue As Integer = 0
            Dim inProgress As Integer = 0
            Dim recentlyCompleted As Integer = 0

            For Each record As DataRow In _records.Rows

                Dim statusValue As String =
                    Convert.ToString(record("status"))

                If statusValue = "In Progress" Then
                    inProgress += 1
                End If

                If statusValue = "Completed" AndAlso
                   Not IsDBNull(record("completed_at")) Then

                    Dim completedAt As DateTime =
                        Convert.ToDateTime(
                            record("completed_at")
                        )

                    If completedAt >= DateTime.Today.AddDays(-6) AndAlso
                       completedAt < DateTime.Today.AddDays(1) Then

                        recentlyCompleted += 1

                    End If

                End If

                If statusValue = "Scheduled" OrElse
                   statusValue = "In Progress" Then

                    If Not IsDBNull(record("scheduled_date")) Then

                        Dim scheduledDate As DateTime =
                            Convert.ToDateTime(
                                record("scheduled_date")
                            ).Date

                        Dim days As Integer =
                            (scheduledDate - DateTime.Today).Days

                        If days >= 0 AndAlso days <= 7 Then
                            dueSoon += 1
                        End If

                        If days < 0 Then
                            overdue += 1
                        End If

                    End If

                End If

            Next

            TxtDueSoon.Text = dueSoon.ToString("N0")
            TxtOverdue.Text = overdue.ToString("N0")
            TxtInProgress.Text = inProgress.ToString("N0")
            TxtCompleted.Text = recentlyCompleted.ToString("N0")

        End Sub

        ' ==========================================
        ' SUMMARY TEXT
        ' ==========================================

        Private Sub UpdateSummary()

            If _records Is Nothing Then Return

            Dim total As Integer = _records.Rows.Count

            Dim openJobs As Integer =
                _records.Select(
                    "status IN ('Scheduled', 'In Progress')"
                ).Length

            Dim completed As Integer =
                _records.Select(
                    "status = 'Completed'"
                ).Length

            TxtSummary.Text =
                $"{total:N0} total records  •  " &
                $"{openJobs:N0} open  •  " &
                $"{completed:N0} completed"

        End Sub

        ' ==========================================
        ' CALENDAR NAVIGATION
        ' ==========================================

        Private Sub CalendarPrevious_Click(sender As Object, e As RoutedEventArgs)
            _displayedCalendarMonth = _displayedCalendarMonth.AddMonths(-1)
            RefreshCalendarMonth()
        End Sub

        Private Sub CalendarNext_Click(sender As Object, e As RoutedEventArgs)
            _displayedCalendarMonth = _displayedCalendarMonth.AddMonths(1)
            RefreshCalendarMonth()
        End Sub

        Private Sub CalendarToday_Click(sender As Object, e As RoutedEventArgs)
            _displayedCalendarMonth = New DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)
            _selectedCalendarDate = DateTime.Today
            ClearForm(DateTime.Today)
            RefreshCalendarMonth()
        End Sub

        ' ==========================================
        ' CALENDAR / TABLE MODES
        ' ==========================================

        Private Sub CalendarMode_Click(sender As Object, e As RoutedEventArgs)
            _calendarShowsTable = False
            RefreshCalendarMonth()
        End Sub

        Private Sub TableMode_Click(sender As Object, e As RoutedEventArgs)
            _calendarShowsTable = True
            RefreshCalendarMonth()
        End Sub

        ' ==========================================
        ' BUILD CALENDAR
        ' ==========================================

        Private Sub RefreshCalendarMonth()
            If CalendarDayCells Is Nothing Then Return

            TxtCalendarMonth.Text = _displayedCalendarMonth.ToString("MMMM yyyy", CultureInfo.InvariantCulture)

            CalendarGridPanel.Visibility = If(_calendarShowsTable, Visibility.Collapsed, Visibility.Visible)
            DgMonthEvents.Visibility = If(_calendarShowsTable, Visibility.Visible, Visibility.Collapsed)

            BtnCalendarMode.Background = If(_calendarShowsTable, Brushes.White, _burgundy)
            BtnCalendarMode.Foreground = If(_calendarShowsTable, Brushes.Black, Brushes.White)

            BtnTableMode.Background = If(_calendarShowsTable, _burgundy, Brushes.White)
            BtnTableMode.Foreground = If(_calendarShowsTable, Brushes.White, Brushes.Black)

            CalendarDayCells.Children.Clear()

            Dim dayStyle As Style = TryCast(Me.FindResource("CalendarDayButtonStyle"), Style)

            Dim firstDay As DateTime = _displayedCalendarMonth
            Dim daysInMonth As Integer = DateTime.DaysInMonth(firstDay.Year, firstDay.Month)
            Dim offset As Integer = (CInt(firstDay.DayOfWeek) + 6) Mod 7

            Dim totalSlots As Integer = CInt(Math.Ceiling((offset + daysInMonth) / 7.0)) * 7

            Dim prevMonth = firstDay.AddMonths(-1)
            Dim daysInPrev = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month)
            Dim nextMonth = firstDay.AddMonths(1)

            For cellIndex As Integer = 0 To totalSlots - 1
                If cellIndex < offset Then
                    ' Trailing day from previous month
                    Dim prevDayNum As Integer = daysInPrev - (offset - 1 - cellIndex)
                    Dim prevDate As New DateTime(prevMonth.Year, prevMonth.Month, prevDayNum)

                    Dim adjButton As New Button With {
                        .Tag = prevDate,
                        .Style = dayStyle,
                        .MinHeight = 52,
                        .Padding = New Thickness(4, 2, 4, 2),
                        .Margin = New Thickness(2),
                        .Background = New SolidColorBrush(Color.FromRgb(&HF9, &HF8, &HF7)),
                        .BorderBrush = New SolidColorBrush(Color.FromRgb(&HE8, &HE5, &HE3)),
                        .BorderThickness = New Thickness(1),
                        .ToolTip = $"View {prevDate:MMMM yyyy}"
                    }

                    Dim adjPanel As New StackPanel()
                    Dim adjLabel As New TextBlock With {
                        .Text = prevDayNum.ToString(),
                        .FontSize = 10,
                        .Foreground = New SolidColorBrush(Color.FromRgb(&HBF, &HB9, &HB7)),
                        .Margin = New Thickness(0, 0, 0, 2)
                    }
                    adjPanel.Children.Add(adjLabel)
                    adjButton.Content = adjPanel

                    AddHandler adjButton.Click, AddressOf CalendarAdjacentDay_Click
                    CalendarDayCells.Children.Add(adjButton)
                    Continue For
                End If

                Dim dayNumber As Integer = cellIndex - offset + 1
                If dayNumber > daysInMonth Then
                    ' Leading day from next month
                    Dim nextDayNum As Integer = dayNumber - daysInMonth
                    Dim nextDate As New DateTime(nextMonth.Year, nextMonth.Month, nextDayNum)

                    Dim adjButton As New Button With {
                        .Tag = nextDate,
                        .Style = dayStyle,
                        .MinHeight = 52,
                        .Padding = New Thickness(4, 2, 4, 2),
                        .Margin = New Thickness(2),
                        .Background = New SolidColorBrush(Color.FromRgb(&HF9, &HF8, &HF7)),
                        .BorderBrush = New SolidColorBrush(Color.FromRgb(&HE8, &HE5, &HE3)),
                        .BorderThickness = New Thickness(1),
                        .ToolTip = $"View {nextDate:MMMM yyyy}"
                    }

                    Dim adjPanel As New StackPanel()
                    Dim adjLabel As New TextBlock With {
                        .Text = nextDayNum.ToString(),
                        .FontSize = 10,
                        .Foreground = New SolidColorBrush(Color.FromRgb(&HBF, &HB9, &HB7)),
                        .Margin = New Thickness(0, 0, 0, 2)
                    }
                    adjPanel.Children.Add(adjLabel)
                    adjButton.Content = adjPanel

                    AddHandler adjButton.Click, AddressOf CalendarAdjacentDay_Click
                    CalendarDayCells.Children.Add(adjButton)
                    Continue For
                End If

                ' Day of current month
                Dim cellDate As New DateTime(firstDay.Year, firstDay.Month, dayNumber)
                Dim isToday As Boolean = (cellDate.Date = DateTime.Today)
                Dim isSelected As Boolean = (_selectedCalendarDate.HasValue AndAlso cellDate.Date = _selectedCalendarDate.Value.Date)

                Dim dailyRecords As New List(Of DataRow)()
                If _records IsNot Nothing Then
                    For Each record As DataRow In _records.Rows
                        If Not IsDBNull(record("scheduled_date")) Then
                            If Convert.ToDateTime(record("scheduled_date")).Date = cellDate Then
                                dailyRecords.Add(record)
                            End If
                        End If
                    Next
                End If

                Dim contentPanel As New StackPanel()

                Dim dateLabel As New TextBlock With {
                    .Text = dayNumber.ToString(),
                    .FontSize = 10.5,
                    .FontWeight = If(isToday OrElse isSelected, FontWeights.Bold, FontWeights.SemiBold),
                    .Foreground = If(isToday OrElse isSelected, _burgundy, New SolidColorBrush(Color.FromRgb(&H4A, &H44, &H46))),
                    .Margin = New Thickness(0, 0, 0, 2)
                }
                contentPanel.Children.Add(dateLabel)

                Dim tooltipLines As New List(Of String)()
                Dim shown As Integer = 0

                For Each record As DataRow In dailyRecords
                    Dim recId As Integer = ViewHelpers.IntOf(record, "maintenance_id")
                    Dim vehicleName As String = SafeField(record, "vehicle")
                    Dim serviceName As String = SafeField(record, "service_type")
                    Dim statusValue As String = SafeField(record, "status")
                    Dim appointmentTime As String = Convert.ToDateTime(record("scheduled_date")).ToString("hh:mm tt")
                    Dim fullDescription As String = vehicleName & " - " & serviceName

                    tooltipLines.Add($"{appointmentTime} | {fullDescription} ({statusValue})")

                    If shown >= 2 Then Continue For

                    Dim eventColor As Brush = _burgundy
                    Dim eventBg As Brush = New SolidColorBrush(Color.FromArgb(28, &H7A, &H8, &H1F))

                    Select Case statusValue
                        Case "Completed"
                            eventColor = New SolidColorBrush(Color.FromRgb(&H13, &H73, &H33))
                            eventBg = New SolidColorBrush(Color.FromArgb(28, &H13, &H73, &H33))
                        Case "Cancelled"
                            eventColor = New SolidColorBrush(Color.FromRgb(&H5F, &H63, &H68))
                            eventBg = New SolidColorBrush(Color.FromArgb(24, &H5F, &H63, &H68))
                        Case "Scheduled"
                            eventColor = New SolidColorBrush(Color.FromRgb(&HC2, &H64, &H00))
                            eventBg = New SolidColorBrush(Color.FromArgb(28, &HC2, &H64, &H00))
                        Case "In Progress"
                            eventColor = _burgundy
                            eventBg = New SolidColorBrush(Color.FromArgb(28, &H7A, &H8, &H1F))
                    End Select

                    Dim eventPill As New Border With {
                        .Background = eventBg,
                        .BorderBrush = eventColor,
                        .BorderThickness = New Thickness(1),
                        .CornerRadius = New CornerRadius(3),
                        .Padding = New Thickness(3, 1, 3, 1),
                        .Margin = New Thickness(0, 1.5, 0, 0),
                        .Cursor = Cursors.Hand,
                        .ToolTip = fullDescription & Environment.NewLine & "Time: " & appointmentTime & Environment.NewLine & "Status: " & statusValue & Environment.NewLine & "(Click to view / edit)"
                    }

                    Dim eventText As New TextBlock With {
                        .Text = fullDescription,
                        .FontSize = 9,
                        .FontWeight = FontWeights.SemiBold,
                        .Foreground = eventColor,
                        .TextTrimming = TextTrimming.CharacterEllipsis
                    }
                    eventPill.Child = eventText

                    AddHandler eventPill.MouseLeftButtonUp,
                        Sub(s As Object, ev As MouseButtonEventArgs)
                            ev.Handled = True
                            Dim targetRecord = FindRecordById(recId)
                            If targetRecord IsNot Nothing Then
                                LoadRecordIntoForm(targetRecord)
                            End If
                        End Sub

                    contentPanel.Children.Add(eventPill)
                    shown += 1
                Next

                If dailyRecords.Count > 2 Then
                    contentPanel.Children.Add(New TextBlock With {
                        .Text = $"+{dailyRecords.Count - 2} more",
                        .FontSize = 8.5,
                        .Foreground = Brushes.Gray,
                        .Margin = New Thickness(0, 1, 0, 0)
                    })
                End If

                Dim cellBg As Brush = Brushes.White
                Dim cellBorder As Brush = New SolidColorBrush(Color.FromRgb(&HE0, &HDB, &HD8))
                Dim borderThickness As New Thickness(1)

                If isSelected Then
                    cellBg = New SolidColorBrush(Color.FromRgb(&HF7, &HE9, &HEB))
                    cellBorder = _burgundy
                    borderThickness = New Thickness(2)
                ElseIf isToday Then
                    cellBorder = _burgundy
                    borderThickness = New Thickness(1.5)
                    If dailyRecords.Count > 0 Then
                        cellBg = New SolidColorBrush(Color.FromRgb(&HFC, &HF5, &HF6))
                    End If
                ElseIf dailyRecords.Count > 0 Then
                    cellBg = New SolidColorBrush(Color.FromRgb(&HFA, &HF6, &HF7))
                End If

                Dim dayButton As New Button With {
                    .Tag = cellDate,
                    .Style = dayStyle,
                    .Content = contentPanel,
                    .MinHeight = 52,
                    .Padding = New Thickness(4, 2, 4, 2),
                    .Margin = New Thickness(2),
                    .Background = cellBg,
                    .BorderBrush = cellBorder,
                    .BorderThickness = borderThickness,
                    .ToolTip = If(tooltipLines.Count > 0,
                                  String.Join(Environment.NewLine, tooltipLines),
                                  $"Click to schedule maintenance on {cellDate:dd MMM yyyy}")
                }

                AddHandler dayButton.Click, AddressOf CalendarDay_Click

                CalendarDayCells.Children.Add(dayButton)
            Next

            LoadMonthlyTable()
        End Sub

        ' ==========================================
        ' SAFE DATABASE FIELD
        ' ==========================================

        Private Function SafeField(record As DataRow, fieldName As String) As String
            If record Is Nothing Then Return String.Empty
            If Not record.Table.Columns.Contains(fieldName) Then Return String.Empty
            If IsDBNull(record(fieldName)) Then Return String.Empty
            Return Convert.ToString(record(fieldName))
        End Function

        ' ==========================================
        ' LOAD TABLE FOR SELECTED MONTH
        ' ==========================================

        Private Sub LoadMonthlyTable()
            If _records Is Nothing Then
                DgMonthEvents.ItemsSource = Nothing
                Return
            End If

            Dim monthTable As DataTable = _records.Clone()

            For Each record As DataRow In _records.Rows
                If IsDBNull(record("scheduled_date")) Then Continue For
                Dim scheduledDate As DateTime = Convert.ToDateTime(record("scheduled_date"))
                If scheduledDate.Year = _displayedCalendarMonth.Year AndAlso scheduledDate.Month = _displayedCalendarMonth.Month Then
                    monthTable.ImportRow(record)
                End If
            Next

            Dim monthView As New DataView(monthTable)
            monthView.Sort = "scheduled_date ASC"
            DgMonthEvents.ItemsSource = monthView
        End Sub

        ' ==========================================
        ' CLICK A CALENDAR DAY
        ' ==========================================

        Private Sub CalendarDay_Click(sender As Object, e As RoutedEventArgs)
            Dim selectedButton As Button = TryCast(sender, Button)
            If selectedButton Is Nothing Then Return
            If Not TypeOf selectedButton.Tag Is DateTime Then Return

            Dim selectedDate As DateTime = DirectCast(selectedButton.Tag, DateTime)
            _selectedCalendarDate = selectedDate.Date

            Dim dailyRecords As New List(Of DataRow)()
            If _records IsNot Nothing Then
                For Each record As DataRow In _records.Rows
                    If Not IsDBNull(record("scheduled_date")) Then
                        If Convert.ToDateTime(record("scheduled_date")).Date = selectedDate.Date Then
                            dailyRecords.Add(record)
                        End If
                    End If
                Next
            End If

            If dailyRecords.Count > 0 Then
                LoadRecordIntoForm(dailyRecords(0))
            Else
                ClearForm(selectedDate)
                RefreshCalendarMonth()
            End If
        End Sub

        Private Sub CalendarAdjacentDay_Click(sender As Object, e As RoutedEventArgs)
            Dim btn = TryCast(sender, Button)
            If btn Is Nothing OrElse Not TypeOf btn.Tag Is DateTime Then Return
            Dim targetDate = DirectCast(btn.Tag, DateTime)
            _displayedCalendarMonth = New DateTime(targetDate.Year, targetDate.Month, 1)
            _selectedCalendarDate = targetDate.Date

            Dim dailyRecords As New List(Of DataRow)()
            If _records IsNot Nothing Then
                For Each record As DataRow In _records.Rows
                    If Not IsDBNull(record("scheduled_date")) Then
                        If Convert.ToDateTime(record("scheduled_date")).Date = targetDate.Date Then
                            dailyRecords.Add(record)
                        End If
                    End If
                Next
            End If

            If dailyRecords.Count > 0 Then
                LoadRecordIntoForm(dailyRecords(0))
            Else
                ClearForm(targetDate)
                RefreshCalendarMonth()
            End If
        End Sub

        Private Function FindRecordById(recordId As Integer) As DataRow
            If _records Is Nothing Then Return Nothing
            For Each row As DataRow In _records.Rows
                If ViewHelpers.IntOf(row, "maintenance_id") = recordId Then
                    Return row
                End If
            Next
            Return Nothing
        End Function

        Private Sub SelectInUpcomingGrid(recordId As Integer)
            If DgMaintenance Is Nothing OrElse DgMaintenance.ItemsSource Is Nothing Then Return
            Dim target As DataRowView = Nothing
            For Each item As DataRowView In DgMaintenance.ItemsSource
                If ViewHelpers.IntOf(item, "maintenance_id") = recordId Then
                    target = item
                    Exit For
                End If
            Next
            DgMaintenance.SelectedItem = target
            If target IsNot Nothing Then
                DgMaintenance.ScrollIntoView(target)
            End If
        End Sub

        Private Sub LoadRecordIntoForm(record As DataRow)
            If record Is Nothing Then Return

            _isSyncingRecordSelection = True
            Try
                _selectedId = ViewHelpers.IntOf(record, "maintenance_id")

                SelectVehicleAndCategory(ViewHelpers.IntOf(record, "vehicle_id"))

                Dim targetService = ViewHelpers.Str(record, "service_type").Trim()
                Dim serviceList = TryCast(CmbService.ItemsSource, List(Of String))
                If serviceList IsNot Nothing AndAlso Not String.IsNullOrEmpty(targetService) AndAlso Not serviceList.Contains(targetService, StringComparer.OrdinalIgnoreCase) Then
                    serviceList.Insert(serviceList.Count - 1, targetService)
                    CmbService.ItemsSource = Nothing
                    CmbService.ItemsSource = serviceList
                End If
                CmbService.SelectedItem = targetService

                If Not IsDBNull(record("scheduled_date")) Then
                    Dim scheduledDate As DateTime = Convert.ToDateTime(record("scheduled_date"))
                    DpDate.SelectedDate = scheduledDate
                    _selectedCalendarDate = scheduledDate.Date
                    If _displayedCalendarMonth.Year <> scheduledDate.Year OrElse _displayedCalendarMonth.Month <> scheduledDate.Month Then
                        _displayedCalendarMonth = New DateTime(scheduledDate.Year, scheduledDate.Month, 1)
                    End If
                End If

                TxtCost.Text = ViewHelpers.DecOf(record, "cost").ToString("0.##")
                CmbStatus.SelectedItem = ViewHelpers.Str(record, "status")
                TxtNotes.Text = ViewHelpers.Str(record, "notes")

                BtnAdd.IsEnabled = False
                BtnSave.IsEnabled = True
                BtnDelete.IsEnabled = True

                SelectInUpcomingGrid(_selectedId)
            Finally
                _isSyncingRecordSelection = False
            End Try

            RefreshCalendarMonth()
        End Sub

        Private Sub DgMaintenance_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            If _isSyncingRecordSelection Then Return
            Dim record As DataRowView = TryCast(DgMaintenance.SelectedItem, DataRowView)
            If record Is Nothing Then Return
            LoadRecordIntoForm(record.Row)
        End Sub

        Private Sub DgMonthEvents_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            If _isSyncingRecordSelection Then Return
            Dim record As DataRowView = TryCast(DgMonthEvents.SelectedItem, DataRowView)
            If record Is Nothing Then Return
            LoadRecordIntoForm(record.Row)
        End Sub

        Private Sub DpDate_SelectedDateChanged(sender As Object, e As SelectionChangedEventArgs)
            If _isSyncingRecordSelection Then Return
            If DpDate.SelectedDate.HasValue Then
                Dim newDate = DpDate.SelectedDate.Value.Date
                _selectedCalendarDate = newDate
                If _displayedCalendarMonth.Year <> newDate.Year OrElse _displayedCalendarMonth.Month <> newDate.Month Then
                    _displayedCalendarMonth = New DateTime(newDate.Year, newDate.Month, 1)
                End If
                RefreshCalendarMonth()
            End If
        End Sub

        ' ==========================================
        ' CLEAR FORM
        ' ==========================================

        Private Sub ClearForm(Optional defaultDate As DateTime? = Nothing)
            _selectedId = 0
            _isSyncingRecordSelection = True
            Try
                DgMaintenance.SelectedItem = Nothing
                If DgMonthEvents IsNot Nothing AndAlso DgMonthEvents.ItemsSource IsNot Nothing Then
                    DgMonthEvents.SelectedItem = Nothing
                End If

                CmbCategory.SelectedIndex = -1
                CmbVehicle.ItemsSource = Nothing
                CmbVehicle.SelectedIndex = -1
                CmbVehicle.IsEnabled = False
                TxtCategoryVehicleCount.Text = "Select a category to see vehicles."

                CmbService.SelectedIndex = -1
                Dim chosenDate = If(defaultDate.HasValue, defaultDate.Value, DateTime.Today)
                DpDate.SelectedDate = chosenDate
                _selectedCalendarDate = chosenDate.Date

                CmbStatus.SelectedItem = "Scheduled"
                TxtCost.Text = "0"
                TxtNotes.Clear()

                BtnAdd.IsEnabled = True
                BtnSave.IsEnabled = False
                BtnDelete.IsEnabled = False
            Finally
                _isSyncingRecordSelection = False
            End Try
        End Sub

        ' ==========================================
        ' VALIDATE FORM
        ' ==========================================

        Private Function ReadForm(
            ByRef vehicleId As Integer,
            ByRef cost As Decimal,
            ByRef statusValue As String,
            ByRef serviceType As String
        ) As String


            ' Category must be selected first.
            If CmbCategory.SelectedItem Is Nothing Then
                Return "Please choose a vehicle category first."
            End If

            ' Vehicle must belong to the selected category.
            If CmbVehicle.SelectedValue Is Nothing Then
                Return "Please choose a vehicle from the selected category."
            End If


            vehicleId =
                Convert.ToInt32(
                    CmbVehicle.SelectedValue
                )

            If String.IsNullOrWhiteSpace(
                If(CmbService.SelectedItem IsNot Nothing, Convert.ToString(CmbService.SelectedItem), CmbService.Text)
            ) Then

                Return "Please choose a maintenance type from the dropdown."
            End If

            serviceType = If(CmbService.SelectedItem IsNot Nothing, Convert.ToString(CmbService.SelectedItem), CmbService.Text).Trim()

            If Not DpDate.SelectedDate.HasValue Then
                Return "Please select a scheduled date."
            End If

            If Not ViewHelpers.TryParseMoney(
                TxtCost.Text,
                cost
            ) Then

                Return "Estimated cost must be a number (0 or more)."

            End If

            statusValue =
                TryCast(
                    CmbStatus.SelectedItem,
                    String
                )

            If String.IsNullOrWhiteSpace(statusValue) Then
                Return "Please choose a maintenance status."
            End If

            Return Nothing

        End Function

        ' ==========================================
        ' ADD MAINTENANCE SCHEDULE
        ' ==========================================

        Private Sub BtnAdd_Click(
            sender As Object,
            e As RoutedEventArgs
        )

            Dim vehicleId As Integer
            Dim cost As Decimal
            Dim statusValue As String = Nothing
            Dim serviceType As String = Nothing

            Dim problem As String =
                ReadForm(
                    vehicleId,
                    cost,
                    statusValue,
                    serviceType
                )

            If problem IsNot Nothing Then

                AlertUtil.ShowWarning(
                    problem,
                    "Maintenance",
                    ViewHelpers.Owner(Me)
                )

                Return

            End If

            Try

                problem = MaintenanceRepository.Add(
                    vehicleId,
                    serviceType,
                    DpDate.SelectedDate.Value,
                    cost,
                    statusValue,
                    TxtNotes.Text.Trim()
                )

                If problem IsNot Nothing Then

                    AlertUtil.ShowWarning(
                        problem,
                        "Maintenance",
                        ViewHelpers.Owner(Me)
                    )

                    Return

                End If

                LoadRecords()
                ClearForm()

            Catch ex As Exception

                AlertUtil.ShowError(
                    "Could not schedule maintenance: " &
                    ex.Message,
                    "Maintenance",
                    ViewHelpers.Owner(Me)
                )

            End Try

        End Sub

        ' ==========================================
        ' SAVE CHANGES
        ' ==========================================

        Private Sub BtnSave_Click(
            sender As Object,
            e As RoutedEventArgs
        )

            If _selectedId = 0 Then Return

            Dim vehicleId As Integer
            Dim cost As Decimal
            Dim statusValue As String = Nothing
            Dim serviceType As String = Nothing

            Dim problem As String =
                ReadForm(
                    vehicleId,
                    cost,
                    statusValue,
                    serviceType
                )

            If problem IsNot Nothing Then

                AlertUtil.ShowWarning(
                    problem,
                    "Maintenance",
                    ViewHelpers.Owner(Me)
                )

                Return

            End If

            Try

                problem = MaintenanceRepository.Update(
                    _selectedId,
                    vehicleId,
                    serviceType,
                    DpDate.SelectedDate.Value,
                    cost,
                    statusValue,
                    TxtNotes.Text.Trim()
                )

                If problem IsNot Nothing Then

                    AlertUtil.ShowWarning(
                        problem,
                        "Maintenance",
                        ViewHelpers.Owner(Me)
                    )

                    Return

                End If

                LoadRecords()
                ClearForm()

            Catch ex As Exception

                AlertUtil.ShowError(
                    "Could not save maintenance: " &
                    ex.Message,
                    "Maintenance",
                    ViewHelpers.Owner(Me)
                )

            End Try

        End Sub

        ' ==========================================
        ' DELETE MAINTENANCE RECORD
        ' ==========================================

        Private Sub BtnDelete_Click(
            sender As Object,
            e As RoutedEventArgs
        )

            If _selectedId = 0 Then Return

            Dim confirmed As Boolean =
                AlertUtil.Confirm(
                    "Delete this maintenance record? " &
                    "This cannot be undone.",
                    "Delete Maintenance",
                    ViewHelpers.Owner(Me)
                )

            If Not confirmed Then Return

            Try

                Dim problem As String =
                    MaintenanceRepository.Delete(
                        _selectedId
                    )

                If problem IsNot Nothing Then

                    AlertUtil.ShowWarning(
                        problem,
                        "Maintenance",
                        ViewHelpers.Owner(Me)
                    )

                    Return

                End If

                LoadRecords()
                ClearForm()

            Catch ex As Exception

                AlertUtil.ShowError(
                    "Could not delete maintenance: " &
                    ex.Message,
                    "Maintenance",
                    ViewHelpers.Owner(Me)
                )

            End Try

        End Sub

        ' ==========================================
        ' CLEAR BUTTON
        ' ==========================================

        Private Sub BtnClear_Click(
            sender As Object,
            e As RoutedEventArgs
        )

            ClearForm()

        End Sub

        ' ==========================================
        ' OPEN SERVICE HISTORY
        ' ==========================================

        Private Sub BtnHistory_Click(
            sender As Object,
            e As RoutedEventArgs
        )

            Try

                Dim historyTable As DataTable =
                    MaintenanceRepository.GetAll()

                Dim historyWindow As New MaintenanceHistoryDialog(
                    historyTable
                )

                Dim ownerWindow As Window =
                    Window.GetWindow(Me)

                If ownerWindow IsNot Nothing Then
                    historyWindow.Owner = ownerWindow
                End If

                historyWindow.ShowDialog()

                LoadRecords()

            Catch ex As Exception

                AlertUtil.ShowError(
                    "Could not open maintenance history: " &
                    ex.Message,
                    "Maintenance History",
                    ViewHelpers.Owner(Me)
                )

            End Try

        End Sub

    End Class

    ' ============================================
    ' MAINTENANCE HISTORY WINDOW
    ' ============================================

    Public Class MaintenanceHistoryDialog
        Inherits Window

        Private ReadOnly _source As DataTable

        Private ReadOnly _visibleRows As New List(Of DataRowView)

        Private ReadOnly _search As New TextBox()
        Private ReadOnly _statusFilter As New ComboBox()
        Private ReadOnly _historyGrid As New DataGrid()

        Private ReadOnly _countLabel As New TextBlock()
        Private ReadOnly _filterInfo As New TextBlock()

        Private ReadOnly _accent As Brush =
            New SolidColorBrush(
                System.Windows.Media.Color.FromRgb(
                    &H7A, &H8, &H1F
                )
            )

        Private ReadOnly _lineBrush As Brush =
            New SolidColorBrush(
                System.Windows.Media.Color.FromRgb(
                    &HE7, &HE3, &HE3
                )
            )

        Public Sub New(
            historyTable As DataTable
        )

            _source = historyTable

            Title = "Maintenance History"

            Width = 1100
            Height = 670

            MinWidth = 850
            MinHeight = 500

            WindowStartupLocation =
                WindowStartupLocation.CenterOwner

            Background =
                New SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(
                        &HF7, &HF6, &HF5
                    )
                )

            FontFamily = New FontFamily("Segoe UI")

            BuildHistoryWindow()
            ApplyHistoryFilters()

        End Sub

        ' ==========================================
        ' CREATE HISTORY WINDOW
        ' ==========================================

        Private Sub BuildHistoryWindow()

            Dim root As New Grid()

            root.RowDefinitions.Add(
                New RowDefinition With {
                    .Height = GridLength.Auto
                }
            )

            root.RowDefinitions.Add(
                New RowDefinition With {
                    .Height = GridLength.Auto
                }
            )

            root.RowDefinitions.Add(
                New RowDefinition With {
                    .Height = New GridLength(
                        1,
                        GridUnitType.Star
                    )
                }
            )

            root.RowDefinitions.Add(
                New RowDefinition With {
                    .Height = GridLength.Auto
                }
            )

            Content = root

            ' HEADER
            Dim header As New StackPanel With {
                .Margin = New Thickness(24, 20, 24, 15)
            }

            header.Children.Add(
                New TextBlock With {
                    .Text = "Maintenance History",
                    .FontSize = 23,
                    .FontWeight = FontWeights.Bold,
                    .Foreground = Brushes.Black
                }
            )

            header.Children.Add(
                New TextBlock With {
                    .Text = "Search and review all vehicle maintenance records",
                    .FontSize = 12,
                    .Foreground = Brushes.Gray,
                    .Margin = New Thickness(0, 5, 0, 0)
                }
            )

            Grid.SetRow(header, 0)
            root.Children.Add(header)

            ' SEARCH PANEL
            Dim filterBorder As New Border With {
                .Background = Brushes.White,
                .BorderBrush = _lineBrush,
                .BorderThickness = New Thickness(1),
                .CornerRadius = New CornerRadius(7),
                .Padding = New Thickness(14),
                .Margin = New Thickness(24, 0, 24, 13)
            }

            Dim filters As New Grid()

            filters.ColumnDefinitions.Add(
                New ColumnDefinition With {
                    .Width = New GridLength(
                        2,
                        GridUnitType.Star
                    )
                }
            )

            filters.ColumnDefinitions.Add(
                New ColumnDefinition With {
                    .Width = New GridLength(
                        1,
                        GridUnitType.Star
                    )
                }
            )

            filters.ColumnDefinitions.Add(
                New ColumnDefinition With {
                    .Width = GridLength.Auto
                }
            )

            Dim searchPanel As New StackPanel With {
                .Margin = New Thickness(0, 0, 12, 0)
            }

            searchPanel.Children.Add(
                New TextBlock With {
                    .Text = "Search records",
                    .FontSize = 11,
                    .FontWeight = FontWeights.SemiBold,
                    .Margin = New Thickness(0, 0, 0, 5)
                }
            )

            _search.Height = 36
            _search.Padding = New Thickness(9, 6, 9, 5)
            _search.BorderBrush = _lineBrush

            searchPanel.Children.Add(_search)

            Grid.SetColumn(searchPanel, 0)
            filters.Children.Add(searchPanel)

            Dim statusPanel As New StackPanel With {
                .Margin = New Thickness(0, 0, 12, 0)
            }

            statusPanel.Children.Add(
                New TextBlock With {
                    .Text = "Status",
                    .FontSize = 11,
                    .FontWeight = FontWeights.SemiBold,
                    .Margin = New Thickness(0, 0, 0, 5)
                }
            )

            _statusFilter.Height = 36

            _statusFilter.Items.Add("All statuses")

            For Each statusValue As String In MaintenanceRepository.Statuses
                _statusFilter.Items.Add(statusValue)
            Next

            _statusFilter.SelectedIndex = 0

            statusPanel.Children.Add(_statusFilter)

            Grid.SetColumn(statusPanel, 1)
            filters.Children.Add(statusPanel)

            Dim clearButton As New Button With {
                .Content = "Clear filters",
                .Height = 36,
                .Padding = New Thickness(13, 0, 13, 0),
                .VerticalAlignment = VerticalAlignment.Bottom,
                .Background = Brushes.White,
                .BorderBrush = _lineBrush,
                .Cursor = Cursors.Hand
            }

            AddHandler clearButton.Click,
                AddressOf ClearHistoryFilters_Click

            Grid.SetColumn(clearButton, 2)
            filters.Children.Add(clearButton)

            filterBorder.Child = filters

            Grid.SetRow(filterBorder, 1)
            root.Children.Add(filterBorder)

            ' HISTORY TABLE
            Dim tableBorder As New Border With {
                .Background = Brushes.White,
                .BorderBrush = _lineBrush,
                .BorderThickness = New Thickness(1),
                .CornerRadius = New CornerRadius(7),
                .Padding = New Thickness(12),
                .Margin = New Thickness(24, 0, 24, 13)
            }

            Dim tableLayout As New DockPanel()

            Dim tableHeading As New StackPanel With {
                .Margin = New Thickness(0, 0, 0, 12)
            }

            tableHeading.Children.Add(
                New TextBlock With {
                    .Text = "Service register",
                    .FontSize = 16,
                    .FontWeight = FontWeights.Bold
                }
            )

            _filterInfo.FontSize = 11
            _filterInfo.Foreground = Brushes.Gray
            _filterInfo.Margin = New Thickness(0, 4, 0, 0)

            tableHeading.Children.Add(_filterInfo)

            DockPanel.SetDock(tableHeading, Dock.Top)
            tableLayout.Children.Add(tableHeading)

            _historyGrid.AutoGenerateColumns = False
            _historyGrid.IsReadOnly = True

            _historyGrid.CanUserAddRows = False
            _historyGrid.CanUserDeleteRows = False

            _historyGrid.HeadersVisibility =
                DataGridHeadersVisibility.Column

            _historyGrid.GridLinesVisibility =
                DataGridGridLinesVisibility.None

            _historyGrid.SelectionMode =
                DataGridSelectionMode.Single

            _historyGrid.SelectionUnit =
                DataGridSelectionUnit.FullRow

            _historyGrid.RowHeight = 40
            _historyGrid.ColumnHeaderHeight = 40

            _historyGrid.BorderThickness = New Thickness(0)

            _historyGrid.AlternatingRowBackground =
                New SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(
                        &HFC, &HFB, &HFB
                    )
                )

            AddHistoryColumn(
                "ID",
                "maintenance_id",
                65
            )

            AddHistoryColumn(
                "Vehicle",
                "vehicle",
                210
            )

            AddHistoryColumn(
                "Service",
                "service_type",
                130
            )

            AddHistoryColumn(
                "Scheduled",
                "scheduled_date",
                145,
                "dd MMM yyyy HH:mm"
            )

            AddHistoryColumn(
                "Cost (PHP)",
                "cost",
                115,
                "N2"
            )

            AddHistoryColumn(
                "Status",
                "status",
                115
            )

            AddHistoryColumn(
                "Completed",
                "completed_at",
                145,
                "dd MMM yyyy HH:mm"
            )

            AddHistoryColumn(
                "Notes",
                "notes",
                200
            )

            tableLayout.Children.Add(_historyGrid)

            tableBorder.Child = tableLayout

            Grid.SetRow(tableBorder, 2)
            root.Children.Add(tableBorder)

            ' FOOTER
            Dim footer As New Grid With {
                .Margin = New Thickness(24, 0, 24, 19)
            }

            footer.ColumnDefinitions.Add(
                New ColumnDefinition With {
                    .Width = New GridLength(
                        1,
                        GridUnitType.Star
                    )
                }
            )

            footer.ColumnDefinitions.Add(
                New ColumnDefinition With {
                    .Width = GridLength.Auto
                }
            )

            _countLabel.FontSize = 12
            _countLabel.Foreground = Brushes.Gray
            _countLabel.VerticalAlignment = VerticalAlignment.Center

            Grid.SetColumn(_countLabel, 0)
            footer.Children.Add(_countLabel)

            Dim closeButton As New Button With {
                .Content = "Close",
                .Width = 95,
                .Height = 36,
                .Margin = New Thickness(0),
                .Background = Brushes.White,
                .BorderBrush = _lineBrush,
                .Cursor = Cursors.Hand
            }

            AddHandler closeButton.Click,
                Sub(sender, args)
                    Me.Close()
                End Sub

            Grid.SetColumn(closeButton, 1)
            footer.Children.Add(closeButton)

            Grid.SetRow(footer, 3)
            root.Children.Add(footer)

            ' Events
            AddHandler _search.TextChanged,
                AddressOf HistorySearchChanged

            AddHandler _statusFilter.SelectionChanged,
                AddressOf HistoryStatusChanged

        End Sub

        ' ==========================================
        ' ADD HISTORY COLUMN
        ' ==========================================

        Private Sub AddHistoryColumn(
            columnTitle As String,
            fieldName As String,
            widthValue As Double,
            Optional formatText As String = Nothing
        )

            Dim columnBinding As New Binding(fieldName)

            If Not String.IsNullOrWhiteSpace(formatText) Then
                columnBinding.StringFormat = formatText
            End If

            Dim column As New DataGridTextColumn With {
                .Header = columnTitle,
                .Binding = columnBinding,
                .Width = New DataGridLength(widthValue)
            }

            _historyGrid.Columns.Add(column)

        End Sub

        ' ==========================================
        ' SEARCH AND FILTER EVENTS
        ' ==========================================

        Private Sub HistorySearchChanged(
            sender As Object,
            e As TextChangedEventArgs
        )

            ApplyHistoryFilters()

        End Sub

        Private Sub HistoryStatusChanged(
            sender As Object,
            e As SelectionChangedEventArgs
        )

            ApplyHistoryFilters()

        End Sub

        ' ==========================================
        ' FILTER HISTORY
        ' ==========================================

        Private Sub ApplyHistoryFilters()

            _visibleRows.Clear()

            Dim searchText As String =
                _search.Text.Trim()

            Dim selectedStatus As String =
                TryCast(
                    _statusFilter.SelectedItem,
                    String
                )

            Dim sourceView As New DataView(_source)

            sourceView.Sort =
                "scheduled_date DESC, maintenance_id DESC"

            For Each record As DataRowView In sourceView

                Dim statusValue As String =
                    GetHistoryText(
                        record,
                        "status"
                    )

                If Not String.IsNullOrWhiteSpace(
                    selectedStatus
                ) AndAlso
                   selectedStatus <> "All statuses" Then

                    If Not String.Equals(
                        statusValue,
                        selectedStatus,
                        StringComparison.OrdinalIgnoreCase
                    ) Then

                        Continue For

                    End If

                End If

                If searchText.Length > 0 Then

                    Dim searchable As String =
                        String.Join(" ", New String() {
                            GetHistoryText(record, "maintenance_id"),
                            GetHistoryText(record, "vehicle"),
                            GetHistoryText(record, "service_type"),
                            GetHistoryText(record, "status"),
                            GetHistoryText(record, "notes")
                        })

                    If searchable.IndexOf(
                        searchText,
                        StringComparison.OrdinalIgnoreCase
                    ) < 0 Then

                        Continue For

                    End If

                End If

                _visibleRows.Add(record)

            Next

            _historyGrid.ItemsSource = Nothing

            _historyGrid.ItemsSource =
                _visibleRows.ToArray()

            _countLabel.Text =
                $"Showing {_visibleRows.Count:N0} of {_source.Rows.Count:N0} records"

            _filterInfo.Text =
                $"{_visibleRows.Count:N0} matching service records"

        End Sub

        ' ==========================================
        ' SAFE HISTORY FIELD
        ' ==========================================

        Private Function GetHistoryText(
            record As DataRowView,
            fieldName As String
        ) As String

            If record Is Nothing Then
                Return String.Empty
            End If

            If Not record.Row.Table.Columns.Contains(fieldName) Then
                Return String.Empty
            End If

            If IsDBNull(record(fieldName)) Then
                Return String.Empty
            End If

            Return Convert.ToString(record(fieldName))

        End Function

        ' ==========================================
        ' CLEAR FILTERS
        ' ==========================================

        Private Sub ClearHistoryFilters_Click(
            sender As Object,
            e As RoutedEventArgs
        )

            _search.Clear()
            _statusFilter.SelectedIndex = 0

            ApplyHistoryFilters()

        End Sub
    End Class

End Namespace
