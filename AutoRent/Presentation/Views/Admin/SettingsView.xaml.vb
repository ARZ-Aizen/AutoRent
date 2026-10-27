Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Data.OleDb
Imports System.IO
Imports System.IO.Compression
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports Microsoft.Win32
Imports AutoRent.Common.Utilities
Imports AutoRent.Data.Context

Namespace Presentation.Views.Admin
    Public Class TableExportModel
        Public Property TableKey As String
        Public Property TableName As String
        Public Property Icon As String
        Public Property TableType As String
        Public Property TypeBadgeBg As SolidColorBrush
        Public Property TypeBadgeFg As SolidColorBrush
        Public Property FilterScope As String
        Public Property RecordCount As Integer
        Public Property FileName As String

        Public ReadOnly Property RecordCountDisplay As String
            Get
                Return $"{RecordCount:N0} records"
            End Get
        End Property
    End Class

    Partial Public Class SettingsView
        Inherits UserControl

        Private _tables As New List(Of TableExportModel)()

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub SettingsView_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Dim app = TryCast(System.Windows.Application.Current, Global.AutoRent.Application)
            Dim currentUser = app?.CurrentUser

            ' Access control guard: Strictly for System Administrator only (no manager, no rental staff)
            If currentUser IsNot Nothing AndAlso Not currentUser.IsAdmin Then
                AlertUtil.ShowWarning("Access Denied: Settings and centralized data exports are restricted strictly to System Administrators.", "Access Restricted", Window.GetWindow(Me))
                Me.IsEnabled = False
                Return
            End If


            ' Default to current year and current month
            Dim now = DateTime.Now
            For i As Integer = 0 To CmbYear.Items.Count - 1
                Dim item = TryCast(CmbYear.Items(i), ComboBoxItem)
                If item IsNot Nothing AndAlso item.Content?.ToString() = now.Year.ToString() Then
                    CmbYear.SelectedIndex = i
                    Exit For
                End If
            Next

            If now.Month >= 1 AndAlso now.Month <= 12 Then
                CmbMonth.SelectedIndex = now.Month ' Index 0 is "All Months", 1 is Jan, 10 is Oct
            End If

            RefreshTableCounts()
        End Sub

        Private Sub CmbFilter_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            If Not IsLoaded Then Return
            RefreshTableCounts()
        End Sub

        Private Sub BtnRefreshCounts_Click(sender As Object, e As RoutedEventArgs)
            RefreshTableCounts()
        End Sub

        Private Sub GetSelectedPeriod(ByRef monthIndex As Integer, ByRef yearValue As Integer, ByRef startDate As DateTime?, ByRef endDate As DateTime?, ByRef periodDisplay As String, ByRef periodSlug As String)
            Dim monthItem = TryCast(CmbMonth.SelectedItem, ComboBoxItem)
            Dim yearItem = TryCast(CmbYear.SelectedItem, ComboBoxItem)

            yearValue = If(yearItem IsNot Nothing AndAlso Integer.TryParse(yearItem.Content?.ToString(), yearValue), yearValue, DateTime.Now.Year)
            monthIndex = If(monthItem IsNot Nothing AndAlso Integer.TryParse(monthItem.Tag?.ToString(), monthIndex), monthIndex, 0)

            If monthIndex >= 1 AndAlso monthIndex <= 12 Then
                Dim monthName = monthItem.Content?.ToString()
                startDate = New DateTime(yearValue, monthIndex, 1)
                endDate = startDate.Value.AddMonths(1)
                periodDisplay = $"{monthName} {yearValue}"
                periodSlug = $"{yearValue}_{monthIndex:D2}_{monthName}"
            Else
                startDate = Nothing
                endDate = Nothing
                periodDisplay = $"All Months ({yearValue} Snapshot / Full Database)"
                periodSlug = $"{yearValue}_All_Months"
            End If
        End Sub

        Private Sub RefreshTableCounts()
            Dim monthIndex As Integer
            Dim yearValue As Integer
            Dim startDate As DateTime?
            Dim endDate As DateTime?
            Dim periodDisplay As String = ""
            Dim periodSlug As String = ""

            GetSelectedPeriod(monthIndex, yearValue, startDate, endDate, periodDisplay, periodSlug)

            Dim isMonthly As Boolean = (startDate.HasValue AndAlso endDate.HasValue)

            _tables = New List(Of TableExportModel) From {
                New TableExportModel With {
                    .TableKey = "Reservations",
                    .TableName = "Reservations",
                    .Icon = ChrW(&HE787),
                    .TableType = "Activity Log",
                    .TypeBadgeBg = New SolidColorBrush(Color.FromRgb(254, 243, 199)),
                    .TypeBadgeFg = New SolidColorBrush(Color.FromRgb(146, 64, 14)),
                    .FilterScope = If(isMonthly, $"Pickup, return or created date within {periodDisplay}", "Full reservations history"),
                    .FileName = $"01_Reservations_{periodSlug}.csv",
                    .RecordCount = QueryCountReservations(startDate, endDate)
                },
                New TableExportModel With {
                    .TableKey = "Dispatches",
                    .TableName = "Daily Dispatches",
                    .Icon = ChrW(&HE81D),
                    .TableType = "Activity Log",
                    .TypeBadgeBg = New SolidColorBrush(Color.FromRgb(254, 243, 199)),
                    .TypeBadgeFg = New SolidColorBrush(Color.FromRgb(146, 64, 14)),
                    .FilterScope = If(isMonthly, $"Scheduled dispatch timestamp within {periodDisplay}", "Full dispatch log register"),
                    .FileName = $"02_Dispatches_{periodSlug}.csv",
                    .RecordCount = QueryCountDispatches(startDate, endDate)
                },
                New TableExportModel With {
                    .TableKey = "Maintenance",
                    .TableName = "Maintenance Schedules",
                    .Icon = ChrW(&HE90F),
                    .TableType = "Activity Log",
                    .TypeBadgeBg = New SolidColorBrush(Color.FromRgb(254, 243, 199)),
                    .TypeBadgeFg = New SolidColorBrush(Color.FromRgb(146, 64, 14)),
                    .FilterScope = If(isMonthly, $"Scheduled or completed date within {periodDisplay}", "Full maintenance work records"),
                    .FileName = $"03_Maintenance_{periodSlug}.csv",
                    .RecordCount = QueryCountMaintenance(startDate, endDate)
                },
                New TableExportModel With {
                    .TableKey = "Vehicles",
                    .TableName = "Fleet Vehicles",
                    .Icon = ChrW(&HE804),
                    .TableType = "Master Register",
                    .TypeBadgeBg = New SolidColorBrush(Color.FromRgb(236, 253, 245)),
                    .TypeBadgeFg = New SolidColorBrush(Color.FromRgb(4, 120, 87)),
                    .FilterScope = "Active vehicle inventory & master fleet register snapshot",
                    .FileName = $"04_Fleet_Vehicles_{periodSlug}.csv",
                    .RecordCount = QueryCountMaster("Vehicles")
                },
                New TableExportModel With {
                    .TableKey = "Customers",
                    .TableName = "Customer Directory",
                    .Icon = ChrW(&HE716),
                    .TableType = "Master Register",
                    .TypeBadgeBg = New SolidColorBrush(Color.FromRgb(236, 253, 245)),
                    .TypeBadgeFg = New SolidColorBrush(Color.FromRgb(4, 120, 87)),
                    .FilterScope = "Registered drivers directory & verified license snapshot",
                    .FileName = $"05_Customers_{periodSlug}.csv",
                    .RecordCount = QueryCountMaster("Customers")
                },
                New TableExportModel With {
                    .TableKey = "Rates",
                    .TableName = "Rental Rates & Tiers",
                    .Icon = ChrW(&HE8EC),
                    .TableType = "Master Register",
                    .TypeBadgeBg = New SolidColorBrush(Color.FromRgb(236, 253, 245)),
                    .TypeBadgeFg = New SolidColorBrush(Color.FromRgb(4, 120, 87)),
                    .FilterScope = "Vehicle category rate cards, weekly & monthly discount tariffs",
                    .FileName = $"06_Rental_Rates_{periodSlug}.csv",
                    .RecordCount = QueryCountMaster("Rates")
                },
                New TableExportModel With {
                    .TableKey = "Users",
                    .TableName = "User Accounts & Security",
                    .Icon = ChrW(&HE77B),
                    .TableType = "Security Register",
                    .TypeBadgeBg = New SolidColorBrush(Color.FromRgb(238, 242, 255)),
                    .TypeBadgeFg = New SolidColorBrush(Color.FromRgb(67, 56, 202)),
                    .FilterScope = "System user accounts & branch permission roles (Audit Log)",
                    .FileName = $"07_Users_{periodSlug}.csv",
                    .RecordCount = QueryCountMaster("Users")
                }
            }

            GridTables.ItemsSource = _tables

            Dim totalRecords = _tables.Sum(Function(t) t.RecordCount)
            TxtStatus.Text = $"Filtered for {periodDisplay} · {_tables.Count} system tables ready for export ({totalRecords:N0} total records)."
        End Sub

        ' ========================================================
        ' RECORD COUNT HELPERS
        ' ========================================================

        Private Function QueryCountReservations(startDate As DateTime?, endDate As DateTime?) As Integer
            Try
                If startDate.HasValue AndAlso endDate.HasValue Then
                    Return Db.Count("SELECT COUNT(*) FROM [Reservations] WHERE ([pickup_date] >= ? AND [pickup_date] < ?) OR ([return_date] >= ? AND [return_date] < ?) OR ([created_at] >= ? AND [created_at] < ?)",
                                    Db.Dt(startDate.Value), Db.Dt(endDate.Value),
                                    Db.Dt(startDate.Value), Db.Dt(endDate.Value),
                                    Db.Dt(startDate.Value), Db.Dt(endDate.Value))
                Else
                    Return Db.Count("SELECT COUNT(*) FROM [Reservations]")
                End If
            Catch ex As Exception
                Return 0
            End Try
        End Function

        Private Function QueryCountDispatches(startDate As DateTime?, endDate As DateTime?) As Integer
            Try
                If startDate.HasValue AndAlso endDate.HasValue Then
                    Return Db.Count("SELECT COUNT(*) FROM [Dispatches] WHERE [scheduled_at] >= ? AND [scheduled_at] < ?",
                                    Db.Dt(startDate.Value), Db.Dt(endDate.Value))
                Else
                    Return Db.Count("SELECT COUNT(*) FROM [Dispatches]")
                End If
            Catch ex As Exception
                Return 0
            End Try
        End Function

        Private Function QueryCountMaintenance(startDate As DateTime?, endDate As DateTime?) As Integer
            Try
                If startDate.HasValue AndAlso endDate.HasValue Then
                    Return Db.Count("SELECT COUNT(*) FROM [Maintenance] WHERE ([scheduled_date] >= ? AND [scheduled_date] < ?) OR ([completed_at] >= ? AND [completed_at] < ?) OR ([created_at] >= ? AND [created_at] < ?)",
                                    Db.Dt(startDate.Value), Db.Dt(endDate.Value),
                                    Db.Dt(startDate.Value), Db.Dt(endDate.Value),
                                    Db.Dt(startDate.Value), Db.Dt(endDate.Value))
                Else
                    Return Db.Count("SELECT COUNT(*) FROM [Maintenance]")
                End If
            Catch ex As Exception
                Return 0
            End Try
        End Function

        Private Function QueryCountMaster(tableName As String) As Integer
            Try
                Return Db.Count($"SELECT COUNT(*) FROM [{tableName}]")
            Catch ex As Exception
                Return 0
            End Try
        End Function

        ' ========================================================
        ' CSV DATA RETRIEVAL
        ' ========================================================

        Private Function GetTableCsv(tableKey As String, startDate As DateTime?, endDate As DateTime?) As String
            Select Case tableKey
                Case "Reservations"
                    Dim dt As DataTable
                    If startDate.HasValue AndAlso endDate.HasValue Then
                        dt = Db.Query("SELECT [reservation_id], [customer_name], [vehicle_id], [pickup_date], [return_date], [total], [status], [created_at] FROM [Reservations] WHERE ([pickup_date] >= ? AND [pickup_date] < ?) OR ([return_date] >= ? AND [return_date] < ?) OR ([created_at] >= ? AND [created_at] < ?) ORDER BY [reservation_id]",
                                      Db.Dt(startDate.Value), Db.Dt(endDate.Value),
                                      Db.Dt(startDate.Value), Db.Dt(endDate.Value),
                                      Db.Dt(startDate.Value), Db.Dt(endDate.Value))
                    Else
                        dt = Db.Query("SELECT [reservation_id], [customer_name], [vehicle_id], [pickup_date], [return_date], [total], [status], [created_at] FROM [Reservations] ORDER BY [reservation_id]")
                    End If
                    Return DataTableToCsv(dt, New String() {"Reservation ID", "Customer Name", "Vehicle ID", "Pickup Date", "Return Date", "Total (PHP)", "Status", "Created At"})

                Case "Dispatches"
                    Dim dt As DataTable
                    If startDate.HasValue AndAlso endDate.HasValue Then
                        dt = Db.Query("SELECT [dispatch_id], [reservation_id], [dispatch_type], [scheduled_at], [branch], [status] FROM [Dispatches] WHERE [scheduled_at] >= ? AND [scheduled_at] < ? ORDER BY [dispatch_id]",
                                      Db.Dt(startDate.Value), Db.Dt(endDate.Value))
                    Else
                        dt = Db.Query("SELECT [dispatch_id], [reservation_id], [dispatch_type], [scheduled_at], [branch], [status] FROM [Dispatches] ORDER BY [dispatch_id]")
                    End If
                    Return DataTableToCsv(dt, New String() {"Dispatch ID", "Reservation ID", "Dispatch Type", "Scheduled At", "Branch", "Status"})

                Case "Maintenance"
                    Dim dt As DataTable
                    If startDate.HasValue AndAlso endDate.HasValue Then
                        dt = Db.Query("SELECT [maintenance_id], [vehicle_id], [service_type], [scheduled_date], [completed_at], [cost], [status], [notes], [created_at] FROM [Maintenance] WHERE ([scheduled_date] >= ? AND [scheduled_date] < ?) OR ([completed_at] >= ? AND [completed_at] < ?) OR ([created_at] >= ? AND [created_at] < ?) ORDER BY [maintenance_id]",
                                      Db.Dt(startDate.Value), Db.Dt(endDate.Value),
                                      Db.Dt(startDate.Value), Db.Dt(endDate.Value),
                                      Db.Dt(startDate.Value), Db.Dt(endDate.Value))
                    Else
                        dt = Db.Query("SELECT [maintenance_id], [vehicle_id], [service_type], [scheduled_date], [completed_at], [cost], [status], [notes], [created_at] FROM [Maintenance] ORDER BY [maintenance_id]")
                    End If
                    Return DataTableToCsv(dt, New String() {"Maintenance ID", "Vehicle ID", "Service Type", "Scheduled Date", "Completed At", "Cost (PHP)", "Status", "Notes", "Created At"})

                Case "Vehicles"
                    Dim dt = Db.Query("SELECT [vehicle_id], [plate], [make], [model], [category], [model_year], [mileage], [status], [created_at] FROM [Vehicles] ORDER BY [vehicle_id]")
                    Return DataTableToCsv(dt, New String() {"Vehicle ID", "Plate Number", "Make", "Model", "Category", "Model Year", "Mileage (km)", "Status", "Registered Date"})

                Case "Customers"
                    Dim dt = Db.Query("SELECT [customer_id], [full_name], [phone], [email], [license_no], [created_at] FROM [Customers] ORDER BY [customer_id]")
                    Return DataTableToCsv(dt, New String() {"Customer ID", "Full Name", "Phone", "Email", "Driver License No", "Registered Date"})

                Case "Rates"
                    Dim dt = Db.Query("SELECT [rate_id], [category], [daily_rate], [weekly_rate], [monthly_rate], [deposit], [late_fee_per_day], [updated_at] FROM [Rates] ORDER BY [rate_id]")
                    Return DataTableToCsv(dt, New String() {"Rate ID", "Vehicle Category", "Daily Rate (PHP)", "Weekly Rate (PHP)", "Monthly Rate (PHP)", "Deposit (PHP)", "Late Fee Per Day (PHP)", "Last Updated"})

                Case "Users"
                    Dim dt = Db.Query("SELECT [user_id], [username], [full_name], [branch], [role], [is_active], [is_deleted], [last_login], [created_at] FROM [Users] ORDER BY [user_id]")
                    Return DataTableToCsv(dt, New String() {"User ID", "Username", "Full Name", "Branch", "Role", "Is Active", "Is Deleted", "Last Login", "Created At"})

                Case Else
                    Return ""
            End Select
        End Function

        Private Shared Function DataTableToCsv(table As DataTable, headerLabels As String()) As String
            Dim sb As New StringBuilder()

            If headerLabels IsNot Nothing AndAlso headerLabels.Length > 0 Then
                sb.AppendLine(String.Join(",", headerLabels.Select(Function(h) EscapeCsv(h))))
            ElseIf table.Columns.Count > 0 Then
                Dim cols As New List(Of String)()
                For Each col As DataColumn In table.Columns
                    cols.Add(EscapeCsv(col.ColumnName))
                Next
                sb.AppendLine(String.Join(",", cols))
            End If

            For Each row As DataRow In table.Rows
                Dim rowCells As New List(Of String)()
                For i As Integer = 0 To table.Columns.Count - 1
                    Dim val = row(i)
                    If IsDBNull(val) OrElse val Is Nothing Then
                        rowCells.Add("")
                    ElseIf TypeOf val Is DateTime Then
                        rowCells.Add(EscapeCsv(CDate(val).ToString("yyyy-MM-dd HH:mm:ss")))
                    ElseIf TypeOf val Is Decimal Then
                        rowCells.Add(EscapeCsv(CDec(val).ToString("F2")))
                    Else
                        rowCells.Add(EscapeCsv(val.ToString()))
                    End If
                Next
                sb.AppendLine(String.Join(",", rowCells))
            Next

            Return sb.ToString()
        End Function

        Private Shared Function EscapeCsv(val As String) As String
            If String.IsNullOrEmpty(val) Then Return """"
            Dim escaped = val.Replace("""", """""")
            Return $"""{escaped}"""
        End Function

        ' ========================================================
        ' CENTRALIZED EXPORT ALL ACTIONS
        ' ========================================================

        Private Sub BtnExportAllZip_Click(sender As Object, e As RoutedEventArgs)
            Dim monthIndex As Integer
            Dim yearValue As Integer
            Dim startDate As DateTime?
            Dim endDate As DateTime?
            Dim periodDisplay As String = ""
            Dim periodSlug As String = ""

            GetSelectedPeriod(monthIndex, yearValue, startDate, endDate, periodDisplay, periodSlug)

            Dim sfd As New SaveFileDialog With {
                .Title = $"Export All Tables ({periodDisplay}) - ZIP Archive",
                .FileName = $"AutoRent_Database_Export_{periodSlug}_{DateTime.Now:yyyyMMdd_HHmm}.zip",
                .DefaultExt = ".zip",
                .Filter = "ZIP Archive (*.zip)|*.zip"
            }

            If sfd.ShowDialog() <> True Then Return

            Try
                TxtStatus.Text = $"Exporting {_tables.Count} tables for {periodDisplay} to ZIP archive..."

                Dim app = TryCast(System.Windows.Application.Current, Global.AutoRent.Application)
                Dim exportedBy = If(app?.CurrentUser IsNot Nothing, app.CurrentUser.DisplayName, "System Administrator")

                Using fileStream As New FileStream(sfd.FileName, FileMode.Create, FileAccess.Write)
                    Using archive As New ZipArchive(fileStream, ZipArchiveMode.Create)
                        ' Add manifest
                        Dim manifestEntry = archive.CreateEntry("00_Export_Manifest.txt", CompressionLevel.Optimal)
                        Using writer As New StreamWriter(manifestEntry.Open(), New UTF8Encoding(True))
                            writer.WriteLine("==================================================================")
                            writer.WriteLine(" AUTORENT CENTRALIZED DATABASE EXPORT MANIFEST")
                            writer.WriteLine("==================================================================")
                            writer.WriteLine($"Exported At:       {DateTime.Now:yyyy-MM-dd HH:mm:ss}")
                            writer.WriteLine($"Exported By:       {exportedBy} (System Administrator)")
                            writer.WriteLine($"Selected Period:   {periodDisplay}")
                            writer.WriteLine($"Database Engine:   Microsoft Access (OLEDB 12.0)")
                            writer.WriteLine("------------------------------------------------------------------")
                            writer.WriteLine("TABLES INCLUDED IN THIS ARCHIVE:")
                            For Each tbl In _tables
                                writer.WriteLine($" - {tbl.FileName,-35} | {tbl.TableName,-24} | {tbl.RecordCount:N0} records")
                            Next
                            writer.WriteLine("------------------------------------------------------------------")
                            writer.WriteLine("NOTE: All files are encoded in RFC 4180 standard UTF-8 with BOM.")
                            writer.WriteLine("Activity tables are filtered by the selected reporting month.")
                            writer.WriteLine("==================================================================")
                        End Using

                        ' Add each table CSV
                        For Each tbl In _tables
                            Dim csvContent = GetTableCsv(tbl.TableKey, startDate, endDate)
                            Dim entry = archive.CreateEntry(tbl.FileName, CompressionLevel.Optimal)
                            Using writer As New StreamWriter(entry.Open(), New UTF8Encoding(True))
                                writer.Write(csvContent)
                            End Using
                        Next
                    End Using
                End Using

                TxtStatus.Text = $"Successfully exported all {_tables.Count} tables to '{Path.GetFileName(sfd.FileName)}' ({periodDisplay})."
                MessageBox.Show($"Successfully exported all {_tables.Count} tables for {periodDisplay} into ZIP archive:{Environment.NewLine}{Environment.NewLine}{sfd.FileName}",
                                "Export Completed", MessageBoxButton.OK, MessageBoxImage.Information)

            Catch ex As Exception
                TxtStatus.Text = $"Export failed: {ex.Message}"
                MessageBox.Show($"Export failed: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
        End Sub

        Private Sub BtnExportAllFolder_Click(sender As Object, e As RoutedEventArgs)
            Dim monthIndex As Integer
            Dim yearValue As Integer
            Dim startDate As DateTime?
            Dim endDate As DateTime?
            Dim periodDisplay As String = ""
            Dim periodSlug As String = ""

            GetSelectedPeriod(monthIndex, yearValue, startDate, endDate, periodDisplay, periodSlug)

            Dim dialog As New OpenFolderDialog With {
                .Title = $"Select Target Folder to Export All {_tables.Count} Tables ({periodDisplay})"
            }

            If dialog.ShowDialog() <> True Then Return

            Dim targetFolder = dialog.FolderName
            If String.IsNullOrWhiteSpace(targetFolder) OrElse Not Directory.Exists(targetFolder) Then Return

            Try
                TxtStatus.Text = $"Exporting 8 CSV tables to folder '{targetFolder}'..."

                Dim app = TryCast(System.Windows.Application.Current, Global.AutoRent.Application)
                Dim exportedBy = If(app?.CurrentUser IsNot Nothing, app.CurrentUser.DisplayName, "System Administrator")

                ' Write manifest
                Dim manifestPath = Path.Combine(targetFolder, $"00_Export_Manifest_{periodSlug}.txt")
                Dim manifestLines As New List(Of String) From {
                    "==================================================================",
                    " AUTORENT CENTRALIZED DATABASE EXPORT MANIFEST",
                    "==================================================================",
                    $"Exported At:       {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                    $"Exported By:       {exportedBy} (System Administrator)",
                    $"Selected Period:   {periodDisplay}",
                    $"Target Directory:  {targetFolder}",
                    "------------------------------------------------------------------",
                    "TABLES INCLUDED IN THIS EXPORT:"
                }
                For Each tbl In _tables
                    manifestLines.Add($" - {tbl.FileName,-35} | {tbl.TableName,-24} | {tbl.RecordCount:N0} records")
                Next
                manifestLines.Add("------------------------------------------------------------------")
                manifestLines.Add("NOTE: All files are encoded in RFC 4180 standard UTF-8 with BOM.")
                manifestLines.Add("==================================================================")
                File.WriteAllLines(manifestPath, manifestLines, New UTF8Encoding(True))

                ' Write each CSV
                For Each tbl In _tables
                    Dim csvContent = GetTableCsv(tbl.TableKey, startDate, endDate)
                    Dim filePath = Path.Combine(targetFolder, tbl.FileName)
                    File.WriteAllText(filePath, csvContent, New UTF8Encoding(True))
                Next

                TxtStatus.Text = $"Successfully exported all {_tables.Count} tables to '{targetFolder}' ({periodDisplay})."
                MessageBox.Show($"Successfully exported all {_tables.Count} tables for {periodDisplay} to folder:{Environment.NewLine}{Environment.NewLine}{targetFolder}",
                                "Export Completed", MessageBoxButton.OK, MessageBoxImage.Information)

            Catch ex As Exception
                TxtStatus.Text = $"Export failed: {ex.Message}"
                MessageBox.Show($"Export to folder failed: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
        End Sub

        Private Sub BtnExportSingleTable_Click(sender As Object, e As RoutedEventArgs)
            Dim btn = TryCast(sender, Button)
            Dim tableKey = btn?.Tag?.ToString()
            If String.IsNullOrEmpty(tableKey) Then Return

            Dim monthIndex As Integer
            Dim yearValue As Integer
            Dim startDate As DateTime?
            Dim endDate As DateTime?
            Dim periodDisplay As String = ""
            Dim periodSlug As String = ""

            GetSelectedPeriod(monthIndex, yearValue, startDate, endDate, periodDisplay, periodSlug)

            Dim tableModel = _tables.FirstOrDefault(Function(t) t.TableKey = tableKey)
            Dim defaultFileName = If(tableModel IsNot Nothing, tableModel.FileName, $"AutoRent_{tableKey}_{periodSlug}.csv")

            Dim sfd As New SaveFileDialog With {
                .Title = $"Export Table: {If(tableModel IsNot Nothing, tableModel.TableName, tableKey)} ({periodDisplay})",
                .FileName = defaultFileName,
                .DefaultExt = ".csv",
                .Filter = "CSV documents (*.csv)|*.csv"
            }

            If sfd.ShowDialog() <> True Then Return

            Try
                Dim csvContent = GetTableCsv(tableKey, startDate, endDate)
                File.WriteAllText(sfd.FileName, csvContent, New UTF8Encoding(True))

                TxtStatus.Text = $"Exported table '{tableKey}' to '{Path.GetFileName(sfd.FileName)}'."
                MessageBox.Show($"Successfully exported table '{If(tableModel IsNot Nothing, tableModel.TableName, tableKey)}' ({periodDisplay}) to CSV:{Environment.NewLine}{Environment.NewLine}{sfd.FileName}",
                                "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information)

            Catch ex As Exception
                MessageBox.Show($"Failed to export table: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
        End Sub
    End Class
End Namespace
