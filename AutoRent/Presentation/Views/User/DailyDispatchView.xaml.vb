Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Effects
Imports AutoRent.Data.Context
Imports AutoRent.Data.Repositories
Imports Microsoft.Win32

Namespace Presentation.Views.User
    Partial Public Class DailyDispatchView
        Inherits UserControl

        Private Const PageSize As Integer = 10

        Private _dispatch As New List(Of DispatchItem)
        Private _filtered As New List(Of DispatchItem)
        Private _page As Integer = 1
        Private _totalPages As Integer = 1
        Private _initialized As Boolean = False

        Public Sub New()
            InitializeComponent()
            _initialized = True
        End Sub

        Private Sub DailyDispatchView_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            CmbDispatchType.ItemsSource = Nothing
            CmbDispatchType.Items.Clear()
            CmbDispatchType.ItemsSource = New String() {"All Dispatches", "Pickups", "Returns"}
            CmbDispatchType.SelectedIndex = 0

            CmbStatusFilter.ItemsSource = Nothing
            CmbStatusFilter.Items.Clear()
            CmbStatusFilter.ItemsSource = New String() {"All Status", "Scheduled", "Completed"}
            CmbStatusFilter.SelectedIndex = 0

            ReloadDispatch()
        End Sub

        Private Sub ReloadDispatch()
            Try
                _dispatch = LoadDispatch()
            Catch ex As Exception
                ShowDbError("load the dispatch board", ex)
                Return
            End Try

            UpdateStatCards()
            ApplyDispatchFilter()
        End Sub

        Private Sub UpdateStatCards()
            Dim total = _dispatch.Count
            Dim pickups = _dispatch.Where(Function(d) String.Equals(d.DispatchType, "Pickup", StringComparison.OrdinalIgnoreCase)).Count()
            Dim returns = _dispatch.Where(Function(d) String.Equals(d.DispatchType, "Return", StringComparison.OrdinalIgnoreCase)).Count()
            Dim completed = _dispatch.Where(Function(d) String.Equals(d.Status, "Completed", StringComparison.OrdinalIgnoreCase)).Count()

            TxtStatTotal.Text = total.ToString()
            TxtStatPickups.Text = pickups.ToString()
            TxtStatReturns.Text = returns.ToString()
            TxtStatCompleted.Text = completed.ToString()
            TxtDispatchSubtitle.Text = $"Today, {Date.Today:MMMM d, yyyy}  •  {pickups} pickups, {returns} returns scheduled"
        End Sub

        Private Sub TxtSearchDispatch_TextChanged(sender As Object, e As TextChangedEventArgs)
            If Not _initialized Then Return
            TxtSearchWatermark.Visibility = If(String.IsNullOrEmpty(TxtSearchDispatch.Text), Visibility.Visible, Visibility.Collapsed)
            _page = 1
            ApplyDispatchFilter()
        End Sub

        Private Sub FilterInputs_Changed(sender As Object, e As SelectionChangedEventArgs)
            If Not _initialized Then Return
            _page = 1
            ApplyDispatchFilter()
        End Sub

        Private Sub BtnResetFilters_Click(sender As Object, e As RoutedEventArgs)
            TxtSearchDispatch.Text = String.Empty
            TxtSearchWatermark.Visibility = Visibility.Visible
            CmbDispatchType.SelectedIndex = 0
            CmbStatusFilter.SelectedIndex = 0
            _page = 1
            ApplyDispatchFilter()
        End Sub

        Private Sub BtnRefresh_Click(sender As Object, e As RoutedEventArgs)
            ReloadDispatch()
        End Sub

        Private Sub ApplyDispatchFilter()
            If _dispatch Is Nothing Then Return

            Dim q As String = TxtSearchDispatch.Text.Trim().ToLowerInvariant()
            Dim typeFilter As String = If(TryCast(CmbDispatchType.SelectedItem, String), "All Dispatches")
            Dim statusFilter As String = If(TryCast(CmbStatusFilter.SelectedItem, String), "All Status")

            _filtered = _dispatch.Where(Function(d)
                                            Dim matchQuery = (q = "" OrElse
                                                (d.Customer IsNot Nothing AndAlso d.Customer.ToLowerInvariant().Contains(q)) OrElse
                                                (d.Vehicle IsNot Nothing AndAlso d.Vehicle.ToLowerInvariant().Contains(q)) OrElse
                                                (d.Plate IsNot Nothing AndAlso d.Plate.ToLowerInvariant().Contains(q)) OrElse
                                                (d.Location IsNot Nothing AndAlso d.Location.ToLowerInvariant().Contains(q)))

                                            Dim matchType = True
                                            If typeFilter = "Pickups" Then
                                                matchType = String.Equals(d.DispatchType, "Pickup", StringComparison.OrdinalIgnoreCase)
                                            ElseIf typeFilter = "Returns" Then
                                                matchType = String.Equals(d.DispatchType, "Return", StringComparison.OrdinalIgnoreCase)
                                            End If

                                            Dim matchStatus = True
                                            If statusFilter = "Scheduled" Then
                                                matchStatus = String.Equals(d.Status, "Scheduled", StringComparison.OrdinalIgnoreCase)
                                            ElseIf statusFilter = "Completed" Then
                                                matchStatus = String.Equals(d.Status, "Completed", StringComparison.OrdinalIgnoreCase)
                                            End If

                                            Return matchQuery AndAlso matchType AndAlso matchStatus
                                        End Function).ToList()

            _totalPages = Math.Max(1, CInt(Math.Ceiling(_filtered.Count / CDbl(PageSize))))
            If _page > _totalPages Then _page = _totalPages
            If _page < 1 Then _page = 1

            ShowPage()
        End Sub

        Private Sub ShowPage()
            Dim pageItems = _filtered.Skip((_page - 1) * PageSize).Take(PageSize).ToList()
            DgDispatch.ItemsSource = pageItems

            If _filtered.Count = 0 Then
                DgDispatch.Visibility = Visibility.Collapsed
                BorderEmptyState.Visibility = Visibility.Visible
                TxtPaging.Text = "NO DISPATCH EVENTS FOUND"
            Else
                DgDispatch.Visibility = Visibility.Visible
                BorderEmptyState.Visibility = Visibility.Collapsed
                TxtPaging.Text = $"SHOWING {pageItems.Count} OF {_filtered.Count} DISPATCHES"
            End If

            BtnPrev.IsEnabled = (_page > 1)
            BtnNext.IsEnabled = (_page < _totalPages)

            BuildPageButtons()
        End Sub

        Private Sub BuildPageButtons()
            IcPages.Items.Clear()

            For i = 1 To _totalPages
                Dim btn As New Button()
                btn.Width = 38
                btn.Height = 38
                btn.FontSize = 13
                btn.FontWeight = FontWeights.Bold
                btn.Cursor = Cursors.Hand
                btn.Content = i.ToString()
                btn.Margin = New Thickness(0, 0, 8, 0)
                btn.Tag = i

                If i = _page Then
                    btn.Template = CreatePageTemplate("#70000F", "White", True)
                    btn.Foreground = Brushes.White
                Else
                    btn.Template = CreatePageTemplate("White", "#7A746D", False)
                    btn.Foreground = New SolidColorBrush(CType(ColorConverter.ConvertFromString("#7A746D"), Color))
                End If

                AddHandler btn.Click, AddressOf PageNumber_Click
                IcPages.Items.Add(btn)
            Next
        End Sub

        Private Function CreatePageTemplate(bgColor As String, fgColor As String, hasShadow As Boolean) As ControlTemplate
            Dim template As New ControlTemplate(GetType(Button))

            Dim borderFactory As New FrameworkElementFactory(GetType(Border))
            borderFactory.SetValue(Border.BackgroundProperty, New SolidColorBrush(CType(ColorConverter.ConvertFromString(bgColor), Color)))
            borderFactory.SetValue(Border.CornerRadiusProperty, New CornerRadius(10))

            If hasShadow Then
                borderFactory.SetValue(Border.BorderThicknessProperty, New Thickness(0))
                Dim shadow As New DropShadowEffect With {
                    .BlurRadius = 8,
                    .ShadowDepth = 2,
                    .Direction = 270,
                    .Opacity = 0.2,
                    .Color = CType(ColorConverter.ConvertFromString(bgColor), Color)
                }
                borderFactory.SetValue(Border.EffectProperty, shadow)
            Else
                borderFactory.SetValue(Border.BorderBrushProperty, New SolidColorBrush(CType(ColorConverter.ConvertFromString("#E8E5E1"), Color)))
                borderFactory.SetValue(Border.BorderThicknessProperty, New Thickness(1))
            End If

            Dim contentFactory As New FrameworkElementFactory(GetType(ContentPresenter))
            contentFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center)
            contentFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center)
            borderFactory.AppendChild(contentFactory)

            template.VisualTree = borderFactory
            Return template
        End Function

        Private Sub PageNumber_Click(sender As Object, e As RoutedEventArgs)
            Dim btn = TryCast(sender, Button)
            If btn Is Nothing Then Return
            Dim targetPage As Integer = Convert.ToInt32(btn.Tag)
            If targetPage <> _page Then
                _page = targetPage
                ShowPage()
            End If
        End Sub

        Private Sub BtnPrev_Click(sender As Object, e As RoutedEventArgs)
            If _page > 1 Then
                _page -= 1
                ShowPage()
            End If
        End Sub

        Private Sub BtnNext_Click(sender As Object, e As RoutedEventArgs)
            If _page < _totalPages Then
                _page += 1
                ShowPage()
            End If
        End Sub

        ' =========================
        ' COMPLETE HANDOFF ACTION
        ' =========================

        Private Sub BtnCompleteDispatch_Click(sender As Object, e As RoutedEventArgs)
            Dim item = TryCast(CType(sender, FrameworkElement).Tag, DispatchItem)
            If item Is Nothing Then Return

            If String.Equals(item.Status, "Completed", StringComparison.OrdinalIgnoreCase) Then Return

            ' If this is a Pickup dispatch, verify customer has a valid driver's license on file
            If item.IsPickup Then
                Dim licTable = Db.Query("SELECT c.[license_no] FROM [Customers] AS c INNER JOIN [Reservations] AS r ON c.[full_name] = r.[customer_name] WHERE r.[reservation_id] = ?", Db.Int(item.ReservationId))
                If licTable.Rows.Count > 0 Then
                    Dim lic = If(IsDBNull(licTable.Rows(0)("license_no")), "", Convert.ToString(licTable.Rows(0)("license_no")).Trim())
                    If String.IsNullOrWhiteSpace(lic) Then
                        AlertUtil.ShowWarning($"Cannot dispatch vehicle to {item.Customer}.{vbCrLf}{vbCrLf}" &
                                        "The customer does not have a verified driver's license on file." & vbCrLf &
                                        "Vehicles cannot be rented or dispatched without a valid driver's license." & vbCrLf &
                                        "Please update their customer profile under Customer Management before dispatching.",
                                        "Driver License Required", ViewHelpers.Owner(Me))
                        Return
                    End If
                End If
            End If

            If Not AlertUtil.Confirm($"Mark {item.DispatchType.ToLower()} handoff for {item.Customer} ({item.Plate}) as Completed?" & vbCrLf &
                                       If(item.IsPickup, "Vehicle will be set to Rented and reservation set to Active.", "Vehicle will be returned to Available and reservation completed."),
                                       "Complete Handoff", ViewHelpers.Owner(Me)) Then Return

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
