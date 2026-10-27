Imports System.Collections.Generic
Imports System.Data
Imports System.Linq
Imports AutoRent.Data.Context
Imports AutoRent.Data.Models

Namespace Data.Repositories
    Public NotInheritable Class RateRepository
        Private Sub New()
        End Sub

        Private Shared ReadOnly _vehicleOverrides As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

        Private Shared ReadOnly _rules As New List(Of PricingRule) From {
            New PricingRule With {
                .RuleId = "RR-001",
                .RuleName = "Standard daily",
                .Category = "All",
                .EffectiveDates = "01 Jan 2026 / Ongoing",
                .CalculationMethod = "Category / vehicle base",
                .RateMultiplier = "Per day",
                .IsActive = True,
                .LastModified = "Admin · 01 Jan, 09:00"
            },
            New PricingRule With {
                .RuleId = "RR-004",
                .RuleName = "Weekend uplift",
                .Category = "All",
                .EffectiveDates = "01 Oct / 31 Dec 2026",
                .CalculationMethod = "Fri-Sun · base multiplier",
                .RateMultiplier = "×1.15",
                .IsActive = True,
                .LastModified = "Admin · 14 Oct, 15:18"
            },
            New PricingRule With {
                .RuleId = "RR-005",
                .RuleName = "Holiday pricing",
                .Category = "All",
                .EffectiveDates = "24 Dec / 31 Dec 2026",
                .CalculationMethod = "Holiday overrides weekend",
                .RateMultiplier = "×1.25",
                .IsActive = True,
                .LastModified = "Admin · 12 Oct, 10:00"
            },
            New PricingRule With {
                .RuleId = "RR-006",
                .RuleName = "Overtime",
                .Category = "All",
                .EffectiveDates = "01 Jan 2026 / Ongoing",
                .CalculationMethod = "After 60-min grace / hour",
                .RateMultiplier = "Daily ÷ 8",
                .IsActive = True,
                .LastModified = "Admin · 01 Jan, 09:10"
            },
            New PricingRule With {
                .RuleId = "RR-007",
                .RuleName = "Late return",
                .Category = "All",
                .EffectiveDates = "01 Jan 2026 / Ongoing",
                .CalculationMethod = "After 8h · replaces overtime",
                .RateMultiplier = "1 daily rate",
                .IsActive = True,
                .LastModified = "Admin · 01 Jan, 09:15"
            },
            New PricingRule With {
                .RuleId = "RR-008",
                .RuleName = "Extra mileage",
                .Category = "All",
                .EffectiveDates = "01 Jan 2026 / Ongoing",
                .CalculationMethod = "Above 200 km / day",
                .RateMultiplier = "₱0.75 / km",
                .IsActive = True,
                .LastModified = "Admin · 03 Oct, 11:30"
            },
            New PricingRule With {
                .RuleId = "RR-009",
                .RuleName = "Refueling",
                .Category = "All",
                .EffectiveDates = "01 Jan 2026 / Ongoing",
                .CalculationMethod = "Actual fuel + service fee",
                .RateMultiplier = "₱25 + fuel",
                .IsActive = True,
                .LastModified = "Admin · 03 Oct, 11:35"
            },
            New PricingRule With {
                .RuleId = "RR-010",
                .RuleName = "Child seat",
                .Category = "All",
                .EffectiveDates = "01 Jan 2026 / Ongoing",
                .CalculationMethod = "Equipment / day",
                .RateMultiplier = "₱15 / day",
                .IsActive = True,
                .LastModified = "Admin · 03 Oct, 11:40"
            },
            New PricingRule With {
                .RuleId = "RR-011",
                .RuleName = "Manager discount",
                .Category = "All",
                .EffectiveDates = "01 Oct / 31 Dec 2026",
                .CalculationMethod = "Authorized percentage",
                .RateMultiplier = "Up to 10%",
                .IsActive = False,
                .LastModified = "Admin · 10 Oct, 16:00"
            }
        }

        Public Shared Function GetAll() As DataTable
            Return Db.Query(
                "SELECT r.[rate_id], r.[category], r.[daily_rate], r.[weekly_rate], r.[monthly_rate], r.[deposit], r.[late_fee_per_day], r.[updated_at], " &
                "(SELECT COUNT(*) FROM [Vehicles] AS v WHERE v.[category] = r.[category]) AS [vehicle_count] " &
                "FROM [Rates] AS r ORDER BY r.[daily_rate]")
        End Function

        Public Shared Function GetAllTiers() As List(Of RateTier)
            Dim list As New List(Of RateTier)()
            Dim table = Db.Query("SELECT [rate_id], [category], [daily_rate], [weekly_rate], [monthly_rate], [deposit], [late_fee_per_day], [updated_at] FROM [Rates] ORDER BY [daily_rate] DESC")

            For Each row As DataRow In table.Rows
                Dim cat = Convert.ToString(row("category"))
                Dim overrideText = "None"
                SyncLock _vehicleOverrides
                    If _vehicleOverrides.ContainsKey(cat) Then
                        overrideText = _vehicleOverrides(cat)
                    End If
                End SyncLock

                list.Add(New RateTier With {
                    .RateId = Convert.ToInt32(row("rate_id")),
                    .Category = cat,
                    .DailyRate = If(IsDBNull(row("daily_rate")), 0D, Convert.ToDecimal(row("daily_rate"))),
                    .WeeklyRate = If(IsDBNull(row("weekly_rate")), 0D, Convert.ToDecimal(row("weekly_rate"))),
                    .MonthlyRate = If(IsDBNull(row("monthly_rate")), 0D, Convert.ToDecimal(row("monthly_rate"))),
                    .Deposit = If(IsDBNull(row("deposit")), 0D, Convert.ToDecimal(row("deposit"))),
                    .LateFeePerDay = If(IsDBNull(row("late_fee_per_day")), 0D, Convert.ToDecimal(row("late_fee_per_day"))),
                    .VehicleOverride = If(String.IsNullOrWhiteSpace(overrideText), "None", overrideText),
                    .UpdatedAt = If(IsDBNull(row("updated_at")), DateTime.Now, Convert.ToDateTime(row("updated_at")))
                })
            Next

            Dim orderMap As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase) From {
                {"Sedan", 1},
                {"SUV (Sport Utility Vehicle)", 2},
                {"Hatchback", 3},
                {"Van", 4},
                {"MPV (Multi-Purpose Vehicle)", 5},
                {"Pickup Truck", 6},
                {"Sports Car", 7},
                {"Luxury Car", 8},
                {"Hybrid Car", 9},
                {"Electric Car (EV)", 10}
            }

            Return list.OrderBy(Function(t) If(orderMap.ContainsKey(t.Category), orderMap(t.Category), 99)).ThenBy(Function(t) t.Category).ToList()
        End Function

        Public Shared Sub SetVehicleOverride(category As String, overrideText As String)
            If String.IsNullOrWhiteSpace(category) Then Return
            SyncLock _vehicleOverrides
                If String.IsNullOrWhiteSpace(overrideText) OrElse overrideText.Trim().Equals("None", StringComparison.OrdinalIgnoreCase) Then
                    _vehicleOverrides.Remove(category.Trim())
                Else
                    _vehicleOverrides(category.Trim()) = overrideText.Trim()
                End If
            End SyncLock
        End Sub

        Public Shared Function GetPricingRules() As List(Of PricingRule)
            SyncLock _rules
                Return _rules.ToList()
            End SyncLock
        End Function

        Public Shared Sub AddPricingRule(rule As PricingRule)
            If rule Is Nothing Then Return
            SyncLock _rules
                _rules.Add(rule)
            End SyncLock
        End Sub

        Public Shared Sub UpdatePricingRule(ruleId As String, ruleName As String, category As String, method As String, multiplier As String, effective As String, isActive As Boolean)
            SyncLock _rules
                Dim rule = _rules.FirstOrDefault(Function(r) r.RuleId.Equals(ruleId, StringComparison.OrdinalIgnoreCase))
                If rule IsNot Nothing Then
                    rule.RuleName = ruleName
                    rule.Category = category
                    rule.CalculationMethod = method
                    rule.RateMultiplier = multiplier
                    rule.EffectiveDates = effective
                    rule.IsActive = isActive
                    rule.LastModified = $"Admin · {DateTime.Now:dd MMM, HH:mm}"
                End If
            End SyncLock
        End Sub

        Public Shared Function GetCategories() As DataTable
            Return Db.Query("SELECT [category] FROM [Rates] ORDER BY [daily_rate]")
        End Function

        Public Shared Function Add(category As String, daily As Decimal, weekly As Decimal, monthly As Decimal,
                                   deposit As Decimal, lateFee As Decimal) As String
            category = If(category, "").Trim()
            Dim issue = ValidateRate(category, daily, weekly, monthly, deposit, lateFee)
            If issue IsNot Nothing Then Return issue
            If CategoryExists(category, 0) Then Return $"The category '{category}' already exists."

            Db.Execute("INSERT INTO [Rates] ([category], [daily_rate], [weekly_rate], [monthly_rate], [deposit], [late_fee_per_day], [updated_at]) VALUES (?, ?, ?, ?, ?, ?, ?)",
                       Db.Text(category, 50), Db.Money(daily), Db.Money(weekly), Db.Money(monthly), Db.Money(deposit), Db.Money(lateFee), Db.Dt(DateTime.Now))
            Return Nothing
        End Function

        Public Shared Function Update(rateId As Integer, oldCategory As String, category As String, daily As Decimal, weekly As Decimal,
                                      monthly As Decimal, deposit As Decimal, lateFee As Decimal) As String
            category = If(category, "").Trim()
            Dim issue = ValidateRate(category, daily, weekly, monthly, deposit, lateFee)
            If issue IsNot Nothing Then Return issue
            If CategoryExists(category, rateId) Then Return $"The category '{category}' already exists."

            Db.Execute("UPDATE [Rates] SET [category] = ?, [daily_rate] = ?, [weekly_rate] = ?, [monthly_rate] = ?, [deposit] = ?, [late_fee_per_day] = ?, [updated_at] = ? WHERE [rate_id] = ?",
                       Db.Text(category, 50), Db.Money(daily), Db.Money(weekly), Db.Money(monthly), Db.Money(deposit), Db.Money(lateFee), Db.Dt(DateTime.Now), Db.Int(rateId))

            If Not String.Equals(oldCategory, category, StringComparison.OrdinalIgnoreCase) Then
                Db.Execute("UPDATE [Vehicles] SET [category] = ? WHERE [category] = ?", Db.Text(category, 50), Db.Text(oldCategory, 50))
                SyncLock _vehicleOverrides
                    If _vehicleOverrides.ContainsKey(oldCategory) Then
                        Dim val = _vehicleOverrides(oldCategory)
                        _vehicleOverrides.Remove(oldCategory)
                        _vehicleOverrides(category) = val
                    End If
                End SyncLock
            End If
            Return Nothing
        End Function

        Private Shared Function ValidateRate(category As String, daily As Decimal, weekly As Decimal,
                                            monthly As Decimal, deposit As Decimal, lateFee As Decimal) As String
            If String.IsNullOrWhiteSpace(category) OrElse category.Length > 50 Then Return "Enter a valid category (1–50 characters)."
            If daily <= 0D OrElse weekly < 0D OrElse monthly < 0D OrElse deposit < 0D OrElse lateFee < 0D Then
                Return "Daily rate must be positive; other prices must not be negative."
            End If
            If daily > 100000000D OrElse weekly > 100000000D OrElse monthly > 100000000D OrElse
               deposit > 100000000D OrElse lateFee > 100000000D Then Return "The amount is too large."
            Return Nothing
        End Function

        Public Shared Function Delete(rateId As Integer, category As String) As String
            If Db.Count("SELECT COUNT(*) FROM [Vehicles] WHERE [category] = ?", Db.Text(category, 50)) > 0 Then
                Return "Vehicles are still using this category. Move them to another category first."
            End If
            Db.Execute("DELETE FROM [Rates] WHERE [rate_id] = ?", Db.Int(rateId))
            SyncLock _vehicleOverrides
                _vehicleOverrides.Remove(category)
            End SyncLock
            Return Nothing
        End Function

        Private Shared Function CategoryExists(category As String, excludeId As Integer) As Boolean
            Return Db.Count("SELECT COUNT(*) FROM [Rates] WHERE [category] = ? AND [rate_id] <> ?", Db.Text(category, 50), Db.Int(excludeId)) > 0
        End Function
    End Class
End Namespace
