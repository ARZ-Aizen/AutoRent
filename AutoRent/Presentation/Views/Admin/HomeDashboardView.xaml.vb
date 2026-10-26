
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Linq
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Shapes
Imports AutoRent.Common.Utilities
Imports AutoRent.Data.Context
Imports AutoRent.Data.Repositories

Namespace Presentation.Views.Admin

    Partial Public Class HomeDashboardView
        Inherits UserControl

        Private _ready As Boolean = False
        Private _period As String = "Today"

        Private ReadOnly _chartValues As New List(Of Decimal)
        Private ReadOnly _chartLabels As New List(Of String)

        Private ReadOnly Accent As Brush =
            New SolidColorBrush(Color.FromRgb(&H7B, &H13, &H25))

        Private ReadOnly Ink As Brush =
            New SolidColorBrush(Color.FromRgb(&H30, &H2D, &H30))

        Private ReadOnly Muted As Brush =
            New SolidColorBrush(Color.FromRgb(&H8B, &H85, &H88))

        Private ReadOnly GridBrush As Brush =
            New SolidColorBrush(Color.FromRgb(&HEE, &HEA, &HEB))

        Private ReadOnly StatusColors As New Dictionary(Of String, Brush) From {
            {"Available", New SolidColorBrush(Color.FromRgb(&H49, &H92, &H70))},
            {"Reserved", New SolidColorBrush(Color.FromRgb(&HD8, &HA4, &H53))},
            {"Rented", Accent},
            {"Maintenance", New SolidColorBrush(Color.FromRgb(&H88, &H83, &H8B))},
            {"Retired", New SolidColorBrush(Color.FromRgb(&HD9, &HD6, &HD8))}
        }

        Public Sub New()
            InitializeComponent()
        End Sub

        ' ==========================================
        ' DASHBOARD LOAD
        ' ==========================================

        Private Sub HomeDashboardView_Loaded(
            sender As Object,
            e As RoutedEventArgs
        ) Handles Me.Loaded

            If _ready Then Return

            _ready = True
            RefreshDashboard()

        End Sub

        ' ==========================================
        ' REFRESH BUTTON
        ' ==========================================

        Private Sub BtnRefresh_Click(
            sender As Object,
            e As RoutedEventArgs
        ) Handles BtnRefresh.Click

            RefreshDashboard()

        End Sub

        ' ==========================================
        ' FORMATTING
        ' ==========================================

        Private Function FormatKpiCurrency(
            amount As Decimal
        ) As String

            If amount >= 1000000D Then
                Return "₱" &
                    (amount / 1000000D).ToString("0.##") &
                    "M"
            End If

            If amount >= 10000D Then
                Return "₱" &
                    (amount / 1000D).ToString("0.#") &
                    "k"
            End If

            Return "₱" & amount.ToString("N2")

        End Function

        Private Function GetCount(
            counts As Dictionary(Of String, Integer),
            key As String
        ) As Integer

            Dim count As Integer = 0

            counts.TryGetValue(key, count)

            Return count

        End Function

        ' ==========================================
        ' LOAD REAL DATABASE INFORMATION
        ' ==========================================

        Private Sub RefreshDashboard()

            Try

                TxtDate.Text =
                    DateTime.Now.ToString(
                        "dddd, MMMM d, yyyy"
                    ) &
                    "  •  Fleet and rental records"

                Dim stats =
                    ReportRepository.GetDashboardStats()

                Dim counts = stats.StatusCounts

                Dim total = stats.TotalVehicles

                Dim available =
                    GetCount(counts, "Available")

                Dim rented =
                    GetCount(counts, "Rented")

                Dim reserved =
                    GetCount(counts, "Reserved")

                Dim maintenance =
                    GetCount(counts, "Maintenance")

                Dim retired =
                    GetCount(counts, "Retired")

                Dim activeFleet =
                    Math.Max(0, total - retired)

                ' STATISTICS CARDS
                TxtTotal.Text =
                    total.ToString("N0")

                TxtAvailable.Text =
                    available.ToString("N0")

                TxtOut.Text =
                    rented.ToString("N0")

                TxtReservedNote.Text =
                    $"{reserved:N0} reserved separately"

                TxtMaint.Text =
                    maintenance.ToString("N0")


                ' Get the number of scheduled maintenance jobs.
                Dim scheduledJobs As Integer = Db.Count(
                    "SELECT COUNT(*) FROM [Maintenance] " &
                    "WHERE [status] = 'Scheduled'"
                )

                ' Display scheduled and overdue maintenance.
                TxtMaintNote.Text =
                    $"{scheduledJobs:N0} scheduled • " &
                    $"{stats.OverdueMaintenance:N0} overdue"


                TxtRevenue.Text =
                    FormatKpiCurrency(
                        stats.RevenueThisMonth
                    )

                TxtRevenue.ToolTip =
                    "₱" &
                    stats.RevenueThisMonth.ToString("N2")

                If activeFleet = 0 Then

                    TxtUtilization.Text = "0.0%"

                Else

                    TxtUtilization.Text =
                        (100.0 * rented / activeFleet).
                        ToString("N1") & "%"

                End If

                ' MAINTENANCE INFORMATION
                TxtCost.Text =
                    "Month's maintenance cost: ₱" &
                    stats.MaintenanceCostThisMonth.ToString("N2")

                TxtOverdue.Text =
                    $"Overdue maintenance: {stats.OverdueMaintenance:N0}"

                TxtUsers.Text =
                    $"Active user accounts: {stats.ActiveUsers:N0}"

                ' FLEET READINESS
                TxtFleetSummary.Text =
                    $"{total:N0} fleet vehicles • {retired:N0} retired"

                TxtFleetAvailable.Text =
                    $"Available  {available:N0}"

                TxtFleetRented.Text =
                    $"Rented  {rented:N0}"

                TxtFleetReserved.Text =
                    $"Reserved  {reserved:N0}"

                TxtFleetMaintenance.Text =
                    $"Maintenance  {maintenance:N0}"

                ' UPDATE STATUS DISTRIBUTION
                DrawStatusBar(counts, total)

                ' REAL CATEGORY COUNTS
                DgCategories.ItemsSource =
                    GetCategorySummary().DefaultView

                ' RECENT RESERVATIONS
                DgRecent.ItemsSource =
                    ReportRepository.
                    GetRecentReservations(8).
                    DefaultView

                ' UPCOMING MAINTENANCE
                DgUpcoming.ItemsSource =
                    MaintenanceRepository.
                    GetUpcoming(6).
                    DefaultView

                ' REVENUE CHART
                LoadRevenuePeriod()

            Catch ex As Exception

                AlertUtil.ShowError(
                    "Could not load dashboard: " &
                    ex.Message,
                    "Dashboard",
                    ViewHelpers.Owner(Me)
                )

            End Try

        End Sub

        ' ==========================================
        ' FLEET STATUS BAR
        ' ==========================================

        Private Sub DrawStatusBar(
            counts As Dictionary(Of String, Integer),
            total As Integer
        )

            StatusBar.Children.Clear()
            StatusBar.ColumnDefinitions.Clear()

            If total <= 0 Then Return

            For Each status As String In VehicleRepository.Statuses

                Dim count =
                    GetCount(counts, status)

                If count <= 0 Then Continue For

                Dim columnIndex =
                    StatusBar.ColumnDefinitions.Count

                StatusBar.ColumnDefinitions.Add(
                    New ColumnDefinition With {
                        .Width = New GridLength(
                            count,
                            GridUnitType.Star
                        )
                    }
                )

                Dim segment As New Border With {
                    .Background = If(
                        StatusColors.ContainsKey(status),
                        StatusColors(status),
                        Muted
                    ),
                    .ToolTip = $"{status}: {count:N0} vehicles"
                }

                Grid.SetColumn(
                    segment,
                    columnIndex
                )

                StatusBar.Children.Add(segment)

            Next

        End Sub

        ' ==========================================
        ' VEHICLE CATEGORY BREAKDOWN
        ' ==========================================

        Private Function GetCategorySummary() As DataTable

            Dim records = Db.Query(
                "SELECT [category], [status] " &
                "FROM [Vehicles] " &
                "ORDER BY [category]"
            )

            Dim grouped As New Dictionary(
                Of String, Integer()
            )(StringComparer.OrdinalIgnoreCase)

            For Each record As DataRow In records.Rows

                Dim category =
                    If(
                        IsDBNull(record("category")),
                        "Uncategorized",
                        Convert.ToString(record("category"))
                    )

                Dim status =
                    If(
                        IsDBNull(record("status")),
                        "",
                        Convert.ToString(record("status"))
                    )

                If String.IsNullOrWhiteSpace(category) Then
                    category = "Uncategorized"
                End If

                If Not grouped.ContainsKey(category) Then

                    grouped(category) =
                        New Integer() {0, 0}

                End If

                ' TOTAL VEHICLES IN CATEGORY
                grouped(category)(0) += 1

                ' RENTED VEHICLES IN CATEGORY
                If String.Equals(
                    status,
                    "Rented",
                    StringComparison.OrdinalIgnoreCase
                ) Then

                    grouped(category)(1) += 1

                End If

            Next

            Dim result As New DataTable()

            result.Columns.Add(
                "category",
                GetType(String)
            )

            result.Columns.Add(
                "total",
                GetType(Integer)
            )

            result.Columns.Add(
                "rented",
                GetType(Integer)
            )


            For Each categoryName As String In grouped.Keys.OrderBy(Function(key) key)

                result.Rows.Add(
                categoryName,
                grouped(categoryName)(0),
                grouped(categoryName)(1)
            )

            Next


            Return result

        End Function

        ' ==========================================
        ' REVENUE CHART FILTER BUTTONS
        ' ==========================================

        Private Sub BtnToday_Click(
            sender As Object,
            e As RoutedEventArgs
        ) Handles BtnToday.Click

            ChangePeriod("Today")

        End Sub

        Private Sub BtnWeek_Click(
            sender As Object,
            e As RoutedEventArgs
        ) Handles BtnWeek.Click

            ChangePeriod("Week")

        End Sub

        Private Sub BtnMonth_Click(
            sender As Object,
            e As RoutedEventArgs
        ) Handles BtnMonth.Click

            ChangePeriod("Month")

        End Sub

        Private Sub ChangePeriod(
            period As String
        )

            _period = period

            Try

                LoadRevenuePeriod()

            Catch ex As Exception

                AlertUtil.ShowError(
                    "Could not update revenue chart: " &
                    ex.Message,
                    "Dashboard",
                    ViewHelpers.Owner(Me)
                )

            End Try

        End Sub

        ' ==========================================
        ' ACTIVE FILTER BUTTON STYLES
        ' ==========================================

        Private Sub UpdateFilterStyles()

            For Each button As Button In New Button() {
                BtnToday,
                BtnWeek,
                BtnMonth
            }

                Dim selected =
                    (button Is BtnToday AndAlso _period = "Today") OrElse
                    (button Is BtnWeek AndAlso _period = "Week") OrElse
                    (button Is BtnMonth AndAlso _period = "Month")

                button.Background =
                    If(selected, Accent, Brushes.White)

                button.Foreground =
                    If(selected, Brushes.White, Ink)

                button.BorderBrush =
                    If(selected, Accent, GridBrush)

            Next

        End Sub

        ' ==========================================
        ' LOAD REVENUE FROM ACCESS DATABASE
        ' ==========================================

        Private Sub LoadRevenuePeriod()

            Dim startDate As DateTime

            Dim endDate =
                DateTime.Today.AddDays(1)

            Dim bucketCount As Integer

            Select Case _period

                Case "Week"

                    startDate =
                        DateTime.Today.AddDays(-6)

                    bucketCount = 7

                    TxtRevenuePeriodLabel.Text =
                        "Last 7 days"

                Case "Month"

                    startDate =
                        DateTime.Today.AddDays(-29)

                    bucketCount = 30

                    TxtRevenuePeriodLabel.Text =
                        "Last 30 days"

                Case Else

                    startDate =
                        DateTime.Today

                    bucketCount = 24

                    TxtRevenuePeriodLabel.Text =
                        "Today"

            End Select

            Dim amounts(
                bucketCount - 1
            ) As Decimal

            Dim rows = Db.Query(
                "SELECT [return_date], [total] " &
                "FROM [Reservations] " &
                "WHERE [status] = 'Completed' " &
                "AND [return_date] >= ? " &
                "AND [return_date] < ?",
                Db.Dt(startDate),
                Db.Dt(endDate)
            )

            For Each record As DataRow In rows.Rows

                If IsDBNull(record("return_date")) OrElse
                   IsDBNull(record("total")) Then

                    Continue For

                End If

                Dim completedAt =
                    Convert.ToDateTime(
                        record("return_date")
                    )

                Dim index As Integer

                If _period = "Today" Then

                    index = CInt(
                        Math.Floor(
                            (completedAt - startDate).TotalHours
                        )
                    )

                Else

                    index = CInt(
                        Math.Floor(
                            (completedAt.Date - startDate).TotalDays
                        )
                    )

                End If

                If index >= 0 AndAlso
                   index < bucketCount Then

                    amounts(index) +=
                        Convert.ToDecimal(
                            record("total")
                        )

                End If

            Next

            _chartValues.Clear()
            _chartLabels.Clear()

            Dim cumulative As Decimal = 0D

            For i As Integer = 0 To bucketCount - 1

                cumulative += amounts(i)

                _chartValues.Add(cumulative)

                If _period = "Today" Then

                    _chartLabels.Add(
                        startDate.AddHours(i).
                        ToString(
                            "h tt",
                            CultureInfo.InvariantCulture
                        )
                    )

                Else

                    _chartLabels.Add(
                        startDate.AddDays(i).
                        ToString(
                            "MMM d",
                            CultureInfo.InvariantCulture
                        )
                    )

                End If

            Next

            TxtRevenuePeriodTotal.Text =
                "Completed rental revenue for period: ₱" &
                cumulative.ToString("N2")

            UpdateFilterStyles()
            DrawRevenueChart()

        End Sub

        ' ==========================================
        ' REDRAW CHART WHEN WINDOW RESIZES
        ' ==========================================

        Private Sub RevenueCanvas_SizeChanged(
            sender As Object,
            e As SizeChangedEventArgs
        ) Handles RevenueCanvas.SizeChanged

            DrawRevenueChart()

        End Sub

        ' ==========================================
        ' FORMAT CHART NUMBERS
        ' ==========================================

        Private Function MoneyTick(
            value As Double
        ) As String

            If value >= 1000000 Then

                Return "₱" &
                    (value / 1000000).ToString("0.#") &
                    "M"

            End If

            If value >= 1000 Then

                Return "₱" &
                    (value / 1000).ToString("0.#") &
                    "k"

            End If

            Return "₱" & value.ToString("0")

        End Function

        ' ==========================================
        ' ADD LABEL TO CHART
        ' ==========================================

        Private Sub PlaceLabel(
            target As Canvas,
            caption As String,
            x As Double,
            y As Double,
            Optional foreground As Brush = Nothing
        )

            Dim label As New TextBlock With {
                .Text = caption,
                .FontSize = 10,
                .Foreground = If(
                    foreground,
                    Muted
                )
            }

            Canvas.SetLeft(label, x)
            Canvas.SetTop(label, y)

            target.Children.Add(label)

        End Sub

        ' ==========================================
        ' DRAW REVENUE PERFORMANCE CHART
        ' ==========================================

        Private Sub DrawRevenueChart()

            If RevenueCanvas Is Nothing Then Return

            RevenueCanvas.Children.Clear()

            Dim chartWidth =
                RevenueCanvas.ActualWidth

            Dim chartHeight =
                RevenueCanvas.ActualHeight

            If chartWidth < 170 OrElse
               chartHeight < 90 OrElse
               _chartValues.Count = 0 Then

                Return

            End If

            Dim left = 62.0
            Dim right = chartWidth - 12.0
            Dim top = 12.0
            Dim bottom = chartHeight - 28.0

            Dim graphWidth = right - left
            Dim graphHeight = bottom - top

            Dim maxRevenue =
                CDbl(_chartValues.Max())

            Dim ceiling =
                If(
                    maxRevenue <= 0,
                    1.0,
                    maxRevenue * 1.12
                )

            ' ======================================
            ' HORIZONTAL GRIDLINES AND VALUE LABELS
            ' ======================================

            For tick As Integer = 0 To 4

                Dim fraction =
                    tick / 4.0

                Dim y =
                    bottom - fraction * graphHeight

                RevenueCanvas.Children.Add(
                    New Line With {
                        .X1 = left,
                        .X2 = right,
                        .Y1 = y,
                        .Y2 = y,
                        .Stroke = GridBrush,
                        .StrokeThickness = 1
                    }
                )

                PlaceLabel(
                    RevenueCanvas,
                    MoneyTick(ceiling * fraction),
                    0,
                    y - 7
                )

            Next

            ' ======================================
            ' DATE AND TIME LABELS
            ' ======================================

            Dim lastIndex =
                _chartValues.Count - 1

            For tick As Integer = 0 To 4

                Dim index = CInt(
                    Math.Floor(
                        lastIndex * tick / 4.0
                    )
                )

                Dim x =
                    left +
                    (graphWidth * index /
                    Math.Max(1, lastIndex))

                PlaceLabel(
                    RevenueCanvas,
                    _chartLabels(index),
                    Math.Min(
                        right - 31,
                        Math.Max(
                            left - 17,
                            x - 17
                        )
                    ),
                    bottom + 7
                )

            Next

            ' ======================================
            ' NO REVENUE AVAILABLE
            ' ======================================

            If maxRevenue <= 0 Then

                PlaceLabel(
                    RevenueCanvas,
                    "No completed rental revenue in this period",
                    left + 12,
                    top + graphHeight / 2,
                    Muted
                )

                Return

            End If

            ' ======================================
            ' BUILD REVENUE STEP LINE
            ' ======================================

            Dim points As New PointCollection()

            Dim previousY As Double =
                bottom -
                CDbl(_chartValues(0)) /
                ceiling * graphHeight

            points.Add(
                New Point(
                    left,
                    previousY
                )
            )

            For i As Integer = 1 To lastIndex

                Dim x =
                    left +
                    graphWidth * i /
                    Math.Max(1, lastIndex)

                Dim newY =
                    bottom -
                    CDbl(_chartValues(i)) /
                    ceiling * graphHeight

                points.Add(
                    New Point(
                        x,
                        previousY
                    )
                )

                points.Add(
                    New Point(
                        x,
                        newY
                    )
                )

                previousY = newY

            Next

            points.Add(
                New Point(
                    right,
                    previousY
                )
            )

            RevenueCanvas.Children.Add(
                New Polyline With {
                    .Points = points,
                    .Stroke = Accent,
                    .StrokeThickness = 2.4,
                    .StrokeLineJoin = PenLineJoin.Round,
                    .StrokeStartLineCap = PenLineCap.Round,
                    .StrokeEndLineCap = PenLineCap.Round
                }
            )

            ' ======================================
            ' REVENUE VALUE MARKERS
            ' ======================================

            For i As Integer = 0 To lastIndex

                If _chartValues(i) <= 0D Then
                    Continue For
                End If

                If i > 0 AndAlso
                   _chartValues(i) =
                   _chartValues(i - 1) Then

                    Continue For

                End If

                Dim x =
                    left +
                    graphWidth * i /
                    Math.Max(1, lastIndex)

                Dim y =
                    bottom -
                    CDbl(_chartValues(i)) /
                    ceiling * graphHeight

                Dim marker As New Ellipse With {
                    .Width = 9,
                    .Height = 9,
                    .Fill = Brushes.White,
                    .Stroke = Accent,
                    .StrokeThickness = 2,
                    .ToolTip =
                        _chartLabels(i) &
                        ": ₱" &
                        _chartValues(i).ToString("N2")
                }

                Canvas.SetLeft(
                    marker,
                    x - 4.5
                )

                Canvas.SetTop(
                    marker,
                    y - 4.5
                )

                RevenueCanvas.Children.Add(marker)

            Next

        End Sub

    End Class

End Namespace
