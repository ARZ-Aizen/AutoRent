Imports System.Collections.Generic
Imports System.Data
Imports System.Linq
Imports AutoRent.Data.Context

Namespace Data.Repositories
    Public Class DashboardStats
        Public Property TotalVehicles As Integer
        Public Property StatusCounts As New Dictionary(Of String, Integer)
        Public Property RevenueThisMonth As Decimal
        Public Property MaintenanceCostThisMonth As Decimal
        Public Property OverdueMaintenance As Integer
        Public Property ActiveUsers As Integer
    End Class

    Public Class FinancialSummary
        Public Property Revenue As Decimal
        Public Property Expenses As Decimal
        Public Property CompletedRentals As Integer
        Public ReadOnly Property Net As Decimal
            Get
                Return Revenue - Expenses
            End Get
        End Property
        Public Property ByCategory As DataTable
        Public Property ByMonth As DataTable
    End Class

    Public NotInheritable Class ReportRepository
        Private Sub New()
        End Sub

        Private Shared Function Lookup(Of T)(source As IDictionary(Of String, T), key As String) As T
            Dim result As T = Nothing
            source.TryGetValue(key, result)
            Return result
        End Function

        Public Shared Function GetDashboardStats() As DashboardStats
            Dim stats As New DashboardStats()

            For Each status In VehicleRepository.Statuses
                stats.StatusCounts(status) = 0
            Next
            Dim byStatus = Db.Query("SELECT [status], COUNT(*) AS [n] FROM [Vehicles] GROUP BY [status]")
            For Each row As DataRow In byStatus.Rows
                Dim key = If(IsDBNull(row("status")), "Available", Convert.ToString(row("status")))
                stats.StatusCounts(key) = Convert.ToInt32(row("n"))
                stats.TotalVehicles += Convert.ToInt32(row("n"))
            Next

            Dim monthStart As New DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)
            Dim nextMonth = monthStart.AddMonths(1)

            stats.RevenueThisMonth = Db.Sum("SELECT SUM([total]) FROM [Reservations] WHERE [status] = 'Completed' AND [return_date] >= ? AND [return_date] < ?", Db.Dt(monthStart), Db.Dt(nextMonth))
            stats.MaintenanceCostThisMonth = Db.Sum("SELECT SUM([cost]) FROM [Maintenance] WHERE [status] = 'Completed' AND [completed_at] >= ? AND [completed_at] < ?", Db.Dt(monthStart), Db.Dt(nextMonth))
            stats.OverdueMaintenance = Db.Count("SELECT COUNT(*) FROM [Maintenance] WHERE [status] = 'Scheduled' AND [scheduled_date] < ?", Db.Dt(DateTime.Today))
            stats.ActiveUsers = UserAdminRepository.ActiveUserCount()
            Return stats
        End Function

        Public Shared Function GetRecentReservations(maxRows As Integer) As DataTable
            Return Db.Query(
                $"SELECT TOP {maxRows} r.[customer_name], v.[plate] & ' - ' & v.[make] & ' ' & v.[model] AS [vehicle], r.[pickup_date], r.[return_date], r.[total], r.[status] " &
                "FROM [Reservations] AS r LEFT JOIN [Vehicles] AS v ON r.[vehicle_id] = v.[vehicle_id] " &
                "ORDER BY r.[created_at] DESC")
        End Function

        Public Shared Function GetSummary(fromDate As DateTime, toDate As DateTime) As FinancialSummary
            Dim startAt = fromDate.Date
            Dim endBefore = toDate.Date.AddDays(1)
            Dim summary As New FinancialSummary()

            Dim rentals = Db.Query(
                "SELECT r.[return_date], r.[total], v.[category] " &
                "FROM [Reservations] AS r LEFT JOIN [Vehicles] AS v ON r.[vehicle_id] = v.[vehicle_id] " &
                "WHERE r.[status] = 'Completed' AND r.[return_date] >= ? AND r.[return_date] < ?",
                Db.Dt(startAt), Db.Dt(endBefore))

            Dim expenses = Db.Query(
                "SELECT [completed_at], [cost] FROM [Maintenance] WHERE [status] = 'Completed' AND [completed_at] >= ? AND [completed_at] < ?",
                Db.Dt(startAt), Db.Dt(endBefore))

            Dim categoryRentals As New SortedDictionary(Of String, Integer)
            Dim categoryRevenue As New SortedDictionary(Of String, Decimal)
            Dim monthRentals As New SortedDictionary(Of String, Integer)
            Dim monthRevenue As New SortedDictionary(Of String, Decimal)
            Dim monthExpenses As New SortedDictionary(Of String, Decimal)

            For Each row As DataRow In rentals.Rows
                Dim amount = If(IsDBNull(row("total")), 0D, Convert.ToDecimal(row("total")))
                Dim category = If(IsDBNull(row("category")), "(Unassigned)", Convert.ToString(row("category")))
                Dim monthKey = Convert.ToDateTime(row("return_date")).ToString("yyyy-MM")

                summary.Revenue += amount
                summary.CompletedRentals += 1
                categoryRentals(category) = Lookup(categoryRentals, category) + 1
                categoryRevenue(category) = Lookup(categoryRevenue, category) + amount
                monthRentals(monthKey) = Lookup(monthRentals, monthKey) + 1
                monthRevenue(monthKey) = Lookup(monthRevenue, monthKey) + amount
            Next

            For Each row As DataRow In expenses.Rows
                Dim cost = If(IsDBNull(row("cost")), 0D, Convert.ToDecimal(row("cost")))
                Dim monthKey = Convert.ToDateTime(row("completed_at")).ToString("yyyy-MM")
                summary.Expenses += cost
                monthExpenses(monthKey) = Lookup(monthExpenses, monthKey) + cost
            Next

            summary.ByCategory = New DataTable()
            summary.ByCategory.Columns.Add("category", GetType(String))
            summary.ByCategory.Columns.Add("rentals", GetType(Integer))
            summary.ByCategory.Columns.Add("revenue", GetType(Decimal))
            For Each kv In categoryRevenue.OrderByDescending(Function(p) p.Value)
                summary.ByCategory.Rows.Add(kv.Key, categoryRentals(kv.Key), kv.Value)
            Next

            Dim months As New SortedSet(Of String)(monthRevenue.Keys)
            months.UnionWith(monthExpenses.Keys)

            summary.ByMonth = New DataTable()
            summary.ByMonth.Columns.Add("month", GetType(String))
            summary.ByMonth.Columns.Add("rentals", GetType(Integer))
            summary.ByMonth.Columns.Add("revenue", GetType(Decimal))
            summary.ByMonth.Columns.Add("expenses", GetType(Decimal))
            summary.ByMonth.Columns.Add("net", GetType(Decimal))
            For Each monthKey In months.Reverse()
                Dim rev = Lookup(monthRevenue, monthKey)
                Dim exp = Lookup(monthExpenses, monthKey)
                summary.ByMonth.Rows.Add(monthKey, Lookup(monthRentals, monthKey), rev, exp, rev - exp)
            Next

            Return summary
        End Function
    End Class
End Namespace
