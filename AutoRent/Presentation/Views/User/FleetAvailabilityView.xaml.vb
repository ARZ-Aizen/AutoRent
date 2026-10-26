Imports System.Linq
Imports System.Windows
Imports System.Windows.Controls

Namespace Presentation.Views.User
    Partial Public Class FleetAvailabilityView
        Inherits UserControl

        Private _fleet As New List(Of FleetItem)
        Private _initialized As Boolean = False

        Public Sub New()
            InitializeComponent()
            _initialized = True
        End Sub

        Private Sub FleetAvailabilityView_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Try
                _fleet = LoadFleet()
            Catch ex As Exception
                ShowDbError("load the fleet", ex)
                Return
            End Try
            ApplyFleetFilter()
        End Sub

        Private Sub TxtFleetSearch_TextChanged(sender As Object, e As TextChangedEventArgs)
            If Not _initialized Then Return
            ApplyFleetFilter()
        End Sub

        Private Sub CmbFleetStatus_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            If Not _initialized Then Return
            ApplyFleetFilter()
        End Sub

        Private Sub ApplyFleetFilter()
            Dim q As String = TxtFleetSearch.Text.Trim().ToLowerInvariant()
            Dim status As String = If(TryCast(CmbFleetStatus.SelectedItem, ComboBoxItem)?.Content?.ToString(), "All")

            Dim result = _fleet.Where(Function(f) (status = "All" OrElse f.Status = status) AndAlso
                                          (q = "" OrElse
                                           f.Plate.ToLowerInvariant().Contains(q) OrElse
                                           f.Model.ToLowerInvariant().Contains(q) OrElse
                                           f.Category.ToLowerInvariant().Contains(q))).ToList()

            DgFleet.ItemsSource = result
            TxtFleetCount.Text = $"{result.Count} of {_fleet.Count} vehicles"
        End Sub
    End Class
End Namespace