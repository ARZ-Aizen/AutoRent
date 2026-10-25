Imports System.Collections.Generic
Imports System.Data
Imports System.Linq
Imports System.Windows
Imports System.Windows.Controls
Imports AutoRent.Common.Utilities
Imports AutoRent.Data.Repositories

Namespace Presentation.Views
    Partial Public Class FleetManagementView
        Inherits UserControl

        Private _table As DataTable
        Private _selectedId As Integer = 0
        Private _ready As Boolean = False

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub FleetManagementView_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            If _ready Then Return

            CmbStatus.ItemsSource = VehicleRepository.Statuses
            CmbFilter.ItemsSource = New String() {"All"}.Concat(VehicleRepository.Statuses).ToList()
            CmbFilter.SelectedIndex = 0
            _ready = True

            LoadCategories()
            LoadVehicles()
            ClearForm()
        End Sub

        ' ---------- Loading ----------

        Private Sub LoadCategories()
            Try
                CmbCategory.ItemsSource = RateRepository.GetCategories().DefaultView
            Catch ex As Exception
                AlertUtil.ShowError($"Could not load categories: {ex.Message}", "Fleet Management", ViewHelpers.Owner(Me))
            End Try
        End Sub

        Private Sub LoadVehicles()
            Try
                _table = VehicleRepository.GetAll()
                DgVehicles.ItemsSource = _table.DefaultView
                ApplyFilter()
            Catch ex As Exception
                AlertUtil.ShowError($"Could not load vehicles: {ex.Message}", "Fleet Management", ViewHelpers.Owner(Me))
            End Try
        End Sub

        Private Sub ApplyFilter()
            If _table Is Nothing Then Return

            Dim parts As New List(Of String)
            Dim q = TxtSearch.Text.Trim()
            If q.Length > 0 Then
                Dim pattern = ViewHelpers.EscapeLike(q)
                parts.Add($"(plate LIKE '%{pattern}%' OR make LIKE '%{pattern}%' OR model LIKE '%{pattern}%' OR category LIKE '%{pattern}%')")
            End If

            Dim status = TryCast(CmbFilter.SelectedItem, String)
            If Not String.IsNullOrEmpty(status) AndAlso status <> "All" Then
                parts.Add($"status = '{status}'")
            End If

            _table.DefaultView.RowFilter = String.Join(" AND ", parts)
            TxtCount.Text = $"{_table.DefaultView.Count} of {_table.Rows.Count} vehicles"
        End Sub

        Private Sub TxtSearch_TextChanged(sender As Object, e As TextChangedEventArgs) Handles TxtSearch.TextChanged
            If _ready Then ApplyFilter()
        End Sub

        Private Sub CmbFilter_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles CmbFilter.SelectionChanged
            If _ready Then ApplyFilter()
        End Sub

        ' ---------- Form ----------

        Private Sub ClearForm()
            _selectedId = 0
            DgVehicles.SelectedItem = Nothing
            TxtPlate.Clear()
            TxtMake.Clear()
            TxtModel.Clear()
            TxtYear.Clear()
            TxtMileage.Text = "0"
            CmbCategory.SelectedIndex = If(CmbCategory.Items.Count > 0, 0, -1)
            CmbStatus.SelectedItem = "Available"
            BtnAdd.IsEnabled = True
            BtnSave.IsEnabled = False
            BtnDelete.IsEnabled = False
        End Sub

        Private Sub DgVehicles_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles DgVehicles.SelectionChanged
            Dim row = TryCast(DgVehicles.SelectedItem, DataRowView)
            If row Is Nothing Then Return

            _selectedId = ViewHelpers.IntOf(row, "vehicle_id")
            TxtPlate.Text = ViewHelpers.Str(row, "plate")
            TxtMake.Text = ViewHelpers.Str(row, "make")
            TxtModel.Text = ViewHelpers.Str(row, "model")
            TxtYear.Text = ViewHelpers.Str(row, "model_year")
            TxtMileage.Text = ViewHelpers.IntOf(row, "mileage").ToString()
            SelectCategory(ViewHelpers.Str(row, "category"))
            CmbStatus.SelectedItem = ViewHelpers.Str(row, "status")

            BtnAdd.IsEnabled = False
            BtnSave.IsEnabled = True
            BtnDelete.IsEnabled = True
        End Sub

        Private Sub SelectCategory(category As String)
            For Each item As DataRowView In CmbCategory.Items
                If String.Equals(ViewHelpers.Str(item, "category"), category, StringComparison.OrdinalIgnoreCase) Then
                    CmbCategory.SelectedItem = item
                    Return
                End If
            Next
            CmbCategory.SelectedIndex = -1
        End Sub

        ''' <summary>Validates the form. Returns an error message, or Nothing when valid.</summary>
        Private Function ReadForm(ByRef modelYear As Integer?, ByRef mileage As Integer, ByRef category As String, ByRef status As String) As String
            If String.IsNullOrWhiteSpace(TxtPlate.Text) Then Return "Enter the plate number."
            If String.IsNullOrWhiteSpace(TxtMake.Text) Then Return "Enter the make."
            If String.IsNullOrWhiteSpace(TxtModel.Text) Then Return "Enter the model."

            Dim catRow = TryCast(CmbCategory.SelectedItem, DataRowView)
            If catRow Is Nothing Then Return "Choose a category. Add categories in Rate Management."
            category = ViewHelpers.Str(catRow, "category")

            modelYear = Nothing
            If Not String.IsNullOrWhiteSpace(TxtYear.Text) Then
                Dim y As Integer
                If Not ViewHelpers.TryParseWhole(TxtYear.Text, y) OrElse y < 1980 OrElse y > DateTime.Today.Year + 1 Then
                    Return $"Year must be between 1980 and {DateTime.Today.Year + 1}."
                End If
                modelYear = y
            End If

            If Not ViewHelpers.TryParseWhole(TxtMileage.Text, mileage) Then Return "Mileage must be a whole number (0 or more)."

            status = TryCast(CmbStatus.SelectedItem, String)
            If String.IsNullOrEmpty(status) Then Return "Choose a status."
            Return Nothing
        End Function

        ' ---------- Actions ----------

        Private Sub BtnAdd_Click(sender As Object, e As RoutedEventArgs) Handles BtnAdd.Click
            Dim year As Integer? = Nothing, mileage As Integer, category As String = Nothing, status As String = Nothing
            Dim problem = ReadForm(year, mileage, category, status)
            If problem IsNot Nothing Then
                AlertUtil.ShowWarning(problem, "Fleet Management", ViewHelpers.Owner(Me))
                Return
            End If

            Try
                problem = VehicleRepository.Add(TxtPlate.Text, TxtMake.Text, TxtModel.Text, category, year, mileage, status)
                If problem IsNot Nothing Then
                    AlertUtil.ShowWarning(problem, "Fleet Management", ViewHelpers.Owner(Me))
                    Return
                End If
                LoadVehicles()
                ClearForm()
            Catch ex As Exception
                AlertUtil.ShowError($"Could not add the vehicle: {ex.Message}", "Fleet Management", ViewHelpers.Owner(Me))
            End Try
        End Sub

        Private Sub BtnSave_Click(sender As Object, e As RoutedEventArgs) Handles BtnSave.Click
            If _selectedId = 0 Then Return

            Dim year As Integer? = Nothing, mileage As Integer, category As String = Nothing, status As String = Nothing
            Dim problem = ReadForm(year, mileage, category, status)
            If problem IsNot Nothing Then
                AlertUtil.ShowWarning(problem, "Fleet Management", ViewHelpers.Owner(Me))
                Return
            End If

            Try
                problem = VehicleRepository.Update(_selectedId, TxtPlate.Text, TxtMake.Text, TxtModel.Text, category, year, mileage, status)
                If problem IsNot Nothing Then
                    AlertUtil.ShowWarning(problem, "Fleet Management", ViewHelpers.Owner(Me))
                    Return
                End If
                LoadVehicles()
                ClearForm()
            Catch ex As Exception
                AlertUtil.ShowError($"Could not save the vehicle: {ex.Message}", "Fleet Management", ViewHelpers.Owner(Me))
            End Try
        End Sub

        Private Sub BtnClear_Click(sender As Object, e As RoutedEventArgs) Handles BtnClear.Click
            ClearForm()
        End Sub

        Private Sub BtnDelete_Click(sender As Object, e As RoutedEventArgs) Handles BtnDelete.Click
            If _selectedId = 0 Then Return
            If Not AlertUtil.Confirm($"Delete vehicle {TxtPlate.Text.Trim().ToUpperInvariant()}? This can't be undone.", "Delete Vehicle", ViewHelpers.Owner(Me)) Then Return

            Try
                Dim problem = VehicleRepository.Delete(_selectedId)
                If problem IsNot Nothing Then
                    AlertUtil.ShowWarning(problem, "Fleet Management", ViewHelpers.Owner(Me))
                    Return
                End If
                LoadVehicles()
                ClearForm()
            Catch ex As Exception
                AlertUtil.ShowError($"Could not delete the vehicle: {ex.Message}", "Fleet Management", ViewHelpers.Owner(Me))
            End Try
        End Sub
    End Class
End Namespace
