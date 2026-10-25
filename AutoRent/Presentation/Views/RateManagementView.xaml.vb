Imports System.Data
Imports System.Windows
Imports System.Windows.Controls
Imports AutoRent.Common.Utilities
Imports AutoRent.Data.Repositories

Namespace Presentation.Views
    Partial Public Class RateManagementView
        Inherits UserControl

        Private _selectedId As Integer = 0
        Private _selectedCategory As String = ""
        Private _ready As Boolean = False

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub RateManagementView_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            If _ready Then Return
            _ready = True
            LoadRates()
            ClearForm()
        End Sub

        Private Sub LoadRates()
            Try
                DgRates.ItemsSource = RateRepository.GetAll().DefaultView
            Catch ex As Exception
                AlertUtil.ShowError($"Could not load rates: {ex.Message}", "Rate Management", ViewHelpers.Owner(Me))
            End Try
        End Sub

        Private Sub ClearForm()
            _selectedId = 0
            _selectedCategory = ""
            DgRates.SelectedItem = Nothing
            TxtCategory.Clear()
            TxtDaily.Clear()
            TxtWeekly.Clear()
            TxtMonthly.Clear()
            TxtDeposit.Text = "0"
            TxtLateFee.Text = "0"
            BtnAdd.IsEnabled = True
            BtnSave.IsEnabled = False
            BtnDelete.IsEnabled = False
        End Sub

        Private Sub DgRates_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles DgRates.SelectionChanged
            Dim row = TryCast(DgRates.SelectedItem, DataRowView)
            If row Is Nothing Then Return

            _selectedId = ViewHelpers.IntOf(row, "rate_id")
            _selectedCategory = ViewHelpers.Str(row, "category")
            TxtCategory.Text = _selectedCategory
            TxtDaily.Text = ViewHelpers.DecOf(row, "daily_rate").ToString("0.##")
            TxtWeekly.Text = ViewHelpers.DecOf(row, "weekly_rate").ToString("0.##")
            TxtMonthly.Text = ViewHelpers.DecOf(row, "monthly_rate").ToString("0.##")
            TxtDeposit.Text = ViewHelpers.DecOf(row, "deposit").ToString("0.##")
            TxtLateFee.Text = ViewHelpers.DecOf(row, "late_fee_per_day").ToString("0.##")

            BtnAdd.IsEnabled = False
            BtnSave.IsEnabled = True
            BtnDelete.IsEnabled = True
        End Sub

        Private Function ReadForm(ByRef daily As Decimal, ByRef weekly As Decimal, ByRef monthly As Decimal,
                                  ByRef deposit As Decimal, ByRef lateFee As Decimal) As String
            If String.IsNullOrWhiteSpace(TxtCategory.Text) Then Return "Enter a category name."
            If Not ViewHelpers.TryParseMoney(TxtDaily.Text, daily) OrElse daily <= 0D Then Return "Daily rate must be a number greater than 0."

            ' Weekly / monthly are optional: blank falls back to 6x / 22x the daily rate (a small multi-day discount)
            If String.IsNullOrWhiteSpace(TxtWeekly.Text) Then
                weekly = daily * 6D
            ElseIf Not ViewHelpers.TryParseMoney(TxtWeekly.Text, weekly) Then
                Return "Weekly rate must be a number (0 or more)."
            End If

            If String.IsNullOrWhiteSpace(TxtMonthly.Text) Then
                monthly = daily * 22D
            ElseIf Not ViewHelpers.TryParseMoney(TxtMonthly.Text, monthly) Then
                Return "Monthly rate must be a number (0 or more)."
            End If

            If String.IsNullOrWhiteSpace(TxtDeposit.Text) Then
                deposit = 0D
            ElseIf Not ViewHelpers.TryParseMoney(TxtDeposit.Text, deposit) Then
                Return "Deposit must be a number (0 or more)."
            End If

            If String.IsNullOrWhiteSpace(TxtLateFee.Text) Then
                lateFee = 0D
            ElseIf Not ViewHelpers.TryParseMoney(TxtLateFee.Text, lateFee) Then
                Return "Late fee must be a number (0 or more)."
            End If
            Return Nothing
        End Function

        Private Sub BtnAdd_Click(sender As Object, e As RoutedEventArgs) Handles BtnAdd.Click
            Dim daily, weekly, monthly, deposit, lateFee As Decimal
            Dim problem = ReadForm(daily, weekly, monthly, deposit, lateFee)
            If problem IsNot Nothing Then
                AlertUtil.ShowWarning(problem, "Rate Management", ViewHelpers.Owner(Me))
                Return
            End If

            Try
                problem = RateRepository.Add(TxtCategory.Text, daily, weekly, monthly, deposit, lateFee)
                If problem IsNot Nothing Then
                    AlertUtil.ShowWarning(problem, "Rate Management", ViewHelpers.Owner(Me))
                    Return
                End If
                LoadRates()
                ClearForm()
            Catch ex As Exception
                AlertUtil.ShowError($"Could not add the category: {ex.Message}", "Rate Management", ViewHelpers.Owner(Me))
            End Try
        End Sub

        Private Sub BtnSave_Click(sender As Object, e As RoutedEventArgs) Handles BtnSave.Click
            If _selectedId = 0 Then Return

            Dim daily, weekly, monthly, deposit, lateFee As Decimal
            Dim problem = ReadForm(daily, weekly, monthly, deposit, lateFee)
            If problem IsNot Nothing Then
                AlertUtil.ShowWarning(problem, "Rate Management", ViewHelpers.Owner(Me))
                Return
            End If

            Try
                problem = RateRepository.Update(_selectedId, _selectedCategory, TxtCategory.Text, daily, weekly, monthly, deposit, lateFee)
                If problem IsNot Nothing Then
                    AlertUtil.ShowWarning(problem, "Rate Management", ViewHelpers.Owner(Me))
                    Return
                End If
                LoadRates()
                ClearForm()
            Catch ex As Exception
                AlertUtil.ShowError($"Could not save the rates: {ex.Message}", "Rate Management", ViewHelpers.Owner(Me))
            End Try
        End Sub

        Private Sub BtnClear_Click(sender As Object, e As RoutedEventArgs) Handles BtnClear.Click
            ClearForm()
        End Sub

        Private Sub BtnDelete_Click(sender As Object, e As RoutedEventArgs) Handles BtnDelete.Click
            If _selectedId = 0 Then Return
            If Not AlertUtil.Confirm($"Delete the '{_selectedCategory}' category?", "Delete Category", ViewHelpers.Owner(Me)) Then Return

            Try
                Dim problem = RateRepository.Delete(_selectedId, _selectedCategory)
                If problem IsNot Nothing Then
                    AlertUtil.ShowWarning(problem, "Rate Management", ViewHelpers.Owner(Me))
                    Return
                End If
                LoadRates()
                ClearForm()
            Catch ex As Exception
                AlertUtil.ShowError($"Could not delete the category: {ex.Message}", "Rate Management", ViewHelpers.Owner(Me))
            End Try
        End Sub
    End Class
End Namespace
