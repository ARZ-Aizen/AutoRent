' Daily Revenue Reports. All data spots are empty: fill them from your code.
' Needs ucVehicleDetails.vb (FiCard) and ucMaintenanceDetails.vb (MaintDraw pills) in the project.
' ucDispatchReports.vb also uses the RptKit module and the chart controls at the bottom of THIS file.
Public Class ucRevenueReports
    Inherits UserControl

    Public Event PrintClicked()
    Public Event ExportClicked()
    Public Event FilterChanged()
    Public Event QuickRangeClicked(rangeName As String)

    ' ---- header ----
    Public ReadOnly lblSubtitle As New Label
    Public ReadOnly btnPrint As New Button
    Public ReadOnly btnExport As New Button

    ' ---- filters ----
    Public ReadOnly dtpDate As New DateTimePicker
    Public ReadOnly cboCategory As New ComboBox
    Public ReadOnly cboPayment As New ComboBox
    Public ReadOnly cboStaff As New ComboBox
    Public ReadOnly cboStatus As New ComboBox
    Public ReadOnly lblRefreshing As New Label

    ' ---- KPI values ----
    Public ReadOnly lblCharges As New Label
    Public ReadOnly lblCollected As New Label
    Public ReadOnly lblOutstanding As New Label
    Public ReadOnly lblDepositsIn As New Label
    Public ReadOnly lblDepositsOut As New Label
    Public ReadOnly lblFees As New Label
    Public ReadOnly lblDiscounts As New Label
    ' ---- KPI sub texts (optional) ----
    Public ReadOnly lblChargesSub As New Label
    Public ReadOnly lblCollectedSub As New Label
    Public ReadOnly lblOutstandingSub As New Label
    Public ReadOnly lblDepositsInSub As New Label
    Public ReadOnly lblDepositsOutSub As New Label
    Public ReadOnly lblFeesSub As New Label
    Public ReadOnly lblDiscountsSub As New Label

    ' ---- charts ----
    Public ReadOnly lblTrendSub As New Label
    Public ReadOnly Trend As New BarChartCtl
    Public ReadOnly CategoryBars As New BarListCtl
    Public ReadOnly PaymentBars As New BarListCtl
    Public ReadOnly lblCvfCharges As New Label
    Public ReadOnly lblCvfFees As New Label
    Public ReadOnly lblCvfNet As New Label

    ' ---- transaction register ----
    Public ReadOnly lblRegisterSub As New Label
    Public ReadOnly lblReconciled As New Label
    Public ReadOnly dgvTransactions As New DataGridView
    Public ReadOnly lblDiscountNote As New Label
    Public ReadOnly lblShowing As New Label

    ' ---- bottom notes ----
    Public ReadOnly lblSummary As New Label
    Public ReadOnly lblSummaryNote As New Label

    Private cFilter, cTrend, cCat, cPay, cCvf, cReg As FiCard
    Private ReadOnly _kpis As New List(Of FiCard)
    Private ReadOnly _filterItems As New List(Of Tuple(Of Label, Control))
    Private ReadOnly _links As New List(Of Label)

    Public Sub New()
        Me.BackColor = RptKit.PageBg
        Me.AutoScroll = True
        Me.AutoScrollMinSize = New Size(960, 800)
        BuildUI()
    End Sub

    Private Sub ArrangeLayout()
        If cReg Is Nothing Then Return
        Dim w As Integer = Math.Max(ClientSize.Width, 960) - 48
        btnExport.Location = New Point(24 + w - btnExport.Width, 20)
        btnPrint.Location = New Point(btnExport.Left - 8 - btnPrint.Width, 20)
        cFilter.SetBounds(24, 96, w, 104)

        Dim kw As Integer = (w - 6 * 10) \ 7
        For i As Integer = 0 To _kpis.Count - 1
            _kpis(i).SetBounds(24 + i * (kw + 10), 214, kw, 84)
        Next

        Dim mw As Integer = w - 3 * 12
        Dim w1 As Integer = CInt(mw * 0.25), w2 As Integer = CInt(mw * 0.27), w3 As Integer = CInt(mw * 0.25)
        Dim w4 As Integer = mw - w1 - w2 - w3
        cTrend.SetBounds(24, 312, w1, 156)
        cCat.SetBounds(24 + w1 + 12, 312, w2, 156)
        cPay.SetBounds(24 + w1 + w2 + 24, 312, w3, 156)
        cCvf.SetBounds(24 + w1 + w2 + w3 + 36, 312, w4, 156)

        cReg.SetBounds(24, 484, w, 276)
        lblSummary.Location = New Point(24, 774)
        lblSummaryNote.Location = New Point(24, 794)
    End Sub

    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e)
        ArrangeLayout()
    End Sub

    Private Sub BuildUI()
        Me.SuspendLayout()
        Me.AutoScrollMinSize = New Size(960, 840)

        Controls.Add(RptKit.Cap("Daily Revenue Reports", RptKit.F(20, FontStyle.Bold), RptKit.Ink, 22, 14))
        RptKit.Spot(lblSubtitle, RptKit.F(9), RptKit.Muted, 26, 58)
        Controls.Add(lblSubtitle)
        RptKit.StyleBtn(btnPrint, "🖶  Print", 90, 34, False)
        RptKit.StyleBtn(btnExport, "⭳  Authorized CSV export", 190, 34, False)
        Controls.AddRange(New Control() {btnPrint, btnExport})

        ' ----- filters -----
        cFilter = New FiCard
        AddFilter("Date range", dtpDate)
        AddFilter("Vehicle Category", cboCategory)
        AddFilter("Payment Method", cboPayment)
        AddFilter("Staff", cboStaff)
        AddFilter("Transaction Status", cboStatus)
        dtpDate.Format = DateTimePickerFormat.Custom : dtpDate.CustomFormat = "dd MMM yyyy"
        cboCategory.Items.AddRange(New Object() {"All categories", "Sedan", "SUV", "Van", "Pickup"})
        cboPayment.Items.AddRange(New Object() {"All methods", "Card", "Bank transfer", "Cash"})
        cboStatus.Items.AddRange(New Object() {"All statuses", "Paid", "Partial"})
        cboStaff.Items.Add("All staff")
        For Each c As ComboBox In New ComboBox() {cboCategory, cboPayment, cboStaff, cboStatus}
            c.SelectedIndex = 0
        Next

        Dim x As Integer = 18
        For Each rng As String In New String() {"Today", "Yesterday", "Last 7 Days", "Last 30 Days", "Custom"}
            Dim lk As New Label With {.Text = rng, .AutoSize = True, .Cursor = Cursors.Hand, .UseMnemonic = False,
                                      .Font = RptKit.F(8, If(rng = "Today", FontStyle.Bold, FontStyle.Regular)),
                                      .ForeColor = RptKit.Maroon, .BackColor = Color.Transparent, .Location = New Point(x, 74)}
            Dim captured As String = rng
            AddHandler lk.Click, Sub(s, e) RaiseEvent QuickRangeClicked(captured)
            cFilter.Controls.Add(lk)
            _links.Add(lk)
            x += TextRenderer.MeasureText(rng, lk.Font).Width + 18
        Next
        RptKit.Spot(lblRefreshing, RptKit.F(7.5!), RptKit.Muted, 0, 76)
        cFilter.Controls.Add(lblRefreshing)
        RptKit.PinRight(cFilter, lblRefreshing, 18)

        AddHandler cFilter.Resize, Sub(s, e)
                                       Dim gap As Integer = 14
                                       Dim cw As Integer = (cFilter.Width - 36 - gap * 4) \ 5
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

        ' ----- KPI cards -----
        AddKpi("Total rental charges", lblCharges, lblChargesSub)
        AddKpi("Payments collected", lblCollected, lblCollectedSub)
        AddKpi("Outstanding balances", lblOutstanding, lblOutstandingSub)
        AddKpi("Deposits received", lblDepositsIn, lblDepositsInSub)
        AddKpi("Deposits refunded", lblDepositsOut, lblDepositsOutSub)
        AddKpi("Additional fees", lblFees, lblFeesSub)
        AddKpi("Discounts applied", lblDiscounts, lblDiscountsSub)

        ' ----- trend / category / payment / charges vs fees -----
        cTrend = New FiCard
        cTrend.Controls.Add(RptKit.Cap("Daily revenue trend", RptKit.F(10, FontStyle.Bold), RptKit.Ink, 14, 10))
        RptKit.Spot(lblTrendSub, RptKit.F(7.5!), RptKit.Muted, 14, 30)
        cTrend.Controls.Add(lblTrendSub)
        cTrend.Controls.Add(Trend)
        AddHandler cTrend.Resize, Sub(s, e) Trend.SetBounds(10, 50, cTrend.Width - 20, cTrend.Height - 58)
        Controls.Add(cTrend)

        cCat = New FiCard
        cCat.Controls.Add(RptKit.Cap("Revenue by category", RptKit.F(10, FontStyle.Bold), RptKit.Ink, 14, 10))
        cCat.Controls.Add(CategoryBars)
        AddHandler cCat.Resize, Sub(s, e) CategoryBars.SetBounds(14, 38, cCat.Width - 28, cCat.Height - 46)
        Controls.Add(cCat)

        cPay = New FiCard
        cPay.Controls.Add(RptKit.Cap("Payment collections", RptKit.F(10, FontStyle.Bold), RptKit.Ink, 14, 10))
        cPay.Controls.Add(PaymentBars)
        AddHandler cPay.Resize, Sub(s, e) PaymentBars.SetBounds(14, 38, cPay.Width - 28, cPay.Height - 46)
        Controls.Add(cPay)

        cCvf = New FiCard
        cCvf.Controls.Add(RptKit.Cap("Charges versus fees", RptKit.F(10, FontStyle.Bold), RptKit.Ink, 14, 10))
        cCvf.Controls.Add(RptKit.Cap("Rental charges", RptKit.F(8), RptKit.Muted, 14, 44))
        RptKit.Spot(lblCvfCharges, RptKit.F(8.5!, FontStyle.Bold), RptKit.Ink, 0, 43)
        cCvf.Controls.Add(lblCvfCharges) : RptKit.PinRight(cCvf, lblCvfCharges, 14)
        cCvf.Controls.Add(RptKit.Cap("Fees / discounts", RptKit.F(8), RptKit.Muted, 14, 70))
        RptKit.Spot(lblCvfFees, RptKit.F(8.5!, FontStyle.Bold), RptKit.Ink, 0, 69)
        cCvf.Controls.Add(lblCvfFees) : RptKit.PinRight(cCvf, lblCvfFees, 14)
        RptKit.Spot(lblCvfNet, RptKit.F(16, FontStyle.Bold), RptKit.Maroon, 14, 100)
        cCvf.Controls.Add(lblCvfNet)
        Controls.Add(cCvf)

        ' ----- transaction register -----
        cReg = New FiCard
        cReg.Controls.Add(RptKit.Cap("Transaction register", RptKit.F(11, FontStyle.Bold), RptKit.Ink, 18, 12))
        RptKit.Spot(lblRegisterSub, RptKit.F(7.5!), RptKit.Muted, 18, 34)
        cReg.Controls.Add(lblRegisterSub)
        lblReconciled.AutoSize = False : lblReconciled.Size = New Size(130, 20)
        lblReconciled.Font = RptKit.F(7.5!, FontStyle.Bold) : lblReconciled.ForeColor = Color.FromArgb(46, 110, 60)
        lblReconciled.BackColor = Color.FromArgb(232, 240, 233) : lblReconciled.TextAlign = ContentAlignment.MiddleCenter
        lblReconciled.Top = 14 : lblReconciled.UseMnemonic = False
        cReg.Controls.Add(lblReconciled) : RptKit.PinRight(cReg, lblReconciled, 18)

        RptKit.StyleGrid(dgvTransactions, "Transaction ID", "Booking ID", "Customer", "Date", "Rental Charge",
                         "Additional Fees", "Deposit", "Amount Collected", "Payment Method", "Status")
        AddHandler dgvTransactions.CellFormatting, Sub(s, e)
                                                       If e.RowIndex >= 0 AndAlso e.ColumnIndex = 0 Then
                                                           e.CellStyle.ForeColor = RptKit.Maroon
                                                           e.CellStyle.Font = RptKit.F(8.5!, FontStyle.Bold)
                                                       End If
                                                   End Sub
        AddHandler dgvTransactions.CellPainting, Sub(s, e)
                                                     If e.RowIndex >= 0 AndAlso e.ColumnIndex = 9 Then MaintDraw.Pill(e)
                                                 End Sub
        cReg.Controls.Add(dgvTransactions)
        RptKit.Spot(lblDiscountNote, RptKit.F(7.5!), RptKit.Muted, 18, 0)
        cReg.Controls.Add(lblDiscountNote)
        RptKit.Spot(lblShowing, RptKit.F(7.5!, FontStyle.Bold), RptKit.Ink, 0, 0)
        cReg.Controls.Add(lblShowing)
        AddHandler lblShowing.SizeChanged, Sub(s, e) lblShowing.Left = cReg.Width - 18 - lblShowing.Width
        AddHandler cReg.Resize, Sub(s, e)
                                    dgvTransactions.SetBounds(1, 58, cReg.Width - 2, cReg.Height - 58 - 34)
                                    lblDiscountNote.Top = cReg.Height - 24
                                    lblShowing.Top = cReg.Height - 24
                                    lblShowing.Left = cReg.Width - 18 - lblShowing.Width
                                End Sub
        Controls.Add(cReg)

        RptKit.Spot(lblSummary, RptKit.F(8.5!, FontStyle.Bold), RptKit.Ink, 24, 774)
        RptKit.Spot(lblSummaryNote, RptKit.F(8), RptKit.Muted, 24, 794)
        Controls.AddRange(New Control() {lblSummary, lblSummaryNote})

        Me.ResumeLayout(False)
        ArrangeLayout()
        AddHandler btnPrint.Click, Sub(s, e) RaiseEvent PrintClicked()
        AddHandler btnExport.Click, Sub(s, e) RaiseEvent ExportClicked()
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
        _kpis.Add(c)
        Controls.Add(c)
    End Sub
End Class

' ======================================================================
'  Shared helpers (used by ucRevenueReports and ucDispatchReports)
' ======================================================================
Friend Module RptKit
    Public ReadOnly Maroon As Color = Color.FromArgb(122, 0, 16)
    Public ReadOnly Ink As Color = Color.FromArgb(38, 38, 38)
    Public ReadOnly Muted As Color = Color.FromArgb(120, 120, 120)
    Public ReadOnly BorderCol As Color = Color.FromArgb(228, 225, 220)
    Public ReadOnly PageBg As Color = Color.FromArgb(245, 244, 242)

    Public Function F(size As Single, Optional style As FontStyle = FontStyle.Regular) As Font
        Return New Font("Segoe UI", size, style)
    End Function

    Public Function Cap(text As String, f As Font, c As Color, x As Integer, y As Integer) As Label
        Return New Label With {.Text = text, .Font = f, .ForeColor = c, .BackColor = Color.Transparent,
                               .AutoSize = True, .Location = New Point(x, y), .UseMnemonic = False}
    End Function

    Public Sub Spot(l As Label, f As Font, c As Color, x As Integer, y As Integer)
        l.Font = f : l.ForeColor = c : l.BackColor = Color.Transparent
        l.AutoSize = True : l.Location = New Point(x, y) : l.UseMnemonic = False
    End Sub

    Public Sub PinRight(card As Control, ctl As Control, margin As Integer)
        Dim place As Action = Sub() ctl.Left = card.Width - margin - ctl.Width
        AddHandler card.Resize, Sub(s, e) place()
        AddHandler ctl.SizeChanged, Sub(s, e) place()
    End Sub

    Public Sub StyleBtn(b As Button, text As String, w As Integer, h As Integer, primary As Boolean)
        b.Text = text : b.Size = New Size(w, h) : b.FlatStyle = FlatStyle.Flat
        b.Font = F(9) : b.Cursor = Cursors.Hand : b.UseVisualStyleBackColor = False : b.UseMnemonic = False
        b.BackColor = If(primary, Maroon, Color.White)
        b.ForeColor = If(primary, Color.White, Ink)
        b.FlatAppearance.BorderColor = If(primary, Maroon, BorderCol)
        b.FlatAppearance.BorderSize = 1
    End Sub

    Public Sub StyleCombo(c As ComboBox)
        c.DropDownStyle = ComboBoxStyle.DropDownList
        c.FlatStyle = FlatStyle.Flat : c.BackColor = Color.White : c.Font = F(9)
    End Sub

    Public Function NewKpi(caption As String, valueLbl As Label, subLbl As Label) As FiCard
        Dim c As New FiCard
        c.Controls.Add(Cap(caption, F(7.5!), Ink, 12, 10))
        Spot(valueLbl, F(17, FontStyle.Bold), Maroon, 10, 28)
        Spot(subLbl, F(7), Muted, 12, 62)
        c.Controls.Add(valueLbl) : c.Controls.Add(subLbl)
        Return c
    End Function

    Public Sub StyleGrid(g As DataGridView, ParamArray headers() As String)
        g.BackgroundColor = Color.White : g.BorderStyle = BorderStyle.None
        g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal : g.GridColor = BorderCol
        g.RowHeadersVisible = False : g.AllowUserToAddRows = False : g.AllowUserToDeleteRows = False
        g.AllowUserToResizeRows = False : g.ReadOnly = True : g.MultiSelect = False
        g.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        g.EnableHeadersVisualStyles = False
        g.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        g.ColumnHeadersHeight = 32 : g.RowTemplate.Height = 36
        g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        g.ScrollBars = ScrollBars.Vertical
        g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(250, 249, 247)
        g.ColumnHeadersDefaultCellStyle.ForeColor = Muted
        g.ColumnHeadersDefaultCellStyle.Font = F(7.5!)
        g.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(250, 249, 247)
        g.ColumnHeadersDefaultCellStyle.SelectionForeColor = Muted
        g.ColumnHeadersDefaultCellStyle.Padding = New Padding(10, 0, 0, 0)
        g.DefaultCellStyle.Font = F(8) : g.DefaultCellStyle.ForeColor = Ink
        g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(250, 249, 247)
        g.DefaultCellStyle.SelectionForeColor = Ink
        g.DefaultCellStyle.Padding = New Padding(10, 0, 0, 0)
        g.DefaultCellStyle.WrapMode = DataGridViewTriState.True
        For i As Integer = 0 To headers.Length - 1
            g.Columns.Add("col" & i, headers(i))
        Next
    End Sub
End Module

''' <summary>Vertical bars; the last bar is highlighted in maroon. Call SetData(labels, values).</summary>
Public Class BarChartCtl
    Inherits Control

    Private _labels() As String = New String() {}
    Private _values() As Double = New Double() {}

    Public Sub New()
        SetStyle(ControlStyles.UserPaint Or ControlStyles.AllPaintingInWmPaint Or
                 ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw, True)
        Me.BackColor = Color.White
    End Sub

    Public Sub SetData(labels() As String, values() As Double)
        _labels = labels : _values = values
        Invalidate()
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim g As Graphics = e.Graphics
        g.Clear(Color.White)
        If _values.Length = 0 Then Return
        Dim maxV As Double = _values.Max()
        If maxV <= 0 Then maxV = 1
        Dim left As Integer = 30, bottom As Integer = Height - 16
        Dim plotH As Integer = bottom - 4
        Dim slot As Single = (Width - left) / CSng(_values.Length)

        Using f As New Font("Segoe UI", 6.5!), tb As New SolidBrush(Color.FromArgb(150, 150, 150)),
              grayBr As New SolidBrush(Color.FromArgb(226, 223, 219)), maroonBr As New SolidBrush(Color.FromArgb(122, 0, 16)),
              mid As New StringFormat With {.Alignment = StringAlignment.Center}
            g.DrawString("$" & If(maxV >= 1000, (maxV / 1000).ToString("0.#") & "k", maxV.ToString("0")), f, tb, 0, 0)
            g.DrawString("$0", f, tb, 0, bottom - 10)
            For i As Integer = 0 To _values.Length - 1
                Dim h As Single = CSng(_values(i) / maxV * (plotH - 10))
                Dim bw As Single = slot * 0.62F
                Dim x As Single = left + i * slot + (slot - bw) / 2
                g.FillRectangle(If(i = _values.Length - 1, maroonBr, grayBr), x, bottom - h, bw, h)
                If i < _labels.Length Then g.DrawString(_labels(i), f, tb, New RectangleF(left + i * slot, bottom + 2, slot, 12), mid)
            Next
        End Using
    End Sub
End Class

''' <summary>Rows of "label ... value" with a thin bar. Call SetItems.</summary>
Public Class BarListCtl
    Inherits Control

    Private ReadOnly _items As New List(Of Tuple(Of String, String, Double))

    Public Sub New()
        SetStyle(ControlStyles.UserPaint Or ControlStyles.AllPaintingInWmPaint Or
                 ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw, True)
        Me.BackColor = Color.White
    End Sub

    ''' <summary>fraction is 0..1 (bar length).</summary>
    Public Sub SetItems(items As IEnumerable(Of Tuple(Of String, String, Double)))
        _items.Clear()
        _items.AddRange(items)
        Invalidate()
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim g As Graphics = e.Graphics
        g.Clear(Color.White)
        If _items.Count = 0 Then Return
        Dim rowH As Single = Height / CSng(Math.Max(_items.Count, 3))
        Using f As New Font("Segoe UI", 7.5!), fb As New Font("Segoe UI", 7.5!, FontStyle.Bold),
              muted As New SolidBrush(Color.FromArgb(120, 120, 120)), ink As New SolidBrush(Color.FromArgb(38, 38, 38)),
              track As New SolidBrush(Color.FromArgb(240, 238, 234)), bar As New SolidBrush(Color.FromArgb(122, 0, 16)),
              far As New StringFormat With {.Alignment = StringAlignment.Far}
            For i As Integer = 0 To _items.Count - 1
                Dim y As Single = i * rowH + 2
                g.DrawString(_items(i).Item1, f, muted, 0, y)
                g.DrawString(_items(i).Item2, fb, ink, New RectangleF(0, y, Width, 14), far)
                g.FillRectangle(track, 0, y + 16, Width, 3)
                g.FillRectangle(bar, 0, y + 16, CSng(Width * Math.Max(0, Math.Min(1, _items(i).Item3))), 3)
            Next
        End Using
    End Sub
End Class