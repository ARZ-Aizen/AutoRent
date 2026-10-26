
Imports System.Collections.Generic
Imports System.Data
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Controls.Primitives
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports Microsoft.Win32
Imports AutoRent.Common.Utilities
Imports AutoRent.Data.Repositories

Namespace Presentation.Views.Admin

    Partial Public Class FleetManagementView
        Inherits UserControl

        Private _vehicles As DataTable
        Private _filtered As DataView
        Private _ready As Boolean = False
        Private _page As Integer = 1

        Private Const PageSize As Integer = 8

        Public Sub New()
            InitializeComponent()
        End Sub

        ' ========================================
        ' LOAD PAGE
        ' ========================================

        Private Sub FleetManagementView_Loaded(
            sender As Object,
            e As RoutedEventArgs
        ) Handles Me.Loaded

            If _ready Then Return
            _ready = True

            CmbStatusFilter.Items.Add("All statuses")

            For Each statusValue As String In VehicleRepository.Statuses
                CmbStatusFilter.Items.Add(statusValue)
            Next

            CmbStatusFilter.SelectedIndex = 0
            CmbSort.SelectedIndex = 0

            ReloadCategories()
            ReloadVehicles()
        End Sub

        ' ========================================
        ' LOAD CATEGORIES
        ' ========================================

        Private Sub ReloadCategories()

            Dim previous =
                TryCast(CmbCategoryFilter.SelectedItem, String)

            CmbCategoryFilter.Items.Clear()
            CmbCategoryFilter.Items.Add("All categories")

            Dim categories = RateRepository.GetCategories()

            For Each categoryRow As DataRow In categories.Rows
                Dim categoryText =
                    Convert.ToString(categoryRow("category"))

                If Not String.IsNullOrWhiteSpace(categoryText) Then
                    CmbCategoryFilter.Items.Add(categoryText)
                End If
            Next

            If previous IsNot Nothing AndAlso
               CmbCategoryFilter.Items.Contains(previous) Then

                CmbCategoryFilter.SelectedItem = previous

            Else
                CmbCategoryFilter.SelectedIndex = 0
            End If

        End Sub

        ' ========================================
        ' READ VEHICLE IMAGE
        ' ========================================

        Private Shared Function ReadVehiclePhoto(
            imagePath As String
        ) As BitmapImage

            Try
                Dim fullPath =
                    VehicleRepository.GetImageFilePath(imagePath)

                If String.IsNullOrWhiteSpace(fullPath) OrElse
                   Not File.Exists(fullPath) Then
                    Return Nothing
                End If

                Dim bitmap As New BitmapImage()

                bitmap.BeginInit()
                bitmap.UriSource =
                    New Uri(fullPath, UriKind.Absolute)
                bitmap.CacheOption = BitmapCacheOption.OnLoad
                bitmap.DecodePixelWidth = 240
                bitmap.EndInit()
                bitmap.Freeze()

                Return bitmap

            Catch
                Return Nothing
            End Try

        End Function

        ' ========================================
        ' LOAD VEHICLES FROM ACCESS
        ' ========================================

        Private Sub ReloadVehicles()

            Try
                _vehicles = VehicleRepository.GetAll()

                _vehicles.Columns.Add(
                    "image_source",
                    GetType(Object)
                )

                _vehicles.Columns.Add(
                    "display_id",
                    GetType(String)
                )

                For Each vehicleRow As DataRowView In _vehicles.DefaultView

                    vehicleRow("display_id") =
                        "VH-" &
                        ViewHelpers.IntOf(
                            vehicleRow,
                            "vehicle_id"
                        ).ToString("0000")

                    Dim pic = ReadVehiclePhoto(
                        ViewHelpers.Str(vehicleRow, "image_path")
                    )

                    If pic IsNot Nothing Then
                        vehicleRow("image_source") = pic
                    End If

                Next

                _filtered = New DataView(_vehicles)
                _page = 1

                ApplyFilters()

            Catch ex As Exception
                MessageBox.Show(
                    "Could not load vehicle records: " & ex.Message,
                    "Fleet Inventory",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                )
            End Try

        End Sub

        ' ========================================
        ' SEARCH AND FILTER EVENTS
        ' ========================================

        Private Sub Search_Changed(
            sender As Object,
            e As TextChangedEventArgs
        )

            If Not _ready OrElse _filtered Is Nothing Then Return

            _page = 1
            ApplyFilters()

        End Sub

        Private Sub Selection_Changed(
            sender As Object,
            e As SelectionChangedEventArgs
        )

            If Not _ready OrElse _filtered Is Nothing Then Return

            _page = 1
            ApplyFilters()

        End Sub

        ' ========================================
        ' APPLY SEARCH, CATEGORY, STATUS, SORT
        ' ========================================

        Private Sub ApplyFilters()

            If _filtered Is Nothing Then Return

            Try
                Dim conditions As New List(Of String)()
                Dim searchText = TxtSearch.Text.Trim()

                If searchText.Length > 0 Then

                    Dim searchValue =
                        ViewHelpers.EscapeLike(searchText)

                    conditions.Add(
                        "(plate LIKE '%" & searchValue & "%' " &
                        "OR make LIKE '%" & searchValue & "%' " &
                        "OR model LIKE '%" & searchValue & "%' " &
                        "OR display_id LIKE '%" & searchValue & "%')"
                    )

                End If

                Dim category =
                    TryCast(CmbCategoryFilter.SelectedItem, String)

                If Not String.IsNullOrWhiteSpace(category) AndAlso
                   category <> "All categories" Then

                    conditions.Add(
                        "category = '" &
                        category.Replace("'", "''") & "'"
                    )

                End If

                Dim statusText =
                    TryCast(CmbStatusFilter.SelectedItem, String)

                If Not String.IsNullOrWhiteSpace(statusText) AndAlso
                   statusText <> "All statuses" Then

                    conditions.Add(
                        "status = '" &
                        statusText.Replace("'", "''") & "'"
                    )

                End If

                _filtered.RowFilter =
                    String.Join(" AND ", conditions)

                Dim sorting =
                    TryCast(CmbSort.SelectedItem, ComboBoxItem)

                _filtered.Sort = If(
                    sorting Is Nothing,
                    "make ASC, model ASC",
                    Convert.ToString(sorting.Tag)
                )

                UpdatePage()

            Catch ex As Exception

                MessageBox.Show(
                    "Filter error: " & ex.Message,
                    "Fleet Inventory",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                )

            End Try

        End Sub

        ' ========================================
        ' PAGINATION
        ' ========================================

        Private Sub UpdatePage()

            If _filtered Is Nothing Then Return

            Dim filteredCount = _filtered.Count

            Dim numberOfPages =
                Math.Max(
                    1,
                    CInt(
                        Math.Ceiling(
                            filteredCount / CDbl(PageSize)
                        )
                    )
                )

            _page =
                Math.Max(1, Math.Min(_page, numberOfPages))

            Dim pageRows As New List(Of DataRowView)()


            Dim firstIndex As Integer = (_page - 1) * PageSize
            Dim lastIndex As Integer = Math.Min(filteredCount, _page * PageSize) - 1

            For rowIndex As Integer = firstIndex To lastIndex
                pageRows.Add(_filtered(rowIndex))
            Next


            DgVehicles.ItemsSource = pageRows

            Dim startRow = If(
                filteredCount = 0,
                0,
                (_page - 1) * PageSize + 1
            )

            Dim endRow =
                Math.Min(filteredCount, _page * PageSize)

            TxtPageInfo.Text =
                $"Showing {startRow}–{endRow} of {filteredCount} vehicles • {PageSize} per page"

            TxtCurrentPage.Text = _page.ToString()
            TxtPageCount.Text = $"of {numberOfPages}"

            BtnPrevious.IsEnabled = (_page > 1)
            BtnNext.IsEnabled = (_page < numberOfPages)

            TxtTotalBadge.Text =
                $"{_vehicles.Rows.Count} total"

            TxtRegisterSummary.Text =
                $"{filteredCount} matching records • Select View or Edit"

            Dim byStatus As New Dictionary(
                Of String, Integer
            )(StringComparer.OrdinalIgnoreCase)

            For Each originalRow As DataRow In _vehicles.Rows

                Dim state =
                    Convert.ToString(originalRow("status"))

                If Not byStatus.ContainsKey(state) Then
                    byStatus(state) = 0
                End If

                byStatus(state) += 1

            Next

            Dim countFor As Func(Of String, Integer) =
                Function(statusKey)
                    Return If(
                        byStatus.ContainsKey(statusKey),
                        byStatus(statusKey),
                        0
                    )
                End Function

            TxtSummary.Text =
                $"{_vehicles.Rows.Count} fleet vehicles · " &
                $"{countFor("Available")} available · " &
                $"{countFor("Reserved")} reserved · " &
                $"{countFor("Rented")} rented · " &
                $"{countFor("Maintenance")} under maintenance"

        End Sub

        Private Sub Previous_Click(
            sender As Object,
            e As RoutedEventArgs
        )

            _page -= 1
            UpdatePage()

        End Sub

        Private Sub Next_Click(
            sender As Object,
            e As RoutedEventArgs
        )

            _page += 1
            UpdatePage()

        End Sub

        ' ========================================
        ' ADD WINDOW
        ' ========================================

        Private Sub Add_Click(
            sender As Object,
            e As RoutedEventArgs
        )

            OpenWindow("Add", Nothing)

        End Sub

        ' ========================================
        ' VIEW WINDOW
        ' ========================================

        Private Sub ViewRow_Click(
            sender As Object,
            e As RoutedEventArgs
        )

            Dim vehicleRow =
                TryCast(
                    DirectCast(sender, Button).DataContext,
                    DataRowView
                )

            If vehicleRow IsNot Nothing Then
                OpenWindow("View", vehicleRow)
            End If

        End Sub

        ' ========================================
        ' EDIT WINDOW
        ' ========================================

        Private Sub EditRow_Click(
            sender As Object,
            e As RoutedEventArgs
        )

            Dim vehicleRow =
                TryCast(
                    DirectCast(sender, Button).DataContext,
                    DataRowView
                )

            If vehicleRow IsNot Nothing Then
                OpenWindow("Edit", vehicleRow)
            End If

        End Sub

        Private Sub Grid_DoubleClick(
            sender As Object,
            e As MouseButtonEventArgs
        )

            Dim vehicleRow =
                TryCast(DgVehicles.SelectedItem, DataRowView)

            If vehicleRow IsNot Nothing Then
                OpenWindow("View", vehicleRow)
            End If

        End Sub

        Private Sub OpenWindow(
            mode As String,
            vehicleRow As DataRowView
        )

            Try
                Dim editor As New VehicleEntryDialog(
                    mode,
                    vehicleRow
                )

                Dim ownerWindow = Window.GetWindow(Me)

                If ownerWindow IsNot Nothing Then
                    editor.Owner = ownerWindow
                End If

                If editor.ShowDialog() = True Then
                    ReloadCategories()
                    ReloadVehicles()
                End If

            Catch ex As Exception
                MessageBox.Show(
                    "Could not open vehicle window: " & ex.Message,
                    "Fleet Inventory",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                )
            End Try

        End Sub

        ' ========================================
        ' EXPORT CSV
        ' ========================================

        Private Sub Export_Click(
            sender As Object,
            e As RoutedEventArgs
        )

            If _filtered Is Nothing Then Return

            Dim picker As New SaveFileDialog With {
                .Title = "Export fleet records",
                .FileName = "AutoRent_Fleet.csv",
                .Filter = "CSV files (*.csv)|*.csv"
            }

            If picker.ShowDialog() <> True Then Return

            Try
                Dim lines As New List(Of String) From {
                    "Vehicle ID,Plate,Make,Model,Year,Category,Mileage (km),Daily Rate,Status"
                }

                Dim names = New String() {
                    "display_id",
                    "plate",
                    "make",
                    "model",
                    "model_year",
                    "category",
                    "mileage",
                    "daily_rate",
                    "status"
                }

                For Each vehicleRow As DataRowView In _filtered

                    Dim values =
                        names.Select(
                            Function(columnName)
                                Return CsvCell(
                                    ViewHelpers.Str(
                                        vehicleRow,
                                        columnName
                                    )
                                )
                            End Function
                        )

                    lines.Add(String.Join(",", values))

                Next

                File.WriteAllLines(
                    picker.FileName,
                    lines,
                    New UTF8Encoding(True)
                )

                MessageBox.Show(
                    $"Exported {_filtered.Count} vehicles.",
                    "Export complete",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                )

            Catch ex As Exception
                MessageBox.Show(
                    "Export failed: " & ex.Message,
                    "Fleet Inventory",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                )
            End Try

        End Sub

        Private Shared Function CsvCell(
            value As String
        ) As String

            Return """" &
                If(value, "").Replace("""", """""") &
                """"

        End Function

    End Class

    ' ============================================
    ' SEPARATE ADD / VIEW / EDIT WINDOW
    ' Built entirely in VB.NET.
    ' No additional XAML file needed.
    ' ============================================

    Public Class VehicleEntryDialog
        Inherits Window

        Private ReadOnly _mode As String
        Private ReadOnly _vehicle As DataRowView

        Private ReadOnly _plate As New TextBox()
        Private ReadOnly _make As New TextBox()
        Private ReadOnly _model As New TextBox()
        Private ReadOnly _year As New TextBox()
        Private ReadOnly _mileage As New TextBox()

        Private ReadOnly _category As New ComboBox()
        Private ReadOnly _status As New ComboBox()

        Private ReadOnly _photo As New Image()
        Private ReadOnly _photoLabel As New TextBlock()

        Private _selectedPhoto As String = Nothing
        Private _oldPhoto As String = Nothing
        Private _removePhoto As Boolean = False

        Private Shared ReadOnly DarkText As Brush =
            New SolidColorBrush(
                System.Windows.Media.Color.FromRgb(47, 43, 45)
            )

        Private Shared ReadOnly Accent As Brush =
            New SolidColorBrush(
                System.Windows.Media.Color.FromRgb(118, 9, 31)
            )

        Public Sub New(
            mode As String,
            vehicleRow As DataRowView
        )

            _mode = mode
            _vehicle = vehicleRow

            Title = If(
                mode = "Add",
                "Add Vehicle",
                If(
                    mode = "Edit",
                    "Edit Vehicle",
                    "Vehicle Details"
                )
            )

            Width = 760
            Height = 675
            MinWidth = 640
            MinHeight = 570

            WindowStartupLocation =
                WindowStartupLocation.CenterOwner

            Background = Brushes.White
            FontFamily = New FontFamily("Segoe UI")
            ResizeMode = ResizeMode.CanResize

            BuildWindow()
            FillValues()

        End Sub

        ' ========================================
        ' DIALOG STYLING HELPERS
        ' ========================================

        Private Shared Function TextLabel(
            caption As String,
            Optional size As Double = 12,
            Optional strong As Boolean = False
        ) As TextBlock

            Return New TextBlock With {
                .Text = caption,
                .FontSize = size,
                .FontWeight = If(
                    strong,
                    FontWeights.SemiBold,
                    FontWeights.Normal
                ),
                .Foreground = DarkText,
                .TextTrimming = TextTrimming.CharacterEllipsis
            }

        End Function

        Private Shared Function StyledButton(
            labelText As String,
            primary As Boolean
        ) As Button

            Return New Button With {
                .Content = labelText,
                .Height = 37,
                .Padding = New Thickness(17, 0, 17, 0),
                .Margin = New Thickness(0, 0, 9, 0),
                .Cursor = Cursors.Hand,
                .FontWeight = FontWeights.SemiBold,
                .FontSize = 12,
                .Foreground = If(
                    primary,
                    Brushes.White,
                    DarkText
                ),
                .Background = If(
                    primary,
                    Accent,
                    Brushes.White
                ),
                .BorderBrush = If(
                    primary,
                    Accent,
                    Brushes.LightGray
                ),
                .BorderThickness = New Thickness(1)
            }

        End Function

        Private Shared Sub StyleTextInput(
            inputBox As TextBox
        )

            inputBox.Height = 37
            inputBox.Padding = New Thickness(9, 7, 9, 5)

            inputBox.BorderBrush =
                New SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(
                        223, 219, 220
                    )
                )

            inputBox.FontSize = 13

        End Sub

        Private Shared Function Field(
            caption As String,
            element As Control
        ) As StackPanel

            Dim container As New StackPanel With {
                .Margin = New Thickness(0, 0, 14, 15)
            }

            Dim header = TextLabel(caption, 11, True)
            header.Margin = New Thickness(0, 0, 0, 6)

            container.Children.Add(header)
            container.Children.Add(element)

            Return container

        End Function

        ' ========================================
        ' BUILD DIALOG WINDOW
        ' ========================================

        Private Sub BuildWindow()

            Dim root As New Grid()

            root.RowDefinitions.Add(
                New RowDefinition With {
                    .Height = GridLength.Auto
                }
            )

            root.RowDefinitions.Add(
                New RowDefinition With {
                    .Height = New GridLength(
                        1,
                        GridUnitType.Star
                    )
                }
            )

            root.RowDefinitions.Add(
                New RowDefinition With {
                    .Height = GridLength.Auto
                }
            )

            Content = root

            ' HEADER
            Dim heading As New StackPanel With {
                .Margin = New Thickness(26, 22, 26, 16)
            }

            Dim titleText = TextLabel(Title, 21, True)
            heading.Children.Add(titleText)

            Dim subheading = TextLabel(
                If(
                    _mode = "View",
                    "Vehicle information from your fleet records",
                    "Update vehicle details and its photo"
                ),
                12
            )

            subheading.Foreground = Brushes.Gray
            subheading.Margin = New Thickness(0, 5, 0, 0)

            heading.Children.Add(subheading)

            Grid.SetRow(heading, 0)
            root.Children.Add(heading)

            ' SCROLLABLE CONTENT
            Dim scroll As New ScrollViewer With {
                .VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            }

            Dim contents As New StackPanel With {
                .Margin = New Thickness(26, 0, 17, 10)
            }

            scroll.Content = contents
            Grid.SetRow(scroll, 1)
            root.Children.Add(scroll)

            ' PHOTO PANEL
            Dim photoPanel As New Border With {
                .Background =
                    New SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(
                            249, 247, 247
                        )
                    ),
                .BorderBrush =
                    New SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(
                            232, 228, 229
                        )
                    ),
                .BorderThickness = New Thickness(1),
                .CornerRadius = New CornerRadius(7),
                .Padding = New Thickness(13),
                .Margin = New Thickness(0, 0, 0, 19)
            }

            Dim photoRow As New StackPanel With {
                .Orientation = Orientation.Horizontal
            }

            Dim photoFrame As New Border With {
                .Width = 145,
                .Height = 98,
                .Background = Brushes.White,
                .CornerRadius = New CornerRadius(5),
                .ClipToBounds = True,
                .BorderBrush = Brushes.Gainsboro,
                .BorderThickness = New Thickness(1)
            }

            _photo.Stretch = Stretch.UniformToFill
            photoFrame.Child = _photo
            photoRow.Children.Add(photoFrame)

            Dim photoActions As New StackPanel With {
                .Margin = New Thickness(17, 5, 0, 0),
                .VerticalAlignment = VerticalAlignment.Center
            }

            photoActions.Children.Add(
                TextLabel("Vehicle photo", 13, True)
            )

            _photoLabel.Text = "No photo selected"
            _photoLabel.Foreground = Brushes.Gray
            _photoLabel.FontSize = 11
            _photoLabel.Margin = New Thickness(0, 6, 0, 10)

            photoActions.Children.Add(_photoLabel)

            If _mode <> "View" Then

                Dim buttons As New StackPanel With {
                    .Orientation = Orientation.Horizontal
                }

                Dim browse =
                    StyledButton("Choose Photo", False)

                AddHandler browse.Click,
                    AddressOf ChoosePhoto

                buttons.Children.Add(browse)

                Dim remove =
                    StyledButton("Remove", False)

                AddHandler remove.Click,
                    AddressOf RemovePhoto

                buttons.Children.Add(remove)

                photoActions.Children.Add(buttons)

            End If

            photoRow.Children.Add(photoActions)
            photoPanel.Child = photoRow
            contents.Children.Add(photoPanel)

            ' TEXT INPUTS
            For Each inputBox As TextBox In New TextBox() {
                _plate,
                _make,
                _model,
                _year,
                _mileage
            }

                StyleTextInput(inputBox)
                inputBox.IsReadOnly = (_mode = "View")

            Next

            _plate.MaxLength = 20
            _make.MaxLength = 50
            _model.MaxLength = 50
            _year.MaxLength = 4

            ' COMBOBOXES
            For Each combo As ComboBox In New ComboBox() {
                _category,
                _status
            }

                combo.Height = 37
                combo.FontSize = 12
                combo.BorderBrush = Brushes.LightGray

                combo.VerticalContentAlignment =
                    VerticalAlignment.Center

                combo.IsEnabled = (_mode <> "View")

            Next

            ' CATEGORY RECORDS
            Dim categories = RateRepository.GetCategories()

            For Each categoryRow As DataRow In categories.Rows

                _category.Items.Add(
                    Convert.ToString(
                        categoryRow("category")
                    )
                )

            Next

            ' STATUS RECORDS
            For Each statusValue As String In VehicleRepository.Statuses
                _status.Items.Add(statusValue)
            Next

            ' TWO-COLUMN FORM
            Dim columns As New UniformGrid With {
                .Columns = 2
            }

            columns.Children.Add(Field("Plate number", _plate))
            columns.Children.Add(Field("Make", _make))
            columns.Children.Add(Field("Model", _model))
            columns.Children.Add(Field("Model year", _year))
            columns.Children.Add(Field("Category", _category))
            columns.Children.Add(Field("Mileage (km)", _mileage))
            columns.Children.Add(Field("Current status", _status))

            contents.Children.Add(columns)

            ' VEHICLE INFORMATION
            If _mode <> "Add" AndAlso
               _vehicle IsNot Nothing Then

                Dim information = TextLabel(
                    "Daily rate: ₱" &
                    ViewHelpers.DecOf(
                        _vehicle,
                        "daily_rate"
                    ).ToString("N2") &
                    "  •  Vehicle ID: VH-" &
                    ViewHelpers.IntOf(
                        _vehicle,
                        "vehicle_id"
                    ).ToString("0000"),
                    12
                )

                information.Foreground = Brushes.Gray
                information.Margin =
                    New Thickness(0, 1, 0, 12)

                contents.Children.Add(information)

            End If

            ' FOOTER
            Dim footer As New DockPanel With {
                .Margin = New Thickness(26, 14, 17, 14)
            }

            Grid.SetRow(footer, 2)
            root.Children.Add(footer)

            ' DELETE BUTTON IN EDIT MODE
            If _mode = "Edit" Then

                Dim deleteButton =
                    StyledButton("Delete Vehicle", False)

                deleteButton.Foreground = Brushes.Firebrick

                AddHandler deleteButton.Click,
                    AddressOf DeleteVehicle

                DockPanel.SetDock(deleteButton, Dock.Left)
                footer.Children.Add(deleteButton)

            End If

            Dim rightSide As New StackPanel With {
                .Orientation = Orientation.Horizontal,
                .HorizontalAlignment = HorizontalAlignment.Right
            }

            Dim cancelButton =
                StyledButton(
                    If(_mode = "View", "Close", "Cancel"),
                    False
                )

            AddHandler cancelButton.Click,
                Sub(sender, args)
                    Me.Close()
                End Sub

            rightSide.Children.Add(cancelButton)

            If _mode <> "View" Then

                Dim saveButton =
                    StyledButton(
                        If(
                            _mode = "Add",
                            "Add Vehicle",
                            "Save Changes"
                        ),
                        True
                    )

                AddHandler saveButton.Click,
                    AddressOf SaveVehicle

                rightSide.Children.Add(saveButton)

            End If

            DockPanel.SetDock(rightSide, Dock.Right)
            footer.Children.Add(rightSide)

        End Sub

        ' ========================================
        ' FILL VEHICLE DETAILS
        ' ========================================

        Private Sub FillValues()

            If _vehicle Is Nothing Then

                _mileage.Text = "0"
                _status.SelectedItem = "Available"

                If _category.Items.Count > 0 Then
                    _category.SelectedIndex = 0
                End If

                Return

            End If

            _plate.Text =
                ViewHelpers.Str(_vehicle, "plate")

            _make.Text =
                ViewHelpers.Str(_vehicle, "make")

            _model.Text =
                ViewHelpers.Str(_vehicle, "model")

            _year.Text =
                ViewHelpers.Str(_vehicle, "model_year")

            _mileage.Text =
                ViewHelpers.IntOf(
                    _vehicle,
                    "mileage"
                ).ToString()

            _category.SelectedItem =
                ViewHelpers.Str(_vehicle, "category")

            _status.SelectedItem =
                ViewHelpers.Str(_vehicle, "status")

            _oldPhoto =
                ViewHelpers.Str(_vehicle, "image_path")

            ShowPreview(
                VehicleRepository.GetImageFilePath(
                    _oldPhoto
                )
            )

        End Sub

        ' ========================================
        ' PHOTO PREVIEW
        ' ========================================

        Private Sub ShowPreview(
            filePath As String
        )

            _photo.Source = Nothing

            If String.IsNullOrWhiteSpace(filePath) OrElse
               Not File.Exists(filePath) Then

                _photoLabel.Text = "No photo selected"
                Return

            End If

            Try
                Dim bitmap As New BitmapImage()

                bitmap.BeginInit()
                bitmap.CacheOption = BitmapCacheOption.OnLoad
                bitmap.DecodePixelWidth = 400
                bitmap.UriSource =
                    New Uri(filePath, UriKind.Absolute)
                bitmap.EndInit()

                bitmap.Freeze()

                _photo.Source = bitmap
                _photoLabel.Text = Path.GetFileName(filePath)

            Catch
                _photoLabel.Text = "Photo unavailable"
            End Try

        End Sub

        ' ========================================
        ' CHOOSE PHOTO
        ' ========================================

        Private Sub ChoosePhoto(
            sender As Object,
            e As RoutedEventArgs
        )

            Dim picker As New OpenFileDialog With {
                .Title = "Choose a vehicle picture",
                .Filter = "Image files|*.jpg;*.jpeg;*.png;*.bmp",
                .Multiselect = False
            }

            If picker.ShowDialog() <> True Then Return

            Try
                Dim selectedFile As New FileInfo(
                    picker.FileName
                )

                If selectedFile.Length >
                   5L * 1024L * 1024L Then

                    MessageBox.Show(
                        Me,
                        "Pictures must be 5 MB or smaller.",
                        "Vehicle photo",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    )

                    Return

                End If

                ShowPreview(selectedFile.FullName)

                If _photo.Source Is Nothing Then

                    MessageBox.Show(
                        Me,
                        "The selected picture could not be opened.",
                        "Vehicle photo",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    )

                    Return

                End If

                _selectedPhoto = selectedFile.FullName
                _removePhoto = False

            Catch ex As Exception

                MessageBox.Show(
                    Me,
                    "Could not load photo: " & ex.Message,
                    "Vehicle photo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                )

            End Try

        End Sub

        ' ========================================
        ' REMOVE PHOTO
        ' ========================================

        Private Sub RemovePhoto(
            sender As Object,
            e As RoutedEventArgs
        )

            _photo.Source = Nothing
            _photoLabel.Text = "No photo selected"

            _selectedPhoto = Nothing
            _removePhoto = True

        End Sub

        ' ========================================
        ' VALIDATION
        ' ========================================

        Private Function ValidateForm(
            ByRef modelYear As Integer?,
            ByRef mileageValue As Integer,
            ByRef categoryValue As String,
            ByRef statusValue As String
        ) As String

            If String.IsNullOrWhiteSpace(_plate.Text) Then
                Return "Enter the plate number."
            End If

            If String.IsNullOrWhiteSpace(_make.Text) Then
                Return "Enter the vehicle make."
            End If

            If String.IsNullOrWhiteSpace(_model.Text) Then
                Return "Enter the vehicle model."
            End If

            If _category.SelectedItem Is Nothing Then
                Return "Choose a category. Add it under Rate Management if needed."
            End If

            categoryValue =
                Convert.ToString(_category.SelectedItem)

            statusValue =
                TryCast(_status.SelectedItem, String)

            If String.IsNullOrWhiteSpace(statusValue) Then
                Return "Choose a status."
            End If

            modelYear = Nothing

            If Not String.IsNullOrWhiteSpace(_year.Text) Then

                Dim yr As Integer

                If Not ViewHelpers.TryParseWhole(
                    _year.Text,
                    yr
                ) OrElse
                   yr < 1980 OrElse
                   yr > DateTime.Today.Year + 1 Then

                    Return $"Year must be between 1980 and {DateTime.Today.Year + 1}."

                End If

                modelYear = yr

            End If

            If Not ViewHelpers.TryParseWhole(
                _mileage.Text,
                mileageValue
            ) Then

                Return "Mileage must be a whole number of at least 0."

            End If

            Return Nothing

        End Function

        ' ========================================
        ' SAVE VEHICLE
        ' ========================================

        Private Sub SaveVehicle(
            sender As Object,
            e As RoutedEventArgs
        )

            Dim yearValue As Integer? = Nothing
            Dim mileageValue As Integer = 0
            Dim categoryValue As String = Nothing
            Dim statusValue As String = Nothing

            Dim problem = ValidateForm(
                yearValue,
                mileageValue,
                categoryValue,
                statusValue
            )

            If problem IsNot Nothing Then

                MessageBox.Show(
                    Me,
                    problem,
                    "Check vehicle details",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                )

                Return

            End If

            Dim imageSaved As String = Nothing
            Dim operationDone As Boolean = False

            Try
                Dim imagePath As String =
                    If(_removePhoto, Nothing, _oldPhoto)

                If Not String.IsNullOrWhiteSpace(
                    _selectedPhoto
                ) Then

                    imageSaved =
                        VehicleRepository.SaveImageToCategory(
                            _selectedPhoto,
                            categoryValue
                        )

                    imagePath = imageSaved

                End If

                If _mode = "Add" Then

                    problem = VehicleRepository.Add(
                        _plate.Text,
                        _make.Text,
                        _model.Text,
                        categoryValue,
                        yearValue,
                        mileageValue,
                        statusValue,
                        imagePath
                    )

                Else

                    problem = VehicleRepository.Update(
                        ViewHelpers.IntOf(
                            _vehicle,
                            "vehicle_id"
                        ),
                        _plate.Text,
                        _make.Text,
                        _model.Text,
                        categoryValue,
                        yearValue,
                        mileageValue,
                        statusValue,
                        imagePath
                    )

                End If

                If problem IsNot Nothing Then

                    MessageBox.Show(
                        Me,
                        problem,
                        "Cannot save vehicle",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    )

                    Return

                End If

                operationDone = True

                If Not String.Equals(
                    _oldPhoto,
                    imagePath,
                    StringComparison.OrdinalIgnoreCase
                ) Then

                    VehicleRepository.DeleteUnusedImage(
                        _oldPhoto
                    )

                End If

                DialogResult = True

            Catch ex As Exception

                MessageBox.Show(
                    Me,
                    "Unable to save vehicle: " & ex.Message,
                    "Fleet Inventory",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                )

            Finally

                If Not operationDone AndAlso
                   imageSaved IsNot Nothing Then

                    VehicleRepository.DeleteUnusedImage(
                        imageSaved
                    )

                End If

            End Try

        End Sub

        ' ========================================
        ' DELETE VEHICLE
        ' ========================================

        Private Sub DeleteVehicle(
            sender As Object,
            e As RoutedEventArgs
        )

            If _vehicle Is Nothing Then Return

            Dim confirmed = MessageBox.Show(
                Me,
                "Delete this vehicle? This cannot be undone." &
                Environment.NewLine &
                "Vehicles used in rentals or maintenance cannot be deleted.",
                "Delete vehicle",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            )

            If confirmed <> MessageBoxResult.Yes Then Return

            Try
                Dim problem = VehicleRepository.Delete(
                    ViewHelpers.IntOf(
                        _vehicle,
                        "vehicle_id"
                    )
                )

                If problem IsNot Nothing Then

                    MessageBox.Show(
                        Me,
                        problem,
                        "Cannot delete vehicle",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    )

                    Return

                End If

                VehicleRepository.DeleteUnusedImage(
                    _oldPhoto
                )

                DialogResult = True

            Catch ex As Exception

                MessageBox.Show(
                    Me,
                    "Could not delete vehicle: " & ex.Message,
                    "Fleet Inventory",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                )

            End Try

        End Sub

    End Class

End Namespace
