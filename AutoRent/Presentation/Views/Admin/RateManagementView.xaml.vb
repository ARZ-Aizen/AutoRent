Imports System.Collections.Generic
Imports System.Data
Imports System.Linq
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports AutoRent.Common.Utilities
Imports AutoRent.Data.Models
Imports AutoRent.Data.Repositories

Namespace Presentation.Views.Admin
    Partial Public Class RateManagementView
        Inherits UserControl

        Private _ready As Boolean = False
        Private _selectedTier As RateTier = Nothing
        Private _selectedRule As PricingRule = Nothing
        Private _isNewTierMode As Boolean = False

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub RateManagementView_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            If _ready Then Return
            _ready = True
            LoadTiers()
            LoadPricingRules()
        End Sub

        Private Sub LoadTiers(Optional selectCategory As String = "")
            Try
                Dim tiers = RateRepository.GetAllTiers()
                DgRateTiers.ItemsSource = tiers

                If tiers.Count > 0 Then
                    Dim target = If(Not String.IsNullOrEmpty(selectCategory),
                                    tiers.FirstOrDefault(Function(t) t.Category.Equals(selectCategory, StringComparison.OrdinalIgnoreCase)),
                                    Nothing)
                    If target Is Nothing Then target = tiers.FirstOrDefault(Function(t) t.Category.Equals("Sedan", StringComparison.OrdinalIgnoreCase))
                    If target Is Nothing Then target = tiers(0)
                    DgRateTiers.SelectedItem = target
                End If
            Catch ex As Exception
                AlertUtil.ShowError($"Could not load rate tiers: {ex.Message}", "Rate Management", ViewHelpers.Owner(Me))
            End Try
        End Sub

        Private Sub LoadPricingRules()
            Try
                Dim rules = RateRepository.GetPricingRules()
                DgPricingRules.ItemsSource = rules

                Dim activeCount = rules.Where(Function(r) r.IsActive).Count()
                Dim inactiveCount = rules.Where(Function(r) Not r.IsActive).Count()
                TxtRulesSummary.Text = $"{activeCount} active / {inactiveCount} inactive"
            Catch ex As Exception
                AlertUtil.ShowError($"Could not load pricing rules: {ex.Message}", "Rate Management", ViewHelpers.Owner(Me))
            End Try
        End Sub

        Private Sub DgRateTiers_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            Dim tier = TryCast(DgRateTiers.SelectedItem, RateTier)
            If tier Is Nothing Then Return

            _selectedTier = tier
            UpdatePreview(tier)
        End Sub

        Private Sub UpdatePreview(tier As RateTier)
            If tier Is Nothing Then Return

            TxtPreviewSubtitle.Text = $"{tier.Category} · 17-19 Oct 2026 · 2 weekend days"
            Dim daily = tier.DailyRate
            Dim baseAmount = daily * 2D
            Dim weekendUplift = baseAmount * 0.15D
            Dim childSeat = 30D
            Dim total = baseAmount + weekendUplift + childSeat

            TxtPreviewBaseLabel.Text = $"Base · 2 × ₱{daily:N0}"
            TxtPreviewBaseVal.Text = $"₱{baseAmount:N2}"
            TxtPreviewUpliftLabel.Text = "Weekend uplift · ×1.15"
            TxtPreviewUpliftVal.Text = $"+₱{weekendUplift:N2}"
            TxtPreviewTotalVal.Text = $"₱{total:N2}"
        End Sub

        Private Sub BtnAddTier_Click(sender As Object, e As RoutedEventArgs)
            _isNewTierMode = True
            TxtTierModalTitle.Text = "Add New Base Rate Tier"
            TxtTierModalSubtitle.Text = "Create a new vehicle category tier with base pricing"
            TxtModalCategory.Clear()
            TxtModalCategory.IsReadOnly = False
            TxtModalCategory.Background = System.Windows.Media.Brushes.White
            TxtModalDaily.Clear()
            TxtModalWeekly.Clear()
            TxtModalMonthly.Clear()
            TxtModalDeposit.Text = "500"
            TxtModalLateFee.Text = "50"
            TxtModalOverride.Text = "None"
            BtnDeleteTier.Visibility = Visibility.Collapsed
            BtnSaveTierSubmit.Content = "Create Tier"

            EditTiersModal.Visibility = Visibility.Visible
            TxtModalCategory.Focus()
        End Sub

        Private Sub BtnEditTiers_Click(sender As Object, e As RoutedEventArgs)
            If _selectedTier Is Nothing AndAlso DgRateTiers.Items.Count > 0 Then
                _selectedTier = TryCast(DgRateTiers.Items(0), RateTier)
            End If

            If _selectedTier Is Nothing Then
                AlertUtil.ShowWarning("Please select a category tier to edit.", "Edit Tiers", ViewHelpers.Owner(Me))
                Return
            End If

            _isNewTierMode = False
            TxtTierModalTitle.Text = "Edit Base Rate Tier"
            TxtTierModalSubtitle.Text = "Customize pricing and overrides for this vehicle category"
            TxtModalCategory.Text = _selectedTier.Category
            TxtModalCategory.IsReadOnly = True
            TxtModalCategory.Background = New System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(250, 249, 248))
            TxtModalDaily.Text = _selectedTier.DailyRate.ToString("0.##")
            TxtModalWeekly.Text = _selectedTier.WeeklyRate.ToString("0.##")
            TxtModalMonthly.Text = _selectedTier.MonthlyRate.ToString("0.##")
            TxtModalDeposit.Text = _selectedTier.Deposit.ToString("0.##")
            TxtModalLateFee.Text = _selectedTier.LateFeePerDay.ToString("0.##")
            TxtModalOverride.Text = _selectedTier.VehicleOverride
            BtnDeleteTier.Visibility = Visibility.Visible
            BtnSaveTierSubmit.Content = "Save Changes"

            EditTiersModal.Visibility = Visibility.Visible
            TxtModalDaily.Focus()
        End Sub

        Private Sub BtnCloseTiersModal_Click(sender As Object, e As RoutedEventArgs)
            EditTiersModal.Visibility = Visibility.Collapsed
        End Sub

        Private Sub BtnDeleteTier_Click(sender As Object, e As RoutedEventArgs)
            If _selectedTier Is Nothing OrElse _isNewTierMode Then Return

            If Not AlertUtil.Confirm($"Are you sure you want to delete the rate tier for '{_selectedTier.Category}'?", "Delete Tier", ViewHelpers.Owner(Me)) Then
                Return
            End If

            Try
                Dim problem = RateRepository.Delete(_selectedTier.RateId, _selectedTier.Category)
                If problem IsNot Nothing Then
                    AlertUtil.ShowWarning(problem, "Delete Tier", ViewHelpers.Owner(Me))
                    Return
                End If

                EditTiersModal.Visibility = Visibility.Collapsed
                AlertUtil.ShowSuccess($"Rate tier '{_selectedTier.Category}' was deleted.", "Delete Tier", ViewHelpers.Owner(Me))
                LoadTiers()
            Catch ex As Exception
                AlertUtil.ShowError($"Could not delete rate tier: {ex.Message}", "Delete Tier", ViewHelpers.Owner(Me))
            End Try
        End Sub

        Private Sub BtnSaveTiers_Click(sender As Object, e As RoutedEventArgs)
            Dim cat = TxtModalCategory.Text.Trim()
            If String.IsNullOrWhiteSpace(cat) Then
                AlertUtil.ShowWarning("Please enter a category name.", "Save Tier", ViewHelpers.Owner(Me))
                Return
            End If

            Dim daily, weekly, monthly, deposit, lateFee As Decimal
            If Not ViewHelpers.TryParseMoney(TxtModalDaily.Text, daily) OrElse daily <= 0D Then
                AlertUtil.ShowWarning("Daily rate must be a number greater than 0.", "Save Tier", ViewHelpers.Owner(Me))
                Return
            End If

            If String.IsNullOrWhiteSpace(TxtModalWeekly.Text) Then
                weekly = daily * 6D
            ElseIf Not ViewHelpers.TryParseMoney(TxtModalWeekly.Text, weekly) Then
                AlertUtil.ShowWarning("Weekly rate must be a valid number.", "Save Tier", ViewHelpers.Owner(Me))
                Return
            End If

            If String.IsNullOrWhiteSpace(TxtModalMonthly.Text) Then
                monthly = daily * 24D
            ElseIf Not ViewHelpers.TryParseMoney(TxtModalMonthly.Text, monthly) Then
                AlertUtil.ShowWarning("Monthly rate must be a valid number.", "Save Tier", ViewHelpers.Owner(Me))
                Return
            End If

            If Not ViewHelpers.TryParseMoney(TxtModalDeposit.Text, deposit) Then deposit = 500D
            If Not ViewHelpers.TryParseMoney(TxtModalLateFee.Text, lateFee) Then lateFee = 50D

            Dim overrideText = TxtModalOverride.Text.Trim()
            If String.IsNullOrWhiteSpace(overrideText) Then overrideText = "None"

            If _isNewTierMode Then
                If Not AlertUtil.Confirm($"Are you sure you want to add the new rate tier for '{cat}'?", "Create Tier", ViewHelpers.Owner(Me)) Then
                    Return
                End If

                Try
                    Dim problem = RateRepository.Add(cat, daily, weekly, monthly, deposit, lateFee)
                    If problem IsNot Nothing Then
                        AlertUtil.ShowWarning(problem, "Create Tier", ViewHelpers.Owner(Me))
                        Return
                    End If

                    RateRepository.SetVehicleOverride(cat, overrideText)
                    EditTiersModal.Visibility = Visibility.Collapsed
                    AlertUtil.ShowSuccess($"Rate tier '{cat}' was created successfully.", "Create Tier", ViewHelpers.Owner(Me))
                    LoadTiers(cat)
                Catch ex As Exception
                    AlertUtil.ShowError($"Could not create rate tier: {ex.Message}", "Create Tier", ViewHelpers.Owner(Me))
                End Try
            Else
                If _selectedTier Is Nothing Then Return

                If Not AlertUtil.Confirm($"Are you sure you want to update the base rate tier for '{cat}'?", "Save Tier", ViewHelpers.Owner(Me)) Then
                    Return
                End If

                Try
                    Dim problem = RateRepository.Update(_selectedTier.RateId, _selectedTier.Category, cat, daily, weekly, monthly, deposit, lateFee)
                    If problem IsNot Nothing Then
                        AlertUtil.ShowWarning(problem, "Save Tier", ViewHelpers.Owner(Me))
                        Return
                    End If

                    RateRepository.SetVehicleOverride(cat, overrideText)
                    EditTiersModal.Visibility = Visibility.Collapsed
                    AlertUtil.ShowSuccess($"Rate tier for '{cat}' was updated successfully.", "Save Tier", ViewHelpers.Owner(Me))
                    LoadTiers(cat)
                Catch ex As Exception
                    AlertUtil.ShowError($"Could not update rate tier: {ex.Message}", "Save Tier", ViewHelpers.Owner(Me))
                End Try
            End If
        End Sub

        Private Sub BtnOverrideInfo_Click(sender As Object, e As RoutedEventArgs)
            VehicleOverrideInfoModal.Visibility = Visibility.Visible
        End Sub

        Private Sub BtnCloseOverrideInfo_Click(sender As Object, e As RoutedEventArgs)
            VehicleOverrideInfoModal.Visibility = Visibility.Collapsed
        End Sub

        Private Sub BtnCreateRule_Click(sender As Object, e As RoutedEventArgs)
            Dim rules = RateRepository.GetPricingRules()
            Dim nextNum = rules.Count + 1
            TxtNewRuleName.Text = $"RR-{nextNum:000} · Seasonal demand"

            Dim tiers = RateRepository.GetAllTiers()
            Dim catList As New List(Of String) From {"All"}
            catList.AddRange(tiers.Select(Function(t) t.Category))
            CmbNewRuleCategory.ItemsSource = catList
            CmbNewRuleCategory.SelectedIndex = 0

            TxtNewRuleMultiplier.Text = "×1.20"
            TxtNewRuleMethod.Text = "Base rate multiplier"
            TxtNewRuleDates.Text = $"{DateTime.Now:dd MMM yyyy} / Ongoing"
            ChkNewRuleActive.IsChecked = True

            CreateRuleModal.Visibility = Visibility.Visible
            TxtNewRuleName.Focus()
        End Sub

        Private Sub BtnCloseRuleModal_Click(sender As Object, e As RoutedEventArgs)
            CreateRuleModal.Visibility = Visibility.Collapsed
        End Sub

        Private Sub BtnSaveNewRule_Click(sender As Object, e As RoutedEventArgs)
            Dim ruleText = TxtNewRuleName.Text.Trim()
            Dim cat = TryCast(CmbNewRuleCategory.SelectedItem, String)
            Dim mult = TxtNewRuleMultiplier.Text.Trim()
            Dim method = TxtNewRuleMethod.Text.Trim()
            Dim dates = TxtNewRuleDates.Text.Trim()
            Dim isActive = ChkNewRuleActive.IsChecked.GetValueOrDefault(True)

            If String.IsNullOrWhiteSpace(ruleText) Then
                AlertUtil.ShowWarning("Please specify a rule name.", "Create Rate Rule", ViewHelpers.Owner(Me))
                Return
            End If

            If String.IsNullOrWhiteSpace(mult) Then
                AlertUtil.ShowWarning("Please specify a rate or multiplier.", "Create Rate Rule", ViewHelpers.Owner(Me))
                Return
            End If

            If Not AlertUtil.Confirm($"Are you sure you want to create the pricing rule '{ruleText}'?", "Create Rate Rule", ViewHelpers.Owner(Me)) Then
                Return
            End If

            Dim ruleId = "RR-000"
            Dim namePart = ruleText
            If ruleText.Contains("·") Then
                Dim parts = ruleText.Split("·"c)
                ruleId = parts(0).Trim()
                namePart = parts(1).Trim()
            Else
                Dim rules = RateRepository.GetPricingRules()
                ruleId = $"RR-{rules.Count + 1:000}"
            End If

            Dim newRule As New PricingRule With {
                .RuleId = ruleId,
                .RuleName = namePart,
                .Category = If(String.IsNullOrEmpty(cat), "All", cat),
                .CalculationMethod = If(String.IsNullOrEmpty(method), "Base rate multiplier", method),
                .RateMultiplier = mult,
                .EffectiveDates = If(String.IsNullOrEmpty(dates), $"{DateTime.Now:dd MMM yyyy} / Ongoing", dates),
                .IsActive = isActive,
                .LastModified = $"Admin · {DateTime.Now:dd MMM, HH:mm}"
            }

            RateRepository.AddPricingRule(newRule)
            CreateRuleModal.Visibility = Visibility.Collapsed
            AlertUtil.ShowSuccess($"Pricing rule '{ruleId}' created successfully.", "Create Rate Rule", ViewHelpers.Owner(Me))
            LoadPricingRules()
        End Sub

        Private Sub DgPricingRules_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            Dim rule = TryCast(DgPricingRules.SelectedItem, PricingRule)
            If rule Is Nothing Then Return

            _selectedRule = rule
            TxtInspectTitle.Text = $"{rule.RuleId} · {rule.RuleName}"
            TxtInspectSubtitle.Text = $"Inspect and modify {rule.Category} category pricing rule"
            TxtInspectRuleName.Text = rule.RuleName
            TxtInspectCategory.Text = rule.Category
            TxtInspectMultiplier.Text = rule.RateMultiplier
            TxtInspectMethod.Text = rule.CalculationMethod
            TxtInspectDates.Text = rule.EffectiveDates
            ChkInspectActive.IsChecked = rule.IsActive

            InspectRuleModal.Visibility = Visibility.Visible
        End Sub

        Private Sub BtnCloseInspectModal_Click(sender As Object, e As RoutedEventArgs)
            InspectRuleModal.Visibility = Visibility.Collapsed
            DgPricingRules.SelectedItem = Nothing
        End Sub

        Private Sub BtnSaveInspectRule_Click(sender As Object, e As RoutedEventArgs)
            If _selectedRule Is Nothing Then Return

            Dim name = TxtInspectRuleName.Text.Trim()
            Dim cat = TxtInspectCategory.Text.Trim()
            Dim mult = TxtInspectMultiplier.Text.Trim()
            Dim method = TxtInspectMethod.Text.Trim()
            Dim dates = TxtInspectDates.Text.Trim()
            Dim isActive = ChkInspectActive.IsChecked.GetValueOrDefault(True)

            If String.IsNullOrWhiteSpace(name) Then
                AlertUtil.ShowWarning("Rule name cannot be blank.", "Save Rule", ViewHelpers.Owner(Me))
                Return
            End If

            If Not AlertUtil.Confirm($"Are you sure you want to save changes to rule '{_selectedRule.RuleId}'?", "Save Rule", ViewHelpers.Owner(Me)) Then
                Return
            End If

            RateRepository.UpdatePricingRule(_selectedRule.RuleId, name, cat, method, mult, dates, isActive)
            InspectRuleModal.Visibility = Visibility.Collapsed
            DgPricingRules.SelectedItem = Nothing
            AlertUtil.ShowSuccess($"Rule '{_selectedRule.RuleId}' updated successfully.", "Save Rule", ViewHelpers.Owner(Me))
            LoadPricingRules()
        End Sub

        Private Sub ModalBackdrop_MouseDown(sender As Object, e As MouseButtonEventArgs)
            EditTiersModal.Visibility = Visibility.Collapsed
            VehicleOverrideInfoModal.Visibility = Visibility.Collapsed
            CreateRuleModal.Visibility = Visibility.Collapsed
            InspectRuleModal.Visibility = Visibility.Collapsed
            DgPricingRules.SelectedItem = Nothing
        End Sub
    End Class
End Namespace
