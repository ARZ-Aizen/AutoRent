Public Class ucAdminOverview
    Inherits UserControl

    ' ---- Custom Event for Audit Logs Navigation ----
    Public Event ViewAuditLogsRequested As EventHandler

    ' ---- Theme Colors ----
    Private ReadOnly CardBgColor As Color = Color.White
    Private ReadOnly BorderColor As Color = Color.FromArgb(225, 225, 225)
    Private ReadOnly PrimaryRed As Color = Color.FromArgb(122, 0, 16)
    Private ReadOnly TextDark As Color = Color.FromArgb(30, 30, 30)
    Private ReadOnly TextMuted As Color = Color.FromArgb(110, 110, 110)

    ' ---- buttons ----
    Public ReadOnly btnExport As New Button With {.Text = "Export snapshot", .Width = 120, .Height = 32, .FlatStyle = FlatStyle.Standard, .Cursor = Cursors.Hand, .Dock = DockStyle.Right}
    Public ReadOnly btnToday As New Button With {.Text = "Today", .Height = 28, .FlatStyle = FlatStyle.Flat, .BackColor = Color.FromArgb(122, 0, 16), .ForeColor = Color.White, .Cursor = Cursors.Hand}
    Public ReadOnly btnLast7 As New Button With {.Text = "Last 7 Days", .Height = 28, .FlatStyle = FlatStyle.Flat, .BackColor = Color.FromArgb(245, 245, 245), .ForeColor = Color.FromArgb(80, 80, 80), .Cursor = Cursors.Hand}
    Public ReadOnly btnLast30 As New Button With {.Text = "Last 30 Days", .Height = 28, .FlatStyle = FlatStyle.Flat, .BackColor = Color.FromArgb(245, 245, 245), .ForeColor = Color.FromArgb(80, 80, 80), .Cursor = Cursors.Hand}
    Public ReadOnly btnCustom As New Button With {.Text = "Custom Date Range", .Height = 28, .FlatStyle = FlatStyle.Flat, .BackColor = Color.FromArgb(245, 245, 245), .ForeColor = Color.FromArgb(80, 80, 80), .Cursor = Cursors.Hand}
    Public ReadOnly btnViewAuditLogs As New Button With {.Text = "View audit logs", .Width = 120, .Height = 26, .FlatStyle = FlatStyle.Standard, .Cursor = Cursors.Hand, .Dock = DockStyle.Right}

    ' ---- data spots ----
    Public ReadOnly lblTotalFleet As New Label
    Public ReadOnly lblAvailable As New Label
    Public ReadOnly lblActiveRentals As New Label
    Public ReadOnly lblMaintenance As New Label
    Public ReadOnly lblTodayRevenue As New Label
    Public ReadOnly lblUtilization As New Label

    Public ReadOnly pnlChart As New Panel With {.Dock = DockStyle.Fill, .BackColor = Color.White}
    Public ReadOnly pnlFleetReadiness As New Panel With {.Dock = DockStyle.Fill, .BackColor = Color.White}
    Public ReadOnly dgvActivity As New DataGridView

    Public Sub New()
        Try
            Me.BackColor = Color.FromArgb(248, 249, 250)
            Me.Dock = DockStyle.Fill

            Dim root As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 1, .RowCount = 4, .Padding = New Padding(16)}
            root.RowStyles.Add(New RowStyle(SizeType.Absolute, 65))  ' Header
            root.RowStyles.Add(New RowStyle(SizeType.Absolute, 105)) ' KPIs
            root.RowStyles.Add(New RowStyle(SizeType.Percent, 55))   ' Charts / Readiness
            root.RowStyles.Add(New RowStyle(SizeType.Percent, 45))   ' Activity Table

            ' Wire up the View audit logs button to raise the event
            AddHandler btnViewAuditLogs.Click, Sub(sender, e) RaiseEvent ViewAuditLogsRequested(Me, EventArgs.Empty)

            ' ==========================================
            ' ROW 0: Title, Subtitle, & Export Button
            ' ==========================================
            Dim headerPanel As New Panel With {.Dock = DockStyle.Fill}
            Dim titleBox As New FlowLayoutPanel With {.Dock = DockStyle.Fill, .FlowDirection = FlowDirection.TopDown, .WrapContents = False}

            Dim lblTitle As New Label With {.Text = "Admin Overview Dashboard", .Font = New Font("Segoe UI", 16, FontStyle.Bold), .ForeColor = TextDark, .AutoSize = True}
            Dim lblSubTitle As New Label With {.Text = "An executive view of fleet readiness, daily performance and controlled operations.", .Font = New Font("Segoe UI", 9.5F, FontStyle.Regular), .ForeColor = TextMuted, .AutoSize = True}

            titleBox.Controls.Add(lblTitle)
            titleBox.Controls.Add(lblSubTitle)

            headerPanel.Controls.Add(btnExport)
            headerPanel.Controls.Add(titleBox)
            root.Controls.Add(headerPanel, 0, 0)

            ' ==========================================
            ' ROW 1: Six KPI Cards
            ' ==========================================
            Dim kpis As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 6, .RowCount = 1}
            For i As Integer = 0 To 5
                kpis.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 16.66F))
                Dim card = CreateKpiCard(
                    SwitchTitle(i),
                    SwitchLabel(i),
                    SwitchSub(i)
                )
                kpis.Controls.Add(card, i, 0)
            Next
            root.Controls.Add(kpis, 0, 1)

            ' ==========================================
            ' ROW 2: Revenue Performance & Fleet Readiness
            ' ==========================================
            Dim midPanel As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 2, .RowCount = 1}
            midPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 65))
            midPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 35))
            midPanel.Padding = New Padding(0, 4, 0, 4)

            ' Left: Revenue Card (Top header added FIRST, Fill panel added LAST)
            Dim cardRevenue As New Panel With {.Dock = DockStyle.Fill, .BackColor = CardBgColor, .Padding = New Padding(12)}
            AddHandler cardRevenue.Paint, AddressOf DrawCardBorder

            Dim revHeader As New Panel With {.Dock = DockStyle.Top, .Height = 36}
            Dim lblRevTitle As New Label With {.Text = "Revenue performance", .Dock = DockStyle.Left, .Font = New Font("Segoe UI", 11, FontStyle.Bold), .ForeColor = TextDark, .TextAlign = ContentAlignment.MiddleLeft, .AutoSize = True}

            Dim tabsPanel As New FlowLayoutPanel With {.Dock = DockStyle.Right, .AutoSize = True, .FlowDirection = FlowDirection.LeftToRight}
            For Each b In New Button() {btnToday, btnLast7, btnLast30, btnCustom}
                b.FlatAppearance.BorderSize = 0
                b.Margin = New Padding(2, 0, 2, 0)
            Next
            tabsPanel.Controls.AddRange(New Control() {btnToday, btnLast7, btnLast30, btnCustom})

            revHeader.Controls.Add(tabsPanel)
            revHeader.Controls.Add(lblRevTitle)

            pnlChart.Dock = DockStyle.Fill
            cardRevenue.Controls.Add(revHeader)    ' Added first (Top)
            cardRevenue.Controls.Add(pnlChart)     ' Added LAST (Fill)
            midPanel.Controls.Add(cardRevenue, 0, 0)

            ' Right: Fleet Readiness Card (Top title added FIRST, Fill panel added LAST)
            Dim cardFleet As New Panel With {.Dock = DockStyle.Fill, .BackColor = CardBgColor, .Padding = New Padding(12), .Margin = New Padding(8, 0, 0, 0)}
            AddHandler cardFleet.Paint, AddressOf DrawCardBorder

            Dim lblFleetTitle As New Label With {.Text = "Fleet readiness", .Dock = DockStyle.Top, .Height = 28, .Font = New Font("Segoe UI", 11, FontStyle.Bold), .ForeColor = TextDark}
            pnlFleetReadiness.Dock = DockStyle.Fill
            cardFleet.Controls.Add(lblFleetTitle)       ' Added first (Top)
            cardFleet.Controls.Add(pnlFleetReadiness)   ' Added LAST (Fill)
            midPanel.Controls.Add(cardFleet, 1, 0)

            root.Controls.Add(midPanel, 0, 2)

            ' ==========================================
            ' ROW 3: Recent Activity Table (Top header added FIRST, Fill grid added LAST)
            ' ==========================================
VB_Label_Fix:
            Dim cardActivity As New Panel With {.Dock = DockStyle.Fill, .BackColor = CardBgColor, .Padding = New Padding(12)}
            AddHandler cardActivity.Paint, AddressOf DrawCardBorder

            Dim actHeader As New Panel With {.Dock = DockStyle.Top, .Height = 32}
            Dim lblActTitle As New Label With {.Text = "Recent administrative activity", .Dock = DockStyle.Left, .Font = New Font("Segoe UI", 11, FontStyle.Bold), .ForeColor = TextDark, .TextAlign = ContentAlignment.MiddleLeft, .AutoSize = True}

            actHeader.Controls.Add(btnViewAuditLogs)
            actHeader.Controls.Add(lblActTitle)

            dgvActivity.Dock = DockStyle.Fill
            dgvActivity.BackgroundColor = Color.White
            dgvActivity.BorderStyle = BorderStyle.None
            dgvActivity.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            dgvActivity.RowHeadersVisible = False
            dgvActivity.AllowUserToAddRows = False
            dgvActivity.ReadOnly = True
            dgvActivity.AutoGenerateColumns = True
            dgvActivity.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            dgvActivity.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            dgvActivity.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250)
            dgvActivity.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9, FontStyle.Bold)
            dgvActivity.ColumnHeadersDefaultCellStyle.ForeColor = TextMuted
            dgvActivity.EnableHeadersVisualStyles = False

            cardActivity.Controls.Add(actHeader)   ' Added first (Top)
            cardActivity.Controls.Add(dgvActivity) ' Added LAST (Fill)
            root.Controls.Add(cardActivity, 0, 3)

            Me.Controls.Add(root)
        Catch ex As Exception
            MessageBox.Show("Error initializing Overview UI: " & ex.Message, "UI Exception", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' --- Helpers for Clean KPI Card Creation (Top/Bottom added first, Fill value added LAST) ---
    Private Function CreateKpiCard(title As String, valLabel As Label, subText As String) As Panel
        Dim pnl As New Panel With {.Dock = DockStyle.Fill, .BackColor = CardBgColor, .Margin = New Padding(4, 0, 4, 0), .Padding = New Padding(8)}
        AddHandler pnl.Paint, AddressOf DrawCardBorder

        Dim lblT As New Label With {.Text = title, .Dock = DockStyle.Top, .Height = 20, .Font = New Font("Segoe UI", 8.5F, FontStyle.Regular), .ForeColor = TextMuted}
        Dim lblS As New Label With {.Text = subText, .Dock = DockStyle.Bottom, .Height = 16, .Font = New Font("Segoe UI", 8, FontStyle.Regular), .ForeColor = TextMuted}

        valLabel.Text = "0"
        valLabel.Dock = DockStyle.Fill
        valLabel.Font = New Font("Segoe UI", 18, FontStyle.Bold)
        valLabel.ForeColor = PrimaryRed
        valLabel.TextAlign = ContentAlignment.MiddleLeft

        pnl.Controls.Add(lblT)      ' Top added first
        pnl.Controls.Add(lblS)      ' Bottom added second
        pnl.Controls.Add(valLabel)  ' Fill added LAST so it doesn't obscure titles
        Return pnl
    End Function

    Private Sub DrawCardBorder(sender As Object, e As PaintEventArgs)
        Dim p = DirectCast(sender, Panel)
        ControlPaint.DrawBorder(e.Graphics, p.ClientRectangle, BorderColor, ButtonBorderStyle.Solid)
    End Sub

    Private Function SwitchTitle(i As Integer) As String
        Select Case i
            Case 0 : Return "Total Fleet Vehicles"
            Case 1 : Return "Available Vehicles"
            Case 2 : Return "Active Rentals"
            Case 3 : Return "Under Maintenance"
            Case 4 : Return "Today's Revenue"
            Case Else : Return "Fleet Utilization Rate"
        End Select
    End Function

    Private Function SwitchLabel(i As Integer) As Label
        Select Case i
            Case 0 : Return lblTotalFleet
            Case 1 : Return lblAvailable
            Case 2 : Return lblActiveRentals
            Case 3 : Return lblMaintenance
            Case 4 : Return lblTodayRevenue
            Case Else : Return lblUtilization
        End Select
    End Function

    Private Function SwitchSub(i As Integer) As String
        Select Case i
            Case 0 : Return "4 categories · active"
            Case 1 : Return "+4 vs yesterday"
            Case 2 : Return "+6 vs yesterday"
            Case 3 : Return "2 overdue services"
            Case 4 : Return "+8.8% vs yesterday"
            Case Else : Return "128 / 184 · 3.3 pp"
        End Select
    End Function

End Class