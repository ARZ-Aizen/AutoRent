Imports System.Drawing.Drawing2D
Imports System.Data.OleDb

Public Class AdminDashboard

    Private ReadOnly ActiveColor As Color = Color.FromArgb(122, 0, 16)
    Private ReadOnly NormalColor As Color = Color.FromArgb(36, 36, 36)

    Private ReadOnly _menuButtons As New List(Of Button)
    Private ReadOnly _panels As New Dictionary(Of Button, UserControl)
    Private _active As Button

    Private _editingId As Integer = 0          ' 0 = adding a new vehicle
    Private _selectedVehicleId As Integer = 0  ' vehicle currently shown in details
    Private _selectedMaintId As Integer = 0    ' maintenance record currently shown in details
    Private _revFrom As Date = Date.Today      ' revenue report range (inclusive)
    Private _revTo As Date = Date.Today
    Private _revBusy As Boolean = False
    Private _dispBusy As Boolean = False

    Private _fullName As String = "Alex Morgan"
    Private _role As String = "System Administrator"

    Public Sub New(fullName As String, role As String)
        Me.New()
        _fullName = fullName
        _role = role
    End Sub

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub AdminDashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' user info
        lblUserName.Text = _fullName
        lblUserRole.Text = _role.ToUpper()
        lblAvatar.Text = Initials(_fullName)
        lblTopAvatar.Text = Initials(_fullName)

        ' date
        lblDay.Text = Date.Now.ToString("dddd").ToUpper()
        lblDate.Text = Date.Now.ToString("dd MMM yyyy")

        ' round shapes
        MakeCircle(lblAvatar)
        MakeCircle(lblTopAvatar)
        MakeRounded(picLogo, 10)

        ' menu buttons
        _menuButtons.AddRange({btnMenuOverview, btnMenuFleet, btnMenuMaintenance, btnMenuRates, btnMenuRevenue,
                               btnMenuDispatch, btnMenuUsers, btnMenuAccess, btnMenuAudit, btnMenuSettings})
        For Each b In _menuButtons
            AddHandler b.Click, AddressOf MenuButton_Click
        Next

        Navigate(btnMenuOverview)
    End Sub

    Private Sub MenuButton_Click(sender As Object, e As EventArgs)
        Navigate(DirectCast(sender, Button))
    End Sub

    ' ---------------------------------------------------------------
    '  Navigation: each menu button shows its own UserControl panel
    ' ---------------------------------------------------------------
    Private Sub Navigate(btn As Button)
        If btn Is Nothing Then Return

        pnlContent.Controls.Clear()

        If Not _panels.ContainsKey(btn) Then
            _panels(btn) = CreatePanel(btn)
        End If

        Dim activePanel As UserControl = _panels(btn)

        ' always go back to the list when Fleet Inventory is clicked
        If TypeOf activePanel Is ucFleetInventory Then
            DirectCast(activePanel, ucFleetInventory).ShowRegister()
        End If
        If TypeOf activePanel Is ucMaintenance Then
            DirectCast(activePanel, ucMaintenance).ShowScheduler()
        End If

        activePanel.Dock = DockStyle.Fill
        activePanel.Visible = True
        pnlContent.Controls.Add(activePanel)
        activePanel.BringToFront()

        ' highlight active menu button
        For Each b In _menuButtons
            Dim isActive As Boolean = (b Is btn)
            b.BackColor = If(isActive, ActiveColor, NormalColor)
            b.ForeColor = If(isActive, Color.White, Color.FromArgb(205, 205, 205))
            b.Font = New Font("Segoe UI", 10.5F, If(isActive, FontStyle.Bold, FontStyle.Regular))
            b.FlatAppearance.MouseOverBackColor = If(isActive, ActiveColor, Color.FromArgb(60, 60, 60))
        Next
        _active = btn

        Dim title As String = If(btn.Tag IsNot Nothing, btn.Tag.ToString(), btn.Text)
        lblCrumb.Text = title & " / " & If(btn Is btnMenuOverview, "Admin Overview Dashboard", title)
    End Sub

    Private Function CreatePanel(btn As Button) As UserControl
        If btn Is btnMenuOverview Then
            Dim ov As New ucAdminOverview()
            AddHandler ov.btnViewAuditLogs.Click, Sub() Navigate(btnMenuAudit)
            LoadOverviewData(ov)
            Return ov

        ElseIf btn Is btnMenuFleet Then
            EnsureVehicleColumns()
            Dim fleet As New ucFleetInventory()
            LoadFleetData(fleet)
            AddHandler fleet.ViewVehicleRequested, Sub(rowIndex) FillVehicleDetails(fleet, rowIndex)
            AddHandler fleet.AddVehicleClicked, Sub() _editingId = 0
            AddHandler fleet.EditVehicleRequested,
                Sub(rowIndex)
                    _editingId = RowVehicleId(fleet, rowIndex)
                    LoadVehicleIntoEditor(fleet, _editingId)
                End Sub
            AddHandler fleet.Details.btnEditVehicle.Click,
                Sub()
                    _editingId = _selectedVehicleId
                    LoadVehicleIntoEditor(fleet, _editingId)
                End Sub
            AddHandler fleet.Editor.SaveClicked, Sub() SaveVehicle(fleet)
            AddHandler fleet.Editor.RetireConfirmed, Sub() RetireVehicle(fleet)
            Return fleet

        ElseIf btn Is btnMenuMaintenance Then
            EnsureMaintenanceTable()
            Dim maint As New ucMaintenance()
            LoadMaintenanceData(maint)
            AddHandler maint.ScheduleClicked, Sub() SaveMaintenance(maint)
            AddHandler maint.RecordRequested, Sub(rowIndex) ShowMaintenanceRecord(maint, rowIndex)
            AddHandler maint.ServiceHistoryClicked, Sub() ShowServiceHistory(maint)
            AddHandler maint.Details.ClearForRentalClicked, Sub() ClearForRental(maint)
            Return maint
        ElseIf btn Is btnMenuRates Then
            EnsureRateRulesTable()
            Dim rates As New ucRateConfiguration()
            LoadRateEditor(rates)
            AddHandler rates.SaveDraftClicked, Sub() SaveRateRule(rates, False)
            AddHandler rates.KeepDraftClicked, Sub() SaveRateRule(rates, False)
            AddHandler rates.ReviewConfirmClicked, Sub() SaveRateRule(rates, True)
            AddHandler rates.CancelClicked, Sub() LoadRateEditor(rates)
            Return rates
        ElseIf btn Is btnMenuRevenue Then
            EnsureRentalColumns()
            Dim rev As New ucRevenueReports()
            _revFrom = Date.Today : _revTo = Date.Today
            LoadRevenueFilters(rev)
            LoadRevenueReport(rev)
            AddHandler rev.FilterChanged,
                Sub()
                    If _revBusy Then Return
                    _revFrom = rev.dtpDate.Value.Date : _revTo = _revFrom
                    LoadRevenueReport(rev)
                End Sub
            AddHandler rev.QuickRangeClicked,
                Sub(name)
                    Dim today As Date = Date.Today
                    Select Case name
                        Case "Today" : _revFrom = today : _revTo = today
                        Case "Yesterday" : _revFrom = today.AddDays(-1) : _revTo = _revFrom
                        Case "Last 7 Days" : _revFrom = today.AddDays(-6) : _revTo = today
                        Case "Last 30 Days" : _revFrom = today.AddDays(-29) : _revTo = today
                        Case Else : Return
                    End Select
                    _revBusy = True
                    rev.dtpDate.Value = _revTo
                    _revBusy = False
                    LoadRevenueReport(rev)
                End Sub
            AddHandler rev.ExportClicked, Sub() ExportGridCsv(rev.dgvTransactions, "revenue-report")
            Return rev

        ElseIf btn Is btnMenuDispatch Then
            EnsureRentalColumns()
            Dim disp As New ucDispatchReports()
            LoadDispatchFilters(disp)
            LoadDispatchReport(disp)
            AddHandler disp.FilterChanged, Sub() If Not _dispBusy Then LoadDispatchReport(disp)
            AddHandler disp.DispatchSelected, Sub(rowIndex) ShowSelectedDispatch(disp, rowIndex)
            AddHandler disp.ExportClicked, Sub() ExportGridCsv(disp.dgvDispatch, "dispatch-report")
            AddHandler disp.PrintReportClicked, Sub() ExportGridCsv(disp.dgvDispatch, "dispatch-report")
            Return disp
        ElseIf btn Is btnMenuUsers Then
            Return New ucUserManagement()
        ElseIf btn Is btnMenuAccess Then
            Return New ucAccessPermissions()
        ElseIf btn Is btnMenuAudit Then
            Return New ucAuditLogs()
        ElseIf btn Is btnMenuSettings Then
            Return New ucSystemSettings()
        End If
        Return New UserControl()
    End Function

    ' ---------------------------------------------------------------
    '  Overview data
    ' ---------------------------------------------------------------
    Private Sub LoadOverviewData(ov As ucAdminOverview)
        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()

                ' 1. Fleet KPI metrics
                Dim qryFleet = "SELECT " &
                           "SUM(IIf([Status]='Available', 1, 0)) AS AvailableCount, " &
                           "SUM(IIf([Status]='Rented', 1, 0)) AS RentedCount, " &
                           "SUM(IIf([Status]='Maintenance', 1, 0)) AS MaintenanceCount, " &
                           "COUNT(*) AS TotalCount " &
                           "FROM Vehicles"

                Using cmd As New OleDbCommand(qryFleet, conn)
                    Using reader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Dim total = If(IsDBNull(reader("TotalCount")), 0, Convert.ToInt32(reader("TotalCount")))
                            Dim available = If(IsDBNull(reader("AvailableCount")), 0, Convert.ToInt32(reader("AvailableCount")))
                            Dim rented = If(IsDBNull(reader("RentedCount")), 0, Convert.ToInt32(reader("RentedCount")))
                            Dim maintenance = If(IsDBNull(reader("MaintenanceCount")), 0, Convert.ToInt32(reader("MaintenanceCount")))

                            ov.lblTotalFleet.Text = total.ToString()
                            ov.lblAvailable.Text = available.ToString()
                            ov.lblActiveRentals.Text = rented.ToString()
                            ov.lblMaintenance.Text = maintenance.ToString()

                            If total > 0 Then
                                ov.lblUtilization.Text = ((CDbl(rented) / CDbl(total)) * 100).ToString("0.0") & "%"
                            Else
                                ov.lblUtilization.Text = "0.0%"
                            End If
                        End If
                    End Using
                End Using

                ' 2. Today's revenue
                Dim qryRevenue = "SELECT SUM(TotalAmount) AS TodayRev FROM Rentals WHERE RentalDate >= Date() AND RentalDate < Date() + 1"
                Using cmdRev As New OleDbCommand(qryRevenue, conn)
                    Dim revResult = cmdRev.ExecuteScalar()
                    Dim todayRev As Decimal = If(revResult Is DBNull.Value OrElse revResult Is Nothing, 0D, Convert.ToDecimal(revResult))
                    ov.lblTodayRevenue.Text = "$" & todayRev.ToString("N0")
                End Using

                ' 3. Recent activity grid
                Dim qryActivity = "SELECT TOP 10 TimestampField, StaffMember, ActionField, RelatedRecord, ResultField FROM AuditLogs ORDER BY TimestampField DESC"
                Using cmdAct As New OleDbCommand(qryActivity, conn)
                    Using adapter As New OleDbDataAdapter(cmdAct)
                        Dim dt As New DataTable()
                        adapter.Fill(dt)

                        Dim grid = ov.dgvActivity
                        grid.AutoGenerateColumns = False
                        grid.Columns.Clear()
                        grid.Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = "Timestamp", .DataPropertyName = "TimestampField"})
                        grid.Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = "Staff Member", .DataPropertyName = "StaffMember"})
                        grid.Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = "Action", .DataPropertyName = "ActionField"})
                        grid.Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = "Related Record", .DataPropertyName = "RelatedRecord"})
                        grid.Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = "Result", .DataPropertyName = "ResultField"})
                        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                        grid.DataSource = dt
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("LoadOverviewData Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ---------------------------------------------------------------
    '  Fleet Inventory data
    '  >>> Change the column names in the SELECT to match your Vehicles table <<<
    ' ---------------------------------------------------------------
    Private Sub LoadFleetData(fleet As ucFleetInventory)
        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                Dim q = "SELECT VehicleID, PlateNumber, Make, Model, VehicleYear, Category, Transmission, DailyRate, [Status] FROM Vehicles"
                Using cmd As New OleDbCommand(q, conn)
                    Using r = cmd.ExecuteReader()
                        fleet.dgvFleet.Rows.Clear()
                        While r.Read()
                            Dim rate As String = If(IsDBNull(r("DailyRate")), "", "$" & Convert.ToDecimal(r("DailyRate")).ToString("N0"))
                            fleet.dgvFleet.Rows.Add(
                                "VH-" & Convert.ToInt32(r("VehicleID")).ToString("0000"),
                                Nothing,
                                r("PlateNumber").ToString(),
                                (r("Make").ToString() & " " & r("Model").ToString()).Trim(),
                                r("VehicleYear").ToString(),
                                r("Category").ToString(),
                                r("Transmission").ToString(),
                                rate,
                                r("Status").ToString(),
                                "")
                        End While
                    End Using
                End Using
                fleet.lblTotal.Text = fleet.dgvFleet.Rows.Count & " total"
                fleet.lblSummary.Text = fleet.dgvFleet.Rows.Count & " fleet vehicles"
            End Using
        Catch ex As Exception
            MessageBox.Show("LoadFleetData Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Fills the details screen from the clicked row
    Private Sub FillVehicleDetails(fleet As ucFleetInventory, rowIndex As Integer)
        Dim row = fleet.dgvFleet.Rows(rowIndex)
        _selectedVehicleId = RowVehicleId(fleet, rowIndex)
        Dim d = fleet.Details
        d.lblVehicleName.Text = Convert.ToString(row.Cells("colModel").Value)
        d.lblVehicleSub.Text = Convert.ToString(row.Cells("colYear").Value)
        d.lblStatus.Text = Convert.ToString(row.Cells("colStatus").Value)
        d.lblRate.Text = Convert.ToString(row.Cells("colRate").Value)
        d.lblVehicleId.Text = Convert.ToString(row.Cells("colId").Value) & " / " & Convert.ToString(row.Cells("colPlate").Value)
        d.lblCategory.Text = Convert.ToString(row.Cells("colCategory").Value)
        d.lblFuel.Text = Convert.ToString(row.Cells("colTransmission").Value)
        d.lblSubtitle.Text = Convert.ToString(row.Cells("colId").Value) & " / " & Convert.ToString(row.Cells("colPlate").Value)
        lblCrumb.Text = "Fleet Inventory / Vehicle Details"
    End Sub

    ' ---------------------------------------------------------------
    '  Add / Edit vehicle
    ' ---------------------------------------------------------------

    ' Adds the extra columns to the Vehicles table once (errors are ignored when a column already exists)
    Private Sub EnsureVehicleColumns()
        Dim cols() As String = {
            "PlateNumber TEXT(30)", "Make TEXT(50)", "Model TEXT(50)", "VehicleYear INTEGER",
            "Category TEXT(30)", "Color TEXT(50)", "FuelType TEXT(30)", "Transmission TEXT(30)",
            "Seats INTEGER", "Odometer LONG", "AcquiredDate DATETIME", "DailyRate CURRENCY",
            "RegistrationExpiry DATETIME", "InsuranceExpiry DATETIME", "Notes MEMO"}
        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                For Each c In cols
                    Try
                        Using cmd As New OleDbCommand("ALTER TABLE Vehicles ADD COLUMN " & c, conn)
                            cmd.ExecuteNonQuery()
                        End Using
                    Catch
                        ' column already exists
                    End Try
                Next
            End Using
        Catch ex As Exception
            MessageBox.Show("EnsureVehicleColumns Error: " & ex.Message)
        End Try
    End Sub

    ' "VH-0001" -> 1
    Private Shared Function RowVehicleId(fleet As ucFleetInventory, rowIndex As Integer) As Integer
        Dim txt As String = Convert.ToString(fleet.dgvFleet.Rows(rowIndex).Cells("colId").Value)
        Dim n As Integer
        Integer.TryParse(txt.Replace("VH-", ""), n)
        Return n
    End Function

    Private Shared Function Txt(r As OleDbDataReader, col As String) As String
        Return If(IsDBNull(r(col)), "", r(col).ToString())
    End Function

    Private Sub LoadVehicleIntoEditor(fleet As ucFleetInventory, id As Integer)
        Dim ed = fleet.Editor
        ed.ClearForm()
        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                Using cmd As New OleDbCommand("SELECT * FROM Vehicles WHERE VehicleID = ?", conn)
                    cmd.Parameters.AddWithValue("?", id)
                    Using r = cmd.ExecuteReader()
                        If r.Read() Then
                            ed.txtVehicleId.Text = "VH-" & id.ToString("0000")
                            ed.txtPlate.Text = Txt(r, "PlateNumber")
                            ed.cboYear.Text = Txt(r, "VehicleYear")
                            ed.cboMake.Text = Txt(r, "Make")
                            ed.txtModel.Text = Txt(r, "Model")
                            ed.cboCategory.Text = Txt(r, "Category")
                            ed.txtColor.Text = Txt(r, "Color")
                            ed.cboFuel.Text = Txt(r, "FuelType")
                            ed.cboTransmission.Text = Txt(r, "Transmission")
                            ed.txtSeats.Text = Txt(r, "Seats")
                            ed.txtOdometer.Text = Txt(r, "Odometer")
                            ed.txtRate.Text = Txt(r, "DailyRate")
                            ed.txtNotes.Text = Txt(r, "Notes")
                            ed.lblStatusBadge.Text = Txt(r, "Status")
                            If Not IsDBNull(r("AcquiredDate")) Then
                                ed.dtpAcquired.Value = Convert.ToDateTime(r("AcquiredDate")) : ed.dtpAcquired.Checked = True
                            End If
                            If Not IsDBNull(r("RegistrationExpiry")) Then ed.dtpRegExpiry.Value = Convert.ToDateTime(r("RegistrationExpiry"))
                            If Not IsDBNull(r("InsuranceExpiry")) Then ed.dtpInsExpiry.Value = Convert.ToDateTime(r("InsuranceExpiry"))
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("LoadVehicleIntoEditor Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub SaveVehicle(fleet As ucFleetInventory)
        Dim ed = fleet.Editor

        ' required fields
        Dim missing As New List(Of String)
        If ed.txtPlate.Text.Trim() = "" Then missing.Add("Plate Number")
        If ed.cboYear.Text.Trim() = "" Then missing.Add("Year")
        If ed.cboMake.Text.Trim() = "" Then missing.Add("Make")
        If ed.txtModel.Text.Trim() = "" Then missing.Add("Model")
        If ed.cboCategory.Text.Trim() = "" Then missing.Add("Vehicle Category")
        If ed.cboFuel.Text.Trim() = "" Then missing.Add("Fuel Type")
        If ed.cboTransmission.Text.Trim() = "" Then missing.Add("Transmission")

        Dim seats As Integer, odo As Integer, rate As Decimal
        If Not Integer.TryParse(ed.txtSeats.Text, seats) Then missing.Add("Seating Capacity")
        If Not Integer.TryParse(ed.txtOdometer.Text, odo) Then missing.Add("Odometer Reading")
        If Not Decimal.TryParse(ed.txtRate.Text, rate) Then missing.Add("Base Daily Rate")

        If missing.Count > 0 Then
            ed.ShowSaveError(missing.Count & " required field" & If(missing.Count > 1, "s", "") & " missing / invalid",
                             String.Join(", ", missing))
            MessageBox.Show("Please complete: " & String.Join(", ", missing), "Missing information",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        ed.ShowSaveError("", "")

        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()

                ' plate must be unique
                Using chk As New OleDbCommand("SELECT COUNT(*) FROM Vehicles WHERE PlateNumber = ? AND VehicleID <> ?", conn)
                    chk.Parameters.AddWithValue("?", ed.txtPlate.Text.Trim())
                    chk.Parameters.AddWithValue("?", _editingId)
                    If Convert.ToInt32(chk.ExecuteScalar()) > 0 Then
                        MessageBox.Show("That plate number is already used by another vehicle.", "Duplicate plate",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If
                End Using

                Dim acquired As Object = If(ed.dtpAcquired.Checked, CObj(ed.dtpAcquired.Value.Date), DBNull.Value)
                Dim sql As String
                If _editingId = 0 Then
                    sql = "INSERT INTO Vehicles (PlateNumber, Make, Model, VehicleYear, Category, [Color], FuelType, Transmission, " &
                          "Seats, Odometer, AcquiredDate, DailyRate, RegistrationExpiry, InsuranceExpiry, [Notes], [Status]) " &
                          "VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,'Available')"
                Else
                    sql = "UPDATE Vehicles SET PlateNumber=?, Make=?, Model=?, VehicleYear=?, Category=?, [Color]=?, FuelType=?, " &
                          "Transmission=?, Seats=?, Odometer=?, AcquiredDate=?, DailyRate=?, RegistrationExpiry=?, " &
                          "InsuranceExpiry=?, [Notes]=? WHERE VehicleID=?"
                End If

                Using cmd As New OleDbCommand(sql, conn)
                    cmd.Parameters.AddWithValue("?", ed.txtPlate.Text.Trim())
                    cmd.Parameters.AddWithValue("?", ed.cboMake.Text.Trim())
                    cmd.Parameters.AddWithValue("?", ed.txtModel.Text.Trim())
                    cmd.Parameters.AddWithValue("?", Convert.ToInt32(ed.cboYear.Text))
                    cmd.Parameters.AddWithValue("?", ed.cboCategory.Text)
                    cmd.Parameters.AddWithValue("?", ed.txtColor.Text.Trim())
                    cmd.Parameters.AddWithValue("?", ed.cboFuel.Text)
                    cmd.Parameters.AddWithValue("?", ed.cboTransmission.Text)
                    cmd.Parameters.AddWithValue("?", seats)
                    cmd.Parameters.AddWithValue("?", odo)
                    cmd.Parameters.AddWithValue("?", acquired)
                    cmd.Parameters.AddWithValue("?", rate)
                    cmd.Parameters.AddWithValue("?", ed.dtpRegExpiry.Value.Date)
                    cmd.Parameters.AddWithValue("?", ed.dtpInsExpiry.Value.Date)
                    cmd.Parameters.AddWithValue("?", ed.txtNotes.Text)
                    If _editingId <> 0 Then cmd.Parameters.AddWithValue("?", _editingId)
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            MessageBox.Show(If(_editingId = 0, "Vehicle added.", "Vehicle updated."), "Fleet Inventory",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            ReloadFleet()
        Catch ex As Exception
            MessageBox.Show("SaveVehicle Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub RetireVehicle(fleet As ucFleetInventory)
        Dim ed = fleet.Editor
        If Not (ed.chkNoRental.Checked AndAlso ed.chkNoObligations.Checked) Then
            MessageBox.Show("Please confirm both checks before retiring this vehicle.", "Retire vehicle",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        If MessageBox.Show("Retire this vehicle? Its history is kept.", "Retire vehicle",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return
        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                Using cmd As New OleDbCommand("UPDATE Vehicles SET [Status] = 'Retired' WHERE VehicleID = ?", conn)
                    cmd.Parameters.AddWithValue("?", _editingId)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
            ReloadFleet()
        Catch ex As Exception
            MessageBox.Show("RetireVehicle Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' rebuilds the Fleet Inventory panel so the list shows fresh data
    Private Sub ReloadFleet()
        _panels.Remove(btnMenuFleet)
        Navigate(btnMenuFleet)
    End Sub

    ' ---------------------------------------------------------------
    '  Maintenance
    ' ---------------------------------------------------------------

    ' Creates the Maintenance table once (error is ignored when it already exists)
    Private Sub EnsureMaintenanceTable()
        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                Try
                    Using cmd As New OleDbCommand(
                        "CREATE TABLE Maintenance (MaintenanceID COUNTER PRIMARY KEY, VehicleID LONG, ServiceType TEXT(50), " &
                        "Description TEXT(255), ScheduledAt DATETIME, Odometer LONG, Provider TEXT(100), " &
                        "EstimatedCost CURRENCY, Priority TEXT(20), Notes MEMO, [Status] TEXT(30))", conn)
                        cmd.ExecuteNonQuery()
                    End Using
                Catch
                    ' table already exists
                End Try
            End Using
        Catch ex As Exception
            MessageBox.Show("EnsureMaintenanceTable Error: " & ex.Message)
        End Try
    End Sub

    Private Shared Function ScalarInt(conn As OleDbConnection, sql As String) As Integer
        Using cmd As New OleDbCommand(sql, conn)
            Dim o = cmd.ExecuteScalar()
            Return If(o Is Nothing OrElse IsDBNull(o), 0, Convert.ToInt32(o))
        End Using
    End Function

    Private Sub LoadMaintenanceData(m As ucMaintenance)
        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()

                ' KPI cards
                m.lblDueCount.Text = ScalarInt(conn, "SELECT COUNT(*) FROM Maintenance WHERE [Status]='Scheduled' AND ScheduledAt >= Now() AND ScheduledAt < Now() + 7").ToString()
                m.lblOverdueCount.Text = ScalarInt(conn, "SELECT COUNT(*) FROM Maintenance WHERE [Status]='Scheduled' AND ScheduledAt < Now()").ToString()
                m.lblUnderCount.Text = ScalarInt(conn, "SELECT COUNT(*) FROM Vehicles WHERE [Status]='Under Maintenance'").ToString()
                m.lblRecentCount.Text = ScalarInt(conn, "SELECT COUNT(*) FROM Maintenance WHERE [Status]='Completed' AND ScheduledAt >= Date() - 7").ToString()

                ' vehicle drop-down: "VH-0001 · PLATE"
                m.cboVehicle.Items.Clear()
                Using cmd As New OleDbCommand("SELECT VehicleID, PlateNumber FROM Vehicles ORDER BY VehicleID", conn)
                    Using r = cmd.ExecuteReader()
                        While r.Read()
                            m.cboVehicle.Items.Add("VH-" & Convert.ToInt32(r("VehicleID")).ToString("0000") & " · " & r("PlateNumber").ToString())
                        End While
                    End Using
                End Using

                ' upcoming appointments (list and table view)
                m.dgvUpcoming.Rows.Clear()
                m.dgvTable.Rows.Clear()
                Dim q = "SELECT MaintenanceID, VehicleID, ScheduledAt, ServiceType, Priority FROM Maintenance " &
                        "WHERE [Status]='Scheduled' AND ScheduledAt >= Date() ORDER BY ScheduledAt"
                Using cmd As New OleDbCommand(q, conn)
                    Using r = cmd.ExecuteReader()
                        While r.Read()
                            Dim id = "MT-" & Convert.ToInt32(r("MaintenanceID")).ToString("0000")
                            Dim veh = "VH-" & Convert.ToInt32(r("VehicleID")).ToString("0000")
                            Dim at = Convert.ToDateTime(r("ScheduledAt")).ToString("dd MMM · HH:mm")
                            m.dgvTable.Rows.Add(id, veh, at, r("ServiceType").ToString(), r("Priority").ToString())
                            If m.dgvUpcoming.Rows.Count < 5 Then
                                m.dgvUpcoming.Rows.Add(id, veh, at, r("ServiceType").ToString(), r("Priority").ToString())
                            End If
                        End While
                    End Using
                End Using

                ' calendar marks for this month
                m.Calendar.Marks.Clear()
                Dim first As New Date(Date.Today.Year, Date.Today.Month, 1)
                Using cmd As New OleDbCommand("SELECT ServiceType, VehicleID, ScheduledAt FROM Maintenance " &
                                              "WHERE [Status]='Scheduled' AND ScheduledAt >= ? AND ScheduledAt < ?", conn)
                    cmd.Parameters.AddWithValue("?", first)
                    cmd.Parameters.AddWithValue("?", first.AddMonths(1))
                    Using r = cmd.ExecuteReader()
                        While r.Read()
                            Dim d = Convert.ToDateTime(r("ScheduledAt")).Date
                            If Not m.Calendar.Marks.ContainsKey(d) Then m.Calendar.Marks(d) = New List(Of String)
                            m.Calendar.Marks(d).Add(r("ServiceType").ToString() & " · VH-" & Convert.ToInt32(r("VehicleID")).ToString("0000"))
                        End While
                    End Using
                End Using
                m.Calendar.Invalidate()
            End Using
        Catch ex As Exception
            MessageBox.Show("LoadMaintenanceData Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub SaveMaintenance(m As ucMaintenance)
        Dim missing As New List(Of String)
        If m.cboVehicle.SelectedIndex < 0 Then missing.Add("Vehicle")
        If m.cboType.SelectedIndex < 0 Then missing.Add("Type")
        If m.txtDescription.Text.Trim() = "" Then missing.Add("Description")
        If m.cboProvider.Text.Trim() = "" Then missing.Add("Service provider")
        If m.cboPriority.SelectedIndex < 0 Then missing.Add("Priority")
        Dim odo As Integer, cost As Decimal
        If Not Integer.TryParse(m.txtOdometer.Text, odo) Then missing.Add("Odometer")
        If Not Decimal.TryParse(m.txtCost.Text, cost) Then missing.Add("Estimated cost")
        If missing.Count > 0 Then
            MessageBox.Show("Please complete: " & String.Join(", ", missing), "Missing information",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            Dim vehicleId As Integer = Convert.ToInt32(m.cboVehicle.Text.Substring(3, 4))
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                Using cmd As New OleDbCommand(
                    "INSERT INTO Maintenance (VehicleID, ServiceType, Description, ScheduledAt, Odometer, Provider, " &
                    "EstimatedCost, Priority, [Notes], [Status]) VALUES (?,?,?,?,?,?,?,?,?,'Scheduled')", conn)
                    Dim sched As Date = m.dtpScheduled.Value
                    sched = New Date(sched.Year, sched.Month, sched.Day, sched.Hour, sched.Minute, 0)
                    cmd.Parameters.Add("?", OleDbType.Integer).Value = vehicleId
                    cmd.Parameters.Add("?", OleDbType.VarWChar).Value = m.cboType.Text
                    cmd.Parameters.Add("?", OleDbType.VarWChar).Value = m.txtDescription.Text.Trim()
                    cmd.Parameters.Add("?", OleDbType.Date).Value = sched
                    cmd.Parameters.Add("?", OleDbType.Integer).Value = odo
                    cmd.Parameters.Add("?", OleDbType.VarWChar).Value = m.cboProvider.Text.Trim()
                    cmd.Parameters.Add("?", OleDbType.Currency).Value = cost
                    cmd.Parameters.Add("?", OleDbType.VarWChar).Value = m.cboPriority.Text
                    cmd.Parameters.Add("?", OleDbType.LongVarWChar).Value = m.txtNotes.Text
                    cmd.ExecuteNonQuery()
                End Using
            End Using
            MessageBox.Show("Service scheduled.", "Maintenance", MessageBoxButtons.OK, MessageBoxIcon.Information)
            _panels.Remove(btnMenuMaintenance)
            Navigate(btnMenuMaintenance)
        Catch ex As Exception
            MessageBox.Show("SaveMaintenance Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ---------------------------------------------------------------
    '  Revenue + Dispatch reports (both read the Rentals table)
    ' ---------------------------------------------------------------

    ' Adds the extra Rentals columns once (errors are ignored when a column already exists)
    Private Sub EnsureRentalColumns()
        Dim cols() As String = {
            "BookingCode TEXT(20)", "VehicleID LONG", "CustomerName TEXT(100)", "StaffName TEXT(80)",
            "PickupAt DATETIME", "ExpectedReturnAt DATETIME", "ActualDispatchAt DATETIME", "ActualReturnAt DATETIME",
            "RentalCharge CURRENCY", "AdditionalFees CURRENCY", "DiscountAmount CURRENCY", "DepositAmount CURRENCY",
            "DepositRefunded CURRENCY", "AmountCollected CURRENCY", "PaymentMethod TEXT(30)",
            "PaymentStatus TEXT(20)", "ContractStatus TEXT(30)"}
        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                For Each colDef As String In cols
                    Try
                        Using cmd As New OleDbCommand("ALTER TABLE Rentals ADD COLUMN " & colDef, conn)
                            cmd.ExecuteNonQuery()
                        End Using
                    Catch
                        ' column already exists
                    End Try
                Next
            End Using
        Catch ex As Exception
            MessageBox.Show("EnsureRentalColumns Error: " & ex.Message)
        End Try
    End Sub

    Private Shared Sub AddParam(cmd As OleDbCommand, v As Object)
        If TypeOf v Is Date Then
            cmd.Parameters.Add("?", OleDbType.Date).Value = CDate(v)
        ElseIf TypeOf v Is Integer Then
            cmd.Parameters.Add("?", OleDbType.Integer).Value = CInt(v)
        Else
            cmd.Parameters.Add("?", OleDbType.VarWChar).Value = Convert.ToString(v)
        End If
    End Sub

    Private Shared Function MakeCmd(conn As OleDbConnection, sql As String, ps As IEnumerable(Of Object)) As OleDbCommand
        Dim cmd As New OleDbCommand(sql, conn)
        For Each prm As Object In ps
            AddParam(cmd, prm)
        Next
        Return cmd
    End Function

    Private Shared Function CountP(conn As OleDbConnection, sql As String, ParamArray ps() As Object) As Integer
        Using cmd = MakeCmd(conn, sql, ps)
            Dim o = cmd.ExecuteScalar()
            Return If(o Is Nothing OrElse IsDBNull(o), 0, Convert.ToInt32(o))
        End Using
    End Function

    Private Shared Function Dec(r As OleDbDataReader, i As Integer) As Decimal
        Return If(r.IsDBNull(i), 0D, Convert.ToDecimal(r.GetValue(i)))
    End Function

    Private Shared Function Txt(r As OleDbDataReader, i As Integer) As String
        Return If(r.IsDBNull(i), "", Convert.ToString(r.GetValue(i)))
    End Function

    Private Shared Function Money(v As Decimal) As String
        Return If(v < 0, "−$" & Math.Abs(v).ToString("N0"), "$" & v.ToString("N0"))
    End Function

    Private Shared Function Stamp(r As OleDbDataReader, i As Integer) As String
        Return If(r.IsDBNull(i), "—", Convert.ToDateTime(r.GetValue(i)).ToString("dd MMM" & vbLf & "HH:mm"))
    End Function

    ' saves a grid to a CSV file the user picks
    Private Sub ExportGridCsv(grid As DataGridView, baseName As String)
        Using dlg As New SaveFileDialog With {.Filter = "CSV file|*.csv", .FileName = baseName & "-" & Date.Today.ToString("yyyyMMdd") & ".csv"}
            If dlg.ShowDialog() <> DialogResult.OK Then Return
            Dim sb As New System.Text.StringBuilder()
            sb.AppendLine(String.Join(",", grid.Columns.Cast(Of DataGridViewColumn)().Select(Function(c) """" & c.HeaderText & """")))
            For Each row As DataGridViewRow In grid.Rows
                sb.AppendLine(String.Join(",", row.Cells.Cast(Of DataGridViewCell)().Select(
                    Function(c) """" & Convert.ToString(c.Value).Replace(vbLf, " ").Replace("""", """""") & """")))
            Next
            System.IO.File.WriteAllText(dlg.FileName, sb.ToString(), System.Text.Encoding.UTF8)
            MessageBox.Show("Saved: " & dlg.FileName, "Export", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Using
    End Sub

    ' ---------- Revenue report ----------
    Private Sub LoadRevenueFilters(rev As ucRevenueReports)
        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                Using cmd As New OleDbCommand("SELECT DISTINCT StaffName FROM Rentals WHERE StaffName IS NOT NULL", conn)
                    Using r = cmd.ExecuteReader()
                        While r.Read()
                            rev.cboStaff.Items.Add(r(0).ToString())
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("LoadRevenueFilters Error: " & ex.Message)
        End Try
    End Sub

    Private Function RevenueFrom(rev As ucRevenueReports, ps As List(Of Object)) As String
        Dim sql As String = " FROM Rentals AS r LEFT JOIN Vehicles AS v ON r.VehicleID = v.VehicleID " &
                            "WHERE r.RentalDate >= ? AND r.RentalDate < ?"
        ps.Add(_revFrom.Date) : ps.Add(_revTo.Date.AddDays(1))
        If rev.cboCategory.SelectedIndex > 0 Then sql &= " AND v.Category = ?" : ps.Add(rev.cboCategory.Text)
        If rev.cboPayment.SelectedIndex > 0 Then sql &= " AND r.PaymentMethod = ?" : ps.Add(rev.cboPayment.Text)
        If rev.cboStaff.SelectedIndex > 0 Then sql &= " AND r.StaffName = ?" : ps.Add(rev.cboStaff.Text)
        If rev.cboStatus.SelectedIndex > 0 Then sql &= " AND r.PaymentStatus = ?" : ps.Add(rev.cboStatus.Text)
        Return sql
    End Function

    Private Sub LoadRevenueReport(rev As ucRevenueReports)
        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                Dim ps As New List(Of Object)
                Dim fromSql As String = RevenueFrom(rev, ps)

                ' KPI totals
                Dim charges, collected, outstanding, depIn, depOut, fees, disc As Decimal
                Dim n As Integer
                Using cmd = MakeCmd(conn, "SELECT Sum(r.RentalCharge), Sum(r.AmountCollected), Sum(r.TotalAmount - r.AmountCollected), " &
                                    "Sum(r.DepositAmount), Sum(r.DepositRefunded), Sum(r.AdditionalFees), Sum(r.DiscountAmount), Count(*)" & fromSql, ps)
                    Using r = cmd.ExecuteReader()
                        If r.Read() Then
                            charges = Dec(r, 0) : collected = Dec(r, 1) : outstanding = Dec(r, 2)
                            depIn = Dec(r, 3) : depOut = Dec(r, 4) : fees = Dec(r, 5) : disc = Dec(r, 6)
                            n = Convert.ToInt32(r.GetValue(7))
                        End If
                    End Using
                End Using
                Dim net As Decimal = charges + fees - disc

                Dim dayText As String = If(_revFrom = _revTo, _revFrom.ToString("dd MMM yyyy"),
                                           _revFrom.ToString("dd MMM") & " – " & _revTo.ToString("dd MMM yyyy"))
                rev.lblSubtitle.Text = dayText & " · " & n & " recorded rental transactions"
                rev.lblCharges.Text = Money(charges)
                rev.lblCollected.Text = Money(collected)
                rev.lblOutstanding.Text = Money(outstanding)
                rev.lblDepositsIn.Text = Money(depIn)
                rev.lblDepositsOut.Text = Money(depOut)
                rev.lblFees.Text = Money(fees)
                rev.lblDiscounts.Text = Money(-disc)
                rev.lblCvfCharges.Text = Money(charges)
                rev.lblCvfFees.Text = "+" & Money(fees) & " / " & Money(-disc)
                rev.lblCvfNet.Text = Money(net) & " net"
                rev.lblRefreshing.Text = "Report refreshed " & Date.Now.ToString("HH:mm")
                rev.lblRegisterSub.Text = "Rental collections exclude deposits · transaction currency USD"
                rev.lblReconciled.Text = n & " record" & If(n = 1, "", "s") & " · reconciled"
                rev.lblSummary.Text = "Revenue " & Money(net) & " = " & Money(charges) & " + " & Money(fees) & " − " & Money(disc) &
                                      " · collected " & Money(collected) & " + outstanding " & Money(outstanding)
                rev.lblSummaryNote.Text = "Refundable deposits are liabilities, not revenue. Outstanding balances are not collected cash."

                ' revenue by category / payment collections
                rev.CategoryBars.SetItems(GroupBars(conn, "v.Category", "Sum(r.TotalAmount)", fromSql, ps))
                rev.PaymentBars.SetItems(GroupBars(conn, "r.PaymentMethod", "Sum(r.AmountCollected)", fromSql, ps))

                ' last 7 days trend (ends on the last day of the selected range)
                Dim days As New Dictionary(Of Date, Double)
                Dim tFrom As Date = _revTo.Date.AddDays(-6)
                Using cmd = MakeCmd(conn, "SELECT Int(r.RentalDate), Sum(r.TotalAmount) FROM Rentals AS r " &
                                    "WHERE r.RentalDate >= ? AND r.RentalDate < ? GROUP BY Int(r.RentalDate)",
                                    New Object() {tFrom, _revTo.Date.AddDays(1)})
                    Using r = cmd.ExecuteReader()
                        While r.Read()
                            days(DateTime.FromOADate(Convert.ToDouble(r.GetValue(0))).Date) = Convert.ToDouble(Dec(r, 1))
                        End While
                    End Using
                End Using
                Dim labels(6) As String, vals(6) As Double
                For i As Integer = 0 To 6
                    Dim d As Date = tFrom.AddDays(i)
                    labels(i) = d.ToString("d MMM")
                    vals(i) = If(days.ContainsKey(d), days(d), 0)
                Next
                rev.Trend.SetData(labels, vals)
                rev.lblTrendSub.Text = "Net revenue · " & _revTo.ToString("dd MMM") & " " & Money(CDec(vals(6)))

                ' transaction register
                rev.dgvTransactions.Rows.Clear()
                Using cmd = MakeCmd(conn, "SELECT TOP 20 r.RentalID, r.BookingCode, r.CustomerName, r.RentalDate, r.RentalCharge, " &
                                    "r.AdditionalFees, r.DepositAmount, r.AmountCollected, r.PaymentMethod, r.PaymentStatus, r.TotalAmount" &
                                    fromSql & " ORDER BY r.RentalDate DESC, r.RentalID DESC", ps)
                    Using r = cmd.ExecuteReader()
                        While r.Read()
                            Dim status As String = Txt(r, 9)
                            If status = "Partial" Then status &= " · " & Money(Dec(r, 10) - Dec(r, 7))
                            rev.dgvTransactions.Rows.Add(
                                "TX-" & Convert.ToInt32(r.GetValue(0)).ToString("0000"), Txt(r, 1), Txt(r, 2),
                                If(r.IsDBNull(3), "", Convert.ToDateTime(r.GetValue(3)).ToString("dd MMM")),
                                Money(Dec(r, 4)), Money(Dec(r, 5)), Money(Dec(r, 6)), Money(Dec(r, 7)), Txt(r, 8), status)
                        End While
                    End Using
                End Using
                rev.lblShowing.Text = "Showing " & rev.dgvTransactions.Rows.Count & " / " & n
                rev.lblDiscountNote.Text = "Discounts: " & Money(disc) & " · Deposits refunded: " & Money(depOut)
            End Using
        Catch ex As Exception
            MessageBox.Show("LoadRevenueReport Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function GroupBars(conn As OleDbConnection, groupCol As String, sumExpr As String, fromSql As String,
                               ps As List(Of Object)) As List(Of Tuple(Of String, String, Double))
        Dim rows As New List(Of Tuple(Of String, Decimal))
        Using cmd = MakeCmd(conn, "SELECT " & groupCol & ", " & sumExpr & fromSql & " GROUP BY " & groupCol, ps)
            Using r = cmd.ExecuteReader()
                While r.Read()
                    rows.Add(Tuple.Create(If(Txt(r, 0) = "", "Unassigned", Txt(r, 0)), Dec(r, 1)))
                End While
            End Using
        End Using
        Dim maxV As Decimal = If(rows.Count = 0, 1D, Math.Max(rows.Max(Function(x) x.Item2), 1D))
        Return rows.Select(Function(x) Tuple.Create(x.Item1, Money(x.Item2), CDbl(x.Item2 / maxV))).ToList()
    End Function

    ' ---------- Dispatch report ----------
    Private Sub LoadDispatchFilters(disp As ucDispatchReports)
        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                Using cmd As New OleDbCommand("SELECT VehicleID, PlateNumber FROM Vehicles ORDER BY VehicleID", conn)
                    Using r = cmd.ExecuteReader()
                        While r.Read()
                            disp.cboVehicle.Items.Add("VH-" & Convert.ToInt32(r("VehicleID")).ToString("0000") & " · " & r("PlateNumber").ToString())
                        End While
                    End Using
                End Using
                Using cmd As New OleDbCommand("SELECT DISTINCT StaffName FROM Rentals WHERE StaffName IS NOT NULL", conn)
                    Using r = cmd.ExecuteReader()
                        While r.Read()
                            disp.cboClerk.Items.Add(r(0).ToString())
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("LoadDispatchFilters Error: " & ex.Message)
        End Try
    End Sub

    Private Sub LoadDispatchReport(disp As ucDispatchReports)
        Try
            Dim d0 As Date = disp.dtpReportDate.Value.Date
            Dim d1 As Date = d0.AddDays(1)
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()

                Dim scheduled = CountP(conn, "SELECT COUNT(*) FROM Rentals WHERE PickupAt >= ? AND PickupAt < ?", d0, d1)
                Dim cancelled = CountP(conn, "SELECT COUNT(*) FROM Rentals WHERE PickupAt >= ? AND PickupAt < ? AND ContractStatus = 'Cancelled'", d0, d1)
                Dim checkOuts = CountP(conn, "SELECT COUNT(*) FROM Rentals WHERE ActualDispatchAt >= ? AND ActualDispatchAt < ?", d0, d1)
                Dim returns = CountP(conn, "SELECT COUNT(*) FROM Rentals WHERE ActualReturnAt >= ? AND ActualReturnAt < ?", d0, d1)
                Dim overdue = CountP(conn, "SELECT COUNT(*) FROM Rentals WHERE ContractStatus = 'Dispatched' AND ExpectedReturnAt < ? AND ActualReturnAt IS NULL", Date.Now)
                Dim notDisp = CountP(conn, "SELECT COUNT(*) FROM Rentals WHERE PickupAt >= ? AND PickupAt < ? AND ActualDispatchAt IS NULL AND ContractStatus <> 'Cancelled'", d0, d1)
                Dim rented = CountP(conn, "SELECT COUNT(*) FROM Vehicles WHERE [Status] = 'Rented'")
                Dim fleet = CountP(conn, "SELECT COUNT(*) FROM Vehicles")

                disp.lblSubtitle.Text = d0.ToString("dd MMM yyyy") & " · prepared at " & Date.Now.ToString("HH:mm")
                disp.lblScheduled.Text = scheduled.ToString()
                disp.lblScheduledSub.Text = (scheduled - cancelled - notDisp) & " out · " & cancelled & " cancelled · " & notDisp & " pending"
                disp.lblCheckOuts.Text = checkOuts.ToString() : disp.lblCheckOutsSub.Text = "Today's departures"
                disp.lblReturns.Text = returns.ToString() : disp.lblReturnsSub.Text = "Today's returns"
                disp.lblCancelled.Text = cancelled.ToString() : disp.lblCancelledSub.Text = "Before release"
                disp.lblOverdue.Text = overdue.ToString() : disp.lblOverdueSub.Text = "Not yet returned"
                disp.lblNotDispatched.Text = notDisp.ToString() : disp.lblNotDispatchedSub.Text = "Scheduled, still pending"
                disp.lblUtilization.Text = If(fleet > 0, (rented * 100.0 / fleet).ToString("0.0") & "%", "0.0%")
                disp.lblUtilizationSub.Text = rented & " / " & fleet & " vehicles"
                disp.lblSafetyTitle.Text = "Safety overrides are not permitted"
                disp.lblSafetyNote.Text = "Unresolved maintenance, an incomplete inspection or an invalid contract blocks dispatch. " &
                                          "Detailed records and exports are role protected and audited."
                disp.lblRegisterSub.Text = "Date + 24-hour time · — means not yet recorded"

                ' register
                Dim ps As New List(Of Object) From {d0, d1}
                Dim sql As String = "SELECT r.BookingCode, r.VehicleID, v.PlateNumber, r.CustomerName, r.StaffName, r.PickupAt, " &
                                    "r.ActualDispatchAt, r.ExpectedReturnAt, r.ActualReturnAt, r.ContractStatus " &
                                    "FROM Rentals AS r LEFT JOIN Vehicles AS v ON r.VehicleID = v.VehicleID " &
                                    "WHERE r.PickupAt >= ? AND r.PickupAt < ?"
                If disp.cboVehicle.SelectedIndex > 0 Then
                    sql &= " AND r.VehicleID = ?" : ps.Add(Convert.ToInt32(disp.cboVehicle.Text.Substring(3, 4)))
                End If
                If disp.cboClerk.SelectedIndex > 0 Then sql &= " AND r.StaffName = ?" : ps.Add(disp.cboClerk.Text)
                If disp.cboContractStatus.SelectedIndex > 0 Then sql &= " AND r.ContractStatus = ?" : ps.Add(disp.cboContractStatus.Text)
                sql &= " ORDER BY r.PickupAt"

                disp.dgvDispatch.Rows.Clear()
                Using cmd = MakeCmd(conn, sql, ps)
                    Using r = cmd.ExecuteReader()
                        While r.Read()
                            Dim vid As String = If(r.IsDBNull(1), "", "VH-" & Convert.ToInt32(r.GetValue(1)).ToString("0000"))
                            disp.dgvDispatch.Rows.Add(Txt(r, 0), vid, Txt(r, 2), Txt(r, 3), Txt(r, 4),
                                                      Stamp(r, 5), Stamp(r, 6), Stamp(r, 7), Stamp(r, 8), Txt(r, 9))
                        End While
                    End Using
                End Using
                disp.lblShowing.Text = "Showing " & disp.dgvDispatch.Rows.Count & " of " & scheduled & " scheduled records"
            End Using
        Catch ex As Exception
            MessageBox.Show("LoadDispatchReport Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ShowSelectedDispatch(disp As ucDispatchReports, rowIndex As Integer)
        Dim row = disp.dgvDispatch.Rows(rowIndex)
        Dim booking As String = Convert.ToString(row.Cells(0).Value)
        Dim vid As String = Convert.ToString(row.Cells(1).Value)
        disp.lblSelTitle.Text = "Selected dispatch · " & booking
        disp.lblSelSub.Text = "Authorized detailed-record preview"
        disp.lblSelVehicle.Text = vid & " / " & Convert.ToString(row.Cells(4).Value)
        disp.lblSelContract.Text = Convert.ToString(row.Cells(9).Value)
        disp.lblSelInspection.Text = "—"
        disp.lblSelFleetStatus.Text = "—"
        If vid.Length >= 7 Then
            Try
                Using conn As New OleDbConnection(DbHelper.ConnectionString)
                    conn.Open()
                    Using cmd As New OleDbCommand("SELECT [Status] FROM Vehicles WHERE VehicleID = ?", conn)
                        cmd.Parameters.Add("?", OleDbType.Integer).Value = Convert.ToInt32(vid.Substring(3, 4))
                        Dim o = cmd.ExecuteScalar()
                        If o IsNot Nothing AndAlso Not IsDBNull(o) Then disp.lblSelFleetStatus.Text = o.ToString()
                    End Using
                End Using
            Catch
            End Try
        End If
    End Sub

    ' ---------------------------------------------------------------
    '  Rate rules
    ' ---------------------------------------------------------------
    Private Sub EnsureRateRulesTable()
        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                Try
                    Using cmd As New OleDbCommand(
                        "CREATE TABLE RateRules (RuleID COUNTER PRIMARY KEY, RuleName TEXT(100), VehicleCategory TEXT(30), " &
                        "VehicleOverride TEXT(60), CalcMethod TEXT(50), RateValue DOUBLE, EffectiveStart DATETIME, " &
                        "EffectiveEnd DATETIME, ConditionText TEXT(80), Precedence INTEGER, ApplicableCharges TEXT(80), " &
                        "RuleState TEXT(20), ChangeReason TEXT(255), ModifiedBy TEXT(80), ModifiedAt DATETIME, [Status] TEXT(20))", conn)
                        cmd.ExecuteNonQuery()
                    End Using
                Catch
                    ' table already exists
                End Try
            End Using
        Catch ex As Exception
            MessageBox.Show("EnsureRateRulesTable Error: " & ex.Message)
        End Try
    End Sub

    ' fills the vehicle drop-down and resets the form
    Private Sub LoadRateEditor(ed As ucRateConfiguration)
        Try
            ed.cboOverride.Items.Clear()
            ed.cboOverride.Items.Add("None · category-wide rule")
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                Using cmd As New OleDbCommand("SELECT VehicleID, PlateNumber FROM Vehicles ORDER BY VehicleID", conn)
                    Using r = cmd.ExecuteReader()
                        While r.Read()
                            ed.cboOverride.Items.Add("VH-" & Convert.ToInt32(r("VehicleID")).ToString("0000") & " · " & r("PlateNumber").ToString())
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("LoadRateEditor Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        ed.ClearForm()
        ed.lblSubtitle.Text = "New rate rule · draft, not yet published"
        ed.lblUnsaved.Text = ""
    End Sub

    Private Sub SaveRateRule(ed As ucRateConfiguration, publish As Boolean)
        Dim missing As New List(Of String)
        Dim rate As Double, precedence As Integer
        If ed.txtRuleName.Text.Trim() = "" Then missing.Add("Rule name")
        If ed.cboCategory.SelectedIndex < 0 Then missing.Add("Vehicle category")
        If ed.cboMethod.SelectedIndex < 0 Then missing.Add("Calculation method")
        If Not Double.TryParse(ed.txtRate.Text, rate) Then missing.Add("Rate / multiplier")
        If ed.cboCondition.SelectedIndex < 0 Then missing.Add("Condition")
        If Not Integer.TryParse(ed.txtPrecedence.Text, precedence) Then missing.Add("Precedence")
        If ed.txtReason.Text.Trim() = "" Then missing.Add("Change reason")
        If missing.Count > 0 Then
            MessageBox.Show("Please complete: " & String.Join(", ", missing), "Missing information",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        If ed.dtpEnd.Value <= ed.dtpStart.Value Then
            MessageBox.Show("Effective end must be after effective start.", "Invalid dates",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        If publish AndAlso Not (ed.chkNewOnly.Checked AndAlso ed.chkNoFinalized.Checked) Then
            MessageBox.Show("Tick both confirmations before publishing.", "Confirm rate update",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()

                ' conflict check: same category + same precedence + overlapping dates
                Using chk As New OleDbCommand("SELECT TOP 1 RuleID, RuleName FROM RateRules WHERE VehicleCategory = ? AND Precedence = ? " &
                                              "AND EffectiveStart <= ? AND EffectiveEnd >= ? AND [Status] <> 'Archived'", conn)
                    chk.Parameters.Add("?", OleDbType.VarWChar).Value = ed.cboCategory.Text
                    chk.Parameters.Add("?", OleDbType.Integer).Value = precedence
                    chk.Parameters.Add("?", OleDbType.Date).Value = ed.dtpEnd.Value
                    chk.Parameters.Add("?", OleDbType.Date).Value = ed.dtpStart.Value
                    Using r = chk.ExecuteReader()
                        If r.Read() Then
                            ed.ShowConflict("Conflict validation · overlapping rule detected",
                                            "RR-" & Convert.ToInt32(r("RuleID")).ToString("0000") & " """ & r("RuleName").ToString() &
                                            """ has the same precedence in these dates. Change this rule's dates or precedence.")
                            Return
                        End If
                    End Using
                End Using
                ed.ShowConflict("", "")

                Dim start As Date = New Date(ed.dtpStart.Value.Year, ed.dtpStart.Value.Month, ed.dtpStart.Value.Day, ed.dtpStart.Value.Hour, ed.dtpStart.Value.Minute, 0)
                Dim [end] As Date = New Date(ed.dtpEnd.Value.Year, ed.dtpEnd.Value.Month, ed.dtpEnd.Value.Day, ed.dtpEnd.Value.Hour, ed.dtpEnd.Value.Minute, 0)
                Dim nowStamp As Date = New Date(Date.Now.Year, Date.Now.Month, Date.Now.Day, Date.Now.Hour, Date.Now.Minute, 0)

                Dim newId As Integer
                Using cmd As New OleDbCommand(
                    "INSERT INTO RateRules (RuleName, VehicleCategory, VehicleOverride, CalcMethod, RateValue, EffectiveStart, EffectiveEnd, " &
                    "ConditionText, Precedence, ApplicableCharges, RuleState, ChangeReason, ModifiedBy, ModifiedAt, [Status]) " &
                    "VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)", conn)
                    cmd.Parameters.Add("?", OleDbType.VarWChar).Value = ed.txtRuleName.Text.Trim()
                    cmd.Parameters.Add("?", OleDbType.VarWChar).Value = ed.cboCategory.Text
                    cmd.Parameters.Add("?", OleDbType.VarWChar).Value = ed.cboOverride.Text
                    cmd.Parameters.Add("?", OleDbType.VarWChar).Value = ed.cboMethod.Text
                    cmd.Parameters.Add("?", OleDbType.Double).Value = rate
                    cmd.Parameters.Add("?", OleDbType.Date).Value = start
                    cmd.Parameters.Add("?", OleDbType.Date).Value = [end]
                    cmd.Parameters.Add("?", OleDbType.VarWChar).Value = ed.cboCondition.Text
                    cmd.Parameters.Add("?", OleDbType.Integer).Value = precedence
                    cmd.Parameters.Add("?", OleDbType.VarWChar).Value = ed.cboCharges.Text
                    cmd.Parameters.Add("?", OleDbType.VarWChar).Value = If(ed.cboState.Text = "", "Active", ed.cboState.Text)
                    cmd.Parameters.Add("?", OleDbType.VarWChar).Value = ed.txtReason.Text.Trim()
                    cmd.Parameters.Add("?", OleDbType.VarWChar).Value = _fullName
                    cmd.Parameters.Add("?", OleDbType.Date).Value = nowStamp
                    cmd.Parameters.Add("?", OleDbType.VarWChar).Value = If(publish, "Published", "Draft")
                    cmd.ExecuteNonQuery()
                End Using
                Using cmd As New OleDbCommand("SELECT @@IDENTITY", conn)
                    newId = Convert.ToInt32(cmd.ExecuteScalar())
                End Using

                Dim ruleCode As String = "RR-" & newId.ToString("0000")
                MessageBox.Show(ruleCode & If(publish, " published.", " saved as draft."), "Rate Configuration",
                                MessageBoxButtons.OK, MessageBoxIcon.Information)
                LoadRateEditor(ed)
                ed.lblLastModified.Text = "Last modified: " & _fullName & " · " & nowStamp.ToString("dd MMM yyyy, HH:mm") & " · " & ruleCode
            End Using
        Catch ex As Exception
            MessageBox.Show("SaveRateRule Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' "Service history": shows every maintenance record (newest first) in the table view
    Private Sub ShowServiceHistory(m As ucMaintenance)
        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                m.dgvTable.Rows.Clear()
                Dim q = "SELECT MaintenanceID, VehicleID, ScheduledAt, ServiceType, Priority FROM Maintenance ORDER BY ScheduledAt DESC"
                Using cmd As New OleDbCommand(q, conn)
                    Using r = cmd.ExecuteReader()
                        While r.Read()
                            m.dgvTable.Rows.Add(
                                "MT-" & Convert.ToInt32(r("MaintenanceID")).ToString("0000"),
                                "VH-" & Convert.ToInt32(r("VehicleID")).ToString("0000"),
                                Convert.ToDateTime(r("ScheduledAt")).ToString("dd MMM yyyy · HH:mm"),
                                r("ServiceType").ToString(), r("Priority").ToString())
                        End While
                    End Using
                End Using
            End Using
            m.SwitchView(False)
        Catch ex As Exception
            MessageBox.Show("ShowServiceHistory Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' "Clear for rental": marks the job completed and the vehicle Available again
    Private Sub ClearForRental(m As ucMaintenance)
        Dim d = m.Details
        If _selectedMaintId = 0 Then Return
        If Not (d.chkWorkVerified.Checked AndAlso d.chkNoActiveRental.Checked AndAlso
                d.chkSafetyPassed.Checked AndAlso d.chkClearanceApproved.Checked) Then
            MessageBox.Show("Tick all four checks before clearing the vehicle for rental.", "Clear for rental",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        If MessageBox.Show("Clear this vehicle for rental?", "Clear for rental",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return
        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                Dim vehicleId As Integer
                Using cmd As New OleDbCommand("SELECT VehicleID FROM Maintenance WHERE MaintenanceID = ?", conn)
                    cmd.Parameters.Add("?", OleDbType.Integer).Value = _selectedMaintId
                    vehicleId = Convert.ToInt32(cmd.ExecuteScalar())
                End Using
                Using cmd As New OleDbCommand("UPDATE Maintenance SET [Status] = 'Completed' WHERE MaintenanceID = ?", conn)
                    cmd.Parameters.Add("?", OleDbType.Integer).Value = _selectedMaintId
                    cmd.ExecuteNonQuery()
                End Using
                Using cmd As New OleDbCommand("UPDATE Vehicles SET [Status] = 'Available' WHERE VehicleID = ?", conn)
                    cmd.Parameters.Add("?", OleDbType.Integer).Value = vehicleId
                    cmd.ExecuteNonQuery()
                End Using
            End Using
            MessageBox.Show("Vehicle cleared for rental.", "Maintenance", MessageBoxButtons.OK, MessageBoxIcon.Information)
            _panels.Remove(btnMenuMaintenance)
            _panels.Remove(btnMenuFleet)
            Navigate(btnMenuMaintenance)
        Catch ex As Exception
            MessageBox.Show("ClearForRental Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' fills the Maintenance Record Details screen for the clicked appointment
    Private Sub ShowMaintenanceRecord(m As ucMaintenance, rowIndex As Integer)
        Try
            Dim idText As String = Convert.ToString(m.dgvUpcoming.Rows(rowIndex).Cells(0).Value)
            Dim id As Integer = Convert.ToInt32(idText.Replace("MT-", ""))
            _selectedMaintId = id
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                Dim q = "SELECT m.*, v.PlateNumber, v.Make, v.Model, v.[Status] AS VehStatus " &
                        "FROM Maintenance AS m INNER JOIN Vehicles AS v ON m.VehicleID = v.VehicleID WHERE m.MaintenanceID = ?"
                Using cmd As New OleDbCommand(q, conn)
                    cmd.Parameters.AddWithValue("?", id)
                    Using r = cmd.ExecuteReader()
                        If r.Read() Then
                            Dim d = m.Details
                            Dim veh = "VH-" & Convert.ToInt32(r("VehicleID")).ToString("0000")
                            Dim at = Convert.ToDateTime(r("ScheduledAt"))
                            d.lblSubtitle.Text = idText & " · " & r("Make").ToString() & " " & r("Model").ToString() & " / " & veh & " · " & r("PlateNumber").ToString()
                            d.lblStatusBadge.Text = r("Status").ToString()
                            d.lblVehStatus.Text = r("VehStatus").ToString()
                            d.lblServiceDate.Text = at.ToString("dd MMM yyyy / HH:mm")
                            d.lblProvider.Text = r("Provider").ToString()
                            d.lblOdoPriority.Text = r("Odometer").ToString() & " km / " & r("Priority").ToString()
                            d.lblEstCost.Text = "$" & Convert.ToDecimal(r("EstimatedCost")).ToString("N0")
                            d.lblReportedIssue.Text = r("Description").ToString()
                            d.dgvLog.Rows.Clear()
                            d.dgvLog.Rows.Add(at.ToString("dd MMM · HH:mm"), "Service scheduled · " & r("ServiceType").ToString(),
                                              r("Provider").ToString(), "$" & Convert.ToDecimal(r("EstimatedCost")).ToString("N0") & " est.", "Logged")
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("ShowMaintenanceRecord Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' bottom-left links
    Private Sub lblSettings_Click(sender As Object, e As EventArgs) Handles lblSettings.Click
        Navigate(btnMenuSettings)
    End Sub

    Private Sub lblLogout_Click(sender As Object, e As EventArgs) Handles lblLogout.Click
        If MessageBox.Show("Do you want to log out?", "Logout", MessageBoxButtons.YesNo,
                           MessageBoxIcon.Question) = DialogResult.Yes Then
            Me.Close()
        End If
    End Sub

    ' ---------------------------------------------------------------
    '  Helpers
    ' ---------------------------------------------------------------
    Private Shared Function Initials(name As String) As String
        Dim parts = name.Split({" "c}, StringSplitOptions.RemoveEmptyEntries)
        If parts.Length = 0 Then Return "?"
        If parts.Length = 1 Then Return parts(0).Substring(0, 1).ToUpper()
        Return (parts(0).Substring(0, 1) & parts(parts.Length - 1).Substring(0, 1)).ToUpper()
    End Function

    Private Shared Sub MakeCircle(c As Control)
        Using gp As New GraphicsPath()
            gp.AddEllipse(0, 0, c.Width, c.Height)
            c.Region = New Region(gp)
        End Using
    End Sub

    Private Shared Sub MakeRounded(c As Control, radius As Integer)
        Dim d As Integer = radius * 2
        Using gp As New GraphicsPath()
            gp.AddArc(0, 0, d, d, 180, 90)
            gp.AddArc(c.Width - d, 0, d, d, 270, 90)
            gp.AddArc(c.Width - d, c.Height - d, d, d, 0, 90)
            gp.AddArc(0, c.Height - d, d, d, 90, 90)
            gp.CloseFigure()
            c.Region = New Region(gp)
        End Using
    End Sub

End Class