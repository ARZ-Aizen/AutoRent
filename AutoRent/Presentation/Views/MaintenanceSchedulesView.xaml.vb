Imports System.Collections.Generic
Imports System.Data
Imports System.Linq
Imports System.Windows
Imports System.Windows.Controls
Imports AutoRent.Common.Utilities
Imports AutoRent.Data.Repositories

Namespace Presentation.Views
    Partial Public Class MaintenanceSchedulesView
        Inherits UserControl

        Private _table As DataTable
        Private _selectedId As Integer = 0
        Private _ready As Boolean = False

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub MaintenanceSchedulesView_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            If _ready Then Return

            CmbService.ItemsSource = New String() {"Oil change", "Tire rotation", "Brake service", "Battery", "Routine inspection", "Body repair", "Other"}
            CmbStatus.ItemsSource = MaintenanceRepository.Statuses
            CmbFilter.ItemsSource = New String() {"All", "Open (Scheduled + In Progress)"}.Concat(MaintenanceRepository.Statuses).ToList()
            CmbFilter.SelectedIndex = 0
            _ready = True

            Try
                CmbVehicle.ItemsSource = VehicleRepository.GetPickList().DefaultView
            Catch ex As Exception
                AlertUtil.ShowError($"Could not load vehicles: {ex.Message}", "Maintenance", ViewHelpers.Owner(Me))
            End Try

            LoadRecords()
            ClearForm()
        End Sub

        Private Sub LoadRecords()
            Try
                _table = MaintenanceRepository.GetAll()
                _table.Columns.Add("due", GetType(String))
                For Each row As DataRow In _table.Rows
                    row("due") = DueText(Convert.ToString(row("status")), row("scheduled_date"))
                Next
                DgMaintenance.ItemsSource = _table.DefaultView
                ApplyFilter()
                UpdateSummary()
            Catch ex As Exception
                AlertUtil.ShowError($"Could not load maintenance records: {ex.Message}", "Maintenance", ViewHelpers.Owner(Me))
            End Try
        End Sub

        ''' <summary>"Overdue" / "Today" / "In n days" for jobs that are still Scheduled; blank otherwise.</summary>
        Private Shared Function DueText(status As String, scheduled As Object) As String
            If status <> "Scheduled" OrElse IsDBNull(scheduled) Then Return String.Empty
            Dim days = (Convert.ToDateTime(scheduled).Date - DateTime.Today).Days
            If days < 0 Then Return "Overdue"
            If days = 0 Then Return "Today"
            Return $"In {days} day{If(days = 1, "", "s")}"
        End Function

        Private Sub UpdateSummary()
            Dim overdue = _table.Select("due = 'Overdue'").Length
            Dim openJobs = _table.Select("status IN ('Scheduled', 'In Progress')").Length
            TxtSummary.Text = $"{openJobs} open job{If(openJobs = 1, "", "s")}, {overdue} overdue. A vehicle with a job In Progress shows as Maintenance in the fleet."
        End Sub

        Private Sub ApplyFilter()
            If _table Is Nothing Then Return
            Dim choice = TryCast(CmbFilter.SelectedItem, String)

            If String.IsNullOrEmpty(choice) OrElse choice = "All" Then
                _table.DefaultView.RowFilter = ""
            ElseIf choice.StartsWith("Open") Then
                _table.DefaultView.RowFilter = "status IN ('Scheduled', 'In Progress')"
            Else
                _table.DefaultView.RowFilter = $"status = '{choice}'"
            End If
            TxtCount.Text = $"{_table.DefaultView.Count} of {_table.Rows.Count} records"
        End Sub

        Private Sub CmbFilter_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles CmbFilter.SelectionChanged
            If _ready Then ApplyFilter()
        End Sub

        Private Sub ClearForm()
            _selectedId = 0
            DgMaintenance.SelectedItem = Nothing
            CmbVehicle.SelectedIndex = -1
            CmbService.Text = ""
            DpDate.SelectedDate = DateTime.Today
            TxtCost.Text = "0"
            CmbStatus.SelectedItem = "Scheduled"
            TxtNotes.Clear()
            BtnAdd.IsEnabled = True
            BtnSave.IsEnabled = False
            BtnDelete.IsEnabled = False
        End Sub

        Private Sub DgMaintenance_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles DgMaintenance.SelectionChanged
            Dim row = TryCast(DgMaintenance.SelectedItem, DataRowView)
            If row Is Nothing Then Return

            _selectedId = ViewHelpers.IntOf(row, "maintenance_id")
            CmbVehicle.SelectedValue = ViewHelpers.IntOf(row, "vehicle_id")
            CmbService.Text = ViewHelpers.Str(row, "service_type")
            If Not IsDBNull(row("scheduled_date")) Then DpDate.SelectedDate = Convert.ToDateTime(row("scheduled_date"))
            TxtCost.Text = ViewHelpers.DecOf(row, "cost").ToString("0.##")
            CmbStatus.SelectedItem = ViewHelpers.Str(row, "status")
            TxtNotes.Text = ViewHelpers.Str(row, "notes")

            BtnAdd.IsEnabled = False
            BtnSave.IsEnabled = True
            BtnDelete.IsEnabled = True
        End Sub

        Private Function ReadForm(ByRef vehicleId As Integer, ByRef cost As Decimal, ByRef status As String) As String
            If CmbVehicle.SelectedValue Is Nothing Then Return "Choose a vehicle."
            vehicleId = Convert.ToInt32(CmbVehicle.SelectedValue)

            If String.IsNullOrWhiteSpace(CmbService.Text) Then Return "Enter the service type."
            If Not DpDate.SelectedDate.HasValue Then Return "Pick the scheduled date."
            If Not ViewHelpers.TryParseMoney(TxtCost.Text, cost) Then Return "Cost must be a number (0 or more)."

            status = TryCast(CmbStatus.SelectedItem, String)
            If String.IsNullOrEmpty(status) Then Return "Choose a status."
            Return Nothing
        End Function

        Private Sub BtnAdd_Click(sender As Object, e As RoutedEventArgs) Handles BtnAdd.Click
            Dim vehicleId As Integer, cost As Decimal, status As String = Nothing
            Dim problem = ReadForm(vehicleId, cost, status)
            If problem IsNot Nothing Then
                AlertUtil.ShowWarning(problem, "Maintenance", ViewHelpers.Owner(Me))
                Return
            End If

            Try
                MaintenanceRepository.Add(vehicleId, CmbService.Text, DpDate.SelectedDate.Value, cost, status, TxtNotes.Text)
                LoadRecords()
                ClearForm()
            Catch ex As Exception
                AlertUtil.ShowError($"Could not add the schedule: {ex.Message}", "Maintenance", ViewHelpers.Owner(Me))
            End Try
        End Sub

        Private Sub BtnSave_Click(sender As Object, e As RoutedEventArgs) Handles BtnSave.Click
            If _selectedId = 0 Then Return

            Dim vehicleId As Integer, cost As Decimal, status As String = Nothing
            Dim problem = ReadForm(vehicleId, cost, status)
            If problem IsNot Nothing Then
                AlertUtil.ShowWarning(problem, "Maintenance", ViewHelpers.Owner(Me))
                Return
            End If

            Try
                problem = MaintenanceRepository.Update(_selectedId, vehicleId, CmbService.Text, DpDate.SelectedDate.Value, cost, status, TxtNotes.Text)
                If problem IsNot Nothing Then
                    AlertUtil.ShowWarning(problem, "Maintenance", ViewHelpers.Owner(Me))
                    Return
                End If
                LoadRecords()
                ClearForm()
            Catch ex As Exception
                AlertUtil.ShowError($"Could not save the schedule: {ex.Message}", "Maintenance", ViewHelpers.Owner(Me))
            End Try
        End Sub

        Private Sub BtnClear_Click(sender As Object, e As RoutedEventArgs) Handles BtnClear.Click
            ClearForm()
        End Sub

        Private Sub BtnDelete_Click(sender As Object, e As RoutedEventArgs) Handles BtnDelete.Click
            If _selectedId = 0 Then Return
            If Not AlertUtil.Confirm("Delete this maintenance record? This can't be undone.", "Delete Record", ViewHelpers.Owner(Me)) Then Return

            Try
                MaintenanceRepository.Delete(_selectedId)
                LoadRecords()
                ClearForm()
            Catch ex As Exception
                AlertUtil.ShowError($"Could not delete the record: {ex.Message}", "Maintenance", ViewHelpers.Owner(Me))
            End Try
        End Sub
    End Class
End Namespace
