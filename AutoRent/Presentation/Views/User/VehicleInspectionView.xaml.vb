Imports System.Linq
Imports System.Windows
Imports System.Windows.Controls
Imports AutoRent.Data.Repositories

Namespace Presentation.Views.User
    Partial Public Class VehicleInspectionView
        Inherits UserControl

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub VehicleInspectionView_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            ReloadData()
        End Sub

        Private Sub ReloadData()
            Try
                CmbInspVehicle.ItemsSource = LoadFleet()
                DgInspections.ItemsSource = LoadInspections()
            Catch ex As Exception
                ShowDbError("load the inspections", ex)
            End Try
        End Sub

        Private Sub BtnSaveInspection_Click(sender As Object, e As RoutedEventArgs)
            Dim veh = TryCast(CmbInspVehicle.SelectedItem, FleetItem)
            Dim inspector As String = TxtInspector.Text.Trim()

            If veh Is Nothing OrElse inspector = "" Then
                MessageBox.Show("Please select a vehicle and enter the inspector's name.",
                                "Vehicle Inspection", MessageBoxButton.OK, MessageBoxImage.Warning)
                Return
            End If

            Dim checks As CheckBox() = {ChkExterior, ChkTires, ChkLights, ChkInterior, ChkBrakes, ChkFluids}
            Dim failed = checks.Where(Function(c) Not c.IsChecked.GetValueOrDefault()) _
                               .Select(Function(c) c.Content.ToString()).ToList()

            Dim passed As Boolean = (failed.Count = 0)
            Dim fuel As String = If(TryCast(CmbFuel.SelectedItem, ComboBoxItem)?.Content?.ToString(), "")

            Dim findings As String = If(passed, "All checks passed", "Failed: " & String.Join(", ", failed))
            Dim notes As String = TxtInspNotes.Text.Trim()
            If notes <> "" Then findings &= " | " & notes

            Try
                ClientRepository.AddInspection(veh.DbId, inspector, fuel, findings, passed)
            Catch ex As Exception
                ShowDbError("save the inspection", ex)
                Return
            End Try

            If passed Then
                MessageBox.Show("Inspection saved: Pass.", "Vehicle Inspection",
                                MessageBoxButton.OK, MessageBoxImage.Information)
            Else
                MessageBox.Show("Inspection saved: Needs Attention." & Environment.NewLine &
                                "The vehicle was marked as Maintenance.", "Vehicle Inspection",
                                MessageBoxButton.OK, MessageBoxImage.Warning)
            End If

            For Each c In checks
                c.IsChecked = False
            Next
            TxtInspNotes.Clear()
            ReloadData()
        End Sub

        Private Sub DgInspections_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles DgInspections.SelectionChanged

        End Sub
    End Class
End Namespace