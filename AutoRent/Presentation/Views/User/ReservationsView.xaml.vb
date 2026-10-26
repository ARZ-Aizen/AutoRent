Imports System.Linq
Imports System.Windows
Imports System.Windows.Controls
Imports AutoRent.Data.Repositories

Namespace Presentation.Views.User
    Partial Public Class ReservationsView
        Inherits UserControl

        Private _customers As New List(Of CustomerItem)
        Private _fleet As New List(Of FleetItem)
        Private _reservations As New List(Of ReservationItem)

        Public Sub New()
            InitializeComponent()
            DpPickup.SelectedDate = Date.Today
            DpReturn.SelectedDate = Date.Today.AddDays(1)
        End Sub

        Private Sub ReservationsView_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            ReloadData()
        End Sub

        Private Sub ReloadData()
            Try
                _customers = LoadCustomers()
                _fleet = LoadFleet()
                _reservations = LoadReservations()
            Catch ex As Exception
                ShowDbError("load the reservations", ex)
                Return
            End Try

            CmbResCustomer.ItemsSource = _customers
            CmbResVehicle.ItemsSource = _fleet.Where(Function(f) f.Status = "Available").ToList()
            DgReservations.ItemsSource = _reservations
        End Sub

        Private Sub BtnCreateReservation_Click(sender As Object, e As RoutedEventArgs)
            Dim cust = TryCast(CmbResCustomer.SelectedItem, CustomerItem)
            Dim veh = TryCast(CmbResVehicle.SelectedItem, FleetItem)

            If cust Is Nothing OrElse veh Is Nothing OrElse
               Not DpPickup.SelectedDate.HasValue OrElse Not DpReturn.SelectedDate.HasValue Then
                MessageBox.Show("Please select a customer, an available vehicle, and both dates.",
                                "New Reservation", MessageBoxButton.OK, MessageBoxImage.Warning)
                Return
            End If

            Dim pickup As Date = DpPickup.SelectedDate.Value
            Dim dropoff As Date = DpReturn.SelectedDate.Value

            If dropoff <= pickup Then
                MessageBox.Show("The return date must be after the pickup date.",
                                "New Reservation", MessageBoxButton.OK, MessageBoxImage.Warning)
                Return
            End If

            Dim days As Integer = (dropoff - pickup).Days
            Dim total As Decimal = days * veh.DailyRate

            Try
                Dim problem = ClientRepository.CreateReservation(cust.Name, veh.DbId, pickup, dropoff, total)
                If problem IsNot Nothing Then
                    MessageBox.Show(problem, "New Reservation", MessageBoxButton.OK, MessageBoxImage.Warning)
                    ReloadData()
                    Return
                End If
            Catch ex As Exception
                ShowDbError("create the reservation", ex)
                Return
            End Try

            ReloadData()
            CmbResCustomer.SelectedIndex = -1
            CmbResVehicle.SelectedIndex = -1
        End Sub

        Private Sub BtnCancelReservation_Click(sender As Object, e As RoutedEventArgs)
            Dim res = TryCast(DgReservations.SelectedItem, ReservationItem)
            If Not CanChangeReservation(res) Then Return
            CloseReservation(res, "Cancelled")
        End Sub

        Private Sub BtnCompleteReservation_Click(sender As Object, e As RoutedEventArgs)
            Dim res = TryCast(DgReservations.SelectedItem, ReservationItem)
            If Not CanChangeReservation(res) Then Return
            CloseReservation(res, "Completed")
        End Sub

        Private Function CanChangeReservation(res As ReservationItem) As Boolean
            If res Is Nothing Then
                MessageBox.Show("Select a reservation first.", "Reservations",
                                MessageBoxButton.OK, MessageBoxImage.Information)
                Return False
            End If

            If res.Status = "Cancelled" OrElse res.Status = "Completed" Then
                MessageBox.Show($"Reservation {res.Id} is already {res.Status.ToLower()}.", "Reservations",
                                MessageBoxButton.OK, MessageBoxImage.Information)
                Return False
            End If

            Return True
        End Function

        Private Sub CloseReservation(res As ReservationItem, newStatus As String)
            Try
                ClientRepository.CloseReservation(res.DbId, res.VehicleId, newStatus)
            Catch ex As Exception
                ShowDbError("update the reservation", ex)
                Return
            End Try
            ReloadData()
        End Sub
    End Class
End Namespace