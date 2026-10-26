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
            Try
                Dim fleet = LoadFleet()
                Dim reservations = LoadReservations()
                Dim dispatch = LoadDispatch()

                TxtHomeDate.Text = Date.Today.ToString("dddd, MMMM d, yyyy")
                TxtStatTotal.Text = fleet.Count.ToString()
                TxtStatAvailable.Text = fleet.Where(Function(f) f.Status = "Available").Count().ToString()
                TxtStatReservations.Text = reservations.Where(Function(r) r.Status = "Confirmed" OrElse r.Status = "Active").Count().ToString()
                TxtStatDispatch.Text = dispatch.Where(Function(d) d.Status = "Scheduled").Count().ToString()
                DgRecent.ItemsSource = reservations.Take(5).ToList()
            Catch ex As Exception
                ShowDbError("load the dashboard", ex)
            End Try
        End Sub
    End Class
End Namespace