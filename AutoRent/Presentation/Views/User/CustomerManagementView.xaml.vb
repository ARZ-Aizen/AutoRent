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
    Partial Public Class CustomerManagementView
        Inherits UserControl

        Private Const PageSize As Integer = 10

        Private _customers As New List(Of CustomerItem)
        Private _filtered As New List(Of CustomerItem)
        Private _page As Integer = 1
        Private _totalPages As Integer = 1
        Private _editingCustomer As CustomerItem = Nothing
        Private _initialized As Boolean = False

        Public Sub New()
            InitializeComponent()
            _initialized = True
        End Sub

        Private Sub CustomerManagementView_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            ' Populate filter combo boxes without static XAML items to avoid WPF ItemsSource conflict
            CmbLicenseFilter.ItemsSource = Nothing
            CmbLicenseFilter.Items.Clear()
            CmbLicenseFilter.ItemsSource = New String() {"All Licenses", "With License", "No License"}
            CmbLicenseFilter.SelectedIndex = 0

            CmbStatusFilter.ItemsSource = Nothing
            CmbStatusFilter.Items.Clear()
            CmbStatusFilter.ItemsSource = New String() {"All Customers", "Active Renters", "Registered Only"}
            CmbStatusFilter.SelectedIndex = 0

            CmbSort.ItemsSource = Nothing
            CmbSort.Items.Clear()
            CmbSort.ItemsSource = New String() {"Name (A–Z)", "Name (Z–A)", "Customer ID (Newest)", "Customer ID (Oldest)"}
            CmbSort.SelectedIndex = 0

            ReloadCustomers()
        End Sub

        Private Sub ReloadCustomers()
            Try
                _customers = LoadCustomers()
            Catch ex As Exception
                ShowDbError("load customers", ex)
                Return
            End Try

            UpdateStatCards()
            ApplyCustomerFilter()
        End Sub

        Private Sub UpdateStatCards()
            Dim total = _customers.Count
            Dim active = _customers.Where(Function(c) c.HasActiveReservation).Count()
            Dim verified = _customers.Where(Function(c) c.HasLicense).Count()
            Dim missing = _customers.Where(Function(c) Not c.HasLicense).Count()

            TxtStatTotal.Text = total.ToString()
            TxtStatActive.Text = active.ToString()
            TxtStatVerified.Text = verified.ToString()
            TxtStatMissing.Text = missing.ToString()
        End Sub

        Private Sub TxtCustSearch_TextChanged(sender As Object, e As TextChangedEventArgs)
            If Not _initialized Then Return
            TxtSearchWatermark.Visibility = If(String.IsNullOrEmpty(TxtCustSearch.Text), Visibility.Visible, Visibility.Collapsed)
            _page = 1
            ApplyCustomerFilter()
        End Sub

        Private Sub FilterInputs_Changed(sender As Object, e As SelectionChangedEventArgs)
            If Not _initialized Then Return
            _page = 1
            ApplyCustomerFilter()
        End Sub

        Private Sub BtnResetFilters_Click(sender As Object, e As RoutedEventArgs)
            TxtCustSearch.Text = String.Empty
            TxtSearchWatermark.Visibility = Visibility.Visible
            CmbLicenseFilter.SelectedIndex = 0
            CmbStatusFilter.SelectedIndex = 0
            CmbSort.SelectedIndex = 0
            _page = 1
            ApplyCustomerFilter()
        End Sub

        Private Sub ApplyCustomerFilter()
            If _customers Is Nothing Then Return

            Dim q As String = TxtCustSearch.Text.Trim().ToLowerInvariant()
            Dim licenseFilter As String = If(TryCast(CmbLicenseFilter.SelectedItem, String), "All Licenses")
            Dim statusFilter As String = If(TryCast(CmbStatusFilter.SelectedItem, String), "All Customers")
            Dim sortOption As String = If(TryCast(CmbSort.SelectedItem, String), "Name (A–Z)")

            Dim result = _customers.Where(Function(c)
                                              ' Text search filter
                                              Dim matchQuery = (q = "" OrElse
                                                  (c.Name IsNot Nothing AndAlso c.Name.ToLowerInvariant().Contains(q)) OrElse
                                                  (c.Phone IsNot Nothing AndAlso c.Phone.ToLowerInvariant().Contains(q)) OrElse
                                                  (c.Email IsNot Nothing AndAlso c.Email.ToLowerInvariant().Contains(q)) OrElse
                                                  (c.License IsNot Nothing AndAlso c.License.ToLowerInvariant().Contains(q)) OrElse
                                                  (c.CustomerIdDisplay IsNot Nothing AndAlso c.CustomerIdDisplay.ToLowerInvariant().Contains(q)))

                                              ' License status filter
                                              Dim matchLicense = True
                                              If licenseFilter = "With License" Then
                                                  matchLicense = c.HasLicense
                                              ElseIf licenseFilter = "No License" Then
                                                  matchLicense = Not c.HasLicense
                                              End If

                                              ' Status filter
                                              Dim matchStatus = True
                                              If statusFilter = "Active Renters" Then
                                                  matchStatus = c.HasActiveReservation
                                              ElseIf statusFilter = "Registered Only" Then
                                                  matchStatus = Not c.HasActiveReservation
                                              End If

                                              Return matchQuery AndAlso matchLicense AndAlso matchStatus
                                          End Function)

            ' Sort results
            Select Case sortOption
                Case "Name (Z–A)"
                    _filtered = result.OrderByDescending(Function(c) c.Name).ToList()
                Case "Customer ID (Newest)"
                    _filtered = result.OrderByDescending(Function(c) c.DbId).ToList()
                Case "Customer ID (Oldest)"
                    _filtered = result.OrderBy(Function(c) c.DbId).ToList()
                Case Else '"Name (A–Z)"
                    _filtered = result.OrderBy(Function(c) c.Name).ToList()
            End Select

            _totalPages = Math.Max(1, CInt(Math.Ceiling(_filtered.Count / CDbl(PageSize))))
            If _page > _totalPages Then _page = _totalPages
            If _page < 1 Then _page = 1

            ShowPage()
        End Sub

        Private Sub ShowPage()
            Dim pageItems = _filtered.Skip((_page - 1) * PageSize).Take(PageSize).ToList()
            DgCustomers.ItemsSource = pageItems

            If _filtered.Count = 0 Then
                DgCustomers.Visibility = Visibility.Collapsed
                BorderEmptyState.Visibility = Visibility.Visible
                TxtPaging.Text = "NO CUSTOMERS FOUND"
            Else
                DgCustomers.Visibility = Visibility.Visible
                BorderEmptyState.Visibility = Visibility.Collapsed
                TxtPaging.Text = $"SHOWING {pageItems.Count} OF {_filtered.Count} CUSTOMERS"
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
                    ' Active page: AutoRent Brand Red with soft glow
                    btn.Template = CreatePageTemplate("#70000F", "White", True)
                    btn.Foreground = Brushes.White
                Else
                    ' Inactive page: clean white border
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
        ' MODAL: ADD / EDIT CUSTOMER
        ' =========================

        Private Sub BtnOpenAddModal_Click(sender As Object, e As RoutedEventArgs)
            _editingCustomer = Nothing
            TxtModalTitle.Text = "Register New Customer"
            TxtModalSubtitle.Text = "Enter customer details and driver license info"
            TxtModalIcon.Text = ChrW(&HE77B)
            BtnSaveModal.Content = "Register Customer"

            TxtModalName.Clear()
            TxtModalPhone.Clear()
            TxtModalEmail.Clear()
            TxtModalLicense.Clear()

            BorderModalError.Visibility = Visibility.Collapsed
            CustomerModalOverlay.Visibility = Visibility.Visible
            TxtModalName.Focus()
        End Sub

        Private Sub BtnEditCustomer_Click(sender As Object, e As RoutedEventArgs)
            Dim selected = TryCast(CType(sender, FrameworkElement).Tag, CustomerItem)
            If selected Is Nothing Then Return

            _editingCustomer = selected
            TxtModalTitle.Text = "Edit Customer Profile"
            TxtModalSubtitle.Text = $"Update profile details for {selected.CustomerIdDisplay}"
            TxtModalIcon.Text = ChrW(&HE70F)
            BtnSaveModal.Content = "Save Changes"

            TxtModalName.Text = selected.Name
            TxtModalPhone.Text = selected.Phone
            TxtModalEmail.Text = selected.Email
            TxtModalLicense.Text = selected.License

            BorderModalError.Visibility = Visibility.Collapsed
            CustomerModalOverlay.Visibility = Visibility.Visible
            TxtModalName.Focus()
        End Sub

        Private Sub BtnCloseModal_Click(sender As Object, e As RoutedEventArgs)
            CustomerModalOverlay.Visibility = Visibility.Collapsed
            _editingCustomer = Nothing
        End Sub

        Private Sub ModalBackdrop_MouseDown(sender As Object, e As MouseButtonEventArgs)
            CustomerModalOverlay.Visibility = Visibility.Collapsed
            _editingCustomer = Nothing
        End Sub

        Private Sub ShowModalError(msg As String)
            TxtModalError.Text = msg
            BorderModalError.Visibility = Visibility.Visible
        End Sub

        Private Sub BtnSaveModal_Click(sender As Object, e As RoutedEventArgs)
            Dim name As String = TxtModalName.Text.Trim()
            Dim phone As String = TxtModalPhone.Text.Trim()
            Dim email As String = TxtModalEmail.Text.Trim()
            Dim license As String = TxtModalLicense.Text.Trim()

            If String.IsNullOrWhiteSpace(name) Then
                ShowModalError("Full name is required.")
                TxtModalName.Focus()
                Return
            End If

            If String.IsNullOrWhiteSpace(phone) Then
                ShowModalError("Phone number is required.")
                TxtModalPhone.Focus()
                Return
            End If

            Try
                If _editingCustomer Is Nothing Then
                    ' Create New Customer
                    Dim problem = ClientRepository.AddCustomer(name, phone, email, license)
                    If problem IsNot Nothing Then
                        ShowModalError(problem)
                        Return
                    End If
                Else
                    ' Update Existing Customer
                    Dim problem = ClientRepository.UpdateCustomer(_editingCustomer.DbId, name, phone, email, license)
                    If problem IsNot Nothing Then
                        ShowModalError(problem)
                        Return
                    End If
                End If
            Catch ex As Exception
                ShowModalError($"Database error: {ex.Message}")
                Return
            End Try

            CustomerModalOverlay.Visibility = Visibility.Collapsed
            _editingCustomer = Nothing
            ReloadCustomers()
        End Sub

        Private Sub BtnDeleteCustomer_Click(sender As Object, e As RoutedEventArgs)
            Dim selected = TryCast(CType(sender, FrameworkElement).Tag, CustomerItem)
            If selected Is Nothing Then Return

            Dim confirm = MessageBox.Show($"Are you sure you want to delete customer '{selected.Name}' ({selected.CustomerIdDisplay})?" & vbCrLf & vbCrLf &
                                          "This action will permanently remove their profile.",
                                          "Delete Customer",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Question)
            If confirm <> MessageBoxResult.Yes Then Return

            Try
                Dim problem = ClientRepository.DeleteCustomer(selected.DbId, selected.Name)
                If problem IsNot Nothing Then
                    MessageBox.Show(problem, "Cannot Delete Customer", MessageBoxButton.OK, MessageBoxImage.Warning)
                    Return
                End If
            Catch ex As Exception
                ShowDbError("delete customer", ex)
                Return
            End Try

            ReloadCustomers()
        End Sub

        ' =========================
        ' CSV EXPORT
        ' =========================

        Private Sub BtnExportCsv_Click(sender As Object, e As RoutedEventArgs)
            If _filtered Is Nothing OrElse _filtered.Count = 0 Then
                MessageBox.Show("There are no customer records to export.", "Export CSV", MessageBoxButton.OK, MessageBoxImage.Information)
                Return
            End If

            Dim dialog As New SaveFileDialog() With {
                .FileName = $"AutoRent_Customers_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
                .DefaultExt = ".csv",
                .Filter = "CSV documents (.csv)|*.csv"
            }

            If dialog.ShowDialog() = True Then
                Try
                    Dim sb As New StringBuilder()
                    sb.AppendLine("Customer ID,Full Name,Phone,Email,Driver License,Rental Status")

                    For Each c In _filtered
                        Dim id = EscapeCsv(c.CustomerIdDisplay)
                        Dim name = EscapeCsv(c.Name)
                        Dim phone = EscapeCsv(c.Phone)
                        Dim email = EscapeCsv(c.Email)
                        Dim license = EscapeCsv(c.License)
                        Dim status = EscapeCsv(c.StatusBadgeText)
                        sb.AppendLine($"{id},{name},{phone},{email},{license},{status}")
                    Next

                    File.WriteAllText(dialog.FileName, sb.ToString(), Encoding.UTF8)
                    MessageBox.Show($"Successfully exported {_filtered.Count} customer records to CSV.", "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information)
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