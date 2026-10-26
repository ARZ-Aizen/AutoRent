Imports System.Linq
Imports System.Windows
Imports System.Windows.Controls
Imports AutoRent.Data.Repositories

Namespace Presentation.Views.User
    Partial Public Class DailyDispatchView
        Inherits UserControl

        Private _dispatch As New List(Of DispatchItem)
        Private _initialized As Boolean = False

        Public Sub New()
            InitializeComponent()
            _initialized = True
        End Sub

        Private Sub DailyDispatchView_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            ReloadDispatch()
        End Sub

        Private Sub ReloadDispatch()
            Try
                _dispatch = LoadDispatch()
            Catch ex As Exception
                ShowDbError("load the dispatch board", ex)
                Return
            End Try
            ApplyDispatchFilter()
        End Sub

        Private Sub CmbDispatchType_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            If Not _initialized Then Return
            ApplyDispatchFilter()
        End Sub

        Private Sub ApplyDispatchFilter()
            Dim kind As String = If(TryCast(CmbDispatchType.SelectedItem, ComboBoxItem)?.Content?.ToString(), "All")

            DgDispatch.ItemsSource = _dispatch.Where(Function(d) kind = "All" OrElse d.DispatchType = kind).ToList()

            Dim pickups = _dispatch.Where(Function(d) d.DispatchType = "Pickup").Count()
            Dim returns = _dispatch.Where(Function(d) d.DispatchType = "Return").Count()
            Dim pending = _dispatch.Where(Function(d) d.Status = "Scheduled").Count()

            TxtDispatchSummary.Text = $"Today, {Date.Today:MMMM d}  •  {pickups} pickups  •  {returns} returns  •  {pending} pending"
        End Sub

        Private Sub BtnMarkDispatchDone_Click(sender As Object, e As RoutedEventArgs)
            Dim item = TryCast(DgDispatch.SelectedItem, DispatchItem)
            If item Is Nothing Then
                MessageBox.Show("Select a dispatch first.", "Daily Dispatch",
                                MessageBoxButton.OK, MessageBoxImage.Information)
                Return
            End If

            If item.Status = "Completed" Then Return

            Try
                ClientRepository.CompleteDispatch(item.DbId, item.ReservationId, item.VehicleId, item.DispatchType)
            Catch ex As Exception
                ShowDbError("update the dispatch", ex)
                Return
            End Try
            ReloadDispatch()
        End Sub
    End Class
End Namespace