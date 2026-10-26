Imports System.Linq
Imports System.Windows
Imports System.Windows.Controls
Imports AutoRent.Data.Repositories

Namespace Presentation.Views.User
    Partial Public Class CustomerManagementView
        Inherits UserControl

        Private _customers As New List(Of CustomerItem)
        Private _initialized As Boolean = False

        Public Sub New()
            InitializeComponent()
            _initialized = True
        End Sub

        Private Sub CustomerManagementView_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            ReloadCustomers()
        End Sub

        Private Sub ReloadCustomers()
            Try
                _customers = LoadCustomers()
            Catch ex As Exception
                ShowDbError("load the customers", ex)
                Return
            End Try
            ApplyCustomerFilter()
        End Sub

        Private Sub TxtCustSearch_TextChanged(sender As Object, e As TextChangedEventArgs)
            If Not _initialized Then Return
            ApplyCustomerFilter()
        End Sub

        Private Sub ApplyCustomerFilter()
            Dim q As String = TxtCustSearch.Text.Trim().ToLowerInvariant()

            DgCustomers.ItemsSource = _customers.Where(Function(c) q = "" OrElse
                                              c.Name.ToLowerInvariant().Contains(q) OrElse
                                              c.Phone.ToLowerInvariant().Contains(q) OrElse
                                              c.Email.ToLowerInvariant().Contains(q) OrElse
                                              c.License.ToLowerInvariant().Contains(q)).ToList()
        End Sub

        Private Sub BtnAddCustomer_Click(sender As Object, e As RoutedEventArgs)
            Dim name As String = TxtCustName.Text.Trim()
            Dim phone As String = TxtCustPhone.Text.Trim()

            If name = "" OrElse phone = "" Then
                MessageBox.Show("Full name and phone are required.", "Add Customer",
                                MessageBoxButton.OK, MessageBoxImage.Warning)
                Return
            End If

            Try
                Dim problem = ClientRepository.AddCustomer(name, phone, TxtCustEmail.Text.Trim(), TxtCustLicense.Text.Trim())
                If problem IsNot Nothing Then
                    MessageBox.Show(problem, "Add Customer", MessageBoxButton.OK, MessageBoxImage.Warning)
                    Return
                End If
            Catch ex As Exception
                ShowDbError("save the customer", ex)
                Return
            End Try

            TxtCustName.Clear()
            TxtCustPhone.Clear()
            TxtCustEmail.Clear()
            TxtCustLicense.Clear()
            ReloadCustomers()
        End Sub

        Private Sub BtnDeleteCustomer_Click(sender As Object, e As RoutedEventArgs)
            Dim selected = TryCast(DgCustomers.SelectedItem, CustomerItem)
            If selected Is Nothing Then
                MessageBox.Show("Select a customer first.", "Delete Customer",
                                MessageBoxButton.OK, MessageBoxImage.Information)
                Return
            End If

            If MessageBox.Show($"Delete {selected.Name}?", "Delete Customer",
                               MessageBoxButton.YesNo, MessageBoxImage.Question) <> MessageBoxResult.Yes Then Return

            Try
                Dim problem = ClientRepository.DeleteCustomer(selected.DbId, selected.Name)
                If problem IsNot Nothing Then
                    MessageBox.Show(problem, "Delete Customer", MessageBoxButton.OK, MessageBoxImage.Warning)
                    Return
                End If
            Catch ex As Exception
                ShowDbError("delete the customer", ex)
                Return
            End Try
            ReloadCustomers()
        End Sub
    End Class
End Namespace