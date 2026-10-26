Imports System.Collections.Generic
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports AutoRent.Common.Utilities
Imports AutoRent.Data.Repositories

Namespace Presentation.Views.Admin
    Partial Public Class HomeDashboardView
        Inherits UserControl

        Private _ready As Boolean = False

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub HomeDashboardView_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            If _ready Then Return
            _ready = True

            TxtDate.Text = DateTime.Today.ToString("dddd, MMMM d, yyyy")

            Try
                Dim stats = ReportRepository.GetDashboardStats()
                Dim counts = stats.StatusCounts

                TxtTotal.Text = stats.TotalVehicles.ToString()
                TxtAvailable.Text = counts("Available").ToString()
                TxtOut.Text = (counts("Reserved") + counts("Rented")).ToString()
                TxtMaint.Text = counts("Maintenance").ToString()
                TxtRevenue.Text = stats.RevenueThisMonth.ToString("N2")
                TxtCost.Text = stats.MaintenanceCostThisMonth.ToString("N2")
                TxtOverdue.Text = stats.OverdueMaintenance.ToString()
                TxtOverdue.Foreground = If(stats.OverdueMaintenance > 0, New SolidColorBrush(Color.FromRgb(&HC6, &H28, &H28)), New SolidColorBrush(Color.FromRgb(&H2B, &H29, &H27)))
                TxtUsers.Text = stats.ActiveUsers.ToString()

                BuildStatusBars(counts, stats.TotalVehicles)

                DgUpcoming.ItemsSource = MaintenanceRepository.GetUpcoming(6).DefaultView
                DgRecent.ItemsSource = ReportRepository.GetRecentReservations(8).DefaultView
            Catch ex As Exception
                AlertUtil.ShowError($"Could not load the dashboard: {ex.Message}", "Dashboard", ViewHelpers.Owner(Me))
            End Try
        End Sub

        Private Shared ReadOnly StatusColors As New Dictionary(Of String, Color) From {
            {"Available", Color.FromRgb(&H2E, &H7D, &H32)},
            {"Reserved", Color.FromRgb(&H6A, &H1B, &H9A)},
            {"Rented", Color.FromRgb(&H15, &H65, &HC0)},
            {"Maintenance", Color.FromRgb(&HE6, &H51, &H0)},
            {"Retired", Color.FromRgb(&H9A, &H94, &H8D)}
        }

        ''' <summary>One row per status: label, a bar sized by share of the fleet, and the count.</summary>
        Private Sub BuildStatusBars(counts As Dictionary(Of String, Integer), total As Integer)
            StatusBars.Children.Clear()

            If total = 0 Then
                StatusBars.Children.Add(New TextBlock With {
                    .Text = "No vehicles yet. Add some in Fleet Management.",
                    .Foreground = New SolidColorBrush(Color.FromRgb(&H7A, &H74, &H6D)),
                    .FontSize = 13,
                    .TextWrapping = TextWrapping.Wrap
                })
                Return
            End If

            For Each status In VehicleRepository.Statuses
                Dim n = counts(status)

                Dim rowGrid As New Grid With {.Margin = New Thickness(0, 0, 0, 14)}
                rowGrid.ColumnDefinitions.Add(New ColumnDefinition With {.Width = New GridLength(100)})
                rowGrid.ColumnDefinitions.Add(New ColumnDefinition With {.Width = New GridLength(1, GridUnitType.Star)})
                rowGrid.ColumnDefinitions.Add(New ColumnDefinition With {.Width = New GridLength(36)})

                Dim label As New TextBlock With {
                    .Text = status, .FontSize = 13, .VerticalAlignment = VerticalAlignment.Center,
                    .Foreground = New SolidColorBrush(Color.FromRgb(&H2B, &H29, &H27))
                }
                Grid.SetColumn(label, 0)

                Dim track As New Border With {
                    .Height = 10, .CornerRadius = New CornerRadius(5),
                    .Background = New SolidColorBrush(Color.FromRgb(&HEF, &HED, &HEA)),
                    .ClipToBounds = True
                }
                Dim fillGrid As New Grid()
                fillGrid.ColumnDefinitions.Add(New ColumnDefinition With {.Width = New GridLength(n, GridUnitType.Star)})
                fillGrid.ColumnDefinitions.Add(New ColumnDefinition With {.Width = New GridLength(total - n, GridUnitType.Star)})
                Dim fill As New Border With {.Background = New SolidColorBrush(StatusColors(status)), .CornerRadius = New CornerRadius(5)}
                Grid.SetColumn(fill, 0)
                fillGrid.Children.Add(fill)
                track.Child = fillGrid
                Grid.SetColumn(track, 1)

                Dim count As New TextBlock With {
                    .Text = n.ToString(), .FontSize = 13, .FontWeight = FontWeights.SemiBold,
                    .HorizontalAlignment = HorizontalAlignment.Right, .VerticalAlignment = VerticalAlignment.Center,
                    .Foreground = New SolidColorBrush(Color.FromRgb(&H2B, &H29, &H27))
                }
                Grid.SetColumn(count, 2)

                rowGrid.Children.Add(label)
                rowGrid.Children.Add(track)
                rowGrid.Children.Add(count)
                StatusBars.Children.Add(rowGrid)
            Next
        End Sub
    End Class
End Namespace
