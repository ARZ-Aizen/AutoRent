Imports System.Data
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports AutoRent.Common.Utilities
Imports AutoRent.Data.Repositories

Namespace Presentation.Views
    Partial Public Class FinancialReportsView
        Inherits UserControl

        Private _summary As FinancialSummary
        Private _from As DateTime
        Private _to As DateTime
        Private _ready As Boolean = False

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub FinancialReportsView_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            If _ready Then Return

            CmbPeriod.ItemsSource = New String() {"This Month", "Last Month", "Last 30 Days", "This Year", "All Time", "Custom"}
            DpFrom.SelectedDate = New DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)
            DpTo.SelectedDate = DateTime.Today
            _ready = True
            CmbPeriod.SelectedIndex = 0   ' triggers the first load
        End Sub

        Private Sub CmbPeriod_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles CmbPeriod.SelectionChanged
            If Not _ready Then Return

            Dim isCustom = TryCast(CmbPeriod.SelectedItem, String) = "Custom"
            PnlFrom.Visibility = If(isCustom, Visibility.Visible, Visibility.Collapsed)
            PnlTo.Visibility = If(isCustom, Visibility.Visible, Visibility.Collapsed)
            Refresh()
        End Sub

        Private Sub DatePicker_Changed(sender As Object, e As SelectionChangedEventArgs) Handles DpFrom.SelectedDateChanged, DpTo.SelectedDateChanged
            If _ready AndAlso TryCast(CmbPeriod.SelectedItem, String) = "Custom" Then Refresh()
        End Sub

        Private Function ResolveRange() As Boolean
            Dim today = DateTime.Today
            Select Case TryCast(CmbPeriod.SelectedItem, String)
                Case "This Month"
                    _from = New DateTime(today.Year, today.Month, 1)
                    _to = today
                Case "Last Month"
                    _from = New DateTime(today.Year, today.Month, 1).AddMonths(-1)
                    _to = _from.AddMonths(1).AddDays(-1)
                Case "Last 30 Days"
                    _from = today.AddDays(-29)
                    _to = today
                Case "This Year"
                    _from = New DateTime(today.Year, 1, 1)
                    _to = today
                Case "All Time"
                    _from = New DateTime(2000, 1, 1)
                    _to = New DateTime(2100, 1, 1)
                Case "Custom"
                    If Not DpFrom.SelectedDate.HasValue OrElse Not DpTo.SelectedDate.HasValue Then Return False
                    _from = DpFrom.SelectedDate.Value
                    _to = DpTo.SelectedDate.Value
                    If _to < _from Then
                        TxtRange.Text = "The end date is before the start date."
                        Return False
                    End If
                Case Else
                    Return False
            End Select
            Return True
        End Function

        Private Sub Refresh()
            If Not ResolveRange() Then Return

            Try
                _summary = ReportRepository.GetSummary(_from, _to)
            Catch ex As Exception
                AlertUtil.ShowError($"Could not build the report: {ex.Message}", "Financial Reports", ViewHelpers.Owner(Me))
                Return
            End Try

            TxtRange.Text = If(TryCast(CmbPeriod.SelectedItem, String) = "All Time", "All dates", $"{_from:MMM dd, yyyy} - {_to:MMM dd, yyyy}")
            TxtRevenue.Text = _summary.Revenue.ToString("N2")
            TxtExpenses.Text = _summary.Expenses.ToString("N2")
            TxtNet.Text = _summary.Net.ToString("N2")
            TxtNet.Foreground = New SolidColorBrush(If(_summary.Net < 0D, Color.FromRgb(&HC6, &H28, &H28), Color.FromRgb(&H2B, &H29, &H27)))
            TxtRentals.Text = _summary.CompletedRentals.ToString()
            DgCategory.ItemsSource = _summary.ByCategory.DefaultView
            DgMonth.ItemsSource = _summary.ByMonth.DefaultView
        End Sub

        ' ---------- CSV export ----------

        Private Sub BtnExport_Click(sender As Object, e As RoutedEventArgs) Handles BtnExport.Click
            If _summary Is Nothing Then Return

            Dim dialog As New Microsoft.Win32.SaveFileDialog With {
                .Filter = "CSV file (*.csv)|*.csv",
                .FileName = $"AutoRent-financial-report-{DateTime.Today:yyyy-MM-dd}.csv"
            }
            If dialog.ShowDialog() <> True Then Return

            Try
                Dim sb As New StringBuilder()
                sb.AppendLine("AutoRent Financial Report")
                sb.AppendLine($"Period,{Csv(TxtRange.Text)}")
                sb.AppendLine($"Revenue,{_summary.Revenue.ToString("0.00", CultureInfo.InvariantCulture)}")
                sb.AppendLine($"Maintenance Expenses,{_summary.Expenses.ToString("0.00", CultureInfo.InvariantCulture)}")
                sb.AppendLine($"Net Income,{_summary.Net.ToString("0.00", CultureInfo.InvariantCulture)}")
                sb.AppendLine($"Completed Rentals,{_summary.CompletedRentals}")
                sb.AppendLine()
                sb.AppendLine("Revenue by Category")
                sb.AppendLine("Category,Rentals,Revenue")
                For Each row As DataRow In _summary.ByCategory.Rows
                    sb.AppendLine($"{Csv(Convert.ToString(row("category")))},{row("rentals")},{Convert.ToDecimal(row("revenue")).ToString("0.00", CultureInfo.InvariantCulture)}")
                Next
                sb.AppendLine()
                sb.AppendLine("Monthly Summary")
                sb.AppendLine("Month,Rentals,Revenue,Expenses,Net")
                For Each row As DataRow In _summary.ByMonth.Rows
                    sb.AppendLine($"{row("month")},{row("rentals")},{Convert.ToDecimal(row("revenue")).ToString("0.00", CultureInfo.InvariantCulture)},{Convert.ToDecimal(row("expenses")).ToString("0.00", CultureInfo.InvariantCulture)},{Convert.ToDecimal(row("net")).ToString("0.00", CultureInfo.InvariantCulture)}")
                Next

                File.WriteAllText(dialog.FileName, sb.ToString(), New UTF8Encoding(True))
                AlertUtil.ShowSuccess("The report was exported.", "Financial Reports", ViewHelpers.Owner(Me))
            Catch ex As Exception
                AlertUtil.ShowError($"Could not export the report: {ex.Message}", "Financial Reports", ViewHelpers.Owner(Me))
            End Try
        End Sub

        Private Shared Function Csv(value As String) As String
            If value Is Nothing Then Return ""
            If value.IndexOfAny({","c, """"c, ControlChars.Cr, ControlChars.Lf}) >= 0 Then
                Return """" & value.Replace("""", """""") & """"
            End If
            Return value
        End Function
    End Class
End Namespace
