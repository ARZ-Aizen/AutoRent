' Daily Dispatch Reports. All data spots are empty: fill them from your code.
' Needs ucVehicleDetails.vb (FiCard), ucMaintenanceDetails.vb (MaintDraw pills) and ucRevenueReports.vb (RptKit) in the project.
Public Class ucDispatchReports
    Inherits UserControl

    Public Event ExportClicked()
    Public Event PrintReportClicked()
    Public Event ViewRecordClicked()
    Public Event FilterChanged()
    Public Event DispatchSelected(rowIndex As Integer)

    ' ---- header ----
    Public ReadOnly lblSubtitle As New Label
    Public ReadOnly btnExport As New Button

    ' ---- KPI values / sub texts ----
    Public ReadOnly lblScheduled As New Label
    Public ReadOnly lblScheduledSub As New Label
    Public ReadOnly lblCheckOuts As New Label
    Public ReadOnly lblCheckOutsSub As New Label
    Public ReadOnly lblReturns As New Label
    Public ReadOnly lblReturnsSub As New Label
    Public ReadOnly lblCancelled As New Label
    Public ReadOnly lblCancelledSub As New Label
    Public ReadOnly lblOverdue As New Label
    Public ReadOnly lblOverdueSub As New Label
    Public ReadOnly lblNotDispatched As New Label
    Public ReadOnly lblNotDispatchedSub As New Label
    Public ReadOnly lblUtilization As New Label
    Public ReadOnly lblUtilizationSub As New Label

    ' ---- filters ----
    Public ReadOnly dtpReportDate As New DateTimePicker
    Public ReadOnly cboVehicle As New ComboBox
    Public ReadOnly cboClerk As New ComboBox
    Public ReadOnly cboContractStatus As New ComboBox

    ' ---- dispatch register ----
    Public ReadOnly lblRegisterSub As New Label
    Public ReadOnly btnPrintReport As New Button
    Public ReadOnly dgvDispatch As New DataGridView
    Public ReadOnly lblShowing As New Label
    Public ReadOnly btnPrev As New Button
    Public ReadOnly btnPage1 As New Button
    Public ReadOnly btnPage2 As New Button
    Public ReadOnly btnPage3 As New Button
    Public ReadOnly btnNext As New Button

    ' ---- selected dispatch ----
    Public ReadOnly lblSelTitle As New Label
    Public ReadOnly lblSelSub As New Label
    Public ReadOnly lblSelVehicle As New Label
    Public ReadOnly lblSelContract As New Label
    Public ReadOnly lblSelInspection As New Label
    Public ReadOnly lblSelFleetStatus As New Label
    Public ReadOnly btnViewRecord As New Button

    ' ---- safety panel ----
    Public ReadOnly lblSafetyTitle As New Label
    Public ReadOnly lblSafetyNote As New Label

    Private cFilter, cReg, cSel As FiCard
    Private pnlSafety As Panel
    Private ReadOnly _kpis As New List(Of FiCard)
    Private ReadOnly _filterItems As New List(Of Tuple(Of Label, Control))

    Public Sub New()
        Me.BackColor = RptKit.PageBg
        Me.AutoScroll = True
        Me.AutoScrollMinSize = New Size(960, 780)
        BuildUI()
    End Sub

    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e)
        ArrangeLayout()
    End Sub

    Private Sub ArrangeLayout()
        If cSel Is Nothing Then Return
        Dim w As Integer = Math.Max(ClientSize.Width, 960) - 48
        btnExport.Location = New Point(24 + w - btnExport.Width, 20)
        Dim kw As Integer = (w - 6 * 10) \ 7
        For i As Integer = 0 To _kpis.Count - 1
            _kpis(i).SetBounds(24 + i * (kw + 10), 96, kw, 96)
        Next
        cFilter.SetBounds(24, 208, w, 72)
        cReg.SetBounds(24, 296, w, 300)
        Dim sw As Integer = CInt(w * 0.58)
        cSel.SetBounds(24, 612, sw, 150)
        pnlSafety.SetBounds(24 + sw + 16, 612, w - sw - 16, 80)
    End Sub

    Private Sub BuildUI()
        Me.SuspendLayout()

        Controls.Add(RptKit.Cap("Daily Dispatch Reports", RptKit.F(20, FontStyle.Bold), RptKit.Ink, 22, 14))
        RptKit.Spot(lblSubtitle, RptKit.F(9), RptKit.Muted, 26, 58)
        Controls.Add(lblSubtitle)
        RptKit.StyleBtn(btnExport, "⭳  Authorized export", 170, 34, False)
        Controls.Add(btnExport)

        ' ----- KPI cards -----
        AddKpi("Scheduled Dispatches", lblScheduled, lblScheduledSub)
        AddKpi("Completed Check-Outs", lblCheckOuts, lblCheckOutsSub)
        AddKpi("Completed Returns", lblReturns, lblReturnsSub)
        AddKpi("Cancelled Reservations", lblCancelled, lblCancelledSub)
        AddKpi("Overdue Returns", lblOverdue, lblOverdueSub)
        AddKpi("Vehicles Not Dispatched", lblNotDispatched, lblNotDispatchedSub)
        AddKpi("Fleet Utilization", lblUtilization, lblUtilizationSub)

        ' ----- filters -----
        cFilter = New FiCard
        AddFilter("Report date", dtpReportDate)
        AddFilter("Vehicle / plate", cboVehicle)
        AddFilter("Assigned clerk", cboClerk)
        AddFilter("Contract status", cboContractStatus)
        dtpReportDate.Format = DateTimePickerFormat.Custom : dtpReportDate.CustomFormat = "dd MMM yyyy"
        cboVehicle.Items.Add("All fleet vehicles")
        cboClerk.Items.Add("All clerks")
        cboContractStatus.Items.AddRange(New Object() {"All statuses", "Reserved", "Dispatched", "Returned", "Cancelled"})
        For Each c As ComboBox In New ComboBox() {cboVehicle, cboClerk, cboContractStatus}
            c.SelectedIndex = 0
        Next
        AddHandler cFilter.Resize, Sub(s, e)
                                       Dim gap As Integer = 16
                                       Dim cw As Integer = (cFilter.Width - 36 - gap * 3) \ 4
                                       Dim xx As Integer = 18
                                       For Each pair In _filterItems
                                           pair.Item1.Location = New Point(xx, 10)
                                           pair.Item2.SetBounds(xx, 32, cw, 26)
                                           xx += cw + gap
                                       Next
                                   End Sub
        For Each pair In _filterItems
            If TypeOf pair.Item2 Is ComboBox Then
                AddHandler DirectCast(pair.Item2, ComboBox).SelectedIndexChanged, Sub(s, e) RaiseEvent FilterChanged()
            Else
                AddHandler DirectCast(pair.Item2, DateTimePicker).ValueChanged, Sub(s, e) RaiseEvent FilterChanged()
            End If
        Next
        Controls.Add(cFilter)

        ' ----- dispatch register -----
        cReg = New FiCard
        cReg.Controls.Add(RptKit.Cap("Dispatch register", RptKit.F(11, FontStyle.Bold), RptKit.Ink, 18, 12))
        RptKit.Spot(lblRegisterSub, RptKit.F(7.5!), RptKit.Muted, 18, 34)
        cReg.Controls.Add(lblRegisterSub)
        RptKit.StyleBtn(btnPrintReport, "🖶  Print report", 120, 30, False)
        btnPrintReport.Top = 12
        cReg.Controls.Add(btnPrintReport) : RptKit.PinRight(cReg, btnPrintReport, 18)

        RptKit.StyleGrid(dgvDispatch, "Booking ID", "Vehicle ID", "Plate", "Customer", "Assigned Clerk", "Scheduled Pickup",
                         "Actual Dispatch Timestamp", "Expected Return", "Actual Return Timestamp", "Contract Status")
        dgvDispatch.RowTemplate.Height = 44
        AddHandler dgvDispatch.CellFormatting, Sub(s, e)
                                                   If e.RowIndex >= 0 AndAlso e.ColumnIndex = 0 Then
                                                       e.CellStyle.ForeColor = RptKit.Maroon
                                                       e.CellStyle.Font = RptKit.F(8, FontStyle.Bold)
                                                   End If
                                               End Sub
        AddHandler dgvDispatch.CellPainting, Sub(s, e)
                                                 If e.RowIndex >= 0 AndAlso e.ColumnIndex = 9 Then MaintDraw.Pill(e)
                                             End Sub
        AddHandler dgvDispatch.CellClick, Sub(s, e)
                                              If e.RowIndex >= 0 Then RaiseEvent DispatchSelected(e.RowIndex)
                                          End Sub
        cReg.Controls.Add(dgvDispatch)
        RptKit.Spot(lblShowing, RptKit.F(7.5!), RptKit.Muted, 18, 0)
        cReg.Controls.Add(lblShowing)

        RptKit.StyleBtn(btnPrev, "‹", 26, 26, False)
        RptKit.StyleBtn(btnPage1, "1", 26, 26, True)
        RptKit.StyleBtn(btnPage2, "2", 26, 26, False)
        RptKit.StyleBtn(btnPage3, "3", 26, 26, False)
        RptKit.StyleBtn(btnNext, "›", 26, 26, False)
        For Each b As Button In New Button() {btnPrev, btnPage1, btnPage2, btnPage3, btnNext}
            b.Font = RptKit.F(8.5!) : b.Padding = New Padding(0)
            cReg.Controls.Add(b)
        Next
        AddHandler cReg.Resize, Sub(s, e)
                                    dgvDispatch.SetBounds(1, 58, cReg.Width - 2, cReg.Height - 58 - 40)
                                    Dim fy As Integer = cReg.Height - 34
                                    lblShowing.Top = fy + 6
                                    Dim xx As Integer = cReg.Width - 18 - 26
                                    For Each b As Button In New Button() {btnNext, btnPage3, btnPage2, btnPage1, btnPrev}
                                        b.Location = New Point(xx, fy)
                                        xx -= 32
                                    Next
                                End Sub
        Controls.Add(cReg)

        ' ----- selected dispatch -----
        cSel = New FiCard
        RptKit.Spot(lblSelTitle, RptKit.F(10.5!, FontStyle.Bold), RptKit.Ink, 18, 12)
        RptKit.Spot(lblSelSub, RptKit.F(7.5!), RptKit.Muted, 18, 32)
        cSel.Controls.AddRange(New Control() {lblSelTitle, lblSelSub})
        AddSelRow("Vehicle / clerk", lblSelVehicle, 58, 0)
        AddSelRow("Contract / payment", lblSelContract, 58, 1)
        AddSelRow("Checkout / return inspection", lblSelInspection, 86, 0)
        AddSelRow("Post-return fleet status", lblSelFleetStatus, 86, 1)
        RptKit.StyleBtn(btnViewRecord, "👁  View authorized record", 200, 30, False)
        btnViewRecord.Location = New Point(18, 112)
        cSel.Controls.Add(btnViewRecord)
        Controls.Add(cSel)

        ' ----- safety panel -----
        pnlSafety = New Panel With {.BackColor = Color.FromArgb(252, 243, 228)}
        RptKit.Spot(lblSafetyTitle, RptKit.F(8.5!, FontStyle.Bold), Color.FromArgb(176, 112, 32), 12, 8)
        lblSafetyNote.AutoSize = False : lblSafetyNote.Font = RptKit.F(7.5!) : lblSafetyNote.ForeColor = RptKit.Muted
        lblSafetyNote.BackColor = Color.Transparent : lblSafetyNote.UseMnemonic = False
        pnlSafety.Controls.AddRange(New Control() {lblSafetyTitle, lblSafetyNote})
        AddHandler pnlSafety.Resize, Sub(s, e) lblSafetyNote.SetBounds(12, 28, pnlSafety.Width - 24, pnlSafety.Height - 32)
        Controls.Add(pnlSafety)

        AddHandler btnExport.Click, Sub(s, e) RaiseEvent ExportClicked()
        AddHandler btnPrintReport.Click, Sub(s, e) RaiseEvent PrintReportClicked()
        AddHandler btnViewRecord.Click, Sub(s, e) RaiseEvent ViewRecordClicked()

        Me.ResumeLayout(False)
        ArrangeLayout()
    End Sub

    Private Sub AddFilter(caption As String, ctl As Control)
        Dim cap As Label = RptKit.Cap(caption, RptKit.F(8, FontStyle.Bold), RptKit.Ink, 0, 0)
        If TypeOf ctl Is ComboBox Then RptKit.StyleCombo(DirectCast(ctl, ComboBox))
        If TypeOf ctl Is DateTimePicker Then ctl.Font = RptKit.F(9)
        cFilter.Controls.Add(cap) : cFilter.Controls.Add(ctl)
        _filterItems.Add(Tuple.Create(cap, ctl))
    End Sub

    Private Sub AddKpi(caption As String, valueLbl As Label, subLbl As Label)
        Dim c As FiCard = RptKit.NewKpi(caption, valueLbl, subLbl)
        valueLbl.Font = RptKit.F(20, FontStyle.Bold)
        valueLbl.Location = New Point(10, 28)
        subLbl.Location = New Point(12, 68)
        _kpis.Add(c)
        Controls.Add(c)
    End Sub

    ' two columns inside the selected-dispatch card (col 0 = left, col 1 = right)
    Private Sub AddSelRow(caption As String, value As Label, y As Integer, col As Integer)
        Dim cap As Label = RptKit.Cap(caption, RptKit.F(8), RptKit.Muted, 18, y)
        RptKit.Spot(value, RptKit.F(8.5!, FontStyle.Bold), RptKit.Ink, 0, y - 1)
        cSel.Controls.Add(cap) : cSel.Controls.Add(value)
        Dim place As Action = Sub()
                                  If col = 0 Then
                                      value.Left = cSel.Width \ 2 - 14 - value.Width
                                  Else
                                      cap.Left = cSel.Width \ 2 + 14
                                      value.Left = cSel.Width - 18 - value.Width
                                  End If
                              End Sub
        AddHandler cSel.Resize, Sub(s, e) place()
        AddHandler value.SizeChanged, Sub(s, e) place()
    End Sub
End Class