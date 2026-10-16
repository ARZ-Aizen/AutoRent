Imports System.Drawing.Drawing2D

' Maintenance Scheduler (calendar + upcoming list + schedule form).
' Clicking a row in "Upcoming service appointments" opens ucMaintenanceDetails inside this control.
' Needs ucVehicleDetails.vb (FiCard) and ucMaintenanceDetails.vb (details + MaintDraw) in the project.
Public Class ucMaintenance
    Inherits UserControl

    Private Shared ReadOnly Maroon As Color = Color.FromArgb(122, 0, 16)
    Private Shared ReadOnly Ink As Color = Color.FromArgb(38, 38, 38)
    Private Shared ReadOnly Muted As Color = Color.FromArgb(120, 120, 120)
    Private Shared ReadOnly BorderCol As Color = Color.FromArgb(228, 225, 220)
    Private Shared ReadOnly PageBg As Color = Color.FromArgb(245, 244, 242)

    Public Event ServiceHistoryClicked()
    Public Event ScheduleClicked()
    Public Event RecordRequested(rowIndex As Integer)

    Public ReadOnly Details As New ucMaintenanceDetails

    ' ---- buttons ----
    Public ReadOnly btnServiceHistory As New Button
    Public ReadOnly btnCalendar As New Button
    Public ReadOnly btnTable As New Button
    Public ReadOnly btnCancel As New Button
    Public ReadOnly btnSchedule As New Button

    ' ---- data spots ----
    Public ReadOnly lblSubtitle As New Label
    Public ReadOnly lblDueCount As New Label
    Public ReadOnly lblDueSub As New Label
    Public ReadOnly lblOverdueCount As New Label
    Public ReadOnly lblOverdueSub As New Label
    Public ReadOnly lblUnderCount As New Label
    Public ReadOnly lblUnderSub As New Label
    Public ReadOnly lblRecentCount As New Label
    Public ReadOnly lblRecentSub As New Label
    Public ReadOnly lblMonth As New Label
    Public ReadOnly lblUpcomingRange As New Label
    Public ReadOnly lblFormSub As New Label
    Public ReadOnly lblTypeHint As New Label
    Public ReadOnly lblSafetyTitle As New Label
    Public ReadOnly lblSafetyWarning As New Label

    Public ReadOnly Calendar As New CalendarGrid
    Public ReadOnly dgvTable As New DataGridView
    Public ReadOnly dgvUpcoming As New DataGridView

    ' ---- schedule form ----
    Public ReadOnly txtMaintId As New TextBox
    Public ReadOnly cboVehicle As New ComboBox
    Public ReadOnly cboType As New ComboBox
    Public ReadOnly txtDescription As New TextBox
    Public ReadOnly dtpScheduled As New DateTimePicker
    Public ReadOnly txtOdometer As New TextBox
    Public ReadOnly cboProvider As New ComboBox
    Public ReadOnly txtCost As New TextBox
    Public ReadOnly cboPriority As New ComboBox
    Public ReadOnly txtNotes As New TextBox

    Private Class FormItem
        Public Cap As Label
        Public Ctl As Control
        Public Y As Integer
        Public Col As Integer   ' 0 = left half, 1 = right half, 2 = full width
    End Class

    Private ReadOnly pnlScheduler As New Panel
    Private ReadOnly _form As New List(Of FormItem)
    Private ReadOnly _kpis As New List(Of FiCard)
    Private cCal, cUpcoming, cForm As FiCard
    Private pnlSafety As Panel

    Public Sub New()
        Me.BackColor = PageBg
        BuildUI()
        SetMonth(Date.Today)
        ClearForm()
    End Sub

    ' ---------------- public ----------------
    Public Sub ShowDetails()
        pnlScheduler.Visible = False
        Details.Visible = True
        Details.BringToFront()
    End Sub

    Public Sub ShowScheduler()
        Details.Visible = False
        pnlScheduler.Visible = True
        pnlScheduler.BringToFront()
    End Sub

    Public Sub SetMonth(d As Date)
        Calendar.MonthStart = New Date(d.Year, d.Month, 1)
        lblMonth.Text = Calendar.MonthStart.ToString("MMMM yyyy")
        Calendar.Invalidate()
    End Sub

    Public Sub ClearForm()
        txtMaintId.Text = "Assigned on save"
        cboVehicle.SelectedIndex = -1
        cboType.SelectedIndex = -1
        cboPriority.SelectedIndex = -1
        cboProvider.SelectedIndex = -1 : cboProvider.Text = ""
        For Each t As TextBox In New TextBox() {txtDescription, txtOdometer, txtCost, txtNotes}
            t.Text = ""
        Next
        dtpScheduled.Value = Date.Now
    End Sub

    ' ---------------- helpers ----------------
    Private Shared Function F(size As Single, Optional style As FontStyle = FontStyle.Regular) As Font
        Return New Font("Segoe UI", size, style)
    End Function

    Private Shared Function Cap(text As String, f As Font, c As Color, x As Integer, y As Integer) As Label
        Return New Label With {.Text = text, .Font = f, .ForeColor = c, .BackColor = Color.Transparent,
                               .AutoSize = True, .Location = New Point(x, y), .UseMnemonic = False}
    End Function

    Private Shared Sub Spot(l As Label, f As Font, c As Color, x As Integer, y As Integer)
        l.Font = f : l.ForeColor = c : l.BackColor = Color.Transparent
        l.AutoSize = True : l.Location = New Point(x, y) : l.UseMnemonic = False
    End Sub

    Private Shared Sub StyleBtn(b As Button, text As String, w As Integer, h As Integer, primary As Boolean)
        b.Text = text : b.Size = New Size(w, h) : b.FlatStyle = FlatStyle.Flat
        b.Font = F(9) : b.Cursor = Cursors.Hand : b.UseVisualStyleBackColor = False : b.UseMnemonic = False
        b.BackColor = If(primary, Maroon, Color.White)
        b.ForeColor = If(primary, Color.White, Ink)
        b.FlatAppearance.BorderColor = If(primary, Maroon, BorderCol)
        b.FlatAppearance.BorderSize = 1
    End Sub

    Private Shared Sub StyleGrid(g As DataGridView, ParamArray headers() As String)
        g.BackgroundColor = Color.White : g.BorderStyle = BorderStyle.None
        g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal : g.GridColor = BorderCol
        g.RowHeadersVisible = False : g.AllowUserToAddRows = False : g.AllowUserToDeleteRows = False
        g.AllowUserToResizeRows = False : g.ReadOnly = True : g.MultiSelect = False
        g.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        g.EnableHeadersVisualStyles = False
        g.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        g.ColumnHeadersHeight = 32 : g.RowTemplate.Height = 34
        g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        g.ScrollBars = ScrollBars.Vertical
        g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(250, 249, 247)
        g.ColumnHeadersDefaultCellStyle.ForeColor = Muted
        g.ColumnHeadersDefaultCellStyle.Font = F(8)
        g.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(250, 249, 247)
        g.ColumnHeadersDefaultCellStyle.SelectionForeColor = Muted
        g.ColumnHeadersDefaultCellStyle.Padding = New Padding(12, 0, 0, 0)
        g.DefaultCellStyle.Font = F(8.5!) : g.DefaultCellStyle.ForeColor = Ink
        g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(250, 249, 247)
        g.DefaultCellStyle.SelectionForeColor = Ink
        g.DefaultCellStyle.Padding = New Padding(12, 0, 0, 0)
        For i As Integer = 0 To headers.Length - 1
            g.Columns.Add("col" & i, headers(i))
        Next
    End Sub

    Private Sub AddKpi(index As Integer, caption As String, valueLbl As Label, subLbl As Label)
        Dim c As New FiCard
        c.Controls.Add(Cap(caption, F(8), Ink, 16, 10))
        Spot(valueLbl, F(20, FontStyle.Bold), Maroon, 14, 26)
        Spot(subLbl, F(7.5!), Muted, 16, 60)
        c.Controls.Add(valueLbl) : c.Controls.Add(subLbl)
        _kpis.Add(c)
        pnlScheduler.Controls.Add(c)
    End Sub

    Private Sub AddFormField(caption As String, ctl As Control, y As Integer, col As Integer)
        Dim fi As New FormItem With {.Cap = Cap(caption, F(8, FontStyle.Bold), Ink, 0, 0), .Ctl = ctl, .Y = y, .Col = col}
        If TypeOf ctl Is TextBox Then
            DirectCast(ctl, TextBox).BorderStyle = BorderStyle.FixedSingle : ctl.Font = F(9)
        ElseIf TypeOf ctl Is ComboBox Then
            Dim cb = DirectCast(ctl, ComboBox)
            cb.FlatStyle = FlatStyle.Flat : cb.BackColor = Color.White : cb.Font = F(9)
            cb.DropDownStyle = If(cb Is cboProvider, ComboBoxStyle.DropDown, ComboBoxStyle.DropDownList)
        ElseIf TypeOf ctl Is DateTimePicker Then
            Dim d = DirectCast(ctl, DateTimePicker)
            d.Format = DateTimePickerFormat.Custom : d.CustomFormat = "dd MMM yyyy HH:mm" : d.Font = F(9)
        End If
        cForm.Controls.Add(fi.Cap) : cForm.Controls.Add(ctl)
        _form.Add(fi)
    End Sub

    ' ---------------- layout ----------------
    Private Sub ArrangeScheduler()
        Dim w As Integer = Math.Max(pnlScheduler.ClientSize.Width, 900) - 48
        Dim lw As Integer = w - 316
        btnServiceHistory.Location = New Point(24 + w - btnServiceHistory.Width, 20)
        Dim kw As Integer = (w - 36) \ 4
        For i As Integer = 0 To _kpis.Count - 1
            _kpis(i).SetBounds(24 + i * (kw + 12), 96, kw, 84)
        Next
        cCal.SetBounds(24, 196, lw, 346)
        cUpcoming.SetBounds(24, 558, lw, 200)
        cForm.SetBounds(24 + lw + 16, 196, 300, 562)
    End Sub

    Private Sub BuildUI()
        Me.SuspendLayout()

        Details.Dock = DockStyle.Fill
        Details.Visible = False
        Controls.Add(Details)

        pnlScheduler.Dock = DockStyle.Fill
        pnlScheduler.BackColor = PageBg
        pnlScheduler.AutoScroll = True
        pnlScheduler.AutoScrollMinSize = New Size(900, 790)
        Controls.Add(pnlScheduler)

        ' ----- header -----
        pnlScheduler.Controls.Add(Cap("Maintenance Scheduler", F(20, FontStyle.Bold), Ink, 22, 14))
        Spot(lblSubtitle, F(9), Muted, 26, 58)
        pnlScheduler.Controls.Add(lblSubtitle)
        StyleBtn(btnServiceHistory, "Service history", 140, 34, False)
        pnlScheduler.Controls.Add(btnServiceHistory)

        ' ----- KPI cards -----
        AddKpi(0, "Due within 7 days", lblDueCount, lblDueSub)
        AddKpi(1, "Overdue", lblOverdueCount, lblOverdueSub)
        AddKpi(2, "Under maintenance", lblUnderCount, lblUnderSub)
        AddKpi(3, "Recently completed", lblRecentCount, lblRecentSub)

        ' ----- calendar card -----
        cCal = New FiCard
        Spot(lblMonth, F(11, FontStyle.Bold), Ink, 18, 14)
        cCal.Controls.Add(lblMonth)
        StyleBtn(btnCalendar, "Calendar", 80, 28, True)
        StyleBtn(btnTable, "Table view", 80, 28, False)
        btnCalendar.Top = 10 : btnTable.Top = 10
        cCal.Controls.AddRange(New Control() {btnCalendar, btnTable})
        cCal.Controls.Add(Calendar)
        StyleGrid(dgvTable, "Maintenance ID", "Vehicle", "Date / Time", "Type", "Priority")
        dgvTable.Visible = False
        cCal.Controls.Add(dgvTable)
        AddHandler cCal.Resize, Sub(s, e)
                                    btnTable.Left = cCal.Width - 18 - btnTable.Width
                                    btnCalendar.Left = btnTable.Left - 8 - btnCalendar.Width
                                    Calendar.SetBounds(18, 50, cCal.Width - 36, cCal.Height - 66)
                                    dgvTable.SetBounds(1, 50, cCal.Width - 2, cCal.Height - 52)
                                End Sub
        AddHandler btnCalendar.Click, Sub(s, e) SwitchView(True)
        AddHandler btnTable.Click, Sub(s, e) SwitchView(False)
        pnlScheduler.Controls.Add(cCal)

        ' ----- upcoming appointments card -----
        cUpcoming = New FiCard
        cUpcoming.Controls.Add(Cap("Upcoming service appointments", F(11, FontStyle.Bold), Ink, 18, 12))
        Spot(lblUpcomingRange, F(7.5!), Muted, 0, 16)
        cUpcoming.Controls.Add(lblUpcomingRange)
        AddHandler lblUpcomingRange.SizeChanged, Sub(s, e) lblUpcomingRange.Left = cUpcoming.Width - 18 - lblUpcomingRange.Width
        StyleGrid(dgvUpcoming, "Service ID", "Vehicle", "Date / Time", "Type", "Priority")
        cUpcoming.Controls.Add(dgvUpcoming)
        AddHandler cUpcoming.Resize, Sub(s, e)
                                         lblUpcomingRange.Left = cUpcoming.Width - 18 - lblUpcomingRange.Width
                                         dgvUpcoming.SetBounds(1, 44, cUpcoming.Width - 2, cUpcoming.Height - 46)
                                     End Sub
        AddHandler dgvUpcoming.CellFormatting, Sub(s, e)
                                                   If e.RowIndex >= 0 AndAlso e.ColumnIndex = 0 Then
                                                       e.CellStyle.ForeColor = Maroon
                                                       e.CellStyle.Font = F(8.5!, FontStyle.Bold)
                                                   End If
                                               End Sub
        AddHandler dgvUpcoming.CellPainting, Sub(s, e)
                                                 If e.RowIndex >= 0 AndAlso e.ColumnIndex = 4 Then MaintDraw.Pill(e)
                                             End Sub
        AddHandler dgvUpcoming.CellClick, Sub(s, e)
                                              If e.RowIndex >= 0 Then
                                                  ShowDetails()
                                                  RaiseEvent RecordRequested(e.RowIndex)
                                              End If
                                          End Sub
        pnlScheduler.Controls.Add(cUpcoming)

        ' ----- schedule maintenance form -----
        cForm = New FiCard
        cForm.Controls.Add(Cap("Schedule maintenance", F(11, FontStyle.Bold), Ink, 18, 12))
        Spot(lblFormSub, F(7.5!), Muted, 18, 36)
        cForm.Controls.Add(lblFormSub)

        txtMaintId.ReadOnly = True : txtMaintId.BackColor = Color.FromArgb(238, 238, 236)
        cboType.Items.AddRange(New Object() {"Routine Service", "Oil Change", "Tire Service", "Brake Inspection", "Engine Repair", "Electrical", "Repair", "Other"})
        cboPriority.Items.AddRange(New Object() {"Low", "Normal", "High", "Urgent"})

        AddFormField("Maintenance ID", txtMaintId, 62, 0)
        AddFormField("Vehicle *", cboVehicle, 62, 1)
        AddFormField("Type *", cboType, 114, 2)
        Spot(lblTypeHint, F(7), Muted, 18, 158)
        lblTypeHint.AutoSize = False : lblTypeHint.Size = New Size(264, 24)
        cForm.Controls.Add(lblTypeHint)
        AddFormField("Description *", txtDescription, 188, 2)
        AddFormField("Scheduled date / time *", dtpScheduled, 240, 0)
        AddFormField("Odometer (km) *", txtOdometer, 240, 1)
        AddFormField("Service provider *", cboProvider, 292, 2)
        AddFormField("Estimated cost (USD) *", txtCost, 344, 0)
        AddFormField("Priority *", cboPriority, 344, 1)
        AddFormField("Notes", txtNotes, 396, 2)

        pnlSafety = New Panel With {.BackColor = Color.FromArgb(252, 243, 228), .Location = New Point(18, 452), .Height = 54}
        Spot(lblSafetyTitle, F(8, FontStyle.Bold), Color.FromArgb(176, 112, 32), 8, 6)
        lblSafetyWarning.AutoSize = False : lblSafetyWarning.Font = F(7.5!) : lblSafetyWarning.ForeColor = Muted
        lblSafetyWarning.BackColor = Color.Transparent : lblSafetyWarning.SetBounds(8, 22, 248, 30)
        lblSafetyWarning.UseMnemonic = False
        pnlSafety.Controls.AddRange(New Control() {lblSafetyTitle, lblSafetyWarning})
        cForm.Controls.Add(pnlSafety)

        StyleBtn(btnCancel, "Cancel", 70, 32, False)
        StyleBtn(btnSchedule, "📅  Schedule service", 150, 32, True)
        btnCancel.Location = New Point(18, 516)
        cForm.Controls.AddRange(New Control() {btnCancel, btnSchedule})

        AddHandler cForm.Resize, Sub(s, e)
                                     Dim fullW As Integer = cForm.Width - 36
                                     Dim halfW As Integer = (fullW - 12) \ 2
                                     For Each fi In _form
                                         Dim x As Integer = 18
                                         Dim wd As Integer = fullW
                                         If fi.Col = 0 Then wd = halfW
                                         If fi.Col = 1 Then x = 18 + halfW + 12 : wd = halfW
                                         fi.Cap.Location = New Point(x, fi.Y)
                                         fi.Ctl.SetBounds(x, fi.Y + 16, wd, 24)
                                     Next
                                     lblTypeHint.Width = fullW
                                     pnlSafety.Width = fullW
                                     btnSchedule.Location = New Point(cForm.Width - 18 - btnSchedule.Width, 516)
                                 End Sub
        pnlScheduler.Controls.Add(cForm)

        AddHandler pnlScheduler.Resize, Sub(s, e) ArrangeScheduler()
        AddHandler btnServiceHistory.Click, Sub(s, e) RaiseEvent ServiceHistoryClicked()
        AddHandler btnSchedule.Click, Sub(s, e) RaiseEvent ScheduleClicked()
        AddHandler btnCancel.Click, Sub(s, e) ClearForm()

        Me.ResumeLayout(False)
        ArrangeScheduler()
    End Sub

    Public Sub SwitchView(showCalendar As Boolean)
        Calendar.Visible = showCalendar
        dgvTable.Visible = Not showCalendar
        btnCalendar.BackColor = If(showCalendar, Maroon, Color.White)
        btnCalendar.ForeColor = If(showCalendar, Color.White, Ink)
        btnTable.BackColor = If(showCalendar, Color.White, Maroon)
        btnTable.ForeColor = If(showCalendar, Ink, Color.White)
    End Sub
End Class

''' <summary>Month calendar grid. Put short texts on days with Marks(date).Add("text").</summary>
Public Class CalendarGrid
    Inherits Control

    Public Event DayClicked(day As Date)

    <System.ComponentModel.Browsable(False), System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
    Public Property MonthStart As Date = New Date(Date.Today.Year, Date.Today.Month, 1)
    Public ReadOnly Marks As New Dictionary(Of Date, List(Of String))

    Public Sub New()
        SetStyle(ControlStyles.UserPaint Or ControlStyles.AllPaintingInWmPaint Or
                 ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw, True)
        Me.BackColor = Color.White
    End Sub

    Private Function GridStart() As Date
        Dim offset As Integer = (CInt(MonthStart.DayOfWeek) + 6) Mod 7   ' Monday = 0
        Return MonthStart.AddDays(-offset)
    End Function

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim g As Graphics = e.Graphics
        g.Clear(Color.White)
        Dim headH As Integer = 24
        Dim cw As Single = Width / 7.0F
        Dim ch As Single = (Height - headH) / 6.0F
        Dim names() As String = {"MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN"}

        Using f As New Font("Segoe UI", 7, FontStyle.Bold), br As New SolidBrush(Color.FromArgb(120, 120, 120))
            For c As Integer = 0 To 6
                g.DrawString(names(c), f, br, c * cw + 4, 6)
            Next
        End Using

        Dim start As Date = GridStart()
        Using border As New Pen(Color.FromArgb(228, 225, 220)),
              fNum As New Font("Segoe UI", 7.5!), fEv As New Font("Segoe UI", 6.5!),
              inkBr As New SolidBrush(Color.FromArgb(38, 38, 38)),
              fadeBr As New SolidBrush(Color.FromArgb(185, 185, 185)),
              evBr As New SolidBrush(Color.FromArgb(122, 0, 16)),
              todayBr As New SolidBrush(Color.FromArgb(250, 228, 230)),
              todayPen As New Pen(Color.FromArgb(122, 0, 16))

            For r As Integer = 0 To 5
                For c As Integer = 0 To 6
                    Dim d As Date = start.AddDays(r * 7 + c)
                    Dim rect As New RectangleF(c * cw, headH + r * ch, cw, ch)
                    Dim inMonth As Boolean = (d.Month = MonthStart.Month)
                    Dim isToday As Boolean = (d.Date = Date.Today)

                    If isToday Then g.FillRectangle(todayBr, rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2)
                    g.DrawRectangle(If(isToday, todayPen, border), rect.X, rect.Y, rect.Width, rect.Height)
                    g.DrawString(d.Day.ToString(), fNum, If(inMonth, inkBr, fadeBr), rect.X + 5, rect.Y + 3)

                    If inMonth AndAlso Marks.ContainsKey(d.Date) Then
                        Dim y As Single = rect.Y + 19
                        For Each t In Marks(d.Date).Take(2)
                            Dim tr As New Rectangle(CInt(rect.X) + 4, CInt(y), CInt(rect.Width) - 8, 12)
                            TextRenderer.DrawText(g, t, fEv, tr, Color.FromArgb(122, 0, 16),
                                TextFormatFlags.EndEllipsis Or TextFormatFlags.NoPadding Or TextFormatFlags.Left)
                            y += 12
                        Next
                    End If
                Next
            Next
        End Using
    End Sub

    Protected Overrides Sub OnMouseClick(e As MouseEventArgs)
        MyBase.OnMouseClick(e)
        Dim headH As Integer = 24
        If e.Y < headH Then Return
        Dim c As Integer = CInt(Math.Floor(e.X / (Width / 7.0F)))
        Dim r As Integer = CInt(Math.Floor((e.Y - headH) / ((Height - headH) / 6.0F)))
        If c < 0 OrElse c > 6 OrElse r < 0 OrElse r > 5 Then Return
        RaiseEvent DayClicked(GridStart().AddDays(r * 7 + c))
    End Sub
End Class