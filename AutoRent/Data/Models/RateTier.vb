Namespace Data.Models
    Public Class RateTier
        Public Property RateId As Integer
        Public Property Category As String = String.Empty
        Public Property DailyRate As Decimal
        Public Property WeeklyRate As Decimal
        Public Property MonthlyRate As Decimal
        Public Property Deposit As Decimal
        Public Property LateFeePerDay As Decimal
        Public Property VehicleOverride As String = "None"
        Public Property UpdatedAt As DateTime

        Public ReadOnly Property DisplayDaily As String
            Get
                Return $"₱{DailyRate:N0}"
            End Get
        End Property

        Public ReadOnly Property DisplayWeekly As String
            Get
                Return $"₱{WeeklyRate:N0}"
            End Get
        End Property

        Public ReadOnly Property DisplayMonthly As String
            Get
                Return $"₱{MonthlyRate:N0}"
            End Get
        End Property
    End Class

    Public Class PricingRule
        Public Property RuleId As String = String.Empty
        Public Property RuleName As String = String.Empty
        Public Property Category As String = "All"
        Public Property EffectiveDates As String = String.Empty
        Public Property CalculationMethod As String = String.Empty
        Public Property RateMultiplier As String = String.Empty
        Public Property IsActive As Boolean = True
        Public Property LastModified As String = String.Empty

        Public ReadOnly Property DisplayTitle As String
            Get
                Return $"{RuleId} · {RuleName}"
            End Get
        End Property

        Public ReadOnly Property StatusText As String
            Get
                Return If(IsActive, "Active", "Inactive")
            End Get
        End Property
    End Class
End Namespace
