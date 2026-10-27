
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

        Private Sub CalendarPrevious_Click(
            sender As Object,
            e As RoutedEventArgs
        )

            _displayedCalendarMonth =
                _displayedCalendarMonth.AddMonths(-1)

            RefreshCalendarMonth()

        End Sub

        Private Sub CalendarNext_Click(
            sender As Object,
            e As RoutedEventArgs
        )

            _displayedCalendarMonth =
                _displayedCalendarMonth.AddMonths(1)

            RefreshCalendarMonth()

        End Sub

        ' ==========================================
        ' CALENDAR / TABLE MODES
        ' ==========================================

        Private Sub CalendarMode_Click(
            sender As Object,
            e As RoutedEventArgs
        )

            _calendarShowsTable = False

            RefreshCalendarMonth()

        End Sub

        Private Sub TableMode_Click(
            sender As Object,
            e As RoutedEventArgs
        )

            _calendarShowsTable = True

            RefreshCalendarMonth()

        End Sub

        ' ==========================================
        ' BUILD CALENDAR
        ' ==========================================

        Private Sub RefreshCalendarMonth()

            If CalendarDayCells Is Nothing Then Return

            TxtCalendarMonth.Text =
                _displayedCalendarMonth.ToString(
                    "MMMM yyyy",
                    CultureInfo.InvariantCulture
                )

            CalendarGridPanel.Visibility =
                If(
                    _calendarShowsTable,
                    Visibility.Collapsed,
                    Visibility.Visible
                )

            DgMonthEvents.Visibility =
                If(
                    _calendarShowsTable,
                    Visibility.Visible,
                    Visibility.Collapsed
                )

            BtnCalendarMode.Background =
                If(
                    _calendarShowsTable,
                    Brushes.White,
                    _burgundy
                )

            BtnCalendarMode.Foreground =
                If(
                    _calendarShowsTable,
                    Brushes.Black,
                    Brushes.White
                )

            BtnTableMode.Background =
                If(
                    _calendarShowsTable,
                    _burgundy,
                    Brushes.White
                )

            BtnTableMode.Foreground =
                If(
                    _calendarShowsTable,
                    Brushes.White,
                    Brushes.Black
                )

            CalendarDayCells.Children.Clear()

            Dim firstDay As DateTime =
                _displayedCalendarMonth

            Dim daysInMonth As Integer =
                DateTime.DaysInMonth(
                    firstDay.Year,
                    firstDay.Month
                )

            Dim offset As Integer =
                (CInt(firstDay.DayOfWeek) + 6) Mod 7

            Dim totalSlots As Integer =
                CInt(
                    Math.Ceiling(
                        (offset + daysInMonth) / 7.0
                    )
                ) * 7

            ' ======================================
            ' CREATE EACH CALENDAR DATE
            ' ======================================

            For cellIndex As Integer = 0 To totalSlots - 1

                Dim dayNumber As Integer =
                    cellIndex - offset + 1

                If dayNumber < 1 OrElse
                   dayNumber > daysInMonth Then

                    Dim emptyCell As New Border With {
                        .MinHeight = 78,
                        .Margin = New Thickness(2),
                        .Background = _lightBackground,
                        .BorderBrush = Brushes.Gainsboro,
                        .BorderThickness = New Thickness(1),
                        .CornerRadius = New CornerRadius(3)
                    }

                    CalendarDayCells.Children.Add(
                        emptyCell
                    )

                    Continue For

                End If

                Dim cellDate As New DateTime(
                    firstDay.Year,
                    firstDay.Month,
                    dayNumber
                )

                Dim isToday As Boolean =
                    cellDate.Date = DateTime.Today

                ' Get maintenance appointments for this date.
                Dim dailyRecords As New List(Of DataRow)

                If _records IsNot Nothing Then

                    For Each record As DataRow In _records.Rows

                        If IsDBNull(record("scheduled_date")) Then
                            Continue For
                        End If

                        Dim scheduledDate As DateTime =
                            Convert.ToDateTime(
                                record("scheduled_date")
                            )

                        If scheduledDate.Date = cellDate Then
                            dailyRecords.Add(record)
                        End If

                    Next

                End If

                ' ==================================
                ' DATE CELL CONTENT
                ' ==================================

                Dim contentPanel As New StackPanel()

                Dim dateLabel As New TextBlock With {
                    .Text = dayNumber.ToString(),
                    .FontSize = 11,
                    .FontWeight = If(
                        isToday,
                        FontWeights.Bold,
                        FontWeights.Normal
                    ),
                    .Foreground = If(
                        isToday,
                        _burgundy,
                        Brushes.DimGray
                    ),
                    .Margin = New Thickness(0, 0, 0, 4)
                }

                contentPanel.Children.Add(dateLabel)

                Dim tooltipLines As New List(Of String)

                Dim shown As Integer = 0

                For Each record As DataRow In dailyRecords

                    Dim vehicleName As String =
                        SafeField(record, "vehicle")

                    Dim serviceName As String =
                        SafeField(record, "service_type")

                    Dim statusValue As String =
                        SafeField(record, "status")

                    Dim appointmentTime As String =
                        Convert.ToDateTime(
                            record("scheduled_date")
                        ).ToString("hh:mm tt")

                    Dim fullDescription As String =
                        vehicleName & " - " &
                        serviceName

                    tooltipLines.Add(
                        appointmentTime & " | " &
                        fullDescription &
                        " (" & statusValue & ")"
                    )

                    If shown >= 2 Then
                        Continue For
                    End If

                    Dim eventColor As Brush = _burgundy

                    Select Case statusValue

                        Case "Completed"
                            eventColor = Brushes.ForestGreen

                        Case "Cancelled"
                            eventColor = Brushes.Gray

                        Case "Scheduled"
                            eventColor = Brushes.DarkOrange

                        Case "In Progress"
                            eventColor = _burgundy

                    End Select

                    Dim eventText As New TextBlock With {
                        .Text = fullDescription,
                        .FontSize = 9,
                        .FontWeight = FontWeights.SemiBold,
                        .Foreground = eventColor,
                        .TextTrimming = TextTrimming.CharacterEllipsis,
                        .Margin = New Thickness(0, 2, 0, 0),
                        .ToolTip = fullDescription &
                                   Environment.NewLine &
                                   "Time: " & appointmentTime &
                                   Environment.NewLine &
                                   "Status: " & statusValue
                    }

                    contentPanel.Children.Add(eventText)

                    shown += 1

                Next

                If dailyRecords.Count > 2 Then

                    contentPanel.Children.Add(
                        New TextBlock With {
                            .Text = "+" &
                                    (dailyRecords.Count - 2).ToString() &
                                    " more",
                            .FontSize = 9,
                            .Foreground = Brushes.Gray
                        }
                    )

                End If

                ' ==================================
                ' DATE BUTTON
                ' ==================================

                Dim cellBackground As Brush =
                    _lightBackground

                If dailyRecords.Count > 0 Then

                    cellBackground =
                        New SolidColorBrush(
                            System.Windows.Media.Color.FromRgb(
                                &HFC, &HF5, &HF6
                            )
                        )

                End If

                Dim dayButton As New Button With {
                    .Tag = cellDate,
                    .Content = contentPanel,
                    .MinHeight = 78,
                    .Padding = New Thickness(5),
                    .Margin = New Thickness(2),
                    .Background = cellBackground,
                    .BorderBrush = If(
                        isToday,
                        _burgundy,
                        Brushes.Gainsboro
                    ),
                    .BorderThickness = New Thickness(1),
                    .HorizontalContentAlignment =
                        HorizontalAlignment.Stretch,
                    .VerticalContentAlignment =
                        VerticalAlignment.Top,
                    .Cursor = Cursors.Hand,
                    .ToolTip = If(
                        tooltipLines.Count > 0,
                        String.Join(
                            Environment.NewLine,
                            tooltipLines
                        ),
                        "Click to schedule maintenance on this date"
                    )
                }

                AddHandler dayButton.Click,
                    AddressOf CalendarDay_Click

                CalendarDayCells.Children.Add(dayButton)

            Next

            ' Update monthly table as well.
            LoadMonthlyTable()

        End Sub

        ' ==========================================
        ' SAFE DATABASE FIELD
        ' ==========================================

        Private Function SafeField(
            record As DataRow,
            fieldName As String
        ) As String

            If record Is Nothing Then
                Return String.Empty
            End If

            If Not record.Table.Columns.Contains(fieldName) Then
                Return String.Empty
            End If

            If IsDBNull(record(fieldName)) Then
                Return String.Empty
            End If

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

            Dim monthTable As DataTable =
                _records.Clone()

            For Each record As DataRow In _records.Rows

                If IsDBNull(record("scheduled_date")) Then
                    Continue For
                End If

                Dim scheduledDate As DateTime =
                    Convert.ToDateTime(
                        record("scheduled_date")
                    )

                If scheduledDate.Year =
                   _displayedCalendarMonth.Year AndAlso
                   scheduledDate.Month =
                   _displayedCalendarMonth.Month Then

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

        Private Sub CalendarDay_Click(
            sender As Object,
            e As RoutedEventArgs
        )

            Dim selectedButton As Button =
                TryCast(sender, Button)

            If selectedButton Is Nothing Then Return

            If Not TypeOf selectedButton.Tag Is DateTime Then
                Return
            End If

            Dim selectedDate As DateTime =
                DirectCast(
                    selectedButton.Tag,
                    DateTime
                )

            DpDate.SelectedDate = selectedDate

            CalMaintenance.DisplayDate = selectedDate
            CalMaintenance.SelectedDate = selectedDate

        End Sub

        ' ==========================================
        ' SELECT MAINTENANCE APPOINTMENT
        ' ==========================================

        Private Sub DgMaintenance_SelectionChanged(
            sender As Object,
            e As SelectionChangedEventArgs
        )

            Dim record As DataRowView =
                TryCast(
                    DgMaintenance.SelectedItem,
                    DataRowView
                )

            If record Is Nothing Then Return

            _selectedId =
                ViewHelpers.IntOf(
                    record,
                    "maintenance_id"
                )


            SelectVehicleAndCategory(
                ViewHelpers.IntOf(
                    record,
                    "vehicle_id"
                )
            )


            CmbService.Text =
                ViewHelpers.Str(
                    record,
                    "service_type"
                )

            If Not IsDBNull(record("scheduled_date")) Then

                Dim scheduledDate As DateTime =
                    Convert.ToDateTime(
                        record("scheduled_date")
                    )

                DpDate.SelectedDate = scheduledDate

                CalMaintenance.DisplayDate =
                    scheduledDate

                CalMaintenance.SelectedDate =
                    scheduledDate

                _displayedCalendarMonth =
                    New DateTime(
                        scheduledDate.Year,
                        scheduledDate.Month,
                        1
                    )

                RefreshCalendarMonth()

            End If

            TxtCost.Text =
                ViewHelpers.DecOf(
                    record,
                    "cost"
                ).ToString("0.##")

            CmbStatus.SelectedItem =
                ViewHelpers.Str(
                    record,
                    "status"
                )

            TxtNotes.Text =
                ViewHelpers.Str(
                    record,
                    "notes"
                )

            BtnAdd.IsEnabled = False
            BtnSave.IsEnabled = True
            BtnDelete.IsEnabled = True

        End Sub

        ' ==========================================
        ' CLEAR FORM
        ' ==========================================

        Private Sub ClearForm()

            _selectedId = 0

            DgMaintenance.SelectedItem = Nothing


            ' Reset the category and vehicle.
            CmbCategory.SelectedIndex = -1

            CmbVehicle.ItemsSource = Nothing
            CmbVehicle.SelectedIndex = -1
            CmbVehicle.IsEnabled = False

            TxtCategoryVehicleCount.Text =
             "Select a category to see vehicles."

            CmbService.Text = ""

            DpDate.SelectedDate =
                DateTime.Today

            CmbStatus.SelectedItem =
                "Scheduled"

            TxtCost.Text = "0"
            TxtNotes.Clear()

            CalMaintenance.DisplayDate =
                DateTime.Today

            CalMaintenance.SelectedDate =
                DateTime.Today

            BtnAdd.IsEnabled = True
            BtnSave.IsEnabled = False
            BtnDelete.IsEnabled = False

        End Sub

        ' ==========================================
        ' VALIDATE FORM
        ' ==========================================

        Private Function ReadForm(
            ByRef vehicleId As Integer,
            ByRef cost As Decimal,
            ByRef statusValue As String
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
                CmbService.Text
            ) Then

                Return "Please choose a maintenance type."

            End If

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

            Dim problem As String =
                ReadForm(
                    vehicleId,
                    cost,
                    statusValue
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
                    CmbService.Text.Trim(),
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

            Dim problem As String =
                ReadForm(
                    vehicleId,
                    cost,
                    statusValue
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
                    CmbService.Text.Trim(),
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
                .Margin = New Thickness(0, 0, 8, 0),
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

            Dim exportButton As New Button With {
                .Content = "Export CSV",
                .Width = 110,
                .Height = 36,
                .Background = _accent,
                .Foreground = Brushes.White,
                .BorderBrush = _accent,
                .Cursor = Cursors.Hand
            }

            AddHandler exportButton.Click,
                AddressOf ExportHistory_Click

            Grid.SetColumn(exportButton, 2)
            footer.Children.Add(exportButton)

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

        ' ==========================================
        ' CSV ESCAPING
        ' ==========================================

        Private Function EscapeCsv(
            value As String
        ) As String

            Dim textValue As String =
                If(value, String.Empty)

            Dim firstText As String =
                textValue.TrimStart()

            If firstText.Length > 0 Then

                Dim firstChar As Char = firstText(0)

                If firstChar = "="c OrElse
                   firstChar = "+"c OrElse
                   firstChar = "-"c OrElse
                   firstChar = "@"c Then

                    textValue = "'" & textValue

                End If

            End If

            Return """" &
                textValue.Replace("""", """""") &
                """"

        End Function

        ' ==========================================
        ' EXPORT MAINTENANCE HISTORY
        ' ==========================================

        Private Sub ExportHistory_Click(
            sender As Object,
            e As RoutedEventArgs
        )

            If _visibleRows.Count = 0 Then

                MessageBox.Show(
                    Me,
                    "There are no records to export.",
                    "Maintenance History",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                )

                Return

            End If

            Dim picker As New SaveFileDialog With {
                .Title = "Export maintenance history",
                .FileName = "AutoRent_Maintenance_History.csv",
                .Filter = "CSV files (*.csv)|*.csv",
                .DefaultExt = ".csv",
                .AddExtension = True
            }

            If picker.ShowDialog() <> True Then Return

            Try

                Dim lines As New List(Of String)

                lines.Add(
                    "Maintenance ID,Vehicle,Service,Scheduled Date," &
                    "Cost,Status,Completed At,Notes"
                )

                For Each record As DataRowView In _visibleRows

                    Dim scheduledText As String = ""
                    Dim completedText As String = ""

                    If Not IsDBNull(record("scheduled_date")) Then

                        scheduledText =
                            Convert.ToDateTime(
                                record("scheduled_date")
                            ).ToString(
                                "yyyy-MM-dd HH:mm",
                                CultureInfo.InvariantCulture
                            )

                    End If

                    If Not IsDBNull(record("completed_at")) Then

                        completedText =
                            Convert.ToDateTime(
                                record("completed_at")
                            ).ToString(
                                "yyyy-MM-dd HH:mm",
                                CultureInfo.InvariantCulture
                            )

                    End If

                    Dim costText As String = ""

                    If Not IsDBNull(record("cost")) Then

                        costText =
                            Convert.ToDecimal(
                                record("cost")
                            ).ToString(
                                "0.00",
                                CultureInfo.InvariantCulture
                            )

                    End If

                    Dim values As String() = {
                        GetHistoryText(record, "maintenance_id"),
                        GetHistoryText(record, "vehicle"),
                        GetHistoryText(record, "service_type"),
                        scheduledText,
                        costText,
                        GetHistoryText(record, "status"),
                        completedText,
                        GetHistoryText(record, "notes")
                    }

                    Dim escaped As New List(Of String)

                    For Each fieldValue As String In values
                        escaped.Add(
                            EscapeCsv(fieldValue)
                        )
                    Next

                    lines.Add(
                        String.Join(",", escaped)
                    )

                Next

                File.WriteAllLines(
                    picker.FileName,
                    lines,
                    New UTF8Encoding(True)
                )

                MessageBox.Show(
                    Me,
                    $"{_visibleRows.Count} maintenance records exported successfully.",
                    "Export Complete",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                )

            Catch ex As Exception

                MessageBox.Show(
                    Me,
                    "Export failed: " & ex.Message,
                    "Maintenance History",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                )

            End Try

        End Sub

    End Class

End Namespace
