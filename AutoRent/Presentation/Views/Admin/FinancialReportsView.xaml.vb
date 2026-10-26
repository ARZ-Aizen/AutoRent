
Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Media
Imports System.Windows.Shapes
Imports AutoRent.Common.Utilities
Imports AutoRent.Data.Context
Imports AutoRent.Data.Repositories
Imports Microsoft.Win32

Namespace Presentation.Views.Admin

    Partial Public Class FinancialReportsView
        Inherits UserControl

        Private _summary As FinancialSummary
        Private _fromDate As DateTime
        Private _toDate As DateTime
        Private _period As String = "This Month"
        Private _ready As Boolean = False

        Private ReadOnly _points As New List(Of RevenuePoint)

        Private ReadOnly _wine As Brush =
            New SolidColorBrush(Color.FromRgb(&H76, &H9, &H1F))

        Private ReadOnly _muted As Brush =
            New SolidColorBrush(Color.FromRgb(&H88, &H81, &H84))

        Private ReadOnly _gridColor As Brush =
            New SolidColorBrush(Color.FromRgb(&HEA, &HE5, &HE5))

        Private Class RevenuePoint
            Public Property Label As String
            Public Property Amount As Decimal
        End Class

        Public Sub New()
            InitializeComponent()
        End Sub

        ' ==========================================
        ' LOAD FINANCIAL REPORTS
        ' ==========================================

        Private Sub FinancialReportsView_Loaded(
            sender As Object,
            e As RoutedEventArgs
        )

            If _ready Then Return

            _ready = True
            ChangePeriod("This Month")

        End Sub

        ' ==========================================
        ' DATE FILTER BUTTONS
        ' ==========================================

        Private Sub BtnToday_Click(
            sender As Object,
            e As RoutedEventArgs
        )

            ChangePeriod("Today")

        End Sub

        Private Sub BtnWeek_Click(
            sender As Object,
            e As RoutedEventArgs
        )

            ChangePeriod("This Week")

        End Sub

        Private Sub BtnMonth_Click(
            sender As Object,
            e As RoutedEventArgs
        )

            ChangePeriod("This Month")

        End Sub

        Private Sub ChangePeriod(
            periodName As String
        )

            _period = periodName

            Dim today As DateTime = DateTime.Today

            Select Case _period

                Case "Today"
                    _fromDate = today

                Case "This Week"
                    _fromDate =
                        today.AddDays(
                            -((CInt(today.DayOfWeek) + 6) Mod 7)
                        )

                Case Else
                    _fromDate =
                        New DateTime(
                            today.Year,
                            today.Month,
                            1
                        )

            End Select

            _toDate = today

            StylePeriodButton(
                BtnToday,
                _period = "Today"
            )

            StylePeriodButton(
                BtnWeek,
                _period = "This Week"
            )

            StylePeriodButton(
                BtnMonth,
                _period = "This Month"
            )

            LoadReport()

        End Sub

        Private Sub StylePeriodButton(
            button As Button,
            isActive As Boolean
        )

            button.Background =
                If(isActive, _wine, Brushes.White)

            button.Foreground =
                If(isActive, Brushes.White, Brushes.Black)

            button.BorderBrush =
                If(isActive, _wine, _gridColor)

        End Sub

        ' ==========================================
        ' GET REAL ACCESS DATABASE DATA
        ' ==========================================

        Private Sub LoadReport()

            Try

                _summary =
                    ReportRepository.GetSummary(
                        _fromDate,
                        _toDate
                    )

                TxtRange.Text =
                    If(
                        _period = "Today",
                        _fromDate.ToString("MMM dd, yyyy"),
                        $"{_fromDate:MMM dd, yyyy} – {_toDate:MMM dd, yyyy}"
                    )

                TxtRevenue.Text =
                    FormatMoney(_summary.Revenue)

                TxtExpenses.Text =
                    FormatMoney(_summary.Expenses)

                TxtNet.Text =
                    FormatMoney(_summary.Net)

                TxtRentals.Text =
                    _summary.CompletedRentals.ToString("N0")

                TxtNet.Foreground =
                    If(
                        _summary.Net < 0D,
                        Brushes.Firebrick,
                        _wine
                    )

                DgCategory.ItemsSource =
                    _summary.ByCategory.DefaultView

                DgMonth.ItemsSource =
                    _summary.ByMonth.DefaultView

                TxtTrendCaption.Text =
                    If(
                        _period = "Today",
                        "Hourly revenue from completed rentals",
                        "Daily revenue from completed rentals"
                    )

                LoadRevenuePoints()
                DrawRevenueChart()

            Catch ex As Exception

                AlertUtil.ShowError(
                    "Could not load financial reports: " &
                    ex.Message,
                    "Financial Reports",
                    ViewHelpers.Owner(Me)
                )

            End Try

        End Sub

        Private Shared Function FormatMoney(
            amount As Decimal
        ) As String

            Return "₱" & amount.ToString("N2")

        End Function

        ' ==========================================
        ' GENERATE REVENUE CHART DATA
        ' ==========================================

        Private Sub LoadRevenuePoints()

            _points.Clear()

            Dim numBuckets As Integer =
                If(
                    _period = "Today",
                    24,
                    CInt(
                        (_toDate.Date - _fromDate.Date).TotalDays
                    ) + 1
                )

            Dim buckets(numBuckets - 1) As Decimal

            Dim rentalRows As DataTable =
                Db.Query(
                    "SELECT [return_date], [total] " &
                    "FROM [Reservations] " &
                    "WHERE [status] = 'Completed' " &
                    "AND [return_date] >= ? " &
                    "AND [return_date] < ?",
                    Db.Dt(_fromDate.Date),
                    Db.Dt(_toDate.Date.AddDays(1))
                )

            For Each record As DataRow In rentalRows.Rows

                If IsDBNull(record("return_date")) Then
                    Continue For
                End If

                Dim completedDate As DateTime =
                    Convert.ToDateTime(
                        record("return_date")
                    )

                Dim index As Integer =
                    If(
                        _period = "Today",
                        completedDate.Hour,
                        CInt(
                            (completedDate.Date - _fromDate.Date).
                            TotalDays
                        )
                    )

                If index < 0 OrElse
                   index >= numBuckets Then

                    Continue For

                End If

                buckets(index) +=
                    If(
                        IsDBNull(record("total")),
                        0D,
                        Convert.ToDecimal(record("total"))
                    )

            Next

            Dim highestAmount As Decimal = 0D
            Dim peakLabel As String = ""
            Dim total As Decimal = 0D

            For index As Integer = 0 To numBuckets - 1

                Dim labelText As String


                If _period = "Today" Then

                    ' Show hours: 1 AM, 2 AM, 3 AM
                    labelText = _fromDate.AddHours(index).ToString("h tt")

                ElseIf _period = "This Week" Then

                    ' Show weekdays: Mon, Tue, Wed
                    labelText = _fromDate.AddDays(index).ToString("ddd")

                Else

                    ' Show day numbers only: 01, 02, 03
                    labelText = _fromDate.AddDays(index).Day.ToString("00")

                End If


                _points.Add(
                    New RevenuePoint With {
                        .Label = labelText,
                        .Amount = buckets(index)
                    }
                )

                total += buckets(index)

                If buckets(index) > highestAmount Then
                    highestAmount = buckets(index)
                    peakLabel = labelText
                End If

            Next

            TxtTrendTotal.Text =
                "Period total: " & FormatMoney(total)

            TxtPeak.Text =
                If(
                    highestAmount > 0D,
                    "Highest: " & peakLabel &
                    " (" & FormatMoney(highestAmount) & ")",
                    "No completed rentals in this period"
                )

        End Sub

        ' ==========================================
        ' REDRAW CHART ON RESIZE
        ' ==========================================

        Private Sub GraphCanvas_SizeChanged(
            sender As Object,
            e As SizeChangedEventArgs
        )

            If _ready Then
                DrawRevenueChart()
            End If

        End Sub

        ' ==========================================
        ' FORMAT GRAPH AXIS VALUES
        ' ==========================================

        Private Shared Function AxisMoney(
            amount As Double
        ) As String

            If amount >= 1000000 Then

                Return "₱" &
                    (amount / 1000000).ToString("0.#") &
                    "m"

            End If

            If amount >= 1000 Then

                Return "₱" &
                    (amount / 1000).ToString("0.#") &
                    "k"

            End If

            Return "₱" & amount.ToString("0.##")

        End Function

        Private Sub ChartText(
            labelText As String,
            x As Double,
            y As Double,
            Optional fontSize As Double = 10
        )

            Dim item As New TextBlock With {
                .Text = labelText,
                .FontSize = fontSize,
                .Foreground = _muted
            }

            Canvas.SetLeft(item, x)
            Canvas.SetTop(item, y)

            GraphCanvas.Children.Add(item)

        End Sub

        ' ==========================================
        ' DARK-BURGUNDY REVENUE CHART
        ' ==========================================

        Private Sub DrawRevenueChart()

            If GraphCanvas Is Nothing Then Return

            GraphCanvas.Children.Clear()

            Dim graphWidth As Double =
                GraphCanvas.ActualWidth

            Dim graphHeight As Double =
                GraphCanvas.ActualHeight

            If graphWidth < 180 OrElse
               graphHeight < 100 OrElse
               _points.Count = 0 Then

                Return

            End If

            Dim left As Double = 57
            Dim right As Double = graphWidth - 9
            Dim top As Double = 16
            Dim bottom As Double = graphHeight - 29

            Dim innerWidth As Double = right - left
            Dim innerHeight As Double = bottom - top

            Dim maximum As Double = 0

            For Each item As RevenuePoint In _points

                maximum =
                    Math.Max(maximum, CDbl(item.Amount))

            Next

            ' Calculate readable vertical axis values.
            Dim upper As Double = 1

            If maximum > 0 Then

                Dim magnitude As Double =
                    Math.Pow(
                        10,
                        Math.Floor(Math.Log10(maximum / 4))
                    )

                Dim tickStep As Double =
                    Math.Ceiling(
                        (maximum / 4) / magnitude
                    ) * magnitude

                upper = tickStep * 4

            End If

            ' ======================================
            ' GRID LINES AND PESO LABELS
            ' ======================================

            For tick As Integer = 0 To 4

                Dim y As Double =
                    bottom -
                    (tick / 4.0) * innerHeight

                GraphCanvas.Children.Add(
                    New Line With {
                        .X1 = left,
                        .X2 = right,
                        .Y1 = y,
                        .Y2 = y,
                        .Stroke = _gridColor,
                        .StrokeThickness = 1
                    }
                )

                If maximum > 0 OrElse tick = 0 Then

                    ChartText(
                        AxisMoney(
                            If(
                                maximum > 0,
                                upper * tick / 4.0,
                                0
                            )
                        ),
                        0,
                        y - 7
                    )

                End If

            Next

            ' ======================================
            ' REVENUE BARS
            ' ======================================

            Dim slotWidth As Double =
                innerWidth / _points.Count

            Dim barWidth As Double =
                Math.Max(
                    4,
                    Math.Min(32, slotWidth * 0.64)
                )

            Dim labelEvery As Integer =
                If(
                    _points.Count > 18,
                    4,
                    If(_points.Count > 10, 2, 1)
                )

            For i As Integer = 0 To _points.Count - 1

                Dim centerX As Double =
                    left + slotWidth * (i + 0.5)

                Dim barHeight As Double =
                    If(
                        maximum = 0,
                        0,
                        CDbl(_points(i).Amount) /
                        upper * innerHeight
                    )

                If barHeight > 0 Then

                    Dim paint As New LinearGradientBrush(
                        Color.FromRgb(&H66, &H5, &H1A),
                        Color.FromRgb(&HB9, &H3C, &H59),
                        90
                    )

                    Dim bar As New Rectangle With {
                        .Width = barWidth,
                        .Height = Math.Max(2, barHeight),
                        .RadiusX = 3,
                        .RadiusY = 3,
                        .Fill = paint,
                        .ToolTip =
                            _points(i).Label & ": " &
                            FormatMoney(_points(i).Amount)
                    }

                    Canvas.SetLeft(
                        bar,
                        centerX - barWidth / 2
                    )

                    Canvas.SetTop(
                        bar,
                        bottom - Math.Max(2, barHeight)
                    )

                    GraphCanvas.Children.Add(bar)

                End If

                If i Mod labelEvery = 0 OrElse
                   i = _points.Count - 1 Then

                    ChartText(
                        _points(i).Label,
                        Math.Max(
                            left - 8,
                            centerX - 15
                        ),
                        bottom + 6,
                        9
                    )

                End If

            Next

            If maximum = 0 Then

                ChartText(
                    "No completed-rental revenue recorded for this period.",
                    left + 10,
                    top + innerHeight / 2,
                    12
                )

            End If

        End Sub

        ' ==========================================
        ' PRINT FINANCIAL REPORT
        ' ==========================================

        Private Sub BtnPrint_Click(
            sender As Object,
            e As RoutedEventArgs
        )

            If _summary Is Nothing Then Return

            Try

                Dim printer As New PrintDialog()

                If printer.ShowDialog() <> True Then
                    Return
                End If

                Dim document As FlowDocument =
                    CreatePrintReport()

                document.PageWidth =
                    If(
                        printer.PrintableAreaWidth > 0,
                        printer.PrintableAreaWidth,
                        760
                    )

                document.PageHeight =
                    If(
                        printer.PrintableAreaHeight > 0,
                        printer.PrintableAreaHeight,
                        1056
                    )

                document.PagePadding =
                    New Thickness(30)

                document.ColumnWidth =
                    Math.Max(
                        300,
                        document.PageWidth - 60
                    )

                Dim printable As IDocumentPaginatorSource =
                    document

                printer.PrintDocument(
                    printable.DocumentPaginator,
                    "AutoRent Financial Report"
                )

            Catch ex As Exception

                AlertUtil.ShowError(
                    "Printing failed: " & ex.Message,
                    "Financial Reports",
                    ViewHelpers.Owner(Me)
                )

            End Try

        End Sub

        ' ==========================================
        ' CREATE A PRINTER-FRIENDLY REPORT
        ' ==========================================

        Private Function CreatePrintReport() As FlowDocument

            Dim document As New FlowDocument With {
                .FontFamily = New FontFamily("Segoe UI"),
                .FontSize = 10,
                .Foreground = Brushes.Black
            }

            document.Blocks.Add(
                New Paragraph(
                    New Run("AutoRent — Financial Report")
                ) With {
                    .FontSize = 19,
                    .FontWeight = FontWeights.Bold,
                    .Foreground = _wine,
                    .Margin = New Thickness(0, 0, 0, 8)
                }
            )

            document.Blocks.Add(
                New Paragraph(
                    New Run("Period: " & TxtRange.Text)
                ) With {
                    .Margin = New Thickness(0, 0, 0, 5)
                }
            )

            document.Blocks.Add(
                New Paragraph(
                    New Run(
                        "Generated: " &
                        DateTime.Now.ToString(
                            "MMM dd, yyyy h:mm tt"
                        )
                    )
                ) With {
                    .Margin = New Thickness(0, 0, 0, 15),
                    .Foreground = _muted
                }
            )

            ' FINANCIAL SUMMARY
            AddReportTable(
                document,
                "Financial summary",
                New String() {
                    "Metric",
                    "Amount"
                },
                New List(Of String()) From {
                    New String() {
                        "Rental revenue",
                        FormatMoney(_summary.Revenue)
                    },
                    New String() {
                        "Completed maintenance costs",
                        FormatMoney(_summary.Expenses)
                    },
                    New String() {
                        "Net after maintenance",
                        FormatMoney(_summary.Net)
                    },
                    New String() {
                        "Completed rentals",
                        _summary.CompletedRentals.ToString("N0")
                    }
                }
            )

            ' CATEGORY SUMMARY
            Dim categoryRows As New List(Of String())

            For Each record As DataRow In _summary.ByCategory.Rows

                categoryRows.Add(
                    New String() {
                        Convert.ToString(record("category")),
                        Convert.ToString(record("rentals")),
                        FormatMoney(
                            Convert.ToDecimal(
                                record("revenue")
                            )
                        )
                    }
                )

            Next

            AddReportTable(
                document,
                "Revenue by category",
                New String() {
                    "Category",
                    "Rentals",
                    "Revenue"
                },
                categoryRows
            )

            ' REVENUE TREND
            Dim trendRows As New List(Of String())

            For Each point As RevenuePoint In _points

                trendRows.Add(
                    New String() {
                        point.Label,
                        FormatMoney(point.Amount)
                    }
                )

            Next

            AddReportTable(
                document,
                "Revenue trend (" & _period & ")",
                New String() {
                    "Hour / day",
                    "Revenue"
                },
                trendRows
            )

            ' MONTHLY SUMMARY
            Dim monthlyRows As New List(Of String())

            For Each record As DataRow In _summary.ByMonth.Rows

                monthlyRows.Add(
                    New String() {
                        Convert.ToString(record("month")),
                        Convert.ToString(record("rentals")),
                        FormatMoney(
                            Convert.ToDecimal(
                                record("revenue")
                            )
                        ),
                        FormatMoney(
                            Convert.ToDecimal(
                                record("expenses")
                            )
                        ),
                        FormatMoney(
                            Convert.ToDecimal(
                                record("net")
                            )
                        )
                    }
                )

            Next

            AddReportTable(
                document,
                "Monthly summary",
                New String() {
                    "Month",
                    "Rentals",
                    "Revenue",
                    "Maintenance",
                    "Net"
                },
                monthlyRows
            )

            document.Blocks.Add(
                New Paragraph(
                    New Run(
                        "Amounts are based on completed rentals " &
                        "(recorded return dates) and completed " &
                        "maintenance (completion dates). " &
                        "Other business costs are not included."
                    )
                ) With {
                    .Margin = New Thickness(0, 16, 0, 0),
                    .FontSize = 9,
                    .Foreground = _muted
                }
            )

            Return document

        End Function

        ' ==========================================
        ' PRINT REPORT TABLE
        ' ==========================================

        Private Sub AddReportTable(
            document As FlowDocument,
            titleText As String,
            headings As String(),
            rows As List(Of String())
        )

            document.Blocks.Add(
                New Paragraph(
                    New Run(titleText)
                ) With {
                    .FontSize = 13,
                    .FontWeight = FontWeights.Bold,
                    .Foreground = _wine,
                    .Margin = New Thickness(0, 12, 0, 5)
                }
            )

            Dim reportTable As New System.Windows.Documents.Table With {
                .CellSpacing = 0
            }

            For i As Integer = 0 To headings.Length - 1

                reportTable.Columns.Add(
                    New TableColumn()
                )

            Next

            Dim group As New TableRowGroup()

            reportTable.RowGroups.Add(group)

            AddPrintRow(
                group,
                headings,
                True
            )

            For Each values As String() In rows

                AddPrintRow(
                    group,
                    values,
                    False
                )

            Next

            If rows.Count = 0 Then

                Dim emptyValues(headings.Length - 1) As String

                emptyValues(0) =
                    "No records for this period"

                AddPrintRow(
                    group,
                    emptyValues,
                    False
                )

            End If

            document.Blocks.Add(reportTable)

        End Sub

        Private Sub AddPrintRow(
            group As TableRowGroup,
            values As String(),
            isHeader As Boolean
        )

            Dim reportRow As New System.Windows.Documents.TableRow()

            For Each value As String In values

                Dim cell As New TableCell(
                    New Paragraph(
                        New Run(If(value, ""))
                    ) With {
                        .Margin = New Thickness(0)
                    }
                ) With {
                    .Padding = New Thickness(6),
                    .BorderBrush = _gridColor,
                    .BorderThickness =
                        New Thickness(0, 0, 0, 0.5),
                    .Background =
                        If(
                            isHeader,
                            Brushes.Gainsboro,
                            Brushes.White
                        ),
                    .FontWeight =
                        If(
                            isHeader,
                            FontWeights.SemiBold,
                            FontWeights.Normal
                        )
                }

                reportRow.Cells.Add(cell)

            Next

            group.Rows.Add(reportRow)

        End Sub

        ' ==========================================
        ' EXPORT FINANCIAL REPORT TO CSV
        ' ==========================================

        Private Sub BtnExport_Click(
            sender As Object,
            e As RoutedEventArgs
        )

            If _summary Is Nothing Then Return

            Dim picker As New SaveFileDialog With {
                .Filter = "CSV files (*.csv)|*.csv",
                .FileName =
                    "AutoRent-financial-report-" &
                    DateTime.Today.ToString("yyyy-MM-dd") &
                    ".csv"
            }

            If picker.ShowDialog() <> True Then Return

            Try

                Dim sb As New StringBuilder()

                sb.AppendLine("AutoRent Financial Report")

                sb.AppendLine(
                    "Period," & Csv(TxtRange.Text)
                )

                sb.AppendLine(
                    "Rental Revenue," &
                    _summary.Revenue.ToString(
                        "0.00",
                        CultureInfo.InvariantCulture
                    )
                )

                sb.AppendLine(
                    "Maintenance Expenses," &
                    _summary.Expenses.ToString(
                        "0.00",
                        CultureInfo.InvariantCulture
                    )
                )

                sb.AppendLine(
                    "Net After Maintenance," &
                    _summary.Net.ToString(
                        "0.00",
                        CultureInfo.InvariantCulture
                    )
                )

                sb.AppendLine(
                    "Completed Rentals," &
                    _summary.CompletedRentals
                )

                sb.AppendLine()

                sb.AppendLine("Revenue by Category")
                sb.AppendLine("Category,Rentals,Revenue")

                For Each record As DataRow In _summary.ByCategory.Rows

                    sb.AppendLine(
                        Csv(
                            Convert.ToString(
                                record("category")
                            )
                        ) & "," &
                        record("rentals") & "," &
                        Convert.ToDecimal(
                            record("revenue")
                        ).ToString(
                            "0.00",
                            CultureInfo.InvariantCulture
                        )
                    )

                Next

                sb.AppendLine()

                sb.AppendLine("Revenue Trend")
                sb.AppendLine("Hour or Day,Revenue")

                For Each point As RevenuePoint In _points

                    sb.AppendLine(
                        Csv(point.Label) & "," &
                        point.Amount.ToString(
                            "0.00",
                            CultureInfo.InvariantCulture
                        )
                    )

                Next

                sb.AppendLine()

                sb.AppendLine("Monthly Summary")
                sb.AppendLine(
                    "Month,Rentals,Revenue,Maintenance,Net"
                )

                For Each record As DataRow In _summary.ByMonth.Rows

                    sb.AppendLine(
                        Csv(
                            Convert.ToString(
                                record("month")
                            )
                        ) & "," &
                        record("rentals") & "," &
                        Convert.ToDecimal(
                            record("revenue")
                        ).ToString(
                            "0.00",
                            CultureInfo.InvariantCulture
                        ) & "," &
                        Convert.ToDecimal(
                            record("expenses")
                        ).ToString(
                            "0.00",
                            CultureInfo.InvariantCulture
                        ) & "," &
                        Convert.ToDecimal(
                            record("net")
                        ).ToString(
                            "0.00",
                            CultureInfo.InvariantCulture
                        )
                    )

                Next

                File.WriteAllText(
                    picker.FileName,
                    sb.ToString(),
                    New UTF8Encoding(True)
                )

                AlertUtil.ShowSuccess(
                    "Financial report exported successfully.",
                    "Financial Reports",
                    ViewHelpers.Owner(Me)
                )

            Catch ex As Exception

                AlertUtil.ShowError(
                    "Could not export CSV: " & ex.Message,
                    "Financial Reports",
                    ViewHelpers.Owner(Me)
                )

            End Try

        End Sub

        Private Shared Function Csv(
            rawText As String
        ) As String

            Dim value As String =
                If(rawText, "")

            Dim trimmed As String =
                value.TrimStart()

            If trimmed.Length > 0 Then

                Dim firstChar As Char =
                    trimmed.Chars(0)

                If firstChar = "="c OrElse
                   firstChar = "+"c OrElse
                   firstChar = "-"c OrElse
                   firstChar = "@"c Then

                    value = "'" & value

                End If

            End If

            Return """" &
                   value.Replace("""", """""") &
                   """"

        End Function

    End Class

End Namespace
