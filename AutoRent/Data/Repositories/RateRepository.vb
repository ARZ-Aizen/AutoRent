Imports System.Data
Imports AutoRent.Data.Context

Namespace Data.Repositories
    Public NotInheritable Class RateRepository
        Private Sub New()
        End Sub

        Public Shared Function GetAll() As DataTable
            Return Db.Query(
                "SELECT r.[rate_id], r.[category], r.[daily_rate], r.[weekly_rate], r.[monthly_rate], r.[deposit], r.[late_fee_per_day], r.[updated_at], " &
                "(SELECT COUNT(*) FROM [Vehicles] AS v WHERE v.[category] = r.[category]) AS [vehicle_count] " &
                "FROM [Rates] AS r ORDER BY r.[daily_rate]")
        End Function

        Public Shared Function GetCategories() As DataTable
            Return Db.Query("SELECT [category] FROM [Rates] ORDER BY [daily_rate]")
        End Function

        Public Shared Function Add(category As String, daily As Decimal, weekly As Decimal, monthly As Decimal,
                                   deposit As Decimal, lateFee As Decimal) As String
            If CategoryExists(category, 0) Then Return $"The category '{category.Trim()}' already exists."

            Db.Execute("INSERT INTO [Rates] ([category], [daily_rate], [weekly_rate], [monthly_rate], [deposit], [late_fee_per_day], [updated_at]) VALUES (?, ?, ?, ?, ?, ?, ?)",
                       Db.Text(category, 30), Db.Money(daily), Db.Money(weekly), Db.Money(monthly), Db.Money(deposit), Db.Money(lateFee), Db.Dt(DateTime.Now))
            Return Nothing
        End Function

        Public Shared Function Update(rateId As Integer, oldCategory As String, category As String, daily As Decimal, weekly As Decimal,
                                      monthly As Decimal, deposit As Decimal, lateFee As Decimal) As String
            If CategoryExists(category, rateId) Then Return $"The category '{category.Trim()}' already exists."

            Db.Execute("UPDATE [Rates] SET [category] = ?, [daily_rate] = ?, [weekly_rate] = ?, [monthly_rate] = ?, [deposit] = ?, [late_fee_per_day] = ?, [updated_at] = ? WHERE [rate_id] = ?",
                       Db.Text(category, 30), Db.Money(daily), Db.Money(weekly), Db.Money(monthly), Db.Money(deposit), Db.Money(lateFee), Db.Dt(DateTime.Now), Db.Int(rateId))

            ' Keep vehicles attached when a category is renamed
            If Not String.Equals(oldCategory, category.Trim(), StringComparison.Ordinal) Then
                Db.Execute("UPDATE [Vehicles] SET [category] = ? WHERE [category] = ?", Db.Text(category, 30), Db.Text(oldCategory, 30))
            End If
            Return Nothing
        End Function

        Public Shared Function Delete(rateId As Integer, category As String) As String
            If Db.Count("SELECT COUNT(*) FROM [Vehicles] WHERE [category] = ?", Db.Text(category, 30)) > 0 Then
                Return "Vehicles are still using this category. Move them to another category first."
            End If
            Db.Execute("DELETE FROM [Rates] WHERE [rate_id] = ?", Db.Int(rateId))
            Return Nothing
        End Function

        Private Shared Function CategoryExists(category As String, excludeId As Integer) As Boolean
            Return Db.Count("SELECT COUNT(*) FROM [Rates] WHERE [category] = ? AND [rate_id] <> ?", Db.Text(category, 30), Db.Int(excludeId)) > 0
        End Function
    End Class
End Namespace
