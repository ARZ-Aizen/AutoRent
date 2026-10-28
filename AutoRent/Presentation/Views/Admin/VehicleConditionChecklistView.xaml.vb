Imports System.Text.RegularExpressions
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Effects
Imports AutoRent.Common.Utilities
Imports AutoRent.Data.Repositories

Namespace Presentation.Views.Admin

    Partial Public Class VehicleConditionChecklistView
        Inherits UserControl

        Private Const PageSize As Integer = 10

        Private _allChecklists As New List(Of InspectionRecord)()
        Private _filteredChecklists As New List(Of InspectionRecord)()
        Private _vehicleOptions As New List(Of FleetVehicleOption)()
        Private _page As Integer = 1
        Private _totalPages As Integer = 1
        Private _editingId As Integer = 0
        Private _isInitializing As Boolean = True

        Public Sub New()
            InitializeComponent()
            _isInitializing = True
        End Sub

        Private Sub VehicleConditionChecklistView_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            If _isInitializing Then
                InitFilterDropdowns()
                InputBehavior.Attach(TxtModalOdometer, "digits", 8)
                _isInitializing = False
            End If

            ReloadData()
        End Sub

        Private Sub InitFilterDropdowns()
            CmbResultFilter.ItemsSource = New String() {"All Results", "Passed (Roadworthy)", "Needs Attention"}
            CmbResultFilter.SelectedIndex = 0

            CmbTypeFilter.ItemsSource = New String() {"All Types", "Pre-Rental", "Post-Rental", "Routine Maintenance"}
            CmbTypeFilter.SelectedIndex = 0

            CmbModalType.ItemsSource = New String() {"Pre-Rental", "Post-Rental", "Routine Maintenance"}
            CmbModalType.SelectedIndex = 0

            CmbModalFuel.ItemsSource = New String() {"Full", "3/4", "1/2", "1/4", "Empty"}
            CmbModalFuel.SelectedIndex = 0
        End Sub

        Public Sub ReloadData()
            Try
                _allChecklists = InspectionRepository.GetAll()
                _vehicleOptions = InspectionRepository.GetVehicleOptions()
                CmbModalVehicle.ItemsSource = _vehicleOptions

                UpdateStatCards()
                ApplyFilter()
            Catch ex As Exception
                AlertUtil.ShowError("Could not load vehicle condition checklists: " & ex.Message, "Error Loading Checklists", ViewHelpers.Owner(Me))
            End Try
        End Sub

        Private Sub UpdateStatCards()
            Dim stats = InspectionRepository.GetSummaryStats()
            TxtStatTotal.Text = stats.TotalChecklists.ToString("N0")
            TxtStatPassed.Text = stats.PassedCount.ToString("N0")
            TxtStatFailed.Text = stats.AttentionCount.ToString("N0")
            TxtStatMaint.Text = stats.MaintenanceVehiclesCount.ToString("N0")
        End Sub

        ' ==============================================================
        ' FILTRATION & PAGINATION
        ' ==============================================================
        Private Sub Filter_Changed(sender As Object, e As EventArgs)
            If _isInitializing Then Return
            ApplyFilter()
        End Sub

        Private Sub BtnResetFilter_Click(sender As Object, e As RoutedEventArgs)
            TxtSearch.Text = ""
            CmbResultFilter.SelectedIndex = 0
            CmbTypeFilter.SelectedIndex = 0
            ApplyFilter()
        End Sub

        Private Sub ApplyFilter()
            Dim q = TxtSearch.Text?.Trim()
            Dim resultSel = TryCast(CmbResultFilter.SelectedItem, String)
            Dim typeSel = TryCast(CmbTypeFilter.SelectedItem, String)

            _filteredChecklists = InspectionRepository.GetAll(q, resultSel, typeSel)
            _totalPages = Math.Max(1, CInt(Math.Ceiling(_filteredChecklists.Count / CDbl(PageSize))))

            If _page > _totalPages Then _page = _totalPages
            If _page < 1 Then _page = 1

            ShowPage()
        End Sub

        Private Sub ShowPage()
            If _filteredChecklists.Count = 0 Then
                DgChecklists.ItemsSource = Nothing
                DgChecklists.Visibility = Visibility.Collapsed
                BorderEmptyState.Visibility = Visibility.Visible
                TxtPaging.Text = "0 records found"
                BtnPrev.IsEnabled = False
                BtnNext.IsEnabled = False
                IcPages.Items.Clear()
                Return
            End If

            BorderEmptyState.Visibility = Visibility.Collapsed
            DgChecklists.Visibility = Visibility.Visible

            Dim pageItems = _filteredChecklists.Skip((_page - 1) * PageSize).Take(PageSize).ToList()
            DgChecklists.ItemsSource = pageItems

            Dim startIdx = ((_page - 1) * PageSize) + 1
            Dim endIdx = Math.Min(_page * PageSize, _filteredChecklists.Count)
            TxtPaging.Text = $"Showing {startIdx} to {endIdx} of {_filteredChecklists.Count:N0} checklist records"

            BtnPrev.IsEnabled = (_page > 1)
            BtnNext.IsEnabled = (_page < _totalPages)

            BuildPageButtons()
        End Sub

        Private Sub BuildPageButtons()
            IcPages.Items.Clear()

            For i = 1 To _totalPages
                Dim pageNum = i
                Dim btn As New Button With {
                    .Content = pageNum.ToString(),
                    .Width = 32,
                    .Height = 32,
                    .Margin = New Thickness(0, 0, 6, 0),
                    .Tag = pageNum,
                    .Cursor = Cursors.Hand,
                    .FontWeight = FontWeights.SemiBold,
                    .FontSize = 11.5
                }

                If pageNum = _page Then
                    btn.Template = CreatePageTemplate("#70000F", "White", True)
                Else
                    btn.Template = CreatePageTemplate("White", "#5C5750", False)
                End If

                AddHandler btn.Click, AddressOf PageNumber_Click
                IcPages.Items.Add(btn)
            Next
        End Sub

        Private Function CreatePageTemplate(bgColor As String, fgColor As String, hasShadow As Boolean) As ControlTemplate
            Dim template As New ControlTemplate(GetType(Button))

            Dim borderFactory As New FrameworkElementFactory(GetType(Border))
            borderFactory.SetValue(Border.BackgroundProperty, New SolidColorBrush(CType(ColorConverter.ConvertFromString(bgColor), Color)))
            borderFactory.SetValue(Border.CornerRadiusProperty, New CornerRadius(8))

            If hasShadow Then
                borderFactory.SetValue(Border.BorderThicknessProperty, New Thickness(0))
                Dim shadow As New DropShadowEffect With {
                    .BlurRadius = 8,
                    .ShadowDepth = 2,
                    .Direction = 270,
                    .Opacity = 0.25,
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
            Dim targetPage = Convert.ToInt32(btn.Tag)
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

        ' ==============================================================
        ' RECORDING & EDITING CHECKLIST
        ' ==============================================================
        Private Sub BtnNewChecklist_Click(sender As Object, e As RoutedEventArgs)
            _editingId = 0
            TxtModalTitle.Text = "Record Digital Vehicle Condition Checklist"
            TxtModalSubtitle.Text = "Perform 6-point multi-point safety & roadworthiness audit"

            ' New checklists may select any vehicle; hide edit-only locked display.
            TxtModalVehicleLabel.Text = "Vehicle to Inspect *"
            BorderLockedVehicle.Visibility = Visibility.Collapsed
            TxtLockedVehicleNote.Visibility = Visibility.Collapsed
            CmbModalVehicle.Visibility = Visibility.Visible
            CmbModalVehicle.IsEnabled = True
            CmbModalVehicle.SelectedIndex = -1
            TxtModalOdometer.Text = ""
            CmbModalType.SelectedIndex = 0
            CmbModalFuel.SelectedIndex = 0

            ' Autofill logged in user
            Dim app = TryCast(System.Windows.Application.Current, Global.AutoRent.Application)
            If app IsNot Nothing AndAlso app.CurrentUser IsNot Nothing Then
                TxtModalInspector.Text = If(Not String.IsNullOrWhiteSpace(app.CurrentUser.DisplayName), app.CurrentUser.DisplayName, app.CurrentUser.Username)
            Else
                TxtModalInspector.Text = "Staff Inspector"
            End If

            SetAllCheckboxes(True)
            TxtModalFindings.Text = ""

            ' Update odometer from selected vehicle
            UpdateModalOdometerFromSelection()

            UpdateChecklistBanner()
            ModalChecklistForm.Visibility = Visibility.Visible
        End Sub

        Private Sub CmbModalVehicle_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            If _editingId = 0 Then
                UpdateModalOdometerFromSelection()
            End If
        End Sub

        Private Sub UpdateModalOdometerFromSelection()
            Dim selectedVeh = TryCast(CmbModalVehicle.SelectedItem, FleetVehicleOption)
            If selectedVeh IsNot Nothing Then
                TxtModalOdometer.Text = selectedVeh.Mileage.ToString()
            End If
        End Sub

        Private Sub BtnEditChecklist_Click(sender As Object, e As RoutedEventArgs)
            Dim btn = TryCast(sender, Button)
            Dim item = TryCast(btn?.Tag, InspectionRecord)
            If item Is Nothing Then Return

            _editingId = item.InspectionId
            TxtModalTitle.Text = $"Edit Condition Checklist · {item.DisplayId}"
            TxtModalSubtitle.Text = $"Updating checklist record for [{item.VehiclePlate}]"

            ' Find vehicle
            Dim vehOpt = _vehicleOptions.FirstOrDefault(Function(v) v.VehicleId = item.VehicleId)
            If vehOpt Is Nothing Then
                AlertUtil.ShowWarning("The vehicle linked to this checklist could not be found.", "Missing Vehicle", ViewHelpers.Owner(Me))
                Return
            End If
            CmbModalVehicle.SelectedItem = vehOpt
            ' Preserve the inspection's vehicle and show a read-only label instead of
            ' a disabled ComboBox that looks broken or encourages changing history.
            TxtModalVehicleLabel.Text = "Inspected Vehicle"
            TxtLockedVehicleLabel.Text = vehOpt.DisplayText
            CmbModalVehicle.Visibility = Visibility.Collapsed
            CmbModalVehicle.IsEnabled = False
            BorderLockedVehicle.Visibility = Visibility.Visible
            TxtLockedVehicleNote.Visibility = Visibility.Visible

            CmbModalType.SelectedItem = item.ChecklistType
            CmbModalFuel.SelectedItem = item.Fuel
            TxtModalInspector.Text = item.Inspector
            TxtModalOdometer.Text = item.Odometer.ToString()

            ChkExterior.IsChecked = item.ExteriorOk
            ChkTires.IsChecked = item.TiresOk
            ChkLights.IsChecked = item.LightsOk
            ChkInterior.IsChecked = item.InteriorOk
            ChkFluids.IsChecked = item.FluidsOk
            ChkBrakes.IsChecked = item.BrakesOk

            TxtModalFindings.Text = item.Findings

            UpdateChecklistBanner()
            ModalChecklistForm.Visibility = Visibility.Visible
        End Sub

        Private Sub BtnCloseModal_Click(sender As Object, e As RoutedEventArgs)
            ModalChecklistForm.Visibility = Visibility.Collapsed
        End Sub

        Private Sub BtnSelectAllChecks_Click(sender As Object, e As RoutedEventArgs)
            SetAllCheckboxes(True)
        End Sub

        Private Sub SetAllCheckboxes(checked As Boolean)
            ChkExterior.IsChecked = checked
            ChkTires.IsChecked = checked
            ChkLights.IsChecked = checked
            ChkInterior.IsChecked = checked
            ChkFluids.IsChecked = checked
            ChkBrakes.IsChecked = checked
            UpdateChecklistBanner()
        End Sub

        Private Sub Checklist_Changed(sender As Object, e As RoutedEventArgs)
            UpdateChecklistBanner()
        End Sub

        Private Sub UpdateChecklistBanner()
            If BorderCheckStatus Is Nothing OrElse TxtCheckStatusText Is Nothing Then Return

            Dim checks As CheckBox() = {ChkExterior, ChkTires, ChkLights, ChkInterior, ChkFluids, ChkBrakes}
            Dim allChecked = checks.All(Function(c) c.IsChecked.GetValueOrDefault())

            If allChecked Then
                BorderCheckStatus.Background = New SolidColorBrush(CType(ColorConverter.ConvertFromString("#E6F4EA"), Color))
                BorderCheckStatus.BorderBrush = New SolidColorBrush(CType(ColorConverter.ConvertFromString("#CEEAD6"), Color))
                TxtCheckStatusIcon.Text = ChrW(&HE73E)
                TxtCheckStatusIcon.Foreground = New SolidColorBrush(CType(ColorConverter.ConvertFromString("#137333"), Color))
                TxtCheckStatusText.Text = "✓ ALL 6 SAFETY POINTS PASSED — Vehicle certified roadworthy and ready for dispatch."
                TxtCheckStatusText.Foreground = New SolidColorBrush(CType(ColorConverter.ConvertFromString("#137333"), Color))
            Else
                BorderCheckStatus.Background = New SolidColorBrush(CType(ColorConverter.ConvertFromString("#FDF2F2"), Color))
                BorderCheckStatus.BorderBrush = New SolidColorBrush(CType(ColorConverter.ConvertFromString("#F5C2C7"), Color))
                TxtCheckStatusIcon.Text = ChrW(&HE7BA)
                TxtCheckStatusIcon.Foreground = New SolidColorBrush(CType(ColorConverter.ConvertFromString("#C62828"), Color))
                TxtCheckStatusText.Text = "⚠️ UNCHECKED DEFECT DETECTED — Vehicle will automatically be set to Maintenance status."
                TxtCheckStatusText.Foreground = New SolidColorBrush(CType(ColorConverter.ConvertFromString("#C62828"), Color))
            End If
        End Sub

        Private Sub NumberOnly_PreviewTextInput(sender As Object, e As TextCompositionEventArgs)
            e.Handled = Not Regex.IsMatch(e.Text, "^[0-9]+$")
        End Sub

        Private Sub BtnSaveInspection_Click(sender As Object, e As RoutedEventArgs)
            Dim selectedVeh = TryCast(CmbModalVehicle.SelectedItem, FleetVehicleOption)
            If selectedVeh Is Nothing Then
                AlertUtil.ShowWarning("Please select a vehicle to inspect.", "Missing Field", ViewHelpers.Owner(Me))
                CmbModalVehicle.Focus()
                Return
            End If

            Dim inspector = TxtModalInspector.Text?.Trim()
            If String.IsNullOrWhiteSpace(inspector) Then
                AlertUtil.ShowWarning("Please enter the name of the conducting inspector.", "Missing Field", ViewHelpers.Owner(Me))
                TxtModalInspector.Focus()
                Return
            End If

            Dim odo As Long = 0
            If Not Long.TryParse(TxtModalOdometer.Text?.Trim(), odo) OrElse odo < 0 Then
                AlertUtil.ShowWarning("Please enter a valid non-negative odometer reading.", "Invalid Odometer", ViewHelpers.Owner(Me))
                TxtModalOdometer.Focus()
                Return
            End If

            Dim ext = ChkExterior.IsChecked.GetValueOrDefault()
            Dim tir = ChkTires.IsChecked.GetValueOrDefault()
            Dim lig = ChkLights.IsChecked.GetValueOrDefault()
            Dim int = ChkInterior.IsChecked.GetValueOrDefault()
            Dim flu = ChkFluids.IsChecked.GetValueOrDefault()
            Dim brk = ChkBrakes.IsChecked.GetValueOrDefault()

            Dim allPassed = ext AndAlso tir AndAlso lig AndAlso int AndAlso flu AndAlso brk
            Dim findings = TxtModalFindings.Text?.Trim()

            If Not allPassed AndAlso String.IsNullOrWhiteSpace(findings) Then
                AlertUtil.ShowWarning("One or more safety checklist items are unchecked. Please enter specific defect notes/findings describing the issue.", "Defect Notes Required", ViewHelpers.Owner(Me))
                TxtModalFindings.Focus()
                Return
            End If

            Dim checkType = If(CmbModalType.SelectedItem IsNot Nothing, CmbModalType.SelectedItem.ToString(), "Pre-Rental")
            Dim fuel = If(CmbModalFuel.SelectedItem IsNot Nothing, CmbModalFuel.SelectedItem.ToString(), "Full")

            Dim rec As New InspectionRecord() With {
                .InspectionId = _editingId,
                .VehicleId = selectedVeh.VehicleId,
                .ChecklistType = checkType,
                .InspectedAt = DateTime.Now,
                .Inspector = inspector,
                .Odometer = odo,
                .Fuel = fuel,
                .ExteriorOk = ext,
                .TiresOk = tir,
                .LightsOk = lig,
                .InteriorOk = int,
                .FluidsOk = flu,
                .BrakesOk = brk,
                .Findings = If(String.IsNullOrWhiteSpace(findings), "All 6 multi-point safety items verified. No defects found.", findings),
                .InspectionResult = If(allPassed, "Passed", "Needs Attention")
            }

            Dim resultErr As String = Nothing
            If _editingId = 0 Then
                resultErr = InspectionRepository.Add(rec)
            Else
                resultErr = InspectionRepository.Update(rec)
            End If

            If resultErr IsNot Nothing Then
                AlertUtil.ShowError(resultErr, "Save Error", ViewHelpers.Owner(Me))
                Return
            End If

            Dim actionText = If(_editingId = 0, "recorded", "updated")
            Dim actualStatus = VehicleStatusAuditRepository.CurrentStatus(selectedVeh.VehicleId)
            Dim statusNotice = If(allPassed,
                                  If(String.Equals(actualStatus, "Available", StringComparison.OrdinalIgnoreCase),
                                     "Safety checks passed. Vehicle is Available.",
                                     "Safety checks passed. Fleet status is still " & actualStatus & " because other conditions may apply."),
                                  "Unchecked safety items require attention. Current fleet status: " & actualStatus & ".")

            AlertUtil.ShowSuccess($"Successfully {actionText} digital condition checklist for [{selectedVeh.Plate}].{Environment.NewLine}{Environment.NewLine}{statusNotice}", "Checklist Certified", ViewHelpers.Owner(Me))

            ModalChecklistForm.Visibility = Visibility.Collapsed
            ReloadData()
        End Sub

        Private Sub BtnDeleteChecklist_Click(sender As Object, e As RoutedEventArgs)
            Dim btn = TryCast(sender, Button)
            Dim item = TryCast(btn?.Tag, InspectionRecord)
            If item Is Nothing Then Return

            If AlertUtil.Confirm($"Are you sure you want to delete inspection record {item.DisplayId} for [{item.VehiclePlate}]?{Environment.NewLine}{Environment.NewLine}This action cannot be undone.", "Delete Checklist Record", ViewHelpers.Owner(Me)) Then
                Dim err = InspectionRepository.Delete(item.InspectionId)
                If err IsNot Nothing Then
                    AlertUtil.ShowError(err, "Delete Error", ViewHelpers.Owner(Me))
                Else
                    AlertUtil.ShowSuccess($"Inspection record {item.DisplayId} was deleted successfully.", "Record Deleted", ViewHelpers.Owner(Me))
                    ReloadData()
                End If
            End If
        End Sub

        ' ==============================================================
        ' DIGITAL INSPECTION CERTIFICATE VIEWER & PRINTER
        ' ==============================================================
        Private Sub BtnViewCertificate_Click(sender As Object, e As RoutedEventArgs)
            Dim btn = TryCast(sender, Button)
            Dim item = TryCast(btn?.Tag, InspectionRecord)
            If item Is Nothing Then Return

            TxtCertId.Text = item.DisplayId
            TxtCertDate.Text = item.DateDisplay
            TxtCertVehicle.Text = $"[{item.VehiclePlate}] {item.VehicleMake} {item.VehicleModel}"
            TxtCertType.Text = $"{item.ChecklistType} Inspection"
            TxtCertOdo.Text = $"{item.Odometer:N0} km"
            TxtCertFuel.Text = item.Fuel
            TxtCertInspector.Text = item.Inspector
            TxtCertFindings.Text = If(String.IsNullOrWhiteSpace(item.Findings), "All systems verified normal.", item.Findings)

            ' Stamp
            If item.IsPass Then
                BorderCertStamp.Background = New SolidColorBrush(CType(ColorConverter.ConvertFromString("#E6F4EA"), Color))
                BorderCertStamp.BorderBrush = New SolidColorBrush(CType(ColorConverter.ConvertFromString("#CEEAD6"), Color))
                BorderCertStamp.BorderThickness = New Thickness(1)
                TxtCertStamp.Text = "CERTIFIED ROADWORTHY"
                TxtCertStamp.Foreground = New SolidColorBrush(CType(ColorConverter.ConvertFromString("#137333"), Color))
            Else
                BorderCertStamp.Background = New SolidColorBrush(CType(ColorConverter.ConvertFromString("#FCE8E6"), Color))
                BorderCertStamp.BorderBrush = New SolidColorBrush(CType(ColorConverter.ConvertFromString("#F5C2C7"), Color))
                BorderCertStamp.BorderThickness = New Thickness(1)
                TxtCertStamp.Text = "SERVICE REQUIRED"
                TxtCertStamp.Foreground = New SolidColorBrush(CType(ColorConverter.ConvertFromString("#C5221F"), Color))
            End If

            ' 6 Points
            StyleCertPoint(TxtCertP1, item.ExteriorOk)
            StyleCertPoint(TxtCertP2, item.TiresOk)
            StyleCertPoint(TxtCertP3, item.LightsOk)
            StyleCertPoint(TxtCertP4, item.InteriorOk)
            StyleCertPoint(TxtCertP5, item.FluidsOk)
            StyleCertPoint(TxtCertP6, item.BrakesOk)

            ModalCertificateView.Visibility = Visibility.Visible
        End Sub

        Private Sub StyleCertPoint(tb As TextBlock, passed As Boolean)
            If passed Then
                tb.Text = "✓ PASSED"
                tb.Foreground = New SolidColorBrush(CType(ColorConverter.ConvertFromString("#137333"), Color))
            Else
                tb.Text = "✗ DEFECT"
                tb.Foreground = New SolidColorBrush(CType(ColorConverter.ConvertFromString("#C5221F"), Color))
            End If
        End Sub

        Private Sub BtnCloseCert_Click(sender As Object, e As RoutedEventArgs)
            ModalCertificateView.Visibility = Visibility.Collapsed
        End Sub

        Private Sub BtnPrintCert_Click(sender As Object, e As RoutedEventArgs)
            Try
                Dim printDlg As New PrintDialog()
                If printDlg.ShowDialog() = True Then
                    printDlg.PrintVisual(PrintableCertificateArea, $"AutoRent_Inspection_Certificate_{TxtCertId.Text}")
                    AlertUtil.ShowSuccess("Inspection Certificate sent to printer successfully.", "Print Job Queued", ViewHelpers.Owner(Me))
                End If
            Catch ex As Exception
                AlertUtil.ShowError("Could not complete printing: " & ex.Message, "Print Error", ViewHelpers.Owner(Me))
            End Try
        End Sub
    End Class

End Namespace

