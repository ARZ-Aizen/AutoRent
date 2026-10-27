Imports System.Linq
Imports System.Windows
Imports System.Windows.Controls

Namespace Presentation.Views.User
    Partial Public Class ClientHomeView
        Inherits UserControl

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub ClientHomeView_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            LoadDashboard()
        End Sub

        Private Sub BtnRefresh_Click(sender As Object, e As RoutedEventArgs)
            LoadDashboard()
        End Sub

        Private Sub LoadDashboard()
            Try
                Dim fleet = LoadFleet()
                Dim reservations = LoadReservations()
                Dim dispatch = LoadDispatch()

                ' Today's date
                TxtHomeDate.Text = $"{Date.Today:dddd, MMMM d, yyyy}  •  AutoRent Client Portal Operations"

                ' Metric counts
                Dim totalFleet = fleet.Count
                Dim availableCount = fleet.Where(Function(f) String.Equals(f.Status, "Available", StringComparison.OrdinalIgnoreCase)).Count()
                Dim reservedCount = fleet.Where(Function(f) String.Equals(If(f.Status, "").Trim(), "Reserved", StringComparison.OrdinalIgnoreCase)).Count()
                Dim rentedCount = fleet.Where(Function(f) String.Equals(f.Status, "Rented", StringComparison.OrdinalIgnoreCase)).Count()
                Dim maintCount = fleet.Where(Function(f)
                                                 Dim s = If(f.Status, "").Trim()
                                                 Return String.Equals(s, "Maintenance", StringComparison.OrdinalIgnoreCase) OrElse
                                                        String.Equals(s, "In Maintenance", StringComparison.OrdinalIgnoreCase) OrElse
                                                        String.Equals(s, "Under Maintenance", StringComparison.OrdinalIgnoreCase)
                                             End Function).Count()
                Dim pendingDispatchCount = dispatch.Where(Function(d) String.Equals(d.Status, "Scheduled", StringComparison.OrdinalIgnoreCase)).Count()

                ' Client KPIs focus on bookings; vehicle status is shown in Fleet Readiness.
                Dim confirmedBookings = reservations.Where(Function(r) String.Equals(r.Status, "Confirmed", StringComparison.OrdinalIgnoreCase)).Count()
                Dim activeBookings = reservations.Where(Function(r) String.Equals(r.Status, "Active", StringComparison.OrdinalIgnoreCase)).Count()
                Dim completedBookings = reservations.Where(Function(r) String.Equals(r.Status, "Completed", StringComparison.OrdinalIgnoreCase)).Count()
                Dim cancelledBookings = reservations.Where(Function(r) String.Equals(r.Status, "Cancelled", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(r.Status, "Canceled", StringComparison.OrdinalIgnoreCase)).Count()
                TxtBookingsTotal.Text = reservations.Count.ToString("N0")
                TxtBookingsConfirmed.Text = confirmedBookings.ToString("N0")
                TxtBookingsActive.Text = activeBookings.ToString("N0")
                TxtBookingsCompleted.Text = completedBookings.ToString("N0")
                TxtBookingsCancelled.Text = cancelledBookings.ToString("N0")

                ' Fleet readiness breakdown
                TxtCountAvailable.Text = $"{availableCount} vehicles"
                TxtCountReserved.Text = $"{reservedCount} vehicles"
                TxtCountRented.Text = $"{rentedCount} vehicles"
                TxtCountMaintenance.Text = $"{maintCount} vehicles"

                Dim availPct = If(totalFleet > 0, Math.Round((availableCount / CDbl(totalFleet)) * 100, 1), 0)
                TxtFleetAvailRate.Text = $"{availPct}% Available"

                ' Segmented distribution bar widths
                ColBarAvailable.Width = New GridLength(Math.Max(0.01, availableCount), GridUnitType.Star)
                ColBarReserved.Width = New GridLength(Math.Max(0.01, reservedCount), GridUnitType.Star)
                ColBarRented.Width = New GridLength(Math.Max(0.01, rentedCount), GridUnitType.Star)
                ColBarMaintenance.Width = New GridLength(Math.Max(0.01, maintCount), GridUnitType.Star)

                ' Dispatch activity
                Dim pickups = dispatch.Where(Function(d) String.Equals(d.DispatchType, "Pickup", StringComparison.OrdinalIgnoreCase)).Count()
                Dim returns = dispatch.Where(Function(d) String.Equals(d.DispatchType, "Return", StringComparison.OrdinalIgnoreCase)).Count()
                TxtCountPickups.Text = pickups.ToString()
                TxtCountReturns.Text = returns.ToString()
                TxtDispatchSummary.Text = $"{dispatch.Count} scheduled handoffs today"

                If pendingDispatchCount = 0 Then
                    TxtDispatchStatusNote.Text = "All scheduled vehicle dispatches are completed."
                Else
                    TxtDispatchStatusNote.Text = $"{pendingDispatchCount} dispatch(es) pending completion today."
                End If

                ' Recent reservations
                DgRecent.ItemsSource = reservations.Take(6).ToList()
            Catch ex As Exception
                ShowDbError("load the dashboard", ex)
            End Try
        End Sub
    End Class
End Namespace