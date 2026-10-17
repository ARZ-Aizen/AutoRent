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
    Private _editingUserId As Integer = 0
    Private _selectedUserId As Integer = 0
    Private _userBusy As Boolean = False
    Private _auditReady As Boolean = False
    Private _auditBusy As Boolean = False
    Private _auditPage As Integer = 1
    Private _actorId As Integer = -1
    Private _pRow As Integer = -1
    Private _pCol As Integer = 0
    Private _pPerm As String = ""
    Private _pRole As String = ""
    Private _pBefore As String = ""
    Private _pAfter As String = ""
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
            EnsureUserColumns()
            Dim ua As New ucUserAccounts()
            LoadUserLists(ua.Editor)
            LoadUserFilters(ua)
            LoadUsers(ua)
            AddHandler ua.FilterChanged, Sub() If Not _userBusy Then LoadUsers(ua)
            AddHandler ua.UserSelected, Sub(i) ShowSelectedUser(ua, i)
            AddHandler ua.CreateUserClicked, Sub() _editingUserId = 0
            AddHandler ua.EditUserRequested, Sub(i) LoadUserIntoForm(ua, i)
            AddHandler ua.Editor.SaveClicked,
                Sub()
                    If SaveUser(ua.Editor) Then
                        LoadUsers(ua)
                        ua.ShowList()
                    End If
                End Sub
            AddHandler ua.DeactivateClicked,
                Sub()
                    If _selectedUserId = 0 Then Return
                    ToggleUserActive(_selectedUserId)
                    LoadUsers(ua)
                End Sub
            AddHandler ua.SecureResetClicked,
                Sub()
                    If _selectedUserId = 0 Then Return
                    MessageBox.Show("A single-use, expiring reset link will be issued. This action is audited.", "Secure password reset",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information)
                End Sub
            Return ua
        ElseIf btn Is btnMenuAccess Then
            Return BuildAccessPanel()
        ElseIf btn Is btnMenuAudit Then
            Return BuildAuditPanel()
        ElseIf btn Is btnMenuSettings Then
            Return BuildSettingsPanel()
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
    '  User form
    ' ---------------------------------------------------------------
    Private Sub EnsureUserColumns()
        Dim cols() As String = {"FullName TEXT(100)", "Username TEXT(50)", "Email TEXT(100)", "Phone TEXT(30)",
                                "Branch TEXT(60)", "RoleName TEXT(40)", "AccountStatus TEXT(30)", "AccessMethod TEXT(30)",
                                "RequireMfa YESNO", "ChangeReason TEXT(255)", "UpdatedBy TEXT(100)", "UpdatedAt DATETIME", "LastLogin DATETIME"}
        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                For Each colDef As String In cols
                    Try
                        Using cmd As New OleDbCommand("ALTER TABLE Users ADD COLUMN " & colDef, conn)
                            cmd.ExecuteNonQuery()
                        End Using
                    Catch
                    End Try
                Next
            End Using
        Catch ex As Exception
            MessageBox.Show("EnsureUserColumns Error: " & ex.Message)
        End Try
    End Sub

    Private Sub LoadUserLists(uf As ucUserForm)
        uf.cboRole.Items.AddRange(New Object() {"System Administrator", "Business Owner", "Branch Manager", "Front-Desk Staff", "Rental Clerk", "Dispatch Officer"})
        uf.cboAccountStatus.Items.AddRange(New Object() {"Invitation pending", "Active", "Suspended", "Disabled"})
    End Sub

    Private Sub LoadUserFilters(ua As ucUserAccounts)
        _userBusy = True
        ua.cboRole.Items.Add("All roles")
        ua.cboRole.Items.AddRange(New Object() {"System Administrator", "Business Owner", "Branch Manager", "Front-Desk Staff", "Rental Clerk", "Dispatch Officer"})
        ua.cboBranch.Items.Add("All branches")
        ua.Editor.cboBranch.Items.Add("HQ")
        ua.cboStatus.Items.AddRange(New Object() {"All statuses", "Active", "Invitation pending", "Suspended", "Deactivated"})
        ua.Editor.cboAccountStatus.Items.Add("Deactivated")
        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                Using cmd As New OleDbCommand("SELECT DISTINCT Branch FROM Users WHERE Branch IS NOT NULL", conn)
                    Using r = cmd.ExecuteReader()
                        While r.Read()
                            Dim b As String = r(0).ToString()
                            If b <> "" Then
                                ua.cboBranch.Items.Add(b)
                                If b <> "HQ" Then ua.Editor.cboBranch.Items.Add(b)
                            End If
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("LoadUserFilters Error: " & ex.Message)
        End Try
        ua.cboRole.SelectedIndex = 0 : ua.cboBranch.SelectedIndex = 0 : ua.cboStatus.SelectedIndex = 0
        _userBusy = False
    End Sub

    Private Sub LoadUsers(ua As ucUserAccounts)
        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                Dim total = CountP(conn, "SELECT COUNT(*) FROM Users")
                Dim act = CountP(conn, "SELECT COUNT(*) FROM Users WHERE AccountStatus = 'Active'")
                Dim pend = CountP(conn, "SELECT COUNT(*) FROM Users WHERE AccountStatus LIKE 'Invit*'")
                Dim deact = CountP(conn, "SELECT COUNT(*) FROM Users WHERE AccountStatus = 'Deactivated'")
                ua.lblSummary.Text = total & " staff accounts · " & act & " active · " & pend & " invitation pending · " & deact & " deactivated · passwords are never displayed"

                Dim ps As New List(Of Object)
                Dim sql As String = "SELECT UserID, FullName, Username, RoleName, Branch, AccountStatus, LastLogin FROM Users WHERE 1=1"
                Dim term As String = ua.txtSearch.Text.Trim()
                If term <> "" Then
                    sql &= " AND (FullName LIKE ? OR Username LIKE ?)"
                    ps.Add("%" & term & "%") : ps.Add("%" & term & "%")
                End If
                If ua.cboRole.SelectedIndex > 0 Then sql &= " AND RoleName = ?" : ps.Add(ua.cboRole.Text)
                If ua.cboBranch.SelectedIndex > 0 Then sql &= " AND Branch = ?" : ps.Add(ua.cboBranch.Text)
                If ua.cboStatus.SelectedIndex > 0 Then sql &= " AND AccountStatus = ?" : ps.Add(ua.cboStatus.Text)
                sql &= " ORDER BY UserID"

                ua.dgvUsers.Rows.Clear()
                Using cmd = MakeCmd(conn, sql, ps)
                    Using r = cmd.ExecuteReader()
                        While r.Read()
                            Dim id As Integer = Convert.ToInt32(r.GetValue(0))
                            Dim status As String = Txt(r, 5)
                            Dim act2 As String = "Edit · Activity"
                            If status = "Deactivated" Then act2 = "Reactivate · Activity"
                            If status.StartsWith("Invit") Then act2 = "Edit · Resend invite"
                            Dim idx As Integer = ua.dgvUsers.Rows.Add("USR-" & id.ToString("0000"), Txt(r, 1), Txt(r, 2), Txt(r, 3), Txt(r, 4), status,
                                If(r.IsDBNull(6), "—", Convert.ToDateTime(r.GetValue(6)).ToString("dd MMM · HH:mm")), act2)
                            ua.dgvUsers.Rows(idx).Tag = id
                        End While
                    End Using
                End Using
                ua.lblShowing.Text = "Showing " & ua.dgvUsers.Rows.Count & " of " & total & " accounts · access restricted to authorized administrators"
                ua.lblPage.Text = "Page 1 of 1 · All records shown"
                ua.lblLifecycleTitle.Text = "Secure account lifecycle"
                ua.lblLifecycleNote.Text = "Deactivation revokes access but preserves attribution. Password resets use a single-use expiring link, never a stored password. Role changes require server-side authorization and an audit entry."
            End Using
        Catch ex As Exception
            MessageBox.Show("LoadUsers Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function UserIdOfRow(ua As ucUserAccounts, rowIndex As Integer) As Integer
        If rowIndex < 0 OrElse rowIndex >= ua.dgvUsers.Rows.Count Then Return 0
        Dim t = ua.dgvUsers.Rows(rowIndex).Tag
        Return If(t Is Nothing, 0, Convert.ToInt32(t))
    End Function

    Private Sub ShowSelectedUser(ua As ucUserAccounts, rowIndex As Integer)
        Dim id As Integer = UserIdOfRow(ua, rowIndex)
        _selectedUserId = id
        If id = 0 Then Return
        Dim row = ua.dgvUsers.Rows(rowIndex)
        ua.lblSelTitle.Text = "Selected account · " & Convert.ToString(row.Cells(1).Value)
        ua.lblSelSub.Text = Convert.ToString(row.Cells(0).Value) & " · " & Convert.ToString(row.Cells(5).Value)
        ua.lblSelRole.Text = Convert.ToString(row.Cells(3).Value)
        ua.lblSelLastActivity.Text = Convert.ToString(row.Cells(6).Value)
        ua.lblSelAuthority.Text = Convert.ToString(row.Cells(4).Value) & " branch only"
        ua.lblSelInvitation.Text = "—"
        ua.btnDeactivate.Text = If(Convert.ToString(row.Cells(5).Value) = "Deactivated", "Reactivate", "Deactivate")
        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                Using cmd As New OleDbCommand("SELECT AccessMethod, UpdatedAt FROM Users WHERE UserID = ?", conn)
                    cmd.Parameters.Add("?", OleDbType.Integer).Value = id
                    Using r = cmd.ExecuteReader()
                        If r.Read() AndAlso Not r.IsDBNull(1) Then
                            ua.lblSelInvitation.Text = Txt(r, 0) & " · " & Convert.ToDateTime(r.GetValue(1)).ToString("dd MMM · HH:mm")
                        End If
                    End Using
                End Using
            End Using
        Catch
        End Try
    End Sub

    Private Sub ToggleUserActive(id As Integer)
        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                Using cmd As New OleDbCommand("UPDATE Users SET AccountStatus = IIf(AccountStatus = 'Deactivated', 'Active', 'Deactivated'), UpdatedBy = ?, UpdatedAt = ? WHERE UserID = ?", conn)
                    cmd.Parameters.Add("?", OleDbType.VarWChar).Value = _fullName
                    cmd.Parameters.Add("?", OleDbType.Date).Value = Date.Now
                    cmd.Parameters.Add("?", OleDbType.Integer).Value = id
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("ToggleUserActive Error: " & ex.Message)
        End Try
    End Sub

    Private Sub LoadUserIntoForm(ua As ucUserAccounts, rowIndex As Integer)
        Dim id As Integer = UserIdOfRow(ua, rowIndex)
        If id = 0 Then Return
        _editingUserId = id
        Dim f As ucUserForm = ua.Editor
        f.ClearForm()
        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                Using cmd As New OleDbCommand("SELECT FullName, Username, Email, Phone, Branch, RoleName, AccountStatus, AccessMethod, RequireMfa, UpdatedBy, UpdatedAt, LastLogin FROM Users WHERE UserID = ?", conn)
                    cmd.Parameters.Add("?", OleDbType.Integer).Value = id
                    Using r = cmd.ExecuteReader()
                        If Not r.Read() Then Return
                        f.txtFullName.Text = Txt(r, 0) : f.txtUsername.Text = Txt(r, 1)
                        f.txtEmail.Text = Txt(r, 2) : f.txtPhone.Text = Txt(r, 3)
                        f.cboBranch.SelectedIndex = f.cboBranch.FindStringExact(Txt(r, 4))
                        f.cboRole.SelectedIndex = f.cboRole.FindStringExact(Txt(r, 5))
                        f.cboAccountStatus.SelectedIndex = f.cboAccountStatus.FindStringExact(Txt(r, 6))
                        If Txt(r, 7) <> "" Then f.SelectMethod(Txt(r, 7))
                        f.chkRequireMfa.Checked = Not r.IsDBNull(8) AndAlso Convert.ToBoolean(r.GetValue(8))
                        f.lblUpdatedByValue.Text = Txt(r, 9)
                        f.lblUpdatedAtValue.Text = If(r.IsDBNull(10), "—", Convert.ToDateTime(r.GetValue(10)).ToString("dd MMM · HH:mm"))
                        f.lblLastLoginValue.Text = If(r.IsDBNull(11), "—", Convert.ToDateTime(r.GetValue(11)).ToString("dd MMM · HH:mm"))
                        f.lblSubtitle.Text = "USR-" & id.ToString("0000") & " · " & Txt(r, 0)
                    End Using
                End Using
            End Using
            f.txtUserId.Text = "USR-" & id.ToString("0000")
        Catch ex As Exception
            MessageBox.Show("LoadUserIntoForm Error: " & ex.Message)
        End Try
    End Sub

    Private Function SaveUser(uf As ucUserForm) As Boolean
        Dim missing As New List(Of String)
        If uf.txtFullName.Text.Trim() = "" Then missing.Add("Full name")
        If uf.txtUsername.Text.Trim() = "" Then missing.Add("Username")
        If uf.cboBranch.Text.Trim() = "" Then missing.Add("Branch")
        If uf.cboRole.SelectedIndex < 0 Then missing.Add("Role")
        If uf.cboAccountStatus.SelectedIndex < 0 Then missing.Add("Account status")
        If uf.txtReason.Text.Trim() = "" Then missing.Add("Change reason")
        If missing.Count > 0 Then
            MessageBox.Show("Please complete: " & String.Join(", ", missing), "Missing information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If
        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                Dim sql As String
                If _editingUserId > 0 Then
                    sql = "UPDATE Users SET FullName=?, Username=?, Email=?, Phone=?, Branch=?, RoleName=?, AccountStatus=?, AccessMethod=?, RequireMfa=?, ChangeReason=?, UpdatedBy=?, UpdatedAt=? WHERE UserID=?"
                Else
                    sql = "INSERT INTO Users (FullName, Username, Email, Phone, Branch, RoleName, AccountStatus, AccessMethod, RequireMfa, ChangeReason, UpdatedBy, UpdatedAt) VALUES (?,?,?,?,?,?,?,?,?,?,?,?)"
                End If
                Using cmd As New OleDbCommand(sql, conn)
                    For Each v As String In New String() {uf.txtFullName.Text.Trim(), uf.txtUsername.Text.Trim(), uf.txtEmail.Text.Trim(),
                                                          uf.txtPhone.Text.Trim(), uf.cboBranch.Text, uf.cboRole.Text, uf.cboAccountStatus.Text, uf.AccessMethod}
                        cmd.Parameters.Add("?", OleDbType.VarWChar).Value = v
                    Next
                    cmd.Parameters.Add("?", OleDbType.Boolean).Value = uf.chkRequireMfa.Checked
                    cmd.Parameters.Add("?", OleDbType.VarWChar).Value = uf.txtReason.Text.Trim()
                    cmd.Parameters.Add("?", OleDbType.VarWChar).Value = _fullName
                    cmd.Parameters.Add("?", OleDbType.Date).Value = Date.Now
                    If _editingUserId > 0 Then cmd.Parameters.Add("?", OleDbType.Integer).Value = _editingUserId
                    cmd.ExecuteNonQuery()
                End Using
            End Using
            WriteAudit(If(_editingUserId > 0, "User modified", "User created"), uf.txtUsername.Text.Trim(), "Success", "", uf.cboRole.Text, uf.txtReason.Text.Trim())
            uf.ShowBanner("Saved", "User record saved " & Date.Now.ToString("dd MMM yyyy HH:mm"))
            _editingUserId = 0
            Return True
        Catch ex As Exception
            MessageBox.Show("SaveUser Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        Return False
    End Function


    ' ---------------------------------------------------------------
    '  Audit trail (AuditEvents) + shared helpers
    ' ---------------------------------------------------------------
    Private Sub EnsureAuditTable()
        If _auditReady Then Return
        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                Try
                    Using cmd As New OleDbCommand(
                        "CREATE TABLE AuditEvents (EventID COUNTER PRIMARY KEY, EventTime DATETIME, ActorID LONG, Username TEXT(80), " &
                        "RoleName TEXT(60), ActionName TEXT(80), AffectedRecord TEXT(80), ResultText TEXT(20), SourceText TEXT(60), " &
                        "RefCode TEXT(30), ReasonText TEXT(255), BeforeValue TEXT(255), AfterValue TEXT(255), ContextText TEXT(255))", conn)
                        cmd.ExecuteNonQuery()
                    End Using
                Catch ex2 As Exception
                    If ex2.Message.IndexOf("already exists", StringComparison.OrdinalIgnoreCase) < 0 Then
                        MessageBox.Show("Could not create AuditEvents: " & ex2.Message)
                    End If
                End Try
            End Using
            _auditReady = True
        Catch ex As Exception
            MessageBox.Show("EnsureAuditTable Error: " & ex.Message)
        End Try
    End Sub

    Private Function CurrentUserId() As Integer
        If _actorId >= 0 Then Return _actorId
        _actorId = 0
        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                Using cmd As New OleDbCommand("SELECT UserID FROM Users WHERE FullName = ?", conn)
                    cmd.Parameters.Add("?", OleDbType.VarWChar).Value = _fullName
                    Dim o = cmd.ExecuteScalar()
                    If o IsNot Nothing AndAlso Not IsDBNull(o) Then _actorId = Convert.ToInt32(o)
                End Using
            End Using
        Catch
        End Try
        Return _actorId
    End Function

    Private Sub WriteAudit(action As String, record As String, result As String, before As String, after As String, reason As String)
        EnsureAuditTable()
        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                Using cmd As New OleDbCommand(
                    "INSERT INTO AuditEvents (EventTime, ActorID, Username, RoleName, ActionName, AffectedRecord, ResultText, SourceText, RefCode, ReasonText, BeforeValue, AfterValue, ContextText) " &
                    "VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?)", conn)
                    cmd.Parameters.Add("?", OleDbType.Date).Value = Date.Now
                    cmd.Parameters.Add("?", OleDbType.Integer).Value = CurrentUserId()
                    For Each v As String In New String() {_fullName, _role, action, record, result, "HQ · " & Environment.MachineName,
                                                          "REQ-" & Date.Now.ToString("HHmmss"), reason, before, after,
                                                          "Server-side decision recorded for " & _role}
                        cmd.Parameters.Add("?", OleDbType.VarWChar).Value = If(v, "")
                    Next
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("WriteAudit Error: " & ex.Message)
        End Try
    End Sub

    ' ---------------------------------------------------------------
    '  Access permissions matrix
    ' ---------------------------------------------------------------
    Private Shared ReadOnly PermRoles() As String = {"System Administrator", "Business Owner / Branch Manager", "Front-Desk Staff", "Rental Clerk", "Dispatch Officer"}
    Private Shared ReadOnly PermNames() As String = {"View Dashboard", "View Fleet Inventory", "Add/Edit Vehicles", "Retire Vehicles", "Manage Maintenance",
                                                     "Configure Rates", "View Revenue", "Export Reports", "Manage Users", "Change Permissions", "View Audit Logs"}
    Private Shared ReadOnly PermSeed() As String = {"RRRRR", "RRRRR", "AAXXX", "AAXXX", "AAXXA", "AAXXX", "RRXXX", "AAXXX", "AXXXX", "AXXXX", "RXXXX"}

    Private Sub EnsureRolePermissions()
        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                Try
                    Using cmd As New OleDbCommand("CREATE TABLE RolePermissions (PermKey TEXT(60), RoleName TEXT(60), Access TEXT(20))", conn)
                        cmd.ExecuteNonQuery()
                    End Using
                Catch
                End Try
                If CountP(conn, "SELECT COUNT(*) FROM RolePermissions") > 0 Then Return
                For i As Integer = 0 To PermNames.Length - 1
                    For j As Integer = 0 To PermRoles.Length - 1
                        Dim a As String = If(PermSeed(i)(j) = "A", "Allowed", If(PermSeed(i)(j) = "R", "Read-only", "Restricted"))
                        Using cmd As New OleDbCommand("INSERT INTO RolePermissions (PermKey, RoleName, Access) VALUES (?,?,?)", conn)
                            cmd.Parameters.Add("?", OleDbType.VarWChar).Value = PermNames(i)
                            cmd.Parameters.Add("?", OleDbType.VarWChar).Value = PermRoles(j)
                            cmd.Parameters.Add("?", OleDbType.VarWChar).Value = a
                            cmd.ExecuteNonQuery()
                        End Using
                    Next
                Next
            End Using
        Catch ex As Exception
            MessageBox.Show("EnsureRolePermissions Error: " & ex.Message)
        End Try
    End Sub

    Private Function BuildAccessPanel() As UserControl
        EnsureRolePermissions()
        EnsureAuditTable()
        _pRow = -1
        Dim pm As New ucPermissionMatrix()
        pm.SetRoles(PermRoles)
        pm.lblSubtitle.Text = "Role policy · changes restricted to the System Administrator"
        pm.lblGuardTitle.Text = "Protected authority · anti-lockout guardrail"
        pm.lblGuardNote.Text = "Every protected action is validated on the server. Manage Users / Change Permissions cannot be removed from the System Administrator."
        pm.lblMatrixTitle.Text = "Role permission matrix"
        pm.lblMatrixSub.Text = "Allowed = action access · Read-only = view without changes · Restricted = no access · click a cell to draft a change"
        pm.lblAuditTitle.Text = "Permission changes are auditable"
        pm.lblAuditNote.Text = "Before / after states, actor and reason are recorded in Audit Logs. Nothing changes until it is confirmed."
        LoadPermissionMatrix(pm)
        AddHandler pm.CellClicked, Sub(r, c) DraftPermission(pm, r, c)
        AddHandler pm.CancelChangeClicked, Sub() CancelPermissionDraft(pm)
        AddHandler pm.ConfirmClicked, Sub() ConfirmPermission(pm)
        AddHandler pm.ReviewClicked,
            Sub()
                If _pRow < 0 Then
                    MessageBox.Show("No pending permission change. Click a cell in the matrix to draft one.", "Review permission changes",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    pm.FocusDraft()
                End If
            End Sub
        Return pm
    End Function

    Private Sub LoadPermissionMatrix(pm As ucPermissionMatrix)
        Dim map As New Dictionary(Of String, String)
        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                Using cmd As New OleDbCommand("SELECT PermKey, RoleName, Access FROM RolePermissions", conn)
                    Using r = cmd.ExecuteReader()
                        While r.Read()
                            map(r(0).ToString() & "|" & r(1).ToString()) = r(2).ToString()
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("LoadPermissionMatrix Error: " & ex.Message)
        End Try
        pm.dgvMatrix.Rows.Clear()
        For Each perm As String In PermNames
            Dim cells(PermRoles.Length) As Object
            cells(0) = perm
            For j As Integer = 0 To PermRoles.Length - 1
                Dim k As String = perm & "|" & PermRoles(j)
                cells(j + 1) = If(map.ContainsKey(k), map(k), "Restricted")
            Next
            pm.dgvMatrix.Rows.Add(cells)
        Next
        pm.ClearDraft()
        _pRow = -1
    End Sub

    Private Sub CancelPermissionDraft(pm As ucPermissionMatrix)
        If _pRow >= 0 AndAlso _pRow < pm.dgvMatrix.Rows.Count Then
            Dim cell = pm.dgvMatrix.Rows(_pRow).Cells(_pCol)
            cell.Value = _pBefore
            cell.Tag = Nothing
            pm.dgvMatrix.InvalidateCell(cell)
        End If
        _pRow = -1
        pm.ClearDraft()
    End Sub

    Private Sub DraftPermission(pm As ucPermissionMatrix, r As Integer, c As Integer)
        If _pRow >= 0 Then CancelPermissionDraft(pm)
        Dim perm As String = Convert.ToString(pm.dgvMatrix.Rows(r).Cells(0).Value)
        Dim role As String = pm.dgvMatrix.Columns(c).HeaderText
        Dim before As String = Convert.ToString(pm.dgvMatrix.Rows(r).Cells(c).Value)
        Dim after As String = If(before = "Allowed", "Read-only", If(before = "Read-only", "Restricted", "Allowed"))
        If role = "System Administrator" AndAlso (perm = "Manage Users" OrElse perm = "Change Permissions") AndAlso after <> "Allowed" Then
            MessageBox.Show("Anti-lockout guardrail: the System Administrator must keep " & perm & " access.", "Change blocked",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            WriteAudit("Permission change blocked", perm, "Denied", before, after, "Anti-lockout guardrail")
            Return
        End If
        _pRow = r : _pCol = c : _pPerm = perm : _pRole = role : _pBefore = before : _pAfter = after
        Dim cell = pm.dgvMatrix.Rows(r).Cells(c)
        cell.Value = after
        cell.Tag = "pending"
        pm.dgvMatrix.InvalidateCell(cell)
        pm.ShowDraft("Draft: " & perm & " for " & role, before, after)
    End Sub

    Private Sub ConfirmPermission(pm As ucPermissionMatrix)
        If _pRow < 0 Then
            MessageBox.Show("Click a matrix cell to draft a change first.", "Nothing to confirm", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        If pm.txtReason.Text.Trim() = "" Then
            MessageBox.Show("Enter an audit reason.", "Missing information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        If Not pm.chkReviewed.Checked Then
            MessageBox.Show("Tick the review confirmation first.", "Missing confirmation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                Using cmd As New OleDbCommand("UPDATE RolePermissions SET Access = ? WHERE PermKey = ? AND RoleName = ?", conn)
                    cmd.Parameters.Add("?", OleDbType.VarWChar).Value = _pAfter
                    cmd.Parameters.Add("?", OleDbType.VarWChar).Value = _pPerm
                    cmd.Parameters.Add("?", OleDbType.VarWChar).Value = _pRole
                    cmd.ExecuteNonQuery()
                End Using
            End Using
            WriteAudit("Permission updated", _pPerm & " / " & _pRole, "Success", _pBefore, _pAfter, pm.txtReason.Text.Trim())
            LoadPermissionMatrix(pm)
        Catch ex As Exception
            MessageBox.Show("ConfirmPermission Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ---------------------------------------------------------------
    '  Audit logs viewer
    ' ---------------------------------------------------------------
    Private Function BuildAuditPanel() As UserControl
        EnsureAuditTable()
        _auditPage = 1
        _auditBusy = True
        Dim av As New ucAuditViewer()
        av.lblSubtitle.Text = "Access-controlled, immutable administrative event register · read-only by design"
        av.cboRange.Items.AddRange(New Object() {"Today", "Last 7 days", "Last 30 days", "All time"})
        av.cboRange.SelectedIndex = 3
        av.cboType.Items.Add("All actions")
        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                Using cmd As New OleDbCommand("SELECT DISTINCT ActionName FROM AuditEvents", conn)
                    Using r = cmd.ExecuteReader()
                        While r.Read()
                            av.cboType.Items.Add(r(0).ToString())
                        End While
                    End Using
                End Using
            End Using
        Catch
        End Try
        av.cboType.SelectedIndex = 0
        av.cboResult.Items.AddRange(New Object() {"All results", "Success", "Denied"})
        av.cboResult.SelectedIndex = 0
        av.lblImmutableTitle.Text = "Immutable history"
        av.lblImmutableNote.Text = "No modification or deletion is offered. Sensitive metadata is visible only to authorized roles."
        _auditBusy = False
        LoadAudit(av)
        AddHandler av.FilterChanged, Sub()
                                         If _auditBusy Then Return
                                         _auditPage = 1
                                         LoadAudit(av)
                                     End Sub
        AddHandler av.PageRequested, Sub(p)
                                         _auditPage = p
                                         LoadAudit(av)
                                     End Sub
        AddHandler av.EventSelected, Sub(i) ShowAuditDetail(av, i)
        Return av
    End Function

    Private Sub LoadAudit(av As ucAuditViewer)
        Const PageSize As Integer = 13
        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                Dim ps As New List(Of Object)
                Dim where As String = " WHERE 1=1"
                Select Case av.cboRange.SelectedIndex
                    Case 0 : where &= " AND EventTime >= ?" : ps.Add(Date.Today)
                    Case 1 : where &= " AND EventTime >= ?" : ps.Add(Date.Today.AddDays(-6))
                    Case 2 : where &= " AND EventTime >= ?" : ps.Add(Date.Today.AddDays(-29))
                End Select
                Dim term As String = av.txtSearch.Text.Trim()
                If term <> "" Then
                    where &= " AND (Username LIKE ? OR ActionName LIKE ? OR AffectedRecord LIKE ?)"
                    ps.Add("%" & term & "%") : ps.Add("%" & term & "%") : ps.Add("%" & term & "%")
                End If
                If av.cboType.SelectedIndex > 0 Then where &= " AND ActionName = ?" : ps.Add(av.cboType.Text)
                If av.cboResult.SelectedIndex > 0 Then where &= " AND ResultText = ?" : ps.Add(av.cboResult.Text)

                Dim total As Integer = CountP(conn, "SELECT COUNT(*) FROM AuditEvents" & where, ps.ToArray())
                Dim pages As Integer = Math.Max(1, CInt(Math.Ceiling(total / PageSize)))
                _auditPage = Math.Max(1, Math.Min(_auditPage, pages))

                av.dgvEvents.Rows.Clear()
                Dim skip As Integer = (_auditPage - 1) * PageSize
                Dim n As Integer = 0
                Using cmd = MakeCmd(conn, "SELECT EventID, EventTime, ActorID, Username, RoleName, ActionName, AffectedRecord, ResultText FROM AuditEvents" &
                                    where & " ORDER BY EventID DESC", ps)
                    Using r = cmd.ExecuteReader()
                        While r.Read()
                            n += 1
                            If n <= skip Then Continue While
                            If av.dgvEvents.Rows.Count >= PageSize Then Exit While
                            Dim id As Integer = Convert.ToInt32(r.GetValue(0))
                            Dim actor As Integer = If(r.IsDBNull(2), 0, Convert.ToInt32(r.GetValue(2)))
                            Dim idx As Integer = av.dgvEvents.Rows.Add("EV-" & id.ToString("0000"),
                                If(r.IsDBNull(1), "", Convert.ToDateTime(r.GetValue(1)).ToString("dd MMM HH:mm")),
                                If(actor > 0, "USR-" & actor.ToString("0000"), "—"), Txt(r, 3), Txt(r, 4), Txt(r, 5), Txt(r, 6), Txt(r, 7))
                            av.dgvEvents.Rows(idx).Tag = id
                        End While
                    End Using
                End Using
                av.lblRegisterSub.Text = "Timestamps in local time · newest first"
                av.lblCount.Text = av.dgvEvents.Rows.Count & " of " & total.ToString("N0") & " events"
                av.SetPaging(_auditPage, pages)
                If av.dgvEvents.Rows.Count = 0 Then
                    av.lblEvTitle.Text = "" : av.lblEvSub.Text = "No event selected"
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("LoadAudit Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ShowAuditDetail(av As ucAuditViewer, rowIndex As Integer)
        If rowIndex < 0 OrElse rowIndex >= av.dgvEvents.Rows.Count Then Return
        Dim t = av.dgvEvents.Rows(rowIndex).Tag
        If t Is Nothing Then Return
        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                Using cmd As New OleDbCommand("SELECT EventTime, Username, RoleName, ActionName, AffectedRecord, ResultText, SourceText, RefCode, ReasonText, BeforeValue, AfterValue, ContextText FROM AuditEvents WHERE EventID = ?", conn)
                    cmd.Parameters.Add("?", OleDbType.Integer).Value = Convert.ToInt32(t)
                    Using r = cmd.ExecuteReader()
                        If Not r.Read() Then Return
                        Dim id As String = "EV-" & Convert.ToInt32(t).ToString("0000")
                        av.lblEvTitle.Text = id & " · " & Txt(r, 3)
                        av.lblEvSub.Text = "Selected event metadata"
                        av.lblEvResult.Text = Txt(r, 5)
                        Dim denied As Boolean = (Txt(r, 5) = "Denied")
                        av.lblEvResult.ForeColor = If(denied, Color.FromArgb(122, 0, 16), Color.FromArgb(46, 110, 60))
                        av.lblEvResult.BackColor = If(denied, Color.FromArgb(250, 228, 230), Color.FromArgb(232, 240, 233))
                        av.lblActor.Text = Txt(r, 1)
                        av.lblRole.Text = Txt(r, 2)
                        av.lblRecord.Text = Txt(r, 4)
                        av.lblOccurred.Text = If(r.IsDBNull(0), "", Convert.ToDateTime(r.GetValue(0)).ToString("dd MMM · HH:mm:ss"))
                        av.lblSource.Text = Txt(r, 6)
                        av.lblReference.Text = Txt(r, 7)
                        Dim b As String = Txt(r, 9), a As String = Txt(r, 10)
                        av.lblChange.Text = If(b = "" AndAlso a = "", "—", If(b = "", "", b & " → ") & a)
                        av.lblReason.Text = If(Txt(r, 8) = "", "", "Reason: " & Txt(r, 8))
                        av.lblCtxTitle.Text = "Authorization context"
                        av.lblCtxNote.Text = Txt(r, 11)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("ShowAuditDetail Error: " & ex.Message)
        End Try
    End Sub

    ' ---------------------------------------------------------------
    '  System settings
    ' ---------------------------------------------------------------
    Private Sub EnsureSettingsTable()
        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                Try
                    Using cmd As New OleDbCommand("CREATE TABLE SystemSettings (SettingKey TEXT(60) PRIMARY KEY, SettingValue TEXT(255))", conn)
                        cmd.ExecuteNonQuery()
                    End Using
                Catch
                End Try
                Dim defaults As New Dictionary(Of String, String) From {
                    {"ConfigVersion", "1"}, {"Currency", "USD"}, {"BillingRule", "24-hour blocks · daily rounding"}, {"GracePeriod", "60"},
                    {"Timezone", "UTC"}, {"ContractTemplate", "Executive rental agreement · v4"},
                    {"ReportFormat", "A4 landscape · USD · ISO dates"}, {"Watermark", "1"}, {"NotifMaintenance", "1"},
                    {"NotifOverdue", "1"}, {"NotifExpiry", "1"}, {"NotifDigest", "1"}, {"NotifSensitive", "1"}, {"QuietHours", "20:00-08:00"}}
                For Each kv In defaults
                    Try
                        Using cmd As New OleDbCommand("INSERT INTO SystemSettings (SettingKey, SettingValue) VALUES (?,?)", conn)
                            cmd.Parameters.Add("?", OleDbType.VarWChar).Value = kv.Key
                            cmd.Parameters.Add("?", OleDbType.VarWChar).Value = kv.Value
                            cmd.ExecuteNonQuery()
                        End Using
                    Catch
                        ' already present
                    End Try
                Next
            End Using
        Catch ex As Exception
            MessageBox.Show("EnsureSettingsTable Error: " & ex.Message)
        End Try
    End Sub

    Private Function BuildSettingsPanel() As UserControl
        EnsureSettingsTable()
        EnsureAuditTable()
        Dim sp As New ucSettingsPage()
        sp.lblSubtitle.Text = "Agency and branch configuration · authorized changes are versioned and attributed"
        sp.lblAgencySub.Text = "Branch configuration"
        sp.lblBillingNote.Text = "The branch timezone determines billing dates, reports and all effective-time boundaries. Grace period is in minutes."
        sp.lblWarnTitle.Text = "New eligible contracts only"
        sp.lblWarnNote.Text = "Billing rules cannot silently alter finalized contracts. Review the effect on late-return calculations before saving."
        sp.lblHistNote.Text = "Saving requires the settings authority. Before / after values are retained in Audit Logs."
        LoadSettings(sp)
        AddHandler sp.SaveClicked, Sub() SaveSettings(sp)
        AddHandler sp.DiscardClicked, Sub() LoadSettings(sp)
        Return sp
    End Function

    Private Sub LoadSettings(sp As ucSettingsPage)
        Dim d As New Dictionary(Of String, String)
        Try
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                Using cmd As New OleDbCommand("SELECT SettingKey, SettingValue FROM SystemSettings", conn)
                    Using r = cmd.ExecuteReader()
                        While r.Read()
                            d(r(0).ToString()) = If(IsDBNull(r(1)), "", r(1).ToString())
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("LoadSettings Error: " & ex.Message)
        End Try
        sp.SetValues(d)
        sp.lblVersion.Text = "CFG-v" & If(d.ContainsKey("ConfigVersion"), d("ConfigVersion"), "1")
        sp.lblSavedBy.Text = If(d.ContainsKey("ConfigSavedBy"), d("ConfigSavedBy"), "—")
        sp.lblSavedAt.Text = If(d.ContainsKey("ConfigSavedAt"), d("ConfigSavedAt"), "—")
    End Sub

    Private Sub SaveSettings(sp As ucSettingsPage)
        Dim ch As List(Of String) = sp.ChangedKeys()
        If ch.Count = 0 Then Return
        Dim v As Dictionary(Of String, String) = sp.GetValues()
        Dim missing As New List(Of String)
        For Each k As String In New String() {"AgencyName", "BranchName", "BranchCode", "BranchAddress"}
            If v(k).Trim() = "" Then missing.Add(sp.FriendlyName(k))
        Next
        Dim grace As Integer
        If Not Integer.TryParse(v("GracePeriod"), grace) OrElse grace < 0 OrElse grace > 1440 Then missing.Add("Return grace period (0–1440 minutes)")
        If missing.Count > 0 Then
            MessageBox.Show("Please complete: " & String.Join(", ", missing), "Missing information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        If sp.NeedsReview() AndAlso Not sp.chkReviewed.Checked Then
            MessageBox.Show("Tick the review box before saving billing or timezone changes.", "Confirm sensitive change",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Try
            Dim newVer As Integer = 1
            Using conn As New OleDbConnection(DbHelper.ConnectionString)
                conn.Open()
                Dim cur As String = ""
                Using cmd As New OleDbCommand("SELECT SettingValue FROM SystemSettings WHERE SettingKey = 'ConfigVersion'", conn)
                    Dim o = cmd.ExecuteScalar()
                    If o IsNot Nothing AndAlso Not IsDBNull(o) Then cur = o.ToString()
                End Using
                Dim n As Integer
                If Integer.TryParse(cur, n) Then newVer = n + 1
                Dim toWrite As New Dictionary(Of String, String)
                For Each k As String In ch : toWrite(k) = v(k) : Next
                toWrite("ConfigVersion") = newVer.ToString()
                toWrite("ConfigSavedBy") = _fullName
                toWrite("ConfigSavedAt") = Date.Now.ToString("dd MMM · HH:mm")
                For Each kv In toWrite
                    Dim rows As Integer
                    Using cmd As New OleDbCommand("UPDATE SystemSettings SET SettingValue = ? WHERE SettingKey = ?", conn)
                        cmd.Parameters.Add("?", OleDbType.VarWChar).Value = kv.Value
                        cmd.Parameters.Add("?", OleDbType.VarWChar).Value = kv.Key
                        rows = cmd.ExecuteNonQuery()
                    End Using
                    If rows = 0 Then
                        Using cmd As New OleDbCommand("INSERT INTO SystemSettings (SettingKey, SettingValue) VALUES (?,?)", conn)
                            cmd.Parameters.Add("?", OleDbType.VarWChar).Value = kv.Key
                            cmd.Parameters.Add("?", OleDbType.VarWChar).Value = kv.Value
                            cmd.ExecuteNonQuery()
                        End Using
                    End If
                Next
            End Using
            Dim before As String = String.Join("; ", ch.Select(Function(k) sp.FriendlyName(k) & " = " & sp.BaselineValue(k)))
            Dim after As String = String.Join("; ", ch.Select(Function(k) sp.FriendlyName(k) & " = " & v(k)))
            WriteAudit("Setting changed", "CFG-v" & newVer, "Success", before, after, "System settings saved")
            LoadSettings(sp)
            MessageBox.Show("Settings saved (version " & newVer & ").", "System Settings", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("SaveSettings Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
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