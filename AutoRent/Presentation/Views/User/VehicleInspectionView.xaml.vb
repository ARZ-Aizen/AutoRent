Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Effects
Imports AutoRent.Data.Repositories
Imports Microsoft.Win32

Namespace Presentation.Views.User
    Partial Public Class VehicleInspectionView
        Inherits UserControl

        Private Const PageSize As Integer = 10

        Private _fleet As New List(Of FleetItem)
        Private _inspections As New List(Of InspectionItem)
        Private _filtered As New List(Of InspectionItem)
        Private _page As Integer = 1
        Private _totalPages As Integer = 1
        Private _initialized As Boolean = False

        Public Sub New()
            InitializeComponent()
            _initialized = True
        End Sub

        Private Sub VehicleInspectionView_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            CmbFuel.ItemsSource = Nothing
            CmbFuel.Items.Clear()
            CmbFuel.ItemsSource = New String() {"Full", "3/4", "1/2", "1/4", "Empty"}
            CmbFuel.SelectedIndex = 0

            CmbResultFilter.ItemsSource = Nothing
            CmbResultFilter.Items.Clear()
            CmbResultFilter.ItemsSource = New String() {"All Results", "Pass", "Needs Attention"}
            CmbResultFilter.SelectedIndex = 0

            UpdateChecklistBanner()
            ReloadData()
        End Sub

        Private Sub ReloadData()
            Try
                _fleet = LoadFleet()
                _inspections = LoadInspections()
            Catch ex As Exception
                ShowDbError("load inspections", ex)
                Return
            End Try

            CmbInspVehicle.ItemsSource = _fleet
            UpdateStatCards()
            ApplyInspectionFilter()
        End Sub

        Private Sub UpdateStatCards()
            Dim total = _inspections.Count
            Dim passed = _inspections.Where(Function(i) i.IsPass).Count()
            Dim failed = _inspections.Where(Function(i) Not i.IsPass).Count()
            Dim maint = _fleet.Where(Function(f)
                                         Dim s = If(f.Status, "").Trim()
                                         Return String.Equals(s, "Maintenance", StringComparison.OrdinalIgnoreCase) OrElse
                                                String.Equals(s, "In Maintenance", StringComparison.OrdinalIgnoreCase) OrElse
                                                String.Equals(s, "Under Maintenance", StringComparison.OrdinalIgnoreCase)
                                     End Function).Count()

            TxtStatTotal.Text = total.ToString()
            TxtStatPassed.Text = passed.ToString()
            TxtStatFailed.Text = failed.ToString()
            TxtStatMaint.Text = maint.ToString()
        End Sub

        Private Sub Checklist_Changed(sender As Object, e As RoutedEventArgs)
            UpdateChecklistBanner()
        End Sub

        Private Sub UpdateChecklistBanner()
            If BorderCheckStatus Is Nothing OrElse TxtCheckStatusText Is Nothing Then Return

            Dim checks As CheckBox() = {ChkExterior, ChkTires, ChkLights, ChkInterior, ChkBrakes, ChkFluids}
            Dim allChecked = checks.All(Function(c) c.IsChecked.GetValueOrDefault())

            If allChecked Then
                BorderCheckStatus.Background = New SolidColorBrush(CType(ColorConverter.ConvertFromString("#E6F4EA"), Color))
                BorderCheckStatus.BorderBrush = New SolidColorBrush(CType(ColorConverter.ConvertFromString("#CEEAD6"), Color))
                TxtCheckStatusIcon.Text = ChrW(&HE73E)
                TxtCheckStatusIcon.Foreground = New SolidColorBrush(CType(ColorConverter.ConvertFromString("#137333"), Color))
                TxtCheckStatusText.Text = "✓ All safety checks passed. Roadworthy status will be confirmed."
                TxtCheckStatusText.Foreground = New SolidColorBrush(CType(ColorConverter.ConvertFromString("#137333"), Color))
            Else
                BorderCheckStatus.Background = New SolidColorBrush(CType(ColorConverter.ConvertFromString("#FDF2F2"), Color))
                BorderCheckStatus.BorderBrush = New SolidColorBrush(CType(ColorConverter.ConvertFromString("#F5C2C7"), Color))
                TxtCheckStatusIcon.Text = ChrW(&HE7BA)
                TxtCheckStatusIcon.Foreground = New SolidColorBrush(CType(ColorConverter.ConvertFromString("#C62828"), Color))
                TxtCheckStatusText.Text = "⚠️ Unchecked items found: Vehicle will be set to Maintenance."
                TxtCheckStatusText.Foreground = New SolidColorBrush(CType(ColorConverter.ConvertFromString("#C62828"), Color))
            End If
        End Sub

        Private Sub BtnSaveInspection_Click(sender As Object, e As RoutedEventArgs)
            Dim veh = TryCast(CmbInspVehicle.SelectedItem, FleetItem)
            Dim inspector As String = TxtInspector.Text.Trim()

            If veh Is Nothing Then
                MessageBox.Show("Please select a target vehicle for inspection.", "Vehicle Inspection",
                                MessageBoxButton.OK, MessageBoxImage.Warning)
                Return
            End If

            If String.IsNullOrWhiteSpace(inspector) Then
                MessageBox.Show("Please enter the inspector's name.", "Vehicle Inspection",
                                MessageBoxButton.OK, MessageBoxImage.Warning)
                TxtInspector.Focus()
                Return
            End If

            Dim checks As CheckBox() = {ChkExterior, ChkTires, ChkLights, ChkInterior, ChkBrakes, ChkFluids}
            Dim failed = checks.Where(Function(c) Not c.IsChecked.GetValueOrDefault()) _
                               .Select(Function(c) c.Content.ToString()).ToList()

            Dim passed As Boolean = (failed.Count = 0)
            Dim fuel As String = If(TryCast(CmbFuel.SelectedItem, String), "Full")

            Dim findings As String = If(passed, "All checks passed", "Defects: " & String.Join(", ", failed))
            Dim notes As String = TxtInspNotes.Text.Trim()
            If notes <> "" Then findings &= " | " & notes

            Try
                ClientRepository.AddInspection(veh.DbId, inspector, fuel, findings, passed)
            Catch ex As Exception
                ShowDbError("save inspection", ex)
                Return
            End Try

            If passed Then
                MessageBox.Show($"Inspection saved successfully: Pass.{vbCrLf}Vehicle '{veh.Display}' remains Available.",
                                "Vehicle Inspection", MessageBoxButton.OK, MessageBoxImage.Information)
            Else
                MessageBox.Show($"Inspection saved: Needs Attention.{vbCrLf}Vehicle '{veh.Display}' has been flagged and placed in Maintenance.",
                                "Vehicle Inspection", MessageBoxButton.OK, MessageBoxImage.Warning)
            End If

            For Each c In checks
                c.IsChecked = False
            Next
            TxtInspNotes.Clear()
            CmbInspVehicle.SelectedIndex = -1
            UpdateChecklistBanner()
            ReloadData()
        End Sub

        Private Sub TxtSearchInsp_TextChanged(sender As Object, e As TextChangedEventArgs)
            If Not _initialized Then Return
            TxtSearchWatermark.Visibility = If(String.IsNullOrEmpty(TxtSearchInsp.Text), Visibility.Visible, Visibility.Collapsed)
            _page = 1
            ApplyInspectionFilter()
        End Sub

        Private Sub FilterInputs_Changed(sender As Object, e As SelectionChangedEventArgs)
            If Not _initialized Then Return
            _page = 1
            ApplyInspectionFilter()
        End Sub

        Private Sub BtnResetFilters_Click(sender As Object, e As RoutedEventArgs)
            TxtSearchInsp.Text = String.Empty
            TxtSearchWatermark.Visibility = Visibility.Visible
            CmbResultFilter.SelectedIndex = 0
            _page = 1
            ApplyInspectionFilter()
        End Sub

        Private Sub ApplyInspectionFilter()
            If _inspections Is Nothing Then Return

            Dim q As String = TxtSearchInsp.Text.Trim().ToLowerInvariant()
            Dim resultFilter As String = If(TryCast(CmbResultFilter.SelectedItem, String), "All Results")

            _filtered = _inspections.Where(Function(i)
                                               Dim matchQuery = (q = "" OrElse
                                                   (i.Vehicle IsNot Nothing AndAlso i.Vehicle.ToLowerInvariant().Contains(q)) OrElse
                                                   (i.Inspector IsNot Nothing AndAlso i.Inspector.ToLowerInvariant().Contains(q)) OrElse
                                                   (i.Findings IsNot Nothing AndAlso i.Findings.ToLowerInvariant().Contains(q)) OrElse
                                                   (i.Fuel IsNot Nothing AndAlso i.Fuel.ToLowerInvariant().Contains(q)))

                                               Dim matchResult = True
                                               If resultFilter = "Pass" Then
                                                   matchResult = i.IsPass
                                               ElseIf resultFilter = "Needs Attention" Then
                                                   matchResult = Not i.IsPass
                                               End If

                                               Return matchQuery AndAlso matchResult
                                           End Function).ToList()

            _totalPages = Math.Max(1, CInt(Math.Ceiling(_filtered.Count / CDbl(PageSize))))
            If _page > _totalPages Then _page = _totalPages
            If _page < 1 Then _page = 1

            ShowPage()
        End Sub

        Private Sub ShowPage()
            Dim pageItems = _filtered.Skip((_page - 1) * PageSize).Take(PageSize).ToList()
            DgInspections.ItemsSource = pageItems

            If _filtered.Count = 0 Then
                DgInspections.Visibility = Visibility.Collapsed
                BorderEmptyState.Visibility = Visibility.Visible
                TxtPaging.Text = "NO INSPECTION LOGS FOUND"
            Else
                DgInspections.Visibility = Visibility.Visible
                BorderEmptyState.Visibility = Visibility.Collapsed
                TxtPaging.Text = $"SHOWING {pageItems.Count} OF {_filtered.Count} INSPECTIONS"
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
        ' CSV EXPORT
        ' =========================

        Private Sub BtnExportCsv_Click(sender As Object, e As RoutedEventArgs)
            If _filtered Is Nothing OrElse _filtered.Count = 0 Then
                MessageBox.Show("There are no inspection records to export.", "Export CSV", MessageBoxButton.OK, MessageBoxImage.Information)
                Return
            End If

            Dim dialog As New SaveFileDialog() With {
                .FileName = $"AutoRent_Inspections_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
                .DefaultExt = ".csv",
                .Filter = "CSV documents (.csv)|*.csv"
            }

            If dialog.ShowDialog() = True Then
                Try
                    Dim sb As New StringBuilder()
                    sb.AppendLine("Inspected Date,Vehicle,Inspector,Fuel Gauge,Result,Findings")

                    For Each insp In _filtered
                        Dim dt = EscapeCsv(insp.DateDisplay)
                        Dim veh = EscapeCsv(insp.Vehicle)
                        Dim inspName = EscapeCsv(insp.Inspector)
                        Dim fuel = EscapeCsv(insp.FuelDisplay)
                        Dim res = EscapeCsv(insp.ResultUpper)
                        Dim findings = EscapeCsv(insp.Findings)
                        sb.AppendLine($"{dt},{veh},{inspName},{fuel},{res},{findings}")
                    Next

                    File.WriteAllText(dialog.FileName, sb.ToString(), Encoding.UTF8)
                    MessageBox.Show($"Successfully exported {_filtered.Count} inspection records to CSV.", "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information)
                Catch ex As Exception
                    MessageBox.Show($"Could not export CSV: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error)
                End Try
            End If
        End Sub

        Private Function EscapeCsv(val As String) As String
            If String.IsNullOrEmpty(val) Then Return """" & """"
            If val.Contains(",") OrElse val.Contains("""") OrElse val.Contains(vbCr) OrElse val.Contains(vbLf) Then
                Return """" & val.Replace("""", """""") & """"
            End If
            Return """" & val & """"
        End Function
    End Class
End Namespace